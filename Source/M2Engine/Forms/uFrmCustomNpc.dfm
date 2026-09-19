object FrmCustomNpc: TFrmCustomNpc
  Left = 568
  Top = 272
  BorderIcons = [biSystemMenu, biMinimize]
  BorderStyle = bsDialog
  BorderWidth = 5
  Caption = #33258#23450#20041'NPC'
  ClientHeight = 632
  ClientWidth = 896
  Color = clBtnFace
  Font.Charset = GB2312_CHARSET
  Font.Color = clWindowText
  Font.Height = -12
  Font.Name = #23435#20307
  Font.Style = []
  Position = poMainFormCenter
  OnCreate = FormCreate
  TextHeight = 12
  object pnlNpcClient: TPanel
    Left = 0
    Top = 0
    Width = 896
    Height = 559
    Align = alClient
    BevelOuter = bvNone
    BorderWidth = 3
    TabOrder = 0
    ExplicitWidth = 892
    ExplicitHeight = 558
    object grp10: TGroupBox
      Left = 3
      Top = 3
      Width = 120
      Height = 553
      Align = alLeft
      Caption = #33258#23450#20041'NPC'#21015#34920
      TabOrder = 0
      ExplicitHeight = 552
      object vstCustomNpc: TVirtualStringTree
        Left = 8
        Top = 16
        Width = 106
        Height = 529
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
        Colors.UnfocusedColor = 15716484
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
        TabOrder = 0
        TreeOptions.PaintOptions = [toShowDropmark, toThemeAware, toUseBlendedImages]
        TreeOptions.SelectionOptions = [toFullRowSelect]
        OnDrawText = vstCustomNpcDrawText
        OnGetText = vstCustomNpcGetText
        OnGetNodeDataSize = vstCustomNpcGetNodeDataSize
        OnNodeClick = vstCustomNpcNodeClick
        Columns = <>
      end
    end
    object pnlNpc: TPanel
      Left = 123
      Top = 3
      Width = 770
      Height = 553
      Align = alClient
      BevelOuter = bvNone
      TabOrder = 1
      ExplicitWidth = 766
      ExplicitHeight = 552
      object GroupBox1: TGroupBox
        Left = 5
        Top = 0
        Width = 426
        Height = 90
        Caption = 'NPC'#34880#26465#35774#32622
        TabOrder = 0
        object grp11: TGroupBox
          Left = 202
          Top = 16
          Width = 65
          Height = 68
          Caption = #32972#26223#20559#31227
          TabOrder = 0
          object Label1: TLabel
            Left = 6
            Top = 21
            Width = 18
            Height = 12
            Caption = 'X'#65306
          end
          object Label2: TLabel
            Left = 6
            Top = 46
            Width = 18
            Height = 12
            Caption = 'Y'#65306
          end
          object seNpcHPBgOffsetX: TSpinEditEx
            Left = 20
            Top = 16
            Width = 40
            Height = 21
            MaxValue = 0
            MinValue = 0
            TabOrder = 0
            Value = 0
            OnChange = seNpcHPBgOffsetXChange
          end
          object seNpcHPBgOffsetY: TSpinEditEx
            Left = 20
            Top = 41
            Width = 40
            Height = 21
            MaxValue = 0
            MinValue = 0
            TabOrder = 1
            Value = 0
            OnChange = seNpcHPBgOffsetYChange
          end
        end
        object GroupBox2: TGroupBox
          Left = 276
          Top = 16
          Width = 65
          Height = 68
          Caption = #34880#26465#20559#31227
          TabOrder = 1
          object Label3: TLabel
            Left = 6
            Top = 21
            Width = 18
            Height = 12
            Caption = 'X'#65306
          end
          object Label4: TLabel
            Left = 6
            Top = 46
            Width = 18
            Height = 12
            Caption = 'Y'#65306
          end
          object seNpcHPOffsetX: TSpinEditEx
            Left = 20
            Top = 17
            Width = 40
            Height = 21
            MaxValue = 0
            MinValue = 0
            TabOrder = 0
            Value = 0
            OnChange = seNpcHPOffsetXChange
          end
          object seNpcHPOffsetY: TSpinEditEx
            Left = 20
            Top = 42
            Width = 40
            Height = 21
            MaxValue = 0
            MinValue = 0
            TabOrder = 1
            Value = 0
            OnChange = seNpcHPOffsetYChange
          end
        end
        object GroupBox3: TGroupBox
          Left = 349
          Top = 16
          Width = 65
          Height = 68
          Caption = #34880#20540#20559#31227
          TabOrder = 2
          object Label5: TLabel
            Left = 6
            Top = 21
            Width = 18
            Height = 12
            Caption = 'X'#65306
          end
          object Label6: TLabel
            Left = 6
            Top = 46
            Width = 18
            Height = 12
            Caption = 'Y'#65306
          end
          object seNpcHPTextOffsetX: TSpinEditEx
            Left = 20
            Top = 17
            Width = 40
            Height = 21
            MaxValue = 0
            MinValue = 0
            TabOrder = 0
            Value = 0
            OnChange = seNpcHPTextOffsetXChange
          end
          object seNpcHPTextOffsetY: TSpinEditEx
            Left = 20
            Top = 42
            Width = 40
            Height = 21
            MaxValue = 0
            MinValue = 0
            TabOrder = 1
            Value = 0
            OnChange = seNpcHPTextOffsetYChange
          end
        end
        object GroupBox4: TGroupBox
          Left = 6
          Top = 16
          Width = 192
          Height = 68
          Caption = #36164#28304#35774#32622
          TabOrder = 3
          object Label7: TLabel
            Left = 8
            Top = 21
            Width = 60
            Height = 12
            Caption = #36164#28304#20301#32622#65306
          end
          object Label8: TLabel
            Left = 8
            Top = 46
            Width = 60
            Height = 12
            Caption = #24320#22987#22270#29255#65306
          end
          object cbbNpcHPFile: TComboBox
            Left = 64
            Top = 17
            Width = 121
            Height = 20
            Style = csDropDownList
            TabOrder = 0
            OnChange = cbbNpcHPFileChange
          end
          object seNpcHPStartIndex: TSpinEditEx
            Left = 64
            Top = 41
            Width = 121
            Height = 21
            Hint = #24320#22987#22270#29255#35774#32622#20026'-1'#26102#65292#20851#38381#33258#23450#20041#34880#26465#13#10#33258#23450#20041#34880#26465#20026#20004#24352#36830#32493#22270#29255#65292#21069#19968#24352#20026#34880#26465#36793#26694#65292#21518#19968#24352#20026#34880#26465
            MaxValue = 2100000000
            MinValue = -1
            ParentShowHint = False
            ShowHint = True
            TabOrder = 1
            Value = -1
            OnChange = seNpcHPStartIndexChange
          end
        end
      end
      object vstNpcAction: TVirtualStringTree
        Left = 5
        Top = 220
        Width = 764
        Height = 330
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
        Colors.UnfocusedColor = 15716484
        Colors.UnfocusedSelectionColor = 14803425
        Colors.UnfocusedSelectionBorderColor = 14803425
        Header.AutoSizeIndex = 2
        Header.Font.Charset = DEFAULT_CHARSET
        Header.Font.Color = clWindowText
        Header.Font.Height = -11
        Header.Font.Name = 'Tahoma'
        Header.Font.Style = []
        Header.Height = 32
        Header.Options = [hoAutoResize, hoColumnResize, hoDrag, hoShowSortGlyphs, hoVisible]
        HintMode = hmHint
        LineStyle = lsSolid
        Margin = 2
        ParentShowHint = False
        ScrollBarOptions.ScrollBars = ssVertical
        ShowHint = True
        TabOrder = 1
        TextMargin = 2
        TreeOptions.MiscOptions = [toAcceptOLEDrop, toCheckSupport, toEditable, toFullRepaintOnResize, toGridExtensions, toInitOnSave, toToggleOnDblClick, toWheelPanning, toEditOnClick]
        TreeOptions.PaintOptions = [toShowButtons, toShowDropmark, toShowHorzGridLines, toShowTreeLines, toShowVertGridLines, toThemeAware, toUseBlendedImages, toFullVertGridLines]
        TreeOptions.SelectionOptions = [toExtendedFocus, toFullRowSelect]
        OnBeforeCellPaint = vstNpcActionBeforeCellPaint
        OnChecked = vstNpcActionChecked
        OnCreateEditor = vstNpcActionCreateEditor
        OnDrawText = vstNpcActionDrawText
        OnEditing = vstNpcActionEditing
        OnGetText = vstNpcActionGetText
        OnGetHint = vstNpcActionGetHint
        OnGetNodeDataSize = vstNpcActionGetNodeDataSize
        OnNodeClick = vstNpcActionNodeClick
        Columns = <
          item
            Position = 0
            Width = 68
            WideText = #26041#21521
            WideHint = ' '
          end
          item
            Position = 1
            Width = 61
            WideText = #21160#20316#31867#22411
          end
          item
            Position = 2
            Width = 251
            WideText = #21160#20316#36164#28304#20301#32622
          end
          item
            Alignment = taRightJustify
            CaptionAlignment = taCenter
            Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coUseCaptionAlignment]
            Position = 3
            Width = 60
            WideText = #24320#22987#22270#29255
          end
          item
            Alignment = taRightJustify
            CaptionAlignment = taCenter
            Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coUseCaptionAlignment]
            Position = 4
            Width = 60
            WideText = #22270#29255#25968#37327
          end
          item
            Alignment = taRightJustify
            CaptionAlignment = taCenter
            Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coUseCaptionAlignment]
            Position = 5
            Width = 60
            WideText = #25773#25918#36895#24230
          end
          item
            Position = 6
            Width = 140
            WideText = #29305#25928#36164#28304#20301#32622
          end
          item
            Alignment = taRightJustify
            CaptionAlignment = taCenter
            Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coUseCaptionAlignment]
            Position = 7
            Width = 60
            WideText = #24320#22987#22270#29255
          end>
        WideDefaultText = ''
      end
      object GroupBox5: TGroupBox
        Left = 441
        Top = 0
        Width = 328
        Height = 90
        Caption = #25209#37327#35745#31639#22270#29255#20301#32622
        TabOrder = 2
        object Label9: TLabel
          Left = 9
          Top = 21
          Width = 60
          Height = 12
          Caption = #24320#22987#22270#29255#65306
        end
        object Label10: TLabel
          Left = 193
          Top = 21
          Width = 60
          Height = 12
          Caption = #25773#25918#25968#37327#65306
        end
        object Label11: TLabel
          Left = 9
          Top = 44
          Width = 60
          Height = 12
          Caption = #31354#30333#25968#37327#65306
        end
        object Label12: TLabel
          Left = 193
          Top = 44
          Width = 60
          Height = 12
          Caption = #26041#21521#25968#37327#65306
        end
        object seNpcCalcStartIndex: TSpinEditEx
          Left = 65
          Top = 16
          Width = 72
          Height = 21
          MaxValue = 2100000000
          MinValue = -1
          TabOrder = 0
          Value = -1
        end
        object seNpcCalcPlayCount: TSpinEditEx
          Left = 249
          Top = 16
          Width = 70
          Height = 21
          MaxValue = 2100000000
          MinValue = 0
          TabOrder = 1
          Value = 0
        end
        object seNpcCalcEmptyCount: TSpinEditEx
          Left = 65
          Top = 39
          Width = 72
          Height = 21
          MaxValue = 2100000000
          MinValue = 0
          TabOrder = 2
          Value = 0
        end
        object seNpcCalcDirCount: TSpinEditEx
          Left = 249
          Top = 39
          Width = 70
          Height = 21
          MaxValue = 8
          MinValue = 1
          TabOrder = 3
          Value = 8
        end
        object btnNpcCalcStand: TButton
          Left = 8
          Top = 63
          Width = 62
          Height = 22
          Caption = #31449#31435
          TabOrder = 4
          OnClick = btnNpcCalcStandClick
        end
        object btnNpcCalcStandEffect: TButton
          Left = 188
          Top = 63
          Width = 62
          Height = 22
          Caption = #31449#31435#29305#25928
          TabOrder = 5
          OnClick = btnNpcCalcStandClick
        end
        object btnNpcCalcHit: TButton
          Left = 76
          Top = 63
          Width = 62
          Height = 22
          Caption = #21160#20316
          TabOrder = 6
          OnClick = btnNpcCalcStandClick
        end
        object btnNpcCalcHitEffect: TButton
          Left = 256
          Top = 63
          Width = 62
          Height = 22
          Caption = #21160#20316#29305#25928
          TabOrder = 7
          OnClick = btnNpcCalcStandClick
        end
      end
      object grp12: TGroupBox
        Left = 545
        Top = 96
        Width = 223
        Height = 59
        Caption = #25209#37327#35774#32622#36164#28304#20301#32622
        TabOrder = 3
        object Label13: TLabel
          Left = 7
          Top = 18
          Width = 36
          Height = 12
          Caption = #25991#20214#65306
        end
        object btnNpcFileStandEffect: TButton
          Left = 106
          Top = 36
          Width = 52
          Height = 18
          Caption = #31449#31435#29305#25928
          TabOrder = 0
          OnClick = btnNpcFileStandClick
        end
        object btnNpcFileActionEffect: TButton
          Left = 162
          Top = 36
          Width = 52
          Height = 18
          Caption = #21160#20316#29305#25928
          TabOrder = 1
          OnClick = btnNpcFileStandClick
        end
        object cbbNpcBatchFile: TComboBox
          Left = 39
          Top = 14
          Width = 178
          Height = 20
          Style = csDropDownList
          TabOrder = 2
        end
        object btnNpcFileStand: TButton
          Left = 38
          Top = 36
          Width = 30
          Height = 18
          Caption = #31449#31435
          TabOrder = 3
          OnClick = btnNpcFileStandClick
        end
        object btnNpcFileAction: TButton
          Left = 72
          Top = 36
          Width = 30
          Height = 18
          Caption = #21160#20316
          TabOrder = 4
          OnClick = btnNpcFileStandClick
        end
      end
      object GroupBox6: TGroupBox
        Left = 207
        Top = 96
        Width = 224
        Height = 118
        Caption = #29305#25928#25345#20037#25773#25918
        TabOrder = 4
        object Label14: TLabel
          Left = 7
          Top = 21
          Width = 60
          Height = 12
          Caption = #36164#28304#20301#32622#65306
        end
        object Label15: TLabel
          Left = 8
          Top = 45
          Width = 60
          Height = 12
          Caption = #24320#22987#22270#29255#65306
        end
        object Label16: TLabel
          Left = 8
          Top = 69
          Width = 60
          Height = 12
          Caption = #22270#29255#25968#37327#65306
        end
        object Label17: TLabel
          Left = 131
          Top = 69
          Width = 36
          Height = 12
          Caption = #36895#24230#65306
        end
        object Label18: TLabel
          Left = 8
          Top = 93
          Width = 72
          Height = 12
          Caption = #20559#31227#22352#26631' X'#65306
        end
        object Label19: TLabel
          Left = 149
          Top = 93
          Width = 18
          Height = 12
          Caption = 'Y'#65306
        end
        object cbbNpcKeepPlayFile: TComboBox
          Left = 63
          Top = 17
          Width = 150
          Height = 20
          Style = csDropDownList
          TabOrder = 0
          OnChange = cbbNpcKeepPlayFileChange
        end
        object seNpcKeepPlayIndex: TSpinEditEx
          Left = 63
          Top = 41
          Width = 63
          Height = 21
          MaxValue = 2100000000
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 1
          Value = 0
          OnChange = seNpcKeepPlayIndexChange
        end
        object seNpcKeepPlayCount: TSpinEditEx
          Left = 63
          Top = 65
          Width = 62
          Height = 21
          MaxValue = 2100000000
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 2
          Value = 0
          OnChange = seNpcKeepPlayCountChange
        end
        object seNpcKeepPlayTime: TSpinEditEx
          Left = 164
          Top = 65
          Width = 48
          Height = 21
          MaxValue = 2100000000
          MinValue = 50
          ParentShowHint = False
          ShowHint = True
          TabOrder = 3
          Value = 50
          OnChange = seNpcKeepPlayTimeChange
        end
        object chkNpcKeepPlayBlendDraw: TCheckBox
          Left = 144
          Top = 43
          Width = 69
          Height = 17
          Caption = #36879#26126#32472#21046
          TabOrder = 4
          OnClick = chkNpcKeepPlayBlendDrawClick
        end
        object seKeepPlayOffsetX: TSpinEditEx
          Left = 76
          Top = 89
          Width = 49
          Height = 21
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 5
          Value = 0
          OnChange = seKeepPlayOffsetXChange
        end
        object seKeepPlayOffsetY: TSpinEditEx
          Left = 164
          Top = 89
          Width = 48
          Height = 21
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 6
          Value = 0
          OnChange = seKeepPlayOffsetYChange
        end
      end
      object GroupBox7: TGroupBox
        Left = 545
        Top = 155
        Width = 223
        Height = 59
        Caption = #25209#37327#25773#25918#36895#24230
        TabOrder = 5
        object Label20: TLabel
          Left = 7
          Top = 19
          Width = 36
          Height = 12
          Caption = #36895#24230#65306
        end
        object seNpcBatchTime: TSpinEditEx
          Left = 39
          Top = 14
          Width = 178
          Height = 21
          MaxValue = 2100000000
          MinValue = 0
          TabOrder = 0
          Value = 0
        end
        object btnNpcTimeStand: TButton
          Left = 39
          Top = 37
          Width = 64
          Height = 17
          Caption = #31449#31435
          TabOrder = 1
          OnClick = btnNpcTimeStandClick
        end
        object btnNpcTimeAction: TButton
          Left = 153
          Top = 37
          Width = 64
          Height = 17
          Caption = #21160#20316
          TabOrder = 2
          OnClick = btnNpcTimeStandClick
        end
      end
      object grp13: TGroupBox
        Left = 5
        Top = 96
        Width = 198
        Height = 118
        Caption = #32472#21046#27169#24335
        TabOrder = 6
        object lbl27: TLabel
          Left = 8
          Top = 21
          Width = 60
          Height = 12
          Caption = #31449#31435#32472#21046#65306
        end
        object Label21: TLabel
          Left = 8
          Top = 45
          Width = 60
          Height = 12
          Caption = #31449#31435#29305#25928#65306
        end
        object Label22: TLabel
          Left = 8
          Top = 69
          Width = 60
          Height = 12
          Caption = #21160#20316#32472#21046#65306
        end
        object Label23: TLabel
          Left = 8
          Top = 93
          Width = 60
          Height = 12
          Caption = #21160#20316#29305#25928#65306
        end
        object cbbNpcStandDrawMode: TComboBox
          Left = 65
          Top = 17
          Width = 126
          Height = 20
          Style = csDropDownList
          TabOrder = 0
          OnChange = cbbNpcStandDrawModeChange
        end
        object cbbNpcStandEffectDrawMode: TComboBox
          Left = 65
          Top = 41
          Width = 126
          Height = 20
          Style = csDropDownList
          TabOrder = 1
          OnChange = cbbNpcStandEffectDrawModeChange
        end
        object cbbNpcActionDrawMode: TComboBox
          Left = 64
          Top = 65
          Width = 126
          Height = 20
          Style = csDropDownList
          TabOrder = 2
          OnChange = cbbNpcActionDrawModeChange
        end
        object cbbNpcActionEffectDrawMode: TComboBox
          Left = 64
          Top = 89
          Width = 126
          Height = 20
          Style = csDropDownList
          TabOrder = 3
          OnChange = cbbNpcActionEffectDrawModeChange
        end
      end
      object grp14: TGroupBox
        Left = 441
        Top = 96
        Width = 100
        Height = 118
        Caption = #32472#21046#39034#24207
        TabOrder = 7
        object lstNpcDrawOrder: TListBox
          Left = 10
          Top = 18
          Width = 60
          Height = 92
          Style = lbOwnerDrawFixed
          ItemHeight = 18
          Items.Strings = (
            #25345#20037#25773#25918
            #35282#33394#32472#21046
            #29305#25928#25773#25918)
          TabOrder = 0
        end
        object btnNpcMoveTop: TButton
          Left = 73
          Top = 30
          Width = 19
          Height = 25
          Caption = #8593
          TabOrder = 1
          OnClick = btnNpcMoveTopClick
        end
        object btnNpcMoveBottom: TButton
          Left = 73
          Top = 57
          Width = 19
          Height = 25
          Caption = #8595
          TabOrder = 2
          OnClick = btnNpcMoveTopClick
        end
      end
    end
  end
  object pnlNpcBottom: TPanel
    Left = 0
    Top = 559
    Width = 896
    Height = 73
    Align = alBottom
    BevelOuter = bvNone
    TabOrder = 1
    ExplicitTop = 558
    ExplicitWidth = 892
    object lbl28: TLabel
      Left = 6
      Top = 3
      Width = 462
      Height = 13
      AutoSize = False
      Caption = #33258#23450#20041'NPC'#24418#35937#20195#30721#20174'10000'#24320#22987#65292#22312' Merchant.txt'#20687#26222#36890'NPC'#19968#26679#28155#21152#24182#37325#26032#21152#36733'NPC'
      Font.Charset = GB2312_CHARSET
      Font.Color = clRed
      Font.Height = -12
      Font.Name = #23435#20307
      Font.Style = []
      ParentFont = False
    end
    object lbl30: TLabel
      Left = 6
      Top = 36
      Width = 348
      Height = 12
      Caption = #30431#37325#30465'/'#27979#35797' 3 324 342 '#27979#35797' 0 10000[324,348] 0 0 0 3 3000 3'
      Font.Charset = GB2312_CHARSET
      Font.Color = clRed
      Font.Height = -12
      Font.Name = #23435#20307
      Font.Style = []
      ParentFont = False
    end
    object lbl31: TLabel
      Left = 6
      Top = 52
      Width = 561
      Height = 12
      Caption = '10000[324,348]'#65306'10000'#20026#24418#35937#65307'[324,348]'#34920#31034#24033#36923#22352#26631#20026'324,342-324,348'#65307#24033#36923#22352#26631#21487#19981#35201
      Font.Charset = GB2312_CHARSET
      Font.Color = clRed
      Font.Height = -12
      Font.Name = #23435#20307
      Font.Style = [fsBold]
      ParentFont = False
    end
    object lbl32: TLabel
      Left = 688
      Top = 4
      Width = 132
      Height = 12
      Caption = #24033#36923'NPC'#31227#21160#38388#38548' ['#31186']'#65306
    end
    object lbl29: TLabel
      Left = 6
      Top = 20
      Width = 570
      Height = 12
      Caption = #33050#26412#21517#31216' '#22320#22270' X Y NPC'#21517' '#26631#35782' '#24418#35937' '#23646#20110#27801#24052#20811'(0/1) '#38543#26426#31227#21160'(0/1) '#31227#21160#38388#38548' '#33258#21160#21464#33394'(0/1) '#21464#33394#38388#38548
      Font.Charset = GB2312_CHARSET
      Font.Color = clRed
      Font.Height = -12
      Font.Name = #23435#20307
      Font.Style = []
      ParentFont = False
    end
    object btnSaveNpc: TButton
      Left = 687
      Top = 40
      Width = 67
      Height = 25
      Caption = #20445#23384
      Enabled = False
      TabOrder = 0
      OnClick = btnSaveNpcClick
    end
    object btnSaveNpcToFile: TButton
      Left = 764
      Top = 40
      Width = 129
      Height = 25
      Caption = #29983#25104#30331#24405#22120#38598#25104#25991#20214
      TabOrder = 1
      OnClick = btnSaveNpcToFileClick
    end
    object chkSendCustomNPCConfig: TCheckBox
      Left = 688
      Top = 21
      Width = 120
      Height = 17
      Hint = #22914#26524#38598#25104#21040#30331#24405#22120#21017#21487#20197#19981#21457#36865
      Caption = #21457#36865#37197#32622#21040#23458#25143#31471
      ParentShowHint = False
      ShowHint = True
      TabOrder = 2
      OnClick = chkSendCustomNPCConfigClick
    end
    object seCustomNpcMoveTime: TSpinEditEx
      Left = 816
      Top = 0
      Width = 77
      Height = 21
      Hint = #35774#32622#21518#37325#21551#29983#25928
      MaxValue = 1000000
      MinValue = 1
      ParentShowHint = False
      ShowHint = True
      TabOrder = 3
      Value = 1
      OnChange = seCustomNpcMoveTimeChange
    end
  end
  object dlgSaveNpcs: TSaveDialog
    Filter = 'NPC'#37197#32622#25991#20214'(*.dat)|*.dat'
    Title = #29983#25104#30331#24405#22120#37197#32622#25991#20214
    Left = 48
    Top = 16
  end
  object ilCheck: TImageList
    Height = 13
    Width = 13
    Left = 16
    Top = 16
    Bitmap = {
      494C010102000A0004000D000D00FFFFFFFFFF10FFFFFFFFFFFFFFFF424D3600
      0000000000003600000028000000340000000D0000000100200000000000900A
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
      F400F4F4F4008F8F8E0000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000008F8F8E00F4F4F400CCCBCA00D5D4
      D400DCDBDB00E1E1E000E7E7E600EBEBEA00ECECEB00ECEBEB00EAE9E900F4F4
      F4008F8F8E008F8F8E00F4F4F400CCCBCA00DBDADA00E9E2DF00BA998C00BD9D
      9000F6F3F200EDEDEC00ECEBEB00EAE9E900F4F4F4008F8F8E00000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00008F8F8E00F4F4F400C6C4C200E9E9E900EDEDED00F0F0F000F4F4F400F6F6
      F600F6F6F600F6F6F600E6E6E600F4F4F4008F8F8E008F8F8E00F4F4F400CAC8
      C600F0ECEA00BB998B00975F4A0098614C00D1B9B000F9F9F900F6F6F600E6E6
      E600F4F4F4008F8F8E0000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000008F8F8E00F4F4F400C2BFBC00E5E4
      E300E9E9E900EDEDED00F2F2F200F4F4F400F5F5F500F4F4F400E2E2E100F4F4
      F4008F8F8E008F8F8E00F4F4F400D1CFCD00E9E1DE00955D4800965F49009760
      4B00A4736100FAF9F800F4F4F400E2E2E100F4F4F4008F8F8E00000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00008F8F8E00F4F4F400BFBBB800E1DFDD00E5E5E400EAEAEA00EFEFEF00F2F2
      F200F2F2F200F2F2F200DEDDDC00F4F4F4008F8F8E008F8F8E00F4F4F400E1E0
      DE00AA7F6E00945C4700E2D4CF00A778670097604B00D5BFB700F6F6F600DEDD
      DC00F4F4F4008F8F8E0000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000008F8F8E00F4F4F400BCB7B200DCD8
      D500DFDCDA00E3E1E000E8E8E800ECECEC00EDEDED00EDEDED00D6D5D400F4F4
      F4008F8F8E008F8F8E00F4F4F400CDC9C500DDCFC900C8AEA300EEEEED00D5C1
      BA00965E4900A5766400F8F8F800D6D5D500F4F4F4008F8F8E00000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00008F8F8E00F4F4F400B9B3AE00D7D1CD00D9D4D000DBD7D400DFDDDB00E3E2
      E100E6E6E500E8E8E800CDCDCC00F4F4F4008F8F8E008F8F8E00F4F4F400B9B3
      AE00DDD9D500E5E2DF00DCD8D500F4F3F200A1715E00945C4700D6C3BC00DCDC
      DB00F4F4F4008F8F8E0000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000008F8F8E00F4F4F400B9B3AE00D5CF
      CB00D5CFCB00D6D1CD00DAD5D200DEDBD800E1DFDD00E4E3E200C8C7C600F4F4
      F4008F8F8E008F8F8E00F4F4F400B9B3AE00D5CFCB00D5CFCB00D6D1CD00E6E2
      E000CFB8AF00925A4500A5776500E8E7E700F4F4F4008F8F8E00000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00008F8F8E00F4F4F400B9B3AE00D5CFCB00D5CFCB00D5CFCB00D5CFCB00D8D3
      D000DCD8D500DFDDDB00C5C3C100F4F4F4008F8F8E008F8F8E00F4F4F400B9B3
      AE00D5CFCB00D5CFCB00D5CFCB00D6D0CC00F1EEED009D6A5700925A4400D0BF
      B900F6F6F6008F8F8E0000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000008F8F8E00F4F4F400B9B3AE00B9B3
      AE00B9B3AE00B9B3AE00B9B3AE00B9B3AE00BAB4AF00BDB9B400C1BEBB00F4F4
      F4008F8F8E008F8F8E00F4F4F400B9B3AE00B9B3AE00B9B3AE00B9B3AE00B9B3
      AE00D0CCC900C0A79D00AB867700E4DFDC00F5F5F5008F8F8E00000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00008F8F8E00F4F4F400F4F4F400F4F4F400F4F4F400F4F4F400F4F4F400F4F4
      F400F4F4F400F4F4F400F4F4F400F4F4F4008F8F8E008F8F8E00F4F4F400F4F4
      F400F4F4F400F4F4F400F4F4F400F4F4F400F4F4F400F8F8F800F9F9F900F6F6
      F600F4F4F4008F8F8E0000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000008F8F8E008F8F8E008F8F8E008F8F
      8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F
      8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F
      8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F8E00000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000424D3E000000000000003E00000028000000340000000D00000001000100
      00000000680000000000000000000000000000000000000000000000FFFFFF00
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000000000000000000000000000}
  end
end
