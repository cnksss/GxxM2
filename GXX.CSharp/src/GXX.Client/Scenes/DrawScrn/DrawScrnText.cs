// ============================================================================================
// 车道 `p10-client-scrn`：DrawScrn.pas 依赖的**文字 token 族**与两个 `GetTextListEx` 重载。
//
// 归属说明（台账 §12.8：「只有在正式归属文件缺失时才允许车道造接缝」）：
//   * `TTokenType` / `TStringToken` / `PStringToken` / `TStringLineEx`        ← DxMemo.pas:238-266
//   * `GetStrinLineExText`                                                   ← DxMemo.pas:2202-2218
//   * `GetTextListEx(HGEFont, …; TextList:TList; …; MaxWidth; …)`            ← DxMemo.pas:2223-2389
//   * `GetTextListEx(Text, …; TokenLine:TStringLineEx; …)`                   ← DxMemo.pas:2394-2524
//   * `GetTextListEx2`（/SCOLOR= 变体）                                      ← DrawScrn.pas:3540-3643
// 这五组在托管工程里**尚无任何声明**（`DxComponent/DxMemo.cs:16` 的头部注释把
// `TTokenType/PStringToken/TStringToken/TStringLineEx` 明确列为"待移植"，`DxMemo.Text.cs` 尚不存在）。
// 【归属申请】DxMemo.pas 全量移植时，本文件的 `TTokenType/TStringToken/TStringLineEx/
// GetStrinLineExText/GetTextListEx` 应由 `GXX.Client.DxComponent`（`DxMemo.Text.cs`）接管并删除本份；
// 只有 `GetTextListEx2` 属于 DrawScrn.pas、永久留在本车道。
// ============================================================================================

using System;
using System.Collections.Generic;
using GXX.Client.GUI.Mir;
using GXX.Client.GUI.Share;
using GXX.Core.Rtl;
using GXX.Core.Util;
using TList = GXX.Client.GUI.Share.TList;

namespace GXX.Client.Scenes;

/// <summary>DxMemo.pas:238 TTokenType = (tt_Text, tt_Item)。</summary>
public enum TTokenType
{
    tt_Text = 0,
    tt_Item = 1,
}

/// <summary>DxMemo.pas:241-248 PStringToken = ^TStringToken / TStringToken。</summary>
public sealed class TStringToken
{
    public TColor FColor;
    public TColor BColor;
    public int Flag;
    public string Text = "";
    public TTokenType TokenType;
}

/// <summary>DxMemo.pas:250-265 TStringLineEx（一行分为多个字符 chongchong 2013-08-26）。</summary>
public sealed class TStringLineEx
{
    private readonly List<TStringToken> _tokens = new List<TStringToken>();

    /// <summary>
    /// 原文 DxMemo.pas:253 `FLineBackColor:TColor`（private，但同单元的 `GetTextListEx.NewLine`
    /// 直接对其赋值 —— Delphi 同单元可访问 private）。托管侧同程序集可访问，故此处直接公开。
    /// </summary>
    public TColor LineBackColor;

    /// <summary>原文 DxMemo.pas:254-255 GetCount。</summary>
    public int Count => _tokens.Count;

    /// <summary>原文 DxMemo.pas:260 `property Tokens[Index]:PStringToken read GetTokens; default;`。</summary>
    public TStringToken this[int index] => _tokens[index];

    /// <summary>原文 DxMemo.pas:261 function Add(FColor, BColor:TColor; Flag:Integer; Text:string; TokenType:TTokenType):PStringToken。</summary>
    public TStringToken Add(TColor fcolor, TColor bcolor, int flag, string text, TTokenType tokenType)
    {
        var token = new TStringToken
        {
            FColor = fcolor,
            BColor = bcolor,
            Flag = flag,
            Text = text ?? "",
            TokenType = tokenType,
        };
        _tokens.Add(token);
        return token;
    }

    /// <summary>原文 DxMemo.pas:262 procedure Delete(Index:Integer)。</summary>
    public void Delete(int index) => _tokens.RemoveAt(index);

    /// <summary>原文 DxMemo.pas:265 procedure Clear（只清 token，不动 FLineBackColor）。</summary>
    public void Clear() => _tokens.Clear();
}

