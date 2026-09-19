using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace GXX.Core.Util;

/// <summary>
/// UnitExpCalc.pas 1:1 语义转换（唐齐时数学表达式解析计算引擎）。
/// 支持：四则运算/括号/正负、百分数 %、幂 ^（右结合）、阶乘 !（1..150）、
/// 符号参数（AddSignParam）、常数 e/PI、以及原单元文档列出的全部函数。
/// </summary>
public static class ExpCalc
{
    private static readonly Dictionary<string, double> SignParams = new(StringComparer.Ordinal);

    // ---------------- 公共 API ----------------

    public static bool CalcExp(string aExpT, out double calResult, out string info)
    {
        calResult = 0;
        info = "";
        try
        {
            if (string.IsNullOrWhiteSpace(aExpT))
            {
                info = "表达式为空";
                return false;
            }
            var parser = new Parser(aExpT);
            calResult = parser.ParseExpression();
            parser.ExpectEnd();
            return true;
        }
        catch (ExpCalcException ex)
        {
            info = ex.Message;
            return false;
        }
        catch (Exception ex)
        {
            info = "计算错误: " + ex.Message;
            return false;
        }
    }

    public static bool CheckCalcExp(string expT, out string info)
    {
        return CalcExp(expT, out _, out info);
    }

    /// <summary>AddSignParam(['a','b'], [1,2])。</summary>
    public static int AddSignParam(string[] aSignList, double[] aSignValue)
    {
        int result = 0;
        if (aSignList == null) return 0;
        for (int i = 0; i < aSignList.Length; i++)
        {
            string name = aSignList[i];
            if (string.IsNullOrEmpty(name)) return result;
            if (!char.IsAsciiLetter(name[0])) return result;
            foreach (char c in name)
                if (!(char.IsAsciiLetterOrDigit(c) || c == '_')) return result;
        }
        int minPN = Math.Min(aSignList.Length, aSignValue.Length) - 1;
        for (int i = 0; i <= minPN; i++)
        {
            result++;
            SignParams[aSignList[i]] = aSignValue[i];
        }
        return result;
    }

    /// <summary>AddSignParam('a=1,b=2.1,c=3')。</summary>
    public static int AddSignParam(string aSignParamStr)
    {
        int result = 0;
        if (string.IsNullOrEmpty(aSignParamStr) || aSignParamStr.Length < 3)
            return 0;
        foreach (var part in aSignParamStr.Split(','))
        {
            int eq = part.IndexOf('=');
            if (eq <= 0) continue;
            string name = part.Substring(0, eq).Trim();
            string valStr = part.Substring(eq + 1).Trim();
            if (name == "" || !double.TryParse(valStr, NumberStyles.Float, CultureInfo.InvariantCulture, out double val))
                continue;
            SignParams[name] = val;
            result++;
        }
        return result;
    }

    public static string GetSignParam(int aindex = 0)
    {
        int i = 0;
        foreach (var kv in SignParams)
        {
            if (i == aindex) return kv.Key + "=" + FormatParam(kv.Value);
            i++;
        }
        return "";
    }

    public static string GetSignParam(string aParamName)
        => SignParams.TryGetValue(aParamName, out double v) ? aParamName + "=" + FormatParam(v) : "";

    private static string FormatParam(double v)
        => v == Math.Floor(v) && Math.Abs(v) < 1e15
            ? ((long)v).ToString()
            : v.ToString("R", CultureInfo.InvariantCulture);

    public static bool DelSignParam(string aParamName)
        => SignParams.Remove(aParamName);

    public static void InitSignParam() => SignParams.Clear();

    // ---------------- 解析器 ----------------

    private class ExpCalcException : Exception
    {
        public ExpCalcException(string msg) : base(msg) { }
    }

    private class Parser
    {
        private readonly string _s;
        private int _p;

        public Parser(string s)
        {
            // 去除空白（原说明：输入可用空格间隔）
            var sb = new StringBuilder(s.Length);
            foreach (char c in s)
                if (!char.IsWhiteSpace(c)) sb.Append(c);
            _s = sb.ToString();
        }

        public void ExpectEnd()
        {
            Skip();
            if (_p < _s.Length)
                throw new ExpCalcException("表达式存在多余字符: " + _s.Substring(_p));
        }

