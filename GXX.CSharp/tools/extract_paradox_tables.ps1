# ParadoxConv.pas -> C# table extraction (final)
$ErrorActionPreference = 'Stop'
$root = 'D:\chuanqi\daima\GXX原版_Delphi7'
$src  = Join-Path $root 'Source\RunGate\ParadoxConv.pas'
$lines = [System.IO.File]::ReadAllLines($src, [System.Text.Encoding]::GetEncoding(936))

$RE_FUNC  = '^function\s+{0}\s*\('
$RE_BEGIN = '^begin\b'
$RE_END   = '^\s*end\s*;?\s*$'
$RE_ELSE  = '^\s*(?i:else)\s*$'
$RE_OF    = '^\s*(?i:of)\s*$'
$RE_CASE1 = '^\s*(?i:case)\s+S\[I\]\s*(?:of)?\s*$'
$RE_CASE2 = '^\s*(?i:case)\s+S\[I\s*\+\s*2\]\s*(?:of)?\s*$'
$RE_CASEK = '^\s*(?i:case)\s'
$RE_BEGK  = '^\s*(?i:begin)\s*$'
$RE_HI    = '^\s*#\$([0-9A-Fa-f]{2})\s*:\s*$'
$RE_ENTRY = '^\s*(?:#\$([0-9A-Fa-f]{2}))(?:\.\.#\$([0-9A-Fa-f]{2}))?\s*:\s*Result\s*:=\s*(.+)$'

function H2([string]$s) { [Convert]::ToInt32($s, 16) }

function FindBegin([string]$name) {
  for ($i = 0; $i -lt $lines.Length; $i++) {
    if ($lines[$i] -match ($RE_FUNC -f [regex]::Escape($name))) {
      for ($j = $i + 1; $j -lt $lines.Length; $j++) { if ($lines[$j] -match $RE_BEGIN) { return $j } }
    }
  }
  throw "function $name not found"
}

# Normalized RHS (all whitespace removed) shapes:
#   Result+#$04#$02 | Result+#$00+S[I] | Result+#$04+AnsiChar(Ord(S[I])-$B0) | Result+S[I+3]
function ParseRhs([string]$rhs) {
  $t = ($rhs -replace '\s', '')
  if ($t.EndsWith(';')) { $t = $t.Substring(0, $t.Length - 1) }
  if ($t.StartsWith('Result+')) { $t = $t.Substring(7) }
  elseif ($t.StartsWith('Result')) { $t = $t.Substring(6) }
  if ($t -match '^#\$[0-9A-Fa-f]{2}#\$[0-9A-Fa-f]{2}$') {
    return @{ Hi = (H2 $t.Substring(2, 2)); Lo = (H2 $t.Substring(6, 2)) }
  }
  if ($t -match '^#\$[0-9A-Fa-f]{2}$') { return @{ Hi = -1; Lo = (H2 $t.Substring(2, 2)) } }
  if ($t -match '^#[0-9A-Fa-f]{2}$') { return @{ Hi = -1; Lo = (H2 $t.Substring(1, 2)) } }
  $hi = -1
  $mh = [regex]::Match($t, '^#\$([0-9A-Fa-f]{2})')
  if ($mh.Success) { $hi = H2 $mh.Groups[1].Value; $t = $t.Substring(4) }
  else {
    $mh2 = [regex]::Match($t, '^#([0-9A-Fa-f]{2})')
    if ($mh2.Success) { $hi = H2 $mh2.Groups[1].Value; $t = $t.Substring(3) }
  }
  if ($t.StartsWith('+')) { $t = $t.Substring(1) }
  if ($t -eq '') { throw "no low half in [$rhs]" }
  $ml = [regex]::Match($t, '^#\$([0-9A-Fa-f]{2})$')
  if ($ml.Success) { return @{ Hi = $hi; Lo = (H2 $ml.Groups[1].Value) } }
  $ml2 = [regex]::Match($t, '^#([0-9A-Fa-f]{2})$')
  if ($ml2.Success) { return @{ Hi = $hi; Lo = (H2 $ml2.Groups[1].Value) } }
  $marr = [regex]::Match($t, '^#\$([0-9A-Fa-f]{2})\+S\[I(\+\d+)?\]$')
  if (-not $marr.Success) { $marr = [regex]::Match($t, '^#([0-9A-Fa-f]{2})\+S\[I(\+\d+)?\]$') }
  if ($marr.Success) {
    $off = 0
    if ($marr.Groups[2].Success) { $off = [int]$marr.Groups[2].Value.Substring(1) }
    return @{ Hi = $hi; Lo = @{ Offset = $off; Sign = 1; Const = (H2 $marr.Groups[1].Value) } }
  }
  $m = [regex]::Match($t, '^AnsiChar\(Ord\(S\[I(\+\d+)?\]\)([+-])#?\$([0-9A-Fa-f]{2})\)$')
  if (-not $m.Success) { $m = [regex]::Match($t, '^AnsiChar\(Ord\(S\[I(\+\d+)?\]\)([+-])#([0-9A-Fa-f]{2})\)$') }
  if ($m.Success) {
    $off = 0
    if ($m.Groups[1].Success) { $off = [int]$m.Groups[1].Value.Substring(1) }
    $sign = 1
    if ($m.Groups[2].Value -eq '-') { $sign = -1 }
    return @{ Hi = $hi; Lo = @{ Offset = $off; Sign = $sign; Const = (H2 $m.Groups[3].Value) } }
  }
  $m2 = [regex]::Match($t, '^S\[I(\+\d+)?\]$')
  if ($m2.Success) {
    $off = 0
    if ($m2.Groups[1].Success) { $off = [int]$m2.Groups[1].Value.Substring(1) }
    return @{ Hi = $hi; Lo = @{ Copy = $true; Offset = $off } }
  }
  throw "unparsed rhs [$rhs]"
}

