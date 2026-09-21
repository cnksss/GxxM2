# 并行报告：p9-m2-forms（M2Engine 窗体/杂项族 7 单元）

> 分支：`par/p9-m2-forms`　基线：`main @ b48e4c43`
> 独占分区：`GXX.CSharp/src/GXX.M2Server/Sweep9/Forms/**`、`GXX.CSharp/tests/GXX.M2Server.Tests/Sweep9Forms*.cs`、本文件
> 本报告**滚动更新**：每完成一个切片即 commit，结论不留在脑子里（派发方要求）。

---

## 0. 开工侦察结论（先落盘）

### 0.1 源文件可用性

| 单元 | UTF-8 镜像 `.pas` | `.dfm` | 备注 |
|---|---|---|---|
| `ConfigMerchant.pas`（654） | ✅ `_analysis/utf8_mirror/M2Engine/Forms/` | ✅ 文本 DFM 466 行 | 直接可读 |
| `ConfigMonGen.pas`（60） | ✅ 同上 | ✅ 文本 DFM 37 行 | |
| `ViewHeroRcd.pas`（453） | ✅ 同上 | ⚠ **镜像里的 `.dfm` 已损坏** | 见 §0.2 |
| `ViewKernelInfo.pas`（198） | ✅ 同上 | ✅ 文本 DFM 500 行 | |
| `uFrmClientPlugManager.pas`（151） | ✅ 同上 | ✅ 文本 DFM 33 行 | |
| `NoticeM.pas`（119） | ✅ `_analysis/utf8_mirror/M2Engine/` | 无 DFM（非窗体） | |
| `uAliyunSendSMSThread.pas`（320） | ✅ 同上 | 无 DFM（非窗体） | |

### 0.2 ⚠ `ViewHeroRcd.dfm`：镜像副本**不可用**（必须回读 `Source/`）

- `_analysis/utf8_mirror/M2Engine/Forms/ViewHeroRcd.dfm` 是**二进制 DFM 被当作文本转码**后的产物：
  首字节 `FF` 变成 `EF A3 B5`（3 字节），长度字段 `83 11 00 00` 变成 `3F 00 00`（**丢字节、长度错**）。
  长度：镜像 4,541 字节 vs 原始 4,510 字节。
- 因此本车道**回读原始二进制** `Source/M2Engine/Forms/ViewHeroRcd.dfm` 并**手工解码**（DFM 是二进制格式，
  本身不是 GBK 文本 ⇒ 不存在乱码风险；文件内字符串按 `vaWString`(0x12, UTF-16LE) / `vaString`(0x06, GBK) 解码）。
- **二进制格式实测结论（与常见资料不同，按实测）**：
  - 文件头 27 字节：`FF 0A 00` + 全大写窗体名（NUL 结尾）+ `30 10` + `Int32`(DFM 流长度 = 4483)，
    紧接着偏移 27 起是标准 `TPF0` 流；`27 + 4483 = 4510` = 文件长度（**自洽**）。
  - 对象节点 = `ShortString 类名` + `ShortString 名` + 属性表（`0x00` 结尾）+ 子对象表（`0x00` 结尾）；
    **本变体没有"前缀字节"**（既不是 inherited 也不是 inline）。
  - `vaList`(0x01) 在本文件里是**无名值列表**（`ColWidths`/`RowHeights`），不是 name=value 表。
  - 解码校验：`p` 收尾 = 4510 = 文件长度（**全覆盖、零残留**），29 个控件与 `.pas:11-39` 声明**逐名一致**。
- 复现：见本报告 §5「二进制 DFM 解码复现命令」。

### 0.3 ★★ 高价值原文缺陷（`ViewKernelInfo.pas:116/:122/:128`）：`@Config.XxxThread` 取的是**指针字段的地址**

```pascal
Config := @g_Config;                          // :99
ThreadInfo := @Config.UserEngineThread;       // :116  ← UserEngineThread: pTThreadInfo（M2Share.pas:1808）
ThreadInfo.hThreadHandle := 0;                // :117  ← 写进"指针字段本身"所在的内存
ThreadInfo.dwRunTick   := 0;                  // :118
ThreadInfo.nRunTime    := 0;                  // :119
ThreadInfo.nMaxRunTime := 0;                  // :120
ThreadInfo.nRunFlag    := 0;                  // :121
```

