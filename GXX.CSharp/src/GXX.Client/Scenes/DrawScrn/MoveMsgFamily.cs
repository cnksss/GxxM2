// ============================================================================================
// 车道 `p10-client-scrn`：DrawScrn.pas 的**滚动/延迟消息族** 1:1 移植。
//
//   原文区间            类
//   430-442   TDrawScreenCenterMsg  （屏幕中央消息；实现 3116-3264）
//   524-537   TDrawDelayMsg         （延迟消息 + %d/%s 倒计时；实现 3268-3475）
//   539-562   TDrawScreenMoveMsg    （单条跑马灯；实现 3479-3870）
//   564-578   TScreenMoveMsgList    （按 Y 分组的跑马灯集合；实现 3872-4000）
//
// 列表承载：原文 `TGList` → 复用 `GXX.Core.Protocol.SDK.TGList`
//（`Items[I]` → 索引器 `[I]`；`Delete(I)` → `RemoveAt(I)`；Lock/UnLock/Count 同名）。
// ============================================================================================

using System;
using System.Collections.Generic;
using GXX.Client.GUI.DxComponent;
using GXX.Client.GUI.Mir;
using GXX.Client.GUI.Share;
using GXX.Core.Rtl;
using TRect = GXX.Client.GUI.DxComponent.TRect;
using TGList = GXX.Core.Protocol.SDK.TGList;
using TGStringList = GXX.Core.Protocol.SDK.TGStringList;

namespace GXX.Client.Scenes;

/// <summary>DrawScrn.pas:430-442 TDrawScreenCenterMsg（屏幕中央多行消息）。</summary>
public class TDrawScreenCenterMsg
{
    public TGStringList m_TextList;
    public byte m_FColor;
    public byte m_BColor;
    public uint m_dwShowTime;

    /// <summary>DrawScrn.pas:437/3116-3122 constructor Create。</summary>
    public TDrawScreenCenterMsg()
    {
        m_TextList = new TGStringList();
        m_FColor = 255;
        m_BColor = 0;
    }

    /// <summary>DrawScrn.pas:438/3124-3129 destructor Destroy; override。</summary>
    public void Free()
    {
        Clear(false);
        m_TextList = null;
    }

    /// <summary>DrawScrn.pas:441/3131-3147 procedure Clear(Lock:Boolean = True)。</summary>
    public void Clear(bool @lock = true)
    {
        if (@lock)
            m_TextList.Lock();

        for (int i = 0; i < m_TextList.Count; i++)
        {
            // 原文 TokenLine.Free（托管侧无资源）
        }
        m_TextList.Clear();

        if (@lock)
            m_TextList.UnLock();
    }

    /// <summary>DrawScrn.pas:439/3149-3217 procedure Add(sMsg:string; FColor, BColor:Byte; nTime:Integer)。</summary>
    public void Add(string sMsg, byte fcolor, byte bcolor, int nTime)
    {
        int i;
        THGEFont hgeFont;
        var tokenLines = new List<object>();
        string sText;
        m_TextList.Lock();
        try
        {
            m_dwShowTime = unchecked(DrawScrnEnv.MyGetTickCount + (uint)nTime * 1000);
            m_FColor = fcolor;
            m_BColor = bcolor;

            Clear(false);

            hgeFont = DrawScrnEnv.FindFont(DrawScrnEnv.g_sCurFontName, 20);
            if (hgeFont != null)
            {
                // 原文此处还有一段被 { } 注释掉的"按 SCREENWIDTH 逐字折行"旧实现（3173-3200），不移植
                try
                {
                    DrawScrnText.GetTextListEx(hgeFont, sMsg, DrawScrnEnv.GetTColor(m_FColor), DrawScrnEnv.GetTColor(m_BColor),
                        tokenLines, DrawScrnEnv.clNone, DrawScrnEnv.SCREENWIDTH - 20);

                    for (i = 0; i < tokenLines.Count; i++)
                    {
                        var tokenLine = (TStringLineEx)tokenLines[i];
                        sText = DrawScrnText.GetStrinLineExText(tokenLine);
                        m_TextList.AddObject(sText, tokenLine);
                    }
                }
                finally
                {
                    // TokenLines.Free
                }
            }
        }
        finally
        {
            m_TextList.UnLock();
        }
    }

