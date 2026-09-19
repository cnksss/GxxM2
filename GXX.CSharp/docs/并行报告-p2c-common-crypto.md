# 并行报告 · 车道 `p2c-common-crypto`（Source/Common 尾部单元）

> 车道：`par/p2c-common-crypto` ｜ 工作树：`.worktrees/p2c-common-crypto` ｜ 基线：`main @ 5bd12133`
> 范围：`Source/Common` 尾部 10 个单元（≈3,700 行）
> 门禁（本工作树内实测）：`dotnet build GXX.slnx -c Debug` **0 error**；
> `GXX.Core.Tests` **230 passed / 0 failed**；`GXX.M2Server.Tests` **5053 passed / 0 failed**。

---

## 1. 分支与提交

| # | commit | 内容 |
|---|---|---|
| 1 | `4923c8d2` | `Crypto/AESUtils.cs` + `CryptoAesTests.cs` |
| 2 | `fc6e10c3` | `Util/HashUnit.cs` + `Crypto/DesUnit.cs` + `DesUnit.Tables.g.cs` + `CryptoDesUnitTests.cs` |
| 3 | `8ca02cd3` | `Util/HashObjList.cs` + `Util/CheckUnit.cs` |
| 4 | `ebc6914d` | `Compress/ZipUnit.cs`、`Compress/CompressUnit.cs`、`UpdateCommon.cs`、`Stubs/MemoryModule{,Def}.cs`、`CommonTailTests.cs` |

全部提交**只新增文件**，未修改任何既有 `src/**`、`*.csproj`、`GXX.slnx`、`Directory.Build.props`、`tools/**`、`docs/并行*.md`。工作树内无临时探查目录残留（生成脚本与探针都在 `%TEMP%` 下）。

---

## 2. 逐单元覆盖判定表

| # | Delphi 源 | 行 | 判定 | 证据 |
|---|---|---|---|---|
| 1 | `Common\AESUtils.pas` | 1218 | **新建**（第三方算法，按 §2.2 用系统库等价实现） | 原文头 `AESUtils.pas:3-13` 自述"摘自 Synopse framework - SynCrypto.pas"；源码树中仅 `Pak.pas:19` 引用 → `Crypto/AESUtils.cs` |
| 2 | `Common\DesUnit.pas` | 824 | **新建（有差异）** | 既有 `Crypto/UnitDes.cs:8` 头部写的是"UnitDes.pas 1:1"、`UnitDes.cs:13` 明示 `BS = 20`；本单元是**表驱动位数组 DES + SHA-1 倍增密钥 + Level=8 轮**，两者 S 盒/轮数/密钥调度/填充全不同 → `Crypto/DesUnit.cs`，**未覆盖**既有文件 |
| 3 | `Common\HashUnit.pas` | 206 | **新建**（部分已被间接覆盖，但签名不兼容） | 既有 `Crypto/UnitDes.cs:17-23` 的 `Hash` 是同一 SHA-1 但签名为 `(string,byte[])` 且仅作 UnitDes 内部依赖；本单元被 `DesUnit.pas:52` 直接 `uses` → `Util/HashUnit.cs`（独立 1:1 移植，测试中与 `UnitDes.Hash` 逐字节互证） |
| 4 | `Common\HashObjList.pas` | 258 | **新建（有差异）** | 既有 `Util/HashList.cs:8-9` 头部写的是"HashList.pas 1:1"；`HashList.cs:62` 的 `HashOf` 是 `(h<<5)^(h>>27)^byte` 且带全局有序链+临界区，本单元用 **CRC16 采样哈希 + 头插 + 无有序链** → `Util/HashObjList.cs`，**未覆盖**既有文件 |
| 5 | `Common\CheckUnit.pas` | 127 | **部分已覆盖 + 新建** | `BufferCRC`/`StringCrc`（`CheckUnit.pas:24-34`）只是 `CheckCrc.Crc32` 薄包装，既有 `Crypto/UnitDes.cs:405-411` 的 `CalcCrc32` 即同一 zlib CRC-32 → 本文件**转调**；`FileCrc`/`CalcFileCRC`/`HashPJW`/`CalcBufferCRC` 在 `src/GXX.Core/**` 无对应 → 新建于 `Util/CheckUnit.cs` |
| 6 | `Common\HashObjList.pas` 的 `HashIndex` | — | 新建 | 同上文件（`HashObjListCrc16.HashIndex`） |
| 7 | `Common\Compress\ZipUnit.pas` | 45 | **已覆盖（包装层新建）** | 既有 `Compress/ZlibEx.cs` 已实现 `CompressBuf/DecompressBuf/CompressBufZ/DecompressBufZ`；本单元只是 4 个转调 → `Compress/ZipUnit.cs` |
| 8 | `Common\Compress\CompressUnit.pas` | 28 | **有差异（接缝）** | `CompType=2` 转调既有 `ZlibEx`；`CompType=1` 走 **RLEUnit.pas（6,648 字节，本波次未移植）** → 显式 `NotSupportedException` 接缝；`CompressUnit.pas` 在源码树**零调用方**（唯一提及是 `Pak.pas:224` 被注释掉的 `//CompressUnit,`） |
| 9 | `Common\UpdateCommon.pas` | 56 | **新建** | 源码树中**零调用方**；常量 + 3 个 packed record → `UpdateCommon.cs` |
| 10 | `Common\MemoryModule.pas` | 713 | **Stub（§2.3 不移植项）** | `Stubs/MemoryModule.cs`：保留全部方法签名 + 6 个纯算术工具（1:1 实现），原生 PE 加载方法抛 `NotSupportedException` |
| 11 | `Common\MemoryModuleDef.pas` | 177 | **Stub（定义层）** | `Stubs/MemoryModuleDef.cs`：常量 + 12 个 PE 结构（`Pack=1` + SizeOf 断言） |

