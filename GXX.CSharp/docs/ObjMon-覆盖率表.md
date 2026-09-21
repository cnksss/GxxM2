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

## 9. 批次J251：**显式声明**的尝试 —— 三种自动归属判定**全部失败**，归属必须**从台账人工播种**

J240 的结论与第 8 节都指向同一个修法：**别再猜归属，改成显式数据**。
本批据此实现"生成 + 注入声明行 + 只读声明行校验"的流水线
（脚本 `tools/gen-objmon-manifest.ps1`、修正版 `tools/audit-objmon-owner.ps1`），
并引入两个**与散文无关的不变量**：
**A 全集**（55 个类都要被声明）、**B 唯一**（每个类只能被一个文件认领）、
**C 存在**（声明里的行号必须与 `ObjMon.pas` 的实际声明行一致）。

### 9.1 三种归属判定都失败了（每种失败方式不同）

| # | 判定方式 | 结果 | 失败原因 |
|---|---|---|---|
| ① | 头 4 行 `///` 里的类名（J241 原判据） | 46 / 9 | **偏严**：同族第一个类名占住头 4 行、兄弟类名落到第 5 行之后 ⇒ 9 个假阴性（第 8.1 节） |
| ② | **全部** `///` 行里的类名（本批第一版 bootstrap） | 声称 55 / 55 | **偏松且错**：散文里的**跨类引用**被当成移植目标 ⇒ 注入后**唯一性校验报出 51 个重复** |
| ③ | **特征方法名**（单元内唯一的方法名）+ 只扫代码行 | 仅 **5 / 55** 可判定 | **过稀**：43 个类的**全部**方法名都与其他类共用（`Create`/`Destroy`/`Run`/`MagicAttackTarget` 等），另 7 个类的特征名在 C# 侧因改名而查不到 |

### 9.2 ②的失败细节（本批最有价值的一次"判据自证伪"）

②对 46 个文件跑出来的第一印象是**完美**的（"推断到归属 55/55、重复 0"），
但注入声明行后，**只读声明行**的唯一性校验立刻报出 **51 个类被多个文件认领**，例如：

```
TMLSBAttackMonster   -> ObjMonExtinguishFireCore.cs, ObjMonIcePeakCore.cs,
                        ObjMonMagicAttackCore.cs, ObjMonMagicNotMove2AttackCore.cs,
                        ObjMonMagicNotMoveAttackCore.cs, ObjMonMeteoriteRainCore.cs,
                        ObjMonMlsbCore.cs, ObjMonMon38_12Core.cs, ObjMonStoneFoxCore.cs
TChickenDeer         -> ObjMonATMonsterCore.cs, ObjMonChickenDeerCore.cs,
                        ObjMonCobwebCore.cs, ObjMonCore.cs, ObjMonExplosionCore.cs,
                        ObjMonSpiderSubclassCore.cs, ObjMonSpitSpiderCore.cs
```

最典型的是 `ObjMonCowFamilyCore.cs` 被分配了 `TGasAttackMonster`、`TMeteoriteRainAttackMonster` ——
**它根本没移植这两个类**；原因是 J243 的文档注释为了对照而写了
"与 J242 的 `TGasAttackMonster` 同型"、而 `TMeteoriteRainAttackMonster` 出现在同类比较句里。
⇒ **这正是 J240 记录的"共现 ≠ 归属"，只是换成了"全部 `///` 行"这个更宽的版本。**

**好消息**：注入是**可回滚**的（每文件仅 +1 行），
本批已 `git checkout` 撤销全部 46 个文件的注入、**未提交任何错误声明**
（核对：`0` 个文件仍含声明行）。**坏消息**：若不引入不变量 B，这批错误数据会被静默接受。

### 9.3 本批的第二个方法论失误：**空集上的全称命题恒真**

②在 **dry-run** 阶段就报过 `B 唯一 : 重复归属 0 => 通过` ——
但我当时**没有声明行**，`已声明 0 / 55`，于是"重复数为 0"是**空洞的真**，
我却把它读成了"唯一性没问题、可以注入"。
⇒ **教训：一个"通过"只有在**前提集合非空**时才携带信息；
不变量校验必须同时断言**基数**（A 全集），否则 B 的通过毫无意义。**
这也是本批把 A 与 B 并列断言的直接原因。

### 9.4 结论与下一步

