object FrmBatchEditAccountInfo: TFrmBatchEditAccountInfo
  Left = 511
  Top = 429
  Width = 670
  Height = 576
  BorderWidth = 3
  Caption = #25209#37327#31105#29992#24080#25143
  Color = clBtnFace
  Constraints.MaxWidth = 670
  Constraints.MinWidth = 670
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'Tahoma'
  Font.Style = []
  OldCreateOrder = False
  OnCreate = FormCreate
  PixelsPerInch = 96
  TextHeight = 14
  object pnlLeft: TPanel
    Left = 0
    Top = 0
    Width = 314
    Height = 531
    Align = alLeft
    BevelOuter = bvNone
    BorderWidth = 3
    TabOrder = 0
    object vstEnableAccount: TVirtualStringTree
      Left = 3
      Top = 36
      Width = 308
      Height = 492
      Align = alClient
      DefaultNodeHeight = 22
      Header.AutoSizeIndex = 0
      Header.Font.Charset = DEFAULT_CHARSET
      Header.Font.Color = clWindowText
      Header.Font.Height = -12
      Header.Font.Name = 'Tahoma'
      Header.Font.Style = []
      Header.Height = 20
      Header.MainColumn = -1
      TabOrder = 0
      TreeOptions.MiscOptions = [toAcceptOLEDrop, toCheckSupport, toFullRepaintOnResize, toInitOnSave, toToggleOnDblClick, toWheelPanning]
      TreeOptions.PaintOptions = [toShowDropmark, toThemeAware, toUseBlendedImages]
      TreeOptions.SelectionOptions = [toFullRowSelect, toMultiSelect]
      OnBeforeItemErase = vstEnableAccountBeforeItemErase
      OnGetText = vstEnableAccountGetText
      Columns = <>
    end
    object pnlLeftTitle: TPanel
      Left = 3
      Top = 3
      Width = 308
      Height = 33
      Align = alTop
      Alignment = taLeftJustify
      BevelOuter = bvNone
      ParentColor = True
      TabOrder = 1
      object lbl1: TLabel
        Left = 1
        Top = 8
        Width = 60
        Height = 14
        Caption = #24050#21551#29992#24080#25143
        Font.Charset = DEFAULT_CHARSET
        Font.Color = clBlue
        Font.Height = -12
        Font.Name = 'Tahoma'
        Font.Style = []
        ParentFont = False
      end
      object btnSetDisable: TButton
        Left = 255
        Top = 2
        Width = 51
        Height = 24
        Caption = #31105#29992
        TabOrder = 0
        OnClick = btnSetDisableClick
      end
      object btn2: TButton
        Left = 198
        Top = 2
        Width = 51
        Height = 24
        Caption = #23548#20837
        TabOrder = 1
        OnClick = btn2Click
      end
      object edtEnabledAccount: TEdit
        Left = 68
        Top = 3
        Width = 130
        Height = 21
        Font.Charset = DEFAULT_CHARSET
        Font.Color = clBlack
        Font.Height = -12
        Font.Name = 'Tahoma'
        Font.Style = []
        ParentFont = False
        TabOrder = 2
        OnChange = edtEnabledAccountChange
      end
    end
  end
  object Panel1: TPanel
    Left = 316
    Top = 0
    Width = 332
    Height = 531
    Align = alRight
    BevelOuter = bvNone
    BorderWidth = 3
    TabOrder = 1
    object pnlRightTitle: TPanel
      Left = 3
      Top = 3
      Width = 326
      Height = 33
      Align = alTop
      Alignment = taLeftJustify
      BevelOuter = bvNone
      TabOrder = 0
      object Label1: TLabel
        Left = 1
        Top = 8
        Width = 60
        Height = 14
        Caption = #24050#31105#29992#24080#25143
        Font.Charset = DEFAULT_CHARSET
        Font.Color = clRed
        Font.Height = -12
        Font.Name = 'Tahoma'
        Font.Style = []
        ParentFont = False
      end
      object btnSetEnabled: TButton
        Left = 264
        Top = 2
        Width = 58
        Height = 24
        Caption = #35299#31105
        TabOrder = 0
        OnClick = btnSetEnabledClick
      end
      object edtDisableAccount: TEdit
        Left = 67
        Top = 3
        Width = 190
        Height = 21
        Font.Charset = DEFAULT_CHARSET
        Font.Color = clBlack
        Font.Height = -12
        Font.Name = 'Tahoma'
        Font.Style = []
        ParentFont = False
        TabOrder = 1
        OnChange = edtDisableAccountChange
      end
    end
    object vstDisableAccount: TVirtualStringTree
      Left = 3
      Top = 36
      Width = 326
      Height = 492
      Align = alClient
      DefaultNodeHeight = 22
      Header.AutoSizeIndex = 0
      Header.Font.Charset = DEFAULT_CHARSET
      Header.Font.Color = clWindowText
      Header.Font.Height = -12
      Header.Font.Name = 'Tahoma'
      Header.Font.Style = []
      Header.Height = 20
      Header.MainColumn = -1
      TabOrder = 1
      TreeOptions.MiscOptions = [toAcceptOLEDrop, toCheckSupport, toFullRepaintOnResize, toInitOnSave, toToggleOnDblClick, toWheelPanning]
      TreeOptions.PaintOptions = [toShowDropmark, toThemeAware, toUseBlendedImages]
      TreeOptions.SelectionOptions = [toFullRowSelect, toMultiSelect]
      OnBeforeItemErase = vstEnableAccountBeforeItemErase
      OnGetText = vstEnableAccountGetText
      Columns = <>
    end
  end
  object dlgOpen: TOpenDialog
    Filter = #25991#26412#25991#20214'(*.txt; *.ini)|*.txt;*.ini'
    Title = #23548#20837#24453#31105#29992#30340#24080#25143#25991#20214
    Left = 112
    Top = 160
  end
end
