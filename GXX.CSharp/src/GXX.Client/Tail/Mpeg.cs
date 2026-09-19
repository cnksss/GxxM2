// 源单元：Source/Client-HGE/Mpeg.pas（原文 98 行，CRLF 计入 113 行）
// 原文 uses：Windows, DShow, ActiveX, Controls
// 原文无同名 .dfm（TMPEG 不是窗体，只是一个包装 DirectShow 的普通类）。
//
// ⚠ 覆盖事实（已核）：Mpeg 单元**不在 Client.dpr 的 uses 列表中**
//   （Source/Client-HGE/Client.dpr:15-113 逐行核对；整棵源码树对 TMPEG 的引用只出现在
//    本单元内部）。它是一份孤儿单元，客户端主程序并不链接它。
//
// ── 转换开发文档 §2.3「不移植项」登记 ──────────────────────────────────────
// 原文 `TMPEG` 是 DirectShow（DShow.pas）的薄封装：CoCreateInstance(CLSID_FilterGraph)
// 拿到 IGraphBuilder，再 QueryInterface 出 IMediaControl / IMediaSeeking /
// IBasicAudio / IVideoWindow，用 `RenderFile` 播放一个媒体文件并把画面
// `put_Owner` 到某个 TWinControl 上。
// 该项属「原生 COM/UI 句柄互操作」：.NET 8 无 DShow.pas 对应的托管投影，
// 而把 DirectShow 图嵌进 WinForms 需要新建一整套 COM 互操作声明
// （IGraphBuilder/IMediaControl/… 的 [ComImport] 接口 + CLSID），
// 这与「先移植纯逻辑、把原生调用收敛到接缝」的规程一致 —— 本单元按 §2.3 做 **Stub**：
//   保留 TMPEG 的构造/析构/Play/Pause/Stop 五个公开成员签名与
//   `Close`/`Init` 两个私有成员（提升为 internal 以便测试），
//   以及原文的 boInit/boPlay 状态机取值，**不**建立任何 COM 对象。
// ──────────────────────────────────────────────────────────────────────────
using System;
using System.Windows.Forms;

namespace GXX.Client.Tail;

/// <summary>
/// Mpeg.pas 1:1 移植（Stub 形态，见文件头 §2.3 登记）。
///
/// <para><b>原文状态机（本移植如实保留其可观察取值）</b>：</para>
/// <list type="bullet">
/// <item><c>Create</c>（Mpeg.pas:47-57）：把 <c>MovieWindow</c> 记下来，五个 COM 接口全置 nil，
///   并把 <c>boInit := FALSE</c>。注意原文第 55 行那句 <c>// boInit:=Init();</c> 是**被注释掉的**，
///   第 56 行才是生效的 <c>boInit := FALSE</c>。</item>
/// <item><c>Init</c>（:65-75）：<c>Result := FALSE</c> 起步；任一步骤 failed 即 Exit，
///   只有全部 QueryInterface 成功才置 True。</item>
/// <item><c>Play</c>（:82-102）：**不检查返回值**地 <c>boInit := Init();</c>，
///   然后 <c>RenderFile</c>；failed 即 Exit（此时 <c>boPlay</c> 仍为初值 FALSE）；
///   成功则 <c>g_pMediaControl.Run</c> 后 <c>boPlay := True</c>。</item>
/// <item><c>Stop</c>（:106-111）：<c>if not boInit then Exit;</c> —— 未初始化则**什么都不做**
///   （包括不调 Close）。</item>
/// <item><c>Close</c>（:35-45）：逐个置 nil、<c>CoUninitialize</c>、<c>boInit := FALSE</c>；
///   **不动 boPlay**（原文如此，是个残留位）。</item>
/// </list>
///
/// <para><b>Stub 行为</b>：不创建 COM 对象，<c>Init()</c> 恒返回 false
/// （等价于原文 <c>CoInitialize</c> 失败的那条出口）；因此 <c>Play</c> 也恒返回 false，
/// 与原文"环境不具备 DirectShow 时"的可观察结果一致。</para>
/// </summary>
public class TMPEG : IDisposable
{
    // 原文 Mpeg.pas:9-16 的接口字段在托管侧无对应类型；以接缝注释保留其存在。
    // 接缝：待 Mpeg 的 DirectShow COM 互操作（IGraphBuilder/IMediaControl/IMediaSeeking/
    //       IBasicAudio/IVideoWindow）落地后接入。

    /// <summary>原文 Mpeg.pas:14 — <c>boInit: Boolean;</c></summary>
    private bool boInit;

    /// <summary>原文 Mpeg.pas:15 — <c>boPlay: Boolean;</c></summary>
    private bool boPlay;

    /// <summary>原文 Mpeg.pas:16 — <c>sFileName: string;</c></summary>
    private string sFileName;

    /// <summary>原文 Mpeg.pas:17 — <c>MovieWindow: TWinControl;</c></summary>
    private readonly Control MovieWindow;

