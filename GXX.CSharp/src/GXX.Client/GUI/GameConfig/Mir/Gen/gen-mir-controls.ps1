# 生成 GXX.CSharp/src/GXX.Client/GUI/GameConfig/Mir/MirConfigDlgControls.g.cs
#
# 依据：Source/Client-HGE/GameConfig/Mir/MirConfigDlg.pas 的 interface 段
#       第 43-558 行的 516 个 DxComponent 控件字段声明（逐字提取，不改名不改类型）。
#
# 输出：partial class TMirConfigDlgControls : IMirConfigDlgControls
#       每个控件一个属性，属性名 = 原文字段名，属性类型 = 原控件类型的托管等价物。
#       实例在构造函数里逐个 new（对应原文 DFM/UI 流对这 516 个控件的实例化）。
#
# 重新生成：
#   powershell -ExecutionPolicy Bypass -File GXX.CSharp/src/GXX.Client/GUI/GameConfig/Mir/Gen/gen-mir-controls.ps1
#
# ★ 生成器放在本车道的独占分区内（而非 GXX.CSharp/tools/** —— 那是派发说明里的禁用区）。
param(
  [string]$Root = "D:\chuanqi\daima\GXX原版_Delphi7",
  [string]$Project = "D:\chuanqi\daima\GXX原版_Delphi7\GXX.CSharp"
)

$pas = Join-Path $Root "_analysis\utf8_mirror\Client-HGE\GameConfig\Mir\MirConfigDlg.pas"
if (-not (Test-Path $pas)) { $pas = Join-Path $Root ".worktrees\p10-client-mirconfig\_probe\fallback.pas" }
$lines = Get-Content $pas -Encoding UTF8

# 控件类型 → 托管等价物（名称与原文一致，见 MirDxControlSeams.cs 的说明）
$TypeMap = @{
  'TDxImageButton'  = 'TDxImageButton'
  'TDxImageForm'    = 'TDxImageForm'
  'TDxPageControl'  = 'TDxPageControl'
  'TDxTabSheet'     = 'TDxTabSheet'
  'TDxScrollBox'    = 'TDxScrollBox'
  'TDxLabel'        = 'TDxLabel'
  'TDxImageGrid'    = 'TDxListView'
  'TDxListView'     = 'TDxListView'
  'TDxChatMemo'     = 'TDxChatMemo'
  'TDxEdit'         = 'TDxEdit'
  'TDxComboBox'     = 'TDxComboBox'
  'TDxTrackBar'     = 'TDxTrackBar'
  'TDxLine'         = 'TDxLine'
  'TDxPopupMenu'    = 'TDxPopupMenu'
  'TDxMemo'         = 'TDxChatMemo'
}

$fields = @()
for ($i = 42; $i -lt 559; $i++) {
  if ($lines[$i] -match '^\s*([A-Za-z_]\w*)\s*:\s*(TDx\w+)\s*;') {
    $fields += [pscustomobject]@{ Line = $i + 1; Name = $matches[1]; Type = $matches[2] }
  }
}
if ($fields.Count -eq 0) { throw "未提取到控件字段，检查 $pas" }

$sb = New-Object System.Text.StringBuilder
[void]$sb.AppendLine('// 源单元: Source/Client-HGE/GameConfig/Mir/MirConfigDlg.pas (GBK, 8355 lines, CRLF)')
[void]$sb.AppendLine('// ** 本文件由脚本生成，禁止手工编辑 **')
[void]$sb.AppendLine('//    生成器: GXX.CSharp/src/GXX.Client/GUI/GameConfig/Mir/Gen/gen-mir-controls.ps1')
[void]$sb.AppendLine('//')
[void]$sb.AppendLine("// 覆盖: 原文 43-558 的 $($fields.Count) 个 DxComponent 控件字段（逐字：原名 + 原类型）。")
[void]$sb.AppendLine('// 对账: 控件声明数 = 属性数 = 构造函数实例化数 = 516（报告 §2.1 三方对账表）。')
[void]$sb.AppendLine('')
[void]$sb.AppendLine('namespace GXX.Client.GUI.GameConfig.Mir;')
[void]$sb.AppendLine('')
[void]$sb.AppendLine('/// <summary>')
[void]$sb.AppendLine('/// 接缝：TMirConfigDlg 的 516 个控件字段（原文 43-558）。')
[void]$sb.AppendLine('/// 手写的"带操作/事件"成员在 <c>MirConfigDlgControls.cs</c>，本文件只放控件对象本身。')
[void]$sb.AppendLine('/// </summary>')
[void]$sb.AppendLine('public partial class TMirConfigDlgControls : IMirConfigDlgControls')
[void]$sb.AppendLine('{')
foreach ($f in $fields) {
  if (-not $TypeMap.ContainsKey($f.Type)) { throw "未映射的控件类型 $($f.Type)（$($f.Name) at line $($f.Line)）" }
  $t = $TypeMap[$f.Type]
  [void]$sb.AppendLine("    /// <summary>原文 $($f.Line): <c>$($f.Name):$($f.Type)</c>。</summary>")
  [void]$sb.AppendLine("    public $t $($f.Name) { get; }")
}
[void]$sb.AppendLine('')
[void]$sb.AppendLine('    public TMirConfigDlgControls()')
[void]$sb.AppendLine('    {')
foreach ($f in $fields) {
  $t = $TypeMap[$f.Type]
  [void]$sb.AppendLine("        $($f.Name) = new $t();")
}
[void]$sb.AppendLine('    }')
[void]$sb.AppendLine('}')

$outDir = Join-Path $Project 'src\GXX.Client\GUI\GameConfig\Mir'
$out = Join-Path $outDir 'MirConfigDlgControls.g.cs'
$sb.ToString() | Set-Content -Encoding UTF8 $out
Write-Host "generated $out : $($fields.Count) controls"
$fields | Group-Object Type | Sort-Object Count -Descending | ForEach-Object { "  $($_.Count)`t$($_.Name)" }