/// <summary>DrawScrn.pas / DxMemo.pas 用到的 token 自由函数。</summary>
public static class DrawScrnText
{
    /// <summary>
    /// DxMemo.pas:2202-2218 / 659 `GetStrinLineExText(Line:TStringLineEx; IncludeItem:Boolean = True):string`。
    /// <para>IncludeItem=True 时拼接全部 token；False 时跳过 `TokenType = tt_Item` 的 token。</para>
    /// </summary>
    public static string GetStrinLineExText(TStringLineEx line, bool includeItem = true)
    {
        string result = "";
        if (includeItem)
        {
            for (int i = 0; i < line.Count; i++)
                result += line[i].Text;
        }
        else
        {
            for (int i = 0; i < line.Count; i++)
                if (line[i].TokenType != TTokenType.tt_Item)
                    result += line[i].Text;
        }
        return result;
    }

    /// <summary>
    /// DxMemo.pas:2223-2389
    /// `GetTextListEx(HGEFont:THGEFont; const Text:WideString; const FColor, BColor:TColor;
    ///   TextList:TList; LineBackColor:TColor; MaxWidth:Integer;
    ///   ItemFColor, ItemBColor:TColor)`（多行 + 按 MaxWidth 折行版）。
    /// <para>DrawScrn.pas:3203 的调用只传前 7 个实参 ⇒ ItemFColor/ItemBColor 取原文接口默认值
    /// （DxMemo.pas:660 `ItemFColor:TColor = clBlue; ItemBColor:TColor = clWhite`）。</para>
    /// </summary>
    public static void GetTextListEx(
        THGEFont hgeFont, string text, TColor fcolor, TColor bcolor,
        TList textList, TColor lineBackColor, int maxWidth,
        TColor? itemFColor = null, TColor? itemBColor = null)
    {
        var b = new MultiLineBuilder(hgeFont, text ?? "", fcolor, bcolor, textList, lineBackColor, maxWidth,
            itemFColor ?? new TColor(0x00FF0000), itemBColor ?? TColor.clWhite);
        b.Run();
    }

    /// <summary>
    /// DxMemo.pas:2394-2524
    /// `GetTextListEx(const Text:WideString; const FColor, BColor:TColor; TokenLine:TStringLineEx;
    ///   ItemFColor:TColor = clBlue; ItemBColor:TColor = clWhite)`（单行版）。
    /// </summary>
    public static void GetTextListEx(
        string text, TColor fcolor, TColor bcolor, TStringLineEx tokenLine,
        TColor? itemFColor = null, TColor? itemBColor = null)
    {
        var b = new SingleLineBuilder(text ?? "", fcolor, bcolor, tokenLine,
            itemFColor ?? new TColor(0x00FF0000), itemBColor ?? TColor.clWhite);
        b.Run();
    }

