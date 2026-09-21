// ============================================================================================
// 车道 `p10-client-scrn`：DrawScrn.pas 的 `TDrawScreen`（声明 674-733 / 实现 4374-4899、5537-5540）
// 1:1 移植。
//
// ★★★ 与 `Scenes/Scenes.cs:694 public class TDrawScreen` 的关系（本车道任务书第 2 条）★★★
//
// 「逐成员对照」实测（`Scenes.cs:693-743`）：
//   | Scenes.cs 成员                        | DrawScrn.pas 出处        | 判定 |
//   |---|---|---|
//   | `CurrentScene`                        | 690 `CurrentScene:TScene` | 重叠 |
//   | `ChangeScene(TSceneType)`             | 4465-4506                 | 重叠（但 Scenes 版**多了随机码弹窗**且**少了临界区**，见下） |
//   | `KeyPress(ref char)`                  | 4441-4445                 | 重叠 |
//   | `KeyDown(ref ushort)`                 | 4447-4451                 | 重叠（Scenes 版传 `new object()` 当 Shift，**丢弃了 TShiftState**） |
//   | `WelcomeScene/LoginScene/SelectChrScene/LoginNoticeScene/PlayScene` | **不在 DrawScrn.pas 的 TDrawScreen 里** | ⚠ 越界：原文这 5 个是 **MShare.pas:1457-1461 的单元级全局** |
//   | `ShowLoginSceneShowRandomCodeDlg` / `OpenRandomCodeDlg` | 683 `m_boShowLoginSceneShowRandomCodeDlg`；`FrmDlg.OpenDRandomCodeDlg` | ⚠ 形态偏离（原文是私有字段 + FrmDlg 调用） |
//   其余 **24 个成员**（m_dwFrameTime / m_dwFrameCount / m_SysMsgList / m_SysMsgListEx /
//   m_boInitialize / FScreen*MsgList / FMoveHintMsgList / HintList / HintX..HintHeight / HintUp /
//   HintColor / DrawDelayMsg / DrawScreenCenterMsg / Create / Destroy / Initialize / Finalize /
//   Update / MouseMove / MouseDown / AddSysMsg / AddChatBoardString / AddTopChatBoardString /
//   ClearChatBoard / AddMoveMsg / AddNewMoveMsg / AddNewLineMsg / AddMoveHintMsg / ShowHint /
//   ClearHint / DrawScreen / DrawMsg_TopLevel / DrawSysMsg_BottomLevel / DrawHint / DrawMove /
//   DrawMoveBefor / SetShowLoginSceneShowRandomCodeDlg）**在 Scenes.cs 里全无声明**。
//
// ⇒ 结论：**是同一份**（不是两份），但 `Scenes.cs` 那一份是**早期车道落的部分成员**。
//   本车道按任务书「若 TDrawScreen 不是 partial 则在报告里登记」执行 ——
//   **不改 `Scenes/Scenes.cs`**（越区），改为：
//     · 本文件给出**完整、自包含**的 1:1 实现 `TDrawScreenScrn`；
//     · D-P10-01 登记**最小改法**：给 `Scenes.cs:694` 加 `partial`，删除其中与
//       `CurrentScene`/`ChangeScene`/`KeyPress`/`KeyDown` 重叠的 4 个成员，
//       把本文件的字段与方法**整块搬入**并把类名 `TDrawScreenScrn` 改回 `TDrawScreen`，
//       方法体**一行都不用改**。
//   `TDrawScreen` 与 `TDrawScreenScrn` 名称不冲突 ⇒ 当前门禁可绿。
//
// 场景全局（WelcomeScene/LoginScene/…）本实现按原文改用 **MShare.pas:1457-1461 的单元级全局**
// （`DrawScrnEnv.WelcomeScene` …），这正是 Scenes.cs 版与原文的偏离点之一。
// ============================================================================================

using System;
using GXX.Client.GUI.DxComponent;
using GXX.Client.GUI.Mir;
using GXX.Client.GUI.Share;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.Core.Util;
using TRect = GXX.Client.GUI.DxComponent.TRect;
using TGStringList = GXX.Core.Protocol.SDK.TGStringList;

