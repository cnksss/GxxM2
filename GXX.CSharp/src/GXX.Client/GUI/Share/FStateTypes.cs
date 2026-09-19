using System;
using System.Globalization;
using GXX.Client.GUI.DxComponent;
using GXX.Core.Protocol;
using GXX.Core.Util;
using static GXX.Client.GUI.Mir.MShareGlobals;

namespace GXX.Client.GUI.Share;

// ============================================================================================
// FState.pas `type` 段（原文 59-1114 行）中除 TFrmDlg 之外的 24 个类型。
//
// 译文规则（docs/转换开发文档.md §3）：
//   * 类型名保留 T 前缀；字段名、方法名、常量名、分支顺序、边界行为逐字照抄。
//   * 原文笔误保留并注释 `// 原文如此（FState.pas:NNNN）`。
//   * Delphi 的 Free 在托管侧是 GC，故析构体保留同名 Destroy() 方法以维持调用点结构。
//   * 四个容器类（TGuildGroup / TGuildGroupList / TShowGuildList / TGuildJoinUserList）与
//     13 个 NPC 控件类给出**完整实现**；控件类继承自车道1 的 GXX.Client.GUI.DxComponent 接缝。
// ============================================================================================

/// <summary>FState.pas:64 TSpotDlgMode = (dmSell, dmRepair, dmStorage, dmUpgrade, dmPleaseDrink)。</summary>
public enum TSpotDlgMode
{
    /// <summary>dmSell</summary>
    dmSell = 0,
    /// <summary>dmRepair</summary>
    dmRepair = 1,
    /// <summary>dmStorage</summary>
    dmStorage = 2,
    /// <summary>dmUpgrade</summary>
    dmUpgrade = 3,
    /// <summary>dmPleaseDrink</summary>
    dmPleaseDrink = 4,
}

/// <summary>
/// FState.pas:66 TDiceInfo（骰子/猜拳动画的单颗骰子状态）。
/// 原文注释保留了原始内存偏移（0x66C..0x684），此处逐字保留。
/// </summary>
public struct TDiceInfo
{
    /// <summary>nDicePoint（0x66C）</summary>
    public int nDicePoint;

    /// <summary>nPlayPoint（0x670 当前骰子点数）</summary>
    public int nPlayPoint;

    /// <summary>nX（0x674）</summary>
    public int nX;

    /// <summary>nY（0x678）</summary>
    public int nY;

    /// <summary>nRunIndex（0x67C）</summary>
    public int nRunIndex;

    /// <summary>nRunCount（0x680）</summary>
    public int nRunCount;

    /// <summary>dwPlayTick（0x684）</summary>
    public uint dwPlayTick;
}

// ============================================================================================
// 行会分组容器（原文 23887-23993 行实现，纯数据、无 UI 依赖）
// ============================================================================================

/// <summary>FState.pas:78 TGuildGroup = class(TObject)。</summary>
public class TGuildGroup
{
    private int FID;
    private string FName;
    private readonly TStringList FMembers = new();

    /// <summary>FState.pas:23887 constructor TGuildGroup.Create(AID:Integer; AName:string)。</summary>
    public TGuildGroup(int AID, string AName)
    {
        FID = AID;
        FName = AName;
        // 原文 FMembers := TStringList.Create; 此处为字段初始化，语义等价。
    }

    /// <summary>
    /// FState.pas:23895 destructor TGuildGroup.Destroy。
    /// 原文 Clear 后 Free（HZQ 20250525 追踪打开行会对化框的内存泄漏）；托管侧只需 Clear。
    /// </summary>
    public virtual void Destroy()
    {
        FMembers.Clear();
    }

    /// <summary>原文 property ID:Integer read FID。</summary>
    public int ID => FID;

    /// <summary>原文 property Name:string read FName write FName。</summary>
    public string Name
    {
        get => FName;
        set => FName = value;
    }

    /// <summary>原文 property Members:TStringList read FMembers。</summary>
    public TStringList Members => FMembers;
}

/// <summary>FState.pas:91 TGuildGroupList = class(TObject)（行会成员分组列表）。</summary>
public class TGuildGroupList
{
    private readonly TList FList = new();

    /// <summary>FState.pas:23904 constructor TGuildGroupList.Create。</summary>
    public TGuildGroupList()
    {
    }

    /// <summary>FState.pas:23910 destructor TGuildGroupList.Destroy（Clear 后释放列表）。</summary>
    public virtual void Destroy()
    {
        Clear();
    }

    /// <summary>FState.pas:23917 function TGuildGroupList.Add(AID:Integer; AName:string):TGuildGroup。</summary>
    public TGuildGroup Add(int AID, string AName)
    {
        TGuildGroup Result = new(AID, AName);
        FList.Add(Result);
        return Result;
    }

