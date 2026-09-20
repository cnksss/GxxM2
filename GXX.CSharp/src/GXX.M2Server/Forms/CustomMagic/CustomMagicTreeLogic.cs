// ============================================================================
// uFrmCustomMagic.pas（Source\M2Engine\Forms\uFrmCustomMagic.pas，4800 行，GBK）1:1 移植
// 车道 p5-m2-custommagic ｜ 命名空间 GXX.M2Server.Forms.CustomMagic
//
// 本文件 = 5 棵 TVirtualStringTree 的**纯逻辑内核**（列文本 / 编辑许可 / 勾选 / 节点点击
// 决策 / 单元格绘制决策 / 主列表红字）。VirtualTrees.pas 未移植，按 p2-rungate-impl 车道
// 已验证的做法：能抽成纯函数的全部抽出来单测，只有绘制与消息泵留接缝。
//
// 覆盖行号（Delphi）：
//   vstCustomMagicDrawText        :1877-1893
//   vstCustomMagicGetNodeDataSize :1895-1898
//   vstCustomMagicGetText         :1900-1908
//   vstAttackDecAttrChecked       :4290-4305
//   vstAttackDecAttrNodeClick     :4306-4325
//   vstAttackDecAttrAfterCellPaint:4326-4346
//   vstAttackDecAttrGetText       :4347-4399
//   vstAttackDecAttrEditing       :4400-4413
//   vstDecElementChecked          :4429-4444
//   vstDecElementNodeClick        :4445-4467
//   vstDecElementAfterCellPaint   :4468-4488
//   vstDecElementGetText          :4489-4520
//   vstDecElementEditing          :4521-4526
//   vstProtectedAddAttrChecked    :4551-4566
//   vstProtectedAddAttrNodeClick  :4567-4586
//   vstProtectedAddAttrAfterCellPaint :4587-4607
//   vstProtectedAddAttrGetText    :4608-4690
//   vstProtectedAddAttrEditing    :4691-4708
// ============================================================================

using System;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Forms.CustomMagic;

/// <summary>原文 <c>clRed / clBlue / clHighlightText</c> 的数值替身（VCL TColor）。</summary>
public static class CustomMagicColors
{
    /// <summary>clRed = $0000FF。</summary>
    public const int clRed = 0x0000FF;
    /// <summary>clBlue = $FF0000。</summary>
    public const int clBlue = 0xFF0000;
    /// <summary>clHighlightText = $00FFFFFF。</summary>
    public const int clHighlightText = 0x00FFFFFF;
}

/// <summary>vstCustomMagic 主列表的纯逻辑（原文 :1877-1908）。</summary>
public static class CustomMagicMainTreeLogic
{
    /// <summary>
    /// 原文 <c>vstCustomMagicGetNodeDataSize</c>（:1895-1898）：
    /// <c>NodeDataSize := SizeOf(TCustomMagicConfig);</c> —— 原文把**类引用**的大小填进去
    /// （4 字节），而树里存的是 <c>PMagicConfigNodeData</c>（指向 TMagicConfigNodeData 记录）。
    /// 两者在本单元内混用无误（记录只有一个指针字段），但审计时容易误判，登记为易错点。
    /// </summary>
    public const int NodeDataSize = 4;

    /// <summary>原文 <c>vstCustomMagicGetText</c>（:1900-1908）：列文本 = Config.MagicName。</summary>
    public static string GetText(TMagicConfigNodeData? data)
        => data?.Config?.MagicName ?? "";

    /// <summary>
    /// 原文 <c>vstCustomMagicDrawText</c>（:1877-1893）的字体颜色判定：
    /// <c>if ConfigNodeData.Config.IsChanged then clRed
    ///   else if Sender.Selected[Node] and Sender.Focused then clHighlightText
    ///   else Sender.Font.Color;</c>
    /// <para>注意分支顺序：IsChanged 优先于选中高亮。</para>
    /// </summary>
    public static int ResolveFontColor(TMagicConfigNodeData? data, bool selected, bool focused, int fontColor)
    {
        if (data != null)
        {
            if (data.Config != null && data.Config.IsChanged)
                return CustomMagicColors.clRed;
            if (selected && focused)
                return CustomMagicColors.clHighlightText;
            return fontColor;
        }
        return fontColor;   // 原文：ConfigNodeData = nil 时不改颜色（DefaultDraw 保持）
    }
}

