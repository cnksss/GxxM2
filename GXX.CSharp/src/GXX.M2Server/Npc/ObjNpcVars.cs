// ============================================================================
// 源单元：Source\M2Engine\ObjNpc.pas（实测 10,546 LF，GBK）
// 本文件：TNormNpc 的**变量解析/赋值**方法族 1:1 移植。
//   · GetVarValue(PlayObject, sData, var nValue)                      4443-4455
//   · GetVarValue(PlayObject, sData, var sValue)                      4457-4465
//   · GetVarValue(PlayObject, sData, var sValue, var nValue,
//                 var IsBreakParseVar)                                4467-4480
//   · GetVarValue(PlayObject, sData, var sVar, var sValue, var nValue) 4482-4493
//   · SetVarValue                                                     4495-4510
//   · GetDynamicValue                                                 4512-4574
//   · SetDynamicValue                                                 4576-4633
//   · GetValNameValue                                                 5690-5877
//   · GetLineVariableText                                             5981-6009
//   · GetDynamicVarList                                               9877-9898
//
// 未覆盖（接缝转发，见报告）：SetValNameValue 4935-5325、GetBoxItemValue 5326-5689、
//   SetBoxItemValue 4645-4934、GetVariableText 6011-9262。
//
// ⚠ CompareLStr 偏差见 ObjNpcUnitFuncs.cs 头部说明：统一转调
//   `GXX.M2Server.Engine.MonGenParseCore.CompareLStr`（原文 1:1 版），
//   不用 `GXX.Core.Util.HUtil32.CompareLStr`（大小写敏感、缺 compn<=0 守卫）。
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.Core.Util;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Npc;

public partial class TNormNpc
{
    /// <summary>
    /// 原文 `procedure GetVarValue(PlayObject: TPlayObject; sData: string; var nValue: Integer); overload;`
    /// （ObjNpc.pas:4443-4455）。
    /// <para><b>原文易错点</b>：sData 不是 `&lt;$...&gt;` 形式时把 `nValue` **硬置 0**（不是保持不变）；
    /// 是 `&lt;$...&gt;` 形式时用 `StrToInt64Def(sValue, nValue)` —— 第二参是**入参当前值**（保留语义）。</para>
    /// </summary>
    public void GetVarValue(TPlayObject PlayObject, string sData, ref int nValue)
    {
        if ((sData.Length > 3) && (DelphiRTL.Pos("<", sData) > 0) && (DelphiRTL.Pos("$", sData) > 0)
            && (DelphiRTL.Pos(">", sData) > 0))
        {
            bool IsBreakParseVar = false;
            string sValue = GetLineVariableText(PlayObject, sData, ref IsBreakParseVar);
            nValue = (int)DelphiRTL.StrToInt64Def(sValue, nValue);
        }
        else
            nValue = 0;
    }

    /// <summary>
    /// 原文 `procedure GetVarValue(PlayObject: TPlayObject; sData: string; var sValue: string); overload;`
    /// （ObjNpc.pas:4457-4465）。
    /// <para><b>差异断言</b>：非 `&lt;$...&gt;` 形式时 `sValue := sData`（**透传**），
    /// 而 `GetVarValue(..., var nValue)` 同分支是把数字置 0 —— 两者不对称，照抄。</para>
    /// </summary>
    public void GetVarValue(TPlayObject PlayObject, string sData, ref string sValue)
    {
        if ((sData.Length > 3) && (DelphiRTL.Pos("<", sData) > 0) && (DelphiRTL.Pos("$", sData) > 0)
            && (DelphiRTL.Pos(">", sData) > 0))
        {
            bool IsBreakParseVar = false;
            sValue = GetLineVariableText(PlayObject, sData, ref IsBreakParseVar);
        }
        else
            sValue = sData;
    }

