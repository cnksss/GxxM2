object FrmHeroMagicSetting: TFrmHeroMagicSetting
  Left = 601
  Top = 363
  BorderStyle = bsDialog
  BorderWidth = 4
  Caption = #33521#38596#25216#33021#35774#32622
  ClientHeight = 620
  ClientWidth = 912
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'Tahoma'
  Font.Style = []
  OldCreateOrder = False
  Position = poMainFormCenter
  OnCreate = FormCreate
  PixelsPerInch = 96
  TextHeight = 13
  object pgcMain: TPageControl
    Left = 0
    Top = 0
    Width = 912
    Height = 584
    ActivePage = ts1
    Align = alClient
    TabOrder = 0
    object ts1: TTabSheet
      BorderWidth = 4
      Caption = #25112#22763#25216#33021
      object vstWarrMagic: TVirtualStringTree
        Left = 0
        Top = 0
        Width = 896
        Height = 548
        Align = alClient
        DefaultNodeHeight = 22
        Header.AutoSizeIndex = 6
        Header.Font.Charset = DEFAULT_CHARSET
        Header.Font.Color = clWindowText
        Header.Font.Height = -12
        Header.Font.Name = 'Tahoma'
        Header.Font.Style = []
        Header.Height = 26
        Header.Options = [hoAutoResize, hoColumnResize, hoDrag, hoShowSortGlyphs, hoVisible]
        HintMode = hmTooltip
        Margin = 2
        ParentShowHint = False
        ShowHint = True
        TabOrder = 0
        TreeOptions.MiscOptions = [toAcceptOLEDrop, toCheckSupport, toEditable, toFullRepaintOnResize, toGridExtensions, toInitOnSave, toToggleOnDblClick, toWheelPanning, toEditOnClick]
        TreeOptions.PaintOptions = [toShowButtons, toShowDropmark, toShowHorzGridLines, toShowTreeLines, toShowVertGridLines, toThemeAware, toUseBlendedImages, toFullVertGridLines]
        TreeOptions.SelectionOptions = [toExtendedFocus, toFullRowSelect]
        OnChecked = vstWarrMagicChecked
        OnCreateEditor = vstWarrMagicCreateEditor
        OnDrawText = vstWarrMagicDrawText
        OnEditing = vstWarrMagicEditing
        OnFreeNode = vstWarrMagicFreeNode
        OnGetText = vstWarrMagicGetText
        OnKeyUp = vstWarrMagicKeyUp
        OnNodeClick = vstWarrMagicNodeClick
        Columns = <
          item
            Position = 0
            Width = 30
          end
          item
            Margin = 2
            Position = 1
            Width = 100
            WideText = #25216#33021#21517#31216
          end
          item
            Position = 2
            Width = 80
            WideText = #25216#33021#31867#22411
          end
          item
            Alignment = taCenter
            Margin = 2
            Position = 3
            Width = 60
            WideText = #20351#29992#20960#29575
          end
          item
            Position = 4
            Width = 60
            WideText = #25915#20987#33539#22260
          end
          item
            Position = 5
            Width = 60
            WideText = #20351#29992#30446#26631
          end
          item
            CaptionAlignment = taCenter
            Margin = 2
            Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coUseCaptionAlignment]
            Position = 6
            Width = 442
            WideText = #20351#29992#26465#20214
          end
          item
            Alignment = taCenter
            CaptionAlignment = taCenter
            Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coShowDropMark, coVisible, coAllowFocus, coUseCaptionAlignment]
            Position = 7
            Width = 60
            WideText = #35774#32622#26465#20214
          end>
        WideDefaultText = ''
      end
    end
    object ts2: TTabSheet
      BorderWidth = 4
      Caption = #27861#24072#25216#33021
      ImageIndex = 1
      object vstWizardMagic: TVirtualStringTree
        Tag = 1
        Left = 0
        Top = 0
        Width = 896
        Height = 548
        Align = alClient
        DefaultNodeHeight = 22
        Header.AutoSizeIndex = 6
        Header.Font.Charset = DEFAULT_CHARSET
        Header.Font.Color = clWindowText
        Header.Font.Height = -12
        Header.Font.Name = 'Tahoma'
        Header.Font.Style = []
        Header.Height = 26
        Header.Options = [hoAutoResize, hoColumnResize, hoDrag, hoShowSortGlyphs, hoVisible]
        Margin = 2
        TabOrder = 0
        TreeOptions.MiscOptions = [toAcceptOLEDrop, toCheckSupport, toEditable, toFullRepaintOnResize, toGridExtensions, toInitOnSave, toToggleOnDblClick, toWheelPanning, toEditOnClick]
        TreeOptions.PaintOptions = [toShowButtons, toShowDropmark, toShowHorzGridLines, toShowTreeLines, toShowVertGridLines, toThemeAware, toUseBlendedImages, toFullVertGridLines]
        TreeOptions.SelectionOptions = [toExtendedFocus, toFullRowSelect]
        OnChecked = vstWarrMagicChecked
        OnCreateEditor = vstWarrMagicCreateEditor
        OnDrawText = vstWarrMagicDrawText
        OnEditing = vstWarrMagicEditing
        OnFreeNode = vstWarrMagicFreeNode
        OnGetText = vstWarrMagicGetText
        OnKeyUp = vstWarrMagicKeyUp
        OnNodeClick = vstWarrMagicNodeClick
        Columns = <
          item
            Position = 0
            Width = 30
          end
          item
            Margin = 2
            Position = 1
            Width = 100
            WideText = #25216#33021#21517#31216
          end
          item
            Position = 2
            Width = 80
            WideText = #25216#33021#31867#22411
          end
          item
            Alignment = taCenter
            Margin = 2
            Position = 3
            Width = 60
            WideText = #20351#29992#20960#29575
          end
          item
            Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coAllowFocus]
            Position = 4
            WideText = #25915#20987#33539#22260
          end
          item
            Position = 5
            Width = 60
            WideText = #20351#29992#30446#26631
          end
          item
            CaptionAlignment = taCenter
            Margin = 2
            Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coUseCaptionAlignment]
            Position = 6
            Width = 502
            WideText = #20351#29992#26465#20214
          end
          item
            Alignment = taCenter
            Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coShowDropMark, coVisible, coAllowFocus]
            Position = 7
            Width = 60
            WideText = #35774#32622#26465#20214
          end>
        WideDefaultText = ''
      end
    end
    object ts3: TTabSheet
      BorderWidth = 4
      Caption = #36947#22763#25216#33021
      ImageIndex = 2
      object vstTaosMagic: TVirtualStringTree
        Tag = 2
        Left = 0
        Top = 0
        Width = 896
        Height = 548
        Align = alClient
        DefaultNodeHeight = 22
        Header.AutoSizeIndex = 6
        Header.Font.Charset = DEFAULT_CHARSET
        Header.Font.Color = clWindowText
        Header.Font.Height = -12
        Header.Font.Name = 'Tahoma'
        Header.Font.Style = []
        Header.Height = 26
        Header.Options = [hoAutoResize, hoColumnResize, hoDrag, hoShowSortGlyphs, hoVisible]
        Margin = 2
        TabOrder = 0
        TreeOptions.MiscOptions = [toAcceptOLEDrop, toCheckSupport, toEditable, toFullRepaintOnResize, toGridExtensions, toInitOnSave, toToggleOnDblClick, toWheelPanning, toEditOnClick]
        TreeOptions.PaintOptions = [toShowButtons, toShowDropmark, toShowHorzGridLines, toShowTreeLines, toShowVertGridLines, toThemeAware, toUseBlendedImages, toFullVertGridLines]
        TreeOptions.SelectionOptions = [toExtendedFocus, toFullRowSelect]
        OnChecked = vstWarrMagicChecked
        OnCreateEditor = vstWarrMagicCreateEditor
        OnDrawText = vstWarrMagicDrawText
        OnEditing = vstWarrMagicEditing
        OnFreeNode = vstWarrMagicFreeNode
        OnGetText = vstWarrMagicGetText
        OnKeyUp = vstWarrMagicKeyUp
        OnNodeClick = vstWarrMagicNodeClick
        Columns = <
          item
            Position = 0
            Width = 30
          end
          item
            Margin = 2
            Position = 1
            Width = 100
            WideText = #25216#33021#21517#31216
          end
          item
            Position = 2
            Width = 80
            WideText = #25216#33021#31867#22411
          end
          item
            Alignment = taCenter
            Margin = 2
            Position = 3
            Width = 60
            WideText = #20351#29992#20960#29575
          end
          item
            Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coAllowFocus]
            Position = 4
            WideText = #25915#20987#33539#22260
          end
          item
            Position = 5
            Width = 70
            WideText = #20351#29992#30446#26631
          end
          item
            Alignment = taCenter
            Margin = 2
            Position = 6
            Width = 492
            WideText = #20351#29992#26465#20214
          end
          item
            CaptionAlignment = taCenter
            Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coShowDropMark, coVisible, coAllowFocus, coUseCaptionAlignment]
            Position = 7
            Width = 60
            WideText = #35774#32622#26465#20214
          end>
        WideDefaultText = ''
      end
    end
  end
  object pnl1: TPanel
    Left = 0
    Top = 584
    Width = 912
    Height = 36
    Align = alBottom
    BevelOuter = bvNone
    TabOrder = 1
    object lbl34: TLabel
      Left = 4
      Top = 7
      Width = 792
      Height = 26
      Caption = 
        'Ctrl+Insert:'#28155#21152#33258#23450#20041#25216#33021#65307'Ctrl+Delete:'#21024#38500#33258#23450#20041#25216#33021#65307#33258#23450#20041#25216#33021#20998#8220#25112#22763#25216#33021#8221#21644#8220#38750#25112#22763#25216#33021#8221#65292#35814#24773#21442 +
        #29031#33258#23450#20041#25216#33021#39029#38754#13#10#33521#38596#33258#23450#20041#25216#33021#65292#38656#35201#22312#25968#25454#24211#20013#22797#21046#19968#26465'MagicID'#19982#20154#29289#33258#23450#20041#25216#33021#30340'MagicID'#30456#21516#19988'Descr='#39#33521#38596 +
        #39#30340#35760#24405
      Font.Charset = GB2312_CHARSET
      Font.Color = clRed
      Font.Height = -13
      Font.Name = #23435#20307
      Font.Style = []
      ParentFont = False
    end
    object btnSave: TButton
      Left = 830
      Top = 5
      Width = 81
      Height = 27
      Caption = #20445#23384
      TabOrder = 0
      OnClick = btnSaveClick
    end
  end
end
