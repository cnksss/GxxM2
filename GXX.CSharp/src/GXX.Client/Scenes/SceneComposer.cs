using System;
using System.Collections.Generic;

namespace GXX.Client.Scenes;

/// <summary>
/// PlayScn.pas DrawScene（2472-3234）场景合成调度核心（批次J47，headless 化）：
/// 行主序遍历——行 J 从 ClientRect.Top-BlockTop 到 Bottom-BlockTop+LONGHEIGHT_IMAGE(32)，
/// 负行跳过（像素 m 仍按行推进）；每行像素 m 自 m_nDefYY-UNITY 起、行尾 +UNITY；
/// 地图对象（EI 图分支）：fridx≠65535 且 0&lt;FileIdx&lt;75 才绘制，五动画文件（11/26/41/56/71）
/// 按 btAni 帧计数 fridx 步进（含 ani&amp;$80 混合位），非 48×32 图块 mmm=m+UNITY-高（混合 mmm=m+PtY-68）；
/// 角色行调度（3061-3202）：按 m_nRy 排序的 actor 列表在 J == m_nRy-BlockTop-m_nDownDrawLevel 行绘制，
/// 绘制坐标 nX=(Rx-ClientRect.Left)×UNITX+m_nDefXX+ShakeX，nY=m+DownDrawLevel×UNITY+ShakeY。
/// </summary>
public sealed class SceneComposer
{
    public struct MapObjectCell
    {
        public int FileIdx1;   // btFileIdx1
        public int Obj2;       // wObj2
        public int Obj2Ani;    // btObj2Ani
    }

    public struct SceneActorInput
    {
        public long RecogId;
        public int Rx;              // m_nRx（绘制 X 基）
        public int Ry;              // m_nRy（行匹配基：J == Ry-BlockTop-DownDrawLevel）
        public int DownDrawLevel;   // m_nDownDrawLevel
        public int ShiftX;
        public int ShiftY;
        public bool HideGhost;      // 隐尸（原文隐藏尸体插件/选项合并后仍绘制的守卫由上层处理）
    }

    public struct SceneDrawOp
    {
        public enum OpKind { MapObject, Actor }
        public OpKind Kind;
        public int RowJ;        // 调度行
        public int PixelX;
        public int PixelY;
        public int FileIdx;     // 地图对象：图库号
        public int ImageIndex;  // 地图对象：图号
        public bool Blend;      // 地图对象：混合绘制
        public long ActorId;    // 角色：RecogId
    }

    public int ClientLeft, ClientTop, ClientRight, ClientBottom;
    public int BlockLeft, BlockTop;
    public int DefXX, DefYY;
    public int ShakeX, ShakeY;
    public int AniCount;
    public bool CanDraw = true;
    public bool MapLoadOk = true;

    /// <summary>地图单元格对象数据接缝（m_MArrEIMapInfo[I,J] 等效；EI 关闭时可返回空）。</summary>
    public Func<int, int, MapObjectCell>? ObjectCell;

    /// <summary>图块尺寸探测（GetCachedImageSize：宽/高；返回 null=未就绪）。</summary>
    public Func<int, int, (int Width, int Height)?>? ObjectTextureProbe;

    /// <summary>按 m_nRy 升序的角色列表（DrawScene 前置 DoSearchSortYDrawActtor 排序）。</summary>
    public List<SceneActorInput> SortedActors = new();

    private static int DelphiRound(double value) => (int)Math.Round(value, MidpointRounding.ToEven);

