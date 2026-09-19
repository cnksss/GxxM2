unit GameCommand;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms, Dialogs, ComCtrls, StdCtrls, M2Share, SpinEditEx,
  VirtualTrees, Vcl.Samples.Spin;

type
  TfrmGameCmd = class(TForm)
    pgcMain: TPageControl;
    TabSheet1: TTabSheet;
    TabSheet2: TTabSheet;
    TabSheet3: TTabSheet;
    GroupBox1: TGroupBox;
    Label1: TLabel;
    edtUserCmdName: TEdit;
    seUserCmdPermission: TSpinEditEx;
    Label6: TLabel;
    btnUserCmdOK: TButton;
    lblUserCmdDesc: TLabel;
    lblUserCmd: TLabel;
    Label2: TLabel;
    Label3: TLabel;
    btnUserCmdSave: TButton;
    GroupBox2: TGroupBox;
    Label4: TLabel;
    Label5: TLabel;
    lblAdminCmdDesc: TLabel;
    lblAdminCmd: TLabel;
    Label7: TLabel;
    Label8: TLabel;
    edtAdminCmdName: TEdit;
    seAdminCmdPermission: TSpinEditEx;
    btnAdminCmdOK: TButton;
    btnAdminCmdSave: TButton;
    GroupBox3: TGroupBox;
    Label9: TLabel;
    Label10: TLabel;
    lblDebugCmdDesc: TLabel;
    lblDebugCmd: TLabel;
    Label11: TLabel;
    Label12: TLabel;
    edtDebugCmdName: TEdit;
    seDebugCmdPermission: TSpinEditEx;
    btnDebugCmdOK: TButton;
    btnDebugCmdSave: TButton;
    vstUserCmd: TVirtualStringTree;
    vstAdminCmd: TVirtualStringTree;
    vstDebugCmd: TVirtualStringTree;
    procedure FormCreate(Sender: TObject);
    procedure edtUserCmdNameChange(Sender: TObject);
    procedure seUserCmdPermissionChange(Sender: TObject);
    procedure btnUserCmdOKClick(Sender: TObject);
    procedure btnUserCmdSaveClick(Sender: TObject);
    procedure edtAdminCmdNameChange(Sender: TObject);
    procedure seAdminCmdPermissionChange(Sender: TObject);
    procedure btnAdminCmdOKClick(Sender: TObject);
    procedure edtDebugCmdNameChange(Sender: TObject);
    procedure seDebugCmdPermissionChange(Sender: TObject);
    procedure btnAdminCmdSaveClick(Sender: TObject);
    procedure btnDebugCmdSaveClick(Sender: TObject);
    procedure vstUserCmdGetText(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex; TextType: TVSTTextType; var
      CellText: string);
    procedure vstUserCmdFreeNode(Sender: TBaseVirtualTree; Node: PVirtualNode);
    procedure vstUserCmdAfterItemPaint(Sender: TBaseVirtualTree; TargetCanvas: TCanvas; Node: PVirtualNode; ItemRect: TRect);
    procedure vstUserCmdBeforeItemErase(Sender: TBaseVirtualTree; TargetCanvas: TCanvas; Node: PVirtualNode; ItemRect: TRect; var
      ItemColor: TColor; var EraseAction: TItemEraseAction);
    procedure vstUserCmdFocusChanged(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex);
    procedure vstAdminCmdFocusChanged(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex);
    procedure vstDebugCmdFocusChanged(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex);
    procedure btnDebugCmdOKClick(Sender: TObject);
  private
    procedure AddUserCommandToList();
    procedure AddAdminCommandToList();
    procedure AddDebugCommandToList();
    { Private declarations }
  public
    procedure Open();
    { Public declarations }
  end;

var
  frmGameCmd: TfrmGameCmd;

implementation

{$R *.dfm}

type
  PNodeData = ^TNodeData;

  TNodeData = record
    Index: Integer; // 命令编号
    IsPermission: BOOL; // 是否支持权限分配
    GameCmd: pTGameCmd;
    CommandParam: string; // 命令参数
    CommandDesc: string; // 命令说明
  end;

procedure TfrmGameCmd.FormCreate(Sender: TObject);
begin
  pgcMain.ActivePageIndex := 0;
  vstUserCmd.NodeDataSize := SizeOf(TNodeData);
  vstAdminCmd.NodeDataSize := SizeOf(TNodeData);
  vstDebugCmd.NodeDataSize := SizeOf(TNodeData);
end;

procedure TfrmGameCmd.Open;
begin
  AddUserCommandToList();
  AddAdminCommandToList();
  AddDebugCommandToList();
  ShowModal;
end;

procedure TfrmGameCmd.AddUserCommandToList;
var
  CmdCount: Integer;

  procedure AddCmdToList(GameCmd: pTGameCmd; sCmdParam, sDesc: string);
  var
    Node: PVirtualNode;
    NodeData: PNodeData;
  begin
    Inc(CmdCount);
    Node := vstUserCmd.AddChild(nil);
    NodeData := vstUserCmd.GetNodeData(Node);
    NodeData.Index := CmdCount;
    NodeData.IsPermission := False;
    NodeData.CommandParam := sCmdParam;
    NodeData.CommandDesc := sDesc;
    NodeData.GameCmd := GameCmd;
  end;

  procedure AddGroupToList(sGroupText: string);
  var
    Node: PVirtualNode;
    NodeData: PNodeData;
  begin
    Node := vstUserCmd.AddChild(nil);
    NodeData := vstUserCmd.GetNodeData(Node);
    FillChar(NodeData^, SizeOf(TNodeData), 0);
    NodeData.Index := 0;
    NodeData.CommandDesc := sGroupText;
  end;

begin
  CmdCount := 0;
  btnUserCmdOK.Enabled := False;
  AddGroupToList('聊天及信息');
  AddCmdToList(@g_GameCommand.PRVMSG, '人物名称', '禁止指定人物发的私聊信息');
  AddCmdToList(@g_GameCommand.ALLOWMSG, '', '禁止别人向自己发私聊信息');
  AddCmdToList(@g_GameCommand.LETSHOUT, '', '禁止接收组队聊天信息');
  AddCmdToList(@g_GameCommand.BANGUILDCHAT, '', '禁止接收行会聊天信息');
  AddCmdToList(@g_GameCommand.BANNATIONCHAT, '', '禁止接收国家聊天信息');
  AddCmdToList(@g_GameCommand.PUBLICCHAT, '', '禁止公聊');
  AddCmdToList(@g_GameCommand.CRYCHAT, '', '禁止喊话');
  AddGroupToList('其他开关');
  AddCmdToList(@g_GameCommand.LETTRADE, '', '禁止交易物品');
  AddCmdToList(@g_GameCommand.LETCHALLENGE, '', '禁止挑战');
  AddCmdToList(@g_GameCommand.LETNATION, '', '允许加入国家');
  AddGroupToList('行会相关');
  AddCmdToList(@g_GameCommand.LETGUILD, '', '允许加入行会');
  AddCmdToList(@g_GameCommand.AUTHALLY, '', '允许行会进入联盟');
  AddCmdToList(@g_GameCommand.AUTH, '', '开始进行行会联盟');
  AddCmdToList(@g_GameCommand.AUTHCANCEL, '', '取消行会联盟关系');
  AddCmdToList(@g_GameCommand.ENDGUILD, '', '退出当前所加入的行会');
  AddGroupToList('传送相关');
  AddCmdToList(@g_GameCommand.ALLOWGROUPCALL, '', '允许记忆传送');
  AddCmdToList(@g_GameCommand.ALLOWMASTERRECALL, '', '允许师徒传送');
  AddCmdToList(@g_GameCommand.ALLOWDEARRCALL, '', '允许夫妻传送');
  AddCmdToList(@g_GameCommand.AllowGuildReCall, '', '允许行会传送');
  AddCmdToList(@g_GameCommand.USERMOVE, 'X Y', '传送到某坐标(需佩戴传送戒指)');
  AddCmdToList(@g_GameCommand.DEARRECALL, '', '夫妻对方传送到身边');
  AddCmdToList(@g_GameCommand.MASTERECALL, '', '师父将徒弟召唤到身边');
  AddCmdToList(@g_GameCommand.GROUPRECALLL, '', '将组队人员传送到身边（需佩戴记忆全套装备）');
  AddCmdToList(@g_GameCommand.GUILDRECALLL, '', '将行会在线成员传送到身边（需要佩戴行会传送装备）');
  AddGroupToList('查询服务');
  AddCmdToList(@g_GameCommand.Data, '', '查看当前服务器日期时间');
  AddCmdToList(@g_GameCommand.DEAR, '', '查询夫妻位置');
  AddCmdToList(@g_GameCommand.MASTER, '', '查询师徒位置');
  AddCmdToList(@g_GameCommand.SEARCHING, '人物名称', '探测人物所在位置（需要穿戴探测装备）');
  AddGroupToList('人物操作');
  AddCmdToList(@g_GameCommand.UNLOCK, '', '开启登陆锁');
  AddCmdToList(@g_GameCommand.LOCKLOGON, '', '开启/关闭登陆锁');
  AddCmdToList(@g_GameCommand.SETPASSWORD, '', '设置仓库密码');
  AddCmdToList(@g_GameCommand.CHGPASSWORD, '', '修改仓库密码');
  AddCmdToList(@g_GameCommand.UNPASSWORD, '', '清除仓库密码（先开锁再清除密码）');
  AddCmdToList(@g_GameCommand.Lock, '', '将仓库锁上');
  AddCmdToList(@g_GameCommand.UNLOCKSTORAGE, '', '仓库解锁');
  AddCmdToList(@g_GameCommand.ATTACKMODE, '', '改变人物攻击模式');
  AddCmdToList(@g_GameCommand.RESTHERO, '', '改变英雄状态（跟随/休息/攻击)');
  AddCmdToList(@g_GameCommand.REST, '', '改变下属状态（休息/攻击)');
  AddCmdToList(@g_GameCommand.DISABLEHORSEINVITE, '', '禁止邀请上马');
  AddCmdToList(@g_GameCommand.TAKEONHORSE, '', '戴马牌后骑上马');
  AddCmdToList(@g_GameCommand.TAKEOFHORSE, '', '从马上下来');
  AddCmdToList(@g_GameCommand.OpenSellPlayer, '', '允许/禁止别人角色交易时向自己发出委托申请');
  AddCmdToList(@g_GameCommand.SendTopChatBoardMsg, '', '千里传音，传音筒命令');
  AddCmdToList(@g_GameCommand.MEMBERFUNCTION, '', '后台管理');
  AddCmdToList(@g_GameCommand.MEMBERFUNCTIONEX, '', '军团');
