object FrmLogManage: TFrmLogManage
  Left = 199
  Top = 164
  Caption = #26085#24535#26597#35810
  ClientHeight = 849
  ClientWidth = 1330
  Color = clBtnFace
  Font.Charset = GB2312_CHARSET
  Font.Color = clWindowText
  Font.Height = -12
  Font.Name = #23435#20307
  Font.Style = []
  OldCreateOrder = False
  Position = poMainFormCenter
  OnCreate = FormCreate
  OnDestroy = FormDestroy
  PixelsPerInch = 96
  TextHeight = 12
  object Panel: TPanel
    Left = 0
    Top = 0
    Width = 1330
    Height = 36
    Align = alTop
    BevelOuter = bvNone
    TabOrder = 0
    object Label1: TLabel
      Left = 8
      Top = 12
      Width = 54
      Height = 12
      Caption = #24320#22987#26085#26399':'
    end
    object Label2: TLabel
      Left = 167
      Top = 12
      Width = 54
      Height = 12
      Caption = #32467#26463#26085#26399':'
    end
    object DateTimeEditBegin: TRzDateTimeEdit
      Left = 61
      Top = 8
      Width = 92
      Height = 20
      EditType = etDate
      OnDateTimeChange = DateTimeEditBeginDateTimeChange
      TabOrder = 0
    end
    object DateTimeEditEnd: TRzDateTimeEdit
      Left = 220
      Top = 8
      Width = 92
      Height = 20
      EditType = etDate
      OnDateTimeChange = DateTimeEditEndDateTimeChange
      TabOrder = 1
    end
    object btnStart: TButton
      Left = 1224
      Top = 5
      Width = 75
      Height = 25
      Caption = #24320#22987#26597#35810
      TabOrder = 12
      OnClick = btnStartClick
    end
    object chkObjName: TCheckBox
      Left = 332
      Top = 9
      Width = 70
      Height = 17
      Caption = #35282#33394#21517#31216
      TabOrder = 2
    end
    object edtObjName: TEdit
      Left = 400
      Top = 8
      Width = 99
      Height = 20
      TabOrder = 3
    end
    object chkItemName: TCheckBox
      Left = 880
      Top = 9
      Width = 59
      Height = 17
      Caption = #29289#21697#21517
      TabOrder = 8
    end
    object edtItemName: TEdit
      Left = 938
      Top = 8
      Width = 99
      Height = 20
      TabOrder = 9
    end
    object chkItemID: TCheckBox
      Left = 1058
      Top = 9
      Width = 59
      Height = 17
      Caption = #29289#21697'ID'
      TabOrder = 10
    end
    object edtItemID: TEdit
      Left = 1116
      Top = 8
      Width = 99
      Height = 20
      TabOrder = 11
    end
    object chkActObjName: TCheckBox
      Left = 692
      Top = 9
      Width = 70
      Height = 17
      Caption = #30446#26631#23545#35937
      TabOrder = 6
    end
    object edtActObjName: TEdit
      Left = 760
      Top = 8
      Width = 99
      Height = 20
      TabOrder = 7
    end
    object chkObjType: TCheckBox
      Left = 524
      Top = 9
      Width = 70
      Height = 17
      Caption = #35282#33394#31867#22411
      TabOrder = 4
    end
    object cbbActionType: TComboBox
      Left = 594
      Top = 8
      Width = 75
      Height = 20
      Style = csDropDownList
      ItemHeight = 12
      TabOrder = 5
    end
  end
  object StatusBar: TStatusBar
    Left = 0
    Top = 830
    Width = 1330
    Height = 19
    Panels = <
      item
        Width = 100
      end
      item
        Width = 100
      end
      item
        Width = 400
      end
      item
        Width = 50
      end>
  end
  object pnlClient: TPanel
    Left = 0
    Top = 36
    Width = 1330
    Height = 794
    Align = alClient
    TabOrder = 2
    object splLeft: TSplitter
      Left = 137
      Top = 1
      Width = 4
      Height = 792
    end
    object vstLog: TVirtualStringTree
      Left = 141
      Top = 1
      Width = 1188
      Height = 792
      Align = alClient
      DefaultNodeHeight = 20
      Header.AutoSizeIndex = 12
      Header.Font.Charset = DEFAULT_CHARSET
      Header.Font.Color = clWindowText
      Header.Font.Height = -11
      Header.Font.Name = 'Tahoma'
      Header.Font.Style = []
      Header.Options = [hoAutoResize, hoColumnResize, hoDrag, hoShowSortGlyphs, hoVisible]
      HintAnimation = hatFade
      HintMode = hmTooltip
      LineStyle = lsSolid
      ParentShowHint = False
      PopupMenu = PopupMenu
      ShowHint = True
      TabOrder = 0
      TreeOptions.MiscOptions = [toAcceptOLEDrop, toFullRepaintOnResize, toGridExtensions, toInitOnSave, toToggleOnDblClick, toWheelPanning, toEditOnClick]
      TreeOptions.PaintOptions = [toShowDropmark, toShowHorzGridLines, toShowVertGridLines, toThemeAware, toUseBlendedImages, toFullVertGridLines]
      TreeOptions.SelectionOptions = [toExtendedFocus, toFullRowSelect, toMultiSelect]
      OnBeforeItemErase = vstLogBeforeItemErase
      OnCompareNodes = vstLogCompareNodes
      OnDrawText = vstLogDrawText
      OnGetText = vstLogGetText
      OnHeaderClick = vstLogHeaderClick
      OnKeyAction = vstLogKeyAction
      Columns = <
        item
          Position = 0
          WideText = #24207#21495
        end
        item
          Position = 1
          Width = 90
          WideText = #21160#20316
        end
        item
          Position = 2
          Width = 60
          WideText = #22320#22270
        end
        item
          Position = 3
          Width = 46
          WideText = #22352#26631'X'
        end
        item
          Position = 4
          Width = 46
          WideText = #22352#26631'Y'
        end
        item
          Position = 5
          Width = 90
          WideText = #35282#33394#21517#31216
        end
        item
          Position = 6
          Width = 65
          WideText = #35282#33394#31867#22411
        end
        item
          Position = 7
          Width = 90
          WideText = #29289#21697#21517#31216
        end
        item
          Position = 8
          Width = 80
          WideText = #29289#21697'ID'
        end
        item
          Position = 9
          Width = 120
          WideText = #30446#26631#23545#35937
        end
        item
          Position = 10
          Width = 80
          WideText = #26032#25968#25454
        end
        item
          Position = 11
          Width = 69
          WideText = #21442#32771#25968#25454
        end
        item
          Position = 12
          Width = 168
          WideText = #25551#36848
        end
        item
          Position = 13
          Width = 130
          WideText = #26102#38388
        end>
    end
    object vstLogType: TVirtualStringTree
      Left = 1
      Top = 1
      Width = 136
      Height = 792
      Align = alLeft
      CheckImageKind = ckXP
      Header.AutoSizeIndex = 0
      Header.Font.Charset = DEFAULT_CHARSET
      Header.Font.Color = clWindowText
      Header.Font.Height = -11
      Header.Font.Name = 'Tahoma'
      Header.Font.Style = []
      Header.MainColumn = -1
      TabOrder = 1
      TreeOptions.MiscOptions = [toAcceptOLEDrop, toCheckSupport, toFullRepaintOnResize, toInitOnSave, toToggleOnDblClick, toWheelPanning]
      TreeOptions.PaintOptions = [toShowButtons, toShowDropmark, toShowTreeLines, toThemeAware, toUseBlendedImages]
      TreeOptions.SelectionOptions = [toFullRowSelect]
      OnCollapsing = vstLogTypeCollapsing
      OnDrawText = vstLogTypeDrawText
      OnGetText = vstLogTypeGetText
      Columns = <>
    end
  end
  object PopupMenu: TPopupMenu
    OnPopup = PopupMenuPopup
    Left = 368
    Top = 248
    object pmiCopy: TMenuItem
      Caption = #22797#21046
      OnClick = pmiCopyClick
    end
    object pmiCopyLine: TMenuItem
      Caption = #22797#21046#36873#20013#34892
      OnClick = pmiCopyLineClick
    end
    object N1: TMenuItem
      Caption = '-'
    end
    object pmiExportLine: TMenuItem
      Caption = #23548#20986#36873#20013#34892
      OnClick = pmiExportLineClick
    end
    object pmiExportAll: TMenuItem
      Caption = #23548#20986#25152#26377
      OnClick = pmiExportAllClick
    end
  end
  object Timer: TTimer
    Enabled = False
    Interval = 100
    OnTimer = TimerTimer
    Left = 424
    Top = 328
  end
  object dlgSave: TSaveDialog
    Filter = #25991#26412#25991#20214'(*.txt)|*.txt'
    Title = #23548#20986#26085#24535#25968#25454
    Left = 264
    Top = 264
  end
end
