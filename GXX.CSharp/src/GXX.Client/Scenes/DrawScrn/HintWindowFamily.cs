// ============================================================================================
// 车道 `p10-client-scrn`：DrawScrn.pas 的**提示窗族** 1:1 移植。
//
//   原文区间            类
//   377-406   THintWindow   （= class(TList)，提示窗）
//   408-428   THintWindows  （提示窗集合 + 临界区）
//   2164-2500 / 2503-2690  实现
//
// ★ 与 FState 的关系判定（本车道任务书第 1 条）——**结论：是同一份，FStateSeams 里的那两个只是接缝**：
//   `GXX.Client/GUI/Share/FStateSeams.cs:407-452` 的 `THintLines` 与 `:599-602` 的 `THintWindows`
//   都写着 `【接缝：待 DrawScrn.pas 移植后接入】`，且 FStateSeams.cs:410-411 明确写
//   「车道8 已在其 TStateWindowsText.cs 中登记同一缺口，但归属为 DrawScrn.pas，**不是** FState.pas」。
//   FState.pas 自身**不声明** THintLines/THintWindows（`FStatePure.cs:75 GetHitLines(THintLines, …)`
//   只是**使用**它们，`FState.pas` 的 uses 里有 DrawScrn）。
//   ⇒ 正式归属是本文件（DrawScrn.pas:318 / :408）。
//   ⇒ 本车道**不改** GUI/Share/**（任务书硬性禁止），因此以**独立命名空间**（GXX.Client.Scenes）
//     落地真实现，并在交付报告 §「与 FState 的同一份 vs 两份」给出**最小改法**：
//       (1) 删除 `FStateSeams.cs:407-452` 的 THintLines 接缝、(2) 删除 `:599-612` 的 THintWindows
//           + DrawScrn 静态接缝、(3) 给 FStatePure.cs / TStateWindowsText.cs / GuiSharePureTests.cs
//           各加一行 `using GXX.Client.Scenes;`（或在调用点写全名）。
//     最小改法**不改任何 FState 逻辑**，只是让接缝指向正式归属。
//
// 注：`THintLines` 的真实实现在 `HintMessageFamily.cs`（同一命名空间）。
// ============================================================================================

using System;
using System.Collections.Generic;
using GXX.Client.GUI.DxComponent;
using GXX.Client.GUI.Mir;
using GXX.Client.GUI.Share;
using GXX.Core.Rtl;
using GXX.Core.Util;
using TRect = GXX.Client.GUI.DxComponent.TRect;

namespace GXX.Client.Scenes;

/// <summary>DrawScrn.pas:377-406 THintWindow（提示窗；原文继承 Delphi Classes.TList）。</summary>
public class THintWindow : List<object>
{
    public bool FVisible;
    public int HintX, HintY, HintWidth, HintHeight;
    public bool HintUp;

    public bool FShowBackground;
    public TRect HintRect;