namespace GXX.Client.Scenes;

/// <summary>
/// DrawScrn.pas:674-733 `TDrawScreen`（场景调度 + 屏幕提示/飘字绘制总控）。
/// <para>★ 类名说明：为避开 `Scenes.cs:694` 的既有部分声明（未加 `partial`，本车道越区不可改），
/// 本实现落在 `TDrawScreenScrn`；见文件头 D-P10-01 的合并说明。</para>
/// </summary>
public class TDrawScreenScrn
{
    public uint m_dwFrameTime;
    public uint m_dwFrameCount;

    public TGStringList m_SysMsgList;
    public TGStringList m_SysMsgListEx;
    public bool m_boInitialize;

    public bool m_boShowLoginSceneShowRandomCodeDlg;

    public TScreenMoveMsgList FScreenMoveMsgList; // 屏幕滚动消息链
    public TScreenNewMoveMsgList FScreenNewMoveMsgList;
    public TScreenNewLineMsgList FScreenNewLineMsgList;
    public TMoveHintMsgList FMoveHintMsgList;

    public TScene CurrentScene;

    public TStringList HintList;
    public int HintX, HintY, HintWidth, HintHeight;
    public bool HintUp;
    public TColor HintColor;
    public TDrawDelayMsg DrawDelayMsg;
    public TDrawScreenCenterMsg DrawScreenCenterMsg;

    /// <summary>DrawScrn.pas:699/4374-4393 constructor Create。</summary>
    public TDrawScreenScrn()
    {
        CurrentScene = null;
        m_dwFrameTime = DrawScrnEnv.MyGetTickCount;
        m_dwFrameCount = 0;
        m_SysMsgList = new TGStringList();
        m_SysMsgListEx = new TGStringList();

        HintList = new TStringList();
        FScreenMoveMsgList = new TScreenMoveMsgList();
        FScreenNewMoveMsgList = new TScreenNewMoveMsgList();
        FScreenNewLineMsgList = new TScreenNewLineMsgList();

        DrawScreenCenterMsg = new TDrawScreenCenterMsg();
        DrawDelayMsg = new TDrawDelayMsg();

        FMoveHintMsgList = new TMoveHintMsgList();

        m_boShowLoginSceneShowRandomCodeDlg = false;
    }

    /// <summary>DrawScrn.pas:700/4395-4418 destructor Destroy; override。</summary>
    public void Free()
    {
        for (int i = 0; i < m_SysMsgList.Count; i++)
        {
            ((TDrawSysMsg)m_SysMsgList.GetObject(i)).Free();
        }
        m_SysMsgList = null;

        for (int i = 0; i < m_SysMsgListEx.Count; i++)
        {
            ((TDrawSysMsgEx)m_SysMsgListEx.GetObject(i)).Free();
        }
        m_SysMsgListEx = null;

        HintList = null;
        DrawScreenCenterMsg.Free();
        DrawDelayMsg.Free();
        FScreenMoveMsgList.Free();
        FScreenNewMoveMsgList.Free();
        FScreenNewLineMsgList.Free();

        FMoveHintMsgList.Free();
    }

    /// <summary>DrawScrn.pas:708/4420-4427 procedure Update。</summary>
    public void Update()
    {
        FScreenMoveMsgList.Update();
        FScreenNewMoveMsgList.Update();
        FScreenNewLineMsgList.Update();
        DrawScrnEnv.HintWindows.UpDate();
        FMoveHintMsgList.Update();
    }

    /// <summary>DrawScrn.pas:706/4429-4433 procedure Initialize。</summary>
    public void Initialize()
    {
        FScreenMoveMsgList.Initialize();
        m_boInitialize = true;
    }

    /// <summary>DrawScrn.pas:707/4435-4439 procedure Finalize。</summary>
    public void Finalize_()
    {
        m_boInitialize = false;
        FScreenMoveMsgList.Finalize_();
    }

    /// <summary>DrawScrn.pas:701/4441-4445 procedure KeyPress(var Key:Char)。</summary>
    public void KeyPress(ref char key)
    {
        if (CurrentScene != null)
            CurrentScene.KeyPress(ref key);
    }

