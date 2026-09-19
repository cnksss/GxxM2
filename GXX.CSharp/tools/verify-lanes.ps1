# ============================================================
#  verify-lanes.ps1   (ASCII-only on purpose: Windows PowerShell 5.1 reads
#  .ps1 as ANSI, so non-ASCII literals here would break parsing.)
#
#  Mechanical enforcement of the parallel-dispatch file-ownership map
#  (docs/parallel ledger section 2, mirrored in tools/lane-zones.tsv).
#
#  For every lane branch + every lane working tree it reports:
#    * OUT-OF-ZONE paths   -- a lane touched a path it does not own
#    * MODIFIED-EXISTING   -- a lane changed/deleted a tracked file
#                             (lanes must only ADD files)
#    * SCRATCH             -- leftover temp dirs (_tmp_src/, .tmp-src/, ...)
#    * commit count / dirty count
#
#  Usage:
#    .\GXX.CSharp\tools\verify-lanes.ps1                 # all lanes
#    .\GXX.CSharp\tools\verify-lanes.ps1 -Lane lane-resources
#    .\GXX.CSharp\tools\verify-lanes.ps1 -Quiet          # only problems
# ============================================================
[CmdletBinding()]
param(
    [string[]]$Lane,
    [switch]$Quiet
)

$ErrorActionPreference = 'Stop'
$repo = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path

$zoneFile = Join-Path $PSScriptRoot 'lane-zones.tsv'
if (-not (Test-Path $zoneFile)) { throw "missing $zoneFile" }

$zones = @{}
# -Encoding UTF8 is required: the zone file holds non-ASCII report paths, and
# PS 5.1 would otherwise decode it as ANSI and produce false OUT-OF-ZONE hits.
foreach ($line in (Get-Content $zoneFile -Encoding UTF8)) {
    if ($line -match '^\s*#' -or $line -match '^\s*$') { continue }
    $parts = $line -split "`t"
    if ($parts.Count -lt 2) { continue }
    $zones[$parts[0].Trim()] = ($parts[1].Trim() -split ';') | ForEach-Object { $_.Trim() } | Where-Object { $_ }
}

function Test-InZone {
    param([string]$Path, [string[]]$Globs)
    $p = $Path -replace '\\', '/'
    foreach ($g in $Globs) {
        $rx = [regex]::Escape($g)
        $rx = $rx -replace '\\\*\\\*', '.*'      # ** -> any depth
        $rx = $rx -replace '\\\*', '[^/]*'       # *  -> within one segment
        if ($p -match "^$rx$") { return $true }
    }
    return $false
}

# Scratch / build-intermediate directories that must never be committed.
# Matched CASE-SENSITIVELY (-cmatch below) so that a legitimate Delphi unit such
# as Source/Client-HGE/DxComponent/Objects.pas is not mistaken for an 'obj*' dir.
# obj*/bin* are included because .gitignore only covers exactly 'obj/' and 'bin/',
# so a lane running MSBuild with a custom -p:BaseIntermediateOutputPath (seen in
# practice: obj_s/, obj_subB/) would otherwise get its intermediates committed.
$SCRATCH_RX = '(^|/)(_tmp[^/]*|\.tmp[^/]*|_probe[^/]*|\.probe[^/]*|_recon[^/]*|\.recon[^/]*|_scan[^/]*|\.scan[^/]*|_stage[^/]*|\.stage[^/]*|_salvage[^/]*|\.scratch[^/]*|_scratch[^/]*|obj[^/]*|bin[^/]*)/'

# A glob prefixed with '!' is an ALLOW-MODIFY grant: the lane may also change
# files that already existed (used for the few lanes whose job is fixing an
# existing shared file, e.g. the GXX.Core RTL/bool-stringification repair).
# Everything else keeps the default rule: lanes may only ADD files.
function Split-ZoneGlobs([string[]]$Globs) {
    $allow = @(); $allowMod = @()
    foreach ($g in $Globs) {
        if ($g.StartsWith('!')) { $allowMod += $g.Substring(1) } else { $allow += $g }
    }
    return [pscustomobject]@{ Allow = $allow; AllowMod = $allowMod }
}

