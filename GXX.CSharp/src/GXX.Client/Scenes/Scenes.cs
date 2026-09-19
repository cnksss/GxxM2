using System;
using System.Collections.Generic;

namespace GXX.Client.Scenes;

/// <summary>IntroScn.pas 29 TSceneType。</summary>
public enum TSceneType
{
    stNone, stWelcome, stLogin, stSelectCountry, stSelectChr, stNewChr, stLoading, stLoginNotice, stPlayGame,
}

/// <summary>IntroScn.pas 27 TLoginState。</summary>
public enum TLoginState
{
    lsLogin, lsNewid, lsNewidRetry, lsChgpw, lsCloseAll, lsRealName, lsBindPhone,
}

/// <summary>IntroScn.pas 33-46 TSelChar（选角槽位）。</summary>
public class TSelChar
{
    public bool Valid;
    public TUserCharacterInfo UserChr = new();
    public bool Selected;
    public bool FreezeState;
    public bool Unfreezing;
    public bool Freezing;
    public int AniIndex;
    public int DarkLevel;
    public int EffIndex;
    public uint StartTime;
    public uint moretime;
    public uint startefftime;
}

/// <summary>选角人物信息（TUserCharacterInfo 子集：AddChr/SelSetText 使用字段）。</summary>
public class TUserCharacterInfo
{
    public string Name = "";
    public int Job;
    public int job;
    public int hair;
    public int Level;
    public int sex;
}

/// <summary>MyGetTickCount 接缝（场景计时测试确定性）。</summary>
public static class SceneTime
{
    public static Func<uint> TickNow = () => (uint)Environment.TickCount;
}

/// <summary>IntroScn.pas 48-67 TScene 基类。</summary>
public abstract class TScene
{
    public int BackgroundColor; // TColor
    public TSceneType scenetype;

    protected TScene(TSceneType ascenetype)
    {
        scenetype = ascenetype;
        BackgroundColor = 0; // clBlack
    }

    public virtual void Initialize() { }
    public virtual void Finalize() { }
    public virtual void OpenScene() { }
    public virtual void CloseScene() { }
    public virtual void OpeningScene() { }
    public virtual void RefreshScene() { }
    public virtual void KeyPress(ref char key) { }
    public virtual void KeyDown(ref ushort key, object shift) { }
    public virtual void MouseMove(object shift, int x, int y) { }
    public virtual void MouseDown(int button, object shift, int x, int y) { }
    public virtual void RenderScene(object sender) { }
}

/// <summary>ClMain.pas 对话框族接缝（FrmDlg Open*/Close* 与编辑框状态记录，测试审计用）。</summary>
public class SceneDialogs
{
    public readonly HashSet<string> OpenDialogs = new(StringComparer.Ordinal);
    public readonly List<string> Messages = new();
    public bool DEdIdVisible = true;
    public bool DEdPasswdVisible = true;
    public string DEdIdText = "";
    public string DEdPasswdText = "";
    public string LastFocus = "";
    public string NewAccountTitle = "";
    public bool CheckUserEntrysResult = true;
    public int MakeNewCharIndex = -1;
    public readonly Dictionary<string, string> SelectChrLabels = new();
    public bool ViewBottomBoxVisible;

    public void Open(string name) => OpenDialogs.Add(name);
    public void Close(string name) => OpenDialogs.Remove(name);
    public bool IsOpen(string name) => OpenDialogs.Contains(name);

