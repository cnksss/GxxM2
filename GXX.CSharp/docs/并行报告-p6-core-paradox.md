# 并行报告：车道 `p6-core-paradox`

**目标单元**：`Source\RunGate\ParadoxDataSet.pas`（GBK，实测 1,362 行，62,954 字节）
**工作树**：`D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p6-core-paradox`（分支 `par/p6-core-paradox`）
**落位**：`GXX.CSharp/src/GXX.Core/Paradox/**`（新建子目录）+ `GXX.CSharp/tests/GXX.Core.Tests/ParadoxDataSet*`

---

## 1. 两个副本"只移植一份"的落地说明（本车道的核心裁定）

### 1.1 副本同一性（实测，不是转述）

| 文件 | 字节 | LF | CR | SHA256 |
|---|---|---|---|---|
| `Source\RunGate\ParadoxDataSet.pas` | 62,954 | 1362 | 1362 | `904B84DF8B40A1BF6211DF609D025540EFCEF57441D7B2C0D6530871B51D136F` |
| `Source\GameCenter\ParadoxDataSet.pas` | 62,954 | 1362 | 1362 | `904B84DF8B40A1BF6211DF609D025540EFCEF57441D7B2C0D6530871B51D136F` |

两者**逐字节相同**（同 SHA256、同行数、同字节数），且都是 **CRLF**（`LF=CR=1362`，无裸 LF 混用）。
> ⚠ 派发提示词里给的 SHA256 是 `B87DC941…`，与工作树实测的 `904B84DF…` 不一致。
> 这不影响"两份副本相同"这一结论（两条路径实测同哈希），但**该哈希值本身对不上**，
> 建议派发方核对它当时的归一化方式（疑似对"归一化行尾后"的字节流取哈希，而实测文件已是纯 CRLF）。

### 1.2 结论：可以放在 `GXX.Core`，但**必须走接缝**

原文 `uses`（`ParadoxDataSet.pas:55`）：`DB, Classes, SysUtils, Forms, ParadoxConv;`

| 依赖 | 托管侧现状 | 方向 | 处置 |
|---|---|---|---|
| `DB`（TDataSet/TField/TFieldDefs/TFieldType…） | **全仓零命中**（`git grep -E "(class\|struct\|enum\|interface) +(TDataSet\|TField\|TFieldType)\b" src/**/*.cs` → 空） | 无 | 新建最小接缝 `ParadoxDataSet.Seams.cs` |
| `Classes`（TFileStream/TMemoryStream） | `TFileStream` 全仓零命中；`TMemoryStream` 在 `GXX.Client.GUI.Share.FStateSeams.cs:173` 有一个占位 | 无 | 同上（见 §6 重名复核） |
| `SysUtils` | `GXX.Core.Rtl.DelphiRTL`（Pos/Copy/Format…） | 同层 | 直接用 |
| `Forms`（`Application.HandleException`，原文 :767） | 无 | 无 | 接缝 `ParadoxApplication.OnHandleException` |
| **`ParadoxConv`** | **`GXX.RunGate/ParadoxConv.cs`（namespace `GXX.RunGate`，public static partial class）** | **反了** | **必须接缝** |

**引用方向（关键）**：`GXX.RunGate.csproj:9` 有 `<ProjectReference Include="..\GXX.Core\GXX.Core.csproj" />`
（`GXX.GameCenter.csproj:9` 同样引用 `GXX.Core`）。
⇒ **`GXX.Core` 不能反向引用 `GXX.RunGate`**（循环工程引用）。因此**不能**在 `GXX.Core` 里直接
`using GXX.RunGate;` 调 `ParadoxConv.Encoding`，也**不能**把 `ParadoxConv.cs` 复制一份过来
（台账 §15：已有真实现就不要另造接缝本体；且会造成第二份转换器）。

**采用的方案（接缝 + 形状声明，不复制实现）**：
`GXX.Core/Paradox/ParadoxConvSeam.cs` 声明 3 个入口的形状 + 可注入钩子：
* `TEncodingKind`（UCS4/UTF8/KOI8R/ISO88595/CP1251/CP866，**与 `ParadoxConv.pas:49` 同序 ⇒ 整数值与 `GXX.RunGate.TEncodingKind` 完全一致**，可直接转型）
* `ParadoxConvSeam.GetCodepageHook : Func<string>`（默认实现 = 原文 :82-99 的 WINDOWS 分支）
* `ParadoxConvSeam.EncodingHook : Func<TEncodingKind, TEncodingKind, string, string>`（默认 = 恒等）

**可行性结论：可行**。`TParadoxDataSet` 对 `ParadoxConv` 的依赖只有 **3 个入口 / 6 个调用点**：
`GetCodepage`（:1171）+ `Encoding`（:1349/:1350/:1354/:1355/:1356）。
接缝面积小、语义明确，且**不需要**在 `GXX.Core` 复制转换器本体。

**集成方接线（一行，落在白名单外文件，故由集成方做 —— 见 §7）**：

```csharp
// 在 GXX.RunGate / GXX.GameCenter 的启动路径（或调用 TParadoxDataSet 之前）执行一次：
GXX.Core.Paradox.ParadoxConvSeam.GetCodepageHook = GXX.RunGate.ParadoxConv.GetCodepage;
GXX.Core.Paradox.ParadoxConvSeam.EncodingHook =
    (src, dst, s) => GXX.RunGate.ParadoxConv.Encoding(
        (GXX.RunGate.TEncodingKind)(int)src, (GXX.RunGate.TEncodingKind)(int)dst, s);
```
（`GXX.GameCenter` 侧需同时引用 `GXX.RunGate`，或把 `ParadoxConv` 下沉/共享 —— 见 §7 的精确签名要求。）

**默认接缝的真实后果（要知情）**：不接线时 `EncodingHook` 是恒等，
而 `GetCodepageHook` 的默认实现返回 `'CP' + GetACP`。由于原文 :1349-1356 比较的常量是
`'UTF-8'/'KOI8R'/'CP1251'`，**两者在 Windows 上永不相等**，所以 `EncodingString` 恒等于入参 ——
**这与原文在 Windows 上的实际行为一致**（原文缺陷 D13），不是接缝引入的偏差。

---

## 2. 全部 commit hash