    /// <summary>
    /// DrawScrn.pas:3540-3643 `GetTextListEx2(const Text:WideString; const FColor, BColor:TColor;
    ///   TokenLine:TStringLineEx)`（`{aaa/SCOLOR=249}` 变体，仅单行）。
    /// </summary>
    public static void GetTextListEx2(string text, TColor fcolor, TColor bcolor, TStringLineEx tokenLine)
    {
        // 原文 3541-3546 的局部变量（C# 的本地函数只能捕获**声明在它之前**的局部变量，
        // 故此处把原文声明顺序前移；赋值时机与原文一致，语义不变）。
        int index = 0;
        int index2, customTextLen;
        string sLine = "";
        char wChar = '\0';
        var outToken = new TStringToken();
        string sRemberDef = "", sRemberCustom = "";

        // ---- 原文 3548-3580 的嵌套 function ProcessCustomColor(var StringToken; var Len):Boolean ----
        bool ProcessCustomColor(TStringToken stringToken, ref int len)
        {
            // PWChar := PWideChar(Text); Inc(PWChar, Index); FoundIndex := Pos('}', PWChar)
            int foundIndex = DelphiRTL.Pos("}", index + 1 <= text.Length ? text.Substring(index) : "");
            if (foundIndex == 0)
                return false;

            len = foundIndex + 1;

            // 取{}中间一部分 aabbcc|100:200:0
            string foundText = DelphiRTL.Copy(text, index + 2, foundIndex - 1);
            string foundText2 = DelphiRTL.UpperCase(foundText);
            foundIndex = DelphiRTL.Pos("/SCOLOR=", foundText2);
            if (foundIndex == 0)
                return false;

            stringToken.TokenType = TTokenType.tt_Item;
            stringToken.Text = DelphiRTL.Copy(foundText, 1, foundIndex - 1);
            foundText = DelphiRTL.Copy(foundText, foundIndex + "/SCOLOR=".Length, int.MaxValue);
            stringToken.Flag = 0;
            stringToken.FColor = new TColor(DelphiRTL.StrToIntDef(DelphiRTL.Trim(foundText), fcolor.Value));
            stringToken.BColor = bcolor;
            return true;
        }

        // ---- 原文 3582-3592 的嵌套 function DoProcessText:Boolean ----
        bool DoProcessText()
        {
            sLine += wChar;
            index++;
            if (index > text.Length)
            {
                tokenLine.Add(fcolor, bcolor, 0, DelphiRTL.Copy(sLine, sRemberCustom.Length + 1, int.MaxValue), TTokenType.tt_Text);
                sRemberCustom = "";
                return true;
            }
            return false;
        }

        tokenLine.Clear();

        index = 1;
        sLine = "";

        // aa{aabbcc|249:200:0}ee
        while (true)
        {
            if (index > text.Length) break;
            wChar = text[index - 1];
            if (wChar != '{')
            {
                if (DoProcessText()) break;
            }
            else
            {
                customTextLen = 0;
                if (!ProcessCustomColor(outToken, ref customTextLen))
                {
                    // 如果不符合定义颜色的格式，则照常处理
                    if (DoProcessText()) break;
                }
                else
                {
                    // 如果符合定义颜色的格式，则将自定义格式中的内容处理完
                    sRemberDef = sLine;
                    if (sLine.Length > 0)
                    {
                        // {自定义文字颜色|249:0}aa{自定义文字颜色|249:0}
                        if (sRemberCustom.Length > 0)
                        {
                            if (sLine.Length > sRemberCustom.Length)
                                tokenLine.Add(fcolor, bcolor, 0, DelphiRTL.Copy(sLine, sRemberCustom.Length + 1, int.MaxValue), TTokenType.tt_Text);
                        }
                        else
                            tokenLine.Add(fcolor, bcolor, 0, sLine, TTokenType.tt_Text);
                    }

                    index2 = 1;
                    while (true)
                    {
                        if (index2 > outToken.Text.Length) break;
                        wChar = outToken.Text[index2 - 1];

                        sLine += wChar;
                        index2++;
                        if (index2 > outToken.Text.Length)
                        {
                            tokenLine.Add(outToken.FColor, outToken.BColor, outToken.Flag,
                                DelphiRTL.Copy(sLine, sRemberDef.Length + 1, int.MaxValue), outToken.TokenType);
                            sRemberDef = "";
                            sRemberCustom = sLine;
                            break;
                        }
                    }

                    index += customTextLen;
                }
            }
        }
    }

    // ==========================================================================================
    // 多行版构建器（DxMemo.pas:2223-2389）
    // ==========================================================================================
    private sealed class MultiLineBuilder
    {
        private readonly THGEFont _hgeFont;
        private readonly string _text;
        private readonly TColor _fcolor;
        private readonly TColor _bcolor;
        private readonly TList _textList;
        private readonly TColor _lineBackColor;
        private readonly int _maxWidth;
        private readonly TColor _itemFColor;
        private readonly TColor _itemBColor;

        private int _index;
        private string _sLine = "";
        private string _sTemp = "";
        private char _wChar;
        private readonly TStringToken _outToken = new TStringToken();
        private int _customTextLen;
        private string _sRemberDef = "";
        private string _sRemberCustom = "";

        public MultiLineBuilder(THGEFont hgeFont, string text, TColor fcolor, TColor bcolor,
            TList textList, TColor lineBackColor, int maxWidth, TColor itemFColor, TColor itemBColor)
        {
            _hgeFont = hgeFont;
            _text = text;
            _fcolor = fcolor;
            _bcolor = bcolor;
            _textList = textList;
            _lineBackColor = lineBackColor;
            _maxWidth = maxWidth;
            _itemFColor = itemFColor;
            _itemBColor = itemBColor;
        }

