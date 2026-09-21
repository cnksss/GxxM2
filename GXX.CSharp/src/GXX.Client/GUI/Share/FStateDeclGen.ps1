param(
  [Parameter(Mandatory=$true)][string]$Src,
  [Parameter(Mandatory=$true)][string]$OutDir
)
# ============================================================================================
# FState.pas declaration-surface generator (parallel lane p2-client-fstate).
#
# Reads the Delphi 7 source (GBK) and emits, WITHOUT hand transcription:
#   FStateDeclManifest.g.cs  - const / type / field / method manifest tables + SHA-256 locks
#   TFrmDlg.Decl.g.cs        - abstract partial class TFrmDlg: all fields + all method stubs
#
# Members listed in $Handwritten are skipped here because a hand written partial file
# (TFrmDlg.Core.cs) supplies their real 1:1 bodies.
#
# NOTE: this file MUST stay pure ASCII. Windows PowerShell 5.1 parses .ps1 as ANSI and a
# non-ASCII byte breaks the parser (project ledger section 8.1). Comments in the generated
# C# are deliberately English/ASCII for the same reason; the hand written companion files
# carry the Chinese prose.
# ============================================================================================

$Handwritten = @(
  'Create','Destroy',
  'CloseDHeroGodBlessDlg','CloseDHeroJewelryBoxDlg',
  'RefrshDStorageViewDlgText','UpdateGuildJoinCondition',
  'GetLastHistroySendSay','GetPreHistroySendSay','GetNextHistroySendSay',
  'IsInputChatEdit','ShowChatEdit','HideChatEdit',
  'ClearScreenMagicButtons','AddScreenMagicButton','DelScreenMagicButton','FindMagicButton',
  'OnMagicButtonClick','OnMagicButtonDblClick','OnMagicButtonMove',
  'SaveMagicButtons','OpenGuildViewMemeberInfo',
  # ---- lane p14-client-fstate slice 1 (TFrmDlg.Handlers.cs) --------------------------------
  # These members have real 1:1 bodies in TFrmDlg.Handlers.cs. They MUST be skipped here:
  # in this compilation a member declared in this generated file cannot be implemented by
  # another partial file (override -> CS0115, same signature -> CS0111). Measured proof and
  # the full rationale are in TFrmDlg.Handlers.cs's header and in
  # docs/parallel-report-p14-client-fstate.md section "generated shell contract".
  'HideAllControls','RestoreHideControls',
  'DStateWinClick','AttactkModeChange','DChgGamePwdDirectPaint','MerchantDlgPaint',
  'DBotPlusAbilDirectPaint','DGameGoldDealCancelClick','OpenDUpgradeDlg','CloseDUpgradeDlg',
  'OpenDRandomCodeDlg','CloseDRandomCodeDlg','DChgGamePwdCloseClick',
  'DBottomInRealArea','DscSelect1InRealArea','DUserState1MouseDown',
  'DMinMapDlgShow','DMinMapDlgHide','DMinMapDlgResize',
  'DMouseMoveClearHints','DUpdateStatusDlgMouseLeave','DItemBagMouseMove',
  'DSayItemDlgCloseClick','DSayItemDlgMouseDown','DSayItemDlgMouseMove',
  # ---- lane p14-client-fstate slice 2 (TFrmDlg.Handlers.cs) --------------------------------
  # Close/open forwarders, guild list line scrolling, and the LieDragon close handlers.
  'DSellDlgCloseClick','DMenuCloseClick','DKsOkClick','DCloseUS1Click',
  'DNewGuildDlgCloseClick','DNewGuildNoticeClick',
  'DGDCloseClick','DGDUpClick','DGDDownClick','DGDEditNoticeClick','DGDEditGradeClick',
  'DCloseStateClick','DCloseBagClick','DBotRankClick','DBotWhisperClick',
  'DMissionDlgClick','DMissionDlgCloseClick','DOpenShopClick',
  'DBotRankingCloseClick','DGrpDlgCloseClick','DFrdCloseClick',
  'DMyHeroStateCloseClick','DMyHeroBagCloseClick',
  'DLieDragonCloseClick','DLieDragonNpcCloseClick',
  # ---- lane p14-client-fstate slice 3 (TFrmDlg.Handlers.cs) --------------------------------
  # The four members that were previously blocked by B-2 (frmMain seam) and became portable
  # once the dispatcher widened that seam, plus the two horse buttons the same seam unblocked.
  'DWebClick','DActionLogClick','DGetBackDeleteHumanClick','DCustomButtonClick',
  'DDownHorseClick','DBotHorseClick',
  # ---- lane p14-client-fstate slice 4 (TFrmDlg.Handlers.cs) --------------------------------
  # The tick-guarded family: one compare against a shared tick, a fixed +N rearm window and a
  # single forwarded send. Fully assertable with the injectable FStateSeamClock.
  'DGDHomeClick','DGDListClick',
  'DBotUserShopClick','DBotRankingClick','DBotFriendClick',
  # ---- lane p14-client-fstate slice 5 (TFrmDlg.Handlers.cs) --------------------------------
  # Rest of the "one guard + one forwarded send" family that only needed MShare globals.
  'DBotTradeClick','BotChallengeClick','DDealCloseClick','DealZeroGold',
  'DChallengeCloseClick','ChallengeZeroGold',
  # ---- lane p14-client-fstate slice 6 (TFrmDlg.Handlers.cs) --------------------------------
  # Help-button throttle (field + difference guard) and the update-dialog reconnect handler.
  'DControlHelpClick','DUpdateStatusDlgDblClick',
  # ---- lane p14-client-fstate slice 7 (TFrmDlg.Handlers.cs) --------------------------------
  # Group-mode toggle pair (two entry points sharing one handler body) and the
  # deal-item / game-gold-deal clear handlers.
  'DBotGroupMouseDown','DGrpAllowGroupClick','DealItemReturnBag','DGameGoldDealMenuDlgCloseClick'
)

