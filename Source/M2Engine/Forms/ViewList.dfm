object frmViewList: TfrmViewList
  Left = 540
  Top = 357
  BorderIcons = [biSystemMenu, biMinimize]
  BorderStyle = bsSingle
  BorderWidth = 6
  Caption = #26597#30475#21015#34920#20449#24687
  ClientHeight = 344
  ClientWidth = 700
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
  object tvPage: TTreeView
    Left = 0
    Top = 0
    Width = 162
    Height = 344
    Align = alLeft
    Indent = 19
    ReadOnly = True
    TabOrder = 0
    OnChange = tvPageChange
    Items.NodeData = {
      0307000000260000000000000000000000FFFFFFFFFFFFFFFF00000000000000
      000B00000001046972C154F87673512A000000000000000000000000000000FF
      FFFFFF00000000000000000000000001068179626B365220906972C1542A0000
      00000000000000000001000000FFFFFFFF000000000000000000000000010641
      51B88B365220906972C1542A000000000000000000000009000000FFFFFFFF00
      000000000000000000000001068179626BD6530B4E6972C1542A000000000000
      00000000000C000000FFFFFFFF00000000000000000000000001064151B88B61
      63D6536972C1542A00000000000000000000000D000000FFFFFFFF0000000000
      000000000000000106184F48516163D6536972C1542A00000000000000000000
      000F000000FFFFFFFF00000000000000000000000001068179626B3E663A79FA
      5169722A000000000000000000000012000000FFFFFFFF000000000000000000
      00000001068179626B0383F456FE62D6532A0000000000000000000000130000
      00FFFFFFFF00000000000000000000000001068179626BEA81A852655105532A
      000000000000000000000004000000FFFFFFFF00000000000000000000000001
      066972C154105EF753D17E9A5B2A000000000000000000000005000000FFFFFF
      FF00000000000000000000000001066972C154BA4E6972D17E9A5B2E00000000
      0000000000000006000000FFFFFFFF00000000000000000000000001086972C1
      544900500030574057D17E9A5B260000000000000000000000FFFFFFFFFFFFFF
      FF00000000000000000400000001042A606972F87673512C0000000000000000
      00000007000000FFFFFFFF0000000000000000000000000107E15D3B902A6061
      63D6536972C15428000000000000000000000008000000FFFFFFFF0000000000
      0000000000000001052A60697206726972C1542A00000000000000000000000A
      000000FFFFFFFF00000000000000000000000001068179626B056E06742A6069
      722A000000000000000000000011000000FFFFFFFF0000000000000000000000
      0001064151B88B0F90C6892A6069722A000000000000000000000002000000FF
      FFFFFF0000000000000000000000000106386E0F62E565D75FC78FE46E2A0000
      00000000000000000003000000FFFFFFFF000000000000000000000000010681
      79626B204F01903057FE562800000000000000000000000B000000FFFFFFFF00
      00000000000000000000000105A17B06745854175268882A0000000000000000
      0000000E000000FFFFFFFF0000000000000000000000000106575B267BC78FE4
      6E1752688828000000000000000000000010000000FFFFFFFF00000000000000
      000000000001058C9AC18B0178BE8B6E7F}
    ExplicitHeight = 343
  end
  object pnlClient: TPanel
    Left = 162
    Top = 0
    Width = 538
    Height = 344
    Align = alClient
    BevelOuter = bvNone
    Padding.Left = 6
    TabOrder = 1
    ExplicitWidth = 534
    ExplicitHeight = 343
    object pgcViewList: TPageControl
      Left = 6
      Top = 23
      Width = 532
      Height = 321
      ActivePage = ts11
      Align = alClient
      TabOrder = 0
      ExplicitWidth = 528
      ExplicitHeight = 320
      object ts00: TTabSheet
        Caption = #31105#27490#21046#36896#29289#21697
        TabVisible = False
        object GroupBox3: TGroupBox
          Left = 8
          Top = 4
          Width = 208
          Height = 300
          Caption = #31105#27490#21046#36896#21015#34920
          TabOrder = 0
          object ListBoxDisableMakeList: TListBox
            Left = 10
            Top = 19
            Width = 188
            Height = 271
            ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
            ItemHeight = 12
            TabOrder = 0
            OnClick = ListBoxDisableMakeListClick
          end
        end
        object GroupBox4: TGroupBox
          Left = 306
          Top = 4
          Width = 208
          Height = 300
          Caption = #29289#21697#21015#34920
          TabOrder = 1
          object ListBoxitemList1: TListBox
            Left = 10
            Top = 19
            Width = 188
            Height = 271
            Hint = 'Ctrl + F '#29289#21697#26597#25214
            ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
            ItemHeight = 12
            MultiSelect = True
            TabOrder = 0
            OnClick = ListBoxitemList1Click
            OnKeyDown = ListBoxItemListKeyDown
          end
        end
        object btnAddDisableMakeItem: TButton
          Left = 225
          Top = 10
          Width = 73
          Height = 25
          Caption = #22686#21152'(&A)'
          TabOrder = 2
          OnClick = btnAddDisableMakeItemClick
        end
        object btnDelDisableMakeItem: TButton
          Left = 225
          Top = 43
          Width = 73
          Height = 25
          Caption = #21024#38500'(&D)'
          TabOrder = 3
          OnClick = btnDelDisableMakeItemClick
        end
        object btnSaveDisableMakeItem: TButton
          Left = 225
          Top = 143
          Width = 73
          Height = 25
          Caption = #20445#23384'(&S)'
          TabOrder = 4
          OnClick = btnSaveDisableMakeItemClick
        end
        object btnAddAllDisableMakeItem: TButton
          Left = 225
          Top = 76
          Width = 73
          Height = 25
          Caption = #20840#37096#22686#21152'(&A)'
          TabOrder = 5
          OnClick = btnAddAllDisableMakeItemClick
        end
        object btnDelAllDisableMakeItem: TButton
          Left = 225
          Top = 109
          Width = 73
          Height = 25
          Caption = #20840#37096#21024#38500'(&D)'
          TabOrder = 6
          OnClick = btnDelAllDisableMakeItemClick
        end
      end
      object ts01: TTabSheet
        Caption = #20801#35768#21046#36896#29289#21697
        ImageIndex = 1
        TabVisible = False
        object GroupBox2: TGroupBox
          Left = 306
          Top = 4
          Width = 208
          Height = 300
          Caption = #29289#21697#21015#34920
          TabOrder = 0
          object ListBoxItemList: TListBox
            Left = 10
            Top = 19
            Width = 188
            Height = 271
            Hint = 'Ctrl + F '#29289#21697#26597#25214
            ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
            ItemHeight = 12
            MultiSelect = True
            TabOrder = 0
            OnClick = ListBoxItemListClick
            OnKeyDown = ListBoxItemListKeyDown
          end
        end
        object GroupBox1: TGroupBox
          Left = 8
          Top = 4
          Width = 208
          Height = 300
          Caption = #20801#35768#21046#36896#21015#34920
          TabOrder = 1
          object ListBoxEnableMakeList: TListBox
            Left = 10
            Top = 19
            Width = 188
            Height = 271
            ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
            ItemHeight = 12
            TabOrder = 0
            OnClick = ListBoxEnableMakeListClick
          end
        end
        object btnAddEnableMakeItem: TButton
          Left = 225
          Top = 10
          Width = 73
          Height = 25
          Caption = #22686#21152'(&A)'
          TabOrder = 2
          OnClick = btnAddEnableMakeItemClick
        end
        object btnDelEnableMakeItem: TButton
          Left = 225
          Top = 43
          Width = 73
          Height = 25
          Caption = #21024#38500'(&D)'
          TabOrder = 3
          OnClick = btnDelEnableMakeItemClick
        end
        object btnSaveEnableMakeItem: TButton
          Left = 225
          Top = 143
          Width = 73
          Height = 25
          Caption = #20445#23384'(&S)'
          TabOrder = 4
          OnClick = btnSaveEnableMakeItemClick
        end
        object btnAddAllEnableMakeItem: TButton
          Left = 225
          Top = 76
          Width = 73
          Height = 25
          Caption = #20840#37096#22686#21152'(&A)'
          TabOrder = 5
          OnClick = btnAddAllEnableMakeItemClick
        end
        object btnDelAllEnableMakeItem: TButton
          Left = 225
          Top = 109
          Width = 73
          Height = 25
          Caption = #20840#37096#21024#38500'(&D)'
          TabOrder = 6
          OnClick = btnDelAllEnableMakeItemClick
        end
      end
      object ts02: TTabSheet
        Hint = #28216#25103#26085#24535#36807#28388#65292#21487#20197#25351#23450#35760#24405#37027#20123#29289#21697#20135#29983#30340#26085#24535#65292#20174#32780#20943#23569#26085#24535#30340#22823#23567#12290
        Caption = #28216#25103#26085#24535#36807#28388
        ImageIndex = 8
        TabVisible = False
        object GroupBox8: TGroupBox
          Left = 8
          Top = 4
          Width = 208
          Height = 300
          Caption = #35760#24405#29289#21697'/'#20107#20214#21015#34920
          TabOrder = 0
          object ListBoxGameLogList: TListBox
            Left = 10
            Top = 19
            Width = 188
            Height = 271
            ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
            ItemHeight = 12
            TabOrder = 0
            OnClick = ListBoxGameLogListClick
            OnKeyDown = ListBoxItemListKeyDown
          end
        end
        object btnAddLogItem: TButton
          Left = 225
          Top = 10
          Width = 73
          Height = 25
          Caption = #22686#21152'(&A)'
          TabOrder = 1
          OnClick = btnAddLogItemClick
        end
        object btnDelLogItem: TButton
          Left = 225
          Top = 43
          Width = 73
          Height = 25
          Caption = #21024#38500'(&D)'
          TabOrder = 2
          OnClick = btnDelLogItemClick
        end
        object btnAddAllLogItem: TButton
          Left = 225
          Top = 76
          Width = 73
          Height = 25
          Caption = #20840#37096#22686#21152'(&A)'
          TabOrder = 3
          OnClick = btnAddAllLogItemClick
        end
        object btnDelAllLogItem: TButton
          Left = 225
          Top = 109
          Width = 73
          Height = 25
          Caption = #20840#37096#21024#38500'(&D)'
          TabOrder = 4
          OnClick = btnDelAllLogItemClick
        end
        object btnSaveLogItem: TButton
          Left = 225
          Top = 143
          Width = 73
          Height = 25
          Caption = #20445#23384'(&S)'
          TabOrder = 5
          OnClick = btnSaveLogItemClick
        end
        object GroupBox9: TGroupBox
          Left = 306
          Top = 4
          Width = 208
          Height = 300
          Caption = #20107#20214'/'#29289#21697#21015#34920
          TabOrder = 6
          object ListBoxitemList2: TListBox
            Left = 10
            Top = 19
            Width = 188
            Height = 271
            ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
            ItemHeight = 12
            MultiSelect = True
            TabOrder = 0
            OnClick = ListBoxitemList2Click
            OnKeyDown = ListBoxItemListKeyDown
          end
        end
      end
      object ts03: TTabSheet
        Caption = #31105#27490#20256#36865#22320#22270
        ImageIndex = 2
        TabVisible = False
        object GroupBox5: TGroupBox
          Left = 8
          Top = 4
          Width = 208
          Height = 300
          Caption = #31105#27490#22320#22270#21015#34920
          TabOrder = 0
          object ListBoxDisableMoveMap: TListBox
            Left = 8
            Top = 19
            Width = 188
            Height = 271
            ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
            ItemHeight = 12
            TabOrder = 0
            OnClick = ListBoxDisableMoveMapClick
          end
        end
        object btnAddDisabelMoveMap: TButton
          Left = 225
          Top = 10
          Width = 73
          Height = 25
          Caption = #22686#21152'(&A)'
          TabOrder = 1
          OnClick = btnAddDisabelMoveMapClick
        end
        object btnDelDisabelMoveMap: TButton
          Left = 225
          Top = 43
          Width = 73
          Height = 25
          Caption = #21024#38500'(&D)'
          TabOrder = 2
          OnClick = btnDelDisabelMoveMapClick
        end
        object btnAddAllDisabelMoveMap: TButton
          Left = 225
          Top = 76
          Width = 73
          Height = 25
          Caption = #20840#37096#22686#21152'(&A)'
          TabOrder = 3
          OnClick = btnAddAllDisabelMoveMapClick
        end
        object btnDelAllDisabelMoveMap: TButton
          Left = 225
          Top = 109
          Width = 73
          Height = 25
          Caption = #20840#37096#21024#38500'(&D)'
          TabOrder = 4
          OnClick = btnDelAllDisabelMoveMapClick
        end
        object btnSaveDisabelMoveMap: TButton
          Left = 225
          Top = 143
          Width = 73
          Height = 25
          Caption = #20445#23384'(&S)'
          TabOrder = 5
          OnClick = btnSaveDisabelMoveMapClick
        end
        object GroupBox6: TGroupBox
          Left = 306
          Top = 4
          Width = 208
          Height = 300
          Caption = #22320#22270#21015#34920
          TabOrder = 6
          object ListBoxMapList: TListBox
            Left = 8
            Top = 19
            Width = 188
            Height = 271
            ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
            ItemHeight = 12
            MultiSelect = True
            TabOrder = 0
            OnClick = ListBoxMapListClick
          end
        end
      end
      object ts04: TTabSheet
        Caption = #29289#21697#24080#21495#32465#23450
        ImageIndex = 4
        TabVisible = False
        object GridItemBindAccount: TStringGrid
          Left = 8
          Top = 8
          Width = 326
          Height = 294
          Hint = #21152#20837#27492#21015#34920#20013#30340#29289#21697#23558#19982#25351#23450#30340#30331#24405#24080#21495#32465#23450#65292#21482#26377#20197#32465#23450#30340#30331#24405#24080#21495#30331#24405#30340#20154#29289#25165#21487#20197#25140#19978#27492#29289#21697#12290
          ColCount = 4
          DefaultRowHeight = 18
          FixedCols = 0
          Options = [goFixedVertLine, goFixedHorzLine, goVertLine, goHorzLine, goRangeSelect, goRowSelect]
          TabOrder = 0
          OnClick = GridItemBindAccountClick
          ColWidths = (
            91
            63
            68
            88)
          RowHeights = (
            18
            18
            18
            18
            18)
        end
        object GroupBox16: TGroupBox
          Left = 346
          Top = 8
          Width = 169
          Height = 177
          Caption = #35268#21017#35774#32622
          TabOrder = 1
          object Label6: TLabel
            Left = 8
            Top = 42
            Width = 48
            Height = 12
            Caption = #29289#21697'IDX:'
          end
          object Label7: TLabel
            Left = 8
            Top = 66
            Width = 54
            Height = 12
            Caption = #29289#21697#24207#21495':'
          end
          object Label8: TLabel
            Left = 8
            Top = 90
            Width = 54
            Height = 12
            Caption = #32465#23450#24080#21495':'
          end
          object Label9: TLabel
            Left = 8
            Top = 18
            Width = 54
            Height = 12
            Caption = #29289#21697#21517#31216':'
          end
          object ButtonItemBindAcountMod: TButton
            Left = 96
            Top = 112
            Width = 65
            Height = 25
            Caption = #20462#25913'(&S)'
            TabOrder = 0
            OnClick = ButtonItemBindAcountModClick
          end
          object EditItemBindAccountItemIdx: TSpinEditEx
            Left = 68
            Top = 39
            Width = 93
            Height = 21
            MaxValue = 5000
            MinValue = 1
            TabOrder = 1
            Value = 10
            OnChange = EditItemBindAccountItemIdxChange
          end
          object EditItemBindAccountItemMakeIdx: TSpinEditEx
            Left = 68
            Top = 63
            Width = 93
            Height = 21
            MaxValue = 0
            MinValue = 0
            TabOrder = 2
            Value = 10
            OnChange = EditItemBindAccountItemMakeIdxChange
          end
          object EditItemBindAccountItemName: TEdit
            Left = 68
            Top = 16
            Width = 93
            Height = 20
            ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
            ReadOnly = True
            TabOrder = 3
          end
          object ButtonItemBindAcountAdd: TButton
            Left = 8
            Top = 112
            Width = 65
            Height = 25
            Caption = #22686#21152'(&A)'
            TabOrder = 4
            OnClick = ButtonItemBindAcountAddClick
          end
          object ButtonItemBindAcountRef: TButton
            Left = 96
            Top = 144
            Width = 65
            Height = 25
            Caption = #21047#26032'(&R)'
            TabOrder = 5
            OnClick = ButtonItemBindAcountRefClick
          end
          object ButtonItemBindAcountDel: TButton
            Left = 8
            Top = 144
            Width = 65
            Height = 25
            Caption = #21024#38500'(&D)'
            TabOrder = 6
            OnClick = ButtonItemBindAcountDelClick
          end
          object EditItemBindAccountName: TEdit
            Left = 68
            Top = 88
            Width = 93
            Height = 20
            ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
            TabOrder = 7
            OnChange = EditItemBindAccountNameChange
          end
        end
      end
      object ts05: TTabSheet
        Caption = #29289#21697#20154#29289#32465#23450
        ImageIndex = 5
        TabVisible = False
        object GridItemBindCharName: TStringGrid
          Left = 8
          Top = 8
          Width = 326
          Height = 294
          Hint = #21152#20837#27492#21015#34920#20013#30340#29289#21697#23558#19982#25351#23450#30340#20154#29289#21517#31216#32465#23450#65292#21482#26377#32465#23450#30340#20154#29289#25165#21487#20197#25140#19978#27492#29289#21697#12290
          ColCount = 4
          DefaultRowHeight = 18
          FixedCols = 0
          Options = [goFixedVertLine, goFixedHorzLine, goVertLine, goHorzLine, goRangeSelect, goRowSelect]
          TabOrder = 0
          OnClick = GridItemBindCharNameClick
          ColWidths = (
            91
            63
            68
            88)
          RowHeights = (
            18
            18
            18
            18
            18)
        end
        object GroupBox17: TGroupBox
          Left = 346
          Top = 8
          Width = 169
          Height = 177
          Caption = #35268#21017#35774#32622
          TabOrder = 1
          object Label10: TLabel
            Left = 8
            Top = 42
            Width = 48
            Height = 12
            Caption = #29289#21697'IDX:'
          end
          object Label11: TLabel
            Left = 8
            Top = 66
            Width = 54
            Height = 12
            Caption = #29289#21697#24207#21495':'
          end
          object Label12: TLabel
            Left = 8
            Top = 90
            Width = 54
            Height = 12
            Caption = #32465#23450#20154#29289':'
          end
          object Label13: TLabel
            Left = 8
            Top = 18
            Width = 54
            Height = 12
            Caption = #29289#21697#21517#31216':'
          end
          object ButtonItemBindCharNameMod: TButton
            Left = 96
            Top = 112
            Width = 65
            Height = 25
            Caption = #20462#25913'(&S)'
            TabOrder = 0
            OnClick = ButtonItemBindCharNameModClick
          end
          object EditItemBindCharNameItemIdx: TSpinEditEx
            Left = 68
            Top = 39
            Width = 93
            Height = 21
            MaxValue = 5000
            MinValue = 1
            TabOrder = 1
            Value = 10
            OnChange = EditItemBindCharNameItemIdxChange
          end
          object EditItemBindCharNameItemMakeIdx: TSpinEditEx
            Left = 68
            Top = 63
            Width = 93
            Height = 21
            MaxValue = 0
            MinValue = 0
            TabOrder = 2
            Value = 10
            OnChange = EditItemBindCharNameItemMakeIdxChange
          end
          object EditItemBindCharNameItemName: TEdit
            Left = 68
            Top = 16
            Width = 93
            Height = 20
            ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
            ReadOnly = True
            TabOrder = 3
          end
          object ButtonItemBindCharNameAdd: TButton
            Left = 8
            Top = 112
            Width = 65
            Height = 25
            Caption = #22686#21152'(&A)'
            TabOrder = 4
            OnClick = ButtonItemBindCharNameAddClick
          end
          object ButtonItemBindCharNameRef: TButton
            Left = 96
            Top = 144
            Width = 65
            Height = 25
            Caption = #21047#26032'(&R)'
            TabOrder = 5
            OnClick = ButtonItemBindCharNameRefClick
          end
          object ButtonItemBindCharNameDel: TButton
            Left = 8
            Top = 144
            Width = 65
            Height = 25
            Caption = #21024#38500'(&D)'
            TabOrder = 6
            OnClick = ButtonItemBindCharNameDelClick
          end
          object EditItemBindCharNameName: TEdit
            Left = 68
            Top = 88
            Width = 93
            Height = 20
            ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
            TabOrder = 7
            OnChange = EditItemBindCharNameNameChange
          end
        end
      end
      object ts06: TTabSheet
        Caption = #29289#21697'IP'#32465#23450
        ImageIndex = 6
        TabVisible = False
        object GridItemBindIPaddr: TStringGrid
          Left = 8
          Top = 8
          Width = 326
          Height = 294
          Hint = #21152#20837#27492#21015#34920#20013#30340#29289#21697#23558#19982#25351#23450#30340#30331#24405'IP'#22320#22336#32465#23450#65292#21482#26377#20197#32465#23450#30340#30331#24405'IP'#22320#22336#30331#24405#30340#20154#29289#25165#21487#20197#25140#19978#27492#29289#21697#12290
          ColCount = 4
          DefaultRowHeight = 18
          FixedCols = 0
          Options = [goFixedVertLine, goFixedHorzLine, goVertLine, goHorzLine, goRangeSelect, goRowSelect]
          TabOrder = 0
          OnClick = GridItemBindIPaddrClick
          ColWidths = (
            91
            63
            68
            88)
          RowHeights = (
            18
            18
            18
            18
            18)
        end
        object GroupBox18: TGroupBox
          Left = 346
          Top = 8
          Width = 169
          Height = 177
          Caption = #35268#21017#35774#32622
          TabOrder = 1
          object Label14: TLabel
            Left = 8
            Top = 42
            Width = 48
            Height = 12
            Caption = #29289#21697'IDX:'
          end
          object Label15: TLabel
            Left = 8
            Top = 66
            Width = 54
            Height = 12
            Caption = #29289#21697#24207#21495':'
          end
          object Label16: TLabel
            Left = 8
            Top = 90
            Width = 42
            Height = 12
            Caption = #32465#23450'IP:'
          end
          object Label17: TLabel
            Left = 8
            Top = 18
            Width = 54
            Height = 12
            Caption = #29289#21697#21517#31216':'
          end
          object ButtonItemBindIPaddrMod: TButton
            Left = 96
            Top = 112
            Width = 65
            Height = 25
            Caption = #20462#25913'(&S)'
            TabOrder = 0
            OnClick = ButtonItemBindIPaddrModClick
          end
          object EditItemBindIPaddrItemIdx: TSpinEditEx
            Left = 68
            Top = 39
            Width = 93
            Height = 21
            MaxValue = 5000
            MinValue = 1
            TabOrder = 1
            Value = 10
            OnChange = EditItemBindIPaddrItemIdxChange
          end
          object EditItemBindIPaddrItemMakeIdx: TSpinEditEx
            Left = 68
            Top = 63
            Width = 93
            Height = 21
            MaxValue = 0
            MinValue = 0
            TabOrder = 2
            Value = 10
            OnChange = EditItemBindIPaddrItemMakeIdxChange
          end
          object EditItemBindIPaddrItemName: TEdit
            Left = 68
            Top = 16
            Width = 93
            Height = 20
            ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
            ReadOnly = True
            TabOrder = 3
          end
          object ButtonItemBindIPaddrAdd: TButton
            Left = 8
            Top = 112
            Width = 65
            Height = 25
            Caption = #22686#21152'(&A)'
            TabOrder = 4
            OnClick = ButtonItemBindIPaddrAddClick
          end
          object ButtonItemBindIPaddrRef: TButton
            Left = 96
            Top = 144
            Width = 65
            Height = 25
            Caption = #21047#26032'(&R)'
            TabOrder = 5
            OnClick = ButtonItemBindIPaddrRefClick
          end
          object ButtonItemBindIPaddrDel: TButton
            Left = 8
            Top = 144
            Width = 65
            Height = 25
            Caption = #21024#38500'(&D)'
            TabOrder = 6
            OnClick = ButtonItemBindIPaddrDelClick
          end
          object EditItemBindIPaddrName: TEdit
            Left = 68
            Top = 88
            Width = 93
            Height = 20
            ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
            TabOrder = 7
            OnChange = EditItemBindIPaddrNameChange
          end
        end
      end
      object ts07: TTabSheet
        Caption = #24033#36923#24618#25441#36215#29289#21697
        ImageIndex = 12
        TabVisible = False
        object GroupBox19: TGroupBox
          Left = 248
          Top = 4
          Width = 150
          Height = 300
          Caption = #29289#21697#21015#34920
          TabOrder = 0
          object lstMoveGuardAllItem: TListBox
            Left = 10
            Top = 19
            Width = 130
            Height = 270
            Hint = 'Ctrl + F '#29289#21697#26597#25214
            ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
            ItemHeight = 12
            MultiSelect = True
            TabOrder = 0
            OnClick = ListBoxItemListClick
            OnKeyDown = ListBoxItemListKeyDown
          end
        end
        object GroupBox24: TGroupBox
          Left = 8
          Top = 4
          Width = 150
          Height = 300
          Caption = #20801#35768#25441#36215#21015#34920
          TabOrder = 1
          object lstMoveGuardPickItemList: TListBox
            Left = 10
            Top = 19
            Width = 130
            Height = 270
            ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
            ItemHeight = 12
            TabOrder = 0
            OnClick = lstMoveGuardPickItemListClick
          end
        end
        object btnMoveGuardPickItemAdd: TButton
          Left = 168
          Top = 10
          Width = 73
          Height = 25
          Caption = #22686#21152'(&A)'
          TabOrder = 2
          OnClick = btnMoveGuardPickItemAddClick
        end
        object btnMoveGuardPickItemDel: TButton
          Left = 168
          Top = 43
          Width = 73
          Height = 25
          Caption = #21024#38500'(&D)'
          TabOrder = 3
          OnClick = btnMoveGuardPickItemDelClick
        end
        object btnMoveGuardPickItemAddAll: TButton
          Left = 168
          Top = 76
          Width = 73
          Height = 25
          Caption = #20840#37096#22686#21152'(&A)'
          TabOrder = 4
          OnClick = btnMoveGuardPickItemAddAllClick
        end
        object btnMoveGuardPickItemDelAll: TButton
          Left = 168
          Top = 109
          Width = 73
          Height = 25
          Caption = #20840#37096#21024#38500'(&D)'
          TabOrder = 5
          OnClick = btnMoveGuardPickItemDelAllClick
        end
        object btnMoveGuardPickItemSave: TButton
          Left = 168
          Top = 143
          Width = 73
          Height = 25
          Caption = #20445#23384'(&S)'
          TabOrder = 6
          OnClick = btnMoveGuardPickItemSaveClick
        end
        object grp1: TGroupBox
          Left = 408
          Top = 4
          Width = 105
          Height = 82
          Caption = #24033#36923#21355#22763#25511#21046
          TabOrder = 7
          object chkMoveSuperGuardPickItem: TCheckBox
            Left = 8
            Top = 20
            Width = 88
            Height = 17
            Caption = #20801#35768#25441#29289#21697
            TabOrder = 0
            OnClick = chkMoveSuperGuardPickItemClick
          end
          object chkMoveSuperGuardAttackMon: TCheckBox
            Left = 8
            Top = 40
            Width = 88
            Height = 17
            Caption = #25915#20987#24618#29289
            TabOrder = 1
            OnClick = chkMoveSuperGuardAttackMonClick
          end
          object chkMoveSuperGuardAttackBB: TCheckBox
            Left = 8
            Top = 60
            Width = 88
            Height = 17
            Caption = #25915#20987#23453#23453
            TabOrder = 2
            OnClick = chkMoveSuperGuardAttackBBClick
          end
        end
        object GroupBox25: TGroupBox
          Left = 408
          Top = 92
          Width = 105
          Height = 45
          Caption = #24033#36923#24339#31661#25163#25511#21046
          TabOrder = 8
          object chkMoveArcherGuardPickItem: TCheckBox
            Left = 8
            Top = 20
            Width = 90
            Height = 17
            Caption = #20801#35768#25441#29289#21697
            TabOrder = 0
            OnClick = chkMoveArcherGuardPickItemClick
          end
        end
      end
      object ts08: TTabSheet
        Caption = #24618#29289#29190#29289#21697
        ImageIndex = 7
        TabVisible = False
        object StringGridMonDropLimit: TStringGrid
          Left = 8
          Top = 8
          Width = 355
          Height = 290
          DefaultRowHeight = 18
          FixedCols = 0
          Options = [goFixedVertLine, goFixedHorzLine, goVertLine, goHorzLine, goRangeSelect, goRowSelect]
          TabOrder = 0
          OnClick = StringGridMonDropLimitClick
          ColWidths = (
            76
            57
            48
            47
            111)
          RowHeights = (
            18
            18
            18
            18
            18)
        end
        object GroupBox7: TGroupBox
          Left = 373
          Top = 8
          Width = 141
          Height = 193
          Caption = #35268#21017#35774#32622
          TabOrder = 1
          object Label29: TLabel
            Left = 8
            Top = 41
            Width = 54
            Height = 12
            Caption = #24050#29190#25968#37327':'
          end
          object Label1: TLabel
            Left = 8
            Top = 64
            Width = 54
            Height = 12
            Caption = #38480#21046#25968#37327':'
          end
          object Label2: TLabel
            Left = 8
            Top = 87
            Width = 54
            Height = 12
            Caption = #26410#29190#25968#37327':'
          end
          object Label3: TLabel
            Left = 8
            Top = 18
            Width = 30
            Height = 12
            Caption = #29289#21697':'
          end
          object Label18: TLabel
            Left = 8
            Top = 110
            Width = 54
            Height = 12
            Caption = #28165#38646#22825#25968':'
          end
          object ButtonMonDropLimitSave: TButton
            Left = 72
            Top = 132
            Width = 59
            Height = 25
            Caption = #20462#25913'(&S)'
            TabOrder = 0
            OnClick = ButtonMonDropLimitSaveClick
          end
          object EditDropCount: TSpinEditEx
            Left = 63
            Top = 38
            Width = 71
            Height = 21
            MaxValue = 100000
            MinValue = 0
            TabOrder = 1
            Value = 10
          end
          object EditCountLimit: TSpinEditEx
            Left = 63
            Top = 61
            Width = 71
            Height = 21
            MaxValue = 100000
            MinValue = 0
            TabOrder = 2
            Value = 10
          end
          object EditNoDropCount: TSpinEditEx
            Left = 63
            Top = 84
            Width = 71
            Height = 21
            MaxValue = 100000
            MinValue = 0
            TabOrder = 3
            Value = 10
          end
          object EditItemName: TEdit
            Left = 40
            Top = 16
            Width = 93
            Height = 20
            ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
            TabOrder = 4
          end
          object ButtonMonDropLimitAdd: TButton
            Left = 8
            Top = 132
            Width = 59
            Height = 25
            Caption = #22686#21152'(&A)'
            TabOrder = 5
            OnClick = ButtonMonDropLimitAddClick
          end
          object ButtonMonDropLimitRef: TButton
            Left = 72
            Top = 160
            Width = 59
            Height = 25
            Caption = #21047#26032'(&R)'
            TabOrder = 6
            OnClick = ButtonMonDropLimitRefClick
          end
          object ButtonMonDropLimitDel: TButton
            Left = 8
            Top = 160
            Width = 59
            Height = 25
            Caption = #21024#38500'(&D)'
            TabOrder = 7
            OnClick = ButtonMonDropLimitDelClick
          end
          object seClearDay: TSpinEditEx
            Left = 63
            Top = 107
            Width = 71
            Height = 21
            MaxValue = 100000
            MinValue = 0
            TabOrder = 8
            Value = 10
          end
        end
      end
      object ts09: TTabSheet
        Hint = #31105#27490#21462#19979#29289#21697#35774#32622#65292#21152#20837#27492#21015#34920#30340#29289#21697#25140#22312#36523#19978#21518#23558#19981#21487#20197#21462#19979#26469#65292#27515#20129#20063#19981#20250#25481#33853#12290
        Caption = #31105#27490#21462#19979#29289#21697
        ImageIndex = 9
        TabVisible = False
        object GroupBox10: TGroupBox
          Left = 8
          Top = 4
          Width = 208
          Height = 300
          Caption = #31105#27490#21462#19979#29289#21697#21015#34920
          TabOrder = 0
          object ListBoxDisableTakeOffList: TListBox
            Left = 10
            Top = 19
            Width = 188
            Height = 271
            Hint = #31105#27490#21462#19979#29289#21697#35774#32622#65292#21152#20837#27492#21015#34920#30340#29289#21697#25140#22312#36523#19978#21518#23558#19981#21487#20197#21462#19979#26469#65292#27515#20129#20063#19981#20250#25481#33853#12290
            ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
            ItemHeight = 12
            TabOrder = 0
            OnClick = ListBoxDisableTakeOffListClick
          end
        end
        object ButtonDisableTakeOffAdd: TButton
          Left = 225
          Top = 10
          Width = 73
          Height = 25
          Caption = #22686#21152'(&A)'
          TabOrder = 1
          OnClick = ButtonDisableTakeOffAddClick
        end
        object ButtonDisableTakeOffDel: TButton
          Left = 225
          Top = 43
          Width = 73
          Height = 25
          Caption = #21024#38500'(&D)'
          TabOrder = 2
          OnClick = ButtonDisableTakeOffDelClick
        end
        object ButtonDisableTakeOffAddAll: TButton
          Left = 225
          Top = 76
          Width = 73
          Height = 25
          Caption = #20840#37096#22686#21152'(&A)'
          TabOrder = 3
          OnClick = ButtonDisableTakeOffAddAllClick
        end
        object ButtonDisableTakeOffDelAll: TButton
          Left = 225
          Top = 109
          Width = 73
          Height = 25
          Caption = #20840#37096#21024#38500'(&D)'
          TabOrder = 4
          OnClick = ButtonDisableTakeOffDelAllClick
        end
        object ButtonDisableTakeOffSave: TButton
          Left = 225
          Top = 143
          Width = 73
          Height = 25
          Caption = #20445#23384'(&S)'
          TabOrder = 5
          OnClick = ButtonDisableTakeOffSaveClick
        end
        object GroupBox11: TGroupBox
          Left = 306
          Top = 4
          Width = 208
          Height = 300
          Caption = #29289#21697#21015#34920
          TabOrder = 6
          object ListBoxitemList3: TListBox
            Left = 10
            Top = 19
            Width = 188
            Height = 271
            Hint = 'Ctrl + F '#29289#21697#26597#25214
            ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
            ItemHeight = 12
            MultiSelect = True
            TabOrder = 0
            OnClick = ListBoxitemList3Click
            OnKeyDown = ListBoxItemListKeyDown
          end
        end
      end
      object ts10: TTabSheet
        Caption = #31105#27490#28165#29702#24618#29289#21015#34920
        ImageIndex = 11
        TabVisible = False
        object GroupBox13: TGroupBox
          Left = 8
          Top = 3
          Width = 208
          Height = 300
          Caption = #31105#27490#28165#29702#24618#29289#21015#34920
          TabOrder = 0
          object ListBoxNoClearMonList: TListBox
            Left = 10
            Top = 19
            Width = 188
            Height = 271
            Hint = #31105#27490#28165#38500#24618#29289#35774#32622#65292#29992#20110#33050#26412#21629#20196'CLEARMAPMON'#65292#21152#20837#27492#21015#34920#30340#24618#29289#65292#22312#20351#29992#27492#33050#26412#21629#20196#26102#19981#20250#34987#28165#38500#12290
            ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
            ItemHeight = 12
            TabOrder = 0
            OnClick = ListBoxNoClearMonListClick
          end
        end
        object ButtonNoClearMonAdd: TButton
          Left = 225
          Top = 10
          Width = 73
          Height = 25
          Caption = #22686#21152'(&A)'
          TabOrder = 1
          OnClick = ButtonNoClearMonAddClick
        end
        object ButtonNoClearMonDel: TButton
          Left = 225
          Top = 43
          Width = 73
          Height = 25
          Caption = #21024#38500'(&D)'
          TabOrder = 2
          OnClick = ButtonNoClearMonDelClick
        end
        object ButtonNoClearMonAddAll: TButton
          Left = 225
          Top = 76
          Width = 73
          Height = 25
          Caption = #20840#37096#22686#21152'(&A)'
          TabOrder = 3
          OnClick = ButtonNoClearMonAddAllClick
        end
        object ButtonNoClearMonDelAll: TButton
          Left = 225
          Top = 109
          Width = 73
          Height = 25
          Caption = #20840#37096#21024#38500'(&D)'
          TabOrder = 4
          OnClick = ButtonNoClearMonDelAllClick
        end
        object ButtonNoClearMonSave: TButton
          Left = 225
          Top = 143
          Width = 73
          Height = 25
          Caption = #20445#23384'(&S)'
          TabOrder = 5
          OnClick = ButtonNoClearMonSaveClick
        end
        object GroupBox14: TGroupBox
          Left = 306
          Top = 3
          Width = 208
          Height = 300
          Caption = #24618#29289#21015#34920
          TabOrder = 6
          object ListBoxMonList: TListBox
            Left = 10
            Top = 19
            Width = 188
            Height = 271
            ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
            ItemHeight = 12
            MultiSelect = True
            TabOrder = 0
            OnClick = ListBoxMonListClick
          end
        end
      end
      object ts11: TTabSheet
        Caption = #31649#29702#21592#21015#34920
        ImageIndex = 10
        TabVisible = False
        object GroupBox12: TGroupBox
          Left = 8
          Top = 4
          Width = 283
          Height = 298
          Caption = #31649#29702#21592#21015#34920
          TabOrder = 0
          object ListBoxAdminList: TListBox
            Left = 10
            Top = 19
            Width = 261
            Height = 270
            ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
            ItemHeight = 12
            TabOrder = 0
            OnClick = ListBoxAdminListClick
          end
        end
        object GroupBox15: TGroupBox
          Left = 304
          Top = 4
          Width = 209
          Height = 130
          Caption = #31649#29702#21592#20449#24687
          TabOrder = 1
          object Label4: TLabel
            Left = 11
            Top = 20
            Width = 54
            Height = 12
            Caption = #35282#33394#21517#31216':'
          end
          object Label5: TLabel
            Left = 11
            Top = 44
            Width = 54
            Height = 12
            Caption = #35282#33394#31561#32423':'
          end
          object LabelAdminIPaddr: TLabel
            Left = 11
            Top = 68
            Width = 42
            Height = 12
            Caption = #30331#24405'IP:'
          end
          object EditAdminName: TEdit
            Left = 67
            Top = 16
            Width = 132
            Height = 20
            ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
            TabOrder = 0
          end
          object EditAdminPremission: TSpinEditEx
            Left = 67
            Top = 39
            Width = 61
            Height = 21
            MaxValue = 10
            MinValue = 1
            TabOrder = 1
            Value = 10
          end
          object ButtonAdminListAdd: TButton
            Left = 11
            Top = 93
            Width = 57
            Height = 26
            Caption = #22686#21152'(&A)'
            TabOrder = 2
            OnClick = ButtonAdminListAddClick
          end
          object ButtonAdminListChange: TButton
            Left = 76
            Top = 93
            Width = 57
            Height = 26
            Caption = #20462#25913'(&M)'
            TabOrder = 3
            OnClick = ButtonAdminListChangeClick
          end
          object ButtonAdminListDel: TButton
            Left = 142
            Top = 93
            Width = 57
            Height = 26
            Caption = #21024#38500'(&D)'
            TabOrder = 4
            OnClick = ButtonAdminListDelClick
          end
          object EditAdminIPaddr: TEdit
            Left = 67
            Top = 64
            Width = 132
            Height = 20
            ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
            TabOrder = 5
          end
        end
        object ButtonAdminLitsSave: TButton
          Left = 456
          Top = 139
          Width = 57
          Height = 25
          Caption = #20445#23384'(&S)'
          TabOrder = 2
          OnClick = ButtonAdminLitsSaveClick
        end
      end
      object ts12: TTabSheet
        Caption = #20801#35768#25441#29289#21015#34920
        ImageIndex = 13
        TabVisible = False
        object Label22: TLabel
          Left = 7
          Top = 292
          Width = 168
          Height = 12
          Caption = #33521#38596#12289#20551#20154#12289#23456#29289#20801#35768#25441#29289#21015#34920
          Font.Charset = GB2312_CHARSET
          Font.Color = clRed
          Font.Height = -12
          Font.Name = #23435#20307
          Font.Style = []
          ParentFont = False
        end
        object GroupBox20: TGroupBox
          Left = 8
          Top = 4
          Width = 208
          Height = 284
          Caption = #20801#35768#25441#21462#29289#21697#21015#34920
          TabOrder = 0
          object ListBoxEnablePickUpList: TListBox
            Left = 10
            Top = 19
            Width = 188
            Height = 254
            ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
            ItemHeight = 12
            TabOrder = 0
            OnClick = ListBoxEnablePickUpListClick
            OnKeyDown = ListBoxItemListKeyDown
          end
        end
        object GroupBox21: TGroupBox
          Left = 306
          Top = 4
          Width = 208
          Height = 300
          Caption = #29289#21697#21015#34920
          TabOrder = 1
          object ListBoxitemList5: TListBox
            Left = 10
            Top = 19
            Width = 188
            Height = 271
            Hint = 'Ctrl + F '#29289#21697#26597#25214
            ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
            ItemHeight = 12
            MultiSelect = True
            TabOrder = 0
            OnClick = ListBoxitemList5Click
            OnKeyDown = ListBoxItemListKeyDown
          end
        end
        object ButtonEnablePickUpAdd: TButton
          Left = 225
          Top = 10
          Width = 73
          Height = 25
          Caption = #22686#21152'(&A)'
          TabOrder = 2
          OnClick = ButtonEnablePickUpAddClick
        end
        object ButtonEnablePickUpDelete: TButton
          Left = 225
          Top = 43
          Width = 73
          Height = 25
          Caption = #21024#38500'(&D)'
          TabOrder = 3
          OnClick = ButtonEnablePickUpDeleteClick
        end
        object ButtonEnablePickUpAddAll: TButton
          Left = 225
          Top = 76
          Width = 73
          Height = 25
          Caption = #20840#37096#22686#21152'(&A)'
          TabOrder = 4
          OnClick = ButtonEnablePickUpAddAllClick
        end
        object ButtonEnablePickUpDeleteAll: TButton
          Left = 225
          Top = 109
          Width = 73
          Height = 25
          Caption = #20840#37096#21024#38500'(&D)'
          TabOrder = 5
          OnClick = ButtonEnablePickUpDeleteAllClick
        end
        object ButtonEnablePickUpSave: TButton
          Left = 225
          Top = 143
          Width = 73
          Height = 25
          Caption = #20445#23384'(&S)'
          TabOrder = 6
          OnClick = ButtonEnablePickUpSaveClick
        end
      end
      object ts13: TTabSheet
        Caption = #20248#20808#25441#21462#21015#34920
        ImageIndex = 14
        TabVisible = False
        object Label23: TLabel
          Left = 8
          Top = 293
          Width = 168
          Height = 12
          Caption = #33521#38596#12289#20551#20154#12289#23456#29289#20248#20808#25441#21462#21015#34920
          Font.Charset = GB2312_CHARSET
          Font.Color = clRed
          Font.Height = -12
          Font.Name = #23435#20307
          Font.Style = []
          ParentFont = False
        end
        object GroupBox22: TGroupBox
          Left = 8
          Top = 4
          Width = 208
          Height = 284
          Caption = #20801#35768#25441#21462#29289#21697#21015#34920
          TabOrder = 0
          object ListBoxPriorityPickUpList: TListBox
            Left = 10
            Top = 19
            Width = 188
            Height = 254
            ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
            ItemHeight = 12
            TabOrder = 0
            OnClick = ListBoxPriorityPickUpListClick
            OnKeyDown = ListBoxItemListKeyDown
          end
        end
        object GroupBox23: TGroupBox
          Left = 306
          Top = 4
          Width = 208
          Height = 300
          Caption = #29289#21697#21015#34920
          TabOrder = 1
          object ListBoxitemList6: TListBox
            Left = 10
            Top = 19
            Width = 188
            Height = 271
            Hint = 'Ctrl + F '#29289#21697#26597#25214
            ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
            ItemHeight = 12
            MultiSelect = True
            TabOrder = 0
            OnClick = ListBoxitemList6Click
            OnKeyDown = ListBoxItemListKeyDown
          end
        end
        object ButtonPriorityPickUpAdd: TButton
          Left = 225
          Top = 10
          Width = 73
          Height = 25
          Caption = #22686#21152'(&A)'
          TabOrder = 2
          OnClick = ButtonPriorityPickUpAddClick
        end
        object ButtonPriorityPickUpDelete: TButton
          Left = 225
          Top = 43
          Width = 73
          Height = 25
          Caption = #21024#38500'(&D)'
          TabOrder = 3
          OnClick = ButtonPriorityPickUpDeleteClick
        end
        object ButtonPriorityPickUpAddAll: TButton
          Left = 225
          Top = 76
          Width = 73
          Height = 25
          Caption = #20840#37096#22686#21152'(&A)'
          TabOrder = 4
          OnClick = ButtonPriorityPickUpAddAllClick
        end
        object ButtonPriorityPickUpDeleteAll: TButton
          Left = 225
          Top = 109
          Width = 73
          Height = 25
          Caption = #20840#37096#21024#38500'(&D)'
          TabOrder = 5
          OnClick = ButtonPriorityPickUpDeleteAllClick
        end
        object ButtonPriorityPickUpSave: TButton
          Left = 225
          Top = 143
          Width = 73
          Height = 25
          Caption = #20445#23384'(&S)'
          TabOrder = 6
          OnClick = ButtonPriorityPickUpSaveClick
        end
      end
      object ts14: TTabSheet
        Caption = #23383#31526#36807#28388#21015#34920
        ImageIndex = 15
        TabVisible = False
        object lbl1: TLabel
          Left = 8
          Top = 4
          Width = 270
          Height = 12
          Caption = #31105#27490#35013#22791#25913#21517'/'#34892#20250#21517'/'#34892#20250#23553#21495'/'#21830#24215#21517#21547#20197#19979#23383#31526
        end
        object lbl2: TLabel
          Left = 8
          Top = 293
          Width = 96
          Height = 12
          Caption = #31105#27490#23383#31526#27599#34892#19968#20010
          Font.Charset = GB2312_CHARSET
          Font.Color = clRed
          Font.Height = -12
          Font.Name = #23435#20307
          Font.Style = []
          ParentFont = False
        end
        object Label24: TLabel
          Left = 328
          Top = 4
          Width = 156
          Height = 12
          Caption = #31105#27490#33258#23450#20041#36755#20837#26694#21547#20197#19979#23383#31526
        end
        object mmoNameFilterList: TMemo
          Left = 8
          Top = 24
          Width = 305
          Height = 262
          ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ScrollBars = ssBoth
          TabOrder = 0
          OnChange = mmoNameFilterListChange
        end
        object btnNameFilterSave: TButton
          Left = 436
          Top = 280
          Width = 75
          Height = 25
          Caption = #20445#23384
          TabOrder = 1
          OnClick = btnNameFilterSaveClick
        end
        object mmoInputBoxFilterList: TMemo
          Left = 326
          Top = 24
          Width = 185
          Height = 250
          Hint = 
            #31105#27490#36755#20837#23383#31526#24050#32463#20869#32622#20102#65306'@  <  >  $ '#13#10#13#10'@InputString'#20013#36755#20837#27861#38750#23383#31526#20250#35302#21457#65306'@InputStringFi' +
            'lter'#13#10#13#10'@InputInteger'#20013#36755#20837#38750#27861#23383#31526#20250#35302#21457#65306'@InputIntegerFilter'#13#10#13#10'<INPUTTEX' +
            'T:>,<INPUTNUM:>'#20013#36755#20837#27861#38750#23383#31526#20250#35302#21457#65306'@InputBoxFilter'#13#10
          ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ScrollBars = ssBoth
          TabOrder = 2
          OnChange = mmoNameFilterListChange
        end
      end
      object ts15: TTabSheet
        Caption = #31105#27490#26174#31034#35013#22791#20986#22788
        ImageIndex = 16
        TabVisible = False
        object GroupBox26: TGroupBox
          Left = 8
          Top = 4
          Width = 208
          Height = 300
          Caption = #31105#27490#26174#31034#35013#22791#20986#22788#29289#21697
          TabOrder = 0
          object lstDisableShowItemFrom: TListBox
            Left = 10
            Top = 19
            Width = 188
            Height = 271
            ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
            ItemHeight = 12
            TabOrder = 0
            OnClick = lstDisableShowItemFromClick
          end
        end
        object btnDisableItemFromAdd: TButton
          Left = 225
          Top = 10
          Width = 73
          Height = 25
          Caption = #22686#21152'(&A)'
          TabOrder = 1
          OnClick = btnDisableItemFromAddClick
        end
        object btnDisableItemFromDel: TButton
          Left = 225
          Top = 43
          Width = 73
          Height = 25
          Caption = #21024#38500'(&D)'
          TabOrder = 2
          OnClick = btnDisableItemFromDelClick
        end
        object btnDisableItemFromAddAll: TButton
          Left = 225
          Top = 76
          Width = 73
          Height = 25
          Caption = #20840#37096#22686#21152'(&A)'
          TabOrder = 3
          OnClick = btnDisableItemFromAddAllClick
        end
        object btnDisableItemFromDelAll: TButton
          Left = 225
          Top = 109
          Width = 73
          Height = 25
          Caption = #20840#37096#21024#38500'(&D)'
          TabOrder = 4
          OnClick = btnDisableItemFromDelAllClick
        end
        object btnDisableItemFromSave: TButton
          Left = 225
          Top = 143
          Width = 73
          Height = 25
          Caption = #20445#23384'(&S)'
          TabOrder = 5
          OnClick = btnDisableItemFromSaveClick
        end
        object GroupBox27: TGroupBox
          Left = 306
          Top = 4
          Width = 208
          Height = 300
          Caption = #29289#21697#21015#34920
          TabOrder = 6
          object ListBoxitemList4: TListBox
            Left = 10
            Top = 19
            Width = 188
            Height = 271
            Hint = 'Ctrl + F '#29289#21697#26597#25214
            ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
            ItemHeight = 12
            MultiSelect = True
            TabOrder = 0
            OnClick = ListBoxitemList4Click
            OnKeyDown = ListBoxItemListKeyDown
          end
        end
      end
      object ts16: TTabSheet
        Caption = #39564#35777#30721#35774#32622
        ImageIndex = 17
        TabVisible = False
        object GroupBox28: TGroupBox
          Left = 8
          Top = 4
          Width = 329
          Height = 300
          Caption = #39564#35777#30721#23383#31526
          TabOrder = 0
          object mmoVerifyCodeChrs: TMemo
            Left = 10
            Top = 19
            Width = 308
            Height = 271
            TabOrder = 0
            WantReturns = False
            OnChange = seVerifyCodeLenChange
          end
        end
        object grp2: TGroupBox
          Left = 349
          Top = 4
          Width = 165
          Height = 157
          Caption = #39564#35777#30721#35774#32622
          TabOrder = 1
          object lbl3: TLabel
            Left = 8
            Top = 21
            Width = 72
            Height = 12
            Caption = #39564#35777#30721#38271#24230#65306
          end
          object Label19: TLabel
            Left = 20
            Top = 45
            Width = 60
            Height = 12
            Caption = #36229#26102#26102#38388#65306
          end
          object Label20: TLabel
            Left = 20
            Top = 69
            Width = 60
            Height = 12
            Caption = #21047#26032#27425#25968#65306
          end
          object Label21: TLabel
            Left = 20
            Top = 93
            Width = 60
            Height = 12
            Caption = #22833#36133#27425#25968#65306
          end
          object seVerifyCodeLen: TSpinEditEx
            Left = 76
            Top = 16
            Width = 80
            Height = 21
            MaxValue = 0
            MinValue = 0
            TabOrder = 0
            Value = 10
            OnChange = seVerifyCodeLenChange
          end
          object seVerifyCodeTimeOut: TSpinEditEx
            Left = 76
            Top = 40
            Width = 80
            Height = 21
            MaxValue = 0
            MinValue = 0
            TabOrder = 1
            Value = 10
            OnChange = seVerifyCodeLenChange
          end
          object seVerifyCodeRefreshCount: TSpinEditEx
            Left = 76
            Top = 64
            Width = 80
            Height = 21
            MaxValue = 0
            MinValue = 0
            TabOrder = 2
            Value = 10
            OnChange = seVerifyCodeLenChange
          end
          object seVerifyCodeFailCount: TSpinEditEx
            Left = 76
            Top = 88
            Width = 80
            Height = 21
            MaxValue = 0
            MinValue = 0
            TabOrder = 3
            Value = 10
            OnChange = seVerifyCodeLenChange
          end
          object btnVerifyCodeOK: TButton
            Left = 81
            Top = 120
            Width = 75
            Height = 25
            Caption = #30830#23450
            TabOrder = 4
            OnClick = btnVerifyCodeOKClick
          end
        end
      end
      object ts17: TTabSheet
        Caption = #20801#35768#36879#35270#24618#29289' ('#20801#35768'PreviewMonDropItem'#21644'PreviewMonDropItemRefresh'#36879#35270#30340#24618#29289')'
        ImageIndex = 17
        TabVisible = False
        object Label937: TLabel
          Left = 337
          Top = 287
          Width = 102
          Height = 12
          Caption = #36879#35270#29289#21697#26174#31034#26102#38388':'
        end
        object Label938: TLabel
          Left = 497
          Top = 287
          Width = 12
          Height = 12
          Caption = #31186
        end
        object grp3: TGroupBox
          Left = 8
          Top = 4
          Width = 241
          Height = 300
          Caption = #20801#35768#36879#35270#24618#29289
          TabOrder = 0
          object lvPreviewItemMon: TListView
            Left = 10
            Top = 19
            Width = 220
            Height = 271
            Columns = <
              item
                Caption = #24618#29289#21517#31216
                Width = 140
              end
              item
                Caption = #21047#26032#38388#38548
                Width = 60
              end>
            ReadOnly = True
            RowSelect = True
            TabOrder = 0
            ViewStyle = vsReport
            OnSelectItem = lvPreviewItemMonSelectItem
          end
        end
        object btnPreviewItemMonAdd: TButton
          Left = 256
          Top = 10
          Width = 73
          Height = 25
          Caption = #22686#21152'(&A)'
          TabOrder = 1
          OnClick = btnPreviewItemMonAddClick
        end
        object btnPreviewItemMonDel: TButton
          Left = 256
          Top = 43
          Width = 73
          Height = 25
          Caption = #21024#38500'(&D)'
          TabOrder = 2
          OnClick = btnPreviewItemMonDelClick
        end
        object btnPreviewItemMonAddAll: TButton
          Left = 256
          Top = 76
          Width = 73
          Height = 25
          Caption = #20840#37096#22686#21152'(&A)'
          TabOrder = 3
          OnClick = btnPreviewItemMonAddAllClick
        end
        object btnPreviewItemMonDelAll: TButton
          Left = 256
          Top = 109
          Width = 73
          Height = 25
          Caption = #20840#37096#21024#38500'(&D)'
          TabOrder = 4
          OnClick = btnPreviewItemMonDelAllClick
        end
        object btnPreviewItemMonSave: TButton
          Left = 256
          Top = 143
          Width = 73
          Height = 25
          Caption = #20445#23384'(&S)'
          TabOrder = 5
          OnClick = btnPreviewItemMonSaveClick
        end
        object grp4: TGroupBox
          Left = 336
          Top = 4
          Width = 183
          Height = 275
          Caption = #24618#29289#21015#34920
          TabOrder = 6
          object lbl4: TLabel
            Left = 10
            Top = 250
            Width = 54
            Height = 12
            Caption = #29190#29575#21047#26032':'
          end
          object lbl5: TLabel
            Left = 147
            Top = 250
            Width = 24
            Height = 12
            Caption = #20998#38047
          end
          object lstPreviewItemMonAll: TListBox
            Left = 10
            Top = 19
            Width = 165
            Height = 221
            Hint = 'Ctrl + F '#29289#21697#26597#25214
            ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
            ItemHeight = 12
            MultiSelect = True
            TabOrder = 0
            OnClick = ListBoxitemList4Click
            OnKeyDown = ListBoxItemListKeyDown
          end
          object sePreviewItemMonRefreshTime: TSpinEditEx
            Left = 64
            Top = 246
            Width = 81
            Height = 21
            Hint = #21516#19968#20010#29609#23478#36879#35270#24618#29289#21518#29190#29575#29289#21697#21047#26032#38388#38548#13#13#26102#38388#22823#20110'0'#26102#65292#21017#21516#19968#29609#23478#36229#36807#19968#23450#30340#26102#38388#23545#24618#29289#20877#27425#36879#35270#20250#21047#26032#29190#29575#65292#20026'0'#21017#19981#21047#26032#29190#29575
            MaxValue = 10
            MinValue = 0
            TabOrder = 1
            Value = 5
          end
        end
        object sePreviewMonItemShowTime: TSpinEditEx
          Left = 440
          Top = 283
          Width = 54
          Height = 21
          Hint = #24618#29289#36879#35270#21518#65292#36879#35270#29289#21697#26174#31034#26102#38388
          MaxValue = 0
          MinValue = 0
          TabOrder = 7
          Value = 17
          OnChange = sePreviewMonItemShowTimeChange
        end
      end
      object ts18: TTabSheet
        Caption = #31105#27490#33539#22260#25342#21462' ('#29992#20110#38480#21046'OpenAutoPickItem'#25342#21462')'
        ImageIndex = 18
        TabVisible = False
        object GroupBox29: TGroupBox
          Left = 8
          Top = 4
          Width = 208
          Height = 300
          Caption = #31105#27490#33539#22260#25342#21462#29289#21697#21015#34920
          TabOrder = 0
          object lstDisableRangePickItem: TListBox
            Left = 10
            Top = 19
            Width = 188
            Height = 271
            ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
            ItemHeight = 12
            TabOrder = 0
            OnClick = lstDisableRangePickItemClick
          end
        end
        object btnDelDisableRangePickItem: TButton
          Left = 225
          Top = 43
          Width = 73
          Height = 25
          Caption = #21024#38500'(&D)'
          TabOrder = 1
          OnClick = btnDelDisableRangePickItemClick
        end
        object btnAddAllDisableRangePickItem: TButton
          Left = 225
          Top = 76
          Width = 73
          Height = 25
          Caption = #20840#37096#22686#21152'(&A)'
          TabOrder = 2
          OnClick = btnAddAllDisableRangePickItemClick
        end
        object btnDelAllDisableRangePickItem: TButton
          Left = 225
          Top = 109
          Width = 73
          Height = 25
          Caption = #20840#37096#21024#38500'(&D)'
          TabOrder = 3
          OnClick = btnDelAllDisableRangePickItemClick
        end
        object btnSaveDisableRangePickItem: TButton
          Left = 225
          Top = 143
          Width = 73
          Height = 25
          Caption = #20445#23384'(&S)'
          TabOrder = 4
          OnClick = btnSaveDisableRangePickItemClick
        end
        object GroupBox30: TGroupBox
          Left = 306
          Top = 4
          Width = 208
          Height = 300
          Caption = #29289#21697#21015#34920
          TabOrder = 5
          object lstDisableRangePickItems: TListBox
            Left = 10
            Top = 19
            Width = 188
            Height = 271
            Hint = 'Ctrl + F '#29289#21697#26597#25214
            ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
            ItemHeight = 12
            MultiSelect = True
            TabOrder = 0
            OnClick = lstDisableRangePickItemsClick
            OnKeyDown = ListBoxItemListKeyDown
          end
        end
        object btnAddDisableRangePickItem: TButton
          Left = 225
          Top = 10
          Width = 73
          Height = 25
          Caption = #22686#21152'(&A)'
          TabOrder = 6
          OnClick = btnAddDisableRangePickItemClick
        end
      end
      object ts19: TTabSheet
        Caption = #31105#27490#33258#21160#20837#21253' ('#29992#20110#38480#21046'OpenAutoDropItemToBag'#33258#21160#20837#21253')'
        ImageIndex = 19
        TabVisible = False
        object GroupBox31: TGroupBox
          Left = 8
          Top = 4
          Width = 208
          Height = 300
          Caption = #31105#27490#33258#21160#20837#21253#29289#21697#21015#34920
          TabOrder = 0
          object lstDisableDropToBagItem: TListBox
            Left = 10
            Top = 19
            Width = 188
            Height = 271
            ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
            ItemHeight = 12
            TabOrder = 0
            OnClick = lstDisableDropToBagItemClick
          end
        end
        object btnDelDisableDropToBagItem: TButton
          Left = 225
          Top = 43
          Width = 73
          Height = 25
          Caption = #21024#38500'(&D)'
          TabOrder = 1
          OnClick = btnDelDisableDropToBagItemClick
        end
        object btnAddAllDisableDropToBagItem: TButton
          Left = 225
          Top = 76
          Width = 73
          Height = 25
          Caption = #20840#37096#22686#21152'(&A)'
          TabOrder = 2
          OnClick = btnAddAllDisableDropToBagItemClick
        end
        object btnDelAllDisableDropToBagItem: TButton
          Left = 225
          Top = 109
          Width = 73
          Height = 25
          Caption = #20840#37096#21024#38500'(&D)'
          TabOrder = 3
          OnClick = btnDelAllDisableDropToBagItemClick
        end
        object btnSaveDisableDropToBagItem: TButton
          Left = 225
          Top = 143
          Width = 73
          Height = 25
          Caption = #20445#23384'(&S)'
          TabOrder = 4
          OnClick = btnSaveDisableDropToBagItemClick
        end
        object GroupBox32: TGroupBox
          Left = 306
          Top = 4
          Width = 208
          Height = 300
          Caption = #29289#21697#21015#34920
          TabOrder = 5
          object lstDisableDropToBagItems: TListBox
            Left = 10
            Top = 19
            Width = 188
            Height = 271
            Hint = 'Ctrl + F '#29289#21697#26597#25214
            ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
            ItemHeight = 12
            MultiSelect = True
            TabOrder = 0
            OnClick = lstDisableDropToBagItemsClick
            OnKeyDown = ListBoxItemListKeyDown
          end
        end
        object btnAddDisableDropToBagItem: TButton
          Left = 225
          Top = 10
          Width = 73
          Height = 25
          Caption = #22686#21152'(&A)'
          TabOrder = 6
          OnClick = btnAddDisableDropToBagItemClick
        end
      end
    end
    object pnlTitle: TPanel
      Left = 6
      Top = 0
      Width = 532
      Height = 23
      Align = alTop
      Alignment = taLeftJustify
      BevelEdges = [beBottom]
      BevelKind = bkSoft
      BevelOuter = bvNone
      Font.Charset = GB2312_CHARSET
      Font.Color = clWindowText
      Font.Height = -12
      Font.Name = #23435#20307
      Font.Style = [fsBold]
      ParentFont = False
      TabOrder = 1
      ExplicitWidth = 528
    end
  end
end
