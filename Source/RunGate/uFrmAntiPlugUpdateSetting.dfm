object FrmAntiPlugUpdateSetting: TFrmAntiPlugUpdateSetting
  Left = 384
  Top = 383
  BorderStyle = bsDialog
  Caption = #32593#20851#25554#20214#33258#21160#26356#26032#35774#32622
  ClientHeight = 145
  ClientWidth = 432
  Color = clBtnFace
  Font.Charset = GB2312_CHARSET
  Font.Color = clWindowText
  Font.Height = -12
  Font.Name = #23435#20307
  Font.Style = []
  OldCreateOrder = False
  Position = poMainFormCenter
  OnCreate = FormCreate
  PixelsPerInch = 96
  TextHeight = 12
  object grpMain: TGroupBox
    Left = 8
    Top = 8
    Width = 417
    Height = 99
    Caption = #33258#21160#26356#26032#35774#32622
    TabOrder = 0
    object lbl1: TLabel
      Left = 19
      Top = 74
      Width = 108
      Height = 12
      Caption = #26356#26032#37197#32622#25991#20214#22320#22336#65306
    end
    object lbl2: TLabel
      Left = 19
      Top = 48
      Width = 108
      Height = 12
      Caption = #33258#21160#26356#26032#26816#27979#38388#38548#65306
    end
    object Label1: TLabel
      Left = 195
      Top = 48
      Width = 24
      Height = 12
      Caption = #20998#38047
    end
    object chkAntiPlugAutoUpdateCheck: TCheckBox
      Left = 8
      Top = 22
      Width = 129
      Height = 17
      Caption = #33258#21160#26816#27979#25554#20214#26356#26032
      TabOrder = 0
    end
    object edtAntiPlugUpdateConfigUrl: TEdit
      Left = 125
      Top = 70
      Width = 285
      Height = 20
      TabOrder = 2
    end
    object seAntiPlugUpdateCheckInterval: TSpinEditEx
      Left = 125
      Top = 43
      Width = 68
      Height = 21
      MaxValue = 120
      MinValue = 1
      TabOrder = 1
      Value = 1
    end
  end
  object btnOK: TButton
    Left = 350
    Top = 112
    Width = 75
    Height = 25
    Caption = #30830#23450
    TabOrder = 1
    OnClick = btnOKClick
  end
end
