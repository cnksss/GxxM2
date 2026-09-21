// ============================================================================
// 源单元：Source/M2Engine/uAliyunSendSMSThread.pas（320 行，GBK）
// 本文件：TAliyunSendSMSThread（:16-38 声明 / :75-257 实现）+ LoadSendSMSConfig（:259-317）
// 接缝与全局见 Sweep9FormsSmsSeams.cs（含全部依赖缺口的登记）。
// ============================================================================

using System.Threading;
using GXX.Core.Protocol;
using GXX.M2Server.Engine;
using GXX.M2Server.Sweep;

namespace GXX.M2Server.Sweep9.Forms;

/// <summary>
/// 原文 `uAliyunSendSMSThread.pas:16-38 TAliyunSendSMSThread = class(TThread)` 1:1。
/// </summary>
public sealed class TAliyunSendSMSThread : IDisposable
{
    // ==================================================================
    // 原文 :17-27 private 字段
    // ==================================================================

    /// <summary>原文 `:18 FTaskList: TSafeList`（既有 <see cref="TSafeList"/>；原文 `TList` 子类）。</summary>
    public TSafeList FTaskList = null!;

    /// <summary>原文 `:19 FEvent: TEvent`（自动重置、初始未触发）。</summary>
    public ISweep9FormsEvent FEvent = null!;

    /// <summary>原文 `:20 FInExecuteLoop: Boolean`（`Sleeping` 的判据：`not FInExecuteLoop`）。</summary>
    public bool FInExecuteLoop;

    /// <summary>原文 `:22 FSendSmsRequest: TSendSmsRequest`（接缝，构造期 `Create`）。</summary>
    public ISweep9FormsSendSmsRequest FSendSmsRequest = null!;

    /// <summary>原文 `:29 protected procedure Execute; override;` 的线程体（托管侧持真实线程）。</summary>
    private Thread? _thread;

    /// <summary>
    /// 原文 `:33 procedure Terminate; reintroduce; virtual;` 的终止标志（`TThread.Terminated`）。
    /// </summary>
    public bool Terminated;

    // ==================================================================
    // 接缝（原文依赖的未移植单元）
    // ==================================================================

    /// <summary>
    /// 接缝：`UserEngine.GetPlayObject(Player): TPlayObject`（:181）。
    /// 返回 `null` = 原文 `Player2 = nil`。**未接线即抛**（§25.2）。
    /// </summary>
    public Func<TCreature, ISweep9FormsSmsPlayer?>? GetPlayObjectHandler;

    /// <summary>接缝：`g_FunctionNPC`（:230，M2Share.pas）。为 `null` 时等价原文 `g_FunctionNPC = nil`（跳过）。</summary>
    public ISweep9FormsFunctionNpc? g_FunctionNPC;

    /// <summary>接缝：`MainOutMessage`（:211/:213）。默认落既有 `SweepSeam.MainOutMessage`。</summary>
    public Action<string> MainOutMessage = msg => SweepSeam.MainOutMessage(msg);

    /// <summary>接缝：`MyGetTickCount`（:205）。默认落既有 `SweepSeam.MyGetTickCount`。</summary>
    public Func<uint> MyGetTickCount = () => SweepSeam.MyGetTickCount();

    /// <summary>
    /// 接缝：`Randomize; Random(900000)`（:188-189）。默认用 CLR `Random`；
    /// 测试注入定值以锁死 `100000 + Random(900000)` 的取值。
    /// </summary>
    public Func<int, int> Random = n => System.Random.Shared.Next(n);

    /// <summary>`UserEngine.GetPlayObject`；未接线即抛。</summary>
    private ISweep9FormsSmsPlayer? GetPlayObject(TCreature player)
        => (GetPlayObjectHandler ?? throw Sweep9FormsKit.NotWired(
            nameof(GetPlayObjectHandler), "UsrEngn.pas UserEngine.GetPlayObject"))(player);

    // ==================================================================
    // :75-90 constructor Create
    // ==================================================================

