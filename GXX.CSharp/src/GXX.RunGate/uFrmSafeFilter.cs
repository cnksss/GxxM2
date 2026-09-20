using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GXX.Core.Rtl;

namespace GXX.RunGate;

/// <summary>原 uFrmSafeFilter.pas:11-208 `TFrmSafeFilter`（DFM: uFrmSafeFilter.dfm）。
/// DFM 几何：Left=361 Top=219 BorderStyle=bsDialog Caption='网络安全过滤'
///           ClientHeight=620 ClientWidth=875 Position=poMainFormCenter PixelsPerInch=96</summary>
public class FrmSafeFilter : Form
{
    // ============================ 控件声明（原名保留） ============================
    public GroupBox grp1;              // DFM: grp1 Left=8 Top=8 Width=240 Height=575 Caption='当前连接' TabOrder=0
    public ListBox lstActive;          // DFM: lstActive Left=7 Top=32 Width=225 Height=534 TabOrder=0
    public Label Label4;               // DFM: Label4 Left=8 Top=17 Width=54 Height=12 Caption='连接列表：'
    public ContextMenuStrip pmActive;  // DFM: pmActive OnPopup/pmActiveChange
    public ToolStripMenuItem mniActiveRefesh, mniActiveSort, N3, mniActiveAddToTemp, mniActiveAddAllToTemp,
                             N2, mniActiveAddToBlock, mniActiveAddAllToBlock, N1, mniActiveKick,
                             mniN8, mniActiveAddAllNoUserToTemp, mniActiveAddAllNoUserToBlock;

    public GroupBox GroupBox1;         // DFM: GroupBox1 Left=256 Top=8 Width=240 Height=575 Caption='IP地址过滤' TabOrder=1
    public ListBox lstTemp;            // DFM: lstTemp Left=7 Top=32 Width=110 Height=295 TabOrder=0
    public ListBox lstBlock;           // DFM: lstBlock Left=122 Top=32 Width=110 Height=295 TabOrder=1
    public ListBox lstIpSection;       // DFM: lstIpSection Left=7 Top=347 Width=225 Height=220 TabOrder=2
    public Label LabelTempList, Label1, Label23;
    public ContextMenuStrip pmTemp, pmBlock, pmIpSection;
    public ToolStripMenuItem mniTempRefresh, mniTempSort, mniN4, mniTempAdd, mniTempDelete, mniTempClear, mniN5,
                             mniTempAddToBlock, mniTempAddAllToBlock;
    public ToolStripMenuItem mniBlockRefresh, mniBlockSort, mniN6, mniBlockAdd, mniBlockDelete, mniBlockClear, mniN7,
                             mniBlockAddToTemp, mniBlockAddAllToTemp;
    public ToolStripMenuItem mniIpSectionSort, mniIpSectionAdd, mniIpSectionDel;

    public GroupBox grp3;              // DFM: grp3 Left=505 Top=7 Width=359 Height=204 Caption="'Mac'地址过滤" TabOrder=2
    public ListBox lstTempMac;         // DFM: lstTempMac Left=7 Top=29 Width=344 Height=78 TabOrder=0
    public ListBox lstBlockMac;        // DFM: lstBlockMac Left=7 Top=123 Width=344 Height=73 TabOrder=1
    public Label lbl1, Label5;
    public ContextMenuStrip pmTempMac, pmBlockMac;
    public ToolStripMenuItem mniTempMacRefresh, mniTempMacSort, MenuItem3, mniTempMacAdd, mniTempMacDelete,
                             mniTempMacClear, MenuItem7, mniTempMacAddToBlock, mniTempMacAddAllToBlock;
    public ToolStripMenuItem mniBlockMacRefresh, mniBlockMacSort, MenuItem4, mniBlockMacAdd, mniBlockMacDelete,
                             mniBlockMacClear, MenuItem9, mniBlockMacAddToTempMac, mniBlockMacAddAllToTempMac;

    public GroupBox grp2;              // DFM: grp2 Left=663 Top=256 Width=201 Height=107 Caption='连接保护' TabOrder=3
    public TSpinEdit seMaxConnect;         // DFM: seMaxConnect Left=76 Top=13 Width=61 Height=21
    public TSpinEdit seKeepConnectTimeOut; // DFM: seKeepConnectTimeOut Left=76 Top=35 Width=61 Height=21
    public TSpinEditEx seIPCountLimit1, seIPCountLimit2, seIPCountLimitTime1, seIPCountLimitTime2;
    public Label Label2, Label3, Label9, Label10, Label22, Label24;

    public GroupBox GroupBox3;         // DFM: GroupBox3 Left=505 Top=216 Width=359 Height=37 Caption='攻击操作' TabOrder=4
    public RadioButton rbAddBlockList, rbAddTempList, rbDisConnect;

    public GroupBox GroupBox4;         // DFM: GroupBox4 Left=505 Top=256 Width=151 Height=82 Caption='流量控制' TabOrder=5
    public TSpinEdit seMaxClientPacketSize, seMaxClientPacketCount;
    public CheckBox chkLostLine;
    public Label Label6, Label8;

    public Button btnOK;               // DFM: btnOK Left=782 Top=589 Width=82 Height=25 Caption='确定(&O)' TabOrder=6 OnClick=btnOKClick
    public Label Label7;               // DFM: Label7 Left=640 Top=595 Width=120 Height=12 Caption='以上参数调后立即生效'

    public GroupBox GroupBox2;         // DFM: GroupBox2 Left=505 Top=342 Width=151 Height=63 Caption='防CC处理' TabOrder=7
    public TSpinEdit seAttackTick, seAttackCount;
    public Label Label11, Label12;

    public GroupBox GroupBox6;         // DFM: GroupBox6 Left=663 Top=409 Width=201 Height=130 Caption='防御设置' TabOrder=8
    public TrackBar trckbrDefenseLevel;    // DFM: trckbrDefenseLevel Left=64 Top=10 Width=133 Height=26
    public CheckBox chkDefenseToLevel1, chkAutoClearTemp, chkResotreDefense, chkAddAllToTemp;
    public TSpinEditEx seAutoClearTemp, seResotreDefense, seDefenseToLevel1, seAddAllToTemp;
    public Label Label25, Label13, Label14, Label15;

    public GroupBox GroupBox5;         // DFM: GroupBox5 Left=505 Top=409 Width=151 Height=130 Caption='发言设置' TabOrder=9
    public TSpinEdit seSayMaxLen, seSayTime, seSayMaxCount, seSayDisableTime;
    public CheckBox chkSayMsgControl;
    public Label Label16, Label17, Label18, Label19;

    public GroupBox grp5;              // DFM: grp5 Left=505 Top=545 Width=359 Height=38 Caption='验证客户端是否合法' TabOrder=10
    public CheckBox chkOpenCheckClient;
    public ComboBox cbbCheckClientFailBlockMode;
    public Label lbl3;

    public GroupBox grp4;              // DFM: grp4 Left=663 Top=366 Width=201 Height=39 Caption='客户端非法包检查' TabOrder=11
    public CheckBox chkCheckClientPacketLegal;
    public TSpinEditEx seCheckClientPacketCount;
    public Label lbl2;

    // ============================ 接缝（可注入） ============================
    /// <summary>接缝：在线连接池（默认空实现 → 列表为空）。</summary>
    public ISafeFilterClientPool ClientPool;

    /// <summary>接缝：9 个 free 函数 + 5 个列表（默认内存实现）。</summary>
    public ISafeFilterHost Host = new InMemorySafeFilterHost();

