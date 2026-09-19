$ErrorActionPreference = 'Stop'
# Dump NpcCmdNames.g.cs constant tables
$f = 'src\GXX.M2Server\Engine\NpcCmdNames.g.cs'
$c = Get-Content -Encoding UTF8 $f
Write-Output ("lines: " + $c.Count)
$c[0..20] | ForEach-Object { $_ }