# NOTE on the source encoding: the file this generator reads is the **UTF-8 mirror**
# (`_analysis/utf8_mirror/...`), not the GBK `Source/` copy. Reading it with an explicit
# UTF-8 decoder is what keeps the Delphi comments that are echoed into the generated C#
# readable; `Get-Content -Encoding Default` is not portable (PS 5.1 = ANSI, PS 7 = UTF-8),
# which is how the first generation of these files ended up with mojibake comments.
$lines = [System.IO.File]::ReadAllLines($Src, [System.Text.Encoding]::UTF8)

# ---------------------------------------------------------------- const section
$consts = New-Object System.Collections.ArrayList
for ($i = 48; $i -lt 57; $i++) {
  $t = $lines[$i].Trim(); if ($t -eq '') { continue }
  [void]$consts.Add([pscustomobject]@{
    Line = $i + 1; Name = ($t -split '=')[0].Trim(); Value = ($t -split '=',2)[1].Trim()
  })
}

# ---------------------------------------------------------------- type section
$types = New-Object System.Collections.ArrayList
for ($i = 59; $i -lt 1114; $i++) {
  if ($lines[$i] -notmatch '^  ([A-Za-z_][A-Za-z0-9_]*)\s*=\s*(.*)$') { continue }
  $name = $Matches[1]; $rest = $Matches[2].Trim()
  if ($name -eq 'TFrmDlg') { continue }
  $kind = 'other'
  if     ($rest -match '^class\s*\(\s*([A-Za-z0-9_]+)\s*\)') { $kind = 'class:' + $Matches[1] }
  elseif ($rest -match '^class\s*$')                        { $kind = 'class' }
  elseif ($rest -match '^record\b')                         { $kind = 'record' }
  elseif ($rest -match '^\(([A-Za-z0-9_,\s]+)\)\s*;')       { $kind = 'enum' }
  elseif ($rest -match '^array\b')                          { $kind = 'array' }
  elseif ($rest -match '^\^')                               { $kind = 'pointer' }
  [void]$types.Add([pscustomobject]@{ Line = $i + 1; Name = $name; Kind = $kind; Text = $rest })
}

