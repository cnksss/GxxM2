# `ObjMon.pas` 声明 ↔ 实现 对账表（批次J239）

> ## ⛔ 2026-09-21 更正（台账 §49，**引用本文件结论前必读**）
> 只读复核车道 `p12-e2only-review` 用方法级抽样证明：**`ObjMon.pas` 的 204 条实现里 0 条真正移植**（裁决 C）。
> 承载它的 **46 个 `ObjMon*Core.cs` 合计约 6,300 个方法，其中约 1,917 条是裸 `=> true;` 恒真断言**
> （调度方独立复核确认：`tools/audit-stubs.ps1` 实测该 46 文件 **1,917 条**）。
> 因此本文件下文"尚未移植 14 类 / 62 方法"等结论**严重低估了缺口**；
> 而 `docs/ObjMon-覆盖率表.md` 的"37/55 已移植"**高估了完成度**（该文件 §6 自己已声明判据失败）。
> **正确口径**：`ObjMon` 已登记为 **REFUTED（真缺口）**，并已开车道 `par/p13-m2-objmon-real` 就地修复那 1,917 条桩。
> 本文件保留作**历史对账记录**，不再作为完成度依据。

> **本文件的用途**：在 J238 到达 `ObjMon.pas` 末尾（第 9501 行 `end.`）之后，
> 把"按实现顺序线性推进"改为**按类对账**。
> 全表由脚本从 `_analysis/utf8_mirror/M2Engine/ObjMon.pas` 直接抽取，
> **不依赖任何人工记忆**，故可随时重跑复核。

---

## 1. 脚本与口径

**声明抽取**：`^\s*(T\w+)\s*=\s*class\s*(\(\s*(\w+)\s*\))?`
**实现抽取**：`^\s*(function|procedure|constructor|destructor)\s+(T\w+)\.(\w+)`

**统计结果**

| 项 | 值 |
|---|---|
| 全单元行数 | **9502**（第 9501 行 `end.`） |
| 类声明条数（脚本直接匹配） | **56** |
| **扣除 `(* *)` 注释里的那一份后的实数** | **55** |
| **没有实现段的类** | **0** |
| 方法实现条目数（含重复计数） | **213** |
| **去重后的方法数** | **206** |

**关键结论：这个单元里**没有任何一个类是"只声明不实现"的** ——
即"剩余工作"是**真实存在、可枚举**的，不是"声明了但没写"。

---

## 2. 重名排查：**一处真、两处假**（本表的第一轮自查）

### 2.1 真的那一处：被 `(* *)` 注掉的重复类声明

`TElfWarriorMonster` 出现了两次，**而第一次是在 Pascal 的 `(* … *)` 块注释里**：

```
471:  // 修正神兽攻击
472:  (*
473:    TElfWarriorMonster = class(TATMonster)
      …
487:    end;
488:  *)
489:  TElfWarriorMonster = class(TSpitSpider)
```

—— 即 **L473 那一份是被注释掉的旧版**（基类 `TATMonster`）、
**L489 是现行的新版**（基类 `TSpitSpider`）——
**这是形态⑩"同一名字定义两次、第一次在块注释里"第一次出现在**类声明**上****
（此前 J220 记的是方法）。

**附带两个"第一次"**：
1. **`(* … *)` 这种 Pascal 注释形式在本系列是第一次见** ——
   此前所有 66 处块注释都是 `{ … }`；
2. 注释上方写着 `// 修正神兽攻击` —— 即**这次改变基类是有意的、并留了说明**。

### 2.2 两处**假**的（我的脚本误报，已排除）

| 名字 | 两处位置 | 真相 |
|---|---|---|
| `TMonster.Run` | **L934**（**有缩进**）与 **L1121**（列 0） | **前者是类体内的声明、后者是实现** —— 不是重复 |
| `TDevilkingMonster.GotoTargetXY` | **L601**（**有缩进**）与 **L639**（列 0） | 同上 |

