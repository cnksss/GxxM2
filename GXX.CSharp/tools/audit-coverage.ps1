# ============================================================
#  audit-coverage.ps1   (ASCII-only on purpose: Windows PowerShell 5.1 reads
#  .ps1 as ANSI, so non-ASCII literals here would break parsing.)
#
#  Delphi .pas unit  ->  C# mapping audit.
#  This is the executable form of docs/转换开发文档.md  §10 DoD item
#  "Checklist.md 中无未映射单元".
#
#  Evidence used per unit (any hit => mapped):
#    E1  a .cs file under GXX.CSharp/src or tests with the same basename
#    E2  some .cs under GXX.CSharp/src mentions "<unit>.pas" in its header doc
#    E3  docs/Checklist.md mentions the unit basename
#    E4  an explicit row in tools/unit-map.tsv
#
#  Usage:
#    .\GXX.CSharp\tools\audit-coverage.ps1                    # summary to console
#    .\GXX.CSharp\tools\audit-coverage.ps1 -Report            # also write docs/并行覆盖审计.md
#    .\GXX.CSharp\tools\audit-coverage.ps1 -Dir M2Engine      # one source dir only
# ============================================================
[CmdletBinding()]
param(
    [string[]]$Dir = @('M2Engine','Client-HGE','DBServer','LoginSrv','SelGate','RunGate','GameCenter','LogDataServer','Common'),
    [switch]$Report,
    [switch]$ShowMapped,
    # NOTE: keep the default ASCII; pass a non-ASCII path from the command line
    # instead (this file is parsed as ANSI by Windows PowerShell 5.1).
    [string]$ReportPath = ''
)

$ErrorActionPreference = 'Stop'
$repo     = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$srcRoot  = Join-Path $repo 'Source'
$csRoot   = Join-Path $repo 'GXX.CSharp'
if ($ReportPath) {
    $reportMd = if ([System.IO.Path]::IsPathRooted($ReportPath)) { $ReportPath } else { Join-Path $csRoot $ReportPath }
} else {
    $reportMd = Join-Path $csRoot 'docs\parallel-coverage-audit.md'
}

# Third-party / non-portable vendors: registered as "not ported" by design
# (docs/转换开发文档.md  §2.3). They are listed separately, never as "missing".
$VENDOR_DIRS = @('BeaEngineSource','FastMM','AliyunSDK','delphizlib.128','LockBox2','PNG','RSA','WinLicense_Inc','Bass','QRCode','PlugIn','Demo','DxComponent\GUI_Backup')
# Units that are pure vendor wrappers / IDE templates, not business code.
$VENDOR_UNITS = @('VMProtectSDK','EHookLIB','uSynHighlighterSample','plgSearchHighlighter','JClasses','DLLLoader','LuaScript','LuaEvent','LuaActor')

# ---- load optional explicit map ------------------------------------------
$explicit = @{}
$mapFile = Join-Path $PSScriptRoot 'unit-map.tsv'
if (Test-Path $mapFile) {
    foreach ($line in Get-Content $mapFile) {
        if ($line -match '^\s*#' -or $line -match '^\s*$') { continue }
        $parts = $line -split "`t"
        if ($parts.Count -ge 2) { $explicit[$parts[0].Trim()] = $parts[1].Trim() }
    }
}
Write-Host "explicit map rows: $($explicit.Count)"

# ---- index the C# side once ----------------------------------------------
$csFiles = Get-ChildItem $csRoot -Recurse -File -Filter *.cs |
           Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' }
Write-Host "cs files indexed : $($csFiles.Count)"

$csByBase = @{}
foreach ($f in $csFiles) { $csByBase[$f.BaseName.ToLowerInvariant()] = $true }

# latin1 keeps byte->char 1:1 so ASCII substring search is encoding-agnostic
#
# E2 was originally "some src .cs mentions <unit>.pas anywhere" -- that produced
# FALSE POSITIVES: an aggregate/service file whose comments merely happen to name
# the unit (e.g. GShare.pas named inside GameCenterService.cs) was scored as a
# port.  A genuine port documents its source in the FILE HEADER doc comment, so
# E2 now only looks at the first HEAD_LINES lines; a mention found later in the
# file is downgraded to WEAK (reported, never counted as mapped).
$HEAD_LINES = 40
$latin1 = [System.Text.Encoding]::GetEncoding(28591)
$csText = @{}
$csHead = @{}
foreach ($f in $csFiles) {
    if ($f.FullName -notmatch '\\src\\') { continue }
    try {
        $t = [System.IO.File]::ReadAllText($f.FullName, $latin1)
        $csText[$f.FullName] = $t
        $ls = $t -split "`n"
        if ($ls.Count -gt $HEAD_LINES) { $t = ($ls[0..($HEAD_LINES - 1)] -join "`n") }
        $csHead[$f.FullName] = $t
    } catch { }
}
Write-Host "src text loaded  : $($csText.Count)"

