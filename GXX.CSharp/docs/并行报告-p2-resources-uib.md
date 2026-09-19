# 并行批次 P2 · 车道 `par/p2-resources-uib` 交付报告

> 范围：客户端 **UIB 资源格式（`Uib.pas`）** 1:1 移植
> 基线：main `acc683eb`
> 产出：`src/GXX.Client/ReadResources/Uib.cs`、`tests/GXX.Client.Tests/ResourceUibTests.cs`、`tests/GXX.Client.Tests/ResourceUibExtraTests.cs`

---

## 1. 结论

| 项 | 结果 |
|---|---|
| `dotnet build GXX.slnx -c Debug` | **0 error**（71 warning，全为既存 xUnit 分析器提示） |
| `dotnet test tests/GXX.Client.Tests -c Debug` | **2345 passed / 0 failed**（基线 2308 → 新增 37 用例） |
| `Uib.pas` 行覆盖 | 1 – 503（接口段 1-38、实现段 40-547，文件共 503 行；全部实存逻辑逐条对照） |

---

## 2. `Uib.pas` 移植对照

### 2.1 单元定位（重要）

`Uib.pas` **不是容器格式解析**：它没有任何文件头/索引/调色板解析逻辑。
`TUibImages` 只维护一份 `m_FileList:TStringList`（**文件名列表，顺序即索引**），
每张图按列表里的路径从磁盘单独 `TDIB.LoadFromFile`。因此本单元的保真点是
「列表语义 + 缓存槽位 + 更新状态机」，而不是二进制格式。

### 2.2 逐块对照

| Delphi（Uib.pas） | C#（Uib.cs） | 说明 |
|---|---|---|
| 14-38 `TUibImages` 声明 | `TUibImages : TGameImages` | `m_FileList` 公开为 `TStringList` |
| 46-50 `Create` | 构造函数 | `m_FileList := TStringList.Create` |
| 52-56 `Destroy` | —（托管 GC） | 无显式释放 |
| 58-67 `Initialize` | `Initialize()` | `BitCount := 8`；`ImageCount := m_FileList.Count`；分配 `m_ImgArr`（引用类型需逐项实例化）；**不检查 FileName/文件存在性** |
| 69-116 `Finalize` | `Finalize_()` | 清三个 IndexList + 释放 `m_ImgArr` + `ImageCount := 0` |
| 118-148 `LoadDxBrightImage` | 同名 | `FileExists` → `LoadFromFile` 失败 `Exit` → `W*H > 4` → 打点 → D3D/普通纹理 |
| 150-181 `LoadDxGrayImage` | 同名 | 同上，打 `dwLatestGrayTime` |
| 183-213 `LoadDxImage` | 同名 | 同上，打 `dwLatestTime` |
| 215-230 `GetIndexByName` | 同名 | `LowerCase` 线性查；未命中 `Add`；**无条件** `ImageCount := Count` |
| 232-263 `GetCachedSurface` | 同名 | `IndexList.Add` **在装载之前**；`UpdateEngine(udtFileOther)` |
| 265-296 `GetCachedGray` | 同名 | `GrayIndexList.Add` 同样前置 |
| 298-329 `GetCachedBright` | 同名 | **守卫在 `Lock` 之前**（与 Surface/Gray 结构不同）——照抄 |
| 331-366 `GetCachedImage` | 同名 | 带 `out int px, py` |
| 368-407 `GetCachedImageSize` | 同名 | 原文注释「HZQ 20230524 尝试添加…未经过验证」；尺寸取自 `m_ImgArr[]` 的 `nWidth/nHeight`，而 `LoadDxImage` 不回填 → **恒为 0**（照抄） |
| 409-444 `GetCachedGrayImage` | 同名 | 带 `out int px, py` |
| 446-482 `GetCachedBrightImage` | 同名 | 带 `out int px, py` |
| 484-508 `GetSurfaceByName` | 同名 | `IndexList.Add` 被原文注释掉 → 本分支不加 |
| 510-543 `StreamSaveToFile` | 同名 | 建目录 + 落盘（异常吞掉）+ 清 `boUpdateStart`；`Stream = nil` 时另置 `boUpdateStop` |
| 545-547 `initialization`（空） | — | 原文为空段 |

### 2.3 接缝（本车道不顺手移植的依赖）

| 原文依赖 | C# 接缝 | 备注 |
|---|---|---|
| `DIB.pas TDIB.Create/LoadFromFile` | `BitmapFileSeams.LoadFromFileFn` | 默认实现解析最小 BMP（14+40 头 + 8/16/24/32bpp 行），失败返回 `null` 对应原文 `except ... Exit` |
| `DxCanvas.pas NewTexture/NewTextureGray/NewTextureBright` | `TextureSeams.*Fn`（既有接缝） | WZL/WIS 车道已建立 |
| `GameImages.pas FileData32/FileDataGray32/FileDataBright32` | D3D 分支与普通分支共用 `TextureSeams.*Fn` | `GameImages.pas` 全量移植后接入 |
| `UpdateEngine.Add(..., StreamSaveToFile)` | `TGameImages.UpdateEngineAddFn`（既有接缝） | 回调 `StreamSaveToFile` 形态以 `(sender, stream, index, fileName)` 提供 |
| `MShare.pas g_boAutoUpdate / g_boDeviceInitializeOK / g_UpdateRetryTime` | `TGameImages` 静态字段（既有） | — |
| `GlobalString.DecodeResStr` / `OutMessage` | `TGameImages.g_DebugTextOut`（既有接缝） | `Uib.pas` 只在 `DebugTextOut` 里用到 |

