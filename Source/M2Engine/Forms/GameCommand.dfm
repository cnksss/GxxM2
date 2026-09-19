object frmGameCmd: TfrmGameCmd
  Left = 648
  Top = 232
  BorderIcons = [biSystemMenu, biMinimize]
  BorderStyle = bsSingle
  BorderWidth = 6
  Caption = #28216#25103#21629#20196#35774#32622
  ClientHeight = 438
  ClientWidth = 712
  Color = clBtnFace
  Font.Charset = GB2312_CHARSET
  Font.Color = clWindowText
  Font.Height = -12
  Font.Name = #23435#20307
  Font.Style = []
  Position = poMainFormCenter
  OnCreate = FormCreate
  TextHeight = 12
  object pgcMain: TPageControl
    Left = 0
    Top = 0
    Width = 712
    Height = 438
    ActivePage = TabSheet1
    Align = alClient
    HotTrack = True
    TabOrder = 0
    ExplicitWidth = 708
    ExplicitHeight = 437
    object TabSheet1: TTabSheet
      BorderWidth = 5
      Caption = #26222#36890#21629#20196
      object GroupBox1: TGroupBox
        Left = 0
        Top = 318
        Width = 694
        Height = 82
        Align = alBottom
        Caption = #21629#20196#35774#32622
        TabOrder = 0
        ExplicitTop = 317
        ExplicitWidth = 690
        DesignSize = (
          694
          82)
        object Label1: TLabel
          Left = 8
          Top = 20
          Width = 54
          Height = 12
          Caption = #21629#20196#21517#31216':'
        end
        object Label6: TLabel
          Left = 280
          Top = 18
          Width = 54
          Height = 12
          Caption = #25152#38656#26435#38480':'
        end
        object lblUserCmdDesc: TLabel
          Left = 64
          Top = 40
          Width = 500
          Height = 12
          AutoSize = False
          Font.Charset = ANSI_CHARSET
          Font.Color = clBlue
          Font.Height = -12
          Font.Name = #23435#20307
          Font.Style = []
          ParentFont = False
        end
        object lblUserCmd: TLabel
          Left = 64
          Top = 60
          Width = 500
          Height = 12
          AutoSize = False
          Font.Charset = ANSI_CHARSET
          Font.Color = clBlue
          Font.Height = -12
          Font.Name = #23435#20307
          Font.Style = []
          ParentFont = False
        end
        object Label2: TLabel
          Left = 8
          Top = 40
          Width = 54
          Height = 12
          Caption = #21629#20196#21151#33021':'
        end
        object Label3: TLabel
          Left = 8
          Top = 60
          Width = 54
          Height = 12
          Caption = #21629#20196#26684#24335':'
        end
        object edtUserCmdName: TEdit
          Left = 64
          Top = 16
          Width = 178
          Height = 20
          ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
          TabOrder = 0
          OnChange = edtUserCmdNameChange
        end
        object seUserCmdPermission: TSpinEditEx
          Left = 340
          Top = 15
          Width = 45
          Height = 21
          MaxValue = 10
          MinValue = 0
          TabOrder = 1
          Value = 10
          OnChange = seUserCmdPermissionChange
        end
        object btnUserCmdOK: TButton
          Left = 619
          Top = 14
          Width = 65
          Height = 25
          Anchors = [akTop, akRight]
          Caption = #30830#23450'(&O)'
          TabOrder = 2
          OnClick = btnUserCmdOKClick
          ExplicitLeft = 615
        end
        object btnUserCmdSave: TButton
          Left = 619
          Top = 48
          Width = 65
          Height = 25
          Anchors = [akTop, akRight]
          Caption = #20445#23384'(&S)'
          TabOrder = 3
          OnClick = btnUserCmdSaveClick
          ExplicitLeft = 615
        end
      end
      object vstUserCmd: TVirtualStringTree
        Left = 0
        Top = 0
        Width = 694
        Height = 318
        Align = alClient
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
        Colors.UnfocusedColor = 7925500
        Colors.UnfocusedSelectionColor = 15987699
        Colors.UnfocusedSelectionBorderColor = 15987699
        DefaultNodeHeight = 20
        Header.AutoSizeIndex = 5
        Header.DefaultHeight = 24
        Header.Font.Charset = DEFAULT_CHARSET
        Header.Font.Color = clWindowText
        Header.Font.Height = -11
        Header.Font.Name = 'Tahoma'
        Header.Font.Style = []
        Header.Height = 24
        Header.Options = [hoAutoResize, hoColumnResize, hoDrag, hoShowSortGlyphs, hoVisible]
        HintMode = hmTooltip
        ParentShowHint = False
        ShowHint = True
        TabOrder = 1
        TreeOptions.PaintOptions = [toShowButtons, toShowDropmark, toShowHorzGridLines, toShowVertGridLines, toThemeAware, toUseBlendedImages]
        TreeOptions.SelectionOptions = [toFullRowSelect]
        OnAfterItemPaint = vstUserCmdAfterItemPaint
        OnBeforeItemErase = vstUserCmdBeforeItemErase
        OnFocusChanged = vstUserCmdFocusChanged
        OnFreeNode = vstUserCmdFreeNode
        OnGetText = vstUserCmdGetText
        ExplicitWidth = 690
        ExplicitHeight = 317
        Columns = <
          item
            Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coShowDropMark, coVisible, coAllowFocus]
            Position = 0
            Width = 40
            WideText = #32534#21495
          end
          item
            Position = 1
            Width = 100
            WideText = #28216#25103#21629#20196
          end
          item
            Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coAllowFocus]
            Position = 2
            Width = 40
            WideText = #20998#26435
          end
          item
            Alignment = taCenter
            Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coAllowFocus]
            Position = 3
            Width = 60
            WideText = #25152#38656#26435#38480
          end
          item
            Position = 4
            Width = 160
            WideText = #21629#20196#26684#24335
          end
          item
            Position = 5
            Width = 390
            WideText = #21629#20196#35828#26126
          end>
      end
    end
    object TabSheet2: TTabSheet
      BorderWidth = 5
      Caption = #31649#29702#21629#20196
      ImageIndex = 1
      object GroupBox2: TGroupBox
        Left = 0
        Top = 318
        Width = 694
        Height = 82
        Align = alBottom
        Caption = #21629#20196#35774#32622
        TabOrder = 0
        ExplicitTop = 317
        ExplicitWidth = 690
        DesignSize = (
          694
          82)
        object Label4: TLabel
          Left = 8
          Top = 20
          Width = 54
          Height = 12
          Caption = #21629#20196#21517#31216':'
        end
        object Label5: TLabel
          Left = 280
          Top = 20
          Width = 54
          Height = 12
          Caption = #25152#38656#26435#38480':'
        end
        object lblAdminCmdDesc: TLabel
          Left = 64
          Top = 40
          Width = 500
          Height = 12
          AutoSize = False
          Font.Charset = ANSI_CHARSET
          Font.Color = clBlue
          Font.Height = -12
          Font.Name = #23435#20307
          Font.Style = []
          ParentFont = False
        end
        object lblAdminCmd: TLabel
          Left = 64
          Top = 60
          Width = 500
          Height = 12
          AutoSize = False
          Font.Charset = ANSI_CHARSET
          Font.Color = clBlue
          Font.Height = -12
          Font.Name = #23435#20307
          Font.Style = []
          ParentFont = False
        end
        object Label7: TLabel
          Left = 8
          Top = 40
          Width = 54
          Height = 12
          Caption = #21629#20196#21151#33021':'
        end
        object Label8: TLabel
          Left = 8
          Top = 60
          Width = 54
          Height = 12
          Caption = #21629#20196#26684#24335':'
        end
        object edtAdminCmdName: TEdit
          Left = 64
          Top = 16
          Width = 178
          Height = 20
          ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
          TabOrder = 0
          OnChange = edtAdminCmdNameChange
        end
        object seAdminCmdPermission: TSpinEditEx
          Left = 340
          Top = 15
          Width = 45
          Height = 21
          MaxValue = 10
          MinValue = 0
          TabOrder = 1
          Value = 10
          OnChange = seAdminCmdPermissionChange
        end
        object btnAdminCmdOK: TButton
          Left = 623
          Top = 16
          Width = 65
          Height = 25
          Anchors = [akTop, akRight]
          Caption = #30830#23450'(&O)'
          TabOrder = 2
          OnClick = btnAdminCmdOKClick
          ExplicitLeft = 619
        end
        object btnAdminCmdSave: TButton
          Left = 623
          Top = 48
          Width = 65
          Height = 25
          Anchors = [akTop, akRight]
          Caption = #20445#23384'(&S)'
          TabOrder = 3
          OnClick = btnAdminCmdSaveClick
          ExplicitLeft = 619
        end
      end
      object vstAdminCmd: TVirtualStringTree
        Left = 0
        Top = 0
        Width = 694
        Height = 318
        Align = alClient
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
        Colors.UnfocusedColor = 7925500
        Colors.UnfocusedSelectionColor = 15987699
        Colors.UnfocusedSelectionBorderColor = 15987699
        DefaultNodeHeight = 20
        Header.AutoSizeIndex = 5
        Header.DefaultHeight = 24
        Header.Font.Charset = DEFAULT_CHARSET
        Header.Font.Color = clWindowText
        Header.Font.Height = -11
        Header.Font.Name = 'Tahoma'
        Header.Font.Style = []
        Header.Height = 24
        Header.Options = [hoAutoResize, hoColumnResize, hoDrag, hoShowSortGlyphs, hoVisible]
        HintMode = hmTooltip
        ParentShowHint = False
        ShowHint = True
        TabOrder = 1
        TreeOptions.PaintOptions = [toShowButtons, toShowDropmark, toShowHorzGridLines, toShowVertGridLines, toThemeAware, toUseBlendedImages]
        TreeOptions.SelectionOptions = [toFullRowSelect]
        OnAfterItemPaint = vstUserCmdAfterItemPaint
        OnBeforeItemErase = vstUserCmdBeforeItemErase
        OnFocusChanged = vstAdminCmdFocusChanged
        OnFreeNode = vstUserCmdFreeNode
        OnGetText = vstUserCmdGetText
        ExplicitHeight = 319
        Columns = <
          item
            Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coShowDropMark, coVisible, coAllowFocus]
            Position = 0
            Width = 40
            WideText = #32534#21495
          end
          item
            Position = 1
            Width = 100
            WideText = #28216#25103#21629#20196
          end
          item
            Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coShowDropMark, coVisible, coAllowFocus]
            Position = 2
            Width = 40
            WideText = #20998#26435
          end
          item
            Alignment = taCenter
            Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coShowDropMark, coVisible, coAllowFocus]
            Position = 3
            Width = 46
            WideText = #26435#38480
          end
          item
            Position = 4
            Width = 160
            WideText = #21629#20196#26684#24335
          end
          item
            Position = 5
            Width = 308
            WideText = #21629#20196#35828#26126
          end>
      end
    end
    object TabSheet3: TTabSheet
      BorderWidth = 5
      Caption = #35843#35797#21629#20196
      ImageIndex = 2
      object GroupBox3: TGroupBox
        Left = 0
        Top = 318
        Width = 694
        Height = 82
        Align = alBottom
        Caption = #21629#20196#35774#32622
        TabOrder = 0
        ExplicitTop = 317
        ExplicitWidth = 690
        DesignSize = (
          694
          82)
        object Label9: TLabel
          Left = 8
          Top = 20
          Width = 54
          Height = 12
          Caption = #21629#20196#21517#31216':'
        end
        object Label10: TLabel
          Left = 280
          Top = 18
          Width = 54
          Height = 12
          Caption = #25152#38656#26435#38480':'
        end
        object lblDebugCmdDesc: TLabel
          Left = 64
          Top = 40
          Width = 500
          Height = 12
          AutoSize = False
          Font.Charset = ANSI_CHARSET
          Font.Color = clBlue
          Font.Height = -12
          Font.Name = #23435#20307
          Font.Style = []
          ParentFont = False
        end
        object lblDebugCmd: TLabel
          Left = 64
          Top = 60
          Width = 500
          Height = 12
          AutoSize = False
          Font.Charset = ANSI_CHARSET
          Font.Color = clBlue
          Font.Height = -12
          Font.Name = #23435#20307
          Font.Style = []
          ParentFont = False
        end
        object Label11: TLabel
          Left = 8
          Top = 40
          Width = 54
          Height = 12
          Caption = #21629#20196#21151#33021':'
        end
        object Label12: TLabel
          Left = 8
          Top = 60
          Width = 54
          Height = 12
          Caption = #21629#20196#26684#24335':'
        end
        object edtDebugCmdName: TEdit
          Left = 64
          Top = 16
          Width = 178
          Height = 20
          ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
          TabOrder = 0
          OnChange = edtDebugCmdNameChange
        end
        object seDebugCmdPermission: TSpinEditEx
          Left = 340
          Top = 15
          Width = 45
          Height = 21
          MaxValue = 10
          MinValue = 0
          TabOrder = 1
          Value = 10
          OnChange = seDebugCmdPermissionChange
        end
        object btnDebugCmdOK: TButton
          Left = 619
          Top = 16
          Width = 65
          Height = 25
          Anchors = [akTop, akRight]
          Caption = #30830#23450'(&O)'
          TabOrder = 2
          OnClick = btnDebugCmdOKClick
          ExplicitLeft = 615
        end
        object btnDebugCmdSave: TButton
          Left = 619
          Top = 48
          Width = 65
          Height = 25
          Anchors = [akTop, akRight]
          Caption = #20445#23384'(&S)'
          TabOrder = 3
          OnClick = btnDebugCmdSaveClick
          ExplicitLeft = 615
        end
      end
      object vstDebugCmd: TVirtualStringTree
        Left = 0
        Top = 0
        Width = 694
        Height = 318
        Align = alClient
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
        Colors.UnfocusedColor = 7925500
        Colors.UnfocusedSelectionColor = 15987699
        Colors.UnfocusedSelectionBorderColor = 15987699
        DefaultNodeHeight = 20
        Header.AutoSizeIndex = 5
        Header.DefaultHeight = 24
        Header.Font.Charset = DEFAULT_CHARSET
        Header.Font.Color = clWindowText
        Header.Font.Height = -11
        Header.Font.Name = 'Tahoma'
        Header.Font.Style = []
        Header.Height = 24
        Header.Options = [hoAutoResize, hoColumnResize, hoDrag, hoShowSortGlyphs, hoVisible]
        HintMode = hmTooltip
        ParentShowHint = False
        ShowHint = True
        TabOrder = 1
        TreeOptions.PaintOptions = [toShowButtons, toShowDropmark, toShowHorzGridLines, toShowVertGridLines, toThemeAware, toUseBlendedImages]
        TreeOptions.SelectionOptions = [toFullRowSelect]
        OnAfterItemPaint = vstUserCmdAfterItemPaint
        OnBeforeItemErase = vstUserCmdBeforeItemErase
        OnFocusChanged = vstDebugCmdFocusChanged
        OnFreeNode = vstUserCmdFreeNode
        OnGetText = vstUserCmdGetText
        ExplicitWidth = 690
        ExplicitHeight = 317
        Columns = <
          item
            Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coShowDropMark, coVisible, coAllowFocus]
            Position = 0
            Width = 40
            WideText = #32534#21495
          end
          item
            Position = 1
            Width = 100
            WideText = #28216#25103#21629#20196
          end
          item
            Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coShowDropMark, coVisible, coAllowFocus]
            Position = 2
            Width = 40
            WideText = #20998#26435
          end
          item
            Alignment = taCenter
            Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coShowDropMark, coVisible, coAllowFocus]
            Position = 3
            Width = 46
            WideText = #26435#38480
          end
          item
            Position = 4
            Width = 160
            WideText = #21629#20196#26684#24335
          end
          item
            Position = 5
            Width = 308
            WideText = #21629#20196#35828#26126
          end>
      end
    end
  end
end