/// <summary>节点点击后应执行的动作（原文三棵树的 NodeClick 决策）。</summary>
public enum TVtNodeClickAction
{
    /// <summary>HitNode = nil → Exit。</summary>
    None,
    /// <summary>HitColumn = 提示列 → ShowHint 取反 + InvalidateNode + SetConfigChanged。</summary>
    ToggleShowHint,
    /// <summary>HitColumn &gt; 0 → PostMessage(Self.Handle, WM_STARTEDITING_*, HitNode, HitColumn)。</summary>
    PostStartEditing,
}

/// <summary>一棵"属性树"（vstAttackDecAttr / vstProtectedAddAttr）的纯逻辑。原文 :4290-4413 / :4551-4708。</summary>
public static class DecAttribTreeLogic
{
    /// <summary>提示列号（原文 :4314 / :4575 <c>HitInfo.HitColumn = 12</c>）。</summary>
    public const int HintColumn = 12;

    /// <summary>原文 <c>Checked</c>（:4290-4305 / :4551-4566）：
    /// <c>if Sender.CheckState[Node] = csCheckedNormal then Data.IsChecked := True else Data.IsChecked := False;</c>
    /// 返回是否真的执行了写回（原文要求 <c>data &lt;&gt; nil</c>）。</summary>
    public static bool ApplyChecked(bool dataIsNull, TCheckState checkState, ref bool isChecked)
    {
        if (dataIsNull)
            return false;
        isChecked = checkState == TCheckState.csCheckedNormal;
        return true;
    }

    /// <summary>原文 <c>NodeClick</c>（:4306-4325 / :4567-4586）的决策（含分支顺序）。</summary>
    public static TVtNodeClickAction ResolveNodeClick(bool hitNodeIsNull, int hitColumn)
    {
        if (hitNodeIsNull)
            return TVtNodeClickAction.None;         // 原文 if HitInfo.HitNode = nil then Exit;
        if (hitColumn == HintColumn)
            return TVtNodeClickAction.ToggleShowHint;
        if (hitColumn > 0)
            return TVtNodeClickAction.PostStartEditing;
        return TVtNodeClickAction.None;
    }

    /// <summary>原文 <c>AfterCellPaint</c>（:4326-4346 / :4587-4607）：
    /// 只有提示列画图标，且 <c>ImageIndex := Integer(Data.ShowHint)</c>，
    /// 坐标 <c>Pt.X := CellRect.Left + (CellRect.Right - CellRect.Left - ilCheck.Width) div 2</c>（Y 同理）。</summary>
    public static bool TryGetHintIconLayout(int column, TRectSeam cellRect, int imageListWidth, int imageListHeight,
                                            bool showHint, out int x, out int y, out int imageIndex)
    {
        x = y = 0;
        imageIndex = 0;
        if (column != HintColumn)
            return false;

        imageIndex = showHint ? 1 : 0;
        x = cellRect.Left + (cellRect.Right - cellRect.Left - imageListWidth) / 2;   // Delphi div = 截断除法
        y = cellRect.Top + (cellRect.Bottom - cellRect.Top - imageListHeight) / 2;
        return true;
    }

    /// <summary>原文 <c>GetText</c>（:4347-4399 / :4608-4690）的减属性版列文本。</summary>
    public static string GetAttackDecAttribText(int column, TAttackDecAttribData data, TMagicChangeAttributesRecord d)
    {
        switch (column)
        {
            case 0: return CustomMagicUtils.MagicAttackDecAttributesTypeNames[(int)data.AttribType];
            case 1: return d.Rate.ToString();
            case 2: return d.RateAdd.ToString();
            case 3:
                // 原文 :4355-4358 if AttribType >= daHitPoint then '-' else IntToStr(LowValue)
                return data.AttribType >= TMagicAttackDecAttributesType.daHitPoint ? "-" : d.LowValue.ToString();
            case 4:
                return data.AttribType >= TMagicAttackDecAttributesType.daHitPoint
                    ? "-" : CustomMagicUtils.MagicAttackDecValueTypeNames[d.LowValueIsPoint ? 1 : 0];
            case 5:
                return data.AttribType >= TMagicAttackDecAttributesType.daHitPoint ? "-" : d.LowValueAdd.ToString();
            case 6: return d.HighValue.ToString();
            case 7: return CustomMagicUtils.MagicAttackDecValueTypeNames[d.HighValueIsPoint ? 1 : 0];
            case 8: return d.HighValueAdd.ToString();
            case 9: return d.Time.ToString();
            case 10: return d.TimeAdd.ToString();
            case 11: return CustomMagicUtils.MagicAttackDecTimeTypeNames[d.TimeAddIsPoint ? 1 : 0];
            case 12: return " ";        // 原文 CellText := ' '
            case 13: return d.HintText;
            default: return "";         // 原文 case 无 else，CellText 保持调用方初值（此处取空串）
        }
    }