| commit | 内容 |
|---|---|
| `ae70b566` | WIP 切片A：记录布局 + 字段类型常量 1:1（原文 117-136 / 425-517 / 97-115）；PxLangTable 脚本抽取 118 项 + 回读比对脚本 |
| `6cc7afae` | WIP 切片B：DB/Classes 最小接缝 + `TParadoxDataSet` 1:1 主体（原文 640-1361，29 个成员）；`GXX.Core` 构建 0 error |
| `8a14e8ea` | 切片C：全量测试 117 例（649 总 / 0 失败）；修正记录布局为 Explicit FieldOffset；修正接缝 `Eof`/`Value` 后备字段与 `GetFieldData` False 传播 |
| `69d2eb22` | 切片D：补 `EncodingField`/`EncodingMemo` 路径与 `FileLoc` 符号位差异断言（653 例全绿）；登记 D17/D18；跨车道重名复核 |
| `8a14e8ea`→后续 | 切片E：抓出并修复 **D19 未初始化内存导致的不确定行为**（空表 `First` 后 `RecNo` 约 1/6 概率为随机值）；连跑 10 次门禁全绿 |
| `982333eb` | 切片E 定稿：修 D19；报告落 `docs/并行报告-p6-core-paradox.md` |
| `4e0c06f4` | 报告计数精确化 |
| 见 §9 后续提交 | **复核提交**：D19 根因三段互证（排除 stale pointer 假设）+ 加固测试（整块头部断言 + 独立"残留复用"证据用例）+ 工作树事故记录 |

（`ae70b566`/`6cc7afae` 标记 `WIP-不可合并` 是因为当时唯一允许的提交点；**最终提交 `69d2eb22` 是全绿可合并的**。）

---

## 3. 逐方法族判定表

| # | 原文方法 | 原文行 | 状态 | 备注 |
|---|---|---|---|---|
| 1 | `TParadoxDataSet.GetBookmarkFlag` | 755-758 | ✅ 已移植 + 覆盖 | 经 `PxRecordHeaderOps` |
| 2 | `SetBookmarkFlag` | 760-763 | ✅ | |
| 3 | `InternalHandleException` | 765-768 | ✅ + **接缝** | `Application.HandleException` → `ParadoxApplication` |
| 4 | `InternalInitFieldDefs` | 771-848 | ✅ + **差异断言** | 含缺陷 D2/D17（字段名读到 TableName 区之后） |
| 5 | `InternalOpen` | 850-906 | ✅ | 含 D3（FileType=1 被拒） |
| 6 | `IsCursorOpen` | 908-911 | ✅ | |
| 7 | `InternalClose` | 913-925 | ✅ + **接缝** | `BindFields/DestroyFields` 上移到 `TDataSet.Close`（顺序等价） |
| 8 | `GetRecord` | 927-1001 | ✅ + **差异断言** | 含 D4（RecordSize=0 除零）、EOF 游标不前进、缓冲清零 |
| 9 | `AllocRecordBuffer` | 1003-1006 | ✅ | `GetMem` → `Marshal.AllocHGlobal` |
| 10 | `FreeRecordBuffer` | 1008-1011 | ✅ | `FreeMem` → `Marshal.FreeHGlobal` |
| 11 | `InternalInitRecord` | 1013-1016 | ✅ | 原文**空实现**，照抄（测：不写任何字节） |
| 12 | `InternalFirst` | 1018-1021 | ✅ | |
| 13 | `InternalLast` | 1023-1026 | ✅ + **差异断言** | D15：`RecordCount+1` 无法经 `SetRecNo` 复现 |
| 14 | `InternalSetToRecord` | 1028-1031 | ✅ | |
| 15 | `GetCanModify` | 1033-1036 | ✅ | 恒 False |
| 16 | `GetRecordCount` | 1038-1041 | ✅ | |
| 17 | `SetRecNo` | 1043-1048 | ✅ + **差异断言** | 越界静默忽略（4 例） |
| 18 | `GetRecNo` | 1050-1053 | ✅ | |
| 19 | `SetTableName` | 1055-1062 | ✅ | Active 先 Close；同值不 Close |
| 20 | `GetLanguage` | 1064-1070 | ✅ | 1..118 边界 |
| 21 | `SetLanguage` | 1072-1091 | ✅ + **差异断言** | 大小写敏感；非 Active 清零 |
| 22 | `NativeToFieldType` | 1093-1115 | ✅ | 17 分支 + 3 个"未列出"（`ftUnknown`）|
| 23 | `ReadDataBlock` | 1117-1123 | ✅ | 越界抛 `Block %d read error` |
| 24 | `DetectLang` | 1125-1151 | ✅ + **差异断言** | 现代/老版本两条路径 + 首命中语义 |
| 25 | `EncodingField` | 1153-1166 | ✅ | OnEncode 优先；`FLanguageID<1` 直接 Exit |
| 26 | `Create` | 1168-1173 | ✅ | `GetCodepage`（接缝）+ `EncodingMemo := True` |
| 27 | `Destroy` | 1175-1178 | ✅ | 原文仅 `inherited Destroy` |
| 28 | `GetFieldData` | 1180-1247 | ✅ + **差异断言** | 逆序读 + xor $80（D6/D7）；Blob 族落空（D8） |
| 29 | `CreateBlobStream` | 1249-1341 | ✅ + **差异断言** | 行内/单块/索引块三条路径；D9/D10/D11/D18 |
| 30 | `EncodingString` | 1343-1359 | ✅ + **差异断言** | D12（无 0 号保护）/D13（常量永不相等） |
| — | `PxLangTable`（118 项） | 520-638 | ✅ **脚本抽取** | 见 §4 |
| — | 记录布局 `TFieldInfoRecord`/`TDataBlock`/`TPxFileHeader`/`TPxDataHeader`/`TPxRecordHeader`/`TPxBlob`/`TPxBlobIdx`/`TPxLang` | 117-517 | ✅ | 布局断言（§5） |
| — | 属性族（`FileHeader`/`DataHeader`/`SortOrderID`/`TableName`/`Language`/`Codepage`/`EncodingMemo`/`OnEncode`/`Active`） | 707-746 | ✅ | 全部有测试 |
| — | 原文 716-745 的 `published` 转发属性（Filter/BeforeOpen/OnCalcFields…） | 716-745 | ⛔ **不移植** | 全部是 `TCustomDataSet` 已有属性的**重声明**，无一行新逻辑；VCL 数据集状态机不在托管接缝范围（见 §6） |

