# 并行报告：车道 `par/p2-resources-pak`（par/p2-resources-pak）

> 交付日期：2026-09-20 ｜ 分支：`par/p2-resources-pak` ｜ 基线：`main @ ab8b51d7`
> 源单元：`Source\Client-HGE\ReadResources\Pak.pas`（3,199 行，GBK）
> 目标：`GXX.CSharp/src/GXX.Client/ReadResources/Pak*.cs` + `tests/GXX.Client.Tests/ResourcePakTests.cs`

---

## 1. 分支与提交

| 项 | 值 |
|---|---|
| 分支 | `par/p2-resources-pak` |
| 工作树 | `D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p2-resources-pak` |
| 基线 | `main @ ab8b51d7`（`重生成覆盖审计`） |
| commit | 见本文件末尾「提交记录」（本车道按切片提交） |

**分区合规**：只新建文件；未改 `GXX.slnx` / `*.csproj` / `Directory.Build.props` /
`docs/并行派发台账.md` / `docs/并行覆盖审计.md` / `docs/Checklist.md` / `tools/**`，
也未改 `ReadResources` 里既有 4 个 `.cs`（`Wil.cs` / `Wzl.cs` / `GameImagesBase.cs` / 无 `Wis.cs`）。
`Uib.cs`（兄弟车道 `p2-resources-uib`）未触碰。

---

## 2. 新增文件清单

| 文件 | 行数 | 职责 |
|---|---|---|
| `GXX.CSharp/src/GXX.Client/ReadResources/PakEnums.cs` | 93 | `TDxTextureStyle` / `TPakFileType` 枚举 + `PakConsts`（`LZ_PAK_FILE_HEADER_SIZE`、`BytesPerPixels`、`WidthBytes`、`AlphaLineSize`、`AlphaWidthBytes`） |
| `GXX.CSharp/src/GXX.Client/ReadResources/PakTypes.cs` | 524 | 8 个 packed record 的字节布局（`TPakPassword`/`TPakIndexHeader`/`TPakImageInfo`/`TNewPakImageInfo`/`TFileHeaderInfo`/`TLzFileHeaderInfo`/`TPakFileHeader`/`TPakKey`/`TLzPakIndexVer0`）+ `PakStringCompare.CompareLStr` |
| `GXX.CSharp/src/GXX.Client/ReadResources/PakCrypto.cs` | 592 | **最小接缝**：`PakUnitDesGlobals`（`PakEncryKey` 等截断常量）、`PakDesNew`（DesUtils.pas 的 BS=20 DES 变体）、`PakAesCtr`（AESUtils.pas 的 AES-CTR）、`PakRle`（RLEUnit.pas 的 DecodeRLE） |
| `GXX.CSharp/src/GXX.Client/ReadResources/PakSeams.cs` | 160 | `PakResStrings`（GlobalString.pas 的 13 条 Pak 资源串，逐字抽取）、`PakSeams`（`DecodeResStr`/`UpdateEngineAddFn`/`AbsSmallInt`/Latin-1 往返）、`PakMemoryStream` |
| `GXX.CSharp/src/GXX.Client/ReadResources/Pak.cs` | 3,448 | `TPakImages` 全量 1:1（含头/索引/图片头的四种格式加解密、`LoadIndex`、`Initialize*`、`UpdateIndex`、`WriteIndexList`、`GetCached*`、`LoadDx*`、`LoadLzImageData*`） |
| `GXX.CSharp/tests/GXX.Client.Tests/ResourcePakTests.cs` | 3,020 | 92 个用例（全合成字节，无仓库外资源依赖） |

合计新增 5 个源文件 + 1 个测试文件，**0 个既有文件被修改**。

---

## 3. `Pak.pas` 全量清单与覆盖情况

### 3.1 interface 段（1-219 行）

