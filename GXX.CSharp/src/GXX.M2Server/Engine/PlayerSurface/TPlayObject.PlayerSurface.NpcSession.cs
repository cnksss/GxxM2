// ============================================================================
// 源单元：Source\M2Engine\ObjPlayer.pas（GBK，49,232 LF）
// 本文件：**NPC 会话与脚本标签片**（切片 3 / 4）。
// 覆盖原文行号范围：
//   ObjPlayer.pas:116-118  m_Script: pTScript / m_NPC: TBaseObject / m_ItemBoxNpc: TBaseObject
//   ObjPlayer.pas:148      m_boBreakLoopGoto: Boolean
//   ObjPlayer.pas:150-154  m_sScriptCurrLable / m_sScriptGoBackLable / m_sLastGotoLabel /
//                          m_dwLastGotoLabelTick / m_nOneLabelGotoCount
//   ObjPlayer.pas:196      m_QuestFlag: TQuestFlag   (Grobal2.pas:4116 `array[0..127] of Byte`)
//   ObjPlayer.pas:269-274  m_sScriptLable / m_sInputData / m_sYesLable / m_sNoLable / m_boMessageBox
//   ObjPlayer.pas:1154-1155 SetQuestFlagStatus / GetQuestFlagStatus 声明
//   ObjPlayer.pas:1253     procedure GetScriptLabel(sMsg: string);
//   ObjPlayer.pas:1646-1652 ClearData 内的初始化（m_dwLastGotoLabelTick := MyGetTickCount）
//   ObjPlayer.pas:6202-6220 TPlayObject.GetQuestFlagStatus 实现
//   ObjPlayer.pas:6222-6241 TPlayObject.SetQuestFlagStatus 实现
//   ObjPlayer.pas:15172-15176 TPlayObject.SetScriptLabel 实现
//   ObjPlayer.pas:15179-15232 TPlayObject.GetScriptLabel 实现
//
// ⚠ 不重复声明的既有成员（git grep 证据见交付报告）：
//   · m_boOffLine / m_boDummyObject → Engine/ObjBase.OnlineMsg.cs:10/13（TCreature）
//   · m_nScriptGotoCount / m_sRandomString → Engine/NpcScriptState.cs:31/32（**另一个类** TNpcScriptState）
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.Core.Util;

namespace GXX.M2Server.Engine;

/// <summary>
/// NPC 会话与脚本标签片的外部依赖接缝。
/// </summary>
public static class PlayerSurfaceNpcSeams
{
    /// <summary>`GetPlayerPermission` 之外的 `TPlayObject` 脚本装载侧（`m_CanJmpScriptLableList` 等）。
    /// 默认空操作。</summary>
    public static Action<TPlayObject, string> SendFirstMsgToClient { get; set; } = (_, _) => { };

    /// <summary>`TNormNpc.GotoLable(TPlayObject, string, Boolean): Boolean`（ObjNpc.pas:9263）。
    /// 托管 `TNormNpc` 归属 `GXX.M2Server.Npc`（**不同命名空间**）—— 按「最小面 + 不造第二份」原则
    /// 用 `Func&lt;object, TPlayObject, string, bool, bool&gt;` 接缝，由集成方注入。
    /// 默认：无宿主，返回 false。</summary>
    public static Func<object, TPlayObject, string, bool, bool> GotoLable { get; set; } = (_, _, _, _) => false;

    /// <summary>`MyGetTickCount()` —— 复用 `GXX.Core.Rtl.DelphiRTL.GetTickCount()`（已是既有实现）。</summary>
    public static Func<uint> MyGetTickCount { get; set; } = GXX.Core.Rtl.DelphiRTL.GetTickCount;

    /// <summary>恢复默认（单测隔离用）。</summary>
    public static void ResetDefaults()
    {
        SendFirstMsgToClient = (_, _) => { };
        GotoLable = (_, _, _, _) => false;
        MyGetTickCount = GXX.Core.Rtl.DelphiRTL.GetTickCount;
    }
}

