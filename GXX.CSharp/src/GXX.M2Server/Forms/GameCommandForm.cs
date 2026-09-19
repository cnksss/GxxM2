using GXX.M2Server.Engine;

namespace GXX.M2Server.Forms;

/// <summary>
/// GameCommand.pas TfrmGameCmd 命令配置窗体（批次J32，1016 行）：
/// 用户命令页完整 1:1 —— AddUserCommandToList 组标题 + 45 条命令表、命令名/权限编辑
/// （edtUserCmdNameChange/seUserCmdPermissionChange 使能 OK/Save）、btnUserCmdOKClick
/// 空名校验弹窗、btnUserCmdSaveClick 写 CommandConf 'Command' 节 42 键 + 'Permission' 节
/// Date/PrvMsg/AllowMsg 3 键（原文仅写前三项权限，保留）。
/// 管理员/调试命令两表随余页接入（AddAdminCommandToList/AddDebugCommandToList）。
/// </summary>
public sealed class GameCommandForm : System.Windows.Forms.Form
{
    public System.Windows.Forms.ListBox ListBoxUserCmd = null!;
    public System.Windows.Forms.TextBox edtUserCmdName = null!;
    public System.Windows.Forms.NumericUpDown seUserCmdPermission = null!;
    public System.Windows.Forms.Button ButtonUserCmdOK = null!;
    public System.Windows.Forms.Button ButtonUserCmdSave = null!;

    // ---- 管理员/调试命令页控件（批次J33） ----
    public System.Windows.Forms.ListBox ListBoxAdminCmd = null!;
    public System.Windows.Forms.TextBox edtAdminCmdName = null!;
    public System.Windows.Forms.NumericUpDown seAdminCmdPermission = null!;
    public System.Windows.Forms.Button ButtonAdminCmdOK = null!;
    public System.Windows.Forms.Button ButtonAdminCmdSave = null!;
    public System.Windows.Forms.ListBox ListBoxDebugCmd = null!;
    public System.Windows.Forms.TextBox edtDebugCmdName = null!;
    public System.Windows.Forms.NumericUpDown seDebugCmdPermission = null!;
    public System.Windows.Forms.Button ButtonDebugCmdOK = null!;
    public System.Windows.Forms.Button ButtonDebugCmdSave = null!;

    /// <summary>用户命令行数据（Delphi vstUserCmd NodeData 等效）。</summary>
    public List<GameCommandRow> UserCommandRows = new();

    /// <summary>管理员/调试命令行数据（批次J33）。</summary>
    public List<GameCommandRow> AdminCommandRows = new();
    public List<GameCommandRow> DebugCommandRows = new();

    public GameCommandForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Text = "游戏命令设置";
        Width = 720;
        Height = 520;
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        var gb = new System.Windows.Forms.GroupBox { Text = "用户命令", Left = 8, Top = 8, Width = 690, Height = 430 };
        Controls.Add(gb);
        ListBoxUserCmd = new System.Windows.Forms.ListBox { Left = 10, Top = 18, Width = 430, Height = 395 };
        ListBoxUserCmd.SelectedIndexChanged += (s, e) => ListBoxUserCmdFocusChanged(s);
        gb.Controls.Add(ListBoxUserCmd);
        gb.Controls.Add(new System.Windows.Forms.Label { Text = "命令名称:", Left = 452, Top = 22, AutoSize = true });
        edtUserCmdName = new System.Windows.Forms.TextBox { Left = 530, Top = 18, Width = 140 };
        edtUserCmdName.TextChanged += (s, e) => edtUserCmdNameChange(s);
        gb.Controls.Add(edtUserCmdName);
        gb.Controls.Add(new System.Windows.Forms.Label { Text = "最小权限:", Left = 452, Top = 52, AutoSize = true });
        seUserCmdPermission = new System.Windows.Forms.NumericUpDown { Left = 530, Top = 48, Width = 70, Maximum = 10 };
        seUserCmdPermission.ValueChanged += (s, e) => seUserCmdPermissionChange(s);
        gb.Controls.Add(seUserCmdPermission);
        ButtonUserCmdOK = new System.Windows.Forms.Button { Text = "应用(&A)", Left = 452, Top = 90, Width = 100, Height = 26, Enabled = false };
        ButtonUserCmdOK.Click += (s, e) => btnUserCmdOKClick(s);
        gb.Controls.Add(ButtonUserCmdOK);
        ButtonUserCmdSave = new System.Windows.Forms.Button { Text = "保存(&S)", Left = 452, Top = 122, Width = 100, Height = 26, Enabled = false };
        ButtonUserCmdSave.Click += (s, e) => ButtonUserCmdSaveClick(s);
        gb.Controls.Add(ButtonUserCmdSave);

