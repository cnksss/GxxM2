unit RunGatePluginInterface;

interface

uses
  Windows, Classes, SysUtils, Menus, Grobal2_Ex, IocpTcpServer;

type
  _TObject = TObject;
  _TMenuItem = TMenuItem;
  _TList = TList;
  _TClientContext = TIocpClientContext;

  TNotifyEventEx = procedure(Sender: _TObject); stdcall;

  //-------------------------------------------------------------------------------------------------------------------------------------------------------
  //---------------------------------------------- TList 对象列表 -----------------------------------------------------------------------------------------
  //-------------------------------------------------------------------------------------------------------------------------------------------------------

  { 列表创建 }
  TList_Create =
    function (): _TList; stdcall;

  { 列表释放 }
  TList_Free =
    procedure(List: _TList); stdcall;

  { 取列表数量 }
  TList_Count =
    function (List: _TList): Integer; stdcall;

  { 清空列表 }
  TList_Clear =
    procedure(List: _TList); stdcall;

  { 添加元素 }
  TList_Add =
    procedure(List: _TList; Item: Pointer); stdcall;

  { 插入元素}
  TList_Insert =
    procedure(List: _TList; Index: Integer; Item: Pointer); stdcall;

  { 根据元素删除 }
  TList_Remove =
     procedure(List: _TList; Item: Pointer); stdcall;

  { 根据索引删除 }
  TList_Delete =
    procedure(List: _TList; Index: Integer); stdcall;

  { 取得元素 }
  TList_GetItem =
    function (List: _TList; Index: Integer): Pointer; stdcall;

  { 设置元素 }
  TList_SetItem =
    procedure(List: _TList; Index: Integer; Item: Pointer); stdcall;

  { 得到元素的索引 }
  TList_IndexOf =
    function (List: _TList; Item: Pointer): Integer; stdcall;

  { 交换元素 }
  TList_Exchange =
    procedure(List: _TList; Index1, Index2: Integer); stdcall;

  { 复制到另一个列表 }
  TList_CopyTo =
    procedure(Source, Dest: _TList); stdcall;


  //-------------------------------------------------------------------------------------------------------------------------------------------------------
  //---------------------------------------------- TMenu 菜单 ---------------------------------------------------------------------------------------------
  //-------------------------------------------------------------------------------------------------------------------------------------------------------

  { 获取子菜单数量 }
  TMenu_Count =
    function (MenuItem: _TMenuItem): Integer; stdcall;

  { 获取某个子菜单 }
  TMenu_GetItems =
    function (MenuItem: _TMenuItem; Index: Integer): _TMenuItem; stdcall;

  { 添加菜单 }
  TMenu_Add =
    function (PlugID: Integer; MenuItem: _TMenuItem; Caption: PAnsiChar;
              Tag: Integer; OnClick: TNotifyEventEx): _TMenuItem; stdcall;

  { 插入菜单 }
  TMenu_Insert =
    function (PlugID: Integer; MenuItem: _TMenuItem; Index: Integer;
              Caption: PAnsiChar; Tag: Integer; OnClick: TNotifyEventEx): _TMenuItem; stdcall;

  { 获取菜单标题 }
  TMenu_GetCaption =
    function (MenuItem: _TMenuItem; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;

  { 设置菜单标题}
  TMenu_SetCaption =
    procedure(MenuItem: _TMenuItem; Caption: PAnsiChar); stdcall;

  { 获取菜单可用 }
  TMenu_GetEnabled =
    function (MenuItem: _TMenuItem): BOOL; stdcall;

  { 设置菜单可用 }
  TMenu_SetEnabled =
    procedure(MenuItem: _TMenuItem; Enabled: BOOL); stdcall;

  { 获取菜单可见 }
  TMenu_GetVisable =
    function (MenuItem: _TMenuItem): BOOL; stdcall;

  { 设置菜单可见 }
  TMenu_SetVisable =
    procedure(MenuItem: _TMenuItem; Visible: BOOL); stdcall;

  { 获取菜单选中状态 }
  TMenu_GetChecked =
    function (MenuItem: _TMenuItem): BOOL; stdcall;

  { 设置菜单选中状态 }
  TMenu_SetChecked =
    procedure(MenuItem: _TMenuItem; Checked: BOOL); stdcall;

  { 获取菜单是否为单选 }
  TMenu_GetRadioItem =
    function (MenuItem: _TMenuItem): BOOL; stdcall;

  { 设置菜单是否为单选 }
  TMenu_SetRadioItem =
    procedure(MenuItem: _TMenuItem; IsRadioItem: BOOL); stdcall;

  { 获取菜单单选分组 }
  TMenu_GetGroupIndex =
    function (MenuItem: _TMenuItem): Integer; stdcall;

  { 设置菜单单选分组 }
  TMenu_SetGroupIndex =
    procedure(MenuItem: _TMenuItem; Value: Integer); stdcall;

  { 获取附加数据 }
  TMenu_GetTag =
    function (MenuItem: _TMenuItem): Integer; stdcall;

  { 设置附加数据 }
  TMenu_SetTag =
    procedure(MenuItem: _TMenuItem; Value: Integer); stdcall;

  //-------------------------------------------------------------------------------------------------------------------------------------------------------
  //---------------------------------------------- TClientContext 客户端会话 ------------------------------------------------------------------------------
  //-------------------------------------------------------------------------------------------------------------------------------------------------------

  { 获取连接ID，由内部分配 }
  TClientContext_GetContextID =
    function(ClientContext: _TClientContext): Integer; stdcall;

  { 获取客户端IP值 }
  TClientContext_GetIPValue =
    function(ClientContext: _TClientContext): DWORD; stdcall;

  { 获取客户端IP地址 }
  TClientContext_GetIpAddr =
    function(ClientContext: _TClientContext; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;

  { 获取连接端口 }
  TClientContext_GetPort =
    function(ClientContext: _TClientContext): Word; stdcall;

  { 是否为175兼容端 }
  TClientContext_IsOldClient =
    function(ClientContext: _TClientContext): BOOL; stdcall;

  { 客户端是否点击公告 }
  TClientContext_IsLoginNotice =
    function(ClientContext: _TClientContext): BOOL; stdcall;

  { 客户端是否做完所有初始化，准备愉快的玩耍 }
  TClientContext_IsPlayGame =
    function(ClientContext: _TClientContext): BOOL; stdcall;

  { 角色识别ID 2021-01-20修改 }
  TClientContext_GetRecogId =
    function(ClientContext: _TClientContext): Int64; stdcall;

  { 获取登录帐户 }
  TClientContext_GetAccount =
    function(ClientContext: _TClientContext; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;

  { 获取登录角色 }
  TClientContext_GetChrName =
    function(ClientContext: _TClientContext; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;

  { 获取客户端的机器码 }
  TClientContext_GetMachineID =
    function(ClientContext: _TClientContext; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;

  { 获取角色的职业 }
  TClientContext_GetJob =
    function(ClientContext: _TClientContext): Byte; stdcall;

  { 获取角色的移动速度 }
  TClientContext_GetMoveSpeed =
    function(ClientContext: _TClientContext): Integer; stdcall;

  { 获取角色的攻击速度 }
  TClientContext_GetAttackSpeed =
    function(ClientContext: _TClientContext): Integer; stdcall;

  { 获取角色的魔法速度 }
  TClientContext_GetSpellSpeed =
    function(ClientContext: _TClientContext): Integer; stdcall;

  { 发送消息到客户端 }
  TClientContext_SendToClientMsg =
    procedure(ClientContext: _TClientContext; Msg: PTDefaultMessage; AddData: PAnsiChar; AddDataLen: Integer); stdcall;

  { 发送消息到服务器 }
  TClientContext_SendToM2ServerMsg =
    procedure(ClientContext: _TClientContext; Msg: PTDefaultMessage; AddData: PAnsiChar; AddDataLen: Integer); stdcall;

  { 发送消息到客户端 2019-04-18作废}
  TClientContext_SendToClientMsg_Ex =
    procedure(ClientContext: _TClientContext; SendData: PAnsiChar; SendLen: Integer); stdcall;

  { 发送消息到服务器 2019-04-18作废}
  TClientContext_SendToM2ServerMsg_Ex =
    procedure(ClientContext: _TClientContext; SendData: PAnsiChar; SendLen: Integer); stdcall;

  { 断开客户端连接 }
  TClientContext_Close =
    procedure(ClientContext: _TClientContext); stdcall;

  { 是否客户端等待退出 }
  TClientContext_IsWaitClose =
    function(ClientContext: _TClientContext): BOOL; stdcall;

  { 锁定用户 }
  TClientContext_LockUser =
    procedure(ClientContext: _TClientContext; LockTime: Integer); stdcall;

  { 客户端反外挂模块是否发完  }
  TClientContext_IsSendClientDllComplete =
    function(ClientContext: _TClientContext): BOOL; stdcall;

  { 客户端反外挂模块发完的时间 }
  TClientContext_GetSendClientDllTick =
    function(ClientContext: _TClientContext): DWORD; stdcall;

  //-------------------------------------------------------------------------------------------------------------------------------------------------------
  //---------------------------------------------- TRunGate -----------------------------------------------------------------------------------------------
  //-------------------------------------------------------------------------------------------------------------------------------------------------------

  { 获取网关所在目录 }
  TRunGate_GetAppPath =
    function (Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;

  { 获取网关程序文件名 }
  TRunGate_GetAppFileName =
    function (Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;

  { 获取插件目录 }
  TRunGate_GetPluginPath =
    function (Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;

  { 获取加载的插件文件名 }
  TRunGate_GetPluginFileName =
    function (Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;

  { 网关上输出信息 }
  TRunGate_AddMainLogMsg =
    procedure(Msg: PAnsiChar; nLevel: Integer); stdcall;

  { 编码包头 }
  TRunGate_EncodeMessage =
    function (Msg: PTDefaultMessage; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;

  { 解码包头 }
  TRunGate_DecodeMessage =
    function (Src: PAnsiChar; SrcLen: DWORD; Msg: PTDefaultMessage): BOOL; stdcall;

  { 编码数据 }
  TRunGate_EncodeBuffer =
    function (Src: PAnsiChar; SrcLen: DWORD; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;

  { 解码数据 }
  TRunGate_DecodeBuffer =
    function (Src: PAnsiChar; SrcLen: DWORD; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;

  { 压缩编码 }
  TRunGate_ZLibEncodeBuffer =
    function (Src: PAnsiChar; SrcLen: DWORD;  Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;

  { 压缩解码 }
  TRunGate_ZLibDecodeBuffer =
    function(Src: PAnsiChar; SrcLen: DWORD; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;

  { 获取所有连接 }
  TRunGate_GetOnlineContextList =
    function (List: _TList): Integer; stdcall;

  { 加入IP动态过滤 }
  TRunGate_AddTempBlockIP =
    procedure (IP: PAnsiChar); stdcall;

  { 加入IP永久过滤 }
  TRunGate_AddBlockIP =
    procedure (IP: PAnsiChar); stdcall;

  { 加入Mac动态过滤 }
  TRunGate_AddTempBlockMac =
    procedure (Mac: PAnsiChar); stdcall;

  { 加入Mac永久过滤 }
  TRunGate_AddBlockMac =
    procedure (Mac: PAnsiChar); stdcall;

  { 通知网关，客户端插件已重新加载 }
  TRunGate_NotifyClientDllReload =
    procedure (); stdcall;

type
  TListFunc = record
    Create: TList_Create;                                   // 列表创建
    Free: TList_Free;                                       // 列表释放
    Count: TList_Count;                                     // 取列表数量
    Clear: TList_Clear;                                     // 清空列表
    Add: TList_Add;                                         // 添加元素
    Insert: TList_Insert;                                   // 插入元素}
    Remove: TList_Remove;                                   // 根据元素删除
    Delete: TList_Delete;                                   // 根据索引删除
    GetItem: TList_GetItem;                                 // 取得元素
    SetItem: TList_SetItem;                                 // 设置元素
    IndexOf: TList_IndexOf;                                 // 得到元素的索引
    Exchange: TList_Exchange;                               // 交换元素
    CopyTo: TList_CopyTo;                                   // 复制到另一个列表

    Reserved: array[0..19] of Pointer;
  end;

  TMemuFunc = record                                     
    Count: TMenu_Count;                                     // 获取子菜单数量
    GetItems: TMenu_GetItems;                               // 获取某个子菜单
    Add: TMenu_Add;                                         // 添加菜单
    Insert: TMenu_Insert;                                   // 插入菜单
    GetCaption: TMenu_GetCaption;                           // 获取菜单标题
    SetCaption: TMenu_SetCaption;                           // 设置菜单标题
    GetEnabled: TMenu_GetEnabled;                           // 获取菜单可用
    SetEnabled: TMenu_SetEnabled;                           // 设置菜单可用
    GetVisable: TMenu_GetVisable;                           // 获取菜单可见
    SetVisable: TMenu_SetVisable;                           // 设置菜单可见
    GetChecked: TMenu_GetChecked;                           // 获取菜单选中状态
    SetChecked: TMenu_SetChecked;                           // 设置菜单选中状态
    GetRadioItem: TMenu_GetRadioItem;                       // 获取菜单是否为单选
    SetRadioItem: TMenu_SetRadioItem;                       // 设置菜单是否为单选
    GetGroupIndex: TMenu_GetGroupIndex;                     // 获取菜单单选分组
    SetGroupIndex: TMenu_SetGroupIndex;                     // 设置菜单单选分组
    GetTag: TMenu_GetTag;                                   // 获取附加数据
    SetTag: TMenu_SetTag;                                   // 设置附加数据

    Reserved: array[0..19] of Pointer;
  end;

  TClientContextFunc = record
    GetContextID: TClientContext_GetContextID;              // 获取连接ID，由内部分配
    GetIPValue: TClientContext_GetIPValue;                  // 获取客户端IP值
    GetIpAddr: TClientContext_GetIpAddr;                    // 获取客户端IP地址
    GetPort: TClientContext_GetPort;                        // 获取连接端口
    IsOldClient: TClientContext_IsOldClient;                // 是否为175兼容端
    IsLoginNotice: TClientContext_IsLoginNotice;            // 客户端是否点击公告
    IsPlayGame: TClientContext_IsPlayGame;                  // 客户端是否做完所有初始化，准备愉快的玩耍
    GetRecogId: TClientContext_GetRecogId;                  // 角色识别ID
    GetAccount: TClientContext_GetAccount;                  // 获取登录帐户
    GetChrName: TClientContext_GetChrName;                  // 获取登录角色
    GetMachineID: TClientContext_GetMachineID;              // 获取客户端的机器码
    GetJob: TClientContext_GetJob;                          // 获取角色的职业
    GetMoveSpeed: TClientContext_GetMoveSpeed;              // 获取角色的移动速度
    GetAttackSpeed: TClientContext_GetAttackSpeed;          // 获取角色的攻击速度
    GetSpellSpeed: TClientContext_GetSpellSpeed;            // 获取角色的魔法速度

    SendToClientMsg: TClientContext_SendToClientMsg;        // 发送消息到客户端
    SendToM2ServerMsg: TClientContext_SendToM2ServerMsg;    // 发送到M2服务器
    
    SendToClientMsg_Ex: TClientContext_SendToClientMsg_Ex;     // 发送消息到客户端  2019-04-18作废
    SendToM2ServerMsg_Ex: TClientContext_SendToM2ServerMsg_Ex; // 发送到M2服务器  2019-04-18作废

    Close: TClientContext_Close;                            // 断开客户端连接
    IsWaitClose: TClientContext_IsWaitClose;                // 是否客户端等待退出
    LockUser: TClientContext_LockUser;                      // 锁定用户

    IsSendClientDllComplete: TClientContext_IsSendClientDllComplete;    // 客户端反外挂模块是否发完
    GetSendClientDllTick: TClientContext_GetSendClientDllTick;          // 客户端反外挂模块发完的时间

    Reserved: array[0..39] of Pointer;
  end;

  TRunGateFunc = record
    GetAppPath: TRunGate_GetAppPath;                        // 获取网关所在目录
    GetAppFileName: TRunGate_GetAppFileName;                // 获取网关程序文件名
    GetPluginPath: TRunGate_GetPluginPath;                  // 获取插件目录
    GetPluginFileName: TRunGate_GetPluginFileName;          // 获取加载的插件文件名

    AddMainLogMsg: TRunGate_AddMainLogMsg;                  // 网关上显示信息
    EncodeMessage: TRunGate_EncodeMessage;                  // 编码包头
    DecodeMessage: TRunGate_DecodeMessage;                  // 解码包头
    EncodeBuffer:  TRunGate_EncodeBuffer;                   // 编码数据
    DecodeBuffer: TRunGate_DecodeBuffer;                    // 解码数据
    ZLibEncodeBuffer: TRunGate_ZLibEncodeBuffer;            // 压缩编码
    ZLibDecodeBuffer: TRunGate_ZLibDecodeBuffer;            // 压缩解码

    GetOnlineContextList: TRunGate_GetOnlineContextList;    // 获取所有连接

    AddTempBlockIP: TRunGate_AddTempBlockIP;                // 加入IP动态过滤
    AddBlockIP: TRunGate_AddBlockIP;                        // 加入IP永久过滤
    AddTempBlockMac: TRunGate_AddTempBlockMac;              // 加入Mac动态过滤
    AddBlockMac: TRunGate_AddBlockMac;                      // 加入Mac永久过滤

    NotifyClientDllReload: TRunGate_NotifyClientDllReload;  // 通知网关，客户端插件已重新加载

    Reserved: array[0..69] of Pointer;
  end;

  PAppFuncDef = ^TAppFuncDef;
  TAppFuncDef = record
    PluginID: Integer;                                      // 插件ID

    List: TListFunc;                                        // 列表
    Menu: TMemuFunc;                                        // 菜单
    Context: TClientContextFunc;                            // 客户端

    RunGate: TRunGateFunc;                                  // 网关函数

    Reserved: array[0..999] of Pointer;
  end;

implementation

end.