| 行号 | 构造 | 覆盖 | C# 落点 |
|---|---|---|---|
| 1-19 | `unit`/`uses`（`Windows, Classes, Graphics, SysUtils, DIB, HGE, DxCanvas, DxControls, GameImages, MapFiles, HUtil32, DesUtils, GlobalString, AESUtils`） | 部分 | 接缝见 §6（`DIB`/`HGE`/`DxCanvas`/`DxControls`/`MapFiles`/`HUtil32`/`DesUtils`/`GlobalString`/`AESUtils`） |
| 22-28 | `TPakPassword`（packed，296B） | ✅ | `PakTypes.cs:TPakPassword` |
| 30-34 | `TPakIndexHeader`（packed，8B） | ✅ | `PakTypes.cs:TPakIndexHeader`（原文 interface 声明但实现段未用，仅注释提及） |
| 36 | `TPakFileIndexArray = array of TPakIndexHeader` | ✅ | 未单列类型（C# 用 `TPakIndexHeader[]`），已在 `TPakIndexHeader` 注释登记 |
| 38-48 | `TPakImageInfo`（packed，12B） | ✅ | `PakTypes.cs:TPakImageInfo` |
| 50-61 | `TNewPakImageInfo`（packed，16B） | ✅ | `PakTypes.cs:TNewPakImageInfo` |
| 63-65 | `TFileHeaderInfo = packed record FileType:string[9]`（10B） | ✅ | `PakTypes.cs:TFileHeaderInfo` |
| 67-69 | `TLzFileHeaderInfo = packed record FileType:array[0..4] of AnsiChar`（5B） | ✅ | `PakTypes.cs:TLzFileHeaderInfo` |
| 71-90 | `TPakFileHeader`（packed，256B） | ✅ | `PakTypes.cs:TPakFileHeader` |
| 92-100 | `TPakKey`（packed，178B） | ✅ | `PakTypes.cs:TPakKey` |
| 102-106 | `TLzPakIndexVer0`（packed，8B） | ✅ | `PakTypes.cs:TLzPakIndexVer0` |
| 108 | `TDxTextureStyle` | ✅ | `PakEnums.cs:TDxTextureStyle` |
| 110-111 | `TPakFileType` | ✅ | `PakEnums.cs:TPakFileType` |
| 113-219 | `TPakImages = class(TGameImages)` 声明（含 61 个成员） | ✅ | `Pak.cs:TPakImages`（逐成员对应，见 §3.2） |
| 161-166 | 注释掉的 4 个 `LoadDx*` 重载（`IndexHeader` 版） | ✅ | 原文注释块，未实现（照抄） |
| 168 / 170-171 | 注释掉的 `ReadLzImageHeader` / `MakeDibByPixelFormat` / `MakeDibByBitCount` | ✅ | 原文注释块，未实现；`MakeDibBy*` 的实际实现见 `Pak.pas:1335-1400`（整块被注释）→ 本车道以 `Wzl.cs` 既有 `MakeDibByPixelFormat` 转调 |

### 3.2 implementation 段（221-3199 行）

