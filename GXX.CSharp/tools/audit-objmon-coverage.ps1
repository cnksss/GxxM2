# tools/audit-objmon-coverage.ps1
# ObjMon.pas 类级覆盖率**独立复测**脚本（批次J250 重建）
#
# 背景：批次J241 曾用"每个 C# 文件头 4 行 /// 注释里出现的 Delphi 类名"
# 作为判据生成 docs/ObjMon-覆盖率表.md，但**当时的生成脚本没有落盘**
# （覆盖率表 §7 记录了生成器的排版缺陷、却没留下脚本），
# 故本脚本是**按该判据重新实现**的版本，用于独立复核"未移植清零"这一结论。
#
# 判据（与 J241 一致）：
#   1. 从 ObjMon.pas 抽出所有 `X = class(...)` 的类名（去重后为"真实类集合"）
#   2. 对每个 C# 文件，只看**头 4 行 /// 注释**，其中出现的 T* 类名即该文件的移植目标
#   3. ObjMon 的类若不在任何文件的头 4 行里 => 记为"未移植"
#
# 已知的判据残余误差方向（J241 §6 实测）：**把已移植的误报成未移植**（安全方向）
#   —— 例：TDevilBat(521)、TDevilkingMonster(505) 实为已移植却曾被误报。
#   故本脚本的输出应视为"待人工核对的候选"，而非终审判决。

[CmdletBinding()]
param(
    # 工程根 = tools 的上一级（即 GXX.CSharp）
    [string]$ProjectRoot = (Split-Path -Parent $PSScriptRoot),
    # 仓库根 = 工程根的上一级（Pascal 源镜像在这里）
    [string]$RepoRoot = (Split-Path -Parent (Split-Path -Parent $PSScriptRoot)),
    [string]$PascalFile = '_analysis/utf8_mirror/M2Engine/ObjMon.pas',
    [string]$SrcDir = 'src',
    [int]$HeadCommentLines = 4
)

$ErrorActionPreference = 'Stop'

$pasPath = Join-Path $RepoRoot $PascalFile
$srcPath = Join-Path $ProjectRoot $SrcDir

if (-not (Test-Path $pasPath)) { throw "找不到 Pascal 源：$pasPath" }
if (-not (Test-Path $srcPath)) { throw "找不到源码目录：$srcPath" }

# ---------- 1. 抽 ObjMon.pas 的类声明 ----------
$pasLines = Get-Content $pasPath -Encoding UTF8

# 记录每个类名首次出现的行号
$classFirstLine = @{}
$classAllLines = @{}

for ($i = 0; $i -lt $pasLines.Count; $i++) {
    $line = $pasLines[$i]
    # 形如：  TFoo = class(TBar)   /  TFoo = class(TBar) // 注释
    if ($line -match '^\s*(T[A-Za-z0-9_]+)\s*=\s*class\b') {
        $name = $Matches[1]
        if (-not $classFirstLine.ContainsKey($name)) {
            $classFirstLine[$name] = ($i + 1)
        }
        if (-not $classAllLines.ContainsKey($name)) { $classAllLines[$name] = @() }
        $classAllLines[$name] += ($i + 1)
    }
}

$allClasses = @($classFirstLine.Keys | Sort-Object)

# 重复声明的类（J239 形态⑩：同名定义两次，首次在块注释里）
$dupClasses = @()
foreach ($k in $classAllLines.Keys) {
    if ($classAllLines[$k].Count -gt 1) {
        $dupClasses += ("{0} @ {1}" -f $k, ($classAllLines[$k] -join ','))
    }
}

# ---------- 2. 抽每个 C# 文件头 N 行 /// 里的类名 ----------
$csFiles = Get-ChildItem -Path $srcPath -Recurse -Filter '*.cs' -File |
    Where-Object { $_.FullName -notmatch '\\(obj|bin)\\' }

$fileTargets = @{}          # 文件名 -> 命中类名集合
$classToFiles = @{}         # 类名 -> 命中它的文件列表

foreach ($f in $csFiles) {
    $lines = Get-Content $f.FullName -Encoding UTF8
    $head = @()
    foreach ($ln in $lines) {
        if ($ln -match '^\s*///') {
            $head += $ln
            if ($head.Count -ge $HeadCommentLines) { break }
        }
    }
    if ($head.Count -eq 0) { continue }

    $blob = ($head -join "`n")
    $hits = @()
    foreach ($cls in $allClasses) {
        # 用词边界，避免 TFoo 命中 TFooBar
        if ($blob -match ("(?<![A-Za-z0-9_])" + [regex]::Escape($cls) + "(?![A-Za-z0-9_])")) {
            $hits += $cls
        }
    }
    if ($hits.Count -gt 0) {
        $fileTargets[$f.Name] = $hits
        foreach ($h in $hits) {
            if (-not $classToFiles.ContainsKey($h)) { $classToFiles[$h] = @() }
            $classToFiles[$h] += $f.Name
        }
    }
}

# ---------- 3. 求差集 ----------
$ported = @($allClasses | Where-Object { $classToFiles.ContainsKey($_) })
$pending = @($allClasses | Where-Object { -not $classToFiles.ContainsKey($_) })

# ---------- 4. 输出 ----------
Write-Output "================ ObjMon 类级覆盖率独立复测 ================"
Write-Output ("源文件      : {0}" -f $PascalFile)
Write-Output ("扫描 C# 文件: {0} 个（{1}/**，排除 obj/bin）" -f $csFiles.Count, $SrcDir)
Write-Output ("判据        : 每个 C# 文件头 {0} 行 /// 注释里出现的 Delphi 类名" -f $HeadCommentLines)
Write-Output ""
Write-Output ("ObjMon.pas 类声明（去重）: {0}" -f $allClasses.Count)
Write-Output ("  其中被判为**已移植**: {0}" -f $ported.Count)
Write-Output ("  其中被判为**未移植**: {0}" -f $pending.Count)
Write-Output ""

if ($dupClasses.Count -gt 0) {
    Write-Output "--- 重复声明的类（形态⑩）---"
    foreach ($d in $dupClasses) { Write-Output ("  " + $d) }
    Write-Output ""
}

if ($pending.Count -gt 0) {
    Write-Output "--- 未移植（**候选**，需人工核对；判据已知会误报已移植者）---"
    foreach ($p in $pending) {
        Write-Output ("  {0,-34} @ ObjMon.pas:{1}" -f $p, $classFirstLine[$p])
    }
    Write-Output ""
}

Write-Output "--- 已移植明细（类 -> 命中文件）---"
foreach ($p in $ported) {
    Write-Output ("  {0,-34} -> {1}" -f $p, (($classToFiles[$p] | Sort-Object -Unique) -join ', '))
}
Write-Output ""
Write-Output "================ 复测结束 ================"