    /// <summary>DrawScrn.pas:385/2187-2230 procedure DrawBackground()。</summary>
    public void DrawBackground()
    {
        TRect sourceRect;
        int nWidth, nHeight;
        TTexture d;
        TColor color;
        sourceRect = DrawScrnRect.Bounds(HintRect.Left + 2, HintRect.Top + 2,
            HintRect.Right - HintRect.Left - 2, HintRect.Bottom - HintRect.Top - 2);
        // SourceRect := Bounds(HintRect.Left + 2, HintRect.Top + 2, HintRect.Right - HintRect.Left - 4, HintRect.Bottom - HintRect.Top - 4);

        color = DrawScrnEnv.GetTColor(DrawScrnEnv.btHintWindowbackgroundColor);
        DrawScrnEnv.GameCanvas.FillRectAlpha(sourceRect, color, DrawScrnEnv.btHintWindowbackgroundAlpha); // $00005E5E   $00006C6C  GetRGB(18)
        sourceRect = HintRect;
        if (FShowBackground && DrawScrnEnv.boShowHintWindowFrame)
        {
            nWidth = sourceRect.Right - sourceRect.Left;
            nHeight = sourceRect.Bottom - sourceRect.Top;
            d = DrawScrnEnv.g_WNewopUIImages[44]; // 横
            if (d != null)
            {
                DrawScrnEnv.GameCanvas.StretchDraw(DrawScrnRect.Bounds(HintRect.Left + 2, HintRect.Top + 2, nWidth, d.Height), d);
                DrawScrnEnv.GameCanvas.StretchDraw(DrawScrnRect.Bounds(HintRect.Left + 2, HintRect.Top + 2 + (nHeight - d.Height) + 1, nWidth, d.Height), d);
            }
            d = DrawScrnEnv.g_WNewopUIImages[45]; // 竖
            if (d != null)
            {
                DrawScrnEnv.GameCanvas.StretchDraw(DrawScrnRect.Bounds(HintRect.Left + 2, HintRect.Top + 2, d.Width, nHeight), d);
                DrawScrnEnv.GameCanvas.StretchDraw(DrawScrnRect.Bounds(HintRect.Left + 2 + (nWidth - d.Width) + 1, HintRect.Top + 2, d.Width, nHeight), d);
            }

            d = DrawScrnEnv.g_WNewopUIImages[42]; // 左上角
            if (d != null)
                DrawScrnEnv.GameCanvas.Draw(HintRect.Left + 2, HintRect.Top + 2, d);

            d = DrawScrnEnv.g_WNewopUIImages[40]; // 右上角
            if (d != null)
                DrawScrnEnv.GameCanvas.Draw(HintRect.Left + 2 + (nWidth - d.Width), HintRect.Top + 2, d);

            d = DrawScrnEnv.g_WNewopUIImages[43]; // 左下角
            if (d != null)
                DrawScrnEnv.GameCanvas.Draw(HintRect.Left + 2, HintRect.Top + 2 + (nHeight - d.Height), d);

            d = DrawScrnEnv.g_WNewopUIImages[41]; // 右下角
            if (d != null)
                DrawScrnEnv.GameCanvas.Draw(HintRect.Left + 2 + (nWidth - d.Width), HintRect.Top + 2 + (nHeight - d.Height), d);
        }
    }

    /// <summary>DrawScrn.pas:386/2364-2371 procedure SetHintX(Value:Integer)。</summary>
    public void SetHintX(int value)
    {
        if (HintX != value)
        {
            HintX = value;
            if (HintX < 0) HintX = 0;
            HintRect = DrawScrnRect.Bounds(HintX, HintY, DrawScrnRect.Max(HintWidth, 20), DrawScrnRect.Max(HintHeight, 20));
        }
    }

    /// <summary>DrawScrn.pas:387/2373-2380 procedure SetHintY(Value:Integer)。</summary>
    public void SetHintY(int value)
    {
        if (HintY != value)
        {
            HintY = value;
            if (HintY < 0) HintY = 0;
            HintRect = DrawScrnRect.Bounds(HintX, HintY, DrawScrnRect.Max(HintWidth, 20), DrawScrnRect.Max(HintHeight, 20));
        }
    }

    /// <summary>DrawScrn.pas:389 constructor Create。</summary>
    public THintWindow()
    {
        FVisible = true;
    }

    /// <summary>DrawScrn.pas:390/2170-2174 destructor Destroy; override。</summary>
    public void Free()
    {
        Clear();
    }

    /// <summary>DrawScrn.pas:391/2176-2185 procedure Clear; override（FVisible 置 False + 逐项 Free）。</summary>
    public new void Clear()
    {
        FVisible = false;
        for (int i = 0; i < Count; i++)
        {
            ((THintLines)this[i]).Free();
        }
        base.Clear();
    }