# Checklist is UTF-8; read it once as raw text for substring search
$checklistPath = Join-Path $csRoot 'docs\Checklist.md'
$checklistText = ''
if (Test-Path $checklistPath) {
    $checklistText = [System.IO.File]::ReadAllText($checklistPath, [System.Text.Encoding]::UTF8)
}

# ---- scan the Delphi side -------------------------------------------------
$rows = @()
foreach ($d in $Dir) {
    $dirPath = Join-Path $srcRoot $d
    if (-not (Test-Path $dirPath)) { continue }
    $files = Get-ChildItem $dirPath -Recurse -File -Filter *.pas -ErrorAction SilentlyContinue
    foreach ($f in $files) {
        $rel = $f.FullName.Substring($srcRoot.Length + 1)
        $isVendor = $false
        foreach ($v in $VENDOR_DIRS) { if ($rel -like "*\$v\*") { $isVendor = $true } }
        if ($VENDOR_UNITS -contains $f.BaseName) { $isVendor = $true }

        $unit = $f.BaseName
        $e1 = $csByBase.ContainsKey($unit.ToLowerInvariant())
        $e2 = $false
        $e2w = $false
        if (-not $e1) {
            $needle = "$unit.pas"
            foreach ($k in $csHead.Keys) { if ($csHead[$k].IndexOf($needle, [StringComparison]::Ordinal) -ge 0) { $e2 = $true; break } }
            if (-not $e2) {
                foreach ($k in $csText.Keys) { if ($csText[$k].IndexOf($needle, [StringComparison]::Ordinal) -ge 0) { $e2w = $true; break } }
            }
        }
        $e3 = $false
        if (-not ($e1 -or $e2)) { $e3 = $checklistText.IndexOf($unit, [StringComparison]::Ordinal) -ge 0 }
        $e4 = $explicit.ContainsKey($unit)

        $mapped = $e1 -or $e2
        $lines = 0
        try { $lines = (Get-Content -LiteralPath $f.FullName -Encoding Default | Measure-Object -Line).Lines } catch { }

        # E4 = the unit is explicitly assigned to a parallel lane, i.e. in flight,
        # NOT finished. It must never be counted as mapped.
        # WEAK = only a non-header mention exists: explicitly NOT a port.
        $verdict = if ($isVendor) { 'VENDOR' }
                   elseif ($mapped) { 'MAPPED' }
                   elseif ($e2w) { 'WEAK' }
                   elseif ($e4) { 'ASSIGNED' }
                   elseif ($e3) { 'CHECKLIST_ONLY' }
                   else { 'UNMAPPED' }

        $rows += [pscustomobject]@{
            Dir = $d; Unit = $unit; Lines = $lines; KB = [math]::Round($f.Length / 1KB)
            E1 = $e1; E2 = $e2; E2w = $e2w; E3 = $e3; E4 = $e4; Verdict = $verdict; Rel = $rel
        }
    }
}

# ---- summarise ------------------------------------------------------------
$byDir = $rows | Group-Object Dir | ForEach-Object {
    $g = $_.Group
    [pscustomobject]@{
        Dir         = $_.Name
        Units       = $g.Count
        Mapped      = ($g | Where-Object Verdict -eq 'MAPPED').Count
        Weak        = ($g | Where-Object Verdict -eq 'WEAK').Count
        Assigned    = ($g | Where-Object Verdict -eq 'ASSIGNED').Count
        ChecklistOn = ($g | Where-Object Verdict -eq 'CHECKLIST_ONLY').Count
        Unmapped    = ($g | Where-Object Verdict -eq 'UNMAPPED').Count
        Vendor      = ($g | Where-Object Verdict -eq 'VENDOR').Count
        UnmappedKB  = [math]::Round((($g | Where-Object Verdict -eq 'UNMAPPED') | Measure-Object KB -Sum).Sum)
    }
} | Sort-Object -Property UnmappedKB -Descending

Write-Host ''
Write-Host '=== per-module coverage ===' -ForegroundColor Cyan
$byDir | Format-Table -AutoSize | Out-String -Width 200 | Write-Host

