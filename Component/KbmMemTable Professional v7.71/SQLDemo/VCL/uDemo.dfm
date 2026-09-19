object Form1: TForm1
  Left = 389
  Top = 220
  Caption = 
    'kbmSQL demo - Copyright 2007-2012 Components4Developers - All ri' +
    'ghts reserved'
  ClientHeight = 455
  ClientWidth = 992
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'MS Sans Serif'
  Font.Style = []
  OldCreateOrder = False
  OnCreate = FormCreate
  OnDestroy = FormDestroy
  PixelsPerInch = 96
  TextHeight = 13
  object PageControl1: TPageControl
    Left = 0
    Top = 0
    Width = 992
    Height = 455
    ActivePage = tsSQLSamples
    Align = alClient
    TabOrder = 0
    object tsDescription: TTabSheet
      Caption = 'Description'
      object Memo1: TMemo
        Left = 0
        Top = 0
        Width = 984
        Height = 427
        Align = alClient
        BevelInner = bvNone
        BevelOuter = bvNone
        BorderStyle = bsNone
        Color = clBtnFace
        Lines.Strings = (
          'SQL and Expression evaluation demo'
          ''
          
            'The SQL Samples page is a sample of how various SQL statements t' +
            'hat are executed on a number of predefined tables.'
          'When the Execute button is pressed, the following happens:'
          ''
          
            '- Some sample data is generated in the kbmMemTable mtTable1, mtT' +
            'able2 and mtTable3. '
          
            '  The data in mtTable1 consists of about 100 records of each 4 f' +
            'ields.'
          
            '  fld1 and fld4 are string fields, fld2 and fld3 are integer fie' +
            'lds.  (other '
          'kbmMemTable fieldtypes are also supported)'
          ''
          
            '- Executes the given SQL which can be one of:  SELECT, INSERT, U' +
            'PDATE and DELETE'
          ''
          'The component executing the SQL is named TkbmMemSQL.'
          
            'It supports registering multiple kbmMemTable with it which each ' +
            'can be aliased (as this demo shows... the component mtTable1 is ' +
            'aliased as table1).'
          ''
          'Whats supported:'
          
            '- Complex calculations, MOD, DIV, +, -, *, /, (, ), AS aliasing,' +
            ' LIKE, '
          'BETWEEN, IN, <,>,<=,>=,<>, NOT, SELECT,'
          'DELETE, UPDATE, INSERT, IS NULL, IS NOT NULL, ORDER '
          'BY, DESC, GROUP BY, MAX, MIN, SUM, AVG, COUNT'
          ''
          'Whats not (yet) supported:'
          '- HAVING, sub selects, other DDL commands, joins.'
          ''
          
            'The Evaluation samples page shows how TkbmMemSQL can be used to ' +
            'evaluate expressions that do not reference tables/fields.'
          'Two methods are shown:  Evaluate and Calculate'
          
            'Calculate only supports calculations. Boolean expressions (condi' +
            'tionals) are explicetly disallowed.'
          
            'Evaluate supports all expressions including conditionals and can' +
            ' thus return true/false in addition to a calculate value.')
        TabOrder = 0
      end
    end
    object tsSQLSamples: TTabSheet
      Caption = 'SQL Samples'
      ImageIndex = 1
      object Splitter1: TSplitter
        Left = 536
        Top = 0
        Height = 427
        ExplicitHeight = 432
      end
      object Panel1: TPanel
        Left = 0
        Top = 0
        Width = 536
        Height = 427
        Align = alLeft
        TabOrder = 0
        DesignSize = (
          536
          427)
        object Button1: TButton
          Left = 10
          Top = 395
          Width = 103
          Height = 25
          Anchors = [akLeft, akBottom]
          Caption = 'Execute top SQL'
          TabOrder = 0
          OnClick = Button1Click
        end
        object mSQL: TMemo
          Left = 3
          Top = 3
          Width = 527
          Height = 301
          Anchors = [akLeft, akTop, akRight, akBottom]
          HideSelection = False
          Lines.Strings = (
            
              'SELECT RecNo,RowID,fld1,fld2 FROM Table1 WHERE fld2 in (10,20,30' +
              ')'
            'SELECT Chr(876) FROM Table1 WHERE fld2 in (10,20,30)'
            'SELECT fld1 FROM Table1 WHERE fld2 in (10,20,30)'
            'SELECT 1-2-3 FROM Table1 LIMIT 1'
            'SELECT fld3,fld3||$Var1 FROM Table1'
            
              'SELECT LeftPad(fld3,'#39'A'#39',10),RightPad(fld3,'#39'B'#39',12),fld3||'#39'ABC'#39' FR' +
              'OM Table1'
            'SELECT fld2+1 as fld2a FROM Table1 ORDER BY fld2a DESC'
            'SELECT fld2+1 as fld2 FROM Table1'
            
              'SELECT fld1,fld2,fld3,fld3 AS SomeField1,fld4 AS SomeField2,fld5' +
              ' FROM table1 WHERE fld5 IN (5) ORDER BY fld2,SomeField2'
            
              'SELECT fld2 as Field2, fld3, sum(fld5) as fld5, Sum(fld2) as Som' +
              'eField1, Sum(fld3) as SomeField2 FROM table1 GROUP BY Field2, fl' +
              'd3'
            
              'SELECT fld2 as Field2, fld3, sum(fld5) as SomeField1, Sum(fld2) ' +
              'as SomeField2, Sum(fld3) as SomeField3 FROM table1 GROUP BY Fiel' +
              'd2, fld3'
            
              'SELECT fld5,sum(fld5) as sumoffld5,count(fld5) as countoffld5 FR' +
              'OM table1 GROUP BY fld5 HAVING count(fld5)>2'
            'SELECT fld2 as somefield, fld3 FROM table1'
            
              'SELECT fld5 as somefield,sum(fld5),count(fld5) FROM table1 GROUP' +
              ' BY somefield HAVING count(fld5)>2'
            'SELECT count(*)+5 FROM table1'
            'SELECT table1.* FROM table1 LIMIT 10 OFFSET 50'
            'SELECT table1.* FROM table1 LIMIT 10 '
            'SELECT table1.* FROM table1 OFFSET 50'
            'INSERT INTO table1 (fld1) VALUES ('#39'Test'#39')'
            'UPDATE table1 SET fld5 = (fld6) + (-fld2) WHERE fld3>10'
            'SELECT fld1,fld2,TRUE AS Visible, 2 AS RecordType FROM table1'
            'SELECT fld2, IF(fld2>10,True,False) AS IsSomething FROM table1'
            
              'SELECT SUM(fld5),SUM(fld6),SUM(fld5)+Sum(fld6) AS TotalField FRO' +
              'M table1'
            'SELECT SUM(fld5)+Sum(fld6) AS TotalField FROM table1'
            'SELECT SUM(fld5+fld6) AS TotalField FROM table1'
            'SELECT table1.* FROM table1'
            
              'SELECT fld5 as somefield,sum(fld5),count(fld5) from table1 group' +
              ' by somefield'
            
              'SELECT tb1.fld1,tb1.fld2,tb2.fld1,tb2.fld6 FROM table1 tb1, tabl' +
              'e2,tb2 WHERE tb1.fld2=tb2.fld6'
            
              'SELECT tb1.fld1,tb1.fld5,SUM(tb1.fld5),COUNT(tb1.fld5) FROM tabl' +
              'e1 tb1 GROUP BY tb1.fld1,tb1.fld5'
            
              'SELECT fld1,fld5,SUM(fld5),COUNT(fld5) FROM table1 tb1 GROUP BY ' +
              'fld1,fld5'
            'UPDATE table1 SET fld1='#39'UPD'#39
            'SELECT count(distinct Left(fld1,4)) from table1'
            
              'SELECT table1.fld1,table1.fld3,table1.fld4 from table1 where tab' +
              'le1.fld1="STR1" and table1.fld3=996'
            
              'SELECT fld1,fld3,fld4 from table1 where fld1="STR1" and fld4<>"S' +
              'TR996" and fld3=996'
            'SELECT length(fld1) from table1'
            
              'SELECT fld1,fld5,sum(fld5),count(fld5) from table1 group by fld1' +
              ',fld5'
            
              'SELECT min(fld2),max(fld2),sum(fld5),avg(fld5),count(*) from tab' +
              'le1 where fld5>5'
            
              'SELECT min(fld2),max(fld2),sum(fld5),avg(fld5),count(*) from tab' +
              'le1'
            
              'SELECT fld5,sum(if(fld5>5,1,0)),count(fld5) from table1 group by' +
              ' fld5'
            'SELECT fld5,sum(fld5),count(fld5) from table1 group by fld5'
            'SELECT count(*) from table1'
            'SELECT count(*) from table1 where fld2>50'
            'SELECT fld5 from table1 group by fld5'
            'SELECT fld3, fld3 as Field3 from table1'
            
              'SELECT fld3,(fld3 mod 10)=0 as bool,fld3 / 13 as somefield from ' +
              'table1 order by bool desc, somefield desc'
            'SELECT fld3,(fld3>950)=0,fld3 / 13 from table1'
            
              'SELECT fld3,(fld3>950)=0,fld3 / 13, (fld3 div 11) from table1 wh' +
              'ere (fld3 mod 11)=0'
            'SELECT fld3>5 from table1'
            'SELECT fld3+1+2 from table1'
            'SELECT fld1,fld3+1+2 from table1'
            'SELECT fld1,fld3 from table1'
            
              'UPDATE table1 SET fld1='#39'UPD'#39' WHERE fld1 in ('#39'STR2'#39','#39'STR4'#39','#39'STR6'#39 +
              ') or fld2=10'
            
              'DELETE FROM table1 WHERE fld1 in ('#39'STR2'#39','#39'STR4'#39','#39'STR6'#39') or fld2=' +
              '10'
            'DELETE FROM table1 WHERE fld1 in ('#39'STR2'#39','#39'STR4'#39','#39'STR6'#39')'
            'DELETE FROM table1 WHERE fld1='#39'STR2'#39
            
              'INSERT INTO table1 (fld1,fld2,fld4) VALUES ('#39'HEJ'#39',1,((2+5)-2)*3-' +
              '1 <>7)'
            'INSERT INTO table1 (fld1,fld2,fld4) VALUES ('#39'HEJ'#39',1,2*3)'
            'SELECT fld2+1 / 2,fld2 FROM table1 ORDER BY fld2'
            'SELECT * FROM table1'
            'SELECT * FROM table1 WHERE fld7 is null'
            'SELECT * FROM table1 WHERE fld7 is not null')
          TabOrder = 1
          WordWrap = False
          OnClick = mSQLClick
        end
        object Memo2: TMemo
          Left = 3
          Top = 310
          Width = 527
          Height = 79
          Anchors = [akLeft, akRight, akBottom]
          Color = clBtnFace
          ReadOnly = True
          TabOrder = 2
        end
        object Button2: TButton
          Left = 290
          Top = 395
          Width = 103
          Height = 25
          Anchors = [akLeft, akBottom]
          Caption = 'Execute all SQL'
          TabOrder = 3
          OnClick = Button2Click
        end
        object Button3: TButton
          Left = 130
          Top = 395
          Width = 130
          Height = 25
          Anchors = [akLeft, akBottom]
          Caption = 'Execute selected SQL'
          TabOrder = 4
          OnClick = Button3Click
        end
      end
      object PageControl2: TPageControl
        Left = 539
        Top = 0
        Width = 445
        Height = 427
        ActivePage = tsData
        Align = alClient
        TabOrder = 1
        object tsData: TTabSheet
          Caption = 'Data/Result'
          object Label1: TLabel
            Left = 0
            Top = 0
            Width = 437
            Height = 13
            Align = alTop
            Caption = 'Raw data'
            ExplicitWidth = 46
          end
          object Splitter2: TSplitter
            Left = 0
            Top = 206
            Width = 437
            Height = 3
            Cursor = crVSplit
            Align = alTop
            ExplicitLeft = -7
            ExplicitTop = 211
          end
          object Panel2: TPanel
            Left = 0
            Top = 209
            Width = 437
            Height = 190
            Align = alClient
            TabOrder = 0
            object Label2: TLabel
              Left = 1
              Top = 1
              Width = 435
              Height = 13
              Align = alTop
              Caption = 'SQL result'
              ExplicitWidth = 49
            end
            object DBGrid2: TDBGrid
              Left = 1
              Top = 14
              Width = 435
              Height = 175
              Align = alClient
              DataSource = dsResult
              TabOrder = 0
              TitleFont.Charset = DEFAULT_CHARSET
              TitleFont.Color = clWindowText
              TitleFont.Height = -11
              TitleFont.Name = 'MS Sans Serif'
              TitleFont.Style = []
            end
          end
          object PageControl3: TPageControl
            Left = 0
            Top = 13
            Width = 437
            Height = 193
            ActivePage = tsDebugDataset
            Align = alTop
            TabOrder = 1
            object tsTable1: TTabSheet
              Caption = 'Table1'
              object dbgTable1: TDBGrid
                Left = 0
                Top = 0
                Width = 429
                Height = 165
                Align = alClient
                DataSource = dsTable1
                TabOrder = 0
                TitleFont.Charset = DEFAULT_CHARSET
                TitleFont.Color = clWindowText
                TitleFont.Height = -11
                TitleFont.Name = 'MS Sans Serif'
                TitleFont.Style = []
              end
            end
            object tsTable2: TTabSheet
              Caption = 'Table2'
              ImageIndex = 1
              object dbgTable2: TDBGrid
                Left = 0
                Top = 0
                Width = 429
                Height = 165
                Align = alClient
                DataSource = dsTable2
                TabOrder = 0
                TitleFont.Charset = DEFAULT_CHARSET
                TitleFont.Color = clWindowText
                TitleFont.Height = -11
                TitleFont.Name = 'MS Sans Serif'
                TitleFont.Style = []
              end
            end
            object tsTable3: TTabSheet
              Caption = 'Table3'
              ImageIndex = 2
              object dbgTable3: TDBGrid
                Left = 0
                Top = 0
                Width = 429
                Height = 165
                Align = alClient
                DataSource = dsTable3
                TabOrder = 0
                TitleFont.Charset = DEFAULT_CHARSET
                TitleFont.Color = clWindowText
                TitleFont.Height = -11
                TitleFont.Name = 'MS Sans Serif'
                TitleFont.Style = []
              end
            end
            object tsDebugDataset: TTabSheet
              Caption = 'Debug'
              ImageIndex = 3
              object DBGrid1: TDBGrid
                Left = 0
                Top = 0
                Width = 429
                Height = 165
                Align = alClient
                DataSource = dsIntermediate
                TabOrder = 0
                TitleFont.Charset = DEFAULT_CHARSET
                TitleFont.Color = clWindowText
                TitleFont.Height = -11
                TitleFont.Name = 'MS Sans Serif'
                TitleFont.Style = []
              end
            end
          end
        end
        object tsParseTree: TTabSheet
          Caption = 'Parse tree'
          ImageIndex = 1
          DesignSize = (
            437
            399)
          object tvParse: TTreeView
            Left = 2
            Top = 0
            Width = 432
            Height = 361
            Anchors = [akLeft, akTop, akRight, akBottom]
            Indent = 19
            TabOrder = 0
          end
          object btnRefreshParseTree: TButton
            Left = 3
            Top = 367
            Width = 75
            Height = 25
            Anchors = [akLeft, akBottom]
            Caption = 'Refresh'
            TabOrder = 1
            OnClick = btnRefreshParseTreeClick
          end
        end
        object tsLog: TTabSheet
          Caption = 'Log'
          ImageIndex = 2
          object mLog: TMemo
            Left = 0
            Top = 0
            Width = 437
            Height = 399
            Align = alClient
            TabOrder = 0
          end
        end
      end
    end
    object tsEvaluationSamples: TTabSheet
      Caption = 'Evaluation samples'
      ImageIndex = 2
      DesignSize = (
        984
        427)
      object Label3: TLabel
        Left = 544
        Top = 264
        Width = 30
        Height = 13
        Caption = 'Result'
      end
      object Label4: TLabel
        Left = 544
        Top = 37
        Width = 410
        Height = 13
        Caption = 
          'Evaluation support conditional expressions in addition to regula' +
          'r calculative expressions'
      end
      object Label5: TLabel
        Left = 544
        Top = 56
        Width = 264
        Height = 13
        Caption = 'Calculation only supports regular calculative expressions'
      end
      object Label6: TLabel
        Left = 544
        Top = 3
        Width = 393
        Height = 28
        AutoSize = False
        Caption = 
          'Demo showing Evaluate and Calculate methods for evaluating or ca' +
          'lculating simple expressions, including variables.'
        WordWrap = True
      end
      object Label7: TLabel
        Left = 112
        Top = 333
        Width = 174
        Height = 13
        Caption = 'Evaluate statement at cursor position'
      end
      object Label8: TLabel
        Left = 112
        Top = 381
        Width = 176
        Height = 13
        Caption = 'Calculate statement at cursor position'
      end
      object Label9: TLabel
        Left = 544
        Top = 83
        Width = 154
        Height = 13
        Caption = 'Two variables has been defined:'
      end
      object Label10: TLabel
        Left = 552
        Top = 104
        Width = 202
        Height = 13
        Caption = 'var1 which contains the string '#39'The red fox'#39
      end
      object Label11: TLabel
        Left = 552
        Top = 123
        Width = 253
        Height = 13
        Caption = 'var2 which contains the floating point value 1234.567'
      end
      object Label12: TLabel
        Left = 544
        Top = 142
        Width = 410
        Height = 43
        AutoSize = False
        Caption = 
          'Variables are also available in SQL where they must be prefixed ' +
          'by $. In Calculate and Evaluate expressions, its legal to refer ' +
          'to variables directly by name without prefixing with $.'
        WordWrap = True
      end
      object Label13: TLabel
        Left = 544
        Top = 185
        Width = 410
        Height = 27
        AutoSize = False
        Caption = 
          'The metadata like display width and data type is obtained upon e' +
          'xpression compilation via the OnGetVariableMetaData event.'
        WordWrap = True
      end
      object Label14: TLabel
        Left = 544
        Top = 215
        Width = 410
        Height = 27
        AutoSize = False
        Caption = 
          'The value of a variable is obtained each time its needed during ' +
          'execution of the expression, via the OnGetVariableValue event.'
        WordWrap = True
      end
      object mEvaluation: TMemo
        Left = 3
        Top = 3
        Width = 527
        Height = 301
        Anchors = [akLeft, akTop, akRight, akBottom]
        HideSelection = False
        Lines.Strings = (
          '1-2-3'
          '3+2/2'
          '(3+2)/2'
          #39'A'#39'='#39'B'#39
          #39'A'#39'='#39'A'#39
          '123+3>124'
          '10+var2'
          '10+$var2'
          '1!23+3>124')
        TabOrder = 0
        WordWrap = False
        OnClick = mSQLClick
      end
      object btnEvaluate: TButton
        Left = 16
        Top = 328
        Width = 75
        Height = 25
        Caption = 'Evaluate'
        TabOrder = 1
        OnClick = btnEvaluateClick
      end
      object btnCalculate: TButton
        Left = 16
        Top = 376
        Width = 75
        Height = 25
        Caption = 'Calculate'
        TabOrder = 2
        OnClick = btnCalculateClick
      end
      object eEvalResult: TEdit
        Left = 544
        Top = 283
        Width = 425
        Height = 21
        ReadOnly = True
        TabOrder = 3
      end
    end
  end
  object mtTable1: TkbmMemTable
    DesignActivation = True
    AttachedAutoRefresh = True
    AttachMaxCount = 1
    FieldDefs = <>
    IndexDefs = <>
    SortOptions = []
    PersistentBackup = False
    ProgressFlags = [mtpcLoad, mtpcSave, mtpcCopy]
    LoadedCompletely = False
    SavedCompletely = False
    FilterOptions = []
    Version = '7.26.00 Standard Edition'
    LanguageID = 0
    SortID = 0
    SubLanguageID = 1
    LocaleID = 1024
    Left = 247
    Top = 16
    object mtTable1fld1: TStringField
      FieldName = 'fld1'
      Size = 10
    end
    object mtTable1fld2: TIntegerField
      FieldName = 'fld2'
    end
    object mtTable1fld3: TIntegerField
      FieldName = 'fld3'
    end
    object mtTable1fld4: TStringField
      FieldName = 'fld4'
      Size = 10
    end
    object mtTable1fld5: TIntegerField
      FieldName = 'fld5'
    end
    object mtTable1fld6: TIntegerField
      FieldName = 'fld6'
    end
    object mtTable1fld7: TIntegerField
      FieldName = 'fld7'
    end
  end
  object dsTable1: TDataSource
    DataSet = mtTable1
    Left = 248
    Top = 72
  end
  object dsResult: TDataSource
    Left = 496
    Top = 352
  end
  object mtTable2: TkbmMemTable
    DesignActivation = True
    AttachedAutoRefresh = True
    AttachMaxCount = 1
    FieldDefs = <>
    IndexDefs = <>
    SortOptions = []
    PersistentBackup = False
    ProgressFlags = [mtpcLoad, mtpcSave, mtpcCopy]
    LoadedCompletely = False
    SavedCompletely = False
    FilterOptions = []
    Version = '7.26.00 Standard Edition'
    LanguageID = 0
    SortID = 0
    SubLanguageID = 1
    LocaleID = 1024
    Left = 303
    Top = 16
    object mtTable2fld1: TStringField
      FieldName = 'fld1'
      Size = 10
    end
    object mtTable2fld2: TIntegerField
      FieldName = 'fld2'
    end
    object mtTable2fld3: TIntegerField
      FieldName = 'fld3'
    end
    object mtTable2fld4: TStringField
      FieldName = 'fld4'
      Size = 10
    end
    object mtTable2fld5: TIntegerField
      FieldName = 'fld5'
    end
    object mtTable2fld6: TIntegerField
      FieldName = 'fld6'
    end
    object mtTable2fld7: TIntegerField
      FieldName = 'fld7'
    end
  end
  object dsTable2: TDataSource
    DataSet = mtTable2
    Left = 304
    Top = 72
  end
  object mtTable3: TkbmMemTable
    DesignActivation = True
    AttachedAutoRefresh = True
    AttachMaxCount = 1
    FieldDefs = <>
    IndexDefs = <>
    SortOptions = []
    PersistentBackup = False
    ProgressFlags = [mtpcLoad, mtpcSave, mtpcCopy]
    LoadedCompletely = False
    SavedCompletely = False
    FilterOptions = []
    Version = '7.26.00 Standard Edition'
    LanguageID = 0
    SortID = 0
    SubLanguageID = 1
    LocaleID = 1024
    Left = 367
    Top = 16
    object mtTable3fld1: TStringField
      FieldName = 'fld1'
      Size = 10
    end
    object mtTable3fld2: TIntegerField
      FieldName = 'fld2'
    end
    object mtTable3fld3: TIntegerField
      FieldName = 'fld3'
    end
    object mtTable3fld4: TStringField
      FieldName = 'fld4'
      Size = 10
    end
    object mtTable3fld5: TIntegerField
      FieldName = 'fld5'
    end
    object mtTable3fld6: TIntegerField
      FieldName = 'fld6'
    end
    object mtTable3fld7: TIntegerField
      FieldName = 'fld7'
    end
  end
  object dsTable3: TDataSource
    DataSet = mtTable3
    Left = 368
    Top = 72
  end
  object dsIntermediate: TDataSource
    Left = 560
    Top = 352
  end
end
