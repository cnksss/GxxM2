// 源单元：Source/Client-HGE/GameConfig/MirReturn/MirReturnConfigDlg.pas（GBK，5,954 行，CRLF）
// 本分片覆盖（原文行号）：
//   973-993    RefreshMySelfMagicList（技能下拉重建 + 刷新挂机技能表）
//   1036-1080  LoadHelpFile（Data\explain2.dat → 帮助页，按"首尾是否有空格"着色）
//   1088-1158  RefShowItem 的控制流骨架（控件构建段见"未覆盖"清单）
//   2564-3320  LoadClientConfig / Initialize / Finalize / Logon / LoadConfig / Run / RefActorList
//   3275-3280  Finalize
//   3281-3285  Logon
//   3286-3320  LoadConfig（含用户名非法字符替换，原文 3290-3310）
//   3321-3324  Run（每 tick 只调 AutoUseItem）
//   3326-3329  RefActorList（空实现）
//   3331-3372  GetShowItem / FindShowItem / FindHintItem / FindPickItem / HintItem
//   3374-3403  Struck（伤害量 → DamageHPUseItem 分派，nObj 0=本人 1=英雄）
//   3405-3439  HealthChange（同上 + MP 分支）
//   4191-4207  NumberSort_1（★ 降序比较器；异常吞噬 → 返回 0）
//   5700-5705  DEditNotRushMonRangeChange
//   5706-5734  ComboBoxPlayAttackValueSelect（Sender 判等的 5 路分派）
//   5776-5841  RefreshGJMagic
//   5942-5954  OnChangedVolumePosition / OnChanggingVolumePosition

using System;
using GXX.Client.GUI.GameConfig.Seams;

namespace GXX.Client.GUI.GameConfig;

public partial class TMirReturnConfigDlg
{
    // ================================================================================
    // MirReturnConfigDlg.pas:973-993  RefreshMySelfMagicList
    // ================================================================================

    /// <summary>
    /// 原文 973-993：
    /// <c>if g_MySelf &lt;&gt; nil then</c> 记住 <c>PlugComboBoxAutoMagic.ItemIndex</c> → <c>Items.Clear</c> →
    /// 逐条 <c>AddObject(魔法名, TObject(魔法))</c> → 若旧下标仍有效则恢复、否则置 <c>-1</c>；
    /// 最后**无条件**调 <c>RefreshGJMagic</c>（2024/2025 的 if 之外）。
    /// </summary>
    public override void RefreshMySelfMagicList()
    {
        if (MirReturnConfigGlobalSeam.g_MySelfExists())                    // 977
        {
            int nItemIndex = Plug.PlugComboBoxAutoMagicItemIndex;          // 979
            Plug.PlugComboBoxAutoMagicItemsClear();                        // 980
            for (int I = 0; I <= MirReturnConfigGlobalSeam.g_MagicList.Count - 1; I++)
            {
                // pTClientMagic(g_MagicList.Items[I]).Def.sMagicName
                var magic = MirReturnConfigGlobalSeam.g_MagicList[I];
                Plug.PlugComboBoxAutoMagicItemsAddObject(
                    MirReturnConfigGlobalSeam.GetMagicName(magic), magic); // 983-984
            }
            if ((nItemIndex >= 0) && (nItemIndex < Plug.PlugComboBoxAutoMagicItemsCount))
                Plug.PlugComboBoxAutoMagicItemIndex = nItemIndex;          // 987
            else
                Plug.PlugComboBoxAutoMagicItemIndex = -1;                  // 989
        }

        RefreshGJMagic();                                                  // 992
    }

    // ================================================================================
    // MirReturnConfigDlg.pas:1036-1080  LoadHelpFile
    // ================================================================================

