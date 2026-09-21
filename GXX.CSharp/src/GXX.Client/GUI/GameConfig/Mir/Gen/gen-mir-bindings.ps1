# 生成 GXX.CSharp/src/GXX.Client/GUI/GameConfig/Mir/MirConfigDlgEventBindings.g.cs
#
# 依据：Source/Client-HGE/GameConfig/Mir/MirConfigDlg.pas 的 Initialize 主体
#       第 4621-5044 行里的全部 `X.OnY := Handler;` 语句（逐行提取，含重复绑定与行号）。
#       以及第 43-558 行的控件字段名清单（用于校验每条绑定的控件名都真实声明过）。
#
# 输出：static class TMirConfigDlgEventBindings_Data { public static readonly TMirConfigDlgEventBinding[] All; }
#
# 重新生成：
#   powershell -ExecutionPolicy Bypass -File GXX.CSharp/src/GXX.Client/GUI/GameConfig/Mir/Gen/gen-mir-bindings.ps1
param(
  [string]$Root = "D:\chuanqi\daima\GXX原版_Delphi7",
  [string]$Project = "D:\chuanqi\daima\GXX原版_Delphi7\GXX.CSharp",
  [int]$FirstLine = 4621,
  [int]$LastLine = 5044
)

$pas = Join-Path $Root "_analysis\utf8_mirror\Client-HGE\GameConfig\Mir\MirConfigDlg.pas"
$lines = Get-Content $pas -Encoding UTF8

# 控件字段名清单（43-558），用于校验绑定引用的控件确实声明过
$ctrlNames = New-Object 'System.Collections.Generic.HashSet[string]'
for ($i = 42; $i -lt 559; $i++) {
  if ($lines[$i] -match '^\s*([A-Za-z_]\w*)\s*:\s*TDx\w+\s*;') { [void]$ctrlNames.Add($matches[1]) }
}

$bindings = @()
for ($i = $FirstLine - 1; $i -lt $LastLine; $i++) {
  if ($lines[$i] -match '^\s*([A-Za-z_]\w*)\.(On\w+)\s*:=\s*([A-Za-z_]\w*)\s*;') {
    $ctrl = $matches[1]; $ev = $matches[2]; $h = $matches[3]
    $bindings += [pscustomobject]@{
      Line = $i + 1; Ctrl = $ctrl; Event = $ev; Handler = $h
      Declared = $ctrlNames.Contains($ctrl)
    }
  }
}

Write-Host "bindings=$($bindings.Count) (原文区间 $FirstLine..$LastLine)"
$undeclared = @($bindings | Where-Object { -not $_.Declared })
if ($undeclared.Count -gt 0) {
  Write-Host "!! 绑定引用了未声明的控件名："
  $undeclared | ForEach-Object { "   line $($_.Line): $($_.Ctrl).$($_.Event) := $($_.Handler)" }
}

$sb = New-Object System.Text.StringBuilder
[void]$sb.AppendLine('// 源单元: Source/Client-HGE/GameConfig/Mir/MirConfigDlg.pas (GBK, 8355 lines, CRLF)')
[void]$sb.AppendLine('// ** 本文件由脚本生成，禁止手工编辑 **')
[void]$sb.AppendLine('//    生成器: GXX.CSharp/src/GXX.Client/GUI/GameConfig/Mir/Gen/gen-mir-bindings.ps1')
[void]$sb.AppendLine('//')
[void]$sb.AppendLine("// 覆盖: 原文 $FirstLine-$LastLine 的 $($bindings.Count) 条 ``X.OnY := Handler;``（逐行，行号即原文行号）。")
[void]$sb.AppendLine('// 对账: 本表条数 = IMirConfigDlgControls.BindEvents 的挂接条数 = 原文绑定数。')
[void]$sb.AppendLine('')
[void]$sb.AppendLine('namespace GXX.Client.GUI.GameConfig.Mir;')
[void]$sb.AppendLine('')
[void]$sb.AppendLine('/// <summary>原文 Initialize 的 279 条事件绑定（数据表，逐行可回溯到原文）。</summary>')
[void]$sb.AppendLine('public static class TMirConfigDlgEventBindings_Data')
[void]$sb.AppendLine('{')
[void]$sb.AppendLine('    public static readonly TMirConfigDlgEventBinding[] All = new TMirConfigDlgEventBinding[]')
[void]$sb.AppendLine('    {')
foreach ($b in $bindings) {
  [void]$sb.AppendLine("        new TMirConfigDlgEventBinding { Line = $($b.Line), Ctrl = `"$($b.Ctrl)`", Event = `"$($b.Event)`", Handler = `"$($b.Handler)`" },   // 原文 $($b.Line)")
}
[void]$sb.AppendLine('    };')
[void]$sb.AppendLine('}')

$outDir = Join-Path $Project 'src\GXX.Client\GUI\GameConfig\Mir'
$out = Join-Path $outDir 'MirConfigDlgEventBindings.g.cs'
$sb.ToString() | Set-Content -Encoding UTF8 $out
Write-Host "generated $out"
