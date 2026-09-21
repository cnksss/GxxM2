// ============================================================================================
// 车道 `p10-client-scrn`：DrawScrn.pas 的**新式消息族** 1:1 移植。
//
//   原文区间            类
//   456-482   TDrawScreenCenterNewlineMsg （中心换行消息 piaoyun 2013-08-02；实现 4903-5264）
//   484-522   TDrawScreenNewMoveMsg       （新式逐行滚动消息；实现 5268-5535）
//   580-590   TScreenNewMoveMsgList       （集合；实现 4004-4099）
//   603-613   TMoveHintMsgList            （鼠标跟随提示；实现 4102-4272）
//   615-625   TScreenNewLineMsgList       （集合；实现 4276-4371）
//
// 列表承载：原文 `TGList` → `GXX.Core.Protocol.SDK.TGList`（`Items[I]`→`[I]`、`Delete(I)`→`RemoveAt(I)`）。
// ============================================================================================

using System;
using System.Collections.Generic;
using GXX.Client.GUI.DxComponent;
using GXX.Client.GUI.Mir;
using GXX.Client.GUI.Share;
using GXX.Core.Rtl;
using GXX.Core.Util;
using TRect = GXX.Client.GUI.DxComponent.TRect;
using TGList = GXX.Core.Protocol.SDK.TGList;
using TGStringList = GXX.Core.Protocol.SDK.TGStringList;

namespace GXX.Client.Scenes;

/// <summary>DrawScrn.pas:456-482 TDrawScreenCenterNewlineMsg（中心换行消息，位置由 Y 决定）。</summary>
public class TDrawScreenCenterNewlineMsg
{
    public List<object> FCacheList;

    public TGStringList m_ShowLines; // 信息内容链
    public byte m_FColor; // 前景色
    public byte m_BColor; // 背景色
    public byte m_btFontSize; // 字体大小
    public uint m_dwStartStandTick; // 开始停留时间
    public int m_nX, m_nY; // Y坐标
    public int m_nCurY; // 当前Y坐标
    public byte m_nState; // 当前状态
    public int m_nStandTime; // 停留时间
    public byte m_nDrawType; // 绘制方式 0带透明框 1淡入淡出

    public bool m_boShowOver;

    /// <summary>DrawScrn.pas:473/4903-4915 procedure ClearTimeCache。</summary>
    public void ClearTimeCache()
    {
        for (int i = FCacheList.Count - 1; i >= 0; i--)
        {
            var cacheText = (TDrawScreenNewMsgCacheText)FCacheList[i];
            if (DrawScrnEnv.tick_diff(cacheText.nAddTick, DrawScrnEnv.MyGetTickCount) >= (uint)(cacheText.nTime + 8000))
            {
                FCacheList.RemoveAt(i);
            }
        }
    }

    /// <summary>DrawScrn.pas:475/5012-5025 constructor Create。</summary>
    public TDrawScreenCenterNewlineMsg()
    {
        m_ShowLines = new TGStringList();
        m_FColor = 255;
        m_BColor = 0;
        m_btFontSize = 20;
        m_nStandTime = 10;
        m_nDrawType = 0;

        m_boShowOver = false;

        FCacheList = new List<object>();
    }

    /// <summary>DrawScrn.pas:476/5027-5035 destructor Destroy; override。</summary>
    public void Free()
    {
        ClearShowLines(true);
        m_ShowLines = null;

        ClearCacheLines();
        FCacheList = null;
    }

    /// <summary>DrawScrn.pas:477/4917-4979 procedure Add(const sMsg:string; FColor, BColor, FontSize:Byte; nX, nY, nTime, nDrawType:Integer)。</summary>
    public void Add(string sMsg, byte fcolor, byte bcolor, byte fontSize, int nX, int nY, int nTime, int nDrawType)
    {
        int i;
        THGEFont hgeFont;
        string sTemp;
        if (sMsg.Length == 0) return;

        if (m_ShowLines.Count > 0)
        {
            var cacheText = new TDrawScreenNewMsgCacheText
            {
                sMsg = sMsg,
                FColor = fcolor,
                BColor = bcolor,
                FontSize = fontSize,
                nX = nX,
                nY = nY,
                nTime = nTime,
                nDrawType = nDrawType,
                nAddTick = DrawScrnEnv.MyGetTickCount,
            };

            FCacheList.Add(cacheText);
            return;
        }

        m_ShowLines.Lock();
        try
        {
            m_nCurY = 0;
            m_nState = 0;
            m_nStandTime = nTime * 1000;
            m_FColor = fcolor;
            m_BColor = bcolor;
            m_btFontSize = fontSize;
            m_nY = nY;
            m_nX = nX;
            m_nDrawType = (byte)nDrawType;

            // ClearShowLines(False);

            hgeFont = DrawScrnEnv.FindFont(DrawScrnEnv.g_sCurFontName, m_btFontSize);
            if (hgeFont != null)
            {
                sTemp = sMsg.Replace("||", "\r\n"); // Delphi sLineBreak = #13#10
                var slLines = new GXX.Core.Util.TStringList();
                try
                {
                    slLines.Text = sTemp;
                    for (i = 0; i < slLines.Count; i++)
                    {
                        var tokenLine = new TStringLineEx();
                        DrawScrnText.GetTextListEx(slLines[i], DrawScrnEnv.GetTColor(m_FColor), DrawScrnEnv.GetTColor(m_BColor), tokenLine);
                        m_ShowLines.AddObject(slLines[i], tokenLine);
                    }
                }
                finally
                {
                    // slLines.Free
                }
            }
        }
        finally
        {
            m_ShowLines.UnLock();
        }
    }

