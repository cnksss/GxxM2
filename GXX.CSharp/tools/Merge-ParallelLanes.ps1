# ============================================================
#  Merge-ParallelLanes.ps1   (ASCII-only on purpose: Windows PowerShell 5.1
#  reads .ps1 as ANSI, so non-ASCII text here would break parsing.)
#
#  Integrates the parallel-dispatch lane branches into the trunk (main).
#  Must be run in the PRIMARY worktree; refuses to run when the trunk has
#  uncommitted changes (i.e. while the sequential session is mid-batch).
#
#  Usage:
#    .\GXX.CSharp\tools\Merge-ParallelLanes.ps1 -DryRun
#    .\GXX.CSharp\tools\Merge-ParallelLanes.ps1 -Lanes lane-resources
#    .\GXX.CSharp\tools\Merge-ParallelLanes.ps1
#
#  See GXX.CSharp\docs\并行派发台账.md for the lane / file-ownership map.
# ============================================================
[CmdletBinding()]
param(
    [string[]]$Lanes = @(
        'lane-client-gui-mir','lane-resources','lane-rungate-utils','lane-dbserver-utils',
        'lane-dxcomponent','lane-login-logdata'
    ),
    [switch]$DryRun,
    [switch]$SkipBuild
)

$ErrorActionPreference = 'Stop'
$repo = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
Write-Host "repo = $repo"

# --- 0. must run in the primary worktree -----------------------------------
$gitCommon = (git -C $repo rev-parse --git-common-dir).Trim()
$gitDir    = (git -C $repo rev-parse --git-dir).Trim()
if ($gitCommon -ne $gitDir) {
    throw "Run this script in the PRIMARY worktree (git-dir=$gitDir, common-dir=$gitCommon)"
}

# --- 1. trunk must be clean ------------------------------------------------
# Coordination files written by the dispatch session itself are ignored here:
# they are intentionally left untracked until final integration.
# (ASCII-only pattern on purpose -- PS 5.1 reads .ps1 as ANSI.)
$COORD_PATTERN = 'GXX\.CSharp/(docs|tools)/'
$dirty = git -C $repo status --porcelain | Where-Object { $_ -notmatch $COORD_PATTERN }
if ($dirty) {
    Write-Host ""
    Write-Host "[ABORT] Primary worktree has uncommitted changes -> the sequential session is working." -ForegroundColor Yellow
    Write-Host "        Wait until it commits, then re-run."
    $dirty | ForEach-Object { Write-Host "    $_" }
    exit 2
}

# --- 2. collect lane status ------------------------------------------------
$plan = @()
foreach ($lane in $Lanes) {
    $branch = "par/$lane"
    $exists = (git -C $repo branch --list $branch)
    if (-not $exists) { Write-Host "[skip] no such branch: $branch" -ForegroundColor DarkGray; continue }

    $ahead  = [int](git -C $repo rev-list --count "main..$branch")
    $behind = [int](git -C $repo rev-list --count "$branch..main")
    if ($ahead -eq 0) { Write-Host "[skip] nothing to merge: $branch" -ForegroundColor DarkGray; continue }

    $files = git -C $repo diff --name-only "main...$branch"
    $plan += [pscustomobject]@{ Lane = $lane; Branch = $branch; Ahead = $ahead; Behind = $behind; Files = $files }
}

if (-not $plan) { Write-Host ""; Write-Host "No lane to merge." -ForegroundColor Green; exit 0 }

Write-Host ""
Write-Host "=== merge plan ===" -ForegroundColor Cyan
foreach ($p in $plan) {
    Write-Host ("{0,-26} +{1,-4} -{2,-4} files={3}" -f $p.Branch, $p.Ahead, $p.Behind, $p.Files.Count)
    $p.Files | ForEach-Object { Write-Host "      $_" -ForegroundColor DarkGray }
}

# --- 3. conflict pre-check: two lanes must not touch the same file ---------
$owner = @{}
foreach ($p in $plan) {
    foreach ($f in $p.Files) {
        if ($owner.ContainsKey($f)) {
            Write-Host ""
            Write-Host "[WARN] same file changed by two lanes: $f ($($owner[$f]) and $($p.Branch))" -ForegroundColor Red
        } else { $owner[$f] = $p.Branch }
    }
}

if ($DryRun) { Write-Host ""; Write-Host "[DryRun] nothing merged." -ForegroundColor Yellow; exit 0 }

# --- 4. merge each lane ----------------------------------------------------
foreach ($p in $plan) {
    Write-Host ""
    Write-Host ">>> git merge --no-ff $($p.Branch)" -ForegroundColor Cyan
    git -C $repo merge --no-ff $p.Branch -m "Merge parallel lane $($p.Lane): $($p.Ahead) commit(s)"
    if ($LASTEXITCODE -ne 0) {
        Write-Host "[CONFLICT] merge of $($p.Branch) failed. The file-ownership rule was violated; resolve manually." -ForegroundColor Red
        Write-Host "  hint: git status ; git diff --name-only --diff-filter=U"
        exit 3
    }
}

# --- 5. integration gate ---------------------------------------------------
if (-not $SkipBuild) {
    Push-Location (Join-Path $repo 'GXX.CSharp')
    try {
        Write-Host ""
        Write-Host ">>> dotnet build GXX.slnx -c Release" -ForegroundColor Cyan
        dotnet build GXX.slnx -c Release --nologo
        if ($LASTEXITCODE -ne 0) { Write-Host "[FAIL] Release build failed after merge." -ForegroundColor Red; exit 4 }

        Write-Host ""
        Write-Host ">>> dotnet test GXX.slnx -c Release" -ForegroundColor Cyan
        dotnet test GXX.slnx -c Release --nologo
        if ($LASTEXITCODE -ne 0) { Write-Host "[FAIL] tests failed after merge." -ForegroundColor Red; exit 5 }
    } finally { Pop-Location }
}

Write-Host ""
Write-Host "=== merge finished, all gates green ===" -ForegroundColor Green
Write-Host "Next: register new test projects in GXX.slnx; update docs/parallel ledger section 6."
