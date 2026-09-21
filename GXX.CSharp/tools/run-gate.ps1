# ============================================================
#  run-gate.ps1   (ASCII-only: Windows PowerShell 5.1 reads .ps1 as ANSI)
#
#  The project gate, with a criterion that CANNOT be fooled by a testhost crash.
#
#  WHY (ledger 52.3, measured by lane p10-db-login-forms 2026-09-21):
#  `dotnet test` prints its per-assembly summary line BEFORE/WHILE the host can die, so a run
#  whose testhost crashed on a StackOverflow still printed
#      "已通过! - 失败: 0，通过: 675"
#  (and the count changed between runs: 675 / 693 ...) with $LASTEXITCODE = 1 and
#  "Stack overflow / 测试主机进程中止" on stderr.  Judging the gate by the summary line alone
#  therefore reports a FALSE GREEN, and every test after the crash point silently did not run.
#
#  Use THIS script (or replicate its three checks) instead of eyeballing test output:
#    1. `dotnet build` exit code must be 0;
#    2. `dotnet test` exit code must be 0;
#    3. the combined output must contain NO crash markers.
#  All three are reported explicitly, with the evidence, so a "green" claim is reproducible.
#
#  Usage:
#    .\GXX.CSharp\tools\run-gate.ps1                 # build + full solution tests
#    .\GXX.CSharp\tools\run-gate.ps1 -SkipBuild
#    .\GXX.CSharp\tools\run-gate.ps1 -Project <csproj>   # one test project instead of the sln
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

# Markers that mean "the run is NOT trustworthy", regardless of any summary line.
$CRASH_MARKERS = @(
    'Stack overflow',
    'StackOverflowException',
    'testhost',
    'Test host process crashed',
    '测试主机进程',
    '测试运行已中止',
    'The active test run was aborted',
    'Process is terminated due to'
)

function Fail([string]$why) {
    Write-Host ''
    Write-Host ("GATE: FAIL -- {0}" -f $why) -ForegroundColor Red
    exit 1
}

Push-Location $Repo
try {
    if (-not $SkipBuild) {
        Write-Host "== build: $Solution ==" -ForegroundColor Cyan
        dotnet build $Solution -c Debug --nologo -m:1 -p:BuildInParallel=false 2>&1 | Tee-Object -FilePath $Log
        if ($LASTEXITCODE -ne 0) { Fail ("dotnet build exit={0}" -f $LASTEXITCODE) }
    } else {
        Write-Host '== build skipped (-SkipBuild) ==' -ForegroundColor Yellow
    }

    $target = if ($Project) { $Project } else { $Solution }
    Write-Host ("== test: {0} ==" -f $target) -ForegroundColor Cyan
    $testArgs = @('test', $target, '-c', 'Debug', '--nologo', '--no-build')
    & dotnet @testArgs 2>&1 | Tee-Object -FilePath $Log -Append
    $code = $LASTEXITCODE

    $text = ''
    try { $text = [System.IO.File]::ReadAllText($Log, [System.Text.Encoding]::UTF8) } catch { }

    $hits = @()
    foreach ($m in $CRASH_MARKERS) { if ($text.IndexOf($m, [System.StringComparison]::Ordinal) -ge 0) { $hits += $m } }

    Write-Host ''
    Write-Host '== gate evidence ==' -ForegroundColor Cyan
    Write-Host ("dotnet test exit code : {0}" -f $code)
    Write-Host ("crash markers found   : {0}" -f $(if ($hits.Count) { $hits -join ', ' } else { 'none' }))

    if ($code -ne 0) { Fail ("dotnet test exit={0} (a summary line saying 已通过 is NOT enough)" -f $code) }
    if ($hits.Count -gt 0) { Fail ("crash markers present: {0} -- results after the crash did not run" -f ($hits -join ', ')) }

    Write-Host ''
    Write-Host 'GATE: PASS (build 0 error, test exit 0, no crash markers)' -ForegroundColor Green
    exit 0
} finally {
    Pop-Location
}