        /// <summary>原文 2231-2236 的嵌套 function NewLine:TStringLineEx（新建并挂到 TextList）。</summary>
        private TStringLineEx NewLine()
        {
            var result = new TStringLineEx { LineBackColor = _lineBackColor };
            _textList.Add(result);
            return result;
        }

        /// <summary>原文 2238-2296 的嵌套 function ProcessCustomColor(var StringToken; var Len):Boolean。</summary>
        private bool ProcessCustomColor(TStringToken stringToken, ref int len)
        {
            string rest = _index + 1 <= _text.Length ? _text.Substring(_index) : "";
            int foundIndex = DelphiRTL.Pos("}", rest);
            if (foundIndex == 0)
                return false;

            len = foundIndex + 1;

            // 取{}中间一部分 aabbcc|100:200:0
            string foundText = DelphiRTL.Copy(_text, _index + 1, foundIndex - 1);
            foundIndex = DelphiRTL.Pos("|", foundText);
            if (foundIndex == 0)
            {
                foundIndex = DelphiRTL.Pos("/", foundText);
                if (foundIndex == 0)
                    return false;

                stringToken.TokenType = TTokenType.tt_Item;
                stringToken.Text = DelphiRTL.Copy(foundText, 1, foundIndex - 1);
                foundText = DelphiRTL.Copy(foundText, foundIndex + 1, int.MaxValue);
                stringToken.Flag = DelphiRTL.StrToIntDef(foundText, 0);
                stringToken.FColor = _itemFColor;
                stringToken.BColor = _itemBColor;
                return true;
            }
            else
            {
                stringToken.TokenType = TTokenType.tt_Text;
                stringToken.Text = DelphiRTL.Copy(foundText, 1, foundIndex - 1);   // aabbcc
                foundText = DelphiRTL.Copy(foundText, foundIndex + 1, int.MaxValue); // 100:200:0
                if (foundText.Length == 0)
                    return false;

                string s1 = "", s2 = "", s3 = "";
                foundText = HUtil32.GetValidStr3(foundText, ref s1, new[] { ' ', '\t', ':' });
                foundText = HUtil32.GetValidStr3(foundText, ref s2, new[] { ' ', '\t', ':' });
                foundText = HUtil32.GetValidStr3(foundText, ref s3, new[] { ' ', '\t', ':' });

                stringToken.FColor = DrawScrnEnv.ColorIndexToTColor((byte)DelphiRTL.StrToIntDef(s1, 255));
                stringToken.BColor = DrawScrnEnv.ColorIndexToTColor((byte)DelphiRTL.StrToIntDef(s2, 255));
                stringToken.Flag = DelphiRTL.StrToIntDef(s3, 0);
                return true;
            }
        }

        /// <summary>原文 2298-2322 的嵌套 function DoProcessText:Boolean。</summary>
        private bool DoProcessText()
        {
            _sTemp = _sLine + _wChar;
            if ((_maxWidth > 0) && (DrawScrnEnv.TextWidth(_hgeFont, _sTemp) > _maxWidth))
            {
                if (_sRemberCustom.Length > 0)
                    ((TStringLineEx)_textList[_textList.Count - 1]).Add(_fcolor, _bcolor, 0,
                        DelphiRTL.Copy(_sLine, _sRemberCustom.Length + 1, int.MaxValue), TTokenType.tt_Text);
                else
                    NewLine().Add(_fcolor, _bcolor, 0, _sLine, TTokenType.tt_Text);
                _sLine = "";
                _sRemberCustom = "";
            }
            else
            {
                _sLine = _sTemp;
                _index++;
                if (_index > _text.Length)
                {
                    if (_sRemberCustom.Length > 0)
                        ((TStringLineEx)_textList[_textList.Count - 1]).Add(_fcolor, _bcolor, 0,
                            DelphiRTL.Copy(_sLine, _sRemberCustom.Length + 1, int.MaxValue), TTokenType.tt_Text);
                    else
                        NewLine().Add(_fcolor, _bcolor, 0, _sLine, TTokenType.tt_Text);
                    _sRemberCustom = "";
                    return true;
                }
            }
            return false;
        }

