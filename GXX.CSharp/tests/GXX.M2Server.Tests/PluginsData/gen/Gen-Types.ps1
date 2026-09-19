# ASCII-only generator: PluginInterface.pas -> PluginInterfaceTypes.g.cs (UTF-8, no BOM).
param(
  [Parameter(Mandatory=$true)][string]$Src,
  [Parameter(Mandatory=$true)][string]$OutFile,
  [Parameter(Mandatory=$true)][string]$Scratch
)

$Utf8NoBom = New-Object System.Text.UTF8Encoding($false)
function Strip([string]$t) {
  $t = [regex]::Replace($t, '\{[^}]*\}', ' ')
  $i = $t.IndexOf('//'); if ($i -ge 0) { $t = $t.Substring(0, $i) }
  return $t.Trim()
}
function NewMap([string[]]$pairs) {
  $d = New-Object 'System.Collections.Generic.Dictionary[string,string]' ([System.StringComparer]::OrdinalIgnoreCase)
  for ($i = 0; $i -lt $pairs.Count; $i += 2) { $d[$pairs[$i]] = $pairs[$i + 1] }
  return $d
}

$simple = NewMap @(
  'Integer','int','LongInt','int','LongWord','uint','Cardinal','uint','DWORD','uint',
  'Word','ushort','SmallInt','short','Byte','byte','ShortInt','sbyte','BOOL','int','Boolean','bool',
  'Int64','long','NativeInt','IntPtr','Pointer','IntPtr','THandle','IntPtr',
  'Single','float','Double','double','Real','double','AnsiChar','byte',
  'PAnsiChar','byte[]','PChar','byte[]','TObject','object',
  'TList','IListHandle','TStringList','IStringListHandle','TMenuItem','IMenuItem',
  'TIniFile','IIniFileHandle','TMemoryStream','IMemoryStreamHandle','TEnvirnoment','IEnvirnoment',
  'TBaseObject','IBaseObjectHandle','TSmartObject','ISmartObjectHandle','TPlayObject','IPlayObjectHandle',
  'TDummyObject','IDummyObjectHandle','THeroObject','IHeroObjectHandle','TNormNpc','INormNpcHandle',
  'TGuild','IGuildHandle','TMagicACList','IMagicACListHandle','TNotifyEventEx','TNotifyEventEx',
  'PSystemTime','IntPtr','TSystemTime','TSystemTime','TMerchant','INormNpcHandle','PMagicACInfo','IntPtr',
  'TAppFuncDef','TAppFuncDef','PAppFuncDef','IntPtr'
)
$rectypes = NewMap @(
  'pTAbility','ref TAbility','pTUserItem','ref TUserItem','pTStdItem','ref TStdItem',
  'pTUserMagic','ref TUserMagic','pTActorIcon','ref TActorIcon','pTAbilityNG','ref TAbilityNG',
  'pTAbilityAlcohol','ref TAbilityAlcohol','pTDefaultMessage','ref TDefaultMessage','pTMagic','ref TMagic',
  'PScriptCmdParam','ref TScriptCmdParam'
)
$retmap = NewMap @(
  'Pointer','IntPtr','PAnsiChar','IntPtr','THandle','IntPtr','NativeInt','IntPtr',
  'BOOL','int','Boolean','bool','Real','double','Single','float','Int64','long','Integer','int',
  'DWORD','uint','Word','ushort','SmallInt','short','Byte','byte','ShortInt','sbyte','AnsiChar','byte',
  'TList','IListHandle','TStringList','IStringListHandle','TMenuItem','IMenuItem','TIniFile','IIniFileHandle',
  'TMemoryStream','IMemoryStreamHandle','TEnvirnoment','IEnvirnoment','TBaseObject','IBaseObjectHandle',
  'TSmartObject','ISmartObjectHandle','TPlayObject','IPlayObjectHandle','TDummyObject','IDummyObjectHandle',
  'THeroObject','IHeroObjectHandle','TNormNpc','INormNpcHandle','TGuild','IGuildHandle',
  'TMagicACList','IMagicACListHandle','TAbility','TAbility','TUserItem','TUserItem','TStdItem','TStdItem',
  'TDefaultMessage','TDefaultMessage','PSystemTime','IntPtr','PMagicACInfo','IntPtr','TMerchant','INormNpcHandle'
)

