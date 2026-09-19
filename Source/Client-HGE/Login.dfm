object FrmLogin: TFrmLogin
  Left = 1285
  Top = 278
  BorderIcons = [biSystemMenu, biMinimize]
  BorderStyle = bsSingle
  Caption = 'Gxx'#32593#32476#30331#24405#22120
  ClientHeight = 478
  ClientWidth = 539
  Color = clBtnFace
  Font.Charset = GB2312_CHARSET
  Font.Color = clWindowText
  Font.Height = -12
  Font.Name = #23435#20307
  Font.Style = []
  OldCreateOrder = False
  Position = poScreenCenter
  OnClose = FormClose
  OnCreate = FormCreate
  OnDestroy = FormDestroy
  PixelsPerInch = 96
  TextHeight = 12
  object lblGameAccount: TLabel
    Left = 8
    Top = 447
    Width = 24
    Height = 12
    Caption = #36134#21495
  end
  object lblGameAccountPassword: TLabel
    Left = 147
    Top = 448
    Width = 24
    Height = 12
    Caption = #23494#30721
  end
  object GroupBox1: TGroupBox
    Left = 8
    Top = 0
    Width = 523
    Height = 289
    Caption = #28216#25103#21015#34920
    TabOrder = 1
    object Label1: TLabel
      Left = 8
      Top = 160
      Width = 48
      Height = 12
      Caption = #28216#25103#21517#31216
      Transparent = True
    end
    object Label2: TLabel
      Left = 8
      Top = 186
      Width = 48
      Height = 24
      Caption = #30331#24405#22320#22336#13#10
      Transparent = True
    end
    object Label3: TLabel
      Left = 179
      Top = 186
      Width = 48
      Height = 12
      Caption = #30331#24405#31471#21475
      Transparent = True
    end
    object Label5: TLabel
      Left = 8
      Top = 212
      Width = 48
      Height = 12
      Caption = #24494#31471#22320#22336
      Transparent = True
    end
    object Label6: TLabel
      Left = 179
      Top = 212
      Width = 48
      Height = 12
      Caption = #24494#31471#31471#21475
      Transparent = True
    end
    object Label7: TLabel
      Left = 348
      Top = 212
      Width = 48
      Height = 12
      Caption = #24494#31471#23494#30721
      Transparent = True
    end
    object Label4: TLabel
      Left = 348
      Top = 186
      Width = 48
      Height = 12
      Caption = #30331#24405#23494#30721
    end
    object Label8: TLabel
      Left = 8
      Top = 238
      Width = 48
      Height = 12
      Caption = #36164#28304#30446#24405
      Transparent = True
    end
    object Label9: TLabel
      Left = 177
      Top = 238
      Width = 48
      Height = 12
      Caption = #24517#22791#34917#19969
      Transparent = True
    end
    object ListView: TListView
      Left = 3
      Top = 21
      Width = 517
      Height = 129
      Columns = <
        item
          Caption = #21517#31216
          Width = 150
        end
        item
          Caption = #22320#22336
          Width = 150
        end
        item
          Caption = #31471#21475
          Width = 60
        end
        item
          Caption = #23494#30721
          Width = 80
        end>
      GridLines = True
      ReadOnly = True
      RowSelect = True
      TabOrder = 10
      ViewStyle = vsReport
      OnClick = ListViewClick
    end
    object ButtonAdd: TButton
      Left = 197
      Top = 261
      Width = 73
      Height = 25
      Caption = #22686#21152
      TabOrder = 11
      OnClick = ButtonAddClick
    end
    object ButtonDel: TButton
      Left = 280
      Top = 261
      Width = 73
      Height = 25
      Caption = #21024#38500
      TabOrder = 12
      OnClick = ButtonDelClick
    end
    object ButtonSave: TButton
      Left = 445
      Top = 261
      Width = 75
      Height = 25
      Caption = #20445#23384
      TabOrder = 13
      OnClick = ButtonSaveClick
    end
    object EditServerAddr: TEdit
      Left = 62
      Top = 182
      Width = 109
      Height = 20
      Hint = #28216#25103#26381#21153#22120'IP'#22320#22336
      Font.Charset = GB2312_CHARSET
      Font.Color = clBlack
      Font.Height = -12
      Font.Name = #23435#20307
      Font.Style = []
      ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
      ParentFont = False
      TabOrder = 1
      Text = '127.0.0.1'
    end
    object EditServerPort: TEdit
      Left = 231
      Top = 182
      Width = 109
      Height = 20
      Font.Charset = GB2312_CHARSET
      Font.Color = clBlack
      Font.Height = -12
      Font.Name = #23435#20307
      Font.Style = []
      ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
      ParentFont = False
      TabOrder = 2
      Text = '7000'
    end
    object EditServerName: TEdit
      Left = 62
      Top = 156
      Width = 278
      Height = 20
      Hint = #28216#25103#26381#21153#22120#21517#31216
      Font.Charset = DEFAULT_CHARSET
      Font.Color = clBlack
      Font.Height = -12
      Font.Name = #23435#20307
      Font.Style = []
      ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
      ParentFont = False
      TabOrder = 0
      Text = #20256#22855#22806#20256
    end
    object edtMicroIP: TEdit
      Left = 62
      Top = 208
      Width = 109
      Height = 20
      Font.Charset = GB2312_CHARSET
      Font.Color = clBlack
      Font.Height = -12
      Font.Name = #23435#20307
      Font.Style = []
      ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
      ParentFont = False
      TabOrder = 4
    end
    object edtMicroPort: TEdit
      Left = 231
      Top = 208
      Width = 109
      Height = 20
      Font.Charset = GB2312_CHARSET
      Font.Color = clBlack
      Font.Height = -12
      Font.Name = #23435#20307
      Font.Style = []
      ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
      ParentFont = False
      TabOrder = 5
    end
    object chkMicroMode: TCheckBox
      Left = 452
      Top = 157
      Width = 68
      Height = 17
      BiDiMode = bdLeftToRight
      Caption = #24494#31471#27169#24335
      ParentBiDiMode = False
      TabOrder = 9
    end
    object edtMicroPassword: TEdit
      Left = 402
      Top = 208
      Width = 118
      Height = 20
      Font.Charset = GB2312_CHARSET
      Font.Color = clBlack
      Font.Height = -12
      Font.Name = #23435#20307
      Font.Style = []
      ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
      ParentFont = False
      TabOrder = 6
      Text = 'GxxM2'
    end
    object EditRunGatePassword: TEdit
      Left = 402
      Top = 182
      Width = 118
      Height = 20
      ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
      TabOrder = 3
      Text = 'GxxM2'
      OnChange = EditRunGatePasswordChange
    end
    object EditResourceDir: TEdit
      Left = 62
      Top = 234
      Width = 109
      Height = 20
      Font.Charset = GB2312_CHARSET
      Font.Color = clBlack
      Font.Height = -12
      Font.Name = #23435#20307
      Font.Style = []
      ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
      ParentFont = False
      TabOrder = 7
    end
    object btnUpdatte: TButton
      Left = 362
      Top = 261
      Width = 73
      Height = 25
      Caption = #26356#26032
      TabOrder = 14
      OnClick = btnUpdatteClick
    end
    object edtGamePlanFile: TEdit
      Left = 231
      Top = 234
      Width = 109
      Height = 20
      Font.Charset = GB2312_CHARSET
      Font.Color = clBlack
      Font.Height = -12
      Font.Name = #23435#20307
      Font.Style = []
      ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
      ParentFont = False
      TabOrder = 8
      Text = 'Newopui.pak'
    end
    object chkShowOpenDoor: TCheckBox
      Left = 357
      Top = 157
      Width = 89
      Height = 17
      Caption = #26174#31034#24320#38376#21160#20316
      TabOrder = 15
    end
  end
  object ButtonStart: TButton
    Left = 460
    Top = 441
    Width = 75
    Height = 25
    Caption = #30331#24405
    TabOrder = 0
    OnClick = ButtonStartClick
  end
  object ButtonClose: TButton
    Left = 381
    Top = 441
    Width = 73
    Height = 25
    Caption = #20851#38381
    TabOrder = 2
    OnClick = ButtonCloseClick
  end
  object RadioGroup: TRzRadioGroup
    Left = 8
    Top = 303
    Width = 523
    Height = 42
    Columns = 6
    ItemIndex = 3
    Items.Strings = (
      '1.76'
      '1.85'
      #33521#38596#29256
      #36830#20987#29256
      #20256#22855#32493#31456
      '205'#26032#30028#38754)
    TabOrder = 3
  end
  object GroupBox2: TGroupBox
    Left = 8
    Top = 358
    Width = 525
    Height = 76
    TabOrder = 4
    object lblColor: TLabel
      Left = 6
      Top = 50
      Width = 48
      Height = 12
      Caption = #39068#33394#20301#25968
    end
    object lblScreenMode: TLabel
      Left = 174
      Top = 50
      Width = 36
      Height = 12
      Caption = #20998#36776#29575
    end
    object Label10: TLabel
      Left = 370
      Top = 50
      Width = 48
      Height = 12
      Caption = #20869#26680#27169#24335
    end
    object CheckBoxD3DFormat: TCheckBox
      Left = 85
      Top = 3
      Width = 73
      Height = 17
      Caption = #32441#29702#21387#32553
      Checked = True
      State = cbChecked
      TabOrder = 0
    end
    object CheckBoxDepthStencil: TCheckBox
      Left = 244
      Top = 3
      Width = 89
      Height = 17
      Caption = 'DepthStencil'
      Checked = True
      State = cbChecked
      TabOrder = 1
    end
    object CheckBoxHardware: TCheckBox
      Left = 343
      Top = 3
      Width = 81
      Height = 17
      Caption = 'Hardware'
      TabOrder = 2
    end
    object CheckBoxVSync: TCheckBox
      Left = 164
      Top = 3
      Width = 65
      Height = 17
      Caption = #22402#30452#21516#27493
      TabOrder = 3
    end
    object CheckBoxWindowMode: TCheckBox
      Left = 6
      Top = 3
      Width = 73
      Height = 17
      Caption = #31383#21475#27169#24335
      Checked = True
      State = cbChecked
      TabOrder = 4
      OnClick = CheckBoxWindowModeClick
    end
    object ComboBoxBitCount: TComboBox
      Left = 60
      Top = 46
      Width = 98
      Height = 20
      Style = csDropDownList
      ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
      ItemHeight = 12
      ItemIndex = 0
      TabOrder = 5
      Text = '32'#20301
      Items.Strings = (
        '32'#20301
        '16'#20301)
    end
    object ComboBoxScreenMode: TComboBox
      Left = 216
      Top = 46
      Width = 141
      Height = 20
      Style = csDropDownList
      ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
      ItemHeight = 12
      TabOrder = 6
      Items.Strings = (
        '800 * 600'
        '1024 * 768'
        '900 * 600')
    end
    object chkCustomUI: TCheckBox
      Left = 6
      Top = 23
      Width = 97
      Height = 17
      Caption = #20351#29992#33258#23450#20041'UI'
      Checked = True
      State = cbChecked
      TabOrder = 7
    end
    object chkShowPropertyGroupCaption: TCheckBox
      Left = 216
      Top = 23
      Width = 137
      Height = 17
      Caption = #26174#31034#23646#24615#20998#32452#26631#39064
      TabOrder = 8
    end
    object chkShow1024UI: TCheckBox
      Left = 109
      Top = 23
      Width = 101
      Height = 17
      Caption = #20351#29992'1024'#30028#38754
      TabOrder = 9
    end
    object cbbClientMode: TComboBox
      Left = 424
      Top = 46
      Width = 98
      Height = 20
      Style = csDropDownList
      ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
      ItemHeight = 12
      ItemIndex = 0
      TabOrder = 10
      Text = #27491#24120#27169#24335
      OnChange = cbbClientModeChange
      Items.Strings = (
        #27491#24120#27169#24335
        #29992#25143#20013#24515#27169#24335
        #23450#21046#27169#24335)
    end
  end
  object edtGameAccountPassword: TEdit
    Left = 177
    Top = 443
    Width = 99
    Height = 20
    PasswordChar = '*'
    TabOrder = 6
  end
  object cbbGameAccount: TComboBox
    Left = 38
    Top = 443
    Width = 99
    Height = 20
    ItemHeight = 12
    TabOrder = 5
    OnChange = cbbGameAccountChange
  end
  object btnUpdateAccount: TButton
    Left = 282
    Top = 441
    Width = 39
    Height = 25
    Caption = #26356#26032
    TabOrder = 7
    OnClick = btnUpdateAccountClick
  end
  object btnDelAccount: TButton
    Left = 327
    Top = 441
    Width = 39
    Height = 25
    Caption = #21024#38500
    TabOrder = 8
    OnClick = btnDelAccountClick
  end
end