    /// <summary>
    /// 原文 1036-1080：
    /// <c>if (PlugMemoConfigHelp &lt;&gt; nil) and FileExists('Data\explain2.dat')</c> 时
    /// <c>LoadFromFile</c>（**异常被空 except 吞掉**）→ <c>FontBackTransparent := True</c> →
    /// 逐行：<c>if Length(Caption) &lt;&gt; Length(Trim(Caption))</c>（**首尾有空白 → 视为标题行**）
    /// 用 clSilver，否则 clWhite；三种状态（Up/Hot/Down）的 Color/BColor(clBlack)/Bold(False) 全部赋值。
    ///
    /// 原文缺陷（登记不改）：<c>TDxLines(PlugMemoConfigHelp.Lines).Items[I]</c> 是**硬转型**，
    /// 若 <c>Lines</c> 不是 TDxLines 会 AV；托管侧由接缝承载该类型假设。
    /// </summary>
    public void LoadHelpFile()
    {
        if ((Plug.PlugMemoConfigHelp != null) &&
            MirReturnGlobalSeam.FileExists(@"Data\explain2.dat"))    // 1042
        {
            try
            {
                Plug.PlugMemoConfigHelpLoadFromFile(@"Data\explain2.dat"); // 1045
            }
            catch
            {
                // 原文 1046-1047：空 except（原文如此）
            }

            Plug.PlugMemoConfigHelpFontBackTransparent = true;             // 1049

            for (int I = 0; I <= Plug.PlugMemoConfigHelpLinesCount - 1; I++)
            {
                string caption = Plug.PlugMemoConfigHelpLineCaption(I);    // 1053
                // 1054: Length(ViewItem.Caption) <> Length(Trim(ViewItem.Caption))
                bool isTitleLine = caption.Length != caption.Trim().Length;
                int color = isTitleLine ? HelpClSilver : HelpClWhite;
                Plug.SetHelpLineColor(I, "Up", color, HelpClBlack, false);   // 1056-1058 / 1068-1070
                Plug.SetHelpLineColor(I, "Hot", color, HelpClBlack, false);  // 1059-1061 / 1071-1073
                Plug.SetHelpLineColor(I, "Down", color, HelpClBlack, false); // 1062-1064 / 1074-1076
            }
        }
    }

    /// <summary>原文 Graphics clSilver = $00C0C0C0。</summary>
    private const int HelpClSilver = 0x00C0C0C0;
    /// <summary>原文 Graphics clWhite = $00FFFFFF。</summary>
    private const int HelpClWhite = 0x00FFFFFF;
    /// <summary>原文 Graphics clBlack = $00000000。</summary>
    private const int HelpClBlack = 0x00000000;

    // ================================================================================
    // MirReturnConfigDlg.pas:3321-3329  Run / RefActorList
    // ================================================================================

    /// <summary>原文 3321-3324：<c>AutoUseItem(Self);</c>（每 tick 唯一动作）。</summary>
    public override void Run()
    {
        AutoUseItem(null);                                                 // 3323
    }

    /// <summary>原文 3326-3329：**空实现**。</summary>
    public override void RefActorList() { }

    // ================================================================================
    // MirReturnConfigDlg.pas:3275-3320  Finalize / Logon / LoadConfig
    // ================================================================================

    /// <summary>原文 3275-3280：<c>if FEnabled then SaveConfigFile;</c></summary>
    public override void Finalize()
    {
        if (FEnabled)                                                      // 3277
            SaveConfigFile();                                              // 3277
    }

    /// <summary>
    /// 原文 3281-3285：<c>g_sPlugServerName := ServerName;</c>（原文如此：**只**记服务器名，
    /// 不做 LoadConfigFile —— 与 MirsConfigDlg 的 Logon 差异点）。
    /// </summary>
    public override void Logon(string ServerName)
    {
        ConfigShareGlobal.g_sPlugServerName = ServerName;                  // 3283
    }

    /// <summary>
    /// 原文 3286-3320：
    /// <c>FLoadControl := True;</c> → 若 <c>g_MySelf &lt;&gt; nil</c> 且用户名非空，
    /// 把 <c>g_sPlugUserName</c> 的 9 个非法文件名字符逐一替换
    /// （'/'-&gt;'{'、'\'-&gt;'}'、':'-&gt;';'、'*'-&gt;'@'、'?'-&gt;'!'、'"'-&gt;'~'、
    ///  '&lt;'-&gt;'('、'&gt;'-&gt;')'、'|'-&gt;'-'）→ <c>LoadConfigFile;</c>。
    ///
    /// 注意：**形参 <c>CharName</c> 原文完全未使用**（原文如此）；
    /// 用户名取的是 <c>g_MySelf.m_sUserName</c> 而不是形参。
    /// </summary>
    public override void LoadConfig(string CharName)
    {
        FLoadControl = true;                                               // 3288
        ConfigShareSeamResolve();                                          // （原文直接用全局 g_MySelf）
        if ((ConfigShareSeam.g_MySelf != null) &&
            (ConfigShareSeam.g_MySelf.m_sUserName != ""))                  // 3289
        {
            ConfigShareGlobal.g_sPlugUserName = ConfigShareSeam.g_MySelf.m_sUserName;  // 3290
            ConfigShareGlobal.g_sPlugUserName = ReplaceFileNameSpecialChar(ConfigShareGlobal.g_sPlugUserName);
        }

        LoadConfigFile();                                                  // 3313
    }