    /// <summary>DrawScrn.pas:393/2232-2284 procedure ShowColor(X, Y; Msg; DrawUp; DrawLeft; ShowBackground)。</summary>
    public void ShowColor(int x, int y, string msg, bool drawUp, bool drawLeft, bool showBackground)
    {
        int nPos;
        string sMsg = "";
        string sColor;
        THintLines hintLines;
        TColor color;
        if (msg == "") return;

        HintWidth = 0;
        HintHeight = 0;
        while (true)
        {
            if (msg == "") break;
            msg = HUtil32.GetValidStr3_Ex(msg, ref sMsg, '\\');
            if (sMsg != "")
            {
                color = TColor.clWhite;
                nPos = DelphiRTL.Pos("/", sMsg);
                if (nPos > 0)
                {
                    sColor = DelphiRTL.Copy(sMsg, 1, nPos - 1);
                    sMsg = DelphiRTL.Copy(sMsg, nPos + 1, sMsg.Length - nPos);
                    color = DrawScrnEnv.GetTColor((byte)DelphiRTL.StrToIntDef(sColor, 255));
                }
                hintLines = new THintLines();

                // 默认为加描边 chongchong 2017-04-16
                hintLines.Add(sMsg, color, DrawScrnEnv.GetHintFontSize, DrawScrnEnv.GetHintFontStyle(TFontStyles.fsNone), DrawScrnEnv.GetHintFontStroke(true));
                Add(hintLines);
                HintWidth = DrawScrnRect.Max(hintLines.Width, HintWidth);
                HintHeight = HintHeight + hintLines.ItemHeight;
            }
        }

        HintHeight = HintHeight + DrawScrnEnv.HintWindowBorderWidth.Top + DrawScrnEnv.HintWindowBorderWidth.Bottom - 5;

        HintX = x;
        HintY = y;

        HintUp = drawUp;

        HintWidth = HintWidth + DrawScrnEnv.HintWindowBorderWidth.Left + DrawScrnEnv.HintWindowBorderWidth.Right - 6; // 24;

        if (HintUp) HintY = HintY - HintHeight;
        if (drawLeft) HintX = HintX - HintWidth;
        if (HintX < 0) HintX = 0;
        if (HintX + HintWidth > DrawScrnEnv.SCREENWIDTH) HintX = DrawScrnEnv.SCREENWIDTH - HintWidth;
        if (HintY < 0) HintY = 0;
        if (HintY + HintHeight > DrawScrnEnv.SCREENHEIGHT) HintY = DrawScrnEnv.SCREENHEIGHT - HintHeight;

        HintRect = DrawScrnRect.Bounds(HintX, HintY, DrawScrnRect.Max(HintWidth, 20), DrawScrnRect.Max(HintHeight, 20));

        FShowBackground = showBackground;
    }

    /// <summary>DrawScrn.pas:395/2286-2326 procedure Show(X, Y; Msg; Color; DrawUp; DrawLeft; ShowBackground)。</summary>
    public void Show(int x, int y, string msg, TColor color, bool drawUp = false, bool drawLeft = false, bool showBackground = true)
    {
        string sMsg = "";
        THintLines hintLines;
        if (msg == "") return;

        HintWidth = 0;
        HintHeight = 0;
        while (true)
        {
            if (msg == "") break;
            msg = HUtil32.GetValidStr3_Ex(msg, ref sMsg, '\\');
            if (sMsg != "")
            {
                hintLines = new THintLines();
                hintLines.Add(sMsg, color, DrawScrnEnv.GetHintFontSize, DrawScrnEnv.GetHintFontStyle(TFontStyles.fsNone), DrawScrnEnv.GetHintFontStroke());
                Add(hintLines);
                HintWidth = DrawScrnRect.Max(hintLines.Width, HintWidth);
                HintHeight = HintHeight + hintLines.ItemHeight;
            }
        }

        HintHeight = HintHeight + DrawScrnEnv.HintWindowBorderWidth.Top + DrawScrnEnv.HintWindowBorderWidth.Bottom - 5;

        HintX = x;
        HintY = y;

        HintUp = drawUp;

        HintWidth = HintWidth + DrawScrnEnv.HintWindowBorderWidth.Left + DrawScrnEnv.HintWindowBorderWidth.Right - 6; // 24;

        if (HintUp) HintY = HintY - HintHeight;
        if (drawLeft) HintX = HintX - HintWidth;
        if (HintX < 0) HintX = 0;
        if (HintX + HintWidth > DrawScrnEnv.SCREENWIDTH) HintX = DrawScrnEnv.SCREENWIDTH - HintWidth;
        if (HintY < 0) HintY = 0;
        if (HintY + HintHeight > DrawScrnEnv.SCREENHEIGHT) HintY = DrawScrnEnv.SCREENHEIGHT - HintHeight;

        HintRect = DrawScrnRect.Bounds(HintX, HintY, DrawScrnRect.Max(HintWidth, 20), DrawScrnRect.Max(HintHeight, 20));

        FShowBackground = showBackground;
    }

