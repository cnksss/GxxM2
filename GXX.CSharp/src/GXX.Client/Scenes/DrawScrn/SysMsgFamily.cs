// ============================================================================================
// 车道 `p10-client-scrn`：DrawScrn.pas 的**系统消息族** 1:1 移植。
//
//   原文区间            类
//   627-640   TDrawSysMsg      （左上角系统消息；实现 2694-2774）
//   642-659   TDrawSysMsgEx    （带渐隐/删除状态的系统消息；实现 2778-3036）
//   661-672   TDrawMoveHintMsg （移动提示消息；实现 3040-3113）
//
// 列表承载：原文 `m_MsgList:TGStringList` → 复用 `GXX.Core.Protocol.SDK.TGStringList`
// （自带 Lock/UnLock，与原文的 Lock/UnLock 一一对应）。
// ============================================================================================

using System;
using GXX.Client.GUI.Mir;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using TGStringList = GXX.Core.Protocol.SDK.TGStringList;

namespace GXX.Client.Scenes;

/// <summary>DrawScrn.pas:627-640 TDrawSysMsg。</summary>
public class TDrawSysMsg
{
    public int m_nX, m_nY;
    public TColor m_FColor, m_BColor;

    public TGStringList m_MsgList;

    public bool m_boDownToUP;
    public bool m_boDrawBottom;

    /// <summary>DrawScrn.pas:635/2694-2704 constructor Create。</summary>
    public TDrawSysMsg()
    {
        m_MsgList = new TGStringList();
        m_nX = 30;
        m_nY = 40;
        m_FColor = DrawScrnEnv.GetTColor(2);
        m_BColor = DrawScrnEnv.GetTColor(0);

        m_boDownToUP = false;
        m_boDrawBottom = false;
    }

    /// <summary>DrawScrn.pas:636/2706-2714 destructor Destroy; override。</summary>
    public void Free()
    {
        // 原文对每一项 Dispose(pTSysMsg(...))：托管侧 TSysMsg 为 class，由 GC 回收（无操作）
        m_MsgList = null;
    }

    /// <summary>DrawScrn.pas:637/2716-2734 procedure Add(sMsg:string; FColor, BColor:Byte)。</summary>
    public void Add(string sMsg, byte fcolor, byte bcolor)
    {
        m_MsgList.Lock();
        try
        {
            if (m_MsgList.Count >= 10)
            {
                m_MsgList.Delete(0);
            }
            var sysMsg = new TSysMsg
            {
                Time = DrawScrnEnv.MyGetTickCount,
                FColor = DrawScrnEnv.GetTColor(fcolor),
                BColor = DrawScrnEnv.GetTColor(bcolor),
            };
            m_MsgList.AddObject(sMsg, sysMsg);
        }
        finally
        {
            m_MsgList.UnLock();
        }
    }

    /// <summary>DrawScrn.pas:638/2736-2764 procedure Draw()。</summary>
    public void Draw()
    {
        int sx, sy;
        TSysMsg sysMsg;
        if (DrawScrnEnv.g_MySelf == null) return;
        m_MsgList.Lock();
        try
        {
            if (m_MsgList.Count > 0)
            {
                sx = m_nX;
                sy = m_nY;
                for (int i = 0; i < m_MsgList.Count; i++)
                {
                    sysMsg = (TSysMsg)m_MsgList.GetObject(i);
                    DrawScrnEnv.BoldTextOut(sx, sy, m_MsgList[i], sysMsg.FColor, sysMsg.BColor);

                    if (m_boDownToUP)
                        sy -= 16;
                    else
                        sy += 16;
                }
                if (DrawScrnEnv.MyGetTickCount - ((TSysMsg)m_MsgList.GetObject(0)).Time >= 3000)
                {
                    m_MsgList.Delete(0);
                }
            }
        }
        finally
        {
            m_MsgList.UnLock();
        }
    }

    /// <summary>DrawScrn.pas:639/2766-2774 procedure Clear（**不 Dispose 元素**，原文如此）。</summary>
    public void Clear()
    {
        m_MsgList.Lock();
        try
        {
            m_MsgList.Clear();
        }
        finally
        {
            m_MsgList.UnLock();
        }
    }
}

/// <summary>DrawScrn.pas:642-659 TDrawSysMsgEx（渐移 + 渐隐删除）。</summary>
public class TDrawSysMsgEx
{
    public int m_nX, m_nY;
    public TColor m_FColor, m_BColor;

    public bool m_boWantDelete;
    public uint m_boWantDeleteTick;
    public int m_nOffsetY;
    public uint m_dwOffsetTick;
    public bool m_boDownToUP;

