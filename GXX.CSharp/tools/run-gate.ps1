# ============================================================
#  run-gate.ps1   (ASCII-ONLY ON PURPOSE -- see ledger 8.1: Windows PowerShell 5.1 reads .ps1
#  as ANSI, so a non-ASCII literal in this file breaks parsing.  The Chinese crash markers this
#  gate looks for are therefore BUILT FROM CHAR CODES at runtime, never typed literally.)
#
#  The project gate, with a criterion that CANNOT be fooled by a testhost crash.
#
#  WHY (ledger 52.3, measured by lane p10-db-login-forms 2026-09-21):
#  `dotnet test` prints its per-assembly summary line BEFORE/WHILE the host can die, so a run
#  whose testhost crashed on a StackOverflow still printed a success line
#      "Passed! - Failed: 0, Passed: 675"
#  (the count even changed between runs: 675 / 693) while $LASTEXITCODE was 1 and the stderr
#  carried a Stack overflow / "test host process was aborted" message.  Judging the gate by the
#  summary line alone therefore reports a FALSE GREEN, and every test after the crash point
#  silently did not run.
#
#  Use THIS script (or replicate its three checks) instead of eyeballing test output:
#    1. `dotnet build` exit code must be 0;
#    2. `dotnet test` exit code must be 0;
#    3. the combined output must contain NO crash marker.
#  All three are printed as evidence, so a "green" claim is reproducible.
#
#  Usage:
#    .\GXX.CSharp\tools\run-gate.ps1                     # build + full solution tests
#    .\GXX.CSharp\tools\run-gate.ps1 -SkipBuild
#    .\GXX.CSharp\tools\run-gate.ps1 -Project <csproj>    # one test project instead of the sln
# ============================================================
[CmdletBinding()]
param(
    [string]$Repo = '',
    [string]$Solution = 'GXX.CSharp/GXX.slnx',
    [string]$Project = '',
    [string]$Log = '',
    [switch]$SkipBuild
)

$ErrorActionPreference = 'Stop'
if (-not $Repo) { $Repo = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path }
if (-not $Log) { $Log = Join-Path $Repo 'GXX.CSharp\_gate_run.log' }

# Chinese markers, assembled from code points so this file stays pure ASCII.
#   U+6D4B U+8BD5 U+4E3B U+673A U+8FDB U+7A0B   = "test host process"
#   U+6D4B U+8BD5 U+8FD0 U+884C U+5DF2 U+4E2D U+6B62 = "test run was aborted"
$cjkTestHost = ([string][char]0x6D4B) + [char]0x8BD5 + [char]0x4E3B + [char]0x673A + [char]0x8FDB + [char]0x7A0B
$cjkAborted  = ([string][char]0x6D4B) + [char]0x8BD5 + [char]0x8FD0 + [char]0x884C + [char]0x5DF2 + [char]0x4E2D + [char]0x6B62

$CRASH_MARKERS = @(
    'Stack overflow',
    'StackOverflowException',
    'testhost',
    'Test host process crashed',
    'The active test run was aborted',
    'Process is terminated due to',
    $cjkTestHost,
    $cjkAborted
)

function Fail([string]$why) {
    Write-Host ''
    Write-Host ("GATE: FAIL -- {0}" -f $why) -ForegroundColor Red
    exit 1
}

Push-Location $Repo
try {
    # NOTE (ledger 59.6): with $ErrorActionPreference = 'Stop', Windows PowerShell 5.1 turns a
    # native command's STDERR into a TERMINATING error when it flows through a pipeline.  The gate
    # then died in the middle of the test phase -- printing NO evidence and exiting 1 -- which is
    # the mirror image of the false green this script exists to prevent (a false RED).
    # Fix: relax the preference for the native calls and merge every stream explicitly (*>&1).
    $ErrorActionPreference = 'Continue'

    if (-not $SkipBuild) {
        Write-Host ("== build: {0} ==" -f $Solution) -ForegroundColor Cyan
        & dotnet build $Solution -c Debug --nologo -m:1 -p:BuildInParallel=false *>&1 |
            Tee-Object -FilePath $Log
        $buildCode = $LASTEXITCODE
        Write-Host ("build exit code       : {0}" -f $buildCode)
        if ($buildCode -ne 0) { Fail ("dotnet build exit={0}" -f $buildCode) }
    } else {
        Write-Host '== build skipped (-SkipBuild) ==' -ForegroundColor Yellow
        if (Test-Path $Log) { Remove-Item $Log -Force }
    }

    $target = if ($Project) { $Project } else { $Solution }
    Write-Host ("== test: {0} ==" -f $target) -ForegroundColor Cyan
    & dotnet test $target -c Debug --nologo --no-build *>&1 |
        Tee-Object -FilePath $Log -Append
    $code = $LASTEXITCODE

    $text = ''
    try { $text = [System.IO.File]::ReadAllText($Log, [System.Text.Encoding]::UTF8) } catch { }

    $hits = @()
    foreach ($m in $CRASH_MARKERS) {
        if ($text.IndexOf($m, [System.StringComparison]::Ordinal) -ge 0) { $hits += $m }
    }

    Write-Host ''
    Write-Host '== gate evidence ==' -ForegroundColor Cyan
    Write-Host ("dotnet test exit code : {0}" -f $code)
    Write-Host ("crash markers found   : {0}" -f $(if ($hits.Count) { $hits -join ', ' } else { 'none' }))

    if ($code -ne 0) { Fail ("dotnet test exit={0} (a summary line saying PASSED is NOT enough)" -f $code) }
    if ($hits.Count -gt 0) { Fail ("crash markers present: {0} -- results after the crash did not run" -f ($hits -join ', ')) }

    Write-Host ''
    Write-Host 'GATE: PASS (build 0 error, test exit 0, no crash markers)' -ForegroundColor Green
    exit 0
} finally {
    Pop-Location
}
