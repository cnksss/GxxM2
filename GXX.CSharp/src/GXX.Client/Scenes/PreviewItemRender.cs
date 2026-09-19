using System;
using System.Collections.Generic;

namespace GXX.Client.Scenes;

/// <summary>Grobal2.pas 6312-6319 TPreviewMonItem（packed record）镜像。</summary>
public sealed class TPreviewMonItem
{
    public string sName = "";
    public byte btColor;
    public bool boValueItem;
    public ushort wLooks;
    public ushort EffectIndex;
    public int nCount;
}

/// <summary>
/// Grobal2.pas 6323-6336 TClientPreviewMonItem 镜像（含逐项特效帧状态）。
/// </summary>
public sealed class TClientPreviewMonItem
{
    public TPreviewMonItem PreiewMonItem = new();

    public int TextureWidth;
    public int TextureHeight;

    public DropItemEffectDef ItemEffect = DropItemEffectDef.None();
    public int ItemEffectFrame;
    public uint ItemEffectTick;

    public int ValueItemEffectFrame;
    public uint ValueItemEffectTick;
}

/// <summary>预览物品的一次绘制（GameCanvas.Draw 的 headless 产物）。</summary>
public sealed record PreviewItemDrawOp(int X, int Y, FxImage Image, bool Blend, string Kind);

/// <summary>预览物品的名字绘制（BoldTextOut 的 headless 产物）。</summary>
public sealed record PreviewNameDrawOp(int X, int Y, string Text, int Color);

/// <summary>
/// PlayScn.pas 物品预览族（批次J82，无纹理 headless 化）——
/// DrawPreviewItem(3493-3579)、DrawPreviewMonItemEffect(3405-3451)、
/// DrawPreviewMonItemValueEffect(3453-3491)。
/// 网格常量 COL_WIDTH = 100 / ROW_HEIGHT = 40；
/// 定位用 (Actor.m_nRy - ClientRect.Top - 1) * UNITY + m_nDefYY 与
/// (Actor.m_nRx - ClientRect.Left) * UNITX + m_nDefXX（与 PlaySceneCore 的角色定位同式）。
/// </summary>
public static class PreviewItemRender
{
    public const int ColWidth = 100;
    public const int RowHeight = 40;

    /// <summary>图标居中基准（3542-3543：`(20 - d.Width) div 2`）。</summary>
    public const int IconBox = 20;

    public const int UnitX = 48;
    public const int Unity = 32;

    /// <summary>
    /// DrawPreviewMonItemEffect（3405-3451）1:1：四重守卫（隐藏特效开关对、FileIndex 范围、
    /// ImageCount、BelowItem 匹配、Time）→ 帧推进（Time 周期）→ 帧区间钳制回卷 →
    /// DrawCenter 居中或原点偏移 → NoBlend 分流。
    /// 返回 null = 不绘（守卫截停）。
    /// </summary>
    public static PreviewItemDrawOp? PlanItemEffect(
        TClientPreviewMonItem item, uint now,
        bool hideItemEffectChecked, bool hideItemEffect,
        bool isBelowItem, bool selfDead,
        int effectImageCount, Func<int, FxImage?> resolve, int nX, int nY)
    {
        var eff = item.ItemEffect;

        if (hideItemEffectChecked && hideItemEffect)
            return null;
        if (eff.FileIndex < 0 || eff.FileIndex >= effectImageCount)
            return null;
        if (eff.ImageCount == 0)
            return null;
        if (eff.BelowItem != isBelowItem)
            return null;
        if (eff.Time <= 0)
            return null;

        if (now - item.ItemEffectTick >= (uint)eff.Time)
        {
            item.ItemEffectFrame++;
            item.ItemEffectTick = now;
        }

        if (item.ItemEffectFrame < eff.StartIndex)
            item.ItemEffectFrame = eff.StartIndex;
        else if (item.ItemEffectFrame >= eff.StartIndex + eff.ImageCount)
            item.ItemEffectFrame = eff.StartIndex;

        var d = resolve(item.ItemEffectFrame);
        if (d == null)
            return null;

        if (eff.DrawCenter)
        {
            nX = nX + eff.OffsetX + (item.TextureWidth - d.Width) / 2;
            nY = nY + eff.OffsetY + (item.TextureHeight - d.Height) / 2;
        }
        else
        {
            nX = nX + eff.OffsetX + d.OriginX;
            nY = nY + eff.OffsetY + d.OriginY;
        }

        return new PreviewItemDrawOp(nX, nY, d, !eff.NoBlend, "ItemEffect");
    }

