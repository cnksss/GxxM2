using GXX.Core.Rtl;
using GXX.Core.Protocol;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Forms;

/// <summary>
/// ViewOnlineHuman.pas TfrmViewOnlineHuman 1:1 转换（在线玩家/英雄查看表，19 列）。
/// m_PlayObjectList/m_HeroObjectList → PlayObjects/HeroObjects；GridHuman.Row → GridSelectedRow；
/// frmHumanInfo（HumanInfo.pas）批次接入前经 ShowHumanInfoHandler 接缝调用。
/// </summary>
public sealed class ViewOnlineHumanForm : System.Windows.Forms.Form
{
    private readonly TUserEngine _engine;
    private readonly GXX.Core.Util.TStringList ViewList = new();
    private uint dwTimeOutTick;

    public System.Windows.Forms.DataGridView GridHuman = null!;
    public System.Windows.Forms.Button ButtonRefGrid = null!;
    public System.Windows.Forms.Button ButtonSearch = null!;
    public System.Windows.Forms.Button ButtonView = null!;
    public System.Windows.Forms.Button ButtonKickPlayOffLine = null!;
    public System.Windows.Forms.Button ButtonKickDummyObject = null!;
    public System.Windows.Forms.TextBox EditSearchName = null!;
    public System.Windows.Forms.CheckBox chkHuman = null!;
    public System.Windows.Forms.CheckBox chkDummy = null!;
    public System.Windows.Forms.CheckBox chkHumanHero = null!;
    public System.Windows.Forms.CheckBox chkDummyHero = null!;
    public System.Windows.Forms.Panel PanelStatus = null!;
    public System.Windows.Forms.Timer Timer = null!;

    /// <summary>Delphi GridHuman.Row（1 起数据行号；0=固定行；-1=无选择）。</summary>
    public int GridSelectedRow = -1;

    // ---- 测试观察点（ViewList 内部状态） ----
    public int ViewListCount => ViewList.Count;
    public TCreature? GetViewObject(int i) => (TCreature?)ViewList.GetObject(i);
    public string ViewListString(int i) => ViewList[i];
    public System.Collections.Generic.IEnumerable<string> ViewListStrings => ViewList.AsEnumerable();

    /// <summary>frmHumanInfo 接缝（HumanInfo.pas 批次接入）：ShowHumanInfo 命中时回调。</summary>
    public Func<TCreature, int>? ShowHumanInfoHandler;

    public ViewOnlineHumanForm() : this(new TUserEngine()) { }

    public ViewOnlineHumanForm(TUserEngine engine)
    {
        _engine = engine;
        InitializeComponent();
        FormCreate();
    }

    private void InitializeComponent()
    {
        Text = "在线人物";
        Width = 900;
        Height = 520;
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;

        chkHuman = new System.Windows.Forms.CheckBox { Text = "玩家", Left = 8, Top = 8, AutoSize = true, Checked = true };
        chkDummy = new System.Windows.Forms.CheckBox { Text = "假人", Left = 70, Top = 8, AutoSize = true };
        chkHumanHero = new System.Windows.Forms.CheckBox { Text = "英雄", Left = 132, Top = 8, AutoSize = true };
        chkDummyHero = new System.Windows.Forms.CheckBox { Text = "假人英雄", Left = 194, Top = 8, AutoSize = true };
        chkDummy.Click += (s, e) => chkDummyClick(s);
        chkHuman.Click += (s, e) => chkDummyClick(s);
        chkHumanHero.Click += (s, e) => chkDummyClick(s);
        chkDummyHero.Click += (s, e) => chkDummyClick(s);
        Controls.Add(chkHuman);
        Controls.Add(chkDummy);
        Controls.Add(chkHumanHero);
        Controls.Add(chkDummyHero);

        GridHuman = new System.Windows.Forms.DataGridView
        {
            Left = 8,
            Top = 32,
            Width = 870,
            Height = 360,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            RowHeadersVisible = false,
            SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false
        };
        string[] headers =
        {
            "序号", "角色名称", "角色类型", "性别", "职业", "等级", "地图", "座标", "登录帐号",
            "登录IP", "权限", "所在地区", "元宝", "游戏点", "秒卡点", "金刚石", "灵符", "离线挂机", "自动回复"
        };
        foreach (var h in headers)
            GridHuman.Columns.Add("c" + GridHuman.Columns.Count, h);
        Controls.Add(GridHuman);

        EditSearchName = new System.Windows.Forms.TextBox { Left = 8, Top = 400, Width = 150 };
        ButtonSearch = new System.Windows.Forms.Button { Text = "查找(&F)", Left = 164, Top = 398, Width = 80, Height = 24 };
        ButtonSearch.Click += (s, e) => ButtonSearchClick(s);
        ButtonView = new System.Windows.Forms.Button { Text = "查看(&V)", Left = 250, Top = 398, Width = 80, Height = 24 };
        ButtonView.Click += (s, e) => ButtonViewClick(s);
        ButtonKickPlayOffLine = new System.Windows.Forms.Button { Text = "踢离线挂机", Left = 336, Top = 398, Width = 90, Height = 24 };
        ButtonKickPlayOffLine.Click += (s, e) => ButtonKickPlayOffLineClick(s);
        ButtonKickDummyObject = new System.Windows.Forms.Button { Text = "踢假人", Left = 432, Top = 398, Width = 80, Height = 24 };
        ButtonKickDummyObject.Click += (s, e) => ButtonKickDummyObjectClick(s);
        ButtonRefGrid = new System.Windows.Forms.Button { Text = "刷新(&R)", Left = 518, Top = 398, Width = 80, Height = 24 };
        ButtonRefGrid.Click += (s, e) => ButtonRefGridClick(s);
        Controls.Add(EditSearchName);
        Controls.Add(ButtonSearch);
        Controls.Add(ButtonView);
        Controls.Add(ButtonKickPlayOffLine);
        Controls.Add(ButtonKickDummyObject);
        Controls.Add(ButtonRefGrid);

        PanelStatus = new System.Windows.Forms.Panel { Left = 8, Top = 430, Width = 870, Height = 24, BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D };
        Controls.Add(PanelStatus);

        Timer = new System.Windows.Forms.Timer { Interval = 1000 };
        Timer.Tick += (s, e) => TimerTimer(s);
    }

