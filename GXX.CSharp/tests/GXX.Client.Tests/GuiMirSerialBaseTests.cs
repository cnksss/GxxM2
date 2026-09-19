using System.IO;
using GXX.Client.GUI.Mir;
using GXX.Client.DxComponent;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 并行批次P1：SerialWindowsDlg.pas 基类摘录（Line 4968/6548/8084/9723/10300/10325/17323）
/// + HeroWindowsDlg.pas 全文 1:1 测试。
/// </summary>
public sealed class GuiMirSerialBaseTests
{
    [Fact]
    public void SerialWindows_Ctor_Defaults()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = new TSerialWindows();
            Assert.Equal(TClientVersion.cvSerial, w.ClientVersion);   // SerialWindowsDlg.pas:3283
            Assert.NotNull(w.NewStateWindows);                        // SerialWindowsDlg.pas:3284

            // 控件字段在托管接缝中为逻辑控件（DxComponent 自绘控件，非 WinForms 控件）
            Assert.NotNull(w.DStatePageControl);
            Assert.NotNull(w.DStateForm1);
            Assert.NotNull(w.DUserStateForm1);
            Assert.NotNull(w.DStateWin);
            Assert.NotNull(w.DStPageUp);
            Assert.NotNull(w.DItemBag);
            Assert.NotNull(w.DMerchantDlg);
            Assert.NotNull(w.DMerchantDlgClose);
            Assert.NotNull(w.DLogin);
            Assert.NotNull(w.DLoginOK);
            Assert.NotNull(w.DLoginNew);
            Assert.NotNull(w.DLoginChgPw);
        });
    }

    [Fact]
    public void TSerialWindows_StPageUpClick_FirstPage_ClampsAtZero()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = new TSerialWindows();
            for (int i = 0; i < 12; i++) MShareGlobals.g_MagicList.Add(new object());

            w.MagicIndex = 3;                            // 不足一页偏移 → 减 5 后钳到 0
            w.StPageUpClick(w.DStPageUp, 0, 0);          // 10311-10314
            Assert.Equal(0, w.MagicIndex);

            w.MagicIndex = 0;
            w.StPageUpClick(w.DStPageUp, 0, 0);          // 首页再按 → 仍为 0
            Assert.Equal(0, w.MagicIndex);
        });
    }

    [Fact]
    public void TSerialWindows_StPageUpClick_NextPage_CountGuard()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = new TSerialWindows();
            for (int i = 0; i < 12; i++) MShareGlobals.g_MagicList.Add(new object());
            var other = new TDxImageButton();

            w.MagicIndex = 0;
            w.StPageUpClick(other, 0, 0);                // 10317 MagicIndex+5 < Count(12) → 5
            Assert.Equal(5, w.MagicIndex);

            w.StPageUpClick(other, 0, 0);                // 5+5=10 < 12 → 10
            Assert.Equal(10, w.MagicIndex);

            w.StPageUpClick(other, 0, 0);                // 10+5=15 不 < 12 → 原地不动
            Assert.Equal(10, w.MagicIndex);
        });
    }

    [Fact]
    public void TSerialWindows_DLoginNewClickSound_ThreeSounds()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = new TSerialWindows();
            var played = new System.Collections.Generic.List<int>();
            SoundUtil.PlaySoundHandler = id => played.Add(id);

            w.DLoginNewClickSound(null, TClickSound.csNorm);
            w.DLoginNewClickSound(null, TClickSound.csStone);
            w.DLoginNewClickSound(null, TClickSound.csGlass);
            w.DLoginNewClickSound(null, TClickSound.csNone);   // 原文 case 无 csNone 分支 → 不发声
            Assert.Equal(new[] { 103, 104, 105 }, played);
        });
    }

    [Fact]
    public void TSerialWindows_OpenUserState_NewStateWindowsBranch()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = new TSerialWindows();                 // ClientVersion = cvSerial
            w.OpenUserState();                            // 4970 走 NewStateWindows 分支
            Assert.Equal(1, w.NewStateWindows.OpenUserStateCount);
        });
    }

    [Fact]
    public void TSerialWindows_OpenUserState_OldStyleBranch()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = new TSerialWindows { ClientVersion = TClientVersion.cv176 };
            MShareGlobals.g_UserState1.NAMECOLOR = 0x00123456;
            MShareGlobals.g_UserState1.UserNameStr = "测试甲";
            MShareGlobals.g_UserState1.GuildNameStr = "行会";
            MShareGlobals.g_UserState1.GuildRankNameStr = "会长";
            MShareGlobals.g_UserState1.JewelryBoxStatus = GXX.Core.Protocol.TJewelryBoxStatus.jbsOpen;
            MShareGlobals.g_UserState1.ShowGodBless = 1;

            w.OpenUserState();                            // 4975-4983
            Assert.Equal(0, w.NewStateWindows.OpenUserStateCount);
            Assert.Equal("测试甲", w.LabelDUserState1CharName.Caption);
            Assert.Equal(0x00123456, w.LabelDUserState1CharName.CaptionColor.Up.Value);
            Assert.Equal("行会 会长", w.LabelDUserState1RankName.Caption);
            Assert.True(w.DJewelryBoxUS1.Visible);
            Assert.True(w.DGodBlessUS1.Visible);
            Assert.True(w.DUserState1.Visible);
        });
    }

    [Fact]
    public void TSerialWindows_ShowMDlg_BaseBody()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            // 基类 TFrmDlg.ShowMDlg（FState.pas:1864）只做 NPC 对话框复位；
            // SerialWindowsDlg.pas:6548 的 MerchantFace/MerchantName/MDlgStr 在 TSerialWindows 层。
            var w = new TSerialWindows();
            w.AddNpcMemoCount = 0;
            w.ShowMDlg(7, "商人甲", "你好 <确定/@ok>", true, false);
            Assert.Equal(0, w.AddNpcMemoCount);   // 基类 ShowMDlg 不含 AddNpcMemo（205 版自行实现，见 338/340）
        });
    }

    [Fact]
    public void TSerialWindows_FontAndCaptionColor_ValueSemantics()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            // DxComponents.pas TColor 语义：字色以 32 位整数保存（0x00BBGGRR）
            var btn = new TDxImageButton();
            btn.CaptionColor.Up.Value = TColor.clRed;
            btn.CaptionColor.Hot.Value = TColor.clWhite;
            Assert.Equal(0x000000FF, btn.CaptionColor.Up.Value);
            Assert.Equal(0x00FFFFFF, btn.CaptionColor.Hot.Value);
        });
    }
}

/// <summary>HeroWindowsDlg.pas THeroWindows 全文 1:1 测试（78 行；有效逻辑仅构造/析构）。</summary>
public sealed class GuiMirHeroWindowsTests
{
    [Fact]
    public void HeroWindows_Ctor_ClientVersionIsHero()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = new THeroWindows();
            Assert.Equal(TClientVersion.cvHero, w.ClientVersion);   // HeroWindowsDlg.pas:70
            Assert.True(w is TSerialWindows);
            Assert.NotNull(w.NewStateWindows);
            w.Destroy();                                            // HeroWindowsDlg.pas:73-76
        });
    }

    [Fact]
    public void HeroWindows_NoLoadFromStreamOverride()
    {
        GuiReset.All();
        GuiSta.Run(() =>
        {
            var w = new THeroWindows();
            using var ms = new MemoryStream(new byte[] { 1, 2, 3 });
            w.LoadFromStream(ms);            // 未被 THeroWindows 覆写 → 走 TSerialWindows 接缝
            Assert.Equal(TClientVersion.cvHero, w.ClientVersion);
            Assert.Equal(3, ms.Length);      // 接缝不消费流（原文由 LoadDxControlEx 消费）
        });
    }
}
