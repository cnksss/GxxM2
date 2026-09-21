using System;
using System.Collections.Generic;
using GXX.Client.GUI.GameConfig.Seams;
using MirDlg = GXX.Client.GUI.GameConfig.Mir;
using GXX.Core.Util;

namespace GXX.Client.GUI.GameConfig;

/// <summary>
/// GameConfigDlgs.pas 1:1 移植（全文 1-174 行）：
/// 配置对话框插件管理（TConfigDlgManage）+ 从流装载内挂 UI（LoadControlFromStream）。
///
/// 原文 <c>ConfigDlgList:TStringList</c> 用 <c>Objects[]</c> 挂 <c>TGameConfigObject</c>，
/// 故统一用 <see cref="TStringList"/> 保留"字符串 + 对象"双列语义。
/// </summary>
public class TConfigDlgManage
{
    /// <summary>原文 <c>ConfigDlgList:TStringList;</c>（GameConfigDlgs.pas:15）</summary>
    public TStringList ConfigDlgList;

    /// <summary>原文 <c>constructor TConfigDlgManage.Create();</c>（GameConfigDlgs.pas:64-67）</summary>
    public TConfigDlgManage()
    {
        ConfigDlgList = new TStringList();
    }

    /// <summary>
    /// 原文 <c>destructor TConfigDlgManage.Destroy;</c>（GameConfigDlgs.pas:69-73）：
    /// 先 <c>UnLoadPlugIn</c> 再释放列表。
    ///
    /// 托管侧说明：本类**不能**声明 C# 终结器（<c>~TConfigDlgManage()</c>），
    /// 因为原文自己有一个公开方法 <c>Finalize</c>（GameConfigDlgs.pas:129），
    /// 二者在 C# 里会重名（CS0111）。因此按原文语义把析构体实现为显式
    /// <see cref="Destroy"/>，由宿主在释放时调用；GC 不保证调用时机，
    /// 与 Delphi 的确定性析构对应关系在报告中登记。
    /// </summary>
    public void Destroy()
    {
        UnLoadPlugIn();
        ConfigDlgList = new TStringList();   // 原文 ConfigDlgList.Free（Free 后不能再访问）
    }

    /// <summary>
    /// 原文 <c>procedure TConfigDlgManage.LoadPlugIn();</c>（GameConfigDlgs.pas:75-107）。
    ///
    /// 逐字语义：
    /// 1) **无条件**先建一个 <c>TJSYConfigDlg</c> 并 <c>AddObject('', ConfigObject)</c>；
    /// 2) <c>{$IF TESTMODE = 0}</c> 段（正式版）：把 <c>g_ConfigClient.ClientConfigs[]</c>
    ///    循环拷进该对象的 <c>ConfigCheckeds[]</c>，再**额外单独**补一条
    ///    <c>ConfigCheckeds[ckSceneShake] := g_ConfigClient.ClientConfigs[51]</c>；
    /// 3) <c>case g_ClientVersion of cv176..cvMirNewUI205:</c> 再建一个 <c>TMirConfigDlg</c> 并加入列表。
    ///
    /// 接缝：<c>TJSYConfigDlg</c>(JSYConfigDlg.pas) / <c>TMirConfigDlg</c>(MirConfigDlg.pas) 属后续批次，
    /// 由 <see cref="PlugInSeam"/> 注入工厂；<c>g_ConfigClient</c>/<c>g_ClientVersion</c> 由
    /// <see cref="ClientGlobalSeam"/> 注入。**TESTMODE 采用正式版（TESTMODE=0）分支。**
    /// </summary>
    public void LoadPlugIn()
    {
        TGameConfigObject ConfigObject = PlugInSeam.CreateJSYConfigDlg();
        ConfigDlgList.AddObject("", ConfigObject);

        // {$IF TESTMODE = 0}
        for (int I = 0; I <= ClientGlobalSeam.ClientConfigsLength() - 1 /*44*/; I++)
        {
            ConfigObject.ConfigCheckeds[(int)(TConfigChecked)I] = ClientGlobalSeam.ClientConfig(I);
        }
        // 原文 ConfigDlgManage.LoadPlugIn 第 88 行写死下标 51：
        // 数组不足 52 项时按 Delphi 边界检查语义跳过（$R+ 下为运行时错误，此处保守跳过）。
        if (ClientGlobalSeam.ClientConfigsLength() > 51)
            ConfigObject.ConfigCheckeds[(int)TConfigChecked.ckSceneShake] = ClientGlobalSeam.ClientConfig(51);
        // {$IFEND}

        switch (ClientGlobalSeam.g_ClientVersion)
        {
            case TClientVersion.cv176:
            case TClientVersion.cv185:
            case TClientVersion.cvHero:
            case TClientVersion.cvSerial:
            case TClientVersion.cvMirSequel:
            case TClientVersion.cvMirNewUI205:
                {
                    ConfigObject = PlugInSeam.CreateMirConfigDlg();
                    ConfigDlgList.AddObject("", ConfigObject);
                    // {$IF TESTMODE = 0}
                    for (int I = 0 /*Low(g_ConfigClient.ClientConfigs)*/; I <= ClientGlobalSeam.ClientConfigsHigh() /*High(...)*/; I++)
                    {
                        ConfigObject.ConfigCheckeds[(int)(TConfigChecked)I] = ClientGlobalSeam.ClientConfig(I);
                    }
                    // {$IFEND}
                    break;
                }
        }
    }

