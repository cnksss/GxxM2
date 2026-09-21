# tools/gen-objmon-manifest.ps1
# ObjMon 移植清单：**显式声明**的生成 + 校验（批次J251）
#
# 为什么要有这个脚本（承接 J240 的结论与 J250 的复测）：
#   J240：三种"类名共现"启发式全失败 —— 共现 != 归属，修法是"显式结构化状态字段"。
#   J250：独立复测又证伪了两个变体 ——
#         J241 原判据（头 4 行 ///）偏严（对 55 个类给出 46/9，9 个全是假阴性）；
#         新写的方法级判据偏松（对未移植的 AxeMon.pas 也判出 43/52）。
#   故本脚本把归属**落成显式数据**：
#     1) 先按"全部 /// 行里的 ObjMon 类名"做**一次性推断**（bootstrap）
#     2) 把一个固定格式的声明行注入每个文件的文档注释：
#            /// 移植类：TClass@Pascal行, TClass2@Pascal行
#     3) 此后校验**只读这一行**，不再看散文 —— 既不看排版、也不靠通用名
#     4) 输出 docs/ObjMon-manifest.tsv 作为**可评审的清单**
#
# 校验的不变量（与散文无关、可独立复核）：
#   A. **全集**：ObjMon.pas 的每个类都被声明（无遗漏）
#   B. **唯一**：每个类只被一个文件声明（无重复归属）
#   C. **存在**：声明里的行号必须与 ObjMon.pas 里的声明行一致（无杜撰）

[CmdletBinding()]
param(
    [string]$ProjectRoot = (Split-Path -Parent $PSScriptRoot),
    [string]$RepoRoot = (Split-Path -Parent (Split-Path -Parent $PSScriptRoot)),
    [string]$PascalFile = '_analysis/utf8_mirror/M2Engine/ObjMon.pas',
    [string]$SrcDir = 'src/GXX.M2Server',
    [string]$ManifestPath = 'docs/ObjMon-manifest.tsv',
    [switch]$Inject,
    [switch]$Report
)

$ErrorActionPreference = 'Stop'

$pasPath = Join-Path $RepoRoot $PascalFile
$srcPath = Join-Path $ProjectRoot $SrcDir
$manPath = Join-Path $ProjectRoot $ManifestPath

# ---------- 1. ObjMon.pas 的类与声明行 ----------
$pasLines = Get-Content $pasPath -Encoding UTF8
$classLine = [ordered]@{}
for ($i = 0; $i -lt $pasLines.Count; $i++) {
    if ($pasLines[$i] -match '^\s*(T[A-Za-z0-9_]+)\s*=\s*class\b') {
        $n = $Matches[1]
        if (-not $classLine.Contains($n)) { $classLine[$n] = ($i + 1) }
    }
}
$allClasses = @($classLine.Keys)

# ---------- 2. bootstrap：按"全部 /// 行"推断归属 ----------
$files = Get-ChildItem -Path $srcPath -Filter 'ObjMon*Core.cs' -File
$inferred = @{}     # class -> file name
$dupes = @{}        # class -> list of files

foreach ($f in $files) {
    $lines = Get-Content $f.FullName -Encoding UTF8
    $blob = (($lines | Where-Object { $_ -match '^\s*///' }) -join "`n")
    foreach ($cls in $allClasses) {
        if ($blob -match ("(?<![A-Za-z0-9_])" + [regex]::Escape($cls) + "(?![A-Za-z0-9_])")) {
            if ($inferred.ContainsKey($cls)) {
                if (-not $dupes.ContainsKey($cls)) { $dupes[$cls] = @($inferred[$cls]) }
                $dupes[$cls] += $f.Name
            } else {
                $inferred[$cls] = $f.Name
            }
        }
    }
}

# ---------- 3. 注入显式声明行 ----------
$declPrefix = '/// 移植类：'
$injected = 0
$skipped = 0

if ($Inject) {
    # 先算每个文件要声明的类（含重复的 —— 重复也应被显式写出来，交给校验去报错）
    $perFile = @{}
    foreach ($f in $files) { $perFile[$f.Name] = @() }
    foreach ($cls in $allClasses) {
        if ($inferred.ContainsKey($cls)) { $perFile[$inferred[$cls]] += $cls }
        if ($dupes.ContainsKey($cls)) {
            foreach ($dn in $dupes[$cls]) { if ($perFile.ContainsKey($dn)) { $perFile[$dn] += $cls } }
        }
    }

    foreach ($f in $files) {
        $lines = New-Object System.Collections.Generic.List[string]
        foreach ($x in (Get-Content $f.FullName -Encoding UTF8)) { $lines.Add($x) }

        $targets = @($perFile[$f.Name] | Sort-Object -Unique)
        if ($targets.Count -eq 0) { $skipped++; continue }

        $parts = @()
        foreach ($t in $targets) { $parts += ("{0}@{1}" -f $t, $classLine[$t]) }
        $newDecl = $declPrefix + ($parts -join ', ')

        # 已存在同前缀行 => 替换；否则插到第一行 /// <summary> 之后
        $existing = -1
        for ($i = 0; $i -lt $lines.Count; $i++) {
            if ($lines[$i].TrimStart().StartsWith($declPrefix)) { $existing = $i; break }
        }
        if ($existing -ge 0) {
            if ($lines[$existing] -eq $newDecl) { $skipped++; continue }
            $lines[$existing] = $newDecl
        } else {
            $pos = -1
            for ($i = 0; $i -lt $lines.Count; $i++) {
                if ($lines[$i] -match '^\s*///\s*<summary>') { $pos = $i + 1; break }
            }
            if ($pos -lt 0) {
                # 没有 <summary>：插到第一个 /// 行之前
                for ($i = 0; $i -lt $lines.Count; $i++) {
                    if ($lines[$i] -match '^\s*///') { $pos = $i; break }
                }
            }
            if ($pos -lt 0) { $skipped++; continue }
            $lines.Insert($pos, $newDecl)
        }

        $text = ($lines -join "`n") + "`n"
        [System.IO.File]::WriteAllText($f.FullName, $text, (New-Object System.Text.UTF8Encoding $false))
        $injected++
    }
}