**接缝清单（原文依赖但未移植，需集成方或后续车道接入）**

| 接缝 | 原文调用点 | 当前状态 | 归属 |
|---|---|---|---|
| `ParadoxConvSeam.GetCodepageHook` | :1171 | 默认实现（等价 WINDOWS 分支） | 集成方一行接线 |
| `ParadoxConvSeam.EncodingHook` | :1349/:1350/:1354/:1355/:1356 | 默认恒等 | 集成方一行接线 |
| `ParadoxApplication.OnHandleException` | :767 | 默认静默 | 集成方注入 |
| `TDataSet`/`TField` 族 | :676-706 全部 override | 已实现最小子集（本车道） | **本车道**（放 `GXX.Core.Paradox`） |
| `DB.pas` 完整数据集状态机 | 无调用点 | **不移植** | 不适用 |

---

## 4. 新增文件 + 覆盖行号范围

| 文件 | 行数 | 覆盖的原文行号 |
|---|---|---|
| `src/GXX.Core/Paradox/ParadoxDataSet.cs` | 959 | **640-749**（类声明 + `TEncodeEvent` + `EParadoxError`）、**751-1361**（implementation 全部 30 个方法体） |
| `src/GXX.Core/Paradox/ParadoxDataSet.Records.cs` | 411 | **97-115**（17 个 pxf 常量）、**117-129**（TFieldInfoRecord）、**132-136**（TDataBlock）、**425-470**（TPxFileHeader）、**472-491**（TPxDataHeader）、**493-497**（TPxRecordHeader）、**499-503**（TPxBlob）、**505-510**（TPxBlobIdx）、**512-517**（TPxLang）；原文 139-423 的偏移说明注释以 XML 注释保留语义 |
| `src/GXX.Core/Paradox/ParadoxDataSet.Seams.cs` | 937 | 接缝（**不覆盖原文行**，仅承载依赖形状） |
| `src/GXX.Core/Paradox/ParadoxConvSeam.cs` | 112 | 接缝（**不覆盖原文行**） |
| `src/GXX.Core/Paradox/ParadoxDataSet.Tables.g.cs` | 135 | **520-638**（`PxLangTable` 118 项，脚本抽取，只读生成物） |
| `src/GXX.Core/Paradox/Gen/extract-pxlangtable.ps1` | 123 | 抽取脚本（含 4 项硬校验） |
| `src/GXX.Core/Paradox/Gen/verify-pxlangtable.ps1` | 89 | 回读比对脚本 |
| `tests/GXX.Core.Tests/ParadoxDataSetTests.cs` | 1615 | 全部测试 |
| `docs/并行报告-p6-core-paradox.md` | 345 | 本报告 |

**逐方法覆盖行号（原文行）**

| 方法 | 覆盖 | 未覆盖 |
|---|---|---|
| `GetBookmarkFlag` 755-758 | 全 | — |
| `SetBookmarkFlag` 760-763 | 全 | — |
| `InternalHandleException` 765-768 | 全 | — |
| `InternalInitFieldDefs` 771-848 | 771-805、811-847 | **806-810**（`{$IFDEF FPC}` 的 `ReadByte` 分支 —— 原文非 FPC 走 `Read(B,1)`，即已移植分支） |
| `InternalOpen` 850-906 | 全（含 .mb 大小写两条尝试） | — |
| `IsCursorOpen` 908-911 | 全 | — |
| `InternalClose` 913-925 | 914-924 | — |
| `GetRecord` 927-1001 | 全 | — |
| `AllocRecordBuffer` 1003-1006 | 全（含 D19 清零差异断言） | — |
| `FreeRecordBuffer` 1008-1011 | 全 | — |
| `InternalInitRecord` 1013-1016 | 全（空实现） | — |
| `InternalFirst` 1018-1021 | 全 | — |
| `InternalLast` 1023-1026 | 全 | — |
| `InternalSetToRecord` 1028-1031 | 全 | — |
| `GetCanModify` 1033-1036 | 全 | — |
| `GetRecordCount` 1038-1041 | 全 | — |
| `SetRecNo` 1043-1048 | 全（合法 2 例 + 越界 4 例） | — |
| `GetRecNo` 1050-1053 | 全 | — |
| `SetTableName` 1055-1062 | 全（同值/异值/未开） | — |
| `GetLanguage` 1064-1070 | 全（范围内 6 例 + 越界 1 例） | — |
| `SetLanguage` 1072-1091 | 全（命中/大小写不符/未命中/非 Active） | — |
| `NativeToFieldType` 1093-1115 | 全 17 分支 + 3 未列出 | — |
| `ReadDataBlock` 1117-1123 | 全（含越界抛异常） | — |
| `DetectLang` 1125-1151 | 全（现代 3 字段/CodePage 不符/老版本 2 例） | — |
| `EncodingField` 1153-1166 | 全（OnEncode 优先 + 返回 null + `FLanguageID<1`） | — |
| `Create` 1168-1173 | 全 | — |
| `Destroy` 1175-1178 | 全（原文仅 `inherited`） | — |
| `GetFieldData` 1180-1247 | 1187-1191（nil）、1194-1223（空值/逆序）、1225-1246（case 全分支） | — |
| `CreateBlobStream` 1249-1341 | 1260-1341（含三条主路径 + 无 .mb + 负偏移） | **1327-1331**（原文注释掉的 `StrLCopy` 代码块 —— 逐字保留为注释，不可执行） |
| `EncodingString` 1343-1359 | 全（1251/866/其它 3 类） | — |

---

## 5. 测试、构建与门禁结果

| 项 | 结果 |
|---|---|
| `dotnet build GXX.slnx -c Debug` | **Build succeeded / 0 Error(s)**（本车道文件零 warning） |
| `dotnet test tests\GXX.Core.Tests` | **Passed! Failed: 0, Passed: 654, Skipped: 0, Total: 654**（D19 根因复核后**共 31 次全绿**：默认 15 + 各 verbosity 12 + 强并行 4；见 §9） |
| 车道前基线（同工作树实测） | **532 例**（不是派发词里的 550 —— 见下） |
| 本车道新增 | **121 例**（`ParadoxDataSetTests.cs`：73 个 `[Fact]` + 48 行 `[Theory]` 数据），0 失败 |

