using GXX.Client.Scenes;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>批次J43：Client 场景层第一片（IntroScn 场景状态机 + TDrawScreen.ChangeScene + Actor/magiceff 数据层）1:1 测试。</summary>
public sealed class ClientSceneTests
{
    [Fact]
    public void DrawScreen_ChangeScene_FullCycle()
    {
        var dialogs = new SceneDialogs();
        var screen = new TDrawScreen
        {
            WelcomeScene = new TWelcomeScene(),
            LoginScene = new TLoginScene(dialogs, _ => { }),
            SelectChrScene = new TSelectChrScene(dialogs),
            LoginNoticeScene = new TLoginNotice(),
            PlayScene = new TPlayScene(dialogs),
        };

        screen.ChangeScene(TSceneType.stWelcome);
        Assert.Same(screen.WelcomeScene, screen.CurrentScene);
        Assert.Equal(16777215, screen.WelcomeScene.BackgroundColor); // clWhite 初值

        screen.ChangeScene(TSceneType.stLogin);
        Assert.Same(screen.LoginScene, screen.CurrentScene);
        Assert.True(dialogs.IsOpen("DLoginDlg"));
        // 旧场景 CloseScene 已执行（背景复位黑色 + 释放纹理）
        Assert.Equal(0, screen.WelcomeScene.BackgroundColor);
        Assert.False(screen.WelcomeScene.FOpenScene);

        // 空臂：stSelectCountry/stNewChr/stLoading 不换场景
        screen.ChangeScene(TSceneType.stSelectCountry);
        Assert.Same(screen.LoginScene, screen.CurrentScene);

        screen.ChangeScene(TSceneType.stPlayGame);
        Assert.Same(screen.PlayScene, screen.CurrentScene);
        Assert.True(dialogs.ViewBottomBoxVisible); // PlayScene.OpenScene → ViewBottomBox(True)

        screen.ChangeScene(TSceneType.stSelectChr);
        Assert.Same(screen.SelectChrScene, screen.CurrentScene);
        Assert.True(dialogs.IsOpen("DSelectChrDlg"));
        Assert.False(dialogs.ViewBottomBoxVisible); // PlayScene.CloseScene → ViewBottomBox(False)

        // 登录场景随机码弹窗钩子
        int randomCodeCalls = 0;
        screen.ChangeScene(TSceneType.stLogin);
        Assert.Equal(0, randomCodeCalls);
        screen.ShowLoginSceneShowRandomCodeDlg = true;
        screen.OpenRandomCodeDlg = () => randomCodeCalls++;
        screen.ChangeScene(TSceneType.stLogin);
        Assert.Equal(1, randomCodeCalls);
    }

    [Fact]
    public void WelcomeScene_AlphaFadeMachine()
    {
        uint tick = 1000;
        SceneTime.TickNow = () => tick;
        try
        {
            var scene = new TWelcomeScene();
            scene.Initialize(); // 背景图就绪（HasTexture）
            scene.OpenScene();
            Assert.Equal(0, scene.FAlpha);
            Assert.Equal(0, scene.FAddValue);

            // 渐亮：FAlpha 每 10 的倍数步进 +1，随后累加
            for (int i = 0; i < 10; i++)
            {
                tick++;
                scene.RenderScene(null!);
            }
            Assert.Equal(10, scene.FAlpha);
            Assert.Equal(1, scene.FAddValue);

            // 推进到 255 封顶
            for (int i = 0; i < 400; i++)
                scene.RenderScene(null!);
            Assert.True(scene.FAlpha >= 255);

            scene.CloseScene();
            Assert.False(scene.FOpenScene);
            Assert.Equal(0, scene.BackgroundColor);
        }
        finally
        {
            SceneTime.TickNow = () => (uint)Environment.TickCount;
        }
    }