    /// <summary>
    /// 原文 <c>procedure TConfigDlgManage.UnLoadPlugIn();</c>（GameConfigDlgs.pas:109-117）：
    /// 逐个 <c>Free</c> 列表里的对象再 <c>Clear</c>。
    /// </summary>
    public void UnLoadPlugIn()
    {
        for (int I = 0; I <= ConfigDlgList.Count - 1; I++)
        {
            // 原文 TGameConfigObject(ConfigDlgList.Objects[I]).Free;
            // 托管侧由 GC 回收；这里只断开引用，语义与 Free 后 Clear 等价。
            ConfigDlgList.PutObject(I, null);
        }
        ConfigDlgList.Clear();
    }

    /// <summary>
    /// 原文 <c>procedure TConfigDlgManage.Initialize;</c>（GameConfigDlgs.pas:119-127）：
    /// <c>Initialize(frmMain.Handle, MakeLong(g_nScreenWidth, g_nScreenHeight), g_ClientVersion, g_boWindowMode)</c>。
    /// </summary>
    public void Initialize()
    {
        for (int I = 0; I <= ConfigDlgList.Count - 1; I++)
        {
            ((TGameConfigObject)ConfigDlgList.GetObject(I)).Initialize(ClientGlobalSeam.frmMainHandle,
                // 注意：原文 `MakeLong(g_nScreenWidth, g_nScreenHeight)` 的结果作为 Byte 参数传入
                // （原文声明的形参类型就是 Byte），这里用显式括号保证先 MakeLong 再截断，
                // 而不是把 (byte) 作用到第一个实参上（那会选中 uint 重载）。
                (byte)(GXX.Core.Rtl.DelphiRTL.MakeLong(ClientGlobalSeam.g_nScreenWidth, ClientGlobalSeam.g_nScreenHeight)),
                ClientGlobalSeam.g_ClientVersion, ClientGlobalSeam.g_boWindowMode);
        }
    }

