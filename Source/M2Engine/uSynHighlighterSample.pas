{ -------------------------------------------------------------------------------
  The contents of this file are subject to the Mozilla Public License
  Version 1.1 (the "License"); you may not use this file except in compliance
  with the License. You may obtain a copy of the License at
  http://www.mozilla.org/MPL/

  Software distributed under the License is distributed on an "AS IS" basis,
  WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License for
  the specific language governing rights and limitations under the License.

  Code template generated with SynGen.
  The original code is: E:\OfficeWorks\GxxEngine\GameOfGeeM2\M2Engine\uSynHighlighterSample.pas, released 2023-06-20.
  Description: Syntax Parser/Highlighter
  The initial author of this file is Administrator.
  Copyright (c) 2023, all rights reserved.

  Contributors to the SynEdit and mwEdit projects are listed in the
  Contributors.txt file.

  Alternatively, the contents of this file may be used under the terms of the
  GNU General Public License Version 2 or later (the "GPL"), in which case
  the provisions of the GPL are applicable instead of those above.
  If you wish to allow use of your version of this file only under the terms
  of the GPL and not to allow others to use your version of this file
  under the MPL, indicate your decision by deleting the provisions above and
  replace them with the notice and other provisions required by the GPL.
  If you do not delete the provisions above, a recipient may use your version
  of this file under either the MPL or the GPL.

  $Id: $

  You may retrieve the latest version of this file at the SynEdit home page,
  located at http://SynEdit.SourceForge.net

  ------------------------------------------------------------------------------- }

{$IFNDEF QUSYNHIGHLIGHTERSAMPLE}
unit uSynHighlighterSample;
{$ENDIF}
{$I SynEdit.inc}

interface

uses
{$IFDEF SYN_CLX}
  QGraphics,
  QSynEditTypes,
  QSynEditHighlighter,
  QSynUnicode,
{$ELSE}
  Graphics,
  SynEditTypes,
  SynEditHighlighter,
  SynUnicode,
{$ENDIF}
  SysUtils,
  Classes;

type
  TtkTokenKind = (tkComment, tkIdentifier, tkJump, tkKey, tkNull, tkParagraph, tkParameterLabels, tkUnknown);

  TRangeState = (rsUnKnown, rsLine, rsParagraph, rsParameterLabels);

  TProcTableProc = procedure of object;

  PIdentFuncTableFunc = ^TIdentFuncTableFunc;
  TIdentFuncTableFunc = function(Index: Integer): TtkTokenKind of object;

