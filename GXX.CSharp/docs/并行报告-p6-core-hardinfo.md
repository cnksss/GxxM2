# 并行报告 — 车道 `p6-core-hardinfo`（移植 `HardInfo.pas`，898 行）

- 源单元：`Source/Common/HardInfo.pas`（GBK，898 行，Delphi 7 / Win32；被 `Client.dpr:69` 与 `M2Share.pas:8446` 使用）
- 车道工作树：`D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p6-core-hardinfo`（分支 `par/p6-core-hardinfo`）
- 产出位置：`GXX.CSharp/src/GXX.Core/Hardware/**`（新目录）、`GXX.CSharp/tests/GXX.Core.Tests/Hardware*.cs`、本报告
- 未改任何 `csproj`/`slnx`（新增 `.cs` 自动纳入编译）；未改任何既有文件

---

## 1. commit 一览

| # | hash | 内容 |
|---|---|---|
| 1 | `27783d6e8d2009ff894f9fd3ff59d1e7e24789df` | `p6-core-hardinfo: 移植 HardInfo.pas 类型面+接缝层+1:1 本体`（3 文件） |
| 2 | `42931544171b8c9bf5fc591f158ee30b321081be` | `p6-core-hardinfo: 165 例测试（布局锁定 / 纯逻辑 / 接缝行为）`（4 文件） |
| 3 | `74f3859a` (见 `git log`) | `p6-core-hardinfo: 补 Win9x 分支的 SCIP 落位保真（原文 :551 @Buffer 首址）`（2 文件） |
| 4 | 本文件所在 commit（`p6-core-hardinfo: 并行报告`） | 仅新增 `docs/并行报告-p6-core-hardinfo.md` |

（本车道全部提交均为**可合并**，无 `WIP`；父提交 = `52d3e076 integrate par/p6-m2-playersurface`。）

---

## 2. 逐函数判定表

| 原文行号 | 例程 / 声明 | 判定 | 托管对应 |
|---|---|---|---|
| :13-18 | `TRegisters` | **已完成**（类型 + 布局断言） | `HardInfoTypes.TRegisters` |
| :19 | `TCPUID`（1-based 数组） | **已完成**（原文死类型；1-based 语义 + 越界断言） | `HardInfoTypes.TCPUID` |
| :21 | `GetWindowsVersion` | **已完成** | `HardInfo.GetWindowsVersion` + `FormatWindowsVersion` |
| :22/:215-231 | `GetDisplayDevice` | **已完成** | `HardInfo.GetDisplayDevice` + `MatchesPrimaryDisplayName` |
| :23/:207-213 | `GetDisplayFrequency` | **已完成** | `HardInfo.GetDisplayFrequency` |
| :26/:46-184 | `GetIdeSerialNumber`（返回 `pchar`） | **已完成** | `HardInfo.GetIdeSerialNumber`（返回 `string`，语义=取到 `#0`） |
| :27/:396-595 | `GetIdeDiskSerialNumber` | **已完成** | `HardInfo.GetIdeDiskSerialNumber` + `DecodeIdSector` |
| :34/:387-394 | `GetCpuIDstringEx` | **已完成** | `HardInfo.GetCpuIDstringEx` + `FormatCpuIDstringEx` |
| :35/:664-705 | `GetAdapterMac` | **已完成** | `HardInfo.GetAdapterMac` + `FormatAdapterAddress6` |
| :37/:305-324 | `GetCpuName`（`{$IFDEF M2SERVER}`） | **已完成**（托管侧无条件提供） | `HardInfo.GetCpuName` + `AssembleCpuName` |
| :38/:327-382 | `GetCPUCount`（`{$IFDEF M2SERVER}`） | **已完成** | `HardInfo.GetCPUCount` + `FormatCPUCount` |
| :39/:287-303 | `GetMemorySize`（`{$IFDEF M2SERVER}`） | **已完成** | `HardInfo.GetMemorySize` + `FormatMemorySize` |
| :128-142 / :487-501 | `ChangeByteOrder`（**两份完全相同**的内嵌例程） | **已完成（纯函数 100%）** | `HardInfo.ChangeByteOrder`（两处共用一份 + `offset` 重载） |
| :233-251 | `CountSetBits` | **已完成（纯函数）**，缺陷锁定 | `HardInfo.CountSetBits` |
| :253-284 | `GetCPUID`（内联 `asm`） | **接缝**（x86/x64 机器码不可移植） | `ICpuIdApi.GetCPUID` |
| :707-773 | `GetNetCardName`（含内嵌 `RegEnum`，**原文未导出**） | **已完成**（可测性改 `public`，差异已登记） | `HardInfo.GetNetCardName` + `RegEnum` + `SplitTextStr` |
| :775-813 | `GetNetCardMac`（**原文未导出**） | **已完成** | `HardInfo.GetNetCardMac` + `FormatNdisAddressHex` |
| :815-831 | `FormatMac`（**原文未导出**） | **已完成（纯函数）** | `HardInfo.FormatMac` |
| :186-204 | S.M.A.R.T. 说明注释块（19 行） | **非代码**，未搬迁（见 §7 差异登记 6） | — |
| :833-897 | `GetHwid2` + `initialization`/`finalization`（`{ ... }` **整段被原文注释**） | **不移植**（不在编译单元内），原文照抄保留在 `HardInfo.cs` 末尾注释块 | — |