        private void Skip()
        {
            // 已在构造时去空白
        }

        private char Cur => _p < _s.Length ? _s[_p] : '\0';

        private bool Eat(char c)
        {
            if (Cur == c) { _p++; return true; }
            return false;
        }

        private void Expect(char c)
        {
            if (!Eat(c))
                throw new ExpCalcException($"缺少字符 '{c}' 位置 {_p + 1}");
        }

        // expr = term (('+'|'-') term)*
        public double ParseExpression()
        {
            double v = ParseTerm();
            while (Cur == '+' || Cur == '-')
            {
                char op = _s[_p++];
                double rhs = ParseTerm();
                v = op == '+' ? v + rhs : v - rhs;
            }
            return v;
        }

        // term = power (('*'|'/'|'mod') power)*
        private double ParseTerm()
        {
            double v = ParsePower();
            while (true)
            {
                if (Cur == '*' || Cur == '/')
                {
                    char op = _s[_p++];
                    double rhs = ParsePower();
                    if (op == '*') v *= rhs;
                    else
                    {
                        if (rhs == 0) throw new ExpCalcException("除数为 0");
                        v /= rhs;
                    }
                }
                else if (IsIdentAt("mod"))
                {
                    _p += 3;
                    double rhs = ParsePower();
                    double m = v % rhs;
                    v = m;
                }
                else break;
            }
            return v;
        }

        // power = unary ('^' power)?   幂右结合
        private double ParsePower()
        {
            double baseV = ParseUnary();
            if (Cur == '^')
            {
                _p++;
                double exp = ParsePower(); // 右结合
                return Math.Pow(baseV, exp);
            }
            return baseV;
        }

        // unary = ('+'|'-') unary | postfix
        private double ParseUnary()
        {
            if (Cur == '+') { _p++; return ParseUnary(); }
            if (Cur == '-') { _p++; return -ParseUnary(); }
            return ParsePostfix();
        }

        // postfix = primary ('!' | '%')*
        private double ParsePostfix()
        {
            double v = ParsePrimary();
            while (true)
            {
                if (Cur == '!')
                {
                    _p++;
                    v = Factorial(v);
                }
                else if (Cur == '%')
                {
                    _p++;
                    v /= 100.0;
                }
                else break;
            }
            return v;
        }

        private static double Factorial(double v)
        {
            double t = Math.Truncate(v);
            if (t != v || t < 1 || t > 150)
                throw new ExpCalcException("阶乘参数必须为 1..150 的整数");
            double r = 1;
            for (int i = 2; i <= (int)t; i++) r *= i;
            return r;
        }

        // primary = number | ident | ident(args) | '(' expr ')'
        private double ParsePrimary()
        {
            if (Eat('('))
            {
                double v = ParseExpression();
                Expect(')');
                return v;
            }

            if (char.IsAsciiDigit(Cur) || Cur == '.')
                return ParseNumber();

            if (char.IsAsciiLetter(Cur) || Cur == '_')
            {
                string ident = ParseIdent();

                // 常数
                if (ident.Equals("PI", StringComparison.OrdinalIgnoreCase) || ident.Equals("pi", StringComparison.OrdinalIgnoreCase))
                    return Math.PI;
                if (ident.Equals("e", StringComparison.Ordinal))
                    return Math.E;

                // 符号参数
                if (SignParams.TryGetValue(ident, out double signVal))
                    return signVal;

                // 函数
                if (Cur == '(')
                {
                    _p++;
                    var args = new List<double>();
                    if (Cur != ')')
                    {
                        args.Add(ParseExpression());
                        while (Cur == ',')
                        {
                            _p++;
                            args.Add(ParseExpression());
                        }
                    }
                    Expect(')');
                    return CallFunction(ident, args);
                }

                throw new ExpCalcException($"未定义的符号: {ident}");
            }

            throw new ExpCalcException($"位置 {_p + 1} 出现意外字符 '{Cur}'");
        }

