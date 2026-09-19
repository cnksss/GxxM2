object FrmSetting: TFrmSetting
  Left = 451
  Top = 182
  BorderStyle = bsDialog
  Caption = #22522#26412#35774#32622
  ClientHeight = 490
  ClientWidth = 737
  Color = clBtnFace
  Font.Charset = GB2312_CHARSET
  Font.Color = clWindowText
  Font.Height = -12
  Font.Name = #23435#20307
  Font.Style = []
  OldCreateOrder = False
  Position = poMainFormCenter
  PixelsPerInch = 96
  TextHeight = 12
  object ButtonOK: TButton
    Left = 558
    Top = 456
    Width = 75
    Height = 25
    Caption = #30830#23450'(&O)'
    TabOrder = 0
    OnClick = ButtonOKClick
  end
  object PageControl1: TPageControl
    Left = 8
    Top = 8
    Width = 721
    Height = 441
    ActivePage = TabSheet1
    TabOrder = 1
    object TabSheet1: TTabSheet
      Caption = #22522#26412#35774#32622
      object GroupBox1: TGroupBox
        Left = 8
        Top = 5
        Width = 345
        Height = 404
        Caption = #22522#26412#35774#32622
        TabOrder = 0
        object Label1: TLabel
          Left = 12
          Top = 136
          Width = 84
          Height = 12
          Caption = #20801#35768#21019#24314#35282#33394#25968
        end
        object Label2: TLabel
          Left = 192
          Top = 38
          Width = 120
          Height = 12
          Caption = #20197#19978#32423#21035#19981#20801#35768#34987#21024#38500
        end
        object Label3: TLabel
          Left = 12
          Top = 160
          Width = 240
          Height = 12
          Caption = #31105#27490#24314#31435#21253#21547#20197#19979#23383#31526#30340#20154#29289#21517#31216'('#19968#34892#19968#20010')'
        end
        object CheckBoxDenyChrName: TCheckBox
          Left = 12
          Top = 76
          Width = 145
          Height = 17
          Caption = #20801#35768#29305#27530#23383#31526#21019#24314#20154#29289
          TabOrder = 0
        end
        object CheckBoxCanDeleteHuman: TCheckBox
          Left = 12
          Top = 36
          Width = 92
          Height = 17
          Caption = #20801#35768#21024#38500#20154#29289
          TabOrder = 1
        end
        object EditCreateChrNameCount: TSpinEdit
          Left = 104
          Top = 134
          Width = 57
          Height = 21
          MaxValue = 20
          MinValue = 1
          TabOrder = 2
          Value = 2
        end
        object CheckBoxCanCreateHuman: TCheckBox
          Left = 12
          Top = 16
          Width = 108
          Height = 17
          Caption = #20801#35768#24314#31435#26032#20154#29289
          TabOrder = 3
        end
        object CheckBoxCanGetBackDeleteHuman: TCheckBox
          Left = 12
          Top = 56
          Width = 116
          Height = 17
          Caption = #20801#35768#25214#22238#21024#38500#20154#29289
          TabOrder = 4
        end
        object EditCanDeleteHumanLowLevel: TSpinEdit
          Left = 136
          Top = 32
          Width = 49
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 5
          Value = 0
        end
        object CheckBoxForbidNumberName: TCheckBox
          Left = 12
          Top = 96
          Width = 173
          Height = 17
          Caption = #31105#27490#24314#31435#21253#21547#25968#23383#30340#20154#29289#21517
          TabOrder = 6
        end
        object CheckBoxForbidLetterName: TCheckBox
          Left = 12
          Top = 116
          Width = 141
          Height = 17
          Caption = #31105#27490#24314#31435#20840#33521#25991#20154#29289#21517
          TabOrder = 7
        end
        object MemoFilterNewHumanName: TMemo
          Left = 16
          Top = 176
          Width = 233
          Height = 218
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ScrollBars = ssBoth
          TabOrder = 8
        end
      end
      object GroupBox2: TGroupBox
        Left = 360
        Top = 5
        Width = 345
        Height = 403
        Caption = #20854#20182#35774#32622
        TabOrder = 1
        object Label4: TLabel
          Left = 12
          Top = 157
          Width = 276
          Height = 12
          Caption = #31105#27490#21253#21547#20197#19979#23383#31526#30340#20154#29289#21517#31216#36827#20837#25490#34892#27036'('#19968#34892#19968#20010')'
        end
        object CheckBoxRanking: TCheckBox
          Left = 12
          Top = 136
          Width = 81
          Height = 17
          Caption = #24320#21551#25490#34892#27036
          TabOrder = 0
        end
        object MemoFilterRankingName: TMemo
          Left = 8
          Top = 176
          Width = 233
          Height = 218
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ScrollBars = ssBoth
          TabOrder = 1
        end
        object chkUseActiveRunGage: TCheckBox
          Left = 8
          Top = 16
          Width = 209
          Height = 17
          Caption = #21482#20998#37197#21487#36830#25509#30340#28216#25103#32593#20851#32473#23458#25143#31471
          TabOrder = 2
        end
        object chkShowBlockIPLog: TCheckBox
          Left = 8
          Top = 34
          Width = 169
          Height = 17
          Caption = #26174#31034#38750#27861#35831#27714#20869#37096#31471#21475#26085#24535
          TabOrder = 3
        end
      end
    end
  end
  object Button1: TButton
    Left = 654
    Top = 456
    Width = 75
    Height = 25
    Caption = #21462#28040'(&C)'
    ModalResult = 2
    TabOrder = 2
    OnClick = ButtonOKClick
  end
end