    /// <summary>FState.pas:23923 procedure TGuildGroupList.Clear（逐个 Free 后清空）。</summary>
    public void Clear()
    {
        for (int I = 0; I <= FList.Count - 1; I++)
        {
            ((TGuildGroup)FList[I]).Destroy();
        }
        FList.Clear();
    }

    /// <summary>FState.pas:23933 function TGuildGroupList.DeleteGroupByName(AName:string):Boolean。</summary>
    public bool DeleteGroupByName(string AName)
    {
        bool Result = false;
        int Index = FindGroupIndex(AName);
        if (Index >= 0)
        {
            ((TGuildGroup)FList[Index]).Destroy();
            FList.Delete(Index);
            Result = true;
        }
        return Result;
    }

    /// <summary>
    /// FState.pas:23946 function TGuildGroupList.DeleteGroupByIndex(Index:Integer):Boolean。
    /// 上界判据是 `Index &lt;= FList.Count - 1`（与 GetGroups 一致），越界返回 False。
    /// </summary>
    public bool DeleteGroupByIndex(int Index)
    {
        bool Result;
        if ((Index >= 0) && (Index <= FList.Count - 1))
        {
            ((TGuildGroup)FList[Index]).Destroy();
            FList.Delete(Index);
            Result = true;
        }
        else
        {
            Result = false; // 原文如此（FState.pas:23952）：HZQ 20250525
        }
        return Result;
    }

    /// <summary>FState.pas:23957 function TGuildGroupList.FindGroup(AName:string):TGuildGroup。</summary>
    public TGuildGroup FindGroup(string AName)
    {
        int Index = FindGroupIndex(AName);
        if (Index >= 0)
            return (TGuildGroup)FList[Index];
        return null; // 原文 Result := nil
    }

    /// <summary>
    /// FState.pas:23968 function TGuildGroupList.FindGroupIndex(AName:string):Integer。
    /// 用 SameText（不区分大小写）比较分组名；未找到返回 -1。
    /// </summary>
    public int FindGroupIndex(string AName)
    {
        int Result = -1;
        for (int I = 0; I <= FList.Count - 1; I++)
        {
            // Delphi SameText：不区分大小写比较（此处按序数忽略大小写对齐，见交付报告的语义说明）
            if (string.Equals(((TGuildGroup)FList[I]).Name, AName, StringComparison.OrdinalIgnoreCase))
            {
                Result = I;
                break;
            }
        }
        return Result;
    }

    /// <summary>FState.pas:23981 function TGuildGroupList.GetCount:Integer。</summary>
    public int Count => FList.Count;

    /// <summary>FState.pas:23986 function TGuildGroupList.GetGroups(Index:Integer):TGuildGroup。</summary>
    public TGuildGroup GetGroups(int Index)
    {
        // 原文 Result := nil 起步；越界保持 nil。
        TGuildGroup Result = null;
        if ((Index >= 0) && (Index <= FList.Count - 1))
            Result = (TGuildGroup)FList[Index];
        return Result;
    }

    /// <summary>原文 property Groups[Index:Integer]:TGuildGroup read GetGroups。</summary>
    public TGuildGroup this[int Index] => GetGroups(Index);
}

// ============================================================================================
// 行会列表（原文 23995-24076 行实现，含手写快速排序）
// ============================================================================================

/// <summary>FState.pas:111 TShowGuildInfo = record（行会名 + 在线数 + 人数）。</summary>
public struct TShowGuildInfo
{
    public string GuildName;
    public int OnlineCount;
    public int Count;
}

/// <summary>FState.pas:117 TShowGuildList = class(TObject)（可排序的行会展示列表）。</summary>
public class TShowGuildList
{
    private readonly TList FList = new();

    /// <summary>FState.pas:23995 constructor TShowGuildList.Create。</summary>
    public TShowGuildList()
    {
    }

    /// <summary>FState.pas:24001 destructor TShowGuildList.Destroy（Clear 后释放列表）。</summary>
    public virtual void Destroy()
    {
        Clear();
    }

    /// <summary>
    /// FState.pas:24008 function TShowGuildList.Add(AName:string; AOnlineCount, ACount:Integer):PShowGuildInfo。
    /// 原文 New(Result) 堆分配后把字段逐个写入。
    /// </summary>
    public PShowGuildInfo Add(string AName, int AOnlineCount, int ACount)
    {
        PShowGuildInfo Result = new();
        FList.Add(Result);
        Result.Value.GuildName = AName;
        Result.Value.OnlineCount = AOnlineCount;
        Result.Value.Count = ACount;
        return Result;
    }