    /// <summary>DrawScrn.pas:480/4981-4997 procedure ClearShowLines(Lock:Boolean = True)。</summary>
    public void ClearShowLines(bool @lock = true)
    {
        if (@lock)
            m_ShowLines.Lock();

        for (int i = 0; i < m_ShowLines.Count; i++)
        {
            // 原文 TokenLine.Free（托管侧无资源）
        }
        m_ShowLines.Clear();

        if (@lock)
            m_ShowLines.UnLock();
    }

    /// <summary>DrawScrn.pas:481/4999-5010 procedure ClearCacheLines。</summary>
    public void ClearCacheLines()
    {
        for (int i = 0; i < FCacheList.Count; i++)
        {
            // 原文 Dispose(CacheText)（托管侧无资源）
        }

        FCacheList.Clear();
    }

    /// <summary>DrawScrn.pas:478/5037-5146 procedure Draw(boDrawBack:Boolean)。</summary>
    public void Draw(bool boDrawBack)
    {
        string sText;
        int nX, nY, nTextHeight;
        THGEFont hgeFont;
        int nCode;

        nCode = 0;

        if ((m_nDrawType / 100 == 0) && !boDrawBack) return;

        int nDrawType = m_nDrawType % 100;
        try
        {
            if (nDrawType == 1)
            {
                nCode = 1;
                DrawEx();
                return;
            }

            nCode = 2;
            m_ShowLines.Lock();
            try
            {
                if (m_ShowLines.Count == 0) return;

                nCode = 3;
                if (m_nState == 0)
                {
                    m_dwStartStandTick = DrawScrnEnv.MyGetTickCount;
                    m_nState = 1;
                }

                nCode = 4;
                if (DrawScrnEnv.MyGetTickCount - m_dwStartStandTick >= (uint)m_nStandTime)
                {
                    nCode = 5;
                    ClearShowLines(false);

                    nCode = 6;
                    if (FCacheList.Count > 0)
                    {
                        nCode = 7;
                        var cacheText = (TDrawScreenNewMsgCacheText)FCacheList[0];
                        Add(cacheText.sMsg, cacheText.FColor, cacheText.BColor, cacheText.FontSize, cacheText.nX, cacheText.nY, cacheText.nTime, cacheText.nDrawType);

                        nCode = 8;
                        FCacheList.RemoveAt(0);
                    }
                    else
                    {
                        m_boShowOver = true;
                    }

                    return;
                }

                nCode = 9;
                // 加粗
                hgeFont = DrawScrnEnv.FindFont(DrawScrnEnv.g_sCurFontName, m_btFontSize, TFontStyles.fsBold);

                nCode = 10;
                nTextHeight = DrawScrnEnv.TextHeight(hgeFont, "Pp");
                nY = m_ShowLines.Count * (nTextHeight + 2);

                nCode = 11;
                if (nDrawType == 0)
                    // 绘制透明矩形框
                    DrawScrnEnv.GameCanvas.FillRectAlpha(DrawScrnRect.Rect(80, m_nY, DrawScrnEnv.SCREENWIDTH - 80, m_nY + nY + 4), DrawScrnEnv.GetTColor(190), 100);

                nCode = 12;
                if (hgeFont != null)
                {
                    nTextHeight = DrawScrnEnv.TextHeight(hgeFont, "Pp");
                    // nY := (SCREENHEIGHT - m_ShowLines.Count * nTextHeight) div 2;
                    nY = m_nY + 4;
                    for (int i = 0; i < m_ShowLines.Count; i++)
                    {
                        nCode = 13;
                        var tokenLine = (TStringLineEx)m_ShowLines.GetObject(i);

                        nCode = 14;
                        sText = DrawScrnText.GetStrinLineExText(tokenLine);
                        if (m_nX == 0)
                            nX = (DrawScrnEnv.SCREENWIDTH - DrawScrnEnv.TextWidth(hgeFont, sText)) / 2;
                        else
                            nX = m_nX;

                        nCode = 15;
                        for (int j = 0; j < tokenLine.Count; j++)
                        {
                            var token = tokenLine[j];
                            DrawScrnEnv.BoldTextOut(hgeFont, nX, nY, token.Text, token.FColor, token.BColor);
                            nX = nX + DrawScrnEnv.TextWidth(hgeFont, token.Text);
                        }

                        nCode = 16;
                        nY += nTextHeight + 2;
                    }
                }
            }
            finally
            {
                m_ShowLines.UnLock();
            }
        }
        catch (Exception)
        {
            DrawScrnEnv.DebugOutStr("[Exception] TDrawScreenCenterNewlineMsg::Draw Error; Code = " + DelphiRTL.IntToStr(nCode));
        }
    }

