object frmRouteEdit: TfrmRouteEdit
  Left = 409
  Top = 188
  BorderIcons = [biSystemMenu]
  BorderStyle = bsSingle
  Caption = #20462#25913#32593#20851#36335#30001
  ClientHeight = 461
  ClientWidth = 451
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
  object Label1: TLabel
    Left = 8
    Top = 12
    Width = 54
    Height = 12
    Caption = #35282#33394#32593#20851':'
  end
  object EditSelGate: TEdit
    Left = 64
    Top = 8
    Width = 97
    Height = 20
    TabOrder = 0
    OnChange = EditSelGateChange
  end
  object GroupBox1: TGroupBox
    Left = 8
    Top = 32
    Width = 436
    Height = 117
    Caption = #20027#28216#25103#32593#20851' '#65288#32593#20851'IP - '#32593#20851#36830#25509#31471#21475' - '#32593#20851#26816#27979#31471#21475#65289
    TabOrder = 1
    object Label2: TLabel
      Left = 8
      Top = 24
      Width = 18
      Height = 12
      Caption = #19968':'
    end
    object Label3: TLabel
      Left = 8
      Top = 47
      Width = 18
      Height = 12
      Caption = #20108':'
    end
    object Label4: TLabel
      Left = 8
      Top = 70
      Width = 18
      Height = 12
      Caption = #19977':'
    end
    object Label5: TLabel
      Left = 8
      Top = 93
      Width = 18
      Height = 12
      Caption = #22235':'
    end
    object Label6: TLabel
      Left = 224
      Top = 24
      Width = 18
      Height = 12
      Caption = #20116':'
    end
    object Label7: TLabel
      Left = 224
      Top = 48
      Width = 18
      Height = 12
      Caption = #20845':'
    end
    object Label8: TLabel
      Left = 224
      Top = 72
      Width = 18
      Height = 12
      Caption = #19971':'
    end
    object Label9: TLabel
      Left = 224
      Top = 93
      Width = 18
      Height = 12
      Caption = #20843':'
    end
    object edtGateIP1: TEdit
      Left = 28
      Top = 20
      Width = 97
      Height = 20
      TabOrder = 0
      OnChange = EditSelGateChange
    end
    object edtGateIP2: TEdit
      Left = 28
      Top = 43
      Width = 97
      Height = 20
      TabOrder = 2
      OnChange = EditSelGateChange
    end
    object edtGatePort1: TEdit
      Left = 127
      Top = 20
      Width = 41
      Height = 20
      TabOrder = 1
      OnChange = EditSelGateChange
    end
    object edtGatePort2: TEdit
      Left = 127
      Top = 43
      Width = 41
      Height = 20
      TabOrder = 3
      OnChange = EditSelGateChange
    end
    object edtGateIP3: TEdit
      Left = 28
      Top = 66
      Width = 97
      Height = 20
      TabOrder = 4
      OnChange = EditSelGateChange
    end
    object edtGatePort3: TEdit
      Left = 127
      Top = 66
      Width = 41
      Height = 20
      TabOrder = 5
      OnChange = EditSelGateChange
    end
    object edtGateIP4: TEdit
      Left = 28
      Top = 89
      Width = 97
      Height = 20
      TabOrder = 6
      OnChange = EditSelGateChange
    end
    object edtGatePort4: TEdit
      Left = 127
      Top = 89
      Width = 41
      Height = 20
      TabOrder = 7
      OnChange = EditSelGateChange
    end
    object edtGateIP5: TEdit
      Left = 244
      Top = 20
      Width = 97
      Height = 20
      TabOrder = 8
      OnChange = EditSelGateChange
    end
    object edtGatePort5: TEdit
      Left = 343
      Top = 20
      Width = 41
      Height = 20
      TabOrder = 9
      OnChange = EditSelGateChange
    end
    object edtGateIP6: TEdit
      Left = 244
      Top = 43
      Width = 97
      Height = 20
      TabOrder = 10
      OnChange = EditSelGateChange
    end
    object edtGatePort6: TEdit
      Left = 343
      Top = 43
      Width = 41
      Height = 20
      TabOrder = 11
      OnChange = EditSelGateChange
    end
    object edtGateIP7: TEdit
      Left = 244
      Top = 66
      Width = 97
      Height = 20
      TabOrder = 12
      OnChange = EditSelGateChange
    end
    object edtGatePort7: TEdit
      Left = 343
      Top = 66
      Width = 41
      Height = 20
      TabOrder = 13
      OnChange = EditSelGateChange
    end
    object edtGateIP8: TEdit
      Left = 244
      Top = 89
      Width = 97
      Height = 20
      TabOrder = 14
      OnChange = EditSelGateChange
    end
    object edtGatePort8: TEdit
      Left = 343
      Top = 89
      Width = 41
      Height = 20
      TabOrder = 15
      OnChange = EditSelGateChange
    end
    object edtDBPort1: TEdit
      Left = 170
      Top = 20
      Width = 41
      Height = 20
      TabOrder = 16
      OnChange = EditSelGateChange
    end
    object edtDBPort2: TEdit
      Left = 170
      Top = 43
      Width = 41
      Height = 20
      TabOrder = 17
      OnChange = EditSelGateChange
    end
    object edtDBPort3: TEdit
      Left = 170
      Top = 66
      Width = 41
      Height = 20
      TabOrder = 18
      OnChange = EditSelGateChange
    end
    object edtDBPort4: TEdit
      Left = 170
      Top = 89
      Width = 41
      Height = 20
      TabOrder = 19
      OnChange = EditSelGateChange
    end
    object edtDBPort5: TEdit
      Left = 387
      Top = 20
      Width = 41
      Height = 20
      TabOrder = 20
      OnChange = EditSelGateChange
    end
    object edtDBPort6: TEdit
      Left = 387
      Top = 43
      Width = 41
      Height = 20
      TabOrder = 21
      OnChange = EditSelGateChange
    end
    object edtDBPort7: TEdit
      Left = 387
      Top = 66
      Width = 41
      Height = 20
      TabOrder = 22
      OnChange = EditSelGateChange
    end
    object edtDBPort8: TEdit
      Left = 387
      Top = 89
      Width = 41
      Height = 20
      TabOrder = 23
      OnChange = EditSelGateChange
    end
  end
  object grp1: TGroupBox
    Left = 8
    Top = 152
    Width = 436
    Height = 273
    TabOrder = 2
    object lbl1: TLabel
      Left = 8
      Top = 21
      Width = 84
      Height = 12
      Caption = #24403#20027#28216#25103#32593#20851#26377
    end
    object lbl2: TLabel
      Left = 148
      Top = 21
      Width = 192
      Height = 12
      Caption = #20010#19981#33021#36830#25509#26102#65292#19981#20877#20998#37197#20027#28216#25103#32593#20851
    end
    object chkOpenRunGate2: TCheckBox
      Left = 8
      Top = -2
      Width = 117
      Height = 17
      Caption = #21551#29992#22791#29992#28216#25103#32593#20851
      TabOrder = 0
      OnClick = chkOpenRunGate2Click
    end
    object seGameGateDisconnectCount: TSpinEditEx
      Left = 96
      Top = 16
      Width = 49
      Height = 21
      Enabled = False
      MaxValue = 8
      MinValue = 1
      TabOrder = 1
      Value = 1
    end
    object vstRunGate: TVirtualStringTree
      Left = 8
      Top = 40
      Width = 417
      Height = 204
      CheckImageKind = ckXP
      Colors.GridLineColor = 12303291
      Colors.UnfocusedSelectionColor = clHighlight
      Colors.UnfocusedSelectionBorderColor = clHighlight
      DefaultNodeHeight = 20
      Enabled = False
      Header.AutoSizeIndex = 1
      Header.Font.Charset = DEFAULT_CHARSET
      Header.Font.Color = clWindowText
      Header.Font.Height = -11
      Header.Font.Name = 'Tahoma'
      Header.Font.Style = []
      Header.Height = 25
      Header.Options = [hoAutoResize, hoColumnResize, hoDrag, hoShowSortGlyphs, hoVisible]
      HintMode = hmHint
      LineStyle = lsSolid
      Margin = 2
      ParentShowHint = False
      ShowHint = True
      TabOrder = 2
      TextMargin = 2
      TreeOptions.MiscOptions = [toAcceptOLEDrop, toCheckSupport, toEditable, toFullRepaintOnResize, toGridExtensions, toInitOnSave, toToggleOnDblClick, toWheelPanning, toEditOnClick]
      TreeOptions.PaintOptions = [toShowButtons, toShowDropmark, toShowHorzGridLines, toShowTreeLines, toShowVertGridLines, toThemeAware, toUseBlendedImages, toFullVertGridLines]
      TreeOptions.SelectionOptions = [toExtendedFocus, toFullRowSelect]
      OnCreateEditor = vstRunGateCreateEditor
      OnEditing = vstRunGateEditing
      OnGetText = vstRunGateGetText
      OnPaintText = vstRunGatePaintText
      OnKeyDown = vstRunGateKeyDown
      OnNodeClick = vstRunGateNodeClick
      Columns = <
        item
          Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coShowDropMark, coVisible, coAllowFocus]
          Position = 0
          Width = 60
          WideText = #32534#21495
        end
        item
          Position = 1
          Width = 113
          WideText = #32593#20851'IP'
          WideHint = ' '
        end
        item
          Alignment = taRightJustify
          CaptionAlignment = taCenter
          Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coUseCaptionAlignment]
          Position = 2
          Width = 80
          WideText = #36830#25509#31471#21475
        end
        item
          Position = 3
          Width = 80
          WideText = #26816#27979#31471#21475
        end
        item
          Alignment = taRightJustify
          CaptionAlignment = taCenter
          Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coWrapCaption, coUseCaptionAlignment]
          Position = 4
          Width = 80
          WideText = #20248#20808#32423#21035
        end>
      WideDefaultText = ''
    end
    object btnAdd: TButton
      Left = 309
      Top = 247
      Width = 56
      Height = 20
      Caption = #26032#22686
      Enabled = False
      TabOrder = 3
      OnClick = btnAddClick
    end
    object btnDel: TButton
      Left = 368
      Top = 247
      Width = 56
      Height = 20
      Caption = #21024#38500
      Enabled = False
      TabOrder = 4
      OnClick = btnDelClick
    end
  end
  object btnCancel: TButton
    Left = 369
    Top = 430
    Width = 73
    Height = 25
    Cancel = True
    Caption = #21462#28040'(&C)'
    ModalResult = 2
    TabOrder = 3
  end
  object btnOK: TButton
    Left = 289
    Top = 430
    Width = 73
    Height = 25
    Caption = #30830#23450'(&O)'
    TabOrder = 4
    OnClick = btnOKClick
  end
end