---

## 3. 测试

### 3.1 `ResourceUibTests.cs`（22 用例）

覆盖：`Initialize`/`Finalize` 计数与列表处置、`GetIndexByName` 大小写不敏感 + 未命中追加、
`LoadDx*` 三条早退（文件不存在 / 加载失败 / 面积 ≤ 4）、`GetSurfaceByName` 的「不加 IndexList」与列表增长、
缓存四入口的 lazy-load 与 IndexList 语义、`GetCachedImageSize` 的恒 0 行为、
UpdateEngine 的 udtFileOther 接缝与空文件名跳过、`StreamSaveToFile` 的落盘/建目录/`boUpdateStop` 翻转/异常吞掉。

> 说明：本文件由事故前的抢救副本 `.worktrees/_salvage/ResourceUibTests.cs` 恢复，
> 在新工作树内针对**重建后的** `Uib.cs` 原样跑通（22/22），用作重建正确性的交叉验证。

### 3.2 `ResourceUibExtraTests.cs`（15 用例，本次新增）

补齐抢救副本未覆盖的面：

1. **像素级解码**：8bpp 索引经 `g_DefColorTable` 展开到 ARGB，逐点断言；
2. **BMP 行序**：自下而上 BMP 被翻正为自上而下（原文 `ScanLine[I]` 语义）；
3. **BMP 失败路径**：非 `BM` 魔数、头长度不足 → `null`；
4. **`LoadDxGrayImage` / `LoadDxBrightImage` 正向分支**：三槽位打点互不干扰、纹理对象互不相同；
5. **`D3DFormat` 大图分支**（≥400×400）形状断言；
6. **缓存入口的未初始化 / 越界早退**（7 个入口 × 两类早退）；
7. **二次访问命中缓存**（不再 `Add`、不再装载）；
8. **UpdateEngine 接缝的另外三条路径**（Gray / Bright / ImageSize）与三种「不触发」条件
   （引擎拒绝受理 / 设备未就绪 / `boUpdateStop` 已置）。

---

## 4. 发现的原文缺陷 / 易错点

| # | 位置 | 现象 | 本移植处置 |
|---|---|---|---|
| 1 | `368-407 GetCachedImageSize` | 函数体把 `m_ImgArr[AIndex].nWidth/nHeight/nPx/nPy` 当结果返回，但 `LoadDxImage` **从不回填**这四个字段，故 `ASize/APoint` 恒为 0。原文自带注释承认「未经过验证，程序中也未使用」 | 照抄；测试断言恒 0 并注明来源 |
| 2 | `484-508 GetSurfaceByName` → `GetIndexByName` | `GetIndexByName` 未命中会把新名字**追加**进 `m_FileList`，使 `Count` 超过 `Initialize` 时分配的 `m_ImgArr` 长度；随后 `m_ImgArr[Index]` 在 Delphi 里是**越界读**（未定义行为） | 加显式边界（`Index >= m_ImgArr.Length` → 返回 nil）并注释：原文此处为越界读 |
| 3 | `298-329 GetCachedBright` | 守卫（`index`/`Initialized`）放在 `Lock` **之外**，与 Surface/Gray 的写法不一致 | 照抄同一结构 |
| 4 | `232-263 / 331-366` | `IndexList.Add` 在**装载之前**无条件执行（装载失败也留痕），而 `GetCachedBitmap`（685-707，属 Wis）是装载后才加 | 照抄 |
| 5 | `DrawZoom/DrawZoomEx` 等 | `Uib.pas` 未覆写 `GetBitmap`，继承基类的 `Result := nil`（本车道基类同语义） | 不额外实现 |
| 6 | `Source.Width * Source.Height > 4` | 注意是**严格大于 4**：1×2、2×2 的图会被整体跳过（槽位保持 nil，**不是** NULLTexture） | 照抄；测试用「面积 ≤ 4 早退」锁定 |

---

## 5. 未完成 / 留作接缝

1. `BitmapFileSeams.LoadFromFileFn` 的默认实现只支持**无压缩 BMP**（含 8/16/24/32bpp）；
   `DIB.pas` 全量移植后应替换为真实 `TDIB.LoadFromFile`（支持 RLE、PCX、DIB 变体等）。
   之所以给默认实现而不是空接缝：本单元的测试需要**可运行的**端到端路径。
2. 8bpp BMP 自带调色板被**有意忽略**（原文 `Uib.pas` 全程使用全局 `g_DefColorTable`，
   不走 `TDIB.LoadFromFile` 读进来的调色板）；接入真实 `DIB.pas` 时需复核这一行为。
3. `FileData32/FileDataGray32/FileDataBright32`（`GameImages.pas`）仍为接缝：
   `D3DFormat = True` 时本移植与普通分支走同一 `TextureSeams.*Fn`，
   在真实实现接入前，D3D 路径的**纹理格式**不保证与原文一致（形状/尺寸一致）。
4. `GameImages.pas TGameImages.FreeOldMemorys_Ex` 的 LRU 回收策略仍为空实现（`Wzl`/`Wis` 车道既定接缝）。

---

## 6. 门禁原始输出（摘要）

```
$ dotnet build GXX.slnx -c Debug --nologo
    71 个警告
    0 个错误

$ dotnet test tests\GXX.Client.Tests\GXX.Client.Tests.csproj -c Debug --nologo
已通过! - 失败:     0，通过:  2345，已跳过:     0，总计:  2345
```
