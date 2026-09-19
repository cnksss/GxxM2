# 并行报告 · p2b-m2-plugins（M2Server 插件层）

> 车道：`par/p2b-m2-plugins` ｜ 工作树：`.worktrees/p2b-m2-plugins`
> 源单元：`Source/M2Engine/PluginInterface.pas`（**实测 3,304 行**）、`Source/M2Engine/PluginImplement.pas`（**实测 6,321 行**）
> 目标：`GXX.CSharp/src/GXX.M2Server/Plugins/**`（新建，独占区）
> 日期：本轮（P2b）｜ 基线：`main`

> ⚠ 任务书给的行数（2,484 / 5,472）与仓库实测（3,304 / 6,321）不符，以**仓库实测**为准。

---

## 1. 分支与提交

| 项 | 值 |
|---|---|
| 分支 | `par/p2b-m2-plugins` |
| 起始基线 | `main`（本工作树创建时的 HEAD） |

提交（按切片）：

| # | commit | 内容 |
|---|---|---|
| 1 | `38920a6d` | ABI 层：`PluginInterfaceTypes.g.cs` + `PluginInterfaceTables.g.cs`（736 委托 / 21 个 Pack=1 记录） |
| 2 | `6b213c6d` | 接缝层 + 托管接口层：`PluginInterfaceSeams.cs` + `PluginInterfaceManaged.g.cs` |
| 3 | `9c92e4a5` | 宿主实现：`PluginHostRuntime.cs` + `PluginInterfaceHost.cs` + `PluginInterfaceHost.Stubs.g.cs` |
| 4 | `d145652b` | 装载层：`PluginAssemblyLoader.cs` |
| 5 | `37a3ba02` | 测试 + 抽取清单/生成器（`Plugins*Tests.cs`、`PluginsData/**`） |
| 6 | `05cba4ca` | 本报告 |
| 7 | `（本提交）` | 报告补 commit hash |

---

## 2. 新增文件清单

### 2.1 源码（`src/GXX.M2Server/Plugins/`，全部新建，未改任何既有 `src/**`）

| 文件 | 行数 | 说明 |
|---|---|---|
| `PluginInterfaceTypes.g.cs` | 2,980 | **脚本抽取生成**：736 个 procedural type → `[UnmanagedFunctionPointer(StdCall)]` 委托；另含手工补齐的 `TNotifyEventEx` / `TM2Engine_GetOtherFileDir` / `TBaseObject_TrainSkill` |
| `PluginInterfaceTables.g.cs` | 953 | **脚本抽取生成**：21 个 `[StructLayout(Sequential, Pack=1)]` 记录（`TScriptCmdParam` + 19 个 `T*Func` + `TAppFuncDef`），字段名/顺序/`Reserved` 逐字对齐 |
| `PluginInterfaceManaged.g.cs` | 848 | **脚本抽取生成**：19 个 `I*Func` 托管接口（与 `T*Func` 字段一一对应）+ `Reserved` 槽位属性 |
| `PluginInterfaceSeams.cs` | 299 | 接缝：14 个 `_T*` 别名接口（`IListHandle`/`IStringListHandle`/`IMenuItem`/`IIniFileHandle`… ）+ `TUserMagic`/`TMagic`/`TDynamicVar`/`TMasterRankInfo` 占位 |
| `PluginHostRuntime.cs` | 275 | ABI 运行时支撑：`TListHandle`/`TStringListHandle`/`TMemoryStreamHandle`/`TIniFileHandle` + `PluginHostText`（`Dest: PAnsiChar; var DestLen` 语义集中实现） |
| `PluginInterfaceHost.cs` | 1,195 | **宿主实现**：原文 741 个 `_T*` 例程中 236 个的 1:1 移植（内存/列表/字符串列表/内存流/菜单/INI/引擎薄封装/BaseObject 字段访问器/Env 只读属性等） |
| `PluginInterfaceHost.Stubs.g.cs` | 3,049 | **脚本提取生成**：其余 500 个例程的显式未完成壳（签名与原文字节一致，方法体抛 `NotImplementedException` 并在消息里带原文行号） |
| `PluginAssemblyLoader.cs` | 380 | 装载层：`PlugList.txt` 解析 + `LoadLibrary/GetProcAddress` 原生路径 + `AssemblyLoadContext` 托管路径 + `UnInit/FreeLibrary/Unload` |

