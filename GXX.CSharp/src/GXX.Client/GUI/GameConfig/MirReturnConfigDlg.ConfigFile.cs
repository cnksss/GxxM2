// 源单元：Source/Client-HGE/GameConfig/MirReturn/MirReturnConfigDlg.pas（GBK，5,954 行，CRLF）
// 本分片覆盖（原文行号）：
//   4486-4786  LoadConfigFile（Config\%s.%s.set，TIniFile，节 Setup/Protect/Hotkey/GJ）
//   4788-5069  SaveConfigFile（同结构的写入侧；含 5060-5066 的异常吞掉 + DebugOutStr）
//
// 结构照抄要点：
//   1) 入口先 <c>sDirectory := ExtractFilePath(ParamStr(0)) + 'Config\'</c> 并 ForceDirectories；
//   2) 用户名非法字符替换（9 个）——**LoadConfigFile/SaveConfigFile/SaveOrLoadBossList/
//      SaveOrLoadGJMonList/SaveOrLoadGJMagicList1/2 共 6 处逐字重复**（原文如此，逐处保留）；
//   3) <c>for I := 0 to 4</c> 的主循环里用 **字符串表 sIdent1..sIdent15** 把
//      5 档用药模式（0=主体 1=英雄 2=战副 3=法副 4=道副）映射到不同的 INI 键前缀；
//   4) <c>for I := 0 to 8</c> × <c>for II := 0 to 4</c> 的超药矩阵，键名是
//      <c>Format('%sHp', [药名])</c>（**用中文药名当 INI 键**）；
//   5) <c>for I := 0 to Length(g_ShortcutKeys) - 1</c> 的快捷键表（16 项，Use/Key/Shift），
//      Shift 用 <c>Move(..., SizeOf(TShiftState))</c> 按位块拷贝 → 托管侧按 int 直接承载。
//
// ★ 原文缺陷（本车道登记，实现里以"显式跳过"复刻安全行为并在测试中固定）：
//   Load 4528-4531 / Save 4829-4834 的循环上界是 <c>Length(FConfigCheckeds) - 1 = 135</c>，
//   而 <c>TConfigChecked</c> 只到 <c>ckObjectHintEffect = 133</c>。
//   Delphi 非范围检查下 <c>TConfigChecked(134)</c>/<c>TConfigChecked(135)</c> 是**非法枚举值**，
//   会读/写数组第 134/135 格（本车道正是为 ckMovePick 加的合成槽位；
//   第 135 格是纯冗余槽位）。原文在范围检查打开（{$R+}）的构建下会抛 ERangeError。
//   托管侧 <c>(TConfigChecked)134</c> 合法但语义未定义，故按"跳过"处理并登记。

using System;
using System.Globalization;
using GXX.Client.GUI.GameConfig.Seams;

namespace GXX.Client.GUI.GameConfig;

public partial class TMirReturnConfigDlg
{
    // ================================================================================
    // 共用：5 档用药模式的 INI 键前缀表（原文 4545-4656 / 4840-4951 的 case 逐字照抄）
    // ================================================================================