    /// <summary>
    /// 原文 <c>procedure TConfigDlgManage.Finalize;</c>（GameConfigDlgs.pas:129-150）。
    ///
    /// **这是原文里最反直觉的一段（照抄）**：
    /// <c>btConfigDlgType = 0</c> 时对**不是** TJSYConfigDlg 的那些调用 Finalize；
    /// 否则（非 0）对**是** TJSYConfigDlg 的那个调用 Finalize。
    /// 即"只 Finalize 当前没在用的那一类对话框"。原文被 { } 注释掉的"全部 Finalize"版本保留为注释。
    /// </summary>
    public void Finalize()
    {
        for (int I = 0; I <= ConfigDlgList.Count - 1; I++)
        {
            if (ClientGlobalSeam.g_ClientConfig.btConfigDlgType == 0)
            {
                if (!(ConfigDlgList.GetObject(I) is TJSYConfigDlg))
                    ((TGameConfigObject)ConfigDlgList.GetObject(I)).Finalize();
            }
            else
            {
                if (ConfigDlgList.GetObject(I) is TJSYConfigDlg)
                    ((TGameConfigObject)ConfigDlgList.GetObject(I)).Finalize();
            }
        }

        /*
        for I := 0 to ConfigDlgList.Count - 1 do
        begin
          TGameConfigObject(ConfigDlgList.Objects[I]).Finalize;
        end;
        */
    }

    /// <summary>
    /// 原文 <c>function TConfigDlgManage.GetConfigDlg(ConfigDlgType:TConfigDlgType):TGameConfigObject;</c>
    /// （GameConfigDlgs.pas:152-168）。
    /// 语义要点：**先按类型精确查找，找不到时若列表非空则退回列表第 0 项**（不是返回 nil）。
    /// </summary>
    public TGameConfigObject GetConfigDlg(TConfigDlgType ConfigDlgType)
    {
        TGameConfigObject Result = null;
        for (int I = 0; I <= ConfigDlgList.Count - 1; I++)
        {
            TGameConfigObject ConfigObject = (TGameConfigObject)ConfigDlgList.GetObject(I);
            if (ConfigObject.ConfigDlgType == ConfigDlgType)
            {
                Result = ConfigObject;
                return Result;
            }
        }
        if (ConfigDlgList.Count > 0)
        {
            Result = (TGameConfigObject)ConfigDlgList.GetObject(0);
        }
        return Result;
    }

    // ================================================================================
    // C# 侧对 `property ConfigCheckeds[Index:TConfigChecked]` 的表达
    // --------------------------------------------------------------------------------
    // C# **不允许命名索引器**（索引器只能叫 this[]），故无法写出与原文同名的
    // `ConfigCheckeds[Index]`；改为在 TGameConfigObject 上暴露同名的 bool[] 属性，
    // 用法 `o.ConfigCheckeds[(int)idx]` 与原文 `o.ConfigCheckeds[idx]` 一一对应，
    // 数组由各实现类自己持有（原文抽象基类也不持有该数组）。
    // ================================================================================
}

/// <summary>
/// GameConfigDlgs.pas:29 <c>function LoadControlFromStream(ControlAddrList:THashedStringList; streamUI:TStream; sUiName:string = ''):Integer;</c>
/// （实现见 GameConfigDlgs.pas:51-62）。
///
/// 接缝：<c>THashedStringList</c> / <c>TStream</c> / <c>FrmDlg.DBackground</c> /
/// <c>LoadDxControlEx</c>（LoadDxControlEx.pas）均为未移植依赖，
/// 这里把它们压成一个最小委托 <see cref="LoadControlFromStreamSeam"/>，
/// **保留原文"先读默认 UI 内存流，再装载外部 UI，最后 Patch 默认 UI，finally 释放"的调用次序与返回值语义**。
/// </summary>
public static class GameConfigDlgs
{
    /// <summary>
    /// 原文 GameConfigDlgs.pas:51-62。
    /// 返回 <c>LoadDxControlEx.LoadControlFromStream(...)</c> 的返回值；
    /// <c>PatchLoadControlFromStream</c> 无返回值，其效果由接缝实现承担。
    /// </summary>
    public static int LoadControlFromStream(object ControlAddrList, object streamUI, string sUiName = "")
    {
        // 原文：msDefaultUI := LoadDxControlEx.LoadCompressedUIData('MIR_CONFIG_DLG_UI', 'ZDAT'); //必须存在，否则报错
        object msDefaultUI = LoadControlFromStreamSeam.LoadCompressedUIData("MIR_CONFIG_DLG_UI", "ZDAT");
        try
        {
            int Result = LoadControlFromStreamSeam.LoadControlFromStream(streamUI, ControlAddrList, sUiName);
            LoadControlFromStreamSeam.PatchLoadControlFromStream(msDefaultUI, ControlAddrList, sUiName);
            return Result;
        }
        finally
        {
            // 原文 msDefaultUI.Free;
            LoadControlFromStreamSeam.FreeMemoryStream(msDefaultUI);
        }
    }
}

