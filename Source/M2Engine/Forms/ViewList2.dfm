object FrmViewList2: TFrmViewList2
  Left = 424
  Top = 191
  ActiveControl = ListBoxitemList
  BorderStyle = bsDialog
  Caption = #26597#30475#21015#34920#20449#24687
  ClientHeight = 472
  ClientWidth = 788
  Color = clBtnFace
  Font.Charset = GB2312_CHARSET
  Font.Color = clWindowText
  Font.Height = -12
  Font.Name = #23435#20307
  Font.Style = []
  Position = poMainFormCenter
  OnCreate = FormCreate
  TextHeight = 12
  object PageControl: TPageControl
    Left = 0
    Top = 0
    Width = 788
    Height = 472
    ActivePage = TabSheet1
    Align = alClient
    TabOrder = 0
    ExplicitWidth = 784
    ExplicitHeight = 471
    object TabSheet1: TTabSheet
      Caption = #21830#38138
      object Label1: TLabel
        Left = 160
        Top = 300
        Width = 54
        Height = 12
        Caption = #29289#21697#31867#21035':'
      end
      object Label2: TLabel
        Left = 160
        Top = 348
        Width = 54
        Height = 12
        Caption = #29289#21697#20215#26684':'
      end
      object Label3: TLabel
        Left = 160
        Top = 324
        Width = 54
        Height = 12
        Caption = #29289#21697#21517#31216':'
      end
      object Label4: TLabel
        Left = 160
        Top = 372
        Width = 66
        Height = 12
        Caption = 'Effect'#24207#21495':'
      end
      object Label5: TLabel
        Left = 160
        Top = 396
        Width = 54
        Height = 12
        Caption = #22270#29255#25968#37327':'
      end
      object Label6: TLabel
        Left = 160
        Top = 420
        Width = 54
        Height = 12
        Caption = #29289#21697#21151#33021':'
      end
      object Label7: TLabel
        Left = 373
        Top = 324
        Width = 120
        Height = 12
        Caption = #29289#21697#25551#36848' ('#22238#36710#25442#34892'):'
      end
      object Label55: TLabel
        Left = 373
        Top = 300
        Width = 54
        Height = 12
        Caption = #20132#26131#36135#24065':'
      end
      object lbl28: TLabel
        Left = 520
        Top = 300
        Width = 30
        Height = 12
        Caption = #25968#37327':'
      end
      object Label95: TLabel
        Left = 518
        Top = 324
        Width = 108
        Height = 12
        Caption = #25209#37327#36141#20080#25968#37327#38480#21046#65306
      end
      object GroupBox11: TGroupBox
        Left = 8
        Top = 4
        Width = 145
        Height = 430
        Caption = #29289#21697#21015#34920
        TabOrder = 0
        object ListBoxitemList: TListBox
          Left = 8
          Top = 16
          Width = 129
          Height = 401
          Hint = 'Ctrl + F '#29289#21697#26597#25214#65292#21491#38190#21487#20197#36827#34892#31579#36873#12290
          ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
          ItemHeight = 12
          MultiSelect = True
          ParentShowHint = False
          PopupMenu = PopupMenu1
          ShowHint = True
          TabOrder = 0
          OnClick = ListBoxitemListClick
          OnKeyDown = ListBoxitemListKeyDown
        end
      end
      object ComboBoxShopType: TComboBox
        Left = 232
        Top = 296
        Width = 121
        Height = 20
        Style = csDropDownList
        Enabled = False
        ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
        TabOrder = 3
        Items.Strings = (
          #35013#39280
          #34917#32473
          #24378#21270
          #22909#21451
          #38480#37327
          #22855#29645)
      end
      object EditShopItemName: TEdit
        Left = 232
        Top = 320
        Width = 121
        Height = 20
        ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
        ReadOnly = True
        TabOrder = 7
      end
      object EditShopItemPrice: TSpinEditEx
        Left = 232
        Top = 344
        Width = 121
        Height = 21
        MaxValue = 0
        MinValue = 0
        TabOrder = 8
        Value = 1
      end
      object EditImageIndex: TSpinEditEx
        Left = 232
        Top = 368
        Width = 121
        Height = 21
        MaxValue = 0
        MinValue = 0
        TabOrder = 11
        Value = 380
      end
      object EditImageCount: TSpinEditEx
        Left = 232
        Top = 392
        Width = 121
        Height = 21
        MaxValue = 100
        MinValue = 1
        TabOrder = 13
        Value = 1
      end
      object EditItemMemo1: TEdit
        Left = 232
        Top = 416
        Width = 121
        Height = 20
        ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
        TabOrder = 15
      end
      object MemoShop: TMemo
        Left = 373
        Top = 344
        Width = 226
        Height = 89
        ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
        TabOrder = 10
      end
      object ButtonAddShopItem: TButton
        Left = 616
        Top = 344
        Width = 65
        Height = 25
        Caption = #22686#21152'(&A)'
        TabOrder = 9
        OnClick = ButtonAddShopItemClick
      end
      object ButtonDelShopItem: TButton
        Left = 616
        Top = 368
        Width = 65
        Height = 25
        Caption = #21024#38500'(&D)'
        TabOrder = 12
        OnClick = ButtonDelShopItemClick
      end
      object ButtonShopRefresh: TButton
        Left = 688
        Top = 416
        Width = 97
        Height = 25
        Caption = #37325#26032#21152#36733'(&R)'
        TabOrder = 17
        OnClick = ButtonShopRefreshClick
      end
      object ButtonShopChgItem: TButton
        Left = 616
        Top = 392
        Width = 65
        Height = 25
        Caption = #20462#25913'(&E)'
        TabOrder = 14
        OnClick = ButtonShopChgItemClick
      end
      object ComboBoxGameMoney: TComboBox
        Left = 429
        Top = 296
        Width = 84
        Height = 20
        Style = csDropDownList
        ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
        TabOrder = 4
      end
      object PageControlShop: TPageControl
        Left = 160
        Top = 8
        Width = 627
        Height = 273
        ActivePage = TabSheetShop6
        TabOrder = 1
        OnChange = PageControlShopChange
        object TabSheetShop1: TTabSheet
          Caption = #35013#39280
          object ListViewShop1: TListView
            Left = 0
            Top = 0
            Width = 619
            Height = 245
            Align = alClient
            Columns = <
              item
                Caption = #31867#21035
              end
              item
                Caption = #29289#21697#21517#31216
                Width = 80
              end
              item
                Caption = #20132#26131#25968#37327
                Width = 60
              end
              item
                Caption = #20132#26131#36135#24065
                Width = 60
              end
              item
                Caption = #29289#21697#20215#26684
                Width = 60
              end
              item
                Caption = 'Effect'#24207#21495
                Width = 75
              end
              item
                Caption = #22270#29255#25968#37327
                Width = 60
              end
              item
                Caption = #29289#21697#21151#33021
                Width = 60
              end
              item
                Caption = #29289#21697#25551#36848
                Width = 130
              end>
            GridLines = True
            ReadOnly = True
            RowSelect = True
            TabOrder = 0
            ViewStyle = vsReport
            OnClick = ListViewShop1Click
          end
        end
        object TabSheetShop2: TTabSheet
          Caption = #34917#32473
          ImageIndex = 1
          object ListViewShop2: TListView
            Left = 0
            Top = 0
            Width = 619
            Height = 245
            Align = alClient
            Columns = <
              item
                Caption = #31867#21035
              end
              item
                Caption = #29289#21697#21517#31216
                Width = 80
              end
              item
                Caption = #20132#26131#25968#37327
                Width = 60
              end
              item
                Caption = #20132#26131#36135#24065
                Width = 60
              end
              item
                Caption = #29289#21697#20215#26684
                Width = 60
              end
              item
                Caption = 'Effect'#24207#21495
                Width = 75
              end
              item
                Caption = #22270#29255#25968#37327
                Width = 60
              end
              item
                Caption = #29289#21697#21151#33021
                Width = 60
              end
              item
                Caption = #29289#21697#25551#36848
                Width = 130
              end>
            GridLines = True
            ReadOnly = True
            RowSelect = True
            TabOrder = 0
            ViewStyle = vsReport
            OnClick = ListViewShop1Click
          end
        end
        object TabSheetShop3: TTabSheet
          Caption = #24378#21270
          ImageIndex = 2
          object ListViewShop3: TListView
            Left = 0
            Top = 0
            Width = 619
            Height = 245
            Align = alClient
            Columns = <
              item
                Caption = #31867#21035
              end
              item
                Caption = #29289#21697#21517#31216
                Width = 80
              end
              item
                Caption = #20132#26131#25968#37327
                Width = 60
              end
              item
                Caption = #20132#26131#36135#24065
                Width = 60
              end
              item
                Caption = #29289#21697#20215#26684
                Width = 60
              end
              item
                Caption = 'Effect'#24207#21495
                Width = 75
              end
              item
                Caption = #22270#29255#25968#37327
                Width = 60
              end
              item
                Caption = #29289#21697#21151#33021
                Width = 60
              end
              item
                Caption = #29289#21697#25551#36848
                Width = 130
              end>
            GridLines = True
            ReadOnly = True
            RowSelect = True
            TabOrder = 0
            ViewStyle = vsReport
            OnClick = ListViewShop1Click
          end
        end
        object TabSheetShop4: TTabSheet
          Caption = #22909#21451
          ImageIndex = 3
          object ListViewShop4: TListView
            Left = 0
            Top = 0
            Width = 619
            Height = 245
            Align = alClient
            Columns = <
              item
                Caption = #31867#21035
              end
              item
                Caption = #29289#21697#21517#31216
                Width = 80
              end
              item
                Caption = #20132#26131#25968#37327
                Width = 60
              end
              item
                Caption = #20132#26131#36135#24065
                Width = 60
              end
              item
                Caption = #29289#21697#20215#26684
                Width = 60
              end
              item
                Caption = 'Effect'#24207#21495
                Width = 75
              end
              item
                Caption = #22270#29255#25968#37327
                Width = 60
              end
              item
                Caption = #29289#21697#21151#33021
                Width = 60
              end
              item
                Caption = #29289#21697#25551#36848
                Width = 130
              end>
            GridLines = True
            ReadOnly = True
            RowSelect = True
            TabOrder = 0
            ViewStyle = vsReport
            OnClick = ListViewShop1Click
          end
        end
        object TabSheetShop5: TTabSheet
          Caption = #38480#37327
          ImageIndex = 4
          object ListViewShop5: TListView
            Left = 0
            Top = 0
            Width = 619
            Height = 245
            Align = alClient
            Columns = <
              item
                Caption = #31867#21035
              end
              item
                Caption = #29289#21697#21517#31216
                Width = 80
              end
              item
                Caption = #20132#26131#25968#37327
                Width = 60
              end
              item
                Caption = #20132#26131#36135#24065
                Width = 60
              end
              item
                Caption = #29289#21697#20215#26684
                Width = 60
              end
              item
                Caption = 'Effect'#24207#21495
                Width = 75
              end
              item
                Caption = #22270#29255#25968#37327
                Width = 60
              end
              item
                Caption = #29289#21697#21151#33021
                Width = 60
              end
              item
                Caption = #29289#21697#25551#36848
                Width = 130
              end>
            GridLines = True
            ReadOnly = True
            RowSelect = True
            TabOrder = 0
            ViewStyle = vsReport
            OnClick = ListViewShop1Click
          end
        end
        object TabSheetShop6: TTabSheet
          Caption = #22855#29645
          ImageIndex = 5
          object ListViewShop6: TListView
            Left = 0
            Top = 0
            Width = 619
            Height = 245
            Align = alClient
            Columns = <
              item
                Caption = #31867#21035
              end
              item
                Caption = #29289#21697#21517#31216
                Width = 80
              end
              item
                Caption = #20132#26131#25968#37327
                Width = 60
              end
              item
                Caption = #20132#26131#36135#24065
                Width = 60
              end
              item
                Caption = #29289#21697#20215#26684
                Width = 60
              end
              item
                Caption = 'Effect'#24207#21495
                Width = 75
              end
              item
                Caption = #22270#29255#25968#37327
                Width = 60
              end
              item
                Caption = #29289#21697#21151#33021
                Width = 60
              end
              item
                Caption = #29289#21697#25551#36848
                Width = 130
              end>
            GridLines = True
            ReadOnly = True
            RowSelect = True
            TabOrder = 0
            ViewStyle = vsReport
            OnClick = ListViewShop1Click
          end
        end
      end
      object ButtonShopItemUP: TButton
        Left = 752
        Top = 288
        Width = 33
        Height = 25
        Caption = #8593
        Enabled = False
        TabOrder = 2
        OnClick = ButtonShopItemUPClick
      end
      object ButtonShopItemDOWN: TButton
        Left = 752
        Top = 312
        Width = 33
        Height = 25
        Caption = #8595
        Enabled = False
        TabOrder = 6
        OnClick = ButtonShopItemDOWNClick
      end
      object ButtonShopSaveItem: TButton
        Left = 616
        Top = 416
        Width = 65
        Height = 25
        Caption = #20445#23384'(&S)'
        TabOrder = 16
        OnClick = ButtonShopSaveItemClick
      end
      object seItemCount: TSpinEditEx
        Left = 552
        Top = 296
        Width = 49
        Height = 21
        MaxValue = 9999999
        MinValue = 1
        TabOrder = 5
        Value = 1
      end
      object chkEnabledBuyShopItemGive: TCheckBox
        Left = 688
        Top = 384
        Width = 97
        Height = 17
        Caption = #24320#21551#21830#38138#36192#36865
        Font.Charset = GB2312_CHARSET
        Font.Color = clBlue
        Font.Height = -12
        Font.Name = #23435#20307
        Font.Style = []
        ParentFont = False
        TabOrder = 18
        OnClick = chkEnabledBuyShopItemGiveClick
      end
      object chkBulkBuy: TCheckBox
        Left = 624
        Top = 297
        Width = 73
        Height = 17
        Caption = #25209#37327#36141#20080
        TabOrder = 19
      end
      object seBulkBuyCount: TSpinEditEx
        Left = 624
        Top = 319
        Width = 67
        Height = 21
        Enabled = False
        MaxValue = 99999
        MinValue = 1
        TabOrder = 20
        Value = 1
      end
    end
    object TabSheet2: TTabSheet
      Caption = #23453#31665
      ImageIndex = 1
      object Label10: TLabel
        Left = 424
        Top = 424
        Width = 264
        Height = 12
        Caption = #27599#20010#31665#23376#20013#30340#29289#21697#19981#33021#23569#20110'9'#20010','#21542#21017#31665#23376#26080#27861#25171#24320
        Font.Charset = GB2312_CHARSET
        Font.Color = clRed
        Font.Height = -12
        Font.Name = #23435#20307
        Font.Style = []
        ParentFont = False
      end
      object GroupBox1: TGroupBox
        Left = 8
        Top = 4
        Width = 145
        Height = 430
        Caption = #29289#21697#21015#34920
        TabOrder = 0
        object ListBoxitemList1: TListBox
          Left = 8
          Top = 16
          Width = 128
          Height = 401
          Hint = 'Ctrl + F '#29289#21697#26597#25214#65292#21491#38190#21487#20197#36827#34892#31579#36873#12290
          ImeMode = imChinese
          ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
          ItemHeight = 12
          ParentShowHint = False
          PopupMenu = PopupMenu2
          ShowHint = True
          TabOrder = 0
          OnClick = ListBoxitemList1Click
          OnKeyDown = ListBoxitemListKeyDown
        end
      end
      object GroupBox2: TGroupBox
        Left = 160
        Top = 4
        Width = 113
        Height = 430
        Caption = #23453#31665#21015#34920
        TabOrder = 1
        object ListBoxBoxItem: TListBox
          Left = 8
          Top = 16
          Width = 97
          Height = 401
          ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
          ItemHeight = 12
          TabOrder = 0
          OnClick = ListBoxBoxItemClick
        end
      end
      object GroupBoxBoxItem: TGroupBox
        Left = 280
        Top = 108
        Width = 504
        Height = 301
        Caption = #23453#31665#29289#21697
        TabOrder = 3
        object Label57: TLabel
          Left = 40
          Top = 16
          Width = 48
          Height = 12
          Caption = #21487#24471#29289#21697
        end
        object Label58: TLabel
          Left = 288
          Top = 16
          Width = 48
          Height = 12
          Caption = #20013#38388#19968#26684
        end
        object Label59: TLabel
          Left = 172
          Top = 16
          Width = 36
          Height = 12
          Caption = #19981#21487#24471
        end
        object Label8: TLabel
          Left = 16
          Top = 251
          Width = 60
          Height = 12
          Caption = #29289#21697#31181#31867#65306
        end
        object Label60: TLabel
          Left = 152
          Top = 251
          Width = 60
          Height = 12
          Caption = #29289#21697#21517#31216#65306
        end
        object lbl1: TLabel
          Left = 152
          Top = 279
          Width = 60
          Height = 12
          Caption = #29289#21697#25968#37327#65306
        end
        object lbl13: TLabel
          Left = 16
          Top = 278
          Width = 60
          Height = 12
          Caption = #29305#27530#29289#21697#65306
        end
        object lbl15: TLabel
          Left = 412
          Top = 16
          Width = 48
          Height = 12
          Caption = #27704#19981#21487#24471
        end
        object ListBoxGiveItem: TListBox
          Left = 8
          Top = 32
          Width = 116
          Height = 203
          ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ItemHeight = 12
          TabOrder = 0
          OnClick = ListBoxGiveItemClick
        end
        object ListBoxCenterItem: TListBox
          Left = 256
          Top = 32
          Width = 116
          Height = 203
          ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ItemHeight = 12
          TabOrder = 2
          OnClick = ListBoxCenterItemClick
        end
        object ListBoxNoGiveItem: TListBox
          Left = 132
          Top = 32
          Width = 116
          Height = 203
          ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ItemHeight = 12
          TabOrder = 1
          OnClick = ListBoxNoGiveItemClick
        end
        object ComboBoxBoxItemType: TComboBox
          Left = 72
          Top = 248
          Width = 73
          Height = 20
          Style = csDropDownList
          ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          TabOrder = 5
          Items.Strings = (
            #21487#24471#29289#21697
            #19981#21487#24471
            #20013#38388#19968#26684
            #27704#19981#21487#24471)
        end
        object edtBoxItemName: TEdit
          Left = 208
          Top = 248
          Width = 177
          Height = 20
          ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ReadOnly = True
          TabOrder = 6
        end
        object cbbOtherItem: TComboBox
          Left = 72
          Top = 275
          Width = 73
          Height = 20
          Style = csDropDownList
          ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          TabOrder = 9
          OnChange = cbbOtherItemChange
          Items.Strings = (
            #22768#26395
            #37329#21018#30707
            #32463#39564)
        end
        object ListBoxEndNoGiveItem: TListBox
          Left = 380
          Top = 32
          Width = 116
          Height = 203
          ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ItemHeight = 12
          TabOrder = 3
          OnClick = ListBoxEndNoGiveItemClick
        end
        object btnAddBoxItem: TButton
          Left = 429
          Top = 238
          Width = 68
          Height = 25
          Caption = #22686#21152
          TabOrder = 4
          OnClick = btnAddBoxItemClick
        end
        object btnDelBoxItem: TButton
          Left = 429
          Top = 270
          Width = 68
          Height = 25
          Caption = #21024#38500
          TabOrder = 7
          OnClick = btnDelBoxItemClick
        end
        object seBoxItemCount: TSpinEditEx
          Left = 208
          Top = 274
          Width = 81
          Height = 21
          Hint = #26222#36890#29289#21697#25351#23450#25968#37327#65292#29305#27530#29289#21697#37329#21018#30707#12289#32463#39564#20540#12289#22768#26395#20540#25968#37327' '
          MaxValue = 2147483647
          MinValue = 1
          ParentShowHint = False
          ShowHint = True
          TabOrder = 8
          Value = 1
          OnChange = seBoxItemCountChange
        end
      end
      object btnSaveBoxItem: TButton
        Left = 705
        Top = 420
        Width = 75
        Height = 25
        Caption = #20445#23384
        TabOrder = 4
        OnClick = btnSaveBoxItemClick
      end
      object grp1: TGroupBox
        Left = 280
        Top = 8
        Width = 504
        Height = 97
        Caption = #23453#31665#37197#32622
        TabOrder = 2
        object lbl16: TLabel
          Left = 72
          Top = 48
          Width = 60
          Height = 12
          Caption = #20108#27425#25910#36153#65306
        end
        object lbl17: TLabel
          Left = 72
          Top = 72
          Width = 60
          Height = 12
          Caption = #20108#27425#25910#36153#65306
        end
        object lbl18: TLabel
          Left = 213
          Top = 48
          Width = 72
          Height = 12
          Caption = #19977#27425#21450#20197#21518#65306
        end
        object lbl21: TLabel
          Left = 213
          Top = 72
          Width = 72
          Height = 12
          Caption = #19977#27425#21450#20197#21518#65306
        end
        object lbl22: TLabel
          Left = 368
          Top = 48
          Width = 60
          Height = 12
          Caption = #32047#35745#28040#36153#65306
        end
        object lbl23: TLabel
          Left = 368
          Top = 72
          Width = 60
          Height = 12
          Caption = #32047#35745#28040#36153#65306
        end
        object lbl24: TLabel
          Left = 136
          Top = 18
          Width = 12
          Height = 12
          Caption = #27425
        end
        object lbl32: TLabel
          Left = 8
          Top = 48
          Width = 65
          Height = 12
          Caption = #37329#24065#25910#36153#65306
          Font.Charset = GB2312_CHARSET
          Font.Color = clWindowText
          Font.Height = -12
          Font.Name = #23435#20307
          Font.Style = [fsBold]
          ParentFont = False
        end
        object Label84: TLabel
          Left = 8
          Top = 72
          Width = 65
          Height = 12
          Caption = #20803#23453#25910#36153#65306
          Font.Charset = GB2312_CHARSET
          Font.Color = clWindowText
          Font.Height = -12
          Font.Name = #23435#20307
          Font.Style = [fsBold]
          ParentFont = False
        end
        object chkNext: TCheckBox
          Left = 8
          Top = 16
          Width = 73
          Height = 17
          Caption = #20801#35768#26059#36716
          TabOrder = 1
          OnClick = chkNextClick
        end
        object seCount: TSpinEditEx
          Left = 80
          Top = 14
          Width = 49
          Height = 21
          Hint = #31532#19968#27425#20813#36153#26059#36716#21518#65292#36716#30424#21487#32487#32493#25910#36153#26059#36716#22810#23569#27425
          Enabled = False
          MaxValue = 255
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 0
          Value = 3
          OnChange = seCountChange
        end
        object seGold: TSpinEditEx
          Left = 128
          Top = 44
          Width = 75
          Height = 21
          Hint = #40664#35748#20540#65306'10'#13#10#31532#20108#27425#24320#22987#25910#36153#37329#24065#25968#37327#65292#35774#32622'0'#20026#19981#25910#37329#24065
          MaxValue = 2147483647
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 2
          Value = 3
          OnChange = seGoldChange
        end
        object seGameGold: TSpinEditEx
          Left = 128
          Top = 68
          Width = 75
          Height = 21
          Hint = #40664#35748#20540#65306'10'#13#10#31532#20108#27425#24320#22987#25910#36153#20803#23453#25968#37327#65292#35774#32622'0'#20026#19981#25910#20803#23453
          MaxValue = 2147483647
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 5
          Value = 3
          OnChange = seGameGoldChange
        end
        object seAddGold: TSpinEditEx
          Left = 280
          Top = 44
          Width = 75
          Height = 21
          Hint = 
            #27492#21442#25968#40664#35748#20540#65306'20'#13#10#31532#19977#27425#21644#20197#21518#25910#36153#20026'['#36215#22987#37329#24065'+'#32047#22686#20540'*('#27425#25968'-2) '#20063#23601#26159'10+20*(3-2)=30  '#31532#22235#27425#21017#20026' 10' +
            '+20*(4-2)=50'#65292#20197#27492#31867#25512'] '
          MaxValue = 2147483647
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 3
          Value = 3
          OnChange = seAddGoldChange
        end
        object seAddGameGold: TSpinEditEx
          Left = 280
          Top = 68
          Width = 75
          Height = 21
          Hint = 
            #27492#21442#25968#40664#35748#20540#65306'20'#13#10#31532#19977#27425#21644#20197#21518#25910#36153#20026'['#36215#22987#20803#23453'+'#32047#22686#20540'*('#27425#25968'-2) '#20063#23601#26159'10+20*(3-2)=30  '#31532#22235#27425#21017#20026' 10' +
            '+20*(4-2)=50'#65292#20197#27492#31867#25512'] '
          MaxValue = 2147483647
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 6
          Value = 3
          OnChange = seAddGameGoldChange
        end
        object seEndGold: TSpinEditEx
          Left = 422
          Top = 44
          Width = 75
          Height = 21
          Hint = #40664#35748#20540#65306'50'#13#10#24403#20154#29289#28040#36153#24635#20540#22823#20110#25110#31561#20110#35813#20540#65292#21017#29609#23478#26377#26426#20250#21487#20197#24471#21040#29289#21697#35774#32622#20026#19981#21487#24471#30340#29289#21697
          MaxValue = 2147483647
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 4
          Value = 3
          OnChange = seEndGoldChange
        end
        object seEndGameGold: TSpinEditEx
          Left = 422
          Top = 68
          Width = 75
          Height = 21
          Hint = #40664#35748#20540#65306'50'#13#10#24403#20154#29289#28040#36153#24635#20540#22823#20110#25110#31561#20110#35813#20540#65292#21017#29609#23478#26377#26426#20250#21487#20197#24471#21040#29289#21697#35774#32622#20026#19981#21487#24471#30340#29289#21697
          MaxValue = 2147483647
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 7
          Value = 3
          OnChange = seEndGameGoldChange
        end
      end
    end
    object TabSheet3: TTabSheet
      Caption = #25991#23383#36807#28388
      ImageIndex = 2
      object GroupBox22: TGroupBox
        Left = 8
        Top = 4
        Width = 777
        Height = 313
        Caption = #28040#24687#36807#28388#21015#34920
        TabOrder = 0
        object ListViewMsgFilter: TListView
          Left = 8
          Top = 16
          Width = 761
          Height = 289
          Columns = <
            item
              Caption = #36807#28388#28040#24687
              Width = 300
            end
            item
              Caption = #26367#25442#28040#24687
              Width = 300
            end>
          GridLines = True
          ReadOnly = True
          RowSelect = True
          TabOrder = 0
          ViewStyle = vsReport
          OnClick = ListViewMsgFilterClick
        end
      end
      object GroupBox23: TGroupBox
        Left = 8
        Top = 328
        Width = 777
        Height = 73
        Caption = #28040#24687#36807#28388#21015#34920#32534#36753
        TabOrder = 1
        object Label22: TLabel
          Left = 8
          Top = 24
          Width = 60
          Height = 12
          Caption = #36807#28388#28040#24687#65306
        end
        object Label23: TLabel
          Left = 8
          Top = 48
          Width = 60
          Height = 12
          Caption = #26367#25442#28040#24687#65306
        end
        object EditFilterMsg: TEdit
          Left = 72
          Top = 20
          Width = 698
          Height = 20
          ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
          MaxLength = 255
          TabOrder = 0
        end
        object EditNewMsg: TEdit
          Left = 72
          Top = 44
          Width = 698
          Height = 20
          Hint = #26367#25442#28040#24687#20026#31354#26102#65292#20002#25481#25972#21477#12290
          ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
          MaxLength = 255
          ParentShowHint = False
          ShowHint = True
          TabOrder = 1
        end
      end
      object ButtonMsgFilterAdd: TButton
        Left = 376
        Top = 408
        Width = 97
        Height = 25
        Caption = #22686#21152'(&A)'
        TabOrder = 2
        OnClick = ButtonMsgFilterAddClick
      end
      object ButtonMsgFilterDel: TButton
        Left = 480
        Top = 408
        Width = 97
        Height = 25
        Caption = #21024#38500'(&D)'
        Enabled = False
        TabOrder = 3
        OnClick = ButtonMsgFilterDelClick
      end
      object ButtonMsgFilterChg: TButton
        Left = 584
        Top = 408
        Width = 97
        Height = 25
        Caption = #20462#25913'(&C)'
        Enabled = False
        TabOrder = 4
        OnClick = ButtonMsgFilterChgClick
      end
      object ButtonMsgFilterSave: TButton
        Left = 688
        Top = 408
        Width = 97
        Height = 25
        Caption = #20445#23384'(&S)'
        Enabled = False
        TabOrder = 5
        OnClick = ButtonMsgFilterSaveClick
      end
    end
    object TabSheet4: TTabSheet
      Caption = #29289#21697#35268#21017
      ImageIndex = 3
      object lbl31: TLabel
        Left = 8
        Top = 432
        Width = 186
        Height = 12
        Caption = 'Ctrl+'#24038#38190#28857#20987#22810#36873#65307#21491#38190#25209#37327#25805#20316
        Font.Charset = GB2312_CHARSET
        Font.Color = clRed
        Font.Height = -12
        Font.Name = #23435#20307
        Font.Style = []
        ParentFont = False
      end
      object GroupBox6: TGroupBox
        Left = 8
        Top = 4
        Width = 145
        Height = 423
        Caption = #38480#21046#29289#21697#21015#34920
        TabOrder = 0
        object ListBoxItemRuleList: TListBox
          Left = 8
          Top = 16
          Width = 129
          Height = 401
          ImeMode = imChinese
          ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
          ItemHeight = 12
          MultiSelect = True
          ParentShowHint = False
          PopupMenu = pmRuleList
          ShowHint = True
          TabOrder = 0
          OnClick = ListBoxItemRuleListClick
          OnKeyDown = ListBoxitemListKeyDown
        end
      end
      object GroupBox7: TGroupBox
        Left = 160
        Top = 4
        Width = 145
        Height = 423
        Caption = #29289#21697#21015#34920
        TabOrder = 1
        object ListBoxItemList2: TListBox
          Left = 8
          Top = 16
          Width = 129
          Height = 401
          Hint = 'Ctrl + F '#29289#21697#26597#25214#65292#21491#38190#21487#20197#36827#34892#31579#36873#12290
          ImeMode = imChinese
          ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
          ItemHeight = 12
          MultiSelect = True
          ParentShowHint = False
          PopupMenu = PopupMenu3
          ShowHint = True
          TabOrder = 0
          OnClick = ListBoxItemList2Click
          OnKeyDown = ListBoxitemListKeyDown
        end
      end
      object GroupBox8: TGroupBox
        Left = 312
        Top = 4
        Width = 465
        Height = 423
        Caption = #35268#21017#35774#32622
        TabOrder = 2
        object Label13: TLabel
          Left = 8
          Top = 20
          Width = 48
          Height = 12
          Caption = #29289#21697#21517#31216
        end
        object EditRuleItemName: TEdit
          Left = 64
          Top = 16
          Width = 185
          Height = 20
          ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
          MaxLength = 14
          TabOrder = 0
        end
        object GroupBoxItemRule: TGroupBox
          Left = 8
          Top = 38
          Width = 449
          Height = 334
          Caption = #29289#21697#35268#21017
          TabOrder = 1
          object lbl34: TLabel
            Left = 26
            Top = 136
            Width = 48
            Height = 12
            Caption = #25293#20987#38480#20215
          end
          object lbl35: TLabel
            Left = 181
            Top = 136
            Width = 36
            Height = 12
            Caption = #26368#20302#20215
          end
          object Label96: TLabel
            Left = 317
            Top = 136
            Width = 36
            Height = 12
            Caption = #26368#39640#20215
          end
          object CheckBox1: TCheckBox
            Left = 7
            Top = 16
            Width = 106
            Height = 17
            Caption = #31105#27490#20002#24323
            TabOrder = 0
          end
          object CheckBox3: TCheckBox
            Tag = 2
            Left = 7
            Top = 52
            Width = 106
            Height = 17
            Caption = #31105#27490#23384#20179
            TabOrder = 6
          end
          object CheckBox4: TCheckBox
            Tag = 4
            Left = 7
            Top = 70
            Width = 106
            Height = 17
            Caption = #31105#27490#20986#21806
            TabOrder = 9
          end
          object CheckBox5: TCheckBox
            Tag = 6
            Left = 280
            Top = 228
            Width = 106
            Height = 17
            Caption = #27515#20129#24517#29190
            TabOrder = 23
            OnClick = CheckBox5Click
          end
          object CheckBox6: TCheckBox
            Tag = 8
            Left = 7
            Top = 88
            Width = 106
            Height = 17
            Hint = #36873#20013#35813#39033#21518#31105#27490#35813#29289#21697#22312#20010#20154#21830#24215#20986#21806#20197#21450#25670#25674
            Caption = #31105#27490#23492#21806
            ParentShowHint = False
            ShowHint = True
            TabOrder = 12
          end
          object CheckBox7: TCheckBox
            Tag = 10
            Left = 141
            Top = 52
            Width = 106
            Height = 17
            Caption = #31105#27490#25361#25112
            TabOrder = 7
          end
          object CheckBox2: TCheckBox
            Tag = 12
            Left = 141
            Top = 163
            Width = 106
            Height = 17
            Hint = #29289#21697#20174#24618#29289#36523#19978#25481#33853#35302#21457'String.ini'#20013#30340'DropItemHint'#25552#31034
            Caption = #24618#29289#25481#33853#25552#31034
            ParentShowHint = False
            ShowHint = True
            TabOrder = 16
          end
          object CheckBox8: TCheckBox
            Tag = 14
            Left = 141
            Top = 70
            Width = 106
            Height = 17
            Caption = #31105#27490#21830#38138#25171#25240
            TabOrder = 10
          end
          object chkNoLevel: TCheckBox
            Tag = 17
            Left = 280
            Top = 16
            Width = 106
            Height = 17
            Hint = #35813#36873#39033#20165#38480#20110#27801#24052#20811#27494#22120#21319#32423','#31105#27490#21319#32423#30340#27494#22120#35774#32622
            Caption = #31105#27490#27494#22120#21319#32423
            ParentShowHint = False
            ShowHint = True
            TabOrder = 2
          end
          object chkNoSell: TCheckBox
            Tag = 20
            Left = 7
            Top = 34
            Width = 106
            Height = 17
            Caption = #31105#27490#25441#36215
            ParentShowHint = False
            ShowHint = False
            TabOrder = 3
          end
          object chkHeroItem: TCheckBox
            Tag = 16
            Left = 7
            Top = 264
            Width = 106
            Height = 17
            Hint = #29289#21697#21482#33021#33521#38596#31359#25140#19981#33021#20154#29289#31359#25140
            Caption = #33521#38596#29289#21697
            ParentShowHint = False
            ShowHint = True
            TabOrder = 27
          end
          object CheckBox12: TCheckBox
            Tag = 1
            Left = 141
            Top = 16
            Width = 106
            Height = 17
            Caption = #31105#27490#20132#26131
            TabOrder = 1
          end
          object CheckBox13: TCheckBox
            Tag = 3
            Left = 141
            Top = 34
            Width = 106
            Height = 17
            Caption = #31105#27490#20462#29702
            TabOrder = 4
          end
          object CheckBox14: TCheckBox
            Tag = 5
            Left = 141
            Top = 228
            Width = 106
            Height = 17
            Caption = #19978#32447#28040#22833
            TabOrder = 22
          end
          object CheckBox15: TCheckBox
            Tag = 7
            Left = 280
            Top = 52
            Width = 106
            Height = 17
            Caption = #31105#27490#33521#38596#35013#22791
            TabOrder = 8
          end
          object CheckBox16: TCheckBox
            Tag = 9
            Left = 141
            Top = 88
            Width = 106
            Height = 17
            Caption = #31105#27490#23384#20010#20154#21830#24215
            TabOrder = 13
          end
          object CheckBox17: TCheckBox
            Tag = 11
            Left = 280
            Top = 34
            Width = 106
            Height = 17
            Caption = #31105#27490#23453#30707#21319#32423
            TabOrder = 5
          end
          object CheckBox18: TCheckBox
            Tag = 13
            Left = 7
            Top = 228
            Width = 106
            Height = 17
            Hint = #36873#20013#35813#39033#21518','#20154#29289#27515#20129#35813#29289#21697#19981#20250#25481#33853
            Caption = #27704#19981#25481#33853
            ParentShowHint = False
            ShowHint = True
            TabOrder = 21
            OnClick = CheckBox18Click
          end
          object chkHeroBag: TCheckBox
            Tag = 15
            Left = 280
            Top = 70
            Width = 106
            Height = 17
            Hint = #29289#21697#31105#27490#25918#21040#33521#38596#21253#35065
            Caption = #31105#27490#33521#38596#21253#35065
            ParentShowHint = False
            ShowHint = True
            TabOrder = 11
          end
          object chkButchItem: TCheckBox
            Tag = 19
            Left = 7
            Top = 181
            Width = 106
            Height = 17
            Hint = #20174#24618#29289#25110#20154#24418#24618#36523#19978#25366#21040#25351#23450#29289#21697#26102#36827#34892#20840#26381#25991#23383#25552#31034','#25552#31034#25991#23383#22312'"String.ini"'#20013#20462#25913
            Caption = #25366#21462#25552#31034
            ParentShowHint = False
            ShowHint = True
            TabOrder = 18
          end
          object chkTriggerHint: TCheckBox
            Tag = 21
            Left = 7
            Top = 163
            Width = 106
            Height = 17
            Hint = #36873#20013#35813#39033#21518#29289#21697#25172#12289#25441#21017#36827#34892#33050#26412#35302#21457
            Caption = #35302#21457#25552#31034
            ParentShowHint = False
            ShowHint = True
            TabOrder = 15
          end
          object chkNoGift: TCheckBox
            Tag = 18
            Left = 280
            Top = 246
            Width = 106
            Height = 17
            Caption = #27515#20129#28040#22833
            ParentShowHint = False
            ShowHint = True
            TabOrder = 26
          end
          object ButtonItemRuleSelAll: TButton
            Left = 8
            Top = 288
            Width = 70
            Height = 25
            Caption = #20840#37096#36873#20013
            TabOrder = 29
            OnClick = ButtonItemRuleSelAllClick
          end
          object ButtonItemRuleNotSelAll: TButton
            Left = 84
            Top = 288
            Width = 70
            Height = 25
            Caption = #20840#37096#19981#36873
            TabOrder = 30
            OnClick = ButtonItemRuleNotSelAllClick
          end
          object CheckBox24: TCheckBox
            Tag = 26
            Left = 280
            Top = 163
            Width = 106
            Height = 17
            Hint = #29289#21697#20174#20154#29289#12289#33521#38596#25110#20551#20154#36523#19978#25481#33853#35302#21457'String.ini'#20013#30340'HumDropItemHint'#25552#31034
            Caption = #20154#29289#25481#33853#25552#31034
            ParentShowHint = False
            ShowHint = True
            TabOrder = 17
          end
          object CheckBox25: TCheckBox
            Tag = 24
            Left = 7
            Top = 246
            Width = 106
            Height = 17
            Caption = #20002#24323#28040#22833
            TabOrder = 24
          end
          object CheckBox26: TCheckBox
            Tag = 22
            Left = 141
            Top = 246
            Width = 106
            Height = 17
            Caption = #19979#32447#24517#25481
            TabOrder = 25
          end
          object CheckBox27: TCheckBox
            Tag = 23
            Left = 141
            Top = 181
            Width = 106
            Height = 17
            Hint = #21246#36873#21518#21017#23453#31665#20013#33719#21462#27492#29289#21697#35302#21457'String.ini'#20844#21578#20869#23481#36827#34892#20840#21306#20844#21578'.'
            Caption = #23453#31665#25552#31034
            ParentShowHint = False
            ShowHint = True
            TabOrder = 19
          end
          object CheckBox28: TCheckBox
            Tag = 25
            Left = 141
            Top = 264
            Width = 106
            Height = 17
            Hint = 
              #24403#26576#20010#29289#21697#25441#36215#25110#20002#24323#26102#65292#35302#21457#23545#24212#30340#21629#20196#13#10#13#10#20154#29289#65306'@DropItemsXX, @PickUpItemsXX'#13#10#33521#38596#65306'@H.Dr' +
              'opItemsXX, @H.PickUpItemsXX'#13#10#13#10#27880#24847#65306'XX'#34920#31034#29289#21697#22312#25968#25454#24211#30340'ID'
            Caption = #35302#21457'ID'
            ParentShowHint = False
            ShowHint = True
            TabOrder = 28
          end
          object CheckBox29: TCheckBox
            Tag = 27
            Left = 8
            Top = 113
            Width = 106
            Height = 17
            Caption = #20801#35768#25293#21334
            TabOrder = 14
          end
          object CheckBox20: TCheckBox
            Tag = 28
            Left = 280
            Top = 181
            Width = 106
            Height = 17
            Hint = #29289#21697#25481#33853#35302#21457'qf'#20013#30340'[@M2DropItem]'
            Caption = #24618#29289#25481#33853#35302#21457
            ParentShowHint = False
            ShowHint = True
            TabOrder = 20
          end
          object cbbAuctionPricesType: TComboBox
            Left = 77
            Top = 132
            Width = 92
            Height = 20
            Style = csDropDownList
            TabOrder = 31
            OnChange = cbbAuctionPricesTypeChange
          end
          object seAuctionPrices_Min: TSpinEditLongWord
            Left = 221
            Top = 131
            Width = 85
            Height = 21
            Hint = #20026'0'#34920#31034#19981#38480#20215
            MaxValue = 0
            MinValue = 0
            ParentShowHint = False
            ShowHint = True
            TabOrder = 32
            Value = 0
            OnChange = seAuctionPrices_MinChange
          end
          object seAuctionPrices_Max: TSpinEditLongWord
            Left = 357
            Top = 131
            Width = 85
            Height = 21
            Hint = #20026'0'#34920#31034#19981#38480#20215
            MaxValue = 0
            MinValue = 0
            ParentShowHint = False
            ShowHint = True
            TabOrder = 33
            Value = 0
            OnChange = seAuctionPrices_MaxChange
          end
          object chkTriggerGetBoxsItem: TCheckBox
            Tag = 29
            Left = 7
            Top = 199
            Width = 106
            Height = 17
            Hint = #21246#36873#21518#21017#23453#31665#20013#33719#21462#27492#29289#21697#35302#21457'qf'#20013#30340'[@GetBoxsItem]'
            Caption = #23453#31665#35302#21457
            ParentShowHint = False
            ShowHint = True
            TabOrder = 34
          end
          object chkDisablePreviewMonItem: TCheckBox
            Tag = 30
            Left = 141
            Top = 199
            Width = 106
            Height = 17
            Caption = #31105#27490#36879#35270
            ParentShowHint = False
            ShowHint = True
            TabOrder = 35
          end
          object CheckBox21: TCheckBox
            Tag = 31
            Left = 280
            Top = 88
            Width = 106
            Height = 17
            Hint = #29289#21697#31105#27490#25918#21040#33521#38596#21253#35065
            Caption = #31105#27490#23456#29289#21253#35065
            ParentShowHint = False
            ShowHint = True
            TabOrder = 36
          end
          object chkHumDrop: TCheckBox
            Tag = 39
            Left = 280
            Top = 199
            Width = 106
            Height = 17
            Hint = #29289#21697#25481#33853#35302#21457'qf'#20013#30340'[@HumDropItem]'
            Caption = #20154#29289#25481#33853#35302#21457
            ParentShowHint = False
            ShowHint = True
            TabOrder = 37
          end
        end
        object ButtonItemRuleAdd: TButton
          Left = 20
          Top = 383
          Width = 63
          Height = 25
          Caption = #22686#21152
          TabOrder = 2
          OnClick = ButtonItemRuleAddClick
        end
        object ButtonItemRuleDel: TButton
          Left = 164
          Top = 383
          Width = 63
          Height = 25
          Caption = #21024#38500
          TabOrder = 3
          OnClick = ButtonItemRuleDelClick
        end
        object ButtonItemRuleAddAll: TButton
          Left = 92
          Top = 383
          Width = 63
          Height = 25
          Caption = #20840#37096#22686#21152
          TabOrder = 4
          OnClick = ButtonItemRuleAddAllClick
        end
        object ButtonItemRuleDelAll: TButton
          Left = 237
          Top = 383
          Width = 63
          Height = 25
          Caption = #20840#37096#21024#38500
          TabOrder = 5
          OnClick = ButtonItemRuleDelAllClick
        end
        object ButtonItemRuleChg: TButton
          Left = 309
          Top = 383
          Width = 63
          Height = 25
          Caption = #20462#25913
          TabOrder = 6
          OnClick = ButtonItemRuleChgClick
        end
        object ButtonItemRuleSave: TButton
          Left = 381
          Top = 383
          Width = 63
          Height = 25
          Caption = #20445#23384
          TabOrder = 7
          OnClick = ButtonItemRuleSaveClick
        end
      end
    end
    object TabSheet5: TTabSheet
      Caption = #29992#25143#21629#20196
      ImageIndex = 4
      object GroupBox5: TGroupBox
        Left = 8
        Top = 4
        Width = 201
        Height = 430
        Caption = #33258#23450#20041#21629#20196#21015#34920
        TabOrder = 0
        object ListBoxUserCommand: TListBox
          Left = 8
          Top = 16
          Width = 185
          Height = 401
          ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
          ItemHeight = 12
          TabOrder = 0
          OnClick = ListBoxUserCommandClick
        end
      end
      object GroupBox10: TGroupBox
        Left = 216
        Top = 4
        Width = 489
        Height = 430
        Caption = #21629#20196#32534#36753
        TabOrder = 1
        object Label11: TLabel
          Left = 8
          Top = 24
          Width = 60
          Height = 12
          Caption = #21629#20196#21517#31216#65306
        end
        object Label12: TLabel
          Left = 8
          Top = 48
          Width = 60
          Height = 12
          Caption = #21629#20196#32534#21495#65306
        end
        object LabelMsg: TLabel
          Left = 8
          Top = 128
          Width = 48
          Height = 12
          Caption = 'LabelMsg'
          Font.Charset = GB2312_CHARSET
          Font.Color = clRed
          Font.Height = -12
          Font.Name = #23435#20307
          Font.Style = []
          ParentFont = False
        end
        object EditCommandName: TEdit
          Left = 72
          Top = 20
          Width = 161
          Height = 20
          ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
          MaxLength = 30
          TabOrder = 0
        end
        object EditCommandIdx: TSpinEditEx
          Left = 72
          Top = 44
          Width = 161
          Height = 21
          MaxValue = 65535
          MinValue = 0
          TabOrder = 1
          Value = 0
        end
        object ButtonUserCommandAdd: TButton
          Left = 8
          Top = 72
          Width = 75
          Height = 25
          Caption = #22686#21152'(&A)'
          TabOrder = 2
          OnClick = ButtonUserCommandAddClick
        end
        object ButtonUserCommandDel: TButton
          Left = 88
          Top = 72
          Width = 75
          Height = 25
          Caption = #21024#38500'(&D)'
          TabOrder = 3
          OnClick = ButtonUserCommandDelClick
        end
        object ButtonUserCommandSave: TButton
          Left = 168
          Top = 72
          Width = 75
          Height = 25
          Caption = #20445#23384'(&S)'
          TabOrder = 4
          OnClick = ButtonUserCommandSaveClick
        end
      end
    end
    object TabSheet6: TTabSheet
      Caption = #22871#35013
      ImageIndex = 5
      object PageControlTzItem: TPageControl
        Left = 0
        Top = 0
        Width = 780
        Height = 444
        ActivePage = TabSheet13
        Align = alClient
        TabOrder = 0
        ExplicitWidth = 784
        ExplicitHeight = 445
        object TabSheet13: TTabSheet
          Caption = #22871#35013#31995#32479
          object Label32: TLabel
            Left = 112
            Top = 356
            Width = 54
            Height = 12
            Caption = #22871#35013#25968#37327':'
          end
          object Label51: TLabel
            Left = 0
            Top = 356
            Width = 54
            Height = 12
            Caption = #22871#35013#32534#21495':'
            Font.Charset = GB2312_CHARSET
            Font.Color = clRed
            Font.Height = -12
            Font.Name = #23435#20307
            Font.Style = []
            ParentFont = False
          end
          object Label33: TLabel
            Left = 0
            Top = 378
            Width = 54
            Height = 12
            Caption = #22871#35013#29289#21697':'
          end
          object lbl29: TLabel
            Left = 0
            Top = 401
            Width = 42
            Height = 12
            Caption = #25552'  '#31034':'
          end
          object Label31: TLabel
            Left = 224
            Top = 356
            Width = 54
            Height = 12
            Caption = #22871#35013#35828#26126':'
          end
          object GroupBox24: TGroupBox
            Left = 0
            Top = 4
            Width = 433
            Height = 341
            Caption = #22871#35013#21015#34920
            TabOrder = 0
            object ListViewGroupItemList: TListView
              Left = 8
              Top = 16
              Width = 417
              Height = 318
              Columns = <
                item
                  Caption = #22871#35013#32534#21495
                  Width = 60
                end
                item
                  Caption = #22871#35013#35828#26126
                  Width = 90
                end
                item
                  Caption = #25968#37327
                  Width = 60
                end
                item
                  Caption = #22871#35013#29289#21697
                  Width = 300
                end>
              GridLines = True
              ReadOnly = True
              RowSelect = True
              TabOrder = 0
              ViewStyle = vsReport
              OnClick = ListViewGroupItemListClick
            end
          end
          object GroupBoxGroupItem: TGroupBox
            Left = 441
            Top = 4
            Width = 336
            Height = 381
            Caption = #38468#21152#23646#24615#35774#32622
            TabOrder = 1
            object CheckBoxGroupItemFlag1: TCheckBox
              Left = 123
              Top = 251
              Width = 49
              Height = 17
              Caption = #40635#30201
              TabOrder = 4
            end
            object CheckBoxGroupItemFlag2: TCheckBox
              Tag = 1
              Left = 123
              Top = 270
              Width = 49
              Height = 17
              Caption = #25252#36523
              TabOrder = 9
            end
            object CheckBoxGroupItemFlag3: TCheckBox
              Tag = 2
              Left = 11
              Top = 270
              Width = 47
              Height = 17
              Caption = #20256#36865
              TabOrder = 7
            end
            object CheckBoxGroupItemFlag4: TCheckBox
              Tag = 3
              Left = 123
              Top = 289
              Width = 49
              Height = 17
              Caption = #22797#27963
              TabOrder = 14
            end
            object CheckBoxGroupItemFlag8: TCheckBox
              Tag = 7
              Left = 59
              Top = 251
              Width = 49
              Height = 17
              Caption = #21560#34880
              Enabled = False
              TabOrder = 3
            end
            object CheckBoxGroupItemFlag7: TCheckBox
              Tag = 6
              Left = 59
              Top = 270
              Width = 49
              Height = 17
              Caption = #25506#27979
              TabOrder = 8
            end
            object CheckBoxGroupItemFlag6: TCheckBox
              Tag = 5
              Left = 11
              Top = 289
              Width = 47
              Height = 17
              Caption = #25216#24039
              TabOrder = 12
            end
            object CheckBoxGroupItemFlag5: TCheckBox
              Tag = 4
              Left = 59
              Top = 289
              Width = 49
              Height = 17
              Caption = #36127#36733
              TabOrder = 13
            end
            object CheckBoxGroupItemFlag16: TCheckBox
              Tag = 15
              Left = 243
              Top = 310
              Width = 89
              Height = 17
              Caption = #19981#25481#36523#19978#35013#22791
              TabOrder = 21
            end
            object CheckBoxGroupItemFlag15: TCheckBox
              Tag = 14
              Left = 243
              Top = 289
              Width = 89
              Height = 17
              Caption = #19981#25481#32972#21253#35013#22791
              TabOrder = 16
            end
            object CheckBoxGroupItemFlag14: TCheckBox
              Tag = 13
              Left = 243
              Top = 270
              Width = 65
              Height = 17
              Caption = #35760#24518#23646#24615
              Enabled = False
              TabOrder = 11
            end
            object CheckBoxGroupItemFlag13: TCheckBox
              Tag = 12
              Left = 176
              Top = 270
              Width = 58
              Height = 17
              Caption = #30772#25252#36523
              TabOrder = 10
            end
            object CheckBoxGroupItemFlag12: TCheckBox
              Tag = 11
              Left = 176
              Top = 289
              Width = 58
              Height = 17
              Caption = #30772#22797#27963
              TabOrder = 15
            end
            object CheckBoxGroupItemFlag11: TCheckBox
              Tag = 10
              Left = 243
              Top = 251
              Width = 57
              Height = 17
              Caption = #38450#20840#27602
              TabOrder = 6
            end
            object CheckBoxGroupItemFlag10: TCheckBox
              Tag = 9
              Left = 176
              Top = 251
              Width = 58
              Height = 17
              Caption = #38450#40635#30201
              TabOrder = 5
            end
            object CheckBoxGroupItemFlag9: TCheckBox
              Tag = 8
              Left = 11
              Top = 251
              Width = 47
              Height = 17
              Caption = #38544#36523
              TabOrder = 2
            end
            object PageControl2: TPageControl
              Left = 4
              Top = 20
              Width = 329
              Height = 230
              ActivePage = TabSheetValue
              Style = tsButtons
              TabOrder = 0
              object TabSheetRate: TTabSheet
                Caption = #27604#20363#22686#21152
                object lbl19: TLabel
                  Left = 0
                  Top = 6
                  Width = 42
                  Height = 12
                  Caption = 'HP'#19978#38480':'
                end
                object lbl20: TLabel
                  Left = 0
                  Top = 30
                  Width = 42
                  Height = 12
                  Caption = 'MP'#19978#38480':'
                end
                object Label14: TLabel
                  Left = 0
                  Top = 54
                  Width = 42
                  Height = 12
                  Caption = #20934'  '#30830':'
                end
                object Label20: TLabel
                  Left = 0
                  Top = 78
                  Width = 42
                  Height = 12
                  Caption = #25935'  '#25463':'
                end
                object Label19: TLabel
                  Left = 0
                  Top = 102
                  Width = 42
                  Height = 12
                  Caption = #39764#36530#36991':'
                end
                object Label18: TLabel
                  Left = 0
                  Top = 126
                  Width = 42
                  Height = 12
                  Caption = #27602#36530#36991':'
                end
                object Label17: TLabel
                  Left = 0
                  Top = 150
                  Width = 42
                  Height = 12
                  Caption = #27602#24674#22797':'
                end
                object Label16: TLabel
                  Left = 0
                  Top = 175
                  Width = 42
                  Height = 12
                  Caption = 'HP'#24674#22797':'
                end
                object Label15: TLabel
                  Left = 145
                  Top = 6
                  Width = 42
                  Height = 12
                  Caption = 'MP'#24674#22797':'
                end
                object Label25: TLabel
                  Left = 105
                  Top = 103
                  Width = 24
                  Height = 12
                  Caption = '/100'
                end
                object Label26: TLabel
                  Left = 105
                  Top = 127
                  Width = 24
                  Height = 12
                  Caption = '/100'
                end
                object Label37: TLabel
                  Left = 105
                  Top = 6
                  Width = 24
                  Height = 12
                  Caption = '/100'
                end
                object Label38: TLabel
                  Left = 105
                  Top = 30
                  Width = 24
                  Height = 12
                  Caption = '/100'
                end
                object lbl25: TLabel
                  Left = 145
                  Top = 30
                  Width = 42
                  Height = 12
                  Caption = #29289'  '#38450':'
                end
                object lbl27: TLabel
                  Left = 145
                  Top = 54
                  Width = 42
                  Height = 12
                  Caption = #39764'  '#38450':'
                end
                object lbl9: TLabel
                  Left = 145
                  Top = 78
                  Width = 42
                  Height = 12
                  Caption = #25915#20987#21147':'
                end
                object lbl11: TLabel
                  Left = 145
                  Top = 102
                  Width = 42
                  Height = 12
                  Caption = #39764#27861#21147':'
                end
                object lbl14: TLabel
                  Left = 145
                  Top = 126
                  Width = 42
                  Height = 12
                  Caption = #36947#26415#21147':'
                end
                object Label39: TLabel
                  Left = 296
                  Top = 30
                  Width = 24
                  Height = 12
                  Caption = '/100'
                end
                object Label40: TLabel
                  Left = 296
                  Top = 54
                  Width = 24
                  Height = 12
                  Caption = '/100'
                end
                object Label41: TLabel
                  Left = 296
                  Top = 78
                  Width = 24
                  Height = 12
                  Caption = '/100'
                end
                object Label42: TLabel
                  Left = 296
                  Top = 102
                  Width = 24
                  Height = 12
                  Caption = '/100'
                end
                object Label43: TLabel
                  Left = 296
                  Top = 126
                  Width = 24
                  Height = 12
                  Caption = '/100'
                end
                object Label9: TLabel
                  Left = 105
                  Top = 54
                  Width = 24
                  Height = 12
                  Caption = '/100'
                end
                object Label21: TLabel
                  Left = 105
                  Top = 78
                  Width = 24
                  Height = 12
                  Caption = '/100'
                end
                object Label24: TLabel
                  Left = 105
                  Top = 151
                  Width = 24
                  Height = 12
                  Caption = '/100'
                end
                object Label28: TLabel
                  Left = 105
                  Top = 175
                  Width = 24
                  Height = 12
                  Caption = '/100'
                end
                object Label29: TLabel
                  Left = 244
                  Top = 6
                  Width = 24
                  Height = 12
                  Caption = '/100'
                end
                object lbl36: TLabel
                  Left = 144
                  Top = 151
                  Width = 174
                  Height = 12
                  Caption = ' '#23646#24615#20540' + '#23646#24615#20540' * '#27604#20363#20540'/100'
                  Font.Charset = GB2312_CHARSET
                  Font.Color = clBlue
                  Font.Height = -12
                  Font.Name = #23435#20307
                  Font.Style = []
                  ParentFont = False
                end
                object EditGroupItemMPRate: TSpinEditEx
                  Tag = 1
                  Left = 44
                  Top = 26
                  Width = 60
                  Height = 21
                  MaxValue = 2147483647
                  MinValue = 0
                  TabOrder = 2
                  Value = 0
                end
                object EditGroupItemHPRate: TSpinEditEx
                  Left = 44
                  Top = 2
                  Width = 60
                  Height = 21
                  MaxValue = 2147483647
                  MinValue = 0
                  TabOrder = 0
                  Value = 0
                end
                object EditSpellRecoverRate: TSpinEditEx
                  Tag = 13
                  Left = 189
                  Top = 2
                  Width = 52
                  Height = 21
                  MaxValue = 25500
                  MinValue = 0
                  TabOrder = 1
                  Value = 0
                end
                object EditHealthRecoverRate: TSpinEditEx
                  Tag = 12
                  Left = 44
                  Top = 170
                  Width = 60
                  Height = 21
                  MaxValue = 25500
                  MinValue = 0
                  TabOrder = 18
                  Value = 0
                end
                object EditPoisonRecoverRate: TSpinEditEx
                  Tag = 11
                  Left = 44
                  Top = 146
                  Width = 60
                  Height = 21
                  MaxValue = 25500
                  MinValue = 0
                  TabOrder = 17
                  Value = 0
                end
                object EditAntiPoisonRate: TSpinEditEx
                  Tag = 10
                  Left = 44
                  Top = 122
                  Width = 60
                  Height = 21
                  MaxValue = 25500
                  MinValue = 0
                  TabOrder = 14
                  Value = 0
                end
                object EditAntiMagicRate: TSpinEditEx
                  Tag = 9
                  Left = 44
                  Top = 98
                  Width = 60
                  Height = 21
                  MaxValue = 25500
                  MinValue = 0
                  TabOrder = 11
                  Value = 0
                end
                object EditSpeedPointRate: TSpinEditEx
                  Tag = 8
                  Left = 44
                  Top = 74
                  Width = 60
                  Height = 21
                  MaxValue = 2147483647
                  MinValue = 0
                  TabOrder = 8
                  Value = 0
                end
                object EditHitPointRate: TSpinEditEx
                  Tag = 7
                  Left = 44
                  Top = 50
                  Width = 60
                  Height = 21
                  MaxValue = 2147483647
                  MinValue = 0
                  TabOrder = 5
                  Value = 0
                end
                object EditGroupItemSCRate: TSpinEditEx
                  Tag = 6
                  Left = 189
                  Top = 122
                  Width = 51
                  Height = 21
                  MaxValue = 2147483647
                  MinValue = 0
                  TabOrder = 15
                  Value = 0
                end
                object EditGroupItemMCRate: TSpinEditEx
                  Tag = 5
                  Left = 189
                  Top = 98
                  Width = 51
                  Height = 21
                  MaxValue = 2147483647
                  MinValue = 0
                  TabOrder = 12
                  Value = 0
                end
                object EditGroupItemDCRate: TSpinEditEx
                  Tag = 4
                  Left = 189
                  Top = 74
                  Width = 51
                  Height = 21
                  MaxValue = 2147483647
                  MinValue = 0
                  TabOrder = 9
                  Value = 0
                end
                object EditGroupItemMACRate: TSpinEditEx
                  Tag = 3
                  Left = 189
                  Top = 50
                  Width = 51
                  Height = 21
                  MaxValue = 2147483647
                  MinValue = 0
                  TabOrder = 6
                  Value = 0
                end
                object EditGroupItemACRate: TSpinEditEx
                  Tag = 2
                  Left = 189
                  Top = 26
                  Width = 51
                  Height = 21
                  MaxValue = 2147483647
                  MinValue = 0
                  TabOrder = 3
                  Value = 0
                end
                object EditGroupItemSCRate2: TSpinEditEx
                  Tag = 18
                  Left = 242
                  Top = 122
                  Width = 51
                  Height = 21
                  MaxValue = 2147483647
                  MinValue = 0
                  TabOrder = 16
                  Value = 0
                end
                object EditGroupItemMCRate2: TSpinEditEx
                  Tag = 17
                  Left = 242
                  Top = 98
                  Width = 51
                  Height = 21
                  MaxValue = 2147483647
                  MinValue = 0
                  TabOrder = 13
                  Value = 0
                end
                object EditGroupItemDCRate2: TSpinEditEx
                  Tag = 16
                  Left = 242
                  Top = 74
                  Width = 51
                  Height = 21
                  MaxValue = 2147483647
                  MinValue = 0
                  TabOrder = 10
                  Value = 0
                end
                object EditGroupItemMACRate2: TSpinEditEx
                  Tag = 15
                  Left = 242
                  Top = 50
                  Width = 51
                  Height = 21
                  MaxValue = 2147483647
                  MinValue = 0
                  TabOrder = 7
                  Value = 0
                end
                object EditGroupItemACRate2: TSpinEditEx
                  Tag = 14
                  Left = 242
                  Top = 26
                  Width = 51
                  Height = 21
                  MaxValue = 2147483647
                  MinValue = 0
                  TabOrder = 4
                  Value = 0
                end
              end
              object TabSheetValue: TTabSheet
                Caption = #22686#21152#23646#24615#28857
                ImageIndex = 1
                object Label69: TLabel
                  Left = 145
                  Top = 30
                  Width = 42
                  Height = 12
                  Caption = #29289'  '#38450':'
                end
                object Label70: TLabel
                  Left = 145
                  Top = 54
                  Width = 42
                  Height = 12
                  Caption = #39764'  '#38450':'
                end
                object Label71: TLabel
                  Left = 145
                  Top = 78
                  Width = 42
                  Height = 12
                  Caption = #25915#20987#21147':'
                end
                object Label72: TLabel
                  Left = 145
                  Top = 102
                  Width = 42
                  Height = 12
                  Caption = #39764#27861#21147':'
                end
                object Label73: TLabel
                  Left = 145
                  Top = 126
                  Width = 42
                  Height = 12
                  Caption = #36947#26415#21147':'
                end
                object Label67: TLabel
                  Left = 8
                  Top = 6
                  Width = 42
                  Height = 12
                  Caption = 'HP'#19978#38480':'
                end
                object Label68: TLabel
                  Left = 8
                  Top = 30
                  Width = 42
                  Height = 12
                  Caption = 'MP'#19978#38480':'
                end
                object Label74: TLabel
                  Left = 8
                  Top = 54
                  Width = 42
                  Height = 12
                  Caption = #20934'  '#30830':'
                end
                object Label75: TLabel
                  Left = 8
                  Top = 78
                  Width = 42
                  Height = 12
                  Caption = #25935'  '#25463':'
                end
                object Label76: TLabel
                  Left = 8
                  Top = 102
                  Width = 42
                  Height = 12
                  Caption = #39764#36530#36991':'
                end
                object Label77: TLabel
                  Left = 8
                  Top = 126
                  Width = 42
                  Height = 12
                  Caption = #27602#36530#36991':'
                end
                object Label78: TLabel
                  Left = 8
                  Top = 150
                  Width = 42
                  Height = 12
                  Caption = #27602#24674#22797':'
                end
                object Label79: TLabel
                  Left = 8
                  Top = 175
                  Width = 42
                  Height = 12
                  Caption = 'HP'#24674#22797':'
                end
                object Label80: TLabel
                  Left = 145
                  Top = 6
                  Width = 42
                  Height = 12
                  Caption = 'MP'#24674#22797':'
                end
                object Label36: TLabel
                  Left = 133
                  Top = 150
                  Width = 54
                  Height = 12
                  Caption = #32463#39564#20493#25968':'
                end
                object Label61: TLabel
                  Left = 254
                  Top = 150
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object seGroupItemSCValue: TSpinEditEx
                  Tag = 6
                  Left = 189
                  Top = 121
                  Width = 60
                  Height = 21
                  Hint = #36947#26415#21147#19979#38480
                  MaxValue = 2147483647
                  MinValue = 0
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 14
                  Value = 0
                end
                object seGroupItemMCValue: TSpinEditEx
                  Tag = 5
                  Left = 189
                  Top = 97
                  Width = 60
                  Height = 21
                  Hint = #39764#27861#21147#19979#38480
                  MaxValue = 2147483647
                  MinValue = 0
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 11
                  Value = 0
                end
                object seGroupItemDCValue: TSpinEditEx
                  Tag = 4
                  Left = 189
                  Top = 73
                  Width = 60
                  Height = 21
                  Hint = #25915#20987#21147#19979#38480
                  MaxValue = 2147483647
                  MinValue = 0
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 8
                  Value = 0
                end
                object seGroupItemMACValue: TSpinEditEx
                  Tag = 3
                  Left = 189
                  Top = 49
                  Width = 60
                  Height = 21
                  Hint = #39764#24481#19979#38480
                  MaxValue = 2147483647
                  MinValue = 0
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 5
                  Value = 0
                end
                object seGroupItemACValue: TSpinEditEx
                  Tag = 2
                  Left = 189
                  Top = 25
                  Width = 60
                  Height = 21
                  Hint = #38450#24481#19979#38480
                  MaxValue = 2147483647
                  MinValue = 0
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 2
                  Value = 0
                end
                object seGroupItemACValue2: TSpinEditEx
                  Tag = 14
                  Left = 254
                  Top = 25
                  Width = 60
                  Height = 21
                  Hint = #38450#24481#19978#38480
                  MaxValue = 2147483647
                  MinValue = 0
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 3
                  Value = 0
                end
                object seGroupItemMACValue2: TSpinEditEx
                  Tag = 15
                  Left = 254
                  Top = 49
                  Width = 60
                  Height = 21
                  Hint = #39764#24481#19978#38480
                  MaxValue = 2147483647
                  MinValue = 0
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 6
                  Value = 0
                end
                object seGroupItemDCValue2: TSpinEditEx
                  Tag = 16
                  Left = 253
                  Top = 73
                  Width = 60
                  Height = 21
                  Hint = #25915#20987#21147#19978#38480
                  MaxValue = 2147483647
                  MinValue = 0
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 9
                  Value = 0
                end
                object seGroupItemMCValue2: TSpinEditEx
                  Tag = 17
                  Left = 253
                  Top = 97
                  Width = 60
                  Height = 21
                  Hint = #39764#27861#21147#19978#38480
                  MaxValue = 2147483647
                  MinValue = 0
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 12
                  Value = 0
                end
                object seGroupItemSCValue2: TSpinEditEx
                  Tag = 18
                  Left = 253
                  Top = 121
                  Width = 60
                  Height = 21
                  Hint = #36947#26415#21147#19978#38480
                  MaxValue = 2147483647
                  MinValue = 0
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 15
                  Value = 0
                end
                object seGroupItemMPValue: TSpinEditEx
                  Tag = 1
                  Left = 52
                  Top = 26
                  Width = 65
                  Height = 21
                  MaxValue = 2147483647
                  MinValue = 0
                  TabOrder = 4
                  Value = 0
                end
                object seGroupItemHPValue: TSpinEditEx
                  Left = 52
                  Top = 2
                  Width = 65
                  Height = 21
                  MaxValue = 2147483647
                  MinValue = 0
                  TabOrder = 1
                  Value = 0
                end
                object seSpellRecoverValue: TSpinEditEx
                  Tag = 13
                  Left = 189
                  Top = 1
                  Width = 60
                  Height = 21
                  MaxValue = 255
                  MinValue = 0
                  TabOrder = 0
                  Value = 0
                end
                object seHealthRecoverValue: TSpinEditEx
                  Tag = 12
                  Left = 52
                  Top = 170
                  Width = 65
                  Height = 21
                  MaxValue = 255
                  MinValue = 0
                  TabOrder = 19
                  Value = 0
                end
                object sePoisonRecoverValue: TSpinEditEx
                  Tag = 11
                  Left = 52
                  Top = 146
                  Width = 65
                  Height = 21
                  MaxValue = 255
                  MinValue = 0
                  TabOrder = 18
                  Value = 0
                end
                object seAntiPoisonValue: TSpinEditEx
                  Tag = 10
                  Left = 52
                  Top = 122
                  Width = 65
                  Height = 21
                  MaxValue = 255
                  MinValue = 0
                  TabOrder = 16
                  Value = 0
                end
                object seAntiMagicValue: TSpinEditEx
                  Tag = 9
                  Left = 52
                  Top = 98
                  Width = 65
                  Height = 21
                  MaxValue = 255
                  MinValue = 0
                  TabOrder = 13
                  Value = 0
                end
                object seSpeedPointValue: TSpinEditEx
                  Tag = 8
                  Left = 52
                  Top = 74
                  Width = 65
                  Height = 21
                  MaxValue = 2147483647
                  MinValue = 0
                  TabOrder = 10
                  Value = 0
                end
                object seHitPointValue: TSpinEditEx
                  Tag = 7
                  Left = 52
                  Top = 50
                  Width = 65
                  Height = 21
                  MaxValue = 2147483647
                  MinValue = 0
                  TabOrder = 7
                  Value = 0
                end
                object SpinEditEx6: TSpinEditEx
                  Tag = 19
                  Left = 189
                  Top = 145
                  Width = 60
                  Height = 21
                  Hint = 
                    '100'#34920#31034#19981#22686#21152#32463#39564#20493#25968#65292'150'#34920#31034'1.5'#20493#32463#39564#13#10#13#10#24403#39046#21462#30340#32463#39564#20493#25968#36229#36807#22871#35013#32463#39564#20493#25968#26102#20197#39046#21462#30340#32463#39564#20493#25968#20026#20934#13#10#21542#21017#20197#22871#35013#20493#25968 +
                    #20026#20934'('#22914'X'#20493#32463#39564#21367')'
                  MaxValue = 2147483647
                  MinValue = 100
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 17
                  Value = 100
                end
              end
            end
            object RadioButtonRate: TRadioButton
              Left = 8
              Top = 357
              Width = 148
              Height = 17
              Caption = #20808#21152#30334#20998#27604#20877#21152#23646#24615#28857
              Checked = True
              TabOrder = 23
              TabStop = True
              OnClick = RadioButtonRateClick
            end
            object RadioButtonValue: TRadioButton
              Tag = 1
              Left = 160
              Top = 357
              Width = 148
              Height = 17
              Caption = #20808#21152#23646#24615#28857#20877#21152#30334#20998#27604
              TabOrder = 24
              OnClick = RadioButtonValueClick
            end
            object ButtonGroupItemSkillPower: TButton
              Left = 197
              Top = 217
              Width = 128
              Height = 26
              Caption = #22871#35013#25216#33021#23041#21147#30334#20998#27604
              Enabled = False
              TabOrder = 1
              OnClick = ButtonGroupItemSkillPowerClick
            end
            object CheckBox9: TCheckBox
              Tag = 16
              Left = 11
              Top = 310
              Width = 47
              Height = 17
              Caption = #20912#20923
              TabOrder = 17
            end
            object CheckBox10: TCheckBox
              Tag = 17
              Left = 59
              Top = 310
              Width = 62
              Height = 17
              Caption = #38450#20912#20923
              TabOrder = 18
            end
            object CheckBox11: TCheckBox
              Tag = 18
              Left = 123
              Top = 310
              Width = 47
              Height = 17
              Caption = #34523#32593
              TabOrder = 19
            end
            object CheckBox19: TCheckBox
              Tag = 19
              Left = 176
              Top = 310
              Width = 62
              Height = 17
              Caption = #38450#34523#32593
              TabOrder = 20
            end
            object chk1: TCheckBox
              Tag = 20
              Left = 11
              Top = 332
              Width = 73
              Height = 17
              Caption = #39764#36947#40635#30201
              TabOrder = 22
            end
          end
          object EditGroupItemIndex: TSpinEditEx
            Left = 56
            Top = 352
            Width = 49
            Height = 21
            Hint = #29992#20110#22871#35013#35302#21457
            MaxValue = 65535
            MinValue = 0
            ParentShowHint = False
            ShowHint = True
            TabOrder = 2
            Value = 1
          end
          object EditGroupItemHint: TEdit
            Left = 56
            Top = 398
            Width = 377
            Height = 20
            Hint = #31359#40784#19968#22871#21518#30340#25552#31034','#22914#26524#19981#38656#35201#25552#31034#21487#20197#19981#22635#20889'.'
            ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
            MaxLength = 100
            ParentShowHint = False
            ShowHint = True
            TabOrder = 6
            Text = #22871#35013'1'#29983#25928','#29289#38450#25552#21319'15%!'
          end
          object EditGroupItemName: TEdit
            Left = 56
            Top = 376
            Width = 377
            Height = 20
            Hint = #29289#21697#20043#38388#20351#29992#20008#38548#24320
            ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
            ParentShowHint = False
            ShowHint = True
            TabOrder = 5
          end
          object EditGroupItemCount: TSpinEditEx
            Left = 168
            Top = 352
            Width = 49
            Height = 21
            MaxValue = 20
            MinValue = 1
            TabOrder = 3
            Value = 1
          end
          object EditGroupItemDesc: TEdit
            Left = 280
            Top = 352
            Width = 153
            Height = 20
            ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
            TabOrder = 4
          end
          object ButtonGroupItemAdd: TButton
            Left = 488
            Top = 389
            Width = 57
            Height = 25
            Caption = #28155#21152'(&A)'
            TabOrder = 7
            OnClick = ButtonGroupItemAddClick
          end
          object ButtonGroupItemDel: TButton
            Left = 544
            Top = 389
            Width = 57
            Height = 25
            Caption = #21024#38500'(&D)'
            Enabled = False
            TabOrder = 8
            OnClick = ButtonGroupItemDelClick
          end
          object ButtonGroupItemChg: TButton
            Left = 600
            Top = 389
            Width = 57
            Height = 25
            Caption = #20462#25913'(&M)'
            Enabled = False
            TabOrder = 9
            OnClick = ButtonGroupItemChgClick
          end
          object ButtonGroupItemSave: TButton
            Left = 656
            Top = 389
            Width = 57
            Height = 25
            Caption = #20445#23384'(&S)'
            Enabled = False
            TabOrder = 10
            OnClick = ButtonGroupItemSaveClick
          end
        end
        object TabSheet14: TTabSheet
          Caption = #22871#35013#22791#27880
          ImageIndex = 1
          object MemoTzItemDesc: TMemo
            Left = 0
            Top = 0
            Width = 780
            Height = 385
            Align = alTop
            ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
            ScrollBars = ssBoth
            TabOrder = 0
            OnChange = MemoTzItemDescChange
          end
          object ButtonTzItemDescSave: TButton
            Left = 707
            Top = 392
            Width = 75
            Height = 25
            Caption = #20445#23384'(&S)'
            TabOrder = 1
            OnClick = ButtonTzItemDescSaveClick
          end
          object chkSendTzItemDescList: TCheckBox
            Left = 571
            Top = 397
            Width = 121
            Height = 17
            Hint = 
              #27492#34920#25968#25454#36807#22810#26377#21487#33021#23548#33268#23458#25143#31471#30331#24405#26102#33719#21462#25968#25454#32531#24930#65292#35831#30830#35748#26159#21542#21457#36865#12304#25512#33616#20351#29992#30331#24405#22120#38598#25104#27492#34920#12305#13#10#20351#29992#30331#24405#22120#38598#25104#21015#34920#65292#35831#19981#35201#21246#36873#65292#21542#21017 +
              #20197#24341#25806#20026#20934
            Caption = #26159#21542#21457#36865#21040#23458#25143#31471
            ParentShowHint = False
            ShowHint = True
            TabOrder = 2
            OnClick = chkSendTzItemDescListClick
          end
          object chkSingleHint: TCheckBox
            Left = 472
            Top = 397
            Width = 81
            Height = 17
            Caption = #21333#21015#26174#31034
            TabOrder = 3
            OnClick = chkSingleHintClick
          end
        end
      end
      object chkTZSupportRenameItem: TCheckBox
        Left = 152
        Top = 2
        Width = 217
        Height = 15
        Caption = #25903#25345#25913#21517#21518#30340#35013#22791#21517#23383#35302#21457#22871#35013#23646#24615
        TabOrder = 1
        OnClick = chkTZSupportRenameItemClick
      end
    end
    object TabSheet8: TTabSheet
      Caption = 'WIL'#36164#28304
      ImageIndex = 7
      object Label27: TLabel
        Left = 216
        Top = 80
        Width = 54
        Height = 12
        Caption = #36164#28304#21517#31216':'
      end
      object Label44: TLabel
        Left = 224
        Top = 160
        Width = 456
        Height = 36
        Caption = 
          #35828#26126#65306#13#25152#26377#25805#20316'WIL'#30340#21629#20196#21644#29289#21697#29305#25928#20013#30340#29289#21697#37117#20250#26681#25454'WIL'#36164#28304#21015#34920#30340#39034#24207#26469#35843#29992#65292#13#27604#22914'PlayEffect,ScreenEff' +
          'ect,MapEffect,OpenMerchantBigDlg,SETITEMEFFECT'#31561#31561#12290
        Font.Charset = GB2312_CHARSET
        Font.Color = clRed
        Font.Height = -12
        Font.Name = #23435#20307
        Font.Style = []
        ParentFont = False
      end
      object LabelFileIndex: TLabel
        Left = 408
        Top = 80
        Width = 6
        Height = 12
      end
      object GroupBox3: TGroupBox
        Left = 8
        Top = 4
        Width = 201
        Height = 430
        Caption = 'WIL'#36164#28304#21015#34920
        TabOrder = 0
        object ListBoxWilNameList: TListBox
          Left = 8
          Top = 16
          Width = 185
          Height = 401
          ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
          ItemHeight = 12
          TabOrder = 0
          OnClick = ListBoxWilNameListClick
        end
      end
      object btnWilNameUP: TButton
        Left = 216
        Top = 16
        Width = 33
        Height = 25
        Caption = #8593
        Enabled = False
        TabOrder = 1
        OnClick = btnWilNameUPClick
      end
      object btnWilNameDown: TButton
        Left = 216
        Top = 40
        Width = 33
        Height = 25
        Caption = #8595
        Enabled = False
        TabOrder = 2
        OnClick = btnWilNameDownClick
      end
      object EditWilName: TEdit
        Left = 272
        Top = 76
        Width = 121
        Height = 20
        ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
        TabOrder = 3
        Text = 'Prguse.wil'
      end
      object btnWilAdd: TButton
        Left = 272
        Top = 112
        Width = 75
        Height = 25
        Caption = #22686#21152'(&A)'
        TabOrder = 4
        OnClick = btnWilAddClick
      end
      object btnWilDel: TButton
        Left = 432
        Top = 112
        Width = 75
        Height = 25
        Caption = #21024#38500'(&D)'
        Enabled = False
        TabOrder = 5
        OnClick = btnWilDelClick
      end
      object btnWilSave: TButton
        Left = 512
        Top = 112
        Width = 75
        Height = 25
        Caption = #20445#23384'(&S)'
        Enabled = False
        TabOrder = 6
        OnClick = btnWilSaveClick
      end
      object ButtonSendEffectImageList: TButton
        Left = 592
        Top = 112
        Width = 129
        Height = 25
        Caption = #26356#26032#21040#23458#25143#31471'(&R)'
        TabOrder = 7
        OnClick = ButtonSendEffectImageListClick
      end
      object btnWilEdit: TButton
        Left = 352
        Top = 112
        Width = 75
        Height = 25
        Caption = #20462#25913
        Enabled = False
        TabOrder = 8
        OnClick = btnWilEditClick
      end
    end
    object TabSheet9: TTabSheet
      Caption = #29305#25928#21015#34920
      ImageIndex = 8
      object Labellbl1: TLabel
        Left = 619
        Top = 408
        Width = 126
        Height = 24
        Caption = #36164#28304#32534#21495#26159#25353#29031'WIL'#36164#28304#13#21015#34920#20013#30340#39034#24207#23545#24212#30340#12290
        Font.Charset = GB2312_CHARSET
        Font.Color = clRed
        Font.Height = -12
        Font.Name = #23435#20307
        Font.Style = []
        ParentFont = False
      end
      object Label48: TLabel
        Left = 619
        Top = 16
        Width = 168
        Height = 108
        Caption = 
          #29305#25928#32534#21495#35828#26126#13#10#13#10#13#20351#29992#33050#26412#21629#20196':'#13#10'SETITEMEFFECT '#32534#21495'(0-65535)'#13#10#13#10#13#26469#20462#25913#29289#21697#29305#25928#65292#20063#21487#20197#22312#29289#21697 +
          #13#29305#25928#20013#30452#25509#32534#36753#29289#21697#29305#25928
        Font.Charset = GB2312_CHARSET
        Font.Color = clRed
        Font.Height = -12
        Font.Name = #23435#20307
        Font.Style = []
        ParentFont = False
      end
      object Label137: TLabel
        Left = 149
        Top = 386
        Width = 30
        Height = 12
        Caption = #22791#27880':'
      end
      object Label46: TLabel
        Left = 149
        Top = 12
        Width = 54
        Height = 12
        Caption = #29305#25928#32534#21495':'
      end
      object Bevel1: TBevel
        Left = 608
        Top = 0
        Width = 2
        Height = 448
        Shape = bsLeftLine
      end
      object GroupBox15: TGroupBox
        Left = 8
        Top = 8
        Width = 129
        Height = 433
        Caption = #29305#25928#21015#34920
        TabOrder = 0
        object lstEffectList: TListBox
          Left = 8
          Top = 16
          Width = 113
          Height = 409
          ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ItemHeight = 12
          ParentShowHint = False
          ShowHint = True
          TabOrder = 0
          OnClick = lstEffectListClick
        end
      end
      object ButtonEffectAdd: TButton
        Left = 346
        Top = 409
        Width = 56
        Height = 25
        Caption = #22686#21152'(&A)'
        TabOrder = 8
        OnClick = ButtonEffectAddClick
      end
      object ButtonEffectChg: TButton
        Left = 410
        Top = 409
        Width = 56
        Height = 25
        Caption = #20462#25913'(&C)'
        TabOrder = 9
        OnClick = ButtonEffectChgClick
      end
      object ButtonEffectDel: TButton
        Left = 474
        Top = 409
        Width = 57
        Height = 25
        Caption = #21024#38500'(&D)'
        TabOrder = 10
        OnClick = ButtonEffectDelClick
      end
      object ButtonEffectSave: TButton
        Left = 538
        Top = 409
        Width = 56
        Height = 25
        Caption = #20445#23384'(&S)'
        TabOrder = 11
        OnClick = ButtonEffectSaveClick
      end
      object GroupBox13: TGroupBox
        Left = 149
        Top = 32
        Width = 445
        Height = 68
        Caption = #29289#21697#20869#35266#29305#25928
        TabOrder = 2
        object lbl2: TLabel
          Left = 8
          Top = 44
          Width = 54
          Height = 12
          Caption = #24320#22987#22270#29255':'
        end
        object lbl3: TLabel
          Left = 172
          Top = 44
          Width = 30
          Height = 12
          Caption = #25968#37327':'
        end
        object lbl4: TLabel
          Left = 8
          Top = 20
          Width = 54
          Height = 12
          Caption = #36164#28304#32534#21495':'
        end
        object Label62: TLabel
          Left = 190
          Top = 20
          Width = 12
          Height = 12
          Caption = 'X:'
        end
        object Label63: TLabel
          Left = 285
          Top = 20
          Width = 12
          Height = 12
          Caption = 'Y:'
        end
        object Label34: TLabel
          Left = 267
          Top = 44
          Width = 30
          Height = 12
          Caption = #36895#24230':'
        end
        object lbl33: TLabel
          Left = 376
          Top = 0
          Width = 48
          Height = 12
          Cursor = crHandPoint
          Caption = #20851#38381#29305#25928
          Font.Charset = GB2312_CHARSET
          Font.Color = clBlue
          Font.Height = -12
          Font.Name = #23435#20307
          Font.Style = []
          ParentFont = False
          OnClick = lbl33Click
          OnMouseEnter = lbl33MouseEnter
          OnMouseLeave = lbl33MouseLeave
        end
        object seItemEffectOffset1: TSpinEditEx
          Left = 62
          Top = 40
          Width = 100
          Height = 21
          MaxValue = 65535
          MinValue = 0
          TabOrder = 3
          Value = 0
        end
        object seItemEffectImageCount1: TSpinEditEx
          Left = 202
          Top = 39
          Width = 60
          Height = 21
          MaxValue = 255
          MinValue = 1
          TabOrder = 4
          Value = 1
        end
        object cbbEffectFileIndex1: TComboBox
          Left = 62
          Top = 16
          Width = 100
          Height = 20
          Style = csDropDownList
          ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          TabOrder = 0
        end
        object seItemEffectOffsetX1: TSpinEditEx
          Left = 202
          Top = 16
          Width = 60
          Height = 21
          Hint = #22352#26631'X'
          MaxValue = 32767
          MinValue = -32768
          ParentShowHint = False
          ShowHint = True
          TabOrder = 1
          Value = 0
        end
        object seItemEffectOffsetY1: TSpinEditEx
          Left = 297
          Top = 15
          Width = 60
          Height = 21
          Hint = #22352#26631'Y'
          MaxValue = 32767
          MinValue = -32768
          ParentShowHint = False
          ShowHint = True
          TabOrder = 2
          Value = 0
        end
        object seItemEffectTime1: TSpinEditEx
          Left = 297
          Top = 40
          Width = 60
          Height = 21
          Hint = #25773#25918#36895#24230
          MaxValue = 65535
          MinValue = 1
          ParentShowHint = False
          ShowHint = True
          TabOrder = 5
          Value = 200
        end
        object chkDrawCenter1: TCheckBox
          Left = 364
          Top = 30
          Width = 68
          Height = 17
          Hint = #19981#35835#21462#36164#28304#20559#31227#22352#26631#65292#33258#21160#19982#29289#21697#23621#20013#23545#40784
          Caption = #23621#20013#27169#24335
          ParentShowHint = False
          ShowHint = True
          TabOrder = 7
        end
        object chkNoBlendMode1: TCheckBox
          Left = 364
          Top = 13
          Width = 69
          Height = 17
          Hint = #21246#36873#21017#26159#26222#36890#32472#21046#26041#24335#65292#19981#21246#20026#29305#25928#32472#21046#26041#24335
          Caption = #26222#36890#32472#21046
          ParentShowHint = False
          ShowHint = True
          TabOrder = 6
        end
        object chkEfectBelowItem1: TCheckBox
          Left = 364
          Top = 47
          Width = 67
          Height = 17
          Hint = #20165#25903#25345#34915#26381#65292#27494#22120#65292#30462#29260#65292#21195#31456
          Caption = #24213#23618#25773#25918
          ParentShowHint = False
          ShowHint = True
          TabOrder = 8
        end
      end
      object GroupBox14: TGroupBox
        Left = 149
        Top = 104
        Width = 445
        Height = 44
        Caption = #29289#21697#22806#35266#29305#25928
        TabOrder = 3
        object lbl5: TLabel
          Left = 246
          Top = 22
          Width = 54
          Height = 12
          Caption = #24320#22987#22270#29255':'
        end
        object lbl6: TLabel
          Left = 97
          Top = 0
          Width = 180
          Height = 12
          Caption = #20165#34915#26381#12289#27494#22120#21644#30462#29260#25903#25345#22806#35266#29305#25928
          Font.Charset = GB2312_CHARSET
          Font.Color = clRed
          Font.Height = -12
          Font.Name = #23435#20307
          Font.Style = []
          ParentFont = False
        end
        object lbl7: TLabel
          Left = 8
          Top = 22
          Width = 54
          Height = 12
          Caption = #36164#28304#32534#21495':'
        end
        object Label85: TLabel
          Left = 376
          Top = 0
          Width = 48
          Height = 12
          Cursor = crHandPoint
          Caption = #20851#38381#29305#25928
          Font.Charset = GB2312_CHARSET
          Font.Color = clBlue
          Font.Height = -12
          Font.Name = #23435#20307
          Font.Style = []
          ParentFont = False
          OnClick = Label85Click
          OnMouseEnter = lbl33MouseEnter
          OnMouseLeave = lbl33MouseLeave
        end
        object seItemEffectOffset2: TSpinEditEx
          Left = 300
          Top = 17
          Width = 58
          Height = 21
          MaxValue = 65535
          MinValue = 0
          TabOrder = 2
          Value = 0
        end
        object cbbEffectFileIndex2: TComboBox
          Left = 62
          Top = 18
          Width = 100
          Height = 20
          Style = csDropDownList
          ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          TabOrder = 0
        end
        object chkNoBlendMode2: TCheckBox
          Left = 167
          Top = 19
          Width = 70
          Height = 17
          Hint = #21246#36873#21017#26159#26222#36890#32472#21046#26041#24335#65292#19981#21246#20026#29305#25928#32472#21046#26041#24335
          Caption = #26222#36890#32472#21046
          ParentShowHint = False
          ShowHint = True
          TabOrder = 1
        end
        object chkNoSex2: TCheckBox
          Left = 365
          Top = 19
          Width = 70
          Height = 17
          Hint = 
            #19981#21246#36873#38656#35201'1200'#24352#22270#29255#32032#26448#12289#30007#30340#20026#21069'600'#24352#12289#22899#30340#20026#21518'600'#24352#13#10#21246#36873#21518#21482#38656#35201'600'#24352#22270#29255#32032#26448#65292#30007#22899#35835#21462#19968#26679#22343#20026'0-599'#12289#20943 +
            #23569#34917#19969#22823#23567
          Caption = #19981#20998#30007#22899
          ParentShowHint = False
          ShowHint = True
          TabOrder = 3
        end
      end
      object GroupBox25: TGroupBox
        Left = 149
        Top = 237
        Width = 445
        Height = 66
        Caption = #29289#21697#22312#21253#35065#20013#30340#29305#25928
        TabOrder = 5
        object lbl8: TLabel
          Left = 8
          Top = 43
          Width = 54
          Height = 12
          Caption = #24320#22987#22270#29255':'
        end
        object lbl10: TLabel
          Left = 172
          Top = 43
          Width = 30
          Height = 12
          Caption = #25968#37327':'
        end
        object lbl12: TLabel
          Left = 8
          Top = 20
          Width = 54
          Height = 12
          Caption = #36164#28304#32534#21495':'
        end
        object Label64: TLabel
          Left = 190
          Top = 19
          Width = 12
          Height = 12
          Caption = 'X:'
        end
        object Label65: TLabel
          Left = 285
          Top = 19
          Width = 12
          Height = 12
          Caption = 'Y:'
        end
        object Label35: TLabel
          Left = 267
          Top = 43
          Width = 30
          Height = 12
          Caption = #36895#24230':'
        end
        object Label87: TLabel
          Left = 376
          Top = 0
          Width = 48
          Height = 12
          Cursor = crHandPoint
          Caption = #20851#38381#29305#25928
          Font.Charset = GB2312_CHARSET
          Font.Color = clBlue
          Font.Height = -12
          Font.Name = #23435#20307
          Font.Style = []
          ParentFont = False
          OnClick = Label87Click
          OnMouseEnter = lbl33MouseEnter
          OnMouseLeave = lbl33MouseLeave
        end
        object seItemEffectOffset3: TSpinEditEx
          Left = 62
          Top = 39
          Width = 100
          Height = 21
          MaxValue = 65535
          MinValue = 0
          TabOrder = 3
          Value = 0
        end
        object seItemEffectImageCount3: TSpinEditEx
          Left = 202
          Top = 39
          Width = 60
          Height = 21
          MaxValue = 255
          MinValue = 1
          TabOrder = 4
          Value = 1
        end
        object cbbEffectFileIndex3: TComboBox
          Left = 62
          Top = 16
          Width = 100
          Height = 20
          Style = csDropDownList
          ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          TabOrder = 0
        end
        object seItemEffectOffsetX3: TSpinEditEx
          Left = 202
          Top = 15
          Width = 60
          Height = 21
          Hint = #22352#26631'X'
          MaxValue = 32767
          MinValue = -32768
          ParentShowHint = False
          ShowHint = True
          TabOrder = 1
          Value = 0
        end
        object seItemEffectOffsetY3: TSpinEditEx
          Left = 297
          Top = 15
          Width = 60
          Height = 21
          Hint = #22352#26631'Y'
          MaxValue = 32767
          MinValue = -32768
          ParentShowHint = False
          ShowHint = True
          TabOrder = 2
          Value = 0
        end
        object seItemEffectTime3: TSpinEditEx
          Left = 297
          Top = 39
          Width = 60
          Height = 21
          Hint = #22352#26631'Y'
          MaxValue = 65535
          MinValue = 1
          ParentShowHint = False
          ShowHint = True
          TabOrder = 5
          Value = 200
        end
        object chkDrawCenter3: TCheckBox
          Left = 364
          Top = 41
          Width = 68
          Height = 17
          Hint = #19981#35835#21462#36164#28304#20559#31227#22352#26631#65292#33258#21160#19982#29289#21697#23621#20013#23545#40784
          Caption = #23621#20013#27169#24335
          ParentShowHint = False
          ShowHint = True
          TabOrder = 7
        end
        object chkNoBlendMode3: TCheckBox
          Left = 364
          Top = 17
          Width = 69
          Height = 17
          Hint = #21246#36873#21017#26159#26222#36890#32472#21046#26041#24335#65292#19981#21246#20026#29305#25928#32472#21046#26041#24335
          Caption = #26222#36890#32472#21046
          ParentShowHint = False
          ShowHint = True
          TabOrder = 6
        end
      end
      object EditEffectDesc: TEdit
        Left = 181
        Top = 382
        Width = 412
        Height = 20
        ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
        MaxLength = 80
        TabOrder = 7
      end
      object EditEffectIndex: TSpinEditEx
        Left = 207
        Top = 8
        Width = 65
        Height = 21
        MaxValue = 65535
        MinValue = 1
        TabOrder = 1
        Value = 1
      end
      object GroupBox27: TGroupBox
        Left = 149
        Top = 151
        Width = 445
        Height = 82
        Caption = #12304#38468#21152#12305#29289#21697#22806#35266#29305#25928
        TabOrder = 4
        object Label30: TLabel
          Left = 8
          Top = 43
          Width = 54
          Height = 12
          Caption = #24320#22987#22270#29255':'
        end
        object Label81: TLabel
          Left = 8
          Top = 21
          Width = 54
          Height = 12
          Caption = #36164#28304#32534#21495':'
        end
        object Label66: TLabel
          Left = 172
          Top = 43
          Width = 30
          Height = 12
          Caption = #25968#37327':'
        end
        object Label82: TLabel
          Left = 267
          Top = 43
          Width = 30
          Height = 12
          Caption = #36895#24230':'
        end
        object lbl26: TLabel
          Left = 172
          Top = 20
          Width = 30
          Height = 12
          Caption = #39034#24207':'
        end
        object Label83: TLabel
          Left = 8
          Top = 63
          Width = 312
          Height = 12
          Caption = #20165#34915#26381#25903#25345#65292#38468#21152#29305#25928#22266#23450#25773#25918#22810#24352#22270#29255#65292#19982#20154#29289#21160#20316#26080#20851
          Font.Charset = GB2312_CHARSET
          Font.Color = clRed
          Font.Height = -12
          Font.Name = #23435#20307
          Font.Style = []
          ParentFont = False
        end
        object Label86: TLabel
          Left = 376
          Top = 0
          Width = 48
          Height = 12
          Cursor = crHandPoint
          Caption = #20851#38381#29305#25928
          Font.Charset = GB2312_CHARSET
          Font.Color = clBlue
          Font.Height = -12
          Font.Name = #23435#20307
          Font.Style = []
          ParentFont = False
          OnClick = Label86Click
          OnMouseEnter = lbl33MouseEnter
          OnMouseLeave = lbl33MouseLeave
        end
        object seAddEffectOffset: TSpinEditEx
          Left = 62
          Top = 39
          Width = 100
          Height = 21
          MaxValue = 65535
          MinValue = 0
          TabOrder = 2
          Value = 0
        end
        object cbbAddEffectFileIndex: TComboBox
          Left = 62
          Top = 17
          Width = 100
          Height = 20
          Style = csDropDownList
          ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          TabOrder = 0
        end
        object seAddEffectPlayCount: TSpinEditEx
          Left = 202
          Top = 38
          Width = 60
          Height = 21
          MaxValue = 65535
          MinValue = 0
          TabOrder = 3
          Value = 0
        end
        object seAddEffectPlayTime: TSpinEditEx
          Left = 297
          Top = 38
          Width = 60
          Height = 21
          MaxValue = 65535
          MinValue = 1
          TabOrder = 4
          Value = 1
        end
        object chkAddEffectNoBlend: TCheckBox
          Left = 364
          Top = 17
          Width = 69
          Height = 17
          Hint = #21246#36873#21017#26159#26222#36890#32472#21046#26041#24335#65292#19981#21246#20026#29305#25928#32472#21046#26041#24335
          Caption = #26222#36890#32472#21046
          ParentShowHint = False
          ShowHint = True
          TabOrder = 5
        end
        object cbbAddEffectDrawOrder: TComboBox
          Left = 202
          Top = 16
          Width = 153
          Height = 20
          Style = csDropDownList
          TabOrder = 1
          Items.Strings = (
            #22312#34915#26381#19978#23618#25773#25918
            #22312#34915#26381#19979#23618#25773#25918)
        end
        object chkAddEffectDrawCenter: TCheckBox
          Left = 364
          Top = 40
          Width = 68
          Height = 17
          Hint = #19981#35835#21462#36164#28304#20559#31227#22352#26631#65292#33258#21160#19982#29289#21697#23621#20013#23545#40784
          Caption = #23621#20013#27169#24335
          ParentShowHint = False
          ShowHint = True
          TabOrder = 6
        end
      end
      object GroupBox28: TGroupBox
        Left = 149
        Top = 308
        Width = 445
        Height = 66
        Caption = #29289#21697#22312#22320#19978#29305#25928
        TabOrder = 6
        object Label88: TLabel
          Left = 8
          Top = 43
          Width = 54
          Height = 12
          Caption = #24320#22987#22270#29255':'
        end
        object Label89: TLabel
          Left = 172
          Top = 43
          Width = 30
          Height = 12
          Caption = #25968#37327':'
        end
        object Label90: TLabel
          Left = 8
          Top = 20
          Width = 54
          Height = 12
          Caption = #36164#28304#32534#21495':'
        end
        object Label91: TLabel
          Left = 190
          Top = 19
          Width = 12
          Height = 12
          Caption = 'X:'
        end
        object Label92: TLabel
          Left = 285
          Top = 19
          Width = 12
          Height = 12
          Caption = 'Y:'
        end
        object Label93: TLabel
          Left = 267
          Top = 43
          Width = 30
          Height = 12
          Caption = #36895#24230':'
        end
        object Label94: TLabel
          Left = 376
          Top = 0
          Width = 48
          Height = 12
          Cursor = crHandPoint
          Caption = #20851#38381#29305#25928
          Font.Charset = GB2312_CHARSET
          Font.Color = clBlue
          Font.Height = -12
          Font.Name = #23435#20307
          Font.Style = []
          ParentFont = False
          OnClick = Label94Click
          OnMouseEnter = lbl33MouseEnter
          OnMouseLeave = lbl33MouseLeave
        end
        object seItemEffectOffset5: TSpinEditEx
          Left = 62
          Top = 39
          Width = 100
          Height = 21
          MaxValue = 65535
          MinValue = 0
          TabOrder = 3
          Value = 0
        end
        object seItemEffectImageCount5: TSpinEditEx
          Left = 202
          Top = 39
          Width = 60
          Height = 21
          MaxValue = 255
          MinValue = 1
          TabOrder = 4
          Value = 1
        end
        object cbbEffectFileIndex5: TComboBox
          Left = 62
          Top = 16
          Width = 100
          Height = 20
          Style = csDropDownList
          ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          TabOrder = 0
        end
        object seItemEffectOffsetX5: TSpinEditEx
          Left = 202
          Top = 15
          Width = 60
          Height = 21
          Hint = #22352#26631'X'
          MaxValue = 32767
          MinValue = -32768
          ParentShowHint = False
          ShowHint = True
          TabOrder = 1
          Value = 0
        end
        object seItemEffectOffsetY5: TSpinEditEx
          Left = 297
          Top = 15
          Width = 60
          Height = 21
          Hint = #22352#26631'Y'
          MaxValue = 32767
          MinValue = -32768
          ParentShowHint = False
          ShowHint = True
          TabOrder = 2
          Value = 0
        end
        object seItemEffectTime5: TSpinEditEx
          Left = 297
          Top = 39
          Width = 60
          Height = 21
          Hint = #22352#26631'Y'
          MaxValue = 65535
          MinValue = 1
          ParentShowHint = False
          ShowHint = True
          TabOrder = 5
          Value = 200
        end
        object chkDrawCenter5: TCheckBox
          Left = 364
          Top = 29
          Width = 68
          Height = 17
          Hint = #19981#35835#21462#36164#28304#20559#31227#22352#26631#65292#33258#21160#19982#29289#21697#23621#20013#23545#40784
          Caption = #23621#20013#27169#24335
          ParentShowHint = False
          ShowHint = True
          TabOrder = 7
        end
        object chkNoBlendMode5: TCheckBox
          Left = 364
          Top = 13
          Width = 69
          Height = 17
          Hint = #21246#36873#21017#26159#26222#36890#32472#21046#26041#24335#65292#19981#21246#20026#29305#25928#32472#21046#26041#24335
          Caption = #26222#36890#32472#21046
          ParentShowHint = False
          ShowHint = True
          TabOrder = 6
        end
        object chkEfectBelowItem5: TCheckBox
          Left = 364
          Top = 46
          Width = 67
          Height = 17
          Caption = #24213#23618#25773#25918
          ParentShowHint = False
          ShowHint = True
          TabOrder = 8
        end
      end
    end
    object TabSheet7: TTabSheet
      Caption = #29289#21697#29305#25928
      ImageIndex = 6
      object Label45: TLabel
        Left = 288
        Top = 20
        Width = 54
        Height = 12
        Caption = #29305#25928#32534#21495':'
      end
      object lblEffectMemo: TLabel
        Left = 288
        Top = 72
        Width = 78
        Height = 12
        Caption = 'lblEffectMemo'
        Font.Charset = GB2312_CHARSET
        Font.Color = clRed
        Font.Height = -12
        Font.Name = #23435#20307
        Font.Style = []
        ParentFont = False
      end
      object Label47: TLabel
        Left = 288
        Top = 44
        Width = 54
        Height = 12
        Caption = #29289#21697#21517#31216':'
      end
      object Label49: TLabel
        Left = 496
        Top = 20
        Width = 168
        Height = 12
        Caption = #29305#25928#32534#21495#21487#20197#22312#29305#25928#21015#34920#20013#32534#36753
        Font.Charset = GB2312_CHARSET
        Font.Color = clRed
        Font.Height = -12
        Font.Name = #23435#20307
        Font.Style = []
        ParentFont = False
      end
      object lblEffectDesc: TLabel
        Left = 288
        Top = 144
        Width = 90
        Height = 12
        Caption = 'LabelEffectDesc'
        Font.Charset = GB2312_CHARSET
        Font.Color = 16744448
        Font.Height = -12
        Font.Name = #23435#20307
        Font.Style = []
        ParentFont = False
      end
      object GroupBox4: TGroupBox
        Left = 8
        Top = 4
        Width = 129
        Height = 430
        Caption = #29289#21697#21015#34920
        TabOrder = 0
        object ListBoxStdItemList1: TListBox
          Left = 8
          Top = 16
          Width = 113
          Height = 401
          Hint = 'Ctrl + F '#29289#21697#26597#25214#65292#21491#38190#21487#20197#36827#34892#31579#36873#12290
          ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ItemHeight = 12
          ParentShowHint = False
          PopupMenu = PopupMenuItem
          ShowHint = True
          TabOrder = 0
          OnClick = ListBoxStdItemList1Click
          OnKeyDown = ListBoxitemListKeyDown
        end
      end
      object GroupBox12: TGroupBox
        Left = 144
        Top = 4
        Width = 129
        Height = 430
        Caption = #29305#25928#29289#21697#21015#34920
        TabOrder = 1
        object ListBoxEffectItemList: TListBox
          Left = 8
          Top = 16
          Width = 113
          Height = 401
          Hint = 'Ctrl + F '#29289#21697#26597#25214#65292#21491#38190#21487#20197#36827#34892#31579#36873#12290
          ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ItemHeight = 12
          ParentShowHint = False
          ShowHint = True
          TabOrder = 0
          OnClick = ListBoxEffectItemListClick
          OnKeyDown = ListBoxitemListKeyDown
        end
      end
      object ComboBoxEffectIndex: TComboBox
        Left = 344
        Top = 16
        Width = 145
        Height = 20
        Style = csDropDownList
        ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
        TabOrder = 2
        OnChange = ComboBoxEffectIndexChange
      end
      object ButtonEffectItemAdd: TButton
        Left = 288
        Top = 400
        Width = 75
        Height = 25
        Caption = #22686#21152
        TabOrder = 4
        OnClick = ButtonEffectItemAddClick
      end
      object ButtonEffectItemChg: TButton
        Left = 368
        Top = 400
        Width = 75
        Height = 25
        Caption = #20462#25913
        TabOrder = 5
        OnClick = ButtonEffectItemChgClick
      end
      object ButtonEffectItemDel: TButton
        Left = 448
        Top = 400
        Width = 75
        Height = 25
        Caption = #21024#38500
        TabOrder = 6
        OnClick = ButtonEffectItemDelClick
      end
      object ButtonEffectItemSave: TButton
        Left = 528
        Top = 400
        Width = 75
        Height = 25
        Caption = #20445#23384
        TabOrder = 7
        OnClick = ButtonEffectItemSaveClick
      end
      object EditEffectItemName: TEdit
        Left = 344
        Top = 40
        Width = 145
        Height = 20
        ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
        TabOrder = 3
      end
    end
    object TabSheet10: TTabSheet
      Caption = #38136#36896#29289#21697
      ImageIndex = 9
      object Label50: TLabel
        Left = 464
        Top = 380
        Width = 54
        Height = 12
        Caption = #25104#21151#26426#29575':'
      end
      object Label52: TLabel
        Left = 464
        Top = 404
        Width = 54
        Height = 12
        Caption = #33719#24471#25968#37327':'
      end
      object GroupBox16: TGroupBox
        Left = 280
        Top = 4
        Width = 337
        Height = 356
        Caption = #25152#38656#29289#21697#21015#34920
        TabOrder = 1
        object Label53: TLabel
          Left = 208
          Top = 292
          Width = 30
          Height = 12
          Caption = #28040#22833':'
        end
        object Label54: TLabel
          Left = 8
          Top = 328
          Width = 54
          Height = 12
          Caption = #29289#21697#21517#31216':'
        end
        object Label56: TLabel
          Left = 206
          Top = 328
          Width = 30
          Height = 12
          Caption = #25968#37327':'
        end
        object ListViewFoundryNeedItemList: TListView
          Left = 7
          Top = 16
          Width = 322
          Height = 265
          Columns = <
            item
              Caption = #24207#21495
            end
            item
              Caption = #29289#21697#21517#31216
              Width = 80
            end
            item
              Caption = #25152#38656#25968#37327
              Width = 120
            end
            item
              Caption = #22833#36133#28040#22833
              Width = 60
            end>
          GridLines = True
          ReadOnly = True
          RowSelect = True
          TabOrder = 0
          ViewStyle = vsReport
          OnClick = ListViewFoundryNeedItemListClick
        end
        object ButtonFoundryNeedItemAdd: TButton
          Left = 8
          Top = 288
          Width = 57
          Height = 25
          Caption = #28155#21152
          TabOrder = 1
          OnClick = ButtonFoundryNeedItemAddClick
        end
        object ButtonFoundryNeedItemDel: TButton
          Left = 72
          Top = 288
          Width = 57
          Height = 25
          Caption = #21024#38500
          TabOrder = 2
          OnClick = ButtonFoundryNeedItemDelClick
        end
        object ButtonFoundryNeedItemChg: TButton
          Left = 136
          Top = 288
          Width = 57
          Height = 25
          Caption = #20462#25913
          TabOrder = 3
          OnClick = ButtonFoundryNeedItemChgClick
        end
        object EditFoundryNeedItemDel: TSpinEditEx
          Left = 240
          Top = 288
          Width = 90
          Height = 21
          Hint = #29289#21697#21512#25104#22833#36133#21518#26159#21542#28040#22833' 0='#19981#28040#22833' 1='#28040#22833' ('#29289#21697#21512#25104#25104#21151#21518#21516#26679#28040#22833')'
          MaxValue = 1
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 4
          Value = 0
        end
        object ComboBoxFoundryNeedItemName: TComboBox
          Left = 64
          Top = 324
          Width = 113
          Height = 20
          ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ParentShowHint = False
          ShowHint = True
          TabOrder = 5
          OnChange = ComboBoxFoundryNeedItemNameChange
        end
        object EditFoundryNeedItemCount: TSpinEditEx
          Left = 240
          Top = 324
          Width = 90
          Height = 21
          MaxValue = 2100000000
          MinValue = 1
          TabOrder = 6
          Value = 1
        end
      end
      object GroupBox17: TGroupBox
        Left = 8
        Top = 4
        Width = 265
        Height = 430
        Caption = #38136#36896#29289#21697#21015#34920
        TabOrder = 0
        object ListViewFoundryItemList: TListView
          Left = 7
          Top = 16
          Width = 250
          Height = 401
          Columns = <
            item
              Caption = #24207#21495
            end
            item
              Caption = #29289#21697#21517#31216
              Width = 80
            end
            item
              Caption = #38136#36896#25968#37327
              Width = 60
            end
            item
              Caption = #25104#21151#29575
            end>
          GridLines = True
          ReadOnly = True
          RowSelect = True
          TabOrder = 0
          ViewStyle = vsReport
          OnClick = ListViewFoundryItemListClick
        end
      end
      object GroupBox18: TGroupBox
        Left = 624
        Top = 4
        Width = 145
        Height = 430
        Caption = #29289#21697#21015#34920
        TabOrder = 2
        object ListBoxFoundryItemList: TListBox
          Left = 8
          Top = 16
          Width = 129
          Height = 401
          Hint = 'Ctrl + F '#29289#21697#26597#25214#65292#21491#38190#21487#20197#36827#34892#31579#36873#12290
          ImeMode = imChinese
          ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
          ItemHeight = 12
          MultiSelect = True
          ParentShowHint = False
          PopupMenu = PopupMenu5
          ShowHint = True
          TabOrder = 0
          OnClick = ListBoxFoundryItemListClick
          OnKeyDown = ListBoxitemListKeyDown
        end
      end
      object ButtonFoundryItemAdd: TButton
        Left = 280
        Top = 376
        Width = 57
        Height = 25
        Caption = #28155#21152'(&A)'
        TabOrder = 3
        OnClick = ButtonFoundryItemAddClick
      end
      object ButtonFoundryItemDel: TButton
        Left = 280
        Top = 408
        Width = 57
        Height = 25
        Caption = #21024#38500'(&D)'
        TabOrder = 7
        OnClick = ButtonFoundryItemDelClick
      end
      object ButtonFoundryItemChg: TButton
        Left = 344
        Top = 376
        Width = 57
        Height = 25
        Caption = #20462#25913'(&M)'
        TabOrder = 4
        OnClick = ButtonFoundryItemChgClick
      end
      object ButtonFoundryItemSave: TButton
        Left = 344
        Top = 408
        Width = 57
        Height = 25
        Caption = #20445#23384'(&S)'
        TabOrder = 8
        OnClick = ButtonFoundryItemSaveClick
      end
      object EditFoundryItemRate: TSpinEditEx
        Left = 520
        Top = 376
        Width = 90
        Height = 21
        MaxValue = 10000000
        MinValue = 1
        TabOrder = 5
        Value = 100
      end
      object EditFoundryGiveItemCount: TSpinEditEx
        Left = 520
        Top = 400
        Width = 90
        Height = 21
        MaxValue = 1000000
        MinValue = 1
        TabOrder = 6
        Value = 1
      end
    end
    object TabSheet11: TTabSheet
      Caption = #29289#21697#22791#27880
      ImageIndex = 10
      object PageControl1: TPageControl
        Left = 0
        Top = 0
        Width = 780
        Height = 444
        ActivePage = tsItemDescTop
        Align = alClient
        TabOrder = 0
        ExplicitWidth = 776
        ExplicitHeight = 443
        object tsItemDesc: TTabSheet
          Caption = #23614#34892#26174#31034
          object MemoItemDesc: TMemo
            Left = 0
            Top = 0
            Width = 772
            Height = 391
            Align = alTop
            ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
            Lines.Strings = (
              ';'#23453#34255#38053#21273'='#21487#29992#26469#24320#21551#21351#40857#23665#24196#20013#30340#8220#31070#31192#23453#34255#8221'\'#30452#25509#33719#24471#20854#20013#30340#23453#29289)
            ScrollBars = ssBoth
            TabOrder = 0
            OnChange = MemoItemDescChange
            ExplicitWidth = 768
          end
          object btnItemDescSave: TButton
            Left = 708
            Top = 394
            Width = 75
            Height = 25
            Caption = #20445#23384'(&S)'
            TabOrder = 1
            OnClick = btnItemDescSaveClick
          end
          object chkSendItemDescList: TCheckBox
            Left = 566
            Top = 398
            Width = 121
            Height = 17
            Hint = 
              #27492#34920#25968#25454#36807#22810#26377#21487#33021#23548#33268#23458#25143#31471#30331#24405#26102#33719#21462#25968#25454#32531#24930#65292#35831#30830#35748#26159#21542#21457#36865#12304#25512#33616#20351#29992#30331#24405#22120#38598#25104#27492#34920#12305#13#10#20351#29992#30331#24405#22120#38598#25104#21015#34920#65292#35831#19981#35201#21246#36873#65292#21542#21017 +
              #20197#24341#25806#20026#20934
            Caption = #26159#21542#21457#36865#21040#23458#25143#31471
            ParentShowHint = False
            ShowHint = True
            TabOrder = 2
            OnClick = chkSendItemDescListClick
          end
        end
        object tsItemDescTop: TTabSheet
          Caption = #39030#34892#26174#31034
          ImageIndex = 1
          object mmoItemDescTop: TMemo
            Left = 0
            Top = 0
            Width = 772
            Height = 391
            Align = alTop
            ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
            Lines.Strings = (
              ';'#23453#34255#38053#21273'='#21487#29992#26469#24320#21551#21351#40857#23665#24196#20013#30340#8220#31070#31192#23453#34255#8221'\'#30452#25509#33719#24471#20854#20013#30340#23453#29289)
            ScrollBars = ssBoth
            TabOrder = 0
            OnChange = mmoItemDescTopChange
            ExplicitWidth = 768
          end
          object chkSendItemDescTopList: TCheckBox
            Left = 566
            Top = 398
            Width = 121
            Height = 17
            Hint = 
              #27492#34920#25968#25454#36807#22810#26377#21487#33021#23548#33268#23458#25143#31471#30331#24405#26102#33719#21462#25968#25454#32531#24930#65292#35831#30830#35748#26159#21542#21457#36865#12304#25512#33616#20351#29992#30331#24405#22120#38598#25104#27492#34920#12305#13#10#20351#29992#30331#24405#22120#38598#25104#21015#34920#65292#35831#19981#35201#21246#36873#65292#21542#21017 +
              #20197#24341#25806#20026#20934
            Caption = #26159#21542#21457#36865#21040#23458#25143#31471
            ParentShowHint = False
            ShowHint = True
            TabOrder = 1
            OnClick = chkSendItemDescTopListClick
          end
          object btnItemDescTopSave: TButton
            Left = 708
            Top = 394
            Width = 75
            Height = 25
            Caption = #20445#23384'(&S)'
            TabOrder = 2
            OnClick = btnItemDescTopSaveClick
          end
        end
      end
      object chkDescSupportRenamItem: TCheckBox
        Left = 144
        Top = 3
        Width = 169
        Height = 14
        Hint = #35813#36873#39033#20063#20250#23545#30331#24405#22120#38598#25104#30340#8220#29289#21697#22791#27880#20449#24687#8221#26377#25928
        Caption = #25903#25345#25913#21517#21518#30340#35013#22791#21517#23383#35774#32622
        ParentShowHint = False
        ShowHint = True
        TabOrder = 1
        OnClick = chkDescSupportRenamItemClick
      end
      object chkNoRenameDescReadDefault: TCheckBox
        Left = 320
        Top = 3
        Width = 118
        Height = 14
        Hint = #35813#36873#39033#20063#20250#23545#30331#24405#22120#38598#25104#30340#8220#29289#21697#22791#27880#20449#24687#8221#26377#25928
        Caption = #26080#25913#21517#22791#27880#35835#40664#35748
        ParentShowHint = False
        ShowHint = True
        TabOrder = 2
        OnClick = chkNoRenameDescReadDefaultClick
      end
    end
    object TabSheet12: TTabSheet
      Caption = #20869#25346#25441#21462
      ImageIndex = 11
      DesignSize = (
        780
        444)
      object LabelItemName: TLabel
        Left = 0
        Top = 375
        Width = 54
        Height = 12
        Caption = #29289#21697#21517#31216':'
      end
      object LabelItemType: TLabel
        Left = 0
        Top = 397
        Width = 54
        Height = 12
        Caption = #29289#21697#31867#22411':'
      end
      object lbl30: TLabel
        Left = 8
        Top = 428
        Width = 162
        Height = 12
        Caption = 'Ctrl+'#24038#38190#22810#36873#65292#21491#38190#25209#37327#35774#32622
        Font.Charset = GB2312_CHARSET
        Font.Color = clRed
        Font.Height = -12
        Font.Name = #23435#20307
        Font.Style = []
        ParentFont = False
      end
      object Label97: TLabel
        Left = 321
        Top = 407
        Width = 48
        Height = 12
        Caption = #20998#32452#24207#21495
      end
      object ListViewFilterItem: TListView
        Left = 2
        Top = 1
        Width = 641
        Height = 361
        Columns = <
          item
            Caption = #29289#21697#31867#22411
            Width = 80
          end
          item
            Caption = #29289#21697#21517#31216
            Width = 100
          end
          item
            Caption = #26497#21697#25552#31034
            Width = 70
          end
          item
            Caption = #33258#21160#25441#21462
            Width = 70
          end
          item
            Caption = #26174#31034#21517#31216
            Width = 70
          end
          item
            Caption = #29305#27530
            Width = 60
          end
          item
            Caption = #20256#36865#25441#36215
            Width = 70
          end
          item
            Caption = #20998#32452#24207#21495
            Width = 80
          end>
        GridLines = True
        MultiSelect = True
        ReadOnly = True
        RowSelect = True
        PopupMenu = pmFilterItem
        SortType = stBoth
        TabOrder = 0
        ViewStyle = vsReport
        OnClick = ListViewFilterItemClick
      end
      object GroupBox20: TGroupBox
        Left = 635
        Top = 0
        Width = 145
        Height = 444
        Align = alRight
        Caption = #29289#21697#21015#34920
        TabOrder = 1
        ExplicitLeft = 631
        ExplicitHeight = 443
        object ListBoxitemList4: TListBox
          Left = 8
          Top = 16
          Width = 129
          Height = 425
          Hint = 'Ctrl + F '#29289#21697#26597#25214#65292#21491#38190#21487#20197#36827#34892#31579#36873#12290#13#21487#20197#22810#36873
          ImeMode = imChinese
          ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
          ItemHeight = 12
          MultiSelect = True
          ParentShowHint = False
          PopupMenu = PopupMenu6
          ShowHint = True
          TabOrder = 0
          OnClick = ListBoxItemList2Click
          OnKeyDown = ListBoxitemListKeyDown
        end
      end
      object ComboBoxItemFilter: TComboBox
        Left = 56
        Top = 394
        Width = 121
        Height = 20
        Style = csDropDownList
        ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
        ItemIndex = 0
        TabOrder = 8
        Text = '('#20840#37096#20998#31867')'
        OnChange = ComboBoxItemFilterChange
        Items.Strings = (
          '('#20840#37096#20998#31867')'
          #20854#20182#31867
          #33647#21697#31867
          #26381#35013#31867
          #27494#22120#31867
          #39318#39280#31867
          #39280#21697#31867
          #35013#39280#31867)
      end
      object EditFilterItemName: TEdit
        Left = 56
        Top = 371
        Width = 121
        Height = 20
        ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
        ReadOnly = True
        TabOrder = 5
      end
      object ButtonFilterAdd: TButton
        Left = 509
        Top = 366
        Width = 43
        Height = 23
        Anchors = [akTop, akRight]
        Caption = #22686#21152
        TabOrder = 3
        OnClick = ButtonFilterAddClick
        ExplicitLeft = 537
      end
      object ButtonFilterDel: TButton
        Left = 552
        Top = 366
        Width = 43
        Height = 23
        Anchors = [akTop, akRight]
        Caption = #21024#38500
        Enabled = False
        TabOrder = 4
        OnClick = ButtonFilterDelClick
        ExplicitLeft = 580
      end
      object GroupBox19: TGroupBox
        Left = 136
        Top = 366
        Width = 369
        Height = 35
        Anchors = [akTop, akRight]
        TabOrder = 2
        ExplicitLeft = 164
        object CheckBoxHintItem: TCheckBox
          Left = 5
          Top = 10
          Width = 72
          Height = 17
          Caption = #26497#21697#25552#31034
          Checked = True
          State = cbChecked
          TabOrder = 0
        end
        object CheckBoxPickUpItem: TCheckBox
          Left = 78
          Top = 10
          Width = 72
          Height = 17
          Caption = #33258#21160#25441#21462
          Checked = True
          State = cbChecked
          TabOrder = 1
        end
        object CheckBoxShowItemName: TCheckBox
          Left = 150
          Top = 10
          Width = 72
          Height = 17
          Caption = #26174#31034#21517#31216
          Checked = True
          State = cbChecked
          TabOrder = 2
        end
        object chkShowSpecial: TCheckBox
          Left = 222
          Top = 11
          Width = 72
          Height = 17
          Caption = #29305#27530#29289#21697
          Checked = True
          State = cbChecked
          TabOrder = 3
        end
        object chkAutoMove: TCheckBox
          Left = 295
          Top = 11
          Width = 72
          Height = 17
          Caption = #33258#21160#20256#36865
          Checked = True
          State = cbChecked
          TabOrder = 4
        end
      end
      object ButtonFilterChg: TButton
        Left = 509
        Top = 390
        Width = 43
        Height = 23
        Anchors = [akTop, akRight]
        Caption = #20462#25913
        TabOrder = 6
        OnClick = ButtonFilterChgClick
        ExplicitLeft = 537
      end
      object ButtonFilterSave: TButton
        Left = 552
        Top = 390
        Width = 43
        Height = 23
        Anchors = [akTop, akRight]
        Caption = #20445#23384
        Enabled = False
        TabOrder = 7
        OnClick = ButtonFilterSaveClick
        ExplicitLeft = 580
      end
      object ButtonFilterAddAll: TButton
        Left = 488
        Top = 416
        Width = 75
        Height = 25
        Caption = #20840#37096#22686#21152
        TabOrder = 9
        OnClick = ButtonFilterAddAllClick
      end
      object ButtonFilterDelAll: TButton
        Left = 568
        Top = 416
        Width = 75
        Height = 25
        Caption = #20840#37096#21024#38500
        TabOrder = 10
        OnClick = ButtonFilterDelAllClick
      end
      object chkSendFilterItemList: TCheckBox
        Left = 183
        Top = 404
        Width = 121
        Height = 17
        Hint = 
          #27492#34920#25968#25454#36807#22810#26377#21487#33021#23548#33268#23458#25143#31471#30331#24405#26102#33719#21462#25968#25454#32531#24930#65292#35831#30830#35748#26159#21542#21457#36865#12304#25512#33616#20351#29992#30331#24405#22120#38598#25104#27492#34920#12305#13#10#20351#29992#30331#24405#22120#38598#25104#21015#34920#65292#35831#19981#35201#21246#36873#65292#21542#21017 +
          #20197#24341#25806#20026#20934
        Caption = #26159#21542#21457#36865#21040#23458#25143#31471
        ParentShowHint = False
        ShowHint = True
        TabOrder = 11
        OnClick = chkSendFilterItemListClick
      end
      object chkEnableHeroUseClientPickItems: TCheckBox
        Left = 334
        Top = 427
        Width = 140
        Height = 17
        Hint = #23458#25143#31471#20869#25346#29289#21697#20013#8220#29305#27530#8221#20026#20248#20808#25441#36215#65292#8220#33258#21160#25342#21462#8221#20026#20801#35768#25441#36215
        Caption = #33521#38596#25441#29289#21516#27493#20869#25346#37197#32622
        TabOrder = 12
        OnClick = chkEnableHeroUseClientPickItemsClick
      end
      object chkEnablePlayerUseClientPickItems: TCheckBox
        Left = 182
        Top = 427
        Width = 140
        Height = 17
        Hint = #23458#25143#31471#20869#25346#29289#21697#20013#8220#29305#27530#8221#20026#20248#20808#25441#36215#65292#8220#33258#21160#25342#21462#8221#20026#20801#35768#25441#36215
        Caption = #20154#29289#25441#29289#21516#27493#20869#25346#37197#32622
        ParentShowHint = False
        ShowHint = True
        TabOrder = 13
        OnClick = chkEnablePlayerUseClientPickItemsClick
      end
      object seItemGroupIndex: TSpinEdit
        Left = 375
        Top = 402
        Width = 89
        Height = 21
        MaxValue = 255
        MinValue = 0
        TabOrder = 14
        Value = 255
      end
    end
    object TabSheet15: TTabSheet
      Caption = #35013#22791#25216#33021#23041#21147
      ImageIndex = 12
      object GroupBox21: TGroupBox
        Left = 8
        Top = 4
        Width = 145
        Height = 438
        Caption = #29289#21697#21015#34920
        TabOrder = 0
        object ListBoxitemList5: TListBox
          Left = 8
          Top = 16
          Width = 129
          Height = 415
          Hint = 'Ctrl + F '#29289#21697#26597#25214#65292#21491#38190#21487#20197#36827#34892#31579#36873#12290
          ImeMode = imChinese
          ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
          ItemHeight = 12
          ParentShowHint = False
          PopupMenu = PopupMenu7
          ShowHint = True
          TabOrder = 0
          OnClick = ListBoxitemList5Click
          OnKeyDown = ListBoxitemListKeyDown
        end
      end
      object GroupBoxSkillPowerItem: TGroupBox
        Left = 160
        Top = 4
        Width = 465
        Height = 401
        TabOrder = 1
        object StringGridSkillPower: TStringGrid
          Left = 8
          Top = 16
          Width = 449
          Height = 377
          ColCount = 3
          DefaultColWidth = 150
          RowCount = 210
          Options = [goFixedVertLine, goFixedHorzLine, goVertLine, goHorzLine, goRangeSelect, goEditing]
          TabOrder = 0
          OnSetEditText = StringGridSkillPowerSetEditText
          ColWidths = (
            150
            145
            127)
          RowHeights = (
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24
            24)
        end
      end
      object GroupBox26: TGroupBox
        Left = 632
        Top = 4
        Width = 145
        Height = 373
        Caption = #35013#22791#25216#33021#23041#21147#21015#34920
        TabOrder = 2
        object ListBoxSkillPowerItem: TListBox
          Left = 8
          Top = 16
          Width = 129
          Height = 345
          ImeMode = imChinese
          ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
          ItemHeight = 12
          ParentShowHint = False
          ShowHint = True
          TabOrder = 0
          OnClick = ListBoxSkillPowerItemClick
        end
      end
      object ButtonAddSkillPowerItem: TButton
        Left = 656
        Top = 384
        Width = 57
        Height = 25
        Caption = #28155#21152'(&A)'
        TabOrder = 3
        OnClick = ButtonAddSkillPowerItemClick
      end
      object ButtonDelSkillPowerItem: TButton
        Left = 656
        Top = 416
        Width = 57
        Height = 25
        Caption = #21024#38500'(&D)'
        TabOrder = 6
        OnClick = ButtonDelSkillPowerItemClick
      end
      object ButtonChgSkillPowerItem: TButton
        Left = 720
        Top = 384
        Width = 57
        Height = 25
        Caption = #20462#25913'(&M)'
        TabOrder = 4
        OnClick = ButtonChgSkillPowerItemClick
      end
      object ButtonSaveSkillPowerItem: TButton
        Left = 720
        Top = 416
        Width = 57
        Height = 25
        Caption = #20445#23384'(&S)'
        TabOrder = 7
        OnClick = ButtonSaveSkillPowerItemClick
      end
      object grp2: TGroupBox
        Left = 160
        Top = 408
        Width = 465
        Height = 34
        Caption = #22686#21152#20260#23475#36873#39033
        TabOrder = 5
        object chkSkillPowerItemUseHum: TCheckBox
          Left = 88
          Top = 11
          Width = 137
          Height = 17
          Caption = #22686#21152#20260#23475#23545#20154#29289#26377#25928
          TabOrder = 0
          OnClick = chkSkillPowerItemUseHumClick
        end
        object chkSkillPowerItemUseMon: TCheckBox
          Left = 264
          Top = 11
          Width = 137
          Height = 17
          Caption = #22686#21152#20260#23475#23545#24618#29289#26377#25928
          TabOrder = 1
          OnClick = chkSkillPowerItemUseMonClick
        end
      end
    end
    object TabSheet16: TTabSheet
      Caption = #33258#23450#20041#36135#24065
      ImageIndex = 13
      object Label98: TLabel
        Left = 3
        Top = 377
        Width = 432
        Height = 12
        Caption = #23458#25143#31471#38656#35201#23567#36864#21047#26032#65292#26381#21153#31471#33258#23450#20041#36135#24065#20462#25913#21518#23454#26102#29983#25928#65292#22312#28216#25103#20013#35831#24910#37325#25805#20316'!!'
        Font.Charset = GB2312_CHARSET
        Font.Color = clRed
        Font.Height = -12
        Font.Name = #23435#20307
        Font.Style = []
        ParentFont = False
      end
      object Label99: TLabel
        Left = 3
        Top = 395
        Width = 252
        Height = 12
        Caption = 'Ctrl+Insert:'#28155#21152#36135#24065#65307'Ctrl+Delete:'#21024#38500#36135#24065
        Font.Charset = GB2312_CHARSET
        Font.Color = clRed
        Font.Height = -12
        Font.Name = #23435#20307
        Font.Style = []
        ParentFont = False
      end
      object Label100: TLabel
        Left = 3
        Top = 413
        Width = 312
        Height = 12
        Caption = '!!!'#28857#20987#21491#20391#31354#30333#21306#22495#33719#21462#25972#34892#28966#28857#20877#36827#34892#28155#21152#21024#38500#25805#20316'!!!'
        Font.Charset = GB2312_CHARSET
        Font.Color = clRed
        Font.Height = -12
        Font.Name = #23435#20307
        Font.Style = []
        ParentFont = False
      end
      object vstCustomMoney: TVirtualStringTree
        Left = 0
        Top = 0
        Width = 792
        Height = 345
        Colors.BorderColor = 15987699
        Colors.DisabledColor = clGray
        Colors.DropMarkColor = 15385233
        Colors.DropTargetColor = 15385233
        Colors.DropTargetBorderColor = 15987699
        Colors.FocusedSelectionColor = 14803425
        Colors.FocusedSelectionBorderColor = 14803425
        Colors.GridLineColor = 12303291
        Colors.HeaderHotColor = clBlack
        Colors.HotColor = clBlack
        Colors.SelectionRectangleBlendColor = 15385233
        Colors.SelectionRectangleBorderColor = 15385233
        Colors.SelectionTextColor = clBlack
        Colors.TreeLineColor = 9471874
        Colors.UnfocusedColor = 7925500
        Colors.UnfocusedSelectionColor = 14803425
        Colors.UnfocusedSelectionBorderColor = 14803425
        DefaultNodeHeight = 20
        Header.AutoSizeIndex = 0
        Header.Font.Charset = DEFAULT_CHARSET
        Header.Font.Color = clWindowText
        Header.Font.Height = -11
        Header.Font.Name = 'Tahoma'
        Header.Font.Style = []
        Header.Height = 40
        Header.MainColumn = 7
        Header.Options = [hoColumnResize, hoDrag, hoShowImages, hoShowSortGlyphs, hoVisible]
        HintMode = hmHint
        LineStyle = lsSolid
        Margin = 2
        ParentShowHint = False
        ShowHint = True
        TabOrder = 0
        TextMargin = 2
        TreeOptions.MiscOptions = [toAcceptOLEDrop, toCheckSupport, toEditable, toFullRepaintOnResize, toGridExtensions, toInitOnSave, toToggleOnDblClick, toWheelPanning, toEditOnClick]
        TreeOptions.PaintOptions = [toShowButtons, toShowDropmark, toShowHorzGridLines, toShowTreeLines, toShowVertGridLines, toThemeAware, toUseBlendedImages, toFullVertGridLines]
        TreeOptions.SelectionOptions = [toExtendedFocus, toFullRowSelect]
        OnCreateEditor = vstCustomMoneyCreateEditor
        OnEditing = vstCustomMoneyEditing
        OnGetText = vstCustomMoneyGetText
        OnGetNodeDataSize = vstCustomMoneyGetNodeDataSize
        OnKeyUp = vstCustomMoneyKeyUp
        OnNodeClick = vstCustomMoneyNodeClick
        Columns = <
          item
            Position = 0
            Width = 66
            WideText = #36135#24065#21517#31216
            WideHint = ' '
          end
          item
            Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coWrapCaption, coUseCaptionAlignment]
            Position = 1
            Width = 80
            WideText = #36135#24065#20998#32452
          end
          item
            Alignment = taRightJustify
            CaptionAlignment = taCenter
            Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coWrapCaption, coUseCaptionAlignment]
            Position = 2
            Width = 80
            WideText = #36135#24065#24207#21495
          end
          item
            Alignment = taRightJustify
            CaptionAlignment = taCenter
            Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coWrapCaption, coUseCaptionAlignment]
            Position = 3
            Width = 80
            WideText = #35760#24405#26085#24535
          end
          item
            Alignment = taRightJustify
            CaptionAlignment = taCenter
            Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coWrapCaption, coUseCaptionAlignment]
            Position = 4
            Width = 80
            WideText = #20010#20154#21830#24215
          end
          item
            Alignment = taRightJustify
            CaptionAlignment = taCenter
            Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coWrapCaption, coUseCaptionAlignment]
            Position = 5
            Width = 80
            WideText = #28216#25103#21830#24215
          end
          item
            Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coWrapCaption, coUseCaptionAlignment]
            Position = 6
            Width = 80
            WideText = #25293#21334#34892
          end
          item
            Alignment = taRightJustify
            CaptionAlignment = taCenter
            Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coWrapCaption, coUseCaptionAlignment]
            Position = 7
            Width = 80
            WideText = #35282#33394#20132#26131
          end
          item
            Position = 8
            Width = 160
          end>
        WideDefaultText = ''
      end
      object btnSaveCustomMoney: TButton
        Left = 698
        Top = 390
        Width = 75
        Height = 25
        Caption = #20445#23384'(&S)'
        Enabled = False
        TabOrder = 1
        OnClick = btnSaveCustomMoneyClick
      end
    end
  end
  object PopupMenuItem: TPopupMenu
    Left = 448
    Top = 223
    object MenuItem_ShowAll: TMenuItem
      Tag = -1
      Caption = #26174#31034#20840#37096#29289#21697
      Checked = True
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem_ShowDress: TMenuItem
      Caption = #21482#26174#31034#34915#26381
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem_ShowNECKLACE: TMenuItem
      Tag = 3
      Caption = #21482#26174#31034#39033#38142
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem_ShowWEAPON: TMenuItem
      Tag = 1
      Caption = #21482#26174#31034#27494#22120
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem_ShowRING: TMenuItem
      Tag = 8
      Caption = #21482#26174#31034#25106#25351
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem_ShowRIGHTHAND: TMenuItem
      Tag = 2
      Caption = #21482#26174#31034#29031#26126#29289
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem_ShowHELMET: TMenuItem
      Tag = 4
      Caption = #21482#26174#31034#22836#30420
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem_ShowARMRING: TMenuItem
      Tag = 6
      Caption = #21482#26174#31034#25163#38255
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem_ShowBOOTS: TMenuItem
      Tag = 11
      Caption = #21482#26174#31034#38795#23376
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem_ShowBUJUK: TMenuItem
      Tag = 9
      Caption = #21482#26174#31034#31526
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem_BELT: TMenuItem
      Tag = 10
      Caption = #21482#26174#31034#33136#24102
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem_CHARM: TMenuItem
      Tag = 12
      Caption = #21482#26174#31034#23453#30707
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem_ShowDrugIrem: TMenuItem
      Tag = 100
      Caption = #21482#26174#31034#33647#29289
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem_ShowBindIrem: TMenuItem
      Tag = 31
      Caption = #21482#26174#31034#25171#21253#29289#21697
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem_ShowOTHERITEM: TMenuItem
      Tag = 111
      Caption = #26174#31034#20854#20182
      OnClick = MenuItem_ShowAllClick
    end
  end
  object PopupMenu1: TPopupMenu
    Left = 384
    Top = 191
    object MenuItem1: TMenuItem
      Tag = -1
      Caption = #26174#31034#20840#37096#29289#21697
      Checked = True
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem2: TMenuItem
      Caption = #21482#26174#31034#34915#26381
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem3: TMenuItem
      Tag = 3
      Caption = #21482#26174#31034#39033#38142
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem4: TMenuItem
      Tag = 1
      Caption = #21482#26174#31034#27494#22120
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem5: TMenuItem
      Tag = 8
      Caption = #21482#26174#31034#25106#25351
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem6: TMenuItem
      Tag = 2
      Caption = #21482#26174#31034#29031#26126#29289
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem7: TMenuItem
      Tag = 4
      Caption = #21482#26174#31034#22836#30420
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem8: TMenuItem
      Tag = 6
      Caption = #21482#26174#31034#25163#38255
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem9: TMenuItem
      Tag = 11
      Caption = #21482#26174#31034#38795#23376
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem10: TMenuItem
      Tag = 9
      Caption = #21482#26174#31034#31526
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem11: TMenuItem
      Tag = 10
      Caption = #21482#26174#31034#33136#24102
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem12: TMenuItem
      Tag = 12
      Caption = #21482#26174#31034#23453#30707
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem13: TMenuItem
      Tag = 100
      Caption = #21482#26174#31034#33647#29289
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem14: TMenuItem
      Tag = 31
      Caption = #21482#26174#31034#25171#21253#29289#21697
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem15: TMenuItem
      Tag = 111
      Caption = #26174#31034#20854#20182
      OnClick = MenuItem_ShowAllClick
    end
  end
  object PopupMenu2: TPopupMenu
    Left = 416
    Top = 191
    object MenuItem16: TMenuItem
      Tag = -1
      Caption = #26174#31034#20840#37096#29289#21697
      Checked = True
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem17: TMenuItem
      Caption = #21482#26174#31034#34915#26381
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem18: TMenuItem
      Tag = 3
      Caption = #21482#26174#31034#39033#38142
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem19: TMenuItem
      Tag = 1
      Caption = #21482#26174#31034#27494#22120
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem20: TMenuItem
      Tag = 8
      Caption = #21482#26174#31034#25106#25351
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem21: TMenuItem
      Tag = 2
      Caption = #21482#26174#31034#29031#26126#29289
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem22: TMenuItem
      Tag = 4
      Caption = #21482#26174#31034#22836#30420
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem23: TMenuItem
      Tag = 6
      Caption = #21482#26174#31034#25163#38255
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem24: TMenuItem
      Tag = 11
      Caption = #21482#26174#31034#38795#23376
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem25: TMenuItem
      Tag = 9
      Caption = #21482#26174#31034#31526
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem26: TMenuItem
      Tag = 10
      Caption = #21482#26174#31034#33136#24102
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem27: TMenuItem
      Tag = 12
      Caption = #21482#26174#31034#23453#30707
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem28: TMenuItem
      Tag = 100
      Caption = #21482#26174#31034#33647#29289
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem29: TMenuItem
      Tag = 31
      Caption = #21482#26174#31034#25171#21253#29289#21697
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem30: TMenuItem
      Tag = 111
      Caption = #26174#31034#20854#20182
      OnClick = MenuItem_ShowAllClick
    end
  end
  object PopupMenu3: TPopupMenu
    Left = 448
    Top = 191
    object MenuItem31: TMenuItem
      Tag = -1
      Caption = #26174#31034#20840#37096#29289#21697
      Checked = True
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem32: TMenuItem
      Caption = #21482#26174#31034#34915#26381
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem33: TMenuItem
      Tag = 3
      Caption = #21482#26174#31034#39033#38142
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem34: TMenuItem
      Tag = 1
      Caption = #21482#26174#31034#27494#22120
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem35: TMenuItem
      Tag = 8
      Caption = #21482#26174#31034#25106#25351
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem36: TMenuItem
      Tag = 2
      Caption = #21482#26174#31034#29031#26126#29289
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem37: TMenuItem
      Tag = 4
      Caption = #21482#26174#31034#22836#30420
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem38: TMenuItem
      Tag = 6
      Caption = #21482#26174#31034#25163#38255
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem39: TMenuItem
      Tag = 11
      Caption = #21482#26174#31034#38795#23376
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem40: TMenuItem
      Tag = 9
      Caption = #21482#26174#31034#31526
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem41: TMenuItem
      Tag = 10
      Caption = #21482#26174#31034#33136#24102
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem42: TMenuItem
      Tag = 12
      Caption = #21482#26174#31034#23453#30707
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem43: TMenuItem
      Tag = 100
      Caption = #21482#26174#31034#33647#29289
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem44: TMenuItem
      Tag = 31
      Caption = #21482#26174#31034#25171#21253#29289#21697
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem45: TMenuItem
      Tag = 111
      Caption = #26174#31034#20854#20182
      OnClick = MenuItem_ShowAllClick
    end
  end
  object PopupMenu4: TPopupMenu
    Left = 320
    Top = 223
    object MenuItem46: TMenuItem
      Tag = -1
      Caption = #26174#31034#20840#37096#29289#21697
      Checked = True
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem47: TMenuItem
      Caption = #21482#26174#31034#34915#26381
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem48: TMenuItem
      Tag = 3
      Caption = #21482#26174#31034#39033#38142
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem49: TMenuItem
      Tag = 1
      Caption = #21482#26174#31034#27494#22120
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem50: TMenuItem
      Tag = 8
      Caption = #21482#26174#31034#25106#25351
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem51: TMenuItem
      Tag = 2
      Caption = #21482#26174#31034#29031#26126#29289
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem52: TMenuItem
      Tag = 4
      Caption = #21482#26174#31034#22836#30420
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem53: TMenuItem
      Tag = 6
      Caption = #21482#26174#31034#25163#38255
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem54: TMenuItem
      Tag = 11
      Caption = #21482#26174#31034#38795#23376
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem55: TMenuItem
      Tag = 9
      Caption = #21482#26174#31034#31526
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem56: TMenuItem
      Tag = 10
      Caption = #21482#26174#31034#33136#24102
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem57: TMenuItem
      Tag = 12
      Caption = #21482#26174#31034#23453#30707
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem58: TMenuItem
      Tag = 100
      Caption = #21482#26174#31034#33647#29289
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem59: TMenuItem
      Tag = 31
      Caption = #21482#26174#31034#25171#21253#29289#21697
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem60: TMenuItem
      Tag = 111
      Caption = #26174#31034#20854#20182
      OnClick = MenuItem_ShowAllClick
    end
  end
  object PopupMenu5: TPopupMenu
    Left = 352
    Top = 223
    object MenuItem61: TMenuItem
      Tag = -1
      Caption = #26174#31034#20840#37096#29289#21697
      Checked = True
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem62: TMenuItem
      Caption = #21482#26174#31034#34915#26381
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem63: TMenuItem
      Tag = 3
      Caption = #21482#26174#31034#39033#38142
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem64: TMenuItem
      Tag = 1
      Caption = #21482#26174#31034#27494#22120
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem65: TMenuItem
      Tag = 8
      Caption = #21482#26174#31034#25106#25351
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem66: TMenuItem
      Tag = 2
      Caption = #21482#26174#31034#29031#26126#29289
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem67: TMenuItem
      Tag = 4
      Caption = #21482#26174#31034#22836#30420
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem68: TMenuItem
      Tag = 6
      Caption = #21482#26174#31034#25163#38255
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem69: TMenuItem
      Tag = 11
      Caption = #21482#26174#31034#38795#23376
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem70: TMenuItem
      Tag = 9
      Caption = #21482#26174#31034#31526
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem71: TMenuItem
      Tag = 10
      Caption = #21482#26174#31034#33136#24102
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem72: TMenuItem
      Tag = 12
      Caption = #21482#26174#31034#23453#30707
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem73: TMenuItem
      Tag = 100
      Caption = #21482#26174#31034#33647#29289
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem74: TMenuItem
      Tag = 31
      Caption = #21482#26174#31034#25171#21253#29289#21697
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem75: TMenuItem
      Tag = 111
      Caption = #26174#31034#20854#20182
      OnClick = MenuItem_ShowAllClick
    end
  end
  object PopupMenu6: TPopupMenu
    Left = 384
    Top = 223
    object MenuItem76: TMenuItem
      Tag = -1
      Caption = #26174#31034#20840#37096#29289#21697
      Checked = True
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem77: TMenuItem
      Caption = #21482#26174#31034#34915#26381
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem78: TMenuItem
      Tag = 3
      Caption = #21482#26174#31034#39033#38142
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem79: TMenuItem
      Tag = 1
      Caption = #21482#26174#31034#27494#22120
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem80: TMenuItem
      Tag = 8
      Caption = #21482#26174#31034#25106#25351
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem81: TMenuItem
      Tag = 2
      Caption = #21482#26174#31034#29031#26126#29289
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem82: TMenuItem
      Tag = 4
      Caption = #21482#26174#31034#22836#30420
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem83: TMenuItem
      Tag = 6
      Caption = #21482#26174#31034#25163#38255
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem84: TMenuItem
      Tag = 11
      Caption = #21482#26174#31034#38795#23376
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem85: TMenuItem
      Tag = 9
      Caption = #21482#26174#31034#31526
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem86: TMenuItem
      Tag = 10
      Caption = #21482#26174#31034#33136#24102
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem87: TMenuItem
      Tag = 12
      Caption = #21482#26174#31034#23453#30707
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem88: TMenuItem
      Tag = 100
      Caption = #21482#26174#31034#33647#29289
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem89: TMenuItem
      Tag = 31
      Caption = #21482#26174#31034#25171#21253#29289#21697
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem90: TMenuItem
      Tag = 111
      Caption = #26174#31034#20854#20182
      OnClick = MenuItem_ShowAllClick
    end
  end
  object PopupMenu7: TPopupMenu
    Left = 416
    Top = 223
    object MenuItem91: TMenuItem
      Tag = -1
      Caption = #26174#31034#20840#37096#29289#21697
      Checked = True
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem92: TMenuItem
      Caption = #21482#26174#31034#34915#26381
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem93: TMenuItem
      Tag = 3
      Caption = #21482#26174#31034#39033#38142
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem94: TMenuItem
      Tag = 1
      Caption = #21482#26174#31034#27494#22120
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem95: TMenuItem
      Tag = 8
      Caption = #21482#26174#31034#25106#25351
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem96: TMenuItem
      Tag = 2
      Caption = #21482#26174#31034#29031#26126#29289
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem97: TMenuItem
      Tag = 4
      Caption = #21482#26174#31034#22836#30420
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem98: TMenuItem
      Tag = 6
      Caption = #21482#26174#31034#25163#38255
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem99: TMenuItem
      Tag = 11
      Caption = #21482#26174#31034#38795#23376
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem100: TMenuItem
      Tag = 9
      Caption = #21482#26174#31034#31526
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem101: TMenuItem
      Tag = 10
      Caption = #21482#26174#31034#33136#24102
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem102: TMenuItem
      Tag = 12
      Caption = #21482#26174#31034#23453#30707
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem103: TMenuItem
      Tag = 100
      Caption = #21482#26174#31034#33647#29289
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem104: TMenuItem
      Tag = 31
      Caption = #21482#26174#31034#25171#21253#29289#21697
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem105: TMenuItem
      Tag = 111
      Caption = #26174#31034#20854#20182
      OnClick = MenuItem_ShowAllClick
    end
  end
  object pmFilterItem: TPopupMenu
    Left = 320
    Top = 191
    object mniN1: TMenuItem
      Caption = #26497#21697#25552#31034
      object mniN3: TMenuItem
        Caption = #25209#37327#35774#32622'(&S)'
        OnClick = mniN3Click
      end
      object mniN4: TMenuItem
        Tag = 1
        Caption = #25209#37327#21462#28040'(&C)'
        OnClick = mniN3Click
      end
    end
    object mniN6: TMenuItem
      Tag = 1
      Caption = #33258#21160#25441#21462
      object mniS1: TMenuItem
        Caption = #25209#37327#35774#32622'(&S)'
        OnClick = mniN3Click
      end
      object mniC1: TMenuItem
        Tag = 1
        Caption = #25209#37327#21462#28040'(&C)'
        OnClick = mniN3Click
      end
    end
    object mniN7: TMenuItem
      Tag = 2
      Caption = #26174#31034#21517#31216
      object mniS2: TMenuItem
        Caption = #25209#37327#35774#32622'(&S)'
        OnClick = mniN3Click
      end
      object mniC2: TMenuItem
        Tag = 1
        Caption = #25209#37327#21462#28040'(&C)'
        OnClick = mniN3Click
      end
    end
    object mniN10: TMenuItem
      Tag = 3
      Caption = #29305#27530#29289#21697
      object mniS3: TMenuItem
        Caption = #25209#37327#35774#32622'(&S)'
        OnClick = mniN3Click
      end
      object mniC4: TMenuItem
        Tag = 1
        Caption = #25209#37327#21462#28040'(&C)'
        OnClick = mniN3Click
      end
    end
    object mniN2: TMenuItem
      Tag = 4
      Caption = #33258#21160#20256#36865
      object mniS4: TMenuItem
        Caption = #25209#37327#35774#32622'(&S)'
        OnClick = mniN3Click
      end
      object mniC3: TMenuItem
        Tag = 1
        Caption = #25209#37327#21462#28040'(&C)'
        OnClick = mniN3Click
      end
    end
    object N1: TMenuItem
      Caption = #20998#32452#24207#21495
      OnClick = N1Click
    end
  end
  object pmRuleList: TPopupMenu
    Left = 352
    Top = 191
  end
end