# ---------- 4. 校验：**只读声明行** ----------
$declared = @{}     # class -> file
$declaredDupes = @{}
$malformed = @()

foreach ($f in $files) {
    foreach ($ln in (Get-Content $f.FullName -Encoding UTF8)) {
        if (-not $ln.TrimStart().StartsWith($declPrefix)) { continue }
        $body = $ln.Substring($ln.IndexOf($declPrefix) + $declPrefix.Length)
        foreach ($item in ($body -split ',')) {
            $t = $item.Trim()
            if ($t -eq '') { continue }
            if ($t -notmatch '^(T[A-Za-z0-9_]+)@(\d+)$') { $malformed += ("{0}: {1}" -f $f.Name, $t); continue }
            $cn = $Matches[1]; $cl = [int]$Matches[2]
            if (-not $classLine.Contains($cn)) { $malformed += ("{0}: 未知类 {1}" -f $f.Name, $cn); continue }
            if ($classLine[$cn] -ne $cl) { $malformed += ("{0}: {1} 行号不符（声明 {2}、实际 {3}）" -f $f.Name, $cn, $cl, $classLine[$cn]); continue }
            if ($declared.ContainsKey($cn)) {
                if (-not $declaredDupes.ContainsKey($cn)) { $declaredDupes[$cn] = @($declared[$cn]) }
                $declaredDupes[$cn] += $f.Name
            } else {
                $declared[$cn] = $f.Name
            }
        }
    }
}

$missing = @($allClasses | Where-Object { -not $declared.ContainsKey($_) })

Write-Output "======== ObjMon 移植清单：显式声明 + 校验 ========"
Write-Output ("ObjMon.pas 类数        : {0}" -f $allClasses.Count)
Write-Output ("ObjMon*Core.cs 文件数  : {0}" -f $files.Count)
Write-Output ("bootstrap 推断到归属   : {0}" -f $inferred.Count)
if ($Inject) { Write-Output ("本次注入/更新声明行    : {0} 文件（跳过 {1}）" -f $injected, $skipped) }
Write-Output ""
Write-Output "--- 不变量校验（只读声明行）---"
Write-Output ("  A 全集 : 已声明 {0} / {1}  => {2}" -f $declared.Count, $allClasses.Count, $(if ($missing.Count -eq 0) { "通过" } else { "失败" }))
Write-Output ("  B 唯一 : 重复归属 {0} 个       => {1}" -f $declaredDupes.Count, $(if ($declaredDupes.Count -eq 0) { "通过" } else { "失败" }))
Write-Output ("  C 存在 : 格式/行号问题 {0} 条  => {1}" -f $malformed.Count, $(if ($malformed.Count -eq 0) { "通过" } else { "失败" }))
Write-Output ""

if ($missing.Count -gt 0) {
    Write-Output "--- 未声明（A 失败）---"
    foreach ($m in $missing) { Write-Output ("  {0,-34} @ ObjMon.pas:{1}" -f $m, $classLine[$m]) }
    Write-Output ""
}
if ($declaredDupes.Count -gt 0) {
    Write-Output "--- 重复归属（B 失败）---"
    foreach ($k in $declaredDupes.Keys) { Write-Output ("  {0,-34} -> {1}" -f $k, ($declaredDupes[$k] -join ', ')) }
    Write-Output ""
}
if ($malformed.Count -gt 0) {
    Write-Output "--- 格式/行号问题（C 失败）---"
    foreach ($m in $malformed) { Write-Output ("  " + $m) }
    Write-Output ""
}

# ---------- 5. 输出清单 ----------
if ($Report -or $Inject) {
    $rows = @()
    foreach ($cls in ($allClasses | Sort-Object)) {
        $w = ''
        if ($declared.ContainsKey($cls)) { $w = $declared[$cls] }
        $rows += ("{0}`t{1}`t{2}" -f $cls, $classLine[$cls], $w)
    }
    $header = "class`tpascal_line`tcore_file"
    $content = ($header + "`n" + ($rows -join "`n") + "`n")
    [System.IO.File]::WriteAllText($manPath, $content, (New-Object System.Text.UTF8Encoding $false))
    Write-Output ("清单已写出：{0}（{1} 行）" -f $ManifestPath, ($rows.Count + 1))
}

Write-Output "======== 结束 ========"
