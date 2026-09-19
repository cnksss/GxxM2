object Form1: TForm1
  Left = 192
  Top = 114
  BorderIcons = [biSystemMenu, biMinimize]
  BorderStyle = bsSingle
  Caption = 'RSA'#31639#27861#28436#31034
  ClientHeight = 412
  ClientWidth = 657
  Color = clBtnFace
  Font.Charset = GB2312_CHARSET
  Font.Color = clWindowText
  Font.Height = -12
  Font.Name = #23435#20307
  Font.Style = []
  OldCreateOrder = False
  OnCreate = FormCreate
  PixelsPerInch = 96
  TextHeight = 12
  object Label1: TLabel
    Left = 16
    Top = 16
    Width = 41
    Height = 12
    AutoSize = False
    Caption = #26126#25991#65306
  end
  object Label2: TLabel
    Left = 16
    Top = 83
    Width = 49
    Height = 12
    AutoSize = False
    Caption = #20844#38053#65306
  end
  object Label3: TLabel
    Left = 16
    Top = 115
    Width = 41
    Height = 12
    AutoSize = False
    Caption = #31169#38053#65306
  end
  object Label4: TLabel
    Left = 16
    Top = 147
    Width = 41
    Height = 12
    AutoSize = False
    Caption = #27169#25968#65306
  end
  object Label5: TLabel
    Left = 16
    Top = 176
    Width = 41
    Height = 12
    AutoSize = False
    Caption = #23494#25991#65306
  end
  object Label6: TLabel
    Left = 16
    Top = 240
    Width = 41
    Height = 12
    AutoSize = False
    Caption = #26126#25991#65306
  end
  object Label7: TLabel
    Left = 16
    Top = 339
    Width = 41
    Height = 12
    AutoSize = False
    Caption = #20301#25968#65306
  end
  object MText: TMemo
    Left = 16
    Top = 32
    Width = 625
    Height = 41
    TabOrder = 0
  end
  object KeyText1: TEdit
    Left = 48
    Top = 80
    Width = 593
    Height = 20
    TabOrder = 1
    Text = 'B71C'
  end
  object KeyText2: TEdit
    Left = 48
    Top = 112
    Width = 593
    Height = 20
    TabOrder = 2
    Text = '8759F63945406F3DFCA9EBCDEC25384F'
  end
  object ModText: TEdit
    Left = 48
    Top = 144
    Width = 593
    Height = 20
    TabOrder = 3
    Text = '47DD20F97D9B3D98E0B25096F88E987E'
  end
  object CText: TMemo
    Left = 16
    Top = 192
    Width = 625
    Height = 41
    Lines.Strings = (
      
        '376D52666B396F496E56676434787A2B4131752F4F377779664C55326C65442F' +
        '444A37574E694C50386B59724364656D41776D6'
      
        '56A4577495A4948764F38784A475A2F4866577037795A614964506B6F3651695' +
        '550487A5379384C4E506B614230326A5A33427A'
      
        '6974377337394E513976365148636D356150534B746A77557656723970706435' +
        '4A68754932567357682F637A38322B6B33612F4'
      
        '46F6A30617A4247666658636846425147744E347A646258674B485A35616D5A5' +
        '6686D5852646D76614C2F314477354574364C71'
      
        '636148392F73313934355362673743364E54754F7742477738714E4348392F4D' +
        '50556D3448764D5150696E50534A63526D64395'
      
        '5664D6D48782B476B38346E6B61456B566F497371674A366B735774685073735' +
        '23846304565494C30673D')
    TabOrder = 4
  end
  object PText: TMemo
    Left = 16
    Top = 256
    Width = 625
    Height = 41
    TabOrder = 5
  end
  object ComboBox1: TComboBox
    Left = 48
    Top = 336
    Width = 145
    Height = 20
    TabOrder = 6
    Items.Strings = (
      '128'#20301
      '256'#20301
      '512'#20301
      '768'#20301
      '1024'#20301
      '2048'#20301)
  end
  object Button1: TButton
    Left = 320
    Top = 312
    Width = 153
    Height = 73
    Caption = #21152#23494
    TabOrder = 7
    OnClick = Button1Click
  end
  object Button2: TButton
    Left = 488
    Top = 312
    Width = 153
    Height = 73
    Caption = #35299#23494
    TabOrder = 8
    OnClick = Button2Click
  end
  object StatusBar1: TStatusBar
    Left = 0
    Top = 393
    Width = 657
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
  object chk1: TCheckBox
    Left = 192
    Top = 8
    Width = 97
    Height = 17
    Caption = 'chk1'
    TabOrder = 10
  end
  object Button3: TButton
    Left = 240
    Top = 344
    Width = 75
    Height = 25
    Caption = 'Button3'
    TabOrder = 11
    OnClick = Button3Click
  end
end