    public void OpenDLoginDlg() => Open("DLoginDlg");
    public void CloseDLoginDlg() => Close("DLoginDlg");
    public void OpenDNewAccountDlg() => Open("DNewAccountDlg");
    public void CloseDNewAccountDlg() => Close("DNewAccountDlg");
    public void OpenDChgPwDlg() => Open("DChgPwDlg");
    public void CloseDChgPwDlg() => Close("DChgPwDlg");
    public void OpenDRealNameDlg() => Open("DRealNameDlg");
    public void CloseDRealNameDlg() => Close("DRealNameDlg");
    public void OpenDGetPwdBackDlg() => Open("DGetPwdBackDlg");
    public void CloseDGetPwdBackDlg() => Close("DGetPwdBackDlg");
    public void CloseDRegAccount() => Close("DRegAccount");
    public void OpenDBindPhone() => Open("DBindPhone");
    public void CloseDBindPhone() => Close("DBindPhone");
    public void OpenDDoorDlg() => Open("DDoorDlg");
    public void CloseDSelServerDlg() => Close("DSelServerDlg");
    public void OpenDSelectChrDlg() => Open("DSelectChrDlg");
    public void CloseDSelectChrDlg() => Close("DSelectChrDlg");
    public void OpenDRandomCodeDlg() => Open("DRandomCodeDlg");
    public void CloseDCreateChrDlg() => Close("DCreateChrDlg");
    public void ViewBottomBox(bool visible) => ViewBottomBoxVisible = visible;
    public void HideChatEdit() => Close("ChatEdit");

    public void DMessageDlg(string msg) => Messages.Add(msg);
    public void SetFocus(string editName) => LastFocus = editName;
}

/// <summary>IntroScn.pas 69-85 TWelcomeScene（开场图渐亮状态机）。</summary>
public class TWelcomeScene : TScene
{
    private bool FInitialized;
    public bool FOpenScene;
    public int FAddValue;
    public int FAlpha;
    public uint FAlphaTick;
    public bool HasTexture;

    public TWelcomeScene() : base(TSceneType.stWelcome)
    {
        FInitialized = false;
        FOpenScene = true;
        BackgroundColor = 16777215; // clWhite
        FAlpha = 0;
        FAlphaTick = SceneTime.TickNow();
    }

    public override void Initialize()
    {
        FInitialized = true;
        HasTexture = true; // Delphi 由 g_BackImage 建纹理；有图时背景色取首像素
    }

    public override void Finalize()
    {
        FInitialized = false;
        HasTexture = false;
    }

    public override void OpenScene()
    {
        FAlpha = 0;
        FAlphaTick = SceneTime.TickNow();
        FAddValue = 0;
        FOpenScene = true;
    }

    public override void CloseScene()
    {
        FOpenScene = false;
        BackgroundColor = 0;
        HasTexture = false;
    }

    public override void RenderScene(object sender)
    {
        // Delphi 302-319：FOpenScene 且有纹理时渐亮（FAlpha 每 10 的倍数递增步进），满 255 全亮
        if (FOpenScene && HasTexture)
        {
            if (FAlpha < 255)
            {
                FAlphaTick = SceneTime.TickNow();
                if (FAlpha % 10 == 0)
                    FAddValue++;
                FAlpha += FAddValue;
            }
        }
        else
            BackgroundColor = 0;
    }
}

/// <summary>IntroScn.pas 87-120 TLoginScene（登录场景状态机）。</summary>
public class TLoginScene : TScene
{
    public int m_nCurFrame;
    public int m_nMaxFrame;
    public uint m_dwStartTime;
    public bool m_boNowOpening;
    public bool m_boOpenFirst;
    public bool m_boUpdateAccountMode;
    public string m_sLoginId = "";
    public string m_sLoginPasswd = "";

    /// <summary>登录成功选服后开门分支：true=门图对话框，false=直接切换选角场景（Delphi 397-416）。</summary>
    public Func<bool> ShowOpenDoor = () => true;
    public SceneDialogs FrmDlg;
    public Action<TSceneType> ChangeSceneFn = _ => { };
    public List<string> BgmPlaylist = new();

    public TLoginScene(SceneDialogs dialogs, Action<TSceneType> changeSceneFn) : base(TSceneType.stLogin)
    {
        FrmDlg = dialogs;
        ChangeSceneFn = changeSceneFn;
    }

