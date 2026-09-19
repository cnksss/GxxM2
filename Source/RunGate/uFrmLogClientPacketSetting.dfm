object FrmLogClientPacketSetting: TFrmLogClientPacketSetting
  Left = 403
  Top = 335
  BorderStyle = bsDialog
  Caption = #23553#21253#35760#24405#35774#32622
  ClientHeight = 238
  ClientWidth = 339
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
  object grpPacketType: TGroupBox
    Left = 8
    Top = 8
    Width = 87
    Height = 191
    Caption = #23553#21253#31867#22411
    TabOrder = 0
    object chkLogMove: TCheckBox
      Left = 8
      Top = 20
      Width = 74
      Height = 17
      Caption = #31227#21160#30456#20851
      TabOrder = 0
    end
    object chkLogSpell: TCheckBox
      Left = 8
      Top = 62
      Width = 74
      Height = 17
      Caption = #39764#27861#30456#20851
      TabOrder = 1
    end
    object chkLogQuery: TCheckBox
      Left = 8
      Top = 83
      Width = 74
      Height = 17
      Caption = #26597#35810#30456#20851
      TabOrder = 2
    end
    object chkLogTeam: TCheckBox
      Left = 8
      Top = 104
      Width = 74
      Height = 17
      Caption = #32452#38431#30456#20851
      TabOrder = 3
    end
    object chkLogHit: TCheckBox
      Left = 8
      Top = 41
      Width = 74
      Height = 17
      Caption = #25915#20987#30456#20851
      TabOrder = 4
    end
    object chkLogGuild: TCheckBox
      Left = 8
      Top = 125
      Width = 74
      Height = 17
      Caption = #34892#20250#30456#20851
      TabOrder = 5
    end
    object chkLogShop: TCheckBox
      Left = 8
      Top = 146
      Width = 74
      Height = 17
      Caption = #21830#38138#25670#25674
      TabOrder = 6
    end
    object chkLogOther: TCheckBox
      Left = 8
      Top = 167
      Width = 74
      Height = 17
      Caption = #20854#20182#25805#20316
      Checked = True
      Enabled = False
      State = cbChecked
      TabOrder = 7
    end
  end
  object grpLogUser: TGroupBox
    Left = 101
    Top = 8
    Width = 231
    Height = 192
    Caption = #35760#24405#25351#23450#20154#29289#65288#20026#31354#34920#31034#35760#24405#25152#26377#65289
    TabOrder = 1
    object lbl1: TLabel
      Left = 7
      Top = 168
      Width = 24
      Height = 12
      Caption = #20154#29289
    end
    object lstLogUser: TListBox
      Left = 8
      Top = 20
      Width = 216
      Height = 141
      ItemHeight = 12
      TabOrder = 0
      OnClick = lstLogUserClick
    end
    object edtUserName: TEdit
      Left = 35
      Top = 165
      Width = 100
      Height = 20
      TabOrder = 1
    end
    object btnAddUser: TButton
      Left = 138
      Top = 164
      Width = 42
      Height = 22
      Caption = #28155#21152
      TabOrder = 2
      OnClick = btnAddUserClick
    end
    object btnDelUser: TButton
      Left = 183
      Top = 164
      Width = 42
      Height = 22
      Caption = #21024#38500
      TabOrder = 3
      OnClick = btnDelUserClick
    end
  end
  object btnOK: TButton
    Left = 176
    Top = 206
    Width = 75
    Height = 25
    Caption = #30830#23450
    TabOrder = 2
    OnClick = btnOKClick
  end
  object btnCancel: TButton
    Left = 256
    Top = 206
    Width = 75
    Height = 25
    Caption = #21462#28040
    ModalResult = 2
    TabOrder = 3
  end
  object chkLogClientPacket: TCheckBox
    Left = 8
    Top = 210
    Width = 105
    Height = 17
    Caption = #24320#21551#23553#21253#35760#24405
    Font.Charset = GB2312_CHARSET
    Font.Color = clWindowText
    Font.Height = -12
    Font.Name = #23435#20307
    Font.Style = [fsBold]
    ParentFont = False
    TabOrder = 4
  end
end