    /// <summary>原文 `:75-90 constructor TAliyunSendSMSThread.Create;`。</summary>
    public TAliyunSendSMSThread()
    {
        // 原文 inherited Create(False);（:77）—— **构造即起线程**
        if (Sweep9FormsSmsSeams.StartThreadOnCreate)
        {
            _thread = new Thread(Execute) { IsBackground = true };
            _thread.Start();
        }

        // 原文 FreeOnTerminate := False;（:78）—— 托管侧由调用方 Dispose（不自动回收）
        FTaskList = new TSafeList("TAliyunSendSMSThread.Locker");              // :79

        FSendSmsRequest = Sweep9FormsSmsSeams.CreateSendSmsRequest();            // :81 TSendSmsRequest.Create
        FSendSmsRequest.SignName = Sweep9FormsSmsGlobals.g_SendSMSConfig.SignName;                  // :82

        // 原文 :84-88 注释（TEvent.Create 的参数含义）逐字保留在文档注释里：
        //   {
        //     TEvent.Create
        //     参数2, Fase 事件对象控制一次后将立即重置(暂停); True 可手动暂停
        //     参数3, Fase 对象建立后控制为暂停状态; True 可运行状态
        //   }
        FEvent = Sweep9FormsSmsSeams.CreateEvent();                            // :89 TEvent.Create(nil, False, False, '')
    }

    // ==================================================================
    // :92-99 destructor Destroy
    // ==================================================================

    /// <summary>原文 `:92-99 destructor TAliyunSendSMSThread.Destroy;`。</summary>
    public void Destroy()
    {
        // 原文 FSendSmsRequest.Free;（:94）
        FSendSmsRequest = null!;
        ClearTaskList();                                                       // :95
        FTaskList.Dispose();                                                   // :96 FTaskList.Free
        FEvent.Dispose();                                                      // :97 FEvent.Free
        // 原文 inherited;（:98）
    }

    /// <summary>`IDisposable` 形态的 `Free`（等价 `Destroy`）。</summary>
    public void Dispose() => Destroy();

    // ==================================================================
    // :101-105 Terminate
    // ==================================================================

    /// <summary>原文 `:101-105 procedure TAliyunSendSMSThread.Terminate;`。</summary>
    public void Terminate()
    {
        // 原文 inherited Terminate;（:103）—— 置 TThread.Terminated
        Terminated = true;
        TriggerEvent();                                                        // :104
    }

    // ==================================================================
    // :107-125 AddTask
    // ==================================================================

    /// <summary>原文 `:107-125 procedure TAliyunSendSMSThread.AddTask(Player, Npc: TBaseObject);`。</summary>
    public void AddTask(TCreature? Player, TCreature? Npc)
    {
        // 原文 var SMSTask: PSMSTask;
        FTaskList.LockW(1);                                                    // :111
        try
        {
            // 原文 New(SMSTask); SMSTask.Player := Player; SMSTask.Npc := Npc; FTaskList.Add(SMSTask);
            var SMSTask = new TSMSTask { Player = Player, Npc = Npc };
            FTaskList.Add(SMSTask);
        }
        finally
        {
            FTaskList.UnLockW();                                               // :118
        }

        // 原文 if GetSleeping then TriggerEvent;（:121-124）
        if (GetSleeping())
        {
            TriggerEvent();
        }
    }

    // ==================================================================
    // :127-140 Execute
    // ==================================================================

    /// <summary>
    /// 原文 `:127-140 procedure TAliyunSendSMSThread.Execute;`。
    /// <para>★ `:130-139` 循环条件只在**顶部**判 `Terminated` ⇒ `Terminate` 之后还会再跑一轮。</para>
    /// </summary>
    public void Execute()
    {
        // 原文 inherited;（:129）
        while (!Terminated)                                                    // :130
        {
            FEvent.WaitFor(uint.MaxValue);                                     // :132 FEvent.WaitFor(INFINITE)
            FInExecuteLoop = true;                                             // :133
            try
            {
                DoExecuteLoop();                                               // :135
            }
            finally
            {
                FInExecuteLoop = false;                                        // :137
            }
        }
    }

    // ==================================================================
    // :142-150 GetSleeping / TriggerEvent
    // ==================================================================

    /// <summary>原文 `:142-145 function TAliyunSendSMSThread.GetSleeping: Boolean;`。</summary>
    public bool GetSleeping()
        => !FInExecuteLoop;                                                    // :144

    /// <summary>原文 `:35 property Sleeping: Boolean read GetSleeping;`。</summary>
    public bool Sleeping => GetSleeping();

    /// <summary>原文 `:147-150 procedure TAliyunSendSMSThread.TriggerEvent;`。</summary>
    public void TriggerEvent()
        => FEvent.SetEvent();                                                  // :149

    // ==================================================================
    // :152-239 DoExecuteLoop
    // ==================================================================

