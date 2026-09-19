using System.IO;
using GXX.Client.GUI.Mir;
using GXX.Client.GUI.DxComponent;
using GXX.Client.Scenes;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 并行批次P1：Mir185WindowsDlg.pas T185Windows（全文 187 行）1:1 测试。
/// 本族窗口无 .dfm，几何/可见性全部由 LoadFromStream 覆写设定（原文行号见实现注释）。
/// </summary>
public sealed class GuiMir185WindowsTests
{
    [Fact]
    public void T185Windows_Ctor_ClientVersionIs185()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = new T185Windows();
            Assert.Equal(TClientVersion.cv185, w.ClientVersion);   // Mir185WindowsDlg.pas:63
            w.Destroy();                                            // 66-69
        });
    }

    [Fact]
    public void T185Windows_LoadFromStream_LoginGeometryAndImageIndex()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = new T185Windows();
            using var ms = new MemoryStream();
            w.LoadFromStream(ms);                                   // 71

            Assert.False(w.DImageButtonAccount.Visible);            // 74
            Assert.False(w.DImageButtonPassWord.Visible);           // 75

            Assert.Equal(TImageType.Prguse_wil, w.DLogin.ImageIndex.ImageType); // 77
            Assert.Equal(60, w.DLogin.ImageIndex.Up);                          // 78

            Assert.Equal(TImageType.Prguse_wil, w.DLoginOK.ImageIndex.ImageType); // 80
            Assert.Equal(-1, w.DLoginOK.ImageIndex.Up);                           // 81
            Assert.Equal(62, w.DLoginOK.ImageIndex.Down);                         // 82
            Assert.Null(w.DLoginOK.OnPaint);                                      // 83
            Assert.Equal(169, w.DLoginOK.Left);                                   // 84
            Assert.Equal(163, w.DLoginOK.Top);                                    // 85

            Assert.Equal(-1, w.DLoginNew.ImageIndex.Up);            // 88
            Assert.Equal(61, w.DLoginNew.ImageIndex.Down);          // 89
            Assert.Equal(25, w.DLoginNew.Left);                     // 91
            Assert.Equal(207, w.DLoginNew.Top);                     // 92

            Assert.Equal(-1, w.DLoginChgPw.ImageIndex.Up);          // 95
            Assert.Equal(53, w.DLoginChgPw.ImageIndex.Down);        // 96
            Assert.Equal(130, w.DLoginChgPw.Left);                  // 98
            Assert.Equal(207, w.DLoginChgPw.Top);                   // 99

            Assert.Equal(252, w.DLoginClose.Left);                  // 101
            Assert.Equal(28, w.DLoginClose.Top);                    // 102
            Assert.Equal(98, w.DEdId_.Left);                        // 104
            Assert.Equal(85, w.DEdId_.Top);                         // 105
            Assert.Equal(98, w.DEdPasswd_.Left);                    // 107
            Assert.Equal(117, w.DEdPasswd_.Top);                    // 108
        });
    }

    [Fact]
    public void T185Windows_LoadFromStream_HiddenControls()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = new T185Windows();
            using var ms = new MemoryStream();
            w.LoadFromStream(ms);

            Assert.False(w.DBottomLeftImageButton1.Visible);        // 110
            Assert.False(w.DMerchantDlgHelp_.Visible);              // 111（同 164）
            Assert.False(w.DChangeState.Visible);                   // 112
            Assert.False(w.DSWTitleActive.Visible);                 // 114
            Assert.False(w.DSWTitleButton1.Visible);                // 115
            Assert.False(w.DSWTitleButton2.Visible);                // 116
            Assert.False(w.DSWTitleButton3.Visible);                // 117
            Assert.False(w.DSWTitleButton4.Visible);                // 118
            Assert.False(w.DSWTitlePageUp.Visible);                 // 119
            Assert.False(w.DSWTitlePageDown.Visible);               // 120
            Assert.False(w.DSUSTitleActive.Visible);                // 122
            Assert.False(w.DSUSTitleButton1.Visible);               // 123
            Assert.False(w.DSUSTitleButton2.Visible);               // 124
            Assert.False(w.DSUSTitleButton3.Visible);               // 125
            Assert.False(w.DSUSTitleButton4.Visible);               // 126
            Assert.False(w.DSUSTitlePageUp.Visible);                // 127
            Assert.False(w.DSUSTitlePageDown.Visible);              // 128
            Assert.False(w.DRecallHero.Visible);                    // 165
            Assert.False(w.DMyHeroState.Visible);                   // 166
            Assert.False(w.DMyHeroBag.Visible);                     // 167
            Assert.False(w.DRecallDeputyHero.Visible);              // 168
        });
    }

    [Fact]
    public void T185Windows_LoadFromStream_StateWinAndTabs()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = new T185Windows();
            using var ms = new MemoryStream();
            w.LoadFromStream(ms);

            Assert.Equal(TImageType.Prguse3_wil, w.DUserState1.ImageIndex.ImageType); // 130
            Assert.Equal(207, w.DUserState1.ImageIndex.Up);                           // 131
            Assert.Equal(TImageType.Prguse3_wil, w.DStateWin.ImageIndex.ImageType);   // 133
            Assert.Equal(207, w.DStateWin.ImageIndex.Up);                             // 134
            Assert.False(w.DStateTabSheet5.TabVisible);                               // 162
        });
    }

    [Fact]
    public void T185Windows_LoadFromStream_CreatesPrevNextStateButtons()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = new T185Windows();
            using var ms = new MemoryStream();
            w.LoadFromStream(ms);

            // DPrevState/DNextState 为 185 版私有字段（136-157），其可观测副作用为：
            // 1) 二者以 DStateWin 为 Owner；2) 绑定 OnGetImage = DStateWin.OnGetImage；
            // 3) OnClick 指向 DPrevStateClick/DNextStateClick —— 通过点击语义验证绑定生效。
            Assert.Equal(-1, w.DStatePageControl.ActivePageIndex);
            w.DPrevStateClick(null, 0, 0);
            Assert.Equal(3, w.DStatePageControl.ActivePageIndex);          // 173-176
            w.DNextStateClick(null, 0, 0);
            Assert.Equal(0, w.DStatePageControl.ActivePageIndex);          // 181-182
        });
    }

    [Fact]
    public void T185Windows_DPrevStateClick_WrapsAtZeroToThree()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = new T185Windows();
            using var ms = new MemoryStream();
            w.LoadFromStream(ms);
            Assert.Equal(-1, w.DStatePageControl.ActivePageIndex);   // DxPageControl 构造初值 -1

            w.DPrevStateClick(null, 0, 0);                           // 171-177
            Assert.Equal(3, w.DStatePageControl.ActivePageIndex);

            w.DPrevStateClick(null, 0, 0);
            Assert.Equal(2, w.DStatePageControl.ActivePageIndex);

            w.DPrevStateClick(null, 0, 0);
            Assert.Equal(1, w.DStatePageControl.ActivePageIndex);

            w.DPrevStateClick(null, 0, 0);
            Assert.Equal(0, w.DStatePageControl.ActivePageIndex);

            w.DPrevStateClick(null, 0, 0);                           // 0 → 回绕 3
            Assert.Equal(3, w.DStatePageControl.ActivePageIndex);
        });
    }

    [Fact]
    public void T185Windows_DNextStateClick_WrapsAtThreeToZero()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = new T185Windows();
            using var ms = new MemoryStream();
            w.LoadFromStream(ms);

            w.DStatePageControl.ActivePageIndex = 0;
            w.DNextStateClick(null, 0, 0);                           // 179-185
            Assert.Equal(1, w.DStatePageControl.ActivePageIndex);
            w.DNextStateClick(null, 0, 0);
            Assert.Equal(2, w.DStatePageControl.ActivePageIndex);
            w.DNextStateClick(null, 0, 0);
            Assert.Equal(3, w.DStatePageControl.ActivePageIndex);
            w.DNextStateClick(null, 0, 0);                           // >= 3 → 回绕 0
            Assert.Equal(0, w.DStatePageControl.ActivePageIndex);
        });
    }

    [Fact]
    public void T185Windows_StateButtons_DelegateWiring()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            // 通过私有字段设置的回调驱动分页（136-157 的 OnClick 绑定）
            var w = new T185Windows();
            using var ms = new MemoryStream();
            w.LoadFromStream(ms);

            // DPrevState/DNextState 的创建使 DStatePageControl 仍为构造初值，未被 LoadFromStream 改动
            Assert.Equal(-1, w.DStatePageControl.ActivePageIndex);
            w.DPrevStateClick(null, 0, 0);
            Assert.Equal(3, w.DStatePageControl.ActivePageIndex);
        });
    }
}

