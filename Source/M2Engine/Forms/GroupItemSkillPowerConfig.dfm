object FrmGroupItemSkillPower: TFrmGroupItemSkillPower
  Left = 1062
  Top = 259
  BorderStyle = bsDialog
  Caption = #22871#35013#25216#33021#23041#21147#30334#20998#27604
  ClientHeight = 599
  ClientWidth = 506
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'Tahoma'
  Font.Style = []
  Position = poOwnerFormCenter
  OnCreate = FormCreate
  TextHeight = 13
  object StringGridSkillPower: TStringGrid
    Left = 8
    Top = 8
    Width = 489
    Height = 553
    ColCount = 3
    DefaultColWidth = 150
    Options = [goFixedVertLine, goFixedHorzLine, goVertLine, goHorzLine, goRangeSelect, goEditing]
    TabOrder = 0
    RowHeights = (
      24
      24
      24
      24
      24)
  end
  object Button1: TButton
    Left = 424
    Top = 568
    Width = 75
    Height = 25
    Caption = #30830#23450
    TabOrder = 1
    OnClick = Button1Click
  end
end