function MapType([string]$delphi, [System.Collections.Generic.HashSet[string]]$seams) {
  $t = $delphi.Trim()
# [zh]   # Delphi "var X: T" / "out X: T" -> C# ref/out parameter (?????????)
  $prefix = ''
  $outFound = $false
  if ($t -match '(?i)^out\s+(.+)$') { $prefix = 'out '; $t = $Matches[1].Trim() }
  elseif ($t -match '(?i)^var\s+(.+)$') { $prefix = 'ref '; $t = $Matches[1].Trim() }
  if ($t.StartsWith('_')) { $t = $t.Substring(1) }
  if ($rectypes.ContainsKey($t)) {
    if ($prefix -eq '') { return $rectypes[$t] }
    # ref T* : the record pointer itself is the out parameter
    return $rectypes[$t]
  }
  if ($simple.ContainsKey($t)) {
    if ($prefix -eq '') { return $simple[$t] }
    $cs = $simple[$t]
    if ($cs -eq 'int' -and $t -eq 'BOOL') { return $prefix + 'int' }
    return $prefix + $cs
  }
  [void]$seams.Add($t)
  return ('TSeam_' + $t)
}
function SplitTop([string]$s) {
  $parts = New-Object System.Collections.Generic.List[string]
  $depth = 0; $cur = ''
  foreach ($c in $s.ToCharArray()) {
    if ($c -eq '(') { $depth++ }
    if ($c -eq ')') { $depth-- }
    if ($c -eq ';' -and $depth -eq 0) { $parts.Add($cur); $cur = ''; continue }
    $cur += $c
  }
  if ($cur.Trim() -ne '') { $parts.Add($cur) }
  return $parts
}
function CloseParen([string]$s) {
  $depth = 0
  for ($k = 0; $k -lt $s.Length; $k++) {
    $c = $s[$k]
    if ($c -eq '(') { $depth++ } elseif ($c -eq ')') { $depth--; if ($depth -eq 0) { return $k } }
  }
  return -1
}
function ParseParams([string]$paramsText) {
  $out = New-Object System.Collections.Generic.List[object]
  if ($paramsText.Trim() -eq '') { return $out }
  foreach ($p in (SplitTop $paramsText)) {
    $seg = $p.Trim(); if ($seg -eq '') { continue }
    $colon = $seg.IndexOf(':')
    if ($colon -lt 0) { throw "no colon in param segment: $seg" }
    $namesRaw = ($seg.Substring(0, $colon)).Trim()
    $typeRaw = ($seg.Substring($colon + 1)).Trim()
# [zh]     # Delphi ? var/out ??????????? "var DestLen: DWORD"?????????
    $mod = ''
    if ($namesRaw -match '(?i)^out\s+(.+)$') { $mod = 'out '; $namesRaw = $Matches[1].Trim() }
    elseif ($namesRaw -match '(?i)^var\s+(.+)$') { $mod = 'ref '; $namesRaw = $Matches[1].Trim() }
    foreach ($n in ($namesRaw -split '\s*,\s*')) { $out.Add(@{ Name = $n.Trim(); DelphiType = $typeRaw; Mod = $mod }) }
  }
  return $out
}

$ifaceLines = Get-Content -LiteralPath (Join-Path $Src 'PluginInterface.pas') -Encoding Default
$acc = ''; $start = 0
$iface = New-Object System.Collections.Generic.List[object]
for ($i = 0; $i -lt $ifaceLines.Count; $i++) {
  $t = Strip $ifaceLines[$i]
  if ($t -eq '') { continue }
  if ($acc -eq '') { $start = $i + 1 }
  $acc = if ($acc -eq '') { $t } else { $acc + ' ' + $t }
  if ($acc -match '^([A-Za-z_][A-Za-z0-9_]*)\s*=\s*(function|procedure)\b') {
    $depth = 0
    for ($k = 0; $k -lt $acc.Length; $k++) { if ($acc[$k] -eq '(') { $depth++ } elseif ($acc[$k] -eq ')') { $depth-- } }
    if ($depth -eq 0 -and $acc -match '(?i)\bstdcall\s*;\s*$') { $iface.Add(@{ Line = $start; Text = ($acc -replace '\s+', ' ') }); $acc = '' }
  } else { if ($t.EndsWith(';')) { $acc = '' } }
}

