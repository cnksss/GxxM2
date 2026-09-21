using System;
using System.Collections.Generic;
using System.Linq;
using GXX.Client.GUI.GameConfig;
using GXX.Client.GUI.GameConfig.Seams;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 批次 P1（车道 lane-client-guiconfig）：ConfigShare.pas(1-992) 1:1 移植测试。
/// 覆盖：TShortcutKey/TConfigDlgType 边界、GetKeyDownStr/GetKey 的按键名与修饰键优先级、
/// 以及 Find* 药/特殊药/技能书查找函数族的分支与边界。
/// </summary>
[Collection("GuiCfgConfigure")]
public sealed class GuiCfgConfigShareTests : IDisposable
{
    public GuiCfgConfigShareTests() => GuiCfgTestEnv.Reset();

    public void Dispose() => GuiCfgTestEnv.Reset();

    // ============================ 常量/类型 ============================

    [Fact]
    public void DlgTypeOrdinalsMatchOriginal()
    {
        // GameConfigDlg.pas:9  (ptDefault, ptJSY)
        Assert.Equal(0, (int)TConfigDlgType.ptDefault);
        Assert.Equal(1, (int)TConfigDlgType.ptJSY);
    }

    [Fact]
    public void ItemTypeOrdinalsMatchOriginal()
    {
        // GameConfigDlg.pas:11
        Assert.Equal(0, (int)TItemType.i_All);
        Assert.Equal(1, (int)TItemType.i_Other);
        Assert.Equal(2, (int)TItemType.i_HPMPDurg);
        Assert.Equal(3, (int)TItemType.i_Dress);
        Assert.Equal(4, (int)TItemType.i_Weapon);
        Assert.Equal(5, (int)TItemType.i_Jewelry);
        Assert.Equal(6, (int)TItemType.i_Decoration);
        Assert.Equal(7, (int)TItemType.i_Decorate);
        Assert.Equal(8, (int)TItemType.i_diy);
    }

    [Fact]
    public void ConfigCheckedSomeSpotChecksAndHigh()
    {
        // GameConfigDlg.pas:26-181 的枚举顺序即 ClientConfigs 下标，故抽查几个"跨界"成员
        Assert.Equal(0, (int)TConfigChecked.ckShowHPLabel);
        Assert.Equal(10, (int)TConfigChecked.ckSimpleShowHumanWeapon);
        Assert.Equal(11, (int)TConfigChecked.ckDisableSelfStruck);
        Assert.Equal(22, (int)TConfigChecked.ckHideWeaponEffect);
        Assert.Equal(33, (int)TConfigChecked.ckSmartFireHit);
        Assert.Equal(44, (int)TConfigChecked.ckHumShootLightenLockTarget);
        Assert.Equal(53, (int)TConfigChecked.ckSceneShake);
        Assert.Equal(55, (int)TConfigChecked.ckShowNpcHPLabel);
        Assert.Equal(67, (int)TConfigChecked.ckSimpleShowHumanDress);
        Assert.Equal(77, (int)TConfigChecked.ckSmartCustomHit1);
        Assert.Equal(85, (int)TConfigChecked.ckHumManuallyCustomHit1);
        Assert.Equal(91, (int)TConfigChecked.ckAutoContinueAttack);
        Assert.Equal(93, (int)TConfigChecked.ckHideMonsterIcons);
        Assert.Equal(95, (int)TConfigChecked.ckAutoDetourPath);
        Assert.Equal(96, (int)TConfigChecked.ckParam12);
        Assert.Equal(105, (int)TConfigChecked.ckUseSuperMedica);
        Assert.Equal(107, (int)TConfigChecked.ckHeroAutoReCallSlave);
        Assert.Equal(113, (int)TConfigChecked.ckNearHint);
        Assert.Equal(119, (int)TConfigChecked.ckAutoDownHorse);
        Assert.Equal(121, (int)TConfigChecked.ckGJ_NoRedPoison);
        Assert.Equal(131, (int)TConfigChecked.ckHideBigHPProgress);
        Assert.Equal(132, (int)TConfigChecked.ckHeroShowNumberState);
        Assert.Equal(133, (int)TConfigChecked.ckObjectHintEffect);
        Assert.Equal(133, TConfigCheckedBounds.HighOrdinal);
    }

    [Fact]
    public void ShortcutKeysArrayIsSixteenZeroInitialised()
    {
        // ConfigShare.pas:27/31  array[0..15] of TShortcutKey，Delphi 全局零初始化
        Assert.Equal(16, ConfigShareGlobal.g_ShortcutKeys.Length);
        Assert.Equal(0, TShortcutKeys.Low);
        Assert.Equal(15, TShortcutKeys.High);
        for (int i = TShortcutKeys.Low; i <= TShortcutKeys.High; i++)
        {
            Assert.Equal(0, ConfigShareGlobal.g_ShortcutKeys[i].Use);
            Assert.Equal(0, ConfigShareGlobal.g_ShortcutKeys[i].Key);
            Assert.Equal(DelphiShiftState.None, ConfigShareGlobal.g_ShortcutKeys[i].Shift);
        }
    }

    [Fact]
    public void ShortcutKeyUseIsByteComparedWithZero()
    {
        // 类型映射规程：Delphi Boolean → byte，与 0 比较
        var k = new TShortcutKey { Use = 1, Key = 65, Shift = DelphiShiftState.ssCtrl };
        Assert.True(k.Use != 0);
        ConfigShareGlobal.g_ShortcutKeys[3] = k;
        Assert.Equal(65, (int)ConfigShareGlobal.g_ShortcutKeys[3].Key);
        Assert.Equal(DelphiShiftState.ssCtrl, ConfigShareGlobal.g_ShortcutKeys[3].Shift);
    }

    [Fact]
    public void ShiftStateFlagBitsFollowDelphiEnumOrder()
    {
        // Delphi TShiftState = set of (ssShift, ssAlt, ssCtrl, ssLeft, ssRight, ssMiddle, ssDouble)
        Assert.Equal(1, (int)DelphiShiftState.ssShift);
        Assert.Equal(2, (int)DelphiShiftState.ssAlt);
        Assert.Equal(4, (int)DelphiShiftState.ssCtrl);
        Assert.Equal(8, (int)DelphiShiftState.ssLeft);
        Assert.Equal(16, (int)DelphiShiftState.ssRight);
        Assert.Equal(32, (int)DelphiShiftState.ssMiddle);
        Assert.Equal(64, (int)DelphiShiftState.ssDouble);
    }

    // ============================ GetKey ============================

    [Fact]
    public void GetKeyDigitsLettersAndNumpadAreChars()
    {
        // ConfigShare.pas:93-96
        Assert.Equal("0", ConfigShare.GetKey(48));
        Assert.Equal("9", ConfigShare.GetKey(57));
        Assert.Equal("A", ConfigShare.GetKey(65));
        Assert.Equal("Z", ConfigShare.GetKey(90));
        // 96..105 走 Chr(Key)：Chr(96)='`'、Chr(97)='a'、Chr(105)='i'（原文就是 Chr，不是 '0'..'9'）
        Assert.Equal("`", ConfigShare.GetKey(96));
        Assert.Equal("a", ConfigShare.GetKey(97));
        Assert.Equal("i", ConfigShare.GetKey(105));
    }

    [Fact]
    public void GetKeyTabScrollAndFKeys()
    {
        // ConfigShare.pas:97-118
        Assert.Equal("Tab", ConfigShare.GetKey(9));
        Assert.Equal("Scroll", ConfigShare.GetKey(145));
        Assert.Equal("F1", ConfigShare.GetKey(112));
        Assert.Equal("F12", ConfigShare.GetKey(123));
    }

    [Theory]
    [InlineData(186, ";")]
    [InlineData(187, "=")]
    [InlineData(188, ",")]
    [InlineData(189, "-")]
    [InlineData(190, ".")]
    [InlineData(191, "/")]
    [InlineData(192, "`")]
    [InlineData(219, "[")]
    [InlineData(220, "\\")]
    [InlineData(221, "]")]
    public void GetKeyOemKeysMapExactly(ushort key, string expected)
    {
        // ConfigShare.pas:119-133
        Assert.Equal(expected, ConfigShare.GetKey(key));
    }

    [Fact]
    public void GetKey222IsEscChar()
    {
        // ConfigShare.pas:131  Result := Char(27)
        Assert.Equal(((char)27).ToString(), ConfigShare.GetKey(222));
    }