    /// <summary>
    /// 原文 `procedure GetVarValue(PlayObject: TPlayObject; sData: string; var sValue: string;
    /// var nValue: Integer; var IsBreakParseVar: Boolean); overload;`（ObjNpc.pas:4467-4480）。
    /// <para>与上一个重载的差异：**总是**先 `IsBreakParseVar := False`；且 `nValue` 用 `StrToInt64Def(sValue, 0)`
    /// —— 第二参是字面量 0（**不是**入参当前值，与 4443 版不同；原文 4473 注释 `// chongchong 2016-08-31`）。</para>
    /// </summary>
    public void GetVarValue(TPlayObject PlayObject, string sData, ref string sValue, ref int nValue,
        ref bool IsBreakParseVar)
    {
        IsBreakParseVar = false;
        if ((sData.Length > 3) && (DelphiRTL.Pos("<", sData) > 0) && (DelphiRTL.Pos("$", sData) > 0)
            && (DelphiRTL.Pos(">", sData) > 0))
        {
            sValue = GetLineVariableText(PlayObject, sData, ref IsBreakParseVar);
            nValue = (int)DelphiRTL.StrToInt64Def(sValue, 0); // chongchong 2016-08-31
        }
        else
        {
            sValue = sData;
            nValue = (int)DelphiRTL.StrToInt64Def(sValue, 0);
        }
    }

    /// <summary>
    /// 原文 `procedure GetVarValue(PlayObject: TPlayObject; sData: string; var sVar, sValue: string;
    /// var nValue: Integer); overload;`（ObjNpc.pas:4482-4493）。
    /// 解析优先级：`GetValNameValue` → `GetDynamicValue` → `GetLineVariableText`。
    /// </summary>
    public void GetVarValue(TPlayObject PlayObject, string sData, ref string sVar, ref string sValue, ref int nValue)
    {
        bool IsBreakParseVar = false;
        sVar = sData;
        if (GetValNameValue(PlayObject, sData, ref sValue, ref nValue))
            return;
        if (GetDynamicValue(PlayObject, sData, ref sValue, ref nValue))
            return;
        sValue = GetLineVariableText(PlayObject, sData, ref IsBreakParseVar);
        nValue = (int)DelphiRTL.StrToInt64Def(sValue, 0);
    }

    /// <summary>
    /// 原文 `function SetVarValue(PlayObject: TPlayObject; const sData, sValue: string;
    /// const nValue: Integer): Boolean;`（ObjNpc.pas:4495-4510）。
    /// <para><b>原文易错点</b>：空串**直接 Exit（Result 保持 False）**；成功分支用 `Exit` 短路。</para>
    /// </summary>
    public bool SetVarValue(TPlayObject PlayObject, string sData, string sValue, int nValue)
    {
        bool Result = false;
        if (sData == "")
            return Result;
        if (NpcSeams.SetValNameValue(this, PlayObject, sData, sValue, nValue))
        {
            Result = true;
            return Result;
        }
        if (SetDynamicValue(PlayObject, sData, sValue, nValue))
        {
            Result = true;
            return Result;
        }
        return Result;
    }

