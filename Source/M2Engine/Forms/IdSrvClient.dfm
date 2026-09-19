object FrmIDSoc: TFrmIDSoc
  Left = 1253
  Top = 556
  Caption = 'FrmIDSoc'
  ClientHeight = 80
  ClientWidth = 159
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'MS Sans Serif'
  Font.Style = []
  OnCreate = FormCreate
  OnDestroy = FormDestroy
  TextHeight = 13
  object IDSocket: TClientSocket
    Active = False
    ClientType = ctNonBlocking
    Port = 0
    OnConnect = IDSocketConnect
    OnDisconnect = IDSocketDisconnect
    OnRead = IDSocketRead
    OnError = IDSocketError
    Left = 51
    Top = 26
  end
  object Timer1: TTimer
    Enabled = False
    Interval = 30000
    OnTimer = Timer1Timer
    Left = 83
    Top = 26
  end
end