# ---------------------------------------------------------------- TFrmDlg body
$bodyStart = 310; $bodyEnd = 1113
$fields = New-Object System.Collections.ArrayList
$methods = New-Object System.Collections.ArrayList
$visibility = 'published'
$i = $bodyStart + 1
while ($i -lt $bodyEnd) {
  $raw = ($lines[$i] -replace '//.*$','')
  # Delphi allows an inline brace comment inside a declaration, e.g. FState.pas:939
  # ends with ":Boolean {<chinese comment>});"
  # Strip it for parsing but keep it in the recorded declaration text for 1:1 fidelity.
  $t = ($raw -replace '\{[^}]*\}','').Trim()
  if ($t -eq '') { $i++; continue }
  if ($t -match '^(private|protected|public|published)\s*$') { $visibility = $Matches[1]; $i++; continue }
  $decl = $t; $declRaw = $raw.Trim(); $startLine = $i + 1; $j = $i
  while (-not $decl.TrimEnd().EndsWith(';') -and $j + 1 -lt $bodyEnd) {
    $j++
    $raw = ($lines[$j] -replace '//.*$','')
    $decl = $decl + ' ' + (($raw -replace '\{[^}]*\}','').Trim())
    $declRaw = $declRaw + ' ' + $raw.Trim()
  }
  $i = $j + 1
  if ($decl -match '^(function|procedure|constructor|destructor)\s+([A-Za-z_][A-Za-z0-9_]*)') {
    [void]$methods.Add([pscustomobject]@{ Line = $startLine; Kind = $Matches[1]; Name = $Matches[2]; Visibility = $visibility; Decl = $decl; DeclRaw = $declRaw })
    continue
  }
  if ($decl -match '^([A-Za-z_][A-Za-z0-9_]*(?:\s*,\s*[A-Za-z_][A-Za-z0-9_]*)*)\s*:\s*(.+);$') {
    $names = $Matches[1]; $ty = $Matches[2].Trim()
    foreach ($n in ($names -split ',')) {
      [void]$fields.Add([pscustomobject]@{ Line = $startLine; Name = $n.Trim(); Type = $ty; Visibility = $visibility })
    }
    continue
  }
  if ($decl -match '^property\s') {
    [void]$methods.Add([pscustomobject]@{ Line = $startLine; Kind = 'property'; Name = (($decl -replace '^property\s+','') -split '\s*:')[0]; Visibility = $visibility; Decl = $decl; DeclRaw = $declRaw })
    continue
  }
  throw ("UNPARSED TFrmDlg member at line " + $startLine + ": " + $decl)
}