    [Theory]
    [InlineData(8, "退格")]
    [InlineData(9, "Tab")]
    [InlineData(13, "Enter")]
    [InlineData(32, "空格")]
    [InlineData(33, "PageUp")]
    [InlineData(34, "PageDown")]
    [InlineData(35, "End")]
    [InlineData(36, "Home")]
    [InlineData(45, "Insert")]
    [InlineData(46, "Delete")]
    public void GetKeyNamedKeysMapExactly(ushort key, string expected)
    {
        // ConfigShare.pas:135-148
        Assert.Equal(expected, ConfigShare.GetKey(key));
    }

    [Fact]
    public void GetKeyUnmappedReturnsEmpty()
    {
        // 16=Shift 不在任何分支（8..46 的 case 无 16）
        Assert.Equal("", ConfigShare.GetKey(16));
        Assert.Equal("", ConfigShare.GetKey(1));
    }

    [Fact]
    public void GetKeyF13IsNotHandled()
    {
        // 上界是 VK_F12(123)，124 (F13) 无分支
        Assert.Equal("", ConfigShare.GetKey(124));
    }

    // ============================ GetKeyDownStr ============================

    [Fact]
    public void PlainLetterHasNoPrefix()
    {
        // ConfigShare.pas:152-195：无修饰键时只返回 GetKey 结果
        Assert.Equal("A", ConfigShare.GetKeyDownStr(65, DelphiShiftState.None));
        Assert.Equal("A", ConfigShare.GetKeyDownStr(65, DelphiShiftState.None, true));
    }

    [Theory]
    [InlineData(DelphiShiftState.ssShift, "Shift+A")]
    [InlineData(DelphiShiftState.ssAlt, "Alt+A")]
    [InlineData(DelphiShiftState.ssCtrl, "Ctrl+A")]
    [InlineData(DelphiShiftState.ssLeft, "Left+A")]
    [InlineData(DelphiShiftState.ssRight, "Right+A")]
    [InlineData(DelphiShiftState.ssMiddle, "Middle+A")]
    [InlineData(DelphiShiftState.ssDouble, "Double+A")]
    public void ModifierPrefixesMatchOriginal(DelphiShiftState shift, string expected)
    {
        Assert.Equal(expected, ConfigShare.GetKeyDownStr(65, shift));
    }

    [Fact]
    public void ModifierPrefixUsesMutuallyExclusiveIfElseChain()
    {
        // ConfigShare.pas:171-191 是 if / else if 链：Shift 优先于 Alt，Alt 优先于 Ctrl，
        // Ctrl 优先于 Left/Right/Middle/Double。
        var all = DelphiShiftState.ssShift | DelphiShiftState.ssAlt | DelphiShiftState.ssCtrl
                | DelphiShiftState.ssLeft | DelphiShiftState.ssRight | DelphiShiftState.ssMiddle
                | DelphiShiftState.ssDouble;
        Assert.Equal("Shift+A", ConfigShare.GetKeyDownStr(65, all));

        Assert.Equal("Alt+A", ConfigShare.GetKeyDownStr(65, DelphiShiftState.ssAlt | DelphiShiftState.ssCtrl));
        Assert.Equal("Ctrl+A", ConfigShare.GetKeyDownStr(65, DelphiShiftState.ssCtrl | DelphiShiftState.ssDouble));
        Assert.Equal("Left+A", ConfigShare.GetKeyDownStr(65, DelphiShiftState.ssLeft | DelphiShiftState.ssDouble));
        Assert.Equal("Right+A", ConfigShare.GetKeyDownStr(65, DelphiShiftState.ssRight | DelphiShiftState.ssMiddle));
        Assert.Equal("Middle+A", ConfigShare.GetKeyDownStr(65, DelphiShiftState.ssMiddle | DelphiShiftState.ssDouble));
    }

    [Fact]
    public void ModifierKeysThemselvesGetNoPrefix()
    {
        // 不满足 (Key <> VK_MENU) and ... 这一串条件 → **不加前缀**；
        // 但 ConfigShare.pas:194 的 `Result := Result + GetKey(Key)` 仍无条件执行：
        //   VK_TAB(9)    → GetKey='Tab'
        //   VK_SCROLL(145) → GetKey='Scroll'
        //   VK_MENU(18)/VK_CONTROL(17)/VK_SHIFT(16) 不在任何 case → ''
        Assert.Equal("", ConfigShare.GetKeyDownStr(18, DelphiShiftState.ssShift));     // VK_MENU
        Assert.Equal("", ConfigShare.GetKeyDownStr(17, DelphiShiftState.ssShift));     // VK_CONTROL
        Assert.Equal("", ConfigShare.GetKeyDownStr(16, DelphiShiftState.ssShift));     // VK_SHIFT
        Assert.Equal("Tab", ConfigShare.GetKeyDownStr(9, DelphiShiftState.ssShift));   // VK_TAB
        Assert.Equal("Scroll", ConfigShare.GetKeyDownStr(145, DelphiShiftState.ssShift)); // VK_SCROLL
        // 全部修饰键都置上也不会给 Tab 加前缀
        var all = DelphiShiftState.ssShift | DelphiShiftState.ssAlt | DelphiShiftState.ssCtrl
                | DelphiShiftState.ssLeft | DelphiShiftState.ssRight | DelphiShiftState.ssMiddle
                | DelphiShiftState.ssDouble;
        Assert.Equal("Tab", ConfigShare.GetKeyDownStr(9, all));
    }

    [Fact]
    public void FunctionKeysRequireIncludeFnForPrefix()
    {
        // ConfigShare.pas:156-161 + 163-169：IsFN 只有 IncludeFN=True 时才成立
        Assert.Equal("F1", ConfigShare.GetKeyDownStr(112, DelphiShiftState.ssCtrl, false));       // 无前缀
        Assert.Equal("Ctrl+F1", ConfigShare.GetKeyDownStr(112, DelphiShiftState.ssCtrl, true));  // 有前缀
        // 默认参数 IncludeFN=False
        Assert.Equal("F5", ConfigShare.GetKeyDownStr(116, DelphiShiftState.ssAlt));
    }

    [Fact]
    public void OemRangeGetsPrefix()
    {
        // 186..222 也在允许加前缀的集合里
        Assert.Equal("Ctrl+;", ConfigShare.GetKeyDownStr(186, DelphiShiftState.ssCtrl));
        Assert.Equal("Shift+\\", ConfigShare.GetKeyDownStr(220, DelphiShiftState.ssShift));
        // 222 的键名是 Char(27)
        Assert.Equal("Ctrl+" + ((char)27), ConfigShare.GetKeyDownStr(222, DelphiShiftState.ssCtrl));
    }

    [Fact]
    public void GetKeyIsAlwaysAppendedEvenForDisallowedKeys()
    {
        // ConfigShare.pas:194：`Result := Result + GetKey(Key);` 在 if 之外，无条件执行。
        // 例：VK_TAB(9) 不加前缀，但仍取 GetKey(9)='Tab'
        Assert.Equal("Tab", ConfigShare.GetKeyDownStr(9, DelphiShiftState.ssCtrl));
        // VK_SCROLL(145) 不在 8..46 → GetKey 返回 'Scroll'
        Assert.Equal("Scroll", ConfigShare.GetKeyDownStr(145, DelphiShiftState.ssCtrl));
        // VK_SHIFT(16) 不在任何 case → 空串
        Assert.Equal("", ConfigShare.GetKeyDownStr(16, DelphiShiftState.ssCtrl));
    }

    [Fact]
    public void KeysOutsideAllRangesProduceNothing()
    {
        // 1..7 与 47 均不在 8..46 也不在其它范围
        Assert.Equal("", ConfigShare.GetKeyDownStr(5, DelphiShiftState.ssShift));
        Assert.Equal("", ConfigShare.GetKeyDownStr(47, DelphiShiftState.ssShift));
    }

    [Fact]
    public void Key47HasNoNameButWouldNeedPrefixCheck()
    {
        // 47 不在 "数字/字母/小键盘/F/OEM" 集合内 → 不加前缀；
        // 也不在 GetKey 的 8..46 → 结果为空串
        Assert.Equal("", ConfigShare.GetKeyDownStr(47, DelphiShiftState.ssAlt, true));
    }

    // ============================ Find* 公共脚手架 ============================

    private static TClientItemSeam Item(string name, byte stdMode = 0, ushort shape = 0,
        byte reserved = 0, int ac1 = 0, int mac1 = 0)
        => new TClientItemSeam
        {
            s = new TStdItemSeam
            {
                Name = name ?? "",
                StdMode = stdMode,
                Shape = shape,
                Reserved = reserved,
                AC1 = ac1,
                MAC1 = mac1,
            }
        };

