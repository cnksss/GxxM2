object FrmRoleDataEdit: TFrmRoleDataEdit
  Left = 373
  Top = 215
  BorderIcons = [biSystemMenu]
  BorderStyle = bsSingle
  Caption = #32534#36753#20154#29289#25968#25454
  ClientHeight = 380
  ClientWidth = 522
  Color = clBtnFace
  Font.Charset = ANSI_CHARSET
  Font.Color = clWindowText
  Font.Height = -12
  Font.Name = #23435#20307
  Font.Style = []
  OldCreateOrder = False
  Position = poDesktopCenter
  OnCreate = FormCreate
  PixelsPerInch = 96
  TextHeight = 12
  object lbl11111: TLabel
    Left = 275
    Top = 353
    Width = 240
    Height = 12
    Caption = #20462#25913#26102#20154#29289#19981#33021#22312#32447#65292#21542#21017#25968#25454#22238#26723#25110#20986#38169#65281
    Font.Charset = ANSI_CHARSET
    Font.Color = clRed
    Font.Height = -12
    Font.Name = #23435#20307
    Font.Style = []
    ParentFont = False
  end
  object PageControl: TPageControl
    Left = 9
    Top = 8
    Width = 504
    Height = 328
    ActivePage = tsBase
    TabOrder = 0
    object tsBase: TTabSheet
      Caption = #26222#36890
      object lbl2: TLabel
        Left = 10
        Top = 36
        Width = 54
        Height = 12
        Caption = #20154#29289#21517#31216':'
      end
      object lbl3: TLabel
        Left = 10
        Top = 60
        Width = 54
        Height = 12
        Caption = #30331#24405#24080#21495':'
      end
      object lbl4: TLabel
        Left = 10
        Top = 84
        Width = 54
        Height = 12
        Caption = #20179#24211#23494#30721':'
      end
      object lbl5: TLabel
        Left = 10
        Top = 108
        Width = 54
        Height = 12
        Caption = #37197#20598#21517#31216':'
      end
      object lbl6: TLabel
        Left = 10
        Top = 132
        Width = 54
        Height = 12
        Caption = #24072#24466#21517#31216':'
      end
      object lbl1: TLabel
        Left = 10
        Top = 12
        Width = 54
        Height = 12
        Caption = #32034#24341#21495#30721':'
      end
      object lbl7: TLabel
        Left = 202
        Top = 12
        Width = 54
        Height = 12
        Caption = #24403#21069#22320#22270':'
      end
      object lbl8: TLabel
        Left = 202
        Top = 36
        Width = 54
        Height = 12
        Caption = #24403#21069#24231#26631':'
      end
      object lbl9: TLabel
        Left = 202
        Top = 60
        Width = 54
        Height = 12
        Caption = #22238#22478#22320#22270':'
      end
      object lbl10: TLabel
        Left = 202
        Top = 84
        Width = 54
        Height = 12
        Caption = #22238#22478#24231#26631':'
      end
      object edtChrName: TEdit
        Left = 66
        Top = 32
        Width = 97
        Height = 20
        Color = cl3DLight
        ReadOnly = True
        TabOrder = 0
      end
      object edtAccount: TEdit
        Left = 66
        Top = 56
        Width = 97
        Height = 20
        Color = cl3DLight
        ReadOnly = True
        TabOrder = 1
      end
      object edtPassword: TEdit
        Left = 66
        Top = 80
        Width = 97
        Height = 20
        TabOrder = 2
        OnChange = edtPasswordChange
      end
      object edtDearName: TEdit
        Left = 66
        Top = 104
        Width = 97
        Height = 20
        TabOrder = 3
        OnChange = edtPasswordChange
      end
      object edtMasterName: TEdit
        Left = 66
        Top = 128
        Width = 97
        Height = 20
        TabOrder = 4
        OnChange = edtPasswordChange
      end
      object edtID: TEdit
        Left = 66
        Top = 8
        Width = 97
        Height = 20
        Color = cl3DLight
        ReadOnly = True
        TabOrder = 5
      end
      object edtCurMap: TEdit
        Left = 258
        Top = 8
        Width = 97
        Height = 20
        TabOrder = 6
        OnChange = edtPasswordChange
      end
      object seCurX: TSpinEditEx
        Left = 258
        Top = 32
        Width = 49
        Height = 21
        MaxValue = 0
        MinValue = 0
        TabOrder = 7
        Value = 0
        OnChange = edtPasswordChange
      end
      object seCurY: TSpinEditEx
        Left = 306
        Top = 32
        Width = 49
        Height = 21
        MaxValue = 0
        MinValue = 0
        TabOrder = 8
        Value = 0
        OnChange = edtPasswordChange
      end
      object edtHomeMap: TEdit
        Left = 258
        Top = 56
        Width = 97
        Height = 20
        TabOrder = 9
        OnClick = edtPasswordChange
      end
      object seHomeX: TSpinEditEx
        Left = 258
        Top = 80
        Width = 49
        Height = 21
        MaxValue = 0
        MinValue = 0
        TabOrder = 10
        Value = 0
        OnChange = edtPasswordChange
      end
      object seHomeY: TSpinEditEx
        Left = 306
        Top = 80
        Width = 49
        Height = 21
        MaxValue = 0
        MinValue = 0
        TabOrder = 11
        Value = 0
        OnChange = edtPasswordChange
      end
      object chkIsMaster: TCheckBox
        Left = 66
        Top = 152
        Width = 57
        Height = 17
        Caption = #24072#29238
        TabOrder = 12
        OnClick = edtPasswordChange
      end
    end
    object tsInfo: TTabSheet
      Caption = #20449#24687
      ImageIndex = 1
      object lbl11: TLabel
        Left = 10
        Top = 12
        Width = 42
        Height = 12
        Caption = #31561'  '#32423':'
      end
      object lbl12: TLabel
        Left = 10
        Top = 36
        Width = 42
        Height = 12
        Caption = #37329'  '#24065':'
      end
      object lbl13: TLabel
        Left = 10
        Top = 60
        Width = 42
        Height = 12
        Caption = #20803'  '#23453':'
      end
      object lbl14: TLabel
        Left = 10
        Top = 84
        Width = 42
        Height = 12
        Caption = #28216#25103#28857':'
      end
      object lbl18: TLabel
        Left = 10
        Top = 184
        Width = 42
        Height = 12
        Caption = #22768#26395#28857':'
      end
      object lbl17: TLabel
        Left = 10
        Top = 160
        Width = 42
        Height = 12
        Caption = #20805#20540#28857':'
      end
      object lbl19: TLabel
        Left = 10
        Top = 208
        Width = 30
        Height = 12
        Caption = 'PK'#28857':'
      end
      object lbl20: TLabel
        Left = 10
        Top = 232
        Width = 42
        Height = 12
        Caption = #36129#29486#24230':'
      end
      object lbl15: TLabel
        Left = 10
        Top = 111
        Width = 42
        Height = 12
        Caption = #37329#21018#30707':'
      end
      object lbl16: TLabel
        Left = 10
        Top = 136
        Width = 42
        Height = 12
        Caption = #28789'  '#31526':'
      end
      object seLevel: TSpinEditEx
        Left = 54
        Top = 8
        Width = 80
        Height = 21
        MaxValue = 65535
        MinValue = 0
        TabOrder = 0
        Value = 0
        OnChange = edtPasswordChange
      end
      object seGold: TSpinEditLongWord
        Left = 54
        Top = 32
        Width = 80
        Height = 21
        MaxValue = 0
        MinValue = 0
        TabOrder = 1
        Value = 0
        OnChange = edtPasswordChange
      end
      object seGameGold: TSpinEditLongWord
        Left = 54
        Top = 56
        Width = 80
        Height = 21
        MaxValue = 0
        MinValue = 0
        TabOrder = 2
        Value = 0
        OnChange = edtPasswordChange
      end
      object seGamePoint: TSpinEditEx
        Left = 54
        Top = 80
        Width = 80
        Height = 21
        MaxValue = 0
        MinValue = 0
        TabOrder = 3
        Value = 0
        OnChange = edtPasswordChange
      end
      object seCreditPoint: TSpinEditEx
        Left = 54
        Top = 179
        Width = 80
        Height = 21
        MaxValue = 0
        MinValue = 0
        TabOrder = 4
        Value = 0
        OnChange = edtPasswordChange
      end
      object sePayPoint: TSpinEditEx
        Left = 54
        Top = 155
        Width = 80
        Height = 21
        MaxValue = 0
        MinValue = 0
        TabOrder = 5
        Value = 0
        OnChange = edtPasswordChange
      end
      object sePKPoint: TSpinEditEx
        Left = 54
        Top = 203
        Width = 80
        Height = 21
        MaxValue = 0
        MinValue = 0
        TabOrder = 6
        Value = 0
        OnChange = edtPasswordChange
      end
      object seContribution: TSpinEditEx
        Left = 54
        Top = 227
        Width = 80
        Height = 21
        MaxValue = 0
        MinValue = 0
        TabOrder = 7
        Value = 0
        OnChange = edtPasswordChange
      end
      object GroupBox6: TGroupBox
        Left = 162
        Top = 4
        Width = 195
        Height = 163
        Caption = #23646#24615#28857
        TabOrder = 8
        object lbl22: TLabel
          Left = 11
          Top = 45
          Width = 18
          Height = 12
          Caption = 'DC:'
        end
        object lbl23: TLabel
          Left = 11
          Top = 67
          Width = 18
          Height = 12
          Caption = 'MC:'
        end
        object lbl24: TLabel
          Left = 11
          Top = 90
          Width = 18
          Height = 12
          Caption = 'SC:'
        end
        object lbl25: TLabel
          Left = 11
          Top = 112
          Width = 18
          Height = 12
          Caption = 'AC:'
        end
        object lbl26: TLabel
          Left = 11
          Top = 136
          Width = 24
          Height = 12
          Caption = 'MAC:'
        end
        object lbl27: TLabel
          Left = 95
          Top = 45
          Width = 18
          Height = 12
          Caption = 'HP:'
        end
        object lbl28: TLabel
          Left = 95
          Top = 68
          Width = 18
          Height = 12
          Caption = 'MP:'
        end
        object lbl29: TLabel
          Left = 95
          Top = 90
          Width = 24
          Height = 12
          Caption = 'Hit:'
        end
        object lbl30: TLabel
          Left = 95
          Top = 112
          Width = 36
          Height = 12
          Caption = 'Speed:'
        end
        object lbl31: TLabel
          Left = 95
          Top = 136
          Width = 18
          Height = 12
          Caption = 'X2:'
        end
        object lbl21: TLabel
          Left = 11
          Top = 21
          Width = 66
          Height = 12
          Caption = #21487#29992#23646#24615#28857':'
        end
        object EditDC: TSpinEditEx
          Left = 35
          Top = 41
          Width = 54
          Height = 21
          Enabled = False
          MaxValue = 0
          MinValue = 0
          TabOrder = 0
          Value = 0
          OnChange = edtPasswordChange
        end
        object EditMC: TSpinEditEx
          Left = 35
          Top = 63
          Width = 54
          Height = 21
          Enabled = False
          MaxValue = 0
          MinValue = 0
          TabOrder = 1
          Value = 0
          OnChange = edtPasswordChange
        end
        object EditSC: TSpinEditEx
          Left = 35
          Top = 85
          Width = 54
          Height = 21
          Enabled = False
          MaxValue = 0
          MinValue = 0
          TabOrder = 2
          Value = 0
          OnChange = edtPasswordChange
        end
        object EditAC: TSpinEditEx
          Left = 35
          Top = 109
          Width = 54
          Height = 21
          Enabled = False
          MaxValue = 0
          MinValue = 0
          TabOrder = 3
          Value = 0
          OnChange = edtPasswordChange
        end
        object EditMAC: TSpinEditEx
          Left = 35
          Top = 133
          Width = 54
          Height = 21
          Enabled = False
          MaxValue = 0
          MinValue = 0
          TabOrder = 4
          Value = 0
          OnChange = edtPasswordChange
        end
        object EditHP: TSpinEditEx
          Left = 130
          Top = 41
          Width = 54
          Height = 21
          Enabled = False
          MaxValue = 0
          MinValue = 0
          TabOrder = 5
          Value = 0
          OnChange = edtPasswordChange
        end
        object EditMP: TSpinEditEx
          Left = 130
          Top = 63
          Width = 54
          Height = 21
          Enabled = False
          MaxValue = 0
          MinValue = 0
          TabOrder = 6
          Value = 0
          OnChange = edtPasswordChange
        end
        object EditHit: TSpinEditEx
          Left = 130
          Top = 85
          Width = 54
          Height = 21
          Enabled = False
          MaxValue = 0
          MinValue = 0
          TabOrder = 7
          Value = 0
          OnChange = edtPasswordChange
        end
        object EditSpeed: TSpinEditEx
          Left = 130
          Top = 109
          Width = 54
          Height = 21
          Enabled = False
          MaxValue = 0
          MinValue = 0
          TabOrder = 8
          Value = 0
          OnChange = edtPasswordChange
        end
        object EditX2: TSpinEditEx
          Left = 130
          Top = 133
          Width = 54
          Height = 21
          Enabled = False
          MaxValue = 0
          MinValue = 0
          TabOrder = 9
          Value = 0
          OnChange = edtPasswordChange
        end
        object seBonusPoint: TSpinEditEx
          Left = 78
          Top = 16
          Width = 106
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 10
          Value = 0
          OnChange = edtPasswordChange
        end
      end
      object seGameDiamond: TSpinEditLongWord
        Left = 54
        Top = 107
        Width = 80
        Height = 21
        MaxValue = 0
        MinValue = 0
        TabOrder = 9
        Value = 0
        OnChange = edtPasswordChange
      end
      object seGameGird: TSpinEditLongWord
        Left = 54
        Top = 131
        Width = 80
        Height = 21
        MaxValue = 0
        MinValue = 0
        TabOrder = 10
        Value = 0
        OnChange = edtPasswordChange
      end
    end
    object tsMagic: TTabSheet
      BorderWidth = 6
      Caption = #25216#33021
      ImageIndex = 2
      object lvMagic: TListView
        Left = 0
        Top = 0
        Width = 484
        Height = 289
        Align = alClient
        Columns = <
          item
            Caption = #24207#21495
            Width = 40
          end
          item
            Caption = #25216#33021
          end
          item
            Caption = #25216#33021#21517#31216
            Width = 100
          end
          item
            Caption = #31561#32423
            Width = 40
          end
          item
            Caption = #20462#28860#28857
            Width = 60
          end
          item
            Caption = #24555#25463#38190
          end>
        GridLines = True
        ReadOnly = True
        RowSelect = True
        TabOrder = 0
        ViewStyle = vsReport
      end
    end
    object tsUserItem: TTabSheet
      BorderWidth = 6
      Caption = #35013#22791
      ImageIndex = 3
      object lvUserItem: TListView
        Left = 0
        Top = 0
        Width = 484
        Height = 289
        Align = alClient
        Columns = <
          item
            Caption = #24207#21495
            Width = 40
          end
          item
            Caption = #35013#22791#20301#32622
            Width = 76
          end
          item
            Caption = #35013#22791#21517#31216
            Width = 76
          end
          item
            Caption = 'Idx'
          end
          item
            Caption = #24207#21015#21495
            Width = 80
          end
          item
            Alignment = taCenter
            Caption = #25345#20037
            Width = 90
          end
          item
            Caption = #21442#25968
            Width = 220
          end>
        GridLines = True
        ReadOnly = True
        RowSelect = True
        TabOrder = 0
        ViewStyle = vsReport
      end
    end
    object tsFenghao: TTabSheet
      BorderWidth = 6
      Caption = #31216#21495
      ImageIndex = 7
      object lvFenghaoItem: TListView
        Left = 0
        Top = 0
        Width = 484
        Height = 289
        Align = alClient
        Columns = <
          item
            Caption = #24207#21495
            Width = 40
          end
          item
            Caption = #35013#22791#21517#31216
            Width = 76
          end
          item
            Caption = 'Idx'
          end
          item
            Caption = #24207#21015#21495
            Width = 80
          end
          item
            Alignment = taCenter
            Caption = #25345#20037
            Width = 90
          end
          item
            Caption = #21442#25968
            Width = 220
          end>
        GridLines = True
        ReadOnly = True
        RowSelect = True
        TabOrder = 0
        ViewStyle = vsReport
      end
    end
    object tsSorage: TTabSheet
      BorderWidth = 6
      Caption = #20179#24211
      ImageIndex = 4
      object lvStorage: TListView
        Left = 0
        Top = 0
        Width = 484
        Height = 289
        Align = alClient
        Columns = <
          item
            Caption = #24207#21495
            Width = 40
          end
          item
            Caption = #35013#22791#21517#31216
            Width = 76
          end
          item
            Caption = 'Idx'
          end
          item
            Caption = #24207#21015#21495
            Width = 80
          end
          item
            Alignment = taCenter
            Caption = #25345#20037
            Width = 90
          end
          item
            Caption = #21442#25968
            Width = 220
          end>
        GridLines = True
        ReadOnly = True
        RowSelect = True
        TabOrder = 0
        ViewStyle = vsReport
      end
    end
    object tsVarU: TTabSheet
      BorderWidth = 6
      Caption = 'U'#21464#37327
      ImageIndex = 5
      object strGridVarU: TStringGrid
        Left = 0
        Top = 0
        Width = 484
        Height = 289
        Align = alClient
        ColCount = 2
        DefaultRowHeight = 20
        RowCount = 101
        Options = [goFixedVertLine, goFixedHorzLine, goVertLine, goHorzLine, goRangeSelect, goEditing]
        TabOrder = 0
        ColWidths = (
          48
          369)
      end
    end
    object tsVarT: TTabSheet
      BorderWidth = 6
      Caption = 'T'#21464#37327
      ImageIndex = 6
      object strGridVarT: TStringGrid
        Left = 0
        Top = 0
        Width = 484
        Height = 289
        Align = alClient
        ColCount = 2
        DefaultRowHeight = 20
        RowCount = 101
        Options = [goFixedVertLine, goFixedHorzLine, goVertLine, goHorzLine, goRangeSelect, goEditing]
        TabOrder = 0
        ColWidths = (
          48
          369)
      end
    end
  end
  object ButtonSaveData: TButton
    Left = 8
    Top = 346
    Width = 81
    Height = 25
    Caption = #20445#23384#20462#25913'(&S)'
    TabOrder = 1
    OnClick = ButtonSaveDataClick
  end
  object ButtonExportData: TButton
    Left = 95
    Top = 346
    Width = 81
    Height = 25
    Caption = #23548#20986#25968#25454'(&E)'
    TabOrder = 2
    OnClick = ButtonExportDataClick
  end
  object ButtonImportData: TButton
    Left = 182
    Top = 346
    Width = 81
    Height = 25
    Caption = #23548#20837#25968#25454'(&I)'
    TabOrder = 3
    OnClick = ButtonExportDataClick
  end
  object SaveDialog: TSaveDialog
    DefaultExt = 'hum'
    Filter = #20154#29289#25968#25454' (*.hum)|*.hum'
    Options = [ofOverwritePrompt, ofHideReadOnly, ofEnableSizing]
    Left = 312
    Top = 304
  end
  object OpenDialog: TOpenDialog
    DefaultExt = 'hum'
    Filter = #20154#29289#25968#25454' (*.hum)|*.hum'
    Left = 344
    Top = 304
  end
end