    /// <summary>
    /// 原文 <c>GetText</c> 的保护属性版（:4608-4690）。
    /// <para>
    /// **与减属性版的差异（差异断言见测试）**：保护版在列 6/7/8 用 <c>AttribType in [aaHide]</c>，
    /// 列 9/10/11 用 <c>AttribType in [aaHP, aaMP]</c>，而列 3/4/5 仍用 <c>&gt;= aaHitPoint</c>。
    /// 另注意 <b>aaNGDamage/aaNGDefense 也 &gt;= aaHitPoint</b>，故列 3/4/5 对它们同样输出 '-'。
    /// </para>
    /// </summary>
    public static string GetProtectedAddAttribText(int column, TProtectAddAttribData data, TMagicChangeAttributesRecord d)
    {
        switch (column)
        {
            case 0: return CustomMagicUtils.MagicProtectAddAttributesTypeNames[(int)data.AttribType];
            case 1: return d.Rate.ToString();
            case 2: return d.RateAdd.ToString();
            case 3:
                return data.AttribType >= TMagicProtectAddAttributesType.aaHitPoint ? "-" : d.LowValue.ToString();
            case 4:
                return data.AttribType >= TMagicProtectAddAttributesType.aaHitPoint
                    ? "-" : CustomMagicUtils.MagicAttackDecValueTypeNames[d.LowValueIsPoint ? 1 : 0];
            case 5:
                return data.AttribType >= TMagicProtectAddAttributesType.aaHitPoint ? "-" : d.LowValueAdd.ToString();
            case 6:
                return data.AttribType == TMagicProtectAddAttributesType.aaHide ? "-" : d.HighValue.ToString();
            case 7:
                return data.AttribType == TMagicProtectAddAttributesType.aaHide
                    ? "-" : CustomMagicUtils.MagicAttackDecValueTypeNames[d.HighValueIsPoint ? 1 : 0];
            case 8:
                return data.AttribType == TMagicProtectAddAttributesType.aaHide ? "-" : d.HighValueAdd.ToString();
            case 9:
                return IsHpOrMp(data.AttribType) ? "-" : d.Time.ToString();
            case 10:
                return IsHpOrMp(data.AttribType) ? "-" : d.TimeAdd.ToString();
            case 11:
                return IsHpOrMp(data.AttribType)
                    ? "-" : CustomMagicUtils.MagicAttackDecTimeTypeNames[d.TimeAddIsPoint ? 1 : 0];
            case 12: return " ";
            case 13: return d.HintText;
            default: return "";
        }
    }

    private static bool IsHpOrMp(TMagicProtectAddAttributesType t)
        => t == TMagicProtectAddAttributesType.aaHP || t == TMagicProtectAddAttributesType.aaMP;

    /// <summary>原文 <c>Editing</c>（:4400-4413）：<c>Allowed := (Node &lt;&gt; nil) and (Column &gt; 0) and (Column &lt;&gt; 12);</c>
    /// 再按列与属性类型禁掉 3/4/5（<c>AttribType &gt;= daHitPoint</c>）。</summary>
    public static bool IsAttackDecAttribEditingAllowed(bool nodeIsNull, int column, TMagicAttackDecAttributesType attribType)
    {
        bool allowed = !nodeIsNull && column > 0 && column != HintColumn;
        if (allowed)
        {
            if ((column == 3 || column == 4 || column == 5) && attribType >= TMagicAttackDecAttributesType.daHitPoint)
                allowed = false;
        }
        return allowed;
    }

