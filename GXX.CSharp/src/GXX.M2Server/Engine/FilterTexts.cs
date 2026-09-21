using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using GXX.Core.Util;

namespace GXX.M2Server.Engine;

/// <summary>FilterTexts.pas TFilterMsg record 镜像。</summary>
public sealed class TFilterMsg
{
    public string Msg = "";
    public string Replace = "";
}

/// <summary>
/// FilterTexts.pas TFilterTexts 1:1（批次J56）：TrimAll（删除 #32..#255 之外字符）、
/// Find/Add/Delete 全部 CompareText（不区分大小写）、Add 重名拒绝（CanFilterMsg 恒真）、
/// Filter 替换核心（'$' 特例：包含 → 全滤且 len=1 置空串否则删 '$'；SameText 全等 → Replace（空则清空）；
/// AnsiContainsText 包含 → 大小写不敏感替换；结果空串即停）、LoadFromFile/SaveToFile（FilterMsgList.txt
/// TAB 分隔、';' 注释跳过）。g_FilterTexts 单元全局由窗体层持有。
/// </summary>
public class TFilterTexts
{
    private readonly List<TFilterMsg> _list = new();
    private int _recordCount;

    public int Count => _list.Count;

    public int RecordCount => _recordCount;

    public TFilterMsg GetItems(int index) => _list[index];

    /// <summary>
    /// TrimAll：Trim 后删除控制字符。Delphi TextChars=[#32..#255] 基于 ANSI 字节流
    /// （GBK 汉字字节 128..255 全部保留），Unicode 下的 1:1 等价 = 仅删除 &lt;#32。
    /// </summary>
    public static string TrimAll(string text)
    {
        text = text.Trim();
        var sb = new StringBuilder(text.Length);
        foreach (var ch in text)
        {
            if (ch >= (char)32)
                sb.Append(ch);
        }
        return sb.ToString();
    }

    public bool Find(string sMsg)
    {
        sMsg = TrimAll(sMsg);
        foreach (var filterMsg in _list)
        {
            if (string.Compare(sMsg, filterMsg.Msg, StringComparison.OrdinalIgnoreCase) == 0)
                return true;
        }
        return false;
    }

    /// <summary>Filter（119-176）：替换命中返回 true 且 sNewMsg 为替换产物。</summary>
    public bool Filter(string sMsg, out string sNewMsg)
    {
        bool result = false;
        sNewMsg = TrimAll(sMsg);
        if (!M2ShareFuncs.CanFilterMsg(sNewMsg))
            return result;

        // '$' 特例
        if (string.Compare(sNewMsg, "$", StringComparison.OrdinalIgnoreCase) == 0 ||
            sNewMsg.IndexOf('$', StringComparison.OrdinalIgnoreCase) >= 0)
        {
            result = true;
            sNewMsg = sNewMsg.Length == 1 ? "" : sNewMsg.Replace("$", "");
            return result;
        }

        foreach (var filterMsg in _list)
        {
            bool isSameText = string.Equals(sNewMsg, filterMsg.Msg, StringComparison.OrdinalIgnoreCase);
            if (isSameText || sNewMsg.IndexOf(filterMsg.Msg, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                result = true;
                if (filterMsg.Replace.Length == 0)
                {
                    sNewMsg = "";
                }
                else
                {
                    sNewMsg = isSameText
                        ? filterMsg.Replace
                        : AnsiReplaceText(sNewMsg, filterMsg.Msg, filterMsg.Replace);
                }
                if (sNewMsg.Length == 0)
                    break;
            }
        }
        return result;
    }

    public bool Add(string sMsg, string sReplaceMsg)
    {
        sMsg = TrimAll(sMsg);
        foreach (var filterMsg in _list)
        {
            if (string.Compare(sMsg, filterMsg.Msg, StringComparison.OrdinalIgnoreCase) == 0)
                return false;
        }
        if (!M2ShareFuncs.CanFilterMsg(sMsg))
            return false;
        _list.Add(new TFilterMsg { Msg = sMsg, Replace = sReplaceMsg });
        _recordCount++;
        return true;
    }

    public bool Delete(string sMsg)
    {
        for (int i = 0; i < _list.Count; i++)
        {
            if (string.Compare(sMsg, _list[i].Msg, StringComparison.OrdinalIgnoreCase) == 0)
            {
                _recordCount--;
                _list.RemoveAt(i);
                return true;
            }
        }
        return false;
    }

    public void LoadFromFile()
    {
        string sFileName = Path.Combine(M2Config.sEnvirDir, "FilterMsgList.txt");
        if (!File.Exists(sFileName))
            return;
        // GBK 一律经 GXX.Core.EncodingInit.GBK 获取：其内部先 Ensure() 注册 CodePagesEncodingProvider，消除加载顺序依赖（CP936 实例等价）。
        foreach (var line in File.ReadLines(sFileName, GXX.Core.EncodingInit.GBK))
        {
            string sLineText = line;
            if (sLineText.Length > 0 && sLineText[0] != ';')
            {
                string sMsg = "", sReplace = "";
                sLineText = TSndaShopList.GetValidStr3(sLineText, ref sMsg, '\t');
                sLineText = TSndaShopList.GetValidStr3(sLineText, ref sReplace, '\t');
                if (M2ShareFuncs.CanFilterMsg(sMsg))
                {
                    _list.Add(new TFilterMsg { Msg = sMsg, Replace = sReplace });
                    _recordCount++;
                }
            }
        }
    }

    public void SaveToFile()
    {
        var saveList = new List<string>();
        foreach (var filterMsg in _list)
            saveList.Add(filterMsg.Msg + "\t" + filterMsg.Replace);
        string sFileName = Path.Combine(M2Config.sEnvirDir, "FilterMsgList.txt");
        try
        {
            File.WriteAllLines(sFileName, saveList, GXX.Core.EncodingInit.GBK);
        }
        catch
        {
            // Delphi try-except 吞异常
        }
    }

    /// <summary>AnsiReplaceText：大小写不敏感的全部替换。</summary>
    internal static string AnsiReplaceText(string text, string from, string to)
    {
        if (from.Length == 0)
            return text;
        var sb = new StringBuilder();
        int pos = 0;
        var cmp = CultureInfo.InvariantCulture.CompareInfo;
        while (pos <= text.Length - from.Length)
        {
            int idx = cmp.IndexOf(text, from, pos, CompareOptions.IgnoreCase);
            if (idx < 0)
                break;
            sb.Append(text, pos, idx - pos);
            sb.Append(to);
            pos = idx + from.Length;
        }
        sb.Append(text, pos, text.Length - pos);
        return sb.ToString();
    }
}