> ⚠ 派发提示词称该单元有 `THardInfo` 类型 —— **原文不存在**。
> `git grep -E "THardInfo" Source/**/*.pas` 零命中；该单元只有 `TRegisters`/`TCPUID` 两个记录 + 12 个单元级函数。
> 另：`GetIdeSerialNumber`/`GetIdeDiskSerialNumber` … 等 12 个函数名在 `HardInfo.pas` 之外**只**被
> `MShare.pas:3554/13377/13383`（客户端）与 `M2Share.pas:8450`（服务端 `GetSerialNumber()`）引用。

---

## 3. 新增文件 + 覆盖行号范围

| 文件 | 行数 | 说明 |
|---|---|---|
| `src/GXX.Core/Hardware/HardInfoTypes.cs` | ~590 | 原文类型面 + 接缝契约结构 + `HardInfoLayout` 偏移常量 |
| `src/GXX.Core/Hardware/HardInfoSeams.cs` | ~390 | 9 个接缝接口 + `THardInfoRuntime` + `THardInfoNullOs`（零 P/Invoke） |
| `src/GXX.Core/Hardware/HardInfo.cs` | ~900 | 原文 1:1 本体（含被注释的 `GetHwid2` 保留块） |
| `tests/GXX.Core.Tests/HardwareLayoutTests.cs` | ~300 | `Marshal.SizeOf`/`OffsetOf` 布局锁定 |
| `tests/GXX.Core.Tests/HardwarePureLogicTests.cs` | ~520 | 纯逻辑 + 差异断言 |
| `tests/GXX.Core.Tests/HardwareSeamTests.cs` | ~810 | 接缝行为（可编程替身） |
| `tests/GXX.Core.Tests/HardwareTestDoubles.cs` | ~330 | 接缝替身 |
| `docs/并行报告-p6-core-hardinfo.md` | 本文件 | — |

### 覆盖行号范围（原文 898 行）

| 原文区间 | 状态 |
|---|---|
| `:11-19`、`:21-40` | **已覆盖**（类型面与接口面） |
| `:44-184` | **已覆盖**（`GetIdeSerialNumber` + 第 1 份 `ChangeByteOrder`） |
| `:186-204` | 未覆盖（**说明性注释**，非代码；`S.M.A.R.T.` 参考资料链接） |
| `:205-251` | **已覆盖**（`GetDisplayFrequency`/`GetDisplayDevice`/`CountSetBits`） |
| `:252-284` | **接缝**（`$IFDEF M2SERVER` + `GetCPUID` 内联 asm）——调用签名/叶号映射/默认实现被覆盖，**asm 本体不可移植亦不可断言** |
| `:285-382` | **已覆盖**（`GetMemorySize`/`GetCpuName`/`GetCPUCount`） |
| `:383-384` | `{$ENDIF}`（预处理标记） |
| `:386-595` | **已覆盖**（`GetCpuIDstringEx` + `GetIdeDiskSerialNumber` + 第 2 份 `ChangeByteOrder`） |
| `:596-831` | **已覆盖**（`GetWindowsVersion`/`GetAdapterMac`/`GetNetCardName`/`GetNetCardMac`/`FormatMac`） |
| `:833-897` | **不移植**（整段被原文 `{ }` 注释掉：`GetHwid2` + `initialization`/`finalization`） |

- **无托管对应的原文代码行**：`:186-204`（19 行说明注释）、`:253-284`（32 行内联 asm）、`:833-897`（65 行被注释代码）——合计 116 行。
- **托管侧新增的、原文没有的成员**（全部在注释里标注为新增，非原文）：
  `HardInfoLayout`（偏移常量）、`MatchesPrimaryDisplayName`、`FormatWindowsVersion`、`FormatMemorySize`、`AssembleCpuName`、
  `FormatCPUCount`、`FormatCpuIDstringEx`、`DecodeIdSector`、`FormatAdapterAddress6`、`FormatNdisAddressHex`、
  `SplitTextStr`、`ChangeByteOrderLoopCount`、`DelphiRound`、`WriteLittleEndian`、`ReadUInt32LE`/`ReadUInt16LE`、
  `BuildScsiMiniportIdentifyRequest`/`BuildScsiIdentifySendCmdIn`/`WriteStruct`、`SetCallName`、`LeftStr`/`RightStr`、
  各结构上的 `Set*/String()` 辅助（标注"测试/接缝辅助（非原文成员）"）。
- **未覆盖的托管成员**：`HardInfo.EnsureIdentifyBuffer`（私有防御守卫）——
  原始缓冲由本移植按原文固定尺寸自行分配（528 / 572 字节），`offset + 256 ≤ 长度` 恒成立 ⇒
  **当前不可达**，无测试。保留它只为在将来接缝实现违约时给出明确错误而不是静默越界读。

---

## 4. 接缝清单 + 精确签名（供集成方接真实实现）

### 4.1 装配点