$lanes = if ($Lane) { $Lane } else { $zones.Keys | Sort-Object }
$problems = 0

foreach ($name in $lanes) {
    if (-not $zones.ContainsKey($name)) { Write-Host "[skip] no zone row for $name" -ForegroundColor DarkGray; continue }
    $split  = Split-ZoneGlobs $zones[$name]
    $globs  = $split.Allow
    $modGlobs = $split.AllowMod
    $branch = "par/$name"
    $wt     = Join-Path $repo ".worktrees\$name"

    $hasBranch = [bool](git -C $repo branch --list $branch)
    $hasWt     = Test-Path $wt
    if (-not $hasBranch -and -not $hasWt) { Write-Host "[skip] lane not present: $name" -ForegroundColor DarkGray; continue }

    $commits = 0; $base = ''
    if ($hasBranch) {
        $base    = (git -C $repo merge-base main $branch).Trim()
        $commits = [int](git -C $repo rev-list --count "$base..$branch")
    }

    # paths that already existed before the lane started: only THOSE may not be
    # modified.  A file the lane added earlier on its own branch is free to be
    # edited again -- incremental slices do exactly that.
    $basePaths = @{}
    if ($base) {
        foreach ($bp in (git -C $repo ls-tree -r --name-only $base)) { $basePaths[$bp] = $true }
    }

    # ---- committed side: added vs modified/deleted -------------------------
    $committedPaths = @()
    if ($hasBranch -and $commits -gt 0) {
        $committedPaths = git -C $repo diff --name-status "$base..$branch"
    }
    # ---- working side ------------------------------------------------------
    $wtPaths = @()
    if ($hasWt) { $wtPaths = git -C $wt status --porcelain -uall }

    $outOfZone = New-Object System.Collections.Generic.List[string]
    $touched   = 0

    foreach ($entry in $committedPaths) {
        if (-not $entry) { continue }
        $cols = $entry -split "`t"
        $stat = $cols[0]; $path = $cols[-1]
        $touched++
        $mayModify = Test-InZone $path $modGlobs
        if (-not (Test-InZone $path $globs) -and -not $mayModify) { $outOfZone.Add("committed [$stat] $path") }
        elseif ($stat -ne 'A' -and -not $mayModify) { $outOfZone.Add("MODIFIED-EXISTING [$stat] $path") }
    }
    foreach ($entry in $wtPaths) {
        if (-not $entry) { continue }
        $stat = $entry.Substring(0, 2)
        $path = $entry.Substring(3).Trim('"')
        $touched++
        if ($path -cmatch $SCRATCH_RX) { $outOfZone.Add("SCRATCH [$stat] $path"); continue }
        $mayModify = Test-InZone $path $modGlobs
        if (-not (Test-InZone $path $globs) -and -not $mayModify) { $outOfZone.Add("worktree [$stat] $path") }
        elseif ($stat -notmatch '^\?\?' -and -not $mayModify -and $basePaths.ContainsKey(($path -replace '\\', '/'))) {
            $outOfZone.Add("MODIFIED-EXISTING [$stat] $path")
        }
    }

    $dirty = ($wtPaths | Measure-Object).Count
    $tag = if ($outOfZone.Count -eq 0) { 'OK  ' } else { 'FAIL' }
    $color = if ($outOfZone.Count -eq 0) { 'Green' } else { 'Red' }
    if (-not $Quiet -or $outOfZone.Count -gt 0) {
        Write-Host ("[{0}] {1,-24} commits={2,-4} dirty={3,-4} touched={4}" -f $tag, $name, $commits, $dirty, $touched) -ForegroundColor $color
    }
    foreach ($v in $outOfZone) { Write-Host "        $v" -ForegroundColor Red; $problems++ }
}

Write-Host ''
if ($problems -eq 0) {
    Write-Host 'ALL LANES WITHIN THEIR ZONES' -ForegroundColor Green
    exit 0
} else {
    Write-Host "$problems zone violation(s) found -- fix before merging." -ForegroundColor Red
    exit 1
}
