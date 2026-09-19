object ftmClientModules: TftmClientModules
  Left = 393
  Top = 230
  Caption = #23458#25143#31471#27169#22359#30333#21517#21333
  ClientHeight = 435
  ClientWidth = 726
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'Tahoma'
  Font.Style = []
  Position = poMainFormCenter
  TextHeight = 13
  object Panel1: TPanel
    Left = 0
    Top = 344
    Width = 726
    Height = 91
    Align = alBottom
    BevelOuter = bvNone
    Ctl3D = False
    ParentCtl3D = False
    TabOrder = 0
    ExplicitTop = 343
    ExplicitWidth = 722
    object Label1: TLabel
      Left = 168
      Top = 10
      Width = 52
      Height = 13
      Caption = #25991#20214#21517#31216':'
    end
    object Label2: TLabel
      Left = 192
      Top = 32
      Width = 25
      Height = 13
      Caption = 'MD5:'
    end
    object ButtonModuleAdd: TButton
      Left = 400
      Top = 58
      Width = 75
      Height = 25
      Caption = #22686#21152
      TabOrder = 0
      OnClick = ButtonModuleAddClick
    end
    object ButtonModuleDel: TButton
      Left = 480
      Top = 58
      Width = 75
      Height = 25
      Caption = #21024#38500
      TabOrder = 1
      OnClick = ButtonModuleDelClick
    end
    object ButtonModuleSave: TButton
      Left = 560
      Top = 58
      Width = 75
      Height = 25
      Caption = #20445#23384
      TabOrder = 2
      OnClick = ButtonModuleSaveClick
    end
    object EditFileName: TEdit
      Left = 224
      Top = 8
      Width = 489
      Height = 19
      TabOrder = 3
    end
    object EditMD5: TEdit
      Left = 224
      Top = 32
      Width = 193
      Height = 19
      TabOrder = 4
    end
    object ButtonLoad: TButton
      Left = 640
      Top = 58
      Width = 75
      Height = 25
      Caption = #37325#26032#21152#36733
      TabOrder = 5
      OnClick = ButtonLoadClick
    end
    object CheckBoxGetCheckModule: TCheckBox
      Left = 8
      Top = 8
      Width = 105
      Height = 17
      Caption = #24320#21551#21453#22806#25346#31995#32479
      TabOrder = 6
      OnClick = CheckBoxGetCheckModuleClick
    end
    object RadioGroupModule: TRadioGroup
      Left = 8
      Top = 32
      Width = 169
      Height = 57
      ItemIndex = 0
      Items.Strings = (
        #30333#21517#21333
        #40657#21517#21333)
      TabOrder = 7
      OnClick = RadioGroupModuleClick
    end
    object CheckBoxClientAddModule: TCheckBox
      Left = 8
      Top = 32
      Width = 161
      Height = 17
      Caption = #23458#25143#31471#33258#21160#25552#20132#21487#30097#27169#22359#21040
      TabOrder = 8
      OnClick = CheckBoxClientAddModuleClick
    end
    object RadioButtonWhiteModule: TRadioButton
      Left = 432
      Top = 32
      Width = 65
      Height = 17
      Caption = #30333#21517#21333
      Checked = True
      TabOrder = 9
      TabStop = True
    end
    object RadioButtonBlackModule: TRadioButton
      Left = 504
      Top = 32
      Width = 65
      Height = 17
      Caption = #40657#21517#21333
      TabOrder = 10
    end
  end
  object PageControl: TPageControl
    Left = 0
    Top = 0
    Width = 726
    Height = 344
    ActivePage = TabSheet1
    Align = alClient
    TabOrder = 1
    ExplicitWidth = 722
    ExplicitHeight = 343
    object TabSheet1: TTabSheet
      Caption = #30333#21517#21333
      object ListViewClientModule: TListView
        Left = 0
        Top = 0
        Width = 718
        Height = 316
        Align = alClient
        Columns = <
          item
            Caption = #24207#21495
          end
          item
            Caption = #28155#21152#27169#24335
            Width = 80
          end
          item
            Caption = 'MD5'
            Width = 220
          end
          item
            Caption = #25991#20214#21517#31216
            Width = 350
          end>
        GridLines = True
        ReadOnly = True
        RowSelect = True
        TabOrder = 0
        ViewStyle = vsReport
        OnClick = ListViewClientModuleClick
        ExplicitWidth = 714
        ExplicitHeight = 315
      end
    end
    object TabSheet2: TTabSheet
      Caption = #40657#21517#21333
      ImageIndex = 1
      object ListViewClientBlackModule: TListView
        Left = 0
        Top = 0
        Width = 718
        Height = 316
        Align = alClient
        Columns = <
          item
            Caption = #24207#21495
          end
          item
            Caption = #28155#21152#27169#24335
            Width = 80
          end
          item
            Caption = 'MD5'
            Width = 220
          end
          item
            Caption = #25991#20214#21517#31216
            Width = 350
          end>
        GridLines = True
        ReadOnly = True
        RowSelect = True
        TabOrder = 0
        ViewStyle = vsReport
        OnClick = ListViewClientBlackModuleClick
        ExplicitWidth = 714
      end
    end
  end
end
