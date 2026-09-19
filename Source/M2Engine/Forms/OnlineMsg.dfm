object frmOnlineMsg: TfrmOnlineMsg
  Left = 551
  Top = 342
  BorderIcons = [biSystemMenu]
  BorderStyle = bsSingle
  Caption = #22312#32447#21457#36865#28040#24687
  ClientHeight = 436
  ClientWidth = 575
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -12
  Font.Name = #23435#20307
  Font.Style = []
  Position = poMainFormCenter
  OnCreate = FormCreate
  DesignSize = (
    575
    436)
  TextHeight = 12
  object lbl4: TLabel
    Left = 8
    Top = 413
    Width = 348
    Height = 12
    Caption = #28857#20987#21457#36865#25805#20316#38480#21046#21551#29992#65292#20851#38381#26412#31383#21475#21518#20197#19978#25805#20316#38480#21046#23558#21462#28040#65281#65281#65281
    Font.Charset = DEFAULT_CHARSET
    Font.Color = clBlue
    Font.Height = -12
    Font.Name = #23435#20307
    Font.Style = []
    ParentFont = False
  end
  object btnSend: TButton
    Left = 501
    Top = 406
    Width = 75
    Height = 25
    Caption = #21457#36865'(&S)'
    TabOrder = 0
    OnClick = btnSendClick
  end
  object pgc1: TPageControl
    Left = 9
    Top = 8
    Width = 541
    Height = 242
    ActivePage = ts1
    Anchors = [akLeft, akTop, akRight]
    TabOrder = 1
    object ts1: TTabSheet
      Caption = #28040#24687#27169#29256
      DesignSize = (
        533
        214)
      object lbl1: TLabel
        Left = 4
        Top = 194
        Width = 132
        Height = 12
        Anchors = [akLeft, akBottom]
        Caption = #21452#20987#22797#21046#25991#26412#21040#21457#36865#26694#20013
      end
      object StringGrid: TStringGrid
        Left = 4
        Top = 5
        Width = 524
        Height = 178
        Anchors = [akLeft, akTop, akRight, akBottom]
        ColCount = 1
        DefaultColWidth = 520
        DefaultRowHeight = 18
        FixedCols = 0
        RowCount = 1
        FixedRows = 0
        ScrollBars = ssVertical
        TabOrder = 0
        OnClick = StringGridClick
        OnDblClick = StringGridDblClick
        ExplicitWidth = 536
      end
      object ButtonAdd: TButton
        Left = 390
        Top = 187
        Width = 67
        Height = 25
        Anchors = [akRight, akBottom]
        Caption = #22686#21152'(&A)'
        Enabled = False
        TabOrder = 1
        OnClick = ButtonAddClick
        ExplicitLeft = 402
      end
      object ButtonDelete: TButton
        Left = 461
        Top = 187
        Width = 67
        Height = 25
        Anchors = [akRight, akBottom]
        Caption = #21024#38500'(&D)'
        Enabled = False
        TabOrder = 2
        OnClick = ButtonDeleteClick
        ExplicitLeft = 473
      end
    end
    object ts2: TTabSheet
      Caption = #21457#36865#35760#24405
      ImageIndex = 1
      DesignSize = (
        533
        214)
      object MemoMsg: TMemo
        Left = 4
        Top = 5
        Width = 524
        Height = 205
        Anchors = [akLeft, akTop, akRight, akBottom]
        TabOrder = 0
        OnChange = MemoMsgChange
        ExplicitWidth = 544
      end
    end
  end
  object grp1: TGroupBox
    Left = 8
    Top = 259
    Width = 568
    Height = 63
    Caption = #25805#20316#38480#21046
    TabOrder = 2
    object chkDisableTrading: TCheckBox
      Left = 9
      Top = 18
      Width = 80
      Height = 17
      Caption = #31105#27490#20132#26131
      Checked = True
      State = cbChecked
      TabOrder = 0
    end
    object chkDisableRepair: TCheckBox
      Left = 9
      Top = 39
      Width = 80
      Height = 17
      Caption = #31105#27490#20462#29702
      Checked = True
      State = cbChecked
      TabOrder = 1
    end
    object chkDisableSaveToStorage: TCheckBox
      Left = 241
      Top = 18
      Width = 80
      Height = 17
      Caption = #31105#27490#23384#20179#24211
      Checked = True
      State = cbChecked
      TabOrder = 2
    end
    object chkDisableGetFromStorage: TCheckBox
      Left = 241
      Top = 39
      Width = 80
      Height = 17
      Caption = #31105#27490#21462#20179#24211
      Checked = True
      State = cbChecked
      TabOrder = 3
    end
    object chkDisableBuy: TCheckBox
      Left = 125
      Top = 18
      Width = 80
      Height = 17
      Caption = #31105#27490#20080#29289#21697
      Checked = True
      State = cbChecked
      TabOrder = 4
    end
    object chkDisableSell: TCheckBox
      Left = 125
      Top = 39
      Width = 80
      Height = 17
      Caption = #31105#27490#21334#29289#21697
      Checked = True
      State = cbChecked
      TabOrder = 5
    end
    object chkDisableDropItem: TCheckBox
      Left = 357
      Top = 18
      Width = 80
      Height = 17
      Caption = #31105#27490#25172#29289#21697
      Checked = True
      State = cbChecked
      TabOrder = 6
    end
    object chkDisableUseNpc: TCheckBox
      Left = 357
      Top = 39
      Width = 87
      Height = 17
      Caption = #31105#27490#20351#29992'npc'
      Checked = True
      State = cbChecked
      TabOrder = 7
    end
    object chkDisableChallenge: TCheckBox
      Left = 473
      Top = 18
      Width = 80
      Height = 17
      Caption = #31105#27490#25361#25112
      Checked = True
      State = cbChecked
      TabOrder = 8
    end
    object chkDisableShop: TCheckBox
      Left = 473
      Top = 39
      Width = 80
      Height = 17
      Caption = #31105#27490#24215#38138
      Checked = True
      State = cbChecked
      TabOrder = 9
    end
  end
  object grp2: TGroupBox
    Left = 8
    Top = 328
    Width = 568
    Height = 73
    Caption = #28040#24687#25511#21046
    TabOrder = 3
    object lbl2: TLabel
      Left = 200
      Top = 22
      Width = 60
      Height = 12
      Caption = #21457#36865#38388#38548#65306
    end
    object Label2: TLabel
      Left = 330
      Top = 22
      Width = 12
      Height = 12
      Caption = #31186
    end
    object Label1: TLabel
      Left = 9
      Top = 49
      Width = 60
      Height = 12
      Caption = #21457#36865#28040#24687#65306
    end
    object lbl3: TLabel
      Left = 403
      Top = 22
      Width = 156
      Height = 12
      Caption = #33258#21160#21457#36865#22312#26412#31383#20307#20851#38381#21518#20572#27490
      Font.Charset = DEFAULT_CHARSET
      Font.Color = clRed
      Font.Height = -12
      Font.Name = #23435#20307
      Font.Style = []
      ParentFont = False
    end
    object chkAutoRun: TCheckBox
      Left = 9
      Top = 20
      Width = 121
      Height = 17
      Caption = #24320#21551#33258#21160#25345#32493#21457#36865
      Checked = True
      State = cbChecked
      TabOrder = 0
    end
    object seAutRunInterval: TSpinEditEx
      Left = 256
      Top = 18
      Width = 73
      Height = 21
      MaxValue = 999999
      MinValue = 1
      TabOrder = 1
      Value = 3
    end
    object ComboBoxMsg: TComboBox
      Left = 67
      Top = 45
      Width = 492
      Height = 20
      Style = csSimple
      TabOrder = 2
      OnChange = ComboBoxMsgChange
      OnKeyPress = ComboBoxMsgKeyPress
    end
  end
  object tmrRun: TTimer
    Enabled = False
    OnTimer = tmrRunTimer
    Left = 136
    Top = 352
  end
end
