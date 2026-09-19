object FrmMain: TFrmMain
  Left = 399
  Top = 281
  Caption = 'GxxM2'#32593#20851
  ClientHeight = 601
  ClientWidth = 716
  Color = clBtnFace
  Font.Charset = GB2312_CHARSET
  Font.Color = clBlack
  Font.Height = -12
  Font.Name = #23435#20307
  Font.Style = []
  KeyPreview = True
  Menu = mmMain
  OldCreateOrder = False
  Position = poMainFormCenter
  OnCloseQuery = FormCloseQuery
  OnCreate = FormCreate
  OnDestroy = FormDestroy
  PixelsPerInch = 96
  TextHeight = 12
  object pgcMain: TPageControl
    Left = 0
    Top = 0
    Width = 716
    Height = 601
    ActivePage = tsRunInfo
    Align = alClient
    TabOrder = 0
    object tsRunInfo: TTabSheet
      Caption = #20449#24687#26085#24535
      object splInfoBottom: TSplitter
        Left = 0
        Top = 439
        Width = 708
        Height = 4
        Cursor = crVSplit
        Align = alBottom
        ExplicitTop = 300
      end
      object mmoMainLog: TMemo
        Left = 0
        Top = 0
        Width = 708
        Height = 439
        Align = alClient
        Color = clBlack
        Font.Charset = GB2312_CHARSET
        Font.Color = clLime
        Font.Height = -12
        Font.Name = #23435#20307
        Font.Style = []
        ParentFont = False
        ReadOnly = True
        ScrollBars = ssBoth
        TabOrder = 0
        OnDblClick = mmoMainLogDblClick
      end
      object pnlInfoBottom: TPanel
        Left = 0
        Top = 443
        Width = 708
        Height = 130
        Align = alBottom
        BevelOuter = bvNone
        TabOrder = 1
        object lvRunGates: TListView
          Left = 0
          Top = 0
          Width = 708
          Height = 111
          Align = alClient
          Columns = <
            item
              Caption = 'ID'
              Width = 26
            end
            item
              Caption = #31471#21475
              Width = 38
            end
            item
              Caption = #24403#21069#36830#25509
              Width = 60
            end
            item
              Caption = #26368#39640#36830#25509
              Width = 60
            end
            item
              Caption = #25910#29992#25143#25968#25454
              Width = 74
            end
            item
              Caption = #21457#29992#25143#25968#25454
              Width = 74
            end
            item
              Caption = #32047#35745#25910#29992#25143#25968#25454
              Width = 98
            end
            item
              Caption = #32047#35745#21457#29992#25143#25968#25454
              Width = 98
            end
            item
              Caption = #26381#21153#22120#36830#25509
              Width = 72
            end
            item
              Caption = #23458#25143#31471#30417#21548
              Width = 72
            end>
          Font.Charset = GB2312_CHARSET
          Font.Color = clBlack
          Font.Height = -12
          Font.Name = #23435#20307
          Font.Style = []
          GridLines = True
          RowSelect = True
          ParentFont = False
          TabOrder = 0
          ViewStyle = vsReport
        end
        object stat: TStatusBar
          Left = 0
          Top = 111
          Width = 708
          Height = 19
          Panels = <
            item
              Width = 500
            end
            item
              Width = 50
            end>
        end
      end
    end
    object tsOnline: TTabSheet
      BorderWidth = 1
      Caption = #22312#32447#29992#25143
      ImageIndex = 1
      object splOnlineUser: TSplitter
        Left = 0
        Top = 346
        Width = 706
        Height = 5
        Cursor = crVSplit
        Align = alBottom
        Color = clBtnFace
        ParentColor = False
        ExplicitTop = 207
      end
      object pnlOnlineUserBottom: TGroupBox
        Left = 0
        Top = 351
        Width = 706
        Height = 220
        Align = alBottom
        Caption = #23458#25143#31471#20449#24687#26174#31034
        TabOrder = 1
        DesignSize = (
          706
          220)
        object Label24: TLabel
          Left = 151
          Top = 15
          Width = 60
          Height = 12
          Caption = #25628#32034#36827#31243#65306
        end
        object Label25: TLabel
          Left = 8
          Top = 15
          Width = 48
          Height = 12
          Caption = #35282#33394#21517#65306
        end
        object lblContextCharName: TLabel
          Left = 52
          Top = 15
          Width = 92
          Height = 12
          AutoSize = False
        end
        object Label27: TLabel
          Left = 8
          Top = 202
          Width = 60
          Height = 12
          Anchors = [akLeft, akBottom]
          Caption = #25805#20316#29366#24577#65306
          Font.Charset = GB2312_CHARSET
          Font.Color = clBlue
          Font.Height = -12
          Font.Name = #23435#20307
          Font.Style = []
          ParentFont = False
        end
        object lblContextStatus: TLabel
          Left = 66
          Top = 202
          Width = 634
          Height = 12
          Anchors = [akLeft, akBottom]
          AutoSize = False
          Font.Charset = GB2312_CHARSET
          Font.Color = clBlue
          Font.Height = -12
          Font.Name = #23435#20307
          Font.Style = []
          ParentFont = False
          OnClick = lblContextStatusClick
          OnMouseEnter = lblContextStatusMouseEnter
          OnMouseLeave = lblContextStatusMouseLeave
        end
        object lblRooDir: TLabel
          Left = 66
          Top = 187
          Width = 343
          Height = 12
          Anchors = [akLeft, akBottom]
          AutoSize = False
          Font.Charset = GB2312_CHARSET
          Font.Color = clBlue
          Font.Height = -12
          Font.Name = #23435#20307
          Font.Style = []
          ParentFont = False
          OnClick = lblContextStatusClick
          OnMouseEnter = lblContextStatusMouseEnter
          OnMouseLeave = lblContextStatusMouseLeave
        end
        object Label51: TLabel
          Left = 8
          Top = 187
          Width = 60
          Height = 12
          Anchors = [akLeft, akBottom]
          Caption = #26681' '#36335' '#24452#65306
          Font.Charset = GB2312_CHARSET
          Font.Color = clBlue
          Font.Height = -12
          Font.Name = #23435#20307
          Font.Style = []
          ParentFont = False
        end
        object lblSaveDir: TLabel
          Left = 518
          Top = 187
          Width = 179
          Height = 12
          Anchors = [akLeft, akBottom]
          AutoSize = False
          Font.Charset = GB2312_CHARSET
          Font.Color = clBlue
          Font.Height = -12
          Font.Name = #23435#20307
          Font.Style = []
          ParentFont = False
          OnClick = lblContextStatusClick
          OnMouseEnter = lblContextStatusMouseEnter
          OnMouseLeave = lblContextStatusMouseLeave
        end
        object Label52: TLabel
          Left = 448
          Top = 187
          Width = 72
          Height = 12
          Anchors = [akLeft, akBottom]
          Caption = #20445#23384#25991#20214#22841#65306
          Font.Charset = GB2312_CHARSET
          Font.Color = clBlue
          Font.Height = -12
          Font.Name = #23435#20307
          Font.Style = []
          ParentFont = False
        end
        object lvContextProcessListInfo: TListView
          Left = 8
          Top = 36
          Width = 690
          Height = 149
          Anchors = [akLeft, akTop, akRight, akBottom]
          Columns = <
            item
              Caption = #25991#20214#21517
              Width = 150
            end
            item
              AutoSize = True
              Caption = #25991#20214#36335#24452
            end
            item
              Caption = 'MD5'
              Width = 212
            end>
          ColumnClick = False
          GridLines = True
          ReadOnly = True
          RowSelect = True
          PopupMenu = pmProcessList
          TabOrder = 0
          ViewStyle = vsReport
          OnDblClick = lvContextProcessListInfoDblClick
        end
        object edtSearch: TEdit
          Left = 207
          Top = 11
          Width = 153
          Height = 20
          Anchors = [akLeft, akTop, akRight]
          TabOrder = 1
        end
        object btnRefreshContextProcessList: TButton
          Left = 498
          Top = 10
          Width = 65
          Height = 22
          Anchors = [akTop, akRight]
          Caption = #21047#26032#36827#31243
          TabOrder = 2
          OnClick = btnRefreshContextProcessListClick
        end
        object btnScreenshotGame: TButton
          Left = 565
          Top = 10
          Width = 65
          Height = 22
          Anchors = [akTop, akRight]
          Caption = #28216#25103#24555#29031
          TabOrder = 3
          OnClick = btnScreenshotGameClick
        end
        object btnScreenshot: TButton
          Left = 632
          Top = 10
          Width = 65
          Height = 22
          Anchors = [akTop, akRight]
          Caption = #26700#38754#24555#29031
          TabOrder = 4
          OnClick = btnScreenshotClick
        end
        object btnSearch: TButton
          Left = 370
          Top = 10
          Width = 49
          Height = 22
          Anchors = [akTop, akRight]
          Caption = #25628#32034
          TabOrder = 5
          OnClick = btnSearchClick
        end
        object btnNextSearch: TButton
          Left = 421
          Top = 10
          Width = 71
          Height = 22
          Anchors = [akTop, akRight]
          Caption = #25628#32034#19979#19968#20010
          TabOrder = 6
          OnClick = btnNextSearchClick
        end
      end
      object pnlOnlineUser: TPanel
        Left = 0
        Top = 0
        Width = 706
        Height = 346
        Align = alClient
        BevelOuter = bvNone
        ParentColor = True
        TabOrder = 0
        object lvOnLine: TListView
          Left = 0
          Top = 29
          Width = 706
          Height = 317
          Align = alClient
          Columns = <
            item
              Caption = #24207#21495
              Width = 36
            end
            item
              Caption = #30331#38470'IP'#22320#22336
              Width = 76
            end
            item
              Caption = #26631#35782
              Width = 36
            end
            item
              Caption = #24080#25143#21517
              Width = 80
            end
            item
              Caption = #21517#31216
              Width = 78
            end
            item
              Caption = #26426#22120#30721
              Width = 140
            end
            item
              Caption = #29256#26412#21495
            end
            item
              Caption = #23458#25143#31471#21457#36865
              Width = 80
            end
            item
              Caption = #24403#21069#29366#24577
              Width = 60
            end
            item
              Caption = #32593#20851'ID'
              Width = 48
            end
            item
              Caption = #25991#20214#25509#25910
              Width = 200
            end>
          Font.Charset = GB2312_CHARSET
          Font.Color = clBlack
          Font.Height = -12
          Font.Name = #23435#20307
          Font.Style = []
          GridLines = True
          ReadOnly = True
          RowSelect = True
          ParentFont = False
          PopupMenu = pmUser
          TabOrder = 1
          ViewStyle = vsReport
          OnClick = lvOnLineClick
          OnDblClick = lvOnLineDblClick
        end
        object pnlOnlineUserSearch: TPanel
          Left = 0
          Top = 0
          Width = 706
          Height = 29
          Align = alTop
          BevelOuter = bvNone
          ParentColor = True
          TabOrder = 0
          object lbl2: TLabel
            Left = 0
            Top = 8
            Width = 60
            Height = 12
            Caption = #25628#32034#23383#27573#65306
          end
          object lbl3: TLabel
            Left = 208
            Top = 8
            Width = 60
            Height = 12
            Caption = #25628#32034#20869#23481#65306
          end
          object edtSearchOnlineText: TEdit
            Left = 264
            Top = 4
            Width = 225
            Height = 20
            TabOrder = 1
            OnKeyDown = edtSearchOnlineTextKeyDown
          end
          object cbbSearchOnlineField: TComboBox
            Left = 56
            Top = 4
            Width = 121
            Height = 20
            Style = csDropDownList
            ItemHeight = 12
            ItemIndex = 0
            TabOrder = 0
            Text = #25628#32034#24080#25143
            Items.Strings = (
              #25628#32034#24080#25143
              #25628#32034#21517#31216
              #25628#32034'IP'
              #25628#32034#26426#22120#30721)
          end
          object chkSearchFuzzyMatch: TCheckBox
            Left = 498
            Top = 6
            Width = 73
            Height = 17
            Caption = #27169#31946#21305#37197
            Checked = True
            State = cbChecked
            TabOrder = 2
          end
          object btnSearchOnline: TButton
            Left = 576
            Top = 3
            Width = 53
            Height = 22
            Caption = #25628#32034
            TabOrder = 3
            OnClick = btnSearchOnlineClick
          end
          object btnSearchOnlineNext: TButton
            Left = 633
            Top = 3
            Width = 71
            Height = 22
            Caption = #25628#32034#19979#19968#20010
            TabOrder = 4
            OnClick = btnSearchOnlineNextClick
          end
        end
      end
    end
    object tsSetting: TTabSheet
      Caption = #35774#32622
      ImageIndex = 3
      object Label47: TLabel
        Left = 157
        Top = 414
        Width = 48
        Height = 12
        Caption = #20154#29289#30331#24405
      end
      object grpNetConfig: TGroupBox
        Left = 8
        Top = 5
        Width = 221
        Height = 186
        Caption = #32593#32476#35774#32622
        TabOrder = 0
        object lblGateIPaddr: TLabel
          Left = 30
          Top = 22
          Width = 60
          Height = 12
          Caption = #32593#20851#22320#22336#65306
        end
        object lblGatePort: TLabel
          Left = 30
          Top = 46
          Width = 60
          Height = 12
          Caption = #32593#20851#31471#21475#65306
        end
        object lblServerPort: TLabel
          Left = 18
          Top = 94
          Width = 72
          Height = 12
          Caption = #26381#21153#22120#31471#21475#65306
        end
        object lblServerIPaddr: TLabel
          Left = 18
          Top = 70
          Width = 72
          Height = 12
          Caption = #26381#21153#22120#22320#22336#65306
        end
        object Label23: TLabel
          Left = 6
          Top = 118
          Width = 84
          Height = 12
          Caption = 'DBServer'#31471#21475#65306
        end
        object lblClientPassword: TLabel
          Left = 30
          Top = 163
          Width = 60
          Height = 12
          Caption = #30331#24405#23494#30721#65306
        end
        object edtGateIPaddr: TEdit
          Left = 90
          Top = 18
          Width = 120
          Height = 20
          Hint = 
            #27492#22320#22336#19968#33324#40664#35748#20026' 0.0.0.0 '#65292#36890#24120#19981#38656#35201#26356#25913#12290#13#10#22914#26524#21333#26426#19978#26377#22810#20010'IP'#22320#22336#26102#65292#21487#35774#32622#20026#26412#26426#20854#13#10#20013#19968#20010'IP'#65292#20197#23454#29616#21516#31471#21475#19981 +
            #21516'IP'#30340#32465#23450#12290
          ImeMode = imClose
          ParentShowHint = False
          ShowHint = True
          TabOrder = 0
          Text = '0.0.0.0'
          OnChange = edtGateIPaddrChange
        end
        object edtGatePort: TEdit
          Left = 90
          Top = 42
          Width = 120
          Height = 20
          Hint = #32593#20851#23545#22806#24320#25918#30340#31471#21475#21495#65292#27492#31471#21475#26631#20934#20026' 7200'#65292#13#10#27492#31471#21475#21487#26681#25454#33258#24049#30340#35201#27714#36827#34892#20462#25913#12290
          ImeMode = imClose
          ParentShowHint = False
          ShowHint = True
          TabOrder = 1
          Text = '7200'
          Visible = False
          OnChange = edtGateIPaddrChange
        end
        object edtServerPort: TEdit
          Left = 90
          Top = 90
          Width = 120
          Height = 20
          Hint = #28216#25103#26381#21153#22120#30340#31471#21475#65292#27492#31471#21475#26631#20934#20026' 5000'#65292#13#10#22914#26524#20351#29992#30340#28216#25103#26381#21153#22120#31471#20462#25913#36807#65292#21017#25913#20026#13#10#30456#24212#30340#31471#21475#23601#34892#20102#12290
          ImeMode = imClose
          ParentShowHint = False
          ShowHint = True
          TabOrder = 3
          Text = '5000'
          OnChange = edtGateIPaddrChange
        end
        object edtServerIPaddr: TEdit
          Left = 90
          Top = 66
          Width = 120
          Height = 20
          Hint = #28216#25103#26381#21153#22120#30340'IP'#22320#22336#65292#22914#26524#26159#21333#26426#36816#34892#26381#21153#13#10#22120#31471#26102#65292#19968#33324#23601#29992' 127.0.0.1 '#12290
          ImeMode = imClose
          ParentShowHint = False
          ShowHint = True
          TabOrder = 2
          Text = '127.0.0.1'
          OnChange = edtGateIPaddrChange
        end
        object chkClientPassword: TCheckBox
          Left = 90
          Top = 138
          Width = 120
          Height = 17
          Hint = #22914#38656'IP'#29256#19975#33021#30331#38470#22120#36827#20837#28216#25103#35831#21247#21246#36873#65292#21542#21017'IP'#29256#19975#33021#30331#38470#22120#23558#26080#27861#36827#20837#28216#25103#65281
          Caption = #21551#29992#30331#24405#23494#30721#26816#27979
          ParentShowHint = False
          ShowHint = True
          TabOrder = 4
          OnClick = edtGateIPaddrChange
        end
        object edtClientPassword: TEdit
          Left = 90
          Top = 159
          Width = 120
          Height = 20
          Hint = #38656#35201#21644#30331#24405#22120#37197#32622#22120#30340#30331#24405#23494#30721#35774#32622#30456#21516#65292#21542#21017#30331#24405#20250#40657#23631
          ImeMode = imClose
          MaxLength = 50
          ParentShowHint = False
          ShowHint = True
          TabOrder = 5
          Text = 'GeeM2'
          OnChange = edtGateIPaddrChange
        end
        object edtDBPort: TEdit
          Left = 90
          Top = 114
          Width = 120
          Height = 20
          Hint = #30417#21548'DBServer'#36830#25509#31471#21475#13#10'DBServer'#29992#27492#31471#21475#30340#36830#25509#26469#26816#27979#32593#20851#24212#29992#26159#21542#27491#24120#13#10#40664#35748#20026'27201'#65292#22914#26524#19981#35201#65292#21487#20462#25913#20026'0'
          ImeMode = imClose
          ParentShowHint = False
          ShowHint = True
          TabOrder = 6
          Text = '5000'
          OnChange = edtGateIPaddrChange
        end
      end
      object grpBaseInfo: TGroupBox
        Left = 8
        Top = 195
        Width = 221
        Height = 90
        Caption = #22522#26412#21442#25968
        TabOrder = 1
        object lblTitleName: TLabel
          Left = 8
          Top = 22
          Width = 84
          Height = 12
          Caption = #24212#29992#31243#24207#26631#39064#65306
        end
        object lblShowLogLevel: TLabel
          Left = 8
          Top = 46
          Width = 84
          Height = 12
          Caption = #26174#31034#26085#24535#31561#32423#65306
        end
        object edtTitleName: TEdit
          Left = 90
          Top = 16
          Width = 120
          Height = 20
          Hint = #31243#24207#26631#39064#19978#26174#31034#30340#21517#31216#65292#27492#21517#31216#21482#29992#20110#26174#31034#13#10#26242#26102#19981#20570#20854#23427#29992#36884#12290
          ParentShowHint = False
          ShowHint = True
          TabOrder = 0
          Text = 'GEE'#32593#32476
          OnChange = edtGateIPaddrChange
        end
        object chkMinimize: TCheckBox
          Left = 90
          Top = 66
          Width = 119
          Height = 17
          Caption = #21551#21160#31243#24207#21518#26368#23567#21270
          TabOrder = 2
          OnClick = edtGateIPaddrChange
        end
        object cbbShowLogLevel: TComboBox
          Left = 90
          Top = 42
          Width = 120
          Height = 20
          Style = csDropDownList
          ItemHeight = 12
          TabOrder = 1
          OnChange = edtGateIPaddrChange
          Items.Strings = (
            '0'#32423
            '1'#32423
            '2'#32423
            '3'#32423
            '4'#32423
            '5'#32423
            '6'#32423
            '7'#32423
            '8'#32423
            '9'#32423
            '10'#32423)
        end
      end
      object grpPerformance: TGroupBox
        Left = 239
        Top = 5
        Width = 242
        Height = 156
        Caption = #24615#33021#35774#32622
        TabOrder = 2
        object lblServerCheckTimeOut: TLabel
          Left = 8
          Top = 20
          Width = 96
          Height = 12
          Caption = #26381#21153#22120#26816#27979#36229#26102#65306
        end
        object Label21: TLabel
          Left = 198
          Top = 20
          Width = 12
          Height = 12
          Caption = #31186
        end
        object lblClientSendBlockSize: TLabel
          Left = 8
          Top = 67
          Width = 96
          Height = 12
          Caption = #23458#25143#31471#25968#25454#22823#23567#65306
        end
        object Label22: TLabel
          Left = 198
          Top = 68
          Width = 36
          Height = 12
          Caption = 'KBytes'
        end
        object Label28: TLabel
          Left = 8
          Top = 43
          Width = 96
          Height = 12
          Caption = #23458#25143#31471#25968#25454#22534#31215#65306
        end
        object Label29: TLabel
          Left = 198
          Top = 44
          Width = 36
          Height = 12
          Caption = 'KBytes'
        end
        object Label30: TLabel
          Left = 8
          Top = 91
          Width = 96
          Height = 12
          Caption = #20869#23384#39044#20998#37197#25968#37327#65306
        end
        object Label31: TLabel
          Left = 198
          Top = 92
          Width = 12
          Height = 12
          Caption = #26465
        end
        object Label32: TLabel
          Left = 8
          Top = 115
          Width = 96
          Height = 12
          Caption = #20869#23384#39044#20998#37197#22823#23567#65306
        end
        object lblRecommendPreAllocatedCount: TLabel
          Left = 212
          Top = 92
          Width = 24
          Height = 12
          Cursor = crHandPoint
          Caption = #25512#33616
          Font.Charset = GB2312_CHARSET
          Font.Color = clBlue
          Font.Height = -12
          Font.Name = #23435#20307
          Font.Style = []
          ParentFont = False
          OnClick = lblRecommendPreAllocatedCountClick
          OnMouseEnter = lblRecommendPreAllocatedCountMouseEnter
          OnMouseLeave = lblRecommendPreAllocatedCountMouseLeave
        end
        object lblPreAllocatedSizeHint: TLabel
          Left = 53
          Top = 136
          Width = 182
          Height = 12
          Alignment = taRightJustify
          Caption = #39044#20998#37197#20869#23384#36807#22823#65292#35831#28857#8220#25512#33616#8221
          Font.Charset = GB2312_CHARSET
          Font.Color = clRed
          Font.Height = -12
          Font.Name = #23435#20307
          Font.Style = [fsBold]
          ParentFont = False
        end
        object seCheckServerTimeOutTime: TSpinEdit
          Left = 104
          Top = 16
          Width = 92
          Height = 21
          Hint = #19982#28216#25103#26381#21153#22120#20043#38388#36890#35759#26816#27979#36229#26102#38388#38548
          EditorEnabled = False
          Increment = 30
          MaxValue = 600
          MinValue = 60
          ParentShowHint = False
          ShowHint = True
          TabOrder = 0
          Value = 60
          OnChange = edtGateIPaddrChange
        end
        object seClientSendBlockSize: TSpinEdit
          Left = 104
          Top = 63
          Width = 92
          Height = 21
          Hint = #19968#27425#21457#36865#32473#23458#25143#31471#26368#22823#25968#25454#21253#22823#23567#13#10#13#10#27880#24847#65306#20462#25913#27492#21442#25968#22312#37325#21551#26381#21153#22120#21518#29983#25928
          EditorEnabled = False
          MaxLength = 2
          MaxValue = 8
          MinValue = 1
          ParentShowHint = False
          ShowHint = True
          TabOrder = 1
          Value = 4
          OnChange = seClientSendBlockSizeChange
        end
        object seClientAccumulateMaxSize: TSpinEdit
          Left = 104
          Top = 39
          Width = 92
          Height = 21
          Hint = #20801#35768#32593#20851#36716#21457#21040#23458#25143#31471#25968#25454#22534#31215#22823#23567#65292#24403#25968#25454#22534#31215#36229#36807#27492#22823#23567#21518#20250#26029#24320#36830#25509
          EditorEnabled = False
          MaxLength = 2
          MaxValue = 2048
          MinValue = 2
          ParentShowHint = False
          ShowHint = True
          TabOrder = 2
          Value = 4
          OnChange = edtGateIPaddrChange
        end
        object sePreAllocatedCount: TSpinEdit
          Left = 104
          Top = 87
          Width = 92
          Height = 21
          Hint = #32593#20851#39044#20998#37197#20869#23384#22823#23567#65292#25968#37327#36234#22823#36229#21344#20869#23384#65292#32593#20851#24615#33021#36234#39640#13#10#13#10#27880#24847#65306#20462#25913#27492#21442#25968#22312#37325#21551#26381#21153#22120#21518#29983#25928
          MaxLength = 6
          MaxValue = 102400
          MinValue = 256
          ParentShowHint = False
          ShowHint = True
          TabOrder = 3
          Value = 256
          OnChange = seClientSendBlockSizeChange
        end
        object edtPreAllocatedSize: TEdit
          Left = 104
          Top = 112
          Width = 129
          Height = 20
          Color = clCream
          ReadOnly = True
          TabOrder = 4
        end
      end
      object btnSettingOK: TButton
        Left = 626
        Top = 406
        Width = 75
        Height = 25
        Caption = #30830#23450
        TabOrder = 3
        OnClick = btnSettingOKClick
      end
      object grpAntiPlug: TGroupBox
        Left = 7
        Top = 290
        Width = 222
        Height = 113
        Caption = #21453#22806#25346#25554#20214#35774#32622
        TabOrder = 4
        object lbl4: TLabel
          Left = 27
          Top = 20
          Width = 66
          Height = 12
          Caption = #36229#26102'2'#26102#38388#65306
        end
        object lbl5: TLabel
          Left = 196
          Top = 20
          Width = 12
          Height = 12
          Caption = #31186
        end
        object Label43: TLabel
          Left = 9
          Top = 44
          Width = 84
          Height = 12
          Caption = #25554#20214#21457#36865#38480#36895#65306
        end
        object Label44: TLabel
          Left = 196
          Top = 44
          Width = 18
          Height = 12
          Caption = 'M/S'
        end
        object Label45: TLabel
          Left = 9
          Top = 68
          Width = 84
          Height = 12
          Caption = #21333#27425#21457#36865#22823#23567#65306
        end
        object Label46: TLabel
          Left = 196
          Top = 68
          Width = 12
          Height = 12
          Caption = 'KB'
        end
        object seRecvAntiPlugHeartbeatTimeOutTime: TSpinEdit
          Left = 91
          Top = 16
          Width = 100
          Height = 21
          MaxLength = 3
          MaxValue = 180
          MinValue = 15
          TabOrder = 0
          Value = 15
          OnChange = edtGateIPaddrChange
        end
        object seAntiPlugStreamSendSpeed: TSpinEdit
          Left = 91
          Top = 40
          Width = 100
          Height = 21
          Hint = #21442#25968#37325#21551#21518#29983#25928
          MaxLength = 1
          MaxValue = 20
          MinValue = 1
          ParentShowHint = False
          ShowHint = True
          TabOrder = 1
          Value = 10
          OnChange = edtGateIPaddrChange
        end
        object cbbAntiPlugStreamSendBlockSize: TComboBox
          Left = 91
          Top = 64
          Width = 100
          Height = 20
          Hint = #19968#27425#21521#23458#25143#31471#21457#36865#25968#25454#22359#22823#23567#65292#21442#25968#37325#21551#21518#29983#25928
          Style = csDropDownList
          ItemHeight = 12
          ParentShowHint = False
          ShowHint = True
          TabOrder = 2
          OnChange = edtGateIPaddrChange
          Items.Strings = (
            '128'
            '96'
            '64'
            '32')
        end
        object chkLogoutNoResendAntiplugStream: TCheckBox
          Left = 9
          Top = 89
          Width = 134
          Height = 17
          Hint = #21246#36873#27492#36873#39033#21518#65292#23458#25143#31471#23567#36864#21518#37325#26032#36827#20837#28216#25103#19981#20877#27425#21457#36865#25554#20214#65292#20197#33410#30465#27969#37327
          Caption = #23567#36864#19981#37325#26032#21457#36865#25554#20214
          ParentShowHint = False
          ShowHint = True
          TabOrder = 3
          OnClick = edtGateIPaddrChange
        end
        object chkAntiplugAllLog: TCheckBox
          Left = 146
          Top = 89
          Width = 69
          Height = 17
          Caption = #25152#26377#26085#24535
          TabOrder = 4
          Visible = False
          OnClick = edtGateIPaddrChange
        end
      end
      object grpVerifyCode: TGroupBox
        Left = 492
        Top = 5
        Width = 209
        Height = 396
        Caption = #39564#35777#30721#31995#32479
        TabOrder = 5
        object lbl6: TLabel
          Left = 8
          Top = 43
          Width = 84
          Height = 12
          Caption = #20801#35768#22833#36133#27425#25968#65306
        end
        object Label33: TLabel
          Left = 8
          Top = 91
          Width = 84
          Height = 12
          Caption = #39564#35777#36229#26102#26102#38388#65306
        end
        object Label34: TLabel
          Left = 8
          Top = 67
          Width = 84
          Height = 12
          Caption = #20801#35768#21047#26032#27425#25968#65306
        end
        object Label35: TLabel
          Left = 188
          Top = 43
          Width = 12
          Height = 12
          Caption = #27425
        end
        object Label36: TLabel
          Left = 188
          Top = 67
          Width = 12
          Height = 12
          Caption = #27425
        end
        object Label37: TLabel
          Left = 188
          Top = 91
          Width = 12
          Height = 12
          Caption = #31186
        end
        object Label38: TLabel
          Left = 8
          Top = 115
          Width = 84
          Height = 12
          Caption = #39564#35777#26102#38388#38388#38548#65306
        end
        object Label39: TLabel
          Left = 188
          Top = 115
          Width = 12
          Height = 12
          Caption = #20998
        end
        object Label40: TLabel
          Left = 132
          Top = 371
          Width = 12
          Height = 12
          Caption = #31186
        end
        object lbl8: TLabel
          Left = 8
          Top = 371
          Width = 60
          Height = 12
          Caption = #21152#36733#38388#38548#65306
        end
        object Label41: TLabel
          Left = 8
          Top = 139
          Width = 84
          Height = 12
          Caption = #39564#35777#25104#21151#22686#21152#65306
        end
        object Label42: TLabel
          Left = 188
          Top = 139
          Width = 12
          Height = 12
          Caption = #20998
        end
        object chkVerifyCode: TCheckBox
          Left = 8
          Top = 17
          Width = 137
          Height = 17
          Caption = #24320#21551#39564#35777#30721#31995#32479
          TabOrder = 0
          OnClick = edtGateIPaddrChange
        end
        object seVerifyCodeErrCount: TSpinEdit
          Left = 90
          Top = 38
          Width = 96
          Height = 21
          Hint = #36755#20837#39564#35777#30721#38169#35823#36229#36807#25351#23450#27425#25968#65292#26029#24320
          MaxLength = 2
          MaxValue = 20
          MinValue = 1
          ParentShowHint = False
          ShowHint = True
          TabOrder = 1
          Value = 4
          OnChange = edtGateIPaddrChange
        end
        object seVerifyCodeWaitTime: TSpinEdit
          Left = 90
          Top = 86
          Width = 96
          Height = 21
          Hint = #36229#36807#25351#23450#26102#38388#27809#26377#25104#21151#39564#35777#21017#26029#24320
          MaxValue = 300
          MinValue = 1
          ParentShowHint = False
          ShowHint = True
          TabOrder = 3
          Value = 4
          OnChange = edtGateIPaddrChange
        end
        object seVerifyCodeRefreshCount: TSpinEdit
          Left = 90
          Top = 62
          Width = 96
          Height = 21
          Hint = #20801#35768#21047#26032#39564#35777#30721#26368#22823#27425#25968
          MaxLength = 2
          MaxValue = 20
          MinValue = 1
          ParentShowHint = False
          ShowHint = True
          TabOrder = 2
          Value = 4
          OnChange = edtGateIPaddrChange
        end
        object lstVerifyCodeExcludeMap: TListBox
          Left = 8
          Top = 222
          Width = 137
          Height = 95
          ItemHeight = 12
          TabOrder = 10
          OnClick = lstVerifyCodeExcludeMapClick
        end
        object btnAdd: TButton
          Left = 148
          Top = 222
          Width = 52
          Height = 25
          Caption = #28155#21152
          TabOrder = 11
          OnClick = btnAddClick
        end
        object btnDel: TButton
          Left = 148
          Top = 257
          Width = 52
          Height = 25
          Caption = #21024#38500
          Enabled = False
          TabOrder = 12
          OnClick = btnDelClick
        end
        object btnClear: TButton
          Left = 148
          Top = 292
          Width = 52
          Height = 25
          Caption = #28165#31354
          TabOrder = 13
          OnClick = btnClearClick
        end
        object seVerifyCodeInterval1: TSpinEdit
          Left = 90
          Top = 110
          Width = 47
          Height = 21
          Hint = #38543#26426#39564#35777#26102#38388#38388#38548#65288#26368#23567#20540#65289
          MaxValue = 300
          MinValue = 1
          ParentShowHint = False
          ShowHint = True
          TabOrder = 4
          Value = 5
          OnChange = edtGateIPaddrChange
        end
        object chkAutoLoadNoVerifyChrList: TCheckBox
          Left = 8
          Top = 320
          Width = 169
          Height = 17
          Caption = #33258#21160#21152#36733#20813#39564#35777#30721#35282#33394#21517#21333
          TabOrder = 14
          OnClick = edtGateIPaddrChange
        end
        object edtLoadNoVerifyChrListFile: TEdit
          Left = 8
          Top = 341
          Width = 192
          Height = 20
          Hint = #25351#23450#20813#39564#35777#30721#35282#33394#21517#21333#25991#20214
          ParentShowHint = False
          ShowHint = True
          TabOrder = 15
          OnChange = edtGateIPaddrChange
        end
        object seAutoLoadNoVerifyChrListInterval: TSpinEdit
          Left = 64
          Top = 366
          Width = 65
          Height = 21
          MaxValue = 99999999
          MinValue = 5
          ParentShowHint = False
          ShowHint = True
          TabOrder = 16
          Value = 5
          OnChange = edtGateIPaddrChange
        end
        object btnLoadNoVerifyChrList: TButton
          Left = 148
          Top = 364
          Width = 52
          Height = 25
          Hint = #25163#21160#21152#36733
          Caption = #21152#36733
          ParentShowHint = False
          ShowHint = True
          TabOrder = 17
          OnClick = btnLoadNoVerifyChrListClick
        end
        object seVerifyCodeInterval2: TSpinEdit
          Left = 139
          Top = 110
          Width = 47
          Height = 21
          Hint = #38543#26426#39564#35777#26102#38388#38388#38548#65288#26368#22823#20540#65289
          MaxValue = 300
          MinValue = 1
          ParentShowHint = False
          ShowHint = True
          TabOrder = 5
          Value = 5
          OnChange = edtGateIPaddrChange
        end
        object seVerifySuccessAddInterval: TSpinEdit
          Left = 90
          Top = 134
          Width = 96
          Height = 21
          Hint = #39564#35777#25104#21151#21518#65292#22686#21152#39564#35777#26102#38388#38388#38548#30340#19978#19979#38480#65292#28982#21518#22312#33539#22260#20869#38543#26426#21462#19968#20010#20540#65292#23601#26159#19979#27425#39564#35777#38388#38548
          MaxValue = 300
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 6
          Value = 5
          OnChange = edtGateIPaddrChange
        end
        object chkVerifyCodeExcludeMap: TCheckBox
          Left = 8
          Top = 202
          Width = 137
          Height = 17
          Hint = #21246#36873#21363#20026#25490#38500#39564#35777#30721#30340#22320#22270#65292#19981#21246#36873#21017#34920#31034#38656#35201#39564#35777#30721#30340#22320#22270
          Caption = #25490#38500#39564#35777#30721#22320#22270#20195#30721#65306
          ParentShowHint = False
          ShowHint = True
          TabOrder = 9
          OnClick = edtGateIPaddrChange
        end
        object chkVerifyFailTriggerScript: TCheckBox
          Left = 8
          Top = 159
          Width = 197
          Height = 17
          Hint = #24403#39564#35777#36798#21040#26368#22823#30340#22833#36133#27425#25968#25110#39564#35777#36229#26102#26102#65306#13#10#13#10#21246#36873#21518#35302#21457'QF'#20013#30340#33050#26412#65292#19981#21246#36873#21017#25481#32447#22788#29702
          Caption = #39564#35777#22833#36133#35302#21457#33050#26412#65306'@VerifyFail'
          ParentShowHint = False
          ShowHint = True
          TabOrder = 7
          OnClick = edtGateIPaddrChange
        end
        object chkVerifyFailLoginVerify: TCheckBox
          Left = 8
          Top = 180
          Width = 193
          Height = 17
          Hint = #21246#36873#21518#26410#36755#20837#27491#30830#39564#35777#30721#30340#35282#33394#65292#20877#27425#36827#20837#28216#25103#31435#21363#39564#35777#65292#24182#19988#25915#20987#21644#39764#27861#26080#25928#65292#39564#35777#36890#36807#24674#22797#65281
          Caption = #39564#35777#26410#23436#25104#26102#37325#36827#28216#25103#31435#21363#39564#35777
          ParentShowHint = False
          ShowHint = True
          TabOrder = 8
          OnClick = edtGateIPaddrChange
        end
      end
      object chkOneMACLimitePlayer: TCheckBox
        Left = 8
        Top = 412
        Width = 97
        Height = 17
        Caption = #21333#26426#22120#30721#38480#23450
        TabOrder = 6
        OnClick = edtGateIPaddrChange
      end
      object seOneMACLimitePlayer: TSpinEdit
        Left = 104
        Top = 410
        Width = 51
        Height = 21
        MaxValue = 0
        MinValue = 0
        TabOrder = 7
        Value = 4
        OnChange = seClientSendBlockSizeChange
      end
      object grpClientExitDaly: TGroupBox
        Left = 239
        Top = 168
        Width = 242
        Height = 234
        Caption = #20154#29289#36864#20986#24310#26102#35774#32622
        TabOrder = 8
        object Label50: TLabel
          Left = 7
          Top = 21
          Width = 96
          Height = 12
          Caption = #20154#29289#23567#36864#26102#24310#26102#65306
        end
        object Label53: TLabel
          Left = 219
          Top = 21
          Width = 12
          Height = 12
          Caption = #31186
        end
        object Label54: TLabel
          Left = 7
          Top = 45
          Width = 96
          Height = 12
          Caption = #20154#29289#22823#36864#26102#24310#26102#65306
        end
        object Label55: TLabel
          Left = 219
          Top = 45
          Width = 12
          Height = 12
          Caption = #31186
        end
        object seClientLogoutDelay: TSpinEdit
          Left = 102
          Top = 17
          Width = 114
          Height = 21
          MaxValue = 10
          MinValue = 0
          TabOrder = 0
          Value = 4
          OnChange = edtGateIPaddrChange
        end
        object seClientCloseDelay: TSpinEdit
          Left = 102
          Top = 41
          Width = 114
          Height = 21
          MaxValue = 10
          MinValue = 0
          TabOrder = 1
          Value = 4
          OnChange = edtGateIPaddrChange
        end
        object chkDelayCloseDisableMove: TCheckBox
          Left = 8
          Top = 67
          Width = 95
          Height = 17
          Hint = #20154#29289#31227#21160#21518#65292#20013#26029#28216#25103#36864#20986
          Caption = #31227#21160#20013#26029#36864#20986
          ParentShowHint = False
          ShowHint = True
          TabOrder = 2
          OnClick = edtGateIPaddrChange
        end
        object chkDelayCloseDisableAttack: TCheckBox
          Left = 141
          Top = 67
          Width = 95
          Height = 17
          Hint = #20154#29289#25915#20987#21518#65292#20013#26029#28216#25103#36864#20986
          Caption = #25915#20987#20013#26029#36864#20986
          ParentShowHint = False
          ShowHint = True
          TabOrder = 3
          OnClick = edtGateIPaddrChange
        end
        object chkDelayCloseDisableSpell: TCheckBox
          Left = 8
          Top = 87
          Width = 95
          Height = 17
          Hint = #20154#29289#37322#25918#39764#27861#21518#65292#20013#26029#28216#25103#36864#20986
          Caption = #39764#27861#20013#26029#36864#20986
          ParentShowHint = False
          ShowHint = True
          TabOrder = 4
          OnClick = edtGateIPaddrChange
        end
        object chkDelayCloseDisableUseItem: TCheckBox
          Left = 141
          Top = 87
          Width = 95
          Height = 17
          Hint = #20154#29289#20351#29992#29289#21697#21518#65292#20013#26029#28216#25103#36864#20986
          Caption = #29289#21697#20013#26029#36864#20986
          ParentShowHint = False
          ShowHint = True
          TabOrder = 5
          OnClick = edtGateIPaddrChange
        end
        object chkBreakClientLogoutHint: TCheckBox
          Left = 8
          Top = 115
          Width = 93
          Height = 17
          Caption = #23567#36864#20013#26029#25552#31034
          TabOrder = 6
          OnClick = edtGateIPaddrChange
        end
        object edtBreakClientLogoutHint: TEdit
          Left = 8
          Top = 134
          Width = 220
          Height = 20
          TabOrder = 7
          Text = #23567#36864#28216#25103#25805#20316#24050#34987#20013#26029
          OnChange = edtGateIPaddrChange
        end
        object chkBreakClientCloseHint: TCheckBox
          Left = 8
          Top = 163
          Width = 93
          Height = 17
          Caption = #22823#36864#20013#26029#25552#31034
          TabOrder = 8
          OnClick = edtGateIPaddrChange
        end
        object edtBreakClientCloseHint: TEdit
          Left = 8
          Top = 182
          Width = 220
          Height = 20
          TabOrder = 9
          Text = #22823#36864#28216#25103#25805#20316#24050#34987#20013#26029
          OnChange = edtGateIPaddrChange
        end
      end
    end
    object tsSysInfo: TTabSheet
      Caption = #31995#32479#20449#24687
      ImageIndex = 2
      DesignSize = (
        708
        573)
      object lblInfoBase: TLabel
        Left = 12
        Top = 27
        Width = 48
        Height = 12
        Caption = #22522#26412#20449#24687
        Font.Charset = GB2312_CHARSET
        Font.Color = clBlue
        Font.Height = -12
        Font.Name = #23435#20307
        Font.Style = []
        ParentFont = False
      end
      object bvl1: TBevel
        Left = 12
        Top = 43
        Width = 684
        Height = 2
        Anchors = [akLeft, akTop, akRight]
        Shape = bsTopLine
      end
      object Label1: TLabel
        Left = 17
        Top = 74
        Width = 84
        Height = 12
        Caption = #24037#20316#32447#31243#25968#37327#65306
      end
      object Label6: TLabel
        Left = 209
        Top = 74
        Width = 84
        Height = 12
        Caption = #26381#21153#36816#34892#26102#38388#65306
      end
      object Label11: TLabel
        Left = 12
        Top = 193
        Width = 120
        Height = 12
        Caption = 'IOCP'#25968#25454#21457#36865#25509#25910#32479#35745
        Font.Charset = GB2312_CHARSET
        Font.Color = clBlue
        Font.Height = -12
        Font.Name = #23435#20307
        Font.Style = []
        ParentFont = False
      end
      object Bevel1: TBevel
        Left = 12
        Top = 209
        Width = 684
        Height = 2
        Anchors = [akLeft, akTop, akRight]
        Shape = bsTopLine
      end
      object Label3: TLabel
        Left = 17
        Top = 218
        Width = 84
        Height = 12
        Caption = #32047#35745#21457#36865#27425#25968#65306
      end
      object Label4: TLabel
        Left = 209
        Top = 222
        Width = 84
        Height = 12
        Caption = #32047#35745#21457#36865#22823#23567#65306
      end
      object Label7: TLabel
        Left = 17
        Top = 240
        Width = 84
        Height = 12
        Caption = #32047#35745#25509#25910#27425#25968#65306
      end
      object Label8: TLabel
        Left = 209
        Top = 240
        Width = 84
        Height = 12
        Caption = #32047#35745#25509#25910#22823#23567#65306
      end
      object Label2: TLabel
        Left = 12
        Top = 266
        Width = 72
        Height = 12
        Caption = 'IOData'#27744#20449#24687
        Font.Charset = GB2312_CHARSET
        Font.Color = clBlue
        Font.Height = -12
        Font.Name = #23435#20307
        Font.Style = []
        ParentFont = False
      end
      object Bevel3: TBevel
        Left = 12
        Top = 282
        Width = 684
        Height = 2
        Anchors = [akLeft, akTop, akRight]
        Shape = bsTopLine
      end
      object Label5: TLabel
        Left = 17
        Top = 291
        Width = 84
        Height = 12
        Caption = #24403#21069#20351#29992#25968#37327#65306
      end
      object Label9: TLabel
        Left = 209
        Top = 291
        Width = 84
        Height = 12
        Caption = #31354#38386#20998#37197#25968#37327#65306
      end
      object lblWorkThreadCount: TLabel
        Left = 101
        Top = 74
        Width = 80
        Height = 12
        AutoSize = False
        Caption = '10'
      end
      object lblServerRunTime: TLabel
        Left = 293
        Top = 74
        Width = 160
        Height = 12
        AutoSize = False
      end
      object lblSendCount: TLabel
        Left = 101
        Top = 218
        Width = 80
        Height = 12
        AutoSize = False
      end
      object lblSendBytesSize: TLabel
        Left = 293
        Top = 222
        Width = 390
        Height = 12
        AutoSize = False
      end
      object lblRecvCount: TLabel
        Left = 101
        Top = 240
        Width = 80
        Height = 12
        AutoSize = False
      end
      object lblRecvBytesSize: TLabel
        Left = 293
        Top = 240
        Width = 390
        Height = 12
        AutoSize = False
      end
      object lblIODataUseCount: TLabel
        Left = 101
        Top = 291
        Width = 80
        Height = 12
        AutoSize = False
      end
      object lblIODataNoUseCount: TLabel
        Left = 293
        Top = 291
        Width = 160
        Height = 12
        AutoSize = False
      end
      object Label12: TLabel
        Left = 12
        Top = 120
        Width = 96
        Height = 12
        Caption = #23458#25143#31471#36830#25509#27744#20449#24687
        Font.Charset = GB2312_CHARSET
        Font.Color = clBlue
        Font.Height = -12
        Font.Name = #23435#20307
        Font.Style = []
        ParentFont = False
      end
      object Bevel2: TBevel
        Left = 12
        Top = 136
        Width = 684
        Height = 2
        Anchors = [akLeft, akTop, akRight]
        Shape = bsTopLine
      end
      object Label13: TLabel
        Left = 17
        Top = 145
        Width = 84
        Height = 12
        Caption = #24403#21069#36830#25509#25968#37327#65306
      end
      object Label14: TLabel
        Left = 17
        Top = 167
        Width = 84
        Height = 12
        Caption = #31354#38386#20998#37197#25968#37327#65306
      end
      object lblContextUseCount: TLabel
        Left = 101
        Top = 145
        Width = 80
        Height = 12
        AutoSize = False
      end
      object lblContextNoUseCount: TLabel
        Left = 101
        Top = 167
        Width = 80
        Height = 12
        AutoSize = False
      end
      object Label10: TLabel
        Left = 209
        Top = 149
        Width = 84
        Height = 12
        Caption = #26368#39640#36830#25509#25968#37327#65306
      end
      object lblContextMaxCount: TLabel
        Left = 293
        Top = 149
        Width = 160
        Height = 12
        AutoSize = False
      end
      object Label15: TLabel
        Left = 12
        Top = 6
        Width = 145
        Height = 12
        Caption = 'BmM2'#19987#29992#21453#22806#25346#28216#25103#32593#20851
        Font.Charset = GB2312_CHARSET
        Font.Color = clWindowText
        Font.Height = -12
        Font.Name = #23435#20307
        Font.Style = [fsBold]
        ParentFont = False
      end
      object lblUpdateTime: TLabel
        Left = 17
        Top = 52
        Width = 144
        Height = 12
        Caption = #26368#21518#26356#26032#26085#26399#65306'2025-02-28'
      end
      object Label17: TLabel
        Left = 209
        Top = 52
        Width = 84
        Height = 12
        Caption = #23448#32593#38142#25509#22320#22336#65306
      end
      object Label18: TLabel
        Left = 293
        Top = 52
        Width = 160
        Height = 12
        AutoSize = False
        Caption = 'www.gxxm2.com'
      end
      object Label19: TLabel
        Left = 17
        Top = 335
        Width = 84
        Height = 12
        Caption = #20869#23384#20998#37197#22823#23567#65306
      end
      object lblIODataMemCount: TLabel
        Left = 101
        Top = 335
        Width = 80
        Height = 12
        AutoSize = False
      end
      object Label20: TLabel
        Left = 17
        Top = 96
        Width = 84
        Height = 12
        Caption = #31243#24207#21344#29992#20869#23384#65306
      end
      object lblAppMemorySize: TLabel
        Left = 101
        Top = 96
        Width = 160
        Height = 12
        AutoSize = False
      end
      object Label16: TLabel
        Left = 17
        Top = 313
        Width = 84
        Height = 12
        Caption = #26368#39640#20351#29992#25968#37327#65306
      end
      object lblIODataMaxUseCount: TLabel
        Left = 101
        Top = 313
        Width = 80
        Height = 12
        AutoSize = False
      end
      object Label26: TLabel
        Left = 209
        Top = 313
        Width = 84
        Height = 12
        Caption = #26368#39640#20869#23384#20998#37197#65306
      end
      object lblIODataMaxMemCount: TLabel
        Left = 293
        Top = 313
        Width = 160
        Height = 12
        AutoSize = False
      end
    end
    object tsIOCP: TTabSheet
      Caption = 'IOCP'#35843#35797#20449#24687
      ImageIndex = 4
      object mmoIOCPLog: TMemo
        Left = 0
        Top = 0
        Width = 708
        Height = 573
        Align = alClient
        Color = clBlack
        Font.Charset = GB2312_CHARSET
        Font.Color = clLime
        Font.Height = -12
        Font.Name = #23435#20307
        Font.Style = []
        ParentFont = False
        ScrollBars = ssBoth
        TabOrder = 0
        OnDblClick = mmoIOCPLogDblClick
      end
    end
  end
  object mmMain: TMainMenu
    Left = 456
    Top = 8
    object mniControl: TMenuItem
      Caption = #25511#21046
      object mmiStartServer: TMenuItem
        Caption = #21551#21160#26381#21153
        OnClick = mmiStartServerClick
      end
      object mmiStopServer: TMenuItem
        Caption = #20572#27490#26381#21153
        OnClick = mmiStopServerClick
      end
      object mniN00: TMenuItem
        Caption = '-'
      end
      object mniDBServerEnabledIP: TMenuItem
        Caption = #21152#36733'DBServer'#25480#26435#36830#25509'IP'#25991#20214
        OnClick = mniDBServerEnabledIPClick
      end
    end
    object mniSetting: TMenuItem
      Caption = #35774#32622
      object mmiWordFilter: TMenuItem
        Caption = #28040#24687#36807#28388
        OnClick = mmiWordFilterClick
      end
      object mmiSafeSetting: TMenuItem
        Caption = #23433#20840#35774#32622
        OnClick = mmiSafeSettingClick
      end
      object mmiWaiGua: TMenuItem
        Caption = #22806#25346#25511#21046
        OnClick = mmiWaiGuaClick
      end
      object mniReadFileIP: TMenuItem
        Caption = #38450#24481#35774#32622
        OnClick = mniReadFileIPClick
      end
      object mniN10: TMenuItem
        Caption = '-'
      end
      object mniMagicCD: TMenuItem
        Caption = #25216#33021'CD'#35774#32622
        OnClick = mniMagicCDClick
      end
      object mniEatItemCD: TMenuItem
        Caption = #21507#33647'CD'#35774#32622
        OnClick = mniEatItemCDClick
      end
      object mniN11: TMenuItem
        Caption = '-'
      end
      object mniProcessBlacklist: TMenuItem
        Caption = #36827#31243#40657#21517#21333
        OnClick = mniProcessBlacklistClick
      end
      object mniN12: TMenuItem
        Caption = '-'
      end
      object mniLogClientPacket: TMenuItem
        Caption = #23553#21253#35760#24405#35774#32622
        OnClick = mniLogClientPacketClick
      end
    end
    object mniHelp: TMenuItem
      Caption = #24110#21161
      object mniAbout: TMenuItem
        Caption = #20851#20110
        OnClick = mniAboutClick
      end
    end
  end
  object tmrStart: TTimer
    Interval = 500
    OnTimer = tmrStartTimer
    Left = 12
    Top = 240
  end
  object tmrRefreshInfo: TTimer
    Enabled = False
    OnTimer = tmrRefreshInfoTimer
    Left = 84
    Top = 247
  end
  object tmrRefreshLog: TTimer
    Interval = 500
    OnTimer = tmrRefreshLogTimer
    Left = 12
    Top = 296
  end
  object pmUser: TPopupMenu
    OnPopup = pmUserPopup
    Left = 52
    Top = 127
    object mniKick: TMenuItem
      Caption = #36386#19979#32447
      OnClick = mniKickClick
    end
    object mniN3: TMenuItem
      Caption = '-'
    end
    object mniAddToTempBlock: TMenuItem
      Caption = #21152#20837#21160#24577#36807#34385#21015#34920
      OnClick = mniAddToTempBlockClick
    end
    object mniAddToBlock: TMenuItem
      Caption = #21152#20837#27704#20037#36807#28388#21015#34920
      OnClick = mniAddToBlockClick
    end
    object N3: TMenuItem
      Caption = '-'
    end
    object mniAddToTempMacBlock: TMenuItem
      Caption = #21152#20837#21160#24577#26426#22120#30721#36807#28388#21015#34920
      OnClick = mniAddToTempMacBlockClick
    end
    object mniAddToMacBlock: TMenuItem
      Caption = #21152#20837#27704#20037#26426#22120#30721#36807#28388#21015#34920
      OnClick = mniAddToMacBlockClick
    end
    object N4: TMenuItem
      Caption = '-'
    end
    object mniCopyUserName: TMenuItem
      Caption = #22797#21046#35282#33394#21517#31216
      OnClick = mniCopyUserNameClick
    end
    object mniCopyMac: TMenuItem
      Caption = #22797#21046#26426#22120#30721
      OnClick = mniCopyMacClick
    end
    object mniCopyIP: TMenuItem
      Caption = #22797#21046'IP'
      OnClick = mniCopyIPClick
    end
  end
  object ServerSocketDB: TServerSocket
    Active = False
    Port = 0
    ServerType = stNonBlocking
    OnClientConnect = ServerSocketDBClientConnect
    OnClientRead = ServerSocketDBClientRead
    OnClientError = ServerSocketDBClientError
    Left = 234
    Top = 64
  end
  object pmProcessList: TPopupMenu
    OnPopup = pmProcessListPopup
    Left = 460
    Top = 330
    object mniAddBlackProcess: TMenuItem
      Caption = #21152#20837#36827#31243#40657#21517#21333
      OnClick = mniAddBlackProcessClick
    end
    object mniSetRoot: TMenuItem
      Caption = #35774#32622#20026#26681#36335#24452
      OnClick = mniSetRootClick
    end
    object mniSendFileToRungate: TMenuItem
      Caption = #20256#36865#25991#20214#21040#32593#20851
      OnClick = mniSendFileToRungateClick
    end
  end
end