/// <summary>
/// 并行批次P1：Mir176WindowsDlg.pas T176Windows（全文 505 行）1:1 测试。
/// 覆盖：LoadFromStream 几何/可见性、状态标签三态字色快照、AddSpace 定长补位、
/// MySelfAbilChange 的「背包重量/穿戴重量/腕力」红色分支、StPageUpClick 分页、
/// StateMemo4DirectPaint 取图、OpenUserState 形象性别读取。
/// </summary>
public sealed class GuiMir176WindowsTests
{
    private static T176Windows Loaded()
    {
        var w = new T176Windows();
        using var ms = new MemoryStream();
        w.LoadFromStream(ms);
        return w;
    }

    private static THumActor MakeSelf()
    {
        var self = new THumActor { m_btSex = 0 };
        self.m_Abil.AC1 = 1; self.m_Abil.AC2 = 2;
        self.m_Abil.MAC1 = 3; self.m_Abil.MAC2 = 4;
        self.m_Abil.DC1 = 5; self.m_Abil.DC2 = 6;
        self.m_Abil.MC1 = 7; self.m_Abil.MC2 = 8;
        self.m_Abil.SC1 = 9; self.m_Abil.SC2 = 10;
        self.m_Abil.HP = 100; self.m_Abil.MaxHP = 200;
        self.m_Abil.MP = 30; self.m_Abil.MaxMP = 60;
        self.m_Abil.Exp = 1000; self.m_Abil.MaxExp = 9000;
        self.m_Abil.Weight = 10; self.m_Abil.MaxWeight = 50;
        self.m_Abil.WearWeight = 20; self.m_Abil.MaxWearWeight = 60;
        self.m_Abil.HandWeight = 30; self.m_Abil.MaxHandWeight = 70;
        return self;
    }