# ---------------------------------------------------------------- type mapping
# PowerShell switch is case-insensitive, which matches Delphi's case-insensitive identifiers.
function Map-Type([string]$s) {
  $s = $s.Trim()
  if ($s -match '^array\s*\[\s*(-?\d+)\s*\.\.\s*(-?\d+)\s*\]\s*of\s+(.+)$') {
    $lo = [int]$Matches[1]; $hi = [int]$Matches[2]
    return (Map-Type $Matches[3]) + '[' + ($hi - $lo + 1) + ']'
  }
  if ($s -match '^\^(.*)$') { return 'p' + (Map-Type $Matches[1]) }
  switch ($s) {
    'Integer' { return 'int' }
    'LongInt' { return 'int' }
    'Cardinal' { return 'uint' }
    'LongWord' { return 'uint' }
    'DWORD' { return 'uint' }
    'Word' { return 'ushort' }
    'SmallInt' { return 'short' }
    'Byte' { return 'byte' }
    'ShortInt' { return 'sbyte' }
    'Boolean' { return 'bool' }
    'Int64' { return 'long' }
    'Comp' { return 'long' }
    'string' { return 'string' }
    'WideString' { return 'string' }
    'Char' { return 'char' }
    'Single' { return 'float' }
    'Double' { return 'double' }
    'TObject' { return 'object' }
    'Pointer' { return 'IntPtr' }
    'TColor' { return 'TColor' }
    'TPoint' { return 'TPoint' }
    'TRect' { return 'TRect' }
    'TShiftState' { return 'TShiftState' }
    'TMouseButton' { return 'TMouseButton' }
    'TGridDrawState' { return 'TGridDrawState' }
    'TMsgDlgButtons' { return 'TMsgDlgButtons' }
    'TModalResult' { return 'TModalResult' }
    'TOnClickEx' { return 'TOnClickEx' }
    'TClickSound' { return 'TClickSound' }
    'TImageType' { return 'TImageType' }
    'TTexture' { return 'TTexture' }
    'TGraphic' { return 'TGraphic' }
    'TGameImages' { return 'TGameImages' }
    'TGuiImageIndex' { return 'TGuiImageIndex' }
    'TClientItem' { return 'TClientItem' }
    'TStorageHeroInfo' { return 'TStorageHeroInfo' }
    'TGuildMemeberInfo' { return 'TGuildMemeberInfo' }
    'TGuardianLevelItemCounts' { return 'TGuardianLevelItemCounts' }
    'TClientVersion' { return 'TClientVersion' }
    'TStringList' { return 'TStringList' }
    'TGList' { return 'TGList' }
    'TMemo' { return 'TMemo' }
    'TList' { return 'TList' }
    'ISuperObject' { return 'ISuperObject' }
    'TMemoryStream' { return 'TMemoryStream' }
    'THashedStringList' { return 'THashedStringList' }
    'TUserEntry' { return 'TUserEntry' }
    'pTUserEntry' { return 'pTUserEntry' }
    'TUserEntryAdd' { return 'TUserEntryAdd' }
    'pTUserEntryAdd' { return 'pTUserEntryAdd' }
    'pTUserCharacterInfo' { return 'pTUserCharacterInfo' }
    'TActor' { return 'TActor' }
    'PTClientMagic' { return 'PTClientMagic' }
    'PTClientItem' { return 'PTClientItem' }
    'pTStdItem' { return 'pTStdItem' }
    'PClientUserShop' { return 'PClientUserShop' }
    'PSellPlayerMoney' { return 'PSellPlayerMoney' }
    'PTSellPlayerStorageViewHeader' { return 'PTSellPlayerStorageViewHeader' }
    'PTSellPlayerStorageExtViewHeader' { return 'PTSellPlayerStorageExtViewHeader' }
    'PGuildJoinUser' { return 'PGuildJoinUser' }
    'TDxControlEngine' { return 'TDxControlEngine' }
    'TDxChatMemo' { return 'TDxChatMemo' }
    'TDxListView' { return 'TDxListView' }
    'TDxImageEdit' { return 'TDxImageEdit' }
    'TDxTreeNode' { return 'TDxTreeNode' }
    'TDiceInfo' { return 'TDiceInfo' }
    'TSpotDlgMode' { return 'TSpotDlgMode' }
    'TGuildGroupList' { return 'TGuildGroupList' }
    'TShowGuildList' { return 'TShowGuildList' }
    'TGuildJoinUserList' { return 'TGuildJoinUserList' }
    'TArrHintWindows' { return 'TArrHintWindows' }
    'TDXLabel' { return 'TDxLabel' }      # original typo, FState.pas:481/484
    'TDXListView' { return 'TDxListView' } # original typo, FState.pas:484
    default { return $s }                 # Dx* control classes keep their names
  }
}

# ---------------------------------------------------------------- C# type overrides
# A few Delphi declarations lose their real type during Map-Type (the original spells them
# with a type that does not exist under that name in the managed tree). For those the real
# managed type is pinned here BY HAND so the .g.cs stays fully reproducible.
#
#   FSayItemHintWin : Delphi `object`  ->  GXX.Client.Scenes.THintWindows
#     original FState.pas:503 is `FSayItemHintWin:TObject;` and is only ever written at
#     FState.pas:1555 as `FSayItemHintWin := DrawScrn.THintWindows.Create;`.
#     D-P10-06 puts THintWindows' home in GXX.Client.Scenes (DrawScrn.pas:408),
#     so the managed field type is THintWindows instead of a bare `object`.
$CsTypeOverrides = @{
  'FSayItemHintWin' = 'THintWindows'
}

function Map-FieldType([string]$name, [string]$delphiType) {
  if ($CsTypeOverrides.ContainsKey($name)) { return $CsTypeOverrides[$name] }
  return (Map-Type $delphiType)
}

