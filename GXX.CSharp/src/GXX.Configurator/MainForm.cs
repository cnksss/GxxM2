using System.Diagnostics;
using System.Text;
using GXX.Core.Launcher;

namespace GXX.Configurator;

public sealed class MainForm : Form
{
    // ---- 基本设置控件 ----
    readonly TextBox _tplPath = new();
    readonly TextBox _outPath = new();
    readonly TextBox _clientFile = new();
    readonly TextBox _dataFile = new();
    readonly TextBox _gamePlan = new();
    readonly TextBox _resDir = new();
    readonly TextBox _runGatePwd = new();
    readonly TextBox _updatePwd = new();
    readonly TextBox _version = new();
    readonly TextBox _bgImage = new();
    readonly TextBox _curNormal = new();
    readonly TextBox _curMount = new();
    readonly TextBox _curUnmount = new();
    readonly TextBox _tcpHost = new();
    readonly NumericUpDown _tcpPort = new();
    readonly TextBox _tcpFile = new();
    readonly TextBox _tcpHost2 = new();
    readonly NumericUpDown _tcpPort2 = new();
    readonly TextBox _tcpFile2 = new();
    readonly TextBox _noticeUrl = new();
    readonly TextBox _homePage = new();
    readonly TextBox _promoId = new();
    readonly CheckBox _windowMode = new() { Text = "窗口模式", AutoSize = true };
    readonly CheckBox _vsync = new() { Text = "垂直同步", AutoSize = true };
    readonly CheckBox _hardware = new() { Text = "硬件加速", AutoSize = true };
    readonly CheckBox _showOpenDoor = new() { Text = "显示开门动画", AutoSize = true };
    readonly CheckBox _show1024 = new() { Text = "启用1024 UI", AutoSize = true };
    readonly CheckBox _changeBit = new() { Text = "允许色深切换", AutoSize = true };
    readonly NumericUpDown _maxClient = new() { Minimum = 1, Maximum = 30 };

    // ---- 界面文字控件 ----
    readonly TextBox[] _attackTexts = new TextBox[ClientDataFields.CountAttackModeTexts];
    readonly TextBox[] _elementTexts = new TextBox[ClientDataFields.CountElementTexts];
    readonly TextBox[] _groupCaptions = new TextBox[ClientDataFields.CountHumGroupCaptions];
    readonly TextBox[] _hairOffsets = new TextBox[12];
    readonly TextBox _expHint = new();
    readonly TextBox _ngExpHint = new();
    readonly TextBox _fluteStoneText = new();
    readonly TextBox _noFluteStoneText = new();
    readonly NumericUpDown _fluteStoneColor = new() { Minimum = 0, Maximum = int.MaxValue };
    readonly NumericUpDown _noFluteStoneColor = new() { Minimum = 0, Maximum = int.MaxValue };

    // ---- 复选框 ----
    readonly CheckBox[] _checks = new CheckBox[ClientDataFields.CountClientConfigs];
    readonly CheckBox _checkEx0 = new() { Text = "CheckedEx0", AutoSize = true };
    readonly NumericUpDown[] _manualHits = new NumericUpDown[5];
    readonly ListView _segments = new();
    readonly TextBox _log = new();

