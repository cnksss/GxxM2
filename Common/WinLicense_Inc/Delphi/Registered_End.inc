{$IFNDEF CPUX64}
  // REGISTERED_END macro
  asm DB $EB, $10, $57, $4c, $20, $20, $03, $00, $00, $00, $00, $00, $00, $00, $57, $4c, $20, $20 end;
{$ELSE}
  RegisteredEnd();
{$ENDIF}