    /// <summary>DrawScrn.pas:440/3219-3264 procedure Draw()。</summary>
    public void Draw()
    {
        int nX, nY, nTextHeight;
        THGEFont hgeFont;
        TStringLineEx tokenLine;
        string s;
        m_TextList.Lock();
        try
        {
            if (m_TextList.Count > 0)
            {
                if (DrawScrnEnv.MyGetTickCount < m_dwShowTime)
                {
                    hgeFont = DrawScrnEnv.FindFont(DrawScrnEnv.g_sCurFontName, 20);
                    if (hgeFont != null)
                    {
                        nTextHeight = DrawScrnEnv.TextHeight(hgeFont, "Pp");

                        nY = (DrawScrnEnv.SCREENHEIGHT - m_TextList.Count * nTextHeight) / 2;

                        for (int i = 0; i < m_TextList.Count; i++)
                        {
                            tokenLine = (TStringLineEx)m_TextList.GetObject(i);
                            s = DrawScrnText.GetStrinLineExText(tokenLine);
                            nX = (DrawScrnEnv.SCREENWIDTH - DrawScrnEnv.TextWidth(hgeFont, s)) / 2;
                            for (int j = 0; j < tokenLine.Count; j++)
                            {
                                var token = tokenLine[j];
                                DrawScrnEnv.BoldTextOut(hgeFont, nX, nY, token.Text, token.FColor, token.BColor);
                                nX = nX + DrawScrnEnv.TextWidth(hgeFont, token.Text);
                            }

                            nY += nTextHeight;
                        }
                    }
                }
                else
                {
                    Clear(false);
                }
            }
        }
        finally
        {
            m_TextList.UnLock();
        }
    }
}

/// <summary>DrawScrn.pas:524-537 TDrawDelayMsg（SendCenterMsg 延迟消息 + %d/%s 倒计时替换）。</summary>
public class TDrawDelayMsg
{
    public uint m_dwDrawFrameCount;
    public TGList m_MsgList;
    public bool m_MoveDraw; // SendCenterMsg只显示几行 chongchong 2014-07-05
    public int m_MoveOffset; // SendCenterMsg只显示几行 chongchong 2014-07-05

    /// <summary>DrawScrn.pas:531/3268-3274 constructor Create。</summary>
    public TDrawDelayMsg()
    {
        m_dwDrawFrameCount = DrawScrnEnv.MyGetTickCount;
        m_MsgList = new TGList();
        m_MoveDraw = false;
        m_MoveOffset = 0;
    }

    /// <summary>DrawScrn.pas:532/3276-3281 destructor Destroy; override。</summary>
    public void Free()
    {
        Clear();
        m_MsgList = null;
    }

    /// <summary>DrawScrn.pas:535/3283-3299 procedure Clear。</summary>
    public void Clear()
    {
        m_MsgList.Lock();
        try
        {
            for (int i = 0; i < m_MsgList.Count; i++)
            {
                // 原文 DelayMsg.Tokens.Free + Dispose(DelayMsg)（托管侧无操作）
            }
            m_MsgList.Clear();
        }
        finally
        {
            m_MsgList.UnLock();
        }
    }

    /// <summary>DrawScrn.pas:536/3301-3319 procedure Delete(RecogId:Int64)。</summary>
    public void Delete(long recogId)
    {
        m_MsgList.Lock();
        try
        {
            for (int i = m_MsgList.Count - 1; i >= 0; i--)
            {
                var delayMsg = (TDelayMsg)m_MsgList[i];
                if (delayMsg.RecogId == recogId)
                {
                    m_MsgList.RemoveAt(i);
                }
            }
        }
        finally
        {
            m_MsgList.UnLock();
        }
    }

