object FrmMessageFilter: TFrmMessageFilter
  Left = 488
  Top = 246
  BorderIcons = [biSystemMenu, biMinimize]
  BorderStyle = bsDialog
  Caption = #28040#24687#25991#23383#36807#28388#35774#32622
  ClientHeight = 249
  ClientWidth = 427
  Color = clBtnFace
  Font.Charset = ANSI_CHARSET
  Font.Color = clWindowText
  Font.Height = -12
  Font.Name = #23435#20307
  Font.Style = []
  OldCreateOrder = False
  PixelsPerInch = 96
  TextHeight = 12
  object Label1: TLabel
    Left = 8
    Top = 8
    Width = 54
    Height = 12
    Caption = #36807#28388#25991#26412':'
  end
  object lstFilterText: TListBox
    Left = 8
    Top = 24
    Width = 153
    Height = 217
    ItemHeight = 12
    TabOrder = 0
    OnClick = lstFilterTextClick
    OnDblClick = lstFilterTextDblClick
  end
  object btnAdd: TButton
    Left = 168
    Top = 216
    Width = 59
    Height = 25
    Caption = #22686#21152'(&A)'
    TabOrder = 1
    OnClick = btnAddClick
  end
  object btnDel: TButton
    Left = 232
    Top = 216
    Width = 59
    Height = 25
    Caption = #21024#38500'(&D)'
    TabOrder = 2
    OnClick = btnDelClick
  end
  object btnOK: TButton
    Left = 360
    Top = 216
    Width = 59
    Height = 25
    Caption = #30830#23450'(&O)'
    TabOrder = 3
    OnClick = btnOKClick
  end
  object btnEdit: TButton
    Left = 296
    Top = 216
    Width = 59
    Height = 25
    Caption = #20462#25913'(&M)'
    TabOrder = 4
    OnClick = btnEditClick
  end
  object GroupBox2: TGroupBox
    Left = 168
    Top = 19
    Width = 251
    Height = 192
    Caption = #36807#28388#36873#39033
    TabOrder = 5
    object Label2: TLabel
      Left = 18
      Top = 168
      Width = 54
      Height = 12
      Caption = #35686#21578#20869#23481':'
    end
    object chkFilterSayMsg: TCheckBox
      Left = 8
      Top = 17
      Width = 97
      Height = 17
      Caption = #24320#21551#25991#23383#36807#28388
      ParentShowHint = False
      ShowHint = True
      TabOrder = 0
      OnClick = chkFilterSayMsgClick
    end
    object rbAllBlock: TRadioButton
      Left = 16
      Top = 37
      Width = 145
      Height = 17
      Caption = #25972#21477#20351#29992#35686#21578#25991#26412#26367#25442
      ParentShowHint = False
      ShowHint = True
      TabOrder = 1
      OnClick = rbAllBlockClick
    end
    object rbSelfBolck: TRadioButton
      Tag = 1
      Left = 16
      Top = 57
      Width = 145
      Height = 17
      Caption = #29305#24449#23383#29992#35686#21578#25991#26412#26367#25442
      ParentShowHint = False
      ShowHint = True
      TabOrder = 2
      OnClick = rbAllBlockClick
    end
    object rbConnClose: TRadioButton
      Tag = 2
      Left = 16
      Top = 78
      Width = 145
      Height = 17
      Hint = #36873#20013#35813#39033#65292#22914#26524#21457#29616#36807#28388#29305#24449#23383#65292#21017#20351#35813#29609#23478#25481#32447#22788#29702#12290
      Caption = #21457#29616#36807#28388#25991#23383#25481#32447#22788#29702
      ParentShowHint = False
      ShowHint = True
      TabOrder = 3
      OnClick = rbAllBlockClick
    end
    object rbDisMsg: TRadioButton
      Tag = 3
      Left = 16
      Top = 98
      Width = 145
      Height = 17
      Hint = #36873#20013#35813#39033#65292#22914#26524#21457#29616#36807#28388#29305#24449#23383#65292#21017#20002#24323#25972#21477#19981#22788#29702#12290
      Caption = #21457#29616#36807#28388#25991#23383#20002#21253#22788#29702
      ParentShowHint = False
      ShowHint = True
      TabOrder = 4
      OnClick = rbAllBlockClick
    end
    object rbDisMsgorSys: TRadioButton
      Tag = 4
      Left = 16
      Top = 118
      Width = 145
      Height = 17
      Hint = #36873#20013#35813#39033#65292#22914#26524#21457#29616#36807#28388#29305#24449#23383#65292#36820#22238#35686#21578#25991#26412#25552#31034#12290#24182#19988#20002#24323#21407#25991#23383#20869#23481#12290
      Caption = #21457#29616#36807#28388#25991#23383#35686#21578#22788#29702
      ParentShowHint = False
      ShowHint = True
      TabOrder = 5
      OnClick = rbAllBlockClick
    end
    object chkFilterSayTriggerScript: TCheckBox
      Left = 17
      Top = 141
      Width = 200
      Height = 17
      Caption = #35302#21457#33050#26412' [@RungateMsgFilter]'
      TabOrder = 6
      OnClick = chkFilterSayTriggerScriptClick
    end
    object edtWarnSayMsg: TEdit
      Left = 72
      Top = 164
      Width = 171
      Height = 20
      TabOrder = 7
      OnChange = edtWarnSayMsgChange
    end
  end
end