    /// <summary>原文 4545-4655 / 4840-4950 的 <c>case I of</c> 字符串表（下标 0..4）。</summary>
    private static readonly string[,] MedicaIdentTable =
    {
        // 0 = 主体
        { "BoUseSuperMedica",
          "RenewHPIsAuto", "RenewHPTime", "RenewHPPercent",
          "RenewSpecialHPIsAuto", "RenewSpecialHPTime", "RenewSpecialHPPercent",
          "RenewMPIsAuto", "RenewMPTime", "RenewMPPercent",
          "RenewSpecialMPIsAuto", "RenewSpecialMPTime", "RenewSpecialMPPercent",
          "RenewDuraMin", "RenewDuraItemName", "RenewDuraTime" },
        // 1 = 英雄
        { "hBoUseSuperMedica",
          "RenewHeroNormalHpIsAuto", "RenewHeroNormalHpTime", "RenewHeroNormalHpPercent",
          "RenewSpecialHeroNormalHpIsAuto", "RenewSpecialHeroNormalHpTime", "RenewSpecialHeroNormalHpPercent",
          "RenewHeroNormalMpIsAuto", "RenewHeroNormalMpTime", "RenewHeroNormalMpPercent",
          "RenewSpecialHeroNormalMpIsAuto", "RenewSpecialHeroNormalMpTime", "RenewSpecialHeroNormalMpPercent",
          "RenewHeroDuraMin", "RenewHeroDuraItemName", "RenewHeroDuraTime" },
        // 2 = 战士副将（z 前缀）
        { "zBoUseSuperMedica",
          "RenewzHeroNormalHpIsAuto", "RenewzHeroNormalHpTime", "RenewzHeroNormalHpPercent",
          "RenewSpecialzHeroNormalHpIsAuto", "RenewSpecialzHeroNormalHpTime", "RenewSpecialzHeroNormalHpPercent",
          "RenewzHeroNormalMpIsAuto", "RenewzHeroNormalMpTime", "RenewzHeroNormalMpPercent",
          "RenewSpecialzHeroNormalMpIsAuto", "RenewSpecialzHeroNormalMpTime", "RenewSpecialzHeroNormalMpPercent",
          "RenewzHeroDuraMin", "RenewzHeroDuraItemName", "RenewzHeroDuraTime" },
        // 3 = 法师副将（f 前缀）
        { "fBoUseSuperMedica",
          "RenewfHeroNormalHpIsAuto", "RenewfHeroNormalHpTime", "RenewfHeroNormalHpPercent",
          "RenewSpecialfHeroNormalHpIsAuto", "RenewSpecialfHeroNormalHpTime", "RenewSpecialfHeroNormalHpPercent",
          "RenewfHeroNormalMpIsAuto", "RenewfHeroNormalMpTime", "RenewfHeroNormalMpPercent",
          "RenewSpecialfHeroNormalMpIsAuto", "RenewSpecialfHeroNormalMpTime", "RenewSpecialfHeroNormalMpPercent",
          "RenewfHeroDuraMin", "RenewfHeroDuraItemName", "RenewfHeroDuraTime" },
        // 4 = 道士副将（d 前缀）。★ 原文 4654 这一行**没有分号**（是 case 最后一支，语法合法）—— 原文如此
        { "dBoUseSuperMedica",
          "RenewdHeroNormalHpIsAuto", "RenewdHeroNormalHpTime", "RenewdHeroNormalHpPercent",
          "RenewSpecialdHeroNormalHpIsAuto", "RenewSpecialdHeroNormalHpTime", "RenewSpecialdHeroNormalHpPercent",
          "RenewdHeroNormalMpIsAuto", "RenewdHeroNormalMpTime", "RenewdHeroNormalMpPercent",
          "RenewSpecialdHeroNormalMpIsAuto", "RenewSpecialdHeroNormalMpTime", "RenewSpecialdHeroNormalMpPercent",
          "RenewdHeroDuraMin", "RenewdHeroDuraItemName", "RenewdHeroDuraTime" },
    };

    /// <summary>
    /// 原文 4697-4738 / 4992-5033 的 <c>case II of</c> 模板表（下标 0..4，5 列）。
    /// ★ 键名模板用 <c>%s</c> 占位**药名**（中文药名进 INI 键 —— 原文如此）。
    /// </summary>
    private static readonly string[,] SuperMedicaIdentTemplate =
    {
        { "%sBoUse", "%sHp", "%sHpTime", "%sMp", "%sMpTime" },                    // 0
        { "%shBoUse", "%sHeroHp", "%sHeroHpTime", "%sHeroMp", "%sHeroMpTime" },   // 1
        { "%szBoUse", "%szHeroHp", "%szHeroHpTime", "%szHeroMp", "%szHeroMpTime" }, // 2
        { "%sfBoUse", "%sfHeroHp", "%sfHeroHpTime", "%sfHeroMp", "%sfHeroMpTime" }, // 3
        { "%sdBoUse", "%sdHeroHp", "%sdHeroHpTime", "%sdHeroMp", "%sdHeroMpTime" }, // 4
    };

    // ================================================================================
    // MirReturnConfigDlg.pas:4486-4786  LoadConfigFile
    // ================================================================================

