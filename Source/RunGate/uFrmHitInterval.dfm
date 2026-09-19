object FrmHitInterval: TFrmHitInterval
  Left = 192
  Top = 130
  BorderStyle = bsDialog
  BorderWidth = 5
  Caption = #25915#20987#38388#38548#35774#32622
  ClientHeight = 518
  ClientWidth = 195
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'Tahoma'
  Font.Style = []
  OldCreateOrder = False
  PixelsPerInch = 96
  TextHeight = 13
  object grpSetting: TGroupBox
    Left = 0
    Top = 0
    Width = 195
    Height = 487
    Align = alClient
    Caption = #25915#20987#38388#38548#35774#32622
    TabOrder = 0
    DesignSize = (
      195
      487)
    object grdHitInterval: TStringGrid
      Left = 6
      Top = 20
      Width = 183
      Height = 460
      Anchors = [akLeft, akTop, akBottom]
      ColCount = 2
      DefaultColWidth = 80
      DefaultRowHeight = 18
      RowCount = 100
      Options = [goFixedVertLine, goFixedHorzLine, goVertLine, goHorzLine, goEditing]
      TabOrder = 0
      RowHeights = (
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18)
    end
  end
  object pnlBottom: TPanel
    Left = 0
    Top = 487
    Width = 195
    Height = 31
    Align = alBottom
    BevelOuter = bvNone
    TabOrder = 1
    object btn1: TButton
      Left = 119
      Top = 4
      Width = 75
      Height = 25
      Caption = #30830#23450
      TabOrder = 0
      OnClick = btn1Click
    end
  end
end