        private double ParseNumber()
        {
            int start = _p;
            while (char.IsAsciiDigit(Cur) || Cur == '.') _p++;
            string num = _s.Substring(start, _p - start);
            if (!double.TryParse(num, NumberStyles.Float, CultureInfo.InvariantCulture, out double v))
                throw new ExpCalcException("数字格式错误: " + num);
            // 科学计数 1e3
            if (Cur == 'e' && _p + 1 < _s.Length && (char.IsAsciiDigit(_s[_p + 1]) || _s[_p + 1] == '+' || _s[_p + 1] == '-'))
            {
                // 注意：与常数 e 冲突时优先按数字科学计数（后随数字/符号+数字）
                int save = _p;
                _p++;
                if (_s[_p] == '+' || _s[_p] == '-') _p++;
                if (char.IsAsciiDigit(Cur))
                {
                    int es = _p;
                    while (char.IsAsciiDigit(Cur)) _p++;
                    string sci = num + "e" + _s.Substring(save + 1, _p - save - 1);
                    if (!double.TryParse(sci, NumberStyles.Float, CultureInfo.InvariantCulture, out v))
                        throw new ExpCalcException("数字格式错误: " + sci);
                }
                else
                {
                    _p = save; // 回退，e 为常数
                }
            }
            return v;
        }

        private string ParseIdent()
        {
            int start = _p;
            while (char.IsAsciiLetterOrDigit(Cur) || Cur == '_') _p++;
            return _s.Substring(start, _p - start);
        }

        private bool IsIdentAt(string word)
        {
            if (_p + word.Length > _s.Length) return false;
            if (string.CompareOrdinal(_s, _p, word, 0, word.Length) != 0) return false;
            // 后一个字符不能构成更长标识符
            int next = _p + word.Length;
            return next >= _s.Length || !(char.IsAsciiLetterOrDigit(_s[next]) || _s[next] == '_');
        }

        // ---------------- 函数分发（对应原 IsCalcFun/My_* 系列） ----------------

        private static double CallFunction(string fn, List<double> a)
        {
            string f = fn.ToLowerInvariant();
            double A(int i)
            {
                if (i >= a.Count) throw new ExpCalcException($"{fn} 缺少第 {i + 1} 个参数");
                return a[i];
            }
            switch (f)
            {
                // 统计
                case "max": Require(a, 1); return a.Max();
                case "min": Require(a, 1); return a.Min();
                case "sum": Require(a, 1); return a.Sum();
                case "avg": Require(a, 1); return a.Average();
                case "stddev": Require(a, 2); return StdDevSample(a);
                // 三角
                case "sin": Require(a, 1); return Math.Sin(A(0));
                case "cos": Require(a, 1); return Math.Cos(A(0));
                case "tan": Require(a, 1); return Math.Tan(A(0));
                case "arcsin": Require(a, 1); return Math.Asin(A(0));
                case "arccos": Require(a, 1); return Math.Acos(A(0));
                case "arctan": Require(a, 1); return Math.Atan(A(0));
                case "degrad": Require(a, 1); return A(0) * Math.PI / 180.0;
                case "raddeg": Require(a, 1); return A(0) * 180.0 / Math.PI;
                case "costh": Require(a, 3); return Costh(A(0), A(1), A(2)); // 余弦定理 cosC
                // 指数对数
                case "sqrt": Require(a, 1); return Math.Sqrt(A(0));
                case "power": Require(a, 2); return Math.Pow(A(0), A(1));
                case "abs": Require(a, 1); return Math.Abs(A(0));
                case "exp": Require(a, 1); return Math.Exp(A(0));
                case "log2": Require(a, 1); return Math.Log2(A(0));
                case "log10": Require(a, 1); return Math.Log10(A(0));
                case "logn": Require(a, 2); return Math.Log(A(0), A(1));
                case "ln": Require(a, 1); return Math.Log(A(0));
                // 数据处理
                case "int": Require(a, 1); return Math.Truncate(A(0));
                case "trunc": Require(a, 1); return Math.Truncate(A(0));
                case "frac": Require(a, 1); return A(0) - Math.Truncate(A(0));
                case "round": Require(a, 1); return Math.Round(A(0), MidpointRounding.AwayFromZero);
                case "roundto": Require(a, 2); return Math.Round(A(0), (int)A(1), MidpointRounding.AwayFromZero);
                case "mod": Require(a, 2); return A(0) % A(1);
                // 几何面积
                case "s_tria": Require(a, 3); return AreaTriangle(A(0), A(1), A(2));
                case "s_circ": Require(a, 1); return Math.PI * A(0) * A(0);
                case "s_elli": Require(a, 2); return Math.PI * A(0) * A(1);
                case "s_rect": Require(a, 2); return A(0) * A(1);
                case "s_poly": Require(a, 2); return AreaPolygon(A(0), (int)A(1)); // s_poly(a, n) 边长a 正 n 边形
                // 平面几何
                case "pdisplanes": Require(a, 4); return Hypot2(A(2) - A(0), A(3) - A(1));
                case "pdisspace": Require(a, 6); return Hypot3(A(3) - A(0), A(4) - A(1), A(5) - A(2));
                case "p_line": Require(a, 5); return PointToLine(A(0), A(1), A(2), A(3), A(4));
                case "p_planes": Require(a, 7); return PointToPlane(A(0), A(1), A(2), A(3), A(4), A(5), A(6));
                // 数列
                case "sn": Require(a, 3); return SumSubSeri(A(0), A(1), A(2));
                case "sqn": Require(a, 3); return SumRatioSeri(A(0), A(1), A(2));
                // 个税（简化按 2019 综合税率表，与原实现一致）
                case "intax": Require(a, 1); return IncomeTax(A(0));
                case "arcintax": Require(a, 1); return ArcIncomeTax(A(0));
                default:
                    throw new ExpCalcException("未知函数: " + fn);
            }
        }

