# =====================================================================================
# GenGuiRecords.ps1 - provenance generator for LoadDx/GuiRecords.g.cs
#
# WHY: LoadDxControl.pas / LoadDxControlEx.pas parse the .GUI binary stream by reading
#      whole Delphi records with "ReadMemory(x, SizeOf(TXxx))".  A faithful C# port must
#      therefore reproduce Delphi's *default* ({$A8}, {$MINENUMSIZE 1}) record layout
#      bit-for-bit: field offsets, padding and total size.  Hand-transcribing 45 records
#      is error prone, so this script extracts every type declaration from
#      Source/Client-HGE/DxComponent/DxComponents.pas (the ONLY declaration site of the
#      TGui* records in the whole tree), computes the Delphi layout and emits strongly
#      typed C# records with ReadAt/WriteAt plus a DelphiField[] offset table.
#
# INPUT : Source/Client-HGE/DxComponent/DxComponents.pas   (GBK / code page 936)
# OUTPUT: src/GXX.Client/LoadDx/GuiRecords.g.cs            (UTF-8, no BOM)
#
# The generated file is committed; this script is the reproducible provenance and the
# read-back audit (it prints the full Type;SizeOf;Align;Fields table on stdout).
#
# NOTE: this file is deliberately ASCII-ONLY - Windows PowerShell 5.1 parses .ps1 as ANSI
#       (see docs/Parallel-Dispatch-Ledger 8.1).  All Chinese documentation lives in the
#       hand written LoadDx sources and in docs/Parallel-Report-p2-client-loaddx.md.
# =====================================================================================
[CmdletBinding()]
param(
    # Default is resolved relative to $PSScriptRoot so that this script stays pure ASCII
    # (the repository root path itself contains non-ASCII characters - see ledger 8.1).
    [string]$SourcePas = '',
    [string]$OutFile   = ''
)

$ErrorActionPreference = 'Stop'
if ([string]::IsNullOrEmpty($SourcePas)) {
    $repoRoot  = Resolve-Path (Join-Path $PSScriptRoot '..\..\..\..\..')
    $SourcePas = Join-Path $repoRoot 'Source\Client-HGE\DxComponent\DxComponents.pas'
}
if ([string]::IsNullOrEmpty($OutFile)) {
    $OutFile = Join-Path (Split-Path -Parent $PSScriptRoot) 'GuiRecords.g.cs'
}
if (-not (Test-Path -LiteralPath $SourcePas)) { throw "source not found: $SourcePas" }
Write-Host "source: $SourcePas"

$gbk = [System.Text.Encoding]::GetEncoding(936)
$text = [System.IO.File]::ReadAllText($SourcePas, $gbk)

# ---- 1. interface section only ------------------------------------------------------
$ifStart = $text.IndexOf("`ninterface")
$ifEnd   = $text.IndexOf("`nimplementation")
if ($ifStart -lt 0 -or $ifEnd -lt 0) { throw 'cannot locate interface/implementation' }
$lineOffset = ($text.Substring(0, $ifStart) -split "`n").Count   # 1-based line of $iface[0]
$iface = $text.Substring($ifStart, $ifEnd - $ifStart)

# ---- 2. strip comments (//, {}, (*) ) ----------------------------------------------
$iface = [regex]::Replace($iface, '(?s)\(\*.*?\*\)', ' ')
$iface = [regex]::Replace($iface, '(?s)\{.*?\}', ' ')
$iface = [regex]::Replace($iface, '//[^\r\n]*', ' ')

# ---- 3. split declarations ----------------------------------------------------------
$declRx = [regex]'(?m)^[ \t]{2}([A-Za-z_]\w*)[ \t]*=[ \t]*'
$matches = $declRx.Matches($iface)
$decls = @{}
$order = New-Object System.Collections.Generic.List[string]
for ($i = 0; $i -lt $matches.Count; $i++) {
    $name = $matches[$i].Groups[1].Value
    $start = $matches[$i].Index + $matches[$i].Length
    $stop = if ($i + 1 -lt $matches.Count) { $matches[$i + 1].Index } else { $iface.Length }
    $body = $iface.Substring($start, $stop - $start)
    if (-not $decls.ContainsKey($name)) {
        $decls[$name] = $body
        [void]$order.Add($name)
    }
}

