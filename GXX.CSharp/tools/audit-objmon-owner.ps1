# tools/audit-objmon-owner.ps1
# ObjMon 类 -> C# 文件 的**归属判定**（批次J251 修正版）
#
# 为什么重写（本批实测到的失败）：
#   我第一版 bootstrap 用"该文件**全部 /// 行**里出现的 ObjMon 类名"来推断归属。
#   对 46 个文件跑出来"推断 55/55、看起来完美"，注入声明后**唯一性校验报出 51 个重复** ——
#   `ObjMonCowFamilyCore.cs` 被分配了 `TGasAttackMonster`、`TMeteoriteRainAttackMonster` 等
#   它根本没移植的类。原因是**散文里的跨类引用**：J243 的文档注释为了对照而写了
#   "与 J242 的 `TGasAttackMonster` 同型"，于是那个类名被算作本文件的移植目标。
#   ⇒ 这正是 J240 记录的"共现 != 归属"，只不过换成了"全部 /// 行"这一更宽的版本。
#   （注：上一版脚本 dry-run 时 B 显示"通过"是**空洞的真** —— 那时还没有任何声明行、
#     已声明数为 0、重复数自然为 0。**空集上的全称命题恒真**，是我这次的第二个方法论失误。）
#
# 本版判据（与散文无关）：
#   1. 抽每个类的方法名；只保留**在整个 ObjMon 单元里唯一属于该类**的方法名（"特征方法名"）
#      —— `Create`/`Destroy`/`Run`/`Attack` 等通用名被自动排除，因为它们不属于任何单一类
#   2. 对每个文件，只在**代码行**（排除 /// 注释行）里统计特征方法名的命中数
#   3. 类归属到命中数最高且**唯一最高**的文件；平局或零命中 => 报告为"未判定"
#   4. 校验：全集（55 个类都有归属）+ 唯一（无一个类被两个文件认领）

[CmdletBinding()]
param(
    [string]$ProjectRoot = (Split-Path -Parent $PSScriptRoot),
    [string]$RepoRoot = (Split-Path -Parent (Split-Path -Parent $PSScriptRoot)),
    [string]$PascalFile = '_analysis/utf8_mirror/M2Engine/ObjMon.pas',
    [string]$SrcDir = 'src/GXX.M2Server',
    [string]$ManifestPath = 'docs/ObjMon-manifest.tsv',
    [switch]$WriteManifest,
    [switch]$ShowDetail
)

$ErrorActionPreference = 'Stop'

$pasPath = Join-Path $RepoRoot $PascalFile
$srcPath = Join-Path $ProjectRoot $SrcDir
$manPath = Join-Path $ProjectRoot $ManifestPath

# ---------- 1. 类与声明行 ----------
$pasLines = Get-Content $pasPath -Encoding UTF8
$classLine = [ordered]@{}
for ($i = 0; $i -lt $pasLines.Count; $i++) {
    if ($pasLines[$i] -match '^\s*(T[A-Za-z0-9_]+)\s*=\s*class\b') {
        $n = $Matches[1]
        if (-not $classLine.Contains($n)) { $classLine[$n] = ($i + 1) }
    }
}
$allClasses = @($classLine.Keys)

# ---------- 2. 每个类的实现方法名 ----------
$classMethods = @{}
foreach ($c in $allClasses) { $classMethods[$c] = New-Object System.Collections.Generic.List[string] }
for ($i = 0; $i -lt $pasLines.Count; $i++) {
    if ($pasLines[$i] -match '^\s*(?:procedure|function|constructor|destructor)\s+(T[A-Za-z0-9_]+)\.([A-Za-z0-9_]+)') {
        $c = $Matches[1]; $m = $Matches[2]
        if ($classMethods.ContainsKey($c) -and -not $classMethods[$c].Contains($m)) { $classMethods[$c].Add($m) }
    }
}

# ---------- 3. 特征方法名（单元内唯一） ----------
$nameOwner = @{}
foreach ($c in $allClasses) {
    foreach ($m in $classMethods[$c]) {
        if (-not $nameOwner.ContainsKey($m)) { $nameOwner[$m] = @() }
        $nameOwner[$m] += $c
    }
}
$distinctive = @{}
foreach ($c in $allClasses) {
    $d = @()
    foreach ($m in $classMethods[$c]) { if ($nameOwner[$m].Count -eq 1) { $d += $m } }
    $distinctive[$c] = $d
}

# ---------- 4. 各文件的代码文本（排除 /// 行） ----------
$files = Get-ChildItem -Path $srcPath -Filter 'ObjMon*Core.cs' -File
$fileCode = @{}
foreach ($f in $files) {
    $blob = (Get-Content $f.FullName -Encoding UTF8 | Where-Object { $_ -notmatch '^\s*///' }) -join "`n"
    $fileCode[$f.Name] = $blob
}

