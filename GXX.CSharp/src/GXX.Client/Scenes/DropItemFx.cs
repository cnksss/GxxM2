using System;
using System.Collections.Generic;

namespace GXX.Client.Scenes;

/// <summary>TDropItemEffect packed record（Grobal2.pas 5933）镜像。</summary>
public struct DropItemEffectDef
{
    public int ItemEffectIndex;
    public int FileIndex;      // SmallInt
    public int StartIndex;     // Word
    public int Time;           // Word
    public int ImageCount;     // Byte
    public bool DrawCenter;
    public bool NoBlend;
    public bool BelowItem;
    public int OffsetX;        // SmallInt
    public int OffsetY;        // SmallInt

    public static DropItemEffectDef None() => new() { FileIndex = -1 };
}

/// <summary>取图产物（GetCachedImage 的 headless 镜像：图宽高与原点偏移）。</summary>
public sealed record FxImage(int ImageIndex, int Width, int Height, int OriginX, int OriginY, bool Gray);

/// <summary>掉落物特效帧状态（TDropItem 内嵌特效字段的 headless 子集）。</summary>
public sealed class DropItemFxState
{
    public bool ShowFlash;
    public int FlashStep;
    public uint FlashTime, FlashStepTime;
    public int ItemEffectFrame;
    public uint ItemEffectTick;
    public int ValueItemEffectFrame;
    public uint ValueItemEffectTick;
}

/// <summary>绘制判定产物（GameCanvas.Draw/DrawBlend 的 headless 镜像）。</summary>
public sealed record DropItemDrawOp(int X, int Y, FxImage Image, bool Blend);

/// <summary>
/// PlayScn.pas 掉落物特效族 headless 化：ClearDropItem 闪烁节拍（735-820）、
/// DrawDropItemEffect（2383-2430）、DrawDropItemValueEffect（2431-2466）、ShowItemName 名字定位（2470 起）。
/// MShare.pas 108-110：UNITY=32、HALFX=24、HALFY=16；g_dwDropItemFlashTime 缺省 5×1000（MShare 2137）。
/// </summary>
public static class DropItemFx
{
    public const int Unity = 32;
    public const int HalfX = 24;
    public const int HalfY = 16;
    public const uint DefaultFlashInterval = 5 * 1000;   // g_dwDropItemFlashTime
    public const uint SpecialFlashInterval = 300;        // 特殊物品快闪 piaoyun 2013-09-10
    public const int FlashStepMs = 20;
    public const int FlashSteps = 10;                    // HZQ 20230524

    /// <summary>ClearDropItem 闪烁节拍：窗口超时 → 开闪并清零步进；每 20ms 一步；≥10 步关闪。</summary>
    public static void UpdateFlash(DropItemFxState st, uint now, uint interval = DefaultFlashInterval)
    {
        if (now - st.FlashTime > interval)
        {
            st.FlashTime = now;
            st.ShowFlash = true;
            st.FlashStepTime = now;
            st.FlashStep = 0;
        }

        if (st.ShowFlash)
        {
            if (now - st.FlashStepTime >= FlashStepMs)
            {
                st.FlashStepTime = now;
                st.FlashStep++;
            }
            if (st.FlashStep < FlashSteps)
            {
                // 闪烁保持
            }
            else
            {
                st.ShowFlash = false;
            }
        }
    }

    /// <summary>特殊物品判定：ShowItem.boShowSpecial 且 ckSpecialQuickFlashing → 300ms 窗口。</summary>
    public static uint FlashInterval(bool quickFlashChecked, bool showSpecial)
        => quickFlashChecked && showSpecial ? SpecialFlashInterval : DefaultFlashInterval;

    /// <summary>
    /// DrawDropItemEffect（2383-2430）1:1：五重守卫 → 帧推进（Time 周期）→ 帧区间钳制回卷 →
    /// DrawCenter 居中或原点偏移 → NoBlend 分流。返回 null = 不绘（守卫截停）。
    /// </summary>
    public static DropItemDrawOp? PlanItemEffect(
        DropItemEffectDef eff, DropItemFxState st, uint now,
        bool hideItemEffectChecked, bool hideItemEffect,
        bool isBelowItem, bool selfDead,
        int texW, int texH, int nX, int nY,
        int effectImageCount, Func<int, FxImage?> resolve)
    {
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

        if (now - st.ItemEffectTick >= (uint)eff.Time)
        {
            st.ItemEffectFrame++;
            st.ItemEffectTick = now;
        }

        if (st.ItemEffectFrame < eff.StartIndex)
            st.ItemEffectFrame = eff.StartIndex;
        else if (st.ItemEffectFrame >= eff.StartIndex + eff.ImageCount)
            st.ItemEffectFrame = eff.StartIndex;

        var d = resolve(st.ItemEffectFrame);
        if (d == null)
            return null;

        if (eff.DrawCenter)
        {
            nX = nX + eff.OffsetX + (texW - d.Width) / 2;
            nY = nY + eff.OffsetY + (texH - d.Height) / 2;
        }
        else
        {
            nX = nX + eff.OffsetX + d.OriginX;
            nY = nY + eff.OffsetY + d.OriginY;
        }

        return new DropItemDrawOp(nX, nY, d, !eff.NoBlend);
    }

