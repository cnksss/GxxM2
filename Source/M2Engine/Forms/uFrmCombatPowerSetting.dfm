object FrmCombatPowerSetting: TFrmCombatPowerSetting
  Left = 470
  Top = 358
  BorderStyle = bsDialog
  BorderWidth = 4
  Caption = #25112#26007#21147#35745#31639#35774#32622
  ClientHeight = 544
  ClientWidth = 520
  Color = clBtnFace
  Font.Charset = GB2312_CHARSET
  Font.Color = clWindowText
  Font.Height = -12
  Font.Name = #23435#20307
  Font.Style = []
  OldCreateOrder = False
  Position = poMainFormCenter
  OnCreate = FormCreate
  PixelsPerInch = 96
  TextHeight = 12
  object pgcMain: TPageControl
    Left = 0
    Top = 0
    Width = 520
    Height = 511
    ActivePage = ts1
    Align = alClient
    TabOrder = 0
    object ts1: TTabSheet
      BorderWidth = 4
      Caption = #24120#35268#23646#24615
      ExplicitLeft = 0
      ExplicitTop = 0
      ExplicitWidth = 0
      ExplicitHeight = 0
      object vstDefPower: TVirtualStringTree
        Left = 0
        Top = 0
        Width = 504
        Height = 476
        Align = alClient
        DefaultNodeHeight = 20
        Header.AutoSizeIndex = 0
        Header.Font.Charset = DEFAULT_CHARSET
        Header.Font.Color = clWindowText
        Header.Font.Height = -11
        Header.Font.Name = 'Tahoma'
        Header.Font.Style = []
        Header.Height = 36
        Header.Options = [hoAutoResize, hoColumnResize, hoDrag, hoShowSortGlyphs, hoVisible]
        Header.PopupMenu = pmCopy
        HintMode = hmHint
        ParentShowHint = False
        ShowHint = True
        TabOrder = 0
        TreeOptions.MiscOptions = [toAcceptOLEDrop, toEditable, toFullRepaintOnResize, toInitOnSave, toToggleOnDblClick, toWheelPanning, toEditOnClick]
        TreeOptions.PaintOptions = [toShowDropmark, toShowHorzGridLines, toShowVertGridLines, toThemeAware, toUseBlendedImages]
        TreeOptions.SelectionOptions = [toExtendedFocus, toFullRowSelect]
        WantTabs = True
        OnBeforeItemErase = vstDefPowerBeforeItemErase
        OnChange = vstDefPowerChange
        OnCreateEditor = vstDefPowerCreateEditor
        OnEditing = vstDefPowerEditing
        OnGetText = vstDefPowerGetText
        OnGetHint = vstDefPowerGetHint
        OnKeyDown = vstDefPowerKeyDown
        OnNodeClick = vstDefPowerNodeClick
        Columns = <
          item
            Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible]
            Position = 0
            Width = 246
            WideText = '1'#28857#25110'1%'#30340#23646#24615#20540
          end
          item
            Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coWrapCaption]
            Position = 1
            Width = 86
            WideText = #25112#22763#25112#26007#21147'+'
          end
          item
            Position = 2
            Width = 86
            WideText = #27861#24072#25112#26007#21147'+'
          end
          item
            Position = 3
            Width = 86
            WideText = #36947#22763#25112#26007#21147'+'
          end>
      end
    end
    object ts2: TTabSheet
      BorderWidth = 4
      Caption = #33258#23450#20041#21464#37327
      ImageIndex = 1
      ExplicitLeft = 0
      ExplicitTop = 0
      ExplicitWidth = 0
      ExplicitHeight = 0
      DesignSize = (
        504
        475)
      object pnlVarTop: TPanel
        Left = 0
        Top = 0
        Width = 504
        Height = 28
        Align = alTop
        Anchors = [akTop, akRight]
        BevelOuter = bvNone
        TabOrder = 0
        object lbl35: TLabel
          Left = 96
          Top = 7
          Width = 60
          Height = 12
          Caption = #25628#32034#21464#37327#65306
        end
        object chkOpenCombatPowerVarCalc: TCheckBox
          Left = 4
          Top = 4
          Width = 77
          Height = 17
          Caption = #35745#31639#21464#37327
          TabOrder = 0
          OnClick = chkOpenCombatPowerVarCalcClick
        end
        object edtItemSearch: TEdit
          Left = 152
          Top = 2
          Width = 121
          Height = 20
          TabOrder = 1
          OnChange = edtItemSearchChange
        end
      end
      object pnlVarBotton: TPanel
        Left = 0
        Top = 433
        Width = 504
        Height = 42
        Align = alBottom
        BevelOuter = bvNone
        TabOrder = 1
        ExplicitTop = 434
        object lbl1: TLabel
          Left = 0
          Top = 6
          Width = 336
          Height = 12
          Caption = 'Ctrl+Insert'#28155#21152#65292'Ctrl+Delete'#21024#38500#65292'Ctrl/Shift'#28857#20987#21487#22810#36873#65307
          Font.Charset = GB2312_CHARSET
          Font.Color = clBlue
          Font.Height = -12
          Font.Name = #23435#20307
          Font.Style = []
          ParentFont = False
        end
        object lbl2: TLabel
          Left = 0
          Top = 24
          Width = 366
          Height = 12
          Caption = #21464#37327#21482#25903#25345'D'#65292'M'#65292'N'#65292'U'#65292'J'#65292'N$'#65307#19978#38754#25112#26007#21147#22686#21152#20026#21464#37327'=1'#26102#30340#25112#26007#21147
          Font.Charset = GB2312_CHARSET
          Font.Color = clBlue
          Font.Height = -12
          Font.Name = #23435#20307
          Font.Style = []
          ParentFont = False
        end
      end
      object vstVarPower: TVirtualStringTree
        Left = 0
        Top = 28
        Width = 504
        Height = 405
        Align = alClient
        DefaultNodeHeight = 20
        Header.AutoSizeIndex = 5
        Header.Font.Charset = DEFAULT_CHARSET
        Header.Font.Color = clWindowText
        Header.Font.Height = -11
        Header.Font.Name = 'Tahoma'
        Header.Font.Style = []
        Header.Height = 36
        Header.Options = [hoAutoResize, hoColumnResize, hoDrag, hoShowSortGlyphs, hoVisible]
        Header.PopupMenu = pmCopy
        TabOrder = 2
        TreeOptions.MiscOptions = [toAcceptOLEDrop, toEditable, toFullRepaintOnResize, toInitOnSave, toToggleOnDblClick, toWheelPanning, toEditOnClick]
        TreeOptions.PaintOptions = [toShowDropmark, toShowHorzGridLines, toShowVertGridLines, toThemeAware, toUseBlendedImages]
        TreeOptions.SelectionOptions = [toExtendedFocus, toFullRowSelect, toMultiSelect]
        WantTabs = True
        OnBeforeItemErase = vstDefPowerBeforeItemErase
        OnChange = vstVarPowerChange
        OnCreateEditor = vstVarPowerCreateEditor
        OnEditing = vstVarPowerEditing
        OnFreeNode = vstVarPowerFreeNode
        OnGetText = vstVarPowerGetText
        OnKeyDown = vstVarPowerKeyDown
        OnKeyUp = vstVarPowerKeyUp
        OnNodeClick = vstVarPowerNodeClick
        ExplicitHeight = 406
        Columns = <
          item
            Position = 0
            Width = 40
            WideText = #32534#21495
          end
          item
            Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coShowDropMark, coVisible]
            Position = 1
            Width = 80
            WideText = #21464#37327#21517#31216
          end
          item
            Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coWrapCaption]
            Position = 2
            Width = 81
            WideText = #25112#22763#25112#26007#21147'+'
          end
          item
            Position = 3
            Width = 81
            WideText = #27861#24072#25112#26007#21147'+'
          end
          item
            Position = 4
            Width = 81
            WideText = #36947#22763#25112#26007#21147'+'
          end
          item
            Position = 5
            Width = 141
            WideText = #22791#27880
          end>
      end
      object btnAddVar: TButton
        Left = 348
        Top = 1
        Width = 75
        Height = 23
        Anchors = [akTop, akRight]
        Caption = #28155#21152#21464#37327
        TabOrder = 3
        OnClick = btnAddVarClick
        ExplicitLeft = 340
      end
      object btnDelVar: TButton
        Left = 429
        Top = 1
        Width = 75
        Height = 23
        Anchors = [akTop, akRight]
        Caption = #21024#38500
        TabOrder = 4
        OnClick = btnDelVarClick
        ExplicitLeft = 421
      end
    end
  end
  object pnlBottom: TPanel
    Left = 0
    Top = 511
    Width = 520
    Height = 33
    Align = alBottom
    BevelOuter = bvNone
    TabOrder = 1
    DesignSize = (
      520
      33)
    object chkOpenCombatPowerCalc: TCheckBox
      Left = 4
      Top = 10
      Width = 237
      Height = 17
      Caption = #21551#29992#25112#26007#21147#35745#31639'    ('#25903#25345#36127#20540')'
      TabOrder = 0
      OnClick = chkOpenCombatPowerCalcClick
    end
    object btnRecalHumanCombatPower: TButton
      Left = 304
      Top = 6
      Width = 129
      Height = 25
      Hint = #24403#20154#29289#22312#32447#21518#65292#35843#25972#35774#32622#21487#33021#20250#23548#33268#20154#29289#25112#26007#21147#35745#31639#19981#20934#65292#25152#20197#35201#37325#26032#35745#31639
      Anchors = [akTop, akRight]
      Caption = #37325#31639#25152#26377#20154#29289#25112#26007#21147
      ParentShowHint = False
      ShowHint = True
      TabOrder = 1
      OnClick = btnRecalHumanCombatPowerClick
    end
    object btnOK: TButton
      Left = 445
      Top = 6
      Width = 75
      Height = 25
      Anchors = [akTop, akRight]
      Caption = #30830#23450
      TabOrder = 2
      OnClick = btnOKClick
    end
  end
  object pmCopy: TPopupMenu
    OnPopup = pmCopyPopup
    Left = 172
    Top = 207
    object mniCopy1: TMenuItem
      Caption = #20174#25112#22763#22797#21046#21040#27861#24072#20026'0'#30340#20540
      OnClick = mniCopy1Click
    end
    object mniCopy2: TMenuItem
      Caption = #20174#25112#22763#22797#21046#21040#36947#22763#20026'0'#30340#20540
      OnClick = mniCopy2Click
    end
  end
end
