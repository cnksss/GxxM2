// SelGate 测试程序集级别设置：
// 被测代码含 Delphi 风格的**进程级全局量**（SelGateGlobals.g_fServiceStarted、
// CSelSessionObj.gDeny / g_LoginData / enterCount、CSelUserList.g_UserList、
// CSelGateIPFilter 的落盘文件），与 xUnit 默认的"类间并行"不兼容。
// 与 Delphi 单进程单实例语义保持一致 ⇒ 关闭并行。
[assembly: Xunit.CollectionBehavior(DisableTestParallelization = true)]