    /// <summary>DrawScrn.pas:702/4447-4451 procedure KeyDown(var Key:Word; Shift:TShiftState)。</summary>
    public void KeyDown(ref ushort key, TShiftState shift)
    {
        if (CurrentScene != null)
            CurrentScene.KeyDown(ref key, shift);
    }

    /// <summary>DrawScrn.pas:703/4453-4457 procedure MouseMove(Shift:TShiftState; X, Y:Integer)。</summary>
    public void MouseMove(TShiftState shift, int x, int y)
    {
        if (CurrentScene != null)
            CurrentScene.MouseMove(shift, x, y);
    }

    /// <summary>DrawScrn.pas:704/4459-4463 procedure MouseDown(Button:TMouseButton; Shift:TShiftState; X, Y:Integer)。</summary>
    public void MouseDown(TMouseButton button, TShiftState shift, int x, int y)
    {
        if (CurrentScene != null)
            CurrentScene.MouseDown((int)button, shift, x, y);
    }

    /// <summary>DrawScrn.pas:709/4465-4506 procedure ChangeScene(SceneType:TSceneType)。</summary>
    public void ChangeScene(TSceneType sceneType)
    {
        // 原文 {$IF IsMultiThreadRender = 1} EnterCriticalSection(g_CriticalSection) ... {$IFEND}
        // —— 该条件编译符号在本工程未定义（见 TaskLog/编译配置），故按"未定义分支"移植：不加临界区。
        if (CurrentScene != null)
            CurrentScene.CloseScene();
        switch (sceneType)
        {
            case TSceneType.stWelcome:
                CurrentScene = DrawScrnEnv.WelcomeScene;
                break;
            case TSceneType.stLogin:
                CurrentScene = DrawScrnEnv.LoginScene;
                break;
            case TSceneType.stSelectCountry:
                break;
            case TSceneType.stSelectChr:
                {
                    // if g_UpdateEngine <> nil then g_UpdateEngine.ClearRequestList;
                    CurrentScene = DrawScrnEnv.SelectChrScene;
                }
                break;
            case TSceneType.stNewChr:
                break;
            case TSceneType.stLoading:
                {
                    // if g_UpdateEngine <> nil then g_UpdateEngine.ClearRequestList;
                }
                break;
            case TSceneType.stLoginNotice:
                {
                    // if g_UpdateEngine <> nil then g_UpdateEngine.ClearRequestList;
                    CurrentScene = DrawScrnEnv.LoginNoticeScene;
                }
                break;
            case TSceneType.stPlayGame:
                {
                    // if g_UpdateEngine <> nil then g_UpdateEngine.ClearRequestList;
                    CurrentScene = DrawScrnEnv.PlayScene;
                }
                break;
        }

        if (CurrentScene != null)
        {
            CurrentScene.OpenScene();
            if ((CurrentScene == DrawScrnEnv.LoginScene) && (m_boShowLoginSceneShowRandomCodeDlg))
            {
                DrawScrnEnv.OpenDRandomCodeDlg();
            }
        }
    }

    /// <summary>DrawScrn.pas:711/4508-4536 procedure AddSysMsg(Msg:string; FColor, Bcolor:Byte; X, Y:Integer; boDownToUP:Boolean; boDrawBottom:Boolean)。</summary>
    public void AddSysMsg(string msg, byte fcolor, byte bcolor, int x, int y, bool boDownToUP = false, bool boDrawBottom = false)
    {
        TDrawSysMsg drawSysMsg;
        TDrawSysMsg curDrawSysMsg;
        m_SysMsgList.Lock();
        try
        {
            curDrawSysMsg = null;
            for (int i = 0; i < m_SysMsgList.Count; i++)
            {
                drawSysMsg = (TDrawSysMsg)m_SysMsgList.GetObject(i);
                if ((drawSysMsg.m_nX == x) && (drawSysMsg.m_nY == y) &&
                    (drawSysMsg.m_boDownToUP == boDownToUP) && (drawSysMsg.m_boDrawBottom == boDrawBottom))
                {
                    curDrawSysMsg = drawSysMsg;
                    break;
                }
            }
            if (curDrawSysMsg == null)
            {
                curDrawSysMsg = new TDrawSysMsg();
                curDrawSysMsg.m_nX = x;
                curDrawSysMsg.m_nY = y;
                curDrawSysMsg.m_boDownToUP = boDownToUP;
                curDrawSysMsg.m_boDrawBottom = boDrawBottom;
                m_SysMsgList.AddObject("", curDrawSysMsg);
            }
            curDrawSysMsg.Add(msg, fcolor, bcolor);
        }
        finally
        {
            m_SysMsgList.UnLock();
        }
    }

