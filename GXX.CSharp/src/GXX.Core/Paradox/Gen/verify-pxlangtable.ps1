# Read-back verification for ParadoxDataSet.Tables.g.cs against the Delphi source.
# Re-parses Source\RunGate\ParadoxDataSet.pas (GBK) and the generated C# table and
# compares all 118 entries field by field (Name / SortOrder / CodePage / SortOrderID).
# Exit code 0 = identical. ASCII-only script; see ledger 8.1.
param(
  [string]$Source = '',
  [string]$Generated = ''
)
$ErrorActionPreference = 'Stop'

$here = Split-Path -Parent $MyInvocation.MyCommand.Path
if ($Source -eq '') {
  $lane = $here
  for ($k = 0; $k -lt 5; $k++) { $lane = Split-Path -Parent $lane }
  $Source = $lane + '\Source\RunGate\ParadoxDataSet.pas'
}
if ($Generated -eq '') { $Generated = (Split-Path -Parent $here) + '\ParadoxDataSet.Tables.g.cs' }

# ---- side A: Delphi source ----
$lines = [System.IO.File]::ReadAllLines($Source, [System.Text.Encoding]::GetEncoding(936))
$start = -1
for ($i = 0; $i -lt $lines.Length; $i++) {
  if ($lines[$i] -match '^\s*PxLangTable\s*:\s*array\s*\[\s*1\s*\.\.\s*118\s*\]\s*of\s+TPxLang') { $start = $i; break }
}
if ($start -lt 0) { throw 'PxLangTable declaration not found' }

$RE_ENTRY = "^\s*\(+Name:\s*'(?<name>(?:[^']|'')*)';\s*SortOrder:\s*(?<so>-?\d+);\s*CodePage:\s*(?<cp>-?\d+);\s*SortOrderID:\s*'(?<sid>(?:[^']|'')*)'\)"
$srcEntries = New-Object System.Collections.ArrayList
for ($i = $start + 1; $i -lt $lines.Length; $i++) {
  $l = $lines[$i]
  if ($l.Trim() -eq '') { break }
  $m = [regex]::Match($l, $RE_ENTRY)
  if (-not $m.Success) { throw "unparsed source line $($i + 1): $l" }
  [void]$srcEntries.Add([pscustomobject]@{
    Name = $m.Groups['name'].Value.Replace("''", "'")
    SO   = [int]$m.Groups['so'].Value
    CP   = [int]$m.Groups['cp'].Value
    SID  = $m.Groups['sid'].Value.Replace("''", "'")
  })
}

# ---- side B: generated C# ----
$gen = [System.IO.File]::ReadAllLines($Generated, [System.Text.Encoding]::UTF8)
# Single-quoted so that backslashes reach the regex engine verbatim; the double
# quote inside the pattern is spliced in as a variable (ASCII-only script).
$Q = [char]0x22
$RE_CS = 'new TPxLang\(Name: ' + $Q + '(?<name>(?:[^' + $Q + '\\]|\\.)*)' + $Q + ', SortOrder: (?<so>-?\d+), CodePage: (?<cp>-?\d+), SortOrderID: ' + $Q + '(?<sid>(?:[^' + $Q + '\\]|\\.)*)' + $Q + '\),'
function UnCs([string]$s) {
  $sb = New-Object System.Text.StringBuilder
  for ($i = 0; $i -lt $s.Length; $i++) {
    $c = $s[$i]
    if ($c -eq '\' -and $i + 1 -lt $s.Length) {
      $n = $s[$i + 1]
      if ($n -eq '"') { [void]$sb.Append('"'); $i++; continue }
      if ($n -eq '\') { [void]$sb.Append('\'); $i++; continue }
      if ($n -eq 'u' -and $i + 5 -lt $s.Length) {
        $hex = $s.Substring($i + 2, 4)
        [void]$sb.Append([char][Convert]::ToInt32($hex, 16)); $i += 5; continue
      }
    }
    [void]$sb.Append($c)
  }
  return $sb.ToString()
}
$genEntries = New-Object System.Collections.ArrayList
foreach ($l in $gen) {
  $m = [regex]::Match($l, $RE_CS)
  if (-not $m.Success) { continue }
  [void]$genEntries.Add([pscustomobject]@{
    Name = (UnCs $m.Groups['name'].Value)
    SO   = [int]$m.Groups['so'].Value
    CP   = [int]$m.Groups['cp'].Value
    SID  = (UnCs $m.Groups['sid'].Value)
  })
}

if ($srcEntries.Count -ne 118) { throw "source entries = $($srcEntries.Count), expected 118" }
if ($genEntries.Count -ne $srcEntries.Count) { throw "generated entries = $($genEntries.Count), source = $($srcEntries.Count)" }

$bad = 0
for ($n = 0; $n -lt $srcEntries.Count; $n++) {
  $a = $srcEntries[$n]; $b = $genEntries[$n]
  if ($a.Name -cne $b.Name -or $a.SO -ne $b.SO -or $a.CP -ne $b.CP -or $a.SID -cne $b.SID) {
    $bad++
    Write-Host ("MISMATCH [{0}] src=({1}|{2}|{3}|{4}) gen=({5}|{6}|{7}|{8})" -f ($n + 1), $a.Name, $a.SO, $a.CP, $a.SID, $b.Name, $b.SO, $b.CP, $b.SID)
  }
}
if ($bad -ne 0) { throw "$bad mismatched entries" }
Write-Host "OK: 118/118 PxLangTable entries identical (Name/SortOrder/CodePage/SortOrderID)"
