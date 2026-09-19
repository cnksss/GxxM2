using System;

namespace GXX.Client.Scenes;

/// <summary>Actor.pas TColorEffect（SDK.pas 363 声明序 1:1：ceGrayScale2 为末位 13）。</summary>
public enum TColorEffect
{
    ceNone = 0,
    ceGrayScale = 1,
    ceBright = 2,
    ceBlack = 3,
    ceWhite = 4,
    ceRed = 5,
    ceGreen = 6,
    ceBlue = 7,
    ceYellow = 8,
    ceFuchsia = 9,
    ceAqua = 10,
    ceSilver = 11,
    ceGray = 12,
    ceGrayScale2 = 13,
}

/// <summary>LoadSurface/DrawChr 规划结果：图库标识 + 图号 + 色彩模式（headless 无纹理本体）。</summary>
public struct ActorBodyImage
{
    public int LibraryId;   // 图库（EN: g_WMonImages 下标 / 自定义怪 ActionFile → g_EffectImageList 下标）
    public int ImageIndex;  // 最终图号
    public TColorEffect Color;
    public bool UseEffectImageList; // true: 图库取 g_EffectImageList[LibraryId]；false: g_WMonImages[LibraryId]
}

/// <summary>
/// Actor.pas TActor.LoadSurface（5480-5593）+ DrawChr（6067-6129）headless 规划：
/// 怪物主体图 = g_WMonImages[外观] 中 GetOffset(外观)+m_nCurrentFrame；
/// m_boReverseFrame 反向播放 = GetOffset+EndFrame-(CurrentFrame-StartFrame)；
/// race 156 自定义怪按 ClientAction：ActionFile 合法 → g_EffectImageList[ActionFile]（图号=当前帧），
/// 否则 g_WMonImages[ChangeAppr-100000]；色彩效果 ceGrayScale/ceGrayScale2/ceBright 变体；
/// DrawChr 绘制位置 = dx+m_nPx+m_nShiftX, dy+m_nPy+m_nShiftY（方向 0..7 守卫）。
/// </summary>
public static class ActorDrawPlan
{
    public static ActorBodyImage? PlanBodyImage(int race, int appearance, int changeAppr, int currentAction,
        int currentFrame, int startFrame, int endFrame, bool reverseFrame, TColorEffect color,
        Func<int, int, (int StartIndex, int PlayCount, int ActionFile)?>? customActionResolver)
    {
        if (race == 156 && changeAppr >= 0)
        {
            var action = customActionResolver?.Invoke(currentAction, changeAppr);
            if (action == null || action.Value.StartIndex < 0 || action.Value.PlayCount <= 0)
                return null;
            var result = new ActorBodyImage { ImageIndex = currentFrame, Color = color };
            if (action.Value.ActionFile >= 0)
            {
                result.LibraryId = action.Value.ActionFile;
                result.UseEffectImageList = true;
            }
            else
            {
                result.LibraryId = changeAppr - 100000;
            }
            return result;
        }

        int index = reverseFrame
            ? ActorOffsets.GetOffset(appearance) + endFrame - (currentFrame - startFrame)
            : ActorOffsets.GetOffset(appearance) + currentFrame;
        return new ActorBodyImage { LibraryId = appearance, ImageIndex = index, Color = color };
    }

    /// <summary>DrawChr 方向守卫（6073：m_btDir 不在 0..7 直接退出）。</summary>
    public static bool CanDrawDir(int dir) => dir is >= 0 and <= 7;

    /// <summary>DrawChr → DrawEffSurface 绘制位置（6076-6081）。</summary>
    public static (int X, int Y) DrawChrPosition(int dx, int dy, int px, int py, int shiftX, int shiftY)
        => (dx + px + shiftX, dy + py + shiftY);

    /// <summary>死亡态主体取灰度图（DrawChr 6105-6108：g_MySelf 死亡 → GetCachedGrayImage）。</summary>
    public static TColorEffect EffectiveColor(TColorEffect color, bool viewerDead)
        => viewerDead ? TColorEffect.ceGrayScale : color;
}
