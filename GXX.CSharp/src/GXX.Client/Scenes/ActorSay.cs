using System;
using System.Collections.Generic;

namespace GXX.Client.Scenes;

/// <summary>
/// Actor.pas TActor.Say（9846-9932）headless 1:1 —— 头顶喊话的标记剥离与换行排版。
///
/// 原文流程：
///   1. m_dwSayTime := TimeGetTime；m_nSayLineCount := 0
///   2. 若 Pos('{', Str) &gt; 0 → GetTextListEx 解析标记后 GetStrinLineExText 还原纯文本
///      （标记形如 {自定义文字颜色|249:0} 或 {aabbcc|249:200:0}，整段连同载荷一并丢弃）
///   3. 清空 m_SayingArr[0..MAXSAY-1] 的 Text/TextSurface
///   4. 逐字节累积 temp，遇 byte &gt;= 128 视为双字节字符一次吞两个字节；
///      每次累积后问 CurrentFont.TextWidth(temp) &gt; MAXWIDTH(150)？
///        - 是 → 落行（n 行）→ Inc(m_nSayLineCount)、Inc(n)
///               → **若 n &gt;= MAXSAY(5) 则 loop := False 并 Break**（此时 temp 尚未清零！）
///               → 否则 Str := Copy(Str, I + 1, len - I)、temp := ''、Break
///        - 否 → Inc(I) 继续
///      内层因 I &gt; len 结束时 loop := False
///   5. 内层跳出后若 temp &lt;&gt; '' 且 n &lt; MAXWIDTH（原文如此，疑为 MAXSAY 笔误，逐字保留）
///      → 落行并 Inc(m_nSayLineCount)
///
/// ⚠ 原文缺陷（本移植逐字保留其**可观测行为**，但不复制其内存破坏）：
///   第 4 步在 n 达到 MAXSAY(5) 时因 9911-9914 的 Break 跳过了 9915-9916 的
///   `Str := Copy(...)` 与 `temp := ''`，于是 temp 仍持有整行文本，
///   导致步骤 5 再落一行 —— 写入 m_SayingArr[5]，而该数组仅有 [0..4] **越界写**，
///   且 m_nSayLineCount 被多加 1。C# 侧以固定 5 槽 + OverflowLines 显式承载该越界行。
/// </summary>
public static class ActorSay
{
    /// <summary>Say 内的 MAXWIDTH 常量（9853）。</summary>
    public const int MaxWidth = 150;

    /// <summary>m_SayingArr 长度（Actor.pas 42：MAXSAY = 5）。</summary>
    public const int MaxSay = 5;

    /// <summary>落行产物（m_SayingArr[n].Text 的 headless 镜像）。</summary>
    public sealed record SayLine(int Index, string Text);

    /// <summary>
    /// Say 排版产物：正常槽位 + 原文越界行（Index &gt;= MaxSay 者为越界）。
    /// </summary>
    public sealed class SayLayout
    {
        public readonly List<SayLine> Lines = new();

        /// <summary>原文会越界写入 m_SayingArr[Index] 的行（Index &gt;= MaxSay）。</summary>
        public readonly List<SayLine> OverflowLines = new();

        /// <summary>m_nSayLineCount（原文计数含越界行）。</summary>
        public int LineCount => Lines.Count + OverflowLines.Count;
    }

    /// <summary>
    /// 标记剥离 1:1（Actor.pas 9860-9868 + DxMemo 2394-2459 GetTextListEx/GetStrinLineExText 语义）：
    /// 逐个 '{' 寻找其后首个 '}'；成对则整段（含 '}'）丢弃，未配对则 '{' 原样保留；
    /// 原文对 '{' 存在与否先做 Pos 判定，不存在时完全不解析。
    /// </summary>
    public static string StripColorMarkup(string str)
    {
        if (str.IndexOf('{') < 0)
            return str;

        var sb = new System.Text.StringBuilder(str.Length);
        int i = 0;
        while (i < str.Length)
        {
            if (str[i] == '{')
            {
                int close = str.IndexOf('}', i);
                if (close >= 0)
                {
                    // 整段标记丢弃：{...} → 不含首尾括号
                    i = close + 1;
                    continue;
                }

                // 未配对：'}' 不存在 → ProcessCustomColor 返回 False，'{' 作为普通字符保留
                sb.Append(str[i]);
                i++;
                continue;
            }

            sb.Append(str[i]);
            i++;
        }

        return sb.ToString();
    }