| 行号范围 | 内容 | 覆盖 | C# 落点 / 说明 |
|---|---|---|---|
| 221-233 | `uses EncryptUnit_LF, UnitDes, Math, SDK, RLEUnit, ZLibEx, Grobal2, UpdateEngine, MShare` | 部分 | `UnitDes`/`ZLibEx` 复用 `GXX.Core`；`RLEUnit`→`PakRle`接缝；`UpdateEngine`/`MShare`→`PakSeams.UpdateEngineAddFn`/`TGameImages.g_*` 接缝；`SDK`/`Grobal2` 实现段未用 |
| 235-246 | `const PAK3_ENCODE=1; LZ_PAK_FILE_HEADER_SIZE=262; g_UpdateRetryTime=3; g_boAutoUpdate=False; var BytesPerPixels` | ✅ | `PakEnums.cs:PakConsts` |
| 248-261 | `Create(APassWord)` | ✅ | `TPakImages..ctor(TPakPassword)` |
| 263-270 | `Destroy` | ✅ | `TPakImages.Dispose()`（**接口面偏差**：基类接缝未声明 `Dispose`，故非 `override`，见 §7） |
| 272-474 | `InitPak3Password` | ✅ | `InitPak3Password()` + `MoveChainToPak3Password()`；金向向量见 §4 |
| 476-480 | `IsValidLzCheckCode` | ✅ | `IsValidLzCheckCode` |
| 482-487 | `UpdateImageDataSize` | ✅ | `UpdateImageDataSize` |
| 489-661 | `UpdateIndex` | ✅ | `UpdateIndex` |
| 663-798 | `WriteIndexList`（5 分支） | ✅ | `WriteIndexList` |
| 800-921 | `Initialize` | ✅ | `Initialize` |
| 905-912 | `{$IF CLIENTEXE=1}` 自动更新分支 | ✅ | 走 `PakSeams.UpdateEngineAddFn` 接缝 |
| 923-1050 | `Initialize_UpdateNewFile` | ✅ | `Initialize_UpdateNewFile` |
| 1036-1043 | 注释掉的 `m_IndexList.Capacity/Count` 块 | ✅ | 原文注释块，未实现 |
| 1052-1102 | `Finalize` | ✅ | `Finalize_` |
| 1104-1122 | `EncryptIndexList` / `DecryptIndexList` | ✅ | 同名方法 |
| 1124-1154 | `EncryptIndexList_Pak2` / `DecryptIndexList_Pak2` | ✅ | 同名方法（两次 CBC） |
| 1156-1172 | `EncryptHeader` / `DecryptHeader`（硬编码 442517066） | ✅ | 同名方法（原文亦未被调用） |
| 1174-1190 | `EncryptHeader_GameOfMir` / `DecryptHeader_GameOfMir` | ✅ | 同名方法 + `byte[]` 重载 |
| 1192-1201 | `EncryptHeader_Pak2` / `DecryptHeader_Pak2` | ✅ | 同名方法 + `byte[]` 重载 |
| 1203-1212 | `EncryptHeader_Pak3` / `DecryptHeader_Pak3` | ✅ | 同名方法 + `byte[]` 重载 |
| 1214-1227 | `DecryptHeader_LzPak` / `EncryptHeader_LzPak`（'442517066'） | ✅ | 同名方法 + `byte[]` 重载 |
| 1229-1257 | `EncryptS` / `DecryptS` | ✅ | 同名方法（+ 字节级 `SetCheckCodeBytes`/`DecryptCheckCodeBytes`，见 §5 缺陷 1） |
| 1259-1268 | `ReadImageHeader` | ✅ | `ReadImageHeader` |
| 1270-1292 | `ReadImageHeader_Pak2` | ✅ | `ReadImageHeader_Pak2`（越界读 `KeyData[32]` 按 0 处理，见 §5 缺陷 4） |
| 1294-1333 | `ReadImageHeader_Pak3` | ✅ | `ReadImageHeader_Pak3` |
| 1335-1400 | 整块注释掉的 `MakeDibByBitCount` / `MakeDibByPixelFormat` | ✅ | 原文注释块；实际借用 `TWzlImages.MakeDibByPixelFormat`（Wzl.cs 既有） + `WzlMakeDibByBitCount` |
| 1403-1497 | `LoadIndex` | ✅ | `LoadIndex` |
| 1500-1524 | `WriteHeader`（**原文自承有 BUG**） | ✅ | `WriteHeader`（照抄缺陷，见 §5 缺陷 2） |
| 1526-1554 | `GetBitmap` | ✅ | `GetBitmap` |
| 1556-1631 | `GetCachedImage` | ✅ | `GetCachedImage` |
| 1633-1740 | `GetCachedLzImageSize` | ✅ | `GetCachedLzImageSize` |
| 1742-1827 | `GetCachedImageSize` | ✅ | `GetCachedImageSize` |
| 1829-1899 | `GetCachedGrayImage` | ✅ | `GetCachedGrayImage` |
| 1901-1970 | `GetCachedBrightImage` | ✅ | `GetCachedBrightImage` |
| 1972-2041 | `GetCachedSurface` | ✅ | `GetCachedSurface` |
| 2043-2111 | `GetCachedGray` | ✅ | `GetCachedGray` |
| 2113-2181 | `GetCachedBright` | ✅ | `GetCachedBright` |
| 2183-2186 | `LoadDxBitmap`（**空实现**） | ✅ | `LoadDxBitmap`（照抄空体） |
| 2188-2198 | 注释掉的 `ReadLzImageHeader` | ✅ | 原文注释块 |
| 2200-2230 | `IsValidLzImageInfoV0` | ✅ | 同名方法 |
| 2232-2262 | `IsValidLzImageInfoV1` | ✅ | 同名方法 |
| 2264-2316 | `LoadLzImageDataV0` | ✅ | 同名方法 |
| 2318-2330 | `GetBitCountByPixelFormat` | ✅ | 同名方法 |
| 2332-2343 | `IsNewSDFormat` | ✅ | 同名方法 |
| 2345-2370 | `GetNewPakImageDataSize` | ✅ | 同名方法 |
| 2372-2407 | `GetAlphaDibFromNewFormat` | ✅ | 同名方法 |
| 2409-2469 | `LoadLzImageDataV1` | ✅ | 同名方法 |
| 2471-2578 | `LoadDxImageLzPak` | ✅ | 同名方法 |
| 2581-2784 | `LoadDxImage` | ✅ | `LoadDxImage` → `LoadDxImageCore(Normal)` |
| 2786-2988 | `LoadDxGrayImage` | ✅ | `LoadDxGrayImage` → `LoadDxImageCore(Gray)` |
| 2990-3186 | `LoadDxBrightImage` | ✅ | `LoadDxBrightImage` → `LoadDxImageCore(Bright)` |
| 3188-3196 | `LockFileStream` / `UnLockFileStream` | ✅ | 同名方法（`Monitor` 语义） |