end;

procedure TfrmGameCmd.edtUserCmdNameChange(Sender: TObject);
begin
  btnUserCmdOK.Enabled := True;
  btnUserCmdSave.Enabled := True;
end;

procedure TfrmGameCmd.seUserCmdPermissionChange(Sender: TObject);
begin
  btnUserCmdOK.Enabled := True;
  btnUserCmdSave.Enabled := True;
end;

procedure TfrmGameCmd.btnUserCmdOKClick(Sender: TObject);
var
  sCommand: string;
  NodeData: PNodeData;
begin
  if vstUserCmd.FocusedNode = nil then
    Exit;
  NodeData := vstUserCmd.GetNodeData(vstUserCmd.FocusedNode);
  if NodeData.Index = 0 then
    Exit;
  sCommand := Trim(edtUserCmdName.Text);
  if sCommand = '' then
  begin
    Application.MessageBox('命令名称不能为空！', '提示信息', MB_OK + MB_ICONERROR);
    edtUserCmdName.SetFocus;
    Exit;
  end;
  NodeData.GameCmd.sCmd := sCommand;
  NodeData.GameCmd.nPermissionMin := seUserCmdPermission.Value;
  vstUserCmd.InvalidateNode(vstUserCmd.FocusedNode);
end;

procedure TfrmGameCmd.btnUserCmdSaveClick(Sender: TObject);
begin
  btnUserCmdSave.Enabled := False;
  CommandConf.WriteString('Command', 'Date', g_GameCommand.Data.sCmd);
  CommandConf.WriteString('Command', 'PrvMsg', g_GameCommand.PRVMSG.sCmd);
  CommandConf.WriteString('Command', 'AllowMsg', g_GameCommand.ALLOWMSG.sCmd);
  CommandConf.WriteString('Command', 'LetShout', g_GameCommand.LETSHOUT.sCmd);
  CommandConf.WriteString('Command', 'LetTrade', g_GameCommand.LETTRADE.sCmd);
  CommandConf.WriteString('Command', 'LetGuild', g_GameCommand.LETGUILD.sCmd);
  // 允许/禁止挑战 piaoyun 2013-07-22
  CommandConf.WriteString('Command', 'LetChallenge', g_GameCommand.LETCHALLENGE.sCmd);
  CommandConf.WriteString('Command', 'EndGuild', g_GameCommand.ENDGUILD.sCmd);
  CommandConf.WriteString('Command', 'BanGuildChat', g_GameCommand.BANGUILDCHAT.sCmd);
  CommandConf.WriteString('Command', 'AuthAlly', g_GameCommand.AUTHALLY.sCmd);
  CommandConf.WriteString('Command', 'Auth', g_GameCommand.AUTH.sCmd);
  CommandConf.WriteString('Command', 'AuthCancel', g_GameCommand.AUTHCANCEL.sCmd);
  CommandConf.WriteString('Command', 'ViewDiary', g_GameCommand.DIARY.sCmd);
  CommandConf.WriteString('Command', 'UserMove', g_GameCommand.USERMOVE.sCmd);
  CommandConf.WriteString('Command', 'Searching', g_GameCommand.SEARCHING.sCmd);
  CommandConf.WriteString('Command', 'AllowGroupCall', g_GameCommand.ALLOWGROUPCALL.sCmd);
  CommandConf.WriteString('Command', 'GroupCall', g_GameCommand.GROUPRECALLL.sCmd);
  CommandConf.WriteString('Command', 'AllowGuildReCall', g_GameCommand.AllowGuildReCall.sCmd);
  CommandConf.WriteString('Command', 'GuildReCall', g_GameCommand.GUILDRECALLL.sCmd);
  CommandConf.WriteString('Command', 'StorageUnLock', g_GameCommand.UNLOCKSTORAGE.sCmd);
  CommandConf.WriteString('Command', 'PasswordUnLock', g_GameCommand.UNLOCK.sCmd);
  CommandConf.WriteString('Command', 'StorageLock', g_GameCommand.Lock.sCmd);
  CommandConf.WriteString('Command', 'StorageSetPassword', g_GameCommand.SETPASSWORD.sCmd);
  CommandConf.WriteString('Command', 'StorageChgPassword', g_GameCommand.CHGPASSWORD.sCmd);
  // CommandConf.WriteString('Command','StorageClearPassword',g_GameCommand.CLRPASSWORD.sCmd)
  // CommandConf.WriteInteger('Permission','StorageClearPassword', g_GameCommand.CLRPASSWORD.nPermissionMin)
  CommandConf.WriteString('Command', 'StorageUserClearPassword', g_GameCommand.UNPASSWORD.sCmd);
  CommandConf.WriteString('Command', 'MemberFunc', g_GameCommand.MEMBERFUNCTION.sCmd);
  CommandConf.WriteString('Command', 'Dear', g_GameCommand.DEAR.sCmd);
  CommandConf.WriteString('Command', 'Master', g_GameCommand.MASTER.sCmd);
  CommandConf.WriteString('Command', 'DearRecall', g_GameCommand.DEARRECALL.sCmd);
  CommandConf.WriteString('Command', 'MasterRecall', g_GameCommand.MASTERECALL.sCmd);
  CommandConf.WriteString('Command', 'AllowDearRecall', g_GameCommand.ALLOWDEARRCALL.sCmd);
  CommandConf.WriteString('Command', 'AllowMasterRecall', g_GameCommand.ALLOWMASTERRECALL.sCmd);
  CommandConf.WriteString('Command', 'AttackMode', g_GameCommand.ATTACKMODE.sCmd);
  CommandConf.WriteString('Command', 'Rest', g_GameCommand.REST.sCmd);
  CommandConf.WriteString('Command', 'TakeOnHorse', g_GameCommand.TAKEONHORSE.sCmd);
  CommandConf.WriteString('Command', 'TakeOffHorse', g_GameCommand.TAKEOFHORSE.sCmd);
  CommandConf.WriteString('Command', 'DisableHorseInvite', g_GameCommand.DISABLEHORSEINVITE.sCmd);
  CommandConf.WriteString('Command', 'SendTopChatBoardMsg', g_GameCommand.SendTopChatBoardMsg.sCmd);
  CommandConf.WriteString('Command', 'PublicChat', g_GameCommand.PUBLICCHAT.sCmd);
  CommandConf.WriteString('Command', 'CryChat', g_GameCommand.CRYCHAT.sCmd);
  CommandConf.WriteString('Command', 'OpenSellPlayer', g_GameCommand.OpenSellPlayer.sCmd);
  CommandConf.WriteInteger('Permission', 'Date', g_GameCommand.Data.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'PrvMsg', g_GameCommand.PRVMSG.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'AllowMsg', g_GameCommand.ALLOWMSG.nPermissionMin);
end;

procedure TfrmGameCmd.AddAdminCommandToList;
var
  CmdCount: Integer;

  procedure AddCmdToList(GameCmd: pTGameCmd; IsPermission: Boolean; sCmdParam, sDesc: string);
  var
    Node: PVirtualNode;
    NodeData: PNodeData;
  begin
    Inc(CmdCount);
    Node := vstAdminCmd.AddChild(nil);
    NodeData := vstAdminCmd.GetNodeData(Node);
    NodeData.Index := CmdCount;
    NodeData.IsPermission := IsPermission;
    NodeData.CommandParam := sCmdParam;
    NodeData.CommandDesc := sDesc;
    NodeData.GameCmd := GameCmd;
  end;

  procedure AddGroupToList(sGroupText: string);
  var
    Node: PVirtualNode;
    NodeData: PNodeData;
  begin
    Node := vstAdminCmd.AddChild(nil);
    NodeData := vstAdminCmd.GetNodeData(Node);
    FillChar(NodeData^, SizeOf(TNodeData), 0);
    NodeData.Index := 0;
    NodeData.CommandDesc := sGroupText;
  end;