    private static TUnBindItem Bind(string name, TUnBindItemType type, int shape)
        => new TUnBindItem { sItemName = name, UnBindItemType = type, nShape = shape };

    /// <summary>构建 60 格包裹（前 6 格为装备位）与 40 格英雄包裹。</summary>
    private static void Setup(int filledBagSlots = 0, int heroBagCount = 40, int heroFilledSlots = 0)
    {
        // 原文 g_ItemArr 是 array[0..ALL_BAG_ITEM_COUNT-1]，ALL_BAG_ITEM_COUNT 含扩展页；
        // 这里开 60 格以便验证"扫描上界是 GetMaxBagCount 而非数组长度"。
        var bag = new TClientItemSeam[60];
        for (int i = 0; i < bag.Length; i++) bag[i] = Item("");
        for (int i = 0; i < filledBagSlots && i < bag.Length; i++) bag[i] = Item("占用" + i);
        ConfigShareSeam.g_ItemArr = bag;

        var hero = new TClientItemSeam[ConfigShareSeam.MAX_HERO_BAG_ITEM];
        for (int i = 0; i < hero.Length; i++) hero[i] = Item("");
        for (int i = 0; i < heroFilledSlots && i < hero.Length; i++) hero[i] = Item("英雄占用" + i);
        ConfigShareSeam.g_HeroItemArr = hero;

        ConfigShareSeam.g_ExtBagOpenItemCount = 0;
        ConfigShareSeam.g_UnbindItemList = new List<TUnBindItem>();
        ConfigShareSeam.g_MyHero = new FakeHero(heroBagCount);
    }

    private sealed class FakeHero : IHeroActorSeam
    {
        public FakeHero(int bagCount) { m_nBagCount = bagCount; }
        public int m_nBagCount { get; }
    }

    /// <summary>HumBagNoUseItemCount &gt; 6 是绑定药查找的**总闸门**。</summary>
    private static void OpenHumGate() => Setup(filledBagSlots: 0);

    [Fact]
    public void HumGateIsHumBagNoUseItemCountGreaterThanSix()
    {
        // 原文 `if HumBagNoUseItemCount > 6`：空包裹 46 格 → 46 > 6 成立
        Setup();
        Assert.Equal(46, ConfigShareSeam.HumBagNoUseItemCount());
        Assert.True(ConfigShareSeam.HumBagNoUseItemCount() > 6);

        // 填满 40 格 → 剩 6 格 → 6 > 6 不成立，绑定药查找一律返回 -1
        Setup(filledBagSlots: 40);
        Assert.Equal(6, ConfigShareSeam.HumBagNoUseItemCount());
        ConfigShareSeam.g_UnbindItemList.Add(Bind("太阳水", TUnBindItemType.t_HP, 5));
        ConfigShareSeam.g_ItemArr[10] = Item("太阳水", stdMode: 31, shape: 5);
        Assert.Equal(-1, ConfigShare.FindHumBindHPItemIndex());
        Assert.Equal(-1, ConfigShare.FindHumBindHPItemIndex("太阳水"));
    }

    [Fact]
    public void HumGateIgnoresFirstSixSlots()
    {
        // 原文统计 Low(g_ItemArr) .. GetMaxBagCount-1，即 0..45 全算
        Setup(filledBagSlots: 6);
        Assert.Equal(40, ConfigShareSeam.HumBagNoUseItemCount());
    }

    [Fact]
    public void GetMaxBagCountAddsExtBagOpenCount()
    {
        Setup();
        Assert.Equal(46, ConfigShareSeam.GetMaxBagCount());
        ConfigShareSeam.g_ExtBagOpenItemCount = 3;
        Assert.Equal(49, ConfigShareSeam.GetMaxBagCount());
        Assert.Equal(ConfigShareSeam.DEF_MAX_BAG_ITEM, 46);
        Assert.Equal(ConfigShareSeam.MAX_HERO_BAG_ITEM, 40);
    }

    // ============================ FindBagItemName / FindHeroBagItemName ============================

    [Fact]
    public void FindBagItemNameMatchesCaseInsensitively()
    {
        // 原文用 CompareText（大小写不敏感）
        Setup();
        ConfigShareSeam.g_ItemArr[7] = Item("SunWater");
        Assert.Equal(7, ConfigShare.FindBagItemName("sunwater"));
        Assert.Equal(7, ConfigShare.FindBagItemName("SUNWATER"));
        Assert.Equal(-1, ConfigShare.FindBagItemName("MoonWater"));
    }

    [Fact]
    public void FindBagItemNameSkipsEmptyNames()
    {
        // 空名槽位即使 sItemName 也是 '' 也不匹配（原文先判 <> ''）
        Setup();
        Assert.Equal(-1, ConfigShare.FindBagItemName(""));
    }

    [Fact]
    public void FindBagItemNameReturnsFirstHit()
    {
        Setup();
        ConfigShareSeam.g_ItemArr[8] = Item("重复");
        ConfigShareSeam.g_ItemArr[30] = Item("重复");
        Assert.Equal(8, ConfigShare.FindBagItemName("重复"));
    }

    [Fact]
    public void FindHeroBagItemNameScanWholeHeroArray()
    {
        Setup();
        ConfigShareSeam.g_HeroItemArr[39] = Item("HeroThing");
        Assert.Equal(39, ConfigShare.FindHeroBagItemName("herothing"));
        Assert.Equal(-1, ConfigShare.FindHeroBagItemName("nope"));
    }

    // ============================ Bind HP/MP/Special ============================

    [Fact]
    public void BindHPNoArgRequiresStdMode31ShapeMatchAndReservedZero()
    {
        OpenHumGate();
        ConfigShareSeam.g_UnbindItemList.Add(Bind("太阳水", TUnBindItemType.t_HP, 5));
        ConfigShareSeam.g_ItemArr[10] = Item("太阳水", stdMode: 31, shape: 5);
        Assert.Equal(10, ConfigShare.FindHumBindHPItemIndex());

        // Reserved <> 0 → 不命中（原文 `and (g_ItemArr[I].s.Reserved = 0)`）
        Setup();
        ConfigShareSeam.g_UnbindItemList.Add(Bind("太阳水", TUnBindItemType.t_HP, 5));
        ConfigShareSeam.g_ItemArr[10] = Item("太阳水", stdMode: 31, shape: 5, reserved: 1);
        Assert.Equal(-1, ConfigShare.FindHumBindHPItemIndex());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(15)]
    [InlineData(33)]
    [InlineData(51)]
    public void BindHPRejectsShapeIn0_1_15To51(ushort shape)
    {
        // 原文 `not (Shape in [0, 1, 15..51])`
        OpenHumGate();
        ConfigShareSeam.g_UnbindItemList.Add(Bind("太阳水", TUnBindItemType.t_HP, shape));
        ConfigShareSeam.g_ItemArr[10] = Item("太阳水", stdMode: 31, shape: shape);
        Assert.Equal(-1, ConfigShare.FindHumBindHPItemIndex());
    }

    [Theory]
    [InlineData(2)]
    [InlineData(14)]
    [InlineData(52)]
    public void BindHPAcceptsShapeOutsideExcludedSet(ushort shape)
    {
        OpenHumGate();
        ConfigShareSeam.g_UnbindItemList.Add(Bind("太阳水", TUnBindItemType.t_HP, shape));
        ConfigShareSeam.g_ItemArr[10] = Item("太阳水", stdMode: 31, shape: shape);
        Assert.Equal(10, ConfigShare.FindHumBindHPItemIndex());
    }

    [Fact]
    public void BindHPRejectsWrongStdMode()
    {
        OpenHumGate();
        ConfigShareSeam.g_UnbindItemList.Add(Bind("太阳水", TUnBindItemType.t_HP, 5));
        ConfigShareSeam.g_ItemArr[10] = Item("太阳水", stdMode: 30, shape: 5);
        Assert.Equal(-1, ConfigShare.FindHumBindHPItemIndex());
    }

    [Fact]
    public void BindHPByTypeIsNotSatisfiedByMPBinding()
    {
        // UnBindItemType 必须精确等于 t_HP
        OpenHumGate();
        ConfigShareSeam.g_UnbindItemList.Add(Bind("太阳水", TUnBindItemType.t_MP, 5));
        ConfigShareSeam.g_ItemArr[10] = Item("太阳水", stdMode: 31, shape: 5);
        Assert.Equal(-1, ConfigShare.FindHumBindHPItemIndex());
        Assert.Equal(10, ConfigShare.FindHumBindMPItemIndex());
    }