    [Fact]
    public void LoginScene_States_PasswordFail_And_OpenDoor()
    {
        var dialogs = new SceneDialogs { DEdIdText = "abc", DEdPasswdText = "pw" };
        TSceneType? changed = null;
        var login = new TLoginScene(dialogs, t => changed = t);

        // ChangeLoginState 各子状态对话框组合
        login.ChangeLoginState(TLoginState.lsNewid);
        Assert.True(dialogs.IsOpen("DNewAccountDlg"));
        Assert.False(dialogs.IsOpen("DLoginDlg"));
        login.ChgPwClick();
        Assert.True(dialogs.IsOpen("DChgPwDlg"));
        Assert.False(dialogs.IsOpen("DNewAccountDlg"));
        login.RealName();
        Assert.True(dialogs.IsOpen("DRealNameDlg"));
        login.BindPhone();
        Assert.True(dialogs.IsOpen("DBindPhone"));
        login.ChgpwCancel();
        Assert.True(dialogs.IsOpen("DLoginDlg"));

        // PassWdFail 四分支
        login.PassWdFail(-1); // 密码错误：清密码、聚焦密码框
        Assert.Equal("", dialogs.DEdPasswdText);
        Assert.Equal("DEdPasswd", dialogs.LastFocus);
        Assert.Equal("abc", dialogs.DEdIdText);
        login.PassWdFail(-3); // 已登录锁定：只聚焦密码框
        Assert.Equal("DEdPasswd", dialogs.LastFocus);
        login.PassWdFail(-100);
        Assert.Equal("DEdId", dialogs.LastFocus);
        login.PassWdFail(0);
        Assert.Equal("", dialogs.DEdIdText);
        Assert.Equal("", dialogs.DEdPasswdText);
        Assert.Equal("DEdId", dialogs.LastFocus);

        // 开门动画分支（boShowOpenDoor=true → 门图对话框）
        login.ShowOpenDoor = () => true;
        login.OpenLoginDoor();
        Assert.True(dialogs.IsOpen("DDoorDlg"));
        Assert.False(dialogs.IsOpen("DSelServerDlg"));
        Assert.Null(changed);

        // 直接切换分支（boShowOpenDoor=false → 隐藏登录框 + 切选角）
        login.ShowOpenDoor = () => false;
        login.OpenLoginDoor();
        Assert.True(login.m_boNowOpening);
        Assert.Equal(TSceneType.stSelectChr, changed);
        Assert.False(dialogs.IsOpen("DLoginDlg")); // HideLoginBox → lsCloseAll
    }

    [Fact]
    public void SelectChr_AddSelectStart()
    {
        var dialogs = new SceneDialogs();
        var scene = new TSelectChrScene(dialogs);
        Assert.All(scene.ChrArr, c => Assert.True(c.FreezeState)); // 构造全石化

        scene.AddChr("战士甲", 0, 2, 30, 0);
        scene.AddChr("法师乙", 1, 3, 28, 1);
        Assert.True(scene.ChrArr[0].Valid);
        Assert.True(scene.ChrArr[1].Valid);
        Assert.False(scene.ChrArr[2].Valid);

        scene.SelectChr(0);
        Assert.True(scene.ChrArr[0].Selected);
        Assert.False(scene.ChrArr[1].Selected);
        Assert.Equal(30, scene.ChrArr[0].DarkLevel);

        string? sent = null;
        scene.SendSelChrFn = n => sent = n;
        scene.SelChrStartClick();
        Assert.Equal("战士甲", sent);

        // 全部未选 → 提示
        scene.ChrArr[0].Selected = false;
        scene.SelChrStartClick();
        Assert.Contains(dialogs.Messages, m => m.StartsWith("还没创建游戏角色！"));

        // 名字过短拒绝（<4 字符）
        scene.MakeNewChar(2);
        Assert.True(scene.ChrArr[2].Valid);
        scene.SelChrNewOk("abc");
        Assert.Contains(dialogs.Messages, m => m.StartsWith("角色名称长度最低4个字符"));
        // MakeNewChar 清空人物信息（sex=0 男 → 发型 '2'）
        string? newName = null, newHair = null, newSex = null;
        scene.SendNewChrFn = (login, name, hair, job, sex) => { newName = name; newHair = hair; newSex = sex; };
        scene.LoginID = "acct001";
        scene.SelChrNewOk("  法师王五  ");
        Assert.Equal("法师王五", newName);
        Assert.Equal("2", newHair);
        Assert.Equal("0", newSex);
        // 女 → 发型 '3'（Randomize 注释体保留 → 恒 '3'）
        scene.MakeNewChar(2);
        scene.ChrArr[2].UserChr.sex = 1;
        scene.SelChrNewOk("法师王五");
        Assert.Equal("3", newHair);
        Assert.Equal("1", newSex);

        // 职业切换重选
        scene.SelChrNewJob(2);
        Assert.Equal(2, scene.ChrArr[scene.NewIndex].UserChr.job);
        scene.SelChrNewJob(9); // 非法忽略
        Assert.Equal(2, scene.ChrArr[scene.NewIndex].UserChr.job);

        // 翻页钳制：Delphi SelChrDown 先查下页首位 Valid，无效则原地不动
        scene.SelChrDown();
        Assert.Equal(0, scene.m_nChrPage);
        scene.ChrArr[2].Valid = true;
        scene.SelChrDown();
        Assert.Equal(1, scene.m_nChrPage);
        scene.SelChrUp();
        scene.SelChrUp();
        Assert.Equal(0, scene.m_nChrPage);

        // ClearChrs：清空 + 默认选中页首位
        scene.ClearChrs();
        Assert.False(scene.ChrArr[0].Valid);
        Assert.True(scene.ChrArr[0].Selected);
    }

