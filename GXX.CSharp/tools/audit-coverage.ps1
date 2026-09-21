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
    [string[]]$Dir = @('M2Engine','Client-HGE','DBServer','LoginSrv','LoginGate','SelGate','RunGate','GameCenter','LogDataServer','Common'),
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
#
# ---- ALSO "not ported BY DESIGN" (added round 24, from the 30-unit verification
# ---- in the ledger section 18.4/18.5).  These are REAL Delphi units, but counting
# ---- them as "remaining work" OVERSTATES the backlog -- which is exactly how the
# ---- four unowned giants were missed earlier (ledger 13.7).  Reasons:
# ----   c1 dead code      : not in any .dpr and has no `uses` anywhere in the tree
# ----   c3 replaced       : a .NET-native facility does the job (managed side uses
# ----                       its own equivalent; porting the Delphi one adds dead code)
# ----   c4 duplicate      : byte-identical to / superseded by another unit that IS ported
# ---- NOTE: basename-keyed, so a unit with copies in two dirs cannot be listed here
# ---- unless EVERY copy is a non-port.  GameCenter\ParadoxDataSet is a byte-identical
# ---- duplicate of the RunGate copy, but that basename must stay countable because the
# ---- RunGate copy IS being ported -> it is deliberately NOT listed here.
$VENDOR_UNITS = @(
    'VMProtectSDK','EHookLIB','uSynHighlighterSample','plgSearchHighlighter','JClasses','DLLLoader','LuaScript','LuaEvent','LuaActor',
    'DES',              # c1 dead code: no .dpr, no uses (tree already has DesUnit/UnitDes/EncryptUnit)
    'DragFromShell',    # c1 dead code: 3rd-party VCL component, zero references
    'uFrmCustomMoney',  # c1 dead code: empty shell form, no references
    'DxControlClpbrd',  # c1 dead code: 153 lines identical to StreamClipbrd, unused
    'SimpleClass',      # c3 .NET collections replace TQueue/TStack/TList/TVector
    'SHSocket',         # c3 Win32 socket decls -> SocketAsyncEventArgs (GatewayKit/TcpLink.cs)
    'FixedMemoryPool',  # c3 manual block allocator -> GC / ConcurrentQueue
    'MemPool',          # c3 manual block allocator -> GC / ConcurrentQueue
    'SyncObj',          # c3 TCriticalSection wrapper -> lock/Monitor
    'IOCPTypeDef',      # c3 IOCP typedefs -> SocketAsyncEventArgs (GatewayProtocol.IocpManager)
    'VersionHelper',    # c3 VerifyVersionInfo wrapper -> OperatingSystem.IsWindowsVersionAtLeast
    'MsCTF',            # c3 COM IME -> WinForms native IME (Checklist section 5)
    'GHeroDB',          # c3 BDE alias/table/field manager, obsolete under the Sqlite architecture
    'Objects',          # c1 design-time only: referenced by GuiEdit.dpr, NOT by Client.dpr
    'FireDragon',       # c4 old duplicate of ObjFireDragon.pas (not in any .dpr)
    # ---- c3 "replaced by the managed runtime", ruled in ledger 18.5#5/#6 and executed 2026-09-21.
    # Registered HERE (not as a .cs header mention) on purpose: a header mention scores the unit
    # MAPPED, which would claim a port that was never made.  These units are NOT ported.
    'imm',              # c3 IME P/Invoke -> WinForms native IME; verified 76/76 routines absent
    'SendQueue',        # c3 both copies (LoginGate/SelGate) -> GatewayKit send queue
    'IOCPManager',      # c3 both copies (LoginGate/SelGate) -> SocketAsyncEventArgs
    # ---- P9 lane par/p9-rungate-rest (2026-09-21, ledger section 40): four RunGate/IOCP units
    # ruled "not ported + evidence" (empty shell / replaced by GatewayKit / callers already
    # not-ported).  That lane's evidence file deliberately spells "<unit>.pas" in its HEADER,
    # which would score them MAPPED -- a port that was never made.  Registered here so the
    # report says "not ported" instead.  All four basenames are unique (verified).
    'IODataPool',       # P9: 586 lines, POVERLAPPEDEx raw pointers; every caller already not-ported
    'IocpTcpClient',    # P9: 340 lines, 20 Win32 IOCP calls inside one method; TcpLink is the live path
    'Qos',              # P9: 293 lines, header-only translation; 42 of 49 names absent from C#
    'DllUpdateCommon',  # P9: 10-line empty shell, 0 declarations, 0 uses anywhere
    # ---- "<dir>/<unit>" form = THAT COPY ONLY (see the duplicate-basename note at the bottom).
    # ThreadPool.pas has three copies with OPPOSITE rulings: the two gateway copies are replaced by
    # GatewayKit (not ported), while LogDataServer's is a REAL gap (TPoolManager/TPoolThread absent)
    # that lane par/p10-db-login-forms is porting.  A bare 'ThreadPool' row would hide that gap.
    # ---- "<dir>/<unit>" form = THAT COPY ONLY.  CORRECTED 2026-09-21 (ledger 44.4): the earlier
    # bare 'SendQueue' / 'IOCPManager' rows were justified as "basename unique", which was only
    # true because LoginGate was not yet in $Dir.  Both units DO have LoginGate + SelGate copies,
    # so they are now registered per copy.  (The ruling is unchanged -- both copies are replaced by
    # GatewayKit -- but a bare row would have covered future copies too, which is not ours to decide.)
    'SelGate/ThreadPool',
    'LoginGate/ThreadPool',
    'LoginGate/SendQueue',
    'SelGate/SendQueue',
    'LoginGate/IOCPManager',
    'SelGate/IOCPManager',
    # ---- found by the read-only review lane par/p11-logingate-review (ledger 44.3):
    # AcceptExWorkedThread: 1,398 lines, 5 classes, ZERO managed declarations anywhere.
    # uDep: basename is unique repo-wide, compiled into LoginGate only.
    'AcceptExWorkedThread',
    'uDep',
    # LoginGate/DesUtils: 1,368 lines whose every call site sits inside {$IF VER_TYPE=1} while
    # Misc.pas:9 sets VER_TYPE=0, i.e. compiled OUT.  MUST stay per-copy: DesUtils has four
    # DIFFERING copies and a bare key would also close the Client copy that PakCrypto.cs ports.
    'LoginGate/DesUtils',
    # ---- c1 dead code, ruled by lane par/p9-m2-datalayer (2026-09-21, ledger 41) with 6 counts:
    # zero hits in .pas/.dpr/.dpk, its own type names hit only itself, ACCOUNTLEN/ACTORNAMELEN are
    # undefined repo-wide (so the original cannot even compile), g_UserShopDB is declared nowhere,
    # and its `unit UserShopDB;` name collides with the live UserShopDB unit.
    'UserShopDB_Old',
    # ---- GuiManage: NOT PORTED, ruled by lane par/p11-client-dxrest2 (ledger 47.2) with three
    # independent proofs, any one of which is fatal to a 1:1 port:
    #   1. the original references type TDxBackground, which exists NOWHERE in the tree
    #      (:29/:443/:454; zero hits in .pas/.dpr/.inc/.dfm besides those lines) -> it cannot build;
    #   2. its job is already carried by GXX.Client.LoadDx in an EVOLVED form (DxControlFactory /
    #      GuiComponentLoader / DxGuiFonts ... with 11 extra version boundaries + DES + name table),
    #      so a 1:1 copy would insert a SECOND deserializer for the same protocol;
    #   3. doing it 1:1 would require redefining 13 out-of-zone seams (TDxEdit/TDxImageGrid/... whose
    #      only home is LoadDx/DxControlSeams.cs, the pending dedup battlefield) -> ledger 14.2.
    # Bare key is safe: GuiManage.pas exists once in the whole tree (verified).
    'GuiManage',
    # ---- ledger 49.1: the 3 of the 13 giant E2-only units whose function IS carried by a
    # substitute framework -- lane par/p12-e2only-review proved 1,188 routines with 0 translated,
    # the behaviour instead being provided by Engine/NpcScriptCommands.cs' command-code dispatcher.
    # Per ledger 39.5/44.5 ("covered by a shared facility" belongs in the not-ported bucket):
    'NpcActionCmd',            # 47,018 lines
    'NpcConditionCmd',         # 10,489
    'HandleCommands'           # 9,151
    # NOTE: 'ThreadPool' is deliberately NOT registered here.  It exists in LogDataServer (a REAL
    # gap: 445 lines, TPoolManager/TPoolThread unported) as well as in LoginGate/SelGate (replaced
    # by design).  This registry keys on the BASENAME, so a row would silently hide the
    # LogDataServer gap -- see the report's DUPLICATE-BASENAME section (ledger 39.3).
)