    /// <summary>原文 4486-4786。见文件头说明。</summary>
    public void LoadConfigFile()
    {
        int nShift = 0;                                                    // 4489（原文未初始化即用，见 4750）

        // 4494-4495：目录准备
        string sDirectory = PrepareConfigDirectory();
        _ = sDirectory;

        // 4497-4517：用户名非法字符替换（原文逐字重复的 6 处之一）
        RefreshPlugUserNameFromMySelf();

        // 4519
        string sFileName = ConfigFileName(MirReturnGlobalSeam.CONFIGFILE);

        IMirReturnIniFile ini = MirReturnGlobalSeam.CreateIniFile(sFileName);   // 4521
        if (ini != null)                                                   // 4522
        {
            // 4526
            MirReturnGlobalSeam.g_SoundVolume =
                ini.ReadInteger("Setup", "Volume", MirReturnGlobalSeam.g_SoundVolume);

            // 4528-4531 ★ 上界 135 超出枚举（见文件头缺陷登记）：越界项跳过
            for (int I = 0; I <= FConfigCheckeds.Length - 1; I++)
            {
                if (!IsRealConfigCheckedIndex(I)) continue;                // 原文缺陷的安全化（登记）
                int idx = I;
                FConfigCheckeds[idx] = ini.ReadBool(
                    "Setup", "Checked" + MirReturnGlobalSeam.IntToStr(I), FConfigCheckeds[idx]);
            }

            // 4533-4534
            g_Config.nColorShowEff = (byte)ini.ReadInteger("Setup", "ColorShowEff", g_Config.nColorShowEff);
            MirReturnGlobalSeam.SetFrmMainColorShowEff(g_Config.nColorShowEff);

            // 4536-4537
            g_Config.nSpecialColor = (byte)ini.ReadInteger("Setup", "SpecialColor", g_Config.nSpecialColor);
            MirReturnGlobalSeam.SetFrmMainSpecialColor(g_Config.nSpecialColor);

            // 4539-4541
            g_Config.MedicaMode = ini.ReadInteger("Protect", "MedicaMode", g_Config.MedicaMode);
            g_Config.MedicaMode = MirReturnGlobalSeam.MaxI(g_Config.MedicaMode, (long)(0));
            g_Config.MedicaMode = MirReturnGlobalSeam.Min(g_Config.MedicaMode, 4);

            // 4543-4691
            for (int I = 0; I <= 4; I++)
            {
                string sIdent = MedicaIdentTable[I, 0];
                string sIdent1 = MedicaIdentTable[I, 1];
                string sIdent2 = MedicaIdentTable[I, 2];
                string sIdent3 = MedicaIdentTable[I, 3];
                string sIdent4 = MedicaIdentTable[I, 4];
                string sIdent5 = MedicaIdentTable[I, 5];
                string sIdent6 = MedicaIdentTable[I, 6];
                string sIdent7 = MedicaIdentTable[I, 7];
                string sIdent8 = MedicaIdentTable[I, 8];
                string sIdent9 = MedicaIdentTable[I, 9];
                string sIdent10 = MedicaIdentTable[I, 10];
                string sIdent11 = MedicaIdentTable[I, 11];
                string sIdent12 = MedicaIdentTable[I, 12];
                string sIdent13 = MedicaIdentTable[I, 13];
                string sIdent14 = MedicaIdentTable[I, 14];
                string sIdent15 = MedicaIdentTable[I, 15];

                g_Config.ChkAutoPercents[I] = ini.ReadBool("Protect", "AutoPercents" + MirReturnGlobalSeam.IntToStr(I + 1) + "Chk", g_Config.ChkAutoPercents[I]);
                g_Config.ChkRenewAutoPercents[I] = ini.ReadBool("Protect", "RenewAutoPercents" + MirReturnGlobalSeam.IntToStr(I + 1) + "Chk", g_Config.ChkRenewAutoPercents[I]);
                g_Config.ChkSuperMedicaPercents[I] = ini.ReadBool("Protect", "SuperMedicaPercents" + MirReturnGlobalSeam.IntToStr(I + 1) + "Chk", g_Config.ChkSuperMedicaPercents[I]);

                g_Config.CheckHpIsAutos[I] = ini.ReadBool("Protect", "Hp" + MirReturnGlobalSeam.IntToStr(I + 1) + "Chk", g_Config.CheckHpIsAutos[I]);
                g_Config.CheckHpPercents[I] = ini.ReadInteger("Protect", "Hp" + MirReturnGlobalSeam.IntToStr(I + 1) + "Hp", g_Config.CheckHpPercents[I]);
                g_Config.CheckHpValues[I] = ini.ReadInteger("Protect", "Hp" + MirReturnGlobalSeam.IntToStr(I + 1) + "Man", g_Config.CheckHpValues[I]);
                g_Config.CheckMpIsAutos[I] = ini.ReadBool("Protect", "Mp" + MirReturnGlobalSeam.IntToStr(I + 1) + "Chk", g_Config.CheckMpIsAutos[I]);
                g_Config.CheckMpPercents[I] = ini.ReadInteger("Protect", "Mp" + MirReturnGlobalSeam.IntToStr(I + 1) + "Hp", g_Config.CheckMpPercents[I]);
                g_Config.CheckMpValues[I] = ini.ReadInteger("Protect", "Mp" + MirReturnGlobalSeam.IntToStr(I + 1) + "Man", g_Config.CheckMpValues[I]);

                g_Config.RenewHPIsAutos[I] = ini.ReadBool("Protect", sIdent1, g_Config.RenewHPIsAutos[I]);
                g_Config.RenewHPTimes[I] = MirReturnGlobalSeam.MaxI(MirReturnConfigGlobalSeam.ClientConfigPluginMinEatItemTime, (long)(ini.ReadInteger("Protect", sIdent2, g_Config.RenewHPTimes[I])));
                g_Config.RenewHPPercents[I] = ini.ReadInteger("Protect", sIdent3, g_Config.RenewHPPercents[I]);

                g_Config.RenewSpecialHPIsAutos[I] = ini.ReadBool("Protect", sIdent4, g_Config.RenewSpecialHPIsAutos[I]);
                g_Config.RenewSpecialHPTimes[I] = MirReturnGlobalSeam.MaxI(MirReturnConfigGlobalSeam.ClientConfigPluginMinEatItemTime, (long)(ini.ReadInteger("Protect", sIdent5, g_Config.RenewSpecialHPTimes[I])));
                g_Config.RenewSpecialHPPercents[I] = ini.ReadInteger("Protect", sIdent6, g_Config.RenewSpecialHPPercents[I]);

                g_Config.RenewMPIsAutos[I] = ini.ReadBool("Protect", sIdent7, g_Config.RenewMPIsAutos[I]);
                g_Config.RenewMPTimes[I] = MirReturnGlobalSeam.MaxI(MirReturnConfigGlobalSeam.ClientConfigPluginMinEatItemTime, (long)(ini.ReadInteger("Protect", sIdent8, g_Config.RenewMPTimes[I])));
                g_Config.RenewMPPercents[I] = ini.ReadInteger("Protect", sIdent9, g_Config.RenewMPPercents[I]);

                g_Config.RenewSpecialMPIsAutos[I] = ini.ReadBool("Protect", sIdent10, g_Config.RenewSpecialMPIsAutos[I]);
                g_Config.RenewSpecialMPTimes[I] = MirReturnGlobalSeam.MaxI(MirReturnConfigGlobalSeam.ClientConfigPluginMinEatItemTime, (long)(ini.ReadInteger("Protect", sIdent11, g_Config.RenewSpecialMPTimes[I])));
                g_Config.RenewSpecialMPPercents[I] = ini.ReadInteger("Protect", sIdent12, g_Config.RenewSpecialMPPercents[I]);

                g_Config.CheckDuraIsAutos[I] = ini.ReadBool("Protect", "Dura" + MirReturnGlobalSeam.IntToStr(I + 1) + "Chk", g_Config.CheckDuraIsAutos[I]);
                // 4686-4688 ★ 原文用的是大写 Ini（与 ini 同一实例）；下界夹紧照抄（Min→1、Time→2）
                g_Config.CheckDuraMin[I] = MirReturnGlobalSeam.MaxI(1, (long)ini.ReadInteger("Protect", sIdent13, g_Config.CheckDuraMin[I]));
                g_Config.CheckDuraValue[I] = ini.ReadString("Protect", sIdent14, g_Config.CheckDuraValue[I]);
                g_Config.CheckDuraTime[I] = MirReturnGlobalSeam.MaxI(2, (long)ini.ReadInteger("Protect", sIdent15, g_Config.CheckDuraTime[I]));

                g_Config.UseSuperMedicas[I] = ini.ReadBool("Protect", sIdent, g_Config.UseSuperMedicas[I]);
            }

            // 4693-4745：超药矩阵（外 药名 内 模式）
            for (int I = 0; I <= 8; I++)
            {
                for (int II = 0; II <= 4; II++)
                {
                    string sIdent1 = SuperMedicaIdentTemplate[II, 0];
                    string sIdent2 = SuperMedicaIdentTemplate[II, 1];
                    string sIdent3 = SuperMedicaIdentTemplate[II, 2];
                    string sIdent4 = SuperMedicaIdentTemplate[II, 3];
                    string sIdent5 = SuperMedicaIdentTemplate[II, 4];

                    string itemName = g_Config.SuperMedicaItemNames[I];
                    g_Config.SuperMedicaUses[II, I] = ini.ReadBool("Protect", MirReturnGlobalSeam.Format1(sIdent1, itemName), g_Config.SuperMedicaUses[II, I]);
                    g_Config.SuperMedicaHPs[II, I] = ini.ReadInteger("Protect", MirReturnGlobalSeam.Format1(sIdent2, itemName), g_Config.SuperMedicaHPs[II, I]);
                    g_Config.SuperMedicaHPTimes[II, I] = MirReturnGlobalSeam.MaxI(MirReturnConfigGlobalSeam.ClientConfigPluginMinEatItemTime, (long)(ini.ReadInteger("Protect", MirReturnGlobalSeam.Format1(sIdent3, itemName), g_Config.SuperMedicaHPTimes[II, I])));
                    g_Config.SuperMedicaMPs[II, I] = ini.ReadInteger("Protect", MirReturnGlobalSeam.Format1(sIdent4, itemName), g_Config.SuperMedicaMPs[II, I]);
                    g_Config.SuperMedicaMPTimes[II, I] = MirReturnGlobalSeam.MaxI(MirReturnConfigGlobalSeam.ClientConfigPluginMinEatItemTime, (long)(ini.ReadInteger("Protect", MirReturnGlobalSeam.Format1(sIdent5, itemName), g_Config.SuperMedicaMPTimes[II, I])));
                }
            }

            // 4746-4752：快捷键表（16 项）
            for (int I = 0; I <= ConfigShareGlobal.g_ShortcutKeys.Length - 1; I++)
            {
                var sk = ConfigShareGlobal.g_ShortcutKeys[I];
                sk.Use = (byte)(ini.ReadBool("Hotkey", "Use" + MirReturnGlobalSeam.IntToStr(I), sk.Use != 0) ? 1 : 0);
                sk.Key = (ushort)ini.ReadInteger("Hotkey", "Key" + MirReturnGlobalSeam.IntToStr(I), sk.Key);
                // 原文 4750：nShift 的**默认值是上一轮的 nShift**（循环体内不被重置）—— 原文如此
                nShift = ini.ReadInteger("Hotkey", "Shift" + MirReturnGlobalSeam.IntToStr(I), nShift);
                // 原文 4751：Move(nShift, g_ShortcutKeys[I].Shift, SizeOf(TShiftState)) 的位块拷贝
                sk.Shift = (DelphiShiftState)nShift;
                ConfigShareGlobal.g_ShortcutKeys[I] = sk;
            }

            // 4754
            g_Config.nHeroDodgeHPPercent = ini.ReadInteger("Protect", "HeroDodgeHPPercent", g_Config.nHeroDodgeHPPercent);

            // 4756-4763
            g_Config.nGJPlayAttackOption = ini.ReadInteger("GJ", "GJPlayAttackOption", g_Config.nGJPlayAttackOption);
            g_Config.nGJNoRedPoisonOption = ini.ReadInteger("GJ", "GJNoRedPoisonOption", g_Config.nGJNoRedPoisonOption);
            g_Config.nGJNoBluePoisonOption = ini.ReadInteger("GJ", "GJNoBluePoisonOption", g_Config.nGJNoBluePoisonOption);
            g_Config.nGJNoDuFuOption = ini.ReadInteger("GJ", "GJNoDuFuOption", g_Config.nGJNoDuFuOption);
            g_Config.nGJBagFullOption = ini.ReadInteger("GJ", "GJBagFullOption", g_Config.nGJBagFullOption);

            g_Config.nGJNotRushMonRange = ini.ReadInteger("GJ", "GJNotRushMonRange", g_Config.nGJNotRushMonRange);
            g_Config.nGJGroupAttackCount = ini.ReadInteger("GJ", "GJGroupAttackCount", g_Config.nGJGroupAttackCount);

            // 4765-4771：按**原文顺序**同步 frmMain（顺序：PlayAttack / NotRushMon / GroupAttack /
            // NoRedPoison / NoBluePoison / NoDuFu / BagFull —— 注意与 4756-4763 的读取顺序**不同**）
            MirReturnGlobalSeam.SetFrmMainGJPlayAttackOption(g_Config.nGJPlayAttackOption);
            MirReturnGlobalSeam.SetFrmMainGJNotRushMonRange(g_Config.nGJNotRushMonRange);
            MirReturnGlobalSeam.SetFrmMainGJGroupAttackCount(g_Config.nGJGroupAttackCount);
            MirReturnGlobalSeam.SetFrmMainGJNoRedPoisonOption(g_Config.nGJNoRedPoisonOption);
            MirReturnGlobalSeam.SetFrmMainGJNoBluePoisonOption(g_Config.nGJNoBluePoisonOption);
            MirReturnGlobalSeam.SetFrmMainGJNoDuFuOption(g_Config.nGJNoDuFuOption);
            MirReturnGlobalSeam.SetFrmMainGJBagFullOption(g_Config.nGJBagFullOption);

            // 4773：ini.Free（托管侧无需释放；原文 ini <> nil 判定已在 4522）
        }

        // 4776-4779：**无条件**拉 4 张名单（Load 方向 = IsSave False）
        SaveOrLoadBossList(false);
        SaveOrLoadGJMonList(false);
        SaveOrLoadGJMagicList1(false);
        SaveOrLoadGJMagicList2(false);

        // 4781-4785：5 个挂机动作下拉的条目文本，全部来自 g_GJActionMode.Text
        Plug.PlugComboBoxNoRedPoisonValueItemsText = MirReturnGlobalSeam.g_GJActionModeText;
        Plug.PlugComboBoxNoBluePoisonValueItemsText = MirReturnGlobalSeam.g_GJActionModeText;
        Plug.PlugComboBoxNoDuFuValueItemsText = MirReturnGlobalSeam.g_GJActionModeText;
        Plug.PlugComboBoxBagFullValueItemsText = MirReturnGlobalSeam.g_GJActionModeText;
        Plug.PlugComboBoxPlayAttackValueItemsText = MirReturnGlobalSeam.g_GJActionModeText;
    }