    /// <summary>Delphi FormCreate：19 列表头，12-16 列用 g_Config 元宝族名称（1:1）。</summary>
    public void FormCreate()
    {
        GridHuman.Columns[12].HeaderText = M2Config.sGameGoldName;
        GridHuman.Columns[13].HeaderText = M2Config.sGamePointName;
        GridHuman.Columns[14].HeaderText = M2Config.sPayMentPointName;
        GridHuman.Columns[15].HeaderText = M2Config.sGameDiamondName;
        GridHuman.Columns[16].HeaderText = M2Config.sGameGirdName;
    }

    // ================= Delphi 1:1 =================

    /// <summary>Delphi Open()；showModal=false 供测试/宿主复用。</summary>
    public void Open(bool showModal = true)
    {
        dwTimeOutTick = DelphiRTL.GetTickCount();
        GetOnlineList();
        RefGridSession();
        Timer.Enabled = true;
        if (showModal)
            ShowDialog();
        Timer.Enabled = false;
    }

    public void GetOnlineList()
    {
        ViewList.Clear();
        if (chkHumanHero.Checked || chkDummyHero.Checked)
        {
            foreach (var hero in _engine.HeroObjects)
            {
                if (hero.m_boDummyObject)
                {
                    if (chkDummyHero.Checked)
                        ViewList.AddObject(hero.m_sCharName, hero);
                }
                else
                {
                    if (chkHumanHero.Checked)
                        ViewList.AddObject(hero.m_sCharName, hero);
                }
            }
        }

        if (chkDummy.Checked || chkHuman.Checked)
        {
            foreach (var playObject in _engine.PlayObjects)
            {
                if (playObject.m_boDummyObject)
                {
                    if (chkDummy.Checked)
                        ViewList.AddObject(playObject.m_sCharName, playObject);
                }
                else
                {
                    if (chkHuman.Checked)
                        ViewList.AddObject(playObject.m_sCharName, playObject);
                }
            }
        }
    }

