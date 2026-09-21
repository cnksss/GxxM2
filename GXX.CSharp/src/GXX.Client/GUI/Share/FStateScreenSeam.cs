using System;

namespace GXX.Client.GUI.Share;

// ============================================================================================
// FState.pas 引用、而本工程尚无托管实体的**画面层**接缝（车道 p14-client-fstate 切片 1）。
//
// 为什么单独一个文件：FState.pas 的十几个鼠标/绘制处理器都会在**第一步**调用
// `DScreen.ClearHint`（MShare.pas 的 `DScreen:TDrawScreen`，即 DrawScrn 单元的绘制屏幕对象）。
// 该对象在本工程尚未落地（Scenes 车道只有 `TDrawScreenScrn` 的部分方法），
// 因此这里给一个**显式留痕**的接缝：默认记一条 NotPorted 记录并计数，
// 使"处理器跑到这一步但画面层还没接上"这件事不可能被静默吞掉（照 §25.2）。
//
// 与 D-P10-06 的关系：`THintLines`/`THintWindows` 已有**正式归属**（GXX.Client.Scenes），
// 故本文件**不再**复刻它们；这里只补 `DScreen` 这一条**尚无归属**的接缝。
// ============================================================================================

/// <summary>
/// MShare.pas 的单元级全局 `DScreen:TDrawScreen`（FState.pas 内的调用点形如
/// `DScreen.ClearHint` / `DScreen.AddChatBoardString`）。
/// 【接缝：待 DrawScrn/MShare 车道的 TDrawScreen 落地后删除本类，改用真实对象】
/// </summary>
public static class FStateScreenSeam
{
    /// <summary>
    /// 每次调用 `ClearHint` 都记一条（原文 ClearHint 无副作用可观测，托管侧用计数留痕）。
    /// 测试可断言"处理器确实走到了 ClearHint 这一步"。
    /// </summary>
    public static readonly System.Collections.Generic.List<string> NotPortedCalls = new();

    /// <summary>ClearHint 被调用的累计次数（测试复位）。</summary>
    public static int ClearHintCount { get; private set; }

    /// <summary>TDrawScreen.ClearHint。【接缝：真实对象未落地】</summary>
    public static void ClearHint()
    {
        ClearHintCount++;
        NotPortedCalls.Add("DScreen.ClearHint (MShare.pas TDrawScreen，正式实现在 Scenes 车道未落地)");
    }

    /// <summary>测试复位。</summary>
    public static void ResetForTests()
    {
        ClearHintCount = 0;
        NotPortedCalls.Clear();
    }
}