- **归属不能从散文或方法名自动推断** —— 三种方式各有不同的失败形态（偏严 / 偏松且错 / 过稀）。
  唯一可信的来源是**逐批台账**（`docs/Checklist.md` 的 J-行）：
  它由人工为每批写明**类名 + 精确行号区间 + 新建的 C# 文件路径**（如 J244 的
  `TElfMonster` 2732-2890、`TElfWarriorMonster` 2893-3064 → `ObjMonElfFamilyCore.cs`）。
- **正确的落地顺序**应是：**先从台账人工播种 `docs/ObjMon-manifest.tsv`（55 行）**，
  再由脚本**只做校验**（A/B/C 三个不变量）——而不是让脚本去生成归属。
  本批只完成了工具与判定，**播种尚未完成**（下一批的首要任务）。
- 三个脚本保留：
  `gen-objmon-manifest.ps1`（生成+注入+校验流水线，**注入功能已验证可回滚**）、
  `audit-objmon-owner.ps1`（特征方法名判定，**已验证过稀**）、
  以及第 8 节的两个。**四者均不得单独用作验收**，其价值在于配合不变量做**反例检测**。

### 9.5 批次J252：第四种判定（**行号区间法**）也失败 —— 根因是**声明区与实现区不同段**

§9.4 说"归属只能从台账人工播种"，但本批先试了一条**与名字无关**的路：
用我在 J242 起**刻意记录**在每个 core 文件文档注释里的 `ObjMon.pas` **精确行号**
（如 `Create（1838-1842）`、`MagicAttackTarget`（6261-6368））去做归属 ——
想法是"数字不会因散文提到别的类而漂移"（脚本 `tools/gen-objmon-manifest-byrange.ps1`）。

**做法**：把类声明行排序得到每个类的"区间"`[L_i, L_{i+1})`，
统计各 core 文件 `///` 行里的整数落在哪个区间，命中最多者为归属。

**结果：46 / 55 归属、9 个未归属，且归属明细明显错误**，例如
`ObjMon36XCore.cs` 认领了 `TCowKingMonster` 与 `TWealthAnimalMon`、
`ObjMonATMonsterCore.cs` 认领了 `TFireSpiritMonster`、
`ObjMonCobwebCore.cs` 认领了 `TTortoiseMonster`。

**根因（本批最有价值的发现）**：
`ObjMon.pas` 是标准 Delphi 单元 —— **所有类声明集中在接口区（第 9-531 行）**，
而**方法实现全部在实现区（约第 540-9500 行）**。
于是用"**声明行**排序"构造出来的区间，与该类**方法实际所在的行**毫无关系：
一个类的声明区间可能只有 6 行宽（如 `TCowMonster` 是 [335,340]），
而它的实现却在 1838-1960；反过来，最后一个类 `TWealthAnimalMon`（声明于 531）
的区间被我算成了 [531, 9502] —— **吞掉了整个实现区**，
于是任何记录了 531 以后行号的文件都会"认领"它（实测命中 281 个数字）。

⇒ **这个错误与前三者不同：它不是判据偏严/偏松，而是我把两种坐标系混为一谈**
（声明序号 vs 实现行号）。**在同一个单元里，这两者是彼此独立的两套编号。**

**修正思路（尚未实施）**：应当用**实现区里的方法头行**（`procedure TFoo.Bar` 的行号）
构造每个类的"实现区间"，再与各文件记录的行号比对 ——
因为我在文件里记录的行号**本身就是实现行号**，两者同属一套坐标系。
本批**未实施**该修正，故行号区间法**仍属失败**。

### 9.6 四种判定的失败形态汇总（供日后避免重复踩坑）

| # | 判定 | 结果 | 失败形态 |
|---|---|---|---|
| ① | 头 4 行 `///` 的类名 | 46 / 9 | 偏严（假阴性 9） |
| ② | 全部 `///` 行的类名 | 声称 55 / 55 | 偏松且错（51 个重复认领，跨类引用） |
| ③ | 特征方法名 + 代码行 | 5 / 55 | 过稀（43 个类无特征方法名） |
| ④ | **声明行区间** + 记录的行号 | 46 / 55 | **坐标系混用**（声明序号 vs 实现行号） |