    public override void OpenScene()
    {
        m_nCurFrame = 0;
        m_nMaxFrame = 10;
        m_sLoginId = "";
        m_sLoginPasswd = "";
        m_boOpenFirst = true;
        FrmDlg.OpenDLoginDlg();
        FrmDlg.CloseDNewAccountDlg();
        FrmDlg.CloseDRealNameDlg();
        FrmDlg.CloseDGetPwdBackDlg();
        FrmDlg.CloseDRegAccount();
        FrmDlg.CloseDBindPhone();
        m_boNowOpening = false;
        BgmPlaylist.Add("bmg_intro");
    }

    public override void CloseScene()
    {
        m_boNowOpening = false;
        FrmDlg.CloseDLoginDlg();
        BgmPlaylist.Clear(); // SilenceSound
    }

    /// <summary>Delphi 368-385 PassWdFail（按 FailCode 清空与聚焦）。</summary>
    public void PassWdFail(int failCode)
    {
        FrmDlg.DEdIdVisible = true;
        FrmDlg.DEdPasswdVisible = true;
        if (failCode == -1)
        {
            FrmDlg.DEdPasswdText = "";
            FrmDlg.SetFocus("DEdPasswd");
        }
        else if (failCode == -3)
        {
            FrmDlg.SetFocus("DEdPasswd");
        }
        else if (failCode == -100)
        {
            FrmDlg.SetFocus("DEdId");
        }
        else
        {
            FrmDlg.DEdIdText = "";
            FrmDlg.DEdPasswdText = "";
            FrmDlg.SetFocus("DEdId");
        }
    }

    /// <summary>Delphi 397-416 OpenLoginDoor（开门动画或直接切场景）。</summary>
    public void OpenLoginDoor()
    {
        if (ShowOpenDoor())
        {
            FrmDlg.OpenDDoorDlg();
            FrmDlg.CloseDSelServerDlg();
        }
        else
        {
            m_boNowOpening = true;
            m_dwStartTime = SceneTime.TickNow();
            HideLoginBox();
            FrmDlg.CloseDSelServerDlg();
            ChangeSceneFn(TSceneType.stSelectChr);
        }
    }

    public void HideLoginBox() => ChangeLoginState(TLoginState.lsCloseAll);

    /// <summary>Delphi 423-499 ChangeLoginState（各登录子状态的对话框开关组合）。</summary>
    public void ChangeLoginState(TLoginState state)
    {
        switch (state)
        {
            case TLoginState.lsLogin:
                FrmDlg.CloseDNewAccountDlg();
                FrmDlg.CloseDChgPwDlg();
                FrmDlg.OpenDLoginDlg();
                FrmDlg.CloseDRealNameDlg();
                FrmDlg.CloseDGetPwdBackDlg();
                FrmDlg.CloseDRegAccount();
                FrmDlg.CloseDBindPhone();
                if (FrmDlg.DEdIdVisible)
                    FrmDlg.SetFocus("DEdId");
                break;
            case TLoginState.lsNewidRetry:
            case TLoginState.lsNewid:
                FrmDlg.OpenDNewAccountDlg();
                FrmDlg.CloseDChgPwDlg();
                FrmDlg.CloseDLoginDlg();
                FrmDlg.CloseDRealNameDlg();
                FrmDlg.CloseDGetPwdBackDlg();
                FrmDlg.CloseDRegAccount();
                FrmDlg.CloseDBindPhone();
                if (FrmDlg.DEdIdVisible)
                    FrmDlg.SetFocus("DEdNewId");
                break;
            case TLoginState.lsChgpw:
                FrmDlg.CloseDNewAccountDlg();
                FrmDlg.OpenDChgPwDlg();
                FrmDlg.CloseDLoginDlg();
                FrmDlg.CloseDRealNameDlg();
                FrmDlg.CloseDGetPwdBackDlg();
                FrmDlg.CloseDRegAccount();
                FrmDlg.CloseDBindPhone();
                break;
            case TLoginState.lsRealName:
                FrmDlg.CloseDNewAccountDlg();
                FrmDlg.CloseDChgPwDlg();
                FrmDlg.CloseDLoginDlg();
                FrmDlg.CloseDGetPwdBackDlg();
                FrmDlg.CloseDRegAccount();
                FrmDlg.CloseDBindPhone();
                FrmDlg.OpenDRealNameDlg();
                break;
            case TLoginState.lsBindPhone:
                FrmDlg.CloseDNewAccountDlg();
                FrmDlg.CloseDChgPwDlg();
                FrmDlg.CloseDLoginDlg();
                FrmDlg.CloseDGetPwdBackDlg();
                FrmDlg.CloseDRegAccount();
                FrmDlg.CloseDRealNameDlg();
                FrmDlg.OpenDBindPhone();
                break;
            case TLoginState.lsCloseAll:
                FrmDlg.CloseDNewAccountDlg();
                FrmDlg.CloseDChgPwDlg();
                FrmDlg.CloseDLoginDlg();
                FrmDlg.CloseDRealNameDlg();
                FrmDlg.CloseDGetPwdBackDlg();
                FrmDlg.CloseDRegAccount();
                FrmDlg.CloseDBindPhone();
                break;
        }
    }