    /// <summary>DrawScrn.pas:394/2328-2362 procedure Show(X, Y; DrawUp; DrawLeft; ShowBackground)（复用已 Add 的行）。</summary>
    public void Show(int x, int y, bool drawUp = false, bool drawLeft = false, bool showBackground = true)
    {
        THintLines hintLines;
        HintX = x;
        HintY = y;
        HintWidth = 0;
        HintHeight = 0;
        HintUp = drawUp;

        for (int i = 0; i < Count; i++)
        {
            hintLines = (THintLines)this[i];
            HintWidth = DrawScrnRect.Max(hintLines.Width, HintWidth);
            HintHeight = HintHeight + hintLines.ItemHeight;

            if (hintLines.MinWidth > 0)
            {
                HintWidth = DrawScrnRect.Max(hintLines.MinWidth, HintWidth);
            }
        }

        HintWidth = HintWidth + DrawScrnEnv.HintWindowBorderWidth.Left + DrawScrnEnv.HintWindowBorderWidth.Right - 6; // Max(HintWidth + 24, 120);
        HintHeight = HintHeight + DrawScrnEnv.HintWindowBorderWidth.Top + DrawScrnEnv.HintWindowBorderWidth.Bottom - 5;

        if (HintUp) HintY = HintY - HintHeight;
        if (drawLeft) HintX = HintX - HintWidth;
        if (HintX < 0) HintX = 0;
        if (HintX + HintWidth > DrawScrnEnv.SCREENWIDTH) HintX = DrawScrnEnv.SCREENWIDTH - HintWidth;
        if (HintY < 0) HintY = 0;
        if (HintY + HintHeight > DrawScrnEnv.SCREENHEIGHT) HintY = DrawScrnEnv.SCREENHEIGHT - HintHeight;

        HintRect = DrawScrnRect.Bounds(HintX, HintY, DrawScrnRect.Max(HintWidth, 20), DrawScrnRect.Max(HintHeight, 20));

        FShowBackground = showBackground;
    }

