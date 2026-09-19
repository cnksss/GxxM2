$ErrorActionPreference = 'Stop'
$f = 'docs\Checklist.md'
$t = Get-Content -Encoding UTF8 $f -Raw
$old = '## 6. 剩余深化清单（第四十三轮批次J36 已启动 MonsterConfig.pas 5256 行巨片拆分（第一片：Open 骨架三接缝 + RefGeneralInfo 常规信息组 16 键 + 怪物时序组 8 键）；下一轮＝MonsterConfig 余部（人形怪装备双列表拖拽/怪物技能双击/自定义怪编辑/掉落限制树）与 ViewList 余片、NPC 脚本 Stub 逐条深化，余项按下表继续迭代）'
$new = '## 6. 剩余深化清单（第四十四轮批次J37/J38 已完成 MonsterConfig.pas 5256 行巨片全部（第二片人形怪配置/装备槽/魔法组）与 ViewList.pas 余片收尾（剩余 12 组列表框映射数据驱动接入）；下一轮＝NPC 脚本 Stub 逐条深化与 Client 场景层，余项按下表继续迭代）'
if ($t.Contains($old)) {
    $t = $t.Replace($old, $new)
    [IO.File]::WriteAllText($f, $t, (New-Object Text.UTF8Encoding $false))
    Write-Output 'header updated'
} else {
    Write-Output 'header not found'
}
