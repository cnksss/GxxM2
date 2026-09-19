$ErrorActionPreference = 'Stop'
$f = 'src\GXX.M2Server\Engine\ViewListState3.cs'
$t = Get-Content -Encoding UTF8 $f -Raw
$t = $t.Replace('public static void ResetViewList3Defaults()', 'public static void ResetDefaults()')
[System.IO.File]::WriteAllText($f, $t, (New-Object Text.UTF8Encoding $false))
Write-Output 'ok'