    /// <summary>FState.pas:24017 procedure TShowGuildList.Clear（逐个 Dispose 后清空）。</summary>
    public void Clear()
    {
        for (int I = 0; I <= FList.Count - 1; I++)
        {
            // 原文 Dispose(PShowGuildInfo(FList.Items[I]))；托管侧由 GC 回收。
        }
        FList.Clear();
    }

    /// <summary>FState.pas:24027 function TShowGuildList.GetCount:Integer。</summary>
    public int Count => FList.Count;

    /// <summary>
    /// FState.pas:24032 function TShowGuildList.GetShowGuilds(Index:Integer):PShowGuildInfo。
    /// 注意上界判据是 `Index &lt; FList.Count`（严格小于），与 TGuildGroupList.GetGroups 的
    /// `&lt;= Count - 1` 写法等价，但**原文两处写法不同**，此处逐字保留。
    /// </summary>
    public PShowGuildInfo GetShowGuilds(int Index)
    {
        PShowGuildInfo Result = null;
        if ((Index >= 0) && (Index < FList.Count))
            Result = (PShowGuildInfo)FList[Index];
        return Result;
    }

    /// <summary>原文 property ShowGuild[Index:Integer]:PShowGuildInfo read GetShowGuilds。</summary>
    public PShowGuildInfo this[int Index] => GetShowGuilds(Index);

    /// <summary>
    /// FState.pas:24039 procedure TShowGuildList.DoSort。
    /// 内嵌 QuickSort 的手写实现，比较顺序为：OnlineCount 降序，其次 Count 降序。
    /// 原文对 I/J 的移动使用了 `while ... do begin Inc(I) end;`（无边界保护），
    /// 之所以安全是因为基准元素 P 必然使内层循环停下；此处逐字保留该结构。
    /// </summary>
    public void DoSort()
    {
        void QuickSort(int L, int R)
        {
            int I;
            int J;
            PShowGuildInfo P;
            do
            {
                I = L;
                J = R;
                P = (PShowGuildInfo)FList[L + (R - L) / 2]; // div：L、R 非负时截断除法
                do
                {
                    while (((PShowGuildInfo)FList[I]).Value.OnlineCount > P.Value.OnlineCount
                        || ((((PShowGuildInfo)FList[I]).Value.OnlineCount == P.Value.OnlineCount)
                            && (((PShowGuildInfo)FList[I]).Value.Count > P.Value.Count)))
                    {
                        I = I + 1;
                    }

                    while (((PShowGuildInfo)FList[J]).Value.OnlineCount < P.Value.OnlineCount
                        || ((((PShowGuildInfo)FList[J]).Value.OnlineCount == P.Value.OnlineCount)
                            && (((PShowGuildInfo)FList[J]).Value.Count < P.Value.Count)))
                    {
                        J = J - 1;
                    }

                    if (I <= J)
                    {
                        FList.Exchange(I, J);
                        I = I + 1;
                        J = J - 1;
                    }
                }
                while (I <= J);

                if (L < J)
                    QuickSort(L, J);
                L = I;
            }
            while (I < R);
        }

        if (FList.Count > 1)
            QuickSort(0, FList.Count - 1);
    }
}

// ============================================================================================
// 行会入会申请列表（原文 24078-24118 行实现）
// ============================================================================================

/// <summary>FState.pas:132 TGuildJoinUserList = class(TObject)。</summary>
public class TGuildJoinUserList
{
    private readonly TList FList = new();

    /// <summary>FState.pas:24078 constructor TGuildJoinUserList.Create。</summary>
    public TGuildJoinUserList()
    {
    }

    /// <summary>FState.pas:24084 destructor TGuildJoinUserList.Destroy。</summary>
    public virtual void Destroy()
    {
        Clear();
    }

    /// <summary>
    /// FState.pas:24091 function TGuildJoinUserList.Add(JoinUser:TGuildJoinUser):PGuildJoinUser。
    /// 原文 New(Result) 后 `Result^ := JoinUser`（整体拷贝记录）。
    /// </summary>
    public PGuildJoinUser Add(TGuildJoinUser JoinUser)
    {
        PGuildJoinUser Result = new();
        FList.Add(Result);
        Result.Value = JoinUser;
        return Result;
    }

    /// <summary>FState.pas:24098 procedure TGuildJoinUserList.Clear（注意原文该处**没有分号**，逐字保留语义）。</summary>
    public void Clear()
    {
        for (int I = 0; I <= FList.Count - 1; I++)
        {
            // 原文 Dispose(PGuildJoinUser(FList.Items[I]))；托管侧由 GC 回收。
        }
        FList.Clear();
    }

    /// <summary>FState.pas:24108 function TGuildJoinUserList.GetCount:Integer。</summary>
    public int Count => FList.Count;

