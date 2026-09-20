# Check-LaneReady.ps1 -- pre-merge readiness check for every parallel lane.
#
# WHY THIS EXISTS
#   A wave was merged that included a lane whose HEAD was deliberately labelled
#   "WIP / do-not-merge" (work-in-progress, must not be merged).  The lane followed the
#   rule; the integrator did not check.  The gate caught it (the commit did not
#   compile, CS0246), but only after a full ~3 minute gate run was wasted and the
#   whole wave had to be aborted and re-merged.
#
#   Run this BEFORE building a wave.  It reports, per lane:
#     ahead  = commits in <branch> not in main  (0 => nothing to merge)
#     head   = that commit's subject
#     WIP    = subject contains 'WIP'  => DO NOT MERGE
#     dirty  = uncommitted files in the lane's worktree (fine, but informative)
#
# USAGE
#   powershell -NoProfile -ExecutionPolicy Bypass -File tools\Check-LaneReady.ps1
#   powershell ... -File tools\Check-LaneReady.ps1 -Main main
#
# Exit code: 0 if no mergeable lane is WIP, 1 otherwise (so it can gate a script).
#
# NOTE: keep this file ASCII-only -- Windows PowerShell 5.1 parses .ps1 as ANSI,
#       so non-ASCII in CODE gets mangled.  (Comments are inert but stay ASCII too.)

[CmdletBinding()]
param(
    [string]$Main = 'main',
    [string]$WorktreeRoot = '.worktrees'
)

$ErrorActionPreference = 'Stop'

function Invoke-Git {
    param([string[]]$Arguments)
    $out = & git @Arguments 2>&1
    if ($LASTEXITCODE -ne 0) { return $null }
    return $out
}

$repoRoot = (Invoke-Git @('rev-parse', '--show-toplevel'))
if (-not $repoRoot) { Write-Error 'not inside a git repository'; exit 2 }
Set-Location $repoRoot

$branches = & git for-each-ref --format='%(refname:short)' 'refs/heads/par/*' 2>&1
if (-not $branches) { Write-Host 'no par/* branches found'; exit 0 }

$wipLanes = 0
$readyLanes = 0

Write-Host ''
Write-Host ('{0,-28} {1,-7} {2,-7} {3}' -f 'LANE', 'AHEAD', 'DIRTY', 'HEAD SUBJECT')
Write-Host ('-' * 110)

foreach ($branch in $branches) {
    if ($branch -eq 'par/integration') { continue }

    $ahead = [int](& git rev-list --count "$Main..$branch" 2>$null)
    $subject = (& git log -1 --format=%s $branch 2>$null)
    if (-not $subject) { $subject = '(no commits)' }

    $wt = Join-Path $WorktreeRoot ($branch -replace '^par/', '')
    $dirty = 0
    if (Test-Path $wt) {
        $dirty = (& git -C $wt status --porcelain 2>$null | Measure-Object).Count
    }

    $isWip = $subject -match 'WIP'
    $flag = ''
    if ($ahead -gt 0) {
        if ($isWip) { $flag = '  <== WIP, DO NOT MERGE'; $wipLanes++ }
        else { $readyLanes++ }
    }

    Write-Host ('{0,-28} {1,-7} {2,-7} {3}{4}' -f $branch, $ahead, $dirty, $subject, $flag)
}

Write-Host ''
if ($wipLanes -gt 0) {
    Write-Host ("BLOCKED: {0} mergeable lane(s) are marked WIP (do-not-merge) -- drop them from the wave." -f $wipLanes) -ForegroundColor Red
    exit 1
}
Write-Host ("OK: {0} lane(s) ready to merge, none marked WIP." -f $readyLanes) -ForegroundColor Green
exit 0
