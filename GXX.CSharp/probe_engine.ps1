$ErrorActionPreference = 'Stop'
$f = 'src\GXX.M2Server\Engine\NpcScriptEngine.cs'
$c = Get-Content -Encoding UTF8 $f
$c | Select-String -Pattern 'RegisterAll|CReg|AReg' | Select-Object -First 8 | ForEach-Object { "L" + $_.LineNumber + ": " + $_.Line.Trim().Substring(0, [Math]::Min(70, $_.Line.Trim().Length)) }