    // 原文 4538-4600 的 TDrawScreen.AddSysMsg / AddSysMsgEx 旧版整段被 (* *) 注释掉 ⇒ 不移植。

    /// <summary>DrawScrn.pas:714/4602-4605 procedure AddChatBoardString(Str:string; FColor, Bcolor:Integer)。</summary>
    public void AddChatBoardString(string str, int fcolor, int bcolor)
    {
        DrawScrnEnv.DChatMemo.Add(str, fcolor, bcolor);
    }

    /// <summary>DrawScrn.pas:715/4607-4610 procedure AddTopChatBoardString(Str:string; FColor, BColor, TimeOut:Integer)。</summary>
    public void AddTopChatBoardString(string str, int fcolor, int bcolor, int timeOut)
    {
        DrawScrnEnv.DChatMemo.AddTop(str, fcolor, bcolor, timeOut);
    }

    /// <summary>DrawScrn.pas:717/4612-4615 procedure AddMoveMsg(...)。</summary>
    public void AddMoveMsg(string sMsg, byte fcolor, byte bcolor, int nY, int nCount,
        bool boShowFrame = true, byte btFrameColor = 190, byte btFontSize = 11,
        bool boFontBold = true, int nMarqueeTime = 60)
    {
        FScreenMoveMsgList.Add(sMsg, fcolor, bcolor, nY, nCount, boShowFrame, btFrameColor, btFontSize, boFontBold, nMarqueeTime);
    }

    /// <summary>DrawScrn.pas:718/4617-4620 procedure AddNewMoveMsg(...)。</summary>
    public void AddNewMoveMsg(string sMsg, byte fcolor, byte bcolor, byte fontSize, int nX, int nY, int nCount)
    {
        FScreenNewMoveMsgList.Add(sMsg, fcolor, bcolor, fontSize, nX, nY, nCount);
    }

    /// <summary>DrawScrn.pas:719/4622-4625 procedure AddNewLineMsg(...)。</summary>
    public void AddNewLineMsg(string sMsg, byte fcolor, byte bcolor, byte fontSize, int nX, int nY, int nTime, int nDrawType)
    {
        FScreenNewLineMsgList.Add(sMsg, fcolor, bcolor, fontSize, nX, nY, nTime, nDrawType);
    }

    /// <summary>DrawScrn.pas:720/4627-4630 procedure AddMoveHintMsg(...)。</summary>
    public void AddMoveHintMsg(string sMsg, byte fcolor, byte bcolor, int nX, int nY)
    {
        FMoveHintMsgList.Add(sMsg, fcolor, bcolor, nX, nY);
    }

    /// <summary>DrawScrn.pas:722/4632-4654 procedure ShowHint(X, Y:Integer; Msg:string; Color:TColor; DrawUp, DrawLeft, ShowBackground:Boolean)。</summary>
    public void ShowHint(int x, int y, string msg, TColor color, bool drawUp, bool drawLeft, bool showBackground)
    {
        // ClearHint;
        DrawScrnEnv.HintWindows.Show(x, y, msg, color, drawUp, drawLeft, showBackground);

        // 原文 4637-4653 的旧实现整段被 { } 注释掉 ⇒ 不移植。
    }

    /// <summary>DrawScrn.pas:723/4656-4659 procedure ClearHint。</summary>
    public void ClearHint()
    {
        HintList.Clear();
    }