    /// <summary>DrawScrn.pas:533/3321-3361 procedure Add(RecogId:Int64; sMsg:string; nTime:Integer; FColor, BColor:TColor; nX:Integer)。</summary>
    public void Add(long recogId, string sMsg, int nTime, TColor fcolor, TColor bcolor, int nX)
    {
        m_MsgList.Lock();
        try
        {
            for (int i = 0; i < m_MsgList.Count; i++)
            {
                var exist = (TDelayMsg)m_MsgList[i];
                if (exist.RecogId == recogId)
                {
                    exist.RecogId = recogId;
                    exist.Msg = sMsg;
                    exist.Time = unchecked(DrawScrnEnv.MyGetTickCount + (uint)nTime * 1000);
                    exist.FColor = fcolor;
                    exist.BColor = bcolor;
                    exist.X = nX;
                    if (exist.Tokens != null)
                        exist.Tokens.Clear();
                    DrawScrnText.GetTextListEx(sMsg, fcolor, bcolor, exist.Tokens);

                    return;
                }
            }

            var delayMsg = new TDelayMsg
            {
                RecogId = recogId,
                Msg = sMsg,
                Time = unchecked(DrawScrnEnv.MyGetTickCount + (uint)nTime * 1000),
                FColor = fcolor,
                BColor = bcolor,
                X = nX,
                Tokens = new TStringLineEx(),
            };
            DrawScrnText.GetTextListEx(sMsg, fcolor, bcolor, delayMsg.Tokens);
            m_MsgList.Add(delayMsg);

            m_MoveDraw = true;
            m_MoveOffset = 0;
        }
        finally
        {
            m_MsgList.UnLock();
        }
    }

    /// <summary>DrawScrn.pas:534/3363-3475 procedure Draw()。</summary>

    // 原文 3365-3379 的嵌套 function GetTimeStr(nTime:Integer):string
    private static string GetTimeStr(int nTime)
    {
        int nSec = nTime / 1000;
        int nMin = nSec / 60;
        if (nMin > 0)
        {
            nSec = nSec % 60;
            return DelphiRTL.IntToStr(nMin) + "分" + DelphiRTL.IntToStr(nSec) + "秒";
        }
        return DelphiRTL.IntToStr(nSec) + "秒";
    }

    // 原文 3380-3381 const SHOW_LINE_COUNT = 5
    private const int SHOW_LINE_COUNT = 5;

    public void Draw()
    {
        int y, startIndex, endIndex, offsetX;
        string sMsg;
        TDelayMsg delayMsg;
        uint curTick;
        m_MsgList.Lock();
        try
        {
            for (int i = m_MsgList.Count - 1; i >= 0; i--)
            {
                delayMsg = (TDelayMsg)m_MsgList[i];
                if (DrawScrnEnv.MyGetTickCount > delayMsg.Time)
                {
                    m_MsgList.RemoveAt(i);
                }
            }

            // SendCenterMsg只显示几行
            if (m_MsgList.Count <= SHOW_LINE_COUNT) m_MoveDraw = false;
        }
        finally
        {
            m_MsgList.UnLock();
        }

        y = DrawScrnEnv.SCREENHEIGHT - 230;
        if (m_MoveDraw)
        {
            m_MoveOffset++;
            if (m_MoveOffset >= 14)
            {
                m_MoveOffset = 0;
                m_MoveDraw = false;
            }
            y = y + m_MoveOffset;
        }

        // HGEFont := TextureFonts.FindFont(g_sCurFontName, 11, [fsBold]); //fsBold  g_sCurFontName
        if (DrawScrnEnv.CurrentFont != null)
        {
            m_MsgList.Lock();
            try
            {
                if (m_MoveDraw)
                    endIndex = m_MsgList.Count - 2;
                else
                    endIndex = m_MsgList.Count - 1;
                startIndex = DrawScrnRect.Max(endIndex - SHOW_LINE_COUNT - 1, 0);

                for (int i = startIndex; i <= endIndex; i++)
                {
                    delayMsg = (TDelayMsg)m_MsgList[i];

                    sMsg = DrawScrnText.GetStrinLineExText(delayMsg.Tokens);

                    curTick = DrawScrnEnv.MyGetTickCount;
                    if (DelphiRTL.Pos("%d", sMsg) > 0)
                        sMsg = DrawScrnEnv.AnsiReplaceText(sMsg, "%d", GetTimeStr(unchecked((int)(delayMsg.Time - curTick))));
                    if (DelphiRTL.Pos("%s", sMsg) > 0)
                        sMsg = DrawScrnEnv.AnsiReplaceText(sMsg, "%s", GetTimeStr(unchecked((int)(delayMsg.Time - curTick))));

                    if (delayMsg.X <= 0)
                    {
                        delayMsg.X = (DrawScrnEnv.SCREENWIDTH - DrawScrnEnv.TextWidth(DrawScrnEnv.CurrentFont, sMsg)) / 2;
                        if (delayMsg.X <= 0) delayMsg.X = 1;
                    }

                    offsetX = 0;
                    for (int j = 0; j < delayMsg.Tokens.Count; j++)
                    {
                        var token = delayMsg.Tokens[j];
                        sMsg = token.Text;

                        if (DelphiRTL.Pos("%d", sMsg) > 0)
                            sMsg = DrawScrnEnv.AnsiReplaceText(sMsg, "%d", GetTimeStr(unchecked((int)(delayMsg.Time - curTick))));
                        if (DelphiRTL.Pos("%s", sMsg) > 0)
                            sMsg = DrawScrnEnv.AnsiReplaceText(sMsg, "%s", GetTimeStr(unchecked((int)(delayMsg.Time - curTick))));

                        if (m_MoveDraw && (i == startIndex))
                            DrawScrnEnv.BoldTextOut(DrawScrnEnv.CurrentFont, delayMsg.X + offsetX, y, sMsg, token.FColor, token.BColor, 255 - m_MoveOffset * 18);
                        else
                            DrawScrnEnv.BoldTextOut(DrawScrnEnv.CurrentFont, delayMsg.X + offsetX, y, sMsg, token.FColor, token.BColor);

                        offsetX = offsetX + DrawScrnEnv.TextWidth(DrawScrnEnv.CurrentFont, sMsg);
                    }

                    y -= DrawScrnEnv.g_CurrentFontHeight + 2;
                }
            }
            finally
            {
                m_MsgList.UnLock();
            }
        }
    }
}