> 说明：`DesUnit.pas` / `HashObjList.pas` / `UpdateCommon.pas` 在 **整个源码树中零调用方**（grep `\b<unit>\b` 结果：DesUnit 0、HashObjList 0、UpdateCommon 0）。`MemoryModule`/`MemoryModuleDef` 的调用方是 `ClMain.pas`/`MShare.pas`（客户端，未在本波次）。
> 调用方最多的三个：`CheckUnit`（24 个文件，含 M2Server / DBServer / RunGate / LoginGate）、`ZipUnit`（`UpdateEngine.pas`、`LocalDB.pas`）、`AESUtils`（`Pak.pas`）。

---

## 3. 新增文件清单与行数

| 文件 | 行 |
|---|---|
| `src/GXX.Core/Crypto/AESUtils.cs` | 429 |
| `src/GXX.Core/Crypto/DesUnit.cs` | 364 |
| `src/GXX.Core/Crypto/DesUnit.Tables.g.cs` | 69（**脚本生成**） |
| `src/GXX.Core/Util/HashUnit.cs` | 104 |
| `src/GXX.Core/Util/HashObjList.cs` | 195 |
| `src/GXX.Core/Util/CheckUnit.cs` | 89 |
| `src/GXX.Core/Compress/ZipUnit.cs` | 90 |
| `src/GXX.Core/Compress/CompressUnit.cs` | 90 |
| `src/GXX.Core/UpdateCommon.cs` | 113 |
| `src/GXX.Core/Stubs/MemoryModule.cs` | 109 |
| `src/GXX.Core/Stubs/MemoryModuleDef.cs` | 174 |
| `tests/GXX.Core.Tests/CryptoAesTests.cs` | 274 |
| `tests/GXX.Core.Tests/CryptoDesUnitTests.cs` | 346 |
| `tests/GXX.Core.Tests/CommonTailTests.cs` | 419 |

### 表数据抽取（禁止手工转录）

`Crypto/DesUnit.Tables.g.cs` 由脚本从 `Source/Common/DesUnit.pas` 抽取（Delphi 源为 GBK，按 `Encoding.Default` 读取），
抽取时校验元素个数与声明上界一致（tblS 的 `array[0..7,0..3,0..15]` 展平为 512 项），
并把 SHA-256 指纹写进测试断言（`TableFingerprints_AreStable`）：