begin
  btnAdminCmdOK.Enabled := False;
  CmdCount := 0;
  AddGroupToList('人物状态操作');
  AddCmdToList(@g_GameCommand.GAMEMASTER, True, '', '进入/退出管理员模式(进入模式后不会受到任何角色攻击)');
  AddCmdToList(@g_GameCommand.OBSERVER, True, '', '进入/退出隐身模式(进入模式后别人看不到自己)');
  AddCmdToList(@g_GameCommand.SUEPRMAN, True, '', '进入/退出无敌模式(进入模式后人物不会死亡)');
  AddCmdToList(@g_GameCommand.KICK, True, '人物名', '将指定人物踢下线');
  AddCmdToList(@g_GameCommand.ReAlive, True, '人物名', '将指定人物复活');
  AddCmdToList(@g_GameCommand.KILL, True, '人物名', '将人物或怪物杀死(杀怪物时需面对怪物)');
  AddGroupToList('人物调整');
  AddCmdToList(@g_GameCommand.CHANGEJOB, True, '人物名 职业(Warr/Wizard/Taos)', '调整人物的职业');
  AddCmdToList(@g_GameCommand.CHANGEGENDER, True, '人物名 性别(男/女)', '调整人物的性别');
  AddCmdToList(@g_GameCommand.HAIR, True, '人物名 发型', '调整人物的发型');
  AddCmdToList(@g_GameCommand.ADJUESTLEVEL, True, '人物名 等级', '调整人物的等级');
  AddCmdToList(@g_GameCommand.Level, True, '', '调整自己的等级');
  AddCmdToList(@g_GameCommand.NGLevel, True, '人物名 内功等级', '调整人物的内功等级');
  AddCmdToList(@g_GameCommand.RENEWLEVEL, True, '人物名 点数(为空则查看)', '调整/查看人物的转生等级');
  AddCmdToList(@g_GameCommand.ADJUESTEXP, True, '人物名 经验值', '调整指定人物的经验值');
  AddCmdToList(@g_GameCommand.SETPERMISSION, True, '人物名 权限等级(0-10)', '调整人物的权限等级(可升GM权限)');
  AddCmdToList(@g_GameCommand.Attack, False, '攻击模式', '设置自己的攻击模式');
  AddCmdToList(@g_GameCommand.HUNGER, True, '人物名 能量值', '调整人物的能量值');
  AddCmdToList(@g_GameCommand.PKPOINT, True, '人物名', '查看人物的PK值');
  AddCmdToList(@g_GameCommand.IncPkPoint, True, '人物名 点数', '增加人物的PK值');
  AddCmdToList(@g_GameCommand.FREEPENALTY, True, '人物名', '清除人物的PK值，同时清零此人英雄的PK值');
  AddCmdToList(@g_GameCommand.BonusPoint, True, '人物名 属性点数', '调整人物的属性点数');
  AddCmdToList(@g_GameCommand.DELBONUSPOINT, True, '人物名', '删除人物的属性点数');
  AddCmdToList(@g_GameCommand.RESTBONUSPOINT, True, '人物名', '将人物的属性点数重新分配');
  AddCmdToList(@g_GameCommand.CLEARMISSION, True, '人物名', '清除人物的任务标志');
  AddCmdToList(@g_GameCommand.CLRPASSWORD, True, '人物名', '清除人物仓库/登录密码');
  AddCmdToList(@g_GameCommand.CHANGEDEARNAME, True, '人物名 配偶名称(如果为 无 则清除)', '更改人物的配偶名称');
  AddCmdToList(@g_GameCommand.CHANGEMASTERNAME, True, '人物名 师徒名称(如果为 无 则清除)', '更改人物的师徒名称');
  AddCmdToList(@g_GameCommand.DelUserShop, True, '人物名', '删除人物的个人商店');
  AddCmdToList(@g_GameCommand.SetUserShopName, True, '人物名 新商店名称', '修改人物的个人商店名称');
  AddCmdToList(@g_GameCommand.DelSellPlayer, True, '人物名', '下架出售的角色');
  AddCmdToList(@g_GameCommand.DELGOLD, True, '人物名 数量', '减少人物金币数量');
  AddCmdToList(@g_GameCommand.ADDGOLD, True, '人物名 数量', '增加人物金币数量');
  AddCmdToList(@g_GameCommand.GAMEGOLD, True, '人物名 控制符(+ - =) 数量', '调整人物的游戏币数量');
  AddCmdToList(@g_GameCommand.GAMEPOINT, True, '人物名 控制符(+ - =) 数量', '调整人物的游戏点数量');
  AddCmdToList(@g_GameCommand.CHANGEGAMEDIAMOND, True, '物名 控制符(+,-,=) 数量', '调整金刚石数量');
  AddCmdToList(@g_GameCommand.CHANGEGAMEGIRD, True, '物名 控制符(+,-,=) 数量', '调整灵符数量');
  AddCmdToList(@g_GameCommand.CREDITPOINT, True, '人物名 控制符(+ - =) 点数', '调整人物的声望点数');
  AddCmdToList(@g_GameCommand.LUCKYPOINT, True, '物名 控制符 幸运点数', '调整人物的幸运点数');
  AddCmdToList(@g_GameCommand.CHANGEGAMEGLORY, True, '物名 控制符(+,-,=) 数量', '调整荣誉值数量');
  AddGroupToList('人物技能');
  AddCmdToList(@g_GameCommand.TRAININGSKILL, True, '人物名  技能名称 修炼等级(0-3)', '增加人物的技能');
  AddCmdToList(@g_GameCommand.DELETESKILL, True, '人物名 技能名称(All)', '删除人物的技能，All代表删除全部技能');
  AddCmdToList(@g_GameCommand.TRAINING, True, '人物名  技能名称 修炼等级(0-3)', '调整人物的技能等级');
  AddGroupToList('人物移动/传送');
  AddCmdToList(@g_GameCommand.POSITIONMOVE, True, '地图号 X Y', '移动到地图指定坐标(小于最大权限受受禁止传送地图列表限制)');
  AddCmdToList(@g_GameCommand.Move, True, '地图号', '移动到地图随机坐标(小于最大权限受受禁止传送地图列表限制)');
  AddCmdToList(@g_GameCommand.TING, True, '人物名', '随机传送人物');
  AddCmdToList(@g_GameCommand.RECALL, True, '人物名', '召唤人物到身边');
  AddCmdToList(@g_GameCommand.SUPERTING, True, '人物名 范围大小', '将指定人物身边范围内的人物随机传送');
  AddCmdToList(@g_GameCommand.MAPMOVE, True, '源地图号 目标地图号', '将整个地图中的人物移动到其它地图中');
  AddCmdToList(@g_GameCommand.REGOTO, True, '人物名', '跟踪人物');
  AddGroupToList('物品操作');
  AddCmdToList(@g_GameCommand.MAKE, True, '物品名称 数量', '制造物品/刷物品');
  AddCmdToList(@g_GameCommand.GiveMine, True, '物品名称 数量 当前持久', '制造物品/刷物品');
  AddCmdToList(@g_GameCommand.SMAKE, True, '参数详见使用说明', '调整自己身上的物品属性');
  AddCmdToList(@g_GameCommand.REFINEWEAPON, True, '攻击力 魔法力 道术 准确度', '调整身上武器属性');
  AddCmdToList(@g_GameCommand.DELETEITEM, True, '人物名 物品名称 数量', '删除人物身上指定的物品');
  AddGroupToList('怪物/宝宝操作');
  AddCmdToList(@g_GameCommand.MOB, True, '怪物名称 数量 等级 国家代码 同国家玩家是否攻击 不同国家怪物是否PK 怪物颜色', '在身边放置怪物/刷怪');
  AddCmdToList(@g_GameCommand.RECALLMOB, True, '怪物名称 数量 召唤等级', '召唤怪物宝宝');
  AddCmdToList(@g_GameCommand.Mission, True, 'X  Y', '设置怪物的集中点(举行怪物攻城用)');
  AddCmdToList(@g_GameCommand.MobPlace, True, 'X  Y 怪物名称 怪物数量',
    '在当前地图指定XY放置怪物(先必须设置怪物的集中点，放置的怪物大刀守卫不会攻击这些怪物');
  AddCmdToList(@g_GameCommand.CLEARMON, True, '地图号(* 为所有) 怪物名称(* 为所有) 掉物品(0,1)', '清除地图中的怪物');
  AddCmdToList(@g_GameCommand.SPIRIT, False, '时间', '祈祷生效宝宝叛变');
  AddCmdToList(@g_GameCommand.SPIRITSTOP, False, '', '祈祷叛变已停止');
  AddCmdToList(@g_GameCommand.LoyaltyPoint, True, '', '调整英雄忠诚度(调整点数/100，有效点数0-10000)');
  AddGroupToList('地图操作');
  AddCmdToList(@g_GameCommand.Map, True, '', '显示当前所在地图相关信息');
  AddCmdToList(@g_GameCommand.SHOWMAPMODE, False, '', '显示地图模式');
  AddCmdToList(@g_GameCommand.SetMapMode, False, '地图号 模式', '设置地图模式');
  AddCmdToList(@g_GameCommand.MOBCOUNT, True, '地图号', '查看地图中怪物数量');
  AddCmdToList(@g_GameCommand.HUMANLOCAL, True, '地图号', '查询人物IP所在地区(需加载IP地区查询插件)');
  AddGroupToList('行会操作');
  AddCmdToList(@g_GameCommand.AddGuild, True, '行会名称 掌门人', '新建一个行会');
  AddCmdToList(@g_GameCommand.DELGUILD, True, '行会名称', '删除一个行会');
  AddCmdToList(@g_GameCommand.CHANGESABUKLORD, True, '行会名称', '更改城堡所属行会');
  AddCmdToList(@g_GameCommand.FORCEDWALLCONQUESTWAR, True, '', '强行开始/停止攻城战');
  AddCmdToList(@g_GameCommand.STARTCONTEST, True, '', '开始行会争霸赛');
  AddCmdToList(@g_GameCommand.CONTESTPOINT, True, '行会名称', '查看行会争霸赛得分情况');
  AddCmdToList(@g_GameCommand.ENDCONTEST, True, '', '结束行会争霸赛');
  AddCmdToList(@g_GameCommand.ANNOUNCEMENT, True, '', '查看行会争霸赛结果');
  AddGroupToList('信息查看');
  AddCmdToList(@g_GameCommand.WHO, True, '', '查看当前服务器在线人数');
  AddCmdToList(@g_GameCommand.TOTAL, True, '', '查看所有服务器在线人数');
  AddCmdToList(@g_GameCommand.INFO, True, '人物名', '看人物信息');
  AddCmdToList(@g_GameCommand.VIEWWHISPER, True, '人物名', '查看指定人物的私聊信息');
  AddCmdToList(@g_GameCommand.MOBLEVEL, True, '', '查看身边角色信息');
  AddCmdToList(@g_GameCommand.HUMANCOUNT, True, '', '查看身边人数');
  AddCmdToList(@g_GameCommand.SABUKWALLGOLD, True, '', '查看城堡金币数');
  AddCmdToList(@g_GameCommand.MACHINEID, True, '', '取用户的机器码');
  AddGroupToList('发言/禁言设置');
  AddCmdToList(@g_GameCommand.DISABLESENDMSG, True, '人物名',
    '将指定人物加入发言过滤列表，加入列表后自己发的文字自己可以看到，其他人看不到');
  AddCmdToList(@g_GameCommand.ENABLESENDMSG, True, '', '将指定人物从发言过滤列表中删除');
  AddCmdToList(@g_GameCommand.DISABLESENDMSGLIST, True, '', '查看发言过滤列表中的内容');
  AddCmdToList(@g_GameCommand.SHUTUP, True, '人物名', '将指定人物禁言');
  AddCmdToList(@g_GameCommand.RELEASESHUTUP, True, '人物名', '将指定人物从禁言列表中删除');
  AddCmdToList(@g_GameCommand.SHUTUPLIST, True, '', '查看禁言列表中的内容');
  AddGroupToList('登录限制/封号');
  AddCmdToList(@g_GameCommand.DENYACCOUNTLOGON, True, '登录帐号 是否永久封(0,1)',
    '将登录帐号加入禁止登录列表，此帐号登录的用户将无法进入游戏');
  AddCmdToList(@g_GameCommand.DELDENYACCOUNTLOGON, True, '登录帐号', '从禁止登录帐号列表中删除帐号');
  AddCmdToList(@g_GameCommand.SHOWDENYACCOUNTLOGON, True, '', '显示禁止登录帐号列表');
  AddCmdToList(@g_GameCommand.DENYCHARNAMELOGON, True, '人物名 是否永久封(0,1)', '将人物名加入禁止登录列表，此人物将无法进入游戏');
  AddCmdToList(@g_GameCommand.DELDENYCHARNAMELOGON, True, '人物名', '从禁止登录人物列表中删除人物');
  AddCmdToList(@g_GameCommand.SHOWDENYCHARNAMELOGON, True, '', '显示禁止登录人物名列表');
  AddCmdToList(@g_GameCommand.DENYIPLOGON, True, 'IP地址 是否永久封(0,1)', '将IP加入禁止登录列表，以这些IP登录的用户将无法进入游戏');
  AddCmdToList(@g_GameCommand.DELDENYIPLOGON, True, 'IP地址', '从禁止登录IP列表中删除IP');
  AddCmdToList(@g_GameCommand.SHOWDENYIPLOGON, True, '', '显示禁止登录IP列表');
  AddCmdToList(@g_GameCommand.DENYIPLOCALLOGON, True, 'IP所在地 是否永久封(0,1)',
    '将IP所在地加入禁止登录列表，以这些IP所在地登录的用户将无法进入游戏');
  AddCmdToList(@g_GameCommand.DELDENYIPLOCALLOGON, True, 'IP所在地', '从禁止登录IP所在地列表中删除IP所在地');
  AddCmdToList(@g_GameCommand.SHOWDENYIPLOCALLOGON, True, '', '显示禁止登录IP所在地列表');
  AddCmdToList(@g_GameCommand.DENYMACHINEIDLOGON, True, '机器码 是否永久封(0,1)',
    '将指定机器码加入禁止登录列表，以这些机器码登录的用户将无法进入游戏');
  AddCmdToList(@g_GameCommand.DELMACHINEIDLOGON, True, '机器码', '从禁止登录机器机列表中删除指定机器码');
  AddCmdToList(@g_GameCommand.SHOWMACHINEIDLOGON, True, '', '显示禁止登录机器码列表');
  AddGroupToList('提问');
  AddCmdToList(@g_GameCommand.STARTQUEST, True, '', '开始提问功能，游戏中所有人同时跳出问题窗口');