    /// <summary>
    /// 原文 `function GetDynamicValue(PlayObject: TPlayObject; sVar: string; var sValue: string;
    /// var nValue: Integer): Boolean;`（ObjNpc.pas:4512-4574）。
    /// <para><b>照抄的原文细节</b>：</para>
    /// <list type="bullet">
    /// <item>4520-4521：进入即 `sValue := sVar; nValue := 0;`（**即使最终返回 False 也已改写出参**）。</item>
    /// <item>4528/4531：`ArrestStringEx` 的返回值赋回 `sData`，但 `sData` 之后**再未被使用**（原文冗余，保留）。</item>
    /// <item>4544：`(sVarName = '') or (sVarType = '')` 才 Exit；`$HUMAN(` 前缀命中但括号内为空 → `sVarName = ''` → Exit。</item>
    /// <item>4556-4569：`case DynamicVar.VarType` **只列了 `vInteger` / `vString`**，`vNone` 落到无分支
    ///   → `Result` 仍为 False，但 4570 的 `Break` **仍然执行**（跳过后续同名项）。</item>
    /// </list>
    /// </summary>
    public bool GetDynamicValue(TPlayObject PlayObject, string sVar, ref string sValue, ref int nValue)
    {
        bool Result = false;
        sValue = sVar;
        nValue = 0;
        string sVarName = "";
        string sVarType = "";
        string sName = "";
        string sData = sVar;
        if ((sData.Length > 2) && (sData[0] == '<') && (sData[1] == '$') && (sData[sData.Length - 1] == '>'))
        {
            string sNameOut = "";
            sData = HUtil32.ArrestStringEx(sData, '<', '>', ref sNameOut);
            sName = sNameOut;
            if (MonGenParseCore.CompareLStr(sName, "$HUMAN(", "$HUMAN(".Length))
            {
                string sVarNameOut = "";
                sData = HUtil32.ArrestStringEx(sName, '(', ')', ref sVarNameOut);
                sVarName = sVarNameOut;
                sVarType = "HUMAN";
            }
            else if (MonGenParseCore.CompareLStr(sName, "$GUILD(", "$GUILD(".Length))
            {
                string sVarNameOut = "";
                sData = HUtil32.ArrestStringEx(sName, '(', ')', ref sVarNameOut);
                sVarName = sVarNameOut;
                sVarType = "GUILD";
            }
            else if (MonGenParseCore.CompareLStr(sName, "$GLOBAL(", "$GLOBAL(".Length))
            {
                string sVarNameOut = "";
                sData = HUtil32.ArrestStringEx(sName, '(', ')', ref sVarNameOut);
                sVarName = sVarNameOut;
                sVarType = "GLOBAL";
            }
            if ((sVarName == "") || (sVarType == ""))
                return Result;
            List<TDynamicVar> DynamicVarList = GetDynamicVarList(PlayObject, sVarType, ref sName);
            if (DynamicVarList == null)
            {
                return Result;
            }
            for (int I = 0; I <= DynamicVarList.Count - 1; I++)
            {
                TDynamicVar DynamicVar = DynamicVarList[I];
                if (ObjNpcText.CompareText(DynamicVar.sName, sVarName) == 0)
                {
                    switch (DynamicVar.VarType)
                    {
                        case TVarType.vInteger:
                            {
                                nValue = DynamicVar.nInternet;
                                sValue = DelphiRTL.IntToStr(nValue);
                                Result = true;
                            }
                            break;
                        case TVarType.vString:
                            {
                                sValue = DynamicVar.sString;
                                nValue = (int)DelphiRTL.StrToInt64Def(sValue, nValue);
                                Result = true;
                            }
                            break;
                    }
                    break;
                }
            }
        }
        return Result;
    }

    /// <summary>
    /// 原文 `function SetDynamicValue(PlayObject: TPlayObject; sVar: string; sValue: string;
    /// nValue: Integer): Boolean;`（ObjNpc.pas:4576-4633）。
    /// <para><b>与 GetDynamicValue 的差异断言</b>：本函数**不**改写出参式地预置（没有 `sValue := sVar` 那两行）；
    /// 且成功分支 `Result := True` 在 `case` **之后**，`vNone` 也会置 True（Get 版不会）。</para>
    /// </summary>
    public bool SetDynamicValue(TPlayObject PlayObject, string sVar, string sValue, int nValue)
    {
        bool Result = false;
        string sVarName = "";
        string sVarType = "";
        string sName = "";
        string sData = sVar;
        if ((sData.Length > 2) && (sData[0] == '<') && (sData[1] == '$') && (sData[sData.Length - 1] == '>'))
        {
            string sNameOut = "";
            sData = HUtil32.ArrestStringEx(sData, '<', '>', ref sNameOut);
            sName = sNameOut;
            if (MonGenParseCore.CompareLStr(sName, "$HUMAN(", "$HUMAN(".Length))
            {
                string sVarNameOut = "";
                sData = HUtil32.ArrestStringEx(sName, '(', ')', ref sVarNameOut);
                sVarName = sVarNameOut;
                sVarType = "HUMAN";
            }
            else if (MonGenParseCore.CompareLStr(sName, "$GUILD(", "$GUILD(".Length))
            {
                string sVarNameOut = "";
                sData = HUtil32.ArrestStringEx(sName, '(', ')', ref sVarNameOut);
                sVarName = sVarNameOut;
                sVarType = "GUILD";
            }
            else if (MonGenParseCore.CompareLStr(sName, "$GLOBAL(", "$GLOBAL(".Length))
            {
                string sVarNameOut = "";
                sData = HUtil32.ArrestStringEx(sName, '(', ')', ref sVarNameOut);
                sVarName = sVarNameOut;
                sVarType = "GLOBAL";
            }
            if ((sVarName == "") || (sVarType == ""))
                return Result;
            List<TDynamicVar> DynamicVarList = GetDynamicVarList(PlayObject, sVarType, ref sName);
            if (DynamicVarList == null)
            {
                return Result;
            }
            for (int I = 0; I <= DynamicVarList.Count - 1; I++)
            {
                TDynamicVar DynamicVar = DynamicVarList[I];
                if (ObjNpcText.CompareText(DynamicVar.sName, sVarName) == 0)
                {
                    switch (DynamicVar.VarType)
                    {
                        case TVarType.vInteger:
                            {
                                DynamicVar.nInternet = nValue;
                            }
                            break;
                        case TVarType.vString:
                            {
                                DynamicVar.sString = sValue;
                            }
                            break;
                    }
                    Result = true;
                    break;
                }
            }
        }
        return Result;
    }