    public FrmSafeFilter()
    {
        // DFM: FrmSafeFilter Caption='网络安全过滤' BorderStyle=bsDialog Position=poMainFormCenter
        Text = "网络安全过滤";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        Location = new Point(361, 219);
        ClientSize = new Size(875, 620);
        Font = new Font("宋体", 9F);                        // Font.Height=-12 Font.Name='宋体'
        MaximizeBox = false;
        MinimizeBox = false;

        // ---- grp1 当前连接 ----
        // DFM: grp1 Left=8 Top=8 Width=240 Height=575 Caption='当前连接' TabOrder=0
        grp1 = new GroupBox { Left = 8, Top = 8, Width = 240, Height = 575, Text = "当前连接", TabIndex = 0 };
        // DFM: Label4 Left=8 Top=17 Width=54 Height=12 Caption='连接列表：'
        Label4 = new Label { Left = 8, Top = 17, Width = 54, Height = 12, Text = "连接列表：" };
        // DFM: lstActive Left=7 Top=32 Width=225 Height=534 TabOrder=0
        lstActive = new ListBox { Left = 7, Top = 32, Width = 225, Height = 534, TabIndex = 0 };
        lstActive.KeyDown += (s, e) => lstActive_KeyDown(s, e);

        // DFM: pmActive（TPopupMenu，含 12 项，其中 3 个 '-' 分隔符）
        pmActive = new ContextMenuStrip();
        mniActiveRefesh = new ToolStripMenuItem("刷新(&R)");
        mniActiveSort = new ToolStripMenuItem("排序(&S)");
        N3 = new ToolStripMenuItem("-");
        mniActiveAddToTemp = new ToolStripMenuItem("加入动态过滤列表(&B)");
        mniActiveAddAllToTemp = new ToolStripMenuItem("全部加入动态过滤列表(&G)");
        N2 = new ToolStripMenuItem("-");
        mniActiveAddToBlock = new ToolStripMenuItem("加入永久过滤列表(&E)");
        mniActiveAddAllToBlock = new ToolStripMenuItem("全部加入永久过滤列表(F)");
        N1 = new ToolStripMenuItem("-");
        mniActiveKick = new ToolStripMenuItem("踢除下线(&T)");
        mniN8 = new ToolStripMenuItem("-");
        mniActiveAddAllNoUserToTemp = new ToolStripMenuItem("无帐号连接全部加入动态过滤");
        mniActiveAddAllNoUserToBlock = new ToolStripMenuItem("无帐号连接全部加入永久过滤");
        pmActive.Items.AddRange(new ToolStripItem[] { mniActiveRefesh, mniActiveSort, N3,
            mniActiveAddToTemp, mniActiveAddAllToTemp, N2, mniActiveAddToBlock, mniActiveAddAllToBlock, N1,
            mniActiveKick, mniN8, mniActiveAddAllNoUserToTemp, mniActiveAddAllNoUserToBlock });
        lstActive.ContextMenuStrip = pmActive;
        pmActive.Opening += (s, e) => pmActive_Change(s, e);

        // ---- GroupBox1 IP 地址过滤 ----
        // DFM: GroupBox1 Left=256 Top=8 Width=240 Height=575 Caption='IP地址过滤' TabOrder=1
        GroupBox1 = new GroupBox { Left = 256, Top = 8, Width = 240, Height = 575, Text = "IP地址过滤", TabIndex = 1 };
        // DFM: LabelTempList Left=7 Top=17 Width=54 Height=12 Caption='动态过滤:'
        LabelTempList = new Label { Left = 7, Top = 17, Width = 54, Height = 12, Text = "动态过滤:" };
        // DFM: Label1 Left=122 Top=17 Width=54 Height=12 Caption='永久过滤:'
        Label1 = new Label { Left = 122, Top = 17, Width = 54, Height = 12, Text = "永久过滤:" };
        // DFM: Label23 Left=7 Top=331 Width=60 Height=12 Caption='过滤IP段: '
        Label23 = new Label { Left = 7, Top = 331, Width = 60, Height = 12, Text = "过滤IP段: " };
        // DFM: lstTemp Left=7 Top=32 Width=110 Height=295 TabOrder=0
        lstTemp = new ListBox { Left = 7, Top = 32, Width = 110, Height = 295, TabIndex = 0 };
        // DFM: lstBlock Left=122 Top=32 Width=110 Height=295 TabOrder=1
        lstBlock = new ListBox { Left = 122, Top = 32, Width = 110, Height = 295, TabIndex = 1 };
        // DFM: lstIpSection Left=7 Top=347 Width=225 Height=220 TabOrder=2
        lstIpSection = new ListBox { Left = 7, Top = 347, Width = 225, Height = 220, TabIndex = 2 };

        pmTemp = new ContextMenuStrip();
        mniTempRefresh = new ToolStripMenuItem("刷新(&R)");
        mniTempSort = new ToolStripMenuItem("排序(&S)");
        mniN4 = new ToolStripMenuItem("-");
        mniTempAdd = new ToolStripMenuItem("增加(&A)");
        mniTempDelete = new ToolStripMenuItem("删除(&D)");
        mniTempClear = new ToolStripMenuItem("清空(&C)");
        mniN5 = new ToolStripMenuItem("-");
        mniTempAddToBlock = new ToolStripMenuItem("加入永久过滤列表(&E)");
        mniTempAddAllToBlock = new ToolStripMenuItem("全部加入永久过滤列表(&F)");
        pmTemp.Items.AddRange(new ToolStripItem[] { mniTempRefresh, mniTempSort, mniN4, mniTempAdd,
            mniTempDelete, mniTempClear, mniN5, mniTempAddToBlock, mniTempAddAllToBlock });
        lstTemp.ContextMenuStrip = pmTemp;
        pmTemp.Opening += (s, e) => pmTemp_Popup(s, e);

        pmBlock = new ContextMenuStrip();
        mniBlockRefresh = new ToolStripMenuItem("刷新(&R)");
        mniBlockSort = new ToolStripMenuItem("排序(&S)");
        mniN6 = new ToolStripMenuItem("-");
        mniBlockAdd = new ToolStripMenuItem("增加(&A)");
        mniBlockDelete = new ToolStripMenuItem("删除(&D)");
        mniBlockClear = new ToolStripMenuItem("清空(&C)");
        mniN7 = new ToolStripMenuItem("-");
        mniBlockAddToTemp = new ToolStripMenuItem("加入动态过滤列表(&B)");
        mniBlockAddAllToTemp = new ToolStripMenuItem("全部加入动态过滤列表(&G)");
        pmBlock.Items.AddRange(new ToolStripItem[] { mniBlockRefresh, mniBlockSort, mniN6, mniBlockAdd,
            mniBlockDelete, mniBlockClear, mniN7, mniBlockAddToTemp, mniBlockAddAllToTemp });
        lstBlock.ContextMenuStrip = pmBlock;
        pmBlock.Opening += (s, e) => pmBlock_Popup(s, e);

        pmIpSection = new ContextMenuStrip();
        mniIpSectionSort = new ToolStripMenuItem("排序(&S)");
        mniIpSectionAdd = new ToolStripMenuItem("增加IP段(&A)");
        mniIpSectionDel = new ToolStripMenuItem("删除IP段(&D)");
        pmIpSection.Items.AddRange(new ToolStripItem[] { mniIpSectionSort, mniIpSectionAdd, mniIpSectionDel });
        lstIpSection.ContextMenuStrip = pmIpSection;
        pmIpSection.Opening += (s, e) => pmIpSection_Popup(s, e);

        // ---- grp3 Mac 地址过滤 ----
        // DFM: grp3 Left=505 Top=7 Width=359 Height=204 Caption="'Mac'地址过滤" TabOrder=2
        grp3 = new GroupBox { Left = 505, Top = 7, Width = 359, Height = 204, Text = "'Mac'地址过滤", TabIndex = 2 };
        // DFM: lbl1 Left=9 Top=15 Width=60 Height=12 Caption='动态过滤：'
        lbl1 = new Label { Left = 9, Top = 15, Width = 60, Height = 12, Text = "动态过滤：" };
        // DFM: Label5 Left=9 Top=108 Width=60 Height=12 Caption='永久过滤：'
        Label5 = new Label { Left = 9, Top = 108, Width = 60, Height = 12, Text = "永久过滤：" };
        // DFM: lstTempMac Left=7 Top=29 Width=344 Height=78 TabOrder=0
        lstTempMac = new ListBox { Left = 7, Top = 29, Width = 344, Height = 78, TabIndex = 0 };
        // DFM: lstBlockMac Left=7 Top=123 Width=344 Height=73 TabOrder=1
        lstBlockMac = new ListBox { Left = 7, Top = 123, Width = 344, Height = 73, TabIndex = 1 };

        pmTempMac = new ContextMenuStrip();
        mniTempMacRefresh = new ToolStripMenuItem("刷新(&R)");
        mniTempMacSort = new ToolStripMenuItem("排序(&S)");
        MenuItem3 = new ToolStripMenuItem("-");
        mniTempMacAdd = new ToolStripMenuItem("增加(&A)");
        mniTempMacDelete = new ToolStripMenuItem("删除(&D)");
        mniTempMacClear = new ToolStripMenuItem("清空(&C)");
        MenuItem7 = new ToolStripMenuItem("-");
        mniTempMacAddToBlock = new ToolStripMenuItem("加入永久MAC过滤列表(&E)");
        mniTempMacAddAllToBlock = new ToolStripMenuItem("全部加入永久MAC过滤列表(&F)");
        pmTempMac.Items.AddRange(new ToolStripItem[] { mniTempMacRefresh, mniTempMacSort, MenuItem3, mniTempMacAdd,
            mniTempMacDelete, mniTempMacClear, MenuItem7, mniTempMacAddToBlock, mniTempMacAddAllToBlock });
        lstTempMac.ContextMenuStrip = pmTempMac;
        pmTempMac.Opening += (s, e) => pmTempMac_Popup(s, e);

        pmBlockMac = new ContextMenuStrip();
        mniBlockMacRefresh = new ToolStripMenuItem("刷新(&R)");
        mniBlockMacSort = new ToolStripMenuItem("排序(&S)");
        MenuItem4 = new ToolStripMenuItem("-");
        mniBlockMacAdd = new ToolStripMenuItem("增加(&A)");
        mniBlockMacDelete = new ToolStripMenuItem("删除(&D)");
        mniBlockMacClear = new ToolStripMenuItem("清空(&C)");
        MenuItem9 = new ToolStripMenuItem("-");
        mniBlockMacAddToTempMac = new ToolStripMenuItem("加入动态过滤列表(&B)");
        mniBlockMacAddAllToTempMac = new ToolStripMenuItem("全部加入动态过滤列表(&G)");
        pmBlockMac.Items.AddRange(new ToolStripItem[] { mniBlockMacRefresh, mniBlockMacSort, MenuItem4, mniBlockMacAdd,
            mniBlockMacDelete, mniBlockMacClear, MenuItem9, mniBlockMacAddToTempMac, mniBlockMacAddAllToTempMac });
        lstBlockMac.ContextMenuStrip = pmBlockMac;
        pmBlockMac.Opening += (s, e) => pmBlockMac_Popup(s, e);

        // ---- grp2 连接保护 ----
        // DFM: grp2 Left=663 Top=256 Width=201 Height=107 Caption='连接保护' TabOrder=3
        grp2 = new GroupBox { Left = 663, Top = 256, Width = 201, Height = 107, Text = "连接保护", TabIndex = 3 };
        Label2 = new Label { Left = 21, Top = 18, Width = 54, Height = 12, Text = "连接限制:" };
        Label3 = new Label { Left = 139, Top = 18, Width = 42, Height = 12, Text = "连接/IP" };
        Label9 = new Label { Left = 21, Top = 40, Width = 54, Height = 12, Text = "连接超时:" };
        Label10 = new Label { Left = 139, Top = 40, Width = 12, Height = 12, Text = "秒" };
        Label22 = new Label { Left = 70, Top = 62, Width = 66, Height = 12, Text = "毫秒/连接数" };
        Label24 = new Label { Left = 70, Top = 85, Width = 66, Height = 12, Text = "毫秒/连接数" };
        seMaxConnect = new TSpinEdit { Left = 76, Top = 13, Width = 61, Height = 21 };
        seKeepConnectTimeOut = new TSpinEdit { Left = 76, Top = 35, Width = 61, Height = 21 };
        seIPCountLimitTime1 = new TSpinEditEx { Left = 8, Top = 58, Width = 60, Height = 21 };
        seIPCountLimit1 = new TSpinEditEx { Left = 138, Top = 58, Width = 55, Height = 21 };
        seIPCountLimitTime2 = new TSpinEditEx { Left = 8, Top = 81, Width = 60, Height = 21 };
        seIPCountLimit2 = new TSpinEditEx { Left = 138, Top = 81, Width = 55, Height = 21 };

        // ---- GroupBox3 攻击操作 ----
        // DFM: GroupBox3 Left=505 Top=216 Width=359 Height=37 Caption='攻击操作' TabOrder=4
        GroupBox3 = new GroupBox { Left = 505, Top = 216, Width = 359, Height = 37, Text = "攻击操作", TabIndex = 4 };
        rbDisConnect = new RadioButton { Left = 8, Top = 14, Width = 73, Height = 17, Text = "断开连接", TabIndex = 0 };
        rbAddTempList = new RadioButton { Left = 98, Top = 14, Width = 121, Height = 17, Text = "加入动态过滤列表", TabIndex = 1 };
        rbAddBlockList = new RadioButton { Left = 232, Top = 14, Width = 119, Height = 17, Text = "加入永久过滤列表", TabIndex = 2 };

        // ---- GroupBox4 流量控制 ----
        // DFM: GroupBox4 Left=505 Top=256 Width=151 Height=82 Caption='流量控制' TabOrder=5
        GroupBox4 = new GroupBox { Left = 505, Top = 256, Width = 151, Height = 82, Text = "流量控制", TabIndex = 5 };
        Label6 = new Label { Left = 13, Top = 18, Width = 54, Height = 12, Text = "最大限制:" };
        Label8 = new Label { Left = 13, Top = 40, Width = 54, Height = 12, Text = "数量限制:" };
        seMaxClientPacketSize = new TSpinEdit { Left = 69, Top = 13, Width = 76, Height = 21 };
        seMaxClientPacketCount = new TSpinEdit { Left = 69, Top = 35, Width = 76, Height = 21 };
        chkLostLine = new CheckBox { Left = 12, Top = 60, Width = 117, Height = 17, Text = "流量异常掉线处理" };

        // ---- btnOK / Label7 ----
        // DFM: btnOK Left=782 Top=589 Width=82 Height=25 Caption='确定(&O)' TabOrder=6 OnClick=btnOKClick
        btnOK = new Button { Left = 782, Top = 589, Width = 82, Height = 25, Text = "确定(&O)", TabIndex = 6 };
        // DFM: Label7 Left=640 Top=595 Width=120 Height=12 Caption='以上参数调后立即生效'
        Label7 = new Label { Left = 640, Top = 595, Width = 120, Height = 12, Text = "以上参数调后立即生效" };

        // ---- GroupBox2 防CC处理 ----
        // DFM: GroupBox2 Left=505 Top=342 Width=151 Height=63 Caption='防CC处理' TabOrder=7
        GroupBox2 = new GroupBox { Left = 505, Top = 342, Width = 151, Height = 63, Text = "防CC处理", TabIndex = 7 };
        Label11 = new Label { Left = 10, Top = 18, Width = 78, Height = 12, Text = "防CC攻击时间:" };
        Label12 = new Label { Left = 10, Top = 41, Width = 78, Height = 12, Text = "'CC'攻击临界数:" };
        seAttackTick = new TSpinEdit { Left = 91, Top = 14, Width = 54, Height = 21 };
        seAttackCount = new TSpinEdit { Left = 91, Top = 36, Width = 54, Height = 21 };

        // ---- GroupBox6 防御设置 ----
        // DFM: GroupBox6 Left=663 Top=409 Width=201 Height=130 Caption='防御设置' TabOrder=8
        GroupBox6 = new GroupBox { Left = 663, Top = 409, Width = 201, Height = 130, Text = "防御设置", TabIndex = 8 };
        Label25 = new Label { Left = 8, Top = 18, Width = 54, Height = 12, Text = "防御等级:" };
        Label13 = new Label { Left = 120, Top = 107, Width = 12, Height = 12, Text = "秒" };
        Label14 = new Label { Left = 120, Top = 85, Width = 12, Height = 12, Text = "次" };
        Label15 = new Label { Left = 120, Top = 63, Width = 24, Height = 12, Text = "毫秒" };
        // DFM: trckbrDefenseLevel Left=64 Top=10 Width=133 Height=26
        trckbrDefenseLevel = new TrackBar { Left = 64, Top = 10, Width = 133, Height = 26,
                                            Minimum = 0, Maximum = 10, TickStyle = TickStyle.None };
        // DFM: chkDefenseToLevel1 Left=8 Top=40 Width=129 Height=17 Caption='受攻击防御调为"1"级'
        chkDefenseToLevel1 = new CheckBox { Left = 8, Top = 40, Width = 129, Height = 17, Text = "受攻击防御调为'1'级" };
        // DFM: chkResotreDefense Left=8 Top=62 Width=129 Height=17 Caption='无攻击还原防御等级'
        chkResotreDefense = new CheckBox { Left = 8, Top = 62, Width = 129, Height = 17, Text = "无攻击还原防御等级" };
        // DFM: chkAutoClearTemp Left=8 Top=84 Width=121 Height=17 Caption='清除动态过滤列表'
        chkAutoClearTemp = new CheckBox { Left = 8, Top = 84, Width = 121, Height = 17, Text = "清除动态过滤列表" };
        // DFM: chkAddAllToTemp Left=8 Top=106 Width=129 Height=17 Caption='连接加入到动态过滤'
        chkAddAllToTemp = new CheckBox { Left = 8, Top = 106, Width = 129, Height = 17, Text = "连接加入到动态过滤" };
        seDefenseToLevel1 = new TSpinEditEx { Left = 136, Top = 38, Width = 56, Height = 21 };
        seResotreDefense = new TSpinEditEx { Left = 136, Top = 60, Width = 56, Height = 21 };
        seAutoClearTemp = new TSpinEditEx { Left = 136, Top = 82, Width = 56, Height = 21 };
        seAddAllToTemp = new TSpinEditEx { Left = 136, Top = 104, Width = 56, Height = 21 };
        trckbrDefenseLevel.ValueChanged += (s, e) => trckbrDefenseLevel_Change(s, e);

        // ---- GroupBox5 发言设置 ----
        // DFM: GroupBox5 Left=505 Top=409 Width=151 Height=130 Caption='发言设置' TabOrder=9
        GroupBox5 = new GroupBox { Left = 505, Top = 409, Width = 151, Height = 130, Text = "发言设置", TabIndex = 9 };
        // DFM: chkSayMsgControl Left=8 Top=16 Width=105 Height=19 Caption='开启发言控制'
        chkSayMsgControl = new CheckBox { Left = 8, Top = 16, Width = 105, Height = 19, Text = "开启发言控制" };
        Label17 = new Label { Left = 8, Top = 41, Width = 54, Height = 12, Text = "文字长度:" };
        Label16 = new Label { Left = 8, Top = 63, Width = 54, Height = 12, Text = "时间间隔:" };
        Label18 = new Label { Left = 8, Top = 85, Width = 54, Height = 12, Text = "发言次数:" };
        Label19 = new Label { Left = 8, Top = 107, Width = 54, Height = 12, Text = "禁言时间:" };
        seSayMaxLen = new TSpinEdit { Left = 64, Top = 36, Width = 55, Height = 21 };
        seSayTime = new TSpinEdit { Left = 64, Top = 58, Width = 55, Height = 21 };
        seSayMaxCount = new TSpinEdit { Left = 64, Top = 80, Width = 55, Height = 21 };
        seSayDisableTime = new TSpinEdit { Left = 64, Top = 102, Width = 55, Height = 21 };
        chkSayMsgControl.Click += (s, e) => chkSayMsgControl_Click(s, e);

        // ---- grp5 验证客户端是否合法 ----
        // DFM: grp5 Left=505 Top=545 Width=359 Height=38 Caption='验证客户端是否合法' TabOrder=10
        grp5 = new GroupBox { Left = 505, Top = 545, Width = 359, Height = 38, Text = "验证客户端是否合法", TabIndex = 10 };
        // DFM: lbl3 Left=124 Top=18 Width=96 Height=12 Caption='客户端验证失败：'
        lbl3 = new Label { Left = 124, Top = 18, Width = 96, Height = 12, Text = "客户端验证失败：" };
        // DFM: chkOpenCheckClient Left=8 Top=16 Width=113 Height=17 Caption='启用客户端验证'
        chkOpenCheckClient = new CheckBox { Left = 8, Top = 16, Width = 113, Height = 17, Text = "启用客户端验证" };
        // DFM: cbbCheckClientFailBlockMode Left=216 Top=14 Width=136 Height=20 Style=csDropDownList ItemIndex=1
        //      Items=('断开'|'IP加入动态过滤列表')
        cbbCheckClientFailBlockMode = new ComboBox { Left = 216, Top = 14, Width = 136, Height = 20,
                                                     DropDownStyle = ComboBoxStyle.DropDownList, TabIndex = 1 };
        cbbCheckClientFailBlockMode.Items.AddRange(new object[] { "断开", "IP加入动态过滤列表" });
        cbbCheckClientFailBlockMode.SelectedIndex = 1;      // DFM: ItemIndex=1

        // ---- grp4 客户端非法包检查 ----
        // DFM: grp4 Left=663 Top=366 Width=201 Height=39 Caption='客户端非法包检查' TabOrder=11
        grp4 = new GroupBox { Left = 663, Top = 366, Width = 201, Height = 39, Text = "客户端非法包检查", TabIndex = 11 };
        // DFM: chkCheckClientPacketLegal Left=8 Top=16 Width=57 Height=17 Caption='数据包'
        chkCheckClientPacketLegal = new CheckBox { Left = 8, Top = 16, Width = 57, Height = 17, Text = "数据包" };
        seCheckClientPacketCount = new TSpinEditEx { Left = 65, Top = 14, Width = 55, Height = 21 };
        // DFM: lbl2 Left=123 Top=18 Width=72 Height=12 Caption='次非法为攻击'
        lbl2 = new Label { Left = 123, Top = 18, Width = 72, Height = 12, Text = "次非法为攻击" };

        // ---- 组装 ----
        grp1.Controls.AddRange(new Control[] { Label4, lstActive });
        GroupBox1.Controls.AddRange(new Control[] { LabelTempList, Label1, Label23, lstTemp, lstBlock, lstIpSection });
        grp3.Controls.AddRange(new Control[] { lbl1, Label5, lstTempMac, lstBlockMac });
        grp2.Controls.AddRange(new Control[] { Label2, Label3, Label9, Label10, Label22, Label24,
                                               seMaxConnect, seKeepConnectTimeOut,
                                               seIPCountLimitTime1, seIPCountLimit1, seIPCountLimitTime2, seIPCountLimit2 });
        GroupBox3.Controls.AddRange(new Control[] { rbDisConnect, rbAddTempList, rbAddBlockList });
        GroupBox4.Controls.AddRange(new Control[] { Label6, Label8, seMaxClientPacketSize, seMaxClientPacketCount, chkLostLine });
        GroupBox2.Controls.AddRange(new Control[] { Label11, Label12, seAttackTick, seAttackCount });
        GroupBox6.Controls.AddRange(new Control[] { Label25, Label13, Label14, Label15, trckbrDefenseLevel,
                                                    chkDefenseToLevel1, chkResotreDefense, chkAutoClearTemp, chkAddAllToTemp,
                                                    seDefenseToLevel1, seResotreDefense, seAutoClearTemp, seAddAllToTemp });
        GroupBox5.Controls.AddRange(new Control[] { chkSayMsgControl, Label17, Label16, Label18, Label19,
                                                    seSayMaxLen, seSayTime, seSayMaxCount, seSayDisableTime });
        grp5.Controls.AddRange(new Control[] { lbl3, chkOpenCheckClient, cbbCheckClientFailBlockMode });
        grp4.Controls.AddRange(new Control[] { chkCheckClientPacketLegal, seCheckClientPacketCount, lbl2 });

        Controls.AddRange(new Control[] { Label7, grp3, GroupBox1, grp2, GroupBox3, GroupBox4, btnOK,
                                          GroupBox2, GroupBox6, GroupBox5, grp1, grp5, grp4 });

        // DFM 的事件接线
        Load += (s, e) => FormCreate(s, e);                          // OnCreate=FormCreate
        btnOK.Click += (s, e) => btnOK_Click(s, e);                  // OnClick=btnOKClick
        mniTempSort.Click += (s, e) => mniTempSort_Click(s, e);
        mniTempDelete.Click += (s, e) => mniTempDelete_Click(s, e);
        mniTempClear.Click += (s, e) => mniTempClear_Click(s, e);
        mniTempAdd.Click += (s, e) => mniTempAdd_Click(s, e);
        mniTempRefresh.Click += (s, e) => mniTempRefresh_Click(s, e);
        mniTempAddToBlock.Click += (s, e) => mniTempAddToBlock_Click(s, e);
        mniTempAddAllToBlock.Click += (s, e) => mniTempAddAllToBlock_Click(s, e);
        mniBlockSort.Click += (s, e) => mniBlockSort_Click(s, e);
        mniBlockDelete.Click += (s, e) => mniBlockDelete_Click(s, e);
        mniBlockClear.Click += (s, e) => mniBlockClear_Click(s, e);
        mniBlockAdd.Click += (s, e) => mniBlockAdd_Click(s, e);
        mniBlockRefresh.Click += (s, e) => mniBlockRefresh_Click(s, e);
        mniBlockAddToTemp.Click += (s, e) => mniBlockAddToTemp_Click(s, e);
        mniBlockAddAllToTemp.Click += (s, e) => mniBlockAddAllToTemp_Click(s, e);
        mniIpSectionSort.Click += (s, e) => mniIpSectionSort_Click(s, e);
        mniIpSectionAdd.Click += (s, e) => mniIpSectionAdd_Click(s, e);
        mniIpSectionDel.Click += (s, e) => mniIpSectionDel_Click(s, e);
        mniActiveRefesh.Click += (s, e) => mniActiveRefesh_Click(s, e);
        mniActiveSort.Click += (s, e) => mniActiveSort_Click(s, e);
        mniActiveAddToTemp.Click += (s, e) => mniActiveAddToTemp_Click(s, e);
        mniActiveAddAllToTemp.Click += (s, e) => mniActiveAddAllToTemp_Click(s, e);
        mniActiveAddToBlock.Click += (s, e) => mniActiveAddToBlock_Click(s, e);
        mniActiveAddAllToBlock.Click += (s, e) => mniActiveAddAllToBlock_Click(s, e);
        mniActiveAddAllNoUserToTemp.Click += (s, e) => mniActiveAddAllNoUserToTemp_Click(s, e);
        mniActiveAddAllNoUserToBlock.Click += (s, e) => mniActiveAddAllNoUserToBlock_Click(s, e);
        mniActiveKick.Click += (s, e) => mniActiveKick_Click(s, e);
        mniTempMacRefresh.Click += (s, e) => mniTempMacRefresh_Click(s, e);
        mniTempMacSort.Click += (s, e) => mniTempMacSort_Click(s, e);
        mniTempMacAdd.Click += (s, e) => mniTempMacAdd_Click(s, e);
        mniTempMacDelete.Click += (s, e) => mniTempMacDelete_Click(s, e);
        mniTempMacClear.Click += (s, e) => mniTempMacClear_Click(s, e);
        mniTempMacAddToBlock.Click += (s, e) => mniTempMacAddToBlock_Click(s, e);
        mniTempMacAddAllToBlock.Click += (s, e) => mniTempMacAddAllToBlock_Click(s, e);
        mniBlockMacRefresh.Click += (s, e) => mniBlockMacRefresh_Click(s, e);
        mniBlockMacSort.Click += (s, e) => mniBlockMacSort_Click(s, e);
        mniBlockMacAdd.Click += (s, e) => mniBlockMacAdd_Click(s, e);
        mniBlockMacDelete.Click += (s, e) => mniBlockMacDelete_Click(s, e);
        mniBlockMacClear.Click += (s, e) => mniBlockMacClear_Click(s, e);
        mniBlockMacAddToTempMac.Click += (s, e) => mniBlockMacAddToTempMac_Click(s, e);
        mniBlockMacAddAllToTempMac.Click += (s, e) => mniBlockMacAddAllToTempMac_Click(s, e);
    }

