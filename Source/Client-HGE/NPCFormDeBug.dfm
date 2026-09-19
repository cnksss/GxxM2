object FrmNPCDeBug: TFrmNPCDeBug
  Left = -118
  Top = 182
  Caption = 'NPC'#30028#38754#35843#35797
  ClientHeight = 400
  ClientWidth = 822
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'Tahoma'
  Font.Style = []
  OldCreateOrder = False
  PixelsPerInch = 96
  TextHeight = 13
  object Memo1: TMemo
    Left = 0
    Top = 0
    Width = 561
    Height = 305
    Font.Charset = DEFAULT_CHARSET
    Font.Color = clWindowText
    Font.Height = -12
    Font.Name = #23435#20307
    Font.Style = []
    ParentFont = False
    TabOrder = 0
  end
  object GroupBox1: TGroupBox
    Left = 3
    Top = 312
    Width = 814
    Height = 73
    Caption = 'NPC'#23545#35805#26694#32972#26223#35774#32622
    TabOrder = 1
    object Label1: TLabel
      Left = 16
      Top = 24
      Width = 52
      Height = 13
      Caption = #22270#29255#36164#28304':'
    end
    object Label2: TLabel
      Left = 228
      Top = 24
      Width = 52
      Height = 13
      Caption = #22270#29255#24207#21495':'
    end
    object ComboBox1: TComboBox
      Left = 76
      Top = 23
      Width = 145
      Height = 22
      Style = csOwnerDrawFixed
      ItemHeight = 16
      TabOrder = 0
    end
    object SpinEdit1: TSpinEdit
      Left = 288
      Top = 23
      Width = 73
      Height = 22
      MaxValue = 0
      MinValue = 0
      TabOrder = 1
      Value = 0
    end
    object Button1: TButton
      Left = 480
      Top = 24
      Width = 75
      Height = 25
      Caption = #24212#29992'(&A)'
      TabOrder = 2
      OnClick = Button1Click
    end
    object Button2: TButton
      Left = 576
      Top = 24
      Width = 75
      Height = 25
      Caption = #20445#23384'(&S)'
      TabOrder = 3
      OnClick = Button2Click
    end
  end
  object Memo2: TMemo
    Left = 576
    Top = 0
    Width = 241
    Height = 305
    Font.Charset = DEFAULT_CHARSET
    Font.Color = clWindowText
    Font.Height = -12
    Font.Name = #23435#20307
    Font.Style = []
    ParentFont = False
    TabOrder = 2
  end
end