    /// <summary>
    /// 原文 `function GetValNameValue(PlayObject: TPlayObject; sVar: string; var sValue: string;
    /// var nValue: Integer): Boolean;`（ObjNpc.pas:5690-5877）。
    /// <para><b>照抄的原文细节 / 易错点</b>：</para>
    /// <list type="bullet">
    /// <item>5697-5698：`sVar = ''` 时 `Exit` —— **出参 `sValue` / `nValue` 保持调用方原值**（故托管侧用 `ref` 而非 `out`）。</item>
    /// <item>5699-5702：随后无条件 `sValue := sVar; nValue := 0;`（**即使最终返回 False 也已改写出参**）。</item>
    /// <item>5703：`&lt;$STR(...)&gt;` 的判定要求 `Length(sData) &gt; 8`（不是 `&gt;= 8`）且**末字符是 `&gt;`、倒数第二字符是 `)`**；
    ///   5705 的 `ArrestStringEx` 返回值被赋回 `sData`，但 `sData` 之后**再未被使用**（原文冗余，保留）。</item>
    /// <item>5709：`SameText(Copy(sName, 1, 10), '&lt;$BOXITEM[')` —— 大小写不敏感定长 10 比较；
    ///   命中后 `sValue := sData`（即括号内内容）并 `StrToInt64Def(sValue, nValue)`（第二参是**入参当前值**，此处恒为 0）。</item>
    /// <item>5719-5745：一整段 `else` 被 `{ }` 块注释掉（"自定义OK框变量"）—— 原文如此，保留。</item>
    /// <item>5829/5849/5863：`L$` / `S$` / `N$` 三个前缀判定用 `UpCase(首字符)` 但 `第 2 字符 = '$'` **区分大小写**。</item>
    /// <item>5870：`n01 := PlayObject.m_ArrayList.GetIndex(...)` 未命中时 `nValue := 0`（**不是**保留原值）。</item>
    /// </list>
    /// <para><b>接缝</b>：`m_nVal`(P) / `m_sString`(S) / `m_TVal`(T) / `m_ArrayList`(L$) 与
    /// `g_Config.GlobaDyMval`(I) / `GlobalVal`(G) / `GlobalAVal`(A) 托管侧尚无归属，走 <see cref="NpcSeams"/>；
    /// 其余 8 个区间直接读既有 <c>GXX.M2Server.Engine.TPlayObject</c> 字段（CombatPower.cs:539-564）。</para>
    /// </summary>
    public bool GetValNameValue(TPlayObject PlayObject, string sVar, ref string sValue, ref int nValue)
    {
        bool Result = false;
        if (sVar == "")
            return Result;
        string sData = sVar;
        sValue = sVar;
        nValue = 0;
        string sName = sVar;
        if ((sData.Length > 8) && MonGenParseCore.CompareLStr(sData, "<$STR(", "<$STR(".Length)
            && (sData[sData.Length - 2] == ')') && (sData[sData.Length - 1] == '>'))
        {
            string sNameOut = "";
            sData = HUtil32.ArrestStringEx(sData, '(', ')', ref sNameOut);
            sName = sNameOut;
            if (sName == "")
                return Result;
        }
        if (string.Equals(DelphiRTL.Copy(sName, 1, 10), "<$BOXITEM[", StringComparison.OrdinalIgnoreCase))
        {
            var box = NpcSeams.GetBoxItemValue(sName, PlayObject);
            Result = box.Result;
            if (Result)
            {
                sData = box.Ret;
                sValue = sData;
                nValue = (int)DelphiRTL.StrToInt64Def(sValue, nValue);
            }
            return Result;
        }
        // 原文 5719-5745：整段 `else`（自定义OK框变量）被 `{ }` 注释掉 —— 原文如此，保留。
        // {
        //   else begin sVarName := ''; Index1 := Pos('<$', sName); ... end;
        // }
        int n01 = NpcSeams.GetValNameNo(sName);
        if (n01 >= 0)
        {
            if (n01 is >= 0 and <= 999)
            {
                // P
                nValue = NpcSeams.GetPlayerPVal(PlayObject, n01);
                sValue = DelphiRTL.IntToStr(nValue);
                Result = true;
            }
            else if (n01 is >= 1000 and <= 1999)
            {
                // D
                nValue = PlayObject.m_DyVal[n01 - 1000];
                sValue = DelphiRTL.IntToStr(nValue);
                Result = true;
            }
            else if (n01 is >= 2000 and <= 2999)
            {
                // M
                nValue = PlayObject.m_nMval[n01 - 2000];
                sValue = DelphiRTL.IntToStr(nValue);
                Result = true;
            }
            else if (n01 is >= 3000 and <= 3999)
            {
                // N
                nValue = PlayObject.m_nInteger[n01 - 3000];
                sValue = DelphiRTL.IntToStr(nValue);
                Result = true;
            }
            else if (n01 is >= 4000 and <= 4999)
            {
                // I
                nValue = NpcSeams.GetGlobaDyMval(n01 - 4000);
                sValue = DelphiRTL.IntToStr(nValue);
                Result = true;
            }
            else if (n01 is >= 5000 and <= 5999)
            {
                // G
                nValue = NpcSeams.GetGlobalVal(n01 - 5000);
                sValue = DelphiRTL.IntToStr(nValue);
                Result = true;
            }
            else if (n01 is >= 6000 and <= 6999)
            {
                // A
                sValue = NpcSeams.GetGlobalAVal(n01 - 6000) ?? "";
                nValue = (int)DelphiRTL.StrToInt64Def(sValue, nValue);
                Result = true;
            }
            else if (n01 is >= 7000 and <= 7999)
            {
                // S
                sValue = NpcSeams.GetPlayerSString(PlayObject, n01 - 7000);
                nValue = (int)DelphiRTL.StrToInt64Def(sValue, nValue);
                Result = true;
            }
            // 私有变量 U-数字型 chongchong 2014-10-18
            else if (n01 is >= 8000 and <= 8499)
            {
                nValue = PlayObject.m_UVal[n01 - 8000];
                sValue = DelphiRTL.IntToStr(nValue);
                Result = true;
            }
            // 私有变量 T-字符串型 chongchong 2014-10-18
            else if (n01 is >= 8500 and <= 8999)
            {
                sValue = NpcSeams.GetPlayerTVal(PlayObject, n01 - 8500);
                nValue = (int)DelphiRTL.StrToInt64Def(sValue, nValue);
                Result = true;
            }
            // 私有变量 J-数字型(1天1清) chongchong 2014-10-18
            else if (n01 is >= 9000 and <= 9499)
            {
                nValue = PlayObject.m_JVal[n01 - 9000];
                sValue = DelphiRTL.IntToStr(nValue);
                Result = true;
            }
            // 私有变量 Z-字符串型(1天1清) chongchong 2014-10-18
            else if (n01 is >= 9500 and <= 9999)
            {
                // ⚠ Engine.TPlayObject.m_ZVal 是 string[]，元素默认 **null**；
                //   原文是 ShortString，默认 **''**。取 '' 以贴合原文（差异已登记）。
                sValue = PlayObject.m_ZVal[n01 - 9500] ?? "";
                nValue = (int)DelphiRTL.StrToInt64Def(sValue, nValue);
                Result = true;
            }
        }
        // L变量
        else if ((sName.Length > 2) && (char.ToUpperInvariant(sName[0]) == 'L') && (sName[1] == '$'))
        {
            string arySIndex = "";
            if ((DelphiRTL.Pos("[", sName) > 0) && (DelphiRTL.Pos("]", sName) > 0))
            {
                // 原文 5833-5834：链式 GetValidStr3，`sData` 的最终值未被使用（原文冗余，保留）
                sData = HUtil32.GetValidStr3(sName, ref sName, new[] { '[' });
                sData = HUtil32.GetValidStr3(sData, ref arySIndex, new[] { ']' });
            }

            sValue = NpcSeams.GetPlayerArrayListValue(PlayObject, DelphiRTL.UpperCase(sName));
            nValue = (int)DelphiRTL.StrToInt64Def(sValue, 0);
            Result = true;
        }
        else if ((sName.Length > 2) && (char.ToUpperInvariant(sName[0]) == 'S') && (sName[1] == '$'))
        {
            int idx = PlayObject.m_StringList.GetIndex(DelphiRTL.UpperCase(sName));
            if (idx >= 0)
            {
                sValue = PlayObject.m_StringList.Strings[idx];
            }
            else
            {
                sValue = "";
            }
            nValue = (int)DelphiRTL.StrToInt64Def(sValue, 0);
            Result = true;
        }
        else if ((sName.Length > 2) && (char.ToUpperInvariant(sName[0]) == 'N') && (sName[1] == '$'))
        {
            n01 = PlayObject.m_IntegerList.GetIndex(DelphiRTL.UpperCase(sName));
            if (n01 >= 0)
            {
                nValue = Convert.ToInt32(PlayObject.m_IntegerList.Objects[n01]);
            }
            else
            {
                nValue = 0;
            }
            sValue = DelphiRTL.IntToStr(nValue);
            Result = true;
        }
        return Result;
    }

