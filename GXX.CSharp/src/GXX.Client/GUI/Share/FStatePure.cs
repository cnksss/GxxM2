using System;
using System.IO;
using System.Text;
using GXX.Client.GUI.DxComponent;
using GXX.Client.GUI.Mir;
using static GXX.Client.GUI.Mir.MShareGlobals;

namespace GXX.Client.GUI.Share;

// ============================================================================================
// FState.pas 的**单元级纯函数**（原文 implementation 段，无 UI/控件树依赖）。
// 覆盖原文行号：
//   3650-3684 procedure GetHitLines(HintLines:THintLines; S:string; DefColor:TColor);  ✅
//   4257-4269 function  GetJobText(nJob:Integer):string;                                ✅
//   4271-4283 function  GetJobTextEx(nJob:Integer):string;                              ✅
//   1135-1152 procedure WriteOutStr(Msg:string);                                        ✅
// ============================================================================================

/// <summary>FState.pas 的单元级（非 TFrmDlg 成员）函数与过程。</summary>
public static class FStatePure
{
    /// <summary>
    /// FState.pas:4257-4269 function GetJobText(nJob:Integer):string。
    /// 0→'战' / 1→'法' / 2→'道' / 其他→'?'（case 无 0..2 之外的显式分支）。
    /// </summary>
    public static string GetJobText(int nJob)
    {
        switch (nJob)
        {
            case 0:
                return "战";
            case 1:
                return "法";
            case 2:
                return "道";
            default:
                return "?";
        }
    }

    /// <summary>
    /// FState.pas:4271-4283 function GetJobTextEx(nJob:Integer):string。
    /// 与 <see cref="GetJobText"/> 的差异：0/1/2 取 g_ConfigClient.ItemHintTextConfig 的
    /// httNeedJobWarr/Wizard/Taos 项文本；**其他分支同样返回 '?'**（原文如此）。
    /// </summary>
    public static string GetJobTextEx(int nJob)
    {
        switch (nJob)
        {
            case 0:
                return ConfigClientExt.ItemHintTextConfig[(int)ConfigClientExt.TItemHintTextType.httNeedJobWarr].Text;
            case 1:
                return ConfigClientExt.ItemHintTextConfig[(int)ConfigClientExt.TItemHintTextType.httNeedJobWizard].Text;
            case 2:
                return ConfigClientExt.ItemHintTextConfig[(int)ConfigClientExt.TItemHintTextType.httNeedJobTaos].Text;
            default:
                return "?";
        }
    }

    /// <summary>
    /// FState.pas:3650-3684 procedure GetHitLines(HintLines:THintLines; S:string; DefColor:TColor)。
    ///
    /// 原文的 "/数字" 颜色转义语法：`文本/255文本2/0文本3`，其中斜杠前的 1..3 位十进制数是
    /// GetRGB 的调色板下标。逐字保留的边界行为：
    ///   1) 无 '/' 时整串按 DefColor 加一行；
    ///   2) 找 '/' 前最多 3 位数字（`if I &lt; nPos - 4 then I := nPos - 4;` 把窗口夹到 3 位）；
    ///   3) I = 0（'/' 在串首，或其前无数字）→ **丢弃该 '/'** 继续（不产出行）；
    ///   4) 数字串用 StrToIntDef(..., 255) 解析，失败按 255（调色板末位=白）；
    ///   5) 循环结束后剩余非空串按最后一行的颜色补一行。
    ///
    /// 注意 `for I := nPos - 1 downto 1`：nPos 是 1-based，故 I 从 '/' 前一个字符开始往前扫；
    /// 若 I 一直退到 0 仍未 Break，说明 '/' 前全是数字（或紧邻串首）。
    /// </summary>
    public static void GetHitLines(THintLines HintLines, string S, TColor DefColor)
    {
        int nPos = Pos(S, '/');
        if (nPos == 0)
        {
            // 原文：HintLines.Add(S, DefColor, GetHintFontSize, GetHintFontStyle([]), GetHintFontStroke(True))
            HintLines.Add(S, DefColor, MShareHintFont.GetHintFontSize(),
                MShareHintFont.GetHintFontStyle(TFontStyles.fsNone), MShareHintFont.GetHintFontStroke(true));
        }
        else
        {
            TColor Color = DefColor;
            while (nPos > 0)
            {
                int I;
                for (I = nPos - 1; I >= 1; I--)
                {
                    // 原文 `if not (S[I] in ['0'..'9']) then Break;`
                    if (!(S[I - 1] >= '0' && S[I - 1] <= '9'))
                        break;
                }

                if (I < nPos - 4)
                    I = nPos - 4;
                if (I == 0)
                {
                    S = CopyFrom(S, nPos + 1);
                    nPos = Pos(S, '/');
                }
                else
                {
                    HintLines.Add(CopyFrom(S, 1, I), Color, MShareHintFont.GetHintFontSize(),
                        MShareHintFont.GetHintFontStyle(TFontStyles.fsNone), MShareHintFont.GetHintFontStroke(true));
                    // 原文 Color := GetRGB(StrToIntDef(Copy(S, I + 1, nPos - I - 1), 255))
                    // GetRGB(c256:Byte)：Delphi 把 Integer 隐式窄化到 Byte（无范围检查时截断低 8 位），
                    // C# 的 (byte) 转换同语义（unchecked 截断）。
                    Color = new TColor(GetRGB((byte)StrToIntDef(CopyFrom(S, I + 1, nPos - I - 1), 255)));
                    S = CopyFrom(S, nPos + 1);
                    nPos = Pos(S, '/');
                }
            }

            if (S.Length > 0)
                HintLines.Add(S, Color, MShareHintFont.GetHintFontSize(),
                    MShareHintFont.GetHintFontStyle(TFontStyles.fsNone), MShareHintFont.GetHintFontStroke(true));
        }
    }