**四条都不能用作验收。** 结论不变：**归属须从台账人工播种**（§9.4），
脚本只负责校验 A/B/C 三个不变量；而 ④ 的修正（改用**实现区间**）
是一条**有明确理由**、值得下一批一试的路子，但它同样必须先通过"反例输入"检验（§9.3）。

### 9.7 批次J253：第五种判定（**实现头行匹配**）**成功** —— 并暴露 §9 不变量 B 本身过强

J252 的失败根因是"坐标系混用"（用**声明**行分区间、却拿**实现**行号去比）。
本批把坐标系统一到**实现区**：抽 `ObjMon.pas` 里所有 `Class.Method` **实现头行**
（共 **206** 个），再用各 core 文件 `///` 行里记录的行号去**投票**
（每个数字投给与之相差 `<= Tol`(=2) 的最近实现头所属的类）。

**结果：ObjMon 侧 55 / 55 全部归属、A 通过**，且明细**与我逐批的记忆完全一致**：

| 文件 | 认领的类 | 对应批次 |
|---|---|---|
| `ObjMonGasFamilyCore.cs` | `TGasAttackMonster`、`TGasMothMonster`、`TGasDungMonster` | J242 |
| `ObjMonCowFamilyCore.cs` | `TCowMonster`、`TMagCowMonster`、`TCowKingMonster` | J243 |
| `ObjMonElfFamilyCore.cs` | `TElfMonster`、`TElfWarriorMonster` | J244 |
| `ObjMonZombieFamilyCore.cs` | `TLightingZombi`、`TDigOutZombi`、`TZilKinZombi` | J245 |
| `ObjMonSkeletonScultureCore.cs` | `TWhiteSkeleton`、`TScultureMonster` | J246 |
| `ObjMonScultureKingCore.cs` | `TScultureKingMonster` | J247 |
| `ObjMonElectronicScolpionCore.cs` | `TElectronicScolpionMon` | J248 |
| `ObjMonDamageArmorCore.cs` | `TDamageArmorAttackMonster` | J249 |

**对照组（§9.3 的纪律，本批终于用上了）**：把**同一判据**拿去跑**未移植**的
`Client-HGE/AxeMon.pas`（52 类）—— 得到 **30 / 52**、**22 个未归属、A 失败**，
且票数普遍很低；而 ObjMon 侧票数普遍较高（多为 6-28）。
⇒ **这是五种判定里第一个"既能解出已知已移植单元、又会在已知未移植单元上失败"的方法**，
即它**具备区分力**，而不是 ② 那种"两边都通过"的空洞成功。

**但必须如实标注它的**残余风险**（不夸大）**：
- 对照组里仍被判为"已归属"的 **30/52** 是**数值巧合造成的假阳性**
  （AxeMon 的行号恰好落在 ObjMon 实现头附近）；
  故该判据**不能当作独立预言机**去判断"某个任意单元是否已移植"，
  只在**"ObjMon + 它自己的 core 文件集合"这一域内**经过核对。
- 因此本批的定位是：**判据已被"佐证"，尚未被"证明"**。
  真正的证据仍是**逐批台账 + 探针 + 测试**；本判据的价值在于
  **把台账结论自动化复算了一遍并得到一致结果**，且能在日后代码改动时**发现漂移**。

**本批同时发现：§9 的不变量 B（"每个类只能被一个文件认领"）本身过强、与现实不符。**
生成的 `docs/ObjMon-manifest.tsv`（56 行 = 表头 + 55 类）中 **41 个文件有归属、5 个文件为空** ——
但空的那 5 个**并非多余**：有些类**合法地跨两个文件**
（`TMonster` 在 `ObjMonCore.cs` + `ObjMonRunCore.cs`；`TFoxMonster` 在
`ObjMonFoxCore.cs` + `ObjMonFoxRunCore.cs`）。我的"单 owner"结构把这类**拆分的类**
只保留了第一个文件、丢掉了第二个。
⇒ **正确的不变量应当是**：
**A′ 全集**（每类至少一个文件）+ **B′ 反向**（每个文件声明的类都确实是它实现过的）
+ **D 覆盖**（对一个类，其**全部**实现方法都能在它声明的文件集合里找到）——
而不是"恰好一个 owner"。**该修正尚未实施**，是本批留下的明确缺口。

**产出**：`docs/ObjMon-manifest.tsv`（55 行清单，**可评审**）、
`tools/gen-objmon-manifest-bymethod.ps1`（判据 v5，含 `-PascalFile` 供对照组复用）。
四个"不可作终审"的旧脚本与本脚本**并存**，全部失败形态已记录在 §9.1/§9.2/§9.6。