    [Fact]
    public void BindHPStartsFromSlotSix()
    {
        // 原文 `for I := Low(g_ItemArr) + 6`：前 6 格不参与绑定药查找
        OpenHumGate();
        ConfigShareSeam.g_UnbindItemList.Add(Bind("太阳水", TUnBindItemType.t_HP, 5));
        ConfigShareSeam.g_ItemArr[5] = Item("太阳水", stdMode: 31, shape: 5);
        Assert.Equal(-1, ConfigShare.FindHumBindHPItemIndex());
        ConfigShareSeam.g_ItemArr[6] = Item("太阳水", stdMode: 31, shape: 5);
        Assert.Equal(6, ConfigShare.FindHumBindHPItemIndex());
    }

    [Fact]
    public void BindHPByNameExitsAfterFirstMatchingBindingEvenIfNotFound()
    {
        // 原文 251-276：绑定表命中后**内层扫完就 Exit**，不会继续试后续绑定项
        OpenHumGate();
        ConfigShareSeam.g_UnbindItemList.Add(Bind("太阳水", TUnBindItemType.t_HP, 99)); // 形状不匹配
        ConfigShareSeam.g_UnbindItemList.Add(Bind("太阳水", TUnBindItemType.t_HP, 5));  // 形状匹配
        ConfigShareSeam.g_ItemArr[10] = Item("太阳水", stdMode: 31, shape: 5);
        Assert.Equal(-1, ConfigShare.FindHumBindHPItemIndex("太阳水"));
    }

    [Fact]
    public void BindHPByNameRequiresReservedZero()
    {
        OpenHumGate();
        ConfigShareSeam.g_UnbindItemList.Add(Bind("太阳水", TUnBindItemType.t_HP, 5));
        ConfigShareSeam.g_ItemArr[10] = Item("太阳水", stdMode: 31, shape: 5, reserved: 0);
        Assert.Equal(10, ConfigShare.FindHumBindHPItemIndex("太阳水"));

        ConfigShareSeam.g_ItemArr[10] = Item("太阳水", stdMode: 31, shape: 5, reserved: 2);
        Assert.Equal(-1, ConfigShare.FindHumBindHPItemIndex("太阳水"));
    }

    [Fact]
    public void BindMPByNameDoesNotCheckReservedAsymmetry()
    {
        // 原文缺陷（照抄）：FindHumBindMPItemIndex(sItemName) 的内层**没有** Reserved 判定，
        // 而 FindHumBindHPItemIndex(sItemName) 有。
        OpenHumGate();
        ConfigShareSeam.g_UnbindItemList.Add(Bind("魔法药", TUnBindItemType.t_MP, 5));
        ConfigShareSeam.g_ItemArr[10] = Item("魔法药", stdMode: 31, shape: 5, reserved: 7);
        Assert.Equal(10, ConfigShare.FindHumBindMPItemIndex("魔法药"));
    }

    [Fact]
    public void BindSpecialUsesSpecialType()
    {
        OpenHumGate();
        ConfigShareSeam.g_UnbindItemList.Add(Bind("祝福油", TUnBindItemType.t_Special, 9));
        ConfigShareSeam.g_ItemArr[11] = Item("祝福油", stdMode: 31, shape: 9);
        Assert.Equal(11, ConfigShare.FindHumBindSpecialItemIndex());
        Assert.Equal(11, ConfigShare.FindHumBindSpecialItemIndex("祝福油"));
        // 找 HP 时不应命中 t_Special
        Assert.Equal(-1, ConfigShare.FindHumBindHPItemIndex());
    }

    [Fact]
    public void BindEmptyUnbindListReturnsMinusOne()
    {
        OpenHumGate();
        ConfigShareSeam.g_ItemArr[10] = Item("太阳水", stdMode: 31, shape: 5);
        Assert.Equal(-1, ConfigShare.FindHumBindHPItemIndex());
        Assert.Equal(-1, ConfigShare.FindHumBindMPItemIndex());
        Assert.Equal(-1, ConfigShare.FindHumBindSpecialItemIndex());
    }

    // ============================ Hero 侧（门槛不同） ============================

    [Fact]
    public void HeroBindRequiresHeroPresent()
    {
        Setup();
        ConfigShareSeam.g_MyHero = null;
        Assert.Equal(-1, ConfigShare.FindHeroBindHPItemIndex());
        Assert.Equal(-1, ConfigShare.FindHeroBindHPItemIndex("太阳水"));
        Assert.Equal(-1, ConfigShare.FindHeroBindItemIndex("太阳水"));
    }

    [Fact]
    public void HeroBindGateIsBagCountMinusHeroBagItemCountAtLeastSix()
    {
        // 原文 `g_MyHero.m_nBagCount - HeroBagItemCount >= 6`
        Setup(heroFilledSlots: 0, heroBagCount: 40);
        Assert.Equal(0, ConfigShareSeam.HeroBagItemCount());
        ConfigShareSeam.g_UnbindItemList.Add(Bind("太阳水", TUnBindItemType.t_HP, 5));
        ConfigShareSeam.g_HeroItemArr[7] = Item("太阳水", stdMode: 31, shape: 5);
        Assert.Equal(7, ConfigShare.FindHeroBindHPItemIndex());

        // 填到只剩 5 格 → 40-35=5 < 6 → 关闭
        Setup(heroFilledSlots: 35, heroBagCount: 40);
        ConfigShareSeam.g_UnbindItemList.Add(Bind("太阳水", TUnBindItemType.t_HP, 5));
        ConfigShareSeam.g_HeroItemArr[37] = Item("太阳水", stdMode: 31, shape: 5);
        Assert.Equal(-1, ConfigShare.FindHeroBindHPItemIndex());
    }

    [Fact]
    public void HeroBindHPDoesNotCheckReserved()
    {
        // 英雄侧一律不判 Reserved（原文 288/315/390/417/492/519 行都无该条件）
        Setup(heroFilledSlots: 0, heroBagCount: 40);
        ConfigShareSeam.g_UnbindItemList.Add(Bind("太阳水", TUnBindItemType.t_HP, 5));
        ConfigShareSeam.g_HeroItemArr[9] = Item("太阳水", stdMode: 31, shape: 5, reserved: 9);
        Assert.Equal(9, ConfigShare.FindHeroBindHPItemIndex());
        Assert.Equal(9, ConfigShare.FindHeroBindHPItemIndex("太阳水"));
    }

    [Fact]
    public void HeroBagItemCountCountsNonEmpty()
    {
        Setup(heroFilledSlots: 3);
        Assert.Equal(3, ConfigShareSeam.HeroBagItemCount());
    }

    // ============================ 未绑定（解包）药 ============================

    [Fact]
    public void UnBindHPFirstPassUsesShape0Or1()
    {
        OpenHumGate();
        ConfigShareSeam.g_ItemArr[9] = Item("金创药", stdMode: 0, shape: 1, ac1: 10);
        Assert.Equal(9, ConfigShare.FindHumUnBindHPItemIndex());
    }

    [Fact]
    public void UnBindHPFirstPassRejectsShape2()
    {
        OpenHumGate();
        ConfigShareSeam.g_ItemArr[9] = Item("金创药", stdMode: 0, shape: 2, ac1: 10);
        Assert.Equal(-1, ConfigShare.FindHumUnBindHPItemIndex());
    }

    [Fact]
    public void UnBindHPSecondPassOnlyScansFirstSixAndRequiresShapeZero()
    {
        // 原文 549-556：第二段 0..5，判据是 Shape = 0（**不是** 0,1）
        OpenHumGate();
        ConfigShareSeam.g_ItemArr[3] = Item("金创药", stdMode: 0, shape: 0, ac1: 10);
        Assert.Equal(3, ConfigShare.FindHumUnBindHPItemIndex());

        OpenHumGate();
        ConfigShareSeam.g_ItemArr[3] = Item("金创药", stdMode: 0, shape: 1, ac1: 10);
        Assert.Equal(-1, ConfigShare.FindHumUnBindHPItemIndex());
    }

    [Fact]
    public void UnBindHPRequiresAc1PositiveAndMac1Zero()
    {
        OpenHumGate();
        ConfigShareSeam.g_ItemArr[9] = Item("金创药", stdMode: 0, shape: 0, ac1: 0);
        Assert.Equal(-1, ConfigShare.FindHumUnBindHPItemIndex());

        OpenHumGate();
        ConfigShareSeam.g_ItemArr[9] = Item("金创药", stdMode: 0, shape: 0, ac1: 5, mac1: 1);
        Assert.Equal(-1, ConfigShare.FindHumUnBindHPItemIndex());
    }

