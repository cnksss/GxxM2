object FrmRemoteQuerySetting: TFrmRemoteQuerySetting
  Left = 353
  Top = 260
  BorderStyle = bsDialog
  Caption = #36828#31243#26597#35810#35774#32622
  ClientHeight = 227
  ClientWidth = 403
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
  object lbl1: TLabel
    Left = 213
    Top = 87
    Width = 138
    Height = 12
    Caption = #31471#21475#20026'0'#20851#38381#36828#31243#31649#29702#21151#33021
    Font.Charset = GB2312_CHARSET
    Font.Color = clBlue
    Font.Height = -12
    Font.Name = #23435#20307
    Font.Style = []
    ParentFont = False
  end
  object lbl2: TLabel
    Left = 213
    Top = 106
    Width = 180
    Height = 12
    Caption = #26080#36830#25509'IP'#38480#21046#26102#65292#21487#20197#20219#24847'IP'#36830#25509
    Font.Charset = GB2312_CHARSET
    Font.Color = clBlue
    Font.Height = -12
    Font.Name = #23435#20307
    Font.Style = []
    ParentFont = False
  end
  object grp2: TGroupBox
    Left = 214
    Top = 6
    Width = 180
    Height = 77
    Caption = #36828#31243#26597#35810#35774#32622
    TabOrder = 0
    object lbl3: TLabel
      Left = 9
      Top = 21
      Width = 52
      Height = 13
      Caption = #26597#35810#31471#21475':'
    end
    object lbl4: TLabel
      Left = 9
      Top = 47
      Width = 52
      Height = 13
      Caption = #26597#35810#23494#30721':'
    end
    object edtPassword: TEdit
      Left = 65
      Top = 43
      Width = 105
      Height = 21
      ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
      MaxLength = 30
      TabOrder = 0
    end
    object sePort: TSpinEditEx
      Left = 64
      Top = 16
      Width = 105
      Height = 22
      MaxValue = 65535
      MinValue = 0
      TabOrder = 1
      Value = 0
    end
  end
  object grp1: TGroupBox
    Left = 6
    Top = 6
    Width = 195
    Height = 214
    Caption = #20801#35768#36830#25509'IP'
    TabOrder = 1
    object lstControlIPList: TListBox
      Left = 8
      Top = 16
      Width = 178
      Height = 191
      ItemHeight = 13
      TabOrder = 0
    end
  end
  object btnOK: TButton
    Left = 319
    Top = 195
    Width = 75
    Height = 25
    Caption = #30830#23450
    TabOrder = 2
    OnClick = btnOKClick
  end
  object pmControlIPList: TPopupMenu
    Left = 136
    Top = 152
    object mniIPAdd: TMenuItem
      Caption = #22686#21152'(&A)'
      OnClick = mniIPAddClick
    end
    object mniIPDelete: TMenuItem
      Caption = #21024#38500'(&D)'
      OnClick = mniIPDeleteClick
    end
    object mniIPClear: TMenuItem
      Caption = #28165#31354'(&C)'
      OnClick = mniIPClearClick
    end
  end
end