type
  TSynSampleSyn = class(TSynCustomHighlighter)
  private
    fRange: TRangeState;
    fTokenID: TtkTokenKind;
    fIdentFuncTable: array [0 .. 12] of TIdentFuncTableFunc;
    fCommentAttri: TSynHighlighterAttributes;
    fIdentifierAttri: TSynHighlighterAttributes;
    fJumpAttri: TSynHighlighterAttributes;
    fKeyAttri: TSynHighlighterAttributes;
    fParagraphAttri: TSynHighlighterAttributes;
    fParameterLabelsAttri: TSynHighlighterAttributes;
    function HashKey(Str: PWideChar): Cardinal;
    function FuncAct(Index: Integer): TtkTokenKind;
    function FuncBreak(Index: Integer): TtkTokenKind;
    function FuncCall(Index: Integer): TtkTokenKind;
    function FuncDefine(Index: Integer): TtkTokenKind;
    function FuncDelaycall(Index: Integer): TtkTokenKind;
    function FuncDelaygoto(Index: Integer): TtkTokenKind;
    function FuncElseact(Index: Integer): TtkTokenKind;
    function FuncElsesay(Index: Integer): TtkTokenKind;
    function FuncGoto(Index: Integer): TtkTokenKind;
    function FuncIf(Index: Integer): TtkTokenKind;
    function FuncOr(Index: Integer): TtkTokenKind;
    function FuncSay(Index: Integer): TtkTokenKind;
    procedure IdentProc;
    procedure UnknownProc;
    function AltFunc(Index: Integer): TtkTokenKind;
    procedure InitIdent;
    function IdentKind(MayBe: PWideChar): TtkTokenKind;
    procedure NullProc;
    procedure CRProc;
    procedure LFProc;
    procedure LineOpenProc;
    procedure LineProc;
    procedure ParagraphOpenProc;
    procedure ParagraphProc;
    procedure ParameterLabelsOpenProc;
    procedure ParameterLabelsProc;
  protected
    function GetSampleSource: UnicodeString; override;
    function IsFilterStored: Boolean; override;
  public
    constructor Create(AOwner: TComponent); override;
    class function GetFriendlyLanguageName: UnicodeString; override;
    class function GetLanguageName: string; override;
    function GetRange: Pointer; override;
    procedure ResetRange; override;
    procedure SetRange(Value: Pointer); override;
    function GetDefaultAttribute(Index: Integer): TSynHighlighterAttributes; override;
    function GetEol: Boolean; override;
    function GetKeyWords(TokenKind: Integer): UnicodeString; override;
    function GetTokenID: TtkTokenKind;
    function GetTokenAttribute: TSynHighlighterAttributes; override;
    function GetTokenKind: Integer; override;
    function IsIdentChar(AChar: WideChar): Boolean; override;
    procedure Next; override;
  published
    property CommentAttri: TSynHighlighterAttributes read fCommentAttri write fCommentAttri;
    property IdentifierAttri: TSynHighlighterAttributes read fIdentifierAttri write fIdentifierAttri;
    property JumpAttri: TSynHighlighterAttributes read fJumpAttri write fJumpAttri;
    property KeyAttri: TSynHighlighterAttributes read fKeyAttri write fKeyAttri;
    property ParagraphAttri: TSynHighlighterAttributes read fParagraphAttri write fParagraphAttri;
    property ParameterLabelsAttri: TSynHighlighterAttributes read fParameterLabelsAttri write fParameterLabelsAttri;
  end;

implementation

uses
{$IFDEF SYN_CLX}
  QSynEditStrConst;
{$ELSE}
  SynEditStrConst;
{$ENDIF}

resourcestring
  SYNS_FilterDelphiCBuilderFormDefinitions = 'All files (*.*)|*.*';
  SYNS_LangDelphiCBuilderFormDefinitions = 'Delphi/C++ Builder Form Definitions';
  SYNS_FriendlyLangDelphiCBuilderFormDefinitions = 'Delphi/C++ Builder Form Definitions';
  SYNS_AttrJump = 'Jump';
  SYNS_FriendlyAttrJump = 'Jump';
  SYNS_AttrParagraph = 'Paragraph';
  SYNS_FriendlyAttrParagraph = 'Paragraph';
  SYNS_AttrParameterLabels = 'ParameterLabels';
  SYNS_FriendlyAttrParameterLabels = 'ParameterLabels';

const
  // as this language is case-insensitive keywords *must* be in lowercase
  KeyWords: array [0 .. 11] of UnicodeString = ('act', 'break', 'call', 'define', 'delaycall', 'delaygoto', 'elseact', 'elsesay',
    'goto', 'if', 'or', 'say');

  KeyIndices: array [0 .. 12] of Integer = (11, 10, 5, 0, 7, 2, 3, 6, 4, 9, -1, 1, 8);

procedure TSynSampleSyn.InitIdent;
var
  i: Integer;
begin
  for i := Low(fIdentFuncTable) to High(fIdentFuncTable) do
    if KeyIndices[i] = -1 then
      fIdentFuncTable[i] := AltFunc;

  fIdentFuncTable[3] := FuncAct;
  fIdentFuncTable[11] := FuncBreak;
  fIdentFuncTable[5] := FuncCall;
  fIdentFuncTable[6] := FuncDefine;
  fIdentFuncTable[8] := FuncDelaycall;
  fIdentFuncTable[2] := FuncDelaygoto;
  fIdentFuncTable[7] := FuncElseact;
  fIdentFuncTable[4] := FuncElsesay;
  fIdentFuncTable[12] := FuncGoto;
  fIdentFuncTable[9] := FuncIf;
  fIdentFuncTable[1] := FuncOr;
  fIdentFuncTable[0] := FuncSay;