    // ================================================================================
    // MirReturnConfigDlg.pas:4788-5069  SaveConfigFile
    // ================================================================================

    /// <summary>原文 4788-5069。见文件头说明。</summary>
    public void SaveConfigFile()
    {
        int nShift = 0;                                                    // 4791

        string sDirectory = PrepareConfigDirectory();                      // 4796-4797
        _ = sDirectory;

        RefreshPlugUserNameFromMySelf();                                   // 4799-4819

        string sFileName = ConfigFileName(MirReturnGlobalSeam.CONFIGFILE); // 4821

        IMirReturnIniFile ini = MirReturnGlobalSeam.CreateIniFile(sFileName);   // 4823
        if (ini != null)                                                   // 4824
        {
            try                                                            // 4826
            {
                ini.WriteInteger("Setup", "Volume", MirReturnGlobalSeam.g_SoundVolume);   // 4827

                // 4829-4834
                for (int I = 0; I <= FConfigCheckeds.Length - 1; I++)
                {
                    // 这4个选项会把及时雨的冲掉 chongchong 2015-04-21
                    if (IsRadarCheckedIndex(I)) continue;                   // 4832-4833
                    if (!IsRealConfigCheckedIndex(I)) continue;             // 原文缺陷的安全化（登记）
                    ini.WriteBool("Setup", "Checked" + MirReturnGlobalSeam.IntToStr(I), FConfigCheckeds[I]);
                }

                ini.WriteInteger("Setup", "ColorShowEff", g_Config.nColorShowEff);     // 4835
                ini.WriteInteger("Setup", "SpecialColor", g_Config.nSpecialColor);     // 4836

                // 4838-4986
                for (int I = 0; I <= 4; I++)
                {
                    string sIdent = MedicaIdentTable[I, 0];
                    string sIdent1 = MedicaIdentTable[I, 1];
                    string sIdent2 = MedicaIdentTable[I, 2];
                    string sIdent3 = MedicaIdentTable[I, 3];
                    string sIdent4 = MedicaIdentTable[I, 4];
                    string sIdent5 = MedicaIdentTable[I, 5];
                    string sIdent6 = MedicaIdentTable[I, 6];
                    string sIdent7 = MedicaIdentTable[I, 7];
                    string sIdent8 = MedicaIdentTable[I, 8];
                    string sIdent9 = MedicaIdentTable[I, 9];
                    string sIdent10 = MedicaIdentTable[I, 10];
                    string sIdent11 = MedicaIdentTable[I, 11];
                    string sIdent12 = MedicaIdentTable[I, 12];
                    string sIdent13 = MedicaIdentTable[I, 13];
                    string sIdent14 = MedicaIdentTable[I, 14];
                    string sIdent15 = MedicaIdentTable[I, 15];

                    ini.WriteBool("Protect", "AutoPercents" + MirReturnGlobalSeam.IntToStr(I + 1) + "Chk", g_Config.ChkAutoPercents[I]);
                    ini.WriteBool("Protect", "RenewAutoPercents" + MirReturnGlobalSeam.IntToStr(I + 1) + "Chk", g_Config.ChkRenewAutoPercents[I]);
                    ini.WriteBool("Protect", "SuperMedicaPercents" + MirReturnGlobalSeam.IntToStr(I + 1) + "Chk", g_Config.ChkSuperMedicaPercents[I]);

                    ini.WriteBool("Protect", "Hp" + MirReturnGlobalSeam.IntToStr(I + 1) + "Chk", g_Config.CheckHpIsAutos[I]);
                    ini.WriteInteger("Protect", "Hp" + MirReturnGlobalSeam.IntToStr(I + 1) + "Hp", g_Config.CheckHpPercents[I]);
                    ini.WriteInteger("Protect", "Hp" + MirReturnGlobalSeam.IntToStr(I + 1) + "Man", g_Config.CheckHpValues[I]);
                    ini.WriteBool("Protect", "Mp" + MirReturnGlobalSeam.IntToStr(I + 1) + "Chk", g_Config.CheckMpIsAutos[I]);
                    ini.WriteInteger("Protect", "Mp" + MirReturnGlobalSeam.IntToStr(I + 1) + "Hp", g_Config.CheckMpPercents[I]);
                    ini.WriteInteger("Protect", "Mp" + MirReturnGlobalSeam.IntToStr(I + 1) + "Man", g_Config.CheckMpValues[I]);

                    ini.WriteBool("Protect", sIdent1, g_Config.RenewHPIsAutos[I]);
                    ini.WriteInteger("Protect", sIdent2, g_Config.RenewHPTimes[I]);
                    ini.WriteInteger("Protect", sIdent3, g_Config.RenewHPPercents[I]);

                    ini.WriteBool("Protect", sIdent4, g_Config.RenewSpecialHPIsAutos[I]);
                    ini.WriteInteger("Protect", sIdent5, g_Config.RenewSpecialHPTimes[I]);
                    ini.WriteInteger("Protect", sIdent6, g_Config.RenewSpecialHPPercents[I]);

                    ini.WriteBool("Protect", sIdent7, g_Config.RenewMPIsAutos[I]);
                    ini.WriteInteger("Protect", sIdent8, g_Config.RenewMPTimes[I]);
                    ini.WriteInteger("Protect", sIdent9, g_Config.RenewMPPercents[I]);

                    ini.WriteBool("Protect", sIdent10, g_Config.RenewSpecialMPIsAutos[I]);
                    ini.WriteInteger("Protect", sIdent11, g_Config.RenewSpecialMPTimes[I]);
                    ini.WriteInteger("Protect", sIdent12, g_Config.RenewSpecialMPPercents[I]);

                    ini.WriteBool("Protect", "Dura" + MirReturnGlobalSeam.IntToStr(I + 1) + "Chk", g_Config.CheckDuraIsAutos[I]);
                    ini.WriteInteger("Protect", sIdent13, g_Config.CheckDuraMin[I]);
                    ini.WriteString("Protect", sIdent14, g_Config.CheckDuraValue[I]);
                    ini.WriteInteger("Protect", sIdent15, g_Config.CheckDuraTime[I]);

                    ini.WriteBool("Protect", sIdent, g_Config.UseSuperMedicas[I]);
                }

                // 4988-5040
                for (int I = 0; I <= 8; I++)
                {
                    for (int II = 0; II <= 4; II++)
                    {
                        string sIdent1 = SuperMedicaIdentTemplate[II, 0];
                        string sIdent2 = SuperMedicaIdentTemplate[II, 1];
                        string sIdent3 = SuperMedicaIdentTemplate[II, 2];
                        string sIdent4 = SuperMedicaIdentTemplate[II, 3];
                        string sIdent5 = SuperMedicaIdentTemplate[II, 4];

                        string itemName = g_Config.SuperMedicaItemNames[I];
                        ini.WriteBool("Protect", MirReturnGlobalSeam.Format1(sIdent1, itemName), g_Config.SuperMedicaUses[II, I]);
                        ini.WriteInteger("Protect", MirReturnGlobalSeam.Format1(sIdent2, itemName), g_Config.SuperMedicaHPs[II, I]);
                        ini.WriteInteger("Protect", MirReturnGlobalSeam.Format1(sIdent3, itemName), g_Config.SuperMedicaHPTimes[II, I]);
                        ini.WriteInteger("Protect", MirReturnGlobalSeam.Format1(sIdent4, itemName), g_Config.SuperMedicaMPs[II, I]);
                        ini.WriteInteger("Protect", MirReturnGlobalSeam.Format1(sIdent5, itemName), g_Config.SuperMedicaMPTimes[II, I]);
                    }
                }

                // 5042-5048：快捷键表
                for (int I = 0; I <= ConfigShareGlobal.g_ShortcutKeys.Length - 1; I++)
                {
                    var sk = ConfigShareGlobal.g_ShortcutKeys[I];
                    ini.WriteBool("Hotkey", "Use" + MirReturnGlobalSeam.IntToStr(I), sk.Use != 0);
                    ini.WriteInteger("Hotkey", "Key" + MirReturnGlobalSeam.IntToStr(I), sk.Key);
                    // 5046：Move(g_ShortcutKeys[I].Shift, nShift, SizeOf(TShiftState);
                    nShift = (int)sk.Shift;
                    ini.WriteInteger("Hotkey", "Shift" + MirReturnGlobalSeam.IntToStr(I), nShift);   // 5047
                }

                ini.WriteInteger("Protect", "HeroDodgeHPPercent", g_Config.nHeroDodgeHPPercent);    // 5050

                // 5052-5059
                ini.WriteInteger("GJ", "GJPlayAttackOption", g_Config.nGJPlayAttackOption);
                ini.WriteInteger("GJ", "GJNoRedPoisonOption", g_Config.nGJNoRedPoisonOption);
                ini.WriteInteger("GJ", "GJNoBluePoisonOption", g_Config.nGJNoBluePoisonOption);
                ini.WriteInteger("GJ", "GJNoDuFuOption", g_Config.nGJNoDuFuOption);
                ini.WriteInteger("GJ", "GJBagFullOption", g_Config.nGJBagFullOption);

                ini.WriteInteger("GJ", "GJNotRushMonRange", g_Config.nGJNotRushMonRange);
                ini.WriteInteger("GJ", "GJGroupAttackCount", g_Config.nGJGroupAttackCount);
            }
            catch (Exception E)                                            // 5060-5066
            {
                MirReturnGlobalSeam.DebugOutStr("[Exception] TMirReturnConfigDlg::SaveConfigFile");
                MirReturnGlobalSeam.DebugOutStr(E.Message);
            }
            // 原文 5067：ini.Free
        }
    }

