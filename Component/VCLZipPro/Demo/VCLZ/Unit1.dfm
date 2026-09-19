object Form1: TForm1
  Left = 0
  Top = 0
  Caption = 'Form1'
  ClientHeight = 156
  ClientWidth = 138
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'Tahoma'
  Font.Style = []
  OldCreateOrder = False
  PixelsPerInch = 96
  TextHeight = 13
  object Label1: TLabel
    Left = 8
    Top = 56
    Width = 6
    Height = 23
    Align = alCustom
    Font.Charset = DEFAULT_CHARSET
    Font.Color = clPurple
    Font.Height = -19
    Font.Name = 'Tahoma'
    Font.Style = [fsBold]
    ParentFont = False
  end
  object Button1: TButton
    Left = 8
    Top = 8
    Width = 121
    Height = 25
    Caption = #1057#1086#1079#1076#1072#1090#1100' '#1072#1088#1093#1080#1074
    Font.Charset = RUSSIAN_CHARSET
    Font.Color = clBlue
    Font.Height = -15
    Font.Name = 'Times New Roman'
    Font.Style = [fsBold]
    ParentFont = False
    TabOrder = 0
    OnClick = Button1Click
  end
  object Button2: TButton
    Left = 8
    Top = 104
    Width = 121
    Height = 25
    Caption = #1056#1072#1089#1087#1072#1082#1086#1074#1072#1090#1100
    Font.Charset = RUSSIAN_CHARSET
    Font.Color = clWindowText
    Font.Height = -15
    Font.Name = 'Times New Roman'
    Font.Style = [fsBold]
    ParentFont = False
    TabOrder = 1
    OnClick = Button2Click
  end
  object VCLZip1: TVCLZip
    ThisReleaseLevel = 'Release'
    MultiZipInfo.BlockSize = 1457600
    Left = 8
    Top = 48
  end
  object VCLUnZip1: TVCLUnZip
    ThisReleaseLevel = 'Release'
    Left = 88
    Top = 48
  end
end