    /// <summary>
    /// 原文 `function GetVariableText(PlayObject: TPlayObject; var sMsg: string; sVariable: string;
    /// var IsBreakParseVar: Boolean; nPos: Integer = 0): Boolean; virtual;`（ObjNpc.pas:6011-9262）。
    /// <para><b>本车道只落"虚方法外壳"</b>：3,252 行的巨型 `case` 表未移植（见报告 §2.2），
    /// 本体经 <see cref="NpcSeams.GetVariableText"/> 转发。</para>
    /// <para>之所以必须做成**真虚方法**而非纯委托：原文 `TMerchant.GetVariableText`(3234-3270)、
    /// `TCastleOfficial.GetVariableText`(1118-1185)、`TGuildOfficial.GetVariableText`(10055-10090)
    /// 三个覆写都用 `inherited GetVariableText(...)`，且 `GetLineVariableText`(5981-6009) 对它是**虚调用**
    /// —— 用委托就破坏了这条虚分派链。</para>
    /// </summary>
    public virtual bool GetVariableText(TPlayObject PlayObject, ref string sMsg, string sVariable,
        ref bool IsBreakParseVar, int nPos)
    {
        var r = NpcSeams.GetVariableText(this, PlayObject, sMsg, sVariable, nPos);
        // sMsg / IsBreakParseVar 是 var 出参 —— 无论返回 True/False 都可能被改写，故无条件回写。
        sMsg = r.SMsg;
        IsBreakParseVar = r.IsBreakParseVar;
        return r.Result;
    }