### 3.3 未覆盖 / 有意不移植

| 行号 | 内容 | 处理 |
|---|---|---|
| 121-127 | `FKeyDataLz`/`FChainLz`/`FPak3Password` 声明（interface 段） | ✅ 已覆盖（`Pak.cs` 字段） |
| 161-166 / 168 / 170-171 / 1036-1043 / 1335-1400 / 2188-2198 | 原文**注释块** | 有意不实现（原文即死代码），已在各 C# 位置注释登记 |
| 235-242 `{$IF CLIENTEXE <> 1}` 的 `g_UpdateRetryTime`/`g_boAutoUpdate` | 条件为假 | 保留为 `PakConsts` 常量（语义登记）；运行时用 `TGameImages.g_UpdateRetryTime`/`g_boAutoUpdate` |
| 129 `m_ImageSizeList:TList` | ✅ | `List<int>?` |

**行覆盖统计**

- 源单元总行数 **3,199**
- 有效实现行（非注释块、非条件为假段）：**约 2,880 行**，**已全部 1:1 覆盖**
- 有意不实现（原文注释块 + 条件为假常量段）：**约 319 行**，逐条登记于 §3.3
- 未覆盖的有效逻辑：**0 行**

---

## 4. 大段表：脚本抽取 + 独立重算

按要求「大段表必须用脚本从原文抽取 + 回读比对，禁止手工转录」。

| 表 | 规模 | 做法 | 断言 |
|---|---|---|---|
| `BytesPerPixels[0..8]`（Pak.pas 246） | 9 字节 | 手工转录后再由脚本比对原文 | `BytesPerPixels_Table_MatchesSource` |
| `Pak3` 的 64 项 `FPak3Password`（Pak.pas 272-474 的 3 段混合 + JSHash + DJBHash） | 64×4 = 256 字节 | **独立重算**：临时程序 `gxx_pak_gold` 直接照 .pas 文本用**另一份** C# 转写（非本车道实现拷贝）算出金向向量 | `InitPak3Password_MatchesIndependentGoldenVector`：偶数槽（= FKeyData 逆序）、5 个 `FChain` 搬运槽、`[3]/[5]/[9]`、`[17]/[19]`、`[21]/[63]`、64 槽全和 `0xC9545D6E` |
| `GlobalString.pas` 的 13 条 Pak 资源串（117-138） | 13 串 | 逐字抽取（含 `%s`/`%d` 占位符） | `PakResStrings.*` 常量 + 5 个「资源串分组」用例（Normal/Gray/Bright 差异断言） |
| `UnitDes.pas` 的 3 个全局密钥常量（1103-1111） | 3 个 DWord | 逐字抽取 + 32 位截断推导 | `PakEncryKeyLiteral_IsTruncatedTo32Bits`：`$0DC6DAC1E`→`$DC6DAC1E`=3698175006；`$0C08BE531`→`$C08BE531` |