    public MainForm()
    {
        Text = "GXX 登录器配置器 —— ClientData.dat 生成器";
        Width = 1080;
        Height = 760;
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Microsoft YaHei UI", 9F);

        var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3 };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 62));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 150));
        Controls.Add(root);

        // ---------- 顶部：模板 / 输出 ----------
        var top = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4, RowCount = 2, Padding = new Padding(8, 6, 8, 0) };
        top.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
        top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        top.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 96));
        top.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 96));
        top.Controls.Add(new Label { Text = "模板 ClientData.dat：", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
        top.Controls.Add(_tplPath, 1, 0);
        _tplPath.Dock = DockStyle.Fill;
        var btnOpen = new Button { Text = "选择模板…", Dock = DockStyle.Fill };
        btnOpen.Click += (_, _) => PickTemplate();
        top.Controls.Add(btnOpen, 2, 0);
        var btnRead = new Button { Text = "读入设置", Dock = DockStyle.Fill };
        btnRead.Click += (_, _) => LoadFromTemplate();
        top.Controls.Add(btnRead, 3, 0);

        top.Controls.Add(new Label { Text = "输出文件：", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 1);
        top.Controls.Add(_outPath, 1, 1);
        _outPath.Dock = DockStyle.Fill;
        var btnOut = new Button { Text = "另存为…", Dock = DockStyle.Fill };
        btnOut.Click += (_, _) => PickOutput();
        top.Controls.Add(btnOut, 2, 1);
        var btnBuild = new Button { Text = "★ 生成", Dock = DockStyle.Fill, Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold) };
        btnBuild.Click += (_, _) => Build();
        top.Controls.Add(btnBuild, 3, 1);
        root.Controls.Add(top, 0, 0);

        // ---------- 中部：选项卡 ----------
        var tabs = new TabControl { Dock = DockStyle.Fill };
        tabs.TabPages.Add(BuildBasicTab());
        tabs.TabPages.Add(BuildOptionsTab());
        tabs.TabPages.Add(BuildTextsTab());
        tabs.TabPages.Add(BuildSegmentsTab());
        tabs.TabPages.Add(BuildHelpTab());
        root.Controls.Add(tabs, 0, 1);

        // ---------- 底部：日志 ----------
        _log.Multiline = true;
        _log.ReadOnly = true;
        _log.ScrollBars = ScrollBars.Vertical;
        _log.Dock = DockStyle.Fill;
        _log.Font = new Font("Consolas", 9F);
        _log.BackColor = Color.FromArgb(30, 30, 30);
        _log.ForeColor = Color.Gainsboro;
        var logBox = new GroupBox { Text = "日志", Dock = DockStyle.Fill, Padding = new Padding(6) };
        logBox.Controls.Add(_log);
        root.Controls.Add(logBox, 0, 2);

        Log("GXX 登录器配置器已启动。");
        Log("工作流：选择模板 ClientData.dat → 读入设置 → 修改 → 生成。");
        Log("说明：配置器以模板为基准，仅改写已实测校准的字段；未映射字段保持字节不变。");
    }

    // ================================================================ 选项卡

    TabPage BuildBasicTab()
    {
        var page = new TabPage("基本设置") { Padding = new Padding(10) };
        var sc = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
        var t = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 4, Padding = new Padding(4) };
        t.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
        t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        t.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
        t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        page.Controls.Add(sc);
        sc.Controls.Add(t);

        int r = 0;
        void Row(string l1, Control c1, string l2 = null, Control c2 = null)
        {
            t.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            t.Controls.Add(new Label { Text = l1, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, r);
            c1.Dock = DockStyle.Fill;
            t.Controls.Add(c1, 1, r);
            if (!string.IsNullOrEmpty(l2))
                t.Controls.Add(new Label { Text = l2, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 2, r);
            if (c2 != null) { c2.Dock = DockStyle.Fill; t.Controls.Add(c2, 3, r); }
            r++;
        }
        void Header(string s)
        {
            var lbl = new Label { Text = s, Dock = DockStyle.Fill, TextAlign = ContentAlignment.BottomLeft, Font = new Font(Font, FontStyle.Bold), ForeColor = Color.FromArgb(0, 70, 140) };
            t.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
            t.Controls.Add(lbl, 0, r);
            t.SetColumnSpan(lbl, 4);
            r++;
        }

        Header("客户端");
        Row("客户端文件", _clientFile, "客户端数据文件", _dataFile);
        Row("补丁文件(必备补丁)", _gamePlan, "Resources 目录", _resDir);
        Row("登录密码(RunGate)", _runGatePwd, "微端更新密码", _updatePwd);
        Row("版本号(日期串)", _version, "推广标识", _promoId);

        Header("图片与光标（留空表示沿用模板）");
        Row("游戏背景图", _bgImage, "鼠标光标", _curNormal);
        Row("镶嵌光标", _curMount, "拆卸光标", _curUnmount);

        Header("远程列表服务器");
        Row("主 TCP 列表服务器", _tcpHost, "端口", _tcpPort);
        Row("主 TCP 列表文件", _tcpFile, "", null);
        Row("备用 TCP 服务器", _tcpHost2, "端口", _tcpPort2);
        Row("备用 TCP 列表文件", _tcpFile2, "", null);
        Row("公告地址", _noticeUrl, "官方首页", _homePage);

        Header("启动表现");
        var fl = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true };
        fl.Controls.AddRange(new Control[] { _windowMode, _vsync, _hardware, _showOpenDoor, _show1024, _changeBit });
        t.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
        t.Controls.Add(fl, 1, r); t.SetColumnSpan(fl, 3);
        r++;
        Row("多开数量", _maxClient);

        return page;
    }

    TabPage BuildOptionsTab()
    {
        var page = new TabPage("客户端选项 (内挂复选框)") { Padding = new Padding(10), AutoScroll = true };
        var fl = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, FlowDirection = FlowDirection.TopDown, WrapContents = false };
        var wa = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, FlowDirection = FlowDirection.LeftToRight, WrapContents = false };
        var wb = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, FlowDirection = FlowDirection.LeftToRight, WrapContents = false };
        var wc = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, FlowDirection = FlowDirection.LeftToRight, WrapContents = false };
        page.Controls.Add(fl);
        fl.Controls.Add(wa); fl.Controls.Add(wb); fl.Controls.Add(wc);

        for (int i = 0; i < _checks.Length; i++)
        {
            var cb = new CheckBox { Text = CheckboxNames.Label(i), AutoSize = true, Margin = new Padding(3, 2, 3, 2) };
            _checks[i] = cb;
            (i % 3 == 0 ? wa : i % 3 == 1 ? wb : wc).Controls.Add(cb);
        }

        var tail = new FlowLayoutPanel { Dock = DockStyle.Bottom, AutoSize = true, Padding = new Padding(0, 8, 0, 0) };
        tail.Controls.Add(_checkEx0);
        for (int i = 0; i < _manualHits.Length; i++)
        {
            tail.Controls.Add(new Label { Text = $"手动技能{i + 1}", AutoSize = true, Margin = new Padding(12, 6, 2, 0) });
            _manualHits[i] = new NumericUpDown { Minimum = 0, Maximum = uint.MaxValue, Width = 110 };
            tail.Controls.Add(_manualHits[i]);
        }
        var btnAll = new Button { Text = "全选", Width = 70 };
        btnAll.Click += (_, _) => { foreach (var c in _checks) c.Checked = true; };
        var btnNone = new Button { Text = "全不选", Width = 70 };
        btnNone.Click += (_, _) => { foreach (var c in _checks) c.Checked = false; };
        tail.Controls.Add(btnAll); tail.Controls.Add(btnNone);
        page.Controls.Add(tail);
        return page;
    }

    TabPage BuildTextsTab()
    {
        var page = new TabPage("界面文字") { Padding = new Padding(10) };
        var sc = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
        var t = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 3, Padding = new Padding(4) };
        t.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170));
        t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        page.Controls.Add(sc);
        sc.Controls.Add(t);

        int r = 0;
        void Header(string s)
        {
            var lbl = new Label { Text = s, Dock = DockStyle.Fill, TextAlign = ContentAlignment.BottomLeft, Font = new Font(Font, FontStyle.Bold), ForeColor = Color.FromArgb(0, 70, 140) };
            t.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));
            t.Controls.Add(lbl, 0, r); t.SetColumnSpan(lbl, 3);
            r++;
        }
        void One(string label, Control c, int span = 2)
        {
            t.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
            t.Controls.Add(new Label { Text = label, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, r);
            c.Dock = DockStyle.Fill; t.Controls.Add(c, 1, r); t.SetColumnSpan(c, span);
            r++;
        }

        Header("攻击模式文字 (AttackModeTexts[0..7])");
        for (int i = 0; i < _attackTexts.Length; i++)
        {
            _attackTexts[i] = new TextBox();
            One($"AttackModeText{i + 1}", _attackTexts[i]);
        }

        Header("元素新属性文字 (ElementNewPropertyTexts[0..26])");
        for (int i = 0; i < _elementTexts.Length; i++)
        {
            _elementTexts[i] = new TextBox();
            One($"ElementNewPropertyText{i + 1}", _elementTexts[i]);
        }

        Header("人物栏属性分组 (HumPropertyGroupCaption[0..6]，\\ 表示换行)");
        for (int i = 0; i < _groupCaptions.Length; i++)
        {
            _groupCaptions[i] = new TextBox();
            One($"HumPropertyGroupCaption{i + 1}", _groupCaptions[i]);
        }

        Header("经验提示 / 物品说明");
        One("AddExpHintText", _expHint);
        One("AddNGExpHintText", _ngExpHint);
        One("ItemHintFluteStoneText", _fluteStoneText);
        One("ItemHintNoFluteStoneText", _noFluteStoneText);
        One("ItemHintFluteStoneColor", _fluteStoneColor);
        One("ItemHintNoFluteStoneColor", _noFluteStoneColor);

        Header("发型偏移 (12 项)");
        string[] hairKeys = { "UserHairOffsetX","UserHairOffsetY","OtherUserHairOffsetX","OtherUserHairOffsetY",
                              "HeroUserHairOffsetX","HeroUserHairOffsetY","UserHairOffsetX2","UserHairOffsetY2",
                              "OtherUserHairOffsetX2","OtherUserHairOffsetY2","HeroUserHairOffsetX2","HeroUserHairOffsetY2" };
        for (int i = 0; i < hairKeys.Length; i++)
        {
            _hairOffsets[i] = new TextBox();
            One(hairKeys[i], _hairOffsets[i]);
        }
        return page;
    }

    TabPage BuildSegmentsTab()
    {
        var page = new TabPage("payload 段落") { Padding = new Padding(10) };
        _segments.View = View.Details;
        _segments.FullRowSelect = true;
        _segments.Dock = DockStyle.Fill;
        _segments.Columns.Add("段落", 220);
        _segments.Columns.Add("Offset", 100);
        _segments.Columns.Add("Size(压缩)", 110);
        _segments.Columns.Add("有 CRC", 70);
        var btn = new Button { Text = "刷新段落表", Dock = DockStyle.Bottom, Height = 30 };
        btn.Click += (_, _) => RefreshSegments();
        page.Controls.Add(_segments);
        page.Controls.Add(btn);
        return page;
    }

    TabPage BuildHelpTab()
    {
        var page = new TabPage("说明") { Padding = new Padding(10) };
        var tb = new TextBox
        {
            Multiline = true,
            ReadOnly = true,
            Dock = DockStyle.Fill,
            ScrollBars = ScrollBars.Vertical,
            Font = new Font("Microsoft YaHei UI", 9.5F),
            Text = string.Join(Environment.NewLine, new[]
            {
                "GXX 登录器配置器 —— 重建说明",
                "",
                "【它做什么】",
                "  以一份既有的 ClientData.dat 为模板，套用你在本工具里的设置，输出新的 ClientData.dat。",
                "  该文件即客户端 ClMain.LoadConfig 读取的『登录器配置』，",
                "  由 TClientParam.sClientDataFile 指向。",
                "",
                "【文件格式（已对真机文件实测确认）】",
                "  文件 = [payload 区] + [TConfigClient 记录]",
                "  记录前 4 字节 nSize = payload 长度；紧接 4 字节 nCrc = CRC32(payload)",
                "  CRC 用的是 Common\\CheckCrc.pas 的变体，初值 $DBC66688（不是标准 $FFFFFFFF）",
                "  整个记录以 DES-CBC(块大小 20) 加密，密钥字符串 = \"3230393649\"（= IntToStr($C08BE531)）",
                "  payload 各段为 zlib 流；带 CRC 的段落其 CRC 是对『解压后』数据算的",
                "",
                "【为什么用模板而不是从零合成】",
                "  线上运行的客户端是比仓库源码更新的修订（记录 23324 字节，比源码声明多出若干字段）。",
                "  本工具只改写已实测校准的字段，未映射字段保持字节不变 —— 兼容性风险为零。",
                "",
                "【已实测校准的字段】",
                "  sGamePlanName @236  sRunGatePassWord @270  ClientConfigs @321 ×101",
                "  ClientConfigs_Ex @422  HumManuallyCustomHits @423  sResourcesDir @443",
                "  sAttackModeTexts @1197×8  sExpAddHintText @1525  sNGExpAddHintText @1586",
                "  sElementNewPropertyTexts @1677×27  sHumPropertyGroupCaption @3864×7",
                "  sItemHintFluteStoneText @4151  sItemHintNoFluteStoneText @4232",
                "  nHairOffsets @4365×12  sGameLoginVersion @4498",
                "  段落表：nBaseUI@8  nShareUI@16  nNewStateWindowUI@24  nConfigDlgUI@32  nJSYUI@40",
                "          nBackBmp@48(+Crc@56)  nCursorDef@60  nCursorMount@72  nCursorUnmount@84",
                "",
                "【关键校验记录】",
                "  · 解码→再编码：与真机 ClientData.dat 逐字节完全相同（211475 字节）",
                "  · ClientConfigs[0..100] 与生成器 Config.ini 的 Checked0..Checked100 比对：101/101 一致",
                "  · nBackBmpCrc 与 CheckCrc32(解压后背景图) 完全一致",
                "  · 保存后未映射区改动字节数 = 0",
                "",
                "【安全提示】",
                "  生成前请备份线上 ClientData.dat。生成后建议先用工具自检（日志会显示回读校验结果）。",
            })
        };
        page.Controls.Add(tb);
        return page;
    }

    // ================================================================ 动作

    void PickTemplate()
    {
        using var d = new OpenFileDialog { Filter = "GXX 配置数据 (*.dat)|*.dat|所有文件 (*.*)|*.*", Title = "选择模板 ClientData.dat" };
        if (d.ShowDialog(this) == DialogResult.OK) { _tplPath.Text = d.FileName; LoadFromTemplate(); }
    }

    void PickOutput()
    {
        using var d = new SaveFileDialog { Filter = "GXX 配置数据 (*.dat)|*.dat", FileName = Path.GetFileName(_dataFile.Text is { Length: > 0 } n ? n : "ClientData.dat") };
        if (d.ShowDialog(this) == DialogResult.OK) _outPath.Text = d.FileName;
    }

    void LoadFromTemplate()
    {
        if (!File.Exists(_tplPath.Text)) { Warn("请先选择模板文件"); return; }
        try
        {
            var s = ClientDataBuilder.ReadSettings(_tplPath.Text);
            Apply(s);
            if (_outPath.Text.Length == 0)
                _outPath.Text = Path.Combine(Path.GetDirectoryName(_tplPath.Text) ?? ".", "ClientData_new.dat");
            RefreshSegments();
            Log($"已从模板读入设置：{_tplPath.Text}");
            var doc = ClientDataFile.Decode(File.ReadAllBytes(_tplPath.Text));
            string warn = ClientDataFields.ValidateTemplate(doc.Record);
            Log(warn == null
                ? $"记录大小 {doc.Record.Length} 字节，与已校准布局相符。"
                : $"注意：{warn}");
            Log($"payload {doc.Payload.Length:N0} 字节，记录 {doc.Record.Length:N0} 字节。");
        }
        catch (Exception ex) { Err("读入失败", ex); }
    }

    void Apply(LauncherSettings s)
    {
        _clientFile.Text = s.ClientFile;
        _dataFile.Text = s.ClientDataFile;
        _gamePlan.Text = s.GamePlanFile;
        _resDir.Text = s.ResourcesDir;
        _runGatePwd.Text = s.RunGatePassword;
        _updatePwd.Text = s.UpdatePassword;
        _version.Text = s.Version;
        _bgImage.Text = s.BackgroundImage;
        _curNormal.Text = s.CursorNormal;
        _curMount.Text = s.CursorMount;
        _curUnmount.Text = s.CursorUnmount;
        _tcpHost.Text = s.PrimaryTcpHost;
        _tcpPort.Value = Clamp(s.PrimaryTcpPort, _tcpPort);
        _tcpFile.Text = s.PrimaryTcpFile;
        _tcpHost2.Text = s.BackupTcpHost;
        _tcpPort2.Value = Clamp(s.BackupTcpPort, _tcpPort2);
        _tcpFile2.Text = s.BackupTcpFile;
        _noticeUrl.Text = s.NoticeUrl;
        _homePage.Text = s.HomePage;
        _promoId.Text = s.PromotionId;
        _windowMode.Checked = s.WindowMode;
        _vsync.Checked = s.VSync;
        _hardware.Checked = s.Hardware;
        _showOpenDoor.Checked = s.ShowOpenDoor;
        _show1024.Checked = s.Show1024;
        _changeBit.Checked = s.ChangeScreenBitCount;
        _maxClient.Value = Math.Clamp(s.MaxClientCount, 1, 30);

        for (int i = 0; i < _checks.Length; i++) _checks[i].Checked = s.ClientConfigs[i];
        _checkEx0.Checked = s.ClientConfigEx0;
        for (int i = 0; i < _manualHits.Length; i++) _manualHits[i].Value = s.ManualCustomHits[i];

        _expHint.Text = s.ExpAddHintText;
        _ngExpHint.Text = s.NGExpAddHintText;
        _fluteStoneText.Text = s.ItemHintFluteStoneText;
        _noFluteStoneText.Text = s.ItemHintNoFluteStoneText;
        _fluteStoneColor.Value = Clamp(s.ItemHintFluteStoneColor, _fluteStoneColor);
        _noFluteStoneColor.Value = Clamp(s.ItemHintNoFluteStoneColor, _noFluteStoneColor);

        for (int i = 0; i < _attackTexts.Length; i++) _attackTexts[i].Text = s.AttackModeTexts[i];
        for (int i = 0; i < _elementTexts.Length; i++) _elementTexts[i].Text = s.ElementTexts[i];
        for (int i = 0; i < _groupCaptions.Length; i++) _groupCaptions[i].Text = s.HumGroupCaptions[i];
        for (int i = 0; i < _hairOffsets.Length; i++) _hairOffsets[i].Text = s.HairOffsets[i].ToString();
    }

    static decimal Clamp(int v, NumericUpDown n) => Math.Clamp((decimal)v, n.Minimum, n.Maximum);

    LauncherSettings Collect()
    {
        var s = new LauncherSettings
        {
            ClientFile = _clientFile.Text.Trim(),
            ClientDataFile = _dataFile.Text.Trim(),
            GamePlanFile = _gamePlan.Text.Trim(),
            ResourcesDir = _resDir.Text.Trim(),
            RunGatePassword = _runGatePwd.Text.Trim(),
            UpdatePassword = _updatePwd.Text.Trim(),
            Version = _version.Text.Trim(),
            BackgroundImage = _bgImage.Text.Trim(),
            CursorNormal = _curNormal.Text.Trim(),
            CursorMount = _curMount.Text.Trim(),
            CursorUnmount = _curUnmount.Text.Trim(),
            PrimaryTcpHost = _tcpHost.Text.Trim(),
            PrimaryTcpPort = (int)_tcpPort.Value,
            PrimaryTcpFile = _tcpFile.Text.Trim(),
            BackupTcpHost = _tcpHost2.Text.Trim(),
            BackupTcpPort = (int)_tcpPort2.Value,
            BackupTcpFile = _tcpFile2.Text.Trim(),
            NoticeUrl = _noticeUrl.Text.Trim(),
            HomePage = _homePage.Text.Trim(),
            PromotionId = _promoId.Text.Trim(),
            WindowMode = _windowMode.Checked,
            VSync = _vsync.Checked,
            Hardware = _hardware.Checked,
            ShowOpenDoor = _showOpenDoor.Checked,
            Show1024 = _show1024.Checked,
            ChangeScreenBitCount = _changeBit.Checked,
            MaxClientCount = (int)_maxClient.Value,
            ExpAddHintText = _expHint.Text,
            NGExpAddHintText = _ngExpHint.Text,
            ItemHintFluteStoneText = _fluteStoneText.Text,
            ItemHintNoFluteStoneText = _noFluteStoneText.Text,
            ItemHintFluteStoneColor = (int)_fluteStoneColor.Value,
            ItemHintNoFluteStoneColor = (int)_noFluteStoneColor.Value,
        };
        for (int i = 0; i < _checks.Length; i++) s.ClientConfigs[i] = _checks[i].Checked;
        s.ClientConfigEx0 = _checkEx0.Checked;
        for (int i = 0; i < _manualHits.Length; i++) s.ManualCustomHits[i] = (uint)_manualHits[i].Value;
        for (int i = 0; i < _attackTexts.Length; i++) s.AttackModeTexts[i] = _attackTexts[i].Text;
        for (int i = 0; i < _elementTexts.Length; i++) s.ElementTexts[i] = _elementTexts[i].Text;
        for (int i = 0; i < _groupCaptions.Length; i++) s.HumGroupCaptions[i] = _groupCaptions[i].Text;
        for (int i = 0; i < _hairOffsets.Length; i++)
            s.HairOffsets[i] = int.TryParse(_hairOffsets[i].Text, out var v) ? v : 0;
        return s;
    }

    void Build()
    {
        if (!File.Exists(_tplPath.Text)) { Warn("请先选择模板文件"); return; }
        if (_outPath.Text.Trim().Length == 0) { Warn("请先指定输出文件"); return; }
        try
        {
            byte[] bg = null, cn = null, cm = null, cu = null;
            if (_bgImage.Text.Trim().Length > 0)
                bg = LoadImageAsBmp(_bgImage.Text.Trim());
            if (_curNormal.Text.Trim().Length > 0) cn = File.ReadAllBytes(_curNormal.Text.Trim());
            if (_curMount.Text.Trim().Length > 0) cm = File.ReadAllBytes(_curMount.Text.Trim());
            if (_curUnmount.Text.Trim().Length > 0) cu = File.ReadAllBytes(_curUnmount.Text.Trim());

            var r = ClientDataBuilder.Build(_tplPath.Text, Collect(), _outPath.Text, bg, cn, cm, cu);
            foreach (var n in r.Notes) Log("· " + n);
            foreach (var w in r.Warnings) Log("⚠ " + w);
            Log(r.Ok ? "★★★ 生成成功，回读校验通过。" : "⚠ 生成完成，但存在警告，请检查上面的日志。");
            RefreshSegments();
            if (r.Ok)
                MessageBox.Show(this, $"生成成功：\n{r.OutputPath}\n\n{r.FileSize:N0} 字节", "完成",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex) { Err("生成失败", ex); }
    }

    /// <summary>把任意图片转成 BM(DIB) 字节 —— 客户端用 TDIB.LoadFromStream 读取。</summary>
    static byte[] LoadImageAsBmp(string path)
    {
        if (!File.Exists(path)) throw new FileNotFoundException("背景图不存在", path);
        using var img = Image.FromFile(path);
        using var bmp = new Bitmap(img.Width, img.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        using (var g = Graphics.FromImage(bmp)) g.DrawImage(img, 0, 0, img.Width, img.Height);
        using var ms = new MemoryStream();
        bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
        return ms.ToArray();
    }

    void RefreshSegments()
    {
        _segments.Items.Clear();
        if (!File.Exists(_tplPath.Text)) return;
        try
        {
            var doc = ClientDataFile.Decode(File.ReadAllBytes(_tplPath.Text));
            foreach (var (name, off, size, crc) in doc.ListSegments())
            {
                var it = new ListViewItem(name);
                it.SubItems.Add(off.ToString("N0"));
                it.SubItems.Add(size.ToString("N0"));
                it.SubItems.Add(crc ? "是" : "否");
                if (size <= 0) it.ForeColor = Color.Gray;
                _segments.Items.Add(it);
            }
        }
        catch (Exception ex) { Log("段落表读取失败：" + ex.Message); }
    }

    // ================================================================ 日志

    void Log(string s)
    {
        _log.AppendText($"[{DateTime.Now:HH:mm:ss}] {s}{Environment.NewLine}");
    }

    void Warn(string s) => MessageBox.Show(this, s, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);

    void Err(string title, Exception ex)
    {
        Log("✘ " + title + "：" + ex.Message);
        MessageBox.Show(this, ex.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