    /// <summary>
    /// DrawDropItemValueEffect（2431-2466）1:1：五道配置门 + boValueItem → 帧推进回卷（Count 环）→
    /// 图号 = 帧 + ValueItemEffectIndex → 恒 Draw（无混合）。
    /// </summary>
    public static DropItemDrawOp? PlanValueItemEffect(
        DropItemFxState st, uint now,
        bool showDropValueItemEff, bool showValueItemEffectChecked, bool showValueItemEffect,
        int valueItemCount, int valueItemPlayTime, int valueItemEffectIndex,
        int offsetX, int offsetY,
        bool boValueItem, bool selfDead,
        int nX, int nY,
        Func<int, FxImage?> resolve)
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
        if (!boValueItem)
            return null;

        if (now - st.ValueItemEffectTick >= (uint)valueItemPlayTime)
        {
            st.ValueItemEffectFrame++;
            st.ValueItemEffectTick = now;
        }

        if (st.ValueItemEffectFrame < 0)
            st.ValueItemEffectFrame = 0;
        else if ((uint)st.ValueItemEffectFrame >= (uint)valueItemCount)
            st.ValueItemEffectFrame = 0;

        var d = resolve(st.ValueItemEffectFrame + valueItemEffectIndex);
        if (d == null)
            return null;

        return new DropItemDrawOp(nX + offsetX + d.OriginX, nY + offsetY + d.OriginY, d, false);
    }

    /// <summary>ShowItemName（2470-2500）：名字居中定位 nX = X + HALFX − w div 2、nY = Y + HALFY − h×2。</summary>
    public static (int X, int Y) NamePos(int x, int y, int width, int height)
        => (x + HalfX - width / 2, y + HalfY - height * 2);
}

/// <summary>点表项（TPointDropItemList headless：同点物品有序表 + 最多 3 个绘制位）。</summary>
public sealed class PointDropItemList
{
    public int PointX, PointY;
    public List<DropItem> Items { get; } = new();
    public List<DropItem> DrawItems { get; } = new();

    public PointDropItemList(int x, int y)
    {
        PointX = x;
        PointY = y;
    }

    /// <summary>RefreshDrawList（DropItemsMgr.pas 192-242）：显示名单（ShowItem.boShowName 且 Visible）优先取 3 个，不足用可见项补足。</summary>
    public void RefreshDrawList()
    {
        DrawItems.Clear();
        foreach (var item in Items)
        {
            bool named = item.ShowItem != null && item.Visible && item.ShowItem.ShowName;
            if (named)
            {
                if (DrawItems.Count < 3)
                    DrawItems.Add(item);
                else
                    item.ClearNameImage();
            }
            else
            {
                item.ClearNameImage();
            }
        }
        if (DrawItems.Count >= 3)
            return;
        if (Items.Count == 0)
            return;
        foreach (var item in Items)
        {
            bool found = false;
            foreach (var d in DrawItems)
            {
                if (ReferenceEquals(d, item))
                {
                    found = true;
                    break;
                }
            }
            if (!found && item.Visible)
                DrawItems.Add(item);
            if (DrawItems.Count >= 3)
                break;
        }
    }
}

/// <summary>pTShowItem 的 headless 镜像（boShowName/boShowSpecial 判定位）。</summary>
public sealed class ShowItemInfo
{
    public bool ShowName;
    public bool ShowSpecial;
}

/// <summary>TDropItem record 的 headless 镜像（特效/闪烁相关字段）。</summary>
public sealed class DropItem
{
    public int Id;
    public int X, Y;
    public bool Visible;
    public bool ShowName;
    public string DBName = "";
    public string Name = "";
    public int OverlapCount;
    public bool ValueItem;
    public ShowItemInfo? ShowItem;
    public int TextureWidth, TextureHeight;

    /// <summary>g_WDnItemImages 下标（Delphi DropItem.looks）。</summary>
    public int Looks;

    /// <summary>ItemTexture 引用（Finalize 清空；headless 以尺寸对象占位）。</summary>
    public SurfaceSize? ItemTexture;

    public DropItemEffectDef ItemEffect;
    public DropItemFxState Fx { get; } = new();

    // ---- 批次J73：ClearDropItem 直接引用的闪烁字段（镜像到 Fx，保持 Delphi 字段名） ----
    public bool ShowFlash { get => Fx.ShowFlash; set => Fx.ShowFlash = value; }
    public int FlashStep { get => Fx.FlashStep; set => Fx.FlashStep = value; }
    public uint FlashTime { get => Fx.FlashTime; set => Fx.FlashTime = value; }
    public uint FlashStepTime { get => Fx.FlashStepTime; set => Fx.FlashStepTime = value; }