    /// <summary>原文 `:152-239 procedure TAliyunSendSMSThread.DoExecuteLoop;`。</summary>
    public void DoExecuteLoop()
    {
        // 原文 var SMSTask: PSMSTask; Player, Npc: TBaseObject; Player2: TPlayObject;
        //        VerifyCode, ErrorCode, ErrorMsg, TemplateCode: string;
        //        // Merchant: TMerchant;   ← 原文 :158 被注释掉的局部变量
        TCreature? Player;
        TCreature? Npc;
        ISweep9FormsSmsPlayer? Player2;
        string VerifyCode, ErrorCode, ErrorMsg, TemplateCode;

        Player = null;                                                         // :160
        Npc = null;                                                            // :161
        FTaskList.LockW(2);                                                    // :162
        try
        {
            if (FTaskList.Count > 0)                                           // :164
            {
                var SMSTask = (TSMSTask)FTaskList[0]!;                         // :166 FTaskList.Items[0]
                Player = SMSTask.Player;                                       // :167
                Npc = SMSTask.Npc;                                             // :168

                // 原文 Dispose(SMSTask);（:170）—— 托管侧由 GC 回收
                FTaskList.Delete(0);                                           // :171
            }
        }
        finally
        {
            FTaskList.UnLockW();                                               // :174
        }

        if (Player == null)                                                    // :177
            return;                                                            // 原文 Exit（**在 try 之前** ⇒ 不 TriggerEvent）

        // 原文 try ... finally TriggerEvent; end;（:180/:236-238）
        try
        {
            Player2 = GetPlayObject(Player);                                   // :181
            if (Player2 == null || Player2.m_boGhost)                          // :182
                return;                                                        // 原文 Exit（finally 仍会 TriggerEvent）

            if (Player2.m_sMobileNumber.Length == 0)                           // :185 Length(...) = 0
                return;                                                        // 原文 Exit

            // 原文 Randomize;（:188）—— 托管侧 Random 自身即按进程随机化
            // 原文 VerifyCode := IntToStr(100000 + Random(900000));（:189）
            VerifyCode = GXX.Core.Rtl.DelphiRTL.IntToStr(100000 + Random(900000));

            // ★ 原文 :191-194 名字与直觉相反：**未绑定** ⇒ 用"绑定"模板
            if (!Player2.m_boMobileBind)
                TemplateCode = Sweep9FormsSmsGlobals.g_SendSMSConfig.TemplateCodeBind;
            else
                TemplateCode = Sweep9FormsSmsGlobals.g_SendSMSConfig.TemplateCodeCheck;

            FSendSmsRequest.TemplateCode = TemplateCode;                       // :196

            // 原文 :198 注释：阿里的这个参数，name: 不支持中文★★★★★★★★，不要角色名 chongchong 2018-06-03
            // 原文 :200 被注释掉的带角色名的调用（逐字保留）：
            //   // if FSendSmsRequest.Request(Player2.m_sMobileNumber,
            //   //   Format('{"name":"%s", "code":"%s"}', [Player2.m_sCharName, VerifyCode]),
            //   //   ErrorCode, ErrorMsg) then
            if (FSendSmsRequest.Request(Player2.m_sMobileNumber,
                    GXX.Core.Rtl.DelphiRTL.Format("{\"code\":\"%s\"}", VerifyCode),
                    out ErrorCode, out ErrorMsg))                               // :202
            {
                Player2.m_sMobileVerifyCode = VerifyCode;                       // :204
                Player2.m_dwMobileVerifyTick = MyGetTickCount();                // :205

                // 原文 Player2.SendMsg(Player2, RM_INPUTMOBILE_VerifyCode, 0, 0, 0, 0, '');
                Player2.SendMsg(Player, Grobal2Const.RM_INPUTMOBILE_VerifyCode, 0, 0, 0, 0, "");
            }
            else if (Npc != null)                                               // :209
            {
                // 原文 MainOutMessage(Format('发送短信失败, SignName: %s; TemplateCode: %s; ...', [...]));（:211-212）
                MainOutMessage(GXX.Core.Rtl.DelphiRTL.Format(
                    "发送短信失败, SignName: %s; TemplateCode: %s; MobilePhone: %s; 错误代码: %s; 错误原因: %s",
                    FSendSmsRequest.SignName, FSendSmsRequest.TemplateCode,
                    Player2.m_sMobileNumber, ErrorCode, ErrorMsg));
                MainOutMessage(Sweep9FormsSmsGlobals.g_RequestStr);             // :213
                // 原文 :214-229 一大段被 { } 注释掉的 m_MerchantList 遍历（逐字保留）：
                //   {
                //     UserEngine.m_MerchantList.LockR(100);
                //     try
                //     for I := 0 to UserEngine.m_MerchantList.Count - 1 do
                //     begin
                //     Merchant := TMerchant(UserEngine.m_MerchantList.Items[I]);
                //     if Merchant = Npc then
                //     begin
                //     Merchant.GotoLable(Player2, '@MobileVerifyCodeSendFail', False);
                //     Break;
                //     end;
                //     end;
                //     finally
                //     UserEngine.m_MerchantList.UnLockR;
                //     end;
                //   }
                // ★ 原文 :230-234 用的是 **g_FunctionNPC**（不是任务里的 Npc）
                if (g_FunctionNPC != null)                                     // :230
                {
                    Player2.m_nScriptGotoCount = 0;                            // :232
                    g_FunctionNPC.GotoLable(Player2, "@MobileVerifyCodeSendFail", false);   // :233
                }
            }
        }
        finally
        {
            TriggerEvent();                                                     // :237
        }
    }