    /// <summary>
    /// DrawPreviewMonItemValueEffect（3453-3491）1:1：五道配置门 + boValueItem →
    /// 帧推进回卷（Count 环，下界 0）→ 图号 = 帧 + dwValueItemEffectIndex →
    /// 恒 Draw（无混合）且偏移取 nValueItemEffectOffsetX/Y + 原点。
    /// </summary>
    public static PreviewItemDrawOp? PlanValueItemEffect(
        TClientPreviewMonItem item, uint now,
        bool showDropValueItemEff, bool showValueItemEffectChecked, bool showValueItemEffect,
        int valueItemCount, int valueItemPlayTime, int valueItemEffectIndex,
        int offsetX, int offsetY,
        bool selfDead, Func<int, FxImage?> resolve, int nX, int nY)
    {
        if (!showDropValueItemEff)
            return null;
        if (!showValueItemEffectChecked)
            return null;
        if (!showValueItemEffect)
            return null;
        if (valueItemCount == 0)
            return null;
        if (valueItemPlayTime <= 0)
            return null;
        if (!item.PreiewMonItem.boValueItem)
            return null;

        if (now - item.ValueItemEffectTick >= (uint)valueItemPlayTime)
        {
            item.ValueItemEffectFrame++;
            item.ValueItemEffectTick = now;
        }

        if (item.ValueItemEffectFrame < 0)
            item.ValueItemEffectFrame = 0;
        else if (item.ValueItemEffectFrame >= valueItemCount)
            item.ValueItemEffectFrame = 0;

        var d = resolve(item.ValueItemEffectFrame + valueItemEffectIndex);
        if (d == null)
            return null;

        return new PreviewItemDrawOp(nX + offsetX + d.OriginX, nY + offsetY + d.OriginY, d, false, "ValueItemEffect");
    }

    /// <summary>物品显示名（3555-3558：nCount &gt; 1 时追加 ' ×N'）。</summary>
    public static string DisplayName(TPreviewMonItem it)
        => it.nCount > 1 ? it.sName + " ×" + it.nCount : it.sName;

    /// <summary>
    /// 网格行列（3515-3523）：nCount &lt; 3 时 1 行 nCount 列；
    /// 否则 nCol = Trunc(Sqrt(nCount))、nRow = (nCount + nCol - 1) div nCol。
    /// </summary>
    public static (int Col, int Row) GridSize(int nCount)
    {
        if (nCount < 3)
            return (nCount, 1);

        int nCol = (int)Math.Sqrt(nCount);
        int nRow = (nCount + nCol - 1) / nCol;
        return (nCol, nRow);
    }

    /// <summary>
    /// 单个格子左上角坐标（3526-3528）：
    /// nY = (Ry - ClientTop - 1) * UNITY + DefYY - Round((nRow-1)/2 * ROW_HEIGHT)；
    /// nX = (Rx - ClientLeft) * UNITX + DefXX - Round((nCol-1)/2 * COL_WIDTH)。
    /// </summary>
    public static (int X, int Y) GridOrigin(
        int rx, int ry, int clientLeft, int clientTop,
        int defXX, int defYY, int nCol, int nRow)
    {
        int nY = (ry - clientTop - 1) * Unity + defYY
                 - DelphiRound((nRow - 1) / 2.0 * RowHeight);
        int nX = (rx - clientLeft) * UnitX + defXX
                 - DelphiRound((nCol - 1) / 2.0 * ColWidth);
        return (nX, nY);
    }

    /// <summary>图标绘制坐标（3542-3543：格子左上 + (20 - 尺寸) div 2）。</summary>
    public static (int X, int Y) IconPos(int cellX, int cellY, int w, int h)
        => (cellX + (IconBox - w) / 2, cellY + (IconBox - h) / 2);

    /// <summary>名字绘制坐标（3560-3561：X 居中于 20 宽、Y = 格子顶 + 20）。</summary>
    public static (int X, int Y) NamePos(int cellX, int cellY, int textWidth)
        => (cellX + (IconBox - textWidth) / 2, cellY + IconBox);

    /// <summary>Delphi Round（银行家舍入：.5 → 最近偶数）。</summary>
    private static int DelphiRound(double v) => (int)Math.Round(v, MidpointRounding.ToEven);

