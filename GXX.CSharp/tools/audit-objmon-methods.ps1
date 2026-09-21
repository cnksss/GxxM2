# tools/audit-objmon-methods.ps1
# ObjMon.pas 类级覆盖率**方法级**复核（批次J250 新增）
#
# 为什么需要这个脚本：
#   J241 的判据（"每个 C# 文件头 4 行 /// 注释里出现的类名"）在批次J250 独立复测时
#   给出 46/55、并标出 9 个"未移植" —— 但经人工核对，**这 9 个全部是已移植的**
#   （J242-J249 各批都实现过），即判据**偏向"误报未移植"**。
#   把阈值从 4 放宽到 40 行会得到 55/0，但"调阈值直到得到想要的答案"正是 J240 记录过的陷阱
#   （共现 != 归属）。故本脚本改用**与文字排版无关**的证据：
#     一个类被视为"已移植"，当且仅当它的**方法名**在 C# 侧以标识符形式出现达到阈值。
#   方法名远比类名具体：类名会被跨类引用误伤，方法名不会。
#
# 判据：
#   1. 从 ObjMon.pas 抽 `Class.Method` 形式的实现头（procedure/function/constructor/destructor）
#      —— 只取**实现区**（第二个 `implementation` 之后本文件里就是全文的 `X.Y` 形式）
#   2. 每个类得到自己的方法名集合
#   3. 在 C# 侧统计每个方法名是否作为**独立标识符**出现（词边界匹配）
#   4. 类的方法命中率 >= 阈值（默认 0.6）= 已移植

[CmdletBinding()]
param(
    [string]$ProjectRoot = (Split-Path -Parent $PSScriptRoot),
    [string]$RepoRoot = (Split-Path -Parent (Split-Path -Parent $PSScriptRoot)),
    [string]$PascalFile = '_analysis/utf8_mirror/M2Engine/ObjMon.pas',
    [string]$SrcDir = 'src',
    [double]$HitRatio = 0.6,
    [switch]$ShowPorted
)

$ErrorActionPreference = 'Stop'

$pasPath = Join-Path $RepoRoot $PascalFile
$srcPath = Join-Path $ProjectRoot $SrcDir

if (-not (Test-Path $pasPath)) { throw "找不到 Pascal 源：$pasPath" }
if (-not (Test-Path $srcPath)) { throw "找不到源码目录：$srcPath" }

# ---------- 1. 类声明 ----------
$pasLines = Get-Content $pasPath -Encoding UTF8
$classFirstLine = @{}
for ($i = 0; $i -lt $pasLines.Count; $i++) {
    if ($pasLines[$i] -match '^\s*(T[A-Za-z0-9_]+)\s*=\s*class\b') {
        $n = $Matches[1]
        if (-not $classFirstLine.ContainsKey($n)) { $classFirstLine[$n] = ($i + 1) }
    }
}
$allClasses = @($classFirstLine.Keys | Sort-Object)

# ---------- 2. 每个类的实现方法名 ----------
$classMethods = @{}
foreach ($cls in $allClasses) { $classMethods[$cls] = New-Object System.Collections.Generic.HashSet[string] }

for ($i = 0; $i -lt $pasLines.Count; $i++) {
    # 形如：procedure TFoo.Bar(...)   /   function TFoo.Bar: Boolean;
    if ($pasLines[$i] -match '^\s*(?:procedure|function|constructor|destructor)\s+(T[A-Za-z0-9_]+)\.([A-Za-z0-9_]+)') {
        $cls = $Matches[1]
        $mth = $Matches[2]
        if ($classMethods.ContainsKey($cls)) { [void]$classMethods[$cls].Add($mth) }
    }
}

# ---------- 3. C# 侧标识符集合 ----------
$csFiles = Get-ChildItem -Path $srcPath -Recurse -Filter '*.cs' -File |
    Where-Object { $_.FullName -notmatch '\\(obj|bin)\\' }

$csText = New-Object System.Text.StringBuilder
foreach ($f in $csFiles) {
    [void]$csText.AppendLine((Get-Content $f.FullName -Encoding UTF8 -Raw))
}
$blob = $csText.ToString()

# ---------- 4. 逐类统计 ----------
$rows = @()
foreach ($cls in $allClasses) {
    $ms = @($classMethods[$cls])
    if ($ms.Count -eq 0) {
        $rows += [pscustomobject]@{ Class = $cls; Line = $classFirstLine[$cls]; Methods = 0; Hits = 0; Ratio = 0.0; Missing = '' }
        continue
    }
    $hits = 0
    $miss = @()
    foreach ($m in $ms) {
        if ($blob -match ("(?<![A-Za-z0-9_])" + [regex]::Escape($m) + "(?![A-Za-z0-9_])")) { $hits++ }
        else { $miss += $m }
    }
    $ratio = [math]::Round($hits / $ms.Count, 3)
    $rows += [pscustomobject]@{
        Class = $cls; Line = $classFirstLine[$cls]; Methods = $ms.Count
        Hits = $hits; Ratio = $ratio; Missing = ($miss -join ',')
    }
}

$ported = @($rows | Where-Object { $_.Methods -gt 0 -and $_.Ratio -ge $HitRatio })
$pending = @($rows | Where-Object { $_.Methods -eq 0 -or $_.Ratio -lt $HitRatio })

Write-Output "======== ObjMon 类级覆盖率**方法级**复核 ========"
Write-Output ("源          : {0}" -f $PascalFile)
Write-Output ("C# 文件数   : {0}" -f $csFiles.Count)
Write-Output ("判据        : 类的方法名在 C# 侧作为独立标识符出现的比例 >= {0}" -f $HitRatio)
Write-Output ""
Write-Output ("ObjMon 类数 : {0}" -f $allClasses.Count)
Write-Output ("  已移植    : {0}" -f $ported.Count)
Write-Output ("  未移植    : {0}" -f $pending.Count)
Write-Output ""

if ($pending.Count -gt 0) {
    Write-Output "--- 未移植（方法级判定）---"
    foreach ($r in ($pending | Sort-Object Ratio, Class)) {
        Write-Output ("  {0,-34} @{1,-5} 方法 {2,3} 命中 {3,3} ({4})  缺: {5}" -f `
            $r.Class, $r.Line, $r.Methods, $r.Hits, $r.Ratio, $r.Missing)
    }
    Write-Output ""
}

if ($ShowPorted) {
    Write-Output "--- 已移植明细 ---"
    foreach ($r in ($ported | Sort-Object Class)) {
        Write-Output ("  {0,-34} @{1,-5} 方法 {2,3} 命中 {3,3} ({4})" -f `
            $r.Class, $r.Line, $r.Methods, $r.Hits, $r.Ratio)
    }
    Write-Output ""
}

Write-Output "======== 复核结束 ========"
