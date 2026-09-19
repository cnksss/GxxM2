object frmMain: TfrmMain
  Left = 629
  Top = 161
  BorderStyle = bsSingle
  BorderWidth = 5
  Caption = #24341#25806#25511#21046#21488
  ClientHeight = 452
  ClientWidth = 674
  Color = clBtnFace
  Font.Charset = ANSI_CHARSET
  Font.Color = clWindowText
  Font.Height = -12
  Font.Name = #23435#20307
  Font.Style = []
  OldCreateOrder = False
  Position = poScreenCenter
  ShowHint = True
  OnCloseQuery = FormCloseQuery
  OnCreate = FormCreate
  OnDestroy = FormDestroy
  PixelsPerInch = 96
  TextHeight = 12
  object PageControl1: TPageControl
    Left = 0
    Top = 0
    Width = 674
    Height = 452
    ActivePage = TabSheet1
    Align = alClient
    HotTrack = True
    TabOrder = 0
    object TabSheet1: TTabSheet
      BorderWidth = 5
      Caption = #26381#21153#22120#25511#21046
      object GroupBox5: TGroupBox
        Left = 0
        Top = 0
        Width = 656
        Height = 414
        Align = alClient
        Caption = #26381#21153#22120#25511#21046
        TabOrder = 0
        object ButtonStartGame: TButton
          Left = 256
          Top = 377
          Width = 145
          Height = 33
          Caption = #21551#21160#28216#25103#25511#21046#22120'(&S)'
          TabOrder = 0
          OnClick = ButtonStartGameClick
        end
        object CheckBoxM2Server: TCheckBox
          Left = 12
          Top = 41
          Width = 170
          Height = 17
          Caption = #28216#25103#20027#31243#24207'(M2Server)'
          TabOrder = 1
          OnClick = CheckBoxM2ServerClick
        end
        object CheckBoxDBServer: TCheckBox
          Left = 12
          Top = 20
          Width = 170
          Height = 17
          Caption = #28216#25103#25968#25454#24211'(DBServer)'
          TabOrder = 2
          OnClick = CheckBoxDBServerClick
        end
        object CheckBoxLoginServer: TCheckBox
          Left = 12
          Top = 83
          Width = 170
          Height = 17
          Caption = #28216#25103#30331#38470#26381#21153#22120'(LoginSrv)'
          TabOrder = 3
          OnClick = CheckBoxLoginServerClick
        end
        object CheckBoxLogServer: TCheckBox
          Left = 476
          Top = 104
          Width = 170
          Height = 17
          Caption = #28216#25103#26085#24535#26381#21153'(LogServer)'
          TabOrder = 4
          OnClick = CheckBoxLogServerClick
        end
        object CheckBoxLoginGate: TCheckBox
          Left = 12
          Top = 62
          Width = 170
          Height = 17
          Caption = #28216#25103#30331#38470#32593#20851'(LoginGate)'
          TabOrder = 5
          OnClick = CheckBoxLoginGateClick
        end
        object CheckBoxSelGate: TCheckBox
          Left = 12
          Top = 104
          Width = 170
          Height = 17
          Caption = #28216#25103#35282#33394#32593#20851#19968'(SelGate)'
          TabOrder = 6
          OnClick = CheckBoxSelGateClick
        end
        object CheckBoxRunGate: TCheckBox
          Left = 252
          Top = 20
          Width = 170
          Height = 17
          Caption = #28216#25103#32593#20851#19968'(Rungate)'
          TabOrder = 7
          OnClick = CheckBoxRunGateClick
        end
        object CheckBoxRunGate1: TCheckBox
          Tag = 1
          Left = 476
          Top = 20
          Width = 170
          Height = 17
          Caption = #28216#25103#32593#20851#20108'(Rungate)'
          TabOrder = 8
          OnClick = CheckBoxRunGateClick
        end
        object CheckBoxRunGate2: TCheckBox
          Tag = 2
          Left = 252
          Top = 41
          Width = 170
          Height = 17
          Caption = #28216#25103#32593#20851#19977'(Rungate)'
          TabOrder = 9
          OnClick = CheckBoxRunGateClick
        end
        object MemoLog: TMemo
          Left = 12
          Top = 179
          Width = 634
          Height = 193
          Color = clNone
          Font.Charset = ANSI_CHARSET
          Font.Color = clLime
          Font.Height = -12
          Font.Name = #23435#20307
          Font.Style = []
          ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
          ParentFont = False
          TabOrder = 10
          OnChange = MemoLogChange
        end
        object CheckBoxRunGate3: TCheckBox
          Tag = 3
          Left = 476
          Top = 41
          Width = 170
          Height = 17
          Caption = #28216#25103#32593#20851#22235'(Rungate)'
          TabOrder = 11
          OnClick = CheckBoxRunGateClick
        end
        object CheckBoxRunGate4: TCheckBox
          Tag = 4
          Left = 252
          Top = 62
          Width = 170
          Height = 17
          Caption = #28216#25103#32593#20851#20116'(Rungate)'
          TabOrder = 12
          OnClick = CheckBoxRunGateClick
        end
        object CheckBoxRunGate5: TCheckBox
          Tag = 5
          Left = 476
          Top = 62
          Width = 170
          Height = 17
          Caption = #28216#25103#32593#20851#20845'(Rungate)'
          TabOrder = 13
          OnClick = CheckBoxRunGateClick
        end
        object CheckBoxRunGate6: TCheckBox
          Tag = 6
          Left = 252
          Top = 83
          Width = 170
          Height = 17
          Caption = #28216#25103#32593#20851#19971'(Rungate)'
          TabOrder = 14
          OnClick = CheckBoxRunGateClick
        end
        object CheckBoxRunGate7: TCheckBox
          Tag = 7
          Left = 476
          Top = 83
          Width = 170
          Height = 17
          Caption = #28216#25103#32593#20851#20843'(Rungate)'
          TabOrder = 15
          OnClick = CheckBoxRunGateClick
        end
        object CheckBoxSelGate1: TCheckBox
          Left = 252
          Top = 104
          Width = 170
          Height = 17
          Caption = #28216#25103#35282#33394#32593#20851#20108'(SelGate)'
          TabOrder = 16
          OnClick = CheckBoxSelGate1Click
        end
        object grp6: TGroupBox
          Left = 12
          Top = 128
          Width = 633
          Height = 43
          Caption = #21551#21160#36873#39033
          TabOrder = 17
          object chkTimerStart: TCheckBox
            Left = 12
            Top = 17
            Width = 77
            Height = 17
            Caption = #23450#26102#21551#21160':'
            TabOrder = 0
            OnClick = chkTimerStartClick
          end
          object chkEmbeddedWindow: TCheckBox
            Left = 301
            Top = 17
            Width = 161
            Height = 17
            Caption = #31383#21475#23884#20837#21040#25511#21046#21488#20013#26174#31034
            TabOrder = 1
            OnClick = chkEmbeddedWindowClick
          end
          object dtpDate: TDateTimePicker
            Left = 85
            Top = 15
            Width = 92
            Height = 20
            Date = 45106.768274583330000000
            Time = 45106.768274583330000000
            TabOrder = 2
          end
          object dtpTime: TDateTimePicker
            Left = 180
            Top = 15
            Width = 77
            Height = 20
            Date = 45106.768563680550000000
            Time = 45106.768563680550000000
            DateMode = dmUpDown
            Kind = dtkTime
            TabOrder = 3
          end
        end
        object btnSetPath: TButton
          Left = 512
          Top = 381
          Width = 134
          Height = 24
          Caption = #33258#21160#35774#32622#26381#21153#22120#30446#24405
          TabOrder = 18
          OnClick = btnSetPathClick
        end
        object Button1: TButton
          Left = 14
          Top = 382
          Width = 81
          Height = 23
          Caption = #24555#25463#20445#23384
          TabOrder = 19
          OnClick = Button1Click
        end
        object Button2: TButton
          Left = 102
          Top = 382
          Width = 107
          Height = 23
          Caption = #24555#25463#21021#22987#21270'Mysql'
          TabOrder = 20
          OnClick = lblMySqlDoInitClick
        end
      end
    end
    object TabSheet15: TTabSheet
      BorderWidth = 5
      Caption = #24080#21495
      ImageIndex = 5
      ExplicitLeft = 0
      ExplicitTop = 0
      ExplicitWidth = 0
      ExplicitHeight = 0
      object GroupBox26: TGroupBox
        Left = 0
        Top = 32
        Width = 656
        Height = 382
        Align = alClient
        Caption = #24080#21495#20449#24687
        TabOrder = 0
        object Label31: TLabel
          Left = 16
          Top = 23
          Width = 54
          Height = 12
          Alignment = taRightJustify
          Caption = #30331#24405#24080#21495':'
        end
        object Label32: TLabel
          Left = 16
          Top = 47
          Width = 54
          Height = 12
          Alignment = taRightJustify
          Caption = #30331#24405#23494#30721':'
        end
        object Label33: TLabel
          Left = 16
          Top = 95
          Width = 54
          Height = 12
          Alignment = taRightJustify
          Caption = #29992#25143#21517#31216':'
        end
        object Label34: TLabel
          Left = 16
          Top = 167
          Width = 54
          Height = 12
          Alignment = taRightJustify
          Caption = #36523#20221#35777#21495':'
        end
        object Label35: TLabel
          Left = 284
          Top = 23
          Width = 42
          Height = 12
          Alignment = taRightJustify
          Caption = #29983'  '#26085':'
        end
        object Label36: TLabel
          Left = 284
          Top = 47
          Width = 42
          Height = 12
          Alignment = taRightJustify
          Caption = #38382#39064#19968':'
        end
        object Label37: TLabel
          Left = 284
          Top = 71
          Width = 42
          Height = 12
          Alignment = taRightJustify
          Caption = #31572#26696#19968':'
        end
        object Label38: TLabel
          Left = 284
          Top = 95
          Width = 42
          Height = 12
          Alignment = taRightJustify
          Caption = #38382#39064#20108':'
        end
        object Label39: TLabel
          Left = 284
          Top = 119
          Width = 42
          Height = 12
          Alignment = taRightJustify
          Caption = #31572#26696#20108':'
        end
        object Label40: TLabel
          Left = 16
          Top = 143
          Width = 54
          Height = 12
          Alignment = taRightJustify
          Caption = #31227#21160#30005#35805':'
        end
        object Label41: TLabel
          Left = 272
          Top = 167
          Width = 54
          Height = 12
          Alignment = taRightJustify
          Caption = #22791#27880#20449#24687':'
        end
        object Label42: TLabel
          Left = 16
          Top = 71
          Width = 54
          Height = 12
          Alignment = taRightJustify
          Caption = #20108#32423#23494#30721':'
        end
        object Label43: TLabel
          Left = 272
          Top = 143
          Width = 54
          Height = 12
          Alignment = taRightJustify
          Caption = #30005#23376#37038#31665':'
        end
        object Label44: TLabel
          Left = 16
          Top = 119
          Width = 54
          Height = 12
          Alignment = taRightJustify
          Caption = #30005#35805#21495#30721':'
        end
        object edtLoginAccount: TEdit
          Left = 71
          Top = 19
          Width = 160
          Height = 20
          Enabled = False
          ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
          MaxLength = 10
          TabOrder = 0
          OnChange = edtLoginAccountChange
        end
        object edtLoginAccountPasswd: TEdit
          Left = 71
          Top = 43
          Width = 160
          Height = 20
          ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
          MaxLength = 10
          TabOrder = 1
          OnChange = edtLoginAccountChange
        end
        object edtLoginAccountUserName: TEdit
          Left = 71
          Top = 91
          Width = 160
          Height = 20
          ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
          MaxLength = 20
          TabOrder = 2
          OnChange = edtLoginAccountChange
        end
        object edtLoginAccountSSNo: TEdit
          Left = 71
          Top = 163
          Width = 159
          Height = 20
          ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
          MaxLength = 14
          TabOrder = 3
          OnChange = edtLoginAccountChange
        end
        object edtLoginAccountBirthDay: TEdit
          Left = 327
          Top = 19
          Width = 160
          Height = 20
          ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
          MaxLength = 10
          TabOrder = 4
          OnChange = edtLoginAccountChange
        end
        object edtLoginAccountQuiz: TEdit
          Left = 327
          Top = 43
          Width = 160
          Height = 20
          ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
          MaxLength = 20
          TabOrder = 5
          OnChange = edtLoginAccountChange
        end
        object edtLoginAccountAnswer: TEdit
          Left = 327
          Top = 67
          Width = 160
          Height = 20
          ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
          MaxLength = 12
          TabOrder = 6
          OnChange = edtLoginAccountChange
        end
        object edtLoginAccountQuiz2: TEdit
          Left = 327
          Top = 91
          Width = 160
          Height = 20
          ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
          MaxLength = 20
          TabOrder = 7
          OnChange = edtLoginAccountChange
        end
        object edtLoginAccountAnswer2: TEdit
          Left = 327
          Top = 115
          Width = 160
          Height = 20
          ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
          MaxLength = 12
          TabOrder = 8
          OnChange = edtLoginAccountChange
        end
        object edtLoginAccountMobilePhone: TEdit
          Left = 71
          Top = 139
          Width = 160
          Height = 20
          ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
          MaxLength = 13
          TabOrder = 9
          OnChange = edtLoginAccountChange
        end
        object edtLoginAccountMemo: TEdit
          Left = 327
          Top = 163
          Width = 160
          Height = 20
          ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
          MaxLength = 20
          TabOrder = 10
          OnChange = edtLoginAccountChange
        end
        object edtLoginAccountEMail: TEdit
          Left = 327
          Top = 139
          Width = 160
          Height = 20
          ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
          MaxLength = 40
          TabOrder = 11
          OnChange = edtLoginAccountChange
        end
        object edtLoginAccountMemo2: TEdit
          Left = 71
          Top = 67
          Width = 160
          Height = 20
          ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
          MaxLength = 20
          TabOrder = 12
          OnChange = edtLoginAccountChange
        end
        object chkFullEditMode: TCheckBox
          Left = 501
          Top = 20
          Width = 93
          Height = 17
          Caption = #20462#25913#24080#21495#20449#24687
          TabOrder = 13
          OnClick = chkFullEditModeClick
        end
        object ButtonLoginAccountOK: TButton
          Left = 422
          Top = 189
          Width = 65
          Height = 25
          Caption = #30830#23450'(&O)'
          Enabled = False
          TabOrder = 14
          OnClick = ButtonLoginAccountOKClick
        end
        object edtLoginAccountPhone: TEdit
          Left = 71
          Top = 115
          Width = 160
          Height = 20
          ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
          MaxLength = 13
          TabOrder = 15
          OnChange = edtLoginAccountChange
        end
      end
      object Panel1: TPanel
        Left = 0
        Top = 0
        Width = 656
        Height = 32
        Align = alTop
        BevelOuter = bvNone
        ParentColor = True
        TabOrder = 1
        object Label30: TLabel
          Left = 9
          Top = 7
          Width = 78
          Height = 12
          Caption = #30331#24405#24080#21495#25628#32034':'
        end
        object edtSearchLoginAccount: TEdit
          Left = 89
          Top = 3
          Width = 105
          Height = 20
          ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
          TabOrder = 0
        end
        object ButtonSearchLoginAccount: TButton
          Left = 201
          Top = 0
          Width = 65
          Height = 25
          Caption = #25628#32034'(&S)'
          TabOrder = 1
          OnClick = ButtonSearchLoginAccountClick
        end
      end
    end
    object TabSheet2: TTabSheet
      Caption = #37197#32622#21521#23548
      ImageIndex = 1
      object PageControl2: TPageControl
        Left = 480
        Top = 144
        Width = 289
        Height = 193
        TabOrder = 0
      end
      object PageControl3: TPageControl
        Left = 0
        Top = 0
        Width = 666
        Height = 424
        ActivePage = TabSheet11
        Align = alClient
        TabOrder = 1
        TabPosition = tpBottom
        object TabSheet4: TTabSheet
          BorderWidth = 5
          Caption = '1-'#22522#26412#35774#32622
          ExplicitLeft = 0
          ExplicitTop = 0
          ExplicitWidth = 0
          ExplicitHeight = 0
          object GroupBox1: TGroupBox
            Left = 0
            Top = 0
            Width = 648
            Height = 305
            Align = alTop
            Caption = #31243#24207#30446#24405#21450#29289#21697#25968#25454#24211#35774#32622
            TabOrder = 0
            object Label1: TLabel
              Left = 11
              Top = 26
              Width = 114
              Height = 12
              Caption = #28216#25103#26381#21153#31471#25152#22312#30446#24405':'
            end
            object Label3: TLabel
              Left = 11
              Top = 205
              Width = 126
              Height = 12
              Caption = #28216#25103#26381#21153#22120#26381#21153#22120#21517#31216':'
            end
            object Label4: TLabel
              Left = 11
              Top = 229
              Width = 126
              Height = 12
              Caption = #28216#25103#26381#21153#22120#22806#32593'IP'#22320#22336':'
            end
            object lbl7: TLabel
              Left = 192
              Top = 282
              Width = 30
              Height = 12
              Caption = #24310#26102':'
            end
            object lbl8: TLabel
              Left = 280
              Top = 282
              Width = 12
              Height = 12
              Caption = #31186
            end
            object LabelNetComIPaddr: TLabel
              Left = 65
              Top = 253
              Width = 72
              Height = 12
              Caption = #22806#32593'IP'#22320#22336'2:'
              Visible = False
            end
            object EditGameDir: TEdit
              Left = 127
              Top = 22
              Width = 512
              Height = 20
              Hint = #36755#20837#26381#21153#22120#25152#22312#30446#24405#12290#19968#33324#40664#35748#20026#8220'D:\MirServer\'#8221#12290
              ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
              TabOrder = 0
              Text = 'D:\MirServer\'
            end
            object EditGameName: TEdit
              Left = 139
              Top = 201
              Width = 323
              Height = 20
              Hint = #36755#20837#28216#25103#30340#21517#31216#12290
              ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
              TabOrder = 1
              Text = 'GxxM2'
            end
            object EditGameExtIPaddr: TEdit
              Left = 139
              Top = 225
              Width = 170
              Height = 20
              Hint = #36755#20837#26381#21153#22120#30340#22806#32593'IP'#22320#22336#12290
              ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
              TabOrder = 2
              Text = '127.0.0.1'
            end
            object chkDoubleLineMode: TCheckBox
              Left = 315
              Top = 227
              Width = 97
              Height = 17
              Caption = #21452'IP'#19968#21306#27169#24335
              TabOrder = 3
              OnClick = chkDoubleLineModeClick
            end
            object ButtonGeneralDefalult: TButton
              Left = 548
              Top = 274
              Width = 90
              Height = 25
              Caption = #40664#35748#35774#32622'(&D)'
              TabOrder = 4
              OnClick = ButtonGeneralDefalultClick
            end
            object chkAutoStartServer: TCheckBox
              Left = 8
              Top = 280
              Width = 177
              Height = 17
              Caption = #25511#21046#22120#21551#21160#21518#33258#21160#21551#21160#26381#21153#31471
              TabOrder = 5
              OnClick = chkAutoStartServerClick
            end
            object EditAutoStartDelayTime: TSpinEdit
              Left = 224
              Top = 278
              Width = 49
              Height = 21
              MaxValue = 0
              MinValue = 0
              TabOrder = 6
              Value = 0
              OnChange = EditAutoStartDelayTimeChange
            end
            object EditGameExtNetComIPaddr: TEdit
              Left = 139
              Top = 249
              Width = 170
              Height = 20
              Hint = #36755#20837#26381#21153#22120#30340#22806#32593'IP'#22320#22336#12290
              ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
              TabOrder = 7
              Text = '127.0.0.1'
              Visible = False
            end
            object chkDynamicIPMode: TCheckBox
              Left = 315
              Top = 251
              Width = 81
              Height = 17
              Caption = #21160#24577'IP'#22320#22336
              TabOrder = 8
              OnClick = chkDynamicIPModeClick
            end
            object grp7: TGroupBox
              Left = 11
              Top = 48
              Width = 628
              Height = 48
              Caption = #29289#21697'/'#25216#33021'/'#24618#29289#25968#25454#24211#35774#32622' [ **'#37325#21551#29983#25928'** ]'
              TabOrder = 10
              object EditHeroDB: TEdit
                Left = 91
                Top = 19
                Width = 78
                Height = 20
                Hint = #26381#21153#22120#31471'BDE '#25968#25454#24211#21517#31216#65292#40664#35748#20026' '#8220'HeroDB'#8221#12290
                ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
                TabOrder = 0
                Text = 'HeroDB'
              end
              object rbBDE: TRadioButton
                Left = 8
                Top = 20
                Width = 81
                Height = 17
                Caption = 'BDE'#25968#25454#24211':'
                TabOrder = 1
                OnClick = rbBDEClick
              end
              object rbSqlite: TRadioButton
                Left = 192
                Top = 20
                Width = 97
                Height = 17
                Caption = 'Sqlite'#25968#25454#24211':'
                TabOrder = 2
                OnClick = rbSqliteClick
              end
              object edtSqliteDB: TRzButtonEdit
                Left = 288
                Top = 19
                Width = 333
                Height = 20
                TabOrder = 3
                AltBtnWidth = 15
                ButtonWidth = 15
                OnButtonClick = edtSqliteDBButtonClick
              end
            end
            object grp8: TGroupBox
              Left = 11
              Top = 101
              Width = 628
              Height = 92
              Caption = #24080#21495'/'#35282#33394'/'#20010#20154#21830#24215#25968#25454#24211#35774#32622'  [ **'#37325#21551#29983#25928'** ]'
              TabOrder = 9
              object rbDataSaveSqlite: TRadioButton
                Left = 8
                Top = 27
                Width = 97
                Height = 17
                Caption = 'Sqlite'#25968#25454#24211
                TabOrder = 0
                OnClick = rbDataSaveSqliteClick
              end
              object grpDataSaveMySql: TGroupBox
                Left = 140
                Top = 14
                Width = 481
                Height = 72
                Caption = 'MySql'#25968#25454#24211#37197#32622
                TabOrder = 1
                object lblSrcDBPort: TLabel
                  Left = 332
                  Top = 22
                  Width = 72
                  Height = 12
                  Caption = #25968#25454#24211#31471#21475#65306
                end
                object lblSrcDBServer: TLabel
                  Left = 8
                  Top = 22
                  Width = 72
                  Height = 12
                  Caption = #26381#21153#22120#22320#22336#65306
                end
                object lblSrcDBUser: TLabel
                  Left = 20
                  Top = 49
                  Width = 60
                  Height = 12
                  Caption = #29992#25143#21517#31216#65306
                end
                object lblSrcDBPassword: TLabel
                  Left = 182
                  Top = 49
                  Width = 36
                  Height = 12
                  Caption = #23494#30721#65306
                end
                object Label28: TLabel
                  Left = 332
                  Top = 49
                  Width = 72
                  Height = 12
                  Caption = #25968#25454#24211#21517#31216#65306
                end
                object lblMySqlLinkTest: TLabel
                  Left = 128
                  Top = 0
                  Width = 48
                  Height = 12
                  Cursor = crHandPoint
                  Caption = #36830#25509#27979#35797
                  Font.Charset = ANSI_CHARSET
                  Font.Color = clBlue
                  Font.Height = -12
                  Font.Name = #23435#20307
                  Font.Style = []
                  ParentFont = False
                  OnClick = lblMySqlLinkTestClick
                end
                object lblMySqlDoInit: TLabel
                  Left = 192
                  Top = 0
                  Width = 72
                  Height = 12
                  Cursor = crHandPoint
                  Caption = #25968#25454#24211#21021#22987#21270
                  Font.Charset = ANSI_CHARSET
                  Font.Color = clBlue
                  Font.Height = -12
                  Font.Name = #23435#20307
                  Font.Style = []
                  ParentFont = False
                  OnClick = lblMySqlDoInitClick
                end
                object edtDataSaveDBServer: TEdit
                  Left = 78
                  Top = 18
                  Width = 233
                  Height = 20
                  TabOrder = 0
                end
                object seDataSaveDBPort: TSpinEditEx
                  Left = 402
                  Top = 17
                  Width = 73
                  Height = 21
                  MaxValue = 65535
                  MinValue = 1
                  TabOrder = 1
                  Value = 3306
                end
                object edtDataSaveDBUser: TEdit
                  Left = 78
                  Top = 44
                  Width = 96
                  Height = 20
                  Ctl3D = True
                  ParentCtl3D = False
                  TabOrder = 2
                end
                object edtDataSaveDBPassword: TEdit
                  Left = 215
                  Top = 44
                  Width = 96
                  Height = 20
                  Ctl3D = True
                  ParentCtl3D = False
                  TabOrder = 3
                end
                object edtDataSaveDataBase: TEdit
                  Left = 402
                  Top = 45
                  Width = 72
                  Height = 20
                  TabOrder = 4
                end
              end
              object rbDataSaveMySql: TRadioButton
                Left = 8
                Top = 56
                Width = 97
                Height = 17
                Caption = 'MySql'#25968#25454#24211
                TabOrder = 2
                OnClick = rbDataSaveSqliteClick
              end
            end
          end
          object ButtonNext1: TButton
            Left = 566
            Top = 354
            Width = 81
            Height = 33
            Caption = #19979#19968#27493'(&N)'
            TabOrder = 1
            OnClick = ButtonNext1Click
          end
          object ButtonReLoadConfig: TButton
            Left = 424
            Top = 354
            Width = 135
            Height = 33
            Caption = #37325#26032#21152#36733#26368#26032#37197#32622'(&R)'
            TabOrder = 2
            OnClick = ButtonReLoadConfigClick
          end
          object grp5: TGroupBox
            Left = 0
            Top = 309
            Width = 392
            Height = 73
            Caption = #25209#37327#20462#25913#31471#21475#21495
            TabOrder = 3
            object lbl15: TLabel
              Left = 8
              Top = 24
              Width = 182
              Height = 12
              Caption = #25152#26377#31471#21475#21495#22312#24403#21069#22522#30784#19978#22686#21152#65306
              Font.Charset = ANSI_CHARSET
              Font.Color = clWindowText
              Font.Height = -12
              Font.Name = #23435#20307
              Font.Style = [fsBold]
              ParentFont = False
            end
            object lbl16: TLabel
              Left = 113
              Top = 48
              Width = 264
              Height = 12
              Caption = #35828#26126#65306#25209#37327#20462#25913#25152#26377#31471#21475#21495#21518#65292#37325#26032#29983#25104#37197#32622#29983#25928
              Font.Charset = ANSI_CHARSET
              Font.Color = clBlue
              Font.Height = -12
              Font.Name = #23435#20307
              Font.Style = []
              ParentFont = False
            end
            object sePortInc: TSpinEdit
              Left = 186
              Top = 19
              Width = 71
              Height = 21
              MaxValue = 0
              MinValue = 0
              ParentShowHint = False
              ShowHint = True
              TabOrder = 0
              Value = 0
              OnChange = EditLoginGate_MainFormYChange
            end
            object btn2: TButton
              Left = 264
              Top = 16
              Width = 113
              Height = 25
              Caption = #25209#37327#20462#25913
              TabOrder = 1
              OnClick = btn2Click
            end
          end
        end
        object TabSheet5: TTabSheet
          BorderWidth = 5
          Caption = '2-'#30331#24405#32593#20851
          ImageIndex = 1
          ExplicitLeft = 0
          ExplicitTop = 0
          ExplicitWidth = 0
          ExplicitHeight = 0
          object ButtonNext2: TButton
            Left = 566
            Top = 354
            Width = 81
            Height = 33
            Caption = #19979#19968#27493'(&N)'
            TabOrder = 0
            OnClick = ButtonNext2Click
          end
          object GroupBox2: TGroupBox
            Left = 0
            Top = 0
            Width = 648
            Height = 95
            Align = alTop
            Caption = #30331#38470#32593#20851#35774#32622
            TabOrder = 1
            object lbl17: TLabel
              Left = 11
              Top = 23
              Width = 72
              Height = 12
              Caption = #26381#21153#22120#31471#21475#65306
            end
            object Label9: TLabel
              Left = 11
              Top = 47
              Width = 66
              Height = 12
              Caption = #31383#21475#24231#26631'X'#65306
            end
            object Label10: TLabel
              Left = 11
              Top = 72
              Width = 66
              Height = 12
              Caption = #31383#21475#24231#26631'Y'#65306
            end
            object ButtonLoginGateDefault: TButton
              Left = 312
              Top = 62
              Width = 90
              Height = 25
              Caption = #40664#35748#35774#32622'(&D)'
              TabOrder = 0
              OnClick = ButtonLoginGateDefaultClick
            end
            object GroupBox27: TGroupBox
              Left = 159
              Top = 14
              Width = 140
              Height = 73
              Caption = #21551#21160#36873#39033
              TabOrder = 1
              object CheckBoxboLoginGate_GetStart: TCheckBox
                Left = 8
                Top = 21
                Width = 89
                Height = 17
                Caption = #21551#21160#30331#24405#32593#20851
                TabOrder = 0
                OnClick = CheckBoxboLoginGate_GetStartClick
              end
              object CheckBoxboLoginGate_GetMinimize: TCheckBox
                Left = 8
                Top = 42
                Width = 113
                Height = 17
                Caption = #21551#21160#25104#21151#21518#26368#23567#21270
                TabOrder = 1
                OnClick = CheckBoxboLoginGate_GetMinimizeClick
              end
            end
            object EditLoginGate_GatePort: TEdit
              Left = 83
              Top = 19
              Width = 60
              Height = 20
              ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
              TabOrder = 2
              Text = '7000'
            end
            object EditLoginGate_MainFormX: TSpinEdit
              Left = 83
              Top = 43
              Width = 60
              Height = 21
              Hint = #21551#21160#31243#24207#31383#21475#22312#23631#24149#19978#30340#20301#32622#65292#24231#26631'X'#12290
              MaxValue = 10000
              MinValue = 0
              TabOrder = 3
              Value = 0
              OnChange = EditLoginGate_MainFormXChange
            end
            object EditLoginGate_MainFormY: TSpinEdit
              Left = 83
              Top = 68
              Width = 60
              Height = 21
              Hint = #21551#21160#31243#24207#31383#21475#22312#23631#24149#19978#30340#20301#32622#65292#24231#26631'Y'#12290
              MaxValue = 10000
              MinValue = 0
              TabOrder = 4
              Value = 0
              OnChange = EditLoginGate_MainFormYChange
            end
          end
          object ButtonPrv2: TButton
            Left = 478
            Top = 354
            Width = 81
            Height = 33
            Caption = #19978#19968#27493'(&P)'
            TabOrder = 2
            OnClick = ButtonPrv2Click
          end
        end
        object TabSheet6: TTabSheet
          BorderWidth = 5
          Caption = '3-'#35282#33394#32593#20851
          ImageIndex = 2
          ExplicitLeft = 0
          ExplicitTop = 0
          ExplicitWidth = 0
          ExplicitHeight = 0
          object GroupBox3: TGroupBox
            Left = 0
            Top = 0
            Width = 648
            Height = 105
            Align = alTop
            Caption = #35282#33394#32593#20851#35774#32622
            TabOrder = 0
            object GroupBox8: TGroupBox
              Left = 8
              Top = 19
              Width = 129
              Height = 73
              Caption = #31383#21475#20301#32622
              TabOrder = 0
              object Label11: TLabel
                Left = 8
                Top = 20
                Width = 36
                Height = 12
                Caption = #24231#26631'X:'
              end
              object Label12: TLabel
                Left = 8
                Top = 45
                Width = 36
                Height = 12
                Caption = #24231#26631'Y:'
              end
              object EditSelGate_MainFormX: TSpinEdit
                Left = 48
                Top = 16
                Width = 65
                Height = 21
                Hint = #21551#21160#31243#24207#31383#21475#22312#23631#24149#19978#30340#20301#32622#65292#24231#26631'X'#12290
                MaxValue = 10000
                MinValue = 0
                TabOrder = 0
                Value = 0
                OnChange = EditSelGate_MainFormXChange
              end
              object EditSelGate_MainFormY: TSpinEdit
                Left = 48
                Top = 41
                Width = 65
                Height = 21
                Hint = #21551#21160#31243#24207#31383#21475#22312#23631#24149#19978#30340#20301#32622#65292#24231#26631'Y'#12290
                MaxValue = 10000
                MinValue = 0
                TabOrder = 1
                Value = 0
                OnChange = EditSelGate_MainFormYChange
              end
            end
            object ButtonSelGateDefault: TButton
              Left = 560
              Top = 70
              Width = 81
              Height = 25
              Caption = #40664#35748#35774#32622'(&D)'
              TabOrder = 1
              OnClick = ButtonSelGateDefaultClick
            end
            object GroupBox24: TGroupBox
              Left = 144
              Top = 19
              Width = 129
              Height = 73
              Caption = #26381#21153#22120#31471#21475
              TabOrder = 2
              object Label29: TLabel
                Left = 11
                Top = 20
                Width = 30
                Height = 12
                Caption = #31471#21475':'
              end
              object Label49: TLabel
                Left = 11
                Top = 44
                Width = 30
                Height = 12
                Caption = #31471#21475':'
              end
              object EditSelGate_GatePort: TEdit
                Left = 42
                Top = 16
                Width = 75
                Height = 20
                ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
                TabOrder = 0
                Text = '7100'
              end
              object EditSelGate_GatePort1: TEdit
                Left = 42
                Top = 40
                Width = 75
                Height = 20
                ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
                TabOrder = 1
                Text = '7100'
              end
            end
            object GroupBoxSelGate_GetStart: TGroupBox
              Left = 280
              Top = 19
              Width = 129
              Height = 73
              Caption = #26159#21542#21551#21160
              TabOrder = 3
              object CheckBoxboSelGate_GetStart: TCheckBox
                Left = 8
                Top = 16
                Width = 105
                Height = 17
                Caption = #21551#21160#35282#33394#32593#20851#19968
                TabOrder = 0
                OnClick = CheckBoxboSelGate_GetStartClick
              end
              object CheckBoxboSelGate_GetStart1: TCheckBox
                Left = 8
                Top = 32
                Width = 105
                Height = 17
                Caption = #21551#21160#35282#33394#32593#20851#20108
                TabOrder = 1
                OnClick = CheckBoxboSelGate_GetStart1Click
              end
              object CheckBoxboSelGate_GetMinimize: TCheckBox
                Left = 8
                Top = 48
                Width = 113
                Height = 17
                Caption = #21551#21160#25104#21151#21518#26368#23567#21270
                TabOrder = 2
                OnClick = CheckBoxboSelGate_GetMinimizeClick
              end
            end
            object chkSelGate_GetMultiThread: TCheckBox
              Left = 536
              Top = 24
              Width = 105
              Height = 17
              Caption = #20351#29992#22810#32447#31243#32593#20851
              TabOrder = 4
              OnClick = chkSelGate_GetMultiThreadClick
            end
          end
          object ButtonPrv3: TButton
            Left = 478
            Top = 354
            Width = 81
            Height = 33
            Caption = #19978#19968#27493'(&P)'
            TabOrder = 1
            OnClick = ButtonPrv3Click
          end
          object ButtonNext3: TButton
            Left = 566
            Top = 354
            Width = 81
            Height = 33
            Caption = #19979#19968#27493'(&N)'
            TabOrder = 2
            OnClick = ButtonNext3Click
          end
        end
        object TabSheet12: TTabSheet
          BorderWidth = 5
          Caption = '4-'#28216#25103#32593#20851
          ImageIndex = 8
          ExplicitLeft = 0
          ExplicitTop = 0
          ExplicitWidth = 0
          ExplicitHeight = 0
          object ButtonPrv4: TButton
            Left = 478
            Top = 354
            Width = 81
            Height = 33
            Caption = #19978#19968#27493'(&P)'
            TabOrder = 0
            OnClick = ButtonPrv4Click
          end
          object ButtonNext4: TButton
            Left = 566
            Top = 354
            Width = 81
            Height = 33
            Caption = #19979#19968#27493'(&N)'
            TabOrder = 1
            OnClick = ButtonNext4Click
          end
          object GroupBox17: TGroupBox
            Left = 0
            Top = 0
            Width = 648
            Height = 172
            Align = alTop
            Caption = #28216#25103#32593#20851#35774#32622
            TabOrder = 2
            object GroupBox18: TGroupBox
              Left = 8
              Top = 16
              Width = 99
              Height = 69
              Caption = #31383#21475#20301#32622
              Enabled = False
              TabOrder = 0
              object Label21: TLabel
                Left = 8
                Top = 22
                Width = 36
                Height = 12
                Caption = #24231#26631'X:'
                Enabled = False
              end
              object Label22: TLabel
                Left = 8
                Top = 46
                Width = 36
                Height = 12
                Caption = #24231#26631'Y:'
                Enabled = False
              end
              object EditRunGate_MainFormX: TSpinEdit
                Left = 44
                Top = 17
                Width = 50
                Height = 21
                Hint = #21551#21160#31243#24207#31383#21475#22312#23631#24149#19978#30340#20301#32622#65292#24231#26631'X'#12290
                Enabled = False
                MaxValue = 10000
                MinValue = 0
                TabOrder = 0
                Value = 0
              end
              object EditRunGate_MainFormY: TSpinEdit
                Left = 44
                Top = 41
                Width = 50
                Height = 21
                Hint = #21551#21160#31243#24207#31383#21475#22312#23631#24149#19978#30340#20301#32622#65292#24231#26631'Y'#12290
                Enabled = False
                MaxValue = 10000
                MinValue = 0
                TabOrder = 1
                Value = 0
              end
            end
            object GroupBox19: TGroupBox
              Left = 8
              Top = 92
              Width = 99
              Height = 69
              Caption = #24320#21551#32593#20851#25968#37327
              TabOrder = 1
              object Label23: TLabel
                Left = 8
                Top = 20
                Width = 30
                Height = 12
                Caption = #25968#37327':'
              end
              object EditRunGate_Connt: TSpinEdit
                Left = 43
                Top = 16
                Width = 50
                Height = 21
                Hint = #35774#32622#24320#21551#28216#25103#32593#20851#25968#37327#65292#19968#33324'200'#20154#20197#19979#30340#24320#19968#20010#32593#20851#65292'400'#20154#20197#19979#30340#24320#20108#20010#32593#20851#65292'400'#20154#20197#19978#30340#24320#19977#20010#32593#20851#12290
                MaxValue = 8
                MinValue = 1
                TabOrder = 0
                Value = 1
                OnChange = EditRunGate_ConntChange
              end
            end
            object GroupBox22: TGroupBox
              Left = 119
              Top = 16
              Width = 341
              Height = 69
              Caption = #26381#21153#22120#31471#21475
              TabOrder = 2
              object LabelRunGate_GatePort1: TLabel
                Left = 8
                Top = 22
                Width = 18
                Height = 12
                Caption = #19968':'
              end
              object LabelLabelRunGate_GatePort2: TLabel
                Left = 96
                Top = 22
                Width = 18
                Height = 12
                Caption = #20108':'
              end
              object LabelRunGate_GatePort3: TLabel
                Left = 184
                Top = 22
                Width = 18
                Height = 12
                Caption = #19977':'
              end
              object LabelRunGate_GatePort4: TLabel
                Left = 272
                Top = 21
                Width = 18
                Height = 12
                Caption = #22235':'
              end
              object LabelRunGate_GatePort5: TLabel
                Left = 8
                Top = 46
                Width = 18
                Height = 12
                Caption = #20116':'
              end
              object LabelRunGate_GatePort6: TLabel
                Left = 96
                Top = 46
                Width = 18
                Height = 12
                Caption = #20845':'
              end
              object LabelRunGate_GatePort7: TLabel
                Left = 184
                Top = 46
                Width = 18
                Height = 12
                Caption = #19971':'
              end
              object LabelRunGate_GatePort78: TLabel
                Left = 272
                Top = 46
                Width = 18
                Height = 12
                Caption = #20843':'
              end
              object EditRunGate_GatePort1: TEdit
                Left = 26
                Top = 18
                Width = 41
                Height = 20
                ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
                TabOrder = 0
                Text = '7200'
              end
              object EditRunGate_GatePort2: TEdit
                Left = 114
                Top = 18
                Width = 41
                Height = 20
                ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
                TabOrder = 1
                Text = '7200'
              end
              object EditRunGate_GatePort3: TEdit
                Left = 202
                Top = 18
                Width = 41
                Height = 20
                ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
                TabOrder = 2
                Text = '7200'
              end
              object EditRunGate_GatePort4: TEdit
                Left = 290
                Top = 17
                Width = 41
                Height = 20
                ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
                TabOrder = 3
                Text = '7200'
              end
              object EditRunGate_GatePort5: TEdit
                Left = 26
                Top = 42
                Width = 41
                Height = 20
                ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
                TabOrder = 4
                Text = '7200'
              end
              object EditRunGate_GatePort6: TEdit
                Left = 113
                Top = 42
                Width = 41
                Height = 20
                ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
                TabOrder = 5
                Text = '7200'
              end
              object EditRunGate_GatePort7: TEdit
                Left = 201
                Top = 42
                Width = 41
                Height = 20
                ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
                TabOrder = 6
                Text = '7200'
              end
              object EditRunGate_GatePort8: TEdit
                Left = 290
                Top = 42
                Width = 41
                Height = 20
                ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
                TabOrder = 7
                Text = '7200'
              end
            end
            object ButtonRunGateDefault: TButton
              Left = 554
              Top = 128
              Width = 81
              Height = 25
              Caption = #40664#35748#35774#32622'(&D)'
              TabOrder = 3
              OnClick = ButtonRunGateDefaultClick
            end
            object GroupBox44: TGroupBox
              Left = 475
              Top = 16
              Width = 160
              Height = 41
              Caption = #26159#21542#26368#23567#21270
              TabOrder = 4
              object CheckBoxboRunGate_GetMinimize: TCheckBox
                Left = 8
                Top = 16
                Width = 113
                Height = 17
                Caption = #21551#21160#25104#21151#21518#26368#23567#21270
                TabOrder = 0
                OnClick = CheckBoxboRunGate_GetMinimizeClick
              end
            end
            object GroupBox28: TGroupBox
              Left = 119
              Top = 92
              Width = 341
              Height = 69
              Caption = 'DBServer'#36830#25509#31471#21475
              TabOrder = 5
              object Label7: TLabel
                Left = 8
                Top = 22
                Width = 18
                Height = 12
                Caption = #19968':'
              end
              object Label8: TLabel
                Left = 96
                Top = 22
                Width = 18
                Height = 12
                Caption = #20108':'
              end
              object Label24: TLabel
                Left = 184
                Top = 22
                Width = 18
                Height = 12
                Caption = #19977':'
              end
              object Label25: TLabel
                Left = 272
                Top = 22
                Width = 18
                Height = 12
                Caption = #22235':'
              end
              object Label26: TLabel
                Left = 8
                Top = 47
                Width = 18
                Height = 12
                Caption = #20116':'
              end
              object Label27: TLabel
                Left = 96
                Top = 47
                Width = 18
                Height = 12
                Caption = #20845':'
              end
              object Label45: TLabel
                Left = 184
                Top = 46
                Width = 18
                Height = 12
                Caption = #19971':'
              end
              object Label46: TLabel
                Left = 272
                Top = 46
                Width = 18
                Height = 12
                Caption = #20843':'
              end
              object edtRunGate_DBPort1: TEdit
                Left = 26
                Top = 18
                Width = 41
                Height = 20
                ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
                TabOrder = 0
                Text = '7200'
              end
              object edtRunGate_DBPort2: TEdit
                Left = 113
                Top = 18
                Width = 41
                Height = 20
                ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
                TabOrder = 1
                Text = '7200'
              end
              object edtRunGate_DBPort3: TEdit
                Left = 201
                Top = 18
                Width = 41
                Height = 20
                ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
                TabOrder = 2
                Text = '7200'
              end
              object edtRunGate_DBPort4: TEdit
                Left = 290
                Top = 18
                Width = 41
                Height = 20
                ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
                TabOrder = 3
                Text = '7200'
              end
              object edtRunGate_DBPort5: TEdit
                Left = 26
                Top = 43
                Width = 41
                Height = 20
                ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
                TabOrder = 4
                Text = '7200'
              end
              object edtRunGate_DBPort6: TEdit
                Left = 113
                Top = 43
                Width = 41
                Height = 20
                ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
                TabOrder = 5
                Text = '7200'
              end
              object edtRunGate_DBPort7: TEdit
                Left = 201
                Top = 42
                Width = 41
                Height = 20
                ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
                TabOrder = 6
                Text = '7200'
              end
              object edtRunGate_DBPort8: TEdit
                Left = 290
                Top = 42
                Width = 41
                Height = 20
                ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
                TabOrder = 7
                Text = '7200'
              end
            end
            object grp4: TGroupBox
              Left = 475
              Top = 62
              Width = 160
              Height = 60
              Caption = #22810#32447#31243#32593#20851#35774#32622
              TabOrder = 6
              object lbl14: TLabel
                Left = 8
                Top = 38
                Width = 96
                Height = 12
                Caption = 'DBServer'#36830#25509#31471#21475
              end
              object CheckBoxboRunGate_GetMultiThread: TCheckBox
                Left = 8
                Top = 16
                Width = 105
                Height = 17
                Caption = #20351#29992#22810#32447#31243#32593#20851
                TabOrder = 0
                OnClick = CheckBoxboRunGate_GetMultiThreadClick
              end
              object edtRunGate_DBPortMulThread: TEdit
                Left = 106
                Top = 34
                Width = 37
                Height = 20
                ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
                TabOrder = 1
                Text = '7200'
              end
            end
          end
        end
        object TabSheet7: TTabSheet
          BorderWidth = 5
          Caption = '5-'#30331#24405#26381#21153#22120
          ImageIndex = 3
          ExplicitLeft = 0
          ExplicitTop = 0
          ExplicitWidth = 0
          ExplicitHeight = 0
          object GroupBox9: TGroupBox
            Left = 0
            Top = 0
            Width = 648
            Height = 115
            Align = alTop
            Caption = #30331#24405#26381#21153#22120#35774#32622
            TabOrder = 0
            object GroupBox10: TGroupBox
              Left = 8
              Top = 16
              Width = 129
              Height = 69
              Caption = #31383#21475#20301#32622
              TabOrder = 0
              object Label13: TLabel
                Left = 8
                Top = 20
                Width = 36
                Height = 12
                Caption = #24231#26631'X:'
              end
              object Label14: TLabel
                Left = 8
                Top = 44
                Width = 36
                Height = 12
                Caption = #24231#26631'Y:'
              end
              object EditLoginServer_MainFormX: TSpinEdit
                Left = 48
                Top = 16
                Width = 65
                Height = 21
                Hint = #21551#21160#31243#24207#31383#21475#22312#23631#24149#19978#30340#20301#32622#65292#24231#26631'X'#12290
                MaxValue = 10000
                MinValue = 0
                TabOrder = 0
                Value = 0
                OnChange = EditLoginServer_MainFormXChange
              end
              object EditLoginServer_MainFormY: TSpinEdit
                Left = 48
                Top = 40
                Width = 65
                Height = 21
                Hint = #21551#21160#31243#24207#31383#21475#22312#23631#24149#19978#30340#20301#32622#65292#24231#26631'Y'#12290
                MaxValue = 10000
                MinValue = 0
                TabOrder = 1
                Value = 0
                OnChange = EditLoginServer_MainFormYChange
              end
            end
            object ButtonLoginServerConfig: TButton
              Left = 464
              Top = 64
              Width = 81
              Height = 25
              Caption = #39640#32423#35774#32622
              TabOrder = 1
              Visible = False
              OnClick = ButtonLoginServerConfigClick
            end
            object ButtonLoginSrvDefault: TButton
              Left = 552
              Top = 64
              Width = 81
              Height = 25
              Caption = #40664#35748#35774#32622'(&D)'
              TabOrder = 2
              OnClick = ButtonLoginSrvDefaultClick
            end
            object GroupBox33: TGroupBox
              Left = 144
              Top = 16
              Width = 121
              Height = 92
              Caption = #21069#32622#26381#21153#22120#31471#21475
              TabOrder = 3
              object Label50: TLabel
                Left = 8
                Top = 20
                Width = 54
                Height = 12
                Caption = #36830#25509#31471#21475':'
              end
              object Label51: TLabel
                Left = 8
                Top = 44
                Width = 54
                Height = 12
                Caption = #36890#35759#31471#21475':'
              end
              object Label2: TLabel
                Left = 8
                Top = 68
                Width = 54
                Height = 12
                Caption = #31649#29702#31471#21475':'
              end
              object EditLoginServerGatePort: TEdit
                Left = 64
                Top = 16
                Width = 49
                Height = 20
                ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
                TabOrder = 0
                Text = '7200'
              end
              object EditLoginServerServerPort: TEdit
                Left = 64
                Top = 40
                Width = 49
                Height = 20
                ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
                TabOrder = 1
                Text = '7200'
              end
              object EditLoginServerControlPort: TEdit
                Left = 64
                Top = 64
                Width = 49
                Height = 20
                Hint = #31649#29702#31471#21475#35774#32622#20026'0'#34920#31034#20851#38381#36828#31243#21151#33021#13#10#22823#20110'0'#21017#24320#21551#36828#31243#31649#29702#21151#33021
                ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
                TabOrder = 2
                Text = '7200'
              end
            end
            object GroupBox34: TGroupBox
              Left = 272
              Top = 16
              Width = 161
              Height = 69
              Caption = #21551#21160#36873#39033
              TabOrder = 4
              object CheckBoxboLoginServer_GetStart: TCheckBox
                Left = 8
                Top = 16
                Width = 105
                Height = 17
                Caption = #21551#21160#30331#24405#26381#21153#22120
                TabOrder = 0
                OnClick = CheckBoxboLoginServer_GetStartClick
              end
              object CheckBoxboLoginServer_GetMinimize: TCheckBox
                Left = 8
                Top = 40
                Width = 113
                Height = 17
                Caption = #21551#21160#25104#21151#21518#26368#23567#21270
                TabOrder = 1
                OnClick = CheckBoxboLoginServer_GetMinimizeClick
              end
            end
          end
          object ButtonPrv5: TButton
            Left = 478
            Top = 354
            Width = 81
            Height = 33
            Caption = #19978#19968#27493'(&P)'
            TabOrder = 1
            OnClick = ButtonPrv5Click
          end
          object ButtonNext5: TButton
            Left = 566
            Top = 354
            Width = 81
            Height = 33
            Caption = #19979#19968#27493'(&N)'
            TabOrder = 2
            OnClick = ButtonNext5Click
          end
        end
        object TabSheet8: TTabSheet
          BorderWidth = 5
          Caption = '6-'#25968#25454#24211#26381#21153#22120
          ImageIndex = 4
          ExplicitLeft = 0
          ExplicitTop = 0
          ExplicitWidth = 0
          ExplicitHeight = 0
          object GroupBox11: TGroupBox
            Left = 0
            Top = 0
            Width = 648
            Height = 99
            Align = alTop
            Caption = #25968#25454#24211#26381#21153#22120#35774#32622
            TabOrder = 0
            object GroupBox12: TGroupBox
              Left = 8
              Top = 20
              Width = 129
              Height = 69
              Caption = #31383#21475#20301#32622
              TabOrder = 0
              object Label15: TLabel
                Left = 8
                Top = 20
                Width = 36
                Height = 12
                Caption = #24231#26631'X:'
              end
              object Label16: TLabel
                Left = 8
                Top = 44
                Width = 36
                Height = 12
                Caption = #24231#26631'Y:'
              end
              object EditDBServer_MainFormX: TSpinEdit
                Left = 48
                Top = 16
                Width = 65
                Height = 21
                Hint = #21551#21160#31243#24207#31383#21475#22312#23631#24149#19978#30340#20301#32622#65292#24231#26631'X'#12290
                MaxValue = 10000
                MinValue = 0
                TabOrder = 0
                Value = 0
                OnChange = EditDBServer_MainFormXChange
              end
              object EditDBServer_MainFormY: TSpinEdit
                Left = 48
                Top = 40
                Width = 65
                Height = 21
                Hint = #21551#21160#31243#24207#31383#21475#22312#23631#24149#19978#30340#20301#32622#65292#24231#26631'Y'#12290
                MaxValue = 10000
                MinValue = 0
                TabOrder = 1
                Value = 0
                OnChange = EditDBServer_MainFormYChange
              end
            end
            object ButtonDBServerDefault: TButton
              Left = 552
              Top = 56
              Width = 81
              Height = 25
              Caption = #40664#35748#35774#32622'(&D)'
              TabOrder = 1
              OnClick = ButtonDBServerDefaultClick
            end
            object GroupBox35: TGroupBox
              Left = 272
              Top = 20
              Width = 161
              Height = 69
              Caption = #21551#21160#36873#39033
              TabOrder = 2
              object CheckBoxDBServerGetStart: TCheckBox
                Left = 8
                Top = 16
                Width = 113
                Height = 17
                Caption = #21551#21160#25968#25454#24211#26381#21153#22120
                TabOrder = 0
                OnClick = CheckBoxDBServerGetStartClick
              end
              object CheckBoxDBServerGetMinimize: TCheckBox
                Left = 8
                Top = 40
                Width = 113
                Height = 17
                Caption = #21551#21160#25104#21151#21518#26368#23567#21270
                TabOrder = 1
                OnClick = CheckBoxDBServerGetMinimizeClick
              end
            end
            object GroupBox36: TGroupBox
              Left = 144
              Top = 20
              Width = 121
              Height = 69
              Caption = #21069#32622#26381#21153#22120#31471#21475
              TabOrder = 3
              object Label52: TLabel
                Left = 8
                Top = 20
                Width = 54
                Height = 12
                Caption = #36830#25509#31471#21475':'
              end
              object Label53: TLabel
                Left = 8
                Top = 44
                Width = 54
                Height = 12
                Caption = #36890#35759#31471#21475':'
              end
              object EditDBServerGatePort: TEdit
                Left = 64
                Top = 16
                Width = 41
                Height = 20
                ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
                TabOrder = 0
                Text = '5100'
              end
              object EditDBServerServerPort: TEdit
                Left = 64
                Top = 40
                Width = 41
                Height = 20
                ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
                TabOrder = 1
                Text = '6000'
              end
            end
          end
          object ButtonPrv6: TButton
            Left = 478
            Top = 354
            Width = 81
            Height = 33
            Caption = #19978#19968#27493'(&P)'
            TabOrder = 1
            OnClick = ButtonPrv6Click
          end
          object ButtonNext6: TButton
            Left = 566
            Top = 354
            Width = 81
            Height = 33
            Caption = #19979#19968#27493'(&N)'
            TabOrder = 2
            OnClick = ButtonNext6Click
          end
        end
        object TabSheet9: TTabSheet
          BorderWidth = 5
          Caption = '7-'#26085#24535#26381#21153#22120
          ImageIndex = 5
          ExplicitLeft = 0
          ExplicitTop = 0
          ExplicitWidth = 0
          ExplicitHeight = 0
          object GroupBox13: TGroupBox
            Left = 0
            Top = 0
            Width = 648
            Height = 97
            Align = alTop
            Caption = #28216#25103#26085#24535#26381#21153#22120#35774#32622
            TabOrder = 0
            object GroupBox14: TGroupBox
              Left = 8
              Top = 16
              Width = 129
              Height = 71
              Caption = #31383#21475#20301#32622
              TabOrder = 0
              object Label17: TLabel
                Left = 10
                Top = 20
                Width = 36
                Height = 12
                Caption = #24231#26631'X:'
              end
              object Label18: TLabel
                Left = 10
                Top = 45
                Width = 36
                Height = 12
                Caption = #24231#26631'Y:'
              end
              object EditLogServer_MainFormX: TSpinEdit
                Left = 48
                Top = 16
                Width = 65
                Height = 21
                Hint = #21551#21160#31243#24207#31383#21475#22312#23631#24149#19978#30340#20301#32622#65292#24231#26631'X'#12290
                MaxValue = 10000
                MinValue = 0
                TabOrder = 0
                Value = 0
                OnChange = EditLogServer_MainFormXChange
              end
              object EditLogServer_MainFormY: TSpinEdit
                Left = 48
                Top = 41
                Width = 65
                Height = 21
                Hint = #21551#21160#31243#24207#31383#21475#22312#23631#24149#19978#30340#20301#32622#65292#24231#26631'Y'#12290
                MaxValue = 10000
                MinValue = 0
                TabOrder = 1
                Value = 0
                OnChange = EditLogServer_MainFormYChange
              end
            end
            object ButtonLogServerDefault: TButton
              Left = 544
              Top = 56
              Width = 81
              Height = 25
              Caption = #40664#35748#35774#32622'(&D)'
              TabOrder = 1
              OnClick = ButtonLogServerDefaultClick
            end
            object GroupBox37: TGroupBox
              Left = 280
              Top = 16
              Width = 153
              Height = 71
              Caption = #21551#21160#36873#39033
              TabOrder = 2
              object CheckBoxLogServerGetStart: TCheckBox
                Left = 8
                Top = 18
                Width = 105
                Height = 17
                Caption = #21551#21160#26085#24535#26381#21153#22120
                TabOrder = 0
                OnClick = CheckBoxLogServerGetStartClick
              end
              object CheckBoxLogServerGetMinimize: TCheckBox
                Left = 8
                Top = 42
                Width = 113
                Height = 17
                Caption = #21551#21160#25104#21151#21518#26368#23567#21270
                TabOrder = 1
                OnClick = CheckBoxLogServerGetMinimizeClick
              end
            end
            object GroupBox38: TGroupBox
              Left = 144
              Top = 16
              Width = 121
              Height = 71
              Caption = #32593#32476#31471#21475
              TabOrder = 3
              object Label54: TLabel
                Left = 8
                Top = 20
                Width = 54
                Height = 12
                Caption = #32593#32476#31471#21475':'
              end
              object EditLogServerPort: TEdit
                Left = 64
                Top = 16
                Width = 41
                Height = 20
                ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
                TabOrder = 0
                Text = '10000'
              end
            end
          end
          object ButtonPrv7: TButton
            Left = 478
            Top = 354
            Width = 81
            Height = 33
            Caption = #19978#19968#27493'(&P)'
            TabOrder = 1
            OnClick = ButtonPrv7Click
          end
          object ButtonNext7: TButton
            Left = 566
            Top = 354
            Width = 81
            Height = 33
            Caption = #19979#19968#27493'(&N)'
            TabOrder = 2
            OnClick = ButtonNext7Click
          end
        end
        object TabSheet10: TTabSheet
          BorderWidth = 5
          Caption = '8-'#20027#26381#21153#22120
          ImageIndex = 6
          ExplicitLeft = 0
          ExplicitTop = 0
          ExplicitWidth = 0
          ExplicitHeight = 0
          object GroupBox15: TGroupBox
            Left = 0
            Top = 0
            Width = 648
            Height = 101
            Align = alTop
            Caption = #28216#25103#24341#25806#26381#21153#22120#35774#32622
            TabOrder = 0
            object GroupBox16: TGroupBox
              Left = 8
              Top = 16
              Width = 125
              Height = 73
              Caption = #31383#21475#20301#32622
              TabOrder = 0
              object Label19: TLabel
                Left = 12
                Top = 20
                Width = 36
                Height = 12
                Caption = #24231#26631'X:'
              end
              object Label20: TLabel
                Left = 12
                Top = 44
                Width = 36
                Height = 12
                Caption = #24231#26631'Y:'
              end
              object EditM2Server_MainFormX: TSpinEdit
                Left = 50
                Top = 16
                Width = 65
                Height = 21
                Hint = #21551#21160#31243#24207#31383#21475#22312#23631#24149#19978#30340#20301#32622#65292#24231#26631'X'#12290
                MaxValue = 10000
                MinValue = 0
                TabOrder = 0
                Value = 0
                OnChange = EditM2Server_MainFormXChange
              end
              object EditM2Server_MainFormY: TSpinEdit
                Left = 50
                Top = 40
                Width = 65
                Height = 21
                Hint = #21551#21160#31243#24207#31383#21475#22312#23631#24149#19978#30340#20301#32622#65292#24231#26631'Y'#12290
                MaxValue = 10000
                MinValue = 0
                TabOrder = 1
                Value = 0
                OnChange = EditM2Server_MainFormYChange
              end
            end
            object ButtonM2ServerDefault: TButton
              Left = 560
              Top = 61
              Width = 81
              Height = 25
              Caption = #40664#35748#35774#32622'(&D)'
              TabOrder = 1
              OnClick = ButtonM2ServerDefaultClick
            end
            object GroupBox32: TGroupBox
              Left = 264
              Top = 16
              Width = 145
              Height = 73
              Caption = #26032#20154#35774#32622
              TabOrder = 2
              object Label61: TLabel
                Left = 8
                Top = 20
                Width = 54
                Height = 12
                Caption = #24320#22987#31561#32423':'
              end
              object Label62: TLabel
                Left = 8
                Top = 44
                Width = 54
                Height = 12
                Caption = #24320#22987#37329#24065':'
              end
              object EditM2Server_TestLevel: TSpinEdit
                Left = 68
                Top = 16
                Width = 69
                Height = 21
                Hint = #20154#29289#36215#22987#31561#32423#12290
                MaxValue = 20000
                MinValue = 0
                TabOrder = 0
                Value = 10
                OnChange = EditM2Server_TestLevelChange
              end
              object EditM2Server_TestGold: TSpinEdit
                Left = 68
                Top = 40
                Width = 69
                Height = 21
                Hint = #27979#35797#27169#24335#20154#29289#36215#22987#37329#24065#25968#12290
                Increment = 1000
                MaxValue = 20000000
                MinValue = 0
                TabOrder = 1
                Value = 10
                OnChange = EditM2Server_TestGoldChange
              end
            end
            object GroupBox39: TGroupBox
              Left = 144
              Top = 16
              Width = 113
              Height = 73
              Caption = #21069#32622#26381#21153#22120#31471#21475
              TabOrder = 3
              object Label55: TLabel
                Left = 8
                Top = 20
                Width = 54
                Height = 12
                Caption = #36830#25509#31471#21475':'
              end
              object Label56: TLabel
                Left = 8
                Top = 44
                Width = 54
                Height = 12
                Caption = #36890#35759#31471#21475':'
              end
              object EditM2ServerGatePort: TEdit
                Left = 64
                Top = 16
                Width = 41
                Height = 20
                ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
                TabOrder = 0
                Text = '5000'
              end
              object EditM2ServerMsgSrvPort: TEdit
                Left = 64
                Top = 40
                Width = 41
                Height = 20
                ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
                TabOrder = 1
                Text = '4900'
              end
            end
            object GroupBox40: TGroupBox
              Left = 416
              Top = 16
              Width = 129
              Height = 73
              Caption = #21551#21160#36873#39033
              TabOrder = 4
              object CheckBoxM2ServerGetStart: TCheckBox
                Left = 8
                Top = 17
                Width = 105
                Height = 17
                Caption = #21551#21160#28216#25103#26381#21153#22120
                TabOrder = 0
                OnClick = CheckBoxM2ServerGetStartClick
              end
              object CheckBoxM2ServerGetMinimize: TCheckBox
                Left = 8
                Top = 41
                Width = 113
                Height = 17
                Caption = #21551#21160#25104#21151#21518#26368#23567#21270
                TabOrder = 1
                OnClick = CheckBoxM2ServerGetMinimizeClick
              end
            end
          end
          object ButtonPrv8: TButton
            Left = 478
            Top = 354
            Width = 81
            Height = 33
            Caption = #19978#19968#27493'(&P)'
            TabOrder = 1
            OnClick = ButtonPrv8Click
          end
          object ButtonNext8: TButton
            Left = 566
            Top = 354
            Width = 81
            Height = 33
            Caption = #19979#19968#27493'(&N)'
            TabOrder = 2
            OnClick = ButtonNext8Click
          end
        end
        object TabSheet11: TTabSheet
          BorderWidth = 5
          Caption = '9-'#20445#23384#37197#32622
          ImageIndex = 7
          object ButtonSave: TButton
            Left = 566
            Top = 354
            Width = 81
            Height = 33
            Caption = #20445#23384'(&S)'
            TabOrder = 0
            OnClick = ButtonSaveClick
          end
          object ButtonGenGameConfig: TButton
            Left = 390
            Top = 354
            Width = 81
            Height = 33
            Caption = #29983#25104#37197#32622'(&G)'
            TabOrder = 1
            OnClick = ButtonGenGameConfigClick
          end
          object ButtonPrv9: TButton
            Left = 478
            Top = 354
            Width = 81
            Height = 33
            Caption = #19978#19968#27493'(&P)'
            TabOrder = 2
            OnClick = ButtonPrv9Click
          end
        end
      end
    end
    object TabSheet66: TTabSheet
      BorderWidth = 5
      Caption = #25968#25454#22791#20221
      ImageIndex = 4
      ExplicitLeft = 0
      ExplicitTop = 0
      ExplicitWidth = 0
      ExplicitHeight = 0
      object LabelBackMsg: TLabel
        Left = 376
        Top = 296
        Width = 6
        Height = 12
        Font.Charset = GB2312_CHARSET
        Font.Color = clGreen
        Font.Height = -12
        Font.Name = #23435#20307
        Font.Style = []
        ParentFont = False
      end
      object GroupBox21: TGroupBox
        Left = 0
        Top = 0
        Width = 656
        Height = 153
        Align = alTop
        Caption = #22791#20221#21015#34920
        TabOrder = 0
        object ListViewDataBackup: TListView
          Left = 8
          Top = 16
          Width = 640
          Height = 129
          Columns = <
            item
              Caption = #25968#25454#30446#24405
              Width = 200
            end
            item
              Caption = #22791#20221#30446#24405
              Width = 200
            end
            item
              Caption = #22791#20221#27425#25968
              Width = 60
            end
            item
              Caption = #22833#36133#27425#25968
              Width = 60
            end
            item
              Caption = #29366#24577
              Width = 100
            end>
          GridLines = True
          ReadOnly = True
          RowSelect = True
          TabOrder = 0
          ViewStyle = vsReport
          OnClick = ListViewDataBackupClick
        end
      end
      object GroupBox29: TGroupBox
        Left = 0
        Top = 160
        Width = 656
        Height = 121
        Caption = #32534#36753
        TabOrder = 1
        object lbl1: TLabel
          Left = 8
          Top = 20
          Width = 54
          Height = 12
          Caption = #25968#25454#30446#24405':'
        end
        object lbl2: TLabel
          Left = 8
          Top = 44
          Width = 54
          Height = 12
          Caption = #22791#20221#30446#24405':'
        end
        object lbl3: TLabel
          Left = 120
          Top = 68
          Width = 12
          Height = 12
          Caption = #28857
        end
        object lbl4: TLabel
          Left = 194
          Top = 68
          Width = 12
          Height = 12
          Caption = #20998
        end
        object lbl5: TLabel
          Left = 116
          Top = 92
          Width = 24
          Height = 12
          Caption = #23567#26102
        end
        object lbl6: TLabel
          Left = 194
          Top = 92
          Width = 12
          Height = 12
          Caption = #20998
        end
        object RadioButtonBackMode2: TRadioButton
          Left = 14
          Top = 89
          Width = 49
          Height = 17
          Caption = #27599#38548
          TabOrder = 3
          OnClick = RadioButtonBackMode2Click
        end
        object RadioButtonBackMode1: TRadioButton
          Left = 14
          Top = 64
          Width = 49
          Height = 17
          Caption = #27599#22825
          Checked = True
          TabOrder = 0
          TabStop = True
          OnClick = RadioButtonBackMode1Click
        end
        object EditSource: TRzButtonEdit
          Left = 64
          Top = 16
          Width = 581
          Height = 20
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          TabOrder = 1
          AltBtnWidth = 15
          ButtonWidth = 15
          OnButtonClick = EditSourceButtonClick
        end
        object EditDest: TRzButtonEdit
          Left = 64
          Top = 40
          Width = 581
          Height = 20
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          TabOrder = 2
          AltBtnWidth = 15
          ButtonWidth = 15
          OnButtonClick = EditDestButtonClick
        end
        object EditHour1: TRzSpinEdit
          Left = 64
          Top = 64
          Width = 47
          Height = 20
          Max = 23.000000000000000000
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          TabOrder = 4
        end
        object EditHour2: TRzSpinEdit
          Left = 64
          Top = 88
          Width = 47
          Height = 20
          Max = 10000000000.000000000000000000
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          TabOrder = 5
        end
        object EditMin1: TRzSpinEdit
          Left = 144
          Top = 64
          Width = 47
          Height = 20
          Max = 59.000000000000000000
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          TabOrder = 6
        end
        object EditMin2: TRzSpinEdit
          Left = 144
          Top = 88
          Width = 47
          Height = 20
          Max = 59.000000000000000000
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          TabOrder = 7
        end
        object chkIsCompress: TCheckBox
          Left = 224
          Top = 80
          Width = 89
          Height = 17
          Caption = #22791#20221#26102#21387#32553
          TabOrder = 8
        end
        object chkAutoStart: TCheckBox
          Left = 352
          Top = 80
          Width = 137
          Height = 17
          Caption = #36816#34892#21518#33258#21160#24320#21551#22791#20221
          TabOrder = 9
          OnClick = chkAutoStartClick
        end
      end
      object ButtonBackChg: TButton
        Left = 0
        Top = 288
        Width = 65
        Height = 25
        Caption = #20462#25913'(&C)'
        TabOrder = 2
        OnClick = ButtonBackChgClick
      end
      object ButtonBackDel: TButton
        Left = 72
        Top = 288
        Width = 65
        Height = 25
        Caption = #21024#38500'(&D)'
        TabOrder = 3
        OnClick = ButtonBackDelClick
      end
      object ButtonBackAdd: TButton
        Left = 144
        Top = 288
        Width = 65
        Height = 25
        Caption = #22686#21152'(&A)'
        TabOrder = 4
        OnClick = ButtonBackAddClick
      end
      object ButtonBackSave: TButton
        Left = 216
        Top = 288
        Width = 65
        Height = 25
        Caption = #20445#23384'(&S)'
        TabOrder = 5
        OnClick = ButtonBackSaveClick
      end
      object ButtonBackStart: TButton
        Left = 288
        Top = 288
        Width = 75
        Height = 25
        Caption = #21551#21160'(&B)'
        TabOrder = 6
        OnClick = ButtonBackStartClick
      end
    end
    object TabSheet3: TTabSheet
      BorderWidth = 5
      Caption = 'HeroM2'#29256#26412#36716#25442
      ImageIndex = 5
      ExplicitLeft = 0
      ExplicitTop = 0
      ExplicitWidth = 0
      ExplicitHeight = 0
      object Label5: TLabel
        Left = 0
        Top = 8
        Width = 60
        Height = 12
        Caption = 'Envir'#30446#24405':'
      end
      object Label6: TLabel
        Left = 0
        Top = 32
        Width = 54
        Height = 12
        Caption = #25968#25454#24211#21517':'
        Transparent = False
      end
      object EditEnvirFilePath: TRzButtonEdit
        Left = 64
        Top = 5
        Width = 592
        Height = 20
        Text = 'D:\MirServer\Mir200\Envir\'
        ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
        TabOrder = 0
        AltBtnWidth = 15
        ButtonWidth = 15
      end
      object EditDBName: TEdit
        Left = 64
        Top = 28
        Width = 121
        Height = 20
        ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
        TabOrder = 1
        Text = 'HeroDB'
      end
      object MemoLog1: TMemo
        Left = 0
        Top = 56
        Width = 655
        Height = 289
        ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
        ScrollBars = ssVertical
        TabOrder = 2
      end
      object ButtonStdMode: TButton
        Left = 0
        Top = 352
        Width = 129
        Height = 25
        Caption = #36716#25442#34915#26381#21644#27494#22120'DB'#36716#25442
        TabOrder = 3
        OnClick = ButtonStdModeClick
      end
      object ButtonUnbindItem: TButton
        Left = 0
        Top = 384
        Width = 129
        Height = 25
        Caption = #25414#32465#29289#21697'DB'#36716#25442
        TabOrder = 4
        OnClick = ButtonUnbindItemClick
      end
      object ButtonUnTakeOffItem: TButton
        Left = 304
        Top = 352
        Width = 129
        Height = 25
        Caption = #20462#22797#35013#22791#31359#33073#19981#20102
        TabOrder = 5
        OnClick = ButtonUnTakeOffItemClick
      end
      object ButtonChangeItemNameColorWhite: TButton
        Left = 440
        Top = 352
        Width = 161
        Height = 25
        Caption = #25152#26377#29289#21697#21517#31216#39068#33394#35843#25972#20026#40644#33394
        TabOrder = 6
        OnClick = ButtonChangeItemNameColorWhiteClick
      end
      object ButtonMapEvent: TButton
        Left = 136
        Top = 352
        Width = 161
        Height = 25
        Caption = 'MapEvent'#22320#22270#35302#21457#25991#20214#36716#25442
        TabOrder = 7
        OnClick = ButtonMapEventClick
      end
    end
    object TabSheet14: TTabSheet
      BorderWidth = 5
      Caption = #25968#25454#28165#29702
      ImageIndex = 6
      ExplicitLeft = 0
      ExplicitTop = 0
      ExplicitWidth = 0
      ExplicitHeight = 0
      object pgc1: TPageControl
        Left = 0
        Top = 0
        Width = 656
        Height = 383
        ActivePage = ts2
        Align = alTop
        TabOrder = 0
        object ts2: TTabSheet
          Caption = #22522#26412#25968#25454
          ImageIndex = 1
          ExplicitLeft = 0
          ExplicitTop = 0
          ExplicitWidth = 0
          ExplicitHeight = 0
          object CheckGroupClear: TRzCheckGroup
            Left = 7
            Top = 1
            Width = 636
            Height = 349
            GroupStyle = gsStandard
            Items.Strings = (
              #24080#21495#25968#25454' Account.DB'
              #35282#33394#25968#25454' RoleData.DB'
              #20010#20154#21830#24215' M2Data.DB'
              #26080#38480#20179#24211' M2Data.DB'
              #25293#21334#25968#25454' M2Data.DB'
              #27494#22120#21319#32423' Market_Upg\*.upg'
              #21830#24215#32531#23384' Market_prices\ Market_saved\'
              #34892#20250#25968#25454
              #27801#22478#25968#25454
              #24341#25806#26085#35760
              #29289#21697#26085#35760
              'G'#21464#37327#12289'A'#21464#37327
              #22810#24072#24466#20449#24687' MasterNo\'
              #22825#19979#31532#19968#38613#20687#20449#24687' Npc_Data\'
              #22269#23478#25968#25454' Nations\')
            TabOrder = 0
            CheckStates = (
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0)
          end
        end
        object ts1: TTabSheet
          Caption = #33258#23450#20041#25968#25454
          ExplicitLeft = 0
          ExplicitTop = 0
          ExplicitWidth = 0
          ExplicitHeight = 0
          object grp1: TGroupBox
            Left = 5
            Top = 5
            Width = 638
            Height = 111
            Caption = #28165#31354#33258#23450#20041#25991#26412#25968#25454'('#22914#20250#21592')'
            Font.Charset = GB2312_CHARSET
            Font.Color = clWindowText
            Font.Height = -12
            Font.Name = #23435#20307
            Font.Style = []
            ParentFont = False
            TabOrder = 0
            object lbl9: TLabel
              Left = 8
              Top = 21
              Width = 120
              Height = 12
              Caption = #28165#31354#33258#23450#20041#25991#20214#36335#24452#65306
              Font.Charset = GB2312_CHARSET
              Font.Color = clWindowText
              Font.Height = -12
              Font.Name = #23435#20307
              Font.Style = []
              ParentFont = False
            end
            object btnMyGetTxtDel: TRzRapidFireButton
              Left = 604
              Top = 15
              Width = 23
              Height = 22
              Glyph.Data = {
                36060000424D3606000000000000360400002800000020000000100000000100
                08000000000000020000830B0000830B00000001000000010000000000003300
                00006600000099000000CC000000FF0000000033000033330000663300009933
                0000CC330000FF33000000660000336600006666000099660000CC660000FF66
                000000990000339900006699000099990000CC990000FF99000000CC000033CC
                000066CC000099CC0000CCCC0000FFCC000000FF000033FF000066FF000099FF
                0000CCFF0000FFFF000000003300330033006600330099003300CC003300FF00
                330000333300333333006633330099333300CC333300FF333300006633003366
                33006666330099663300CC663300FF6633000099330033993300669933009999
                3300CC993300FF99330000CC330033CC330066CC330099CC3300CCCC3300FFCC
                330000FF330033FF330066FF330099FF3300CCFF3300FFFF3300000066003300
                66006600660099006600CC006600FF0066000033660033336600663366009933
                6600CC336600FF33660000666600336666006666660099666600CC666600FF66
                660000996600339966006699660099996600CC996600FF99660000CC660033CC
                660066CC660099CC6600CCCC6600FFCC660000FF660033FF660066FF660099FF
                6600CCFF6600FFFF660000009900330099006600990099009900CC009900FF00
                990000339900333399006633990099339900CC339900FF339900006699003366
                99006666990099669900CC669900FF6699000099990033999900669999009999
                9900CC999900FF99990000CC990033CC990066CC990099CC9900CCCC9900FFCC
                990000FF990033FF990066FF990099FF9900CCFF9900FFFF99000000CC003300
                CC006600CC009900CC00CC00CC00FF00CC000033CC003333CC006633CC009933
                CC00CC33CC00FF33CC000066CC003366CC006666CC009966CC00CC66CC00FF66
                CC000099CC003399CC006699CC009999CC00CC99CC00FF99CC0000CCCC0033CC
                CC0066CCCC0099CCCC00CCCCCC00FFCCCC0000FFCC0033FFCC0066FFCC0099FF
                CC00CCFFCC00FFFFCC000000FF003300FF006600FF009900FF00CC00FF00FF00
                FF000033FF003333FF006633FF009933FF00CC33FF00FF33FF000066FF003366
                FF006666FF009966FF00CC66FF00FF66FF000099FF003399FF006699FF009999
                FF00CC99FF00FF99FF0000CCFF0033CCFF0066CCFF0099CCFF00CCCCFF00FFCC
                FF0000FFFF0033FFFF0066FFFF0099FFFF00CCFFFF00FFFFFF00000080000080
                000000808000800000008000800080800000C0C0C00080808000191919004C4C
                4C00B2B2B200E5E5E500C8AC2800E0CC6600F2EABF00B59B2400D8E9EC009933
                6600D075A300ECC6D900646F710099A8AC00E2EFF10000000000000000000000
                0000000000000000000000000000000000000000000000000000000000000000
                0000000000000000000000000000000000000000000000000000E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E809090909
                0909090909090909E8E8E8E8818181818181818181818181E8E8E8E809101010
                1010101010101009E8E8E8E881ACACACACACACACACACAC81E8E8E8E809101010
                1010101010101009E8E8E8E881ACACACACACACACACACAC81E8E8E8E809090909
                0909090909090909E8E8E8E8818181818181818181818181E8E8E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8}
              NumGlyphs = 2
              OnClick = btnMyGetTxtDelClick
            end
            object btnMyGetTxtAdd: TRzRapidFireButton
              Left = 580
              Top = 15
              Width = 23
              Height = 22
              Glyph.Data = {
                36060000424D3606000000000000360400002800000020000000100000000100
                08000000000000020000830B0000830B00000001000000010000000000003300
                00006600000099000000CC000000FF0000000033000033330000663300009933
                0000CC330000FF33000000660000336600006666000099660000CC660000FF66
                000000990000339900006699000099990000CC990000FF99000000CC000033CC
                000066CC000099CC0000CCCC0000FFCC000000FF000033FF000066FF000099FF
                0000CCFF0000FFFF000000003300330033006600330099003300CC003300FF00
                330000333300333333006633330099333300CC333300FF333300006633003366
                33006666330099663300CC663300FF6633000099330033993300669933009999
                3300CC993300FF99330000CC330033CC330066CC330099CC3300CCCC3300FFCC
                330000FF330033FF330066FF330099FF3300CCFF3300FFFF3300000066003300
                66006600660099006600CC006600FF0066000033660033336600663366009933
                6600CC336600FF33660000666600336666006666660099666600CC666600FF66
                660000996600339966006699660099996600CC996600FF99660000CC660033CC
                660066CC660099CC6600CCCC6600FFCC660000FF660033FF660066FF660099FF
                6600CCFF6600FFFF660000009900330099006600990099009900CC009900FF00
                990000339900333399006633990099339900CC339900FF339900006699003366
                99006666990099669900CC669900FF6699000099990033999900669999009999
                9900CC999900FF99990000CC990033CC990066CC990099CC9900CCCC9900FFCC
                990000FF990033FF990066FF990099FF9900CCFF9900FFFF99000000CC003300
                CC006600CC009900CC00CC00CC00FF00CC000033CC003333CC006633CC009933
                CC00CC33CC00FF33CC000066CC003366CC006666CC009966CC00CC66CC00FF66
                CC000099CC003399CC006699CC009999CC00CC99CC00FF99CC0000CCCC0033CC
                CC0066CCCC0099CCCC00CCCCCC00FFCCCC0000FFCC0033FFCC0066FFCC0099FF
                CC00CCFFCC00FFFFCC000000FF003300FF006600FF009900FF00CC00FF00FF00
                FF000033FF003333FF006633FF009933FF00CC33FF00FF33FF000066FF003366
                FF006666FF009966FF00CC66FF00FF66FF000099FF003399FF006699FF009999
                FF00CC99FF00FF99FF0000CCFF0033CCFF0066CCFF0099CCFF00CCCCFF00FFCC
                FF0000FFFF0033FFFF0066FFFF0099FFFF00CCFFFF00FFFFFF00000080000080
                000000808000800000008000800080800000C0C0C00080808000191919004C4C
                4C00B2B2B200E5E5E500C8AC2800E0CC6600F2EABF00B59B2400D8E9EC009933
                6600D075A300ECC6D900646F710099A8AC00E2EFF10000000000000000000000
                0000000000000000000000000000000000000000000000000000000000000000
                0000000000000000000000000000000000000000000000000000E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8
                09090909E8E8E8E8E8E8E8E8E8E8E8E881818181E8E8E8E8E8E8E8E8E8E8E8E8
                09101009E8E8E8E8E8E8E8E8E8E8E8E881ACAC81E8E8E8E8E8E8E8E8E8E8E8E8
                09101009E8E8E8E8E8E8E8E8E8E8E8E881ACAC81E8E8E8E8E8E8E8E8E8E8E8E8
                09101009E8E8E8E8E8E8E8E8E8E8E8E881ACAC81E8E8E8E8E8E8E8E809090909
                0910100909090909E8E8E8E88181818181ACAC8181818181E8E8E8E809101010
                1010101010101009E8E8E8E881ACACACACACACACACACAC81E8E8E8E809101010
                1010101010101009E8E8E8E881ACACACACACACACACACAC81E8E8E8E809090909
                0910100909090909E8E8E8E88181818181ACAC8181818181E8E8E8E8E8E8E8E8
                09101009E8E8E8E8E8E8E8E8E8E8E8E881ACAC81E8E8E8E8E8E8E8E8E8E8E8E8
                09101009E8E8E8E8E8E8E8E8E8E8E8E881ACAC81E8E8E8E8E8E8E8E8E8E8E8E8
                09101009E8E8E8E8E8E8E8E8E8E8E8E881ACAC81E8E8E8E8E8E8E8E8E8E8E8E8
                09090909E8E8E8E8E8E8E8E8E8E8E8E881818181E8E8E8E8E8E8E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8}
              NumGlyphs = 2
              OnClick = btnMyGetTxtAddClick
            end
            object btnMyGetTxtOpen: TRzRapidFireButton
              Left = 554
              Top = 15
              Width = 23
              Height = 22
              Glyph.Data = {
                36060000424D3606000000000000360400002800000020000000100000000100
                08000000000000020000430B0000430B00000001000000010000000000003300
                00006600000099000000CC000000FF0000000033000033330000663300009933
                0000CC330000FF33000000660000336600006666000099660000CC660000FF66
                000000990000339900006699000099990000CC990000FF99000000CC000033CC
                000066CC000099CC0000CCCC0000FFCC000000FF000033FF000066FF000099FF
                0000CCFF0000FFFF000000003300330033006600330099003300CC003300FF00
                330000333300333333006633330099333300CC333300FF333300006633003366
                33006666330099663300CC663300FF6633000099330033993300669933009999
                3300CC993300FF99330000CC330033CC330066CC330099CC3300CCCC3300FFCC
                330000FF330033FF330066FF330099FF3300CCFF3300FFFF3300000066003300
                66006600660099006600CC006600FF0066000033660033336600663366009933
                6600CC336600FF33660000666600336666006666660099666600CC666600FF66
                660000996600339966006699660099996600CC996600FF99660000CC660033CC
                660066CC660099CC6600CCCC6600FFCC660000FF660033FF660066FF660099FF
                6600CCFF6600FFFF660000009900330099006600990099009900CC009900FF00
                990000339900333399006633990099339900CC339900FF339900006699003366
                99006666990099669900CC669900FF6699000099990033999900669999009999
                9900CC999900FF99990000CC990033CC990066CC990099CC9900CCCC9900FFCC
                990000FF990033FF990066FF990099FF9900CCFF9900FFFF99000000CC003300
                CC006600CC009900CC00CC00CC00FF00CC000033CC003333CC006633CC009933
                CC00CC33CC00FF33CC000066CC003366CC006666CC009966CC00CC66CC00FF66
                CC000099CC003399CC006699CC009999CC00CC99CC00FF99CC0000CCCC0033CC
                CC0066CCCC0099CCCC00CCCCCC00FFCCCC0000FFCC0033FFCC0066FFCC0099FF
                CC00CCFFCC00FFFFCC000000FF003300FF006600FF009900FF00CC00FF00FF00
                FF000033FF003333FF006633FF009933FF00CC33FF00FF33FF000066FF003366
                FF006666FF009966FF00CC66FF00FF66FF000099FF003399FF006699FF009999
                FF00CC99FF00FF99FF0000CCFF0033CCFF0066CCFF0099CCFF00CCCCFF00FFCC
                FF0000FFFF0033FFFF0066FFFF0099FFFF00CCFFFF00FFFFFF00000080000080
                000000808000800000008000800080800000C0C0C00080808000191919004C4C
                4C00B2B2B200E5E5E500C8AC2800E0CC6600F2EABF00B59B2400D8E9EC009933
                6600D075A300ECC6D900646F710099A8AC00E2EFF10000000000000000000000
                0000000000000000000000000000000000000000000000000000000000000000
                0000000000000000000000000000000000000000000000000000E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8A378787878
                787878787878AAE8E8E8E88181818181818181818181ACE8E8E8A3A3D5CECECE
                CECECECECEA378E8E8E88181E3ACACACACACACACAC8181E8E8E8A3A3CED5D5D5
                D5D5D5D5D5CE78A3E8E88181ACE3E3E3E3E3E3E3E3AC8181E8E8A3A3CED5D5D5
                D5D5D5D5D5CEAA78E8E88181ACE3E3E3E3E3E3E3E3ACAC81E8E8A3CEA3D5D5D5
                D5D5D5D5D5CED578A3E881AC81E3E3E3E3E3E3E3E3ACE38181E8A3CEAAAAD5D5
                D5D5D5D5D5CED5AA78E881ACACACE3E3E3E3E3E3E3ACE3AC81E8A3D5CEA3D6D6
                D6D6D6D6D6D5D6D678E881E3AC81E3E3E3E3E3E3E3E3E3E381E8A3D5D5CEA3A3
                A3A3A3A3A3A3A3A3CEE881E3E3AC81818181818181818181ACE8A3D6D5D5D5D5
                D6D6D6D6D678E8E8E8E881E3E3E3E3E3E3E3E3E3E381E8E8E8E8E8A3D6D6D6D6
                A3A3A3A3A3E8E8E8E8E8E881E3E3E3E38181818181E8E8E8E8E8E8E8A3A3A3A3
                E8E8E8E8E8E8E8090909E8E881818181E8E8E8E8E8E8E8818181E8E8E8E8E8E8
                E8E8E8E8E8E8E8E80909E8E8E8E8E8E8E8E8E8E8E8E8E8E88181E8E8E8E8E8E8
                E8E8E809E8E8E809E809E8E8E8E8E8E8E8E8E881E8E8E881E881E8E8E8E8E8E8
                E8E8E8E8090909E8E8E8E8E8E8E8E8E8E8E8E8E8818181E8E8E8}
              NumGlyphs = 2
              OnClick = btnMyGetTxtOpenClick
            end
            object edtMyGetTXT: TEdit
              Left = 128
              Top = 17
              Width = 417
              Height = 20
              Font.Charset = GB2312_CHARSET
              Font.Color = clWindowText
              Font.Height = -12
              Font.Name = #23435#20307
              Font.Style = []
              ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
              ParentFont = False
              TabOrder = 0
              Text = #36825#37324#36873#25321#20320#35201#28165#31354#30340#25991#20214#36335#24452#28155#21152#36827#21015#34920
            end
            object lstMyGetTXT: TListBox
              Left = 10
              Top = 40
              Width = 618
              Height = 62
              Hint = #25152#26377#24453#28165#31354#25968#25454#30340#21015#34920
              ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
              ItemHeight = 12
              MultiSelect = True
              ParentShowHint = False
              ShowHint = True
              TabOrder = 1
            end
          end
          object grp2: TGroupBox
            Left = 5
            Top = 122
            Width = 638
            Height = 111
            Caption = #21024#38500#33258#23450#20041#25991#20214#25968#25454
            Font.Charset = GB2312_CHARSET
            Font.Color = clWindowText
            Font.Height = -12
            Font.Name = #23435#20307
            Font.Style = []
            ParentFont = False
            TabOrder = 1
            object lbl10: TLabel
              Left = 8
              Top = 20
              Width = 120
              Height = 12
              Caption = #21024#38500#33258#23450#20041#25991#20214#36335#24452#65306
            end
            object btnMyGetFileOpen: TRzRapidFireButton
              Left = 553
              Top = 14
              Width = 23
              Height = 22
              Glyph.Data = {
                36060000424D3606000000000000360400002800000020000000100000000100
                08000000000000020000430B0000430B00000001000000010000000000003300
                00006600000099000000CC000000FF0000000033000033330000663300009933
                0000CC330000FF33000000660000336600006666000099660000CC660000FF66
                000000990000339900006699000099990000CC990000FF99000000CC000033CC
                000066CC000099CC0000CCCC0000FFCC000000FF000033FF000066FF000099FF
                0000CCFF0000FFFF000000003300330033006600330099003300CC003300FF00
                330000333300333333006633330099333300CC333300FF333300006633003366
                33006666330099663300CC663300FF6633000099330033993300669933009999
                3300CC993300FF99330000CC330033CC330066CC330099CC3300CCCC3300FFCC
                330000FF330033FF330066FF330099FF3300CCFF3300FFFF3300000066003300
                66006600660099006600CC006600FF0066000033660033336600663366009933
                6600CC336600FF33660000666600336666006666660099666600CC666600FF66
                660000996600339966006699660099996600CC996600FF99660000CC660033CC
                660066CC660099CC6600CCCC6600FFCC660000FF660033FF660066FF660099FF
                6600CCFF6600FFFF660000009900330099006600990099009900CC009900FF00
                990000339900333399006633990099339900CC339900FF339900006699003366
                99006666990099669900CC669900FF6699000099990033999900669999009999
                9900CC999900FF99990000CC990033CC990066CC990099CC9900CCCC9900FFCC
                990000FF990033FF990066FF990099FF9900CCFF9900FFFF99000000CC003300
                CC006600CC009900CC00CC00CC00FF00CC000033CC003333CC006633CC009933
                CC00CC33CC00FF33CC000066CC003366CC006666CC009966CC00CC66CC00FF66
                CC000099CC003399CC006699CC009999CC00CC99CC00FF99CC0000CCCC0033CC
                CC0066CCCC0099CCCC00CCCCCC00FFCCCC0000FFCC0033FFCC0066FFCC0099FF
                CC00CCFFCC00FFFFCC000000FF003300FF006600FF009900FF00CC00FF00FF00
                FF000033FF003333FF006633FF009933FF00CC33FF00FF33FF000066FF003366
                FF006666FF009966FF00CC66FF00FF66FF000099FF003399FF006699FF009999
                FF00CC99FF00FF99FF0000CCFF0033CCFF0066CCFF0099CCFF00CCCCFF00FFCC
                FF0000FFFF0033FFFF0066FFFF0099FFFF00CCFFFF00FFFFFF00000080000080
                000000808000800000008000800080800000C0C0C00080808000191919004C4C
                4C00B2B2B200E5E5E500C8AC2800E0CC6600F2EABF00B59B2400D8E9EC009933
                6600D075A300ECC6D900646F710099A8AC00E2EFF10000000000000000000000
                0000000000000000000000000000000000000000000000000000000000000000
                0000000000000000000000000000000000000000000000000000E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8A378787878
                787878787878AAE8E8E8E88181818181818181818181ACE8E8E8A3A3D5CECECE
                CECECECECEA378E8E8E88181E3ACACACACACACACAC8181E8E8E8A3A3CED5D5D5
                D5D5D5D5D5CE78A3E8E88181ACE3E3E3E3E3E3E3E3AC8181E8E8A3A3CED5D5D5
                D5D5D5D5D5CEAA78E8E88181ACE3E3E3E3E3E3E3E3ACAC81E8E8A3CEA3D5D5D5
                D5D5D5D5D5CED578A3E881AC81E3E3E3E3E3E3E3E3ACE38181E8A3CEAAAAD5D5
                D5D5D5D5D5CED5AA78E881ACACACE3E3E3E3E3E3E3ACE3AC81E8A3D5CEA3D6D6
                D6D6D6D6D6D5D6D678E881E3AC81E3E3E3E3E3E3E3E3E3E381E8A3D5D5CEA3A3
                A3A3A3A3A3A3A3A3CEE881E3E3AC81818181818181818181ACE8A3D6D5D5D5D5
                D6D6D6D6D678E8E8E8E881E3E3E3E3E3E3E3E3E3E381E8E8E8E8E8A3D6D6D6D6
                A3A3A3A3A3E8E8E8E8E8E881E3E3E3E38181818181E8E8E8E8E8E8E8A3A3A3A3
                E8E8E8E8E8E8E8090909E8E881818181E8E8E8E8E8E8E8818181E8E8E8E8E8E8
                E8E8E8E8E8E8E8E80909E8E8E8E8E8E8E8E8E8E8E8E8E8E88181E8E8E8E8E8E8
                E8E8E809E8E8E809E809E8E8E8E8E8E8E8E8E881E8E8E881E881E8E8E8E8E8E8
                E8E8E8E8090909E8E8E8E8E8E8E8E8E8E8E8E8E8818181E8E8E8}
              NumGlyphs = 2
              OnClick = btnMyGetTxtOpenClick
            end
            object btnMyGetFileAdd: TRzRapidFireButton
              Left = 579
              Top = 14
              Width = 23
              Height = 22
              Glyph.Data = {
                36060000424D3606000000000000360400002800000020000000100000000100
                08000000000000020000830B0000830B00000001000000010000000000003300
                00006600000099000000CC000000FF0000000033000033330000663300009933
                0000CC330000FF33000000660000336600006666000099660000CC660000FF66
                000000990000339900006699000099990000CC990000FF99000000CC000033CC
                000066CC000099CC0000CCCC0000FFCC000000FF000033FF000066FF000099FF
                0000CCFF0000FFFF000000003300330033006600330099003300CC003300FF00
                330000333300333333006633330099333300CC333300FF333300006633003366
                33006666330099663300CC663300FF6633000099330033993300669933009999
                3300CC993300FF99330000CC330033CC330066CC330099CC3300CCCC3300FFCC
                330000FF330033FF330066FF330099FF3300CCFF3300FFFF3300000066003300
                66006600660099006600CC006600FF0066000033660033336600663366009933
                6600CC336600FF33660000666600336666006666660099666600CC666600FF66
                660000996600339966006699660099996600CC996600FF99660000CC660033CC
                660066CC660099CC6600CCCC6600FFCC660000FF660033FF660066FF660099FF
                6600CCFF6600FFFF660000009900330099006600990099009900CC009900FF00
                990000339900333399006633990099339900CC339900FF339900006699003366
                99006666990099669900CC669900FF6699000099990033999900669999009999
                9900CC999900FF99990000CC990033CC990066CC990099CC9900CCCC9900FFCC
                990000FF990033FF990066FF990099FF9900CCFF9900FFFF99000000CC003300
                CC006600CC009900CC00CC00CC00FF00CC000033CC003333CC006633CC009933
                CC00CC33CC00FF33CC000066CC003366CC006666CC009966CC00CC66CC00FF66
                CC000099CC003399CC006699CC009999CC00CC99CC00FF99CC0000CCCC0033CC
                CC0066CCCC0099CCCC00CCCCCC00FFCCCC0000FFCC0033FFCC0066FFCC0099FF
                CC00CCFFCC00FFFFCC000000FF003300FF006600FF009900FF00CC00FF00FF00
                FF000033FF003333FF006633FF009933FF00CC33FF00FF33FF000066FF003366
                FF006666FF009966FF00CC66FF00FF66FF000099FF003399FF006699FF009999
                FF00CC99FF00FF99FF0000CCFF0033CCFF0066CCFF0099CCFF00CCCCFF00FFCC
                FF0000FFFF0033FFFF0066FFFF0099FFFF00CCFFFF00FFFFFF00000080000080
                000000808000800000008000800080800000C0C0C00080808000191919004C4C
                4C00B2B2B200E5E5E500C8AC2800E0CC6600F2EABF00B59B2400D8E9EC009933
                6600D075A300ECC6D900646F710099A8AC00E2EFF10000000000000000000000
                0000000000000000000000000000000000000000000000000000000000000000
                0000000000000000000000000000000000000000000000000000E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8
                09090909E8E8E8E8E8E8E8E8E8E8E8E881818181E8E8E8E8E8E8E8E8E8E8E8E8
                09101009E8E8E8E8E8E8E8E8E8E8E8E881ACAC81E8E8E8E8E8E8E8E8E8E8E8E8
                09101009E8E8E8E8E8E8E8E8E8E8E8E881ACAC81E8E8E8E8E8E8E8E8E8E8E8E8
                09101009E8E8E8E8E8E8E8E8E8E8E8E881ACAC81E8E8E8E8E8E8E8E809090909
                0910100909090909E8E8E8E88181818181ACAC8181818181E8E8E8E809101010
                1010101010101009E8E8E8E881ACACACACACACACACACAC81E8E8E8E809101010
                1010101010101009E8E8E8E881ACACACACACACACACACAC81E8E8E8E809090909
                0910100909090909E8E8E8E88181818181ACAC8181818181E8E8E8E8E8E8E8E8
                09101009E8E8E8E8E8E8E8E8E8E8E8E881ACAC81E8E8E8E8E8E8E8E8E8E8E8E8
                09101009E8E8E8E8E8E8E8E8E8E8E8E881ACAC81E8E8E8E8E8E8E8E8E8E8E8E8
                09101009E8E8E8E8E8E8E8E8E8E8E8E881ACAC81E8E8E8E8E8E8E8E8E8E8E8E8
                09090909E8E8E8E8E8E8E8E8E8E8E8E881818181E8E8E8E8E8E8E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8}
              NumGlyphs = 2
              OnClick = btnMyGetTxtAddClick
            end
            object btnMyGetFileDel: TRzRapidFireButton
              Left = 603
              Top = 14
              Width = 23
              Height = 22
              Glyph.Data = {
                36060000424D3606000000000000360400002800000020000000100000000100
                08000000000000020000830B0000830B00000001000000010000000000003300
                00006600000099000000CC000000FF0000000033000033330000663300009933
                0000CC330000FF33000000660000336600006666000099660000CC660000FF66
                000000990000339900006699000099990000CC990000FF99000000CC000033CC
                000066CC000099CC0000CCCC0000FFCC000000FF000033FF000066FF000099FF
                0000CCFF0000FFFF000000003300330033006600330099003300CC003300FF00
                330000333300333333006633330099333300CC333300FF333300006633003366
                33006666330099663300CC663300FF6633000099330033993300669933009999
                3300CC993300FF99330000CC330033CC330066CC330099CC3300CCCC3300FFCC
                330000FF330033FF330066FF330099FF3300CCFF3300FFFF3300000066003300
                66006600660099006600CC006600FF0066000033660033336600663366009933
                6600CC336600FF33660000666600336666006666660099666600CC666600FF66
                660000996600339966006699660099996600CC996600FF99660000CC660033CC
                660066CC660099CC6600CCCC6600FFCC660000FF660033FF660066FF660099FF
                6600CCFF6600FFFF660000009900330099006600990099009900CC009900FF00
                990000339900333399006633990099339900CC339900FF339900006699003366
                99006666990099669900CC669900FF6699000099990033999900669999009999
                9900CC999900FF99990000CC990033CC990066CC990099CC9900CCCC9900FFCC
                990000FF990033FF990066FF990099FF9900CCFF9900FFFF99000000CC003300
                CC006600CC009900CC00CC00CC00FF00CC000033CC003333CC006633CC009933
                CC00CC33CC00FF33CC000066CC003366CC006666CC009966CC00CC66CC00FF66
                CC000099CC003399CC006699CC009999CC00CC99CC00FF99CC0000CCCC0033CC
                CC0066CCCC0099CCCC00CCCCCC00FFCCCC0000FFCC0033FFCC0066FFCC0099FF
                CC00CCFFCC00FFFFCC000000FF003300FF006600FF009900FF00CC00FF00FF00
                FF000033FF003333FF006633FF009933FF00CC33FF00FF33FF000066FF003366
                FF006666FF009966FF00CC66FF00FF66FF000099FF003399FF006699FF009999
                FF00CC99FF00FF99FF0000CCFF0033CCFF0066CCFF0099CCFF00CCCCFF00FFCC
                FF0000FFFF0033FFFF0066FFFF0099FFFF00CCFFFF00FFFFFF00000080000080
                000000808000800000008000800080800000C0C0C00080808000191919004C4C
                4C00B2B2B200E5E5E500C8AC2800E0CC6600F2EABF00B59B2400D8E9EC009933
                6600D075A300ECC6D900646F710099A8AC00E2EFF10000000000000000000000
                0000000000000000000000000000000000000000000000000000000000000000
                0000000000000000000000000000000000000000000000000000E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E809090909
                0909090909090909E8E8E8E8818181818181818181818181E8E8E8E809101010
                1010101010101009E8E8E8E881ACACACACACACACACACAC81E8E8E8E809101010
                1010101010101009E8E8E8E881ACACACACACACACACACAC81E8E8E8E809090909
                0909090909090909E8E8E8E8818181818181818181818181E8E8E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8}
              NumGlyphs = 2
              OnClick = btnMyGetTxtDelClick
            end
            object edtMyGetFile: TEdit
              Left = 128
              Top = 17
              Width = 417
              Height = 20
              ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
              TabOrder = 0
              Text = #36825#37324#36873#25321#20320#35201#21024#38500#30340#25991#20214#36335#24452#28155#21152#36827#21015#34920
            end
            object lstMyGetFile: TListBox
              Left = 8
              Top = 40
              Width = 618
              Height = 62
              Hint = #25152#26377#24453#21024#38500#30340#25991#20214#25968#25454#30340#21015#34920
              ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
              ItemHeight = 12
              MultiSelect = True
              ParentShowHint = False
              ShowHint = True
              TabOrder = 1
            end
          end
          object grp3: TGroupBox
            Left = 5
            Top = 241
            Width = 637
            Height = 111
            Caption = #28165#31354#33258#23450#20041#30446#24405#25968#25454
            Font.Charset = GB2312_CHARSET
            Font.Color = clWindowText
            Font.Height = -12
            Font.Name = #23435#20307
            Font.Style = []
            ParentFont = False
            TabOrder = 2
            object lbl11: TLabel
              Left = 8
              Top = 19
              Width = 120
              Height = 12
              Caption = #28165#31354#33258#23450#20041#30446#24405#36335#24452#65306
            end
            object btnMyGetDirOpen: TRzRapidFireButton
              Left = 552
              Top = 13
              Width = 23
              Height = 22
              Glyph.Data = {
                36060000424D3606000000000000360400002800000020000000100000000100
                08000000000000020000420B0000420B00000001000000010000000000003300
                00006600000099000000CC000000FF0000000033000033330000663300009933
                0000CC330000FF33000000660000336600006666000099660000CC660000FF66
                000000990000339900006699000099990000CC990000FF99000000CC000033CC
                000066CC000099CC0000CCCC0000FFCC000000FF000033FF000066FF000099FF
                0000CCFF0000FFFF000000003300330033006600330099003300CC003300FF00
                330000333300333333006633330099333300CC333300FF333300006633003366
                33006666330099663300CC663300FF6633000099330033993300669933009999
                3300CC993300FF99330000CC330033CC330066CC330099CC3300CCCC3300FFCC
                330000FF330033FF330066FF330099FF3300CCFF3300FFFF3300000066003300
                66006600660099006600CC006600FF0066000033660033336600663366009933
                6600CC336600FF33660000666600336666006666660099666600CC666600FF66
                660000996600339966006699660099996600CC996600FF99660000CC660033CC
                660066CC660099CC6600CCCC6600FFCC660000FF660033FF660066FF660099FF
                6600CCFF6600FFFF660000009900330099006600990099009900CC009900FF00
                990000339900333399006633990099339900CC339900FF339900006699003366
                99006666990099669900CC669900FF6699000099990033999900669999009999
                9900CC999900FF99990000CC990033CC990066CC990099CC9900CCCC9900FFCC
                990000FF990033FF990066FF990099FF9900CCFF9900FFFF99000000CC003300
                CC006600CC009900CC00CC00CC00FF00CC000033CC003333CC006633CC009933
                CC00CC33CC00FF33CC000066CC003366CC006666CC009966CC00CC66CC00FF66
                CC000099CC003399CC006699CC009999CC00CC99CC00FF99CC0000CCCC0033CC
                CC0066CCCC0099CCCC00CCCCCC00FFCCCC0000FFCC0033FFCC0066FFCC0099FF
                CC00CCFFCC00FFFFCC000000FF003300FF006600FF009900FF00CC00FF00FF00
                FF000033FF003333FF006633FF009933FF00CC33FF00FF33FF000066FF003366
                FF006666FF009966FF00CC66FF00FF66FF000099FF003399FF006699FF009999
                FF00CC99FF00FF99FF0000CCFF0033CCFF0066CCFF0099CCFF00CCCCFF00FFCC
                FF0000FFFF0033FFFF0066FFFF0099FFFF00CCFFFF00FFFFFF00000080000080
                000000808000800000008000800080800000C0C0C00080808000191919004C4C
                4C00B2B2B200E5E5E500C8AC2800E0CC6600F2EABF00B59B2400D8E9EC009933
                6600D075A300ECC6D900646F710099A8AC00E2EFF10000000000000000000000
                0000000000000000000000000000000000000000000000000000000000000000
                0000000000000000000000000000000000000000000000000000E8E8E8E8E8E8
                E8E8A3CEA3D5D5D5D5D5E8E8E8E8E8E8E8E881AC81E3E3E3E3E3E8E8E8E8E8E8
                E8E8A3CEA3D6D6D6D6D6E8E8E8E8E8E8E8E881AC81E3E3E3E3E3E8E8E8E8E8E8
                E8E8A3D5D5A3A3A3A3A3E8E8E8E8E8E8E8E881E3E38181818181E8E8E8E8DFE8
                DFE8A3D6D5D5D5D6D6D6E8E8E8E8DFE8DFE881E3E3E3E3E3E3E3E8E8E8E8E8E8
                E8E8E8A3D6D6D6A3A3A3E8E8E8E8E8E8E8E8E881E3E3E3818181E8E8E8E8DFE8
                E8E8E8E8A3A3A3E8E8E8E8E8E8E8DFE8E8E8E8E8818181E8E8E8E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8A378787878
                78787878E8E8E8E8E8E8E8818181818181818181E8E8E8E8E8E8A3CEA3D5CECE
                CECEA3D578E8E8E8E8E881AC81E3ACACACAC81E381E8E8E8E8E8A3CEA3D5D5D5
                D5D5CED578E8E8E8E8E881AC81E3E3E3E3E3ACE381E8E8E8E8E8A3CEA3D5D5D5
                D5D5CED578E8E8E8E8E881AC81E3E3E3E3E3ACE381E8E8E8E8E8A3CEA3D6D6D6
                D6D6D5D8D8D8D8D8E8E881AC81E3E3E3E3E3E38181818181E8E8A3D5D5A3A3A3
                A3A3A3A3D8D8D8D8E8E881E3E38181818181818181818181E8E8A3D6D5D5D5D6
                D6D678E8E8D8D8D8D8E881E3E3E3E3E3E3E381E8E881818181E8E8A3D6D6D6A3
                A3A3E8E8E8E8D8E8D8D8E881E3E3E3818181E8E8E8E881E88181E8E8A3A3A3E8
                E8E8E8E8E8E8E8E8E8D8E8E8818181E8E8E8E8E8E8E8E8E8E881}
              NumGlyphs = 2
              OnClick = btnMyGetTxtOpenClick
            end
            object btnMyGetDirAdd: TRzRapidFireButton
              Left = 578
              Top = 13
              Width = 23
              Height = 22
              Glyph.Data = {
                36060000424D3606000000000000360400002800000020000000100000000100
                08000000000000020000830B0000830B00000001000000010000000000003300
                00006600000099000000CC000000FF0000000033000033330000663300009933
                0000CC330000FF33000000660000336600006666000099660000CC660000FF66
                000000990000339900006699000099990000CC990000FF99000000CC000033CC
                000066CC000099CC0000CCCC0000FFCC000000FF000033FF000066FF000099FF
                0000CCFF0000FFFF000000003300330033006600330099003300CC003300FF00
                330000333300333333006633330099333300CC333300FF333300006633003366
                33006666330099663300CC663300FF6633000099330033993300669933009999
                3300CC993300FF99330000CC330033CC330066CC330099CC3300CCCC3300FFCC
                330000FF330033FF330066FF330099FF3300CCFF3300FFFF3300000066003300
                66006600660099006600CC006600FF0066000033660033336600663366009933
                6600CC336600FF33660000666600336666006666660099666600CC666600FF66
                660000996600339966006699660099996600CC996600FF99660000CC660033CC
                660066CC660099CC6600CCCC6600FFCC660000FF660033FF660066FF660099FF
                6600CCFF6600FFFF660000009900330099006600990099009900CC009900FF00
                990000339900333399006633990099339900CC339900FF339900006699003366
                99006666990099669900CC669900FF6699000099990033999900669999009999
                9900CC999900FF99990000CC990033CC990066CC990099CC9900CCCC9900FFCC
                990000FF990033FF990066FF990099FF9900CCFF9900FFFF99000000CC003300
                CC006600CC009900CC00CC00CC00FF00CC000033CC003333CC006633CC009933
                CC00CC33CC00FF33CC000066CC003366CC006666CC009966CC00CC66CC00FF66
                CC000099CC003399CC006699CC009999CC00CC99CC00FF99CC0000CCCC0033CC
                CC0066CCCC0099CCCC00CCCCCC00FFCCCC0000FFCC0033FFCC0066FFCC0099FF
                CC00CCFFCC00FFFFCC000000FF003300FF006600FF009900FF00CC00FF00FF00
                FF000033FF003333FF006633FF009933FF00CC33FF00FF33FF000066FF003366
                FF006666FF009966FF00CC66FF00FF66FF000099FF003399FF006699FF009999
                FF00CC99FF00FF99FF0000CCFF0033CCFF0066CCFF0099CCFF00CCCCFF00FFCC
                FF0000FFFF0033FFFF0066FFFF0099FFFF00CCFFFF00FFFFFF00000080000080
                000000808000800000008000800080800000C0C0C00080808000191919004C4C
                4C00B2B2B200E5E5E500C8AC2800E0CC6600F2EABF00B59B2400D8E9EC009933
                6600D075A300ECC6D900646F710099A8AC00E2EFF10000000000000000000000
                0000000000000000000000000000000000000000000000000000000000000000
                0000000000000000000000000000000000000000000000000000E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8
                09090909E8E8E8E8E8E8E8E8E8E8E8E881818181E8E8E8E8E8E8E8E8E8E8E8E8
                09101009E8E8E8E8E8E8E8E8E8E8E8E881ACAC81E8E8E8E8E8E8E8E8E8E8E8E8
                09101009E8E8E8E8E8E8E8E8E8E8E8E881ACAC81E8E8E8E8E8E8E8E8E8E8E8E8
                09101009E8E8E8E8E8E8E8E8E8E8E8E881ACAC81E8E8E8E8E8E8E8E809090909
                0910100909090909E8E8E8E88181818181ACAC8181818181E8E8E8E809101010
                1010101010101009E8E8E8E881ACACACACACACACACACAC81E8E8E8E809101010
                1010101010101009E8E8E8E881ACACACACACACACACACAC81E8E8E8E809090909
                0910100909090909E8E8E8E88181818181ACAC8181818181E8E8E8E8E8E8E8E8
                09101009E8E8E8E8E8E8E8E8E8E8E8E881ACAC81E8E8E8E8E8E8E8E8E8E8E8E8
                09101009E8E8E8E8E8E8E8E8E8E8E8E881ACAC81E8E8E8E8E8E8E8E8E8E8E8E8
                09101009E8E8E8E8E8E8E8E8E8E8E8E881ACAC81E8E8E8E8E8E8E8E8E8E8E8E8
                09090909E8E8E8E8E8E8E8E8E8E8E8E881818181E8E8E8E8E8E8E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8}
              NumGlyphs = 2
              OnClick = btnMyGetTxtAddClick
            end
            object btnMyGetDirDel: TRzRapidFireButton
              Left = 602
              Top = 13
              Width = 23
              Height = 22
              Glyph.Data = {
                36060000424D3606000000000000360400002800000020000000100000000100
                08000000000000020000830B0000830B00000001000000010000000000003300
                00006600000099000000CC000000FF0000000033000033330000663300009933
                0000CC330000FF33000000660000336600006666000099660000CC660000FF66
                000000990000339900006699000099990000CC990000FF99000000CC000033CC
                000066CC000099CC0000CCCC0000FFCC000000FF000033FF000066FF000099FF
                0000CCFF0000FFFF000000003300330033006600330099003300CC003300FF00
                330000333300333333006633330099333300CC333300FF333300006633003366
                33006666330099663300CC663300FF6633000099330033993300669933009999
                3300CC993300FF99330000CC330033CC330066CC330099CC3300CCCC3300FFCC
                330000FF330033FF330066FF330099FF3300CCFF3300FFFF3300000066003300
                66006600660099006600CC006600FF0066000033660033336600663366009933
                6600CC336600FF33660000666600336666006666660099666600CC666600FF66
                660000996600339966006699660099996600CC996600FF99660000CC660033CC
                660066CC660099CC6600CCCC6600FFCC660000FF660033FF660066FF660099FF
                6600CCFF6600FFFF660000009900330099006600990099009900CC009900FF00
                990000339900333399006633990099339900CC339900FF339900006699003366
                99006666990099669900CC669900FF6699000099990033999900669999009999
                9900CC999900FF99990000CC990033CC990066CC990099CC9900CCCC9900FFCC
                990000FF990033FF990066FF990099FF9900CCFF9900FFFF99000000CC003300
                CC006600CC009900CC00CC00CC00FF00CC000033CC003333CC006633CC009933
                CC00CC33CC00FF33CC000066CC003366CC006666CC009966CC00CC66CC00FF66
                CC000099CC003399CC006699CC009999CC00CC99CC00FF99CC0000CCCC0033CC
                CC0066CCCC0099CCCC00CCCCCC00FFCCCC0000FFCC0033FFCC0066FFCC0099FF
                CC00CCFFCC00FFFFCC000000FF003300FF006600FF009900FF00CC00FF00FF00
                FF000033FF003333FF006633FF009933FF00CC33FF00FF33FF000066FF003366
                FF006666FF009966FF00CC66FF00FF66FF000099FF003399FF006699FF009999
                FF00CC99FF00FF99FF0000CCFF0033CCFF0066CCFF0099CCFF00CCCCFF00FFCC
                FF0000FFFF0033FFFF0066FFFF0099FFFF00CCFFFF00FFFFFF00000080000080
                000000808000800000008000800080800000C0C0C00080808000191919004C4C
                4C00B2B2B200E5E5E500C8AC2800E0CC6600F2EABF00B59B2400D8E9EC009933
                6600D075A300ECC6D900646F710099A8AC00E2EFF10000000000000000000000
                0000000000000000000000000000000000000000000000000000000000000000
                0000000000000000000000000000000000000000000000000000E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E809090909
                0909090909090909E8E8E8E8818181818181818181818181E8E8E8E809101010
                1010101010101009E8E8E8E881ACACACACACACACACACAC81E8E8E8E809101010
                1010101010101009E8E8E8E881ACACACACACACACACACAC81E8E8E8E809090909
                0909090909090909E8E8E8E8818181818181818181818181E8E8E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8
                E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8}
              NumGlyphs = 2
              OnClick = btnMyGetTxtDelClick
            end
            object edtMyGetDir: TEdit
              Left = 128
              Top = 16
              Width = 417
              Height = 20
              ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
              TabOrder = 0
              Text = #36825#37324#36873#25321#20320#35201#28165#31354#30340#30446#24405#36335#24452#28155#21152#36827#21015#34920
            end
            object lstMyGetDir: TListBox
              Left = 7
              Top = 39
              Width = 618
              Height = 62
              Hint = #25152#26377#24453#28165#31354#30340#30446#24405#25968#25454#30340#21015#34920
              ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
              ItemHeight = 12
              MultiSelect = True
              ParentShowHint = False
              ShowHint = True
              TabOrder = 1
            end
          end
        end
      end
      object btnStartClear: TRzBitBtn
        Left = 570
        Top = 388
        Width = 81
        Caption = #24320#22987#28165#29702
        TabOrder = 1
        OnClick = btnStartClearClick
        Glyph.Data = {
          36060000424D3606000000000000360400002800000020000000100000000100
          08000000000000020000E30E0000E30E00000001000000000000000000003300
          00006600000099000000CC000000FF0000000033000033330000663300009933
          0000CC330000FF33000000660000336600006666000099660000CC660000FF66
          000000990000339900006699000099990000CC990000FF99000000CC000033CC
          000066CC000099CC0000CCCC0000FFCC000000FF000033FF000066FF000099FF
          0000CCFF0000FFFF000000003300330033006600330099003300CC003300FF00
          330000333300333333006633330099333300CC333300FF333300006633003366
          33006666330099663300CC663300FF6633000099330033993300669933009999
          3300CC993300FF99330000CC330033CC330066CC330099CC3300CCCC3300FFCC
          330000FF330033FF330066FF330099FF3300CCFF3300FFFF3300000066003300
          66006600660099006600CC006600FF0066000033660033336600663366009933
          6600CC336600FF33660000666600336666006666660099666600CC666600FF66
          660000996600339966006699660099996600CC996600FF99660000CC660033CC
          660066CC660099CC6600CCCC6600FFCC660000FF660033FF660066FF660099FF
          6600CCFF6600FFFF660000009900330099006600990099009900CC009900FF00
          990000339900333399006633990099339900CC339900FF339900006699003366
          99006666990099669900CC669900FF6699000099990033999900669999009999
          9900CC999900FF99990000CC990033CC990066CC990099CC9900CCCC9900FFCC
          990000FF990033FF990066FF990099FF9900CCFF9900FFFF99000000CC003300
          CC006600CC009900CC00CC00CC00FF00CC000033CC003333CC006633CC009933
          CC00CC33CC00FF33CC000066CC003366CC006666CC009966CC00CC66CC00FF66
          CC000099CC003399CC006699CC009999CC00CC99CC00FF99CC0000CCCC0033CC
          CC0066CCCC0099CCCC00CCCCCC00FFCCCC0000FFCC0033FFCC0066FFCC0099FF
          CC00CCFFCC00FFFFCC000000FF003300FF006600FF009900FF00CC00FF00FF00
          FF000033FF003333FF006633FF009933FF00CC33FF00FF33FF000066FF003366
          FF006666FF009966FF00CC66FF00FF66FF000099FF003399FF006699FF009999
          FF00CC99FF00FF99FF0000CCFF0033CCFF0066CCFF0099CCFF00CCCCFF00FFCC
          FF0000FFFF0033FFFF0066FFFF0099FFFF00CCFFFF00FFFFFF00000080000080
          000000808000800000008000800080800000C0C0C00080808000191919004C4C
          4C00B2B2B200E5E5E500C8AC2800E0CC6600F2EABF00B59B2400D8E9EC009933
          6600D075A300ECC6D900646F710099A8AC00E2EFF10000000000000000000000
          0000000000000000000000000000000000000000000000000000000000000000
          0000000000000000000000000000000000000000000000000000E8E8E8E8E8E8
          E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8
          E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8
          E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8
          E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8121212
          12121212121212E8E8E8E8E8E881818181818181818181E8E8E8E8E812181818
          1818121812121212E8E8E8E881E2E2E2E2E281E281818181E8E8E8E8121E1818
          1818181218121212E8E8E8E881ACE2E2E2E2E281E2818181E8E8E8E812181E18
          1818181812181212E8E8E8E881E2ACE2E2E2E2E281E28181E8E8E8E8121E181E
          1818181818121812E8E8E8E881ACE2ACE2E2E2E2E281E281E8E8E8E8121E1E18
          1E18181818181212E8E8E8E881ACACE2ACE2E2E2E2E28181E8E8E8E8128D1E1E
          181E181818181812E8E8E8E881E3ACACE2ACE2E2E2E2E281E8E8E8E8128D8D1E
          1E181E1818181812E8E8E8E881E3E3ACACE2ACE2E2E2E281E8E8E8E8E8121212
          12121212121212E8E8E8E8E8E881818181818181818181E8E8E8E8E8E8E8E8E8
          E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8
          E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8
          E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8}
        NumGlyphs = 2
      end
      object btnClearSave: TRzBitBtn
        Left = 485
        Top = 388
        Width = 81
        Caption = #20445#23384
        Enabled = False
        TabOrder = 2
        OnClick = btnClearSaveClick
        Glyph.Data = {
          36060000424D3606000000000000360400002800000020000000100000000100
          08000000000000020000730E0000730E00000001000000000000000000003300
          00006600000099000000CC000000FF0000000033000033330000663300009933
          0000CC330000FF33000000660000336600006666000099660000CC660000FF66
          000000990000339900006699000099990000CC990000FF99000000CC000033CC
          000066CC000099CC0000CCCC0000FFCC000000FF000033FF000066FF000099FF
          0000CCFF0000FFFF000000003300330033006600330099003300CC003300FF00
          330000333300333333006633330099333300CC333300FF333300006633003366
          33006666330099663300CC663300FF6633000099330033993300669933009999
          3300CC993300FF99330000CC330033CC330066CC330099CC3300CCCC3300FFCC
          330000FF330033FF330066FF330099FF3300CCFF3300FFFF3300000066003300
          66006600660099006600CC006600FF0066000033660033336600663366009933
          6600CC336600FF33660000666600336666006666660099666600CC666600FF66
          660000996600339966006699660099996600CC996600FF99660000CC660033CC
          660066CC660099CC6600CCCC6600FFCC660000FF660033FF660066FF660099FF
          6600CCFF6600FFFF660000009900330099006600990099009900CC009900FF00
          990000339900333399006633990099339900CC339900FF339900006699003366
          99006666990099669900CC669900FF6699000099990033999900669999009999
          9900CC999900FF99990000CC990033CC990066CC990099CC9900CCCC9900FFCC
          990000FF990033FF990066FF990099FF9900CCFF9900FFFF99000000CC003300
          CC006600CC009900CC00CC00CC00FF00CC000033CC003333CC006633CC009933
          CC00CC33CC00FF33CC000066CC003366CC006666CC009966CC00CC66CC00FF66
          CC000099CC003399CC006699CC009999CC00CC99CC00FF99CC0000CCCC0033CC
          CC0066CCCC0099CCCC00CCCCCC00FFCCCC0000FFCC0033FFCC0066FFCC0099FF
          CC00CCFFCC00FFFFCC000000FF003300FF006600FF009900FF00CC00FF00FF00
          FF000033FF003333FF006633FF009933FF00CC33FF00FF33FF000066FF003366
          FF006666FF009966FF00CC66FF00FF66FF000099FF003399FF006699FF009999
          FF00CC99FF00FF99FF0000CCFF0033CCFF0066CCFF0099CCFF00CCCCFF00FFCC
          FF0000FFFF0033FFFF0066FFFF0099FFFF00CCFFFF00FFFFFF00000080000080
          000000808000800000008000800080800000C0C0C00080808000191919004C4C
          4C00B2B2B200E5E5E500C8AC2800E0CC6600F2EABF00B59B2400D8E9EC009933
          6600D075A300ECC6D900646F710099A8AC00E2EFF10000000000000000000000
          0000000000000000000000000000000000000000000000000000000000000000
          0000000000000000000000000000000000000000000000000000E8E8E8E8E8E8
          E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E809090909
          090909090909090909E8E8E881818181818181818181818181E8E809101009E3
          1009E3E3E309101009E8E881ACAC81E3AC81E3E3E381ACAC81E8E809101009E3
          1009E3E3E309101009E8E881ACAC81E3AC81E3E3E381ACAC81E8E809101009E3
          1009E3E3E309101009E8E881ACAC81E3AC81E3E3E381ACAC81E8E809101009E3
          E3E3E3E3E309101009E8E881ACAC81E3E3E3E3E3E381ACAC81E8E80910101009
          090909090910101009E8E881ACACAC818181818181ACACAC81E8E80910101010
          101010101010101009E8E881ACACACACACACACACACACACAC81E8E80910100909
          090909090909101009E8E881ACAC8181818181818181ACAC81E8E8091009D7D7
          D7D7D7D7D7D7091009E8E881AC81D7D7D7D7D7D7D7D781AC81E8E8091009D709
          0909090909D7091009E8E881AC81D7818181818181D781AC81E8E8091009D7D7
          D7D7D7D7D7D7091009E8E881AC81D7D7D7D7D7D7D7D781AC81E8E809E309D709
          0909090909D7090909E8E881E381D7818181818181D7818181E8E8091009D7D7
          D7D7D7D7D7D7091009E8E881AC81D7D7D7D7D7D7D7D781AC81E8E80909090909
          090909090909090909E8E88181818181818181818181818181E8E8E8E8E8E8E8
          E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8E8}
        NumGlyphs = 2
      end
    end
    object ts3: TTabSheet
      BorderWidth = 3
      Caption = #26381#21153#31383#21475
      ImageIndex = 7
      ExplicitLeft = 0
      ExplicitTop = 0
      ExplicitWidth = 0
      ExplicitHeight = 0
      object pnlProgramWindow: TPanel
        Left = 0
        Top = 0
        Width = 660
        Height = 418
        Align = alClient
        BevelOuter = bvLowered
        TabOrder = 0
      end
    end
    object TabSheet13: TTabSheet
      Caption = #30456#20851#20449#24687
      ImageIndex = 3
      ExplicitLeft = 0
      ExplicitTop = 0
      ExplicitWidth = 0
      ExplicitHeight = 0
      object img1: TImage
        Left = 96
        Top = 372
        Width = 473
        Height = 31
        AutoSize = True
        Picture.Data = {
          07544269746D61702AAC0000424D2AAC0000000000003600000028000000D901
          00001F0000000100180000000000F4AB0000120B0000120B0000000000000000
          0000FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFF00FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF00FFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFDFCCB6CCAE
          8ACCAE8AD9C2A7F9F5F0FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF2EBE2D2B899CCAE8ACCAE8AECE1D3
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFECE1D3CCAE8ACCAE8AD2B899F2EBE2FFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF9F5F0D9C2
          A7CCAE8ACCAE8ADFCCB6FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF2EBE2D2B899CCAE8ACCAE8AECE1D3FFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFDFCCB6CCAE8ACCAE8AD9C2A7F9F5F0FFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFE5D7C5CCAE8ABF9B6ECCAE8AF2EBE2FFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF9F5F0E5D7
          C5D2B899CCAE8AC6A57CBF9B6ECCAE8ACCAE8AE5D7C5F2EBE2FFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF9F5F0D9C2A7CCAE8ACCAE8AECE1D3FFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFECE1D3CCAE8ACCAE8AD9C2A7F9F5F0FFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF9F5F0D9C2A7CCAE8ACCAE
          8AECE1D3FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFECE1D3CCAE8ACCAE8AD9C2A7F9F5F0
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFDFCCB6CCAE8ACCAE8AE5D7C5FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFECE1D3CCAE8ACCAE8AD9C2A7F9F5F0FFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF9F5F0D9C2A7CCAE8ACCAE
          8AECE1D3FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFF9F5F0D9C2A7CCAE8ACCAE8ACCAE8ACCAE8ACCAE8ACCAE8A
          CCAE8ACCAE8ACCAE8ACCAE8ACCAE8ACCAE8ACCAE8ACCAE8ACCAE8ACCAE8AE5D7
          C5FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFD9C2A7CCAE8ABF9B6ED2B899F9F5F0FFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF2EBE2E5D7C5CC
          AE8ACCAE8ABF9B6EBF9B6ECCAE8ACCAE8AD2B899E5D7C5F9F5F0FFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF9
          F5F0E5D7C5D2B899CCAE8AC6A57CBF9B6ECCAE8ACCAE8AE5D7C5F9F5F0FFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFECE1D3CCAE8ACC
          AE8AD9C2A7FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFF2EBE2D2B899CCAE8AD2B899F2EBE2FFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFDFCCB6CCAE8ACCAE8ADFCCB6FFFFFFFF
          FFFFFFFFFF00FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFECE1D3A874349B60179B60179B60179B6017A16A26D9C3A8FFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFC7A67D9B6017
          9B60179B60179B60179B6017BA9260F9F5F0FFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFF9F5F0B488519B60179B60179B60179B60179B6017
          C7A67DFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFD9C3A8A16A269B60179B60179B60179B6017AE7E43ECE1D3FFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFC7A67D9B60179B60
          179B60179B60179B6017B48851F9F5F0FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFECE1D3A874349B60179B60179B60179B6017
          A16A26E0CDB6FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFD9C3A89B60179B60179B
          60179B6017A16A26ECE1D3FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF9F5
          F0D3B999AE7E439B60179B60179B60179B60179B60179B60179B60179B60179B
          60179B6017A87434C7A67DF2EBE2FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFAE7E439B60
          179B60179B60179B6017CDAF8BFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFD9C3A89B60179B60179B6017
          9B6017A16A26F9F5F0FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFAE7E439B60179B60179B60179B6017CDAF8BFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFD9C3A89B6017
          9B60179B60179B6017A16A26F9F5F0FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFAE7E439B60179B60179B60179B6017C19C6EFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFC7A67D9B60179B60179B6017
          9B6017A16A26ECE1D3FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF9F5
          F0A874349B60179B60179B60179B6017D9C3A8FFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF2EBE2A16A269B60179B60179B6017
          9B60179B60179B60179B60179B60179B60179B60179B60179B60179B60179B60
          179B60179B60179B60179B6017C7A67DFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFC19C6E9B60179B60179B6017
          9B6017AE7E43F9F5F0FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF2EBE2C7
          A67DA874349B60179B60179B60179B60179B60179B60179B60179B60179B6017
          9B60179B6017BA9260E0CDB6FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFF9F5F0D3B999AE7E439B60179B60179B60179B60179B60179B60179B6017
          9B60179B60179B6017AE7E43CDAF8BF2EBE2FFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFC7A67D9B60179B60179B60179B6017AE7E43F9F5F0FFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFE0CDB69B60179B60179B60179B60179B6017D9C3
          A8FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFBA92609B60179B
          60179B60179B6017C19C6EFFFFFFFFFFFF00FFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFB689529E62189E62189E62189E62189E62189E
          62189E6218F3EBE2FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFDAC4A89E62189E62189E62189E62189E62189E62189E6218C9A77DFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFCEB08B9E62189E6218
          9E62189E62189E62189E62189E6218DAC4A8FFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFF3EBE29E62189E62189E62189E62189E62189E
          62189E6218B08043FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFE7D8C59E62189E62189E62189E62189E62189E62189E6218C39D6FFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFAA76359E6218
          9E62189E62189E62189E62189E6218A46C26F3EBE2FFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFAA76359E62189E62189E62189E62189E6218BC9360FFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFF9F5F1CEB08BA46C269E62189E62189E62189E62189E62189E62189E
          62189E62189E62189E62189E62189E62189E62189E62189E6218C39D6FF9F5F1
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFE7D8C59E62189E62189E62189E62189E62189E6218DAC4A8FFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFDAC4A8
          9E62189E62189E62189E62189E62189E6218CEB08BFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFE7D8C59E62189E62189E62189E62189E62189E6218DA
          C4A8FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFDAC4A89E62189E62189E62189E62189E62189E6218CEB08BFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFE7D8C59E62189E62189E62189E
          62189E62189E6218EDE1D4FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF3EBE2
          9E62189E62189E62189E62189E62189E6218AA7635FFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFD4BA9A9E62189E62189E62189E62189E6218A46C26FF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFCEB08B
          9E62189E62189E62189E62189E62189E62189E62189E62189E62189E62189E62
          189E62189E62189E62189E62189E62189E62189E62189E62189E6218FFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF3EBE2
          9E62189E62189E62189E62189E62189E6218D4BA9AFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFF3EBE2C39D6F9E62189E62189E62189E62189E62189E62189E62189E6218
          9E62189E62189E62189E62189E62189E62189E62189E6218C39D6FFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFF9F5F1CEB08BA46C269E62189E62189E62189E62189E6218
          9E62189E62189E62189E62189E62189E62189E62189E62189E62189E6218C39D
          6FF9F5F1FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFF9E62189E62189E62189E62189E62189E6218
          D4BA9AFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFAA76359E62189E62
          189E62189E62189E62189E6218F3EBE2FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFEDE1D49E62189E62189E62189E62189E62189E6218EDE1D4FFFFFF00FFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF3ECE2A16519A16519A1
          6519A16519A16519A16519A16519A16519CAA97EFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFB38244A16519A16519A16519A16519A165
          19A16519A16519A16519FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFAD7836A16519A16519A16519A16519A16519A16519A16519B38244FFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFCAA97EA16519A1
          6519A16519A16519A16519A16519A16519A16519E7D8C5FFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFC49F70A16519A16519A16519A16519A16519A1
          6519A16519A16519F9F5F1FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFE2CFB7A16519A16519A16519A16519A16519A16519A16519A16519D0B2
          8CFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFB38244A16519A16519A16519A16519A16519
          C49F70FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFEDE2D4B38244A16519A16519A16519A16519A1
          6519A16519A16519A16519A16519A16519A16519A16519A16519A16519A16519
          A16519A16519A16519A76F27EDE2D4FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFE7D8C5A16519A16519A16519A16519A1
          6519A16519A16519EDE2D4FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFEDE2D4A76F27A16519A16519A16519A16519A16519A16519DCC5
          A8FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFE7D8C5A16519A16519A1
          6519A16519A16519A16519A16519EDE2D4FFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFEDE2D4A76F27A16519A16519A16519A16519A165
          19A16519DCC5A8FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFE7
          D8C5A16519A16519A16519A16519A16519A16519DCC5A8FFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFD6BB9AA16519A16519A16519A16519A16519A16519A165
          19EDE2D4FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFBE9561A16519A16519A1
          6519A16519A16519B38244FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFD6BB9AA16519A16519A16519A16519A16519A16519A165
          19A16519A16519A16519A16519A16519A16519A16519A16519A16519A16519A1
          6519A16519A76F27FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFF9F5F1A16519A16519A16519A16519A16519A16519DCC5
          A8FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFE2CFB7A76F27A16519A16519A16519A16519A16519
          A16519A16519A16519A16519A16519A16519A16519A16519A16519A16519A165
          19A16519A16519DCC5A8FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEDE2D4B38244A16519A16519A16519
          A16519A16519A16519A16519A16519A16519A16519A16519A16519A16519A165
          19A16519A16519A16519A16519A76F27EDE2D4FFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFA16519A16519
          A16519A16519A16519A16519C49F70FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFEDE2D4A16519A16519A16519A16519A16519A16519A16519D6BB9AFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFD6BB9AA16519A16519A16519A16519A16519
          A16519F9F5F1FFFFFF00FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFDDC6A9A4681BA4681BA4681BA4681BA4681BA4681BA4681BA4681BAA7129
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEEE3D4A4681BA468
          1BA4681BA4681BA4681BA4681BA4681BA4681BA4681BE2D0B7FFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFF4ECE2A4681BA4681BA4681BA4681BA4681BA468
          1BA4681BA4681BA4681BEEE3D4FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFAA7129A4681BA4681BA4681BA4681BA4681BA4681BA4681BA4681B
          CCAA7FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFAF7B38A4681BA4
          681BA4681BA4681BA4681BA4681BA4681BA4681BD7BD9BFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFC19763A4681BA4681BA4681BA4681BA468
          1BA4681BA4681BA4681BB58446FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFE2D0B7A4681B
          A4681BA4681BA4681BAA7129EEE3D4FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEEE3D4AA7129A4681BA4
          681BA4681BA4681BA4681BA4681BA4681BA4681BA4681BA4681BA4681BA4681B
          A4681BA4681BA4681BA4681BA4681BA4681BA4681BA4681BAA7129EEE3D4FFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFB5
          8446A4681BA4681BA4681BA4681BA4681BA4681BAA7129EEE3D4FFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF4ECE2AA7129A4681BA4681BA4681BA468
          1BA4681BA4681BB58446FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFB58446A4681BA4681BA4681BA4681BA4681BA4681BAA7129EEE3D4
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF4ECE2AA7129A4681BA468
          1BA4681BA4681BA4681BA4681BB58446FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFA4681BA4681BA4681BA4681BA4681BA4681B
          D1B38DFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFB58446A4681BA4681BA468
          1BA4681BA4681BA4681BA4681BCCAA7FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFB58446A4681BA4681BA4681BA4681BA4681BC6A171FFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF9F6F1B58446A4681BA468
          1BA4681BA4681BA4681BA4681BA4681BA4681BA4681BA4681BA4681BA4681BA4
          681BA4681BA4681BA4681BA4681BAA7129DDC6A9FFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFCCAA7FA4681BA468
          1BA4681BA4681BB58446F9F6F1FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFDDC6A9A4681BA4681BA4681B
          A4681BA4681BA4681BA4681BA4681BA4681BA4681BA4681BA4681BA4681BA468
          1BA4681BA4681BA4681BA4681BA4681BA4681BD1B38DFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEEE3D4AA7129
          A4681BA4681BA4681BA4681BA4681BA4681BA4681BA4681BA4681BA4681BA468
          1BA4681BA4681BA4681BA4681BA4681BA4681BA4681BA4681BA4681BA4681BE2
          D0B7FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFBB8E54A4681BA4681BA4681BA4681BA4681BBB8E54FFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFCCAA7FA4681BA4681BA4681BA4681BA4681BA4
          681BA4681BB58446FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFCCAA7FA4681B
          A4681BA4681BA4681BA4681BAF7B38FFFFFFFFFFFF00FFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFC39963A76B1CA76B1CA76B1CA76B1CA76B1C
          A76B1CA76B1CA76B1CA76B1CE3D1B8FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFCEAC80A76B1CA76B1CA76B1CA76B1CA76B1CA76B1CA76B1CA76B1CA7
          6B1CCEAC80FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFD8BE9BA76B1CA76B
          1CA76B1CA76B1CA76B1CA76B1CA76B1CA76B1CA76B1CCEAC80FFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFE3D1B8A76B1CA76B1CA76B1CA76B1CA76B1C
          A76B1CA76B1CA76B1CA76B1CB88747FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFEEE3D4A76B1CA76B1CA76B1CA76B1CA76B1CA76B1CA76B1CA76B1CA76B1C
          B88747FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF9F6F1A76B1CA76B
          1CA76B1CA76B1CA76B1CA76B1CA76B1CA76B1CA76B1CA76B1CF9F6F1FFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFE9DAC6CEAC80BD9055CEAC80F4ECE3FFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF9
          F6F1B27E38A76B1CA76B1CA76B1CA76B1CA76B1CA76B1CA76B1CBD9055DEC7AA
          E9DAC6FFFFFFFFFFFFEEE3D4E3D1B8C8A371AD742AA76B1CA76B1CA76B1CA76B
          1CA76B1CA76B1CAD742AF9F6F1FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFF9F6F1B88747A76B1CA76B1CA76B1CA76B1CA76B1C
          A76B1CB88747F9F6F1FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF9F6F1B88747A76B
          1CA76B1CA76B1CA76B1CA76B1CA76B1CB27E38F9F6F1FFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF9F6F1B88747A76B1CA76B1CA76B1C
          A76B1CA76B1CA76B1CB88747F9F6F1FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF9F6
          F1B88747A76B1CA76B1CA76B1CA76B1CA76B1CA76B1CB27E38F9F6F1FFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFB88747A76B1C
          A76B1CA76B1CA76B1CA76B1CBD9055FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF4EC
          E3A76B1CA76B1CA76B1CA76B1CA76B1CA76B1CA76B1CA76B1CB27E38FFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFA76B1CA76B1CA76B1CA76B1CA76B1CA76B1C
          D8BE9BFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFF9F6F1B88747A76B1CA76B1CA76B1CA76B1CA76B1CA76B1CA76B1CD8
          BE9BE9DAC6E9DAC6E9DAC6E9DAC6E9DAC6E9DAC6E9DAC6E9DAC6FFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFDEC7AAC8A371BD9055D8BE9BF9F6F1FFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEEE3D4
          A76B1CA76B1CA76B1CA76B1CA76B1CA76B1CA76B1CA76B1CAD742AC8A371D3B5
          8DE9DAC6E9DAC6DEC7AACEAC80B27E38A76B1CA76B1CA76B1CA76B1CAD742AF4
          ECE3FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFF9F6F1B27E38A76B1CA76B1CA76B1CA76B1CA76B1CA76B1CA76B1CBD90
          55DEC7AAE9DAC6FFFFFFFFFFFFEEE3D4DEC7AAC8A371A76B1CA76B1CA76B1CA7
          6B1CA76B1CA76B1CA76B1CAD742AEEE3D4FFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFCEAC80A76B1CA76B1CA76B1CA76B
          1CA76B1CA76B1CFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFB27E38A76B1CA7
          6B1CA76B1CA76B1CA76B1CA76B1CA76B1CA76B1CF4ECE3FFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFBD9055A76B1CA76B1CA76B1CA76B1CA76B1CC39963FFFFFFFFFF
          FF00FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFB0772CAB6E1E
          AB6E1EAB6E1EAB6E1EAB6E1EAB6E1EAB6E1EAB6E1EAB6E1ECBA573FFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFB6803AAB6E1EAB6E1EAB6E1EAB6E1EAB
          6E1EAB6E1EAB6E1EAB6E1EAB6E1EBB8948FFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFC59B65AB6E1EAB6E1EAB6E1EAB6E1EAB6E1EAB6E1EAB6E1EAB6E1EAB
          6E1EB6803AFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFCBA573AB6E1E
          AB6E1EAB6E1EAB6E1EAB6E1EAB6E1EAB6E1EAB6E1EAB6E1EAB6E1EFAF6F1FFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFDABF9CAB6E1EAB6E1EAB6E1EAB6E1EAB6E1E
          AB6E1EAB6E1EAB6E1EAB6E1EAB6E1EF4EDE3FFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFDFC8AAAB6E1EAB6E1EAB6E1EAB6E1EAB6E1EAB6E1EAB6E1EAB6E1EAB
          6E1EAB6E1EE5D2B8FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFCBA573AB6E1EAB6E1EAB6E1EAB6E1EAB6E1EAB6E1E
          B0772CEADBC7FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFAF6
          F1C59B65AB6E1EAB6E1EAB6E1EAB6E1EAB6E1EAB6E1ECBA573FFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFAF6F1B0772C
          AB6E1EAB6E1EAB6E1EAB6E1EAB6E1EAB6E1EBB8948FFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFC09256AB6E1EAB6E1EAB6E1EAB6E1EAB6E1EAB6E1EB0772CEFE4D5FF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FAF6F1B0772CAB6E1EAB6E1EAB6E1EAB6E1EAB6E1EAB6E1EBB8948FFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFC09256AB6E1EAB6E1EAB6E1EAB6E1EAB6E1EAB6E1EB0
          772CEFE4D5FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFCBA573AB6E1EAB6E1EAB6E1EAB6E1EAB6E1EB6803AFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFDABF9CAB6E1EAB6E1EAB6E1EAB6E1EAB6E1EAB6E1EAB
          6E1EAB6E1EAB6E1EEFE4D5FFFFFFFFFFFFFFFFFFFFFFFFEFE4D5AB6E1EAB6E1E
          AB6E1EAB6E1EAB6E1EAB6E1EEADBC7FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFAF6F1BB8948AB6E1EAB6E1EAB
          6E1EAB6E1EAB6E1EAB6E1EB0772CEFE4D5FFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFB6803AAB6E1EAB6E1EAB6E1EAB6E1EAB6E1EAB6E1EB077
          2CDFC8AAFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFE5D2B8CB
          A573C09256C59B65EADBC7FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFCBA573AB6E1EAB6E1EAB6E1EAB6E1EAB6E
          1EAB6E1EB0772CEADBC7FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFF4EDE3BB8948AB6E1EAB6E1EAB6E1EAB6E1EAB6E1EAB6E1EC09256FFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFDFC8
          AAAB6E1EAB6E1EAB6E1EAB6E1EAB6E1EAB6E1EF4EDE3FFFFFFFFFFFFFFFFFFFF
          FFFFEFE4D5AB6E1EAB6E1EAB6E1EAB6E1EAB6E1EAB6E1EAB6E1EAB6E1EAB6E1E
          DABF9CFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFB0772CAB6E1EAB6E1EAB6E1EAB6E
          1EAB6E1ED5B68EFFFFFFFFFFFF68FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFF0E4D5AF721FAF721FAF721FAF721FAF721FAF721FAF721FAF721FAF72
          1FAF721FB47B2DFAF6F1FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFE6D3B9AF721FAF
          721FAF721FAF721FAF721FAF721FAF721FAF721FAF721FAF721FAF721FF5EDE3
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFB47B2DAF721FAF721FAF721FAF721FAF
          721FAF721FAF721FAF721FAF721FAF721FEBDCC7FFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFAF6F1AF721FAF721FAF721FAF721FAF721FAF721FAF721FAF721FAF72
          1FAF721FAF721FE1CAABFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFC89E65AF721F
          AF721FAF721FAF721FAF721FAF721FAF721FAF721FAF721FAF721FD7B88FFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFBE8D49AF721FAF721FAF721FAF721FAF
          721FAF721FAF721FAF721FAF721FAF721FCDA773FFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF5EDE3AF721FAF721FAF721F
          AF721FAF721FAF721FB47B2DF0E4D5FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFC89E65AF721FAF721FAF721FAF721FAF
          721FAF721FF5EDE3FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFF0E4D5B47B2DAF721FAF721FAF721FAF721FAF721FAF72
          1FCDA773FFFFFFFFFFFFFFFFFFCDA773AF721FAF721FAF721FAF721FAF721FAF
          721FAF721FEBDCC7FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF0E4D5B47B2DAF721FAF721FAF721FAF72
          1FAF721FAF721FCDA773FFFFFFFFFFFFFFFFFFCDA773AF721FAF721FAF721FAF
          721FAF721FAF721FAF721FEBDCC7FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFDCC19DAF721FAF721FAF721FAF72
          1FAF721FAF721FFAF6F1FFFFFFFFFFFFFFFFFFFFFFFFBE8D49AF721FAF721FAF
          721FAF721FAF721FAF721FAF721FAF721FAF721FD2B081FFFFFFFFFFFFFFFFFF
          FFFFFFE6D3B9AF721FAF721FAF721FAF721FAF721FAF721FFAF6F1FFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFAF6F1C39557AF721FAF721FAF721FAF721FAF721FAF721FB47B2DF0E4D5
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFE1CAABAF721FAF721FAF721FAF72
          1FAF721FAF721FB47B2DF0E4D5FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF5EDE3AF721FAF72
          1FAF721FAF721FAF721FAF721FB47B2DF0E4D5FFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFAF6F1BE8D49AF721FAF721FAF721F
          AF721FAF721FAF721FE1CAABFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFF0E4D5AF721FAF721FAF721FAF721FAF721FAF721FE6
          D3B9FFFFFFFFFFFFFFFFFFFFFFFFD2B081AF721FAF721FAF721FAF721FAF721F
          AF721FAF721FAF721FAF721FBE8D49FFFFFFFFFFFFFFFFFFFFFFFFFAF6F1AF72
          1FAF721FAF721FAF721FAF721FAF721FE6D3B9FFFFFFFFFFFF06FFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFD9BA90B37521B37521B37521B37521B375
          21C18F4BB37521B37521B37521B37521B37521E2CBABFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFD0A975B37521B37521B37521B37521B37521C18F4BB37521B37521
          B37521B37521B37521E2CBABFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFECDCC7B37521B3
          7521B37521B37521B37521C18F4BB37521B37521B37521B37521B37521D0A975
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFE2CBABB37521B37521B37521B37521B375
          21C18F4BB37521B37521B37521B37521B37521D0A975FFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFB37521B37521B37521B37521B37521BD863DB87E2FB37521B375
          21B37521B37521BD863DFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF5EEE3B37521B3
          7521B37521B37521B37521C18F4BB37521B37521B37521B37521B37521BD863D
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          D0A975B37521B37521B37521B37521B37521B37521E2CBABFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF5EEE3B3
          7521B37521B37521B37521B37521B37521DEC29DFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFE7D4B9B37521B375
          21B37521B37521B37521B37521B37521D9BA90FFFFFFDEC29DB37521B37521B3
          7521B37521B37521B37521B37521E2CBABFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFE7D4
          B9B37521B37521B37521B37521B37521B37521B37521D9BA90FFFFFFDEC29DB3
          7521B37521B37521B37521B37521B37521B37521E2CBABFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFECDC
          C7B37521B37521B37521B37521B37521B37521ECDCC7FFFFFFFFFFFFFFFFFFF5
          EEE3B37521B37521B37521B37521B37521B37521B37521B37521B37521B37521
          BD863DFFFFFFFFFFFFFFFFFFFFFFFFD9BA90B37521B37521B37521B37521B375
          21BD863DFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFD0A975B37521B37521B37521
          B37521B37521B37521B87E2FF1E5D5FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFBD86
          3DB37521B37521B37521B37521B37521B37521ECDCC7FFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFD0A975B37521B37521B37521B37521B37521B37521E2CBABFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FAF6F1B87E2FB37521B37521B37521B37521B37521C69859FFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFB37521B37521B3
          7521B37521B37521B37521D9BA90FFFFFFFFFFFFFFFFFFFFFFFFBD863DB37521
          B37521B37521B37521B37521B37521B37521B37521B37521B37521F5EEE3FFFF
          FFFFFFFFFFFFFFECDCC7B37521B37521B37521B37521B37521B37521F5EEE3FF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFC5924CB779
          23B77923B77923B77923B77923EDDDC8B77923B77923B77923B77923B77923CE
          A368FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFBC8131B77923B77923B77923B77923
          C08A3FE4CDACB77923B77923B77923B77923B77923D2AB76FFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFD7B484B77923B77923B77923B77923B77923E4CDACC08A3FB77923
          B77923B77923B77923BC8131FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFCEA368B779
          23B77923B77923B77923B77923EDDDC8B77923B77923B77923B77923B77923C0
          8A3FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFE8D5BAB77923B77923B77923B77923B779
          23D2AB76D2AB76B77923B77923B77923B77923B77923F1E6D6FFFFFFFFFFFFFF
          FFFFFFFFFFDFC49EB77923B77923B77923B77923B77923E4CDACC08A3FB77923
          B77923B77923B77923B77923F6EEE3FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFBC8131B77923B77923B77923B77923B77923C08A
          3FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFC99B5AB77923B77923B77923B77923B77923C99B5A
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFE4CDACB77923B77923B77923B77923B77923B77923B77923D2
          AB76B77923B77923B77923B77923B77923B77923B77923D7B484FFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFE4CDACB77923B77923B77923B77923B77923B7
          7923B77923D2AB76B77923B77923B77923B77923B77923B77923B77923D7B484
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFAF7F1B77923B77923B77923B77923B77923B77923DF
          C49EFFFFFFFFFFFFFFFFFFDFC49EB77923B77923B77923B77923B77923C08A3F
          B77923B77923B77923B77923B77923F1E6D6FFFFFFFFFFFFFFFFFFCEA368B779
          23B77923B77923B77923B77923CEA368FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFD2AB76B77923B77923B77923B77923B77923B77923BC8131F1E6D6FFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFF1E6D6B77923B77923B77923B77923B77923B77923CEA368FF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFBC8131B77923B77923B77923B77923B7
          7923C08A3FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFD7B484B77923B77923B77923B77923B779
          23B77923F6EEE3FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFC5924CB77923B77923B77923B77923B77923CEA368FFFFFFFFFFFF
          FFFFFFF1E6D6B77923B77923B77923B77923B77923C08A3FB77923B77923B779
          23B77923B77923DFC49EFFFFFFFFFFFFFFFFFFDFC49EB77923B77923B77923B7
          7923B77923BC8131FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFBF7F1BB7D24BB7D24BB7D24BB7D24BB7D24BF8532FFFFFFC8954DBB
          7D24BB7D24BB7D24BB7D24BB7D24FBF7F1FFFFFFFFFFFFFFFFFFE5CEADBB7D24
          BB7D24BB7D24BB7D24BB7D24D9B684F6EFE4BB7D24BB7D24BB7D24BB7D24BB7D
          24C48D3FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFC8954DBB7D24BB7D24BB7D24BB7D24
          BB7D24F2E7D6D9B684BB7D24BB7D24BB7D24BB7D24BB7D24EAD6BAFFFFFFFFFF
          FFFFFFFFF6EFE4BB7D24BB7D24BB7D24BB7D24BB7D24C8954DFFFFFFC48D3FBB
          7D24BB7D24BB7D24BB7D24BB7D24F6EFE4FFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFD9B684BB7D
          24BB7D24BB7D24BB7D24BB7D24E1C69FEAD6BABB7D24BB7D24BB7D24BB7D24BB
          7D24D9B684FFFFFFFFFFFFFFFFFFFFFFFFC48D3FBB7D24BB7D24BB7D24BB7D24
          BB7D24FBF7F1D5AE76BB7D24BB7D24BB7D24BB7D24BB7D24E5CEADFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF2E7D6BB7D24BB7D24BB7D
          24BB7D24BB7D24BB7D24D9B684FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFF6EFE4EEDEC8EEDEC8EEDEC8EEDEC8D5AE76BB7D24BB7D24
          BB7D24BB7D24BB7D24BF8532FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFD9B684BB7D24BB7D24BB
          7D24BB7D24BB7D24BB7D24BB7D24BB7D24BB7D24BB7D24BB7D24BB7D24BB7D24
          D5AE76FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFD9B684BB
          7D24BB7D24BB7D24BB7D24BB7D24BB7D24BB7D24BB7D24BB7D24BB7D24BB7D24
          BB7D24BB7D24D5AE76FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFC48D3FBB7D24BB
          7D24BB7D24BB7D24BB7D24D9B684FFFFFFFFFFFFFFFFFFC8954DBB7D24BB7D24
          BB7D24BB7D24BF8532F2E7D6BB7D24BB7D24BB7D24BB7D24BB7D24D9B684FFFF
          FFFFFFFFFFFFFFC8954DBB7D24BB7D24BB7D24BB7D24BB7D24DDBE91FFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFD5AE76BB7D24BB7D24BB7D24BB7D
          24BB7D24BB7D24BF8532F2E7D6FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFE1C69FBB7D24BB7D24BB7D24BB
          7D24BB7D24BB7D24EAD6BAFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF2E7D6BB7D24BB
          7D24BB7D24BB7D24BB7D24BB7D24D9B684FFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF2E7D6BB7D
          24BB7D24BB7D24BB7D24BB7D24BB7D24E5CEADFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFD5AE76BB7D24BB7D24BB7D24BB7D24
          BB7D24C8954DFFFFFFFFFFFFFFFFFFD9B684BB7D24BB7D24BB7D24BB7D24BB7D
          24F2E7D6BF8532BB7D24BB7D24BB7D24BB7D24C8954DFFFFFFFFFFFFFFFFFFD9
          B684BB7D24BB7D24BB7D24BB7D24BB7D24CC9E5BFFFFFFFFFFFFFFFFFF01FFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEBD7BBBF8126BF8126BF8126BF8126BF
          8126D3A96AFFFFFFE3C8A0BF8126BF8126BF8126BF8126BF8126E3C8A0FFFFFF
          FFFFFFFFFFFFD3A96ABF8126BF8126BF8126BF8126BF8126F3E7D6FFFFFFC389
          34BF8126BF8126BF8126BF8126BF8126F3E7D6FFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFBF7F1BF8126
          BF8126BF8126BF8126BF8126C38934FFFFFFF3E7D6BF8126BF8126BF8126BF81
          26BF8126D3A96AFFFFFFFFFFFFFFFFFFE3C8A0BF8126BF8126BF8126BF8126BF
          8126E3C8A0FFFFFFD3A96ABF8126BF8126BF8126BF8126BF8126E3C8A0FFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFCB994FBF8126BF8126BF8126BF8126BF8126F3E7D6FFFFFFC3
          8934BF8126BF8126BF8126BF8126C38934FFFFFFFFFFFFFFFFFFF3E7D6BF8126
          BF8126BF8126BF8126BF8126D3A96AFFFFFFE3C8A0BF8126BF8126BF8126BF81
          26BF8126D3A96AFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFE7D0ADBF8126BF8126BF8126BF8126BF8126BF8126EFDFC9FFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFF3E7D6C79141BF8126BF8126BF8126BF8126
          BF8126BF8126BF8126BF8126BF8126BF8126BF8126BF8126FFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFD7B078BF8126BF8126BF8126BF8126BF8126BF8126BF8126BF8126
          BF8126BF8126BF8126CB994FFBF7F1FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFD7B078BF8126BF8126BF8126BF8126BF8126BF8126
          BF8126BF8126BF8126BF8126BF8126CB994FFBF7F1FFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFD3A96ABF8126BF8126BF8126BF8126BF8126CFA15CFFFFFFFFFFFF
          F7EFE4BF8126BF8126BF8126BF8126BF8126D7B078FFFFFFC79141BF8126BF81
          26BF8126BF8126C79141FFFFFFFFFFFFFFFFFFBF8126BF8126BF8126BF8126BF
          8126BF8126EFDFC9FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFD7B078BF8126BF8126BF8126BF8126BF8126BF8126C38934F3E7D6FFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFD7
          B078BF8126BF8126BF8126BF8126BF8126BF8126FBF7F1FFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFE7D0ADBF8126BF8126BF8126BF8126BF8126BF8126EFDFC9FFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFC38934BF8126BF8126BF8126BF8126BF8126DBB885FF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFE3C8A0
          BF8126BF8126BF8126BF8126BF8126BF8126FFFFFFFFFFFFFFFFFFC79141BF81
          26BF8126BF8126BF8126C79141FFFFFFD7B078BF8126BF8126BF8126BF8126BF
          8126F7EFE4FFFFFFFFFFFFCFA15CBF8126BF8126BF8126BF8126BF8126DFC092
          FFFFFFFFFFFFFFFFFF01FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFDAB379C3
          8528C38528C38528C38528C38528E5C9A1FFFFFFF7F0E4C38528C38528C38528
          C38528C38528CE9C50FFFFFFFFFFFFFBF7F2C38528C38528C38528C38528C385
          28CB9443FFFFFFFFFFFFD6AB6BC38528C38528C38528C38528C38528E5C9A1FF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFE8D1AEC38528C38528C38528C38528C38528D6AB6BFFFFFFFFFF
          FFCB9443C38528C38528C38528C38528C38528FBF7F2FFFFFFFFFFFFCE9C50C3
          8528C38528C38528C38528C38528F7F0E4FFFFFFE5C9A1C38528C38528C38528
          C38528C38528D6AB6BFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF7F0E4C38528C38528C38528C38528C3
          8528C78D35FFFFFFFFFFFFDAB379C38528C38528C38528C38528C38528ECD9BC
          FFFFFFFFFFFFDDBB86C38528C38528C38528C38528C38528E8D1AEFFFFFFF4E8
          D7C38528C38528C38528C38528C38528C78D35FFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFE1C293C38528C38528C38528C38528C38528C3
          8528F4E8D7FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFD2A45EC38528
          C38528C38528C38528C38528C38528C38528C38528C38528C38528C38528C385
          28C38528F4E8D7FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFCE9C50C38528C38528C38528
          C38528C38528C38528C38528C38528C38528CB9443FBF7F2FFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFCE9C50C38528
          C38528C38528C38528C38528C38528C38528C38528C38528CB9443FBF7F2FFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFE1C293C38528C38528C38528C38528
          C38528C78D35FFFFFFFFFFFFE5C9A1C38528C38528C38528C38528C38528ECD9
          BCFFFFFFDDBB86C38528C38528C38528C38528C38528F4E8D7FFFFFFF4E8D7C3
          8528C38528C38528C38528C38528C38528F7F0E4FFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFDAB379C38528C38528C38528C38528C3
          8528C38528CE9C50FBF7F2FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFD2A45EC38528C38528C38528C38528C38528C78D35
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFE1C293C38528C38528C38528C38528
          C38528C38528F4E8D7FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFD2A45EC38528C38528C3
          8528C38528C38528D2A45EFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFF0E0C9C38528C38528C38528C38528C38528C38528F4E8
          D7FFFFFFF4E8D7C38528C38528C38528C38528C38528DDBB86FFFFFFECD9BCC3
          8528C38528C38528C38528C38528E5C9A1FFFFFFFFFFFFC78D35C38528C38528
          C38528C38528C38528E8D1AEFFFFFFFFFFFFFFFFFF00FFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFCF9845C8892AC8892AC8892AC8892AC8892AF1E1CAFFFFFF
          FFFFFFD29F52C8892AC8892AC8892AC8892AC8892AF8F0E4FFFFFFEAD3AFC889
          2AC8892AC8892AC8892AC8892AE0BD88FFFFFFFFFFFFE7CBA1C8892AC8892AC8
          892AC8892AC8892AD9AE6DFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFDDB57AC8892AC8892AC8892AC889
          2AC8892AE3C494FFFFFFFFFFFFE0BD88C8892AC8892AC8892AC8892AC8892AEA
          D3AFFFFFFFF8F0E4C8892AC8892AC8892AC8892AC8892AD29F52FFFFFFFFFFFF
          F5E9D7C8892AC8892AC8892AC8892AC8892ACB9037FFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEAD3AFC8
          892AC8892AC8892AC8892AC8892AD6A75FFFFFFFFFFFFFEEDABCC8892AC8892A
          C8892AC8892AC8892ADDB57AFFFFFFFFFFFFCF9845C8892AC8892AC8892AC889
          2AC8892AFCF8F2FFFFFFFFFFFFCB9037C8892AC8892AC8892AC8892AC8892AF5
          E9D7FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFE3C494C8892AC8
          892AC8892AC8892AC8892AC8892AFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFD29F52C8892AC8892AC8892AC8892AC8892AC8892AC8892AC889
          2AC8892AC8892AC8892AC8892ACB9037FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FCF8F2D29F52C8892AC8892AC8892AC8892AC8892AC8892AC8892ACB9037F5E9
          D7FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFCF8F2D29F52C8892AC8892AC8892AC8892AC8892AC8892AC889
          2ACB9037F5E9D7FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF1E1CA
          C8892AC8892AC8892AC8892AC8892AC8892AFCF8F2FFFFFFD29F52C8892AC889
          2AC8892AC8892ACF9845FFFFFFFFFFFFF8F0E4C8892AC8892AC8892AC8892AC8
          892AE0BD88FFFFFFEEDABCC8892AC8892AC8892AC8892AC8892ACB9037FFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFDD
          B57AC8892AC8892AC8892AC8892AC8892AC8892AD9AE6DFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFD6A75FC8892AC8892A
          C8892AC8892AC8892AD29F52FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFE3C494
          C8892AC8892AC8892AC8892AC8892AC8892AFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFD6A75FC8892AC8892AC8892AC8892AC8892AD6A75FFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFC8892AC8892AC889
          2AC8892AC8892AC8892AEEDABCFFFFFFE0BD88C8892AC8892AC8892AC8892AC8
          892AF8F0E4FFFFFFFFFFFFCF9845C8892AC8892AC8892AC8892AD29F52FFFFFF
          FCF8F2C8892AC8892AC8892AC8892AC8892AC8892AF5E9D7FFFFFFFFFFFFFFFF
          FF82FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF5EAD7CC8D2CCC8D2CCC8D2CCC8D2C
          CC8D2CCC8D2CFFFFFFFFFFFFFFFFFFE9CDA2CC8D2CCC8D2CCC8D2CCC8D2CCC8D
          2CE5C695FFFFFFD6A254CC8D2CCC8D2CCC8D2CCC8D2CCC8D2CF9F1E5FFFFFFFF
          FFFFF5EAD7CC8D2CCC8D2CCC8D2CCC8D2CCC8D2CCC8D2CFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFCF94
          39CC8D2CCC8D2CCC8D2CCC8D2CCC8D2CF2E2CAFFFFFFFFFFFFF5EAD7CC8D2CCC
          8D2CCC8D2CCC8D2CCC8D2CD9AA61FFFFFFE2BF89CC8D2CCC8D2CCC8D2CCC8D2C
          CC8D2CECD4B0FFFFFFFFFFFFFFFFFFCF9439CC8D2CCC8D2CCC8D2CCC8D2CCC8D
          2CF2E2CAFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFDCB16ECC8D2CCC8D2CCC8D2CCC8D2CCC8D2CE5C695FFFFFF
          FFFFFFFFFFFFCF9439CC8D2CCC8D2CCC8D2CCC8D2CCF9439FCF8F2EFDBBDCC8D
          2CCC8D2CCC8D2CCC8D2CCC8D2CDFB87BFFFFFFFFFFFFFFFFFFDCB16ECC8D2CCC
          8D2CCC8D2CCC8D2CCC8D2CE5C695FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFE5C695CC8D2CCC8D2CCC8D2CCC8D2CCC8D2CCC8D2CF2E2CAFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFE5C695CC8D2CCC8D2CCC8D2CCC8D
          2CCC8D2CCC8D2CCC8D2CCC8D2CCC8D2CCC8D2CCC8D2CCC8D2CE2BF89FFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFDBBDCC8D2CCC8D2CCC8D2CCC8D2CCC8D
          2CCC8D2CCC8D2CE5C695FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFDBBDCC8D2CCC8D2CCC8D
          2CCC8D2CCC8D2CCC8D2CCC8D2CE5C695FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFCF8F2CC8D2CCC8D2CCC8D2CCC8D2CCC8D2CCC8D2CF2E2
          CAF5EAD7CC8D2CCC8D2CCC8D2CCC8D2CCC8D2CE2BF89FFFFFFFFFFFFFFFFFFD6
          A254CC8D2CCC8D2CCC8D2CCC8D2CD29B46FFFFFFE5C695CC8D2CCC8D2CCC8D2C
          CC8D2CCC8D2CD9AA61FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFD6A254CC8D2CCC8D2CCC8D2CCC8D2CCC8D2C
          CC8D2CE9CDA2FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFD9AA61CC8D2CCC8D2CCC8D2CCC8D2CCC8D2CCC8D2CFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFE5C695CC8D2CCC8D2CCC8D2CCC8D2CCC8D2CCC8D2CF2E2
          CAFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFD6A254CC8D2CCC8D2CCC8D2CCC8D2CCC8D2C
          DCB16EFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFD6A254CC8D2CCC8D2CCC8D2CCC8D2CCC8D2CE5C695FFFFFFCF9439CC
          8D2CCC8D2CCC8D2CCC8D2CD6A254FFFFFFFFFFFFFFFFFFE2BF89CC8D2CCC8D2C
          CC8D2CCC8D2CCC8D2CF9F1E5F2E2CACC8D2CCC8D2CCC8D2CCC8D2CCC8D2CCC8D
          2CFFFFFFFFFFFFFFFFFFFFFFFFF5FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEACFA3
          D0912ED0912ED0912ED0912ED0912EDCAD62FFFFFFFFFFFFFFFFFFFCF8F2D091
          2ED0912ED0912ED0912ED0912ED69F48F9F1E5D0912ED0912ED0912ED0912ED0
          912ED9A655FFFFFFFFFFFFFFFFFFFFFFFFD3983BD0912ED0912ED0912ED0912E
          D0912EF3E3CBFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFF6EAD8D0912ED0912ED0912ED0912ED0912ED0912EFFFFFFFF
          FFFFFFFFFFFFFFFFD9A655D0912ED0912ED0912ED0912ED0912EF9F1E5D69F48
          D0912ED0912ED0912ED0912ED0912EFCF8F2FFFFFFFFFFFFFFFFFFDFB470D091
          2ED0912ED0912ED0912ED0912EE7C896FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFD3983BD0912ED0912ED0912E
          D0912ED0912EF3E3CBFFFFFFFFFFFFFFFFFFE5C18AD0912ED0912ED0912ED091
          2ED0912EEDD6B0E2BA7DD0912ED0912ED0912ED0912ED0912EF0DCBDFFFFFFFF
          FFFFFFFFFFEACFA3D0912ED0912ED0912ED0912ED0912EDCAD62FFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF3E3CBD0912ED0912ED0912ED0912E
          D0912ED0912EF0DCBDFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFEDD6B0DFB470DCAD62DCAD62DCAD62DCAD62DCAD62DCAD62DCAD62DCAD62DF
          B470EDD6B0FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF6EAD8D3983BD091
          2ED0912ED0912ED0912ED0912ED0912ED0912ED0912EEDD6B0FFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF6EA
          D8D3983BD0912ED0912ED0912ED0912ED0912ED0912ED0912ED0912EEDD6B0FF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFD69F48D0912ED091
          2ED0912ED0912ED0912EEACFA3E5C18AD0912ED0912ED0912ED0912ED0912EF6
          EAD8FFFFFFFFFFFFFFFFFFEACFA3D0912ED0912ED0912ED0912ED0912EF6EAD8
          DFB470D0912ED0912ED0912ED0912ED0912EE7C896FFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFCF8F2D69F48
          D0912ED0912ED0912ED0912ED0912ED3983BFCF8F2FFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFE7C896D0912ED0912ED0912ED0912ED091
          2ED0912EF9F1E5FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF3E3CBD0912ED0912ED091
          2ED0912ED0912ED0912EF0DCBDFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFCF8F2D0912ED0912E
          D0912ED0912ED0912ED0912EE7C896FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFE2BA7DD0912ED0912ED0912ED0912ED0
          912EDFB470F0DCBDD0912ED0912ED0912ED0912ED0912EEACFA3FFFFFFFFFFFF
          FFFFFFF6EAD8D0912ED0912ED0912ED0912ED0912EEACFA3EACFA3D0912ED091
          2ED0912ED0912ED0912EDCAD62FFFFFFFFFFFFFFFFFFFFFFFF98FFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFE1B670D4952FD4952FD4952FD4952FD4952FE9CA97FFFF
          FFFFFFFFFFFFFFFFFFFFE1B670D4952FD4952FD4952FD4952FD4952FE4BD7DD4
          952FD4952FD4952FD4952FD4952FECD0A4FFFFFFFFFFFFFFFFFFFFFFFFE1B670
          D4952FD4952FD4952FD4952FD4952FE9CA97FFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFECD0A4D4952FD4952FD4952FD4
          952FD4952FDFB063FFFFFFFFFFFFFFFFFFFFFFFFECD0A4D4952FD4952FD4952F
          D4952FD4952FE4BD7DD4952FD4952FD4952FD4952FD4952FE1B670FFFFFFFFFF
          FFFFFFFFFFFFFFECD0A4D4952FD4952FD4952FD4952FD4952FDFB063FFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF7EBD8
          D4952FD4952FD4952FD4952FD4952FD4952FFFFFFFFFFFFFFFFFFFFFFFFFF7EB
          D8D4952FD4952FD4952FD4952FD4952FE1B670D79C3CD4952FD4952FD4952FD4
          952FD79C3CFFFFFFFFFFFFFFFFFFFFFFFFF7EBD8D4952FD4952FD4952FD4952F
          D4952FD4952FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFCF8F2
          D4952FD4952FD4952FD4952FD4952FD4952FE4BD7DFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFCF8F2D9A249D4952FD4952FD4952FD4952FD4952FD4952FD4952FD4952FD4
          952FD4952FF7EBD8FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFCF8F2D9A249D4952FD4952FD4952FD4952FD4952FD4952FD4
          952FD4952FD4952FD4952FF7EBD8FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFE1B670D4952FD4952FD4952FD4952FD4952FE7C48AD9A249D4952FD4
          952FD4952FD4952FD9A249FFFFFFFFFFFFFFFFFFFFFFFFFAF2E5D4952FD4952F
          D4952FD4952FD4952FE4BD7DDCA956D4952FD4952FD4952FD4952FD4952FF2DE
          BEFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFEFD7B1D79C3CD4952FD4952FEFD7B1FFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFF4E4CBD4952FD4952FD4952FD4952FD4952FD4952FE9CA
          97FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF2DEBED495
          2FD4952FD4952FD4952FD4952FD4952FEFD7B1FFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFCF8F2D4952FD4952FD4952FD4952FD4952FD4952FE4BD7DFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFF2DEBED4952FD4952FD4952FD4952FD4952FD4952FF4E4CBFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFECD0A4D4
          952FD4952FD4952FD4952FD4952FDCA956E4BD7DD4952FD4952FD4952FD4952F
          D4952FFAF2E5FFFFFFFFFFFFFFFFFFFFFFFFD9A249D4952FD4952FD4952FD495
          2FD9A249E7C48AD4952FD4952FD4952FD4952FD4952FE7C48AFFFFFFFFFFFFFF
          FFFFFFFFFF00FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFD89931D89931D89931D899
          31D89931D89931F3DFBEFFFFFFFFFFFFFFFFFFFFFFFFF0D9B1D89931D89931D8
          9931D89931D89931D89931D89931D89931D89931D89931D89931FAF2E5FFFFFF
          FFFFFFFFFFFFFFFFFFEBCC98D89931D89931D89931D89931D89931DFAC58FFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFE2
          B365D89931D89931D89931D89931D89931E9C68BFFFFFFFFFFFFFFFFFFFFFFFF
          FAF2E5D89931D89931D89931D89931D89931D89931D89931D89931D89931D899
          31D89931F0D9B1FFFFFFFFFFFFFFFFFFFFFFFFF5E5CBD89931D89931D89931D8
          9931D89931D89931FDF9F2FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFEBCC98D89931D89931D89931D89931D89931DFAC58FFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFDDA64BD89931D89931D89931D89931D89931D8
          9931D89931D89931D89931D89931E7BF7FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          D89931D89931D89931D89931D89931D89931F3DFBEFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFDFAC58D89931D89931D89931D89931D89931DA9F
          3EFDF9F2FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFDF9F2DFAC58D89931D89931D89931D89931D89931D8
          9931D89931D89931D89931D89931D89931DA9F3EF8ECD8FFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFDF9F2DFAC58D89931D89931D89931D8
          9931D89931D89931D89931D89931D89931D89931D89931DA9F3EF8ECD8FFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEBCC98D89931D89931D89931D89931D8
          9931DA9F3ED89931D89931D89931D89931D89931EBCC98FFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFDFAC58D89931D89931D89931D89931DA9F3ED89931D89931D899
          31D89931D89931D89931F8ECD8FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF8ECD8D89931D89931D89931D89931
          D89931F5E5CBFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFE2B365D89931D899
          31D89931D89931D89931DA9F3EFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFDF9F2D89931D89931D89931D89931D89931D89931DDA64BFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFDFAC58D89931D89931D89931D89931D8
          9931DA9F3EFDF9F2FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFE2B365D89931D89931D89931D89931D899
          31DA9F3EFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFF5E5CBD89931D89931D89931D89931D89931D89931DA9F3E
          D89931D89931D89931D89931E2B365FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFE9C6
          8BD89931D89931D89931D89931D89931DA9F3ED89931D89931D89931D89931D8
          9931EED2A5FFFFFFFFFFFFFFFFFFFFFFFFA2FFFFFFFFFFFFFFFFFFFFFFFFF4E0
          BFDC9C33DC9C33DC9C33DC9C33DC9C33DC9C33FDF9F2FFFFFFFFFFFFFFFFFFFF
          FFFFFDF9F2DC9C33DC9C33DC9C33DC9C33DC9C33DC9C33DC9C33DC9C33DC9C33
          DC9C33E3AF59FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF6E6CCDC9C33DC9C33DC9C
          33DC9C33DC9C33DC9C33FDF9F2FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFDF9F2DC9C33DC9C33DC9C33DC9C33DC9C33DC9C33F4E0BF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFE3AF59DC9C33DC9C33DC9C33DC9C33DC9C
          33DC9C33DC9C33DC9C33DC9C33DC9C33FDF9F2FFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFDC9C33DC9C33DC9C33DC9C33DC9C33DC9C33F4E0BFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFE3AF59DC9C33DC9C33DC9C
          33DC9C33DC9C33EBC78DFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEBC78DDC9C33DC
          9C33DC9C33DC9C33DC9C33DC9C33DC9C33DC9C33DC9C33DC9C33F4E0BFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFE5B566DC9C33DC9C33DC9C33DC9C33DC9C33EBC7
          8DFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF0D4A5DC9C33DC9C
          33DC9C33DC9C33DC9C33DC9C33EDCD99FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFE7BB73DC9C33DC9C33DC
          9C33DC9C33DC9C33DC9C33DC9C33DC9C33DC9C33DC9C33DC9C33DC9C33DC9C33
          E3AF59FDF9F2FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFE7BB73DC
          9C33DC9C33DC9C33DC9C33DC9C33DC9C33DC9C33DC9C33DC9C33DC9C33DC9C33
          DC9C33DC9C33E3AF59FDF9F2FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF6E6CCDC
          9C33DC9C33DC9C33DC9C33DC9C33DC9C33DC9C33DC9C33DC9C33DC9C33DC9C33
          FBF3E5FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF2DAB2DC9C33DC9C33DC9C33DC9C
          33DC9C33DC9C33DC9C33DC9C33DC9C33DC9C33DC9C33FFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF6E6CC
          DC9C33DC9C33DC9C33DC9C33DC9C33E7BB73FFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFEDCD99DC9C33DC9C33DC9C33DC9C33DC9C33DC9C33F8ECD9FFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFE7BB73DC9C33DC9C33DC
          9C33DC9C33DC9C33DC9C33F0D4A5FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF0D4A5DC
          9C33DC9C33DC9C33DC9C33DC9C33DC9C33EDCD99FFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF4E0BFDC9C33DC9C
          33DC9C33DC9C33DC9C33DC9C33E9C180FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFDC9C33DC9C33DC9C33
          DC9C33DC9C33DC9C33DC9C33DC9C33DC9C33DC9C33DC9C33F2DAB2FFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFBF3E5DC9C33DC9C33DC9C33DC9C33DC9C33DC9C33DC
          9C33DC9C33DC9C33DC9C33DC9C33F6E6CCFFFFFFFFFFFFFFFFFFFFFFFFCAFFFF
          FFFFFFFFFFFFFFFFFFFFEECA8DE0A034E0A034E0A034E0A034E0A034E4AC4DFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEABE74E0A034E0A034E0A034E0A034
          E0A034E0A034E0A034E0A034E0A034F1D5A6FFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFE0A034E0A034E0A034E0A034E0A034E0A034F5E1BFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF5E1BFE0A034E0A034E0A034
          E0A034E0A034E0A034FBF3E6FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF1D5A6E0A0
          34E0A034E0A034E0A034E0A034E0A034E0A034E0A034E0A034EABE74FFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFE8B867E0A034E0A034E0A034E0A034E0A034
          EECA8DFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFDF9
          F2E0A034E0A034E0A034E0A034E0A034E0A034F3DBB3FFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFF9EDD9E0A034E0A034E0A034E0A034E0A034E0A034E0A034E0A034
          E0A034E2A641FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFCF99E0A034E0A0
          34E0A034E0A034E0A034E6B25AFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFDF9F2E2A641E0A034E0A034E0A034E0A034E0A034E0A034F3DBB3FF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FBF3E6EECA8DE4AC4DE0A034EABE74FDF9F2FFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEC
          C480E0A034E0A034E0A034E0A034E0A034E0A034E4AC4DFBF3E6E6B25AE0A034
          E0A034E0A034E0A034E0A034E0A034E6B25AFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFECC480E0A034E0A034E0A034E0A034E0A034E0A034E4AC4DFBF3E6
          E6B25AE0A034E0A034E0A034E0A034E0A034E0A034E6B25AFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFDF9F2E0A034E0A034E0A034E0A034E0A034E0A034E0A034
          E0A034E0A034E0A034E6B25AFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFDF9
          F2E0A034E0A034E0A034E0A034E0A034E0A034E0A034E0A034E0A034E0A034E8
          B867FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFF9EDD9E0A034E0A034E0A034E0A034E0A034E0A034FBF3
          E6FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEECA8DE0A034E0A034E0A034E0A034E0
          A034E0A034F7E7CCFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFF7E7CCE0A034E0A034E0A034E0A034E0A034E0A034E0A034F3DBB3FFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFDF9F2E2A641E0A034E0A034E0A034E0A034E0A034E0A034
          F3DBB3FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFF9EDD9E2A641E0A034E0A034E0A034E0A034E0A034E0A034FBF3E6FFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFE6B25AE0A034E0A034E0A034E0A034E0A034E0A034E0A034E0A034E0A0
          34E0A034FDF9F2FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFE6B25AE0A034E0
          A034E0A034E0A034E0A034E0A034E0A034E0A034E0A034E0A034FFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFF33FFFFFFFFFFFFFFFFFFFFFFFFE9B45CE4A336E4A336E4
          A336E4A336E4A336EEC682FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF5DCB3
          E4A336E4A336E4A336E4A336E4A336E4A336E4A336E4A336E4A336FDF9F2FFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEBBA68E4A336E4A336E4A336E4A336E4
          A336EEC682FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          F0CB8EE4A336E4A336E4A336E4A336E4A336E7AF4FFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFCF3E6E4A336E4A336E4A336E4A336E4A336E4A336E4A336E4
          A336E4A336F7E2C0FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF1D19AE4A336
          E4A336E4A336E4A336E4A336E7AF4FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFF7E2C0E4A336E4A336E4A336E4A336E4A336E4A336FC
          F3E6FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFE7AF4FE4A336E4A336E4A336
          E4A336E4A336E4A336E4A336E4A336F0CB8EFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFF8E8CDE4A336E4A336E4A336E4A336E4A336E4A336FCF3E6FFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF3D7A7E4A336E4A336E4A336E4
          A336E4A336E4A336E4A336F0CB8EFDF9F2FFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFCF3E6F1D19AE6A943E4A336E4A336E4A336E4A336ECC075FFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFF5DCB3E4A336E4A336E4A336E4A336E4A336E4A336E6A943
          FAEED9FFFFFFFDF9F2E6A943E4A336E4A336E4A336E4A336E4A336E4A336EEC6
          82FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFF5DCB3E4A336E4A336E4A336E4A336E4A336
          E4A336E6A943FAEED9FFFFFFFDF9F2E6A943E4A336E4A336E4A336E4A336E4A3
          36E4A336EEC682FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFE7AF4FE4A336E4A336
          E4A336E4A336E4A336E4A336E4A336E4A336E4A336F3D7A7FFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFECC075E4A336E4A336E4A336E4A336E4A336E4
          A336E4A336E4A336E4A336F0CB8EFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFE7AF4FE4A336E4A3
          36E4A336E4A336E4A336E9B45CFDF9F2FFFFFFFFFFFFFFFFFFFCF3E6E6A943E4
          A336E4A336E4A336E4A336E4A336E4A336FFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFECC075E4A336E4A336E4A336E4A336
          E4A336E4A336E4A336ECC075FAEED9FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFDF9F2F3D7A7F0CB8EEBBA68F0CB8EFDF9F2FFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF3D7A7E4A336E4A336
          E4A336E4A336E4A336E4A336E4A336F0CB8EFDF9F2FFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFF1D19AE6A943E4A336E4A336E4A336E4A336E4A336E4
          A336F0CB8EFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEEC682E4A336E4A336E4A336E4A336E4A3
          36E4A336E4A336E4A336E4A336ECC075FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFF3D7A7E4A336E4A336E4A336E4A336E4A336E4A336E4A336E4A336
          E4A336E9B45CFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF00FFFFFFFFFFFFFFFFFFFC
          F4E6E7A637E7A637E7A637E7A637E7A637E7A637F6DDB4FFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFE9AC44E7A637E7A637E7A637E7A637E7A637E7A6
          37E7A637EFC276FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF3D29BE7
          A637E7A637E7A637E7A637E7A637EAB150FFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFEAB150E7A637E7A637E7A637E7A637E7A637F0C8
          82FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFC276E7A637E7A637E7
          A637E7A637E7A637E7A637E7A637E9AC44FFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFF9E9CDE7A637E7A637E7A637E7A637E7A637E7A637FCF4E6FFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF0C882E7A637E7A637E7
          A637E7A637E7A637EAB150FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          F4D8A7E7A637E7A637E7A637E7A637E7A637E7A637E7A637E7A637FAEED9FFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFE7A637E7A637E7A637E7A637E7
          A637E7A637F6DDB4FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFEFC276E7A637E7A637E7A637E7A637E7A637E7A637E7A637E9AC44EFC276
          F4D8A7F9E9CDF9E9CDF7E3C0F3D29BEDBC69E7A637E7A637E7A637E7A637E7A6
          37E7A637E7A637E9AC44FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF7E3C0E7A637E7A637E7A637E7A637
          E7A637E7A637E7A637F7E3C0FFFFFFFFFFFFFFFFFFFAEED9E9AC44E7A637E7A6
          37E7A637E7A637E7A637E7A637F3D29BFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF7E3C0E7A637E7A637
          E7A637E7A637E7A637E7A637E7A637F7E3C0FFFFFFFFFFFFFFFFFFFAEED9E9AC
          44E7A637E7A637E7A637E7A637E7A637E7A637F3D29BFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFEFC276E7A637E7A637E7A637E7A637E7A637E7A637E7A637E7A637E7A6
          37FCF4E6FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF6DDB4E7A637E7
          A637E7A637E7A637E7A637E7A637E7A637E7A637E7A637F4D8A7FFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFF3D29BE7A637E7A637E7A637E7A637E7A637E7A637EAB150F6DDB4F9
          E9CDF4D8A7E9AC44E7A637E7A637E7A637E7A637E7A637E7A637EFC276FFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFDF9F2
          EAB150E7A637E7A637E7A637E7A637E7A637E7A637E7A637E7A637EAB150EDBC
          69F3D29BF3D29BEDBC69ECB75DE7A637E7A637E7A637E7A637E7A637EDBC69FF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFEFC276E7A637E7A637E7A637E7A637E7A637E7A637E7A637E9AC
          44EFC276F4D8A7F9E9CDF9E9CDF4D8A7F0C882E9AC44E7A637E7A637E7A637E7
          A637E7A637E7A637E7A637ECB75DFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF4D8A7E7A6
          37E7A637E7A637E7A637E7A637E7A637E7A637E7A637E7A637F6DDB4FFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFCF4E6E7A637E7A637E7A637E7A637
          E7A637E7A637E7A637E7A637E7A637EFC276FFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFF0FFFFFFFFFFFFFFFFFFF7DFB4EAA939EAA939EAA939EAA939EAA939EAA939
          FBEFDAFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF2C984EAA939EAA9
          39EAA939EAA939EAA939EAA939EAA939F7DFB4FFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFAE9CDEAA939EAA939EAA939EAA939EAA939EAA939FCF4E6
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFCF4E6EAA939EAA939EAA9
          39EAA939EAA939EAA939F6D9A8FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFF7DFB4EAA939EAA939EAA939EAA939EAA939EAA939EAA939F2C984FFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEAA939EAA939EAA939EAA9
          39EAA939EAA939F7DFB4FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFEDB452EAA939EAA939EAA939EAA939EAA939F1C477FFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFCF4E6EAA939EAA939EAA939EAA939EAA939EAA9
          39EAA939EDB452FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEF
          BF6BEAA939EAA939EAA939EAA939EAA939F2C984FFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFAF3EEB95EEAA939EAA939EAA939EAA939
          EAA939EAA939EAA939EAA939EAA939EAA939EAA939EAA939EAA939EAA939EAA9
          39EAA939EAA939EAA939EAA939EAA939EAA939F3CF90FFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFBEFDAEBAE45
          EAA939EAA939EAA939EAA939EAA939EAA939F4D49CFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFF7DFB4EAA939EAA939EAA939EAA939EAA939EAA939EAA939F7DFB4FF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FBEFDAEBAE45EAA939EAA939EAA939EAA939EAA939EAA939F4D49CFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFF7DFB4EAA939EAA939EAA939EAA939EAA939EAA939EA
          A939F7DFB4FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF4D49CEAA939EAA939EAA939EAA939EAA9
          39EAA939EAA939EAA939EFBF6BFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFEFAF3EAA939EAA939EAA939EAA939EAA939EAA939EAA939EAA939
          EAA939FAE9CDFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFAF3EDB452EAA939EAA939EAA939EA
          A939EAA939EAA939EAA939EAA939EAA939EAA939EAA939EAA939EAA939EAA939
          EAA939EAA939FAE9CDFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFBEFDAEBAE45EAA939EAA939EAA939EAA939EAA9
          39EAA939EAA939EAA939EAA939EAA939EAA939EAA939EAA939EAA939EAA939EA
          A939EAA939EAA939EAA939FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFAF3EEB95EEAA939EAA939EAA9
          39EAA939EAA939EAA939EAA939EAA939EAA939EAA939EAA939EAA939EAA939EA
          A939EAA939EAA939EAA939EAA939EAA939EAA939EEB95EFEFAF3FFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFAE9CDEAA939EAA939EAA939EAA939EAA939EAA939EAA939EA
          A939EBAE45FEFAF3FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          EEB95EEAA939EAA939EAA939EAA939EAA939EAA939EAA939EAA939F4D49CFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFF33FFFFFFFFFFFFFFFFFFF3C678EDAC3AEDAC3A
          EDAC3AEDAC3AEDAC3AEEB146FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFF9E5C1EDAC3AEDAC3AEDAC3AEDAC3AEDAC3AEDAC3AEDAC3AFEFAF3FF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEDAC3AEDAC3AEDAC3A
          EDAC3AEDAC3AEDAC3AF8E0B5FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFF7DBA8EDAC3AEDAC3AEDAC3AEDAC3AEDAC3AEDAC3AFCEFDAFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFAF3EDAC3AEDAC3AEDAC3AEDAC3AEDAC3A
          EDAC3AEDAC3AF9E5C1FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFF2C16BEDAC3AEDAC3AEDAC3AEDAC3AEDAC3AF4CB84FFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFCEFDAEDAC3AEDAC3AEDAC3AEDAC3AEDAC3AEDAC3A
          F7DBA8FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF0BC5FEDAC
          3AEDAC3AEDAC3AEDAC3AEDAC3AEDAC3AF5D091FFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFF6D59CEDAC3AEDAC3AEDAC3AEDAC3AEDAC3AEFB653
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          F4CB84EDAC3AEDAC3AEDAC3AEDAC3AEDAC3AEDAC3AEDAC3AEDAC3AEDAC3AEDAC
          3AEDAC3AEDAC3AEDAC3AEDAC3AEDAC3AEDAC3AEDAC3AEDAC3AEDAC3AF0BC5FFE
          FAF3FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFF2C16BEDAC3AEDAC3AEDAC3AEDAC3AEDAC3AEDAC3AF3C678FFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF6D59CEDAC3AEDAC3AEDAC3AED
          AC3AEDAC3AEDAC3AF0BC5FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFF2C16BEDAC3AEDAC3AEDAC3AEDAC3AEDAC3AEDAC
          3AF3C678FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF6D59CEDAC3AED
          AC3AEDAC3AEDAC3AEDAC3AEDAC3AF0BC5FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFAEACEEDAC
          3AEDAC3AEDAC3AEDAC3AEDAC3AEDAC3AEDAC3AEDAC3AF8E0B5FFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF4CB84EDAC3AEDAC3AEDAC3A
          EDAC3AEDAC3AEDAC3AEDAC3AEDAC3AFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFC
          EFDAEEB146EDAC3AEDAC3AEDAC3AEDAC3AEDAC3AEDAC3AEDAC3AEDAC3AEDAC3A
          EDAC3AEDAC3AEDAC3AEDAC3AEDAC3AF4CB84FFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFAF3F0BC
          5FEDAC3AEDAC3AEDAC3AEDAC3AEDAC3AEDAC3AEDAC3AEDAC3AEDAC3AEDAC3AED
          AC3AEDAC3AEDAC3AEDAC3AEDAC3AEDAC3AEDAC3AEFB653FFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFF4CB84EDAC3AEDAC3AEDAC3AEDAC3AEDAC3AEDAC3AEDAC3AEDAC3AED
          AC3AEDAC3AEDAC3AEDAC3AEDAC3AEDAC3AEDAC3AEDAC3AEDAC3AEDAC3AF2C16B
          FEFAF3FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEDAC3AEDAC3AEDAC3AED
          AC3AEDAC3AEDAC3AEDAC3AEDAC3AF4CB84FFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFF8E0B5EDAC3AEDAC3AEDAC3AEDAC3AEDAC3AEDAC
          3AEDAC3AEDAC3AFAEACEFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF0FFFFFFFFFFFF
          FFFFFFF6CC85F0AE3BF0AE3BF0AE3BF0AE3BF0AE3BF6CC85FFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF1B347F0AE3BF0AE3BF0AE3BF0
          AE3BF0AE3BF6CC85FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFF3BD60F0AE3BF0AE3BF0AE3BF0AE3BF0AE3BFAE6C2FFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFF9E1B5F0AE3BF0AE3BF0AE3BF0AE3BF0AE3BF2
          B854FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF5C778
          F0AE3BF0AE3BF0AE3BF0AE3BF0AE3BF2B854FFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFF7D291F0AE3BF0AE3BF0AE3BF0AE3BF0AE3BF7
          D291FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFDF5E6F0AE3BF0AE3B
          F0AE3BF0AE3BF0AE3BF0AE3BFDF5E6FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFF8DBA9F0AE3BF0AE3BF0AE3BF0AE3BF0AE3BF0AE3BFDF5E6FF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFAE6C2F0AE3BF0AE3B
          F0AE3BF0AE3BF0AE3BF3BD60FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFBEBCEF2B854F0AE3BF0AE3BF0AE3BF0AE
          3BF0AE3BF0AE3BF0AE3BF0AE3BF0AE3BF0AE3BF0AE3BF0AE3BF0AE3BF0AE3BF0
          AE3BF1B347F7D69DFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF5C778F0AE3BF0AE3BF0AE3BF0AE
          3BF0AE3BF3BD60FEFAF3FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFF6CC85F0AE3BF0AE3BF0AE3BF0AE3BF0AE3BF5C778FFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF5C778F0AE3BF0AE
          3BF0AE3BF0AE3BF0AE3BF3BD60FEFAF3FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFF6CC85F0AE3BF0AE3BF0AE3BF0AE3BF0AE3BF5C778FFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFF3BD60F0AE3BF0AE3BF0AE3BF0AE3BF0AE3BF0AE3BF2
          B854FEFAF3FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FBEBCEF0AE3BF0AE3BF0AE3BF0AE3BF0AE3BF0AE3BF0AE3BF7D69DFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFDF5E6F4C26CF0AE3BF0AE3BF0AE3BF0AE3B
          F0AE3BF0AE3BF0AE3BF0AE3BF0AE3BF0AE3BF0AE3BF0AE3BF7D291FFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFF8DBA9F1B347F0AE3BF0AE3BF0AE3BF0AE3BF0
          AE3BF0AE3BF0AE3BF0AE3BF0AE3BF0AE3BF0AE3BF0AE3BF0AE3BF0AE3BF1B347
          FCF0DAFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFBEBCEF2B854F0AE3BF0AE3BF0
          AE3BF0AE3BF0AE3BF0AE3BF0AE3BF0AE3BF0AE3BF0AE3BF0AE3BF0AE3BF0AE3B
          F0AE3BF2B854F9E1B5FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFF7D291F0AE3BF0AE3BF0AE3BF0AE3BF0AE3BF0AE3BF0AE3BFCF0DAFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFAF3F1B347F0AE
          3BF0AE3BF0AE3BF0AE3BF0AE3BF0AE3BF4C26CFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFF00FFFFFFFFFFFFFFFFFFFDF0DAF3B548F2B03CF2B03CF2B03CF4BF
          61FEFAF3FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFB
          E6C2F3B548F2B03CF2B03CF2B03CF4BF61FEFAF3FFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFCEBCEF3B548F2B03CF2B03CF2B03CF4BF
          61FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFAF3F4BF61F2
          B03CF2B03CF2B03CF3B548FDF0DAFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFDF5E7F4BA54F2B03CF2B03CF2B03CF3B548FDF0DAFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFAF3F4BA54F2
          B03CF2B03CF2B03CF3B548FDF5E7FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFF7CE85F2B03CF2B03CF2B03CF2B03CFAE1B6FFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF5C46DF2B03CF2B03CF2
          B03CF2B03CFAE1B6FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFF6C979F2B03CF2B03CF2B03CF2B03CFBE6C2FFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFCEBCEF7CE85F3B548F2B03CF2B03CF2B03CF2B03CF2B03CF2B03CF2B03CF2
          B03CF2B03CF2B03CF5C46DF9DCA9FEFAF3FFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFDF0
          DAF3B548F2B03CF2B03CF2B03CF5C46DFEFAF3FFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFAF3F7CE85F2B03CF2B03CF2B03CF3B548
          FDF0DAFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFDF0DAF3B548F2B03CF2B03CF2B03CF5C46DFEFAF3FFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFAF3F7CE85F2B03CF2B03C
          F2B03CF3B548FDF0DAFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFDF5E7F4BF61F2B03CF2
          B03CF2B03CF2B03CF4BA54FDF0DAFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFAE1B6F3B548F2B03CF2B03CF2B03CF2B0
          3CF8D392FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FAE1B6F5C46DF2B03CF2B03CF2B03CF2B03CF2B03CF2B03CF2B03CF2B03CF6C9
          79FCEBCEFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFA
          E1B6F6C979F2B03CF2B03CF2B03CF2B03CF2B03CF2B03CF2B03CF2B03CF2B03C
          F2B03CF4BA54F8D79DFEFAF3FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFCEBCEF7CE85F3B548F2B03CF2B03CF2B03CF2B03CF2B03CF2B03C
          F2B03CF2B03CF2B03CF6C979FAE1B6FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFAF3F7CE85F2B03CF2B03CF2B03CF2B03C
          F3B548FAE1B6FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFDF0DAF4BA54F2B03CF2B03CF2B03CF2B03CF5C46DFEFAF3FF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF20FFFFFFFFFFFFFFFFFFFFFFFFFEFA
          F3FADDAAF9D89EFBE2B6FEFAF3FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFEF5E7FADDAAF9D89EFBE2B6FFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEF5
          E7FADDAAF9D89EFCE7C2FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFBE2B6F9D89EFADDAAFEF5E7FFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFAF3FBE2B6F9D8
          9EFADDAAFEFAF3FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFEFAF3FBE2B6F9D89EFBE2B6FEFAF3FFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFCECCEF9D89EF9D89EFDF1
          DAFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFCE7C2F9D89EF9D89EFEF5E7FFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFCE7C2F9D89EFADDAAFEF5
          E7FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFDF1DAFBE2B6F9D89EF7
          CA7AF7C56EF9D492F9D89EFBE2B6FCECCEFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFEFAF3FBE2B6F9D89EFCE7C2FFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FCECCEF9D89EFADDAAFEFAF3FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFAF3FBE2B6F9D89EFCE7C2FF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFCECCEF9D89EFADDAAFEFAF3FFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFCE7C2F9D89EF9D89EFCE7C2FFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFA
          F3FBE2B6F9D89EFADDAAFEF5E7FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFAF3FCE7C2F9D89EF7CA7AF8CF
          86F9D89EFCE7C2FEFAF3FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFCECCEFADDAAF9D89EF7C56E
          F7C56EF9D89EF9D89EFCE7C2FEF5E7FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFDF1DAFBE2B6
          F9D89EF7CA7AF7C56EF9D89EFADDAAFCECCEFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FDF1DAF9D89EF9D89EFBE2B6FEFAF3FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFCE7C2F9D89EF9
          D89EFDF1DAFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF20FFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFF20FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
          FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF20}
        Visible = False
      end
      object GroupBox41: TGroupBox
        Left = 5
        Top = 6
        Width = 655
        Height = 355
        Caption = #29256#26412#20449#24687
        TabOrder = 0
        object LabelVersion: TLabel
          Left = 8
          Top = 16
          Width = 204
          Height = 12
          Caption = #36719#20214#21517#31216': GxxM2'#21453#22806#25346#25968#25454#24341#25806#25511#21046#21488
        end
        object Label60: TLabel
          Left = 8
          Top = 32
          Width = 78
          Height = 12
          Caption = #36719#20214#29256#26412': 1.0'
        end
      end
    end
  end
  object TimerStartGame: TTimer
    Enabled = False
    Interval = 200
    OnTimer = TimerStartGameTimer
    Left = 632
    Top = 64
  end
  object TimerStopGame: TTimer
    Enabled = False
    Interval = 500
    OnTimer = TimerStopGameTimer
    Left = 632
    Top = 96
  end
  object TimerCheckRun: TTimer
    Enabled = False
    Interval = 2000
    OnTimer = TimerCheckRunTimer
    Left = 632
    Top = 32
  end
  object ServerSocket: TServerSocket
    Active = False
    Address = '0.0.0.0'
    Port = 6350
    ServerType = stNonBlocking
    Left = 632
  end
  object Timer: TTimer
    Enabled = False
    Interval = 10
    Left = 632
    Top = 128
  end
  object TimerStart: TTimer
    Enabled = False
    Interval = 100
    OnTimer = TimerStartTimer
    Left = 632
    Top = 224
  end
  object TimerClose: TTimer
    Enabled = False
    Interval = 100
    OnTimer = TimerCloseTimer
    Left = 632
    Top = 192
  end
  object TimerAutoStartServer: TTimer
    Enabled = False
    OnTimer = TimerAutoStartServerTimer
    Left = 632
    Top = 160
  end
  object ClearServerOpenDialog: TOpenDialog
    Left = 91
    Top = 119
  end
end
