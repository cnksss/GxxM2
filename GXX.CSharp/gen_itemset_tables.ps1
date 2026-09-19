$ErrorActionPreference = 'Stop'
$pas = Get-ChildItem -Path 'D:\chuanqi\daima' -Recurse -Filter 'ItemSet.pas' -ErrorAction SilentlyContinue |
    Where-Object { $_.FullName -like '*utf8_mirror*' } | Select-Object -First 1
$m2 = Get-ChildItem -Path 'D:\chuanqi\daima' -Recurse -Filter 'M2Share.pas' -ErrorAction SilentlyContinue |
    Where-Object { $_.FullName -like '*utf8_mirror*' } | Select-Object -First 1
$c = Get-Content -Encoding UTF8 $pas.FullName
$share = Get-Content -Encoding UTF8 $m2.FullName

# 1) ButtonAddValueSaveClick ordered (key, field) pairs, lines 1384..1618
$pairs = New-Object System.Collections.Generic.List[string]
$rx = [regex]"Config\.WriteInteger\('Setup',\s*'(\w+)',\s*g_Config\.(\w+)\);"
for ($i = 1383; $i -le 1617; $i++) {
    $m = $rx.Match($c[$i])
    if ($m.Success) { $pairs.Add($m.Groups[1].Value + '|' + $m.Groups[2].Value) }
}
Write-Output ("AddValue keys: " + $pairs.Count)

# 2) defaults: scan M2Share typed-constant region (4200..5600) for "field: value;"
$defaults = @{}
$rxDef = [regex]"(\w+)\s*:\s*(\$\w+|\d+)\s*;"
for ($i = 4200; $i -le 5600; $i++) {
    foreach ($m in $rxDef.Matches($share[$i])) {
        $k = $m.Groups[1].Value; $v = $m.Groups[2].Value
        if (-not $defaults.ContainsKey($k)) { $defaults[$k] = $v }
    }
}

# 3) emit C# file
$out = New-Object System.Collections.Generic.List[string]
$out.Add('// Auto-generated: ItemSet.pas ButtonAddValueSaveClick ordered keys + typed-constant defaults (J35).')
$out.Add('namespace GXX.M2Server.Engine;')
$out.Add('')
$out.Add('public static class ItemSetAddValueTables')
$out.Add('{')
$out.Add('    public static readonly (string Key, string Field)[] Keys =')
$out.Add('    {')
foreach ($p in $pairs) {
    $a = $p -split '\|'
    $out.Add('        new("' + $a[0] + '", "' + $a[1] + '"),')
}
$out.Add('    };')
$out.Add('')
$out.Add('    public static readonly System.Collections.Generic.Dictionary<string, int> Defaults = new()')
$out.Add('    {')
foreach ($p in $pairs) {
    $a = $p -split '\|'
    $field = $a[1]
    $dv = if ($defaults.ContainsKey($field)) { $defaults[$field] } else { '0' }
    $hex = if ($dv.StartsWith('$')) { '0x' + $dv.Substring(1) } else { $dv }
    $out.Add('        ["' + $a[0] + '"] = ' + $hex + ',')
}
$out.Add('    };')
$out.Add('}')
[System.IO.File]::WriteAllLines('src\GXX.M2Server\Engine\ItemSetAddValueTables.g.cs', $out, (New-Object Text.UTF8Encoding $true))
Write-Output ("written lines: " + $out.Count)
