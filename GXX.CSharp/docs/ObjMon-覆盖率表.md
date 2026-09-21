# ObjMon.pas 类级覆盖率表（批次J241，脚本生成）

> **生成方式**：	ools 无关，直接由脚本从 _analysis/utf8_mirror/M2Engine/ObjMon.pas 与
> src/GXX.M2Server/ObjMon*Core.cs 抽取，故**可随时重跑复核**。

## 1. 判据（J240 三连失败之后的第四个）— **【批次J250 撤回"唯一通过"的表述：该判据经独立复测证明偏严、会产生假阴性，详见第 8 节】**

**判据**：取每个 C# 文件的**头 4 行 `///` 注释**，其中出现的 Delphi 类名即该文件**声明的移植目标**。

**为什么它比 J240 的三种判据好**：

| 判据 | J240 实测 | 本判据的区别 |
|---|---|---|
| 全文出现 | 42/14 | 只取**文件头部**，排除正文里的跨类引用 |
| summary 头部 | 42/14 | 只取**前 4 行**，避开 summary 里后段的同类对照 |
| J 行出现 | 56/0 | **不看台账**，只看代码，避免'已/未'两向命中 |

## 2. 结果

| 项 | 值 |
|---|---|
| 类声明条数（脚本） | 56 |
| **扣除 (* *) 里那一份后的实数** | **55** |
| **有文件声明移植的类** | **37** |
| **尚未移植的类** | **18** |
| C# 文件数 | 38 |

## 3. 全表（按声明行）