    // ================================================================================
    // 共用辅助（原文在 6 个方法里**逐字重复**的同两段代码，此处收敛为两个私有方法；
    // 每个调用点都注明了原文行号，语义与重复版完全一致）
    // ================================================================================

    /// <summary>
    /// 原文 <c>sDirectory := ExtractFilePath(ParamStr(0)) + 'Config\';
    /// if not DirectoryExists(sDirectory) then ForceDirectories(sDirectory);</c>
    /// （4486/4495、4796/4797、5208/5209、5494/5495、5548/5549、5629/5630 共 6 处）。
    /// </summary>
    private static string PrepareConfigDirectory()
    {
        string sDirectory = MirReturnGlobalSeam.AppPath + @"Config\";
        if (!MirReturnGlobalSeam.DirectoryExists(sDirectory))
            MirReturnGlobalSeam.ForceDirectories(sDirectory);
        return sDirectory;
    }

    /// <summary>
    /// 原文 6 处逐字重复的"用户名非法字符替换"：
    /// <c>if (g_MySelf &lt;&gt; nil) and (g_MySelf.m_sUserName &lt;&gt; '') then
    /// begin g_sPlugUserName := g_MySelf.m_sUserName; for I := 1 to Length(...) do
    /// if g_sPlugUserName[I] in ['/', '\', ':', '*', '?', '"', '&lt;', '&gt;', '|'] then case ... end;</c>
    /// （4497-4517、4799-4819、5211-5231、5497-5517、5551-5571、5632-5652）。
    /// </summary>
    private static void RefreshPlugUserNameFromMySelf()
    {
        if ((ConfigShareSeam.g_MySelf != null) && (ConfigShareSeam.g_MySelf.m_sUserName != ""))
        {
            ConfigShareGlobal.g_sPlugUserName = ConfigShareSeam.g_MySelf.m_sUserName;
            ConfigShareGlobal.g_sPlugUserName =
                ReplaceFileNameSpecialChar(ConfigShareGlobal.g_sPlugUserName);
        }
    }