```csharp
// GXX.Core.Hardware.THardInfoRuntime —— 与 GXX.Core.Async.TAsyncCallRuntime 同一模式
public THardInfoRuntime(
    IVersionApi version, IDisplayApi display, ICpuIdApi cpuId, IMemoryApi memory,
    IProcessorInformationApi processor, IDiskIdentifyApi disk, INetBiosApi netBios,
    INdisQueryApi ndis, IRegistryApi registry);
public static THardInfoRuntime Default { get; }   // = 全部能力由 THardInfoNullOs 承接（失败/空）
```

每个公开入口的最后一个形参都是 `THardInfoRuntime runtime = null`（`null` ⇒ `Default`）。
**默认实现不做任何真实 OS 调用**（`git grep DllImport -- 'GXX.CSharp/src/GXX.Core'` 仍为零命中）。

### 4.2 逐接缝签名

```csharp
/// 原文 :606 GetVersionEx(VersionInfo)（Windows.pas / SysUtils）
public interface IVersionApi {
    bool GetVersionEx(ref TOSVersionInfo versionInfo);
}

/// 原文 :211 EnumDisplaySettings(nil, Cardinal(-1), DeviceMode) / :224 EnumDisplayDevices(nil, cc, lp, 0)
public interface IDisplayApi {
    bool EnumDisplaySettings(uint iModeNum, ref TDeviceMode lpDevMode);          // iModeNum = $FFFFFFFF (ENUM_CURRENT_SETTINGS)
    bool EnumDisplayDevices(uint dwDevNum, ref TDisplayDevice lpDisplayDevice, uint dwFlags);
}

/// 原文 :253-284 procedure GetCPUID(Param: Cardinal; var Registers: TRegisters);（内联 DB $0F,$A2）
public interface ICpuIdApi {
    void GetCPUID(uint Param, out TRegisters Registers);   // ★ 原文 asm 忽略 Param（见 §6 D1）
}

/// 原文 :293 GlobalMemoryStatusEx(oMemoinfo)
public interface IMemoryApi {
    bool GlobalMemoryStatusEx(ref TMemoryStatusEx buffer);
}

/// 原文 :344 GetLastError / :391 GetSystemInfo / :342+:348 GetLogicalProcessorInformation
public interface IProcessorInformationApi {
    bool GetLogicalProcessorInformation(SYSTEM_LOGICAL_PROCESSOR_INFORMATION[] buffer, ref uint returnLength);
    uint GetLastError();
    void GetSystemInfo(out TSystemInfo lpSystemInfo);
}
// ★ 接缝实现必须用 HardInfoLayout.SizeOfSystemLogicalProcessorInformation(=24) 做"元素数↔字节数"换算，
//   否则原文 :346/:360 的 `div sizeof` 会算错。

/// 原文 :145-152 + :173-174（PhysicalDrive0 / SMARTVSD）+ :506-540（'\\.\Scsi0:' + IOCTL_SCSI_MINIPORT）
/// ★ 请求结构（SCIP / SRB_IO_CONTROL / Buffer）的构造**留在移植单元内**（属原文逻辑、已被断言），
///   接缝只承担 CreateFile + DeviceIoControl + CloseHandle 本身。
public interface IDiskIdentifyApi {
    uint Win32Platform { get; }                                                    // SysUtils.Win32Platform（:145/:506）
    bool PhysicalDriveIoControl(TSendCmdInParams sendCmdIn, int sendCmdInSize,
                                byte[] idOutCmd, int outOffset, int outSize, out uint bytesReturned);
        // 原文取值：sendCmdInSize=32(:173 `SizeOf(TSendCmdInParams)-1`)、outOffset=0、outSize=528(:125)
    bool ScsiMiniportIoControl(byte[] buffer, int bufferSize, out uint bytesReturned);
        // buffer=572(:477 BufferSize)、SRB 头已填（'SCSIDISK'/$001B0501/Timeout=2/Length=544）、SCIP 在 +28
    bool SmartVsdIoControl(TSendCmdInParams sendCmdIn, int sendCmdInSize, byte[] buffer,
                           int outOffset, int outSize, out uint bytesReturned);
        // GetIdeSerialNumber     : SCIP 独立（:152+:173 传 @SCIP），outOffset=0  / outSize=528
        // GetIdeDiskSerialNumber : SCIP **同时写在 buffer 首址**（:551 `@Buffer`），outOffset=32 / outSize=528(W9xBufferSize)
        //                           ← @pInData^.bBuffer；即 IDENTIFY 落在 +48
}

/// 原文 :678/:681/:686/:695 NetBios(@Ncb)（uses Nb30）
/// ★ 托管差异：原文把数据缓冲地址写在 NCB 里（:679 `ncb_buffer := @Lanaenum`、:693 `:= @Adapter`）。
///   托管侧本移植**仍把真实地址写进 ncb_buffer**（用 fixed 钉住数组），同时经第 2 个形参把数组传给接缝：
///   托管实现读 ncbBuffer，原生实现读 ncb.ncb_buffer。
public interface INetBiosApi {
    byte NetBios(ref TNCB ncb, byte[] ncbBuffer);          // 返回 0 = NRC_GOODRET
}

/// 原文 :792-799 CreateFile('\\.\'+NetCardName) + DeviceIoControl(IOCTL_NDIS_QUERY_GLOBAL_STATS=$00170002, @inBuf(4B OID), @outBuf(256B))
public interface INdisQueryApi {
    bool DeviceIoControlNdis(string deviceName, int ioControlCode, int oid, byte[] outBuffer, out uint bytesReturned);
        // 设备名已拼成 "\\\\.\\" + NetCardName；oid = OID_802_3_PERMANENT_ADDRESS = $01010101（小端 4 字节输入）
}

/// 原文 :719-745（RegOpenKeyEx/RegEnumKeyEx/RegEnumValue/RegCloseKey）+ :752-772（Registry.TRegistry）
public interface IRegistryApi {
    bool RegOpenKeyEx(nint rootKey, string name, uint samDesired, out nint hKey);          // samDesired = KEY_READ($20019)
    uint RegEnumKeyEx(nint hKey, int index, byte[] nameBuffer, ref uint nameLength);        // nameLength 出参 = 名称长度（不含 NUL）
    uint RegEnumValue(nint hKey, int index, byte[] nameBuffer, ref uint nameLength);
    void RegCloseKey(nint hKey);
    nint RegistryCreate();                                                                 // TRegistry.Create（默认 FAccess = KEY_READ）
    void RegistrySetRootKey(nint registry, nint rootKey);                                   // HKEY_LOCAL_MACHINE
    bool RegistryOpenKey(nint registry, string key, bool canCreate);
    string RegistryReadString(nint registry, string name);                                  // name = 'ServiceName'
    void RegistryCloseKey(nint registry);
    void RegistryFree(nint registry);
}
```