    /// <summary>FState.pas:24113 function TGuildJoinUserList.GetJoinUsers(Index:Integer):PGuildJoinUser。</summary>
    public PGuildJoinUser GetJoinUsers(int Index)
    {
        PGuildJoinUser Result = null;
        if ((Index >= 0) && (Index < FList.Count))
            Result = (PGuildJoinUser)FList[Index];
        return Result;
    }

    /// <summary>原文 property JoinUsers[Index:Integer]:PGuildJoinUser read GetJoinUsers。</summary>
    public PGuildJoinUser this[int Index] => GetJoinUsers(Index);
}

// ============================================================================================
// NPC 控件族（原文 1154-1415 行实现）
// 基类来自车道1 的 GXX.Client.GUI.DxComponent 接缝。
// ============================================================================================

/// <summary>FState.pas:146 TNpcButton = class(TDxImageButton)（NPC 界面上的动画图片按钮）。</summary>
public class TNpcButton : TDxImageButton
{
    public string m_ACaption;
    public string m_sPostText;
    public string m_sCmd;
    public uint m_dwPlayImageTick;
    public uint m_dwPlayImageTime;
    public int m_nStartImageIndex;
    public int m_nStopImageCount;
    public int m_nImageIndex;
    public bool m_boBlend;
    public int m_nShowBG;
    public TPoint m_TextureOffset;

    /// <summary>FState.pas:1154 constructor TNpcButton.Create(AOwner:TDxControl)。</summary>
    public TNpcButton(TDxControl AOwner) : base(AOwner)
    {
        m_sPostText = "";
        m_sCmd = "";
        m_nStartImageIndex = 0;
        m_nStopImageCount = 0;
        m_nImageIndex = 0;

        m_dwPlayImageTick = FStateSeamClock.Now;
        m_dwPlayImageTime = 300;

        m_boBlend = false;

        m_nShowBG = 0;

        m_TextureOffset = TPoint.Point(0, 0);
    }

    /// <summary>FState.pas:1173 destructor TNpcButton.Destroy（原文只调 inherited）。</summary>
    public virtual void Destroy()
    {
        // 原文只调 inherited（无自有资源）。
    }

    /// <summary>
    /// FState.pas:1178 procedure TNpcButton.Update()。
    /// 分支顺序逐字保留：
    ///   1) (AddData1 &gt; 0) and (AddData2 &gt;= AddData1) 直接 Exit；
    ///   2) m_nStopImageCount &gt; m_nStartImageIndex 才播放；
    ///   3) 播放间隔严格大于 m_dwPlayImageTime 才推进（等于时不推进）；
    ///   4) 越界判据 `(m_nImageIndex &lt; 0) or (m_nImageIndex &gt; m_nStopImageCount)`（**上界是
    ///      m_nStopImageCount 本身而非 -1**，原文如此，等于把 stop 也算进循环）；
    ///   5) 归位时若 AddData1 &gt; 0 则 AddData2 自增。
    /// </summary>
    public virtual void Update()
    {
        // 原文 inherited（TDxImageButton.Update 的绘制推进，待 DxImageButton.pas 移植）
        if ((DxControlExt.GetAddData1(this) > 0) && (DxControlExt.GetAddData2(this) >= DxControlExt.GetAddData1(this)))
            return; // 原文 Exit

        if (m_nStopImageCount > m_nStartImageIndex)
        {
            if (FStateSeamClock.Now - m_dwPlayImageTick > m_dwPlayImageTime)
            {
                m_dwPlayImageTick = FStateSeamClock.Now;
                m_nImageIndex = m_nImageIndex + 1;
                if ((m_nImageIndex < 0) || (m_nImageIndex > m_nStopImageCount))
                {
                    m_nImageIndex = m_nStartImageIndex;

                    if (DxControlExt.GetAddData1(this) > 0)
                        DxControlExt.SetAddData2(this, DxControlExt.GetAddData2(this) + 1);
                }
                // 原文 ImageIndex.Up := m_nImageIndex;
                ImageIndex.Up = m_nImageIndex;
            }
        }
    }
}

/// <summary>FState.pas:165 TNpcGraphicButton = class(TDxImageButton)（素材来自 TGraphic 的永久位图按钮）。</summary>
public class TNpcGraphicButton : TDxImageButton
{
    private TTexture m_GraphicTexture;

    /// <summary>
    /// FState.pas:25138-25143 constructor TNpcGraphicButton.Create(AOwner:TDxControl)。
    /// 原文：inherited Create(AOwner); Self.Tag := 0;
    /// </summary>
    public TNpcGraphicButton(TDxControl AOwner) : base(AOwner)
    {
        Tag = 0;
    }