end;

{$Q-}

function TSynSampleSyn.HashKey(Str: PWideChar): Cardinal;
begin
  Result := 0;
  while IsIdentChar(Str^) do
  begin
    Result := Result * 61 + Ord(Str^) * 5;
    inc(Str);
  end;
  Result := Result mod 13;
  fStringLen := Str - fToIdent;
end;
{$Q+}

function TSynSampleSyn.FuncAct(Index: Integer): TtkTokenKind;
begin
  if IsCurrentToken(KeyWords[Index]) then
    Result := tkKey
  else
    Result := tkIdentifier;
end;

function TSynSampleSyn.FuncBreak(Index: Integer): TtkTokenKind;
begin
  if IsCurrentToken(KeyWords[Index]) then
    Result := tkJump
  else
    Result := tkIdentifier;
end;

function TSynSampleSyn.FuncCall(Index: Integer): TtkTokenKind;
begin
  if IsCurrentToken(KeyWords[Index]) then
    Result := tkJump
  else
    Result := tkIdentifier;
end;

function TSynSampleSyn.FuncDefine(Index: Integer): TtkTokenKind;
begin
  if IsCurrentToken(KeyWords[Index]) then
    Result := tkKey
  else
    Result := tkIdentifier;
end;

function TSynSampleSyn.FuncDelaycall(Index: Integer): TtkTokenKind;
begin
  if IsCurrentToken(KeyWords[Index]) then
    Result := tkJump
  else
    Result := tkIdentifier;
end;

function TSynSampleSyn.FuncDelaygoto(Index: Integer): TtkTokenKind;
begin
  if IsCurrentToken(KeyWords[Index]) then
    Result := tkJump
  else
    Result := tkIdentifier;
end;

function TSynSampleSyn.FuncElseact(Index: Integer): TtkTokenKind;
begin
  if IsCurrentToken(KeyWords[Index]) then
    Result := tkKey
  else
    Result := tkIdentifier;
end;

function TSynSampleSyn.FuncElsesay(Index: Integer): TtkTokenKind;
begin
  if IsCurrentToken(KeyWords[Index]) then
    Result := tkKey
  else
    Result := tkIdentifier;
end;

function TSynSampleSyn.FuncGoto(Index: Integer): TtkTokenKind;
begin
  if IsCurrentToken(KeyWords[Index]) then
    Result := tkJump
  else
    Result := tkIdentifier;
end;

function TSynSampleSyn.FuncIf(Index: Integer): TtkTokenKind;
begin
  if IsCurrentToken(KeyWords[Index]) then
    Result := tkKey
  else
    Result := tkIdentifier;
end;

function TSynSampleSyn.FuncOr(Index: Integer): TtkTokenKind;
begin
  if IsCurrentToken(KeyWords[Index]) then
    Result := tkKey
  else
    Result := tkIdentifier;
end;

function TSynSampleSyn.FuncSay(Index: Integer): TtkTokenKind;
begin
  if IsCurrentToken(KeyWords[Index]) then
    Result := tkKey
  else
    Result := tkIdentifier;
end;

function TSynSampleSyn.AltFunc(Index: Integer): TtkTokenKind;
begin
  Result := tkIdentifier;
end;

function TSynSampleSyn.IdentKind(MayBe: PWideChar): TtkTokenKind;
var
  Key: Cardinal;
begin
  fToIdent := MayBe;
  Key := HashKey(MayBe);
  if Key <= High(fIdentFuncTable) then
    Result := fIdentFuncTable[Key](KeyIndices[Key])
  else
    Result := tkIdentifier;
end;

procedure TSynSampleSyn.NullProc;
begin
  fTokenID := tkNull;
  inc(Run);
