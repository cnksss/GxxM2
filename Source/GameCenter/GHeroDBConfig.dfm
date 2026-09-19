object FrmHeroDB: TFrmHeroDB
  Left = 965
  Top = 269
  BorderIcons = []
  BorderStyle = bsDialog
  Caption = 'HeroDB'#33258#21160#37197#32622
  ClientHeight = 330
  ClientWidth = 511
  Color = clBtnFace
  Font.Charset = GB2312_CHARSET
  Font.Color = clWindowText
  Font.Height = -12
  Font.Name = #23435#20307
  Font.Style = []
  OldCreateOrder = False
  Position = poOwnerFormCenter
  PixelsPerInch = 96
  TextHeight = 12
  object PageControl: TPageControl
    Left = 0
    Top = 0
    Width = 511
    Height = 289
    ActivePage = TabSheet2
    Align = alTop
    TabOrder = 0
    object TabSheet1: TTabSheet
      Caption = 'HeroDB'
      object Label1: TLabel
        Left = 16
        Top = 20
        Width = 66
        Height = 12
        Caption = #25968#25454#24211#21035#21517':'
      end
      object Label2: TLabel
        Left = 16
        Top = 44
        Width = 66
        Height = 12
        Caption = #25968#25454#24211#36335#24452':'
      end
      object EditHeroDB: TEdit
        Left = 88
        Top = 16
        Width = 121
        Height = 20
        ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
        TabOrder = 0
        Text = 'HeroDB'
      end
      object EditHeroDBPath: TRzButtonEdit
        Left = 88
        Top = 40
        Width = 249
        Height = 20
        ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
        TabOrder = 1
        AltBtnWidth = 15
        ButtonWidth = 15
        OnButtonClick = EditHeroDBPathButtonClick
      end
      object ButtonSaveHeroDBConfig: TButton
        Left = 384
        Top = 34
        Width = 97
        Height = 25
        Caption = #33258#21160#37197#32622'HeroDB'
        TabOrder = 2
        OnClick = ButtonSaveHeroDBConfigClick
      end
      object MemoLog: TMemo
        Left = 16
        Top = 72
        Width = 481
        Height = 177
        BorderStyle = bsNone
        Color = clBtnFace
        Ctl3D = False
        ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
        ParentCtl3D = False
        ReadOnly = True
        TabOrder = 3
      end
    end
    object TabSheet2: TTabSheet
      Caption = 'StdItems.DB'
      ImageIndex = 1
      object GroupBox1: TGroupBox
        Left = 8
        Top = 8
        Width = 193
        Height = 241
        Caption = 'StdItems.DB'#20013#32570#23569#20197#19979#23383#27573
        TabOrder = 0
        object ListBoxStdItems: TListBox
          Left = 8
          Top = 16
          Width = 177
          Height = 217
          ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ItemHeight = 12
          TabOrder = 0
        end
      end
      object ButtonCreateStdItemsField: TButton
        Left = 216
        Top = 224
        Width = 257
        Height = 25
        Caption = #33258#21160#21019#24314#25968#25454#24211#8220'StdItems'#8221#20013#32570#23569#30340#23383#27573
        TabOrder = 1
        OnClick = ButtonCreateStdItemsFieldClick
      end
      object MemoLog1: TMemo
        Left = 208
        Top = 16
        Width = 289
        Height = 177
        BorderStyle = bsNone
        Color = clBtnFace
        Ctl3D = False
        ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
        ParentCtl3D = False
        ReadOnly = True
        TabOrder = 2
      end
    end
    object TabSheet3: TTabSheet
      Caption = 'Monster.DB'
      ImageIndex = 2
      object GroupBox2: TGroupBox
        Left = 8
        Top = 8
        Width = 193
        Height = 241
        Caption = 'Monster.DB'#20013#32570#23569#20197#19979#23383#27573
        TabOrder = 0
        object ListBoxMonster: TListBox
          Left = 8
          Top = 16
          Width = 177
          Height = 217
          ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ItemHeight = 12
          TabOrder = 0
        end
      end
      object MemoLog2: TMemo
        Left = 208
        Top = 16
        Width = 289
        Height = 177
        BorderStyle = bsNone
        Color = clBtnFace
        Ctl3D = False
        ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
        ParentCtl3D = False
        ReadOnly = True
        TabOrder = 1
      end
      object ButtonMonsterField: TButton
        Left = 216
        Top = 224
        Width = 257
        Height = 25
        Caption = #33258#21160#21019#24314#25968#25454#24211#8220'Monster'#8221#20013#32570#23569#30340#23383#27573
        TabOrder = 2
        OnClick = ButtonMonsterFieldClick
      end
    end
    object TabSheet4: TTabSheet
      Caption = 'Magic.DB'
      ImageIndex = 3
      object GroupBox3: TGroupBox
        Left = 8
        Top = 8
        Width = 193
        Height = 241
        Caption = 'Magic.DB'#20013#32570#23569#20197#19979#23383#27573
        TabOrder = 0
        object ListBoxMagic: TListBox
          Left = 8
          Top = 16
          Width = 177
          Height = 217
          ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ItemHeight = 12
          TabOrder = 0
        end
      end
      object MemoLog3: TMemo
        Left = 208
        Top = 16
        Width = 289
        Height = 177
        BorderStyle = bsNone
        Color = clBtnFace
        Ctl3D = False
        ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
        ParentCtl3D = False
        ReadOnly = True
        TabOrder = 1
      end
      object ButtonMagicField: TButton
        Left = 216
        Top = 224
        Width = 257
        Height = 25
        Caption = #33258#21160#21019#24314#25968#25454#24211#8220'Magic'#8221#20013#32570#23569#30340#23383#27573
        TabOrder = 2
        OnClick = ButtonMagicFieldClick
      end
    end
  end
  object ButtonClose: TButton
    Left = 216
    Top = 296
    Width = 75
    Height = 25
    Caption = #21462#28040
    TabOrder = 1
    OnClick = ButtonCloseClick
  end
end
