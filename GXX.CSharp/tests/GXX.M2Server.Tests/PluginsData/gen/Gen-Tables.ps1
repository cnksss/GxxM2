# ASCII-only generator: emit PluginInterfaceTables.g.cs (Packed=1 structs) for PluginInterface.pas.
# Sources: records.tsv / fields.tsv / iface_manifest.tsv produced by the sibling scripts.
param(
  [Parameter(Mandatory=$true)][string]$Scratch,
  [Parameter(Mandatory=$true)][string]$OutFile
)
$Utf8NoBom = New-Object System.Text.UTF8Encoding($false)

$fields = @{}
foreach ($l in ([System.IO.File]::ReadAllLines((Join-Path $Scratch 'fields.tsv'), [System.Text.Encoding]::UTF8))) {
  if ($l.Trim() -eq '') { continue }
  $f = $l -split "`t"
  if (-not $fields.ContainsKey($f[0])) { $fields[$f[0]] = New-Object System.Collections.Generic.List[object] }
  $fields[$f[0]].Add(@{ Order = [int]$f[1]; Line = [int]$f[2]; Name = $f[3]; Type = $f[4]; Count = [int]$f[5] })
}
$recs = @{}
foreach ($l in ([System.IO.File]::ReadAllLines((Join-Path $Scratch 'records.tsv'), [System.Text.Encoding]::UTF8))) {
  if ($l.Trim() -eq '') { continue }
  $f = $l -split "`t"
  $recs[$f[0]] = @{ Start = [int]$f[1]; End = [int]$f[2]; Fields = [int]$f[3] }
}

$ptype = @{}
$ptype['Pointer'] = 'IntPtr'; $ptype['PAnsiChar'] = 'IntPtr'; $ptype['THandle'] = 'IntPtr'
$ptype['Integer'] = 'int'; $ptype['DWORD'] = 'uint'; $ptype['Word'] = 'ushort'; $ptype['Byte'] = 'byte'
$ptype['ShortInt'] = 'sbyte'; $ptype['SmallInt'] = 'short'; $ptype['Int64'] = 'long'; $ptype['BOOL'] = 'int'
$ptype['NativeInt'] = 'IntPtr'; $ptype['Real'] = 'double'; $ptype['PSystemTime'] = 'IntPtr'
$ptype['TObject'] = 'object'; $ptype['TNotifyEventEx'] = 'TNotifyEventEx'; $ptype['PWideChar'] = 'IntPtr'
foreach ($pair in @(
    @('_TList', 'IListHandle'), @('_TStringList', 'IStringListHandle'), @('_TMenuItem', 'IMenuItem'),
    @('_TIniFile', 'IIniFileHandle'), @('_TMemoryStream', 'IMemoryStreamHandle'), @('_TMagicACList', 'IMagicACListHandle'),
    @('_TEnvirnoment', 'IEnvirnoment'), @('_TBaseObject', 'IBaseObjectHandle'), @('_TSmartObject', 'ISmartObjectHandle'),
    @('_TPlayObject', 'IPlayObjectHandle'), @('_TDummyObject', 'IDummyObjectHandle'), @('_THeroObject', 'IHeroObjectHandle'),
    @('_TNormNpc', 'INormNpcHandle'), @('_TGuild', 'IGuildHandle'))) { $ptype[$pair[0]] = $pair[1] }

$fntype = @{}
foreach ($l in ([System.IO.File]::ReadAllLines((Join-Path $Scratch 'iface_manifest.tsv'), [System.Text.Encoding]::UTF8))) {
  if ($l.Trim() -eq '') { continue }
  $f = $l -split "`t"
  $fntype[$f[0]] = $true
}
# the two function pointers referenced by T*Func records but never declared in PluginInterface.pas
# (declarations are added by Gen-Types.ps1 from the PluginImplement.pas bodies)
$fntype['TM2Engine_GetOtherFileDir'] = $true
$fntype['TBaseObject_TrainSkill'] = $true
$fntype['TNotifyEventEx'] = $true
# [zh] # records embedded as fields (??? T*Func ?????? TAppFuncDef)
$recordNames = @{}
foreach ($l in ([System.IO.File]::ReadAllLines((Join-Path $Scratch 'records.tsv'), [System.Text.Encoding]::UTF8))) {
  if ($l.Trim() -eq '') { continue }
  $recordNames[($l -split "`t")[0]] = $true
}

$order = @(
  'TScriptCmdParam', 'TMemoryFunc', 'TListFunc', 'TStringListFunc', 'TMemoryStreamFunc', 'TMemuFunc',
  'TIniFileFunc', 'TMagicACListFunc', 'TMapManagerFunc', 'TEnvirnomentFunc', 'TM2EngineFunc',
  'TBaseObjectFunc', 'TSmartObjectFunc', 'TPlayObjectFunc', 'TDummyObjectFunc', 'THeroObjectFunc',
  'TNormNpcFunc', 'TUserEngineFunc', 'TGuildManagerFunc', 'TGuildFunc', 'TAppFuncDef'
)

$out = New-Object System.Collections.Generic.List[string]
$out.Add('using System;')
$out.Add('using System.Runtime.InteropServices;')
$out.Add('using GXX.Core.Protocol;')
$out.Add('')
$out.Add('namespace GXX.M2Server.Plugins;')
$out.Add('')

$seams = New-Object 'System.Collections.Generic.HashSet[string]'
foreach ($rn in $order) {
  $r = $recs[$rn]
  $out.Add('/// <summary>RECORD@' + $rn + '@' + $r.Start + '@' + $r.End + '@' + $r.Fields + '</summary>')
  $out.Add('[StructLayout(LayoutKind.Sequential, Pack = 1)]')
  $out.Add('public unsafe struct ' + $rn)
  $out.Add('{')
  foreach ($fl in $fields[$rn]) {
    $tn = $fl.Type
    $cs = $null
    if ($tn -like 'array of *') {
      $cs = 'fixed long ' + $fl.Name + '[' + $fl.Count + ']'
    } elseif ($fntype.ContainsKey($tn)) {
      $cs = $tn + ' ' + $fl.Name
    } elseif ($recordNames.ContainsKey($tn)) {
      $cs = $tn + ' ' + $fl.Name
    } elseif ($ptype.ContainsKey($tn)) {
      $cs = $ptype[$tn] + ' ' + $fl.Name
    } else {
      [void]$seams.Add($tn)
      $cs = 'TSeam_' + $tn + ' ' + $fl.Name
    }
    $out.Add('    public ' + $cs + ';        // FIELD@' + $fl.Line + '@' + $fl.Name + '@' + $fl.Type + '@' + $fl.Count)
  }
  $out.Add('}')
  $out.Add('')
}

[System.IO.File]::WriteAllText($OutFile, (($out -join "`r`n") + "`r`n"), $Utf8NoBom)
Write-Output ("records emitted = {0}; unmapped field types = {1}" -f $order.Count, $seams.Count)
if ($seams.Count -gt 0) { Write-Output ("seams: " + (($seams | Sort-Object) -join ', ')) }