# ---- 4. type model ------------------------------------------------------------------
# kind: scalar | enum | set | array | record | shortstr | skip
$types = @{}
function Add-Scalar($n, $size, $align, $cs, $kind) {
    $script:types[$n] = [pscustomobject]@{ Name = $n; Kind = $kind; Size = $size; Align = $align; Cs = $cs }
}

Add-Scalar 'Integer'  4 4 'int'    'scalar'
Add-Scalar 'LongInt'  4 4 'int'    'scalar'
Add-Scalar 'LongWord' 4 4 'int'    'scalar'
Add-Scalar 'Cardinal' 4 4 'int'    'scalar'
Add-Scalar 'DWORD'    4 4 'int'    'scalar'
Add-Scalar 'TColor'   4 4 'int'    'scalar'
Add-Scalar 'Word'     2 2 'ushort' 'scalar'
Add-Scalar 'Byte'     1 1 'byte'   'scalar'
Add-Scalar 'Boolean'  1 1 'bool'   'scalar'
Add-Scalar 'Char'     1 1 'byte'   'scalar'
Add-Scalar 'TDateTime' 8 8 'double' 'scalar'
Add-Scalar 'TFontStyles' 1 1 'byte' 'set'
Add-Scalar 'TShiftState' 1 1 'byte' 'set'
Add-Scalar 'TMouseEvents' 1 1 'TMouseEvents' 'enum'

# types that already exist in GXX.Client.DxComponent (never regenerate: CS0101/CS0104 risk).
# TAlignEx / TDrawAligment became DxComponent-owned on 2026-09-20 (lane p2-dxcontrols-rest:
# DxImageForm.cs / DxControls.cs) - they are the single home for those two enums now, so we
# reference them instead of emitting our own copies.
$existingEnums = @('TClientVersion','TReferenceX','TButtonAnimationShowType','TGuiType','TImageType','TMouseEvents',
                   'TProgressValueType','TLineStyle','TButtonStyle','TClickSound',
                   'TAlignEx','TDrawAligment')
# Delphi name -> C# name when the seam declares the type somewhere other than namespace level
$typeNameRemap = @{ 'TClickSound' = 'TClickSound' }   # nested inside TDxImageButton in DxLabel.cs
function Map-CsName([string]$name) { if ($typeNameRemap.ContainsKey($name)) { return $typeNameRemap[$name] } return $name }
# VCL enums referenced by the records but not declared in DxComponents.pas -> generated here
$vclEnumValues = @{
    'TTabPosition'        = @('tpTop','tpBottom','tpLeft','tpRight')                                # ComCtrls
    'TVerticalAlignment'  = @('taVerticalCenter','taTopJustify','taBottomJustify')                  # Classes
    'TScrollStyle'        = @('ssNone','ssHorizontal','ssVertical','ssBoth','ssAutoHorizontal','ssAutoVertical','ssAutoBoth')  # Forms
}
$newEnums = @('TTabPosition','TVerticalAlignment','TScrollStyle')
# types aliased onto an existing seam type
$types['TAlignment'] = [pscustomobject]@{ Name='TAlignment'; Kind='enum'; Size=1; Align=1; Cs='TDxAlignment'; Values=@() }
$types['TRect']      = [pscustomobject]@{ Name='TRect';      Kind='rect';  Size=16; Align=4; Cs='TDxRect' }
$types['TPoint']     = [pscustomobject]@{ Name='TPoint';     Kind='point'; Size=8;  Align=4; Cs='TDxPoint' }

function Get-Bound([string]$expr) {
    # evaluates the simple integer expressions used as array bounds (e.g. "8 - 1")
    $e = [regex]::Replace($expr, '\s+', '')
    if ($e -match '^(\d+)$') { return [int]$Matches[1] }
    if ($e -match '^(\d+)-(\d+)$') { return ([int]$Matches[1] - [int]$Matches[2]) }
    if ($e -match '^(\d+)\+(\d+)$') { return ([int]$Matches[1] + [int]$Matches[2]) }
    throw "unsupported array bound '$expr'"
}

