# tools/gen-objmon-manifest-byrange.ps1
# 用**行号区间**做归属判定（批次J252）
#
# 为什么这条路与前三者本质不同：
#   前三种判定都基于**名字**（类名 / 方法名），而名字会跨类、跨文件、跨单元碰撞 ——
#   已被证明三种形态全失败（覆盖率表 §8.1 偏严、§9.2 偏松且错、§9.2③ 过稀）。
#   本脚本改用**数字**：J242 起我在每个 core 文件的文档注释里**刻意记录了**
#   `ObjMon.pas` 的精确行号区间（如 `Create（1838-1842，**五行**）`、
#   `MagicAttackTarget(): Boolean`（6261-6368）），并把该类声明写在文档里。
#   于是归属可以这样算：
#     1. 把 ObjMon.pas 的类声明行排序，得到每个类占用的**行区间** [L_i, L_{i+1})
#     2. 对每个 core 文件，从它的 /// 行里抽出所有落在 [1, 9502] 的整数
#     3. 统计该文件的数字落在各个类区间里的个数
#     4. 某类归属到"在该区间内命中数字最多且唯一最多"的文件
#   —— 数字不会因为散文提到别的类而漂移；一个文件引用另一个类的行号
#      只有在它真的移植了那个类时才会发生（否则没有理由写那些行号）。
#
# 校验：A 全集（55 类都被归属）、B 唯一（无类被两个文件认领）、
#       C 单调（每个文件的数字应集中、不应横跨大量不相关类区间）

[CmdletBinding()]
param(
    [string]$ProjectRoot = (Split-Path -Parent $PSScriptRoot),
    [string]$RepoRoot = (Split-Path -Parent (Split-Path -Parent $PSScriptRoot)),
    [string]$PascalFile = '_analysis/utf8_mirror/M2Engine/ObjMon.pas',
    [string]$SrcDir = 'src/GXX.M2Server',
    [string]$ManifestPath = 'docs/ObjMon-manifest.tsv',
    [int]$MinHits = 2,
    [switch]$WriteManifest,
    [switch]$ShowDetail
)

$ErrorActionPreference = 'Stop'

$pasPath = Join-Path $RepoRoot $PascalFile
$srcPath = Join-Path $ProjectRoot $SrcDir
$manPath = Join-Path $ProjectRoot $ManifestPath

# ---------- 1. 类与行区间 ----------
$pasLines = Get-Content $pasPath -Encoding UTF8
$decl = [ordered]@{}
for ($i = 0; $i -lt $pasLines.Count; $i++) {
    if ($pasLines[$i] -match '^\s*(T[A-Za-z0-9_]+)\s*=\s*class\b') {
        $n = $Matches[1]
        if (-not $decl.Contains($n)) { $decl[$n] = ($i + 1) }
    }
}
$all = @($decl.Keys)
# 按声明行排序，构建半开区间
$ordered = @($all | Sort-Object { $decl[$_] })
$lo = @{}; $hi = @{}
for ($i = 0; $i -lt $ordered.Count; $i++) {
    $lo[$ordered[$i]] = $decl[$ordered[$i]]
    if ($i -lt $ordered.Count - 1) { $hi[$ordered[$i]] = $decl[$ordered[$i + 1]] - 1 }
    else { $hi[$ordered[$i]] = 9502 }
}

# ---------- 2. 每个 core 文件里 /// 行上的行号 ----------
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

# ---------- 3. 归属 ----------
$owner = @{}
$scoresOf = @{}
$unresolved = @()

foreach ($c in $all) {
    $scores = @()
    foreach ($fn in $fileNums.Keys) {
        $s = 0
        foreach ($v in $fileNums[$fn]) { if ($v -ge $lo[$c] -and $v -le $hi[$c]) { $s++ } }
        if ($s -ge $MinHits) { $scores += [pscustomobject]@{ File = $fn; Score = $s } }
    }
    if ($scores.Count -eq 0) { $unresolved += $c; continue }
    $top = ($scores | Sort-Object Score -Descending)[0].Score
    $win = @($scores | Where-Object { $_.Score -eq $top })
    if ($win.Count -ne 1) { $unresolved += $c; continue }
    $owner[$c] = $win[0].File
    $scoresOf[$c] = $win[0].Score
}

# ---------- 4. 校验 ----------
$perFile = @{}
foreach ($c in $owner.Keys) {
    $fn = $owner[$c]
    if (-not $perFile.ContainsKey($fn)) { $perFile[$fn] = @() }
    $perFile[$fn] += $c
}
# "唯一"在本结构下天然成立（每类只写一次 owner）；
# 真正的风险是**同一文件认领了本不属于它的类** => 用 C 单调性代理：
#   一个文件认领的类的区间应当集中在它的数字分布范围内
$spread = @()
foreach ($fn in $perFile.Keys) {
    $ls = @($perFile[$fn] | ForEach-Object { $lo[$_] })
    if ($ls.Count -gt 1) {
        $span = ($ls | Measure-Object -Maximum).Maximum - ($ls | Measure-Object -Minimum).Minimum
        if ($span -gt 3000) { $spread += [pscustomobject]@{ File = $fn; Span = $span; Classes = ($perFile[$fn] -join ',') } }
    }
}
$missing = @($all | Where-Object { -not $owner.ContainsKey($_) })

Write-Output "======== ObjMon 归属判定（行号区间法）========"
Write-Output ("ObjMon 类数    : {0}   文件数: {1}" -f $all.Count, $files.Count)
Write-Output ("MinHits        : {0}" -f $MinHits)
Write-Output ("已归属 / 未归属: {0} / {1}" -f $owner.Count, $missing.Count)
Write-Output ("  A 全集 : {0}" -f $(if ($missing.Count -eq 0) { "通过" } else { "失败" }))
Write-Output ("  B 唯一 : 天然成立（每类单一 owner）")
Write-Output ("  C 单调 : 跨度过大（>3000 行）的文件 {0} 个 => {1}" -f $spread.Count, $(if ($spread.Count -eq 0) { "通过" } else { "需复核" }))
Write-Output ""

if ($missing.Count -gt 0) {
    Write-Output "--- 未归属 ---"
    foreach ($c in $missing) { Write-Output ("  {0,-34} [{1}-{2}]" -f $c, $lo[$c], $hi[$c]) }
    Write-Output ""
}
if ($spread.Count -gt 0) {
    Write-Output "--- 跨度异常（可能把不相干的类认领进来）---"
    foreach ($s in $spread) { Write-Output ("  {0,-36} span={1}  {2}" -f $s.File, $s.Span, $s.Classes) }
    Write-Output ""
}

Write-Output "--- 归属明细 ---"
foreach ($fn in ($perFile.Keys | Sort-Object)) {
    Write-Output ("  {0}" -f $fn)
    foreach ($c in ($perFile[$fn] | Sort-Object { $lo[$_] })) {
        Write-Output ("      {0,-34} [{1,5}-{2,5}] 命中 {3}" -f $c, $lo[$c], $hi[$c], $scoresOf[$c])
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