### 9.8 批次J254：**A′/B′/D 不变量**实施 —— D 有效并通过 54/55，但"多文件"放宽会**重新引入假阳性**

按 §9.7 的结论，本批把判定升级到 v6（`tools/verify-objmon-manifest.ps1`）：
- **A′ 全集**：每个类至少一个文件
- **B′ 反向**：每个文件声明的类，该文件里确实记录了该类的实现头行（>= MinVotes 票）
- **D 覆盖**：对该类的**每一条**实现头行，其声明文件集合里都记录过对应行号（±Tol）
- 归属规则改为**多文件**：候选 = 票数 >= `Max(MinVotes, 25% × 该类最高票)`

**结果**：

| 不变量 | 结果 |
|---|---|
| A′ 全集 | **通过**（55 / 55 有归属） |
| B′ 反向 | **通过** |
| **D 覆盖** | **54 / 55 完全覆盖**；仅 **1 个**未完全覆盖 |

**D 抓到的那个类值得单独说（本批最有价值的产出）**：

```
TDevilkingMonster   覆盖 4 / 8 (0.5)
```

`TDevilkingMonster` 有 **8 个实现头**，而其声明文件集合里只记录了 **4 个**对应行号
⇒ **它可能只被移植了一半**。
注意这恰是 J241 判为"**误报**（其实是已移植）"的两个类之一 ——
本批的 D 不变量给出了**相反方向的证据**：它**未必完整**。
⇒ 这是五轮判定尝试以来**第一次由复查工具产出一个具体的、可执行的移植线索**，
应作为下一批的**首要核查项**（逐一核对那 8 个实现头里哪 4 个没有对应实现）。
（对照：其余 54 个类的实现头**全部**被覆盖，说明 D 并非普遍偏严 —— 它挑出的是孤例。）

**但"多文件"这一放宽本身失败了（必须如实记录）**：

17 个类被判为跨多文件，其中**多数是假阳性**，例如

```
TCowKingMonster  -> ObjMonCobwebCore.cs, ObjMonElectronicScolpionCore.cs,
                    ObjMonMagicNotMoveCore.cs, ObjMonMeteoriteRainCore.cs,
                    ObjMonMon35_2Core.cs, ObjMonXueLingLeaderCore.cs, ObjMonCowFamilyCore.cs   (7 个)
TDevilkingMonster -> 6 个       TDevilBat -> ObjMonDevilBatCore.cs + ObjMonZombieFamilyCore.cs
```

而**真正**跨文件的只有少数几个（`TMonster` = Core + RunCore、`TFoxMonster` = Fox + FoxRun 等）。
⇒ **v5（唯一最高票）精确但会丢掉合法的第二个文件；v6（25% 阈值）能捞回第二个文件、却引入大量假阳性** ——
这是**精度与召回的直接冲突**，靠调阈值解决不了（调高退回 v5、调低更糟）。

**结论**：**"哪些类真正跨多个文件"属于需要人工裁定的少量特例**
（从本次结果看约 5-8 个），应当**写进清单**而不是**用阈值推断** ——
这再次落到 §9.4 的结论上：**归属须人工播种、脚本只做校验**。
故本批**没有**用 v6 覆盖已提交的 `docs/ObjMon-manifest.tsv`（v5 的单 owner 版更精确），
v6 脚本**保留**用于提供 **D 覆盖**这一项（它是本批真正的增量价值）。

**当前状态的可验收范围（截至本批）**：
- 能**自动复算并一致**的：55 个类的归属（v5）、D 覆盖（v6，54/55）
- **不能**自动的：多文件的判定（须人工）、以及"该清单是否**完整**"（无独立预言机，见 §9.7 的对照组结论）
- 一个**待核查的具体线索**：`TDevilkingMonster` 的 8 个实现头里只有 4 个有对应记录

### 9.9 批次J255：**撤回 J254 对 `TDevilkingMonster` 的推断** —— D 的"4/8"有两处独立成因

J254 的 D 不变量报出 `TDevilkingMonster` **覆盖 4/8（0.5）**，我据此写下
"**它可能只被移植了一半**"并把它列为下一批首要线索。本批逐行核对后**必须撤回该推断**，
因为实际成因与我的解释不同，且 D 本身有一处缺陷：