    /// <summary>
    /// 原文 Mpeg.pas:47-57 <c>constructor TMPEG.Create(PlayWindow: TWinControl);</c>
    /// </summary>
    public TMPEG(Control PlayWindow)
    {
        MovieWindow = PlayWindow;
        sFileName = string.Empty;
        // 原文如此（Mpeg.pas:55）：// boInit:=Init();  ← 被注释掉
        // 原文如此（Mpeg.pas:56）：boInit := FALSE;
        boInit = false;
    }

    /// <summary>原文 Mpeg.pas:59-63 <c>destructor TMPEG.Destroy;</c> —— <c>Close(); inherited;</c></summary>
    public void Dispose()
    {
        Close();
        GC.SuppressFinalize(this);
    }

    /// <summary>原文 Mpeg.pas:35-45 <c>procedure TMPEG.Close;</c>（原文为 private；此处 public 供测试直接驱动，行为一致）</summary>
    public void Close()
    {
        // 原文如此（Mpeg.pas:37-42）：逐个 Assigned 后 Stop / 置 nil（COM 接口释放）。
        // 接缝：待 Mpeg 的 DirectShow COM 互操作落地后接入。
        // 原文如此（Mpeg.pas:44）：boInit := FALSE;
        boInit = false;
        // 原文如此（Mpeg.pas:35-45）：Close **不**重置 boPlay —— 保留该残留位。
    }

    /// <summary>
    /// 原文 Mpeg.pas:65-75 <c>function TMPEG.Init: Boolean;</c>（原文为 private；此处 public 供测试直接驱动，行为一致）
    /// </summary>
    /// <returns>原文在 <c>CoInitialize</c> / <c>CoCreateInstance</c> / 任一 <c>QueryInterface</c>
    /// 失败时的取值 <c>FALSE</c>（Stub 恒返回该值）。</returns>
    public bool Init()
    {
        // 原文如此（Mpeg.pas:67）：Result := FALSE; // 初始化COM介面
        // 接缝：待 Mpeg 的 DirectShow COM 互操作落地后接入
        //       （CoInitialize → CoCreateInstance(CLSID_FilterGraph) → 4×QueryInterface）。
        return false;
    }

    /// <summary>
    /// 原文 Mpeg.pas:82-102 <c>function TMPEG.Play(sFileName: string): Boolean;</c>
    /// </summary>
    public bool Play(string sFileName)
    {
        // 原文如此（Mpeg.pas:87）：Result := FALSE;
        // 原文如此（Mpeg.pas:88）：boInit := Init();  ← 注意：**不判断返回值**
        boInit = Init();
        if (!boInit)
        {
            // 原文在 Init 失败后仍会调 RenderFile（g_pGraphBuilder 为 nil 会崩）；
            // 这里对应原文 RenderFile 失败后 `if failed(_hr) then Exit;` 的出口，
            // 此时 boPlay 保持未修改。原文如此（Mpeg.pas:91）。
            this.sFileName = sFileName;
            return false;
        }

        // 以下为原文 :90-101 的 COM 路径，Stub 不执行：
        //   MultiByteToWideChar(CP_ACP, 0, PChar(sFileName), -1, @wFile, MAX_PATH); // 轉換格式
        //   _hr := g_pGraphBuilder.renderfile(@wFile, nil);
        //   if MovieWindow <> nil then begin
        //     g_pVideoWindow.put_Owner(MovieWindow.Handle);
        //     g_pVideoWindow.put_windowstyle(WS_CHILD or WS_Clipsiblings);
        //     g_pVideoWindow.SetWindowposition(0, 0, MovieWindow.Width, MovieWindow.Height);
        //   end;
        //   g_pMediaControl.Run;
        //   boPlay := True;
        this.sFileName = sFileName;
        boPlay = true;
        return true;
    }

    /// <summary>原文 Mpeg.pas:77-80 <c>procedure TMPEG.Pause;</c></summary>
    public void Pause()
    {
        // 原文如此（Mpeg.pas:79）：g_pMediaControl.Pause;  ← 原文**不判空**，未初始化即崩。
        // Stub：未初始化时静默返回（差值已登记）。
        if (!boInit) return;
        // 接缝：待 Mpeg 的 DirectShow COM 互操作落地后接入。
    }

    /// <summary>原文 Mpeg.pas:106-111 <c>procedure TMPEG.Stop;</c></summary>
    public void Stop()
    {
        // 原文如此（Mpeg.pas:108）：if not boInit then Exit;
        if (!boInit) return;
        // 原文如此（Mpeg.pas:109-110）：g_pMediaControl.Stop; Close();
        Close();
    }

    /// <summary>原文 Mpeg.pas:14 的 <c>boInit</c>（原为 private 字段；此处暴露只读视图作为测试接缝）</summary>
    public bool BoInit => boInit;

    /// <summary>原文 Mpeg.pas:15 的 <c>boPlay</c>（原为 private 字段；此处暴露只读视图作为测试接缝）</summary>
    public bool BoPlay => boPlay;

    /// <summary>原文 Mpeg.pas:16 的 <c>sFileName</c>（原为 private 字段；此处暴露只读视图作为测试接缝）</summary>
    public string FileName => sFileName;

    /// <summary>原文 Mpeg.pas:17 的 <c>MovieWindow</c>（原为 private 字段；此处暴露只读视图作为测试接缝）</summary>
    public Control PlayWindowControl => MovieWindow;
}
