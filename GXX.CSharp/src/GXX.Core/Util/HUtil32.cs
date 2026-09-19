using System;
using System.Globalization;
using System.IO;
using System.Text;
using GXX.Core.Rtl;

namespace GXX.Core.Util;

/// <summary>
/// HUtil32.pas 1:1 转换：字符串截取/过滤/数值转换等 ~70 个工具函数。
/// 语义保持 Delphi 1-based；涉及 GDI/Canvas 的绘制函数属于 UI 层，归入客户端项目。
/// </summary>
public static class HUtil32
{
    public struct TyNameTable
    {
        public string Name;
        public int Value;
    }

    public struct TLRect
    {
        public int Left, Top, Right, Bottom;
    }

    public const int MAXDEFCOLOR = 16;
    public static readonly string[] LiMarkerNames = { "DISC", "CIRCLE", "SQUARE" };
    public static readonly string[] PreDefineNames = { "LEFT", "RIGHT", "CENTER" };

    public static int[] PosArray = Array.Empty<int>();

    // ---------------- ArrestString 系列 ----------------

    /// <summary>ArrestString：在 Source 中查找 SearchAfter 与 ArrestBefore 之间的内容。</summary>
    public static string ArrestString(string source, string searchAfter, string arrestBefore, string[] dropTags, ref string rsltStr)
    {
        string result;
        int srcLen = source.Length;
        int afterLen = searchAfter.Length;
        int beforeLen = arrestBefore.Length;
        int fixedLen = srcLen - beforeLen + 1;
        int curTagLen = 0;
        rsltStr = "";
        result = "";
        int srcPos;
        if (afterLen >= 1 && beforeLen >= 1)
        {
            if (srcLen >= afterLen)
            {
                for (srcPos = 1; srcPos <= fixedLen; srcPos++)
                {
                    if (Copy(source, srcPos, afterLen) == searchAfter)
                        break;
                }
                if (srcPos <= fixedLen)
                {
                    srcPos += afterLen;
                    int endPos = 1;
                    for (int findPos = srcPos; findPos <= srcLen; findPos++)
                    {
                        if (Copy(source, findPos, beforeLen) == arrestBefore)
                        {
                            endPos = findPos - 1;
                            break;
                        }
                    }
                    if (endPos >= srcPos)
                    {
                        rsltStr = Copy(source, srcPos, endPos - srcPos + 1);
                        if (dropTags != null)
                            for (int i = 0; i < dropTags.Length; i++)
                                rsltStr = ReplaceStr(rsltStr, dropTags[i], "");
                        result = Copy(source, endPos + beforeLen, srcLen);
                    }
                    else result = "";
                }
                else result = "";
            }
            else result = "";
        }
        else result = "";
        return result;
    }

    public static string ReplaceStr(string source, string oldValue, string newValue)
        => source == null ? "" : source.Replace(oldValue, newValue);

    /// <summary>ArrestVariable：Left 后必须紧跟 Center 才成立，返回 Left 位置（1-based）。</summary>
    public static int ArrestVariable(string source, char left, char center, char right, int startPos, ref string arrestStr)
    {
        arrestStr = "";
        int result = 0;
        int nPos = 0;
        if (source == "") return 0;
        int index = startPos;
        bool bo1D = false, bo2D = false;
        if (index <= 0) index = 1;
        while (true)
        {
            if (index > source.Length) break;
            if (source[index - 1] == left)
            {
                bo1D = true;
                nPos = index;
                index++;
                if (index > source.Length) break;
                if (source[index - 1] == center)
                {
                    bo2D = true;
                    index++;
                    if (index > source.Length) break;
                }
                else
                {
                    bo1D = false;
                    bo2D = false;
                }
            }
            if (bo1D && bo2D)
            {
                if (source[index - 1] == right)
                {
                    int nLen = index - nPos - 1;
                    arrestStr = Copy(source, nPos + 1, nLen);
                    result = nPos;
                    break;
                }
            }
            index++;
        }
        return result;
    }

