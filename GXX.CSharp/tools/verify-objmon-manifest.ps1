# tools/verify-objmon-manifest.ps1
# 归属判定 v6 + **A'/B'/D 不变量**（批次J254 —— 实施 §9.7 留下的缺口）
#
# v5（J253）只做了"单 owner"归属，本版按 §9.7 的结论改成：
#   A' 全集  ：每个类**至少**一个文件
#   B' 反向  ：每个文件声明的类，该文件里确实记录了该类的实现头行（>= MinVotes 票）
#   D  覆盖  ：对该类的**每一条**实现头行，其声明文件集合里都记录过对应行号（+-Tol）
#              —— 这一条才是真正有价值的：它能发现"一个类只被移植了一部分"
#
# 归属规则（多文件）：候选 = 票数 >= Max(MinVotes, 25% * 该类最高票) 的所有文件。
#   （v5 的"只取唯一最高"会把 TMonster / TFoxMonster 这类**合法跨两个文件**的类的
#     第二个文件丢掉 —— J253 已实测到 41/46 文件有归属、5 个为空。）

[CmdletBinding()]
param(
    [string]$ProjectRoot = (Split-Path -Parent $PSScriptRoot),
    [string]$RepoRoot = (Split-Path -Parent (Split-Path -Parent $PSScriptRoot)),
    [string]$PascalFile = '_analysis/utf8_mirror/M2Engine/ObjMon.pas',
    [string]$SrcDir = 'src/GXX.M2Server',
    [string]$ManifestPath = 'docs/ObjMon-manifest.tsv',
    [int]$Tol = 2,
    [int]$MinVotes = 2,
    [double]$KeepRatio = 0.25,
    [switch]$WriteManifest
)

$ErrorActionPreference = 'Stop'
$pasPath = Join-Path $RepoRoot $PascalFile
$srcPath = Join-Path $ProjectRoot $SrcDir
$manPath = Join-Path $ProjectRoot $ManifestPath

# ---- 1. 类声明行 + 实现头行 ----
$pasLines = Get-Content $pasPath -Encoding UTF8
$decl = [ordered]@{}
for ($i = 0; $i -lt $pasLines.Count; $i++) {
    if ($pasLines[$i] -match '^\s*(T[A-Za-z0-9_]+)\s*=\s*class\b') {
        $n = $Matches[1]; if (-not $decl.Contains($n)) { $decl[$n] = ($i + 1) }
    }
}
$all = @($decl.Keys)
$headersOf = @{}
foreach ($c in $all) { $headersOf[$c] = New-Object System.Collections.Generic.List[int] }
$headerOwner = @{}
for ($i = 0; $i -lt $pasLines.Count; $i++) {
    if ($pasLines[$i] -match '^\s*(?:procedure|function|constructor|destructor)\s+(T[A-Za-z0-9_]+)\.([A-Za-z0-9_]+)') {
        $c = $Matches[1]; $ln = $i + 1
        if ($headersOf.ContainsKey($c)) { $headersOf[$c].Add($ln); $headerOwner[$ln] = $c }
    }
}

# ---- 2. 各文件 /// 行号 ----
$files = Get-ChildItem -Path $srcPath -Filter 'ObjMon*Core.cs' -File
$fileNums = @{}
foreach ($f in $files) {
    $nums = New-Object System.Collections.Generic.List[int]
    foreach ($ln in (Get-Content $f.FullName -Encoding UTF8)) {
        if ($ln -notmatch '^\s*///') { continue }
        foreach ($m in [regex]::Matches($ln, '(?<![0-9])([0-9]{1,5})(?![0-9])')) {
            $v = [int]$m.Groups[1].Value
            if ($v -ge 1 -and $v -le 9502) { $nums.Add($v) }
        }
    }
    $fileNums[$f.Name] = $nums
}

# ---- 3. 投票 ----
$votes = @{}
foreach ($fn in $fileNums.Keys) {
    $tally = @{}
    foreach ($v in $fileNums[$fn]) {
        for ($d = -$Tol; $d -le $Tol; $d++) {
            $k = $v + $d
            if ($headerOwner.ContainsKey($k)) {
                $c = $headerOwner[$k]
                if (-not $tally.ContainsKey($c)) { $tally[$c] = 0 }
                $tally[$c]++
                break
            }
        }
    }
    $votes[$fn] = $tally
}

# ---- 4. 多文件归属 ----
$owners = @{}; $maxVote = @()
foreach ($c in $all) {
    $vs = @()
    foreach ($fn in $votes.Keys) { if ($votes[$fn].ContainsKey($c)) { $vs += $votes[$fn][$c] } }
    $mx = 0; if ($vs.Count -gt 0) { $mx = ($vs | Measure-Object -Maximum).Maximum }
    $maxVote += $mx
    $thr = [Math]::Max($MinVotes, [int][Math]::Ceiling($mx * $KeepRatio))
    $sel = @()
    foreach ($fn in $votes.Keys) {
        if ($votes[$fn].ContainsKey($c) -and $votes[$fn][$c] -ge $thr) { $sel += $fn }
    }
    if ($sel.Count -gt 0) { $owners[$c] = @($sel | Sort-Object) }
}