function Map-Default([string]$s) {
  $s = $s.Trim()
  if ($s -eq 'True')  { return 'true' }
  if ($s -eq 'False') { return 'false' }
  if ($s -eq 'nil')   { return 'null' }
  if ($s -eq "''")    { return '""' }
  return $s
}

function Map-Params([string]$p) {
  if ([string]::IsNullOrWhiteSpace($p)) { return '' }
  $out = New-Object System.Collections.ArrayList
  foreach ($part in ($p -split ';')) {
    $p2 = $part.Trim(); if ($p2 -eq '') { continue }
    $mod = ''
    if ($p2 -match '^(var|const|out)\s+(.+)$') { $mod = $Matches[1]; $p2 = $Matches[2].Trim() }
    $def = ''
    if ($p2 -match '^(.+?)\s*=\s*(.+)$') { $p2 = $Matches[1].Trim(); $def = Map-Default $Matches[2] }
    if ($p2 -notmatch '^(.+?)\s*:\s*(.+)$') { throw ("bad param: " + $part) }
    $names = $Matches[1]; $ty = Map-Type $Matches[2]
    $prefix = ''
    if ($mod -eq 'var') { $prefix = 'ref ' } elseif ($mod -eq 'out') { $prefix = 'out ' }
    foreach ($n in ($names -split ',')) {
      $one = $prefix + $ty + ' ' + $n.Trim()
      if ($def -ne '') { $one = $one + ' = ' + $def }
      [void]$out.Add($one)
    }
  }
  return ($out -join ', ')
}

function Sha256-Lines([string[]]$items) {
  $joined = ($items -join "`n")
  $sha = [System.Security.Cryptography.SHA256]::Create()
  $bytes = $sha.ComputeHash([System.Text.Encoding]::UTF8.GetBytes($joined))
  return (($bytes | ForEach-Object { $_.ToString('x2') }) -join '')
}

function XmlEscape([string]$s) {
  return $s.Replace('&','&amp;').Replace('<','&lt;').Replace('>','&gt;')
}

$nl = "`r`n"
$utf8 = New-Object System.Text.UTF8Encoding($false)

# ---------------------------------------------------------------- manifest file
$sb = New-Object System.Text.StringBuilder
[void]$sb.Append('// <auto-generated>').Append($nl)
[void]$sb.Append('//     Generated by src/GXX.Client/GUI/Share/FStateDeclGen.ps1 from the Delphi source;').Append($nl)
[void]$sb.Append('//     do not hand transpose. Regenerate instead of editing.').Append($nl)
[void]$sb.Append('//     Source: Source/Client-HGE/GUI/Share/FState.pas').Append($nl)
[void]$sb.Append('//       const 49-57 / type 60-1114 / var 1117 / const 1133 / TFrmDlg members 311-1114').Append($nl)
[void]$sb.Append('// </auto-generated>').Append($nl).Append($nl)
[void]$sb.Append('namespace GXX.Client.GUI.Share;').Append($nl).Append($nl)

[void]$sb.Append('/// <summary>FState.pas unit level constants (source lines 49-57: 8 scalars + 1 string array).</summary>').Append($nl)
[void]$sb.Append('public static class FStateConstTable').Append($nl).Append('{').Append($nl)
[void]$sb.Append('    /// <summary>Constant count.</summary>').Append($nl)
[void]$sb.Append('    public const int Count = ' + $consts.Count + ';').Append($nl).Append($nl)
[void]$sb.Append('    /// <summary>SHA-256 of the names joined by ''\n'' (UTF-8), declaration order.</summary>').Append($nl)
[void]$sb.Append('    public const string NamesSha256 = "' + (Sha256-Lines ($consts | ForEach-Object { $_.Name })) + '";').Append($nl).Append($nl)
[void]$sb.Append('    /// <summary>Constant names in source declaration order.</summary>').Append($nl)
[void]$sb.Append('    public static readonly string[] Names =').Append($nl).Append('    {').Append($nl)
foreach ($c in $consts) { [void]$sb.Append('        "' + $c.Name + '", // line ' + $c.Line + ' : ' + $c.Value).Append($nl) }
[void]$sb.Append('    };').Append($nl)
[void]$sb.Append('}').Append($nl).Append($nl)