    /// <summary>
    /// 原文 `procedure SendCustemMsg(PlayObject: TPlayObject; sMsg: string); virtual;`（ObjNpc.pas:9837-9862）。
    /// <para>本车道只落"虚方法外壳"：本体（含 `g_Config.boSendCustemMsg` 门与 `g_FilterTexts.Filter` 敏感词过滤）
    /// 未覆盖，经 <see cref="NpcSeams.SendCustemMsg"/> 转发。</para>
    /// <para>做成虚方法的理由同 <see cref="GetVariableText"/>：`TMerchant.SendCustemMsg`(4235-4238)、
    /// `TGuildOfficial.SendCustemMsg`(10386-10390)、`TCastleOfficial.SendCustemMsg`(10391-10405)
    /// 三个覆写都是 `inherited;`。</para>
    /// </summary>
    public virtual void SendCustemMsg(TPlayObject PlayObject, string sMsg)
    {
        NpcSeams.SendCustemMsg(this, PlayObject, sMsg);
    }

    /// <summary>
    /// 原文 `procedure Click(PlayObject: TPlayObject); virtual;`（ObjNpc.pas:4431-4442）。
    /// <para>本车道只落"虚方法外壳"：本体（把 `PlayObject` 的 6 个脚本标签字段归零后 `GotoLable(Player,'@main',False)`）
    /// 未覆盖 —— 那 6 个字段（`m_nScriptGotoCount`/`m_sScriptGoBackLable`/`m_sScriptCurrLable`/`m_sRandomString`/
    /// `m_sInputData`/`m_sNpcSelectItemName`）在托管侧 `TPlayObject` 上**一个都没有**，属 `ObjPlayer.pas` 面（见报告 §8.6）。</para>
    /// <para>做成虚方法的理由：`TMerchant.Click`(3228-3232)、`TGuildOfficial.Click`(10049-10053)、
    /// `TCastleOfficial.Click`(1107-1116) 三个覆写都用 `inherited`。</para>
    /// </summary>
    public virtual void Click(TPlayObject PlayObject)
    {
        NpcSeams.Click(this, PlayObject);
    }