### 2.2 测试（`tests/GXX.M2Server.Tests/`）

| 文件 | 用例 | 说明 |
|---|---|---|
| `PluginsInterfaceManifestTests.cs` | 9 | **接口面齐全性**（本车道最重要的测试）：声明集合 vs 脚本抽取清单逐条一致 |
| `PluginsHostRuntimeTests.cs` | 41 | 宿主实现的边界行为（缓冲语义/空实现/PlugID error/往返） |
| `PluginsAssemblyLoaderTests.cs` | 16 | 装载路径 + 21 个记录的 `SizeOf` 布局断言 |
| `PluginsData/*.tsv` | — | 脚本抽取的机器可读清单（测试回读比对的真源） |
| `PluginsData/gen/*.ps1` | — | 生成器（可复现，见 §7） |

> **未改**：`GXX.slnx`、任何 `*.csproj`、`Directory.Build.props`、`docs/并行*.md`、`docs/Checklist.md`、`tools/**`、任何既有 `src/**`。

---

## 3. 两个单元的全量清单与覆盖

### 3.1 `PluginInterface.pas`（3,304 行）

| 区间 | 内容 | 状态 |
|---|---|---|
| 1–43 | 单元头注释（接口约定 1.1→1.5 变更记录）+ `uses` | ✅ 已覆盖（版本/约定写入 `PluginInterfaceTypes.g.cs` 头注释） |
| 44–73 | **14 个 `_T*` 类型别名**（`_TList = TList` … `_TGuild = TGuild`） | ✅ 已映射为接缝接口（`PluginInterfaceSeams.cs`） |
| 75–111 | `PScriptCmdParam` + `TScriptCmdParam`（**34 个字段**，原文 :111 把 `nParam10` 与 `end;` 写在同一行） | ✅ 1:1（`Pack=1`） |
| 113–2341 | **736 个 procedural type**（`stdcall` 函数指针） | ✅ 736/736（`PluginInterfaceTypes.g.cs`） |
| 2342–3184 | **21 个记录**：`TMemoryFunc`…`TAppFuncDef`（含各 `Reserved` 数组） | ✅ 21/21（`PluginInterfaceTables.g.cs`） |
| 3186–3301 | 宿主导出函数说明（**注释块内 42 个 `Init/UnInit/Hook*` 签名**） | ✅ 逐条登记（测试 `ExportSurface_MatchesOriginalCommentedExportList`） |
| 3302–3304 | `implementation … end.`（**空实现段**） | ✅ 照抄：本单元无代码，托管侧无需产物 |

### 3.2 `PluginImplement.pas`（6,321 行）

| 区间 | 内容 | 状态 |
|---|---|---|
| 1–39 | 单元头 + `uses` | ✅ |
| 40–46 | `PNotifyEventMethod` / `TNotifyEventMethod`（`Click: TNotifyEventEx; Sender: TObject`） | ✅（`PluginMenuNotify` + `TNotifyEventMethod`） |
| 48–857 | **741 个 `_T*` 例程的前置声明**（interface 段） | ✅ 由实现段覆盖 |
| 858–865 | `implementation` + `uses svMain/M2Share/DesUtils/PluginManager` | ✅ 证据（装载路径判读见 §4.4） |
| 870–1131 | TMemory / TList / TStrList（子集） | ✅ 全部 1:1 移植 |
| 1133–1153 | `_TStrList_Exchange/LoadFromFile/SaveToFile/CopyTo` | ✅ |
| 1159–1222 | TMemStream 全族（13 个） | ✅ |
| 1228–1501 | TMenu 全族（26 个） | ✅ |
| 1506–1565 | TIniFile 全族（10 个） | ✅ |
| 1573–1790 | TMapManager / TEnvirnoment | ✅（含 :1750/:1757 两处**原文空实现**照抄为恒 False） |
| 1792–2135 | M2 引擎相关 26 个 | ✅ 9 个完整移植；`ZLib*`/`Encrypt*`/`DecryptPassword` 4 个为**具名接缝**（待 ZlibEx / EncryptUnit_LF 上移 GXX.Core） |
| 2142–3320 | TBaseObject（185 个） | ⚠ 70 个纯字段访问器 + 属性访问器已移植；其余 145 个为显式未完成壳 |
| 3321–4542 | TSmartObject（103 个） | ⚠ 全部为显式未完成壳（接缝到 `ISmartObjectHandle`） |
| 4543–5133 | TPlayObject（159 个） | ⚠ 全部未完成壳（+3 个原文缺陷例程已按实现体签名登记） |
| 5134–5244 | TDummyObject（3 个） | ⚠ 未完成壳 |
| 5245–5461 | THeroObject（33 个） | ⚠ 未完成壳 |
| 5462–5614 | TNormNpc（18 个） | ⚠ 未完成壳 |
| 5615–6060 | TUserEngine（53 个） | ⚠ 未完成壳 |
| 6060–6321 | TGuildManager（4 个）+ TGuild（31 个） | ⚠ 未完成壳 |