### 4.3 集成方接真实实现时的**结构布局注意**

| 结构 | 本声明（Delphi 7 / Win32） | 备注 |
|---|---|---|
| `TIdSector` / `TSendCmdInParams` / `TSendCmdOutParams` / `TSrbIoControl` / `TDriverStatus` / `TIDERegs` / `TMemoryStatusEx` / `TOSVersionInfo` / `TDeviceMode` / `TDisplayDevice` / `TLanaEnum` / `TAdapterStatus` | 与 Win32 逐字节一致（`Pack=1` 者按原文 `packed record`），**平台无关** | 已用 `Marshal.SizeOf/OffsetOf` 锁死 |
| `SYSTEM_LOGICAL_PROCESSOR_INFORMATION` | `ProcessorMask` 取 **4 字节**（Delphi 7 x86 的 `ULONG_PTR`）⇒ `SizeOf` 恒 24 = 原文 `sizeof` | x64 原生为 32 |
| `TNCB` | 含指针（`nint`）⇒ x86=64、x64=88；Win64 原生 NCB 为 96（`ncb_reserve` 18 字节） | **x64 下与原生不兼容**，真实实现须另声明 |
| `TSystemInfo` | 含 2 指针 ⇒ x86=36、x64=48（对齐补位）；`dwActiveProcessorMask` 为 4 字节 | **x64 下字段偏移与原生不同**，真实实现须另声明 |
| `TDeviceMode` | 只声明到 `dmDisplayFrequency`（偏移 120）；完整 DEVMODEA 为 156 字节 | 接缝实现若走原生 P/Invoke 需补全余下 32 字节 |

---

## 5. 测试用例数与门禁结果

| 项 | 结果 |
|---|---|
| 新增测试用例 | **165 例** = `HardwareLayoutTests` **20** + `HardwarePureLogicTests` **90**（含 Theory 展开）+ `HardwareSeamTests` **55** |
| `dotnet build GXX.slnx -c Debug` | **0 error**（147 warning，全部为既有 xUnit 分析器告警） |
| `dotnet test tests/GXX.Core.Tests` | **832 / 832 通过**（= 基线 667 + 新增 165） |
| 仅 Hardware 过滤 | **165 / 165 通过** |

### ⚠ 门禁的非确定性（与 HardInfo 无关，务必读）

基线 `GXX.Core.Tests` **不是稳定全绿**。两组独立证据：

**(a) 把本车道全部 7 个新文件移出工作树**（干净基线）后连跑 4 次：

```
run 1: failed=0 passed=667
run 2: failed=1 passed=666
run 3: failed=1 passed=666
run 4: failed=0 passed=667
```

**(b) 在本车道同一构建产物上，只跑基线 667 例**（`--filter "FullyQualifiedName!~Hardware"`，Hardware 测试不执行）连跑 6 次：

```
run 1: failed=1 passed=666
run 2..6: failed=0 passed=667
```