        /// <summary>原文 2323-2388 主体。</summary>
        public void Run()
        {
            _textList.Clear();

            _index = 1;
            _sLine = "";
            _sTemp = "";

            while (true)
            {
                if (_index > _text.Length) break;
                _wChar = _text[_index - 1];
                if (_wChar != '{')
                {
                    if (DoProcessText()) break;
                }
                else
                {
                    _customTextLen = 0;
                    if (!ProcessCustomColor(_outToken, ref _customTextLen))
                    {
                        if (DoProcessText()) break;
                    }
                    else
                    {
                        _sRemberDef = _sLine;
                        if (_sLine.Length > 0)
                        {
                            if (_sRemberCustom.Length > 0)
                            {
                                if (_sLine.Length > _sRemberCustom.Length)
                                    ((TStringLineEx)_textList[_textList.Count - 1]).Add(_fcolor, _bcolor, 0,
                                        DelphiRTL.Copy(_sLine, _sRemberCustom.Length + 1, int.MaxValue), TTokenType.tt_Text);
                            }
                            else
                                NewLine().Add(_fcolor, _bcolor, 0, _sLine, TTokenType.tt_Text);
                        }

                        int index2 = 1;
                        while (true)
                        {
                            if (index2 > _outToken.Text.Length) break;
                            _wChar = _outToken.Text[index2 - 1];

                            _sTemp = _sLine + _wChar;
                            if ((_maxWidth > 0) && (DrawScrnEnv.TextWidth(_hgeFont, _sTemp) > _maxWidth))
                            {
                                if (_sRemberDef.Length > 0)
                                    ((TStringLineEx)_textList[_textList.Count - 1]).Add(_outToken.FColor, _outToken.BColor,
                                        _outToken.Flag, DelphiRTL.Copy(_sLine, _sRemberDef.Length + 1, int.MaxValue), _outToken.TokenType);
                                else
                                    NewLine().Add(_outToken.FColor, _outToken.BColor, _outToken.Flag, _sLine, _outToken.TokenType);
                                _sRemberDef = "";
                                _sLine = "";
                            }
                            else
                            {
                                _sLine = _sTemp;
                                index2++;
                                if (index2 > _outToken.Text.Length)
                                {
                                    if (_sRemberDef.Length > 0)
                                        ((TStringLineEx)_textList[_textList.Count - 1]).Add(_outToken.FColor, _outToken.BColor,
                                            _outToken.Flag, DelphiRTL.Copy(_sLine, _sRemberDef.Length + 1, int.MaxValue), _outToken.TokenType);
                                    else
                                        NewLine().Add(_outToken.FColor, _outToken.BColor, _outToken.Flag, _sLine, _outToken.TokenType);
                                    _sRemberDef = "";
                                    _sRemberCustom = _sLine;
                                    break;
                                }
                            }
                        }

                        _index += _customTextLen;
                    }
                }
            }
        }
    }

    // ==========================================================================================
    // 单行版构建器（DxMemo.pas:2394-2524）
    // ==========================================================================================
    private sealed class SingleLineBuilder
    {
        private readonly string _text;
        private readonly TColor _fcolor;
        private readonly TColor _bcolor;
        private readonly TStringLineEx _tokenLine;
        private readonly TColor _itemFColor;
        private readonly TColor _itemBColor;

        private int _index;
        private string _sLine = "";
        private char _wChar;
        private readonly TStringToken _outToken = new TStringToken();
        private int _customTextLen;
        private string _sRemberDef = "";
        private string _sRemberCustom = "";

        public SingleLineBuilder(string text, TColor fcolor, TColor bcolor, TStringLineEx tokenLine,
            TColor itemFColor, TColor itemBColor)
        {
            _text = text;
            _fcolor = fcolor;
            _bcolor = bcolor;
            _tokenLine = tokenLine;
            _itemFColor = itemFColor;
            _itemBColor = itemBColor;
        }

