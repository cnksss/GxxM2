object FrmBDEToSqlite: TFrmBDEToSqlite
  Left = 0
  Top = 0
  BorderIcons = [biSystemMenu]
  Caption = #25968#25454#24211#36716#25442
  ClientHeight = 103
  ClientWidth = 495
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clBlack
  Font.Height = -11
  Font.Name = 'Tahoma'
  Font.Style = []
  OldCreateOrder = False
  Position = poScreenCenter
  PixelsPerInch = 96
  TextHeight = 13
  object lbl1: TLabel
    Left = 9
    Top = 16
    Width = 84
    Height = 13
    Caption = #28304#25968#25454#24211#30446#24405#65306
  end
  object lbl2: TLabel
    Left = 9
    Top = 42
    Width = 86
    Height = 13
    Caption = 'Sqlite'#20445#23384#25991#20214#65306
  end
  object lbl3: TLabel
    Left = 8
    Top = 66
    Width = 318
    Height = 26
    Caption = #35828#26126':'#36716#25442#23436#21518#35774#32622#37197#32622#21521#23548#35835#21462#36716#23436#21518#30340'Sqlite'#25968#25454#24211#65281#65281#65281#13#10#36716#25442#21069#35831#20808#22791#20221#25968#25454#65281
    Font.Charset = DEFAULT_CHARSET
    Font.Color = clRed
    Font.Height = -11
    Font.Name = 'Tahoma'
    Font.Style = []
    ParentFont = False
  end
  object btnSrc: TRzButtonEdit
    Left = 91
    Top = 12
    Width = 401
    Height = 21
    Text = 'D:\MirServer\Mud2\DB\'
    TabOrder = 0
    AltBtnWidth = 15
    ButtonWidth = 15
    OnButtonClick = btnSrcButtonClick
  end
  object btnDest: TRzButtonEdit
    Left = 91
    Top = 39
    Width = 401
    Height = 21
    Text = 'D:\MirServer\Mud2\DB\GxxM2.db'
    TabOrder = 1
    AltBtnWidth = 15
    ButtonWidth = 15
    OnButtonClick = btnDestButtonClick
  end
  object FBtnbtn1: TButton
    Left = 410
    Top = 68
    Width = 75
    Height = 25
    Caption = #36716#25442
    TabOrder = 2
    OnClick = FBtnbtn1Click
  end
  object CheckBox1: TCheckBox
    Left = 341
    Top = 72
    Width = 65
    Height = 17
    Caption = #37325#26032#25490#24207
    TabOrder = 3
  end
end
