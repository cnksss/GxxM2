object FrmMagicCD: TFrmMagicCD
  Left = 425
  Top = 285
  BorderStyle = bsDialog
  Caption = #25216#33021'CD'#35774#32622
  ClientHeight = 429
  ClientWidth = 369
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
  object lbl4: TLabel
    Left = 8
    Top = 405
    Width = 36
    Height = 12
    Caption = #25216#33021#65306
  end
  object vstMagicCD: TVirtualStringTree
    Left = 8
    Top = 78
    Width = 353
    Height = 312
    Colors.GridLineColor = 12303291
    Colors.UnfocusedSelectionColor = 14803425
    Colors.UnfocusedSelectionBorderColor = 14803425
    DefaultNodeHeight = 22
    Header.AutoSizeIndex = -1
    Header.DefaultHeight = 24
    Header.Font.Charset = GB2312_CHARSET
    Header.Font.Color = clWindowText
    Header.Font.Height = -12
    Header.Font.Name = #23435#20307
    Header.Font.Style = []
    Header.Height = 24
    Header.Options = [hoAutoResize, hoColumnResize, hoDrag, hoShowHint, hoShowSortGlyphs, hoVisible]
    HintMode = hmHint
    LineStyle = lsSolid
    Margin = 3
    ParentShowHint = False
    ShowHint = True
    TabOrder = 0
    TextMargin = 3
    TreeOptions.MiscOptions = [toAcceptOLEDrop, toCheckSupport, toEditable, toFullRepaintOnResize, toInitOnSave, toToggleOnDblClick, toWheelPanning, toEditOnClick]
    TreeOptions.PaintOptions = [toHideFocusRect, toShowButtons, toShowDropmark, toShowHorzGridLines, toShowVertGridLines, toThemeAware, toUseBlendedImages, toFullVertGridLines]
    TreeOptions.SelectionOptions = [toExtendedFocus, toFullRowSelect]
    OnBeforeItemErase = vstMagicCDBeforeItemErase
    OnCreateEditor = vstMagicCDCreateEditor
    OnDrawText = vstMagicCDDrawText
    OnEditing = vstMagicCDEditing
    OnFreeNode = vstMagicCDFreeNode
    OnGetText = vstMagicCDGetText
    OnKeyDown = vstMagicCDKeyDown
    OnNodeClick = vstMagicCDNodeClick
    Columns = <
      item
        CaptionAlignment = taCenter
        Margin = 1
        Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coUseCaptionAlignment]
        Position = 0
        Spacing = 1
        Width = 60
        WideText = #25216#33021'ID'
      end
      item
        CaptionAlignment = taCenter
        Margin = 1
        Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coUseCaptionAlignment]
        Position = 1
        Spacing = 1
        Width = 140
        WideText = #25216#33021#21517#31216
        WideHint = #21333#20301#65306#27627#31186
      end
      item
        Margin = 1
        Position = 2
        Spacing = 1
        Width = 149
        WideText = #20919#30830#26102#38388' ['#27627#31186']'
      end>
  end
  object btnOK: TButton
    Left = 286
    Top = 398
    Width = 75
    Height = 25
    Caption = #30830#23450
    TabOrder = 1
    OnClick = btnOKClick
  end
  object GroupBox1: TGroupBox
    Left = 8
    Top = 5
    Width = 353
    Height = 68
    Caption = #25216#33021#20919#30830#26102#38388#26410#21040#25552#31034
    TabOrder = 2
    object lbl1: TLabel
      Left = 105
      Top = 20
      Width = 36
      Height = 12
      Caption = #20869#23481#65306
    end
    object lbl2: TLabel
      Left = 8
      Top = 20
      Width = 36
      Height = 12
      Caption = #20301#32622#65306
    end
    object Label1: TLabel
      Left = 8
      Top = 45
      Width = 36
      Height = 12
      Caption = #32972#26223#65306
    end
    object Label2: TLabel
      Left = 105
      Top = 45
      Width = 36
      Height = 12
      Caption = #25991#23383#65306
    end
    object lbl3: TLabel
      Left = 205
      Top = 45
      Width = 18
      Height = 12
      Caption = 'X'#65306
    end
    object Label4: TLabel
      Left = 280
      Top = 45
      Width = 18
      Height = 12
      Caption = 'Y'#65306
    end
    object edtMagicCDMsgText: TEdit
      Left = 138
      Top = 16
      Width = 208
      Height = 20
      TabOrder = 0
      Text = #25216#33021#23578#26410#20919#30830#65292#35831#31561#24453'%time'#31186
    end
    object cbbMagicCDMsgType: TComboBox
      Left = 40
      Top = 16
      Width = 58
      Height = 20
      Style = csDropDownList
      ItemHeight = 12
      TabOrder = 1
      Items.Strings = (
        #32842#22825#26694
        #23631#24149
        #26080#25552#31034)
    end
    object seMagicCDFColor: TColorIndexEdit
      Left = 138
      Top = 41
      Width = 58
      Height = 21
      MaxLength = 3
      MaxValue = 255
      MinValue = 0
      TabOrder = 2
      Value = 255
    end
    object seMagicCDBColor: TColorIndexEdit
      Left = 40
      Top = 40
      Width = 58
      Height = 21
      MaxLength = 3
      MaxValue = 255
      MinValue = 0
      TabOrder = 3
      Value = 255
    end
    object seMagicCDShowX: TSpinEditEx
      Left = 219
      Top = 40
      Width = 52
      Height = 21
      MaxValue = 0
      MinValue = 0
      TabOrder = 4
      Value = 0
    end
    object seMagicCDShowY: TSpinEditEx
      Left = 294
      Top = 40
      Width = 52
      Height = 21
      MaxValue = 0
      MinValue = 0
      TabOrder = 5
      Value = 0
    end
  end
  object edtMagicName: TEdit
    Left = 40
    Top = 401
    Width = 113
    Height = 20
    TabOrder = 3
    OnKeyDown = edtMagicNameKeyDown
  end
  object btnSearch: TButton
    Left = 155
    Top = 399
    Width = 41
    Height = 24
    Caption = #26597#25214
    TabOrder = 4
    OnClick = btnSearchClick
  end
  object btnSearchNext: TButton
    Left = 199
    Top = 399
    Width = 48
    Height = 24
    Caption = #19979#19968#20010
    TabOrder = 5
    OnClick = btnSearchNextClick
  end
  object dlgOpen1: TOpenDialog
    Filter = #25216#33021#25968#25454#24211'(Magic.DB)|*.DB'
    Left = 152
    Top = 96
  end
end