```
IP    count=64  sha256=8792407d850b9150203af58a1aef5cc475038e3472bbade1a7db5f22512f581b
UnIP  count=64  sha256=8a57e119ba7a71e0a5751bbf59700f6fdc26ffec295680f44f63c61c5a525b03
E     count=48  sha256=bb8d87ef4dc93e39aba7e0edf9135764379aaa05fbc79bc907c6a15f116cbdf5
S     count=512 sha256=0325e6f9ad57f0f38af25ed08f733f0e85e2c2c818de03e9ef9d6951bd59f72f
P     count=32  sha256=861ebba16456ae91815611c4cf4bb4957e5952b4a882e02c48df85920b093402
```

`AESUtils.cs` 的 10 张表（SBox/InvSBox/Te0..3/Td0..3）由原文 `ComputeAesStaticTables`（`AESUtils.pas:1229-1282`）**运行时生成**，非转录；
`HashObjListCrc16.Crc16Table`（256 项）由多项式 `$1021` **生成**并逐值与原文常量比对，非转录。

---

## 4. 黄金向量的来源与实测

### 4.1 AESUtils（SynCrypto AES-CTR，第三方 → 系统库等价实现）

原文自述是 Synopse SynCrypto 的 AES-CTR 移植（`AESUtils.pas:3-13`），故按 `转换开发文档.md` §2.2 用
`System.Security.Cryptography` 等价实现，并用三类向量证明互通：

| 类 | 来源 | 实测 |
|---|---|---|
| V1 单分组 | **FIPS-197** C.1/C.2/C.3（AES-128/192/256） | `69c4e0d86a7b0430d8cdb78070b4c55a` / `dda97ca4864cdfe06eaf70a0ec0d7191` / `8ea2b7ca516745bfeafc49904b496089` ✅ |
| V2 CTR 密钥流 | **NIST SP 800-38A F.5.1**（key=`2b7e15…4f3c`，counter=f0f1…feff） | 首块密钥流 `ec8cdf7398607cb0f2d21675ea9ea1e4` ✅ |
| V3 互通 | 与 .NET 内建 `Aes`（ECB 单分组）**逐字节**一致（128/192/256 三种密钥长度） | ✅ |
| V4 自推向量 | 原文计数器块 = **16 字节全零**（`AESUtils.pas:1178`），自增点固定在下标 7 | 4 块密钥流 `7df76b0c…` / `dc0a3bc3…` / `d4ccbed3…` / `37abeec0…` ✅ |

**推导依据**：原文 `DoAESEncrypt`（`AESUtils.pas:1171-1205`）先 `FillChar(Block,0)` 再逐块 `DoBlock` + 手工大端自增（下标 7 起），
所以第 1 个密钥流块 = `AES(全零块)`，第 2 块 = `AES(byte7=1)` …… 这正是上表 V4 的四组值。
`AESDecrypt` 与 `AESEncrypt` 内部**完全相同**（`AESUtils.pas:1218-1227`），已用自反断言锁定。

### 4.2 HashUnit（SHA-1）

FIPS 180-1 标准向量：`"abc"`→`a9993e36…`、`""`→`da39a3ee…`、
56 字节边界 `"abcdbcde…nopq"`→`84983e441c3bd26ebaae4aa1f95129e5e54670f1`、
64 字节整块 `"a"×64`→`0098ba824b5c16427bd7a1122a5a442a25ec644d`；并与既有 `UnitDes.Hash`（.NET SHA1）互证。

### 4.3 DesUnit（表驱动 DES 变体）

| 类 | 内容 | 实测 |
|---|---|---|
| V1 | SHA-1 密钥派生第一步 | 同 4.2 ✅ |
| V2 | `GetBits`/`SetBits` 位展开（MSB 在前、可逆、边界 `0x00/0x01/0xA5/0xFF`） | ✅ |
| V3 | **子密钥 K1**：与公开标准 DES 教学例（key=`133457799BBCDFF1`）的 `K1 = 1B02EFFC7072` 完全一致 | ✅ |
| V4 | `DES` 与 `UNDES` **互逆**（cLoop = 1/2/4/8/16） | ✅ |
| V5 | `tblS` 与 FIPS 46-3 逐值比对（1 处差异） + 5 张表 SHA-256 指纹 | ✅ |
| V6 | 端到端 `EncryptDes`/`DecryptDes`：固定 `RandomFunc` 后往返 + 长度规则 + 确定性 | ✅ |