# Walk the case body with block-depth tracking (begin/end, case/end), stop at the
# `else` of the OUTER case or at the `end` that closes it.
function ParseToUcs4([string]$name) {
  $i = FindBegin $name
  $rules = @{}
  $raise = $false
  while ($i -lt $lines.Length -and -not ($lines[$i] -match $RE_CASE1)) { $i++ }
  if ($i -ge $lines.Length) { throw "no case S[I] in $name" }
  $depth = 1
  $j = $i + 1
  while ($j -lt $lines.Length) {
    $l = $lines[$j]
    $isElse = $l -match $RE_ELSE
    $isEnd = $l -match $RE_END
    $isBegin = $l -match $RE_BEGK
    if ($isElse) { break }
    if ($isBegin) { $depth++ }
    elseif ($isEnd) { $depth--; if ($depth -eq 0) { break } }
    if ($l -match '(?i:raise)') { $raise = $true; $j++; continue }
    $m = [regex]::Match($l, $RE_ENTRY)
    if ($m.Success) {
      $a = H2 $m.Groups[1].Value
      $b = $a
      if ($m.Groups[2].Success) { $b = H2 $m.Groups[2].Value }
      $r = ParseRhs $m.Groups[3].Value
      # to-UCS4 的形态必为两个常量（#$hi#$lo）：即使原文写作 "#$00 + S[I]"，
      # 在该行内 S[I] 就是本条 case 的常量字节，故直接折成 (hi*256+lo)。
      if ($r.Lo -is [hashtable]) {
        # `Result + #$hh + S[I]`：该行内 S[I] 即本条 case 的常量字节，折成 (hh<<8)|byte
        if ($r.Lo['Copy'] -eq $true) {
          $val = ($r.Hi * 256) + $a
        } else {
          $val = $r.Hi * 256 + (($a + ($r.Lo['Sign'] * $r.Lo['Const'])) -band 0xFF)
        }
      } else {
        $val = $r.Hi * 256 + [int]$r.Lo
      }
      for ($v = $a; $v -le $b; $v++) { if (-not $rules.ContainsKey($v)) { $rules[$v] = $val } }
    }
    $j++
  }
  return @{ Rules = $rules; Raise = $raise }
}

