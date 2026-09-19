object ObjectsDlg: TObjectsDlg
  Left = 266
  Top = 126
  Width = 204
  Height = 382
  BorderStyle = bsSizeToolWin
  Caption = 'ObjectsDlg'
  Color = clBtnFace
  Font.Charset = GB2312_CHARSET
  Font.Color = clWindowText
  Font.Height = -12
  Font.Name = #23435#20307
  Font.Style = []
  OldCreateOrder = False
  OnCreate = FormCreate
  OnDestroy = FormDestroy
  PixelsPerInch = 96
  TextHeight = 12
  object JvInspector: TJvInspector
    Left = 0
    Top = 0
    Width = 188
    Height = 344
    Align = alClient
    ItemHeight = 16
    OnDataValueChanged = JvInspectorDataValueChanged
  end
end