/// <summary>DrawScrn.pas:539-562 TDrawScreenMoveMsg（单条跑马灯，含水平/垂直两种方向）。</summary>
public class TDrawScreenMoveMsg
{
    public TGList m_MsgList;
    public TMoveMsg m_nCurrMoveMsg;
    public uint m_dwMoveTick;
    public int FOffSetX, FOffSetY, FTextSize, FMoveSize;
    public TRect DestRect;
    public TRect SrcRect;

    /// <summary>DrawScrn.pas:549/3479-3483 constructor Create。</summary>
    public TDrawScreenMoveMsg()
    {
        m_MsgList = new TGList();
        m_nCurrMoveMsg = null;
    }

    /// <summary>DrawScrn.pas:550/3485-3506 destructor Destroy; override。</summary>
    public void Free()
    {
        for (int i = 0; i < m_MsgList.Count; i++)
        {
            var moveMsg = (TMoveMsg)m_MsgList[i];
            if (moveMsg == m_nCurrMoveMsg)
                m_nCurrMoveMsg = null;
        }
        m_MsgList = null;

        if (m_nCurrMoveMsg != null)
        {
            m_nCurrMoveMsg = null;
        }
    }

    /// <summary>DrawScrn.pas:557/3508-3511 procedure Initialize（原文空体）。</summary>
    public void Initialize()
    {
    }

    /// <summary>DrawScrn.pas:558/3513-3516 procedure Finalize（原文空体）。</summary>
    public void Finalize_()
    {
    }

    /// <summary>DrawScrn.pas:547/3518-3521 function GetTop:Integer。</summary>
    public int GetTop() => DestRect.Top;

    /// <summary>DrawScrn.pas:548/3523-3528 function GetCount:Integer。</summary>
    public int GetCount()
    {
        m_MsgList.Lock();
        int result = m_MsgList.Count;
        m_MsgList.UnLock();
        return result;
    }

    /// <summary>DrawScrn.pas:560/3530-3535 function MoveOver:Boolean。</summary>
    public bool MoveOver()
    {
        m_MsgList.Lock();
        bool result = (m_MsgList.Count <= 0) && (m_nCurrMoveMsg == null);
        m_MsgList.UnLock();
        return result;
    }