function ParseFromUcs4([string]$name) {
  $i = FindBegin $name
  $map = @{}
  $raise = $false
  while ($i -lt $lines.Length -and -not ($lines[$i] -match $RE_CASE2)) { $i++ }
  if ($i -ge $lines.Length) { throw "no case S[I+2] in $name" }
  $depth = 1; $j = $i + 1; $cur = -1
  while ($j -lt $lines.Length) {
    $l = $lines[$j]
    $isElse = $l -match $RE_ELSE
    $isEnd = $l -match $RE_END
    $isBegin = $l -match $RE_BEGK
    if ($l -match $RE_CASEK) { $depth++ }
    if ($isElse) {
      # 外层 case 的 else（depth 回到 1 后遇到）→ 终止；内层 case 的 else（depth>=2）→ 跳过
      if ($depth -le 1) { break }
      $j++; continue
    }
    if ($isBegin) { $depth++ }
    elseif ($isEnd) { $depth--; if ($depth -eq 0) { break } }
    $mh = [regex]::Match($l, $RE_HI)
    if ($mh.Success -and $depth -le 2) { $cur = H2 $mh.Groups[1].Value }
    if ($l -match '(?i:raise)') { $raise = $true; $j++; continue }
    $ml = [regex]::Match($l, $RE_ENTRY)
    # 仅采集内层 case（depth >= 3）的条目；外层 case 里 depth==2 的条目属于 #hi:
    # 关键字本身，不应记录（否则会串到上一个 #hi 分组）。
    if ($ml.Success -and $cur -ge 0 -and $depth -ge 3) {
      $a = H2 $ml.Groups[1].Value
      $b = $a
      if ($ml.Groups[2].Success) { $b = H2 $ml.Groups[2].Value }
      $r = ParseRhs $ml.Groups[3].Value
      if ($r.Lo -is [int]) {
        for ($v = $a; $v -le $b; $v++) { $map[('{0},{1}' -f $cur, $v)] = $r.Lo }
      }
      elseif ($r.Lo -is [hashtable]) {
        if ($r.Lo['Copy'] -eq $true) {
          # `Result + S[I + 3]` → 恒等拷贝（true 标记，C# 侧原样输出低字节 0..255）
          for ($v = $a; $v -le $b; $v++) { $map[('{0},{1}' -f $cur, $v)] = @($v, $true) }
        }
        else {
          # `AnsiChar(Ord(S[I + 3]) ± $NN)` → 按区间逐值求值（ParadoxConv.pas:449 等）
          for ($v = $a; $v -le $b; $v++) {
            $map[('{0},{1}' -f $cur, $v)] = @((($v + ($r.Lo['Sign'] * $r.Lo['Const'])) -band 0xFF), $false)
          }
        }
      }
      else { throw "unsupported UCS4->byte rule [$l]" }
    }
    $j++
  }
  return @{ Map = $map; Raise = $raise }
}

$tables = [ordered]@{
  'Cp1251ToUcs4Table'   = @{ P = (ParseToUcs4 'Cp1251ToUCS4');     Line = 297;  Kind = 'to';   Comment = 'CP1251 -> UCS4' }
  'Koi8rToUcs4Table'    = @{ P = (ParseToUcs4 'Koi8rToUCS4');      Line = 700;  Kind = 'to';   Comment = 'KOI8-R -> UCS4' }
  'Iso88595ToUcs4Table' = @{ P = (ParseToUcs4 'ISO88595ToUCS4');   Line = 1030; Kind = 'to';   Comment = 'ISO 8859-5 -> UCS4' }
  'Ucs4ToCp1251Table'   = @{ P = (ParseFromUcs4 'UCS4ToCp1251');   Line = 378;  Kind = 'from'; Comment = 'UCS4 -> CP1251' }
  'Ucs4ToKoi8rTable'    = @{ P = (ParseFromUcs4 'UCS4ToKoi8r');    Line = 844;  Kind = 'from'; Comment = 'UCS4 -> KOI8-R' }
  'Ucs4ToIso88595Table' = @{ P = (ParseFromUcs4 'UCS4ToISO88595'); Line = 1052; Kind = 'from'; Comment = 'UCS4 -> ISO 8859-5' }
  'Ucs4ToCp866Table'    = @{ P = (ParseFromUcs4 'UCS4ToCp866');    Line = 576;  Kind = 'from'; Comment = 'UCS4 -> CP866' }
}
foreach ($k in $tables.Keys) {
  $t = $tables[$k]
  if ($t.Kind -eq 'to') { Write-Host "$k : $($t.P.Rules.Count) rules raise=$($t.P.Raise)" }
  else { Write-Host "$k : $($t.P.Map.Count) bytes raise=$($t.P.Raise)" }
}