$seams = New-Object 'System.Collections.Generic.HashSet[string]'
$decls = New-Object System.Collections.Generic.List[string]
$manifest = New-Object System.Collections.Generic.List[string]
$n = 0
foreach ($e in $iface) {
  $s = $e.Text -replace '(?i)\bstdcall\s*;.*$', ''
  $s = $s.Trim().TrimEnd(';').Trim()
  $i = $s.IndexOf('=')
  $name = $s.Substring(0, $i).Trim()
  $rest = ($s.Substring($i + 1)).Trim()
  $isFunc = $false
  if ($rest -match '^(?i)function\b') { $isFunc = $true }
  $rest = ($rest -replace '^(?i)(function|procedure)\b', '').Trim()
  $close = CloseParen $rest
  if ($close -lt 0) { throw "unbalanced parens: $($e.Text)" }
  $plist = New-Object System.Collections.Generic.List[string]
  foreach ($q in (ParseParams ($rest.Substring(1, $close - 1)))) {
    $cs = MapType $q.DelphiType $seams
    if ($q.Mod -ne '' -and -not $cs.StartsWith('ref ') -and -not $cs.StartsWith('out ')) { $cs = $q.Mod + $cs }
    $nm = $q.Name
    if ($nm -match '^(?i)(Type|Params|Name|Object|String|Delegate|Event|Base|Ref|Out|In|Lock|Checked|Index|Count|Item|Value|Size|Stream|List|Menu|Guild|Player|Hero|Sender|Pointer|Array|Read|Write|Get|Set|Add)$') { $nm = 'p' + $nm }
    $plist.Add(($cs + ' ' + $nm))
  }
  $retCs = 'void'
  if ($isFunc) {
    $retText = $rest.Substring($close + 1).Trim()
    if ($retText.StartsWith(':')) { $retText = $retText.Substring(1).Trim() }
    $retCs = MapType $retText $seams
  }
  $decls.Add("/// <summary>ORIG_LINE_" + $e.Line + "</summary>")
  $decls.Add("[UnmanagedFunctionPointer(CallingConvention.StdCall)]")
  $decls.Add("public delegate " + $retCs + " " + $name + "(" + ($plist -join ', ') + ");")
  $decls.Add("")
  $manifest.Add(("{0}`t{1}`t{2}" -f $name, $e.Line, $e.Text))
  $n++
}

$head = @(
  '// =====================================================================================',
  '// PLUGIN_INTERFACE_ABI_HEADER',
  '// =====================================================================================',
  '',
  'using System;',
  'using System.Runtime.InteropServices;',
  'using GXX.Core.Protocol;',
  '',
  'namespace GXX.M2Server.Plugins;',
  '',
  '/// <summary>NOTIFY_EVENT_EX@114</summary>',
  '[UnmanagedFunctionPointer(CallingConvention.StdCall)]',
  'public delegate void TNotifyEventEx(object? Sender);',
  ''
)
# [zh] # ????????????? T*Func ?????:2510 / :2697??? PluginInterface.pas ???
# [zh] # ??? procedural type?PluginImplement.pas ????? :1852 / :3197?????????????
# The next two function pointers are referenced by T*Func records (:2510 / :2697) but
# PluginInterface.pas never declares their procedural types (PluginImplement.pas does have
# bodies at :1852 / :3197). Signatures below are taken from those bodies.
$extraDecls = New-Object System.Collections.Generic.List[string]
$extraDecls.Add('/// <summary>MISSING_TYPE_SOURCE@1852@PluginInterface.pas:2510</summary>')
$extraDecls.Add('[UnmanagedFunctionPointer(CallingConvention.StdCall)]')
$extraDecls.Add('public delegate int TM2Engine_GetOtherFileDir(int M2FileType, byte[] Dest, ref uint DestLen);')
$extraDecls.Add('')
$extraDecls.Add('/// <summary>MISSING_TYPE_SOURCE@3197@PluginInterface.pas:2697</summary>')
$extraDecls.Add('[UnmanagedFunctionPointer(CallingConvention.StdCall)]')
$extraDecls.Add('public delegate int TBaseObject_TrainSkill(IBaseObjectHandle BaseObject, ref TUserMagic UserMagic, int nTranPoint, int IsDoCheck);')
$extraDecls.Add('')
[System.IO.File]::WriteAllText($OutFile, (($head + $decls.ToArray() + $extraDecls.ToArray()) -join "`r`n") + "`r`n", $Utf8NoBom)
[System.IO.File]::WriteAllText((Join-Path $Scratch 'iface_manifest.tsv'), (($manifest -join "`n") + "`n"), $Utf8NoBom)
Write-Output ("proctypes={0} seams={1}" -f $n, $seams.Count)
Write-Output ("seams: " + (($seams | Sort-Object) -join ', '))