    /// <summary>DrawScrn.pas:552-554/3645-3704 procedure Add(sMsg; FColor; BColor; nY; nCount; boShowFrame; btFrameColor; btFontSize; boFontBold; nMarqueeTime)。</summary>
    public void Add(string sMsg, byte fcolor, byte bcolor, int nY, int nCount,
        bool boShowFrame = true, byte btFrameColor = 190, byte btFontSize = 11,
        bool boFontBold = true, int nMarqueeTime = 60)
    {
        THGEFont hgeFont;
        m_MsgList.Lock();
        try
        {
            // HGEFont := TextureFonts.FindFont(g_sCurFontName, 11, [fsBold]);

            if (boFontBold)
                hgeFont = DrawScrnEnv.FindFont(DrawScrnEnv.g_sCurFontName, btFontSize, TFontStyles.fsBold);
            else
                hgeFont = DrawScrnEnv.FindFont(DrawScrnEnv.g_sCurFontName, btFontSize, TFontStyles.fsNone);

            if (hgeFont != null)
            {
                var moveMsg = new TMoveMsg();
                moveMsg.Text = new TStringLineEx();

                moveMsg.Count = nCount;
                moveMsg.Y = nY;
                DrawScrnText.GetTextListEx2(sMsg, DrawScrnEnv.GetTColor(fcolor), DrawScrnEnv.GetTColor(bcolor), moveMsg.Text);

                string s = "";
                for (int i = 0; i < moveMsg.Text.Count; i++)
                {
                    var token = moveMsg.Text[i];
                    s = s + token.Text;
                }

                moveMsg.Width = DrawScrnEnv.TextWidth(hgeFont, s);
                moveMsg.Height = DrawScrnEnv.TextHeight(hgeFont, s);

                moveMsg.Orientation = TMoveMessageOrientation.mbHorizontal;
                moveMsg.ShowFrame = boShowFrame;
                moveMsg.FrameColor = btFrameColor;
                moveMsg.btFontSize = btFontSize;
                moveMsg.boFontBold = boFontBold;
                moveMsg.nMarqueeTime = nMarqueeTime;

                DestRect.Top = nY;
                m_MsgList.Insert(0, moveMsg);
            }
        }
        finally
        {
            m_MsgList.UnLock();
        }
    }