    /// <summary>FState.pas:25145-25151 destructor TNpcGraphicButton.Destroy（原文 Free 贴图后 inherited）。</summary>
    public virtual void Destroy()
    {
        // 原文 if m_GraphicTexture <> nil then m_GraphicTexture.Free; —— 托管侧由 GC 回收。
    }

    /// <summary>
    /// FState.pas:25153-25156 procedure TNpcGraphicButton.SetGraphic(AGraphic:TGraphic)。
    /// 原文：m_GraphicTexture := NewTextureFromGraphic(AGraphic);
    /// 【接缝：待 DxCanvas/HGE 的 NewTextureFromGraphic（TGraphic → TTexture）移植后接入】
    /// </summary>
    public void SetGraphic(TGraphic AGraphic)
    {
        m_GraphicTexture = NewTextureFromGraphicHandler?.Invoke(AGraphic);
    }

    /// <summary>NewTextureFromGraphic 的接缝注入点（DxCanvas/HGE 未移植）。</summary>
    public static Func<TGraphic, TTexture> NewTextureFromGraphicHandler;

    /// <summary>
    /// FState.pas:25158-25163 procedure TNpcGraphicButton.Paint。
    /// 原文：inherited; GameCanvas.Draw(VirtualRect.Left, VirtualRect.Top, m_GraphicTexture);
    /// GameCanvas 走车道1 的 MShareGlobals.TGameCanvas（可断言绘制调用）。
    /// </summary>
    public virtual void Paint()
    {
        // 原文 inherited（TDxImageButton.Paint 的自身绘制，待 DxImageButton.pas 移植）
        GXX.Client.GUI.Mir.MShareGlobals.GameCanvas.Draw(VirtualRect.Left, VirtualRect.Top, m_GraphicTexture);
    }

    /// <summary>原文 property GraphicTexture:TTexture read m_GraphicTexture write m_GraphicTexture。</summary>
    public TTexture GraphicTexture
    {
        get => m_GraphicTexture;
        set => m_GraphicTexture = value;
    }
}

/// <summary>FState.pas:178 TNpcItemButton = class(TDxImageButton)（NPC 包裹格子的物品按钮）。</summary>
public class TNpcItemButton : TDxImageButton
{
    public string m_sCmd;
    public int m_nIndex;
    public int m_nCount;
    public bool m_boShowBorder;
    public int m_nFaceIndex;
    public int m_Light;      // 发光
    public int m_nGray;      // 置灰

    public int m_LightFrameIndex;
    public uint m_LightFrameTick;

    /// <summary>FState.pas:23861 constructor TNpcItemButton.Create(AOwner:TDxControl)。</summary>
    public TNpcItemButton(TDxControl AOwner) : base(AOwner)
    {
        m_boShowBorder = false;
        m_Light = 0;
        m_LightFrameIndex = 0;
        m_LightFrameTick = 0;
    }
}

/// <summary>FState.pas:193 TNpcUserItemButton = class(TNpcItemButton)。</summary>
public class TNpcUserItemButton : TNpcItemButton
{
    /// <summary>FState.pas:23870 constructor TNpcUserItemButton.Create(AOwner:TDxControl)（原文只调 inherited）。</summary>
    public TNpcUserItemButton(TDxControl AOwner) : base(AOwner)
    {
    }
}

/// <summary>FState.pas:198 TNpcItemBoxButton = class(TDxImageButton)（自定义 OK 框按钮，无自有构造）。</summary>
public class TNpcItemBoxButton : TDxImageButton
{
    public string m_StdModes;
    public int m_nIndex;

    public TNpcItemBoxButton(TDxControl AOwner) : base(AOwner)
    {
    }
}

/// <summary>FState.pas:204 TNpcProgressBoxButton = class(TDxImageButton)（进度条按钮）。</summary>
public class TNpcProgressBoxButton : TDxImageButton
{
    public string m_sCmd;
    public int m_nBgIndex;
    public int m_nProgressStartIndex;
    public int m_nProgressIndex;
    public int m_nProgressCount;
    public uint m_nProgressRefresTick;
    public int m_nProgressRefresTime;
    public int m_nProgressOffsetX;
    public int m_nProgressOffsetY;
    public int m_nProgressMinValue;
    public int m_nProgressMaxValue;
    public int m_nProgressValue;
    public int m_nProgressDist;
    public int m_nProgressTextColor; // 原文 TColor
    public int m_nProgressTextOffsetX;
    public int m_nProgressTextOffsetY;
    public string m_sText;

    public TNpcProgressBoxButton(TDxControl AOwner) : base(AOwner)
    {
    }
}

/// <summary>FState.pas:225 TNpcLabel = class(TDxLabel)（NPC 提示标签，支持自动轮换字色）。</summary>
public class TNpcLabel : TDxLabel
{
    public string m_sPostText;
    public string m_sCmd;
    public TList m_AutoColors;
    public uint m_dwAutoColorTick;
    public int m_nColorIndex;