end;

procedure TfrmGameCmd.AddDebugCommandToList;
var
  CmdCount: Integer;

  procedure AddCmdToList(GameCmd: pTGameCmd; sCmdParam, sDesc: string);
  var
    Node: PVirtualNode;
    NodeData: PNodeData;
  begin
    Inc(CmdCount);
    Node := vstDebugCmd.AddChild(nil);
    NodeData := vstDebugCmd.GetNodeData(Node);
    NodeData.Index := CmdCount;
    NodeData.IsPermission := False;
    NodeData.CommandParam := sCmdParam;
    NodeData.CommandDesc := sDesc;
    NodeData.GameCmd := GameCmd;
  end;

  procedure AddGroupToList(sGroupText: string);
  var
    Node: PVirtualNode;
    NodeData: PNodeData;
  begin
    Node := vstDebugCmd.AddChild(nil);
    NodeData := vstDebugCmd.GetNodeData(Node);
    FillChar(NodeData^, SizeOf(TNodeData), 0);
    NodeData.Index := 0;
    NodeData.CommandDesc := sGroupText;
  end;

begin
  btnDebugCmdOK.Enabled := False;
  CmdCount := 0;
  AddGroupToList('重新加载配置');
  AddCmdToList(@g_GameCommand.RELOADADMIN, '', '重新加载管理员列表');
  AddCmdToList(@g_GameCommand.ReLoadNpc, '', '重新加载NPC脚本');
  AddCmdToList(@g_GameCommand.RELOADMANAGE, '', '重新加载登录脚本');
  AddCmdToList(@g_GameCommand.RELOADROBOTMANAGE, '', '重新加载机器人配置');
  AddCmdToList(@g_GameCommand.RELOADROBOT, '', '重新加载机器人脚本');
  AddCmdToList(@g_GameCommand.RELOADMONITEMS, '', '重新加载怪物爆率配置');
  AddCmdToList(@g_GameCommand.RELOADDIARY, '', '未使用');
  AddCmdToList(@g_GameCommand.RELOADITEMDB, '', '重新加载物品数据库');
  AddCmdToList(@g_GameCommand.RELOADMAGICDB, '', '未使用');
  AddCmdToList(@g_GameCommand.RELOADMONSTERDB, '', '重新加载怪物数据库');
  AddCmdToList(@g_GameCommand.RELOADMINMAP, '', '重新加载小地图配置');
  AddCmdToList(@g_GameCommand.RELOADGUILD, '', '');
  AddCmdToList(@g_GameCommand.RELOADGUILDALL, '', '');
  AddCmdToList(@g_GameCommand.RELOADLINENOTICE, '', '重新加载游戏公告信息');
  AddCmdToList(@g_GameCommand.RELOADABUSE, '', '重新加载脏话过滤配置');
  AddGroupToList('其他');
  AddCmdToList(@g_GameCommand.SHOWFLAG, '', '');
  AddCmdToList(@g_GameCommand.SETFLAG, '', '');
  AddCmdToList(@g_GameCommand.SHOWOPEN, '', '');
  AddCmdToList(@g_GameCommand.SETOPEN, '', '');
  AddCmdToList(@g_GameCommand.SHOWUNIT, '', '');
  AddCmdToList(@g_GameCommand.SETUNIT, '', '');
  AddCmdToList(@g_GameCommand.MOBNPC, '', '');
  AddCmdToList(@g_GameCommand.DELNPC, '', '');
  AddCmdToList(@g_GameCommand.LOTTERYTICKET, '', '');
  AddCmdToList(@g_GameCommand.BACKSTEP, '', '');
  AddCmdToList(@g_GameCommand.RECONNECTION, '', '将指定人物重新切换网络连接');
  AddCmdToList(@g_GameCommand.CHANGEGATE, '', '切换网络连接');
  AddCmdToList(@g_GameCommand.DISABLEFILTER, '', '禁用脏话过滤功能');
  AddCmdToList(@g_GameCommand.CHGUSERFULL, '', '');
  AddCmdToList(@g_GameCommand.CHGZENFASTSTEP, '', '');
  AddCmdToList(@g_GameCommand.OXQUIZROOM, '', '');
  AddCmdToList(@g_GameCommand.BALL, '', '');
  AddCmdToList(@g_GameCommand.FIREBURN, '', '');
  AddCmdToList(@g_GameCommand.TESTFIRE, '', '');
  AddCmdToList(@g_GameCommand.TESTSTATUS, '', '');
  AddCmdToList(@g_GameCommand.TESTGOLDCHANGE, '', '');
  AddCmdToList(@g_GameCommand.GSA, '', '');
  AddCmdToList(@g_GameCommand.TESTGA, '', '');
  AddCmdToList(@g_GameCommand.MAPINFO, '', '显示地图信息');
  AddCmdToList(@g_GameCommand.CLEARBAG, '', '清除背包全部物品');
