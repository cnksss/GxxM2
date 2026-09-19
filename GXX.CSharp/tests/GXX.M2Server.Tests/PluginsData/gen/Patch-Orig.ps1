# ASCII-only patcher for the generated ABI files.
#   -File <cs>  -Kind types|tables  -Manifest <tsv>  -Header <txt>
# Chinese dynamic text is assembled from code points so this file stays pure ASCII.
param(
  [Parameter(Mandatory=$true)][string]$File,
  [Parameter(Mandatory=$true)][ValidateSet('types', 'tables', 'managed')][string]$Kind,
  [Parameter(Mandatory=$true)][string]$Manifest,
  [Parameter(Mandatory=$true)][string]$Header
)
$Utf8NoBom = New-Object System.Text.UTF8Encoding($false)
$up = [string][char]0x539F + [string][char]0x6587
$lp = [string][char]0xFF08
$rp = [string][char]0xFF09
$pd = [string][char]0x3002
$tick = [string][char]0x0060
$orig = [string][char]0x539F
$colon = [string][char]0x003A
$comma = [string][char]0x002C

$sigMap = @{}
foreach ($l in ([System.IO.File]::ReadAllLines($Manifest, [System.Text.Encoding]::UTF8))) {
  if ($l.Trim() -eq '') { continue }
  $f = $l -split "`t"
  if ($f.Count -lt 3) { continue }
  $sigMap[$f[1]] = $f[2]
}
$script:sigMap = $sigMap
$script:up = $up; $script:lp = $lp; $script:rp = $rp; $script:pd = $pd; $script:tick = $tick
$script:orig = $orig; $script:colon = $colon; $script:comma = $comma

$src = [System.IO.File]::ReadAllText($File, [System.Text.Encoding]::UTF8)

$src = [regex]::Replace($src, '/// <summary>ORIG_LINE_(\d+)</summary>', {
    param($m)
    $ln = $m.Groups[1].Value
    if (-not $script:sigMap.ContainsKey($ln)) { return $m.Value }
    return '/// <summary>' + $script:up + ' ' + $script:tick + $script:sigMap[$ln] + $script:tick + $script:lp +
      'PluginInterface.pas:' + $ln + $script:rp + $script:pd + '</summary>'
  })

$src = [regex]::Replace($src, '/// <summary>RECORD@([A-Za-z0-9_]+)@(\d+)@(\d+)@(\d+)</summary>', {
    param($m)
    return '/// <summary>' + $script:up + ' ' + $m.Groups[1].Value + $script:lp + 'PluginInterface.pas:' +
      $m.Groups[2].Value + '-' + $m.Groups[3].Value + $script:comma + $m.Groups[4].Value +
      ' fields' + $script:rp + $script:pd + '</summary>'
  })

$src = [regex]::Replace($src, '// FIELD@(\d+)@([A-Za-z0-9_]+)@([^@\r\n]+?)(?:@(-?\d+))?\r?$', {
    param($m)
    $ct = 1
    if ($m.Groups[4].Success -and $m.Groups[4].Value -ne '') { $ct = [int]$m.Groups[4].Value }
    $extra = ''
    if ($ct -gt 1) { $extra = ' x' + $ct }
    return '// ' + $script:orig + ' ' + $m.Groups[1].Value + '  ' + $m.Groups[2].Value + $script:colon +
      ' ' + $m.Groups[3].Value + $extra
  }, [System.Text.RegularExpressions.RegexOptions]::Multiline)

$src = [regex]::Replace($src, '/// <summary>NOTIFY_EVENT_EX@(\d+)</summary>', {
    param($m)
# [zh]     # "?? `TNotifyEventEx = procedure(Sender: _TObject); stdcall`?PluginInterface.pas:114??
# [zh]     #  ????? `X = function(...)` ???????? 736 ?????????"
    return '/// <summary>' + $script:up + ' ' + $script:tick + 'TNotifyEventEx = procedure(Sender: _TObject); stdcall' + $script:tick +
      $script:lp + 'PluginInterface.pas:' + $m.Groups[1].Value + $script:rp + [char]0xFF1B +
      [char]0x8BE5 + [char]0x7C7B + [char]0x578B + [char]0x4E0D + [char]0x662F + ' ' + $script:tick + 'X = function(...)' + $script:tick +
      ' ' + [char]0x5F62 + [char]0x5F0F + [char]0xFF0C + [char]0x6545 + [char]0x4E0D + [char]0x5728 + [char]0x4E0A + [char]0x9762 + ' 736 ' +
      [char]0x6761 + [char]0x4E4B + [char]0x5217 + [char]0xFF0C + [char]0x624B + [char]0x5DE5 + [char]0x8865 + [char]0x9F50 + $script:pd + '</summary>'
  })

$src = [regex]::Replace($src, '/// <summary>MANAGED@([A-Za-z0-9_]+)@([A-Za-z0-9_]+)@(\d+)</summary>', {
    param($m)
# [zh]     # "?? T*Func ??????? I*Func?N ????? T*Func ????????"
    return '/// <summary>' + $script:up + ' ' + $m.Groups[1].Value + ' ' + [char]0x5BF9 + [char]0x5E94 + [char]0x7684 +
      [char]0x6258 + [char]0x7BA1 + [char]0x63A5 + [char]0x53E3 + ' ' + $m.Groups[2].Value + $script:lp +
      $m.Groups[3].Value + ' ' + [char]0x4E2A + [char]0x6210 + [char]0x5458 + $script:rp + $script:pd + '</summary>'
  })

$src = [regex]::Replace($src, '/// <summary>MISSING_TYPE_SOURCE@(\d+)@(PluginInterface\.pas:\d+)</summary>', {
    param($m)
# [zh]     # "?????????PluginImplement.pas:<impl line> ????? <iface ref> ???"
    return '/// <summary>' + $script:up + [char]0x7F3A + [char]0x5931 + [char]0x7C7B + [char]0x578B + [char]0x58F0 + [char]0x660E +
      [char]0xFF1A + 'PluginImplement.pas:' + $m.Groups[1].Value + ' ' + [char]0x5B9E + [char]0x73B0 + [char]0x4F53 +
      [char]0xFF1B + [char]0x88AB + ' ' + $m.Groups[2].Value + ' ' + [char]0x5F15 + [char]0x7528 + $script:pd + '</summary>'
  })

$hdrLines = [System.IO.File]::ReadAllLines($Header, [System.Text.Encoding]::UTF8)
$hdrText = (($hdrLines -join "`r`n") + "`r`n")
if ($Kind -eq 'types') {
  $src = [regex]::Replace($src, '(?s)\A// =+\r?\n// PLUGIN_INTERFACE_ABI_HEADER.*?\r?\n\r?\n', ($hdrText + "`r`n"))
} elseif ($Kind -eq 'stubs') {
  # no file header insertion; the STUBS_HEADER placeholder was already replaced above
} else {
  $src = [regex]::Replace($src, '\Ausing System;', ($hdrText + "`r`n" + 'using System;'))
}
$left = ([regex]::Matches($src, 'ORIG_LINE_|FIELD@|RECORD@|PLUGIN_INTERFACE_ABI_HEADER')).Count

[System.IO.File]::WriteAllText($File, $src, $Utf8NoBom)
$left = ([regex]::Matches($src, 'ORIG_LINE_|FIELD@|RECORD@|PLUGIN_INTERFACE_ABI_HEADER')).Count
Write-Output ("patched " + (Split-Path $File -Leaf) + ": remaining placeholders = " + $left)
