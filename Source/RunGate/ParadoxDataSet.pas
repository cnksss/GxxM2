{***************************************************************}
{                                                               }
{         Parodox Component for Lazarus Version 0.7             }
{                                                               }
{ Copyright (c) 2007-2009 Kudriavtsev Pavel (paulkudr@mail.ru)  }
{ Copyright (c) 2007-2008 Gerry Kleinpenning                    }
{ Copyright (c) 2009      Sergey (User32!!!)                    }
{                                                               }
{                                                               }
{     DataSet компонент для чтения файлов paradox.              }
{                                                               }
{     - Файлы открываются только для чтения;                    }
{     - Поддерживаются Blob-поля (не кэшируются);               }
{     - Перекодировка из cp1251 и cp866 в UTF8, cp1251, koi8r;  }
{     - Не поддерживаются индексы.                              }
{                                                               }
{     Roadmap                                                   }
{                                                               }
{     - Поддержка индексов;                                     }
{                                                               }
{  Добавление возможности записи пока не планируется.           }
{                                                               }
{***************************************************************}
{
 ***************************************************************************
 *                                                                         *
 *   This source is free software; you can redistribute it and/or modify   *
 *   it under the terms of the GNU General Public License as published by  *
 *   the Free Software Foundation; either version 2 of the License, or     *
 *   (at your option) any later version.                                   *
 *                                                                         *
 *   This code is distributed in the hope that it will be useful, but      *
 *   WITHOUT ANY WARRANTY; without even the implied warranty of            *
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU     *
 *   General Public License for more details.                              *
 *                                                                         *
 *   A copy of the GNU General Public License is available on the World    *
 *   Wide Web at <http://www.gnu.org/copyleft/gpl.html>. You can also      *
 *   obtain it by writing to the Free Software Foundation,                 *
 *   Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA.        *
 *                                                                         *
 ***************************************************************************
}
unit ParadoxDataSet;

{$IFDEF FPC}
  {$mode DELPHI}
{$ENDIF}

{$H+}

interface

uses
  DB, Classes, SysUtils, Forms, ParadoxConv;

{$IFDEF VER310}           // Delphi XE10.1
  {$DEFINE XE}
{$ENDIF}

{$IFDEF VER300}           // Delphi XE10
  {$DEFINE XE}
{$ENDIF}

{$IFDEF VER290}           // Delphi XE8
  {$DEFINE XE}
{$ENDIF}

{$IFDEF VER280}           // Delphi XE7
  {$DEFINE XE}
{$ENDIF}

{$IFDEF VER270}           // Delphi XE6
  {$DEFINE XE}
{$ENDIF}

{$IFDEF VER260}           // Delphi XE5
  {$DEFINE XE}
{$ENDIF}

{$IFDEF VER250}           // Delphi XE4
  {$DEFINE XE}
{$ENDIF}

{$IFDEF VER240}           // Delphi XE3
  {$DEFINE XE}
{$ENDIF}

{$IFDEF VER230}           // Delphi XE2
  {$DEFINE XE}
{$ENDIF}

{$IFDEF VER220}           // Delphi XE
  {$DEFINE XE}
{$ENDIF}

const
  { Paradox codes for field types }
  pxfAlpha        = $01;
  pxfDate         = $02;
  pxfShort        = $03;
  pxfLong         = $04;
  pxfCurrency     = $05;
  pxfNumber       = $06;
  pxfLogical      = $09;
  pxfMemoBLOB     = $0C;
  pxfBLOB         = $0D;
  pxfFmtMemoBLOB  = $0E;
  pxfOLE          = $0F;
  pxfGraphic      = $10;
  pxfTime         = $14;
  pxfTimestamp    = $15;
  pxfAutoInc      = $16;
  pxfBCD          = $17;
  pxfBytes        = $18;