    /// <summary>
    /// FState.pas:1135-1152 procedure WriteOutStr(Msg:string)。
    /// 调试日志：向 `.\Test.txt` 追加 `TimeToStr(Time) + ' ' + Msg` 一行。
    /// 原文用 Delphi 的 Append/Rewrite 二分支（文件存在则追加，否则重写）；
    /// 托管侧用 File.AppendAllText（不存在则创建），语义等价且少一次存在性判断 —— 该改写已在
    /// 交付报告登记。时间格式保留 Delphi 的 TimeToStr（HH:NN:SS）。
    /// </summary>
    public static void WriteOutStr(string Msg)
    {
        string flname = ".\\Test.txt";
        File.AppendAllText(flname, DelphiTimeToStr(DateTime.Now) + " " + Msg + Environment.NewLine, Encoding.UTF8);
    }

    /// <summary>原文 WriteOutStr 内的 flname（供测试断言/清理）。</summary>
    public const string WriteOutStrFileName = ".\\Test.txt";

    /// <summary>Delphi SysUtils.TimeToStr 的默认格式（HH:NN:SS，24 小时制）。</summary>
    public static string DelphiTimeToStr(DateTime t)
        => t.Hour.ToString("D2") + ":" + t.Minute.ToString("D2") + ":" + t.Second.ToString("D2");

    // ---------------------------------------------------------------------------------------
    // Delphi 字符串原语（1-based）的局部等价实现，使上面的移植保持原文形态。
    // ---------------------------------------------------------------------------------------

    /// <summary>Delphi `Pos(SubStr, S)`：1-based，未找到返回 0。</summary>
    private static int Pos(string s, char sub)
    {
        int i = s.IndexOf(sub);
        return i < 0 ? 0 : i + 1;
    }

    /// <summary>
    /// Delphi `Copy(S, Index, Count)`：Index 为 1-based。
    /// 原文语义：Index &gt; Length(S) → 空串；Count 超出则截到串尾；Count 省略/&gt; MaxInt 取到串尾。
    /// Index &lt; 1 时 Delphi 会抛 ERangeError（此处以钳到 1 处理并注释——调用点只传 ≥1）。
    /// </summary>
    private static string CopyFrom(string s, int index, int count = int.MaxValue)
    {
        if (index > s.Length) return "";
        int start = index - 1;
        if (start < 0) start = 0;
        int len = count;
        if (len < 0) len = 0;
        // 注意用减法而不是 `start + len > s.Length`：count 的默认值是 int.MaxValue，
        // 相加会溢出成负数从而绕过夹取，最终让 Substring 抛 ArgumentOutOfRange。
        if (len > s.Length - start) len = s.Length - start;
        return s.Substring(start, len);
    }

    /// <summary>Delphi `StrToIntDef(S, Def)`：解析失败或空串返回 Def。</summary>
    private static int StrToIntDef(string s, int def)
    {
        if (string.IsNullOrEmpty(s)) return def;
        return int.TryParse(s, System.Globalization.NumberStyles.AllowLeadingSign,
            System.Globalization.CultureInfo.InvariantCulture, out int v) ? v : def;
    }
}