    // =================================================================================
    // 原 uFrmSafeFilter.pas:476-480 FormCreate
    // =================================================================================
    /// <summary>原 :476-480 `TFrmSafeFilter.FormCreate`：只清空两个 listbox。</summary>
    public void FormCreate(object sender, EventArgs e)
    {
        lstTemp.Items.Clear();      // 原 :478
        lstBlock.Items.Clear();     // 原 :479
    }

    // =================================================================================
    // 原 :343-473 Open
    // =================================================================================
    /// <summary>原 :343-473 `TFrmSafeFilter.Open`（全局量/列表 → 控件）。</summary>
    public void Open()
    {
        lstActive.Items.Clear();          // 原 :351
        lstTemp.Items.Clear();            // 原 :352
        lstBlock.Items.Clear();           // 原 :353
        lstIpSection.Items.Clear();       // 原 :354
        lstTempMac.Items.Clear();         // 原 :355
        lstBlockMac.Items.Clear();        // 原 :356

        var host = Host;
        host.TempIPList.Lock();                                                   // 原 :358
        try
        {
            for (int i = 0; i < host.TempIPList.Count; i++)                       // 原 :360-361
                lstTemp.Items.Add(host.TempIPList[i]);
        }
        finally { host.TempIPList.UnLock(); }                                     // 原 :363

        host.BlockIPList.Lock();                                                  // 原 :366
        try
        {
            for (int i = 0; i < host.BlockIPList.Count; i++)                      // 原 :368-369
                lstBlock.Items.Add(host.BlockIPList[i]);
        }
        finally { host.BlockIPList.UnLock(); }                                    // 原 :371

        // 原 :374-398：在线连接列表（倒序遍历 + 只保留非空 RemoteAddr）
        if (ClientPool != null)
        {
            foreach (string item in SafeFilterLogic.BuildActiveItems(ClientPool.GetOnlineContextList()))
                lstActive.Items.Add(item);
        }

        // 原 :402-405：IP 段列表 → lstIpSection（原文直接遍历 g_IPSectionList 并 Format 成 'B - E'）
        lstIpSection.Items.Clear();
        foreach (var sec in Host.IPSectionList)                                   // 原 :402-405
            lstIpSection.Items.Add(sec.Display);

        host.TempMacList.Lock();                                                  // 原 :411
        try
        {
            for (int i = 0; i < host.TempMacList.Count; i++)                      // 原 :413-415
                lstTempMac.Items.Add(host.TempMacList[i]);
        }
        finally { host.TempMacList.UnLock(); }                                    // 原 :418

        host.BlockMacList.Lock();                                                 // 原 :421
        try
        {
            for (int i = 0; i < host.BlockMacList.Count; i++)                     // 原 :423-425
                lstBlockMac.Items.Add(host.BlockMacList[i]);
        }
        finally { host.BlockMacList.UnLock(); }                                   // 原 :428

        seMaxConnect.Value = FormGlobals.g_nMaxConnOfIPaddr;                      // 原 :431
        switch (FormGlobals.g_BlockMethod)                                        // 原 :432-436
        {
            case TBlockIPMethod.bmDisconnect: rbDisConnect.Checked = true; break;
            case TBlockIPMethod.bmTempBlock: rbAddTempList.Checked = true; break;
            case TBlockIPMethod.bmBlockList: rbAddBlockList.Checked = true; break;
        }

        seAttackTick.Value = (int)FormGlobals.g_dwAttackTick;                     // 原 :438
        seAttackCount.Value = FormGlobals.g_nAttackCount;                         // 原 :439
        seMaxClientPacketSize.Value = FormGlobals.g_nMaxClientPacketSize;         // 原 :440
        seMaxClientPacketCount.Value = FormGlobals.g_nMaxClientPacketCount;       // 原 :441
        chkLostLine.Checked = FormGlobals.g_boKickOverPacketSize;                 // 原 :442
        seKeepConnectTimeOut.Value = (int)FormGlobals.g_dwKeepConnectTimeOut;     // 原 :443
        chkCheckClientPacketLegal.Checked = FormGlobals.g_boCheckClientPacketLegal;   // 原 :445
        seCheckClientPacketCount.Value = FormGlobals.g_nCheckClientPacketCount;   // 原 :446
        chkSayMsgControl.Checked = FormGlobals.g_boSayMsgControl;                 // 原 :449
        chkSayMsgControl_Click(chkSayMsgControl, EventArgs.Empty);                // 原 :450
        seSayMaxLen.Value = (int)FormGlobals.g_dwSayMaxLen;                       // 原 :451
        seSayTime.Value = (int)FormGlobals.g_dwSayTime;                           // 原 :452
        seSayMaxCount.Value = (int)FormGlobals.g_dwSayMaxCount;                   // 原 :453
        seSayDisableTime.Value = (int)FormGlobals.g_dwSayDisableTime;             // 原 :454
        seIPCountLimitTime1.Value = (int)FormGlobals.g_dwIPCountLimitTime1;       // 原 :456
        seIPCountLimit1.Value = (int)FormGlobals.g_dwIPCountLimit1;               // 原 :457
        seIPCountLimitTime2.Value = (int)FormGlobals.g_dwIPCountLimitTime2;       // 原 :458
        seIPCountLimit2.Value = (int)FormGlobals.g_dwIPCountLimit2;               // 原 :459
        trckbrDefenseLevel.Value = ClampDefense(FormGlobals.g_dwDefenseLevel);    // 原 :461
        chkDefenseToLevel1.Checked = FormGlobals.g_boDefenseToLevel1;             // 原 :462
        seDefenseToLevel1.Value = (int)FormGlobals.g_dwDefenseToLevel1;           // 原 :463
        chkResotreDefense.Checked = FormGlobals.g_boResotreDefense;               // 原 :464
        seResotreDefense.Value = (int)FormGlobals.g_dwResotreDefense;             // 原 :465
        chkAutoClearTemp.Checked = FormGlobals.g_boAutoClearTemp;                 // 原 :466
        seAutoClearTemp.Value = (int)FormGlobals.g_dwAutoClearTemp;               // 原 :467
        chkAddAllToTemp.Checked = FormGlobals.g_boAddAllToTemp;                   // 原 :468
        seAddAllToTemp.Value = (int)FormGlobals.g_dwAddAllToTemp;                 // 原 :469
        chkOpenCheckClient.Checked = FormGlobals.g_boOpenCheckClient;             // 原 :471
        cbbCheckClientFailBlockMode.SelectedIndex = (int)FormGlobals.g_CheckClientFailBlockMethod;   // 原 :472
        UpdateHints();                                                            // 原 :463/858 的 Hint 刷新（原文由 trckbr 事件触发）
    }