    /// <summary>
    /// 原文 3291-3310 的**原地字符替换循环**（9 个 case）。
    /// 与 <see cref="ConfigSeams.ProcessFileNameSpecialChar"/> 结果一致，
    /// 但原文这里是**逐字符 in 集合 + case** 写法，故按原文保留同一顺序与映射表。
    /// </summary>
    private static string ReplaceFileNameSpecialChar(string s)
    {
        var a = s.ToCharArray();
        for (int I = 0; I <= a.Length - 1; I++)
        {
            char c = a[I];
            if (c == '/' || c == '\\' || c == ':' || c == '*' || c == '?' ||
                c == '"' || c == '<' || c == '>' || c == '|')
            {
                a[I] = c switch
                {
                    '/' => '{',
                    '\\' => '}',
                    ':' => ';',
                    '*' => '@',
                    '?' => '!',
                    '"' => '~',
                    '<' => '(',
                    '>' => ')',
                    '|' => '-',
                    _ => c,
                };
            }
        }
        return new string(a);
    }

    /// <summary>原文 3288 之前的隐含依赖：<c>g_MySelf</c> 由 MShare 建立，此处无额外动作（保留调用点以便审计）。</summary>
    private static void ConfigShareSeamResolve() { }

    // ================================================================================
    // MirReturnConfigDlg.pas:3331-3372  ShowItem 查询族
    // ================================================================================

    /// <summary>原文 3331-3334：<c>Result := FileItemDB.Find(ItemName);</c></summary>
    public override TShowItem GetShowItem(string ItemName) => FileItemDB.Find(ItemName);

    /// <summary>原文 3336-3345：命中则返回 <c>boShowName</c>，否则 <c>False</c>（**不**短路成 nil 判定）。</summary>
    public override bool FindShowItem(string ItemName)
    {
        TShowItem ShowItem = FileItemDB.Find(ItemName);                    // 3340
        if (ShowItem != null)
            return ShowItem.boShowName != 0;                               // 3342
        return false;                                                      // 3344
    }

    /// <summary>原文 3347-3356：命中则返回 <c>boHintMsg</c>，否则 <c>False</c>。</summary>
    public override bool FindHintItem(string ItemName)
    {
        TShowItem ShowItem = FileItemDB.Find(ItemName);                    // 3351
        if (ShowItem != null)
            return ShowItem.boHintMsg != 0;                                // 3353
        return false;                                                      // 3355
    }

    /// <summary>原文 3358-3367：命中则返回 <c>boPickup</c>，否则 <c>False</c>。</summary>
    public override bool FindPickItem(string ItemName)
    {
        TShowItem ShowItem = FileItemDB.Find(ItemName);                    // 3362
        if (ShowItem != null)
            return ShowItem.boPickup != 0;                                 // 3364
        return false;                                                      // 3366
    }

    /// <summary>原文 3369-3372：<c>FileItemDB.Hint(ItemName, X, Y);</c></summary>
    public override void HintItem(string ItemName, int X, int Y)
        => FileItemDB.Hint(ItemName, X, Y);

    // ================================================================================
    // MirReturnConfigDlg.pas:3374-3439  Struck / HealthChange
    // ================================================================================

