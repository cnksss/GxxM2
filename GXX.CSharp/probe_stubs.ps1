$ErrorActionPreference = 'Stop'
$codes = Get-Content -Encoding UTF8 'src\GXX.M2Server\Engine\NpcCmdCodes.cs' |
    Select-String -Pattern 'public const int (nNC_\w+|nNA_\w+) = (\d+);' |
    ForEach-Object { [pscustomobject]@{ Name = $_.Matches[0].Groups[1].Value; Code = [int]$_.Matches[0].Groups[2].Value } }
$impl = Get-Content -Encoding UTF8 'src\GXX.M2Server\Engine\NpcScriptCommands.cs' -Raw
$implemented = @{}
foreach ($m in [regex]::Matches($impl, 'NpcCmdCodes\.(nNC_\w+|nNA_\w+)')) { $implemented[$m.Groups[1].Value] = $true }
$missing = $codes | Where-Object { -not $implemented.ContainsKey($_.Name) }
Write-Output ("total: " + $codes.Count + "  referenced: " + ($codes.Count - $missing.Count) + "  stub-only: " + $missing.Count)
$missing | ForEach-Object { Write-Output ("  " + $_.Name + " = " + $_.Code) }