失败用例恒为 `GXX.Core.Tests.ParadoxDataSetCursorTests.EmptyTable_First_IsEof_NoRecords`
（`tests/GXX.Core.Tests/ParadoxDataSetTests.cs:779`，`Assert.Equal(0, ds.RecNo)`），
实际值**每次不同**（观测到 `1701869908` = 0x656C6F74、`41514896` = 0x02797A10）⇒ 读到未初始化缓冲。
可疑点：`ParadoxDataSet.GetRecNo => PxRecordHeaderOps.GetRecordIndex(ActiveBuffer)`
（`src/GXX.Core/Paradox/ParadoxDataSet.cs:607`），空表 `First()` 走 grEOF 分支、`ActiveBuffer` 未填充/未清零。
现象与并行度相关（`dotnet test -v n` 更慢，多次全绿；默认 verbosity 下失败率高）。
**合计观测：干净基线 4 次 2 红、同构建仅跑基线 6 次 1 红 ⇒ 与 HardInfo 车道无关。**

- 本车道**不改** `src/GXX.Core/Paradox/**`（不在独占区）。已就此事单独上报集成方。
- 本车道带新测试时连跑 8 次：`832/832` ×5、`831/832` ×3（失败那几次的唯一红点仍是上述 Paradox 用例；
  **Hardware 165 例在这 8 次里全部为绿**）。

---

## 6. 发现的原文缺陷 / 易错点（带 `文件:行`）

> 全部**照抄保留**，并在测试里用差异断言锁定（改 Core 或改判定时相关断言会红）。

### D1 ★ `GetCPUID` 的 asm **从不装载参数**（`HardInfo.pas:253-284`）
`PUSH EBX/EDI` → `MOV EDI, Registers` → `XOR EBX,EBX; XOR ECX,ECX; XOR EDX,EDX` → `DB $0F,$A2`，
**全程没有 `MOV EAX, Param`** ⇒ 实参叶号被完全忽略，实际叶号是调用时 EAX 的残留值、ECX 恒 0。
**后果：`GetCpuName` 在原文里永远拿不到 CPU 品牌串**（`GetCpuName` 是 `{$IFDEF M2SERVER}` 专用）。
托管接缝保留了 `Param` 形参（签名 1:1），由真实实现裁定是否照抄。

### D2 ★ `GetIdeSerialNumber` 的越界写（`:181`）
`(PChar(@sSerialNumber) + SizeOf(sSerialNumber))^ := #0;` —— `sSerialNumber` 只有 20 字节（`:80`），
写出界 1 字节正好落在 `wBufferType`（`:82`，TIdSector 偏移 40）的**低字节**上。
本移植在同一原始缓冲的 +40 处按字节等价照做（`GetIdeSerialNumber_NulTerminatorOverrunsIntoWBufferType` 锁定）。

### D3 `ChangeByteOrder` 只交换**相邻字节对**，不是整段反转（`:128-142`/`:487-501`）
故 `Size` 为 4 的字段取小端时形如 `[b0,b1,b2,b3] → [b1,b0,b3,b2]`（**不是** `[b3,b2,b1,b0]`）。
另：`Size` 为奇数时**最后一字节不参与**交换（`Size shr 1` 向下取整）；`Size` 为负时 Delphi 的 `shr` 是
**逻辑**右移 ⇒ 迭代 2^31-1 次、越权读写到 AV（托管侧抛 `IndexOutOfRangeException`，见 `ChangeByteOrder_NegativeSize_ThrowsAtArrayBoundary`）。

### D4 ★★ `CountSetBits` **不是 popcount**（`:233-251`）
`bitSetCount := Ifthen(...)` 是**赋值不是累加**，循环只跑 `I = 0..30`（`LSHIFT - 1` 是上界），
且 `bitTest: Uint64 ← 1 shl 31` 以 32 位求值后扩到 64 位。逐轮推导掩码 = `$FFFFFFFF shl (31-k)`，
末轮（k=30）掩码 = `$FFFFFFFE`（与有符号/无符号 64 位语义无关，已消歧）⇒
**`CountSetBits(x) = 1 当且仅当 x ∈ {0,1}，否则 0`**。
`GetCPUCount` 的 `RelationCache` 分支 `if JJJ = 1` 因此实际判的是"掩码 ∈ {0,1}"。

### D5 `GetCPUCount` 的 `case` 无 `else`（`:363-378`）
现代 Windows 返回 `RelationGroup(4)`/`RelationProcessorDie(5)`/`RelationNumaNodeEx(6)`/`RelationProcessorModule(7)`，
全部被**静默丢弃**、不计入任何计数（Delphi 7 的 `TLogicalProcessorRelationship` 只声明 0..3）。

### D6 `GetCPUCount` 第一次调用成功时不改写 `ReturnLength`（`:341-360`）
`Count := ReturnLength div sizeof(...)`，而 `ReturnLength` 只在失败且 `ERROR_INSUFFICIENT_BUFFER` 时被 API 更新
⇒ 若第一次"恰好成功"，`Count` 恒 = 1（只统计 1 项）。
另：`Count` 未与 `Length(Buffer)` 复核（`:360`），越界时原文越权读相邻内存，托管侧抛 `IndexOutOfRangeException`。

### D7 ★ `GetWindowsVersion` 内层 `case` 无 `else`（`:641-644`）
`major = 5` 且 `minor ∉ {0,1}` ⇒ `result` 保持函数结果初值 `''`，最终只剩 `", Build: N"`
⇒ **Windows XP(5.1) 之外的 5.2/5.3（2003/XP x64）不被识别**。
（`Windows XP` 本身是 5.1，落 `case 1` ⇒ `'Windows Whistler'`，也是原文的过时命名。）

