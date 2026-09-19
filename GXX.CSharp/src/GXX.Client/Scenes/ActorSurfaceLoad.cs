using System;
using System.Collections.Generic;

namespace GXX.Client.Scenes;

/// <summary>
/// HUtil32.GetValidStr3_Ex（1456-1539）1:1 —— 以单字符分隔符切分，
/// 返回剩余串，Dest 取本段。语义细节：
///   - Dest 先被赋为整个输入（Len=0 时直接 Exit，Dest 保持原串）
///   - 丢掉开头**连续**的分隔符（StartIndex 反复后移）
///   - 遇到第一个「非开头」的分隔符时，Dest = [StartIndex, I)，Result = (I, Len]
///   - 全程无分隔符命中且 StartIndex &gt; 1（即只有开头是分隔符）→ Dest 取去掉开头分隔符后的整段，Result = ''
/// </summary>
public static class StrUtilEx
{
    public static string GetValidStr3Ex(string str, char divider, out string dest)
    {
        dest = str;
        string result = "";
        int len = str.Length;
        if (len == 0)
            return result;

        bool isStart = false;
        int startIndex = 1;                             // Delphi 1-based

        for (int i = 1; i <= len; i++)
        {
            char c = str[i - 1];
            if (c == divider)
            {
                if (isStart)
                {
                    dest = str.Substring(startIndex - 1, i - startIndex);
                    result = str.Substring(i, len - i);
                    return result;
                }

                startIndex = i + 1;
            }
            else if (!isStart)
            {
                isStart = true;
                startIndex = i;
            }
        }

        if (startIndex > 1)
            dest = str.Substring(startIndex - 1, len - startIndex + 1);

        return result;
    }
}

/// <summary>
/// 多行名字纹理的排版产物（DxCanvas.GetTextTexture 1151-1246 的 headless 镜像）。
/// 纹理宽度 = 各行最大宽；高度 = 行高 × 行数；每行按 DT_CENTER 居中绘制。
/// </summary>
public sealed class TextTextureLayout
{
    public readonly List<string> Lines = new();

    /// <summary>纹理宽（各行绘制宽度最大值）。</summary>
    public int Width;

    /// <summary>纹理高（行高 × 行数）。</summary>
    public int Height;

    /// <summary>单行行高（原文以 '|' 的 GetTextExtentPoint32 高度为基准）。</summary>
    public int LineHeight;

    /// <summary>每行居中后的绘制 x 起点（DT_CENTER 于 Rect(0, I*nLH, nW, ...) 内）。</summary>
    public readonly List<int> CenteredX = new();

    /// <summary>每行绘制 y 起点（I * nLH）。</summary>
    public readonly List<int> LineY = new();
}

/// <summary>
/// TActor 纹理装载层（批次J77）：LoadSaySurface（9504-9524）、LoadNameSurface（9526-9561）、
/// LoadNumberLableSurface（9386-9398）。
/// 原文全部与 GameCanvas/CurrentFont 句柄耦合，此处以接缝注入等价能力。
/// </summary>
public partial class TActorCore
{
    /// <summary>GameCanvas.Active and GameCanvas.Initialized（原文统一前置守卫）。</summary>
    public static Func<bool> CanvasReadyFn = () => true;

    /// <summary>CurrentFont.GetImageInfo(text) 的等效（返回 null 表示无纹理）。</summary>
    public static Func<string, LabelSurface?> GetImageInfoFn = s => LabelSurface.Of(s.Length * 6, 14);

    /// <summary>CurrentFont.TextWidth 之外的单字/单行度量，供 GetTextTexture 用。</summary>
    public static Func<string, int> TextExtentWidthFn = s => s.Length * 6;

    /// <summary>行高度量（原文取 '|' 的 cy）。</summary>
    public static Func<int> LineHeightFn = () => 14;

    /// <summary>MyGetTickCount 见 ActorMotion.cs（m_MyGetTickCountFn 同名接缝，此处复用）。</summary>

    /// <summary>m_sNameText / m_sShopNameText / m_sNumberLableText 的当前值与上一轮值。</summary>
    public string m_sNameText = "";
    public string m_sShopNameText = "";
    public string m_sNumberLableText = "";
    public string m_sCurNameText = "";
    public string m_sCurShopNameText = "";
    public string m_sCurNumberLableText = "";

    /// <summary>多行名字纹理的排版缓存（LoadedNameLayout）。</summary>
    public TextTextureLayout? LoadedNameLayout;

    // ===================== LoadSaySurface（9504-9524） =====================