**教训**：只按"`procedure T<类>.<方法>`"抽取会把**声明**与**实现**都抓进来 ——
**必须用缩进或"是否在 `implementation` 之后"来区分这两者**。
本表已据此把这两条从"重名"里剔除。

### 2.3 计数修正

- 声明条数 **56** → **实为 55**（`TElfWarriorMonster` 那一份在 `(* *)` 里、不应计入）；
- 实现条目 **213** → **去重后 206**；
  多出的 7 条正是 `TElfWarriorMonster` 被 `$decls` **重复计数**所致
  （同一名字在 `$decls` 里出现两次、故 `$impls` 被累加两次）。

---

## 3. 尚未移植的类（脚本判定 14 个 / 62 条方法实现）

**按声明行排列**（`sub_*` 是反编译名）：

| 声明行 | 类 | 基类 | 方法实现 |
|---|---|---|---|
| 327 | `TGasAttackMonster` | `TATMonster` | `Create,Destroy,sub_4A9C78,AttackTarget` |
| 335 | `TCowMonster` | `TATMonster` | `Create,Destroy` |
| 341 | `TMagCowMonster` | `TATMonster` | `Create,Destroy,sub_4A9F6C,AttackTarget` |
| 350 | `TCowKingMonster` | `TATMonster` | `Create,Attack,Initialize,Run` |
| 368 | `TElectronicScolpionMon` | `TMonster` | `Create,Destroy,LightingAttack,RefreshAppr,Run` |
| 380 | `TLightingZombi` | `TMonster` | `Create,Destroy,LightingAttack,Run` |
| 389 | `TDigOutZombi` | `TMonster` | `Create,Destroy,sub_4AA8DC,Run` |
| 398 | `TZilKinZombi` | `TATMonster` | `Create,Destroy,Die,Run` |
| 409 | `TWhiteSkeleton` | `TATMonster` | `Create,Destroy,RecalcAbilitys,Run,sub_4AAD54` |
| 444 | `TGasMothMonster` | `TGasAttackMonster` | `Create,Destroy,sub_4A9C78,Run` |
| 452 | `TGasDungMonster` | `TGasAttackMonster` | `Create,Destroy` |
| 458 | `TElfMonster` | `TMonster` | `AppearNow,Create,Destroy,RecalcAbilitys,ResetElfMon,Run` |
| **489** | **`TElfWarriorMonster`** | **`TSpitSpider`** | `AppearNow,Create,Destroy,RecalcAbilitys,ResetElfMon,AttackTarget,Run` |

> **注**：`TElfWarriorMonster` 只列**一次**（L489）。
> L473 那一份在 `(* *)` 注释里、**不是**有效声明（见 §2.1）。

**可成批的分组**（便于下一批选题）：

- **毒气/蛛网族**：`TGasAttackMonster` + `TGasMothMonster` + `TGasDungMonster`（后者继承前者、
  且 `sub_4A9C78` 在三者间共用）
- **牛族**：`TCowMonster` + `TMagCowMonster` + `TCowKingMonster`
- **僵尸/骷髅族**：`TElectronicScolpionMon` + `TLightingZombi` + `TDigOutZombi` + `TZilKinZombi` + `TWhiteSkeleton`
- **精灵族**：`TElfMonster` + `TElfWarriorMonster`（`AppearNow` / `ResetElfMon` / `RecalcAbilitys` 同位）

---

## 4. ⚠️ **本表的口径限制（必须随表一起读）**

本表第 3 节的"尚未移植"判定，用的是
**"类名是否以词边界出现在我写的任何 `ObjMon*Core.cs` 里"** 这一启发式。

**它会把"只在注释里被提到"误判为"已移植"** —— 已实测到确证的两个假阳性：

| 类 | 被哪个文件提到 | 性质 |
|---|---|---|
| `TScultureMonster`（L420） | `ObjMonIcePeakCore.cs` | **仅在注释里作同类参照**、**并未移植** |
| `TScultureKingMonster`（L431） | `ObjMonIcePeakCore.cs`、`ObjMonMagicNotMoveCore.cs` | 同上、**并未移植** |