end;

procedure TfrmGameCmd.edtAdminCmdNameChange(Sender: TObject);
begin
  btnAdminCmdOK.Enabled := True;
  btnAdminCmdSave.Enabled := True;
end;

procedure TfrmGameCmd.seAdminCmdPermissionChange(Sender: TObject);
begin
  btnAdminCmdOK.Enabled := True;
  btnAdminCmdSave.Enabled := True;
end;

procedure TfrmGameCmd.btnAdminCmdOKClick(Sender: TObject);
var
  sCommand: string;
  NodeData: PNodeData;
begin
  if vstAdminCmd.FocusedNode = nil then
    Exit;
  NodeData := vstAdminCmd.GetNodeData(vstAdminCmd.FocusedNode);
  if NodeData.Index = 0 then
    Exit;
  sCommand := Trim(edtAdminCmdName.Text);
  if sCommand = '' then
  begin
    Application.MessageBox('命令名称不能为空！', '提示信息', MB_OK + MB_ICONERROR);
    edtAdminCmdName.SetFocus;
    Exit;
  end;
  NodeData.GameCmd.sCmd := sCommand;
  NodeData.GameCmd.nPermissionMin := seAdminCmdPermission.Value;
  vstAdminCmd.InvalidateNode(vstAdminCmd.FocusedNode);
end;