        private static void Require(List<double> a, int n)
        {
            if (a.Count < n) throw new ExpCalcException("函数参数不足");
        }

        private static double StdDevSample(List<double> a)
        {
            // 对应 Delphi Math.StdDev：总体标准差（除 N）
            double mean = a.Average();
            double sumSq = a.Sum(v => (v - mean) * (v - mean));
            return Math.Sqrt(sumSq / a.Count);
        }

        private static double Costh(double a, double b, double c)
            => (a * a + b * b - c * c) / (2 * a * b);

        private static double AreaTriangle(double a, double b, double c)
        {
            double s = (a + b + c) / 2;
            return Math.Sqrt(s * (s - a) * (s - b) * (s - c));
        }

        private static double AreaPolygon(double side, int n)
            => n * side * side / (4 * Math.Tan(Math.PI / n));

        private static double Hypot2(double dx, double dy) => Math.Sqrt(dx * dx + dy * dy);
        private static double Hypot3(double dx, double dy, double dz) => Math.Sqrt(dx * dx + dy * dy + dz * dz);

        private static double PointToLine(double x0, double y0, double a, double b, double c)
            => Math.Abs(a * x0 + b * y0 + c) / Math.Sqrt(a * a + b * b);

        private static double PointToPlane(double x0, double y0, double z0, double a, double b, double c, double d)
            => Math.Abs(a * x0 + b * y0 + c * z0 + d) / Math.Sqrt(a * a + b * b + c * c);

        private static double SumSubSeri(double a1, double d, double n) // 等差前 n 项和
            => n * (2 * a1 + (n - 1) * d) / 2;

        private static double SumRatioSeri(double a1, double q, double n) // 等比前 n 项和
        {
            if (Math.Abs(q - 1) < 1e-12) return a1 * n;
            return a1 * (1 - Math.Pow(q, n)) / (1 - q);
        }

        private static double IncomeTax(double x)
        {
            // 2019 综合所得月度税率表（对应原实现）
            double[,] brackets = {
                { 0, 0.03 }, { 3000, 0.10 }, { 12000, 0.20 },
                { 25000, 0.25 }, { 35000, 0.30 }, { 55000, 0.35 }, { 80000, 0.45 }
            };
            double tax = 0;
            for (int i = brackets.GetLength(0) - 1; i >= 0; i--)
            {
                if (x > brackets[i, 0])
                {
                    tax = (x - brackets[i, 0]) * brackets[i, 1];
                    for (int j = i - 1; j >= 0; j--)
                        tax += (brackets[j + 1, 0] - brackets[j, 0]) * brackets[j, 1];
                    break;
                }
            }
            return tax;
        }

        private static double ArcIncomeTax(double t)
        {
            // 反算：二分求 x 使 IncomeTax(x) = t
            double lo = 0, hi = 1e7;
            for (int i = 0; i < 200; i++)
            {
                double mid = (lo + hi) / 2;
                if (IncomeTax(mid) < t) lo = mid; else hi = mid;
            }
            return (lo + hi) / 2;
        }
    }
}
