object FrmCustomMagicCopySetting: TFrmCustomMagicCopySetting
  Left = 450
  Top = 256
  BorderStyle = bsDialog
  Caption = #22797#21046#37197#32622
  ClientHeight = 122
  ClientWidth = 237
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
  object Label1: TLabel
    Left = 16
    Top = 32
    Width = 60
    Height = 13
    Caption = #24403#21069#37197#32622#65306
  end
  object grp1: TGroupBox
    Left = 8
    Top = 8
    Width = 221
    Height = 75
    Caption = #36873#39033
    TabOrder = 0
    object lbl1: TLabel
      Left = 8
      Top = 24
      Width = 72
      Height = 13
      Caption = #22797#21046#28304#37197#32622#65306
    end
    object lbl2: TLabel
      Left = 8
      Top = 48
      Width = 72
      Height = 13
      Caption = #22797#21046#37197#32622#21040#65306
    end
    object edtSource: TEdit
      Left = 78
      Top = 20
      Width = 132
      Height = 21
      ReadOnly = True
      TabOrder = 0
    end
    object cbbDest: TComboBox
      Left = 78
      Top = 44
      Width = 133
      Height = 21
      Style = csDropDownList
      ItemHeight = 13
      TabOrder = 1
    end
  end
  object btnOK: TButton
    Left = 64
    Top = 88
    Width = 75
    Height = 25
    Caption = #30830#23450
    Default = True
    TabOrder = 1
    OnClick = btnOKClick
  end
  object btnCancel: TButton
    Left = 152
    Top = 88
    Width = 75
    Height = 25
    Cancel = True
    Caption = #21462#28040
    ModalResult = 2
    TabOrder = 2
  end
end
