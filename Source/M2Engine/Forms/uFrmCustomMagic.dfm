object FrmCustomMagic: TFrmCustomMagic
  Left = 303
  Top = 205
  Caption = #33258#23450#20041#25216#33021
  ClientHeight = 689
  ClientWidth = 828
  Color = clBtnFace
  Constraints.MinHeight = 745
  Constraints.MinWidth = 850
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'Tahoma'
  Font.Style = []
  Position = poMainFormCenter
  OnCreate = FormCreate
  TextHeight = 13
  object grpMonster: TGroupBox
    Left = 0
    Top = 0
    Width = 118
    Height = 659
    Align = alLeft
    Caption = #25216#33021#21015#34920
    TabOrder = 0
    ExplicitHeight = 676
    object vstCustomMagic: TVirtualStringTree
      Left = 7
      Top = 19
      Width = 104
      Height = 600
      Colors.BorderColor = 15987699
      Colors.DisabledColor = clGray
      Colors.DropMarkColor = 15385233
      Colors.DropTargetColor = 15385233
      Colors.DropTargetBorderColor = 15987699
      Colors.FocusedSelectionColor = 15385233
      Colors.FocusedSelectionBorderColor = clWhite
      Colors.GridLineColor = 15987699
      Colors.HeaderHotColor = clBlack
      Colors.HotColor = clBlack
      Colors.SelectionRectangleBlendColor = 15385233
      Colors.SelectionRectangleBorderColor = 15385233
      Colors.SelectionTextColor = clBlack
      Colors.TreeLineColor = 9471874
      Colors.UnfocusedColor = 17814584
      Colors.UnfocusedSelectionColor = 15987699
      Colors.UnfocusedSelectionBorderColor = 15987699
      Header.AutoSizeIndex = 0
      Header.Font.Charset = DEFAULT_CHARSET
      Header.Font.Color = clWindowText
      Header.Font.Height = -11
      Header.Font.Name = 'Tahoma'
      Header.Font.Style = []
      Header.MainColumn = -1
      Indent = 0
      TabOrder = 0
      TreeOptions.PaintOptions = [toShowDropmark, toThemeAware, toUseBlendedImages]
      TreeOptions.SelectionOptions = [toFullRowSelect]
      OnDrawText = vstCustomMagicDrawText
      OnGetText = vstCustomMagicGetText
      OnGetNodeDataSize = vstCustomMagicGetNodeDataSize
      OnNodeClick = vstCustomMagicNodeClick
      Columns = <>
    end
  end
  object pgcMain: TPageControl
    Left = 118
    Top = 0
    Width = 710
    Height = 659
    ActivePage = tsAttack
    Align = alClient
    Font.Charset = GB2312_CHARSET
    Font.Color = clWindowText
    Font.Height = -12
    Font.Name = #23435#20307
    Font.Style = []
    ParentFont = False
    TabOrder = 1
    ExplicitWidth = 716
    ExplicitHeight = 676
    object tsAttack: TTabSheet
      Caption = #23458#25143#31471#35774#32622
      ImageIndex = 1
      object pgcClient: TPageControl
        Left = 1
        Top = 88
        Width = 703
        Height = 509
        ActivePage = tsBase
        TabOrder = 0
        object tsBase: TTabSheet
          Caption = #22522#26412#35774#32622
          object lbl1: TLabel
            Left = 5
            Top = 281
            Width = 104
            Height = 12
            Caption = #33258#23450#20041#21160#20316#35828#26126#65306
            Font.Charset = GB2312_CHARSET
            Font.Color = clBlue
            Font.Height = -12
            Font.Name = #23435#20307
            Font.Style = [fsBold]
            ParentFont = False
          end
          object Label31: TLabel
            Left = 17
            Top = 304
            Width = 540
            Height = 108
            Caption = 
              #26222#36890#21160#20316#65306#26222' '#36890' '#30733#65306#24320#22987#22270#29255'200'#65292#25773#25918#25968#37327'4'#65292#31354#30333#25968#37327'4'#65292#35835#21462'Hum.wzl'#65307#13#10#13#10#36830#20987#21160#20316#65306#20506#22825#36767#22320#65306#24320#22987#22270#29255'400'#65292#25773 +
              #25918#25968#37327'13'#65292#31354#30333#25968#37327'7'#65292#35835#21462'cboHum.wzl'#13#10#13#10#33258#23450#20041#21160#20316#26159#26681#25454#35282#33394#22806#35266#36827#34892#35835#21462#65292#27599#20214#34915#26381#32032#26448#30340#21160#20316#37117#26159#19968#26679#30340#13#10#13#10#35835 +
              #21462'Hum.wzl'#36824#26159'Hum?.wzl'#65292#26159#26681#25454#34915#26381#22806#35266#26469#33258#21160#21028#26029#65292#26080#38656#32416#32467'Hum2.wzl'#25110'cobHum2.wzl'#35813#22914#20309#35774#32622#13#10#13 +
              #10#34920' Magic '#20013#65292'EffectType = 0'#20026#25112#22763#25216#33021#65292#21542#21017#20026#38750#25112#22763#25216#33021
          end
          object grpClientBaseSetting: TGroupBox
            Left = 5
            Top = 4
            Width = 185
            Height = 92
            Caption = #25216#33021#22270#26631
            TabOrder = 0
            object Label50: TLabel
              Left = 9
              Top = 21
              Width = 60
              Height = 12
              Caption = #36164#28304#25991#20214#65306
            end
            object Label51: TLabel
              Left = 9
              Top = 45
              Width = 60
              Height = 12
              Caption = #24320#22987#22270#29255#65306
            end
            object lbl6: TLabel
              Left = 9
              Top = 68
              Width = 132
              Height = 12
              Caption = #25353#19979#22270#26631#20026#65306#24320#22987#22270#29255'+1'
              Font.Charset = GB2312_CHARSET
              Font.Color = clRed
              Font.Height = -12
              Font.Name = #23435#20307
              Font.Style = []
              ParentFont = False
            end
            object cbbClientIconFile: TComboBox
              Left = 66
              Top = 17
              Width = 114
              Height = 20
              Style = csDropDownList
              TabOrder = 0
              OnChange = cbbClientIconFileChange
            end
            object seClientIconIndex: TSpinEditEx
              Left = 66
              Top = 40
              Width = 114
              Height = 21
              MaxValue = 0
              MinValue = 0
              TabOrder = 1
              Value = 0
              OnChange = seClientIconIndexChange
            end
          end
          object GroupBox2: TGroupBox
            Left = 5
            Top = 102
            Width = 185
            Height = 164
            Caption = #22768#38899#25928#26524
            TabOrder = 1
            object Label225: TLabel
              Left = 9
              Top = 21
              Width = 90
              Height = 12
              Caption = #25112#22763#25216#33021' ('#30007')'#65306
            end
            object Label226: TLabel
              Left = 9
              Top = 45
              Width = 90
              Height = 12
              Caption = #25112#22763#25216#33021' ('#22899')'#65306
            end
            object lbl3: TLabel
              Left = 8
              Top = 69
              Width = 84
              Height = 12
              Caption = #20351#29992#39764#27861#22768#38899#65306
            end
            object Label227: TLabel
              Left = 8
              Top = 93
              Width = 84
              Height = 12
              Caption = #39764#27861#39134#34892#22768#38899#65306
            end
            object Label228: TLabel
              Left = 8
              Top = 117
              Width = 84
              Height = 12
              Caption = #39764#27861#29190#28856#22768#38899#65306
            end
            object Label205: TLabel
              Left = 8
              Top = 141
              Width = 84
              Height = 12
              Caption = #25216#33021#22833#36133#22768#38899#65306
            end
            object edtSound1: TEdit
              Left = 95
              Top = 17
              Width = 84
              Height = 20
              TabOrder = 0
              OnChange = edtSound1Change
            end
            object edtSound2: TEdit
              Tag = 1
              Left = 95
              Top = 41
              Width = 84
              Height = 20
              TabOrder = 1
              OnChange = edtSound1Change
            end
            object edtSound3: TEdit
              Tag = 2
              Left = 95
              Top = 65
              Width = 84
              Height = 20
              TabOrder = 2
              OnChange = edtSound1Change
            end
            object edtSound4: TEdit
              Tag = 3
              Left = 95
              Top = 89
              Width = 84
              Height = 20
              TabOrder = 3
              OnChange = edtSound1Change
            end
            object edtSound5: TEdit
              Tag = 4
              Left = 95
              Top = 113
              Width = 84
              Height = 20
              TabOrder = 4
              OnChange = edtSound1Change
            end
            object edtSound6: TEdit
              Tag = 5
              Left = 95
              Top = 137
              Width = 84
              Height = 20
              TabOrder = 5
              OnChange = edtSound1Change
            end
          end
        end
        object tsEffect: TTabSheet
          Caption = #25216#33021#25928#26524
          ImageIndex = 1
          object grpFly: TGroupBox
            Left = 178
            Top = 0
            Width = 162
            Height = 241
            Caption = #39134#34892#39764#27861#25928#26524
            TabOrder = 1
            object lbl5: TLabel
              Left = 9
              Top = 21
              Width = 60
              Height = 12
              Caption = #36164#28304#25991#20214#65306
            end
            object Label1: TLabel
              Left = 9
              Top = 43
              Width = 60
              Height = 12
              Caption = #24320#22987#22270#29255#65306
            end
            object Label2: TLabel
              Left = 9
              Top = 65
              Width = 60
              Height = 12
              Caption = #25773#25918#25968#37327#65306
            end
            object Label3: TLabel
              Left = 9
              Top = 87
              Width = 60
              Height = 12
              Caption = #31354#30333#25968#37327#65306
            end
            object Label4: TLabel
              Left = 9
              Top = 131
              Width = 60
              Height = 12
              Caption = #32472#21046#27169#24335#65306
            end
            object Label5: TLabel
              Left = 9
              Top = 153
              Width = 60
              Height = 12
              Caption = #26041#21521#25968#37327#65306
            end
            object Label15: TLabel
              Left = 9
              Top = 109
              Width = 60
              Height = 12
              Caption = #25773#25918#36895#24230#65306
            end
            object Label16: TLabel
              Left = 9
              Top = 175
              Width = 60
              Height = 12
              Caption = #29031#20142#33539#22260#65306
            end
            object cbbClientFlyFile: TComboBox
              Left = 66
              Top = 17
              Width = 88
              Height = 20
              Style = csDropDownList
              TabOrder = 0
              OnChange = cbbClientFlyFileChange
            end
            object cbbClientFlyDrawMode: TComboBox
              Left = 66
              Top = 127
              Width = 88
              Height = 20
              Style = csDropDownList
              TabOrder = 5
              OnChange = cbbClientFlyDrawModeChange
              Items.Strings = (
                #36879#26126#32472#21046
                #21407#22987#32472#21046)
            end
            object cbbClientFlyDirCount: TComboBox
              Left = 66
              Top = 149
              Width = 88
              Height = 20
              Style = csDropDownList
              TabOrder = 6
              OnChange = cbbClientFlyDirCountChange
            end
            object chkClientFlyCalcDir: TCheckBox
              Left = 9
              Top = 194
              Width = 116
              Height = 17
              Caption = #39134#34892#25928#26524#35745#31639#26041#21521
              TabOrder = 8
              OnClick = chkClientFlyCalcDirClick
            end
            object seClientFlyPlayTime: TSpinEditEx
              Left = 66
              Top = 105
              Width = 88
              Height = 21
              MaxValue = 0
              MinValue = 0
              TabOrder = 4
              Value = 0
              OnChange = seClientFlyPlayTimeChange
            end
            object seClientFlyEmptyCount: TSpinEditEx
              Left = 66
              Top = 83
              Width = 88
              Height = 21
              MaxValue = 0
              MinValue = 0
              TabOrder = 3
              Value = 0
              OnChange = seClientFlyEmptyCountChange
            end
            object seClientFlyPlayCount: TSpinEditEx
              Left = 66
              Top = 61
              Width = 88
              Height = 21
              MaxValue = 0
              MinValue = 0
              TabOrder = 2
              Value = 0
              OnChange = seClientFlyPlayCountChange
            end
            object seClientFlyStartIndex: TSpinEditEx
              Left = 66
              Top = 39
              Width = 88
              Height = 21
              MaxValue = 0
              MinValue = 0
              TabOrder = 1
              Value = 0
              OnChange = seClientFlyStartIndexChange
            end
            object seClientFlyLightRange: TSpinEditEx
              Left = 66
              Top = 171
              Width = 88
              Height = 21
              Hint = #29992#20110#35774#32622#40657#22812#25928#26524#26102#24618#29289#29031#20142#33539#22260#65292#20540#36234#22823#33539#22260#36234#22823#65307'0'#34920#31034#26080#29031#20142#25928#26524#65292#26080#40657#22812#21151#33021#26080#38656#20462#25913
              MaxValue = 5
              MinValue = 0
              ParentShowHint = False
              ShowHint = True
              TabOrder = 7
              Value = 0
              OnChange = seClientFlyLightRangeChange
            end
            object chkClientFlyFireGunMode: TCheckBox
              Left = 9
              Top = 214
              Width = 116
              Height = 17
              Hint = #22320#29425#28779#27169#24335#22270#29255#25968#37327#26368#23569#20026'3'#24352#13#10#13#10#21246#36873#22320#29425#28779#27169#24335#21518#65292#19981#25903#25345#30446#26631#25928#26524
              Caption = #22320#29425#28779#27169#24335#25773#25918
              ParentShowHint = False
              ShowHint = True
              TabOrder = 9
              OnClick = chkClientFlyFireGunModeClick
            end
          end
          object grpSelf: TGroupBox
            Left = 5
            Top = 0
            Width = 161
            Height = 288
            Caption = #33258#36523#25773#25918#39764#27861#25928#26524
            Font.Charset = DEFAULT_CHARSET
            Font.Color = clBlack
            Font.Height = -11
            Font.Name = 'Tahoma'
            Font.Style = []
            ParentFont = False
            TabOrder = 0
            object Label17: TLabel
              Left = 9
              Top = 21
              Width = 60
              Height = 13
              Caption = #36164#28304#25991#20214#65306
            end
            object Label18: TLabel
              Left = 9
              Top = 44
              Width = 60
              Height = 13
              Caption = #24320#22987#22270#29255#65306
            end
            object Label19: TLabel
              Left = 9
              Top = 87
              Width = 60
              Height = 13
              Caption = #22270#29255#25968#37327#65306
            end
            object Label20: TLabel
              Left = 9
              Top = 156
              Width = 60
              Height = 13
              Caption = #32472#21046#39034#24207#65306
            end
            object Label21: TLabel
              Left = 9
              Top = 134
              Width = 60
              Height = 13
              Caption = #25773#25918#36895#24230#65306
            end
            object Label22: TLabel
              Left = 9
              Top = 179
              Width = 60
              Height = 13
              Caption = #32472#21046#27169#24335#65306
            end
            object Label24: TLabel
              Left = 9
              Top = 111
              Width = 60
              Height = 13
              Caption = #31354#30333#25968#37327#65306
            end
            object Label25: TLabel
              Left = 9
              Top = 249
              Width = 60
              Height = 13
              Caption = #29031#20142#33539#22260#65306
            end
            object Label96: TLabel
              Left = 9
              Top = 202
              Width = 60
              Height = 13
              Caption = #26041#21521#25968#37327#65306
            end
            object Label99: TLabel
              Left = 9
              Top = 226
              Width = 60
              Height = 13
              Caption = #26041#21521#35745#31639#65306
            end
            object cbbClientSelfFile: TComboBox
              Left = 66
              Top = 17
              Width = 88
              Height = 21
              Style = csDropDownList
              TabOrder = 0
              OnChange = cbbClientSelfFileChange
            end
            object cbbClientSelfDrawOrder: TComboBox
              Left = 66
              Top = 152
              Width = 88
              Height = 21
              Style = csDropDownList
              TabOrder = 5
              OnChange = cbbClientSelfDrawOrderChange
              Items.Strings = (
                #20808#32472#33258#36523#20877#32472#39764#27861#25928#26524
                #20808#32472#39764#27861#25928#26524#20877#32472#33258#36523)
            end
            object seClientSelfPlayTime: TSpinEditEx
              Left = 66
              Top = 129
              Width = 88
              Height = 22
              Hint = #24403#24618#29289#33258#36523#25928#26524#25773#25918#19981#23436#25972#26102#65292#21487#20197#36866#24403#35843#24555#25773#25918#36895#24230
              MaxValue = 0
              MinValue = 0
              ParentShowHint = False
              ShowHint = True
              TabOrder = 4
              Value = 0
              OnChange = seClientSelfPlayTimeChange
            end
            object seClientSelfPlayCount: TSpinEditEx
              Left = 66
              Top = 84
              Width = 88
              Height = 22
              Hint = #24403#22270#29255#25968#37327#36807#22810#65292#25773#25918#26102#19981#33021#25773#25918#23436#25351#23450#24352#25968#12290#21487#20197#36866#24403#30340#35843#24555#25773#25918#36895#24230
              MaxValue = 0
              MinValue = 0
              ParentShowHint = False
              ShowHint = True
              TabOrder = 2
              Value = 0
              OnChange = seClientSelfPlayCountChange
            end
            object seClientSelfStartIndex: TSpinEditEx
              Left = 66
              Top = 40
              Width = 88
              Height = 22
              MaxValue = 0
              MinValue = 0
              TabOrder = 1
              Value = 0
              OnChange = seClientSelfStartIndexChange
            end
            object cbbClientSelfDrawMode: TComboBox
              Left = 66
              Top = 176
              Width = 88
              Height = 21
              Style = csDropDownList
              TabOrder = 6
              OnChange = cbbClientSelfDrawModeChange
              Items.Strings = (
                #36879#26126#32472#21046
                #21407#22987#32472#21046)
            end
            object seClientSelfEmptyCount: TSpinEditEx
              Left = 66
              Top = 107
              Width = 88
              Height = 22
              Hint = #29992#20110'8'#26041#21521#25915#20987#26102#35745#31639#26041#21521
              MaxValue = 0
              MinValue = 0
              ParentShowHint = False
              ShowHint = True
              TabOrder = 3
              Value = 0
              OnChange = seClientSelfEmptyCountChange
            end
            object cbbClientSelfDirCount: TComboBox
              Left = 66
              Top = 199
              Width = 87
              Height = 21
              Style = csDropDownList
              TabOrder = 7
              OnChange = cbbClientSelfDirCountChange
            end
            object chkClientSelfPlayDelayAction: TCheckBox
              Left = 8
              Top = 279
              Width = 153
              Height = 16
              Hint = #24403#20154#29289#21160#20316#26377#24310#32531#26102#65292#21435#25481#21246#36873#13#10#24403#20154#29289#33258#36523#25928#26524#25773#25918#19981#23436#25972#25110#26377#26102#19981#20986#30446#26631#25773#25918#25928#26524#65292#21246#36873#27492#39033
              Caption = #25773#25928#26524#26102#24310#32531#25915#20987#21160#20316
              Font.Charset = GB2312_CHARSET
              Font.Color = clWindowText
              Font.Height = -12
              Font.Name = #23435#20307
              Font.Style = [fsBold]
              ParentFont = False
              ParentShowHint = False
              ShowHint = True
              TabOrder = 10
              Visible = False
              OnClick = chkClientSelfPlayDelayActionClick
            end
            object seClientSelfLightRange: TSpinEditEx
              Left = 66
              Top = 244
              Width = 88
              Height = 22
              Hint = #29992#20110#35774#32622#40657#22812#25928#26524#26102#24618#29289#29031#20142#33539#22260#65292#20540#36234#22823#33539#22260#36234#22823#65307'0'#34920#31034#26080#29031#20142#25928#26524#65292#26080#40657#22812#21151#33021#26080#38656#20462#25913
              MaxValue = 5
              MinValue = 0
              ParentShowHint = False
              ShowHint = True
              TabOrder = 9
              Value = 0
              OnChange = seClientSelfLightRangeChange
            end
            object cbbClientSelfDirCalcType: TComboBox
              Left = 66
              Top = 221
              Width = 87
              Height = 21
              Hint = #27880#24847#65306#26222#36890#35745#31639#26041#24335#65292'16'#26041#21521#20063#25353'8'#26041#21521#35745#31639
              Style = csDropDownList
              ParentShowHint = False
              ShowHint = True
              TabOrder = 8
              OnChange = cbbClientSelfDirCalcTypeChange
              Items.Strings = (
                #19981#20998#26041#21521
                #26222#36890#35745#31639
                #33258#25105#20013#24515#22810#26041#21521)
            end
            object chkClientSelfPlayFailNoDraw: TCheckBox
              Left = 8
              Top = 269
              Width = 149
              Height = 17
              Hint = #21246#36873#34920#31034#25216#33021#37322#25918#25928#26524#21644#21160#20316#19981#21516#27493#22788#29702#65292#19968#33324#29992#20110#30452#32447#39764#27861#25928#26524#21246#36873#65288#22914#65306#30142#20809#30005#24433#65292#24403#25216#33021'CD'#26102#38388#19981#21040#26102#65292#19981#26174#31034#30452#32447#39764#27861#65289
              Caption = #25216#33021#22833#36133#30340#26102#20505#19981#32472#21046
              Font.Charset = DEFAULT_CHARSET
              Font.Color = clNavy
              Font.Height = -11
              Font.Name = 'Tahoma'
              Font.Style = [fsBold]
              ParentFont = False
              ParentShowHint = False
              ShowHint = True
              TabOrder = 11
              OnClick = chkClientSelfPlayFailNoDrawClick
            end
            object chkSelf_SyncHumAction: TCheckBox
              Left = 12
              Top = 64
              Width = 145
              Height = 16
              Hint = #21246#36873#27492#36873#39033#21518#65292#22270#29255#25968#37327#19982#25773#25918#36895#24230#22343#20197#20154#29289#33258#36523#21160#20316#20026#20934#12290
              Caption = #19982#20154#29289#30340#21160#20316#21516#27493#25773#25918
              ParentShowHint = False
              ShowHint = True
              TabOrder = 12
              OnClick = chkSelf_SyncHumActionClick
            end
          end
          object grpTarget: TGroupBox
            Left = 525
            Top = 0
            Width = 161
            Height = 395
            Caption = #30446#26631#25773#25918#25928#26524
            TabOrder = 3
            object Label37: TLabel
              Left = 9
              Top = 21
              Width = 60
              Height = 12
              Caption = #36164#28304#25991#20214#65306
            end
            object Label38: TLabel
              Left = 9
              Top = 43
              Width = 60
              Height = 12
              Caption = #24320#22987#22270#29255#65306
            end
            object Label39: TLabel
              Left = 9
              Top = 87
              Width = 60
              Height = 12
              Caption = #22270#29255#25968#37327#65306
            end
            object Label40: TLabel
              Left = 9
              Top = 110
              Width = 60
              Height = 12
              Caption = #25773#25918#36895#24230#65306
            end
            object Label41: TLabel
              Left = 9
              Top = 131
              Width = 60
              Height = 12
              Caption = #32472#21046#27169#24335#65306
            end
            object Label42: TLabel
              Left = 9
              Top = 174
              Width = 60
              Height = 12
              Caption = #29031#20142#33539#22260#65306
            end
            object Label43: TLabel
              Left = 9
              Top = 265
              Width = 60
              Height = 12
              Caption = #25345#32493#26102#38388#65306
            end
            object Label44: TLabel
              Left = 9
              Top = 331
              Width = 60
              Height = 12
              Caption = #20260#23475#33539#22260#65306
            end
            object Label45: TLabel
              Left = 9
              Top = 308
              Width = 60
              Height = 12
              Caption = #20260#23475#38388#38548#65306
            end
            object Label47: TLabel
              Left = 143
              Top = 265
              Width = 12
              Height = 12
              Caption = #31186
            end
            object bvl1: TBevel
              Left = 4
              Top = 234
              Width = 154
              Height = 3
              Shape = bsBottomLine
            end
            object Label236: TLabel
              Left = 9
              Top = 353
              Width = 60
              Height = 12
              Caption = #29031#20142#33539#22260#65306
            end
            object Label334: TLabel
              Left = 9
              Top = 287
              Width = 60
              Height = 12
              Caption = #26102#38388#36882#22686#65306
            end
            object Label335: TLabel
              Left = 131
              Top = 287
              Width = 24
              Height = 12
              Caption = #27627#31186
            end
            object Label212: TLabel
              Left = 3
              Top = 65
              Width = 66
              Height = 12
              Caption = #24320#22987#22270#29255'2'#65306
            end
            object Label237: TLabel
              Left = 3
              Top = 152
              Width = 66
              Height = 12
              Caption = #32472#21046#27169#24335'2'#65306
            end
            object Label200: TLabel
              Left = 143
              Top = 331
              Width = 12
              Height = 12
              Caption = #26684
            end
            object Label46: TLabel
              Left = 143
              Top = 308
              Width = 12
              Height = 12
              Caption = #31186
            end
            object cbbClientTargetFile: TComboBox
              Left = 66
              Top = 17
              Width = 88
              Height = 20
              Style = csDropDownList
              TabOrder = 0
              OnChange = cbbClientTargetFileChange
            end
            object seClientTargetPlayTime: TSpinEditEx
              Left = 66
              Top = 105
              Width = 88
              Height = 21
              MaxValue = 0
              MinValue = 0
              TabOrder = 4
              Value = 0
              OnChange = seClientTargetPlayTimeChange
            end
            object seClientTargetPlayCount: TSpinEditEx
              Left = 66
              Top = 84
              Width = 88
              Height = 21
              MaxValue = 0
              MinValue = 0
              TabOrder = 3
              Value = 0
              OnChange = seClientTargetPlayCountChange
            end
            object seClientTargetStartIndex: TSpinEditEx
              Left = 66
              Top = 39
              Width = 88
              Height = 21
              MaxValue = 0
              MinValue = 0
              TabOrder = 1
              Value = 0
              OnChange = seClientTargetStartIndexChange
            end
            object cbbClientTargetDrawMode: TComboBox
              Left = 66
              Top = 127
              Width = 88
              Height = 20
              Style = csDropDownList
              TabOrder = 5
              OnChange = cbbClientTargetDrawModeChange
              Items.Strings = (
                #36879#26126#32472#21046
                #21407#22987#32472#21046)
            end
            object chkClientTargetMultiPlay: TCheckBox
              Left = 8
              Top = 193
              Width = 80
              Height = 17
              Caption = #22810#30446#26631#25773#25918
              TabOrder = 8
              OnClick = chkClientTargetMultiPlayClick
            end
            object chkClientTargetLockDraw: TCheckBox
              Left = 8
              Top = 215
              Width = 116
              Height = 17
              Caption = #25928#26524#22987#32456#36319#38543#30446#26631
              TabOrder = 9
              OnClick = chkClientTargetLockDrawClick
            end
            object seClientTargetLightRange: TSpinEditEx
              Left = 66
              Top = 170
              Width = 88
              Height = 21
              Hint = #29992#20110#35774#32622#40657#22812#25928#26524#26102#33258#36523#29031#20142#33539#22260#65292#20540#36234#22823#33539#22260#36234#22823#65307'0'#34920#31034#26080#29031#20142#25928#26524#12290#26080#40657#22812#21151#33021#26080#38656#20462#25913
              MaxValue = 5
              MinValue = 0
              ParentShowHint = False
              ShowHint = True
              TabOrder = 7
              Value = 0
              OnChange = seClientTargetLightRangeChange
            end
            object chkClientTargetKeepPlay: TCheckBox
              Left = 8
              Top = 241
              Width = 97
              Height = 17
              Hint = #25928#26524#25345#32493#25773#25918#31867#22411#20110#28779#22681#25928#26524#65292#22312#22320#38754#19978#26174#31034#19968#27573#29305#25928#65292#25932#26041#36827#20837#21518#65292#20250#25345#32493#20943#23545#26041#20260#23475#13#10#13#10#26412#36873#39033#21246#36873#21518#65292#25928#26524#22987#32456#36319#38543#30446#26631#23558#26080#25928
              Caption = #25928#26524#25345#32493#25773#25918
              ParentShowHint = False
              ShowHint = True
              TabOrder = 10
              OnClick = chkClientTargetKeepPlayClick
            end
            object seClientTargetKeepTime: TSpinEditEx
              Left = 66
              Top = 260
              Width = 76
              Height = 21
              MaxValue = 65535
              MinValue = 1
              TabOrder = 11
              Value = 5
              OnChange = seClientTargetKeepTimeChange
            end
            object seClientTargetKeepAttackRange: TSpinEditEx
              Left = 66
              Top = 327
              Width = 76
              Height = 21
              Hint = 
                #21322#24452#33539#22260#65307'0'#65306#34920#31034#30446#26631#22352#26631#65292'1'#65306#30446#26631#21450#21608#36793#19968#26684#20849'9'#26684' ( '#20260#23475#33539#22260' = ('#21322#24452#33539#22260'*2 + 1)'#30340#24179#26041' )'#13#10#24403#21322#24452#33539#22260#22823#20110'0'#26102 +
                #65292#19988#19981#21246#36873#27599#26684#21333#29420#25773#25918#26102#65292#20960#20010#30456#37051#30340#25345#32493#25773#25918#20250#23545#30446#26631#20135#29983#20260#23475#21472#21152
              MaxValue = 8
              MinValue = 0
              ParentShowHint = False
              ShowHint = True
              TabOrder = 13
              Value = 0
              OnChange = seClientTargetKeepAttackRangeChange
            end
            object seClientTargetKeepAttackInterval: TSpinEditEx
              Left = 66
              Top = 305
              Width = 76
              Height = 21
              MaxValue = 255
              MinValue = 1
              TabOrder = 12
              Value = 1
              OnChange = seClientTargetKeepAttackIntervalChange
            end
            object chkClientTargetKeepMultiPlay: TCheckBox
              Left = 10
              Top = 375
              Width = 96
              Height = 16
              Hint = #22914#26524#20260#23475#33539#22260#20026'1('#21363'9'#26684#33539#22260')'#65292#33509#21246#36873#27492#36873#39033#65292#21017'9'#26684#27599#26684#37117#25773#25918#25928#26524#65292#21542#21017'9'#26684#33539#22260#21482#20849'1'#20010#25928#26524
              Caption = #27599#26684#21333#29420#25773#25918
              Font.Charset = GB2312_CHARSET
              Font.Color = clWindowText
              Font.Height = -12
              Font.Name = #23435#20307
              Font.Style = [fsBold]
              ParentFont = False
              TabOrder = 15
              OnClick = chkClientTargetKeepMultiPlayClick
            end
            object seTargetKeepLightRange: TSpinEditEx
              Left = 66
              Top = 349
              Width = 88
              Height = 21
              Hint = #29992#20110#35774#32622#40657#22812#25928#26524#26102#33258#36523#29031#20142#33539#22260#65292#20540#36234#22823#33539#22260#36234#22823#65307'0'#34920#31034#26080#29031#20142#25928#26524#12290#26080#40657#22812#21151#33021#26080#38656#20462#25913
              MaxValue = 5
              MinValue = 0
              ParentShowHint = False
              ShowHint = True
              TabOrder = 14
              Value = 0
              OnChange = seTargetKeepLightRangeChange
            end
            object seClientTargetKeepTime2: TSpinEditEx
              Left = 66
              Top = 282
              Width = 61
              Height = 21
              MaxValue = 1000000
              MinValue = 0
              TabOrder = 16
              Value = 5
              OnChange = seClientTargetKeepTime2Change
            end
            object seClientTargetStartIndex2: TSpinEditEx
              Left = 66
              Top = 61
              Width = 88
              Height = 21
              Hint = '>=0'#21017#24320#21551#29305#25928'2'#65292#29305#25928'1'#21644#29305#25928'2'#21516#27493#25773#25918
              MaxValue = 0
              MinValue = 0
              ParentShowHint = False
              ShowHint = True
              TabOrder = 2
              Value = 0
              OnChange = seClientTargetStartIndex2Change
            end
            object cbbClientTargetDrawMode2: TComboBox
              Left = 66
              Top = 149
              Width = 88
              Height = 20
              Hint = #29305#25928'2'#32472#21046#27169#24335
              Style = csDropDownList
              ParentShowHint = False
              ShowHint = True
              TabOrder = 6
              OnChange = cbbClientTargetDrawMode2Change
              Items.Strings = (
                #36879#26126#32472#21046
                #21407#22987#32472#21046)
            end
          end
          object grpFlyEff: TGroupBox
            Left = 178
            Top = 246
            Width = 162
            Height = 87
            Hint = #31867#20284#20110'magic6.wzl'#20013#30340'140'#39134#34892#31526#23545#24212#30340'310'#39134#34892#31526#29305#25928
            Caption = #39134#34892#39764#27861#29305#25928
            ParentShowHint = False
            ShowHint = True
            TabOrder = 5
            object Label14: TLabel
              Left = 9
              Top = 21
              Width = 60
              Height = 12
              Caption = #36164#28304#25991#20214#65306
            end
            object Label48: TLabel
              Left = 9
              Top = 43
              Width = 60
              Height = 12
              Caption = #24320#22987#22270#29255#65306
            end
            object Label49: TLabel
              Left = 9
              Top = 65
              Width = 60
              Height = 12
              Caption = #32472#21046#27169#24335#65306
            end
            object cbbClientFlyEffFile: TComboBox
              Left = 66
              Top = 17
              Width = 88
              Height = 20
              Style = csDropDownList
              TabOrder = 0
              OnChange = cbbClientFlyEffFileChange
            end
            object seClientFlyEffStartIndex: TSpinEditEx
              Left = 66
              Top = 39
              Width = 88
              Height = 21
              MaxValue = 0
              MinValue = 0
              TabOrder = 1
              Value = 0
              OnChange = seClientFlyEffStartIndexChange
            end
            object cbbClientFlyEffDrawMode: TComboBox
              Left = 66
              Top = 61
              Width = 88
              Height = 20
              Style = csDropDownList
              TabOrder = 2
              OnChange = cbbClientFlyEffDrawModeChange
              Items.Strings = (
                #36879#26126#32472#21046
                #21407#22987#32472#21046)
            end
          end
          object GroupBox1: TGroupBox
            Left = 352
            Top = 220
            Width = 161
            Height = 261
            Caption = #30446#26631#25773#25918#25928#26524#8212#12304#21069#22863#12305
            TabOrder = 4
            object Label26: TLabel
              Left = 9
              Top = 20
              Width = 60
              Height = 12
              Caption = #36164#28304#25991#20214#65306
            end
            object Label27: TLabel
              Left = 9
              Top = 43
              Width = 60
              Height = 12
              Caption = #24320#22987#22270#29255#65306
            end
            object Label28: TLabel
              Left = 9
              Top = 87
              Width = 60
              Height = 12
              Caption = #22270#29255#25968#37327#65306
            end
            object Label29: TLabel
              Left = 9
              Top = 151
              Width = 60
              Height = 12
              Caption = #25773#25918#36895#24230#65306
            end
            object Label30: TLabel
              Left = 9
              Top = 173
              Width = 60
              Height = 12
              Caption = #32472#21046#27169#24335#65306
            end
            object Label36: TLabel
              Left = 9
              Top = 218
              Width = 60
              Height = 12
              Caption = #29031#20142#33539#22260#65306
            end
            object Label209: TLabel
              Left = 4
              Top = 65
              Width = 66
              Height = 12
              Caption = #24320#22987#22270#29255'2'#65306
            end
            object Label213: TLabel
              Left = 4
              Top = 195
              Width = 66
              Height = 12
              Caption = #32472#21046#27169#24335'2'#65306
            end
            object Label86: TLabel
              Left = 9
              Top = 112
              Width = 60
              Height = 12
              Caption = #31354#30333#25968#37327#65306
            end
            object cbbClientPreTargetFile: TComboBox
              Left = 66
              Top = 17
              Width = 88
              Height = 20
              Style = csDropDownList
              TabOrder = 0
              OnChange = cbbClientPreTargetFileChange
            end
            object seClientPreTargetPlayTime: TSpinEditEx
              Left = 66
              Top = 148
              Width = 88
              Height = 21
              MaxValue = 0
              MinValue = 0
              TabOrder = 6
              Value = 0
              OnChange = seClientPreTargetPlayTimeChange
            end
            object seClientPreTargetPlayCount: TSpinEditEx
              Left = 66
              Top = 84
              Width = 88
              Height = 21
              MaxValue = 0
              MinValue = 0
              TabOrder = 3
              Value = 0
              OnChange = seClientPreTargetPlayCountChange
            end
            object seClientPreTargetStartIndex: TSpinEditEx
              Left = 66
              Top = 38
              Width = 88
              Height = 21
              MaxValue = 0
              MinValue = 0
              TabOrder = 1
              Value = 0
              OnChange = seClientPreTargetStartIndexChange
            end
            object cbbClientPreTargetDrawMode: TComboBox
              Left = 66
              Top = 170
              Width = 88
              Height = 20
              Style = csDropDownList
              TabOrder = 7
              OnChange = cbbClientPreTargetDrawModeChange
              Items.Strings = (
                #36879#26126#32472#21046
                #21407#22987#32472#21046)
            end
            object chkClientPreTargetLockDraw: TCheckBox
              Left = 8
              Top = 238
              Width = 116
              Height = 17
              Caption = #25928#26524#22987#32456#36319#38543#30446#26631
              TabOrder = 10
              OnClick = chkClientPreTargetLockDrawClick
            end
            object seClientPreTargetLightRange: TSpinEditEx
              Left = 66
              Top = 214
              Width = 88
              Height = 21
              Hint = #29992#20110#35774#32622#40657#22812#25928#26524#26102#33258#36523#29031#20142#33539#22260#65292#20540#36234#22823#33539#22260#36234#22823#65307'0'#34920#31034#26080#29031#20142#25928#26524#12290#26080#40657#22812#21151#33021#26080#38656#20462#25913
              MaxValue = 5
              MinValue = 0
              ParentShowHint = False
              ShowHint = True
              TabOrder = 9
              Value = 0
              OnChange = seClientPreTargetLightRangeChange
            end
            object seClientPreTargetStartIndex2: TSpinEditEx
              Left = 65
              Top = 61
              Width = 88
              Height = 21
              Hint = '>=0'#21017#24320#21551#29305#25928'2'#65292#29305#25928'1'#21644#29305#25928'2'#21516#27493#25773#25918
              MaxValue = 0
              MinValue = 0
              ParentShowHint = False
              ShowHint = True
              TabOrder = 2
              Value = 0
              OnChange = seClientPreTargetStartIndex2Change
            end
            object cbbClientPreTargetDrawMode2: TComboBox
              Left = 66
              Top = 192
              Width = 88
              Height = 20
              Hint = #29305#25928'2'#32472#21046#27169#24335
              Style = csDropDownList
              ParentShowHint = False
              ShowHint = True
              TabOrder = 8
              OnChange = cbbClientPreTargetDrawMode2Change
              Items.Strings = (
                #36879#26126#32472#21046
                #21407#22987#32472#21046)
            end
            object seClientPreTargetEmptyCount: TSpinEditEx
              Left = 66
              Top = 107
              Width = 88
              Height = 21
              Hint = #27492#36873#39033#21482#23545#25112#22763#25216#33021#26377#25928
              MaxValue = 0
              MinValue = 0
              ParentShowHint = False
              ShowHint = True
              TabOrder = 4
              Value = 0
              OnChange = seClientPreTargetEmptyCountChange
            end
            object chkClientPreTargetCalcDir: TCheckBox
              Left = 55
              Top = 130
              Width = 97
              Height = 17
              Hint = #27492#36873#39033#21482#23545#25112#22763#25216#33021#26377#25928#13#10#26041#21521#20026#20154#29289#25915#20987#30446#26631#30340#26041#21521#65292#32780#38750#30446#26631#33258#24049#30340#26041#21521#13#10#27492#36873#39033#20027#35201#29992#20110#24320#22825#26025#21629#20013#21518#26029#20912#29305#25928
              Caption = #35745#31639#25915#20987#26041#21521
              ParentShowHint = False
              ShowHint = True
              TabOrder = 5
              OnClick = chkClientPreTargetCalcDirClick
            end
          end
          object grpFastMove: TGroupBox
            Left = 352
            Top = 0
            Width = 161
            Height = 218
            Caption = #30636#31227#25928#26524
            TabOrder = 2
            object Label218: TLabel
              Left = 9
              Top = 21
              Width = 60
              Height = 12
              Caption = #36164#28304#25991#20214#65306
            end
            object Label219: TLabel
              Left = 9
              Top = 43
              Width = 60
              Height = 12
              Caption = #24320#22987#22270#29255#65306
            end
            object Label220: TLabel
              Left = 9
              Top = 65
              Width = 60
              Height = 12
              Caption = #22270#29255#25968#37327#65306
            end
            object Label222: TLabel
              Left = 9
              Top = 109
              Width = 60
              Height = 12
              Caption = #25773#25918#36895#24230#65306
            end
            object Label223: TLabel
              Left = 9
              Top = 131
              Width = 60
              Height = 12
              Caption = #32472#21046#27169#24335#65306
            end
            object Label224: TLabel
              Left = 9
              Top = 87
              Width = 60
              Height = 12
              Caption = #31354#30333#25968#37327#65306
            end
            object Label221: TLabel
              Left = 9
              Top = 153
              Width = 60
              Height = 12
              Caption = #29031#20142#33539#22260#65306
            end
            object cbbClientFastMoveFile: TComboBox
              Left = 66
              Top = 17
              Width = 88
              Height = 20
              Style = csDropDownList
              TabOrder = 0
              OnChange = cbbClientFastMoveFileChange
            end
            object seClientFastMovePlayTime: TSpinEditEx
              Left = 66
              Top = 105
              Width = 88
              Height = 21
              MaxValue = 0
              MinValue = 0
              TabOrder = 4
              Value = 0
              OnChange = seClientFastMovePlayTimeChange
            end
            object seClientFastMovePlayCount: TSpinEditEx
              Left = 66
              Top = 61
              Width = 88
              Height = 21
              MaxValue = 0
              MinValue = 0
              TabOrder = 2
              Value = 0
              OnChange = seClientFastMovePlayCountChange
            end
            object seClientFastMoveStartIndex: TSpinEditEx
              Left = 66
              Top = 39
              Width = 88
              Height = 21
              MaxValue = 0
              MinValue = 0
              TabOrder = 1
              Value = 0
              OnChange = seClientFastMoveStartIndexChange
            end
            object cbbClientFastMoveDrawMode: TComboBox
              Left = 66
              Top = 127
              Width = 88
              Height = 20
              Style = csDropDownList
              TabOrder = 5
              OnChange = cbbClientFastMoveDrawModeChange
              Items.Strings = (
                #36879#26126#32472#21046
                #21407#22987#32472#21046)
            end
            object seClientFastMoveEmptyCount: TSpinEditEx
              Left = 66
              Top = 83
              Width = 88
              Height = 21
              Hint = #29992#20110'8'#26041#21521#25915#20987#26102#35745#31639#26041#21521
              MaxValue = 0
              MinValue = 0
              ParentShowHint = False
              ShowHint = True
              TabOrder = 3
              Value = 0
              OnChange = seClientFastMoveEmptyCountChange
            end
            object chkClientFastMoveCalcDir: TCheckBox
              Left = 8
              Top = 172
              Width = 105
              Height = 17
              Caption = #25928#26524#35745#31639#26041#21521
              Font.Charset = DEFAULT_CHARSET
              Font.Color = clWindowText
              Font.Height = -11
              Font.Name = 'Tahoma'
              Font.Style = []
              ParentFont = False
              ParentShowHint = False
              ShowHint = True
              TabOrder = 7
              OnClick = chkClientFastMoveCalcDirClick
            end
            object seFastMoveLightRange: TSpinEditEx
              Left = 66
              Top = 149
              Width = 88
              Height = 21
              Hint = #29992#20110#35774#32622#40657#22812#25928#26524#26102#33258#36523#29031#20142#33539#22260#65292#20540#36234#22823#33539#22260#36234#22823#65307'0'#34920#31034#26080#29031#20142#25928#26524#12290#26080#40657#22812#21151#33021#26080#38656#20462#25913
              MaxValue = 5
              MinValue = 0
              ParentShowHint = False
              ShowHint = True
              TabOrder = 6
              Value = 0
              OnChange = seFastMoveLightRangeChange
            end
            object chkClientFastMoveNoHitAction: TCheckBox
              Left = 8
              Top = 193
              Width = 129
              Height = 17
              Caption = #30636#31227#25915#20987#26080#30733#21160#20316
              Font.Charset = DEFAULT_CHARSET
              Font.Color = clWindowText
              Font.Height = -11
              Font.Name = 'Tahoma'
              Font.Style = []
              ParentFont = False
              ParentShowHint = False
              ShowHint = True
              TabOrder = 8
              OnClick = chkClientFastMoveNoHitActionClick
            end
          end
          object GroupBox4: TGroupBox
            Left = 5
            Top = 291
            Width = 161
            Height = 180
            Caption = #33258#36523#25345#32493#25773#25918#39764#27861#29305#25928
            Font.Charset = DEFAULT_CHARSET
            Font.Color = clBlack
            Font.Height = -11
            Font.Name = 'Tahoma'
            Font.Style = []
            ParentFont = False
            TabOrder = 6
            object Label229: TLabel
              Left = 9
              Top = 21
              Width = 60
              Height = 13
              Caption = #36164#28304#25991#20214#65306
            end
            object Label230: TLabel
              Left = 9
              Top = 44
              Width = 60
              Height = 13
              Caption = #24320#22987#22270#29255#65306
            end
            object Label231: TLabel
              Left = 9
              Top = 67
              Width = 60
              Height = 13
              Caption = #22270#29255#25968#37327#65306
            end
            object Label233: TLabel
              Left = 9
              Top = 90
              Width = 60
              Height = 13
              Caption = #25773#25918#36895#24230#65306
            end
            object Label234: TLabel
              Left = 9
              Top = 113
              Width = 60
              Height = 13
              Caption = #32472#21046#27169#24335#65306
            end
            object Label232: TLabel
              Left = 9
              Top = 136
              Width = 60
              Height = 13
              Caption = #25345#32493#26102#38388#65306
            end
            object Label235: TLabel
              Left = 142
              Top = 136
              Width = 12
              Height = 13
              Caption = #31186
            end
            object Label336: TLabel
              Left = 9
              Top = 159
              Width = 60
              Height = 13
              Caption = #26102#38388#36882#22686#65306
            end
            object Label337: TLabel
              Left = 142
              Top = 159
              Width = 12
              Height = 13
              Caption = #31186
            end
            object cbbClientSelfKeepFile: TComboBox
              Left = 66
              Top = 17
              Width = 88
              Height = 21
              Style = csDropDownList
              TabOrder = 0
              OnChange = cbbClientSelfKeepFileChange
            end
            object seClientSelfKeepPlayTime: TSpinEditEx
              Left = 66
              Top = 86
              Width = 88
              Height = 22
              MaxValue = 0
              MinValue = 0
              TabOrder = 3
              Value = 0
              OnChange = seClientSelfKeepPlayTimeChange
            end
            object seClientSelfKeepPlayCount: TSpinEditEx
              Left = 66
              Top = 63
              Width = 88
              Height = 22
              MaxValue = 0
              MinValue = 0
              TabOrder = 2
              Value = 0
              OnChange = seClientSelfKeepPlayCountChange
            end
            object seClientSelfKeepStartIndex: TSpinEditEx
              Left = 66
              Top = 40
              Width = 88
              Height = 22
              MaxValue = 0
              MinValue = 0
              TabOrder = 1
              Value = 0
              OnChange = seClientSelfKeepStartIndexChange
            end
            object cbbClientSelfKeepDrawMode: TComboBox
              Left = 66
              Top = 109
              Width = 88
              Height = 21
              Style = csDropDownList
              TabOrder = 4
              OnChange = cbbClientSelfKeepDrawModeChange
              Items.Strings = (
                #36879#26126#32472#21046
                #21407#22987#32472#21046)
            end
            object seClientSelfKeepTime: TSpinEditEx
              Left = 66
              Top = 132
              Width = 74
              Height = 22
              MaxValue = 65535
              MinValue = 0
              TabOrder = 5
              Value = 5
              OnChange = seClientSelfKeepTimeChange
            end
            object seClientSelfKeepTime2: TSpinEditEx
              Left = 66
              Top = 155
              Width = 74
              Height = 22
              MaxValue = 0
              MinValue = 0
              TabOrder = 6
              Value = 5
              OnChange = seClientSelfKeepTime2Change
            end
          end
        end
        object tsTargetStatus: TTabSheet
          Caption = #30446#26631#29366#24577
          ImageIndex = 2
          object lbl7: TLabel
            Left = 7
            Top = 453
            Width = 528
            Height = 12
            Caption = #30446#26631#29366#24577#19981#21516#20110#30446#26631#25928#26524#65292#30446#26631#25928#26524#21482#26159#23436#25104#19968#27425#25773#25918#65292#32780#30446#26631#29366#24577#26159#22312#26377#25928#30340#26102#38388#20869#20276#38543#30446#26631#23384#22312
          end
          object Label58: TLabel
            Left = 7
            Top = 431
            Width = 420
            Height = 12
            Caption = #30446#26631#29366#24577#20027#35201#29992#20110#23454#29616#31867#20284#39764#27861#30462#25928#26524#65292#20351#29992#30446#26631#29366#24577#26102#65292#19981#24314#35758#20351#29992#25345#32493#25773#25918
          end
          object GroupBox11: TGroupBox
            Left = 2
            Top = 5
            Width = 161
            Height = 160
            Caption = #26174#31034#25928#26524
            TabOrder = 0
            object Label177: TLabel
              Left = 9
              Top = 20
              Width = 60
              Height = 12
              Caption = #36164#28304#25991#20214#65306
            end
            object Label178: TLabel
              Left = 9
              Top = 44
              Width = 60
              Height = 12
              Caption = #24320#22987#22270#29255#65306
            end
            object Label179: TLabel
              Left = 9
              Top = 67
              Width = 60
              Height = 12
              Caption = #25773#25918#25968#37327#65306
            end
            object Label180: TLabel
              Left = 9
              Top = 91
              Width = 60
              Height = 12
              Caption = #31354#30333#25968#37327#65306
            end
            object Label181: TLabel
              Left = 9
              Top = 114
              Width = 60
              Height = 12
              Caption = #32472#21046#27169#24335#65306
            end
            object Label186: TLabel
              Left = 9
              Top = 182
              Width = 60
              Height = 12
              Caption = #25773#25918#36895#24230#65306
              Visible = False
            end
            object cbbTargetStatus1_File: TComboBox
              Left = 66
              Top = 16
              Width = 88
              Height = 20
              Style = csDropDownList
              TabOrder = 0
              OnChange = cbbTargetStatus1_FileChange
            end
            object cbbTargetStatus1_DrawMode: TComboBox
              Left = 66
              Top = 111
              Width = 88
              Height = 20
              Style = csDropDownList
              TabOrder = 5
              OnChange = cbbTargetStatus1_DrawModeChange
              Items.Strings = (
                #36879#26126#32472#21046
                #21407#22987#32472#21046)
            end
            object seTargetStatus1_PlayTime: TSpinEditEx
              Left = 66
              Top = 178
              Width = 88
              Height = 21
              MaxValue = 0
              MinValue = 0
              TabOrder = 4
              Value = 0
              Visible = False
              OnChange = seTargetStatus1_PlayTimeChange
            end
            object seTargetStatus1_EmptyCount: TSpinEditEx
              Left = 66
              Top = 87
              Width = 88
              Height = 21
              MaxValue = 0
              MinValue = 0
              TabOrder = 3
              Value = 0
              OnChange = seTargetStatus1_EmptyCountChange
            end
            object seTargetStatus1_PlayCount: TSpinEditEx
              Left = 66
              Top = 63
              Width = 88
              Height = 21
              MaxValue = 0
              MinValue = 0
              TabOrder = 2
              Value = 0
              OnChange = seTargetStatus1_PlayCountChange
            end
            object seTargetStatus1_StartIndex: TSpinEditEx
              Left = 66
              Top = 39
              Width = 88
              Height = 21
              MaxValue = 0
              MinValue = 0
              TabOrder = 1
              Value = 0
              OnChange = seTargetStatus1_StartIndexChange
            end
            object chkTargetStatus1_CalcDir: TCheckBox
              Left = 67
              Top = 137
              Width = 83
              Height = 16
              Caption = #35745#31639#26041#21521
              TabOrder = 6
              OnClick = chkTargetStatus1_CalcDirClick
            end
          end
          object GroupBox13: TGroupBox
            Left = 169
            Top = 5
            Width = 162
            Height = 160
            Caption = #21463#25915#20987#25928#26524
            TabOrder = 1
            object Label190: TLabel
              Left = 9
              Top = 20
              Width = 60
              Height = 12
              Caption = #36164#28304#25991#20214#65306
            end
            object Label191: TLabel
              Left = 9
              Top = 44
              Width = 60
              Height = 12
              Caption = #24320#22987#22270#29255#65306
            end
            object Label192: TLabel
              Left = 9
              Top = 67
              Width = 60
              Height = 12
              Caption = #25773#25918#25968#37327#65306
            end
            object Label193: TLabel
              Left = 9
              Top = 91
              Width = 60
              Height = 12
              Caption = #31354#30333#25968#37327#65306
            end
            object Label194: TLabel
              Left = 9
              Top = 114
              Width = 60
              Height = 12
              Caption = #32472#21046#27169#24335#65306
            end
            object Label196: TLabel
              Left = 9
              Top = 182
              Width = 60
              Height = 12
              Caption = #25773#25918#36895#24230#65306
              Visible = False
            end
            object cbbTargetStatus2_File: TComboBox
              Left = 66
              Top = 16
              Width = 88
              Height = 20
              Style = csDropDownList
              TabOrder = 0
              OnChange = cbbTargetStatus2_FileChange
            end
            object cbbTargetStatus2_DrawMode: TComboBox
              Left = 66
              Top = 111
              Width = 88
              Height = 20
              Style = csDropDownList
              TabOrder = 5
              OnChange = cbbTargetStatus2_DrawModeChange
              Items.Strings = (
                #36879#26126#32472#21046
                #21407#22987#32472#21046)
            end
            object seTargetStatus2_PlayTime: TSpinEditEx
              Left = 66
              Top = 178
              Width = 88
              Height = 21
              MaxValue = 0
              MinValue = 0
              TabOrder = 4
              Value = 0
              Visible = False
              OnChange = seTargetStatus2_PlayTimeChange
            end
            object seTargetStatus2_EmptyCount: TSpinEditEx
              Left = 66
              Top = 87
              Width = 88
              Height = 21
              MaxValue = 0
              MinValue = 0
              TabOrder = 3
              Value = 0
              OnChange = seTargetStatus2_EmptyCountChange
            end
            object seTargetStatus2_PlayCount: TSpinEditEx
              Left = 66
              Top = 63
              Width = 88
              Height = 21
              MaxValue = 0
              MinValue = 0
              TabOrder = 2
              Value = 0
              OnChange = seTargetStatus2_PlayCountChange
            end
            object seTargetStatus2_StartIndex: TSpinEditEx
              Left = 66
              Top = 39
              Width = 88
              Height = 21
              MaxValue = 0
              MinValue = 0
              TabOrder = 1
              Value = 0
              OnChange = seTargetStatus2_StartIndexChange
            end
            object chkTargetStatus2_CalcDir: TCheckBox
              Left = 67
              Top = 137
              Width = 83
              Height = 16
              Caption = #35745#31639#26041#21521
              TabOrder = 6
              OnClick = chkTargetStatus2_CalcDirClick
            end
          end
        end
      end
      object GroupBox3: TGroupBox
        Left = 1
        Top = 2
        Width = 702
        Height = 85
        Caption = #22522#26412#35774#32622
        TabOrder = 1
        object lbl4: TLabel
          Left = 8
          Top = 16
          Width = 84
          Height = 12
          Caption = #25216#33021#24378#21270#31561#32423#65306
        end
        object Label52: TLabel
          Left = 8
          Top = 40
          Width = 84
          Height = 12
          Caption = #25216#33021#21160#20316#31867#22411#65306
        end
        object lbl15: TLabel
          Left = 168
          Top = 40
          Width = 78
          Height = 12
          Caption = #33258#23450#20041#21160#20316#65306
          Font.Charset = GB2312_CHARSET
          Font.Color = clBlue
          Font.Height = -12
          Font.Name = #23435#20307
          Font.Style = [fsBold]
          ParentFont = False
        end
        object lbl16: TLabel
          Left = 247
          Top = 40
          Width = 60
          Height = 12
          Caption = #24320#22987#22270#29255#65306
        end
        object Label11: TLabel
          Left = 374
          Top = 40
          Width = 60
          Height = 12
          Caption = #25773#25918#25968#37327#65306
        end
        object Label195: TLabel
          Left = 502
          Top = 40
          Width = 60
          Height = 12
          Caption = #31354#30333#25968#37327#65306
        end
        object lblMagicWarrNGOption: TLabel
          Left = 8
          Top = 64
          Width = 84
          Height = 12
          Hint = #25193#23637#30340#20869#25346#33258#23450#20041#25216#33021#36873#39033#24320#20851#65292#20165#38480#29992#22312#31867#20284#20110#24320#22825#26025#31867#24555#25463#38190#25216#33021#33258#21160#37322#25918
          Caption = #20869#25346#23545#24212#36873#39033#65306
          ParentShowHint = False
          ShowHint = True
        end
        object lbl18: TLabel
          Left = 374
          Top = 16
          Width = 60
          Height = 12
          Caption = #25216#33021#27169#24335#65306
        end
        object cbbClientLevel: TComboBox
          Left = 88
          Top = 12
          Width = 70
          Height = 20
          Style = csDropDownList
          TabOrder = 0
          OnChange = cbbClientLevelChange
        end
        object chkClientLock: TCheckBox
          Left = 168
          Top = 14
          Width = 73
          Height = 17
          Caption = #39764#27861#38145#23450
          TabOrder = 2
          OnClick = chkClientLockClick
        end
        object cbbClientActionType: TComboBox
          Left = 88
          Top = 36
          Width = 70
          Height = 20
          Hint = #33258#23450#20041#21160#20316#35835#21462#20154#29289#32032#26448#23545#24212#30340#25991#20214#65306'hum.wzl'#65292'hum2.wzl.......'
          Style = csDropDownList
          ParentShowHint = False
          ShowHint = True
          TabOrder = 1
          OnChange = cbbClientActionTypeChange
        end
        object chkClientLockSelf: TCheckBox
          Left = 247
          Top = 14
          Width = 114
          Height = 17
          Caption = #38145#23450#23545#33258#24049#37322#25918
          TabOrder = 3
          OnClick = chkClientLockSelfClick
        end
        object btnCopyConfig: TButton
          Left = 618
          Top = 56
          Width = 75
          Height = 25
          Caption = #22797#21046#37197#32622#21040
          TabOrder = 4
          OnClick = btnCopyConfigClick
        end
        object seClientActionStartIndex: TSpinEditEx
          Left = 303
          Top = 36
          Width = 60
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 5
          Value = 0
          OnChange = seClientActionStartIndexChange
        end
        object seClientActionPlayCount: TSpinEditEx
          Left = 431
          Top = 36
          Width = 60
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 6
          Value = 0
          OnChange = seClientActionPlayCountChange
        end
        object seClientActionEmptyCount: TSpinEditEx
          Left = 558
          Top = 36
          Width = 60
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 7
          Value = 0
          OnChange = seClientActionEmptyCountChange
        end
        object chkClientActionContinue: TCheckBox
          Left = 627
          Top = 38
          Width = 67
          Height = 17
          Hint = #36830#20987#21160#20316#35835#21462#20154#29289#32032#26448#23545#24212#30340#25991#20214#65306'cbohum.wzl'#65292'cbohum2.wzl.......'
          Caption = #36830#20987#21160#20316
          ParentShowHint = False
          ShowHint = True
          TabOrder = 8
          OnClick = chkClientActionContinueClick
        end
        object cbbMagicSwitchMode: TComboBox
          Left = 431
          Top = 12
          Width = 93
          Height = 20
          Hint = 
            #25915#26432#27169#24335#19982#25915#26432#25216#33021#30456#21516#65292#24517#39035#20329#25140#27494#22120#25165#35302#21457#13#10#13#10#25915#26432#27169#24335#30340#25216#33021#23558#19981#20250#26816#27979#25216#33021#20351#29992#26465#20214#13#10#13#10#24320#20851#27169#24335#25216#33021#24314#35758#21152#20010#26102#38388#38388#38548#65292#21542#21017 +
            #25216#33021#24320#21551#21518#65292#33258#21160#25915#20987#20250#24433#21709#31995#32479#40664#35748#30340#28872#28779#12289#21050#26432#31561
          Style = csDropDownList
          ParentShowHint = False
          ShowHint = True
          TabOrder = 9
          OnChange = cbbMagicSwitchModeChange
        end
        object cbbMagicWarrNGOption: TComboBox
          Left = 89
          Top = 60
          Width = 118
          Height = 20
          Hint = #29992#20110#20869#25346#25511#21046#25112#22763#25216#33021#33258#21160#37322#25918#65292#22914#26032#24320#22825#26025#12289#36880#26085#31561#20851#32852#25216#33021#25110#29420#31435#36873#39033
          Style = csDropDownList
          ParentShowHint = False
          ShowHint = True
          TabOrder = 10
          OnChange = cbbMagicWarrNGOptionChange
        end
        object chkSwitchModeNoClose: TCheckBox
          Left = 543
          Top = 14
          Width = 149
          Height = 17
          Hint = 
            #27492#36873#39033#29992#20110#31867#20284#21050#26432#12289#21322#26376#65292#24403#24320#20851#27169#24335#30340#25216#33021#24320#21551#21518#65292#38271#26102#38388#19981#20351#29992#20063#19981#20250#33258#21160#20851#38381#13#10#13#10#22914#20992#20992#21050#26432#32465#23450#20869#25346#33258#21160#25216#33021#25511#21046#21487#19981#21246#36873#27492#39033#65292 +
            #19981#29992#20869#25346#25511#21046#30340#20992#20992#21050#26432#21017#38656#21246#36873#65292#27492#31867#25216#33021#19981#24314#35758#21516#26102#23398#20064#24182#23384#22312'2'#20010#25110#20197#19978
          Caption = #25216#33021#24320#21551#29992#21518#19981#33258#21160#20851#38381
          ParentShowHint = False
          ShowHint = True
          TabOrder = 11
          OnClick = chkSwitchModeNoCloseClick
        end
        object chkMagicAutoOpen: TCheckBox
          Left = 247
          Top = 61
          Width = 94
          Height = 17
          Hint = #24403#29992#33050#26412#23398#20064#25216#33021#25110#37325#26032#36827#20837#28216#25103#21518#65292#25216#33021#33258#21160#24320#21551#65288#31867#20284#20110#21050#26432#25216#33021#65289
          Caption = #25216#33021#33258#21160#24320#21551
          ParentShowHint = False
          ShowHint = True
          TabOrder = 12
          OnClick = chkMagicAutoOpenClick
        end
        object chkClientNotRaiseHand: TCheckBox
          Left = 347
          Top = 61
          Width = 78
          Height = 17
          Hint = #25216#33021#30452#25509#37322#25918#65292#27809#26377#25260#25163#21160#20316
          Caption = #26080#25260#25163#25216#33021
          ParentShowHint = False
          ShowHint = True
          TabOrder = 13
          OnClick = chkClientNotRaiseHandClick
        end
      end
    end
    object tsServerAttack: TTabSheet
      Caption = #26381#21153#22120#31471#35774#32622
      ImageIndex = 2
      object pgcMagicType: TPageControl
        Left = 0
        Top = 145
        Width = 712
        Height = 504
        ActivePage = tsMagicAttack
        Align = alClient
        Style = tsFlatButtons
        TabOrder = 0
        ExplicitWidth = 708
        ExplicitHeight = 503
        object tsMagicAttack: TTabSheet
          Caption = #25915#20987#36873#39033
          TabVisible = False
          object pgcAttack: TPageControl
            Left = 0
            Top = 202
            Width = 700
            Height = 299
            ActivePage = tsAdditionals
            MultiLine = True
            Style = tsFlatButtons
            TabOrder = 0
            object tsAdditionals: TTabSheet
              Caption = #38468#21152#20260#23475
              object Label60: TLabel
                Left = 59
                Top = 5
                Width = 36
                Height = 12
                Caption = #20960#29575#65306
              end
              object Label62: TLabel
                Left = 289
                Top = 5
                Width = 36
                Height = 12
                Caption = #26102#38388#65306
              end
              object Label63: TLabel
                Left = 59
                Top = 27
                Width = 36
                Height = 12
                Caption = #20960#29575#65306
              end
              object Label64: TLabel
                Left = 289
                Top = 27
                Width = 36
                Height = 12
                Caption = #26102#38388#65306
              end
              object Label65: TLabel
                Left = 59
                Top = 50
                Width = 36
                Height = 12
                Caption = #20960#29575#65306
              end
              object Label67: TLabel
                Left = 289
                Top = 50
                Width = 36
                Height = 12
                Caption = #26102#38388#65306
              end
              object Label68: TLabel
                Left = 59
                Top = 72
                Width = 36
                Height = 12
                Caption = #20960#29575#65306
              end
              object Label69: TLabel
                Left = 289
                Top = 72
                Width = 36
                Height = 12
                Caption = #26102#38388#65306
              end
              object Label70: TLabel
                Left = 59
                Top = 116
                Width = 36
                Height = 12
                Caption = #20960#29575#65306
              end
              object Label71: TLabel
                Left = 289
                Top = 116
                Width = 36
                Height = 12
                Caption = #26684#25968#65306
              end
              object Label72: TLabel
                Left = 59
                Top = 139
                Width = 36
                Height = 12
                Caption = #20960#29575#65306
              end
              object Label73: TLabel
                Left = 289
                Top = 139
                Width = 36
                Height = 12
                Caption = #27604#20363#65306
              end
              object Label74: TLabel
                Left = 59
                Top = 161
                Width = 36
                Height = 12
                Caption = #20960#29575#65306
              end
              object Label75: TLabel
                Left = 289
                Top = 161
                Width = 36
                Height = 12
                Caption = #27604#20363#65306
              end
              object Label76: TLabel
                Left = 59
                Top = 183
                Width = 36
                Height = 12
                Caption = #20960#29575#65306
              end
              object Label77: TLabel
                Left = 289
                Top = 183
                Width = 36
                Height = 12
                Caption = #26102#38388#65306
              end
              object Label78: TLabel
                Left = 59
                Top = 206
                Width = 36
                Height = 12
                Caption = #20960#29575#65306
              end
              object Label79: TLabel
                Left = 289
                Top = 206
                Width = 36
                Height = 12
                Caption = #26102#38388#65306
              end
              object Label80: TLabel
                Left = 59
                Top = 228
                Width = 36
                Height = 12
                Caption = #20960#29575#65306
              end
              object Label81: TLabel
                Left = 289
                Top = 228
                Width = 36
                Height = 12
                Caption = #26102#38388#65306
              end
              object Label82: TLabel
                Left = 577
                Top = 5
                Width = 36
                Height = 12
                Caption = #25481#34880#65306
              end
              object Label83: TLabel
                Left = 59
                Top = 94
                Width = 36
                Height = 12
                Caption = #20960#29575#65306
              end
              object Label84: TLabel
                Left = 289
                Top = 94
                Width = 36
                Height = 12
                Caption = #26102#38388#65306
              end
              object Label54: TLabel
                Left = 137
                Top = 5
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label55: TLabel
                Left = 137
                Top = 27
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label56: TLabel
                Left = 137
                Top = 50
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label57: TLabel
                Left = 137
                Top = 72
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label59: TLabel
                Left = 137
                Top = 94
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label61: TLabel
                Left = 137
                Top = 116
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label101: TLabel
                Left = 137
                Top = 139
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label102: TLabel
                Left = 137
                Top = 161
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label103: TLabel
                Left = 137
                Top = 183
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label104: TLabel
                Left = 137
                Top = 206
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label105: TLabel
                Left = 137
                Top = 228
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label106: TLabel
                Left = 158
                Top = 5
                Width = 60
                Height = 12
                Caption = #20960#29575#36882#22686#65306
              end
              object Label107: TLabel
                Left = 158
                Top = 27
                Width = 60
                Height = 12
                Caption = #20960#29575#36882#22686#65306
              end
              object Label108: TLabel
                Left = 158
                Top = 50
                Width = 60
                Height = 12
                Caption = #20960#29575#36882#22686#65306
              end
              object Label109: TLabel
                Left = 158
                Top = 72
                Width = 60
                Height = 12
                Caption = #20960#29575#36882#22686#65306
              end
              object Label110: TLabel
                Left = 158
                Top = 116
                Width = 60
                Height = 12
                Caption = #20960#29575#36882#22686#65306
              end
              object Label111: TLabel
                Left = 158
                Top = 139
                Width = 60
                Height = 12
                Caption = #20960#29575#36882#22686#65306
              end
              object Label112: TLabel
                Left = 158
                Top = 161
                Width = 60
                Height = 12
                Caption = #20960#29575#36882#22686#65306
              end
              object Label113: TLabel
                Left = 158
                Top = 183
                Width = 60
                Height = 12
                Caption = #20960#29575#36882#22686#65306
              end
              object Label114: TLabel
                Left = 158
                Top = 206
                Width = 60
                Height = 12
                Caption = #20960#29575#36882#22686#65306
              end
              object Label115: TLabel
                Left = 158
                Top = 228
                Width = 60
                Height = 12
                Caption = #20960#29575#36882#22686#65306
              end
              object Label116: TLabel
                Left = 158
                Top = 94
                Width = 60
                Height = 12
                Caption = #20960#29575#36882#22686#65306
              end
              object Label117: TLabel
                Left = 459
                Top = 5
                Width = 60
                Height = 12
                Caption = #26102#38388#36882#22686#65306
              end
              object Label118: TLabel
                Left = 459
                Top = 27
                Width = 60
                Height = 12
                Caption = #26102#38388#36882#22686#65306
              end
              object Label119: TLabel
                Left = 459
                Top = 50
                Width = 60
                Height = 12
                Caption = #26102#38388#36882#22686#65306
              end
              object Label120: TLabel
                Left = 459
                Top = 72
                Width = 60
                Height = 12
                Caption = #26102#38388#36882#22686#65306
              end
              object Label121: TLabel
                Left = 459
                Top = 116
                Width = 60
                Height = 12
                Caption = #26684#25968#36882#22686#65306
              end
              object Label122: TLabel
                Left = 459
                Top = 139
                Width = 60
                Height = 12
                Caption = #27604#20363#36882#22686#65306
              end
              object Label123: TLabel
                Left = 459
                Top = 161
                Width = 60
                Height = 12
                Caption = #27604#20363#36882#22686#65306
              end
              object Label124: TLabel
                Left = 459
                Top = 183
                Width = 60
                Height = 12
                Caption = #26102#38388#36882#22686#65306
              end
              object Label125: TLabel
                Left = 459
                Top = 206
                Width = 60
                Height = 12
                Caption = #26102#38388#36882#22686#65306
              end
              object Label126: TLabel
                Left = 459
                Top = 228
                Width = 60
                Height = 12
                Caption = #26102#38388#36882#22686#65306
              end
              object Label127: TLabel
                Left = 459
                Top = 94
                Width = 60
                Height = 12
                Caption = #26102#38388#36882#22686#65306
              end
              object Label128: TLabel
                Left = 560
                Top = 5
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label129: TLabel
                Left = 560
                Top = 27
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label130: TLabel
                Left = 560
                Top = 50
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label131: TLabel
                Left = 560
                Top = 72
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label132: TLabel
                Left = 560
                Top = 94
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label133: TLabel
                Left = 560
                Top = 116
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label134: TLabel
                Left = 560
                Top = 139
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label135: TLabel
                Left = 560
                Top = 161
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label136: TLabel
                Left = 560
                Top = 183
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label138: TLabel
                Left = 560
                Top = 206
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label139: TLabel
                Left = 560
                Top = 228
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label141: TLabel
                Left = 260
                Top = 5
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label142: TLabel
                Left = 260
                Top = 27
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label143: TLabel
                Left = 260
                Top = 50
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label144: TLabel
                Left = 260
                Top = 72
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label152: TLabel
                Left = 260
                Top = 94
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label154: TLabel
                Left = 260
                Top = 116
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label156: TLabel
                Left = 260
                Top = 139
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label158: TLabel
                Left = 260
                Top = 161
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label162: TLabel
                Left = 260
                Top = 183
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label163: TLabel
                Left = 260
                Top = 206
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label165: TLabel
                Left = 260
                Top = 228
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label168: TLabel
                Left = 366
                Top = 116
                Width = 12
                Height = 12
                Caption = #26684
              end
              object Label172: TLabel
                Left = 366
                Top = 138
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label173: TLabel
                Left = 366
                Top = 161
                Width = 6
                Height = 12
                Caption = '%'
              end
              object lbl14: TLabel
                Left = 438
                Top = 250
                Width = 246
                Height = 12
                Caption = #36882#22686#20197#26222#36890#25216#33021#31561#32423'+'#24378#21270#25216#33021#31561#32423#20026#35745#31639#22522#25968
                Font.Charset = GB2312_CHARSET
                Font.Color = clBlue
                Font.Height = -12
                Font.Name = #23435#20307
                Font.Style = []
                ParentFont = False
              end
              object Label201: TLabel
                Left = 75
                Top = 250
                Width = 36
                Height = 12
                Caption = #26102#38388#65306
              end
              object Label204: TLabel
                Left = 222
                Top = 250
                Width = 60
                Height = 12
                Caption = #26102#38388#36882#22686#65306
              end
              object Label238: TLabel
                Left = 322
                Top = 250
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label23: TLabel
                Left = 339
                Top = 250
                Width = 36
                Height = 12
                Caption = #24310#26102#65306
              end
              object chkAdditional0: TCheckBox
                Left = 0
                Top = 3
                Width = 47
                Height = 16
                Caption = #32511#27602
                TabOrder = 0
                OnClick = chkAdditional0Click
              end
              object seAdditionalRate0: TSpinEditEx
                Left = 93
                Top = 0
                Width = 41
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 1
                Value = 0
                OnChange = seAdditionalRate0Change
              end
              object seAdditionalTime0: TSpinEditEx
                Left = 321
                Top = 0
                Width = 41
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 2
                Value = 0
                OnChange = seAdditionalTime0Change
              end
              object chkAdditional1: TCheckBox
                Tag = 1
                Left = 0
                Top = 25
                Width = 47
                Height = 16
                Caption = #32418#27602
                TabOrder = 3
                OnClick = chkAdditional0Click
              end
              object seAdditionalRate1: TSpinEditEx
                Tag = 1
                Left = 93
                Top = 22
                Width = 41
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 4
                Value = 0
                OnChange = seAdditionalRate0Change
              end
              object seAdditionalTime1: TSpinEditEx
                Tag = 1
                Left = 321
                Top = 22
                Width = 41
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 5
                Value = 0
                OnChange = seAdditionalTime0Change
              end
              object chkAdditional2: TCheckBox
                Tag = 2
                Left = 0
                Top = 48
                Width = 47
                Height = 16
                Caption = #40635#30201
                TabOrder = 6
                OnClick = chkAdditional0Click
              end
              object seAdditionalRate2: TSpinEditEx
                Tag = 2
                Left = 93
                Top = 45
                Width = 41
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 7
                Value = 0
                OnChange = seAdditionalRate0Change
              end
              object seAdditionalTime2: TSpinEditEx
                Tag = 2
                Left = 321
                Top = 45
                Width = 41
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 8
                Value = 0
                OnChange = seAdditionalTime0Change
              end
              object chkAdditional3: TCheckBox
                Tag = 3
                Left = 0
                Top = 70
                Width = 47
                Height = 15
                Caption = #20912#20923
                TabOrder = 9
                OnClick = chkAdditional0Click
              end
              object seAdditionalRate3: TSpinEditEx
                Tag = 3
                Left = 93
                Top = 67
                Width = 41
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 10
                Value = 0
                OnChange = seAdditionalRate0Change
              end
              object seAdditionalTime3: TSpinEditEx
                Tag = 3
                Left = 321
                Top = 67
                Width = 41
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 11
                Value = 0
                OnChange = seAdditionalTime0Change
              end
              object chkAdditional4: TCheckBox
                Tag = 4
                Left = 0
                Top = 115
                Width = 47
                Height = 16
                Caption = #25512#21160
                TabOrder = 12
                OnClick = chkAdditional0Click
              end
              object seAdditionalRate4: TSpinEditEx
                Tag = 4
                Left = 93
                Top = 111
                Width = 41
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 13
                Value = 0
                OnChange = seAdditionalRate0Change
              end
              object seAdditionalTime4: TSpinEditEx
                Tag = 4
                Left = 321
                Top = 111
                Width = 41
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 14
                Value = 0
                OnChange = seAdditionalTime0Change
              end
              object chkAdditional5: TCheckBox
                Tag = 5
                Left = 0
                Top = 137
                Width = 47
                Height = 15
                Caption = #21560#34880
                TabOrder = 15
                OnClick = chkAdditional0Click
              end
              object seAdditionalRate5: TSpinEditEx
                Tag = 5
                Left = 93
                Top = 134
                Width = 41
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 16
                Value = 0
                OnChange = seAdditionalRate0Change
              end
              object seAdditionalTime5: TSpinEditEx
                Tag = 5
                Left = 321
                Top = 134
                Width = 41
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 17
                Value = 0
                OnChange = seAdditionalTime0Change
              end
              object chkAdditional6: TCheckBox
                Tag = 6
                Left = 0
                Top = 159
                Width = 47
                Height = 16
                Caption = #21560#34013
                TabOrder = 18
                OnClick = chkAdditional0Click
              end
              object seAdditionalRate6: TSpinEditEx
                Tag = 6
                Left = 93
                Top = 156
                Width = 41
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 19
                Value = 0
                OnChange = seAdditionalRate0Change
              end
              object seAdditionalTime6: TSpinEditEx
                Tag = 6
                Left = 321
                Top = 156
                Width = 41
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 20
                Value = 0
                OnChange = seAdditionalTime0Change
              end
              object chkAdditional7: TCheckBox
                Tag = 7
                Left = 0
                Top = 181
                Width = 47
                Height = 16
                Caption = #34523#32593
                TabOrder = 21
                OnClick = chkAdditional0Click
              end
              object seAdditionalRate7: TSpinEditEx
                Tag = 7
                Left = 93
                Top = 178
                Width = 41
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 22
                Value = 0
                OnChange = seAdditionalRate0Change
              end
              object seAdditionalTime7: TSpinEditEx
                Tag = 7
                Left = 321
                Top = 178
                Width = 41
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 23
                Value = 0
                OnChange = seAdditionalTime0Change
              end
              object chkAdditional8: TCheckBox
                Tag = 8
                Left = 0
                Top = 203
                Width = 60
                Height = 17
                Caption = #38646#38450#24481
                TabOrder = 24
                OnClick = chkAdditional0Click
              end
              object seAdditionalRate8: TSpinEditEx
                Tag = 8
                Left = 93
                Top = 200
                Width = 41
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 25
                Value = 0
                OnChange = seAdditionalRate0Change
              end
              object seAdditionalTime8: TSpinEditEx
                Tag = 8
                Left = 321
                Top = 201
                Width = 41
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 26
                Value = 0
                OnChange = seAdditionalTime0Change
              end
              object chkAdditional9: TCheckBox
                Tag = 9
                Left = 0
                Top = 227
                Width = 60
                Height = 15
                Caption = #38646#39764#24481
                TabOrder = 27
                OnClick = chkAdditional0Click
              end
              object seAdditionalRate9: TSpinEditEx
                Tag = 9
                Left = 93
                Top = 223
                Width = 41
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 28
                Value = 0
                OnChange = seAdditionalRate0Change
              end
              object seAdditionalTime9: TSpinEditEx
                Tag = 9
                Left = 321
                Top = 223
                Width = 41
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 29
                Value = 0
                OnChange = seAdditionalTime0Change
              end
              object chkseAdditionaHighLevel4: TCheckBox
                Left = 577
                Top = 136
                Width = 110
                Height = 16
                Caption = #21487#25512#21516#31561#32423#30446#26631
                TabOrder = 30
                OnClick = chkseAdditionaHighLevel4Click
              end
              object seAdditionaHP0: TSpinEditEx
                Left = 608
                Top = 0
                Width = 65
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 31
                Value = 0
                OnChange = seAdditionaHP0Change
              end
              object chkAdditional10: TCheckBox
                Tag = 10
                Left = 0
                Top = 91
                Width = 47
                Height = 17
                Caption = #20912#23553
                TabOrder = 32
                OnClick = chkAdditional0Click
              end
              object seAdditionalRate10: TSpinEditEx
                Tag = 10
                Left = 93
                Top = 89
                Width = 41
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 33
                Value = 0
                OnChange = seAdditionalRate0Change
              end
              object seAdditionalTime10: TSpinEditEx
                Tag = 10
                Left = 321
                Top = 89
                Width = 41
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 34
                Value = 0
                OnChange = seAdditionalTime0Change
              end
              object seAdditionalRate0_2: TSpinEditEx
                Left = 215
                Top = 0
                Width = 41
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 35
                Value = 0
                OnChange = seAdditionalRate0_2Change
              end
              object seAdditionalRate1_2: TSpinEditEx
                Tag = 1
                Left = 215
                Top = 22
                Width = 41
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 36
                Value = 0
                OnChange = seAdditionalRate0_2Change
              end
              object seAdditionalRate2_2: TSpinEditEx
                Tag = 2
                Left = 215
                Top = 45
                Width = 41
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 37
                Value = 0
                OnChange = seAdditionalRate0_2Change
              end
              object seAdditionalRate3_2: TSpinEditEx
                Tag = 3
                Left = 215
                Top = 67
                Width = 41
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 38
                Value = 0
                OnChange = seAdditionalRate0_2Change
              end
              object seAdditionalRate4_2: TSpinEditEx
                Tag = 4
                Left = 215
                Top = 111
                Width = 41
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 39
                Value = 0
                OnChange = seAdditionalRate0_2Change
              end
              object seAdditionalRate5_2: TSpinEditEx
                Tag = 5
                Left = 215
                Top = 134
                Width = 41
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 40
                Value = 0
                OnChange = seAdditionalRate0_2Change
              end
              object seAdditionalRate6_2: TSpinEditEx
                Tag = 6
                Left = 215
                Top = 156
                Width = 41
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 41
                Value = 0
                OnChange = seAdditionalRate0_2Change
              end
              object seAdditionalRate7_2: TSpinEditEx
                Tag = 7
                Left = 215
                Top = 178
                Width = 41
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 42
                Value = 0
                OnChange = seAdditionalRate0_2Change
              end
              object seAdditionalRate8_2: TSpinEditEx
                Tag = 8
                Left = 215
                Top = 201
                Width = 41
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 43
                Value = 0
                OnChange = seAdditionalRate0_2Change
              end
              object seAdditionalRate9_2: TSpinEditEx
                Tag = 9
                Left = 215
                Top = 223
                Width = 41
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 44
                Value = 0
                OnChange = seAdditionalRate0_2Change
              end
              object seAdditionalRate10_2: TSpinEditEx
                Tag = 10
                Left = 215
                Top = 89
                Width = 41
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 45
                Value = 0
                OnChange = seAdditionalRate0_2Change
              end
              object seAdditionalTime0_2: TSpinEditEx
                Left = 516
                Top = 0
                Width = 41
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 46
                Value = 0
                OnChange = seAdditionalTime0_2Change
              end
              object seAdditionalTime1_2: TSpinEditEx
                Tag = 1
                Left = 516
                Top = 22
                Width = 41
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 47
                Value = 0
                OnChange = seAdditionalTime0_2Change
              end
              object seAdditionalTime2_2: TSpinEditEx
                Tag = 2
                Left = 516
                Top = 45
                Width = 41
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 48
                Value = 0
                OnChange = seAdditionalTime0_2Change
              end
              object seAdditionalTime3_2: TSpinEditEx
                Tag = 3
                Left = 516
                Top = 67
                Width = 41
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 49
                Value = 0
                OnChange = seAdditionalTime0_2Change
              end
              object seAdditionalTime4_2: TSpinEditEx
                Tag = 4
                Left = 516
                Top = 111
                Width = 41
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 50
                Value = 0
                OnChange = seAdditionalTime0_2Change
              end
              object seAdditionalTime5_2: TSpinEditEx
                Tag = 5
                Left = 516
                Top = 134
                Width = 41
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 51
                Value = 0
                OnChange = seAdditionalTime0_2Change
              end
              object seAdditionalTime6_2: TSpinEditEx
                Tag = 6
                Left = 516
                Top = 156
                Width = 41
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 52
                Value = 0
                OnChange = seAdditionalTime0_2Change
              end
              object seAdditionalTime7_2: TSpinEditEx
                Tag = 7
                Left = 516
                Top = 178
                Width = 41
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 53
                Value = 0
                OnChange = seAdditionalTime0_2Change
              end
              object seAdditionalTime8_2: TSpinEditEx
                Tag = 8
                Left = 516
                Top = 201
                Width = 41
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 54
                Value = 0
                OnChange = seAdditionalTime0_2Change
              end
              object seAdditionalTime9_2: TSpinEditEx
                Tag = 9
                Left = 516
                Top = 223
                Width = 41
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 55
                Value = 0
                OnChange = seAdditionalTime0_2Change
              end
              object seAdditionalTime10_2: TSpinEditEx
                Tag = 10
                Left = 516
                Top = 89
                Width = 41
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 56
                Value = 0
                OnChange = seAdditionalTime0_2Change
              end
              object cbbPushedType4: TComboBox
                Left = 577
                Top = 112
                Width = 103
                Height = 20
                Style = csDropDownList
                TabOrder = 57
                OnChange = cbbPushedType4Change
                Items.Strings = (
                  #36319#30446#26631#21516#27493#31227#21160
                  #33258#36523#20013#24515#22235#21608#25512#21160
                  #21333#20307#25512#21160#30446#26631
                  #30446#26631#20013#24515#22235#21608#25512#21160
                  #37326#34542#20914#25758#25512#21160
                  #36861#24515#21050#25512#21160)
              end
              object cbbAdditionalTime0_1: TComboBox
                Left = 366
                Top = 1
                Width = 59
                Height = 20
                Style = csDropDownList
                ItemIndex = 0
                TabOrder = 58
                Text = #31186
                OnChange = cbbAdditionalTime0_1Change
                Items.Strings = (
                  #31186
                  #23041#21147'%')
              end
              object cbbAdditionalTime1_1: TComboBox
                Tag = 1
                Left = 366
                Top = 23
                Width = 59
                Height = 20
                Style = csDropDownList
                ItemIndex = 0
                TabOrder = 59
                Text = #31186
                OnChange = cbbAdditionalTime0_1Change
                Items.Strings = (
                  #31186
                  #23041#21147'%')
              end
              object cbbAdditionalTime2_1: TComboBox
                Tag = 2
                Left = 366
                Top = 45
                Width = 59
                Height = 20
                Style = csDropDownList
                ItemIndex = 0
                TabOrder = 60
                Text = #31186
                OnChange = cbbAdditionalTime0_1Change
                Items.Strings = (
                  #31186
                  #23041#21147'%')
              end
              object cbbAdditionalTime3_1: TComboBox
                Tag = 3
                Left = 366
                Top = 67
                Width = 59
                Height = 20
                Style = csDropDownList
                ItemIndex = 0
                TabOrder = 61
                Text = #31186
                OnChange = cbbAdditionalTime0_1Change
                Items.Strings = (
                  #31186
                  #23041#21147'%')
              end
              object cbbAdditionalTime10_1: TComboBox
                Tag = 10
                Left = 366
                Top = 89
                Width = 59
                Height = 20
                Style = csDropDownList
                ItemIndex = 0
                TabOrder = 62
                Text = #31186
                OnChange = cbbAdditionalTime0_1Change
                Items.Strings = (
                  #31186
                  #23041#21147'%')
              end
              object cbbAdditionalTime7_1: TComboBox
                Tag = 7
                Left = 366
                Top = 178
                Width = 59
                Height = 20
                Style = csDropDownList
                ItemIndex = 0
                TabOrder = 63
                Text = #31186
                OnChange = cbbAdditionalTime0_1Change
                Items.Strings = (
                  #31186
                  #23041#21147'%')
              end
              object cbbAdditionalTime8_1: TComboBox
                Tag = 8
                Left = 366
                Top = 201
                Width = 59
                Height = 20
                Style = csDropDownList
                ItemIndex = 0
                TabOrder = 64
                Text = #31186
                OnChange = cbbAdditionalTime0_1Change
                Items.Strings = (
                  #31186
                  #23041#21147'%')
              end
              object cbbAdditionalTime9_1: TComboBox
                Tag = 9
                Left = 366
                Top = 223
                Width = 59
                Height = 20
                Style = csDropDownList
                ItemIndex = 0
                TabOrder = 65
                Text = #31186
                OnChange = cbbAdditionalTime0_1Change
                Items.Strings = (
                  #31186
                  #23041#21147'%')
              end
              object chkAttackTargetStatus: TCheckBox
                Tag = 9
                Left = 0
                Top = 248
                Width = 70
                Height = 16
                Hint = #21246#36873#21518#65292#20250#22312#30446#26631#19978#25345#32493#26174#31034#29366#24577#65292#26412#36873#39033#37197#21512#20854#20182#35843#25972#23646#24615#36873#39033#20351#29992
                Caption = #30446#26631#29366#24577
                TabOrder = 66
                OnClick = chkAttackTargetStatusClick
              end
              object seAttackTargetStatusTime: TSpinEditEx
                Tag = 9
                Left = 108
                Top = 245
                Width = 41
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 67
                Value = 0
                OnChange = seAttackTargetStatusTimeChange
              end
              object seAttackTargetStatusTime_2: TSpinEditEx
                Tag = 9
                Left = 280
                Top = 245
                Width = 40
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 68
                Value = 0
                OnChange = seAttackTargetStatusTime_2Change
              end
              object cbbAttackTargetStatusTime_1: TComboBox
                Tag = 9
                Left = 152
                Top = 246
                Width = 60
                Height = 20
                Style = csDropDownList
                ItemIndex = 0
                TabOrder = 69
                Text = #31186
                OnChange = cbbAttackTargetStatusTime_1Change
                Items.Strings = (
                  #31186
                  #23041#21147'%')
              end
              object seAttackTargetStatusDelay: TSpinEditEx
                Tag = 9
                Left = 369
                Top = 245
                Width = 56
                Height = 21
                Hint = #21333#20301#65306#27627#31186
                MaxValue = 0
                MinValue = 0
                ParentShowHint = False
                ShowHint = True
                TabOrder = 70
                Value = 0
                OnChange = seAttackTargetStatusDelayChange
              end
            end
            object tsSubAttrib: TTabSheet
              Caption = #25915#20987#20943#23646#24615
              ImageIndex = 1
              object vstAttackDecAttr: TVirtualStringTree
                Left = 0
                Top = 0
                Width = 692
                Height = 268
                Align = alClient
                Colors.BorderColor = 15987699
                Colors.DisabledColor = clGray
                Colors.DropMarkColor = 15385233
                Colors.DropTargetColor = 15385233
                Colors.DropTargetBorderColor = 15987699
                Colors.FocusedSelectionColor = 15385233
                Colors.FocusedSelectionBorderColor = clWhite
                Colors.GridLineColor = 15987699
                Colors.HeaderHotColor = clBlack
                Colors.HotColor = clBlack
                Colors.SelectionRectangleBlendColor = 15385233
                Colors.SelectionRectangleBorderColor = 15385233
                Colors.SelectionTextColor = clBlack
                Colors.TreeLineColor = 9471874
                Colors.UnfocusedColor = 17813616
                Colors.UnfocusedSelectionColor = 15987699
                Colors.UnfocusedSelectionBorderColor = 15987699
                DefaultNodeHeight = 22
                Header.AutoSizeIndex = 0
                Header.Height = 37
                Header.Options = [hoColumnResize, hoDrag, hoShowSortGlyphs, hoVisible]
                Header.ParentFont = True
                LineStyle = lsSolid
                TabOrder = 0
                TreeOptions.MiscOptions = [toAcceptOLEDrop, toCheckSupport, toEditable, toFullRepaintOnResize, toGridExtensions, toInitOnSave, toToggleOnDblClick, toWheelPanning, toEditOnClick]
                TreeOptions.PaintOptions = [toShowButtons, toShowDropmark, toShowHorzGridLines, toShowTreeLines, toShowVertGridLines, toThemeAware, toUseBlendedImages, toFullVertGridLines]
                TreeOptions.SelectionOptions = [toExtendedFocus, toFullRowSelect]
                OnAfterCellPaint = vstAttackDecAttrAfterCellPaint
                OnChecked = vstAttackDecAttrChecked
                OnCreateEditor = vstAttackDecAttrCreateEditor
                OnEditing = vstAttackDecAttrEditing
                OnGetText = vstAttackDecAttrGetText
                OnNodeClick = vstAttackDecAttrNodeClick
                ExplicitWidth = 696
                Columns = <
                  item
                    Position = 0
                    Width = 110
                    WideText = #20943#23646#24615
                  end
                  item
                    Alignment = taRightJustify
                    CaptionAlignment = taCenter
                    Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coWrapCaption, coUseCaptionAlignment]
                    Position = 1
                    Width = 40
                    WideText = #20960#29575#13#10'%'
                  end
                  item
                    Alignment = taRightJustify
                    CaptionAlignment = taCenter
                    Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coWrapCaption, coUseCaptionAlignment]
                    Position = 2
                    Width = 37
                    WideText = #20960#29575#13#10#36882#22686
                  end
                  item
                    Alignment = taRightJustify
                    CaptionAlignment = taCenter
                    Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coUseCaptionAlignment]
                    Position = 3
                    Width = 48
                    WideText = #19979#38480#20540
                  end
                  item
                    Alignment = taRightJustify
                    CaptionAlignment = taCenter
                    Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coWrapCaption, coUseCaptionAlignment]
                    Position = 4
                    Width = 36
                    WideText = #19979#38480#13#10#21333#20301
                  end
                  item
                    Alignment = taRightJustify
                    CaptionAlignment = taCenter
                    Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coWrapCaption, coUseCaptionAlignment]
                    Position = 5
                    Width = 37
                    WideText = #19979#38480#13#10#36882#22686
                  end
                  item
                    Alignment = taRightJustify
                    CaptionAlignment = taCenter
                    Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coUseCaptionAlignment]
                    Position = 6
                    Width = 48
                    WideText = #19978#38480#20540
                  end
                  item
                    Alignment = taRightJustify
                    CaptionAlignment = taCenter
                    Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coWrapCaption, coUseCaptionAlignment]
                    Position = 7
                    Width = 38
                    WideText = #19978#38480#13#10#21333#20301
                  end
                  item
                    Alignment = taRightJustify
                    CaptionAlignment = taCenter
                    Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coWrapCaption, coUseCaptionAlignment]
                    Position = 8
                    Width = 38
                    WideText = #19978#38480#13#10#36882#22686
                  end
                  item
                    Alignment = taRightJustify
                    CaptionAlignment = taCenter
                    Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coWrapCaption, coUseCaptionAlignment]
                    Position = 9
                    Width = 36
                    WideText = #26102#38388#13#10'('#31186')'
                  end
                  item
                    Alignment = taRightJustify
                    CaptionAlignment = taCenter
                    Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coWrapCaption, coUseCaptionAlignment]
                    Position = 10
                    Width = 38
                    WideText = #26102#38388#13#10#36882#22686
                  end
                  item
                    Alignment = taRightJustify
                    CaptionAlignment = taCenter
                    Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coWrapCaption, coUseCaptionAlignment]
                    Position = 11
                    Width = 38
                    WideText = #36882#22686#13#10#21333#20301
                  end
                  item
                    Alignment = taCenter
                    Position = 12
                    Width = 36
                    WideText = #25552#31034
                  end
                  item
                    Position = 13
                    Width = 158
                    WideText = #25552#31034#25991#23383
                  end>
              end
            end
            object tsAttackDecElement: TTabSheet
              Caption = #20943#20803#32032#23646#24615
              ImageIndex = 2
              object vstDecElement: TVirtualStringTree
                Left = 0
                Top = 0
                Width = 692
                Height = 268
                Align = alClient
                Colors.BorderColor = 15987699
                Colors.DisabledColor = clGray
                Colors.DropMarkColor = 15385233
                Colors.DropTargetColor = 15385233
                Colors.DropTargetBorderColor = 15987699
                Colors.FocusedSelectionColor = 15385233
                Colors.FocusedSelectionBorderColor = clWhite
                Colors.GridLineColor = 15987699
                Colors.HeaderHotColor = clBlack
                Colors.HotColor = clBlack
                Colors.SelectionRectangleBlendColor = 15385233
                Colors.SelectionRectangleBorderColor = 15385233
                Colors.SelectionTextColor = clBlack
                Colors.TreeLineColor = 9471874
                Colors.UnfocusedColor = 17813616
                Colors.UnfocusedSelectionColor = 15987699
                Colors.UnfocusedSelectionBorderColor = 15987699
                DefaultNodeHeight = 22
                Header.AutoSizeIndex = 10
                Header.Height = 37
                Header.Options = [hoAutoResize, hoColumnResize, hoDrag, hoShowSortGlyphs, hoVisible]
                Header.ParentFont = True
                LineStyle = lsSolid
                TabOrder = 0
                TreeOptions.MiscOptions = [toAcceptOLEDrop, toCheckSupport, toEditable, toFullRepaintOnResize, toGridExtensions, toInitOnSave, toToggleOnDblClick, toWheelPanning, toEditOnClick]
                TreeOptions.PaintOptions = [toShowButtons, toShowDropmark, toShowHorzGridLines, toShowTreeLines, toShowVertGridLines, toThemeAware, toUseBlendedImages, toFullVertGridLines]
                TreeOptions.SelectionOptions = [toExtendedFocus, toFullRowSelect]
                OnAfterCellPaint = vstDecElementAfterCellPaint
                OnChecked = vstDecElementChecked
                OnCreateEditor = vstDecElementCreateEditor
                OnEditing = vstDecElementEditing
                OnGetText = vstDecElementGetText
                OnNodeClick = vstDecElementNodeClick
                ExplicitWidth = 696
                Columns = <
                  item
                    Position = 0
                    Width = 133
                    WideText = #20943#20803#32032#23646#24615
                  end
                  item
                    Alignment = taRightJustify
                    CaptionAlignment = taCenter
                    Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coWrapCaption, coUseCaptionAlignment]
                    Position = 1
                    Width = 40
                    WideText = #20960#29575#13#10'%'
                  end
                  item
                    Alignment = taRightJustify
                    CaptionAlignment = taCenter
                    Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coWrapCaption, coUseCaptionAlignment]
                    Position = 2
                    Width = 40
                    WideText = #20960#29575#13#10#36882#22686
                  end
                  item
                    Alignment = taRightJustify
                    CaptionAlignment = taCenter
                    Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coUseCaptionAlignment]
                    Position = 3
                    Width = 38
                    WideText = #20943#20540
                  end
                  item
                    Alignment = taRightJustify
                    CaptionAlignment = taCenter
                    Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coWrapCaption, coUseCaptionAlignment]
                    Position = 4
                    Width = 46
                    WideText = #20943#20540#13#10#21333#20301
                  end
                  item
                    Alignment = taRightJustify
                    CaptionAlignment = taCenter
                    Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coWrapCaption, coUseCaptionAlignment]
                    Position = 5
                    Width = 45
                    WideText = #20943#20540#13#10#36882#22686
                  end
                  item
                    Alignment = taRightJustify
                    CaptionAlignment = taCenter
                    Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coWrapCaption, coUseCaptionAlignment]
                    Position = 6
                    Width = 44
                    WideText = #26102#38388#13#10'('#31186')'
                  end
                  item
                    Alignment = taRightJustify
                    CaptionAlignment = taCenter
                    Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coWrapCaption, coUseCaptionAlignment]
                    Position = 7
                    Width = 44
                    WideText = #26102#38388#13#10#36882#22686
                  end
                  item
                    Alignment = taRightJustify
                    CaptionAlignment = taCenter
                    Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coWrapCaption, coUseCaptionAlignment]
                    Position = 8
                    Width = 38
                    WideText = #36882#22686#13#10#21333#20301
                  end
                  item
                    Alignment = taCenter
                    Position = 9
                    Width = 37
                    WideText = #25552#31034
                  end
                  item
                    Position = 10
                    Width = 187
                    WideText = #25552#31034#25991#23383
                  end>
              end
            end
            object ts1: TTabSheet
              Caption = #30772#38450'/'#30772#39764#27861#30462
              ImageIndex = 3
              object Label776: TLabel
                Left = 434
                Top = 5
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label92: TLabel
                Left = 91
                Top = 5
                Width = 36
                Height = 12
                Caption = #20960#29575#65306
              end
              object Label94: TLabel
                Left = 169
                Top = 5
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label95: TLabel
                Left = 190
                Top = 5
                Width = 60
                Height = 12
                Caption = #20960#29575#36882#22686#65306
              end
              object Label97: TLabel
                Left = 454
                Top = 5
                Width = 60
                Height = 12
                Caption = #27604#20363#36882#22686#65306
              end
              object Label98: TLabel
                Left = 552
                Top = 5
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label100: TLabel
                Left = 292
                Top = 5
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label93: TLabel
                Left = 322
                Top = 5
                Width = 60
                Height = 12
                Caption = #30772#38450#27604#20363#65306
              end
              object Label137: TLabel
                Left = 434
                Top = 29
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label140: TLabel
                Left = 91
                Top = 29
                Width = 36
                Height = 12
                Caption = #20960#29575#65306
              end
              object Label145: TLabel
                Left = 169
                Top = 29
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label146: TLabel
                Left = 190
                Top = 29
                Width = 60
                Height = 12
                Caption = #20960#29575#36882#22686#65306
              end
              object Label147: TLabel
                Left = 454
                Top = 29
                Width = 60
                Height = 12
                Caption = #27604#20363#36882#22686#65306
              end
              object Label148: TLabel
                Left = 552
                Top = 29
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label149: TLabel
                Left = 292
                Top = 29
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label150: TLabel
                Left = 322
                Top = 29
                Width = 60
                Height = 12
                Caption = #30772#38450#27604#20363#65306
              end
              object Label151: TLabel
                Left = 434
                Top = 53
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label153: TLabel
                Left = 91
                Top = 53
                Width = 36
                Height = 12
                Caption = #20960#29575#65306
              end
              object Label155: TLabel
                Left = 169
                Top = 53
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label157: TLabel
                Left = 190
                Top = 53
                Width = 60
                Height = 12
                Caption = #20960#29575#36882#22686#65306
              end
              object Label159: TLabel
                Left = 454
                Top = 53
                Width = 60
                Height = 12
                Caption = #27604#20363#36882#22686#65306
              end
              object Label160: TLabel
                Left = 552
                Top = 53
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label161: TLabel
                Left = 292
                Top = 53
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label164: TLabel
                Left = 322
                Top = 53
                Width = 60
                Height = 12
                Caption = #30772#38450#27604#20363#65306
              end
              object Label166: TLabel
                Left = 434
                Top = 78
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label167: TLabel
                Left = 91
                Top = 78
                Width = 36
                Height = 12
                Caption = #20960#29575#65306
              end
              object Label169: TLabel
                Left = 169
                Top = 78
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label170: TLabel
                Left = 190
                Top = 78
                Width = 60
                Height = 12
                Caption = #20960#29575#36882#22686#65306
              end
              object Label171: TLabel
                Left = 454
                Top = 78
                Width = 60
                Height = 12
                Caption = #27604#20363#36882#22686#65306
              end
              object Label174: TLabel
                Left = 552
                Top = 78
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label175: TLabel
                Left = 292
                Top = 78
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label176: TLabel
                Left = 322
                Top = 78
                Width = 60
                Height = 12
                Caption = #30772#30462#27604#20363#65306
              end
              object Label182: TLabel
                Left = 434
                Top = 102
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label183: TLabel
                Left = 91
                Top = 102
                Width = 36
                Height = 12
                Caption = #20960#29575#65306
              end
              object Label184: TLabel
                Left = 169
                Top = 102
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label185: TLabel
                Left = 190
                Top = 102
                Width = 60
                Height = 12
                Caption = #20960#29575#36882#22686#65306
              end
              object Label187: TLabel
                Left = 454
                Top = 102
                Width = 60
                Height = 12
                Caption = #27604#20363#36882#22686#65306
              end
              object Label188: TLabel
                Left = 552
                Top = 102
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label189: TLabel
                Left = 292
                Top = 102
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label197: TLabel
                Left = 322
                Top = 102
                Width = 60
                Height = 12
                Caption = #30772#30462#27604#20363#65306
              end
              object Label66: TLabel
                Left = 434
                Top = 126
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label87: TLabel
                Left = 91
                Top = 126
                Width = 36
                Height = 12
                Caption = #20960#29575#65306
              end
              object Label88: TLabel
                Left = 169
                Top = 126
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label89: TLabel
                Left = 190
                Top = 126
                Width = 60
                Height = 12
                Caption = #20960#29575#36882#22686#65306
              end
              object Label203: TLabel
                Left = 454
                Top = 126
                Width = 60
                Height = 12
                Caption = #27604#20363#36882#22686#65306
              end
              object Label239: TLabel
                Left = 552
                Top = 126
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label240: TLabel
                Left = 292
                Top = 126
                Width = 6
                Height = 12
                Caption = '%'
              end
              object Label241: TLabel
                Left = 322
                Top = 126
                Width = 60
                Height = 12
                Caption = #30772#30462#27604#20363#65306
              end
              object lbl19: TLabel
                Left = 0
                Top = 160
                Width = 456
                Height = 12
                Caption = #25915#20987#30772#38450#65306#25112#22763#25216#33021#30772#30446#26631#29289#29702#38450#24481#30340#30334#20998#27604#65307#38750#25112#22763#25216#33021#21017#30772#30446#26631#39764#27861#38450#24481#30340#30334#20998#27604
                Font.Charset = GB2312_CHARSET
                Font.Color = clBlue
                Font.Height = -12
                Font.Name = #23435#20307
                Font.Style = []
                ParentFont = False
              end
              object Label90: TLabel
                Left = 0
                Top = 200
                Width = 654
                Height = 12
                Caption = #30772#38450#27604#20363#65306#20197#25216#33021#31561#32423#20026#20934'('#25216#33021#31561#32423'+'#24378#21270#31561#32423')'#65307#22914#30772#38450#27604#20363#20026'10'#65292#27604#20363#36882#22686#20026'10%'#65292#21017#19977#32423#25216#33021#30772#38450#27604#20363#20026#65306'10+10*3=40%'
                Font.Charset = GB2312_CHARSET
                Font.Color = clBlue
                Font.Height = -12
                Font.Name = #23435#20307
                Font.Style = []
                ParentFont = False
              end
              object Label91: TLabel
                Left = 0
                Top = 180
                Width = 636
                Height = 12
                Caption = #30772#38450#20960#29575#65306#20197#25216#33021#31561#32423#20026#20934'('#25216#33021#31561#32423'+'#24378#21270#31561#32423')'#65307#22914#20960#29575#20026'10%'#65292#20960#29575#36882#22686#20026'10%'#65292#21017#19977#32423#25216#33021#30772#38450#20960#29575#20026#65306'10+10*3=40%'
                Font.Charset = GB2312_CHARSET
                Font.Color = clBlue
                Font.Height = -12
                Font.Name = #23435#20307
                Font.Style = []
                ParentFont = False
              end
              object seMagicACHumValue: TSpinEditEx
                Left = 378
                Top = 0
                Width = 52
                Height = 21
                MaxValue = 100
                MinValue = 0
                TabOrder = 0
                Value = 0
                OnChange = seMagicACHumValueChange
              end
              object seMagicACMonValue: TSpinEditEx
                Tag = 1
                Left = 378
                Top = 24
                Width = 52
                Height = 21
                MaxValue = 100
                MinValue = 0
                TabOrder = 1
                Value = 0
                OnChange = seMagicACHumValueChange
              end
              object seMagicACHeroValue: TSpinEditEx
                Tag = 2
                Left = 378
                Top = 49
                Width = 52
                Height = 21
                MaxValue = 100
                MinValue = 0
                TabOrder = 2
                Value = 0
                OnChange = seMagicACHumValueChange
              end
              object seDefenceHumValue: TSpinEditEx
                Tag = 3
                Left = 378
                Top = 73
                Width = 52
                Height = 21
                MaxValue = 100
                MinValue = 0
                TabOrder = 3
                Value = 0
                OnChange = seMagicACHumValueChange
              end
              object seDefenceMonValue: TSpinEditEx
                Tag = 4
                Left = 378
                Top = 97
                Width = 52
                Height = 21
                MaxValue = 100
                MinValue = 0
                TabOrder = 4
                Value = 0
                OnChange = seMagicACHumValueChange
              end
              object seDefenceHeroValue: TSpinEditEx
                Tag = 5
                Left = 378
                Top = 122
                Width = 52
                Height = 21
                MaxValue = 100
                MinValue = 0
                TabOrder = 5
                Value = 0
                OnChange = seMagicACHumValueChange
              end
              object chkMagicACHum: TCheckBox
                Left = 0
                Top = 3
                Width = 81
                Height = 16
                Caption = #23545#20154#29289#30772#38450
                TabOrder = 6
                OnClick = chkMagicACHumClick
              end
              object seMagicACHumRate: TSpinEditEx
                Left = 125
                Top = 0
                Width = 41
                Height = 21
                MaxValue = 100
                MinValue = 0
                TabOrder = 7
                Value = 0
                OnChange = seMagicACHumRateChange
              end
              object seMagicACHumRateAdd: TSpinEditEx
                Left = 247
                Top = 0
                Width = 41
                Height = 21
                MaxValue = 100
                MinValue = 0
                TabOrder = 8
                Value = 0
                OnChange = seMagicACHumRateAddChange
              end
              object seMagicACHumValueAdd: TSpinEditEx
                Left = 511
                Top = 1
                Width = 41
                Height = 21
                MaxValue = 100
                MinValue = 0
                TabOrder = 9
                Value = 0
                OnChange = seMagicACHumValueAddChange
              end
              object chkMagicACMon: TCheckBox
                Tag = 1
                Left = 0
                Top = 27
                Width = 81
                Height = 16
                Caption = #23545#24618#29289#30772#38450
                TabOrder = 10
                OnClick = chkMagicACHumClick
              end
              object seMagicACMonRate: TSpinEditEx
                Tag = 1
                Left = 125
                Top = 24
                Width = 41
                Height = 21
                MaxValue = 100
                MinValue = 0
                TabOrder = 11
                Value = 0
                OnChange = seMagicACHumRateChange
              end
              object seMagicACMonRateAdd: TSpinEditEx
                Tag = 1
                Left = 247
                Top = 24
                Width = 41
                Height = 21
                MaxValue = 100
                MinValue = 0
                TabOrder = 12
                Value = 0
                OnChange = seMagicACHumRateAddChange
              end
              object seMagicACMonValueAdd: TSpinEditEx
                Tag = 1
                Left = 511
                Top = 25
                Width = 41
                Height = 21
                MaxValue = 100
                MinValue = 0
                TabOrder = 13
                Value = 0
                OnChange = seMagicACHumValueAddChange
              end
              object chkMagicACHero: TCheckBox
                Tag = 2
                Left = 0
                Top = 51
                Width = 81
                Height = 16
                Caption = #23545#33521#38596#30772#38450
                TabOrder = 14
                OnClick = chkMagicACHumClick
              end
              object seMagicACHeroRate: TSpinEditEx
                Tag = 2
                Left = 125
                Top = 48
                Width = 41
                Height = 21
                MaxValue = 100
                MinValue = 0
                TabOrder = 15
                Value = 0
                OnChange = seMagicACHumRateChange
              end
              object seMagicACHeroRateAdd: TSpinEditEx
                Tag = 2
                Left = 247
                Top = 48
                Width = 41
                Height = 21
                MaxValue = 100
                MinValue = 0
                TabOrder = 16
                Value = 0
                OnChange = seMagicACHumRateAddChange
              end
              object seMagicACHeroValueAdd: TSpinEditEx
                Tag = 2
                Left = 511
                Top = 49
                Width = 41
                Height = 21
                MaxValue = 100
                MinValue = 0
                TabOrder = 17
                Value = 0
                OnChange = seMagicACHumValueAddChange
              end
              object chkDefenceHum: TCheckBox
                Tag = 3
                Left = 0
                Top = 76
                Width = 81
                Height = 16
                Caption = #23545#20154#29289#30772#30462
                TabOrder = 18
                OnClick = chkMagicACHumClick
              end
              object seDefenceHumRate: TSpinEditEx
                Tag = 3
                Left = 125
                Top = 73
                Width = 41
                Height = 21
                MaxValue = 100
                MinValue = 0
                TabOrder = 19
                Value = 0
                OnChange = seMagicACHumRateChange
              end
              object seDefenceHumRateAdd: TSpinEditEx
                Tag = 3
                Left = 247
                Top = 73
                Width = 41
                Height = 21
                MaxValue = 100
                MinValue = 0
                TabOrder = 20
                Value = 0
                OnChange = seMagicACHumRateAddChange
              end
              object seDefenceHumValueAdd: TSpinEditEx
                Tag = 3
                Left = 511
                Top = 73
                Width = 41
                Height = 21
                MaxValue = 100
                MinValue = 0
                TabOrder = 21
                Value = 0
                OnChange = seMagicACHumValueAddChange
              end
              object chkDefenceMon: TCheckBox
                Tag = 4
                Left = 0
                Top = 100
                Width = 81
                Height = 16
                Caption = #23545#24618#29289#30772#30462
                TabOrder = 22
                OnClick = chkMagicACHumClick
              end
              object seDefenceMonRate: TSpinEditEx
                Tag = 4
                Left = 125
                Top = 97
                Width = 41
                Height = 21
                MaxValue = 100
                MinValue = 0
                TabOrder = 23
                Value = 0
                OnChange = seMagicACHumRateChange
              end
              object seDefenceMonRateAdd: TSpinEditEx
                Tag = 4
                Left = 247
                Top = 97
                Width = 41
                Height = 21
                MaxValue = 100
                MinValue = 0
                TabOrder = 24
                Value = 0
                OnChange = seMagicACHumRateAddChange
              end
              object seDefenceMonValueAdd: TSpinEditEx
                Tag = 4
                Left = 511
                Top = 97
                Width = 41
                Height = 21
                MaxValue = 100
                MinValue = 0
                TabOrder = 25
                Value = 0
                OnChange = seMagicACHumValueAddChange
              end
              object chkDefenceHero: TCheckBox
                Tag = 5
                Left = 0
                Top = 124
                Width = 81
                Height = 16
                Caption = #23545#33521#38596#30772#30462
                TabOrder = 26
                OnClick = chkMagicACHumClick
              end
              object seDefenceHeroRate: TSpinEditEx
                Tag = 5
                Left = 125
                Top = 121
                Width = 41
                Height = 21
                MaxValue = 100
                MinValue = 0
                TabOrder = 27
                Value = 0
                OnChange = seMagicACHumRateChange
              end
              object seDefenceHeroRateAdd: TSpinEditEx
                Tag = 5
                Left = 247
                Top = 121
                Width = 41
                Height = 21
                MaxValue = 100
                MinValue = 0
                TabOrder = 28
                Value = 0
                OnChange = seMagicACHumRateAddChange
              end
              object seDefenceHeroValueAdd: TSpinEditEx
                Tag = 5
                Left = 511
                Top = 121
                Width = 41
                Height = 21
                MaxValue = 100
                MinValue = 0
                TabOrder = 29
                Value = 0
                OnChange = seMagicACHumValueAddChange
              end
            end
          end
          object grpOptions: TGroupBox
            Left = 0
            Top = 1
            Width = 311
            Height = 81
            Caption = #25915#20987#36873#39033
            TabOrder = 1
            object Label6: TLabel
              Left = 190
              Top = 38
              Width = 60
              Height = 12
              Caption = #36817#25915#33539#22260#65306
            end
            object Label7: TLabel
              Left = 288
              Top = 38
              Width = 12
              Height = 12
              Caption = #26684
            end
            object Label8: TLabel
              Left = 190
              Top = 16
              Width = 60
              Height = 12
              Caption = #32676#25915#33539#22260#65306
            end
            object Label9: TLabel
              Left = 288
              Top = 16
              Width = 12
              Height = 12
              Caption = #26684
            end
            object Label12: TLabel
              Left = 10
              Top = 20
              Width = 60
              Height = 12
              Caption = #25915#20987#30446#26631#65306
            end
            object lblAttackWidth: TLabel
              Left = 190
              Top = 60
              Width = 60
              Height = 12
              Caption = #23485#24230#33539#22260#65306
              Visible = False
            end
            object lblH_AttackWidth: TLabel
              Left = 288
              Top = 60
              Width = 12
              Height = 12
              Caption = #26684
              Visible = False
            end
            object seAttackNearRange: TSpinEditEx
              Left = 247
              Top = 34
              Width = 37
              Height = 21
              MaxValue = 0
              MinValue = 0
              TabOrder = 1
              Value = 0
              OnChange = seAttackNearRangeChange
            end
            object seAttackGroupRange: TSpinEditEx
              Left = 247
              Top = 11
              Width = 37
              Height = 21
              MaxValue = 0
              MinValue = 0
              TabOrder = 2
              Value = 0
              OnChange = seAttackGroupRangeChange
            end
            object cbbAttackTarget: TComboBox
              Left = 67
              Top = 16
              Width = 97
              Height = 20
              Style = csDropDownList
              TabOrder = 0
              OnChange = cbbAttackTargetChange
            end
            object chkEnableAntiMagic: TCheckBox
              Left = 10
              Top = 38
              Width = 94
              Height = 18
              Caption = #20801#35768#39764#27861#36530#36991
              TabOrder = 3
              OnClick = chkEnableAntiMagicClick
            end
            object chkEnableHitPoint: TCheckBox
              Left = 10
              Top = 59
              Width = 122
              Height = 18
              Caption = #35745#31639#20934#30830#25935#25463#24230
              TabOrder = 4
              OnClick = chkEnableHitPointClick
            end
            object seAttackLineWidth: TSpinEditEx
              Left = 247
              Top = 56
              Width = 37
              Height = 21
              MaxValue = 10
              MinValue = 0
              TabOrder = 5
              Value = 0
              Visible = False
              OnChange = seAttackLineWidthChange
            end
          end
          object grpCallMob: TGroupBox
            Left = 320
            Top = 1
            Width = 384
            Height = 81
            Caption = '   '#21484#21796#24618#29289#25915#20987
            TabOrder = 2
            object lbl11: TLabel
              Left = 7
              Top = 23
              Width = 42
              Height = 12
              Caption = #24618#29289'1'#65306
            end
            object Label207: TLabel
              Left = 113
              Top = 23
              Width = 36
              Height = 12
              Caption = #25968#37327#65306
            end
            object Label214: TLabel
              Left = 13
              Top = 52
              Width = 36
              Height = 12
              Caption = #20960#29575#65306
            end
            object Label215: TLabel
              Left = 98
              Top = 52
              Width = 6
              Height = 12
              Caption = '%'
            end
            object Label210: TLabel
              Left = 192
              Top = 23
              Width = 42
              Height = 12
              Caption = #24618#29289'2'#65306
            end
            object Label211: TLabel
              Left = 305
              Top = 23
              Width = 36
              Height = 12
              Caption = #25968#37327#65306
            end
            object lbl8: TLabel
              Left = 197
              Top = 52
              Width = 36
              Height = 12
              Caption = #21467#21464#65306
            end
            object lbl9: TLabel
              Left = 297
              Top = 52
              Width = 60
              Height = 12
              Caption = #26102#38388'('#20998#38047')'
            end
            object Label199: TLabel
              Left = 113
              Top = 52
              Width = 36
              Height = 12
              Caption = #31561#32423#65306
            end
            object edtCallMonster1: TEdit
              Left = 44
              Top = 19
              Width = 60
              Height = 20
              TabOrder = 3
              OnChange = edtCallMonster1Change
            end
            object seCallMonsterNum1: TSpinEditEx
              Left = 144
              Top = 19
              Width = 36
              Height = 21
              MaxValue = 0
              MinValue = 0
              TabOrder = 1
              Value = 0
              OnChange = seCallMonsterNum1Change
            end
            object chkEnabledCallMonster: TCheckBox
              Left = 9
              Top = -1
              Width = 22
              Height = 17
              TabOrder = 0
              OnClick = chkEnabledCallMonsterClick
            end
            object seCallMonstersRate: TSpinEditEx
              Left = 44
              Top = 47
              Width = 49
              Height = 21
              MaxValue = 100
              MinValue = 0
              TabOrder = 6
              Value = 0
              OnChange = seCallMonstersRateChange
            end
            object edtCallMonster2: TEdit
              Tag = 1
              Left = 229
              Top = 19
              Width = 64
              Height = 20
              TabOrder = 4
              OnChange = edtCallMonster1Change
            end
            object seCallMonsterNum2: TSpinEditEx
              Tag = 1
              Left = 336
              Top = 19
              Width = 36
              Height = 21
              MaxValue = 0
              MinValue = 0
              TabOrder = 2
              Value = 0
              OnChange = seCallMonsterNum1Change
            end
            object seCallMonstersRoyaltySec: TSpinEditEx
              Tag = 3
              Left = 229
              Top = 47
              Width = 64
              Height = 21
              MaxValue = 0
              MinValue = 0
              TabOrder = 5
              Value = 0
              OnChange = seCallMonstersRoyaltySecChange
            end
            object seCallMonstersLevel: TSpinEditEx
              Left = 144
              Top = 47
              Width = 36
              Height = 21
              MaxValue = 0
              MinValue = 0
              TabOrder = 7
              Value = 0
              OnChange = seCallMonstersLevelChange
            end
          end
          object grpPower: TGroupBox
            Left = 0
            Top = 89
            Width = 704
            Height = 44
            Caption = #25216#33021#23041#21147
            TabOrder = 3
            object Label10: TLabel
              Left = 376
              Top = 20
              Width = 6
              Height = 12
              Caption = '%'
            end
            object Label13: TLabel
              Left = 9
              Top = 20
              Width = 60
              Height = 12
              Caption = #23041#21147#35745#31639#65306
            end
            object lbl2: TLabel
              Left = 158
              Top = 20
              Width = 60
              Height = 12
              Caption = #25216#33021#31561#32423#65306
            end
            object Label53: TLabel
              Left = 277
              Top = 20
              Width = 60
              Height = 12
              Caption = #23041#21147#20493#25968#65306
            end
            object lblLineAttackAddPower: TLabel
              Left = 574
              Top = 20
              Width = 60
              Height = 12
              Caption = #32447#24615#36882#22686#65306
            end
            object lblLineAttackAddPowerPerc: TLabel
              Left = 681
              Top = 20
              Width = 6
              Height = 12
              Caption = '%'
            end
            object Label206: TLabel
              Left = 403
              Top = 20
              Width = 102
              Height = 12
              Caption = #19981#27515#31995#24618#29289#20260#23475'+'#65306
            end
            object Label208: TLabel
              Left = 548
              Top = 20
              Width = 6
              Height = 12
              Caption = '%'
            end
            object seAttackPowerRate: TSpinEditEx
              Left = 333
              Top = 15
              Width = 41
              Height = 21
              MaxValue = 0
              MinValue = 0
              TabOrder = 2
              Value = 0
              OnChange = seAttackPowerRateChange
            end
            object cbbAttackPowerLevel: TComboBox
              Left = 215
              Top = 16
              Width = 49
              Height = 20
              Style = csDropDownList
              TabOrder = 1
              OnChange = cbbAttackPowerLevelChange
            end
            object cbbAttackPowerCalc: TComboBox
              Left = 66
              Top = 16
              Width = 84
              Height = 20
              Style = csDropDownList
              TabOrder = 0
              OnChange = cbbAttackPowerCalcChange
            end
            object seLineAttackAddPower: TSpinEditEx
              Left = 631
              Top = 15
              Width = 46
              Height = 21
              Hint = #25216#33021#23041#21147#38543#30528#19982#25915#20987#32773#30340#36317#31163#21464#36828#32780#22686#21152#25110#20943#23567#65292#21487#22635#36127#25968#13#10#13#10#35813#21442#25968#21482#38024#23545#20110#32447#24615#25216#33021#12304#30452#32447#25915#20987#12289'8'#26041#21521#25915#20987#21450'16'#26041#21521#25915#20987#12305
              MaxValue = 0
              MinValue = 0
              ParentShowHint = False
              ShowHint = True
              TabOrder = 3
              Value = 0
              OnChange = seLineAttackAddPowerChange
            end
            object seAttackPowerUndeadAdd: TSpinEditEx
              Left = 501
              Top = 15
              Width = 43
              Height = 21
              Hint = #25216#33021#23545#19981#27515#31995#24618#29289#20260#23475#39069#22806#22686#21152#30334#20998#27604
              MaxValue = 0
              MinValue = 0
              ParentShowHint = False
              ShowHint = True
              TabOrder = 4
              Value = 0
              OnChange = seAttackPowerUndeadAddChange
            end
          end
          object grpMove: TGroupBox
            Left = 0
            Top = 139
            Width = 704
            Height = 62
            Caption = '   '#20801#35768#30636#31227#31227#21160#25915#20987
            TabOrder = 4
            object Label216: TLabel
              Left = 10
              Top = 21
              Width = 60
              Height = 12
              Caption = #20351#29992#20960#29575#65306
            end
            object Label217: TLabel
              Left = 114
              Top = 21
              Width = 6
              Height = 12
              Caption = '%'
            end
            object Label242: TLabel
              Left = 314
              Top = 42
              Width = 60
              Height = 12
              Caption = #31361#36827#26684#25968#65306
            end
            object seAttackTeleportRate: TSpinEditEx
              Left = 67
              Top = 16
              Width = 44
              Height = 21
              Hint = #25968#20540#36234#22823#20960#29575#36234#39640#65292'0%'#26102#34920#31034#19981#30636#31227
              MaxValue = 100
              MinValue = 0
              ParentShowHint = False
              ShowHint = True
              TabOrder = 0
              Value = 0
              OnChange = seAttackTeleportRateChange
            end
            object chkAttackTeleportRunHum: TCheckBox
              Left = 134
              Top = 18
              Width = 58
              Height = 17
              Hint = #25171#24320#27492#21151#33021#21518#65292#20154#29289#23558#21487#20197#31359#36807#20854#20182#20154#29289
              Caption = #21487#31359#20154
              TabOrder = 2
              OnClick = chkAttackTeleportRunHumClick
            end
            object chkAttackTeleportRunMon: TCheckBox
              Left = 200
              Top = 18
              Width = 60
              Height = 17
              Hint = #25171#24320#27492#21151#33021#21518#65292#20154#29289#23558#21487#20197#31359#36807#24618#29289
              Caption = #21487#31359#24618
              TabOrder = 3
              OnClick = chkAttackTeleportRunMonClick
            end
            object chkAttackTeleportRunNpc: TCheckBox
              Left = 269
              Top = 18
              Width = 63
              Height = 17
              Hint = #25171#24320#27492#21151#33021#21518#65292#20154#29289#23558#21487#20197#31359#36807'NPC'
              Caption = #21487#31359'NPC'
              TabOrder = 4
              OnClick = chkAttackTeleportRunNpcClick
            end
            object chkAttackTeleportRunGuard: TCheckBox
              Left = 340
              Top = 18
              Width = 68
              Height = 17
              Hint = #25171#24320#27492#21151#33021#21518#65292#20154#29289#23558#21487#20197#31359#36807#23432#21355'('#22823#20992#12289#24339#31661#25163')'
              Caption = #21487#31359#23432#21355
              TabOrder = 5
              OnClick = chkAttackTeleportRunGuardClick
            end
            object chkAttackTeleportRunObstacle: TCheckBox
              Left = 417
              Top = 18
              Width = 69
              Height = 17
              Hint = #25171#24320#27492#21151#33021#21518#65292#20154#29289#23558#21487#20197#31359#36807#23432#21355'('#22823#20992#12289#24339#31661#25163')'
              Caption = #21487#31359#38556#30861
              TabOrder = 6
              OnClick = chkAttackTeleportRunObstacleClick
            end
            object chkAttackTeleportWarDisHumRun: TCheckBox
              Left = 494
              Top = 18
              Width = 93
              Height = 17
              Hint = #25171#24320#27492#21151#33021#21518#65292#22312#25915#22478#21306#22495#25915#30340#22478#26102#27573#20840#37096#31105#27490
              Caption = #25915#22478#21306#22495#20840#31105
              TabOrder = 7
              OnClick = chkAttackTeleportWarDisHumRunClick
            end
            object chkAttackTeleportCannotRunItem: TCheckBox
              Left = 596
              Top = 18
              Width = 89
              Height = 17
              Caption = #31105#27490#39134#35013#22791
              TabOrder = 8
              OnClick = chkAttackTeleportCannotRunItemClick
            end
            object chkNoTeleportNoAttack: TCheckBox
              Left = 9
              Top = 40
              Width = 123
              Height = 17
              Caption = #27809#26377#30636#31227#26102#19981#25915#20987
              TabOrder = 9
              OnClick = chkNoTeleportNoAttackClick
            end
            object chkAttackTeleportAfterDamage: TCheckBox
              Left = 134
              Top = 40
              Width = 87
              Height = 17
              Caption = #30636#31227#21069#25915#20987
              TabOrder = 10
              OnClick = chkAttackTeleportAfterDamageClick
            end
            object chkAttackTeleportRush: TCheckBox
              Left = 228
              Top = 40
              Width = 87
              Height = 17
              Caption = #31361#36827#22411#30636#31227
              TabOrder = 11
              OnClick = chkAttackTeleportRushClick
            end
            object seAttackTeleportRushCount: TSpinEditEx
              Left = 374
              Top = 37
              Width = 43
              Height = 21
              Hint = #25216#33021#23545#19981#27515#31995#24618#29289#20260#23475#39069#22806#22686#21152#30334#20998#27604
              MaxValue = 0
              MinValue = 0
              ParentShowHint = False
              ShowHint = True
              TabOrder = 12
              Value = 0
              OnChange = seAttackTeleportRushCountChange
            end
            object chkAttackTeleportAttack: TCheckBox
              Left = 9
              Top = -1
              Width = 22
              Height = 17
              TabOrder = 1
              OnClick = chkAttackTeleportAttackClick
            end
          end
        end
        object tsMagicProtected: TTabSheet
          Caption = #22686#30410#36873#39033
          ImageIndex = 1
          TabVisible = False
          object pgcProtected: TPageControl
            Left = 0
            Top = 0
            Width = 704
            Height = 494
            ActivePage = tsProtectedDec
            Align = alClient
            Style = tsFlatButtons
            TabOrder = 0
            ExplicitWidth = 700
            ExplicitHeight = 493
            object tsProtectedDec: TTabSheet
              Caption = #22686#30410#21152#26222#36890#23646#24615
              object Label198: TLabel
                Left = 73
                Top = 399
                Width = 36
                Height = 12
                Caption = #26102#38388#65306
              end
              object Label202: TLabel
                Left = 174
                Top = 399
                Width = 60
                Height = 12
                Caption = #26102#38388#36882#22686#65306
              end
              object lbl10: TLabel
                Left = 423
                Top = 399
                Width = 36
                Height = 12
                Caption = #24310#26102#65306
              end
              object lbl17: TLabel
                Left = 149
                Top = 399
                Width = 12
                Height = 12
                Caption = #31186
              end
              object Label85: TLabel
                Left = 294
                Top = 399
                Width = 60
                Height = 12
                Caption = #36882#22686#21333#20301#65306
              end
              object chkProtectAddHPSlow: TCheckBox
                Left = 0
                Top = 371
                Width = 205
                Height = 20
                Caption = #21333#27425#21152'HP'#26102#20998#22810#27425#22238#34880#65307#22238#34880#27425#25968#65306
                TabOrder = 0
                OnClick = chkProtectAddHPSlowClick
              end
              object seProtectAddHPSlowCount: TSpinEditEx
                Left = 206
                Top = 370
                Width = 40
                Height = 21
                Hint = '0'#20351#29992#31995#32479#30340#22238#34880#22522#25968#65292#22823#20110'0'#25351#23450#27425#25968
                MaxValue = 1000
                MinValue = 0
                ParentShowHint = False
                ShowHint = True
                TabOrder = 1
                Value = 0
                OnChange = seProtectAddHPSlowCountChange
              end
              object vstProtectedAddAttr: TVirtualStringTree
                Left = 0
                Top = 0
                Width = 696
                Height = 367
                Align = alTop
                Colors.BorderColor = 15987699
                Colors.DisabledColor = clGray
                Colors.DropMarkColor = 15385233
                Colors.DropTargetColor = 15385233
                Colors.DropTargetBorderColor = 15987699
                Colors.FocusedSelectionColor = 15385233
                Colors.FocusedSelectionBorderColor = clWhite
                Colors.GridLineColor = 15987699
                Colors.HeaderHotColor = clBlack
                Colors.HotColor = clBlack
                Colors.SelectionRectangleBlendColor = 15385233
                Colors.SelectionRectangleBorderColor = 15385233
                Colors.SelectionTextColor = clBlack
                Colors.TreeLineColor = 9471874
                Colors.UnfocusedColor = 17813616
                Colors.UnfocusedSelectionColor = 15987699
                Colors.UnfocusedSelectionBorderColor = 15987699
                DefaultNodeHeight = 22
                Header.AutoSizeIndex = 0
                Header.Height = 37
                Header.Options = [hoColumnResize, hoDrag, hoShowSortGlyphs, hoVisible]
                Header.ParentFont = True
                LineStyle = lsSolid
                TabOrder = 2
                TreeOptions.MiscOptions = [toAcceptOLEDrop, toCheckSupport, toEditable, toFullRepaintOnResize, toGridExtensions, toInitOnSave, toToggleOnDblClick, toWheelPanning, toEditOnClick]
                TreeOptions.PaintOptions = [toShowButtons, toShowDropmark, toShowHorzGridLines, toShowTreeLines, toShowVertGridLines, toThemeAware, toUseBlendedImages, toFullVertGridLines]
                TreeOptions.SelectionOptions = [toExtendedFocus, toFullRowSelect]
                OnAfterCellPaint = vstProtectedAddAttrAfterCellPaint
                OnChecked = vstProtectedAddAttrChecked
                OnCreateEditor = vstProtectedAddAttrCreateEditor
                OnEditing = vstProtectedAddAttrEditing
                OnGetText = vstProtectedAddAttrGetText
                OnNodeClick = vstProtectedAddAttrNodeClick
                Columns = <
                  item
                    Position = 0
                    Width = 110
                    WideText = #21152#23646#24615
                  end
                  item
                    Alignment = taRightJustify
                    CaptionAlignment = taCenter
                    Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coWrapCaption, coUseCaptionAlignment]
                    Position = 1
                    Width = 40
                    WideText = #20960#29575#13#10'%'
                  end
                  item
                    Alignment = taRightJustify
                    CaptionAlignment = taCenter
                    Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coWrapCaption, coUseCaptionAlignment]
                    Position = 2
                    Width = 37
                    WideText = #20960#29575#13#10#36882#22686
                  end
                  item
                    Alignment = taRightJustify
                    CaptionAlignment = taCenter
                    Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coUseCaptionAlignment]
                    Position = 3
                    Width = 48
                    WideText = #19979#38480#20540
                  end
                  item
                    Alignment = taRightJustify
                    CaptionAlignment = taCenter
                    Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coWrapCaption, coUseCaptionAlignment]
                    Position = 4
                    Width = 36
                    WideText = #19979#38480#13#10#21333#20301
                  end
                  item
                    Alignment = taRightJustify
                    CaptionAlignment = taCenter
                    Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coWrapCaption, coUseCaptionAlignment]
                    Position = 5
                    Width = 37
                    WideText = #19979#38480#13#10#36882#22686
                  end
                  item
                    Alignment = taRightJustify
                    CaptionAlignment = taCenter
                    Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coUseCaptionAlignment]
                    Position = 6
                    Width = 48
                    WideText = #19978#38480#20540
                  end
                  item
                    Alignment = taRightJustify
                    CaptionAlignment = taCenter
                    Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coWrapCaption, coUseCaptionAlignment]
                    Position = 7
                    Width = 38
                    WideText = #19978#38480#13#10#21333#20301
                  end
                  item
                    Alignment = taRightJustify
                    CaptionAlignment = taCenter
                    Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coWrapCaption, coUseCaptionAlignment]
                    Position = 8
                    Width = 38
                    WideText = #19978#38480#13#10#36882#22686
                  end
                  item
                    Alignment = taRightJustify
                    CaptionAlignment = taCenter
                    Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coWrapCaption, coUseCaptionAlignment]
                    Position = 9
                    Width = 36
                    WideText = #26102#38388#13#10'('#31186')'
                  end
                  item
                    Alignment = taRightJustify
                    CaptionAlignment = taCenter
                    Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coWrapCaption, coUseCaptionAlignment]
                    Position = 10
                    Width = 38
                    WideText = #26102#38388#13#10#36882#22686
                  end
                  item
                    Alignment = taRightJustify
                    CaptionAlignment = taCenter
                    Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coWrapCaption, coUseCaptionAlignment]
                    Position = 11
                    Width = 38
                    WideText = #36882#22686#13#10#21333#20301
                  end
                  item
                    Alignment = taCenter
                    Position = 12
                    Width = 36
                    WideText = #25552#31034
                  end
                  item
                    Position = 13
                    Width = 158
                    WideText = #25552#31034#25991#23383
                  end>
              end
              object chkProtectTargetStatus: TCheckBox
                Tag = 9
                Left = 0
                Top = 397
                Width = 71
                Height = 16
                Hint = #21246#36873#21518#65292#20250#22312#30446#26631#19978#25345#32493#26174#31034#29366#24577#65292#26412#36873#39033#37197#21512#20854#20182#35843#25972#23646#24615#36873#39033#20351#29992
                Caption = #30446#26631#29366#24577
                TabOrder = 3
                OnClick = chkProtectTargetStatusClick
              end
              object seProtectTargetStatusTime: TSpinEditEx
                Tag = 9
                Left = 106
                Top = 394
                Width = 41
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 4
                Value = 0
                OnChange = seProtectTargetStatusTimeChange
              end
              object seProtectTargetStatusTime_2: TSpinEditEx
                Tag = 9
                Left = 231
                Top = 394
                Width = 41
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 5
                Value = 0
                OnChange = seProtectTargetStatusTime_2Change
              end
              object seProtectTargetStatusTimeDelay: TSpinEditEx
                Tag = 9
                Left = 454
                Top = 394
                Width = 65
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 6
                Value = 0
                OnChange = seProtectTargetStatusTimeDelayChange
              end
              object cbbProtectTargetStatusTime_1: TComboBox
                Tag = 9
                Left = 353
                Top = 395
                Width = 59
                Height = 20
                Style = csDropDownList
                TabOrder = 7
                OnChange = cbbProtectTargetStatusTime_1Change
                Items.Strings = (
                  #31186
                  #28857)
              end
            end
            object tsProtectedAddElement: TTabSheet
              Caption = #22686#30410#21152#20803#32032#23646#24615
              ImageIndex = 1
              object vstAddElement: TVirtualStringTree
                Left = 0
                Top = 0
                Width = 696
                Height = 463
                Align = alClient
                Colors.BorderColor = 15987699
                Colors.DisabledColor = clGray
                Colors.DropMarkColor = 15385233
                Colors.DropTargetColor = 15385233
                Colors.DropTargetBorderColor = 15987699
                Colors.FocusedSelectionColor = 15385233
                Colors.FocusedSelectionBorderColor = clWhite
                Colors.GridLineColor = 15987699
                Colors.HeaderHotColor = clBlack
                Colors.HotColor = clBlack
                Colors.SelectionRectangleBlendColor = 15385233
                Colors.SelectionRectangleBorderColor = 15385233
                Colors.SelectionTextColor = clBlack
                Colors.TreeLineColor = 9471874
                Colors.UnfocusedColor = 17813616
                Colors.UnfocusedSelectionColor = 15987699
                Colors.UnfocusedSelectionBorderColor = 15987699
                DefaultNodeHeight = 22
                Header.AutoSizeIndex = 10
                Header.Height = 37
                Header.Options = [hoAutoResize, hoColumnResize, hoDrag, hoShowSortGlyphs, hoVisible]
                Header.ParentFont = True
                LineStyle = lsSolid
                TabOrder = 0
                TreeOptions.MiscOptions = [toAcceptOLEDrop, toCheckSupport, toEditable, toFullRepaintOnResize, toGridExtensions, toInitOnSave, toToggleOnDblClick, toWheelPanning, toEditOnClick]
                TreeOptions.PaintOptions = [toShowButtons, toShowDropmark, toShowHorzGridLines, toShowTreeLines, toShowVertGridLines, toThemeAware, toUseBlendedImages, toFullVertGridLines]
                TreeOptions.SelectionOptions = [toExtendedFocus, toFullRowSelect]
                OnAfterCellPaint = vstDecElementAfterCellPaint
                OnChecked = vstDecElementChecked
                OnCreateEditor = vstDecElementCreateEditor
                OnEditing = vstDecElementEditing
                OnGetText = vstDecElementGetText
                OnNodeClick = vstDecElementNodeClick
                ExplicitHeight = 471
                Columns = <
                  item
                    Position = 0
                    Width = 133
                    WideText = #21152#20803#32032#23646#24615
                  end
                  item
                    Alignment = taRightJustify
                    CaptionAlignment = taCenter
                    Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coWrapCaption, coUseCaptionAlignment]
                    Position = 1
                    Width = 40
                    WideText = #20960#29575#13#10'%'
                  end
                  item
                    Alignment = taRightJustify
                    CaptionAlignment = taCenter
                    Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coWrapCaption, coUseCaptionAlignment]
                    Position = 2
                    Width = 40
                    WideText = #20960#29575#13#10#36882#22686
                  end
                  item
                    Alignment = taRightJustify
                    CaptionAlignment = taCenter
                    Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coUseCaptionAlignment]
                    Position = 3
                    Width = 38
                    WideText = #21152#20540
                  end
                  item
                    Alignment = taRightJustify
                    CaptionAlignment = taCenter
                    Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coWrapCaption, coUseCaptionAlignment]
                    Position = 4
                    Width = 46
                    WideText = #21152#20540#13#10#21333#20301
                  end
                  item
                    Alignment = taRightJustify
                    CaptionAlignment = taCenter
                    Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coWrapCaption, coUseCaptionAlignment]
                    Position = 5
                    Width = 45
                    WideText = #21152#20540#13#10#36882#22686
                  end
                  item
                    Alignment = taRightJustify
                    CaptionAlignment = taCenter
                    Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coWrapCaption, coUseCaptionAlignment]
                    Position = 6
                    Width = 44
                    WideText = #26102#38388#13#10'('#31186')'
                  end
                  item
                    Alignment = taRightJustify
                    CaptionAlignment = taCenter
                    Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coWrapCaption, coUseCaptionAlignment]
                    Position = 7
                    Width = 44
                    WideText = #26102#38388#13#10#36882#22686
                  end
                  item
                    Alignment = taRightJustify
                    CaptionAlignment = taCenter
                    Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coWrapCaption, coUseCaptionAlignment]
                    Position = 8
                    Width = 38
                    WideText = #36882#22686#13#10#21333#20301
                  end
                  item
                    Alignment = taCenter
                    Position = 9
                    Width = 37
                    WideText = #25552#31034
                  end
                  item
                    Position = 10
                    Width = 191
                    WideText = #25552#31034#25991#23383
                  end>
              end
            end
          end
        end
      end
      object pnlMagicServer: TPanel
        Left = 0
        Top = 0
        Width = 702
        Height = 145
        Align = alTop
        BevelOuter = bvNone
        TabOrder = 1
        ExplicitWidth = 708
        object lbl12: TLabel
          Left = 9
          Top = 7
          Width = 60
          Height = 12
          Caption = #25805#20316#27169#24335#65306
        end
        object lblAttackDelay: TLabel
          Left = 293
          Top = 7
          Width = 84
          Height = 12
          Caption = #20260#23475#24310#26102#26102#38388#65306
          Font.Charset = GB2312_CHARSET
          Font.Color = clBlue
          Font.Height = -12
          Font.Name = #23435#20307
          Font.Style = []
          ParentFont = False
        end
        object lblAttackDelayTime: TLabel
          Left = 434
          Top = 7
          Width = 24
          Height = 12
          Caption = #27627#31186
        end
        object lblProtectTargetRangeTitle: TLabel
          Left = 293
          Top = 7
          Width = 84
          Height = 12
          Caption = #22686#30410#30446#26631#33539#22260#65306
          Visible = False
        end
        object lblProtectTargetRangeValue: TLabel
          Left = 432
          Top = 7
          Width = 12
          Height = 12
          Caption = #26684
          Visible = False
        end
        object cbbOperateMode: TComboBox
          Left = 68
          Top = 3
          Width = 103
          Height = 20
          Style = csDropDownList
          TabOrder = 0
          OnChange = cbbOperateModeChange
        end
        object grpInterval: TGroupBox
          Left = 2
          Top = 25
          Width = 710
          Height = 68
          Caption = #25216#33021#20351#29992#38388#38548
          TabOrder = 1
          object Label32: TLabel
            Left = 8
            Top = 21
            Width = 60
            Height = 12
            Caption = #20351#29992#38388#38548#65306
          end
          object Label33: TLabel
            Left = 372
            Top = 21
            Width = 60
            Height = 12
            Caption = #22833#36133#25552#31034#65306
          end
          object Label34: TLabel
            Left = 372
            Top = 45
            Width = 60
            Height = 12
            Caption = #25104#21151#25552#31034#65306
          end
          object Label35: TLabel
            Left = 8
            Top = 45
            Width = 60
            Height = 12
            Caption = #20851#38381#25552#31034#65306
          end
          object lbl20: TLabel
            Left = 129
            Top = 21
            Width = 84
            Height = 12
            Caption = #21097#20313#26102#38388#21464#37327'%d'
            Font.Charset = GB2312_CHARSET
            Font.Color = clRed
            Font.Height = -12
            Font.Name = #23435#20307
            Font.Style = []
            ParentFont = False
          end
          object seUseInterval: TSpinEditEx
            Left = 65
            Top = 16
            Width = 59
            Height = 21
            MaxValue = 0
            MinValue = 0
            TabOrder = 0
            Value = 0
            OnChange = seUseIntervalChange
          end
          object chkFailNoShowEff: TCheckBox
            Left = 140
            Top = 40
            Width = 177
            Height = 18
            Caption = #26102#38388#26410#21040#25110#22833#36133#26102#19981#26174#31034#39764#27861
            TabOrder = 1
            Visible = False
            OnClick = chkFailNoShowEffClick
          end
          object edtFailMsg: TEdit
            Left = 429
            Top = 17
            Width = 260
            Height = 20
            TabOrder = 2
            OnChange = edtFailMsgChange
          end
          object edtCloseMsg: TEdit
            Left = 65
            Top = 41
            Width = 260
            Height = 20
            TabOrder = 4
            OnChange = edtCloseMsgChange
          end
          object edtSucceedMsg: TEdit
            Left = 429
            Top = 41
            Width = 260
            Height = 20
            TabOrder = 3
            OnChange = edtSucceedMsgChange
          end
        end
        object seAttackDelayTime: TSpinEditEx
          Left = 374
          Top = 2
          Width = 56
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 2
          Value = 0
          OnChange = seAttackDelayTimeChange
        end
        object chkAttackUseNG: TCheckBox
          Left = 183
          Top = 5
          Width = 104
          Height = 15
          Caption = #20351#29992#20869#21151#20540#37322#25918
          TabOrder = 3
          OnClick = chkAttackUseNGClick
        end
        object grpNeedItem: TGroupBox
          Left = 2
          Top = 96
          Width = 710
          Height = 47
          Hint = #25915#26432#27169#24335#30340#25216#33021#23558#19981#20250#26816#27979#25216#33021#20351#29992#26465#20214
          Caption = #20351#29992#26465#20214
          TabOrder = 4
          object lblNeedItem: TLabel
            Left = 86
            Top = 21
            Width = 72
            Height = 12
            Caption = #38656#35201#30340#29289#21697#65306
          end
          object lblNeedItemCount: TLabel
            Left = 258
            Top = 21
            Width = 60
            Height = 12
            Caption = #29289#21697#25968#37327#65306
          end
          object lblNeedItemCustomItemName: TLabel
            Left = 403
            Top = 21
            Width = 72
            Height = 12
            Caption = #33258#23450#20041#29289#21697#65306
          end
          object lblCheckVarName: TLabel
            Left = 87
            Top = 53
            Width = 72
            Height = 12
            Caption = #24453#26816#27979#21464#37327#65306
            Visible = False
          end
          object lblCheckVarType: TLabel
            Left = 259
            Top = 53
            Width = 60
            Height = 12
            Caption = #26816#27979#26041#24335#65306
            Visible = False
          end
          object lblCheckVarValue: TLabel
            Left = 403
            Top = 53
            Width = 72
            Height = 12
            Caption = #24453#26816#27979#30340#20540#65306
            Visible = False
          end
          object lblCheckVarAdd: TLabel
            Left = 574
            Top = 53
            Width = 60
            Height = 12
            Caption = #21464#37327#35843#25972#65306
            Visible = False
          end
          object cbbNeedItem: TComboBox
            Left = 154
            Top = 17
            Width = 95
            Height = 20
            Style = csDropDownList
            TabOrder = 1
            OnChange = cbbNeedItemChange
          end
          object seNeedItemCount: TSpinEditEx
            Left = 316
            Top = 17
            Width = 61
            Height = 21
            Hint = #25968#37327#26159#25351#20943#29289#21697#25345#20037#25968#37327#65292#35774#32622#20026'3'#34920#31034#65306'3*'#27602#31526#25345#20037#27604#20363' '#13#10' (M2'#8212'>'#21151#33021#35774#32622#8212'>'#25216#33021#39764#27861#8212'>'#27602#31526#25345#20037#27604#20363')'
            MaxValue = 0
            MinValue = 0
            ParentShowHint = False
            ShowHint = True
            TabOrder = 0
            Value = 0
            OnChange = seNeedItemCountChange
          end
          object edtNeedItemCustomItemName: TEdit
            Left = 471
            Top = 17
            Width = 94
            Height = 20
            TabOrder = 2
            OnChange = edtNeedItemCustomItemNameChange
          end
          object chkNeedItemUseBagItem: TCheckBox
            Left = 574
            Top = 19
            Width = 119
            Height = 17
            Hint = #19981#21246#36873#21017#21482#20351#29992#35013#22791#29289#21697#65292#21246#36873#21482#20351#29992#35013#22791#29289#21697#25110#21253#35065#29289#21697
            Caption = #20801#35768#20351#29992#21253#35065#29289#21697
            ParentShowHint = False
            ShowHint = True
            TabOrder = 3
            OnClick = chkNeedItemUseBagItemClick
          end
          object chkCheckVarValue: TCheckBox
            Left = 9
            Top = 19
            Width = 74
            Height = 17
            Hint = #21246#36873#34920#31034#26816#27979#21464#37327#65292#19981#21246#36873#21017#26816#27979#20329#25140
            Caption = #26816#27979#21464#37327
            ParentShowHint = False
            ShowHint = True
            TabOrder = 4
            OnClick = chkCheckVarValueClick
          end
          object edtCheckVarName: TEdit
            Left = 155
            Top = 49
            Width = 93
            Height = 20
            MaxLength = 20
            TabOrder = 5
            Visible = False
            OnChange = edtCheckVarNameChange
          end
          object cbbCheckVarType: TComboBox
            Left = 317
            Top = 49
            Width = 61
            Height = 20
            Style = csDropDownList
            TabOrder = 6
            Visible = False
            OnChange = cbbCheckVarTypeChange
          end
          object seCheckVarValue: TSpinEditEx
            Left = 471
            Top = 49
            Width = 93
            Height = 21
            Hint = #22914#65306#24453#26816#27979#21464#37327#65306'N1'#65307#26816#27979#26041#24335#65306'>'#65307#26816#27979#20540#65306'3'#13#10#13#10#34920#31034#28385#36275#26465#20214#65306'N1>3'#26102#25165#33021#20351#29992#25216#33021
            MaxValue = 0
            MinValue = 0
            ParentShowHint = False
            ShowHint = True
            TabOrder = 7
            Value = 0
            Visible = False
            OnChange = seCheckVarValueChange
          end
          object seCheckVarAdd: TSpinEditEx
            Left = 632
            Top = 49
            Width = 56
            Height = 21
            Hint = #24403#28385#36275#21464#37327#26816#27979#26465#20214#24182#20351#29992#25216#33021#26102#65292#23545#21464#37327#30340#20540#36827#34892'+/-'#25805#20316#13#10#13#10#22914#65306#21464#37327#35843#25972#22635'-3'#65292#21017#20351#29992#25216#33021#21518#65292#21464#37327#30340#20540'-3'
            MaxValue = 0
            MinValue = 0
            ParentShowHint = False
            ShowHint = True
            TabOrder = 8
            Value = 0
            Visible = False
            OnChange = seCheckVarAddChange
          end
        end
        object seProtectTargetRange: TSpinEditEx
          Left = 374
          Top = 2
          Width = 56
          Height = 21
          Hint = #20197#22686#30410#30446#26631#20026#20013#24515#65292#22686#30410#30340#38431#21451#33539#22260
          MaxValue = 9
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 5
          Value = 0
          Visible = False
          OnChange = seProtectTargetRangeChange
        end
        object chkAttackNoChangeDir: TCheckBox
          Left = 471
          Top = 5
          Width = 119
          Height = 15
          Hint = #27492#36873#39033#20027#35201#29992#20110#37326#34542#20914#25758#25216#33021#65292#25915#20987#30446#26631#20063#19981#20877#26159#40736#26631#20301#32622
          Caption = #25216#33021#19981#25913#20154#29289#26041#21521
          ParentShowHint = False
          ShowHint = True
          TabOrder = 6
          OnClick = chkAttackNoChangeDirClick
        end
        object chkDisableInSafeZone: TCheckBox
          Left = 598
          Top = 4
          Width = 101
          Height = 17
          Caption = #31105#27490#23433#20840#21306#20351#29992
          TabOrder = 7
          OnClick = chkDisableInSafeZoneClick
        end
      end
    end
  end
  object pnlBottom: TPanel
    Left = 0
    Top = 659
    Width = 828
    Height = 30
    Align = alBottom
    BevelOuter = bvNone
    TabOrder = 2
    ExplicitTop = 676
    ExplicitWidth = 834
    object lbl13: TLabel
      Left = 3
      Top = 8
      Width = 407
      Height = 12
      Caption = #33258#23450#20041#25216#33021#65306'MagID>=1000'#19988'<1300'#65292#27599#20010#33258#23450#20041#25216#33021#30340'MagID'#24517#39035#19981#21516
      Font.Charset = GB2312_CHARSET
      Font.Color = clRed
      Font.Height = -12
      Font.Name = #23435#20307
      Font.Style = [fsBold]
      ParentFont = False
    end
    object btnSave: TButton
      Left = 635
      Top = 2
      Width = 55
      Height = 25
      Caption = #20445#23384
      Enabled = False
      TabOrder = 0
      OnClick = btnSaveClick
    end
    object chkSendCustomMagicConfig: TCheckBox
      Left = 431
      Top = 6
      Width = 186
      Height = 17
      Hint = #22914#26524#38598#25104#21040#30331#24405#22120#21017#21487#20197#19981#21457#36865
      Caption = #21457#36865#33258#23450#20041#25216#33021#37197#32622#21040#23458#25143#31471
      ParentShowHint = False
      ShowHint = True
      TabOrder = 2
      OnClick = chkSendCustomMagicConfigClick
    end
    object btnMakeConfigData: TButton
      Left = 697
      Top = 2
      Width = 129
      Height = 25
      Caption = #29983#25104#30331#24405#22120#38598#25104#25991#20214
      TabOrder = 1
      OnClick = btnMakeConfigDataClick
    end
  end
  object txtMagicWarr: TStaticText
    Left = 288
    Top = 4
    Width = 217
    Height = 14
    AutoSize = False
    TabOrder = 3
  end
  object dlgSaveMagics: TSaveDialog
    Filter = #25216#33021#37197#32622#25991#20214'(*.dat)|*.dat'
    Title = #29983#25104#30331#24405#22120#37197#32622#25991#20214
    Left = 419
    Top = 315
  end
  object ilCheck: TImageList
    Height = 13
    Width = 13
    Left = 387
    Top = 315
    Bitmap = {
      494C010102000A0004000D000D00FFFFFFFFFF10FFFFFFFFFFFFFFFF424D3600
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
