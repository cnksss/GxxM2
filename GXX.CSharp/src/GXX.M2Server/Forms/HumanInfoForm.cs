using GXX.Core.Protocol;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Forms;

/// <summary>
/// HumanInfo.pas TfrmHumanInfo 1:1 核心转换（在线角色详情查看/编辑/踢下线/监视刷新）。
/// RefHumanInfo 全量 400 行逐控件展示按核心属性子集落地，其余控件批次接入；
/// ButtonSaveClick 校验（等级上限/PK 0..2000000/声望·荣誉·点数非负/点数≤20000000）与写回 1:1。
/// </summary>
public sealed class HumanInfoForm : System.Windows.Forms.Form
{
    /// <summary>Delphi BaseObject 字段。</summary>
    public TCreature? BaseObject;
    private bool boRefHuman;

    public System.Windows.Forms.TextBox EditHumanStatus = null!;
    public System.Windows.Forms.TextBox EditLevel = null!;
    public System.Windows.Forms.TextBox EditGold = null!;
    public System.Windows.Forms.TextBox EditPKPoint = null!;
    public System.Windows.Forms.TextBox EditGameGold = null!;
    public System.Windows.Forms.TextBox EditGamePoint = null!;
    public System.Windows.Forms.TextBox seGameDiamond = null!;
    public System.Windows.Forms.TextBox seGameGird = null!;
    public System.Windows.Forms.TextBox EditCreditPoint = null!;
    public System.Windows.Forms.TextBox EditGameGlory = null!;
    public System.Windows.Forms.TextBox EditBonusPoint = null!;
    public System.Windows.Forms.TextBox EditSayMsg = null!;
    public System.Windows.Forms.CheckBox CheckBoxMonitor = null!;
    public System.Windows.Forms.CheckBox CheckBoxGameMaster = null!;
    public System.Windows.Forms.CheckBox CheckBoxObserver = null!;
    public System.Windows.Forms.CheckBox CheckBoxSuperMan = null!;
    public System.Windows.Forms.Button ButtonKick = null!;
    public System.Windows.Forms.Button ButtonSave = null!;
    public System.Windows.Forms.Timer Timer = null!;

    public HumanInfoForm()
    {
        InitializeComponent();
    }

    private System.Windows.Forms.TextBox MakeRow(string caption, int top)
    {
        var lb = new System.Windows.Forms.Label { Text = caption, Left = 14, Top = top + 3, AutoSize = true };
        var edit = new System.Windows.Forms.TextBox { Left = 140, Top = top, Width = 180 };
        Controls.Add(lb);
        Controls.Add(edit);
        return edit;
    }

    private void InitializeComponent()
    {
        Text = "角色信息";
        Width = 380;
        Height = 520;
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;

        int y = 8;
        EditHumanStatus = MakeRow("状态:", y); y += 28;
        EditLevel = MakeRow("等级:", y); y += 28;
        EditGold = MakeRow("金币:", y); y += 28;
        EditPKPoint = MakeRow("PK值:", y); y += 28;
        EditGameGold = MakeRow("元宝:", y); y += 28;
        EditGamePoint = MakeRow("游戏点:", y); y += 28;
        seGameDiamond = MakeRow("金刚石:", y); y += 28;
        seGameGird = MakeRow("灵符:", y); y += 28;
        EditCreditPoint = MakeRow("声望:", y); y += 28;
        EditGameGlory = MakeRow("荣誉值:", y); y += 28;
        EditBonusPoint = MakeRow("属性点:", y); y += 28;
        EditSayMsg = MakeRow("自动回复:", y); y += 32;

        CheckBoxMonitor = new System.Windows.Forms.CheckBox { Text = "监视刷新", Left = 14, Top = y, AutoSize = true };
        CheckBoxMonitor.Click += (s, e) => CheckBoxMonitorClick(s);
        Controls.Add(CheckBoxMonitor); y += 26;
        CheckBoxGameMaster = new System.Windows.Forms.CheckBox { Text = "GM", Left = 14, Top = y, AutoSize = true };
        CheckBoxObserver = new System.Windows.Forms.CheckBox { Text = "观察者", Left = 90, Top = y, AutoSize = true };
        CheckBoxSuperMan = new System.Windows.Forms.CheckBox { Text = "无敌", Left = 180, Top = y, AutoSize = true };
        Controls.Add(CheckBoxGameMaster);
        Controls.Add(CheckBoxObserver);
        Controls.Add(CheckBoxSuperMan); y += 32;

        ButtonKick = new System.Windows.Forms.Button { Text = "踢下线(&K)", Left = 14, Top = y, Width = 90, Height = 26 };
        ButtonKick.Click += (s, e) => ButtonKickClick(s);
        ButtonSave = new System.Windows.Forms.Button { Text = "保存(&S)", Left = 120, Top = y, Width = 90, Height = 26 };
        ButtonSave.Click += (s, e) => ButtonSaveClick(s);
        Controls.Add(ButtonKick);
        Controls.Add(ButtonSave);

        Timer = new System.Windows.Forms.Timer { Interval = 1000 };
        Timer.Tick += (s, e) => TimerTimer(s);
    }