function Get-FieldList([string]$body) {
    $fields = New-Object System.Collections.Generic.List[object]
    $inner = $body
    $ri = $inner.IndexOf('record')
    $ei = $inner.LastIndexOf('end')
    if ($ri -lt 0 -or $ei -lt 0) { return $fields }
    $inner = $inner.Substring($ri + 6, $ei - $ri - 6)
    foreach ($chunk in ($inner -split ';')) {
        $c = $chunk.Trim()
        if ($c.Length -eq 0) { continue }
        $c = [regex]::Replace($c, '\s+', ' ')
        $colon = $c.IndexOf(':')
        if ($colon -lt 0) { continue }
        $names = $c.Substring(0, $colon).Trim() -split ','
        $typeName = $c.Substring($colon + 1).Trim()
        foreach ($n in $names) { $t = $n.Trim(); if ($t.Length -gt 0) { $fields.Add([pscustomobject]@{ Name = $t; Type = $typeName }) } }
    }
    return $fields
}

function Get-SourceLine([string]$declName) {
    $rx = [regex]("(?m)^[ \t]{2}" + [regex]::Escape($declName) + "[ \t]*=")
    $m = $rx.Match($iface)
    if (-not $m.Success) { return 0 }
    $prefix = $iface.Substring(0, $m.Index)
    return $lineOffset + ($prefix -split "`n").Count - 1
}

# ---- first pass: record skeletons + enums -------------------------------------------
foreach ($name in $order) {
    $body = $decls[$name]
    if ($types.ContainsKey($name)) { continue }
    $trim = $body.TrimStart()
    if ($trim.StartsWith('(')) {
        $inner = $trim.Substring(1, $trim.IndexOf(')') - 1)
        $vals = @()
        foreach ($v in ($inner -split ',')) { $vv = $v.Trim(); if ($vv.Length -gt 0) { $vals += $vv } }
        $types[$name] = [pscustomobject]@{ Name = $name; Kind = 'enum'; Size = 1; Align = 1; Cs = $name; Values = $vals }
    }
    elseif ($trim.StartsWith('record') -or $trim.StartsWith('packed record')) {
        $packed = $trim.StartsWith('packed')
        $types[$name] = [pscustomobject]@{ Name = $name; Kind = 'record'; Packed = $packed; Fields = (Get-FieldList $body) }
    }
    elseif ($trim.StartsWith('set of ')) {
        $types[$name] = [pscustomobject]@{ Name = $name; Kind = 'set'; Size = 1; Align = 1; Cs = 'byte' }
    }
    elseif ($trim.StartsWith('array[')) {
        $m = [regex]::Match($trim, 'array\[(\d+)\.\.([^\]]+)\]\s+of\s+([\w\.]+)')
        if ($m.Success) {
            $types[$name] = [pscustomobject]@{ Name = $name; Kind = 'array'; Lo = [int]$m.Groups[1].Value; Hi = (Get-Bound $m.Groups[2].Value); Elem = $m.Groups[3].Value }
        }
    }
}
foreach ($n in $newEnums) {
    if (-not $types.ContainsKey($n)) { $types[$n] = [pscustomobject]@{ Name = $n; Kind = 'enum'; Size = 1; Align = 1; Cs = $n; Values = $vclEnumValues[$n] } }
}

$script:layoutCache = @{}
function Get-RecordLayout([string]$name) {
    if ($script:layoutCache.ContainsKey($name)) { return $script:layoutCache[$name] }
    $d = $types[$name]
    $recAlignMax = if ($d.Packed) { 1 } else { 8 }
    $off = 0
    $maxAlign = 1
    $list = New-Object System.Collections.Generic.List[object]
    foreach ($f in $d.Fields) {
        $rt = Resolve-Type $f.Type
        $a = [Math]::Min($rt.Align, $recAlignMax)
        if (-not $d.Packed) {
            if ($a -gt 0) { $off = [int]([Math]::Ceiling($off / $a) * $a) }
        }
        $list.Add([pscustomobject]@{ Name = $f.Name; Type = $f.Type.Trim(); Offset = $off; Size = $rt.Size; Info = $rt })
        $off += $rt.Size
        if ($rt.Align -gt $maxAlign) { $maxAlign = $rt.Align }
    }
    $recAlign = [Math]::Min($maxAlign, $recAlignMax)
    if (-not $d.Packed) { $off = [int]([Math]::Ceiling($off / $recAlign) * $recAlign) }
    $res = [pscustomobject]@{ Name = $name; Size = $off; Align = $recAlign; Fields = $list; Packed = $d.Packed }
    $script:layoutCache[$name] = $res
    return $res
}