---

## 5. 测试用例数与门禁结果

| 工程 | 用例 | 结果 |
|---|---|---|
| `GXX.Core.Tests` | **230**（本波次新增 **68**：CryptoAesTests 15、CryptoDesUnitTests 23、CommonTailTests 30） | **0 failed** |
| `GXX.M2Server.Tests` | **5053** | **0 failed** |
| `dotnet build GXX.slnx -c Debug` | — | **0 error**（86 warning，全部为既有文件） |

---

## 6. 发现的原文缺陷 / 易错点（均已注释保留或按需修正，逐条留痕）

1. **`HashUnit.pas` 的 SHA-1 在 `Index == 56` 时把 `$80` 覆盖**（`HashUnit.pas:210-217`）：
   `HashBuffer[Index] := $80` 写在 `Index=56`，紧接着 `PDWord(@HashBuffer[56])^ := …` 把长度写进 `[56..63]`，
   两者**共用同一字节区间**。因为中间有 `if Index >= 56 then Compress` 先把补位块压掉，原算法恰好正确。
   本移植用"先算补位长度、再逐块拼"的等价写法；**如果照字面顺序写就会算错**（我在实现期踩过这个坑）。
2. **`DesUnit.pas` 的 `EncryptDes` 即使已对齐也多分配一块**（`DesUnit.pas:750`，`(DataSize div 8 + 1) * 8`）：
   `DataSize = 8` 时返回 16。已按原文保留并加断言。
3. **`DesUnit.pas` 的填充是随机的**（`DesUnit.pas:754` `Random(255)`）：输出不确定。
   本移植提供 `DesUnit.RandomFunc` 钩子，测试固定随机源后锁定行为；默认仍与原文同分布（`[0,254]`）。
4. **`DesUnit.pas` 的 `FindByteBack($FF)` 会误截断**（`DesUnit.pas:803`）：明文自身含 `$FF` 时解密长度偏小；
   未命中返回 **-1**（不是 0）。
5. **`DesUnit.pas` 的 `HexToInt` 静默跳过非法字符**（`DesUnit.pas:855` 的 `raise` 被注释掉）：
   `"1g"` → `0x1`（不是 `0x10`，也不抛异常）。测试已按实际行为锁定。
6. **`HashObjList.pas` 的 `Add` / `Modify` 从不给 `Result` 赋值**（`HashObjList.pas:205-231`）：
   Delphi 下**恒返回 False**；`Modify` 还只做 `P := Find(Name)^`，**无任何副作用**。
   本移植照抄（返回 false + 不改值），并用差异断言把该行为钉住，防止后续被"顺手修正"。
7. **`HashObjList.pas` 的 `CRC16` 在 ≥32 字节时只采样**（`HashObjList.pas:99-106` `Step := iCount div 32 + 1`）：
   不是全量 CRC；测试用"改动未采样字节结果不变"做差异断言。
8. **`CheckUnit.pas` 的 `CalcFileCRC` 不是 CRC**（`CheckUnit.pas:72-104`）：
   只是"长度向下取整到 4 倍数的 DWord 异或和"，且丢弃尾部不足 4 字节的部分；
   `if nFileHandle = 0` 恒假（`FileOpen` 失败返回 -1）。已按原文保留并加差异断言。
9. **`CheckUnit.pas` 的 `HashPJW` 用有符号 `Longint` 高位掩码**（`CheckUnit.pas:66`）：按 `unchecked` 语义逐位照抄。
10. **`AESUtils.pas` 的计数器自增循环含死代码**（`AESUtils.pas:1186-1193`）：
    `repeat … if (Block[Offset] <> 0) or (Offset = 7) then break;` —— `Offset` 已 `Dec` 过，`Offset = 7` 恒假。
    实测该条件在任何输入下都不影响结果（等价于标准大端进位），已在计数器块级断言中钉住
    （第 255/256 块计数器 = `00000000000000ff…` / `0000000000000100…`）。
11. **`AESUtils.pas` 的 `AESDecrypt` 与 `AESEncrypt` 实现完全相同**（`AESUtils.pas:1218-1227`）：
    CTR 下成立，已用自反断言锁定。
