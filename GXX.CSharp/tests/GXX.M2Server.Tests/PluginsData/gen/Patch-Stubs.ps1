# ASCII-only patcher for PluginInterfaceHost.Stubs.g.cs.
param(
  [Parameter(Mandatory=$true)][string]$File,
  [Parameter(Mandatory=$true)][string]$Header
)
$Utf8NoBom = New-Object System.Text.UTF8Encoding($false)
$src = [System.IO.File]::ReadAllText($File, [System.Text.Encoding]::UTF8)
$orig = [string][char]0x539F + [string][char]0x6587
$tick = [string][char]0x0060

# 1) per-method original signature comment
$src = [regex]::Replace($src, '/// <summary>STUB@sig@([^<]*)</summary>', {
    param($m)
    return '/// <summary>' + $orig + ' ' + $tick + $m.Groups[1].Value + $tick + '</summary>'
  })

# 2) NotImplementedException message
$src = [regex]::Replace($src, 'STUB_MSG@([A-Za-z0-9_]+)@(\d+)', {
    param($m)
    return [char]0x63A5 + [char]0x7F1D + [char]0xFF1A + 'PluginInterfaceHost.' + $m.Groups[1].Value + ' ' +
      [char]0x5F85 + ' PluginImplement.pas:' + $m.Groups[2].Value + ' ' + [char]0x6240 + [char]0x5C5E +
      [char]0x5F15 + [char]0x64CE + [char]0x5355 + [char]0x5143 + [char]0x79FB + [char]0x690D + [char]0x540E +
      [char]0x63A5 + [char]0x5165
  })

# 3) file header
$hdr = [System.IO.File]::ReadAllText($Header, [System.Text.Encoding]::UTF8)
$src = [regex]::Replace($src, '/// <summary>STUBS_HEADER</summary>', $hdr.TrimEnd("`r", "`n"))

[System.IO.File]::WriteAllText($File, $src, $Utf8NoBom)
$left = ([regex]::Matches($src, 'STUB_MSG@|STUB@sig@|STUBS_HEADER')).Count
Write-Output ('patched stubs: remaining placeholders = ' + $left)