# ---- E2 claims REFUTED by a verification lane -----------------------------
# A read-only review lane can PROVE that an E2 mention belongs to a different unit (borrowed
# name): e.g. ledger 44.3 found 12 of LoginGate's 15 MAPPED rows were borrowed from a differing
# sibling, and 4 units had no managed declaration at all.  Without this registry such a finding
# has nowhere to live -- the unit would keep scoring MAPPED because the mention is still there.
# "<dir>/<unit>" entries apply to that copy only, same as $VENDOR_UNITS.
$E2_REFUTED = @(
    'LoginGate/Misc',          # ledger 44.3: the 8 enforcement routines have 0 hits repo-wide
    'LoginGate/FuncForComm',   # ledger 44.3: TProcMsgThread/TAddressInfo have 0 hits repo-wide
    # ---- ledger 49.1: the 7 REAL gaps among the 13 giant E2-only units that lane
    # par/p12-e2only-review adjudicated as class C ("not ported / name borrowed"), verified by
    # method-level sampling.  They have NO substitute framework, so they must stay visible as
    # gaps instead of hiding in MAPPED (putting them in $VENDOR_UNITS would hide them).
    # Basenames verified unique repo-wide, so bare keys are safe here.
    'ObjHero',                 # 14,664
    'StateWindows',            # 14,027
    'MShare',                  # 13,522
    'Actor',                   # 18,009
    'ObjMon'                   # 9,502  (46 *Core.cs hold ~1,917 bare `=> true;` stubs)
)