    public void NewClick()
    {
        m_boUpdateAccountMode = false;
        FrmDlg.NewAccountTitle = "";
        ChangeLoginState(TLoginState.lsNewid);
    }

    public void NewIdRetry(bool boupdate)
    {
        m_boUpdateAccountMode = boupdate;
        ChangeLoginState(TLoginState.lsNewidRetry);
    }

    public void RealName() => ChangeLoginState(TLoginState.lsRealName);

    public void BindPhone() => ChangeLoginState(TLoginState.lsBindPhone);

    public void ChgPwClick() => ChangeLoginState(TLoginState.lsChgpw);

    public void NewAccountClose()
    {
        if (!m_boUpdateAccountMode)
            ChangeLoginState(TLoginState.lsLogin);
    }

    public void ChgpwOk() { }

    public void ChgpwCancel() => ChangeLoginState(TLoginState.lsLogin);
}

/// <summary>IntroScn.pas 121-160 TSelectChrScene（选角场景，20 槽位）。</summary>
public class TSelectChrScene : TScene
{
    public int m_nCurFrame;
    public int m_nMaxFrame;
    public uint m_dwStartTime;
    public int m_nChrPage;
    public int m_nHighIndex;
    public bool CreateChrMode;
    public int m_nSelected;
    public int NewIndex;
    public readonly TSelChar[] ChrArr = new TSelChar[20];

    public SceneDialogs FrmDlg;
    public Action<string>? SendSelChrFn;
    public Action<string>? SendDelChrFn;
    public Action<string, string, string, string, string>? SendNewChrFn;
    public string LoginID = "";
    public Func<int, string> JobNameFn = _ => "";

    public TSelectChrScene(SceneDialogs dialogs) : base(TSceneType.stSelectChr)
    {
        FrmDlg = dialogs;
        CreateChrMode = false;
        for (int i = 0; i < ChrArr.Length; i++)
        {
            ChrArr[i] = new TSelChar();
            ChrArr[i].FreezeState = true;
        }
        NewIndex = 0;
        m_nChrPage = 0;
        m_nHighIndex = 0;
    }

    public override void OpenScene()
    {
        m_nCurFrame = 0;
        m_nMaxFrame = 10;
        FrmDlg.OpenDSelectChrDlg();
    }

    public override void CloseScene()
    {
        FrmDlg.CloseDSelectChrDlg();
    }