    /// <summary>DrawScrn.pas:559/3706-3815 procedure Update。</summary>
    public void Update()
    {
        int backgroundHeight;
        m_MsgList.Lock();
        try
        {
            if ((m_nCurrMoveMsg != null) && (m_nCurrMoveMsg.Count <= 0))
            {
                // 原文 try/except 里只做 Text.Free + Dispose，托管侧无操作
                m_nCurrMoveMsg = null;
            }

            if ((m_nCurrMoveMsg == null) && (m_MsgList.Count > 0))
            {
                m_nCurrMoveMsg = (TMoveMsg)m_MsgList[0];
                m_MsgList.RemoveAt(0);
                FMoveSize = 0;

                // BackgroundHeight := 30;
                backgroundHeight = m_nCurrMoveMsg.Height + m_nCurrMoveMsg.Height / 4;

                switch (m_nCurrMoveMsg.Orientation)
                {
                    case TMoveMessageOrientation.mbHorizontal:
                        {
                            FOffSetX = DrawScrnEnv.SCREENWIDTH - 80;
                            FOffSetY = m_nCurrMoveMsg.Y + (backgroundHeight - m_nCurrMoveMsg.Height) / 2;
                            FTextSize = m_nCurrMoveMsg.Width;

                            DestRect.Left = 80;
                            DestRect.Top = m_nCurrMoveMsg.Y; // FOffSetY;
                            DestRect.Right = DestRect.Left + DrawScrnEnv.SCREENWIDTH - 80 * 2;
                            DestRect.Bottom = DestRect.Top + backgroundHeight; // m_nCurrMoveMsg.Height;

                            SrcRect = DrawScrnRect.Bounds(0, 0, 0, m_nCurrMoveMsg.Height);
                        }
                        break;
                    case TMoveMessageOrientation.mbVertical:
                        {
                            FOffSetX = 80;
                            FOffSetY = m_nCurrMoveMsg.Y + (backgroundHeight - m_nCurrMoveMsg.Height) / 2;
                            FTextSize = m_nCurrMoveMsg.Height;

                            DestRect.Left = 80;
                            DestRect.Top = m_nCurrMoveMsg.Y; // FOffSetY;
                            DestRect.Right = DestRect.Left + DrawScrnEnv.SCREENWIDTH - 80 * 2;
                            DestRect.Bottom = DestRect.Top + backgroundHeight; // m_nCurrMoveMsg.Height;
                            SrcRect = DrawScrnRect.Bounds(0, 0, m_nCurrMoveMsg.Width, m_nCurrMoveMsg.Height);
                        }
                        break;
                    default:
                        // ★ 原文缺陷（照抄）：`case` 无 else；Orientation = mbNone 时
                        //   DestRect/FOffSetX/FOffSetY/FTextSize 全部保持上一轮的值（首次为 0）。
                        break;
                }
            }
            if (m_nCurrMoveMsg != null)
            {
                // SENDMOVEMSG滚动速度快 chongchong 2013-11-13
                backgroundHeight = m_nCurrMoveMsg.Height + m_nCurrMoveMsg.Height / 4;
                if (DrawScrnEnv.MyGetTickCount - m_dwMoveTick > (uint)m_nCurrMoveMsg.nMarqueeTime)
                { // 原值 18
                    m_dwMoveTick = DrawScrnEnv.MyGetTickCount;
                    FMoveSize += 2;
                    switch (m_nCurrMoveMsg.Orientation)
                    {
                        case TMoveMessageOrientation.mbHorizontal:
                            {
                                if (FMoveSize >= m_nCurrMoveMsg.Width + (DestRect.Right - DestRect.Left))
                                {
                                    m_nCurrMoveMsg.Count--;

                                    FMoveSize = 0;
                                    FOffSetX = DrawScrnEnv.SCREENWIDTH - 80;
                                    FOffSetY = m_nCurrMoveMsg.Y + (backgroundHeight - m_nCurrMoveMsg.Height) / 2;
                                    FTextSize = m_nCurrMoveMsg.Width;

                                    DestRect.Left = 80;
                                    DestRect.Top = m_nCurrMoveMsg.Y; // FOffSetY;
                                    DestRect.Right = DestRect.Left + DrawScrnEnv.SCREENWIDTH - 80 * 2;
                                    DestRect.Bottom = DestRect.Top + backgroundHeight; // m_nCurrMoveMsg.Height;

                                    SrcRect = DrawScrnRect.Bounds(0, 0, 0, m_nCurrMoveMsg.Height);
                                }
                                else
                                {
                                    if (FOffSetX > 80)
                                        FOffSetX -= 2;
                                    if (FMoveSize >= DestRect.Right - DestRect.Left)
                                    {
                                        SrcRect = DrawScrnRect.Bounds(FMoveSize - (DestRect.Right - DestRect.Left), 0,
                                            DrawScrnRect.Min(m_nCurrMoveMsg.Width, (DestRect.Right - DestRect.Left)), m_nCurrMoveMsg.Height);
                                    }
                                    else
                                    {
                                        SrcRect = DrawScrnRect.Bounds(0, 0, DrawScrnRect.Min(m_nCurrMoveMsg.Width, FMoveSize), m_nCurrMoveMsg.Height);
                                    }
                                }
                            }
                            break;
                        case TMoveMessageOrientation.mbVertical:
                            {
                                if (FMoveSize >= m_nCurrMoveMsg.Height)
                                {
                                    m_nCurrMoveMsg.Count--;

                                    FMoveSize = 0;
                                    FOffSetX = 80;
                                    FOffSetY = m_nCurrMoveMsg.Y + (backgroundHeight - m_nCurrMoveMsg.Height) / 2;
                                    FTextSize = m_nCurrMoveMsg.Height;

                                    DestRect.Left = 80;
                                    DestRect.Top = m_nCurrMoveMsg.Y; // FOffSetY;
                                    DestRect.Right = DestRect.Left + DrawScrnEnv.SCREENWIDTH - 80 * 2;
                                    DestRect.Bottom = DestRect.Top + backgroundHeight; // m_nCurrMoveMsg.Height;
                                    SrcRect = DrawScrnRect.Bounds(0, 0, m_nCurrMoveMsg.Width, m_nCurrMoveMsg.Height);
                                }
                                else
                                {
                                    // 原文此处为空 else 分支
                                }
                            }
                            break;
                        default:
                            // ★ 原文缺陷（照抄）：mbNone 时不移动、不递减 Count（死循环风险，原文如此）。
                            break;
                    }
                }
            }
        }
        finally
        {
            m_MsgList.UnLock();
        }
    }

