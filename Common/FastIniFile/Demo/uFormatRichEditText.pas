unit uFormatRichEditText;

{$I CompVers.inc}

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms,
  ComCtrls, RichEdit;

procedure FormatRichEditText(RichEdit: TRichEdit);

implementation


function FindFirstComment(const S: String): Integer;
var
  I: Integer;
  InQuotes: Boolean;
begin
  Result := 0;
  InQuotes := False;
  for I := 1 to Length(S) do
    case S[I] of
      '"': InQuotes := not InQuotes;
      ';': if not InQuotes then
           begin
             Result := I;
             Exit;
           end;
    end;
end;

procedure InitFormat(var Format: TCharFormat);
begin
  FillChar(Format, SizeOf(TCharFormat), 0);
  Format.cbSize := SizeOf(TCharFormat);
end;

procedure MarkTextAsSection(RichEdit: TRichEdit; S, L: integer);
var
  CharRange: TCharRange;
  Format   : TCharFormat;
begin
  CharRange.cpMin := S;
  CharRange.cpMax := S+L;
  RichEdit.Perform(EM_EXSETSEL, 0, Longint(@CharRange));

  InitFormat(Format);
  Format.dwMask      := CFM_COLOR + CFM_BOLD;
  Format.dwEffects   := CFE_BOLD;
  Format.crTextColor := clRed;
  RichEdit.Perform(EM_SETCHARFORMAT, SCF_SELECTION, LPARAM(@Format))
end;

procedure MarkTextAsComment(RichEdit: TRichEdit; S, L: integer);
var
  CharRange: TCharRange;
  Format   : TCharFormat;
begin
  CharRange.cpMin := S;
  CharRange.cpMax := S+L;
  RichEdit.Perform(EM_EXSETSEL, 0, Longint(@CharRange));

  InitFormat(Format);
  Format.dwMask      := CFM_COLOR + CFM_ITALIC;
  Format.dwEffects   := CFE_ITALIC;
  Format.crTextColor := clBlue;
  RichEdit.Perform(EM_SETCHARFORMAT, SCF_SELECTION, LPARAM(@Format))
end;

procedure MarkTextAsIdent(RichEdit: TRichEdit; S, L: integer);
var
  CharRange: TCharRange;
  Format   : TCharFormat;
begin
  CharRange.cpMin := S;
  CharRange.cpMax := S+L;
  RichEdit.Perform(EM_EXSETSEL, 0, Longint(@CharRange));

  InitFormat(Format);
  Format.dwMask      := CFM_COLOR + CFM_BOLD;
  Format.dwEffects   := CFE_BOLD;
  Format.crTextColor := clGray;
  RichEdit.Perform(EM_SETCHARFORMAT, SCF_SELECTION, LPARAM(@Format))
end;

procedure MarkTextAsValue(RichEdit: TRichEdit; S, L: integer);
var
  CharRange: TCharRange;
  Format   : TCharFormat;
begin
  CharRange.cpMin := S;
  CharRange.cpMax := S+L;
  RichEdit.Perform(EM_EXSETSEL, 0, Longint(@CharRange));

  InitFormat(Format);
  Format.dwMask      := CFM_COLOR;
  Format.dwEffects   := 0;
  Format.crTextColor := clGreen;
  RichEdit.Perform(EM_SETCHARFORMAT, SCF_SELECTION, LPARAM(@Format))
end;

procedure FormatRichEditText(RichEdit: TRichEdit);
var
  C,                      // Position of inline comments
  I,                      // Just a counter
  L        : integer;     // Tracking text pos within richedit
  Line     : string;      // Current line being scanned.
  LLine    : integer;     // Length of current line
  EqualPos : integer;     // Position of '=' within current line
  EventMask: Integer;
  sl       : TStringList; // Temp copy of richedit.lines.
begin
  // Notes - Need to work with copy of richedit.lines
  //         at some point richedit.lines screws up
  //         and returns empty strings.
  //       - WordWrap must be False to function properly.

  RichEdit.WordWrap := False;

  // Code taking way to much time.
  Screen.Cursor := crHourGlass;
  try
    // Shut down all event generating stuff of the RichEdit.
    EventMask := SendMessage(RichEdit.Handle, EM_SETEVENTMASK, 0, 0);

    // It also helps a bit to clear the redraw flag for moment
    // and turn it on again when finished
    RichEdit.Perform(WM_SETREDRAW, 0, 0);
    try
      // But for all these changes to work you definitely need
      // a copy of the RichEdit.Lines in a separate Stringlist
      // otherwise only the visible part of the richedit gets
      // updated.
      sl := TStringList.Create;
      try
        sl.Assign(RichEdit.Lines);

        L := 0;
        for i := 0 to sl.Count - 1 do
        begin
          Line := sl[i];
          LLine:= Length(Line);
          if LLine > 0 then
          begin
            // handle section headers
            if (Line[1] = '[') and (Line[LLine] = ']') then
              MarkTextAsSection(RichEdit, L, LLine)
            else
            begin
              // comments
              if Line[1] = ';' then
                MarkTextAsComment(RichEdit, L, LLine)
              else
              begin
                // parameters and their values
                C := FindFirstComment(Line);
                if C > 0 then
                  MarkTextAsComment(RichEdit, Pred(L + C), Succ(LLine - C));
                EqualPos := Pos('=', Line);
                if EqualPos > 0 then
                begin
                  MarkTextAsIdent(RichEdit, L, Pred(EqualPos));
                  if C > 0 then
                    MarkTextAsValue(RichEdit, L + EqualPos, Pred(C - EqualPos))
                  else
                    MarkTextAsValue(RichEdit, L + EqualPos, LLine - EqualPos);
                end;
              end;
            end;
          end;
          // add 2 more for CR and LF chars
          {$ifdef Compiler12_up}
            L := L + LLine + 1;
          {$else}
            L := L + LLine + 2;
          {$endif}
        end;
      finally
        sl.Free;
      end;
    finally
      SendMessage(RichEdit.Handle, EM_SETEVENTMASK, 0, EventMask);
      RichEdit.Perform(WM_SETREDRAW, 1, 0);
      RichEdit.Invalidate;
    end;
  finally
    Screen.Cursor := crDefault;
  end;
  // Be nice and put the cursor at the beginning of the INI-file.
  RichEdit.SelStart  := 0;
  RichEdit.SelLength := 0;
end;

end.
 