    /// <summary>FState.pas:1201 constructor TNpcLabel.Create(AOwner:TDxControl)。</summary>
    public TNpcLabel(TDxControl AOwner) : base(AOwner)
    {
        m_sPostText = "";
        m_sCmd = "";
        m_nColorIndex = 0;
        m_AutoColors = new TList();
        m_dwAutoColorTick = FStateSeamClock.Now;
    }

    /// <summary>FState.pas:1211 destructor TNpcLabel.Destroy（原文先 Free 再 inherited）。</summary>
    public virtual void Destroy()
    {
        // 原文 m_AutoColors.Free；托管侧由 GC 回收。
    }

    /// <summary>
    /// FState.pas:1217 procedure TNpcLabel.Update()。
    /// 原文判据：m_AutoColors.Count &gt; 0 且 (Now - m_dwAutoColorTick) &gt; 600（严格大于）。
    /// 越界时把 m_nColorIndex 归零；随后写入 Up 与 Disabled **两态**（不是五态）。
    /// 赋色用 m_AutoColors.Items[m_nColorIndex]（Delphi TList 元素即 TColor 装箱值）。
    /// </summary>
    public virtual void Update()
    {
        // 原文 inherited（TDxLabel.Update）
        if (m_AutoColors.Count > 0)
        {
            if (FStateSeamClock.Now - m_dwAutoColorTick > 600)
            {
                m_dwAutoColorTick = FStateSeamClock.Now;
                if ((m_nColorIndex < 0) || (m_nColorIndex >= m_AutoColors.Count))
                    m_nColorIndex = 0;
                // 原文 CaptionColor.Up.Color := TColor(m_AutoColors.Items[m_nColorIndex]);
                // 车道1 接缝把 TDxFont 的该字段命名为 Value（同一 TColor 语义）。
                CaptionColor.Up.Value = (int)m_AutoColors[m_nColorIndex];
                CaptionColor.Disabled.Value = (int)m_AutoColors[m_nColorIndex];
                m_nColorIndex = m_nColorIndex + 1;
            }
        }
    }
}

/// <summary>FState.pas:237 TCountDownLabel = class(TDxLabel)（倒计时标签）。</summary>
public class TCountDownLabel : TDxLabel
{
    private TOnClickEx FClickEvent;

    public string m_sCmd;
    public int m_OldCountDownValue;
    public int m_CountDownValue;
    public int m_LoopCount;
    public uint m_UpdateTick;
    public bool m_boChineseFormat;

    /// <summary>FState.pas:1232 constructor TCountDownLabel.Create(AOwner:TDxControl)。</summary>
    public TCountDownLabel(TDxControl AOwner) : base(AOwner)
    {
        m_sCmd = "";
        m_CountDownValue = 0;
        m_OldCountDownValue = 0;
        m_LoopCount = 1;
        m_UpdateTick = FStateSeamClock.Now;
    }

    /// <summary>FState.pas:1242 destructor TCountDownLabel.Destroy（原文空体只调 inherited）。</summary>
    public virtual void Destroy()
    {
    }

    /// <summary>设置点击回调（原文私有字段 FClickEvent，由子类/外部注入）。</summary>
    public void SetClickEvent(TOnClickEx handler) => FClickEvent = handler;

    /// <summary>
    /// FState.pas:1248 procedure TCountDownLabel.UpdateCaption。
    /// 中文格式：`%.2d天%.2d时%.2d分%.2d秒`；否则 `%.2d:%.2d:%.2d`。
    /// 注意中文分支先算 nDay 并扣掉天数，非中文分支**不扣天数**（小时直接是总小时数）。
    /// nDay 在非中文分支被计算但未使用（原文如此）。
    /// </summary>
    public void UpdateCaption()
    {
        int nTime;
        int nDay;
        int nHour;
        int nMinute;
        int nSecond;

        nTime = m_CountDownValue;
        nDay = nTime / 86400; // div

        if (m_boChineseFormat)
        {
            nTime = nTime - nDay * 86400;
            nHour = nTime / 3600;
            nTime = nTime - nHour * 3600;
            nMinute = nTime / 60;
            nSecond = nTime % 60;
            Caption = DelphiFmt.Pct2d(nDay) + "天" + DelphiFmt.Pct2d(nHour) + "时"
                    + DelphiFmt.Pct2d(nMinute) + "分" + DelphiFmt.Pct2d(nSecond) + "秒";
        }
        else
        {
            nHour = nTime / 3600;
            nTime = nTime - nHour * 3600;
            nMinute = nTime / 60;
            nSecond = nTime % 60;
            Caption = DelphiFmt.Pct2d(nHour) + ":" + DelphiFmt.Pct2d(nMinute) + ":" + DelphiFmt.Pct2d(nSecond);
        }
    }

