using System;
using System.Collections.Generic;
using GXX.Client.GUI.GameConfig.Seams;
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
/// 接缝：后续批次的配置对话框实现单元
/// （JSYConfigDlg.pas 的 TJSYConfigDlg、MirConfigDlg.pas 的 TMirConfigDlg）。
/// </summary>
public static class PlugInSeam
{
    /// <summary>接缝：待 JSYConfigDlg.pas 移植后接入 <c>TJSYConfigDlg.Create</c>。</summary>
    public static Func<TGameConfigObject> CreateJSYConfigDlg = () => new TStubGameConfigObject(TConfigDlgType.ptJSY);

    /// <summary>接缝：待 MirConfigDlg.pas 移植后接入 <c>TMirConfigDlg.Create</c>。</summary>
    public static Func<TGameConfigObject> CreateMirConfigDlg = () => new TStubGameConfigObject(TConfigDlgType.ptDefault);
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

/// <summary>
/// 接缝：JSYConfigDlg.pas 的 <c>TJSYConfigDlg</c>。
/// 原类未移植；这里只作为**类型标记**出现，以便 GameConfigDlgs.Finalize 的
/// <c>is TJSYConfigDlg</c> 判定保持与原文一致。接入真实实现时改为继承真实基类。
/// </summary>
public class TJSYConfigDlg : TStubGameConfigObject
{
    public TJSYConfigDlg() : base(TConfigDlgType.ptJSY) { }
}