    /// <summary>接缝辅助：把 Host 的 IPSectionList 拷一份（原文直接用 `g_IPSectionList.Items[I]`）。</summary>
    private List<IPSECTION> CollectIPSections()
    {
        var list = new List<IPSECTION>();
        for (int i = 0; i < Host.IPSectionList.Count; i++) list.Add(Host.IPSectionList[i]);
        return list;
    }

    private static int ClampDefense(uint v) => v > 10 ? 10 : (int)v;   // TrackBar 0..10（原 DFM 的等级范围）

    /// <summary>原 :861-881 `TFrmSafeFilter.UpdateHints`（5 组 Hint 成对赋值）。</summary>
    public void UpdateHints()
    {
        var hints = SafeFilterLogic.BuildHints(trckbrDefenseLevel.Value, seDefenseToLevel1.Value,
                                               seResotreDefense.Value, seAutoClearTemp.Value, seAddAllToTemp.Value);
        var tip = _tip ??= new ToolTip();
        tip.SetToolTip(trckbrDefenseLevel, hints["trckbrDefenseLevel"]);
        tip.SetToolTip(chkDefenseToLevel1, hints["chkDefenseToLevel1"]);
        tip.SetToolTip(seDefenseToLevel1, hints["seDefenseToLevel1"]);
        tip.SetToolTip(seResotreDefense, hints["seResotreDefense"]);
        tip.SetToolTip(chkResotreDefense, hints["chkResotreDefense"]);
        tip.SetToolTip(chkAutoClearTemp, hints["chkAutoClearTemp"]);
        tip.SetToolTip(seAutoClearTemp, hints["seAutoClearTemp"]);
        tip.SetToolTip(chkAddAllToTemp, hints["chkAddAllToTemp"]);
        tip.SetToolTip(seAddAllToTemp, hints["seAddAllToTemp"]);
    }

    private ToolTip _tip;

    /// <summary>原 :856-859 `trckbrDefenseLevelChange` → UpdateHints。</summary>
    public void trckbrDefenseLevel_Change(object sender, EventArgs e) => UpdateHints();   // 原 :858

    // =================================================================================
    // 原 :683-803 btnOKClick
    // =================================================================================
    /// <summary>原 :683-803 `TFrmSafeFilter.btnOKClick`。
    /// ★ 注意原文末尾是 `Close`（原 :802）而**不是** `ModalResult := mrOK`
    ///   —— 于是 `ShowFrmSafeFilter` 的 `ShowModal = mrOk` 永远为 False（原文缺陷 D10）。</summary>
    public void btnOK_Click(object sender, EventArgs e)
    {
        FormGlobals.g_BlockMethod = SafeFilterLogic.ResolveBlockMethod(rbDisConnect.Checked, rbAddTempList.Checked);   // 原 :687-692

        FormGlobals.g_nMaxClientPacketSize = seMaxClientPacketSize.Value;         // 原 :694
        FormGlobals.g_nMaxClientPacketCount = seMaxClientPacketCount.Value;       // 原 :695
        FormGlobals.g_boKickOverPacketSize = chkLostLine.Checked;                 // 原 :696
        FormGlobals.g_boCheckClientPacketLegal = chkCheckClientPacketLegal.Checked;   // 原 :698
        FormGlobals.g_nCheckClientPacketCount = seCheckClientPacketCount.Value;   // 原 :699
        FormGlobals.g_dwAttackTick = (uint)seAttackTick.Value;                    // 原 :701
        FormGlobals.g_nAttackCount = seAttackCount.Value;                         // 原 :702
        FormGlobals.g_nMaxConnOfIPaddr = seMaxConnect.Value;                      // 原 :703
        FormGlobals.g_dwKeepConnectTimeOut = (uint)seKeepConnectTimeOut.Value;    // 原 :704
        FormGlobals.g_dwIPCountLimitTime1 = (uint)seIPCountLimitTime1.Value;      // 原 :706
        FormGlobals.g_dwIPCountLimit1 = (uint)seIPCountLimit1.Value;              // 原 :707
        FormGlobals.g_dwIPCountLimitTime2 = (uint)seIPCountLimitTime2.Value;      // 原 :708
        FormGlobals.g_dwIPCountLimit2 = (uint)seIPCountLimit2.Value;              // 原 :709
        FormGlobals.g_dwSayMaxLen = (uint)seSayMaxLen.Value;                      // 原 :711
        FormGlobals.g_dwSayTime = (uint)seSayTime.Value;                          // 原 :712
        FormGlobals.g_dwSayMaxCount = (uint)seSayMaxCount.Value;                  // 原 :713
        FormGlobals.g_dwSayDisableTime = (uint)seSayDisableTime.Value;            // 原 :714
        FormGlobals.g_dwDefenseLevel = (uint)trckbrDefenseLevel.Value;            // 原 :716
        FormGlobals.g_boDefenseToLevel1 = chkDefenseToLevel1.Checked;             // 原 :718
        FormGlobals.g_dwDefenseToLevel1 = (uint)seDefenseToLevel1.Value;          // 原 :719
        FormGlobals.g_boResotreDefense = chkResotreDefense.Checked;               // 原 :721
        FormGlobals.g_dwResotreDefense = (uint)seResotreDefense.Value;            // 原 :722
        FormGlobals.g_boAutoClearTemp = chkAutoClearTemp.Checked;                 // 原 :724
        FormGlobals.g_dwAutoClearTemp = (uint)seAutoClearTemp.Value;              // 原 :725
        FormGlobals.g_boAddAllToTemp = chkAddAllToTemp.Checked;                   // 原 :727
        FormGlobals.g_dwAddAllToTemp = (uint)seAddAllToTemp.Value;                // 原 :728
        FormGlobals.g_boOpenCheckClient = chkOpenCheckClient.Checked;             // 原 :730
        FormGlobals.g_CheckClientFailBlockMethod = (TBlockIPMethod)cbbCheckClientFailBlockMode.SelectedIndex;   // 原 :731

        WriteConfigIni(FormGlobals.g_sIniFileName);                               // 原 :733-800

        Close();                                                                  // 原 :802（**不是** ModalResult := mrOK）
    }