    /// <summary>ArrestStringEx2（1-based；返回 Left 位置）。</summary>
    public static int ArrestStringEx2(string source, char left, char center, char right, ref string arrestStr)
    {
        arrestStr = "";
        int result = 0;
        int nPos = 0;
        if (source == "") return 0;
        bool bo1D = false, bo2D = false;
        for (int index = 1; index <= source.Length; index++)
        {
            if (source[index - 1] == left)
            {
                bo1D = true;
                nPos = index;
                continue;
            }
            if (source[index - 1] == center)
            {
                if (bo1D)
                {
                    bo2D = true;
                    continue;
                }
                bo1D = false;
                bo2D = false;
            }
            if (source[index - 1] == right)
            {
                if (bo1D && bo2D)
                {
                    int nLen = index - nPos - 1;
                    arrestStr = Copy(source, nPos + 1, nLen);
                    result = nPos;
                    break;
                }
                bo1D = false;
                bo2D = false;
            }
        }
        return result;
    }

    /// <summary>ArrestStringEx（WideString 版）。</summary>
    public static string ArrestStringEx(string source, char searchStart, char searchEnd, ref string arrestStr)
    {
        arrestStr = "";
        string result = "";
        int srcLen = source.Length;
        int startIdx = source.IndexOf(searchStart) + 1;
        if (startIdx > 0)
        {
            int endIdx = -1;
            for (int i = startIdx + 1; i <= srcLen; i++)
            {
                if (source[i - 1] == searchEnd) { endIdx = i; break; }
            }
            if (endIdx > 0)
            {
                arrestStr = Copy(source, startIdx + 1, endIdx - startIdx - 1);
                result = Copy(source, endIdx + 1, srcLen);
            }
            else arrestStr = Copy(source, startIdx + 1, srcLen);
        }
        return result;
    }

    public static string ArrestStringEx_Ansi(string source, char searchStart, char searchEnd, ref string arrestStr)
        => ArrestStringEx(source, searchStart, searchEnd, ref arrestStr);

    /// <summary>CaptureString：取首个空格分隔的词（支持引号）。</summary>
    public static string CaptureString(string source, ref string rdstr)
    {
        int st, et, c, len;
        if (source == "")
        {
            rdstr = "";
            return "";
        }
        c = 1;
        len = source.Length;
        while (source[c - 1] == ' ')
        {
            if (c < len) c++;
            else break;
        }
        if (source[c - 1] == '"' && c < len)
        {
            st = c + 1;
            et = len;
            for (int i = c + 1; i <= len; i++)
                if (source[i - 1] == '"') { et = i - 1; break; }
        }
        else
        {
            st = c;
            et = len;
            for (int i = c; i <= len; i++)
                if (source[i - 1] == ' ') { et = i - 1; break; }
        }
        rdstr = Copy(source, st, et - st + 1);
        if (len >= et + 2)
            return Copy(source, et + 2, len - (et + 1));
        return "";
    }

    // ---------------- GetValidStr 系列 ----------------

    /// <summary>GetValidStr3：Str 以 Divider 分隔，Dest 取出第一段，返回余下字符串。</summary>
    public static string GetValidStr3(string str, ref string dest, char[] divider)
    {
        const char CH_SPACE = ' ';
        int strLen = str.Length;
        int strPos = 1;
        int strAftPos;
        dest = "";
        if (strLen == 0) return str;
        if (str[0] == CH_SPACE)
        {
            str = KillFirstSpace(ref str);
            strLen = str.Length;
        }
        while (strPos <= strLen && str[strPos - 1] == CH_SPACE) strPos++;
        strAftPos = strPos;
        while (strPos <= strLen && Array.IndexOf(divider, str[strPos - 1]) < 0) strPos++;
        dest = Copy(str, strAftPos, strPos - strAftPos);
        while (strPos <= strLen && str[strPos - 1] == CH_SPACE) strPos++;
        return Copy(str, strPos, strLen);
    }