    [Fact]
    public void SelectChr_SelChrSelect1And2_Click_Semantics()
    {
        var dialogs = new SceneDialogs();
        var scene = new TSelectChrScene(dialogs);
        scene.AddChr("甲", 0, 2, 30, 0);
        scene.AddChr("乙", 1, 3, 28, 1);

        scene.SelChrSelect1Click();
        Assert.True(scene.ChrArr[0].Selected);
        Assert.True(scene.ChrArr[0].Unfreezing);
        Assert.False(scene.ChrArr[0].FreezeState);
        Assert.False(scene.ChrArr[1].Selected);

        scene.SelChrSelect2Click();
        Assert.True(scene.ChrArr[1].Selected);
        Assert.False(scene.ChrArr[0].Selected);
        Assert.True(scene.ChrArr[0].FreezeState);
    }

    [Fact]
    public void ActorActionTables_HumanTable()
    {
        Assert.Equal(0, ActorActionTables.HA.ActStand.start);
        Assert.Equal(4, ActorActionTables.HA.ActStand.frame);
        Assert.Equal(4, ActorActionTables.HA.ActStand.skip);
        Assert.Equal(200, ActorActionTables.HA.ActStand.ftime);
        Assert.Equal(64, ActorActionTables.HA.ActWalk.start);
        Assert.Equal(6, ActorActionTables.HA.ActWalk.frame);
        Assert.Equal(80, ActorActionTables.HA.ActWalk.ftime);
        Assert.Equal(128, ActorActionTables.HA.ActRun.start);
        Assert.Equal(100, ActorActionTables.HA.ActRun.ftime);
        Assert.Equal(200, ActorActionTables.HA.ActHit.start);
        Assert.Equal(328, ActorActionTables.HA.ActBigHit.start);
        Assert.Equal(8, ActorActionTables.HA.ActBigHit.frame);
        Assert.Equal(392, ActorActionTables.HA.ActSpell.start);
        Assert.Equal(50, ActorActionTables.HA.ActSpell.ftime);
        Assert.Equal(472, ActorActionTables.HA.ActStruck.start);
        Assert.Equal(3, ActorActionTables.HA.ActStruck.frame);
        Assert.Equal(536, ActorActionTables.HA.ActDie.start);
        Assert.Equal(4, ActorActionTables.HA.ActDie.frame);
        // 连击表 18 项：1=追心刺、17=万剑归宗
        Assert.Equal(18, ActorActionTables.HA.ActContinuousHits.Length);
        Assert.Equal(80, ActorActionTables.HA.ActContinuousHits[1].start);
        Assert.Equal(8, ActorActionTables.HA.ActContinuousHits[1].frame);
        Assert.Equal(1760, ActorActionTables.HA.ActContinuousHits[17].start);
        Assert.Equal(14, ActorActionTables.HA.ActContinuousHits[17].frame);
        Assert.Equal(60, ActorActionTables.HA.ActContinuousHits[17].ftime);
    }

    [Fact]
    public void ActorActionTables_MonsterTables_And_Selector()
    {
        // MA10（8Frame 带刀卫士）
        Assert.Equal(0, ActorActionTables.MA10.ActStand.start);
        Assert.Equal(4, ActorActionTables.MA10.ActStand.frame);
        Assert.Equal(192, ActorActionTables.MA10.ActStruck.start);
        Assert.Equal(2, ActorActionTables.MA10.ActStruck.frame);
        Assert.Equal(208, ActorActionTables.MA10.ActDie.start);
        // MA9 未列字段（ActRun/ActAttack2）缺省零（Delphi typed-constant 尾部缺省）
        Assert.Equal(0, ActorActionTables.MA9.ActRun.start);
        Assert.Equal(0, ActorActionTables.MA9.ActAttack2.frame);

        Assert.Equal(ActorActionTables.MA9, ActorActionTables.GetRaceByPM(9, 0));
        Assert.Equal(ActorActionTables.MA14, ActorActionTables.GetRaceByPM(17, 0)); // 17→MA14
        Assert.Equal(ActorActionTables.MA19, ActorActionTables.GetRaceByPM(21, 0)); // 19..21→MA19
        Assert.Equal(ActorActionTables.MA30, ActorActionTables.GetRaceByPM(34, 0)); // 赤月恶魔
        Assert.Equal(ActorActionTables.MA95, ActorActionTables.GetRaceByPM(95, 0)); // 火龙守护兽
        // race 50 嵌套 Appr 分表
        Assert.Equal(ActorActionTables.MA36, ActorActionTables.GetRaceByPM(50, 23));
        Assert.Equal(ActorActionTables.MA37, ActorActionTables.GetRaceByPM(50, 27));
        Assert.Equal(ActorActionTables.MA54, ActorActionTables.GetRaceByPM(50, 56)); // 54..58
        Assert.Equal(ActorActionTables.MA54, ActorActionTables.GetRaceByPM(50, 97)); // 94..98
        Assert.Equal(ActorActionTables.NPC1, ActorActionTables.GetRaceByPM(50, 59));
        Assert.Equal(ActorActionTables.NPC60, ActorActionTables.GetRaceByPM(50, 62));
        Assert.Equal(ActorActionTables.NPC210, ActorActionTables.GetRaceByPM(50, 210));
        Assert.Equal(ActorActionTables.MA35, ActorActionTables.GetRaceByPM(50, 999)); // 嵌套缺省
        // 多值臂 221,224 与 222/223/225
        Assert.Equal(ActorActionTables.MA19, ActorActionTables.GetRaceByPM(221, 0));
        Assert.Equal(ActorActionTables.MA19, ActorActionTables.GetRaceByPM(224, 0));
        Assert.Equal(ActorActionTables.MA222, ActorActionTables.GetRaceByPM(222, 0));
        Assert.Equal(ActorActionTables.MA225, ActorActionTables.GetRaceByPM(225, 0));
        // 缺省 MA19
        Assert.Equal(ActorActionTables.MA19, ActorActionTables.GetRaceByPM(5, 0));
        Assert.Equal(ActorActionTables.MA19, ActorActionTables.GetRaceByPM(300, 0));
    }