    /// <summary>Delphi 672-710 SelChrSelect1Click（页内左位选择，石化复活态复位）。</summary>
    public void SelChrSelect1Click()
    {
        int idx = m_nChrPage * 2;
        if (!ChrArr[idx].Selected && ChrArr[idx].Valid)
        {
            for (int i = 0; i < ChrArr.Length; i++)
            {
                ChrArr[i].Selected = false;
                ChrArr[i].FreezeState = true;
            }
            ChrArr[idx].Selected = true;
            ChrArr[idx].FreezeState = false;
            ChrArr[idx].Unfreezing = true;
            ChrArr[idx].Freezing = false;
            ChrArr[idx].AniIndex = 0;
            ChrArr[idx].DarkLevel = 0;
            ChrArr[idx].EffIndex = 0;
            ChrArr[idx].StartTime = SceneTime.TickNow();
            ChrArr[idx].moretime = SceneTime.TickNow();
            ChrArr[idx].startefftime = SceneTime.TickNow();
        }
    }

    /// <summary>Delphi 712-749 SelChrSelect2Click（页内右位选择）。</summary>
    public void SelChrSelect2Click()
    {
        int idx = m_nChrPage * 2 + 1;
        if (idx < ChrArr.Length && !ChrArr[idx].Selected && ChrArr[idx].Valid)
        {
            for (int i = 0; i < ChrArr.Length; i++)
            {
                ChrArr[i].Selected = false;
                ChrArr[i].FreezeState = true;
            }
            ChrArr[idx].Selected = true;
            ChrArr[idx].FreezeState = false;
            ChrArr[idx].Unfreezing = true;
            ChrArr[idx].Freezing = false;
            ChrArr[idx].AniIndex = 0;
            ChrArr[idx].DarkLevel = 0;
            ChrArr[idx].EffIndex = 0;
            ChrArr[idx].StartTime = SceneTime.TickNow();
            ChrArr[idx].moretime = SceneTime.TickNow();
            ChrArr[idx].startefftime = SceneTime.TickNow();
        }
    }

    /// <summary>Delphi 782-813 SelChrStartClick（首个 Valid+Selected 角色发送；无名提示）。</summary>
    public void SelChrStartClick()
    {
        string chrname = "";
        int i = 0;
        while (true)
        {
            if (ChrArr[i].Valid && ChrArr[i].Selected)
            {
                chrname = ChrArr[i].UserChr.Name;
                break;
            }
            i++;
            if (i > ChrArr.GetUpperBound(0))
                break;
        }
        if (chrname != "")
            SendSelChrFn?.Invoke(chrname);
        else
            FrmDlg.DMessageDlg("还没创建游戏角色！\r\n点击创建角色按钮创建一个游戏角色。");
    }

    /// <summary>Delphi 815-859 SelChrNewChrClick（上限校验 + MakeNewChar(HighIndex+1)）。</summary>
    public void SelChrNewChrClick()
    {
        m_nSelected = 0;
        if (ChrArr[0].Valid && ChrArr[0].Selected)
            m_nSelected = 0;
        if (ChrArr[1].Valid && ChrArr[1].UserChr.Name != "" && ChrArr[1].Selected)
            m_nSelected = 1;
        if (ChrArr[2].Valid && ChrArr[2].UserChr.Name != "" && ChrArr[2].Selected)
            m_nSelected = 2;

        if (m_nHighIndex > ChrArr.GetUpperBound(0))
            FrmDlg.DMessageDlg("可创建角色已到上限。");
        else
            MakeNewChar(m_nHighIndex + 1);
    }

    /// <summary>Delphi 909-932 ClearChrs（清空并默认选中页首位——原文 FreezeState 置 False 瑕疵保留）。</summary>
    public void ClearChrs()
    {
        for (int i = 0; i < ChrArr.Length; i++)
        {
            ChrArr[i] = new TSelChar();
            ChrArr[i].FreezeState = false;
            ChrArr[i].Selected = false;
            ChrArr[i].UserChr.Name = "";
        }
        ChrArr[m_nChrPage * 2].FreezeState = false;
        ChrArr[m_nChrPage * 2].Selected = true;
    }