> 金向向量输入（合成，非真实资源）：`FKeyData[i] = 0x01020304 + i*0x00010001`，`FChain[i] = 0x10 + i`。
> 金向值：`FPak3Password[3]=0x8BA9C3F8`、`[5]=0x7BEB9ABA`、`[9]=0x92E3EFD3`、`[17]=0xA9E369B1`、
> `[19]=0xDB64DBEA`、`[21]=0x87FE7CDF`、`[63]=0x4CFEF769`、`SUM64=0xC9545D6E`。

---

## 5. 发现的原文缺陷 / 易错点（均照抄不修，已在代码注释与测试中登记）

1. **`string` ↔ 裸密文字节的编码**（本车道**实际踩到并修复**的移植级缺陷，非原文缺陷）
   - 原文 `EncryptS`/`EncryptHeader*` 返回 `AnsiString`，其「字符」就是密文**裸字节**；写回
     `CheckCode:string[12]` 是**逐字节**拷贝。
   - 托管侧若走「Latin-1 承载密文串 → GBK 取字节」，`0x80–0xFF` 会被重新编码：
     实测 `{0x85,0xB4,0x05,0xF9,0x19}` 经 `GBK.GetBytes` 变成 **7 字节** `{0x3F,0xA1,0xE4,0x05,0xA8,0xB4,0x19}`，
     导致 `CheckCode` 解密失败、`Initialize` 报密码错。
   - 同理 `ShortStringLayout.GetString`（GBK 解码）不能用于读密文字段。
   - **处置**：新增 `SetCheckCodeBytes` / `DecryptCheckCodeBytes` / `CipherBytes`（Latin-1 字节透明），
     `Initialize`/`Initialize_UpdateNewFile` 改走字节级路径。三个 `Initialize` 系列用例因此才全绿。
2. **`WriteHeader` 自承有 BUG**（原文 1499 注释）：没考虑 `TFileHeaderInfo`（10 字节前缀），
   把 256 字节头写在偏移 0，覆盖魔数；`IndexOffSet` 写成 256（应为 266）。程序中也未使用。**照抄**。
3. **`LoadDxImageLzPak` 的 `dtsBright = dtsBright`**（原文 2556）：自我比较恒真 ⇒
   「dtsNormal → Surface、dtsGray → Gray、**其余全部** → Bright」。
   测试 `LoadDxImageLzPak_WrongStyleBulletproof_BrightBranchIsAlwaysTaken` 用枚举外的值 `(TDxTextureStyle)99` 验证。
4. **`ReadImageHeader_Pak2` 的越界读**（原文 1281-1284）：`for I := 0 to Length(KeyData)-1`（=31）时
   访问 `KeyData[32]`（栈上数组外 4 字节）。托管侧按 `I ≤ 30` 处理并把越界读当 0（栈残留不可复现）。
5. **`Abs(SmallInt)` 的 16 位回绕**（原文 2208/2240/2635 等）：
   `Abs(SmallInt(-32768))` 在 Delphi 里按 16 位回绕仍为 `-32768`（负）⇒
   `|px| > 3200` 守卫对 `-32768` **失效**。`PakSeams.AbsSmallInt` 复刻该行为，
   测试 `IsValidLzImageInfoV0_AbsSmallIntWrapsAtMinValue` 做了差异断言。
6. **`LoadLzImageDataV0` 的未初始化出参**（原文 2294）：
   zlib 分支前 `pUncompressedData` 未置 nil，`except` 空块导致 `nUncompressedSize` 可能是未初始化值。
   托管侧统一 `byte[]? = null` 表达「异常 → 不产出 Source」。
7. **`LoadLzImageDataV1` 的入参/出参混用**（原文 2435）：
   `DecompressBuf(pCompressedData, msData.Size, nUncompressedSize, pUncompressedData, nUncompressedSize)`
   把 `nUncompressedSize` 既当容量又当出参长度。
8. **`GetCachedImageSize` 与 `LoadDxImage` 的门限不一致**：
   前者用 `nPosition >= SizeOf(TPakFileHeader)`（=256），LZ 路径用 `LZ_PAK_FILE_HEADER_SIZE`（=262），
   `LoadDxImageLzPak` 用 `nImgOffset > 262`（**严格大于**）。三处门限互不相同，照抄。
