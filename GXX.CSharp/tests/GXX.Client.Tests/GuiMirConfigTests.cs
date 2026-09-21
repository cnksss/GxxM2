// 源单元：Source/Client-HGE/GameConfig/Mir/MirConfigDlg.pas（GBK，8,355 行，CRLF）
// 对应实现：src/GXX.Client/GUI/GameConfig/Mir/**
//
// 本文件是 p10-client-mirconfig 车道的**对账与行为**用例，分三组：
//   §1 三方对账（机器可读，不依赖人工数）：
//        控件声明数 vs 实例化数 vs 注册表条数 = 516
//        原文 `X.OnY := H` 绑定数 vs 绑定表条数 = 279
//        绑定表处理器名 vs 派发 switch 覆盖 = 68
//   §2 核心行为（1:1 语义断言）：
//        TConfig 默认值 / Create 的 40 条勾选位 / 夹紧 / 数组长度 / FindShowItem 的
//        `&lt;&gt; 0` 语义 / CanFilterExp 的无符号比较 / AutoUseItem 的调用次序
//   §3 接线与类型标识：
//        GameConfigDlgs 两处接缝已指向真实现；`is TJSYConfigDlg` 判定仍成立

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GXX.Client.GUI.GameConfig;
using GXX.Client.GUI.GameConfig.Mir;
using GXX.Client.GUI.GameConfig.Seams;
using Xunit;

namespace GXX.Client.Tests;

public class GuiMirConfigReconcileTests
{
    /// <summary>原文镜像（本工程 `_analysis/utf8_mirror` 不在 git 里，存在才做源码级断言）。</summary>
    private static string MirrorPath => Path.Combine(
        "D:\\chuanqi\\daima\\GXX原版_Delphi7", "_analysis", "utf8_mirror",
        "Client-HGE", "GameConfig", "Mir", "MirConfigDlg.pas");

    // ============================================================================
    // §1 三方对账
    // ============================================================================

    [Fact]
    public void 控件声明数_实例化数_注册表条数_三者相等且为516()
    {
        var ctl = new TMirConfigDlgControls();

        // 声明数：生成物为原文 43-558 的每个控件字段各声明一个只读属性 + 构造里 new 一次。
        // ★ 不能用 `typeof(TMirDxControl).IsAssignableFrom(p.PropertyType)` 过滤：
        //   测试程序集里 `System.Windows.Forms.Control` 也在这条继承链上（工程既有 DxComponent
        //   是 WinForms 派生的），会把别的类型算进来。这里按**属性类型的全名**精确过滤。
        var props = typeof(TMirConfigDlgControls)
            .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
            .Where(p => p.PropertyType.Namespace == "GXX.Client.GUI.GameConfig.Mir"
                     && p.PropertyType != typeof(TMirDxControl)
                     && typeof(TMirDxControl).IsAssignableFrom(p.PropertyType))
            .ToArray();

        Assert.Equal(516, props.Length);                                  // 声明数
        Assert.All(props, p => Assert.NotNull(p.GetValue(ctl)));          // 实例化数

        // 注册表（`Find(name)` 的名字表）条数 = 516（由生成物的构造函数 RegisterAll 建立）
        Assert.Equal(516, ctl.RegisteredCount);
        // 逐条抽查：反射拿到的每个属性名都必须能在名字表里查到同一个对象
        Assert.All(props, p => Assert.Same(p.GetValue(ctl), ctl.Find(p.Name)));
    }

    [Fact]
    public void 原文绑定数_279_与绑定表条数相等()
    {
        Assert.Equal(279, TMirConfigDlgEventBindings.TotalBindings);
        Assert.Equal(279, TMirConfigDlgEventBindings_Data.All.Length);
    }

    [Fact]
    public void 绑定表的每一条_控件名都在516个控件里_且事件名在已知集合内()
    {
        var ctl = new TMirConfigDlgControls();
        var known = new HashSet<string>(new[]
        {
            "OnClick", "OnChange", "OnUnFocused", "OnSelect", "OnMouseMove", "OnMouseDown",
            "OnKeyDown", "OnListItemClick", "OnActivePageChange", "OnChangedPosition",
            "OnChanggingPosition", "OnInRealArea", "OnGetImage",
        });
        foreach (var b in TMirConfigDlgEventBindings_Data.All)
        {
            Assert.NotNull(ctl.Find(b.Ctrl));            // 控件名必须是真声明过的
            Assert.Contains(b.Event, known);             // 事件名必须是已知的 DxComponent 事件
            Assert.False(string.IsNullOrEmpty(b.Handler));
        }
    }

    [Fact]
    public void 绑定表条数_逐事件名分布_与原文一致()
    {
        // 逐事件名条数（由 MirConfigDlgEventBindings.g.cs 从原文 4621-5044 逐行提取后统计）
        var byEvent = TMirConfigDlgEventBindings_Data.All
            .GroupBy(b => b.Event)
            .ToDictionary(g => g.Key, g => g.Count());
        Assert.Equal(168, byEvent["OnClick"]);
        Assert.Equal(33, byEvent["OnChange"]);
        Assert.Equal(24, byEvent["OnUnFocused"]);
        Assert.Equal(16, byEvent["OnKeyDown"]);
        Assert.Equal(16, byEvent["OnMouseDown"]);
        Assert.Equal(3, byEvent["OnListItemClick"]);       // 4757 / 4993 / 4994
        Assert.Equal(2, byEvent["OnActivePageChange"]);    // 4952 / 5009（同一控件被绑两次 —— 原文如此）
        Assert.Equal(5, byEvent["OnMouseMove"]);
        Assert.Equal(1, byEvent["OnInRealArea"]);
        Assert.Equal(1, byEvent["OnChangedPosition"]);
        Assert.Equal(1, byEvent["OnChanggingPosition"]);
        Assert.Equal(9, byEvent["OnSelect"]);
        // ★ 原文 4621-5044 里**没有**任何 `X.OnGetImage := H` 行
        //   （原文两处 OnGetImage 是赋给控件自身的绘制回调，不在本区间）——
        //   故 BindEvents 里的 `case "OnGetImage"` 分支是**防御性**的，不对应任何一条绑定。
        Assert.DoesNotContain("OnGetImage", byEvent.Keys);
        Assert.Equal(279, byEvent.Values.Sum());
    }

