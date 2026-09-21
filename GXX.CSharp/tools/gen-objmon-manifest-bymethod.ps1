# tools/gen-objmon-manifest-bymethod.ps1
# 归属判定 v5：**实现头行匹配**（批次J253 —— 修正 J252 的坐标系错误）
#
# J252 的失败根因：用"类**声明**行"排序分区间，而类**实现**在另一个区段
#   （ObjMon.pas 声明在第 9-531 行、实现在约 540-9500 行）=> 坐标系混用。
# 本版的修正：改用**同一套坐标系** ——
#   我在每个 core 文件文档注释里记录的行号**本身就是实现行号**
#   （如 `Create（1838-1842）` 的 1838 就是 `constructor TCowMonster.Create;` 所在行），
#   故判定应与"**实现头行**"（procedure/function/constructor/destructor Class.Method 所在行）比对。
#
# 判据：
#   1. 抽 ObjMon.pas 里所有 `Class.Method` 实现头的行号 -> header[class] = 行号集合
#   2. 对每个 core 文件，取其 /// 行里的整数（[1,9502]）
#   3. 对每个整数 v，找**与之相等或相差 <= Tol** 的实现头行，投该行所属类一票
#   4. 类归属到票数最多且唯一最多的文件；票数 < MinVotes 或并列 => 未判定
#   —— 只看"我是否记录了那个类的**方法所在行**"，不依赖类名出现与否。

[CmdletBinding()]
param(
    [string]$ProjectRoot = (Split-Path -Parent $PSScriptRoot),
    [string]$RepoRoot = (Split-Path -Parent (Split-Path -Parent $PSScriptRoot)),
    [string]$PascalFile = '_analysis/utf8_mirror/M2Engine/ObjMon.pas',
    [string]$SrcDir = 'src/GXX.M2Server',
    [string]$ManifestPath = 'docs/ObjMon-manifest.tsv',
    [int]$Tol = 2,
    [int]$MinVotes = 2,
    [switch]$WriteManifest
)

$ErrorActionPreference = 'Stop'

$pasPath = Join-Path $RepoRoot $PascalFile
$srcPath = Join-Path $ProjectRoot $SrcDir
$manPath = Join-Path $ProjectRoot $ManifestPath

# ---------- 1. 类声明行 ----------
$pasLines = Get-Content $pasPath -Encoding UTF8
$decl = [ordered]@{}
for ($i = 0; $i -lt $pasLines.Count; $i++) {
    if ($pasLines[$i] -match '^\s*(T[A-Za-z0-9_]+)\s*=\s*class\b') {
        $n = $Matches[1]
        if (-not $decl.Contains($n)) { $decl[$n] = ($i + 1) }
    }
}
$all = @($decl.Keys)

# ---------- 2. 实现头行 -> 类 ----------
$headerOf = @{}          # 行号 -> 类
$headersOf = @{}         # 类 -> 行号数组
foreach ($c in $all) { $headersOf[$c] = @() }
for ($i = 0; $i -lt $pasLines.Count; $i++) {
    if ($pasLines[$i] -match '^\s*(?:procedure|function|constructor|destructor)\s+(T[A-Za-z0-9_]+)\.([A-Za-z0-9_]+)') {
        $c = $Matches[1]
        if ($headersOf.ContainsKey($c)) {
            $ln = $i + 1
            $headerOf[$ln] = $c
            $headersOf[$c] += $ln
        }
    }
}

# ---------- 3. 各 core 文件的 /// 行号 ----------
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

# ---------- 4. 投票 ----------
$votes = @{}      # file -> (class -> count)
foreach ($fn in $fileNums.Keys) {
    $tally = @{}
    foreach ($v in $fileNums[$fn]) {
        for ($d = -$Tol; $d -le $Tol; $d++) {
            $k = $v + $d
            if ($headerOf.ContainsKey($k)) {
                $c = $headerOf[$k]
                if (-not $tally.ContainsKey($c)) { $tally[$c] = 0 }
                $tally[$c]++
                break          # 一个数字只投最近的一票
            }
        }
    }
    $votes[$fn] = $tally
}

$owner = @{}; $score = @{}; $unresolved = @()
foreach ($c in $all) {
    $best = @()
    foreach ($fn in $votes.Keys) {
        $n = 0
        if ($votes[$fn].ContainsKey($c)) { $n = $votes[$fn][$c] }
        if ($n -ge $MinVotes) { $best += [pscustomobject]@{ File = $fn; N = $n } }
    }
    if ($best.Count -eq 0) { $unresolved += $c; continue }
    $top = ($best | Sort-Object N -Descending)[0].N
    $win = @($best | Where-Object { $_.N -eq $top })
    if ($win.Count -ne 1) { $unresolved += $c; continue }
    $owner[$c] = $win[0].File
    $score[$c] = $win[0].N
}

# ---------- 5. 输出 ----------
$perFile = @{}
foreach ($c in $owner.Keys) {
    $fn = $owner[$c]
    if (-not $perFile.ContainsKey($fn)) { $perFile[$fn] = @() }
    $perFile[$fn] += $c
}
$noHeaders = @($all | Where-Object { $headersOf[$_].Count -eq 0 })

Write-Output "======== 归属判定 v5：实现头行匹配 ========"
Write-Output ("ObjMon 类数 {0} / 文件数 {1} / Tol {2} / MinVotes {3}" -f $all.Count, $files.Count, $Tol, $MinVotes)
Write-Output ("实现头总数 {0}（无实现头的类 {1}）" -f $headerOf.Count, $noHeaders.Count)
Write-Output ("已归属 {0} / 未归属 {1}" -f $owner.Count, $unresolved.Count)
Write-Output ""
Write-Output ("  A 全集 : {0}" -f $(if ($unresolved.Count -eq 0) { "通过" } else { "失败" }))
Write-Output ("  B 唯一 : 一个类只能有一个 owner（结构保证）")
Write-Output ""

if ($unresolved.Count -gt 0) {
    Write-Output "--- 未归属 ---"
    foreach ($c in $unresolved) {
        $bestN = 0
        foreach ($fn in $votes.Keys) { if ($votes[$fn].ContainsKey($c)) { if ($votes[$fn][$c] -gt $bestN) { $bestN = $votes[$fn][$c] } } }
        Write-Output ("  {0,-34} @{1,-5} 实现头 {2,3} 个 / 最高票 {3}" -f $c, $decl[$c], $headersOf[$c].Count, $bestN)
    }
    Write-Output ""
}

Write-Output "--- 归属明细（按文件）---"
foreach ($fn in ($perFile.Keys | Sort-Object)) {
    Write-Output ("  {0}" -f $fn)
    foreach ($c in ($perFile[$fn] | Sort-Object { $decl[$_] })) {
        Write-Output ("      {0,-34} @{1,5}  票 {2}" -f $c, $decl[$c], $score[$c])
    }
}

if ($WriteManifest) {
    $rows = @()
    foreach ($c in ($all | Sort-Object { $decl[$_] })) {
        $w = ''; if ($owner.ContainsKey($c)) { $w = $owner[$c] }
        $rows += ("{0}`t{1}`t{2}" -f $c, $decl[$c], $w)
    }
    [System.IO.File]::WriteAllText($manPath, ("class`tpascal_line`tcore_file`n" + ($rows -join "`n") + "`n"), (New-Object System.Text.UTF8Encoding $false))
    Write-Output ("`n清单已写出：{0}（{1} 行）" -f $ManifestPath, ($rows.Count + 1))
}
Write-Output "======== 结束 ========"