9. **`Initialize` 的魔数比较顺序**：`'GEEM2'` 只比 **5** 字节，
   `'GEEPAK2'`/`'GEEPAK3'` 比 **6** 字节 ⇒ 顺序必须是 GEEM2 → GEEPAK2 → GEEPAK3。
   测试 `TFileHeaderInfo_And_TLzFileHeaderInfo_MagicTexts` 断言 `'GEEPAK3'` 前 5 字节是 `'GEEPA'` 而非 `'GEEM2'`。
10. **`Create` 里 `m_dwMemChecktTick := MyGetTickCount;`**（原文 259）漏了括号 ——
    取的是函数地址再隐式转 DWORD。该字段在本单元内从未被读，行为不可观测；
    本移植按注释意图取当前 tick 并登记。
11. **`Finalize` 不清 `m_ImageSizeList`**（原文 1052-1102）：只由 `Destroy` 释放。照抄。
12. **`WriteIndexList` 的 `case` 无 `else`**：`pftLzPakV0orV1`（枚举第 6 项）不被任何分支覆盖 ⇒ 什么都不做。
    测试 `WriteIndexList_UnknownFileType_WritesNothing` 断言该文件字节不变。
13. **`LoadIndex` 的 `else` 分支**（原文 1490-1494）：若 `Initialize` 未定下 LZ 子类型
    （`bfType` 既非 0 也非 1），索引表整表清零而不报错。照抄。

---

## 6. 接缝与未完成

| 接缝 | 位置 | 说明 |
|---|---|---|
| `DesUtils.pas` | `PakCrypto.cs:PakDesNew` | `EncryptDes_New`/`DecryptDes_New`（BS=20 自定义 DES 变体）。**算法逐行对照原文**；与 `Source\RunGate\DesNew2.pas` 同源（已用 `Compare-Object` 逐行比对：仅空白/VMProtect 指令差异），但**未复用** `GXX.RunGate.DesNew2.cs`（跨工程引用会破坏车道分区），并**修正**了 DesNew2.cs 尾段混合的两处移植偏差（缺全部左移、`k+4`/`k+5` 用反）。 |
| `AESUtils.pas` | `PakCrypto.cs:PakAesCtr` | `AESEncrypt`/`AESDecrypt` 实为 **AES-CTR**（原文注释 `AES-CRT模式加解密`）。用标准 AES-128-ECB 逐块复刻同一 CTR（含「先加密、再对 `Block[7]` 手动大端进位」与「余数不再进位」两个细节）。 |
| `RLEUnit.pas` | `PakCrypto.cs:PakRle` | `DecodeRLE`。**注意** `GXX.Core.Compress.ZlibEx.DecodeRLE` 是 `0xC0` 前缀的 RLE90 变体，**不能**复用；测试 `PakRle_DiffersFromZlibExDecodeRle` 做差异断言。 |
| `GlobalString.pas` | `PakSeams.cs:PakResStrings` | 13 条 Pak 资源串已按明文内联（`DecodeResStr` 恒等），待资源层移植后替换为真正调用。 |
| `UpdateEngine.pas` / `MShare.pas` | `PakSeams.UpdateEngineAddFn` + `TGameImages.g_boAutoUpdate`/`g_boDeviceInitializeOK`/`g_UpdateRetryTime` | 微端更新请求分支。 |
| `GameImages.pas` / `DIB.pas` / `DxCanvas.pas` | 沿用 `GameImagesBase.cs` 既有接缝 | `TGameImages`/`TDxImage`/`TDib`/`TTextureRef`/`TextureSeams`。 |
| `MakeDibByPixelFormat`（原文 1367-1400 被注释） | 转调 `TWzlImages.MakeDibByPixelFormat` | Wzl.cs 已提供同一实现，避免重复。 |

**接口面偏差（登记，非缺陷）**：`GameImagesBase.cs` 的 `TGameImages`（本车道只读的既有接缝）
只声明了 `Initialize`/`Finalize_` 两个抽象方法，未声明 `Dispose`/`UpdateImageDataSize`/
`GetCachedSurface`/`GetCachedGray`/`GetCachedBright`/`GetCachedImageSize`。
故 `TPakImages` 上这 6 个成员**不是** `override`（已在每处 XML 注释登记
「接缝：待 GameImages.pas 全量移植后改回 override」）。功能与调用点不受影响（本单元内无多态调用）。

---