    public void RefGridSession()
    {
        PanelStatus.Text = "正在取得数据...";
        GridHuman.Rows.Clear();
        GridHuman.Rows.Add(ViewList.Count <= 0 ? 1 : ViewList.Count);

        for (int i = 0; i < ViewList.Count; i++)
        {
            var curObj = (TCreature)ViewList.GetObject(i)!;
            GridHuman[0, i].Value = i.ToString();
            GridHuman[1, i].Value = curObj.m_sCharName;
            GridHuman[3, i].Value = M2ShareFuncs.IntToSex(curObj.m_btGender);
            GridHuman[4, i].Value = M2ShareFuncs.IntToJob(curObj.m_btJob);
            GridHuman[5, i].Value = curObj.m_wAbil.Level.ToString();
            GridHuman[6, i].Value = curObj.m_sMapName;
            GridHuman[7, i].Value = curObj.m_nCurrX + ":" + curObj.m_nCurrY;

            if (curObj.m_btRace == Grobal2Const.RC_HEROOBJECT)
            {
                GridHuman[2, i].Value = curObj.m_boDummyObject ? "假人英雄" : "玩家英雄";
                GridHuman[8, i].Value = "";
                if (curObj.m_Master != null)
                {
                    var master = (TPlayObject)curObj.m_Master;
                    GridHuman[9, i].Value = master.m_sIPaddr;
                    GridHuman[10, i].Value = master.m_btPermission.ToString();
                    GridHuman[11, i].Value = master.m_sIPLocal;
                }
                else
                {
                    GridHuman[9, i].Value = "";
                    GridHuman[10, i].Value = "";
                    GridHuman[11, i].Value = "";
                }
                GridHuman[12, i].Value = "0";
                GridHuman[13, i].Value = "0";
                GridHuman[14, i].Value = "0";
                GridHuman[15, i].Value = "0";
                GridHuman[16, i].Value = "0";
                GridHuman[17, i].Value = "否";
                GridHuman[18, i].Value = "";
            }
            else if (curObj.m_btRace == Grobal2Const.RC_PLAYOBJECT)
            {
                var playObject = (TPlayObject)curObj;
                GridHuman[2, i].Value = playObject.m_boDummyObject ? "假人" : "玩家";
                GridHuman[8, i].Value = playObject.m_sUserID;
                GridHuman[9, i].Value = playObject.m_sIPaddr;
                GridHuman[10, i].Value = playObject.m_btPermission.ToString();
                GridHuman[11, i].Value = playObject.m_sIPLocal;
                GridHuman[12, i].Value = playObject.m_nGameGold.ToString();
                GridHuman[13, i].Value = playObject.m_nGamePoint.ToString();
                GridHuman[14, i].Value = playObject.m_nPayMentPoint.ToString();
                GridHuman[15, i].Value = playObject.m_nGameDiamond.ToString();
                GridHuman[16, i].Value = playObject.m_nGameGird.ToString();
                GridHuman[17, i].Value = playObject.m_boOffLine ? "是" : "否";
                GridHuman[18, i].Value = playObject.m_sAutoSendMsg;
            }
        }
    }

    public void ButtonRefGridClick(object? sender)
    {
        dwTimeOutTick = DelphiRTL.GetTickCount();
        GetOnlineList();
        RefGridSession();
    }

    public void SortOnlineList(int nSort)
    {
        var sortList = new GXX.Core.Util.TStringList();
        switch (nSort)
        {
            case 0:
                ViewList.Sort();
                return;
            case 1:
                for (int i = 0; i < ViewList.Count; i++)
                {
                    var obj = (TCreature)ViewList.GetObject(i)!;
                    int btType;
                    if (obj.m_btRace == Grobal2Const.RC_PLAYOBJECT)
                        btType = !obj.m_boDummyObject ? 0 : 1;
                    else
                        btType = !obj.m_boDummyObject ? 2 : 3;
                    sortList.AddObject(btType.ToString(), obj);
                }
                break;
            case 2:
                for (int i = 0; i < ViewList.Count; i++)
                    sortList.AddObject(((TCreature)ViewList.GetObject(i)!).m_btGender.ToString(), ViewList.GetObject(i));
                break;
            case 3:
                for (int i = 0; i < ViewList.Count; i++)
                    sortList.AddObject(((TCreature)ViewList.GetObject(i)!).m_btJob.ToString(), ViewList.GetObject(i));
                break;
            case 4:
                for (int i = 0; i < ViewList.Count; i++)
                    sortList.AddObject(((TCreature)ViewList.GetObject(i)!).m_wAbil.Level.ToString(), ViewList.GetObject(i));
                break;
            case 5:
                for (int i = 0; i < ViewList.Count; i++)
                    sortList.AddObject(((TCreature)ViewList.GetObject(i)!).m_sMapName, ViewList.GetObject(i));
                break;
            case 6:
                for (int i = 0; i < ViewList.Count; i++)
                {
                    var obj = (TCreature)ViewList.GetObject(i)!;
                    string sIPaddr = "";
                    if (obj.m_btRace == Grobal2Const.RC_PLAYOBJECT)
                        sIPaddr = ((TPlayObject)obj).m_sIPaddr;
                    else if (obj.m_Master != null)
                        sIPaddr = ((TPlayObject)obj.m_Master).m_sIPaddr;
                    sortList.AddObject(sIPaddr, obj);
                }
                break;
            case 7:
                for (int i = 0; i < ViewList.Count; i++)
                {
                    var obj = (TCreature)ViewList.GetObject(i)!;
                    int btPermission;
                    if (obj.m_btRace == Grobal2Const.RC_PLAYOBJECT)
                        btPermission = ((TPlayObject)obj).m_btPermission;
                    else if (obj.m_Master != null)
                        btPermission = ((TPlayObject)obj.m_Master).m_btPermission;
                    else
                        btPermission = 0;
                    sortList.AddObject(btPermission.ToString(), obj);
                }
                break;
            case 8:
                for (int i = 0; i < ViewList.Count; i++)
                {
                    var obj = (TCreature)ViewList.GetObject(i)!;
                    string sIPLocal = "";
                    if (obj.m_btRace == Grobal2Const.RC_PLAYOBJECT)
                        sIPLocal = ((TPlayObject)obj).m_sIPLocal;
                    else if (obj.m_Master != null)
                        sIPLocal = ((TPlayObject)obj.m_Master).m_sIPLocal;
                    sortList.AddObject(sIPLocal, obj);
                }
                break;
        }
        ViewList.Clear(); for (int i = 0; i < sortList.Count; i++) ViewList.AddObject(sortList[i], sortList.GetObject(i));
        ViewList.Sort();
    }

