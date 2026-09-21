using System;
using System.Collections.Generic;

namespace GXX.Client.Scenes;

/// <summary>
/// **车道 `p17-client-actor`**：`Actor.pas` 的 <c>THeroActor</c>（**17437-17533，6 条实现**）1:1。
///
/// <para><b>★ 空壳迁移</b>：原先只是 <c>PlaySceneNewActor.cs:493-497</c> 的一行空壳
/// （<c>public class THeroActor : TActor { public override string ActorClass =&gt; "THeroActor"; }</c>）。
/// 本车道把类声明移出并在本文件写真实现。</para>
///
/// <para><b>★ 基类关系必须更正</b>：原文层级是 <c>THeroActor : THumActor</c>（2049），
/// 而 <c>PlaySceneNewActor.cs</c> 的空壳写的是 <c>: TActor</c> —— 这是 p12 复核指出的
/// <b>"同名但层级错"的静默缺陷形态</b>：<c>THumActor</c> 的
/// <c>CalcActorFrame</c>/<c>GetDefaultFrame</c>/<c>LoadSurface</c>/<c>DrawChr</c>/<c>Run</c>
/// 覆写会被整段绕过（英雄退化成普通怪）。本类据此声明为 <c>: THumActor</c>。</para>
///
/// <para><b>字段来源</b>：<c>m_nBagCount</c>(2050) / <c>m_nAngryValue</c>(2051) /
/// <c>m_nMaxAngryValue</c>(2052) 已在 <c>ActorNpcEnv.cs</c> 的 <c>TActorCore</c> 扩展里声明。</para>
///
/// <para><b>这 6 条都不是虚覆写</b>：原文声明（2054-2059）全是**普通 public 方法**
/// （<c>FindGroupMagic</c> / <c>GetGroupMagicId</c> / <c>GroupAttack</c> / <c>Rest</c> /
/// <c>Protect</c> / <c>Target</c>），故托管侧**不加 <c>virtual</c>/<c>override</c>**
/// —— 加了会凭空造出原文没有的多态点（台帐 §18.8 的反向形态）。</para>
/// </summary>
public partial class THeroActor : THumActor
{
    /// <summary>类名（原文无此成员；迁移前 <c>PlaySceneNewActor.cs:496</c> 空壳上的原值，逐字保留）。</summary>
    public override string ActorClass => "THeroActor";

    /// <summary>
    /// `THeroActor.FindGroupMagic`（**17437-17456，19 行**）1:1。
    ///
    /// <para><b>流程</b>：<c>Result := nil</c> → 锁 <c>g_HeroMagicList</c> →
    /// 从 0 到 <c>Count - 1</c> **线性查找首个** <c>pm.Def.wMagicId = GetGroupMagicId</c> → 命中即 <c>Break</c> →
    /// 解锁 → 返回。</para>
    ///
    /// <para><b>★ <c>GetGroupMagicId</c> 在循环条件里被调用 N 次</b>（每个元素一次），
    /// 不是循环外只算一次 —— 原文如此。因为 <c>GetGroupMagicId</c> 只读
    /// <c>g_MySelf.m_btJob</c> 与 <c>m_btJob</c>，循环内不变，故结果等价；
    /// 但若把 <c>GetGroupMagicId</c> 提到循环外，就与原文的**副作用次数**不同了
    /// （本实现逐字保留"每次比较都调一次"）。</para>
    ///
    /// <para><b>接缝</b>：<see cref="ActorNpcEnv.HeroMagicIdListFn"/> 承载
    /// <c>g_HeroMagicList</c> 的 <c>Def.wMagicId</c> 序列；<c>Lock/UnLock</c> 对 headless
    /// 无对应物（无共享容器），逐字略。</para>
    ///
    /// <para><b>返回类型说明</b>：原文返回 <c>pTClientMagic</c>（指向 <c>g_HeroMagicList</c> 元素的指针）。
    /// headless 侧不持有该列表本体，只持有 <c>Def.wMagicId</c> 序列，故返回
    /// <c>int?</c>（命中的 <c>wMagicId</c>；<c>null</c> = 原文的 <c>Result := nil</c>）。
    /// <b>这是已登记的偏离 D-P17-01</b>：调用方拿不到整条 <c>TClientMagic</c>。</para>
    /// </summary>
    public int? FindGroupMagic()
    {
        int? result = null;                                               // 17442
        IReadOnlyList<int> list = ActorNpcEnv.HeroMagicIdListFn();
        int targetMagicId = GetGroupMagicId();

        for (int i = 0; i <= list.Count - 1; i++)                          // 17445
        {
            if (list[i] == targetMagicId)                                 // 17447
            {
                result = list[i];                                         // 17448
                break;                                                    // 17449
            }
        }

        return result;                                                    // 17456
    }