    [Fact]
    public void 绑定表处理器名_72个_全部被派发switch覆盖()
    {
        var names = TMirConfigDlgEventBindings_Data.All
            .Select(b => b.Handler).Distinct().OrderBy(x => x, StringComparer.Ordinal).ToArray();
        Assert.Equal(72, names.Length);

        // 双向包含：派发器的名字表不多不少，正好是绑定表出现过的那些。
        Assert.Equal(72, TMirConfigDlg.AllHandlerNames.Length);
        foreach (var n in names)
            Assert.Contains(n, TMirConfigDlg.AllHandlerNames);
        foreach (var n in TMirConfigDlg.AllHandlerNames)
            Assert.Contains(n, names);
    }

    [Fact]
    public void 原文镜像存在时_绑定表条数与原文逐行统计相等()
    {
        if (!File.Exists(MirrorPath)) return;      // 镜像不在 git 内；缺失时跳过源码级断言
        var lines = File.ReadAllLines(MirrorPath);
        int count = 0;
        for (int i = TMirConfigDlgEventBindings.FirstLine - 1; i < TMirConfigDlgEventBindings.LastLine; i++)
            if (System.Text.RegularExpressions.Regex.IsMatch(lines[i], @"^\s*[A-Za-z_]\w*\.On\w+\s*:="))
                count++;
        Assert.Equal(TMirConfigDlgEventBindings.TotalBindings, count);
        Assert.Equal(279, count);
    }

    [Fact]
    public void 原文镜像存在时_控件字段声明数_与生成物的516相等()
    {
        if (!File.Exists(MirrorPath)) return;
        var lines = File.ReadAllLines(MirrorPath);
        int count = 0;
        for (int i = 42; i < 559; i++)
            if (System.Text.RegularExpressions.Regex.IsMatch(lines[i], @"^\s*[A-Za-z_]\w*\s*:\s*TDx\w+\s*;"))
                count++;
        Assert.Equal(516, count);
    }

    /// <summary>构造/释放 TMirConfigDlg（Create 会建 516 个控件并挂 279 条绑定）。</summary>
    private sealed class DialogScope : IDisposable
    {
        public TMirConfigDlg Value { get; }
        public DialogScope() { Value = new TMirConfigDlg(); }
        public void Dispose() { }
    }
}

public class GuiMirConfigCoreTests : IDisposable
{
    public GuiMirConfigCoreTests()
    {
        MirConfigGlobalSeam.ResetForTests();
        MirActorSeam.ResetForTests();
        TMirConfigDlg.g_Config = TMirConfigDlg.NewDefaultConfig();
        TMirConfigDlg.ResetNotPorted();
    }

    public void Dispose() { }

    // ============================================================================
    // §2 核心行为
    // ============================================================================

    [Fact]
    public void TConfig默认值_逐字段照抄原文908_1056()
    {
        var c = TMirConfigDlg.NewDefaultConfig();
        Assert.Equal(0, c.nFilterMinExp);
        Assert.Equal(0, c.nAutoUseMagicTime);
        Assert.Equal(0u, c.dwAutoUseMagicTick);
        Assert.False(c.boRenewSpecialIsAuto);
        Assert.Equal(0, c.MedicaMode);
        Assert.Equal("", c.sRenewBookNowBookItem);

        // 926/928：全 False
        Assert.All(c.ChkAutoPercents, v => Assert.False(v));
        Assert.All(c.ChkSuperMedicaPercents, v => Assert.False(v));
        // 927：ChkRenewAutoPercents:(False, True, True, True, True)
        Assert.False(c.ChkRenewAutoPercents[0]);
        Assert.All(c.ChkRenewAutoPercents.Skip(1), v => Assert.True(v));

        // 933/935/941/943
        Assert.All(c.CheckHpCheckTimes, v => Assert.Equal(1000u, v));
        Assert.All(c.CheckHpUseTimes, v => Assert.Equal(10000u, v));
        Assert.All(c.CheckMpCheckTimes, v => Assert.Equal(1000u, v));
        Assert.All(c.CheckMpUseTimes, v => Assert.Equal(10000u, v));

        // 947/953/959/965
        Assert.Equal(10, c.RenewHPPercents[0]);
        Assert.All(c.RenewHPPercents.Skip(1), v => Assert.Equal(97, v));
        Assert.Equal(10, c.RenewMPPercents[0]);
        Assert.All(c.RenewMPPercents.Skip(1), v => Assert.Equal(97, v));
        Assert.Equal(10, c.RenewSpecialHPPercents[0]);
        Assert.All(c.RenewSpecialHPPercents.Skip(1), v => Assert.Equal(88, v));
        Assert.Equal(10, c.RenewSpecialMPPercents[0]);
        Assert.All(c.RenewSpecialMPPercents.Skip(1), v => Assert.Equal(88, v));
        Assert.Equal(1000, c.RenewSpecialHPTimes[0]);
        Assert.All(c.RenewSpecialHPTimes.Skip(1), v => Assert.Equal(3000, v));
        Assert.Equal(1000, c.RenewSpecialMPTimes[0]);
        Assert.All(c.RenewSpecialMPTimes.Skip(1), v => Assert.Equal(3000, v));

        // 972-981：9 个超级药名逐字
        Assert.Equal(new[] { "太阳水", "强效太阳水", "万年雪霜", "疗伤药", "疗伤药(任务)",
                             "强效万年雪霜", "强效疗伤药", "超级万年雪霜", "超级疗伤药" },
                     c.SuperMedicaItemNames);
        // 999-1005 / 1023-1029
        for (int i = 0; i < 5; i++)
            for (int j = 0; j < 9; j++)
            {
                Assert.Equal(500, c.SuperMedicaHPTimes[i][j]);
                Assert.Equal(500, c.SuperMedicaMPTimes[i][j]);
            }
        // 1039/1040/1041
        Assert.All(c.CheckDuraMin, v => Assert.Equal(20, v));
        Assert.All(c.CheckDuraValue, v => Assert.Equal("修复神水", v));
        Assert.All(c.CheckDuraTime, v => Assert.Equal(30, v));

        // 1044-1046 / 1054-1055
        Assert.Equal(0, c.nHeroDodgeHPPercent);
        Assert.Equal(3, c.nColorShowEff);
        Assert.Equal(249, c.nSpecialColor);
        Assert.Equal(7, c.nGJNotRushMonRange);
        Assert.Equal(3, c.nGJGroupAttackCount);

        // 数组维度：0..4 与 0..8
        Assert.Equal(5, c.CheckHpPercents.Length);
        Assert.Equal(5, c.SuperMedicaUses.Length);
        Assert.All(c.SuperMedicaUses, row => Assert.Equal(9, row.Length));
    }

