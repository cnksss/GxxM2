using GXX.M2Server.Engine;

namespace GXX.M2Server.Forms;

/// <summary>
/// CastleManage.pas TfrmCastleManage 1:1 转换（城堡管理：基本信息/守卫设置/攻城设置三页）。
/// ListView 对象挂载用 Tag 等效 Delphi SubItems.Objects[0]；SpinEditEx → NumericUpDown。
/// </summary>
public sealed class CastleManageForm : System.Windows.Forms.Form
{
    private bool boRefing;

    // ---- 全局（Delphi 单元级变量 1:1） ----
    public static TAttackerInfo? SelAttackGuildInfo;
    public static TUserCastle? CurCastle;
    public static CastleManageForm? frmCastleManage;

    public System.Windows.Forms.GroupBox GroupBox1 = null!;
    public System.Windows.Forms.ListView ListViewCastle = null!;
    public System.Windows.Forms.TabPage TabSheet1 = null!;
    public System.Windows.Forms.TabPage TabSheet3 = null!;
    public System.Windows.Forms.TabPage TabSheet4 = null!;
    public System.Windows.Forms.TextBox EditOwenGuildName = null!;
    public System.Windows.Forms.NumericUpDown EditTotalGold = null!;
    public System.Windows.Forms.NumericUpDown EditTodayIncome = null!;
    public System.Windows.Forms.NumericUpDown EditTechLevel = null!;
    public System.Windows.Forms.NumericUpDown EditPower = null!;
    public System.Windows.Forms.TextBox EditCastleName = null!;
    public System.Windows.Forms.TextBox EditCastleOfGuild = null!;
    public System.Windows.Forms.TextBox EditHomeMap = null!;
    public System.Windows.Forms.NumericUpDown SpinEditNomeX = null!;
    public System.Windows.Forms.NumericUpDown SpinEditNomeY = null!;
    public System.Windows.Forms.TextBox EditTunnelMap = null!;
    public System.Windows.Forms.TextBox EditPalace = null!;
    public System.Windows.Forms.TextBox EditWarStatus = null!;
    public System.Windows.Forms.Button ButtonSave = null!;
    public System.Windows.Forms.Button ButtonStartWar = null!;
    public System.Windows.Forms.Button ButtonStopWar = null!;
    public System.Windows.Forms.ListView ListViewGuard = null!;
    public System.Windows.Forms.Button ButtonRefresh = null!;
    public System.Windows.Forms.ListView ListViewAttackSabukWall = null!;
    public System.Windows.Forms.Button ButtonAttackAdd = null!;
    public System.Windows.Forms.Button ButtonAttackEdit = null!;
    public System.Windows.Forms.Button ButtonAttackDel = null!;
    public System.Windows.Forms.Button ButtonRefAttackSabukWall = null!;
    public System.Windows.Forms.Timer Timer1 = null!;

    public CastleManageForm()
    {
        InitializeComponent();
        frmCastleManage = this;
    }

    private static System.Windows.Forms.ListView MakeListView(params (string text, int width)[] cols)
    {
        var lv = new System.Windows.Forms.ListView
        {
            View = System.Windows.Forms.View.Details,
            FullRowSelect = true,
            HideSelection = false
        };
        foreach (var (text, width) in cols)
            lv.Columns.Add(text, width);
        return lv;
    }