    [Fact]
    public void UnBindMPRequiresAc1ZeroAndMac1Positive()
    {
        OpenHumGate();
        ConfigShareSeam.g_ItemArr[9] = Item("魔法药", stdMode: 0, shape: 0, ac1: 0, mac1: 5);
        Assert.Equal(9, ConfigShare.FindHumUnBindMPItemIndex());

        OpenHumGate();
        ConfigShareSeam.g_ItemArr[9] = Item("魔法药", stdMode: 0, shape: 0, ac1: 1, mac1: 5);
        Assert.Equal(-1, ConfigShare.FindHumUnBindMPItemIndex());
    }

    [Fact]
    public void UnBindSpecialRequiresShapeAc1Mac1AllPositive()
    {
        OpenHumGate();
        ConfigShareSeam.g_ItemArr[9] = Item("祝福油", stdMode: 0, shape: 3, ac1: 1, mac1: 1);
        Assert.Equal(9, ConfigShare.FindHumUnBindSpecialItemIndex());

        OpenHumGate();
        ConfigShareSeam.g_ItemArr[9] = Item("祝福油", stdMode: 0, shape: 0, ac1: 1, mac1: 1);
        Assert.Equal(-1, ConfigShare.FindHumUnBindSpecialItemIndex());
    }

    [Fact]
    public void UnBindByTypeOnEqualNameSkipsShapeChecks()
    {
        // OnEqualName=True 时命中即返回，不看装备形态
        OpenHumGate();
        ConfigShareSeam.g_ItemArr[9] = Item("金创药", stdMode: 99, shape: 99, ac1: 0, mac1: 9);
        Assert.Equal(-1, ConfigShare.FindHumUnBindHPItemIndex("金创药", false));
        Assert.Equal(9, ConfigShare.FindHumUnBindHPItemIndex("金创药", true));
    }

    [Fact]
    public void UnBindByTypeDefaultOnEqualNameIsFalse()
    {
        OpenHumGate();
        ConfigShareSeam.g_ItemArr[9] = Item("金创药", stdMode: 99, shape: 99, ac1: 0, mac1: 9);
        Assert.Equal(-1, ConfigShare.FindHumUnBindHPItemIndex("金创药"));
    }

    [Fact]
    public void UnBindHeroOnlyShapeZero()
    {
        Setup();
        ConfigShareSeam.g_HeroItemArr[9] = Item("金创药", stdMode: 0, shape: 0, ac1: 10);
        Assert.Equal(9, ConfigShare.FindHeroUnBindHPItemIndex());

        Setup();
        ConfigShareSeam.g_HeroItemArr[9] = Item("金创药", stdMode: 0, shape: 1, ac1: 10);
        Assert.Equal(-1, ConfigShare.FindHeroUnBindHPItemIndex());
    }

    [Fact]
    public void UnBindByNameIsCaseInsensitive()
    {
        OpenHumGate();
        ConfigShareSeam.g_ItemArr[9] = Item("金创药", stdMode: 0, shape: 0, ac1: 10);
        Assert.Equal(9, ConfigShare.FindHumUnBindHPItemIndex("金创药"));
        Assert.Equal(9, ConfigShare.FindHumUnBindBookItemIndex("金创药"));
    }

    // ============================ 技能书 / 通用绑定 ============================

    [Fact]
    public void UnBindBookHasNoShapeOrStatChecks()
    {
        OpenHumGate();
        ConfigShareSeam.g_ItemArr[20] = Item("召唤骷髅", stdMode: 77, shape: 88, reserved: 5, ac1: -1, mac1: -1);
        Assert.Equal(20, ConfigShare.FindHumUnBindBookItemIndex("召唤骷髅"));
    }

    [Fact]
    public void BindBookDoesNotCheckUnbindItemType()
    {
        // 原文 821-846：命中绑定名 + StdMode=31 + Shape 不在排除集 → 就返回，**不看 UnBindItemType**
        OpenHumGate();
        ConfigShareSeam.g_UnbindItemList.Add(Bind("召唤骷髅", TUnBindItemType.t_Book, 12));
        ConfigShareSeam.g_ItemArr[20] = Item("召唤骷髅", stdMode: 31, shape: 12);
        Assert.Equal(20, ConfigShare.FindHumBindBookItemIndex("召唤骷髅"));
        Assert.Equal(20, ConfigShare.FindHumBookItemIndex("召唤骷髅"));
    }

    [Fact]
    public void BindBookRejectsExcludedShape()
    {
        OpenHumGate();
        ConfigShareSeam.g_UnbindItemList.Add(Bind("回城卷", TUnBindItemType.t_Book, 20));
        ConfigShareSeam.g_ItemArr[20] = Item("回城卷", stdMode: 31, shape: 20);
        Assert.Equal(-1, ConfigShare.FindHumBindBookItemIndex("回城卷"));
    }

