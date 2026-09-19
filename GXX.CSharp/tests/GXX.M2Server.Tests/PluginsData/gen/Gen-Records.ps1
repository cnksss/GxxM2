# ASCII-only: full inventory of PluginInterface.pas type declarations.
# Emits: records.tsv / fields.tsv / aliases.tsv  (UTF-8, no BOM)
param(
  [Parameter(Mandatory=$true)][string]$Src,
  [Parameter(Mandatory=$true)][string]$Scratch
)
$Utf8NoBom = New-Object System.Text.UTF8Encoding($false)
function Strip([string]$t) {
  $t = [regex]::Replace($t, '\{[^}]*\}', ' ')
  $i = $t.IndexOf('//'); if ($i -ge 0) { $t = $t.Substring(0, $i) }
  return $t.Trim()
}
$lines = Get-Content -LiteralPath (Join-Path $Src 'PluginInterface.pas') -Encoding Default
$records = New-Object System.Collections.Generic.List[string]
$fields = New-Object System.Collections.Generic.List[string]
$aliases = New-Object System.Collections.Generic.List[string]

$curRec = $null; $curStart = 0; $order = 0; $inRec = $false; $acc = ''; $accLine = 0
for ($i = 0; $i -lt $lines.Count; $i++) {
  $n = $i + 1
  $t = Strip $lines[$i]
  if ($t -eq '') { continue }
  if (-not $inRec) {
    if ($t -match '^([A-Za-z_][A-Za-z0-9_]*)\s*=\s*(packed\s+)?record\b') {
      $inRec = $true; $curRec = $Matches[1]; $curStart = $n; $order = 0; $acc = ''; $accLine = 0
      continue
    }
    if ($t -match '^([A-Za-z_][A-Za-z0-9_]*)\s*=\s*([^=]+);$' -and $t -notmatch '=\s*(function|procedure)\b') {
      $aliases.Add(("{0}`t{1}`t{2}" -f $Matches[1], $n, ($Matches[2] -replace '\s+', ' ').Trim()))
    }
    continue
  }
  if ($t -match '^end\s*;') {
    $records.Add(("{0}`t{1}`t{2}`t{3}" -f $curRec, $curStart, $n, $order))
    $inRec = $false; $curRec = $null
    continue
  }
  if ($acc -eq '') { $accLine = $n }
  $acc = if ($acc -eq '') { $t } else { $acc + ' ' + $t }
  if ($acc.EndsWith(';')) {
    $stmt = $acc; $acc = ''
    # a statement may terminate the record on the same line ("... nParam10: Integer end;")
    $endsRec = $false
    if ($stmt -match '(?i)\bend\s*;$') { $endsRec = $true; $stmt = ($stmt -replace '(?i)\s*\bend\s*;$', '').Trim() }
    if ($stmt -ne '') {
      # original :111 puts the last field and the record terminator on one line
      # ("nParam10: Integer end;"). Drop the trailing ';' and any trailing 'end'
      # so the accumulator yields exactly one "name: type" statement.
      $stmt = ($stmt -replace '(?i)\s*\bend\s*$', '').Trim()
      $stmt = $stmt.TrimEnd(';').Trim()
      if ($stmt -match '^([A-Za-z_][A-Za-z0-9_]*)\s*:\s*(.+)$') {
        $fname = $Matches[1]; $ftype = ($Matches[2] -replace '\s+', ' ').Trim(); $cnt = 1
        if ($ftype -match '(?i)^array\s*\[(.+?)\]\s*of\s*(.+)$') {
          $rng = $Matches[1]; if ($rng -match '(\d+)\s*\.\.\s*(\d+)') { $cnt = [int]$Matches[2] - [int]$Matches[1] + 1 } else { $cnt = -1 }
          $ftype = 'array of ' + $Matches[2].Trim()
        }
        $order++
        $fields.Add(("{0}`t{1}`t{2}`t{3}`t{4}`t{5}" -f $curRec, $order, $accLine, $fname, $ftype, $cnt))
      }
    }
    if ($endsRec) {
      $records.Add(("{0}`t{1}`t{2}`t{3}" -f $curRec, $curStart, $n, $order))
      $inRec = $false; $curRec = $null
    }
  }
}
[System.IO.File]::WriteAllText((Join-Path $Scratch 'records.tsv'), (($records -join "`n") + "`n"), $Utf8NoBom)
[System.IO.File]::WriteAllText((Join-Path $Scratch 'fields.tsv'), (($fields -join "`n") + "`n"), $Utf8NoBom)
[System.IO.File]::WriteAllText((Join-Path $Scratch 'aliases.tsv'), (($aliases -join "`n") + "`n"), $Utf8NoBom)
Write-Output ("records={0} fields={1} aliases={2}" -f $records.Count, $fields.Count, $aliases.Count)