    /// <summary>DrawScrn.pas:479/5148-5264 procedure DrawEx。</summary>
    public void DrawEx()
    {
        int nX, nY, nTextHeight;
        THGEFont hgeFont;
        byte nTempAlpha = 0;

        TStringLineEx tokenLine;
        string sText;
        m_ShowLines.Lock();
        try
        {
            if (m_ShowLines.Count == 0) return;

            // 加粗
            hgeFont = DrawScrnEnv.FindFont(DrawScrnEnv.g_sCurFontName, m_btFontSize, TFontStyles.fsBold);

            nTextHeight = DrawScrnEnv.TextHeight(hgeFont, "Pp");
            nY = m_ShowLines.Count * (nTextHeight + 2);

            if ((m_nState == 0) && (m_nCurY == 0))
                m_nCurY = m_nY + nY * 1;
            else
                m_nCurY -= 1;

            if ((m_nCurY <= m_nY) && (m_nState == 0))
            {
                m_nState = 1;
                m_nCurY = m_nY;
                // 开始停留
                m_dwStartStandTick = DrawScrnEnv.MyGetTickCount;
            }

            if (m_nState != 1)
            {
                nTempAlpha = unchecked((byte)(255 - Math.Abs((int)Math.Round((double)(m_nCurY - m_nY) / (1 * nY) * 255))));
                if ((m_nState == 2) && (m_nCurY < m_nY - nY))
                {
                    // m_boShowOver := True;

                    ClearShowLines(false);

                    if (FCacheList.Count == 0)
                    {
                        m_boShowOver = true;
                    }
                    else
                    {
                        while (FCacheList.Count > 0)
                        {
                            var cacheText = (TDrawScreenNewMsgCacheText)FCacheList[0];
                            if (DrawScrnEnv.tick_diff(cacheText.nAddTick, DrawScrnEnv.MyGetTickCount) >= (uint)(cacheText.nTime + 5000))
                            {
                                FCacheList.RemoveAt(0);

                                if (FCacheList.Count == 0)
                                    m_boShowOver = true;
                            }
                            else
                            {
                                Add(cacheText.sMsg, cacheText.FColor, cacheText.BColor, cacheText.FontSize, cacheText.nX, cacheText.nY, cacheText.nTime, cacheText.nDrawType);

                                FCacheList.RemoveAt(0);
                                break;
                            }
                        }
                    }

                    return;
                }
            }
            else
            {
                m_nCurY = m_nY;
                nTempAlpha = 255;
                if (DrawScrnEnv.MyGetTickCount - m_dwStartStandTick >= (uint)m_nStandTime)
                {
                    m_nState = 2;
                }
            }

            // 绘制透明矩形框
            // DrawScrnEnv.GameCanvas.FillRectAlpha(Rect(80, m_nY, SCREENWIDTH - 80, m_nY + nY + 4), GetRGB(190), 100);

            if (hgeFont != null)
            {
                nTextHeight = DrawScrnEnv.TextHeight(hgeFont, "Pp");
                // nY := (SCREENHEIGHT - m_ShowLines.Count * nTextHeight) div 2;
                nY = m_nCurY;
                for (int i = 0; i < m_ShowLines.Count; i++)
                {
                    tokenLine = (TStringLineEx)m_ShowLines.GetObject(i);
                    sText = DrawScrnText.GetStrinLineExText(tokenLine);

                    if (m_nX == 0)
                        nX = (DrawScrnEnv.SCREENWIDTH - DrawScrnEnv.TextWidth(hgeFont, sText)) / 2;
                    else
                        nX = m_nX;

                    for (int j = 0; j < tokenLine.Count; j++)
                    {
                        var token = tokenLine[j];
                        if (m_nState == 1)
                            DrawScrnEnv.BoldTextOut(hgeFont, nX, nY, token.Text, token.FColor, token.BColor, nTempAlpha);
                        else
                            DrawScrnEnv.BoldTextOutEx(hgeFont, nX, nY, token.Text, token.FColor, token.BColor, nTempAlpha);
                        nX = nX + DrawScrnEnv.TextWidth(hgeFont, token.Text);
                    }

                    nY += nTextHeight + 2;
                }
            }
        }
        finally
        {
            m_ShowLines.UnLock();
        }
    }
}