**覆盖率（严格口径）**

| 口径 | 数量 |
|---|---|
| `PluginInterface.pas` procedural type | **736 / 736 = 100%** |
| `PluginInterface.pas` 记录 | **21 / 21 = 100%** |
| `PluginImplement.pas` 例程**签名面**（方法名 + 参数 + 返回） | **741 / 741 = 100%**（可直接反射核对） |
| `PluginImplement.pas` 例程**实现体**（去掉 throw 的真实逻辑） | **241 / 741 = 32.5%**（236 个手工 1:1 + 3 个缺陷例程 + 2 个笔误例程） |
| 例程**行为未移植**（显式未完成壳） | **500 / 741 = 67.5%** |

> 说明：交付物能在无引擎单元的前提下编译、且接口面零遗漏；实现体缺口全部集中在"必须访问
> `ObjBase/ObjPlayer/ObjSmartMon/Envir/Guild/UsrEngn` 私有状态"的函数上（见 §4.5 接缝清单）。

---

## 4. 接口面清单 + 托管侧映射

### 4.1 原始插件形态：**原生 DLL 导出表**（不是 COM）

| 证据 | 行号 | 内容 |
|---|---|---|
| 装载 | `PluginManager.pas:2310` | `Moudle := LoadLibrary(PChar(sFileName));` |
| 取符号 | `PluginManager.pas:2313` | `DoInit := GetProcAddress(Moudle, 'Init');` |
| 同上 | `PluginManager.pas:2398-2401` | `LoadPlugin` 里同一套 `LoadLibrary` + `GetProcAddress('Init')` |
| 内存模块 | `PluginManager.pas:1853 / 1856` | `MemMoudle := MemoryLoadLibrary(RS.Memory)` / `MemoryGetProcAddress(MemMoudle, 'Init')` |
| 各 Hook | `PluginManager.pas:2017-2018 / 2050 / 2082-2085 / 2116-2117 / 2149-2152` | 逐个 `MemoryGetProcAddress(Plugin.FModule, 'HookXXX')` |
| 卸载 | `PluginManager.pas:461-465 / 473-478` | `UnInit := GetProcAddress(FModule,'UnInit')` → `FreeLibrary` / `MemoryFreeLibrary` |
| 接口文件本身 | `PluginInterface.pas:3190-3301` | 注释块列出全部导出函数签名（**42 个**，见 4.2） |
| 内存模块备注 | `PluginManager.pas:15` | 注释：`MemoryModule` 国产改动版"时灵时不灵，还会崩溃"，已换 GitHub 原版 `MemoryModuleEx` |

**结论**：插件契约 = **C 风格 `stdcall` 函数指针表**（宿主把 `TAppFuncDef` 填充后交给插件的 `Init`，插件按需回填 `TAppFuncDef` 里的 741 个回调槽位）。不是 COM，没有 `IUnknown`/`QueryInterface`，没有注册表注册。

