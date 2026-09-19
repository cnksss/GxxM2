object frmDemo: TfrmDemo
  Left = 273
  Top = 102
  BorderIcons = [biSystemMenu, biMinimize]
  BorderStyle = bsSingle
  Caption = 'TFastIniFile demo'
  ClientHeight = 602
  ClientWidth = 792
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'MS Sans Serif'
  Font.Style = []
  OldCreateOrder = False
  Position = poScreenCenter
  OnCreate = FormCreate
  OnShow = FormShow
  PixelsPerInch = 96
  TextHeight = 13
  object btnExit: TButton
    Left = 710
    Top = 568
    Width = 75
    Height = 25
    Cancel = True
    Caption = 'E&xit'
    Default = True
    TabOrder = 1
    OnClick = btnExitClick
  end
  object PageControl1: TPageControl
    Left = 8
    Top = 8
    Width = 693
    Height = 585
    ActivePage = TabSheet1
    TabOrder = 0
    object TabSheet1: TTabSheet
      Caption = 'Speed'
      ImageIndex = 1
      object lblResults: TLabel
        Left = 236
        Top = 8
        Width = 35
        Height = 13
        Caption = 'Results'
      end
      object btnAbort: TSpeedButton
        Left = 648
        Top = 3
        Width = 20
        Height = 20
        Glyph.Data = {
          76010000424D7601000000000000760000002800000020000000100000000100
          04000000000000010000130B0000130B00001000000000000000000000000000
          800000800000008080008000000080008000808000007F7F7F00BFBFBF000000
          FF0000FF000000FFFF00FF000000FF00FF00FFFF0000FFFFFF00333333333333
          3333333333FFFFF3333333333999993333333333F77777FFF333333999999999
          33333337777FF377FF3333993370739993333377FF373F377FF3399993000339
          993337777F777F3377F3393999707333993337F77737333337FF993399933333
          399377F3777FF333377F993339903333399377F33737FF33377F993333707333
          399377F333377FF3377F993333101933399377F333777FFF377F993333000993
          399377FF3377737FF7733993330009993933373FF3777377F7F3399933000399
          99333773FF777F777733339993707339933333773FF7FFF77333333999999999
          3333333777333777333333333999993333333333377777333333}
        NumGlyphs = 2
        Visible = False
        OnClick = btnAbortClick
      end
      object btnDelete: TBitBtn
        Left = 16
        Top = 512
        Width = 201
        Height = 33
        Caption = 'Delete test files !'
        Font.Charset = DEFAULT_CHARSET
        Font.Color = clBlue
        Font.Height = -11
        Font.Name = 'MS Sans Serif'
        Font.Style = []
        ParentFont = False
        TabOrder = 8
        OnClick = btnDeleteClick
      end
      object mmoResults: TMemo
        Left = 236
        Top = 24
        Width = 437
        Height = 521
        Lines.Strings = (
          'if "Sections"  *  "Idents/Values" > 4000 then'
          '  Standard TIniFile is not tested;'
          ''
          
            'When you change the numbers in the SpinEdits the test files need' +
            's to'
          
            'be deleted first by using the big button at the bottom of the Ta' +
            'bSheet.'
          ''
          
            'For large number Idents/Values you can use the button "Write all' +
            ' using TStringList". '
          'F.i. TMemInfile will take a minute to write ± 200K items.'
          '')
        TabOrder = 9
      end
      object btnWrite: TButton
        Left = 16
        Top = 84
        Width = 200
        Height = 25
        Caption = 'Write / Rewrite'
        TabOrder = 4
        OnClick = btnWriteClick
      end
      object btnRandomRead: TButton
        Left = 16
        Top = 112
        Width = 200
        Height = 25
        Caption = '"Random" read'
        Enabled = False
        TabOrder = 5
        OnClick = btnRandomReadClick
      end
      object btnRead: TButton
        Left = 16
        Top = 140
        Width = 200
        Height = 25
        Caption = 'Sequential read'
        Enabled = False
        TabOrder = 6
        OnClick = btnReadClick
      end
      object spnSections: TSpinEdit
        Left = 116
        Top = 25
        Width = 97
        Height = 22
        Increment = 100
        MaxValue = 20000
        MinValue = 10
        TabOrder = 1
        Value = 400
        OnChange = spnSectionsChange
      end
      object spnIdents: TSpinEdit
        Left = 116
        Top = 49
        Width = 97
        Height = 22
        Increment = 10
        MaxValue = 20000
        MinValue = 5
        TabOrder = 3
        Value = 10
        OnChange = spnSectionsChange
      end
      object btnReadEverything: TButton
        Left = 16
        Top = 168
        Width = 200
        Height = 25
        Caption = 'Read everything'
        Enabled = False
        TabOrder = 7
        OnClick = btnReadEverythingClick
      end
      object chkIdents: TCheckBox
        Left = 20
        Top = 52
        Width = 93
        Height = 17
        Caption = 'Idents/Values:'
        Checked = True
        State = cbChecked
        TabOrder = 2
        OnClick = chkIdentsClick
      end
      object chkSections: TCheckBox
        Left = 20
        Top = 28
        Width = 93
        Height = 17
        AllowGrayed = True
        Caption = 'Sections'
        Enabled = False
        State = cbGrayed
        TabOrder = 0
      end
      object pgbResults: TProgressBar
        Left = 402
        Top = 4
        Width = 243
        Height = 17
        Min = 0
        Max = 100
        Step = 1
        TabOrder = 10
        Visible = False
      end
      object btnEraseHalf: TButton
        Left = 16
        Top = 236
        Width = 200
        Height = 25
        Caption = 'Erase half of the sections + Rewrite'
        Enabled = False
        TabOrder = 11
        OnClick = btnEraseHalfClick
      end
      object btnCreateEraseRewrite: TButton
        Left = 16
        Top = 264
        Width = 200
        Height = 25
        Caption = 'Write, erase and rewrite sections  (long!)'
        TabOrder = 12
        OnClick = btnCreateEraseRewriteClick
      end
      object btnEraseLastSection: TButton
        Left = 16
        Top = 292
        Width = 200
        Height = 25
        Caption = 'Erase and rewrite last section'
        TabOrder = 13
        OnClick = btnEraseLastSectionClick
      end
      object btnCreateWriteFree: TButton
        Left = 16
        Top = 332
        Width = 200
        Height = 25
        Caption = 'Create Write/Rewrite Free'
        TabOrder = 14
        OnClick = btnCreateWriteFreeClick
      end
      object btnCreateReadFree: TButton
        Left = 16
        Top = 372
        Width = 200
        Height = 25
        Caption = 'Create Read Free'
        Enabled = False
        TabOrder = 15
        OnClick = btnCreateReadFreeClick
      end
      object btnWriteUsingTStringList: TButton
        Left = 16
        Top = 400
        Width = 200
        Height = 25
        Caption = 'Write all using TStringList'
        TabOrder = 16
        OnClick = btnWriteUsingTStringListClick
      end
      object btnWriteUsingSetStrings: TButton
        Left = 16
        Top = 428
        Width = 200
        Height = 25
        Caption = 'Write/Rewrite using SetStrings'
        TabOrder = 17
        OnClick = btnWriteUsingSetStringsClick
      end
      object btnStatistics: TButton
        Left = 16
        Top = 468
        Width = 200
        Height = 25
        Caption = 'Statistics'
        TabOrder = 18
        OnClick = btnStatisticsClick
      end
      object btnWriteOne: TButton
        Left = 16
        Top = 208
        Width = 200
        Height = 25
        Caption = 'Write/Change one setting.'
        Enabled = False
        TabOrder = 19
        OnClick = btnWriteOneClick
      end
    end
    object TabSheet2: TTabSheet
      Caption = 'Extras'
      object lblValue: TLabel
        Left = 168
        Top = 324
        Width = 27
        Height = 13
        Caption = 'Value'
      end
      object lblComment: TLabel
        Left = 304
        Top = 324
        Width = 44
        Height = 13
        Caption = 'Comment'
      end
      object lblDateTime: TLabel
        Left = 168
        Top = 404
        Width = 344
        Height = 13
        Caption = 
          'Returned value formatted by using local (long) Date/Time format ' +
          'settings.'
      end
      object lblQuotedValues: TLabel
        Left = 168
        Top = 364
        Width = 217
        Height = 13
        Caption = 'Concatenation of the the three quoted values.'
      end
      object rchDemo: TRichEdit
        Left = 12
        Top = 12
        Width = 661
        Height = 305
        Font.Charset = ANSI_CHARSET
        Font.Color = clWindowText
        Font.Height = -13
        Font.Name = 'Courier New'
        Font.Style = []
        HideScrollBars = False
        ParentFont = False
        ReadOnly = True
        ScrollBars = ssVertical
        TabOrder = 0
      end
      object btnReadValueAndComment: TButton
        Left = 12
        Top = 336
        Width = 144
        Height = 25
        Caption = 'Read Value and Comment'
        TabOrder = 1
        OnClick = btnReadValueAndCommentClick
      end
      object btnReadQuotedValues: TButton
        Left = 12
        Top = 376
        Width = 144
        Height = 25
        Caption = 'Read Quoted Values'
        TabOrder = 4
        OnClick = btnReadQuotedValuesClick
      end
      object btnReadWriteFixedDateTime: TButton
        Left = 12
        Top = 416
        Width = 144
        Height = 25
        Caption = 'Write/Read Fixed DateTime'
        TabOrder = 6
        OnClick = btnReadWriteFixedDateTimeClick
      end
      object btnWriteStrings: TButton
        Left = 12
        Top = 496
        Width = 144
        Height = 25
        Caption = 'Write TStrings'
        TabOrder = 9
        OnClick = btnWriteStringsClick
      end
      object lbxRecent1: TListBox
        Left = 168
        Top = 496
        Width = 161
        Height = 49
        Font.Charset = DEFAULT_CHARSET
        Font.Color = clWindowText
        Font.Height = -11
        Font.Name = 'MS Sans Serif'
        Font.Style = []
        ItemHeight = 13
        Items.Strings = (
          'C:\Demo\pSimpleDemo.dpr'
          'C:\Demo\pSimpleDemo.pas'
          'C:\Demo\demo.ini')
        ParentFont = False
        TabOrder = 10
      end
      object edtDateTime: TEdit
        Left = 168
        Top = 418
        Width = 501
        Height = 21
        TabOrder = 7
      end
      object edtQuotedValues: TEdit
        Left = 168
        Top = 378
        Width = 501
        Height = 21
        TabOrder = 5
      end
      object edtValue: TEdit
        Left = 168
        Top = 338
        Width = 121
        Height = 21
        TabOrder = 2
      end
      object edtComment: TEdit
        Left = 304
        Top = 338
        Width = 121
        Height = 21
        TabOrder = 3
      end
      object btnReadStrings: TButton
        Left = 356
        Top = 496
        Width = 144
        Height = 25
        Caption = 'Read TStrings'
        Enabled = False
        TabOrder = 11
        OnClick = btnReadStringsClick
      end
      object lbxRecent2: TListBox
        Left = 508
        Top = 496
        Width = 161
        Height = 49
        Font.Charset = DEFAULT_CHARSET
        Font.Color = clWindowText
        Font.Height = -11
        Font.Name = 'MS Sans Serif'
        Font.Style = []
        ItemHeight = 13
        ParentFont = False
        TabOrder = 12
      end
      object btnWriteFont: TButton
        Left = 12
        Top = 456
        Width = 144
        Height = 25
        Caption = 'Write font'
        TabOrder = 8
        OnClick = btnWriteFontClick
      end
      object btnReadFont: TButton
        Left = 168
        Top = 456
        Width = 144
        Height = 25
        Caption = 'Read font'
        Enabled = False
        TabOrder = 13
        OnClick = btnReadFontClick
      end
      object btnRestoreFont: TButton
        Left = 328
        Top = 456
        Width = 144
        Height = 25
        Caption = 'Restore font'
        Enabled = False
        TabOrder = 14
        OnClick = btnRestoreFontClick
      end
    end
    object TabSheet3: TTabSheet
      Caption = 'Compatibility'
      ImageIndex = 2
      object lblOriginal: TLabel
        Left = 8
        Top = 6
        Width = 35
        Height = 13
        Caption = 'Original'
      end
      object Label1: TLabel
        Left = 8
        Top = 214
        Width = 34
        Height = 13
        Caption = 'TIniFile'
      end
      object Label2: TLabel
        Left = 176
        Top = 214
        Width = 57
        Height = 13
        Caption = 'TMemIniFile'
      end
      object Label3: TLabel
        Left = 344
        Top = 214
        Width = 54
        Height = 13
        Caption = 'TFastIniFile'
      end
      object Label5: TLabel
        Left = 232
        Top = 290
        Width = 35
        Height = 13
        Caption = 'Results'
      end
      object rchComp: TRichEdit
        Left = 8
        Top = 20
        Width = 665
        Height = 189
        Font.Charset = ANSI_CHARSET
        Font.Color = clWindowText
        Font.Height = -13
        Font.Name = 'Courier New'
        Font.Style = []
        HideScrollBars = False
        ParentFont = False
        ReadOnly = True
        ScrollBars = ssVertical
        TabOrder = 0
      end
      object rchComp1: TRichEdit
        Left = 8
        Top = 228
        Width = 162
        Height = 55
        Font.Charset = ANSI_CHARSET
        Font.Color = clWindowText
        Font.Height = -13
        Font.Name = 'Courier New'
        Font.Style = []
        ParentFont = False
        PlainText = True
        TabOrder = 1
      end
      object rchComp3: TRichEdit
        Left = 344
        Top = 228
        Width = 162
        Height = 55
        Font.Charset = ANSI_CHARSET
        Font.Color = clWindowText
        Font.Height = -13
        Font.Name = 'Courier New'
        Font.Style = []
        ParentFont = False
        PlainText = True
        TabOrder = 3
      end
      object rchComp2: TRichEdit
        Left = 176
        Top = 228
        Width = 162
        Height = 55
        Font.Charset = ANSI_CHARSET
        Font.Color = clWindowText
        Font.Height = -13
        Font.Name = 'Courier New'
        Font.Style = []
        ParentFont = False
        PlainText = True
        TabOrder = 2
      end
      object btnCheckEmptyReadDefault: TButton
        Left = 16
        Top = 304
        Width = 201
        Height = 25
        Caption = 'Read section with "empty" entries.'
        TabOrder = 4
        OnClick = btnCheckEmptyReadDefaultClick
      end
      object Memo1: TMemo
        Left = 232
        Top = 304
        Width = 444
        Height = 245
        ScrollBars = ssVertical
        TabOrder = 9
      end
      object btnTestIfAnsiCompareIsNeeded: TButton
        Left = 16
        Top = 432
        Width = 201
        Height = 25
        Caption = 'Test if ANSI compare is needed'
        TabOrder = 8
        OnClick = btnTestIfAnsiCompareIsNeededClick
      end
      object btnTestReadBackSpaces: TButton
        Left = 16
        Top = 404
        Width = 201
        Height = 25
        Caption = 'Test read back spaces'
        TabOrder = 7
        OnClick = btnTestReadBackSpacesClick
      end
      object btnReadSection: TButton
        Left = 16
        Top = 328
        Width = 201
        Height = 25
        Caption = '... ReadSections().'
        Enabled = False
        TabOrder = 5
        OnClick = btnReadSectionClick
      end
      object btnReadSectionValues: TButton
        Left = 16
        Top = 352
        Width = 201
        Height = 25
        Caption = '...ReadSectionValues().'
        Enabled = False
        TabOrder = 6
        OnClick = btnReadSectionValuesClick
      end
    end
  end
end
