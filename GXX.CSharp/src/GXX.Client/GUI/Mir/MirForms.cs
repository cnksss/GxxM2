using System;
using GXX.Client.GUI.DxComponent;
using GXX.Core.Protocol;

namespace GXX.Client.GUI.Mir;

// ============================================================================================
// 【接缝】GUI/Mir 窗口族所需的最小全局面。
//
// 逐条对应（原名保留，便于与原 .pas 对照）：
//   MShare.pas 单元级 var  → MShareGlobals 静态字段
//   ClFunc.pas 单元级函数   → ClFuncs 静态方法
//   SoundUtil.pas PlaySound → SoundUtil 静态方法（可注入 handler）
//   FState.pas  TStateWindows（连击版人物状态窗口，1041KB 单元，未移植）
//   SDK.pas     SCREENWIDTH/SCREENHEIGHT
//
// 未移植的依赖一律只留最小接缝，绝不顺手移植：
//   MShare / FState / StateWindows / ClMain / GameImages / HGE / SoundUtil 的真实实现
//   待各自单元移植后把这里的方法体替换为调用即可（调用点已按原名写成 1:1 形态）。
// ============================================================================================

/// <summary>MShare.pas:493 TConfigClient（登录器下发的客户端内挂配置；本车道按字段逐个复刻注释）。</summary>
public sealed class TConfigClient
{
    /// <summary>MShare.pas boShow1024：1024 分辨率 UI 开关（MirSequelDlg/MirNewUI205Dlg 使用）。</summary>
    public byte boShow1024;

    /// <summary>MShare.pas nNPCMsgDlgTextOffsetX：NPC 对话框文字修正 X。</summary>
    public int nNPCMsgDlgTextOffsetX;

    /// <summary>MShare.pas nNPCMsgDlgTextOffsetY：NPC 对话框文字修正 Y。</summary>
    public int nNPCMsgDlgTextOffsetY;

    /// <summary>MShare.pas boNPCGuiCanMove：NPC 界面能否移动。</summary>
    public byte boNPCGuiCanMove;

    /// <summary>MShare.pas boHeroStateDlgNoMove：英雄状态窗是否禁止自动避让。</summary>
    public byte boHeroStateDlgNoMove;

    /// <summary>MShare.pas boUseOldSerialWindows：使用旧的（185/英雄）装备窗口。</summary>
    public byte boUseOldSerialWindows;

    /// <summary>MShare.pas boCustomUI：是否自定义 UI。</summary>
    public byte boCustomUI;

    /// <summary>MShare.pas boShowBagGameInfo：包裹是否显示元宝/灵符/金刚石信息。</summary>
    public byte boShowBagGameInfo;

    /// <summary>MShare.pas boShowBagGameGoldSeparator：游戏币数量是否加千分位。</summary>
    public byte boShowBagGameGoldSeparator;

    /// <summary>MShare.pas boDescSupportRenamItem：备注支持改名物品。</summary>
    public byte boDescSupportRenamItem;

    /// <summary>MShare.pas boNoRenameDescReadDefault：未改名物品读取默认备注。</summary>
    public byte boNoRenameDescReadDefault;

    /// <summary>MShare.pas btSuspensionShowItem：悬浮显示物品（0=旧模式）。</summary>
    public byte btSuspensionShowItem;

    /// <summary>MShare.pas boStateWindowsType：人物状态窗类型（0=1.76 传统样式）。</summary>
    public int boStateWindowsType;
}

/// <summary>SDK.pas:324 SCREENWIDTH/SCREENHEIGHT（默认 1024；ClMain 初始化时赋 g_nScreenWidth）。</summary>
public static class ScreenSize
{
    public static int SCREENWIDTH = 1024;
    public static int SCREENHEIGHT = 768;
}

/// <summary>
/// SoundUtil.pas 的两套语义：
///   PlaySound(nIdx:Integer)      —— 按 g_SoundList 索引播放（DLoginNewClickSound 用）
///   GetGoldStr(gold:LongWord)    —— 数值千分位/亿万分段（ClFunc.pas 与 MShare.pas 各有一份实现）
/// 【接缝：待 SoundUtil / ClFunc 移植后接入真实实现】
/// </summary>
public static class SoundUtil
{
    // SoundUtil.pas:147-149
    public const int s_norm_button_click = 103;
    public const int s_rock_button_click = 104;
    public const int s_glass_button_click = 105;

    /// <summary>测试注入点；为 null 时走真实 Bass 播放（此处留空实现）。</summary>
    public static Action<int> PlaySoundHandler;

    /// <summary>SoundUtil.pas:27/37 procedure PlaySound(nIdx:Integer)。</summary>
    public static void PlaySound(int nIdx) => PlaySoundHandler?.Invoke(nIdx);
}