    /// <summary>DrawScrn.pas:555/3817-3870 procedure Draw()。</summary>
    public void Draw()
    {
        THGEFont hgeFont;
        TImageInfo imageInfo;
        int nX;
        TRect paintRect;
        int textWidth;
        m_MsgList.Lock();
        try
        {
            if (m_nCurrMoveMsg != null)
            {
                // HGEFont := TextureFonts.FindFont(g_sCurFontName, 11, [fsBold]);
                if (m_nCurrMoveMsg.boFontBold)
                    hgeFont = DrawScrnEnv.FindFont(DrawScrnEnv.g_sCurFontName, m_nCurrMoveMsg.btFontSize, TFontStyles.fsBold);
                else
                    hgeFont = DrawScrnEnv.FindFont(DrawScrnEnv.g_sCurFontName, m_nCurrMoveMsg.btFontSize, TFontStyles.fsNone);

                if (hgeFont != null)
                {
                    if (m_nCurrMoveMsg.ShowFrame)
                        DrawScrnEnv.GameCanvas.FillRectAlpha(DestRect, DrawScrnEnv.GetTColor(m_nCurrMoveMsg.FrameColor), 100);

                    nX = 0;
                    paintRect = SrcRect;
                    for (int i = 0; i < m_nCurrMoveMsg.Text.Count; i++)
                    {
                        var token = m_nCurrMoveMsg.Text[i];
                        imageInfo = DrawScrnEnv.GetImageInfo(hgeFont, token.Text);

                        DrawScrnEnv.GameCanvas.TextRect(nX + FOffSetX - 1, FOffSetY, paintRect, DrawScrnEnv.GetTColor((byte)token.BColor.Value));
                        DrawScrnEnv.GameCanvas.TextRect(nX + FOffSetX + 1, FOffSetY, paintRect, DrawScrnEnv.GetTColor((byte)token.BColor.Value));
                        DrawScrnEnv.GameCanvas.TextRect(nX + FOffSetX, FOffSetY - 1, paintRect, DrawScrnEnv.GetTColor((byte)token.BColor.Value));
                        DrawScrnEnv.GameCanvas.TextRect(nX + FOffSetX, FOffSetY + 1, paintRect, DrawScrnEnv.GetTColor((byte)token.BColor.Value));
                        DrawScrnEnv.GameCanvas.TextRect(nX + FOffSetX, FOffSetY, paintRect, token.FColor);

                        textWidth = DrawScrnEnv.TextWidth(hgeFont, token.Text);

                        if (paintRect.Left <= textWidth)
                        {
                            nX = nX + textWidth - paintRect.Left;
                            paintRect.Left = 0;
                            paintRect.Right = paintRect.Right - textWidth - paintRect.Left;
                        }
                        else if (paintRect.Left >= textWidth)
                        {
                            paintRect.Left = paintRect.Left - textWidth;
                            paintRect.Right = paintRect.Right - textWidth;
                        }
                    }
                }
            }
        }
        finally
        {
            m_MsgList.UnLock();
        }
    }

    /// <summary>DrawScrn.pas:556 property Top:Integer read GetTop。</summary>
    public int Top => GetTop();

    /// <summary>DrawScrn.pas:561 property Count:Integer read GetCount。</summary>
    public int Count => GetCount();
}

/// <summary>DrawScrn.pas:564-578 TScreenMoveMsgList（按 DestRect.Top 分组的跑马灯集合）。</summary>
public class TScreenMoveMsgList
{
    public TGList m_MoveObjList;

    /// <summary>DrawScrn.pas:568/3872-3875 constructor Create。</summary>
    public TScreenMoveMsgList()
    {
        m_MoveObjList = new TGList();
    }

