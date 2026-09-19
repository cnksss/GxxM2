object TextSearchDialog: TTextSearchDialog
  Left = 132
  Top = 168
  BorderStyle = bsDialog
  Caption = #25628#32032#25991#26412
  ClientHeight = 182
  ClientWidth = 333
  Color = clBtnFace
  Font.Charset = GB2312_CHARSET
  Font.Color = clWindowText
  Font.Height = -12
  Font.Name = #23435#20307
  Font.Style = []
  Position = poScreenCenter
  OnCloseQuery = FormCloseQuery
  TextHeight = 12
  object Label1: TLabel
    Left = 10
    Top = 15
    Width = 84
    Height = 12
    Caption = #25628#32034#30446#26631#25991#26412#65306
  end
  object cbSearchText: TComboBox
    Left = 94
    Top = 11
    Width = 228
    Height = 20
    TabOrder = 0
  end
  object gbSearchOptions: TGroupBox
    Left = 10
    Top = 43
    Width = 154
    Height = 127
    Caption = #25628#32034#36873#39033
    TabOrder = 1
    object cbSearchCaseSensitive: TCheckBox
      Left = 8
      Top = 17
      Width = 133
      Height = 17
      Caption = #21306#20998#22823#23567#20889
      TabOrder = 0
    end
    object cbSearchWholeWords: TCheckBox
      Left = 8
      Top = 39
      Width = 133
      Height = 17
      Caption = #20840#23383#31526#21305#37197
      TabOrder = 1
    end
    object cbSearchFromCursor: TCheckBox
      Left = 8
      Top = 61
      Width = 133
      Height = 17
      Caption = #20174#20809#26631#22788#24320#22987#25628#32034
      TabOrder = 2
    end
    object cbSearchSelectedOnly: TCheckBox
      Left = 8
      Top = 83
      Width = 133
      Height = 17
      Caption = #22312#36873#20013#25991#26412#20013#25628#32034
      TabOrder = 3
    end
    object cbRegularExpression: TCheckBox
      Left = 8
      Top = 104
      Width = 133
      Height = 17
      Caption = #27491#21017#34920#36798#24335
      TabOrder = 4
    end
  end
  object rgSearchDirection: TRadioGroup
    Left = 171
    Top = 43
    Width = 151
    Height = 65
    Caption = #25628#32034#26041#21521
    ItemIndex = 0
    Items.Strings = (
      #27491#21521#25628#32034
      #21453#21521#25628#32034)
    TabOrder = 2
  end
  object btnOK: TButton
    Left = 174
    Top = 147
    Width = 70
    Height = 23
    Caption = #30830#35748
    Default = True
    ModalResult = 1
    TabOrder = 3
  end
  object btnCancel: TButton
    Left = 249
    Top = 147
    Width = 70
    Height = 23
    Cancel = True
    Caption = #21462#28040
    ModalResult = 2
    TabOrder = 4
  end
end