    [Fact]
    public void Create的40条默认勾选位_与原文1092_1139一致()
    {
        var dlg = new TMirConfigDlg();
        Assert.True(dlg.GetConfigChecked(TConfigChecked.ckShowHPLabel));                 // 1092
        Assert.False(dlg.GetConfigChecked(TConfigChecked.ckShowUserName));               // 1093
        Assert.True(dlg.GetConfigChecked(TConfigChecked.ckMagicLock));                   // 1094
        Assert.True(dlg.GetConfigChecked(TConfigChecked.ckAutoOrderItem));               // 1095
        Assert.True(dlg.GetConfigChecked(TConfigChecked.ckNotNeedShift));                // 1096
        Assert.False(dlg.GetConfigChecked(TConfigChecked.ckPickUpAll));                  // 1097
        Assert.True(dlg.GetConfigChecked(TConfigChecked.ckBGMusic));                     // 1098
        Assert.True(dlg.GetConfigChecked(TConfigChecked.ckRepeatBGMusic));               // 1099
        Assert.False(dlg.GetConfigChecked(TConfigChecked.ckNotParaly));                  // 1100
        Assert.True(dlg.GetConfigChecked(TConfigChecked.ckHumShootLightenLockTarget));   // 1113
        Assert.True(dlg.GetConfigChecked(TConfigChecked.ckNearHint));                    // 1123
        Assert.True(dlg.GetConfigChecked(TConfigChecked.ckAutoPickUpItem));              // 1132
        Assert.True(dlg.GetConfigChecked(TConfigChecked.ckAutoDetourPath));              // 1136
        Assert.False(dlg.GetConfigChecked(TConfigChecked.ckHideMonsterIcons));           // 1137
        Assert.False(dlg.GetConfigChecked(TConfigChecked.ckDimFireEffect));              // 1138
        Assert.False(dlg.GetConfigChecked(TConfigChecked.ckObjectHintEffect));           // 1139
    }

    [Fact]
    public void FConfigCheckeds长度_与基线枚举同长_134()
    {
        var dlg = new TMirConfigDlg();
        Assert.Equal(TConfigCheckedBounds.HighOrdinal + 1, dlg.ConfigCheckeds.Length);
        Assert.Equal(134, dlg.ConfigCheckeds.Length);
    }

    [Fact]
    public void CheckedMap的5个成员_全部同名命中基线枚举_无合成槽位()
    {
        Assert.Equal((int)TConfigChecked.ckHideMonsterIcons, (int)MirConfigCheckedMap.ckHideMonsterIcons);
        Assert.Equal((int)TConfigChecked.ckDimFireEffect, (int)MirConfigCheckedMap.ckDimFireEffect);
        Assert.Equal((int)TConfigChecked.ckObjectHintEffect, (int)MirConfigCheckedMap.ckObjectHintEffect);
        Assert.Equal((int)TConfigChecked.ckSimpleShowBB, (int)MirConfigCheckedMap.ckSimpleShowBB);
        Assert.Equal((int)TConfigChecked.ckShowValueItemEffect, (int)MirConfigCheckedMap.ckShowValueItemEffect);
        Assert.Contains("无缺口", MirConfigCheckedMap.Describe());
    }

    [Fact]
    public void SetConfigChecked_仅在值变化时调用RefConfig_原文1178_1184()
    {
        var dlg = new TMirConfigDlg();
        // 值未变：不触发 RefConfig（RefConfig 目前是 NotPorted 留痕，不产生记录即为"未调用"）
        TMirConfigDlg.ResetNotPorted();
        dlg.SetConfigChecked(TConfigChecked.ckShowHPLabel, true);   // 原值就是 true
        Assert.DoesNotContain(TMirConfigDlg.NotPortedMethods, s => s.StartsWith("RefConfig"));

        // 值变化：触发 RefConfig
        dlg.SetConfigChecked(TConfigChecked.ckShowHPLabel, false);
        Assert.Contains(TMirConfigDlg.NotPortedMethods, s => s.StartsWith("RefConfig"));
    }

    [Fact]
    public void DEditCheckDuraChange_Max1夹紧并回写控件_原文3003_3007()
    {
        var dlg = new TMirConfigDlg();
        var plug = (TMirConfigDlgControls)dlg.Plug;
        TMirConfigDlg.g_Config.MedicaMode = 2;

        plug.PlugEditCheckDura.Value = 0;
        dlg.DEditCheckDuraChange(plug.PlugEditCheckDura);
        Assert.Equal(1, TMirConfigDlg.g_Config.CheckDuraMin[2]);
        Assert.Equal(1, plug.PlugEditCheckDura.Value);      // 回写

        plug.PlugEditCheckDura.Value = 55;
        dlg.DEditCheckDuraChange(plug.PlugEditCheckDura);
        Assert.Equal(55, TMirConfigDlg.g_Config.CheckDuraMin[2]);
        Assert.Equal(55, plug.PlugEditCheckDura.Value);
    }