    public static string GetValidStr3_Ex(string str, ref string dest, char divider)
        => GetValidStr3(str, ref dest, new[] { divider });

    public static string GetValidStrCap(string str, ref string dest, char[] divider)
    {
        if (str.Length > 0 && str[0] >= 'a' && str[0] <= 'z')
            str = (char)(str[0] - 32) + str.Substring(1);
        return GetValidStr3(str, ref dest, divider);
    }

    public static string SkipStr(string src, char[] skips)
    {
        int i;
        int strPos = 1;
        int strLen = src.Length;
        while (strPos <= strLen)
        {
            for (i = 0; i < skips.Length; i++)
                if (src[strPos - 1] == skips[i]) break;
            if (i >= skips.Length) break;
            strPos++;
        }
        return Copy(src, strPos, strLen);
    }

    public static string CombineDirFile(string srcDir, string targName)
    {
        if (srcDir == "") return targName;
        if (targName == "") return srcDir;
        if (srcDir[srcDir.Length - 1] != '\\')
            if (targName.Length > 0 && targName[0] != '\\')
                return srcDir + "\\" + targName;
        if (srcDir[srcDir.Length - 1] == '\\' && targName.Length > 0 && targName[0] == '\\')
            return srcDir + targName.Substring(1);
        return srcDir + targName;
    }

    public static string KillFirstSpace(ref string str)
    {
        int cnt, len = str.Length;
        for (cnt = 1; cnt <= len; cnt++)
        {
            if (str[cnt - 1] != ' ')
            {
                str = Copy(str, cnt, len - cnt + 1);
                return str;
            }
        }
        return str;
    }

    public static void KillGabageSpace(ref string str)
    {
        int cnt, len = str.Length;
        for (cnt = len; cnt >= 1; cnt--)
        {
            if (str[cnt - 1] != ' ')
            {
                str = Copy(str, 1, cnt);
                KillFirstSpace(ref str);
                break;
            }
        }
    }

    public static int GetSpaceCount(string str)
    {
        int spaceCount = 0;
        for (int cnt = 0; cnt < str.Length; cnt++)
            if (str[cnt] == ' ') spaceCount++;
        return spaceCount;
    }

    public static string RemoveSpace(string str)
    {
        var sb = new StringBuilder();
        foreach (char c in str) if (c != ' ') sb.Append(c);
        return sb.ToString();
    }

    public static string GetFirstWord(string str, ref string sWord, ref int frontSpace)
    {
        int len = str.Length;
        if (len <= 0) return "";
        frontSpace = 0;
        int cnt;
        for (cnt = 1; cnt <= len; cnt++)
        {
            if (str[cnt - 1] == ' ') frontSpace++;
            else break;
        }
        var buf = new StringBuilder();
        for (int c2 = cnt; c2 <= len; c2++)
        {
            if (str[c2 - 1] != ' ') buf.Append(str[c2 - 1]);
            else
            {
                sWord = buf.ToString();
                return Copy(str, c2, len - c2 + 1);
            }
        }
        sWord = buf.ToString();
        return "";
    }

    // ---------------- 数值转换 ----------------

    public static int HexToIntEx(string shapStr) => HexToInt(Copy(shapStr, 2, shapStr.Length - 1));

    public static int HexToInt(string str)
    {
        int val = 0;
        int count = str.Length;
        for (int i = 1; i <= count; i++)
        {
            char digit = str[i - 1];
            int cur;
            if (digit >= '0' && digit <= '9') cur = digit - '0';
            else if (digit >= 'A' && digit <= 'F') cur = digit - 'A' + 10;
            else if (digit >= 'a' && digit <= 'f') cur = digit - 'a' + 10;
            else cur = 0;
            val += cur << (4 * (count - i));
        }
        return val;
    }