    /// <summary>DrawScrn.pas:396/2398-2500 procedure Draw()。</summary>
    public void Draw()
    {
        THintLines hintLines;
        TRect paintRect;
        TRect lineRect = default;
        int hintIconWidth;
        bool haveWinHintImage;
        DrawBackground();
        paintRect = HintRect;

        // 优化提示信息过多时占CPU资源 chongchong 2013-11-16
        if (paintRect.Left > DrawScrnEnv.SCREENWIDTH) return;
        if (paintRect.Top > DrawScrnEnv.SCREENHEIGHT) return;

        hintIconWidth = 0;
        for (int i = 0; i < Count; i++)
        {
            hintLines = (THintLines)this[i];

            if (hintLines.IsItemIconText)
                hintIconWidth = DrawScrnRect.Max(hintIconWidth, hintLines.Width);
        }

        haveWinHintImage = false;
        int nY = paintRect.Top;
        for (int i = 0; i < Count; i++)
        {
            hintLines = (THintLines)this[i];

            switch (hintLines.Alignment)
            {
                case TAlignment.taLeftJustify:
                    {
                        lineRect = DrawScrnRect.Bounds(paintRect.Left, nY, paintRect.Right - paintRect.Left, hintLines.ItemHeight);
                    }
                    break;
                case TAlignment.taRightJustify:
                    {
                        if (hintLines.IsItemIconText)
                        {
                            lineRect = DrawScrnRect.Bounds(paintRect.Left, nY, hintIconWidth, hintLines.ItemHeight);
                            DrawScrnRect.OffsetRect(ref lineRect, (paintRect.Right - paintRect.Left - hintIconWidth - DrawScrnEnv.HintWindowBorderWidth.Right - 6), 0);
                        }
                        else
                        {
                            lineRect = DrawScrnRect.Bounds(paintRect.Left, nY, hintLines.Width, hintLines.ItemHeight);
                            DrawScrnRect.OffsetRect(ref lineRect, (paintRect.Right - paintRect.Left - hintLines.Width - DrawScrnEnv.HintWindowBorderWidth.Right - 6), 0);
                        }
                    }
                    break;
                case TAlignment.taCenter:
                    {
                        if (hintLines.IsItemIconText)
                        {
                            lineRect = DrawScrnRect.Bounds(paintRect.Left, nY, hintIconWidth, hintLines.ItemHeight);
                            DrawScrnRect.OffsetRect(ref lineRect, (paintRect.Right - paintRect.Left - hintIconWidth - DrawScrnEnv.HintWindowBorderWidth.Right - 6) / 2, 0);
                        }
                        else
                        {
                            lineRect = DrawScrnRect.Bounds(paintRect.Left, nY, hintLines.Width, hintLines.ItemHeight);
                            DrawScrnRect.OffsetRect(ref lineRect, (paintRect.Right - paintRect.Left - hintLines.Width - DrawScrnEnv.HintWindowBorderWidth.Right - 6) / 2, 0);
                        }
                    }
                    break;
                default:
                    // ★ 原文缺陷（照抄）：`case` **没有 else 分支**，若 Alignment 落在三个值之外，
                    //   `LineRect` 会沿用上一轮的矩形。TAlignment 仅三值，故实际不可达；
                    //   此处保留"沿用上一轮"的语义（lineRect 不重新赋值）。
                    break;
            }

            hintLines.PaintWithoutWinHintImage(paintRect, lineRect, ref haveWinHintImage);
            nY = nY + hintLines.ItemHeight;

            // 优化提示信息过多时占CPU资源 chongchong 2013-11-16
            if (nY > DrawScrnEnv.SCREENHEIGHT) break;
        }

        // <WinNewopUI:259这种最上层绘制
        if (haveWinHintImage)
        {
            nY = paintRect.Top;
            for (int i = 0; i < Count; i++)
            {
                hintLines = (THintLines)this[i];

                switch (hintLines.Alignment)
                {
                    case TAlignment.taLeftJustify:
                        {
                            lineRect = DrawScrnRect.Bounds(paintRect.Left, nY, paintRect.Right - paintRect.Left, hintLines.ItemHeight);
                        }
                        break;
                    case TAlignment.taRightJustify:
                        {
                            if (hintLines.IsItemIconText)
                            {
                                lineRect = DrawScrnRect.Bounds(paintRect.Left, nY, hintIconWidth, hintLines.ItemHeight);
                                DrawScrnRect.OffsetRect(ref lineRect, (paintRect.Right - paintRect.Left - hintIconWidth - DrawScrnEnv.HintWindowBorderWidth.Right - 6), 0);
                            }
                            else
                            {
                                lineRect = DrawScrnRect.Bounds(paintRect.Left, nY, hintLines.Width, hintLines.ItemHeight);
                                DrawScrnRect.OffsetRect(ref lineRect, (paintRect.Right - paintRect.Left - hintLines.Width - DrawScrnEnv.HintWindowBorderWidth.Right - 6), 0);
                            }
                        }
                        break;
                    case TAlignment.taCenter:
                        {
                            if (hintLines.IsItemIconText)
                            {
                                lineRect = DrawScrnRect.Bounds(paintRect.Left, nY, hintIconWidth, hintLines.ItemHeight);
                                DrawScrnRect.OffsetRect(ref lineRect, (paintRect.Right - paintRect.Left - hintIconWidth - DrawScrnEnv.HintWindowBorderWidth.Right - 6) / 2, 0);
                            }
                            else
                            {
                                lineRect = DrawScrnRect.Bounds(paintRect.Left, nY, hintLines.Width, hintLines.ItemHeight);
                                DrawScrnRect.OffsetRect(ref lineRect, (paintRect.Right - paintRect.Left - hintLines.Width - DrawScrnEnv.HintWindowBorderWidth.Right - 6) / 2, 0);
                            }
                        }
                        break;
                    default:
                        break;
                }

                hintLines.PaintWinHintImage(paintRect, lineRect);
                nY = nY + hintLines.ItemHeight;

                // 优化提示信息过多时占CPU资源 chongchong 2013-11-16
                if (nY > DrawScrnEnv.SCREENHEIGHT) break;
            }
        }
    }

    /// <summary>DrawScrn.pas:398 property Visible。</summary>
    public bool Visible { get => FVisible; set => FVisible = value; }