    /// <summary>
    /// FState.pas:1273 procedure TCountDownLabel.Update()。
    /// 分支顺序逐字保留：
    ///   1) m_LoopCount = 0 → Caption := '00:00:00' 并 Exit；
    ///   2) m_CountDownValue &lt;= 0 → m_LoopCount := 0、'00:00:00'，若 m_sCmd 非空且有回调则回调，Exit；
    ///   3) (Now - m_UpdateTick) &gt;= 1000 才推进（**这里是 &gt;=，与 TImgCountDownButton 的 &gt; 不同**）；
    ///   4) m_UpdateTick 累加 1000（不是赋 Now，会保留漂移）；
    ///   5) 减到 0 时回调、把 m_CountDownValue 重置为 m_OldCountDownValue + 1、m_LoopCount &gt; 0 则递减。
    /// </summary>
    public virtual void Update()
    {
        // 原文 inherited
        if (m_LoopCount == 0)
        {
            Caption = "00:00:00";
            return;
        }

        if (m_CountDownValue <= 0)
        {
            m_LoopCount = 0;
            Caption = "00:00:00";
            if ((m_sCmd.Length > 0) && (FClickEvent != null))
            {
                FClickEvent(this, 0, 0);
            }
            return;
        }

        if (FStateSeamClock.Now - m_UpdateTick >= 1000)
        {
            m_UpdateTick = m_UpdateTick + 1000;
            m_CountDownValue = m_CountDownValue - 1;
            UpdateCaption();

            if (m_CountDownValue == 0)
            {
                if ((m_sCmd.Length > 0) && (FClickEvent != null))
                {
                    FClickEvent(this, 0, 0);
                }

                m_CountDownValue = m_OldCountDownValue + 1;

                if (m_LoopCount > 0)
                    m_LoopCount = m_LoopCount - 1;
            }
        }
    }
}

/// <summary>FState.pas:253 TImgCountDownButton = class(TDxImageButton)（图片倒计时按钮）。</summary>
public class TImgCountDownButton : TDxImageButton
{
    public int m_ImgStartIndex;
    public int m_ImgSpace;
    public string m_sCmd;
    public int m_OldCountDownValue;
    public int m_CountDownValue;
    public int m_LoopCount;
    public uint m_UpdateTick;

    /// <summary>FState.pas:1311 constructor TImgCountDownButton.Create(AOwner:TDxControl)。</summary>
    public TImgCountDownButton(TDxControl AOwner) : base(AOwner)
    {
        m_sCmd = "";
        m_CountDownValue = 0;
        m_OldCountDownValue = 0;
        m_LoopCount = 1;
        m_UpdateTick = FStateSeamClock.Now;

        m_ImgStartIndex = 0;
        m_ImgSpace = 0;
    }

    /// <summary>FState.pas:1324 destructor TImgCountDownButton.Destroy（原文空体只调 inherited）。</summary>
    public virtual void Destroy()
    {
    }

    /// <summary>
    /// FState.pas:1330 procedure TImgCountDownButton.UpdateCaption。
    /// 与 TCountDownLabel.UpdateCaption 的差异（必须保留）：**没有**中文分支、**不计算** nDay。
    /// </summary>
    public void UpdateCaption()
    {
        int nTime;
        int nHour;
        int nMinute;
        int nSecond;

        nTime = m_CountDownValue;

        nHour = nTime / 3600;
        nTime = nTime - nHour * 3600;
        nMinute = nTime / 60;
        nSecond = nTime % 60;

        Caption = DelphiFmt.Pct2d(nHour) + ":" + DelphiFmt.Pct2d(nMinute) + ":" + DelphiFmt.Pct2d(nSecond);
    }

    /// <summary>
    /// FState.pas:1344 procedure TImgCountDownButton.Update()。
    /// 与 TCountDownLabel.Update 的差异（必须保留）：
    ///   1) **没有** `m_CountDownValue &lt;= 0` 的前置分支；
    ///   2) 推进判据是 `&gt; 1000`（严格大于），不是 `&gt;= 1000`；
    ///   3) 结束动作是调 frmMain.SendMerchantDlgSelect(g_nCurMerchant, m_sCmd)，不做 FClickEvent。
    /// </summary>
    public virtual void Update()
    {
        // 原文 inherited
        if (m_LoopCount == 0)
        {
            Caption = "00:00:00";
            return;
        }

        if (FStateSeamClock.Now - m_UpdateTick > 1000)
        {
            m_UpdateTick = m_UpdateTick + 1000;
            m_CountDownValue = m_CountDownValue - 1;
            UpdateCaption();

            if (m_CountDownValue == 0)
            {
                if (m_sCmd.Length > 0)
                {
                    // 原文 frmMain.SendMerchantDlgSelect(g_nCurMerchant, m_sCmd);
                    FStateClMainSeam.SendMerchantDlgSelect(FStateClMainSeam.g_nCurMerchant, m_sCmd);
                }

                m_CountDownValue = m_OldCountDownValue + 1;

                if (m_LoopCount > 0)
                    m_LoopCount = m_LoopCount - 1;
            }
        }
    }