# ---- PARTIALLY ported units (the "third state" asked for by lane p12) -----
# p12's core conclusion (ledger 49.1) was: "E2 holds but the implementation does not exist" had no
# home in the five buckets, and neither did "some of it exists".  A unit that is genuinely HALF
# ported must not read MAPPED (that claims completion) nor REFUTED (that denies the work done).
# PARTIAL is that state: excluded from `mapped`, kept out of the not-ported bucket, and listed
# with the measured ratio so the next lane knows exactly what it inherits.
# "<dir>/<unit>" applies to that copy only, same as $VENDOR_UNITS / $E2_REFUTED.
$PARTIAL_UNITS = @(
    # ledger 49.1: p12 measured these two by method-level sampling.
    'ObjNpc',          # 67/112 top-level routines ported (lane-built ObjNpcRoutineRegistry.cs)
    'FunctionConfig',  # 142/926 = 15.3%
    # ledger 56.1: advanced from REFUTED by lane par/p14-client-fstate.  The unit was a 515-member
    # <auto-generated> throw-shell with ~20 real members; two slices took it to 465 throws and
    # 71/533 declared members really implemented (13.32%).  Its own IL-based guard test
    # (newobj NotSupportedException) is what keeps "forwarding body" from passing as "implemented".
    'FState'           # 90/533 = 16.89% (round 2); PENDING 171 is the main battlefield
    # ledger 57.1/58.1: ObjPlayer advanced from REFUTED by lane par/p13-m2-objplayer.
    # Measured: 17/811 = 2.10% at start, ~367/811 = 45.3% delivered, and the lane itself insists on
    # the CREDIBLE LOWER BOUND 38% because 37 integration-failing tests were ticketed as skips
    # (D-P13-09) instead of being deleted -- "unverified counts as not done" applied to its own work.
    # 15 of 21 line segments remain (~189 routines).
    ,'ObjPlayer'       # 38%~45.3% (lower bound adopted)
)

# ---- per-copy entries of the not-ported registry --------------------------
# A "<dir>/<unit>" entry marks ONLY that copy.  Needed wherever one basename has copies with
# OPPOSITE rulings (e.g. ThreadPool.pas: two gateway copies replaced by GatewayKit, one
# LogDataServer copy that is a real gap).  Bare entries keep the old any-copy meaning.
$vendorByCopy = @{}
foreach ($v in $VENDOR_UNITS) { if ($v -like '*/*') { $vendorByCopy[$v] = $true } }
$refutedByCopy = @{}
foreach ($v in $E2_REFUTED) { if ($v -like '*/*') { $refutedByCopy[$v] = $true } }
$partialByCopy = @{}
foreach ($v in $PARTIAL_UNITS) { if ($v -like '*/*') { $partialByCopy[$v] = $true } }

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

