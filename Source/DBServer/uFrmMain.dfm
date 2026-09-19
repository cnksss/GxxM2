object FrmMain: TFrmMain
  Left = 583
  Top = 317
  Caption = #25968#25454#24211#26381#21153#22120
  ClientHeight = 383
  ClientWidth = 549
  Color = clBtnFace
  Font.Charset = GB2312_CHARSET
  Font.Color = clWindowText
  Font.Height = -12
  Font.Name = #23435#20307
  Font.Style = []
  Menu = MainMenu
  OldCreateOrder = False
  OnCloseQuery = FormCloseQuery
  OnCreate = FormCreate
  OnDestroy = FormDestroy
  PixelsPerInch = 96
  TextHeight = 12
  object MemoLog: TMemo
    Left = 0
    Top = 0
    Width = 549
    Height = 153
    Align = alTop
    Color = clBlack
    Font.Charset = GB2312_CHARSET
    Font.Color = clLime
    Font.Height = -12
    Font.Name = #23435#20307
    Font.Style = []
    ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
    ParentFont = False
    ScrollBars = ssVertical
    TabOrder = 0
    OnChange = MemoLogChange
    OnDblClick = MemoLogDblClick
  end
  object Panel: TPanel
    Left = 0
    Top = 153
    Width = 549
    Height = 59
    Align = alTop
    ParentBackground = False
    TabOrder = 1
    object LabelLoadHumRcd: TLabel
      Left = 88
      Top = 8
      Width = 72
      Height = 12
      Caption = #35835#21462#20154#29289#25968#25454
    end
    object LabelSaveHumRcd: TLabel
      Left = 88
      Top = 32
      Width = 72
      Height = 12
      BiDiMode = bdLeftToRight
      Caption = #20445#23384#20154#29289#25968#25454
      ParentBiDiMode = False
    end
    object LabelLoadHeroRcd: TLabel
      Left = 272
      Top = 8
      Width = 72
      Height = 12
      Caption = #35835#21462#33521#38596#25968#25454
    end
    object LabelSaveHeroRcd: TLabel
      Left = 272
      Top = 32
      Width = 72
      Height = 12
      Caption = #20445#23384#33521#38596#25968#25454
    end
    object LabelCreateHero: TLabel
      Left = 192
      Top = 8
      Width = 48
      Height = 12
      Caption = #21019#24314#33521#38596
    end
    object LabelCreateHum: TLabel
      Left = 8
      Top = 8
      Width = 48
      Height = 12
      Caption = #21019#24314#20154#29289
    end
    object LabelDeleteHum: TLabel
      Left = 8
      Top = 32
      Width = 48
      Height = 12
      Caption = #21024#38500#20154#29289
    end
    object LabelDeleteHero: TLabel
      Left = 192
      Top = 32
      Width = 48
      Height = 12
      Caption = #21024#38500#28216#25103
    end
    object LabelWorkStatus: TLabel
      Left = 376
      Top = 32
      Width = 90
      Height = 12
      Caption = 'LabelWorkStatus'
      Font.Charset = GB2312_CHARSET
      Font.Color = clGreen
      Font.Height = -12
      Font.Name = #23435#20307
      Font.Style = []
      ParentFont = False
    end
    object LabelStatus: TLabel
      Left = 376
      Top = 8
      Width = 66
      Height = 12
      Caption = 'LabelStatus'
    end
  end
  object ModuleGrid: TStringGrid
    Left = 0
    Top = 212
    Width = 549
    Height = 171
    Align = alClient
    ColCount = 3
    DefaultColWidth = 150
    DefaultRowHeight = 14
    FixedCols = 0
    Options = [goFixedVertLine, goFixedHorzLine, goVertLine, goHorzLine, goRangeSelect, goRowSelect]
    TabOrder = 2
    ExplicitHeight = 151
    ColWidths = (
      168
      162
      150)
  end
  object TimerStart: TTimer
    Enabled = False
    OnTimer = TimerStartTimer
    Left = 48
    Top = 16
  end
  object TimerMain: TTimer
    Interval = 1
    OnTimer = TimerMainTimer
    Left = 88
    Top = 16
  end
  object MainMenu: TMainMenu
    Left = 168
    Top = 16
    object MENU_CONTROL: TMenuItem
      Caption = #25511#21046'(&C)'
      object MENU_CONTROL_START: TMenuItem
        Caption = #21551#21160#26381#21153'(&S)'
        OnClick = MENU_CONTROL_STARTClick
      end
      object MENU_CONTROL_STOP: TMenuItem
        Caption = #20572#27490#26381#21153'(&T)'
        OnClick = MENU_CONTROL_STOPClick
      end
      object N1: TMenuItem
        Caption = #37325#26032#21152#36733
        object G1: TMenuItem
          Caption = #32593#20851#35774#32622'(&G)'
          OnClick = G1Click
        end
        object C1: TMenuItem
          Caption = #35282#33394#36807#28388#21015#34920'(&C)'
          OnClick = C1Click
        end
      end
      object MENU_CONTROL_EXIT: TMenuItem
        Caption = #36864#20986'(&X)'
        OnClick = MENU_CONTROL_EXITClick
      end
    end
    object MENU_OPTION: TMenuItem
      Caption = #36873#39033'(&O)'
      object MENU_OPTION_GENERAL: TMenuItem
        Caption = #22522#26412#35774#32622'(&G)'
        OnClick = MENU_OPTION_GENERALClick
      end
      object MENU_OPTION_GAMEGATE: TMenuItem
        Caption = #32593#20851#35774#32622'(&R)'
        OnClick = MENU_OPTION_GAMEGATEClick
      end
      object N2: TMenuItem
        Caption = '-'
      end
      object MENU_OPTION_SHOWLOG: TMenuItem
        Caption = #26174#31034#26085#24535
        OnClick = MENU_OPTION_SHOWLOGClick
      end
      object mniShowQuryChrLog: TMenuItem
        Caption = #26174#31034#26597#35810#35282#33394#26085#24535
        OnClick = mniShowQuryChrLogClick
      end
    end
    object MENU_MANAGE: TMenuItem
      Caption = #31649#29702'(&M)'
      object MENU_MANAGE_DATA: TMenuItem
        Caption = #25968#25454#31649#29702'(&D)'
        OnClick = MENU_MANAGE_DATAClick
      end
      object MENU_OUTDATA: TMenuItem
        Caption = #25968#25454#23548#20986'(&O)'
        OnClick = MENU_OUTDATAClick
      end
      object MENU_RANKING: TMenuItem
        Caption = #25490#34892#27036#31649#29702'(&R)'
        OnClick = MENU_RANKINGClick
      end
    end
    object MENU_TEST: TMenuItem
      Caption = #27979#35797'(&T)'
      object MENU_TEST_SELGATE: TMenuItem
        Caption = #36873#25321#32593#20851'(&S)'
        OnClick = MENU_TEST_SELGATEClick
      end
    end
    object MENU_HELP: TMenuItem
      Caption = #24110#21161'(&H)'
      object MENU_HELP_VERSION: TMenuItem
        Caption = #20851#20110'(&A)'
        OnClick = MENU_HELP_VERSIONClick
      end
    end
  end
  object TimerClose: TTimer
    Enabled = False
    Interval = 500
    OnTimer = TimerCloseTimer
    Left = 16
    Top = 16
  end
end
