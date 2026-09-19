$ErrorActionPreference = 'Continue'
Set-Location $PSScriptRoot
if (Test-Path 'GXX.sln') { Remove-Item 'GXX.sln' -Force }
dotnet new sln -n GXX --format slnx --force | Out-Null
$projs = Get-ChildItem -Path 'src', 'tests' -Recurse -Filter *.csproj | Sort-Object FullName
$fail = 0
foreach ($p in $projs) {
    $rel = $p.FullName.Substring((Get-Location).Path.Length + 1)
    $r = dotnet sln GXX.sln add $rel 2>&1
    if ($LASTEXITCODE -ne 0) { $fail++; Write-Output ("FAIL: " + $rel + " => " + ($r -join ' ')) }
}
Write-Output ("failures: " + $fail)
Get-Content -Encoding UTF8 'GXX.slnx'
