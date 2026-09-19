object FrmLogin: TFrmLogin
  Left = 478
  Top = 108
  BorderStyle = bsDialog
  Caption = 'FrmLogin'
  ClientHeight = 152
  ClientWidth = 508
  Color = clBtnFace
  Font.Charset = GB2312_CHARSET
  Font.Color = clWindowText
  Font.Height = -12
  Font.Name = #23435#20307
  Font.Style = []
  OldCreateOrder = False
  Position = poScreenCenter
  OnCreate = FormCreate
  PixelsPerInch = 96
  TextHeight = 12
  object Label1: TLabel
    Left = 16
    Top = 20
    Width = 48
    Height = 12
    Caption = #20256#22855#30446#24405
  end
  object DialogButtons: TRzDialogButtons
    Left = 0
    Top = 116
    Width = 508
    HotTrack = True
    OnClickOk = DialogButtonsClickOk
    OnClickCancel = DialogButtonsClickCancel
    TabOrder = 0
  end
  object EditGamePath: TRzButtonEdit
    Left = 72
    Top = 16
    Width = 417
    Height = 20
    ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
    TabOrder = 1
    OnButtonClick = EditGamePathButtonClick
  end
  object RadioGroup: TRzRadioGroup
    Left = 16
    Top = 48
    Width = 473
    Height = 57
    Columns = 4
    ItemIndex = 3
    Items.Strings = (
      '1.76'
      '1.85'
      #33521#38596#29256
      #36830#20987#29256
      #20256#22855#32493#31456
      #20256#22855#22806#20256
      #20256#22855#24402#26469)
    TabOrder = 2
    OnClick = RadioGroupClick
    object CheckBoxD3DFormat: TCheckBox
      Left = 192
      Top = 32
      Width = 73
      Height = 17
      Caption = #32441#29702#21387#32553
      TabOrder = 0
      OnClick = CheckBoxD3DFormatClick
    end
  end
end