        /// <summary>原文 2402-2459 的嵌套 function ProcessCustomColor (var StringToken; var Len):Boolean。</summary>
        private bool ProcessCustomColor(TStringToken stringToken, ref int len)
        {
            string rest = _index + 1 <= _text.Length ? _text.Substring(_index) : "";
            int foundIndex = DelphiRTL.Pos("}", rest);
            if (foundIndex == 0)
                return false;

            len = foundIndex + 1;

            string foundText = DelphiRTL.Copy(_text, _index + 1, foundIndex - 1);
            foundIndex = DelphiRTL.Pos("|", foundText);
            if (foundIndex == 0)
            {
                foundIndex = DelphiRTL.Pos("/", foundText);
                if (foundIndex == 0)
                    return false;

                stringToken.TokenType = TTokenType.tt_Item;
                stringToken.Text = DelphiRTL.Copy(foundText, 1, foundIndex - 1);
                foundText = DelphiRTL.Copy(foundText, foundIndex + 1, int.MaxValue);
                stringToken.Flag = DelphiRTL.StrToIntDef(foundText, 0);
                stringToken.FColor = _itemFColor;
                stringToken.BColor = _itemBColor;
                return true;
            }
            else
            {
                stringToken.TokenType = TTokenType.tt_Text;
                stringToken.Text = DelphiRTL.Copy(foundText, 1, foundIndex - 1);
                foundText = DelphiRTL.Copy(foundText, foundIndex + 1, int.MaxValue);
                if (foundText.Length == 0)
                    return false;

                string s1 = "", s2 = "", s3 = "";
                foundText = HUtil32.GetValidStr3(foundText, ref s1, new[] { ' ', '\t', ':' });
                foundText = HUtil32.GetValidStr3(foundText, ref s2, new[] { ' ', '\t', ':' });
                foundText = HUtil32.GetValidStr3(foundText, ref s3, new[] { ' ', '\t', ':' });

                stringToken.FColor = DrawScrnEnv.ColorIndexToTColor((byte)DelphiRTL.StrToIntDef(s1, 255));
                stringToken.BColor = DrawScrnEnv.ColorIndexToTColor((byte)DelphiRTL.StrToIntDef(s2, 255));
                stringToken.Flag = DelphiRTL.StrToIntDef(s3, 0);
                return true;
            }
        }

        /// <summary>原文 2461-2471 的嵌套 function DoProcessText:Boolean。</summary>
        private bool DoProcessText()
        {
            _sLine += _wChar;
            _index++;
            if (_index > _text.Length)
            {
                _tokenLine.Add(_fcolor, _bcolor, 0, DelphiRTL.Copy(_sLine, _sRemberCustom.Length + 1, int.MaxValue), TTokenType.tt_Text);
                _sRemberCustom = "";
                return true;
            }
            return false;
        }

        /// <summary>原文 2472-2523 主体。</summary>
        public void Run()
        {
            _tokenLine.Clear();

            _index = 1;
            _sLine = "";

            while (true)
            {
                if (_index > _text.Length) break;
                _wChar = _text[_index - 1];
                if (_wChar != '{')
                {
                    if (DoProcessText()) break;
                }
                else
                {
                    _customTextLen = 0;
                    if (!ProcessCustomColor(_outToken, ref _customTextLen))
                    {
                        if (DoProcessText()) break;
                    }
                    else
                    {
                        _sRemberDef = _sLine;
                        if (_sLine.Length > 0)
                        {
                            if (_sRemberCustom.Length > 0)
                            {
                                if (_sLine.Length > _sRemberCustom.Length)
                                    _tokenLine.Add(_fcolor, _bcolor, 0,
                                        DelphiRTL.Copy(_sLine, _sRemberCustom.Length + 1, int.MaxValue), TTokenType.tt_Text);
                            }
                            else
                                _tokenLine.Add(_fcolor, _bcolor, 0, _sLine, TTokenType.tt_Text);
                        }

                        int index2 = 1;
                        while (true)
                        {
                            if (index2 > _outToken.Text.Length) break;
                            _wChar = _outToken.Text[index2 - 1];

                            _sLine += _wChar;
                            index2++;
                            if (index2 > _outToken.Text.Length)
                            {
                                _tokenLine.Add(_outToken.FColor, _outToken.BColor, _outToken.Flag,
                                    DelphiRTL.Copy(_sLine, _sRemberDef.Length + 1, int.MaxValue), _outToken.TokenType);
                                _sRemberDef = "";
                                _sRemberCustom = _sLine;
                                break;
                            }
                        }

                        _index += _customTextLen;
                    }
                }
            }
        }
    }
}