/// <summary>DrawScrn.pas:484-522 TDrawScreenNewMoveMsg（逐行淡入淡出滚动消息）。</summary>
public class TDrawScreenNewMoveMsg
{
    public List<object> FCacheList;

    public TGStringList m_ShowLines; // 信息内容链
    public byte m_FontSize;
    public TColor m_FColor, m_BColor; // 前景色/背景色

    public int m_TotalCount, m_CurrentCount; // 次数

    public int m_nX;
    public int m_Y0, m_Y1, m_Y2; // 三行文字的定位位置

    public int m_OffsetY1, m_OffsetY2; // 文字当前显示位置

    public byte m_Alpha1, m_Alpha2; // 文字的透明度

    public int m_CurLineIndex;
    public bool m_IsStayShow; // 是否为停留显示 （不是滚动显示）

    public uint m_StartStayTime; // 开始停留时间
    public uint m_LastTime;

    public int m_LineHeight; // 每一行的高度
    public int m_StepMove; // 每一步移动几个点
    public int m_StepAlphaChange; // 每移动一步的透明度改变值

    public THGEFont m_HGEFont;

    public bool m_ShowOver;

    /// <summary>DrawScrn.pas:515/5268-5273 constructor Create。</summary>
    public TDrawScreenNewMoveMsg()
    {
        m_ShowLines = new TGStringList();
        m_ShowOver = false;
        FCacheList = new List<object>();
    }

    /// <summary>DrawScrn.pas:516/5275-5283 destructor Destroy; override。</summary>
    public void Free()
    {
        ClearShowLines();
        m_ShowLines = null;

        ClearCacheLines();
        FCacheList = null;
    }

    /// <summary>DrawScrn.pas:517/5285-5296 procedure ClearShowLines。</summary>
    public void ClearShowLines()
    {
        for (int i = 0; i < m_ShowLines.Count; i++)
        {
            // 原文 TokenLine.Free
        }

        m_ShowLines.Clear();
    }

    /// <summary>DrawScrn.pas:518/5298-5309 procedure ClearCacheLines。</summary>
    public void ClearCacheLines()
    {
        for (int i = 0; i < FCacheList.Count; i++)
        {
            // 原文 Dispose(CacheText)
        }

        FCacheList.Clear();
    }

    /// <summary>DrawScrn.pas:520/5311-5380 procedure Add(sMsg:string; FColor, BColor:Byte; FontSize:Byte; nX, nY, nCount:Integer)。</summary>
    public void Add(string sMsg, byte fcolor, byte bcolor, byte fontSize, int nX, int nY, int nCount)
    {
        int i;
        if (sMsg.Length == 0) return;

        var slLines = new GXX.Core.Util.TStringList();
        m_ShowLines.Lock();
        try
        {
            if (m_ShowLines.Count > 0)
            {
                var cacheText = new TDrawScreenNewMsgCacheText
                {
                    sMsg = sMsg,
                    FColor = fcolor,
                    BColor = bcolor,
                    FontSize = fontSize,
                    nX = nX,
                    nY = nY,
                    nCount = nCount,
                };

                FCacheList.Add(cacheText);
                return;
            }

            m_HGEFont = DrawScrnEnv.FindFont(DrawScrnEnv.g_sCurFontName, fontSize, TFontStyles.fsBold);
            if (m_HGEFont == null) return;

            // ClearShowLines;

            m_FontSize = fontSize;
            m_FColor = DrawScrnEnv.GetTColor(fcolor);
            m_BColor = DrawScrnEnv.GetTColor(bcolor);
            m_TotalCount = nCount;
            m_CurrentCount = 1; // 次数
            m_nX = nX;

            m_LineHeight = DrawScrnEnv.TextHeight(m_HGEFont, "文字") + 2;
            m_Y0 = nY - m_LineHeight;
            m_Y1 = nY;
            m_Y2 = nY + m_LineHeight;
            m_StepMove = 2;
            m_StepAlphaChange = (int)Math.Round(250.0 / (m_LineHeight / (double)m_StepMove));

            // 拆分每行
            sMsg = sMsg.Replace("||", "\r\n"); // Delphi sLineBreak = #13#10
            slLines.Text = sMsg;
            for (i = 0; i < slLines.Count; i++)
            {
                var tokenLine = new TStringLineEx();
                DrawScrnText.GetTextListEx(slLines[i], m_FColor, m_BColor, tokenLine);
                m_ShowLines.AddObject(slLines[i], tokenLine);
            }

            m_CurLineIndex = 0;
            m_IsStayShow = true; // 是否为停留显示 （不是滚动显示）
            m_StartStayTime = DrawScrnEnv.MyGetTickCount; // 开始停留时间
        }
        finally
        {
            m_ShowLines.UnLock();
        }
    }

