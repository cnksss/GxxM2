object FrmAttackSabukWall: TFrmAttackSabukWall
  Left = 713
  Top = 301
  BorderStyle = bsDialog
  Caption = 'FrmAttackSabukWall'
  ClientHeight = 235
  ClientWidth = 411
  Color = clBtnFace
  Font.Charset = GB2312_CHARSET
  Font.Color = clWindowText
  Font.Height = -12
  Font.Name = #23435#20307
  Font.Style = []
  TextHeight = 12
  object Label1: TLabel
    Left = 208
    Top = 52
    Width = 54
    Height = 12
    Caption = #34892#20250#21517#31216':'
  end
  object Label2: TLabel
    Left = 208
    Top = 76
    Width = 54
    Height = 12
    Caption = #25915#22478#26102#38388':'
  end
  object GroupBox1: TGroupBox
    Left = 8
    Top = 8
    Width = 193
    Height = 217
    Caption = #34892#20250#21015#34920
    TabOrder = 0
    object ListBoxGuild: TListBox
      Left = 8
      Top = 16
      Width = 177
      Height = 193
      ItemHeight = 12
      TabOrder = 0
      OnClick = ListBoxGuildClick
    end
  end
  object EditGuildName: TEdit
    Left = 264
    Top = 48
    Width = 137
    Height = 20
    TabOrder = 1
    Text = 'EditGuildName'
  end
  object RzDateTimeEditAttackDate: TDateTimePicker
    Left = 264
    Top = 72
    Width = 137
    Height = 20
    Date = 41597.000000000000000000
    Time = 0.809317083330825000
    TabOrder = 2
  end
  object ButtonOK: TButton
    Left = 256
    Top = 192
    Width = 75
    Height = 25
    Caption = #30830#23450'(&O)'
    TabOrder = 3
    OnClick = ButtonOKClick
  end
  object CheckBoxAll: TCheckBox
    Left = 208
    Top = 16
    Width = 73
    Height = 17
    Caption = #25152#26377#34892#20250
    TabOrder = 4
    OnClick = CheckBoxAllClick
  end
  object ButtonCancel: TButton
    Left = 336
    Top = 192
    Width = 75
    Height = 25
    Caption = #21462#28040'(&C)'
    TabOrder = 5
    OnClick = ButtonCancelClick
  end
end