$out = New-Object System.Text.StringBuilder
[void]$out.AppendLine('// <auto-generated>')
[void]$out.AppendLine('// Generated by script from Source\RunGate\ParadoxConv.pas -- do not edit by hand.')
[void]$out.AppendLine('//   * to-UCS4 tables: `case S[I] of` entries `Result := Result + <#hi><#lo>`;')
[void]$out.AppendLine('//   * from-UCS4 tables: `case S[I+2] of` / `case S[I+3] of`, keyed by (S[I+2], S[I+3]);')
[void]$out.AppendLine('//     value 0xFF sentinel means "copy S[I+3] unchanged" (identity range).')
[void]$out.AppendLine('// </auto-generated>')
[void]$out.AppendLine('using System.Collections.Generic;')
[void]$out.AppendLine('')
[void]$out.AppendLine('namespace GXX.RunGate;')
[void]$out.AppendLine('')
[void]$out.AppendLine('public static partial class ParadoxConv')
[void]$out.AppendLine('{')
foreach ($k in $tables.Keys) {
  $t = $tables[$k]
  if ($t.Kind -eq 'to') {
    $rules = $t.P.Rules
    [void]$out.AppendLine("    /// <summary>$($t.Comment) (ParadoxConv.pas:$($t.Line), script-extracted, $($rules.Count) entries).</summary>")
    [void]$out.AppendLine("    private static readonly Dictionary<int, ushort> " + $k + " = new Dictionary<int, ushort>()")
    [void]$out.AppendLine("    {")
    foreach ($key in ($rules.Keys | Sort-Object)) {
      $val = [int]$rules[$key]
      [void]$out.AppendLine("        [0x" + ("{0:X2}" -f [int]$key) + "] = " + $val + ",")
    }
    [void]$out.AppendLine("    };")
  } else {
    $map = $t.P.Map
    [void]$out.AppendLine("    /// <summary>$($t.Comment) (ParadoxConv.pas:$($t.Line), script-extracted, $($map.Count) bytes).</summary>")
    [void]$out.AppendLine("    private static readonly Dictionary<int, Dictionary<int, byte>> " + $k + " = new Dictionary<int, Dictionary<int, byte>>()")
    [void]$out.AppendLine("    {")
    $his = $map.Keys | ForEach-Object { [int]($_ -split ',')[0] } | Sort-Object -Unique
    foreach ($hi in $his) {
      [void]$out.AppendLine(("        [0x{0:X2}] = new Dictionary<int, byte>" -f $hi))
      [void]$out.AppendLine("        {")
      $es = @()
      foreach ($mk in $map.Keys) {
        $pp = $mk -split ','
        if ([int]$pp[0] -ne $hi) { continue }
        $mv = $map[$mk]
        $es += ,([pscustomobject]@{ Key = [int]$pp[1]; Val = [int]$mv[0]; IsId = [bool]$mv[1] })
      }
      foreach ($e in ($es | Sort-Object { $_.Key })) {
        if ($e.IsId) {
          [void]$out.AppendLine('            [0x' + ('{0:X2}' -f [int]$e.Key) + '] = Identity,')
        } else {
          [void]$out.AppendLine('            [0x' + ('{0:X2}' -f [int]$e.Key) + '] = ' + [int]$e.Val + ',')
        }
      }
      [void]$out.AppendLine("        },")
    }
    [void]$out.AppendLine("    };")
  }
  [void]$out.AppendLine('')
}
[void]$out.AppendLine('}')
$dest = 'D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\lane-rungate-utils\GXX.CSharp\src\GXX.RunGate\ParadoxConv.Tables.g.cs'
[System.IO.File]::WriteAllText($dest, $out.ToString(), (New-Object System.Text.UTF8Encoding($false)))
Write-Host "written $dest"
