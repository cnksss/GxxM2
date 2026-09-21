# ============================================================
#  audit-stubs.ps1   (ASCII-only on purpose: Windows PowerShell 5.1 reads
#  .ps1 as ANSI, so non-ASCII literals here would break parsing.)
#
#  Silent-stub census for the Delphi -> C# port.
#
#  WHY THIS EXISTS (ledger 48.1, 2026-09-21):
#  A read-only lane proved that 46 ObjMon*Core.cs files hold ~6,300 methods of which
#  ~1,900 are bare `=> true;`.  Repo-wide the count is 4,728 across 144 files (4,710 of
#  them in GXX.M2Server).  A bare `=> true;` body compiles, passes tests and looks like
#  coverage while implementing NOTHING of the original semantics -- it is the only defect
#  class found so far that makes the TEST numbers optimistic, not just the report.
#
#  The project's own convention is to leave an explicit trace for anything not ported
#  (`NotPorted(name, original line)`), so a bare `=> true;` is a defect, NOT a placeholder.
#
#  Usage:
#    .\GXX.CSharp\tools\audit-stubs.ps1                  # totals + per project + top files
#    .\GXX.CSharp\tools\audit-stubs.ps1 -Top 40          # longer file list
#    .\GXX.CSharp\tools\audit-stubs.ps1 -File <path>     # detail for one file
# ============================================================
[CmdletBinding()]
param(
    [int]$Top = 20,
    [string]$File = '',
    [switch]$Quiet
)

$ErrorActionPreference = 'Stop'
$repo = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$srcRoot = Join-Path $repo 'GXX.CSharp\src'
$latin1 = [System.Text.Encoding]::GetEncoding(28591)

# Only a body that is EXACTLY `=> true;` (optionally with a trailing comment) counts.
# `=> true; // ...` is still a silent stub; `=> SomeCondition;` is not counted at all.
$pattern = '=>\s*true\s*;\s*(//.*)?$'

function Get-StubCount([string]$path) {
    $n = 0
    try {
        foreach ($line in [System.IO.File]::ReadAllLines($path, $latin1)) {
            if ($line -match $pattern) { $n++ }
        }
    } catch { }
    return $n
}

if ($File) {
    $p = if ([System.IO.Path]::IsPathRooted($File)) { $File } else { Join-Path $repo $File }
    if (-not (Test-Path $p)) { Write-Host "not found: $p" -ForegroundColor Red; exit 2 }
    $n = Get-StubCount $p
    Write-Host ("{0}  => true;  lines = {1}" -f $p, $n)
    $i = 0
    foreach ($line in [System.IO.File]::ReadAllLines($p, $latin1)) {
        $i++
        if ($line -match $pattern) { Write-Host ("{0,6}: {1}" -f $i, $line.Trim()) }
    }
    exit 0
}

$files = Get-ChildItem $srcRoot -Recurse -File -Filter *.cs |
         Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' }

$rows = @()
foreach ($f in $files) {
    $n = Get-StubCount $f.FullName
    if ($n -gt 0) {
        $rel = $f.FullName.Substring($srcRoot.Length + 1)
        $proj = ($rel -split '\\')[0]
        $rows += [pscustomobject]@{ N = $n; File = $rel; Proj = $proj }
    }
}

$total = ($rows | Measure-Object N -Sum).Sum
if (-not $total) { $total = 0 }

if (-not $Quiet) {
    Write-Host ''
    Write-Host ("SILENT STUBS ('=> true;' bodies): files={0}  total={1}" -f $rows.Count, $total) -ForegroundColor Yellow
    Write-Host ''
    Write-Host '=== by project ===' -ForegroundColor Cyan
    $rows | Group-Object Proj | ForEach-Object {
        [pscustomobject]@{
            Project = $_.Name
            Files   = $_.Count
            Stubs   = ($_.Group | Measure-Object N -Sum).Sum
        }
    } | Sort-Object Stubs -Descending | Format-Table -AutoSize | Out-String -Width 200 | Write-Host

    Write-Host ("=== top {0} files ===" -f $Top) -ForegroundColor Cyan
    $rows | Sort-Object N -Descending | Select-Object -First $Top |
        Format-Table -AutoSize | Out-String -Width 200 | Write-Host

    Write-Host 'Reminder (ledger 48.1): a bare `=> true;` is a DEFECT, not a placeholder.' -ForegroundColor Yellow
    Write-Host 'Anything intentionally unimplemented must leave an explicit trace (NotPorted(name, line)),' -ForegroundColor Yellow
    Write-Host 'and every delivery must reconcile: real bodies + NotPorted + silent stubs = method count.' -ForegroundColor Yellow
}