    private void InitializeComponent()
    {
        Text = "城堡管理";
        Width = 700;
        Height = 520;
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;

        GroupBox1 = new System.Windows.Forms.GroupBox { Text = "城堡列表", Left = 8, Top = 8, Width = 210, Height = 460 };
        ListViewCastle = MakeListView(("序号", 50), ("目录", 80), ("名称", 60));
        ListViewCastle.Bounds = new System.Drawing.Rectangle(8, 20, 190, 420);
        ListViewCastle.Click += (s, e) => ListViewCastleClick(s);
        GroupBox1.Controls.Add(ListViewCastle);
        Controls.Add(GroupBox1);

        var tabs = new System.Windows.Forms.TabControl { Left = 224, Top = 8, Width = 452, Height = 460 };
        TabSheet1 = new System.Windows.Forms.TabPage { Text = "基本信息" };
        TabSheet3 = new System.Windows.Forms.TabPage { Text = "守卫设置" };
        TabSheet4 = new System.Windows.Forms.TabPage { Text = "攻城设置" };
        tabs.TabPages.AddRange(new[] { TabSheet1, TabSheet3, TabSheet4 });
        Controls.Add(tabs);

        // ---- 基本信息 ----
        int y = 10;
        AddText(TabSheet1, "所属行会:", y, out EditOwenGuildName); y += 30;
        AddSpin(TabSheet1, "总金币:", y, out EditTotalGold); y += 30;
        AddSpin(TabSheet1, "今日收入:", y, out EditTodayIncome); y += 30;
        AddSpin(TabSheet1, "技术等级:", y, out EditTechLevel); y += 30;
        AddSpin(TabSheet1, "发展度:", y, out EditPower); y += 34;
        AddText(TabSheet1, "城堡名称:", y, out EditCastleName); y += 30;
        AddText(TabSheet1, "占领行会:", y, out EditCastleOfGuild); y += 30;
        AddText(TabSheet1, "城堡地图:", y, out EditHomeMap); y += 30;
        AddText(TabSheet1, "皇宫地图:", y, out EditPalace); y += 30;
        AddSpin(TabSheet1, "城堡坐标X:", y, out SpinEditNomeX); y += 30;
        AddSpin(TabSheet1, "城堡坐标Y:", y, out SpinEditNomeY); y += 30;
        AddText(TabSheet1, "密道地图:", y, out EditTunnelMap); y += 34;
        AddText(TabSheet1, "状态:", y, out EditWarStatus);

        ButtonSave = new System.Windows.Forms.Button { Text = "保存(&S)", Left = 240, Top = 384, Width = 80, Height = 26 };
        ButtonSave.Click += (s, e) => ButtonSaveClick(s);
        ButtonStartWar = new System.Windows.Forms.Button { Text = "开始攻城", Left = 328, Top = 384, Width = 80, Height = 26 };
        ButtonStartWar.Click += (s, e) => ButtonStartWarClick(s);
        ButtonStopWar = new System.Windows.Forms.Button { Text = "停止攻城", Left = 414, Top = 384, Width = 80, Height = 26 };
        ButtonStopWar.Click += (s, e) => ButtonStopWarClick(s);
        TabSheet1.Controls.Add(ButtonSave);
        TabSheet1.Controls.Add(ButtonStartWar);
        TabSheet1.Controls.Add(ButtonStopWar);

        // ---- 守卫设置 ----
        ListViewGuard = MakeListView(("序号", 50), ("名称", 170), ("坐标", 130), ("HP", 130), ("状态", 70));
        ListViewGuard.Bounds = new System.Drawing.Rectangle(8, 8, 430, 370);
        ButtonRefresh = new System.Windows.Forms.Button { Text = "刷新(&R)", Left = 8, Top = 386, Width = 90, Height = 26 };
        ButtonRefresh.Click += (s, e) => ButtonRefreshClick(s);
        TabSheet3.Controls.Add(ListViewGuard);
        TabSheet3.Controls.Add(ButtonRefresh);

        // ---- 攻城设置 ----
        ListViewAttackSabukWall = MakeListView(("序号", 50), ("行会名称", 200), ("攻城时间", 150));
        ListViewAttackSabukWall.Bounds = new System.Drawing.Rectangle(8, 8, 430, 330);
        ListViewAttackSabukWall.Click += (s, e) => ListViewAttackSabukWallClick(s);
        ButtonAttackAdd = new System.Windows.Forms.Button { Text = "增加(&A)", Left = 8, Top = 346, Width = 90, Height = 26, Enabled = false };
        ButtonAttackAdd.Click += (s, e) => ButtonAttackAddClick(s);
        ButtonAttackEdit = new System.Windows.Forms.Button { Text = "编辑(&E)", Left = 106, Top = 346, Width = 90, Height = 26, Enabled = false };
        ButtonAttackEdit.Click += (s, e) => ButtonAttackEditClick(s);
        ButtonAttackDel = new System.Windows.Forms.Button { Text = "删除(&D)", Left = 204, Top = 346, Width = 90, Height = 26, Enabled = false };
        ButtonAttackDel.Click += (s, e) => ButtonAttackDelClick(s);
        ButtonRefAttackSabukWall = new System.Windows.Forms.Button { Text = "刷新(&R)", Left = 302, Top = 346, Width = 90, Height = 26, Enabled = false };
        ButtonRefAttackSabukWall.Click += (s, e) => ButtonRefAttackSabukWallClick(s);
        TabSheet4.Controls.Add(ListViewAttackSabukWall);
        TabSheet4.Controls.Add(ButtonAttackAdd);
        TabSheet4.Controls.Add(ButtonAttackEdit);
        TabSheet4.Controls.Add(ButtonAttackDel);
        TabSheet4.Controls.Add(ButtonRefAttackSabukWall);

        Timer1 = new System.Windows.Forms.Timer { Interval = 200 };
        Timer1.Tick += (s, e) => Timer1Timer(s);
    }