    /// <summary>DrawScrn.pas:399 property Width:Integer read HintWidth write HintWidth。</summary>
    public int Width { get => HintWidth; set => HintWidth = value; }

    /// <summary>DrawScrn.pas:400 property Height:Integer read HintHeight write HintHeight。</summary>
    public int Height { get => HintHeight; set => HintHeight = value; }

    /// <summary>DrawScrn.pas:401 property X:Integer read HintX write SetHintX。</summary>
    public int X { get => HintX; set => SetHintX(value); }

    /// <summary>DrawScrn.pas:402 property Y:Integer read HintY write SetHintY。</summary>
    public int Y { get => HintY; set => SetHintY(value); }

    /// <summary>DrawScrn.pas:404/2382-2388 procedure SetHintXExt(Value:Integer)（**不夹紧 &lt; 0**，与 SetHintX 不同）。</summary>
    public void SetHintXExt(int value)
    {
        if (HintX != value)
        {
            HintX = value;
            HintRect = DrawScrnRect.Bounds(HintX, HintY, DrawScrnRect.Max(HintWidth, 20), DrawScrnRect.Max(HintHeight, 20));
        }
    }

    /// <summary>DrawScrn.pas:405/2390-2396 procedure SetHintYExt(Value:Integer)（**不夹紧 &lt; 0**）。</summary>
    public void SetHintYExt(int value)
    {
        if (HintY != value)
        {
            HintY = value;
            HintRect = DrawScrnRect.Bounds(HintX, HintY, DrawScrnRect.Max(HintWidth, 20), DrawScrnRect.Max(HintHeight, 20));
        }
    }
}

/// <summary>DrawScrn.pas:408-428 THintWindows（提示窗集合；原文用 TRTLCriticalSection 保护）。</summary>
public class THintWindows
{
    public List<object> FWindows;
    public readonly TRTLCriticalSection CriticalSection = new TRTLCriticalSection();

    /// <summary>DrawScrn.pas:415/2503-2508 constructor Create。</summary>
    public THintWindows()
    {
        FWindows = new List<object>();
    }

    /// <summary>DrawScrn.pas:416/2510-2520 destructor Destroy; override。</summary>
    public void Free()
    {
        for (int i = 0; i < FWindows.Count; i++)
        {
            ((THintWindow)FWindows[i]).Free();
        }
    }

    /// <summary>DrawScrn.pas:417/2522-2535 procedure Initialize。</summary>
    public void Initialize()
    {
        CriticalSection.Enter();
        try
        {
            for (int i = 0; i < FWindows.Count; i++)
            {
                ((THintWindow)FWindows[i]).Free();
            }
            FWindows.Clear();
        }
        finally
        {
            CriticalSection.Leave();
        }
    }

    /// <summary>
    /// DrawScrn.pas:418/2537-2550 procedure Finalize。
    /// <para>★ 托管侧命名 `Finalize_`：`System.Object.Finalize` 是保留成员，直接声明会触发 CS0114
    /// （工程既有先例：`TGameImages.Finalize_()`）。语义与原文一致。</para>
    /// </summary>
    public void Finalize_()
    {
        CriticalSection.Enter();
        try
        {
            for (int i = 0; i < FWindows.Count; i++)
            {
                ((THintWindow)FWindows[i]).Free();
            }
            FWindows.Clear();
        }
        finally
        {
            CriticalSection.Leave();
        }
    }

    /// <summary>DrawScrn.pas:419/2590-2625 procedure Draw()。</summary>
    public void Draw()
    {
        THintWindow hintWindow;
        CriticalSection.Enter();
        try
        {
            if (FWindows.Count > 0)
            {
                hintWindow = (THintWindow)FWindows[FWindows.Count - 1];
                if (hintWindow.Visible)
                { // 检测是不是有新的 悬浮显示 如果有把不显示的删除 否则会闪烁
                    for (int i = FWindows.Count - 1; i >= 0; i--)
                    {
                        hintWindow = (THintWindow)FWindows[i];
                        if (!hintWindow.Visible)
                        {
                            FWindows.RemoveAt(i); // 原文 Classes.TList.Delete(I) 1:1
                            hintWindow.Free();
                        }
                    }
                }

                for (int i = 0; i < FWindows.Count; i++)
                {
                    hintWindow = (THintWindow)FWindows[i];
                    hintWindow.Draw();
                }

                for (int i = FWindows.Count - 1; i >= 0; i--)
                {
                    hintWindow = (THintWindow)FWindows[i];
                    if (!hintWindow.Visible)
                    {
                        FWindows.RemoveAt(i); // 原文 Classes.TList.Delete(I) 1:1
                        hintWindow.Free();
                    }
                }
            }
        }
        finally
        {
            CriticalSection.Leave();
        }
    }