    /// <summary>原 :733-800 的 27 个 INI 键（**键序即原文顺序**，逐字节照抄）。</summary>
    public static void WriteConfigIni(string iniFileName)
    {
        var ini = new TIniFileEx(iniFileName);                                    // 原 :733
        ini.WriteInteger(RunGateConst.GateClass, "AttackTick", (int)FormGlobals.g_dwAttackTick);                 // 原 :734
        ini.WriteInteger(RunGateConst.GateClass, "AttackCount", FormGlobals.g_nAttackCount);                     // 原 :735
        ini.WriteInteger(RunGateConst.GateClass, "MaxConnOfIPaddr", FormGlobals.g_nMaxConnOfIPaddr);             // 原 :736
        ini.WriteInteger(RunGateConst.GateClass, "BlockMethod", (int)FormGlobals.g_BlockMethod);                 // 原 :737
        ini.WriteInteger(RunGateConst.GateClass, "MaxClientPacketSize", FormGlobals.g_nMaxClientPacketSize);     // 原 :738
        ini.WriteInteger(RunGateConst.GateClass, "MaxClientPacketCount", FormGlobals.g_nMaxClientPacketCount);   // 原 :739
        // ★ D11：这两个键名相似但值来源不同 —— MaxClientMsgCount 用的是全局常量 nMaxClientMsgCount
        ini.WriteInteger(RunGateConst.GateClass, "MaxClientMsgCount", FormGlobals.nMaxClientMsgCount);           // 原 :740
        ini.WriteBool(RunGateConst.GateClass, "KickOverPacket", FormGlobals.g_boKickOverPacketSize ? (byte)1 : (byte)0);        // 原 :741
        ini.WriteBool(RunGateConst.GateClass, "CheckClientPacketLegal", FormGlobals.g_boCheckClientPacketLegal ? (byte)1 : (byte)0);  // 原 :743
        ini.WriteInteger(RunGateConst.GateClass, "CheckClientPacketCount", FormGlobals.g_nCheckClientPacketCount);   // 原 :744
        ini.WriteInteger(RunGateConst.GateClass, "KeepConnectTimeOut", (int)FormGlobals.g_dwKeepConnectTimeOut);     // 原 :747
        ini.WriteBool(RunGateConst.GateClass, "SayMsgControl", FormGlobals.g_boSayMsgControl ? (byte)1 : (byte)0);   // 原 :750
        ini.WriteInteger(RunGateConst.GateClass, "SayMaxLen", (int)FormGlobals.g_dwSayMaxLen);                       // 原 :753
        ini.WriteInteger(RunGateConst.GateClass, "SayTime", (int)FormGlobals.g_dwSayTime);                           // 原 :756
        ini.WriteInteger(RunGateConst.GateClass, "SayMaxCount", (int)FormGlobals.g_dwSayMaxCount);                   // 原 :759
        ini.WriteInteger(RunGateConst.GateClass, "SayDisableTime", (int)FormGlobals.g_dwSayDisableTime);             // 原 :762
        ini.WriteInteger(RunGateConst.GateClass, "IPCountLimitTime1", (int)FormGlobals.g_dwIPCountLimitTime1);       // 原 :764
        ini.WriteInteger(RunGateConst.GateClass, "IPCountLimit1", (int)FormGlobals.g_dwIPCountLimit1);               // 原 :765
        ini.WriteInteger(RunGateConst.GateClass, "IPCountLimitTime2", (int)FormGlobals.g_dwIPCountLimitTime2);       // 原 :766
        ini.WriteInteger(RunGateConst.GateClass, "IPCountLimit2", (int)FormGlobals.g_dwIPCountLimit2);               // 原 :767
        ini.WriteInteger(RunGateConst.GateClass, "DefenseLevel", (int)FormGlobals.g_dwDefenseLevel);                 // 原 :770
        ini.WriteBool(RunGateConst.GateClass, "IsDefenseToLevel1", FormGlobals.g_boDefenseToLevel1 ? (byte)1 : (byte)0);   // 原 :773
        ini.WriteInteger(RunGateConst.GateClass, "DefenseToLevel1", (int)FormGlobals.g_dwDefenseToLevel1);           // 原 :776
        ini.WriteBool(RunGateConst.GateClass, "IsResotreDefense", FormGlobals.g_boResotreDefense ? (byte)1 : (byte)0);     // 原 :779
        ini.WriteInteger(RunGateConst.GateClass, "ResotreDefense", (int)FormGlobals.g_dwResotreDefense);             // 原 :782
        ini.WriteBool(RunGateConst.GateClass, "IsAutoClearTemp", FormGlobals.g_boAutoClearTemp ? (byte)1 : (byte)0);       // 原 :785
        ini.WriteInteger(RunGateConst.GateClass, "AutoClearTemp", (int)FormGlobals.g_dwAutoClearTemp);               // 原 :788
        ini.WriteBool(RunGateConst.GateClass, "IsAddAllToTemp", FormGlobals.g_boAddAllToTemp ? (byte)1 : (byte)0);         // 原 :791
        ini.WriteInteger(RunGateConst.GateClass, "AddAllToTemp", (int)FormGlobals.g_dwAddAllToTemp);                 // 原 :794
        ini.WriteBool(RunGateConst.GateClass, "OpenCheckClient", FormGlobals.g_boOpenCheckClient ? (byte)1 : (byte)0);     // 原 :796
        ini.WriteInteger(RunGateConst.GateClass, "CheckClientFailBlockMethod", (int)FormGlobals.g_CheckClientFailBlockMethod);   // 原 :798
        ini.Dispose();                                                            // 原 :800 IniFile.Free
    }

    // =================================================================================
    // 原 :482-681 的 IP 列表菜单
    // =================================================================================
    public void mniTempSort_Click(object sender, EventArgs e) => lstTemp.Sorted = true;       // 原 :484
    public void mniBlockSort_Click(object sender, EventArgs e) => lstBlock.Sorted = true;     // 原 :558
    public void mniIpSectionSort_Click(object sender, EventArgs e) => lstIpSection.Sorted = true;   // 原 :1184
    public void mniTempMacSort_Click(object sender, EventArgs e) => lstTempMac.Sorted = true; // 原 :1264
    public void mniBlockMacSort_Click(object sender, EventArgs e) => lstBlockMac.Sorted = true;    // 原 :1377
    public void mniActiveSort_Click(object sender, EventArgs e) => lstActive.Sorted = true;   // 原 :920

    /// <summary>原 :487-499 `mniTempDeleteClick`。</summary>
    public void mniTempDelete_Click(object sender, EventArgs e)
    {
        if (lstTemp.SelectedIndex >= 0 && lstTemp.SelectedIndex < lstTemp.Items.Count)     // 原 :489
        {
            Host.TempIPList.Lock();                                                        // 原 :491
            try { Host.TempIPList.Delete(lstTemp.SelectedIndex); }                          // 原 :493（按**下标**删）
            finally { Host.TempIPList.UnLock(); }                                          // 原 :495
            lstTemp.Items.RemoveAt(lstTemp.SelectedIndex);                                 // 原 :497
        }
    }

    /// <summary>原 :501-511 `mniTempClearClick`。</summary>
    public void mniTempClear_Click(object sender, EventArgs e)
    {
        Host.TempIPList.Lock();                                                            // 原 :503
        try { Host.TempIPList.Clear(); }                                                   // 原 :505
        finally { Host.TempIPList.UnLock(); }                                              // 原 :507
        lstTemp.Items.Clear();                                                             // 原 :510
    }

    /// <summary>原 :513-535 `mniTempAddToBlockClick`。</summary>
    public void mniTempAddToBlock_Click(object sender, EventArgs e)
    {
        if (lstTemp.SelectedIndex >= 0 && lstTemp.SelectedIndex < lstTemp.Items.Count)     // 原 :517
        {
            string ip = lstTemp.Items[lstTemp.SelectedIndex].ToString();                   // 原 :519

            Host.TempIPList.Lock();                                                        // 原 :521
            try { Host.TempIPList.DeleteItem(ip); }                                            // 原 :523（按**值**删）
            finally { Host.TempIPList.UnLock(); }                                          // 原 :525

            lstTemp.Items.RemoveAt(lstTemp.SelectedIndex);                                 // 原 :528
            lstBlock.Items.Add(ip);                                                        // 原 :529
            Host.AddBlockIP(ip);                                                           // 原 :531
            Host.SaveBlockIPList();                                                        // 原 :533
        }
    }

    /// <summary>原 :537-554 `mniTempAddAllToBlockClick` —— ★ **D1 缺陷照抄**：
    /// 循环体用 `Items[ItemIndex]` 而不是 `Items[I]`，于是"全部加入"会重复加入当前选中项。</summary>
    public void mniTempAddAllToBlock_Click(object sender, EventArgs e)
    {
        for (int i = 0; i < lstTemp.Items.Count; i++)                                     // 原 :542
        {
            string ip = lstTemp.SelectedIndex >= 0 && lstTemp.SelectedIndex < lstTemp.Items.Count
                ? lstTemp.Items[lstTemp.SelectedIndex].ToString() : "";                    // 原 :544（原文 Items[ItemIndex]）
            lstBlock.Items.Add(ip);                                                        // 原 :545
            Host.AddBlockIP(ip);                                                           // 原 :547
        }
        Host.TempIPList.Clear();                                                           // 原 :550
        lstTemp.Items.Clear();                                                             // 原 :551
        Host.SaveBlockIPList();                                                            // 原 :553
    }

    /// <summary>原 :561-581 `mniBlockAddToTempClick`。</summary>
    public void mniBlockAddToTemp_Click(object sender, EventArgs e)
    {
        if (lstBlock.SelectedIndex >= 0 && lstBlock.SelectedIndex < lstBlock.Items.Count)   // 原 :565
        {
            string ip = lstBlock.Items[lstBlock.SelectedIndex].ToString();                  // 原 :567
            lstBlock.Items.RemoveAt(lstBlock.SelectedIndex);                                // 原 :568
            Host.BlockIPList.Lock();                                                        // 原 :570
            try { Host.BlockIPList.DeleteItem(ip); }                                            // 原 :572（按**值**删）
            finally { Host.BlockIPList.UnLock(); }                                          // 原 :574
            lstTemp.Items.Add(ip);                                                          // 原 :576
            Host.AddTempBlockIP(ip);                                                        // 原 :578
            Host.SaveBlockIPList();                                                         // 原 :579
        }
    }