function Resolve-Type([string]$typeName) {
    $t = $typeName.Trim()
    $m = [regex]::Match($t, '^string\[(.+)\]$')
    if ($m.Success) {
        $n = $m.Groups[1].Value
        $len = if ($n.StartsWith('$')) { [Convert]::ToInt32($n.Substring(1), 16) } else { [int]$n }
        return [pscustomobject]@{ Kind = 'shortstr'; Size = $len + 1; Align = 1; Max = $len; Cs = 'string' }
    }
    $m = [regex]::Match($t, '^array\[(\d+)\.\.(\d+)\]\s+of\s+(.+)$')
    if ($m.Success) {
        $elem = Resolve-Type $m.Groups[3].Value
        $count = [int]$m.Groups[2].Value - [int]$m.Groups[1].Value + 1
        return [pscustomobject]@{ Kind = 'fixedarray'; Size = $count * $elem.Size; Align = $elem.Align; Count = $count; Cs = $elem.Cs; ElemName = $m.Groups[3].Value.Trim(); ElemCs = $elem.Cs; ElemKind = $elem.Kind }
    }
    if (-not $types.ContainsKey($t)) { throw "unknown type '$t'" }
    $d = $types[$t]
    if ($d.Kind -eq 'record') {
        $sz = Get-RecordLayout $t
        return [pscustomobject]@{ Kind = 'nested'; Size = $sz.Size; Align = $sz.Align; Cs = $t; Rec = $sz }
    }
    switch ($d.Kind) {
        'enum'   { return [pscustomobject]@{ Kind = 'enum'; Size = 1; Align = 1; Cs = (Map-CsName $d.Cs); Values = $d.Values } }
        'scalar' { return [pscustomobject]@{ Kind = 'scalar'; Size = $d.Size; Align = $d.Align; Cs = $d.Cs } }
        'set'    { return [pscustomobject]@{ Kind = 'set'; Size = 1; Align = 1; Cs = 'byte' } }
        'rect'   { return [pscustomobject]@{ Kind = 'rect'; Size = 16; Align = 4; Cs = 'TDxRect' } }
        'point'  { return [pscustomobject]@{ Kind = 'point'; Size = 8; Align = 4; Cs = 'TDxPoint' } }
        'array'  {
            $elem = Resolve-Type $d.Elem
            $count = $d.Hi - $d.Lo + 1
            return [pscustomobject]@{ Kind = 'fixedarray'; Size = $count * $elem.Size; Align = $elem.Align; Count = $count; Cs = $elem.Cs; ElemName = $d.Elem.Trim(); ElemCs = $elem.Cs; ElemKind = $elem.Kind }
        }
        default  { throw "unsupported kind $($d.Kind) for $t" }
    }
}

# ---- 5. emit ------------------------------------------------------------------------
function Elem-ReadExpr([string]$kind, [string]$cs, [string]$expr) {
    switch ($kind) {
        'nested' { return "$cs.ReadAt(b, $expr)" }
        'enum'   { return "($cs)b[$expr]" }
        'set'    { return "b[$expr]" }
        'scalar' {
            switch ($cs) {
                'int'    { return "GuiCodec.ReadInt32(b, $expr)" }
                'ushort' { return "GuiCodec.ReadUInt16(b, $expr)" }
                'byte'   { return "GuiCodec.ReadByte(b, $expr)" }
                'bool'   { return "GuiCodec.ReadBool(b, $expr)" }
                'double' { return "GuiCodec.ReadDouble(b, $expr)" }
                default  { return "GuiCodec.ReadInt32(b, $expr)" }
            }
        }
        default  { return "$cs.ReadAt(b, $expr)" }
    }
}