    /// <summary>
    /// `THeroActor.GetGroupMagicId`（**17458-17484，27 行**）1:1。
    ///
    /// <para><b>查表（行 × 列，行为主角职业，列为英雄职业）</b>：</para>
    /// <code>
    /// g_MySelf.m_btJob \ m_btJob |   0 |   1 |   2
    /// ---------------------------+-----+-----+-----
    ///   0                        |  60 |  62 |  61
    ///   1                        |  62 |  65 |  64
    ///   2                        |  61 |  64 |  63
    /// </code>
    ///
    /// <para><b>★ 原文没有 <c>else</c></b>：<c>g_MySelf.m_btJob</c> 或 <c>m_btJob</c> 落在
    /// 0..2 之外时 <c>Result</c> **保持 0**（17460 的初值）。逐字保留，见测试
    /// <c>GetGroupMagicId_OutOfRangeJobsStayZero</c>。</para>
    ///
    /// <para><b>注意表的对称性</b>：<c>(0,1)=62</c> 与 <c>(1,0)=62</c> 对称，
    /// 但 <c>(0,0)=60</c> 与 <c>(1,1)=65</c>、<c>(2,2)=63</c> **不在对角线上相同** ——
    /// 用"对称矩阵"或"取大/取小"的直觉简化会错。</para>
    /// </summary>
    public int GetGroupMagicId()
    {
        int result = 0;                                                   // 17460
        switch (ActorNpcEnv.MySelfJobFn())                                // 17461
        {
            case 0:
                switch (m_btJob)
                {
                    case 0: result = 60; break;                           // 17464
                    case 1: result = 62; break;                           // 17465
                    case 2: result = 61; break;                           // 17466
                }
                break;
            case 1:
                switch (m_btJob)
                {
                    case 0: result = 62; break;                           // 17471
                    case 1: result = 65; break;                           // 17472
                    case 2: result = 64; break;                           // 17473
                }
                break;
            case 2:
                switch (m_btJob)
                {
                    case 0: result = 61; break;                           // 17478
                    case 1: result = 64; break;                           // 17479
                    case 2: result = 63; break;                           // 17480
                }
                break;
        }

        return result;                                                    // 17484
    }

    /// <summary>
    /// `THeroActor.GroupAttack`（**17486-17495，9 行**）1:1。
    ///
    /// <para><b>★ 副作用顺序</b>：17491 先把 <c>g_dwLatestSpellTick := MyGetTickCount</c>
    /// **无条件**写好，17493 才 <c>SendClientMessage(CM_HEROGROUPATTACK, 0, 0, 0, 0)</c>。
    /// 原文注释（17490）说明了原因：使用道法合击技能时快速按 F1 使用灵魂火符会卡技能
    /// —— 该打点就是为压制这个竞态（chongchong 2015-09-14）。故**不可**把它挪到 if 之外或删掉。</para>
    ///
    /// <para>17488 的调试 <c>AddChatBoardString</c> 被原文注释，逐字不移植。</para>
    /// </summary>
    public void GroupAttack()
    {
        if (m_nMaxAngryValue > 0 && m_nAngryValue >= m_nMaxAngryValue)     // 17489
        {
            ActorNpcEnv.SetLatestSpellTickFn(ActorNpcEnv.MyGetTickCountFn());   // 17491

            ActorNpcEnv.SendClientMessageFn(CM_HEROGROUPATTACK, 0, 0, 0, 0);    // 17493
        }
    }

    /// <summary>
    /// `THeroActor.Rest`（**17497-17500**）1:1：**只有** <c>frmMain.SendSay('@RestHero')</c> 一句
    /// （没有条件、没有本地状态修改）。
    /// </summary>
    public void Rest()
    {
        ActorNpcEnv.SendSayFn("@RestHero");                               // 17499
    }

