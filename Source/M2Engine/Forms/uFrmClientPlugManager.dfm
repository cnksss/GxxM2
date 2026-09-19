object FrmClientPlugManager: TFrmClientPlugManager
  Left = 726
  Top = 423
  BorderStyle = bsDialog
  Caption = #23458#25143#31471#25554#20214#31649#29702
  ClientHeight = 243
  ClientWidth = 625
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'Tahoma'
  Font.Style = []
  Position = poMainFormCenter
  TextHeight = 13
  object ListBoxPlugin: TListBox
    Left = 9
    Top = 9
    Width = 518
    Height = 217
    ItemHeight = 13
    TabOrder = 0
  end
  object ButtonRef: TButton
    Left = 534
    Top = 9
    Width = 81
    Height = 27
    Caption = #21047#26032'(&R)'
    TabOrder = 1
    OnClick = ButtonRefClick
  end
end
