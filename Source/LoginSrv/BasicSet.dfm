object FrmBasicSet: TFrmBasicSet
  Left = 134
  Top = 435
  BorderIcons = [biSystemMenu, biMinimize]
  BorderStyle = bsSingle
  Caption = #22522#26412#35774#32622
  ClientHeight = 372
  ClientWidth = 419
  Color = clBtnFace
  Font.Charset = GB2312_CHARSET
  Font.Color = clWindowText
  Font.Height = -12
  Font.Name = #23435#20307
  Font.Style = []
  OldCreateOrder = False
  Position = poOwnerFormCenter
  OnCreate = FormCreate
  PixelsPerInch = 96
  TextHeight = 12
  object PageControl1: TPageControl
    Left = 8
    Top = 8
    Width = 401
    Height = 329
    ActivePage = TabSheet1
    TabOrder = 0
    object TabSheet1: TTabSheet
      Caption = #26222#36890#35774#32622
      object GroupBox1: TGroupBox
        Left = 6
        Top = 2
        Width = 167
        Height = 159
        Caption = #21151#33021#35774#32622
        TabOrder = 0
        object CheckBoxTestServer: TCheckBox
          Left = 8
          Top = 15
          Width = 156
          Height = 17
          Caption = #27979#35797#27169#24335
          TabOrder = 0
          OnClick = CheckBoxTestServerClick
        end
        object CheckBoxEnableMakingID: TCheckBox
          Left = 8
          Top = 32
          Width = 156
          Height = 17
          Caption = #20801#35768#21019#24314#36134#21495
          TabOrder = 1
          OnClick = CheckBoxEnableMakingIDClick
        end
        object CheckBoxEnableGetbackPassword: TCheckBox
          Left = 8
          Top = 50
          Width = 156
          Height = 17
          Caption = #20801#35768#21462#22238#23494#30721
          TabOrder = 2
          OnClick = CheckBoxEnableGetbackPasswordClick
        end
        object chkDisableIDSamePassword: TCheckBox
          Left = 8
          Top = 67
          Width = 156
          Height = 17
          Hint = #21246#36873#21518#27880#20876#36134#21495#20197#21450#20462#25913#23494#30721#26102#19981#20801#35768#36134#21495#21644#23494#30721#19968#26679#65292#13#10'IP'#29256#35831#21247#21246#36873#65292#38450#27490#32769#30331#38470#22120#36827#20837#27880#20876#36134#21495#26080#21709#24212#65281
          Caption = #31105#27490'ID'#23494#30721#30456#21516
          ParentShowHint = False
          ShowHint = True
          TabOrder = 3
          OnClick = chkDisableIDSamePasswordClick
        end
        object chkDisableQuizSameAnswer: TCheckBox
          Left = 8
          Top = 84
          Width = 156
          Height = 17
          Hint = #21246#36873#21518#27880#20876#36134#21495#26102#20505#23494#30721#20445#25252#38382#39064#21644#31572#26696#19981#33021#19968#26679#65292#13#10'IP'#29256#35831#21247#21246#36873#65292#38450#27490#32769#30331#38470#22120#36827#20837#27880#20876#36134#21495#26080#21709#24212#65281
          Caption = #31105#27490#38382#39064#31572#26696#30456#21516
          ParentShowHint = False
          ShowHint = True
          TabOrder = 4
          OnClick = chkDisableQuizSameAnswerClick
        end
        object CheckBoxGetbackPasswordCheckAll: TCheckBox
          Left = 8
          Top = 101
          Width = 156
          Height = 17
          Caption = #25214#22238#23494#30721#39035#23494#20445#23436#20840#27491#30830
          TabOrder = 5
          OnClick = CheckBoxGetbackPasswordCheckAllClick
        end
        object chkDisableIDSameL2Password: TCheckBox
          Left = 8
          Top = 119
          Width = 156
          Height = 17
          Caption = #31105#27490#20108#32423#23494#30721#21644'ID'#30456#21516
          TabOrder = 6
          OnClick = chkDisableIDSameL2PasswordClick
        end
        object chkDisableL2SamePassword: TCheckBox
          Left = 8
          Top = 136
          Width = 156
          Height = 17
          Caption = #31105#27490#20108#32423#23494#30721#21644#23494#30721#30456#21516
          TabOrder = 7
          OnClick = chkDisableL2SamePasswordClick
        end
      end
      object GroupBox2: TGroupBox
        Left = 6
        Top = 162
        Width = 168
        Height = 37
        Caption = #28165#29702#36134#21495#35774#32622
        TabOrder = 1
        object Label1: TLabel
          Left = 79
          Top = 16
          Width = 24
          Height = 12
          Caption = #38388#38548
        end
        object Label2: TLabel
          Left = 149
          Top = 16
          Width = 12
          Height = 12
          Caption = #31186
        end
        object CheckBoxAutoClear: TCheckBox
          Left = 8
          Top = 14
          Width = 73
          Height = 17
          Caption = #33258#21160#28165#29702
          TabOrder = 0
          OnClick = CheckBoxAutoClearClick
        end
        object SpinEditAutoClearTime: TSpinEdit
          Left = 106
          Top = 12
          Width = 43
          Height = 21
          MaxValue = 1000000
          MinValue = 1
          TabOrder = 1
          Value = 1
          OnChange = SpinEditAutoClearTimeChange
        end
      end
      object ButtonRestoreBasic: TButton
        Left = 6
        Top = 201
        Width = 91
        Height = 18
        Caption = #24674#22797#40664#35748#20540'(&D)'
        TabOrder = 2
        OnClick = ButtonRestoreBasicClick
      end
      object GroupBox8: TGroupBox
        Left = 179
        Top = 40
        Width = 210
        Height = 101
        Caption = #24320#21551#39564#35777#30721
        TabOrder = 3
        object Label11: TLabel
          Left = 7
          Top = 78
          Width = 54
          Height = 12
          Caption = #38169#35823#27425#25968':'
        end
        object Label16: TLabel
          Left = 108
          Top = 78
          Width = 54
          Height = 12
          Caption = #26356#25442#27425#25968':'
        end
        object lbl2: TLabel
          Left = 7
          Top = 55
          Width = 54
          Height = 12
          Caption = #30331#24405#25197#26354':'
        end
        object Label17: TLabel
          Left = 108
          Top = 55
          Width = 54
          Height = 12
          Caption = #20854#20182#25197#26354':'
        end
        object chkRandomCodeLogin: TCheckBox
          Left = 8
          Top = 16
          Width = 84
          Height = 17
          Hint = #24080#25143#30331#24405#38656#35201#39564#35777#30721
          Caption = #30331#24405#39564#35777#30721
          ParentShowHint = False
          ShowHint = True
          TabOrder = 0
          OnClick = chkRandomCodeLoginClick
        end
        object EditRandomCodeErrorMaxCount: TSpinEdit
          Left = 63
          Top = 74
          Width = 41
          Height = 21
          MaxValue = 10
          MinValue = 1
          TabOrder = 1
          Value = 3
          OnChange = EditRandomCodeErrorMaxCountChange
        end
        object seRandomCodeRefreshMaxCount: TSpinEdit
          Left = 164
          Top = 74
          Width = 41
          Height = 21
          MaxValue = 10
          MinValue = 1
          TabOrder = 2
          Value = 3
          OnChange = seRandomCodeRefreshMaxCountChange
        end
        object chkRandomCodePwdGetback: TCheckBox
          Left = 8
          Top = 34
          Width = 73
          Height = 17
          Hint = #23494#30721#25214#22238#38656#35201#39564#35777#30721
          Caption = #23494#30721#25214#22238
          ParentShowHint = False
          ShowHint = True
          TabOrder = 3
          OnClick = chkRandomCodePwdGetbackClick
        end
        object chkRandomCodePwdChange: TCheckBox
          Left = 120
          Top = 34
          Width = 73
          Height = 17
          Hint = #23494#30721#20462#25913#38656#35201#39564#35777#30721
          Caption = #23494#30721#20462#25913
          ParentShowHint = False
          ShowHint = True
          TabOrder = 4
          OnClick = chkRandomCodePwdChangeClick
        end
        object chkRandomCodeReg: TCheckBox
          Left = 120
          Top = 16
          Width = 84
          Height = 17
          Hint = #24080#25143#27880#20876#38656#35201#39564#35777#30721
          Caption = #27880#20876#39564#35777#30721
          ParentShowHint = False
          ShowHint = True
          TabOrder = 5
          OnClick = chkRandomCodeRegClick
        end
        object seLoginWaveValue: TSpinEdit
          Left = 63
          Top = 51
          Width = 41
          Height = 21
          Hint = #30331#24405#39564#35777#30721#25197#26354#31243#24230#65292#25968#23383#36234#22823#36234#19981#23481#26131#36776#35782
          MaxValue = 10
          MinValue = 1
          ParentShowHint = False
          ShowHint = True
          TabOrder = 6
          Value = 1
          OnChange = seLoginWaveValueChange
        end
        object seOtherWaveValue: TSpinEdit
          Left = 164
          Top = 51
          Width = 41
          Height = 21
          Hint = #20854#20182#39564#35777#30721#25197#26354#31243#24230#65292#25968#23383#36234#22823#36234#19981#23481#26131#36776#35782
          MaxValue = 6
          MinValue = 1
          ParentShowHint = False
          ShowHint = True
          TabOrder = 7
          Value = 1
          OnChange = seOtherWaveValueChange
        end
      end
      object grpL2Password: TGroupBox
        Left = 179
        Top = 144
        Width = 211
        Height = 73
        Caption = #20108#32423#23494#30721#26657#39564#36873#39033
        TabOrder = 4
        object chkChangedMACCheckL2: TCheckBox
          Left = 8
          Top = 33
          Width = 81
          Height = 17
          Caption = #26426#22120#30721#25913#21464
          TabOrder = 0
          OnClick = chkChangedMACCheckL2Click
        end
        object chkChangedIPCheckL2: TCheckBox
          Left = 112
          Top = 33
          Width = 56
          Height = 17
          Caption = 'IP'#25913#21464
          TabOrder = 1
          OnClick = chkChangedIPCheckL2Click
        end
        object chkAlwaysCheckL2: TCheckBox
          Left = 8
          Top = 51
          Width = 137
          Height = 17
          Caption = #27599#27425#36827#20837#37117#38656#35201#25928#39564
          TabOrder = 2
          OnClick = chkAlwaysCheckL2Click
        end
        object chkEnabledL2Password: TCheckBox
          Left = 8
          Top = 16
          Width = 139
          Height = 17
          Caption = #24320#25143#20108#32423#23494#30721#21151#33021
          TabOrder = 3
          OnClick = chkEnabledL2PasswordClick
        end
      end
      object GroupBox7: TGroupBox
        Left = 179
        Top = 3
        Width = 210
        Height = 37
        Caption = #33258#21160#35299#38500#38145#23450#36134#21495
        TabOrder = 5
        object Label9: TLabel
          Left = 59
          Top = 17
          Width = 48
          Height = 12
          Caption = #31561#24453#26102#38388
        end
        object Label10: TLabel
          Left = 167
          Top = 17
          Width = 12
          Height = 12
          Caption = #20998
        end
        object CheckBoxAutoUnLockAccount: TCheckBox
          Left = 8
          Top = 15
          Width = 49
          Height = 17
          Caption = #24320#21551
          TabOrder = 0
          OnClick = CheckBoxAutoUnLockAccountClick
        end
        object SpinEditUnLockAccountTime: TSpinEdit
          Left = 110
          Top = 13
          Width = 55
          Height = 21
          MaxValue = 1000000
          MinValue = 1
          TabOrder = 1
          Value = 1
          OnChange = SpinEditUnLockAccountTimeChange
        end
      end
      object GroupBox6: TGroupBox
        Left = 8
        Top = 224
        Width = 169
        Height = 73
        Caption = #26032#29256#30331#38470
        TabOrder = 6
        object chkNewLoginDlg: TCheckBox
          Left = 8
          Top = 16
          Width = 97
          Height = 17
          Hint = #23494#30721#25214#22238#38656#35201#39564#35777#30721
          Caption = #21551#29992#26032#29256#30331#38470
          ParentShowHint = False
          ShowHint = True
          TabOrder = 0
          OnClick = chkNewLoginDlgClick
        end
        object chkNewLoginInto: TCheckBox
          Left = 8
          Top = 32
          Width = 145
          Height = 17
          Hint = #23494#30721#25214#22238#38656#35201#39564#35777#30721
          Caption = #21019#24314#36134#25143#30452#25509#36827#20837#28216#25103
          ParentShowHint = False
          ShowHint = True
          TabOrder = 1
          OnClick = chkNewLoginIntoClick
        end
        object chkNewLoginPhone: TCheckBox
          Left = 8
          Top = 48
          Width = 105
          Height = 17
          Hint = #23494#30721#25214#22238#38656#35201#39564#35777#30721
          Caption = #25552#31034#32465#23450#25163#26426#21495
          ParentShowHint = False
          ShowHint = True
          TabOrder = 2
          OnClick = chkNewLoginPhoneClick
        end
      end
    end
    object TabSheet2: TTabSheet
      Caption = #32593#32476#35774#32622
      ImageIndex = 1
      object ButtonRestoreNet: TButton
        Left = 318
        Top = 159
        Width = 67
        Height = 25
        Caption = #40664#35748'(&D)'
        TabOrder = 0
        OnClick = ButtonRestoreNetClick
      end
      object GroupBox3: TGroupBox
        Left = 8
        Top = 4
        Width = 185
        Height = 65
        Caption = #32593#20851#35774#32622
        TabOrder = 1
        object Label3: TLabel
          Left = 8
          Top = 18
          Width = 54
          Height = 12
          Caption = #32465#23450#22320#22336':'
        end
        object Label4: TLabel
          Left = 8
          Top = 41
          Width = 54
          Height = 12
          Caption = #32593#20851#31471#21475':'
        end
        object EditGateAddr: TEdit
          Left = 72
          Top = 14
          Width = 105
          Height = 20
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          TabOrder = 0
          OnChange = EditGateAddrChange
        end
        object EditGatePort: TEdit
          Left = 72
          Top = 37
          Width = 57
          Height = 20
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          TabOrder = 1
          OnChange = EditGatePortChange
        end
      end
      object GroupBox4: TGroupBox
        Left = 200
        Top = 4
        Width = 185
        Height = 65
        Caption = #36828#31243#30417#25511#35774#32622
        TabOrder = 2
        object Label5: TLabel
          Left = 8
          Top = 18
          Width = 54
          Height = 12
          Caption = #32465#23450#22320#22336':'
        end
        object Label6: TLabel
          Left = 8
          Top = 41
          Width = 54
          Height = 12
          Caption = #32593#20851#31471#21475':'
        end
        object EditMonAddr: TEdit
          Left = 72
          Top = 14
          Width = 105
          Height = 20
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          TabOrder = 0
          OnChange = EditMonAddrChange
        end
        object EditMonPort: TEdit
          Left = 72
          Top = 37
          Width = 57
          Height = 20
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          TabOrder = 1
          OnChange = EditMonPortChange
        end
      end
      object GroupBox5: TGroupBox
        Left = 8
        Top = 75
        Width = 185
        Height = 65
        Caption = #26381#21153#22120#32593#32476#35774#32622
        TabOrder = 3
        object Label7: TLabel
          Left = 8
          Top = 18
          Width = 54
          Height = 12
          Caption = #32465#23450#22320#22336':'
        end
        object Label8: TLabel
          Left = 8
          Top = 41
          Width = 54
          Height = 12
          Caption = #20351#29992#31471#21475':'
        end
        object EditServerAddr: TEdit
          Left = 72
          Top = 14
          Width = 105
          Height = 20
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          TabOrder = 0
          OnChange = EditServerAddrChange
        end
        object EditServerPort: TEdit
          Left = 72
          Top = 37
          Width = 57
          Height = 20
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          TabOrder = 1
          OnChange = EditServerPortChange
        end
      end
      object CheckBoxDynamicIPMode: TCheckBox
        Left = 201
        Top = 78
        Width = 97
        Height = 17
        Caption = #21160#24577#22495#21517#27169#24335
        TabOrder = 4
        OnClick = CheckBoxDynamicIPModeClick
      end
      object chkShowBlockIPLog: TCheckBox
        Left = 8
        Top = 198
        Width = 169
        Height = 17
        Caption = #26174#31034#38750#27861#35831#27714#20869#37096#31471#21475#26085#24535
        TabOrder = 5
        OnClick = chkShowBlockIPLogClick
      end
    end
    object TabSheet3: TTabSheet
      Caption = #36828#31243#31649#29702#35774#32622
      ImageIndex = 2
      object lbl1: TLabel
        Left = 208
        Top = 79
        Width = 138
        Height = 12
        Caption = #31471#21475#20026'0'#20851#38381#36828#31243#31649#29702#21151#33021
        Font.Charset = GB2312_CHARSET
        Font.Color = clBlue
        Font.Height = -12
        Font.Name = #23435#20307
        Font.Style = []
        ParentFont = False
      end
      object Label14: TLabel
        Left = 208
        Top = 98
        Width = 180
        Height = 12
        Caption = #26080#36830#25509'IP'#38480#21046#26102#65292#21487#20197#20219#24847'IP'#36830#25509
        Font.Charset = GB2312_CHARSET
        Font.Color = clBlue
        Font.Height = -12
        Font.Name = #23435#20307
        Font.Style = []
        ParentFont = False
      end
      object GroupBox9: TGroupBox
        Left = 210
        Top = 7
        Width = 177
        Height = 65
        Caption = #36828#31243#31649#29702#35774#32622
        TabOrder = 0
        object Label12: TLabel
          Left = 8
          Top = 18
          Width = 54
          Height = 12
          Caption = #31649#29702#31471#21475':'
        end
        object Label13: TLabel
          Left = 8
          Top = 42
          Width = 54
          Height = 12
          Caption = #31649#29702#23494#30721':'
        end
        object edtControlPort: TEdit
          Left = 64
          Top = 14
          Width = 57
          Height = 20
          Hint = #35774#32622#20026'0'#34920#31034#19981#24320#21551
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ParentShowHint = False
          ShowHint = True
          TabOrder = 0
          OnChange = edtControlPortChange
        end
        object edtControlPassword: TEdit
          Left = 64
          Top = 38
          Width = 105
          Height = 20
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          MaxLength = 30
          TabOrder = 1
          OnChange = edtControlPasswordChange
        end
      end
      object grp1: TGroupBox
        Left = 6
        Top = 3
        Width = 195
        Height = 214
        Caption = #20801#35768#36830#25509'IP'
        TabOrder = 1
        object lstControlIPList: TListBox
          Left = 8
          Top = 16
          Width = 177
          Height = 191
          ItemHeight = 12
          PopupMenu = pmControlIPList
          TabOrder = 0
        end
      end
    end
    object ts1: TTabSheet
      Caption = #23494#30721#25193#23637#35774#32622
      ImageIndex = 3
      object Label15: TLabel
        Left = 8
        Top = 73
        Width = 180
        Height = 12
        Caption = #23494#30721#31105#27490#21253#21547#20197#19979#23383#31526'('#19968#34892#19968#20010')'
      end
      object chkDisablePwdSameChr: TCheckBox
        Left = 8
        Top = 8
        Width = 169
        Height = 17
        Caption = #31105#27490#23494#30721#20026#30456#21516#25968#23383#25110#23383#27597
        TabOrder = 0
        OnClick = chkDisablePwdSameChrClick
      end
      object chkDisablePwdAllNum: TCheckBox
        Left = 8
        Top = 29
        Width = 153
        Height = 17
        Caption = #31105#27490#23494#30721#20840#37096#26159#25968#23383
        TabOrder = 1
        OnClick = chkDisablePwdAllNumClick
      end
      object mmoDisablePassword: TMemo
        Left = 8
        Top = 90
        Width = 233
        Height = 124
        ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
        ScrollBars = ssBoth
        TabOrder = 2
        OnChange = mmoDisablePasswordChange
      end
      object chkDisablePwdAllLetter: TCheckBox
        Left = 8
        Top = 50
        Width = 153
        Height = 17
        Caption = #31105#27490#23494#30721#20840#37096#26159#23383#27597
        TabOrder = 3
        OnClick = chkDisablePwdAllLetterClick
      end
    end
  end
  object ButtonSave: TButton
    Left = 248
    Top = 341
    Width = 75
    Height = 25
    Caption = #20445#23384'(&S)'
    TabOrder = 1
    OnClick = ButtonSaveClick
  end
  object ButtonClose: TButton
    Left = 334
    Top = 341
    Width = 75
    Height = 25
    Caption = #30830#23450'(&O)'
    TabOrder = 2
    OnClick = ButtonCloseClick
  end
  object pmControlIPList: TPopupMenu
    Left = 96
    Top = 8
    object mniIPAdd: TMenuItem
      Caption = #22686#21152'(&A)'
      OnClick = mniIPAddClick
    end
    object mniIPDelete: TMenuItem
      Caption = #21024#38500'(&D)'
      OnClick = mniIPDeleteClick
    end
    object mniIPClear: TMenuItem
      Caption = #28165#31354'(&C)'
      OnClick = mniIPClearClick
    end
  end
end