    /// <summary>
    /// 原文 `function GetLineVariableText(PlayObject: TPlayObject; sMsg: string;
    /// var IsBreakParseVar: Boolean): string;`（ObjNpc.pas:5981-6009）。
    /// <para><b>照抄的原文细节</b>：`nStartPos := 1`（**1-based**）；`GetVariableText` 返回 False 时
    /// `nStartPos := nPos + 2`（跳过本次 `&lt;$`）；成功时不推进 startPos（由 `GetVariableText` 自行改写 `sMsg`）；
    /// 无论成败 `Inc(nC)`，`nC &gt;= 1001` 强制退出（防死循环）；`IsBreakParseVar` 是**按引用贯穿整轮**的。</para>
    /// <para>真正的替换逻辑在 <see cref="GetVariableText"/>（6011-9262 未覆盖，接缝转发）；
    /// 本处是**虚调用**，故 `TMerchant`/`TCastleOfficial`/`TGuildOfficial` 的覆写会生效 —— 与原文一致。</para>
    /// </summary>
    public string GetLineVariableText(TPlayObject PlayObject, string sMsg, ref bool IsBreakParseVar)
    {
        int nC = 0;
        int nStartPos = 1;
        while (true)
        {
            string s14 = sMsg;
            if ((DelphiRTL.Pos(">", s14) <= 0) || (DelphiRTL.Pos("$", s14) <= 0) || (nStartPos >= s14.Length))
                break;

            string s10 = "";
            int nPos = HUtil32.ArrestVariable(s14, '<', '$', '>', nStartPos, ref s10);
            if (s10 == "")
                break;

            // 原文 6000：`if not GetVariableText(PlayObject, sMsg, s10, IsBreakParseVar, nPos) then nStartPos := nPos + 2;`
            if (!GetVariableText(PlayObject, ref sMsg, s10, ref IsBreakParseVar, nPos))
            {
                nStartPos = nPos + 2;
            }

            nC++;
            if (nC >= 1001)
                break;
        }

        return sMsg;
    }

    /// <summary>
    /// 原文 `function GetDynamicVarList(PlayObject: TPlayObject; sType: string;
    /// var sName: string): TList;`（ObjNpc.pas:9877-9898）。
    /// <para><b>照抄的原文细节</b>：`sType` 用 `CompareLStr` 做**前缀**匹配（大小写不敏感），
    /// 故 `'HUMANX'` 也会命中 HUMAN 分支；`'GUILD'` 分支在 `m_MyGuild = nil` 时 `Exit`（Result 保持 nil）
    /// **且不改写 `sName`**；三个分支按 HUMAN → GUILD → GLOBAL 顺序短路。</para>
    /// </summary>
    public List<TDynamicVar> GetDynamicVarList(TPlayObject PlayObject, string sType, ref string sName)
    {
        List<TDynamicVar> Result = null;
        if (MonGenParseCore.CompareLStr(sType, "HUMAN", "HUMAN".Length))
        {
            Result = NpcSeams.GetPlayerDynamicVarList(PlayObject);
            sName = PlayObject.m_sCharName;
        }
        else if (MonGenParseCore.CompareLStr(sType, "GUILD", "GUILD".Length))
        {
            string guildName = NpcSeams.GetPlayerGuildName(PlayObject);
            if (guildName == null)
                return Result;

            Result = NpcSeams.GetGuildDynamicVarList(PlayObject);
            sName = guildName;
        }
        else if (MonGenParseCore.CompareLStr(sType, "GLOBAL", "GLOBAL".Length))
        {
            Result = NpcSeams.GetGlobalDynamicVarList();
            sName = "GLOBAL";
        }
        return Result;
    }
}