    private static void AddText(System.Windows.Forms.Control parent, string caption, int top, out System.Windows.Forms.TextBox edit)
    {
        var lb = new System.Windows.Forms.Label { Text = caption, Left = 16, Top = top + 4, AutoSize = true };
        edit = new System.Windows.Forms.TextBox { Left = 110, Top = top, Width = 190 };
        parent.Controls.Add(lb);
        parent.Controls.Add(edit);
    }

    private static void AddSpin(System.Windows.Forms.Control parent, string caption, int top, out System.Windows.Forms.NumericUpDown edit)
    {
        var lb = new System.Windows.Forms.Label { Text = caption, Left = 16, Top = top + 4, AutoSize = true };
        edit = new System.Windows.Forms.NumericUpDown { Left = 110, Top = top, Width = 120, Maximum = 2000000000 };
        parent.Controls.Add(lb);
        parent.Controls.Add(edit);
    }

    // ================= Delphi 1:1 =================

    public void Open(bool showModal = true)
    {
        ButtonSave.Enabled = true;
        ((System.Windows.Forms.TabControl)TabSheet1.Parent!).SelectedIndex = 0;
        SelAttackGuildInfo = null;
        RefCastleList();
        Timer1.Enabled = true;
        if (showModal)
            ShowDialog();
    }

    public void RefCastleInfo()
    {
        if (CurCastle == null)
            return;
        boRefing = true;
        EditOwenGuildName.Text = CurCastle.m_MasterGuild == null ? "" : CurCastle.m_MasterGuild.sGuildName;
        EditTotalGold.Value = CurCastle.m_nTotalGold;
        EditTodayIncome.Value = CurCastle.m_nTodayIncome;
        EditTechLevel.Value = CurCastle.m_nTechLevel;
        EditPower.Value = CurCastle.m_nPower;
        RefGuardList();
        EditCastleName.Text = CurCastle.m_sName;
        EditCastleOfGuild.Text = CurCastle.m_MasterGuild?.sGuildName ?? "";
        EditPalace.Text = CurCastle.m_sPalaceMap;
        EditHomeMap.Text = CurCastle.m_sHomeMap;
        SpinEditNomeX.Value = CurCastle.m_nHomeX;
        SpinEditNomeY.Value = CurCastle.m_nHomeY;
        EditTunnelMap.Text = CurCastle.m_sSecretMap;

        ButtonStartWar.Enabled = !CurCastle.m_boUnderWar;
        ButtonStopWar.Enabled = !ButtonStartWar.Enabled;

        ButtonAttackAdd.Enabled = true;
        ButtonRefAttackSabukWall.Enabled = true;

        EditWarStatus.Text = CurCastle.m_boUnderWar ? "攻城中..." : "停战中...";

        RefCastleAttackSabukWall();
        boRefing = false;
    }

