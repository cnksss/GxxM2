using System;
using System.IO;
using System.Reflection;
using GXX.GatewayKit;
using GXX.GatewayKit.Rest11;
using Xunit;

namespace GXX.GatewayKit.Tests;

// ============================================================================
// 集成方新增（台账 §62.1 / X-P17-01）：**opt-in 选项的绑定可达性**用例。
//
// 为什么必须有这条用例：`LoginGateService.Rest11Options` 曾经是 `{ get; set; }` 自动属性，
// 它**隐藏**（而不是覆写）基类 `GateService.Rest11Options`（基类是 get-only 的
// `protected virtual … => null`；C# 既不允许 override 放宽可见性，也不允许 get-only→get-set）。
// 后果：`GateService.LoadConfig:79` 与 `CheckIP:228` 里的 `Rest11Options` **静态绑定到基类视图**，
// 恒为 null ⇒ `IsBlockIP` / `IsBlockIPArea` / `OverConnectOfIP` 三处判定**永远不可达**、
// `EnableIpAddrFilterResidual` 成了装饰品 —— 而且是**静默**的：不抛、不打日志、测试全绿。
// 而本工程把暴露它的 `CS0108`/`CS0114` 在 `Directory.Build.props` 里全局 `NoWarn` 掉了。
//
// 本用例直接断言"**基类那三处读取看到的东西**"（反射取基类视图），因此修复前后会一红一绿 ——
// 这是"可接线 ≠ 已接线"（§33.4/§58.5）在**属性绑定**这一层的对应物。
// ============================================================================
public class Rest14LoginGateOptionsBindingTests
{
    private static string NewIniPath()
    {
        string p = Path.Combine(Path.GetTempPath(), "rest14bind_" + Guid.NewGuid().ToString("N") + ".ini");
        File.WriteAllText(p, "");
        return p;
    }

    [Fact]
    public void DerivedPublicOptions_AreVisibleToTheBaseInternalReaders_IntegratorFix()
    {
        string ini = NewIniPath();
        try
        {
            using var svc = new GXX.LoginGate.LoginGateService(Rest11LoginGateOptions.All, ini);

            // ① 派生类的公开视图（对外 API）
            Assert.NotNull(svc.Rest11Options);

            // ② 基类的内部视图 —— LoadConfig:79 / CheckIP:228 读的就是它
            PropertyInfo? baseView = typeof(GateService).GetProperty(
                "Rest11Options", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(baseView);                       // 基类上的那一个（protected virtual）
            object? seenByBase = baseView!.GetValue(svc);
            Assert.NotNull(seenByBase);                     // ★ 修复前为 null ⇒ 执法面不可达
            Assert.Same(svc.Rest11Options, seenByBase);     // 两个视图必须是**同一个对象**

            // ③ 写穿：改派生属性，基类视图必须同步（否则"接线了但基类看不见"会重现）
            Rest11LoginGateOptions off = Rest11LoginGateOptions.Disabled;
            svc.Rest11Options = off;
            Assert.Same(off, baseView.GetValue(svc));
        }
        finally { try { File.Delete(ini); } catch { } }
    }

    [Fact]
    public void DefaultConstruction_LeavesEveryRest11BranchOff()
    {
        // 反向护栏：不传选项时（本工程集成用例就是这么建的）基类视图必须为 null ⇒ 默认路径零新分支。
        string ini = NewIniPath();
        try
        {
            using var svc = new GXX.LoginGate.LoginGateService(null, ini);
            Assert.Null(svc.Rest11Options);
            PropertyInfo? baseView = typeof(GateService).GetProperty(
                "Rest11Options", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.Null(baseView!.GetValue(svc));
        }
        finally { try { File.Delete(ini); } catch { } }
    }
}