    /// <summary>DrawScrn.pas:569/3877-3891 destructor Destroy; override。</summary>
    public void Free()
    {
        m_MoveObjList.Lock();
        try
        {
            for (int i = 0; i < m_MoveObjList.Count; i++)
            {
                ((TDrawScreenMoveMsg)m_MoveObjList[i]).Free();
            }
        }
        finally
        {
            m_MoveObjList.UnLock();
        }
        m_MoveObjList = null;
    }

    /// <summary>DrawScrn.pas:570/3893-3920 procedure Add(...)。</summary>
    public void Add(string sMsg, byte fcolor, byte bcolor, int nY, int nCount,
        bool boShowFrame = true, byte btFrameColor = 190, byte btFontSize = 11,
        bool boFontBold = true, int nMarqueeTime = 60)
    {
        bool boFind;
        m_MoveObjList.Lock();
        try
        {
            boFind = false;
            for (int i = 0; i < m_MoveObjList.Count; i++)
            {
                var drawScreenMoveMsg = (TDrawScreenMoveMsg)m_MoveObjList[i];
                if (drawScreenMoveMsg.Top == nY)
                {
                    drawScreenMoveMsg.Add(sMsg, fcolor, bcolor, nY, nCount, boShowFrame, btFrameColor, btFontSize, boFontBold, nMarqueeTime);
                    boFind = true;
                    break;
                }
            }
            if (!boFind)
            {
                var drawScreenMoveMsg = new TDrawScreenMoveMsg();
                drawScreenMoveMsg.Add(sMsg, fcolor, bcolor, nY, nCount, boShowFrame, btFrameColor, btFontSize, boFontBold, nMarqueeTime);
                m_MoveObjList.Add(drawScreenMoveMsg);
            }
        }
        finally
        {
            m_MoveObjList.UnLock();
        }
    }

    /// <summary>DrawScrn.pas:571/3922-3934 procedure Draw()。</summary>
    public void Draw()
    {
        m_MoveObjList.Lock();
        try
        {
            for (int i = 0; i < m_MoveObjList.Count; i++)
            {
                ((TDrawScreenMoveMsg)m_MoveObjList[i]).Draw();
            }
        }
        finally
        {
            m_MoveObjList.UnLock();
        }
    }

    /// <summary>DrawScrn.pas:576/3936-3957 procedure Update。</summary>
    public void Update()
    {
        m_MoveObjList.Lock();
        try
        {
            for (int i = m_MoveObjList.Count - 1; i >= 0; i--)
            {
                var drawScreenMoveMsg = (TDrawScreenMoveMsg)m_MoveObjList[i];
                if (drawScreenMoveMsg.MoveOver())
                {
                    m_MoveObjList.RemoveAt(i);
                    drawScreenMoveMsg.Free();
                }
            }

            for (int i = 0; i < m_MoveObjList.Count; i++)
            {
                ((TDrawScreenMoveMsg)m_MoveObjList[i]).Update();
            }
        }
        finally
        {
            m_MoveObjList.UnLock();
        }
    }

    /// <summary>DrawScrn.pas:574/3959-3971 procedure Initialize。</summary>
    public void Initialize()
    {
        m_MoveObjList.Lock();
        try
        {
            for (int i = 0; i < m_MoveObjList.Count; i++)
            {
                ((TDrawScreenMoveMsg)m_MoveObjList[i]).Initialize();
            }
        }
        finally
        {
            m_MoveObjList.UnLock();
        }
    }

    /// <summary>DrawScrn.pas:575/3973-3985 procedure Finalize。</summary>
    public void Finalize_()
    {
        m_MoveObjList.Lock();
        try
        {
            for (int i = 0; i < m_MoveObjList.Count; i++)
            {
                ((TDrawScreenMoveMsg)m_MoveObjList[i]).Finalize_();
            }
        }
        finally
        {
            m_MoveObjList.UnLock();
        }
    }

    /// <summary>DrawScrn.pas:577/3987-4000 procedure Clear。</summary>
    public void Clear()
    {
        m_MoveObjList.Lock();
        try
        {
            for (int i = 0; i < m_MoveObjList.Count; i++)
            {
                ((TDrawScreenMoveMsg)m_MoveObjList[i]).Free();
            }
            m_MoveObjList.Clear();
        }
        finally
        {
            m_MoveObjList.UnLock();
        }
    }
}