    /// <summary>原 :583-599 `mniBlockDeleteClick`。</summary>
    public void mniBlockDelete_Click(object sender, EventArgs e)
    {
        if (lstBlock.SelectedIndex >= 0 && lstBlock.SelectedIndex < lstBlock.Items.Count)   // 原 :587
        {
            string ip = lstBlock.Items[lstBlock.SelectedIndex].ToString();                  // 原 :589
            lstBlock.Items.RemoveAt(lstBlock.SelectedIndex);                                // 原 :590
            Host.BlockIPList.Lock();                                                        // 原 :592
            try { Host.BlockIPList.DeleteItem(ip); }                                            // 原 :594
            finally { Host.BlockIPList.UnLock(); }                                          // 原 :596
        }
    }

    /// <summary>原 :601-605 `mniBlockClearClick` —— ★ **D2：没有 Lock/UnLock**。</summary>
    public void mniBlockClear_Click(object sender, EventArgs e)
    {
        Host.BlockIPList.Clear();        // 原 :603（原文**未加锁**）
        lstBlock.Items.Clear();          // 原 :604
    }

    /// <summary>原 :607-623 `mniBlockAddAllToTempClick`（这里用的是 `Items[I]` —— 与 D1 不同，是正确写法）。</summary>
    public void mniBlockAddAllToTemp_Click(object sender, EventArgs e)
    {
        for (int i = 0; i < lstBlock.Items.Count; i++)                                     // 原 :612
        {
            string ip = lstBlock.Items[i].ToString();                                       // 原 :614
            lstTemp.Items.Add(ip);                                                          // 原 :615
            Host.AddTempBlockIP(ip);                                                        // 原 :616
        }
        Host.BlockIPList.Clear();                                                           // 原 :619
        lstBlock.Items.Clear();                                                             // 原 :620
        Host.SaveBlockIPList();                                                             // 原 :622
    }

    /// <summary>原 :651-665 `mniTempRefreshClick`。</summary>
    public void mniTempRefresh_Click(object sender, EventArgs e)
    {
        lstTemp.Items.Clear();                                                              // 原 :655
        Host.TempIPList.Lock();                                                             // 原 :656
        try
        {
            for (int i = 0; i < Host.TempIPList.Count; i++)                                 // 原 :658-660
                lstTemp.Items.Add(Host.TempIPList[i]);
        }
        finally { Host.TempIPList.UnLock(); }                                               // 原 :662
    }

    /// <summary>原 :667-681 `mniBlockRefreshClick`。</summary>
    public void mniBlockRefresh_Click(object sender, EventArgs e)
    {
        lstBlock.Items.Clear();                                                             // 原 :671
        Host.BlockIPList.Lock();                                                            // 原 :672
        try
        {
            for (int i = 0; i < Host.BlockIPList.Count; i++)                                // 原 :674-676
                lstBlock.Items.Add(Host.BlockIPList[i]);
        }
        finally { Host.BlockIPList.UnLock(); }                                              // 原 :678
    }

    /// <summary>原 :805-824 `mniTempAddClick`（★ 提示文本与 mniBlockAddClick 不同，见 SafeFilterLogic）。</summary>
    public void mniTempAdd_Click(object sender, EventArgs e)
    {
        string ip = "";                                                                     // 原 :809
        if (!MessageBoxSeam.InputQueryWithValue(SafeFilterLogic.CaptionIpFilter, SafeFilterLogic.PromptIpInput, ref ip))
            return;                                                                         // 原 :810 Exit
        if (!RunGateNet.IsIPaddr(ip))                                                       // 原 :811
        {
            SafeFilterLogic.ErrMessage(SafeFilterLogic.MsgTempAddBadIp);                     // 原 :813
            return;                                                                         // 原 :814 Exit
        }
        lstTemp.Items.Add(ip);                                                              // 原 :816
        Host.TempIPList.Lock();                                                             // 原 :818
        try { Host.TempIPList.Add(ip); }                                                    // 原 :820
        finally { Host.TempIPList.UnLock(); }                                               // 原 :822
    }

    /// <summary>原 :826-845 `mniBlockAddClick`。</summary>
    public void mniBlockAdd_Click(object sender, EventArgs e)
    {
        string ip = "";                                                                     // 原 :830
        if (!MessageBoxSeam.InputQueryWithValue(SafeFilterLogic.CaptionIpFilter, SafeFilterLogic.PromptIpInput, ref ip))
            return;                                                                         // 原 :831 Exit
        if (!RunGateNet.IsIPaddr(ip))                                                       // 原 :832
        {
            SafeFilterLogic.ErrMessage(SafeFilterLogic.MsgBlockAddBadIp);                    // 原 :834
            return;                                                                         // 原 :835 Exit
        }
        lstBlock.Items.Add(ip);                                                             // 原 :837
        Host.BlockIPList.Lock();                                                            // 原 :838
        try { Host.BlockIPList.Add(ip); }                                                   // 原 :840
        finally { Host.BlockIPList.UnLock(); }                                              // 原 :842
        Host.SaveBlockIPList();                                                             // 原 :844
    }

    /// <summary>原 :847-854 `chkSayMsgControlClick`。</summary>
    public void chkSayMsgControl_Click(object sender, EventArgs e)
    {
        FormGlobals.g_boSayMsgControl = chkSayMsgControl.Checked;                           // 原 :849
        seSayMaxLen.Enabled = FormGlobals.g_boSayMsgControl;                                // 原 :850
        seSayTime.Enabled = FormGlobals.g_boSayMsgControl;                                  // 原 :851
        seSayMaxCount.Enabled = FormGlobals.g_boSayMsgControl;                              // 原 :852
        seSayDisableTime.Enabled = FormGlobals.g_boSayMsgControl;                           // 原 :853
    }

    // =================================================================================
    // 原 :883-1103 的"当前连接"菜单
    // =================================================================================
    /// <summary>原 :883-916 `mniActiveRefeshClick`。</summary>
    public void mniActiveRefesh_Click(object sender, EventArgs e)
    {
        lstActive.Items.Clear();                                                            // 原 :890
        if (ClientPool == null) return;
        foreach (string item in SafeFilterLogic.BuildActiveItems(ClientPool.GetOnlineContextList()))
            lstActive.Items.Add(item);                                                      // 原 :895-912
    }

    private ISafeFilterClient ActiveContextAt(int index)
    {
        if (ClientPool == null) return null;
        var list = ClientPool.GetOnlineContextList();
        // 原文 `Items.Objects[ItemIndex]` 与列表项一一对应；托管侧用序号回查（列表顺序一致）。
        int reversed = list.Count - 1 - index;
        return reversed >= 0 && reversed < list.Count ? list[reversed] : null;
    }