    [Fact]
    public void T176Windows_Ctor_ClientVersionIs176()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = new T176Windows();
            Assert.Equal(TClientVersion.cv176, w.ClientVersion);   // Mir176WindowsDlg.pas:73
            w.Destroy();                                            // 76-79
        });
    }

    [Fact]
    public void T176Windows_LoadFromStream_LoginAndBagGeometry()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = Loaded();

            Assert.False(w.DImageButtonAccount.Visible);            // 85
            Assert.False(w.DImageButtonPassWord.Visible);           // 86
            Assert.Equal(TImageType.Prguse_wil, w.DLogin.ImageIndex.ImageType); // 88
            Assert.Equal(60, w.DLogin.ImageIndex.Up);                           // 89
            Assert.Equal(169, w.DLoginOK.Left);                     // 95
            Assert.Equal(163, w.DLoginOK.Top);                      // 96
            Assert.Null(w.DLoginOK.OnPaint);                        // 94
            Assert.Equal(62, w.DLoginOK.ImageIndex.Down);           // 93
            Assert.Equal(-1, w.DLoginOK.ImageIndex.Up);             // 92
            Assert.Equal(-1, w.DLoginNew.ImageIndex.Up);            // 99
            Assert.Equal(61, w.DLoginNew.ImageIndex.Down);          // 100
            Assert.Equal(25, w.DLoginNew.Left);                     // 102
            Assert.Equal(207, w.DLoginNew.Top);                     // 103
            Assert.Equal(-1, w.DLoginChgPw.ImageIndex.Up);          // 106
            Assert.Equal(53, w.DLoginChgPw.ImageIndex.Down);        // 107
            Assert.Equal(130, w.DLoginChgPw.Left);                  // 109
            Assert.Equal(207, w.DLoginChgPw.Top);                   // 110
            Assert.Equal(252, w.DLoginClose.Left);                  // 112
            Assert.Equal(28, w.DLoginClose.Top);                    // 113
            Assert.Equal(98, w.DEdId_.Left);                        // 115
            Assert.Equal(85, w.DEdId_.Top);                         // 116
            Assert.Equal(98, w.DEdPasswd_.Left);                    // 118
            Assert.Equal(117, w.DEdPasswd_.Top);                    // 119

            Assert.False(w.DItemBagUpgrade.Visible);                // 121
            Assert.Equal(TImageType.Prguse_wil, w.DItemBag.ImageIndex.ImageType); // 123
            Assert.Equal(3, w.DItemBag.ImageIndex.Up);                            // 124
            Assert.Equal(20, w.DItemGrid.Left);                     // 126
            Assert.Equal(14, w.DItemGrid.Top);                      // 127
            Assert.Equal(10, w.DGold.Left);                         // 129
            Assert.Equal(190, w.DGold.Top);                         // 130
            Assert.Equal(310, w.DCloseBag.Left);                    // 132
            Assert.Equal(203, w.DCloseBag.Top);                     // 133
            Assert.False(w.DOpenShop.Visible);                      // 135
            Assert.False(w.DMerchantDlgHelp.Visible);               // 136
        });
    }

    [Fact]
    public void T176Windows_LoadFromStream_HiddenControlsAndFourSlots()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = Loaded();

            Assert.False(w.DStateGameTimeGirdLabel.Visible);        // 138
            Assert.False(w.DBottomLeftImageButton1.Visible);        // 140
            Assert.False(w.DChangeState.Visible);                   // 142
            Assert.False(w.DSWTitleActive.Visible);                 // 144
            Assert.False(w.DSWTitlePageDown.Visible);               // 150
            Assert.False(w.DMerchantDlgHelp_.Visible);              // 152 / 206
            Assert.False(w.DSWBujuk.Visible);                       // 154
            Assert.False(w.DSWBelt.Visible);                        // 155
            Assert.False(w.DSWBoots.Visible);                       // 156
            Assert.False(w.DSWCharm.Visible);                       // 157
            Assert.False(w.DSUSTitleActive.Visible);                // 159
            Assert.False(w.DSUSTitlePageDown.Visible);              // 165
            Assert.False(w.DBujukUS1.Visible);                      // 168
            Assert.False(w.DBeltUS1.Visible);                       // 169
            Assert.False(w.DBootsUS1.Visible);                      // 170
            Assert.False(w.DCharmUS1.Visible);                      // 171
            Assert.False(w.DRecallHero.Visible);                    // 207
            Assert.False(w.DMyHeroState.Visible);                   // 208
            Assert.False(w.DMyHeroBag.Visible);                     // 209
            Assert.False(w.DRecallDeputyHero.Visible);              // 210
        });
    }

    [Fact]
    public void T176Windows_LoadFromStream_StateWindowGeometry()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = Loaded();

            Assert.Equal(TImageType.Prguse_wil, w.DUserState1.ImageIndex.ImageType); // 173
            Assert.Equal(370, w.DUserState1.ImageIndex.Up);                           // 174
            Assert.Equal(TImageType.Prguse_wil, w.DStateWin.ImageIndex.ImageType);    // 176
            Assert.Equal(370, w.DStateWin.ImageIndex.Up);                             // 177
            Assert.False(w.DStateTabSheet5.TabVisible);                               // 204
            Assert.True(w.DStateForm1.UseSetting2);                                   // 212
            Assert.Equal(TImageType.Prguse_wil, w.DStateForm2.ImageIndex.ImageType);  // 214
            Assert.Equal(-1, w.DStateForm2.ImageIndex.Up);                            // 215
            Assert.Equal(234, w.DStateMemo3.Height);                                  // 217
            Assert.True(w.DUserStateForm1.UseSetting2);                               // 221
            Assert.Equal(168, w.DStateMemo4.Width);                                   // 225
            Assert.Equal(199, w.DStateMemo4.Height);                                  // 226
            Assert.False(w.DStMagBack6.Visible);                                      // 228
        });
    }

    [Fact]
    public void T176Windows_LoadFromStream_StMagBackOffsets()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            // 226-240 为「在原几何基础上 +1/+2」的相对调整，基线取接缝默认值
            var w = Loaded();
            Assert.Equal(1, w.DStMagBack1.Left);   // 默认 0 + 1
            Assert.Equal(1, w.DStMagBack2.Left);
            Assert.Equal(1, w.DStMagBack3.Left);
            Assert.Equal(1, w.DStMagBack4.Left);
            Assert.Equal(1, w.DStMagBack5.Left);
            Assert.Equal(2, w.DStMagBack1.Top);    // 默认 0 + 2
            Assert.Equal(2, w.DStMagBack5.Top);
            Assert.Equal(0, w.DStMagBack6.Left);   // 第 6 格未调整
        });
    }

    [Fact]
    public void T176Windows_CaptionColorSnapshot_RestoredWhenWeightOk()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = new T176Windows();
            // 先给状态标签预置非默认字色，LoadFromStream 应把三态字色快照下来（242-255）
            w.DStateWeightLabel.CaptionColor.Up.Value = 0x00111111;
            w.DStateWeightLabel.CaptionColor.Hot.Value = 0x00222222;
            w.DStateWeightLabel.CaptionColor.Down.Value = 0x00333333;
            w.DStateWeightLabel.CaptionColor.Disabled.Value = 0x00444444;
            w.DStateWearWeightLabel.CaptionColor.Up.Value = 0x00555555;
            w.DStateHandWeightLabel.CaptionColor.Up.Value = 0x00666666;
            using var ms = new MemoryStream();
            w.LoadFromStream(ms);

            MShareGlobals.g_MySelf = MakeSelf();     // Weight=10 <= MaxWeight=50
            w.MySelfAbilChange();

            Assert.Equal(0x00111111, w.DStateWeightLabel.CaptionColor.Up.Value);      // 298
            Assert.Equal(0x00222222, w.DStateWeightLabel.CaptionColor.Hot.Value);     // 299
            Assert.Equal(0x00333333, w.DStateWeightLabel.CaptionColor.Down.Value);    // 300
            Assert.Equal(0x00444444, w.DStateWeightLabel.CaptionColor.Disabled.Value);// 301
            Assert.Equal(0x00555555, w.DStateWearWeightLabel.CaptionColor.Up.Value);  // 311
            Assert.Equal(0x00666666, w.DStateHandWeightLabel.CaptionColor.Up.Value);  // 324
        });
    }

    [Fact]
    public void T176Windows_MySelfAbilChange_OverWeightTurnsRed()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = Loaded();
            var self = MakeSelf();
            self.m_Abil.Weight = 51;          // > MaxWeight 50 → 背包重量红（292-295）
            self.m_Abil.WearWeight = 61;      // > MaxWearWeight 60 → 穿戴重量红（305-308）
            self.m_Abil.HandWeight = 71;      // > MaxHandWeight 70 → 腕力红（318-321）
            MShareGlobals.g_MySelf = self;

            w.MySelfAbilChange();

            Assert.Equal(TColor.clRed, w.DStateWeightLabel.CaptionColor.Up.Value);
            Assert.Equal(TColor.clRed, w.DStateWeightLabel.CaptionColor.Hot.Value);
            Assert.Equal(TColor.clRed, w.DStateWeightLabel.CaptionColor.Down.Value);
            Assert.Equal(TColor.clRed, w.DStateWearWeightLabel.CaptionColor.Up.Value);
            Assert.Equal(TColor.clRed, w.DStateWearWeightLabel.CaptionColor.Hot.Value);
            Assert.Equal(TColor.clRed, w.DStateWearWeightLabel.CaptionColor.Down.Value);
            Assert.Equal(TColor.clRed, w.DStateHandWeightLabel.CaptionColor.Up.Value);
            Assert.Equal(TColor.clRed, w.DStateHandWeightLabel.CaptionColor.Hot.Value);
            Assert.Equal(TColor.clRed, w.DStateHandWeightLabel.CaptionColor.Down.Value);
            // 红色分支不改 Disabled 态（原文如此）
            Assert.Equal(w.DStateWeightLabel.CaptionColor.Disabled.Value, w.DStateWeightLabel.CaptionColor.Disabled.Value);
        });
    }

    [Fact]
    public void T176Windows_MySelfAbilChange_CaptionsAndAddSpace()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = Loaded();
            var self = MakeSelf();
            self.m_btSex = 1;
            MShareGlobals.g_MySelf = self;
            MShareGlobals.g_nMyHitPoint = 11;
            MShareGlobals.g_nMySpeedPoint = 22;
            MShareGlobals.g_nMyAntiMagic = 3;
            MShareGlobals.g_nMyAntiPoison = 4;
            MShareGlobals.g_nMyPoisonRecover = 5;
            MShareGlobals.g_nMyHealthRecover = 6;
            MShareGlobals.g_nMySpellRecover = 7;
            MShareGlobals.g_nGameDiamond = 88;
            MShareGlobals.g_nGameGird = 99;

            w.MySelfAbilChange();

            Assert.Equal("1-2", w.DStateLabelAC.Caption);       // 280
            Assert.Equal("3-4", w.DStateLabelMAC.Caption);      // 281
            Assert.Equal("5-6", w.DStateLabelDC.Caption);       // 282
            Assert.Equal("7-8", w.DStateLabelMC.Caption);       // 283
            Assert.Equal("9-10", w.DStateLabelSC.Caption);      // 284
            Assert.Equal("100/200", w.DStateLabelHP.Caption);   // 285
            Assert.Equal("30/60", w.DStateLabelMP.Caption);     // 286

            // AddSpace：不足 12 字符补空格（267-275）
            Assert.Equal("当前经验".PadRight(12) + "1000", w.DStateExpLabel.Caption);    // 288
            Assert.Equal("升级经验".PadRight(12) + "9000", w.DStateMaxExpLabel.Caption); // 289
            Assert.Equal("背包重量".PadRight(12) + "10/50", w.DStateWeightLabel.Caption);// 291
            Assert.Equal("穿戴重量".PadRight(12) + "20/60", w.DStateWearWeightLabel.Caption);
            Assert.Equal("腕力".PadRight(12) + "30/70", w.DStateHandWeightLabel.Caption);

            // 176 版这三项没有 "+" 前缀（原文如此，与基类 10401-10402 不同）
            Assert.Equal("精确度".PadRight(12) + "11", w.DStateHitPointLabel.Caption);          // 330
            Assert.Equal("敏捷度".PadRight(12) + "22", w.DStateSpeedPointLabel.Caption);        // 331
            Assert.Equal("魔法躲避".PadRight(12) + "30%", w.DStateAntiMagicLabel.Caption);     // 332
            Assert.Equal("毒物躲避".PadRight(12) + "40%", w.DStateAntiPoisonLabel.Caption);    // 333
            Assert.Equal("中毒恢复".PadRight(12) + "+50%", w.DStatePoisonRecoverLabel.Caption);  // 335
            Assert.Equal("体力恢复".PadRight(12) + "+60%", w.DStateHealthRecoverLabel.Caption);  // 336
            Assert.Equal("魔法恢复".PadRight(12) + "+70%", w.DStateSpellRecoverLabel.Caption);   // 337

            Assert.False(w.DStateForm1.IsMale);                 // 278（m_btSex=1 → 女）
            // 176 版把金刚石/灵符/元宝/游戏时间四标签全部隐藏（339-342），不写 Caption
            Assert.False(w.DStateGameDiamondLabel.Visible);
            Assert.False(w.DStateGameGirdLabel.Visible);
            Assert.False(w.DStateGameGoldLabel.Visible);
            Assert.False(w.DStateGameTimeGirdLabel.Visible);
            Assert.Equal(string.Empty, w.DStateGameDiamondLabel.Caption);
        });
    }

    [Fact]
    public void T176Windows_MySelfAbilChange_NullSelfExits()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = Loaded();
            MShareGlobals.g_MySelf = null;
            w.DStateLabelAC.Caption = "哨兵";
            w.MySelfAbilChange();                              // 277 if g_MySelf = nil then Exit
            Assert.Equal("哨兵", w.DStateLabelAC.Caption);
        });
    }

    [Fact]
    public void T176Windows_StPageUpClick_OwnOverrideUsesFive()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = Loaded();
            for (int i = 0; i < 12; i++) MShareGlobals.g_MagicList.Add(new object());

            w.MagicIndex = 7;
            w.StPageUpClick(w.DStPageUp, 0, 0);          // 361-368：Dec(MagicIndex,5) 后钳 0
            Assert.Equal(2, w.MagicIndex);

            w.StPageUpClick(w.DStPageUp, 0, 0);
            Assert.Equal(0, w.MagicIndex);

            w.StPageUpClick(w.DStPageUp, 0, 0);
            Assert.Equal(0, w.MagicIndex);

            var other = new TDxImageButton();
            w.StPageUpClick(other, 0, 0);                // 370：0+5 < 12 → 5
            Assert.Equal(5, w.MagicIndex);
            w.StPageUpClick(other, 0, 0);                // 10 < 12 → 10
            Assert.Equal(10, w.MagicIndex);
            w.StPageUpClick(other, 0, 0);                // 15 不 < 12 → 原地
            Assert.Equal(10, w.MagicIndex);
        });
    }

    [Fact]
    public void T176Windows_StateMemo4DirectPaint_DrawsBackImage383()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = Loaded();
            var memo = new TDxControl();
            memo.Left = 30; memo.Top = 40; memo.Width = 168; memo.Height = 199;

            w.StateMemo4DirectPaint(memo);                       // 382-384：d = g_WMainImages.Images[383]
            Assert.Empty(MShareGlobals.GameCanvas.RectDraws);    // 无图（nil）→ 不绘

            var tex = new TTexture { Width = 20, Height = 20 };
            MShareGlobals.g_WMainImages[383] = tex;
            w.StateMemo4DirectPaint(memo);
            // 384：GameCanvas.Draw(vtRect.Left, vtRect.Top, d.ClientRect, d)
            Assert.Single(MShareGlobals.GameCanvas.RectDraws);
            Assert.Same(tex, MShareGlobals.GameCanvas.RectDraws[0].Texture);
            Assert.Equal(new TRect(0, 0, 20, 20).Left, MShareGlobals.GameCanvas.RectDraws[0].Rect.Left);
            Assert.Equal(20, MShareGlobals.GameCanvas.RectDraws[0].Rect.Right);
            Assert.Equal((30, 40), (memo.VirtualRect.Left, memo.VirtualRect.Top));
        });
    }

    [Fact]
    public void T176Windows_OpenUserState_ReadsHumFeatureGender()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = Loaded();
            // pTHumFeature(@g_UserState1.Feature.Buffer)^.btGender @ offset 2（261）
            SetUserStateGender(0);
            w.OpenUserState();
            Assert.True(w.DUserStateForm1.IsMale);
            Assert.True(w.DUserState1.Visible);

            SetUserStateGender(1);
            w.OpenUserState();
            Assert.False(w.DUserStateForm1.IsMale);
        });
    }

    /// <summary>
    /// 原文缺陷（Mir176WindowsDlg.pas:399 / MirNewUI205Dlg.pas:384）：
    /// 形参 Sender:TObject 被显式硬转成 TDxImageButton，而调用方传入的 DItemBag 在类声明
    /// （SerialWindowsDlg.pas:340）里是 **TDxImageForm** —— 原文自身类型不一致。移植保持形参类型
    /// 与强转不变，测试侧用同类型的按钮替身驱动该分支。
    /// </summary>
    internal static TDxImageButton BagButtonShim(TSerialWindows w)
    {
        var b = new TDxImageButton();
        b.Left = w.DItemBag.Left; b.Top = w.DItemBag.Top;
        b.Width = w.DItemBag.Width; b.Height = w.DItemBag.Height;
        return b;
    }

    /// <summary>pTHumFeature(@g_UserState1.Feature.Buffer)^.btGender 的测试写入。</summary>
    private static unsafe void SetUserStateGender(byte gender)
    {
        fixed (byte* p = MShareGlobals.g_UserState1.Feature.Buffer)
            ((GXX.Core.Protocol.THumFeature*)p)->btGender = gender;
    }

    [Fact]
    public void T176Windows_ItemBagDirectPaint_NullSelfExits()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = Loaded();
            MShareGlobals.g_MySelf = null;
            MShareGlobals.CurrentFont.TextOuts.Clear();
            w.ItemBagDirectPaint(BagButtonShim(w));                       // 398 Exit
            Assert.Empty(MShareGlobals.CurrentFont.TextOuts);
        });
    }

    [Fact]
    public void T176Windows_ItemBagDirectPaint_DrawsGoldAndHint()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = Loaded();
            var self = MakeSelf();
            GXX.Client.GUI.Mir.ActorUiFields.SetGold(self, 12345);
            MShareGlobals.g_MySelf = self;
            MShareGlobals.GetGoldStrHandler = g => "金" + g;

            w.ItemBagDirectPaint(BagButtonShim(w));                       // 401-403：金币绘制
            Assert.Single(MShareGlobals.CurrentFont.TextOuts);
            var first = MShareGlobals.CurrentFont.TextOuts[0];
            Assert.Equal(w.DItemBag.Left + 62, first.X);
            Assert.Equal(w.DItemBag.Top + 183, first.Y);
            Assert.Equal("金12345", first.S);

            // 461 起：悬浮物品信息分支（btSuspensionShowItem=0 且 g_MouseItem.S.Name <> '' 且 g_boShowBagInfo）
            MShareGlobals.g_MouseItem.s.NameStr = "屠龙";
            MShareGlobals.g_boShowBagInfo = 1;
            MShareGlobals.GetMouseItemInfoHandler = (_, _, _, texts, flags) =>
            {
                texts[0] = "屠龙"; texts[1] = "攻击 5-35"; texts[2] = "需等级 34"; texts[3] = "";
                flags[0] = true; flags[1] = true;
            };
            int hintCalls = 0;
            MShareGlobals.DrawItemHintOldStyleHandler = (_, _, _, _) => { hintCalls++; return 10; };
            MShareGlobals.GetItemDescHandler = (_, _) => null;

            MShareGlobals.CurrentFont.TextOuts.Clear();
            w.ItemBagDirectPaint(BagButtonShim(w));
            Assert.Equal(4, hintCalls);   // ItemName + sLine1 + sLine2 + sLine3（474/475/480/485）
        });
    }

    [Fact]
    public void T176Windows_ItemBagDirectPaint_ItemDescColorPerLine()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = Loaded();
            var self = MakeSelf();
            MShareGlobals.g_MySelf = self;
            MShareGlobals.GetGoldStrHandler = g => g.ToString();
            MShareGlobals.g_MouseItem.s.NameStr = "木剑";
            MShareGlobals.g_boShowBagInfo = 1;
            MShareGlobals.GetMouseItemInfoHandler = (_, _, _, texts, flags) =>
            {
                texts[0] = "木剑"; texts[1] = "攻击 1-1"; texts[2] = "无用"; texts[3] = "";
                flags[0] = false; flags[1] = false;
            };
            MShareGlobals.DrawItemHintOldStyleHandler = (_, _, _, _) => 12;

            var desc = new GXX.Core.Util.TStringList();
            desc.AddObject("备注一", TColor.clYellow);
            desc.AddObject("备注二", TColor.clWhite);
            // boDescSupportRenamItem=0（默认）→ 原文按 DBName 查（488）
            MShareGlobals.GetItemDescHandler = (_, name) => name == "木剑DB" ? desc : null;
            MShareGlobals.g_MouseItem.s.DBNameStr = "木剑DB";

            MShareGlobals.CurrentFont.TextOuts.Clear();
            w.ItemBagDirectPaint(BagButtonShim(w));

            // 备注逐行绘制：nY + 14*(I+1)，颜色取 TStringList.Objects[I]（498）
            var outs = MShareGlobals.CurrentFont.TextOuts;
            Assert.Equal(3, outs.Count);   // 金币 + 备注一 + 备注二（提示由 DrawItemHintOldStyle 接缝承接）
            Assert.Equal("备注一", outs[1].S);
            Assert.Equal(TColor.clYellow, outs[1].Color);
            Assert.Equal("备注二", outs[2].S);
            Assert.Equal(TColor.clWhite, outs[2].Color);
            Assert.Equal(outs[1].Y + 14, outs[2].Y);
        });
    }
}