    // ================= Delphi 1:1 =================

    /// <summary>Delphi Open()；showModal=false 供测试/宿主复用。</summary>
    public void Open(bool showModal = true)
    {
        CheckBoxMonitor.Checked = false;
        btnSaveU_Reset();
        RefHumanInfo();
        ButtonKick.Enabled = true;
        Timer.Enabled = true;
        if (showModal)
            ShowDialog();
        CheckBoxMonitor.Checked = false;
        Timer.Enabled = false;
    }

    private void btnSaveU_Reset()
    {
        ButtonSave.Enabled = true;
    }

    public void RefHumanInfo()
    {
        if (BaseObject == null)
            return;
        EditLevel.Text = BaseObject.m_wAbil.Level.ToString();
        if (BaseObject is TPlayObject playObject)
        {
            EditGold.Text = playObject.m_nGold.ToString();
            EditPKPoint.Text = playObject.m_nPKPOINT.ToString();
            EditGameGold.Text = playObject.m_nGameGold.ToString();
            EditGamePoint.Text = playObject.m_nGamePoint.ToString();
            seGameDiamond.Text = playObject.m_nGameDiamond.ToString();
            seGameGird.Text = playObject.m_nGameGird.ToString();
            EditCreditPoint.Text = BaseObject.m_wAbil.CreditPoint.ToString();
            EditGameGlory.Text = playObject.m_nGameGlory.ToString();
            EditBonusPoint.Text = playObject.m_nBonusPoint.ToString();
            EditSayMsg.Text = playObject.m_sAutoSendMsg;
            CheckBoxGameMaster.Checked = playObject.m_boGameMaster;
            CheckBoxObserver.Checked = playObject.m_boObServer;
            CheckBoxSuperMan.Checked = playObject.m_boSuperman;
        }
    }

    public void TimerTimer(object? sender)
    {
        if (BaseObject == null)
            return;
        if (BaseObject.m_boGhost)
        {
            EditHumanStatus.Text = "下线";
            BaseObject = null;
            return;
        }
        if (boRefHuman)
            RefHumanInfo();
    }

    public void CheckBoxMonitorClick(object? sender)
    {
        boRefHuman = CheckBoxMonitor.Checked;
        ButtonSave.Enabled = !boRefHuman;
    }

    public void ButtonKickClick(object? sender)
    {
        if (BaseObject == null)
            return;
        if (BaseObject.m_btRace == Grobal2Const.RC_PLAYOBJECT)
        {
            var playObject = (TPlayObject)BaseObject;
            playObject.m_boOffLine = false;
            playObject.m_boPlayOffLine = false;
            playObject.m_boEmergencyClose = true;
        }
        else
        {
            // Delphi: HeroObject.LogOut（THeroObject 批次接入前以 Ghost 标记等效）
            if (BaseObject is TPlayObject heroPlay)
                heroPlay.MakeGhost();
        }
        ButtonKick.Enabled = false;
    }

