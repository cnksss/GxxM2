# ============================================================
#  GenSqlCreateTable.ps1   (ASCII-ONLY ON PURPOSE -- see note at the bottom)
#
#  Generates the C# port of the const sections of
#     Source/Common/MySqlCreateTableSql.pas
#     Source/Common/MsSqlCreateTableSql.pas
#  into
#     GXX.CSharp/src/GXX.Core/Data/<Unit>.cs
#  plus a fingerprint test file
#     GXX.CSharp/tests/GXX.Core.Tests/SqlCreateTableSqlTests.cs
#
#  Both Delphi units are pure constant-string concatenation
#  ('chunk' + sLineBreak + 'chunk' + OTHER_CONST + ...), so per the project
#  rules they are script-extracted, never hand-transcribed.
#
#  Extraction
#    * a definition starts at a line whose first non-space run is  NAME =
#    * its expression ends at the first unquoted ';'
#    * tokens: 'quoted literal' | // comment | { } comment | (* *) comment |
#              identifier | ;
#
#  Verification (all must pass, otherwise nothing is written)
#    V1 round-trip: the generated .cs is re-parsed (name + expression) and every
#       constant is re-evaluated from its own emitted text and compared with the
#       value obtained from the Delphi source.
#    V2 newline census: number of CRLF literals in the generated constants
#       equals the number of sLineBreak tokens in the Delphi definitions.
#    V3 statement census: same number of ';' characters on both sides.
#    V4 the emitted test file pins length + first line + last line per constant.
#
#  Usage:
#    .\GenSqlCreateTable.ps1 -OutRoot <repo root of the target worktree>
# ============================================================
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)][string]$OutRoot,
    [switch]$VerifyOnly
)

$ErrorActionPreference = 'Stop'
$OutRoot   = (Resolve-Path $OutRoot).Path
$srcCommon = Join-Path $OutRoot 'Source\Common'
$csRoot    = Join-Path $OutRoot 'GXX.CSharp'
if (-not (Test-Path $srcCommon)) { throw "not a repo root: $OutRoot" }

$GBK      = [System.Text.Encoding]::GetEncoding(936)
$DEF_RX   = [regex]'(?m)^[ \t]*([A-Za-z_]\w*)[ \t]*=[ \t]*'
$TOK_RX   = [regex]"(?s)'(?:[^']|'')*'|//[^\n]*|\{[^}]*\}|\(\*.*?\*\)|[A-Za-z_]\w*|;"
$EXPR_RX  = [regex]'"((?:[^"\\]|\\.)*)"|([A-Za-z_]\w*)'
$CONST_RX = [regex]'(?m)^[ \t]*public const string ([A-Za-z_]\w*) = (.*);[ \t\r]*$'

function Get-ConstRegion([string]$text) {
    $impl = $text.IndexOf('implementation')
    $head = if ($impl -ge 0) { $text.Substring(0, $impl) } else { $text }
    $c = $head.IndexOf('const')
    if ($c -lt 0) { throw 'no const section' }
    return $head.Substring($c + 5)
}

function Parse-Defs([string]$region) {
    $defs = New-Object System.Collections.Generic.List[object]
    $ms = $DEF_RX.Matches($region)
    for ($i = 0; $i -lt $ms.Count; $i++) {
        $m     = $ms[$i]
        $name  = $m.Groups[1].Value
        $start = $m.Index + $m.Length
        $end   = if ($i + 1 -lt $ms.Count) { $ms[$i + 1].Index } else { $region.Length }
        $block = $region.Substring($start, $end - $start)

        $tokens = New-Object System.Collections.Generic.List[object]
        foreach ($tm in $TOK_RX.Matches($block)) {
            $v = $tm.Value
            if ($v -eq ';') { break }
            if ($v[0] -eq "'") {
                $tokens.Add([pscustomobject]@{ Kind = 'str'; Text = $v.Substring(1, $v.Length - 2).Replace("''", "'") })
            } elseif ($v.StartsWith('//') -or $v[0] -eq '{' -or $v.StartsWith('(*')) {
                continue
            } else {
                $tokens.Add([pscustomobject]@{ Kind = 'id'; Text = $v })
            }
        }
        $defs.Add([pscustomobject]@{ Name = $name; Tokens = $tokens })
    }
    return $defs
}

function Expand-Tokens($tokens, [hashtable]$values) {
    $sb = New-Object System.Text.StringBuilder
    foreach ($t in $tokens) {
        if ($t.Kind -eq 'str') { [void]$sb.Append($t.Text) }
        elseif ($t.Text -eq 'sLineBreak') { [void]$sb.Append("`r`n") }
        elseif ($values.ContainsKey($t.Text)) { [void]$sb.Append($values[$t.Text]) }
        else { throw "unknown identifier in const expression: $($t.Text)" }
    }
    return $sb.ToString()
}