    /// <summary>
    /// Say 1:1（9846-9932）：剥离标记 → 清空旧行 → 双字节感知换行 → 返回落行结果
    /// （含原文越界行，见类型注释）。
    /// <paramref name="textWidth"/> 为 CurrentFont.TextWidth 的等效度量（按字节串求宽）。
    /// </summary>
    public static SayLayout PlanSay(string str, Func<string, int> textWidth)
    {
        var layout = new SayLayout();

        // 原文第一件事就是清空全部槽位（9870-9875）
        str = StripColorMarkup(str);

        int n = 0;
        bool loop = true;

        while (loop)
        {
            string temp = "";
            int i = 1;                                  // Delphi 1-based
            int len = str.Length;

            while (true)
            {
                if (i > len)
                {
                    loop = false;
                    break;
                }

                char c = str[i - 1];
                if ((byte)c >= 128 || c > 0xFF)
                {
                    // 双字节字符：一次吞两个字节
                    temp += c;
                    i++;
                    if (i <= len)
                    {
                        temp += str[i - 1];
                    }
                    else
                    {
                        loop = false;
                        break;
                    }
                }
                else
                {
                    temp += c;
                }

                int aline = textWidth(temp);
                if (aline > MaxWidth)
                {
                    Add(layout, n, temp);
                    n++;
                    if (n >= MaxSay)
                    {
                        // 原文 9911-9914：Break 时 temp 尚未清零、Str 也未截断，
                        // 控制流落到外层 9921 的 `if temp <> ''` → 多落一行（越界写）
                        loop = false;
                        break;
                    }

                    // Str := Copy(Str, I + 1, len - I)
                    str = i + 1 <= str.Length ? str.Substring(i) : "";
                    temp = "";
                    break;
                }

                i++;
            }

            if (temp != "")
            {
                // 原文 `if n < MAXWIDTH then`（MAXWIDTH=150，与 MAXSAY=5 不同）——逐字保留此笔误
                if (n < MaxWidth)
                {
                    Add(layout, n, temp);
                    n++;
                }
            }
        }

        return layout;
    }

    /// <summary>落行：Index &lt; MaxSay 入正常槽，否则记入越界集合（原文越界写的显式化）。</summary>
    private static void Add(SayLayout layout, int index, string text)
    {
        var line = new SayLine(index, text);
        if (index < MaxSay)
            layout.Lines.Add(line);
        else
            layout.OverflowLines.Add(line);
    }
}

/// <summary>
/// TActor.Say 的场景侧落地（批次J76）：把 PlanSay 结果写入 m_SayingArr 并打点 m_dwSayTime。
/// </summary>
public partial class TActorCore
{
    /// <summary>CurrentFont.TextWidth 接缝。</summary>
    public static Func<string, int> TextWidthFn = s => s.Length * 6;

    /// <summary>
    /// Say 1:1（9846-9932）：打点 → m_nSayLineCount 归零 → 全槽清空 → 排版落行。
    /// 返回排版产物（含原文越界行）；越界行**不写入** m_SayingArr（原文为越界写，
    /// C# 侧显式丢弃以免破坏内存），但 m_nSayLineCount 仍按原文计数含之。
    /// </summary>
    public List<ActorSay.SayLine> Say(string str)
    {
        EnsureSayingSlots();

        m_dwSayTime = SceneTime.TickNow();
        m_nSayLineCount = 0;

        // 全槽清空（9870-9875）
        for (int i = 0; i < SayingArr.Count; i++)
        {
            SayingText[i] = "";
            SayingArr[i].Width = 0;
            SayingArr[i].Height = 0;
            SayingArr[i].HasImage = false;
        }

        var layout = ActorSay.PlanSay(str, TextWidthFn);

        foreach (var line in layout.Lines)
        {
            SayingText[line.Index] = line.Text;
        }

        // 原文 m_nSayLineCount 逐次 Inc，含越界那一行
        m_nSayLineCount = layout.LineCount;
        SayOverflowLines = layout.OverflowLines;

        return layout.Lines;
    }

    /// <summary>上一次 Say 中原文会越界写入 m_SayingArr 的行（headless 审计位）。</summary>
    public List<ActorSay.SayLine> SayOverflowLines = new();
}