$tot = [pscustomobject]@{
    Units       = $rows.Count
    Mapped      = ($rows | Where-Object Verdict -eq 'MAPPED').Count
    Weak        = ($rows | Where-Object Verdict -eq 'WEAK').Count
    Assigned    = ($rows | Where-Object Verdict -eq 'ASSIGNED').Count
    ChecklistOn = ($rows | Where-Object Verdict -eq 'CHECKLIST_ONLY').Count
    Unmapped    = ($rows | Where-Object Verdict -eq 'UNMAPPED').Count
    Vendor      = ($rows | Where-Object Verdict -eq 'VENDOR').Count
}
Write-Host ("TOTAL units={0}  mapped={1}  weak(on-header-less mention)={2}  assigned={3}  checklist-only={4}  unmapped={5}  vendor={6}" -f `
    $tot.Units, $tot.Mapped, $tot.Weak, $tot.Assigned, $tot.ChecklistOn, $tot.Unmapped, $tot.Vendor) -ForegroundColor Green

Write-Host ''
Write-Host '=== top UNMAPPED by size (candidate next batches) ===' -ForegroundColor Yellow
$rows | Where-Object Verdict -eq 'UNMAPPED' | Sort-Object KB -Descending |
    Select-Object -First 40 Dir, Unit, Lines, KB, Rel | Format-Table -AutoSize | Out-String -Width 200 | Write-Host

if ($ShowMapped) {
    Write-Host ''
    Write-Host '=== MAPPED ===' -ForegroundColor Cyan
    $rows | Where-Object Verdict -eq 'MAPPED' | Sort-Object Dir, Unit |
        Format-Table -AutoSize | Out-String -Width 200 | Write-Host
}

# ---- optional markdown report --------------------------------------------
if ($Report) {
    $sb = New-Object System.Text.StringBuilder
    [void]$sb.AppendLine('# Parallel coverage audit (Delphi .pas -> C#)')
    [void]$sb.AppendLine('')
    [void]$sb.AppendLine("> Generated by ``GXX.CSharp/tools/audit-coverage.ps1`` at $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')")
    [void]$sb.AppendLine('> Evidence: E1 same-basename .cs / E2 "<unit>.pas" referenced from a src .cs header / E3 mentioned in Checklist.md / E4 explicit tools/unit-map.tsv row')
    [void]$sb.AppendLine('')
    [void]$sb.AppendLine('## Totals')
    [void]$sb.AppendLine('')
    [void]$sb.AppendLine('| units | mapped | assigned (in flight) | checklist-only | unmapped | vendor |')
    [void]$sb.AppendLine('|---|---|---|---|---|---|')
    [void]$sb.AppendLine("| $($tot.Units) | $($tot.Mapped) | $($tot.Assigned) | $($tot.ChecklistOn) | $($tot.Unmapped) | $($tot.Vendor) |")
    [void]$sb.AppendLine('')
    [void]$sb.AppendLine('## Per module')
    [void]$sb.AppendLine('')
    [void]$sb.AppendLine('| dir | units | mapped | assigned | checklist-only | unmapped | vendor | unmapped KB |')
    [void]$sb.AppendLine('|---|---|---|---|---|---|---|---|')
    foreach ($r in $byDir) {
        [void]$sb.AppendLine("| $($r.Dir) | $($r.Units) | $($r.Mapped) | $($r.Assigned) | $($r.ChecklistOn) | $($r.Unmapped) | $($r.Vendor) | $($r.UnmappedKB) |")
    }
    [void]$sb.AppendLine('')
    [void]$sb.AppendLine('## ASSIGNED units (owned by a parallel lane, work in flight)')
    [void]$sb.AppendLine('')
    [void]$sb.AppendLine('| dir | unit | lines | KB | lane |')
    [void]$sb.AppendLine('|---|---|---|---|---|')
    foreach ($r in ($rows | Where-Object Verdict -eq 'ASSIGNED' | Sort-Object KB -Descending)) {
        [void]$sb.AppendLine("| $($r.Dir) | $($r.Unit) | $($r.Lines) | $($r.KB) | $($explicit[$r.Unit]) |")
    }
    [void]$sb.AppendLine('')
    [void]$sb.AppendLine('## UNMAPPED units, largest first')
    [void]$sb.AppendLine('')
    [void]$sb.AppendLine('| dir | unit | lines | KB | source path |')
    [void]$sb.AppendLine('|---|---|---|---|---|')
    foreach ($r in ($rows | Where-Object Verdict -eq 'UNMAPPED' | Sort-Object KB -Descending)) {
        [void]$sb.AppendLine("| $($r.Dir) | $($r.Unit) | $($r.Lines) | $($r.KB) | ``$($r.Rel)`` |")
    }
    [void]$sb.AppendLine('')
    [void]$sb.AppendLine('## CHECKLIST_ONLY units (mentioned in Checklist.md but no direct .cs evidence)')
    [void]$sb.AppendLine('')
    [void]$sb.AppendLine('| dir | unit | lines | KB |')
    [void]$sb.AppendLine('|---|---|---|---|')
    foreach ($r in ($rows | Where-Object Verdict -eq 'CHECKLIST_ONLY' | Sort-Object KB -Descending)) {
        [void]$sb.AppendLine("| $($r.Dir) | $($r.Unit) | $($r.Lines) | $($r.KB) |")
    }
    [System.IO.File]::WriteAllText($reportMd, $sb.ToString(), (New-Object System.Text.UTF8Encoding($false)))
    Write-Host "report written: $reportMd" -ForegroundColor Green
}
