using Xunit;

// =====================================================================================
// xUnit 并行开关：把本车道的测试类全部放进同一个串行集合。
//
// 原因：DxControls 的接缝层里有**进程级静态状态** —— DxControlEngine.ControlEngineList、
// DxRootRegistry / DxControlHooks 的 ConditionalWeakTable、DxControlOps.MoneyListGetIndex /
// CanMoveSink、DxApplication.MainForm。并行跑会让「A 测试刚建的引擎被 B 测试读名字」这类
// 交叉污染出现（实测：单跑全绿、并行跑 21 个失败且报错位置与断言不符）。
// =====================================================================================

[CollectionDefinition("dxctrl-serial", DisableParallelization = true)]
public sealed class DxCtrlSerialCollection
{
}