end;

procedure TSynSampleSyn.CRProc;
begin
  fTokenID := tkUnknown;
  inc(Run);
  if fLine[Run] = #10 then
    inc(Run);
end;

procedure TSynSampleSyn.LFProc;
begin
  fTokenID := tkUnknown;
  inc(Run);
end;

procedure TSynSampleSyn.LineOpenProc;
begin
  inc(Run);
  fRange := rsLine;
  LineProc;
  fTokenID := tkComment;
end;

procedure TSynSampleSyn.LineProc;
begin
  fTokenID := tkComment;
  repeat
    if (fLine[Run] = '#') and (fLine[Run + 1] = '1') then
    begin
      inc(Run, 2);
      fRange := rsUnKnown;
      Break;
    end;
    if not IsLineEnd(Run) then
      inc(Run);
  until IsLineEnd(Run);
end;

procedure TSynSampleSyn.ParagraphOpenProc;
begin
  inc(Run);
  if (fLine[Run] = '@') then
  begin
    inc(Run, 1);
    fRange := rsParagraph;
    ParagraphProc;
    fTokenID := tkParagraph;
  end
  else
    fTokenID := tkIdentifier;
end;

procedure TSynSampleSyn.ParagraphProc;
begin
  fTokenID := tkParagraph;
  repeat
    if (fLine[Run] = ']') then
    begin
      inc(Run, 1);
      fRange := rsUnKnown;
      Break;
    end;
    if not IsLineEnd(Run) then
      inc(Run);
  until IsLineEnd(Run);
end;

procedure TSynSampleSyn.ParameterLabelsOpenProc;
begin
  inc(Run);
  fRange := rsParameterLabels;
  ParameterLabelsProc;
  fTokenID := tkParameterLabels;
end;

procedure TSynSampleSyn.ParameterLabelsProc;
begin
  fTokenID := tkParameterLabels;
  repeat
    if (fLine[Run] = '>') then
    begin
      inc(Run, 1);
      fRange := rsUnKnown;
      Break;
    end;
    if not IsLineEnd(Run) then
      inc(Run);
  until IsLineEnd(Run);
end;

constructor TSynSampleSyn.Create(AOwner: TComponent);
begin
  inherited Create(AOwner);
  fCaseSensitive := False;

  fCommentAttri := TSynHighlighterAttributes.Create(SYNS_AttrComment, SYNS_FriendlyAttrComment);
  fCommentAttri.Style := [fsItalic];
  fCommentAttri.Foreground := clGreen;
  AddAttribute(fCommentAttri);

  fIdentifierAttri := TSynHighlighterAttributes.Create(SYNS_AttrIdentifier, SYNS_FriendlyAttrIdentifier);
  AddAttribute(fIdentifierAttri);

  fJumpAttri := TSynHighlighterAttributes.Create(SYNS_AttrJump, SYNS_FriendlyAttrJump);
  fJumpAttri.Style := [fsUnderline, fsItalic];
  fJumpAttri.Foreground := clBlue;
  fJumpAttri.Background := clSilver;
  AddAttribute(fJumpAttri);

  fKeyAttri := TSynHighlighterAttributes.Create(SYNS_AttrReservedWord, SYNS_FriendlyAttrReservedWord);
  fKeyAttri.Foreground := clBlue;
  AddAttribute(fKeyAttri);

  fParagraphAttri := TSynHighlighterAttributes.Create(SYNS_AttrParagraph, SYNS_FriendlyAttrParagraph);
  fParagraphAttri.Foreground := $008000FF;
  AddAttribute(fParagraphAttri);

  fParameterLabelsAttri := TSynHighlighterAttributes.Create(SYNS_AttrParameterLabels, SYNS_FriendlyAttrParameterLabels);
  fParameterLabelsAttri.Foreground := clTeal;
  AddAttribute(fParameterLabelsAttri);

  SetAttributesOnChange(DefHighlightChange);
  InitIdent;
  fDefaultFilter := SYNS_FilterDelphiCBuilderFormDefinitions;
  fRange := rsUnKnown;