`UserEngineThread` / `IDSocketThread` / `DBSOcketThread` 在 `g_Config` 里是**指针字段**（`pTThreadInfo`，4 字节）。
`@Config.UserEngineThread` 取的是**该指针字段的地址**（作者显然想写 `Config.UserEngineThread` —— 但那也没 `New`，
仍是 nil 解引用），于是 `ThreadInfo.xxx := 0` 把 `TThreadInfo`（M2Share.pas:558-568，8 字段 × 4B）**覆盖到 `g_Config`
的后续字段上**。按字段偏移精确换算（`TThreadInfo` 字段序：`dwRunTick`+0 / `boActived`+4 / `nRunFlag`+8 /
`boTerminaled`+12 / `nRunTime`+16 / `nMaxRunTime`+20 / `hThreadHandle`+24 / `dwThreadID`+28；
`g_Config` 该段布局：`+0 UserEngineThread`、`+4 IDSocketThread`、`+8 DBSOcketThread`、`+12 nUserSellOffCount`、
`+16 nUserSellOffTax`、`+20 nSkill69CD`、`+24 nSkill69AddTime`、`+28 nSkill69AddRange`、`+32 boSkill69SameLevel`、
`+36 nSkill70CD`），**`FormCreate` 跑完一次的净效果**是：

| 被写坏的 `g_Config` 字段 | 变成 | 来源 |
|---|---|---|
| `UserEngineThread` | `nil` | `:116` 基址 +0 ← `dwRunTick = 0` |
| `DBSOcketThread` | `nil` | `@IDSocketThread` 基址 +4 ← `nRunTime`、或 `@UserEngineThread` +8 ← `nRunFlag` |
| `IDSocketThread` | `nil` | `@IDSocketThread` 基址 +0 ← `dwRunTick = 0` |
| `nUserSellOffCount` | `0` | `@IDSocketThread` +8 ← `nRunFlag` |
| `nUserSellOffTax` | `0` | `@UserEngineThread` +16 ← `nRunTime` |
| `nSkill69CD` | `0` | `@UserEngineThread` +20 ← `nMaxRunTime` |
| `nSkill69AddTime` | `0` | `@UserEngineThread` +24 ← `hThreadHandle` |
| `nSkill69AddRange` | `0` | `@DBSOcketThread` +20 ← `nMaxRunTime` |
| `boSkill69SameLevel` | `False` | `@DBSOcketThread` +24 ← `hThreadHandle`（4 字节写入的低字节） |

⇒ **打开"内核数据查看"窗口会把三个线程指针置空、并清零 5 个 `Skill69`/寄售配置字段。**
处置：**逐字保留**（用显式内存视图模型 `TConfigThreadRegion` 复刻 4 字节重叠写），
并在测试里用**逐字段差异断言**锁死这张表（D-P9-01 登记，见 §2）。

### 0.4 依赖缺口（先登记，动手时按接缝处理）

托管侧**不存在**下列原文依赖 ⇒ 按 §25.2「接缝默认显式抛/显式注入，不臆造中性替身」处理：

| 原文依赖 | 现状 | 处置 |
|---|---|---|
| `UserEngine`（`UsrEngn.pas` 全局对象） | 全树 0 命中 | 每个窗体自带**实例级** `Func/Action` 接缝（照 `TFrmDummySetting` 的既有形态） |
| `UserEngine.m_MerchantList` / `.m_MonGenList` | 0 命中 | 同上（列表面：`LockR/UnLockR/Count/Items`） |
| `g_M2RunThreadMgr` / `TM2RunThread`（`M2Threads.pas`） | 0 命中 | 同上（`ThreadDesc/Handle/ThreadID/RunTick/MinRunTick/MaxRunTick/ThreadCPUUsage/MaxThreadCPUUsage`） |
| `g_PlugClientList` / `g_PlugFileMD5ListText{,Len,CRC}`（`M2Share.pas:3971/3804-3806`） | 0 命中 | 接缝静态类 + `pTPlugClientInfo` 托管等价（原文是 `New/Dispose` 的**指针**记录） |
| `TSendSmsRequest` / `g_AcsUtil`（`SendSmsRequest.pas` / `acsUtils.pas`） | 0 命中 | 接缝 |
| `TEvent`（`SyncObjs`） | 0 命中 | 接缝（`TSafeList` 已存在：`GXX.M2Server.Sweep.TSafeList`） |
| `TPlayObject.m_sMobileNumber` / `m_boMobileBind` / `m_sMobileVerifyCode` / `m_dwMobileVerifyTick` | 0 命中 | 接缝（待 `ObjPlayer.pas` 批次落地） |
| `g_FunctionNPC` | 仅注释命中 | 接缝 |
| `TBaseObject` | 托管侧为 `TCreature`（全树 0 处 `class TBaseObject`） | 接缝参数用 `TCreature`，登记 D-P9-02 |

