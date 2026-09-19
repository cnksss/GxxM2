object Form1: TForm1
  Left = 267
  Top = 201
  BorderIcons = [biSystemMenu, biMinimize]
  BorderStyle = bsSingle
  Caption = 'RSA'#35745#31639#24037#20855
  ClientHeight = 237
  ClientWidth = 529
  Color = clBtnFace
  Font.Charset = ANSI_CHARSET
  Font.Color = clWindowText
  Font.Height = -12
  Font.Name = #23435#20307
  Font.Style = []
  OldCreateOrder = False
  OnCreate = FormCreate
  PixelsPerInch = 96
  TextHeight = 12
  object Label5: TLabel
    Left = 8
    Top = 187
    Width = 41
    Height = 12
    AutoSize = False
    Caption = #20301#25968#65306
  end
  object GroupBox1: TGroupBox
    Left = 8
    Top = 8
    Width = 513
    Height = 81
    Caption = #20844#38053#35745#31639
    TabOrder = 0
    object Label1: TLabel
      Left = 8
      Top = 19
      Width = 41
      Height = 12
      AutoSize = False
      Caption = #27169#25968#65306
    end
    object Label2: TLabel
      Left = 8
      Top = 51
      Width = 41
      Height = 12
      AutoSize = False
      Caption = #20844#38053#65306
    end
    object Edit1: TEdit
      Left = 40
      Top = 16
      Width = 465
      Height = 20
      TabOrder = 0
    end
    object Edit2: TEdit
      Left = 40
      Top = 48
      Width = 465
      Height = 20
      TabOrder = 1
    end
  end
  object GroupBox2: TGroupBox
    Left = 8
    Top = 96
    Width = 513
    Height = 81
    Caption = #31169#38053#35745#31639
    TabOrder = 1
    object Label3: TLabel
      Left = 8
      Top = 19
      Width = 41
      Height = 12
      AutoSize = False
      Caption = #27169#25968#65306
    end
    object Label4: TLabel
      Left = 8
      Top = 51
      Width = 41
      Height = 12
      AutoSize = False
      Caption = #31169#38053#65306
    end
    object Edit3: TEdit
      Left = 40
      Top = 16
      Width = 465
      Height = 20
      TabOrder = 0
    end
    object Edit4: TEdit
      Left = 40
      Top = 48
      Width = 465
      Height = 20
      TabOrder = 1
    end
  end
  object Button1: TButton
    Left = 344
    Top = 184
    Width = 75
    Height = 25
    Caption = #24320#22987#35745#31639
    TabOrder = 2
    OnClick = Button1Click
  end
  object Button2: TButton
    Left = 440
    Top = 184
    Width = 75
    Height = 25
    Caption = #36864#20986
    TabOrder = 3
    OnClick = Button2Click
  end
  object ComboBox1: TComboBox
    Left = 40
    Top = 184
    Width = 89
    Height = 20
    ItemHeight = 12
    TabOrder = 4
    Items.Strings = (
      '128'
      '256'
      '512'
      '768'
      '1024'
      '2048')
  end
  object StatusBar1: TStatusBar
    Left = 0
    Top = 218
    Width = 529
    Height = 19
    Panels = <
      item
        Text = #12298#36719#20214#21152#35299#23494#25216#26415'-'#36719#20214#21152#23494#12299
        Width = 170
      end
      item
        Text = #20316#32773#65306#21490#23376#33635
        Width = 90
      end
      item
        Text = 'http://www.pefine.com'
        Width = 50
      end>
  end
end
