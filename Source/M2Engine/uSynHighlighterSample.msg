TSynSampleSyn
tk
IdentStart '_', 'a'..'z', 'A'..'Z':: '_', '0'..'9', 'a'..'z', 'A'..'Z'::

 
TOKENTYPES
Identifier
Key Foreground=clBlue
Paragraph Foreground=$008000FF
ParameterLabels Foreground=clTeal
Comment Style=[fsItalic]|Foreground=clGreen
Jump Style=[fsUnderline, fsItalic]|Foreground=clBlue|Background=clSilver
|><|


CHARS
'A'..'Z', 'a'..'z', '_':: Ident
BeginProc
  fTokenID := IdentKind(fLine + Run);
  inc(Run, fStringLen);
  while IsIdentChar(fLine[Run]) do
    Inc(Run);
EndProc
|><|


KEYS
IF
OR
SAY
ACT
ELSEACT
ELSESAY
DEFINE
|><|


KEYS Jump
BREAK
CALL
DELAYCALL
GOTO
DELAYGOTO
|><|


ENCLOSEDBY
Comment,Line,;,#1
Paragraph,Paragraph,[@,]
ParameterLabels,ParameterLabels,<,>
|><|