    /// <summary>原 :933-955 `mniActiveAddToTempClick`（★ 用**裸** MessageBox，见 D8）。</summary>
    public void mniActiveAddToTemp_Click(object sender, EventArgs e)
    {
        if (lstActive.SelectedIndex < 0 || lstActive.SelectedIndex >= lstActive.Items.Count) return;   // 原 :939
        string ip = SafeFilterLogic.GetIPAddrFromActiveItem(lstActive.Items[lstActive.SelectedIndex].ToString());   // 原 :941

        string msg = "将此IP加入到过态过滤列表后，此IP建立的所有连接将被强行中断，是否继续？";          // 原 :943
        string title = "确认信息 - " + ip;                                                              // 原 :944
        if (MessageBoxSeam.Show(msg, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
            return;                                                                                     // 原 :945 Exit

        lstTemp.Items.Add(ip);                                                                          // 原 :947
        Host.AddTempBlockIP(ip);                                                                        // 原 :948
        var ctx = ActiveContextAt(lstActive.SelectedIndex);                                             // 原 :950
        if (ctx != null && SameText(ctx.RemoteAddr, ip)) ctx.Close();                                    // 原 :951-952
        mniActiveRefesh_Click(this, EventArgs.Empty);                                                    // 原 :953
    }

    /// <summary>原 :957-977 `mniActiveAddAllToTempClick`。</summary>
    public void mniActiveAddAllToTemp_Click(object sender, EventArgs e)
    {
        if (!SafeFilterLogic.QuestionMessage("全部加入到过态过滤列表后，所有IP建立的所有连接将被强行中断，是否继续？", ""))
            return;                                                                                     // 原 :963 Exit

        for (int i = 0; i < lstActive.Items.Count; i++)                                                  // 原 :965
        {
            string ip = SafeFilterLogic.GetIPAddrFromActiveItem(lstActive.Items[i].ToString());          // 原 :967
            lstTemp.Items.Add(ip);                                                                       // 原 :969
            Host.AddTempBlockIP(ip);                                                                     // 原 :970
            var ctx = ActiveContextAt(i);                                                                // 原 :971
            if (ctx != null && SameText(ctx.RemoteAddr, ip)) ctx.Close();                                 // 原 :972-973
        }
        mniActiveRefesh_Click(this, EventArgs.Empty);                                                    // 原 :976
    }

    /// <summary>原 :979-1003 `mniActiveAddToBlockClick`。</summary>
    public void mniActiveAddToBlock_Click(object sender, EventArgs e)
    {
        if (lstActive.SelectedIndex < 0 || lstActive.SelectedIndex >= lstActive.Items.Count) return;   // 原 :985
        string ip = SafeFilterLogic.GetIPAddrFromActiveItem(lstActive.Items[lstActive.SelectedIndex].ToString());   // 原 :987

        string msg = "将此IP加入到永久过滤列表后，此IP建立的所有连接将被强行中断，是否继续？";          // 原 :989
        string title = "确认信息 - " + ip;                                                              // 原 :990
        if (!SafeFilterLogic.QuestionMessage(msg, title)) return;                                        // 原 :991 Exit

        lstBlock.Items.Add(ip);                                                                          // 原 :993
        Host.AddBlockIP(ip);                                                                             // 原 :994
        var ctx = ActiveContextAt(lstActive.SelectedIndex);                                              // 原 :995
        if (ctx != null && SameText(ctx.RemoteAddr, ip)) ctx.Close();                                    // 原 :997-998
        mniActiveRefesh_Click(this, EventArgs.Empty);                                                    // 原 :1000
        Host.SaveBlockIPList();                                                                          // 原 :1001
    }

    /// <summary>原 :1005-1027 `mniActiveAddAllToBlockClick`。</summary>
    public void mniActiveAddAllToBlock_Click(object sender, EventArgs e)
    {
        string msg = "全部加入到永久过滤列表后，所有IP建立的所有连接将被强行中断，是否继续？";           // 原 :1012
        if (!SafeFilterLogic.QuestionMessage(msg, "")) return;                                            // 原 :1013 Exit

        for (int i = 0; i < lstActive.Items.Count; i++)                                                   // 原 :1015
        {
            string ip = SafeFilterLogic.GetIPAddrFromActiveItem(lstActive.Items[i].ToString());           // 原 :1017
            lstBlock.Items.Add(ip);                                                                       // 原 :1018
            Host.AddBlockIP(ip);                                                                          // 原 :1019
            var ctx = ActiveContextAt(i);                                                                 // 原 :1020
            if (ctx != null && SameText(ctx.RemoteAddr, ip)) ctx.Close();                                  // 原 :1021-1022
        }
        mniActiveRefesh_Click(this, EventArgs.Empty);                                                     // 原 :1025
        Host.SaveBlockIPList();                                                                           // 原 :1026
    }

    /// <summary>原 :1029-1055 `mniActiveAddAllNoUserToTempClick`（只处理**用户名为空**的连接）。</summary>
    public void mniActiveAddAllNoUserToTemp_Click(object sender, EventArgs e)
    {
        string msg = "全部加入到过态过滤列表后，所有IP建立的所有连接将被强行中断，是否继续？";           // 原 :1036
        if (!SafeFilterLogic.QuestionMessage(msg, "")) return;                                            // 原 :1037 Exit

        for (int i = 0; i < lstActive.Items.Count; i++)                                                   // 原 :1039
        {
            string s = lstActive.Items[i].ToString();
            string ip = SafeFilterLogic.GetIPAddrFromActiveItem(s);                                        // 原 :1041
            string userName = SafeFilterLogic.GetUserNameFromActiveItem(s);                                // 原 :1042

            if (userName.Length == 0)                                                                      // 原 :1044
            {
                lstTemp.Items.Add(ip);                                                                     // 原 :1046
                Host.AddTempBlockIP(ip);                                                                   // 原 :1047
                var ctx = ActiveContextAt(i);                                                              // 原 :1048
                if (ctx != null && SameText(ctx.RemoteAddr, ip)) ctx.Close();                               // 原 :1049-1050
            }
        }
        mniActiveRefesh_Click(this, EventArgs.Empty);                                                      // 原 :1054
    }

    /// <summary>原 :1057-1085 `mniActiveAddAllNoUserToBlockClick`。</summary>
    public void mniActiveAddAllNoUserToBlock_Click(object sender, EventArgs e)
    {
        string msg = "全部加入到永久过滤列表后，所有IP建立的所有连接将被强行中断，是否继续？";           // 原 :1065
        if (!SafeFilterLogic.QuestionMessage(msg, "")) return;                                            // 原 :1066 Exit

        for (int i = 0; i < lstActive.Items.Count; i++)                                                   // 原 :1068
        {
            string s = lstActive.Items[i].ToString();
            string ip = SafeFilterLogic.GetIPAddrFromActiveItem(s);                                        // 原 :1070
            string userName = SafeFilterLogic.GetUserNameFromActiveItem(s);                                // 原 :1071

            if (userName.Length == 0)                                                                      // 原 :1073
            {
                lstBlock.Items.Add(ip);                                                                    // 原 :1075
                Host.AddBlockIP(ip);                                                                       // 原 :1076
                var ctx = ActiveContextAt(i);                                                              // 原 :1077
                if (ctx != null && SameText(ctx.RemoteAddr, ip)) ctx.Close();                               // 原 :1078-1079
            }
        }
        mniActiveRefesh_Click(this, EventArgs.Empty);                                                      // 原 :1083
        Host.SaveBlockIPList();                                                                            // 原 :1084
    }

    /// <summary>原 :1087-1103 `mniActiveKickClick`。</summary>
    public void mniActiveKick_Click(object sender, EventArgs e)
    {
        if (lstActive.SelectedIndex < 0 || lstActive.SelectedIndex >= lstActive.Items.Count) return;        // 原 :1091
        string s = lstActive.Items[lstActive.SelectedIndex].ToString();                                     // 原 :1093
        string ip = SafeFilterLogic.GetIPAddrFromActiveItem(s);                                             // 原 :1094
        string userName = SafeFilterLogic.GetUserNameFromActiveItem(s);                                     // 原 :1095

        if (SafeFilterLogic.QuestionMessage("是否确认将此连接断开？", "确认信息 - " + ip + " [" + userName + "]"))   // 原 :1097
        {
            ActiveContextAt(lstActive.SelectedIndex)?.Close();                                              // 原 :1099
            mniActiveRefesh_Click(this, EventArgs.Empty);                                                   // 原 :1100
        }
    }

    // =================================================================================
    // 原 :1120-1195 IP 段菜单
    // =================================================================================
    /// <summary>原 :1120-1152 `mniIpSectionAddClick`。</summary>
    public void mniIpSectionAdd_Click(object sender, EventArgs e)
    {
        string ip = "";                                                                                     // 原 :1126
        if (!MessageBoxSeam.InputQueryWithValue(SafeFilterLogic.CaptionIpSection, SafeFilterLogic.PromptIpSectionBegin, ref ip))
            return;                                                                                         // 原 :1127 Exit
        uint beginAddr = RunGateNet.IP2Long(ip);                                                            // 原 :1128
        if (beginAddr == RunGateNet.INADDR_NONE)                                                            // 原 :1129
        {
            MessageBoxSeam.ShowInformation(SafeFilterLogic.MsgIPFormatError, SafeFilterLogic.MsgIPFormatErrorTitle);   // 原 :1131
            return;                                                                                         // 原 :1132 Exit
        }

        if (!MessageBoxSeam.InputQueryWithValue(SafeFilterLogic.CaptionIpSection, SafeFilterLogic.PromptIpSectionEnd, ref ip))
            return;                                                                                         // 原 :1134 Exit
        uint endAddr = RunGateNet.IP2Long(ip);                                                              // 原 :1135
        if (endAddr == RunGateNet.INADDR_NONE)                                                              // 原 :1136
        {
            MessageBoxSeam.ShowInformation(SafeFilterLogic.MsgIPFormatError, SafeFilterLogic.MsgIPFormatErrorTitle);   // 原 :1138
            return;                                                                                         // 原 :1139 Exit
        }

        if (endAddr >= beginAddr)                                                                            // 原 :1141（等于也算通过）
        {
            var section = new IPSECTION { nBeginAddr = beginAddr, nEndAddr = endAddr };                      // 原 :1143-1145
            Host.IPSectionList.Add(section);                                                                 // 原 :1146
            lstIpSection.Items.Add(section.Display);                                                         // 原 :1147
            Host.SaveIPSectionList();                                                                        // 原 :1148
        }
        else
        {
            MessageBoxSeam.ShowInformation(SafeFilterLogic.MsgEndLessThanBegin, SafeFilterLogic.MsgIPFormatErrorTitle);   // 原 :1151
        }
    }

    /// <summary>原 :1154-1180 `mniIpSectionDelClick`。
    /// 原文按**指针身份**在 `g_IPSectionList` 里找到选中项并 `Delete(I)+Dispose`；
    /// 托管侧等价：先做快照，再重建为"去掉该项"的列表（**必须在使用前快照**，否则 Clear 后快照为空）。</summary>
    public void mniIpSectionDel_Click(object sender, EventArgs e)
    {
        if (lstIpSection.SelectedIndex < 0 || lstIpSection.SelectedIndex >= lstIpSection.Items.Count) return;   // 原 :1159
        var section = lstIpSection.SelectedIndex < Host.IPSectionList.Count
            ? Host.IPSectionList[lstIpSection.SelectedIndex] : null;                                            // 原 :1161
        lstIpSection.Items.RemoveAt(lstIpSection.SelectedIndex);                                                // 原 :1162

        // ★ 先快照，再 Clear（此前实现先 Clear 再取值 → 结果恒为空列表）
        var remaining = new List<IPSECTION>();
        // 原文此处用 `g_IPSectionList.Lock` / `Unlock`（原 :1164/:1176）；
        // 接缝把 IPSectionList 建模为普通 `List<IPSECTION>`（无锁），故不调用 Lock/UnLock。
        foreach (var s in Host.IPSectionList)                                                                   // 原 :1166-1174
            if (!ReferenceEquals(s, section)) remaining.Add(s);
        Host.IPSectionList.Clear();
        Host.IPSectionList.AddRange(remaining);

        Host.SaveIPSectionList();                                                                               // 原 :1179
    }

    /// <summary>原 :1187-1195 `pmIpSectionPopup`。</summary>
    public void pmIpSection_Popup(object sender, EventArgs e)
    {
        SafeFilterLogic.PopupStateIpSection(lstIpSection.Items.Count, lstIpSection.SelectedIndex,
                                            out bool sort, out bool del);
        mniIpSectionSort.Enabled = sort;     // 原 :1191
        mniIpSectionDel.Enabled = del;       // 原 :1194
    }

    // =================================================================================
    // 原 :1246-1467 MAC 菜单
    // =================================================================================
    /// <summary>原 :1246-1260 `mniTempMacRefreshClick`。</summary>
    public void mniTempMacRefresh_Click(object sender, EventArgs e)
    {
        lstTempMac.Items.Clear();                                                       // 原 :1250
        Host.TempMacList.Lock();                                                        // 原 :1251
        try
        {
            for (int i = 0; i < Host.TempMacList.Count; i++)                            // 原 :1253-1255
                lstTempMac.Items.Add(Host.TempMacList[i]);
        }
        finally { Host.TempMacList.UnLock(); }                                          // 原 :1258
    }

    /// <summary>原 :1267-1284 `mniTempMacAddClick`（去重后再加）。</summary>
    public void mniTempMacAdd_Click(object sender, EventArgs e)
    {
        string mac = "";                                                                // 原 :1271
        if (!MessageBoxSeam.InputQueryWithValue(SafeFilterLogic.CaptionTempMac, SafeFilterLogic.PromptMacInput, ref mac))
            return;                                                                     // 原 :1272 Exit

        Host.TempMacList.Lock();                                                        // 原 :1274
        try
        {
            if (Host.TempMacList.IndexOf(mac) < 0)                                      // 原 :1276
            {
                Host.TempMacList.Add(mac);                                              // 原 :1278
                lstTempMac.Items.Add(mac);                                              // 原 :1279
            }
        }
        finally { Host.TempMacList.UnLock(); }                                          // 原 :1282
    }

    /// <summary>原 :1286-1302 `mniTempMacDeleteClick`（按**值**定位后删）。</summary>
    public void mniTempMacDelete_Click(object sender, EventArgs e)
    {
        if (lstTempMac.SelectedIndex >= 0 && lstTempMac.SelectedIndex < lstTempMac.Items.Count)   // 原 :1290
        {
            Host.TempMacList.Lock();                                                              // 原 :1292
            try
            {
                int index = Host.TempMacList.IndexOf(lstTempMac.Items[lstTempMac.SelectedIndex].ToString());   // 原 :1294
                if (index >= 0) Host.TempMacList.Delete(index);                                    // 原 :1295-1296
            }
            finally { Host.TempMacList.UnLock(); }                                                 // 原 :1298
            lstTempMac.Items.RemoveAt(lstTempMac.SelectedIndex);                                   // 原 :1300
        }
    }

    /// <summary>原 :1304-1314 `mniTempMacClearClick`。</summary>
    public void mniTempMacClear_Click(object sender, EventArgs e)
    {
        Host.TempMacList.Lock();                                                        // 原 :1306
        try { Host.TempMacList.Clear(); }                                               // 原 :1308
        finally { Host.TempMacList.UnLock(); }                                          // 原 :1310
        lstTempMac.Items.Clear();                                                       // 原 :1313
    }

    /// <summary>原 :1316-1338 `mniTempMacAddToBlockClick` —— ★ **D3：用 UI 下标删列表**。</summary>
    public void mniTempMacAddToBlock_Click(object sender, EventArgs e)
    {
        if (lstTempMac.SelectedIndex >= 0 && lstTempMac.SelectedIndex < lstTempMac.Items.Count)   // 原 :1320
        {
            string mac = lstTempMac.Items[lstTempMac.SelectedIndex].ToString();                    // 原 :1322

            Host.TempMacList.Lock();                                                              // 原 :1324
            try { Host.TempMacList.Delete(lstTempMac.SelectedIndex); }                             // 原 :1326（★ 用下标）
            finally { Host.TempMacList.UnLock(); }                                                 // 原 :1328

            lstTempMac.Items.RemoveAt(lstTempMac.SelectedIndex);                                   // 原 :1331
            lstBlockMac.Items.Add(mac);                                                            // 原 :1332
            Host.AddBlockMac(mac);                                                                 // 原 :1334
            Host.SaveBlockMacList();                                                               // 原 :1336
        }
    }

    /// <summary>原 :1340-1357 `mniTempMacAddAllToBlockClick`（正确用 `Items[I]`）。</summary>
    public void mniTempMacAddAllToBlock_Click(object sender, EventArgs e)
    {
        for (int i = 0; i < lstTempMac.Items.Count; i++)                                          // 原 :1345
        {
            string mac = lstTempMac.Items[i].ToString();                                          // 原 :1347
            lstBlockMac.Items.Add(mac);                                                           // 原 :1348
            Host.AddBlockMac(mac);                                                                // 原 :1350
        }
        Host.TempMacList.Clear();                                                                 // 原 :1353
        lstTempMac.Items.Clear();                                                                 // 原 :1354
        Host.SaveBlockMacList();                                                                  // 原 :1356
    }

    /// <summary>原 :1359-1373 `mniBlockMacRefreshClick`。</summary>
    public void mniBlockMacRefresh_Click(object sender, EventArgs e)
    {
        lstBlockMac.Items.Clear();                                                              // 原 :1363
        Host.BlockMacList.Lock();                                                               // 原 :1364
        try
        {
            for (int i = 0; i < Host.BlockMacList.Count; i++)                                    // 原 :1366-1368
                lstBlockMac.Items.Add(Host.BlockMacList[i]);
        }
        finally { Host.BlockMacList.UnLock(); }                                                 // 原 :1371
    }

    /// <summary>原 :1380-1399 `mniBlockMacAddClick`。</summary>
    public void mniBlockMacAdd_Click(object sender, EventArgs e)
    {
        string mac = "";                                                                        // 原 :1384
        if (!MessageBoxSeam.InputQueryWithValue(SafeFilterLogic.CaptionBlockMac, SafeFilterLogic.PromptMacInput, ref mac))
            return;                                                                             // 原 :1385 Exit

        Host.BlockMacList.Lock();                                                               // 原 :1387
        try
        {
            if (Host.BlockMacList.IndexOf(mac) < 0)                                             // 原 :1389
            {
                Host.BlockMacList.Add(mac);                                                     // 原 :1391
                lstBlockMac.Items.Add(mac);                                                     // 原 :1392
            }
        }
        finally { Host.BlockMacList.UnLock(); }                                                 // 原 :1395
        Host.SaveBlockMacList();                                                                // 原 :1398
    }

    /// <summary>原 :1401-1420 `mniBlockMacDeleteClick`。</summary>
    public void mniBlockMacDelete_Click(object sender, EventArgs e)
    {
        if (lstBlockMac.SelectedIndex >= 0 && lstBlockMac.SelectedIndex < lstBlockMac.Items.Count)   // 原 :1405
        {
            Host.BlockMacList.Lock();                                                                // 原 :1407
            try
            {
                int index = Host.BlockMacList.IndexOf(lstBlockMac.Items[lstBlockMac.SelectedIndex].ToString());   // 原 :1409
                if (index >= 0) Host.BlockMacList.Delete(index);                                      // 原 :1411-1412
            }
            finally { Host.BlockMacList.UnLock(); }                                                   // 原 :1414
            lstBlockMac.Items.RemoveAt(lstBlockMac.SelectedIndex);                                     // 原 :1417
            Host.SaveBlockMacList();                                                                   // 原 :1418
        }
    }

    /// <summary>原 :1422-1427 `mniBlockMacClearClick`。</summary>
    public void mniBlockMacClear_Click(object sender, EventArgs e)
    {
        Host.BlockMacList.Clear();          // 原 :1424（原文同样**未加锁** —— 与 D2 同型）
        lstBlockMac.Items.Clear();          // 原 :1425
        Host.SaveBlockMacList();            // 原 :1426
    }

    /// <summary>原 :1429-1449 `mniBlockMacAddToTempMacClick` —— ★ **D4：用 UI 下标删列表**。</summary>
    public void mniBlockMacAddToTempMac_Click(object sender, EventArgs e)
    {
        if (lstBlockMac.SelectedIndex >= 0 && lstBlockMac.SelectedIndex < lstBlockMac.Items.Count)   // 原 :1433
        {
            string mac = lstBlockMac.Items[lstBlockMac.SelectedIndex].ToString();                     // 原 :1435

            Host.BlockMacList.Lock();                                                                 // 原 :1437
            try { Host.BlockMacList.Delete(lstBlockMac.SelectedIndex); }                               // 原 :1439（★ 用下标）
            finally { Host.BlockMacList.UnLock(); }                                                    // 原 :1441

            lstTempMac.Items.Add(mac);                                                                 // 原 :1443
            lstBlockMac.Items.RemoveAt(lstBlockMac.SelectedIndex);                                     // 原 :1444
            Host.AddTempBlockMac(mac);                                                                 // 原 :1446
            Host.SaveBlockMacList();                                                                   // 原 :1447
        }
    }

    /// <summary>原 :1451-1467 `mniBlockMacAddAllToTempMacClick`。</summary>
    public void mniBlockMacAddAllToTempMac_Click(object sender, EventArgs e)
    {
        for (int i = 0; i < lstBlockMac.Items.Count; i++)                                            // 原 :1456
        {
            string mac = lstBlockMac.Items[i].ToString();                                            // 原 :1458
            lstTempMac.Items.Add(mac);                                                                // 原 :1459
            Host.AddTempBlockMac(mac);                                                                // 原 :1460
        }
        Host.BlockMacList.Clear();                                                                    // 原 :1463
        lstBlockMac.Items.Clear();                                                                    // 原 :1464
        Host.SaveBlockMacList();                                                                      // 原 :1466
    }

    // =================================================================================
    // 原 :1469-1493 的 MAC 弹出菜单使能 + 原 :1210-1244 Ctrl+F 查找
    // =================================================================================
    /// <summary>原 :1469-1480 `pmTempMacPopup`。</summary>
    public void pmTempMac_Popup(object sender, EventArgs e)
    {
        SafeFilterLogic.PopupStateTemp(lstTempMac.Items.Count, lstTempMac.SelectedIndex,
            out bool sort, out bool clear, out bool addAll, out bool del, out bool addTo);
        mniTempMacSort.Enabled = sort;              // 原 :1473
        mniTempMacClear.Enabled = clear;            // 原 :1474
        mniTempMacAddAllToBlock.Enabled = addAll;   // 原 :1475
        mniTempMacDelete.Enabled = del;             // 原 :1478
        mniTempMacAddToBlock.Enabled = addTo;       // 原 :1479
    }

    /// <summary>原 :1482-1493 `pmBlockMacPopup`。</summary>
    public void pmBlockMac_Popup(object sender, EventArgs e)
    {
        SafeFilterLogic.PopupStateTemp(lstBlockMac.Items.Count, lstBlockMac.SelectedIndex,
            out bool sort, out bool clear, out bool addAll, out bool del, out bool addTo);
        mniBlockMacSort.Enabled = sort;                  // 原 :1486
        mniBlockMacClear.Enabled = clear;                // 原 :1487
        mniBlockMacAddAllToTempMac.Enabled = addAll;     // 原 :1488
        mniBlockMacDelete.Enabled = del;                 // 原 :1491
        mniBlockMacAddToTempMac.Enabled = addTo;         // 原 :1492
    }

    /// <summary>原 :625-636 `pmTempPopup`。</summary>
    public void pmTemp_Popup(object sender, EventArgs e)
    {
        SafeFilterLogic.PopupStateTemp(lstTemp.Items.Count, lstTemp.SelectedIndex,
            out bool sort, out bool clear, out bool addAll, out bool del, out bool addTo);
        mniTempSort.Enabled = sort;               // 原 :629
        mniTempClear.Enabled = clear;             // 原 :630
        mniTempAddAllToBlock.Enabled = addAll;    // 原 :631
        mniTempDelete.Enabled = del;              // 原 :634
        mniTempAddToBlock.Enabled = addTo;        // 原 :635
    }

    /// <summary>原 :638-649 `pmBlockPopup`。</summary>
    public void pmBlock_Popup(object sender, EventArgs e)
    {
        SafeFilterLogic.PopupStateTemp(lstBlock.Items.Count, lstBlock.SelectedIndex,
            out bool sort, out bool clear, out bool addAll, out bool del, out bool addTo);
        mniBlockSort.Enabled = sort;               // 原 :642
        mniBlockClear.Enabled = clear;             // 原 :643
        mniBlockAddAllToTemp.Enabled = addAll;     // 原 :644
        mniBlockDelete.Enabled = del;              // 原 :647
        mniBlockAddToTemp.Enabled = addTo;         // 原 :648
    }

    /// <summary>原 :1105-1118 `pmActiveChange`。</summary>
    public void pmActive_Change(object sender, EventArgs e)
    {
        SafeFilterLogic.PopupStateActive(lstActive.Items.Count, lstActive.SelectedIndex,
            out bool sort, out bool addAllToTemp, out bool addAllToBlock,
            out bool addToTemp, out bool addToBlock, out bool kick);
        mniActiveSort.Enabled = sort;                     // 原 :1110
        mniActiveAddAllToTemp.Enabled = addAllToTemp;     // 原 :1111
        mniActiveAddAllToBlock.Enabled = addAllToBlock;   // 原 :1112
        mniActiveAddToTemp.Enabled = addToTemp;           // 原 :1115
        mniActiveAddToBlock.Enabled = addToBlock;         // 原 :1116
        mniActiveKick.Enabled = kick;                     // 原 :1117
        // ★ 注：`mniActiveAddAllNoUserToTemp/Block`（原 :139-140 新增项）
        //   在原文的 pmActiveChange **未被赋值** → 恒为 **Enabled=False**（DFM 无 Enabled=True）。
        mniActiveAddAllNoUserToTemp.Enabled = false;
        mniActiveAddAllNoUserToBlock.Enabled = false;
    }

    /// <summary>原 :1210-1244 `lstActiveKeyDown`（Ctrl+F 查找，SameText 命中即定位）。</summary>
    public void lstActive_KeyDown(object sender, KeyEventArgs e)
    {
        if (!(sender is ListBox listBox)) return;                                        // 原 :1217（原文是 TListBox）

        if (e.Control && e.KeyCode == Keys.F)                                             // 原 :1220 `(ssCtrl in Shift) and (Key = Ord('F'))`
        {
            bool isMacList = ReferenceEquals(listBox, lstTempMac) || ReferenceEquals(listBox, lstBlockMac);   // 原 :1222
            SafeFilterLogic.InputQueryTitlesFor(isMacList, out string s1, out string s2);                     // 原 :1224-1230

            string input = "";
            if (MessageBoxSeam.InputQueryWithValue(s1, s2, ref input))                     // 原 :1232
            {
                var items = new List<string>();
                for (int i = 0; i < listBox.Items.Count; i++) items.Add(listBox.Items[i].ToString());
                int hit = SafeFilterLogic.FindListItem(items, input);                       // 原 :1234-1241
                if (hit >= 0) listBox.SelectedIndex = hit;                                  // 原 :1238
            }
        }
    }

    /// <summary>原 :368 `SameText`（大小写不敏感 + 忽略首尾空白）。</summary>
    private static bool SameText(string a, string b)
        => string.Equals((a ?? "").Trim(), (b ?? "").Trim(), StringComparison.OrdinalIgnoreCase);
}

/// <summary>默认的 <see cref="ISafeFilterHost"/> 实现：全部内存态（测试与未接线时可用）。</summary>
public class InMemorySafeFilterHost : ISafeFilterHost
{
    public TSafeHashStringList TempIPList { get; } = new TSafeHashStringList();
    public TSafeHashStringList BlockIPList { get; } = new TSafeHashStringList();
    public List<IPSECTION> IPSectionList { get; } = new List<IPSECTION>();
    public TSafeHashStringList TempMacList { get; } = new TSafeHashStringList();
    public TSafeHashStringList BlockMacList { get; } = new TSafeHashStringList();

    public int AddBlockIPCount;
    public int AddTempBlockIPCount;
    public int AddBlockMacCount;
    public int AddTempBlockMacCount;
    public int SaveBlockIPListCount;
    public int SaveIPSectionListCount;
    public int SaveBlockMacListCount;

    public void AddBlockIP(string ip) => AddBlockIPCount++;
    public void AddTempBlockIP(string ip) => AddTempBlockIPCount++;
    public void AddBlockMac(string mac) => AddBlockMacCount++;
    public void AddTempBlockMac(string mac) => AddTempBlockMacCount++;
    public void SaveBlockIPList() => SaveBlockIPListCount++;
    public void SaveIPSectionList() => SaveIPSectionListCount++;
    public void SaveBlockMacList() => SaveBlockMacListCount++;
}
