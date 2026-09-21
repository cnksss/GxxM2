// 源单元：Source/Client-HGE/GameConfig/Mir/MirConfigDlg.pas（GBK，8,355 行，CRLF）
// 本分片覆盖（原文行号）：
//   5046-5050  Finalize
//   5052-5063  Logon
//   5065-5078  LoadConfig
//   5080-5083  Run
//   5085-5089  RefActorList（空实现，在 Misc 分片）
//   5090-5132  GetShowItem / FindShowItem / FindHintItem / FindPickItem / HintItem
//   5133-5187  Struck / HealthChange
//   6300-6327  AutoUseMagic / CanFilterExp
//   4535-5044  Initialize（控件装配 + 279 条绑定已由构建器完成，本文件补 UI 流与补丁段）
//
// ★ 本文件的成员**已 1:1 移入**（非骨架）。

using System;
using GXX.Client.GUI.GameConfig.Seams;

namespace GXX.Client.GUI.GameConfig.Mir;

public partial class TMirConfigDlg
{
    // ================================================================================
    // 5046-5063  Finalize / Logon
    // ✓ 已 1:1 移入
    // ================================================================================

    /// <summary>
    /// 原文 5046-5050：<c>SaveConfigFile; // FInitializeed := False;</c>
    /// ★ 注意末行是**被注释掉**的 —— 原文不重置 FInitializeed（原文如此）。
    /// </summary>
    public override void Finalize()
    {
        SaveConfigFile();                                      // 5048
        // FInitializeed := False;                             // 5049 原文如此（注释掉）
    }

    /// <summary>
    /// 原文 5052-5063：
    /// <c>g_sPlugServerName := ServerName;</c> →
    /// <c>sDirectory := g_sSelfFilePath + 'Config\';</c>（不存在则建）→
    /// <c>sFileName := sDirectory + g_sPlugServerName + DecodeResStr(SBindItemFileName);</c> →
    /// <c>LoadNGCustomUnbindItemList(sFileName); RefBindItemList;</c>
    /// </summary>
    public override void Logon(string ServerName)
    {
        MirConfigGlobalSeam.g_sPlugServerName = ServerName;                              // 5056

        string sDirectory = MirConfigGlobalSeam.g_sSelfFilePath + "Config\\";            // 5058
        if (!MirConfigGlobalSeam.DirectoryExists(sDirectory))                            // 5059
            MirConfigGlobalSeam.ForceDirectories(sDirectory);                            // 5059
        string sFileName = sDirectory + MirConfigGlobalSeam.g_sPlugServerName
                         + MirConfigGlobalSeam.DecodeResStr(MirActorSeam.SBindItemFileName);   // 5060
        MirActorSeam.LoadNGCustomUnbindItemList(sFileName);                              // 5061
        RefBindItemList();                                                               // 5062
    }

    // ================================================================================
    // 5065-5083  LoadConfig / Run
    // ✓ 已 1:1 移入
    // ================================================================================

    /// <summary>
    /// 原文 5065-5078：
    /// <c>g_sPlugUserName := ProcessFileNameSpecialChar(CharName);</c> →
    /// <c>if FLoadControl and (not FLoadConfig) then</c> 六步
    /// （FLoadConfig=True / LoadFormFile / LoadConfigFile / RefConfig /
    ///  SendPlugInConfig(nHeroDodgeHPPercent) / RefShowItem / FEnabled=True）。
    /// ★ 注意 <c>FLoadConfig := True</c> 写在**最前**，且门禁是
    ///   <c>FLoadControl and (not FLoadConfig)</c> —— 只加载一次。
    /// </summary>
    public override void LoadConfig(string CharName)
    {
        MirConfigGlobalSeam.g_sPlugUserName = MirActorSeam.ProcessFileNameSpecialChar(CharName);   // 5067

        if (FLoadControl && (!FLoadConfig))                    // 5069
        {
            FLoadConfig = true;                                // 5070
            FilterItemsGlobal.g_FileItemDB.LoadFormFile();     // 5071
            LoadConfigFile();                                  // 5072
            RefConfig();                                       // 5073
            MirConfigGlobalSeam.SendPlugInConfig(g_Config.nHeroDodgeHPPercent);   // 5074
            RefShowItem();                                     // 5075
            FEnabled = true;                                   // 5076
        }
    }

    /// <summary>原文 5080-5083：<c>AutoUseItem(Self);</c> —— 只有一行，参数是 Self。</summary>
    public override void Run() => AutoUseItem(this);           // 5082

    // ================================================================================
    // 5090-5132  物品查询转发
    // ✓ 已 1:1 移入
    // ================================================================================

    /// <summary>原文 5090-5093。</summary>
    public override TShowItem GetShowItem(string ItemName) => FilterItemsGlobal.g_FileItemDB.Find(ItemName);