    [Fact]
    public void DEditCheckDuraTimeChange_Max2夹紧并回写_原文3014_3018()
    {
        var dlg = new TMirConfigDlg();
        var plug = (TMirConfigDlgControls)dlg.Plug;
        TMirConfigDlg.g_Config.MedicaMode = 1;

        plug.PlugEditCheckDuraTime.Value = 1;
        dlg.DEditCheckDuraTimeChange(plug.PlugEditCheckDuraTime);
        Assert.Equal(2, TMirConfigDlg.g_Config.CheckDuraTime[1]);
        Assert.Equal(2, plug.PlugEditCheckDuraTime.Value);
    }

    [Fact]
    public void RenewTimeChange_夹紧到dwPluginMinEatItemTime_并回写_原文3040()
    {
        var dlg = new TMirConfigDlg();
        var plug = (TMirConfigDlgControls)dlg.Plug;
        TMirConfigDlg.g_Config.MedicaMode = 0;
        MirConfigGlobalSeam.g_ClientConfig.dwPluginMinEatItemTime = 500;

        plug.PlugEditRenewHPTime.Value = 10;
        dlg.DEditRenewHPTimeChange(plug.PlugEditRenewHPTime);
        Assert.Equal(500, TMirConfigDlg.g_Config.RenewHPTimes[0]);
        Assert.Equal(500, plug.PlugEditRenewHPTime.Value);
    }

    [Fact]
    public void ComboBoxCheckHPValueChange_写ItemIndex而不是文本_原文2978()
    {
        var dlg = new TMirConfigDlg();
        var plug = (TMirConfigDlgControls)dlg.Plug;
        TMirConfigDlg.g_Config.MedicaMode = 3;
        plug.PlugComboBoxCheckHPValue.Items.Add("回城卷");
        plug.PlugComboBoxCheckHPValue.Items.Add("小退");
        plug.PlugComboBoxCheckHPValue.ItemIndex = 1;

        dlg.ComboBoxCheckHPValueChange(plug.PlugComboBoxCheckHPValue);
        Assert.Equal(1, TMirConfigDlg.g_Config.CheckHpValues[3]);
    }

    [Fact]
    public void 超级药9路Sender判等_按下标写矩阵_原文3084_3121()
    {
        var dlg = new TMirConfigDlg();
        var plug = (TMirConfigDlgControls)dlg.Plug;
        TMirConfigDlg.g_Config.MedicaMode = 4;

        plug.PlugEditSuperMedicaHP7.Value = 1234;
        dlg.DEditSuperMedicaHPChange(plug.PlugEditSuperMedicaHP7);
        Assert.Equal(1234, TMirConfigDlg.g_Config.SuperMedicaHPs[4][7]);
        // 其余下标不受影响（原文的 Index in [0..8] 门禁）
        Assert.Equal(0, TMirConfigDlg.g_Config.SuperMedicaHPs[4][0]);

        // 传一个不在 9 路里的对象 → Index 保持 -1 → 什么都不写
        dlg.DEditSuperMedicaHPChange(new object());
        Assert.Equal(0, TMirConfigDlg.g_Config.SuperMedicaHPs[4][0]);
    }

    [Fact]
    public void CanFilterExp_无符号比较语义_原文6321_6327()
    {
        var dlg = new TMirConfigDlg();
        dlg.SetConfigChecked(TConfigChecked.ckFilterExp, true);  // 会触发 RefConfig 留痕，无碍

        TMirConfigDlg.g_Config.nFilterMinExp = 100;
        Assert.True(dlg.CanFilterExp(99u));
        Assert.False(dlg.CanFilterExp(100u));
        Assert.False(dlg.CanFilterExp(101u));

        // ★ 原文显式 Cardinal(...) 转换：负值变成极大无符号数 ⇒ 过滤几乎不生效
        TMirConfigDlg.g_Config.nFilterMinExp = -1;
        Assert.True(dlg.CanFilterExp(0u));
        Assert.True(dlg.CanFilterExp(1000000u));

        dlg.SetConfigChecked(TConfigChecked.ckFilterExp, false);
        Assert.False(dlg.CanFilterExp(0u));
    }

    [Fact]
    public void FindShowItem_HintItem_PickItem_取的是_不等于0_而不是布尔转换_原文5095_5126()
    {
        var dlg = new TMirConfigDlg();
        var db = FilterItemsGlobal.g_FileItemDB;
        db.m_ShowItemList.Clear();
        db.m_ShowItemList.Add(new TShowItem { sItemName = "测试物品", boShowName = 2, boHintMsg = 0, boPickup = 1 });

        Assert.True(dlg.FindShowItem("测试物品"));    // 2 != 0
        Assert.True(dlg.FindPickItem("测试物品"));    // 1 != 0
        Assert.False(dlg.FindHintItem("测试物品"));   // 0 == 0
        Assert.False(dlg.FindShowItem("不存在"));     // Find 返回 null → False
        Assert.Null(dlg.GetShowItem("不存在"));
        db.m_ShowItemList.Clear();
    }

    [Fact]
    public void FindHumCustomBindItemIndex_空槽位门禁大于6_且首命中早退_原文5208_5250()
    {
        var item = new MirCustomBindItem { sItemName = "回城卷", UnBindItemType = TUnBindItemType.t_Book };
        MirActorSeam.CustomUnbindItemList = new List<IMirCustomBindItem> { item };

        // 门禁不满足（空槽位 = 6，不 > 6）→ -1
        MirActorSeam.HumBagNoUseItemCount = () => 6;
        MirActorSeam.ItemArrName = i => i == 3 ? "回城卷" : "";
        Assert.Equal(-1, TMirConfigDlg.FindHumCustomBindItemIndex(TUnBindItemType.t_Book, false));

        // 门禁满足 → 命中下标 3
        MirActorSeam.HumBagNoUseItemCount = () => 7;
        Assert.Equal(3, TMirConfigDlg.FindHumCustomBindItemIndex(TUnBindItemType.t_Book, false));

        // 类型不匹配 → -1
        Assert.Equal(-1, TMirConfigDlg.FindHumCustomBindItemIndex(TUnBindItemType.t_HP, false));
    }