    /// <summary>守卫列表（RefCastleInfo 内嵌循环 1:1：城门 0/左墙 1/中墙 2/右墙 3/弓箭手 +4/卫兵 +4）。</summary>
    private void RefGuardList()
    {
        ListViewGuard.Items.Clear();
        AddGuardRow("0", CastleManageRow.Of(CurCastle!.m_MainDoor));
        AddGuardRow("1", CastleManageRow.Of(CurCastle.m_LeftWall));
        AddGuardRow("2", CastleManageRow.Of(CurCastle.m_CenterWall));
        AddGuardRow("3", CastleManageRow.Of(CurCastle.m_RightWall));
        for (int i = 0; i < CurCastle.m_Archer.Length; i++)
            AddGuardRow((i + 4).ToString(), CastleManageRow.Of(CurCastle.m_Archer[i]));
        for (int i = 0; i < CurCastle.m_Guard.Length; i++)
            AddGuardRow((i + 4).ToString(), CastleManageRow.Of(CurCastle.m_Guard[i]));
    }

    private void AddGuardRow(string caption, CastleManageRow row)
    {
        var item = new System.Windows.Forms.ListViewItem(caption);
        item.SubItems.Add(row.Name);
        item.SubItems.Add(row.Pos);
        item.SubItems.Add(row.Hp);
        item.SubItems.Add(row.Status);
        ListViewGuard.Items.Add(item);
    }

    public void RefCastleList()
    {
        CastleState.g_CastleManager.Lock();
        try
        {
            for (int i = 0; i < CastleState.g_CastleManager.m_CastleList.Count; i++)
            {
                var userCastle = CastleState.g_CastleManager.m_CastleList[i];
                var item = new System.Windows.Forms.ListViewItem(i.ToString()) { Tag = userCastle };
                item.SubItems.Add(userCastle.m_sConfigDir);
                item.SubItems.Add(userCastle.m_sName);
                ListViewCastle.Items.Add(item);
            }
        }
        finally
        {
            CastleState.g_CastleManager.UnLock();
        }
    }

    public void RefCastleAttackSabukWall()
    {
        if (CurCastle == null)
            return;
        ListViewAttackSabukWall.Items.Clear();
        for (int i = 0; i < CurCastle.m_AttackWarList.Count; i++)
        {
            var attackerInfo = CurCastle.m_AttackWarList[i];
            var item = new System.Windows.Forms.ListViewItem(i.ToString()) { Tag = attackerInfo };
            item.SubItems.Add(attackerInfo.sGuildName);
            item.SubItems.Add(TUserCastle.DelphiDateToStr(attackerInfo.AttackDate));
            ListViewAttackSabukWall.Items.Add(item);
        }
    }

    /// <summary>Delphi ListView.Selected 等效取行（无句柄环境下 SelectedItems 为空，按 Selected 标志回退）。</summary>
    private static System.Windows.Forms.ListViewItem? GetSelected(System.Windows.Forms.ListView lv)
    {
        if (lv.SelectedItems.Count > 0)
            return lv.SelectedItems[0];
        foreach (System.Windows.Forms.ListViewItem it in lv.Items)
            if (it.Selected) return it;
        return null;
    }

    public void ListViewCastleClick(object? sender)
    {
        CurCastle = null;
        var item = GetSelected(ListViewCastle);
        if (item == null)
            return;
        CurCastle = item.Tag as TUserCastle;
        RefCastleInfo();
    }

    public void ButtonRefreshClick(object? sender)
    {
        RefCastleInfo();
    }

    public void ButtonAttackAddClick(object? sender)
    {
        var frm = new AttackSabukWallForm();
        AttackSabukWallForm.FrmAttackSabukWall = frm; // Delphi: FrmAttackSabukWall := TFrmAttackSabukWall.Create(Owner)
        AttackSabukWallForm.frmCastleManage = this;
        frm.Open(true, showModal: false);
    }

    public void ButtonAttackEditClick(object? sender)
    {
        if (CurCastle == null)
            return;
        if (SelAttackGuildInfo == null)
            return;
        var frm = new AttackSabukWallForm();
        AttackSabukWallForm.FrmAttackSabukWall = frm;
        AttackSabukWallForm.frmCastleManage = this;
        frm.Open(false, showModal: false);
    }

    public void ListViewAttackSabukWallClick(object? sender)
    {
        ButtonAttackEdit.Enabled = false;
        ButtonAttackDel.Enabled = false;
        SelAttackGuildInfo = null;
        var item = GetSelected(ListViewAttackSabukWall);
        if (item == null)
            return;
        SelAttackGuildInfo = item.Tag as TAttackerInfo;
        ButtonAttackEdit.Enabled = true;
        ButtonAttackDel.Enabled = true;
    }