    public TGStringList m_MsgList;

    /// <summary>DrawScrn.pas:654/2778-2791 constructor Create。</summary>
    public TDrawSysMsgEx()
    {
        m_MsgList = new TGStringList();
        m_nX = 30;
        m_nY = 40;
        m_FColor = DrawScrnEnv.GetTColor(2);
        m_BColor = DrawScrnEnv.GetTColor(0);

        m_boWantDelete = false;
        m_boWantDeleteTick = DrawScrnEnv.MyGetTickCount;
        m_nOffsetY = 0;
        m_dwOffsetTick = DrawScrnEnv.MyGetTickCount;
        m_boDownToUP = false;
    }

    /// <summary>DrawScrn.pas:655/2793-2801 destructor Destroy; override。</summary>
    public void Free()
    {
        m_MsgList = null;
    }

    /// <summary>DrawScrn.pas:656/2803-2821 procedure Add(sMsg:string; FColor, BColor:Byte)。</summary>
    public void Add(string sMsg, byte fcolor, byte bcolor)
    {
        m_MsgList.Lock();
        try
        {
            if (m_MsgList.Count >= 10)
            {
                m_MsgList.Delete(0);
            }
            var sysMsg = new TSysMsg
            {
                Time = DrawScrnEnv.MyGetTickCount,
                FColor = DrawScrnEnv.GetTColor(fcolor),
                BColor = DrawScrnEnv.GetTColor(bcolor),
            };
            m_MsgList.AddObject(sMsg, sysMsg);
        }
        finally
        {
            m_MsgList.UnLock();
        }
    }

    /// <summary>
    /// DrawScrn.pas:657/2916-3026 procedure Draw()。
    /// <para>★ 原文 2823-2914 有一整份**被 `(* *)` 注释掉的旧实现**（nShowTime/nDeleteTime/
    ///   nMinShowTime/OffsetY_Step 版）；本车道按 2916-3026 的**生效版**移植，
    ///   并在进度表里登记"注释块不移植"。</para>
    /// </summary>
    public void Draw()
    {
        int sx, sy;
        TSysMsg sysMsg;
        uint tempTime;
        int alpha;

        uint nShowTime, nStepTime; // HZQ 20230524 Integer;
        int nShowCount;
        if (DrawScrnEnv.g_MySelf == null) return;
        m_MsgList.Lock();
        try
        {
            if (m_MsgList.Count > 0)
            {
                if (m_MsgList.Count > 8)
                {
                    nShowTime = 700;  // 显示多少时间隐藏
                    nStepTime = 20;   // 多长时间渐移一个像素
                }
                else if (m_MsgList.Count > 5)
                {
                    nShowTime = 1500;
                    nStepTime = 30;
                }
                else
                {
                    nShowTime = 2000;
                    nStepTime = 50;
                }

                sysMsg = (TSysMsg)m_MsgList.GetObject(0);
                if (m_MsgList.Count > 5)
                {
                    if (!m_boWantDelete)
                    {
                        m_boWantDelete = true;
                        m_boWantDeleteTick = DrawScrnEnv.MyGetTickCount;
                        m_nOffsetY = 0;
                        m_dwOffsetTick = DrawScrnEnv.MyGetTickCount;
                    }
                }

                if (m_boWantDelete)
                {
                    if (m_nOffsetY >= 16)
                    {
                        m_MsgList.Delete(0);

                        m_boWantDelete = false;
                        m_nOffsetY = 0;
                        m_dwOffsetTick = DrawScrnEnv.MyGetTickCount;

                        if (m_MsgList.Count > 0)
                        {
                            sysMsg = (TSysMsg)m_MsgList.GetObject(0);
                            tempTime = DrawScrnEnv.MyGetTickCount - sysMsg.Time;
                            if (tempTime >= nShowTime)
                            {
                                m_boWantDelete = true;
                                m_boWantDeleteTick = DrawScrnEnv.MyGetTickCount;
                                m_nOffsetY = 0;
                                m_dwOffsetTick = DrawScrnEnv.MyGetTickCount;
                            }
                        }
                    }
                    else
                    {
                        if (DrawScrnEnv.MyGetTickCount - m_dwOffsetTick >= nStepTime)
                        {
                            m_nOffsetY++;
                            m_dwOffsetTick = DrawScrnEnv.MyGetTickCount;
                        }
                    }
                }
                else
                {
                    sysMsg = (TSysMsg)m_MsgList.GetObject(0);
                    tempTime = DrawScrnEnv.MyGetTickCount - sysMsg.Time;
                    if (tempTime >= nShowTime)
                    {
                        m_boWantDelete = true;
                        m_boWantDeleteTick = DrawScrnEnv.MyGetTickCount;
                        m_nOffsetY = 0;
                        m_dwOffsetTick = DrawScrnEnv.MyGetTickCount;
                    }
                }

                nShowCount = 0;
                sx = m_nX;

                if (m_boDownToUP)
                    sy = m_nY + m_nOffsetY;
                else
                    sy = m_nY - m_nOffsetY;

                for (int i = 0; i < m_MsgList.Count; i++)
                {
                    sysMsg = (TSysMsg)m_MsgList.GetObject(i);

                    if ((i == 0) && (m_nOffsetY > 0))
                        alpha = 180 - m_nOffsetY * 15;
                    else
                        alpha = 255;

                    if (alpha < 0) alpha = 0;

                    DrawScrnEnv.BoldTextOut(sx, sy, m_MsgList[i], sysMsg.FColor, sysMsg.BColor, alpha);

                    if (m_boDownToUP)
                        sy -= 16; // 经验显示要改成向上滚动 chongchong 2018-05-28
                    else
                        sy += 16;

                    nShowCount++;
                    if (nShowCount > 5) break;
                }
            }
        }
        finally
        {
            m_MsgList.UnLock();
        }
    }