[void]$sb.Append('/// <summary>FState.pas type declarations (source lines 60-1114: ' + $types.Count + ' non TFrmDlg types).</summary>').Append($nl)
[void]$sb.Append('public static class FStateTypeTable').Append($nl).Append('{').Append($nl)
[void]$sb.Append('    /// <summary>Type count.</summary>').Append($nl)
[void]$sb.Append('    public const int Count = ' + $types.Count + ';').Append($nl).Append($nl)
[void]$sb.Append('    /// <summary>SHA-256 of the names joined by ''\n'' (UTF-8), declaration order.</summary>').Append($nl)
[void]$sb.Append('    public const string NamesSha256 = "' + (Sha256-Lines ($types | ForEach-Object { $_.Name })) + '";').Append($nl).Append($nl)
[void]$sb.Append('    /// <summary>Type names in source declaration order.</summary>').Append($nl)
[void]$sb.Append('    public static readonly string[] Names =').Append($nl).Append('    {').Append($nl)
foreach ($t in $types) { [void]$sb.Append('        "' + $t.Name + '", // line ' + $t.Line + ' : ' + $t.Kind).Append($nl) }
[void]$sb.Append('    };').Append($nl).Append($nl)
[void]$sb.Append('    /// <summary>Source kind marker per <see cref="Names"/> entry (class:X / record / enum / array / pointer).</summary>').Append($nl)
[void]$sb.Append('    public static readonly string[] Kinds =').Append($nl).Append('    {').Append($nl)
foreach ($t in $types) { [void]$sb.Append('        "' + $t.Kind + '", // ' + $t.Name).Append($nl) }
[void]$sb.Append('    };').Append($nl)
[void]$sb.Append('}').Append($nl).Append($nl)

[void]$sb.Append('/// <summary>TFrmDlg field declarations (source lines 311-1114: ' + $fields.Count + ' entries).</summary>').Append($nl)
[void]$sb.Append('public static class TFrmDlgFieldTable').Append($nl).Append('{').Append($nl)
[void]$sb.Append('    /// <summary>Field count (multi name lines are expanded, so this exceeds the line count).</summary>').Append($nl)
[void]$sb.Append('    public const int Count = ' + $fields.Count + ';').Append($nl).Append($nl)
[void]$sb.Append('    /// <summary>SHA-256 of the names joined by ''\n'' (UTF-8), declaration order.</summary>').Append($nl)
[void]$sb.Append('    public const string NamesSha256 = "' + (Sha256-Lines ($fields | ForEach-Object { $_.Name })) + '";').Append($nl).Append($nl)
[void]$sb.Append('    /// <summary>Field names in source declaration order.</summary>').Append($nl)
[void]$sb.Append('    public static readonly string[] Names =').Append($nl).Append('    {').Append($nl)
foreach ($f in $fields) { [void]$sb.Append('        "' + $f.Name + '", // line ' + $f.Line + ' : ' + (Map-FieldType $f.Name $f.Type)).Append($nl) }
[void]$sb.Append('    };').Append($nl)
[void]$sb.Append('}').Append($nl).Append($nl)

[void]$sb.Append('/// <summary>TFrmDlg method/property declarations (source lines 311-1114: ' + $methods.Count + ' entries).</summary>').Append($nl)
[void]$sb.Append('public static class TFrmDlgMethodTable').Append($nl).Append('{').Append($nl)
[void]$sb.Append('    /// <summary>Member count (incl. 5 overload groups, 1 property, 1 constructor, 1 destructor).</summary>').Append($nl)
[void]$sb.Append('    public const int Count = ' + $methods.Count + ';').Append($nl).Append($nl)
[void]$sb.Append('    /// <summary>SHA-256 of the names joined by ''\n'' (UTF-8), declaration order (overload names repeat).</summary>').Append($nl)
[void]$sb.Append('    public const string NamesSha256 = "' + (Sha256-Lines ($methods | ForEach-Object { $_.Name })) + '";').Append($nl).Append($nl)
[void]$sb.Append('    /// <summary>Member names in source declaration order (overloads repeat).</summary>').Append($nl)
[void]$sb.Append('    public static readonly string[] Names =').Append($nl).Append('    {').Append($nl)
foreach ($m in $methods) { [void]$sb.Append('        "' + $m.Name + '", // line ' + $m.Line + ' : ' + $m.Kind + ' ' + $m.Visibility).Append($nl) }
[void]$sb.Append('    };').Append($nl).Append($nl)
[void]$sb.Append('    /// <summary>Full source declaration text, 1:1, for verbatim read back comparison.</summary>').Append($nl)
[void]$sb.Append('    public static readonly string[] Decls =').Append($nl).Append('    {').Append($nl)
foreach ($m in $methods) { [void]$sb.Append('        ' + ($m.DeclRaw | ConvertTo-Json -Compress) + ', // line ' + $m.Line).Append($nl) }
[void]$sb.Append('    };').Append($nl)
[void]$sb.Append('}').Append($nl)

