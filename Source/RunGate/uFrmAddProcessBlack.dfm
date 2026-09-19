object FrmAddProcessBlack: TFrmAddProcessBlack
  Left = 271
  Top = 330
  BorderStyle = bsDialog
  Caption = #28155#21152#36827#31243#40657#21517#21333
  ClientHeight = 122
  ClientWidth = 365
  Color = clBtnFace
  Font.Charset = GB2312_CHARSET
  Font.Color = clWindowText
  Font.Height = -13
  Font.Name = #23435#20307
  Font.Style = []
  OldCreateOrder = False
  Position = poMainFormCenter
  PixelsPerInch = 96
  TextHeight = 13
  object grp1: TGroupBox
    Left = 6
    Top = 6
    Width = 353
    Height = 75
    Caption = #36827#31243#20449#24687
    TabOrder = 0
    object lbl1: TLabel
      Left = 6
      Top = 22
      Width = 52
      Height = 13
      Caption = #36827#31243#21517#65306
    end
    object Label1: TLabel
      Left = 11
      Top = 48
      Width = 47
      Height = 13
      Caption = 'MD5'#20540#65306
    end
    object edtProcessName: TEdit
      Left = 54
      Top = 18
      Width = 291
      Height = 21
      TabOrder = 0
    end
    object edtProcessMD5: TEdit
      Left = 54
      Top = 44
      Width = 291
      Height = 21
      MaxLength = 32
      TabOrder = 1
    end
  end
  object btnOK: TButton
    Left = 200
    Top = 88
    Width = 75
    Height = 25
    Caption = #30830#23450
    TabOrder = 1
    OnClick = btnOKClick
  end
  object btnCancel: TButton
    Left = 284
    Top = 88
    Width = 75
    Height = 25
    Caption = #21462#28040
    ModalResult = 2
    TabOrder = 2
  end
end