function Cs-Literal([string]$s) {
    $s = $s.Replace('\', '\\').Replace('"', '\"')
    $s = $s.Replace("`r", '\r').Replace("`n", '\n')
    return '"' + $s + '"'
}

function Cs-Expr($tokens) {
    $parts = @()
    foreach ($t in $tokens) {
        if ($t.Kind -eq 'str') { $parts += (Cs-Literal $t.Text) }
        elseif ($t.Text -eq 'sLineBreak') { $parts += '"\r\n"' }
        else { $parts += $t.Text }
    }
    if ($parts.Count -eq 0) { return '""' }
    return ($parts -join ' + ')
}

function Unescape-Cs([string]$s) {
    $sb = New-Object System.Text.StringBuilder
    $i = 0
    while ($i -lt $s.Length) {
        if ($s[$i] -eq '\' -and $i + 1 -lt $s.Length) {
            $n = $s[$i + 1]
            if ($n -eq 'r') { [void]$sb.Append("`r"); $i += 2; continue }
            if ($n -eq 'n') { [void]$sb.Append("`n"); $i += 2; continue }
            if ($n -eq 't') { [void]$sb.Append("`t"); $i += 2; continue }
            [void]$sb.Append($n); $i += 2; continue
        }
        [void]$sb.Append($s[$i]); $i++
    }
    return $sb.ToString()
}

function Count-Char([string]$s, [char]$c) {
    $n = 0
    foreach ($ch in $s.ToCharArray()) { if ($ch -eq $c) { $n++ } }
    return $n
}

# ---------------------------------------------------------------- generate --
$units = @(
    [pscustomobject]@{ Pas = 'MySqlCreateTableSql.pas'; Cs = 'MySqlCreateTableSql.cs'; Class = 'MySqlCreateTableSql' },
    [pscustomobject]@{ Pas = 'MsSqlCreateTableSql.pas'; Cs = 'MsSqlCreateTableSql.cs'; Class = 'MsSqlCreateTableSql' }
)

$dataDir = Join-Path $csRoot 'src\GXX.Core\Data'
if (-not (Test-Path $dataDir)) { New-Item -ItemType Directory -Path $dataDir -Force | Out-Null }

$failures = New-Object System.Collections.Generic.List[string]
$testRows = New-Object System.Collections.Generic.List[object]
$utf8NoBom = New-Object System.Text.UTF8Encoding($false)

foreach ($u in $units) {
    $text   = [System.IO.File]::ReadAllText((Join-Path $srcCommon $u.Pas), $GBK)
    $region = Get-ConstRegion $text
    $defs   = Parse-Defs $region

    $values       = @{}
    $body         = New-Object System.Text.StringBuilder
    $delphiSemi   = 0
    $delphiBreaks = 0
    $delphiChars  = 0
    foreach ($d in $defs) {
        $v = Expand-Tokens $d.Tokens $values
        $values[$d.Name] = $v
        $delphiSemi   += (Count-Char $v ';')
        $delphiChars  += $v.Length
        foreach ($t in $d.Tokens) { if ($t.Text -eq 'sLineBreak') { $delphiBreaks++ } }
        [void]$body.AppendLine('    public const string ' + $d.Name + ' = ' + (Cs-Expr $d.Tokens) + ';')
        [void]$body.AppendLine()
        $lines = $v -split "`r`n"
        $testRows.Add([pscustomobject]@{
            Class = $u.Class; Name = $d.Name; Len = $v.Length
            First = $lines[0]
            Last  = ($lines | Where-Object { $_ -ne '' } | Select-Object -Last 1)
        })
    }

    $header = @(
        '// <auto-generated>',
        '//   Generated by GXX.CSharp/tools/gen/GenSqlCreateTable.ps1 from',
        ('//   Source/Common/' + $u.Pas + ' -- 1:1 port of the const section; do not edit by hand.'),
        '//   Delphi sLineBreak is CRLF (#13#10) and is emitted as the C# literal "\r\n".',
        '//   Constant names keep the original spelling, including the original typo',
        '//   CEATE (not CREATE) in *_CEATE_DB_CONSTANT.',
        '// </auto-generated>',
        '',
        'namespace GXX.Core.Data;',
        '',
        '/// <summary>',
        ('/// ' + $u.Pas + ' const section, 1:1 (script-extracted, read back for comparison).'),
        '/// </summary>',
        ('public static class ' + $u.Class),
        '{'
    ) -join "`r`n"
    $csText = $header + "`r`n" + $body.ToString() + '}' + "`r`n"
    if (-not $VerifyOnly) { [System.IO.File]::WriteAllText((Join-Path $dataDir $u.Cs), $csText, $utf8NoBom) }

    # ---- V1 round-trip -----------------------------------------------------
    $seen    = @{}
    $csSemi  = 0
    $csBreak = 0
    foreach ($m in $CONST_RX.Matches($csText)) {
        $name = $m.Groups[1].Value
        $expr = $m.Groups[2].Value
        $csBreak += ([regex]::Matches($expr, '"\\r\\n"')).Count
        if (-not $values.ContainsKey($name)) { $failures.Add("V1 $($u.Cs): emitted constant not in Delphi source: $name"); continue }
        $sb = New-Object System.Text.StringBuilder
        $ok = $true
        foreach ($tm in $EXPR_RX.Matches($expr)) {
            if ($tm.Groups[1].Success) { [void]$sb.Append((Unescape-Cs $tm.Groups[1].Value)) }
            else {
                $id = $tm.Groups[2].Value
                if ($values.ContainsKey($id)) { [void]$sb.Append($values[$id]) }
                else { $failures.Add("V1 $($u.Cs): emitted expression references unknown '$id'"); $ok = $false }
            }
        }
        if (-not $ok) { continue }
        $rt = $sb.ToString()
        if ($rt -ne $values[$name]) {
            $failures.Add("V1 $($u.Cs): round-trip mismatch for $name (delphi=$($values[$name].Length) chars, emitted=$($rt.Length) chars)")
        }
        $csSemi += (Count-Char $rt ';')
        $seen[$name] = $true
    }
    foreach ($d in $defs) {
        if (-not $seen.ContainsKey($d.Name)) { $failures.Add("V1 $($u.Cs): definition missing from emitted file: $($d.Name)") }
    }
    if ($seen.Count -ne $defs.Count) { $failures.Add("V1 $($u.Cs): emitted const count $($seen.Count) != delphi def count $($defs.Count)") }

    # ---- V2 / V3 censuses --------------------------------------------------
    if ($csBreak -ne $delphiBreaks) { $failures.Add("V2 $($u.Cs): CRLF census $csBreak != sLineBreak tokens $delphiBreaks") }
    if ($csSemi  -ne $delphiSemi)   { $failures.Add("V3 $($u.Cs): semicolon census $csSemi != delphi $delphiSemi") }

    Write-Host ("OK  {0,-26} consts={1,-3} chars={2,-7} sLineBreak={3,-5} semicolons={4}" -f `
        $u.Pas, $defs.Count, $delphiChars, $delphiBreaks, $delphiSemi) -ForegroundColor Green
}

if ($failures.Count -gt 0) {
    Write-Host ''
    foreach ($f in $failures) { Write-Host $f -ForegroundColor Red }
    throw "$($failures.Count) verification failure(s)"
}

# ------------------------------------------------------------- test file ----
if (-not $VerifyOnly) {
    $tsb = New-Object System.Text.StringBuilder
    $hdr = @(
        '// <auto-generated>',
        '//   Generated by GXX.CSharp/tools/gen/GenSqlCreateTable.ps1.',
        '//   The per-constant fingerprints (length / first line / last line) are',
        '//   extracted from the Delphi source, so any later drift in the ported',
        '//   constants fails here.  See the conversion doc, section 5 (test gate).',
        '// </auto-generated>',
        '',
        'using System;',
        'using System.Collections.Generic;',
        'using System.Linq;',
        'using System.Reflection;',
        'using GXX.Core.Data;',
        'using Xunit;',
        '',
        'namespace GXX.Core.Tests;',
        '',
        '/// <summary>',
        '/// MySqlCreateTableSql.pas / MsSqlCreateTableSql.pas const section, pinned 1:1.',
        '/// </summary>',
        'public sealed class SqlCreateTableSqlTests',
        '{',
        '    private static readonly Dictionary<string, string> All = new();',
        '',
        '    static SqlCreateTableSqlTests()',
        '    {',
        '        foreach (var t in new[] { typeof(MySqlCreateTableSql), typeof(MsSqlCreateTableSql) })',
        '            foreach (var f in t.GetFields(BindingFlags.Public | BindingFlags.Static))',
        '                if (f.IsLiteral && f.FieldType == typeof(string))',
        '                    All[t.Name + "." + f.Name] = (string)f.GetValue(null)!;',
        '    }',
        '',
        '    public static IEnumerable<object[]> Fingerprints()',
        '    {'
    ) -join "`r`n"
    [void]$tsb.AppendLine($hdr)
    foreach ($r in $testRows) {
        $key = "$($r.Class).$($r.Name)"
        [void]$tsb.AppendLine(('        yield return new object[] {{ "{0}", {1}, {2}, {3} }};' -f `
            $key, $r.Len, (Cs-Literal $r.First), (Cs-Literal $r.Last)))
    }
    $tail = @(
        '    }',
        '',
        '    [Theory]',
        '    [MemberData(nameof(Fingerprints))]',
        '    public void ConstantMatchesDelphiSource(string key, int len, string first, string last)',
        '    {',
        '        Assert.True(All.ContainsKey(key), $"missing constant: {key}");',
        '        var v = All[key];',
        '        Assert.Equal(len, v.Length);',
        '        var lines = v.Split(new[] { "\r\n" }, StringSplitOptions.None);',
        '        Assert.Equal(first, lines[0]);',
        '        Assert.Equal(last, lines.Last(l => l.Length > 0));',
        '    }',
        '',
        '    [Fact]',
        '    public void ConstantCountMatchesDelphiSource()',
        '    {',
        ('        Assert.Equal(' + $testRows.Count + ', All.Count);'),
        '    }',
        '',
        '    /// <summary>Delphi sLineBreak is CRLF, not LF -- pin the newline bytes.</summary>',
        '    [Fact]',
        '    public void UsesCrLfNotLf()',
        '    {',
        '        var v = MySqlCreateTableSql.MYSQL_CEATE_DB_CONSTANT;',
        '        Assert.Contains("\r\n", v);',
        '        Assert.DoesNotContain("\n\n", v);',
        '    }',
        '',
        '    /// <summary>The CEATE typo is in the original source; it must not be "fixed".</summary>',
        '    [Fact]',
        '    public void KeepsOriginalTypoInConstantName()',
        '    {',
        '        Assert.Contains("MySqlCreateTableSql.MYSQL_CEATE_DB_CONSTANT", All.Keys);',
        '        Assert.DoesNotContain("MySqlCreateTableSql.MYSQL_CREATE_DB_CONSTANT", All.Keys);',
        '    }',
        '',
        '    /// <summary>Account script embeds the db_constant script, then the version REPLACE.</summary>',
        '    [Fact]',
        '    public void AccountScriptEmbedsDbConstantThenVersion()',
        '    {',
        '        var v = MySqlCreateTableSql.MYSQL_CREATE_ACCOUNT_TABLES;',
        '        var embedded = MySqlCreateTableSql.MYSQL_CEATE_DB_CONSTANT;',
        '        Assert.Contains(embedded, v);',
        '        Assert.True(v.IndexOf(embedded, StringComparison.Ordinal)',
        '                    > v.IndexOf("CREATE TABLE `Account`", StringComparison.Ordinal));',
        '        Assert.EndsWith("Values(\"account_version\", " + MySqlCreateTableSql.MYSQL_DBVERSION + ")", v);',
        '    }',
        '',
        '    [Theory]',
        '    [InlineData("MySqlCreateTableSql.MYSQL_CREATE_ROLEDATA_TABLES")]',
        '    [InlineData("MySqlCreateTableSql.MYSQL_CREATE_HERO_ROLEDATA_TABLES")]',
        '    [InlineData("MySqlCreateTableSql.MYSQL_CREATE_HUMAN_ROLEDATA_TABLES")]',
        '    [InlineData("MySqlCreateTableSql.MYSQL_CREATE_HUMAN_1_ROLEDATA_TABLES")]',
        '    [InlineData("MySqlCreateTableSql.MYSQL_CREATE_HUMAN_2_ROLEDATA_TABLES")]',
        '    [InlineData("MySqlCreateTableSql.MYSQL_CREATE_M2DATA_TABLES")]',
        '    [InlineData("MsSqlCreateTableSql.MSSQL_CREATE_ROLEDATA_TABLES")]',
        '    [InlineData("MsSqlCreateTableSql.MSSQL_CREATE_M2DATA_TABLES")]',
        '    public void TableScriptsContainCreateTable(string key)',
        '    {',
        '        var v = All[key];',
        '        Assert.True(v.Length > 100);',
        '        Assert.Contains("CREATE TABLE", v);',
        '        Assert.Contains(";", v);',
        '    }',
        '}'
    ) -join "`r`n"
    [void]$tsb.AppendLine($tail)

    [System.IO.File]::WriteAllText((Join-Path $csRoot 'tests\GXX.Core.Tests\SqlCreateTableSqlTests.cs'), $tsb.ToString(), $utf8NoBom)
    Write-Host 'wrote tests/GXX.Core.Tests/SqlCreateTableSqlTests.cs' -ForegroundColor Green
}

Write-Host ''
Write-Host 'GenSqlCreateTable: V1 round-trip + V2 CRLF census + V3 semicolon census all passed.' -ForegroundColor Green