        // ---- 管理员命令页（批次J33） ----
        var gbAdmin = new System.Windows.Forms.GroupBox { Text = "管理员命令", Left = 8, Top = 444, Width = 690, Height = 200 };
        Controls.Add(gbAdmin);
        ListBoxAdminCmd = new System.Windows.Forms.ListBox { Left = 10, Top = 18, Width = 430, Height = 165 };
        ListBoxAdminCmd.SelectedIndexChanged += (s, e) => ListBoxAdminCmdFocusChanged(s);
        gbAdmin.Controls.Add(ListBoxAdminCmd);
        gbAdmin.Controls.Add(new System.Windows.Forms.Label { Text = "命令名称:", Left = 452, Top = 22, AutoSize = true });
        edtAdminCmdName = new System.Windows.Forms.TextBox { Left = 530, Top = 18, Width = 140 };
        edtAdminCmdName.TextChanged += (s, e) => edtAdminCmdNameChange(s);
        gbAdmin.Controls.Add(edtAdminCmdName);
        gbAdmin.Controls.Add(new System.Windows.Forms.Label { Text = "最小权限:", Left = 452, Top = 52, AutoSize = true });
        seAdminCmdPermission = new System.Windows.Forms.NumericUpDown { Left = 530, Top = 48, Width = 70, Maximum = 10 };
        seAdminCmdPermission.ValueChanged += (s, e) => seAdminCmdPermissionChange(s);
        gbAdmin.Controls.Add(seAdminCmdPermission);
        ButtonAdminCmdOK = new System.Windows.Forms.Button { Text = "应用(&A)", Left = 452, Top = 90, Width = 100, Height = 26, Enabled = false };
        ButtonAdminCmdOK.Click += (s, e) => btnAdminCmdOKClick(s);
        gbAdmin.Controls.Add(ButtonAdminCmdOK);
        ButtonAdminCmdSave = new System.Windows.Forms.Button { Text = "保存(&S)", Left = 452, Top = 122, Width = 100, Height = 26, Enabled = false };
        ButtonAdminCmdSave.Click += (s, e) => ButtonAdminCmdSaveClick(s);
        gbAdmin.Controls.Add(ButtonAdminCmdSave);

