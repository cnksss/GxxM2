# ASCII-only generator: emit PluginInterfaceManaged.g.cs (I* interfaces) from iface_manifest.tsv.
param(
  [Parameter(Mandatory=$true)][string]$Scratch,
  [Parameter(Mandatory=$true)][string]$OutFile
)
$Utf8NoBom = New-Object System.Text.UTF8Encoding($false)

function NewMap([string[]]$pairs) {
  $d = New-Object 'System.Collections.Generic.Dictionary[string,string]' ([System.StringComparer]::OrdinalIgnoreCase)
  for ($i = 0; $i -lt $pairs.Count; $i += 2) { $d[$pairs[$i]] = $pairs[$i + 1] }
  return $d
}
$simple = NewMap @(
  'Integer','int','LongInt','int','LongWord','uint','Cardinal','uint','DWORD','uint',
  'Word','ushort','SmallInt','short','Byte','byte','ShortInt','sbyte','BOOL','bool','Boolean','bool',
  'Int64','long','NativeInt','IntPtr','Pointer','IntPtr','THandle','IntPtr',
  'Single','float','Double','double','Real','double','AnsiChar','byte',
  'PAnsiChar','byte[]','PChar','byte[]','TObject','object',
  'TList','IListHandle','TStringList','IStringListHandle','TMenuItem','IMenuItem',
  'TIniFile','IIniFileHandle','TMemoryStream','IMemoryStreamHandle','TEnvirnoment','IEnvirnoment',
  'TBaseObject','IBaseObjectHandle','TSmartObject','ISmartObjectHandle','TPlayObject','IPlayObjectHandle',
  'TDummyObject','IDummyObjectHandle','THeroObject','IHeroObjectHandle','TNormNpc','INormNpcHandle',
  'TGuild','IGuildHandle','TMagicACList','IMagicACListHandle','TNotifyEventEx','Action<object>',
  'PSystemTime','IntPtr','TSystemTime','TSystemTime','TMerchant','INormNpcHandle','PMagicACInfo','IntPtr',
  'TAppFuncDef','TAppFuncDef','PAppFuncDef','IntPtr'
)
$rectypes = NewMap @(
  'pTAbility','TAbility','pTUserItem','TUserItem','pTStdItem','TStdItem','pTUserMagic','TUserMagic',
  'pTActorIcon','TActorIcon','pTAbilityNG','TAbilityNG','pTAbilityAlcohol','TAbilityAlcohol',
  'pTDefaultMessage','TDefaultMessage','pTMagic','TMagic','PScriptCmdParam','TScriptCmdParam'
)
function MapType([string]$delphi, [System.Collections.Generic.HashSet[string]]$seams) {
  $t = $delphi.Trim()
  if ($t.StartsWith('_')) { $t = $t.Substring(1) }
  if ($t.StartsWith('var ')) { $t = $t.Substring(4).Trim() }
  if ($rectypes.ContainsKey($t)) { return $rectypes[$t] }
  if ($simple.ContainsKey($t)) { return $simple[$t] }
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

# delegate name -> (paramCount, paramTypeList, returnType)
$delegates = @{}
foreach ($l in ([System.IO.File]::ReadAllLines((Join-Path $Scratch 'iface_manifest.tsv'), [System.Text.Encoding]::UTF8))) {
  if ($l.Trim() -eq '') { continue }
  $f = $l -split "`t"
  $name = $f[0]
  $s = $f[2] -replace '(?i)\bstdcall\s*;.*$', ''
  $s = $s.Trim().TrimEnd(';').Trim()
  $i = $s.IndexOf('=')
  $rest = ($s.Substring($i + 1)).Trim()
  $isFunc = $false
  if ($rest -match '^(?i)function\b') { $isFunc = $true }
  $rest = ($rest -replace '^(?i)(function|procedure)\b', '').Trim()
  $close = CloseParen $rest
  $params = New-Object System.Collections.Generic.List[object]
  if ($close -ge 0) {
    $pt = $rest.Substring(1, $close - 1)
    foreach ($p in (SplitTop $pt)) {
      $seg = $p.Trim(); if ($seg -eq '') { continue }
      $colon = $seg.IndexOf(':')
      $namesRaw = ($seg.Substring(0, $colon) -replace '(?i)\bvar\b', '').Trim()
      $typeRaw = ($seg.Substring($colon + 1) -replace '(?i)\bvar\b', '').Trim()
      foreach ($n in ($namesRaw -split '\s*,\s*')) { $params.Add(@{ Name = $n.Trim(); DelphiType = $typeRaw }) }
    }
  }
  $ret = 'void'
  if ($isFunc) {
    $rt = $rest.Substring($close + 1).Trim()
    if ($rt.StartsWith(':')) { $rt = $rt.Substring(1).Trim() }
    $ret = $rt
  }
  $delegates[$name] = @{ Params = $params; Ret = $ret; IsFunc = $isFunc }
}

# Two function pointers that the original never declares but T*Func records reference
# (signatures taken from the bodies in PluginImplement.pas).
$delegates['TM2Engine_GetOtherFileDir'] = @{
  Params = @(@{ Name = 'M2FileType'; DelphiType = 'Integer' }, @{ Name = 'Dest'; DelphiType = 'PAnsiChar' }, @{ Name = 'DestLen'; DelphiType = 'DWORD' })
  Ret = 'BOOL'; IsFunc = $true
}
$delegates['TBaseObject_TrainSkill'] = @{
  Params = @(@{ Name = 'BaseObject'; DelphiType = 'TBaseObject' }, @{ Name = 'UserMagic'; DelphiType = 'pTUserMagic' }, @{ Name = 'nTranPoint'; DelphiType = 'Integer' }, @{ Name = 'IsDoCheck'; DelphiType = 'BOOL' })
  Ret = 'BOOL'; IsFunc = $true
}

$fields = @{}
foreach ($l in ([System.IO.File]::ReadAllLines((Join-Path $Scratch 'fields.tsv'), [System.Text.Encoding]::UTF8))) {
  if ($l.Trim() -eq '') { continue }
  $f = $l -split "`t"
  if (-not $fields.ContainsKey($f[0])) { $fields[$f[0]] = New-Object System.Collections.Generic.List[object] }
  $fields[$f[0]].Add(@{ Order = [int]$f[1]; Line = [int]$f[2]; Name = $f[3]; Type = $f[4]; Count = [int]$f[5] })
}

$ifaceOf = @{
  'TMemoryFunc' = 'IMemoryFunc'; 'TListFunc' = 'IListFunc'; 'TStringListFunc' = 'IStringListFunc'
  'TMemoryStreamFunc' = 'IMemoryStreamFunc'; 'TMemuFunc' = 'IMemuFunc'; 'TIniFileFunc' = 'IIniFileFunc'
  'TMagicACListFunc' = 'IMagicACListFunc'; 'TMapManagerFunc' = 'IMapManagerFunc'
  'TEnvirnomentFunc' = 'IEnvirnomentFunc'; 'TM2EngineFunc' = 'IM2EngineFunc'
  'TBaseObjectFunc' = 'IBaseObjectFunc'; 'TSmartObjectFunc' = 'ISmartObjectFunc'
  'TPlayObjectFunc' = 'IPlayObjectFunc'; 'TDummyObjectFunc' = 'IDummyObjectFunc'
  'THeroObjectFunc' = 'IHeroObjectFunc'; 'TNormNpcFunc' = 'INormNpcFunc'
  'TUserEngineFunc' = 'IUserEngineFunc'; 'TGuildManagerFunc' = 'IGuildManagerFunc'
  'TGuildFunc' = 'IGuildFunc'
}
$order = @('TMemoryFunc', 'TListFunc', 'TStringListFunc', 'TMemoryStreamFunc', 'TMemuFunc', 'TIniFileFunc',
  'TMagicACListFunc', 'TMapManagerFunc', 'TEnvirnomentFunc', 'TM2EngineFunc', 'TBaseObjectFunc',
  'TSmartObjectFunc', 'TPlayObjectFunc', 'TDummyObjectFunc', 'THeroObjectFunc', 'TNormNpcFunc',
  'TUserEngineFunc', 'TGuildManagerFunc', 'TGuildFunc')

$reservedNames = '^(Type|Params|Name|Object|String|Delegate|Event|Base|Ref|Out|In|Lock|Checked|Index|Count|Item|Value|Size|Stream|List|Menu|Guild|Player|Hero|Sender|Pointer|Array|Read|Write|Get|Set|Add)$'
$seams = New-Object 'System.Collections.Generic.HashSet[string]'
$out = New-Object System.Collections.Generic.List[string]
$out.Add('using System;')
$out.Add('using System.Collections.Generic;')
$out.Add('using GXX.Core.Protocol;')
$out.Add('')
$out.Add('namespace GXX.M2Server.Plugins;')
$out.Add('')

foreach ($rn in $order) {
  $iname = $ifaceOf[$rn]
  $out.Add('/// <summary>MANAGED@' + $rn + '@' + $iname + '@' + $fields[$rn].Count + '</summary>')
  $out.Add('public interface ' + $iname)
  $out.Add('{')
  foreach ($fl in $fields[$rn]) {
    if ($fl.Type -like 'array of *' -or $fl.Type -eq 'Pointer') {
      # Reserved 占位数组：托管接口以 IntPtr 属性表达（原文语义即"保留槽位"）
      $out.Add('    IntPtr ' + $fl.Name + ' { get; set; }        // FIELD@' + $fl.Line + '@' + $fl.Name + '@' + $fl.Type + '@' + $fl.Count)
      continue
    }
    if (-not $delegates.ContainsKey($fl.Type)) { throw ("no delegate for field type " + $fl.Type + " in " + $rn) }
    $d = $delegates[$fl.Type]
    $plist = New-Object System.Collections.Generic.List[string]
    foreach ($q in $d.Params) {
      $cs = MapType $q.DelphiType $seams
      $nm = $q.Name
      if ($nm -match $reservedNames) { $nm = 'p' + $nm }
      $plist.Add(($cs + ' ' + $nm))
    }
    $rt = 'void'
    if ($d.IsFunc) { $rt = MapType $d.Ret $seams }
    $out.Add('    ' + $rt + ' ' + $fl.Name + '(' + ($plist -join ', ') + ');        // FIELD@' + $fl.Line + '@' + $fl.Name + '@' + $fl.Type)
  }
  $out.Add('}')
  $out.Add('')
}

[System.IO.File]::WriteAllText($OutFile, (($out -join "`r`n") + "`r`n"), $Utf8NoBom)
Write-Output ("interfaces={0} members={1} seams={2}" -f $order.Count, (($fields.Values | ForEach-Object { $_.Count }) | Measure-Object -Sum).Sum, $seams.Count)
if ($seams.Count -gt 0) { Write-Output ("seams: " + (($seams | Sort-Object) -join ', ')) }
