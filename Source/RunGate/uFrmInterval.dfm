object FrmInterval: TFrmInterval
  Left = 192
  Top = 130
  BorderStyle = bsDialog
  BorderWidth = 5
  Caption = #25915#20987#38388#38548#35774#32622
  ClientHeight = 576
  ClientWidth = 238
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'Tahoma'
  Font.Style = []
  OldCreateOrder = False
  Position = poMainFormCenter
  OnShow = FormShow
  PixelsPerInch = 96
  TextHeight = 13
  object grpSetting: TGroupBox
    Left = 0
    Top = 0
    Width = 238
    Height = 484
    Align = alClient
    Caption = #38388#38548#35774#32622
    TabOrder = 0
    DesignSize = (
      238
      484)
    object btnZero: TSpeedButton
      Left = 220
      Top = 241
      Width = 14
      Height = 21
      Caption = '0'
      OnClick = btnZeroClick
    end
    object grdInterval: TStringGrid
      Left = 7
      Top = 20
      Width = 210
      Height = 458
      Anchors = [akLeft, akTop, akBottom]
      ColCount = 2
      DefaultColWidth = 100
      DefaultRowHeight = 18
      RowCount = 100
      Options = [goFixedVertLine, goFixedHorzLine, goVertLine, goHorzLine, goEditing, goThumbTracking]
      TabOrder = 0
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
        18
        18
        18
        18)
    end
  end
  object pnlBottom: TPanel
    Left = 0
    Top = 484
    Width = 238
    Height = 92
    Align = alBottom
    BevelOuter = bvNone
    TabOrder = 1
    object btnOK: TButton
      Left = 162
      Top = 67
      Width = 75
      Height = 25
      Caption = #30830#23450
      TabOrder = 0
      OnClick = btnOKClick
    end
    object grpBatch: TGroupBox
      Left = 0
      Top = 0
      Width = 238
      Height = 65
      Caption = ' '#25209#37327#35774#32622#38388#38548
      TabOrder = 1
      object lblSpeed0: TLabel
        Left = 8
        Top = 16
        Width = 122
        Height = 13
        Caption = #24403#36895#24230'=0'#26102#26102#38388#38388#38548#65306
      end
      object lblIncSpeedDecTime: TLabel
        Left = 8
        Top = 40
        Width = 122
        Height = 13
        Caption = #36895#24230#27599'+1'#26102#38388#38548#20943#23569#65306
      end
      object btnAll: TButton
        Left = 190
        Top = 36
        Width = 41
        Height = 22
        Caption = #35774#32622
        TabOrder = 0
        OnClick = btnAllClick
      end
      object seSpeed0: TSpinEditEx
        Left = 128
        Top = 12
        Width = 104
        Height = 22
        MaxValue = 0
        MinValue = 0
        TabOrder = 1
        Value = 0
      end
      object seIncSpeedDecTime: TSpinEditEx
        Left = 128
        Top = 36
        Width = 60
        Height = 22
        MaxValue = 0
        MinValue = 0
        TabOrder = 2
        Value = 0
      end
    end
    object chkSendSpeedIntervalsToClient: TCheckBox
      Left = 0
      Top = 71
      Width = 153
      Height = 17
      Hint = #21246#36873#21518#65292#38388#38548#20250#21457#24448#23458#25143#31471#65292#23458#25143#31471#20250#25511#21046#32452#21512#38388#38548#8805#35774#32622#30340#38388#38548
      Caption = #21516#27493#38388#38548#35774#32622#21040#23458#25143#31471
      ParentShowHint = False
      ShowHint = True
      TabOrder = 2
    end
  end
end