        // ---- 调试命令页（批次J33） ----
        var gbDebug = new System.Windows.Forms.GroupBox { Text = "调试命令", Left = 8, Top = 648, Width = 690, Height = 200 };
        Controls.Add(gbDebug);
        ListBoxDebugCmd = new System.Windows.Forms.ListBox { Left = 10, Top = 18, Width = 430, Height = 165 };
        ListBoxDebugCmd.SelectedIndexChanged += (s, e) => ListBoxDebugCmdFocusChanged(s);
        gbDebug.Controls.Add(ListBoxDebugCmd);
        gbDebug.Controls.Add(new System.Windows.Forms.Label { Text = "命令名称:", Left = 452, Top = 22, AutoSize = true });
        edtDebugCmdName = new System.Windows.Forms.TextBox { Left = 530, Top = 18, Width = 140 };
        edtDebugCmdName.TextChanged += (s, e) => edtDebugCmdNameChange(s);
        gbDebug.Controls.Add(edtDebugCmdName);
        gbDebug.Controls.Add(new System.Windows.Forms.Label { Text = "最小权限:", Left = 452, Top = 52, AutoSize = true });
        seDebugCmdPermission = new System.Windows.Forms.NumericUpDown { Left = 530, Top = 48, Width = 70, Maximum = 10 };
        seDebugCmdPermission.ValueChanged += (s, e) => seDebugCmdPermissionChange(s);
        gbDebug.Controls.Add(seDebugCmdPermission);
        ButtonDebugCmdOK = new System.Windows.Forms.Button { Text = "应用(&A)", Left = 452, Top = 90, Width = 100, Height = 26, Enabled = false };
        ButtonDebugCmdOK.Click += (s, e) => btnDebugCmdOKClick(s);
        gbDebug.Controls.Add(ButtonDebugCmdOK);
        ButtonDebugCmdSave = new System.Windows.Forms.Button { Text = "保存(&S)", Left = 452, Top = 122, Width = 100, Height = 26, Enabled = false };
        ButtonDebugCmdSave.Click += (s, e) => ButtonDebugCmdSaveClick(s);
        gbDebug.Controls.Add(ButtonDebugCmdSave);
    }

    // ================= Delphi 1:1 =================

    /// <summary>Delphi Open()：构建用户/管理员/调试三命令表。</summary>
    public void Open(bool showModal = true)
    {
        AddUserCommandToList();
        AddAdminCommandToList();
        AddDebugCommandToList();
        if (showModal)
            ShowDialog();
    }

    /// <summary>AddUserCommandToList 1:1（组标题行 Index=0；命令行 Index 从 1 递增）。</summary>
    public void AddUserCommandToList()
    {
        UserCommandRows = GameCommandState.BuildUserCommandList();
        ListBoxUserCmd.Items.Clear();
        foreach (var row in UserCommandRows)
        {
            ListBoxUserCmd.Items.Add(row.IsGroup
                ? row.GroupText
                : row.Cmd!.sCmd + "(" + row.Cmd.sCmd + ") " + row.Param + " " + row.Desc);
        }
        ButtonUserCmdOK.Enabled = false;
        ButtonUserCmdSave.Enabled = false;
    }

    private GameCommandRow? FocusedRow =>
        ListBoxUserCmd.SelectedIndex >= 0 && ListBoxUserCmd.SelectedIndex < UserCommandRows.Count
            ? UserCommandRows[ListBoxUserCmd.SelectedIndex]
            : null;

    public void ListBoxUserCmdFocusChanged(object? sender)
    {
        var row = FocusedRow;
        if (row == null || row.IsGroup || row.Cmd == null)
            return;
        edtUserCmdName.Text = row.Cmd.sCmd;
        seUserCmdPermission.Value = row.Cmd.nPermissionMin;
    }

    public void edtUserCmdNameChange(object? sender)
    {
        ButtonUserCmdOK.Enabled = true;
        ButtonUserCmdSave.Enabled = true;
    }

    public void seUserCmdPermissionChange(object? sender)
    {
        ButtonUserCmdOK.Enabled = true;
        ButtonUserCmdSave.Enabled = true;
    }

    public void btnUserCmdOKClick(object? sender)
    {
        var row = FocusedRow;
        if (row == null || row.IsGroup || row.Cmd == null)
            return;
        string sCommand = edtUserCmdName.Text.Trim();
        if (sCommand == "")
        {
            M2Forms.MessageBox("命令名称不能为空！", "提示信息", M2Forms.MB_OK | M2Forms.MB_ICONERROR);
            if (edtUserCmdName.CanFocus)
                edtUserCmdName.Focus();
            return;
        }
        row.Cmd.sCmd = sCommand;
        row.Cmd.nPermissionMin = (int)seUserCmdPermission.Value;
    }

    /// <summary>btnUserCmdSaveClick 1:1（'Command' 节 42 键；'Permission' 节原文仅写前三项）。</summary>
    public void ButtonUserCmdSaveClick(object? sender)
    {
        ButtonUserCmdSave.Enabled = false;
        var conf = M2ShareState.CommandConfIni;
        conf.WriteString("Command", "Date", GameCommandState.g_GameCommand.Data.sCmd);
        conf.WriteString("Command", "PrvMsg", GameCommandState.g_GameCommand.PRVMSG.sCmd);
        conf.WriteString("Command", "AllowMsg", GameCommandState.g_GameCommand.ALLOWMSG.sCmd);
        conf.WriteString("Command", "LetShout", GameCommandState.g_GameCommand.LETSHOUT.sCmd);
        conf.WriteString("Command", "LetTrade", GameCommandState.g_GameCommand.LETTRADE.sCmd);
        conf.WriteString("Command", "LetGuild", GameCommandState.g_GameCommand.LETGUILD.sCmd);
        conf.WriteString("Command", "LetChallenge", GameCommandState.g_GameCommand.LETCHALLENGE.sCmd); // 允许/禁止挑战
        conf.WriteString("Command", "EndGuild", GameCommandState.g_GameCommand.ENDGUILD.sCmd);
        conf.WriteString("Command", "BanGuildChat", GameCommandState.g_GameCommand.BANGUILDCHAT.sCmd);
        conf.WriteString("Command", "AuthAlly", GameCommandState.g_GameCommand.AUTHALLY.sCmd);
        conf.WriteString("Command", "Auth", GameCommandState.g_GameCommand.AUTH.sCmd);
        conf.WriteString("Command", "AuthCancel", GameCommandState.g_GameCommand.AUTHCANCEL.sCmd);
        conf.WriteString("Command", "ViewDiary", GameCommandState.g_GameCommand.DIARY.sCmd);
        conf.WriteString("Command", "UserMove", GameCommandState.g_GameCommand.USERMOVE.sCmd);
        conf.WriteString("Command", "Searching", GameCommandState.g_GameCommand.SEARCHING.sCmd);
        conf.WriteString("Command", "AllowGroupCall", GameCommandState.g_GameCommand.ALLOWGROUPCALL.sCmd);
        conf.WriteString("Command", "GroupCall", GameCommandState.g_GameCommand.GROUPRECALLL.sCmd);
        conf.WriteString("Command", "AllowGuildReCall", GameCommandState.g_GameCommand.AllowGuildReCall.sCmd);
        conf.WriteString("Command", "GuildReCall", GameCommandState.g_GameCommand.GUILDRECALLL.sCmd);
        conf.WriteString("Command", "StorageUnLock", GameCommandState.g_GameCommand.UNLOCKSTORAGE.sCmd);
        conf.WriteString("Command", "PasswordUnLock", GameCommandState.g_GameCommand.UNLOCK.sCmd);
        conf.WriteString("Command", "StorageLock", GameCommandState.g_GameCommand.Lock.sCmd);
        conf.WriteString("Command", "StorageSetPassword", GameCommandState.g_GameCommand.SETPASSWORD.sCmd);
        conf.WriteString("Command", "StorageChgPassword", GameCommandState.g_GameCommand.CHGPASSWORD.sCmd);
        conf.WriteString("Command", "StorageUserClearPassword", GameCommandState.g_GameCommand.UNPASSWORD.sCmd);
        conf.WriteString("Command", "MemberFunc", GameCommandState.g_GameCommand.MEMBERFUNCTION.sCmd);
        conf.WriteString("Command", "Dear", GameCommandState.g_GameCommand.DEAR.sCmd);
        conf.WriteString("Command", "Master", GameCommandState.g_GameCommand.MASTER.sCmd);
        conf.WriteString("Command", "DearRecall", GameCommandState.g_GameCommand.DEARRECALL.sCmd);
        conf.WriteString("Command", "MasterRecall", GameCommandState.g_GameCommand.MASTERECALL.sCmd);
        conf.WriteString("Command", "AllowDearRecall", GameCommandState.g_GameCommand.ALLOWDEARRCALL.sCmd);
        conf.WriteString("Command", "AllowMasterRecall", GameCommandState.g_GameCommand.ALLOWMASTERRECALL.sCmd);
        conf.WriteString("Command", "AttackMode", GameCommandState.g_GameCommand.ATTACKMODE.sCmd);
        conf.WriteString("Command", "Rest", GameCommandState.g_GameCommand.REST.sCmd);
        conf.WriteString("Command", "TakeOnHorse", GameCommandState.g_GameCommand.TAKEONHORSE.sCmd);
        conf.WriteString("Command", "TakeOffHorse", GameCommandState.g_GameCommand.TAKEOFHORSE.sCmd);
        conf.WriteString("Command", "DisableHorseInvite", GameCommandState.g_GameCommand.DISABLEHORSEINVITE.sCmd);
        conf.WriteString("Command", "SendTopChatBoardMsg", GameCommandState.g_GameCommand.SendTopChatBoardMsg.sCmd);
        conf.WriteString("Command", "PublicChat", GameCommandState.g_GameCommand.PUBLICCHAT.sCmd);
        conf.WriteString("Command", "CryChat", GameCommandState.g_GameCommand.CRYCHAT.sCmd);
        conf.WriteString("Command", "OpenSellPlayer", GameCommandState.g_GameCommand.OpenSellPlayer.sCmd);
        conf.WriteInteger("Permission", "Date", GameCommandState.g_GameCommand.Data.nPermissionMin);
        conf.WriteInteger("Permission", "PrvMsg", GameCommandState.g_GameCommand.PRVMSG.nPermissionMin);
        conf.WriteInteger("Permission", "AllowMsg", GameCommandState.g_GameCommand.ALLOWMSG.nPermissionMin);
    }

    // ================= 管理员命令页（批次J33） =================

    /// <summary>AddAdminCommandToList 1:1（100 条命令，IsPermission 标记）。</summary>
    public void AddAdminCommandToList()
    {
        AdminCommandRows = GameCommandState.BuildAdminCommandList();
        ListBoxAdminCmd.Items.Clear();
        foreach (var row in AdminCommandRows)
            ListBoxAdminCmd.Items.Add(row.Cmd!.sCmd + " " + row.Param + " " + row.Desc + (row.IsPermission ? " √" : ""));
        ButtonAdminCmdOK.Enabled = false;
        ButtonAdminCmdSave.Enabled = false;
    }

    private GameCommandRow? FocusedAdminRow =>
        ListBoxAdminCmd.SelectedIndex >= 0 && ListBoxAdminCmd.SelectedIndex < AdminCommandRows.Count
            ? AdminCommandRows[ListBoxAdminCmd.SelectedIndex]
            : null;

    public void ListBoxAdminCmdFocusChanged(object? sender)
    {
        var row = FocusedAdminRow;
        if (row == null || row.Cmd == null)
            return;
        edtAdminCmdName.Text = row.Cmd.sCmd;
        seAdminCmdPermission.Value = row.Cmd.nPermissionMin;
    }

    public void edtAdminCmdNameChange(object? sender)
    {
        ButtonAdminCmdOK.Enabled = true;
        ButtonAdminCmdSave.Enabled = true;
    }

    public void seAdminCmdPermissionChange(object? sender)
    {
        ButtonAdminCmdOK.Enabled = true;
        ButtonAdminCmdSave.Enabled = true;
    }

    public void btnAdminCmdOKClick(object? sender)
    {
        var row = FocusedAdminRow;
        if (row == null || row.Cmd == null)
            return;
        string sCommand = edtAdminCmdName.Text.Trim();
        if (sCommand == "")
        {
            M2Forms.MessageBox("命令名称不能为空！", "提示信息", M2Forms.MB_OK | M2Forms.MB_ICONERROR);
            if (edtAdminCmdName.CanFocus)
                edtAdminCmdName.Focus();
            return;
        }
        row.Cmd.sCmd = sCommand;
        row.Cmd.nPermissionMin = (int)seAdminCmdPermission.Value;
    }

    /// <summary>btnAdminCmdSaveClick 1:1（'Command' 节 55 键 + 'Permission' 节 48 键，含 Make/PositionMove/Move
    /// 的 Min/Max 双键与 SpiritStart/SpiritStop/Spirit 注释等原文形态）。</summary>
    public void ButtonAdminCmdSaveClick(object? sender)
    {
        var admin = GameCommandState.AdminCmdByName;
        ButtonAdminCmdSave.Enabled = false;
        var conf = M2ShareState.CommandConfIni;
        conf.WriteString("Command", "ObServer", admin["OBSERVER"].sCmd);
        conf.WriteString("Command", "GameMaster", admin["GAMEMASTER"].sCmd);
        conf.WriteString("Command", "SuperMan", admin["SUEPRMAN"].sCmd);
        conf.WriteString("Command", "StorageClearPassword", admin["CLRPASSWORD"].sCmd);
        conf.WriteString("Command", "Who", admin["WHO"].sCmd);
        conf.WriteString("Command", "Total", admin["TOTAL"].sCmd);
        conf.WriteString("Command", "Make", admin["MAKE"].sCmd);
        conf.WriteString("Command", "PositionMove", admin["POSITIONMOVE"].sCmd);
        conf.WriteString("Command", "Move", admin["Move"].sCmd);
        conf.WriteString("Command", "Recall", admin["RECALL"].sCmd);
        conf.WriteString("Command", "ReGoto", admin["REGOTO"].sCmd);
        conf.WriteString("Command", "Ting", admin["TING"].sCmd);
        conf.WriteString("Command", "SuperTing", admin["SUPERTING"].sCmd);
        conf.WriteString("Command", "MapMove", admin["MAPMOVE"].sCmd);
        conf.WriteString("Command", "Info", admin["INFO"].sCmd);
        conf.WriteString("Command", "HumanLocal", admin["HUMANLOCAL"].sCmd);
        conf.WriteString("Command", "ViewWhisper", admin["VIEWWHISPER"].sCmd);
        conf.WriteString("Command", "MobLevel", admin["MOBLEVEL"].sCmd);
        conf.WriteString("Command", "MobCount", admin["MOBCOUNT"].sCmd);
        conf.WriteString("Command", "HumanCount", admin["HUMANCOUNT"].sCmd);
        conf.WriteString("Command", "Map", admin["Map"].sCmd);
        conf.WriteString("Command", "Level", admin["Level"].sCmd);
        conf.WriteString("Command", "Kick", admin["KICK"].sCmd);
        conf.WriteString("Command", "ReAlive", admin["ReAlive"].sCmd);
        conf.WriteString("Command", "Kill", admin["KILL"].sCmd);
        conf.WriteString("Command", "ChangeJob", admin["CHANGEJOB"].sCmd);
        conf.WriteString("Command", "FreePenalty", admin["FREEPENALTY"].sCmd);
        conf.WriteString("Command", "PkPoint", admin["PKPOINT"].sCmd);
        conf.WriteString("Command", "IncPkPoint", admin["IncPkPoint"].sCmd);
        conf.WriteString("Command", "ChangeGender", admin["CHANGEGENDER"].sCmd);
        conf.WriteString("Command", "Hair", admin["HAIR"].sCmd);
        conf.WriteString("Command", "BonusPoint", admin["BonusPoint"].sCmd);
        conf.WriteString("Command", "DelBonuPoint", admin["DELBONUSPOINT"].sCmd);
        conf.WriteString("Command", "RestBonuPoint", admin["RESTBONUSPOINT"].sCmd);
        conf.WriteString("Command", "SetPermission", admin["SETPERMISSION"].sCmd);
        conf.WriteString("Command", "ReNewLevel", admin["RENEWLEVEL"].sCmd);
        conf.WriteString("Command", "DelGold", admin["DELGOLD"].sCmd);
        conf.WriteString("Command", "AddGold", admin["ADDGOLD"].sCmd);
        conf.WriteString("Command", "GameGold", admin["GAMEGOLD"].sCmd);
        conf.WriteString("Command", "GamePoint", admin["GAMEPOINT"].sCmd);
        conf.WriteString("Command", "CreditPoint", admin["CREDITPOINT"].sCmd);
        conf.WriteString("Command", "RefineWeapon", admin["REFINEWEAPON"].sCmd);
        conf.WriteString("Command", "AdjuestTLevel", admin["ADJUESTLEVEL"].sCmd);
        conf.WriteString("Command", "AdjuestExp", admin["ADJUESTEXP"].sCmd);
        conf.WriteString("Command", "ChangeDearName", admin["CHANGEDEARNAME"].sCmd);
        conf.WriteString("Command", "ChangeGameDiamond", admin["CHANGEGAMEDIAMOND"].sCmd);
        conf.WriteString("Command", "ChangeGameGird", admin["CHANGEGAMEGIRD"].sCmd);
        conf.WriteString("Command", "ChangeGameGlory", admin["CHANGEGAMEGLORY"].sCmd);
        conf.WriteString("Command", "Hunger", admin["HUNGER"].sCmd);
        conf.WriteString("Command", "SpiritStart", admin["SPIRIT"].sCmd);
        conf.WriteString("Command", "SpiritStop", admin["SPIRITSTOP"].sCmd);
        conf.WriteString("Command", "NGLevel", admin["NGLevel"].sCmd);
        conf.WriteString("Command", "DelUserShop", admin["DelUserShop"].sCmd);
        conf.WriteString("Command", "SetUserShopName", admin["SetUserShopName"].sCmd);
        conf.WriteString("Command", "DelSellPlayer", admin["DelSellPlayer"].sCmd);

        conf.WriteInteger("Permission", "GameMaster", admin["GAMEMASTER"].nPermissionMin);
        conf.WriteInteger("Permission", "ObServer", admin["OBSERVER"].nPermissionMin);
        conf.WriteInteger("Permission", "SuperMan", admin["SUEPRMAN"].nPermissionMin);
        conf.WriteInteger("Permission", "StorageClearPassword", admin["CLRPASSWORD"].nPermissionMin);
        conf.WriteInteger("Permission", "Who", admin["WHO"].nPermissionMin);
        conf.WriteInteger("Permission", "Total", admin["TOTAL"].nPermissionMin);
        conf.WriteInteger("Permission", "MakeMin", admin["MAKE"].nPermissionMin);
        conf.WriteInteger("Permission", "MakeMax", admin["MAKE"].nPermissionMax);
        conf.WriteInteger("Permission", "PositionMoveMin", admin["POSITIONMOVE"].nPermissionMin);
        conf.WriteInteger("Permission", "PositionMoveMax", admin["POSITIONMOVE"].nPermissionMax);
        conf.WriteInteger("Permission", "MoveMin", admin["Move"].nPermissionMin);
        conf.WriteInteger("Permission", "MoveMax", admin["Move"].nPermissionMax);
        conf.WriteInteger("Permission", "Recall", admin["RECALL"].nPermissionMin);
        conf.WriteInteger("Permission", "ReGoto", admin["REGOTO"].nPermissionMin);
        conf.WriteInteger("Permission", "Ting", admin["TING"].nPermissionMin);
        conf.WriteInteger("Permission", "SuperTing", admin["SUPERTING"].nPermissionMin);
        conf.WriteInteger("Permission", "MapMove", admin["MAPMOVE"].nPermissionMin);
        conf.WriteInteger("Permission", "Info", admin["INFO"].nPermissionMin);
        conf.WriteInteger("Permission", "HumanLocal", admin["HUMANLOCAL"].nPermissionMin);
        conf.WriteInteger("Permission", "ViewWhisper", admin["VIEWWHISPER"].nPermissionMin);
        conf.WriteInteger("Permission", "MobLevel", admin["MOBLEVEL"].nPermissionMin);
        conf.WriteInteger("Permission", "MobCount", admin["MOBCOUNT"].nPermissionMin);
        conf.WriteInteger("Permission", "HumanCount", admin["HUMANCOUNT"].nPermissionMin);
        conf.WriteInteger("Permission", "Map", admin["Map"].nPermissionMin);
        conf.WriteInteger("Permission", "Level", admin["Level"].nPermissionMin);
        conf.WriteInteger("Permission", "Kick", admin["KICK"].nPermissionMin);
        conf.WriteInteger("Permission", "ReAlive", admin["ReAlive"].nPermissionMin);
        conf.WriteInteger("Permission", "Kill", admin["KILL"].nPermissionMin);
        conf.WriteInteger("Permission", "ChangeJob", admin["CHANGEJOB"].nPermissionMin);
        conf.WriteInteger("Permission", "FreePenalty", admin["FREEPENALTY"].nPermissionMin);
        conf.WriteInteger("Permission", "PkPoint", admin["PKPOINT"].nPermissionMin);
        conf.WriteInteger("Permission", "IncPkPoint", admin["IncPkPoint"].nPermissionMin);
        conf.WriteInteger("Permission", "ChangeGender", admin["CHANGEGENDER"].nPermissionMin);
        conf.WriteInteger("Permission", "Hair", admin["HAIR"].nPermissionMin);
        conf.WriteInteger("Permission", "BonusPoint", admin["BonusPoint"].nPermissionMin);
        conf.WriteInteger("Permission", "DelBonuPoint", admin["DELBONUSPOINT"].nPermissionMin);
        conf.WriteInteger("Permission", "RestBonuPoint", admin["RESTBONUSPOINT"].nPermissionMin);
        conf.WriteInteger("Permission", "SetPermission", admin["SETPERMISSION"].nPermissionMin);
        conf.WriteInteger("Permission", "ReNewLevel", admin["RENEWLEVEL"].nPermissionMin);
        conf.WriteInteger("Permission", "DelGold", admin["DELGOLD"].nPermissionMin);
        conf.WriteInteger("Permission", "AddGold", admin["ADDGOLD"].nPermissionMin);
        conf.WriteInteger("Permission", "GameGold", admin["GAMEGOLD"].nPermissionMin);
        conf.WriteInteger("Permission", "GamePoint", admin["GAMEPOINT"].nPermissionMin);
        conf.WriteInteger("Permission", "CreditPoint", admin["CREDITPOINT"].nPermissionMin);
    }

    // ================= 调试命令页（批次J33） =================

    /// <summary>AddDebugCommandToList 1:1（40 条命令）。</summary>
    public void AddDebugCommandToList()
    {
        DebugCommandRows = GameCommandState.BuildDebugCommandList();
        ListBoxDebugCmd.Items.Clear();
        foreach (var row in DebugCommandRows)
            ListBoxDebugCmd.Items.Add(row.Cmd!.sCmd + " " + row.Param + " " + row.Desc);
        ButtonDebugCmdOK.Enabled = false;
        ButtonDebugCmdSave.Enabled = false;
    }

    private GameCommandRow? FocusedDebugRow =>
        ListBoxDebugCmd.SelectedIndex >= 0 && ListBoxDebugCmd.SelectedIndex < DebugCommandRows.Count
            ? DebugCommandRows[ListBoxDebugCmd.SelectedIndex]
            : null;

    public void ListBoxDebugCmdFocusChanged(object? sender)
    {
        var row = FocusedDebugRow;
        if (row == null || row.Cmd == null)
            return;
        edtDebugCmdName.Text = row.Cmd.sCmd;
        seDebugCmdPermission.Value = row.Cmd.nPermissionMin;
    }

    public void edtDebugCmdNameChange(object? sender)
    {
        ButtonDebugCmdOK.Enabled = true;
        ButtonDebugCmdSave.Enabled = true;
    }

    public void seDebugCmdPermissionChange(object? sender)
    {
        ButtonDebugCmdOK.Enabled = true;
        ButtonDebugCmdSave.Enabled = true;
    }

    public void btnDebugCmdOKClick(object? sender)
    {
        var row = FocusedDebugRow;
        if (row == null || row.Cmd == null)
            return;
        string sCommand = edtDebugCmdName.Text.Trim();
        if (sCommand == "")
        {
            M2Forms.MessageBox("命令名称不能为空！", "提示信息", M2Forms.MB_OK | M2Forms.MB_ICONERROR);
            if (edtDebugCmdName.CanFocus)
                edtDebugCmdName.Focus();
            return;
        }
        row.Cmd.sCmd = sCommand;
        row.Cmd.nPermissionMin = (int)seDebugCmdPermission.Value;
    }

    /// <summary>btnDebugCmdSaveClick 1:1（'Command' 节 24 键）。</summary>
    public void ButtonDebugCmdSaveClick(object? sender)
    {
        var admin = GameCommandState.DebugCmdByName;
        ButtonDebugCmdSave.Enabled = false;
        var conf = M2ShareState.CommandConfIni;
        conf.WriteString("Command", "SHOWFLAG", admin["SHOWFLAG"].sCmd);
        conf.WriteString("Command", "SETFLAG", admin["SETFLAG"].sCmd);
        conf.WriteString("Command", "SHOWOPEN", admin["SHOWOPEN"].sCmd);
        conf.WriteString("Command", "SETOPEN", admin["SETOPEN"].sCmd);
        conf.WriteString("Command", "SHOWUNIT", admin["SHOWUNIT"].sCmd);
        conf.WriteString("Command", "SETUNIT", admin["SETUNIT"].sCmd);
        conf.WriteString("Command", "MOBNPC", admin["MOBNPC"].sCmd);
        conf.WriteString("Command", "DELNPC", admin["DELNPC"].sCmd);
        conf.WriteString("Command", "LOTTERYTICKET", admin["LOTTERYTICKET"].sCmd);
        conf.WriteString("Command", "RELOADADMIN", admin["RELOADADMIN"].sCmd);
        conf.WriteString("Command", "ReLoadNpc", admin["ReLoadNpc"].sCmd);
        conf.WriteString("Command", "RELOADMANAGE", admin["RELOADMANAGE"].sCmd);
        conf.WriteString("Command", "RELOADROBOTMANAGE", admin["RELOADROBOTMANAGE"].sCmd);
        conf.WriteString("Command", "RELOADROBOT", admin["RELOADROBOT"].sCmd);
        conf.WriteString("Command", "RELOADMONITEMS", admin["RELOADMONITEMS"].sCmd);
        conf.WriteString("Command", "RELOADDIARY", admin["RELOADDIARY"].sCmd);
        conf.WriteString("Command", "RELOADITEMDB", admin["RELOADITEMDB"].sCmd);
        conf.WriteString("Command", "RELOADMAGICDB", admin["RELOADMAGICDB"].sCmd);
        conf.WriteString("Command", "RELOADMONSTERDB", admin["RELOADMONSTERDB"].sCmd);
        conf.WriteString("Command", "RELOADMINMAP", admin["RELOADMINMAP"].sCmd);
        conf.WriteString("Command", "RELOADGUILD", admin["RELOADGUILD"].sCmd);
        conf.WriteString("Command", "RELOADGUILDALL", admin["RELOADGUILDALL"].sCmd);
        conf.WriteString("Command", "RELOADLINENOTICE", admin["RELOADLINENOTICE"].sCmd);
        conf.WriteString("Command", "RELOADABUSE", admin["RELOADABUSE"].sCmd);
    }
}

