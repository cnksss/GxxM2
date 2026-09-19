unit Common;

interface

const
  CHECKCRACK = 0;
  LockProcessMsg = 1;                                                                               // 如果用多线程收包，这里要改成1 chongchong 2016-08-15

  // 服务器模块之间
  SG_CHECKCODEADDR = 1006;
  GS_QUIT = 2000;                                                                                   // 关闭
  GS_USERACCOUNT = 2001;
  GS_CHANGEACCOUNTINFO = 2002;
  GS_SETPARENTWINDOW = 2003;
  GS_CANCELSETPARENTWINDOW = 2004;
  SG_FORMHANDLE = 1000;                                                                             // 服务器HANLD
  SG_STARTNOW = 1001;                                                                               // 正在启动服务器...
  SG_STARTOK = 1002;                                                                                // 服务器启动完成...
  SG_ACTIVE = 1003;

  SS_LOGINCOST = 103;

  SS_OPENSESSION = 1000;
  SS_CLOSESESSION = 1010;
  SS_SOFTOUTSESSION = 1020;
  SS_SERVERINFO = 1030;
  SS_KEEPALIVE = 1040;
  SS_KICKUSER = 1110;
  SS_SERVERLOAD = 1130;
  SS_ADDIPTOGATE = 1131;

  SS_PASSWORDSUCCESS = 1132;                                                                        // 离线挂机
  SS_GetAccountInfo = 1133;                                                                         // 获取帐户信息
  SS_GetAccountInfoRet = 1134;                                                                            // 返回帐户信息

  SS_ChangeAccountInfo = 1135;
  SS_ChangeAccountInfoRet = 1136;

  SM_CERTIFICATION_SUCCESS = 502;

  SG_USERACCOUNT = 1003;
  SG_USERACCOUNTNOTFOUND = 1004;                                                                    // 没有找到账号
  SG_USERACCOUNTCHANGESTATUS = 1005;                                                                // 账号更新成功

  WM_SENDPROCMSG = 11111;
  CM_GETGAMELIST = 2000;
  SM_SENDGAMELIST = 5000;

  UNKNOWMSG = 1007;
// ----------------------------------------------

  DB_LOADHUMANRCD = 1000;                                                                           // 读取人物数据
  DB_SAVEHUMANRCD = 1001;                                                                           // 保存人物数据
  DB_HUMANCHANGENAME = 1002;                                                                        // 修改人物名称

  DB_LOADHERORCD = 1003;                                                                            // 读取英雄数据
  DB_SAVEHERORCD = 1004;                                                                            // 保存英雄数据
  DB_NEWHERORCD = 1005;                                                                             // 新建英雄
  DB_DELHERORCD = 1006;                                                                             // 删除英雄
  DB_QUERYSTORAGEHEROINFO = 1007;                                                                   // 查询寄存英雄
  DB_ASSESSHERO = 1008;                                                                             // 评定英雄
  DB_HEROCHANGENAME = 1009;                                                                         // 英雄改名

  DB_LOADDUMMY = 1010;                                                                              // 假人登录，只查询假人名称是否被占用
  DB_GETRANKDATA = 1011;                                                                            // 读取排行榜
  DB_QUERYHUMANINFO = 1012;                                                                         // 读取行会成员信息
  DB_HUMANCHANGEGOLD = 1013;                                                                        // 修改人物货币数据

  DB_CHECKCONNECT = 1014;
  DB_SAVEMAGICLIST = 1015;                                                                          // 读取魔法列表
  DB_SAVESTDITEMLIST = 1016;                                                                        // 读取物品列表

  DB_M2CACHERANKDATA = 1017;                                                                        // M2缓存排行数据

  DB_BUY_PLAYER = 1018;                                                                             // 购买角色
  DB_SELL_PLAYER_Delegator = 1019;                                                                             // 购买角色

  //------------------------------------------------------------------------------------------------------------------------------------------
  
  DBR_LOADHUMANRCD = 11000;                                                                           // 读取人物数据
  DBR_SAVEHUMANRCD = 11001;                                                                           // 保存人物数据
  DBR_HUMANCHANGENAME = 11002;                                                                        // 修改人物名称

  DBR_LOADHERORCD = 11003;                                                                            // 读取英雄数据
  DBR_SAVEHERORCD = 11004;                                                                            // 保存英雄数据
  DBR_NEWHERORCD = 11005;                                                                             // 新建英雄
  DBR_DELHERORCD = 11006;                                                                             // 删除英雄
  DBR_QUERYSTORAGEHEROINFO = 11007;                                                                   // 查询寄存英雄
  DBR_ASSESSHERO = 11008;                                                                             // 评定英雄
  DBR_HEROCHANGENAME = 11009;                                                                         // 英雄改名

  DBR_LOADDUMMY = 11010;                                                                              // 假人登录，只查询假人名称是否被占用
  DBR_GETRANKDATA = 11011;                                                                            // 读取排行榜
  DBR_QUERYHUMANINFO = 11012;                                                                         // 读取行会成员信息
  DBR_HUMANCHANGEGOLD = 11013;                                                                        // 修改人物货币数据

  DBR_CHECKCONNECT = 11014;
  DBR_SAVEMAGICLIST = 11015;                                                                          // 读取魔法列表
  DBR_SAVESTDITEMLIST = 11016;                                                                        // 读取物品列表

  DBR_M2CACHERANKDATA = 11017;

  DBR_BUY_PLAYER = 11018;
  DBR_SELL_PLAYER_Delegator = 11019;
  
  DBR_FAIL = 8888;


  GM_OPEN = 1;
  GM_CLOSE = 2;
  GM_CHECKSERVER = 3;                                                                               // Send check signal to Server
  GM_CHECKCLIENT = 4;                                                                               // Send check signal to Client
  GM_DATA = 5;
  GM_SERVERUSERINDEX = 6;
  GM_RECEIVE_OK = 7;
  GM_CLOSECONNECT = 8;

  GM_COMPDATA = 9;
  GM_KICK = 10;

  GM_DELAY_CLOSE = 20;

  GM_DATA_CACHE = 11;                                                                               // 数据缓存 chongchong 2015-05-15

  GM_NO_CERTIFICATION = 12;

  GM_FULL_SERVICE_MSG = 13;

  GM_RANDOM_DATA = 14;                                                                              // ★★★★★★搞乱在线人物的数据 chongchong 2016-08-01

  GM_RUN_GATE_VER = 15;

  GM_RUN_GATE_MAGICS = 16;