    /// <summary>DrawScrn.pas:716/4661-4691 procedure ClearChatBoard。</summary>
    public void ClearChatBoard()
    {
        FScreenMoveMsgList.Clear();
        FScreenNewMoveMsgList.Clear();
        FScreenNewLineMsgList.Clear();

        DrawScreenCenterMsg.Clear();
        DrawDelayMsg.Clear();

        m_SysMsgList.Lock();
        try
        {
            for (int i = 0; i < m_SysMsgList.Count; i++)
            {
                ((TDrawSysMsg)m_SysMsgList.GetObject(i)).Free();
            }
            m_SysMsgList.Clear();
        }
        finally
        {
            m_SysMsgList.UnLock();
        }

        m_SysMsgListEx.Lock();
        try
        {
            for (int i = 0; i < m_SysMsgListEx.Count; i++)
            {
                ((TDrawSysMsgEx)m_SysMsgListEx.GetObject(i)).Free();
            }
            m_SysMsgListEx.Clear();
        }
        finally
        {
            m_SysMsgListEx.UnLock();
        }
    }

    /// <summary>DrawScrn.pas:725/4693-4818 procedure DrawScreen()（地图右上角区域标记 + 左上角绿色信息）。</summary>
    public void DrawScreen()
    {
        int k;
        string str;
        string s1;
        TTexture d;
        TActor mySelf;
        TActor focusCret;
        TActor myHero;
        bool boShowNumber;
        str = "";
        int line = 1;
        if (DrawScrnEnv.g_MySelf != null)
        {
            k = 0;

            for (int i = 0; i <= 15; i++)
            {
                // 地图右上角战斗，安全标记 显示错误 ($01 shr I) 改为 ($01 shl I) chongchong 2014-01-07
                if ((DrawScrnEnv.g_nAreaStateValue & (0x01 << i)) != 0)
                {
                    d = MShareGlobals.g_WMainImages[DrawScrnConst.AREASTATEICONBASE + i];
                    if (d != null)
                    {
                        k = k + d.Width;
                        DrawScrnEnv.GameCanvas.Draw(DrawScrnEnv.SCREENWIDTH - k, 0, d.ClientRect, d);
                    }
                }
            }

            if (DrawScrnEnv.PlugInEnabled && DrawScrnEnv.boShowGreenHint && DrawScrnEnv.ConfigCheckeds_ckShowGreenHint)
            {
                // 原文 {$IF IsMultiThreadRender = 1} EnterCriticalSection(g_ActorLock) {$IFEND}：未定义分支
                mySelf = DrawScrnEnv.g_MySelf;
                focusCret = DrawScrnEnv.g_FocusCret;
                myHero = DrawScrnEnv.g_MyHero;
                if ((mySelf != null) && (!DrawScrnEnv.IsValidActorEx(mySelf)))
                {
                    mySelf = null;
                }
                if ((focusCret != null) && (!DrawScrnEnv.IsValidActorEx(focusCret)))
                {
                    focusCret = null;
                }
                if ((myHero != null) && (!DrawScrnEnv.IsValidActorEx(myHero)))
                {
                    myHero = null;
                }

                if (mySelf != null)
                {
                    if (!DrawScrnEnv.boGreenHintNewStyle)
                    {
                        str =
                            "等级: " + DelphiRTL.IntToStr((int)mySelf.m_Abil.Level) + " 经验(" + DelphiRTL.IntToStr((int)mySelf.m_Abil.Exp) + "/" + DelphiRTL.IntToStr((int)mySelf.m_Abil.MaxExp) + ")" +
                            " 负重: " + DelphiRTL.IntToStr(mySelf.m_Abil.Weight) + "/" + DelphiRTL.IntToStr(mySelf.m_Abil.MaxWeight) +
                            " " + DrawScrnEnv.g_sGoldName + ": " + DelphiRTL.IntToStr(ActorUiFields.GetGold(mySelf)) +
                            " " + DrawScrnEnv.g_sGameGoldName + ": " + DelphiRTL.IntToStr(ActorUiFields.GetGameGold(mySelf)) +
                            " 鼠标: " + DelphiRTL.IntToStr(DrawScrnEnv.g_nMouseCurrX) + ":" + DelphiRTL.IntToStr(DrawScrnEnv.g_nMouseCurrY) + "(" + DelphiRTL.IntToStr(DrawScrnEnv.g_nMouseX) + ":" + DelphiRTL.IntToStr(DrawScrnEnv.g_nMouseY) + ")";
                    }
                    else
                    {
                        str =
                            " 防御: " + DelphiRTL.IntToStr(DrawScrnEnv.g_MySelf.m_Abil.AC1) + "-" + DelphiRTL.IntToStr(DrawScrnEnv.g_MySelf.m_Abil.AC2) +
                            " 魔防: " + DelphiRTL.IntToStr(DrawScrnEnv.g_MySelf.m_Abil.MAC1) + "-" + DelphiRTL.IntToStr(DrawScrnEnv.g_MySelf.m_Abil.MAC2) +
                            " 攻击: " + DelphiRTL.IntToStr(DrawScrnEnv.g_MySelf.m_Abil.DC1) + "-" + DelphiRTL.IntToStr(DrawScrnEnv.g_MySelf.m_Abil.DC2) +
                            " 魔法: " + DelphiRTL.IntToStr(DrawScrnEnv.g_MySelf.m_Abil.MC1) + "-" + DelphiRTL.IntToStr(DrawScrnEnv.g_MySelf.m_Abil.MC2) +
                            " 道术: " + DelphiRTL.IntToStr(DrawScrnEnv.g_MySelf.m_Abil.SC1) + "-" + DelphiRTL.IntToStr(DrawScrnEnv.g_MySelf.m_Abil.SC2) +
                            " 鼠标: " + DelphiRTL.IntToStr(DrawScrnEnv.g_nMouseCurrX) + ":" + DelphiRTL.IntToStr(DrawScrnEnv.g_nMouseCurrY) + "(" + DelphiRTL.IntToStr(DrawScrnEnv.g_nMouseX) + ":" + DelphiRTL.IntToStr(DrawScrnEnv.g_nMouseY) + ")";
                    }
                }

                if (focusCret != null)
                {
                    boShowNumber = false;
                    if (focusCret.m_btRace is ActorLabelConsts.RC_PLAYOBJECT or ActorLabelConsts.RC_HEROOBJECT)
                    {
                        if ((DrawScrnEnv.boHumStruckShowNumber && focusCret.m_boStruckShowNumber) || (!DrawScrnEnv.boHumStruckShowNumber) || (focusCret == DrawScrnEnv.g_MySelf) || focusCret.m_boOpenHealth)
                        {
                            if (focusCret.m_Abil.MaxHP > 0)
                            {
                                boShowNumber = true;
                            }
                        }
                    }
                    else
                    {
                        if ((DrawScrnEnv.boMonStruckShowNumber && focusCret.m_boStruckShowNumber) || (!DrawScrnEnv.boMonStruckShowNumber) || focusCret.m_boOpenHealth)
                        {
                            if ((focusCret.m_btRace != ActorLabelConsts.RC_MERCHANT) && (focusCret.m_Abil.MaxHP > 0))
                            {
                                boShowNumber = true;
                            }
                        }
                    }

                    if (boShowNumber)
                        s1 = "目标: " + focusCret.m_sUserName + "(" + DelphiRTL.IntToStr((int)focusCret.m_Abil.HP) + "/" + DelphiRTL.IntToStr((int)focusCret.m_Abil.MaxHP) + ")";
                    else
                        s1 = "目标: " + focusCret.m_sUserName + "(0/0)";
                }
                else
                {
                    s1 = "目标: -/-";
                }

                if (DrawScrnEnv.TextWidth(DrawScrnEnv.CurrentFont, str + s1 + " ") > DrawScrnEnv.SCREENWIDTH)
                {
                    str = str + "\r" + s1;
                    line++;
                }
                else
                    str = str + " " + s1;

                if (myHero != null)
                {
                    s1 = DelphiRTL.Format("我的英雄: %s(%d/%d)", myHero.m_sUserName, myHero.m_nCurrX, myHero.m_nCurrY);
                    if (DelphiRTL.Pos("\r", str) <= 0)
                    {
                        if (DrawScrnEnv.TextWidth(DrawScrnEnv.CurrentFont, str + s1 + " ") > DrawScrnEnv.SCREENWIDTH)
                        {
                            str = str + "\r" + s1;
                            line++;
                        }
                        else
                            str = str + " " + s1;
                    }
                    else
                        str = str + " " + s1;
                }
                DrawScrnEnv.BoldTextOut(2, 0, str, DrawScrnEnv.clLime);
            }

            if ((DrawScrnEnv.g_nAreaStateValue & 0x04) != 0)
            {
                if (str.Length > 0)
                {
                    DrawScrnEnv.BoldTextOut(2, DrawScrnEnv.g_CurrentFontHeight * line, "攻城区域", TColor.clWhite);
                }
                else
                    DrawScrnEnv.BoldTextOut(2, 0, "攻城区域", TColor.clWhite);
            }
        }
    }