> **门禁可重复性**：修复 D19 后，`dotnet test GXX.Core.Tests` **连跑 31 次**（默认 15 + `-v q/m/n/d` 各 3 + 强并行 4）
> 全部 `Passed! Failed: 0`；全解决方案 `dotnet build GXX.slnx` 亦复跑多次 0 error。
> **必要性与充分性对照实验**：仅去掉 `AllocRecordBuffer` 的 `FillChar` 一行 → 第 2 次即复现失败（§9.1c）。

> **基线漂移说明**：派发词写的基线是 `GXX.Core.Tests = 550 例全绿`，但**本工作树在本车道动任何代码前实测为 532 例**。
> 差别来自 `main` 在派发词写作之后、且本工作树基线分支（`603672af`，批次 J215）之前的移动——
> 我**没有碰过**任何既有测试文件，故新增失败为 0，符合"不得新增失败"的门禁要求。

**测试用例分布（按类）**

| 测试类 | 例数 | 侧重点 |
|---|---|---|
| `ParadoxDataSetLayoutTests` | 9 | 记录字节布局、Pack、固定数组、枚举宽度 |
| `ParadoxLangTableTests` | 4（含 12 行 Theory） | 118 项抽取正确性 + 1-based 不变量 |
| `ParadoxDataSetOpenTests` | 13（含 8 行 Theory） | 空名/缺文件/FileType/加密判定/.mb/字段构建 |
| `ParadoxDataSetLangTests` | 8 | `DetectLang` 双路径 + `Language` 往返 |
| `ParadoxDataSetCursorTests` | 14（含 4 行 Theory） | 游标/EOF/RecNo/除零/块越界/缓冲初始化不变量 |
| `ParadoxDataSetFieldDataTests` | 12（含 27 行 Theory） | 字段族逐分支 + 空值 + 逆序 + xor |
| `ParadoxDataSetBlobTests` | 6 | 行内/单块/索引块/无 .mb/负偏移 |
| `ParadoxDataSetEncodingTests` | 12 | `EncodingField`/`EncodingString`/接缝注入 |
| `ParadoxDataSetConsumerContractTests` | 9 | 两个真实消费者的调用面 + 接缝工具 |

**大段常量的脚本抽取 + 回读比对（硬性要求 4）**

* `Gen/extract-pxlangtable.ps1`：从 GBK 原文解析 `PxLangTable`（原文 520-638），带 **4 项硬校验**——
  ① 条目数必须 = 118；② 最后一条必须在原文第 638 行；③ 数组终止行必须 = 638；
  ④ **逐项校验"第 N 项来自第 520+N 行"**（把 Delphi 1-based 索引与物理行号绑定）。
  脚本**纯 ASCII**（台账 §8.1：PowerShell 5.1 按 ANSI 解析 `.ps1`；首版因含中文路径字面量直接解析失败）。
* `Gen/verify-pxlangtable.ps1`：把原文与生成物**双向重解析**并逐字段（Name/SortOrder/CodePage/SortOrderID）比对。
  实测：**`OK: 118/118 PxLangTable entries identical`**。
* 该抽取过程**抓出了一个真实缺陷**：首版脚本把空行当数组终止符，导致 118 项**全部错位**（索引 N 实际取到第 N+1 项附近的内容），
  4 项硬校验的加入正是为了不让这类静默错位再次发生。

---

## 6. 发现的原文缺陷 / 易错点（带 `文件:行`）

> 全部在 `ParadoxDataSet.cs` 文件头的 D1-D18 清单里逐条登记，并由差异断言锁定。