    /// <summary>Delphi 934-965 AddChr（首个空槽入列）。</summary>
    public void AddChr(string uname, int job, int hair, int level, int sex)
    {
        int n = 0;
        while (true)
        {
            if (!ChrArr[n].Valid)
                break;
            n++;
            if (n > ChrArr.GetUpperBound(0))
            {
                n = -1;
                break;
            }
        }
        if (n < 0)
            return;
        ChrArr[n].UserChr.Name = uname;
        ChrArr[n].UserChr.job = job;
        ChrArr[n].UserChr.hair = hair;
        ChrArr[n].UserChr.Level = level;
        ChrArr[n].UserChr.sex = sex;
        ChrArr[n].Valid = true;
    }

    /// <summary>Delphi 967-978 MakeNewChar。</summary>
    public void MakeNewChar(int index)
    {
        CreateChrMode = true;
        NewIndex = index;
        ChrArr[NewIndex].UserChr = new TUserCharacterInfo();
        FrmDlg.MakeNewCharIndex = index;
        ChrArr[NewIndex].Valid = true;
        ChrArr[NewIndex].FreezeState = false;
        SelectChr(NewIndex);
    }

    /// <summary>Delphi 980-998 SelectChr（选中互斥 + DarkLevel=30）。</summary>
    public void SelectChr(int index)
    {
        ChrArr[index].Selected = true;
        ChrArr[index].DarkLevel = 30;
        ChrArr[index].StartTime = SceneTime.TickNow();
        ChrArr[index].moretime = SceneTime.TickNow();
        if (index == 0)
        {
            ChrArr[1].Selected = false;
            ChrArr[2].Selected = false;
        }
        else if (index == 1)
        {
            ChrArr[0].Selected = false;
            ChrArr[2].Selected = false;
        }
        else
        {
            ChrArr[0].Selected = false;
            ChrArr[1].Selected = false;
        }
    }

    /// <summary>Delphi 1000-1017 SelChrNewClose。</summary>
    public void SelChrNewClose()
    {
        ChrArr[NewIndex].Valid = false;
        CreateChrMode = false;
        FrmDlg.CloseDCreateChrDlg();
        ChrArr[m_nSelected].Selected = true;
        ChrArr[m_nSelected].FreezeState = false;
    }

    public void SelChrUp()
    {
        m_nChrPage--;
        if (m_nChrPage < 0)
            m_nChrPage = 0;
        SelSetText();
    }

    public void SelChrDown()
    {
        if ((m_nChrPage + 1) * 2 < ChrArr.Length && !ChrArr[(m_nChrPage + 1) * 2].Valid)
            return;
        m_nChrPage++;
        if (m_nChrPage > ChrArr.GetUpperBound(0))
            m_nChrPage = ChrArr.GetUpperBound(0);
        SelSetText();
    }

    /// <summary>Delphi 1037-1104 SelSetText（页内两栏 + 第三栏标签回填）。</summary>
    public void SelSetText()
    {
        SetChrLabels("1", m_nChrPage * 2);
        SetChrLabels("2", m_nChrPage * 2 + 1);
        SetChrLabels("3", 2);
    }

    private void SetChrLabels(string suffix, int index)
    {
        if (index < ChrArr.Length && ChrArr[index].Valid)
        {
            FrmDlg.SelectChrLabels["CharName" + suffix] = ChrArr[index].UserChr.Name;
            FrmDlg.SelectChrLabels["Level" + suffix] = ChrArr[index].UserChr.Level.ToString();
            FrmDlg.SelectChrLabels["Job" + suffix] = JobNameFn(ChrArr[index].UserChr.job);
        }
        else
        {
            FrmDlg.SelectChrLabels["CharName" + suffix] = "";
            FrmDlg.SelectChrLabels["Level" + suffix] = "";
            FrmDlg.SelectChrLabels["Job" + suffix] = "";
        }
    }