    /// <summary>DrawScrn.pas:420/2627-2643 procedure Clear。</summary>
    public void Clear()
    {
        THintWindow hintWindow;
        CriticalSection.Enter();
        try
        {
            for (int i = FWindows.Count - 1; i >= 0; i--)
            {
                hintWindow = (THintWindow)FWindows[i];
                hintWindow.Free();
            }
            DrawScrnEnv.g_LastHintMakeIndex = -1;
            FWindows.Clear();
        }
        finally
        {
            CriticalSection.Leave();
        }
    }

    /// <summary>DrawScrn.pas:421/2645-2653 procedure Add(HintWindow:THintWindow)。</summary>
    public void Add(THintWindow hintWindow)
    {
        CriticalSection.Enter();
        try
        {
            FWindows.Add(hintWindow);
        }
        finally
        {
            CriticalSection.Leave();
        }
    }

    /// <summary>DrawScrn.pas:422/2675-2690 procedure UpDate（原文整段被注释，空体）。</summary>
    public void UpDate()
    {
        // 原文整个方法体被 { } 注释掉 ⇒ 空实现（原文如此）
    }

    /// <summary>DrawScrn.pas:423/2552-2569 procedure ShowColor(X, Y; Msg; DrawUp; DrawLeft; ShowBackground)。</summary>
    public void ShowColor(int x, int y, string msg, bool drawUp = false, bool drawLeft = false, bool showBackground = true)
    {
        THintWindow hintWindow;
        CriticalSection.Enter();
        try
        {
            for (int i = 0; i < FWindows.Count; i++)
            {
                ((THintWindow)FWindows[i]).Visible = false;
            }

            hintWindow = new THintWindow();
            hintWindow.ShowColor(x, y, msg, drawUp, drawLeft, showBackground);
            FWindows.Add(hintWindow);
        }
        finally
        {
            CriticalSection.Leave();
        }
    }

    /// <summary>DrawScrn.pas:424/2571-2588 procedure Show(X, Y; Msg; Color; DrawUp; DrawLeft; ShowBackground)。</summary>
    public void Show(int x, int y, string msg, TColor color, bool drawUp = false, bool drawLeft = false, bool showBackground = true)
    {
        THintWindow hintWindow;
        CriticalSection.Enter();
        try
        {
            for (int i = 0; i < FWindows.Count; i++)
            {
                ((THintWindow)FWindows[i]).Visible = false;
            }

            hintWindow = new THintWindow();
            hintWindow.Show(x, y, msg, color, drawUp, drawLeft, showBackground);
            FWindows.Add(hintWindow);
        }
        finally
        {
            CriticalSection.Leave();
        }
    }

    /// <summary>DrawScrn.pas:413/2655-2663 function GetCount:Integer。</summary>
    public int GetCount()
    {
        CriticalSection.Enter();
        try
        {
            return FWindows.Count;
        }
        finally
        {
            CriticalSection.Leave();
        }
    }

    /// <summary>DrawScrn.pas:414/2665-2673 function GetItems(Index:Integer):THintWindow。</summary>
    public THintWindow GetItems(int index)
    {
        CriticalSection.Enter();
        try
        {
            return (THintWindow)FWindows[index];
        }
        finally
        {
            CriticalSection.Leave();
        }
    }

    /// <summary>DrawScrn.pas:426 property Count:Integer read GetCount。</summary>
    public int Count => GetCount();

    /// <summary>DrawScrn.pas:427 property Items[Index:Integer]:THintWindow read GetItems。</summary>
    public THintWindow Items(int index) => GetItems(index);
}