/// <summary>
/// FState.pas:311 TFrmDlg 顶层窗口基类（1041KB 单元，未移植）。
/// 这里是本车道 5 个窗口实际用到的字段/方法的最小面；其余成员待 FState 单元移植后补齐。
/// </summary>
public class TFrmDlg
{
    // ---------------- 控件字段（FState.pas TFrmDlg / SerialWindowsDlg.pas TSerialWindows 类声明） ----------------
    public TDxImageForm DMerchantDlg;
    public TDxImageButton DMerchantDlgClose;
    public TDxImageButton DMerchantDlgHelp;
    public TDxImageForm DItemBag;
    public TDxImageForm DMenuDlg;
    public TDxImageForm DSellDlg;
    public TDxImageForm DHeroStateDlg;
    public TDxImageForm DHeroStateDlg185;

    /// <summary>SerialWindowsDlg.pas DMerchantDlgClick（NPC 对话框内超链接的点击处理器）。</summary>
    public TOnClickEx DMerchantDlgClick;

    // ---------------- NPC 对话框状态（FState.pas TFrmDlg 单元级/字段） ----------------
    public int MerchantFace;
    public string MerchantName = string.Empty;
    public string MDlgStr = string.Empty;

    /// <summary>FState.pas TFrmDlg.MerchantFace/MerchantName/MDlgStr 之外的门限字段。</summary>
    public bool RequireAddPoints;
    public uint LastestClickTime;

    /// <summary>
    /// FState.pas:1864 TFrmDlg.ShowMDlg 的几何/避让部分（子类先 inherited 再叠加自身逻辑）。
    /// 原文此处访问 DPageControlMission 等未移植控件，本车道 5 个窗口均不触及，故只保留
    /// g_boOpenMerchantBigDlg 的默认对话框复位分支。
    /// 【接缝：待 FState 单元移植后补全 DBackground 树遍历】
    /// </summary>
    public virtual void ShowMDlg(int face, string mname, string msgstr, bool boSetBagItemPos = true, bool IsDesigning = false)
    {
        // FState.pas TFrmDlg.ShowMDlg：if not g_boOpenMerchantBigDlg then
        //   复位 NPC 对话框位置/图号/关闭按钮矩形，并设置 Floating。
        // g_MerchantImageIndex / g_MerchantCloseButtonRect 属 SerialWindowsDlg 的全局（见 TSerialWindows）。
    }

    /// <summary>
    /// FState.pas:13167 TFrmDlg.AddNpcMemo：把 NPC 脚本文本解析成一组子控件（FCOLOR:/IMG:/ITEMSHOW: …）。
    /// 该函数约 400 行且依赖 DxComponent 全部控件与图库，本车道以接缝表达：
    /// 记录调用次数与文本（供测试断言分支走位），真实控件生成待 FState 单元移植后接入。
    /// </summary>
    public int AddNpcMemoCount;
    public string LastAddNpcMemoText = string.Empty;
    public TDxControl LastAddNpcMemoOwner;

    /// <summary>FState.pas:13167 AddNpcMemo(AOwner, X, Y, AOnClick, Text, ChangeOwnerSize, IsFromAddDlg, OneLineHeight, IsDesigning)。</summary>
    public virtual void AddNpcMemo(TDxControl AOwner, int x, int y, TOnClickEx aOnClick, string text,
        bool ChangeOwnerSize = true, bool IsFromAddDlg = false, int OneLineHeight = 16, bool IsDesigning = false)
    {
        AddNpcMemoCount++;
        LastAddNpcMemoText = text;
        LastAddNpcMemoOwner = AOwner;
        // 【接缝：待 FState 单元移植后接入 AddNpcMemo 的控件生成】
    }

    /// <summary>FState.pas:2256 TFrmDlg.DLoginNewClickSound（csNorm/csStone/csGlass 三种点击音效）。</summary>
    public void DLoginNewClickSound(object Sender, TClickSound Clicksound)
    {
        switch (Clicksound)
        {
            case TClickSound.csNorm:
                SoundUtil.PlaySound(SoundUtil.s_norm_button_click);
                break;
            case TClickSound.csStone:
                SoundUtil.PlaySound(SoundUtil.s_rock_button_click);
                break;
            case TClickSound.csGlass:
                SoundUtil.PlaySound(SoundUtil.s_glass_button_click);
                break;
        }
    }

    /// <summary>FState.pas TFrmDlg.MySelfAbilChange（TSerialWindows 覆写；此处为虚钩子）。</summary>
    public virtual void MySelfAbilChange() { }

    /// <summary>FState.pas TFrmDlg.OpenUserState（TSerialWindows 覆写；此处为虚钩子）。</summary>
    public virtual void OpenUserState() { }
}