    /// <summary>
    /// 原文 3374-3403：
    /// <c>if FEnabled then</c> → <c>if g_MySelf &lt;&gt; nil then</c> →
    /// 本人分支 <c>nDamage := g_MySelf.m_Abil.HP - HP; if nDamage &gt; 0 then DamageHPUseItem(0, nDamage);</c>
    /// 英雄分支 <c>nDamage := g_MyHero.m_Abil.HP - HP; if nDamage &gt; 0 then DamageHPUseItem(1, nDamage);</c>
    /// 注意原文英雄分支的 <c>if g_MyHero &lt;&gt; nil</c> **嵌在** <c>g_MySelf &lt;&gt; nil</c> 之内 —— 照抄。
    /// 另注意 <c>g_MyHero</c> 分支的 <c>begin/end</c> 与本人分支多一层（语义相同）—— 原文如此。
    /// </summary>
    public override void Struck(object Actor, int HP, int MaxHP)
    {
        if (FEnabled)                                                      // 3379
        {
            if (GetActorHP != null && HasMySelf())                         // 3381
            {
                if (ReferenceEquals(Actor, MySelfActor()))                 // 3383
                {
                    int nDamage = GetActorHP(MySelfActor()) - HP;          // 3385
                    if (nDamage > 0)
                        DamageHPUseItem(0, nDamage);                       // 3387
                }

                if (HasMyHero())                                           // 3390
                {
                    if (ReferenceEquals(Actor, MyHeroActor()))             // 3392
                    {
                        int nDamage = GetActorHP(MyHeroActor()) - HP;      // 3394
                        if (nDamage > 0)
                        {
                            DamageHPUseItem(1, nDamage);                   // 3397
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 原文 3405-3439：与 <see cref="Struck"/> 同形，另加**两段 MP 分支**：
    /// 本人 <c>g_MySelf.m_Abil.MP - MP</c> → <c>DamageMPUseItem(0, nDamage)</c>；
    /// 英雄 <c>g_MyHero.m_Abil.MP - MP</c> → <c>DamageMPUseItem(1, nDamage)</c>。
    /// 顺序照抄：HP 在先、MP 在后（本人/英雄各自先 HP 后 MP）。
    /// </summary>
    public override void HealthChange(object Actor, int HP, int MP, int MaxHP)
    {
        if (FEnabled)                                                      // 3409
        {
            if (HasMySelf())                                               // 3411
            {
                if (ReferenceEquals(Actor, MySelfActor()))                 // 3413
                {
                    int nDamage = GetActorHP(MySelfActor()) - HP;          // 3415
                    if (nDamage > 0)
                        DamageHPUseItem(0, nDamage);                       // 3417

                    nDamage = GetActorMP(MySelfActor()) - MP;              // 3419
                    if (nDamage > 0)
                        DamageMPUseItem(0, nDamage);                       // 3421
                }

                if (HasMyHero())                                           // 3424
                {
                    if (ReferenceEquals(Actor, MyHeroActor()))             // 3426
                    {
                        int nDamage = GetActorHP(MyHeroActor()) - HP;      // 3428
                        if (nDamage > 0)
                            DamageHPUseItem(1, nDamage);                   // 3430

                        nDamage = GetActorMP(MyHeroActor()) - MP;          // 3432
                        if (nDamage > 0)
                            DamageMPUseItem(1, nDamage);                   // 3434
                    }
                }
            }
        }
    }

    // ================================================================================
    // MirReturnConfigDlg.pas:4191-4207  NumberSort_1（**单元级** function，非 TMirReturnConfigDlg 方法）
    // ================================================================================

    /// <summary>
    /// 原文 4191-4207 的**单元级** <c>function NumberSort_1(List:TStringList; Index1,Index2:Integer):Integer;</c>
    /// （TStringList.CustomSort 的比较器）。
    ///
    /// **语义照抄（重要）**：这是**降序**比较器 ——
    /// <c>Value1 &gt; Value2 → -1</c>、<c>Value1 &lt; Value2 → 1</c>、相等 → 0。
    /// 且 <c>Result := 0</c> 预置在 <c>try</c> **之外**、异常被空 <c>except</c> 吞掉
    /// → **任一侧不是合法整数时返回 0（视为相等）**，而不是抛异常。
    /// 托管侧以 <c>int.TryParse</c> 复刻同一"吞掉"行为。
    /// </summary>
    public static int NumberSort_1(System.Collections.Generic.IReadOnlyList<string> List, int Index1, int Index2)
    {
        int Result = 0;                                                    // 4195
        try
        {
            int Value1 = StrToInt(List[Index1]);                           // 4197
            int Value2 = StrToInt(List[Index2]);                           // 4198
            if (Value1 > Value2)
                Result = -1;                                               // 4200
            else if (Value1 < Value2)
                Result = 1;                                                // 4202
            else
                Result = 0;                                                // 4204
        }
        catch
        {
            // 原文 4205-4206：空 except（**吞掉异常并保留 Result=0**，原文如此）
        }
        return Result;
    }

    /// <summary>Delphi <c>StrToInt</c>：失败抛 <c>EConvertError</c>；此处保留该语义供 <see cref="NumberSort_1"/> 捕获。</summary>
    private static int StrToInt(string s)
    {
        if (s == null) throw new FormatException("StrToInt: nil");
        return int.Parse(s.Trim(), System.Globalization.CultureInfo.InvariantCulture);
    }

    // ================================================================================
    // MirReturnConfigDlg.pas:5700-5705  DEditNotRushMonRangeChange
    // ================================================================================

    /// <summary>原文 5700-5704：两段无条件写回（配置 + frmMain），**无夹紧**。</summary>
    public void DEditNotRushMonRangeChange()
    {
        g_Config.nGJNotRushMonRange = Plug.PlugEditNotRushMonRange;        // 5702
        MirReturnGlobalSeam.SetFrmMainGJNotRushMonRange(g_Config.nGJNotRushMonRange); // 5703
    }

    // ================================================================================
    // MirReturnConfigDlg.pas:5706-5734  ComboBoxPlayAttackValueSelect
    // ================================================================================

    /// <summary>
    /// 原文 5706-5734：<c>Sender</c> 与 5 个下拉**逐一判等**（顺序 if/else if），
    /// 各自把 <c>ItemIndex</c> 写入对应 <c>g_Config.nGJ*Option</c> 并同步 frmMain。
    /// 托管侧 Sender 用控件名（<c>MirReturnControlId</c>）表达 —— 与 <c>CheckBoxClickEx</c> 同一做法。
    /// </summary>
    public void ComboBoxPlayAttackValueSelect(MirReturnControlId Sender)
    {
        if (Sender == MirReturnControlId.PlugComboBoxPlayAttackValue)                    // 5708
        {
            g_Config.nGJPlayAttackOption = Plug.PlugComboBoxPlayAttackValue;             // 5710
            MirReturnGlobalSeam.SetFrmMainGJPlayAttackOption(g_Config.nGJPlayAttackOption); // 5711
        }
        else if (Sender == MirReturnControlId.PlugComboBoxNoRedPoisonValue)
        {
            g_Config.nGJNoRedPoisonOption = Plug.PlugComboBoxNoRedPoisonValue;
            MirReturnGlobalSeam.SetFrmMainGJNoRedPoisonOption(g_Config.nGJNoRedPoisonOption);
        }
        else if (Sender == MirReturnControlId.PlugComboBoxNoBluePoisonValue)
        {
            g_Config.nGJNoBluePoisonOption = Plug.PlugComboBoxNoBluePoisonValue;
            MirReturnGlobalSeam.SetFrmMainGJNoBluePoisonOption(g_Config.nGJNoBluePoisonOption);
        }
        else if (Sender == MirReturnControlId.PlugComboBoxNoDuFuValue)
        {
            g_Config.nGJNoDuFuOption = Plug.PlugComboBoxNoDuFuValue;
            MirReturnGlobalSeam.SetFrmMainGJNoDuFuOption(g_Config.nGJNoDuFuOption);
        }
        else if (Sender == MirReturnControlId.PlugComboBoxBagFullValue)
        {
            g_Config.nGJBagFullOption = Plug.PlugComboBoxBagFullValue;
            MirReturnGlobalSeam.SetFrmMainGJBagFullOption(g_Config.nGJBagFullOption);
        }
    }

    // ================================================================================
    // MirReturnConfigDlg.pas:5776-5841  RefreshGJMagic
    // ================================================================================

    /// <summary>
    /// 原文 5776-5841。见 <c>MirReturnConfigDlg.RefreshGJMagic.cs</c> 的独立分片实现
    /// （方法体较长且需 TDxListView 行模型，接缝已就位）。
    /// </summary>
    private void RefreshGJMagicCore() => RefreshGJMagicImpl();

    // ================================================================================
    // MirReturnConfigDlg.pas:5942-5954  音量
    // ================================================================================

    /// <summary>
    /// 原文 5942-5946：<c>g_SoundVolume := TrackBarVolume.Position;
    /// PlugCheckBoxVolume.Caption := '音量';</c>
    /// （★ 注意原文**没有**把音量写进任何 <c>g_Config</c> 字段 —— 音量是 BassSound 侧全局）。
    /// </summary>
    public void OnChangedVolumePosition()
    {
        MirReturnGlobalSeam.g_SoundVolume = Plug.TrackBarVolumePosition;    // 5944
        Plug.PlugCheckBoxVolumeCaption = "音量";                            // 5945
    }

    /// <summary>
    /// 原文 5948-5952：<c>g_SoundVolume := TrackBarVolume.Position;
    /// PlugCheckBoxVolume.Caption := IntToStr(g_SoundVolume);</c>
    /// 拖动中把标题换成当前音量数字（与落定后的 '音量' 形成**同一控件的两种文案**）。
    /// </summary>
    public void OnChanggingVolumePosition()
    {
        MirReturnGlobalSeam.g_SoundVolume = Plug.TrackBarVolumePosition;    // 5950
        Plug.PlugCheckBoxVolumeCaption = MirReturnGlobalSeam.g_SoundVolume.ToString(
            System.Globalization.CultureInfo.InvariantCulture);             // 5951
    }

    // ---------------------------------------------------------------------------------
    // Actor 接缝（原文直接读 g_MySelf / g_MyHero 的 m_Abil.HP/MP 与引用判等）
    // ---------------------------------------------------------------------------------

    /// <summary>接缝：<c>g_MySelf.m_Abil.HP</c> / <c>g_MyHero.m_Abil.HP</c>（取当前值）。</summary>
    public Func<object, int> GetActorHP = _ => 0;
    /// <summary>接缝：<c>g_MySelf.m_Abil.MP</c> / <c>g_MyHero.m_Abil.MP</c>（取当前值）。</summary>
    public Func<object, int> GetActorMP = _ => 0;

    /// <summary>接缝：原文 <c>g_MySelf &lt;&gt; nil</c>（测试可注入假演员）。</summary>
    public Func<object> MySelfResolver = () => null;
    /// <summary>接缝：原文 <c>g_MyHero &lt;&gt; nil</c>。</summary>
    public Func<object> MyHeroResolver = () => null;

    private bool HasMySelf() => MySelfResolver() != null;
    private bool HasMyHero() => MyHeroResolver() != null;
    private object MySelfActor() => MySelfResolver();
    private object MyHeroActor() => MyHeroResolver();
}

/// <summary>
/// 接缝：<c>ComboBoxPlayAttackValueSelect</c> 的 <c>Sender</c> 等值判定。
/// 原文用 <c>Sender = PlugComboBoxXxx</c>（控件引用判等），托管侧以控件名枚举表达 ——
/// 与 <c>CheckBoxClickEx</c> 的控件名表同一做法（便于 1:1 对照与测试断言）。
/// </summary>
public enum MirReturnControlId
{
    None = 0,
    PlugComboBoxPlayAttackValue,
    PlugComboBoxNoRedPoisonValue,
    PlugComboBoxNoBluePoisonValue,
    PlugComboBoxNoDuFuValue,
    PlugComboBoxBagFullValue,
}

/// <summary>
/// 接缝：MirReturnConfigDlg 在 Lifecycle 分片里额外依赖的全局量
/// （原文 <c>g_MySelf</c> 存在性、<c>g_MagicList</c> 元素取魔法名、音量后端）。
/// </summary>
public static class MirReturnConfigGlobalSeam
{
    /// <summary>接缝：MShare.pas <c>g_MySelf &lt;&gt; nil</c>。</summary>
    public static Func<bool> g_MySelfExists = () => ConfigShareSeam.g_MySelf != null;

    /// <summary>接缝：MShare.pas <c>g_MagicList</c>（<c>Items[I]</c> 是 <c>pTClientMagic</c>）。</summary>
    public static System.Collections.Generic.List<object> g_MagicList => MirReturnGlobalSeam.g_MagicList;

    /// <summary>接缝：<c>pTClientMagic(...).Def.sMagicName</c>（由宿主注入取名字器）。</summary>
    public static Func<object, string> GetMagicName = _ => "";

    /// <summary>接缝：<c>pTClientMagic(...).Def.wMagicId</c>（原文 5793/5801/5806/5822/5830/5835）。</summary>
    public static Func<object, uint> GetMagicId = _ => 0u;

    /// <summary>
    /// 接缝：原文 <c>g_ClientConfig.dwPluginMinEatItemTime</c>（内挂最小吃药间隔）。
    ///
    /// **精确签名要求（需集成方在只读文件里补一个字段）**：只读接缝
    /// <c>src/GXX.Client/GUI/GameConfig/Seams/ConfigSeams.cs:98</c> 的 <c>TClientConfig</c>
    /// （<c>public sealed class</c>）目前只有 3 个字段，**缺** 本字段。
    /// 原文依据：<c>Grobal2.pas:5089 dwPluginMinEatItemTime:LongWord</c>，默认值
    /// <c>Grobal2.pas:6436 dwPluginMinEatItemTime: 500</c>。
    /// 本车道按规程不修改只读文件，故在本接缝内以同名字段承载；
    /// 承载默认值 500（与原文一致）。见交付报告的"接缝清单"。
    /// </summary>
    public static uint ClientConfigPluginMinEatItemTime = 500;

    // ---------------------------------------------------------------------------------
    // 演员状态接缝（原 AutoEat* 族直接读 g_MySelf/g_MyHero 的 m_boDeath / m_Abil）
    // ---------------------------------------------------------------------------------

    /// <summary>接缝：原文 <c>g_MyHero &lt;&gt; nil</c>。</summary>
    public static Func<bool> g_MyHeroExists = () => ConfigShareSeam.g_MyHero != null;

    /// <summary>接缝：原文 <c>g_MySelf.m_boDeath</c>。</summary>
    public static Func<bool> MySelfIsDeath = () => false;
    /// <summary>接缝：原文 <c>g_MyHero.m_boDeath</c>。</summary>
    public static Func<bool> MyHeroIsDeath = () => false;
    /// <summary>接缝：原文 <c>g_MySelf.m_boShopStall</c>（摆摊中不自动练功）。</summary>
    public static Func<bool> MySelfIsShopStall = () => false;

    /// <summary>接缝：原文 <c>g_MySelf.m_Abil</c>（TAbility，已由 Grobal2 移植）。</summary>
    public static Func<GXX.Core.Protocol.TAbility> MySelfAbil = () => default;
    /// <summary>接缝：原文 <c>g_MyHero.m_Abil</c>。</summary>
    public static Func<GXX.Core.Protocol.TAbility> MyHeroAbil = () => default;

    /// <summary>接缝：ClMain.pas <c>frmMain.AutoEatItem(nIndex:Integer)</c>。</summary>
    public static Action<int> AutoEatItem = _ => { };
    /// <summary>接缝：ClMain.pas <c>frmMain.HeroEatItem(nIndex:Integer)</c>。</summary>
    public static Action<int> HeroEatItem = _ => { };

    /// <summary>接缝：MShare.pas <c>g_nMouseX</c>（原文 4472 <c>frmMain.UseMagic(g_nMouseX, …)</c>）。</summary>
    public static int g_nMouseX;
    /// <summary>接缝：MShare.pas <c>g_nMouseY</c>。</summary>
    public static int g_nMouseY;
    /// <summary>接缝：ClMain.pas <c>frmMain.UseMagic(nX, nY:Integer; Magic:PTClientMagic)</c>。</summary>
    public static Action<int, int, object> UseMagic = (_, __, ___) => { };

    /// <summary>测试/复位用。</summary>
    public static void ResetForTests()
    {
        g_MySelfExists = () => ConfigShareSeam.g_MySelf != null;
        GetMagicName = _ => "";
        GetMagicId = _ => 0u;
        g_MyHeroExists = () => ConfigShareSeam.g_MyHero != null;
        MySelfIsDeath = () => false;
        MyHeroIsDeath = () => false;
        MySelfIsShopStall = () => false;
        MySelfAbil = () => default;
        MyHeroAbil = () => default;
        AutoEatItem = _ => { };
        HeroEatItem = _ => { };
        g_nMouseX = 0;
        g_nMouseY = 0;
        UseMagic = (_, __, ___) => { };
        MirReturnGlobalSeam.ResetForTests();
    }
}
