// 源单元：Source/Client-HGE/GameConfig/Mir/MirConfigDlg.pas（GBK，8,355 行，CRLF）
//
// 本文件承载 TMirConfigDlg 用到的 <c>TConfigChecked</c> 成员与基线枚举的对照。
//
// ★ 本车道核实结论（**没有缺口**）：
//   MirConfigDlg.pas 直接使用的每一个 <c>ck*</c> 名字，都能在基线
//   <c>GXX.Client.GUI.GameConfig.TConfigChecked</c>（GameConfigDlg.pas 1:1 移植）里找到
//   **同名**成员。逐个点名（原文行号 → 基线枚举值）：
//     ckHideMonsterIcons(1137)         → ckHideMonsterIcons = 93
//     ckDimFireEffect(1138)            → ckDimFireEffect = 94
//     ckObjectHintEffect(1139)         → ckObjectHintEffect = 133
//     ckSimpleShowBB(2344/2944/4090)   → ckSimpleShowBB = 76
//     ckShowValueItemEffect(2156/2866)→ ckShowValueItemEffect = 90
//   故**不需要**别名表，也**不需要**合成槽位（与 MirsConfigDlg/MirReturnConfigDlg
//   两个车道"必须开合成槽位"的情形不同 —— 那两个单元用的是 <c>ckMovePick</c>
//   这类基线里确实没有的名字）。
//
// ★ 但 FConfigCheckeds 的长度仍取 <c>HighOrdinal + 1</c>（= 134），
//   与基线枚举完全同长；越界访问在本单元不存在。

namespace GXX.Client.GUI.GameConfig.Mir;

/// <summary>
/// MirConfigDlg.pas 的 <c>TConfigChecked</c> 使用面与基线枚举的对照表（供审计与测试逐条断言）。
/// </summary>
public static class MirConfigCheckedMap
{
    /// <summary>原文 1137（<c>PlugCheckBoxHideMonsterIcons</c>）。</summary>
    public const TConfigChecked ckHideMonsterIcons = TConfigChecked.ckHideMonsterIcons;

    /// <summary>原文 1138（<c>PlugCheckBoxDimFireEffect</c>）。</summary>
    public const TConfigChecked ckDimFireEffect = TConfigChecked.ckDimFireEffect;

    /// <summary>原文 1139（<c>PlugCheckBoxNearEffect</c>，HZQ 20230829 补丁新增的勾选位）。</summary>
    public const TConfigChecked ckObjectHintEffect = TConfigChecked.ckObjectHintEffect;

    /// <summary>原文 2344/2944/4090（<c>PlugCheckSimpleShowBB</c>）。</summary>
    public const TConfigChecked ckSimpleShowBB = TConfigChecked.ckSimpleShowBB;

    /// <summary>原文 2156/2866（<c>PlugCheckBoxShowValueItemEffect</c>）。</summary>
    public const TConfigChecked ckShowValueItemEffect = TConfigChecked.ckShowValueItemEffect;

    /// <summary>
    /// 基线枚举长度（= 134）。TMirConfigDlg.FConfigCheckeds 的长度即取此值。
    /// </summary>
    public static int Length => TConfigCheckedBounds.HighOrdinal + 1;

    /// <summary>对照表（供报告与测试逐条打印）。</summary>
    public static string Describe() =>
        "ckHideMonsterIcons(1137)->" + (int)ckHideMonsterIcons +
        "; ckDimFireEffect(1138)->" + (int)ckDimFireEffect +
        "; ckObjectHintEffect(1139)->" + (int)ckObjectHintEffect +
        "; ckSimpleShowBB(2344/2944/4090)->" + (int)ckSimpleShowBB +
        "; ckShowValueItemEffect(2156/2866)->" + (int)ckShowValueItemEffect +
        "; 无缺口（全部同名命中基线枚举，长度 " + Length + "）";
}
