object FrmGameSpeed: TFrmGameSpeed
  Left = 219
  Top = 171
  BorderStyle = bsDialog
  Caption = #22806#25346#25511#21046
  ClientHeight = 589
  ClientWidth = 761
  Color = clBtnFace
  Font.Charset = GB2312_CHARSET
  Font.Color = clBlack
  Font.Height = -12
  Font.Name = #23435#20307
  Font.Style = []
  OldCreateOrder = False
  Position = poMainFormCenter
  OnCreate = FormCreate
  PixelsPerInch = 96
  TextHeight = 12
  object lbl7: TLabel
    Left = 687
    Top = 536
    Width = 60
    Height = 12
    Caption = #27425#36386#20154#19979#32447
  end
  object lbl10: TLabel
    Left = 8
    Top = 560
    Width = 144
    Height = 12
    Caption = #23458#25143#31471#19978#20256#20869#25346#29289#21697#38388#38548#65306
    Font.Charset = GB2312_CHARSET
    Font.Color = clBlue
    Font.Height = -12
    Font.Name = #23435#20307
    Font.Style = []
    ParentFont = False
  end
  object lbl11: TLabel
    Left = 211
    Top = 560
    Width = 12
    Height = 12
    Caption = #31186
  end
  object GroupBox1: TGroupBox
    Left = 8
    Top = 8
    Width = 744
    Height = 383
    Caption = #21442#25968#35774#32622
    TabOrder = 0
    object Label3: TLabel
      Left = 64
      Top = 1
      Width = 210
      Height = 12
      Caption = #35302#21457' QFunction.txt [@UsePlugin]'#23383#27573
      Font.Charset = GB2312_CHARSET
      Font.Color = clRed
      Font.Height = -12
      Font.Name = #23435#20307
      Font.Style = []
      ParentFont = False
    end
    object vstAntiPlugAction: TVirtualStringTree
      Left = 7
      Top = 18
      Width = 730
      Height = 358
      Colors.FocusedSelectionColor = 14803425
      Colors.FocusedSelectionBorderColor = 14803425
      Colors.GridLineColor = 12303291
      Colors.UnfocusedSelectionColor = 14803425
      Colors.UnfocusedSelectionBorderColor = 14803425
      DefaultNodeHeight = 22
      Header.AutoSizeIndex = 6
      Header.DefaultHeight = 24
      Header.Font.Charset = GB2312_CHARSET
      Header.Font.Color = clWindowText
      Header.Font.Height = -12
      Header.Font.Name = #23435#20307
      Header.Font.Style = []
      Header.Height = 24
      Header.Options = [hoAutoResize, hoColumnResize, hoDrag, hoShowHint, hoShowSortGlyphs, hoVisible]
      HintMode = hmHint
      LineStyle = lsSolid
      Margin = 3
      ParentShowHint = False
      ShowHint = True
      TabOrder = 0
      TextMargin = 3
      TreeOptions.MiscOptions = [toAcceptOLEDrop, toCheckSupport, toFullRepaintOnResize, toInitOnSave, toToggleOnDblClick, toWheelPanning, toEditOnClick]
      TreeOptions.PaintOptions = [toHideFocusRect, toShowButtons, toShowDropmark, toShowHorzGridLines, toShowVertGridLines, toThemeAware, toUseBlendedImages, toFullVertGridLines]
      TreeOptions.SelectionOptions = [toExtendedFocus, toFullRowSelect]
      OnAfterCellPaint = vstAntiPlugActionAfterCellPaint
      OnChecked = vstAntiPlugActionChecked
      OnChecking = vstAntiPlugActionChecking
      OnCreateEditor = vstAntiPlugActionCreateEditor
      OnDrawText = vstAntiPlugActionDrawText
      OnEditing = vstAntiPlugActionEditing
      OnGetText = vstAntiPlugActionGetText
      OnGetHint = vstAntiPlugActionGetHint
      OnNodeClick = vstAntiPlugActionNodeClick
      Columns = <
        item
          Margin = 1
          Position = 0
          Spacing = 1
          Width = 112
          WideText = #26159#21542#25511#21046
        end
        item
          Alignment = taRightJustify
          CaptionAlignment = taCenter
          Margin = 1
          Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coUseCaptionAlignment]
          Position = 1
          Spacing = 1
          WideText = #38388#38548
          WideHint = #21333#20301#65306#27627#31186
        end
        item
          Margin = 1
          Position = 2
          Spacing = 1
          Width = 120
          WideText = #36229#36895#22788#29702#26041#24335
        end
        item
          Margin = 1
          Position = 3
          Spacing = 1
          Width = 80
          WideText = #32047#35745#36229#36895#22788#29702
        end
        item
          Alignment = taRightJustify
          CaptionAlignment = taCenter
          Margin = 1
          Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coAllowFocus, coUseCaptionAlignment]
          Position = 4
          Spacing = 1
          Width = 56
          WideText = #28014#21160#38388#38548
        end
        item
          Margin = 1
          Position = 5
          Spacing = 1
          Width = 54
          WideText = #36229#36895#25552#31034
        end
        item
          Margin = 1
          Position = 6
          Spacing = 1
          Width = 228
          WideText = #36229#36895#25552#31034#20449#24687
        end
        item
          Alignment = taCenter
          Position = 7
          WideText = #34917#20607#20540
        end
        item
          Alignment = taCenter
          Margin = 1
          Position = 8
          Spacing = 1
          Style = vsOwnerDraw
          Width = 32
          WideText = #35843#35797
        end>
    end
  end
  object btnSave: TButton
    Left = 607
    Top = 558
    Width = 70
    Height = 23
    Caption = #20445#23384'(&S)'
    TabOrder = 6
    OnClick = btnSaveClick
  end
  object btnClose: TButton
    Left = 682
    Top = 558
    Width = 70
    Height = 23
    Caption = #20851#38381'(&E)'
    TabOrder = 7
    OnClick = btnCloseClick
  end
  object GroupBox2: TGroupBox
    Left = 8
    Top = 396
    Width = 252
    Height = 65
    Caption = #25552#31034#35774#32622
    TabOrder = 1
    object lbl2: TLabel
      Left = 8
      Top = 43
      Width = 30
      Height = 12
      Caption = #25991#23383':'
    end
    object Label1: TLabel
      Left = 8
      Top = 20
      Width = 30
      Height = 12
      Caption = #32972#26223':'
    end
    object lbl1: TLabel
      Left = 111
      Top = 20
      Width = 54
      Height = 12
      Caption = #25552#31034#26041#24335':'
    end
    object lbl4: TLabel
      Left = 111
      Top = 43
      Width = 54
      Height = 12
      Caption = #25928#26524#39044#35272':'
    end
    object seFColor: TColorIndexEdit
      Left = 39
      Top = 39
      Width = 60
      Height = 21
      MaxLength = 3
      MaxValue = 255
      MinValue = 0
      TabOrder = 2
      Value = 255
      OnChange = seFColorChange
      ShowNoneColor = False
    end
    object seBColor: TColorIndexEdit
      Left = 39
      Top = 16
      Width = 60
      Height = 21
      MaxLength = 3
      MaxValue = 255
      MinValue = 0
      TabOrder = 0
      Value = 255
      OnChange = seBColorChange
      ShowNoneColor = False
    end
    object cbbMsgType: TComboBox
      Left = 166
      Top = 16
      Width = 80
      Height = 20
      Style = csDropDownList
      ItemHeight = 12
      ItemIndex = 0
      TabOrder = 1
      Text = #23494#20154#25552#31034
      OnChange = cbbMsgTypeChange
      Items.Strings = (
        #23494#20154#25552#31034
        #24377#31383#25552#31034)
    end
    object edtPreview: TEdit
      Left = 166
      Top = 39
      Width = 79
      Height = 20
      ReadOnly = True
      TabOrder = 3
      Text = #25552#31034#25991#23383#39044#35272
    end
  end
  object GroupBox3: TGroupBox
    Left = 265
    Top = 396
    Width = 297
    Height = 65
    Caption = #38145#23450#35774#32622
    TabOrder = 2
    object Label6: TLabel
      Left = 8
      Top = 20
      Width = 54
      Height = 12
      Caption = #38145#23450#26102#38271':'
    end
    object Label7: TLabel
      Left = 114
      Top = 20
      Width = 12
      Height = 12
      Caption = #31186
    end
    object lbl3: TLabel
      Left = 8
      Top = 43
      Width = 54
      Height = 12
      Caption = #38145#23450#25552#31034':'
    end
    object seLockTime: TSpinEditEx
      Left = 63
      Top = 15
      Width = 50
      Height = 21
      MaxValue = 0
      MinValue = 0
      TabOrder = 0
      Value = 0
      OnChange = seLockTimeChange
    end
    object chkSaveLockStatus: TCheckBox
      Left = 141
      Top = 18
      Width = 67
      Height = 17
      Hint = 
        #21246#36873#21518#65292#29609#23478#19979#32447#23558#20445#23384#38145#23450#29366#24577#13#10#22914#65306#29609#23478#38145#23450'10'#31186#65292#22312#31532'2'#31186#26102#19979#32447#65292#20877#27425#19978#32447#23558#32487#32493#38145#23450'8'#31186#13#10#27880#65306#37325#21551#32593#20851#23558#21024#38500#25152#26377#20445#23384#30340#38145#23450 +
        #29366#24577#20449#24687#65281
      Caption = #20445#23384#29366#24577
      ParentShowHint = False
      ShowHint = True
      TabOrder = 1
      OnClick = chkSaveLockStatusClick
    end
    object chkShowLockLog: TCheckBox
      Left = 223
      Top = 18
      Width = 66
      Height = 17
      Caption = #26174#31034#26085#24535
      TabOrder = 2
      OnClick = chkShowLockLogClick
    end
    object edtShowLockMsg: TEdit
      Left = 63
      Top = 38
      Width = 226
      Height = 21
      Color = clWhite
      Font.Charset = DEFAULT_CHARSET
      Font.Color = clWindowText
      Font.Height = -11
      Font.Name = 'MS Sans Serif'
      Font.Style = []
      ParentFont = False
      TabOrder = 3
      OnChange = edtShowLockMsgChange
    end
  end
  object GroupBox5: TGroupBox
    Left = 265
    Top = 464
    Width = 297
    Height = 85
    Caption = #38388#38548#35774#32622' ['#27627#31186']'
    TabOrder = 3
    object Label5: TLabel
      Left = 166
      Top = 19
      Width = 66
      Height = 12
      Caption = #20132#26131#21040#25361#25112':'
    end
    object Label8: TLabel
      Left = 166
      Top = 41
      Width = 66
      Height = 12
      Caption = #37326#34542#21518#25915#20987':'
    end
    object Label4: TLabel
      Left = 7
      Top = 19
      Width = 78
      Height = 12
      Caption = #20010#20154#21830#24215#25628#32034':'
    end
    object Label10: TLabel
      Left = 7
      Top = 41
      Width = 78
      Height = 12
      Caption = #20010#20154#21830#24215#36141#20080':'
    end
    object Label2: TLabel
      Left = 7
      Top = 63
      Width = 78
      Height = 12
      Caption = #35282#33394#31359#25140#35013#22791':'
    end
    object seDealTryAttackTime: TSpinEditEx
      Left = 232
      Top = 15
      Width = 44
      Height = 21
      Hint = #25511#21046#20132#26131#25110#25361#25112#30340#38388#38548#26102#38388#12289#20351#29992#25915#20987#12289#39764#27861#21069#21518#20351#29992#20132#26131#25110#25361#25112#30340#38388#38548#26102#38388
      MaxValue = 0
      MinValue = 0
      ParentShowHint = False
      ShowHint = True
      TabOrder = 0
      Value = 0
      OnChange = seDealTryAttackTimeChange
    end
    object seBrutalAttackTime: TSpinEditEx
      Left = 232
      Top = 37
      Width = 44
      Height = 21
      Hint = #20351#29992#37326#34542#21518#38656#35201#31561#24453#22810#38271#26102#38388#25165#21487#20197#36827#34892#25915#20987#12289#39764#27861#12289#31227#21160#13#10#13#10#20351#29992#36716#21521#21518#38656#35201#31561#24453#22810#38271#26102#38388#25165#21487#20197#20351#29992#37326#34542#20914#25758
      MaxValue = 0
      MinValue = 0
      ParentShowHint = False
      ShowHint = True
      TabOrder = 2
      Value = 0
      OnChange = seBrutalAttackTimeChange
    end
    object chkDealTryAttackHint: TCheckBox
      Left = 277
      Top = 17
      Width = 12
      Height = 17
      Hint = #20132#26131#21040#25361#25112#36229#36895#26102#25552#31034
      ParentShowHint = False
      ShowHint = True
      TabOrder = 1
      OnClick = chkDealTryAttackHintClick
    end
    object chkBrutalAttackHint: TCheckBox
      Left = 277
      Top = 39
      Width = 15
      Height = 17
      Hint = #37326#34542#21040#25915#20987#36229#36895#26102#25552#31034
      ParentShowHint = False
      ShowHint = True
      TabOrder = 3
      OnClick = chkBrutalAttackHintClick
    end
    object seUserShopSearchTime: TSpinEditEx
      Left = 86
      Top = 14
      Width = 44
      Height = 21
      Hint = #25511#21046#20010#20154#21830#24215#29289#21697#25628#32034#38388#38548#26102#38388
      MaxValue = 0
      MinValue = 0
      ParentShowHint = False
      ShowHint = True
      TabOrder = 4
      Value = 0
      OnChange = seUserShopSearchTimeChange
    end
    object chkUserShopSearchHint: TCheckBox
      Left = 132
      Top = 16
      Width = 16
      Height = 17
      Hint = #25628#32034#20010#20154#21830#24215#29289#21697#36229#36895#26102#25552#31034
      ParentShowHint = False
      ShowHint = True
      TabOrder = 5
      OnClick = chkUserShopSearchHintClick
    end
    object seUserShopBuyTime: TSpinEditEx
      Left = 86
      Top = 36
      Width = 44
      Height = 21
      Hint = #25511#21046#20010#20154#21830#24215#29289#21697#36141#20080#38388#38548#26102#38388
      MaxValue = 0
      MinValue = 0
      ParentShowHint = False
      ShowHint = True
      TabOrder = 6
      Value = 0
      OnChange = seUserShopBuyTimeChange
    end
    object chkUserShopBuyHint: TCheckBox
      Left = 132
      Top = 38
      Width = 16
      Height = 17
      Hint = #36141#20080#20010#20154#21830#24215#29289#21697#36229#36895#26102#25552#31034
      ParentShowHint = False
      ShowHint = True
      TabOrder = 7
      OnClick = chkUserShopBuyHintClick
    end
    object seTakeOnItemTime: TSpinEditEx
      Left = 86
      Top = 58
      Width = 44
      Height = 21
      Hint = #25511#21046#20154#29289#25110#33521#38596#31359#25140#35013#22791#30340#38388#38548#26102#38388
      MaxValue = 0
      MinValue = 0
      ParentShowHint = False
      ShowHint = True
      TabOrder = 8
      Value = 0
      OnChange = seTakeOnItemTimeChange
    end
    object chkTakeOnItemHint: TCheckBox
      Left = 132
      Top = 60
      Width = 15
      Height = 17
      Hint = #31359#25140#35013#22791#36229#36895#26102#25552#31034
      ParentShowHint = False
      ShowHint = True
      TabOrder = 9
      OnClick = chkTakeOnItemHintClick
    end
    object chkZeroCompensationValueClearPool: TCheckBox
      Left = 183
      Top = 61
      Width = 107
      Height = 17
      Alignment = taLeftJustify
      Caption = '0'#34917#20607#26102#28165#34917#20607#27744
      TabOrder = 10
      OnClick = chkZeroCompensationValueClearPoolClick
    end
  end
  object grp2: TGroupBox
    Left = 8
    Top = 464
    Width = 252
    Height = 85
    Caption = #20854#20182#35774#32622
    TabOrder = 4
    object lbl5: TLabel
      Left = 8
      Top = 18
      Width = 120
      Height = 12
      Caption = #36830#32493#36229#36895#25918#34892#26102#38388#22686#21152
    end
    object Label9: TLabel
      Left = 219
      Top = 18
      Width = 24
      Height = 12
      Caption = #27627#31186
    end
    object seContinueSpeedPassIncTime: TSpinEditEx
      Left = 133
      Top = 14
      Width = 83
      Height = 21
      MaxValue = 0
      MinValue = 0
      TabOrder = 0
      Value = 0
      OnChange = seContinueSpeedPassIncTimeChange
    end
    object chkSpeedClearData: TCheckBox
      Left = 9
      Top = 38
      Width = 237
      Height = 17
      Caption = #24403#36229#36895#22788#29702#21518#65292#28165#31354#25152#26377#26410#22788#29702#30340#25968#25454#21253
      Font.Charset = GB2312_CHARSET
      Font.Color = clBlack
      Font.Height = -12
      Font.Name = #23435#20307
      Font.Style = []
      ParentFont = False
      TabOrder = 1
      OnClick = chkSpeedClearDataClick
    end
    object chkShowAttackLog: TCheckBox
      Left = 9
      Top = 60
      Width = 94
      Height = 17
      Caption = #26174#31034#36229#36895#26085#24535
      Font.Charset = GB2312_CHARSET
      Font.Color = clBlack
      Font.Height = -12
      Font.Name = #23435#20307
      Font.Style = []
      ParentFont = False
      TabOrder = 2
      OnClick = chkShowAttackLogClick
    end
    object chkShowDropConcurrentLog: TCheckBox
      Left = 129
      Top = 84
      Width = 115
      Height = 17
      Caption = #26174#31034#22810#20493#24182#21457#26085#24535
      Font.Charset = GB2312_CHARSET
      Font.Color = clBlack
      Font.Height = -12
      Font.Name = #23435#20307
      Font.Style = []
      ParentFont = False
      TabOrder = 3
      Visible = False
      OnClick = chkShowDropConcurrentLogClick
    end
  end
  object GroupBox6: TGroupBox
    Left = 566
    Top = 396
    Width = 187
    Height = 65
    Caption = #21152#36895#35268#21017#25511#21046
    TabOrder = 5
    object lblSpeedValue: TLabel
      Left = 87
      Top = 0
      Width = 6
      Height = 12
      Font.Charset = GB2312_CHARSET
      Font.Color = clBlue
      Font.Height = -12
      Font.Name = #23435#20307
      Font.Style = []
      ParentFont = False
      Transparent = False
    end
    object lbl9: TLabel
      Left = 8
      Top = 20
      Width = 60
      Height = 12
      Caption = #24635#35760#24405#25968#65306
    end
    object Label12: TLabel
      Left = 8
      Top = 41
      Width = 60
      Height = 12
      Caption = #36229#36895#27425#25968#65306
    end
    object trckbrSpeedValue: TTrackBar
      Left = 64
      Top = 36
      Width = 118
      Height = 22
      Hint = #26368#22810#20801#35768#36229#36895#27425#25968#65288#40664#35748#20540'9'#65289#65292#24403#23553#21253#36229#36895'>='#35774#23450#30340#27425#25968#26102#35270#20026#36229#36895#12290#13#10#13#10#35774#32622#36234#23567#36234#20005#26684#65292#36234#22823#36234#23485#26494
      Max = 18
      Min = 2
      ParentShowHint = False
      Position = 9
      ShowHint = True
      TabOrder = 0
      ThumbLength = 10
      TickMarks = tmTopLeft
      OnChange = trckbrSpeedValueChange
    end
    object trckbrCollectCount: TTrackBar
      Left = 64
      Top = 15
      Width = 118
      Height = 22
      Hint = #35774#32622#23553#21253#35760#24405#25968#65288#40664#35748#20540'15'#65289#13#10#13#10#24403#22312#35774#32622#30340#8220#24635#35760#24405#25968#8221#20013#30340#23553#21253#36229#36895'>='#8220#36229#36895#27425#25968#8221#21028#23450#20026#36229#36895
      Max = 20
      Min = 5
      ParentShowHint = False
      Position = 15
      ShowHint = True
      TabOrder = 1
      ThumbLength = 10
      TickMarks = tmTopLeft
      OnChange = trckbrSpeedValueChange
    end
  end
  object GroupBox4: TGroupBox
    Left = 566
    Top = 464
    Width = 187
    Height = 60
    Caption = #32047#35745#36229#36895#35268#21017
    TabOrder = 8
    object lbl6: TLabel
      Left = 8
      Top = 17
      Width = 78
      Height = 12
      Caption = #32479#35745#26102#38388#38388#38548':'
    end
    object Label11: TLabel
      Left = 8
      Top = 39
      Width = 78
      Height = 12
      Caption = #26368#22823#36229#36895#27425#25968':'
    end
    object lbl8: TLabel
      Left = 164
      Top = 17
      Width = 12
      Height = 12
      Caption = #31186
    end
    object seSumSpeedCheckTime: TSpinEditEx
      Left = 87
      Top = 13
      Width = 77
      Height = 21
      Hint = #22312#25351#23450#26102#38388#20869#36229#36895#22788#29702#27425#25968'>="'#26368#22823#36229#36895#27425#25968'"'#65292#21017#35302#21457#32047#35745#36229#36895#22788#29702
      MaxValue = 0
      MinValue = 0
      ParentShowHint = False
      ShowHint = True
      TabOrder = 0
      Value = 0
      OnChange = seSumSpeedCheckTimeChange
    end
    object seSumSpeedMaxCount: TSpinEditEx
      Left = 87
      Top = 35
      Width = 93
      Height = 21
      Hint = #22312#25351#23450#26102#38388#20869#36229#36895#22788#29702#27425#25968'>="'#26368#22823#36229#36895#27425#25968'"'#65292#21017#35302#21457#32047#35745#36229#36895#22788#29702
      MaxValue = 0
      MinValue = 0
      ParentShowHint = False
      ShowHint = True
      TabOrder = 1
      Value = 0
      OnChange = seSumSpeedMaxCountChange
    end
  end
  object chkContinueSpeedCloseSocket: TCheckBox
    Left = 568
    Top = 533
    Width = 70
    Height = 17
    Hint = #24320#21551#36229#36895#25511#21046#26102#26377#25928#13#10#13#10#20165#23545#65306#25915#20987#12289#39764#27861#12289#36208#36335#12289#36305#27493#20197#21450#36716#21521#26377#25928
    Caption = #36830#32493#36229#36895
    ParentShowHint = False
    ShowHint = True
    TabOrder = 9
    OnClick = chkContinueSpeedCloseSocketClick
  end
  object seContinueSpeedCount: TSpinEditEx
    Left = 637
    Top = 531
    Width = 47
    Height = 21
    MaxValue = 8
    MinValue = 2
    ParentShowHint = False
    ShowHint = False
    TabOrder = 10
    Value = 2
    OnChange = seContinueSpeedCountChange
  end
  object seClientUploadPickItemsTime: TSpinEditEx
    Left = 150
    Top = 555
    Width = 59
    Height = 21
    Hint = 
      #23458#25143#31471#19978#20256#20869#25346#8220#29289#21697#8221#36873#39033#20013#8220#25342#21462#8221#12289#8220#29305#27530#8221#30340#29289#21697#21015#34920#21040'M2'#30340#19978#20256#38388#38548#13#10#13#10#26412#36873#39033#20462#25913#21518#19981#20250#20027#21160#36890#30693#23458#25143#31471#65292#35831#23613#37327#19981#35201#22312#29609#23478 +
      #36827#20837#28216#25103#21518#35843#25972
    MaxValue = 3000
    MinValue = 10
    ParentShowHint = False
    ShowHint = True
    TabOrder = 11
    Value = 0
    OnChange = seTakeOnItemTimeChange
  end
  object ilCheck: TImageList
    Height = 13
    Width = 13
    Left = 264
    Top = 120
    Bitmap = {
      494C01010200050014000D000D00FFFFFFFFFF10FFFFFFFFFFFFFFFF424D3600
      0000000000003600000028000000340000000D0000000100200000000000900A
      0000000000000000000000000000000000008F8F8E008F8F8E008F8F8E008F8F
      8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F
      8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F
      8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F8E00000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00008F8F8E00F4F4F400F4F4F400F4F4F400F4F4F400F4F4F400F4F4F400F4F4
      F400F4F4F400F4F4F400F4F4F400F4F4F4008F8F8E008F8F8E00F4F4F400F4F4
      F400F4F4F400F5F5F500F9F9F900F8F8F800F5F5F500F4F4F400F4F4F400F4F4
      F400F4F4F4008F8F8E0000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000008F8F8E00F4F4F400CCCBCA00D5D4
      D400DCDBDB00E1E1E000E7E7E600EBEBEA00ECECEB00ECEBEB00EAE9E900F4F4
      F4008F8F8E008F8F8E00F4F4F400CCCBCA00DBDADA00E9E2DF00BA998C00BD9D
      9000F6F3F200EDEDEC00ECEBEB00EAE9E900F4F4F4008F8F8E00000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00008F8F8E00F4F4F400C6C4C200E9E9E900EDEDED00F0F0F000F4F4F400F6F6
      F600F6F6F600F6F6F600E6E6E600F4F4F4008F8F8E008F8F8E00F4F4F400CAC8
      C600F0ECEA00BB998B00975F4A0098614C00D1B9B000F9F9F900F6F6F600E6E6
      E600F4F4F4008F8F8E0000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000008F8F8E00F4F4F400C2BFBC00E5E4
      E300E9E9E900EDEDED00F2F2F200F4F4F400F5F5F500F4F4F400E2E2E100F4F4
      F4008F8F8E008F8F8E00F4F4F400D1CFCD00E9E1DE00955D4800965F49009760
      4B00A4736100FAF9F800F4F4F400E2E2E100F4F4F4008F8F8E00000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00008F8F8E00F4F4F400BFBBB800E1DFDD00E5E5E400EAEAEA00EFEFEF00F2F2
      F200F2F2F200F2F2F200DEDDDC00F4F4F4008F8F8E008F8F8E00F4F4F400E1E0
      DE00AA7F6E00945C4700E2D4CF00A778670097604B00D5BFB700F6F6F600DEDD
      DC00F4F4F4008F8F8E0000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000008F8F8E00F4F4F400BCB7B200DCD8
      D500DFDCDA00E3E1E000E8E8E800ECECEC00EDEDED00EDEDED00D6D5D400F4F4
      F4008F8F8E008F8F8E00F4F4F400CDC9C500DDCFC900C8AEA300EEEEED00D5C1
      BA00965E4900A5766400F8F8F800D6D5D500F4F4F4008F8F8E00000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00008F8F8E00F4F4F400B9B3AE00D7D1CD00D9D4D000DBD7D400DFDDDB00E3E2
      E100E6E6E500E8E8E800CDCDCC00F4F4F4008F8F8E008F8F8E00F4F4F400B9B3
      AE00DDD9D500E5E2DF00DCD8D500F4F3F200A1715E00945C4700D6C3BC00DCDC
      DB00F4F4F4008F8F8E0000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000008F8F8E00F4F4F400B9B3AE00D5CF
      CB00D5CFCB00D6D1CD00DAD5D200DEDBD800E1DFDD00E4E3E200C8C7C600F4F4
      F4008F8F8E008F8F8E00F4F4F400B9B3AE00D5CFCB00D5CFCB00D6D1CD00E6E2
      E000CFB8AF00925A4500A5776500E8E7E700F4F4F4008F8F8E00000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00008F8F8E00F4F4F400B9B3AE00D5CFCB00D5CFCB00D5CFCB00D5CFCB00D8D3
      D000DCD8D500DFDDDB00C5C3C100F4F4F4008F8F8E008F8F8E00F4F4F400B9B3
      AE00D5CFCB00D5CFCB00D5CFCB00D6D0CC00F1EEED009D6A5700925A4400D0BF
      B900F6F6F6008F8F8E0000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000008F8F8E00F4F4F400B9B3AE00B9B3
      AE00B9B3AE00B9B3AE00B9B3AE00B9B3AE00BAB4AF00BDB9B400C1BEBB00F4F4
      F4008F8F8E008F8F8E00F4F4F400B9B3AE00B9B3AE00B9B3AE00B9B3AE00B9B3
      AE00D0CCC900C0A79D00AB867700E4DFDC00F5F5F5008F8F8E00000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00008F8F8E00F4F4F400F4F4F400F4F4F400F4F4F400F4F4F400F4F4F400F4F4
      F400F4F4F400F4F4F400F4F4F400F4F4F4008F8F8E008F8F8E00F4F4F400F4F4
      F400F4F4F400F4F4F400F4F4F400F4F4F400F4F4F400F8F8F800F9F9F900F6F6
      F600F4F4F4008F8F8E0000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000008F8F8E008F8F8E008F8F8E008F8F
      8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F
      8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F
      8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F8E00000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000424D3E000000000000003E00000028000000340000000D00000001000100
      00000000680000000000000000000000000000000000000000000000FFFFFF00
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000000000000000000000000000}
  end
end