function Elem-WriteStmt([string]$kind, [string]$cs, [string]$lhs, [string]$expr) {
    switch ($kind) {
        'nested' { return "$lhs.WriteAt(b, $expr);" }
        'enum'   { return "b[$expr] = (byte)$lhs;" }
        'set'    { return "b[$expr] = $lhs;" }
        'scalar' {
            switch ($cs) {
                'int'    { return "GuiCodec.WriteInt32(b, $expr, $lhs);" }
                'ushort' { return "GuiCodec.WriteUInt16(b, $expr, $lhs);" }
                'byte'   { return "b[$expr] = $lhs;" }
                'bool'   { return "GuiCodec.WriteBool(b, $expr, $lhs);" }
                'double' { return "GuiCodec.WriteDouble(b, $expr, $lhs);" }
                default  { return "GuiCodec.WriteInt32(b, $expr, $lhs);" }
            }
        }
        default  { return "$lhs.WriteAt(b, $expr);" }
    }
}

$sb = New-Object System.Text.StringBuilder
function W([string]$s) { [void]$sb.AppendLine($s) }

W '// <auto-generated>'
W '//   Generated by Tools/GenGuiRecords.ps1 from'
W '//   Source/Client-HGE/DxComponent/DxComponents.pas (interface section, GBK).'
W '//   DO NOT EDIT BY HAND - re-run the script instead.'
W '//'
W '//   Reproduces the Delphi 7 default record layout ({$A8} alignment, {$MINENUMSIZE 1})'
W '//   so that "ReadMemory(x, SizeOf(TXxx))" in LoadDxControl.pas / LoadDxControlEx.pas'
W '//   consumes exactly the same number of bytes as the original client.'
W '//   Field offsets below are emitted by the generator, never typed by hand.'
W '// </auto-generated>'
W ''
W 'using System;'
W 'using System.Linq;'
W 'using GXX.Client.DxComponent;'
W ''
W 'using TAlignment = GXX.Client.DxComponent.TDxAlignment;'
W ''
W 'namespace GXX.Client.LoadDx;'
W ''
W '/// <summary>One field of a Delphi record under the default {$A8} alignment.</summary>'
W 'public readonly struct DelphiField'
W '{'
W '    public readonly string Name;'
W '    public readonly int Offset;'
W '    public readonly int Size;'
W ''
W '    public DelphiField(string name, int offset, int size) { Name = name; Offset = offset; Size = size; }'
W ''
W '    public override string ToString() => $"{Name}@{Offset}+{Size}";'
W '}'
W ''
W '/// <summary>Delphi default-alignment record layout: SizeOf / alignment / field offset table.</summary>'
W 'public sealed class DelphiRecordLayout'
W '{'
W '    public readonly string Name;'
W '    public readonly int Size;'
W '    public readonly int Align;'
W '    public readonly DelphiField[] Fields;'
W ''
W '    public DelphiRecordLayout(string name, int size, int align, DelphiField[] fields)'
W '    {'
W '        Name = name; Size = size; Align = align; Fields = fields;'
W '    }'
W ''
W '    /// <summary>Offset lookup by field name (not present in the original; used by tests/docs).</summary>'
W '    public int OffsetOf(string fieldName)'
W '    {'
W '        foreach (var f in Fields) if (f.Name == fieldName) return f.Offset;'
W '        throw new ArgumentException($"no field {fieldName} in {Name}");'
W '    }'
W '}'
W ''
W '/// <summary>Field level codec for the .GUI stream (little endian, Delphi x86 memory image).</summary>'
W 'public static class GuiCodec'
W '{'
W '    public static int ReadInt32(byte[] b, int o) => b[o] | (b[o + 1] << 8) | (b[o + 2] << 16) | (b[o + 3] << 24);'
W '    public static void WriteInt32(byte[] b, int o, int v)'
W '    {'
W '        b[o] = (byte)v; b[o + 1] = (byte)(v >> 8); b[o + 2] = (byte)(v >> 16); b[o + 3] = (byte)(v >> 24);'
W '    }'
W '    public static ushort ReadUInt16(byte[] b, int o) => (ushort)(b[o] | (b[o + 1] << 8));'
W '    public static void WriteUInt16(byte[] b, int o, ushort v) { b[o] = (byte)v; b[o + 1] = (byte)(v >> 8); }'
W '    public static byte ReadByte(byte[] b, int o) => b[o];'
W '    public static bool ReadBool(byte[] b, int o) => b[o] != 0;'
W '    public static void WriteBool(byte[] b, int o, bool v) => b[o] = v ? (byte)1 : (byte)0;'
W '    public static double ReadDouble(byte[] b, int o) => BitConverter.ToDouble(b, o);'
W '    public static void WriteDouble(byte[] b, int o, double v) => BitConverter.GetBytes(v).CopyTo(b, o);'
W ''
W '    /// <summary>Delphi string[N] (ShortString): 1 length byte + raw GBK bytes.</summary>'
W '    public static string ReadShortString(byte[] b, int o, int maxLen)'
W '    {'
W '        int len = b[o];'
W '        if (len > maxLen) len = maxLen;'
W '        if (len <= 0) return string.Empty;'
W '        return GXX.Core.EncodingInit.GBK.GetString(b, o + 1, len);'
W '    }'
W ''
W '    public static void WriteShortString(byte[] b, int o, int maxLen, string value)'
W '    {'
W '        var raw = GXX.Core.EncodingInit.GBK.GetBytes(value ?? string.Empty);'
W '        int len = Math.Min(raw.Length, maxLen);'
W '        b[o] = (byte)len;'
W '        Array.Copy(raw, 0, b, o + 1, len);'
W '    }'
W '}'
W ''