    /// <summary>DrawScrn.pas:726/4822-4854 procedure DrawMsg_TopLevel()（上层绘制 2018-09-27 18:40:19）。</summary>
    public void DrawMsg_TopLevel()
    {
        TDrawSysMsg drawSysMsg;
        if ((!m_boInitialize) || (DrawScrnEnv.g_MySelf == null)) return;
        if (CurrentScene == DrawScrnEnv.PlayScene)
        {
            m_SysMsgList.Lock();
            try
            {
                for (int i = 0; i < m_SysMsgList.Count; i++)
                {
                    drawSysMsg = (TDrawSysMsg)m_SysMsgList.GetObject(i);

                    if (!drawSysMsg.m_boDrawBottom) drawSysMsg.Draw();
                }
            }
            finally
            {
                m_SysMsgList.UnLock();
            }

            m_SysMsgListEx.Lock();
            try
            {
                for (int i = 0; i < m_SysMsgListEx.Count; i++)
                {
                    ((TDrawSysMsgEx)m_SysMsgListEx.GetObject(i)).Draw();
                }
            }
            finally
            {
                m_SysMsgListEx.UnLock();
            }

            DrawDelayMsg.Draw();
            DrawScreenCenterMsg.Draw();

            FMoveHintMsgList.Draw();
        }
    }

