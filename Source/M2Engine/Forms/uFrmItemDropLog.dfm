object FrmItemDropLog: TFrmItemDropLog
  Left = 447
  Top = 327
  BorderStyle = bsDialog
  BorderWidth = 3
  Caption = 'FrmItemDropLog'
  ClientHeight = 501
  ClientWidth = 525
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'Tahoma'
  Font.Style = []
  KeyPreview = True
  Position = poMainFormCenter
  OnKeyPress = FormKeyPress
  TextHeight = 13
  object vstLogs: TVirtualStringTree
    Left = 0
    Top = 0
    Width = 525
    Height = 501
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
    Colors.UnfocusedColor = 15716828
    Colors.UnfocusedSelectionColor = 15987699
    Colors.UnfocusedSelectionBorderColor = 15987699
    DefaultNodeHeight = 22
    Header.AutoSizeIndex = 0
    Header.DefaultHeight = 24
    Header.Font.Charset = DEFAULT_CHARSET
    Header.Font.Color = clWindowText
    Header.Font.Height = -12
    Header.Font.Name = 'Tahoma'
    Header.Font.Style = []
    Header.Height = 26
    Header.Options = [hoColumnResize, hoDrag, hoShowSortGlyphs, hoVisible]
    ScrollBarOptions.AlwaysVisible = True
    ScrollBarOptions.ScrollBars = ssVertical
    TabOrder = 0
    TreeOptions.MiscOptions = [toAcceptOLEDrop, toCheckSupport, toFullRepaintOnResize, toGridExtensions, toInitOnSave, toToggleOnDblClick, toWheelPanning, toEditOnClick]
    TreeOptions.PaintOptions = [toShowButtons, toShowDropmark, toShowHorzGridLines, toShowTreeLines, toShowVertGridLines, toThemeAware, toUseBlendedImages, toFullVertGridLines]
    TreeOptions.SelectionOptions = [toExtendedFocus, toFullRowSelect]
    OnFreeNode = vstLogsFreeNode
    OnGetText = vstLogsGetText
    OnGetNodeDataSize = vstLogsGetNodeDataSize
    ExplicitWidth = 529
    ExplicitHeight = 502
    Columns = <
      item
        Alignment = taCenter
        Position = 0
        Width = 130
        WideText = #25481#33853#26102#38388
      end
      item
        Alignment = taCenter
        Position = 1
        Width = 100
        WideText = #29289#21697#25317#26377#32773
      end
      item
        Alignment = taCenter
        Position = 2
        Width = 80
        WideText = #25481#33853#24618#29289
      end
      item
        Alignment = taCenter
        Position = 3
        Width = 80
        WideText = #22320#22270
      end
      item
        Alignment = taCenter
        Position = 4
        Width = 80
        WideText = #22352#26631
      end>
  end
end