    public void ComboBoxSortClick(int itemIndex)
    {
        if (itemIndex < 0)
            return;
        dwTimeOutTick = DelphiRTL.GetTickCount();
        GetOnlineList();
        SortOnlineList(itemIndex);
        RefGridSession();
    }

    public void TimerTimer(object? sender)
    {
        if (DelphiRTL.GetTickCount() - dwTimeOutTick > 100000 && ViewList.Count > 0)
        {
            ViewList.Clear();
            RefGridSession();
        }
    }

    public void ButtonSearchClick(object? sender)
    {
        string sHumanName = EditSearchName.Text.Trim();
        if (sHumanName == "")
        {
            M2Forms.ErrorBox("请输入一个角色名称！");
            return;
        }

        for (int i = 0; i < ViewList.Count; i++)
        {
            var baseObject = (TCreature)ViewList.GetObject(i)!;
            if (string.Equals(baseObject.m_sCharName, sHumanName, StringComparison.OrdinalIgnoreCase))
            {
                GridSelectedRow = i + 1; // Delphi GridHuman.Row := I + 1
                return;
            }
        }
        M2Forms.MessageBox("角色没有在线！", "提示信息", M2Forms.MB_OK | 0x40 /*MB_ICONINFORMATION*/);
    }

    public void ButtonViewClick(object? sender)
    {
        ShowHumanInfo();
    }

    public void ShowHumanInfo()
    {
        int nSelIndex = GridSelectedRow;
        nSelIndex--;
        if (nSelIndex < 0 || ViewList.Count <= nSelIndex)
        {
            M2Forms.MessageBox("请先选择一个要查看的角色！", "提示信息", M2Forms.MB_OK | 0x40);
            return;
        }
        string sPlayObjectName = ViewList[nSelIndex];
        var baseObject = (TCreature?)ViewList.GetObject(nSelIndex);

        if (baseObject == null)
        {
            M2Forms.MessageBox(sPlayObjectName + " 此角色已经不在线！", "提示信息", M2Forms.MB_OK | 0x40);
            return;
        }

        ShowHumanInfoHandler?.Invoke(baseObject); // frmHumanInfo.BaseObject := ...; frmHumanInfo.Open()
    }

    public void ButtonKickPlayOffLineClick(object? sender)
    {
        lock (_engine)
        {
            foreach (var player in _engine.PlayObjects)
            {
                if (player.m_boOffLine && !M2ShareGlobals.SearchSellPlayer(player.m_sCharName, out int _))
                    player.MakeGhost();
            }
        }

        dwTimeOutTick = DelphiRTL.GetTickCount();
        GetOnlineList();
        RefGridSession();
    }

    public void ButtonKickDummyObjectClick(object? sender)
    {
        lock (_engine)
        {
            foreach (var player in _engine.PlayObjects)
            {
                if (player.m_boDummyObject)
                    player.MakeGhost();
            }
        }

        dwTimeOutTick = DelphiRTL.GetTickCount();
        GetOnlineList();
        RefGridSession();
    }

    public void chkDummyClick(object? sender)
    {
        dwTimeOutTick = DelphiRTL.GetTickCount();
        GetOnlineList();
        RefGridSession();
    }
}