    /// <summary>DrawScrn.pas:521/5382-5535 procedure Draw()。</summary>
    public void Draw()
    {
        int nX, nY;
        string s;
        TStringLineEx tokenLine;
        if (m_HGEFont == null) return;
        m_ShowLines.Lock();
        try
        {
            if (m_IsStayShow)
            {
                if (DrawScrnEnv.MyGetTickCount - m_StartStayTime <= 2000)
                {
                    if (m_CurLineIndex > m_ShowLines.Count - 1)
                    {
                        if (m_CurrentCount < m_TotalCount)
                        {
                            m_CurrentCount++;
                            m_CurLineIndex = 0;
                        }
                        else
                        {
                            // m_ShowOver := True;

                            ClearShowLines();

                            if (FCacheList.Count == 0)
                            {
                                m_ShowOver = true;
                            }
                            else
                            {
                                while (FCacheList.Count > 0)
                                {
                                    var cacheText = (TDrawScreenNewMsgCacheText)FCacheList[0];
                                    if (DrawScrnEnv.tick_diff(cacheText.nAddTick, DrawScrnEnv.MyGetTickCount) >= (uint)(cacheText.nTime + 5000))
                                    {
                                        FCacheList.RemoveAt(0);

                                        if (FCacheList.Count == 0)
                                            m_ShowOver = true;
                                    }
                                    else
                                    {
                                        Add(cacheText.sMsg, cacheText.FColor, cacheText.BColor, cacheText.FontSize, cacheText.nX, cacheText.nY, cacheText.nCount);

                                        FCacheList.RemoveAt(0);
                                        break;
                                    }
                                }
                            }

                            return;
                        }
                    }

                    tokenLine = (TStringLineEx)m_ShowLines.GetObject(m_CurLineIndex);
                    s = DrawScrnText.GetStrinLineExText(tokenLine);
                    if (m_nX == 0)
                        nX = (DrawScrnEnv.SCREENWIDTH - DrawScrnEnv.TextWidth(m_HGEFont, s)) / 2;
                    else
                        nX = m_nX;
                    nY = m_Y1;
                    for (int i = 0; i < tokenLine.Count; i++)
                    {
                        var token = tokenLine[i];
                        DrawScrnEnv.BoldTextOut(m_HGEFont, nX, nY, token.Text, token.FColor, token.BColor);
                        nX = nX + DrawScrnEnv.TextWidth(m_HGEFont, token.Text);
                    }
                }
                else
                {
                    m_IsStayShow = false;

                    m_OffsetY1 = m_Y1;
                    m_OffsetY2 = m_Y2;

                    m_Alpha1 = 250;
                    m_Alpha2 = 0;

                    m_LastTime = unchecked(DrawScrnEnv.MyGetTickCount - 1000);
                }

                return;
            }

            if (DrawScrnEnv.MyGetTickCount - m_LastTime >= 100)
            {
                m_OffsetY1 = m_OffsetY1 - m_StepMove;
                m_Alpha1 = unchecked((byte)(m_Alpha1 - m_StepAlphaChange));

                m_OffsetY2 = m_OffsetY2 - m_StepMove;
                m_Alpha2 = unchecked((byte)(m_Alpha2 + m_StepAlphaChange));

                if ((m_OffsetY1 <= m_Y0) || (m_OffsetY2 <= m_Y1))
                {
                    m_OffsetY1 = m_Y0;
                    m_Alpha1 = 0;

                    m_OffsetY2 = m_Y1;
                    m_Alpha2 = 255;

                    m_CurLineIndex++;

                    m_IsStayShow = true;
                    m_StartStayTime = DrawScrnEnv.MyGetTickCount;
                    return;
                }
                m_LastTime = DrawScrnEnv.MyGetTickCount;
            }

            if (m_CurLineIndex > m_ShowLines.Count - 1) return;

            tokenLine = (TStringLineEx)m_ShowLines.GetObject(m_CurLineIndex);
            s = DrawScrnText.GetStrinLineExText(tokenLine);
            if (m_nX == 0)
                nX = (DrawScrnEnv.SCREENWIDTH - DrawScrnEnv.TextWidth(m_HGEFont, s)) / 2;
            else
                nX = m_nX;
            nY = m_OffsetY1;
            for (int i = 0; i < tokenLine.Count; i++)
            {
                var token = tokenLine[i];
                DrawScrnEnv.BoldTextOutEx(m_HGEFont, nX, nY, token.Text, token.FColor, token.BColor, m_Alpha1);
                nX = nX + DrawScrnEnv.TextWidth(m_HGEFont, token.Text);
            }

            if (m_CurLineIndex + 1 <= m_ShowLines.Count - 1)
            {
                tokenLine = (TStringLineEx)m_ShowLines.GetObject(m_CurLineIndex + 1);
                s = DrawScrnText.GetStrinLineExText(tokenLine);
                if (m_nX == 0)
                    nX = (DrawScrnEnv.SCREENWIDTH - DrawScrnEnv.TextWidth(m_HGEFont, s)) / 2;
                else
                    nX = m_nX;
                nY = m_OffsetY2;
                for (int i = 0; i < tokenLine.Count; i++)
                {
                    var token = tokenLine[i];
                    DrawScrnEnv.BoldTextOutEx(m_HGEFont, nX, nY, token.Text, token.FColor, token.BColor, m_Alpha2);
                    nX = nX + DrawScrnEnv.TextWidth(m_HGEFont, token.Text);
                }
            }
        }
        finally
        {
            m_ShowLines.UnLock();
        }
    }
}