# ---- E2 evidence VALIDITY (ledger 49.2) -----------------------------------
# A header mention only counts as evidence of a PORT if the file is not itself an admission of
# absence.  Measured by read-only lane par/p12-e2only-review: FState.pas scored MAPPED although
# 291 of its 330 name hits came from GUI/Share/TFrmDlg.Decl.g.cs -- an <auto-generated> shell
# whose 515 members ALL `throw new NotSupportedException`, with a header that says so.
# Two file classes are therefore barred from providing E2:
#   * generated shells / stub shells that announce it in their header;
#   * files whose own header states the unit is not ported (NotPorted( / 未移植 / NotSupported).
$csHeadNoE2 = @{}
$NEG_MARKERS = @('<auto-generated>', 'NotSupportedException', 'NotPorted(')
$NEG_MARKER_CJK = ([string][char]0x672A) + [char]0x79FB + [char]0x690D   # "wei yi zhi" = not ported

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
    try {
        # marker scan must read real UTF-8: the latin1 view above is byte-transparent on purpose
        $u = [System.IO.File]::ReadAllText($f.FullName, [System.Text.Encoding]::UTF8)
        $uls = $u -split "`n"
        if ($uls.Count -gt $HEAD_LINES) { $u = ($uls[0..($HEAD_LINES - 1)] -join "`n") }
        $neg = $false
        foreach ($mk in $NEG_MARKERS) { if ($u.IndexOf($mk, [StringComparison]::Ordinal) -ge 0) { $neg = $true; break } }
        if (-not $neg -and $u.IndexOf($NEG_MARKER_CJK, [StringComparison]::Ordinal) -ge 0) { $neg = $true }
        if ($neg) { $csHeadNoE2[$f.FullName] = $true }
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
        if ($vendorByCopy.ContainsKey("$d/$($f.BaseName)")) { $isVendor = $true }

        $unit = $f.BaseName

        # A Delphi unit name is an ASCII identifier.  A .pas file whose BASENAME
        # carries non-ASCII characters (e.g. a "-<backup suffix>" copy kept beside
        # the real unit) cannot be a compilable unit reference of this project: it
        # is a hand-kept backup or annotated copy.  Report it separately -- never
        # as "missing work".  Written with an escape so this script stays ASCII-only.
        $isNonUnit = $unit -match '[^\x00-\x7F]'

        # A DATED BACKUP COPY is not a unit either -- and unlike the non-ASCII case above, these
        # names are perfectly legal ASCII identifiers, so the rule above misses them.  Measured
        # 2026-09-21: Client-HGE\ClMain20230516.pas (54,187 lines) sat in the report as a real
        # "unit" and even scored MAPPED via a mention, inflating both totals.  Rule: "<stem><date>.pas"
        # beside a live "<stem>.pas" means a hand-kept copy.
        $isDatedBackup = $false
        if ($unit -match '^(.*?)(\d{8}|\d{6})$') {
            $stem = $Matches[1]
            if ($stem.Length -ge 3) {
                try { if (Test-Path (Join-Path $f.DirectoryName "$stem.pas")) { $isDatedBackup = $true } } catch { }
            }
        }

        $e1 = $csByBase.ContainsKey($unit.ToLowerInvariant())
        $e2 = $false
        $e2w = $false
        $e2File = ''
        if (-not $e1) {
            $needle = "$unit.pas"
            foreach ($k in $csHead.Keys) {
                if ($csHeadNoE2.ContainsKey($k)) { continue }   # ledger 49.2: not evidence
                if ($csHead[$k].IndexOf($needle, [StringComparison]::Ordinal) -ge 0) { $e2 = $true; $e2File = $k.Substring($csRoot.Length + 1); break }
            }
            if (-not $e2) {
                foreach ($k in $csText.Keys) { if ($csText[$k].IndexOf($needle, [StringComparison]::Ordinal) -ge 0) { $e2w = $true; break } }
            }
        }
        $e3 = $false
        if (-not ($e1 -or $e2)) { $e3 = $checklistText.IndexOf($unit, [StringComparison]::Ordinal) -ge 0 }
        $e4 = $false
        $e4lane = ''
        # E4 keys: "<unit>" assigns EVERY copy with that basename, "<dir>/<unit>" assigns ONLY the
        # copy in that source dir.  The per-copy form exists because basenames repeat (30+ groups
        # in this tree): without it, assigning LogDataServer's ThreadPool would also have to cover
        # SelGate's, whose ruling is the opposite ("replaced by GatewayKit, not ported").
        if ($explicit.ContainsKey("$d/$unit")) { $e4 = $true; $e4lane = $explicit["$d/$unit"] }
        elseif ($explicit.ContainsKey($unit)) { $e4 = $true; $e4lane = $explicit[$unit] }

        $mapped = $e1 -or $e2
        $lines = 0
        # PHYSICAL line count (count of LF + 1 if the file does not end with LF).
        # The previous implementation used `... | Measure-Object -Line`, which skips
        # blank lines and therefore UNDERCOUNTED every unit that has blank lines --
        # measured on this tree the error ran 1.04x .. 1.19x (e.g. MirClientContext.pas
        # 9,758 vs 11,126 actual), i.e. units looked up to a fifth smaller than they
        # are.  Lane p2-rungate-impl caught this.  Reading bytes through the latin1
        # (byte-transparent) codec also makes the count independent of the source's
        # GBK encoding and robust to the CRLF/bare-LF mixtures in this tree.
        try {
            $txt = [System.IO.File]::ReadAllText($f.FullName, $latin1)
            $lines = ($txt -split "`n").Count
            if ($txt.EndsWith("`n")) { $lines-- }
        } catch { }

        # E4 = the unit is explicitly assigned to a parallel lane, i.e. in flight,
        # NOT finished. It must never be counted as mapped.
        # WEAK = only a non-header mention exists: explicitly NOT a port.
        # ORDER MATTERS: ASSIGNED is tested BEFORE WEAK.  A unit can be both (it has a lane row
        # AND a stray body mention); "someone owns it" is the more actionable fact, and letting
        # WEAK win would hide an in-flight unit from the ASSIGNED table -- the dispatcher's list.
        # A verification lane proved the E2 mention does not describe THIS unit (borrowed name).
        # This BEATS MAPPED unconditionally: the whole point of the registry is that the mention
        # is still sitting there and would otherwise keep scoring the unit as ported.  A lane row
        # does not restore the claim -- it only means someone is now fixing the real gap, and the
        # unit still shows up in the in-flight table (which keys on E4, not on the verdict).
        $isRefuted = $false
        if ($E2_REFUTED -contains $unit) { $isRefuted = $true }
        if ($refutedByCopy.ContainsKey("$d/$($f.BaseName)")) { $isRefuted = $true }

        # PARTIAL: a measured, genuinely incomplete port (the "third state" of ledger 49.1).
        # Beats MAPPED (it must not claim completion) but not REFUTED (a refuted claim is a
        # stronger, negative statement).  A lane row still shows it in the in-flight table.
        $isPartial = $false
        if ($PARTIAL_UNITS -contains $unit) { $isPartial = $true }
        if ($partialByCopy.ContainsKey("$d/$($f.BaseName)")) { $isPartial = $true }

        $verdict = if ($isVendor) { 'VENDOR' }
                   elseif ($isNonUnit) { 'NONUNIT' }
                   elseif ($isDatedBackup) { 'NONUNIT' }
                   elseif ($isRefuted) { 'REFUTED' }
                   elseif ($isPartial) { 'PARTIAL' }
                   elseif ($mapped) { 'MAPPED' }
                   elseif ($e4) { 'ASSIGNED' }
                   elseif ($e2w) { 'WEAK' }
                   elseif ($e3) { 'CHECKLIST_ONLY' }
                   else { 'UNMAPPED' }

        # ---- UI dimension: a VCL form/dialog unit has a .dfm beside its .pas. ----
        # The project's DoD is "code AND interface AND functionality fully translated", but the
        # report only ever measured code units.  A sibling .dfm is an OBJECTIVE marker of "this
        # unit is a window/dialog", so the 界面 dimension can be counted instead of guessed.
        $hasDfm = $false
        try { $hasDfm = (Test-Path ([System.IO.Path]::ChangeExtension($f.FullName, '.dfm'))) } catch { }

        $rows += [pscustomobject]@{
            Dir = $d; Unit = $unit; Lines = $lines; KB = [math]::Round($f.Length / 1KB)
            E1 = $e1; E2 = $e2; E2w = $e2w; E3 = $e3; E4 = $e4; Verdict = $verdict; Rel = $rel; Lane = $e4lane
            E2File = $e2File
            HasDfm = $hasDfm
        }
    }
}