`PluginImplement.pas` 自身**没有 `exports` 段**（全文件 0 处）——它是**宿主侧**实现，由 `M2Server.dpr:75` 编进本体，再由 `PluginManager.pas:2327` 的 `{$I PluginFuncAssign.inc}` 把 741 个 `_T*` 逐个赋给 `TAppFuncDef` 的字段。

### 4.2 导出函数面（宿主 ← 插件方向，42 个）

`Init`、`UnInit`、`HookGetIPLocal`、`HookEngineReadyToStart`、`HookEngineStartComplete`、`HookEngineReloadComplete`、`HookLoadScriptFile`、`HookDecryptScriptFile`、`HookDecryptScriptLine`、`HookNpcLoadConditionCmd`、`HookNpcConditionProcess`、`HookNpcLoadActionCmd`、`HookNpcActionProcess`、`HookUserSelect`、`HookUserCommand`、`HookGetVariableText`、`HookBaseObjectCreate`、`HookBaseObjectRecalAbilBegin`、`HookBaseObjectRecalAbilEnd`、`HookBaseObjectRun`、`HookBaseObjectProcessMsg`、`HookBaseObjectStruck`、`HookBaseObjectMagicStruck`、`HookBaseObjectAttack`、`HookBaseObjectMagicAttack`、`HookBaseObjectDie`、`HookBaseObjectMakeGhost`、`HookBaseObjectFree`、`HookPlayerCreate`、`HookPlayerLogin1..4`、`HookPlayerRun`、`HookPlayerViewRangeNewObject`、`HookPlayerProcessMsgBegin`、`HookPlayerProcessMsgEnd`、`HookPlayerFree`、`HookDummyObjectRunBegin`、`HookDummyObjectRunEnd`、`HookHeroObjectCreate`、`HookHeroObjectFree`。

（`PluginInterface.pas:3190-3301` 的注释块；测试 `PluginsAssemblyLoaderTests.ExportSurface_MatchesOriginalCommentedExportList` 逐条登记。）

### 4.3 回调表接口面（宿主 → 插件方向，736 条）

| 分组 | 条数 | 分组 | 条数 |
|---|---|---|---|
| `TMemory_*` | 3 | `TMapManager_*` | 2 |
| `TList_*` | 13 | `TEnvir_*` | 25 |
| `TStrList_*` | 24 | `TM2Engine_*` | 26 |
| `TStrLit_*`（原文笔误） | 2 | `TBaseObject_*`（含 `TBaseobject_*` 1 条） | 185 |
| `TMemStream_*` | 13 | `TSmartObject_*` | 103 |
| `TMenu_*` | 26 | `TPlayObject_*`（含 `TPlayObejct_*` 1 条） | 159 |
| `TIniFile_*` | 10 | `TDummyObject_*` | 3 |
| `TMagicACList_*` | 3 | `THeroObject_*` | 33 |
| | | `TNormNpc_*` | 18 |
| | | `TUserEngine_*` | 53 |
| | | `TGuildManager_*` | 4 |
| | | `TGuild_*` | 31 |

合计 **736**，逐条名字 + 原文行号见 `tests/GXX.M2Server.Tests/PluginsData/PluginInterface.manifest.tsv`，
测试 `ProceduralTypes_MatchExtractedManifest_NameByLineByLine` 断言：

1. 反射取出的委托集合（`GXX.M2Server.Plugins` 命名空间）与清单**名字集合完全相等**；
2. 每个委托的 XML 注释里必须出现 `PluginInterface.pas:<清单行号>`；
3. 全部 736 条必须是 `CallingConvention.StdCall`；
4. 6 组关键签名的形参类型逐项匹配（`BOOL→int`、`PAnsiChar→byte[]`、`var DestLen→ref uint`、`pT*→ref T*`、`NativeInt→IntPtr`）。

### 4.4 托管侧映射方案