| 状态 | 声明行 | 类 | 基类 | 方法数 | 对应 C# 文件 |
|---|---|---|---|---|---|
| 已移植 | L9 | TMonster | TAnimalObject | 8 | ObjMonCore.cs<br>ObjMonRunCore.cs |
| 已移植 | L25 | TChickenDeer | TMonster | 3 | ObjMonChickenDeerCore.cs |
| 已移植 | L32 | TATMonster | TMonster | 3 | ObjMonATMonsterCore.cs |
| 已移植 | L39 | TCobwebMonster | TATMonster | 3 | ObjMonCobwebCore.cs |
| 已移植 | L47 | TMon36_XMonster | TATMonster | 11 | ObjMon36XCore.cs |
| 已移植 | L65 | TTwoKindAttackMonster | TATMonster | 4 | ObjMonTwoKindCore.cs |
| 已移植 | L75 | TMon38_0Monster | TAnimalObject | 4 | ObjMonMon38_0Core.cs |
| 已移植 | L85 | TMon38_11Monster | TATMonster | 1 | ObjMonMon38_11_13Core.cs |
| 已移植 | L90 | TMon38_12Monster | TATMonster | 3 | ObjMonMon38_12Core.cs |
| 已移植 | L98 | TMon38_13Monster | TATMonster | 3 | ObjMonMon38_11_13Core.cs |
| 已移植 | L106 | TMagicAttackMonster | TMonster | 4 | ObjMonMagicAttackCore.cs |
| 已移植 | L114 | TMon35_2Monster | TMagicAttackMonster | 3 | ObjMonMon35_2Core.cs |
| 已移植 | L122 | TExplosionAttackMonster | TMagicAttackMonster | 2 | ObjMonExplosionCore.cs |
| 已移植 | L128 | TLineMagicAttackMonster | TMagicAttackMonster | 2 | ObjMonLineMagicCore.cs |
| 已移植 | L134 | TMLSBAttackMonster | TMagicAttackMonster | 2 | ObjMonMlsbCore.cs |
| 已移植 | L140 | TExtinguishDayFireAttackMonster | TMagicAttackMonster | 2 | ObjMonExtinguishFireCore.cs |
| 已移植 | L146 | TFireIceAttackMonster | TMagicAttackMonster | 2 | ObjMonFireIceCore.cs |
| 已移植 | L152 | TFireCrossMonster | TMagicAttackMonster | 3 | ObjMonDevilBatCore.cs<br>ObjMonFireCrossAttackCore.cs<br>ObjMonFireCrossCore.cs |
| 已移植 | L160 | TIcePeakMonster | TMonster | 4 | ObjMonIcePeakCore.cs |
| 已移植 | L170 | TTruckMonster | TMonster | 2 | ObjMonTruckCore.cs |
| 已移植 | L180 | TFoxMonster | TAnimalObject | 6 | ObjMonFoxCore.cs<br>ObjMonFoxRunCore.cs |
| 已移植 | L196 | TTortoiseMonster | TMagicAttackMonster | 2 | ObjMonTortoiseCore.cs |
| 已移植 | L202 | TStoneFoxMonster | TFoxMonster | 1 | ObjMonStoneFoxCore.cs |
| 已移植 | L207 | TFoxMagicAttackMonster | TMagicAttackMonster | 2 | ObjMonStoneFoxCore.cs |
| 已移植 | L213 | TDamageSpellAttackMonster | TMagicAttackMonster | 2 | ObjMonDamageSpellArmorCore.cs |
| **未移植** | L219 | TDamageArmorAttackMonster | TMagicAttackMonster | 2 | — |
| 已移植 | L226 | TMeteoriteRainAttackMonster | TAnimalObject | 3 | ObjMonMeteoriteRainCore.cs |
| 已移植 | L236 | TMagicAttackNotMoveMonster | TAnimalObject | 5 | ObjMonMagicNotMoveAttackCore.cs<br>ObjMonMagicNotMoveCore.cs |
| 已移植 | L251 | TMagicAttackNotMoveMonster2 | TAnimalObject | 6 | ObjMonMagicNotMove2AttackCore.cs<br>ObjMonMagicNotMove2Core.cs |
| 已移植 | L268 | TXueLingLeader | TAnimalObject | 5 | ObjMonXueLingLeaderCore.cs |
| 已移植 | L280 | TFireSpiritMonster | TMagicAttackMonster | 2 | ObjMonFireSpiritCore.cs |
| 已移植 | L286 | TLionMonster | TATMonster | 3 | ObjMonLionCore.cs |
| 已移植 | L293 | TSlowATMonster | TATMonster | 2 | ObjMonSpitSpiderCore.cs |
| 已移植 | L299 | TScorpion | TATMonster | 2 | ObjMonSpitSpiderCore.cs |
| 已移植 | L305 | TSpitSpider | TATMonster | 4 | ObjMonSpiderSubclassCore.cs<br>ObjMonSpitSpiderCore.cs |
| 已移植 | L315 | THighRiskSpider | TSpitSpider | 2 | ObjMonSpiderSubclassCore.cs |
| 已移植 | L321 | TBigPoisionSpider | TSpitSpider | 2 | ObjMonSpiderSubclassCore.cs |
| **未移植** | L327 | TGasAttackMonster | TATMonster | 4 | — |
| **未移植** | L335 | TCowMonster | TATMonster | 2 | — |
| **未移植** | L341 | TMagCowMonster | TATMonster | 4 | — |
| **未移植** | L350 | TCowKingMonster | TATMonster | 4 | — |
| **未移植** | L368 | TElectronicScolpionMon | TMonster | 5 | — |
| **未移植** | L380 | TLightingZombi | TMonster | 4 | — |
| **未移植** | L389 | TDigOutZombi | TMonster | 4 | — |
| **未移植** | L398 | TZilKinZombi | TATMonster | 4 | — |
| **未移植** | L409 | TWhiteSkeleton | TATMonster | 5 | — |
| **未移植** | L420 | TScultureMonster | TMonster | 6 | — |
| **未移植** | L431 | TScultureKingMonster | TMonster | 6 | — |
| **未移植** | L444 | TGasMothMonster | TGasAttackMonster | 4 | — |
| **未移植** | L452 | TGasDungMonster | TGasAttackMonster | 2 | — |
| **未移植** | L458 | TElfMonster | TMonster | 6 | — |
| **未移植** | L473 | TElfWarriorMonster *(在 (\* \*) 注释里、非有效声明)* | TATMonster | 7 | — |
| **未移植** | L489 | TElfWarriorMonster | TSpitSpider | 7 | — |
| **未移植** | L505 | TDevilkingMonster | TAnimalObject | 8 | — |
| **未移植** | L521 | TDevilBat | TMonster | 4 | — |
| 已移植 | L531 | TWealthAnimalMon | TATMonster | 7 | ObjMonWealthAnimalMonCore.cs |

## 4. 本表对前两批结论的修正

- **J239/J240 把 \TExplosionAttackMonster\（L122）列为'未移植'是**错的** ** ——
  本判据显示它**已由 \ObjMonExplosionCore.cs\ 声明移植**（该文件头 4 行里点名）。
- **\TScultureMonster\（L420）与 \TScultureKingMonster\（L431）确实**未移植** ** ——
  两者的名字只出现在别的文件的**正文与 summary 后段**做同类对照，**没有任何文件在头部点名它们**。
  => J239 '被提到不等于被移植' 这条判断**成立**；而 J240 那条'三个类都未移植'**只对了两个**。