| ID | 位置 | 缺陷 | 影响 / 差异断言 |
|---|---|---|---|
| **D17** | `:797` + `:800-803` + `:805` | **字段名被读到 TableName 区之后**：`:797` 先把 P 加上"字段信息区之后"的尺寸，`:800-803` 又加 TableName 区尺寸（v7=261 / v3.5=79），`:805` 拿这个 **P** 当**字段名区起点**去 `Seek`。按 Paradox 布局，不含 TableName 尺寸的 `P0` 才是字段名区起点 | **读到 261/79 字节之后的名字**。`:799` 的注释 `// TableName size` 说明该 +261/+79 本是给表名用的，却被复用到字段名 Seek 上。实测：合成表必须把名字放到 `P0+261` 才读得出来 |
| **D18** | `:1281-1286` | `Loc := Blob.FileLoc and $FFFFFF00` 与 `Idx := Blob.FileLoc and $FF` **共用同一 32 位字段**：① 最高位置位时 `and $FFFFFF00` 得**负数**，`Seek(Loc+9)` 被喂负偏移；② 低 8 位被 Idx 占用 ⇒ **.mb 偏移必须是 256 的倍数**，否则偏移被索引覆盖 | ① 差异断言：`FileLoc=$800000FF` ⇒ `Loc=int.MinValue` ⇒ Seek 抛异常（原文无防护）；② 本轮实测踩到：偏移 `$80` ⇒ `FileLoc=$000000FF` ⇒ `Loc=0`，**静默读到文件头**（12 个 0 字节） |
| **D1** | `:809-816` / `:837-844` | `repeat/until` 只在 `B<>0` 时写入 ⇒ 文件在此处截断时**死循环**（Delphi 下 `Read(B,1)` 读不到不改变 B） | 未构造（需截断文件）；已注释登记 |
| **D3** | `:866` | `not (FileType in [0, 2])` ⇒ **FileType=1（.PX 主索引）也被拒** | 差异断言：FileType 1/3/4/5 全部抛 `is not .DB data file` |
| **D4** | `:973` | `AddDataSize div FFileHeader.RecordSize` ⇒ **RecordSize=0 除零** | 断言：`DivideByZeroException` |
| **D6/D7** | `:1203` / `:1207` / `:1216` | 空值判定只对 `[2..6, $14..$16]` 生效；数值字段按**逆序**读入；对 `P[FieldSize-1]`（= 文件首字节）异或 `$80` | 差异断言：记录 `[01 02 03 04]` ⇒ `0x81020304`（不是小端 `0x04030201`）；全 0 ⇒ `GetFieldData=False` |
| **D8** | `:1225-1246` | `case` 里**没有** `pxfMemoBLOB/pxfBLOB/pxfFmtMemoBLOB/pxfOLE/pxfGraphic/pxfBCD/pxfBytes` 分支（`:1237-1243` 仅以注释列出） | 断言：这 7 类 `GetFieldData` 返回 False |
| **D9** | `:1269` | `Header := Src + Field.Size - SizeOf(TPxBlob)`：`Field.Size < 10` 时**向前越界读** | 断言：`FieldSize=8` ⇒ 读到前一字段尾部；`Blob.Length > -2` 恒真 ⇒ 恒走 .mb 分支 |
| **D11** | `:1303-1305` | 索引条目按 **5 字节步长**（`5 * Idx`）定位，却按 `SizeOf(TPxBlobIdx)`=**6** 读结构 ⇒ `ModCnt` 被污染 | 断言：`TPxBlobIdx` 布局 = `Offset(0)/Len16(1)/ModCnt(2)/Len(4)`，Size=6 |
| **D12** | `:1346` | `case PxLangTable[FLanguageID].CodePage` **无 0 号保护**：`FLanguageID=0` 时越界 | 断言：经 `CreateBlobStream` + `EncodingMemo` 路径**可达**（`EncodingField` 的 `:1163` 守卫绕不过它），托管侧 `NullReferenceException` |
| **D13** | `:1349-1356` vs `:1171` | 比较常量 `'UTF-8'/'KOI8R'/'CP1251'` 与 `Create` 里 `GetCodepage`（Windows = `'CP' + GetACP`，如 `'CP936'`）**永不相等** | 断言：默认 `Codepage` 下 `EncodingString` 恒等返回 |
| **D14** | `:855` | `CreateFmt('TableName is not set', [])` **无占位符**（原文如此） | 断言：消息 = `ParadoxDataSet.EParadoxError: TableName is not set` |
| **D15** | `:1045` | `if (Value < 1) or (Value >= RecordCount + 1)` ⇒ `RecNo := RecordCount+1` 被拒，`:1025` 的 `InternalLast` 造的游标无法复现 | 断言：3 条表 `RecNo=4` 被静默忽略 |
| **D16** | `:1080` | `PxLangTable[I].Name = Value` 是**大小写敏感**短串比较，与 `:1066` 的 1..118 边界检查不对称 | 断言：`"parados china 936"` 不命中 |
| **D10** | `:1277` | `Blob.Length > Field.Size - SizeOf(TPxBlob)` 用同一表达式同时表达"长度"与"是否行内 Blob" | 已被 §Blob 三路径测试间接锁定 |
| **D19** | `:1005` + `:937-957` + `:1052` | `GetMem` **不清零**，而 gmPrior/gmNext 在 `FCursor <= 1` / `FCursor >= RecordCount` 时直接返回 `grBOF`/`grEOF`，**连 `RecordIndex` 都不写**；`:992-994` 的 else 只清零**用户记录区**（不含头部 6 字节）⇒ `GetRecNo` 读到**本次刚分配、尚未写过**的非托管内存 | **本轮最有价值的发现**，也是唯一造成**间歇性失败**的缺陷。实测值随进程堆复用而变（`166957392` / `-35615349` / `0x656C6F74`），约 1/6 概率；并行度越高越易显形。**根因三段互证见 §9**（已排除"stale pointer/use-after-free"假设）。Delphi 下 `GetMem` 同样不清零 ⇒ **原文在该路径本就没有确定值**（未定义行为）。处置：`AllocRecordBuffer` **分配即清零**（有意偏离，已登记） |
| **D20** | `:1052` | `GetRecNo` 在 `ActiveBuffer` 未分配（nil）时解引用空指针 | 原文从不查询"打开但未 First"的 `RecNo`，未触发；托管侧同样不额外保护 |

**其它易错点（不是原文缺陷，但会坑移植者）**

1. **`Pack=1` 表达不了 Delphi 的 `packed record`**（本车道最贵的坑）。
   CLR 只接受 `Pack ∈ {1,2,4,8,16,32,64,128}`；`Pack=1` 会让 `fixed byte[N]` 紧贴前一字段，
   对 `TPxFileHeader`/`TPxDataHeader` 这类带**显式偏移语义**的记录会造成字段整体前移。
   实测：`TPxDataHeader` 得 **32** 字节（应 40）、`TPxBlobIdx` 得 **5** 字节（应 6）。
   → 已全部改为 `LayoutKind.Explicit` + `FieldOffset` 逐字段钉死（注释给出原文行号）。
2. **`TPxDataHeader` 的 `$006C/$0072` 是文件绝对偏移**（该记录从 `$58` 开始），
   在本记录内的相对偏移应减去 `$58`（= `$14`/`$1A`）。首版直接照抄绝对偏移 ⇒ Size 变成 120。
3. **`TPxRecordHeader` 的枚举宽度**：Delphi 7 枚举最小占 **2** 字节 ⇒ `SizeOf = 4+2 = 6`；
   托管 `enum : int` 是 4 字节，直接 Marshal 会得 8，使原文 `:986/:992` 的
   "用户记录在缓冲内的起始偏移"**整体偏 2 字节**。已改为 `ushort` 承载 + `PxRecordHeaderOps` 访问器。
4. **`TDataSet.Eof` 不能写成自动属性**：`public bool Eof { get; protected set; }` 会生成**第二个后备字段**，
   而 `Next()`/`Resync()` 写的是 `FEof` ⇒ `Eof` 永远读不到（本车道实测：越过末记录后 `Eof` 恒为 false）。
   已改为 `public bool Eof => FEof;`。
5. **接缝漏传 `GetFieldData` 的 False**：数值分支若写成 `x ? value : 0`，会把"空值"变成 `0`
   （消费者 `DateSetToSqlite` 会写入 0 而非 NULL）。已全部改为 `if (!GetFieldData(...)) return null;`。

---

## 7. 接缝清单 + 需要集成方改白名单外文件的精确签名要求

### 7.1 需要集成方动手的（白名单外文件）