    /// <summary>
    /// DrawPreviewItem（3493-3579）1:1：入口三道守卫（列表非空 / Recog ≠ 0 / ShowTime ≠ 0）
    /// → 超时守卫 → FindActor → 幽灵/死亡守卫 → 网格布局 → 逐格取图（观察者死亡取灰度）、
    /// 记录纹理宽高、绘制下方光效 → 绘图标 → 绘制上方光效 → 名字（含 ×N）→
    /// 图标再次绘制 ValueItem 特效 → 列推进，nIndex 达 nCount 立即 Exit（内层直接返回，
    /// **不绘完当前行剩余格**）→ 行推进。
    /// </summary>
    public static (List<PreviewItemDrawOp> Draws, List<PreviewNameDrawOp> Names) DrawPreviewItem(
        IReadOnlyList<TClientPreviewMonItem>? g_PreviewItem,
        int g_nPreviewItemActorRecog,
        uint g_dwPreviewItemShowTime,
        uint g_dwPreviewItemShowTick,
        uint now,
        bool actorFound, bool actorGhost, bool actorDeath,
        int actorRx, int actorRy,
        int clientLeft, int clientTop,
        int defXX, int defYY,
        bool selfDead,
        bool hideItemEffectChecked, bool hideItemEffect,
        bool showDropValueItemEff, bool showValueItemEffectChecked, bool showValueItemEffect,
        int valueItemCount, int valueItemPlayTime, int valueItemEffectIndex,
        int valueItemOffsetX, int valueItemOffsetY,
        int effectImageCount,
        Func<ushort, FxImage?> resolveLooks,
        Func<ushort, FxImage?> resolveLooksGray,
        Func<int, FxImage?> resolveEffect,
        Func<int, FxImage?> resolveValueEffect,
        Func<string, int> textWidth,
        Func<byte, int> getRgb)
    {
        var draws = new List<PreviewItemDrawOp>();
        var names = new List<PreviewNameDrawOp>();

        if (g_PreviewItem == null || g_PreviewItem.Count == 0)
            return (draws, names);
        if (g_nPreviewItemActorRecog == 0)
            return (draws, names);
        if (g_dwPreviewItemShowTime == 0)
            return (draws, names);

        // 3509：tick_diff(g_dwPreviewItemShowTick, now) > ShowTime → Exit
        if (now - g_dwPreviewItemShowTick > g_dwPreviewItemShowTime)
            return (draws, names);

        if (!actorFound)
            return (draws, names);
        if (actorGhost || actorDeath)
            return (draws, names);

        int nCount = g_PreviewItem.Count;
        var (nCol, nRow) = GridSize(nCount);

        int nIndex = 0;
        var (startX, startY) = GridOrigin(actorRx, actorRy, clientLeft, clientTop, defXX, defYY, nCol, nRow);
        int nY = startY;

        for (int i = 0; i < nRow; i++)
        {
            int nX = startX;

            for (int ii = 0; ii < nCol; ii++)
            {
                var item = g_PreviewItem[nIndex];

                // 3533-3536：观察者（g_MySelf）死亡 → g_WDnItemImages.Grays，否则 .Images
                var d = selfDead
                    ? resolveLooksGray(item.PreiewMonItem.wLooks)
                    : resolveLooks(item.PreiewMonItem.wLooks);

                if (d != null)
                {
                    item.TextureWidth = d.Width;
                    item.TextureHeight = d.Height;

                    var (tmpX, tmpY) = IconPos(nX, nY, d.Width, d.Height);

                    var below = PlanItemEffect(item, now, hideItemEffectChecked, hideItemEffect,
                        true, selfDead, effectImageCount, resolveEffect, tmpX, tmpY);
                    if (below != null)
                        draws.Add(below);

                    draws.Add(new PreviewItemDrawOp(tmpX, tmpY, d, false, "Icon"));

                    var above = PlanItemEffect(item, now, hideItemEffectChecked, hideItemEffect,
                        false, selfDead, effectImageCount, resolveEffect, tmpX, tmpY);
                    if (above != null)
                        draws.Add(above);
                }

                string sName = DisplayName(item.PreiewMonItem);
                var (nameX, nameY) = NamePos(nX, nY, textWidth(sName));
                names.Add(new PreviewNameDrawOp(nameX, nameY, sName, getRgb(item.PreiewMonItem.btColor)));

                if (d != null)
                {
                    var (tmpX, tmpY) = IconPos(nX, nY, d.Width, d.Height);
                    var val = PlanValueItemEffect(item, now,
                        showDropValueItemEff, showValueItemEffectChecked, showValueItemEffect,
                        valueItemCount, valueItemPlayTime, valueItemEffectIndex,
                        valueItemOffsetX, valueItemOffsetY,
                        selfDead, resolveValueEffect, tmpX, tmpY);
                    if (val != null)
                        draws.Add(val);
                }

                nX += ColWidth;
                nIndex++;
                if (nIndex >= nCount)
                    return (draws, names);          // 3574：内层直接 Exit
            }

            nY += RowHeight;
        }

        return (draws, names);
    }
}