    /// <summary>Delphi 1106-1150 SelChrNewOk（名字 &lt;4 字符拒绝；性别发型规则 男='2' 女='3'）。</summary>
    public void SelChrNewOk(string chrName)
    {
        string chrname = chrName.Trim();
        if (chrname.Length < 4)
        {
            FrmDlg.DMessageDlg("角色名称长度最低4个字符或2汉字！");
            return;
        }
        ChrArr[NewIndex].Valid = false;
        CreateChrMode = false;
        FrmDlg.CloseDCreateChrDlg();
        ChrArr[m_nSelected].Selected = true;
        ChrArr[m_nSelected].FreezeState = false;

        string shair;
        switch (ChrArr[NewIndex].UserChr.sex)
        {
            case 0: shair = "2"; break;
            default: shair = "3"; break; // Randomize 注释体保留 → 恒 '3'
        }
        string sjob = ChrArr[NewIndex].UserChr.job.ToString();
        string ssex = ChrArr[NewIndex].UserChr.sex.ToString();
        SendNewChrFn?.Invoke(LoginID, chrname, shair, sjob, ssex);
    }

    /// <summary>Delphi 1152-1158 SelChrNewJob。</summary>
    public void SelChrNewJob(int job)
    {
        if (job is >= 0 and <= 2 && ChrArr[NewIndex].UserChr.job != job)
        {
            ChrArr[NewIndex].UserChr.job = job;
            SelectChr(NewIndex);
        }
    }

    /// <summary>Delphi 1160-1166 SelChrNewSex。</summary>
    public void SelChrNewSex(int sex)
    {
        if (sex != ChrArr[NewIndex].UserChr.sex)
        {
            ChrArr[NewIndex].UserChr.sex = sex;
            SelectChr(NewIndex);
        }
    }
}

/// <summary>IntroScn.pas 162-167 TLoginNotice。</summary>
public class TLoginNotice : TScene
{
    public TLoginNotice() : base(TSceneType.stLoginNotice)
    {
    }
}

/// <summary>DrawScrn.pas 674 TDrawScreen（场景调度：CurrentScene + ChangeScene + 输入分发）。</summary>
public class TDrawScreen
{
    public TScene? CurrentScene;
    public TWelcomeScene? WelcomeScene;
    public TLoginScene? LoginScene;
    public TSelectChrScene? SelectChrScene;
    public TLoginNotice? LoginNoticeScene;
    public TPlayScene? PlayScene;
    public bool ShowLoginSceneShowRandomCodeDlg;
    public Action? OpenRandomCodeDlg;

    public void KeyPress(ref char key) => CurrentScene?.KeyPress(ref key);
    public void KeyDown(ref ushort key) => CurrentScene?.KeyDown(ref key, new object());

    /// <summary>DrawScrn.pas 4465-4506 ChangeScene（旧场景 CloseScene → 分派 → 新场景 OpenScene；stSelectCountry/stNewChr/stLoading 空臂；随机码弹窗钩子）。</summary>
    public void ChangeScene(TSceneType sceneType)
    {
        CurrentScene?.CloseScene();
        switch (sceneType)
        {
            case TSceneType.stWelcome:
                CurrentScene = WelcomeScene;
                break;
            case TSceneType.stLogin:
                CurrentScene = LoginScene;
                break;
            case TSceneType.stSelectCountry:
                break;
            case TSceneType.stSelectChr:
                CurrentScene = SelectChrScene;
                break;
            case TSceneType.stNewChr:
                break;
            case TSceneType.stLoading:
                break;
            case TSceneType.stLoginNotice:
                CurrentScene = LoginNoticeScene;
                break;
            case TSceneType.stPlayGame:
                CurrentScene = PlayScene;
                break;
        }
        if (CurrentScene != null)
        {
            CurrentScene.OpenScene();
            if (CurrentScene == LoginScene && ShowLoginSceneShowRandomCodeDlg)
                OpenRandomCodeDlg?.Invoke();
        }
    }
}
