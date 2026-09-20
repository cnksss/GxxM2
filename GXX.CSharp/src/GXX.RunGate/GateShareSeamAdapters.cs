// =====================================================================================
// 源单元：Source\RunGate\GateShare.pas（3595 LF）+ Source\RunGate\uFrmProcessBlacklist.pas
//   —— **把上一轮留下的"只计数、未落盘"接缝接到本轮的 1:1 产物上**
//
// 背景（上一轮报告 §6.1 明确登记）：
//   `uFrmProcessBlacklist.cs` 的 `ProcessBlacklistUnit.ShowFrmProcessBlacklist` 在"添加/删除"后调用
//   `Sink?.SaveProcessBlacklist()` / `Sink?.RebuildProcessBlacklist()`，而 `Sink` 默认为 **null**
//   → 原文 `uFrmProcessBlacklist.pas:155-156 / :189-190` 的落盘 + 重算缓存**从未发生**。
//   本文件的 `GateShareProcessBlacklistSink` 就是把这两个触发器接到 `GateShareLists` 的真实实现上。
//
// 原文调用点（uFrmProcessBlacklist.pas）：
//   :155-156  `SaveProcessBlacklist; RebuildProcessBlacklist;`（添加后）
//   :189-190  同上（删除后）
//   :40-66    `ShowFrmProcessBlacklist` 先 `FillList`，再在模态返回后执行上面的两步
//
// 接线方式（生产）：
//   `ProcessBlacklistUnit.Sink = new GateShareProcessBlacklistSink();`
// 或在应用启动时赋值一次。测试直接 `new` 并用临时 exe 目录。
//
// ★ 有意**未**接线：`ISafeFilterHost`（`uFrmSafeFilter.cs`）。原因见报告 §13：
//   该接口把 `TempIPList`/`BlockIPList` 声明为 `TSafeHashStringList`（字符串表，窗体直接读写并遍历），
//   而原文这两者是 `TAddressList`（只存 `nIPaddr` 数值）。做成真实实现必须新增一层
//   "字符串视图 ↔ TAddressList"双向同步，会改动 12 个窗体测试类 —— 属后续事项，本轮不碰。
// =====================================================================================

using System;

namespace GXX.RunGate;

/// <summary>把 <see cref="IProcessBlacklistSink"/> 接到 GateShare 的 1:1 落盘/重算实现上。
/// 与 <see cref="InMemorySafeFilterHost"/> 不同，本实现**真的写文件**（`GateSharePaths.ExeDir +
/// 'ProcessBlacklist.txt'`，原文 `GateShare.pas:1964`）。</summary>
public sealed class GateShareProcessBlacklistSink : IProcessBlacklistSink
{
    /// <summary>原文 `uFrmProcessBlacklist.pas:155` / `:189` —— `SaveProcessBlacklist;`。</summary>
    public void SaveProcessBlacklist() => GateShareLists.SaveProcessBlacklist();

    /// <summary>原文 `uFrmProcessBlacklist.pas:156` / `:190` —— `RebuildProcessBlacklist;`。</summary>
    public void RebuildProcessBlacklist() => GateShareLists.RebuildProcessBlacklist();

    /// <summary>当前落盘文件路径（测试与日志用；原文用 `ExtractFilePath(ParamStr(0)) + 'ProcessBlacklist.txt'`）。</summary>
    public static string FilePath => GateSharePaths.ExeDir + "ProcessBlacklist.txt";
}
