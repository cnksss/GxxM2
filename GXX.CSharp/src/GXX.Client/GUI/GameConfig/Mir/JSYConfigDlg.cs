// 源单元：Source/Client-HGE/GameConfig/MirJSY/JSYConfigDlg.pas（GBK，6,395 行，CRLF）
//
// ⏳ 本文件是 **TJSYConfigDlg 的过渡实现**（诚实登记，不是完成品）：
//   JSYConfigDlg.pas 与 MirConfigDlg.pas 是**同一代、同类**的两个配置对话框，
//   字段名与逻辑大量同名同形（g_Config / Create / 访问器 / RefConfig / AutoEat* /
//   AutoProtect / DuraWarning / LoadConfigFile / SaveConfigFile …），
//   差别主要在**UI 技术栈**：Mir 用 DxComponent 自绘控件 + 'MIRCONFIGDLG' 资源流，
//   JSY 用标准 VCL（<c>TFrmJSYDlg = class(TForm)</c> + 114 KB 的 JSYConfigDlg.dfm）。
//
//   本车道本次交付把接缝接到**真实现**（不再是 TStubGameConfigObject），但 JSY 的
//   6395 行逐行搬运尚未完成 ⇒ 这里让 TJSYConfigDlg 继承已翻译的 TMirConfigDlg，
//   并按其真实类型语义覆写 `GetType = ptJSY`（GameConfigDlgs.Finalize 的
//   `is TJSYConfigDlg` 判定因此成立）。
//
//   ★ 与 TStubGameConfigObject 的关键差别（这就是本次接线带来的真实收益）：
//     - 原有 TStubGameConfigObject 的 Finalize/Logout/... 全是**空实现**；
//       现在走 TMirConfigDlg 的真实体（SaveConfigFile / FEnabled / FLoadConfig …）；
//     - ConfigCheckeds 数组与 Create 里的 40 条默认勾选位**真实存在**。
//   ★ 未做的（不许假报）：JSY 自己的 UI 构建、RefConfig、AutoEat* 等 303 个方法
//     尚未逐一搬运。报告 §未完成/阻塞项 有完整清单与下一步切法。

using GXX.Client.GUI.GameConfig.Seams;

namespace GXX.Client.GUI.GameConfig.Mir;

/// <summary>
/// <c>TJSYConfigDlg = class(TGameConfigObject)</c>（JSYConfigDlg.pas:51）的过渡实现。
/// 继承 <see cref="TMirConfigDlg"/>（同代同类的完整骨架），只覆写类型标识。
///
/// ★ 命名与命名空间：原 <c>GXX.Client.GUI.GameConfig.TJSYConfigDlg</c>（桩）
///   已由本类**改名**为 <c>GXX.Client.GUI.GameConfig.Mir.TJSYRealConfigDlg</c> 承载真实现；
///   而原命名空间下的 <c>TJSYConfigDlg</c> 名字保留为它的子类（见 GameConfigDlgs.cs 的收口块），
///   以保证既有测试里的 <c>new TJSYConfigDlg()</c> 与 <c>is TJSYConfigDlg</c> 判定不受影响。
/// </summary>
public class TJSYRealConfigDlg : TMirConfigDlg
{
    /// <summary>原文 JSYConfigDlg.pas:1126-1129：<c>Result := ptJSY;</c></summary>
    public override TConfigDlgType GetType() => TConfigDlgType.ptJSY;

    /// <summary>
    /// 原文 JSYConfigDlg.pas:1012-1112 的 <c>TJSYConfigDlg.Create</c> 与 Mir 版同形；
    /// 本过渡实现直接复用 Mir 的 Create（含 40 条默认勾选位与 FMemo 建立）。
    /// </summary>
    public TJSYRealConfigDlg(IMirConfigDlgControls plug = null) : base(plug) { }
}