/// <summary>
/// ObjPlayer.pas 的 **NPC 会话 / 脚本标签 / 任务标志** 面。
/// </summary>
public partial class TPlayObject
{
    // ------------------------------------------------------------------
    // NPC 会话绑定（ObjPlayer.pas:116-118）
    // ------------------------------------------------------------------

    /// <summary>
    /// 原文 `m_Script: pTScript; // 0x62C`（ObjPlayer.pas:116）。
    /// 托管侧 `TScript` 由 ObjNpc 车道在 `GXX.M2Server.Npc` 定义（`ObjNpcSeams.cs:81`，对应
    /// `M2Definition.pas:223-228`）—— 本车道**复用该类型，不造第二份**。
    /// </summary>
    public Npc.TScript? m_Script;

    /// <summary>
    /// 原文 `m_NPC: TBaseObject; // 0x630`（ObjPlayer.pas:117）——当前对话的 NPC。
    /// 托管侧 `TBaseObject` 尚未切出，用最薄的 <see cref="TCreature"/> 代表
    /// （同 ObjNpc 车道报告 §6.3「托管侧签名偏差」的既有口径）。
    /// </summary>
    public TCreature? m_NPC;

    /// <summary>原文 `m_ItemBoxNpc: TBaseObject;`（ObjPlayer.pas:118）。
    /// 用法见 ObjPlayer.pas:41751/41755 —— `TNormNpc(m_ItemBoxNpc).GotoLable(Self, '@ItemIntoBox'+..., False)`。</summary>
    public TCreature? m_ItemBoxNpc;

    // ------------------------------------------------------------------
    // 脚本跳转状态（ObjPlayer.pas:148-154 / 269-274）
    // ------------------------------------------------------------------

    /// <summary>原文 `m_boBreakLoopGoto: Boolean;`（ObjPlayer.pas:148）。</summary>
    public bool m_boBreakLoopGoto;

    /// <summary>原文 `m_sScriptCurrLable: string; // 用于处理 @back 脚本命令`（ObjPlayer.pas:150）。</summary>
    public string m_sScriptCurrLable = "";

    /// <summary>原文 `m_sScriptGoBackLable: string; // 用于处理 @back 脚本命令`（ObjPlayer.pas:151）。</summary>
    public string m_sScriptGoBackLable = "";

    /// <summary>原文 `m_sLastGotoLabel: string;`（ObjPlayer.pas:152）——跳转防环用。</summary>
    public string m_sLastGotoLabel = "";

    /// <summary>
    /// 原文 `m_dwLastGotoLabelTick: LongWord;`（ObjPlayer.pas:153）——跳转防环用。
    /// 原文 `ClearData` 里初始化为 `MyGetTickCount`（ObjPlayer.pas:1651，**注意不是 0**）。
    /// </summary>
    public uint m_dwLastGotoLabelTick = PlayerSurfaceNpcSeams.MyGetTickCount();

    /// <summary>原文 `m_nOneLabelGotoCount: Integer;`（ObjPlayer.pas:154）——同一标签跳转计数（防死循环）。</summary>
    public int m_nOneLabelGotoCount;

    /// <summary>原文 `m_CanJmpScriptLableList: TStringList;`（ObjPlayer.pas:146）——`GetScriptLabel` 的产出。</summary>
    public readonly List<string> m_CanJmpScriptLableList = new();

    /// <summary>原文 `m_CanRequestStdItemList: TList;`（ObjPlayer.pas:147）——`&lt;ItemShow:...&gt;` 请求的标准物品下标。</summary>
    public readonly List<int> m_CanRequestStdItemList = new();

    /// <summary>原文 `m_sScriptLable: string; // 0x32C`（ObjPlayer.pas:269）。</summary>
    public string m_sScriptLable = "";

    /// <summary>原文 `m_sInputData: string; // 0x32C`（ObjPlayer.pas:270）——输入框回填数据。</summary>
    public string m_sInputData = "";