12. **`AESUtils.pas` 的 `KeyBufLen` 大于密钥长度时不做上界检查**（`AESUtils.pas:1140-1144`）：
    原文会越界拷贝；本移植做了数组上界钳制（托管侧必须），并在文件头登记该偏差。
13. **`ZipUnit.pas` / `CompressUnit.pas` 的 `OutBytes` 是出参**：`CompressBuffer` 的**返回值是原始长度**而非压缩后长度
    （`CompressUnit.pas:15-18`）。C# 无出参指针，直接按返回值截断会切掉 raw-deflate 流的末尾 2 字节
    （本波次实际踩到：`ZlibEx.DecompressBufZ(inData, rawSize)` 把输入长度当成输出长度，末尾字节丢失）。
    已修正为传 `inData.Length`，并在 `CompressUnit.cs` 注释说明该接缝。
14. **`CompressUnit.pas` 的 `CompType=1` 依赖未移植的 `RLEUnit.pas`**：已显式 `NotSupportedException` 接缝，
    不静默返回错值（该单元零调用方，不影响任何现有路径）。

---

## 7. 接缝 / 未完成（**重要，请集成者判读**）

1. **`DesUnit` 端到端未能证明等于"标准 DES"**：
   - 已证：子密钥 K1 与公开标准 DES 教学例一致；`DES`/`UNDES` 严格互逆；
     `tblS` 与 FIPS 46-3 逐值比对（仅 1 处差异）；位展开/表指纹锁定。
   - **未证**：本波次未能用 FIPS 46-3 公开向量（如 key=`133457799BBCDFF1`, plain=`0123456789ABCDEF`
     → `85E813540F0AB405`）证明生产 DES 端到端等于标准 DES。原文的 386 汇编轮结构
     （128 字节 `Bits` 缓冲 + `Current`/`Next` 指针交换 + `SUB EBX,48` 倒序取密钥）与
     "标准 Feistel + 末轮交换"之间的逐轮等价性，我没有取得可复现的对照证据。
   - **影响面**：`DesUnit.pas` 在源码树中**零调用方**，故该未闭合项不影响任何现有路径；
     但若后续要拿它与外部 DES 工具互通，必须先补上端到端向量验证。
   - **本波次的处置**：只写**可证**性质的断言（互逆、确定性、表内容、子密钥），
     **不写**"等于标准 DES"的断言；`UnitDes.cs`（另一套算法）保持不动。

2. **`RLEUnit.pas` 未移植**（6,648 字节）：`CompressUnit.pas` 的 `CompType=1` 分支以
   `NotSupportedException` 显式接缝。`CompressUnit.pas` 零调用方，风险低。

3. **`MemoryModule` 的原生 PE 加载**按 `转换开发文档.md` §2.3 判为不移植项：
   全部方法签名保留、纯算术工具 1:1 实现、加载路径抛 `NotSupportedException`。
   托管侧等价能力是 `AssemblyLoadContext`（`M2Engine/PluginManager` 侧），本波次未涉及。

4. **`TPakKey` / `TSocketBuffer` 的 `BytesOf/FromBytes`**：`TSocketBuffer` 已提供并按小端断言；
   `TPakKey` 只断言了 `SizeOf = 178`（固定缓冲字段的 `BytesOf` 未实现，因为源码树中无调用方）。

5. **`DesUnit` 的 `UNDES` 与原文汇编的密钥取用方向**：
   原文汇编里 `DES` 与 `UNDES` **都**从密钥缓冲末尾往前取（各自的 `KeysAddr` 初值不同，
   但循环体里都是 `SUB EBX, 48`）。若严格照抄，两者**不是**彼此的逆，加解密无法还原明文。
   本移植让 `DES` 倒序、`UNDES` 正序取子密钥（标准 Feistel 解密要求），
   从而让 `EncryptDes`/`DecryptDes` 真正互逆 —— 这是**与原文汇编的刻意偏差**，已在文件注释中登记。

---

## 8. 给集成者的一句话

本车道**只新增文件**，可安全 `git merge --no-ff par/p2c-common-crypto`；
唯一需要人工判读的是第 7 节第 1 条（`DesUnit` 端到端未与标准 DES 对齐，但该单元零调用方）。