procedure TfrmGameCmd.btnAdminCmdSaveClick(Sender: TObject);
begin
  btnAdminCmdSave.Enabled := False;
  CommandConf.WriteString('Command', 'ObServer', g_GameCommand.OBSERVER.sCmd);
  CommandConf.WriteString('Command', 'GameMaster', g_GameCommand.GAMEMASTER.sCmd);
  CommandConf.WriteString('Command', 'SuperMan', g_GameCommand.SUEPRMAN.sCmd);
  CommandConf.WriteString('Command', 'StorageClearPassword', g_GameCommand.CLRPASSWORD.sCmd);
  CommandConf.WriteString('Command', 'Who', g_GameCommand.WHO.sCmd);
  CommandConf.WriteString('Command', 'Total', g_GameCommand.TOTAL.sCmd);
  CommandConf.WriteString('Command', 'Make', g_GameCommand.MAKE.sCmd);
  CommandConf.WriteString('Command', 'PositionMove', g_GameCommand.POSITIONMOVE.sCmd);
  CommandConf.WriteString('Command', 'Move', g_GameCommand.Move.sCmd);
  CommandConf.WriteString('Command', 'Recall', g_GameCommand.RECALL.sCmd);
  CommandConf.WriteString('Command', 'ReGoto', g_GameCommand.REGOTO.sCmd);
  CommandConf.WriteString('Command', 'Ting', g_GameCommand.TING.sCmd);
  CommandConf.WriteString('Command', 'SuperTing', g_GameCommand.SUPERTING.sCmd);
  CommandConf.WriteString('Command', 'MapMove', g_GameCommand.MAPMOVE.sCmd);
  CommandConf.WriteString('Command', 'Info', g_GameCommand.INFO.sCmd);
  CommandConf.WriteString('Command', 'HumanLocal', g_GameCommand.HUMANLOCAL.sCmd);
  CommandConf.WriteString('Command', 'ViewWhisper', g_GameCommand.VIEWWHISPER.sCmd);
  CommandConf.WriteString('Command', 'MobLevel', g_GameCommand.MOBLEVEL.sCmd);
  CommandConf.WriteString('Command', 'MobCount', g_GameCommand.MOBCOUNT.sCmd);
  CommandConf.WriteString('Command', 'HumanCount', g_GameCommand.HUMANCOUNT.sCmd);
  CommandConf.WriteString('Command', 'Map', g_GameCommand.Map.sCmd);
  CommandConf.WriteString('Command', 'Level', g_GameCommand.Level.sCmd);
  CommandConf.WriteString('Command', 'Kick', g_GameCommand.KICK.sCmd);
  CommandConf.WriteString('Command', 'ReAlive', g_GameCommand.ReAlive.sCmd);
  CommandConf.WriteString('Command', 'Kill', g_GameCommand.KILL.sCmd);
  CommandConf.WriteString('Command', 'ChangeJob', g_GameCommand.CHANGEJOB.sCmd);
  CommandConf.WriteString('Command', 'FreePenalty', g_GameCommand.FREEPENALTY.sCmd);
  CommandConf.WriteString('Command', 'PkPoint', g_GameCommand.PKPOINT.sCmd);
  CommandConf.WriteString('Command', 'IncPkPoint', g_GameCommand.IncPkPoint.sCmd);
  CommandConf.WriteString('Command', 'ChangeGender', g_GameCommand.CHANGEGENDER.sCmd);
  CommandConf.WriteString('Command', 'Hair', g_GameCommand.HAIR.sCmd);
  CommandConf.WriteString('Command', 'BonusPoint', g_GameCommand.BonusPoint.sCmd);
  CommandConf.WriteString('Command', 'DelBonuPoint', g_GameCommand.DELBONUSPOINT.sCmd);
  CommandConf.WriteString('Command', 'RestBonuPoint', g_GameCommand.RESTBONUSPOINT.sCmd);
  CommandConf.WriteString('Command', 'SetPermission', g_GameCommand.SETPERMISSION.sCmd);
  CommandConf.WriteString('Command', 'ReNewLevel', g_GameCommand.RENEWLEVEL.sCmd);
  CommandConf.WriteString('Command', 'DelGold', g_GameCommand.DELGOLD.sCmd);
  CommandConf.WriteString('Command', 'AddGold', g_GameCommand.ADDGOLD.sCmd);
  CommandConf.WriteString('Command', 'GameGold', g_GameCommand.GAMEGOLD.sCmd);
  CommandConf.WriteString('Command', 'GamePoint', g_GameCommand.GAMEPOINT.sCmd);
  CommandConf.WriteString('Command', 'CreditPoint', g_GameCommand.CREDITPOINT.sCmd);
  CommandConf.WriteString('Command', 'RefineWeapon', g_GameCommand.REFINEWEAPON.sCmd);
  CommandConf.WriteString('Command', 'AdjuestTLevel', g_GameCommand.ADJUESTLEVEL.sCmd);
  CommandConf.WriteString('Command', 'AdjuestExp', g_GameCommand.ADJUESTEXP.sCmd);
  CommandConf.WriteString('Command', 'ChangeDearName', g_GameCommand.CHANGEDEARNAME.sCmd);
  CommandConf.WriteString('Command', 'ChangeMasterName', g_GameCommand.CHANGEMASTERNAME.sCmd);
  CommandConf.WriteString('Command', 'RecallMob', g_GameCommand.RECALLMOB.sCmd);
  CommandConf.WriteString('Command', 'Training', g_GameCommand.TRAINING.sCmd);
  CommandConf.WriteString('Command', 'OpTraining', g_GameCommand.TRAININGSKILL.sCmd);
  CommandConf.WriteString('Command', 'DeleteSkill', g_GameCommand.DELETESKILL.sCmd);
  CommandConf.WriteString('Command', 'DeleteItem', g_GameCommand.DELETEITEM.sCmd);
  CommandConf.WriteString('Command', 'ClearMission', g_GameCommand.CLEARMISSION.sCmd);
  CommandConf.WriteString('Command', 'AddGuild', g_GameCommand.AddGuild.sCmd);
  CommandConf.WriteString('Command', 'DelGuild', g_GameCommand.DELGUILD.sCmd);
  CommandConf.WriteString('Command', 'ChangeSabukLord', g_GameCommand.CHANGESABUKLORD.sCmd);
  CommandConf.WriteString('Command', 'ForcedWallConQuestWar', g_GameCommand.FORCEDWALLCONQUESTWAR.sCmd);
  CommandConf.WriteString('Command', 'ContestPoint', g_GameCommand.CONTESTPOINT.sCmd);
  CommandConf.WriteString('Command', 'StartContest', g_GameCommand.STARTCONTEST.sCmd);
  CommandConf.WriteString('Command', 'EndContest', g_GameCommand.ENDCONTEST.sCmd);
  CommandConf.WriteString('Command', 'Announcement', g_GameCommand.ANNOUNCEMENT.sCmd);
  CommandConf.WriteString('Command', 'MobLevel', g_GameCommand.MOBLEVEL.sCmd);
  CommandConf.WriteString('Command', 'Mission', g_GameCommand.Mission.sCmd);
  CommandConf.WriteString('Command', 'LoyaltyPoint', g_GameCommand.LoyaltyPoint.sCmd);
  CommandConf.WriteString('Command', 'MachineID', g_GameCommand.MACHINEID.sCmd);
  CommandConf.WriteString('Command', 'MOB', g_GameCommand.MOB.sCmd);
  // -------------------------------------------- chongchong 2013-12-25
  CommandConf.WriteString('Command', 'MobPlace', g_GameCommand.MobPlace.sCmd);
  CommandConf.WriteString('Command', 'ClearMon', g_GameCommand.CLEARMON.sCmd);
  CommandConf.WriteString('Command', 'GiveMine', g_GameCommand.GiveMine.sCmd);
  CommandConf.WriteString('Command', 'Supermake', g_GameCommand.SMAKE.sCmd);
  CommandConf.WriteString('Command', 'DisableSendMsg', g_GameCommand.DISABLESENDMSG.sCmd);
  CommandConf.WriteString('Command', 'EnableSendMsg', g_GameCommand.ENABLESENDMSG.sCmd);
  CommandConf.WriteString('Command', 'DisableSendMsgList', g_GameCommand.DISABLESENDMSGLIST.sCmd);
  CommandConf.WriteString('Command', 'Shutup', g_GameCommand.SHUTUP.sCmd);
  CommandConf.WriteString('Command', 'ReleaseShutup', g_GameCommand.RELEASESHUTUP.sCmd);
  CommandConf.WriteString('Command', 'ShutupList', g_GameCommand.SHUTUPLIST.sCmd);
  CommandConf.WriteString('Command', 'SabukWallGold', g_GameCommand.SABUKWALLGOLD.sCmd);
  CommandConf.WriteString('Command', 'StartQuest', g_GameCommand.STARTQUEST.sCmd);
  CommandConf.WriteString('Command', 'DenyIPaddrLogon', g_GameCommand.DENYIPLOGON.sCmd);
  CommandConf.WriteString('Command', 'DenyIPLocalLogon', g_GameCommand.DENYIPLOCALLOGON.sCmd);
  CommandConf.WriteString('Command', 'DenyAccountLogon', g_GameCommand.DENYACCOUNTLOGON.sCmd);
  CommandConf.WriteString('Command', 'DenyCharNameLogon', g_GameCommand.DENYCHARNAMELOGON.sCmd);
  CommandConf.WriteString('Command', 'DenyMachineIDLogon', g_GameCommand.DENYMACHINEIDLOGON.sCmd);
  CommandConf.WriteString('Command', 'DelDenyIPLogon', g_GameCommand.DELDENYIPLOGON.sCmd);
  CommandConf.WriteString('Command', 'DelDenyIPLocalLogon', g_GameCommand.DELDENYIPLOCALLOGON.sCmd);
  CommandConf.WriteString('Command', 'DelDenyAccountLogon', g_GameCommand.DELDENYACCOUNTLOGON.sCmd);
  CommandConf.WriteString('Command', 'DelDenyCharNameLogon', g_GameCommand.DELDENYCHARNAMELOGON.sCmd);
  CommandConf.WriteString('Command', 'DelMachineIDLogon', g_GameCommand.DELMACHINEIDLOGON.sCmd);
  CommandConf.WriteString('Command', 'ShowDenyIPLogon', g_GameCommand.SHOWDENYIPLOGON.sCmd);
  CommandConf.WriteString('Command', 'ShowDenyIPLocalLogon', g_GameCommand.SHOWDENYIPLOCALLOGON.sCmd);
  CommandConf.WriteString('Command', 'ShowDenyAccountLogon', g_GameCommand.SHOWDENYACCOUNTLOGON.sCmd);
  CommandConf.WriteString('Command', 'ShowDenyCharNameLogon', g_GameCommand.SHOWDENYCHARNAMELOGON.sCmd);
  CommandConf.WriteString('Command', 'ShowMachineIDLogon', g_GameCommand.SHOWMACHINEIDLOGON.sCmd);
  CommandConf.WriteString('Command', 'SetMapMode', g_GameCommand.SetMapMode.sCmd); //
  CommandConf.WriteString('Command', 'ShowMapMode', g_GameCommand.SHOWMAPMODE.sCmd); //
  CommandConf.WriteString('Command', 'Attack', g_GameCommand.Attack.sCmd); //
  CommandConf.WriteString('Command', 'LuckPoint', g_GameCommand.LUCKYPOINT.sCmd);
  CommandConf.WriteString('Command', 'ChangeGameDiamond', g_GameCommand.CHANGEGAMEDIAMOND.sCmd);
  CommandConf.WriteString('Command', 'ChangeGameGird', g_GameCommand.CHANGEGAMEGIRD.sCmd);
  CommandConf.WriteString('Command', 'ChangeGameGlory', g_GameCommand.CHANGEGAMEGLORY.sCmd);
  CommandConf.WriteString('Command', 'Hunger', g_GameCommand.HUNGER.sCmd); //
  CommandConf.WriteString('Command', 'SpiritStart', g_GameCommand.SPIRIT.sCmd); //
  CommandConf.WriteString('Command', 'SpiritStop', g_GameCommand.SPIRITSTOP.sCmd); //
  CommandConf.WriteString('Command', 'NGLevel', g_GameCommand.NGLevel.sCmd);
  CommandConf.WriteString('Command', 'DelUserShop', g_GameCommand.DelUserShop.sCmd);
  CommandConf.WriteString('Command', 'SetUserShopName', g_GameCommand.SetUserShopName.sCmd);
  CommandConf.WriteString('Command', 'DelSellPlayer', g_GameCommand.DelSellPlayer.sCmd);
  // -------------------------------------------------------------------------------------------------------------------------
  CommandConf.WriteInteger('Permission', 'GameMaster', g_GameCommand.GAMEMASTER.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'ObServer', g_GameCommand.OBSERVER.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'SuperMan', g_GameCommand.SUEPRMAN.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'StorageClearPassword', g_GameCommand.CLRPASSWORD.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'Who', g_GameCommand.WHO.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'Total', g_GameCommand.TOTAL.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'MakeMin', g_GameCommand.MAKE.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'MakeMax', g_GameCommand.MAKE.nPermissionMax);
  CommandConf.WriteInteger('Permission', 'PositionMoveMin', g_GameCommand.POSITIONMOVE.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'PositionMoveMax', g_GameCommand.POSITIONMOVE.nPermissionMax);
  CommandConf.WriteInteger('Permission', 'MoveMin', g_GameCommand.Move.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'MoveMax', g_GameCommand.Move.nPermissionMax);
  CommandConf.WriteInteger('Permission', 'Recall', g_GameCommand.RECALL.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'ReGoto', g_GameCommand.REGOTO.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'Ting', g_GameCommand.TING.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'SuperTing', g_GameCommand.SUPERTING.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'MapMove', g_GameCommand.MAPMOVE.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'Info', g_GameCommand.INFO.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'HumanLocal', g_GameCommand.HUMANLOCAL.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'ViewWhisper', g_GameCommand.VIEWWHISPER.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'MobLevel', g_GameCommand.MOBLEVEL.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'MobCount', g_GameCommand.MOBCOUNT.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'HumanCount', g_GameCommand.HUMANCOUNT.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'Map', g_GameCommand.Map.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'Level', g_GameCommand.Level.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'Kick', g_GameCommand.KICK.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'ReAlive', g_GameCommand.ReAlive.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'Kill', g_GameCommand.KILL.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'ChangeJob', g_GameCommand.CHANGEJOB.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'FreePenalty', g_GameCommand.FREEPENALTY.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'PkPoint', g_GameCommand.PKPOINT.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'IncPkPoint', g_GameCommand.IncPkPoint.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'ChangeGender', g_GameCommand.CHANGEGENDER.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'Hair', g_GameCommand.HAIR.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'BonusPoint', g_GameCommand.BonusPoint.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'DelBonuPoint', g_GameCommand.DELBONUSPOINT.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'RestBonuPoint', g_GameCommand.RESTBONUSPOINT.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'SetPermission', g_GameCommand.SETPERMISSION.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'ReNewLevel', g_GameCommand.RENEWLEVEL.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'DelGold', g_GameCommand.DELGOLD.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'AddGold', g_GameCommand.ADDGOLD.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'GameGold', g_GameCommand.GAMEGOLD.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'GamePoint', g_GameCommand.GAMEPOINT.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'CreditPoint', g_GameCommand.CREDITPOINT.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'RefineWeapon', g_GameCommand.REFINEWEAPON.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'AdjuestTLevel', g_GameCommand.ADJUESTLEVEL.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'AdjuestExp', g_GameCommand.ADJUESTEXP.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'ChangeDearName', g_GameCommand.CHANGEDEARNAME.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'ChangeMasterName', g_GameCommand.CHANGEMASTERNAME.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'RecallMob', g_GameCommand.RECALLMOB.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'Training', g_GameCommand.TRAINING.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'OpTraining', g_GameCommand.TRAININGSKILL.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'DeleteSkill', g_GameCommand.DELETESKILL.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'DeleteItem', g_GameCommand.DELETEITEM.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'ClearMission', g_GameCommand.CLEARMISSION.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'AddGuild', g_GameCommand.AddGuild.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'DelGuild', g_GameCommand.DELGUILD.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'ChangeSabukLord', g_GameCommand.CHANGESABUKLORD.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'ForcedWallConQuestWar', g_GameCommand.FORCEDWALLCONQUESTWAR.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'ContestPoint', g_GameCommand.CONTESTPOINT.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'StartContest', g_GameCommand.STARTCONTEST.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'EndContest', g_GameCommand.ENDCONTEST.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'Announcement', g_GameCommand.ANNOUNCEMENT.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'MobLevel', g_GameCommand.MOBLEVEL.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'Mission', g_GameCommand.Mission.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'MobPlace', g_GameCommand.MobPlace.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'ClearMon', g_GameCommand.CLEARMON.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'GiveMine', g_GameCommand.GiveMine.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'Supermake', g_GameCommand.SMAKE.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'LoyaltyPoint', g_GameCommand.LoyaltyPoint.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'MachineID', g_GameCommand.MACHINEID.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'MOB', g_GameCommand.MOB.nPermissionMin);
  // 2013-12-25
  CommandConf.WriteInteger('Permission', 'DisableSendMsg', g_GameCommand.DISABLESENDMSG.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'EnableSendMsg', g_GameCommand.ENABLESENDMSG.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'DisableSendMsgList', g_GameCommand.DISABLESENDMSGLIST.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'Shutup', g_GameCommand.SHUTUP.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'ReleaseShutup', g_GameCommand.RELEASESHUTUP.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'ShutupList', g_GameCommand.SHUTUPLIST.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'SabukWallGold', g_GameCommand.SABUKWALLGOLD.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'StartQuest', g_GameCommand.STARTQUEST.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'DenyIPaddrLogon', g_GameCommand.DENYIPLOGON.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'DenyIPLocalLogon', g_GameCommand.DENYIPLOCALLOGON.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'DenyAccountLogon', g_GameCommand.DENYACCOUNTLOGON.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'DenyCharNameLogon', g_GameCommand.DENYCHARNAMELOGON.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'DenyMachineIDLogon', g_GameCommand.DENYMACHINEIDLOGON.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'DelDenyIPLogon', g_GameCommand.DELDENYIPLOGON.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'DelDenyIPLocalLogon', g_GameCommand.DELDENYIPLOCALLOGON.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'DelDenyAccountLogon', g_GameCommand.DELDENYACCOUNTLOGON.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'DelDenyCharNameLogon', g_GameCommand.DELDENYCHARNAMELOGON.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'DelMachineIDLogon', g_GameCommand.DELMACHINEIDLOGON.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'ShowDenyIPLogon', g_GameCommand.SHOWDENYIPLOGON.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'ShowDenyIPLocalLogon', g_GameCommand.SHOWDENYIPLOCALLOGON.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'ShowDenyAccountLogon', g_GameCommand.SHOWDENYACCOUNTLOGON.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'ShowDenyCharNameLogon', g_GameCommand.SHOWDENYCHARNAMELOGON.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'ShowMachineIDLogon', g_GameCommand.SHOWMACHINEIDLOGON.nPermissionMin);
  // CommandConf.WriteInteger('Permission', 'SetMapMode', g_GameCommand.SetMapMode.nPermissionMin);
  // CommandConf.WriteInteger('Permission', 'ShowMapMode', g_GameCommand.ShowMapMode.nPermissionMin);
  // CommandConf.WriteInteger('Permission', 'Attack', g_GameCommand.Attack.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'LuckPoint', g_GameCommand.LUCKYPOINT.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'ChangeGameDiamond', g_GameCommand.CHANGEGAMEDIAMOND.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'ChangeGameGird', g_GameCommand.CHANGEGAMEGIRD.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'ChangeGameGlory', g_GameCommand.CHANGEGAMEGLORY.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'NGLevel', g_GameCommand.NGLevel.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'DelUserShop', g_GameCommand.DelUserShop.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'SetUserShopName', g_GameCommand.SetUserShopName.nPermissionMin);
  CommandConf.WriteInteger('Permission', 'DelSellPlayer', g_GameCommand.DelSellPlayer.nPermissionMin);
  // CommandConf.WriteInteger('Permission', 'Hunger', g_GameCommand.Hunger.nPermissionMin);
  // CommandConf.WriteInteger('Permission', 'ChangeItemName', g_GameCommand.ChangeItemName.nPermissionMin);
  // CommandConf.WriteInteger('Permission', 'SpiritStart', g_GameCommand.Spirit.nPermissionMin);
  // CommandConf.WriteInteger('Permission', 'SpiritStop', g_GameCommand.SpiritStop.nPermissionMin);
