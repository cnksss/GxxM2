// ============================================================================
// 源单元：Source\M2Engine\ObjNpc.pas（调用点）／Source\M2Engine\M2Share.pas·ObjPlayer.pas（宿主）
// 本文件：`TMerchant.UserSelect` 的 **"消息/输入"族**（`RemoteMsg` 2177-2204 /
//          `InPutInteger` 2461-2484 / `InPutString` 2486-2506）所需的**宿主面接缝**。
//
// 报告 §24 已把这 4 项列给调度方；本轮按"接缝、零跨文件"的建议落地。
// ★ 默认值全部按台账规程「**能对应到原文某个已定义状态就是忠实；对应不到才必须抛**」判定：
//   这 4 项在原文里都**有明确的未初始化状态**（Delphi 字段/全局的零值），故默认值是**复现**、
//   不是"发明一个原文从不会出现的值"（对照偏差 D37：那是"真值不可知"，故必须抛）。
// ============================================================================

using System;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Npc;

/// <summary>"消息/输入"族（`RemoteMsg`/`InPutInteger`/`InPutString`）的宿主面接缝。</summary>
public static class ObjNpcInputSeams
{
    /// <summary>
    /// 原文 `TargetObject.m_boRemoteMsg`（ObjNpc.pas:2188）—— "对方是否接受歌曲"标志。
    /// <para>字段本体在 **ObjPlayer.pas**（未移植）。**默认 `false`** 是忠实的：
    /// Delphi 字段**未设置时就是 False** ⇒ 复现原文已有状态（非"发明值"）。</para>
    /// <para><b>★ 删除条件（可执行）</b>：当 `m_boRemoteMsg` 在托管侧落地（`grep -n 'm_boRemoteMsg'
    /// src/GXX.M2Server/Engine/` 出现**赋值**而非仅声明）时，删除本接缝、改为直读。</para>
    /// </summary>
    public static Func<TPlayObject, bool> GetBoRemoteMsg { get; set; } = _ => false;

    /// <summary>
    /// 原文 `g_sUserNotOnLine`（ObjNpc.pas:2201，M2Share.pas 全局串；
    /// 原文该处**带注释** `{ '  没有在线！' }`）。
    /// <para>接缝：待 M2Share 的 StringConf 接入。默认值**即原文注释里的默认文本**（不是语义占位）。</para>
    /// </summary>
    public static string g_sUserNotOnLine { get; set; } = "  没有在线！";

    /// <summary>
    /// 原文 `g_InputBoxFilterList`（ObjNpc.pas:2468/2494，M2Share.pas 全局 `TStringList`）。
    /// <para>**只用于判 `&lt;&gt; nil`**。**默认 `null`** 是忠实的：原文该全局**未初始化即 nil**
    /// ⇒ **整段非法字符过滤被跳过**是原文真实行为。</para>
    /// <para><b>★ 删除条件</b>：`g_InputBoxFilterList` 在托管侧落地时删除本接缝（判据同 §24.2）。</para>
    /// </summary>
    public static Func<object?> GetInputBoxFilterList { get; set; } = () => null;

    /// <summary>
    /// 原文 `GetInputBoxInFilterList(sMsg): Boolean`（ObjNpc.pas:2470/2496，M2Share.pas）。
    /// <para>**仅在上面的 `GetInputBoxFilterList() != null` 时才被调用**。
    /// **默认 `false`** = "不在过滤名单" ⇒ 复现"该全局为 nil 时整段被跳过"的等价结果。</para>
    /// <para><b>★ 删除条件</b>：M2Share.pas 的 `GetInputBoxInFilterList` 移植后删除。</para>
    /// </summary>
    public static Func<string, bool> GetInputBoxInFilterList { get; set; } = _ => false;

    /// <summary>
    /// 原文 `UserEngine.GetPlayObject(sMsg): TPlayObject`（ObjNpc.pas:2185，M2Share/UsrEngn 面）——
    /// 按**玩家名**查在线玩家对象。
    /// <para>**默认 `null`** 是忠实的：查不到就是 `nil`（原文 2186 的 `TargetObject &lt;&gt; nil` 判定），
    /// 而"无宿主"等价于"查不到"。</para>
    /// <para><b>★ 删除条件（可执行）</b>：`UserEngine.GetPlayObject` 在托管侧落地时删除本接缝；
    /// 判据：`grep -n 'GetPlayObject' src/GXX.M2Server/` 出现**代码声明**（`TPlayObject? GetPlayObject(`）。</para>
    /// </summary>
    public static Func<string, TPlayObject?> GetPlayObject { get; set; } = _ => null;

    /// <summary>恢复默认（单测隔离用；由 <c>NpcSeams.ResetDefaults</c> 调用）。</summary>
    public static void ResetDefaults()
    {
        GetBoRemoteMsg = _ => false;
        g_sUserNotOnLine = "  没有在线！";
        GetInputBoxFilterList = () => null;
        GetInputBoxInFilterList = _ => false;
        GetPlayObject = _ => null;
    }
}
