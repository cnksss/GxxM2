object StructureDlg: TStructureDlg
  Left = 1142
  Top = 211
  Width = 202
  Height = 257
  BorderStyle = bsSizeToolWin
  Caption = 'StructureDlg'
  Color = clBtnFace
  Font.Charset = GB2312_CHARSET
  Font.Color = clWindowText
  Font.Height = -12
  Font.Name = #23435#20307
  Font.Style = []
  OldCreateOrder = False
  OnCreate = FormCreate
  PixelsPerInch = 96
  TextHeight = 12
  object TreeView: TRzTreeView
    Left = 0
    Top = 0
    Width = 186
    Height = 219
    SelectionPen.Color = clBtnShadow
    Align = alClient
    AutoSelect = True
    Indent = 19
    MultiSelect = True
    PopupMenu = PopupMenu
    ReadOnly = True
    TabOrder = 0
    OnClick = TreeViewClick
    OnDblClick = TreeViewDblClick
    OnKeyDown = TreeViewKeyDown
  end
  object PopupMenu: TPopupMenu
    Left = 48
    Top = 40
    object Menu_Add: TMenuItem
      Caption = #26032#24314
      OnClick = Menu_AddClick
    end
    object Menu_Delete: TMenuItem
      Caption = #21024#38500
      Enabled = False
      OnClick = Menu_DeleteClick
    end
    object Menu_Out: TMenuItem
      Caption = #23548#20986
    end
    object Menu_IN: TMenuItem
      Caption = #23548#20837
    end
  end
end