**复用（已存在，不新增替身）**：`GXX.M2Server.Npc.TMerchant`（含 `m_sScript/m_boCastle/...` 全部字段）、
`GXX.M2Server.Npc.NpcProcessCommand.sNF_*`、`GXX.Core.Protocol.TUserItem`、
`GXX.M2Server.Sweep.TSafeList`、`GXX.M2Server.Sweep.SweepSeam.MainOutMessage / MyGetTickCount`、
`GXX.Core.Util.CheckUnit.BufferCrc`、`GXX.Core.Protocol.EDcode.zEncodeString`、
`GXX.M2Server.Forms.M2Forms`（MessageBox 无头闸门）、`M2ShareState.ConfigIni`、`M2Config.*`。

### 0.5 无头 UI 规程（照 `DummySetting` 车道已验证形态）

- 窗体一律 `System.Windows.Forms.Form` 子类；控件树在构造期实例化（**不需要窗口句柄**即可断言 `Items/Text/Checked/Value`）。
- `ShowModal` → `Sweep9FormsMessageBoxSeam.UiEnabled`（默认 true=生产；测试置 false，否则挂死 testhost）。
- `Application.MessageBox` → 实例级 `MessageBoxHandler`，默认转调 `M2Forms.MessageBox`。
- 所有事件处理器 `public`，测试**直调**，不用消息泵。

---

## 1. 逐单元进度（滚动）

| 单元 | 行数 | 已移植方法数/总方法数 | DFM 控件数（对账） | DFM 绑定数（对账） | 状态 |
|---|---|---|---|---|---|
| `NoticeM.pas` | 119 | 待填 | 无窗体 | 无窗体 | 未开始 |
| `ConfigMonGen.pas` | 60 | 待填 | 待填 | 待填 | 未开始 |
| `uFrmClientPlugManager.pas` | 151 | 待填 | 待填 | 待填 | 未开始 |
| `ViewHeroRcd.pas` | 453 | 待填 | 待填 | 待填 | 未开始 |
| `ViewKernelInfo.pas` | 198 | 待填 | 待填 | 待填 | 未开始 |
| `ConfigMerchant.pas` | 654 | 待填 | 待填 | 待填 | 未开始 |
| `uAliyunSendSMSThread.pas` | 320 | 待填 | 无窗体 | 无窗体 | 未开始 |

---

## 2. 偏离登记（D-P9-xx）

| 编号 | 位置 | 原文 | 托管 | 理由 |
|---|---|---|---|---|
| D-P9-01 | `ViewKernelInfo.pas:116-133` | `@Config.UserEngineThread` 的 4 字节重叠写（§0.3） | 用 `TConfigThreadRegion` 的**显式内存视图**复刻同一组 4 字节写 | 托管侧无裸指针；用显式 byte/dword 视图**精确复刻**原文缺陷，而不是"修正"它 |
| D-P9-02 | `uAliyunSendSMSThread.pas:12/:37` | `Player, Npc: TBaseObject` | 形参类型用托管基类 `TCreature` | 托管侧无 `TBaseObject` 类型（`class TBaseObject` 全树 0 命中）；`TBaseObject` 在原文即 `TCreature` 的别名层 |

---

## 3. 未完成 / 阻塞项

（滚动登记）

---

## 4. 门禁记录

（滚动登记）

---

## 5. 二进制 DFM 解码复现命令

```powershell
$path = "D:\chuanqi\daima\GXX原版_Delphi7\Source\M2Engine\Forms\ViewHeroRcd.dfm"
$b = [System.IO.File]::ReadAllBytes($path)     # ★ 必须 ReadAllBytes；Get-Content 会读坏二进制
# 头部 27 字节：FF 0A 00 + 'TFRMHEROFDBVIEWER' + 00 + 30 10 + Int32(0x1183=4483)
# 流从偏移 27 开始（'TPF0'），到 4510 结束。
# 对象 = ShortString类名 + ShortString名 + 属性表(0x00结尾) + 子对象表(0x00结尾)；无前缀字节。
# 值类型：01=vaList(无名值表,0x00结尾) 02=int8 03=int16 04=int32 06=vaString 07=vaIdent
#         08=False 09=True 0B=vaSet(ShortString表,0x00结尾) 12=vaWString(Int32字符数+UTF-16LE)
```