    [Fact]
    public void BindItemIndexMatchesAnyTypeOnShape()
    {
        // FindHumBindItemIndex 不看 UnBindItemType，只看 shape 匹配。
        // 注意判据是 `not (Shape in [0, 1, 15..51])`，故 shape 必须**不在**该集合内（这里取 12）。
        // （绑定表记录的 sItemName 必须与请求名一致，原文 859 行是 CompareText(UnBindItem.sItemName, sItemName)）
        OpenHumGate();
        ConfigShareSeam.g_UnbindItemList.Add(Bind("任意", TUnBindItemType.t_Book, 12));
        ConfigShareSeam.g_ItemArr[20] = Item("任意", stdMode: 31, shape: 12);
        Assert.Equal(20, ConfigShare.FindHumBindItemIndex("任意"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(15)]
    [InlineData(30)]
    [InlineData(51)]
    public void BindItemIndexRejectsShapeInExcludedSet(ushort shape)
    {
        // 原文 848-873：`not (g_ItemArr[I].s.Shape in [0, 1, 15..51])` ——
        // 落在排除集内的 shape 即使与绑定表一致也**不命中**。
        OpenHumGate();
        ConfigShareSeam.g_UnbindItemList.Add(Bind("任意", TUnBindItemType.t_Book, shape));
        ConfigShareSeam.g_ItemArr[20] = Item("任意", stdMode: 31, shape: shape);
        Assert.Equal(-1, ConfigShare.FindHumBindItemIndex("任意"));
        // 对照：同一 shape 走"不看 shape 排除集"的 Book 版… 也仍然被排除（Book 版同样带该条件）
        Assert.Equal(-1, ConfigShare.FindHumBindBookItemIndex("任意"));
    }

    [Theory]
    [InlineData(2)]
    [InlineData(12)]
    [InlineData(14)]
    [InlineData(52)]
    public void BindItemIndexAcceptsShapeOutsideExcludedSet(ushort shape)
    {
        OpenHumGate();
        ConfigShareSeam.g_UnbindItemList.Add(Bind("任意", TUnBindItemType.t_Book, shape));
        ConfigShareSeam.g_ItemArr[20] = Item("任意", stdMode: 31, shape: shape);
        Assert.Equal(20, ConfigShare.FindHumBindItemIndex("任意"));
    }

    [Fact]
    public void BindItemIndexRequiresUnbindNameToMatchRequestedName()
    {
        // 绑定表名与请求名不符 → 直接返回 -1（原文 859 行比较的就是 UnBindItem.sItemName）
        OpenHumGate();
        ConfigShareSeam.g_UnbindItemList.Add(Bind("绑定表名", TUnBindItemType.t_Book, 12));
        ConfigShareSeam.g_ItemArr[20] = Item("请求名", stdMode: 31, shape: 12);
        Assert.Equal(-1, ConfigShare.FindHumBindItemIndex("请求名"));
        Assert.Equal(20, ConfigShare.FindHumBindItemIndex("绑定表名"));
    }

    [Fact]
    public void HeroBindItemIndexExitsEarlyWhenHeroNil()
    {
        Setup();
        ConfigShareSeam.g_MyHero = null;
        ConfigShareSeam.g_UnbindItemList.Add(Bind("任意", TUnBindItemType.t_Book, 30));
        ConfigShareSeam.g_HeroItemArr[3] = Item("任意", stdMode: 31, shape: 30);
        Assert.Equal(-1, ConfigShare.FindHeroBindItemIndex("任意"));
    }

    [Fact]
    public void HeroBindItemIndexFindsOnHeroBag()
    {
        // 英雄侧同样带 `not (Shape in [0, 1, 15..51])`，故 shape 取 12
        Setup(heroBagCount: 40);
        ConfigShareSeam.g_UnbindItemList.Add(Bind("任意", TUnBindItemType.t_Book, 12));
        ConfigShareSeam.g_HeroItemArr[3] = Item("任意", stdMode: 31, shape: 12);
        Assert.Equal(3, ConfigShare.FindHeroBindItemIndex("任意"));
    }

    [Fact]
    public void HeroBindItemIndexRejectsExcludedShape()
    {
        Setup(heroBagCount: 40);
        ConfigShareSeam.g_UnbindItemList.Add(Bind("任意", TUnBindItemType.t_Book, 30));
        ConfigShareSeam.g_HeroItemArr[3] = Item("任意", stdMode: 31, shape: 30);
        Assert.Equal(-1, ConfigShare.FindHeroBindItemIndex("任意"));
    }

    [Fact]
    public void UnBindBookSecondPassCoversFirstSixSlots()
    {
        OpenHumGate();
        ConfigShareSeam.g_ItemArr[2] = Item("召唤骷髅", stdMode: 1, shape: 1);
        Assert.Equal(2, ConfigShare.FindHumUnBindBookItemIndex("召唤骷髅"));
    }

    // ============================ 组合入口（UnBind 优先，Bind 兜底） ============================

    [Fact]
    public void FindHumHPItemIndexPrefersUnBind()
    {
        OpenHumGate();
        // 绑定表里有一个形状 5 的 HP 药，包裹 7 格放了形状 5（绑定型）
        ConfigShareSeam.g_UnbindItemList.Add(Bind("太阳水", TUnBindItemType.t_HP, 5));
        ConfigShareSeam.g_ItemArr[7] = Item("太阳水", stdMode: 31, shape: 5);
        // UnBind 判据（StdMode=0）不满足 → 落到 Bind
        Assert.Equal(7, ConfigShare.FindHumHPItemIndex());

        // 再放一个真正"解包"的金创药在 8 格 → UnBind 先命中
        ConfigShareSeam.g_ItemArr[8] = Item("金创药", stdMode: 0, shape: 0, ac1: 5);
        Assert.Equal(8, ConfigShare.FindHumHPItemIndex());
    }

    [Fact]
    public void FindHumMPItemIndexPrefersUnBind()
    {
        OpenHumGate();
        ConfigShareSeam.g_UnbindItemList.Add(Bind("魔法药", TUnBindItemType.t_MP, 5));
        ConfigShareSeam.g_ItemArr[7] = Item("魔法药", stdMode: 31, shape: 5);
        Assert.Equal(7, ConfigShare.FindHumMPItemIndex());
    }

    [Fact]
    public void FindHumSpecialItemIndexPrefersUnBind()
    {
        OpenHumGate();
        ConfigShareSeam.g_UnbindItemList.Add(Bind("祝福油", TUnBindItemType.t_Special, 5));
        ConfigShareSeam.g_ItemArr[7] = Item("祝福油", stdMode: 31, shape: 5);
        Assert.Equal(7, ConfigShare.FindHumSpecialItemIndex());
    }

    [Fact]
    public void FindHeroFamiliesPreferUnBind()
    {
        Setup();
        ConfigShareSeam.g_UnbindItemList.Add(Bind("太阳水", TUnBindItemType.t_HP, 5));
        ConfigShareSeam.g_HeroItemArr[7] = Item("太阳水", stdMode: 31, shape: 5);
        Assert.Equal(7, ConfigShare.FindHeroHPItemIndex());
        Assert.Equal(7, ConfigShare.FindHeroHPItemIndex("太阳水"));
    }

    [Fact]
    public void FindHeroMPAndSpecialFamilies()
    {
        Setup();
        ConfigShareSeam.g_UnbindItemList.Add(Bind("魔法药", TUnBindItemType.t_MP, 5));
        ConfigShareSeam.g_HeroItemArr[7] = Item("魔法药", stdMode: 31, shape: 5);
        Assert.Equal(7, ConfigShare.FindHeroMPItemIndex());
        Assert.Equal(7, ConfigShare.FindHeroMPItemIndex("魔法药"));

        Setup();
        ConfigShareSeam.g_UnbindItemList.Add(Bind("祝福油", TUnBindItemType.t_Special, 5));
        ConfigShareSeam.g_HeroItemArr[7] = Item("祝福油", stdMode: 31, shape: 5);
        Assert.Equal(7, ConfigShare.FindHeroSpecialItemIndex());
        Assert.Equal(7, ConfigShare.FindHeroSpecialItemIndex("祝福油"));
    }

    [Fact]
    public void FindHumHPByNamePrefersUnBindThenBind()
    {
        OpenHumGate();
        // 只有绑定项：UnBind(名) 找不到 → Bind(名)
        ConfigShareSeam.g_UnbindItemList.Add(Bind("太阳水", TUnBindItemType.t_HP, 5));
        ConfigShareSeam.g_ItemArr[7] = Item("太阳水", stdMode: 31, shape: 5);
        Assert.Equal(7, ConfigShare.FindHumHPItemIndex("太阳水"));
    }

    [Fact]
    public void FindHumMPByNameDropsOnEqualNameFlag()
    {
        // **原文缺陷（照抄）**：ConfigShare.pas:924 调用 FindHumUnBindMPItemIndex(sItemName)，
        // 没把 OnEqualName 传下去；所以 OnEqualName=True 在 MP 组合入口上**无效**。
        OpenHumGate();
        ConfigShareSeam.g_ItemArr[9] = Item("魔法药", stdMode: 99, shape: 99, ac1: 0, mac1: 9);
        Assert.Equal(-1, ConfigShare.FindHumMPItemIndex("魔法药", true));   // 与 HP 的行为不同
        Assert.Equal(9, ConfigShare.FindHumHPItemIndex("魔法药", true));    // HP 会传下去
    }

    [Fact]
    public void FindHumSpecialByNamePassesOnEqualName()
    {
        OpenHumGate();
        ConfigShareSeam.g_ItemArr[9] = Item("祝福油", stdMode: 99, shape: 99, ac1: 0, mac1: 0);
        Assert.Equal(9, ConfigShare.FindHumSpecialItemIndex("祝福油", true));
        Assert.Equal(-1, ConfigShare.FindHumSpecialItemIndex("祝福油", false));
    }

    [Fact]
    public void AllNoArgFindersReturnMinusOneOnEmptyState()
    {
        Setup();
        ConfigShareSeam.g_UnbindItemList.Clear();
        Assert.Equal(-1, ConfigShare.FindBagItemName("x"));
        Assert.Equal(-1, ConfigShare.FindHeroBagItemName("x"));
        Assert.Equal(-1, ConfigShare.FindHumBindHPItemIndex());
        Assert.Equal(-1, ConfigShare.FindHumBindMPItemIndex());
        Assert.Equal(-1, ConfigShare.FindHumBindSpecialItemIndex());
        Assert.Equal(-1, ConfigShare.FindHumUnBindHPItemIndex());
        Assert.Equal(-1, ConfigShare.FindHumUnBindMPItemIndex());
        Assert.Equal(-1, ConfigShare.FindHumUnBindSpecialItemIndex());
        Assert.Equal(-1, ConfigShare.FindHumHPItemIndex());
        Assert.Equal(-1, ConfigShare.FindHumMPItemIndex());
        Assert.Equal(-1, ConfigShare.FindHumSpecialItemIndex());
        Assert.Equal(-1, ConfigShare.FindHumBookItemIndex("x"));
    }

    [Fact]
    public void RepeatedCallsAreIdempotent()
    {
        OpenHumGate();
        ConfigShareSeam.g_UnbindItemList.Add(Bind("太阳水", TUnBindItemType.t_HP, 5));
        ConfigShareSeam.g_ItemArr[10] = Item("太阳水", stdMode: 31, shape: 5);
        for (int i = 0; i < 3; i++)
            Assert.Equal(10, ConfigShare.FindHumBindHPItemIndex());
    }

    [Fact]
    public void ScanStopsAtMaxBagCountSoExtSlotsAreInvisible()
    {
        // GetMaxBagCount=46 → 下标 46/47（数组里存在，模拟扩展页未开启）不参与扫描
        OpenHumGate();
        ConfigShareSeam.g_UnbindItemList.Add(Bind("太阳水", TUnBindItemType.t_HP, 5));
        ConfigShareSeam.g_ItemArr[46] = Item("太阳水", stdMode: 31, shape: 5);
        Assert.Equal(-1, ConfigShare.FindHumBindHPItemIndex());

        ConfigShareSeam.g_ExtBagOpenItemCount = 1;
        Assert.Equal(46, ConfigShare.FindHumBindHPItemIndex());
    }
}

/// <summary>
/// 车道 lane-client-guiconfig 的全局状态复位/隔离工具。
/// 配置层依赖大量 Delphi 单元级全局，xunit 并行执行会互相污染，
/// 故所有 GameConfig 测试归入同一个 xunit Collection 串行执行，并在每个测试前后复位。
/// </summary>
internal static class GuiCfgTestEnv
{
    public static void Reset()
    {
        ConfigShareSeam.g_ItemArr = Array.Empty<TClientItemSeam>();
        ConfigShareSeam.g_HeroItemArr = Array.Empty<TClientItemSeam>();
        ConfigShareSeam.g_UnbindItemList = new List<TUnBindItem>();
        ConfigShareSeam.g_MySelf = null;
        ConfigShareSeam.g_MyHero = null;
        ConfigShareSeam.g_ExtBagOpenItemCount = 0;

        ConfigShareGlobal.g_sPlugServerName = "";
        ConfigShareGlobal.g_sPlugUserName = "";
        for (int i = TShortcutKeys.Low; i <= TShortcutKeys.High; i++)
            ConfigShareGlobal.g_ShortcutKeys[i] = default;

        SelfFilePathSeam.g_sSelfFilePath = "";
        NextDirectionSeam.GetNextDirection = (x1, y1, x2, y2) => 0;
        ChatBoardSeam.AddChatBoardString = (msg, color, backColor) => { };
        LastChat = null;

        LoadControlFromStreamSeam.LoadCompressedUIData = (name, ext) => null;
        LoadControlFromStreamSeam.LoadControlFromStream = (s, l, n) => 0;
        LoadControlFromStreamSeam.PatchLoadControlFromStream = (m, l, n) => { };
        LoadControlFromStreamSeam.FreeMemoryStream = ms => { };
        LoadControlCalls.Clear();

        ClientGlobalSeam.g_ClientVersion = TClientVersion.cvMirs;
        ClientGlobalSeam.g_ClientConfig = new TClientConfig();
        ClientGlobalSeam.ConfigClientConfigs = Array.Empty<bool>();
        ClientGlobalSeam.frmMainHandle = IntPtr.Zero;
        ClientGlobalSeam.g_nScreenWidth = 0;
        ClientGlobalSeam.g_nScreenHeight = 0;
        ClientGlobalSeam.g_boWindowMode = false;

        PlugInSeam.CreateJSYConfigDlg = () => new TJSYConfigDlg();
        // 集成方修正（台账 §47）：这里原本装的是**桩** `TStubGameConfigObject`，而它是**静态接缝**、装完**不还原**
        // ⇒ xUnit 同进程内泄漏给后续测试类，造成"同一二进制第一次全绿、第二次 3 条失败"（次序性假红/假绿）。
        // 正式实现已由车道 p10-client-mirconfig 接线（GameConfigDlgs.cs），故这里同样指向真实现。
        PlugInSeam.CreateMirConfigDlg = () => new GXX.Client.GUI.GameConfig.Mir.TMirConfigDlg();
    }

    /// <summary>供 Hint / 聊天栏断言使用。</summary>
    public static (string Msg, int Color, int BackColor)? LastChat;

    /// <summary>供 LoadControlFromStream 调用次序断言使用。</summary>
    public static readonly List<string> LoadControlCalls = new List<string>();

    // ================================================================================
    // MirsConfigDlg.g_Config 的快照/还原
    // --------------------------------------------------------------------------------
    // MirsConfigDlg.pas 的 g_Config 是**单元级全局**（Delphi initialization 段零初始化 +
    // 显式初值），测试之间会互相污染。这里在每个测试前后做深拷贝快照。
    // ================================================================================

    private static TMirsConfigDlg.TConfig _cfgSnap;
    private static bool _snapSound, _snapBgs, _snapRepeatBgs;

    private static bool _cfgSnapTaken;

    /// <summary>
    /// 只在**首个**测试实例上取一次 g_Config 的原始默认值快照。
    /// 原因：g_Config 是静态单例，测试会改它；若每个测试都重新快照，就会把"被改过的值"当成基准。
    /// </summary>
    public static void SnapshotMirsConfigOnce()
    {
        if (_cfgSnapTaken) return;
        _cfgSnapTaken = true;
        SnapshotMirsConfig();
    }

    /// <summary>把当前 g_Config 深拷贝保存（须在 <see cref="Reset"/> 之后、任何修改之前调用）。</summary>
    public static void SnapshotMirsConfig()
    {
        var s = TMirsConfigDlg.g_Config;
        var c = new TMirsConfigDlg.TConfig
        {
            nFilterMinExp = s.nFilterMinExp,
            nAutoUseMagicTime = s.nAutoUseMagicTime,
            dwAutoUseMagicTick = s.dwAutoUseMagicTick,
            boRenewSpecialIsAuto = s.boRenewSpecialIsAuto,
            nRenewSpecialPercent = s.nRenewSpecialPercent,
            nRenewSpecialTime = s.nRenewSpecialTime,
            boRenewBookIsAuto = s.boRenewBookIsAuto,
            nRenewBookPercent = s.nRenewBookPercent,
            nRenewBookTime = s.nRenewBookTime,
            nRenewBookNowBookIndex = s.nRenewBookNowBookIndex,
            sRenewBookNowBookItem = s.sRenewBookNowBookItem,
            nRenewHeroHPTime = s.nRenewHeroHPTime,
            nRenewHeroHPPercent = s.nRenewHeroHPPercent,
            nRenewHeroMPTime = s.nRenewHeroMPTime,
            nRenewHeroMPPercent = s.nRenewHeroMPPercent,
            boRenewHeroSpecialIsAuto = s.boRenewHeroSpecialIsAuto,
            nRenewHeroSpecialTime = s.nRenewHeroSpecialTime,
            nRenewHeroSpecialPercent = s.nRenewHeroSpecialPercent,
            boRenewHeroLogOutIsAuto = s.boRenewHeroLogOutIsAuto,
            nRenewHeroLogOutTime = s.nRenewHeroLogOutTime,
            nRenewHeroLogOutPercent = s.nRenewHeroLogOutPercent,
            boRenewCloseIsAuto = s.boRenewCloseIsAuto,
            nRenewCloseTime = s.nRenewCloseTime,
            nRenewClosePercent = s.nRenewClosePercent,
            MedicaMode = s.MedicaMode,
        };
        Mp(s.CheckHpIsAutos, c.CheckHpIsAutos);
        Mp(s.CheckHpPercents, c.CheckHpPercents);
        Mp(s.CheckHpValues, c.CheckHpValues);
        Mp(s.CheckHpCheckTimes, c.CheckHpCheckTimes);
        Mp(s.CheckHpCheckTicks, c.CheckHpCheckTicks);
        Mp(s.CheckHpUseTimes, c.CheckHpUseTimes);
        Mp(s.CheckHpUseTicks, c.CheckHpUseTicks);
        Mp(s.CheckMpIsAutos, c.CheckMpIsAutos);
        Mp(s.CheckMpPercents, c.CheckMpPercents);
        Mp(s.CheckMpValues, c.CheckMpValues);
        Mp(s.CheckMpCheckTimes, c.CheckMpCheckTimes);
        Mp(s.CheckMpCheckTicks, c.CheckMpCheckTicks);
        Mp(s.CheckMpUseTimes, c.CheckMpUseTimes);
        Mp(s.CheckMpUseTicks, c.CheckMpUseTicks);
        Mp(s.RenewHPIsAutos, c.RenewHPIsAutos);
        Mp(s.RenewHPPercents, c.RenewHPPercents);
        Mp(s.RenewHPTimes, c.RenewHPTimes);
        Mp(s.RenewHPTicks, c.RenewHPTicks);
        Mp(s.RenewMPIsAutos, c.RenewMPIsAutos);
        Mp(s.RenewMPPercents, c.RenewMPPercents);
        Mp(s.RenewMPTimes, c.RenewMPTimes);
        Mp(s.RenewMPTicks, c.RenewMPTicks);
        Mp(s.RenewSpecialHPIsAutos, c.RenewSpecialHPIsAutos);
        Mp(s.RenewSpecialHPPercents, c.RenewSpecialHPPercents);
        Mp(s.RenewSpecialHPTimes, c.RenewSpecialHPTimes);
        Mp(s.RenewSpecialHPTicks, c.RenewSpecialHPTicks);
        Mp(s.RenewSpecialMPIsAutos, c.RenewSpecialMPIsAutos);
        Mp(s.RenewSpecialMPPercents, c.RenewSpecialMPPercents);
        Mp(s.RenewSpecialMPTimes, c.RenewSpecialMPTimes);
        Mp(s.RenewSpecialMPTicks, c.RenewSpecialMPTicks);
        Mp(s.UseSuperMedicas, c.UseSuperMedicas);
        Mp(s.SuperMedicaItemNames, c.SuperMedicaItemNames);
        Mp(s.SuperMedicaUses, c.SuperMedicaUses);
        Mp(s.SuperMedicaHPs, c.SuperMedicaHPs);
        Mp(s.SuperMedicaHPTimes, c.SuperMedicaHPTimes);
        Mp(s.SuperMedicaHPTicks, c.SuperMedicaHPTicks);
        Mp(s.SuperMedicaMPs, c.SuperMedicaMPs);
        Mp(s.SuperMedicaMPTimes, c.SuperMedicaMPTimes);
        Mp(s.SuperMedicaMPTicks, c.SuperMedicaMPTicks);
        _cfgSnap = c;

        _snapSound = MirsConfigGlobalSeam.g_boSound;
        _snapBgs = MirsConfigGlobalSeam.g_boBGSound;
        _snapRepeatBgs = MirsConfigGlobalSeam.g_boRepeatBGSound;
    }

    /// <summary>把 <see cref="SnapshotMirsConfig"/> 保存的状态写回 g_Config。</summary>
    public static void RestoreMirsConfig()
    {
        if (_cfgSnap == null) return;
        var s = TMirsConfigDlg.g_Config;
        var c = _cfgSnap;
        s.nFilterMinExp = c.nFilterMinExp;
        s.nAutoUseMagicTime = c.nAutoUseMagicTime;
        s.dwAutoUseMagicTick = c.dwAutoUseMagicTick;
        s.boRenewSpecialIsAuto = c.boRenewSpecialIsAuto;
        s.nRenewSpecialPercent = c.nRenewSpecialPercent;
        s.nRenewSpecialTime = c.nRenewSpecialTime;
        s.boRenewBookIsAuto = c.boRenewBookIsAuto;
        s.nRenewBookPercent = c.nRenewBookPercent;
        s.nRenewBookTime = c.nRenewBookTime;
        s.nRenewBookNowBookIndex = c.nRenewBookNowBookIndex;
        s.sRenewBookNowBookItem = c.sRenewBookNowBookItem;
        s.nRenewHeroHPTime = c.nRenewHeroHPTime;
        s.nRenewHeroHPPercent = c.nRenewHeroHPPercent;
        s.nRenewHeroMPTime = c.nRenewHeroMPTime;
        s.nRenewHeroMPPercent = c.nRenewHeroMPPercent;
        s.boRenewHeroSpecialIsAuto = c.boRenewHeroSpecialIsAuto;
        s.nRenewHeroSpecialTime = c.nRenewHeroSpecialTime;
        s.nRenewHeroSpecialPercent = c.nRenewHeroSpecialPercent;
        s.boRenewHeroLogOutIsAuto = c.boRenewHeroLogOutIsAuto;
        s.nRenewHeroLogOutTime = c.nRenewHeroLogOutTime;
        s.nRenewHeroLogOutPercent = c.nRenewHeroLogOutPercent;
        s.boRenewCloseIsAuto = c.boRenewCloseIsAuto;
        s.nRenewCloseTime = c.nRenewCloseTime;
        s.nRenewClosePercent = c.nRenewClosePercent;
        s.MedicaMode = c.MedicaMode;
        Mp(c.CheckHpIsAutos, s.CheckHpIsAutos);
        Mp(c.CheckHpPercents, s.CheckHpPercents);
        Mp(c.CheckHpValues, s.CheckHpValues);
        Mp(c.CheckHpCheckTimes, s.CheckHpCheckTimes);
        Mp(c.CheckHpCheckTicks, s.CheckHpCheckTicks);
        Mp(c.CheckHpUseTimes, s.CheckHpUseTimes);
        Mp(c.CheckHpUseTicks, s.CheckHpUseTicks);
        Mp(c.CheckMpIsAutos, s.CheckMpIsAutos);
        Mp(c.CheckMpPercents, s.CheckMpPercents);
        Mp(c.CheckMpValues, s.CheckMpValues);
        Mp(c.CheckMpCheckTimes, s.CheckMpCheckTimes);
        Mp(c.CheckMpCheckTicks, s.CheckMpCheckTicks);
        Mp(c.CheckMpUseTimes, s.CheckMpUseTimes);
        Mp(c.CheckMpUseTicks, s.CheckMpUseTicks);
        Mp(c.RenewHPIsAutos, s.RenewHPIsAutos);
        Mp(c.RenewHPPercents, s.RenewHPPercents);
        Mp(c.RenewHPTimes, s.RenewHPTimes);
        Mp(c.RenewHPTicks, s.RenewHPTicks);
        Mp(c.RenewMPIsAutos, s.RenewMPIsAutos);
        Mp(c.RenewMPPercents, s.RenewMPPercents);
        Mp(c.RenewMPTimes, s.RenewMPTimes);
        Mp(c.RenewMPTicks, s.RenewMPTicks);
        Mp(c.RenewSpecialHPIsAutos, s.RenewSpecialHPIsAutos);
        Mp(c.RenewSpecialHPPercents, s.RenewSpecialHPPercents);
        Mp(c.RenewSpecialHPTimes, s.RenewSpecialHPTimes);
        Mp(c.RenewSpecialHPTicks, s.RenewSpecialHPTicks);
        Mp(c.RenewSpecialMPIsAutos, s.RenewSpecialMPIsAutos);
        Mp(c.RenewSpecialMPPercents, s.RenewSpecialMPPercents);
        Mp(c.RenewSpecialMPTimes, s.RenewSpecialMPTimes);
        Mp(c.RenewSpecialMPTicks, s.RenewSpecialMPTicks);
        Mp(c.UseSuperMedicas, s.UseSuperMedicas);
        Mp(c.SuperMedicaItemNames, s.SuperMedicaItemNames);
        Mp(c.SuperMedicaUses, s.SuperMedicaUses);
        Mp(c.SuperMedicaHPs, s.SuperMedicaHPs);
        Mp(c.SuperMedicaHPTimes, s.SuperMedicaHPTimes);
        Mp(c.SuperMedicaHPTicks, s.SuperMedicaHPTicks);
        Mp(c.SuperMedicaMPs, s.SuperMedicaMPs);
        Mp(c.SuperMedicaMPTimes, s.SuperMedicaMPTimes);
        Mp(c.SuperMedicaMPTicks, s.SuperMedicaMPTicks);
        // 注意：_cfgSnap **不置 null** —— 它是"原始默认值"的长期基准，每个测试都要能还原到它。

        MirsConfigGlobalSeam.g_boSound = _snapSound;
        MirsConfigGlobalSeam.g_boBGSound = _snapBgs;
        MirsConfigGlobalSeam.g_boRepeatBGSound = _snapRepeatBgs;
    }

    private static void Mp(bool[] from, bool[] to) { for (int i = 0; i < Math.Min(from.Length, to.Length); i++) to[i] = from[i]; }
    private static void Mp(int[] from, int[] to) { for (int i = 0; i < Math.Min(from.Length, to.Length); i++) to[i] = from[i]; }
    private static void Mp(uint[] from, uint[] to) { for (int i = 0; i < Math.Min(from.Length, to.Length); i++) to[i] = from[i]; }
    private static void Mp(string[] from, string[] to) { for (int i = 0; i < Math.Min(from.Length, to.Length); i++) to[i] = from[i]; }
    private static void Mp(bool[,] from, bool[,] to)
    {
        for (int i = 0; i < from.GetLength(0) && i < to.GetLength(0); i++)
            for (int j = 0; j < from.GetLength(1) && j < to.GetLength(1); j++) to[i, j] = from[i, j];
    }
    private static void Mp(int[,] from, int[,] to)
    {
        for (int i = 0; i < from.GetLength(0) && i < to.GetLength(0); i++)
            for (int j = 0; j < from.GetLength(1) && j < to.GetLength(1); j++) to[i, j] = from[i, j];
    }
}
[CollectionDefinition("GuiCfgConfigure", DisableParallelization = true)]
public sealed class GuiCfgConfigureCollection { }