| # | 目标文件 | 需要的精确签名 / 动作 |
|---|---|---|
| 1 | `GXX.CSharp/src/GXX.RunGate/`（任意启动路径，如 `Program.cs` 或 `RunGateService.cs`） | `GXX.Core.Paradox.ParadoxConvSeam.GetCodepageHook = GXX.RunGate.ParadoxConv.GetCodepage;`<br>`GXX.Core.Paradox.ParadoxConvSeam.EncodingHook = (src, dst, s) => GXX.RunGate.ParadoxConv.Encoding((GXX.RunGate.TEncodingKind)(int)src, (GXX.RunGate.TEncodingKind)(int)dst, s);`<br>（`ParadoxConv` 是 `public static partial class`，`Encoding(TEncodingKind, TEncodingKind, string) : string` 与 `GetCodepage() : string` 均已 public；`TEncodingKind` 整数值两边一致） |
| 2 | `GXX.CSharp/src/GXX.GameCenter/GXX.GameCenter.csproj` **或** 把 `ParadoxConv` 下沉到共享工程 | `GXX.GameCenter` 目前**只引用 `GXX.Core`**，拿不到 `GXX.RunGate.ParadoxConv`。任选其一：<br>(a) 给 `GXX.GameCenter.csproj` 加 `<ProjectReference Include="..\GXX.RunGate\GXX.RunGate.csproj" />`（注意 `GXX.RunGate` 是 `WinExe`）；<br>(b) 建议：把 `ParadoxConv.cs` + `ParadoxConv.Tables.g.cs` 从 `GXX.RunGate` **下沉到 `GXX.Core`**（namespace 改 `GXX.Core.Paradox` 或 `GXX.Core.Crypto`），两工程各自引用即得；此时 `ParadoxConvSeam` 可直接内联调用、接缝可删。<br>**(b) 更符合"两个副本只移植一份"的原始意图**，但属跨工程搬迁，需集成方裁定 |
| 3 | `GXX.CSharp/src/GXX.M2Server/Forms/BDEToSqliteForm.cs:35`（`IGBDEDataSet`） | 该文件（`GBDEtoSqlite.pas` 的已完成移植）把 `TParadoxDataSet` 留成了接缝 `IGBDEDataSet`（`FieldCount/Fields/Eof/First/Next`），其 `Fields[].AsString/AsInteger/AsFloat` 与 `TParadoxDataSet` 的**字段视图语义等价**。建议由集成方加一个 ~25 行的适配器：<br>`IGBDEDataSet` ← `TParadoxDataSet`（`FieldCount = FieldCount`；`Fields[i].AsString = ds.Fields[i].AsString`；`Eof`；`First()`；`Next()`）。<br>**不要**在 `GXX.Core` 反向引用 `GXX.M2Server`（会形成循环）。**解析逻辑已在 `GXX.Core`，不要重写**。<br>注：`GBDEFieldType` 只有 `ftString/ftInteger/ftFloat`，而 `GBDEtoSqlite.pas:135` 原文对其它类型本就抛异常 —— 适配器保持该行为。 |
| 4 | `GXX.CSharp/src/GXX.RunGate/uFrmMagicCD.cs`（`uFrmMagicCD.pas` 的移植，若已存在） | 消费者面为 `TableName` / `Open` / `First` / `RecordCount` / `FieldByName(name).AsInteger` / `.AsString` / `Next` / `Close`，**全部已具备**，无需改动即可直接切到 `TParadoxDataSet` |

### 7.2 本车道已提供、**不需要**集成方再造的接缝

`TDataSet` / `TField` / `TFieldDef` / `TFieldDefs` / `TFieldType` / `TGetResult` / `TGetMode` /
`TBookmarkFlag` / `TBlobStreamMode` / 16 个 `T*Field` 子类 / `TFileStream` / `TMemoryStream` /
`EParadoxError` / `TEncodeEvent` / `ParadoxApplication` / `PxFileUtils` / `PxAnsi` / `PxDateTime` / `PxBuffer`
—— **全在 `namespace GXX.Core.Paradox`**。台账 §15 规程：后续车道若需要这些类型，
**引用即可，不要另造第二份**。

### 7.3 跨车道类型重名复核（台账 §14/§17 规程，开工与收尾各跑一次）

命令：`git grep -n -E "(class|struct|enum|interface|delegate) +(partial +)?<Name>\b" main -- 'GXX.CSharp/src/**/*.cs'`

| 类型 | main 上的命中 | 裁定 |
|---|---|---|
| `TMemoryStream` | `src/GXX.Client/GUI/Share/FStateSeams.cs:173`（`namespace GXX.Client.GUI.Share`，仅 `Buffer/Position/Size` 的占位） | **无冲突**：`namespace GXX.Core.Paradox` 是本车道新建（main 零命中）；且 main 上**没有任何文件同时** `using GXX.Client.GUI.Share;` 与 `using GXX.Core.Paradox;`（实测）⇒ 不会 CS0104。两者语义也不同。**已实测全解决方案构建 0 error 佐证** |
| `TEncodingKind` | `src/GXX.RunGate/ParadoxConv.cs:40` | **有意为之**的接缝对应物（整数值一致），不是重复定义 |
| 其余全部（`TDataSet`/`TField`/`TFieldType`/`TGetResult`/`TGetMode`/`TBookmarkFlag`/`TBlobStreamMode`/`TFieldDefs`/`TFieldDef`/`TFileStream`/`EParadoxError`/`TDataBlock`/`TPxLang` 等） | **零命中** | 允许新增 |

---

## 8. 诚实说明：未完成部分与剩余量

### 8.1 已完成（可交付）

* `TParadoxDataSet` **30 个方法 + 全属性 + 全部记录布局 + 118 项语言表**：1:1 移植完成，未覆盖行号仅
  3 处（`806-810` FPC 分支、`1327-1331` 原文注释代码块、`139-423` 注释文档），均为**不可执行或非 FPC 目标**。
* 121 例新测试，**654/654 全绿**（31 连跑，§9.5）；全解决方案 **0 error**。
* 大段常量脚本抽取 + 双向回读比对（118/118）。

### 8.2 未做（明确不做，理由充分）

| 项 | 理由 |
|---|---|
| `DB.pas` 完整数据集状态机（`dsInsert`/`dsEdit`/书签全族/`Filter`/`Filtered`/`OnFilterRecord`/`TDataSource`） | 原文 `ParadoxDataSet.pas` **一行都没用到**；只有 `:716-745` 的 `published` 属性**重声明**（无新逻辑）。移植它等于顺手移植整个 VCL DB 层，违反台账 §15 与"最小接缝"要求 |
| `TBlobStream` 类 | 原文 `:706` 返回 `TStream`（实为 `TMemoryStream`），**没有** `TBlobStream` |
| `Forms.pas` 的 `Application` 全族 | 只用到 `HandleException(Self)` 一处（`:767`），已做成可注入钩子 |
| `TDataSet` 的 `Eof`/`Bof` 与 Delphi 的**内部状态位**完全对齐 | 见 §6 易错点 4：原文 `TParadoxDataSet` **没有**覆盖 `Eof`；托管侧据 `GetRecord` 的 `grEOF` 返回值定义 `Eof`，**可观察行为等价**（`FieldByName` 取值为空、`RecNo` 不变、bookmark = `bfEOF`），但不是"同一个内部状态位"。已在 `TDataSet.Eof` XML 注释里写明 |

