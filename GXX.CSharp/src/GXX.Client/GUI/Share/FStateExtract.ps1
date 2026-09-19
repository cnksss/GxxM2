param([string]$src, [string]$out)
# Routine inventory for FState.pas.
# Tracks (* *) and { } block comments so commented-out routines are not counted (the unit
# really does contain a commented-out duplicate of TFrmDlg.DNPC_PLAYIMG_PAINT at 23702-23773).
$lines = Get-Content -LiteralPath $src -Encoding Default

$code = New-Object 'bool[]' $lines.Count
$inParen = $false
$inBrace = $false
for ($i = 0; $i -lt $lines.Count; $i++) {
  $l = $lines[$i]
  $sb = New-Object System.Text.StringBuilder
  $n = $l.Length; $k = 0
  while ($k -lt $n) {
    if ($inParen) {
      if ($k + 1 -lt $n -and $l[$k] -eq '*' -and $l[$k+1] -eq ')') { $inParen = $false; $k += 2; continue }
      $k++; continue
    }
    if ($inBrace) {
      if ($l[$k] -eq '}') { $inBrace = $false }
      $k++; continue
    }
    if ($k + 1 -lt $n -and $l[$k] -eq '(' -and $l[$k+1] -eq '*') { $inParen = $true; $k += 2; continue }
    if ($k + 1 -lt $n -and $l[$k] -eq '/' -and $l[$k+1] -eq '/') { break }
    if ($l[$k] -eq '{') { $inBrace = $true; $k++; continue }
    [void]$sb.Append($l[$k]); $k++
  }
  $code[$i] = ($sb.ToString().Trim() -ne '')
}

$reTop = '^(function|procedure|constructor|destructor)\s'
$reKw  = '^(const|type|var|resourcestring|threadvar|initialization|finalization)\s*$'

$sections = New-Object System.Collections.ArrayList
$starts = New-Object System.Collections.ArrayList
for ($i = 0; $i -lt $lines.Count; $i++) {
  if (-not $code[$i]) { continue }
  $l = $lines[$i]
  if ($l -match $reTop) { [void]$starts.Add($i) }
  elseif ($l -match $reKw) { [void]$sections.Add([pscustomobject]@{ Line = $i + 1; Kind = $l.Trim() }) }
}

$routines = New-Object System.Collections.ArrayList
for ($k = 0; $k -lt $starts.Count; $k++) {
  $s = $starts[$k]
  $e = if ($k + 1 -lt $starts.Count) { $starts[$k + 1] - 1 } else { $lines.Count - 1 }
  while ($e -gt $s -and (-not $code[$e])) { $e-- }
  $b = -1
  for ($j = $s; $j -le $e; $j++) { if ($code[$j] -and $lines[$j] -match '^begin\b') { $b = $j; break } }
  $hdr = $lines[$s].Trim()
  $bline = if ($b -lt 0) { -1 } else { $b + 1 }
  $nm = $hdr -replace '^(function|procedure|constructor|destructor)\s+', ''
  $nm = ($nm -split '\(')[0]; $nm = ($nm -split ';')[0]
  [void]$routines.Add([pscustomobject]@{
      Line = $s + 1; EndLine = $e + 1; BodyBegin = $bline; Lines = ($e - $s + 1); Name = $nm.Trim(); Header = $hdr
    })
}

$routines | Export-Csv -LiteralPath (Join-Path $out 'routines.csv') -NoTypeInformation -Encoding UTF8
$sections | Export-Csv -LiteralPath (Join-Path $out 'sections.csv') -NoTypeInformation -Encoding UTF8

"ROUTINES=$($routines.Count)"
"ROUTINE_LINES=$(($routines | Measure-Object -Property Lines -Sum).Sum)"
"NO_BEGIN=$(@($routines | Where-Object { $_.BodyBegin -lt 0 }).Count)"
$dup = $routines | Group-Object Name | Where-Object { $_.Count -gt 1 }
"DUPLICATE_ROUTINE_NAMES=$($dup.Count)"
foreach ($d in $dup) { "  $($d.Name) : $(($d.Group | ForEach-Object { $_.Line }) -join ',')" }
