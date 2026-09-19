object frmViewOnlineHuman: TfrmViewOnlineHuman
  Left = 367
  Top = 233
  Caption = #22312#32447#20154#29289
  ClientHeight = 457
  ClientWidth = 964
  Color = clBtnFace
  Font.Charset = GB2312_CHARSET
  Font.Color = clWindowText
  Font.Height = -12
  Font.Name = #23435#20307
  Font.Style = []
  Position = poMainFormCenter
  OnCreate = FormCreate
  OnDestroy = FormDestroy
  TextHeight = 12
  object PanelStatus: TPanel
    Left = 0
    Top = 0
    Width = 964
    Height = 410
    Align = alClient
    Caption = #27491#22312#35835#21462#25968#25454'...'
    TabOrder = 0
    ExplicitWidth = 960
    ExplicitHeight = 409
    object GridHuman: TStringGrid
      Left = 1
      Top = 1
      Width = 962
      Height = 408
      Align = alClient
      ColCount = 19
      DefaultRowHeight = 18
      FixedCols = 0
      RowCount = 25
      Options = [goFixedVertLine, goFixedHorzLine, goVertLine, goHorzLine, goRangeSelect, goRowSelect]
      TabOrder = 0
      OnDblClick = GridHumanDblClick
      ExplicitWidth = 958
      ExplicitHeight = 407
      ColWidths = (
        33
        78
        52
        31
        44
        39
        37
        47
        74
        89
        32
        138
        50
        50
        50
        50
        50
        60
        285)
      RowHeights = (
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18)
    end
  end
  object Panel1: TPanel
    Left = 0
    Top = 410
    Width = 964
    Height = 47
    Align = alBottom
    BevelOuter = bvNone
    TabOrder = 1
    ExplicitTop = 409
    ExplicitWidth = 960
    object Label1: TLabel
      Left = 211
      Top = 17
      Width = 30
      Height = 12
      Caption = #25490#24207':'
    end
    object lbl1: TLabel
      Left = 8
      Top = 17
      Width = 60
      Height = 12
      Caption = #21047#26032#38388#38548#65306
    end
    object ButtonRefGrid: TButton
      Left = 133
      Top = 12
      Width = 52
      Height = 22
      Caption = #21047#26032'(&R)'
      TabOrder = 0
      OnClick = ButtonRefGridClick
    end
    object ComboBoxSort: TComboBox
      Left = 244
      Top = 13
      Width = 97
      Height = 20
      Style = csDropDownList
      ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
      TabOrder = 1
      OnClick = ComboBoxSortClick
      Items.Strings = (
        #21517#31216
        #35282#33394#31867#22411
        #24615#21035
        #32844#19994
        #31561#32423
        #22320#22270
        #65321#65328
        #26435#38480
        #25152#22312#22320#21306)
    end
    object EditSearchName: TEdit
      Left = 348
      Top = 13
      Width = 113
      Height = 20
      ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
      TabOrder = 2
    end
    object ButtonSearch: TButton
      Left = 466
      Top = 11
      Width = 59
      Height = 25
      Caption = #25628#32034'(&S)'
      TabOrder = 3
      OnClick = ButtonSearchClick
    end
    object ButtonView: TButton
      Left = 529
      Top = 11
      Width = 81
      Height = 25
      Caption = #20154#29289#20449#24687'(&H)'
      TabOrder = 4
      OnClick = ButtonViewClick
    end
    object ButtonKickPlayOffLine: TButton
      Left = 617
      Top = 11
      Width = 88
      Height = 25
      Caption = #36386#31163#32447#25346#26426'(&K)'
      TabOrder = 5
      OnClick = ButtonKickPlayOffLineClick
    end
    object chkDummyHero: TCheckBox
      Left = 865
      Top = 6
      Width = 96
      Height = 17
      Caption = #26174#31034#20551#20154#33521#38596
      Checked = True
      State = cbChecked
      TabOrder = 6
      OnClick = chkDummyClick
    end
    object chkDummy: TCheckBox
      Left = 787
      Top = 6
      Width = 73
      Height = 17
      Caption = #26174#31034#20551#20154
      Checked = True
      State = cbChecked
      TabOrder = 7
      OnClick = chkDummyClick
    end
    object ButtonKickDummyObject: TButton
      Left = 711
      Top = 11
      Width = 69
      Height = 25
      Caption = #36386#20551#20154'(&K)'
      TabOrder = 8
      OnClick = ButtonKickDummyObjectClick
    end
    object chkHuman: TCheckBox
      Left = 787
      Top = 24
      Width = 81
      Height = 17
      Caption = #26174#31034#29609#23478
      Checked = True
      State = cbChecked
      TabOrder = 9
      OnClick = chkDummyClick
    end
    object chkHumanHero: TCheckBox
      Left = 865
      Top = 22
      Width = 96
      Height = 17
      Caption = #26174#31034#29609#23478#33521#38596
      Checked = True
      State = cbChecked
      TabOrder = 10
      OnClick = chkDummyClick
    end
    object cbbRefreshTime: TComboBox
      Left = 64
      Top = 13
      Width = 57
      Height = 20
      Style = csDropDownList
      ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
      TabOrder = 11
      OnChange = cbbRefreshTimeChange
      Items.Strings = (
        #19981#33258#21160#21047#26032
        '30'#31186
        '60'#31186
        '90'#31186)
    end
  end
  object Timer: TTimer
    Enabled = False
    OnTimer = TimerTimer
    Left = 16
    Top = 16
  end
  object tmrRefresh: TTimer
    Enabled = False
    OnTimer = tmrRefreshTimer
    Left = 48
    Top = 16
  end
end
