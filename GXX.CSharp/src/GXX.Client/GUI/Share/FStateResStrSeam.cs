using System;
using System.Collections.Generic;
using GXX.Core.Util;

namespace GXX.Client.GUI.Share;

// ============================================================================================
// FState.pas 引用、而本单元不得直接占有归属的**资源串 / 全局**接缝（车道 p14-client-fstate 切片 2）。
//
// 为什么单独一个文件：`FState.pas` 的对话框处理器会调
//   `DecodeResStr(SGuildDelMem)` / `DecodeResStr(SGuildEditNotice)` …（原文 17901/17908/17924 等）
// 这些 `S*` 常量定义在 **ClFunc.pas / MShare.pas 的 resourcestring**（不在本单元），
// 而 `DecodeResStr` 是 HUtil32.pas 的单元级函数。托管侧两者都还没有正式归属
// （`GXX.Client.GUI.GameConfig.Mir.MirConfigSeams.cs` 有一份私有同名接缝，但那是别的命名空间、
//  不可跨用）。按本工程的既有做法，这里落一个**最小注入接缝**：
//
//   * 默认值 = 原文的常量名本身（不是空串）—— 这样"忘了注入"表现为**可见的占位**，
//     而不是静默变成空提示（§25.2 的反静默原则）；
//   * 真实文本待 ClFunc/MShare 车道的 resourcestring 落地后一次性替换；
//   * 本类**不计入 FState 的未移植缺口**（源不在 FState.pas）。
// ============================================================================================

/// <summary>
/// ClFunc.pas / MShare.pas 的 resourcestring 与 HUtil32.pas 的 `DecodeResStr` 接缝。
/// 【接缝：待 ClFunc/MShare 车道落地 resourcestring 后删除本类，改用正式常量】
/// </summary>
public static class FStateResStrSeam
{
    /// <summary>HUtil32.pas `function DecodeResStr(s:string):string`（资源串解码）。</summary>
    public static Func<string, string> DecodeResStr = s => s ?? "";

    /// <summary>MShare.pas resourcestring `SGuildDelMem`（原文 17901 调用点）。</summary>
    public static string SGuildDelMem = "SGuildDelMem";

    /// <summary>MShare.pas resourcestring `SGuildEditNotice`（原文 17908 调用点）。</summary>
    public static string SGuildEditNotice = "SGuildEditNotice";

    /// <summary>MShare.pas resourcestring `SGuildEditGradeHint`（原文 17924 调用点）。</summary>
    public static string SGuildEditGradeHint = "SGuildEditGradeHint";

    /// <summary>MShare.pas resourcestring `SGuildAddMem`（原文 17894 调用点；含一个 `%s`）。</summary>
    public static string SGuildAddMem = "SGuildAddMem";

    /// <summary>MShare.pas resourcestring `SGuildAllyAsk`（原文 17930 调用点；含两个 `%s`）。</summary>
    public static string SGuildAllyAsk = "SGuildAllyAsk";

    /// <summary>MShare.pas resourcestring `SGuildAllyScript`（原文 17931 调用点）。</summary>
    public static string SGuildAllyScript = "SGuildAllyScript";

    /// <summary>MShare.pas resourcestring `SGuildBreakAllyAsk`（原文 17936 调用点）。</summary>
    public static string SGuildBreakAllyAsk = "SGuildBreakAllyAsk";

    /// <summary>MShare.pas resourcestring `SGuildBreakAllyScript`（原文 17938 调用点）。</summary>
    public static string SGuildBreakAllyScript = "SGuildBreakAllyScript";

    /// <summary>
    /// Delphi `System.sLineBreak`（本工程按 Windows 口径 = CRLF）。
    /// 原文 17930 用它填 `SGuildAllyAsk` 的两个 `%s`（弹窗里两行空行）。
    /// 【接缝：`GXX.Core.Rtl.DelphiRTL` 目前没有 sLineBreak 常量，故在此按原文语义落一份】
    /// </summary>
    public const string sLineBreak = "\r\n";

    /// <summary>测试复位。</summary>
    public static void ResetForTests()
    {
        DecodeResStr = s => s ?? "";
        SGuildDelMem = "SGuildDelMem";
        SGuildEditNotice = "SGuildEditNotice";
        SGuildEditGradeHint = "SGuildEditGradeHint";
        SGuildAddMem = "SGuildAddMem";
        SGuildAllyAsk = "SGuildAllyAsk";
        SGuildAllyScript = "SGuildAllyScript";
        SGuildBreakAllyAsk = "SGuildBreakAllyAsk";
        SGuildBreakAllyScript = "SGuildBreakAllyScript";
    }
}