type
{$IFNDEF XE}
  TRecordBuffer = PAnsiChar;
  TValueBuffer = Pointer;
{$ENDIF}

  { Information about field }

  PFieldInfoRecord = ^TFieldInfoRecord;
  TFieldInfoRecord = packed record
    FieldType: Byte;
    FieldSize: Byte;
  end;

  PDataBlock = ^TDataBlock;
  TDataBlock = packed record
    NextBlock: Word;
    BlockNumber: Word;
    AddDataSize: Word;
  end;


  {-----------------------------------------------------------------------------------
    offset  type        usage
    ==============================================================================
    | 0000 | integer     recordSize                                              |
    |      |                                                                     |
    |      |        This is the size of a user record in this table.             |
    |      |                                                                     |
    |      |        For primary index files, each "record" is actually the       |
    |      |        field or fields in the index, plus three integers which      |
    |      |        are not referenced in the header.                            |
    |      |                                                                     |
    |      |        Secondary index files also have additional fields, but       |
    |      |        these are listed in the header.                              |
    |      |                                                                     |
    +------+---------------------------------------------------------------------+
    | 0002 | integer     headerSize (always $0800)                               |
    |      |                                                                     |
    |      |        You can change headerSize, and move the data blocks          |
    |      |        accordingly, to create larger or smaller table headers.      |
    |      |        Borland'S TUTILITY program would flag an error, but          |
    |      |        Paradox, the Borland Database Engine and the Paradox         |
    |      |        Engine will all still work with these tables.                |
    |      |                                                                     |
    +------+---------------------------------------------------------------------+
    | 0004 | byte        fileType                                                |
    |      |                                                                     |
    |      |           0 = this is an indexed .DB data file                      |
    |      |           1 = this is a primary index .PX file                      |
    |      |           2 = this is a non-indexed .DB data file                   |
    |      |           3 = this is a non-incrementing secondary index .Xnn file  |
    |      |           4 = this is a secondary index .Ynn file (inc or non-inc)  |
    |      |           5 = this is an incrementing secondary index .Xnn file     |
    |      |           6 = this is a non-incrementing secondary index .XGn file  |
    |      |           7 = this is a secondary index .YGn file (inc or non inc)  |
    |      |           8 = this is an incrementing secondary index .XGn file     |
    |      |                                                                     |
    +------+---------------------------------------------------------------------+
    | 0005 | byte        maxTableSize                                            |
    |      |                                                                     |
    |      |        This is the "maximum table size" determined when this        |
    |      |        table was created.  It really indicates the size of each     |
    |      |        block of records in the data section of the table.           |
    |      |                                                                     |
    |      |           1 =   64M    (block size = $0400 bytes)                   |
    |      |           2 =  128M    (block size = $0800 bytes)                   |
    |      |           3 =  192M    (block size = $0C00 bytes)                   |
    |      |           4 =  256M    (block size = $1000 bytes)                   |
    |      |                                                                     |
    +------+---------------------------------------------------------------------+
    | 0006 | Integer     numRecords                                              |
    |      |                                                                     |
    |      |        This is the number of records in this file.                  |
    |      |                                                                     |
    +------+---------------------------------------------------------------------+
    | 000A | word        nextBlock                                               |
    |      |                                                                     |
    |      |        I'm not certain what this really is, but it seems to be      |
    |      |        the same as fileBlocks unless there is an empty block in     |
    |      |        the table.                                                   |
    |      |                                                                     |
    +------+---------------------------------------------------------------------+
    | 000C | word        fileBlocks                                              |
    |      |                                                                     |
    |      |        This is the number of data blocks in the file.               |
    |      |        (Each "block" is a cluster of records.)                      |
    |      |                                                                     |
    +------+---------------------------------------------------------------------+
    | 000E | word        firstBlock                                              |
    |      |                                                                     |
    |      |        Always 1 unless the table is empty.                          |
    |      |                                                                     |
    +------+---------------------------------------------------------------------+
    | 0010 | word        lastBlock                                               |
    |      |                                                                     |
    |      |        This works out to the number of blocks that the table        |
    |      |        would contain if every block was packed.                     |
    |      |                                                                     |
    +------+---------------------------------------------------------------------+
    | 0012 | word        unknown                                                 |
    |      |                                                                     |
    |      |        The value of this field seems to the change when records     |
    |      |        or blocks have been added to the table, but I still haven't  |
    |      |        figured it out.                                              |
    |      |                                                                     |
    +------+---------------------------------------------------------------------+
    | 0014 | byte        modifiedFlags1                                          |
    |      |                                                                     |
    |      |        A rebuild is required if this is not zero.                   |
    |      |                                                                     |
    +------+---------------------------------------------------------------------+
    | 0015 | byte        indexFieldNumber                                        |
    |      |                                                                     |
    |      |        In the .Xnn file of a secondary index, this is the number    |
    |      |        of the field it is referencing.                              |
    |      |                                                                     |
    |      |        This will be zero in the other files.                        |
    |      |                                                                     |
    +------+---------------------------------------------------------------------+
    | 0016 | pointer     primaryIndexWorkspace                                   |
    |      |                                                                     |
    |      |        Pointer to the primary index file header (in RAM).           |
    |      |        This will be a NIL if there is no primary index.             |
    |      |                                                                     |
    +------+---------------------------------------------------------------------+
    | 001A | pointer     unknown  (suspected pointer)                            |
    |      |                                                                     |
    |      |        This field is usually a NIL pointer.  I've only seen it      |
    |      |        used in 5.0 tables with BCD field types.  It is probably     |
    |      |        just a workspace pointer.                                    |
    |      |                                                                     |
    +------+---------------------------------------------------------------------+
    | 001E...0020        unknown                                                 |
    |      |                                                                     |
    |      |        I have only seen these three bytes used in .PX files.        |
    |      |                                                                     |


    +------+---------------------------------------------------------------------+
    | 0021 | integer     numFields                                               |
    |      |                                                                     |
    |      |        This is the number of fields in the table.  If this is an    |
    |      |        index file, then it would only be the number of fields in    |
    |      |        this index.                                                  |
    |      |                                                                     |
    +------+---------------------------------------------------------------------+
    | 0023 | integer     primaryKeyFields                                        |
    |      |                                                                     |
    |      |        This is the number of fields in the file'S primary key.      |
    |      |        It will be a zero for .PX and .Ynn files; and 2 for .Xnn     |
    |      |        secondary index files.                                       |
    |      |                                                                     |
    +------+---------------------------------------------------------------------+
    | 0025 | Integer     encryption1                                             |
    |      |                                                                     |
    |      |        This was where the encryption information was stored for     |
    |      |        versions 3.0 and 3.5.  (It was a zero if not encrypted.)     |
    |      |                                                                     |
    |      |        Subsequent versions store the value $FF00FF00 here, and      |
    |      |        move the encryption code to offset $005C.  Even so, these    |
    |      |        newer versions still maintain this information at both       |
    |      |        locations while working in RAM.                              |
    |      |                                                                     |
    |      |        Primary and .Ynn secondary index files always use this       |
    |      |        field to store the encryption code, but it is often a        |
    |      |        zero because the Paradox Engine and the Borland Database     |
    |      |        Engine do not always encrypt index files.  You can encrypt   |
    |      |        unencrypted index files by following these steps:            |
    |      |                                                                     |
    |      |           Begin with an empty encrypted table;                      |
    |      |           Truncate the index files to headerSize;                   |
    |      |           Zeroize nextBlock, fileBlocks, firstBlock and lastBlock;  |
    |      |           Copy four bytes from the .DB data file'S encryption1      |
    |      |               field (for version 3), or the encryption2 field       |
    |      |               (for versions 4 and above) into the encryption1       |
    |      |               or encryption2 field of the index file;               |
    |      |           Test thoroughly.                                          |
    |      |                                                                     |
    |      |                                                                     |
    +------+---------------------------------------------------------------------+
    | 0029 | byte        sortOrder                                               |
    |      |                                                                     |
    |      |           $00:  ASCII                                               |
    |      |           $B7:  International                                       |
    |      |           $82:  Norwegian/Danish                                    |
    |      |           $E6:  Norwegian/Danish (4.0)                              |
    |      |           $F0:  Swedish/Finnish                                     |
    |      |                                                                     |
    +------+---------------------------------------------------------------------+
    | 002A | byte        modifiedFlags2                                          |
    |      |                                                                     |
    |      |        A rebuild is required if this is not zero.                   |
    |      |                                                                     |
    +------+---------------------------------------------------------------------+
    | 002B...002C        unknown    (always 0)                                   |
    +------+---------------------------------------------------------------------+
    | 002D | byte        changeCount1                                            |
    |      |                                                                     |
    |      |        This is incremented whenever the file header is updated.     |
    |      |                                                                     |
    +------+---------------------------------------------------------------------+
    | 002E | byte        changeCount2                                            |
    |      |                                                                     |
    |      |        I'm not certain when this is incremented.                    |
    |      |                                                                     |
    +------+---------------------------------------------------------------------+
    | 002F | byte        unknown                                                 |
    +------+---------------------------------------------------------------------+
    | 0030 | ^pchar      tableNamePtrPtr                                         |
    |      |                                                                     |
    |      |        This is a pointer to tableNamePtr, which is a pointer to     |
    |      |        tableName.  Paradox uses this field to gain faster access    |
    |      |        to tableName because that part of the header is accessed     |
    |      |        sequentially.                                                |
    |      |                                                                     |
    +------+---------------------------------------------------------------------+
    | 0034 | pointer     fldInfoPtr                                              |
    |      |                                                                     |
    |      |        Pointer to the list of field identifiers.  This is listed    |
    |      |        in the accompanying Pascal record definition as a            |
    |      |        PFldInfoRec type.                                            |
    |      |                                                                     |
    |      |        You can use this pointer value to locate the table header    |
    |      |        in memory during run time.  Just subtract $0078 from this    |
    |      |        value (for 4.0+ tables), or $0058 (for .PX and .Ynn index    |
    |      |        files and version 3.0 tables).                               |
    |      |                                                                     |
    +------+---------------------------------------------------------------------+
    | 0038 | byte        writeProtected                                          |
    |      |                                                                     |
    |      |           0        write protection OFF                             |
    |      |           1        write protection ON                              |
    |      |                                                                     |
    +------+---------------------------------------------------------------------+
    | 0039 | byte        fileVersionID                                           |
    |      |                                                                     |
    |      |           $03      version 3.0                                      |
    |      |           $04      version 3.5                                      |
    |      |           $05..09  version 4.x   (usually = $09)                    |
    |      |           $0A,$0B  version 5.x                                      |
    |      |           $0C      version 7.x                                      |
    |      |                                                                     |
    +------+---------------------------------------------------------------------+
    | 003A | word        maxBlocks                                               |
    |      |                                                                     |
    |      |        I don't know what this is for.  It is usually the same       |
    |      |        as fileBlocks (at offset 000C).                              |
    |      |                                                                     |
    +------+---------------------------------------------------------------------+
    | 003C | byte        unknown                                                 |
    +------+---------------------------------------------------------------------+
    | 003D | byte        auxPasswords                                            |
    |      |                                                                     |
    |      |        Number of auxiliary passwords assigned to the table.         |
    |      |                                                                     |
    +------+---------------------------------------------------------------------+
    | 003E...003F        unknown                                                 |
    +------+---------------------------------------------------------------------+
    | 0040 | pointer     cryptInfoStartPtr                                       |
    |      |                                                                     |
    |      |        Points to cryptInfo field.  It is always NIL when not        |
    |      |        encrypted.  It is sometimes NIL even when encrypted.         |
    |      |                                                                     |
    +------+---------------------------------------------------------------------+
    | 0044 | pointer     cryptInfoEndPtr                                         |
    |      |                                                                     |
    |      |        Points to end of cryptInfo.  This is NIL if not encrypted.   |
    |      |                                                                     |
    +------+---------------------------------------------------------------------+
    | 0048 | byte        unknown                                                 |
    +------+---------------------------------------------------------------------+
    | 0049 | Integer     autoInc                                                 |
    |      |                                                                     |
    |      |        This long integer stores the value used for the next auto    |
    |      |        incrementing field in tables with a "+" autoincrementing     |
    |      |        field type.                                                  |
    |      |                                                                     |
    |      |        Formerly used as a modification count.                       |
    |      |                                                                     |
    |      |        My thanks to Orlando Ruiz for informing me of this change.   |
    |      |                                                                     |
    +------+---------------------------------------------------------------------+
    | 004D...004E        unknown                                                 |
    +------+---------------------------------------------------------------------+
    | 004F | byte        indexUpdateRequired                                     |
    +------+---------------------------------------------------------------------+
    | 0050...0054        unknown                                                 |
    +------+---------------------------------------------------------------------+
    | 0055 | byte        refIntegrity                                            |
    |      |                                                                     |
    |      |        A value here (=2?) denotes that this table uses              |
    |      |        referential integrity checks.                                |
    |      |                                                                     |
    |      |                                                                     |
    |      |             inxDirection (sec'y index file only)                    |
    |      |                                                                     |
    |      |        Secondary .Xnn index files of v7.0 tables use this           |
    |      |        field to indicate sort order direction:                      |
    |      |                                                                     |
    |      |           $01      ascending sort                                   |
    |      |           $11      descending sort                                  |
    |      |                                                                     |
    +------+---------------------------------------------------------------------+
    | 0056...0057        unknown                                                 |
    ==============================================================================
  -----------------------------------------------------------------------------------}

  { Header of file }

  PPxFileHeader = ^TPxFileHeader;
  TPxFileHeader = packed record
    RecordSize            : Word;
    HeaderSize            : Word;
    FileType              : Byte;
    MaxTableSize          : Byte;
    NumRecords            : Integer;
    NextBlock             : Word;
    FileBlocks            : Word;
    FirstBlock            : Word;
    LastBlock             : Word;
    Unknown12x13          : Word;
    ModifiedFlags1        : Byte;
    IndexFieldNumber 	    : Byte;
    PrimaryIndexWorkspace : Integer; //pointer;
    UnknownPtr1A          : Integer; //pointer;
    Unknown1Ex20          : array[$001E..$0020] of Byte;
    NumFields             : Word;
    PrimaryKeyFields      : Word;
    Encryption1           : Integer;
    SortOrder             : Byte;
    ModifiedFlags2        : Byte;
    Unknown2Bx2C          : array[$002B..$002C] of Byte;
    ChangeCount1          : Byte;
    ChangeCount2          : Byte;
    Unknown2F             : Byte;
    TableNamePtrPtr       : Integer;   //^pAnsichar;
    FieldInfoPtr          : Integer;   // PFieldInfoRecord;
    WriteProtected        : Byte;
    FileVersionID         : Byte;
    MaxBlocks             : Word;
    Unknown3C             : Byte;
    AuxPasswords          : Byte;
    Unknown3Ex3F          : array[$003E..$003F] of Byte;
    CryptInfoStartPtr     : Integer; //pointer;
    CryptInfoEndPtr       : Integer; //pointer;
    Unknown48             : Byte;
    AutoInc               : Integer;
    Unknown4Dx4E          : array[$004D..$004E] of Byte;
    IndexUpdateRequired   : Byte;
    Unknown50x54          : array[$0050..$0054] of Byte;
    RefIntegrity          : Byte;
    Unknown56x57          : array[$0056..$0057] of Byte;
  end;

  PPxDataHeader = ^TPxDataHeader;
  TPxDataHeader = packed record
    { Возможно идентификатор версии файла
      $0105..$0109 version 4.x (обычно = $0109)
      $010A, $010B version 5.x (обычно = $010B)
      $010C        version 7.0 }
    FileVersionID: Word;
    { Принимает те же значения, что и FileVersionID }
    FileVersionID2: Word;
    { Равно 0, если файл не зашифрован }
    Encryption2: Integer;
    FileUpdateTime: Integer;
    HiFieldID: Word;
    HiFieldIDInfo: Word;
    SometimesNumFields: Word;
    DosGlobalCodePage: Word;
    Unknown6Cx6F: array[$006C..$006F] of Byte;
    ChangeCount4: Word;
    Unknown72x77: array[$0072..$0077] of Byte;
  end;

  TPxRecordHeader = packed record
    RecordIndex: integer;
    BookmarkFlag: TBookmarkFlag;
  end;
  PPxRecordHeader=^TPxRecordHeader;

  TPxBlob = packed record {10-Byte Blob Info Block}
    FileLoc: Integer;
    Length: Integer;
    ModCnt: Word;
  end;

  TPxBlobIdx = packed record {Blob Pointer Array Entry}
    Offset: Byte;
    Len16: Byte;
    ModCnt: Word;
    Len: Byte;
  end;

  TPxLang = record
    Name: string[20];
    SortOrder: Byte;
    CodePage: Word;
    SortOrderID: string[8];
  end;

const
  PxLangTable: array[1..118] of TPxLang =
    ((Name: 'Access General';        SortOrder: 161; CodePage: 1252; SortOrderID: 'ACCGEN'),
     (Name: 'Access Greece';         SortOrder: 53;  CodePage: 1253; SortOrderID: 'ACCGREEK'),
     (Name: 'Access Japanese';       SortOrder: 49;  CodePage: 932;  SortOrderID: 'ACCJAPAN'),
     (Name: 'Access Nord/Danish';    SortOrder: 58;  CodePage: 1251; SortOrderID: 'ACCNRDAN'),
     (Name: 'Access Swed/Finnish';   SortOrder: 78;  CodePage: 1252; SortOrderID: 'ACCSWFIN'),
     (Name: '''ascii'' ANSI';        SortOrder: 76;  CodePage: 1252; SortOrderID: 'DBWINUS0'),
     (Name: 'Borland ANSI Arabic';   SortOrder: 63;  CodePage: 1256; SortOrderID: 'BLWINAR0'),
     (Name: 'Borland DAN Latin-1';   SortOrder: 20;  CodePage: 1252; SortOrderID: 'BLLT1DA0'),
     (Name: 'Borland DEU Latin-1';   SortOrder: 24;  CodePage: 1252; SortOrderID: 'BLLT1DE0'),
     (Name: 'Borland ENG Latin-1';   SortOrder: 47;  CodePage: 1252; SortOrderID: 'BLLT1UK0'),
     (Name: 'Borland ENU Latin-1';   SortOrder: 55;  CodePage: 1252; SortOrderID: 'BLLT1US0'),
     (Name: 'Borland ESP Latin-1';   SortOrder: 39;  CodePage: 1252; SortOrderID: 'BLLT1ES0'),
     (Name: 'Borland FIN Latin-1';   SortOrder: 30;  CodePage: 1252; SortOrderID: 'BLLT1FI0'),
     (Name: 'Borland FRA Latin-1';   SortOrder: 39;  CodePage: 1252; SortOrderID: 'BLLT1FR0'),
     (Name: 'Borland FRC Latin-1';   SortOrder: 19;  CodePage: 1252; SortOrderID: 'BLLT1CA0'),
     (Name: 'Borland ISL Latin-1';   SortOrder: 43;  CodePage: 1252; SortOrderID: 'BLLT1IS0'),
     (Name: 'Borland ITA Latin-1';   SortOrder: 44;  CodePage: 1252; SortOrderID: 'BLLT1IT0'),
     (Name: 'Borland NLD Latin-1';   SortOrder: 41;  CodePage: 1252; SortOrderID: 'BLLT1NL0'),
     (Name: 'Borland NOR Latin-1';   SortOrder: 44;  CodePage: 1252; SortOrderID: 'BLLT1NO0'),
     (Name: 'Borland PTG Latin-1';   SortOrder: 51;  CodePage: 1252; SortOrderID: 'BLLT1PT0'),
     (Name: 'Borland SVE Latin-1';   SortOrder: 56;  CodePage: 1252; SortOrderID: 'BLLT1SV0'),
     (Name: 'DB2 SQL ANSI DEU';      SortOrder: 5;   CodePage: 1252; SortOrderID: 'db2andeu'),
     (Name: 'dBASE BUL 868';         SortOrder: 181; CodePage: 868;  SortOrderID: 'BGDB868'),
     (Name: 'dBASE CHS cp936';       SortOrder: 233; CodePage: 936;  SortOrderID: 'DB936CN0'),
     (Name: 'dBASE CHT cp950';       SortOrder: 255; CodePage: 950;  SortOrderID: 'DB950TW0'),
     (Name: 'dBASE CSY cp852';       SortOrder: 242; CodePage: 852;  SortOrderID: 'DB852CZ0'),
     (Name: 'dBASE CSY cp867';       SortOrder: 248; CodePage: 867;  SortOrderID: 'DB867CZ0'),
     (Name: 'dBASE DAN cp865';       SortOrder: 222; CodePage: 865;  SortOrderID: 'DB865DA0'),
     (Name: 'dBASE DEU cp437';       SortOrder: 221; CodePage: 437;  SortOrderID: 'DB437DE0'),
     (Name: 'dBASE DEU cp850';       SortOrder: 220; CodePage: 850;  SortOrderID: 'DB850DE0'),
     (Name: 'dBASE ELL GR437';       SortOrder: 109; CodePage: 737;  SortOrderID: 'db437gr0'),
     (Name: 'dBASE ENG cp437';       SortOrder: 244; CodePage: 437;  SortOrderID: 'DB437UK0'),
     (Name: 'dBASE ENG cp850';       SortOrder: 243; CodePage: 850;  SortOrderID: 'DB850UK0'),
     (Name: 'dBASE ENU cp437';       SortOrder: 252; CodePage: 437;  SortOrderID: 'DB437US0'),
     (Name: 'dBASE ENU cp850';       SortOrder: 251; CodePage: 850;  SortOrderID: 'DB850US0'),
     (Name: 'dBASE ESP cp437';       SortOrder: 237; CodePage: 437;  SortOrderID: 'DB437ES1'),
     (Name: 'dBASE ESP cp850';       SortOrder: 235; CodePage: 850;  SortOrderID: 'DB850ES0'),
     (Name: 'dBASE FIN cp437';       SortOrder: 227; CodePage: 437;  SortOrderID: 'DB437FI0'),
     (Name: 'dBASE FRA cp437';       SortOrder: 236; CodePage: 437;  SortOrderID: 'DB437FR0'),
     (Name: 'dBASE FRA cp850';       SortOrder: 235; CodePage: 850;  SortOrderID: 'DB850FR0'),
     (Name: 'dBASE FRC cp850';       SortOrder: 220; CodePage: 850;  SortOrderID: 'DB850CF0'),
     (Name: 'dBASE FRC cp863';       SortOrder: 225; CodePage: 863;  SortOrderID: 'DB863CF1'),
     (Name: 'dBASE HUN cp852';       SortOrder: 148; CodePage: 852;  SortOrderID: 'db852hdc'),
     (Name: 'dBASE ITA cp437';       SortOrder: 241; CodePage: 437;  SortOrderID: 'DB437IT0'),
     (Name: 'dBASE ITA cp850';       SortOrder: 241; CodePage: 850;  SortOrderID: 'DB850IT1'),
     (Name: 'dBASE JPN cp932';       SortOrder: 238; CodePage: 932;  SortOrderID: 'DB932JP0'),
     (Name: 'dBASE JPN Dic932';      SortOrder: 239; CodePage: 932;  SortOrderID: 'DB932JP1'),
     (Name: 'dBASE KOR cp949';       SortOrder: 246; CodePage: 949;  SortOrderID: 'DB949KO0'),
     (Name: 'dBASE NLD cp437';       SortOrder: 238; CodePage: 437;  SortOrderID: 'DB437NL0'),
     (Name: 'dBASE NLD cp850';       SortOrder: 237; CodePage: 850;  SortOrderID: 'DB850NL0'),
     (Name: 'dBASE NOR cp865';       SortOrder: 246; CodePage: 865;  SortOrderID: 'DB865NO0'),
     (Name: 'dBASE PLK cp852';       SortOrder: 116; CodePage: 852;  SortOrderID: 'db852po0'),
     (Name: 'dBASE PTB cp850';       SortOrder: 247; CodePage: 850;  SortOrderID: 'DB850PT0'),
     (Name: 'dBASE PTG cp860';       SortOrder: 248; CodePage: 860;  SortOrderID: 'DB860PT0'),
     (Name: 'dBASE RUS cp866';       SortOrder: 129; CodePage: 866;  SortOrderID: 'db866ru0'),
     (Name: 'dBASE SLO cp852';       SortOrder: 116; CodePage: 852;  SortOrderID: 'db852sl0'),
     (Name: 'dBASE SVE cp437';       SortOrder: 253; CodePage: 437;  SortOrderID: 'DB437SV0'),
     (Name: 'dBASE SVE cp850';       SortOrder: 253; CodePage: 850;  SortOrderID: 'DB850SV1'),
     (Name: 'dBASE THA cp874';       SortOrder: 117; CodePage: 874;  SortOrderID: 'db874th0'),
     (Name: 'dBASE TRK cp857';       SortOrder: 0;   CodePage: 857;  SortOrderID: 'DB857TR0'),
     (Name: 'FoxPro Czech 1250';     SortOrder: 120; CodePage: 1250; SortOrderID: 'FOXCZWIN'),
     (Name: 'FoxPro Czech DOS895';   SortOrder: 48;  CodePage: 895;  SortOrderID: 'FOXCZ895'),
     (Name: 'FoxPro German 1252';    SortOrder: 100; CodePage: 1252; SortOrderID: 'FOXDEWIN'),
     (Name: 'FoxPro German 437';     SortOrder: 20;  CodePage: 437;  SortOrderID: 'FOXDE437'),
     (Name: 'FoxPro Nordic 1252';    SortOrder: 120; CodePage: 1252; SortOrderID: 'FOXNOWIN'),
     (Name: 'FoxPro Nordic 437';     SortOrder: 40;  CodePage: 437;  SortOrderID: 'FOXNO437'),
     (Name: 'FoxPro Nordic 850';     SortOrder: 39;  CodePage: 850;  SortOrderID: 'FOXNO850'),
     (Name: 'Hebrew dBASE';          SortOrder: 35;  CodePage: 862;  SortOrderID: 'dbHebrew'),
     (Name: 'MSSQL ANSI Greek';      SortOrder: 122; CodePage: 1253; SortOrderID: 'MSSGRWIN'),
     (Name: 'Oracle SQL WE850';      SortOrder: 27;  CodePage: 850;  SortOrderID: 'ORAWE850'),
     (Name: 'Paradox ANSI HEBREW';   SortOrder: 76;  CodePage: 1255; SortOrderID: 'ANHEBREW'),
     (Name: 'Paradox ''ascii''';     SortOrder: 0;   CodePage: 437;  SortOrderID: 'ascii'),
     (Name: 'Paradox BUL 868';       SortOrder: 71;  CodePage: 868;  SortOrderID: 'BULGARIA'),
     (Name: 'Paradox China 936';     SortOrder: 3;   CodePage: 936;  SortOrderID: 'china'),
     (Name: 'Paradox Cyrr 866';      SortOrder: 192; CodePage: 866;  SortOrderID: 'cyrr'),
     (Name: 'Paradox Czech 852';     SortOrder: 13;  CodePage: 852;  SortOrderID: 'czech'),
     (Name: 'Paradox Czech 867';     SortOrder: 226; CodePage: 867;  SortOrderID: 'cskamen'),
     (Name: 'Paradox ESP 437';       SortOrder: 22;  CodePage: 437;  SortOrderID: 'SPANISH'),
     (Name: 'Paradox Greek GR437';   SortOrder: 74;  CodePage: 737;  SortOrderID: 'grcp437'),
     (Name: 'Paradox ''hebrew''';    SortOrder: 125; CodePage: 862;  SortOrderID: 'hebrew'),
     (Name: 'Paradox Hun 852 DC';    SortOrder: 177; CodePage: 852;  SortOrderID: 'hun852dc'),
     (Name: 'Paradox ''intl''';      SortOrder: 183; CodePage: 437;  SortOrderID: 'intl'),
     (Name: 'Paradox ''intl'' 850';  SortOrder: 84;  CodePage: 850;  SortOrderID: 'intl850'),
     (Name: 'Paradox ISL 861';       SortOrder: 208; CodePage: 861;  SortOrderID: 'iceland'),
     (Name: 'Paradox ''japan''';     SortOrder: 10;  CodePage: 932;  SortOrderID: 'japan'),
     (Name: 'Paradox Korea 949';     SortOrder: 18;  CodePage: 949;  SortOrderID: 'korea'),
     (Name: 'Paradox ''nordan''';    SortOrder: 130; CodePage: 865;  SortOrderID: 'nordan'),
     (Name: 'Paradox ''nordan40''';  SortOrder: 230; CodePage: 865;  SortOrderID: 'nordan40'),
     (Name: 'Paradox Polish 852';    SortOrder: 143; CodePage: 852;  SortOrderID: 'polish'),
     (Name: 'Paradox Slovene 852';   SortOrder: 252; CodePage: 852;  SortOrderID: 'slovene'),
     (Name: 'Paradox ''swedfin''';   SortOrder: 240; CodePage: 437;  SortOrderID: 'swedfin'),
     (Name: 'Paradox Taiwan 950';    SortOrder: 132; CodePage: 950;  SortOrderID: 'taiwan'),
     (Name: 'Paradox Thai 874';      SortOrder: 166; CodePage: 874;  SortOrderID: 'thai'),
     (Name: 'Paradox ''turk''';      SortOrder: 198; CodePage: 857;  SortOrderID: 'turk'),
     (Name: 'Pdox ANSI Bulgaria';    SortOrder: 230; CodePage: 1251; SortOrderID: 'BGPD1251'),
     (Name: 'Pdox ANSI Cyrillic';    SortOrder: 143; CodePage: 1251; SortOrderID: 'ancyrr'),
     (Name: 'Pdox ANSI Czech';       SortOrder: 220; CodePage: 1250; SortOrderID: 'anczech'),
     (Name: 'Pdox ANSI Greek';       SortOrder: 14;  CodePage: 1253; SortOrderID: 'angreek1'),
     (Name: 'Pdox ANSI Hun. DC';     SortOrder: 225; CodePage: 1250; SortOrderID: 'anhundc'),
     (Name: 'Pdox ANSI Intl';        SortOrder: 98;  CodePage: 1252; SortOrderID: 'ANSIINTL'),
     (Name: 'Pdox ANSI Intl850';     SortOrder: 17;  CodePage: 1252; SortOrderID: 'ANSII850'),
     (Name: 'Pdox ANSI Nordan4';     SortOrder: 78;  CodePage: 1252; SortOrderID: 'ANSINOR4'),
     (Name: 'Pdox ANSI Polish';      SortOrder: 94;  CodePage: 1250; SortOrderID: 'anpolish'),
     (Name: 'Pdox ANSI Slovene';     SortOrder: 111; CodePage: 1250; SortOrderID: 'ansislov'),
     (Name: 'Pdox ANSI Spanish';     SortOrder: 93;  CodePage: 1252; SortOrderID: 'ANSISPAN'),
     (Name: 'Pdox ANSI Swedfin';     SortOrder: 105; CodePage: 1252; SortOrderID: 'ANSISWFN'),
     (Name: 'Pdox ANSI Turkish';     SortOrder: 213; CodePage: 1254; SortOrderID: 'ANTURK'),
     (Name: 'Pdox ''ascii'' Japan';  SortOrder: 0;   CodePage: 437;  SortOrderID: 'ascii'),
     (Name: 'pdx ANSI Czech ''CH'''; SortOrder: 83;  CodePage: 1250; SortOrderID: 'anczechw'),
     (Name: 'pdx ANSI ISO L_2 CZ';   SortOrder: 42;  CodePage: 1250; SortOrderID: 'anil2czw'),
     (Name: 'pdx Czech 852 ''CH''';  SortOrder: 132; CodePage: 852;  SortOrderID: 'czechw'),
     (Name: 'pdx Czech 867 ''CH''';  SortOrder: 89;  CodePage: 867;  SortOrderID: 'cskamenw'),
     (Name: 'pdx ISO L_2 Czech';     SortOrder: 91;  CodePage: 592;  SortOrderID: 'il2czw'),
     (Name: '''Spanish'' ANSI';      SortOrder: 60;  CodePage: 1252; SortOrderID: 'DBWINES0'),
     (Name: 'SQL Link ROMAN8';       SortOrder: 20;  CodePage: 8;    SortOrderID: 'BLROM800'),
     (Name: 'Sybase SQL Dic437';     SortOrder: 209; CodePage: 437;  SortOrderID: 'SYDC437'),
     (Name: 'Sybase SQL Dic850';     SortOrder: 208; CodePage: 850;  SortOrderID: 'SYDC850'),
     (Name: '''WEurope'' ANSI';      SortOrder: 64;  CodePage: 1252; SortOrderID: 'DBWINWE0'));

type

  TEncodeEvent = function(Sender: TObject; Field: TField; S: AnsiString): AnsiString of object;

  { TPdx }

  TParadoxDataSet = class(TDataSet)
  private
    FTableName: string;
    FFileStream: TFileStream;
    FBlobStream: TFileStream;
    FIsOpen: boolean;
    FCursor: Integer;
    FFileHeader: TPxFileHeader;
    FDataHeader: TPxDataHeader;
    FFields: array of TFieldInfoRecord;
    FFieldOffsets: array of Integer;

    { Показывает зашифрован ли файл }
    FIsEncrypted: Boolean;
    FSortOrderID: AnsiString;
    FLanguageID: Integer;
    FCodepage: string;
    FEncodingMemo: Boolean;
    FOnEncode: TEncodeEvent;

    function GetLanguage: AnsiString;
    procedure SetLanguage(const Value: AnsiString);
    procedure SetTableName(const Value: string);
    function NativeToFieldType(NativeType: Byte): TFieldType;
    function ReadDataBlock(BlockNum: Word): TDataBlock;
    { Определяет язык открытой таблицы }
    function DetectLang: Integer;
    function EncodingString(S: AnsiString): AnsiString;
    function EncodingField(S: AnsiString; Field: TField): AnsiString;
  protected
    function GetBookmarkFlag(Buffer: TRecordBuffer): TBookmarkFlag; override;
    procedure SetBookmarkFlag(Buffer: TRecordBuffer; Value: TBookmarkFlag); override;

    procedure InternalHandleException; override;
    procedure InternalInitFieldDefs; override;
    procedure InternalOpen; override;
    function IsCursorOpen: Boolean; override;
    procedure InternalClose; override;

    function GetRecord(Buffer: TRecordBuffer; GetMode: TGetMode; DoCheck: Boolean): TGetResult; override;
    function AllocRecordBuffer: TRecordBuffer; override;
    procedure FreeRecordBuffer(var Buffer: TRecordBuffer); override;
    procedure InternalInitRecord(Buffer: TRecordBuffer); override;

    procedure InternalFirst; override;
    procedure InternalLast; override;
    procedure InternalSetToRecord(Buffer: TRecordBuffer); override;

    function GetCanModify: Boolean; override;

    function GetRecordCount: Integer; override;

    procedure SetRecNo(Value: Integer); override;
    function GetRecNo: Integer; override;
    //Здесь номер записи считается от 1
  public
    constructor Create(AOwner:TComponent); override;
    destructor Destroy; override;

    function GetFieldData(Field: TField; {$IFDEF XE} var {$ENDIF} Buffer: TValueBuffer): Boolean; override;
    function CreateBlobStream(Field: TField; Mode: TBlobStreamMode): TStream; override;
    property FileHeader: TPxFileHeader read FFileHeader;
    property DataHeader: TPxDataHeader read FDataHeader;
    property SortOrderID: AnsiString read FSortOrderID;
  published
    property TableName: string read FTableName write SetTableName;
    property Language: AnsiString read GetLanguage write SetLanguage;
    property Codepage: string read FCodepage write FCodepage;
    property EncodingMemo: Boolean read FEncodingMemo write FEncodingMemo;
    
    property Active;
//    property FieldDefs stored FieldDefsStored;
    property Filter;
    property Filtered;
    property FilterOptions;
    property BeforeOpen;
    property AfterOpen;
    property BeforeClose;
    property AfterClose;
    property BeforeInsert;
    property AfterInsert;
    property BeforeEdit;
    property AfterEdit;
    property BeforePost;
    property AfterPost;
    property BeforeCancel;
    property AfterCancel;
    property BeforeDelete;
    property AfterDelete;
    property BeforeRefresh;
    property AfterRefresh;
    property BeforeScroll;
    property AfterScroll;
    property OnCalcFields;
    property OnDeleteError;
    property OnEditError;
    property OnFilterRecord;
    property OnNewRecord;
    property OnPostError;
    
    property OnEncode: TEncodeEvent read FOnEncode write FOnEncode;
  end;
  
  EParadoxError = class(Exception);

implementation

{ TParadoxDataSet }

function TParadoxDataSet.GetBookmarkFlag(Buffer: TRecordBuffer): TBookmarkFlag;
begin
  Result := PPxRecordHeader(Buffer)^.BookmarkFlag;
end;

procedure TParadoxDataSet.SetBookmarkFlag(Buffer: TRecordBuffer; Value: TBookmarkFlag);
begin
  PPxRecordHeader(Buffer)^.BookmarkFlag := Value;
end;

procedure TParadoxDataSet.InternalHandleException;
begin
  Application.HandleException(Self);
end;

//-------------------------------------载入所有字段名 chongchong 2016-07-31
procedure TParadoxDataSet.InternalInitFieldDefs;
var
  I: Integer;
  P, Offset: Integer;
  B: Byte;
  S: AnsiString;
begin
  if (FFileHeader.FileVersionID >= $05) then
    P := $78
  else
    P := $58;

  FFileStream.Seek(P, soFromBeginning);

  SetLength(FFields, FFileHeader.NumFields);
  SetLength(FFieldOffsets, FFileHeader.NumFields);

  Offset := 0;
  FieldDefs.Clear;
  for I := 0 to FFileHeader.NumFields - 1 do
  begin
    FFieldOffsets[I] := Offset;
    FFileStream.Read(FFields[I], SizeOf(TFieldInfoRecord));
    Offset := Offset + FFields[I].FieldSize;
  end;

  P := P + FFileHeader.NumFields*SizeOf(TFieldInfoRecord) + 4 + FFileHeader.NumFields*4;

  // TableName size
  if (FFileHeader.FileVersionID >= $0C) then
    P := P + 261
  else
    P := P + 79;

  FFileStream.Seek(P, soFromBeginning);
  for I := 0 to FFileHeader.NumFields - 1 do
  begin
    S := '';
    repeat
    {$IFDEF FPC}
      B := FFileStream.ReadByte;
    {$ELSE}
      FFileStream.Read(B, 1); //add by User32!!!
    {$ENDIF}
      if (B <> 0) then S := S + AnsiChar(B);
    until B = 0;

    case FFields[I].FieldType of
      pxfAlpha, pxfMemoBLOB, pxfBLOB, pxfFmtMemoBLOB, pxfOLE, pxfGraphic :
        FieldDefs.Add(S, NativeToFieldType(FFields[I].FieldType), FFields[I].FieldSize, False);
    else
      FieldDefs.Add(S, NativeToFieldType(FFields[i].FieldType));
    end;
  end;

  if not FIsEncrypted then
  begin
    P := FFileHeader.NumFields * 2;
  end
  else
  begin
    // Здесь нужно рассчитать смещение для зашифрованного файла
  end;

  FFileStream.Seek(P, soFromCurrent);
  S := '';
  repeat
  {$IFDEF FPC}
    B := FFileStream.ReadByte;
  {$ELSE}
    FFileStream.Read(B, 1); //add by User32!!!
  {$ENDIF}
    if (B <> 0) then S := S + AnsiChar(B);
  until B = 0;
  FSortOrderID := S;

  FLanguageID := DetectLang;
end;

procedure TParadoxDataSet.InternalOpen;
var
  FileName: string;
begin
  if TableName = '' then
    raise EParadoxError.CreateFmt('TableName is not set', []);

  try
    FFileStream := TFileStream.Create(TableName, fmOpenRead);
  except
    on E:Exception do
      raise EParadoxError.CreateFmt('Unable to open database "%S" - %S', [FTableName, E.Message]);
  end;
  
  FFileStream.Read(FFileHeader, SizeOf(FFileHeader));

  if  not (FFileHeader.FileType in [0, 2]) then
    raise EParadoxError.CreateFmt('"%S" - is not .DB data file', [FTableName]);

  if (FFileHeader.FileVersionID >= $05) then
  begin
    FFileStream.Read(FDataHeader, SizeOf(FDataHeader));
    FIsEncrypted := FDataHeader.Encryption2 <> 0;
  end
  else
  begin
    FIsEncrypted := FFileHeader.Encryption1 <> 0;
  end;

  FileName := ChangeFileExt(TableName, '.mb');
  if FileExists(FileName) then
  begin
    try
      FBlobStream := TFileStream.Create(ChangeFileExt(TableName, '.mb'), fmOpenRead);
    except
      FBlobStream := nil;
    end;

    if FBlobStream = nil then
    begin
      try
        FBlobStream := TFileStream.Create(ChangeFileExt(TableName, '.MB'), fmOpenRead);
      except
        //remark on 2010/02/18 by bruce0829@yahoo.com.tw
        //write('Blob file is not open: ');
        //writeln(ChangeFileExt(TableName, '.MB'));
        FBlobStream := nil;
      end;
    end;
  end;

  InternalInitFieldDefs;
  if DefaultFields then CreateFields;
  BindFields(True); //Привязываем поля к БД
  FIsOpen := True;
  FCursor := 0;
end;

function TParadoxDataSet.IsCursorOpen: Boolean;
begin
  Result := FIsOpen;
end;

procedure TParadoxDataSet.InternalClose;
begin
  BindFields(False); //Отвязываем поля
  if DefaultFields then DestroyFields;
  FIsOpen := False;

  if FBlobStream <> nil then
    FBlobStream.Free;

  FFileStream.Free;
  
  FLanguageID := 0;
end;

function TParadoxDataSet.GetRecord(Buffer: TRecordBuffer; GetMode: TGetMode; DoCheck: Boolean): TGetResult;
var
  BlockIndex, RecordStart: Word;
  TotalRecords: Integer;
  DataBlock: TDataBlock;
  P: TRecordBuffer;
  nSeekPos: Integer;
begin
  Result := grOK;

  case GetMode of
    gmPrior:
      begin
        if FCursor <= 1 then
          Result := grBOF
        else
          Dec(FCursor);
      end;
    gmNext:
      begin
        if FCursor >= RecordCount then
          Result := grEOF
        else
          Inc(FCursor);
      end;
    gmCurrent:
      begin
        if (FCursor < 1) or (FCursor > RecordCount) then
          Result := grError;
      end;
  end;

  if Result = grOK then
  begin
    PPxRecordHeader(Buffer)^.RecordIndex := FCursor;
    PPxRecordHeader(Buffer)^.BookmarkFlag := bfCurrent; //  GJK
    // Находим позицию записи в файле

    // 修正读取有问题  chongchong 2016-07-30
    BlockIndex := FFileHeader.FirstBlock;
    TotalRecords := 0;
    RecordStart := 0;
    while (TotalRecords < FCursor) do
    begin
      DataBlock := ReadDataBlock(BlockIndex);
      RecordStart := TotalRecords;
      TotalRecords := TotalRecords + (DataBlock.AddDataSize div FFileHeader.RecordSize) + 1;
      if TotalRecords >= FCursor then
        Break
      else
        BlockIndex := DataBlock.NextBlock;
    end;

    nSeekPos := FFileHeader.HeaderSize +
      (BlockIndex - 1) * FFileHeader.MaxTableSize * 1024 +
      (FCursor - RecordStart - 1) * FFileHeader.RecordSize +
      SizeOf(TDataBlock);

    FFileStream.Seek(nSeekPos, soFromBeginning);
    P := Buffer + SizeOf(TPxRecordHeader);
    FFileStream.Read(P^, FFileHeader.RecordSize);
  end
  else
  begin
    // This prevents garbage in datagrid when the last record was deleted
    P := Buffer + SizeOf(TPxRecordHeader);          // GJK
    FillChar(P^, FFileHeader.RecordSize, 0);          // GJK
    PPxRecordHeader(Buffer)^.BookmarkFlag := bfEOF; //  GJK
  end;

  if DoCheck and (Result = grError) then
  begin
    DatabaseError('Error in GetRecord()');
  end;
end;

function TParadoxDataSet.AllocRecordBuffer: TRecordBuffer;
begin
  GetMem(Result, SizeOf(TPxRecordHeader) + FFileHeader.RecordSize);
end;

procedure TParadoxDataSet.FreeRecordBuffer(var Buffer: TRecordBuffer);
begin
  FreeMem(Pointer(Buffer), SizeOf(TPxRecordHeader) + FFileHeader.RecordSize);
end;

procedure TParadoxDataSet.InternalInitRecord(Buffer: TRecordBuffer);
begin

end;

procedure TParadoxDataSet.InternalFirst;
begin
  FCursor := 0;
end;

procedure TParadoxDataSet.InternalLast;
begin
  FCursor := RecordCount + 1;
end;

procedure TParadoxDataSet.InternalSetToRecord(Buffer: TRecordBuffer);
begin
  FCursor := PPxRecordHeader(Buffer)^.RecordIndex;
end;

function TParadoxDataSet.GetCanModify: Boolean;
begin
  Result := False;
end;

function TParadoxDataSet.GetRecordCount: Integer;
begin
  Result := FFileHeader.NumRecords;
end;

procedure TParadoxDataSet.SetRecNo(Value: Integer);
begin
  if (Value < 1) or (Value >= RecordCount + 1) then Exit;
  FCursor := Value;
  Resync([]);
end;

function TParadoxDataSet.GetRecNo: Integer;
begin
  Result := PPxRecordHeader(ActiveBuffer)^.RecordIndex;
end;

procedure TParadoxDataSet.SetTableName(const Value: string);
begin
  if FTableName <> Value then
  begin
    if Active then Close;
    FTableName := Value;
  end;
end;

function TParadoxDataSet.GetLanguage: AnsiString;
begin
  if (FLanguageID > 0) and (FLanguageID <= 118) then
    Result := PxLangTable[FLanguageID].Name
  else
    Result := '';
end;

procedure TParadoxDataSet.SetLanguage(const Value: AnsiString);
var
  I: Integer;
begin
  if Active then
  begin
    for I := 1 to 118 do
    begin
      if PxLangTable[I].Name = Value then
      begin
        FLanguageID := I;
        Break;
      end;
    end;
  end
  else
  begin
    FLanguageID := 0;
  end;
end;

function TParadoxDataSet.NativeToFieldType(NativeType: Byte): TFieldType;
begin
  Result := ftUnknown;
  case NativeType of
    pxfAlpha:       Result := ftString;
    pxfDate:        Result := ftDate;
    pxfShort:       Result := ftSmallInt;
    pxfLong:        Result := ftInteger;
    pxfCurrency:    Result := ftCurrency;
    pxfNumber:      Result := ftFloat;
    pxfLogical:     Result := ftBoolean;
    pxfMemoBLOB:    Result := ftMemo;
    pxfBLOB:        Result := ftBlob;
    pxfFmtMemoBLOB: Result := ftFmtMemo;
    pxfOLE:         Result := ftParadoxOle;
    pxfGraphic:     Result := ftGraphic;
    pxfTime:        Result := ftTime;
    pxfTimestamp:   Result := ftDateTime;
    pxfAutoInc:     Result := ftAutoInc;
    pxfBCD:         Result := ftBCD;
    pxfBytes:       Result := ftBytes;
  end;
end;

function TParadoxDataSet.ReadDataBlock(BlockNum: Word): TDataBlock;
begin
  if (BlockNum < 1) or (BlockNum > FFileHeader.FileBlocks) then
    raise EParadoxError.CreateFmt('Block %d read error', [BlockNum]);
  FFileStream.Seek(FFileHeader.HeaderSize + (BlockNum - 1) * FFileHeader.MaxTableSize * 1024, soFromBeginning);
  FFileStream.ReadBuffer(Result, SizeOf(TDataBlock));
end;

function TParadoxDataSet.DetectLang: Integer;
var
  I: Integer;
begin
  Result := 0;
  for I := 1 to 118 do
  begin
    if (FFileHeader.FileVersionID >= $05) then
    begin
      if (PxLangTable[I].SortOrder = FFileHeader.SortOrder) and
        (PxLangTable[I].CodePage = FDataHeader.DosGlobalCodePage) and
        (PxLangTable[I].SortOrderID = FSortOrderID) then
      begin
        Result := I;
        Break;
      end;
    end
    else
    begin
      if (PxLangTable[I].SortOrder = FFileHeader.SortOrder) then
      begin
        Result := I;
        Break;
      end;
    end;
  end;
end;

function TParadoxDataSet.EncodingField(S: AnsiString; Field: TField): AnsiString;
begin
  Result := S;

  if Assigned(FOnEncode) then
  begin
    Result := FOnEncode(Self, Field, S);
  end
  else
  begin
    if FLanguageID < 1 then Exit;
    Result := EncodingString(S);
  end;  
end;

constructor TParadoxDataSet.Create(AOwner: TComponent);
begin
  inherited Create(AOwner);
  FCodepage := GetCodepage;
  FEncodingMemo := True;
end;

destructor TParadoxDataSet.Destroy;
begin
  inherited Destroy;
end;

function TParadoxDataSet.GetFieldData(Field: TField; {$IFDEF XE} var {$ENDIF} Buffer: TValueBuffer): Boolean;
var
  P: array of AnsiChar;
  I: Integer;
  Src: TRecordBuffer;
  IsNull: Boolean;
begin
  if Buffer = nil then
  begin
    Result := False;
    Exit;
  end;

  Result := True;
  SetLength(P, FFields[Field.FieldNo - 1].FieldSize);

{$IFDEF XE}
  Src := PByte(ActiveBuffer + SizeOf(TPxRecordHeader) + FFieldOffsets[Field.FieldNo - 1]);
{$ELSE}
  Src := ActiveBuffer + SizeOf(TPxRecordHeader) + FFieldOffsets[Field.FieldNo - 1];
{$ENDIF}

  IsNull := True;
  if FFields[Field.FieldNo - 1].FieldType in [2..6, $14..$16] then
  begin
    for I := 0 to FFields[Field.FieldNo - 1].FieldSize - 1 do
    begin
      P[I] := PAnsiChar(Src + FFields[Field.FieldNo - 1].FieldSize - I - 1)^;
      if Ord(P[I]) <> 0 then
      begin
        IsNull := False;
      end;
    end;

    //GJK:Using a loop var outside the loop can cause (in Delphi) strange behavior
    //P[I] := Chr(Ord(P[I]) xor $80);
    P[FFields[Field.FieldNo - 1].FieldSize - 1] := AnsiChar(Ord(P[FFields[Field.FieldNo - 1].FieldSize - 1]) xor $80);

    if IsNull then
    begin
      Result := False;
      Exit;
    end;
  end;

  case FFields[Field.FieldNo - 1].FieldType of
    pxfAlpha:       StrLCopy(PAnsiChar(Buffer), PAnsiChar(Src), FFields[Field.FieldNo - 1].FieldSize);
    pxfDate:        PLongint(Buffer)^ := PLongint(P)^;
    pxfShort:       PSmallInt(Buffer)^ := PSmallInt(P)^;
    pxfLong:        PInteger(Buffer)^ := PInteger(P)^;
    pxfCurrency:    PDouble(Buffer)^ := PDouble(P)^;
    pxfNumber:      PDouble(Buffer)^ := PDouble(P)^;
    pxfLogical:     PWordbool(Buffer)^ := (Ord(Src^) = $80);
    pxfTime:        PDouble(Buffer)^ :=  PDouble(P)^;
    pxfTimestamp:   PDouble(Buffer)^ :=  PDouble(P)^;
    pxfAutoInc:     PInteger(Buffer)^ := PInteger(P)^;

    //pxfMemoBLOB     = $0C;
    //pxfBLOB         = $0D;
    //pxfFmtMemoBLOB  = $0E;
    //pxfOLE          = $0F;
    //pxfGraphic      = $10;
    //pxfBCD          = $17;
    //pxfBytes        = $18;
  else
    Result := False;
  end;
end;

function TParadoxDataSet.CreateBlobStream(Field: TField; Mode: TBlobStreamMode): TStream;
var
  MS: TMemoryStream;
  Src, Header: TRecordBuffer;
  Blob: TPxBlob;
  BlobIdx: TPxBlobIdx;
  S: AnsiString;
  Idx: Byte;
  Loc: Integer;
  //Buffer: PAnsiChar;
begin
  Result := nil;
  if (Mode <> bmRead) then Exit;

{$IFDEF XE}
  Src := PByte(ActiveBuffer + SizeOf(TPxRecordHeader) + FFieldOffsets[Field.FieldNo - 1]);
{$ELSE}
  Src := ActiveBuffer + SizeOf(TPxRecordHeader) + FFieldOffsets[Field.FieldNo - 1];
{$ENDIF}

  Header := Src + Field.Size - SizeOf(TPxBlob);
  Move(Header^, Blob, SizeOf(Blob));

  if Blob.Length = 0 then Exit;

  MS := TMemoryStream.Create;
  Result := MS;

  if Blob.Length > Field.Size - SizeOf(TPxBlob) then
  begin
    if FBlobStream <> nil then
    begin
      Idx := Blob.FileLoc and $FF;
      Loc := Blob.FileLoc and $FFFFFF00;

      if Idx = $FF then
      begin {Read from a Single Blob Block}
        FBlobStream.Seek(Loc + 9, soFromBeginning);
        if Field.DataType = ftMemo then
        begin
          SetLength(S, Blob.Length);
          FBlobStream.Read(S[1], Blob.Length);

          if EncodingMemo then S := EncodingField(S, Field);

          MS.Write(S[1], Length(S));
        end
        else
        begin
          MS.CopyFrom(FBlobStream, Blob.Length);
        end;
      end
      else
      begin
        FBlobStream.Seek(Loc + 12 + 5 * Idx, soFromBeginning);
        FBlobStream.Read(BlobIdx, SizeOf(TPxBlobIdx));
        FBlobStream.Seek(Loc + 16 * BlobIdx.Offset, soFromBeginning);

        if Field.DataType = ftMemo then
        begin
          SetLength(S, Blob.Length);
          FBlobStream.Read(S[1], Blob.Length);

          if EncodingMemo then S := EncodingField(S, Field);

          MS.Write(S[1], Length(S));
        end
        else
        begin
          MS.CopyFrom(FBlobStream, Blob.Length);
        end;
      end;
    end;
  end
  else
  begin
    if Field.DataType = ftMemo then
    begin
      {
      StrLCopy(Buffer, PAnsiChar(Src), Blob.Length);
      S := Buffer;
      MS.Write(S[1], Length(S));
      }
      MS.Write(Src, Blob.Length);
    end
    else
    begin
      MS.Write(Src, Blob.Length);
    end;
  end;
    
  MS.Position := 0;
end;

function TParadoxDataSet.EncodingString(S: AnsiString): AnsiString;
begin
  Result := S;
  case PxLangTable[FLanguageID].CodePage of
    1251:
      begin
        if Codepage = 'UTF-8' then Result := Encoding(CP1251, UTF8, S);
        if Codepage = 'KOI8R' then Result := Encoding(CP1251, KOI8R, S);
      end;
    866:
      begin
        if Codepage = 'UTF-8' then Result := Encoding(CP866, UTF8, S);
        if Codepage = 'KOI8R' then Result := Encoding(CP866, KOI8R, S);
        if Codepage = 'CP1251' then Result := Encoding(CP866, CP1251, S);
      end;
  end;
end;

end.