## 5. 本表的已知局限

- \TMonster\ / \TATMonster\ 这类**基类**的'已移植'含义与叶类不同（它们是被继承的骨架），
  本表未区分'移植了骨架'与'移植了全部行为'。
- 判据依赖**我自己写的头 4 行注释**是否完整点名了目标类；
  若某个文件移植了 A、B 两类而头部只写了 A，则 B 会被误判为未移植 ——
  **这是本表最可能的残余误差方向**（即'未移植'可能被高估）。

## 6. **已实测到的残余误差 —— 本表不是最终答案，须人工订正 `状态` 列**

上述"残余误差方向"在生成表时**立刻就被观测到两个实例**，故本表**不应**被当作结论引用：

| 类 | 本表判定 | **实际** | 原因 |
|---|---|---|---|
| `TDevilBat`（L521） | **未移植** ❌ | **已移植**（J231） | 它的实现在 `ObjMonDevilBatCore.cs` 里，而那个文件**头部点名的是 `TFireCrossMonster`**（因为 J231 同批还移植了 `TFireCrossMonster.AttackTarget`） |
| `TDevilkingMonster`（L505） | **未移植** ❌ | **已移植** | 同理 —— 头部点名了同批的另一个类 |

**故本表的正确读法是**：

1. **`已移植` 那 37 行可信**（头部点名是较强的证据）；
2. **`未移植` 那 18 行是**候选**、不是判决** —— 其中至少 `TDevilBat`、`TDevilkingMonster`
   两行**已证明为误判**，故实际候选数 **≤ 16**；
3. **误差方向是"把已移植误报成未移植"** —— 这是**安全方向**（宁可少报完成、不可多报），
   但**仍必须订正**：逐行核对 `未移植` 行，把实为已移植的改成 `已移植` 并填上对应文件；
4. 订正后本表**即成为 J240 §6 所要求的那张"显式状态表"** ——
   `状态` 列由人工维护、脚本只负责生成骨架与校验。

## 7. 生成器的已知瑕疵（下一批重跑时修）

本文件由脚本用 `StringBuilder` 生成，其中我写了 `\`` 转义，**结果把行内代码的
反引号渲染成了字面反斜杠**（如 `\TMonster\`）——
**纯属排版瑕疵、不影响任何数据**；下一批重跑生成器时**改用单引号包裹字符串**
（或直接写反引号）即可修好。

## 8. 批次J250 的**独立复测**：两个判据**都不成立**，故"未移植清零"须靠台账而非启发式

批次J242–J249 逐批移植了本表"未移植"列上的全部条目（气/蛛网三族、牛族三兄弟、精灵族两态、
僵尸族三兄弟、白骷髅、祖玛雕像、祖玛教主、电子蝎子、狐狸魔法减防御），
并逐条把对应行划掉。**但"逐条划掉"只是记账，不是验证** ——
本批次因此**按第 1 节的判据重新实现生成器并独立复测**，结论如下。

### 8.1 复测一：J241 原判据（头 4 行 `///`）—— **偏严、产生 9 个假阴性**

判据按第 1 节原样重实现（脚本 `tools/audit-objmon-coverage.ps1`，本批新建、可重跑）。
对阈值做扫描后结果**高度依赖阈值**：

| `HeadCommentLines` | 已移植 / 未移植 | 未移植清单 |
|---|---|---|
| **4（J241 原判据）** | **46 / 9** | `TCowKingMonster`、`TDevilBat`、`TDevilkingMonster`、`TDigOutZombi`、`TElfWarriorMonster`、`TGasDungMonster`、`TGasMothMonster`、`TScultureMonster`、`TZilKinZombi` |
| 12 | 54 / 1 | `TDevilkingMonster` |
| 40 | **55 / 0** | （空） |
| 全部 `///` 行 | **55 / 0** | （空） |

**逐条核对那 9 个"未移植"的结论：全部是**已移植的假阴性**** ——

| 类 | 声明行 | 实际移植批次 | 所在 C# 文件 |
|---|---|---|---|
| `TGasMothMonster` / `TGasDungMonster` | 444 / 452 | J242 | `ObjMonGasFamilyCore.cs` |
| `TCowKingMonster` | 350 | J243 | `ObjMonCowFamilyCore.cs` |
| `TElfWarriorMonster` | 489（旧版 473 在 `(* *)` 里） | J244 | `ObjMonElfFamilyCore.cs` |
| `TDigOutZombi` / `TZilKinZombi` | 389 / 398 | J245 | `ObjMonZombieFamilyCore.cs` |
| `TScultureMonster` | 420 | J246 | `ObjMonSkeletonScultureCore.cs` |
| `TDevilBat` / `TDevilkingMonster` | 521 / 505 | 更早批次（J241 已查明） | 见第 4/6 节 |