// //////////////////////////////////////////////////////////////////////////////
  SP_GM_LOGIN = 102;

  SP_GM_GETUSER = 103;
  SP_SM_GETUSER_SUCCESS = 104;
  SP_SM_GETUSER_FAIL = 105;

  SP_GM_ADDUSER = 106;
  SP_SM_ADDUSER_SUCCESS = 107;
  SP_SM_ADDUSER_FAIL = 108;

  SP_GM_DELUSER = 109;
  SP_SM_DELUSER_SUCCESS = 110;
  SP_SM_DELUSER_FAIL = 111;

  SP_GM_CHGUSER = 112;
  SP_SM_CHGUSER_SUCCESS = 113;
  SP_SM_CHGUSER_FAIL = 114;

  SP_GM_SEARCHUSER = 115;
  SP_SM_SEARCHUSER_SUCCESS = 116;
  SP_SM_SEARCHUSER_FAIL = 117;

  GM_LOGIN = 118;
  SM_LOGIN_SUCCESS = 119;
  SM_LOGIN_FAIL = 120;

  GM_GETUSER = 121;
  SM_GETUSER_SUCCESS = 122;
  SM_GETUSER_FAIL = 123;

  GM_ADDUSER = 124;
  SM_ADDUSER_SUCCESS = 125;
  SM_ADDUSER_FAIL = 126;

  GM_DELUSER = 127;
  SM_DELUSER_SUCCESS = 128;
  SM_DELUSER_FAIL = 129;

  GM_CHGUSER = 130;
  SM_CHGUSER_SUCCESS = 131;
  SM_CHGUSER_FAIL = 132;

  GM_SEARCHUSER = 133;
  SM_SEARCHUSER_SUCCESS = 134;
  SM_SEARCHUSER_FAIL = 135;
// //////////////////////////////////////////////////////////////////////////////

type
  TProgamType = (tDBServer, tLoginSrv, tLogServer, tM2Server, tLoginGate,
    tLoginGate1, tSelGate, tSelGate1, tRunGate, tRunGate1, tRunGate2,
    tRunGate3, tRunGate4, tRunGate5, tRunGate6, tRunGate7);

  // 176   185   英雄版  连击版   传奇续章     外传   归来
  TClientUIType = (cv176, cv185, cvHero, cvSerial, cvMirSequel, cvMirs, cvMirReturn);

  TM2ServerConfig = packed record                                                                   // Size 20
    nOffset: Integer;
    nParam: Cardinal;
    nSize: Integer;
  end;
  pTM2ServerConfig = ^TM2ServerConfig;

implementation

end.