[System.IO.File]::WriteAllText((Join-Path $OutDir 'FStateDeclManifest.g.cs'), $sb.ToString(), $utf8)

# ---------------------------------------------------------------- TFrmDlg declarations
$sb2 = New-Object System.Text.StringBuilder
[void]$sb2.Append('// <auto-generated>').Append($nl)
[void]$sb2.Append('//     Generated by src/GXX.Client/GUI/Share/FStateDeclGen.ps1 from the Delphi source;').Append($nl)
[void]$sb2.Append('//     do not hand transpose. Regenerate instead of editing.').Append($nl)
[void]$sb2.Append('//     Source: Source/Client-HGE/GUI/Share/FState.pas, TFrmDlg class body lines 311-1114.').Append($nl)
[void]$sb2.Append('// </auto-generated>').Append($nl).Append($nl)
[void]$sb2.Append('using System;').Append($nl)
[void]$sb2.Append('using GXX.Client.GUI.DxComponent;').Append($nl)
[void]$sb2.Append('using GXX.Client.GUI.Mir;').Append($nl)
[void]$sb2.Append('using GXX.Client.Scenes;').Append($nl)
[void]$sb2.Append('using GXX.Core.Protocol;').Append($nl)
[void]$sb2.Append('using GXX.Core.Util;').Append($nl)
[void]$sb2.Append('using TGList = GXX.Core.Protocol.SDK.TGList;').Append($nl).Append($nl)
[void]$sb2.Append('namespace GXX.Client.GUI.Share;').Append($nl).Append($nl)
[void]$sb2.Append('/// <summary>').Append($nl)
[void]$sb2.Append('/// FState.pas:311 TFrmDlg (top level window base class) declaration surface: ' + $fields.Count + ' fields + ' + $methods.Count + ' methods/properties.').Append($nl)
[void]$sb2.Append('/// FState.pas implements part of these in its implementation section; every member not yet').Append($nl)
[void]$sb2.Append('/// ported throws instead of silently returning a default, so an accidental call is loud.').Append($nl)
[void]$sb2.Append('///').Append($nl)
[void]$sb2.Append('/// Delphi declares TFrmDlg as a concrete class whose `virtual; abstract;` members raise').Append($nl)
[void]$sb2.Append('/// EAbstractError only when called. To keep that runtime semantics (and to keep the class').Append($nl)
[void]$sb2.Append('/// instantiable for the ported members, exactly as Delphi allows) these are modelled as').Append($nl)
[void]$sb2.Append('/// virtual members that throw, rather than C# `abstract` members.').Append($nl)
[void]$sb2.Append('/// </summary>').Append($nl)
[void]$sb2.Append('public partial class TFrmDlg').Append($nl).Append('{').Append($nl)

