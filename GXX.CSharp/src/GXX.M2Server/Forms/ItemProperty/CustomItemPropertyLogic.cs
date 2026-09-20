// ============================================================================
// uFrmCustomItemProperty.pas 的**纯逻辑**部分（可无头单测）
// 车道 p8-m2-itemprop-misc ｜ 命名空间 GXX.M2Server.Forms.ItemProperty
//
// 抽出理由（《并行派发台账》§25.2 / 无头 UI 规程）：窗体的取值/写回规则一旦藏在
// VCL 控件后面就无法断言。本文件把三块**不依赖控件生命周期**的语义抽成纯函数：
//   1. TStrings.Text ↔ 行集合 的往返（Delphi Classes.TStrings.GetTextStr / SetTextStr）；
//   2. 行号标签文本（原文 :489/:495 `'当前行：' + IntToStr(mmoVar.CaretPos.Y + 1)`）；
//   3. Low/High 界限常量（原文 :435 的 `Low(g_CustomItemPropertyBindNames) to High(...)`）。
// 控件的创建/销毁/ShowModal 仍留在接缝层（CustomItemPropertySeams.cs）。
// ============================================================================

using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.Core.Util;

namespace GXX.M2Server.Forms.ItemProperty;

/// <summary>uFrmCustomItemProperty.pas 用到的纯逻辑（无控件依赖）。</summary>
public static class CustomItemPropertyLogic
{
    /// <summary>
    /// Delphi <c>System.sLineBreak</c>（Windows = #13#10）。原文 M2Share.pas:22237
    /// <c>S := S + g_CustomItemPropertyBindNames[i] + sLineBreak;</c> 与
    /// TStrings.Text 的换行符都是它。
    /// </summary>
    public const string sLineBreak = "\r\n";

    /// <summary>
    /// <c>Low(g_CustomItemPropertyBindNames)</c>（M2Share.pas:3992 声明为 <c>array [1 .. 60]</c>）。
    /// </summary>
    public const int LowBindType = 1;

    /// <summary>
    /// <c>High(g_CustomItemPropertyBindNames)</c> =
    /// <c>CUSTOM_PROPERTY_BIND_TYPE_COUNT</c>（Grobal2.pas:64 = 60；托管侧 Grobal2Const 同名常量）。
    /// </summary>
    public const int HighBindType = Grobal2Const.CUSTOM_PROPERTY_BIND_TYPE_COUNT;

    /// <summary>
    /// Delphi <c>TStrings.GetTextStr</c>（classes.pas）：**每行后都追加 sLineBreak（含最后一行）**，
    /// 故 Count &gt; 0 时结果必以 CRLF 结尾；Count = 0 时返回空串。
    /// <para>
    /// 用途：原文 :304 <c>mmoVar.Text := g_CustomItemPropertyTextVarList.Text</c>、
    /// :472 <c>g_CustomItemPropertyTextVarList.Text := mmoVar.Text</c>。
    /// </para>
    /// <para>
    /// ★ 迁移记录（车道 p8-m2-itemprop-misc 请求 #3，已执行）：本方法原为临时实现
    /// （因 <c>GXX.Core.Util.TStringList</c> 缺 <c>Text</c>）；现 <c>TStringList.Text</c> 已按
    /// <c>TStrings.GetTextStr/SetTextStr</c> 补齐，本方法**降级为纯转调**，保留 Delphi 侧函数名。
    /// </para>
    /// </summary>
    public static string GetTextStr(TStringList list) => list == null ? "" : list.Text;

    /// <summary>
    /// Delphi <c>TStrings.SetTextStr</c>（classes.pas）：按 <c>#13</c> / <c>#10</c> / <c>#13#10</c>
    /// 切行并**先 Clear 再逐行 Add**。
    /// <para>边界（1:1，均有单测）：</para>
    /// <list type="bullet">
    /// <item><c>''</c> → 0 行（Count = 0）；</item>
    /// <item><c>"a\r\n"</c> → 1 行 <c>["a"]</c>（末尾 CRLF 不留空行）；</item>
    /// <item><c>"\r\n"</c> → 1 行 <c>[""]</c>（首字符即换行 → 先 Add 一个空串）；</item>
    /// <item><c>"a\n\nb"</c> → <c>["a","","b"]</c>（连续 LF 产生空行）；</item>
    /// <item>裸 <c>\r</c>（无 \n）也算换行（原文 <c>if P^ = #13 then Inc(P); if P^ = #10 then Inc(P);</c>）。</item>
    /// </list>
    /// <para>★ 迁移记录同 <see cref="GetTextStr"/>：现为纯转调 <c>TStringList.Text</c> 的 setter。</para>
    /// </summary>
    public static void SetTextStr(TStringList list, string value)
    {
        if (list == null) return;
        list.Text = value;
    }

    /// <summary>
    /// 原文 :489 / :495 <c>lblLineNum.Caption := '当前行：' + IntToStr(mmoVar.CaretPos.Y + 1);</c>
    /// <para>
    /// **无 Delphi 格式串**（纯 `+` 拼接），故不走 GXX.Core.Rtl.DelphiFormat（§17.2 只约束格式串）；
    /// IntToStr 的十进制语义由 <see cref="DelphiRTL.IntToStr(int)"/> 保证（InvariantCulture）。
    /// </para>
    /// </summary>
    public static string LineNumCaption(int caretPosY) => "当前行：" + DelphiRTL.IntToStr(caretPosY + 1);
}