### 8.3 已知保真度妥协（3 处，均已在代码注释与本节登记）

1. **`TPxRecordHeader` 的 `BookmarkFlag` 用 `ushort` 承载**（而非枚举字段）。
   原因：Delphi 7 枚举最小 2 字节 vs 托管 `enum:int` 4 字节。语义经 `PxRecordHeaderOps` 完全一致，
   尺寸（6）与 Delphi 一致 ⇒ `:986/:992` 的偏移语义**正确**。若强行用枚举字段，偏移会偏 2 字节（更糟）。
2. **`InternalClose` 里的 `BindFields(False)` / `DestroyFields` 上移到 `TDataSet.Close`**。
   原顺序：`Close` → `InternalClose` → 内部先 `BindFields(False)`、再 `DestroyFields`、再关流。
   托管顺序：先 `BindFields(False)` + `DestroyFields`，**再** `InternalClose`。差别在"字段析构 vs 文件关闭"的先后，
   对原文可观察行为无影响（两者都不互相引用）。已在代码注释标注 `// 接缝`。
3. **`DelphiFormat` / 异常消息前缀**：`Exception.CreateFmt` 在 Delphi 下会拼
   `<单元>.<类名>: <消息>`（类名取自 RTTI/`GetEnumName`）。托管侧显式用
   `PxDelphiNaming.ClassNameOf` 复刻为 `ParadoxDataSet.EParadoxError: ...`。
   若集成方认为实际 Delphi 版本给出的是短名 `EParadoxError`，**只需改 `PxDelphiNaming.ClassNameOf` 一处**（1 行）。
4. **`AllocRecordBuffer` 显式清零**（D19）。原文用不清零的 `GetMem`，其 `grBOF`/`grEOF`
   分支不写 `RecordIndex`，因此**原文在该路径下读的是未初始化内存**。托管侧清零使行为可重复，
   代价是"与原文的具体垃圾值不同"——但原文那个值本就不可复现，故不视为保真度损失。
   若集成方坚持逐字节照抄该不确定性，删掉 `AllocRecordBuffer` 里的 `PxBuffer.FillChar` 一行即可
   （但门禁会重新变成偶发红）。

### 8.4 剩余量（估算）

* 若要**完整**覆盖 `DB.pas`/`Classes.pas` 的 VCL 数据集契约：这是**独立于本单元**的巨型工程
  （`DB.pas` 本体 2000+ 行 + `TCustomDataSet` 状态机），建议**不派发**，除非有消费者真的需要 `Filter`/书签/编辑。
* 若采纳 §7.1 第 2 条的方案 (b)（`ParadoxConv` 下沉到 `GXX.Core`）：工作量约 10 行（改 namespace + 删接缝），
  属集成方裁定范围。

### 8.5 环境备注

* 并发车道导致的 `MSBUILD MSB4166` 本轮**未出现**；`dotnet build`/`dotnet test` 各跑多次均稳定。
* 曾遇到一次"`dotnet test` 报 24 例失败、但实际是我新增测试的期望值算错"——按规程回读原文后确认
  **是测试期望写错（不是实现写错）**，已逐条修正（其中"逆序读 + xor"的字节置换我一开始推导反了，
  原文 `:1207` 是 `P[I] := Src[FieldSize-I-1]`、`:1216` 再对 `P[FieldSize-1]` 异或，
  等价于 `result = (rec[0]^$80)<<24 | rec[1]<<16 | rec[2]<<8 | rec[3]`）。
* **一次真实的间歇性失败（值得记入台账）**：门禁曾**偶发** 1 例失败（6 次里 1 次），
  报 `EmptyTable_First_IsEof_NoRecords: Expected 0, Actual 166957392`。
  这**不是**测试期望写错，而是 §6 的 **D19**（原文未初始化内存）在托管侧的暴露。
  规程价值：**"偶发失败"必须连跑到复现并定位根因，不能当环境抖动重跑放过** ——
  本例若按"重跑即可"处理，就会把一个真实的原文缺陷留到集成后才炸。

---

## 9. D19 根因复核（集成方复核后回填，2026-09）

> 背景：集成方派发 `p6-core-hardinfo` 复核时，**在它自己的干净基线上又复现了同一条 flaky**
> （4 次里 2 次），并给出**相反假设**："根因很可能不是 D19，而像 `ActiveBuffer` 指向了
> 已被释放或从未属于本次 `First()` 的内存（use-after-free / 陈旧指针）"，依据是
> 实测值"像文本"（`0x656C6F74` = `74 6F 6C 65` = `"tole"`）。
> 本节用**三段互证**给出最终结论：**集成方的假设可以排除，根因就是 D19（use-of-uninitialized-memory）**。

### 9.1 证据链

| # | 实验 | 结果 | 结论 |
|---|---|---|---|
| (a) | 列出 `FActiveBuffer` 的**全部**赋值/释放点 | 赋值仅 2 处（`TDataSet.Next()` 与 `Resync()`，都是 `if (FActiveBuffer == IntPtr.Zero) FActiveBuffer = AllocRecordBuffer();`）；清零点仅 1 处（`TDataSet.Close()` 置 `IntPtr.Zero`） | 空表用例从头到尾**只分配一次**，`FActiveBuffer` 无第二个指针源 ⇒ **不存在**"上一实例遗留"或"已释放"的指针 |
| (b) | 在 grEOF 路径现场打印 `ActiveBuffer` 的原始 6 字节 | `00-00-00-00-02-00`（`RecordIndex=0`、`BookmarkFlagRaw=bfEOF=2`） | 缓冲**内容与布局都正确**，没有被覆写 ⇒ 不是"读到别人的缓冲" |
| (c) | **因果实验**：仅注释掉 `AllocRecordBuffer` 里的 `PxBuffer.FillChar(p, size, 0)`，跑全量测试 | **第 2 次即复现** `Expected: 0, Actual: -35615349`；恢复 `FillChar` 后**连续 31 次全绿** | 该行**既是充分条件也是必要条件** ⇒ 根因锁定为"分配后未初始化" |

