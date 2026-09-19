object FrmCombatPowerAddVar: TFrmCombatPowerAddVar
  Left = 547
  Top = 572
  BorderStyle = bsDialog
  Caption = #28155#21152#21464#37327
  ClientHeight = 154
  ClientWidth = 293
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'Tahoma'
  Font.Style = []
  OldCreateOrder = False
  Position = poMainFormCenter
  PixelsPerInch = 96
  TextHeight = 14
  object grp1: TGroupBox
    Left = 9
    Top = 8
    Width = 276
    Height = 104
    Caption = #21464#37327#35774#32622
    TabOrder = 0
    object lbl1: TLabel
      Left = 24
      Top = 24
      Width = 48
      Height = 13
      Caption = #21464#37327#21517#65306
    end
    object lbl2: TLabel
      Left = 12
      Top = 49
      Width = 60
      Height = 13
      Caption = #19979#26631#24320#22987#65306
    end
    object Label1: TLabel
      Left = 12
      Top = 76
      Width = 60
      Height = 13
      Caption = #21464#37327#25968#37327#65306
    end
    object cbbVarName: TComboBox
      Left = 72
      Top = 19
      Width = 108
      Height = 22
      ItemHeight = 14
      TabOrder = 0
      Items.Strings = (
        'D'
        'M'
        'N'
        'U'
        'J'
        'N$')
    end
    object seVarIndex: TSpinEditEx
      Left = 72
      Top = 45
      Width = 108
      Height = 22
      MaxValue = 0
      MinValue = 0
      TabOrder = 1
      Value = 0
    end
    object seVarCount: TSpinEditEx
      Left = 72
      Top = 72
      Width = 108
      Height = 22
      MaxValue = 600
      MinValue = 1
      TabOrder = 2
      Value = 1
    end
    object chkBatch: TCheckBox
      Left = 196
      Top = 22
      Width = 69
      Height = 17
      Caption = #25209#37327#28155#21152
      Checked = True
      State = cbChecked
      TabOrder = 3
      OnClick = chkBatchClick
    end
  end
  object btnOK: TButton
    Left = 210
    Top = 120
    Width = 75
    Height = 25
    Caption = #30830#23450
    TabOrder = 1
    OnClick = btnOKClick
  end
end