W '// -------------------------------------------------------------------------------------'
W '// VCL enums referenced by the records but not declared in DxComponents.pas.'
W '// -------------------------------------------------------------------------------------'
W ''
$emitted = @{}
$enumOrder = New-Object System.Collections.Generic.List[string]
foreach ($n in $order) { [void]$enumOrder.Add($n) }
foreach ($n in $newEnums) { [void]$enumOrder.Add($n) }
foreach ($n in $enumOrder) {
    if (-not $types.ContainsKey($n)) { continue }
    $t = $types[$n]
    if ($t.Kind -ne 'enum') { continue }
    if ($existingEnums -contains $n) { continue }
    if ($t.Values.Count -eq 0) { continue }
    if ($emitted.ContainsKey($n)) { continue }
    $emitted[$n] = $true
    $srcLine = Get-SourceLine $n
    if ($srcLine -gt 0) { W "// src: DxComponents.pas:$srcLine" } else { W "// src: VCL (referenced by DxComponents.pas)" }
    W "public enum $n : byte"
    W '{'
    for ($i = 0; $i -lt $t.Values.Count; $i++) {
        $sep = if ($i -lt $t.Values.Count - 1) { ',' } else { '' }
        W "    $($t.Values[$i]) = $i$sep"
    }
    W '}'
    W ''
}