### 9.2 为什么值"像文本"、为什么与并行度相关（假设被证伪的机制解释）

`AllocHGlobal`/`FreeHGlobal` 背后是**进程堆**：同一进程内先前的分配被释放后，同样的尺寸会**常数级复用**同一批块。
`GXX.Core.Tests` 里有大量"含可打印字节"的短命分配——例如 `PxSyntheticDb` 反复构造 `byte[]`、
用 `PxAnsi` 把 GBK 字段名（`MagName`/`Descr`/`MagId`）搬进/搬出托管数组、`FromBytes/ToBytes` 的 `char[]`/`byte[]` 往返。
于是那 4 字节**恰好可读成 ASCII** 是复用巧合，而不是"另一个数据集实例的记录文本"。

这与并行度相关的原因：**唯一让该缺陷显形的随机源就是堆状态**——并行度越高/越杂，复用模式越随机，
"恰好非 0"的概率越高；而 `-v n`（更慢、调度更松散）跑多次全绿，只是**碰巧每次复用到的块都是 0**
（例如复用了一块刚被 `Array.Clear` 过的数组），**不是"没有缺陷"**。
⇒ 复核发现的"值每次不同"与"与并行度相关"**恰恰是未初始化内存的典型指纹**，与 stale pointer 无关。

### 9.3 修复方式与"为什么不只兜底 RecNo"

保留**分配即清零**（`AllocRecordBuffer` 里 `PxBuffer.FillChar(p, size, 0)`），理由：

1. 它修的是**对象本身**（"新分配的记录缓冲"这一不变量：**返回时整块已定义**），
   而不是某一个读取点。同一块头部的另外 5 字节（`BookmarkFlag` +4..+5）与
   `InternalSetToRecord`(:1030) 读的是**同一个头部** —— 只把 `GetRecNo` 包一层 `if` 返回 0
   会漏掉它们，正是"掩盖症状"。
2. 语义上等价于 Delphi 下"恰好拿到清零内存"的那一支，**不改变任何"会写入"的路径**
   （`:961` 写 `RecordIndex`、`:994` 写 `bfEOF` 全部照旧），只是把原文的未定义值**固定**为一个确定值。
3. 与原文的偏离点已在 `ParadoxDataSet.cs` 文件头 **D19** 条目与 `AllocRecordBuffer` 的 XML 注释里写清
   （含 (a)(b)(c) 三段证据），并在报告本节登记。

### 9.4 加固后的测试（把不变量写进断言）

`EmptyTable_First_IsEof_NoRecords` 从"只断 `RecNo == 0`"加固为**断言整块头部**：
`PxBuffer.ToArray(ds.ActiveBuffer, PxRecordHeaderOps.Size)` 必须等于
`{ 00 00 00 00 02 00 }`（`RecordIndex=0` + `BookmarkFlag=bfEOF`）。

并**新增一条独立证据用例** `AllocRecordBuffer_FreshBufferIsFullyDefined_NotRecycledGarbage`：
先建一个含**可打印文本** `"tole"` 记录的数据集并**释放**（故意在堆里留下 ASCII 残留，
复现集成方看到的 `0x656C6F74` 来源），再建空表走 grEOF，断言 `RecNo == 0` 且
`NotEqual(0x656C6F74, ...)` —— **直接对"残留复用"这一根因机制下断言**，不依赖 `RecNo` 的单一读点。

### 9.5 稳定性证明（本次要求的 10 连跑，实际做了 31 次）

| 配置 | 次数 | 结果 |
|---|---|---|
| 默认 verbosity | **15** | 15/15 全绿 |
| `-v q` / `-v m` / `-v n` / `-v d` | 各 3 = **12** | 12/12 全绿（含集成方称"多次全绿"的 `-v n`） |
| `xUnit.MaxParallelThreads=16`（强制高并行） | **4** | 4/4 全绿 |
| **合计** | **31** | **31/31 全绿，0 失败** |

用例总数 **654**（比上一版 653 多 1，即 9.4 新增的独立证据用例）。
对照实验（9.1c）：同一份代码**仅去掉那一行 `FillChar`** → 第 2 次即失败。
⇒ "修复有效"与"修复必要"两侧都有实测支撑。

### 9.6 ⚠ 复核期间发现的工作树事故（与本 flaky 无关，但必须上报）

在本次复核开始时实测发现：**本工作树的 `GXX.Core` 目录下 64 个已跟踪文件全部从磁盘上消失**
（`git status` 显示 64 条 ` D`），覆盖 `Async/ Compress/ Crypto/ Data/ IO/ Launcher/ Protocol/ Rtl/ Stubs/ Util/`
**以及本车道的 `Paradox/**` 全部 7 个文件**。根目录只剩 `CommonConst.g.cs / EncodingInit.cs / Share.cs / UpdateCommon.cs`。
`GXX.Core.csproj` 本身还在，所以项目仍在、但**只剩 4 个 .cs**。

* **性质**：这是"把文件挪出树再挪回来"式实验（`p6-core-hardinfo` 称移出了 7 个文件）的**误伤**——
  移动的范围远超其声明的 7 个文件，把整个 `GXX.Core` 子树都移走了。
* **处置**：本车道用 `git checkout HEAD -- GXX.CSharp/src/GXX.Core` **完整恢复**（全部来自 HEAD 提交，
  无任何内容损失），`git status` 随即归零、`cs` 文件数 69。
* **给集成方/调度方的提醒**：
  1. **任何"移出文件做干净基线"的实验，都必须在移出前后各跑一次 `git status --porcelain | Measure-Object` 并比对数量**，
     否则"移出 7 个"和"移出 71 个"在报告里长得一模一样。
  2. **恢复必须按路径白名单 `git checkout HEAD -- <自己的独占区>`**，不要用 `git checkout .` 或 `git restore .`
     —— 那会把**别的车道未提交的在飞产出**一起静默还原，是本工程 §13.2 那类事故的翻版。
  3. 车道报告里的"全绿"结论**必须绑定 `git status` 为空**；否则可能是在"源码已被移走、用的是陈旧 DLL"
     的树上得出的（本车道这次即无法排除该风险，故恢复后**全部重跑**）。

