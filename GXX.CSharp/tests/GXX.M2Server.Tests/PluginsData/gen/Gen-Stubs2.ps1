# ASCII-only generator: emit PluginInterfaceHost.Stubs.g.cs for the routines that are still stubs.
param(
  [Parameter(Mandatory=$true)][string]$Scratch,
  [Parameter(Mandatory=$true)][string]$OutFile,
  [Parameter(Mandatory=$true)][string]$PortedList
)
$Utf8NoBom = New-Object System.Text.UTF8Encoding($false)
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
  'PAnsiChar','byte[]','PChar','byte[]','TObject','object'
)
$direct = NewMap @(
  'TList','IListHandle','TStringList','IStringListHandle','TMenuItem','IMenuItem','TIniFile','IIniFileHandle',
  'TMemoryStream','IMemoryStreamHandle','TEnvirnoment','IEnvirnoment','TBaseObject','IBaseObjectHandle',
  'TSmartObject','ISmartObjectHandle','TPlayObject','IPlayObjectHandle','TDummyObject','IDummyObjectHandle',
  'THeroObject','IHeroObjectHandle','TNormNpc','INormNpcHandle','TGuild','IGuildHandle','TMagicACList','IMagicACListHandle',
  'TNotifyEventEx','TNotifyEventEx','TObject','object'
)
$rectypes = NewMap @(
  'pTAbility','ref TAbility','pTUserItem','ref TUserItem','pTStdItem','ref TStdItem','pTUserMagic','ref TUserMagic',
  'pTActorIcon','ref TActorIcon','pTAbilityNG','ref TAbilityNG','pTAbilityAlcohol','ref TAbilityAlcohol',
  'pTDefaultMessage','ref TDefaultMessage','pTMagic','ref TMagic','PScriptCmdParam','ref TScriptCmdParam',
  'PSystemTime','IntPtr'
)
$reserved = '^(?i)(Type|Params|Name|Object|String|Delegate|Event|Base|Ref|Out|In|Lock|Checked|Index|Count|Item|Value|Size|Stream|List|Menu|Guild|Player|Hero|Sender|Pointer|Array|Read|Write|Get|Set|Add)$'

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
function MapType([string]$delphi) {
  $t = $delphi.Trim()
  $mod = ''
  if ($t -match '(?i)^out\s+(.+)$') { $mod = 'out '; $t = $Matches[1].Trim() }
  elseif ($t -match '(?i)^var\s+(.+)$') { $mod = 'ref '; $t = $Matches[1].Trim() }
  if ($t.StartsWith('_')) { $t = $t.Substring(1) }
  if ($rectypes.ContainsKey($t)) { return $rectypes[$t] }
  if ($direct.ContainsKey($t)) {
    $cs = $direct[$t]
    if ($mod -eq '' -or $cs -eq 'object' -or $cs -eq 'byte[]') { return $cs }
    return $mod + $cs
  }
  if ($simple.ContainsKey($t)) {
    $cs = $simple[$t]
    if ($mod -eq '') { return $cs }
    return $mod + $cs
  }
  return 'IntPtr'
}

$ported = New-Object 'System.Collections.Generic.HashSet[string]' ([System.StringComparer]::Ordinal)
foreach ($l in ([System.IO.File]::ReadAllLines($PortedList, [System.Text.Encoding]::UTF8))) {
  $x = $l.Trim(); if ($x -ne '' -and -not $x.StartsWith('#')) { [void]$ported.Add($x) }
}

$lines = [System.IO.File]::ReadAllLines((Join-Path $Scratch 'iface_manifest.tsv'), [System.Text.Encoding]::UTF8)
$out = New-Object System.Collections.Generic.List[string]
$out.Add('using System;')
$out.Add('using System.Collections.Generic;')
$out.Add('using GXX.Core.Protocol;')
$out.Add('')
$out.Add('namespace GXX.M2Server.Plugins;')
$out.Add('')
$out.Add('/// <summary>STUBS_HEADER</summary>')
$out.Add('public sealed partial class PluginInterfaceHost')
$out.Add('{')

$n = 0
foreach ($l in $lines) {
  if ($l.Trim() -eq '') { continue }
  $f = $l -split "`t"
  $delphiName = $f[0]; $line = $f[1]; $text = $f[2]
  # iface_manifest.tsv already stores the sanitized name (leading '_' stripped)
  $mname = $delphiName
  if ($ported.Contains($mname)) { continue }

  $s = $text -replace '(?i)\bstdcall\s*;.*$', ''
  $s = $s.Trim().TrimEnd(';').Trim()
  $i = $s.IndexOf('=')
  $rest = ($s.Substring($i + 1)).Trim()
  $isFunc = $false
  if ($rest -match '^(?i)function\b') { $isFunc = $true }
  $rest = ($rest -replace '^(?i)(function|procedure)\b', '').Trim()
  $close = CloseParen $rest
  $plist = New-Object System.Collections.Generic.List[string]
  $mods = New-Object System.Collections.Generic.List[object]
  foreach ($seg in (SplitTop ($rest.Substring(1, $close - 1)))) {
    $sg = $seg.Trim(); if ($sg -eq '') { continue }
    $colon = $sg.IndexOf(':')
    $namesRaw = ($sg.Substring(0, $colon)).Trim()
    $typeRaw = ($sg.Substring($colon + 1)).Trim()
    $mod = ''
    if ($namesRaw -match '(?i)^out\s+(.+)$') { $mod = 'out '; $namesRaw = $Matches[1].Trim() }
    elseif ($namesRaw -match '(?i)^var\s+(.+)$') { $mod = 'ref '; $namesRaw = $Matches[1].Trim() }
    foreach ($nm0 in ($namesRaw -split '\s*,\s*')) {
      $nm = $nm0.Trim()
      if ($nm -match $reserved) { $nm = 'p' + $nm }
      $cs = MapType $typeRaw
      if ($mod -ne '' -and -not $cs.StartsWith('ref ') -and -not $cs.StartsWith('out ')) { $cs = $mod + $cs }
      $plist.Add(($cs + ' ' + $nm))
      $mods.Add(@{ Name = $nm; Mod = $mod })
    }
  }
  $retCs = 'void'
  if ($isFunc) {
    $retText = $rest.Substring($close + 1).Trim()
    if ($retText.StartsWith(':')) { $retText = $retText.Substring(1).Trim() }
    $retCs = MapType $retText
  }
  $out.Add('    /// <summary>STUB@sig@' + $text + '</summary>')
  $out.Add('    public ' + $retCs + ' ' + $mname + '(' + ($plist -join ', ') + ')')
  $out.Add('    {')
  foreach ($m in $mods) {
    if ($m.Mod -eq 'ref ' -or $m.Mod -eq 'out ') { $out.Add('        ' + $m.Name + ' = default!;') }
  }
  $out.Add('        throw new NotImplementedException("STUB_MSG@' + $mname + '@' + $line + '");')
  $out.Add('    }')
  $out.Add('')
  $n++
}
$out.Add('}')

[System.IO.File]::WriteAllText($OutFile, (($out -join "`r`n") + "`r`n"), $Utf8NoBom)
Write-Output ("stubs={0} ported={1}" -f $n, $ported.Count)
