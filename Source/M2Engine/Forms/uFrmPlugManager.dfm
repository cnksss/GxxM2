object FrmPlugManager: TFrmPlugManager
  Left = 466
  Top = 318
  BorderStyle = bsDialog
  BorderWidth = 8
  Caption = #25554#20214#31649#29702
  ClientHeight = 449
  ClientWidth = 699
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'Tahoma'
  Font.Style = []
  Position = poMainFormCenter
  OnCreate = FormCreate
  TextHeight = 13
  object grpPlugInfo: TGroupBox
    Left = 238
    Top = 0
    Width = 461
    Height = 449
    Align = alClient
    Caption = #25554#20214#20449#24687
    TabOrder = 0
    ExplicitWidth = 457
    ExplicitHeight = 448
    DesignSize = (
      461
      449)
    object mmoPlugInfo: TMemo
      Left = 9
      Top = 19
      Width = 435
      Height = 420
      Anchors = [akLeft, akTop, akRight, akBottom]
      ReadOnly = True
      TabOrder = 0
      ExplicitWidth = 431
      ExplicitHeight = 419
    end
  end
  object grpPlugList: TGroupBox
    Left = 0
    Top = 0
    Width = 234
    Height = 449
    Align = alLeft
    Caption = #25554#20214#21015#34920
    TabOrder = 1
    ExplicitHeight = 448
    DesignSize = (
      234
      449)
    object vstPlug: TVirtualStringTree
      Left = 9
      Top = 19
      Width = 216
      Height = 391
      Anchors = [akLeft, akTop, akRight, akBottom]
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
      Colors.UnfocusedColor = 7334688
      Colors.UnfocusedSelectionColor = 15987699
      Colors.UnfocusedSelectionBorderColor = 15987699
      Header.AutoSizeIndex = 1
      Header.Font.Charset = DEFAULT_CHARSET
      Header.Font.Color = clWindowText
      Header.Font.Height = -11
      Header.Font.Name = 'Tahoma'
      Header.Font.Style = []
      Header.Height = 22
      Header.Options = [hoAutoResize, hoColumnResize, hoDrag, hoShowSortGlyphs, hoVisible]
      Header.PopupMenu = pmPlugList
      TabOrder = 0
      TreeOptions.PaintOptions = [toShowButtons, toShowDropmark, toShowHorzGridLines, toShowTreeLines, toShowVertGridLines, toThemeAware, toUseBlendedImages]
      TreeOptions.SelectionOptions = [toExtendedFocus, toFullRowSelect]
      OnBeforeItemErase = vstPlugBeforeItemErase
      OnFocusChanged = vstPlugFocusChanged
      OnFreeNode = vstPlugFreeNode
      OnGetText = vstPlugGetText
      OnPaintText = vstPlugPaintText
      ExplicitHeight = 390
      Columns = <
        item
          Position = 0
          Width = 40
          WideText = #32534#21495
        end
        item
          Position = 1
          Width = 172
          WideText = #25554#20214#21517#31216
        end>
    end
    object btnLoadPlug: TButton
      Left = 9
      Top = 417
      Width = 99
      Height = 25
      Caption = #21152#36733#25554#20214
      Enabled = False
      TabOrder = 1
      OnClick = mniLoadPlugClick
    end
    object btnUnloadPlug: TButton
      Left = 126
      Top = 417
      Width = 99
      Height = 25
      Caption = #21368#36733#25554#20214
      Enabled = False
      TabOrder = 2
      OnClick = mniUnloadPlugClick
    end
  end
  object pnlSpliter: TPanel
    Left = 234
    Top = 0
    Width = 4
    Height = 449
    Align = alLeft
    BevelOuter = bvNone
    TabOrder = 2
    ExplicitHeight = 448
  end
  object pmPlugList: TPopupMenu
    OnPopup = pmPlugListPopup
    Left = 120
    Top = 240
    object mniLoadPlug: TMenuItem
      Caption = #21152#36733#25554#20214
      OnClick = mniLoadPlugClick
    end
    object mniUnloadPlug: TMenuItem
      Caption = #21368#36733#25554#20214
      OnClick = mniUnloadPlugClick
    end
  end
end