    /// <summary>DrawScrn.pas:658/3028-3036 procedure Clear。</summary>
    public void Clear()
    {
        m_MsgList.Lock();
        try
        {
            m_MsgList.Clear();
        }
        finally
        {
            m_MsgList.UnLock();
        }
    }
}

/// <summary>DrawScrn.pas:661-672 TDrawMoveHintMsg（恒向下累加 16，无 m_boDownToUP 分支）。</summary>
public class TDrawMoveHintMsg
{
    public int m_nX, m_nY;
    public TColor m_FColor, m_BColor;

    public TGStringList m_MsgList;

    /// <summary>DrawScrn.pas:667/3040-3047 constructor Create。</summary>
    public TDrawMoveHintMsg()
    {
        m_MsgList = new TGStringList();
        m_nX = 30;
        m_nY = 40;
        m_FColor = DrawScrnEnv.GetTColor(2);
        m_BColor = DrawScrnEnv.GetTColor(0);
    }

    /// <summary>DrawScrn.pas:668/3049-3057 destructor Destroy; override。</summary>
    public void Free()
    {
        m_MsgList = null;
    }

    /// <summary>DrawScrn.pas:669/3059-3077 procedure Add(sMsg:string; FColor, BColor:Byte)。</summary>
    public void Add(string sMsg, byte fcolor, byte bcolor)
    {
        m_MsgList.Lock();
        try
        {
            if (m_MsgList.Count >= 10)
            {
                m_MsgList.Delete(0);
            }
            var sysMsg = new TSysMsg
            {
                Time = DrawScrnEnv.MyGetTickCount,
                FColor = DrawScrnEnv.GetTColor(fcolor),
                BColor = DrawScrnEnv.GetTColor(bcolor),
            };
            m_MsgList.AddObject(sMsg, sysMsg);
        }
        finally
        {
            m_MsgList.UnLock();
        }
    }

    /// <summary>DrawScrn.pas:670/3079-3103 procedure Draw()。</summary>
    public void Draw()
    {
        int sx, sy;
        TSysMsg sysMsg;
        if (DrawScrnEnv.g_MySelf == null) return;
        m_MsgList.Lock();
        try
        {
            if (m_MsgList.Count > 0)
            {
                sx = m_nX;
                sy = m_nY;
                for (int i = 0; i < m_MsgList.Count; i++)
                {
                    sysMsg = (TSysMsg)m_MsgList.GetObject(i);
                    DrawScrnEnv.BoldTextOut(sx, sy, m_MsgList[i], sysMsg.FColor, sysMsg.BColor);
                    sy += 16;
                }
                if (DrawScrnEnv.MyGetTickCount - ((TSysMsg)m_MsgList.GetObject(0)).Time >= 3000)
                {
                    m_MsgList.Delete(0);
                }
            }
        }
        finally
        {
            m_MsgList.UnLock();
        }
    }

    /// <summary>DrawScrn.pas:671/3105-3113 procedure Clear。</summary>
    public void Clear()
    {
        m_MsgList.Lock();
        try
        {
            m_MsgList.Clear();
        }
        finally
        {
            m_MsgList.UnLock();
        }
    }
}