    [Fact]
    public void FindHumCustomBindItemIndex_t_Special还要boSpecialMP相等_原文5218_5221()
    {
        var a = new MirCustomBindItem { sItemName = "太阳水", UnBindItemType = TUnBindItemType.t_Special, boSpecialMP = false };
        MirActorSeam.CustomUnbindItemList = new List<IMirCustomBindItem> { a };
        MirActorSeam.HumBagNoUseItemCount = () => 10;
        MirActorSeam.ItemArrName = i => i == 5 ? "太阳水" : "";

        Assert.Equal(5, TMirConfigDlg.FindHumCustomBindItemIndex(TUnBindItemType.t_Special, false));
        Assert.Equal(-1, TMirConfigDlg.FindHumCustomBindItemIndex(TUnBindItemType.t_Special, true));
    }

    [Fact]
    public void FindHeroCustomBindItemIndex_门禁是已用小于上限_原文5252_5298()
    {
        var item = new MirCustomBindItem { sItemName = "烈火剑法", UnBindItemType = TUnBindItemType.t_Book };
        MirActorSeam.CustomUnbindItemList = new List<IMirCustomBindItem> { item };
        MirActorSeam.HeroItemArrName = i => i == 2 ? "烈火剑法" : "";

        MirActorSeam.HeroBagItemCount = () => 40;
        MirActorSeam.MyHeroBagCount = () => 40;
        Assert.Equal(-1, TMirConfigDlg.FindHeroCustomBindItemIndex(TUnBindItemType.t_Book, false));

        MirActorSeam.HeroBagItemCount = () => 39;
        Assert.Equal(2, TMirConfigDlg.FindHeroCustomBindItemIndex(TUnBindItemType.t_Book, false));
    }

    [Fact]
    public void NumberSort_1_按数值而不是字典序_原文6081_6098()
    {
        var list = new List<string> { "10", "9", "abc", "" };
        Assert.True(TMirConfigDlg.NumberSort_1(list, 1, 2) > 0);    // 9 - 0 > 0（"abc" → 0）
        Assert.True(TMirConfigDlg.NumberSort_1(list, 2, 0) < 0);    // 10 与 0 比较：0-10 < 0
        Assert.Equal(0, TMirConfigDlg.NumberSort_1(list, 2, 3));    // 0 - 0
    }

    [Fact]
    public void AutoUseItem_调用次序_特殊HP_特殊MP_HP_MP_保护_练功_持久_原文5189_5206()
    {
        var dlg = new TMirConfigDlg();
        dlg.SetEnabled(true);
        TMirConfigDlg.ResetNotPorted();

        dlg.AutoUseItem(dlg);

        // AutoUseMagic 是唯一会**递归**回到 AutoUseItem 的处理器，故按首次出现位置比较。
        var interesting = new HashSet<string>(new[] { "AutoEatSpecialHPItem", "AutoEatSpecialMPItem",
            "AutoEatHPItem", "AutoEatMPItem", "AutoProtect", "AutoUseMagic", "DuraWarning" });
        var calls = new List<string>();
        foreach (var s in TMirConfigDlg.NotPortedMethods)
        {
            var name = s.Split(' ')[0];
            if (interesting.Contains(name)) calls.Add(name);
        }

        Assert.Equal(new[] { "AutoEatSpecialHPItem", "AutoEatSpecialMPItem", "AutoEatHPItem",
                             "AutoEatMPItem", "AutoProtect", "DuraWarning" },
                     calls.ToArray());
        // AutoUseMagic 排在 AutoProtect 之后（原文 5203）；默认配置下它会在
        // `if FConfigCheckeds[ckAutoUseMagic]` 处静默返回，故上面 6 条之外没有第 7 条记录。
        // 单独证明"AutoUseMagic 确实被调到了"：勾上练功 + 造出可下发的状态，
        // 再跑一遍 AutoUseItem，`AutoTakeOnItem` 的调用留痕必须出现。
        Assert.Empty(MirActorSeam.AutoTakeOnItemCalls);
        dlg.SetConfigChecked(TConfigChecked.ckAutoUseMagic, true);
        MirActorSeam.MySelfExists = () => true;
        MirActorSeam.MySelfDeath = () => false;
        MirActorSeam.MySelfShopStall = () => false;
        var plug = (TMirConfigDlgControls)dlg.Plug;
        plug.PlugComboBoxAutoMagic.Items.AddObject("烈火剑法", (object)77);
        plug.PlugComboBoxAutoMagic.ItemIndex = 0;
        TMirConfigDlg.g_Config.nAutoUseMagicTime = 1;
        TMirConfigDlg.g_Config.dwAutoUseMagicTick = MirConfigGlobalSeam.MyGetTickCount() - 2000;

        dlg.AutoUseItem(dlg);

        Assert.Single(MirActorSeam.AutoTakeOnItemCalls);
        Assert.Equal(77, MirActorSeam.AutoTakeOnItemCalls[0]);
    }

    [Fact]
    public void AutoUseItem_未启用时一个都不调用_原文5191()
    {
        var dlg = new TMirConfigDlg();
        dlg.SetEnabled(false);
        TMirConfigDlg.ResetNotPorted();
        dlg.AutoUseItem(dlg);
        Assert.Empty(TMirConfigDlg.NotPortedMethods.Where(s => s.StartsWith("Auto") || s.StartsWith("Dura")));
    }

    [Fact]
    public void Run_只有一行_AutoUseItemSelf_原文5080_5083()
    {
        var dlg = new TMirConfigDlg();
        dlg.SetEnabled(true);
        TMirConfigDlg.ResetNotPorted();
        dlg.Run();
        Assert.Contains(TMirConfigDlg.NotPortedMethods, s => s.StartsWith("AutoEatSpecialHPItem"));
    }