    // ==================================================================
    // :241-257 ClearTaskList
    // ==================================================================

    /// <summary>原文 `:241-257 procedure TAliyunSendSMSThread.ClearTaskList;`。</summary>
    public void ClearTaskList()
    {
        FTaskList.LockW(3);                                                    // :246
        try
        {
            for (int I = 0; I < FTaskList.Count; I++)                           // :248
            {
                // 原文 SMSTask := FTaskList.Items[I]; Dispose(SMSTask);（:250-251）—— GC 回收
                _ = FTaskList[I];
            }
            FTaskList.Clear();                                                 // :253
        }
        finally
        {
            FTaskList.UnLockW();                                               // :255
        }
    }

    // ==================================================================
    // :259-317 LoadSendSMSConfig（单元级过程）
    // ==================================================================

    /// <summary>
    /// 原文 `:259-317 procedure LoadSendSMSConfig;`（**单元级**过程 ⇒ 托管侧静态方法）。
    /// </summary>
    public static void LoadSendSMSConfig()
    {
        // 原文 var FileName: string; IniFile: TIniFile;
        string FileName;

        // 原文 FileName := ExtractFilePath(Application.ExeName) + 'SendSMS.ini';（:264）
        FileName = M2ShareState.g_sSelfFilePath + "SendSMS.ini";

        // 原文 IniFile := TIniFile.Create(FileName);（:266）
        var IniFile = Sweep9FormsSmsSeams.CreateIniFile(FileName);
        try
        {
            // 原文 :268-269 无条件清理两个历史键（迁移残留）
            IniFile.DeleteKey("aliyun", "Endpoint");
            IniFile.DeleteKey("aliyun", "Topic");

            // ---- KeyID（:271-274）----
            if (!IniFile.ValueExists("aliyun", "KeyID"))
                IniFile.WriteString("aliyun", "KeyID", Sweep9FormsSmsGlobals.g_AcsUtil.KeyID);
            else
                Sweep9FormsSmsGlobals.g_AcsUtil.KeyID =
                    IniFile.ReadString("aliyun", "KeyID", Sweep9FormsSmsGlobals.g_AcsUtil.KeyID);

            // ---- KeySecret（:276-279）----
            if (!IniFile.ValueExists("aliyun", "KeySecret"))
                IniFile.WriteString("aliyun", "KeySecret", Sweep9FormsSmsGlobals.g_AcsUtil.KeySecret);
            else
                Sweep9FormsSmsGlobals.g_AcsUtil.KeySecret =
                    IniFile.ReadString("aliyun", "KeySecret", Sweep9FormsSmsGlobals.g_AcsUtil.KeySecret);

            // ---- SignName（:281-284）----
            if (!IniFile.ValueExists("aliyun", "SignName"))
                IniFile.WriteString("aliyun", "SignName", Sweep9FormsSmsGlobals.g_SendSMSConfig.SignName);
            else
                Sweep9FormsSmsGlobals.g_SendSMSConfig.SignName =
                    IniFile.ReadString("aliyun", "SignName", Sweep9FormsSmsGlobals.g_SendSMSConfig.SignName);

            // ---- TemplateCodeBind（:286-289）----
            if (!IniFile.ValueExists("aliyun", "TemplateCodeBind"))
                IniFile.WriteString("aliyun", "TemplateCodeBind", Sweep9FormsSmsGlobals.g_SendSMSConfig.TemplateCodeBind);
            else
                Sweep9FormsSmsGlobals.g_SendSMSConfig.TemplateCodeBind =
                    IniFile.ReadString("aliyun", "TemplateCodeBind", Sweep9FormsSmsGlobals.g_SendSMSConfig.TemplateCodeBind);

            // ---- TemplateCodeCheck（:291-294）----
            if (!IniFile.ValueExists("aliyun", "TemplateCodeCheck"))
                IniFile.WriteString("aliyun", "TemplateCodeCheck", Sweep9FormsSmsGlobals.g_SendSMSConfig.TemplateCodeCheck);
            else
                Sweep9FormsSmsGlobals.g_SendSMSConfig.TemplateCodeCheck =
                    IniFile.ReadString("aliyun", "TemplateCodeCheck", Sweep9FormsSmsGlobals.g_SendSMSConfig.TemplateCodeCheck);

            // ---- ResendVerifyCodeCount（:296-299，**整数键**）----
            if (!IniFile.ValueExists("aliyun", "ResendVerifyCodeCount"))
                IniFile.WriteInteger("aliyun", "ResendVerifyCodeCount", Sweep9FormsSmsGlobals.g_SendSMSConfig.ResendVerifyCodeCount);
            else
                Sweep9FormsSmsGlobals.g_SendSMSConfig.ResendVerifyCodeCount =
                    IniFile.ReadInteger("aliyun", "ResendVerifyCodeCount", Sweep9FormsSmsGlobals.g_SendSMSConfig.ResendVerifyCodeCount);

            // ---- VerifySendInterval（:301-304）----
            if (!IniFile.ValueExists("aliyun", "VerifySendInterval"))
                IniFile.WriteInteger("aliyun", "VerifySendInterval", Sweep9FormsSmsGlobals.g_SendSMSConfig.VerifySendInterval);
            else
                Sweep9FormsSmsGlobals.g_SendSMSConfig.VerifySendInterval =
                    IniFile.ReadInteger("aliyun", "VerifySendInterval", Sweep9FormsSmsGlobals.g_SendSMSConfig.VerifySendInterval);

            // ---- VerifyCodeTimeOutTime（:306-309）----
            if (!IniFile.ValueExists("aliyun", "VerifyCodeTimeOutTime"))
                IniFile.WriteInteger("aliyun", "VerifyCodeTimeOutTime", Sweep9FormsSmsGlobals.g_SendSMSConfig.VerifyCodeTimeOutTime);
            else
                Sweep9FormsSmsGlobals.g_SendSMSConfig.VerifyCodeTimeOutTime =
                    IniFile.ReadInteger("aliyun", "VerifyCodeTimeOutTime", Sweep9FormsSmsGlobals.g_SendSMSConfig.VerifyCodeTimeOutTime);

            // ---- VerifyCodeErrorCount（:311-314）----
            if (!IniFile.ValueExists("aliyun", "VerifyCodeErrorCount"))
                IniFile.WriteInteger("aliyun", "VerifyCodeErrorCount", Sweep9FormsSmsGlobals.g_SendSMSConfig.VerifyCodeErrorCount);
            else
                Sweep9FormsSmsGlobals.g_SendSMSConfig.VerifyCodeErrorCount =
                    IniFile.ReadInteger("aliyun", "VerifyCodeErrorCount", Sweep9FormsSmsGlobals.g_SendSMSConfig.VerifyCodeErrorCount);
        }
        finally
        {
            IniFile.Dispose();                                                 // :316 IniFile.Free
        }
    }
}

/// <summary>
/// `ISweep9FormsSendSmsRequest` 的**空实现**：仅当调用方既没注入 `CreateSendSmsRequest`
/// 又不做任何发送时才会用到（构造期 `Create` 必须有对象，原文 `TSendSmsRequest.Create` 是真对象）。
/// <para>
/// **不提供"静默成功"**：`Request` 直接抛"未接线"（§25.2）。
/// `SignName`/`TemplateCode` 可读写（原文构造期就写 `SignName`）。
/// </para>
/// </summary>
public sealed class Sweep9FormsNullSendSmsRequest : ISweep9FormsSendSmsRequest
{
    /// <inheritdoc />
    public string SignName { get; set; } = "";
    /// <inheritdoc />
    public string TemplateCode { get; set; } = "";

    /// <inheritdoc />
    public bool Request(string mobilePhone, string paramStr, out string errorCode, out string errorMsg)
        => throw Sweep9FormsKit.NotWired(
            nameof(ISweep9FormsSendSmsRequest), "SendSmsRequest.pas TSendSmsRequest.Request");
}