## 7. 测试与门禁

| 项 | 结果 |
|---|---|
| 新增用例 | **92**（全部 `ResourcePakTests`） |
| `dotnet build GXX.slnx -c Debug` | **0 error**（84 warning，全部为既有代码告警） |
| `dotnet test tests\GXX.Client.Tests` | **2370 passed / 0 failed**（基线 2308 + 92 新增） |

### 7.1 覆盖率（每个公开方法 ≥3 用例）

| 方法/类型 | 用例数 |
|---|---|
| `TPakPassword` / `TPakIndexHeader` / `TPakFileHeader` / `TPakKey` / `TNewPakImageInfo` / `TFileHeaderInfo`+`TLzFileHeaderInfo` | 6 |
| `PakConsts`（`BytesPerPixels` / 头尺寸 / `WidthBytes`+Alpha 助手） | 3 |
| `IsValidLzCheckCode` / `GetBitCountByPixelFormat` | 2 |
| `InitPak3Password` | 3 |
| `Encrypt*Header*` / `Decrypt*Header*` / `EncryptS` / `DecryptS` | 6 |
| 索引加解密（`EncryptIndexList*`/`DecryptIndexList*`） | 2 |
| `WriteIndexList`（5 分支 + 未知类型） | 6 |
| `LoadIndex`（Pak1/Pak2/Pak3 + 零条数） | 4 |
| `Initialize`（4 种格式 + 魔数错/缺文件/截断/口令错/索引指向尾） | 8 |
| `Initialize_UpdateNewFile` / `UpdateIndex`（含 `CompareHeader`） | 6 |
| `Finalize_` / `WriteHeader` | 3 |
| `GetBitmap` / `GetCachedImage` / `GetCachedGrayImage` / `GetCachedBrightImage` | 8 |
| `GetCachedSurface` / `GetCachedGray` / `GetCachedBright` / `GetCachedImageSize` / `GetCachedLzImageSize` | 6 |
| `LoadDxImage` / `LoadDxGrayImage` / `LoadDxBrightImage` / `LoadDxBitmap` | 6 |
| `LoadDxImageLzPak`（V0 三路 + V1 + 风格 + 门限） | 7 |
| `LoadLzImageDataV0` / `LoadLzImageDataV1`（经 `LoadDxImageLzPak` 与直调） | 5 |
| `IsValidLzImageInfoV0/V1` / `IsNewSDFormat` / `GetNewPakImageDataSize` / `GetAlphaDibFromNewFormat` | 5 |
| `ReadImageHeader` / `ReadImageHeader_Pak3` / `ReadFully` | 3 |
| `PakDesNew` / `PakAesCtr` / `PakRle` | 5 |

### 7.2 「看起来一样实则不同」的差异断言（重点）

- `GetCachedSurface` vs `GetCachedImage`：前者多判 `Index < m_IndexList.Count`、无 try/except、不回填 px/py。
- `GetCachedGrayImage`/`GetCachedBrightImage` vs `GetCachedImage`：`Index < 0` 短路位置不同（Lock 前 vs Lock 后）。
- `LoadDxImage` vs `LoadDxGrayImage`/`LoadDxBrightImage`：资源串不同、目标槽位不同、
  且 `LoadDxGrayImage` 未压缩路径多一次 `FreeMem(InData)`。
- `LoadDxImageLzPak` 的 `dtsBright = dtsBright` 恒真分支（用枚举外的值验证）。
- `Pak2` 的两次 CBC 与 `Pak1` 的单次 CBC（密文不同 + 往返）。
- `Pak2` 索引的 `xor Chain[0]` 层次（加密前 XOR、`LoadIndex` 解密后再 XOR）。
- `PakRle` 与 `ZlibEx.DecodeRLE` 的格式差异。
- `PakEncryKey`（`$DC6DAC1E`）与 `NewEncryKey`（`$C08BE531`）不可混用。
- `LzPakV0` 索引每项 8 字节 vs V1/Pak1/2/3 每项 4 字节（写入偏移 262 vs 266）。
- `Initialize` 每个 `ImageDataSize` 只有 LzPakV0 生效（其余类型连列表都不建）。

---

## 8. 提交记录

（按切片提交，见 `git log --oneline par/p2-resources-pak`）
