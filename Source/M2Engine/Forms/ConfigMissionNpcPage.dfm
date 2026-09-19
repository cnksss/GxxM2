object FrmMissionNpcPageEditDlg: TFrmMissionNpcPageEditDlg
  Left = 1238
  Top = 262
  BorderIcons = [biSystemMenu, biMinimize]
  BorderStyle = bsDialog
  Caption = #20219#21153'NPC'#39029#38754#32534#36753
  ClientHeight = 209
  ClientWidth = 317
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'MS Sans Serif'
  Font.Style = []
  Position = poMainFormCenter
  TextHeight = 13
  object GroupBox: TGroupBox
    Left = 8
    Top = 4
    Width = 177
    Height = 197
    Caption = #20219#21153'NPC'#39029#38754
    TabOrder = 0
    object ListBoxMissionPageCaptionList: TListBox
      Left = 8
      Top = 16
      Width = 161
      Height = 169
      ItemHeight = 13
      TabOrder = 0
      OnClick = ListBoxMissionPageCaptionListClick
    end
  end
  object ButtonMissionNpcAdd: TButton
    Left = 192
    Top = 98
    Width = 73
    Height = 25
    Caption = #22686#21152'(&A)'
    TabOrder = 1
    OnClick = ButtonMissionNpcAddClick
  end
  object ButtonEnablePickUpDelete: TButton
    Left = 192
    Top = 124
    Width = 73
    Height = 25
    Caption = #21024#38500'(&D)'
    Enabled = False
    TabOrder = 2
    OnClick = ButtonEnablePickUpDeleteClick
  end
  object ButtonEnablePickUpSave: TButton
    Left = 192
    Top = 150
    Width = 73
    Height = 25
    Caption = #20445#23384'(&S)'
    Enabled = False
    TabOrder = 3
    OnClick = ButtonEnablePickUpSaveClick
  end
  object EditMissionPage: TEdit
    Left = 192
    Top = 16
    Width = 121
    Height = 21
    TabOrder = 4
  end
  object ButtonSendMissionNpc: TButton
    Left = 192
    Top = 176
    Width = 129
    Height = 25
    Caption = #26356#26032#21040#23458#25143#31471'(&R)'
    TabOrder = 5
    OnClick = ButtonSendMissionNpcClick
  end
  object ButtonUp: TButton
    Left = 192
    Top = 42
    Width = 33
    Height = 25
    Caption = #8593
    TabOrder = 6
    OnClick = ButtonUpClick
  end
  object ButtonDown: TButton
    Left = 192
    Top = 68
    Width = 33
    Height = 25
    Caption = #8595
    TabOrder = 7
    OnClick = ButtonDownClick
  end
end