**成因一（D 的缺陷）：D 把**注释掉的实现头**也算进了分母。**
`TDevilkingMonster` 的 8 个实现头里，有 **2 个位于一个 `(* *)` 块注释之内**：
`(*` 在第 **576** 行、`*)` 在第 **637** 行，故 **L577 `Think()`** 与 **L601 `GotoTargetXY`**
都是**死代码**（这正是 J239 记录过的**形态⑩**：同名定义两次、首次在块注释里 ——
J239 那次出现在**类声明**上，本处出现在**方法**上）。
⇒ **D 的分母应当是 6（活实现头）而不是 8** —— D **没有排除被注释掉的代码区**，
这是它的一处真实缺陷（与 J250/J252 那几次判据缺陷同属"没有处理注释区"这一类）。

**成因二（线索并未完全消失，但依据变了）：活的 6 个实现头是** ——
`Create`(L551，带 `// 004A8B74`)、`Destroy`(L566)、
`Operate(ProcessMsg: pTProcessMessage): Boolean`(L571)、`GotoTargetXY`(L639，活的这版)、
`RunToTargetXY`(L683)、`Run`(L703)。
而**在 C# 侧搜 `Devilking` / `GotoTargetXY` / `RunToTargetXY` / `Operate` 四个词，
`ObjMon36XCore.cs` 里**一个都没有**（本批实测）** ——
⇒ 尽管 v5 把 `TDevilkingMonster` 归属给了 `ObjMon36XCore.cs`（依据是该文件 `///` 行里记录的行号），
**该文件里却找不到这个类的任何成员** ⇒ **v5 的这次归属很可能是**假阳性**（数字巧合），
且 `TDevilkingMonster` **确实可能是未移植/欠移植**的 —— 但**理由与 J254 写的不同**：
不是"8 个头只覆盖 4 个"，而是"**归属本身就存疑、且该类成员在 C# 侧缺失**"。

**⇒ 校准后的结论**：
- J254 的"TDevilkingMonster 可能只被移植了一半"**措辞撤回**（其"4/8"含 2 个死代码头，是 D 的分母错误）。
- 但**同一批次的另一个证据（C# 侧查无此类成员）独立支持"它欠移植"** —— 线索**保留**，
  只是**依据换成了"成员缺失"而非"覆盖比 4/8"**，并**附带一个新的疑点**：
  **v5 对该类的归属可能是假阳性**（对照 §9.8 已记录 v5 会丢文件、v6 会造假的精度/召回冲突）。
- **D 需要修正**：计算分母时应**排除 `(* *)` 与 `{ }` 注释区内的实现头**；
  该修正**尚未实施**。

**本批顺带记录 `TDevilkingMonster` 的几处实质内容**（供日后补移植）：
- `Create` 设 `m_nViewRange := 5`、`m_nRunTime := 250`、
  `m_dwSearchTime := 3000 + Random(2000)`、`m_btRaceServer := **108**`、
  `m_boMission := True`、`m_nMissionPointIndex := 0`；并**把 `m_dwThinkTick` 初始化**。
- `Operate` 是**纯转发覆写**：`Result := inherited Operate(ProcessMsg);` ——
  形态④（空覆写）的**表达式形式**；其声明带 VMT 槽位 `// FFFC`。
- `Run` **开头无条件** `m_TargetCret := nil; m_Master := nil;` —— 即**无视一切目标与主人**。
- 块注释里那份**被废弃的寻路版 `GotoTargetXY`**（含嵌套函数 `WalkToNext`、`GotoNext`，
  用 `g_FindPath.FindPath1`）被活版取代，第 **638** 行的注释解释了原因：
  "**不用寻路算法。用英雄的处理方式看下到底么样，如果有问题再考虑还原上面的寻路 chongchong 2017-05-12**" ——
  即**两代实现并存、旧的一代被整段停放**，与 J233/J245 的"被禁一代在别处还活着"形成对照
  （**本处旧的一代是**真的停用了**）。`Run` 里对 `Think` 的调用也被 `{ }` 关掉（L721-724）。
- `TDevilkingMonster.Create` 的反编译地址注释 `// 004A8B74` 与第 **788** 行
  `constructor TMonster.Create; // 004A8B74` **完全相同** ⇒ 形态㉞（地址注释不唯一）再现。