    /// <summary>
    /// 原文 <c>sFileName := ExtractFilePath(ParamStr(0)) + Format(TEMPLATE, [g_sPlugServerName, g_sPlugUserName]);</c>
    /// （4519、4821、5233、5519、5573、5654）。
    /// </summary>
    private static string ConfigFileName(string template)
        => MirReturnGlobalSeam.AppPath +
           MirReturnGlobalSeam.Format2(template, ConfigShareGlobal.g_sPlugServerName, ConfigShareGlobal.g_sPlugUserName);

    /// <summary>
    /// 原文 4829-4834 的 <c>TConfigChecked(I)</c>：仅 0..133 是合法枚举成员。
    /// 135 格数组里 134 = ckMovePick 合成槽位、135 = 冗余槽位 —— 见文件头缺陷登记。
    /// </summary>
    private static bool IsRealConfigCheckedIndex(int I)
        => I >= 0 && I <= TConfigCheckedBounds.HighOrdinal;

    /// <summary>
    /// 原文 4832 的排除集合：<c>ckShowRadarPlayer/Actor/Npc/AttackNpc</c>（雷达四项，
    /// 注释"这4个选项会把及时雨的冲掉 chongchong 2015-04-21"）。
    /// ★ 仅 <c>SaveConfigFile</c> 排除，<c>LoadConfigFile</c> **读**时不排除 —— 原文不对称，照抄。
    /// </summary>
    private static bool IsRadarCheckedIndex(int I)
        => I == (int)TConfigChecked.ckShowRadarPlayer
        || I == (int)TConfigChecked.ckShowRadarActor
        || I == (int)TConfigChecked.ckShowRadarNpc
        || I == (int)TConfigChecked.ckShowRadarAttackNpc;
}
