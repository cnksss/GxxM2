using System;
using GXX.Core.Rtl;

namespace GXX.GameCenter;

/// <summary>
/// GMain.pas（7,060 行）TfrmMain 1:1 移植的窗体骨架。
/// <para>
/// 本轮（P1 分片）落地范围：
/// <list type="bullet">
/// <item>第 15..17 行 const（<see cref="sProgramName"/>）</item>
/// <item>第 19..383 行 TfrmMain 的 362 个 DFM 字段声明（<c>GMainForm.Fields.g.cs</c>，脚本抽取 + 回读比对）</item>
/// <item>第 503..570 行 private/protected/public 成员字段与全部方法签名清单（本文件 + <c>GMainForm.Methods.g.cs</c>）</item>
/// </list>
/// 未落地范围（留待后续分片）：窗体构造与 DFM 布局（GMain.dfm 5,211 行）、
/// 实现段全部 171 个例行程序体（其中配置生成段 1619..2606、辅助过程 581..864、
/// 备份清单 1446..1540、清库 SQL 5730..5845 已由 <see cref="GMainConfig"/>、
/// <see cref="GMainHelpers"/>、<see cref="GMainBackList"/>、<see cref="GMainClearSql"/> 落地）。
/// </para>
/// </summary>
public sealed partial class MainForm : System.Windows.Forms.Form
{
    /// <summary>GMain.pas:16 <c>sProgramName = '引擎控制台';</c></summary>
    public const string sProgramName = "引擎控制台";

    // ================= GMain.pas:503-507 private 字段 =================

    /// <summary>GMain.pas:504 <c>m_boOpen: Boolean;</c></summary>
    private bool m_boOpen;

    /// <summary>GMain.pas:505 <c>m_nStartStatus: Integer;</c></summary>
    private int m_nStartStatus;

    /// <summary>GMain.pas:506 <c>m_dwShowTick: LongWord;</c></summary>
    private uint m_dwShowTick;

    /// <summary>GMain.pas:507 <c>m_StartTime: TDateTime;</c></summary>
    private DateTime m_StartTime;

    // GMain.pas:509 <c>//FOldCaption: string;</c>（原文已注释）

    // ================= 测试辅助：读取私有状态 =================

    /// <summary>测试辅助：读 m_boOpen（GMain.pas:504）。</summary>
    public bool Get_m_boOpen() => m_boOpen;

    /// <summary>测试辅助：写 m_boOpen。</summary>
    public void Set_m_boOpen(bool value) => m_boOpen = value;

    /// <summary>测试辅助：读 m_nStartStatus（GMain.pas:505）。</summary>
    public int Get_m_nStartStatus() => m_nStartStatus;

    /// <summary>测试辅助：写 m_nStartStatus。</summary>
    public void Set_m_nStartStatus(int value) => m_nStartStatus = value;

    /// <summary>测试辅助：读 m_dwShowTick（GMain.pas:506）。</summary>
    public uint Get_m_dwShowTick() => m_dwShowTick;

    /// <summary>测试辅助：写 m_dwShowTick。</summary>
    public void Set_m_dwShowTick(uint value) => m_dwShowTick = value;

    /// <summary>测试辅助：读 m_StartTime（GMain.pas:507）。</summary>
    public DateTime Get_m_StartTime() => m_StartTime;

    /// <summary>测试辅助：写 m_StartTime。</summary>
    public void Set_m_StartTime(DateTime value) => m_StartTime = value;

    // ================= GMain.pas:866-870 MainOutMessage（已落地） =================

    /// <summary>
    /// GMain.pas:866 <c>procedure TfrmMain.MainOutMessage(sMsg: string);</c>
    /// 原文：<c>sMsg := '[' + DateTimeToStr(Now) + '] ' + sMsg; MemoLog.Lines.Add(sMsg);</c>
    /// <c>DateTimeToStr</c> 使用系统短日期/时间格式；托管侧以 <see cref="DateTimeToStr"/> 的等价格式输出。
    /// </summary>
    public void MainOutMessage(string sMsg)
    {
        sMsg = "[" + DateTimeToStr(DelphiRTL.Now()) + "] " + sMsg;
        AppendMemoLog(sMsg);
    }

    /// <summary>
    /// GMain.pas:869 <c>MemoLog.Lines.Add(sMsg)</c> 接缝：
    /// 已接线时写宿主控件；未接线时写入 <see cref="MemoLogLines"/>，使逻辑可脱离 UI 单测。
    /// （非 public 字段：避免与 DFM 的 362 个公开控件字段计数混淆。）
    /// </summary>
    private Action<string>? MemoLogAddHandler;

    /// <summary>设置/清除 Memo 行写入接缝。</summary>
    public void SetMemoLogAddHandler(Action<string>? handler) => MemoLogAddHandler = handler;

    /// <summary>窗体外单测可读的 Memo 行容器（未接线时的兜底目标）。</summary>
    public System.Collections.Generic.List<string> MemoLogLines { get; } = new();

    private void AppendMemoLog(string s)
    {
        if (MemoLogAddHandler != null) MemoLogAddHandler(s);
        else MemoLogLines.Add(s);
    }

    // ================= Delphi DateTimeToStr（System.SysUtils） =================

    /// <summary>
    /// Delphi <c>SysUtils.DateTimeToStr</c>：使用 <c>ShortDateFormat + ' ' + LongTimeFormat</c>。
    /// Delphi 7 默认 ShortDateFormat='yyyy/M/d'、LongTimeFormat='h:nn:ss'（24 小时制，小时无前导零）。
    /// </summary>
    public static string DateTimeToStr(DateTime value)
        => value.ToString("yyyy/M/d H:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

    /// <summary>
    /// GMain.pas 实现段的 171 个例行程序（含 event handler）清单，
    /// 由 <c>GMainForm.Methods.g.cs</c>（脚本从原文抽取）提供为 <see cref="MainPasRoutineIndex"/>。
    /// </summary>
    public static int MainPasRoutineCount => MainPasRoutineIndex.Length;
}