/// <summary>DrawScrn.pas:580-590 TScreenNewMoveMsgList。</summary>
public class TScreenNewMoveMsgList
{
    public TGList FItemList;

    /// <summary>DrawScrn.pas:584/4004-4007 constructor Create。</summary>
    public TScreenNewMoveMsgList()
    {
        FItemList = new TGList();
    }

    /// <summary>DrawScrn.pas:585/4009-4023 destructor Destroy; override。</summary>
    public void Free()
    {
        FItemList.Lock();
        try
        {
            for (int i = 0; i < FItemList.Count; i++)
            {
                ((TDrawScreenNewMoveMsg)FItemList[i]).Free();
            }
        }
        finally
        {
            FItemList.UnLock();
        }
        FItemList = null;
    }

    /// <summary>DrawScrn.pas:586/4025-4050 procedure Add(sMsg; FColor; BColor; FontSize; nX; nY; nCount)。</summary>
    public void Add(string sMsg, byte fcolor, byte bcolor, byte fontSize, int nX, int nY, int nCount)
    {
        bool boFind;
        FItemList.Lock();
        try
        {
            boFind = false;
            for (int i = 0; i < FItemList.Count; i++)
            {
                var moveMsg = (TDrawScreenNewMoveMsg)FItemList[i];
                if ((moveMsg.m_Y1 == nY) && (moveMsg.m_nX == nX))
                {
                    moveMsg.Add(sMsg, fcolor, bcolor, fontSize, nX, nY, nCount);
                    boFind = true;
                    break;
                }
            }
            if (!boFind)
            {
                var moveMsg = new TDrawScreenNewMoveMsg();
                moveMsg.Add(sMsg, fcolor, bcolor, fontSize, nX, nY, nCount);
                FItemList.Add(moveMsg);
            }
        }
        finally
        {
            FItemList.UnLock();
        }
    }

    /// <summary>DrawScrn.pas:587/4052-4064 procedure Draw()。</summary>
    public void Draw()
    {
        FItemList.Lock();
        try
        {
            for (int i = 0; i < FItemList.Count; i++)
            {
                ((TDrawScreenNewMoveMsg)FItemList[i]).Draw();
            }
        }
        finally
        {
            FItemList.UnLock();
        }
    }

    /// <summary>DrawScrn.pas:588/4066-4083 procedure Update。</summary>
    public void Update()
    {
        FItemList.Lock();
        try
        {
            for (int i = FItemList.Count - 1; i >= 0; i--)
            {
                var moveMsg = (TDrawScreenNewMoveMsg)FItemList[i];
                if (moveMsg.m_ShowOver)
                {
                    FItemList.RemoveAt(i);
                    moveMsg.Free();
                }
            }
        }
        finally
        {
            FItemList.UnLock();
        }
    }

    /// <summary>DrawScrn.pas:589/4085-4099 procedure Clear。</summary>
    public void Clear()
    {
        FItemList.Lock();
        try
        {
            for (int i = 0; i < FItemList.Count; i++)
            {
                ((TDrawScreenNewMoveMsg)FItemList[i]).Free();
            }
            FItemList.Clear();
        }
        finally
        {
            FItemList.UnLock();
        }
    }
}

/// <summary>DrawScrn.pas:603-613 TMoveHintMsgList（鼠标跟随提示，逐帧上移渐隐）。</summary>
public class TMoveHintMsgList
{
    public TGList FItemList;

    /// <summary>DrawScrn.pas:607/4102-4105 constructor Create。</summary>
    public TMoveHintMsgList()
    {
        FItemList = new TGList();
    }

    /// <summary>DrawScrn.pas:608/4107-4121 destructor Destroy; override（原文循环体为空）。</summary>
    public void Free()
    {
        FItemList.Lock();
        try
        {
            for (int i = 0; i < FItemList.Count; i++)
            {
                // ★ 原文缺陷（照抄）：析构循环体**为空** —— 元素不 Dispose（内存泄漏，原文如此）。
            }
        }
        finally
        {
            FItemList.UnLock();
        }
        FItemList = null;
    }