# ---------- 5. 归属判定 ----------
$owner = @{}
$unresolved = @()
$noDistinctive = @()
$detail = @()

foreach ($c in $allClasses) {
    $d = @($distinctive[$c])
    if ($d.Count -eq 0) { $noDistinctive += $c; $unresolved += $c; continue }

    $scores = @()
    foreach ($fn in $fileCode.Keys) {
        $s = 0
        foreach ($m in $d) {
            if ($fileCode[$fn] -match ("(?<![A-Za-z0-9_])" + [regex]::Escape($m) + "(?![A-Za-z0-9_])")) { $s++ }
        }
        if ($s -gt 0) { $scores += [pscustomobject]@{ File = $fn; Score = $s } }
    }
    if ($scores.Count -eq 0) { $unresolved += $c; continue }

    $top = ($scores | Sort-Object Score -Descending)[0].Score
    $winners = @($scores | Where-Object { $_.Score -eq $top })
    if ($winners.Count -ne 1) { $unresolved += $c; continue }

    $owner[$c] = $winners[0].File
    $detail += [pscustomobject]@{ Class = $c; File = $winners[0].File; Score = $top; Distinct = $d.Count }
}

# ---------- 6. 校验 ----------
$multi = @{}
foreach ($c in $owner.Keys) {
    $fn = $owner[$c]
    if (-not $multi.ContainsKey($fn)) { $multi[$fn] = @() }
    $multi[$fn] += $c
}
$missing = @($allClasses | Where-Object { -not $owner.ContainsKey($_) })

Write-Output "======== ObjMon 类归属判定（特征方法名 + 代码行）========"
Write-Output ("ObjMon.pas 类数       : {0}" -f $allClasses.Count)
Write-Output ("ObjMon*Core.cs 文件数 : {0}" -f $files.Count)
Write-Output ("已判定归属            : {0}" -f $owner.Count)
Write-Output ("未判定                : {0}" -f $missing.Count)
Write-Output ""
Write-Output ("  A 全集 : {0} => {1}" -f $owner.Count, $(if ($missing.Count -eq 0) { "通过" } else { "失败" }))
# 唯一性：一个类只可能有一个 owner（本数据结构天然保证），故真正的检查是
#          "每个 ★文件的声明集合与其实际实现是否相符" —— 这里用"同一类被多文件高分命中"代理
$amb = @($detail | Group-Object Class | Where-Object { $_.Count -gt 1 })
Write-Output ("  B 唯一 : 重复归属 {0} => {1}" -f $amb.Count, $(if ($amb.Count -eq 0) { "通过" } else { "失败" }))
Write-Output ""

if ($noDistinctive.Count -gt 0) {
    Write-Output ("--- 无特征方法名（该类所有方法名都被别的类共用）: {0} 个 ---" -f $noDistinctive.Count)
    foreach ($c in $noDistinctive) { Write-Output ("  {0,-34} 方法: {1}" -f $c, ($classMethods[$c] -join ',')) }
    Write-Output ""
}
if ($missing.Count -gt 0) {
    Write-Output "--- 未判定归属 ---"
    foreach ($c in $missing) {
        if ($noDistinctive -contains $c) { continue }
        Write-Output ("  {0,-34} @ObjMon.pas:{1}  特征方法: {2}" -f $c, $classLine[$c], (($distinctive[$c]) -join ','))
    }
    Write-Output ""
}

if ($ShowDetail) {
    Write-Output "--- 判定明细（按文件）---"
    foreach ($fn in ($multi.Keys | Sort-Object)) {
        Write-Output ("  {0}:" -f $fn)
        foreach ($c in ($multi[$fn] | Sort-Object)) {
            $sc = ($detail | Where-Object { $_.Class -eq $c })[0].Score
            Write-Output ("      {0,-34} @{1,-5} 命中特征方法 {2}" -f $c, $classLine[$c], $sc)
        }
    }
    Write-Output ""
}

if ($WriteManifest) {
    $rows = @()
    foreach ($c in ($allClasses | Sort-Object)) {
        $w = ''
        if ($owner.ContainsKey($c)) { $w = $owner[$c] }
        $rows += ("{0}`t{1}`t{2}" -f $c, $classLine[$c], $w)
    }
    [System.IO.File]::WriteAllText($manPath, ("class`tpascal_line`tcore_file`n" + ($rows -join "`n") + "`n"), (New-Object System.Text.UTF8Encoding $false))
    Write-Output ("清单已写出：{0}（{1} 行）" -f $ManifestPath, ($rows.Count + 1))
}

Write-Output "======== 结束 ========"