    /// <summary>DrawScrn.pas:727/4856-4874 procedure DrawSysMsg_BottomLevel()（下层绘制 获得经验 2018-09-27 18:40:21）。</summary>
    public void DrawSysMsg_BottomLevel()
    {
        TDrawSysMsg drawSysMsg;
        if ((!m_boInitialize) || (DrawScrnEnv.g_MySelf == null)) return;
        if (CurrentScene == DrawScrnEnv.PlayScene)
        {
            m_SysMsgList.Lock();
            try
            {
                for (int i = 0; i < m_SysMsgList.Count; i++)
                {
                    drawSysMsg = (TDrawSysMsg)m_SysMsgList.GetObject(i);

                    if (drawSysMsg.m_boDrawBottom) drawSysMsg.Draw();
                }
            }
            finally
            {
                m_SysMsgList.UnLock();
            }
        }
    }

    /// <summary>DrawScrn.pas:729/4876-4884 procedure DrawMove()。</summary>
    public void DrawMove()
    {
        if ((!m_boInitialize) || (DrawScrnEnv.g_MySelf == null)) return;
        if (CurrentScene == DrawScrnEnv.PlayScene)
        {
            FScreenMoveMsgList.Draw();
            FScreenNewMoveMsgList.Draw();
            FScreenNewLineMsgList.Draw(true);
        }
    }

    /// <summary>DrawScrn.pas:730/4886-4894 procedure DrawMoveBefor()。</summary>
    public void DrawMoveBefor()
    {
        if ((!m_boInitialize) || (DrawScrnEnv.g_MySelf == null)) return;
        if (CurrentScene == DrawScrnEnv.PlayScene)
        {
            //    FScreenMoveMsgList.Draw();
            //    FScreenNewMoveMsgList.Draw();
            FScreenNewLineMsgList.Draw(false);
        }
    }

    /// <summary>DrawScrn.pas:728/4896-4899 procedure DrawHint（原文空体）。</summary>
    public void DrawHint()
    {
    }

    /// <summary>DrawScrn.pas:732/5537-5540 procedure SetShowLoginSceneShowRandomCodeDlg。</summary>
    public void SetShowLoginSceneShowRandomCodeDlg()
    {
        m_boShowLoginSceneShowRandomCodeDlg = true;
    }
}