end;

procedure TfrmGameCmd.edtDebugCmdNameChange(Sender: TObject);
begin
  btnDebugCmdOK.Enabled := True;
  btnDebugCmdSave.Enabled := True;
end;

procedure TfrmGameCmd.seDebugCmdPermissionChange(Sender: TObject);
begin
  btnDebugCmdOK.Enabled := True;
  btnDebugCmdSave.Enabled := True;
end;

procedure TfrmGameCmd.btnDebugCmdSaveClick(Sender: TObject);
begin
  btnDebugCmdSave.Enabled := False;
  CommandConf.WriteString('Command', 'SHOWFLAG', g_GameCommand.SHOWFLAG.sCmd);
  CommandConf.WriteString('Command', 'SETFLAG', g_GameCommand.SETFLAG.sCmd);
  CommandConf.WriteString('Command', 'SHOWOPEN', g_GameCommand.SHOWOPEN.sCmd);
  CommandConf.WriteString('Command', 'SETOPEN', g_GameCommand.SETOPEN.sCmd);
  CommandConf.WriteString('Command', 'SHOWUNIT', g_GameCommand.SHOWUNIT.sCmd);
  CommandConf.WriteString('Command', 'SETUNIT', g_GameCommand.SETUNIT.sCmd);
  CommandConf.WriteString('Command', 'MOBNPC', g_GameCommand.MOBNPC.sCmd);
  CommandConf.WriteString('Command', 'DELNPC', g_GameCommand.DELNPC.sCmd);
  CommandConf.WriteString('Command', 'LOTTERYTICKET', g_GameCommand.LOTTERYTICKET.sCmd);
  CommandConf.WriteString('Command', 'RELOADADMIN', g_GameCommand.RELOADADMIN.sCmd);
  CommandConf.WriteString('Command', 'ReLoadNpc', g_GameCommand.ReLoadNpc.sCmd);
  CommandConf.WriteString('Command', 'RELOADMANAGE', g_GameCommand.RELOADMANAGE.sCmd);
  CommandConf.WriteString('Command', 'RELOADROBOTMANAGE', g_GameCommand.RELOADROBOTMANAGE.sCmd);
  CommandConf.WriteString('Command', 'RELOADROBOT', g_GameCommand.RELOADROBOT.sCmd);
  CommandConf.WriteString('Command', 'RELOADMONITEMS', g_GameCommand.RELOADMONITEMS.sCmd);
  CommandConf.WriteString('Command', 'RELOADDIARY', g_GameCommand.RELOADDIARY.sCmd);
  CommandConf.WriteString('Command', 'RELOADITEMDB', g_GameCommand.RELOADITEMDB.sCmd);
  CommandConf.WriteString('Command', 'RELOADMAGICDB', g_GameCommand.RELOADMAGICDB.sCmd);
  CommandConf.WriteString('Command', 'RELOADMONSTERDB', g_GameCommand.RELOADMONSTERDB.sCmd);
  CommandConf.WriteString('Command', 'RELOADMINMAP', g_GameCommand.RELOADMINMAP.sCmd);
  CommandConf.WriteString('Command', 'RELOADGUILD', g_GameCommand.RELOADGUILD.sCmd);
  CommandConf.WriteString('Command', 'RELOADGUILDALL', g_GameCommand.RELOADGUILDALL.sCmd);
  CommandConf.WriteString('Command', 'RELOADLINENOTICE', g_GameCommand.RELOADLINENOTICE.sCmd);
  CommandConf.WriteString('Command', 'RELOADABUSE', g_GameCommand.RELOADABUSE.sCmd);
  CommandConf.WriteString('Command', 'BACKSTEP', g_GameCommand.BACKSTEP.sCmd);
  CommandConf.WriteString('Command', 'RECONNECTION', g_GameCommand.RECONNECTION.sCmd);
  CommandConf.WriteString('Command', 'CHANGEGATE', g_GameCommand.CHANGEGATE.sCmd);
  CommandConf.WriteString('Command', 'DISABLEFILTER', g_GameCommand.DISABLEFILTER.sCmd);
  CommandConf.WriteString('Command', 'CHGUSERFULL', g_GameCommand.CHGUSERFULL.sCmd);
  CommandConf.WriteString('Command', 'CHGZENFASTSTEP', g_GameCommand.CHGZENFASTSTEP.sCmd);
  CommandConf.WriteString('Command', 'OXQUIZROOM', g_GameCommand.OXQUIZROOM.sCmd);
  CommandConf.WriteString('Command', 'BALL', g_GameCommand.BALL.sCmd);
  CommandConf.WriteString('Command', 'FIREBURN', g_GameCommand.FIREBURN.sCmd);
  CommandConf.WriteString('Command', 'TESTFIRE', g_GameCommand.TESTFIRE.sCmd);
  CommandConf.WriteString('Command', 'TESTSTATUS', g_GameCommand.TESTSTATUS.sCmd);
  CommandConf.WriteString('Command', 'TESTGOLDCHANGE', g_GameCommand.TESTGOLDCHANGE.sCmd);
  CommandConf.WriteString('Command', 'GSA', g_GameCommand.GSA.sCmd);
  CommandConf.WriteString('Command', 'TESTGA', g_GameCommand.TESTGA.sCmd);
  CommandConf.WriteString('Command', 'MAPINFO', g_GameCommand.MAPINFO.sCmd);
  CommandConf.WriteString('Command', 'CLEARBAG', g_GameCommand.CLEARBAG.sCmd);
