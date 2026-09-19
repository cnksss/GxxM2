using System;
using System.Collections.Generic;

namespace GXX.Client.Scenes;

/// <summary>TGameImages 的 headless 镜像：按 Looks 取图（null = 无此图）。</summary>
public interface IFengHaoImageSource
{
    /// <summary>GetCachedImage(Looks, dx, dy)。</summary>
    LabelSurface? GetCachedImage(int looks);

    /// <summary>GetCachedGrayImage(Looks, dx, dy)。</summary>
    LabelSurface? GetCachedGrayImage(int looks);
}

/// <summary>
/// TActor.LoadFengHaoSurface（9563-9607）的 TActorCore 落地（批次J78）——封号（称号）纹理装载。
/// </summary>
public partial class TActorCore
{
    /// <summary>g_ClientConfig.nTitleFileIndex（&lt; 0 或越界即不显示封号图案）。</summary>
    public static int ClientConfig_nTitleFileIndex = -1;

    /// <summary>g_EffectImageList（按索引取 TGameImages；越界返回 null）。</summary>
    public static Func<int, IFengHaoImageSource?> EffectImageListGetter = _ => null;

    /// <summary>g_EffectImageList.Count。</summary>
    public static Func<int> EffectImageListCountFn = () => 0;

    /// <summary>m_nOldFengHaoSurfaceID（上一轮 m_nActiveFengHaoID，供差异检测）。</summary>
    public int m_nOldFengHaoSurfaceID;

    /// <summary>m_dwLoadFengHaoSurfaceTime（成功取到图案时的打点）。</summary>
    public uint m_dwLoadFengHaoSurfaceTime;

    /// <summary>
    /// LoadFengHaoSurface 1:1（9563-9607）。
    ///
    /// 原文顺序至关重要：
    ///   0. **无任何守卫**先做三件事：清空 m_FengHaoImageInfo、记录 m_nOldFengHaoSurfaceID、
    ///      把 m_FengHaoEffectSurface 置 nil（9568-9570）
    ///   1. 画布未就绪 → Exit
    ///   2. 骑马（m_btHorse ∈ [1,2]）且非双人骑被邀请人（m_btDoubleHumHorse = 0）→ Exit
    ///   3. m_btActiveFengHaoReserved = 2 → 再次置 nil 后 Exit
    ///   4. 摆摊 → 再次置 nil 后 Exit
    ///   5. 字体可用且封号名非空且 Reserved = 0 → 追加一个封号文字纹理
    ///   6. nTitleFileIndex &lt; 0 → Exit（**在步骤 5 之后**，故已装入的文字纹理被保留）
    ///   7. nTitleFileIndex 越界 → Exit（同上）
    ///   8. 取图源非 nil 时按 m_boDeath 取灰图/彩图，成功后打点 m_dwLoadFengHaoSurfaceTime
    ///
    /// 要点：步骤 2-4 的 Exit 发生在步骤 5 之前，因此这三条路径下 m_FengHaoImageInfo 恒为空；
    ///       而步骤 6-7 的 Exit 在步骤 5 之后，文字纹理已被装入并保留。
    /// </summary>
    public void LoadFengHaoSurface()
    {
        // ---- 9568-9570：先无条件重置（早于一切守卫） ----
        FengHaoImageInfo.Clear();
        m_nOldFengHaoSurfaceID = m_nActiveFengHaoID;
        m_FengHaoEffectSurface = null;

        if (!CanvasReadyFn())
            return;

        // 骑马（非双人骑被邀请人）不显示封号
        if ((m_btHorse == 1 || m_btHorse == 2) && m_btDoubleHumHorse == 0)
            return;

        // Reserved = 2 → 不显示封号
        if (m_btActiveFengHaoReserved == 2)
        {
            m_FengHaoEffectSurface = null;
            return;
        }

        // 摆摊时不显示称号
        if (m_boShopStall)
        {
            m_FengHaoEffectSurface = null;
            return;
        }

        // ---- 9589-9594：封号文字纹理 ----
        if (CurrentFontAvailableFn())
        {
            if (m_sActiveFengHaoName != "" && m_btActiveFengHaoReserved == 0)
            {
                var info = GetImageInfoFn(m_sActiveFengHaoName);
                if (info != null)
                    FengHaoImageInfo.Add(info);
            }
        }

        // ---- 9596-9597：图案索引守卫（在文字纹理装载之后） ----
        if (ClientConfig_nTitleFileIndex < 0)
            return;
        if (ClientConfig_nTitleFileIndex >= EffectImageListCountFn())
            return;

        var images = EffectImageListGetter(ClientConfig_nTitleFileIndex);
        if (images != null)
        {
            m_FengHaoEffectSurface = m_boDeath
                ? images.GetCachedGrayImage(m_dwActiveFengHaoLooks)
                : images.GetCachedImage(m_dwActiveFengHaoLooks);
            m_dwLoadFengHaoSurfaceTime = (uint)MyGetTickCountFn();
        }
    }
}