```
原生语义                                →  托管方案
────────────────────────────────────────────────────────────────────────────
LoadLibrary(dll)                        →  AssemblyLoadContext.LoadFromAssemblyPath（每插件一个 ALC，collectible）
GetProcAddress(m,'Init')                →  IM2ServerPlugin.Init(...)（托管接口）；原生 DLL 仍走 LoadLibraryA/GetProcAddress 兼容
GetProcAddress(m,'UnInit')              →  IM2ServerPlugin.UnInit()（托管）；原生走 GetProcAddress
MemoryLoadLibrary(Memory)               →  ❌ 无法等价（见 §4.5）
FreeLibrary / MemoryFreeLibrary         →  AssemblyLoadContext.Unload()
stdcall 函数指针 T*                     →  [UnmanagedFunctionPointer(StdCall)] 委托 + I*Func 托管接口（两套并列）
packed record TAppFuncDef               →  [StructLayout(Sequential, Pack=1)] struct（24024 字节 @x64，含 Reserved[1000]）
Dest: PAnsiChar; var DestLen: DWORD     →  byte[] + ref uint（PluginHostText 集中实现原文 5 处同构语义）
TMethod（方法指针 + 数据指针）           →  ❌ 无法等价 → PluginMenuNotify 直接持有回调
REPEAT 0..3 + VMProtect/内联 hook        →  不适用（无对应实现）
PlugList.txt 逐行解析                    →  PluginAssemblyLoader.ParsePlugList（逐字复刻 :2299-2303）
PlugID 反查（NativeInt(TempPlug)=PlugID）→  PluginId 句柄 + IPluginHostEnv.PluginExists
```

### 4.5 无法等价的原生语义（逐条）

| 原生语义 | 位置 | 托管侧处置 |
|---|---|---|
| `MemoryLoadLibrary` / `MemoryGetProcAddress`（从内存解析 PE、手工重定位/导入表） | `PluginManager.pas:1853/1856`、`MemoryModuleEx.pas` | **无法等价**。托管侧只能从文件系统/字节流转 `Assembly`；已用 `IsManagedAssembly`（PE CLI 目录项）分流，原生 DLL 仍走 `LoadLibrary` |
| **内联汇编 / VEH / 反调试 hook** | `Client.dpr`、`CheckProcessModules`（不在本单元） | 本单元无内联汇编，无需处置 |
| `TMethod`（`Method.Code`+`Method.Data`，`record` 直接强转 `TNotifyEvent`） | `PluginImplement.pas:1330-1333`、`:1389-1392` | **无法等价**：委托不能携带数据指针 → `PluginMenuNotify`（持 `Click`+`Sender`）；`NotifyEventEx` 直接回调 |
| 指针回写（`Dest: PAnsiChar` 写入调用方缓冲、`var BindValue: Byte`） | `:1028`、`:1535`、`:2127` | 部分等价：`byte[]` 就地写 + `ref` 数值；**跨进程/跨 DLL 的原始指针写回在托管侧无法表达**，故保留 ABI 层（可 P/Invoke） |
| `AllocMem`/`FreeMem`/`ReallocMem` 手工堆 | `:875/884/893` | `AllocHGlobal`/`FreeHGlobal`/`ReAllocHGlobal`；**`_TMemory_Realloc` 原文就不回写新指针**（参数非 `var P`），托管侧照抄该缺陷 |
| 插件崩溃隔离（原生 `except` 只捕 Delphi 异常，AV 直接带崩进程） | `:876-878` 等 | 托管侧异常可捕，但 `AccessViolation` 默认杀进程 → 用 ALC 卸载 + try/catch，**不等价于原生的进程级隔离** |
| `g_PluginManager.Items[I]` 的 `NativeInt(TempPlug)` 指针相等判定 | `:1309/1368` | 换为句柄表；**不是同一语义**（原版靠对象地址，托管对象会移动） |

---

## 5. 测试与门禁结果

```
dotnet build GXX.slnx -c Debug --nologo           → 0 error（90 warning，全部为既有工程的警告，无本车道新增）
dotnet test tests/GXX.M2Server.Tests/... -c Debug → 通过 5119 / 失败 0 / 跳过 0
   其中本车道新增 66 例（PluginsInterfaceManifestTests 9 + PluginsHostRuntimeTests 41 + PluginsAssemblyLoaderTests 16）
```

基线 5053 例 → 本轮 5119 例 = **+66 例**，无回归。

