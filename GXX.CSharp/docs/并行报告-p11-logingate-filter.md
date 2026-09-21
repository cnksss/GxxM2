# 并行报告 p11-logingate-filter：LoginGate 执法面缺口移植

> 车道：`p11-logingate-filter`（写车道）
> 工作树：`D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p11-logingate-filter`（分支 `par/p11-logingate-filter`）
> 承接：`docs/并行报告-p11-logingate-review.md`（只读复核车道）§2/§3/§6/§7 与 §5.4 的车道建议

## 0. 进行中（本文件先落盘，防宿主杀车道丢产出）

目标单元（**只对 LoginGate 份负责**，SelGate 份不在本车道范围）：
`LoginGate/Misc.pas`(321) / `LoginGate/FuncForComm.pas`(590) / `LoginGate/IPAddrFilter.pas`(400 残部) /
`LoginGate/ConfigManager.pas`(269 残部) / `LoginGate/ClientSession.pas`(820 残部)

硬约束（不可违反）：新设施**可选启用（opt-in）**、**默认不改变现有行为**、**绝不统一掉 SelGate 语义**；
与既有设施语义不同则**并存并写清差异**。

（下文将在移植过程中逐节补齐。）