/// <summary>
/// GameConfigDlgs.pas:32 的单元级全局 <c>ConfigDlgManage:TConfigDlgManage;</c>
/// （unit initialization 段 Create，finalization 段 Free）。
/// </summary>
public static class GameConfigDlgsGlobal
{
    /// <summary>原文 <c>ConfigDlgManage:TConfigDlgManage;</c></summary>
    public static readonly TConfigDlgManage ConfigDlgManage = new TConfigDlgManage();
}

/// <summary>
/// 接缝：LoadDxControlEx.pas（DxComponents 控件从压缩 UI 数据/流装载）。
/// 待该单元移植后接入；本车道只保留调用次序与返回值形状。
/// </summary>
public static class LoadControlFromStreamSeam
{
    /// <summary>接缝：<c>LoadDxControlEx.LoadCompressedUIData(sName, sExt):TMemoryStream</c>。</summary>
    public static Func<string, string, object> LoadCompressedUIData = (name, ext) => null;

    /// <summary>接缝：<c>LoadDxControlEx.LoadControlFromStream(streamUI, Parent, ControlAddrList, sUiName):Integer</c>。</summary>
    public static Func<object, object, string, int> LoadControlFromStream = (streamUI, controlAddrList, sUiName) => 0;

    /// <summary>接缝：<c>LoadDxControlEx.PatchLoadControlFromStream(memory, Parent, ControlAddrList, sUiName)</c>。</summary>
    public static Action<object, object, string> PatchLoadControlFromStream = (memory, controlAddrList, sUiName) => { };

    /// <summary>接缝：<c>TMemoryStream.Free</c>（托管侧为 GC，保留调用点以便将来对接真实实现）。</summary>
    public static Action<object> FreeMemoryStream = ms => { };
}

/// <summary>
/// 接缝：GameConfigDlgs.pas 依赖的客户端全局（g_ClientVersion / g_ConfigClient / frmMain /
/// g_nScreenWidth / g_nScreenHeight / g_boWindowMode）。
/// </summary>
public static class ClientGlobalSeam
{
    /// <summary>接缝：MShare.pas 的 <c>g_ClientVersion:TClientVersion</c>。</summary>
    public static TClientVersion g_ClientVersion = TClientVersion.cvMirs;

    /// <summary>接缝：MShare.pas 的 <c>g_ConfigClient:TConfigClient</c>；此处只保留配置数组读取。</summary>
    public static TClientConfig g_ClientConfig = new TClientConfig();

    /// <summary>接缝：<c>g_ConfigClient.ClientConfigs</c> 数组本身（GameConfigDlgs.pas:85/101）。</summary>
    public static bool[] ConfigClientConfigs = Array.Empty<bool>();

    /// <summary>原文 <c>Length(g_ConfigClient.ClientConfigs) - 1</c> 的上界。</summary>
    public static int ClientConfigsLength() => ConfigClientConfigs.Length;

    /// <summary>原文 <c>High(g_ConfigClient.ClientConfigs)</c>。</summary>
    public static int ClientConfigsHigh() => ConfigClientConfigs.Length - 1;

    /// <summary>原文 <c>g_ConfigClient.ClientConfigs[I]</c>。</summary>
    public static bool ClientConfig(int I) => ConfigClientConfigs[I];