# ---- pre-pass: per-copy hashes for multi-copy basenames -------------------
# An E2 verdict (and any not-ported registry row) keys on the BASENAME, so it can be "borrowed"
# from a sibling copy.  Byte-identical copies are the same source (the mention transfers);
# DIFFERING copies are two units and the mention may describe the other one.
# Measured on LoginGate: 12 of its 15 MAPPED rows were borrowed from a differing sibling.
$srcHash = @{}        # source path -> sha256[:8]  (only for rows in a duplicate group)
$dupDistinct = @{}    # unit basename -> count of distinct hashes across its copies
$dupGroups = @($rows | Group-Object Unit | Where-Object { ($_.Group | Select-Object -ExpandProperty Dir -Unique).Count -gt 1 })
foreach ($g in $dupGroups) {
    $hs = @{}
    foreach ($r in $g.Group) {
        $h = '?'
        try { $h = (Get-FileHash -Algorithm SHA256 -LiteralPath (Join-Path $srcRoot $r.Rel)).Hash.Substring(0, 8) } catch { }
        $srcHash[$r.Rel] = $h
        $hs[$h] = $true
    }
    $dupDistinct[$g.Name] = $hs.Keys.Count
}

# ---- summarise ------------------------------------------------------------
$byDir = $rows | Group-Object Dir | ForEach-Object {
    $g = $_.Group
    [pscustomobject]@{
        Dir         = $_.Name
        Units       = $g.Count
        Mapped      = @($g | Where-Object Verdict -eq 'MAPPED').Count
        Weak        = @($g | Where-Object Verdict -eq 'WEAK').Count
        Assigned    = @($g | Where-Object Verdict -eq 'ASSIGNED').Count
        ChecklistOn = @($g | Where-Object Verdict -eq 'CHECKLIST_ONLY').Count
        Unmapped    = @($g | Where-Object Verdict -eq 'UNMAPPED').Count
        Vendor      = @($g | Where-Object Verdict -eq 'VENDOR').Count
        NonUnit     = @($g | Where-Object Verdict -eq 'NONUNIT').Count
        UnmappedKB  = [math]::Round((($g | Where-Object Verdict -eq 'UNMAPPED') | Measure-Object KB -Sum).Sum)
    }
} | Sort-Object -Property UnmappedKB -Descending

Write-Host ''
Write-Host '=== per-module coverage ===' -ForegroundColor Cyan
$byDir | Format-Table -AutoSize | Out-String -Width 200 | Write-Host

