// ============================================================================
// 本车道 p13-m2-objplayer 的**测试串行化**声明。
//
// 为什么需要它（**集成时实测到的真实缺陷**）：
//   本单元的 7 个测试类（Core1/2/3/4/6 + ServerSend1/2 + WarrContinueHitManager）
//   都要读写**同一批静态量**：
//     · `PlayerSurfacePortLedger.NotPortedMethods`（留痕台账，多类断言"恰好 N 条"）
//     · `PlayerSurfaceMsgSeams` / `PlayerSurfaceSocketSeams` / `PlayerSurfaceBaseSeams` /
//       `PlayerSurfaceOperateSeams` / `PlayerSurfaceWarrContinueConfig` / 各片自己的 `*Seams`
//     · `PlayerSurfaceMessageTable`（报文派发表）
//   xUnit 默认**按类并行**（不同测试类 = 不同 collection ⇒ 并行）。
//   集成后首次全量跑：**39 failed / 10,647 passed**，失败集中在"精确条数"与
//   "接缝被别的类重置"这两类断言上（例如 `Assert.Equal(34, NotPortedCount)` 读到别的片的值）。
//
// 修法：把它们放进**同一个 collection** 并 `DisableParallelization = true`
//   ⇒ xUnit 会把该 collection 内的所有类**串行**执行，类间不再互相踩。
//   （不能加 assembly 级 `[assembly: CollectionBehavior(DisableTestParallelization = true)]`：
//    那属于 `GXX.M2Server.Tests` 的公共面，会影响其它车道，且放在本车道文件里
//    会与将来别的车道加的同类特性**撞 CS0579 重复特性**。）
//
// ⚠ 只覆盖本车道的 7 个类；**不影响**测试工程里的其它任何类。
// ============================================================================

using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>本车道全部测试类的**共享串行 collection**（见文件头说明）。</summary>
[CollectionDefinition(Name, DisableParallelization = true)]
public class PlayerSurfacePortLedgerSerialCollection
{
    /// <summary>collection 名（各测试类用 <c>[Collection(Name)]</c> 引用）。</summary>
    public const string Name = "PlayerSurfacePortLedgerSerial";
}