    /// <summary>
    /// 原文 5095-5104：
    /// <c>ShowItem := g_FileItemDB.Find(ItemName); if ShowItem &lt;&gt; nil then Result := ShowItem.boShowName else Result := False;</c>
    /// ★ 原文**没有**把 <c>boShowName</c>（Byte）转成 Boolean，而是直接赋给 Boolean 的 Result ——
    ///   Delphi 里这是隐式 <c>&lt;&gt; 0</c>。托管侧照抄（<c>!= 0</c>）。
    /// </summary>
    public override bool FindShowItem(string ItemName)
    {
        TShowItem ShowItem = FilterItemsGlobal.g_FileItemDB.Find(ItemName);   // 5099
        if (ShowItem != null)                                  // 5100
            return ShowItem.boShowName != 0;                   // 5101
        return false;                                          // 5103
    }

    /// <summary>原文 5106-5115：同 <see cref="FindShowItem"/>，取 <c>boHintMsg</c>。</summary>
    public override bool FindHintItem(string ItemName)
    {
        TShowItem ShowItem = FilterItemsGlobal.g_FileItemDB.Find(ItemName);   // 5110
        if (ShowItem != null)                                  // 5111
            return ShowItem.boHintMsg != 0;                    // 5112
        return false;                                          // 5114
    }

    /// <summary>原文 5117-5126：同 <see cref="FindShowItem"/>，取 <c>boPickup</c>。</summary>
    public override bool FindPickItem(string ItemName)
    {
        TShowItem ShowItem = FilterItemsGlobal.g_FileItemDB.Find(ItemName);   // 5121
        if (ShowItem != null)                                  // 5122
            return ShowItem.boPickup != 0;                     // 5123
        return false;                                          // 5125
    }

    /// <summary>原文 5128-5131。</summary>
    public override void HintItem(string ItemName, int X, int Y)
        => FilterItemsGlobal.g_FileItemDB.Hint(ItemName, X, Y);   // 5130

    // ================================================================================
    // 5133-5187  Struck / HealthChange
    // ✓ 已 1:1 移入
    // ================================================================================

    /// <summary>
    /// 原文 5133-5156：<c>Struck</c>（受击 → 按掉落量喝药）。
    /// 语义照抄：
    ///  <c>if FEnabled then if g_MySelf &lt;&gt; nil then</c>
    ///   本人分支：<c>nDamage := Integer(g_MySelf.m_Abil.HP) - HP; if nDamage &gt; 0 then DamageHPUseItem(0, nDamage);</c>
    ///   英雄分支：<c>nDamage := HP;</c>（★ **不是差值**，原文如此）<c>if nDamage &gt; 0 then DamageHPUseItem(1, nDamage);</c>
    /// ★ 英雄分支**嵌在** <c>g_MySelf &lt;&gt; nil</c> 之内（原文如此：g_MySelf 为 nil 时英雄也不处理），
    ///   与 MirsConfigDlg/MirReturnConfigDlg 同一处照抄。
    /// </summary>
    public override void Struck(object Actor, int HP, int MaxHP)
    {
        if (FEnabled)                                          // 5138
        {
            if (MirActorSeam.MySelfExists())                   // 5139
            {
                if (ReferenceEquals(Actor, MySelfActorIdentity()))  // 5140  Actor = g_MySelf
                {
                    int nDamage = MirActorSeam.MySelfHP() - HP;  // 5141
                    if (nDamage > 0)                           // 5142
                        DamageHPUseItem(0, nDamage);           // 5143
                }

                if (MirActorSeam.MyHeroExists())               // 5146
                {
                    if (ReferenceEquals(Actor, MyHeroActorIdentity()))   // 5147
                    {
                        int nDamage = HP;                      // 5148  ★ 原文如此：取 HP 而非差值
                        if (nDamage > 0)                       // 5149
                            DamageHPUseItem(1, nDamage);       // 5150
                    }
                }
            }
        }
    }

    /// <summary>
    /// 原文 5158-5187：<c>HealthChange</c>（HP/MP 同时变化 → 两条 Damage* 路径）。
    /// 本人与英雄各自读 <c>m_Abil.HP</c>/<c>m_Abil.MP</c> 求差；英雄分支同样嵌在 <c>g_MySelf &lt;&gt; nil</c> 内。
    /// </summary>
    public override void HealthChange(object Actor, int HP, int MP, int MaxHP)
    {
        if (FEnabled)                                          // 5162
        {
            if (MirActorSeam.MySelfExists())                   // 5163
            {
                if (ReferenceEquals(Actor, MySelfActorIdentity()))  // 5164
                {
                    int nDamage = MirActorSeam.MySelfHP() - HP;   // 5165
                    if (nDamage > 0)                           // 5166
                        DamageHPUseItem(0, nDamage);           // 5167

                    nDamage = MirActorSeam.MySelfMP() - MP;    // 5169
                    if (nDamage > 0)                           // 5170
                        DamageMPUseItem(0, nDamage);           // 5171
                }

                if (MirActorSeam.MyHeroExists())               // 5174
                {
                    if (ReferenceEquals(Actor, MyHeroActorIdentity()))   // 5175
                    {
                        int nDamage = MirActorSeam.MyHeroHP() - HP;   // 5176
                        if (nDamage > 0)                       // 5177
                            DamageHPUseItem(1, nDamage);       // 5178

                        nDamage = MirActorSeam.MyHeroMP() - MP;   // 5180
                        if (nDamage > 0)                       // 5181
                            DamageMPUseItem(1, nDamage);       // 5182
                    }
                }
            }
        }
    }