    /// <summary>接缝：Client.dpr 的 <c>frmMain: TfrmMain</c>（ClMain.pas）的窗口句柄。</summary>
    public static IntPtr frmMainHandle = IntPtr.Zero;

    /// <summary>接缝：MShare.pas 的 <c>g_nScreenWidth:Integer</c>。</summary>
    public static int g_nScreenWidth;

    /// <summary>接缝：MShare.pas 的 <c>g_nScreenHeight:Integer</c>。</summary>
    public static int g_nScreenHeight;

    /// <summary>接缝：MShare.pas 的 <c>g_boWindowMode:Boolean</c>。</summary>
    public static bool g_boWindowMode;
}

/// <summary>
/// 接缝：配置对话框实现单元的工厂。
///
/// ★ 本接缝已**接线**（派发方授权的**唯一**改动点，见 p10-client-mirconfig 车道）：
///   - <c>JSYConfigDlg.pas</c> → <c>GXX.Client.GUI.GameConfig.Mir.TJSYRealConfigDlg</c>
///     （过渡实现：继承已翻译的 <c>TMirConfigDlg</c>，覆写 <c>GetType = ptJSY</c>；
///      JSY 自身的 6395 行逐行搬运仍未完成，见报告 §未完成/阻塞项）；
///   - <c>MirConfigDlg.pas</c> → <c>GXX.Client.GUI.GameConfig.Mir.TMirConfigDlg</c>
///     （真实现；`GetType = ptDefault`，含 516 个控件声明 + 279 条事件绑定对账表）。
///
/// 工厂仍保留为可注入委托，便于测试替换（原文是直接 `TMirConfigDlg.Create`，
/// 这里保留"可替换"只是为了可测，默认值就是真实现）。
/// </summary>
public static class PlugInSeam
{
    /// <summary>接缝：原文 <c>TJSYConfigDlg.Create</c>（JSYConfigDlg.pas）。**已接入真实现。**</summary>
    public static Func<TGameConfigObject> CreateJSYConfigDlg =
        () => new TJSYConfigDlg();

    /// <summary>接缝：原文 <c>TMirConfigDlg.Create</c>（MirConfigDlg.pas:1058）。**已接入真实现。**</summary>
    public static Func<TGameConfigObject> CreateMirConfigDlg =
        () => new MirDlg.TMirConfigDlg();
}

/// <summary>
/// 接缝：TJSYConfigDlg（JSYConfigDlg.pas）尚未移植时用于占位的配置对象。
/// 仅实现 <see cref="TGameConfigObject"/> 的抽象面，**不含任何业务逻辑**；
/// GameConfigDlgs.Finalize 的 <c>is TJSYConfigDlg</c> 类型判定由子类 <see cref="TJSYConfigDlg"/> 表达。
/// </summary>
public class TStubGameConfigObject : TGameConfigObject
{
    private readonly TConfigDlgType _type;
    private readonly bool[] _checkeds = new bool[TConfigCheckedBounds.HighOrdinal + 1];
    private bool _visible;
    private bool _enabled;
    private bool _protectEnabled;

    public TStubGameConfigObject(TConfigDlgType type) { _type = type; }

    /// <summary>对应原文子类的 <c>FConfigCheckeds:array[TConfigChecked] of Boolean</c>。</summary>
    public override bool[] ConfigCheckeds => _checkeds;

    /// <summary>观测用：Finalize 被调用次数（原文 <c>procedure Finalize;</c> 无可观测副作用）。</summary>
    public int FinalizeCalls;