    /// <summary>
    /// `THeroActor.Protect`（**17502-17505**）1:1：<c>SendClientMessage(CM_HEROPROTECT, 0, g_nMouseCurrX, g_nMouseCurrY, 0)</c>
    /// —— 第二个实参是 <b>0</b>（不是目标 recog），坐标取**鼠标当前位置**。
    /// </summary>
    public void Protect()
    {
        ActorNpcEnv.SendClientMessageFn(CM_HEROPROTECT, 0,
            ActorNpcEnv.MouseCurrXFn(), ActorNpcEnv.MouseCurrYFn(), 0);   // 17504
    }

    /// <summary>
    /// `THeroActor.Target`（**17507-17533，26 行**）1:1。
    ///
    /// <para><b>★ 17512-17519：先在锁内把两个全局引用**快照到局部**</b>
    /// （<c>TargetCret</c> / <c>FocusCret</c>），再在锁外判。这是原文刻意的并发保护；
    /// 直接读全局两次会拿到不同的值。</para>
    ///
    /// <para><b>分支语义（注意 <c>or</c> 的**非**德摩根形态）</b>：</para>
    /// <list type="bullet">
    /// <item>17521 的条件是
    ///   <c>(TargetCret = nil) or ((TargetCret &lt;&gt; nil) and IsValidActor(TargetCret) and TargetCret.m_boDeath)</c>
    ///   —— 即"没有目标" **或** "目标存在且**有效**且**已死**"。
    ///   ★ 若目标存在但 <c>IsValidActor</c> 为假（已从 ActorList 移除），**整个条件为假**，
    ///   于是走 17529 的 else 分支 —— 而 else 里 **不判 <c>m_boDeath</c>**
    ///   （只判 <c>&lt;&gt; nil</c> 与 <c>IsValidActor</c>）。这个不对称是原文如此。</item>
    /// <item>17521 为真时：焦点存在且有效且**未死** ⇒ 发焦点坐标；否则 ⇒ 发鼠标坐标 + recog=0。</item>
    /// <item>17529 为假时（else）：焦点存在且有效 ⇒ 发焦点坐标；**没有 else**，都不满足则什么都不发。</item>
    /// </list>
    /// </summary>
    public void Target()
    {
        // 17512-17519：锁内快照
        TActor? targetCret = ActorNpcEnv.TargetCretFn();                  // 17515
        TActor? focusCret = ActorNpcEnv.FocusCretFn();                    // 17516

        if (targetCret == null
            || (targetCret != null && ActorNpcEnv.IsValidActorFn(targetCret) && targetCret.m_boDeath))   // 17521
        {
            if (focusCret != null && ActorNpcEnv.IsValidActorFn(focusCret) && !focusCret.m_boDeath)       // 17522
            {
                ActorNpcEnv.SendClientMessageFn(CM_HEROTARGET,
                    focusCret.m_nRecogId, focusCret.m_nCurrX, focusCret.m_nCurrY, 0);                     // 17523
            }
            else
            {
                ActorNpcEnv.SendClientMessageFn(CM_HEROTARGET, 0,
                    ActorNpcEnv.MouseCurrXFn(), ActorNpcEnv.MouseCurrYFn(), 0);                           // 17526
            }
        }
        else
        {
            if (focusCret != null && ActorNpcEnv.IsValidActorFn(focusCret))                                // 17530
            {
                ActorNpcEnv.SendClientMessageFn(CM_HEROTARGET,
                    focusCret.m_nRecogId, focusCret.m_nCurrX, focusCret.m_nCurrY, 0);                     // 17531
            }
        }
    }

    // ──────────────────────────────────────────────────────────────────────
    // 常量（Grobal2.pas：CM_* 客户端命令码，实值逐条核对）
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>`CM_HEROTARGET = 158`（Common/Grobal2.pas:418）。</summary>
    public const int CM_HEROTARGET = 158;

    /// <summary>`CM_HEROGROUPATTACK = 160`（Common/Grobal2.pas:420）。</summary>
    public const int CM_HEROGROUPATTACK = 160;

    /// <summary>`CM_HEROPROTECT = 162`（Common/Grobal2.pas:422）。</summary>
    public const int CM_HEROPROTECT = 162;
}