end;

procedure TSynSampleSyn.IdentProc;
begin
  fTokenID := IdentKind(fLine + Run);
  inc(Run, fStringLen);
  while IsIdentChar(fLine[Run]) do
    inc(Run);
end;

procedure TSynSampleSyn.UnknownProc;
begin
  inc(Run);
  fTokenID := tkUnknown;
end;

procedure TSynSampleSyn.Next;
begin
  fTokenPos := Run;
  case fLine[Run] of
    #0:
      NullProc;
    #10:
      LFProc;
    #13:
      CRProc;
    ';':
      LineOpenProc;
    '[':
      ParagraphOpenProc;
    '<':
      ParameterLabelsOpenProc;
    'A' .. 'Z', 'a' .. 'z', '_':
      IdentProc;
  else
    UnknownProc;
  end;
  inherited;
end;

function TSynSampleSyn.GetDefaultAttribute(Index: Integer): TSynHighlighterAttributes;
begin
  case Index of
    SYN_ATTR_COMMENT:
      Result := fCommentAttri;
    SYN_ATTR_IDENTIFIER:
      Result := fIdentifierAttri;
    SYN_ATTR_KEYWORD:
      Result := fKeyAttri;
  else
    Result := nil;
  end;
end;

function TSynSampleSyn.GetEol: Boolean;
begin
  Result := Run = fLineLen + 1;
end;

function TSynSampleSyn.GetKeyWords(TokenKind: Integer): UnicodeString;
begin
  Result := 'ACT,BREAK,CALL,DEFINE,DELAYCALL,DELAYGOTO,ELSEACT,ELSESAY,GOTO,IF,OR,SAY';
end;

function TSynSampleSyn.GetTokenID: TtkTokenKind;
begin
  Result := fTokenID;
end;

function TSynSampleSyn.GetTokenAttribute: TSynHighlighterAttributes;
begin
  case GetTokenID of
    tkComment:
      Result := fCommentAttri;
    tkIdentifier:
      Result := fIdentifierAttri;
    tkJump:
      Result := fJumpAttri;
    tkKey:
      Result := fKeyAttri;
    tkParagraph:
      Result := fParagraphAttri;
    tkParameterLabels:
      Result := fParameterLabelsAttri;
    tkUnknown:
      Result := fIdentifierAttri;
  else
    Result := nil;
  end;
end;

function TSynSampleSyn.GetTokenKind: Integer;
begin
  Result := Ord(fTokenID);
end;

function TSynSampleSyn.IsIdentChar(AChar: WideChar): Boolean;
begin
  case AChar of
    '_', '0' .. '9', 'a' .. 'z', 'A' .. 'Z':
      Result := True;
  else
    Result := False;
  end;
end;

function TSynSampleSyn.GetSampleSource: UnicodeString;
begin
  Result := 'Sample source for: '#13#10 + 'Syntax Parser/Highlighter';
end;

function TSynSampleSyn.IsFilterStored: Boolean;
begin
  Result := fDefaultFilter <> SYNS_FilterDelphiCBuilderFormDefinitions;
end;

class function TSynSampleSyn.GetFriendlyLanguageName: UnicodeString;
begin
  Result := SYNS_FriendlyLangDelphiCBuilderFormDefinitions;
end;

class function TSynSampleSyn.GetLanguageName: string;
begin
  Result := SYNS_LangDelphiCBuilderFormDefinitions;
end;

procedure TSynSampleSyn.ResetRange;
begin
  fRange := rsUnKnown;
end;

procedure TSynSampleSyn.SetRange(Value: Pointer);
begin
  fRange := TRangeState(Value);
end;

function TSynSampleSyn.GetRange: Pointer;
begin
  Result := Pointer(fRange);
end;

initialization

{$IFNDEF SYN_CPPB_1}
  RegisterPlaceableHighlighter(TSynSampleSyn);
{$ENDIF}

end.