    /// <summary>FState.pas:24924 procedure TImgCountDownButton.Paint（按 m_ImgStartIndex/m_ImgSpace 逐字画图）。</summary>
    public virtual void Paint()
    {
        // 【接缝：待 DxImageButton.Paint + HGE 绘制后端移植后接入】
    }
}

/// <summary>FState.pas:270 TNpcInputEdit = class(TDxEdit)（NPC 输入框，可限制为数字）。</summary>
public class TNpcInputEdit : TDxEdit
{
    public bool m_IsNumber;
    public int m_ID;
    public int m_MinValue;
    public int m_MaxValue;
    public string m_ValidityTips;

    /// <summary>FState.pas:1373 constructor TNpcInputEdit.Create(AOwner:TDxControl)。</summary>
    public TNpcInputEdit(TDxControl AOwner) : base(AOwner)
    {
        m_IsNumber = false;
        m_ID = 0;
        m_MinValue = 0;
        m_MaxValue = 0;
    }

    /// <summary>
    /// FState.pas:1382 procedure TNpcInputEdit.KeyPress(var Key:Char)。
    /// 只拦空格（VK_SPACE = $20）：置为 #0 并 Beep 后 Exit，不调 inherited。
    /// 其余按键一律交给 inherited。
    /// </summary>
    public virtual void KeyPress(ref char Key)
    {
        // 原文注释：不让输入空格 2019-12-20 23:56:28
        if (Key == (char)VK_SPACE)
        {
            Key = (char)0; // 原文 Key := #0
            Beep();
            return;
        }

        // 原文 inherited（TDxEdit.KeyPress）
    }

    /// <summary>WinAPI VK_SPACE = 0x20。</summary>
    public const int VK_SPACE = 0x20;

    /// <summary>原文 Windows.Beep（系统默认提示音）。</summary>
    protected virtual void Beep()
    {
        // 【接缝：待 DxEdit 的声音通道移植；此处保留调用点】
    }

    /// <summary>FState.pas:1394 destructor TNpcInputEdit.Destroy（原文空体只调 inherited）。</summary>
    public virtual void Destroy()
    {
    }
}

/// <summary>FState.pas:284 TNpcScrollBox = class(TDxScrollBox)（NPC 任务/说明滚动框）。</summary>
public class TNpcScrollBox : TDxScrollBox
{
    public TStringList m_List;

    /// <summary>FState.pas:1400 constructor TNpcScrollBox.Create(AOwner:TDxControl)。</summary>
    public TNpcScrollBox(TDxControl AOwner) : base(AOwner)
    {
        m_List = new TStringList();
    }

    /// <summary>FState.pas:1410 destructor TNpcScrollBox.Destroy（先 inherited 再 Free m_List）。</summary>
    public virtual void Destroy()
    {
        // 原文 inherited; m_List.Free; —— 注意顺序与 TNpcLabel 相反（原文如此）。
    }
}

/// <summary>FState.pas:300 TMissionLabel = class(TDxTreeNode)。</summary>
public class TMissionLabel : TDxTreeNode
{
    public string m_sCmd;

    public TMissionLabel(TDxControl AOwner) : base(AOwner)
    {
    }
}

/// <summary>FState.pas:305 TMagicButton = class(TDxImageButton)（屏幕上的可拖动技能图标）。</summary>
public class TMagicButton : TDxImageButton
{
    public PTClientMagic m_Magic;

    public TMagicButton(TDxControl AOwner) : base(AOwner)
    {
    }

    /// <summary>FState.pas:24803 procedure TMagicButton.Paint（按技能图标 + CD 遮罩绘制）。</summary>
    public virtual void Paint()
    {
        // 【接缝：待 DxImageButton.Paint + HGE 绘制后端移植后接入】
    }
}

/// <summary>
/// Delphi `Format('%.2d', [v])` 的语义等价（宽度 2、零填充；负号不计入宽度，
/// 故 -1 → "-01"，与 C# 的 D2 一致）。
/// </summary>
public static class DelphiFmt
{
    /// <summary>Delphi `%.2d`。</summary>
    public static string Pct2d(int v) => v.ToString("D2", CultureInfo.InvariantCulture);
}