# ---- 5. A' / B' / D ----
$noOwner = @($all | Where-Object { -not $owners.ContainsKey($_) })

$bFail = @()
foreach ($c in $owners.Keys) {
    foreach ($fn in $owners[$c]) {
        if (-not $votes[$fn].ContainsKey($c) -or $votes[$fn][$c] -lt $MinVotes) { $bFail += ("{0} -> {1}" -f $c, $fn) }
    }
}

$dRows = @(); $dFail = @()
foreach ($c in $all) {
    if (-not $owners.ContainsKey($c)) { continue }
    $set = @{}; foreach ($fn in $owners[$c]) { $set[$fn] = $true }
    $hs = @($headersOf[$c])
    if ($hs.Count -eq 0) { continue }
    $cov = 0
    foreach ($h in $hs) {
        $ok = $false
        foreach ($fn in $set.Keys) {
            foreach ($v in $fileNums[$fn]) { if ([Math]::Abs($v - $h) -le $Tol) { $ok = $true; break } }
            if ($ok) { break }
        }
        if ($ok) { $cov++ }
    }
    $ratio = [math]::Round($cov / $hs.Count, 3)
    $dRows += [pscustomobject]@{ Class = $c; Headers = $hs.Count; Covered = $cov; Ratio = $ratio; Files = ($owners[$c] -join ',') }
    if ($cov -lt $hs.Count) { $dFail += $c }
}

# ---- 6. 输出 ----
Write-Output "======== ObjMon manifest v6：A'/B'/D 不变量 ========"
Write-Output ("类 {0} / 文件 {1} / Tol {2} / MinVotes {3} / KeepRatio {4}" -f $all.Count, $files.Count, $Tol, $MinVotes, $KeepRatio)
Write-Output ("有归属的类 {0} / 无归属 {1}" -f $owners.Count, $noOwner.Count)
$multi = @($owners.Keys | Where-Object { $owners[$_].Count -gt 1 })
Write-Output ("跨多文件的类 {0} 个" -f $multi.Count)
Write-Output ("文件被引用数 {0} / {1}" -f (@($owners.Values | ForEach-Object { $_ } | Sort-Object -Unique).Count), $files.Count)
Write-Output ""
Write-Output ("  A' 全集 : {0}" -f $(if ($noOwner.Count -eq 0) { "通过" } else { "失败" }))
Write-Output ("  B' 反向 : {0}" -f $(if ($bFail.Count -eq 0) { "通过" } else { "失败" }))
Write-Output ("  D  覆盖 : 完全覆盖 {0} / {1} 类；未完全覆盖 {2} 个 => {3}" -f ($dRows.Count - $dFail.Count), $dRows.Count, $dFail.Count, $(if ($dFail.Count -eq 0) { "通过" } else { "需复核" }))
Write-Output ""

if ($multi.Count -gt 0) {
    Write-Output "--- 跨多文件的类（v5 会丢掉第二个文件）---"
    foreach ($c in ($multi | Sort-Object { $decl[$_] })) { Write-Output ("  {0,-34} {1}" -f $c, ($owners[$c] -join ' + ')) }
    Write-Output ""
}
if ($dFail.Count -gt 0) {
    Write-Output "--- D 未完全覆盖（该类的实现头未在其声明文件里全部出现）---"
    foreach ($r in ($dRows | Where-Object { $_.Covered -lt $_.Headers } | Sort-Object Ratio)) {
        Write-Output ("  {0,-34} 覆盖 {1,3}/{2,3} ({3})  {4}" -f $r.Class, $r.Covered, $r.Headers, $r.Ratio, $r.Files)
    }
    Write-Output ""
}
if ($noOwner.Count -gt 0) {
    Write-Output "--- A' 未归属 ---"
    foreach ($c in $noOwner) { Write-Output ("  {0,-34} @{1} 实现头 {2} 个" -f $c, $decl[$c], $headersOf[$c].Count) }
    Write-Output ""
}

if ($WriteManifest) {
    $rows = @()
    foreach ($c in ($all | Sort-Object { $decl[$_] })) {
        $w = ''; if ($owners.ContainsKey($c)) { $w = ($owners[$c] -join ' + ') }
        $rows += ("{0}`t{1}`t{2}" -f $c, $decl[$c], $w)
    }
    [System.IO.File]::WriteAllText($manPath, ("class`tpascal_line`tcore_file`n" + ($rows -join "`n") + "`n"), (New-Object System.Text.UTF8Encoding $false))
    Write-Output ("清单已写出：{0}（{1} 行）" -f $ManifestPath, ($rows.Count + 1))
}
Write-Output "======== 结束 ========"
