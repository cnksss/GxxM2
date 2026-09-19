object Form1: TForm1
  Left = 288
  Top = 166
  BorderStyle = bsSingle
  Caption = 'simple Shoutcast Radio'
  ClientHeight = 255
  ClientWidth = 360
  Color = clBtnFace
  UseDockManager = True
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'MS Sans Serif'
  Font.Style = []
  OldCreateOrder = False
  Position = poScreenCenter
  OnClose = FormClose
  OnCreate = FormCreate
  PixelsPerInch = 96
  TextHeight = 13
  object Label1: TLabel
    Left = 8
    Top = 16
    Width = 25
    Height = 13
    Caption = 'URL:'
  end
  object ComboBox1: TComboBox
    Left = 40
    Top = 16
    Width = 225
    Height = 21
    ItemHeight = 13
    TabOrder = 0
    Text = 'http://64.236.34.67:80/stream/1039'
    Items.Strings = (
      'http://64.236.34.67:80/stream/1039'
      'http://64.236.34.4:80/stream/1040'
      'http://62.4.21.34:8000'
      'http://64.62.197.5:8000'
      'http://62.179.101.66:7128'
      'http://64.236.34.97:80/stream/1025'
      'http://64.236.34.196:80/stream/2005')
  end
  object Button6: TButton
    Left = 280
    Top = 16
    Width = 65
    Height = 25
    Caption = 'connect'
    TabOrder = 1
    OnClick = Button6Click
  end
  object GroupBox5: TGroupBox
    Left = 8
    Top = 40
    Width = 233
    Height = 65
    Caption = 'Buffering'
    TabOrder = 2
    object Label7: TLabel
      Left = 8
      Top = 24
      Width = 46
      Height = 13
      Caption = 'Prebuffer:'
      Font.Charset = DEFAULT_CHARSET
      Font.Color = clWindowText
      Font.Height = -11
      Font.Name = 'MS Sans Serif'
      Font.Style = []
      ParentFont = False
    end
    object Label2: TLabel
      Left = 8
      Top = 44
      Width = 31
      Height = 13
      Caption = 'Buffer:'
      Font.Charset = DEFAULT_CHARSET
      Font.Color = clWindowText
      Font.Height = -11
      Font.Name = 'MS Sans Serif'
      Font.Style = []
      ParentFont = False
    end
    object Label8: TLabel
      Left = 181
      Top = 24
      Width = 20
      Height = 13
      Caption = 'N/A'
      Font.Charset = ANSI_CHARSET
      Font.Color = clWindowText
      Font.Height = -11
      Font.Name = 'MS Sans Serif'
      Font.Style = []
      ParentFont = False
    end
    object Label3: TLabel
      Left = 181
      Top = 44
      Width = 20
      Height = 13
      Caption = 'N/A'
      Font.Charset = ANSI_CHARSET
      Font.Color = clWindowText
      Font.Height = -11
      Font.Name = 'MS Sans Serif'
      Font.Style = []
      ParentFont = False
    end
    object TrackBar1: TTrackBar
      Left = 56
      Top = 21
      Width = 121
      Height = 20
      LineSize = 100
      Max = 1000
      Orientation = trHorizontal
      PageSize = 100
      Frequency = 10
      Position = 300
      SelEnd = 0
      SelStart = 0
      TabOrder = 0
      ThumbLength = 12
      TickMarks = tmBoth
      TickStyle = tsNone
      OnChange = TrackBar1Change
    end
    object TrackBar2: TTrackBar
      Left = 56
      Top = 40
      Width = 121
      Height = 17
      LineSize = 100
      Max = 1000
      Orientation = trHorizontal
      Frequency = 10
      Position = 150
      SelEnd = 0
      SelStart = 0
      TabOrder = 1
      ThumbLength = 12
      TickMarks = tmBoth
      TickStyle = tsNone
      OnChange = TrackBar2Change
    end
  end
  object GroupBox4: TGroupBox
    Left = 248
    Top = 48
    Width = 105
    Height = 79
    Caption = 'MetaData'
    TabOrder = 3
    object RadioButton3: TRadioButton
      Left = 8
      Top = 24
      Width = 57
      Height = 25
      Caption = 'parse'
      Checked = True
      Font.Charset = DEFAULT_CHARSET
      Font.Color = clWindowText
      Font.Height = -11
      Font.Name = 'MS Sans Serif'
      Font.Style = []
      ParentFont = False
      TabOrder = 0
      TabStop = True
    end
    object RadioButton4: TRadioButton
      Left = 8
      Top = 48
      Width = 89
      Height = 17
      Caption = 'don'#39't parse'
      Font.Charset = DEFAULT_CHARSET
      Font.Color = clWindowText
      Font.Height = -11
      Font.Name = 'MS Sans Serif'
      Font.Style = []
      ParentFont = False
      TabOrder = 1
    end
  end
  object GroupBox2: TGroupBox
    Left = 8
    Top = 128
    Width = 345
    Height = 65
    Caption = 'Server info'
    Font.Charset = ANSI_CHARSET
    Font.Color = clWindowText
    Font.Height = -11
    Font.Name = 'MS Sans Serif'
    Font.Style = []
    ParentFont = False
    TabOrder = 4
    object Label20: TLabel
      Left = 8
      Top = 16
      Width = 31
      Height = 13
      Caption = 'Name:'
      Font.Charset = ANSI_CHARSET
      Font.Color = clWindowText
      Font.Height = -11
      Font.Name = 'MS Sans Serif'
      Font.Style = []
      ParentFont = False
    end
    object Label21: TLabel
      Left = 8
      Top = 32
      Width = 32
      Height = 13
      Caption = 'Genre:'
      Font.Charset = ANSI_CHARSET
      Font.Color = clWindowText
      Font.Height = -11
      Font.Name = 'MS Sans Serif'
      Font.Style = []
      ParentFont = False
    end
    object Label22: TLabel
      Left = 8
      Top = 48
      Width = 22
      Height = 13
      Caption = 'URL'
      Font.Charset = ANSI_CHARSET
      Font.Color = clWindowText
      Font.Height = -11
      Font.Name = 'MS Sans Serif'
      Font.Style = []
      ParentFont = False
    end
    object Label23: TLabel
      Left = 272
      Top = 48
      Width = 33
      Height = 13
      Caption = 'Bitrate:'
      Font.Charset = ANSI_CHARSET
      Font.Color = clWindowText
      Font.Height = -11
      Font.Name = 'MS Sans Serif'
      Font.Style = []
      ParentFont = False
    end
    object Label24: TLabel
      Left = 56
      Top = 16
      Width = 24
      Height = 13
      Cursor = crHandPoint
      Caption = 'N/A'
      Font.Charset = ANSI_CHARSET
      Font.Color = clWindowText
      Font.Height = -11
      Font.Name = 'MS Sans Serif'
      Font.Style = [fsBold]
      ParentFont = False
    end
    object Label25: TLabel
      Left = 56
      Top = 32
      Width = 20
      Height = 13
      Cursor = crHandPoint
      Caption = 'N/A'
      Font.Charset = ANSI_CHARSET
      Font.Color = clWindowText
      Font.Height = -11
      Font.Name = 'MS Sans Serif'
      Font.Style = []
      ParentFont = False
    end
    object Label26: TLabel
      Left = 56
      Top = 48
      Width = 20
      Height = 13
      Cursor = crHandPoint
      Caption = 'N/A'
      Font.Charset = ANSI_CHARSET
      Font.Color = clWindowText
      Font.Height = -11
      Font.Name = 'MS Sans Serif'
      Font.Style = []
      ParentFont = False
      OnClick = Label26Click
    end
    object Label27: TLabel
      Left = 312
      Top = 48
      Width = 20
      Height = 13
      Caption = 'N/A'
      Font.Charset = ANSI_CHARSET
      Font.Color = clWindowText
      Font.Height = -11
      Font.Name = 'MS Sans Serif'
      Font.Style = []
      ParentFont = False
    end
  end
  object GroupBox3: TGroupBox
    Left = 8
    Top = 195
    Width = 345
    Height = 53
    Caption = 'Stream info'
    TabOrder = 5
    object Label16: TLabel
      Left = 10
      Top = 18
      Width = 23
      Height = 13
      Caption = 'Title:'
      Font.Charset = DEFAULT_CHARSET
      Font.Color = clWindowText
      Font.Height = -11
      Font.Name = 'MS Sans Serif'
      Font.Style = []
      ParentFont = False
    end
    object Label17: TLabel
      Left = 10
      Top = 35
      Width = 25
      Height = 13
      Caption = 'URL:'
      Font.Charset = DEFAULT_CHARSET
      Font.Color = clWindowText
      Font.Height = -11
      Font.Name = 'MS Sans Serif'
      Font.Style = []
      ParentFont = False
    end
    object Label18: TLabel
      Left = 42
      Top = 18
      Width = 24
      Height = 13
      Cursor = crHandPoint
      Caption = 'N/A'
      Font.Charset = ANSI_CHARSET
      Font.Color = clWindowText
      Font.Height = -11
      Font.Name = 'MS Sans Serif'
      Font.Style = [fsBold]
      ParentFont = False
    end
    object Label19: TLabel
      Left = 42
      Top = 35
      Width = 20
      Height = 13
      Cursor = crHandPoint
      Caption = 'N/A'
      Font.Charset = ANSI_CHARSET
      Font.Color = clWindowText
      Font.Height = -11
      Font.Name = 'MS Sans Serif'
      Font.Style = []
      ParentFont = False
      OnClick = Label19Click
    end
  end
  object GroupBox9: TGroupBox
    Left = 8
    Top = 104
    Width = 233
    Height = 25
    TabOrder = 6
    object Label5: TLabel
      Left = 6
      Top = 8
      Width = 65
      Height = 13
      Caption = 'Current State:'
      Font.Charset = DEFAULT_CHARSET
      Font.Color = clWindowText
      Font.Height = -11
      Font.Name = 'MS Sans Serif'
      Font.Style = []
      ParentFont = False
    end
    object Label6: TLabel
      Left = 81
      Top = 8
      Width = 24
      Height = 13
      Caption = 'N/A'
      Font.Charset = ANSI_CHARSET
      Font.Color = clWindowText
      Font.Height = -11
      Font.Name = 'MS Sans Serif'
      Font.Style = [fsBold]
      ParentFont = False
    end
  end
  object TmrNillAll: TTimer
    Enabled = False
    Interval = 1
    OnTimer = TmrNillAllTimer
    Left = 312
    Top = 216
  end
end