关键断言摘录：

- `ProceduralTypes_MatchExtractedManifest_NameByLineByLine`：736 条委托名集合、条数、**每条原文行号**逐条相等；
- `HostCallbacks_CoverAll741RoutinesInPluginImplement`：741 个例程名（734 + 7 个原文缺陷例程）在 `PluginInterfaceHost` 上**全部有对应方法**；
- `AllFunctionTables_FieldListsMatchExtractedInventory`：21 个记录的字段名/顺序与抽取清单完全一致；
- `TAppFuncDef_LayoutMatchesOriginal`：21 字段 + `Reserved[1000]` + `Pack=1`（24024 字节 @x64）；
- `TStrList_GetText_BufferBoundaryFollowsOriginal`：`Dest=nil` / `DestLen==Len` / `DestLen==Len+1` 三个边界，验证原文 `DestLen > Length(S)` 与"无论如何都改写 DestLen"；
- `TIniFile_ReadString_EmptyValueNeverWrites`：验证原文 :1533 比其它 Get* 多出的 `Length(S) > 0`；
- `TEnvir_GetMapParam_IsAnOriginalEmptyStub`：验证原文 :1750/:1757 的**空实现照抄**（未补全）；
- `TM2Engine_MainOutMessage_IgnoresIsAddTime`：验证原文 :1904 忽略 `IsAddTime` 恒传 True。

---

## 6. 发现的原文缺陷 / 易错点

| # | 缺陷 | 位置 | 处置 |
|---|---|---|---|
| 1 | **拼写不一致**：procedural type 名写作 `TStrLit_LoadFromFile`（Lit），实现体写作 `_TStrList_LoadFromFile`（List） | `PluginInterface.pas:255/258` vs `PluginImplement.pas:1139/1145` | 类型名**原样保留** `TStrLit_*`；实现体按原文名 `TStrList_*`；两者都在，测试断言"两个名字并存、且 `TStrLit_*` 是未完成壳" |
| 2 | **类型声明缺失 ×2**：`TM2Engine_GetOtherFileDir`（被 :2510 的记录字段与 `PluginFuncAssign.inc:165` 使用）、`TBaseObject_TrainSkill`（被 :2697 与 `PluginFuncAssign.inc:409` 使用），但 `PluginInterface.pas` 没有对应 procedural type | `PluginImplement.pas:1852` / `:3197` | 按**实现体签名**补齐委托（`PluginInterfaceTypes.g.cs` 末尾，注释标注"原文缺失类型声明"） |
| 3 | **7 个例程只在实现里存在**：`_TStrList_LoadFromFile`、`_TStrList_SaveToFile`、`_TM2Engine_GetOtherFileDir`、`_TBaseObject_TrainSkill`、`_TPlayObject_GetAlcohol`、`_TPlayObject_GetHeroM2ShopList`、`_TPlayObject_GetHeroM2ShopOpenList` | `PluginImplement.pas` 各处 | 全部登记（`PluginsData/PluginImplement.extra.tsv`），并逐个移植/接缝；其中后 3 个**未被 `PluginFuncAssign.inc` 引用**（死代码） |
| 4 | `_TM2Engine_MainOutMessage` 忽略入参 `IsAddTime`，恒传 `True` | `:1904` | 照抄 + 测试锁定 |
| 5 | `_TEnvir_GetMapParam` / `_TEnvir_GetMapParamValue` 是**空实现**（只有 `Result := False`） | `:1750-1753` / `:1757-1760` | 照抄为恒 0，**不补全** + 测试锁定 |
| 6 | `_TMemory_Realloc(P, Size)` 参数不是 `var P`，realloc 搬家后新指针丢失 | `:890` | 照抄签名 + 注释标注该原生缺陷 |
| 7 | `TMenu_Add/Insert` 里 `Item := TMenuItem.Create(MenuItem)` 用了 **MenuItem 作 Owner**（Add）/ `FrmMain.MainMenu`（Insert），且 `if MenuItem = nil` 分支里只 `MainMenu.Items.Add(Item)`（Insert 分支才真插入索引）| `:1321/1341` / `:1380/1399` | 托管侧按可用接缝照抄结构并注释差异 |
| 8 | `TScriptCmdParam` 最后一个字段与 `end;` 同行（`nParam10: Integer end;`），且**没有 `sParam10` 之外的 `nRawParam`**；本单元里它是**唯一**"非 0 起、非 10 对齐"的记录 | `:111` | 抽取器已处理（34 字段），测试断言 34 字段 + 末字段 `nParam10` |
| 9 | `TGuild` 在 `PluginInterface.pas` 写作 `TGuild`，在 `PluginImplement.pas` 写作 `TGUild`（大小写不一致） | `:73` vs 多处实现体 | 接缝接口用 `IGuildHandle`，注释标注原文两种拼写 |
| 10 | `_TBaseobject_IsNGMonster` / `_TBaseobject_GetGuildRankNo` / `_TBaseobject_GetGuildRankName` 的 `TBaseobject_` **小写 o**，与同族 `TBaseObject_` 不一致 | `PluginInterface.pas:735/897/900` | 类型名与实现体名照抄原文大小写 |
| 11 | `_TPlayObejct_IncExp`（`Obejct` 拼错） | `PluginInterface.pas:1653` | 照抄 |
| 12 | `_TBaseObject_SendBagItems(Plyaer: ...)`（`Player` 拼错） | `PluginInterface.pas:1866` | 参数名照抄 `Plyaer` |
| 13 | `_TBaseObject_GetUnPosionValue` / `GetUnTammingValue` / `TM2Engine_CheckBindType` 等拼写（Posion/Tamming） | 多处 | 照抄 |
| 14 | `PluginManager.pas:15` 明确记录国产 `MemoryModule` 改动版"时灵时不灵，还会崩溃" | `PluginManager.pas:15` | 托管侧**不复刻**该内存装载路径（见 §4.5） |