    /// <summary>原文 `m_sYesLable: string;`（ObjPlayer.pas:272）。</summary>
    public string m_sYesLable = "";

    /// <summary>原文 `m_sNoLable: string;`（ObjPlayer.pas:273）。</summary>
    public string m_sNoLable = "";

    /// <summary>原文 `m_boMessageBox: Boolean;`（ObjPlayer.pas:274）。</summary>
    public bool m_boMessageBox;

    // ------------------------------------------------------------------
    // 任务标志（ObjPlayer.pas:196 / Grobal2.pas:4116）
    // ------------------------------------------------------------------

    /// <summary>
    /// 原文 `m_QuestFlag: TQuestFlag; // 0x128 129`（ObjPlayer.pas:196），
    /// 其中 `TQuestFlag = array [0 .. 127] of Byte;`（Grobal2.pas:4116）。
    /// </summary>
    public readonly byte[] m_QuestFlag = new byte[128];

    /// <summary>原文 `TQuestFlag` 的元素个数（Grobal2.pas:4116 `array [0 .. 127]`）。</summary>
    public const int QUEST_FLAG_COUNT = 128;

    /// <summary>
    /// 原文 `function TPlayObject.GetQuestFlagStatus(nFlag: Integer): Integer;`
    /// （ObjPlayer.pas:1155 声明，:6202-6220 实现）。
    /// <code>
    ///   Result := 0;
    ///   Dec(nFlag);
    ///   if nFlag &lt; 0 then Exit;
    ///   n10 := nFlag div 8;
    ///   n14 := (nFlag mod 8);
    ///   if (n10 - SizeOf(TQuestFlag)) &lt; 0 then
    ///     if ((128 shr n14) and (m_QuestFlag[n10])) &lt;&gt; 0 then Result := 1 else Result := 0;
    /// </code>
    /// </summary>
    /// <remarks>
    /// ⚠ **原文缺陷（照抄并登记）**：边界判断写的是 `SizeOf(TQuestFlag)`（= **128 字节**），
    /// 而**本意显然是 `Length(TQuestFlag)`**（= 128 个元素）—— 二者在本例中恰好**数值相同**，
    /// 所以 `n10` 必须 **&lt; 128** 才进入分支，即 `nFlag &lt;= 1024`。
    /// 由于 `m_QuestFlag` 只有 128 个元素，`n10` 最大 127 恰好不越界 —— 侥幸正确。
    /// 该「巧合」在 `TQuestFlag` 改成 `array[0..N] of Word`（字节数 = 2N）时会**立刻越界**，
    /// 故此处**逐字复刻 128 这个字节数**而不是写成 `m_QuestFlag.Length`，
    /// 并在差异断言里锁定 `nFlag = 1025`（n10 = 128）时**返回 0 且不抛**。
    /// </remarks>
    public int GetQuestFlagStatus(int nFlag)
    {
        // 原文 6206：Result := 0;
        int result = 0;

        // 原文 6207：Dec(nFlag);
        nFlag--;

        // 原文 6208-6209：if nFlag < 0 then Exit;
        if (nFlag < 0) return result;

        // 原文 6211：n10 := nFlag div 8;   （Delphi `div` 对负数向零截断；此处 nFlag >= 0）
        int n10 = nFlag / 8;
        // 原文 6212：n14 := (nFlag mod 8);
        int n14 = nFlag % 8;

        // 原文 6213：if (n10 - SizeOf(TQuestFlag)) < 0 then   ← 原文如此（ObjPlayer.pas:6213）
        if (n10 - 128 < 0)
        {
            // 原文 6215：if ((128 shr n14) and (m_QuestFlag[n10])) <> 0 then
            if (((128 >> n14) & m_QuestFlag[n10]) != 0)
                result = 1;      // 原文 6216
            else
                result = 0;      // 原文 6218
        }

        return result;           // 原文 6219 落回 Result
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SetQuestFlagStatus(nFlag: Integer; nValue: Integer);`
    /// （ObjPlayer.pas:1154 声明，:6222-6241 实现）。
    /// <code>
    ///   Dec(nFlag);
    ///   if nFlag &lt; 0 then Exit;
    ///   n10 := nFlag div 8;  n14 := (nFlag mod 8);
    ///   if (n10 - SizeOf(TQuestFlag)) &lt; 0 then
    ///   begin
    ///     bt15 := m_QuestFlag[n10];
    ///     if nValue = 0 then m_QuestFlag[n10] := (not(128 shr n14)) and (bt15)
    ///     else               m_QuestFlag[n10] := (128 shr n14) or (bt15);
    ///   end;
    /// </code>
    /// </summary>
    /// <remarks>
    /// ⚠ Delphi `not` 是**按位移位取反**：`not(128 shr n14)` 在 Byte 上下文里**截断为 8 位**。
    /// 托管侧必须用 `(byte)~(128 &gt;&gt; n14)`（先按 32 位取反再截断），
    /// 若写成 `~(128 &gt;&gt; n14)` 赋给 `byte` 也会隐式截断 —— 但**显式强转**更能表达原文语义。
    /// </remarks>
    public void SetQuestFlagStatus(int nFlag, int nValue)
    {
        // 原文 6227：Dec(nFlag);
        nFlag--;

        // 原文 6228-6229：if nFlag < 0 then Exit;
        if (nFlag < 0) return;

        // 原文 6231-6232
        int n10 = nFlag / 8;
        int n14 = nFlag % 8;

        // 原文 6233：if (n10 - SizeOf(TQuestFlag)) < 0 then   ← 原文如此（ObjPlayer.pas:6233）
        if (n10 - 128 < 0)
        {
            // 原文 6235：bt15 := m_QuestFlag[n10];
            byte bt15 = m_QuestFlag[n10];
            if (nValue == 0)
                // 原文 6237：m_QuestFlag[n10] := (not(128 shr n14)) and (bt15)
                m_QuestFlag[n10] = (byte)((byte)~(128 >> n14) & bt15);
            else
                // 原文 6239：m_QuestFlag[n10] := (128 shr n14) or (bt15);
                m_QuestFlag[n10] = (byte)((128 >> n14) | bt15);
        }
    }

    // ------------------------------------------------------------------
    // 脚本标签收集（ObjPlayer.pas:15172 / 15179）
    // ------------------------------------------------------------------

    /// <summary>
    /// 原文 `procedure TPlayObject.SetScriptLabel(sLabel: string);`（ObjPlayer.pas:15172-15176）。
    /// <code>
    ///   m_CanJmpScriptLableList.Clear;
    ///   m_CanJmpScriptLableList.Add(sLabel);
    /// </code>
    /// </summary>
    public void SetScriptLabel(string sLabel)
    {
        // 原文 15174：m_CanJmpScriptLableList.Clear;
        m_CanJmpScriptLableList.Clear();
        // 原文 15175：m_CanJmpScriptLableList.Add(sLabel);
        m_CanJmpScriptLableList.Add(sLabel);
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.GetScriptLabel(sMsg: string);`（ObjPlayer.pas:1253 声明，
    /// :15179-15232 实现）——「取得当前脚本可以跳转的标签」。
    /// </summary>
    /// <remarks>
    /// 逐行对照（含原文笔误照抄）：
    /// <list type="bullet">
    ///   <item><description>:15186 `m_CanJmpScriptLableList.Clear`；:15187 的
    ///     `// m_CanRequestStdItemList.Clear;` 是**注释掉的**（抽奖脚本没有时间分析）→ 照抄不执行。</description></item>
    ///   <item><description>:15193 `sMsg := GetValidStr3_Ex(sMsg, sText, '\')` —— 用**单反斜杠**作分隔符。</description></item>
    ///   <item><description>:15199-15200：`if sText[1] &lt;&gt; '&lt;' then sText := '&lt;' + GetValidStr3_Ex(sText, sData, '&lt;');`
    ///     —— ⚠ 原文用 `sText[1]`（Delphi **1-based**），托管 `sText[0]`。</description></item>
    ///   <item><description>:15203 `CompareLStr(sCmdStr, 'ItemShow:', 9)` —— 前 9 字符比较；
    ///     托管侧 `HUtil32.CompareLStr`（`HUtil32.cs:591`）已存在（台账 §18.3 已修为大小写不敏感 + `compn &lt;= 0` 守卫）。</description></item>
    ///   <item><description>:15205 `Copy(sCmdStr, 10, MaxInt)` —— 托管 `Substring(9)`（Delphi 1-based → C# 0-based）。</description></item>
    ///   <item><description>:15212 `m_CanRequestStdItemList.Add(Pointer(nPos))` —— 原文往 `TList` 塞**指针**，
    ///     托管侧用 `List&lt;int&gt;`（值即下标）。</description></item>
    ///   <item><description>:15219 `(Length(sLabel) &gt;= 2) and (sLabel[2] = '@') and (sLabel[Length(sLabel)] = ')')`。</description></item>
    ///   <item><description>:15227 `m_CanJmpScriptLableList.Add(Trim(sLabel))`。</description></item>
    /// </list>
    /// </remarks>
    public void GetScriptLabel(string sMsg)
    {
        // 原文 15186：m_CanJmpScriptLableList.Clear;
        m_CanJmpScriptLableList.Clear();
        // 原文 15187：// m_CanRequestStdItemList.Clear;   ← 原文注释，不执行

        // 原文 15184：nPos: Integer;  （原文是过程级局部变量，跨 while 复用）
        int nPos;

        // 原文 15188：while (True) do
        while (true)
        {
            // 原文 15190-15191：if sMsg = '' then Break;
            if (sMsg == "") break;

            // 原文 15193：sMsg := GetValidStr3_Ex(sMsg, sText, '\');
            string sText = "";
            sMsg = HUtil32.GetValidStr3_Ex(sMsg, ref sText, '\\');

            // 原文 15194：if sText <> '' then
            if (sText != "")
            {
                // 原文 15196：sData := '';
                string sData = "";

                // 原文 15197：while (Pos('<', sText) > 0) and (Pos('>', sText) > 0) and (sText <> '') do
                // Delphi `Pos` 返回 1-based（0 = 未找到）→ 托管 `IndexOf` 返回 0-based（-1 = 未找到）。
                while (sText.IndexOf('<') >= 0 && sText.IndexOf('>') >= 0 && sText != "")
                {
                    // 原文 15199-15200：if sText[1] <> '<' then sText := '<' + GetValidStr3_Ex(sText, sData, '<');
                    if (sText[0] != '<')
                        sText = "<" + HUtil32.GetValidStr3_Ex(sText, ref sData, '<');

                    // 原文 15202：sText := ArrestStringEx(sText, '<', '>', sCmdStr);
                    string sCmdStr = "";
                    sText = HUtil32.ArrestStringEx(sText, '<', '>', ref sCmdStr);

                    // 原文 15203：if CompareLStr(sCmdStr, 'ItemShow:', 9) then
                    if (HUtil32.CompareLStr(sCmdStr, "ItemShow:", 9))
                    {
                        // 原文 15205：sTemp := Copy(sCmdStr, 10, MaxInt);
                        string sTemp = sCmdStr.Length >= 9 ? sCmdStr.Substring(9) : "";
                        // 原文 15206：nPos := Pos(':', sTemp);
                        nPos = sTemp.IndexOf(':') + 1;   // Delphi Pos 是 1-based，0 表示未找到
                        if (nPos > 0)
                        {
                            // 原文 15209：sTemp := Copy(sTemp, 1, nPos - 1);
                            sTemp = sTemp.Substring(0, nPos - 1);
                            // 原文 15210：nPos := StrToIntDef(sTemp, -1);
                            nPos = int.TryParse(sTemp, System.Globalization.NumberStyles.Integer,
                                System.Globalization.CultureInfo.InvariantCulture, out var parsed) ? parsed : -1;
                            // 原文 15211-15212：if nPos >= 0 then m_CanRequestStdItemList.Add(Pointer(nPos));
                            if (nPos >= 0)
                                m_CanRequestStdItemList.Add(nPos);
                        }
                    }

                    // 原文 15216：sLabel := GetValidStr3_Ex(sCmdStr, sCmdStr, '/');
                    // ★★ 原文缺陷 D-P6-1（ObjPlayer.pas:15216，**照抄并锁死**）：
                    //   Delphi `GetValidStr3(Str, var Dest, Divider)` 把**分隔符之前**的部分写进
                    //   `Dest`，**返回值**是分隔符**之后**的剩余串（HUtil32.pas:1467/1486-1487/1503）。
                    //   原文第二个实参就是 `sCmdStr` 本身，且结果赋给了 `sLabel` ——
                    //   于是 `sLabel` = **剩余串**，`sCmdStr` = **真正的标签**。
                    //   紧接着 15226 判的是 `sLabel[1] = '@'` → 对 `<@main>` 这样的输入，
                    //   sLabel = ""（无 '/'）或 "xxx"（有 '/'），**永远不以 '@' 开头**
                    //   → `m_CanJmpScriptLableList` **恒为空**，`LableIsCanJmp` 因此只会
                    //   命中 '@main'/'@HeroMap'/Yes/No 这几个硬编码项。
                    //   实测：输入 "<@main/x>" → sLabel = "x"、sCmdStr = "@main"。
                    //   本车道**逐字照抄**该行为（不"修好"它），差异断言见
                    //   PlayerSurfaceNpcSessionTests.GetScriptLabel_* 系列。
                    string sLabel = HUtil32.GetValidStr3_Ex(sCmdStr, ref sCmdStr, '/');
                    // 原文 15217：if sLabel <> '' then
                    if (sLabel != "")
                    {
                        // 原文 15219：if (Length(sLabel) >= 2) and (sLabel[2] = '@') and (sLabel[Length(sLabel)] = ')') then
                        if (sLabel.Length >= 2 && sLabel[1] == '@' && sLabel[sLabel.Length - 1] == ')')
                        {
                            // 原文 15221：nPos := Pos('(', sLabel); // 检测 <输入/@@InputInteger1(请输入元宝数量：)>
                            nPos = sLabel.IndexOf('(') + 1;
                            // 原文 15222-15223：if (nPos > 0) then sLabel := Copy(sLabel, 1, nPos - 1);
                            if (nPos > 0)
                                sLabel = sLabel.Substring(0, nPos - 1);
                        }

                        // 原文 15226-15227：if (Length(sLabel) > 0) and (sLabel[1] = '@') then m_CanJmpScriptLableList.Add(Trim(sLabel));
                        if (sLabel.Length > 0 && sLabel[0] == '@')
                            m_CanJmpScriptLableList.Add(sLabel.Trim());
                    }
                }
            }
        }
    }

    // ------------------------------------------------------------------
    // Initialize 覆写（ObjBase.pas:32888-32895 的 m_MagicList 段落在 TPlayObject 上的落点）
    // ------------------------------------------------------------------

    /// <summary>
    /// 原文 `TBaseObject.Initialize` 内部用 `TSmartObject(Self).m_MagicList` 强转访问魔术列表
    /// （ObjBase.pas:32890-32895）；托管侧 `m_MagicList` 落在 `TPlayObject`（`ObjBase.cs:167`），
    /// 故在此覆写并转发接缝 —— **保持虚分派链**（原文 `TBoxMonster.Initialize`(ObjNpc.pas:10521)、
    /// `TNormNpc.Initialize`(:9864) 都是 `inherited Initialize`）。
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();
        PlayerSurfaceBaseSeams.InitializeMagicLevelClamp(this);
    }
}