**故**：

- 脚本给出的 "已移植 42 / 未移植 14" **是一个上界估计**；
- **真实的已移植数**应 **≤ 42**、**未移植数 ≥ 14**；
- **权威口径**只能来自"每个 C# 文件 ↔ 它对应的 Delphi 类"的显式映射，
  而不是名字出现与否。

> **下一批应先建立那张显式映射表**（文件名 → Delphi 类 → 方法清单），
> 再据此产出真实的覆盖率；本表第 3 节在映射表建立前**只作选题索引**使用。

---

## 5. 与既有台账的关系

- `docs/Checklist.md` 的 §4 计数表按**测试项数**统计（M2Server 等），
  与**本表的"方法实现条数"不是同一口径**，两者不可直接相除。
- 本表只覆盖 `ObjMon.pas` **一个单元**；
  `AxeMon.pas`（9456 行）、`MirConfigDlg.pas`（7442 行）等尚未进入对账。

---

## 6. **三种自动覆盖率判据全部失败**（批次J240 的实测结论）

J239 用"名字出现在我写的 C# 文件里"这一条判据，得到 **42 已移植 / 14 未移植**，
并**自己就发现了两个假阳性**（`TScultureMonster`、`TScultureKingMonster` —— 只在注释里被提及）。
J240 又试了另外两种判据，**结果三种全都不可用**，且**每一种都因一个不同的、可命名的原因而失败**：

| # | 判据 | 实测结果 | **失败原因** |
|---|---|---|---|
| 1 | 类名出现在**任一** `ObjMon*Core.cs` 里 | 42 / 14 | 我的文件会**引用兄弟类**做跨类对照（如"同 J228 的火灵"） |
| 2 | 类名出现在 C# 文件的 **`/// <summary>` 头部** | **仍是 42 / 14** | **我的 summary 头部里同样会引用兄弟类** —— 收窄到头部**并不能**把"声明"与"引用"分开 |
| 3 | 类名出现在 `Checklist.md` 的**任一 J 行**里 | **56 / 0（全部"已覆盖"）** | J 行**既**在"已移植"里点名、**也**在"尚未移植"清单里点名 —— 判据**两向都命中**、故恒为真 |

**判据 3 的失败最值得记住**：它给出"**100% 完成**"这种**最危险**的错误答案 ——
因为台账的每一行都同时在陈述"做了什么"与"还差什么"。
`TScultureMonster` / `TScultureKingMonster` / `TExplosionAttackMonster` /
`TGasAttackMonster` / `TCowMonster` / `TElfMonster` 在这条判据下**全部**被判为"已覆盖"，
而它们**恰恰是未移植的**。

### 结论：**类级覆盖率无法由"名字出现"推出，必须由显式字段承载**

- 三种判据的失败**不是实现瑕疵、而是判据本身的类型错误** ——
  "名字出现"测的是**共现**，而覆盖率要的是**归属**；两者在"我的写作习惯里大量引用同类"的前提下**必然脱钩**。
- 因此正确的做法是**给每个类加一个结构化的状态字段**，例如在台账里新增一张
  `| 类名 | 声明行 | 基类 | 方法数 | 状态 | 对应 C# 文件 |` 的表，
  `状态 ∈ {已移植, 未移植, 部分, 实现在别处}`，**由人逐行填**、脚本只做**校验**（如"状态=已移植 则 对应文件必须存在"）。
- **在该表建立之前，本文档第 3 节的"未移植清单"只能当**选题索引**，不可当覆盖率引用。**
- **下一批的第一件事**：建立这张显式表（可先用 `TSculture*` / `TGas*` / `TCow*` / `TElf*` / `T*Zombi` / `TWhiteSkeleton`
  这几族做种子、逐类读一遍确认状态），再谈覆盖率。