---

## 7. 生成器（可复现）

`PluginInterfaceTypes.g.cs` / `PluginInterfaceTables.g.cs` / `PluginInterfaceManaged.g.cs` / `PluginInterfaceHost.Stubs.g.cs`
**全部由脚本从 `.pas` 原文抽取生成**（禁止手工转录大段表/常量），生成器随源码入库：
`tests/GXX.M2Server.Tests/PluginsData/gen/*.ps1`（纯 ASCII，UTF-8 无 BOM，兼容 PowerShell 5.1 的 ANSI 解析）。

管线（按序执行）：

```powershell
$src = 'D:\chuanqi\daima\GXX原版_Delphi7\Source\M2Engine'
$pd  = '<worktree>\GXX.CSharp\src\GXX.M2Server\Plugins'
$sc  = '<worktree>\GXX.CSharp\tests\GXX.M2Server.Tests\PluginsData'
& gen\Gen-Records.ps1 -Src $src -Scratch $sc                    # → records.tsv / fields.tsv / aliases.tsv
& gen\Gen-Types.ps1   -Src $src -OutFile "$pd\PluginInterfaceTypes.g.cs" -Scratch $sc
& gen\Patch-Orig.ps1  -File "$pd\PluginInterfaceTypes.g.cs" -Kind types -Manifest "$sc\iface_manifest.tsv" -Header gen\hdr_types.txt
& gen\Gen-Tables.ps1  -Scratch $sc -OutFile "$pd\PluginInterfaceTables.g.cs"
& gen\Patch-Orig.ps1  -File "$pd\PluginInterfaceTables.g.cs" -Kind tables -Manifest "$sc\iface_manifest.tsv" -Header gen\hdr_tables.txt
& gen\Gen-Managed.ps1 -Scratch $sc -OutFile "$pd\PluginInterfaceManaged.g.cs"
& gen\Patch-Orig.ps1  -File "$pd\PluginInterfaceManaged.g.cs" -Kind managed -Manifest "$sc\iface_manifest.tsv" -Header gen\hdr_managed.txt
& gen\Gen-Stubs2.ps1  -Scratch $sc -OutFile "$pd\PluginInterfaceHost.Stubs.g.cs" -PortedList gen\ported-methods.txt
& gen\Patch-Stubs.ps1 -File "$pd\PluginInterfaceHost.Stubs.g.cs" -Header gen\hdr_stubs.txt
```

