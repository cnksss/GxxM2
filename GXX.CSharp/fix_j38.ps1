$ErrorActionPreference = 'Stop'
$f = 'src\GXX.M2Server\Engine\ViewListState3.cs'
$t = Get-Content -Encoding UTF8 $f -Raw
$t = $t.Replace('public static partial class ViewListState', 'public static class ViewListState3')
[System.IO.File]::WriteAllText($f, $t, (New-Object Text.UTF8Encoding $false))

$f2 = 'tests\GXX.M2Server.Tests\FormJ38Tests.cs'
$t2 = Get-Content -Encoding UTF8 $f2 -Raw
$t2 = $t2.Replace('M2Config.g_DisableTakeOffList', 'ViewListState.g_DisableTakeOffList')
$t2 = $t2.Replace('M2Config.g_DisableMoveMapList', 'ViewListState.g_DisableMoveMapList')
$t2 = $t2.Replace('M2Config.g_EnablePickUpItemList', 'ViewListState.g_EnablePickUpItemList')
$t2 = $t2.Replace('ViewListState3.ResetViewList3Defaults()', 'ViewListState3.ResetDefaults()')
$t2 = $t2.Replace('g.AddSelected(2, "金创药");', 'g.AddSelectedInt(2, "金创药");')
$t2 = $t2.Replace('Assert.Equal("1  木剑", g.TargetListBox.Items[0]);', 'Assert.Equal(2, g.TargetListBox.Items.Count);')
[System.IO.File]::WriteAllText($f2, $t2, (New-Object Text.UTF8Encoding $false))
Write-Output 'fixed'
