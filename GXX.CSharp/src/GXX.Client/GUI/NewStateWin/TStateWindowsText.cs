namespace GXX.Client.GUI.NewStateWin;

/// <summary>
/// StateWindows.pas 的两段**顶层文本处理**函数移植（原文 10776-10841 行）。
///
///   - GetGodBlessItemCaption  10776-10804  神佑格提示标题（12 生肖常量表 + 配置覆盖）
///   - GetHitLines             10768-10841  神佑格提示文字「一行多个颜色」解析
///
/// 接缝：GetGodBlessItem（MShare.pas，从文件配置读单条描述）以委托传入。
/// 接缝：THintLines / GetHintFontSize / GetHintFontStyle / GetHintFontStroke（FState.pas）未移植，
/// 本文件只产出「文本 + 颜色」序列（等价于 HintLines.Add 的首两个实参）。
/// 接缝：GetRGB / g_DefColorTable（MShare.pas:4111-4114）未移植，以委托注入颜色解析。
/// </summary>
public static class TStateWindowsText
{
    /// <summary>
    /// 原文 10778-10790：Captions:array[0..11]。
    /// 「原文如此（StateWindows.pas:10784）」：第 6 项写作 '已蛇'（地支本字为「巳」），照抄不改。
    /// </summary>
    public static readonly string[] GodBlessCaptions =
    {
        "子鼠", "丑牛", "寅虎", "卯兔", "辰龙", "已蛇",
        "午马", "未羊", "申猴", "酉鸡", "戌狗", "亥猪",
    };

    /// <summary>
    /// 原文 10776-10804 的 GetGodBlessItemCaption。
    /// 取值顺序：
    ///   1) Index 在 [0..11] 时先查配置项 'title' + IntToStr(Index + 1)；
    ///   2) 配置存在且 Count &gt; 0 → 取第 0 行；
    ///   3) 否则 → Captions[Index] + '神佑格'；
    ///   4) Index 越界 → '神佑格' + IntToStr(Index + 1)（**不是** Captions 的兜底）。
    /// </summary>
    /// <param name="index">神佑格序号（0-based）</param>
    /// <param name="getGodBlessItem">
    /// 配置读取接缝（MShare.pas 的 GetGodBlessItem）；
    /// 返回 null 表示未配置。元素为 TStringList 的字符串序列。
    /// </param>
    public static string GetGodBlessItemCaption(int index, Func<string, IReadOnlyList<string>> getGodBlessItem)
    {
        // 原文 10795：if (Index >= Low(Captions)) and (Index <= High(Captions)) then
        if (index >= 0 && index <= GodBlessCaptions.Length - 1)
        {
            // 原文 10796：GetGodBlessItem('title' + IntToStr(Index + 1))
            var itemDesc = getGodBlessItem?.Invoke("title" + (index + 1));
            if (itemDesc != null && itemDesc.Count > 0)
                return itemDesc[0];

            return GodBlessCaptions[index] + "神佑格";
        }

        return "神佑格" + (index + 1);
    }

    /// <summary>命中提示的一行（对应 THintLines 内一项的文本与颜色）。</summary>
    public readonly struct HintLine
    {
        /// <summary>提示文本</summary>
        public readonly string Text;
        /// <summary>颜色（原文 TColor，由颜色提供方给出）</summary>
        public readonly int Color;

        public HintLine(string text, int color)
        {
            Text = text;
            Color = color;
        }
    }

    /// <summary>
    /// 原文 10808-10841 的 GetHitLines：把 '文字/RRGGBB/文字/...' 拆成多行带色文本。
    /// 逐字要点：
    ///   - 无 '/' 时整串作一行（原文 10815-10816），不做颜色解析；
    ///   - '/' 前的数字串被当作颜色值，从 nPos-1 向前逐字符扫描；
    ///   - `if I &lt; nPos - 4 then I := nPos - 4`（最多回退 4 位，即颜色串不超过 5 位）；
    ///   - `I = 0`（'/' 在串首且前面没有非数字）时**不加行**，只消费掉该 '/'；
    ///   - 颜色值用 StrToIntDef(..., 255)（原文 10832），再把 S 截到下一个 '/' 之后；
    ///   - 结尾剩余非空串追加一行（原文 10838-10839）。
    /// </summary>
    /// <param name="s">原始提示串</param>
    /// <param name="defColor">默认颜色（原文参数 DefColor:TColor）</param>
    /// <param name="getRGB">
    /// 颜色值 → TColor 的解析接缝（MShare.pas:4111 GetRGB(c256:Byte)）；
    /// 传 null 时color 字段直接返回解析出的数值。
    /// </param>
    public static List<HintLine> GetHitLines(string s, int defColor, Func<int, int> getRGB = null)
    {
        var result = new List<HintLine>();
        if (s == null)
            s = string.Empty;

        int nPos = Pos(s, "/");
        if (nPos == 0)
        {
            result.Add(new HintLine(s, defColor));
            return result;
        }

        Func<int, int> resolve = getRGB ?? (v => v);

        int color = defColor;

        while (nPos > 0)
        {
            // 原文 10820-10822：
            //   for I := nPos - 1 downto 1 do if not (S[I] in ['0'..'9']) then Break;
            // Delphi 语义：初值 < 终值 1 时循环体一次都不执行，循环变量停在**初值**（nPos-1）。
            // nPos = 1 时初值 0 → I 保持 0 → 走「I = 0」分支（原文 10826-10829）。
            int I = nPos - 1;
            if (I >= 1)
            {
                for (; I >= 1; I--)
                {
                    char c = s[I - 1];
                    if (c < '0' || c > '9')
                        break;
                }
            }

            if (I < nPos - 4)
                I = nPos - 4;
            if (I == 0)
            {
                s = Copy(s, nPos + 1, int.MaxValue);
                nPos = Pos(s, "/");
            }
            else
            {
                result.Add(new HintLine(Copy(s, 1, I), color));

                // 原文 10832：Color := GetRGB(StrToIntDef(Copy(S, I + 1, nPos - I - 1), 255));
                int colorValue = StrToIntDef(Copy(s, I + 1, nPos - I - 1), 255);
                color = resolve(colorValue);

                s = Copy(s, nPos + 1, int.MaxValue);
                nPos = Pos(s, "/");
            }
        }

        if (s.Length > 0)
            result.Add(new HintLine(s, color));

        return result;
    }

    // ===================== Delphi RTL 语义垫片（本地，避免与 GXX.Core.Rtl 口径混用） =====================

    /// <summary>Delphi Pos：1-based，找不到返回 0。</summary>
    private static int Pos(string s, string sub)
    {
        if (string.IsNullOrEmpty(sub)) return 0;
        int idx = s.IndexOf(sub, StringComparison.Ordinal);
        return idx < 0 ? 0 : idx + 1;
    }

    /// <summary>
    /// Delphi Copy(s, index, count)：1-based，index &lt; 1 会被钳到 1；越界返回空串。
    /// </summary>
    private static string Copy(string s, int index, int count)
    {
        if (count <= 0) return string.Empty;
        if (index < 1) index = 1;
        if (index > s.Length) return string.Empty;
        int len = Math.Min(count, s.Length - index + 1);
        return s.Substring(index - 1, len);
    }

    /// <summary>Delphi StrToIntDef：解析失败或空串返回默认值。</summary>
    private static int StrToIntDef(string s, int def)
    {
        if (string.IsNullOrEmpty(s)) return def;
        s = s.Trim();
        if (s.Length == 0) return def;
        return int.TryParse(s, out int v) ? v : def;
    }
}