$tot = [pscustomobject]@{
    Units       = $rows.Count
    Mapped      = @($rows | Where-Object Verdict -eq 'MAPPED').Count
    Weak        = @($rows | Where-Object Verdict -eq 'WEAK').Count
    Assigned    = @($rows | Where-Object Verdict -eq 'ASSIGNED').Count
    ChecklistOn = @($rows | Where-Object Verdict -eq 'CHECKLIST_ONLY').Count
    Unmapped    = @($rows | Where-Object Verdict -eq 'UNMAPPED').Count
    Vendor      = @($rows | Where-Object Verdict -eq 'VENDOR').Count
    NonUnit     = @($rows | Where-Object Verdict -eq 'NONUNIT').Count
    Refuted     = @($rows | Where-Object Verdict -eq 'REFUTED').Count
    Partial     = @($rows | Where-Object Verdict -eq 'PARTIAL').Count
}
Write-Host ("TOTAL units={0}  mapped={1}  partial={2}  weak={3}  assigned={4}  checklist-only={5}  unmapped={6}  not-ported={7}  non-unit={8}" -f `
    $tot.Units, $tot.Mapped, $tot.Partial, $tot.Weak, $tot.Assigned, $tot.ChecklistOn, $tot.Unmapped, $tot.Vendor, $tot.NonUnit) -ForegroundColor Green

# ---- MAPPED-on-mention-only summary (the over-claim risk) ------------------
$e2only = @($rows | Where-Object { $_.Verdict -eq 'MAPPED' -and -not $_.E1 })
if ($e2only.Count -gt 0) {
    $e2lines = ($e2only | Measure-Object Lines -Sum).Sum
    Write-Host ''
    Write-Host ("=== MAPPED on header mention only (E2-only): {0} units / {1} lines -- claimed, not proven ===" -f `
        $e2only.Count, $e2lines) -ForegroundColor Yellow
    $e2only | Sort-Object KB -Descending | Select-Object -First 12 Dir, Unit, Lines, KB |
        Format-Table -AutoSize | Out-String -Width 200 | Write-Host
}