    /// <summary>DrawScrn.pas:609/4123-4163 procedure Add(sMsg:string; FColor, BColor:Byte; nX, nY:Integer)。</summary>
    public void Add(string sMsg, byte fcolor, byte bcolor, int nX, int nY)
    {
        FItemList.Lock();
        try
        {
            var msgRecord = new TMoveHintMsgRecord
            {
                Msg = sMsg,
                FColor = fcolor,
                BColor = bcolor,
            };

            if (nX == 0)
            {
                msgRecord.nX = DrawScrnEnv.g_nMoveMouseX;

                if (DrawScrnEnv.DMerchantDlg_Visible)
                {
                    msgRecord.nX = msgRecord.nX - DrawScrnEnv.DMerchantDlg_VirtualRect.Left;
                }
            }
            else
            {
                msgRecord.nX = nX;
            }

            if (nY == 0)
            {
                msgRecord.nY = DrawScrnEnv.g_nMoveMouseY - DrawScrnEnv.g_CurrentFontHeight - 5;

                if (DrawScrnEnv.DMerchantDlg_Visible)
                {
                    msgRecord.nY = msgRecord.nY - DrawScrnEnv.DMerchantDlg_VirtualRect.Top;
                }
            }
            else
            {
                msgRecord.nY = nY + 60;
            }

            msgRecord.nYOffset = 0;
            msgRecord.LastUpdateTick = DrawScrnEnv.MyGetTickCount;

            FItemList.Add(msgRecord);
        }
        finally
        {
            FItemList.UnLock();
        }
    }

    /// <summary>DrawScrn.pas:610/4165-4229 procedure Draw()。</summary>
    public void Draw()
    {
        int j;
        TImageInfo imageInfo;
        THGEFont hgeFont;
        string s = "", s2;
        int count;

        int oX, oY;
        hgeFont = DrawScrnEnv.FindFont(DrawScrnEnv.g_sCurFontName, 9);
        if (hgeFont == null) return;

        if (DrawScrnEnv.DMerchantDlg_Visible)
        {
            oX = DrawScrnEnv.DMerchantDlg_VirtualRect.Left;
            oY = DrawScrnEnv.DMerchantDlg_VirtualRect.Top;
        }
        else
        {
            oX = 0; // HZQ 20230525 补全初始化
            oY = 0;
        }

        FItemList.Lock();
        try
        {
            if (FItemList.Count > 0)
            { // HZQ 20230525 先判断Count大于0再去构造TStringList
                var sl = new GXX.Core.Util.TStringList();
                try
                {
                    for (int i = 0; i < FItemList.Count; i++)
                    {
                        var msgRecord = (TMoveHintMsgRecord)FItemList[i];

                        sl.Clear();

                        count = 0;
                        s2 = msgRecord.Msg;
                        while (true)
                        {
                            s2 = HUtil32.GetValidStr3_Ex(s2, ref s, '\\');

                            if (s.Length == 0) break;
                            sl.Add(s);
                            count++;
                            if (count >= 10) break;
                        }

                        for (j = 0; j < sl.Count; j++)
                        {
                            s = sl[j];

                            imageInfo = DrawScrnEnv.GetImageInfo(hgeFont, s);

                            DrawScrnEnv.FontTextOut(hgeFont, oX + msgRecord.nX - 1, oY + msgRecord.nY - msgRecord.nYOffset - (DrawScrnEnv.g_CurrentFontHeight + 2) * (sl.Count - 1 - j), imageInfo.ImageIndexs, DrawScrnEnv.GetTColor(msgRecord.BColor));
                            DrawScrnEnv.FontTextOut(hgeFont, oX + msgRecord.nX + 1, oY + msgRecord.nY - msgRecord.nYOffset - (DrawScrnEnv.g_CurrentFontHeight + 2) * (sl.Count - 1 - j), imageInfo.ImageIndexs, DrawScrnEnv.GetTColor(msgRecord.BColor));
                            DrawScrnEnv.FontTextOut(hgeFont, oX + msgRecord.nX, oY + msgRecord.nY - 1 - msgRecord.nYOffset - (DrawScrnEnv.g_CurrentFontHeight + 2) * (sl.Count - 1 - j), imageInfo.ImageIndexs, DrawScrnEnv.GetTColor(msgRecord.BColor));
                            DrawScrnEnv.FontTextOut(hgeFont, oX + msgRecord.nX, oY + msgRecord.nY + 1 - msgRecord.nYOffset - (DrawScrnEnv.g_CurrentFontHeight + 2) * (sl.Count - 1 - j), imageInfo.ImageIndexs, DrawScrnEnv.GetTColor(msgRecord.BColor));
                            DrawScrnEnv.FontTextOut(hgeFont, oX + msgRecord.nX, oY + msgRecord.nY - msgRecord.nYOffset - (DrawScrnEnv.g_CurrentFontHeight + 2) * (sl.Count - 1 - j), imageInfo.ImageIndexs, DrawScrnEnv.GetTColor(msgRecord.FColor));
                        }
                    }
                }
                finally
                {
                    // SL.Free
                }
            }
        }
        finally
        {
            FItemList.UnLock();
        }
    }