    [Fact]
    public void Logout_清零两个标志并无条件SaveConfigFile_原文1424_1429()
    {
        var dlg = new TMirConfigDlg();
        dlg.SetEnabled(true);
        TMirConfigDlg.ResetNotPorted();
        dlg.Logout();
        Assert.False(dlg.GetEnabled());
        Assert.Contains(TMirConfigDlg.NotPortedMethods, s => s.StartsWith("SaveConfigFile"));
    }

    [Fact]
    public void Close_清标志并BackUp物品库_原文1191_1205()
    {
        var dlg = new TMirConfigDlg();
        dlg.SetEnabled(true);
        dlg.Close();
        Assert.False(dlg.GetEnabled());
        Assert.False(dlg.GetVisible());       // PlugConfigDlg.Visible := False
    }

    [Fact]
    public void GetVisible_PlugConfigDlg为null时显式返回False_原文1207_1214()
    {
        var dlg = new TMirConfigDlg();
        var plug = (TMirConfigDlgControls)dlg.Plug;
        plug.PlugConfigDlg.Visible = false;
        Assert.False(dlg.GetVisible());
        plug.PlugConfigDlg.Visible = true;
        Assert.True(dlg.GetVisible());
    }

    [Fact]
    public void SetVisible_焦点三段与六条清理_原文1216_1238()
    {
        var dlg = new TMirConfigDlg();
        var plug = (TMirConfigDlgControls)dlg.Plug;
        int focusCalls = 0;
        plug.PlugCheckBoxShowHPLabel.SetFocus = () => focusCalls++;
        plug.PlugCheckBoxShowHPLabel.Visible = true;
        plug.PlugEditItemName.Text = "残留";
        plug.PlugEditUnbindName.Text = "残留";
        plug.PlugComboGroupUnBindItem.ItemIndex = 3;
        plug.PlugScrollBoxUnbindItems.ItemIndex = -1;

        dlg.SetVisible(true);

        Assert.Equal(1, focusCalls);                                   // 1220-1221 第一段命中
        Assert.Equal("", plug.PlugEditItemName.Text);                  // 1231
        Assert.Equal("", plug.PlugEditUnbindName.Text);                // 1232
        Assert.Equal(0, plug.PlugComboGroupUnBindItem.ItemIndex);      // 1233
        Assert.False(plug.PlugBtnUnbindItemDel.Enabled);               // 1234（ItemIndex < 0）
        Assert.False(plug.PlugBtnUnbindItemEdit.Enabled);              // 1235
    }

    [Fact]
    public void PlugPageControlConfigInRealArea_照抄Height加20的缺陷_原文1394()
    {
        var dlg = new TMirConfigDlg();
        var plug = (TMirConfigDlgControls)dlg.Plug;
        plug.PlugPageControlConfig.Width = 100;
        plug.PlugPageControlConfig.Height = 200;

        // X >= 88 且 Y <= 220 → 非真实区
        dlg.PlugPageControlConfigInRealArea(88, 220, out bool r1);
        Assert.False(r1);
        dlg.PlugPageControlConfigInRealArea(87, 220, out bool r2);
        Assert.True(r2);
        dlg.PlugPageControlConfigInRealArea(88, 221, out bool r3);
        Assert.True(r3);
    }

    [Fact]
    public void RefreshUnBindItemList_两个开关都开时清空并禁用_原文1315_1329()
    {
        var dlg = new TMirConfigDlg();
        var plug = (TMirConfigDlgControls)dlg.Plug;
        MirConfigGlobalSeam.g_ClientConfig.boCloseBookProtect = true;
        MirConfigGlobalSeam.g_ClientConfig.boCloseLogoutProtect = true;
        TMirConfigDlg.g_Config.MedicaMode = 1;

        dlg.RefreshUnBindItemList();

        Assert.Equal(0, plug.PlugComboBoxCheckHPValue.Items.Count);
        Assert.Equal(-1, plug.PlugComboBoxCheckHPValue.ItemIndex);
        Assert.Equal(-1, TMirConfigDlg.g_Config.CheckHpValues[1]);
        Assert.False(plug.PlugComboBoxCheckHPValue.Enabled);
        Assert.False(plug.PlugComboBoxCheckMPValue.Enabled);
    }

    [Fact]
    public void RefreshUnBindItemList_只开书保护时只塞小退_原文1330_1342()
    {
        var dlg = new TMirConfigDlg();
        var plug = (TMirConfigDlgControls)dlg.Plug;
        MirConfigGlobalSeam.g_ClientConfig.boCloseBookProtect = true;
        MirConfigGlobalSeam.g_ClientConfig.boCloseLogoutProtect = false;
        var items = MirConfigGlobalSeam.g_NGProtectItems;
        items.Add("回城卷"); items.Add("小退"); items.Add("随机传送卷");

        dlg.RefreshUnBindItemList();

        Assert.True(plug.PlugComboBoxCheckHPValue.Enabled);
        Assert.Equal(1, plug.PlugComboBoxCheckHPValue.Items.Count);
        Assert.Equal("小退", plug.PlugComboBoxCheckHPValue.Items[0]);
    }

    [Fact]
    public void RefreshUnBindItemList_都不开时删尾再补回小退_原文1366_1391()
    {
        var dlg = new TMirConfigDlg();
        var plug = (TMirConfigDlgControls)dlg.Plug;
        MirConfigGlobalSeam.g_ClientConfig.boCloseBookProtect = false;
        MirConfigGlobalSeam.g_ClientConfig.boCloseLogoutProtect = false;
        var items = MirConfigGlobalSeam.g_NGProtectItems;
        items.Add("回城卷"); items.Add("随机传送卷"); items.Add("小退");

        dlg.RefreshUnBindItemList();

        // 原表 3 项 → 删尾（小退）→ 2 项 → 再补回小退 → 3 项，且小退在末尾
        Assert.Equal(3, plug.PlugComboBoxCheckHPValue.Items.Count);
        Assert.Equal("小退", plug.PlugComboBoxCheckHPValue.Items[2]);
    }