    public override TConfigDlgType GetType() => _type;
    public override bool GetConfigChecked(TConfigChecked Index) => _checkeds[(int)Index];
    public override void SetConfigChecked(TConfigChecked Index, bool Value) => _checkeds[(int)Index] = Value;
    public override bool GetVisible() => _visible;
    public override void SetVisible(bool Value) => _visible = Value;
    public override bool GetEnabled() => _enabled;
    public override void SetEnabled(bool Value) => _enabled = Value;
    public override bool GetProtectEnabled() => _protectEnabled;
    public override void SetProtectEnabled(bool Value) => _protectEnabled = Value;
    public override void Open() { }
    public override void Close() { }
    public override void LoadConfig(string CharName) { }
    public override void Initialize(IntPtr Handle, byte ScreenMode, TClientVersion ClientVersion, bool WindowMode) { }
    public override void Finalize() { FinalizeCalls++; }
    public override void Logon(string ServerName) { }
    public override void Logout() { }
    public override bool FormKeyDown(ref ushort Key, DelphiShiftState Shift) => false;
    public override bool FormKeyPress(ref char Key) => false;
    public override void RefreshMySelfAbil() { }
    public override void RefreshMyHeroAbil() { }
    public override void RefreshMySelfMagicList() { }
    public override void RefreshMyHeroMagicList() { }
    public override void RefreshUnBindItemList() { }
    public override void RefKeyboardConfig() { }
    public override void Struck(object Actor, int HP, int MaxHP) { }
    public override void HealthChange(object Actor, int HP, int MP, int MaxHP) { }
    public override void LoadClientConfig(TClientConfig ClientConfig) { }
    public override void Run() { }
    public override void RefActorList() { }
    public override bool CanFilterExp(uint Exp) => false;
    public override TShowItem GetShowItem(string ItemName) => null;
    public override bool FindShowItem(string ItemName) => false;
    public override bool FindHintItem(string ItemName) => false;
    public override bool FindPickItem(string ItemName) => false;
    public override void HintItem(string ItemName, int X, int Y) { }
    public override void ClearShowItem() { }
    public override void RefShowItem() { }
    public override void AddToBossList(string sName) { }
    public override void RemoveFromBossList(string sName) { }
    public override void AddOrRemoveBossList(string sNamt) { }
}

// ================================================================================
// 【已收口】原 `public class TJSYConfigDlg : TStubGameConfigObject`
// --------------------------------------------------------------------------------
// 该桩**已被真实现替换**（派发方授权的唯一改动点，p10-client-mirconfig 车道）：
//   真实现 = GXX.Client.GUI.GameConfig.Mir.TJSYRealConfigDlg（继承已翻译的 TMirConfigDlg，
//   覆写 `GetType = ptJSY`；Create / 访问器 / Finalize / Logout / SaveConfigFile 都是真实体）。
//   下面这个同名类**只是它的公开名字**（保持原命名空间与原类名，
//   使既有调用点与测试里的 `new TJSYConfigDlg()` / `is TJSYConfigDlg` 不必改动），
//   自身不再有任何桩逻辑。
// GameConfigDlgs.Finalize 的 `is TJSYConfigDlg` 判定与 AddObject 次序**完全不变**。
// 保留 TStubGameConfigObject（仍有测试与接缝默认值在用）。
// ================================================================================

/// <summary>
/// JSYConfigDlg.pas 的 <c>TJSYConfigDlg</c> 的公开名字（真实现见
/// <see cref="MirDlg.TJSYRealConfigDlg"/>）。**不再是桩。**
///
/// <c>FinalizeCalls</c> 是**观测计数器**（原 TStubGameConfigObject 上的测试钩子）：
/// 真实现的 <c>Finalize</c>（原文 5046-5050 → <c>SaveConfigFile</c>）**照常执行**，
/// 计数只是叠加在上面，不改变任何行为。
/// </summary>
public class TJSYConfigDlg : MirDlg.TJSYRealConfigDlg
{
    public TJSYConfigDlg() { }

    /// <summary>观测用：<c>Finalize</c> 被调用次数（行为不变，仅计数）。</summary>
    public int FinalizeCalls;

    /// <inheritdoc/>
    public override void Finalize()
    {
        base.Finalize();
        FinalizeCalls++;
    }
}