### D8 `GetWindowsVersion` 的 98SE 判据是 `szCSDVersion[1]`（`:622`）
即**第 2 个字符**（Windows 98 SE 的 CSDVersion 为 `" A "`）。`"A"`（A 在下标 0）**不会**被判为 98 SE。

### D9 `GetWindowsVersion` 的 build 截断（`:658`）
`IntToStr(Loword(dwBuildNumber))` 只取低 16 位 ⇒ build ≥ 65536 被截断（`$00010005` → `"5"`）。

### D10 ★ `GetIdeDiskSerialNumber` 两分支对同一 IOCTL 取**不同偏移**（`:520-522`+`:575` vs `:552`+`:575`）
NT 分支：`pOutData = @Buffer + sizeof(SRB_IO_CONTROL)(28)`，IDENTIFY 在 **+44**；
Win9x 分支：`pOutData := @pInData^.bBuffer`（= `Buffer + 32`），IDENTIFY 在 **+48**。
同一份输出被两个偏移解释 ⇒ 至少一侧必然错位。

### D11 `GetAdapterMac` 的多余 NCBENUM 调用（`:677-681`）
第一次 `NetBios(NCBENUM)` 在 `ncb_buffer = nil`、`ncb_length = 0` 时发出（`ZeroMemory` 之后、`:679-680` 之前），
返回码被丢弃；真正的枚举在 `:681`。**恒发两次 NCBENUM**。

### D12 ★ `GetAdapterMac` 丢弃最后一次 NetBios 的返回码（`:695`）
`NCBASTAT` 失败时 `Adapter` 保持 `ZeroMemory` 的全 0 ⇒ 返回 `'000000000000'`，
与"真实的 MAC 恰好全 0"**无法区分**。

### D13 `GetAdapterMac` 的 `Lanaenum.lana[aNo]` 无边界检查（`:685`/`:691`）
`aNo` 是 `Integer`，合法域 0..254。`aNo = -1` 会读到相邻的 `length` 字节，更远则越权读写。
托管侧抛 `IndexOutOfRangeException`。

### D14 `GetAdapterMac` 的空 `try..finally`（`:702-704`）
`try ... finally` 的 finally 体为空（无任何释放）—— 原文冗余，照抄保留。

### D15 `GetMemorySize` 的分档边界与取整（`:295-302`）
判据是**严格大于**：恰好 1GB → `'1024MB'`、恰好 1MB → `'1024KB'`、恰好 1KB → `'1024Bytes'`。
`Round` 是**半值取偶**：1.5GiB → `'2GB'`、2.5GiB → `'2GB'`（半值上取会得 3GB）。
另：`GlobalMemoryStatusEx` 的返回值被丢弃（`:293`）、`oMemoinfo` 未初始化。

### D16 `GetDisplayFrequency` 丢弃返回值 + 未初始化 `DeviceMode`（`:211-212`）
`EnumDisplaySettings` 的 `Boolean` 被丢弃，`DeviceMode` 是未初始化局部 ⇒ 失败时返回栈上垃圾。
托管侧值类型默认全 0 ⇒ 失败时返回 0（差异登记）。

### D17 `GetIdeSerialNumber` 的 `FillChar` 只清 32/33 字节（`:155`）
`FillChar(SCIP, SizeOf(TSendCmdInParams) - 1, #0)` ⇒ `SCIP.bBuffer[0]` 那 1 字节未清
（靠单元级全局的静态初值兜住）。另 `:162` `bDriveNumber := 0;`、`:167-168` 的 `bDriveHeadReg` 条件赋值**被注释掉**。

### D18 `GetNetCardName` 的四处（`:757`/`:760-772`/`:762-770`/`:726-742`）
① 内嵌 `RegEnum` 的 `Boolean` 返回值被丢弃；② `Reg.Free`/`netList.Free` **不在 `try..finally`** 中；
③ 打开注册表子键成功即 `Break`，无 `ServiceName` 存在性判断（读不到就返回空串）；
④ `repeat until iRes <> ERROR_SUCCESS` 把**任何**非 0（含真实错误）都当成正常结束。
另：`RegEnum` 的 `DoKeys = False` 分支（`RegEnumValue`）在本单元内**无任何调用点**（死分支）。
该函数**整体是死代码**（唯一引用点在被注释掉的 `GetHwid2` 内）。

### D19 `GetNetCardMac` 的越界读与死常量（`:802`/`:778`）
`for i := 1 to BytesReturned` 未与 `256` 比较；`OID_802_3_CURRENT_ADDRESS`（`:778`）声明后**从未使用**。

### D20 `GetHwid2` 整段被注释 ⇒ 单元**没有 initialization 段**（`:833-897`）
被注释掉的块里含 `initialization CoInitialize(nil); finalization CoUninitialize();`，
故编译产物**不注册 COM**。其依赖全部未移植：WMI 器件类（`TDiskDriveInfo`/`TBiosInfo`/`TProcessorInfo`/`TComputerSystemInfo`）
与 `MD5Util.RivestStr`（`git grep RivestStr -- 'GXX.CSharp/src/**/*.cs'` 零命中）。
调用点状态：`MShare.pas:13383` 对该函数的引用同样被注释掉。
**当前活代码里唯一的"机器码"路径**是 `MShare.pas:3554` 与 `M2Share.pas:8450`。