    public static int Str_ToInt(string str, int def)
    {
        int result = def;
        if (str != "")
        {
            char c0 = str[0];
            if ((c0 >= '0' && c0 <= '9') || c0 == '+' || c0 == '-')
                int.TryParse(str, out result);
        }
        return result;
    }

    public static float Str_ToFloatDef(string str, float def)
        => float.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out float v) ? v : def;

    public static double Str_ToFloat(string str)
    {
        if (str != "" && double.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out double v))
            return v;
        return 0;
    }

    public static string IntToStr2(int n)
    {
        string s = n.ToString();
        return s.PadLeft(4, '0');
    }

    public static string IntToStrFill(int num, int len, char fill)
        => num.ToString().PadLeft(len, fill);

    public static bool IsStringNumber(string str)
    {
        if (str == "") return false;
        for (int i = 0; i < str.Length; i++)
        {
            char c = str[i];
            if (i == 0 && (c == '-' || c == '+')) continue;
            if (c < '0' || c > '9') return false;
        }
        return true;
    }

    public static bool IsNumber(string str)
    {
        if (str == "") return false;
        foreach (char c in str)
            if (c < '0' || c > '9') return false;
        return true;
    }

    public static bool IsVarNumber(string str)
    {
        if (str == "") return false;
        foreach (char c in str)
            if ((c < '0' || c > '9') && c != '$' && c != 'A' && c != 'B' && c != 'C' && c != 'D' && c != 'E' && c != 'F'
                && c != 'a' && c != 'b' && c != 'c' && c != 'd' && c != 'e' && c != 'f')
                return false;
        return true;
    }

    public static bool IsFloatNumeric(string str)
        => double.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out _);

    public static bool IsUniformStr(string src, char ch)
    {
        if (src == "") return true;
        foreach (char c in src)
            if (c != ch) return false;
        return true;
    }

    public static bool IsEnglish(char ch) => (ch >= 'A' && ch <= 'Z') || (ch >= 'a' && ch <= 'z');
    public static bool IsEngNumeric(char ch) => (ch >= 'A' && ch <= 'Z') || (ch >= 'a' && ch <= 'z') || (ch >= '0' && ch <= '9');

    public static bool IsInB(string src, int pos, string targ)
    {
        if (src == "" || targ == "") return false;
        if (pos <= 0 || pos + targ.Length - 1 > src.Length) return false;
        return string.CompareOrdinal(src, pos - 1, targ, 0, targ.Length) == 0;
    }

    public static bool IsInRect(int x, int y, TLRect rect)
        => x >= rect.Left && x <= rect.Right && y >= rect.Top && y <= rect.Bottom;

    public static string Trim_R(string str) => DelphiRTL.TrimRight(str);

    public static string TrimEx(string s) => s.Trim(' ', '\t', '\r', '\n');

    /// <summary>CutHalfCode：截掉半个 GBK 双字节码。</summary>
    public static string CutHalfCode(string str)
    {
        byte[] data = EncodingInit.GBK.GetBytes(str);
        int len = data.Length;
        // 若最后一个字节是双字节字符的高半部，去掉它
        int cut = len;
        int i = 0;
        while (i < len)
        {
            if (data[i] > 0x81 && i + 1 < len && IsLeadByte(data[i])) i += 2;
            else i++;
        }
        if (i != len) cut = i;
        return EncodingInit.GBK.GetString(data, 0, cut);
    }

    private static bool IsLeadByte(byte b) => (b >= 0x81 && b <= 0xFE) && !(b >= 0xA1 && b <= 0xA9);

    // ---------------- 字符串截取辅助 ----------------

    public static string CatchString(string source, char cap, ref string catched)
    {
        int n = source.IndexOf(cap) + 1;
        if (n <= 0) { catched = ""; return ""; }
        int n2 = -1;
        for (int i = n + 1; i <= source.Length; i++)
            if (source[i - 1] == cap) { n2 = i; break; }
        if (n2 <= 0) { catched = ""; return ""; }
        catched = Copy(source, n + 1, n2 - n - 1);
        return Copy(source, n2 + 1, source.Length);
    }

    public static string DivString(string source, char cap, ref string sel)
    {
        int n = source.IndexOf(cap);
        if (n < 0) { sel = source; return ""; }
        sel = Copy(source, 1, n);
        return Copy(source, n + 2, source.Length);
    }

    public static string DivTailString(string source, char cap, ref string sel)
    {
        int n = source.LastIndexOf(cap);
        if (n < 0) { sel = source; return ""; }
        sel = Copy(source, n + 2, source.Length);
        return Copy(source, 1, n);
    }

    public static int SPos(string substr, string str) => str.IndexOf(substr, StringComparison.Ordinal) + 1;

    public static int NumCopy(string str)
    {
        string numstr = "";
        foreach (char c in str)
            if (c >= '0' && c <= '9') numstr += c;
        return numstr == "" ? 0 : int.Parse(numstr);
    }

    public static string GetMonDay() => DateTime.Now.ToString("M-d", CultureInfo.InvariantCulture);

    // ---------------- 比较辅助 ----------------

    public static bool CompareLStr(string src, string targ, int compn)
    {
        if (src.Length < compn || targ.Length < compn) return false;
        return string.CompareOrdinal(src, 0, targ, 0, compn) == 0;
    }

    public static bool CompareBackLStr(string src, string targ, int compn)
    {
        if (src.Length < compn || targ.Length < compn) return false;
        return string.CompareOrdinal(src, src.Length - compn, targ, targ.Length - compn, compn) == 0;
    }

    public static bool CompareBuffer(byte[] p1, byte[] p2, int len)
    {
        if (p1.Length < len || p2.Length < len) return false;
        for (int i = 0; i < len; i++)
            if (p1[i] != p2[i]) return false;
        return true;
    }

    public static string ReplaceChar(string src, char srcchr, char repchr)
    {
        var sb = new StringBuilder(src.Length);
        foreach (char c in src)
            sb.Append(c == srcchr ? repchr : c);
        return sb.ToString();
    }

    // ---------------- 文件辅助 ----------------

    public static int FileSizeOf(string fname)
    {
        try
        {
            var fi = new FileInfo(Path.GetFullPath(fname));
            if (fi.Exists) return (int)fi.Length;
            return 0;
        }
        catch { return 0; }
    }

    public static bool FileCopy(string source, string dest)
    {
        try
        {
            File.Copy(source, dest, true);
            return true;
        }
        catch { return false; }
    }

    public static bool FileCopyEx(string source, string dest) => FileCopy(source, dest);

    public static void GetDirList(string path, System.Collections.Generic.List<string> flList)
    {
        try
        {
            string dir = Path.GetDirectoryName(Path.GetFullPath(path)) ?? "";
            string mask = Path.GetFileName(path);
            if (mask == "") mask = "*.*";
            if (Directory.Exists(dir))
                foreach (string f in Directory.GetFiles(dir, mask))
                    flList.Add(f);
        }
        catch { }
    }

    // ---------------- 布尔转换 ----------------

    public static string BoolToStr(bool boo) => boo ? "True" : "False";
    public static string BoolToStr2(bool boo) => boo ? "1" : "0";
    public static string BooleanToStr(bool boo) => boo ? "True" : "False";
    public static string BoolToIntStr(bool boo) => boo ? "1" : "0";
    public static string BoolToCStr(bool boo) => boo ? "是" : "否";
    public static int BoolToInt(bool boo) => boo ? 1 : 0;
    public static bool StrToBool(string str) => Str_ToInt(str, 0) != 0;

    public static int GetDayCount(DateTime maxDate, DateTime minDate)
        => _MAX((int)(maxDate.Date - minDate.Date).TotalDays, 0);

    public static int GetCodeMsgSize(double x)
        => Math.Floor(x) < x ? (int)Math.Truncate(x) + 1 : (int)Math.Truncate(x);

    public static string IntToSex(byte btSex)
    {
        switch (btSex)
        {
            case 0: return "男";
            case 1: return "女";
            default: return "未知";
        }
    }

    public static string IntToJob(byte btJob)
    {
        switch (btJob)
        {
            case 0: return "战士";
            case 1: return "法师";
            case 2: return "道士";
            default: return "未知";
        }
    }

    public static bool IsIPaddr(string ip)
    {
        if (ip == "") return false;
        string[] parts = ip.Split('.');
        if (parts.Length != 4) return false;
        foreach (string p in parts)
        {
            if (p == "" || !int.TryParse(p, out int v) || v < 0 || v > 255) return false;
        }
        return true;
    }

    public static int MakeHumanFeature(byte btRaceImg, byte btDress, byte btWeapon, byte btHair)
        => DelphiRTL.MakeLong(DelphiRTL.MakeWord(btRaceImg, btWeapon), DelphiRTL.MakeWord(btHair, btDress));

    public static int MakeMonsterFeature(byte btRaceImg, byte btWeapon, ushort wAppr)
        => DelphiRTL.MakeLong(DelphiRTL.MakeWord(btRaceImg, btWeapon), wAppr);

    public static int FloatToStringHelper(double f) => 0; // 占位防误用（原 FloatToString 在 UI 层使用）

    public static string FloatToStrFixFmt(double fVal, int prec, int digit)
    {
        string fstr = fVal.ToString("G15", CultureInfo.InvariantCulture);
        var buf = new StringBuilder();
        int cnt = 0;
        bool dotDone = false;
        for (int i = 0; i < fstr.Length; i++)
        {
            char c = fstr[i];
            if (c == '.')
            {
                buf.Append('.');
                cnt = 0;
                for (int j = i + 1; j < fstr.Length; j++)
                {
                    if (cnt < digit) buf.Append(fstr[j]);
                    else return buf.ToString();
                    cnt++;
                }
                dotDone = true;
                break;
            }
            if (cnt < prec) buf.Append(c);
            cnt++;
        }
        _ = dotDone;
        return buf.ToString();
    }

    public static string FloatToString(double f) => FloatToStrFixFmt(f, 5, 2);

    public static string sub_49ADB8(int nPos, string sMsg, string sStr, string sText)
    {
        if (nPos > 0)
        {
            string s14 = Copy(sMsg, 1, nPos - 1);
            string s18 = Copy(sMsg, sStr.Length + nPos, sMsg.Length);
            return s14 + sText + s18;
        }
        int n10 = Pos(sStr, sMsg);
        if (n10 > 0)
        {
            string s14 = Copy(sMsg, 1, n10 - 1);
            string s18 = Copy(sMsg, sStr.Length + n10, sMsg.Length);
            return s14 + sText + s18;
        }
        return sMsg;
    }

    public static int Pos(string substr, string s) => DelphiRTL.Pos(substr, s);
    public static string Copy(string s, int index, int count) => DelphiRTL.Copy(s, index, count);
    public static int _MIN(int n1, int n2) => DelphiRTL._MIN(n1, n2);
    public static int _MAX(int n1, int n2) => DelphiRTL._MAX(n1, n2);
    public static uint _MinLong(uint n1, uint n2) => DelphiRTL._MinLong(n1, n2);
    public static uint _MaxLong(uint n1, uint n2) => DelphiRTL._MaxLong(n1, n2);

    public static int TagCount(string source, char tag)
    {
        int count = 0;
        foreach (char c in source) if (c == tag) count++;
        return count;
    }

    public static TLRect LRect(int l, int t, int r, int b)
        => new TLRect { Left = l, Top = t, Right = r, Bottom = b };
}