    /// <summary>DrawScrn.pas:611/4231-4255 procedure Update。</summary>
    public void Update()
    {
        FItemList.Lock();
        try
        {
            for (int i = FItemList.Count - 1; i >= 0; i--)
            {
                var msgRecord = (TMoveHintMsgRecord)FItemList[i];

                if (msgRecord.nYOffset >= 40)
                {
                    if (DrawScrnEnv.MyGetTickCount - msgRecord.LastUpdateTick >= 1500)
                    {
                        FItemList.RemoveAt(i);
                    }
                }
                else if (DrawScrnEnv.MyGetTickCount - msgRecord.LastUpdateTick >= 10)
                {
                    msgRecord.LastUpdateTick = DrawScrnEnv.MyGetTickCount;
                    msgRecord.nYOffset += 2;
                }
            }
        }
        finally
        {
            FItemList.UnLock();
        }
    }

    /// <summary>DrawScrn.pas:612/4257-4272 procedure Clear。</summary>
    public void Clear()
    {
        FItemList.Lock();
        try
        {
            for (int i = 0; i < FItemList.Count; i++)
            {
                // 原文 Dispose(MsgRecord)
            }
            FItemList.Clear();
        }
        finally
        {
            FItemList.UnLock();
        }
    }
}

/// <summary>DrawScrn.pas:615-625 TScreenNewLineMsgList。</summary>
public class TScreenNewLineMsgList
{
    public TGList FItemList;

    /// <summary>DrawScrn.pas:619/4276-4279 constructor Create。</summary>
    public TScreenNewLineMsgList()
    {
        FItemList = new TGList();
    }

    /// <summary>DrawScrn.pas:620/4281-4295 destructor Destroy; override。</summary>
    public void Free()
    {
        FItemList.Lock();
        try
        {
            for (int i = 0; i < FItemList.Count; i++)
            {
                ((TDrawScreenCenterNewlineMsg)FItemList[i]).Free();
            }
        }
        finally
        {
            FItemList.UnLock();
        }
        FItemList = null;
    }

    /// <summary>DrawScrn.pas:621/4297-4323 procedure Add(sMsg; FColor; BColor; FontSize; nX; nY; nTime; nDrawType)。</summary>
    public void Add(string sMsg, byte fcolor, byte bcolor, byte fontSize, int nX, int nY, int nTime, int nDrawType)
    {
        bool boFind;
        FItemList.Lock();
        try
        {
            boFind = false;
            for (int i = 0; i < FItemList.Count; i++)
            {
                var moveMsg = (TDrawScreenCenterNewlineMsg)FItemList[i];
                if ((moveMsg.m_nY == nY) && (moveMsg.m_nX == nX))
                {
                    moveMsg.ClearTimeCache();
                    moveMsg.Add(sMsg, fcolor, bcolor, fontSize, nX, nY, nTime, nDrawType);
                    boFind = true;
                    break;
                }
            }
            if (!boFind)
            {
                var moveMsg = new TDrawScreenCenterNewlineMsg();
                moveMsg.Add(sMsg, fcolor, bcolor, fontSize, nX, nY, nTime, nDrawType);
                FItemList.Add(moveMsg);
            }
        }
        finally
        {
            FItemList.UnLock();
        }
    }

    /// <summary>DrawScrn.pas:622/4325-4337 procedure Draw(boDrawBack:Boolean)。</summary>
    public void Draw(bool boDrawBack)
    {
        FItemList.Lock();
        try
        {
            for (int i = 0; i < FItemList.Count; i++)
            {
                ((TDrawScreenCenterNewlineMsg)FItemList[i]).Draw(boDrawBack);
            }
        }
        finally
        {
            FItemList.UnLock();
        }
    }

    /// <summary>DrawScrn.pas:623/4339-4356 procedure Update。</summary>
    public void Update()
    {
        FItemList.Lock();
        try
        {
            for (int i = FItemList.Count - 1; i >= 0; i--)
            {
                var moveMsg = (TDrawScreenCenterNewlineMsg)FItemList[i];
                if (moveMsg.m_boShowOver)
                {
                    FItemList.RemoveAt(i);
                    moveMsg.Free();
                }
            }
        }
        finally
        {
            FItemList.UnLock();
        }
    }

    /// <summary>DrawScrn.pas:624/4358-4371 procedure Clear。</summary>
    public void Clear()
    {
        FItemList.Lock();
        try
        {
            for (int i = 0; i < FItemList.Count; i++)
            {
                ((TDrawScreenCenterNewlineMsg)FItemList[i]).Free();
            }
            FItemList.Clear();
        }
        finally
        {
            FItemList.UnLock();
        }
    }
}
