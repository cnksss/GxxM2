# ============================================================
#  Register-TestProjects.ps1   (ASCII-only on purpose: Windows PowerShell 5.1
#  reads .ps1 as ANSI, so non-ASCII literals here would break parsing.)
#
#  Lanes are forbidden from editing GXX.slnx, so their brand-new test projects
#  are invisible to `dotnet build GXX.slnx`.  At merge time the integrator runs
#  this script once to register them.  Idempotent.
#
#  Usage:
#    .\GXX.CSharp\tools\Register-TestProjects.ps1            # report only
#    .\GXX.CSharp\tools\Register-TestProjects.ps1 -Apply     # rewrite GXX.slnx
# ============================================================
[CmdletBinding()]
param([switch]$Apply)

$ErrorActionPreference = 'Stop'
$repo   = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$csRoot = Join-Path $repo 'GXX.CSharp'
$slnx   = Join-Path $csRoot 'GXX.slnx'
if (-not (Test-Path $slnx)) { throw "missing $slnx" }

$text = [System.IO.File]::ReadAllText($slnx, [System.Text.Encoding]::UTF8)

# projects that already exist on disk under tests/
$onDisk = Get-ChildItem (Join-Path $csRoot 'tests') -Recurse -File -Filter *.csproj |
          ForEach-Object { 'tests/' + $_.FullName.Substring((Join-Path $csRoot 'tests').Length + 1).Replace('\', '/') } |
          Sort-Object

$missing = @()
foreach ($rel in $onDisk) {
    if ($text.IndexOf("Project Path=`"$rel`"", [StringComparison]::Ordinal) -lt 0) { $missing += $rel }
}

Write-Host "test projects on disk : $($onDisk.Count)"
Write-Host "already in GXX.slnx   : $($onDisk.Count - $missing.Count)"
Write-Host "missing from GXX.slnx : $($missing.Count)"
foreach ($m in $missing) { Write-Host "    $m" -ForegroundColor Yellow }

if ($missing.Count -eq 0) { Write-Host "nothing to do." -ForegroundColor Green; exit 0 }
if (-not $Apply) { Write-Host "run again with -Apply to register them." -ForegroundColor Cyan; exit 0 }

# insert a <Project .../> line for each missing project into the /tests/ folder block
$block = ($missing | ForEach-Object { "    <Project Path=`"$_`" />" }) -join "`r`n"
$anchor = '  </Folder>'   # first </Folder> closes /src/, second closes /tests/
$idx = $text.IndexOf($anchor, $text.IndexOf($anchor, [StringComparison]::Ordinal) + 1, [StringComparison]::Ordinal)
if ($idx -lt 0) { throw "cannot locate the /tests/ Folder block in GXX.slnx" }

$new = $text.Substring(0, $idx) + $block + "`r`n" + $text.Substring($idx)
[System.IO.File]::WriteAllText($slnx, $new, (New-Object System.Text.UTF8Encoding($false)))
Write-Host "GXX.slnx updated with $($missing.Count) project(s)." -ForegroundColor Green

Push-Location $csRoot
try {
    dotnet build GXX.slnx -c Debug --nologo
    if ($LASTEXITCODE -ne 0) { Write-Host "[FAIL] build after registration" -ForegroundColor Red; exit 4 }
    dotnet test GXX.slnx -c Debug --nologo
    if ($LASTEXITCODE -ne 0) { Write-Host "[FAIL] tests after registration" -ForegroundColor Red; exit 5 }
} finally { Pop-Location }
Write-Host "registered + gates green." -ForegroundColor Green