    // ================================================================================
    // 6300-6327  AutoUseMagic / CanFilterExp
    // ✓ 已 1:1 移入
    // ================================================================================

    /// <summary>
    /// 原文 6300-6319：<c>AutoUseMagic</c>。
    /// <c>if FConfigCheckeds[ckAutoUseMagic] then</c> → <c>g_MySelf &lt;&gt; nil</c> 且**非死亡**且**非摆摊** →
    /// 下拉下标区间校验 → 时间间隔门禁（<c>Cardinal(nAutoUseMagicTime) * 1000</c>）→
    /// 取 <c>Items.Objects[ItemIndex]</c> 当 <c>pTClientMagic</c> →
    /// <c>frmMain.AutoTakeOnItem</c> + <c>frmMain.UseMagic(g_nMouseX, g_nMouseY, ClientMagic)</c>。
    /// ★ 原文的 <c>Sender</c> 形参在本方法里**未被使用**（原文如此）。
    /// </summary>
    public void AutoUseMagic(object Sender)
    {
        if (FConfigCheckeds[(int)TConfigChecked.ckAutoUseMagic])   // 6304
        {
            if (MirActorSeam.MySelfExists() &&                     // 6305
                (!MirActorSeam.MySelfDeath()) &&                   // 6306
                (!MirActorSeam.MySelfShopStall()))                 // 6307
            {
                if ((PlugCtl.PlugComboBoxAutoMagic.ItemIndex >= 0) &&
                    (PlugCtl.PlugComboBoxAutoMagic.ItemIndex < PlugCtl.PlugComboBoxAutoMagic.Items.Count))   // 6308
                {
                    if (MirConfigGlobalSeam.MyGetTickCount() - g_Config.dwAutoUseMagicTick
                        > (uint)g_Config.nAutoUseMagicTime * 1000)     // 6309
                    {
                        g_Config.dwAutoUseMagicTick = MirConfigGlobalSeam.MyGetTickCount();   // 6310

                        object ClientMagic = PlugCtl.PlugComboBoxAutoMagic.Items
                            .GetObject(PlugCtl.PlugComboBoxAutoMagic.ItemIndex);              // 6312
                        MirActorSeam.AutoTakeOnItem(ClientMagic);                             // 6313
                        MirActorSeam.UseMagic(MirActorSeam.g_nMouseX, MirActorSeam.g_nMouseY, ClientMagic);   // 6314
                    }
                }
            }
        }
    }

    /// <summary>
    /// 原文 6321-6327：
    /// <c>Result := False; if FConfigCheckeds[ckFilterExp] then Result := Exp &lt; Cardinal(g_Config.nFilterMinExp);</c>
    /// ★ 类型语义照抄：<c>Exp:LongWord</c> 与 <c>Cardinal(nFilterMinExp)</c> 都是**无符号**比较
    ///   （原文显式写了 <c>Cardinal(...)</c> 转换）—— 若 <c>nFilterMinExp</c> 为负，
    ///   转换后变成极大的无符号数，过滤几乎不生效。托管侧用 <c>uint</c> 复刻同一语义。
    /// </summary>
    public override bool CanFilterExp(uint Exp)
    {
        bool Result = false;                                   // 6323
        if (FConfigCheckeds[(int)TConfigChecked.ckFilterExp])   // 6324
            Result = Exp < unchecked((uint)g_Config.nFilterMinExp);   // 6325
        return Result;
    }

    // ================================================================================
    // 演员接缝：原文的 <c>Actor = g_MySelf</c> 是**引用判等**。
    // 托管侧 g_MySelf 未移植，故接缝提供一个可注入的"当前本人/英雄对象"，
    // 判等语义与原文一致（ReferenceEquals）。
    // ================================================================================
    /// <summary>接缝：原文 <c>g_MySelf</c> 的对象身份（用于 <c>Actor = g_MySelf</c> 判等）。</summary>
    protected static object MySelfActorIdentity() => MirActorSeam.MySelfObject();
    /// <summary>接缝：原文 <c>g_MyHero</c> 的对象身份。</summary>
    protected static object MyHeroActorIdentity() => MirActorSeam.MyHeroObject();

    /// <summary>原文 4535-5044 的 <c>Initialize</c> 里"控件装配"部分的托管等价入口（见报告 §未完成项）。</summary>
    public override void Initialize(IntPtr Handle, byte ScreenMode, TClientVersion ClientVersion, bool WindowMode)
        => NotPorted(nameof(Initialize), 4535);
}