回读比对：测试 `PluginsInterfaceManifestTests` 读同一批 `.tsv` 清单，用反射核对生成结果 ——
生成器与测试**共用一份真源**，任一侧漂移都会立刻红。

---

## 8. 接缝清单（待后续移植接入）

| 接缝 | 待接入单元 | 影响面 |
|---|---|---|
| `IPluginHostEnv` | `M2Share`（g_Config/g_version/g_buildtime）、`svMain`（FrmMain）、`Envir`（g_MapManager）、`uMagicACUtils`（g_MagicACList） | `TMenu_*`/`TIniFile_Create`/`TM2Engine_*`/`TMapManager_*` 共 60 个宿主例程 |
| `IUserEngineSeam` | `UsrEngn`（TUserEngine） | `TUserEngine_*` 53 个 |
| `IBaseObjectHandle` | `ObjBase`（TCreature） | `TBaseObject_*` 185 个 |
| `ISmartObjectHandle` / `IPlayObjectHandle` / `IHeroObjectHandle` / `IDummyObjectHandle` / `INormNpcHandle` | `ObjPlayer` / `ObjHero` / `ObjDummy` / `ObjNpc` | `TSmartObject_*` 103 + `TPlayObject_*` 159 + `THeroObject_*` 33 + `TDummyObject_*` 3 + `TNormNpc_*` 18 |
| `IGuildHandle` / 行会管理 | `Guild` | `TGuild_*` 31 + `TGuildManager_*` 4 |
| `IListHandle` / `IStringListHandle` / `IMemoryStreamHandle` / `IMenuItem` / `IIniFileHandle` | `Classes` / `Menus` / `IniFiles` 的 C# 对应物 | 已给出托管承载（`PluginHostRuntime.cs`），待真实实现替换 |
| `TUserMagic` / `TMagic` / `TDynamicVar` / `TMasterRankInfo` | `Grobal2.pas` / `Magic.pas` | 参数级 5 个例程 |
| `IM2ServerPlugin` + `TPlugInit`（原生 ABI） | `PluginManager`（本波未移植，属 M2Server 常驻区） | 装载层 |
| `BufferCrc.Compute` | `Common/CheckCrc.pas` | **算法待核**：当前用标准 CRC-32，不保证与原版 `BufferCrc` 数值一致 |
| `ZLibEncode/DecodeBuffer`、`Encrypt/DecryptBuffer`、`Encrypt/DecryptPassword` | `GXX.Core.Compress.ZlibEx`、`EncryptUnit_LF`（RunGate 车道已移植，待上移 GXX.Core） | 4 个例程 |

---

## 9. 未完成 / 后续建议

1. **500 个宿主例程仍是显式未完成壳**（`PluginInterfaceHost.Stubs.g.cs`，抛 `NotImplementedException` 并带原文行号）。
   签名面 100% 齐全、可编译、可反射核对；实现体需等 `ObjBase/ObjPlayer/ObjSmartMon/Envir/Guild/UsrEngn` 落地后逐条替换。
2. `PluginManager.pas`（2,448 行）**不属本车道**（M2Server 常驻区）。本车道只复刻了它的**装载路径判读**
   （`LoadLibrary/GetProcAddress/PlugList.txt/UnInit`）到 `PluginAssemblyLoader.cs`，供集成时替换。
3. `BufferCrc` 算法待与 `Common/CheckCrc.pas` 对齐（当前用标准 CRC-32，**数值不保证一致**）。
4. `MemoryModuleEx.pas`（815 行）的内存装载路径**不复刻**（§4.5），若集成方需要二进制插件兼容，须由
   原生装载路径（本车道已实现 `LoadLibraryA/GetProcAddress`）承担。
5. 本车道未触碰 `PluginFuncAssign.inc` / `PluginFuncLoad_Dll.inc` / `PluginFuncLoad_Mem.inc`（它们只在
   `PluginManager.pas` 里被 `{$I}` 引用，属装载层证据，不是独立单元）。