end;

procedure TfrmGameCmd.vstUserCmdGetText(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex; TextType: TVSTTextType;
  var CellText: string);
var
  NodeData: PNodeData;
begin
  NodeData := Sender.GetNodeData(Node);
  if NodeData.Index > 0 then
  begin
    case Column of
      0:
        CellText := IntToStr(NodeData.Index);
      1:
        CellText := NodeData.GameCmd.sCmd;
      2:
        begin
          if NodeData.IsPermission then
            CellText := '√'
          else
            CellText := '';
        end;
      3:
        CellText := Format('%d/%d', [NodeData.GameCmd.nPermissionMin, NodeData.GameCmd.nPermissionMax]);
      4:
        CellText := Format('@%s %s', [NodeData.GameCmd.sCmd, NodeData.CommandParam]);
      5:
        CellText := NodeData.CommandDesc;
    end;
  end
  else
  begin
    CellText := '';
  end;
end;

procedure TfrmGameCmd.vstUserCmdFreeNode(Sender: TBaseVirtualTree; Node: PVirtualNode);
var
  NodeData: PNodeData;
begin
  NodeData := Sender.GetNodeData(Node);
  NodeData.CommandParam := '';
  NodeData.CommandDesc := '';
end;

procedure TfrmGameCmd.vstUserCmdAfterItemPaint(Sender: TBaseVirtualTree; TargetCanvas: TCanvas; Node: PVirtualNode; ItemRect:
  TRect);
var
  NodeData: PNodeData;
begin
  NodeData := Sender.GetNodeData(Node);
  if NodeData.Index = 0 then
  begin
    TargetCanvas.Brush.Color := $00F2E4D8;
    TargetCanvas.FillRect(ItemRect);
    ItemRect.Left := ItemRect.Left + 10;
    TargetCanvas.Font.Color := clBlue;
    TargetCanvas.Font.Style := [fsBold];
    Windows.DrawText(TargetCanvas.Handle, PChar(NodeData.CommandDesc), Length(NodeData.CommandDesc), ItemRect, DT_NOPREFIX or
      DT_VCENTER or DT_SINGLELINE or DT_LEFT);
  end;
end;

procedure TfrmGameCmd.vstUserCmdBeforeItemErase(Sender: TBaseVirtualTree; TargetCanvas: TCanvas; Node: PVirtualNode; ItemRect:
  TRect; var ItemColor: TColor; var EraseAction: TItemEraseAction);
begin
  if Node.Index mod 2 <> 0 then
  begin
    ItemColor := $00FBFBFB;
    EraseAction := eaColor;
  end;
end;

procedure TfrmGameCmd.vstUserCmdFocusChanged(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex);
var
  NodeData: PNodeData;
begin
  if Sender.FocusedNode = nil then
    Exit;
  NodeData := Sender.GetNodeData(Node);
  if NodeData.GameCmd <> nil then
  begin
    edtUserCmdName.Text := NodeData.GameCmd.sCmd;
    seUserCmdPermission.Value := NodeData.GameCmd.nPermissionMin;
    lblUserCmdDesc.Caption := NodeData.CommandDesc;
    lblUserCmd.Caption := Format('%s %s', [NodeData.GameCmd.sCmd, NodeData.CommandParam]);
  end
  else
  begin
    edtUserCmdName.Clear;
    seUserCmdPermission.Value := 0;
    lblUserCmdDesc.Caption := '';
    lblUserCmd.Caption := '';
  end;

  btnUserCmdOK.Enabled := False;
end;

procedure TfrmGameCmd.vstAdminCmdFocusChanged(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex);
var
  NodeData: PNodeData;
begin
  if Sender.FocusedNode = nil then
    Exit;

  NodeData := Sender.GetNodeData(Node);
  if NodeData.GameCmd <> nil then
  begin
    edtAdminCmdName.Text := NodeData.GameCmd.sCmd;
    seAdminCmdPermission.Value := NodeData.GameCmd.nPermissionMin;
    lblAdminCmdDesc.Caption := NodeData.CommandDesc;
    lblAdminCmd.Caption := Format('%s %s', [NodeData.GameCmd.sCmd, NodeData.CommandParam]);
  end
  else
  begin
    edtAdminCmdName.Clear;
    seAdminCmdPermission.Value := 0;
    lblAdminCmdDesc.Caption := '';
    lblAdminCmd.Caption := '';
  end;

  btnAdminCmdOK.Enabled := False;
end;

procedure TfrmGameCmd.vstDebugCmdFocusChanged(Sender: TBaseVirtualTree; Node: PVirtualNode; Column: TColumnIndex);
var
  NodeData: PNodeData;
begin
  if Sender.FocusedNode = nil then
    Exit;

  NodeData := Sender.GetNodeData(Node);
  if NodeData.GameCmd <> nil then
  begin
    edtDebugCmdName.Text := NodeData.GameCmd.sCmd;
    seDebugCmdPermission.Value := NodeData.GameCmd.nPermissionMin;
    lblDebugCmdDesc.Caption := NodeData.CommandDesc;
    lblDebugCmd.Caption := Format('%s %s', [NodeData.GameCmd.sCmd, NodeData.CommandParam]);
  end
  else
  begin
    edtDebugCmdName.Clear;
    seDebugCmdPermission.Value := 0;
    lblDebugCmdDesc.Caption := '';
    lblDebugCmd.Caption := '';
  end;

  btnDebugCmdOK.Enabled := False;
end;

procedure TfrmGameCmd.btnDebugCmdOKClick(Sender: TObject);
var
  sCommand: string;
  NodeData: PNodeData;
begin
  if vstDebugCmd.FocusedNode = nil then
    Exit;

  NodeData := vstDebugCmd.GetNodeData(vstDebugCmd.FocusedNode);
  if NodeData.Index = 0 then
    Exit;

  sCommand := Trim(edtDebugCmdName.Text);
  if sCommand = '' then
  begin
    Application.MessageBox('命令名称不能为空！', '提示信息', MB_OK + MB_ICONERROR);
    edtDebugCmdName.SetFocus;
    Exit;
  end;

  NodeData.GameCmd.sCmd := sCommand;
  NodeData.GameCmd.nPermissionMin := seDebugCmdPermission.Value;

  vstDebugCmd.InvalidateNode(vstDebugCmd.FocusedNode);
end;

end.