# ---- UI dimension summary (objective marker: a sibling .dfm) --------------
$uiRows = @($rows | Where-Object HasDfm)
if ($uiRows.Count -gt 0) {
    $uiMapped = @($uiRows | Where-Object Verdict -eq 'MAPPED').Count
    Write-Host ''
    Write-Host ("=== UI units (sibling .dfm present): total={0}  mapped={1}  NOT mapped={2} ===" -f `
        $uiRows.Count, $uiMapped, ($uiRows.Count - $uiMapped)) -ForegroundColor Cyan
    $uiRows | Where-Object Verdict -ne 'MAPPED' | Group-Object Verdict |
        Select-Object Name, Count | Format-Table -AutoSize | Out-String -Width 200 | Write-Host
}

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
    [void]$sb.AppendLine('| units | mapped | partial | weak | assigned (in flight) | checklist-only | unmapped | not-ported | non-unit |')
    [void]$sb.AppendLine('|---|---|---|---|---|---|---|---|---|')
    [void]$sb.AppendLine("| $($tot.Units) | $($tot.Mapped) | $($tot.Partial) | $($tot.Weak) | $($tot.Assigned) | $($tot.ChecklistOn) | $($tot.Unmapped) | $($tot.Vendor) | $($tot.NonUnit) |")
    [void]$sb.AppendLine('')
    [void]$sb.AppendLine('## Per module')
    [void]$sb.AppendLine('')
    [void]$sb.AppendLine('| dir | units | mapped | assigned | checklist-only | unmapped | not-ported | non-unit | unmapped KB |')
    [void]$sb.AppendLine('|---|---|---|---|---|---|---|---|---|')
    foreach ($r in $byDir) {
        [void]$sb.AppendLine("| $($r.Dir) | $($r.Units) | $($r.Mapped) | $($r.Assigned) | $($r.ChecklistOn) | $($r.Unmapped) | $($r.Vendor) | $($r.NonUnit) | $($r.UnmappedKB) |")
    }
    [void]$sb.AppendLine('')
    [void]$sb.AppendLine('## PARTIAL units (measured incomplete -- the third state, ledger 49.1/50.2)')
    [void]$sb.AppendLine('')
    [void]$sb.AppendLine('These units ARE partly ported, but the port is measurably incomplete.  They are excluded')
    [void]$sb.AppendLine('from `mapped` (that would claim completion) and deliberately NOT put in the not-ported')
    [void]$sb.AppendLine('bucket (that would deny the work already done).  The ratio is measured by method-level')
    [void]$sb.AppendLine('sampling and recorded in the ledger; a lane inheriting one of these starts from that ratio.')
    [void]$sb.AppendLine('')
    [void]$sb.AppendLine('| dir | unit | lines | KB | source path |')
    [void]$sb.AppendLine('|---|---|---|---|---|')
    foreach ($r in ($rows | Where-Object Verdict -eq 'PARTIAL' | Sort-Object KB -Descending)) {
        [void]$sb.AppendLine("| $($r.Dir) | $($r.Unit) | $($r.Lines) | $($r.KB) | ``$($r.Rel)`` |")
    }
    [void]$sb.AppendLine('')
    [void]$sb.AppendLine('## REFUTED E2 claims (a verification lane proved the mention is borrowed)')
    [void]$sb.AppendLine('')
    [void]$sb.AppendLine('These units used to score MAPPED purely because some .cs header mentions their name.')
    [void]$sb.AppendLine('A read-only lane then proved the mention belongs to a DIFFERENT unit or that no managed')
    [void]$sb.AppendLine('code exists at all, so the claim is withdrawn here.  They are real gaps.')
    [void]$sb.AppendLine('')
    [void]$sb.AppendLine('| dir | unit | lines | KB | source path |')
    [void]$sb.AppendLine('|---|---|---|---|---|')
    foreach ($r in ($rows | Where-Object Verdict -eq 'REFUTED' | Sort-Object KB -Descending)) {
        [void]$sb.AppendLine("| $($r.Dir) | $($r.Unit) | $($r.Lines) | $($r.KB) | ``$($r.Rel)`` |")
    }
    [void]$sb.AppendLine('')
    [void]$sb.AppendLine('## MAPPED on a header mention ONLY (E2-only, no same-basename .cs) -- WEAK EVIDENCE')
    [void]$sb.AppendLine('')
    [void]$sb.AppendLine('These rows are counted MAPPED, but the ONLY evidence is that some unrelated .cs file')
    [void]$sb.AppendLine('mentions "<unit>.pas" inside its first 40 lines.  There is no same-basename port.')
    [void]$sb.AppendLine('That is exactly how LoginGate''s 15 units were scored (ledger 41.2), and E2 matches on the')
    [void]$sb.AppendLine('BASENAME alone, so a mention coming from a SIBLING copy (SelGate vs LoginGate, M2Engine vs')
    [void]$sb.AppendLine('Client-HGE) scores every copy.  Treat each row as "claimed, not proven" and verify per copy')
    [void]$sb.AppendLine('before citing it as done.')
    [void]$sb.AppendLine('')
    [void]$sb.AppendLine('| dir | unit | lines | KB | E2 evidence (.cs that mentions it) | sibling copies w/ differing bytes | source path |')
    [void]$sb.AppendLine('|---|---|---|---|---|---|---|')
    foreach ($r in ($rows | Where-Object { $_.Verdict -eq 'MAPPED' -and -not $_.E1 } | Sort-Object KB -Descending)) {
        $n = if ($dupDistinct.ContainsKey($r.Unit)) { $dupDistinct[$r.Unit] } else { 1 }
        $warn = if ($n -gt 1) { "**$n (borrow risk)**" } else { "$n" }
        [void]$sb.AppendLine("| $($r.Dir) | $($r.Unit) | $($r.Lines) | $($r.KB) | ``$($r.E2File)`` | $warn | ``$($r.Rel)`` |")
    }
    [void]$sb.AppendLine('')
    [void]$sb.AppendLine('## ASSIGNED units (a lane owns them -- keyed on the unit-map row, not on the verdict)')
    [void]$sb.AppendLine('')
    [void]$sb.AppendLine('| dir | unit | lines | KB | lane | verdict |')
    [void]$sb.AppendLine('|---|---|---|---|---|---|')
    foreach ($r in ($rows | Where-Object { $_.E4 } | Sort-Object KB -Descending)) {
        [void]$sb.AppendLine("| $($r.Dir) | $($r.Unit) | $($r.Lines) | $($r.KB) | $($r.Lane) | $($r.Verdict) |")
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
    [void]$sb.AppendLine('## WEAK units (body-mention only -- NOT a port; MUST be triaged every wave)')
    [void]$sb.AppendLine('')
    [void]$sb.AppendLine('A WEAK row means: no same-basename .cs (E1 fails), no "<unit>.pas" inside any src .cs')
    [void]$sb.AppendLine('header doc comment (E2 fails), but the unit name appears somewhere in a .cs body.')
    [void]$sb.AppendLine('')
    [void]$sb.AppendLine('**Do not read these as done.**  Whole unported units hide in this bucket: measured on')
    [void]$sb.AppendLine('2026-09-21, 30,000+ Delphi lines were sitting here while the report showed unmapped=0,')
    [void]$sb.AppendLine('because a single body mention downgraded an entire unit out of UNMAPPED.')
    [void]$sb.AppendLine('')
    [void]$sb.AppendLine('Triage EVERY row into exactly one of:')
    [void]$sb.AppendLine('')
    [void]$sb.AppendLine('1. **add E2 evidence** -- the unit really is ported; add `// source unit: <unit>.pas`')
    [void]$sb.AppendLine('   to the header of the .cs that declares it (cheapest, and it keeps the report honest);')
    [void]$sb.AppendLine('2. **assign a lane** -- it is not ported; add a tools/unit-map.tsv row + a lane zone;')
    [void]$sb.AppendLine('3. **function-level check** -- the unit declares no classes (procedure library / P/Invoke),')
    [void]$sb.AppendLine('   so class-name matching cannot decide it; record the verdict in the ledger.')
    [void]$sb.AppendLine('')
    [void]$sb.AppendLine('| dir | unit | lines | KB | source path |')
    [void]$sb.AppendLine('|---|---|---|---|---|')
    foreach ($r in ($rows | Where-Object Verdict -eq 'WEAK' | Sort-Object KB -Descending)) {
        [void]$sb.AppendLine("| $($r.Dir) | $($r.Unit) | $($r.Lines) | $($r.KB) | ``$($r.Rel)`` |")
    }
    [void]$sb.AppendLine('')
    [void]$sb.AppendLine('## DUPLICATE-BASENAME units (E1/E2 evidence is AMBIGUOUS -- verify per copy)')
    [void]$sb.AppendLine('')
    [void]$sb.AppendLine('The same unit basename exists in more than one source dir above.  E1 (same-basename .cs)')
    [void]$sb.AppendLine('and E2 ("<unit>.pas" in a header) both match on the BASENAME alone, so the verdict shown')
    [void]$sb.AppendLine('for these rows applies to EVERY copy: porting (or merely naming) ONE copy flips them all')
    [void]$sb.AppendLine('to MAPPED and thereby HIDES the other copies still being unported.')
    [void]$sb.AppendLine('')
    [void]$sb.AppendLine('Rule: never cite a MAPPED verdict for a row listed here without per-copy evidence,')
    [void]$sb.AppendLine('and never add such a name to the tools/audit-coverage.ps1 not-ported registries or to a')
    [void]$sb.AppendLine('.cs header -- that would silently close the sibling copies too.')
    [void]$sb.AppendLine('')
    [void]$sb.AppendLine('**sha256[:8] decides whether an E2 mention is transferable**: the SAME hash on two copies')
    [void]$sb.AppendLine('means they are byte-identical source, so a port of one genuinely covers the other; DIFFERENT')
    [void]$sb.AppendLine('hashes mean two distinct units and each needs its own evidence (measured on LoginGate: 12 of')
    [void]$sb.AppendLine('its 15 MAPPED rows were "borrowed" from a differing sibling -- ledger 44.3).')
    [void]$sb.AppendLine('')
    [void]$sb.AppendLine('| dir | unit | lines | verdict | sha256[:8] | distinct hashes in group |')
    [void]$sb.AppendLine('|---|---|---|---|---|---|')
    foreach ($g in ($dupGroups | Sort-Object Name)) {
        foreach ($r in ($g.Group | Sort-Object Dir)) {
            [void]$sb.AppendLine("| $($r.Dir) | $($r.Unit) | $($r.Lines) | $($r.Verdict) | $($srcHash[$r.Rel]) | $($dupDistinct[$g.Name]) |")
        }
    }
    [void]$sb.AppendLine('')
    [void]$sb.AppendLine('## UI units (a .dfm sits beside the .pas) -- the INTERFACE dimension')
    [void]$sb.AppendLine('')
    [void]$sb.AppendLine('A sibling `.dfm` is the objective marker of "this Delphi unit is a window/dialog", so the')
    [void]$sb.AppendLine('界面 half of the DoD can be counted rather than guessed.  Rows below are UI units that are')
    [void]$sb.AppendLine('NOT yet MAPPED -- i.e. the remaining interface work (a MAPPED UI unit still needs its')
    [void]$sb.AppendLine('DFM control/event reconciliation reviewed, which this script cannot judge).')
    [void]$sb.AppendLine('')
    [void]$sb.AppendLine('| dir | unit | lines | verdict | lane / source |')
    [void]$sb.AppendLine('|---|---|---|---|---|')
    foreach ($r in ($rows | Where-Object { $_.HasDfm -and $_.Verdict -ne 'MAPPED' } | Sort-Object KB -Descending)) {
        $note = if ($r.Lane) { $r.Lane } else { "``$($r.Rel)``" }
        [void]$sb.AppendLine("| $($r.Dir) | $($r.Unit) | $($r.Lines) | $($r.Verdict) | $note |")
    }
    [void]$sb.AppendLine('')
    [void]$sb.AppendLine('## CHECKLIST_ONLY units (mentioned in Checklist.md but no direct .cs evidence)')
    [void]$sb.AppendLine('')
    [void]$sb.AppendLine('| dir | unit | lines | KB |')
    [void]$sb.AppendLine('|---|---|---|---|')
    foreach ($r in ($rows | Where-Object Verdict -eq 'CHECKLIST_ONLY' | Sort-Object KB -Descending)) {
        [void]$sb.AppendLine("| $($r.Dir) | $($r.Unit) | $($r.Lines) | $($r.KB) |")
    }
    [void]$sb.AppendLine('## NON-UNIT files (backup / annotated copies -- NOT a coverage gap)')
    [void]$sb.AppendLine('')
    [void]$sb.AppendLine('| dir | file | lines | KB | source path |')
    [void]$sb.AppendLine('|---|---|---|---|---|')
    foreach ($r in ($rows | Where-Object Verdict -eq 'NONUNIT' | Sort-Object KB -Descending)) {
        [void]$sb.AppendLine("| $($r.Dir) | $($r.Unit) | $($r.Lines) | $($r.KB) | ``$($r.Rel)`` |")
    }
    [void]$sb.AppendLine('')
    [System.IO.File]::WriteAllText($reportMd, $sb.ToString(), (New-Object System.Text.UTF8Encoding($false)))
    Write-Host "report written: $reportMd" -ForegroundColor Green
}