    /// <summary>原文 <c>Editing</c>（:4691-4708）：三条互斥的禁编辑规则（else-if 链，顺序照抄）。</summary>
    public static bool IsProtectedAddAttribEditingAllowed(bool nodeIsNull, int column, TMagicProtectAddAttributesType attribType)
    {
        bool allowed = !nodeIsNull && column > 0 && column != HintColumn;
        if (allowed)
        {
            if ((column == 3 || column == 4 || column == 5) && attribType >= TMagicProtectAddAttributesType.aaHitPoint)
                allowed = false;
            else if ((column == 6 || column == 7 || column == 8) && attribType == TMagicProtectAddAttributesType.aaHide)
                allowed = false;
            else if ((column == 9 || column == 10 || column == 11) && IsHpOrMp(attribType))
                allowed = false;
        }
        return allowed;
    }
}

/// <summary>元素树（vstDecElement / vstAddElement）的纯逻辑。原文 :4429-4532。</summary>
public static class ElementTreeLogic
{
    /// <summary>提示列号（原文 :4453 <c>HitInfo.HitColumn = 9</c>）。</summary>
    public const int HintColumn = 9;

    /// <summary>原文 <c>Checked</c>（:4429-4444）。</summary>
    public static bool ApplyChecked(bool dataIsNull, TCheckState checkState, ref bool isChecked)
    {
        if (dataIsNull)
            return false;
        isChecked = checkState == TCheckState.csCheckedNormal;
        return true;
    }

    /// <summary>原文 <c>NodeClick</c>（:4445-4467）的决策。</summary>
    public static TVtNodeClickAction ResolveNodeClick(bool hitNodeIsNull, int hitColumn)
    {
        if (hitNodeIsNull)
            return TVtNodeClickAction.None;
        if (hitColumn == HintColumn)
            return TVtNodeClickAction.ToggleShowHint;
        if (hitColumn > 0)
            return TVtNodeClickAction.PostStartEditing;
        return TVtNodeClickAction.None;
    }

    /// <summary>
    /// 原文 <c>NodeClick</c> 里 <c>if Sender = vstDecElement then
    /// PostMessage(..., WM_STARTEDITING_DEC_ELEMENT, ...) else PostMessage(..., WM_STARTEDITING_INC_ELEMENT, ...)</c>
    /// 的消息号选择（:4462-4464）。
    /// </summary>
    public static int StartEditingMessage(bool senderIsDecElement)
        => senderIsDecElement ? CustomMagicWm.WM_STARTEDITING_DEC_ELEMENT : CustomMagicWm.WM_STARTEDITING_INC_ELEMENT;

    /// <summary>原文 <c>AfterCellPaint</c>（:4468-4488）：提示列按 <c>Integer(Data.ShowHint)</c> 画图标。</summary>
    public static bool TryGetHintIconLayout(int column, TRectSeam cellRect, int imageListWidth, int imageListHeight,
                                            bool showHint, out int x, out int y, out int imageIndex)
        => DecAttribTreeLogic.TryGetHintIconLayout(column, cellRect, imageListWidth, imageListHeight, showHint,
                                                   out x, out y, out imageIndex);

    /// <summary>原文 <c>GetText</c>（:4489-4520）：元素树列文本，无 "-" 占位逻辑。</summary>
    public static string GetText(int column, TMagicElementData data, TMagicAttackChangeElementRecord d)
    {
        switch (column)
        {
            case 0: return CustomMagicUtils.ItemElementsTypeNames[(int)data.ElementType];
            case 1: return d.Rate.ToString();
            case 2: return d.RateAdd.ToString();
            case 3: return d.Value.ToString();
            case 4: return CustomMagicUtils.MagicAttackDecValueTypeNames[d.ValueIsPoint ? 1 : 0];
            case 5: return d.ValueAdd.ToString();
            case 6: return d.Time.ToString();
            case 7: return d.TimeAdd.ToString();
            case 8: return CustomMagicUtils.MagicAttackDecTimeTypeNames[d.TimeAddIsPoint ? 1 : 0];
            case 9: return " ";
            case 10: return d.HintText;
            default: return "";
        }
    }

    /// <summary>原文 <c>Editing</c>（:4521-4526）：<c>Allowed := (Node &lt;&gt; nil) and (Column &gt; 0) and (Column &lt;&gt; 9);</c>
    /// —— **元素树没有按属性类型的附加禁令**（与属性树不同，差异断言见测试）。</summary>
    public static bool IsEditingAllowed(bool nodeIsNull, int column)
        => !nodeIsNull && column > 0 && column != HintColumn;
}
