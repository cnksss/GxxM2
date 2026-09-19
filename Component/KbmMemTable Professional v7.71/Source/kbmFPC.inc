// FPC Support file.

{$IFDEF FPC_FULLVERSION}
 {$IF FPC_FULLVERSION>=020604}
  {$MODE DELPHI}
  {$DEFINE _FPC}
 {$ENDIF}
 {$IFNDEF _FPC}
   FreePascal v. 2.6.4 or newer is required!
 {$ENDIF}
{$ENDIF}