    /// <summary>
    /// DrawScene 行主序调度 1:1：先按行推进（负行只推进 m 不产出），行内先产出地图对象
    /// （对象高非 48×32 时 mmm=m+UNITY-高，混合时 mmm=m+PtY-68），再产出该行的角色
    /// （J == Ry-BlockTop-DownDrawLevel；nX=(Rx-Left)×UNITX+DefXX+ShakeX，nY=m+Level×UNITY+ShakeY）。
    /// </summary>
    public List<SceneDrawOp> ComposeSchedule()
    {
        var ops = new List<SceneDrawOp>();
        if (!CanDraw || !MapLoadOk)
            return ops;

        int m = DefYY - UNITY;
        for (int j = ClientTop - BlockTop; j <= ClientBottom - BlockTop + LONGHEIGHT_IMAGE; j++)
        {
            if (!CanDraw)
                break;
            if (j < 0)
            {
                m += UNITY;
                continue;
            }

            int n = DefXX - UNITX * 2;
            for (int i = ClientLeft - BlockLeft - 2; i <= ClientRight - BlockLeft + 2; i++)
            {
                if (!CanDraw)
                    break;
                if (MapLoadOk && i >= 0 && i < LOGICALMAPUNIT * 3 && j >= 0 && j < LOGICALMAPUNIT * 3)
                {
                    var cell = ObjectCell?.Invoke(i, j);
                    if (cell != null)
                    {
                        // wObj2 对象（DrawScene 2650-2733 同构）
                        int fileIdx = cell.Value.FileIdx1;
                        int fridx = cell.Value.Obj2;
                        if (fridx != 65535 && fileIdx < 75 && fileIdx > 0)
                        {
                            bool blend = false;
                            if (fileIdx is 11 or 26 or 41 or 56 or 71)
                            {
                                int ani = cell.Value.Obj2Ani;
                                if (ani != 255 && ani > 0)
                                {
                                    if ((ani & 0x80) > 0)
                                        blend = true;
                                    switch (fileIdx)
                                    {
                                        case 11:
                                            fridx += ani > 10 ? (AniCount / 2 % 5) : (AniCount / 2 % ani);
                                            break;
                                        case 26:
                                            fridx += ani == 190 ? (AniCount / 2 % 14) : (AniCount / 2 % 10);
                                            break;
                                        case 41:
                                            fridx += ani == 184 ? (AniCount / 2 % 16) : (AniCount / 2 % 10);
                                            break;
                                        case 56:
                                            fridx += ani >= 136 ? (AniCount / 2 % 10)
                                                : ani >= 69 ? (AniCount / 2 % 5)
                                                : (AniCount / 2 % 6);
                                            break;
                                        case 71:
                                            fridx += AniCount / 2 % 6;
                                            break;
                                    }
                                }
                            }

                            var size = ObjectTextureProbe?.Invoke(fileIdx, fridx);
                            if (size != null && size.Value.Width * size.Value.Height > 4 && !blend)
                            {
                                // 非 48×32 的立式对象：mmm = m + UNITY - 高
                                if ((size.Value.Width != 48 || size.Value.Height != 32) && size.Value.Width > 0)
                                {
                                    int mmm = m + UNITY - size.Value.Height + ShakeY;
                                    if (n + ShakeX + size.Value.Width >= 0 && n + ShakeX <= SCREENWIDTH &&
                                        mmm + size.Value.Height >= 0 && mmm < MAPSURFACEHEIGHT)
                                    {
                                        ops.Add(new SceneDrawOp
                                        {
                                            Kind = SceneDrawOp.OpKind.MapObject,
                                            RowJ = j,
                                            PixelX = n + ShakeX,
                                            PixelY = mmm,
                                            FileIdx = fileIdx,
                                            ImageIndex = fridx,
                                            Blend = false,
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
                n += UNITX;
            }

            // 角色行（3064-3202）：J == m_nRy - BlockTop - DownDrawLevel
            foreach (var actor in SortedActors)
            {
                if (j == actor.Ry - BlockTop - actor.DownDrawLevel)
                {
                    int nX = (actor.Rx - ClientLeft) * UNITX + DefXX + ShakeX + actor.ShiftX;
                    int nY = m + actor.DownDrawLevel * UNITY + ShakeY + actor.ShiftY;
                    ops.Add(new SceneDrawOp
                    {
                        Kind = SceneDrawOp.OpKind.Actor,
                        RowJ = j,
                        PixelX = nX,
                        PixelY = nY,
                        ActorId = actor.RecogId,
                    });
                }
            }

            m += UNITY;
        }
        return ops;
    }

    // ---- 常量（PlayScn/MShare/SDK 对齐） ----
    public const int UNITX = 48;
    public const int UNITY = 32;
    public const int LOGICALMAPUNIT = 40;
    public const int LONGHEIGHT_IMAGE = 32;
    public const int SCREENWIDTH = 1024;
    public const int MAPSURFACEHEIGHT = 768;
}
