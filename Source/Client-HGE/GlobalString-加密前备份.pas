unit GlobalString;

interface

uses
  SDK;

resourcestring
  SQuitAppAsk =               '你想退出游戏吗？';
  SAppLogoutAsk =             '你想退出到选择角色界面吗？';

  SAppInit01Log =             'AppInit: 1';
  SAppInit02Log =             'AppInit: 2';
  SAppInit03Log =             'AppInit: 3';
  SAppInit04Log =             'AppInit: 4';
  SAppInit05Log =             'AppInit: 5';
  SAppInit06Log =             'AppInit: 6';
  SAppInit07Log =             'AppInit: 7';
  SAppInit08Log =             'AppInit: 8';
  SAppInit09Log =             'AppInit: 9';
  SAppInit10Log =             'AppInit: 10';
  SAppInit11Log =             'AppInit: 11';

  SAppInitErr01 =             'Exception: Application Init Error 1';
  SAppInitErr02 =             'Exception: Application Init Error 2';
  SAppLoadConfigErr =         'Exception: Application LoadConfig Error';
  SAppCreateMainFormErr =     'Exception: Application Create MainForm Error';

  SLoadMemoryPlugFail =       '加载插件失败:';

{$IF TESTMODE = 1}
  SBTMemoryMoudleErr01 =      'BuildImportTable: can''t load library: ';
  SBTMemoryMoudleErr02 =      'BuildImportTable: ReallocMemory failed';
  SBTMemoryMoudleErr03 =      'BuildImportTable: GetProcAddress failed';
  SBTMemoryMoudleErr04 =      'FinalizeSections: VirtualProtect failed';
  SBTMemoryMoudleErr05 =      'BTMemoryLoadLibary: dll dos header is not valid';
  SBTMemoryMoudleErr06 =      'BTMemoryLoadLibary: IMAGE_NT_SIGNATURE is not valid';
  SBTMemoryMoudleErr07 =      'BTMemoryLoadLibary: VirtualAlloc failed';
  SBTMemoryMoudleErr08 =      'BTMemoryLoadLibary: BuildImportTable failed';
  SBTMemoryMoudleErr09 =      'BTMemoryLoadLibary: Get DLLEntyPoint failed';
  SBTMemoryMoudleErr10 =      'BTMemoryLoadLibary: Can''t attach library';
  SBTMemoryMoudleErr11 =      'BTMemoryLoadLibary: except code:';
  SBTMemoryMoudleErr12 =      'BTMemoryGetProcAddress: no export table found';
  SBTMemoryMoudleErr13 =      'BTMemoryGetProcAddress: DLL doesn''t export anything';
  SBTMemoryMoudleErr14 =      'BTMemoryGetProcAddress: exported symbol not found';
  SBTMemoryMoudleErr15 =      'BTMemoryGetProcAddress: name <-> ordinal number don''t match';
  SBTMemoryMoudleErr16 =      'BTMemoryGetProcAddress: no export table found';
  SBTMemoryMoudleErr17 =      'BTMemoryGetProcAddress: DLL doesn''t export anything';
  SBTMemoryMoudleErr18 =      'BTMemoryGetProcAddress: exported index not found';
  SBTMemoryMoudleErr19 =      'BTMemoryGetProcAddress: index out of range';
{$ELSE}
  SBTMemoryMoudleErr01 =      ' ';
  SBTMemoryMoudleErr02 =      ' ';
  SBTMemoryMoudleErr03 =      ' ';
  SBTMemoryMoudleErr04 =      ' ';
  SBTMemoryMoudleErr05 =      ' ';
  SBTMemoryMoudleErr06 =      ' ';
  SBTMemoryMoudleErr07 =      ' ';
  SBTMemoryMoudleErr08 =      ' ';
  SBTMemoryMoudleErr09 =      ' ';
  SBTMemoryMoudleErr10 =      ' ';
  SBTMemoryMoudleErr11 =      ' ';
  SBTMemoryMoudleErr12 =      ' ';
  SBTMemoryMoudleErr13 =      ' ';
  SBTMemoryMoudleErr14 =      ' ';
  SBTMemoryMoudleErr15 =      ' ';
  SBTMemoryMoudleErr16 =      ' ';
  SBTMemoryMoudleErr17 =      ' ';
  SBTMemoryMoudleErr18 =      ' ';
  SBTMemoryMoudleErr19 =      ' ';
{$IFEND}

  SLoadResourceProgress =     '正在读取资源，请稍候...%d%%';

  SClientVersionFail =        '当前登录器不匹配，请到官方网站更新登录器！';
  SClientItemSizeNotEqualM2 = 'TClientItem结构和M2Server中的大小不符';

  SFindModuleMsg =            'Uc=LYcdqMBmIUoQ_Q@QFY@uhHCIEHS]>PPMfYpqMIbItXcQfQNxoYaLnToYRFqMAV_QCOo]mVsTgTrIUU?M`Z@meMp@lNPiHN`IhT`a?JQE?N_=eFp]RNl';
  SFindProcessMsg =           'Uc=LYcdqMBmIUoQ_Q@QFY@uhHCIEHS]>PPMfYpqMIbItXcQfQNxoYaaiLO]QLrEnYOQGPOaqGs]]TsYsXPHmGqI@ZSInICUlFrapQqEeMa]`TOA=VoAVWsEcKOp';
  SCheckErrorMsg =            'O>xlLb\lHrhuO@asPbipQA]RI>iJM_aNO_=rYCY@RcAQZ_QMZ>i?MNyqIridIBqFVsAjVOaSGrUUPRxy';
  SCheckErrorMsg2 =           'Uc=LYcdqMBmIUoQ_Q@QFY@uhHCIEHS]>PPMfYpqMIbItXcQfQNxoYaamRAEfYcepWBxnVNxlTr]dN`arFrufWqIhTNiVQC]]QAQsHSeoHbEcYR]T';


  SErrorText1 =               '输入乱码的文字.';
  SErrorText2 =               '乱码的文字不能为空！';
  SErrorText3 =               'debug\文字乱码.dat';

  SUpdateStatus =             '云客户端状态: ';
  SUpdateCurrentSpeed =       '当前传输速度: ';
  SUpdateTotalSize =          '累计传输流量: ';
  SUpdateTryAgain =           '正在尝试重连...';
  SUpdateNotOpen =            '未启用微端';
  SUpdateChangeServer =       '双击图标切换线路';

  SUpdatePasswordErr =        '[Password] TUpdateEngine::Update password error';
  SUpdateTimeOutFile =        '[TimeOut] TUpdateEngine::Update file: %s';
  SUpdateTimeOutIndex =       '[TimeOut] TUpdateEngine::Update index: %s';
  SUpdateTimeOutImage =       '[TimeOut] TUpdateEngine::Update image: %s; index: %d';
  SUpdateWavDecompressErr =   '[Exception] TUpdateEngine::UpdateWav Decompress error: %s';
  SUpdateMapDecompressErr =   '[Exception] TUpdateEngine::UpdateMap Decompress error: %s';
  SUpdateOtherDecompressErr = '[Exception] TUpdateEngine::UpdateOther Decompress error: %s';
  SUpdatePakDecompressErr =   '[Exception] TUpdateEngine::UpdatePak Decompress error: %s; index: %d';
  SUpdatePakWriteLenErr =     '[Exception] TUpdateEngine::UpdatePak Write file len error: %s; index: %d';
  SUpdatePakWriteErr =        '[Exception] TUpdateEngine::UpdatePak Write file error: %s; index: %d';
  SUpdatePakFreeMemErr =      '[Exception] TUpdateEngine::UpdatePak Free memory error: %s; index: %d';
  SUpdateWzlDecompressErr =   '[Exception] TUpdateEngine::UpdateWzl Decompress error: %s; index: %d';
  SUpdateWzlWriteErr =        '[Exception] TUpdateEngine::UpdateWzl Write file error: %s; index: %d';
  SUpdateWzlImageErr =        '[Exception] TUpdateEngine::UpdateWzl image: %s error; index: %d';
  SUpdateWzlFreeMemErr =      '[Exception] TUpdateEngine::UpdateWzl Free memory error: %s; index: %d';
  SUpdateWzlIndexErr1 =       '[Exception] TUpdateEngine::UpdateWzl index error: %s';
  SUpdateWzlIndexErr2 =       '[Exception] TUpdateEngine::UpdateWzl index 2 error: %s';
  SUpdateWzlIndexErr3 =       '[Exception] TUpdateEngine::UpdateWzl index 3 error: %s';

  SJSYConfigDlgSaveErr =      '[Exception] TJSYConfigDlg::SaveConfigFile';
  SMirConfigDlgSaveErr =      '[Exception] TMirConfigDlg::SaveConfigFile';

  SPakPasswordErr =           '[Password] TPakImages::Update index password error: %s';
  SPakPasswordErr2 =          '[Password] TPakImages::Initialize password error: %s';
  SPakGetCacheImageErr =      '[Exception] TPakImages::GetCachedImage file error: %s; index: %d';
  SPakLoadDxImageErr =        '[Exception] TPakImages::LoadDxImage position error: %s; Index: %d; X: %d; Y: %d';
  SPakLoadDxImageLenErr =     '[Exception] TPakImages::LoadDxImage Length error: %s; Index: %d; Length: %d';
  SPakLoadDxImageSizeErr =     '[Exception] TPakImages::LoadDxImage Size error: %s; Index: %d; Width: %d; Height: %d';

  SPakImageDecompressErr =    '[Exception] TPakImages::LoadDxImage decompressBuf: %s; Index: %d';
  SPakLoadDxImageErr2 =       '[Exception] TPakImages::LoadDxImage error 2: %s; Index: %d';
  SPakImageDecompressErr2 =   '[Exception] TPakImages::LoadDxImage decompressBuf error 2: %s; Index: %d';
  SPakLoadDxImageErr3 =       '[Exception] TPakImages::LoadDxImage error 3: %s; Index: %d';
  SPakLoadDxImageErr4 =       '[Exception] TPakImages::LoadDxImage error 4: %s; Index: %d';
  SPakLoadDxImageErr5 =       '[Exception] TPakImages::LoadDxImage error 5: %s; Index: %d';
  SPakLoadDxGrayImageErr =    '[Exception] TPakImages::LoadDxGrayImage position error: %s; Index: %d; X: %d; Y: %d';
  SPakLoadDxGrayImageLenErr = '[Exception] TPakImages::LoadDxGrayImage Length error: %s; Index: %d; Length: %d';
  SPakLoadDxGrayImageSizeErr ='[Exception] TPakImages::LoadDxGrayImage Size error: %s; Index: %d; Width: %d; Height: %d';
  SPakGrayImageDecompErr =    '[Exception] TPakImages::LoadDxGrayImage decompressBuf error: %s; Index: %d';
  SPakGrayImageDecompErr2 =   '[Exception] TPakImages::LoadDxGrayImage decompressBuf error 2: %s; Index: %d';
  SPakBrightImageErr =        '[Exception] TPakImages::LoadDxBrightImage position error: %s; Index: %d; X: %d; Y: %d';
  SPakBrightImageLenErr =     '[Exception] TPakImages::LoadDxBrightImage Length error: %s; Index: %d; Length: %d';
  SPakBrightImageSizeErr =    '[Exception] TPakImages::LoadDxBrightImage Size error: %s; Index: %d; Width: %d; Height: %d';
  SPakBrightImageDecompErr =  '[Exception] TPakImages::LoadDxBrightImage decompressBuf error: %s; Index: %d';
  SPakBrightImageDecompErr2 = '[Exception] TPakImages::LoadDxBrightImage decompressBuf error 2: %s; Index: %d';

  SWzlInitErr =               '[Exception] TWzlImages::Initialize error: %s';
  SWzlLoadIndexErr =          '[Exception] TWzlImages::LoadIndex error: %s';
  SWzlBrightImageDecompErr =  '[Exception] TWzlImages::LoadDxBrightImage decompressBuf error: %s; Index: %d';
  SWzlBrightImageErr =        '[Exception] TWzlImages::LoadDxBrightImage position error: %s; Index: %d; X: %d; Y: %d';
  SWzlBrightImageLenErr =     '[Exception] TWzlImages::LoadDxBrightImage Length error: %s; Index: %d; Length: %d';
  SWzlBrightImageSizeErr =    '[Exception] TWzlImages::LoadDxBrightImage Size error: %s; Index: %d; Width: %d; Height: %d';


  SWZLGrayImageDecompErr =    '[Exception] TWzlImages::LoadDxGrayImage decompressBuf error: %s; Index: %d';
  SWzlGrayImageErr =          '[Exception] TWzlImages::LoadDxGrayImage position error: %s; Index: %d; X: %d; Y: %d';
  SWzlGrayImageLenErr =       '[Exception] TWzlImages::LoadDxGrayImage Length error: %s; Index: %d; Length: %d';
  SWzlGrayImageSizeErr =      '[Exception] TWzlImages::LoadDxGrayImage Size error: %s; Index: %d; Width: %d; Height: %d';


  SWZLImageDecompressErr =    '[Exception] TWzlImages::LoadDxImage decompressBuf: %s; Index: %d';
  SWzlImageErr =              '[Exception] TWzlImages::LoadDxImage position error: %s; Index: %d; X: %d; Y: %d';
  SWzlImageLenErr =           '[Exception] TWzlImages::LoadDxImage Length error: %s; Index: %d; Length: %d';
  SWzlImageSizeErr =          '[Exception] TWzlImages::LoadDxImage Size error: %s; Index: %d; Width: %d; Height: %d';

  SWilLoadDxBrightImageErr =    '[Exception] TWMImages::LoadDxBrightImage position error: %s; X: %d; Y: %d';
  SWilLoadDxBrightImageSizeErr ='[Exception] TWMImages::LoadDxBrightImage Size error: %s; Width: %d; Height: %d';

  SWilLoadDxGrayImageErr =    '[Exception] TWMImages::LoadDxGrayImage position error: %s; X: %d; Y: %d';
  SWilLoadDxGrayImageSizeErr ='[Exception] TWMImages::LoadDxGrayImage Size error: %s; Width: %d; Height: %d';

  SWilLoadDxImageErr =        '[Exception] TWMImages::LoadDxImage position error: %s; X: %d; Y: %d';
  SWilLoadDxImageSizeErr =    '[Exception] TWMImages::LoadDxImage Size error: %s; Width: %d; Height: %d';

  SFrmMainTimerEventErr =     '[Exception] TfrmMain::TimerEvent';

  SPetMsg1 =                  '请先召唤宠物再使用物品.';
  SPetMsg2 =                  '是否确认学习宠物技能 "%s"？';

  SBindHint1 =                '        <NewopUI:300:3> 已绑定';
  SBindHint2 =                '<NewopUI:300:2> 已绑定';

  SUpgradeDescText =          ';技能类型(普通技能/内功技能),技能名,需要强化等级,强化描述';

  SAuctionHintMsg1 =          '1.拍卖物品购买竞价时当输入价格等于或者大于一口价则按照';
  SAuctionHintMsg2 =          '  一口价费用进行立即购买';
  SAuctionHintMsg3 =          '2.上架物品左侧为你背包可出售物品，选中即可进行上架操作';
  SAuctionHintMsg4 =          '3.物品上架设置最低承受价和理想价后确定上架';
  SAuctionHintMsg5 =          '4.选择发送全服广播后会进行全服广播并将物品置顶到购买栏';
  SAuctionHintMsg6 =          '5.竞价过和购买成功的物品在我的关注页面查看操作';
  SAuctionHintMsg7 =          '6.物品上架后就无法取回，请慎重设置拍卖底价';
  SAuctionHintMsg8 =          '7.点击当前价格、一口价、剩余时间可以对物品列表进行排序';
  SAuctionHintMsg9 =          '8.选择物品品质(名字颜色)可以快速筛选出你需求的品质物品';

  SBigMapHint =               '255/左键单击自动寻路\255/右键单击自动传送\251/(传送需要佩戴传送装备)\250/(M键或Tab键打开或关闭该窗口)';
  SFindPathHint =             '自动移动至坐标(%d:%d)，点击鼠标任意键停止...';
  SPathGotoXYOK =             '自动移动坐标点(%d:%d)已到达';
  SPathGotoXYFail =           '自动移动坐标点(%d:%d)不可到达';

  SL2PasswordMsg1 =           '必须为帐户设置二级密码才能继续游戏，请输入新的二级密码：';
  SL2PasswordMsg2 =           '请再一次输入二级密码：';
  SL2PasswordMsg3 =           '2次密码不一致，请输入新的二级密码：';
  SL2PasswordMsg4 =           '二级密码不能和帐户名相同，请重新输入：';
  SL2PasswordMsg5 =           '二级密码不能和帐户密码相同，请重新输入：';
  SL2PasswordMsg6 =           '二级密码不能不能包含字符"@"及"/"，请重新输入：';
  SL2PasswordMsg7 =           '二级密码错误！';
  SL2PasswordMsg8 =           '未设置二级密码，连接中断';
  SL2PasswordMsg9 =           '二级密码设置成功，请牢记';
  SL2PasswordMsg10 =          '请输入二级密码：';
  SL2PasswordMsg11 =          '密码错误，请再次输入二级密码：';

  SRandCodeEmptyMsg =         '验证码不能为空！';
  SPasswordInptu2Msg =        '二次输入的密码不匹配！';
  SPasswordLenErr =           '密码长度必须大于3位！';
  SPasswordNotEqual =         '两次输入的密码不一致！';

  SNoFlute =                  '[%s]没有镶嵌宝石！';

  SRemoveStoneMsg1 =          '[卸下失败]：提交的信息错误！';
  SRemoveStoneMsg2 =          '[卸下失败]：卸下所使用的道具错误！';
  SRemoveStoneMsg3 =          '[卸下失败]：装备没有可卸下的宝石！';

  SStoneMsg1 =                '相同属性的宝石镶嵌不能超过%d个';
  SStoneMsg2 =                '相同宝石的镶嵌不能超过%d个';

  SUnknowError =              '[失败]：未知错误！';

  // 拍卖
  SAuctionAddNoItem =         '请先选择要上架的物品';
  SAuctionAddBindItem =       '绑定物品不允许上架';
  SAuctionStartSellNoItem =   '请先放入要拍卖的物品';
  SAuctionStartSellPrice1 =   '拍卖底价必须大于0';
  SAuctionStartSellPrice2 =   '拍卖底价必须小于一口价';
  SAuctionStartSellPrice3 =   '价格必须大于底价和当前价格！';
  SAuctionNoSelectCurrentType =   '请选择价格类型！';
  SAuctionStartSellAsk =      '是否将物品 %s 以 %d {%s/AUTOCOLOR=250,251,254,222} 的底价拍卖';

  SAuctionTradingStatus01 =   '订单删除';
  SAuctionTradingStatus02 =   '物品流拍';
  SAuctionTradingStatus03 =   '拍卖成功';

  SAuctionAttentionMsg1 =     '[关注物品失败]：关注的物品不存在！';
  SAuctionAttentionMsg2 =     '[关注物品失败]：物品已经被关注！';
  SAuctionAttentionMsg3 =     '[取消关注失败]：出价的物品不能取消关注！';
  SAuctionAttentionMsg4 =     '[取消关注失败]：该物品已取消关注或其他原因！';

  SAuctionAddItemMsg1 =       '[失败]：拍卖底价错误！';
  SAuctionAddItemMsg2 =       '[失败]：拍卖一口价错误！';
  SAuctionAddItemMsg3 =       '[失败]：底价必须小于一口价！';
  SAuctionAddItemMsg4 =       '[失败]：正在拍卖的物品已达到最大数量！';
  SAuctionAddItemMsg4_2 =     '请先取回“流拍的物品”及“成功拍得的物品”！';
  SAuctionAddItemMsg5 =       '[失败]：物品禁止拍卖！';
  SAuctionAddItemMsg6 =       '[失败]：发送全服广播费用不够！';
  SAuctionAddItemMsg7 =       '[失败]：绑定的物品禁止拍卖！';
  SAuctionAddItemMsg8 =       '[失败]：地图禁止拍卖！';
  SAuctionAddItemMsg9 =       '[失败]：拍卖货币类型错误！';
  SAuctionAddItemMsg10 =      '[失败]：拍卖货币类型错误，请重新打开物品上架页面！';
  SAuctionAddItemMsg11 =      '[失败]：拍卖底价不能低于最低价';
  SAuctionAddItemMsg12 =      '[失败]：拍卖一口价不高于最高价';

  SAuctionCancelItemMsg1 =    '[取消竞拍失败]：未找到竞拍记录！';
  SAuctionCancelItemMsg2 =    '[取消竞拍失败]：你无权取消该竞拍物品！';
  SAuctionCancelItemMsg3 =    '[取消竞拍失败]：物品已有人出价！';
  SAuctionCancelItemMsg4 =    '[取消竞拍失败]：物品无法取消！';

  SAuctionRetrieveItemMsg1 =  '[取回物品失败]：未找到竞拍记录！';
  SAuctionRetrieveItemMsg2 =  '[取回物品失败]：你无权取回该竞拍物品！';
  SAuctionRetrieveItemMsg3 =  '[取回物品失败]：物品无法取回！';
  SAuctionRetrieveItemMsg4 =  '[取回物品失败]：背包空间不够';

  SAuctionDeleteItemMsg1 =    '[删除物品失败]：未找到竞拍记录！';
  SAuctionDeleteItemMsg2 =    '[删除物品失败]：你无权删除该竞拍物品！';
  SAuctionDeleteItemMsg3 =    '[删除物品失败]：物品未取回！';

  SAuctionItemNoExistsMsg =   '物品不存在！';
  SAuctionItemStopMsg =       '物品竞拍结束！';
  SAuctionItemIsSelfMsg =     '不能参与自己竞拍的物品！';
  SAuctionItemDisableMsg =    '物品禁止拍卖！';
  SAuctionItemPricesErrMsg1 = '出价不能低于底价！';
  SAuctionItemPricesErrMsg2 = '出价必须高于现价！';
  SAuctionItemMapDisableMsg = '地图禁止拍卖！';

  SAuctionItemBuyFail1 =      '[竞价失败]';
  SAuctionItemBuyFail2 =      '[一口价失败]';
  SAuctionItemJoinBuy =       '参与竞价';
  SAuctionItemJoinAuction =   '竞拍';

  SAuctionItemDelete =        '删除记录';
  SAuctionItemGetBack =       '取回物品';

  SAuctionItemDelete2 =       '删关注';
  SAuctionItemGetBack2 =      '取物品';

  SNGHintAutoHideMode =       '勾选此项道士将自动使用隐身术';
  SNGHintGroupAttack =        '该选项用于控制是否使用群攻魔法列表勾选的技能';
  SNGHintDisableSelfStruck =  '自己受到攻击时不弯腰';
  SNGHintContinueButchItem =  '挖尸体时，在手动挖一次后自动挖';
  SNGHintExpFilter =          '当获取经验小于下面指定的经验时不显示';
  SNGHintDuraWarning =        '当装备持久低于最大持久的10%时提示';
  SNGHintSpecialQuickFlash =  '列表中勾选了“特殊”的物品，闪光速度变快';
  SNGHintHideItemEffect =     '是否隐藏地面物品的特效显示';
  SNGHintHumManuallySnowWind = '勾选鼠标指哪打哪儿，不勾选自动攻击最后攻击的怪物目标';
  SNGHintBagFastItemCmp =     '快速分辨背包内属性比已佩戴装备属性高的物品';
  SNGHintShowHPUnit =         '血量或MP超过亿显示E；超过10万显示W';
  SNGHintDefaultItem =        '恢复默认配置';
  SNGHintOnlyShowUserName =   '勾选后只显示人物名字，而不显示封号、行会等附加信息';
  SNGHintAutoOpenSpell =      '无攻击目标时自动凝聚需CD间隔的技能，此项会不停消耗MP';

  SGameStopMsg1 =             '检测到使用外挂，游戏已被中止。 如有问题请联系游戏管理员。Code=2';
  SGameStopMsg2 =             '速度异常，游戏已被中止。 如有问题请联系游戏管理员。';

  SCannotExitGame1 =          '输入验证码时不能退出游戏！';
  SCannotExitGame2 =          '攻击状态不能退出游戏！';

  SUserShopGetBack =          '取回';
  SUserShopBuy =              '购买';
  SUserShopSell =             '出售';
  SUserShopSelled =           '已售';
  SUserShopRetunMyStorage =   '返回仓库';
  SUserShopDeleteItem =       '删除';
  SUserShopGetMoney =         '取款';
  SUserShopToShop =           '放入店铺';

  SUserShopMoneyType =        '请选择金币类型';
  SUserShopToStorage =        '放入仓库';
  SUserShopOperateAsk =       '是否将物品 %s 以 %d {%s/AUTOCOLOR=250,251,254,222} %s';

  SUserShopQueryFail =        '[失败]：%s还没有店铺！';

  SMyShopAddItemFail1 =       '[物品增加失败]：操作过于频繁请稍后在试！';
  SMyShopAddItemFail2 =       '[物品增加失败]：你还没有店铺！';
  SMyShopAddItemFail3 =       '[物品增加失败]：加入店铺的物品价格必须大于0！';
  SMyShopAddItemFail4 =       '[物品增加失败]：该物品禁止交易！';
  SMyShopAddItemFail5 =       '[物品增加失败]：该物品禁止存仓库！';
  SMyShopAddItemFail6 =       '[物品增加失败]：只能同时出售%d个物品';
  SMyShopAddItemFail7 =       '[物品增加失败]：你的仓库只能存放%d个物品';
  SMyShopAddItemFail8 =       '[物品增加失败]：该物品禁止放入个人商店！';
  SMyShopAddItemFail9 =       '[物品增加失败]：该地图禁止使用个人商店！';
  SMyShopAddItemFail10 =      '[物品增加失败]：未知错误！';

  SMyShopChangeItemOK =       '[成功]：修改成功！';
  SMyShopChangeItemFail1 =    '[修改物品失败]：操作过于频繁请稍后在试！';
  SMyShopChangeItemFail2 =    '[修改物品失败]：你还没有店铺！';
  SMyShopChangeItemFail3 =    '[修改物品失败]：加入店铺的物品价格必须大于0！';
  SMyShopChangeItemFail4 =    '[修改物品失败]：该物品禁止交易！';
  SMyShopChangeItemFail5 =    '[修改物品失败]：该物品禁止存仓库！';
  SMyShopChangeItemFail6 =    '[修改物品失败]：只能同时出售%d个物品';
  SMyShopChangeItemFail7 =    '[修改物品失败]：你的仓库只能存放%d个物品';
  SMyShopChangeItemFail8 =    '[修改物品失败]：该地图禁止使用个人商店！';
  SMyShopChangeItemFail9 =    '[修改物品失败]：未知错误\\该物品可能不存在，请点击下面的“搜索”重新刷新一下！';

  SMyShopMyShopItemFail1 =    '[失败]：操作过于频繁请稍后在试！';
  SMyShopMyShopItemFail2 =    '[失败]：你还没有店铺！';
  SMyShopMyShopItemFail3 =    '[失败]：该物品禁止交易！';
  SMyShopMyShopItemFail4 =    '[失败]：加入店铺的物品价格必须大于0！';
  SMyShopMyShopItemFail5 =    '[失败]：该物品禁止存仓库！';
  SMyShopMyShopItemFail6 =    '[失败]：包裹已满，无法取回！';
  SMyShopMyShopItemFail7 =    '[失败]：只能同时出售%d个物品';
  SMyShopMyShopItemFail8 =    '[失败]：你的仓库只能存放%d个物品';
  SMyShopMyShopItemFail9 =    '[失败]：该地图禁止使用个人商店！';
  SMyShopMyShopItemFail10 =   '[失败]：该物品禁止放入个人商店！';
  SMyShopMyShopItemFail11 =   '[失败]：未知错误\\该物品可能不存在，请点击下面的“搜索”重新刷新一下！';

  SUserShopBuyFail1 =         '[失败]：该物品已经删除！';
  SUserShopBuyFail2 =         '[失败]：该物品已经出售！';
  SUserShopBuyFail3 =         '[失败]：该物品禁止出售！';
  SUserShopBuyFail4 =         '[失败]: 你的%s不够';
  SUserShopBuyFail5 =         '[失败]：你的包裹已满！';
  SUserShopBuyFail6 =         '[失败]：该用户不在线无法购买！';
  SUserShopBuyFail7 =         '[失败]：物品已修改，请刷新后购买！';
  SUserShopBuyFail8 =         '[失败]：该地图禁止使用个人商店！';
  SUserShopBuyFail9 =         '[失败]：该物品已经删除！';
  SUserShopBuyFail10 =        '[失败]：该物品已下架或已经出售！ ';
  SUserShopBuyFail11 =        '点击界面"搜索"按钮可刷新页面！';

  SGuildEditNotice =          '[修改行会公告内容。]';
  SGuildAddMem =              '请输入想加入%s的人物名称：';
  SGuildDelMem =              '请输入想要开除的人物名称：';
  SGuildEditGradeHint =       '[修改行会成员的等级和职位。 # 警告 : 不能增加行会成员/删除行会成员。]';
  SGuildAllyAsk =             '对方结盟行会必需在 [允许结盟]状态下。%s而且二个行会的掌门必须面对面。%s是否确认行会结盟？';
  SGuildAllyScript =          '@联盟';

  SGuildBreakAllyAsk =        '请输入您想取消结盟的行会的名字：';
  SGuildBreakAllyScript =     '@取消联盟 ';

  SGuildOpenFail =            '你仍旧没有加入行会';

  SGuildAddMemberFail1 =      '你没有权利使用这个命令。';
  SGuildAddMemberFail2 =      '想加入进来的成员应该来面对掌门人。';
  SGuildAddMemberFail3 =      '对方已经加入我们的行会。';
  SGuildAddMemberFail4 =      '对方已经加入其他行会。';
  SGuildAddMemberFail5 =      '对方不允许加入行会。';
  SGuildAddMemberFail6 =      '行会成员已经满员。';
  SGuildAddMemberFail7 =      '不同国家人员禁止加行会。';

  SGuildUnMasterSelfFail1 =   '你没有权利使用这个命令。';
  SGuildUnMasterSelfFail2 =   '行会掌门不能卸任！';
  SGuildUnMasterSelfFail3 =   '卸任失败。';

  SGuildJoinToFail1 =         '你已经是行会成员。';
  SGuildJoinToFail2 =         '行会不存在！';
  SGuildJoinToFail3 =         '你申请的行会禁止玩家加入。';
  SGuildJoinToFail4 =         '等级不够';
  SGuildJoinToFail5 =         '职业不符';
  SGuildJoinToFail7 =         '你已经申请加入别的行会：';

  SGuildCancelJoinToFail1 =   '取消失败，申请取消的行会不存在';
  SGuildCancelJoinToFail2 =   '您未申请加入行会';

  SGuildViewJoinFail =        '要查看的行会不存在。';

  SGuildDelMemberFail1 =      '不能使用命令！';
  SGuildDelMemberFail2 =      '此人非本行会成员！';
  SGuildDelMemberFail3 =      '行会掌门人不能开除自己！';
  SGuildDelMemberFail4 =      '不能使用命令Z！';
  SGuildDelMemberFail5 =      '还没有加入行会！';
  SGuildDelMemberFail6 =      '行会掌门不能退出！';

  SGuildRankUpdateFail1 =     '[提示信息] 掌门人位置不能为空。';
  SGuildRankUpdateFail2 =     '[提示信息] 新的行会掌门人已经被传位。';
  SGuildRankUpdateFail3 =     '[提示信息] 一个行会最多只能有二个掌门人。';
  SGuildRankUpdateFail4 =     '[提示信息] 掌门人位置不能为空。';
  SGuildRankUpdateFail5 =     '[提示信息] 不能添加成员/删除成员。';
  SGuildRankUpdateFail6 =     '[提示信息] 职位重复或者出错。';

  SGuildNotMaster =           '您不是行会掌门人，无法进行此操作。';
  SGuildNotMaster1 =          '您不是行会正掌门，无法进行此操作。';
  SGuildMemberFull =          '行会满员';

  SGuildMakeAllyResult1 =     '您无此权限！';
  SGuildMakeAllyResult2 =     '结盟失败！';
  SGuildMakeAllyResult3 =     '行会结盟必须双方掌门人面对面！';
  SGuildMakeAllyResult4 =     '对方行会掌门人不允许结盟！';

  SGuildBreakAllyResult1 =    '解除结盟！';
  SGuildBreakAllyResult2 =    '此行会不是您行会的结盟行会！';
  SGuildBreakAllyResult3 =    '没有此行会！';

  SGuildBuildOK =             '行会建立成功。';

  SGuildBuildFail1 =          '您已经加入其它行会。';
  SGuildBuildFail2 =          '缺少创建费用。';
  SGuildBuildFail3 =          '你没有准备好需要的全部物品。';
  SGuildBuildFail4 =          '创建行会失败！';

  SGuildUserJoinOK =          '您被批准加入行会 ';

  GuildAddAttentionRet1 =     '您已经关注了行会 ';
  GuildAddAttentionRet2 =     '不支持关注行会自己 。';

  SGuildDelAttentionRet1 =    '您还未关注行会 ';

  SGuildRequestAllyRet1 =     '向行会 %s 发送申请成功。';
  SGuildRequestAllyRet2 =     '行会 %s 不存在。';
  SGuildRequestAllyRet3 =     '您已经向行会 %s 发送过申请。';
  SGuildRequestAllyRet4 =     '与行会 %s 已经联盟。';
  SGuildRequestAllyRet5 =     '行会 %s 禁止联盟。';
  SGuildRequestAllyRet6 =     '不支持行会联盟自己。';

  SGuildAcceptAllyRet1 =      '行会 %s 不存在。';
  SGuildAcceptAllyRet2 =      '行会 %s 没有联盟申请。';

  SGuildSetMaster1Ret1 =      '不能转让给自己。';
  SGuildSetMaster1Ret2 =      '目标玩家 %s 不是本行会成员。';

  SGuildAddGuildWarRet1 =     '联盟行会 %s 不能宣战。';
  SGuildAddGuildWarRet2 =     '宣战不能在本服务器上运行！';
  SGuildAddGuildWarRet3 =     '与行会 %s 正在宣战中。';
  SGuildAddGuildWarRet4 =     '你没有足够的金币！';
  SGuildAddGuildWarRet5 =     '宣战失败！';

  SGuildCannotDelMaster =     '不能逐出主掌门人。';

  SGuildMemberMoveToRankRet1 = '不是行会成员。';
  SGuildMemberMoveToRankRet2 = '的封号和目标封号一样。';
  SGuildMemberMoveToRankRet3 = '目标封号不存在。';
  SGuildMemberMoveToRankRet4 = '只有一个管理员。';

  SGJSetPoint =               '（鼠标左键设置路径点，右键清除） ';
  SCustomMonNoConfig =        '！！！！！自定义怪物没有配置信息【Appr=%d】！！！！！';
  SCustomMagicNoConfig =      '！！！！！自定义技能没有配置信息【MagId=%d】！！！！！';

  SFBMapMsg1 =                '副本剩余时间 %.2d:%.2d:%.2d';
  SFBMapMsg2 =                '将在 %d 秒后自动离开副本';
  SFBMapMsg3 =                '警告：副本内队伍职业搭配不符合要求，将在 %d 秒后结束副本';
  SMirrorMapMsg1 =            '副本地图剩余时间 %.2d:%.2d:%.2d';        // 镜像改为副本  
  STimeMapMsg1 =              '地图剩余时间 %.2d:%.2d:%.2d';        // 镜像改为副本

  SInputGuildName =           '请输入行会名称.';
  SInputInfo =                '输入信息.';
  SInputEmpty =               '信息不能为空！';

  SInputNumFilter =           '输入数据中包含了非法符号，请重新输入！';
  SInputNumOutRange =         '输入数字范围必须在0到21亿之间，请重新输入！';

  SImportItemTitle =          '导入物品数据';
  SExportItemTitle =          '导出物品数据';
  SExportItemFileName =       'ItemData.txt';
  SImportItemOK =             '导入物品数据成功.';
  SExportItemOK =             '导出物品数据成功.';

  SCustomItemType =           '自定类';

  SMonNameEmpty =             '怪物名不能为空';
  SAddMonNameExists =         '怪物名已存在，添加失败';
  SEditMonNameExists =        '怪物名已存在，修改失败';

  SBossNameEmpty =            'BOSS名不能为空';
  SAddBossNameExists =        'BOSS名已存在，添加失败';
  SEditBossNameExists =       'BOSS名已存在，修改失败';

  SItemUseNoMsg =             '你的%s已使用完';
  SHeroItemUseNoMsg =         '你英雄的%s已使用完';

  SMPItemNoMsg =              '你的魔法药已使用完';
  SHeroMPItemNoMsg =          '你英雄的魔法药已使用完';

  SHPItemNoMsg =              '你的金创药已使用完';
  SHeroHPItemNoMsg =          '你英雄的金创药已使用完';

  SSpecialHPItemNoMsg =       '你的特殊体力药已使用完';
  SHeroSpecialHPItemNoMsg =   '你英雄的特殊体力药已使用完';

  SSpecialMPItemNoMsg =       '你的特殊魔法药已使用完';
  SHeroSpecialMPItemNoMsg =   '你英雄的特殊魔法药已使用完';

  SItemDuraWarning =          '你的[%s]持久已到%d，请及时修理或更换！';
  SHeroItemDuraWarning =      '英雄的[%s]持久已到%d，请及时修理或更换！';
  SItemDuraTooLow =           '%s 持久过低';

  SRestoreDefSettingAsk =     '你想恢复成系统默认设置吗 ?';
  SBindItemFileName =         '-BindItemList.txt';

  SMagicUpgrateHint =         '(点击技能按钮强化技能)';
  SMagicSetKeyHint =          '(点击技能按钮设置快捷键)';

  SUpgradeItemSucces =        '[成功] 装备升级成功！';
  SUpgradeItemFail1 =         '[失败] 装备升级失败,装备已破碎！';
  SUpgradeItemFail2 =         '[失败] 装备升级失败！';
  SUpgradeItemFail3 =         '[失败] 请在第一个格子里放入您要升级的装备！';
  SUpgradeItemFail4 =         '[失败] 请在第二个格子里放入升级宝石！';
  SUpgradeItemFail5 =         '[失败] 该装备禁止升级！';
  SUpgradeItemFail6 =         '[失败] 该装备无法升级！';
  SUpgradeItemFail7 =         '[失败] 第二个格子放的不是升级宝石！';
  SUpgradeItemFail8 =         '[失败] 第三个格子放的不是附加宝石！';


  function DecodeResStr(S: string): string;

implementation

function DecodeResStr(S: string): string;
begin
  Result := S;
end;

end.