    public void ButtonRefAttackSabukWallClick(object? sender)
    {
        if (CurCastle == null)
            return;
        RefCastleAttackSabukWall();
    }

    public void ButtonAttackDelClick(object? sender)
    {
        if (CurCastle == null)
            return;
        if (SelAttackGuildInfo == null)
            return;
        if (M2Forms.MessageBox("是否确认删除此行会攻城申请？" + "\r\n\r\n" + "行会名称：" + SelAttackGuildInfo.sGuildName + "\r\n" + "攻城时间：" + TUserCastle.DelphiDateToStr(SelAttackGuildInfo.AttackDate),
                "确认信息", M2Forms.MB_YESNO | M2Forms.MB_ICONQUESTION) == M2Forms.IDYES)
        {
            for (int i = CurCastle.m_AttackWarList.Count - 1; i >= 0; i--)
            {
                var attackerInfo = CurCastle.m_AttackWarList[i];
                if (attackerInfo == SelAttackGuildInfo)
                {
                    CurCastle.m_AttackWarList.RemoveAt(i);
                    CurCastle.Save();
                    SelAttackGuildInfo = null;
                    break;
                }
            }
            RefCastleAttackSabukWall();
        }
    }

    public void ButtonSaveClick(object? sender)
    {
        if (CurCastle == null)
            return;
        CurCastle.m_sHomeMap = EditHomeMap.Text;
        CurCastle.m_sPalaceMap = EditPalace.Text;
        CurCastle.m_sHomeMap = EditHomeMap.Text; // Delphi 原句重复赋值，1:1 保留
        CurCastle.m_nHomeX = (int)SpinEditNomeX.Value;
        CurCastle.m_nHomeY = (int)SpinEditNomeY.Value;
        CurCastle.m_sSecretMap = EditTunnelMap.Text;
        CurCastle.Save();
        ButtonSave.Enabled = false;
    }

    public void ButtonStartWarClick(object? sender)
    {
        if (CurCastle != null)
        {
            if (!CurCastle.m_boUnderWar)
            {
                foreach (var guild in CastleState.g_GuildManager.GuildList)
                    CurCastle.AddAttackerInfo(guild, 0);
                CurCastle.Save();
                CurCastle.m_boStartWar = false;
                CurCastle.StartWar(DateTime.Now);
                RefCastleInfo();
            }
        }
    }

    public void ButtonStopWarClick(object? sender)
    {
        if (CurCastle != null)
        {
            if (CurCastle.m_boUnderWar)
            {
                CurCastle.StopWar();
                RefCastleInfo();
            }
        }
    }

    public void Timer1Timer(object? sender)
    {
        Timer1.Enabled = false;
        if (ListViewCastle.SelectedItems.Count == 0 && ListViewCastle.Items.Count > 0)
        {
            ListViewCastle.Items[0].Selected = true;
            ListViewCastleClick(this);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (frmCastleManage == this) frmCastleManage = null;
        base.Dispose(disposing);
    }
}

/// <summary>守卫列表行（Delphi RefCastleInfo 行组装 1:1；TCastleDoor 状态列待 ObjMon2 批次接入）。</summary>
internal readonly struct CastleManageRow
{
    public readonly string Name;
    public readonly string Pos;
    public readonly string Hp;
    public readonly string Status;

    private CastleManageRow(string name, string pos, string hp, string status)
    {
        Name = name; Pos = pos; Hp = hp; Status = status;
    }

    public static CastleManageRow Of(TObjUnit u)
    {
        if (u.BaseObj != null)
            return new CastleManageRow(
                u.BaseObj.m_sCharName,
                $"{u.BaseObj.m_nCurrX}:{u.BaseObj.m_nCurrY}",
                $"{u.BaseObj.m_wAbil.HP}/{u.BaseObj.m_wAbil.MaxHP}",
                "");
        return new CastleManageRow(u.sName, $"{u.nX}:{u.nY}", "0/0", "");
    }
}