foreach ($f in $fields) {
  if ($f.Visibility -eq 'private') { $v = 'private' } elseif ($f.Visibility -eq 'protected') { $v = 'protected' } else { $v = 'public' }
  $ct = Map-FieldType $f.Name $f.Type
  [void]$sb2.Append('    /// <summary>Source line ' + $f.Line + ' : ' + (XmlEscape $f.Type) + ' (' + $f.Visibility + ')</summary>').Append($nl)
  if ($ct -match '^(.+)\[(\d+)\]$') {
    # Delphi `array[a..b] of T` is a value field; C# needs an initialiser for the element count.
    [void]$sb2.Append('    ' + $v + ' ' + $Matches[1] + '[] ' + $f.Name + ' = new ' + $Matches[1] + '[' + $Matches[2] + '];').Append($nl)
  } else {
    [void]$sb2.Append('    ' + $v + ' ' + $ct + ' ' + $f.Name + ';').Append($nl)
  }
}
[void]$sb2.Append($nl)

$skipped = New-Object System.Collections.ArrayList
foreach ($m in $methods) {
  if ($Handwritten -contains $m.Name) { [void]$skipped.Add($m.Name); continue }
  $d = $m.Decl
  $isAbstract = ($d -match 'abstract')
  if ($m.Kind -eq 'property') {
    if ($d -match '^property\s+([A-Za-z_][A-Za-z0-9_]*)\s*:\s*([A-Za-z_][A-Za-z0-9_]*)\s+read\s+([A-Za-z_][A-Za-z0-9_]*)') {
      [void]$sb2.Append('    /// <summary>Source line ' + $m.Line + ' : ' + (XmlEscape $d) + '</summary>').Append($nl)
      [void]$sb2.Append('    public ' + (Map-Type $Matches[2]) + ' ' + $Matches[1] + ' => ' + $Matches[3] + ';').Append($nl)
    } else { throw ("bad property: " + $d) }
    continue
  }
  if ($m.Kind -eq 'constructor') {
    [void]$sb2.Append('    /// <summary>Source line ' + $m.Line + ' : ' + (XmlEscape $d) + '</summary>').Append($nl)
    [void]$sb2.Append('    protected TFrmDlg()').Append($nl)
    [void]$sb2.Append('        => throw new NotSupportedException("TFrmDlg.Create: not ported yet (FState.pas:' + $m.Line + ')");').Append($nl)
    continue
  }
  if ($m.Kind -eq 'destructor') {
    [void]$sb2.Append('    /// <summary>Source line ' + $m.Line + ' : ' + (XmlEscape $d) + '</summary>').Append($nl)
    [void]$sb2.Append('    public virtual void Destroy()').Append($nl)
    [void]$sb2.Append('        => throw new NotSupportedException("TFrmDlg.Destroy: not ported yet (FState.pas:' + $m.Line + ')");').Append($nl)
    continue
  }
  if ($d -notmatch '^(function|procedure)\s+([A-Za-z_][A-Za-z0-9_]*)\s*(?:\((.*?)\))?\s*(?::\s*([A-Za-z_][A-Za-z0-9_]*))?\s*;(.*)$') {
    throw ("bad method decl: " + $d)
  }
  $kind = $Matches[1]; $name = $Matches[2]; $params = $Matches[3]; $ret = $Matches[4]
  $csParams = Map-Params $params
  $retType = if ($kind -eq 'function') { Map-Type $ret } else { 'void' }
  $modifier = if ($isAbstract) { 'public abstract ' } else { 'public virtual ' }
  $why = if ($isAbstract) { 'abstract in Delphi' } else { 'not ported yet' }
  [void]$sb2.Append('    /// <summary>Source line ' + $m.Line + ' : ' + (XmlEscape $d) + '</summary>').Append($nl)
  $why = if ($isAbstract) { 'abstract in Delphi' } else { 'not ported yet' }
  [void]$sb2.Append('    public virtual ' + $retType + ' ' + $name + '(' + $csParams + ')').Append($nl)
  [void]$sb2.Append('        => throw new NotSupportedException("TFrmDlg.' + $name + ': ' + $why + ' (FState.pas:' + $m.Line + ')");').Append($nl)
}
[void]$sb2.Append('}').Append($nl)
[System.IO.File]::WriteAllText((Join-Path $OutDir 'TFrmDlg.Decl.g.cs'), $sb2.ToString(), $utf8)

"CONSTS=$($consts.Count) TYPES=$($types.Count) FIELDS=$($fields.Count) METHODS=$($methods.Count)"
"SKIPPED(handwritten)=$(($skipped | Select-Object -Unique) -join ',')"
