object FrmGlobalVarEdit: TFrmGlobalVarEdit
  Left = 322
  Top = 262
  BorderStyle = bsDialog
  Caption = 'FrmGlobalVarEdit'
  ClientHeight = 546
  ClientWidth = 733
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'Tahoma'
  Font.Style = []
  OldCreateOrder = False
  Position = poMainFormCenter
  PixelsPerInch = 96
  TextHeight = 13
  object strngrdVar: TStringGrid
    Left = 8
    Top = 8
    Width = 717
    Height = 497
    ColCount = 3
    Options = [goFixedVertLine, goFixedHorzLine, goVertLine, goHorzLine, goRangeSelect, goEditing]
    TabOrder = 0
    OnSetEditText = strngrdVarSetEditText
    ColWidths = (
      64
      333
      291)
  end
  object btnClearVar: TButton
    Left = 8
    Top = 512
    Width = 75
    Height = 25
    Caption = #20840#37096#28165#38500
    TabOrder = 1
    OnClick = btnClearVarClick
  end
  object btnRefreshVar: TButton
    Left = 88
    Top = 512
    Width = 75
    Height = 25
    Caption = #21047#26032#21464#37327#20540
    TabOrder = 2
    OnClick = btnRefreshVarClick
  end
  object btnSave: TButton
    Left = 532
    Top = 512
    Width = 93
    Height = 25
    Caption = #20445#23384#21464#37327#20462#25913
    TabOrder = 3
    OnClick = btnSaveClick
  end
  object btnSaveDesc: TButton
    Left = 632
    Top = 512
    Width = 93
    Height = 25
    Caption = #20445#23384#22791#27880#20462#25913
    TabOrder = 4
    OnClick = btnSaveDescClick
  end
end