foreach ($n in $order) {
    if (-not $types.ContainsKey($n)) { continue }
    if ($types[$n].Kind -ne 'record') { continue }
    $lay = Get-RecordLayout $n
    $packedNote = if ($lay.Packed) { 'packed record' } else { 'record' }
    W '// -------------------------------------------------------------------------------------'
    W "// $n = $packedNote   src: DxComponents.pas:$((Get-SourceLine $n))   SizeOf=$($lay.Size)  Align=$($lay.Align)"
    W '// -------------------------------------------------------------------------------------'
    W "public sealed class $n"
    W '{'
    W "    public const int SizeOf = $($lay.Size);"
    W "    public const int AlignOf = $($lay.Align);"
    W ''
    W "    /// <summary>Field offsets under the Delphi default alignment ($($lay.Fields.Count) fields).</summary>"
    W "    public static readonly DelphiRecordLayout Layout = new DelphiRecordLayout(""$n"", $($lay.Size), $($lay.Align), new DelphiField[]"
    W '    {'
    foreach ($f in $lay.Fields) {
        W "        new DelphiField(""$($f.Name)"", $($f.Offset), $($f.Size)),"
    }
    W '    });'
    W ''
    foreach ($f in $lay.Fields) {
        $info = $f.Info
        switch ($info.Kind) {
            'shortstr'   { W "    public string $($f.Name);" }
            'fixedarray' {
                if ($info.ElemName -eq 'TRect') { W "    public TDxRect[] $($f.Name) = new TDxRect[$($info.Count)];" }
                elseif ($info.ElemName -eq 'TPoint') { W "    public TDxPoint[] $($f.Name) = new TDxPoint[$($info.Count)];" }
                elseif ($info.ElemKind -eq 'nested') { W "    public $($info.ElemCs)[] $($f.Name) = Enumerable.Range(0, $($info.Count)).Select(_ => new $($info.ElemCs)()).ToArray();" }
                else { W "    public $($info.ElemCs)[] $($f.Name) = new $($info.ElemCs)[$($info.Count)];" }
            }
            'nested'     { W "    public $($info.Cs) $($f.Name) = new $($info.Cs)();" }
            'enum'       { W "    public $($info.Cs) $($f.Name);" }
            'set'        { W "    public byte $($f.Name);" }
            'rect'       { W "    public TDxRect $($f.Name);" }
            'point'      { W "    public TDxPoint $($f.Name);" }
            'scalar'     { W "    public $($info.Cs) $($f.Name);" }
            default      { W "    public $($info.Cs) $($f.Name);" }
        }
    }
    W ''
    W "    /// <summary>Decode from b[at .. at+SizeOf-1] (the original reads the whole record at once).</summary>"
    W "    public static $n ReadAt(byte[] b, int at)"
    W '    {'
    W "        var v = new $n();"
    foreach ($f in $lay.Fields) {
        $info = $f.Info
        $o = "at + $($f.Offset)"
        if ($info.Kind -eq 'fixedarray') {
            $elem = $info.ElemName.Trim()
            $esz = [int]($info.Size / $info.Count)
            if ($elem -eq 'TRect') {
                W "        for (int i = 0; i < $($info.Count); i++) v.$($f.Name)[i] = TDxRect.Rect(GuiCodec.ReadInt32(b, $o + i * $esz + 0), GuiCodec.ReadInt32(b, $o + i * $esz + 4), GuiCodec.ReadInt32(b, $o + i * $esz + 8), GuiCodec.ReadInt32(b, $o + i * $esz + 12));"
            } elseif ($elem -eq 'TPoint') {
                W "        for (int i = 0; i < $($info.Count); i++) v.$($f.Name)[i] = new TDxPoint(GuiCodec.ReadInt32(b, $o + i * $esz + 0), GuiCodec.ReadInt32(b, $o + i * $esz + 4));"
            } else {
                W "        for (int i = 0; i < $($info.Count); i++) v.$($f.Name)[i] = $(Elem-ReadExpr $info.ElemKind $info.ElemCs "$o + i * $esz");"
            }
            continue
        }
        switch ($info.Kind) {
            'shortstr' { W "        v.$($f.Name) = GuiCodec.ReadShortString(b, $o, $($info.Max));" }
            'nested'   { W "        v.$($f.Name) = $($info.Cs).ReadAt(b, $o);" }
            'enum'     { W "        v.$($f.Name) = ($($info.Cs))b[$o];" }
            'set'      { W "        v.$($f.Name) = b[$o];" }
            'rect'     { W "        v.$($f.Name) = TDxRect.Rect(GuiCodec.ReadInt32(b, $o + 0), GuiCodec.ReadInt32(b, $o + 4), GuiCodec.ReadInt32(b, $o + 8), GuiCodec.ReadInt32(b, $o + 12));" }
            'point'    { W "        v.$($f.Name) = new TDxPoint(GuiCodec.ReadInt32(b, $o + 0), GuiCodec.ReadInt32(b, $o + 4));" }
            'scalar'   {
                switch ($info.Cs) {
                    'int'      { W "        v.$($f.Name) = GuiCodec.ReadInt32(b, $o);" }
                    'ushort'   { W "        v.$($f.Name) = GuiCodec.ReadUInt16(b, $o);" }
                    'byte'     { W "        v.$($f.Name) = GuiCodec.ReadByte(b, $o);" }
                    'bool'     { W "        v.$($f.Name) = GuiCodec.ReadBool(b, $o);" }
                    'double'   { W "        v.$($f.Name) = GuiCodec.ReadDouble(b, $o);" }
                    default    { W "        v.$($f.Name) = GuiCodec.ReadInt32(b, $o);" }
                }
            }
            default    { W "        v.$($f.Name) = GuiCodec.ReadInt32(b, $o);" }
        }
    }
    W '        return v;'
    W '    }'
    W ''
    W "    /// <summary>Encode into b[at .. at+SizeOf-1] (mirror of the GUI editor SaveComponent).</summary>"
    W "    public void WriteAt(byte[] b, int at)"
    W '    {'
    foreach ($f in $lay.Fields) {
        $info = $f.Info
        $o = "at + $($f.Offset)"
        if ($info.Kind -eq 'fixedarray') {
            $elem = $info.ElemName.Trim()
            $esz = [int]($info.Size / $info.Count)
            if ($elem -eq 'TRect') {
                W "        for (int i = 0; i < $($info.Count); i++) { GuiCodec.WriteInt32(b, $o + i * $esz + 0, $($f.Name)[i].Left); GuiCodec.WriteInt32(b, $o + i * $esz + 4, $($f.Name)[i].Top); GuiCodec.WriteInt32(b, $o + i * $esz + 8, $($f.Name)[i].Right); GuiCodec.WriteInt32(b, $o + i * $esz + 12, $($f.Name)[i].Bottom); }"
            } elseif ($elem -eq 'TPoint') {
                W "        for (int i = 0; i < $($info.Count); i++) { GuiCodec.WriteInt32(b, $o + i * $esz + 0, $($f.Name)[i].X); GuiCodec.WriteInt32(b, $o + i * $esz + 4, $($f.Name)[i].Y); }"
            } else {
                W "        for (int i = 0; i < $($info.Count); i++) $(Elem-WriteStmt $info.ElemKind $info.ElemCs "$($f.Name)[i]" "$o + i * $esz")"
            }
            continue
        }
        switch ($info.Kind) {
            'shortstr' { W "        GuiCodec.WriteShortString(b, $o, $($info.Max), $($f.Name));" }
            'nested'   { W "        ($($f.Name) ?? new $($info.Cs)()).WriteAt(b, $o);" }
            'enum'     { W "        b[$o] = (byte)$($f.Name);" }
            'set'      { W "        b[$o] = $($f.Name);" }
            'rect'     { W "        GuiCodec.WriteInt32(b, $o + 0, $($f.Name).Left); GuiCodec.WriteInt32(b, $o + 4, $($f.Name).Top); GuiCodec.WriteInt32(b, $o + 8, $($f.Name).Right); GuiCodec.WriteInt32(b, $o + 12, $($f.Name).Bottom);" }
            'point'    { W "        GuiCodec.WriteInt32(b, $o + 0, $($f.Name).X); GuiCodec.WriteInt32(b, $o + 4, $($f.Name).Y);" }
            'scalar'   {
                switch ($info.Cs) {
                    'int'    { W "        GuiCodec.WriteInt32(b, $o, $($f.Name));" }
                    'ushort' { W "        GuiCodec.WriteUInt16(b, $o, $($f.Name));" }
                    'byte'   { W "        b[$o] = $($f.Name);" }
                    'bool'   { W "        GuiCodec.WriteBool(b, $o, $($f.Name));" }
                    'double' { W "        GuiCodec.WriteDouble(b, $o, $($f.Name));" }
                    default  { W "        GuiCodec.WriteInt32(b, $o, $($f.Name));" }
                }
            }
            default    { W "        GuiCodec.WriteInt32(b, $o, $($f.Name));" }
        }
    }
    W '    }'
    W ''
    W "    public byte[] ToBytes() { var b = new byte[SizeOf]; WriteAt(b, 0); return b; }"
    W ''
    W "    public static $n FromBytes(byte[] b, int offset = 0) => ReadAt(b, offset);"
    W '}'
    W ''
}

[System.IO.File]::WriteAllText($OutFile, $sb.ToString(), (New-Object System.Text.UTF8Encoding($false)))
Write-Host "wrote $OutFile"

# ---- 6. read-back audit table (stdout) ----------------------------------------------
Write-Host ''
Write-Host 'AUDIT;Type;SizeOf;Align;Fields'
foreach ($n in $order) {
    if (-not $types.ContainsKey($n)) { continue }
    if ($types[$n].Kind -ne 'record') { continue }
    $lay = Get-RecordLayout $n
    $fs = ($lay.Fields | ForEach-Object { "$($_.Name)@$($_.Offset)+$($_.Size)" }) -join ' '
    Write-Host "AUDIT;$n;$($lay.Size);$($lay.Align);$fs"
}
