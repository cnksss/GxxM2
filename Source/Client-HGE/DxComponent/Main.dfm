object FrmMain: TFrmMain
  Left = 784
  Top = 118
  BorderIcons = [biSystemMenu, biMinimize]
  BorderStyle = bsSingle
  Caption = 'GUI'#32534#36753#24037#20855
  ClientHeight = 600
  ClientWidth = 800
  Color = clBtnFace
  Font.Charset = GB2312_CHARSET
  Font.Color = clWindowText
  Font.Height = -12
  Font.Name = #23435#20307
  Font.Style = []
  OldCreateOrder = False
  PopupMenu = PopupMenuMain
  Position = poScreenCenter
  OnCloseQuery = FormCloseQuery
  OnCreate = FormCreate
  OnDblClick = FormDblClick
  OnDestroy = FormDestroy
  OnKeyDown = FormKeyDown
  OnKeyPress = FormKeyPress
  OnKeyUp = FormKeyUp
  OnMouseDown = FormMouseDown
  OnMouseMove = FormMouseMove
  OnMouseUp = FormMouseUp
  PixelsPerInch = 96
  TextHeight = 12
  object PopupMenuMain: TPopupMenu
    OnPopup = PopupMenuMainPopup
    Left = 80
    Top = 48
    object Menu_MainPage: TMenuItem
      Caption = #36820#22238#39318#39029
      OnClick = Menu_MainPageClick
    end
    object N7: TMenuItem
      Caption = '-'
    end
    object Menu_LoadFromFile: TMenuItem
      Caption = #25171#24320
      OnClick = Menu_LoadFromFileClick
    end
    object N6: TMenuItem
      Caption = '-'
    end
    object Menu_Background: TMenuItem
      Caption = #26597#30475
    end
    object Menu_Create: TMenuItem
      Caption = #26032#24314
      object Menu_CreateTDxImageForm: TMenuItem
        Tag = 1
        Caption = 'TDxImageForm'
        OnClick = OnCreateDxControlClick
      end
      object Menu_CreateTDxImageButton: TMenuItem
        Tag = 2
        Caption = 'TDxImageButton'
        OnClick = OnCreateDxControlClick
      end
      object Menu_CreateTDxPageControl: TMenuItem
        Tag = 10
        Caption = 'TDxPageControl'
        OnClick = OnCreateDxControlClick
      end
      object Menu_CreateTDxImageGrid: TMenuItem
        Tag = 5
        Caption = 'TDxImageGrid'
        OnClick = OnCreateDxControlClick
      end
      object Menu_CreateTDxComboBox: TMenuItem
        Tag = 9
        Caption = 'TDxComboBox'
        OnClick = OnCreateDxControlClick
      end
      object Menu_CreateTDxLabel: TMenuItem
        Tag = 4
        Caption = 'TDxLabel'
        OnClick = OnCreateDxControlClick
      end
      object Menu_CreateTDxEdit: TMenuItem
        Tag = 3
        Caption = 'TDxEdit'
        OnClick = OnCreateDxControlClick
      end
      object Menu_CreateTDxMemo: TMenuItem
        Tag = 6
        Caption = 'TDxMemo'
        OnClick = OnCreateDxControlClick
      end
      object Menu_CreateTDxChatMemo: TMenuItem
        Tag = 7
        Caption = 'TDxChatMemo'
        OnClick = OnCreateDxControlClick
      end
      object Menu_CreateTDxListView: TMenuItem
        Tag = 13
        Caption = 'TDxListView'
        OnClick = OnCreateDxControlClick
      end
      object Menu_CreateTDxTreeView: TMenuItem
        Tag = 12
        Caption = 'TDxTreeView'
        OnClick = OnCreateDxControlClick
      end
      object Menu_CreateTDxPopupMenu: TMenuItem
        Tag = 8
        Caption = 'TDxPopupMenu'
        OnClick = OnCreateDxControlClick
      end
      object Menu_CreateTDxLine: TMenuItem
        Tag = 14
        Caption = 'TDxLine'
        OnClick = OnCreateDxControlClick
      end
      object Menu_CreateTDxImageFormShape: TMenuItem
        Tag = 15
        Caption = 'TDxImageFormShape'
        OnClick = OnCreateDxControlClick
      end
    end
    object N2: TMenuItem
      Caption = '-'
    end
    object Menu_Copy: TMenuItem
      Caption = #22797#21046
      OnClick = Menu_CopyClick
    end
    object Menu_Paste: TMenuItem
      Caption = #31896#36148
      OnClick = Menu_PasteClick
    end
    object Menu_Delete: TMenuItem
      Caption = #21024#38500
      OnClick = Menu_DeleteClick
    end
    object N4: TMenuItem
      Caption = '-'
    end
    object Menu_Save: TMenuItem
      Caption = #20445#23384
      OnClick = Menu_SaveClick
    end
    object Menu_SaveToFile: TMenuItem
      Caption = #21478#23384#20026
      OnClick = Menu_SaveToFileClick
    end
    object N5: TMenuItem
      Caption = '-'
    end
    object Menu_Exit: TMenuItem
      Caption = #36864#20986
    end
  end
  object TimerClose: TTimer
    Enabled = False
    OnTimer = TimerCloseTimer
    Left = 144
    Top = 48
  end
  object OpenDialog: TOpenDialog
    Left = 112
    Top = 32
  end
  object SaveDialog: TSaveDialog
    Left = 248
    Top = 48
  end
  object TimerStart: TTimer
    Enabled = False
    Interval = 500
    OnTimer = TimerStartTimer
    Left = 184
    Top = 48
  end
  object PopupMenu1: TPopupMenu
    OnPopup = PopupMenuMainPopup
    Left = 80
    Top = 136
    object MenuItem1_MainPage: TMenuItem
      Caption = #36820#22238
      OnClick = MenuItem1_MainPageClick
    end
    object MenuItem2: TMenuItem
      Caption = '-'
    end
    object MenuItem1_CreateTDxTabSheet: TMenuItem
      Tag = 11
      Caption = #22686#21152#26032#39029
      OnClick = OnCreateDxControlClick
    end
  end
  object PopupMenu2: TPopupMenu
    OnPopup = PopupMenuMainPopup
    Left = 128
    Top = 136
    object MenuItem2_MainPage: TMenuItem
      Caption = #36820#22238
      OnClick = MenuItem1_MainPageClick
    end
    object MenuItem3: TMenuItem
      Caption = '-'
    end
    object MenuItem2_DeleteTDxTabSheet: TMenuItem
      Caption = #21024#38500#24403#21069#39029
      OnClick = Menu_DeleteClick
    end
  end
  object Timer: TTimer
    Enabled = False
    Interval = 1
    Left = 312
    Top = 72
  end
end