    /// <summary>NameImageInfo（位图字体产物：宽高；CurrentFont.GetImageInfo 的 headless 镜像）。</summary>
    public int NameImageWidth { get; private set; }
    public int NameImageHeight { get; private set; }

    /// <summary>名字图是否已生成（Length(NameImageInfo.ImageIndexs) &gt; 0 的等效位）。</summary>
    public bool NameImageGenerated { get; private set; }

    /// <summary>CurrentFont.GetImageInfo 结果落位。</summary>
    public void SetNameImage(int width, int height)
    {
        NameImageWidth = width;
        NameImageHeight = height;
        NameImageGenerated = true;
        NameImageCleared = false;
    }

    public void ClearNameImage()
    {
        // NameImageInfo 清空（headless 无位图字体，仅语义位）
        NameImageWidth = 0;
        NameImageHeight = 0;
        NameImageGenerated = false;
        NameImageCleared = true;
    }

    public bool NameImageCleared { get; private set; }
}

/// <summary>
/// TDropItemsMgr（246-560）headless 镜像：按 id 索引 + 按 (Y,X) 有序点表；
/// AddDropItem 绑定 g_DropItemEffectList[EffectIndex] 并头部插入；DelDropItem 仅置 Visible=false 后移除。
/// </summary>
public class DropItemsStore
{
    private readonly Dictionary<int, DropItem> _byId = new();
    private readonly List<PointDropItemList> _points = new();
    private readonly Func<int, DropItemEffectDef?> _effectResolver;
    private readonly Func<uint> _now;

    public DropItemsStore(Func<int, DropItemEffectDef?> effectResolver, Func<uint>? now = null)
    {
        _effectResolver = effectResolver;
        _now = now ?? (() => 0);
    }

    public IReadOnlyList<PointDropItemList> Points => _points;
    public IReadOnlyDictionary<int, DropItem> ById => _byId;

    private static int PointKey(int x, int y) => unchecked((y << 16) | (x & 0xFFFF)); // MakeLong 语义：X 低位、Y 高位

    public PointDropItemList? GetPointList(int x, int y)
    {
        int key = PointKey(x, y);
        foreach (var p in _points)
        {
            if (PointKey(p.PointX, p.PointY) == key)
                return p;
        }
        return null;
    }

    public DropItem? GetItemById(int id) => _byId.TryGetValue(id, out var it) ? it : null;

    /// <summary>AddDropItem（432-540）语义 1:1：特效命中 → 拷贝特效 + 帧置 StartIndex + 头部插入；否则 FileIndex=-1 尾插。</summary>
    public DropItem AddDropItem(int id, int x, int y, string dbName, int effectIndex)
    {
        var item = new DropItem
        {
            Id = id,
            X = x,
            Y = y,
            Visible = true,
            DBName = dbName,
        };

        _byId[id] = item;

        var list = GetPointList(x, y);
        if (list == null)
        {
            list = new PointDropItemList(x, y);
            _points.Insert(InsertIndex(x, y), list);
        }

        DropItemEffectDef? eff = null;
        if (effectIndex > 0)
            eff = _effectResolver(effectIndex);

        if (eff != null)
        {
            item.ItemEffect = eff.Value;
            item.Fx.ItemEffectFrame = eff.Value.StartIndex;
            item.Fx.ItemEffectTick = _now();
            item.Fx.FlashTime = _now();
            item.Fx.ShowFlash = false;
            item.Fx.FlashStepTime = _now();
            item.Fx.FlashStep = 0;
        }
        else
        {
            item.ItemEffect = DropItemEffectDef.None();
        }

        if (eff != null && list.Items.Count > 0)
            list.Items.Insert(0, item);
        else
            list.Items.Add(item);

        return item;
    }

    /// <summary>DelDropItem（545-580）语义：Visible=false + 点表/索引移除；返回该项（无 → null）。</summary>
    public DropItem? DelDropItem(int id)
    {
        if (!_byId.Remove(id, out var item))
            return null;
        item.Visible = false;

        var list = GetPointList(item.X, item.Y);
        if (list != null)
        {
            for (int i = 0; i < list.Items.Count; i++)
            {
                if (list.Items[i].Id == id)
                {
                    list.Items.RemoveAt(i);
                    break;
                }
            }
            if (list.Items.Count == 0)
                _points.Remove(list);
        }
        return item;
    }

    /// <summary>PointSearch 有序性：按 (Y&lt;&lt;16|X) 升序插入位。</summary>
    private int InsertIndex(int x, int y)
    {
        int key = PointKey(x, y);
        int idx = 0;
        foreach (var p in _points)
        {
            if (PointKey(p.PointX, p.PointY) < key)
                idx++;
            else
                break;
        }
        return idx;
    }
}