    public bool ButtonSaveClick(object? sender)
    {
        if (BaseObject == null)
            return false;

        if (BaseObject.m_btRace != Grobal2Const.RC_PLAYOBJECT && BaseObject.m_btRace != Grobal2Const.RC_HEROOBJECT)
            return false;

        string sAutoSendMsg = EditSayMsg.Text.Trim();
        uint nLevel = (uint)Math.Max(0, DelphiRTL_StrToInt(EditLevel.Text));
        uint nGold = (uint)Math.Max(0, DelphiRTL_StrToInt(EditGold.Text));
        int nPKPOINT = DelphiRTL_StrToInt(EditPKPoint.Text);
        int nGameGold = DelphiRTL_StrToInt(EditGameGold.Text);
        int nGamePoint = DelphiRTL_StrToInt(EditGamePoint.Text);
        int nGameDiamond = DelphiRTL_StrToInt(seGameDiamond.Text);
        int nGameGrid = DelphiRTL_StrToInt(seGameGird.Text);
        int nCreditPoint = DelphiRTL_StrToInt(EditCreditPoint.Text);
        int nGameGlory = DelphiRTL_StrToInt(EditGameGlory.Text);
        int nBonusPoint = DelphiRTL_StrToInt(EditBonusPoint.Text);
        bool boGameMaster = CheckBoxGameMaster.Checked;
        bool boObServer = CheckBoxObserver.Checked;
        bool boSuperMan = CheckBoxSuperMan.Checked;

        long maxLevel;
        if (M2Config.btMaxLevel == 0)
            maxLevel = ushort.MaxValue;
        else if (M2Config.btMaxLevel == 1)
            maxLevel = int.MaxValue;
        else
            maxLevel = uint.MaxValue;
        int maxNgLevel = 1000; // MAXNG_LEVEL

        if (nLevel > maxLevel || nNGLevelInvalid(maxNgLevel) || nPKPOINT < 0 || nPKPOINT > 2000000 ||
            nCreditPoint < 0 || nGameGlory < 0 || nBonusPoint < 0 || nBonusPoint > 20000000)
        {
            M2Forms.MessageBox("输入数据不正确！", "错误信息", M2Forms.MB_OK);
            return false;
        }

        if (BaseObject is TPlayObject playObject)
        {
            playObject.m_nGold = nGold;
            playObject.m_nPKPOINT = nPKPOINT;
            playObject.m_nGameGold = nGameGold;
            playObject.m_nGamePoint = nGamePoint;
            playObject.m_nGameDiamond = nGameDiamond;
            playObject.m_nGameGird = nGameGrid;
            playObject.m_nGameGlory = nGameGlory;
            playObject.m_nBonusPoint = nBonusPoint;
            playObject.m_sAutoSendMsg = sAutoSendMsg;
            playObject.m_boGameMaster = boGameMaster;
            playObject.m_boObServer = boObServer;
            playObject.m_boSuperman = boSuperMan;

            // 批次J13：属性点计入 m_BonusAbil 并经 RecalcAdjusBonus 重算 DC/MC/SC/AC/MAC 与 MaxHP/MP
            playObject.m_BonusAbil.BonusPoint = nBonusPoint;
            playObject.RecalcAdjusBonus();
        }
        BaseObject.m_wAbil.Level = nLevel;
        BaseObject.m_wAbil.CreditPoint = nCreditPoint;
        return true;
    }

    private static bool nNGLevelInvalid(int ngLevel) => ngLevel < 0 || ngLevel > 1000;

    private static int DelphiRTL_StrToInt(string s)
        => GXX.Core.Rtl.DelphiRTL.StrToIntDef(s.Trim(), 0);
}