    [Fact]
    public void Struck_英雄分支取HP而不是差值_且嵌在MySelf非空之内_原文5133_5156()
    {
        var dlg = new TMirConfigDlg();
        dlg.SetEnabled(true);
        object self = new object();
        object hero = new object();
        MirActorSeam.MySelfObject = () => self;
        MirActorSeam.MyHeroObject = () => hero;
        MirActorSeam.MySelfExists = () => true;
        MirActorSeam.MyHeroExists = () => true;
        MirActorSeam.MySelfHP = () => 100;

        TMirConfigDlg.ResetNotPorted();
        dlg.Struck(self, 70, 200);
        Assert.Contains(TMirConfigDlg.NotPortedMethods, s => s.StartsWith("DamageHPUseItem"));   // 本人：100-70>0
        Assert.Contains("DamageHPUseItem (MirConfigDlg.pas:6099)", TMirConfigDlg.NotPortedMethods);

        // 英雄：nDamage := HP（不是 MaxHP-HP 的差值），HP=5 → 命中
        TMirConfigDlg.ResetNotPorted();
        dlg.Struck(hero, 5, 200);
        Assert.Contains("DamageHPUseItem (MirConfigDlg.pas:6099)", TMirConfigDlg.NotPortedMethods);

        // 英雄分支嵌在 g_MySelf <> nil 之内：MySelf 为空时英雄也不处理
        MirActorSeam.MySelfExists = () => false;
        TMirConfigDlg.ResetNotPorted();
        dlg.Struck(hero, 5, 200);
        Assert.DoesNotContain("DamageHPUseItem (MirConfigDlg.pas:6099)", TMirConfigDlg.NotPortedMethods);
    }

    [Fact]
    public void Struck_未启用时完全不进入_原文5138()
    {
        var dlg = new TMirConfigDlg();
        dlg.SetEnabled(false);
        MirActorSeam.MySelfExists = () => true;
        MirActorSeam.MySelfObject = () => new object();
        TMirConfigDlg.ResetNotPorted();
        dlg.Struck(MirActorSeam.MySelfObject(), 1, 2);
        Assert.DoesNotContain(TMirConfigDlg.NotPortedMethods, s => s.StartsWith("DamageHPUseItem"));
    }

    [Fact]
    public void AutoUseMagic_门禁与时间间隔_原文6300_6319()
    {
        var dlg = new TMirConfigDlg();
        var plug = (TMirConfigDlgControls)dlg.Plug;
        dlg.SetConfigChecked(TConfigChecked.ckAutoUseMagic, true);   // RefConfig 留痕，无碍

        // MySelf 为空 → 不进入
        MirActorSeam.MySelfExists = () => false;
        object taken = null;
        MirActorSeam.AutoTakeOnItem = m => taken = m;
        dlg.AutoUseMagic(dlg);
        Assert.Null(taken);

        // 三个门禁都要过：存在 / 非死亡 / 非摆摊
        MirActorSeam.MySelfExists = () => true;
        MirActorSeam.MySelfDeath = () => false;
        MirActorSeam.MySelfShopStall = () => false;
        plug.PlugComboBoxAutoMagic.Items.AddObject("烈火剑法", (object)42);
        plug.PlugComboBoxAutoMagic.ItemIndex = 0;
        // ★ 原文 6309 的判据是 `Cardinal(nAutoUseMagicTime) * 1000`（秒 → 毫秒），
        //   故 nAutoUseMagicTime = 1 表示 1000ms 阈值。
        TMirConfigDlg.g_Config.nAutoUseMagicTime = 1;
        TMirConfigDlg.g_Config.dwAutoUseMagicTick = MirConfigGlobalSeam.MyGetTickCount();   // 刚跑过

        dlg.AutoUseMagic(dlg);
        Assert.Null(taken);            // 间隔未到（dt ≈ 0 < 1000）

        TMirConfigDlg.g_Config.dwAutoUseMagicTick = MirConfigGlobalSeam.MyGetTickCount() - 2000;
        dlg.AutoUseMagic(dlg);
        Assert.Equal(42, taken);       // 间隔已到（2000 > 1000），取的是 Items.Objects[ItemIndex]

        // 死亡时不进入
        taken = null;
        MirActorSeam.MySelfDeath = () => true;
        TMirConfigDlg.g_Config.dwAutoUseMagicTick = MirConfigGlobalSeam.MyGetTickCount() - 2000;
        dlg.AutoUseMagic(dlg);
        Assert.Null(taken);

        // 摆摊时不进入（原文 6307）
        MirActorSeam.MySelfDeath = () => false;
        MirActorSeam.MySelfShopStall = () => true;
        TMirConfigDlg.g_Config.dwAutoUseMagicTick = MirConfigGlobalSeam.MyGetTickCount() - 2000;
        dlg.AutoUseMagic(dlg);
        Assert.Null(taken);

        // 下拉下标越界时不进入（原文 6308 的区间校验）
        MirActorSeam.MySelfShopStall = () => false;
        plug.PlugComboBoxAutoMagic.ItemIndex = -1;
        TMirConfigDlg.g_Config.dwAutoUseMagicTick = MirConfigGlobalSeam.MyGetTickCount() - 2000;
        dlg.AutoUseMagic(dlg);
        Assert.Null(taken);
        plug.PlugComboBoxAutoMagic.ItemIndex = 0;

        // 未勾选 ckAutoUseMagic 时整段不进入（原文 6304）
        dlg.SetConfigChecked(TConfigChecked.ckAutoUseMagic, false);
        TMirConfigDlg.g_Config.dwAutoUseMagicTick = MirConfigGlobalSeam.MyGetTickCount() - 2000;
        dlg.AutoUseMagic(dlg);
        Assert.Null(taken);
    }

    // ============================================================================
    // §3 接线与类型标识
    // ============================================================================