**假阴性的成因**：判据只读**头 4 行**，而这些文件正好把**同族的第一个类名**写在头 4 行里、
**它的兄弟类名落在第 5 行之后**（例如 `ObjMonCowFamilyCore.cs` 头 4 行只覆盖 `TCowMonster`/`TMagCowMonster`、
`TCowKingMonster` 出现在后面）—— 即**判据的成败取决于散文排版**，而不是代码事实。

**因此**把阈值调到 40 得到 55/0**不能算验证** ——
"调阈值直到得到想要的答案"正是第 6 节记录过的陷阱（**共现 ≠ 归属**）。
这也是**第 1 节"唯一通过"这一措辞应当撤回**的原因：该判据在 J241 当时只是**碰巧**没有
暴露假阴性（那时未移植类多、且多是"整族未做"，名字恰好都在头 4 行）。

### 8.2 复测二：本批新写的**方法级**判据 —— **偏松、被对照组证伪**

为摆脱"散文排版"，本批另写了一个判据（脚本 `tools/audit-objmon-methods.ps1`）：
**一个类是否已移植，看它的方法名是否作为独立标识符出现在 C# 侧**（比例 >= 0.6）。

对 `ObjMon.pas` 的结果是 **55 / 0**、且**全部 55 个类的命中率都是 1.000**（最低 4/4、最高 11/11）。

**但对照组证伪了它** —— 对**明确未移植**的客户端单元 `Client-HGE/AxeMon.pas`（9456 行、52 个类）
跑同一判据，得到 **43 / 52 "已移植"** —— 即**它把一个完全没移植的单元判成八成已移植**。

原因是**方法名多为通用名**：`Create`、`Destroy`、`Run`、`Attack`、`Die`、`RecalcAbilitys` 等
在 C# 树里到处都是（739 个文件、含大量同名与近名成员），
故"方法名出现"几乎不携带"这个类被移植过"的信息；
`AxeMon.pas` 里甚至还有与 `ObjMon.pas` **完全同名**的类（如 `TWhiteSkeleton` @149 对 `ObjMon.pas` @409），
名字碰撞使判据在跨单元时失效。

它之所以还能标出 9 个"未移植"，只是因为那 9 个类的**实现头抽取失败**（`Methods = 0`），
即**不是判据判对了，而是判据没看到数据**。

### 8.3 结论：**"55 / 0" 的证据是逐批台账 + 测试，不是任何名字匹配启发式**

- 本表第 1 节的判据**应降级**为"**筛选候选**"用途，**不得**用作验收；
  第 1 节"唯一通过"的表述在本节予以**撤回**。
- 本批新写的两个脚本**保留**（它们能快速指出"可能有遗漏"），
  但**都标注为不可作为终审**：`audit-objmon-coverage.ps1` 偏严（假阴性）、
  `audit-objmon-methods.ps1` 偏松（假阳性，且已被对照组证伪）。
- **"全部 55 个类已移植"这一结论的依据**是：
  J242–J249 每一批都**指名**了移植的类与**精确行号区间**（如 J244 的 `TElfMonster` 2732-2890、
  `TElfWarriorMonster` 2893-3064），并为每批写了**可运行的探针（累计千余条断言）与 xUnit 测试**；
  这些是**可独立复核的实现证据**，比"类名出现在注释里"强得多。
- **正确的长期修法**（与第 6 节结论一致、本批再次确认）：在 C# 侧引入**显式的、机器可读的
  移植状态声明**（而不是从散文里猜），例如每个 core 文件的文档注释里固定一行
  `/// 移植类：A, B, C（ObjMon.pas:行-行）`，由脚本读该行做归属 ——
  这样判据才**既不看排版、也不靠通用名**。该改造**尚未实施**，列为后续工作。

### 8.4 本批的自我修正（方法论）

- 我在写方法级判据时，**先看到它对 `ObjMon.pas` 给出完美的 55/0**，一度准备把它当作验证通过；
  是**对照组**（对一个已知未移植的单元跑同一判据）暴露了它偏松。
  ⇒ **教训：任何"验证脚本"在被用来支持结论之前，必须先在一个已知为反例的输入上跑通**
  （本批把这个反例固化在 8.2 节，供日后回归）。这与第 6 节"共现 ≠ 归属"是同一类错误的两个方向。