    [Fact]
    public void GetMonAction_FileOverride()
    {
        string dir = Path.Combine(Path.GetTempPath(), "gxx_j43_" + Guid.NewGuid().ToString("N")) + "\\";
        Directory.CreateDirectory(Path.Combine(dir, "Graphics", "Monster"));
        try
        {
            ActorActionFiles.SelfResourcePath = () => dir;
            Assert.Null(ActorActionFiles.GetMonAction(7)); // 缺文件 → null

            // 写 108 字节外置动作表（MA10 内容）并回读
            var src = ActorActionTables.MA10;
            var bytes = new byte[108];
            WriteAction(bytes, 0, src.ActStand);
            WriteAction(bytes, 12, src.ActWalk);
            WriteAction(bytes, 24, src.ActRun);
            WriteAction(bytes, 36, src.ActAttack);
            WriteAction(bytes, 48, src.ActCritical);
            WriteAction(bytes, 60, src.ActStruck);
            WriteAction(bytes, 72, src.ActDie);
            WriteAction(bytes, 84, src.ActDeath);
            WriteAction(bytes, 96, src.ActAttack2);
            File.WriteAllBytes(Path.Combine(dir, "Graphics", "Monster", "7.pm"), bytes);

            var loaded = ActorActionFiles.GetMonAction(7);
            Assert.NotNull(loaded);
            Assert.Equal(src.ActStand.start, loaded.Value.ActStand.start);
            Assert.Equal(src.ActStand.frame, loaded.Value.ActStand.frame);
            Assert.Equal(src.ActStand.ftime, loaded.Value.ActStand.ftime);
            Assert.Equal(src.ActStruck.start, loaded.Value.ActStruck.start);
            Assert.Equal(src.ActDeath.start, loaded.Value.ActDeath.start);
        }
        finally
        {
            ActorActionFiles.SelfResourcePath = () => "";
            try { Directory.Delete(dir, true); } catch { /* best effort */ }
        }
    }

    private static void WriteAction(byte[] b, int off, TActionInfo a)
    {
        BitConverter.GetBytes(a.start).CopyTo(b, off);
        BitConverter.GetBytes(a.frame).CopyTo(b, off + 4);
        BitConverter.GetBytes(a.skip).CopyTo(b, off + 6);
        BitConverter.GetBytes(a.ftime).CopyTo(b, off + 8);
        BitConverter.GetBytes(a.usetick).CopyTo(b, off + 10);
    }

    [Fact]
    public void MagicEff_DataStructures()
    {
        Assert.Equal(10, MagicEffConsts.FLYBASE);
        Assert.Equal(170, MagicEffConsts.EXPLOSIONBASE);
        Assert.Equal(120, MagicEffConsts.READYTIME);
        Assert.Equal(447, MagicEffConsts.FLYOMAAXEBASE);
        var um = new TUseMagicInfo
        {
            ServerMagicCode = 1,
            MagicSerial = 5,
            target = 123456L,
            EffectType = TMagicType.mtFly,
            EffectNumber = 3,
            targx = 100,
            targy = 200,
            Recusion = true,
            anitime = 60,
            NewLevel = 2,
            MagicLevel = 3,
        };
        Assert.Equal(TMagicType.mtFly, um.EffectType);
        Assert.Equal(123456L, um.target);
        Assert.Equal(2, um.NewLevel);
    }
}