### D21 `GetCpuIDstringEx` 的拼接不可逆（`:392-393`）
4 个无符号数**无分隔符**相连 ⇒ `(1,23,4,5)` 与 `(12,3,4,5)` 都得 `"12345"`。
（该函数的值被 `MShare.pas:3554` 直接拼进机器码，故歧义会传导到机器码。）

### D22 `FormatMac` 恒 6 轮的补位（`:822-830`）
不足 12 位时仍补足 5 个分隔符：`''` → `'-----'`、`'A1'` → `'A1-----'`；超过 12 位被丢弃。

### D23 `GetDisplayDevice` 只设一次 `cb`（`:221`）+ 循环无上限
`lpDisplayDevice.cb` 在**循环外**设置一次（MSDN 范例是每轮都设）；接缝/API 若恒返回 true ⇒ `while` 死循环。

### D24 全文用词纠正
派发提示词中的 `THardInfo` **不存在**（见 §2 末尾）。

---

## 7. 存疑项 / 有意偏离（**已登记，未擅自"统一"**）

1. **`IntToStr(DWORD)` 的重载选择**：Delphi 7 的 `IntToStr` 有 `Integer` 与 `Int64` 两个重载，
   `DWORD` 实参落到哪个本仓库无法取证（决定 ≥ `$80000000` 时打印无符号还是有符号）。
   本移植沿 `GXX.Core.Rtl.DelphiRTL.IntToStr(uint)`（无符号），并用
   `FormatCpuIDstringEx_HighBitDwordIsPrintedUnsigned` 把该选择**锁死**（Core 若改判该用例会红）。
   四个字段的真实取值域（OEM ID ≤ 9、处理器数 ≤ 128、类型 ≤ 8664、Revision ≤ 65535）都不触及该分歧。
2. **`Uint64` 在 Delphi 7 中的有符号性**：影响 `CountSetBits` 的 `bitTest div 2`（有符号 `div` 是算术右移）。
   已证明**两种语义下的低 32 位一致** ⇒ `CountSetBits` 的结果与歧义无关（写入实现注释）。
3. **`Nb30.pas` 本仓库无源码**：`TNCB`/`TLanaEnum`/`TAdapterStatus` 按 `NCB`/`LANA_ENUM`/`ADAPTER_STATUS`
   的 Win32 定义重建（逐字段与 NB30.H 一致，`sizeof` = 64/256/60 已断言），但在本仓无法与 Delphi 源码逐字回读比对。
   `TNCB`/`TLanaEnum`/`TAdapterStatus` 的**类型归属**是 `Nb30.pas`，本车道按"接缝契约类型"落在 `Hardware/` 下
   （未另造接缝本体，符合台账 §15/§22.4）。
4. **`GXX.Core.Util.TStringList` 没有 `Text` 属性**
   （`grep "public string Text" Util/TStringList.cs` 零命中），而原文 `:759` 是 `netList.Text := netListText`。
   本车道**没有**改 `Util/**`（不在独占区），改为在 `Hardware/` 内内联实现
   `Classes.TStrings.SetTextStr` 语义（`HardInfo.SplitTextStr`，含"结尾换行不产生空行"），并单测 10 例。
   **建议集成方**把 `Text` 属性补进 `TStringList`（届时可删除 `SplitTextStr`）。
5. **`DelphiRTL.AnsiString` 的 XML 注释与实现不一致**（既有 Core 问题，未改）：
   注释写"截断到第一个 `\0`"，实现（`DelphiRTL.cs:202-209`）是**不截断**的定长 GBK 解码。
   本移植**依赖实现**（`GetIdeDiskSerialNumber` 的 `SetString` 定长语义正确），
   并用 `GetIdeDiskSerialNumber_SetStringDoesNotTruncateAtNul` 锁定。
   ⚠ 若将来有人把注释当规格去"修正"实现，该用例会红 —— 那正是守卫意图。
6. **`:186-204` 的 S.M.A.R.T. 参考资料注释块（19 行）未搬迁**：纯说明性文字（MSDN/WinDDK/第三方链接），
   不含代码，且其中的 URL 与 "Win98 SMARTVSD.VXD 安装说明" 属第三方资料，搬迁到 C# 源里无审计价值。
   **如需逐字保留可随时补**（行程成本极低）。
7. **`GetNetCardName`/`GetNetCardMac`/`FormatMac` 由 `implementation` 段改 `public`**：为可测性（原文单元外不可见）。
   三个函数在原文里都是**死代码**（唯一引用点在被注释掉的 `GetHwid2` 内）。
8. **托管侧新增的边界守卫**（原文无）：
   `GetAdapterMac` 的 `lana` 索引检查、`FormatAdapterAddress6`/`FormatNdisAddressHex` 的数组越界、
   `GetCPUCount` 的 `Buffer[III]`、`HardInfo.EnsureIdentifyBuffer`。原文对应处是**越权读写**，
   托管侧一律抛异常（差异登记）。方向是"变响亮"，不是"变正确"。