    /// <summary>
    /// LoadSaySurface 1:1（9504-9524）：
    /// 首行有文本 → 画布就绪才继续（否则 Exit 且不改动任何槽位）；逐行对非空文本取纹理。
    /// 首行为空 → 清空**全部**槽位（Text/Width/Height/ImageIndexs）且不看画布状态。
    /// </summary>
    public void LoadSaySurface()
    {
        EnsureSayingSlots();

        if (SayingText.Count > 0 && SayingText[0] != "")
        {
            if (!CanvasReadyFn())
                return;

            for (int i = 0; i <= m_nSayLineCount - 1; i++)
            {
                if (i >= SayingArr.Count)
                    break;
                if (SayingText[i] != "")
                    SayingArr[i] = GetImageInfoFn(SayingText[i]) ?? new LabelSurface();
            }
        }
        else
        {
            for (int i = 0; i < SayingArr.Count; i++)
            {
                SayingText[i] = "";
                SayingArr[i].Width = 0;
                SayingArr[i].Height = 0;
                SayingArr[i].HasImage = false;
            }
        }
    }

    // ===================== LoadNameSurface（9526-9561） =====================

    /// <summary>
    /// LoadNameSurface 1:1（9526-9561）：
    /// 先无条件释放旧 m_NameTextSurface（并置 nil）→ 画布就绪且字体可用才继续（否则 Exit，
    /// 此时名字纹理已被清空但 m_sCur* 与打点**未更新**）→
    /// m_sNameText 非空时按 '\' 切分为多行并生成多行纹理
    /// （GetValidStr3_Ex 循环：空串即停）→ 摆摊时另取店名纹理 →
    /// 打点 m_dwShowShopNameTimeTick 并记录 m_sCurNameText/m_sCurShopNameText。
    /// </summary>
    public void LoadNameSurface()
    {
        // 无条件释放旧纹理（9531-9534）
        m_NameTextSurface = null;

        if (!CanvasReadyFn())
            return;
        if (!CurrentFontAvailableFn())
            return;

        if (m_sNameText.Length > 0)
        {
            var sl = new List<string>();
            string sNameText = m_sNameText;
            while (true)
            {
                if (sNameText == "")
                    break;
                sNameText = StrUtilEx.GetValidStr3Ex(sNameText, '\\', out string sName);
                sl.Add(sName);
            }

            LoadedNameLayout = BuildTextTexture(sl);
            m_NameTextSurface = LabelSurface.Of(LoadedNameLayout.Width, LoadedNameLayout.Height);
        }
        else
        {
            LoadedNameLayout = null;
        }

        // 摆摊时的店名纹理（9555-9556）
        if (m_boShopStall)
            m_ShopNameImageInfo = GetImageInfoFn(m_sShopNameText);

        m_dwShowShopNameTimeTick = (uint)MyGetTickCountFn();
        m_sCurNameText = m_sNameText;
        m_sCurShopNameText = m_sShopNameText;
    }

    /// <summary>
    /// GetTextTexture 1:1（DxCanvas 1151-1246 的几何部分）：
    /// 宽 = max(各行宽度)；行高 = '|' 的高度；高 = 行高 × 行数；
    /// 每行在 Rect(0, I*nLH, nW, ...) 内按 DT_CENTER 居中。
    /// </summary>
    public static TextTextureLayout BuildTextTexture(List<string> lines)
    {
        var layout = new TextTextureLayout();
        layout.Lines.AddRange(lines);

        int nW = 0;
        foreach (var s in lines)
        {
            int cx = TextExtentWidthFn(s);
            if (cx > nW)
                nW = cx;
        }

        int nLH = LineHeightFn();
        layout.Width = nW;
        layout.LineHeight = nLH;
        layout.Height = nLH * lines.Count;

        for (int i = 0; i < lines.Count; i++)
        {
            layout.LineY.Add(i * nLH);
            // DT_CENTER：x = (nW - 本行宽) / 2
            layout.CenteredX.Add((nW - TextExtentWidthFn(lines[i])) / 2);
        }

        return layout;
    }

    // ===================== LoadNumberLableSurface（9386-9398） =====================

    /// <summary>
    /// LoadNumberLableSurface 1:1（9386-9398）：
    /// 画布未就绪直接 Exit（**连打点都不做**）→ 打点 → 无条件清零数字纹理三字段 →
    /// 字体可用时记录 m_sCurNumberLableText 并取纹理（字体不可用时保持清零状态）。
    /// </summary>
    public void LoadNumberLableSurface()
    {
        if (!CanvasReadyFn())
            return;

        m_ShowNumberLableTimeTick = (uint)MyGetTickCountFn();

        m_NumberLableImageInfo = new LabelSurface();

        if (CurrentFontAvailableFn())
        {
            m_sCurNumberLableText = m_sNumberLableText;
            m_NumberLableImageInfo = GetImageInfoFn(m_sNumberLableText) ?? new LabelSurface();
        }
    }
}
