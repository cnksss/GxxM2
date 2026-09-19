object frmHumanInfo: TfrmHumanInfo
  Left = 445
  Top = 471
  BorderIcons = [biSystemMenu, biMinimize]
  BorderStyle = bsSingle
  Caption = #20154#29289#23646#24615
  ClientHeight = 305
  ClientWidth = 645
  Color = clBtnFace
  Font.Charset = GB2312_CHARSET
  Font.Color = clWindowText
  Font.Height = -12
  Font.Name = #23435#20307
  Font.Style = []
  ShowHint = True
  OnCreate = FormCreate
  TextHeight = 12
  object lbl1: TLabel
    Left = 96
    Top = 280
    Width = 60
    Height = 12
    Caption = #20154#29289#29366#24577#65306
  end
  object PageControl1: TPageControl
    Left = 8
    Top = 8
    Width = 633
    Height = 257
    ActivePage = tsBaseInfo
    TabOrder = 0
    object tsBaseInfo: TTabSheet
      Caption = #20154#29289#20449#24687
      object GroupBox1: TGroupBox
        Left = 8
        Top = 13
        Width = 201
        Height = 193
        Caption = #26597#30475#20449#24687
        TabOrder = 0
        object Label1: TLabel
          Left = 8
          Top = 19
          Width = 54
          Height = 12
          Caption = #20154#29289#21517#31216':'
        end
        object Label2: TLabel
          Left = 8
          Top = 43
          Width = 54
          Height = 12
          Caption = #25152#22312#22320#22270':'
        end
        object Label3: TLabel
          Left = 8
          Top = 67
          Width = 54
          Height = 12
          Caption = #25152#22312#24231#26631':'
        end
        object Label4: TLabel
          Left = 8
          Top = 91
          Width = 54
          Height = 12
          Caption = #30331#24405#24080#21495':'
        end
        object Label5: TLabel
          Left = 8
          Top = 115
          Width = 42
          Height = 12
          Caption = #30331#24405'IP:'
        end
        object Label6: TLabel
          Left = 8
          Top = 139
          Width = 54
          Height = 12
          Caption = #30331#24405#26102#38388':'
        end
        object Label7: TLabel
          Left = 8
          Top = 163
          Width = 54
          Height = 12
          Caption = #22312#32447#26102#38271':'
        end
        object EditName: TEdit
          Left = 64
          Top = 16
          Width = 129
          Height = 20
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ReadOnly = True
          TabOrder = 0
          Text = 'EditName'
        end
        object EditMap: TEdit
          Left = 64
          Top = 40
          Width = 129
          Height = 20
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ReadOnly = True
          TabOrder = 1
          Text = 'Edit1'
        end
        object EditXY: TEdit
          Left = 64
          Top = 64
          Width = 129
          Height = 20
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ReadOnly = True
          TabOrder = 2
          Text = 'Edit1'
        end
        object EditAccount: TEdit
          Left = 64
          Top = 88
          Width = 129
          Height = 20
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ReadOnly = True
          TabOrder = 3
          Text = 'Edit1'
        end
        object EditIPaddr: TEdit
          Left = 64
          Top = 112
          Width = 129
          Height = 20
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ReadOnly = True
          TabOrder = 4
          Text = 'Edit1'
        end
        object EditLogonTime: TEdit
          Left = 64
          Top = 136
          Width = 129
          Height = 20
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ReadOnly = True
          TabOrder = 5
          Text = 'Edit1'
        end
        object EditLogonLong: TEdit
          Left = 64
          Top = 160
          Width = 129
          Height = 20
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ReadOnly = True
          TabOrder = 6
          Text = 'Edit1'
        end
      end
      object GroupBox11: TGroupBox
        Left = 216
        Top = 16
        Width = 401
        Height = 185
        Caption = #31163#32447#25346#26426
        TabOrder = 1
        object Label20: TLabel
          Left = 8
          Top = 20
          Width = 60
          Height = 12
          Caption = #33258#21160#21457#35328#65306
        end
        object EditSayMsg: TEdit
          Left = 72
          Top = 16
          Width = 321
          Height = 20
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          TabOrder = 0
          Text = 'EditSayMsg'
        end
      end
    end
    object TabSheet2: TTabSheet
      Caption = #26222#36890#25968#25454
      ImageIndex = 1
      object GroupBox2: TGroupBox
        Left = 8
        Top = 8
        Width = 207
        Height = 217
        Caption = #21487#35843#23646#24615
        TabOrder = 0
        object Label12: TLabel
          Left = 8
          Top = 18
          Width = 42
          Height = 12
          Caption = #31561'  '#32423':'
        end
        object Label8: TLabel
          Left = 8
          Top = 42
          Width = 42
          Height = 12
          Caption = #37329#24065#25968':'
        end
        object Label9: TLabel
          Left = 8
          Top = 66
          Width = 42
          Height = 12
          Caption = 'PK'#28857#25968':'
        end
        object Label10: TLabel
          Left = 8
          Top = 90
          Width = 54
          Height = 12
          Caption = #24403#21069#32463#39564':'
        end
        object Label21: TLabel
          Left = 8
          Top = 114
          Width = 54
          Height = 12
          Caption = #21319#32423#32463#39564':'
        end
        object Label22: TLabel
          Left = 8
          Top = 138
          Width = 54
          Height = 12
          Caption = #20869#21151#31561#32423':'
        end
        object Label34: TLabel
          Left = 8
          Top = 162
          Width = 54
          Height = 12
          Caption = #20869#21151#32463#39564':'
        end
        object Label23: TLabel
          Left = 8
          Top = 186
          Width = 54
          Height = 12
          Caption = #21319#32423#32463#39564':'
        end
        object EditGold: TSpinEditLongWord
          Left = 68
          Top = 39
          Width = 131
          Height = 21
          Increment = 1000
          MaxValue = 0
          MinValue = 0
          TabOrder = 0
          Value = 10
        end
        object EditPKPoint: TSpinEditEx
          Left = 68
          Top = 63
          Width = 131
          Height = 21
          Increment = 50
          MaxValue = 20000
          MinValue = 0
          TabOrder = 1
          Value = 10
        end
        object EditExp: TSpinEditEx
          Left = 68
          Top = 87
          Width = 131
          Height = 21
          Enabled = False
          MaxValue = 2100000000
          MinValue = 0
          TabOrder = 2
          Value = 10
        end
        object EditMaxExp: TSpinEditEx
          Left = 68
          Top = 111
          Width = 131
          Height = 21
          Enabled = False
          MaxValue = 2100000000
          MinValue = 0
          TabOrder = 3
          Value = 10
        end
        object EditNGLevel: TSpinEditEx
          Left = 68
          Top = 135
          Width = 131
          Height = 21
          MaxValue = 65535
          MinValue = 0
          TabOrder = 4
          Value = 10
        end
        object EditNGExp: TSpinEditEx
          Left = 68
          Top = 159
          Width = 131
          Height = 21
          Hint = #24403#21069#20869#21151#32463#39564
          Enabled = False
          MaxValue = 2100000000
          MinValue = 0
          TabOrder = 6
          Value = 0
        end
        object EditNGMaxExp: TSpinEditEx
          Left = 68
          Top = 183
          Width = 131
          Height = 21
          Enabled = False
          MaxValue = 2100000000
          MinValue = 0
          TabOrder = 5
          Value = 10
        end
        object EditLevel: TSpinEditLongWord
          Left = 68
          Top = 14
          Width = 131
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 7
          Value = 0
        end
      end
      object GroupBox6: TGroupBox
        Left = 409
        Top = 8
        Width = 153
        Height = 73
        Caption = #20154#29289#29366#24577
        TabOrder = 1
        object CheckBoxGameMaster: TCheckBox
          Left = 8
          Top = 16
          Width = 113
          Height = 17
          Caption = 'GM'#27169#24335
          TabOrder = 0
        end
        object CheckBoxSuperMan: TCheckBox
          Left = 8
          Top = 32
          Width = 113
          Height = 17
          Caption = #26080#25932#27169#24335
          TabOrder = 1
        end
        object CheckBoxObserver: TCheckBox
          Left = 8
          Top = 48
          Width = 113
          Height = 17
          Caption = #38544#36523#27169#24335
          TabOrder = 2
        end
      end
      object GroupBox9: TGroupBox
        Left = 220
        Top = 8
        Width = 183
        Height = 217
        Caption = #21487#35843#23646#24615
        TabOrder = 2
        object Label26: TLabel
          Left = 8
          Top = 18
          Width = 42
          Height = 12
          Caption = #28216#25103#24065':'
        end
        object Label27: TLabel
          Left = 8
          Top = 42
          Width = 42
          Height = 12
          Caption = #28216#25103#28857':'
        end
        object Label28: TLabel
          Left = 8
          Top = 115
          Width = 42
          Height = 12
          Caption = #22768#26395#28857':'
        end
        object Label29: TLabel
          Left = 8
          Top = 164
          Width = 54
          Height = 12
          Caption = #23646#24615#28857#19968':'
        end
        object Label19: TLabel
          Left = 8
          Top = 188
          Width = 54
          Height = 12
          Hint = #24050#20998#37197#23646#24615#28857#25968#12290
          Caption = #23646#24615#28857#20108':'
        end
        object Label24: TLabel
          Left = 8
          Top = 67
          Width = 42
          Height = 12
          Caption = #37329#21018#30707':'
        end
        object Label25: TLabel
          Left = 8
          Top = 91
          Width = 42
          Height = 12
          Caption = #28789'  '#31526':'
        end
        object Label30: TLabel
          Left = 8
          Top = 140
          Width = 42
          Height = 12
          Caption = #33635'  '#35465':'
        end
        object EditGameGold: TSpinEditLongWord
          Left = 68
          Top = 15
          Width = 109
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 0
          Value = 10
        end
        object EditGamePoint: TSpinEditLongWord
          Left = 68
          Top = 39
          Width = 109
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 1
          Value = 10
        end
        object EditCreditPoint: TSpinEditEx
          Left = 68
          Top = 112
          Width = 109
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 2
          Value = 10
        end
        object EditBonusPoint: TSpinEditEx
          Left = 68
          Top = 161
          Width = 109
          Height = 21
          Hint = #26410#20998#37197#23646#24615#28857
          MaxValue = 0
          MinValue = 0
          TabOrder = 3
          Value = 10
        end
        object EditEditBonusPointUsed: TSpinEditEx
          Left = 68
          Top = 185
          Width = 109
          Height = 21
          Hint = #26410#20998#37197#23646#24615#28857
          Enabled = False
          MaxValue = 0
          MinValue = 0
          TabOrder = 4
          Value = 10
        end
        object seGameDiamond: TSpinEditLongWord
          Left = 68
          Top = 64
          Width = 109
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 5
          Value = 10
        end
        object seGameGird: TSpinEditLongWord
          Left = 68
          Top = 88
          Width = 109
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 6
          Value = 10
        end
        object EditGameGlory: TSpinEditEx
          Left = 68
          Top = 137
          Width = 109
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 7
          Value = 10
        end
      end
      object GroupBox4: TGroupBox
        Left = 409
        Top = 88
        Width = 153
        Height = 81
        Caption = #20154#29289#29190#29575
        TabOrder = 3
        object lbl2: TLabel
          Left = 8
          Top = 21
          Width = 84
          Height = 12
          Caption = #20803#32032#29190#29575#22686#21152#65306
        end
        object Label31: TLabel
          Left = 8
          Top = 45
          Width = 84
          Height = 12
          Caption = #21629#20196#29190#29575#20493#25968#65306
        end
        object edtNewValue11: TEdit
          Left = 87
          Top = 17
          Width = 57
          Height = 20
          ReadOnly = True
          TabOrder = 0
        end
        object edtKillMonBurstRate: TEdit
          Left = 87
          Top = 41
          Width = 57
          Height = 20
          ReadOnly = True
          TabOrder = 1
        end
      end
    end
    object TabSheet3: TTabSheet
      Caption = #23646#24615#28857
      ImageIndex = 2
      object GroupBox3: TGroupBox
        Left = 8
        Top = 8
        Width = 153
        Height = 193
        Caption = #20154#29289#23646#24615
        TabOrder = 0
        object Label11: TLabel
          Left = 8
          Top = 19
          Width = 30
          Height = 12
          Caption = #38450#24481':'
        end
        object Label13: TLabel
          Left = 8
          Top = 43
          Width = 30
          Height = 12
          Caption = #39764#38450':'
        end
        object Label14: TLabel
          Left = 8
          Top = 67
          Width = 42
          Height = 12
          Caption = #25915#20987#21147':'
        end
        object Label15: TLabel
          Left = 8
          Top = 91
          Width = 30
          Height = 12
          Caption = #39764#27861':'
        end
        object Label16: TLabel
          Left = 8
          Top = 115
          Width = 30
          Height = 12
          Caption = #36947#26415':'
        end
        object Label17: TLabel
          Left = 8
          Top = 139
          Width = 42
          Height = 12
          Caption = #29983#21629#20540':'
        end
        object Label18: TLabel
          Left = 8
          Top = 163
          Width = 42
          Height = 12
          Caption = #39764#27861#20540':'
        end
        object EditAC: TEdit
          Left = 56
          Top = 16
          Width = 81
          Height = 20
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ReadOnly = True
          TabOrder = 0
          Text = 'EditName'
        end
        object EditMAC: TEdit
          Left = 56
          Top = 40
          Width = 81
          Height = 20
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ReadOnly = True
          TabOrder = 1
          Text = 'EditName'
        end
        object EditDC: TEdit
          Left = 56
          Top = 64
          Width = 81
          Height = 20
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ReadOnly = True
          TabOrder = 2
          Text = 'EditName'
        end
        object EditMC: TEdit
          Left = 56
          Top = 88
          Width = 81
          Height = 20
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ReadOnly = True
          TabOrder = 3
          Text = 'EditName'
        end
        object EditSC: TEdit
          Left = 56
          Top = 112
          Width = 81
          Height = 20
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ReadOnly = True
          TabOrder = 4
          Text = 'EditName'
        end
        object EditHP: TEdit
          Left = 56
          Top = 136
          Width = 81
          Height = 20
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ReadOnly = True
          TabOrder = 5
          Text = 'EditName'
        end
        object EditMP: TEdit
          Left = 56
          Top = 160
          Width = 81
          Height = 20
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ReadOnly = True
          TabOrder = 6
          Text = 'EditName'
        end
      end
    end
    object TabSheet4: TTabSheet
      BorderWidth = 4
      Caption = #35013#22791#29289#21697
      ImageIndex = 3
      object pgc1: TPageControl
        Left = 0
        Top = 0
        Width = 617
        Height = 221
        ActivePage = ts3
        Align = alClient
        TabOrder = 0
        object ts3: TTabSheet
          BorderWidth = 4
          Caption = #36523#19978#35013#22791
          object GridUserItem: TStringGrid
            Left = 0
            Top = 0
            Width = 601
            Height = 185
            Align = alClient
            ColCount = 10
            DefaultColWidth = 55
            DefaultRowHeight = 20
            RowCount = 30
            Options = [goFixedVertLine, goFixedHorzLine, goVertLine, goHorzLine, goRangeSelect, goColSizing, goRowSelect]
            TabOrder = 0
            ColWidths = (
              80
              67
              63
              68
              45
              45
              44
              43
              46
              67)
            RowHeights = (
              20
              20
              20
              20
              20
              20
              20
              20
              20
              20
              20
              20
              20
              20
              20
              20
              20
              20
              20
              20
              20
              20
              20
              20
              20
              20
              20
              20
              20
              20)
          end
        end
        object ts4: TTabSheet
          BorderWidth = 4
          Caption = #39318#39280#30418#35013#22791
          ImageIndex = 1
          object GridJewelryBoxItems: TStringGrid
            Left = 0
            Top = 0
            Width = 601
            Height = 185
            Align = alClient
            ColCount = 10
            DefaultColWidth = 55
            DefaultRowHeight = 18
            RowCount = 7
            Options = [goFixedVertLine, goFixedHorzLine, goVertLine, goHorzLine, goRangeSelect, goColSizing, goRowSelect]
            TabOrder = 0
            ColWidths = (
              55
              67
              63
              68
              45
              45
              44
              43
              46
              88)
            RowHeights = (
              18
              18
              18
              18
              18
              18
              18)
          end
        end
        object ts5: TTabSheet
          BorderWidth = 4
          Caption = #31070#20305#34955#35013#22791
          ImageIndex = 2
          object GridGodBlessItems: TStringGrid
            Left = 0
            Top = 0
            Width = 601
            Height = 185
            Align = alClient
            ColCount = 10
            DefaultColWidth = 55
            DefaultRowHeight = 18
            RowCount = 13
            Options = [goFixedVertLine, goFixedHorzLine, goVertLine, goHorzLine, goRangeSelect, goColSizing, goRowSelect]
            TabOrder = 0
            ColWidths = (
              55
              67
              63
              68
              45
              45
              44
              43
              46
              88)
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
        end
      end
    end
    object TabSheet5: TTabSheet
      BorderWidth = 4
      Caption = #32972#21253#29289#21697
      ImageIndex = 4
      object GridBagItem: TStringGrid
        Left = 0
        Top = 0
        Width = 617
        Height = 221
        Align = alClient
        ColCount = 10
        DefaultColWidth = 55
        DefaultRowHeight = 18
        RowCount = 14
        Options = [goFixedVertLine, goFixedHorzLine, goVertLine, goHorzLine, goRangeSelect, goColSizing, goRowSelect]
        TabOrder = 0
        ColWidths = (
          55
          67
          63
          68
          45
          45
          44
          43
          46
          88)
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
          18)
      end
    end
    object TabSheet6: TTabSheet
      BorderWidth = 2
      Caption = #20179#24211#29289#21697
      ImageIndex = 5
      object tbcStorage: TTabControl
        Left = 0
        Top = 0
        Width = 621
        Height = 225
        Align = alClient
        Style = tsFlatButtons
        TabOrder = 0
        Tabs.Strings = (
          #20179#24211'1'
          #20179#24211'2'
          #20179#24211'3'
          #20179#24211'4')
        TabIndex = 0
        object GridStorageItem: TStringGrid
          Left = 4
          Top = 27
          Width = 613
          Height = 194
          Align = alClient
          ColCount = 10
          DefaultColWidth = 55
          DefaultRowHeight = 18
          FixedCols = 0
          RowCount = 14
          Options = [goFixedVertLine, goFixedHorzLine, goVertLine, goHorzLine, goRangeSelect, goColSizing, goRowSelect]
          TabOrder = 0
          ColWidths = (
            55
            67
            63
            67
            45
            45
            44
            43
            46
            89)
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
            18)
        end
      end
    end
    object TabSheet7: TTabSheet
      BorderWidth = 4
      Caption = #26080#38480#20179#29289#21697
      ImageIndex = 6
      object GridStorageItemEx: TStringGrid
        Left = 0
        Top = 0
        Width = 617
        Height = 221
        Align = alClient
        ColCount = 10
        DefaultColWidth = 55
        DefaultRowHeight = 18
        FixedCols = 0
        RowCount = 14
        Options = [goFixedVertLine, goFixedHorzLine, goVertLine, goHorzLine, goRangeSelect, goColSizing, goRowSelect]
        TabOrder = 0
        ColWidths = (
          55
          67
          63
          67
          45
          45
          44
          43
          46
          89)
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
          18)
      end
    end
    object tsVarU: TTabSheet
      BorderWidth = 4
      Caption = 'U'#21464#37327
      ImageIndex = 7
      object strGridVarU: TStringGrid
        Left = 0
        Top = 0
        Width = 617
        Height = 195
        Align = alClient
        ColCount = 2
        DefaultRowHeight = 20
        RowCount = 101
        Options = [goFixedVertLine, goFixedHorzLine, goVertLine, goHorzLine, goRangeSelect, goEditing]
        TabOrder = 0
        OnSetEditText = strGridVarUSetEditText
        ColWidths = (
          48
          546)
        RowHeights = (
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20)
      end
      object Panel1: TPanel
        Left = 0
        Top = 195
        Width = 617
        Height = 26
        Align = alBottom
        BevelOuter = bvNone
        TabOrder = 1
        object btnSaveU: TButton
          Left = 519
          Top = 1
          Width = 97
          Height = 25
          Caption = #20445#23384'U'#21464#37327#20462#25913
          TabOrder = 0
          OnClick = btnSaveUClick
        end
      end
    end
    object tsVarT: TTabSheet
      BorderWidth = 4
      Caption = 'T'#21464#37327
      ImageIndex = 8
      object strGridVarT: TStringGrid
        Left = 0
        Top = 0
        Width = 617
        Height = 195
        Align = alClient
        ColCount = 2
        DefaultRowHeight = 20
        RowCount = 101
        Options = [goFixedVertLine, goFixedHorzLine, goVertLine, goHorzLine, goRangeSelect, goEditing]
        TabOrder = 0
        OnSetEditText = strGridVarTSetEditText
        ColWidths = (
          48
          546)
        RowHeights = (
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20)
      end
      object pnl1: TPanel
        Left = 0
        Top = 195
        Width = 617
        Height = 26
        Align = alBottom
        BevelOuter = bvNone
        TabOrder = 1
        object btnSaveT: TButton
          Left = 519
          Top = 1
          Width = 97
          Height = 25
          Caption = #20445#23384'T'#21464#37327#20462#25913
          TabOrder = 0
          OnClick = btnSaveTClick
        end
      end
    end
    object TabSheet1: TTabSheet
      BorderWidth = 4
      Caption = 'J'#21464#37327
      ImageIndex = 9
      object Panel2: TPanel
        Left = 0
        Top = 195
        Width = 617
        Height = 26
        Align = alBottom
        BevelOuter = bvNone
        TabOrder = 0
        object btnSaveJ: TButton
          Left = 519
          Top = 1
          Width = 97
          Height = 25
          Caption = #20445#23384'J'#21464#37327#20462#25913
          TabOrder = 0
          OnClick = btnSaveJClick
        end
      end
      object strGridVarJ: TStringGrid
        Left = 0
        Top = 0
        Width = 617
        Height = 195
        Align = alClient
        ColCount = 2
        DefaultRowHeight = 20
        RowCount = 101
        Options = [goFixedVertLine, goFixedHorzLine, goVertLine, goHorzLine, goRangeSelect, goEditing]
        TabOrder = 1
        OnSetEditText = strGridVarJSetEditText
        ColWidths = (
          48
          546)
        RowHeights = (
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20)
      end
    end
    object TabSheet8: TTabSheet
      BorderWidth = 4
      Caption = 'Z'#21464#37327
      ImageIndex = 10
      object Panel3: TPanel
        Left = 0
        Top = 195
        Width = 617
        Height = 26
        Align = alBottom
        BevelOuter = bvNone
        TabOrder = 0
        object btnSaveZ: TButton
          Left = 519
          Top = 1
          Width = 97
          Height = 25
          Caption = #20445#23384'T'#21464#37327#20462#25913
          TabOrder = 0
          OnClick = btnSaveZClick
        end
      end
      object strGridVarZ: TStringGrid
        Left = 0
        Top = 0
        Width = 617
        Height = 195
        Align = alClient
        ColCount = 2
        DefaultRowHeight = 20
        RowCount = 101
        Options = [goFixedVertLine, goFixedHorzLine, goVertLine, goHorzLine, goRangeSelect, goEditing]
        TabOrder = 1
        OnSetEditText = strGridVarZSetEditText
        ColWidths = (
          48
          546)
        RowHeights = (
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20
          20)
      end
    end
  end
  object ButtonSave: TButton
    Left = 544
    Top = 273
    Width = 96
    Height = 25
    Caption = #20462#25913#20154#29289#25968#25454
    TabOrder = 1
    OnClick = ButtonSaveClick
  end
  object EditHumanStatus: TEdit
    Left = 152
    Top = 276
    Width = 105
    Height = 20
    ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
    ReadOnly = True
    TabOrder = 2
  end
  object ButtonKick: TButton
    Left = 264
    Top = 273
    Width = 65
    Height = 25
    Caption = #36386#19979#32447
    TabOrder = 3
    OnClick = ButtonKickClick
  end
  object CheckBoxMonitor: TCheckBox
    Left = 8
    Top = 277
    Width = 73
    Height = 17
    Caption = #33258#21160#21047#26032
    TabOrder = 4
    OnClick = CheckBoxMonitorClick
  end
  object Timer: TTimer
    Enabled = False
    OnTimer = TimerTimer
    Left = 456
    Top = 224
  end
end