9. **`Pos("")` = 0（Delphi 为 1）**：本单元**未使用 `Pos`**（`uses StrUtils` 只用到 `LeftStr`/`RightStr`，
   两者已按 `Copy` 语义实现）⇒ **不受该未修缺陷影响**。按规程此处申报。

---

## 8. 诚实说明：未完成部分 / 剩余量 / 无法验证项

### 8.1 在无头环境里**根本无法验证**的函数（全部为接缝侧，本车道已把可验证部分榨干）

| 函数 | 无法验证的部分 | 本车道已覆盖的部分 |
|---|---|---|
| `GetCpuIDstringEx` | `GetSystemInfo` 的真实返回 | 4 段拼接与歧义（纯函数） |
| `GetCpuName` | 真实 `CPUID`（且原文 asm 本身是坏的，见 D1）；真实 CPU 品牌串是否能解出 | 叶号序列 `$80000002/$80000003/$80000004`、寄存器→字符串的小端拼装、NUL 截断 |
| `GetCPUCount` | 真实 `GetLogicalProcessorInformation` 输出；现代 Windows 的 `Relationship` 取值分布 | 两次调用的缓冲算术、`Count` 推导、统计与格式化、`case` 无 else |
| `GetMemorySize` | 真实 `GlobalMemoryStatusEx` 值 | 全部分档边界 + 半值取偶 |
| `GetIdeSerialNumber` / `GetIdeDiskSerialNumber` | 真实 `CreateFile('\\.\PhysicalDrive0' | '\\.\Scsi0:' | '\\.\SMARTVSD')` 权限与 `DeviceIoControl`；真实 IDENTIFY 扇区内容；**两块盘/RAID/NVMe 上的行为**（NVMe 无 ATA IDENTIFY） | SCIP/SRB 请求构造、分支偏移 44/48、字节序变换、`SetString` 定长、失败路径不改写 `ref` 形参 |
| `GetWindowsVersion` | 真实 `GetVersionEx`（Win8.1+ 未声明 supportedOS 时**返回假版本**，需改 `RtlGetVersion`/WMI） | 全部版本映射分支 + 三个原文缺陷 |
| `GetDisplayFrequency` / `GetDisplayDevice` | 真实 `user32` 枚举；多显示器/显示器热插拔 | 返回码被丢弃的语义、`cb` 只设一次、大小写敏感匹配、`cc` 递增顺序 |
| `GetAdapterMac` | 真实 `NetBios`（本机是否有 LANA、NCBASTAT 结构填充） | 四次调用序列、`ncb_buffer/ncb_length` 时序、`callname='*'`、MAC 格式化、三处原文缺陷 |
| `GetNetCardName` | 真实注册表 `...\NetworkCards\` 内容 | 枚举/CRLF 拼接/首个可打开键/`ServiceName` |
| `GetNetCardMac` | 真实 `IOCTL_NDIS_QUERY_GLOBAL_STATS`（Win10+ 上常需管理员且行为已变） | 设备名/IOCTL/OID 取值、`BytesReturned` 1-based 语义 |

### 8.2 明确**未移植**的部分

- `GetHwid2` 及其 `initialization`/`finalization`（原文**整段被注释**，见 D20）：**0 行移植**，原文照抄保留在 C# 注释块。
  将来要做"机器码 WMI 深化"（`Checklist.md:269`）需要：① WMI 器件层（`Win32_BIOS`/`Win32_BaseBoard`/`Win32_DiskDrive`/`Win32_Processor`）
  ② `MD5Util.RivestStr`（`Source/Common/MD5Util.pas`，`GXX.Core` 尚未移植）③ `CoInitialize/CoUninitialize` 的托管归口。
- **没有任何** `P/Invoke` 实现（与 `GXX.Core` 现状一致）：接缝的"真实实现"是本车道的**有意留白**，
  精确签名见 §4。真实实现落地后建议新增 `Hardware/HardInfoSeams.Win32.cs`（按 §4.3 的平台布局注意处理 x64）。

### 8.3 剩余量估计

| 剩余工作 | 量 |
|---|---|
| 接缝真实实现（P/Invoke/WMI，9 个接口 20 个成员） | 中等（约 400~600 行 + 需在真机/CI 上做集成验证） |
| `GetHwid2` + WMI 器件层 + `RivestStr` | 大（跨 2 个单元，属独立车道） |
| `TStringList.Text` 属性（见 §7.4） | 小 |
| Paradox flaky 用例（见 §5） | 小（但不属本车道） |

---

## 9. 复核方式速查

```powershell
cd D:\chuanqi\daima\GXX原版_Delphi7\.worktrees\p6-core-hardinfo\GXX.CSharp
$env:DOTNET_CLI_UI_LANGUAGE='en'
dotnet build GXX.slnx -c Debug --nologo
dotnet test tests\GXX.Core.Tests\GXX.Core.Tests.csproj -c Debug --nologo --filter "FullyQualifiedName~Hardware"
```
