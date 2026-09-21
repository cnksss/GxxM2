// ============================================================================
//  车道 p9-m2-monsters：ObjRobot 单元所需的 **TBaseObject 面补成员**
//
//  依据：任务书第 4 条「依赖缺失时优先用 partial 在你自己的新文件里补成员
//  （先 git grep 确认 main 上确实没有）」+ 台账 §19.6「partial 让'补成员'不必碰别人的文件」。
//
//  补成员清单（**逐条已 git grep 确认 main 全树 0 命中**）：
//    1. `TCreature.m_boSuperMan`
//       原文：ObjBase.pas:134  `m_boSuperMan: Boolean; // 0x2B8  无敌模式`
//       —— 归属 **TBaseObject**（= 托管 `TCreature` 这一层）。
//       消费点：ObjRobot.pas:270 `m_boSuperMan := True;`。
//       恢复途径：ObjBase.pas 全量移植落地时，把本 partial 块**整块删除**
//       （否则 CS0102 重复定义）。
//       （同类"跨区补成员必须留删除指引"的先例见
//        docs/并行报告-p8-m2-dummysetting.md 的"跨区事项"第 1 条。）
//
//  未补的成员（**有意不补**，台账 §18.7 的既定判断：车道不自行声明 TPlayObject 替身）：
//    `TPlayObject.SendSocket`（ObjPlayer.pas:1191 virtual / :3526 实现）
//      ⇒ 本批次只登记为形式偏差 **D-P9-02**（`override` 不可表达），
//        不在此处造一个"能 override 的空虚方法"——那会与正式移植的签名撞车（CS0111）。
//
//  ⚠ 命名空间必须是 `GXX.M2Server.Engine`（与 Engine/ObjBase.cs 的 partial 同域），
//    否则会变成**另一个同名类型**（编译期不报错、运行期字段永远是 false）——
//    这正是本文件第一版踩到的坑，已用构建失败 + 本注释固化。
// ============================================================================

namespace GXX.M2Server.Engine
{
    /// <summary>
    /// `ObjBase.pas` 的 `TBaseObject` = 托管侧 <see cref="TCreature"/> 的**补成员**分区。
    /// </summary>
    public abstract partial class TCreature
    {
        /// <summary>
        /// `ObjBase.pas:134 m_boSuperMan: Boolean; // 0x2B8  无敌模式`
        /// <para>★ **本成员由车道 p9-m2-monsters 在 partial 分区内补齐**（main 上确实不存在）。
        /// ObjBase.pas 正式移植落地后请**删除本行**，避免 CS0102。</para>
        /// </summary>
        public bool m_boSuperMan;
    }
}