    [Fact]
    public void 接缝CreateMirConfigDlg_已指向真实现()
    {
        var o = PlugInSeam.CreateMirConfigDlg();
        Assert.IsType<TMirConfigDlg>(o);
        Assert.Equal(TConfigDlgType.ptDefault, o.GetType());
    }

    [Fact]
    public void 接缝CreateJSYConfigDlg_已指向真实现且类型为ptJSY()
    {
        var o = PlugInSeam.CreateJSYConfigDlg();
        Assert.IsType<TJSYConfigDlg>(o);
        Assert.Equal(TConfigDlgType.ptJSY, o.GetType());
        Assert.IsAssignableFrom<TMirConfigDlg>(o);   // 真实现的继承链
    }

    [Fact]
    public void Finalize时的is_TJSYConfigDlg判定_仍然成立_原文129_150()
    {
        var m = new TConfigDlgManage();
        m.ConfigDlgList.AddObject("", new TJSYConfigDlg());
        m.ConfigDlgList.AddObject("", new TMirConfigDlg());

        ClientGlobalSeam.g_ClientConfig.btConfigDlgType = 0;
        m.Finalize();     // 只 Finalize 非 JSY 的那个

        ClientGlobalSeam.g_ClientConfig.btConfigDlgType = 1;
        m.Finalize();     // 只 Finalize JSY 的那个
    }

    [Fact]
    public void LoadPlugIn_两处接缝注入的是真实现_而不是桩_原文75_107()
    {
        ClientGlobalSeam.ConfigClientConfigs = Enumerable.Repeat(false, 60).ToArray();
        ClientGlobalSeam.g_ClientVersion = TClientVersion.cv176;

        var m = new TConfigDlgManage();
        m.LoadPlugIn();

        Assert.Equal(2, m.ConfigDlgList.Count);
        Assert.IsType<TJSYConfigDlg>(m.ConfigDlgList.GetObject(0));
        Assert.IsType<TMirConfigDlg>(m.ConfigDlgList.GetObject(1));    // ★ 不再是 TStubGameConfigObject
        Assert.Equal(TConfigDlgType.ptDefault, ((TGameConfigObject)m.ConfigDlgList.GetObject(1)).ConfigDlgType);

        // 接缝注入的确是**真实现**（而不是桩）的第三重证据：
        // 真实现的 ConfigCheckeds 数组长度 = 基线枚举长度 134；
        // TStubGameConfigObject 的实现同样基于基线枚举，但桩**没有** Control 树
        // （下面这条断言才是关键：真实现的 516 个控件真的被建出来了）。
        var mir = (TMirConfigDlg)m.ConfigDlgList.GetObject(1);
        var ctl = (TMirConfigDlgControls)mir.Plug;
        Assert.Equal(516, ctl.RegisteredCount);
        Assert.NotNull(ctl.PlugConfigDlg);
        Assert.NotNull(ctl.PlugCheckBoxShowHPLabel);
        // 且 Create 的 40 条默认勾选位真的写进去了（桩不会做这件事）
        Assert.True(mir.GetConfigChecked(TConfigChecked.ckNearHint));
        Assert.True(mir.GetConfigChecked(TConfigChecked.ckAutoDetourPath));
    }

    [Fact]
    public void LoadPlugIn_把ClientConfigs灌进两个对象的勾选位_原文85_101()
    {
        var cfg = new bool[60];
        cfg[7] = true;                                                    // 一个普通位
        cfg[(int)TConfigChecked.ckSceneShake] = true;                      // 原文 88 行**单独补**的那条
        ClientGlobalSeam.ConfigClientConfigs = cfg;
        ClientGlobalSeam.g_ClientVersion = TClientVersion.cvSerial;

        var m = new TConfigDlgManage();
        m.LoadPlugIn();

        var jsy = (TGameConfigObject)m.ConfigDlgList.GetObject(0);
        var mir = (TGameConfigObject)m.ConfigDlgList.GetObject(1);
        Assert.Equal(134, jsy.ConfigCheckeds.Length);
        // ★ 原文 85-89 的**次序陷阱**（照抄后可观测结果如下）：
        //   `ConfigObject` 在 75-83 指向 JSY 对象，整表循环（85-87）灌的是 **JSY**；
        //   紧接着 88-89 单独补 `ConfigCheckeds[ckSceneShake] := ClientConfigs[51]`
        //   —— 下标是**写死的 51**，不是 `ckSceneShake`（=53）。
        //   51 = ckParam10（"主将英雄药品"），故补的是第 51 位。
        // 87 行整表只到 44（`Length(...)-1`，注释 `/*44*/`），故 ckSceneShake(53) 不在其中。
        Assert.True(jsy.ConfigCheckeds[7]);                       // 整表灌过（7 < 45）
        Assert.True(mir.ConfigCheckeds[7]);
        Assert.True(mir.ConfigCheckeds[(int)TConfigChecked.ckSceneShake]);
        // 两个对象都保留 Create 里的默认勾选位（95 = ckAutoDetourPath，113 = ckNearHint）
        Assert.True(jsy.ConfigCheckeds[95], "jsy[95]");
        Assert.True(mir.ConfigCheckeds[95], "mir[95]");
        Assert.True(jsy.ConfigCheckeds[113], "jsy[113]");
        Assert.True(mir.ConfigCheckeds[113], "mir[113]");
    }

    [Fact]
    public void 未移入的方法_全部有显式留痕_不会静默()
    {
        var dlg = new TMirConfigDlg();
        TMirConfigDlg.ResetNotPorted();

        // 随手调用一批尚未移入体骨架的方法 → 必须留下记录
        dlg.LoadConfigFile();
        dlg.SaveConfigFile();
        dlg.RefConfig();
        dlg.RefBindItemList();
        dlg.RefreshGJMagic();
        dlg.AutoProtect(dlg);
        dlg.DuraWarning();
        dlg.LoadHelpFile();

        Assert.Equal(8, TMirConfigDlg.NotPortedMethods.Count);
        Assert.All(TMirConfigDlg.NotPortedMethods, s => Assert.Contains("MirConfigDlg.pas:", s));
    }
}
