unit Actor;

interface

uses
  Windows,
  Messages,
  SysUtils,
  Classes,
  Graphics,
  Controls,
  HUtil32,
  DxCanvas,
  HGECanvas,
  Grobal2,
  HGE,
  magiceff,
  GameImages,
  ClFunc,
  SDK,                                         
  Math,
  HGEFontEx,
  MirConfigDlg,
  DxMemo,
  Forms,
  MMSystem;

const
  MAXACTORSOUND = 3;
  CMMX = 150;
  CMMY = 200;

  MIN_ATTACK_FRAME_TIME = 16; // 服务器处理间隔是100，17 * 6 = 112毫秒
  MIN_SPELL_FRAME_TIME = 25;

  HUMANFRAME = 600;
  MONFRAME = 280;
  EXPMONFRAME = 360;
  SCULMONFRAME = 440;
  ZOMBIFRAME = 430;
  MERCHANTFRAME = 60;
  MAXSAY = 5;
  // MON1_FRAME =
  // MON2_FRAME =

  RUN_MINHEALTH = 10;
  DEFSPELLFRAME = 10;
  FIREHIT_READYFRAME = 6;
  MAGBUBBLEBASE = 3890; // 魔法盾效果图位置
  MAGBUBBLESTRUCKBASE = 3900; // 被攻击时魔法盾效果图位置
  MAXWPEFFECTFRAME = 5;
  WPEFFECTBASE = 3750;
  // EffectBase = 0;

type
  // 注意，这里加了数量下面 THealthNumberArray 的数量也要同步 数量 = 1 + 1 + 1 + 1 + 10 * 60

  TNumberType = (tHP, tMP, tGreen, tMiss, tTextHP, tBlastHP, tFatalBlow1, tFatalBlow2, tFatalBlow3, tFatalBlow4);
  //tFata1Blow1:暴击
  //tFata1Blow2:会心一击
  //tFata1Blow3:卓越
  //tFata1Blow4: 致命

  //HZQ 20230816 增加新的绘制效果选项
  TNumberDrawStyle = (ndsNormal, ndsFastExit, ndsMovingFadeOut, ndsStayFadeOut);
  //ndsNormal           无效果，退场条件 nOffsetY >=50
  //ndsFastExit         快速退场，退场高度 nOffsetY >= 20
  //ndsMovingFadeOut    移动渐隐退场， 边上升，边消隐，退场条件 nOffsetY >=50
  //ndsStayFadeOut      停留渐隐屠场， 上升到，固定位置后，渐隐，退场条件 byAlpha <= 32;

  //tHP和tTextHP的区别，tHp后来的飘血会顶掉先来人，tTextHP可以同屏10组存在

const
  //NumberTypeCounts 定义了在同屏的化最多可以出现多少个飘血
  NumberTypeIndexs:array[TNumberType] of Integer = (0, 1, 2, 3, 4, 14, 24, 34, 44, 54); //定义了某种类型(TNumberType)在数组中的起始位置
  NumberTypeCounts:array[TNumberType] of Integer = (1, 1, 1, 1, 10, 10, 10, 10, 10, 10); //定义了某种类型(TNumberType)，在数组中的数量


type
  THealthNumber = record
    sNumber:string;
    nNumber:Integer;
    nOffsetX:Integer;
    nOffsetY:Integer;
    nWidth:Integer;
    nHeight:Integer;
    dwUpdateHPTick:LongWord;
    dwStartHPTick:LongWord;
    nResID:Integer; //HZQ 20230609 增加自定义资源字段
    nResStartIdx:Integer; //资源起始索引
    nNmType:TNumberType; //优化结构，减少程序中的判断
    nDrawStyle:TNumberDrawStyle; //新增绘制样式
    byAlpha:Byte; //每次绘制时的Alpha值
    ImageIndexs:array of Integer;
  end;
  pTHealthNumber = ^THealthNumber;
  THealthNumberArray = array[0..64] of THealthNumber;

  TCustomMagicStatusEffect = record
    boShow:Boolean;
    Status1_File:Smallint;
    Status1_StartIndex:Word;
    Status1_PlayCount:Word;
    Status1_EmptyCount:Word;
    //Status1_PlayTime: Word;
    Status1_DrawMode:TCustomDrawMode;
    Status1_CalcDir:Boolean;

    Status2_File:Smallint;
    Status2_StartIndex:Word;
    Status2_PlayCount:Word;
    Status2_EmptyCount:Word;
    //Status2_PlayTime: Word;
    Status2_DrawMode:TCustomDrawMode;
    Status2_CalcDir:Boolean;

    m_nGenAniTick:LongWord;
    m_nGenAniIndex:Integer;
    m_nStruck:Integer;
  end;

  // 脚本播放
  TClientActorEffect = record
    nEffectFileIndex:SmallInt; // WIL资源编号
    nEffectImageOffSet:Integer; // 开始图片号
    wEffectImageCount:Word; // 播放图片数
    wEffectFrameTime:Word; // 播放速度 毫秒
    nOldLoopCount:Integer;
    nLoopCount:Integer;
    nOldCurrentFrame:Integer;
    nCurrentFrame:Integer;
    dwEffectTick:LongWord;
    Texture:TObject;
    nX, nY:Integer;
    btDrawOrder:Byte; // 是否在角色下层绘制 chongchong 2014-09-12

    nOffsetX:Integer; // 偏移X chongchong 2014-11-03
    nOffsetY:Integer; // 偏移Y chongchong 2014-11-03

    boBlendMode:Boolean;

    boWantDelete:Boolean;
  end;
  pTClientActorEffect = ^TClientActorEffect;

  TClientSelfPlay = record
    SelfPlay:TSelfKeepPlay;
    SelfKeep_Index:Integer;
    SelfKeep_StartTime:LongWord;
    SelfKeep_LastTick:LongWord;
    Images:TObject;
  end;

  // 聊天结构
  TSayingInfo = record
    Text:string;
    TextSurface:TImageInfo;
  end;
  pTSayingInfo = ^TSayingInfo;

  // 角色配置 -- 指图片库中的描述  2013-6-16
  TActionInfo = packed record
    start:Integer;
    frame:Word;
    skip:Word;
    ftime:Word;
    usetick:Word;
  end;
  pTActionInfo = ^TActionInfo;

  TActorIconIndex = record
    dwTime:Longword;
    dwTick:Longword;
    nOCurrentFrame:Integer;
    nCurrentFrame:Integer;
    nEndFrame:Integer;
    Texture:TObject;
    dwLoadTick:LongWord;
    nX:Integer;
    nY:Integer;
    DefTextureWidth:Integer;
  end;
  pTActorIconIndex = ^TActorIconIndex;

  TActorIconIndexArray = array[0..9] of TActorIconIndex;

  // 人物动作 -- piaoyun 2013-6-16
  THumanAction = packed record
    ActStand:TActionInfo; // 1
    ActWalk:TActionInfo; // 8
    ActRun:TActionInfo; // 8
    ActRushLeft:TActionInfo;
    ActRushRight:TActionInfo;
    ActWarMode:TActionInfo; // 1
    ActHit:TActionInfo; // 6
    ActHeavyHit:TActionInfo; // 6
    ActBigHit:TActionInfo; // 6
    ActFireHitReady:TActionInfo; // 6
    ActSpell:TActionInfo; // 6
    ActSitdown:TActionInfo; // 1
    ActStruck:TActionInfo; // 3
    ActDie:TActionInfo; // 4
    ActContinuousHits:array[0..18 - 1] of TActionInfo; // 连击
  end;
  pTHumanAction = ^THumanAction;

  // 怪物动作 -- piaoyun 2013-6-16
  TMonsterAction = packed record
    ActStand:TActionInfo; // 1
    ActWalk:TActionInfo; // 8
    ActRun:TActionInfo;
    ActAttack:TActionInfo; // 6
    ActCritical:TActionInfo; // 6
    ActStruck:TActionInfo; // 3
    ActDie:TActionInfo; // 4
    ActDeath:TActionInfo;
    ActAttack2:TActionInfo;
  end;
  pTMonsterAction = ^TMonsterAction;

const
  // var
    { TODO -opiaoyun -c注释 : 精灵动作帧配置 【2013-6-16】 }
  HA:THumanAction = (
    ActStand:(start:0; frame:4; skip:4; ftime:200; usetick:0);
    ActWalk:(start:64; frame:6; skip:2; ftime:80; usetick:2);
    ActRun:(start:128; frame:6; skip:2; ftime:100; usetick:3);
    ActRushLeft:(start:128; frame:3; skip:5; ftime:100; usetick:3);
    ActRushRight:(start:131; frame:3; skip:5; ftime:100; usetick:3);
    ActWarMode:(start:192; frame:1; skip:0; ftime:200; usetick:0);
    // ActHit:    (start: 200;    frame: 5;  skip: 3;  ftime: 140;  usetick: 0);
    ActHit:(start:200; frame:6; skip:2; ftime:80; usetick:0);
    ActHeavyHit:(start:264; frame:6; skip:2; ftime:80; usetick:0);
    ActBigHit:(start:328; frame:8; skip:0; ftime:80; usetick:0); // 原来是60 chongchong 2016-11-29
    ActFireHitReady:(start:192; frame:6; skip:4; ftime:60; usetick:0);
    ActSpell:(start:392; frame:6; skip:2; ftime:50; usetick:0);
    ActSitdown:(start:456; frame:2; skip:0; ftime:300; usetick:0);
    ActStruck:(start:472; frame:3; skip:5; ftime:80; usetick:0);
    ActDie:(start:536; frame:4; skip:4; ftime:120; usetick:0);

    ActContinuousHits:
    (
    (start:0; frame:6; skip:4; ftime:80; usetick:0), // 0
    (start:80; frame:8; skip:2; ftime:80; usetick:0), // 1   // 追心刺
    (start:160; frame:15; skip:5; ftime:80; usetick:0), // 2   // 三绝杀
    (start:320; frame:6; skip:4; ftime:100; usetick:0), // 3   // 断岳斩
    (start:400; frame:13; skip:7; ftime:80; usetick:0), // 4   // 倚天劈地
    (start:560; frame:10; skip:0; ftime:80; usetick:0), // 5   // 横扫千军
    (start:640; frame:6; skip:4; ftime:80; usetick:0), // 6   // 凤舞祭
    (start:720; frame:6; skip:4; ftime:80; usetick:0), // 7
    (start:800; frame:8; skip:2; ftime:80; usetick:0), // 8   // 冰天雪地
    (start:880; frame:10; skip:0; ftime:80; usetick:0), // 9
    (start:960; frame:10; skip:0; ftime:80; usetick:0), // 10
    (start:1040; frame:13; skip:7; ftime:60; usetick:0), // 11  // 双龙破
    (start:1200; frame:6; skip:4; ftime:80; usetick:0), // 12  // 虎啸诀
    (start:1280; frame:6; skip:4; ftime:80; usetick:0), // 13
    (start:1360; frame:9; skip:1; ftime:80; usetick:0), // 14  // 惊雷爆
    (start:1440; frame:12; skip:8; ftime:60; usetick:0), // 15  // 八卦掌
    (start:1600; frame:12; skip:8; ftime:60; usetick:0), // 16  // 三焰咒
    (start:1760; frame:14; skip:6; ftime:60; usetick:0) // 17  // 万剑归宗
    )
    );
  MA9:TMonsterAction = (
    ActStand:(start:0; frame:1; skip:7; ftime:200; usetick:0);
    ActWalk:(start:64; frame:6; skip:2; ftime:120; usetick:3);
    ActAttack:(start:64; frame:6; skip:2; ftime:150; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:64; frame:6; skip:2; ftime:100; usetick:0);
    ActDie:(start:0; frame:1; skip:7; ftime:140; usetick:0);
    ActDeath:(start:0; frame:1; skip:7; ftime:0; usetick:0);
    );
  MA10:TMonsterAction = (// (8Frame) 带刀卫士
    ActStand:(start:0; frame:4; skip:4; ftime:200; usetick:0);
    ActWalk:(start:64; frame:6; skip:2; ftime:120; usetick:3);
    ActAttack:(start:128; frame:4; skip:4; ftime:150; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:192; frame:2; skip:0; ftime:100; usetick:0);
    ActDie:(start:208; frame:4; skip:4; ftime:140; usetick:0);
    ActDeath:(start:272; frame:1; skip:0; ftime:0; usetick:0);
    );
  MA11:TMonsterAction = (
    ActStand:(start:0; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:80; frame:6; skip:4; ftime:120; usetick:3);
    ActAttack:(start:160; frame:6; skip:4; ftime:100; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:240; frame:2; skip:0; ftime:100; usetick:0);
    ActDie:(start:260; frame:10; skip:0; ftime:140; usetick:0);
    ActDeath:(start:340; frame:1; skip:0; ftime:0; usetick:0);
    );
  MA12:TMonsterAction = (
    ActStand:(start:0; frame:4; skip:4; ftime:200; usetick:0);
    ActWalk:(start:64; frame:6; skip:2; ftime:120; usetick:3);
    ActAttack:(start:128; frame:6; skip:2; ftime:150; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:192; frame:2; skip:0; ftime:150; usetick:0);
    ActDie:(start:208; frame:4; skip:4; ftime:160; usetick:0);
    ActDeath:(start:272; frame:1; skip:0; ftime:0; usetick:0);
    );
  MA13:TMonsterAction = (
    ActStand:(start:0; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:10; frame:8; skip:2; ftime:160; usetick:0);
    ActAttack:(start:30; frame:6; skip:4; ftime:120; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:110; frame:2; skip:0; ftime:100; usetick:0);
    ActDie:(start:130; frame:10; skip:0; ftime:120; usetick:0);
    ActDeath:(start:20; frame:9; skip:0; ftime:150; usetick:0);
    );
  MA14:TMonsterAction = (
    ActStand:(start:0; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:80; frame:6; skip:4; ftime:160; usetick:3);
    ActAttack:(start:160; frame:6; skip:4; ftime:100; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:240; frame:2; skip:0; ftime:100; usetick:0);
    ActDie:(start:260; frame:10; skip:0; ftime:120; usetick:0);
    ActDeath:(start:340; frame:10; skip:0; ftime:100; usetick:0);
    );
  MA15:TMonsterAction = (
    ActStand:(start:0; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:80; frame:6; skip:4; ftime:160; usetick:3);
    ActAttack:(start:160; frame:6; skip:4; ftime:100; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:240; frame:2; skip:0; ftime:100; usetick:0);
    ActDie:(start:260; frame:10; skip:0; ftime:120; usetick:0);
    ActDeath:(start:1; frame:1; skip:0; ftime:100; usetick:0);
    );
  MA16:TMonsterAction = (
    ActStand:(start:0; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:80; frame:6; skip:4; ftime:160; usetick:3);
    ActAttack:(start:160; frame:6; skip:4; ftime:160; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:240; frame:2; skip:0; ftime:100; usetick:0);
    ActDie:(start:260; frame:4; skip:6; ftime:160; usetick:0);
    ActDeath:(start:0; frame:1; skip:0; ftime:160; usetick:0);
    );
  MA17:TMonsterAction = (
    ActStand:(start:0; frame:4; skip:6; ftime:60; usetick:0);
    ActWalk:(start:80; frame:6; skip:4; ftime:160; usetick:3);
    ActAttack:(start:160; frame:6; skip:4; ftime:100; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:240; frame:2; skip:0; ftime:100; usetick:0);
    ActDie:(start:260; frame:10; skip:0; ftime:100; usetick:0);
    ActDeath:(start:340; frame:1; skip:0; ftime:140; usetick:0);
    );
  MA19:TMonsterAction = (
    ActStand:(start:0; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:80; frame:6; skip:4; ftime:160; usetick:3);
    ActAttack:(start:160; frame:6; skip:4; ftime:100; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:240; frame:2; skip:0; ftime:100; usetick:0);
    ActDie:(start:260; frame:10; skip:0; ftime:140; usetick:0);
    ActDeath:(start:340; frame:1; skip:0; ftime:140; usetick:0);
    );
  MA20:TMonsterAction = (
    ActStand:(start:0; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:80; frame:6; skip:4; ftime:160; usetick:3);
    ActAttack:(start:160; frame:6; skip:4; ftime:120; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:240; frame:2; skip:0; ftime:100; usetick:0);
    ActDie:(start:260; frame:10; skip:0; ftime:100; usetick:0);
    ActDeath:(start:340; frame:10; skip:0; ftime:170; usetick:0);
    );
  MA21:TMonsterAction = (
    ActStand:(start:0; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActAttack:(start:10; frame:6; skip:4; ftime:120; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:20; frame:2; skip:0; ftime:100; usetick:0);
    ActDie:(start:30; frame:10; skip:0; ftime:160; usetick:0);
    ActDeath:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    );
  MA22:TMonsterAction = (
    ActStand:(start:80; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:160; frame:6; skip:4; ftime:160; usetick:3);
    ActAttack:(start:240; frame:6; skip:4; ftime:100; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:320; frame:2; skip:0; ftime:100; usetick:0);
    ActDie:(start:340; frame:10; skip:0; ftime:160; usetick:0);
    ActDeath:(start:0; frame:6; skip:4; ftime:170; usetick:0);
    );
  MA23:TMonsterAction = (
    ActStand:(start:20; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:100; frame:6; skip:4; ftime:160; usetick:3);
    ActAttack:(start:180; frame:6; skip:4; ftime:100; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:260; frame:2; skip:0; ftime:100; usetick:0);
    ActDie:(start:280; frame:10; skip:0; ftime:160; usetick:0);
    ActDeath:(start:0; frame:20; skip:0; ftime:100; usetick:0);
    );
  MA24:TMonsterAction = (
    ActStand:(start:0; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:80; frame:6; skip:4; ftime:160; usetick:3);
    ActAttack:(start:160; frame:6; skip:4; ftime:100; usetick:0);
    ActCritical:(start:240; frame:6; skip:4; ftime:100; usetick:0);
    ActStruck:(start:320; frame:2; skip:0; ftime:100; usetick:0);
    ActDie:(start:340; frame:10; skip:0; ftime:140; usetick:0);
    ActDeath:(start:420; frame:1; skip:0; ftime:140; usetick:0);
    );

  MA25:TMonsterAction = (
    ActStand:(start:0; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:70; frame:10; skip:0; ftime:200; usetick:3);
    ActAttack:(start:20; frame:6; skip:4; ftime:120; usetick:0);
    ActCritical:(start:10; frame:6; skip:4; ftime:120; usetick:0);
    ActStruck:(start:50; frame:2; skip:0; ftime:100; usetick:0);
    ActDie:(start:60; frame:10; skip:0; ftime:200; usetick:0);
    ActDeath:(start:80; frame:10; skip:0; ftime:200; usetick:3);
    );

  MA26:TMonsterAction = (
    ActStand:(start:0; frame:1; skip:7; ftime:200; usetick:0);
    ActWalk:(start:0; frame:0; skip:0; ftime:160; usetick:0);
    ActAttack:(start:56; frame:6; skip:2; ftime:500; usetick:0);
    ActCritical:(start:64; frame:6; skip:2; ftime:500; usetick:0);
    ActStruck:(start:0; frame:4; skip:4; ftime:100; usetick:0);
    ActDie:(start:24; frame:10; skip:0; ftime:120; usetick:0);
    ActDeath:(start:0; frame:0; skip:0; ftime:150; usetick:0);
    );
  MA27:TMonsterAction = (
    ActStand:(start:0; frame:1; skip:7; ftime:200; usetick:0);
    ActWalk:(start:0; frame:0; skip:0; ftime:160; usetick:0);
    ActAttack:(start:0; frame:0; skip:0; ftime:250; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:250; usetick:0);
    ActStruck:(start:0; frame:0; skip:0; ftime:100; usetick:0);
    ActDie:(start:0; frame:10; skip:0; ftime:120; usetick:0);
    ActDeath:(start:0; frame:0; skip:0; ftime:150; usetick:0);
    );
  MA28:TMonsterAction = (
    ActStand:(start:80; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:160; frame:6; skip:4; ftime:160; usetick:3);
    ActAttack:(start:0; frame:6; skip:4; ftime:100; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:240; frame:2; skip:0; ftime:100; usetick:0);
    ActDie:(start:260; frame:10; skip:0; ftime:120; usetick:0);
    ActDeath:(start:0; frame:10; skip:0; ftime:100; usetick:0);
    );
  MA29:TMonsterAction = (
    ActStand:(start:80; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:160; frame:6; skip:4; ftime:160; usetick:3);
    ActAttack:(start:240; frame:6; skip:4; ftime:100; usetick:0);
    ActCritical:(start:0; frame:10; skip:0; ftime:100; usetick:0);
    ActStruck:(start:320; frame:2; skip:0; ftime:100; usetick:0);
    ActDie:(start:340; frame:10; skip:0; ftime:120; usetick:0);
    ActDeath:(start:0; frame:10; skip:0; ftime:100; usetick:0);
    );
  MA30:TMonsterAction = (
    ActStand:(start:0; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:0; frame:10; skip:0; ftime:200; usetick:3);
    ActAttack:(start:10; frame:6; skip:4; ftime:120; usetick:0);
    ActCritical:(start:10; frame:6; skip:4; ftime:120; usetick:0);
    ActStruck:(start:20; frame:2; skip:0; ftime:100; usetick:0);
    ActDie:(start:30; frame:20; skip:0; ftime:150; usetick:0);
    ActDeath:(start:0; frame:10; skip:0; ftime:200; usetick:3);
    );
  MA31:TMonsterAction = (
    ActStand:(start:0; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:0; frame:10; skip:0; ftime:200; usetick:3);
    ActAttack:(start:10; frame:6; skip:4; ftime:120; usetick:0);
    ActCritical:(start:0; frame:6; skip:4; ftime:120; usetick:0);
    ActStruck:(start:0; frame:2; skip:8; ftime:100; usetick:0);
    ActDie:(start:20; frame:10; skip:0; ftime:200; usetick:0);
    ActDeath:(start:0; frame:10; skip:0; ftime:200; usetick:3);
    );

  MA32:TMonsterAction = (
    ActStand:(start:0; frame:1; skip:9; ftime:200; usetick:0);
    ActWalk:(start:0; frame:6; skip:4; ftime:200; usetick:3);
    ActAttack:(start:0; frame:6; skip:4; ftime:120; usetick:0);
    ActCritical:(start:0; frame:6; skip:4; ftime:120; usetick:0);
    ActStruck:(start:0; frame:2; skip:8; ftime:100; usetick:0);
    ActDie:(start:80; frame:10; skip:0; ftime:80; usetick:0);
    ActDeath:(start:80; frame:10; skip:0; ftime:200; usetick:3);
    );

  MA33:TMonsterAction = (
    ActStand:(start:0; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:80; frame:6; skip:4; ftime:200; usetick:3);
    ActAttack:(start:160; frame:6; skip:4; ftime:120; usetick:0);
    ActCritical:(start:340; frame:6; skip:4; ftime:120; usetick:0);
    ActStruck:(start:240; frame:2; skip:0; ftime:100; usetick:0);
    ActDie:(start:260; frame:10; skip:0; ftime:200; usetick:0);
    ActDeath:(start:260; frame:10; skip:0; ftime:200; usetick:0);
    );

  MA34:TMonsterAction = (
    ActStand:(start:0; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:80; frame:6; skip:4; ftime:200; usetick:3);
    ActAttack:(start:160; frame:6; skip:4; ftime:120; usetick:0);
    ActCritical:(start:320; frame:6; skip:4; ftime:120; usetick:0);
    ActStruck:(start:400; frame:2; skip:0; ftime:100; usetick:0);
    ActDie:(start:420; frame:20; skip:0; ftime:200; usetick:0);
    ActDeath:(start:420; frame:20; skip:0; ftime:200; usetick:0);
    );

  MA35:TMonsterAction = (
    ActStand:(start:0; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActAttack:(start:30; frame:10; skip:0; ftime:150; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:0; frame:1; skip:9; ftime:0; usetick:0);
    ActDie:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActDeath:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    );

  MA36:TMonsterAction = (
    ActStand:(start:0; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActAttack:(start:30; frame:20; skip:0; ftime:150; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:0; frame:1; skip:9; ftime:0; usetick:0);
    ActDie:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActDeath:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    );

  MA37:TMonsterAction = (
    ActStand:(start:30; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActAttack:(start:30; frame:4; skip:6; ftime:150; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:0; frame:1; skip:9; ftime:0; usetick:0);
    ActDie:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActDeath:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    );

  MA38:TMonsterAction = (
    ActStand:(start:0; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActAttack:(start:80; frame:6; skip:4; ftime:150; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActDie:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActDeath:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    );

  MA39:TMonsterAction = (
    ActStand:(start:0; frame:4; skip:6; ftime:300; usetick:0);
    ActWalk:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActAttack:(start:10; frame:6; skip:4; ftime:150; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:20; frame:2; skip:0; ftime:150; usetick:0);
    ActDie:(start:30; frame:10; skip:0; ftime:80; usetick:0);
    ActDeath:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    );

  MA40:TMonsterAction = (
    ActStand:(start:0; frame:4; skip:6; ftime:250; usetick:0);
    ActWalk:(start:80; frame:6; skip:4; ftime:210; usetick:3);
    ActAttack:(start:160; frame:6; skip:4; ftime:110; usetick:0);
    ActCritical:(start:580; frame:20; skip:0; ftime:80; usetick:0);
    ActStruck:(start:240; frame:2; skip:0; ftime:120; usetick:0);
    ActDie:(start:260; frame:20; skip:0; ftime:130; usetick:0);
    ActDeath:(start:260; frame:20; skip:0; ftime:130; usetick:0);
    );

  MA41:TMonsterAction = (
    ActStand:(start:0; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActAttack:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActDie:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActDeath:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    );

  MA42:TMonsterAction = (
    ActStand:(start:0; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:10; frame:8; skip:2; ftime:160; usetick:0);
    ActAttack:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActDie:(start:30; frame:10; skip:0; ftime:120; usetick:0);
    ActDeath:(start:30; frame:10; skip:0; ftime:150; usetick:0);
    );

  MA43:TMonsterAction = (
    ActStand:(start:0; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:80; frame:6; skip:4; ftime:160; usetick:0);
    ActAttack:(start:160; frame:6; skip:4; ftime:160; usetick:0);
    ActCritical:(start:160; frame:6; skip:4; ftime:160; usetick:0);
    ActStruck:(start:240; frame:2; skip:0; ftime:150; usetick:0);
    ActDie:(start:260; frame:10; skip:0; ftime:120; usetick:0);
    ActDeath:(start:340; frame:10; skip:0; ftime:100; usetick:0);
    );

  MA44:TMonsterAction = (
    ActStand:(start:0; frame:10; skip:0; ftime:300; usetick:0);
    ActWalk:(start:10; frame:6; skip:4; ftime:150; usetick:0);
    ActAttack:(start:20; frame:6; skip:4; ftime:150; usetick:0);
    ActCritical:(start:40; frame:10; skip:0; ftime:150; usetick:0);
    ActStruck:(start:40; frame:2; skip:8; ftime:150; usetick:0);
    ActDie:(start:30; frame:6; skip:4; ftime:150; usetick:0);
    ActDeath:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    );

  MA45:TMonsterAction = (
    ActStand:(start:0; frame:10; skip:0; ftime:300; usetick:0);
    ActWalk:(start:0; frame:10; skip:0; ftime:300; usetick:0);
    ActAttack:(start:10; frame:10; skip:0; ftime:300; usetick:0);
    ActCritical:(start:10; frame:10; skip:0; ftime:100; usetick:0);
    ActStruck:(start:0; frame:1; skip:9; ftime:300; usetick:0);
    ActDie:(start:0; frame:1; skip:9; ftime:300; usetick:0);
    ActDeath:(start:0; frame:1; skip:9; ftime:300; usetick:0);
    );

  MA46:TMonsterAction = (
    ActStand:(start:0; frame:20; skip:0; ftime:100; usetick:0);
    ActWalk:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActAttack:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActDie:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActDeath:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    );
  MA47:TMonsterAction = (// 嗜血教主
    ActStand:(start:0; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:80; frame:6; skip:4; ftime:200; usetick:3);
    ActAttack:(start:160; frame:6; skip:4; ftime:120; usetick:0);
    ActCritical:(start:260; frame:6; skip:4; ftime:120; usetick:0);
    ActStruck:(start:240; frame:2; skip:0; ftime:100; usetick:0);
    ActDie:(start:524; frame:6; skip:0; ftime:200; usetick:0);
    ActDeath:(start:524; frame:6; skip:0; ftime:200; usetick:0);
    );

  MA54:TMonsterAction = (
    ActStand:(start:0; frame:10; skip:0; ftime:200; usetick:0);
    ActWalk:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActAttack:(start:0; frame:10; skip:0; ftime:200; usetick:0);
    ActCritical:(start:0; frame:10; skip:0; ftime:200; usetick:0);
    ActStruck:(start:0; frame:10; skip:0; ftime:200; usetick:0);
    ActDie:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActDeath:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    );

  MA95:TMonsterAction = (// 火龙守护兽
    ActStand:(start:3; frame:1; skip:0; ftime:0; usetick:0);
    ActWalk:(start:0; frame:0; skip:0; ftime:0; usetick:3);
    ActAttack:(start:8; frame:10; skip:2; ftime:160; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActDie:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActDeath:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    );

  NPC1:TMonsterAction = (// NPC
    ActStand:(start:0; frame:1; skip:0; ftime:100; usetick:0);
    ActWalk:(start:0; frame:1; skip:0; ftime:100; usetick:0);
    ActAttack:(start:0; frame:1; skip:0; ftime:100; usetick:0);
    ActCritical:(start:0; frame:1; skip:0; ftime:100; usetick:0);
    ActStruck:(start:0; frame:1; skip:0; ftime:100; usetick:0);
    ActDie:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActDeath:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    );

  { TODO -opiaoyun -c修复 : 月灵死亡特效问题-参数修改【2013-6-7】 }
    (*MA56: TMonsterAction = (                      // 月灵参数修改后
    ActStand: (start: 0; frame: 4; skip: 6; ftime: 200; usetick: 0);
    ActWalk: (start: 80; frame: 6; skip: 4; ftime: 200; usetick: 3);
    ActAttack: (start: 160; frame: 6; skip: 4; ftime: 160; usetick: 0);
    ActCritical: (start: 0; frame: 0; skip: 0; ftime: 0; usetick: 0);
    ActStruck: (start: 240; frame: 2; skip: 0; ftime: 100; usetick: 0);
    // ActDie: (start: 260; frame: 10; skip: 0; ftime: 200; usetick: 0);
    // ActDeath: (start: 260; frame: 6; skip: 0; ftime: 200; usetick: 0);
    ActDie: (Start: 260; frame: 10; skip: 0; ftime: 200; usetick: 0);
    ActDeath: (Start: 0; frame: 10; skip: 0; ftime: 100; usetick: 0);
    ); *)

  // JS引擎的代码
  MA56:TMonsterAction = (
    ActStand:(Start:0; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(Start:80; frame:6; skip:4; ftime:160; usetick:0);
    ActAttack:(Start:160; frame:4; skip:6; ftime:160; usetick:0);
    ActCritical:(Start:160; frame:6; skip:4; ftime:160; usetick:0);
    ActStruck:(Start:240; frame:2; skip:0; ftime:150; usetick:0);
    ActDie:(Start:260; frame:10; skip:0; ftime:120; usetick:0);
    ActDeath:(Start:0; frame:10; skip:0; ftime:100; usetick:0);
    );

  {MA56: TMonsterAction = (// 月灵 -- 原始
    ActStand: (start: 0; frame: 4; skip: 6; ftime: 200; usetick: 0);
    ActWalk: (start: 80; frame: 6; skip: 4; ftime: 160; usetick: 3); //
    ActAttack: (start: 160; frame: 6; skip: 4; ftime: 100; usetick: 0);
    ActCritical: (start: 0; frame: 0; skip: 0; ftime: 0; usetick: 0);
    ActStruck: (start: 240; frame: 2; skip: 0; ftime: 100; usetick: 0);
    ActDie: (start: 260; frame: 6; skip: 4; ftime: 140; usetick: 0);
    ActDeath: (start: 340; frame: 10; skip: 0; ftime: 140; usetick: 0);
    ); }
  // 圣殿护卫
  MA102:TMonsterAction = (
    ActStand:(start:0; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:80; frame:6; skip:4; ftime:160; usetick:3);
    ActAttack:(start:160; frame:6; skip:4; ftime:100; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:240; frame:2; skip:0; ftime:100; usetick:0);
    ActDie:(start:260; frame:10; skip:0; ftime:140; usetick:0);
    ActDeath:(start:420; frame:6; skip:4; ftime:170; usetick:0);
    );

  MA103:TMonsterAction = (
    ActStand:(start:0; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:80; frame:6; skip:4; ftime:200; usetick:3);
    ActAttack:(start:160; frame:6; skip:4; ftime:120; usetick:0);
    ActCritical:(start:340; frame:10; skip:0; ftime:120; usetick:0);
    ActStruck:(start:240; frame:2; skip:0; ftime:100; usetick:0);
    ActDie:(start:260; frame:10; skip:0; ftime:200; usetick:0);
    ActDeath:(start:260; frame:10; skip:0; ftime:200; usetick:0);
    );

  MA104:TMonsterAction = (
    ActStand:(start:0; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:80; frame:6; skip:4; ftime:200; usetick:3);
    ActAttack:(start:160; frame:6; skip:4; ftime:120; usetick:0);
    ActCritical:(start:340; frame:7; skip:3; ftime:120; usetick:0);
    ActStruck:(start:240; frame:2; skip:0; ftime:100; usetick:0);
    ActDie:(start:260; frame:10; skip:0; ftime:200; usetick:0);
    ActDeath:(start:260; frame:10; skip:0; ftime:200; usetick:0);
    );

  MA105:TMonsterAction = (
    ActStand:(start:0; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:80; frame:6; skip:4; ftime:200; usetick:3);
    ActAttack:(start:160; frame:6; skip:4; ftime:120; usetick:0);
    ActCritical:(start:340; frame:6; skip:4; ftime:120; usetick:0);
    ActStruck:(start:240; frame:2; skip:0; ftime:100; usetick:0);
    ActDie:(start:260; frame:9; skip:1; ftime:200; usetick:0);
    ActDeath:(start:260; frame:9; skip:1; ftime:200; usetick:0);
    );

  MA106:TMonsterAction = (
    ActStand:(start:0; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:80; frame:6; skip:4; ftime:200; usetick:3);
    ActAttack:(start:160; frame:6; skip:4; ftime:120; usetick:0);
    ActCritical:(start:340; frame:8; skip:2; ftime:120; usetick:0);
    ActStruck:(start:240; frame:2; skip:0; ftime:100; usetick:0);
    ActDie:(start:260; frame:10; skip:0; ftime:200; usetick:0);
    ActDeath:(start:260; frame:10; skip:0; ftime:200; usetick:0);
    ActAttack2:(start:420; frame:9; skip:1; ftime:120; usetick:0);
    );

  MA107:TMonsterAction = (// 雪域卫士 冰峰效果
    ActStand:(start:0; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:80; frame:6; skip:4; ftime:200; usetick:3);
    ActAttack:(start:160; frame:6; skip:4; ftime:120; usetick:0);
    ActCritical:(start:820; frame:10; skip:0; ftime:200; usetick:0);
    ActStruck:(start:240; frame:2; skip:0; ftime:100; usetick:0);
    ActDie:(start:260; frame:10; skip:0; ftime:200; usetick:0);
    ActDeath:(start:740; frame:6; skip:4; ftime:170; usetick:0);
    ActAttack2:(start:340; frame:6; skip:4; ftime:120; usetick:0);
    );

  MA108:TMonsterAction = (
    ActStand:(start:0; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:0; frame:4; skip:6; ftime:0; usetick:0);
    ActAttack:(start:10; frame:4; skip:6; ftime:120; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:10; frame:2; skip:0; ftime:0; usetick:0);
    ActDie:(start: - 20; frame:10; skip:0; ftime:200; usetick:0);
    ActDeath:(start: - 20; frame:10; skip:0; ftime:200; usetick:0);
    );

  MA109:TMonsterAction = (
    ActStand:(start:0; frame:10; skip:0; ftime:200; usetick:0);
    ActWalk:(start:10; frame:10; skip:0; ftime:160; usetick:3);
    ActAttack:(start:20; frame:10; skip:0; ftime:100; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:30; frame:4; skip:0; ftime:100; usetick:0);
    ActDie:(start:40; frame:10; skip:0; ftime:140; usetick:0);
    ActDeath:(start:40; frame:10; skip:0; ftime:140; usetick:0);
    );
  {
  MA110: TMonsterAction = (
    ActStand: (start: 0; frame: 10; skip: 0; ftime: 200; usetick: 0);
    ActWalk: (start: 10; frame: 10; skip: 0; ftime: 160; usetick: 3);
    ActAttack: (start: 20; frame: 10; skip: 0; ftime: 100; usetick: 0);
    ActCritical: (start: 0; frame: 0; skip: 0; ftime: 0; usetick: 0);
    ActStruck: (start: 0; frame: 2; skip: 0; ftime: 200; usetick: 0);
    ActDie: (start: 400; frame: 18; skip: 0; ftime: 140; usetick: 0);
    ActDeath: (start: 400; frame: 18; skip: 0; ftime: 140; usetick: 0);
    );
  }
  MA110:TMonsterAction = (
    ActStand:(start:0; frame:30; skip:0; ftime:200; usetick:0);
    ActWalk:(start:10; frame:10; skip:0; ftime:160; usetick:3);
    ActAttack:(start:20; frame:10; skip:0; ftime:100; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:0; frame:2; skip:0; ftime:200; usetick:0);
    ActDie:(start:400; frame:18; skip:0; ftime:140; usetick:0);
    ActDeath:(start:400; frame:18; skip:0; ftime:140; usetick:0);
    );

  MA112:TMonsterAction = (
    ActStand:(start:0; frame:10; skip:0; ftime:200; usetick:0);
    ActWalk:(start:10; frame:4; skip:6; ftime:160; usetick:3);
    ActAttack:(start:0; frame:10; skip:0; ftime:100; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:10; frame:4; skip:0; ftime:200; usetick:0);
    ActDie:(start:20; frame:10; skip:0; ftime:140; usetick:0);
    ActDeath:(start:20; frame:10; skip:0; ftime:140; usetick:0);
    );

  // 修复 Mon35-0  狮子死亡特效 piaoyun 2013-11-16
  MA113:TMonsterAction = (
    ActStand:(start:0; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:80; frame:6; skip:4; ftime:200; usetick:3);
    ActRun:(start:340; frame:6; skip:4; ftime:200; usetick:3);
    ActAttack:(start:160; frame:6; skip:4; ftime:120; usetick:0);
    ActCritical:(start:420; frame:10; skip:0; ftime:120; usetick:0);
    ActStruck:(start:240; frame:2; skip:0; ftime:100; usetick:0);
    ActDie:(start:260; frame:8; skip:2; ftime:200; usetick:0);
    ActDeath:(start:260; frame:8; skip:2; ftime:200; usetick:0);
    );

  MA114:TMonsterAction = (
    ActStand:(start:0; frame:10; skip:0; ftime:200; usetick:0);
    ActWalk:(start:80; frame:6; skip:4; ftime:160; usetick:3);
    ActAttack:(start:160; frame:6; skip:4; ftime:100; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:240; frame:2; skip:0; ftime:100; usetick:0);
    ActDie:(start:260; frame:10; skip:0; ftime:140; usetick:0);
    ActDeath:(start:260; frame:10; skip:0; ftime:140; usetick:0);
    );

  MA115:TMonsterAction = (
    ActStand:(start:0; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:80; frame:6; skip:4; ftime:200; usetick:3);
    ActAttack:(start:160; frame:6; skip:4; ftime:120; usetick:0);
    ActCritical:(start:340; frame:6; skip:4; ftime:120; usetick:0);
    ActStruck:(start:240; frame:2; skip:0; ftime:100; usetick:0);
    ActDie:(start:260; frame:8; skip:2; ftime:200; usetick:0);
    ActDeath:(start:260; frame:8; skip:2; ftime:200; usetick:0);
    ActAttack2:(start:420; frame:7; skip:3; ftime:120; usetick:0);
    );

  // Mon27-6 冰柱怪物效果修复 piaoyun 2013-11-15
  MA117:TMonsterAction = (
    ActStand:(start:0; frame:2; skip:8; ftime:200; usetick:0);
    ActWalk:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActAttack:(start:0; frame:2; skip:8; ftime:100; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:10; frame:2; skip:8; ftime:100; usetick:0);
    ActDie:(start:20; frame:8; skip:2; ftime:140; usetick:0);
    ActDeath:(start:20; frame:8; skip:2; ftime:140; usetick:0);
    );

  // 新骷髅测试 动作帧定义 piaoyun 2013-07-27
  MA200:TMonsterAction = (
    ActStand:(start:0; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:80; frame:6; skip:4; ftime:160; usetick:3);
    ActAttack:(start:160; frame:6; skip:4; ftime:100; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:240; frame:2; skip:0; ftime:100; usetick:0);
    ActDie:(start:260; frame:6; skip:4; ftime:120; usetick:0);
    ActDeath:(start:340; frame:5; skip:0; ftime:100; usetick:0);
    );

  MA201:TMonsterAction = (
    ActStand:(start:0; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:80; frame:6; skip:4; ftime:160; usetick:3);
    ActAttack:(start:160; frame:6; skip:4; ftime:100; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:240; frame:2; skip:0; ftime:100; usetick:0);
    ActDie:(start:260; frame:6; skip:4; ftime:120; usetick:0);
    ActDeath:(start:340; frame:5; skip:0; ftime:100; usetick:0);
    );

  MA202:TMonsterAction = (
    ActStand:(start:0; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:80; frame:6; skip:4; ftime:160; usetick:3);
    ActAttack:(start:160; frame:6; skip:4; ftime:100; usetick:0);
    ActCritical:(start:400; frame:10; skip:0; ftime:100; usetick:0);
    ActStruck:(start:240; frame:2; skip:8; ftime:100; usetick:0);
    ActDie:(start:320; frame:10; skip:0; ftime:120; usetick:0);
    ActDeath:(start:320; frame:10; skip:0; ftime:100; usetick:0);
    ActAttack2:(start:400; frame:6; skip:4; ftime:140; usetick:0);
    );

  MA203:TMonsterAction = (
    ActStand:(start:0; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:80; frame:6; skip:4; ftime:160; usetick:3);
    ActAttack:(start:160; frame:8; skip:2; ftime:100; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:240; frame:3; skip:7; ftime:100; usetick:0);
    ActDie:(start:320; frame:9; skip:1; ftime:120; usetick:0);
    ActDeath:(start:320; frame:9; skip:1; ftime:100; usetick:0);
    //ActAttack2: (start: 400; frame: 6; skip: 4; ftime: 140; usetick: 0);
    );

  MA204:TMonsterAction = (
    ActStand:(start:0; frame:9; skip:1; ftime:200; usetick:0);
    ActWalk:(start:80; frame:7; skip:3; ftime:160; usetick:3);
    ActAttack:(start:160; frame:8; skip:2; ftime:100; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:240; frame:10; skip:0; ftime:100; usetick:0);
    ActDie:(start:480; frame:10; skip:0; ftime:120; usetick:0);
    ActDeath:(start:480; frame:10; skip:0; ftime:100; usetick:0);
    //ActAttack2: (start: 400; frame: 6; skip: 4; ftime: 140; usetick: 0);
    );

  MA205:TMonsterAction = (
    ActStand:(start:0; frame:6; skip:4; ftime:200; usetick:0);
    ActWalk:(start:80; frame:6; skip:4; ftime:160; usetick:3);
    ActAttack:(start:160; frame:8; skip:2; ftime:100; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:240; frame:3; skip:7; ftime:100; usetick:0);
    ActDie:(start:320; frame:8; skip:2; ftime:120; usetick:0);
    ActDeath:(start:320; frame:8; skip:2; ftime:100; usetick:0);
    );

  MA206:TMonsterAction = (
    ActStand:(start:0; frame:1; skip:9; ftime:200; usetick:0);
    ActWalk:(start:0; frame:0; skip:0; ftime:0; usetick:3);
    ActAttack:(start:0; frame:1; skip:9; ftime:100; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:0; frame:1; skip:9; ftime:100; usetick:0);
    ActDie:(start:0; frame:6; skip:4; ftime:120; usetick:0);
    ActDeath:(start:0; frame:6; skip:4; ftime:100; usetick:0);
    );

  MA207:TMonsterAction = (
    ActStand:(start:0; frame:5; skip:5; ftime:200; usetick:0);
    ActWalk:(start:80; frame:6; skip:4; ftime:160; usetick:3);
    ActAttack:(start:160; frame:8; skip:2; ftime:100; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:240; frame:3; skip:7; ftime:100; usetick:0);
    ActDie:(start:320; frame:7; skip:3; ftime:120; usetick:0);
    ActDeath:(start:320; frame:7; skip:3; ftime:100; usetick:0);
    );

  MA208:TMonsterAction = (
    ActStand:(start:0; frame:7; skip:3; ftime:200; usetick:0);
    ActWalk:(start:80; frame:6; skip:4; ftime:160; usetick:3);
    ActAttack:(start:160; frame:7; skip:3; ftime:100; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:240; frame:3; skip:7; ftime:100; usetick:0);
    ActDie:(start:320; frame:7; skip:3; ftime:120; usetick:0);
    ActDeath:(start:320; frame:7; skip:3; ftime:100; usetick:0);
    );

  MA209:TMonsterAction = (
    ActStand:(start:0; frame:9; skip:1; ftime:200; usetick:0);
    ActWalk:(start:80; frame:6; skip:4; ftime:160; usetick:3);
    ActAttack:(start:160; frame:7; skip:3; ftime:100; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:240; frame:4; skip:6; ftime:100; usetick:0);
    ActDie:(start:480; frame:7; skip:3; ftime:120; usetick:0);
    ActDeath:(start:480; frame:7; skip:3; ftime:100; usetick:0);
    ActAttack2:(start:320; frame:7; skip:3; ftime:140; usetick:0);
    );

  MA220:TMonsterAction = (
    ActStand:(start:170; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActAttack:(start:250; frame:6; skip:4; ftime:120; usetick:0);
    ActCritical:(start:0; frame:9; skip:1; ftime:140; usetick:0);
    ActStruck:(start:330; frame:2; skip:0; ftime:100; usetick:0);
    ActDie:(start:350; frame:10; skip:0; ftime:120; usetick:0);
    ActDeath:(start:350; frame:10; skip:0; ftime:120; usetick:0);
    );

  MA222:TMonsterAction = (
    ActStand:(start:0; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:80; frame:6; skip:4; ftime:160; usetick:3);
    ActAttack:(start:160; frame:6; skip:4; ftime:100; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:240; frame:2; skip:0; ftime:100; usetick:0);
    ActDie:(start:260; frame:10; skip:0; ftime:140; usetick:0);
    ActDeath:(start:260; frame:10; skip:0; ftime:140; usetick:0);
    ActAttack2:(start:340; frame:6; skip:4; ftime:140; usetick:0);
    );

  MA223:TMonsterAction = (
    ActStand:(start:0; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:80; frame:6; skip:4; ftime:160; usetick:0);
    ActAttack:(start:160; frame:10; skip:0; ftime:100; usetick:0);
    ActCritical:(start:500; frame:8; skip:2; ftime:100; usetick:0); // 这里作为第三个动作使用
    ActStruck:(start:240; frame:2; skip:0; ftime:100; usetick:0);
    ActDie:(start:260; frame:14; skip:6; ftime:140; usetick:0);
    ActDeath:(start:260; frame:14; skip:6; ftime:140; usetick:0);
    ActAttack2:(start:420; frame:10; skip:0; ftime:100; usetick:0);
    );

  MA225:TMonsterAction = (
    ActStand:(start:0; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:80; frame:6; skip:4; ftime:160; usetick:3);
    ActAttack:(start:160; frame:8; skip:2; ftime:100; usetick:0);
    ActCritical:(start:500; frame:8; skip:2; ftime:100; usetick:0); // 这里作为第三个动作使用   -- 第四个动作从 此基础+80 开始
    ActStruck:(start:240; frame:2; skip:0; ftime:100; usetick:0);
    ActDie:(start:260; frame:16; skip:4; ftime:140; usetick:0);
    ActDeath:(start:260; frame:16; skip:4; ftime:140; usetick:0);
    ActAttack2:(start:420; frame:10; skip:0; ftime:100; usetick:0);
    );

  // Mon26-1 - 4 怪物死亡效果修复 piaoyun 2013-11-15
  MA252:TMonsterAction = (
    ActStand:(start:0; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:80; frame:6; skip:4; ftime:160; usetick:3);
    ActAttack:(start:160; frame:6; skip:4; ftime:100; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:240; frame:2; skip:0; ftime:100; usetick:0);
    ActDie:(start:260; frame:8; skip:2; ftime:140; usetick:0);
    ActDeath:(start:260; frame:8; skip:2; ftime:140; usetick:0);
    ActAttack2:(start:400; frame:6; skip:4; ftime:140; usetick:0);
    );

  // 血灵教主 chongchong 2014-09-10
  MA253:TMonsterAction = (
    ActStand:(start:0; frame:1; skip:0; ftime:200; usetick:0);
    ActWalk:(start:0; frame:1; skip:0; ftime:200; usetick:3);
    ActAttack:(start:8; frame:6; skip:0; ftime:100; usetick:0);
    ActCritical:(start:0; frame:1; skip:0; ftime:200; usetick:0);
    ActStruck:(start:0; frame:1; skip:0; ftime:200; usetick:0);
    ActDie:(start:16; frame:5; skip:0; ftime:140; usetick:0);
    ActDeath:(start:16; frame:4; skip:0; ftime:140; usetick:0);
    );

  // 新神兽 Mon41-2, Mon41-3 chongchong 2014-11-21
  MA254:TMonsterAction = (
    ActStand:(start:608 - 352; frame:6; skip:2; ftime:200; usetick:0);
    ActWalk:(start:672 - 352; frame:6; skip:2; ftime:160; usetick:3);
    ActAttack:(start:672 - 352; frame:6; skip:4; ftime:100; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:736 - 352; frame:2; skip:6; ftime:100; usetick:0);
    ActDie:(start:800 - 352; frame:10; skip:0; ftime:120; usetick:0);
    ActDeath:(start:0; frame:10; skip:6; ftime:100; usetick:0);
    );

  // 新神兽 Mon41-2, Mon41-3 chongchong 2014-11-21
  MA255:TMonsterAction = (
    ActStand:(start:880 - 480; frame:6; skip:2; ftime:200; usetick:0);
    ActWalk:(start:944 - 480; frame:6; skip:2; ftime:160; usetick:3);
    ActAttack:(start:1008 - 480; frame:6; skip:2; ftime:100; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:1072 - 480; frame:2; skip:6; ftime:100; usetick:0);
    ActDie:(start:1136 - 480; frame:10; skip:6; ftime:120; usetick:0);
    ActDeath:(start:0; frame:10; skip:6; ftime:100; usetick:0);
    );

  NPC60:TMonsterAction = (// NPC
    ActStand:(start:0; frame:4; skip:6; ftime:100; usetick:0);
    ActWalk:(start:0; frame:4; skip:6; ftime:100; usetick:0);
    ActAttack:(start:0; frame:4; skip:6; ftime:100; usetick:0);
    ActCritical:(start:0; frame:4; skip:6; ftime:100; usetick:0);
    ActStruck:(start:0; frame:4; skip:6; ftime:100; usetick:0);
    ActDie:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActDeath:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    );

  NPC81:TMonsterAction = (// NPC
    ActStand:(start:0; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:0; frame:4; skip:6; ftime:200; usetick:0);
    ActAttack:(start:10; frame:10; skip:0; ftime:150; usetick:0);
    ActCritical:(start:10; frame:10; skip:0; ftime:150; usetick:0);
    ActStruck:(start:10; frame:10; skip:0; ftime:150; usetick:0);
    ActDie:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActDeath:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    );

  NPC84:TMonsterAction = (// NPC
    ActStand:(start:0; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActAttack:(start:4; frame:7; skip:3; ftime:150; usetick:0);
    ActCritical:(start:22; frame:8; skip:2; ftime:0; usetick:0);
    ActStruck:(start:0; frame:1; skip:9; ftime:0; usetick:0);
    ActDie:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActDeath:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    );

  NPC101:TMonsterAction = (// NPC
    ActStand:(start:0; frame:10; skip:0; ftime:200; usetick:0);
    ActWalk:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActAttack:(start:10; frame:10; skip:0; ftime:200; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:200; usetick:0);
    ActStruck:(start:0; frame:0; skip:0; ftime:200; usetick:0);
    ActDie:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActDeath:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    );

  NPC210:TMonsterAction = (// NPC
    ActStand:(start:0; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActAttack:(start:30; frame:10; skip:0; ftime:100; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:200; usetick:0);
    ActStruck:(start:0; frame:0; skip:0; ftime:200; usetick:0);
    ActDie:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActDeath:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    );

  NPC226:TMonsterAction = (
    ActStand:(start:0; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActAttack:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActDie:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActDeath:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    );

  NPC236:TMonsterAction = (
    ActStand:(start:0; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActAttack:(start:30; frame:10; skip:0; ftime:150; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActDie:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActDeath:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    );

  NPC241:TMonsterAction = (
    ActStand:(start:0; frame:2; skip:8; ftime:200; usetick:0);
    ActWalk:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActAttack:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActDie:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActDeath:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    );

  NPC244:TMonsterAction = (
    ActStand:(start:0; frame:10; skip:0; ftime:200; usetick:0);
    ActWalk:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActAttack:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActDie:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActDeath:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    );

  NPC245:TMonsterAction = (
    ActStand:(start:0; frame:3; skip:7; ftime:200; usetick:0);
    ActWalk:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActAttack:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActDie:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActDeath:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    );

  NPC246:TMonsterAction = (
    ActStand:(start:0; frame:6; skip:4; ftime:200; usetick:0);
    ActWalk:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActAttack:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActDie:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActDeath:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    );

  NPC250:TMonsterAction = (
    ActStand:(start:0; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActAttack:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActDie:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActDeath:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    );

  NPC252:TMonsterAction = (
    ActStand:(start:0; frame:6; skip:4; ftime:200; usetick:0);
    ActWalk:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActAttack:(start:80; frame:10; skip:0; ftime:150; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActDie:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActDeath:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    );

  NPC253:TMonsterAction = (
    ActStand:(start:0; frame:8; skip:2; ftime:200; usetick:0);
    ActWalk:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActAttack:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActDie:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActDeath:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    );

  NPC258:TMonsterAction = (
    ActStand:(start:0; frame:4; skip:6; ftime:200; usetick:0);
    ActWalk:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActAttack:(start:80; frame:10; skip:0; ftime:150; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActDie:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActDeath:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    );

  NPC263:TMonsterAction = (
    ActStand:(start:0; frame:6; skip:4; ftime:200; usetick:0);
    ActWalk:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActAttack:(start:80; frame:10; skip:0; ftime:150; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActDie:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActDeath:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    );

  NPC269:TMonsterAction = (
    ActStand:(start:0; frame:6; skip:4; ftime:200; usetick:0);
    ActWalk:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActAttack:(start:80; frame:6; skip:4; ftime:150; usetick:0);
    ActCritical:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActStruck:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActDie:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    ActDeath:(start:0; frame:0; skip:0; ftime:0; usetick:0);
    );

  WORDER:array[0..1, 0..599] of byte = (
    (
    // 沥瘤
    0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
    1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1,
    0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1,
    // 叭扁
    0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
    1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
    0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1,
    // 顿扁
    0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
    1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 1,
    0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1,
    // war葛靛
    0, 1, 1, 1, 0, 0, 0, 0,
    // 傍拜
    1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0,
    1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0,
    0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 1, 1,
    // 傍拜 2
    0, 1, 1, 0, 0, 0, 1, 1, 0, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0,
    1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1,
    0, 0, 0, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 0, 1, 1,
    // 傍拜3
    1, 1, 0, 1, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0,
    1, 1, 0, 0, 1, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0, 0, 0, 0,
    0, 0, 0, 0, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 0, 0,
    // 付过
    0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 1, 1,
    1, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1,
    0, 0, 1, 1, 0, 0, 1, 1, 0, 0, 0, 1, 0, 0, 1, 1,
    // 澵扁
    0, 0, 1, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 0, 0,
    // 嘎扁
    0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
    1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1,
    0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1,
    // 静矾咙
    0, 0, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
    1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1,
    0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1
    ),

    (
    // 沥瘤
    0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
    1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1,
    0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1,
    // 叭扁
    0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
    1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
    0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1,
    // 顿扁
    0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
    1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 1,
    0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1,
    // war葛靛
    1, 1, 1, 1, 0, 0, 0, 0,
    // 傍拜
    1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0,
    1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0,
    0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 1, 1,
    // 傍拜 2
    0, 1, 1, 0, 0, 0, 1, 1, 0, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0,
    1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1,
    0, 0, 0, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 0, 1, 1,
    // 傍拜3
    1, 1, 0, 1, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0,
    1, 1, 0, 0, 1, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0, 0, 0, 0,
    0, 0, 0, 0, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 0, 0,
    // 付过
    0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 1, 1,
    1, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1,
    0, 0, 1, 1, 0, 0, 1, 1, 0, 0, 0, 1, 0, 0, 1, 1,
    // 澵扁
    0, 0, 1, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 0, 0,
    // 嘎扁
    0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
    1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1,
    0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1,
    // 静矾咙
    0, 0, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
    1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1,
    0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1
    )
    );

  EffDir:array[0..7] of byte = (0, 0, 1, 1, 1, 1, 1, 0);

type
  pTActor = ^TActor;
  TActor = class // Size
    m_nRecogId:Int64; // 角色标识
    m_btDir:byte; // 当前站立方向
    m_nOldDir:Integer;

    m_SM60HitX:Integer;
    m_SM60HitY:Integer;
    m_SM60HitDir:Integer;

    m_btSex:byte; // 性别
    m_btRace:byte; // RaceImg
    m_btHair:byte; // 头发类型
    m_wDress:Word; // 衣服类型
    m_wWeapon:Word; // 武器类型
    m_wWeaponSound:Word; // 武器声音
    m_btHorse:byte; // 马类型
    m_btDoubleHumHorse:byte; // 骑马 - 双人骑马类型 chongchong 2013-10-14
    m_boShowHorseWingsEffect:Boolean; // 骑马 - 是否显示马翅膀特效 chongchong 2013-10-16
    m_btHorseEffectType:Byte; // 骑马 - 马特效 chongchong 2015-10-24

    m_btHorseHum:Word; // 骑马 三方马上的人物造型 chongchong 2013-10-17
    m_btHorseHumExpand:Byte; // 骑马 三方马上的人物造型扩展 chongchong 2014-09-24

    m_btHorseHair:Byte; // 骑马 三方马上的人物头发 chongchong 2013-10-17

    m_boShowFashion:Boolean; // 是否显示时装 chongchong 2013-10-23
    m_boMagicShield:Boolean; // 使用护身装备 chongchong 2014-04-07
    m_btReLevel:Integer; // 转生等级 chongchong 2014-05-07

    m_btOldHair:Byte;
    m_boPlayMoster:Boolean; // 是否是人形怪 chongchong 2015-09-19

    m_wEffect:Word; // 天使类型
    m_btJob:byte; // 职业 0:武士  1:法师  2:道士
    m_wAppearance:Word; // Appr
    m_btDeathState:byte;
    // m_nFeature: Integer; //0x18
    // m_nFeatureEx: Integer; //0x18

    m_wEffect_30:Word;

    m_boEffectNormalDraw:Boolean;
    m_boEffect_30NormalDraw:Boolean;
    m_boDressEffNoSex:Boolean;
    m_boDressEff_30NoSex:Boolean;

    FCurrX:Integer;
    FCurrY:Integer;
    FOldx:Integer;
    FOldy:Integer;
    FRx:Integer;
    FRy:Integer;
    FActBeforX:Integer;
    FActBeforY:Integer;

    m_nChangeAppr:Integer;

    m_wShield:Word; // 盾牌 chongchong 2013-09-16

    m_btCaseltGuild:Byte; // 1=沙行会成员 //2=沙行会掌门

    m_nWeaponEffectIndex:SmallInt; // 武器发光效果外观wil 编号
    m_wDBWeaponEffectOffSet:Word;
    m_wWeaponEffectOffSet:Word; // 武器发光效外观偏移
    m_nDressEffectIndex:SmallInt; // 衣服发光效外观wil 编号
    m_wDressEffectOffSet:Word; // 衣服发光效外观偏移
    m_nShieldEffectIndex:SmallInt; // 武器发光效果外观wil 编号
    m_wShieldEffectOffSet:Word; // 武器发光效外观偏移

    m_boDressEffectNoBlend:Boolean;
    m_boDressEffectNoSex:Boolean;

    m_nMedalEffectIndex:SmallInt; // 勋章发光效外观wil 编号
    m_wMedalEffectOffSet:Word; // 勋章发光效外观偏移
    m_boMedalEffectNoBlend:Boolean;
    m_boMedalEffectNoSex:Boolean;

    m_boWeaponEffectNoBlend:Boolean;
    m_boWeaponEffectNoSex:Boolean;
    m_boShieldEffectNoBlend:Boolean;
    m_boShieldEffectNoSex:Boolean;

    m_boDressEffectDrawNoBlend:Boolean;
    m_boWeaponEffectDrawNoBlend:Boolean;
    m_boShieldEffectDrawNoBlend:Boolean;

    m_WeaponEffectSurface:TTexture;
    m_DBWeaponEffectSurface:TTexture;
    m_DressEffectSurface:TTexture;

    m_ShieldEffectSurface:TTexture;

    m_nDressEffectX:Integer;
    m_nDressEffectY:Integer;
    m_nWeaponEffectX:Integer;
    m_nWeaponEffectY:Integer;
    m_nShieldEffectX:Integer;
    m_nShieldEffectY:Integer;

    m_nDBWeaponEffectX:Integer;
    m_nDBWeaponEffectY:Integer;

    m_boMedalEffectDrawNoBlend:Boolean;
    m_MedalEffectSurface:TTexture;
    m_nMedalEffectX:Integer;
    m_nMedalEffectY:Integer;

    m_nDressAddEffectIndex:SmallInt; // 附加衣服特效
    m_bDressAddEffectOrder:Byte; // 附加衣服特效绘制顺序
    m_wDressAddEffectOffSet:Word; // 附加衣服特效偏移
    m_wDressAddEffectCount:Word; // 附加衣服特效数量
    m_wDressAddEffectTime:Word; // 附加衣服特效时间
    m_boDressAddEffectNoBlend:Boolean; // 附加衣服特效 - 普通绘制
    m_boDressAddEffectDrawCenter:Boolean;

    m_btCboDressUseDiyImage:Byte; // 连击时衣服使用自定义资源
    m_btCboWeaponUseDiyImage:Byte; // 连击时武器使用自定义资源

    m_DressAddEffectSurface:TTexture;

    m_nDressAddEffectX:Integer;
    m_nDressAddEffectY:Integer;

    m_nDressAddEffectCurIndex:Integer;
    m_nDressAddEffectLastTick:LongWord;

    m_Feature:TFeature;

    m_nState:Integer;
    m_boGhost:Boolean;
    m_boDeath:Boolean;
    m_dwDeathTick:LongWord;
    m_boSkeleton:Boolean;

    m_boFreeActor:Boolean;
    m_boDelActor:Boolean;
    m_boDelActionAfterFinished:Boolean;
    m_sDescUserName:string; // 人物名称，后缀
    m_sUserName:string;

    m_nNameColor:Integer;
    m_Abil:TAbility;
    m_OAbil:TAbility;
    m_nGold:LongWord; // 金币数量
    m_nGameGold:LongWord; // 游戏币数量
    m_nGamePoint:LongWord; // 游戏点数量
    m_nGloryPoint:Integer; // 荣誉
    //m_nHitSpeed: shortint;                                                                          // 攻击速度 0: 扁夯, (-)蠢覆 (+)狐抚
    m_boVisible:Boolean;
    m_boHoldPlace:Boolean;

    m_SayingArr:array[0..MAXSAY - 1] of TSayingInfo;
    // m_SayWidthsArr: array[0..MAXSAY - 1] of Integer;
    m_dwSayTime:longword;
    m_nSayX:Integer;
    m_nSayY:Integer;
    m_nSayLineCount:Integer;

    m_boChangeEff:Boolean; // 是否变色--针对BOSS piaoyun 2013-09-09
    m_nShiftX:Integer;
    m_nShiftY:Integer;

    m_nPx:Integer;
    m_nHpx:Integer;
    m_nWpx:Integer;
    m_nSpx:Integer;
    m_nSpx_30:Integer;

    m_nPy:Integer;
    m_nHpy:Integer;
    m_nWpy:Integer;
    m_nSpy:Integer;
    m_nSpy_30:Integer;

    m_nDownDrawLevel:Integer;
    m_nTargetX:Integer;
    m_nTargetY:Integer;
    m_nTargetRecog:Int64;
    m_nHiterCode:Integer;
    m_nMagicNum:Integer;
    m_nEffectNum:Integer;
    m_MagicType:TMagicType;
    m_Saying:string; // 用于自定义攻击多目标特效(目标列表) chongchong 2014-09-28

    m_nCurrentEvent:Integer;
    m_boDigFragment:Boolean;
    m_boThrow:Boolean;

    m_nBodyOffset:Integer;
    m_nHairOffset:Integer;
    m_nHumWinOffset:Integer;
    m_nHumWinOffset_30:Integer;

    m_nWeaponOffset:Integer;
    m_boUseMagic:Boolean;
    m_boHitEffect:Boolean;
    m_boUseEffect:Boolean;
    m_nHitEffectNumber_Old:Integer;
    m_nHitEffectNumber:Integer;
    m_nHitEffectLevel_Old:Integer;
    m_nHitEffectLevel:Integer;
    m_nHitEffectLevel2:Integer;
    m_dwWaitMagicRequest:longword;
    m_nWaitForRecogId:Int64;
    // m_nWaitForFeature: Integer;
    m_nWaitForStatus:Integer;
    m_WaitForFeature:TFeature;

    m_boHitEndEffect:Boolean;
    m_nHitEndX:Integer;
    m_nHitEndY:Integer;

    m_boMagicEndEffect:Boolean;
    m_nMagicEndX:Integer;
    m_nMagicEndY:Integer;

    m_nCurEffFrame:Integer;
    m_nSpellFrame:Integer;
    m_nSpellSkipFrame:Integer;
    m_CurMagic:TUseMagicInfo; //m_CurMagic.EffectNumber 0x110
    // GlimmingMode: Boolean;
    // CurGlimmer: integer;
    // MaxGlimmer: integer;
    // GlimmerTime: longword;
    m_dwGenAnicountTime:longword;
    m_dwGenNewHitAnicountTime:LongWord;
    m_dwGenNewMagAnicountTime:LongWord;
    m_nGenAniCount:Integer;
    m_nGenNewHitAniCount:Integer;
    m_nGenNewMagAniCount:Integer;

    m_boOpenHealth:Boolean;
    m_noInstanceOpenHealth:Boolean;
    m_dwOpenHealthStart:longword;
    m_dwOpenHealthTime:longword; // Integer;jacky

    // SRc: TRect;  //Screen Rect 拳搁狼 角力谅钎(付快胶 扁霖)
    m_BodySurface:TTexture;
    // m_BodyAlphaSurface: TTexture;

    m_HorseSurface:TTexture; // 骑马 chongchong 2013-10-12

    m_boGrouped:Boolean; // 是否组队
    m_dwCurrentActionTick:LongWord;
    m_nCurrentAction:Integer;
    m_boReverseFrame:Boolean;
    m_boWarMode:Boolean;
    m_dwWarModeTime:longword;

    // 修改自定义技能无动作时，要完全无动作 chongchong 2019-03-15 13:27:14
    m_boCustomMagicNoAction:Boolean;

    m_nChrLight:Integer;
    m_nMagLight:Integer;
    m_nRushDir:Integer; // 0, 1
    m_nXxI:Integer;
    m_boLockEndFrame:Boolean;
    m_dwLastStruckTime:longword;
    m_dwSendQueryUserNameTime:longword;
    m_dwDeleteTime:longword;

    // 荤款靛 瓤苞
    m_nMagicStruckSound:Integer; // 被魔法攻击弯腰发出的声音
    m_boRunSound:Boolean; // 跑步发出的声音
    m_nFootStepSound:Integer; // CM_WALK, CM_RUN 走步声
    m_nStruckSound:Integer; // SM_STRUCK 腰声音
    m_nStruckWeaponSound:Integer; // 被指定武器攻击弯腰声音

    m_nAppearSound:Integer;
    m_nNormalSound:Integer;
    m_nAttackSound:Integer;
    m_nWeaponSound:Integer;
    m_nScreamSound:Integer;
    m_nDieSound:Integer; // SM_DEATHNOW
    m_nDie2Sound:Integer;

    m_nMagicStartSound:Integer;
    m_nMagicFireSound:Integer;
    m_nMagicExplosionSound:Integer;
    m_Action:pTMonsterAction;
    m_OldColorEffect:TColorEffect;
    m_ColorEffect:TColorEffect;

    m_dwMoveTime:Int64;
    m_nMoveStepCount:Integer;
    m_boCanMove:Boolean;
    m_nMoveSpeed:SmallInt; // 行走速度  -10 ~ +10
    m_nAttackSpeed:SmallInt; // 攻击速度  -10 ~ +10
    m_nSpellSpeed:SmallInt; // 魔法速度  -10 ~ +10

    m_sNameText:string;
    m_sCurNameText:string;

    m_HealthNumberArray:THealthNumberArray;
    m_HealthNumberLock:TRTLCriticalSection;

    m_NumberFramIndex:array[TNumberType] of Integer;

    m_boShopStall:Boolean; // 是否在摆摊
    m_btBodyColor:Byte; // 人体颜色
    m_ActorIcons:TActorIconArray;
    m_ActorIconIndexs:TActorIconIndexArray;
    m_ActorEffects:TGList;

    m_sActiveFengHaoName:string; // 激活的封号名
    m_nActiveFengHaoID:Integer; // 激活的封号ID
    m_dwActiveFengHaoLooks:Word; // 激活封号的Looks
    m_btActiveFengHaoReserved:Byte;
    m_nActiveFengHaoColor:Integer;

    m_ShowNumberLableTimeTick:LongWord;
    m_sCurNumberLableText:string;
    m_sNumberLableText:string;

    m_sCurShopNameText:string;
    m_sShopNameText:string;

    m_dwShowShopNameTimeTick:LongWord;
    m_NameTextSurface:TTexture;
    m_ShopNameImageInfo:TImageInfo;
    m_NumberLableImageInfo:TImageInfo;

    m_FengHaoEffectSurface:TTexture;
    m_FengHaoImageInfo:TImageInfos;
    m_nOldFengHaoSurfaceID:Integer;
    m_dwLoadFengHaoSurfaceTime:Longword;

    m_boShowHealthNumber:Boolean;
    m_HearMsgColor:TColor; // 公聊颜色

    m_boCobweb:Boolean; // 网罩住了
    m_dwCobwebTick:LongWord;
    m_nCobwebIndex:Integer;
    m_boDuanJin:Boolean; // 断筋

    m_boToxicSmoke:Boolean; // 是否中了毒烟 chongchong 2013-11-10
    m_nToxicSmokeIndex:Integer;
    m_dwToxicSmokeTick:LongWord; // 毒烟时间 chongchong 2013-11-10

    m_boForeverFrozen:Boolean; // 是否永恒冰冻 piaoyun 2013-12-05
    m_nForeverFrozenIndex:Integer; // 永恒冰冻帧序 piaoyun 2013-12-05
    m_dwForeverFrozenTick:LongWord; // 永恒冰冻时间 piaoyun 2013-12-05

    m_boDingShen:Boolean; // 定身 chongchong 2015-03-12
    m_boTanHuan:Boolean; // 瘫痪 chongchong 2015-03-12
    m_boThunderPalsy:Boolean;

    m_CustomMagicStatusEffect:TCustomMagicStatusEffect;

    m_UseEffectImage:TGameImages;

    m_boCanDraw:Boolean;

    m_nStartFrame:Integer;
    m_nEndFrame:Integer;
    m_nCurrentFrame:Integer;
    m_nEffectStart:Integer;
    m_nEffectFrame:Integer;
    m_nEffectEnd:Integer;

    m_dwFrameTime:longword;
    m_dwStartTime:longword;
    m_dwActionEndTime:longWord;

    m_StartCounter:Int64;

    m_boStruckShowNumber:Boolean;

    m_boSendQueryBigHPProgress:Boolean;
    m_boShowBigHPProgress:Boolean; // 是否显示怪物大血条 chongchong 2016-05-10
    m_BigHPProgressInfo:TMonHPProgress;

    m_boShowPhantom:Boolean; // 绘制放大的人物效果
    m_btPhantomAlpha:Byte; // 放大的人物透明度

    m_btRealRace:Byte; // 服务器传过来的脸型 (Race字段) 2013-07-19
    m_HumsBBType:THumBBType; // 是否为人物的宝宝 (挂机要用) chongchong 2014-11-29
    m_IsExploreItem:Boolean;

    m_dwExploreItemEffectTick:LongWord;
    m_dwExploreItemEffectFrame:LongWord;

    m_boShowHair:Boolean;
    m_SelfKeepPlay:TClientSelfPlay;

    //m_PreviewItem: array of TPreviewMonItem;
  private
    m_boSelfEffectRunning:Boolean; // 是否显示自身动画
    m_boSelfEffectBlendDraw:Boolean;
    m_nSelfEffectCurrentFrame:Integer; // 当前帧
    m_nSelfEffectEndFrame:Integer; // 自身动画最后帧数
    m_nSelfEffectFrameTime:LongWord; // 自身动画时间间隔
    m_SelfEffectGameImage:TGameImages; // 自身动画所在图库
    m_dwSelfEffectLastTick:LongWord; // 上一帧时间

  private
    function GetMessage(ChrMsg:pTChrMsg):Boolean;
    function GetCurrX:Integer;
    function GetCurrY:Integer;
    procedure SetCurrX(Value:Integer);
    procedure SetCurrY(Value:Integer);
    function GetOldX:Integer;
    function GetOldY:Integer;
    procedure SetOldX(Value:Integer);
    procedure SetOldY(Value:Integer);
    function GetRX:Integer;
    function GetRY:Integer;
    procedure SetRX(Value:Integer);
    procedure SetRY(Value:Integer);
    function GetActBeforeX:Integer;
    function GetActBeforeY:Integer;
    procedure SetActBeforeX(Value:Integer);
    procedure SetActBeforeY(Value:Integer);
  protected
    m_dwEffectStartTime:longword;
    m_dwEffectFrameTime:longword;
    m_nMaxTick:Integer;
    m_nCurTick:Integer;
    m_nMoveStep:Integer;
    m_btStep:Integer;
    m_boMsgMuch:Boolean;
    m_dwStruckFrameTime:longword;
    m_nCurrentDefFrame:Integer;
    m_dwDefFrameTime:longword;
    m_nDefFrameCount:Integer;
    m_nSkipTick:Integer;
    m_dwSmoothMoveTime:longword;
    m_dwLoadSurfaceTime:longword;
    m_boLoadSurface:Boolean;

    m_nWpord:Integer;

    m_nStartPosHitFrame:Integer;
    m_nStartHitFrame:Integer;
    m_nEndHitFrame:Integer;
    m_nCurrentHitFrame:Integer;
    m_dwStartHitTime:longword;
    m_btHitDir:Byte;

    m_nStartPosMagicFrame:Integer;
    m_nStartMagicFrame:Integer;
    m_nEndMagicFrame:Integer;
    m_nCurrentMagicFrame:Integer;
    m_dwStartMagicTime:longword;
    m_btMagicDir:Byte;

    m_boCreateEffect:Boolean;
    {平滑移动By 一支笔 at:2022-03-25 16:34:19}
    //m_dwStartMoveTime: LongWord;
    //m_fVecX: Double;
    //m_fVecY: Double;

    function DefaultMotion:Boolean; virtual;
    function GetDefaultFrame(wmode:Boolean):Integer; virtual;
    procedure DrawEffSurface(Source:TTexture; ddx, ddy:Integer; blend:Boolean; ceff:TColorEffect);
    procedure DrawStateEffSurface(ddx, ddy:Integer);
    procedure StretchDrawEffSurface(Source:TTexture; ddx, ddy:Integer; blend:Boolean; ceff:TColorEffect);
    procedure DrawWeaponGlimmer(ddx, ddy:Integer); virtual;
    procedure DrawDressEffect(ddx, ddy:Integer; blend:Boolean; ceff:TColorEffect); virtual;
    //procedure DrawDressEffectEx(ddx, ddy: Integer; blend: Boolean; ceff: TColorEffect); virtual;
    procedure DrawShieldEffect(ddx, ddy:Integer); virtual;

  public
    m_MsgList:TGList; // list of PTChrMsg
    RealActionMsg:TChrMsg; // FrmMain

    constructor Create; virtual;
    destructor Destroy; override;
    procedure CalcActorFrame; virtual;
    function GetNextHitTime:Integer;
    //function CanMove: Boolean;
    function FindMsg(wIdent:Word):Boolean;

    procedure SendMsg(wIdent:Word; nX, nY, ndir:Integer; Feature:TFeature; nState:Int64; sStr:string; nSound:Integer); overload;
    function UpdateMsg(wIdent:Word; nX, nY, ndir:Integer; Feature:TFeature; nState:Int64; sStr:string; nSound:Integer; IsScreenSync:Boolean = False):Boolean; overload;

    procedure SendMsg(wIdent:Word; nX, nY, ndir:Integer; nState:Int64; sStr:string; nSound:Integer); overload;
    function UpdateMsg(wIdent:Word; nX, nY, ndir:Integer; nState:Int64; sStr:string; nSound:Integer; IsScreenSync:Boolean = False):Boolean; overload;
    procedure UpdateStruckMsg(wIdent:Word; nX, nY, ndir:Integer; Feature:TFeature; nState:Int64; sStr:string; nSound:Integer);
    procedure ActionChanged;

    procedure CleanUserMsgs;
    procedure CleanMsgs;
    procedure CleanStruckMsg;
    procedure DeleteMsg(wIdent:Word);
    procedure ProcMsg;
    procedure ProcLastMsg;
    procedure ProcHurryMsg;
    function IsIdle:Boolean;
    function ActionFinished:Boolean;
    function CanWalk:Integer;
    function CanRun:Integer;
    function Strucked:Boolean;
    procedure Shift(dir, step, cur, Max:Integer);
    procedure ReadyAction(Msg:TChrMsg);
    function CharWidth:Integer;
    function CharHeight:Integer;
    function CheckSelect(dx, dy:Integer):Boolean;
    procedure CleanCharMapSetting(X, Y:Integer);
    procedure ShowName;
    procedure ShowShopName;
    procedure ShowNumberLable;
    procedure ShowIcons(IsBackActor:Boolean);
    procedure DrawPlayEffect(ddx, ddy:Integer; IsBackActor:Boolean);

    procedure DrawSelfEffect(dx, dy:Integer; IsBackActor:Boolean);
    procedure DrawLockTargetEffect(Frame:Integer; dx, dy:Integer);
    procedure DrawExploreItemEffect(dx, dy:Integer);

    // procedure DrawPreviewItem(dx, dy: Integer);

    //HZQ 20230816 增加样式方法
    procedure ShowHealthNumber;

    procedure AddHealthNumber(NumberType:TNumberType; Number:Integer; nResID, nResStartIdx:Integer; nDrawStyle:TNumberDrawStyle);
    function NewHealthNumberFromGroup(NumberType:TNumberType):pTHealthNumber;
    procedure ShowSay;
    procedure Say(Str:string);
    procedure SetSound; virtual;
    procedure SetMagicSound(wMagicID:Word);
    procedure Run; virtual;
    procedure RunSound; virtual;
    procedure RunActSound(frame:Integer); virtual;
    procedure RunFrameAction(frame:Integer); virtual;
    procedure ActionEnded; virtual;
    function DoMove(step:Integer):Boolean;
    //function DoSmoothMove(step: Integer): Boolean;
    procedure MoveFail(nRestoreX:Integer = -1; nRestoreY:Integer = -1; nDir:Integer = -1);
    function CanCancelAction:Boolean;
    procedure CancelAction;
    procedure FeatureChanged; virtual;
    function light:Integer; virtual;

    function GetDrawEffectValue:TColorEffect;
    procedure DrawChr(dx, dy:Integer; blend:Boolean; boFlag:Boolean); virtual;
    procedure DrawEff(dx, dy:Integer); virtual;
    //procedure DrawNearObjectHintEffect(nX, nY:Integer);virtual; //HZQ 20230829 绘制指定目标效果
    function GetNearObjectHintInfo(var X, Y:Integer; out nFriendFlag:Integer):Boolean;

    procedure LoadNameSurface;
    procedure LoadNumberLableSurface;
    procedure LoadSaySurface;
    procedure LoadActorIcons;
    procedure LoadHealthNumber;
    procedure LoadSurface(Sender:TObject); virtual;

    procedure LoadFengHaoSurface;
    procedure LoadPlayEffectSurface;

    procedure Initialize; virtual;
    procedure Finalize; virtual;

    function CheckLoadUserName:Boolean; virtual;
    function CheckLoadNumberLable:Boolean;
    function CheckLoadHealthNumber:Boolean;
    function CheckLoadSay:Boolean;
    function CheckLoadActorIcon:Boolean;
    function CheckLoadSurface:Boolean; virtual;
    function CheckLoadFengHaoSurface:Boolean; virtual;
    function CheckLoadPlayEffect:Boolean;

    procedure PlaySelfEffect(MsgIdent:Integer; nType:Integer);
    procedure PlaySelfEffectCustom(ImgFileIndex, ImgIndex, ImgCount, FrameTime:Integer; IsBlendDraw:Boolean);

    property m_nCurrX:Integer read GetCurrX write SetCurrX;
    property m_nCurrY:Integer read GetCurrY write SetCurrY;

    property m_nOldx:Integer read GetOldX write SetOldX;
    property m_nOldy:Integer read GetOldY write SetOldY;

    property m_nRx:Integer read GetRX write SetRX;
    property m_nRy:Integer read GetRY write SetRY;

    property m_nActBeforeX:Integer read GetActBeforeX write SetActBeforeX;
    property m_nActBeforeY:Integer read GetActBeforeY write SetActBeforeY;
    //m_nCurrX: Integer;                                                                              // 当前所在地图座标X
    //m_nCurrY: Integer;                                                                              // 当前所在地图座标Y
  end;

  TNpcActor = class(TActor)
  private
    m_nEffX:Integer;
    m_nEffY:Integer;
    m_bo248:Boolean;
    m_dwUseEffectTick:longword;
    m_EffSurface:TTexture;

    m_nKeepFrame:Integer;
    m_nKeepX:Integer;
    m_nKeepY:Integer;
    m_KeepSurface:TTexture;
    m_LastKeepPlayTick:LongWord;
  public
    constructor Create; override;
    procedure Run; override;
    procedure CalcActorFrame; override;
    function GetDefaultFrame(wmode:Boolean):Integer; override;
    procedure LoadSurface(Sender:TObject); override;
    procedure DrawChr(dx, dy:Integer; blend:Boolean; boFlag:Boolean); override;
    procedure DrawEff(dx, dy:Integer); override;
    procedure Initialize; override;
    procedure Finalize; override;
    function CheckLoadUserName:Boolean; override;
  end;

  TStatuaryNpcActor = class(TNpcActor)
  private
    FIsFinalized:Boolean;
    m_nEffigyState:TFeature_New;

    m_nEffigyOffset:Integer;
    m_nOldEffigyOffset:Integer;

    m_boShowStatuary:Boolean;

    m_boScaleShow:Boolean;

    m_IsGrayShow:Boolean;

    m_HumTexture:TTexture;
    m_HumEffTexture:TTexture;
    m_WeaponEffTexture:TTexture;
  public
    constructor Create; override;
    destructor Destroy; override;
    procedure Finalize; override;

    procedure CalcActorFrame; override;
    function GetDefaultFrame(wmode:Boolean):Integer; override;
    procedure LoadSurface(Sender:TObject); override;
    procedure DrawChr(dx, dy:Integer; blend:Boolean; boFlag:Boolean); override;
    function CheckLoadSurface:Boolean; override;
    procedure SetEffigyState(IsGrayShow:Boolean; IsScaleShow:Boolean; nEffigyState:TFeature_New; nOffset:Integer);

    property boScaleShow:Boolean read m_boScaleShow;
  end;

  THumActor = class(TActor) // Size: 0x27C Address: 0x00475BB8
    m_boTrainingNG:Boolean; // 是否学习过内功
    m_boTrainingXF:Boolean; // 是否学习过心法
    m_AbilNG:TClientAbilityNG; // 内功属性
    m_Alcohol:TAbilityAlcohol; // 酒属性
    m_HumMeridians:THumMeridians; // 人物经络

    m_wHeroM2DressEffect:Word;
    m_boHeroM2DressNoBlend:Boolean;

    m_wOldHeroM2DressEffect:Byte;
    m_boOldHeroM2DressNoBlend:Boolean;

    m_nHeroM2DressEffectX:Integer;
    m_nHeroM2DressEffectY:Integer;
    m_HeroM2DressEffect:TTexture;

    m_nJewelryBoxStatus:TJewelryBoxStatus; // 首饰盒状态 0:未激活; 1:激活; 2:开启 chongchong 2013-10-19
    m_boShowGodBless:Boolean; // 显示神佑袋 chongchong 2014-04-18

    m_FriendHitList:TStringList;
  private
    m_HairSurface:TTexture;
    m_WeaponSurface:TTexture;
    m_ShieldSurface:TTexture; // 盾牌 chongchong 2013-09-16

    m_HorseWingsEffectSurface:TTexture; // 骑马 马翅膀特效 chongchong 2013-10-16
    m_HorseEffectSurface:TTexture; // 骑马 马特效 chongchong 2015-10-24

    m_HorseHairSurface:TTexture; // 骑马 马上人物的头发 chongchong 2013-10-17
    m_HorseHumSurface:TTexture; // 骑马 马上人物 chongchong 2013-10-17

    m_HumWinSurface:TTexture;
    m_HumWinSurface_30:TTexture;

    m_ShopStallSurface:TTexture;
    m_ShopHeadSurface:TTexture; // 摆摊头顶显示个人商店图片 piaoyun 2013-09-13
    m_boWeaponEffect:Boolean;
    m_nCurWeaponEffect:Integer;
    m_nCurBubbleStruck:Integer;
    m_nCurNewHitBubbleStruck:Integer;
    m_nCurNewMagBubbleStruck:Integer;
    m_dwWeaponpEffectTime:longword;
    m_boHideWeapon:Boolean;
    m_nFrame:Integer;
    m_dwFrameTick:longword;
    // m_dwFrameTime: longword;
    m_bo2D0:Boolean;

    m_boBrokenShield:Boolean;
    m_nBrokenShieldEffect:Integer;
    m_dwBrokenShieldEffectTime:LongWord;

    m_dwHitFrameTime:longword;

    m_nShieldx:Integer;
    m_nShieldy:Integer;

    m_nHorseWingsEffectX,
      m_nHorseWingsEffectY:Integer;

    m_nHorseEffectX,
      m_nHorseEffectY:Integer;

    m_nHorseHairX,
      m_nHorseHairY:Integer;

    m_nHorseHumX,
      m_nHorseHumY:Integer;

    m_dwMagicFrameTime:longword;
    m_OnShopStall:TNotifyEvent;

    m_nShopStallX, m_nShopStallY:Integer;

    m_nCurSelfEffFrame:Integer;
    m_dwCurSelfEffFrameTick:Integer;
  private
    procedure OnTargetFinished(Sender:TObject);
    procedure OnTargetExplosion(Sender:TObject);
  protected
    function DefaultMotion:Boolean; override;
    function GetDefaultFrame(wmode:Boolean):Integer; override;
    procedure DrawDressEffect(ddx, ddy:Integer; blend:Boolean; ceff:TColorEffect); override;
    //procedure DrawDressEffectEx(ddx, ddy: Integer; blend: Boolean; ceff: TColorEffect); override;
  public
    procedure CalcActorFrame; override;
    constructor Create; override;
    destructor Destroy; override;
    procedure Run; override;
    procedure RunFrameAction(frame:Integer); override;
    function light:Integer; override;
    procedure LoadSurface(Sender:TObject); override;

    procedure DoWeaponBreakEffect;
    procedure DoBrokenShieldEffect;
    procedure DrawChr(dx, dy:Integer; blend:Boolean; boFlag:Boolean); override;
    procedure Initialize; override;
    procedure Finalize; override;
    function CheckLoadUserName:Boolean; override;

    function CheckLoadDressAddEffect:Boolean;
    procedure LoadDressAddEffect;

    function CheckLoadSurface:Boolean; override;

    function UseMagicDelayTime(dwDelayTime:Integer):Integer;
    property OnShopStall:TNotifyEvent read m_OnShopStall write m_OnShopStall;

    procedure TakeHorse; // 召唤/收回马 (骑马) chongchong 2013-10-12

    procedure PlayMagicEffect(UseMagicInfo:PTUseMagicInfo; nTargetRecogID:Int64);
  end;

  THeroActor = class(THumActor)
    m_nBagCount:Integer;
    m_nAngryValue:Integer; // 愤怒值
    m_nMaxAngryValue:Integer; // 最大愤怒值
    m_sLoyalPoint:string; // 忠诚度
    m_boFixedHero:Boolean; // 有没有评定
    m_boIsDeputy:Boolean; // 是不是副将

    m_btAttackMode:Byte;
  public
    function GetGroupMagicId:Integer;
    function FindGroupMagic:pTClientMagic;
    procedure Rest;
    procedure Target;
    procedure Protect;
    procedure GroupAttack;
  end;

function GetRaceByPM(race:Integer; Appr:Word):pTMonsterAction;
function GetOffset(Appr:Integer):Integer;
function GetNpcOffset(nAppr:Integer):Integer;
function PointRect(X, Y:Integer; SrcRect:TRect):TRect;

implementation

uses
  ClMain,
  PlugEngine,
  SoundUtil,
  clEvent,
  MShare,
  FState,
  GameConfigDlg,
  GameConfigDlgs,
  SerialWindowsDlg,
  DxComponents,
  GlobalString,
  CustomActor,
  PlayScn;

const
  LightColor = $404040;
  LightAlpha = 255;

  // RaceImg、Appr 对应数据库：RaceImg、Appr  --- piaoyun 【2013-6-15】

function GetRaceByPM(race:Integer; Appr:Word):pTMonsterAction;
begin
  case race of
    9 {01}:Result := @MA9;
    10 {02}:Result := @MA10;
    11 {03}:Result := @MA11;
    12 {04}:Result := @MA12;
    13 {05}:Result := @MA13;
    14 {06}:Result := @MA14;
    15 {07}:Result := @MA15;
    16 {08}:Result := @MA16;
    17 {06}:Result := @MA14;
    18 {06}:Result := @MA14;
    19 {0A}:Result := @MA19;
    20 {0A}:Result := @MA19;
    21 {0A}:Result := @MA19;
    22 {07}:Result := @MA15;
    23 {06}:Result := @MA14;
    24 {04}:Result := @MA12;
    30 {09}:Result := @MA17;
    31 {09}:Result := @MA17;
    32 {0F}:Result := @MA24;
    33 {10}:Result := @MA25;
    34 {11}:Result := @MA30; // 赤月恶魔
    35 {12}:Result := @MA31;
    36 {13}:Result := @MA32;
    37 {0A}:Result := @MA19;
    40 {0A}:Result := @MA19;
    41 {0B}:Result := @MA20;
    42 {0B}:Result := @MA20;
    43 {0C}:Result := @MA21;
    45 {0A}:Result := @MA19;
    47 {0D}:Result := @MA22;
    48 {0E}:Result := @MA23;
    49 {0E}:Result := @MA23;
    50 {27}:begin
        case Appr of
          23 {01}:Result := @MA36;
          24 {02}:Result := @MA37;
          25 {02}:Result := @MA37;
          26 {00}:Result := @MA35;
          27 {02}:Result := @MA37;
          28 {00}:Result := @MA35;
          29 {00}:Result := @MA35;
          30 {00}:Result := @MA35;
          31 {00}:Result := @MA35;
          32 {02}:Result := @MA37;
          33 {00}:Result := @MA35;
          34 {00}:Result := @MA35;
          35 {03}:Result := @MA41;
          36 {03}:Result := @MA41;
          37 {03}:Result := @MA41;
          38 {03}:Result := @MA41;
          39 {03}:Result := @MA41;
          40 {03}:Result := @MA41;
          41 {03}:Result := @MA41;
          42 {04}:Result := @MA46;
          43 {04}:Result := @MA46;
          44 {04}:Result := @MA46;
          45 {04}:Result := @MA46;
          46 {04}:Result := @MA46;
          47 {04}:Result := @MA46;
          48 {03}:Result := @MA41;
          49 {03}:Result := @MA41;
          50 {03}:Result := @MA41;
          51 {00}:Result := @MA35;
          52 {03}:Result := @MA41;
          53 {03}:Result := @MA41;
          54..58, 94..98:Result := @MA54;
          59:Result := @NPC1;
          60..68:Result := @NPC60;
          70..75:Result := @NPC60;
          76, 77..80:Result := @MA35;
          81..83:Result := @NPC81;
          84:Result := @NPC84;
          90..92:Result := @NPC60;

          99:Result := @NPC60;
          100:Result := @NPC1;
          101:Result := @NPC101;
          102:Result := @NPC60;

          200..203, 207, 208:Result := @MA35;
          204..206:Result := @NPC60;
          209:Result := @NPC60;
          210:Result := @NPC210;
          211..225:Result := @NPC1;

          226..235:Result := @NPC226;
          236..240:Result := @NPC236;
          241..243:Result := @NPC241;
          244:Result := @NPC244;
          245:Result := @NPC245;
          246, 247, 248, 249, 251, 254, 255, 256, 259, 260, 261, 262:Result := @NPC246;
          263, 264:Result := @NPC263;
          265, 266, 267, 268, 271, 272:Result := @NPC246;
          269, 270:Result := @NPC269;
          250:Result := @NPC250;
          252:Result := @NPC252;
          253, 257:Result := @NPC253;
          258:Result := @NPC258;
          else
            Result := @MA35;
        end;
      end;

    52 {0A}:Result := @MA19;
    53 {0A}:Result := @MA19;
    54 {14}:Result := @MA28; // 小神兽
    55 {15}:Result := @MA29; // 大神兽
    56 {15}:Result := @MA56; // @MA33;
    60 {16}:Result := @MA33;
    61 {16}:Result := @MA33;
    62 {16}:Result := @MA33;
    63 {17}:Result := @MA34;
    64 {18}:Result := @MA19;
    65 {18}:Result := @MA19;
    66 {18}:Result := @MA19;
    67 {18}:Result := @MA19;
    68 {18}:Result := @MA19;
    69 {18}:Result := @MA19;
    70 {19}:Result := @MA33;
    71 {19}:Result := @MA33;
    72 {19}:Result := @MA33;
    73 {1A}:Result := @MA19;
    74 {1B}:Result := @MA19;
    75 {1C}:Result := @MA39;
    76 {1D}:Result := @MA38;
    77 {1E}:Result := @MA39;
    78 {1F}:Result := @MA40;
    79 {20}:Result := @MA19;
    80 {21}:Result := @MA42;
    81 {22}:Result := @MA43;
    83 {23}:Result := @MA44; // 火龙教主 piaoyun 2013-08-19
    84 {24}:Result := @MA45;
    85 {24}:Result := @MA45;
    86 {24}:Result := @MA45;
    87 {24}:Result := @MA45;
    88 {24}:Result := @MA45;
    89 {24}:Result := @MA45;
    90 {11}:Result := @MA30;
    95:Result := @MA95; // 火龙守护者 piaoyun 2013-08-19
    98 {25}:Result := @MA27;
    99 {26}:Result := @MA26;
    100:Result := @MA19;
    101:Result := @MA19;
    102:Result := @MA102;
    103:Result := @MA103;
    104:Result := @MA104;
    105:Result := @MA105;
    106:Result := @MA106;
    107:Result := @MA107;
    108:Result := @MA108;
    109:Result := @MA109;
    // 真狐月天珠动作帧
    110:Result := @MA110;
    111:Result := @MA19;
    112:Result := @MA112;
    113:Result := @MA113;
    114:Result := @MA114;
    115:Result := @MA115;
    116:Result := @MA33;
    117:Result := @MA117;
    // 新怪物测试 piaoyun 2013-07-27
    200:Result := @MA200;
    201:Result := @MA201;
    202:Result := @MA202;
    203:Result := @MA203;
    204:Result := @MA204;
    205:Result := @MA205;
    206:Result := @MA206;
    207:Result := @MA207;
    208:Result := @MA208;
    209:Result := @MA209;
    210:Result := @MA202;
    252:Result := @MA252;

    220:Result := @MA220;
    221, 224:Result := @MA19;
    222:Result := @MA222;
    223:Result := @MA223;
    225:Result := @MA225;
    // 圣兽
    //270..272: Result := @MA252;

    // 血灵教主 chongchong 2014-09-10
    253:Result := @MA253;

    // 新神兽 Mon41-2, Mon41-3 chongchong 2014-11-21
    254:Result := @MA254;
    255:Result := @MA255;
    else
      Result := @MA19;
  end
end;

// 计算方式MonX(X=Appr div 10)+1
{ TODO -opiaoyun -c注释 : 计算MonX--怪物图片偏移【2013-6-15】 }

function GetOffset(Appr:Integer):Integer;
var
  nrace, npos:Integer;
begin
  Result := 0;
  // 修正appr>=1000时，只读每个文件第一组怪物 chongchong 2015-09-07
  if (Appr >= 1000) then begin
    Result := (Appr mod 10) * 360;
    Exit;
  end;

  nrace := Appr div 10;
  npos := Appr mod 10;
  case nrace of
    0:Result := npos * 280;
    1:Result := npos * 230;
    2, 3, 7..12:Result := npos * 360;
    4:begin
        Result := npos * 360;
        if npos = 1 then Result := 600;
      end;
    5:Result := npos * 430;
    6:Result := npos * 440;
    // 13:   Result := npos * 360;
    13:
      case npos of
        0:Result := 0;
        1:Result := 360;
        2:Result := 440;
        3:Result := 550;
        else
          Result := npos * 360;
      end;
    14:Result := npos * 360;
    15:Result := npos * 360;
    16:Result := npos * 360;
    17:
      case npos of // Mon18
        2:Result := 920;
        3:Result := 1280;
        else
          Result := npos * 350;
      end;
    18:
      case npos of // Mon19
        0:Result := 0;
        1:Result := 520;
        2:Result := 950;

        3:Result := 1574;
        4:Result := 1934;
        5:Result := 2294;
        6:Result := 2654;
        7:Result := 3014;
      end;
    19:
      case npos of // Mon20
        0:Result := 0;
        1:Result := 370;
        2:Result := 810;
        3:Result := 1250;
        4:Result := 1630;
        5:Result := 2010;
        6:Result := 2390;
      end;
    20:
      case npos of // Mon21
        0:Result := 0;
        1:Result := 360;
        2:Result := 720;
        3:Result := 1080;
        4:Result := 1440;
        5:Result := 1800;
        6:Result := 2350;
        7:Result := 3060;
      end;
    21:
      case npos of // Mon22
        0:Result := 0;
        1:Result := 460;
        2:Result := 820;
        3:Result := 1180;
        4:Result := 1540;
        5:Result := 1900;
        // 6: Result := 2260;
        6:Result := 2440;
        7:Result := 2570;
        8:Result := 2700;
      end;
    22:
      case npos of // Mon23
        0:Result := 0;
        1:Result := 430;
        2:Result := 1290;
        3:Result := 1810;
      end;
    23:
      case npos of // Mon24
        0:Result := 0;
        1:Result := 340;
        2:Result := 680;
        3:Result := 1180;
        4:Result := 1770;
        5:Result := 2610;
        6:Result := 2950;
        7:Result := 3290;
        8:Result := 3750;
        9:Result := 4460;
      end;
    24:
      case npos of // Mon25
        0:Result := 0;
        1:Result := 510;
        2:Result := 1090;
      end;
    25:
      case npos of // Mon26
        0:Result := 0;
        1:Result := 510;
        2:Result := 1020;
        3:Result := 1370;
        4:Result := 1720;
        5:Result := 2070;
        6:Result := 2740;
        7:Result := 3780;
        8:Result := 3820;
        9:Result := 4170;
      end;

    26:
      case npos of // Mon27
        0:Result := 0;
        1:Result := 340;
        2:Result := 680;
        3:Result := 1190;
        4:Result := 2100;
        5:Result := 2440;
        6:Result := 2540;
        7:Result := 3570;
      end;

    27:
      // 圣兽基址重新定义 piaoyun 2013-11-23
      case npos of // Mon28
        0:Result := 0;
        1:Result := 350;
        2:Result := 780;
        3:Result := 1130;
        4:Result := 1560;
        5:Result := 1910;
        // 三个完整圣兽
        {0: Result := 0;
        1: Result := 780;
        2: Result := 1560;
        // 从中间扩充的三个圣兽
        3: Result := 350;
        4: Result := 1130;
        5: Result := 1910; }
      end;

    28:
      case npos of // Mon29
        0:Result := 0;
        1:Result := 600;
      end;

    29:
      case npos of // Mon30
        0:Result := 0;
        1:Result := 360;
        2:Result := 720;
        3:Result := 1070;
      end;
    32:
      case npos of // Mon33
        0:Result := 0;
        1:Result := 440;
        2:Result := 820;
        3:Result := 1360;
        4:Result := 2650;
        5:Result := 2680;
        6:Result := 2790;
        7:Result := 2900;
        8:Result := 3500;
        9:Result := 3930;
      end;
    33:
      case npos of // Mon34
        0:Result := 20;
        1:Result := 720;
        2:Result := 1160;
        3:Result := 1840;
        4:Result := 2540;
        5:Result := 2900;
        6:Result := 3250;
        7:Result := 3310;
      end;
    34:
      case npos of // Mon35
        0:Result := 0;
        1:Result := 680;
        2:Result := 1030;
      end;
    { TODO -opiaoyun -c扩展 : 对新客户端版本 Mon36支持 【2013-6-15】 }
    35: // Mon36
      case nPos of
        0:Result := 0;
        1:Result := 810;
        2:Result := -1; // 保留
        3:Result := 1800;
        4:Result := 2610;
        5:Result := 3420;
        6:Result := 4390;
        7:Result := 5200;
        8:Result := 6170;
        9:Result := 6980;
        10:Result := 7790;
        11:Result := 8760;
        12:Result := 9570;
        13:Result := -1; // 保留
        14:Result := 11030;
        15:Result := 12000;
        16:Result := 13800;
        17:Result := 14770;
        18:Result := 15580;
        19:Result := 16390;
        20:Result := 17360;
        21:Result := 18330;
        22:Result := 19300;
        23:Result := 20270;
        24:Result := 21240;
        25:Result := 22050;
        26:Result := 22860;
        27:Result := 23990;
        28:Result := 24800;
        29:Result := 25930;
      end;
    36: // Mon37
      case nPos of
        0:Result := 0;
        1:Result := 400;
        2:Result := 960;
        3:Result := 1360;
        4:Result := 1440;
        5:Result := 1840;
        6:Result := 2240;
        7:Result := 2840;
        8:Result := 3320;
      end;
    40:
      // 新神兽 Mon41-2, Mon41-3 chongchong 2014-11-21
      case nPos of
        0:Result := 0;
        1:Result := 352;
        2:Result := 480;
      end;
    49, 50, 51, 52, 53:Result := npos * 360;
    { TODO -opiaoyun -c扩展 : 对新客户端版本 Mon36支持 【2013-12-09】 }
    60: // Mon36
      case nPos of
        0:Result := 0;
        1:Result := 810;
        2:Result := -1; // 保留
        3:Result := 1800;
        4:Result := 2610;
        5:Result := 3420;
        6:Result := 4390;
        7:Result := 5200;
        8:Result := 6170;
        9:Result := 6980;
      end;
    61:
      case nPos of
        0:Result := 7790;
        1:Result := 8760;
        2:Result := 9570;
        3:Result := -1; // 保留
        4:Result := 11030;
        5:Result := 12000;
        6:Result := 13800;
        7:Result := 14770;
        8:Result := 15580;
        9:Result := 16390;
      end;
    62:
      case nPos of
        0:Result := 17360;
        1:Result := 18330;
        2:Result := 19300;
        3:Result := 20270;
        4:Result := 21240;
        5:Result := 22050;
        6:Result := 22860;
        7:Result := 23990;
        8:Result := 24800;
        9:Result := 25930;
      end;

    63:
      case npos of // Mon32
        0:Result := 0;
        1:Result := 610;
        2:Result := 1050;
        3:Result := 1420;
        4:Result := 1860;
        5:Result := 2230;
        6:Result := 2670;
        7:Result := 3190;
        8:Result := 3710;
        9:Result := 4390;
      end;
    64:
      case npos of // Mon32
        0:Result := 4750;
        1:Result := 5270;
        2:Result := 5870;
        3:Result := 6680;
      end;

    95:
      case npos of
        // 血灵教主 chongchong 2014-09-10
        3:Result := 3008;
        else
          // 新骷髅测试 piaoyun 2013-07-27
          Result := npos * 360; // Mon-kulou.wzl piaoyun 2013-07-27
      end;
    80:
      case npos of
        0:Result := 0;
        1:Result := 80;
        2:Result := 300;
        3:Result := 301;
        4:Result := 302;
        5:Result := 320;
        6:Result := 321;
        7:Result := 322;
        8:Result := 321;
      end;
    90:
      case npos of
        0:Result := 80;
        1:Result := 168;
        2:Result := 184;
        3:Result := 200;
        4:Result := 1770;
        5:Result := 1780;
        6:Result := 1790;
      end;
    else
      Result := npos * 360;
  end;
end;

function GetNpcOffset(nAppr:Integer):Integer;
begin
  if nAppr >= 2000 then
    nAppr := nAppr - 2000;

  case nAppr of
    24, 25:Result := (nAppr - 24) * 60 + 1470;
    0..22:Result := nAppr * 60;
    23:Result := 1380;
    27, 32:Result := (nAppr - 26) * 60 + 1620 - 30;
    26, 28, 29, 30, 31, 33..41:Result := (nAppr - 26) * 60 + 1620;
    42, 43:Result := 2580;
    44..47:Result := 2640;
    48..50:Result := (nAppr - 48) * 60 + 2700;
    51:Result := 2880;
    52:Result := 2960;
    54..58:Result := 4490 + (nAppr - 54) * 10;
    94..98:Result := 4490 + (nAppr - 94) * 10;
    59:Result := 4540;
    60..67:Result := 3060 + (nAppr - 60) * 60;
    68:Result := 3600;
    70..75:Result := 3780 + (nAppr - 70) * 10;
    76, 77:Result := 3840 + (nAppr - 76) * 60;
    78..80:Result := 4060 + (nAppr - 78) * 60;
    81..83:Result := 3960 + (nAppr - 81) * 20;
    84:Result := 4030;
    90..92:Result := 3750 + (nAppr - 90) * 10;

    99:Result := 4240;
    100:Result := 4560;
    101:Result := 4770;
    102:Result := 4810;

    200..207:Result := (nAppr - 200) * 70;
    208:Result := 630;
    209:Result := 700;
    210:Result := 740;
    211:Result := 810;
    212:Result := 820;
    213:Result := 830;
    214:Result := 840;
    215:Result := 850;
    216:Result := 860;
    217:Result := 870;
    218:Result := 900;
    219:Result := 930;
    220:Result := 970;
    221:Result := 980;
    222:Result := 990;
    223:Result := 1020;
    224:Result := 1030;
    225:Result := 1060;
    226:Result := 0;
    227:Result := 40;
    228:Result := 80;
    229:Result := 120;
    230:Result := 160;
    231:Result := 200;
    232:Result := 240;
    233:Result := 280;
    234:Result := 320;
    235:Result := 360;
    236:Result := 400;
    237:Result := 470;
    238:Result := 540;
    239:Result := 610;
    240:Result := 680;
    241:Result := 750;
    242:Result := 820;
    243:Result := 890;
    244:Result := 950;
    245:Result := 1010;
    246:Result := 0;
    247:Result := 90;
    248:Result := 180;
    249:Result := 270;
    250:Result := 360;
    251:Result := 450;
    252:Result := 540;
    253:Result := 710;
    254:Result := 800;
    255:Result := 890;
    256:Result := 980;
    257:Result := 1070;
    258:Result := 1160;
    259:Result := 1330;
    260:Result := 1420;
    261:Result := 1510;
    262:Result := 1600;
    263:Result := 1690;
    264:Result := 1860;
    265:Result := 2030;
    266:Result := 2120;
    267:Result := 2210;
    268:Result := 2300;
    269:Result := 2390;
    270:Result := 2560;
    271:Result := 2730;
    272:Result := 2820;
    else if nAppr >= 1000 then
      Result := (nAppr - 1000) * 60
    else if nAppr > 200 then
      Result := (nAppr - 200) * 70
    else
      Result := (nAppr - 200) * 60
  end;
end;

function PointRect(X, Y:Integer; SrcRect:TRect):TRect;
begin
  SrcRect.Left := SrcRect.Left + X;
  SrcRect.Top := SrcRect.Top + Y;
  SrcRect.Right := SrcRect.Right + X;
  SrcRect.Bottom := SrcRect.Bottom + Y;
  if SrcRect.Left < 0 then
    SrcRect.Left := 0;
  if SrcRect.Top < 0 then
    SrcRect.Top := 0;
  if SrcRect.Right > MAPSURFACEWIDTH then
    SrcRect.Right := MAPSURFACEWIDTH;
  if SrcRect.Bottom > MAPSURFACEHEIGHT then
    SrcRect.Bottom := MAPSURFACEHEIGHT;
  if SrcRect.Right < 0 then
    SrcRect.Right := 0;
  if SrcRect.Bottom < 0 then
    SrcRect.Bottom := 0;
  Result := SrcRect;
end;

constructor TActor.Create;
var
  I:Integer;
begin
  inherited Create;

  InitializeCriticalSection(m_HealthNumberLock);

  FillChar(m_Abil, SizeOf(TAbility), 0);
  FillChar(m_OAbil, SizeOf(TAbility), 0);
  m_Action := nil;
  // FillChar(m_Action, SizeOf(m_Action), 0);

  m_MsgList := TGList.Create;
  m_nRecogId := 0;
  m_BodySurface := nil;
  // m_BodyAlphaSurface := nil;
  m_nGold := 0;
  m_boVisible := True;
  m_boHoldPlace := True;

  m_dwCurrentActionTick := TimeGetTime;
  m_nCurrentAction := 0;
  m_boReverseFrame := False;
  m_nShiftX := 0;
  m_nShiftY := 0;
  m_boChangeEff := False;
  m_nDownDrawLevel := 0;
  m_nCurrentFrame := -1;
  m_nEffectFrame := -1;
  RealActionMsg.ident := 0;
  m_sUserName := '';
  //m_sShowUserName := '';
  m_nNameColor := clWhite;
  m_dwSendQueryUserNameTime := 0; // TimeGetTime;
  m_boWarMode := False;
  m_dwWarModeTime := 0; // War mode
  m_boCustomMagicNoAction := False;
  m_boDeath := False;
  m_dwDeathTick := TimeGetTime;
  m_boSkeleton := False;
  m_boFreeActor := True;
  m_boDelActor := False;
  m_boDelActionAfterFinished := False;

  m_nChrLight := 0;
  m_nMagLight := 0;
  m_boLockEndFrame := False;
  m_dwSmoothMoveTime := 0; // TimeGetTime;
  m_dwGenAnicountTime := 0;
  m_dwGenNewHitAnicountTime := 0;
  m_dwGenNewMagAnicountTime := 0;
  m_dwDefFrameTime := 0;
  m_dwLoadSurfaceTime := 0;
  m_boGrouped := False;
  m_boOpenHealth := False;
  m_noInstanceOpenHealth := False;
  m_CurMagic.ServerMagicCode := 0;
  // CurMagic.MagicSerial := 0;

  m_nSpellFrame := DEFSPELLFRAME;

  m_nNormalSound := -1;
  m_nFootStepSound := -1;
  m_nAttackSound := -1;
  m_nWeaponSound := -1;
  m_nStruckSound := s_struck_body_longstick;
  m_nStruckWeaponSound := -1;
  m_nScreamSound := -1;
  m_nDieSound := -1;
  m_nDie2Sound := -1;

  m_WeaponEffectSurface := nil;
  m_DBWeaponEffectSurface := nil;
  m_DressEffectSurface := nil;
  m_MedalEffectSurface := nil;

  m_ShieldEffectSurface := nil;
  m_OldColorEffect := ceNone;
  m_ColorEffect := ceNone;
  m_boGhost := False;

  m_dwMoveTime := timeGetTime;
  m_nMoveStepCount := 0;
  m_boCanMove := False;

  m_nMoveSpeed := 0; // 行走速度
  m_nAttackSpeed := 0; // 攻击速度
  m_nSpellSpeed := 0; // 魔法速度

  m_sNameText := '';
  m_sCurNameText := '';

  FillChar(m_HealthNumberArray, SizeOf(m_HealthNumberArray), 0);

  FillChar(m_NumberFramIndex, SizeOf(m_NumberFramIndex), 0);

  m_boShopStall := False;
  m_btBodyColor := 0; // 人体颜色
  FillChar(m_ActorIconIndexs, SizeOf(m_ActorIconIndexs), 0);
  FillChar(m_ActorIcons, SizeOf(m_ActorIcons), 0);
  for I := Low(TActorIconArray) to High(TActorIconArray) do begin
    m_ActorIcons[I].nFileIndex := -1;
    m_ActorIconIndexs[I].Texture := nil;
  end;

  m_ActorEffects := TGList.Create;
  m_dwShowShopNameTimeTick := MyGetTickCount;
  m_sNumberLableText := '';
  m_sCurNumberLableText := '';

  m_ShowNumberLableTimeTick := MyGetTickCount;
  m_NameTextSurface := nil;
  m_NumberLableImageInfo.Width := 0;
  m_NumberLableImageInfo.Height := 0;
  m_NumberLableImageInfo.ImageIndexs := nil;
  m_ShopNameImageInfo.Width := 0;
  m_ShopNameImageInfo.Height := 0;
  m_ShopNameImageInfo.ImageIndexs := nil;

  m_FengHaoEffectSurface := nil;
  SetLength(m_FengHaoImageInfo, 0);
  m_nOldFengHaoSurfaceID := 0;
  m_dwLoadFengHaoSurfaceTime := 0;

  m_sCurShopNameText := '';
  m_sShopNameText := '';

  m_nHitEffectLevel := 0;
  m_nHitEffectLevel2 := 0;

  m_boShowHealthNumber := False;

  m_HearMsgColor := clWhite;

  m_boLoadSurface := False;

  m_boCobweb := False; // 网罩住了
  m_UseEffectImage := nil;
  m_boStruckShowNumber := False;
  m_boShowBigHPProgress := False;
  m_boSendQueryBigHPProgress := False;
  m_boDuanJin := False;

  m_boShowPhantom := False; // 绘制放大的人物效果
  m_btPhantomAlpha := 120; // 放大的人物透明度

  m_boDingShen := False; // 定身 chongchong 2015-03-12
  m_boTanHuan := False; // 瘫痪 chongchong 2015-03-12
  m_boThunderPalsy := False;

  FillChar(m_CustomMagicStatusEffect, SizeOf(TCustomMagicStatusEffect), 0);
  FillChar(m_SelfKeepPlay, Sizeof(m_SelfKeepPlay), 0);

  m_boCreateEffect := False;

  m_boSelfEffectRunning := False; // 是否显示自身动画
  m_boSelfEffectBlendDraw := True;
  m_nSelfEffectCurrentFrame := 0; // 当前帧
  m_nSelfEffectEndFrame := 0; // 自身动画最后帧数
  m_nSelfEffectFrameTime := 0; // 自身动画时间间隔
  m_SelfEffectGameImage := nil; // 自身动画所在图库
  m_dwSelfEffectLastTick := TimeGetTime; // 上一帧时间

  m_IsExploreItem := False;
  m_nChangeAppr := -1;
end;

destructor TActor.Destroy;
var
  I:Integer;
  Msg:pTChrMsg;
begin
  for I := 0 to m_MsgList.Count - 1 do begin
    Msg := m_MsgList.Items[I];
    Dispose(Msg);
  end;
  m_MsgList.Free;

  for I := 0 to m_ActorEffects.Count - 1 do begin
    Dispose(pTClientActorEffect(m_ActorEffects.Items[I]));
  end;
  m_ActorEffects.Free;

  if m_NameTextSurface <> nil then begin
    m_NameTextSurface.Free;
    m_NameTextSurface := nil;
  end;

  DeleteCriticalSection(m_HealthNumberLock);
  inherited Destroy;
end;

procedure TActor.SendMsg(wIdent:Word; nX, nY, ndir:Integer; nState:Int64; sStr:string; nSound:Integer);
begin
  SendMsg(wIdent, nX, nY, ndir, m_Feature, nState, sStr, nSound);
end;

function TActor.UpdateMsg(wIdent:Word; nX, nY, ndir:Integer; nState:Int64; sStr:string; nSound:Integer; IsScreenSync:Boolean):Boolean;
begin
  Result := UpdateMsg(wIdent, nX, nY, ndir, m_Feature, nState, sStr, nSound, IsScreenSync);
end;

{$IF DEBUG_HIT_DELAY = 1}
var
  LastSendMsgHitTick:LongWord;
  {$IFEND}

procedure TActor.SendMsg(wIdent:Word; nX, nY, ndir:Integer; Feature:TFeature; nState:Int64; sStr:string; nSound:Integer);
var
  Msg:pTChrMsg;
begin
  New(Msg);
  Msg.ident := wIdent;
  Msg.X := nX;
  Msg.Y := nY;
  Msg.dir := ndir;
  Msg.Feature := Feature;
  Msg.State := nState;
  Msg.saying := sStr;
  Msg.sound := nSound;

  m_MsgList.Lock;
  try
    //if Msg.Ident = SM_MAGICFIRE then begin
      //DScreen.AddChatBoardString('~~~~~~~~~~~~~~~~~~~~~~~~ADD  SM_MAGICFIRE', clRed, clBlack);
    //end;
    if Msg.Ident = SM_SPELL then begin
       //DScreen.AddChatBoardString('ADD SM_SPELL', clRed, clBlack);
    end;

    m_MsgList.Add(Msg);
  finally
    m_MsgList.UnLock;
  end;

  {$IF DEBUG_HIT_DELAY = 1}
  // 修改测试chonchong 2016-12-09
  if (wIdent = CM_HIT) or
    (wIdent = CM_HEAVYHIT) or
    (wIdent = CM_BIGHIT) or
    (wIdent = CM_POWERHIT) or
    (wIdent = CM_LONGHIT) or
    (wIdent = CM_WIDEHIT) or
    (wIdent = CM_FIREHIT) or
    (wIdent = CM_CRSHIT) or
    (wIdent = CM_TWNHIT) or
    (wIdent = CM_SWORDHIT) or
    (wIdent = CM_43HIT) or
    (wIdent = CM_43HIT) or
    (wIdent = CM_66HIT) or
    (wIdent = CM_66HIT1) or
    (wIdent = CM_101HIT) or
    (wIdent = CM_102HIT) or
    (wIdent = CM_103HIT) or
    (wIdent = CM_113HIT) or
    (wIdent = CM_115HIT) or
    ((wIdent >= CM_CUSTOM_HIT1) and (wIdent <= CM_CUSTOM_HIT100)) then begin
    DScreen.AddChatBoardString('消息处理间隔:' + IntToStr(TimeGetTime - LastSendMsgHitTick), clWhite, clBlack);
    LastSendMsgHitTick := TimeGetTime;
  end;
  {$IFEND}
end;

function TActor.FindMsg(wIdent:Word):Boolean;
var
  I:Integer;
  Msg:pTChrMsg;
begin
  Result := False;
  m_MsgList.Lock;
  try
    for I := 0 to m_MsgList.Count - 1 do begin
      Msg := m_MsgList.Items[I];
      if (Msg.ident = wIdent) then begin
        Result := True;
        Break;
      end;
    end;
  finally
    m_MsgList.UnLock;
  end;
end;

// 组合速度控制  chongchong 2017-01-22

function CheckSendIdent(Ident:Word; MagicID:Word):Boolean;
{$J+}
const
  LastAction:TBaseAction = baOther;
const
  LastSendTick:array[TAntiPlugActionMode] of LongWord = (
    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
    0, 0, 0, 0, 0, 0, 0, 0, 0);
  {$J-}
var
  CurTick:LongWord;
  nSpeed:Integer;
  CustomMagicConfig:PClientCustomMagicConfig;
begin
  Result := True;
  CurTick := timeGetTime;

  case Ident of
    CM_WALK:begin
        nSpeed := g_MySelf.m_nMoveSpeed;
        if nSpeed <= -HALF_SPEED_INTERVALS_COUNT then
          nSpeed := 0
        else if nSpeed >= HALF_SPEED_INTERVALS_COUNT then
          nSpeed := SPEED_INTERVALS_COUNT - 1
        else
          nSpeed := HALF_SPEED_INTERVALS_COUNT + nSpeed;

        // 攻击到走路
        if (LastAction = baHit) then begin
          if g_wActionSpeedIntervals[amHitToWalk][nSpeed] > 0 then begin
            Result := (CurTick - LastSendTick[amHit]) > g_wActionSpeedIntervals[amHitToWalk][nSpeed];
          end;
        end

          // 魔法到走路
        else if (LastAction = baSpell) then begin
          if g_wActionSpeedIntervals[amSpellToWalk][nSpeed] > 0 then begin
            Result := (CurTick - LastSendTick[amSpell]) > g_wActionSpeedIntervals[amSpellToWalk][nSpeed];
          end;
        end

          // 转身到移动
        else if (LastAction = baTurn) then begin
          if g_wActionSpeedIntervals[amTurnToMove][nSpeed] > 0 then begin
            Result := (CurTick - LastSendTick[amTurn]) > g_wActionSpeedIntervals[amTurnToMove][nSpeed];
          end;
        end

          // 挖肉到移动
        else if (LastAction = baCutMeat) then begin
          if g_wActionSpeedIntervals[amCutMeatToMove][nSpeed] > 0 then begin
            Result := (CurTick - LastSendTick[amCutMeat]) > g_wActionSpeedIntervals[amCutMeatToMove][nSpeed];
          end;
        end;

        if Result then begin
          LastSendTick[amWalk] := CurTick;
          LastAction := baWalk;
        end;
      end;
    CM_RUN:begin
        nSpeed := g_MySelf.m_nMoveSpeed;
        if nSpeed <= -HALF_SPEED_INTERVALS_COUNT then
          nSpeed := 0
        else if nSpeed >= HALF_SPEED_INTERVALS_COUNT then
          nSpeed := SPEED_INTERVALS_COUNT - 1
        else
          nSpeed := HALF_SPEED_INTERVALS_COUNT + nSpeed;

        // 攻击到跑步
        if (LastAction = baHit) then begin
          if g_wActionSpeedIntervals[amHitToRun][nSpeed] > 0 then begin
            Result := (CurTick - LastSendTick[amHit]) > g_wActionSpeedIntervals[amHitToRun][nSpeed];
          end;
        end

          // 魔法到跑步
        else if (LastAction = baSpell) then begin
          if g_wActionSpeedIntervals[amSpellToRun][nSpeed] > 0 then begin
            Result := (CurTick - LastSendTick[amSpell]) > g_wActionSpeedIntervals[amSpellToRun][nSpeed];
          end;
        end
          // 转身到移动
        else if (LastAction = baTurn) then begin
          if g_wActionSpeedIntervals[amTurnToMove][nSpeed] > 0 then begin
            Result := (CurTick - LastSendTick[amTurn]) > g_wActionSpeedIntervals[amTurnToMove][nSpeed];
          end;
        end

          // 挖肉到移动
        else if (LastAction = baCutMeat) then begin
          if g_wActionSpeedIntervals[amCutMeatToMove][nSpeed] > 0 then begin
            Result := (CurTick - LastSendTick[amCutMeat]) > g_wActionSpeedIntervals[amCutMeatToMove][nSpeed];
          end;
        end;

        if Result then begin
          LastSendTick[amRun] := CurTick;
          LastAction := baRun;
        end;
      end;
    CM_TURN:begin
        {
        // 攻击到转向
        if (LastAction = baHit) then
        begin

        end

        // 魔法到转向
        else if (LastAction = baSpell) then
        begin

        end

        // 移动到转向
        else if (LastAction in [baWalk, baRun]) then
        begin

        end;
        }
        LastSendTick[amTurn] := CurTick;
        LastAction := baTurn;
      end;
    CM_HIT, CM_HEAVYHIT, CM_BIGHIT, CM_POWERHIT, CM_LONGHIT, CM_WIDEHIT,
      CM_FIREHIT, CM_CRSHIT, CM_TWNHIT, CM_SWORDHIT,
      CM_43HIT, CM_66HIT, CM_66HIT1, CM_101HIT, CM_102HIT, CM_103HIT, CM_113HIT, CM_115HIT,
      CM_CUSTOM_HIT001..(CM_CUSTOM_HIT001 + CUSTOM_MAGIC_COUNT - 1):begin
        nSpeed := g_MySelf.m_nAttackSpeed;
        if nSpeed <= -HALF_SPEED_INTERVALS_COUNT then
          nSpeed := 0
        else if nSpeed >= HALF_SPEED_INTERVALS_COUNT then
          nSpeed := SPEED_INTERVALS_COUNT - 1
        else
          nSpeed := HALF_SPEED_INTERVALS_COUNT + nSpeed;

        // 走路到攻击
        if (LastAction = baWalk) then begin
          if g_wActionSpeedIntervals[amWalkToHit][nSpeed] > 0 then begin
            Result := (CurTick - LastSendTick[amWalk]) > g_wActionSpeedIntervals[amWalkToHit][nSpeed];
          end;
        end else if (LastAction = baRun) then begin // 跑步到攻击
          if g_wActionSpeedIntervals[amRunToHit][nSpeed] > 0 then begin
            Result := (CurTick - LastSendTick[amRun]) > g_wActionSpeedIntervals[amRunToHit][nSpeed];
          end;
        end else if (LastAction = baTurn) then begin // 转向到攻击
          if g_wActionSpeedIntervals[amTurnToHit][nSpeed] > 0 then begin
            Result := (CurTick - LastSendTick[amTurn]) > g_wActionSpeedIntervals[amTurnToHit][nSpeed];
          end;
        end else if (LastAction = baCutMeat) then begin // 挖肉到攻击
          if g_wActionSpeedIntervals[amCutMeatToHit][nSpeed] > 0 then begin
            Result := (CurTick - LastSendTick[amCutMeat]) > g_wActionSpeedIntervals[amCutMeatToHit][nSpeed];
          end;
        end else if (LastAction = baHit) then begin
          Result := (CurTick - LastSendTick[amHit]) > Cardinal(g_MySelf.GetNextHitTime);
        end;

        if Result then begin
          LastSendTick[amHit] := CurTick;
          LastAction := baHit;
        end;
      end;
    CM_SPELL:begin
        nSpeed := g_MySelf.m_nSpellSpeed;
        if nSpeed <= -HALF_SPEED_INTERVALS_COUNT then
          nSpeed := 0
        else if nSpeed >= HALF_SPEED_INTERVALS_COUNT then
          nSpeed := SPEED_INTERVALS_COUNT - 1
        else
          nSpeed := HALF_SPEED_INTERVALS_COUNT + nSpeed;

        if MagicID in [7 {攻杀}, 12 {刺杀}, 25 {半月}, 26 {烈火}, 40 {双龙斩}, 42 {龙影}, 43 {雷霆剑法}, 56 {逐日剑法}, 66 {开天斩}, 113 {断空斩}, 115 {血魄一击}] then begin
          Exit;
        end;

        if CheckIsCustomMagic(MagicID) then begin
          CustomMagicConfig := GetCustomMagicConfig(MagicID);
          if (CustomMagicConfig <> nil) and (CustomMagicConfig.MagicBaseConfig.MagicSwitchMode <> msmNone) then begin
            Exit;
          end;
        end;

        // 走路到魔法
        if (LastAction = baWalk) then begin
          if g_wActionSpeedIntervals[amWalkToSpell][nSpeed] > 0 then begin
            Result := (CurTick - LastSendTick[amWalk]) > g_wActionSpeedIntervals[amWalkToSpell][nSpeed];
          end;
        end

          // 跑步到魔法
        else if (LastAction = baRun) then begin
          if g_wActionSpeedIntervals[amRunToSpell][nSpeed] > 0 then begin
            Result := (CurTick - LastSendTick[amRun]) > g_wActionSpeedIntervals[amRunToSpell][nSpeed];
          end;
        end

          // 转向到魔法
        else if (LastAction = baTurn) then begin
          if g_wActionSpeedIntervals[amTurnToSpell][nSpeed] > 0 then begin
            Result := (CurTick - LastSendTick[amTurn]) > g_wActionSpeedIntervals[amTurnToSpell][nSpeed];
          end;
        end

          // 挖肉到魔法
        else if (LastAction = baCutMeat) then begin
          if g_wActionSpeedIntervals[amCutMeatToSpell][nSpeed] > 0 then begin
            Result := (CurTick - LastSendTick[amCutMeat]) > g_wActionSpeedIntervals[amCutMeatToSpell][nSpeed];
          end;
        end;

        if Result then begin
          LastSendTick[amSpell] := CurTick;
          LastAction := baSpell;
        end;
      end;
    CM_SITDOWN:begin
        {
        // 移动到挖肉
        if (LastAction in [baWalk, baRun]) then
        begin
        end;
        }

        LastSendTick[amCutMeat] := CurTick;
        LastAction := baCutMeat;
      end
    else begin
        LastAction := baOther;
      end;
  end;
end;

function TActor.UpdateMsg(wIdent:Word; nX, nY, ndir:Integer; Feature:TFeature; nState:Int64; sStr:string; nSound:Integer; IsScreenSync:Boolean):Boolean;
var
  I:Integer;
  Msg:pTChrMsg;
begin
  if Self = g_MySelf then begin
    if not IsScreenSync then begin
      if not CheckSendIdent(wIdent, ndir) then begin
        Result := False;
        Exit;
      end;
    end; 
  end;


  m_MsgList.Lock;
  try
    for I := m_MsgList.Count - 1 downto 0 do begin
      Msg := m_MsgList.Items[I];
      if ((Self = g_MySelf) and ((Msg.ident >= SM_ACTION_MIN) and (Msg.ident <= SM_ACTION_MAX) or (Msg.ident = SM_STRUCK))) or (Msg.ident = wIdent) then begin
        Dispose(Msg);
        m_MsgList.Delete(I);
        Continue;
      end;
    end;
  finally
    m_MsgList.UnLock;
  end;  

  SendMsg(wIdent, nX, nY, ndir, Feature, nState, sStr, nSound);
  Result := True;

end;

procedure TActor.UpdateStruckMsg(wIdent:Word; nX, nY, ndir:Integer; Feature:TFeature; nState:Int64; sStr:string; nSound:Integer);
var
  I:Integer;
  Msg:pTChrMsg;
begin
  m_MsgList.Lock;
  try
    for I := m_MsgList.Count - 1 downto 0 do begin
      Msg := m_MsgList.Items[I];
      if (Msg.ident = wIdent) then begin
        Dispose(Msg);
        m_MsgList.Delete(I);
        Continue;
      end;
    end;
  finally
    m_MsgList.UnLock;
  end;
  SendMsg(wIdent, nX, nY, ndir, Feature, nState, sStr, nSound);
end;

procedure TActor.CleanStruckMsg;
var
  I:Integer;
  Msg:pTChrMsg;
begin
  m_MsgList.Lock;
  try
    for I := m_MsgList.Count - 1 downto 0 do begin
      Msg := m_MsgList.Items[I];
      if (Msg.Ident = SM_STRUCK) then begin
        m_MsgList.Delete(I);
        Dispose(Msg);
        // DScreen.AddChatBoardString('CleanStruckMsg '+m_sUserName+' '+IntToStr(m_nCurrentAction), clRed, clWhite);
      end;
    end;
  finally
    m_MsgList.UnLock;
  end;
end;

procedure TActor.CleanMsgs;
var
  I:Integer;
  Msg:pTChrMsg;
begin
  m_MsgList.Lock;
  try
    for I := m_MsgList.Count - 1 downto 0 do begin
      Msg := m_MsgList.Items[I];
      if (Msg.Ident <> SM_SPACEMOVE_SHOW) and (Msg.Ident <> SM_SPACEMOVE_SHOW2) then begin
        Dispose(Msg);
        m_MsgList.Delete(I);
      end;
      // end;
    end;
    //m_MsgList.Clear;
  finally
    m_MsgList.UnLock;
  end;
end;

procedure TActor.DeleteMsg(wIdent:Word);
var
  I:Integer;
  Msg:pTChrMsg;
begin
  m_MsgList.Lock;
  try
    for I := m_MsgList.Count - 1 downto 0 do begin
      Msg := m_MsgList.Items[I];
      if (Msg.Ident = wIdent) then begin
        m_MsgList.Delete(I);
        Dispose(Msg);
      end;
    end;
    // m_MsgList.Clear;
  finally
    m_MsgList.UnLock;
  end;
end;
{
procedure TActor.CleanUserMsgs;
var
  I: Integer;
  Msg: pTChrMsg;
begin
  m_MsgList.Lock;
  try
    for I := m_MsgList.Count - 1 downto 0 do
    begin
      Msg := m_MsgList.Items[I];
      if (Msg.ident >= 1) and
        (Msg.ident <= 47) then
      begin
        m_MsgList.Delete(I);
        Dispose(Msg);
        Continue;
      end;
    end;
  finally
    m_MsgList.UnLock;
  end;
end; }

// 清除消息号在[3000,3099]之间的消息 -- piaoyun 2013-07-13

procedure TActor.CleanUserMsgs;
var
  I:Integer;
  Msg:pTChrMsg;
begin
  m_MsgList.Lock;
  try
    for I := m_MsgList.Count - 1 downto 0 do begin
      Msg := m_MsgList.Items[I];

      {if (Msg.Ident >= 3000) and // 基本运动消息：走、跑等
         (Msg.Ident <= 3099) then begin }

      if ((Msg.Ident > 2999) and // 基本运动消息：走、跑等
        (Msg.Ident <> CM_100HIT) and
        (Msg.Ident <> CM_101HIT) and
        (Msg.Ident <> CM_102HIT) and
        (Msg.Ident <> CM_103HIT) and
        (Msg.Ident <> CM_103HIT + 1)) or // 排除连击技能

      // 修复攻击有时候会卡 chongchong 2015-06-25
      // TfrmMain.ActionFailed 时，可能数据没清完，导致TfrmMain.AttackTarget失败
      (Msg.Ident = SM_HIT) or
        (Msg.Ident = SM_HEAVYHIT) or
        (Msg.Ident = SM_POWERHIT) or
        (Msg.Ident = SM_LONGHIT) or
        (Msg.Ident = SM_WIDEHIT) or
        (Msg.Ident = SM_BIGHIT) or
        (Msg.Ident = SM_60HIT) or
        (Msg.Ident = SM_61HIT) or
        (Msg.Ident = SM_62HIT) or
        (Msg.Ident = SM_66HIT) or
        (Msg.Ident = SM_TWNHIT) or
        (Msg.Ident = SM_CRSHIT) or
        (Msg.Ident = SM_43HIT) or
        (Msg.Ident = SM_SWORDHIT) or
        ((Msg.Ident >= SM_CUSTOM_HIT001) and (Msg.Ident < SM_CUSTOM_HIT001 + CUSTOM_MAGIC_COUNT)) then begin
        Dispose(Msg);
        m_MsgList.Delete(I);
      end;
    end;
  finally
    m_MsgList.UnLock;
  end;
end;

procedure TActor.CalcActorFrame;
var
  bofly:Boolean;
  MonsterConfig, TempMonsterConfig:PClientCustomMonsterConfig;
  ClientAction:PMonsterClientAction;
  I, TempDir:Integer;
begin
  m_boUseMagic := False;
  m_nCurrentFrame := -1;
  if (m_btRace in [156]) and (m_nChangeAppr >= 0) then begin
    MonsterConfig := nil;

    for I := 0 to g_CustomMonsterConfig.Count - 1 do begin
      TempMonsterConfig := g_CustomMonsterConfig.Items[I];
      if TempMonsterConfig.wMonsterAppr = m_nChangeAppr then begin
        MonsterConfig := TempMonsterConfig;
        Break;
      end;
    end;

    if MonsterConfig <> nil then begin
      m_nCurrentFrame := -1;

      case m_nCurrentAction of
        0, SM_TURN:begin
            if (m_nState and STATE_STONE_MODE) <> 0 then begin
              ClientAction := @MonsterConfig.Actions[matStoneRevive];

              if ClientAction.CalcDir then
                TempDir := m_btDir
              else
                TempDir := 0;
              m_nStartFrame := ClientAction.StartIndex + TempDir * (ClientAction.PlayCount + ClientAction.EmptyCount);
              m_nEndFrame := m_nStartFrame;
              m_dwFrameTime := ClientAction.PlayTime;
              m_dwStartTime := TimeGetTime;
              m_StartCounter := timeGetTime;
              m_nDefFrameCount := ClientAction.PlayCount;
            end
            else begin
              ClientAction := @MonsterConfig.Actions[matStand];

              if ClientAction.CalcDir then
                TempDir := m_btDir
              else
                TempDir := 0;
              m_nStartFrame := ClientAction.StartIndex + TempDir * (ClientAction.PlayCount + ClientAction.EmptyCount);
              m_nEndFrame := m_nStartFrame + ClientAction.PlayCount - 1;
              m_dwFrameTime := ClientAction.PlayTime;
              m_dwStartTime := TimeGetTime;
              m_StartCounter := timeGetTime;
              m_nDefFrameCount := ClientAction.PlayCount;
            end;
            Shift(m_btDir, 0, 0, 1);
          end;
        SM_WALK, SM_RUSH, SM_RUSHKUNG, SM_BACKSTEP:begin
            ClientAction := @MonsterConfig.Actions[matWalk];

            if ClientAction.CalcDir then
              TempDir := m_btDir
            else
              TempDir := 0;
            m_nStartFrame := ClientAction.StartIndex + TempDir * (ClientAction.PlayCount + ClientAction.EmptyCount);
            m_nEndFrame := m_nStartFrame + ClientAction.PlayCount - 1;
            m_dwFrameTime := ClientAction.PlayTime;
            m_dwStartTime := TimeGetTime;
            m_StartCounter := timeGetTime;
            m_nMaxTick := HA.ActWalk.usetick;
            m_nCurTick := 0;
            m_nMoveStep := 1;

            if m_nCurrentAction = SM_BACKSTEP then begin
              m_nMoveStep := m_btStep;
              Shift(GetBack(m_btDir), m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1)
            end
            else
              Shift(m_btDir, m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1);
          end;
        SM_DIGUP:begin
            ClientAction := @MonsterConfig.Actions[matStoneRevive];

            if ClientAction.CalcDir then
              TempDir := m_btDir
            else
              TempDir := 0;
            m_nStartFrame := ClientAction.StartIndex + TempDir * (ClientAction.PlayCount + ClientAction.EmptyCount);
            m_nEndFrame := m_nStartFrame + ClientAction.PlayCount - 1;
            m_dwFrameTime := ClientAction.PlayTime;
            m_dwStartTime := TimeGetTime;
            m_StartCounter := timeGetTime;
            m_nMaxTick := 0;
            m_nCurTick := 0;
            m_nDefFrameCount := ClientAction.PlayCount;

            m_nState := 0;

            Shift(m_btDir, 0, 0, 1);
          end;
        SM_LIGHTINGEX:begin

          end;
        SM_HIT:begin
            ClientAction := @MonsterConfig.Actions[matDefAttack];

            if ClientAction.CalcDir then
              TempDir := m_btDir
            else
              TempDir := 0;
            m_nStartFrame := ClientAction.StartIndex + TempDir * (ClientAction.PlayCount + ClientAction.EmptyCount);
            m_nEndFrame := m_nStartFrame + ClientAction.PlayCount - 1;
            m_dwFrameTime := ClientAction.PlayTime;
            m_dwStartTime := TimeGetTime;
            m_dwWarModeTime := TimeGetTime;
            Shift(m_btDir, 0, 0, 1);
          end;
        SM_STRUCK:begin
            ClientAction := @MonsterConfig.Actions[matStruck];

            if ClientAction.CalcDir then
              TempDir := m_btDir
            else
              TempDir := 0;
            m_nStartFrame := ClientAction.StartIndex + TempDir * (ClientAction.PlayCount + ClientAction.EmptyCount);
            m_nEndFrame := m_nStartFrame + ClientAction.PlayCount - 1;
            m_dwFrameTime := m_dwStruckFrameTime;
            m_dwStartTime := TimeGetTime;
            Shift(m_btDir, 0, 0, 1);

            m_CustomMagicStatusEffect.m_nStruck := 0;
          end;
        SM_DEATH:begin
            ClientAction := @MonsterConfig.Actions[matDie];

            if ClientAction.CalcDir then
              TempDir := m_btDir
            else
              TempDir := 0;
            m_nStartFrame := ClientAction.StartIndex + TempDir * (ClientAction.PlayCount + ClientAction.EmptyCount) + ClientAction.PlayCount - 1;
            m_nEndFrame := m_nStartFrame;
            m_dwFrameTime := ClientAction.PlayTime;
            m_dwStartTime := TimeGetTime;
          end;
        SM_NOWDEATH:begin
            ClientAction := @MonsterConfig.Actions[matDie];

            if ClientAction.CalcDir then
              TempDir := m_btDir
            else
              TempDir := 0;
            m_nStartFrame := ClientAction.StartIndex + TempDir * (ClientAction.PlayCount + ClientAction.EmptyCount);
            m_nEndFrame := m_nStartFrame + ClientAction.PlayCount - 1;
            m_dwFrameTime := ClientAction.PlayTime;
            m_dwStartTime := TimeGetTime;
          end;
        SM_SKELETON:begin

          end;
      end;
    end;

  end
  else begin
    m_nBodyOffset := GetOffset(m_wAppearance);
    m_Action := GetRaceByPM(m_btRace, m_wAppearance);
    if m_Action = nil then Exit;

    case m_nCurrentAction of
      SM_TURN:begin
          m_nStartFrame := m_Action.ActStand.start + m_btDir * (m_Action.ActStand.frame + m_Action.ActStand.skip);
          m_nEndFrame := m_nStartFrame + m_Action.ActStand.frame - 1;
          m_dwFrameTime := m_Action.ActStand.ftime;
          m_dwStartTime := TimeGetTime;
          m_StartCounter := timeGetTime;
          m_nDefFrameCount := m_Action.ActStand.frame;
          Shift(m_btDir, 0, 0, 1);
        end;
      SM_WALK, SM_RUSH, SM_RUSHKUNG, SM_BACKSTEP:begin
          m_nStartFrame := m_Action.ActWalk.start + m_btDir * (m_Action.ActWalk.frame + m_Action.ActWalk.skip);
          m_nEndFrame := m_nStartFrame + m_Action.ActWalk.frame - 1;
          m_dwFrameTime := m_Action.ActWalk.ftime;
          m_dwStartTime := TimeGetTime;
          m_StartCounter := timeGetTime;
          m_nMaxTick := m_Action.ActWalk.usetick;
          m_nCurTick := 0;
          m_nMoveStep := 1;
          if m_nCurrentAction = SM_BACKSTEP then begin
            m_nMoveStep := m_btStep;
            Shift(GetBack(m_btDir), m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1)
          end
          else
            Shift(m_btDir, m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1);
        end;
      (*
      SM_BACKSTEP:
         begin
            startframe := pm.ActWalk.start + (pm.ActWalk.frame - 1) + Dir * (pm.ActWalk.frame + pm.ActWalk.skip);
            m_nEndFrame := startframe - (pm.ActWalk.frame - 1);
            m_dwFrameTime := pm.ActWalk.ftime;
            m_dwStartTime := TimeGetTime;
            m_nMaxTick := pm.ActWalk.UseTick;
            m_nCurTick := 0;
            m_nMoveStep := 1;
            Shift (GetBack(Dir), m_nMoveStep, 0, m_nEndFrame-startframe+1);
         end;
      *)
      SM_LIGHTINGEX:begin
          m_nStartFrame := m_Action.ActAttack.start + m_btDir * (m_Action.ActAttack.frame + m_Action.ActAttack.skip);
          m_nEndFrame := m_nStartFrame + m_Action.ActAttack.frame - 1;
          m_dwFrameTime := m_Action.ActAttack.ftime;
          m_dwStartTime := TimeGetTime;
          m_StartCounter := timeGetTime;
          // WarMode := TRUE;
          m_dwWarModeTime := TimeGetTime;
          Shift(m_btDir, 0, 0, 1);

          if m_nMagicNum > 0 then begin
            SetMagicSound(m_nMagicNum);
            PlayScene.NewMagic(Self,
              111,
              m_nEffectNum, // Effect
              m_nCurrX,
              m_nCurrY,
              m_nTargetX,
              m_nTargetY,
              m_nTargetRecog,
              m_MagicType, // EffectType
              True,
              0,
              bofly);
            if bofly then
              g_PlaySound.PlaySound(m_nMagicFireSound)
            else
              g_PlaySound.PlaySound(m_nMagicExplosionSound);
          end;
        end;
      SM_HIT:begin
          m_nStartFrame := m_Action.ActAttack.start + m_btDir * (m_Action.ActAttack.frame + m_Action.ActAttack.skip);
          m_nEndFrame := m_nStartFrame + m_Action.ActAttack.frame - 1;
          m_dwFrameTime := m_Action.ActAttack.ftime;
          m_dwStartTime := TimeGetTime;
          m_StartCounter := timeGetTime;
          // WarMode := TRUE;
          m_dwWarModeTime := TimeGetTime;
          Shift(m_btDir, 0, 0, 1);
        end;
      SM_STRUCK:begin
          m_nStartFrame := m_Action.ActStruck.start + m_btDir * (m_Action.ActStruck.frame + m_Action.ActStruck.skip);
          m_nEndFrame := m_nStartFrame + m_Action.ActStruck.frame - 1;
          m_dwFrameTime := m_dwStruckFrameTime; // pm.ActStruck.ftime;
          m_dwStartTime := TimeGetTime;
          m_StartCounter := timeGetTime;
          Shift(m_btDir, 0, 0, 1);

          m_CustomMagicStatusEffect.m_nStruck := 0;
        end;
      SM_DEATH:begin
          m_nStartFrame := m_Action.ActDie.start + m_btDir * (m_Action.ActDie.frame + m_Action.ActDie.skip);
          m_nEndFrame := m_nStartFrame + m_Action.ActDie.frame - 1;
          m_nStartFrame := m_nEndFrame; //
          m_dwFrameTime := m_Action.ActDie.ftime;
          m_dwStartTime := TimeGetTime;
          m_StartCounter := timeGetTime;
        end;
      SM_NOWDEATH:begin
          m_nStartFrame := m_Action.ActDie.start + m_btDir * (m_Action.ActDie.frame + m_Action.ActDie.skip);
          m_nEndFrame := m_nStartFrame + m_Action.ActDie.frame - 1;
          m_dwFrameTime := m_Action.ActDie.ftime;
          m_dwStartTime := TimeGetTime;
          m_StartCounter := timeGetTime;
        end;
      SM_SKELETON:begin
          m_nStartFrame := m_Action.ActDeath.start + m_btDir;
          m_nEndFrame := m_nStartFrame + m_Action.ActDeath.frame - 1;
          m_dwFrameTime := m_Action.ActDeath.ftime;
          m_dwStartTime := TimeGetTime;
          m_StartCounter := timeGetTime;
        end;
    end;
  end;
  {$IF Enabled_PlugEngine = 1}
  if Assigned(HookTActor_CalcActorFrame) then begin
    try
      HookTActor_CalcActorFrame(Self);
    except
      on E:Exception do begin
        DebugOutStr('[Exception] HookTActor_CalcActorFrame');
        DebugOutStr(E.Message);
      end;
    end;
  end;
  {$IFEND}

end;

procedure TActor.ReadyAction(Msg:TChrMsg);
var
  n:Integer;
  UseMagic:PTUseMagicInfo;
begin
  m_nActBeforeX := m_nCurrX;
  m_nActBeforeY := m_nCurrY;
  {
   if Msg.ident = SM_ALIVE then begin
     if m_boDeath and(Self = g_MySelf) then begin
       g_boCanDrawTileMap := True;
       g_BassSound.Clear;
     end;
     m_boDeath := False;
     m_boSkeleton := False;
   end;
   }
  if (Msg.ident = SM_ALIVE) then begin
    m_boDeath := False;
    m_boSkeleton := False;
    // m_boStruckShowNumber:= False;
    if (Self = g_MySelf) then begin
      g_boCanDrawTileMap := True;
      g_BassSound.Clear;
    end;
  end;

  //if (not m_boDeath) {or ((m_wAppearance >= 901) and (m_wAppearance <= 906))} then
  // 修复魔法施展范围不足时 其他玩家看到人物跑动的假动作 piaoyun 2013-09-15
  if (not m_boDeath) and (msg.Ident <> SM_MAGICFIRE) and (msg.Ident <> SM_MAGICFIRE_FAIL) then begin
    case Msg.ident of
      SM_TURN, SM_WALK, SM_BACKSTEP, SM_RUSH, SM_MAGICMOVE, {十步一杀} SM_RUSHKUNG, SM_RUN, SM_HORSERUN, SM_DIGUP, SM_ALIVE, SM_100HIT,
      SM_CUSTOM_MAGICMOVE001..(SM_CUSTOM_MAGICMOVE001 + CUSTOM_MAGIC_COUNT - 1),
        SM_CUSTOM_PUSH001..(SM_CUSTOM_PUSH001 + CUSTOM_MAGIC_COUNT - 1):begin
          // m_nFeature := Msg.Feature;
          // m_nState := Msg.State;
          { TODO -c特效 -opiaoyun  : 启动的时候天使特效没有，蛋疼的处理了 【2013-5-24】  }
          m_Feature := Msg.Feature;

          // m_nState := Msg.State;
          if Msg.State and STATE_OPENHEATH <> 0 then
            m_boOpenHealth := True
          else
            m_boOpenHealth := False;
        end;
    end;

    //if (Msg.ident = SM_LIGHTING) or (Msg.ident = SM_LIGHTINGEX) then n := 0;
    if g_MySelf = Self then begin
      if (Msg.ident = CM_WALK) then
        if not PlayScene.UnLockCanWalk(Msg.X, Msg.Y) then
          Exit;
      if (Msg.ident = CM_RUN) then
        if not PlayScene.UnLockCanRun(g_MySelf.m_nCurrX, g_MySelf.m_nCurrY, Msg.X, Msg.Y) then
          Exit;
      if (Msg.ident = CM_HORSERUN) then
        if not PlayScene.UnLockCanRun(g_MySelf.m_nCurrX, g_MySelf.m_nCurrY, Msg.X, Msg.Y) then
          Exit;

      //if (Msg.Ident = CM_WALK) or (Msg.Ident = CM_RUN) then begin
      //    OutputDebugString('CM_WALK or CM_RUN');
      //end;

      case Msg.ident of
        CM_TURN,
          CM_WALK,
          CM_SITDOWN,
          CM_RUN,
          CM_HIT,
          CM_HEAVYHIT,
          CM_BIGHIT,
          CM_POWERHIT,
          CM_LONGHIT,
          CM_WIDEHIT,
          CM_FIREHIT,
          CM_CRSHIT,
          CM_TWNHIT, // 以上是老端技能

        CM_43HIT, // 以下是新技能
        CM_SWORDHIT,

        CM_66HIT, // 开天斩
        CM_100HIT,
          CM_101HIT,
          CM_102HIT,
          CM_103HIT,
          CM_113HIT, // 断空斩
        CM_115HIT, // 血魄一击

        CM_CUSTOM_HIT001..CM_CUSTOM_HIT001 + CUSTOM_MAGIC_COUNT - 1:begin
            RealActionMsg := Msg;
            {.$I VMProtectBeginMutation.inc}
            RealActionMsg.x := Makecode_T(Msg.x);
            RealActionMsg.y := Makecode_T(Msg.y);
            {.$I VMProtectEnd.inc}

            m_btDir := msg.dir;

            if (Msg.ident >= CM_CUSTOM_HIT001) and (Msg.ident < CM_CUSTOM_HIT001 + CUSTOM_MAGIC_COUNT) then
              Msg.Ident := SM_CUSTOM_HIT001 + (Msg.ident - CM_CUSTOM_HIT001)
            else begin
              { TODO -c修改 -opiaoyun : ★★★★★★这里按照IGE修改，修复技能不能使用★★★★★★ 【2013-5-20】}
              Msg.ident := Msg.ident - 3000; // Msg.ident + 1100;

              // 根据Grobal2.pas 来修正 by PiaoYun
              case msg.Ident of
                25:Msg.ident := SM_FIREHIT; // 烈火剑法
                36:Msg.Ident := SM_CRSHIT; // 双龙斩
                37:Msg.Ident := SM_TWNHIT; // 龙影剑法
                // 战士连击技能修复 -- piaoyun 2013-07-13
                100:Msg.Ident := SM_100HIT;
                101:Msg.Ident := SM_101HIT;
                102:Msg.Ident := SM_102HIT;
                103:Msg.Ident := SM_103HIT;
              end;
            end;

            // 修正战士技能 自身技能8方向计算错误 chongchong 2016-08-04
            // 战士的 m_CurMagic在此没有赋值,而在代码 procedure DrawSelfMagicEffect(AMagicID: WORD; BeforeDraw: Boolean; IsWarrDraw: Boolean); 中要用
            if Msg.ident <> CM_115HIT then begin
              m_CurMagic.targx := -1;
              m_CurMagic.targy := -1;
            end
            else begin
              m_CurMagic.targx := Msg.X;
              m_CurMagic.targy := Msg.Y;
            end;

            if Msg.ident > SM_HORSERUN then {// 强化技能} begin
              m_nHitEffectLevel := Msg.State;
              m_CurMagic.NewLevel := Msg.State;
            end;

            // 4级技能强化 -- 4级烈火 chongchong 2013-12-04
            if Msg.Ident = SM_FIREHIT then
              m_nHitEffectLevel2 := Msg.sound;

            {$IF DEBUG_HIT_DELAY = 1}
            g_SelfHitStart.Add(IntToStr(TimeGetTime));
            g_SelfHitStart.SaveToFile(g_sSelfHitStartFile);
            {$IFEND}
          end;
        CM_HORSERUN:begin
            RealActionMsg := Msg;
            {.$I VMProtectBeginMutation.inc}
            RealActionMsg.x := Makecode_T(Msg.x);
            RealActionMsg.y := Makecode_T(Msg.y);
            {.$I VMProtectEnd.inc}
            Msg.ident := SM_HORSERUN;
          end;
        {CM_THROW: begin
            if m_Feature.Feature <> 0 then begin
              m_nTargetX := TActor(Msg.Feature.Feature).m_nCurrX;
              m_nTargetY := TActor(Msg.Feature.Feature).m_nCurrY;
              m_nTargetRecog := TActor(Msg.Feature.Feature).m_nRecogId;
            end;
            RealActionMsg := Msg;
            Msg.ident := SM_THROW;
          end;}

        // 开天斩轻击 piaoyun 2013-08-24
        CM_66HIT1:begin
            RealActionMsg := Msg;
            {.$I VMProtectBeginMutation.inc}
            RealActionMsg.x := Makecode_T(Msg.x);
            RealActionMsg.y := Makecode_T(Msg.y);
            {.$I VMProtectEnd.inc}
            Msg.Ident := SM_66HIT1;
          end;

        CM_SPELL:begin
            UseMagic := pTUseMagicInfo(Msg.Feature.Feature);

            RealActionMsg := Msg;
            {.$I VMProtectBeginMutation.inc}
            RealActionMsg.x := Makecode_T(Msg.x);
            RealActionMsg.y := Makecode_T(Msg.y);
            {.$I VMProtectEnd.inc}
            RealActionMsg.dir := UseMagic.MagicSerial;
            Msg.ident := SM_SPELL;
            //DScreen.AddChatBoardString('~~~~~~~~~~~~~~~~~~~~~~~~SM_SPELL', clRed, clBlack);
          end;
      end;

      m_nOldx := m_nCurrX;
      m_nOldy := m_nCurrY;
      m_nOldDir := m_btDir;
      // DScreen.AddChatBoardString(Format('%s:★★★★★★★★★★记录坐标:%d,%d', [FormatDateTime('hh:mm:ss', Time), m_nOldx, m_nOldy]), GetRGB(0), GetRGB(255));
    end;
    case Msg.ident of
      SM_STRUCK:begin
          m_nMagicStruckSound := Msg.X;
          n := Round(200 - m_Abil.Level * 5);
          if n > 80 then
            m_dwStruckFrameTime := n
          else
            m_dwStruckFrameTime := 80;

          // 延长怪物弯腰时间
          m_dwStruckFrameTime := m_dwStruckFrameTime + g_ClientConfig.btMonStruckFrameDelayTime;

          m_dwLastStruckTime := TimeGetTime;
        end;
      SM_SPELL:begin
          m_btDir := Msg.dir;
          // msg.x  :targetx
          // msg.y  :targety
          UseMagic := pTUseMagicInfo(Msg.Feature.Feature);
          // UseMagic := PTUseMagicInfo(Msg.Feature);
          if UseMagic <> nil then begin
            if UseMagic^.MagicSerial = 63 then begin
              m_boUseMagic := False;
            end;

            m_CurMagic := UseMagic^;
            m_CurMagic.ServerMagicCode := -1; // FIRE

            // ★★★★修正自定义技能的问题★★★★★ ++++  chongchong 2016-04-21
            m_boUseMagic := False;

            // 准备动作把目标坐标和目标对象先去掉再说
            // 动作成功的时候还会发对象和坐标过来的
            // 失败时不显示飞行或目标效果 chongchong 2015-05-09

            m_CurMagic.targx := Msg.X;
            m_CurMagic.targy := Msg.Y;
            m_nTargetRecog := m_CurMagic.target;

            {$IF TESTMODE = 1}
            //DScreen.AddChatBoardString('sm_spell' + IntToStr(m_CurMagic.EffectNumber), clRed, clBlack);
            {$IFEND}

            Dispose(UseMagic);
          end;
        end;
      // 自定义怪物攻击 chongchong 2014-07-22
      SM_ATTACK01, SM_ATTACK02, SM_ATTACK03, SM_ATTACK04, SM_ATTACK05, SM_ATTACK06:begin
          m_btDir := Msg.dir;
          m_nCurrX := Msg.X;
          m_nCurrY := Msg.Y;

          // 自定义怪物变脸 2021-04-19 11:53:02
          if m_nChangeAppr >= 0 then
            m_nCurrentAction := SM_HIT
          else
            m_nCurrentAction := Msg.ident;

          m_Saying := Msg.saying;
          UseMagic := pTUseMagicInfo(Msg.Feature.Feature);
          // UseMagic := PTUseMagicInfo(Msg.Feature);
          if UseMagic <> nil then begin
            m_CurMagic := UseMagic^;
            m_nTargetRecog := m_CurMagic.target;
            Dispose(UseMagic);
          end;
        end;
      else begin
          {m_nCurrX := msg.x;
          m_nCurrY := msg.y;
          m_btDir := msg.dir;}
          if (Msg.ident <> SM_MAGICFIRE) and (Msg.ident <> SM_MAGICFIRE_FAIL) then begin // 可能产生黑屏
            (*
            // 修正在不停的跑步过程中，使用随机传送卷可能导致的卡位 chongchong 2017-11-21
            if (Self = g_MySelf) and (Msg.ident in [SM_RUN, SM_WALK]) and (MyGetTickCount - g_dwMapMovingWaitTick <= 500) then
            begin

            end
            else
            *)
            begin
              // 修复战战合击破魂斩卡位（使用合击之后，立马跑步) chongchong 2018-06-20  17:21:35
              // m_nCurrX := Msg.X;
              // m_nCurrY := Msg.Y;

              if Msg.Ident <> SM_60HIT then begin
                m_nCurrX := Msg.X;
                m_nCurrY := Msg.Y;
              end
              else begin
                m_SM60HitX := Msg.X;
                m_SM60HitY := Msg.Y;
                m_SM60HitDir := Msg.dir;
              end;

              // 处理火龙守护兽 方向改变问题 piaoyun 2013-08-19
              if m_btRace <> 95 then begin
                if (msg.Ident = SM_BACKSTEP) or
                  (msg.Ident = SM_100HIT) or
                  ((msg.Ident >= SM_CUSTOM_PUSH001) and (Msg.Ident < SM_CUSTOM_PUSH001 + CUSTOM_MAGIC_COUNT)) then begin
                  m_btDir := LoByte(Word(msg.dir));
                  m_btStep := HiByte(Word(msg.dir));
                  if m_btStep = 0 then m_btStep := 1;
                end
                else begin
                  // 修复战战合击破魂斩卡位（使用合击之后，立马跑步) chongchong 2018-06-20  17:21:35
                  //m_btDir := Msg.dir;

                  if Msg.Ident <> SM_60HIT then
                    m_btDir := Msg.dir
                  else
                    m_btDir := GetNextDirection(m_nCurrX, m_nCurrY, m_SM60HitX, m_SM60HitY);
                end;

              end;
            end;
          end;
        end;
    end;

    case Msg.ident of
      {平滑移动  By 一支笔 at:2022-03-25 16:54:43}
      (*
      SM_RUN,
      SM_RUSH,
      SM_RUSHKUNG,
      SM_MAGICMOVE,
      SM_CUSTOM_MAGICMOVE001..(SM_CUSTOM_MAGICMOVE001 + CUSTOM_MAGIC_COUNT - 1),
      SM_HORSERUN,
      SM_BACKSTEP,
      SM_100HIT,
      SM_CUSTOM_PUSH001..(SM_CUSTOM_PUSH001 + CUSTOM_MAGIC_COUNT - 1),
      SM_WALK:
      begin
        m_fVecX := (m_nCurrX - m_nActBeforeX) * 48 / g_dwRunIntervalTime;
        m_fVecY := (m_nCurrY - m_nActBeforeY) * 32 / g_dwRunIntervalTime;
//        m_fVecX := (((m_nCurrX * 48)) - ((m_nActBeforeX * 48))) / g_dwRunIntervalTime;
//        m_fVecY := (((m_nCurrY * 32)) - ((m_nActBeforeY * 32))) / g_dwRunIntervalTime;
//        m_dwStartMoveTime := TimeGetTime;
      end;
      *)
      SM_HIT, // 14
      SM_HEAVYHIT, // 15
      SM_POWERHIT, // 18
      SM_LONGHIT, // 19
      SM_WIDEHIT, // 24
      SM_BIGHIT, // 16
      //SM_FIREHIT,
      SM_60HIT,
        SM_61HIT,
        SM_62HIT,
        SM_66HIT,
        SM_TWNHIT,
        SM_CRSHIT,
        SM_43HIT,
        SM_SWORDHIT,
        SM_101HIT,
        SM_102HIT,
        SM_103HIT,
        SM_CUSTOM_HIT001..(SM_CUSTOM_HIT001 + CUSTOM_MAGIC_COUNT - 1):begin
          m_nHitEffectLevel := Msg.State;

          // 修复开天斩随着角色飘 (按了开天的快捷后，迅速跑开） chongchong 2018-06-20 15:16:43
          //m_nHitEndX := m_nRx;
          //m_nHitEndY := m_nRy;
        end;

      // 4级技能强化 -- 4级烈火 chongchong 2013-12-04
      SM_FIREHIT:begin
          m_nHitEffectLevel := Msg.State;
          m_nHitEffectLevel2 := Msg.sound;

          // 修复开天斩随着角色飘 (按了开天的快捷后，迅速跑开） chongchong 2018-06-20 15:16:43
          //m_nHitEndX := m_nRx;
          //m_nHitEndY := m_nRy;
        end;
    end;

    // 自定义怪物变脸 2021-04-19 11:53:02
    if (m_nChangeAppr >= 0) and
      ( (Msg.ident = SM_ATTACK01) or (Msg.ident = SM_ATTACK02) or  (Msg.ident = SM_ATTACK03) or (Msg.ident = SM_ATTACK04)
        or (Msg.ident = SM_ATTACK05) or (Msg.ident = SM_ATTACK06) ) then begin
      m_nCurrentAction := SM_HIT
    end else begin
      m_nCurrentAction := Msg.ident;
    end;

    m_dwCurrentActionTick := TimeGetTime;

    {$IF TESTMODE = 1}
    //DScreen.AddChatBoardString('~~~~~~m_nCurrentAction' + IntToStr(m_nCurrentAction), clRed, clBlack);
    {$IFEND}

    // 去掉复活时多余的动作 chongchong 2016-05-04
    //if Msg.Ident <> SM_ALIVE then

    // 加入这个，这里面有可能取到这2个消息，然后把 m_boUseMagic := False chongchong 2017-12-02
    // 导致自定义技能绘制自身动作不完整
    if (Msg.ident <> SM_MAGICFIRE) and (Msg.ident <> SM_MAGICFIRE_FAIL) then begin
      CalcActorFrame;
    end;

    ActionChanged;

    {
    if (self = g_MySelf) and (m_nCurrentAction <> 0) then
    begin
      OutputDebugString(PChar('m_nCurrentAction=' + IntToStr(m_nCurrentAction)));
    end;
    }
  end
  else begin
    if Msg.ident = SM_SKELETON then begin
      m_nCurrentAction := Msg.ident;
      m_dwCurrentActionTick := TimeGetTime;
      CalcActorFrame;
      ActionChanged;
      m_boSkeleton := True;
    end;
  end;

  if (Msg.ident = SM_DEATH) or (Msg.ident = SM_NOWDEATH) then begin
    m_boStruckShowNumber := False;
    m_boShowBigHPProgress := False;
    m_boDeath := True;
    m_dwDeathTick := TimeGetTime;
    // DScreen.AddChatBoardString('Actor.m_boDeath 2', clRed, clWhite);
    PlayScene.ActorDied(Self);

    if Self = g_MySelf then
      g_boCanDrawTileMap := True;
  end;

  RunSound;
end;

function TActor.GetCurrX:Integer;
begin
  if (g_MySelf <> nil) and {(m_btRace = RC_PLAYOBJECT) and}(m_nRecogId = g_MySelf.m_nRecogId) then
    Result := Cutecode_T(FCurrX)
  else
    Result := FCurrX;
end;

function TActor.GetCurrY:Integer;
begin
  if (g_MySelf <> nil) and {(m_btRace = RC_PLAYOBJECT) and}(m_nRecogId = g_MySelf.m_nRecogId) then
    Result := Cutecode_T(FCurrY)
  else
    Result := FCurrY;
end;

procedure TActor.SetCurrX(Value:Integer);
begin
  if (g_MySelf <> nil) and {(m_btRace = RC_PLAYOBJECT) and}(m_nRecogId = g_MySelf.m_nRecogId) then
    FCurrX := Makecode_T(Value)
  else
    FCurrX := Value;
end;

procedure TActor.SetCurrY(Value:Integer);
begin
  if (g_MySelf <> nil) and {(m_btRace = RC_PLAYOBJECT) and}(m_nRecogId = g_MySelf.m_nRecogId) then
    FCurrY := Makecode_T(Value)
  else
    FCurrY := Value;
end;

function TActor.GetOldX:Integer;
begin
  if (g_MySelf <> nil) and {(m_btRace = RC_PLAYOBJECT) and}(m_nRecogId = g_MySelf.m_nRecogId) then
    Result := Cutecode_T(FOldx)
  else
    Result := FOldx;
end;

function TActor.GetOldY:Integer;
begin
  if (g_MySelf <> nil) and {(m_btRace = RC_PLAYOBJECT) and}(m_nRecogId = g_MySelf.m_nRecogId) then
    Result := Cutecode_T(FOldy)
  else
    Result := FOldy;
end;

procedure TActor.SetOldX(Value:Integer);
begin
  if (g_MySelf <> nil) and {(m_btRace = RC_PLAYOBJECT) and}(m_nRecogId = g_MySelf.m_nRecogId) then
    FOldx := Makecode_T(Value)
  else
    FOldx := Value;
end;

procedure TActor.SetOldY(Value:Integer);
begin
  if (g_MySelf <> nil) and {(m_btRace = RC_PLAYOBJECT) and}(m_nRecogId = g_MySelf.m_nRecogId) then
    FOldy := Makecode_T(Value)
  else
    FOldy := Value;
end;

function TActor.GetRX:Integer;
begin
  if (g_MySelf <> nil) and {(m_btRace = RC_PLAYOBJECT) and}(m_nRecogId = g_MySelf.m_nRecogId) then
    Result := Cutecode_T(FRx)
  else
    Result := FRx;
end;

function TActor.GetRY:Integer;
begin
  if (g_MySelf <> nil) and {(m_btRace = RC_PLAYOBJECT) and}(m_nRecogId = g_MySelf.m_nRecogId) then
    Result := Cutecode_T(FRy)
  else
    Result := FRy;
end;

procedure TActor.SetRX(Value:Integer);
begin
  if (g_MySelf <> nil) and {(m_btRace = RC_PLAYOBJECT) and}(m_nRecogId = g_MySelf.m_nRecogId) then
    FRx := Makecode_T(Value)
  else
    FRx := Value;
end;

procedure TActor.SetRY(Value:Integer);
begin
  if (g_MySelf <> nil) and {(m_btRace = RC_PLAYOBJECT) and}(m_nRecogId = g_MySelf.m_nRecogId) then
    FRy := Makecode_T(Value)
  else
    FRy := Value;
end;

function TActor.GetActBeforeX:Integer;
begin
  if (g_MySelf <> nil) and {(m_btRace = RC_PLAYOBJECT) and}(m_nRecogId = g_MySelf.m_nRecogId) then
    Result := Cutecode_T(FActBeforX)
  else
    Result := FActBeforX;
end;

function TActor.GetActBeforeY:Integer;
begin
  if (g_MySelf <> nil) and {(m_btRace = RC_PLAYOBJECT) and}(m_nRecogId = g_MySelf.m_nRecogId) then
    Result := Cutecode_T(FActBeforY)
  else
    Result := FActBeforY;
end;

procedure TActor.SetActBeforeX(Value:Integer);
begin
  if (g_MySelf <> nil) and {(m_btRace = RC_PLAYOBJECT) and}(m_nRecogId = g_MySelf.m_nRecogId) then
    FActBeforX := Makecode_T(Value)
  else
    FActBeforX := Value;
end;

procedure TActor.SetActBeforeY(Value:Integer);
begin
  if (g_MySelf <> nil) and {(m_btRace = RC_PLAYOBJECT) and}(m_nRecogId = g_MySelf.m_nRecogId) then
    FActBeforY := Makecode_T(Value)
  else
    FActBeforY := Value;
end;

function TActor.GetMessage(ChrMsg:pTChrMsg):Boolean;
var
  Msg:pTChrMsg;
begin
  Result := False;
  m_MsgList.Lock;
  try
    if m_MsgList.Count > 0 then begin
      Msg := m_MsgList.Items[0];

      {
      if Msg.Ident = SM_MAGICFIRE then
      begin
        OutputDebugString('aaa');
      end;
      }

      ChrMsg.ident := Msg.ident;
      ChrMsg.X := Msg.X;
      ChrMsg.Y := Msg.Y;
      ChrMsg.dir := Msg.dir;
      ChrMsg.State := Msg.State;
      ChrMsg.Feature := Msg.Feature;
      ChrMsg.saying := Msg.saying;
      ChrMsg.sound := Msg.sound;
      Dispose(Msg);
      m_MsgList.Delete(0);
      Result := True;
    end;
  finally
    m_MsgList.UnLock;
  end;
end;

procedure TActor.ProcLastMsg;
var
  I:Integer;
  Msg:pTChrMsg;
begin
  m_MsgList.Lock;
  try
    for I := m_MsgList.Count - 1 downto 0 do begin
      Msg := m_MsgList.Items[I];
      case Msg.Ident of
        SM_ALIVE:begin
            m_boDeath := False;
            m_boSkeleton := False;
            g_boCanDrawTileMap := True;
            g_BassSound.Clear;
            break;
          end;
        SM_DEATH, SM_NOWDEATH:begin
            m_boDeath := True;
            m_dwDeathTick := TimeGetTime;
            // DScreen.AddChatBoardString('Actor.m_boDeath 3', clRed, clWhite);
            m_boStruckShowNumber := False;
            m_boShowBigHPProgress := False;
            PlayScene.ActorDied(Self);
            if Self = g_MySelf then
              g_boCanDrawTileMap := True;
            break;
          end;
      end;
    end;
  finally
    m_MsgList.UnLock;
  end;
end;

procedure TActor.PlaySelfEffect(MsgIdent:Integer; nType:Integer);
begin
  case MsgIdent of
    SM_LEVELUP:begin
        if not m_boSelfEffectRunning then begin
          m_nSelfEffectCurrentFrame := 110; // 当前帧
          m_nSelfEffectEndFrame := 124; // 自身动画最后帧数
          m_nSelfEffectFrameTime := 60; // 自身动画时间间隔
          m_SelfEffectGameImage := g_WMain2Images; // 自身动画所在图库
          m_dwSelfEffectLastTick := TimeGetTime; // 上一帧时间
          m_boSelfEffectBlendDraw := True;
          m_boSelfEffectRunning := True; // 是否显示自身动画
          g_PlaySound.PlaySound(s_powerup);
        end;
      end;
    SM_STRUCKEFFECT:begin
        case nType of
          1: {// 挖到人形怪物品} begin
              m_nSelfEffectCurrentFrame := 30; // 当前帧
              m_nSelfEffectEndFrame := 54; // 自身动画最后帧数
              m_nSelfEffectFrameTime := 100; // 自身动画时间间隔
              m_SelfEffectGameImage := g_WMain2Images; // 自身动画所在图库
              m_dwSelfEffectLastTick := TimeGetTime; // 上一帧时间
              m_boSelfEffectBlendDraw := True;
              m_boSelfEffectRunning := True; // 是否显示自身动画
              g_PlaySound.PlaySound(s_dare_win);
            end;
          2: {// 挖到探索怪物品} begin
              // 探索要加这个动画 2020-06-27 23:21:36
              m_nSelfEffectCurrentFrame := 0; // 当前帧
              m_nSelfEffectEndFrame := 24; // 自身动画最后帧数
              m_nSelfEffectFrameTime := 100; // 自身动画时间间隔
              m_SelfEffectGameImage := g_WMain2Images; // 自身动画所在图库
              m_dwSelfEffectLastTick := TimeGetTime; // 上一帧时间
              m_boSelfEffectBlendDraw := True;
              m_boSelfEffectRunning := True; // 是否显示自身动画
              g_PlaySound.PlaySound(s_dare_win);
            end;
        end;
      end;
  end;
end;

procedure TActor.PlaySelfEffectCustom(ImgFileIndex, ImgIndex, ImgCount, FrameTime:Integer; IsBlendDraw:Boolean);
var
  GameImages:TGameImages;
begin
  if (ImgFileIndex >= 0) and (ImgFileIndex < g_EffectImageList.Count) and (ImgCount > 0) and (FrameTime > 0) then begin
    GameImages := TGameImages(g_EffectImageList.Objects[ImgFileIndex]);

    if GameImages = nil then Exit;

    m_nSelfEffectCurrentFrame := ImgIndex; // 当前帧
    m_nSelfEffectEndFrame := ImgIndex + ImgCount - 1; // 自身动画最后帧数
    m_nSelfEffectFrameTime := FrameTime; // 自身动画时间间隔
    m_SelfEffectGameImage := GameImages; // 自身动画所在图库
    m_dwSelfEffectLastTick := TimeGetTime; // 上一帧时间
    m_boSelfEffectBlendDraw := IsBlendDraw;
    m_boSelfEffectRunning := True; // 是否显示自身动画
  end;
end;

procedure TActor.ProcMsg;
var
  PMsg:PTChrMsg;
  Msg:TChrMsg;
  meff:TMagicEff;

  {
  keyvalue: TKeyBoardState;
  Shift: TShiftState;
  }
begin
  {
    检测该动作是否在指定时间内绘制完成，
    如果未完成则加快动作， 防止占用时间过长导致数据堆积，不能及时处理, 会造成人物动作延时卡，慢动作等等问题
  }

  //boGetNextMsg := False;

  { TODO -c修复 -opiaoyun : ★★★★★ 修复转向灵活度2 【2013-6-9】 ★★★★★ }
  { 修复跟着目标PK，目标放技能有时候会卡 + SM_RUN chongchong 2016-04-30 }
  if (m_nCurrentAction > 0) and (Self <> g_MySelf) and (m_MsgList.Count > 0) then begin
    case m_nCurrentAction of
      SM_TURN: {// 防止别人已经转向完成，已经在做下个动作，而自己看到的还是转向} begin // 如果是转向动作，消息列表中有其他消息时，立即停止该动作
          ActionEnded;

          m_nCurrentAction := 0;
          m_boUseMagic := False;
        end;
      SM_RUN:begin
          // 修正 2016-05-02
          m_MsgList.Lock;
          try
            PMsg := m_MsgList.Items[0];
            if (PMsg.Ident = SM_SPELL) and (m_CurMagic.ServerMagicCode < 0) then begin
              m_nCurrentAction := 0;
              ActionEnded;
              m_boUseMagic := False;
            end;
          finally
            m_MsgList.UnLock;
          end;

          // 加了这个可能会导致跑的时候突然加速 chongchong 2016-05-04
          //boGetNextMsg := True;
        end;
    end; // end case
  end;

  // 不能放在这里，放在这里会导致：
  // 一个人追着另一个人打，跑步时会一跳一跳的 chongchong 2016-12-19
  (*
  // 修正攻击间隔不均匀，移动自：TfrmMain.MouseTimerTimer chongchong 2016-12-10
  if (m_nCurrentAction = 0) and (self = g_MySelf) and (g_TargetCret <> nil) then
  begin
    if PlayScene.IsValidActor(g_TargetCret) and (not g_TargetCret.m_boDeath) and (not g_MySelf.m_boShopStall) {摆摊禁止移动} and (g_MySelf.m_btHorse = 0) then
    begin
      FillChar(keyvalue, SizeOf(TKeyBoardState), #0);
      if GetKeyboardState(keyvalue) then
      begin
        // DScreen.AddChatBoardString('MouseTimerTimer 6', clGreen, clWhite);
        Shift := [];
        if ((keyvalue[VK_SHIFT] and $80) <> 0) then Shift := Shift + [ssShift];
        if (not (g_TargetCret.m_btRace in [0, 1]) and
          (g_TargetCret.m_btRace <> RCC_GUARD) and
          (g_TargetCret.m_btRace <> RCC_MERCHANT) and
          (Pos('(', g_TargetCret.m_sUserName) = 0) // 林牢乐绰 各(碍力傍拜 秦具窃)
          )
          or (g_TargetCret.m_nNameColor = ENEMYCOLOR) // 利篮 磊悼 傍拜捞 凳
          or ((PlugInEnabled and g_ClientConfig.boNotNeedShift and
            g_ConfigDlg.ConfigCheckeds[ckNotNeedShift]) and
            ((g_ConfigDlg.ConfigCheckeds[ckShiftSwitch] and g_boShift) or (not g_ConfigDlg.ConfigCheckeds[ckShiftSwitch])) and
            (g_TargetCret.m_btRace <> RCC_MERCHANT)) // 免Shift
          or ((ssShift in Shift) and (not FrmDlg.DedChat.Enabled)) then
        begin
          //DScreen.AddChatBoardString('消息处理间隔:' + IntToStr(TimeGetTime - LastAttackTick), clWhite, clBlack);
          FrmMain.AttackTarget(g_TargetCret);
        end;
      end;
    end;
  end;
  *)

  if ((m_nCurrentAction = 0) or ((m_nCurrentAction = SM_RUN) and (m_nCurrentFrame >= m_nEndFrame))) and GetMessage(@Msg) then begin
    case Msg.ident of
      SM_STRUCK:begin
          m_nHiterCode := Msg.sound;
          ReadyAction(Msg);
        end;
      SM_SPACEMOVE_HIDE:begin
          meff := TScrollHideEffect.Create(250, 10, m_nCurrX, m_nCurrY, Self);
          PlayScene.AddEffectList(meff);
          g_PlaySound.PlaySound(s_spacemove_out);
        end;
      SM_SPACEMOVE_HIDE2:begin
          meff := TScrollHideEffect.Create(1590, 10, m_nCurrX, m_nCurrY, Self);
          PlayScene.AddEffectList(meff);
          g_PlaySound.PlaySound(s_spacemove_out);
        end;
      SM_SPACEMOVE_SHOW:begin
          meff := TCharEffect.Create(260, 10, Self);
          PlayScene.AddEffectList(meff);
          Msg.ident := SM_TURN;
          ReadyAction(Msg);
          g_PlaySound.PlaySound(s_spacemove_in);
        end;
      SM_SPACEMOVE_SHOW2:begin
          meff := TCharEffect.Create(1600, 10, Self);
          PlayScene.AddEffectList(meff);
          Msg.ident := SM_TURN;
          ReadyAction(Msg);
          g_PlaySound.PlaySound(s_spacemove_in);
        end;
      SM_MAGICFIRE:begin
          //DScreen.AddChatBoardString('~~~~~~~~~~~~~~~~~~~~~~~~SM_MAGICFIRE', clRed, clBlack);

          if (m_CurMagic.ServerMagicCode <> 0) or CheckIsCustomMagic(m_CurMagic.MagicSerial) then begin
            m_CurMagic.ServerMagicCode := 111;
            m_CurMagic.target := Msg.State;
            if Msg.Y in [0..MAXMAGICTYPE - 1] then
              m_CurMagic.EffectType := TMagicType(Msg.Y); // EffectType
            m_CurMagic.EffectNumber := LoWord(Msg.dir); // Effect
            m_CurMagic.targx := Msg.Feature.Feature;
            // m_CurMagic.targx := Msg.Feature;
            m_CurMagic.targy := Msg.X;
            m_nTargetRecog := m_CurMagic.target;
            m_CurMagic.Recusion := True;

            m_CurMagic.NewLevel := HiWord(Msg.dir);

            // 4级技能强化 -- 4级灵魂火符 4级灭天火 chongchong 2013-12-04
            m_CurMagic.MagicLevel := Msg.sound;

            m_Saying := Msg.saying;

            if m_CurMagic.NewLevel mod 2 = 0 then
              m_CurMagic.MagicItemType := True
            else
              m_CurMagic.MagicItemType := False;

            if m_CurMagic.MagicItemType then // 强化技能等级
              m_CurMagic.NewLevel := m_CurMagic.NewLevel div 2
            else
              m_CurMagic.NewLevel := (m_CurMagic.NewLevel - 1) div 2;
          end;
        end;
      SM_MAGICFIRE_FAIL:begin
          //DScreen.AddChatBoardString('~~~~~~~~~~~~~~~~~~~~~~~~SM_MAGICFIRE_FAIL', clRed, clBlack);
          if (m_CurMagic.ServerMagicCode <> 0) or CheckIsCustomMagic(m_CurMagic.MagicSerial) then begin
            m_CurMagic.ServerMagicCode := 0;
          end;
          { TODO -opiaoyun -c修复 : 技能释放失败时，动作过快【2013-6-8】 }
          // m_boUseMagic := False;  // 注释这个--
          m_boMagicEndEffect := False;
          if Self = g_MySelf then begin
            // DScreen.AddChatBoardString(' SM_MAGICFIRE_FAIL :' + IntToStr(TimeGetTime) + ' g_dwLatestSpellTick:' + IntToStr(g_dwLatestSpellTick), clGreen, clWhite);
            g_boLatestSpell := False;
            g_dwLatestSpellTick := MyGetTickCount; // + g_MySelf.UseMagicDelayTime + 1000;
            g_dwMagicDelayTime := Max(g_MySelf.UseMagicDelayTime(100), g_dwMagicDelayTime);
          end;
        end;
      SM_LEVELUP:begin
          {                                                                                   // 升级效果
          meff := TShowPlayEffect.Create(110, 14, Self);
          PlayScene.AddEffectList(meff);
          g_PlaySound.PlaySound(s_powerup);
          }
        end;
      else begin
          ReadyAction(Msg);
        end;
    end;
  end;
end;

type
  TActionType = (atOther, atHit, atWalk, atRun);

procedure TActor.ProcHurryMsg; // 处理魔法数据
{$J+}
const
  LastSendHitTick:LongWord = 0;
const
  LastSendMoveTick:LongWord = 0;
const
  LastActionType:TActionType = atOther;
  {$J-}
var
  n:Integer;
  Msg:TChrMsg;
  fin:Boolean;
  CurSendHitTick:LongWord;
  nNextHitTime:Integer;

  //dwMoveIntervalTime: Integer;
  //dwStepMoveTime, dwMoveTime: Integer;
begin
  m_MsgList.Lock;
  try
    for n := m_MsgList.Count - 1 downto 0 do begin
      Msg := pTChrMsg(m_MsgList.Items[n])^;
      fin := False;

      case Msg.ident of
        SM_MAGICFIRE:begin
            //DScreen.AddChatBoardString('~~~~~~~~~~~~~~~~~~~~~~~~SM_MAGICFIRE', clRed, clBlack);

            // 修正英雄跑步后使用自定义技能，比如：追别人 或是 被别人追，释放自定义技能，会出现画面丢失。 chongchong 2018-08-16 21:54:31
            // {or CheckIsCustomMagic(m_CurMagic.MagicSerial)} 注释掉的是这行

            if (m_CurMagic.ServerMagicCode <> 0) {or CheckIsCustomMagic(m_CurMagic.MagicSerial)} then begin
              m_CurMagic.ServerMagicCode := 111;
              m_CurMagic.target := Msg.State;
              if Msg.Y in [0..MAXMAGICTYPE - 1] then
                m_CurMagic.EffectType := TMagicType(Msg.Y); // EffectType
              m_CurMagic.EffectNumber := LoWord(Msg.dir); // Effect
              m_CurMagic.targx := Msg.Feature.Feature;
              // m_CurMagic.targx := Msg.Feature;
              m_CurMagic.targy := Msg.X;
              m_nTargetRecog := m_CurMagic.target;
              m_CurMagic.Recusion := True;

              m_CurMagic.NewLevel := HiWord(Msg.dir);

              // 4级技能强化 -- 4级灵魂火符 4级灭天火 chongchong 2013-12-04
              m_CurMagic.MagicLevel := Msg.sound;

              m_Saying := Msg.saying;

              if m_CurMagic.NewLevel mod 2 = 0 then
                m_CurMagic.MagicItemType := True
              else
                m_CurMagic.MagicItemType := False;

              if m_CurMagic.MagicItemType then // 强化技能等级
                m_CurMagic.NewLevel := m_CurMagic.NewLevel div 2
              else
                m_CurMagic.NewLevel := (m_CurMagic.NewLevel - 1) div 2;

              fin := True;
              // DScreen.AddSysMsg ('SM_MAGICFIRE ',255,0,100,100);
            end;
          end;
        SM_MAGICFIRE_FAIL:begin
            //DScreen.AddChatBoardString('~~~~~~~~~~~~~~~~~~~~~~~~SM_MAGICFIRE_FAIL', clRed, clBlack);
            if (m_CurMagic.ServerMagicCode <> 0) or CheckIsCustomMagic(m_CurMagic.MagicSerial) then begin
              m_CurMagic.ServerMagicCode := 0;
              fin := True;
            end;
            { TODO -opiaoyun -c修复 : 技能释放失败时，动作过快【2013-6-8】 }
            // m_boUseMagic := False;  // 注释这个--
            m_boMagicEndEffect := False;
            if Self = g_MySelf then begin
              // DScreen.AddChatBoardString(' SM_MAGICFIRE_FAIL :' + IntToStr(TimeGetTime) + ' g_dwLatestSpellTick:' + IntToStr(g_dwLatestSpellTick), clGreen, clWhite);
              g_boLatestSpell := False;
              g_dwLatestSpellTick := MyGetTickCount; // + g_MySelf.UseMagicDelayTime + 1000;
              g_dwMagicDelayTime := Max(g_MySelf.UseMagicDelayTime(100), g_dwMagicDelayTime);
            end;
          end;
      end;

      if fin then begin
        Dispose(pTChrMsg(m_MsgList.Items[n]));
        m_MsgList.Delete(n);
      end;

    end;
  finally
    m_MsgList.UnLock;
  end;

  // 修正攻击间隔不均匀，移动自：TfrmMain.ProcessActionMessages chongchong 2016-12-10
  if Self = g_MySelf then begin
    // g_wActionSpeedIntervals
    if (g_MySelf.RealActionMsg.ident > 0) and (not g_boMapMovingWait) then begin
      if not CheckSendIdent(g_MySelf.RealActionMsg.ident, g_MySelf.RealActionMsg.dir) then begin
        Exit;
      end;

      // ★★★★★★★★修正人物跑步攻击时，第二刀加快★★★★★★★★ 攻击间隔 chongchong 2016-01-08
      if (g_MySelf.RealActionMsg.ident = CM_HIT) or
        (g_MySelf.RealActionMsg.ident = CM_HEAVYHIT) or
        (g_MySelf.RealActionMsg.ident = CM_BIGHIT) or
        (g_MySelf.RealActionMsg.ident = CM_POWERHIT) or
        (g_MySelf.RealActionMsg.ident = CM_LONGHIT) or
        (g_MySelf.RealActionMsg.ident = CM_WIDEHIT) or
        (g_MySelf.RealActionMsg.ident = CM_FIREHIT) or
        (g_MySelf.RealActionMsg.ident = CM_CRSHIT) or
        (g_MySelf.RealActionMsg.ident = CM_TWNHIT) or
        (g_MySelf.RealActionMsg.ident = CM_SWORDHIT) or
        (g_MySelf.RealActionMsg.ident = CM_43HIT) or
        (g_MySelf.RealActionMsg.ident = CM_66HIT) or
        (g_MySelf.RealActionMsg.ident = CM_66HIT1) or
        (g_MySelf.RealActionMsg.ident = CM_101HIT) or
        (g_MySelf.RealActionMsg.ident = CM_102HIT) or
        (g_MySelf.RealActionMsg.ident = CM_103HIT) or
        (g_MySelf.RealActionMsg.ident = CM_113HIT) or
        (g_MySelf.RealActionMsg.ident = CM_115HIT) or
        ((g_MySelf.RealActionMsg.ident >= CM_CUSTOM_HIT001) and (g_MySelf.RealActionMsg.ident < CM_CUSTOM_HIT001 + CUSTOM_MAGIC_COUNT)) then begin
        CurSendHitTick := timeGetTime;

        nNextHitTime := g_MySelf.GetNextHitTime;

        if ((CurSendHitTick - LastSendHitTick) <= Cardinal(nNextHitTime))
          and ((CurSendHitTick - g_ActionRecvTick) <= Cardinal(Max(0, nNextHitTime - 50))) then begin //HZQ 20230525
          //OutputDebugString('aaa');
          Exit;
        end;

        {// 恢复攻击到移动间隔 chongchong 2017-12-07

        if LastActionType = atWalk then
        begin
          dwMoveIntervalTime := g_ClientConfig.dwWalkIntervalTime;
          if g_MySelf.m_nMoveSpeed <> 0 then
            dwMoveIntervalTime := Max(dwMoveIntervalTime - (g_ClientConfig.dwIncMoveSpeedDecInterval * g_MySelf.m_nMoveSpeed), 0);

          // 单步速度 chongchong 2017-12-05
          if g_ClientConfig.nMoveSpeed <> 0 then
            dwStepMoveTime := _Max(90 - Round(90 * g_ClientConfig.nMoveSpeed / 1000), 1)
          else
            dwStepMoveTime := 90;
          dwMoveTime := dwStepMoveTime * HA.ActWalk.frame;

          if ((CurSendHitTick - LastSendMoveTick) <= Max(dwMoveIntervalTime, dwMoveTime)) then
          begin
            Exit;
          end;
        end
        else if LastActionType = atRun then
        begin
          dwMoveIntervalTime := g_ClientConfig.dwRunIntervalTime;
          if g_MySelf.m_nMoveSpeed <> 0 then
            dwMoveIntervalTime := Max(dwMoveIntervalTime - (g_ClientConfig.dwIncMoveSpeedDecInterval * g_MySelf.m_nMoveSpeed), 0);

          // 单步速度 chongchong 2017-12-05
          if g_ClientConfig.nMoveSpeed <> 0 then
            dwStepMoveTime := _Max(90 - Round(90 * g_ClientConfig.nMoveSpeed / 1000), 1)
          else
            dwStepMoveTime := 90;
          dwMoveTime := dwStepMoveTime * HA.ActRun.frame;

          if ((CurSendHitTick - LastSendMoveTick) <= Max(dwMoveIntervalTime, dwMoveTime)) then
          begin
            Exit;
          end;
        end;
        }

        //DScreen.AddChatBoardString('消息处理间隔:' + IntToStr(TimeGetTime - LastSendHitTick), clWhite, clBlack);
        LastSendHitTick := CurSendHitTick;
        LastActionType := atHit;
      end
      else if (g_MySelf.RealActionMsg.Ident = CM_WALK) or (g_MySelf.RealActionMsg.Ident = CM_RUN) then begin
        {
        // 恢复攻击到移动间隔 chongchong 2017-12-07
        CurSendHitTick := timeGetTime

        nNextHitTime := g_MySelf.GetNextHitTime;

        if ((CurSendHitTick - LastSendHitTick) <= nNextHitTime) then
        begin
          Exit;
        end;

        LastSendMoveTick := CurSendHitTick;

        if g_MySelf.RealActionMsg.Ident = CM_WALK then
          LastActionType := atWalk
        else
          LastActionType := atRun;
        }
      end;

      FrmMain.FailAction := g_MySelf.RealActionMsg.ident;
      FrmMain.FailDir := g_MySelf.RealActionMsg.dir;

      (*
      DScreen.AddChatBoardString('消息处理间隔:' + IntToStr(TimeGetTime - LastProcessActionTick), clWhite, clBlack);
      LastProcessActionTick := TimeGetTime;
      *)

      // 攻击富贵兽的技能，要带上物品MakeIndex 2018-01-25
      if (g_MySelf.RealActionMsg.ident = CM_SPELL) and (g_MySelf.RealActionMsg.dir = 60000) then begin
        //DScreen.AddChatBoardString('攻击富贵兽', clWhite, clBlack);
        FrmMain.SendActionMsg(g_MySelf.RealActionMsg.ident,
          g_MySelf.RealActionMsg.X,
          g_MySelf.RealActionMsg.Y,
          g_MySelf.RealActionMsg.dir,
          g_MySelf.RealActionMsg.State, g_MySelf.RealActionMsg.saying);
      end else begin
        FrmMain.SendActionMsg(g_MySelf.RealActionMsg.ident,
          g_MySelf.RealActionMsg.X,
          g_MySelf.RealActionMsg.Y,
          g_MySelf.RealActionMsg.dir,
          g_MySelf.RealActionMsg.State);
      end;

      g_MySelf.RealActionMsg.ident := 0;

      if g_nMDlgX <> -1 then begin
        if (abs(g_nMDlgX - g_MySelf.m_nCurrX) >= 8) or (abs(g_nMDlgY - g_MySelf.m_nCurrY) >= 8) then begin
          FrmDlg.CloseMDlg;
          // FrmDlg.CloseBigMDlg;
          // FrmDlg.DBook.Visible := False;
          g_nMDlgX := -1;
        end;
      end;
    end;
  end;
end;

function TActor.IsIdle:Boolean;
begin
  // 修正在不停的跑步过程中，使用随机传送卷可能导致的卡位 ++  or (m_nCurrentAction = SM_TURN) chongchong 2017-11-21
  //if (m_nCurrentAction in [0,SM_TURN]) and (m_MsgList.Count = 0) then //龙族代码
  if (m_nCurrentAction = 0) and (m_MsgList.Count = 0) then begin
    Result := True
  end else begin
    Result := False;
  end;
end;

function TActor.ActionFinished:Boolean;
begin
  if (m_nCurrentAction = 0) or (m_nCurrentFrame >= m_nEndFrame - 1) (*or (MyGetTickCount - g_dwMapMovingWaitTick > 200) *) then // 增加切换地图超过200毫秒
    Result := True
  else
    Result := False;
  // DScreen.AddChatBoardString('m_nCurrentAction:' + IntToStr(m_nCurrentAction)+' m_nStartFrame:'+IntToStr(m_nStartFrame)+' m_nEndFrame:'+IntToStr(m_nEndFrame)+' m_nCurrentFrame:'+IntToStr(m_nCurrentFrame), clRed, clBlue);
end;

function TActor.GetNextHitTime:Integer;
var
  NextHitTime:Integer;
begin
  NextHitTime := Max(0, g_ClientConfig.dwHitFrameTime);

  if g_boAttackSlow and (Self = g_MySelf) then
    NextHitTime := NextHitTime + 1500; // 腕力超过时，减慢攻击速度

  // chongchong 2016-12-13
  if m_nAttackSpeed <> 0 then begin // 攻击速
    if m_nAttackSpeed <> 0 then begin
      NextHitTime := Max(Int64(NextHitTime) - Int64(m_nAttackSpeed) * Int64(g_ClientConfig.dwIncSpeedDecInterval), 0);
    end;
  end;

  NextHitTime := Max(0, NextHitTime);
  if NextHitTime < 100 then
    NextHitTime := 100;

  Result := NextHitTime;
end;

(*
function TActor.CanMove: Boolean;
var
  dwStepMoveTime: LongWord;
  nMoveSpeed: Integer;
  CurrTick: Int64;
begin
  Result := False;
  m_boCanMove := False;

  dwStepMoveTime := 90;
  if (m_btRace = RCC_USERHUMAN) and (m_nMoveSpeed <> 0) then
  begin
    if g_ClientConfig.nMoveSpeed <> 0 then
      dwStepMoveTime := _Max(90 - Round(90 * g_ClientConfig.nMoveSpeed / 1000), 1)
    else
      dwStepMoveTime := 90;

    {
    // 修复未加速的人在跑动的时候看加速的跑动人会抖动 chongchong 2015-04-21
    nMoveSpeed := m_nMoveSpeed + g_ClientConfig.nMoveSpeed;
    if nMoveSpeed <> 0 then
      dwStepMoveTime := Max(90 - Round(90 * nMoveSpeed / 1000), 1);
    }
  end;

  CurrTick := timeGetTime;
  if (CurrTick - m_dwMoveTime) >= dwStepMoveTime then
  begin
    m_dwMoveTime := CurrTick;
    Inc(m_nMoveStepCount);
    if m_nMoveStepCount > 1 then
      m_nMoveStepCount := 0;
    Result := True;
    m_boCanMove := True;
  end;
end;
*)

function TActor.CanWalk:Integer;
begin
  // 修改 在传送时候不允许移动
  if (*(TimeGetTime - LastStruckTime < 1300) or*)(MyGetTickCount - g_dwLatestSpellTick < g_dwMagicPKDelayTime) or (g_ChrAction = caNone) or g_boMapMovingWait then
    Result := -1 // 掉饭捞
  else
    Result := 1;

  // 战技锁定
  if m_nState and $00010000 <> 0 then Result := -1;
end;

function TActor.CanRun:Integer;
begin
  Result := 1;
  // 修改 在传送时候不允许移动
  // 检查人物的HP值是否低于指定值，低于指定值将不允许跑
  if (m_Abil.HP < RUN_MINHEALTH) or (g_ChrAction = caNone) or g_boMapMovingWait then begin
    Result := -1;
  end;

  // 战技锁定
  if m_nState and $00010000 <> 0 then Result := -1;

  // 检查人物是否被攻击，如果被攻击将不允许跑，取消检测将可以跑步逃跑
  // if (TimeGetTime - LastStruckTime < 3*1000) or (TimeGetTime - LatestSpellTime < MagicPKDelayTime) then
  // Result := -2;
end;

function TActor.Strucked:Boolean;
var
  I:Integer;
begin
  Result := False;
  for I := 0 to m_MsgList.Count - 1 do begin
    if pTChrMsg(m_MsgList[I]).ident = SM_STRUCK then begin
      Result := True;
      Break;
    end;
  end;
end;

// dir : 规氢
// step : 捞悼 沫
// cur : 泅犁 胶跑
// max : 弥措 胶跑

procedure TActor.Shift(dir, step, cur, Max:Integer);
var
  unx, uny, ss, v:Integer;
begin
  unx := UNITX * step;
  uny := UNITY * step;
  if cur > Max then cur := Max;
  m_nRx := m_nCurrX;
  m_nRy := m_nCurrY;

  case dir of
    DR_UP:begin
        ss := Round((Max - cur) / Max) * step;
        m_nShiftX := 0;
        m_nRy := m_nCurrY + ss;
        if ss = step then
          m_nShiftY := -Round(uny / Max * cur)
        else
          m_nShiftY := Round(uny / Max * (Max - cur));

        // 修正人影在树影下跑时闪动 chongchong 2015-06-15
        if m_nShiftY mod 2 <> 0 then m_nShiftY := m_nShiftY + 1;
      end;
    DR_UPRIGHT:begin
        if Max >= 6 then
          v := 2
        else
          v := 0;
        ss := Round((Max - cur + v) / Max) * step;
        m_nRx := m_nCurrX - ss;
        m_nRy := m_nCurrY + ss;
        if ss = step then begin
          m_nShiftX := Round(unx / Max * cur);
          m_nShiftY := -Round(uny / Max * cur);
        end
        else begin
          m_nShiftX := -Round(unx / Max * (Max - cur));
          m_nShiftY := Round(uny / Max * (Max - cur));
        end;
        if m_nShiftX mod 2 <> 0 then m_nShiftX := m_nShiftX + 1;
        if m_nShiftY mod 2 <> 0 then m_nShiftY := m_nShiftY + 1;
      end;
    DR_RIGHT:begin
        ss := Round((Max - cur) / Max) * step;
        m_nRx := m_nCurrX - ss;
        if ss = step then
          m_nShiftX := Round(unx / Max * cur)
        else
          m_nShiftX := -Round(unx / Max * (Max - cur));
        if m_nShiftX mod 2 <> 0 then m_nShiftX := m_nShiftX + 1;
        m_nShiftY := 0;
      end;
    DR_DOWNRIGHT:begin
        if Max >= 6 then
          v := 2
        else
          v := 0;

        // ★★★★★★★★ 修正骑马一步三格黑边问题 ★★★★★★★★ 2015-06-15
        if step = 3 then
          v := 1
        else if step = 4 then begin
          if cur = 3 then
            v := 0
          else
            v := 1;
        end
        else if step = 5 then {// 修正追心刺推动5格黑边问题 2021-01-26} begin
          if cur = 4 then
            v := 1
          else
            v := 0;
        end;

        ss := Round((Max - cur - v) / Max) * step;
        m_nRx := m_nCurrX - ss;
        m_nRy := m_nCurrY - ss;
        if (ss = step) then begin
          m_nShiftX := Round(unx / Max * cur);
          m_nShiftY := Round(uny / Max * cur);
        end
        else begin
          m_nShiftX := -Round(unx / Max * (Max - cur));
          m_nShiftY := -Round(uny / Max * (Max - cur));
        end;
        if m_nShiftX mod 2 <> 0 then m_nShiftX := m_nShiftX + 1;
        if m_nShiftY mod 2 <> 0 then m_nShiftY := m_nShiftY + 1;
      end;
    DR_DOWN:begin
        if Max >= 6 then
          v := 1
        else
          v := 0;
        ss := Round((Max - cur - v) / Max) * step;
        m_nShiftX := 0;
        m_nRy := m_nCurrY - ss;
        if ss = step then
          m_nShiftY := Round(uny / Max * cur)
        else
          m_nShiftY := -Round(uny / Max * (Max - cur));
        if m_nShiftY mod 2 <> 0 then m_nShiftY := m_nShiftY + 1;
      end;
    DR_DOWNLEFT:begin
        if Max >= 6 then
          v := 2
        else
          v := 0;

        ss := Round((Max - cur - v) / Max) * step;
        m_nRx := m_nCurrX + ss;
        m_nRy := m_nCurrY - ss;
        if ss = step then begin
          m_nShiftX := -Round(unx / Max * cur);
          m_nShiftY := Round(uny / Max * cur);
        end
        else begin
          m_nShiftX := Round(unx / Max * (Max - cur));
          m_nShiftY := -Round(uny / Max * (Max - cur));
        end;
        if m_nShiftX mod 2 <> 0 then m_nShiftX := m_nShiftX + 1;
        if m_nShiftY mod 2 <> 0 then m_nShiftY := m_nShiftY + 1;
      end;
    DR_LEFT:begin
        ss := Round((Max - cur) / Max) * step;
        m_nRx := m_nCurrX + ss;
        if ss = step then
          m_nShiftX := -Round(unx / Max * cur)
        else
          m_nShiftX := Round(unx / Max * (Max - cur));

        if m_nShiftX mod 2 <> 0 then m_nShiftX := m_nShiftX + 1;
        m_nShiftY := 0;
      end;
    DR_UPLEFT:begin
        if Max >= 6 then
          v := 2
        else
          v := 0;

        if step = 4 then begin
          if cur = 3 then
            v := 0
          else
            v := 1;
        end
        else if step = 5 then {// 修正追心刺推动5格黑边问题 2021-01-26} begin
          if cur = 4 then
            v := 0
          else
            v := 1;
        end;

        ss := Round((Max - cur + v) / Max) * step;
        m_nRx := m_nCurrX + ss;
        m_nRy := m_nCurrY + ss;
        if ss = step then begin
          m_nShiftX := -Round(unx / Max * cur);
          m_nShiftY := -Round(uny / Max * cur);
        end
        else begin
          m_nShiftX := Round(unx / Max * (Max - cur));
          m_nShiftY := Round(uny / Max * (Max - cur));
        end;
        if m_nShiftX mod 2 <> 0 then m_nShiftX := m_nShiftX + 1;
        if m_nShiftY mod 2 <> 0 then m_nShiftY := m_nShiftY + 1;
      end;
  end;
end;

procedure TActor.FeatureChanged;
var
  I:Integer;
  MonsterConfig:PClientCustomMonsterConfig;
  boShopStall, IsFoundCustomMonsterConfig:Boolean;
begin
  case m_btRace of
    // human
    0, 1:begin
        m_nChangeAppr := pTHumFeature(@m_Feature.Buffer).nChangeAppr;
        if m_nChangeAppr >= 0 then begin
          m_wAppearance := m_nChangeAppr;

          // 战士正在攻击中，变脸会卡或绘制错误 2020-10-20 22:20:10
          if Self = g_MySelf then begin
            g_MySelf.ActionEnded;
            g_MySelf.m_nCurrentAction := 0;
            g_MySelf.m_boHitEndEffect := False;
            g_MySelf.m_boHitEffect := False;
            frmMain.UnLockAction;
          end;
        end
        else
          m_wAppearance := 0;
        m_btHorse := pTHumFeature(@m_Feature.Buffer).btHorseType;
        m_btDoubleHumHorse := pTHumFeature(@m_Feature.Buffer).btDoubleHumHorseType;
        m_boShowHorseWingsEffect := pTHumFeature(@m_Feature.Buffer).boShowHorseWingsEffect;
        m_btHorseHum := pTHumFeature(@m_Feature.Buffer).btHorseHum;
        m_btHorseHumExpand := pTHumFeature(@m_Feature.Buffer).btHorseHumExpand;
        m_btHorseHair := pTHumFeature(@m_Feature.Buffer).btHorseHair;
        m_btHorseEffectType := pTHumFeature(@m_Feature.Buffer).btHorseEffectType;

        m_boShowFashion := pTHumFeature(@m_Feature.Buffer).boShowFashion;
        m_boMagicShield := pTHumFeature(@m_Feature.Buffer).boMagicShield;
        m_btReLevel := pTHumFeature(@m_Feature.Buffer).btReLevel;

        if g_ClientVersion in [cv176, cv185, cvHero, cvSerial, cvMirSequel, cvMirNewUI205] then begin
          if Self = g_MySelf then begin
            // 修得攻击时骑马问题 chongchong 2013-11-08
            if m_btHorse <> 0 then begin
              g_MySelf.ActionEnded;
              g_MySelf.m_nCurrentAction := 0;
            end;
            TSerialWindows(FrmDlg).DDownHorse.Visible := (m_btHorse in [1, 2]) and (m_btDoubleHumHorse = 0);
            TSerialWindows(FrmDlg).NewStateWindows.DSWShowFashion.Checked := m_boShowFashion;
          end
          else if Self = g_MyHero then
            TSerialWindows(FrmDlg).NewStateWindows.DHeroSWShowFashion.Checked := m_boShowFashion;
        end;

        m_btSex := pTHumFeature(@m_Feature.Buffer).btGender;
        m_btJob := pTHumFeature(@m_Feature.Buffer).btJob;
        m_btHair := pTHumFeature(@m_Feature.Buffer).btHair;
        m_wDress := pTHumFeature(@m_Feature.Buffer).wDress;
        m_wWeapon := pTHumFeature(@m_Feature.Buffer).wWeapon;
        m_wWeaponSound := pTHumFeature(@m_Feature.Buffer).wWeaponSound;
        m_wEffect := pTHumFeature(@m_Feature.Buffer).wDressEffType;
        m_wEffect_30 := pTHumFeature(@m_Feature.Buffer).wDressEffType_30;
        m_wShield := pTHumFeature(@m_Feature.Buffer).wShield; // 盾牌 chongchong 2013-09-16

        m_boEffectNormalDraw := pTHumFeature(@m_Feature.Buffer).boDressEffNormalDraw;
        m_boEffect_30NormalDraw := pTHumFeature(@m_Feature.Buffer).boDressEff_30NormalDraw;
        m_boDressEffNoSex := pTHumFeature(@m_Feature.Buffer).boDressEffNoSex;
        m_boDressEff_30NoSex := pTHumFeature(@m_Feature.Buffer).boDressEff_30NoSex;

        m_btOldHair := pTHumFeature(@m_Feature.Buffer).btOldHair;
        m_boPlayMoster := pTHumFeature(@m_Feature.Buffer).boPlayMoster;

        m_btCaseltGuild := pTHumFeature(@m_Feature.Buffer).btCaseltGuild; // 1=沙行会成员 //2=沙行会掌门
        m_nWeaponEffectIndex := pTHumFeature(@m_Feature.Buffer).nWeaponEffectIndex; // 武器发光效果外观wil 编号
        m_wDBWeaponEffectOffSet := pTHumFeature(@m_Feature.Buffer).wDBWeaponEffectOffSet; // 武器发光效果外观DB 2020-11-11 00:51:03
        m_wWeaponEffectOffSet := pTHumFeature(@m_Feature.Buffer).wWeaponEffectOffSet; // 武器发光效外观偏移
        m_nDressEffectIndex := pTHumFeature(@m_Feature.Buffer).nDressEffectIndex; // 衣服发光效外观wil 编号
        m_wDressEffectOffSet := pTHumFeature(@m_Feature.Buffer).wDressEffectOffSet; // 衣服发光效外观偏移
        m_nShieldEffectIndex := pTHumFeature(@m_Feature.Buffer).nShieldEffectIndex; // 武器发光效果外观wil 编号
        m_wShieldEffectOffSet := pTHumFeature(@m_Feature.Buffer).wShieldEffectOffSet; // 武器发光效外观偏移

        m_boDressEffectNoBlend := pTHumFeature(@m_Feature.Buffer).boDressEffectNoBlend;
        m_boDressEffectNoSex := pTHumFeature(@m_Feature.Buffer).boDressEffectNoSex;

        m_nMedalEffectIndex := pTHumFeature(@m_Feature.Buffer).nMedalEffectIndex; // 衣服发光效外观wil 编号
        m_wMedalEffectOffSet := pTHumFeature(@m_Feature.Buffer).wMedalEffectOffSet; // 衣服发光效外观偏移
        m_boMedalEffectNoBlend := pTHumFeature(@m_Feature.Buffer).boMedalEffectNoBlend;
        m_boMedalEffectNoSex := pTHumFeature(@m_Feature.Buffer).boMedalEffectNoSex;

        m_btCboDressUseDiyImage := pTHumFeature(@m_Feature.Buffer).btCboDressUseDiyImage;
        m_btCboWeaponUseDiyImage := pTHumFeature(@m_Feature.Buffer).btCboWeaponUseDiyImage;

        m_boWeaponEffectNoBlend := pTHumFeature(@m_Feature.Buffer).boWeaponEffectNoBlend;
        m_boWeaponEffectNoSex := pTHumFeature(@m_Feature.Buffer).boWeaponEffectNoSex;
        m_boShieldEffectNoBlend := pTHumFeature(@m_Feature.Buffer).boShieldEffectNoBlend;
        m_boShieldEffectNoSex := pTHumFeature(@m_Feature.Buffer).boShieldEffectNoSex;

        m_nDressAddEffectIndex := pTHumFeature(@m_Feature.Buffer).nDressAddEffectIndex;
        m_bDressAddEffectOrder := pTHumFeature(@m_Feature.Buffer).bDressAddEffectOrder;
        m_wDressAddEffectOffSet := pTHumFeature(@m_Feature.Buffer).wDressAddEffectOffSet;
        m_wDressAddEffectCount := pTHumFeature(@m_Feature.Buffer).wDressAddEffectCount;
        m_wDressAddEffectTime := pTHumFeature(@m_Feature.Buffer).wDressAddEffectTime;
        m_boDressAddEffectNoBlend := pTHumFeature(@m_Feature.Buffer).boDressAddEffectNoBlend;
        m_boDressAddEffectDrawCenter := pTHumFeature(@m_Feature.Buffer).boDressAddEffectDrawCenter;

        m_btBodyColor := pTHumFeature(@m_Feature.Buffer).btBodyColor; // 人体颜色
        m_boShowHair := pTHumFeature(@m_Feature.Buffer).boShowHair;

        boShopStall := m_boShopStall;
        m_boShopStall := pTHumFeature(@m_Feature.Buffer).boShopStall;

        m_sActiveFengHaoName := pTHumFeature(@m_Feature.Buffer).sActiveFengHaoName; // 激活的封号名
        m_nActiveFengHaoID := pTHumFeature(@m_Feature.Buffer).nActiveFengHaoID; // 激活的封号ID
        m_dwActiveFengHaoLooks := pTHumFeature(@m_Feature.Buffer).dwActiveFengHaoLooks; // 激活封号的Looks
        m_btActiveFengHaoReserved := pTHumFeature(@m_Feature.Buffer).btActiveFengHaoReserved;
        m_nActiveFengHaoColor := GetRGB(pTHumFeature(@m_Feature.Buffer).btActiveFengHaoColor); // 激活封号的颜色

        //m_sShowUserName := pTHumFeature(@m_Feature.Buffer).sShowName;

        m_HumsBBType := bbNo;

        if (m_btRace = 0) and (boShopStall <> m_boShopStall) and
          Assigned(THumActor(Self).m_OnShopStall) then begin
          if m_boShopStall then begin
            m_btDir := pTHumFeature(@m_Feature.Buffer).btShopStallDir;
          end;

          THumActor(Self).m_OnShopStall(Self);
        end;
        // 发型偏移计算1 -- piaoyun 2013-6-10
        {if m_btHair < 6 then begin
          if (m_btHair < 4) or (m_btHorse > 0) then begin
            case m_btSex of
              0: m_nHairOffset := m_btHair * 2 * HUMANFRAME;
              1: m_nHairOffset := (m_btHair + 2) * HUMANFRAME;
            end;
          end else begin
            case m_btHair of
              4: m_nHairOffset := 3600;
              5: m_nHairOffset := 4800;
            else m_nHairOffset := -1;
            end;
          end;
        end else begin
          m_nHairOffset := 3600 + (m_btHair - 6) * HUMANFRAME * 2 + m_btSex * HUMANFRAME;
        end; }
        case m_btHair of
          0..5:begin
              if (m_btHair < 4) or (m_btHorse > 0) then begin
                case m_btSex of
                  0:m_nHairOffset := m_btHair * 2 * HUMANFRAME;
                  1:m_nHairOffset := (m_btHair + 2) * HUMANFRAME;
                end;
              end
              else begin
                case m_btHair of // 头盔
                  4:m_nHairOffset := 3600;
                  5:m_nHairOffset := 4800;
                  else
                    m_nHairOffset := -1;
                end;
              end;
            end;
          50..59:begin
              m_nHairOffset := (m_btHair - 50) * HUMANFRAME * 2 + m_btSex * HUMANFRAME;
            end;
          60..69:begin
              m_nHairOffset := (m_btHair - 60) * HUMANFRAME * 2 + m_btSex * HUMANFRAME;
            end;
          else {(m_btHair - 6)}
            m_nHairOffset := 3600 + (m_btHair - 100) * HUMANFRAME * 2 + m_btSex * HUMANFRAME;
        end;

        m_nWeaponOffset := HUMANFRAME * m_wWeapon; // (weapon*2 + m_btSex);

        if m_wEffect <> 0 then begin
          if m_wEffect = 50 then
            m_nHumWinOffset := 352
          else
            m_nHumWinOffset := (m_wEffect - 1) * HUMANFRAME;
        end;

        if m_wEffect_30 <> 0 then begin
          if m_wEffect_30 = 50 then
            m_nHumWinOffset_30 := 352
          else
            m_nHumWinOffset_30 := (m_wEffect_30 - 1) * HUMANFRAME;
        end;

        if g_MySelf = Self then
          FrmDlg.MySelfAbilChange
        else if g_MyHero = Self then
          FrmDlg.MyHeroAbilChange;

        if g_ClientConfig.boUseHeroM2Shop then begin
          if (g_nHeroM2SelectRecogId <> 0) and (m_nRecogId = g_nHeroM2SelectRecogId) then begin
            if not m_boShopStall then begin
              g_nHeroM2SelectRecogId := 0;
              g_HeroM2SelectActor := nil;
              g_HeroM2SelectShopItem.S.Name := '';
              FrmDlg.CloseDHeroM2ShopRemoteDlg;

              if (g_MySelf <> Self) then
                DScreen.AddChatBoardString('对方已经取消了摆摊！', clRed, clWhite);
            end;
          end;
        end;
      end;
    50:; // npc
    else begin
        // 自定义怪物变脸增加++++ 2021-04-19 12:33:16
        m_btRace := pTMonFeature(@m_Feature.Buffer).wRaceImg;

        m_wAppearance := pTMonFeature(@m_Feature.Buffer).wAppr;
        m_btBodyColor := pTMonFeature(@m_Feature.Buffer).btBodyColor; // 人体颜色
        m_nBodyOffset := GetOffset(m_wAppearance);
        m_HumsBBType := pTMonFeature(@m_Feature.Buffer).HumBBType;
        m_nChangeAppr := pTMonFeature(@m_Feature.Buffer).nChangeAppr;
        if Self is TCustomActor then begin
          IsFoundCustomMonsterConfig := False;
          for I := 0 to g_CustomMonsterConfig.Count - 1 do begin
            MonsterConfig := g_CustomMonsterConfig.Items[I];
            if MonsterConfig.wMonsterAppr = m_wAppearance then begin
              IsFoundCustomMonsterConfig := True;
              TCustomActor(Self).Config := MonsterConfig^;
              Break;
            end;
          end;
          if not IsFoundCustomMonsterConfig then begin
            DScreen.AddChatBoardString(Format(DecodeResStr(SCustomMonNoConfig), [m_wAppearance]), clWhite, clRed);
          end;
        end;
      end;
  end;

  {$IF Enabled_PlugEngine = 1}
  if Assigned(HookTActor_FeatureChanged) then begin
    try
      HookTActor_FeatureChanged(Self);
    except
      on E:Exception do begin
        DebugOutStr('[Exception] HookTActor_FeatureChanged');
        DebugOutStr(E.Message);
      end;
    end;
  end;
  {$IFEND}

end;

function TActor.light:Integer;
begin
  Result := m_nChrLight;
end;

procedure TActor.Initialize;
begin

end;

procedure TActor.Finalize;
var
  I:Integer;
begin
  m_BodySurface := nil;
  m_WeaponEffectSurface := nil;
  m_DBWeaponEffectSurface := nil;
  m_DressEffectSurface := nil;
  m_ShieldEffectSurface := nil;
  m_MedalEffectSurface := nil;
  m_DressAddEffectSurface := nil;
  m_HorseSurface := nil;

  m_sNameText := '';
  m_sCurNameText := '';
  m_sNumberLableText := '';
  m_sCurNumberLableText := '';

  m_sCurShopNameText := '';
  // m_sShopNameText := '';

  if m_NameTextSurface <> nil then begin
    m_NameTextSurface.Free;
    m_NameTextSurface := nil;
  end;

  m_NumberLableImageInfo.Width := 0;
  m_NumberLableImageInfo.Height := 0;
  m_NumberLableImageInfo.ImageIndexs := nil;
  m_ShopNameImageInfo.Width := 0;
  m_ShopNameImageInfo.Height := 0;
  m_ShopNameImageInfo.ImageIndexs := nil;

  m_FengHaoEffectSurface := nil;
  SetLength(m_FengHaoImageInfo, 0);

  for I := Low(TActorIconArray) to High(TActorIconArray) do begin
    m_ActorIconIndexs[I].Texture := nil;
  end;

  for I := 0 to Length(m_SayingArr) - 1 do begin
    m_SayingArr[I].TextSurface.Width := 0;
    m_SayingArr[I].TextSurface.Height := 0;
    m_SayingArr[I].TextSurface.ImageIndexs := nil;
  end;

  for I := 0 to Length(m_HealthNumberArray) - 1 do begin
    m_HealthNumberArray[I].nWidth := 0;
    SetLength(m_HealthNumberArray[I].ImageIndexs, 0);
  end;

  m_dwLoadSurfaceTime := MyGetTickCount - 60 * 1000;
  m_dwLoadFengHaoSurfaceTime := MyGetTickCount - 60 * 1000;
end;

procedure TActor.LoadSurface(Sender:TObject);
var
  mimg:TGameImages;
  Index:Integer;

  I:Integer;
  MonsterConfig, TempMonsterConfig:PClientCustomMonsterConfig;
  ClientAction:PMonsterClientAction;
  giBody:TGameImages;
  nBodyOffset:Integer;
begin
  if (not GameCanvas.Active) or (not GameCanvas.Initialized) then Exit;
  m_dwLoadSurfaceTime := MyGetTickCount;
  m_boLoadSurface := False;
  m_BodySurface := nil;

  if (m_btRace in [156]) and (m_nChangeAppr >= 0) then begin
    MonsterConfig := nil;
    m_BodySurface := nil;

    for I := 0 to g_CustomMonsterConfig.Count - 1 do begin
      TempMonsterConfig := g_CustomMonsterConfig.Items[I];
      if TempMonsterConfig.wMonsterAppr = m_nChangeAppr then begin
        MonsterConfig := TempMonsterConfig;
        Break;
      end;
    end;

    ClientAction := nil;
    if MonsterConfig <> nil then begin
      case m_nCurrentAction of
        0, SM_TURN:begin
            if (m_nState and STATE_STONE_MODE) <> 0 then
              ClientAction := @MonsterConfig.Actions[matStoneRevive]
            else
              ClientAction := @MonsterConfig.Actions[matStand];
          end;
        SM_WALK, SM_RUSH, SM_RUSHKUNG, SM_BACKSTEP:begin
            ClientAction := @MonsterConfig.Actions[matWalk];
          end;
        SM_DIGUP:begin
            ClientAction := @MonsterConfig.Actions[matStoneRevive];
          end;
        SM_LIGHTINGEX:begin

          end;
        SM_HIT:begin
            ClientAction := @MonsterConfig.Actions[matDefAttack];
          end;
        SM_STRUCK:begin
            ClientAction := @MonsterConfig.Actions[matStruck];
          end;
        SM_DEATH:begin
            ClientAction := @MonsterConfig.Actions[matDie];
          end;
        SM_NOWDEATH:begin
            ClientAction := @MonsterConfig.Actions[matDie];
          end;
        SM_SKELETON:begin

          end;
      end;
    end;

    if (ClientAction <> nil) and (ClientAction.StartIndex >= 0) and (ClientAction.PlayCount > 0) then begin
      if (ClientAction.ActionFile >= 0) and (ClientAction.ActionFile < g_EffectImageList.Count) then
        giBody := TGameImages(g_EffectImageList.Objects[ClientAction.ActionFile])
      else
        giBody := g_WMonImages.Images[m_nChangeAppr - 100000];

      nBodyOffset := m_nCurrentFrame;
    end else begin
      giBody := nil; //HZQ 20230525
      nBodyOffset := 0;
    end;

    if giBody <> nil then begin
      if (not m_boReverseFrame) then begin
        case m_ColorEffect of
          ceGrayScale, ceGrayScale2:m_BodySurface := giBody.GetCachedGrayImage(nBodyOffset, m_nPx, m_nPy);
          ceBright:m_BodySurface := giBody.GetCachedBrightImage(nBodyOffset, m_nPx, m_nPy);
          else
            m_BodySurface := giBody.GetCachedImage(nBodyOffset, m_nPx, m_nPy);
        end;
      end;
    end;
  end else begin
    if PlugInEnabled and g_ClientConfig.boHideGhost and g_ConfigDlg.ConfigCheckeds[ckHideGhost]
      and m_boDeath and (not ((m_wAppearance >= 900) and (m_wAppearance <= 906))) then begin
      Finalize;
    end else begin
      mimg := g_WMonImages.Images[m_wAppearance];
      if mimg <> nil then begin
        if (not m_boReverseFrame) then begin
          case m_ColorEffect of
            ceGrayScale, ceGrayScale2:m_BodySurface := mimg.GetCachedGrayImage(GetOffset(m_wAppearance) + m_nCurrentFrame, m_nPx, m_nPy);
            ceBright:m_BodySurface := mimg.GetCachedBrightImage(GetOffset(m_wAppearance) + m_nCurrentFrame, m_nPx, m_nPy);
            else
              m_BodySurface := mimg.GetCachedImage(GetOffset(m_wAppearance) + m_nCurrentFrame, m_nPx, m_nPy);
          end;
        end else begin
          Index := GetOffset(m_wAppearance) + m_nEndFrame - (m_nCurrentFrame - m_nStartFrame);
          case m_ColorEffect of
            ceGrayScale, ceGrayScale2:m_BodySurface := mimg.GetCachedGrayImage(Index, m_nPx, m_nPy);
            ceBright:m_BodySurface := mimg.GetCachedBrightImage(Index, m_nPx, m_nPy);
            else
              m_BodySurface := mimg.GetCachedImage(Index, m_nPx, m_nPy);
          end;
        end;
      end;
    end;
  end;
  ActionChanged;
end;

function TActor.CharWidth:Integer;
begin
  if m_btHorse = 0 then begin
    if m_BodySurface <> nil then
      Result := m_BodySurface.Width
    else
      Result := 48;
  end
  else begin
    if m_HorseSurface <> nil then
      Result := m_HorseSurface.Width
    else
      Result := 100;
  end;
end;

function TActor.CharHeight:Integer;
begin
  if m_btHorse = 0 then begin
    if m_BodySurface <> nil then
      Result := m_BodySurface.Height
    else
      Result := 70;
  end
  else begin
    if m_HorseSurface <> nil then
      Result := m_HorseSurface.Height
    else
      Result := 70;
  end;
end;

function TActor.CheckSelect(dx, dy:Integer):Boolean;
begin
  Result := False;
  if m_btHorse = 0 then begin
    if Assigned(m_BodySurface) then begin
      if CheckTextureAlpha(m_BodySurface, dx, dy) and
        CheckTextureAlpha(m_BodySurface, dx - 1, dy) and
        CheckTextureAlpha(m_BodySurface, dx + 1, dy) and
        CheckTextureAlpha(m_BodySurface, dx, dy - 1) and
        CheckTextureAlpha(m_BodySurface, dx, dy + 1) then
        Result := True;
    end;
  end
  else begin
    if Assigned(m_HorseSurface) then begin
      if CheckTextureAlpha(m_HorseSurface, dx, dy) and
        CheckTextureAlpha(m_HorseSurface, dx - 1, dy) and
        CheckTextureAlpha(m_HorseSurface, dx + 1, dy) and
        CheckTextureAlpha(m_HorseSurface, dx, dy - 1) and
        CheckTextureAlpha(m_HorseSurface, dx, dy + 1) then
        Result := True;
    end;
  end;
end;

// ++++++++ （新加）将状态绘制单独搞出来 2020-05-23 23:24:29

procedure TActor.DrawStateEffSurface(ddx, ddy:Integer);
var
  d:TTexture;
  pX, pY:Integer;
begin
  if (m_boCobweb and (not m_boDuanJin)) and (not m_boGhost) and (not m_boDeath) then begin // 蜘蛛网罩住
    if TimeGetTime - m_dwCobwebTick > 100 then begin
      m_dwCobwebTick := TimeGetTime;
      Inc(m_nCobwebIndex);
    end;
    if (m_nCobwebIndex < 0) or (m_nCobwebIndex > 9) then
      m_nCobwebIndex := 0;

    d := g_WNewopUIImages.GetCachedImage(320 + m_nCobwebIndex, pX, pY);
    if d <> nil then begin
      GameCanvas.DrawBlend(m_nSayX - d.Width div 2, ddy + pY - 20, d); // (Source.Width - d.Width) div 2
    end;
  end;

  // 毒烟 chongchong 2013-11-10
  if (m_boToxicSmoke) and (not m_boGhost) and (not m_boDeath) then begin
    if TimeGetTime - m_dwToxicSmokeTick > 80 then begin
      m_dwToxicSmokeTick := TimeGetTime;
      Inc(m_nToxicSmokeIndex);
    end;
    if (m_nToxicSmokeIndex < 0) or (m_nToxicSmokeIndex > 9) then
      m_nToxicSmokeIndex := 0;

    d := g_cboEffect.GetCachedImage(4010 + m_nToxicSmokeIndex, pX, pY); //Images[4010 + m_nToxicSmokeIndex];
    if d <> nil then begin
      GameCanvas.DrawBlend(m_nSayX - d.Width div 2, ddy + pY, d); // (Source.Width - d.Width) div 2
    end;
  end;

  // 永恒冰冻 piaoyun 2013-12-05
  if (m_boForeverFrozen) and (not m_boGhost) and (not m_boDeath) then begin
    if TimeGetTime - m_dwForeverFrozenTick > 80 then begin
      m_dwForeverFrozenTick := TimeGetTime;
      Inc(m_nForeverFrozenIndex);
    end;
    if (m_nForeverFrozenIndex < 0) or (m_nForeverFrozenIndex > 3) then
      m_nForeverFrozenIndex := 0;

    d := g_WNewopUIImages.GetCachedImage(330 + m_nForeverFrozenIndex, pX, pY);
    if d <> nil then begin
      GameCanvas.DrawBlend(m_nSayX - d.Width div 2, ddy + pY, d);
    end;
  end;
end;

procedure TActor.DrawEffSurface(Source:TTexture; ddx, ddy:Integer; blend:Boolean; ceff:TColorEffect);
begin
  if m_nState and $00800000 <> 0 then begin
    blend := True;
  end;
  if Assigned(Source) then begin
    if not blend then begin
      if m_btBodyColor <> 0 then begin
        GameCanvas.DrawColor(ddx, ddy, Source, GetRGB(m_btBodyColor));
      end
      else begin
        case ceff of
          ceNone:GameCanvas.Draw(ddx, ddy, Source);
          ceGrayScale, ceGrayScale2:GameCanvas.Draw(ddx, ddy, Source);
          ceBright:GameCanvas.Draw(ddx, ddy, Source);
          ceBlack:GameCanvas.DrawColor(ddx, ddy, Source, clBlack);
          ceWhite:GameCanvas.DrawColor(ddx, ddy, Source, clWhite);
          ceRed:GameCanvas.DrawColor(ddx, ddy, Source, clRed);
          // 更改绿色为草绿色 clGreen --> clLime   piaoyun 2013-06-28
          ceGreen:GameCanvas.DrawColor(ddx, ddy, Source, clLime);
          ceBlue:GameCanvas.DrawColor(ddx, ddy, Source, clBlue);
          ceYellow:GameCanvas.DrawColor(ddx, ddy, Source, clYellow);
          ceFuchsia:GameCanvas.DrawColor(ddx, ddy, Source, clFuchsia);
          ceAqua:GameCanvas.DrawColor(ddx, ddy, Source, clAqua);
          ceSilver:GameCanvas.DrawColor(ddx, ddy, Source, clSilver);
          ceGray:GameCanvas.DrawColor(ddx, ddy, Source, clGray);
        end;
      end;
    end
    else begin
      if m_btBodyColor <> 0 then begin
        GameCanvas.DrawColorAlpha(ddx, ddy, Source, GetRGB(m_btBodyColor), 150);
      end
      else begin
        case ceff of
          ceNone:GameCanvas.DrawColorAlpha(ddx, ddy, Source, clWhite, 150);
          ceGrayScale, ceGrayScale2:GameCanvas.DrawColorAlpha(ddx, ddy, Source, clWhite, 150);
          ceBright:GameCanvas.DrawColorAlpha(ddx, ddy, Source, clWhite, 150);
          ceBlack:GameCanvas.DrawColorAlpha(ddx, ddy, Source, clBlack, 150);
          ceWhite:GameCanvas.DrawColorAlpha(ddx, ddy, Source, clWhite, 150);
          ceRed:GameCanvas.DrawColorAlpha(ddx, ddy, Source, clRed, 150);
          ceGreen:GameCanvas.DrawColorAlpha(ddx, ddy, Source, clGreen, 150);
          ceBlue:GameCanvas.DrawColorAlpha(ddx, ddy, Source, clBlue, 150);
          ceYellow:GameCanvas.DrawColorAlpha(ddx, ddy, Source, clYellow, 150);
          ceFuchsia:GameCanvas.DrawColorAlpha(ddx, ddy, Source, clFuchsia, 150);
          ceAqua:GameCanvas.DrawColorAlpha(ddx, ddy, Source, clAqua, 150);
          ceSilver:GameCanvas.DrawColorAlpha(ddx, ddy, Source, clSilver, 150);
          ceGray:GameCanvas.DrawColorAlpha(ddx, ddy, Source, clGray, 150);
        end;
      end;
    end;
  end;

  // 将状态绘制单独搞出来 2020-05-23 23:24:29
  {
  if (m_BodySurface = Source) and (m_boCobweb and (not m_boDuanJin)) and (not m_boGhost) and (not m_boDeath) then
  begin                                                                                             // 蜘蛛网罩住
    if TimeGetTime - m_dwCobwebTick > 100 then
    begin
      m_dwCobwebTick := TimeGetTime;
      Inc(m_nCobwebIndex);
    end;
    if (m_nCobwebIndex < 0) or (m_nCobwebIndex > 9) then
      m_nCobwebIndex := 0;

    d := g_WNewopUIImages.Images[320 + m_nCobwebIndex];
    if d <> nil then
    begin
      GameCanvas.DrawBlend(m_nSayX - d.Width div 2, ddy, d);                                        // (Source.Width - d.Width) div 2
    end;
  end;

  // 毒烟 chongchong 2013-11-10
  if (m_BodySurface = Source) and (m_boToxicSmoke) and (not m_boGhost) and (not m_boDeath) then
  begin
    if TimeGetTime - m_dwToxicSmokeTick > 80 then
    begin
      m_dwToxicSmokeTick := TimeGetTime;
      Inc(m_nToxicSmokeIndex);
    end;
    if (m_nToxicSmokeIndex < 0) or (m_nToxicSmokeIndex > 9) then
      m_nToxicSmokeIndex := 0;

    d := g_cboEffect.GetCachedImage(4010 + m_nToxicSmokeIndex, pX, pY);                             //Images[4010 + m_nToxicSmokeIndex];
    if d <> nil then
    begin
      GameCanvas.DrawBlend(m_nSayX - d.Width div 2, ddy + pY + 35, d);                              // (Source.Width - d.Width) div 2
    end;
  end;

  // 永恒冰冻 piaoyun 2013-12-05
  if (m_BodySurface = Source) and (m_boForeverFrozen) and (not m_boGhost) and (not m_boDeath) then
  begin
    if TimeGetTime - m_dwForeverFrozenTick > 80 then
    begin
      m_dwForeverFrozenTick := TimeGetTime;
      Inc(m_nForeverFrozenIndex);
    end;
    if (m_nForeverFrozenIndex < 0) or (m_nForeverFrozenIndex > 3) then
      m_nForeverFrozenIndex := 0;

    d := g_WNewopUIImages.GetCachedImage(330 + m_nForeverFrozenIndex, pX, pY);
    if d <> nil then
    begin
      GameCanvas.DrawBlend(m_nSayX - d.Width div 2, ddy + pY + 35, d);
    end;
  end;
  }
end;

procedure TActor.StretchDrawEffSurface(Source:TTexture; ddx, ddy:Integer; blend:Boolean; ceff:TColorEffect);
var
  DestRect, SrcRect:TRect;
  nX, nY:Integer;
  nWidth, nHeight:Integer;
begin
  if Assigned(Source) then begin
    nWidth := Round(Source.Width * 1.5);
    nHeight := Round(Source.Height * 1.5);
    nX := 12;
    nY := UNITY;
    DestRect := Bounds(ddx - nX, ddy - nY, nWidth, nHeight);
    SrcRect := Source.ClientRect;

    if m_btBodyColor <> 0 then begin
      GameCanvas.StretchDraw(DestRect, SrcRect, Source, m_btPhantomAlpha, GetRGB(m_btBodyColor));
    end else begin
      case ceff of
        ceNone:GameCanvas.StretchDraw(DestRect, SrcRect, Source, m_btPhantomAlpha);
        ceGrayScale, ceGrayScale2:GameCanvas.StretchDraw(DestRect, SrcRect, Source, m_btPhantomAlpha);
        ceBright:GameCanvas.StretchDraw(DestRect, SrcRect, Source, m_btPhantomAlpha);
        ceBlack:GameCanvas.StretchDraw(DestRect, SrcRect, Source, m_btPhantomAlpha, clBlack);
        ceWhite:GameCanvas.StretchDraw(DestRect, SrcRect, Source, m_btPhantomAlpha, clWhite);
        ceRed:GameCanvas.StretchDraw(DestRect, SrcRect, Source, m_btPhantomAlpha, clRed);
        ceGreen:GameCanvas.StretchDraw(DestRect, SrcRect, Source, m_btPhantomAlpha, clGreen);
        ceBlue:GameCanvas.StretchDraw(DestRect, SrcRect, Source, m_btPhantomAlpha, clBlue);
        ceYellow:GameCanvas.StretchDraw(DestRect, SrcRect, Source, m_btPhantomAlpha, clYellow);
        ceFuchsia:GameCanvas.StretchDraw(DestRect, SrcRect, Source, m_btPhantomAlpha, clFuchsia);
        ceAqua:GameCanvas.StretchDraw(DestRect, SrcRect, Source, m_btPhantomAlpha, clAqua);
        ceSilver:GameCanvas.StretchDraw(DestRect, SrcRect, Source, m_btPhantomAlpha, clSilver);
        ceGray:GameCanvas.StretchDraw(DestRect, SrcRect, Source, m_btPhantomAlpha, clGray);
      end;
    end;

    { if not blend then begin
       if m_btColor < 255 then begin
         GameCanvas.StretchDraw(DestRect, SrcRect, Source, m_btPhantomsAlpha, GetRGB(m_btColor));
       end else begin
         case ceff of
           ceNone: GameCanvas.StretchDraw(DestRect, SrcRect, Source, m_btPhantomsAlpha);
           ceGrayScale, ceGrayScale2: GameCanvas.StretchDraw(DestRect, SrcRect, Source, m_btPhantomsAlpha);
           ceBright: GameCanvas.StretchDraw(DestRect, SrcRect, Source, m_btPhantomsAlpha);
           ceBlack: GameCanvas.StretchDraw(DestRect, SrcRect, Source, m_btPhantomsAlpha, clBlack);
           ceWhite: GameCanvas.StretchDraw(DestRect, SrcRect, Source, m_btPhantomsAlpha, clWhite);
           ceRed: GameCanvas.StretchDraw(DestRect, SrcRect, Source, m_btPhantomsAlpha, clRed);
           ceGreen: GameCanvas.StretchDraw(DestRect, SrcRect, Source, m_btPhantomsAlpha, clGreen);
           ceBlue: GameCanvas.StretchDraw(DestRect, SrcRect, Source, m_btPhantomsAlpha, clBlue);
           ceYellow: GameCanvas.StretchDraw(DestRect, SrcRect, Source, m_btPhantomsAlpha, clYellow);
           ceFuchsia: GameCanvas.StretchDraw(DestRect, SrcRect, Source, m_btPhantomsAlpha, clFuchsia);
           ceAqua: GameCanvas.StretchDraw(DestRect, SrcRect, Source, m_btPhantomsAlpha, clAqua);
         end;
       end;
     end else begin
       if m_btColor < 255 then begin
         GameCanvas.StretchDraw(DestRect, SrcRect, Source, m_btPhantomsAlpha, GetRGB(m_btColor));
       end else begin
         case ceff of
           ceNone: GameCanvas.StretchDraw(DestRect, SrcRect, Source, m_btPhantomsAlpha, clWhite);
           ceGrayScale, ceGrayScale2: GameCanvas.StretchDraw(DestRect, SrcRect, Source, m_btPhantomsAlpha, clWhite);
           ceBright: GameCanvas.StretchDraw(DestRect, SrcRect, Source, m_btPhantomsAlpha, clWhite);
           ceBlack: GameCanvas.StretchDraw(DestRect, SrcRect, Source, m_btPhantomsAlpha, clBlack);
           ceWhite: GameCanvas.StretchDraw(DestRect, SrcRect, Source, m_btPhantomsAlpha, clWhite);
           ceRed: GameCanvas.StretchDraw(DestRect, SrcRect, Source, m_btPhantomsAlpha, clRed);
           ceGreen: GameCanvas.StretchDraw(DestRect, SrcRect, Source, 120, clGreen);
           ceBlue: GameCanvas.StretchDraw(DestRect, SrcRect, Source, 120, clBlue);
           ceYellow: GameCanvas.StretchDraw(DestRect, SrcRect, Source, 120, clYellow);
           ceFuchsia: GameCanvas.StretchDraw(DestRect, SrcRect, Source, 120, clFuchsia);
           ceAqua: GameCanvas.StretchDraw(DestRect, SrcRect, Source, 120, clAqua);
         end;
       end;
     end; }
  end;
end;

procedure TActor.DrawWeaponGlimmer(ddx, ddy:Integer);
begin
  if (not g_ConfigDlg.ConfigCheckeds[ckHideWeaponEffect]) or (not PlugInEnabled) then begin
    if Assigned(m_WeaponEffectSurface) then begin
      if m_boWeaponEffectDrawNoBlend then begin
        GameCanvas.Draw(
          ddx + m_nWeaponEffectX + m_nShiftX,
          ddy + m_nWeaponEffectY + m_nShiftY,
          m_WeaponEffectSurface);
      end
      else begin
        GameCanvas.DrawBlend(
          ddx + m_nWeaponEffectX + m_nShiftX,
          ddy + m_nWeaponEffectY + m_nShiftY,
          m_WeaponEffectSurface);
      end;
    end;

    if Assigned(m_DBWeaponEffectSurface) then begin
      GameCanvas.DrawBlend(
        ddx + m_nDBWeaponEffectX + m_nShiftX,
        ddy + m_nDBWeaponEffectY + m_nShiftY,
        m_DBWeaponEffectSurface);
    end;
  end;
end;

procedure TActor.DrawShieldEffect(ddx, ddy:Integer);
begin
  if (not g_ConfigDlg.ConfigCheckeds[ckHideWeaponEffect]) or (not PlugInEnabled) then begin
    if Assigned(m_ShieldEffectSurface) then begin
      if m_boShieldEffectDrawNoBlend then begin
        GameCanvas.Draw(
          ddx + m_nShieldEffectX + m_nShiftX,
          ddy + m_nShieldEffectY + m_nShiftY,
          m_ShieldEffectSurface);
      end
      else begin
        GameCanvas.DrawBlend(
          ddx + m_nShieldEffectX + m_nShiftX,
          ddy + m_nShieldEffectY + m_nShiftY,
          m_ShieldEffectSurface);
      end;
    end;
  end;
end;

procedure TActor.DrawPlayEffect(ddx, ddy:Integer; IsBackActor:Boolean);
var
  I:Integer;
  ActorEffect:pTClientActorEffect;
begin
  if m_btHorse > 0 then Exit;

  // 特效
  m_ActorEffects.Lock;
  try
    for I := 0 to m_ActorEffects.Count - 1 do begin
      ActorEffect := m_ActorEffects.Items[I];

      if (ActorEffect.nLoopCount <> 0) and (
        ((ActorEffect.btDrawOrder = 0) and (not IsBackActor)) or
        ((ActorEffect.btDrawOrder <> 0) and IsBackActor)
        ) then begin
        if (ActorEffect.Texture <> nil) and (ActorEffect.nEffectFileIndex >= 0) and (ActorEffect.nEffectFileIndex < g_EffectImageList.Count) then begin
          if ActorEffect.boBlendMode then begin
            GameCanvas.DrawBlend(
              ddx + ActorEffect.nX + ActorEffect.nOffsetX + m_nShiftX,
              ddy + ActorEffect.nY + ActorEffect.nOffsetY + m_nShiftY,
              TTexture(ActorEffect.Texture));
          end else begin
            GameCanvas.Draw(
              ddx + ActorEffect.nX + ActorEffect.nOffsetX + m_nShiftX,
              ddy + ActorEffect.nY + ActorEffect.nOffsetY + m_nShiftY,
              TTexture(ActorEffect.Texture));
          end;
        end;
      end;
    end;
  finally
    m_ActorEffects.UnLock;
  end;
end;

{ TODO -ochongchong -c修改 : 人物高亮时翅膀过亮 【2013-07-30】 }

procedure TActor.DrawDressEffect(ddx, ddy:Integer; blend:Boolean; ceff:TColorEffect);
var
  nX, nY:Integer;
begin
  if Assigned(m_DressEffectSurface) then begin
    nX := ddx + m_nDressEffectX + m_nShiftX;
    nY := ddy + m_nDressEffectY + m_nShiftY;

    if m_boDressEffectDrawNoBlend then
      GameCanvas.Draw(nX, nY, m_DressEffectSurface)
    else
      GameCanvas.DrawBlend(nX, nY, m_DressEffectSurface);
  end;
end;

{ TODO -ochongchong -c修改 : 人物高亮时翅膀过亮 【2013-07-30】 }

{
procedure TActor.DrawDressEffectEx(ddx, ddy: Integer; blend: Boolean; ceff: TColorEffect);
var
  nX, nY: Integer;
begin
  if Assigned(m_DressEffectSurface) then
  begin
    nX := ddx + m_nDressEffectX + m_nShiftX;
    nY := ddy + m_nDressEffectY + m_nShiftY;
    GameCanvas.DrawBlend(nX, nY, m_DressEffectSurface);
    GameCanvas.DrawColorAlpha(nX, nY, m_DressEffectSurface, LightColor, LightAlpha, Blend_SrcAlphaColor);
  end;
end;
}

// 人物显示颜色，中毒

function TActor.GetDrawEffectValue:TColorEffect;
var
  ceff:TColorEffect;
begin
  ceff := ceNone;
  if (g_MySelf <> nil) and g_MySelf.m_boDeath then begin
    Result := ceGrayScale;
  end
  else begin
    if (g_FocusCret = Self) or (g_MagicTarget = Self) then begin
      ceff := ceBright;
    end;

    if m_boThunderPalsy then begin
      ceff := ceYellow;
    end;

    if m_nState and $80000000 <> 0 then begin
      ceff := ceGreen;
    end;

    if m_nState and $40000000 <> 0 then begin
      ceff := ceRed;
    end;

    if m_nState and $20000000 <> 0 then begin
      ceff := ceBlue;
    end;

    if m_nState and $10000000 <> 0 then begin
      ceff := ceYellow;
    end;

    if m_nState and $08000000 <> 0 then begin
      ceff := ceFuchsia;
    end;

    if m_nState and $04000000 <> 0 then begin // 石化
      ceff := ceGrayScale2;
    end;

    if m_nState and $00080000 <> 0 then begin
      ceff := ceAqua;
    end;

    // 战技锁定 STATE_CONTINUOUSMAGICLOCK
    if m_nState and $00010000 <> 0 then begin
      ceff := ceGray; //ceSilver ;
    end;

    Result := ceff;

    // BOSS变色 piaoyun 2013-09-09
    if (m_boChangeEff) and (g_ConfigDlg.ConfigCheckeds[ckColorShow]) and (frmMain.nColorShowEff + 1 in [1..9]) then begin
      Result := TColorEffect(frmMain.nColorShowEff + 1);
    end;
  end;
end;

procedure TActor.DrawChr(dx, dy:Integer; blend:Boolean; boFlag:Boolean);
var
  idx, ax, ay:Integer;
  d:TTexture;
  wimg:TGameImages;
begin
  if not (m_btDir in [0..7]) then Exit;

  if Assigned(m_BodySurface) then begin
    DrawEffSurface(
      m_BodySurface,
      dx + m_nPx + m_nShiftX,
      dy + m_nPy + m_nShiftY,
      blend,
      m_ColorEffect);

    DrawStateEffSurface(dx + m_nShiftX, dy + m_nShiftY);
  end;

  {$IF Enabled_PlugEngine = 1}
  if Assigned(HookTActor_DrawChr1) then begin
    try
      HookTActor_DrawChr1(Self, dx, dy, blend, boFlag);
    except
      on E:Exception do begin
        DebugOutStr('[Exception] HookTActor_DrawChr1');
        DebugOutStr(E.Message);
      end;
    end;
  end;
  {$IFEND}

  if m_boUseMagic and (m_CurMagic.EffectNumber > 0) then begin
    if m_nCurEffFrame in [0..m_nSpellFrame - 1] then begin
      GetEffectBase(m_CurMagic.EffectNumber - 1, 0, wimg, idx, m_CurMagic.NewLevel);
      d := nil;
      idx := idx + m_nCurEffFrame;
      if wimg <> nil then
        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          d := wimg.GetCachedGrayImage(idx, ax, ay)
        else
          d := wimg.GetCachedImage(idx, ax, ay);
      if d <> nil then
        GameCanvas.DrawBlend(
          dx + ax + m_nShiftX,
          dy + ay + m_nShiftY,
          d);
    end;
  end;

  {$IF Enabled_PlugEngine = 1}
  if Assigned(HookTActor_DrawChr2) then begin
    try
      HookTActor_DrawChr2(Self, dx, dy, blend, boFlag);
    except
      on E:Exception do begin
        DebugOutStr('[Exception] HookTActor_DrawChr2');
        DebugOutStr(E.Message);
      end;
    end;
  end;
  {$IFEND}
end;

procedure TActor.DrawEff(dx, dy:Integer);
begin

end;

function TActor.GetDefaultFrame(wmode:Boolean):Integer;
var
  cf:Integer;
  pm:pTMonsterAction;
  I, TempDir:Integer;
  MonsterConfig, TempMonsterConfig:PClientCustomMonsterConfig;
begin
  if (m_btRace in [156]) and (m_nChangeAppr >= 0) then begin
    Result := 0;
    MonsterConfig := nil;

    for I := 0 to g_CustomMonsterConfig.Count - 1 do begin
      TempMonsterConfig := g_CustomMonsterConfig.Items[I];
      if TempMonsterConfig.wMonsterAppr = m_nChangeAppr then begin
        MonsterConfig := TempMonsterConfig;
        Break;
      end;
    end;

    if MonsterConfig <> nil then begin
      if m_boDeath then begin
        if m_boSkeleton then
          Result := MonsterConfig.Actions[matDie].StartIndex
        else begin
          if MonsterConfig.Actions[matDie].CalcDir then
            TempDir := m_btDir
          else
            TempDir := 0;
          Result := MonsterConfig.Actions[matDie].StartIndex + TempDir * (MonsterConfig.Actions[matDie].PlayCount + MonsterConfig.Actions[matDie].EmptyCount) + (MonsterConfig.Actions[matDie].PlayCount - 1);
        end;
      end
      else begin
        if (m_nState and STATE_STONE_MODE) <> 0 then begin
          if MonsterConfig.Actions[matStoneRevive].CalcDir then
            TempDir := m_btDir
          else
            TempDir := 0;

          Result := MonsterConfig.Actions[matStoneRevive].StartIndex + TempDir * (MonsterConfig.Actions[matStoneRevive].PlayCount + MonsterConfig.Actions[matStoneRevive].EmptyCount);
        end
        else begin
          if m_nCurrentDefFrame < 0 then
            cf := 0
          else if m_nCurrentDefFrame >= MonsterConfig.Actions[matStand].PlayCount then
            cf := 0
          else
            cf := m_nCurrentDefFrame;

          if MonsterConfig.Actions[matStand].CalcDir then
            TempDir := m_btDir
          else
            TempDir := 0;
          Result := MonsterConfig.Actions[matStand].StartIndex + TempDir * (MonsterConfig.Actions[matStand].PlayCount + MonsterConfig.Actions[matStand].EmptyCount) + cf;
        end;
      end;
    end;
  end
  else begin
    Result := 0; // Jacky
    pm := GetRaceByPM(m_btRace, m_wAppearance);
    if pm = nil then Exit;

    if m_boDeath then begin
      if m_boSkeleton then
        Result := pm.ActDeath.start
      else
        Result := pm.ActDie.start + m_btDir * (pm.ActDie.frame + pm.ActDie.skip) + (pm.ActDie.frame - 1);
    end
    else begin
      m_nDefFrameCount := pm.ActStand.frame;
      if m_nCurrentDefFrame < 0 then
        cf := 0
      else if m_nCurrentDefFrame >= pm.ActStand.frame then
        cf := 0
      else
        cf := m_nCurrentDefFrame;
      Result := pm.ActStand.start + m_btDir * (pm.ActStand.frame + pm.ActStand.skip) + cf;
    end;
  end;
end;

function TActor.DefaultMotion:Boolean;
var
  nCurrentFrame:Integer;
begin
  m_boReverseFrame := False;
  if m_boWarMode then begin
    if (TimeGetTime - m_dwWarModeTime > 4 * 1000) then {// and not BoNextTimeFireHit then} begin
      m_boWarMode := False;
      m_boCustomMagicNoAction := False;
    end;
  end;
  nCurrentFrame := GetDefaultFrame(m_boWarMode);
  Shift(m_btDir, 0, 1, 1);
  Result := nCurrentFrame <> m_nCurrentFrame;
  m_nCurrentFrame := nCurrentFrame;
end;

{ TODO -opiaoyun -c注释 : 设置魔法音效 【2013-6-17】 }
/////////////////////////////////////////////////////////////
// 最后修改：piaoyun 2013-08-24                            //
// 此算法原本是按照 10000 + 技能ID * 10 + (0..2) 三种声音，//
// 但是技能号被历届修改乱七八糟了~ 只能大体按照此公式计算  //
// 老技能应该大部分能对上号。不对的将在下面CASE语句修正 -- //
// 详细编号列表，请查看核心文档之-背景音乐表.txt           //
/////////////////////////////////////////////////////////////

procedure TActor.SetMagicSound(wMagicID:Word);
begin
  if wMagicID > 0 then begin
    m_nMagicStartSound := 10000 + wMagicID * 10;
    m_nMagicFireSound := 10000 + wMagicID * 10 + 1;
    m_nMagicExplosionSound := 10000 + wMagicID * 10 + 2;

    // 对不上号的技能，进行声音修正
    case wMagicID of
      11: {// 雷电术去掉起手声音 chongchong 2015-11-25} begin
          m_nMagicStartSound := -1;
        end;
      62:begin
          m_nMagicStartSound := 10520;
          m_nMagicFireSound := 10521;
          m_nMagicExplosionSound := 10522;
        end;
      63:begin
          m_nMagicStartSound := 10530;
          m_nMagicFireSound := 10531;
          m_nMagicExplosionSound := 10532;
        end;
      64:begin
          m_nMagicStartSound := 10540;
          m_nMagicFireSound := 10541;
          m_nMagicExplosionSound := 10542;
        end;
      65:begin
          m_nMagicStartSound := 10550;
          m_nMagicFireSound := 10551;
          m_nMagicExplosionSound := 10552;
        end;
      58:begin // 流星火雨
          m_nMagicStartSound := s_hit_Lxhy_0;
          m_nMagicFireSound := s_hit_Lxhy_0;
          m_nMagicExplosionSound := s_hit_Lxhy_3;
        end;

      57:begin // 噬血术
          m_nMagicStartSound := 10000 + 10 * 48;
          m_nMagicFireSound := 10000 + 10 * 48 + 1;
          m_nMagicExplosionSound := 10000 + 10 * 48 + 2;
        end;
      48:begin // 气功波37
          m_nMagicStartSound := 10000 + 10 * 37;
          m_nMagicFireSound := 10000 + 10 * 37 + 1;
          m_nMagicExplosionSound := 10000 + 10 * 37 + 2;
        end;
      50:begin // 无极真气36
          m_nMagicStartSound := 10000 + 10 * 36;
          m_nMagicFireSound := 10000 + 10 * 36 + 1;
          m_nMagicExplosionSound := 10000 + 10 * 36 + 2;
        end;
      51:begin // 群体施毒术
          m_nMagicStartSound := 10060;
          m_nMagicFireSound := 10061;
          m_nMagicExplosionSound := 10062;
        end;
      52:begin // 飓风破
          m_nMagicStartSound := 10000 + 10 * 47;
          m_nMagicFireSound := 10000 + 10 * 47 + 1;
          m_nMagicExplosionSound := 10000 + 10 * 47 + 2;
        end;
      38, 46 {新诅咒术}:begin
          // 诅咒术{38}声音修复 piaoyun 2013-08-24
          m_nMagicStartSound := 10520;
          //m_nMagicFireSound := 10000 + 10 * 52 + 1;
          m_nMagicExplosionSound := 10522;
        end;
      71:begin // 擒龙手
          m_nMagicStartSound := 10000 + 10 * 28;
          m_nMagicFireSound := 10000 + 10 * 28 + 1;
          m_nMagicExplosionSound := 10000 + 10 * 28 + 2;
        end;
      72:begin // 乾坤大挪移
          m_nMagicStartSound := 10000 + 10 * 21;
          m_nMagicFireSound := 10000 + 10 * 21 + 1;
          m_nMagicExplosionSound := 10000 + 10 * 21 + 2;
        end;
      73:begin // 道力盾声音 piaoyun 2013-08-27
          m_nMagicStartSound := 10000 + 10 * 31;
          m_nMagicFireSound := 10000 + 10 * 31 + 1;
          m_nMagicExplosionSound := 10000 + 10 * 31 + 2;
        end;
      87:begin // 武力盾声音 piaoyun 2013-08-27
          m_nMagicStartSound := 10000 + 10 * 31;
          m_nMagicFireSound := 10000 + 10 * 31 + 1;
          m_nMagicExplosionSound := 10000 + 10 * 31 + 2;
        end;
      88:begin // 新武力盾声音 piaoyun 2013-08-27
          m_nMagicStartSound := 10000 + 10 * 31;
          m_nMagicFireSound := 10000 + 10 * 31 + 1;
          m_nMagicExplosionSound := 10000 + 10 * 31 + 2;
        end;
      89:begin // 新道力盾声音 piaoyun 2013-08-27
          m_nMagicStartSound := 10000 + 10 * 31;
          m_nMagicFireSound := 10000 + 10 * 31 + 1;
          m_nMagicExplosionSound := 10000 + 10 * 31 + 2;
        end;
      76:begin // 召唤圣兽
          m_nMagicStartSound := 10000 + 10 * 30;
          m_nMagicFireSound := 10000 + 10 * 30 + 1;
          m_nMagicExplosionSound := 10000 + 10 * 30 + 2;
        end;
      107:begin // 双龙破
          m_nMagicStartSound := s_cboFs1_start;
          m_nMagicExplosionSound := s_cboFs1_target;
        end;
      104:begin // 凤舞祭
          m_nMagicStartSound := s_cboFs2_start;
          m_nMagicExplosionSound := s_cboFs2_target;
        end;
      105:begin // 惊雷爆
          m_nMagicStartSound := s_cboFs3_start;
          m_nMagicExplosionSound := s_cboFs3_target;
        end;
      106:begin // 冰天雪地
          m_nMagicStartSound := s_cboFs4_start;
          m_nMagicExplosionSound := s_cboFs4_target;
        end;
      108:begin // 虎啸诀
          m_nMagicStartSound := s_cboDs1_start;
          m_nMagicExplosionSound := s_cboDs1_target;
        end;
      109:begin // 八卦掌
          m_nMagicStartSound := s_cboDs2_start;
          m_nMagicExplosionSound := s_cboDs2_target;
        end;
      110:begin // 三焰咒
          m_nMagicStartSound := s_cboDs3_start;
          m_nMagicExplosionSound := s_cboDs3_target;
        end;
      111:begin // 万剑归宗
          m_nMagicStartSound := s_cboDs4_start;
          m_nMagicExplosionSound := s_cboDs4_target;
        end;
      114:begin // 倚天辟地
          m_nMagicStartSound := s_xsls_death;
          m_nMagicExplosionSound := s_xsws_pbec;
        end;
      116:begin // 血魄一击(法) 声音
          m_nMagicStartSound := 11036;
          m_nMagicExplosionSound := 11037;
        end;
      117:begin // 血魄一击(道) 声音
          m_nMagicStartSound := 11040;
          m_nMagicExplosionSound := 11041;
        end;
      199:begin
          m_nMagicStartSound := 11000;
          m_nMagicFireSound := 0;
          m_nMagicExplosionSound := 11002;
        end;
      200:begin
          m_nMagicStartSound := 11010;
          m_nMagicFireSound := 0;
          m_nMagicExplosionSound := 11012;
        end;
      // 新技能声音 -- 2013-6-17
      201:begin
          m_nMagicStartSound := 10330;
          m_nMagicFireSound := 10331;
          m_nMagicExplosionSound := 10430;
        end;
      202: {// 裂神符声音} begin
          m_nMagicStartSound := 10130;
          m_nMagicFireSound := 10131;
          m_nMagicExplosionSound := 10132;
        end;
      203: {// 死亡之眼声音} begin
          m_nMagicStartSound := 10490;
          m_nMagicFireSound := 10491;
          m_nMagicExplosionSound := 10492;
        end;
      204: {// 十步一杀声音} begin
          m_nMagicStartSound := 10461;
          m_nMagicFireSound := 0;
          m_nMagicExplosionSound := 10522;
        end;
      205: {// 冰霜雪雨声音} begin
          m_nMagicStartSound := s_hit_Lxhy_0;
          m_nMagicFireSound := 0;
          m_nMagicExplosionSound := s_hit_Lxhy_3;
        end;
      206: {// 冰霜群雨声音} begin
          m_nMagicStartSound := s_xf;
          m_nMagicFireSound := 0;
          m_nMagicExplosionSound := s_xsws_pbec;
        end;
      208:begin
          // 208 {旋风斩} chongchong 2014-09-14
          m_nMagicStartSound := 11060;
          m_nMagicFireSound := 0;
          m_nMagicExplosionSound := 0;
        end;
      34: {// 解毒术声音 chongchong 2013-11-19} begin
          m_nMagicStartSound := 10020;
          m_nMagicFireSound := 0;
          m_nMagicExplosionSound := 10492;
        end;
      41: {// 狮子吼声音 chongchong 2013-12-10} begin
          m_nMagicStartSound := 10430;
          m_nMagicFireSound := 0;
          m_nMagicExplosionSound := 0;
        end;
    end;
  end;
end;

// 人物动作声音(脚步声、武器攻击声)

procedure TActor.SetSound;
var
  cx, cy, bidx, wunit, attackweapon:Integer;
  hiter:TActor;
begin
  if m_btRace in [0, 1] then begin
    if (Self = g_MySelf) and
      ((m_nCurrentAction = SM_WALK) or
      (m_nCurrentAction = SM_BACKSTEP) or
      (m_nCurrentAction = SM_RUN) or
      (m_nCurrentAction = SM_HORSERUN) or
      (m_nCurrentAction = SM_RUSH) or
      (m_nCurrentAction = SM_RUSHKUNG) or
      (m_nCurrentAction = SM_100HIT)
      )
      then begin
      cx := g_MySelf.m_nCurrX - Map.m_nBlockLeft;
      cy := g_MySelf.m_nCurrY - Map.m_nBlockTop;
      cx := cx div 2 * 2;
      cy := cy div 2 * 2;

      bidx := Map.m_MArr[cx, cy].wBkImg and $7FFF;
      wunit := Map.m_MArr[cx, cy].btArea;

      bidx := wunit * 10000 + bidx - 1;
      case bidx of
        // 陋篮 钱
        330..349, 450..454, 550..554, 750..754,
          950..954, 1250..1254, 1400..1424, 1455..1474,
          1500..1524, 1550..1574:
          m_nFootStepSound := s_walk_lawn_l;

        // 吝埃钱

        // 变 钱
        250..254, 1005..1009, 1050..1054, 1060..1064, 1450..1454,
          1650..1654:
          m_nFootStepSound := s_walk_rough_l;

        // 倒 辨
        // 措府籍 官蹿
        605..609, 650..654, 660..664, 2000..2049,
          3025..3049, 2400..2424, 4625..4649, 4675..4678:
          m_nFootStepSound := s_walk_stone_l;

        // 悼奔救
        1825..1924, 2150..2174, 3075..3099, 3325..3349,
          3375..3399:
          m_nFootStepSound := s_walk_cave_l;

        // 唱公官蹿
        3230, 3231, 3246, 3277:
          m_nFootStepSound := s_walk_wood_l;

        // 带傈..
        3780..3799:
          m_nFootStepSound := s_walk_wood_l;

        3825..4434:
          if (bidx - 3825) mod 25 = 0 then
            m_nFootStepSound := s_walk_wood_l
          else if m_btHorse = 0 then
            m_nFootStepSound := s_walk_ground_l
          else
            m_nFootStepSound := s_horse_walk_ground_l;

        // 笼救(家府 喊风 救巢)
        2075..2099, 2125..2149:
          m_nFootStepSound := s_walk_room_l;

        // 俺匡
        1800..1824:
          m_nFootStepSound := s_walk_water_l;

        else begin
            if m_btHorse = 0 then
              m_nFootStepSound := s_walk_ground_l
            else
              m_nFootStepSound := s_horse_walk_ground_l;
          end;
      end;
      // 泵傈郴何
      if (bidx >= 825) and (bidx <= 1349) then begin
        if ((bidx - 825) div 25) mod 2 = 0 then
          m_nFootStepSound := s_walk_stone_l;
      end;
      // 悼奔郴何
      if (bidx >= 1375) and (bidx <= 1799) then begin
        if ((bidx - 1375) div 25) mod 2 = 0 then
          m_nFootStepSound := s_walk_cave_l;
      end;
      case bidx of
        1385, 1386, 1391, 1392:
          m_nFootStepSound := s_walk_wood_l;
      end;

      bidx := Map.m_MArr[cx, cy].wMidImg and $7FFF;

      bidx := bidx - 1;
      case bidx of
        0..115:begin
            if m_btHorse = 0 then
              m_nFootStepSound := s_walk_ground_l
            else
              m_nFootStepSound := s_horse_walk_ground_l;
          end;
        120..124:
          m_nFootStepSound := s_walk_lawn_l;
      end;

      bidx := Map.m_MArr[cx, cy].wFrImg and $7FFF;
      bidx := bidx - 1;
      case bidx of
        // 寒倒辨
        221..289, 583..658, 1183..1206, 7163..7295,
          7404..7414:
          m_nFootStepSound := s_walk_stone_l;
        // 唱公付风
        3125..3267, {3319..3345, 3376..3433,} 3757..3948,
        6030..6999:
          m_nFootStepSound := s_walk_wood_l;
        // 规官蹿
        3316..3589:
          m_nFootStepSound := s_walk_room_l;
      end;
      if (m_nCurrentAction = SM_RUN) or (m_nCurrentAction = SM_HORSERUN) then
        m_nFootStepSound := m_nFootStepSound + 2;

    end;

    if m_btSex = 0 then begin // 男
      m_nScreamSound := s_man_struck;
      m_nDieSound := s_man_die;
    end
    else begin // 女
      m_nScreamSound := s_wom_struck;
      m_nDieSound := s_wom_die;
    end;

    case m_nCurrentAction of
      SM_THROW, SM_HIT, SM_HIT + 1, SM_HIT + 2, SM_POWERHIT, SM_LONGHIT, SM_WIDEHIT, SM_FIREHIT, SM_CRSHIT, SM_TWNHIT, SM_43HIT, SM_66HIT {开天斩重击}, SM_66HIT1 {开天斩轻击}, 113 {断空斩},
      SM_CUSTOM_HIT001..(SM_CUSTOM_HIT001 + CUSTOM_MAGIC_COUNT - 1):begin
          if m_wWeaponSound = 0 then begin
            case (m_wWeapon div 2) of
              6, 20:m_nWeaponSound := s_hit_short;
              1:m_nWeaponSound := s_hit_wooden;
              2, 13, 9, 5, 14, 22:m_nWeaponSound := s_hit_sword;
              4, 17, 10, 15, 16, 23:m_nWeaponSound := s_hit_do;
              3, 7, 11:m_nWeaponSound := s_hit_axe;
              24:m_nWeaponSound := s_hit_club;
              8, 12, 18, 21:m_nWeaponSound := s_hit_long;
              else
                m_nWeaponSound := s_hit_fist;
            end;
          end
          else begin
            m_nWeaponSound := m_wWeaponSound;
          end;
        end;
      SM_101HIT:begin
          case m_btSex of
            0:m_nWeaponSound := s_cboZs1_start_m;
            1:m_nWeaponSound := s_cboZs1_start_w;
          end;
        end;
      SM_100HIT:m_nWeaponSound := s_cboZs2_start;

      SM_102HIT:begin
          case m_btSex of
            0:m_nWeaponSound := s_cboZs3_start_m;
            1:m_nWeaponSound := s_cboZs3_start_w;
          end;
        end;
      SM_103HIT:m_nWeaponSound := s_cboZs4_start;

      SM_60HIT:m_nWeaponSound := 10510;
      SM_61HIT:m_nWeaponSound := 10511;
      SM_62HIT:m_nWeaponSound := 10511;

      SM_SWORDHIT:begin
          case m_btSex of
            0:m_nWeaponSound := s_hit_ZRJF_M;
            1:m_nWeaponSound := s_hit_ZRJF_W;
          end;
        end;
      SM_STRUCK:begin
          if m_nMagicStruckSound >= 1 then begin
          end
          else begin
            hiter := PlayScene.FindActor(m_nHiterCode);
            if hiter <> nil then begin
              attackweapon := hiter.m_wWeapon div 2;
              if hiter.m_btRace in [0, 1] then
                case (m_wDress div 2) of
                  3:
                    case attackweapon of
                      6:m_nStruckSound := s_struck_armor_sword;
                      1, 2, 4, 5, 9, 10, 13, 14, 15, 16, 17:m_nStruckSound := s_struck_armor_sword;
                      3, 7, 11:m_nStruckSound := s_struck_armor_axe;
                      8, 12, 18:m_nStruckSound := s_struck_armor_longstick;
                      else
                        m_nStruckSound := s_struck_armor_fist;
                    end;
                  else
                    case attackweapon of
                      6:m_nStruckSound := s_struck_body_sword;
                      1, 2, 4, 5, 9, 10, 13, 14, 15, 16, 17:m_nStruckSound := s_struck_body_sword;
                      3, 7, 11:m_nStruckSound := s_struck_body_axe;
                      8, 12, 18:m_nStruckSound := s_struck_body_longstick;
                      else
                        m_nStruckSound := s_struck_body_fist;
                    end;
                end;
            end;
          end;
        end;
    end;

    if m_boUseMagic and (m_CurMagic.MagicSerial > 0) then begin
      SetMagicSound(m_CurMagic.MagicSerial);
    end;

  end
  else begin
    if m_nCurrentAction = SM_STRUCK then begin
      if m_nMagicStruckSound >= 1 then begin // 付过栏肺 嘎澜
        // strucksound := s_struck_magic;  //烙矫..
      end
      else begin
        hiter := PlayScene.FindActor(m_nHiterCode);
        if hiter <> nil then begin // 锭赴仇捞 公均栏肺 锭啡绰瘤 八荤
          attackweapon := hiter.m_wWeapon div 2;
          case attackweapon of
            6:m_nStruckSound := s_struck_body_sword;
            1, 2, 4, 5, 9, 10, 13, 14, 15, 16, 17:m_nStruckSound := s_struck_body_sword;
            3, 11:m_nStruckSound := s_struck_body_axe;
            8, 12, 18:m_nStruckSound := s_struck_body_longstick;
            else
              m_nStruckSound := s_struck_body_fist;
          end;
        end;
      end;
    end;

    if m_boUseMagic and (m_CurMagic.MagicSerial > 0) then begin
      SetMagicSound(m_CurMagic.MagicSerial);
    end;

    if m_btRace = 50 then begin
      // //
    end
    else begin
      m_nAppearSound := 200 + (m_wAppearance) * 10;
      m_nNormalSound := 200 + (m_wAppearance) * 10 + 1;
      m_nAttackSound := 200 + (m_wAppearance) * 10 + 2; // 快况撅
      m_nWeaponSound := 200 + (m_wAppearance) * 10 + 3; // 茸(公扁戎滴冯)
      m_nScreamSound := 200 + (m_wAppearance) * 10 + 4;
      m_nDieSound := 200 + (m_wAppearance) * 10 + 5;
      m_nDie2Sound := 200 + (m_wAppearance) * 10 + 6;

      // 新骷髅测试 piaoyun 2013-07-27
      if (m_wAppearance >= 950) and (m_wAppearance <= 952) then begin
        // 设置成变异骷髅一样音效
        m_nAppearSound := 200 + (37) * 10;
        m_nNormalSound := 200 + (37) * 10 + 1;
        m_nAttackSound := 200 + (37) * 10 + 2;
        m_nWeaponSound := 200 + (37) * 10 + 3;
        m_nScreamSound := 200 + (37) * 10 + 4;
        m_nDieSound := 200 + (37) * 10 + 5;
        m_nDie2Sound := 200 + (37) * 10 + 6;
      end;

      if (m_wAppearance >= 270) and (m_wAppearance <= 275) then begin // 圣兽
        if m_wAppearance mod 2 = 0 then begin
          m_nAppearSound := 200 + 170 * 10;
          m_nNormalSound := 200 + 170 * 10 + 1;
          m_nAttackSound := 200 + 170 * 10 + 2; // 快况撅
          m_nWeaponSound := 200 + 170 * 10 + 3; // 茸(公扁戎滴冯)
          m_nScreamSound := 200 + 170 * 10 + 4;
          m_nDieSound := 200 + 170 * 10 + 5;
          m_nDie2Sound := 200 + 170 * 10 + 6;
        end
        else begin
          m_nAppearSound := 200 + 171 * 10;
          m_nNormalSound := 200 + 171 * 10 + 1;
          m_nAttackSound := 200 + 171 * 10 + 2; // 快况撅
          m_nWeaponSound := 200 + 171 * 10 + 3; // 茸(公扁戎滴冯)
          m_nScreamSound := 200 + 171 * 10 + 4;
          m_nDieSound := 200 + 171 * 10 + 5;
          m_nDie2Sound := 200 + 171 * 10 + 6;
        end;
      end;

      // mon41神兽声音 chongchong 2014-11-26
      if m_wAppearance = 401 then begin
        m_nAppearSound := 200 + 170 * 10;
        m_nNormalSound := 200 + 170 * 10 + 1;
        m_nAttackSound := 200 + 170 * 10 + 2; // 快况撅
        m_nWeaponSound := 200 + 170 * 10 + 3; // 茸(公扁戎滴冯)
        m_nScreamSound := 200 + 170 * 10 + 4;
        m_nDieSound := 200 + 170 * 10 + 5;
        m_nDie2Sound := 200 + 170 * 10 + 6;
      end
      else if m_wAppearance = 402 then begin
        m_nAppearSound := 200 + 171 * 10;
        m_nNormalSound := 200 + 171 * 10 + 1;
        m_nAttackSound := 200 + 171 * 10 + 2; // 快况撅
        m_nWeaponSound := 200 + 171 * 10 + 3; // 茸(公扁戎滴冯)
        m_nScreamSound := 200 + 171 * 10 + 4;
        m_nDieSound := 200 + 171 * 10 + 5;
        m_nDie2Sound := 200 + 171 * 10 + 6;
      end;
    end;
  end;

  if m_nCurrentAction = SM_STRUCK then begin
    hiter := PlayScene.FindActor(m_nHiterCode);
    if hiter <> nil then begin // 锭赴仇捞 公均栏肺 锭啡绰瘤 八荤
      attackweapon := hiter.m_wWeapon div 2;
      if hiter.m_btRace in [0, 1] then
        case (attackweapon div 2) of
          6, 20:m_nStruckWeaponSound := s_struck_short;
          1:m_nStruckWeaponSound := s_struck_wooden;
          2, 13, 9, 5, 14, 22:m_nStruckWeaponSound := s_struck_sword;
          4, 17, 10, 15, 16, 23:m_nStruckWeaponSound := s_struck_do;
          3, 7, 11:m_nStruckWeaponSound := s_struck_axe;
          24:m_nStruckWeaponSound := s_struck_club;
          8, 12, 18, 21:m_nStruckWeaponSound := s_struck_wooden; // long;
          // else struckweaponsound := s_struck_fist;
        end;
    end;
  end;
end;

procedure TActor.RunSound;
var
  CustomMagicConfig:PClientCustomMagicConfig;
  MagicPlusLevel:TMagicPlusLevel;
  ClientConfig:PMagicClientConfig;
begin
  m_boRunSound := True;
  SetSound;
  case m_nCurrentAction of
    SM_STRUCK:begin
        if (m_nStruckWeaponSound >= 0) then g_PlaySound.PlaySound(m_nStruckWeaponSound);
        if (m_nStruckSound >= 0) then g_PlaySound.PlaySound(m_nStruckSound);
        if (m_nScreamSound >= 0) then g_PlaySound.PlaySound(m_nScreamSound);
      end;
    SM_NOWDEATH:begin
        if (m_nDieSound >= 0) and m_boDeath then begin
          PlaySound(m_nDieSound);
          // if Self.m_btRace = RC_USERHUMAN then
          if Self = g_MySelf then
            frmMain.SendDelayMsg(g_MySelf, RCM_PlayBGMgameover, 0, 0, 0, 0, '', 500);
          // PlayBGM(bmg_gameover176);
        end;
      end;
    SM_THROW, SM_HIT, SM_FLYAXE, SM_LIGHTING, SM_DIGDOWN:begin
        if m_nAttackSound >= 0 then g_PlaySound.PlaySound(m_nAttackSound);
      end;
    SM_ALIVE, SM_DIGUP:begin
        g_PlaySound.PlaySound(m_nAppearSound);
      end;
    SM_SPELL:begin
        CustomMagicConfig := GetCustomMagicConfig(m_CurMagic.MagicSerial);

        if CustomMagicConfig <> nil then begin
          case m_CurMagic.NewLevel of
            0:MagicPlusLevel := mplNone;
            1..3:MagicPlusLevel := mpl1_3;
            4..6:MagicPlusLevel := mpl4_6;
            7..9:MagicPlusLevel := mpl7_9;
            else
              MagicPlusLevel := mpl7_9;
          end;

          ClientConfig := @CustomMagicConfig.MagicConfigs[MagicPlusLevel];

          if (Length(ClientConfig.Sounds[cmstUseMagic]) > 0) then begin
            PlaySound(ClientConfig.Sounds[cmstUseMagic]);
          end;
        end
        else
          g_PlaySound.PlaySound(m_nMagicStartSound);
      end;
  end;
  { SetSound;
   if m_wAppearance >= 230 then begin
     case m_nCurrentAction of
       SM_STRUCK:
         begin
           if (m_nStruckWeaponSound >= 0) then g_PlaySound.PlaySound(m_nStruckWeaponSound);
           if (m_nStruckSound >= 0) then g_PlaySound.PlaySound(m_nStruckSound);
           if (m_nScreamSound >= 0) then g_PlaySound.PlaySound(m_nScreamSound);
         end;
       SM_NOWDEATH:
         begin
           if (m_nDieSound >= 0) then begin
             PlaySound(m_nDieSound);

           end;
         end;
       SM_THROW, SM_HIT, SM_FLYAXE, SM_LIGHTING, SM_DIGDOWN:
         begin
           if m_nAttackSound >= 0 then g_PlaySound.PlaySound(m_nAttackSound);
         end;
       SM_ALIVE, SM_DIGUP:
         begin
           g_PlaySound.PlaySound(m_nAppearSound);
         end;
       SM_SPELL:
         begin
           g_PlaySound.PlaySound(m_nMagicStartSound);
         end;
     end;
   end else begin

     case m_nCurrentAction of
       SM_STRUCK:
         begin
           if (m_nStruckWeaponSound >= 0) then g_PlaySound.PlaySound(m_nStruckWeaponSound);
           if (m_nStruckSound >= 0) then g_PlaySound.PlaySound(m_nStruckSound);
           if (m_nScreamSound >= 0) then g_PlaySound.PlaySound(m_nScreamSound);
         end;
       SM_NOWDEATH:
         begin
           if (m_nDieSound >= 0) then begin
             PlaySound(m_nDieSound);
 // if Self.m_btRace = RC_USERHUMAN then
             if Self = g_MySelf then
               PlayBGM(bmg_gameover176);
           end;
         end;
       SM_THROW, SM_HIT, SM_FLYAXE, SM_LIGHTING, SM_DIGDOWN:
         begin
           if m_nAttackSound >= 0 then g_PlaySound.PlaySound(m_nAttackSound);
         end;
       SM_ALIVE, SM_DIGUP:
         begin
           g_PlaySound.PlaySound(m_nAppearSound);
         end;
       SM_SPELL:
         begin
           g_PlaySound.PlaySound(m_nMagicStartSound);
         end;
     end;
   end;  }
end;

procedure TActor.RunActSound(frame:Integer);
var
  CustomMagicConfig:PClientCustomMagicConfig;
  MagicPlusLevel:TMagicPlusLevel;
  ClientConfig:PMagicClientConfig;
begin
  if m_boRunSound then begin
    if m_btRace in [0, 1] then begin
      case m_nCurrentAction of
        SM_THROW, SM_HIT, SM_HIT + 1, SM_HIT + 2:
          if frame = 2 then begin
            g_PlaySound.PlaySound(m_nWeaponSound);
            m_boRunSound := False; // 附加声音
          end;
        SM_CUSTOM_HIT001..(SM_CUSTOM_HIT001 + CUSTOM_MAGIC_COUNT - 1):begin
            if frame = 2 then begin
              CustomMagicConfig := GetCustomMagicConfig(m_nCurrentAction - SM_CUSTOM_HIT001 + CUSTOM_MAGIC_START_ID);
              if CustomMagicConfig <> nil then begin
                case m_CurMagic.NewLevel of
                  0:MagicPlusLevel := mplNone;
                  1..3:MagicPlusLevel := mpl1_3;
                  4..6:MagicPlusLevel := mpl4_6;
                  7..9:MagicPlusLevel := mpl7_9;
                  else
                    MagicPlusLevel := mpl7_9;
                end;

                ClientConfig := @CustomMagicConfig.MagicConfigs[MagicPlusLevel];

                if (m_btSex = 0) and (Length(ClientConfig.Sounds[cmstManWarr]) > 0) then begin
                  PlaySound(ClientConfig.Sounds[cmstManWarr]);
                  m_boRunSound := False;
                end
                else if (m_btSex = 1) and (Length(ClientConfig.Sounds[cmstWomanWarr]) > 0) then begin
                  PlaySound(ClientConfig.Sounds[cmstWomanWarr]);
                  m_boRunSound := False;
                end
                else begin
                  g_PlaySound.PlaySound(m_nWeaponSound);
                  m_boRunSound := False; // 附加声音
                end;
              end;
            end;
          end;
        SM_POWERHIT:
          if frame = 2 then begin
            g_PlaySound.PlaySound(m_nWeaponSound);
            if m_btSex = 0 then
              g_PlaySound.PlaySound(s_yedo_man)
            else
              g_PlaySound.PlaySound(s_yedo_woman);
            m_boRunSound := False; // 茄锅父 家府晨
          end;
        SM_LONGHIT:
          if frame = 2 then begin
            g_PlaySound.PlaySound(m_nWeaponSound);
            g_PlaySound.PlaySound(s_longhit);
            m_boRunSound := False; // 茄锅父 家府晨
          end;
        SM_WIDEHIT:
          if frame = 2 then begin
            g_PlaySound.PlaySound(m_nWeaponSound);
            g_PlaySound.PlaySound(s_widehit);
            m_boRunSound := False; // 茄锅父 家府晨
          end;
        SM_FIREHIT:
          if frame = 2 then begin
            g_PlaySound.PlaySound(m_nWeaponSound);
            g_PlaySound.PlaySound(s_firehit);
            m_boRunSound := False; // 茄锅父 家府晨
          end;
        SM_TWNHIT, SM_CRSHIT, SM_43HIT:
          if frame = 2 then begin
            g_PlaySound.PlaySound(m_nWeaponSound);
            if m_nCurrentAction = SM_TWNHIT then // 龙影剑法声音 chongchong 2013-11-19
              g_PlaySound.PlaySound(11058)
            else
              g_PlaySound.PlaySound(s_widehit);
            m_boRunSound := False; // 茄锅父 家府晨
          end;
        SM_60HIT: {// 破魂斩声音 chongchong 2014-10-15} begin
            if frame = 2 then begin
              g_PlaySound.PlaySound(m_nWeaponSound);
              PlaySound(s_phz);
              m_boRunSound := False;
            end;
          end;
        SM_61HIT: {// 劈星斩声音待加} begin
            if frame = 2 then begin
              PlaySound(124);
              PlaySound(10512);

              //g_PlaySound.PlaySound(m_nWeaponSound);
              m_boRunSound := False;
            end;
          end;
        SM_SWORDHIT:
          if frame = 2 then begin
            g_PlaySound.PlaySound(m_nWeaponSound);
            g_PlaySound.PlaySound(s_firehit);
            m_boRunSound := False;
          end;
        SM_100HIT..SM_103HIT:
          if frame = 2 then begin
            g_PlaySound.PlaySound(m_nWeaponSound);
            g_PlaySound.PlaySound(s_firehit);
            m_boRunSound := False;
            // 断岳斩屏幕震动 piaoyun 2013-09-14
            if (m_nCurrentAction = SM_102HIT) and (g_ConfigDlg.ConfigCheckeds[ckSceneShake]) then begin
              PlayScene.SceneShake();
            end;

          end;
        SM_66HIT, SM_66HIT1: //开天斩轻击、重击声音 piaoyun 2013-08-24
          if frame = 2 then begin
            g_PlaySound.PlaySound(11056);
            m_boRunSound := FALSE;
          end;

        // 断空斩声音 chongchong 2018-01-29
        SM_113HIT:
          if frame = 2 then begin
            if m_btSex = 0 then
              g_PlaySound.PlaySound(11030)
            else
              g_PlaySound.PlaySound(11031);

            m_boRunSound := FALSE;
          end;

        // 血魄一击(战)声音 chongchong 2018-01-29
        SM_115HIT:
          if frame = 2 then begin
            if m_btSex = 0 then
              g_PlaySound.PlaySound(11033)
            else
              g_PlaySound.PlaySound(11034);

            m_boRunSound := FALSE;
          end;
      end;
    end
    else begin
      if m_btRace = 50 then begin
      end
      else begin
        // add chongchong 修改神兽叫 【2013-07-24】
        // if (m_nCurrentAction = SM_WALK) or (m_nCurrentAction = SM_TURN) then
        if (m_nCurrentAction = SM_TURN) then begin
          if (frame = 1) and (Random(8) = 1) then begin
            g_PlaySound.PlaySound(m_nNormalSound);
            m_boRunSound := False;
          end;
        end;
        if m_nCurrentAction = SM_HIT then begin
          if (frame = 3) and (m_nAttackSound >= 0) then begin
            g_PlaySound.PlaySound(m_nWeaponSound);
            m_boRunSound := False;
          end;
        end;
        case m_wAppearance of
          80:begin
              if m_nCurrentAction = SM_NOWDEATH then begin
                if (frame = 2) then begin
                  g_PlaySound.PlaySound(m_nDie2Sound);
                  m_boRunSound := False;
                end;
              end;
            end;
        end;
      end;

      // Mon36_X 怪物声音 piaoyun 2013-12-06
      if (m_btRace = 202) or (m_btRace = 203) or
        (m_btRace = 204) or (m_btRace = 205) or
        (m_btRace = 206) or (m_btRace = 207) or
        (m_btRace = 208) or (m_btRace = 209) then begin
        if (m_nCurrentAction = SM_TURN) then begin
          g_PlaySound.PlaySound(542);
          m_boRunSound := False;
        end;
        if m_nCurrentAction = SM_STRUCK then begin
          g_PlaySound.PlaySound(495);
          m_boRunSound := False;
        end;
        if m_nCurrentAction = SM_NOWDEATH then begin
          g_PlaySound.PlaySound(496);
          m_boRunSound := False;
        end;
      end;
    end;
  end;
end;

procedure TActor.RunFrameAction(frame:Integer);
begin
end;

procedure TActor.ActionChanged;
begin

end;

procedure TActor.ActionEnded;
begin
  if Self = g_MySelf then begin
    if {(m_nCurrentAction >= SM_100HIT) and (m_nCurrentAction <= SM_103HIT) or }((m_nCurrentAction = SM_SPELL)
    and (m_CurMagic.EffectNumber in [104..111])) then begin // 连击
        frmMain.StartContinuousMagicAttack; // 连击魔法
    end else begin
        //Self.m_nCurrentAction := 0; //HZQ 20230703 本轮动作完成
    end;  
  end;
  // frmMain.SendDelayMsg(g_MySelf, RCM_StartContinuousMagicAttack, 0, 0, 0, 0, '', 100);
  m_dwActionEndTime := TimeGetTime;
end;

function TActor.CheckLoadUserName:Boolean;
var
  sShowName:string;
begin
  Result := False;
  if (not GameCanvas.Active) or (not GameCanvas.Initialized) then Exit;
  m_sNameText := '';

  if PlugInEnabled then begin
    if not ((g_ClientConfig.boHideGhost and g_ConfigDlg.ConfigCheckeds[ckHideGhost]) and m_boDeath) then begin
      if m_boDeath and m_IsExploreItem and (not m_boSkeleton) then begin
        m_sNameText := '(可探索)\' + m_sDescUserName + '\' + m_sUserName;
      end
      else if g_ClientConfig.boShowMonName and g_ConfigDlg.ConfigCheckeds[ckShowMonName] and (not m_boDeath) and ((m_HumsBBType = bbNo) or g_ClientConfig.boSlaveAlwaysShowName) then begin
        m_sNameText := m_sDescUserName + '\' + m_sUserName;

        if (g_FocusCret = Self) and (g_ClientConfig.btMonsterShowLevel = 2) and (Length(g_ClientConfig.sMonsterShowLevelFormat) > 0) and (m_Abil.Level > 0) and not (m_btRealRace in [RC_BOX, RC_BOX2]) then begin
          sShowName := StringReplace(g_ClientConfig.sMonsterShowLevelFormat, '%d', IntToStr(m_Abil.Level), [rfReplaceAll, rfIgnoreCase]);
          sShowName := StringReplace(sShowName, '%s', m_sUserName, [rfReplaceAll, rfIgnoreCase]);

          m_sNameText := m_sDescUserName + '\' + sShowName;
        end;
      end
      else if (g_FocusCret = Self) then begin
        if (g_ClientConfig.btMonsterShowLevel = 2) and (Length(g_ClientConfig.sMonsterShowLevelFormat) > 0) and (m_Abil.Level > 0) and not (m_btRealRace in [RC_BOX, RC_BOX2]) then begin
          sShowName := StringReplace(g_ClientConfig.sMonsterShowLevelFormat, '%d', IntToStr(m_Abil.Level), [rfReplaceAll, rfIgnoreCase]);
          sShowName := StringReplace(sShowName, '%s', m_sUserName, [rfReplaceAll, rfIgnoreCase]);

          m_sNameText := m_sDescUserName + '\' + sShowName;
        end
        else begin
          m_sNameText := m_sDescUserName + '\' + m_sUserName;
        end;
      end;
    end;
  end
  else begin
    if (g_FocusCret = Self) then begin
      if (g_ClientConfig.btMonsterShowLevel = 2) and (Length(g_ClientConfig.sMonsterShowLevelFormat) > 0) and (m_Abil.Level > 0) and not (m_btRealRace in [RC_BOX, RC_BOX2]) then begin
        sShowName := StringReplace(g_ClientConfig.sMonsterShowLevelFormat, '%d', IntToStr(m_Abil.Level), [rfReplaceAll, rfIgnoreCase]);
        sShowName := StringReplace(sShowName, '%s', m_sUserName, [rfReplaceAll, rfIgnoreCase]);

        m_sNameText := m_sDescUserName + '\' + sShowName;
      end
      else begin
        m_sNameText := m_sDescUserName + '\' + m_sUserName;
      end;
    end;
  end;

  // 尝试修复npc乱码，chongchong 2016-02-23 原来与 NameTimeTick 相关的是 30s，现改为 3s NameTimeTick > 1000 * 3
  Result := (CompareText(m_sCurNameText, m_sNameText) <> 0) (*or (MyGetTickCount - m_dwShowNameTimeTick > 1000 * 2)*);
end;

function TActor.CheckLoadActorIcon:Boolean;
var
  I:Integer;
  ActorIcon:pTActorIcon;
  ActorIconIndex:pTActorIconIndex;
begin
  //顶戴花翎
  Result := False;
  if (not GameCanvas.Active) or (not GameCanvas.Initialized) then begin
    for I := Low(TActorIconArray) to High(TActorIconArray) do begin
      m_ActorIconIndexs[I].Texture := nil;
    end;
    Exit;
  end;

  for I := Low(TActorIconArray) to High(TActorIconArray) do begin
    ActorIcon := @m_ActorIcons[I];
    ActorIconIndex := @m_ActorIconIndexs[I];
    if (ActorIcon.nFileIndex >= 0) and (ActorIcon.nFileIndex <= g_EffectImageList.Count + 1) and (ActorIcon.nIconCount >= 1) then begin
      if (ActorIconIndex.nCurrentFrame <= ActorIconIndex.nEndFrame - 1)
        and (TimeGetTime - ActorIconIndex.dwTick >= Cardinal(ActorIcon.nPlayTime)) and (ActorIcon.nIconCount >= 2) then begin
        Inc(ActorIconIndex.nCurrentFrame);
        ActorIconIndex.dwTick := TimeGetTime;
      end;

      if ActorIconIndex.nCurrentFrame > ActorIconIndex.nEndFrame - 1 then begin
        ActorIconIndex.nOCurrentFrame := -1;
        ActorIconIndex.nCurrentFrame := ActorIcon.nIconIndex;
        ActorIconIndex.dwTick := TimeGetTime;
      end;

      if (ActorIconIndex.nOCurrentFrame <> ActorIconIndex.nCurrentFrame) or (TimeGetTime - ActorIconIndex.dwLoadTick >= 1000 * 2) then begin
        Result := True;
        ActorIconIndex.nOCurrentFrame := ActorIconIndex.nCurrentFrame;
      end;
    end;
  end;
end;

procedure TActor.LoadActorIcons;
var
  I:Integer;
  GameImages:TGameImages;
begin
  // 顶戴花翎
  if (not GameCanvas.Active) or (not GameCanvas.Initialized) then begin
    Exit;
  end;

  if (g_ConfigDlg.ConfigCheckeds[ckHideActorIcons]) and
    (
    ((m_btRace = RC_PLAYOBJECT) and (not m_boPlayMoster)) or
    (m_btRace = RC_HEROOBJECT)
    ) then Exit;

  //HZQ 20230605 增加隐藏怪物顶戴花翎
  if (g_ConfigDlg.ConfigCheckeds[ckHideMonsterIcons]) and Grobal2.IsMonster(m_btRace, m_boPlayMoster) then begin
    Exit;
  end;

  g_EffectImageList.Lock;
  try
    for I := Low(TActorIconArray) to High(TActorIconArray) do begin
      m_ActorIconIndexs[I].Texture := nil;
      if (m_ActorIcons[I].nFileIndex >= 0) and (m_ActorIcons[I].nFileIndex < g_EffectImageList.Count)
        and (m_ActorIcons[I].nIconCount > 0) then begin
        m_ActorIconIndexs[I].dwLoadTick := TimeGetTime;

        {
        if not (
            g_ConfigDlg.ConfigCheckeds[ckHideTitle] and
            g_ClientConfig.boHideIconWithHideTitle and
            (
              ((m_btRace = RC_PLAYOBJECT) and (not m_boPlayMoster)) or
              (m_btRace = RC_HEROOBJECT)
            )
          ) then
        }
        begin
          GameImages := TGameImages(g_EffectImageList.Objects[m_ActorIcons[I].nFileIndex]);
          if GameImages <> nil then begin
            case m_ColorEffect of
              ceGrayScale:m_ActorIconIndexs[I].Texture := GameImages.GetCachedGrayImage(m_ActorIconIndexs[I].nCurrentFrame, m_ActorIconIndexs[I].nX, m_ActorIconIndexs[I].nY);
              ceBright:m_ActorIconIndexs[I].Texture := GameImages.GetCachedBrightImage(m_ActorIconIndexs[I].nCurrentFrame, m_ActorIconIndexs[I].nX, m_ActorIconIndexs[I].nY);
              else
                m_ActorIconIndexs[I].Texture := GameImages.GetCachedImage(m_ActorIconIndexs[I].nCurrentFrame, m_ActorIconIndexs[I].nX, m_ActorIconIndexs[I].nY);
            end;

            if (m_ActorIconIndexs[I].DefTextureWidth = 0) and (m_ActorIconIndexs[I].Texture <> nil) then begin
              m_ActorIconIndexs[I].DefTextureWidth := TTexture(m_ActorIconIndexs[I].Texture).Width;
            end;
          end;
        end;
      end;
    end;
  finally
    g_EffectImageList.UnLock;
  end;
end;

function TActor.CheckLoadPlayEffect:Boolean;
var
  I:Integer;
  ActorEffect:pTClientActorEffect;
begin
  Result := False;
  // 脚本命令播放特效
  m_ActorEffects.Lock;
  try
    for I := m_ActorEffects.Count - 1 downto 0 do begin
      ActorEffect := m_ActorEffects.Items[I];
      if ActorEffect.nLoopCount <> 0 then begin
        // if ActorEffect.nOldCurrentFrame >= 0 then begin
        if (TimeGetTime - ActorEffect.dwEffectTick > ActorEffect.wEffectFrameTime) then begin
          ActorEffect.dwEffectTick := TimeGetTime;
          Inc(ActorEffect.nCurrentFrame);
        end;
        // end;
        if ActorEffect.nCurrentFrame > ActorEffect.nEffectImageOffSet + ActorEffect.wEffectImageCount - 1 then begin
          ActorEffect.nCurrentFrame := ActorEffect.nEffectImageOffSet;
          ActorEffect.nOldCurrentFrame := -1;
          ActorEffect.dwEffectTick := TimeGetTime;

          if ActorEffect.nLoopCount > 0 then
            Dec(ActorEffect.nLoopCount);
        end;

        if ActorEffect.nOldCurrentFrame <> ActorEffect.nCurrentFrame then begin
          ActorEffect.nOldCurrentFrame := ActorEffect.nCurrentFrame;
          Result := True;
        end;
      end
      else begin
        m_ActorEffects.Delete(I);
        Dispose(ActorEffect);
        Result := True;
      end;
    end;
  finally
    m_ActorEffects.UnLock;
  end;
end;

procedure TActor.LoadPlayEffectSurface;
var
  I:Integer;
  GameImages:TGameImages;
  ActorEffect:pTClientActorEffect;
begin
  // 脚本命令播放特效
  m_ActorEffects.Lock;
  try
    g_EffectImageList.Lock;
    try
      if m_ActorEffects.Count > 0 then begin
        // DebugOutStr('m_ActorEffects.Count:' + IntToStr(m_ActorEffects.Count));
        for I := 0 to m_ActorEffects.Count - 1 do begin
          ActorEffect := m_ActorEffects.Items[I];
          ActorEffect.Texture := nil;
          if (ActorEffect.nEffectFileIndex >= 0) and (ActorEffect.nLoopCount <> 0) and (ActorEffect.nEffectFileIndex < g_EffectImageList.Count) then begin
            GameImages := TGameImages(g_EffectImageList.Objects[ActorEffect.nEffectFileIndex]);
            if GameImages <> nil then begin
              // DebugOutStr('FileName:' + GameImages.FileName);
              case m_ColorEffect of
                ceGrayScale:ActorEffect.Texture := GameImages.GetCachedGrayImage(ActorEffect.nCurrentFrame, ActorEffect.nX, ActorEffect.nY);
                ceBright:ActorEffect.Texture := GameImages.GetCachedBrightImage(ActorEffect.nCurrentFrame, ActorEffect.nX, ActorEffect.nY);
                else
                  ActorEffect.Texture := GameImages.GetCachedImage(ActorEffect.nCurrentFrame, ActorEffect.nX, ActorEffect.nY);
              end;
            end;
          end;
        end;
      end;
    finally
      g_EffectImageList.UnLock;
    end;
  finally
    m_ActorEffects.UnLock;
  end;
end;

function TActor.CheckLoadSurface:Boolean;
begin
  Result := False;
  if (not GameCanvas.Active) or (not GameCanvas.Initialized) then Exit;
  m_ColorEffect := GetDrawEffectValue;
  if (MyGetTickCount - m_dwLoadSurfaceTime >= 2 * 1000) or m_boLoadSurface or
    ((m_OldColorEffect <> m_ColorEffect) and ((m_ColorEffect in [ceGrayScale, ceGrayScale2, ceBright]) or (m_OldColorEffect in [ceGrayScale, ceGrayScale2, ceBright]))) then begin
    Result := True;
  end;
  m_OldColorEffect := m_ColorEffect;
end;

function TActor.CheckLoadFengHaoSurface:Boolean;
begin
  Result := False;
  if (not GameCanvas.Active) or (not GameCanvas.Initialized) then Exit;

  if (m_nActiveFengHaoID = 0) then begin
    SetLength(m_FengHaoImageInfo, 0);
    m_FengHaoEffectSurface := nil;
    m_nOldFengHaoSurfaceID := 0;
    m_dwLoadFengHaoSurfaceTime := 0;
    Exit;
  end;

  Result := (not g_ConfigDlg.ConfigCheckeds[ckHideTitle]) and (
    (m_nOldFengHaoSurfaceID <> m_nActiveFengHaoID) or (MyGetTickCount - m_dwLoadFengHaoSurfaceTime > 1000));

  if g_ConfigDlg.ConfigCheckeds[ckHideTitle] then begin
    SetLength(m_FengHaoImageInfo, 0);
    m_FengHaoEffectSurface := nil;
    m_nOldFengHaoSurfaceID := 0;
    m_dwLoadFengHaoSurfaceTime := 0;
  end;
end;

procedure TActor.Run;

  function MagicTimeOut:Boolean;
  begin
    if Self = g_MySelf then begin
      Result := TimeGetTime - m_dwWaitMagicRequest > 3000;
    end
    else
      Result := TimeGetTime - m_dwWaitMagicRequest > 2000;

    if Result then
      m_CurMagic.ServerMagicCode := 0;
  end;
var
  prv, nCurEffFrame:Integer;

  m_dwFrameTimetime:longword;
  bofly, boLoadSurface:Boolean;
  nErrorCode:Integer;

  NpcConfig:PClientCustomNpcConfig;
  NpcDirAction:PNpcDirAction;
begin
  nErrorCode := 0;
  try
    if (m_nCurrentAction = SM_WALK) or
      (m_nCurrentAction = SM_BACKSTEP) or
      (m_nCurrentAction = SM_RUN) or
      (m_nCurrentAction = SM_HORSERUN) or
      (m_nCurrentAction = SM_MAGICMOVE) or // 十步一杀
    (m_nCurrentAction = SM_RUSH) or
      (m_nCurrentAction = SM_RUSHKUNG) or
      (m_nCurrentAction = SM_100HIT) or
      ((m_nCurrentAction >= SM_CUSTOM_PUSH001) and (m_nCurrentAction < SM_CUSTOM_PUSH001 + CUSTOM_MAGIC_COUNT)) or
      ((m_nCurrentAction >= SM_CUSTOM_MAGICMOVE001) and (m_nCurrentAction < SM_CUSTOM_MAGICMOVE001 + CUSTOM_MAGIC_COUNT))
      then begin
      Exit;
    end;

    nErrorCode := 1;
    boLoadSurface := CheckLoadSurface;

    m_boMsgMuch := (Self <> g_MySelf) and (m_MsgList.Count >= 2);

    nErrorCode := 2;
    RunActSound(m_nCurrentFrame - m_nStartFrame);

    nErrorCode := 3;
    RunFrameAction(m_nCurrentFrame - m_nStartFrame);

    nErrorCode := 4;
    nCurEffFrame := m_nCurEffFrame;
    prv := m_nCurrentFrame;
    if m_nCurrentAction <> 0 then begin
      nErrorCode := 5;
      if (m_nCurrentFrame < m_nStartFrame) or (m_nCurrentFrame > m_nEndFrame) then
        m_nCurrentFrame := m_nStartFrame;

      if (Self <> g_MySelf) and (m_boUseMagic) and (not (m_wAppearance in [54..58, 94..98])) then begin
        m_dwFrameTimetime := Round(m_dwFrameTime / 1.8);
      end
      else begin
        // 修正传送门NPC播放时快时慢 chongchong 2014-04-06  + and (not (m_wAppearance in [54..58, 94..98])
        if m_boMsgMuch and (not ((m_btRace = 50) and (m_wAppearance in [54..58, 94..98]))) then
          m_dwFrameTimetime := Round(m_dwFrameTime * 2 / 3)
        else
          m_dwFrameTimetime := m_dwFrameTime;
      end;

      nErrorCode := 6;

      if TimeGetTime - m_dwStartTime > m_dwFrameTimetime then begin
        if m_nCurrentFrame < m_nEndFrame then begin
          if m_boUseMagic then begin
            if (m_nCurEffFrame = m_nSpellFrame - 2) or (MagicTimeOut) then begin
              // 修正自定义技能举手卡 or CheckIsCustomMagic(m_CurMagic.MagicSerial)  chongchong 2016-08-31
              if (m_CurMagic.ServerMagicCode >= 0) or CheckIsCustomMagic(m_CurMagic.MagicSerial) or (MagicTimeOut) then begin
                Inc(m_nCurrentFrame);
                Inc(m_nCurEffFrame);
                m_dwStartTime := TimeGetTime;
                m_StartCounter := timeGetTime;
              end;
            end
            else begin
              if m_nCurrentFrame < m_nEndFrame - 1 then Inc(m_nCurrentFrame);
              Inc(m_nCurEffFrame);
              m_dwStartTime := TimeGetTime;
              m_StartCounter := timeGetTime;
            end;
          end
          else begin
            Inc(m_nCurrentFrame);
            m_dwStartTime := TimeGetTime;
            m_StartCounter := timeGetTime;
          end;
        end
        else begin
          if m_boDelActionAfterFinished then begin
            m_dwDeleteTime := TimeGetTime;
            m_boDelActor := True;
            m_boFreeActor := True;
          end;

          if Self = g_MySelf then begin
            nErrorCode := 7;
            if frmMain.ServerAcceptNextAction(IsAttackAction(m_nCurrentAction)) then begin
              nErrorCode := 8;
              ActionEnded;
              m_nCurrentAction := 0;
              m_boUseMagic := False;
            end;
          end
          else begin
            nErrorCode := 9;
            ActionEnded;
            m_nCurrentAction := 0;
            m_boUseMagic := False;
          end;
        end;

        nErrorCode := 10;
        if m_boUseMagic then begin
          // 付过阑 静绰 版快
          if m_nCurEffFrame = m_nSpellFrame - 1 then begin // 付过 惯荤 矫痢
            // 付过 惯荤
            if (m_CurMagic.ServerMagicCode > 0) and (m_CurMagic.EffectNumber > 0) and (not m_boCreateEffect) then begin
              nErrorCode := 11;
              m_boCreateEffect := True;
              with m_CurMagic do
                PlayScene.NewMagic(Self,
                  ServerMagicCode,
                  EffectNumber, // Effect
                  m_nCurrX,
                  m_nCurrY,
                  targx,
                  targy,
                  target,
                  EffectType, // EffectType
                  Recusion,
                  anitime,
                  bofly, m_CurMagic.NewLevel, m_CurMagic.MagicItemType);

              nErrorCode := 12;

              if bofly then
                g_PlaySound.PlaySound(m_nMagicFireSound)
              else
                g_PlaySound.PlaySound(m_nMagicExplosionSound);
            end;
            // LatestSpellTime := TimeGetTime;
            m_CurMagic.ServerMagicCode := 0;
          end
          else
            m_boCreateEffect := False;
        end;
      end;
      if m_wAppearance in [0, 1, 43] then
        m_nCurrentDefFrame := -10
      else
        m_nCurrentDefFrame := 0;
      m_dwDefFrameTime := TimeGetTime;
    end
    else begin

      // begin 修正传送门NPC播放时快时慢 chongchong 2014-04-06
      if ((m_btRace = 50) and (m_wAppearance in [54..58, 94..98])) then begin
        if TimeGetTime - m_dwDefFrameTime > MA54.ActStand.ftime then begin
          m_dwDefFrameTime := TimeGetTime;
          Inc(m_nCurrentDefFrame);
          if m_nCurrentDefFrame >= m_nDefFrameCount then
            m_nCurrentDefFrame := 0;
        end;
        if DefaultMotion then boLoadSurface := True;
      end
        // end 修正传送门NPC播放时快时慢 chongchong 2014-04-06 -------------

        // begin 修正自定义怪站立时播放速度不对 chongchong 2014-09-04 -----------
      else if (Self is TCustomActor) and (m_nChangeAppr < 0) then begin
        if TimeGetTime - m_dwDefFrameTime > TCustomActor(Self).Config.Actions[matStand].PlayTime then begin
          m_dwDefFrameTime := TimeGetTime;
          Inc(m_nCurrentDefFrame);
          if m_nCurrentDefFrame >= m_nDefFrameCount then
            m_nCurrentDefFrame := 0;
        end;
        if DefaultMotion then boLoadSurface := True;
      end
        // end 修正自定义怪站立时播放速度不对 chongchong 2014-09-04 -----------

      else if (Self is TNpcActor) and (m_wAppearance >= 10000) then begin
        NpcConfig := GetCustomNpcConfig(m_wAppearance);
        NpcDirAction := nil;
        if NpcConfig <> nil then begin
          if NpcConfig.wDirCount > 1 then
            m_btDir := m_btDir mod NpcConfig.wDirCount
          else
            m_btDir := 0;
          NpcDirAction := @NpcConfig.Actions[m_btDir];
        end;

        if m_nCurrentAction in [SM_HIT, SM_WALK] then begin
          if (NpcDirAction <> nil) and (NpcDirAction.Act_Time > 0) then begin
            if TimeGetTime - m_dwDefFrameTime > NpcDirAction.Act_Time then begin
              m_dwDefFrameTime := TimeGetTime;
              Inc(m_nCurrentDefFrame);
              if m_nCurrentDefFrame >= m_nDefFrameCount then
                m_nCurrentDefFrame := 0;
            end;
            if DefaultMotion then boLoadSurface := True;
          end
          else begin
            if TimeGetTime - m_dwDefFrameTime > 500 then begin
              m_dwDefFrameTime := TimeGetTime;
              Inc(m_nCurrentDefFrame);
              if m_nCurrentDefFrame >= m_nDefFrameCount then
                m_nCurrentDefFrame := 0;
            end;
            if DefaultMotion then boLoadSurface := True;
          end;
        end else begin
          if (NpcDirAction <> nil) and (NpcDirAction.Std_Time > 0) then begin
            if TimeGetTime - m_dwDefFrameTime > NpcDirAction.Std_Time then begin
              m_dwDefFrameTime := TimeGetTime;
              Inc(m_nCurrentDefFrame);
              if m_nCurrentDefFrame >= m_nDefFrameCount then
                m_nCurrentDefFrame := 0;
            end;
            if DefaultMotion then boLoadSurface := True;
          end else begin
            if TimeGetTime - m_dwDefFrameTime > 500 then begin
              m_dwDefFrameTime := TimeGetTime;
              Inc(m_nCurrentDefFrame);
              if m_nCurrentDefFrame >= m_nDefFrameCount then
                m_nCurrentDefFrame := 0;
            end;
            if DefaultMotion then boLoadSurface := True;
          end;
        end;
      end else if TimeGetTime - m_dwSmoothMoveTime > 200 then begin
        if TimeGetTime - m_dwDefFrameTime > 500 then begin
          m_dwDefFrameTime := TimeGetTime;
          Inc(m_nCurrentDefFrame);
          if m_nCurrentDefFrame >= m_nDefFrameCount then
            m_nCurrentDefFrame := 0;
        end;
        if DefaultMotion then boLoadSurface := True;
      end;
    end;

    if (prv <> m_nCurrentFrame) or (nCurEffFrame <> m_nCurEffFrame) then
      boLoadSurface := True;

    nErrorCode := 13;
    if boLoadSurface then begin
      m_dwLoadSurfaceTime := TimeGetTime;
      PlayScene.LoadSurface(LoadSurface);
    end;

    nErrorCode := 14;
    if CheckLoadUserName then LoadNameSurface;

    nErrorCode := 15;
    if CheckLoadNumberLable then LoadNumberLableSurface;

    nErrorCode := 16;
    if CheckLoadSay then LoadSaySurface;

    nErrorCode := 17;
    //if CheckLoadActorIcon then LoadActorIcons;

    nErrorCode := 18;
    //if CheckLoadHealthNumber then LoadHealthNumber;
    if CheckLoadFengHaoSurface then LoadFengHaoSurface; // 修复一直跑称号显示错误 chongchong 2014-10-20

    //if CheckLoadPlayEffect then LoadPlayEffectSurface;
  except
    on E:Exception do begin
      DebugOutStr('TActor.Run:' + IntToStr(nErrorCode));
      DebugOutStr(E.Message);
    end;
  end;
end;

function TActor.DoMove(step:Integer):Boolean;
var
  prv, curstep, maxstep:Integer;
  fastmove, normmove:Boolean;
  boLoadSurface:Boolean;
  NormalEffect:TNormalDrawEffect;
  CustomMagicConfig:PClientCustomMagicConfig;
begin
  Result := False;
  fastmove := False;
  normmove := False;

  boLoadSurface := CheckLoadSurface;

  if (m_nCurrentAction = SM_BACKSTEP) then // or (CurrentAction = SM_RUSH) or (CurrentAction = SM_RUSHKUNG) then
    fastmove := True;
  if (m_nCurrentAction = SM_RUSH) or (m_nCurrentAction = SM_RUSHKUNG) or (m_nCurrentAction = SM_MAGICMOVE) {十步一杀} or
  ((m_nCurrentAction >= SM_CUSTOM_MAGICMOVE001) and (m_nCurrentAction < SM_CUSTOM_MAGICMOVE001 + CUSTOM_MAGIC_COUNT)) then
    normmove := True;
  if (Self = g_MySelf) and (not fastmove) and (not normmove) then begin
    g_boMoveSlow := False;
    g_boAttackSlow := False;
    g_nMoveSlowLevel := 0;
    if m_Abil.Weight > m_Abil.MaxWeight then begin
      if not (PlugInEnabled and g_ClientConfig.boSpeedSlow and g_ConfigDlg.ConfigCheckeds[ckSpeedSlow]) then begin
        g_nMoveSlowLevel := m_Abil.Weight div m_Abil.MaxWeight;
        g_boMoveSlow := True;
      end;
    end;
    if m_Abil.WearWeight > m_Abil.MaxWearWeight then begin
      if not (PlugInEnabled and g_ClientConfig.boSpeedSlow and g_ConfigDlg.ConfigCheckeds[ckSpeedSlow]) then begin
        g_nMoveSlowLevel := g_nMoveSlowLevel + m_Abil.WearWeight div m_Abil.MaxWearWeight;
        g_boMoveSlow := True;
      end;
    end;
    if m_Abil.HandWeight > m_Abil.MaxHandWeight then begin
      if not (PlugInEnabled and g_ClientConfig.boSpeedSlow and g_ConfigDlg.ConfigCheckeds[ckSpeedSlow]) then
        g_boAttackSlow := True;
    end;
    if g_boMoveSlow and (m_nSkipTick < g_nMoveSlowLevel) then begin
      Inc(m_nSkipTick);

      if (m_btDir in [0..7]) then begin
        if boLoadSurface then begin
          m_dwLoadSurfaceTime := MyGetTickCount;
          PlayScene.LoadSurface(LoadSurface);
          if (Self = g_MySelf) then begin
            ActionChanged;
          end;
        end;

      end;

      if CheckLoadUserName then LoadNameSurface;
      if CheckLoadNumberLable then LoadNumberLableSurface;
      if CheckLoadSay then LoadSaySurface;
      //if CheckLoadActorIcon then LoadActorIcons;
      //if CheckLoadHealthNumber then LoadHealthNumber;
      if CheckLoadFengHaoSurface then LoadFengHaoSurface; // 修复一直跑称号显示错误 chongchong 2014-10-20
      //if CheckLoadPlayEffect then LoadPlayEffectSurface;
      Exit;
    end
    else begin
      m_nSkipTick := 0;
    end;
    if (m_nCurrentAction = SM_WALK) or
      (m_nCurrentAction = SM_BACKSTEP) or
      (m_nCurrentAction = SM_RUN) or
      (m_nCurrentAction = SM_HORSERUN) or
      (m_nCurrentAction = SM_RUSH) or
      (m_nCurrentAction = SM_RUSHKUNG)
      then begin
      case (m_nCurrentFrame - m_nStartFrame) of
        1:g_PlaySound.PlaySound(m_nFootStepSound);
        4:g_PlaySound.PlaySound(m_nFootStepSound + 1);
      end;
    end;
  end;

  Result := False;

  if Self.m_btRace = RC_HEROOBJECT then
    m_boMsgMuch := m_MsgList.Count >= 1
  else
    m_boMsgMuch := (Self <> g_MySelf) and (m_MsgList.Count >= 2);

  {
  // 修正消息处理不及时的判断 chongchong 2016-05-04
  if Self <> g_MySelf then
  begin
    if (m_nCurrentAction in [SM_RUN, SM_WALK]) and (m_MsgList.Count = 1) then
    begin
      m_MsgList.Lock;
      try
        PMsg := m_MsgList.Items[0];
        if PMsg.Ident in [SM_RUN, SM_WALK] then
        begin
          m_boMsgMuch := True;
        end;
      finally
        m_MsgList.UnLock;
      end;
    end
    else
      m_boMsgMuch := (m_MsgList.Count >= 2);
  end
  else
  begin
    m_boMsgMuch := False;
  end;
  }

  prv := m_nCurrentFrame;
  if (m_nCurrentAction = SM_WALK) or
    (m_nCurrentAction = SM_RUN) or
    (m_nCurrentAction = SM_HORSERUN) or
    (m_nCurrentAction = SM_RUSH) or
    (m_nCurrentAction = SM_RUSHKUNG) or
    (m_nCurrentAction = SM_MAGICMOVE) or // 十步一杀
  (m_nCurrentAction = SM_100HIT) or // 追心刺
  ((m_nCurrentAction >= SM_CUSTOM_PUSH001) and (m_nCurrentAction < SM_CUSTOM_PUSH001 + CUSTOM_MAGIC_COUNT)) or
    ((m_nCurrentAction >= SM_CUSTOM_MAGICMOVE001) and (m_nCurrentAction < SM_CUSTOM_MAGICMOVE001 + CUSTOM_MAGIC_COUNT))
    then begin
    RunActSound(m_nCurrentFrame - m_nStartFrame); // 增加
    if (m_nCurrentFrame < m_nStartFrame) or (m_nCurrentFrame > m_nEndFrame) then begin
      m_nCurrentFrame := m_nStartFrame - 1;

      {
      if Self <> g_MySelf then
      begin
        DScreen.AddChatBoardString('时间差' + IntToStr(TimeGetTime - g_MySelfRunStartTick), clRed, clBlack);
      end
      else
      begin
        g_MySelfRunStartTick := TimeGetTime;
      end;
      }

      {$IF DEBUG_RUN_DELAY_OTHER = 1}
      if Self <> g_MySelf then begin
        g_OtherRunStart.Add(IntToStr(TimeGetTime));
        g_OtherRunStart.SaveToFile(g_sOtherRunStartFile);
      end;
      {$IFEND}

    end;
    if m_nCurrentFrame < m_nEndFrame then begin
      Inc(m_nCurrentFrame);
      if m_boMsgMuch and (not normmove) then // //加快步伐
        if m_nCurrentFrame < m_nEndFrame then
          Inc(m_nCurrentFrame);

      curstep := m_nCurrentFrame - m_nStartFrame + 1;
      maxstep := m_nEndFrame - m_nStartFrame + 1;
      Shift(m_btDir, m_nMoveStep, curstep, maxstep);
    end;
    if m_nCurrentFrame >= m_nEndFrame then begin
      if Self = g_MySelf then begin
        if frmMain.ServerAcceptNextAction(IsAttackAction(m_nCurrentAction)) then begin
          ActionEnded;

          {$IF DEBUG_RUN_DELAY_SELF = 1}
          if m_nCurrentAction = SM_RUN then begin
            g_SelfRunEnd.Add(IntToStr(TimeGetTime));
            g_SelfRunEnd.SaveToFile(g_sSelfRunEndFile);
          end;
          {$IFEND}
          // m_nCurrentAction := 0;              增加自定义技能判断 By 一支笔 at:2021-07-07 12:41:12
          if (m_nCurrentAction >= SM_CUSTOM_PUSH001) and (m_nCurrentAction < SM_CUSTOM_PUSH001 + CUSTOM_MAGIC_COUNT) then begin
            CustomMagicConfig := GetCustomMagicConfig(Abs(m_nCurrentAction) - SM_CUSTOM_PUSH001 + CUSTOM_MAGIC_START_ID);
            if (CustomMagicConfig <> nil) and (not CustomMagicConfig.MagicBaseConfig.MagicActionContinue) then begin
              m_nCurrentAction := 0;
            end;
          end
          else
            if (m_nCurrentAction <> SM_100HIT) then // 修正追心刺最后一帧计算错误 chongchong 2014-09-21
              m_nCurrentAction := 0;
          //
          m_boLockEndFrame := True; //
          m_dwSmoothMoveTime := TimeGetTime;
          ActionChanged;
        end;
      end
      else begin
        ActionEnded;

        {$IF DEBUG_RUN_DELAY_OTHER = 1}
        if m_nCurrentAction = SM_RUN then begin
          g_OtherRunEnd.Add(IntToStr(TimeGetTime));
          g_OtherRunEnd.SaveToFile(g_sOtherRunEndFile);
        end;
        {$IFEND}
        //增加自定义技能判断 By 一支笔 at:2021-07-07 12:41:12
        if (m_nCurrentAction >= SM_CUSTOM_PUSH001) and (m_nCurrentAction < SM_CUSTOM_PUSH001 + CUSTOM_MAGIC_COUNT) then begin
          CustomMagicConfig := GetCustomMagicConfig(Abs(m_nCurrentAction) - SM_CUSTOM_PUSH001 + CUSTOM_MAGIC_START_ID);
          if (CustomMagicConfig <> nil) and (not CustomMagicConfig.MagicBaseConfig.MagicActionContinue) then begin
            m_nCurrentAction := 0;
          end;
        end
        else
          // 修正英雄追心刺最后一帧计算错误 chongchong 2017-11-16
          if (m_nCurrentAction <> SM_100HIT) then
            m_nCurrentAction := 0;

        m_boLockEndFrame := True;
        m_dwSmoothMoveTime := TimeGetTime;
        ActionChanged;
      end;
    end;

    if (m_nCurrentAction = SM_RUSH) or (m_nCurrentAction = SM_100HIT) or
      ((m_nCurrentAction >= SM_CUSTOM_PUSH001) and (m_nCurrentAction < SM_CUSTOM_PUSH001 + CUSTOM_MAGIC_COUNT)) then begin
      if Self = g_MySelf then begin
        g_dwDizzyDelayStart := MyGetTickCount;
        g_dwDizzyDelayTime := 150; // 300;
      end;
    end;
    if m_nCurrentAction = SM_RUSHKUNG then begin
      if m_nCurrentFrame >= m_nEndFrame - 3 then begin
        m_nCurrX := m_nActBeforeX;
        m_nCurrY := m_nActBeforeY;
        m_nRx := m_nCurrX;
        m_nRy := m_nCurrY;
        ActionEnded;
        m_nCurrentAction := 0;
        m_boLockEndFrame := True;
        ActionChanged;
      end;
    end;

    // 十步一杀 -- 爆炸特效
    if m_nCurrentAction = SM_MAGICMOVE then begin
      if m_nCurrentFrame = m_nEndFrame - 3 then begin
        // modify chongchong 2013-07-18
        // PlayScene.m_EffectList.Add(TNormalDrawEffect.Create(m_nCurrX, m_nCurrY, g_WMagic10Images, 220, 10, HA.ActHit.ftime, True));
        {$IF IsMultiThreadRender = 1}
        PlayScene.m_EffectList.Lock;
        try
          {$IFEND}
          NormalEffect := TNormalDrawEffect.Create(m_nCurrX, m_nCurrY, g_WMagic10Images, 220, 10, HA.ActHit.ftime, True);
          // 十步一杀照亮黑夜范围扩大 chongchong 2015-03-04
          NormalEffect.light := 3;
          PlayScene.m_EffectList.Add(NormalEffect);
          {$IF IsMultiThreadRender = 1}
        finally
          PlayScene.m_EffectList.UnLock;
        end;
        {$IFEND}
      end;
    end;
    Result := True;
  end

    {$IF DEBUG_HIT_DELAY = 1}
    // 修正攻击间隔不均匀 chongchong 2016-12-09
  else if (m_nCurrentAction = SM_HIT) or
    (m_nCurrentAction = SM_HEAVYHIT) or
    (m_nCurrentAction = SM_BIGHIT) or
    (m_nCurrentAction = SM_POWERHIT) or
    (m_nCurrentAction = SM_LONGHIT) or
    (m_nCurrentAction = SM_WIDEHIT) or
    (m_nCurrentAction = SM_FIREHIT) or
    (m_nCurrentAction = SM_CRSHIT) or
    (m_nCurrentAction = SM_TWNHIT) or
    (m_nCurrentAction = SM_SWORDHIT) or
    (m_nCurrentAction = SM_43HIT) or
    (m_nCurrentAction = SM_66HIT) or
    (m_nCurrentAction = SM_66HIT1) or
    (m_nCurrentAction = SM_101HIT) or
    (m_nCurrentAction = SM_102HIT) or
    (m_nCurrentAction = SM_103HIT) or
    (m_nCurrentAction = SM_113HIT) or
    (m_nCurrentAction = SM_115HIT) or
    ((m_nCurrentAction >= SM_CUSTOM_HIT1) and (m_nCurrentAction <= SM_CUSTOM_HIT100)) then begin
    if m_nCurrentFrame >= m_nEndFrame then begin
      if Self = g_MySelf then begin
        g_SelfHitEnd.Add(IntToStr(TimeGetTime));
        g_SelfHitEnd.SaveToFile(g_sSelfHitEndFile);
      end;
    end;
  end;
  {$ELSE}
  ;
  {$IFEND}

  if (m_nCurrentAction = SM_BACKSTEP) then begin
    if (m_nCurrentFrame > m_nEndFrame) or (m_nCurrentFrame < m_nStartFrame) then begin
      m_nCurrentFrame := m_nEndFrame + 1;
    end;
    if m_nCurrentFrame > m_nStartFrame then begin
      Dec(m_nCurrentFrame);
      if m_boMsgMuch or fastmove then // 加快步伐
        if m_nCurrentFrame > m_nStartFrame then Dec(m_nCurrentFrame);

      curstep := m_nEndFrame - m_nCurrentFrame + 1;
      maxstep := m_nEndFrame - m_nStartFrame + 1;
      Shift(GetBack(m_btDir), m_nMoveStep, curstep, maxstep);
    end;
    if m_nCurrentFrame <= m_nStartFrame then begin
      if Self = g_MySelf then begin
        // if FrmMain.ServerAcceptNextAction then begin
        ActionEnded;
        m_nCurrentAction := 0;
        m_boLockEndFrame := True;
        m_dwSmoothMoveTime := TimeGetTime;

        g_dwDizzyDelayStart := MyGetTickCount;
        g_dwDizzyDelayTime := 1000; // 1檬 掉饭捞
        // end;
      end
      else begin
        ActionEnded;
        m_nCurrentAction := 0;
        m_boLockEndFrame := True;
        m_dwSmoothMoveTime := TimeGetTime;
      end;
      ActionChanged;
    end;
    Result := True;
  end;

  boLoadSurface := CheckLoadSurface;
  if (prv <> m_nCurrentFrame) then
    boLoadSurface := True;

  if (m_btDir in [0..7]) then begin
    if boLoadSurface then begin
      m_dwLoadSurfaceTime := MyGetTickCount;
      PlayScene.LoadSurface(LoadSurface);

      if CheckLoadUserName then LoadNameSurface;
      if CheckLoadNumberLable then LoadNumberLableSurface;
      if CheckLoadSay then LoadSaySurface;
      //if CheckLoadActorIcon then LoadActorIcons;
      //if CheckLoadHealthNumber then LoadHealthNumber;
      if CheckLoadFengHaoSurface then LoadFengHaoSurface; // 修复一直跑称号显示错误 chongchong 2014-10-20
      //if CheckLoadPlayEffect then LoadPlayEffectSurface;
    end;
  end;
end;

(*
function TActor.DoSmoothMove(step: Integer): Boolean;
var
  prv, curstep, maxstep: Integer;
  fastmove, normmove: Boolean;
  boLoadSurface: Boolean;
  NormalEffect: TNormalDrawEffect;
  CustomMagicConfig: PClientCustomMagicConfig;
  currTime, movetick: LongWord;
begin
  Result := False;
  fastmove := False;
  normmove := False;

  boLoadSurface := CheckLoadSurface;

  if (m_nCurrentAction = SM_BACKSTEP) or ((m_nCurrentAction = SM_RUSH)) then                                                          // or (CurrentAction = SM_RUSH) or (CurrentAction = SM_RUSHKUNG) then
    fastmove := True;
  if (m_nCurrentAction = SM_RUSHKUNG) or (m_nCurrentAction = SM_MAGICMOVE) {十步一杀} or
    ((m_nCurrentAction >= SM_CUSTOM_MAGICMOVE001) and (m_nCurrentAction < SM_CUSTOM_MAGICMOVE001 + CUSTOM_MAGIC_COUNT)) then
    normmove := True;
  if (Self = g_MySelf) and (not fastmove) and (not normmove) then
  begin
    g_boMoveSlow := False;
    g_boAttackSlow := False;
    g_nMoveSlowLevel := 0;
    if m_Abil.Weight > m_Abil.MaxWeight then
    begin
      if not (PlugInEnabled and g_ClientConfig.boSpeedSlow and g_ConfigDlg.ConfigCheckeds[ckSpeedSlow]) then
      begin
        g_nMoveSlowLevel := m_Abil.Weight div m_Abil.MaxWeight;
        g_boMoveSlow := True;
      end;
    end;
    if m_Abil.WearWeight > m_Abil.MaxWearWeight then
    begin
      if not (PlugInEnabled and g_ClientConfig.boSpeedSlow and g_ConfigDlg.ConfigCheckeds[ckSpeedSlow]) then
      begin
        g_nMoveSlowLevel := g_nMoveSlowLevel + m_Abil.WearWeight div m_Abil.MaxWearWeight;
        g_boMoveSlow := True;
      end;
    end;
    if m_Abil.HandWeight > m_Abil.MaxHandWeight then
    begin
      if not (PlugInEnabled and g_ClientConfig.boSpeedSlow and g_ConfigDlg.ConfigCheckeds[ckSpeedSlow]) then
        g_boAttackSlow := True;
    end;
    if g_boMoveSlow and (m_nSkipTick < g_nMoveSlowLevel) then
    begin
      Inc(m_nSkipTick);

      if (m_btDir in [0..7]) then
      begin
        if boLoadSurface then
        begin
          m_dwLoadSurfaceTime := MyGetTickCount;
          PlayScene.LoadSurface(LoadSurface);
          if (Self = g_MySelf) then
          begin
            ActionChanged;
          end;
        end;

      end;

      if CheckLoadUserName then LoadNameSurface;
      if CheckLoadNumberLable then LoadNumberLableSurface;
      if CheckLoadSay then LoadSaySurface;
      //if CheckLoadActorIcon then LoadActorIcons;
      //if CheckLoadHealthNumber then LoadHealthNumber;
      if CheckLoadFengHaoSurface then LoadFengHaoSurface;    // 修复一直跑称号显示错误 chongchong 2014-10-20
      //if CheckLoadPlayEffect then LoadPlayEffectSurface;
      Exit;
    end
    else
    begin
      m_nSkipTick := 0;
    end;
    if (m_nCurrentAction = SM_WALK) or
      (m_nCurrentAction = SM_BACKSTEP) or
      (m_nCurrentAction = SM_RUN) or
      (m_nCurrentAction = SM_HORSERUN) or
      (m_nCurrentAction = SM_RUSH) or
      (m_nCurrentAction = SM_RUSHKUNG)
      then
    begin
      case (m_nCurrentFrame - m_nStartFrame) of
        1: g_PlaySound.PlaySound(m_nFootStepSound);
        4: g_PlaySound.PlaySound(m_nFootStepSound + 1);
      end;
    end;
  end;

  Result := False;

  if Self.m_btRace = RC_HEROOBJECT then
    m_boMsgMuch := m_MsgList.Count >= 1
  else
    m_boMsgMuch := (Self <> g_MySelf) and (m_MsgList.Count >= 2);

  prv := m_nCurrentFrame;
  if (m_nCurrentAction = SM_WALK) or
    (m_nCurrentAction = SM_RUN) or
    (m_nCurrentAction = SM_HORSERUN) or
    (m_nCurrentAction = SM_RUSH) or
    (m_nCurrentAction = SM_RUSHKUNG) or
    (m_nCurrentAction = SM_MAGICMOVE) or                                                            // 十步一杀
    (m_nCurrentAction = SM_100HIT) or                                                                  // 追心刺
    ((m_nCurrentAction >= SM_CUSTOM_PUSH001) and (m_nCurrentAction < SM_CUSTOM_PUSH001 + CUSTOM_MAGIC_COUNT)) or
    ((m_nCurrentAction >= SM_CUSTOM_MAGICMOVE001) and (m_nCurrentAction < SM_CUSTOM_MAGICMOVE001 + CUSTOM_MAGIC_COUNT))
    then
  begin
    RunActSound(m_nCurrentFrame - m_nStartFrame);                                                   // 增加
    if (m_nCurrentFrame < m_nStartFrame) or (m_nCurrentFrame > m_nEndFrame) then
    begin
      m_nCurrentFrame := m_nStartFrame - 1;

{$IF DEBUG_RUN_DELAY_OTHER = 1}
      if Self <> g_MySelf then
      begin
        g_OtherRunStart.Add(IntToStr(TimeGetTime));
        g_OtherRunStart.SaveToFile(g_sOtherRunStartFile);
      end;
{$IFEND}

    end;
//    if m_nCurrentFrame < m_nEndFrame then
//    begin
      {平滑移动 By 一支笔 at:2022-03-25 16:37:37}
      currTime := TimeGetTime;
//    end;
    if currTime - m_dwCurrentActionTick < g_dwRunIntervalTime then
    begin
      movetick := currTime - m_dwCurrentActionTick;
      m_nShiftX := Round(movetick * m_fVecX);
      m_nShiftY := Round(movetick * m_fVecY);
      m_nCurrentFrame := m_nStartFrame + _MIN(movetick div step, m_nEndFrame - m_nStartFrame);
    end
    else
//    if currTime - m_dwCurrentActionTick >= g_dwRunIntervalTime{m_nCurrentFrame >= m_nEndFrame} then
    begin
      if Self = g_MySelf then
      begin
        if frmMain.ServerAcceptNextAction(IsAttackAction(m_nCurrentAction)) then
        begin
          ActionEnded;

{$IF DEBUG_RUN_DELAY_SELF = 1}
          if m_nCurrentAction = SM_RUN then
          begin
            g_SelfRunEnd.Add(IntToStr(TimeGetTime));
            g_SelfRunEnd.SaveToFile(g_sSelfRunEndFile);
          end;
{$IFEND}
          // m_nCurrentAction := 0;              增加自定义技能判断 By 一支笔 at:2021-07-07 12:41:12
          if (m_nCurrentAction >= SM_CUSTOM_PUSH001) and (m_nCurrentAction < SM_CUSTOM_PUSH001 + CUSTOM_MAGIC_COUNT) then
          begin
            CustomMagicConfig := GetCustomMagicConfig(Abs(m_nCurrentAction) - SM_CUSTOM_PUSH001 + CUSTOM_MAGIC_START_ID);
            if (CustomMagicConfig <> nil) and (not CustomMagicConfig.MagicBaseConfig.MagicActionContinue) then
            begin
                m_nCurrentAction := 0;
            end;
          end
          else
          if (m_nCurrentAction <> SM_100HIT) then // 修正追心刺最后一帧计算错误 chongchong 2014-09-21
            m_nCurrentAction := 0;
                                                                             //
          m_boLockEndFrame := True;                                                                 //
          m_dwSmoothMoveTime := TimeGetTime;
          ActionChanged;
        end;
      end
      else
      begin
        ActionEnded;

{$IF DEBUG_RUN_DELAY_OTHER = 1}
        if m_nCurrentAction = SM_RUN then
        begin
          g_OtherRunEnd.Add(IntToStr(TimeGetTime));
          g_OtherRunEnd.SaveToFile(g_sOtherRunEndFile);
        end;
{$IFEND}
        //增加自定义技能判断 By 一支笔 at:2021-07-07 12:41:12
          if (m_nCurrentAction >= SM_CUSTOM_PUSH001) and (m_nCurrentAction < SM_CUSTOM_PUSH001 + CUSTOM_MAGIC_COUNT) then
          begin
            CustomMagicConfig := GetCustomMagicConfig(Abs(m_nCurrentAction) - SM_CUSTOM_PUSH001 + CUSTOM_MAGIC_START_ID);
            if (CustomMagicConfig <> nil) and (not CustomMagicConfig.MagicBaseConfig.MagicActionContinue) then
            begin
                m_nCurrentAction := 0;
            end;
          end
          else
        // 修正英雄追心刺最后一帧计算错误 chongchong 2017-11-16
        if (m_nCurrentAction <> SM_100HIT) then
          m_nCurrentAction := 0;

        m_boLockEndFrame := True;
        m_dwSmoothMoveTime := TimeGetTime;
        ActionChanged;
      end;
    end;

    if (m_nCurrentAction = SM_RUSH) or (m_nCurrentAction = SM_100HIT) or
      ((m_nCurrentAction >= SM_CUSTOM_PUSH001) and (m_nCurrentAction < SM_CUSTOM_PUSH001 + CUSTOM_MAGIC_COUNT)) then
    begin
      if Self = g_MySelf then
      begin
        g_dwDizzyDelayStart := MyGetTickCount;
        g_dwDizzyDelayTime := 150;                                                                  // 300;
      end;
    end;
    if m_nCurrentAction = SM_RUSHKUNG then
    begin
      if m_nCurrentFrame >= m_nEndFrame - 3 then
      begin
        m_nCurrX := m_nActBeforeX;
        m_nCurrY := m_nActBeforeY;
        m_nRx := m_nCurrX;
        m_nRy := m_nCurrY;
        ActionEnded;
        m_nCurrentAction := 0;
        m_boLockEndFrame := True;
        ActionChanged;
      end;
    end;

    // 十步一杀 -- 爆炸特效
    if m_nCurrentAction = SM_MAGICMOVE then
    begin
      if m_nCurrentFrame = m_nEndFrame - 3 then
      begin
        // modify chongchong 2013-07-18
        // PlayScene.m_EffectList.Add(TNormalDrawEffect.Create(m_nCurrX, m_nCurrY, g_WMagic10Images, 220, 10, HA.ActHit.ftime, True));
{$IF IsMultiThreadRender = 1}
        PlayScene.m_EffectList.Lock;
        try
{$IFEND}
          NormalEffect := TNormalDrawEffect.Create(m_nCurrX, m_nCurrY, g_WMagic10Images, 220, 10, HA.ActHit.ftime, True);
          // 十步一杀照亮黑夜范围扩大 chongchong 2015-03-04
          NormalEffect.light := 3;
          PlayScene.m_EffectList.Add(NormalEffect);
{$IF IsMultiThreadRender = 1}
        finally
          PlayScene.m_EffectList.UnLock;
        end;
{$IFEND}
      end;
    end;
    Result := True;
  end

  {$IF DEBUG_HIT_DELAY = 1}
  // 修正攻击间隔不均匀 chongchong 2016-12-09
  else if (m_nCurrentAction = SM_HIT) or
    (m_nCurrentAction = SM_HEAVYHIT) or
    (m_nCurrentAction = SM_BIGHIT) or
    (m_nCurrentAction = SM_POWERHIT) or
    (m_nCurrentAction = SM_LONGHIT) or
    (m_nCurrentAction = SM_WIDEHIT) or
    (m_nCurrentAction = SM_FIREHIT) or
    (m_nCurrentAction = SM_CRSHIT) or
    (m_nCurrentAction = SM_TWNHIT) or
    (m_nCurrentAction = SM_SWORDHIT) or
    (m_nCurrentAction = SM_43HIT) or
    (m_nCurrentAction = SM_66HIT) or
    (m_nCurrentAction = SM_66HIT1) or
    (m_nCurrentAction = SM_101HIT) or
    (m_nCurrentAction = SM_102HIT) or
    (m_nCurrentAction = SM_103HIT) or
    (m_nCurrentAction = SM_113HIT) or
    (m_nCurrentAction = SM_115HIT) or
    ((m_nCurrentAction >= SM_CUSTOM_HIT1) and (m_nCurrentAction <= SM_CUSTOM_HIT100)) then
  begin
    if m_nCurrentFrame >= m_nEndFrame then
    begin
      if Self = g_MySelf then
      begin
        g_SelfHitEnd.Add(IntToStr(TimeGetTime));
        g_SelfHitEnd.SaveToFile(g_sSelfHitEndFile);
      end;
    end;
  end;
{$ELSE}
  ;
{$IFEND}

  if (m_nCurrentAction = SM_BACKSTEP) then
  begin
    if (m_nCurrentFrame > m_nEndFrame) or (m_nCurrentFrame < m_nStartFrame) then
    begin
      m_nCurrentFrame := m_nEndFrame + 1;
    end;
    if m_nCurrentFrame > m_nStartFrame then
    begin
      Dec(m_nCurrentFrame);
      if m_boMsgMuch or fastmove then                                                               // 加快步伐
        if m_nCurrentFrame > m_nStartFrame then Dec(m_nCurrentFrame);

      curstep := m_nEndFrame - m_nCurrentFrame + 1;
      maxstep := m_nEndFrame - m_nStartFrame + 1;
      Shift(GetBack(m_btDir), m_nMoveStep, curstep, maxstep);
    end;
    if m_nCurrentFrame <= m_nStartFrame then
    begin
      if Self = g_MySelf then
      begin
            // if FrmMain.ServerAcceptNextAction then begin
        ActionEnded;
        m_nCurrentAction := 0;
        m_boLockEndFrame := True;
        m_dwSmoothMoveTime := TimeGetTime;

        g_dwDizzyDelayStart := MyGetTickCount;
        g_dwDizzyDelayTime := 1000;                                                                 // 1檬 掉饭捞
            // end;
      end
      else
      begin
        ActionEnded;
        m_nCurrentAction := 0;
        m_boLockEndFrame := True;
        m_dwSmoothMoveTime := TimeGetTime;
      end;
      ActionChanged;
    end;
    Result := True;
  end;

  boLoadSurface := CheckLoadSurface;
  if (prv <> m_nCurrentFrame) then
    boLoadSurface := True;

  if (m_btDir in [0..7]) then
  begin
    if boLoadSurface then
    begin
      m_dwLoadSurfaceTime := MyGetTickCount;
      PlayScene.LoadSurface(LoadSurface);

      if CheckLoadUserName then LoadNameSurface;
      if CheckLoadNumberLable then LoadNumberLableSurface;
      if CheckLoadSay then LoadSaySurface;
      //if CheckLoadActorIcon then LoadActorIcons;
      //if CheckLoadHealthNumber then LoadHealthNumber;
      if CheckLoadFengHaoSurface then LoadFengHaoSurface;  // 修复一直跑称号显示错误 chongchong 2014-10-20
      //if CheckLoadPlayEffect then LoadPlayEffectSurface;
    end;
  end;
end;
*)

procedure TActor.MoveFail(nRestoreX:Integer; nRestoreY:Integer; nDir:Integer);
begin
  // ActionEnded;
  m_nCurrentAction := 0;
  m_boLockEndFrame := True;

  // 修复错位问题 chongchong 2014-10-28
  if (nRestoreX > -1) and (nRestoreY > -1) and (nDir > -1) then begin
    g_MySelf.m_nCurrX := nRestoreX;
    g_MySelf.m_nCurrY := nRestoreY;
    g_MySelf.m_btDir := nDir;
  end
  else begin
    case g_ActionCode of
      CM_WALK, CM_RUN, CM_HORSERUN:begin
          g_MySelf.m_nCurrX := m_nOldx;
          g_MySelf.m_nCurrY := m_nOldy;
          g_MySelf.m_btDir := m_nOldDir;
        end;
    end;
  end;

  CleanUserMsgs;

  g_boCanDrawTileMap := True;

  ActionChanged;
end;

function TActor.CanCancelAction:Boolean;
begin
  Result := False;
  if m_nCurrentAction = SM_HIT then
    if not m_boUseEffect then
      Result := True;
end;

procedure TActor.CancelAction;
begin
  m_nCurrentAction := 0; // 悼累 肯丰
  m_boLockEndFrame := True;
end;

procedure TActor.CleanCharMapSetting(X, Y:Integer);
begin
  g_MySelf.m_nCurrX := X;
  g_MySelf.m_nCurrY := Y;
  g_MySelf.m_nRx := X;
  g_MySelf.m_nRy := Y;
  m_nOldx := X;
  m_nOldy := Y;
  m_nCurrentAction := 0;
  m_nCurrentFrame := -1;
  CleanUserMsgs;
end;

{.$DEFINE HEALTH_NUMBER_TEST} //HZQ 20230831 增加增加飘血时间间隔的测试

{$IFDEF HEALTH_NUMBER_TEST}
const
TEST_ARRAY_LEN = 300;
var
arrNumberTick:array[0..TEST_ARRAY_LEN-1] of DWORD;
ngIndex:Integer = 0;
slst:TStringList;
sOutFileName:string;
{$ENDIF}


function TActor.NewHealthNumberFromGroup(NumberType:TNumberType):pTHealthNumber;
var
    i, nCount, nStart, nIndex, nMinOffset:Integer;
    Temp:pTHealthNumber;
    Arr:array[0..10] of pTHealthNumber; //默认固定组数
begin
    if NumberTypeIndexs[NumberType] + m_NumberFramIndex[NumberType] > High(m_HealthNumberArray) then begin
         DebugOut('m_HealthNumberArray Length Error');
         m_NumberFramIndex[NumberType] := 0;
    end;

    nStart := NumberTypeIndexs[NumberType];
    nIndex := m_NumberFramIndex[NumberType];
    Result := @m_HealthNumberArray[nStart + nIndex];
    //HealthNumber.nDrawStyle := nDrawStyle; //HZQ 20230816 暴击和致命打击使用先前的样式 默认保持先前的样式不更改
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    Result.nOffsetX := g_ClientConfig.nHealthNumberOffsetX;
    Result.nOffsetY := g_ClientConfig.nHealthNumberOffsetY;

    nCount := 1;
    arr[0] := Result;

    {
    for I := nIndex + 1 to NumberTypeCounts[NumberType] - 1 do begin
        Temp := @m_HealthNumberArray[nStart + I];
        if Length(Temp.sNumber) > 0 then begin
           arr[nCount] := Temp;
           Inc(nCount);
        end;
    end;

    for i := 0 to nIndex - 1 do begin
        Temp := @m_HealthNumberArray[nStart + I];
        if Length(Temp.sNumber) > 0 then begin
           arr[nCount] := Temp;
           Inc(nCount);
        end;
    end; }

    for i := nIndex - 1 downto 0 do begin
        Temp := @m_HealthNumberArray[nStart + I];
        if Length(Temp.sNumber) > 0 then begin
           arr[nCount] := Temp;
           Inc(nCount);
        end;
    end;

    for i := NumberTypeCounts[NumberType] - 1 downto nIndex + 1 do begin
        Temp := @m_HealthNumberArray[nStart + I];
        if Length(Temp.sNumber) > 0 then begin
           arr[nCount] := Temp;
           Inc(nCount);
        end;
    end; 

    nMinOffset := 40 div nCount;
    if nMinOffset mod 2 <> 0 then begin
        nMinOffset := nMinOffset + 1;
    end;

    //nMinOffset := 4;
    //此处没新增一个飘血，则把进行一定的偏移，防止遮挡
    if nCount >= 2 then begin
        if (Arr[0].nOffsetY - Arr[1].nOffsetY) < nMinOffset then begin //Y逐渐变小
            for I := 1 to nCount-1 do begin
                if (Arr[i].nOffsetX - Arr[I-1].nOffsetX) < nMinOffset then begin
                    Arr[I].nOffsetX := Arr[I].nOffsetX + nMinOffset;
                    Arr[I].nOffsetY := Arr[I].nOffsetY - nMinOffset;
                end;
            end;
        end;
    end;
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    Inc(m_NumberFramIndex[NumberType]);
    if m_NumberFramIndex[NumberType] >= NumberTypeCounts[NumberType] then begin
        m_NumberFramIndex[NumberType] := 0;
    end;
end;

procedure TActor.AddHealthNumber(NumberType:TNumberType; Number:Integer; nResID, nResStartIdx:Integer; nDrawStyle:TNumberDrawStyle);
var
  HealthNumber:pTHealthNumber;
  {$IFDEF HEALTH_NUMBER_TEST}
  i:Integer;
  {$ENDIF}
begin
  if NumberType = tMiss then begin
    EnterCriticalSection(m_HealthNumberLock);
    try
      HealthNumber := @m_HealthNumberArray[Integer(tMiss)];
      HealthNumber.nNmType := NumberType; //HZQ 20230609 记录自己的类型
      HealthNumber.nNumber := 0;
      HealthNumber.sNumber := IntToStr(abs(Number));
      HealthNumber.nOffsetX := 0;
      HealthNumber.nOffsetY := 0;
      HealthNumber.nWidth := 0;
      HealthNumber.nHeight := 0;
      HealthNumber.nResID := nResID; //HZQ 20230609
      HealthNumber.nResStartIdx := nResStartIdx;
      HealthNumber.dwStartHPTick := TimeGetTime;
      HealthNumber.nDrawStyle := nDrawStyle; //HZQ 20230816 默认使用快速退场方式
      HealthNumber.byAlpha := 255;
      SetLength(HealthNumber.ImageIndexs, 0);
    finally
      LeaveCriticalSection(m_HealthNumberLock);
    end;
    Exit;
  end;

  {$IFDEF HEALTH_NUMBER_TEST}
  if ngIndex < TEST_ARRAY_LEN then begin
      arrNumberTick[ngIndex] := MyGetTickCount;
      Inc(ngIndex);
  end else begin
     slst := TStringList.Create;
     for i := 0 to TEST_ARRAY_LEN - 1 do begin
         //slst.Add(Format('[%.2d] = %d', [i, arrNumberTick[i]]));
         slst.Add(Format('%d', [arrNumberTick[i]]));
     end;
     sOutFileName := Format('%s/NumberLog_%s.txt',  [ExtractFileDir(ParamStr(0)), FormatDateTime('yyyymmdd_hhmmss_zzz', now())]);
     slst.SaveToFile(sOutFileName);
     slst.Free;
     ngIndex := 0;
  end;
  {$ENDIF}

  if (abs(Number) <> 0)
    and (PlugInEnabled and g_ClientConfig.boShowMoveLable and g_ConfigDlg.ConfigCheckeds[ckShowMoveLable]) then begin
    EnterCriticalSection(m_HealthNumberLock);
    try
      if NumberType = tTextHP then begin
         HealthNumber := NewHealthNumberFromGroup(tTextHP);
      end else if NumberType in [tBlastHP, tFatalBlow1, tFatalBlow2, tFatalBlow3, tFatalBlow4] then begin
         HealthNumber :=  NewHealthNumberFromGroup(NumberType);
      end else begin
         HealthNumber := @m_HealthNumberArray[Integer(NumberType)];
         //HealthNumber.nDrawStyle := nDrawStyle;
      end;

      HealthNumber.nNumber := Number;

      HealthNumber.nNmType := NumberType;
      HealthNumber.nDrawStyle := nDrawStyle;
      HealthNumber.byAlpha := 255; //初始化Alpha值

      HealthNumber.nResID := nResID; //HZQ 20230609 赋值默认参数
      HealthNumber.nResStartIdx := nResStartIdx;

      HealthNumber.sNumber := IntToStr(abs(Number));
      HealthNumber.nOffsetX := g_ClientConfig.nHealthNumberOffsetX;
      HealthNumber.nOffsetY := g_ClientConfig.nHealthNumberOffsetY;
      HealthNumber.nWidth := 0;
      HealthNumber.nHeight := 0;
      SetLength(HealthNumber.ImageIndexs, 0);

      HealthNumber := @m_HealthNumberArray[Integer(tMiss)];
      HealthNumber.nWidth := 0;
      SetLength(HealthNumber.ImageIndexs, 0);
      HealthNumber.sNumber := '';
    finally
      LeaveCriticalSection(m_HealthNumberLock);
    end;
  end;
end;

procedure TActor.ShowIcons(IsBackActor:Boolean);
var
  I:Integer;
  Texture:TTexture;
  nX, nY:Integer;
  HpBarOffsetX, HpBarOffsetY:Integer;
begin
  //if m_btHorse > 0 then Exit;

  // 摆摊不绘制些信息 add m_boShopStall 2019-07-23 10:14:54
  if m_boShopStall then Exit;

  if (m_boDeath) or (not m_boCanDraw) then Exit;

  if g_ConfigDlg.ConfigCheckeds[ckHideActorIcons] then begin
    if ((m_btRace = RC_PLAYOBJECT) and (not m_boPlayMoster)) or (m_btRace = RC_HEROOBJECT) then begin
      Exit;
    end;
  end;

  //HZQ 20230605 增加隐藏怪物顶戴花翎
  if g_ConfigDlg.ConfigCheckeds[ckHideMonsterIcons] then begin
    if Grobal2.IsMonster(m_btRace, m_boPlayMoster) then begin
      Exit;
    end;
  end;

  if (m_btRace in [RC_PLAYOBJECT, RC_HEROOBJECT]) then begin
    HpBarOffsetX := g_ClientConfig.nHumHPBarOffsetX;
    HpBarOffsetY := g_ClientConfig.nHumHPBarOffsetY;
  end else if (m_btRace = RC_MERCHANT) then begin
    HpBarOffsetX := g_ClientConfig.nNpcHPBarOffsetX;
    HpBarOffsetY := g_ClientConfig.nNpcHPBarOffsetY;
  end else begin
    HpBarOffsetX := g_ClientConfig.nMonHPBarOffsetX;
    HpBarOffsetY := g_ClientConfig.nMonHPBarOffsetY;
  end;

  (* HZQ 20230605 前置判断
  if(not(
          (g_ConfigDlg.ConfigCheckeds[ckHideActorIcons]) and (((m_btRace = RC_PLAYOBJECT) and (not m_boPlayMoster)) or (m_btRace = RC_HEROOBJECT))
        )
    ) then *)
  begin
    for I := Low(TActorIconArray) to High(TActorIconArray) do begin
      if (m_ActorIconIndexs[I].Texture <> nil) and (m_ActorIcons[I].nFileIndex >= 0)
        and (m_ActorIcons[I].nFileIndex < g_EffectImageList.Count) and (m_ActorIcons[I].nIconCount > 0) then begin
        Texture := TTexture(m_ActorIconIndexs[I].Texture);
        if Texture <> nil then begin
          // 修正Seticon显示错误 chongchong 2017-07-29
          nX := m_nSayX - {Texture.Width div 2} m_ActorIconIndexs[I].DefTextureWidth div 2 + m_ActorIcons[I].nX + m_ActorIconIndexs[I].nX;
          nY := m_nSayY - 32 + m_ActorIcons[I].nY + m_ActorIconIndexs[I].nY;

          if ((m_ActorIcons[I].btDrawOrder = 0) and (not IsBackActor)) or
            ((m_ActorIcons[I].btDrawOrder <> 0) and IsBackActor) then begin
            if m_ActorIcons[I].boBlend then
              GameCanvas.DrawBlend(nX + HpBarOffsetX, nY + HpBarOffsetY, Texture)
            else
              GameCanvas.Draw(nX + HpBarOffsetX, nY + HpBarOffsetY, Texture);
          end;
        end;
      end;
    end;
  end;
end;

procedure TActor.ShowHealthNumber;
var
  I, II, nX, nY, nError, nIndex:Integer;
  HealthNumber:pTHealthNumber;
  //NumberType:TNumberType;
  nDrawStyle:TNumberDrawStyle;
  Texture:TTexture;
  //Alpha:Integer; //HZQ 控制飘血图像随着偏移，逐渐减淡
begin
  EnterCriticalSection(m_HealthNumberLock);
  try
    if m_boShowHealthNumber then begin
      if m_boCanDraw and (
        (g_ClientConfig.boHealthNumberText or (not m_boDeath)) or
        (g_ClientConfig.boHealthNumberText and m_boDeath and (tick_diff(m_dwDeathTick, TimeGetTime) <= 10000))
        ) then begin
        for I := 0 to Length(m_HealthNumberArray) - 1 do begin
          HealthNumber := @m_HealthNumberArray[I];
          //NumberType := HealthNumber.nNmType; //HZQ 20230609 程序优化，直接读取数值
          nDrawStyle := HealthNumber.nDrawStyle;

          if HealthNumber.nWidth > 0 then begin
            nX := m_nSayX - HealthNumber.nWidth div 2 + HealthNumber.nOffsetX;

            //if NumberType in [tTextHP, tBlastHP, tFatalBlow1, tFatalBlow2, tFatalBlow3, tFatalBlow4] then begin
            if nDrawStyle = ndsMovingFadeOut then begin
              nY := m_nSayY - 15 - HealthNumber.nHeight + HealthNumber.nOffsetY;

              // 死的时候，飘血高度不变chongchong 2015-10-29
              {
              // 来自TPlayScene.DrawScene
              if Actor.m_boDeath then
                Actor.m_nSayY := m + UNITY + Actor.m_nShiftY + 16 - 60 + (Actor.m_nDownDrawLevel * UNITY)
              else
                Actor.m_nSayY := m + UNITY + Actor.m_nShiftY + 16 - 95 + (Actor.m_nDownDrawLevel * UNITY);
              }
              if m_boDeath then begin
                nY := nY - 35;
              end;
            end else  begin
                nY := m_nSayY - 15 - HealthNumber.nHeight + HealthNumber.nOffsetY;
            end;

            nError := 0; //hzq 20230525 消除编译器警告
            nIndex := 0;
            try
              for II := 0 to Length(HealthNumber.ImageIndexs) - 1 do begin
                nError := 1;
                nIndex := II; //HZQ 20250525 消除W1037 FOR-Loop variable 'II' may be undefined after loop
                if HealthNumber.nResID < 0 then begin //HZQ 20230609 添加自定义飘血
                    Texture := g_WNewopUIImages.Images[HealthNumber.ImageIndexs[II]];
                end else begin
                    Texture := MShare.GetEffectImageListTexture(HealthNumber.nResID, HealthNumber.ImageIndexs[II]);
                end;
                nError := 2;
                if Texture <> nil then begin
                  nError := 3;
                  GameCanvas.DrawAlpha(nX, nY, Texture, HealthNumber.byAlpha);
                  nError := 4;
                  Inc(nX, Texture.Width);
                  nError := 5;
                end;
              end;
            except
              DebugOutStr('ShowHealthNumber ' + inttostr(nError) + ' Length(HealthNumber.ImageIndexs) ' + inttostr(Length(HealthNumber.ImageIndexs)) + ' II ' + inttostr(nIndex));
              if (nIndex < Length(HealthNumber.ImageIndexs)) then
                DebugOutStr('ShowHealthNumber ' + inttostr(nError) + ' Length(HealthNumber.ImageIndexs) ' + inttostr(Length(HealthNumber.ImageIndexs)) + ' HealthNumber.ImageIndexs[II] ' + inttostr(HealthNumber.ImageIndexs[nIndex]));
            end;
          end;
        end;
      end;
    end else begin
      for I := 0 to Length(m_HealthNumberArray) - 1 do begin
        HealthNumber := @m_HealthNumberArray[I];
        HealthNumber.nWidth := 0;
        HealthNumber.nHeight := 0;
        SetLength(HealthNumber.ImageIndexs, 0);
      end;
    end;
  finally
    LeaveCriticalSection(m_HealthNumberLock);
  end;
end;

procedure TActor.DrawSelfEffect(dx, dy:Integer; IsBackActor:Boolean);
var
  D:TTexture;
  aX, aY:Integer;
  wimg:TGameImages;
  idx:Integer;
  IsBlendDraw:Boolean;
  IsDraw:Boolean;
begin
  if not IsBackActor then begin
    if m_CustomMagicStatusEffect.boShow and (m_btHorse = 0) then begin
      if TimeGetTime - m_CustomMagicStatusEffect.m_nGenAniTick > 120 then begin
        m_CustomMagicStatusEffect.m_nGenAniTick := TimeGetTime;
        Inc(m_CustomMagicStatusEffect.m_nGenAniIndex);
        if m_CustomMagicStatusEffect.m_nGenAniIndex > 100000 then m_CustomMagicStatusEffect.m_nGenAniIndex := 0;
        Inc(m_CustomMagicStatusEffect.m_nStruck);
        if m_CustomMagicStatusEffect.m_nStruck > 100000 then m_CustomMagicStatusEffect.m_nStruck := 100000;
      end;

      wimg := nil;
      idx := 0;
      IsBlendDraw := False; //HZQ 20250525
      if (m_nCurrentAction = SM_STRUCK) and (m_CustomMagicStatusEffect.m_nStruck < m_CustomMagicStatusEffect.Status2_PlayCount)
        and (m_CustomMagicStatusEffect.Status2_File >= 0) and (m_CustomMagicStatusEffect.Status2_File < g_EffectImageList.Count) then begin
        if m_CustomMagicStatusEffect.Status2_CalcDir then
          idx := m_CustomMagicStatusEffect.Status2_StartIndex + (m_CustomMagicStatusEffect.Status2_PlayCount + m_CustomMagicStatusEffect.Status2_EmptyCount) * m_btDir + m_CustomMagicStatusEffect.m_nStruck
        else
          idx := m_CustomMagicStatusEffect.Status2_StartIndex + m_CustomMagicStatusEffect.m_nStruck;

        wimg := TGameImages(g_EffectImageList.Objects[m_CustomMagicStatusEffect.Status2_File]);
        IsBlendDraw := m_CustomMagicStatusEffect.Status2_DrawMode = mdmBlend;
      end else begin
        if (m_CustomMagicStatusEffect.Status1_PlayCount > 0) and (m_CustomMagicStatusEffect.Status1_File >= 0)
          and (m_CustomMagicStatusEffect.Status1_File < g_EffectImageList.Count) then begin
          if m_CustomMagicStatusEffect.Status2_CalcDir then begin
            idx := m_CustomMagicStatusEffect.Status1_StartIndex + (m_CustomMagicStatusEffect.Status1_PlayCount + m_CustomMagicStatusEffect.Status1_EmptyCount) * m_btDir +
              (m_CustomMagicStatusEffect.m_nGenAniIndex mod m_CustomMagicStatusEffect.Status1_PlayCount);
          end else begin
            idx := m_CustomMagicStatusEffect.Status1_StartIndex + (m_CustomMagicStatusEffect.m_nGenAniIndex mod m_CustomMagicStatusEffect.Status1_PlayCount);
          end;

          wimg := TGameImages(g_EffectImageList.Objects[m_CustomMagicStatusEffect.Status1_File]);
          IsBlendDraw := m_CustomMagicStatusEffect.Status1_DrawMode = mdmBlend;
        end;
      end;

      if wimg <> nil then begin
        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          d := wimg.GetCachedGrayImage(idx, ax, ay)
        else
          d := wimg.GetCachedImage(idx, ax, ay);

        if d <> nil then begin
          if IsBlendDraw then begin
            GameCanvas.DrawBlend(dx + ax + m_nShiftX, dy + ay + m_nShiftY + 2, d);
          end else begin
            GameCanvas.Draw(dx + ax + m_nShiftX, dy + ay + m_nShiftY + 2, d);
          end;
        end;
      end;
    end;

    if m_boSelfEffectRunning and (m_SelfEffectGameImage <> nil) then begin
      if TimeGetTime - m_dwSelfEffectLastTick >= m_nSelfEffectFrameTime then begin
        m_dwSelfEffectLastTick := TimeGetTime;
        Inc(m_nSelfEffectCurrentFrame);
      end;

      if m_nSelfEffectCurrentFrame > m_nSelfEffectEndFrame then begin
        m_boSelfEffectRunning := False;
        Exit;
      end;

      D := m_SelfEffectGameImage.GetCachedImage(m_nSelfEffectCurrentFrame, aX, aY);

      if D <> nil then begin
        if m_boSelfEffectBlendDraw then
          GameCanvas.DrawBlend(dx + ax + m_nShiftX, dy + ay + m_nShiftY, D)
        else
          GameCanvas.Draw(dx + ax + m_nShiftX, dy + ay + m_nShiftY, D);
      end;
    end;
  end;

  if IsBackActor then
    IsDraw := m_SelfKeepPlay.SelfPlay.SelfKeep_DrawOrder = mdoPriorMagic
  else
    IsDraw := m_SelfKeepPlay.SelfPlay.SelfKeep_DrawOrder = mdoPriorSelf;

  if IsDraw then begin
    if (TimeGetTime - m_SelfKeepPlay.SelfKeep_StartTime <= m_SelfKeepPlay.SelfPlay.SelfKeep_KeepTime * 1000) and
      (m_SelfKeepPlay.Images <> nil) and (m_SelfKeepPlay.SelfPlay.SelfKeep_PlayCount > 0) and (m_SelfKeepPlay.SelfPlay.SelfKeep_KeepTime > 0) then begin
      if (TimeGetTime - m_SelfKeepPlay.SelfKeep_LastTick) >= m_SelfKeepPlay.SelfPlay.SelfKeep_PlayTime then begin
        Inc(m_SelfKeepPlay.SelfKeep_Index);
        if m_SelfKeepPlay.SelfKeep_Index >= m_SelfKeepPlay.SelfPlay.SelfKeep_PlayCount then
          m_SelfKeepPlay.SelfKeep_Index := 0;

        m_SelfKeepPlay.SelfKeep_LastTick := TimeGetTime;
      end;

      if m_SelfKeepPlay.SelfPlay.SelfKeep_StartIndex >= 0 then begin
        idx := m_SelfKeepPlay.SelfPlay.SelfKeep_StartIndex + m_SelfKeepPlay.SelfKeep_Index;
        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          d := TGameImages(m_SelfKeepPlay.Images).GetCachedGrayImage(idx, ax, ay)
        else
          d := TGameImages(m_SelfKeepPlay.Images).GetCachedImage(idx, ax, ay);
        if d <> nil then begin
          if m_SelfKeepPlay.SelfPlay.SelfKeep_DrawMode = mdmBlend then
            GameCanvas.DrawBlend(
              dx + ax + m_nShiftX,
              dy + ay + m_nShiftY,
              d)
          else
            GameCanvas.Draw(
              dx + ax + m_nShiftX,
              dy + ay + m_nShiftY,
              d)
        end;
      end;

      if m_SelfKeepPlay.SelfPlay.SelfKeep_StartIndex2 >= 0 then begin
        idx := m_SelfKeepPlay.SelfPlay.SelfKeep_StartIndex2 + m_SelfKeepPlay.SelfKeep_Index;
        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          d := TGameImages(m_SelfKeepPlay.Images).GetCachedGrayImage(idx, ax, ay)
        else
          d := TGameImages(m_SelfKeepPlay.Images).GetCachedImage(idx, ax, ay);
        if d <> nil then begin
          if m_SelfKeepPlay.SelfPlay.SelfKeep_DrawMode2 = mdmBlend then
            GameCanvas.DrawBlend(
              dx + ax + m_nShiftX,
              dy + ay + m_nShiftY,
              d)
          else
            GameCanvas.Draw(
              dx + ax + m_nShiftX,
              dy + ay + m_nShiftY,
              d)
        end;
      end;
    end;
  end;
end;

procedure TActor.DrawLockTargetEffect(Frame:Integer; dx, dy:Integer);
var
  D:TTexture;
  aX, aY:Integer;
begin
  if not g_ConfigDlg.ConfigCheckeds[ckShowTargetAperture] then Exit;

  if (g_WNewopUIImages <> nil) then begin
    D := g_WNewopUIImages.GetCachedImage(Frame, aX, aY);

    if D <> nil then
      GameCanvas.Draw(dx + ax + m_nShiftX, dy + ay + m_nShiftY, D);
  end;
end;

//20230829 
function TActor.GetNearObjectHintInfo(var X, Y:Integer; out nFriendFlag:Integer):Boolean;
begin
    if (Self <> g_MySelf) and (not m_boDeath) and (g_BossList.IndexOf(Self.m_sUserName) >= 0) then begin
        X := X + Self.m_nShiftX;
        Y := Y + Self.m_nShiftY;
        if TSerialWindows(FrmDlg).DMemoFriend.Lines.IndexOf(Self.m_sUserName) <> -1 then begin
            nFriendFlag := 1;
        end else begin
            nFriendFlag := 0;
        end;
        Result := True;
    end else begin
        Result := False;
    end;
end;

procedure TActor.DrawExploreItemEffect(dx, dy:Integer);
var
  nX, nY:Integer;
  oX, oY:Integer;
  d:TTexture;
begin
  if not m_boDeath then Exit;
  if not m_IsExploreItem then Exit;
  if m_boSkeleton then Exit;

  if g_ConfigClient.dwExploreItemIconCount <= 0 then Exit;

  if tick_diff(m_dwDeathTick, TimeGetTime) < 800 then begin
    m_dwExploreItemEffectTick := timeGetTime;
    m_dwExploreItemEffectFrame := 0;
    Exit;
  end;

  if tick_diff(m_dwExploreItemEffectTick, TimeGetTime) >= g_ConfigClient.dwExploreItemIconPlayTime then begin
    Inc(m_dwExploreItemEffectFrame);
    m_dwExploreItemEffectTick := timeGetTime;
    if m_dwExploreItemEffectFrame >= g_ConfigClient.dwExploreItemIconCount then
      m_dwExploreItemEffectFrame := 0;
  end;

  d := g_WNewopUIImages.GetCachedImage(g_ConfigClient.dwExploreItemIconIndex + m_dwExploreItemEffectFrame, oX, oY);

  nX := m_nSayX + g_ConfigClient.nExploreItemIconOffsetX;
  nY := m_nSayY - 20 + g_ConfigClient.nExploreItemIconOffsetY;

  if d <> nil then begin
    GameCanvas.Draw(nX + oX, nY + oY, d);
  end;
end;

(*
procedure TActor.DrawPreviewItem(dx, dy: Integer);
const
  COL_WIDTH = 100;
  ROW_HEIGHT = 40;
var
  I, II: Integer;
  Item: pTPreviewMonItem;
  nCount, nIndex, nRow, nCol, nX, nY, nTempX, nTempY: Integer;
  d: TTexture;
  sName: string;
begin
  if m_boDeath or m_boGhost then Exit;

  if (m_PreviewItem = nil) or (Length(m_PreviewItem) = 0) then Exit;

  nCount := Length(m_PreviewItem);
  if nCount < 3 then
  begin
    nCol := nCount;
    nRow := 0;
  end
  else
  begin
    nCol := Trunc(Sqrt(nCount));
    nRow := (nCount + nCol - 1) div nCol;
  end;

  nIndex := 0;
  nY := dy - Round(((nRow - 1) / 2) * ROW_HEIGHT);
  for I := 0 to nRow - 1 do
  begin
    nX := dx - Round(((nCol - 1) / 2) * COL_WIDTH);

    for II := 0 to nCol - 1 do
    begin
      Item := @m_PreviewItem[nIndex];

      if (g_MySelf <> nil) and g_MySelf.m_boDeath then
        d := g_WDnItemImages.Grays[Item.wLooks]
      else
        d := g_WDnItemImages.Images[Item.wLooks];

      sName := Item.sName;
      if Item.nCount > 1 then
      begin
        sName := sName + ' ×' + IntToStr(Item.nCount);
      end;

      if d <> nil then
      begin
        nTempX := nX + (20 - d.Width) div 2;
        nTempY := nY + (20 - d.Height) div 2;
        GameCanvas.Draw(nTempX,
          nTempY,
          d.ClientRect,
          d);
      end;

      nTempX := nX + (20 - CurrentFont.TextWidth(sName)) div 2;
      nTempY := nY + 20;

      BoldTextOut(nTempX, nTempY, sName, GetRGB(Item.btColor));

      nX := nX + COL_WIDTH;
      Inc(nIndex);
      if nIndex >= nCount then Exit;
    end;

    nY := nY + ROW_HEIGHT;
  end;
end;
*)

procedure TActor.LoadHealthNumber;
var
  I, II:Integer;
  HealthNumber:pTHealthNumber;
  NumberType:TNumberType;
  nOffsetIndex:Integer;
  nIndex:Integer;
  boShowHealthNumber:Boolean;
  Texture:TTexture;
begin
  if (not GameCanvas.Active) or (not GameCanvas.Initialized) then Exit;
  boShowHealthNumber := (PlugInEnabled and g_ClientConfig.boShowMoveLable and g_ConfigDlg.ConfigCheckeds[ckShowMoveLable]);

  if boShowHealthNumber then begin
    EnterCriticalSection(m_HealthNumberLock);
    try
      for I := 0 to Length(m_HealthNumberArray) - 1 do begin

        nOffsetIndex := -1; //HZQ 20230525
        HealthNumber := @m_HealthNumberArray[I];
        NumberType := HealthNumber.nNmType; //HZQ 20230609 程序优化，直接读取数值

        if HealthNumber.sNumber <> '' then begin
          if HealthNumber.nResID < 0 then begin //HZQ 增加自定义飘血资源处理
              case NumberType of
                tHP:nOffsetIndex := 50;
                tMP:nOffsetIndex := 70;
                tGreen:nOffsetIndex := 90;
                tMiss:nOffsetIndex := 204;
                tTextHP:nOffsetIndex := 500;
                tBlastHP:nOffsetIndex := 520;
                tFatalBlow1:nOffsetIndex := 1670;
                tFatalBlow2:nOffsetIndex := 1690;
                tFatalBlow3:nOffsetIndex := 1710;
                tFatalBlow4:nOffsetIndex := 1730;
              end;
          end else begin
              nOffsetIndex := HealthNumber.nResStartIdx;
          end;

          if g_ConfigClient.boHealthNumberSeparate then begin
            if HealthNumber.nResID < 0  then begin //HZQ 20230609 自定义飘血
              // 自己
              if (Self = g_MySelf) or (Self = g_MyHero) then begin
                case NumberType of
                  tHP:nOffsetIndex := g_ConfigClient.nHealthNumberSelfOffset;
                  tMP:nOffsetIndex := g_ConfigClient.nHealthNumberSelfOffset + 20;
                  tGreen:nOffsetIndex := g_ConfigClient.nHealthNumberSelfOffset + 40;
                  tTextHP:nOffsetIndex := g_ConfigClient.nHealthNumberSelfOffset + 60;
                  tBlastHP:nOffsetIndex := g_ConfigClient.nHealthNumberSelfOffset + 80;
                  //tFatalBlow1: nOffsetIndex := g_ConfigClient.nHealthNumberSelfOffset + 100;
                  //tFatalBlow2: nOffsetIndex := g_ConfigClient.nHealthNumberSelfOffset + 120;
                  //tFatalBlow3: nOffsetIndex := g_ConfigClient.nHealthNumberSelfOffset + 140;
                  //tFatalBlow4: nOffsetIndex := g_ConfigClient.nHealthNumberSelfOffset + 160;
                end;
              end else if (m_btRace in [RC_PLAYOBJECT, RC_HEROOBJECT]) and (not m_boPlayMoster {非人行怪}) then begin // 别人
                case NumberType of
                  tHP:nOffsetIndex := g_ConfigClient.nHealthNumberHumOffset;
                  tMP:nOffsetIndex := g_ConfigClient.nHealthNumberHumOffset + 20;
                  tGreen:nOffsetIndex := g_ConfigClient.nHealthNumberHumOffset + 40;
                  tTextHP:nOffsetIndex := g_ConfigClient.nHealthNumberHumOffset + 60;
                  tBlastHP:nOffsetIndex := g_ConfigClient.nHealthNumberHumOffset + 80;
                  //tFatalBlow1: nOffsetIndex := g_ConfigClient.nHealthNumberHumOffset + 100;
                  //tFatalBlow2: nOffsetIndex := g_ConfigClient.nHealthNumberHumOffset + 120;
                  //tFatalBlow3: nOffsetIndex := g_ConfigClient.nHealthNumberHumOffset + 140;
                  //tFatalBlow4: nOffsetIndex := g_ConfigClient.nHealthNumberHumOffset + 160;
                end;
              end;
            end else begin
                nOffsetIndex := HealthNumber.nResStartIdx;
            end;
          end;

          //if (HealthNumber.nWidth <= 0) then begin
          if (nOffsetIndex >= 0) and (HealthNumber.nWidth <= 0) then begin //HZQ 20230525
            if (HealthNumber.nResID >= 0) and (HealthNumber.nResStartIdx >= 0) then begin
              if HealthNumber.nNumber > 0 then begin
                nIndex := nOffsetIndex + 11; //+
              end else begin
                nIndex := nOffsetIndex + 10; //-
              end;

              Texture := MShare.GetEffectImageListTexture(HealthNumber.nResID, nIndex); //g_WNewopUIImages.Images[nIndex];
              if Texture <> nil then begin
                SetLength(HealthNumber.ImageIndexs, Length(HealthNumber.ImageIndexs) + 1);
                HealthNumber.ImageIndexs[Length(HealthNumber.ImageIndexs) - 1] := nIndex;
                Inc(HealthNumber.nWidth, Texture.Width);
                HealthNumber.nHeight := Max(HealthNumber.nHeight, Texture.Height);
              end;

              for II := 1 to Length(HealthNumber.sNumber) do begin
                  nIndex := nOffsetIndex + StrToInt(HealthNumber.sNumber[II]);
                  Texture := MShare.GetEffectImageListTexture(HealthNumber.nResID, nIndex);
                  if Texture <> nil then begin
                    SetLength(HealthNumber.ImageIndexs, Length(HealthNumber.ImageIndexs) + 1);
                    HealthNumber.ImageIndexs[Length(HealthNumber.ImageIndexs) - 1] := nIndex;
                    Inc(HealthNumber.nWidth, Texture.Width);
                    HealthNumber.nHeight := Max(HealthNumber.nHeight, Texture.Height);
                  end;
              end;
              Inc(HealthNumber.nHeight, 2);
            end else begin 
              if NumberType = tMiss then begin
                Texture := g_WNewopUIImages.Images[nOffsetIndex];
                if Texture <> nil then begin
                  SetLength(HealthNumber.ImageIndexs, Length(HealthNumber.ImageIndexs) + 1);
                  HealthNumber.ImageIndexs[Length(HealthNumber.ImageIndexs) - 1] := nOffsetIndex;
                  Inc(HealthNumber.nWidth, Texture.Width);
                  HealthNumber.nHeight := Max(HealthNumber.nHeight, Texture.Height);
                end;
              end else begin
                HealthNumber.nHeight := 0;

                if NumberType in [tFatalBlow1, tFatalBlow2, tFatalBlow3, tFatalBlow4] then begin
                  nIndex := nOffsetIndex + 11;
                  Texture := g_WNewopUIImages.Images[nIndex];
                  if Texture <> nil then begin
                    SetLength(HealthNumber.ImageIndexs, Length(HealthNumber.ImageIndexs) + 1);
                    HealthNumber.ImageIndexs[Length(HealthNumber.ImageIndexs) - 1] := nIndex;
                    Inc(HealthNumber.nWidth, Texture.Width);
                    HealthNumber.nHeight := Max(HealthNumber.nHeight, Texture.Height);
                  end;

                  if HealthNumber.nNumber < 0 then begin
                    nIndex := nOffsetIndex + 10;
                    Texture := g_WNewopUIImages.Images[nIndex];
                    if Texture <> nil then begin
                      SetLength(HealthNumber.ImageIndexs, Length(HealthNumber.ImageIndexs) + 1);
                      HealthNumber.ImageIndexs[Length(HealthNumber.ImageIndexs) - 1] := nIndex;
                      Inc(HealthNumber.nWidth, Texture.Width);
                      HealthNumber.nHeight := Max(HealthNumber.nHeight, Texture.Height);
                    end;
                  end;
                end else begin
                  if HealthNumber.nNumber > 0 then begin
                    nIndex := nOffsetIndex + 11;
                  end else begin
                    nIndex := nOffsetIndex + 10;
                  end;
                  Texture := g_WNewopUIImages.Images[nIndex];
                  if Texture <> nil then begin
                    SetLength(HealthNumber.ImageIndexs, Length(HealthNumber.ImageIndexs) + 1);
                    HealthNumber.ImageIndexs[Length(HealthNumber.ImageIndexs) - 1] := nIndex;
                    Inc(HealthNumber.nWidth, Texture.Width);
                    HealthNumber.nHeight := Max(HealthNumber.nHeight, Texture.Height);
                  end;
                end;

                for II := 1 to Length(HealthNumber.sNumber) do begin
                  nIndex := nOffsetIndex + StrToInt(HealthNumber.sNumber[II]);
                  Texture := g_WNewopUIImages.Images[nIndex];
                  if Texture <> nil then begin
                    SetLength(HealthNumber.ImageIndexs, Length(HealthNumber.ImageIndexs) + 1);
                    HealthNumber.ImageIndexs[Length(HealthNumber.ImageIndexs) - 1] := nIndex;
                    Inc(HealthNumber.nWidth, Texture.Width);
                    HealthNumber.nHeight := Max(HealthNumber.nHeight, Texture.Height);
                  end;
                end;
                Inc(HealthNumber.nHeight, 2);
              end;
            end;
          end;
        end;
      end;
    finally
      LeaveCriticalSection(m_HealthNumberLock);
    end;
  end;
end;

procedure ClearHealthNumber(pHN:pTHealthNumber);
begin
    pHN.nWidth := 0;
    SetLength(pHN.ImageIndexs, 0);
    pHN.sNumber := '';
end;

function TActor.CheckLoadHealthNumber:Boolean;
var
  I:Integer;
  HealthNumber:pTHealthNumber;
  //NumberType:TNumberType;
  boShowHealthNumber:Boolean;
  //HealthNumbeTextHPCount: Integer;
  //HealthNumbeBlastHPCount: Integer;
  dwCurTick:Cardinal;
begin
  Result := False;

  if (not GameCanvas.Active) or (not GameCanvas.Initialized) then Exit;
  boShowHealthNumber := (PlugInEnabled and g_ClientConfig.boShowMoveLable and g_ConfigDlg.ConfigCheckeds[ckShowMoveLable]);

  if boShowHealthNumber then begin
    EnterCriticalSection(m_HealthNumberLock);
    try
      dwCurTick := timeGetTime;

      for I := 0 to Length(m_HealthNumberArray) - 1 do begin
        HealthNumber := @m_HealthNumberArray[I];
        //NumberType := HealthNumber.nNmType;
        if HealthNumber.sNumber <> '' then begin
          if (dwCurTick - HealthNumber.dwUpdateHPTick) >= Cardinal(g_ClientConfig.nHealthNumberMoveSpeed) then begin
              HealthNumber.dwUpdateHPTick := dwCurTick;
              case HealthNumber.nDrawStyle of
              ndsNormal:begin
                  Inc(HealthNumber.nOffsetX);
                  Dec(HealthNumber.nOffsetY);
                  if abs(HealthNumber.nOffsetY - g_ClientConfig.nHealthNumberOffsetY) >= 50 then begin
                      ClearHealthNumber(HealthNumber);
                  end;
              end;

              ndsFastExit:begin
                  Dec(HealthNumber.nOffsetY);
                  if abs(HealthNumber.nOffsetY) >= 20 then begin
                      ClearHealthNumber(HealthNumber);
                  end;
              end;

              ndsMovingFadeOut: begin
                  Inc(HealthNumber.nOffsetX, 2);
                  Dec(HealthNumber.nOffsetY, 2);
                  HealthNumber.byAlpha := Max(0, 255 - Abs(HealthNumber.nOffsetX - g_ClientConfig.nHealthNumberOffsetX) * 3);
                  if abs(HealthNumber.nOffsetY - g_ClientConfig.nHealthNumberOffsetY) >= 50 then begin
                      ClearHealthNumber(HealthNumber);
                  end;
              end;

              ndsStayFadeOut:begin
                  if abs(HealthNumber.nOffsetY - g_ClientConfig.nHealthNumberOffsetY) >= 30 then begin
                      if HealthNumber.byAlpha >= 24 then begin
                          HealthNumber.byAlpha := HealthNumber.byAlpha - 24;
                      end else begin
                          HealthNumber.byAlpha := 0;
                      end;

                      if HealthNumber.byAlpha <= 24 then begin
                         ClearHealthNumber(HealthNumber);
                      end;
                  end else begin
                      Inc(HealthNumber.nOffsetX, 1);
                      Dec(HealthNumber.nOffsetY, 1);
                  end;   
              end;

              else begin
                  Inc(HealthNumber.nOffsetX, 2);
                  Dec(HealthNumber.nOffsetY, 2);
                  if abs(HealthNumber.nOffsetY - g_ClientConfig.nHealthNumberOffsetY) >= 50 then begin
                      ClearHealthNumber(HealthNumber);
                  end;
              end;
              end;
          end;

          if HealthNumber.nWidth <= 0 then begin
            Result := True;
          end;
        end;
      end;
    finally
      LeaveCriticalSection(m_HealthNumberLock);
    end;
  end;

  if m_boShowHealthNumber <> boShowHealthNumber then begin
    m_boShowHealthNumber := boShowHealthNumber;
    if m_boShowHealthNumber then
      Result := True;
  end;
end;

function TActor.CheckLoadNumberLable:Boolean;
var
  Abil:TAbility;
begin
  //加载血条文字（数字或百分比）
  Result := False;
  if (not GameCanvas.Active) or (not GameCanvas.Initialized) then Exit;
  m_sNumberLableText := '';

  if PlugInEnabled and (not m_boDeath) then begin
    Abil := m_Abil;
    // 这里调整了最大血量  chongchong 2016-06-04
    {
    if Abil.MaxHP < Abil.HP then
      Abil.MaxHP := Abil.HP;
    }

    if Abil.MaxMP < Abil.MP then
      Abil.MaxMP := Abil.MP;

    // 显示人物血量(数字显示)Jacky

    if (m_btRace in [RC_PLAYOBJECT, RC_HEROOBJECT]) then begin
      if (g_ClientConfig.boHumStruckShowNumber and m_boStruckShowNumber) or (not g_ClientConfig.boHumStruckShowNumber)
        or (Self = g_MySelf) or (Self = g_MyHero) {自己英雄显血 piaoyun 2013-09-07} or m_boOpenHealth then begin
        if ((g_ClientConfig.boShowNumberLable and g_ConfigDlg.ConfigCheckeds[ckShowNumberLable]) or m_boOpenHealth)
          and (Abil.MaxHP > 0) then begin
          // 别的人物英雄百分比显示血量 chongchong 2016-10-11
          if g_boMapHumAndHeroPercentHP (*and (not m_boPlayMoster {排除掉人形怪})*) //HZQ 20230529把别人排除掉的人形怪百分比显示还原
          and (((Self <> g_MySelf) and (Self <> g_MyHero)) or m_boOpenHealth) then begin
            m_sNumberLableText := IntToStr(Min(100, Round(Abil.HP / Abil.MaxHP * 100))) + '%'
          end else begin
            if g_ClientConfig.boShowHPUnit and g_ConfigDlg.ConfigCheckeds[ckShowHPUnit] then begin
              m_sNumberLableText := HpAddUnit(Abil.HP) + '/' + HpAddUnit(Abil.MaxHP);
            end else begin
              m_sNumberLableText := IntToStr(Abil.HP) + '/' + IntToStr(Abil.MaxHP);
            end;
          end;
        end;
      end;
    end else begin
      if (g_ClientConfig.boMonStruckShowNumber and m_boStruckShowNumber) or (not g_ClientConfig.boMonStruckShowNumber)
        or m_boOpenHealth then begin
        if (m_btRace <> RC_MERCHANT) and (Abil.MaxHP > 0)
          and ((g_ClientConfig.boShowNumberLable and g_ConfigDlg.ConfigCheckeds[ckShowNumberLable]) or m_boOpenHealth) then begin
          // m_sNumberLableText := IntToStr(Abil.HP) + '/' + IntToStr(Abil.MaxHP);
          if g_ClientConfig.boShowHPUnit and g_ConfigDlg.ConfigCheckeds[ckShowHPUnit] then begin
            m_sNumberLableText := HpAddUnit(Abil.HP) + '/' + HpAddUnit(Abil.MaxHP);
          end else begin
            m_sNumberLableText := IntToStr(Abil.HP) + '/' + IntToStr(Abil.MaxHP);
          end;
        end;
      end;
    end;

    if (m_btRace {<= 1} in [0, 1]) and (Abil.MaxHP > 0) and (Abil.Level > 0) then begin
      if g_ClientConfig.boShowJobAndLevel and g_ConfigDlg.ConfigCheckeds[ckShowJobAndLevel] then begin // 显示人物职业等级(数字显示)
        if m_sNumberLableText <> '' then m_sNumberLableText := m_sNumberLableText + '/';
        case m_btJob of
          0:m_sNumberLableText := m_sNumberLableText + 'Z';
          1:m_sNumberLableText := m_sNumberLableText + 'F';
          2:m_sNumberLableText := m_sNumberLableText + 'D';
          3:m_sNumberLableText := m_sNumberLableText + 'C';
          else
            m_sNumberLableText := m_sNumberLableText + 'UnKnow';
        end;
        m_sNumberLableText := m_sNumberLableText + IntToStr(Abil.Level);
      end;
    end;
  end;

  if (CompareText(m_sCurNumberLableText, m_sNumberLableText) <> 0) or
    (m_Abil.HP <> m_OAbil.HP) or (m_Abil.MaxHP <> m_OAbil.MaxHP) or
    (TimeGetTime - m_ShowNumberLableTimeTick > 30) then begin
    m_OAbil := m_Abil;
    Result := True;
  end;
end;

procedure TActor.LoadNumberLableSurface;
begin
  if (not GameCanvas.Active) or (not GameCanvas.Initialized) then Exit;
  m_ShowNumberLableTimeTick := MyGetTickCount;
  m_NumberLableImageInfo.Width := 0;
  m_NumberLableImageInfo.Height := 0;
  m_NumberLableImageInfo.ImageIndexs := nil;
  if CurrentFont <> nil then begin
    m_sCurNumberLableText := m_sNumberLableText;
    m_NumberLableImageInfo := CurrentFont.GetImageInfo(m_sNumberLableText);
  end;

end;

procedure TActor.ShowSay;

  procedure DrawSay(nX, nY:Integer; ImageIndexs:TImageIndexs; Color:TColor);
  begin
    if CurrentFont <> nil then begin
      CurrentFont.TextOut(nX - 1, nY, ImageIndexs, clBlack);
      CurrentFont.TextOut(nX + 1, nY, ImageIndexs, clBlack);
      CurrentFont.TextOut(nX, nY - 1, ImageIndexs, clBlack);
      CurrentFont.TextOut(nX, nY + 1, ImageIndexs, clBlack);
      CurrentFont.TextOut(nX, nY, ImageIndexs, Color);
    end;
  end;
var
  I:Integer;
  Color:TColor;
  HpBarOffsetX, HpBarOffsetY, HPOffsetY:Integer;
begin
  if (m_SayingArr[0].Text <> '') then begin
    if TimeGetTime - m_dwSayTime < 4 * 1000 then begin
      if m_boCanDraw then begin
        if (m_btRace in [RC_PLAYOBJECT, RC_HEROOBJECT]) then begin
          HpBarOffsetX := g_ClientConfig.nHumHPBarOffsetX;
          HpBarOffsetY := g_ClientConfig.nHumHPBarOffsetY;
        end
        else if (m_btRace = RC_MERCHANT) then begin
          HpBarOffsetX := g_ClientConfig.nNpcHPBarOffsetX;
          HpBarOffsetY := g_ClientConfig.nNpcHPBarOffsetY;
        end
        else begin
          HpBarOffsetX := g_ClientConfig.nMonHPBarOffsetX;
          HpBarOffsetY := g_ClientConfig.nMonHPBarOffsetY;
        end;

        if g_NewopUI170TextureArray[0] <> nil then begin
          if g_ConfigDlg.ConfigCheckeds[ckShowNumberLable] or g_ConfigDlg.ConfigCheckeds[ckShowJobAndLevel] then
            HPOffsetY := g_NewopUI170TextureArray[0].Height * 2 + 14
          else if g_ConfigDlg.ConfigCheckeds[ckShowHPLabel] then
            HPOffsetY := g_NewopUI170TextureArray[0].Height * 2 + 2
          else
            HPOffsetY := 2;
        end
        else begin
          if g_ConfigDlg.ConfigCheckeds[ckShowNumberLable] then
            HPOffsetY := 18
          else if g_ConfigDlg.ConfigCheckeds[ckShowHPLabel] then
            HPOffsetY := 6
          else
            HPOffsetY := 2;
        end;

        // 称号显示时，坐标再上移 2020-08-03 21:57:23
        if (CurrentFont <> nil) and ((m_FengHaoEffectSurface <> nil) or (Length(m_FengHaoImageInfo) > 0)) then begin
          HPOffsetY := HPOffsetY + 18;
        end;

        for I := 0 to m_nSayLineCount - 1 do begin
          if Length(m_SayingArr[I].TextSurface.ImageIndexs) > 0 then begin
            if m_boDeath then
              Color := clGray
            else
              Color := m_HearMsgColor; // clWhite;

            DrawSay(
              m_nSayX - (m_SayingArr[I].TextSurface.Width div 2) + HpBarOffsetX,
              m_nSayY - HPOffsetY + HpBarOffsetY - (m_nSayLineCount * 16) + I * 14,
              m_SayingArr[I].TextSurface.ImageIndexs,
              Color);
          end;
        end;
      end;
    end
    else begin
      m_SayingArr[0].Text := '';
      m_SayingArr[0].TextSurface.Width := 0;
      m_SayingArr[0].TextSurface.Height := 0;
      m_SayingArr[0].TextSurface.ImageIndexs := nil;
    end;
  end;
end;

function TActor.CheckLoadSay:Boolean;
var
  I:Integer;
begin
  Result := False;
  if (not GameCanvas.Active) or (not GameCanvas.Initialized) then Exit;
  if (m_SayingArr[0].Text <> '') then begin
    if TimeGetTime - m_dwSayTime < 2 * 1000 then begin
      for I := 0 to m_nSayLineCount - 1 do begin
        if (m_SayingArr[I].Text <> '') and (Length(m_SayingArr[I].TextSurface.ImageIndexs) <= 0) then begin
          Result := True;
          break;
        end;
      end;
    end
    else begin
      m_SayingArr[0].Text := '';
      m_SayingArr[0].TextSurface.Width := 0;
      m_SayingArr[0].TextSurface.Height := 0;
      m_SayingArr[0].TextSurface.ImageIndexs := nil;
    end;
  end;
end;

procedure TActor.LoadSaySurface;
var
  I:Integer;
begin
  if (m_SayingArr[0].Text <> '') then begin
    if (not GameCanvas.Active) or (not GameCanvas.Initialized) then Exit;
    for I := 0 to m_nSayLineCount - 1 do begin
      if (m_SayingArr[I].Text <> '') then begin
        m_SayingArr[I].TextSurface := CurrentFont.GetImageInfo(m_SayingArr[I].Text);
      end;
    end;
  end
  else begin
    for I := 0 to Length(m_SayingArr) - 1 do begin
      m_SayingArr[I].Text := '';
      m_SayingArr[I].TextSurface.Width := 0;
      m_SayingArr[I].TextSurface.Height := 0;
      m_SayingArr[I].TextSurface.ImageIndexs := nil;
    end;
  end;
end;

procedure TActor.LoadNameSurface;
var
  sNameText, sName:string;
  SL:TStringList;
begin
  if m_NameTextSurface <> nil then begin
    m_NameTextSurface.Free;
    m_NameTextSurface := nil;
  end;

  if (not GameCanvas.Active) or (not GameCanvas.Initialized) then Exit;
  if CurrentFont = nil then Exit;

  if Length(m_sNameText) > 0 then begin
    SL := TStringList.Create;
    try
      sNameText := m_sNameText;
      while True do begin
        if sNameText = '' then break;
        sNameText := GetValidStr3_Ex(sNameText, sName, '\');
        SL.Add(sName);
      end;

      m_NameTextSurface := GetTextTexture(CurrentFont, SL);
    finally
      SL.Free;
    end;
  end;

  if m_boShopStall then
    m_ShopNameImageInfo := CurrentFont.GetImageInfo(m_sShopNameText);

  m_dwShowShopNameTimeTick := MyGetTickCount;
  m_sCurNameText := m_sNameText;
  m_sCurShopNameText := m_sShopNameText;
end;

procedure TActor.LoadFengHaoSurface;
var
  Images:TGameImages;
  dx, dy:Integer;
begin
  SetLength(m_FengHaoImageInfo, 0);
  m_nOldFengHaoSurfaceID := m_nActiveFengHaoID;
  m_FengHaoEffectSurface := nil;

  if (not GameCanvas.Active) or (not GameCanvas.Initialized) then Exit;

  // 骑马 双人骑被邀请人不显示 chongchong 2013-10-14
  if (m_btHorse in [1, 2]) and (m_btDoubleHumHorse = 0) then Exit;

  // Reserved = 2, 不显示封号
  if m_btActiveFengHaoReserved = 2 then begin
    m_FengHaoEffectSurface := nil;
    Exit;
  end;

  // 摆摊时不显示称号 chongchong 2016-08-31
  if m_boShopStall then begin
    m_FengHaoEffectSurface := nil;
    Exit;
  end;

  if CurrentFont <> nil then begin
    if (m_sActiveFengHaoName <> '') and (m_btActiveFengHaoReserved = 0) then begin
      SetLength(m_FengHaoImageInfo, Length(m_FengHaoImageInfo) + 1);
      m_FengHaoImageInfo[Length(m_FengHaoImageInfo) - 1] := CurrentFont.GetImageInfo(m_sActiveFengHaoName);
    end;
  end;

  if g_ClientConfig.nTitleFileIndex < 0 then Exit;
  if g_ClientConfig.nTitleFileIndex >= g_EffectImageList.Count then Exit;

  Images := TGameImages(g_EffectImageList.Objects[g_ClientConfig.nTitleFileIndex]);
  if Images <> nil then begin
    if m_boDeath then
      m_FengHaoEffectSurface := Images.GetCachedGrayImage(m_dwActiveFengHaoLooks, dx, dy)
    else
      m_FengHaoEffectSurface := Images.GetCachedImage(m_dwActiveFengHaoLooks, dx, dy);
    m_dwLoadFengHaoSurfaceTime := MyGetTickCount;
  end;
end;

procedure TActor.ShowName;
const
  _OFFSET = 2;
var
  nX, nY:Integer;
  nColor:Integer;
  HpBarOffsetX, HpBarOffsetY:Integer;
begin
  // 绘制封号 chongchong 2014-05-25
  nX := m_nSayX;

  if g_NewopUI170TextureArray[0] <> nil then begin
    if g_ConfigDlg.ConfigCheckeds[ckShowNumberLable] or g_ConfigDlg.ConfigCheckeds[ckShowJobAndLevel] then
      nY := m_nSayY - g_NewopUI170TextureArray[0].Height * 2 - 32
    else if g_ConfigDlg.ConfigCheckeds[ckShowHPLabel] then
      nY := m_nSayY - g_NewopUI170TextureArray[0].Height * 2 - 20
    else
      nY := m_nSayY - 19;
  end
  else begin
    if g_ConfigDlg.ConfigCheckeds[ckShowNumberLable] then
      nY := m_nSayY - 38
    else if g_ConfigDlg.ConfigCheckeds[ckShowHPLabel] then
      nY := m_nSayY - 26
    else
      nY := m_nSayY - 19;
  end;

  if (m_btRace in [RC_PLAYOBJECT, RC_HEROOBJECT]) then begin
    HpBarOffsetX := g_ClientConfig.nHumHPBarOffsetX;
    HpBarOffsetY := g_ClientConfig.nHumHPBarOffsetY;
  end
  else if (m_btRace = RC_MERCHANT) then begin
    HpBarOffsetX := g_ClientConfig.nNpcHPBarOffsetX;
    HpBarOffsetY := g_ClientConfig.nNpcHPBarOffsetY;
  end
  else begin
    HpBarOffsetX := g_ClientConfig.nMonHPBarOffsetX;
    HpBarOffsetY := g_ClientConfig.nMonHPBarOffsetY;
  end;

  // 摆摊不绘制些信息 add not m_boShopStall 2019-07-23 10:14:54
  if not m_boShopStall then begin
    if (CurrentFont <> nil) then begin
      if m_FengHaoEffectSurface <> nil then begin
        nY := nY + 16 - m_FengHaoEffectSurface.Height + HpBarOffsetY;
        if (Length(m_FengHaoImageInfo) = 0) then begin
          GameCanvas.Draw(nX - m_FengHaoEffectSurface.Width div 2 + HpBarOffsetX, nY, m_FengHaoEffectSurface);
        end
        else begin
          nX := nX - (m_FengHaoImageInfo[0].Width + _OFFSET + m_FengHaoEffectSurface.Width) div 2;
          GameCanvas.Draw(nX + HpBarOffsetX, nY - (m_FengHaoEffectSurface.Height - g_CurrentFontHeight) div 2, m_FengHaoEffectSurface);

          nX := nX + m_FengHaoEffectSurface.Width + _OFFSET + HpBarOffsetX;
          CurrentFont.TextOut(nX - 1, nY, m_FengHaoImageInfo[0].ImageIndexs, clBlack);
          CurrentFont.TextOut(nX + 1, nY, m_FengHaoImageInfo[0].ImageIndexs, clBlack);
          CurrentFont.TextOut(nX, nY - 1, m_FengHaoImageInfo[0].ImageIndexs, clBlack);
          CurrentFont.TextOut(nX, nY + 1, m_FengHaoImageInfo[0].ImageIndexs, clBlack);

          CurrentFont.TextOut(nX, nY, m_FengHaoImageInfo[0].ImageIndexs, m_nActiveFengHaoColor);
        end;
      end
      else if (Length(m_FengHaoImageInfo) > 0) then begin
        nX := nX - m_FengHaoImageInfo[0].Width div 2 + HpBarOffsetX;
        CurrentFont.TextOut(nX - 1, nY, m_FengHaoImageInfo[0].ImageIndexs, clBlack);
        CurrentFont.TextOut(nX + 1, nY, m_FengHaoImageInfo[0].ImageIndexs, clBlack);
        CurrentFont.TextOut(nX, nY - 1, m_FengHaoImageInfo[0].ImageIndexs, clBlack);
        CurrentFont.TextOut(nX, nY + 1, m_FengHaoImageInfo[0].ImageIndexs, clBlack);

        CurrentFont.TextOut(nX, nY, m_FengHaoImageInfo[0].ImageIndexs, m_nActiveFengHaoColor);
      end;
    end;
  end;

  if (m_btRace in [RC_PLAYOBJECT, RC_HEROOBJECT]) then begin
    HpBarOffsetX := g_ClientConfig.nHumNameOffsetX;
    HpBarOffsetY := g_ClientConfig.nHumNameOffsetY;
  end
  else if (m_btRace = RC_MERCHANT) then begin
    HpBarOffsetX := g_ClientConfig.nNpcNameOffsetX;
    HpBarOffsetY := g_ClientConfig.nNpcNameOffsetY;
  end
  else begin
    HpBarOffsetX := g_ClientConfig.nMonNameOffsetX;
    HpBarOffsetY := g_ClientConfig.nMonNameOffsetY;
  end;

  // 绘制人名 chongchong 2014-05-25
  nX := m_nSayX;
  if m_btHorse = 0 then
    nY := m_nSayY + 30
  else
    nY := m_nSayY + 50;

  nX := nX + HpBarOffsetX;
  nY := nY + HpBarOffsetY;

  if (m_NameTextSurface <> nil) and m_boCanDraw then begin
    nColor := m_nNameColor;

    if (Self is TStatuaryNpcActor) then {// and (I <> 0) then} begin
      nColor := GetRGB(g_ClientConfig.btMerchant273NameColor);
    end
    else if (Self is THumActor) then begin
      if g_MyTargetList.IndexOf(m_sUserName) >= 0 then begin
        nColor := clLime;
      end;
    end;

    GameCanvas.DrawColor(nX - 1 - m_NameTextSurface.Width div 2, nY, m_NameTextSurface, clBlack);
    GameCanvas.DrawColor(nX + 1 - m_NameTextSurface.Width div 2, nY, m_NameTextSurface, clBlack);
    GameCanvas.DrawColor(nX - m_NameTextSurface.Width div 2, nY - 1, m_NameTextSurface, clBlack);
    GameCanvas.DrawColor(nX - m_NameTextSurface.Width div 2, nY + 1, m_NameTextSurface, clBlack);
    GameCanvas.DrawColor(nX - m_NameTextSurface.Width div 2, nY, m_NameTextSurface, nColor);
  end;
end;

procedure TActor.ShowShopName;
var
  nX, nY:Integer;
  TempN:Integer;
  HpBarOffsetX, HpBarOffsetY:Integer;
  DShopNameBG:TTexture;
  R:TRect;
begin
  if m_boShopStall and (m_ShopNameImageInfo.Width > 0) and (CurrentFont <> nil) then begin
    m_dwShowShopNameTimeTick := MyGetTickCount;
    if m_boCanDraw then begin
      if (m_btRace in [RC_PLAYOBJECT, RC_HEROOBJECT]) then begin
        HpBarOffsetX := g_ClientConfig.nHumNameOffsetX;
        HpBarOffsetY := g_ClientConfig.nHumNameOffsetY;
      end
      else if (m_btRace = RC_MERCHANT) then begin
        HpBarOffsetX := g_ClientConfig.nNpcNameOffsetX;
        HpBarOffsetY := g_ClientConfig.nNpcNameOffsetY;
      end
      else begin
        HpBarOffsetX := g_ClientConfig.nMonNameOffsetX;
        HpBarOffsetY := g_ClientConfig.nMonNameOffsetY;
      end;

      DShopNameBG := g_WNewopUIImages.Grays[102];

      // 处理摆摊商铺名称绘制位置
      if not g_ClientConfig.boShopHeadPic then begin
        nY := m_nSayY - 25;
      end
      else begin
        if DShopNameBG <> nil then
          nY := m_nSayY - 43
        else
          nY := m_nSayY - 40;
      end;

      nX := m_nSayX - m_ShopNameImageInfo.Width div 2;

      nX := nX + HpBarOffsetX;
      nY := nY + HpBarOffsetY;

      if DShopNameBG <> nil then begin
        TempN := (DShopNameBG.Height - m_ShopNameImageInfo.Height) div 2;
        R.Left := nX - 1 - TempN;
        R.Top := nY - 1 - TempN;

        R.Right := R.Left + m_ShopNameImageInfo.Width + 2 + TempN * 2;
        R.Bottom := R.Top + DShopNameBG.Height;
        GameCanvas.StretchDraw(R, DShopNameBG);
      end;

      CurrentFont.TextOut(nX - 1, nY, m_ShopNameImageInfo.ImageIndexs, clBlack);
      CurrentFont.TextOut(nX + 1, nY, m_ShopNameImageInfo.ImageIndexs, clBlack);
      CurrentFont.TextOut(nX, nY - 1, m_ShopNameImageInfo.ImageIndexs, clBlack);
      CurrentFont.TextOut(nX, nY + 1, m_ShopNameImageInfo.ImageIndexs, clBlack);

      if (Self = g_MySelf) or (Self = g_FocusCret) then
        CurrentFont.TextOut(nX, nY, m_ShopNameImageInfo.ImageIndexs, GetRGB(250))
      else
        CurrentFont.TextOut(nX, nY, m_ShopNameImageInfo.ImageIndexs, $0086C2DF);
    end;
  end;
end;

// 数字显血绘制 piaoyun 2013-09-13

procedure TActor.ShowNumberLable;
var
  nX, nY, oX, oY:Integer;
  BaseConfig:PClientBaseConfig;
  HpBarOffsetX, HpBarOffsetY:Integer;
begin
  // 摆摊屏蔽数字显血 piaoyun 2013-09-13
  if m_boShopStall { and g_ClientConfig.boShopHeadPic } then Exit;

  if (m_NumberLableImageInfo.Width > 0) and (CurrentFont <> nil) then begin
    m_ShowNumberLableTimeTick := MyGetTickCount;
    if m_boCanDraw then begin
      if (m_btRace in [RC_PLAYOBJECT, RC_HEROOBJECT]) then begin
        HpBarOffsetX := g_ClientConfig.nHumHPBarOffsetX;
        HpBarOffsetY := g_ClientConfig.nHumHPBarOffsetY;
      end
      else if (m_btRace = RC_MERCHANT) then begin
        HpBarOffsetX := g_ClientConfig.nNpcHPBarOffsetX;
        HpBarOffsetY := g_ClientConfig.nNpcHPBarOffsetY;
      end
      else begin
        HpBarOffsetX := g_ClientConfig.nMonHPBarOffsetX;
        HpBarOffsetY := g_ClientConfig.nMonHPBarOffsetY;
      end;

      oX := 0;
      oY := 0;
      if (m_nChangeAppr < 0) and (Self is TCustomActor) then begin
        BaseConfig := @(Self as TCustomActor).Config.BaseConfig;
        //if BaseConfig.HPStartIndex >= 0 then
        begin
          oX := BaseConfig.HPOffsetX + BaseConfig.HPTextOffsetX;
          oY := BaseConfig.HPOffsetY + BaseConfig.HPTextOffsetY;
        end;
      end;

      nX := m_nSayX - m_NumberLableImageInfo.Width div 2 + oX + HpBarOffsetX;

      if g_NewopUI170TextureArray[0] <> nil then
        nY := m_nSayY - (g_NewopUI170TextureArray[0].Height * 2 + 4) - 12 + oY + HpBarOffsetY
      else
        nY := m_nSayY - 22 + oY + HpBarOffsetY;

      CurrentFont.TextOut(nX - 1, nY, m_NumberLableImageInfo.ImageIndexs, clBlack);
      CurrentFont.TextOut(nX + 1, nY, m_NumberLableImageInfo.ImageIndexs, clBlack);
      CurrentFont.TextOut(nX, nY - 1, m_NumberLableImageInfo.ImageIndexs, clBlack);
      CurrentFont.TextOut(nX, nY + 1, m_NumberLableImageInfo.ImageIndexs, clBlack);
      CurrentFont.TextOut(nX, nY, m_NumberLableImageInfo.ImageIndexs, clWhite);
    end;
    // GameCanvas.DrawColor(0, 0, g_ActorNameTextureImages.Texture.ClientRect, g_ActorNameTextureImages.Texture, clWhite);
  end;
end;

procedure TActor.Say(Str:string);
var
  I, len, aline, n:Integer;
  temp:string;
  loop:Boolean;
  TokenLine:TStringLineEx;
const
  MAXWIDTH = 150;
begin
  m_dwSayTime := TimeGetTime;
  m_nSayLineCount := 0;

  // 去掉人物头顶文字上的标记 chongchong 2015-01-17 02:12:06

  if Pos('{', Str) > 0 then begin
    TokenLine := TStringLineEx.Create;
    try
      GetTextListEx(Str, clNone, clNone, TokenLine);
      Str := GetStrinLineExText(TokenLine);
    finally
      TokenLine.Free;
    end;
  end;

  for I := 0 to Length(m_SayingArr) - 1 do begin
    m_SayingArr[I].Text := '';
    m_SayingArr[I].TextSurface.Width := 0;
    m_SayingArr[I].TextSurface.Height := 0;
    m_SayingArr[I].TextSurface.ImageIndexs := nil;
  end;

  n := 0;
  loop := True;
  while loop do begin
    temp := '';
    I := 1;
    len := Length(Str);
    while True do begin
      if I > len then begin
        loop := False;
        Break;
      end;
      if byte(Str[I]) >= 128 then begin
        temp := temp + Str[I];
        Inc(I);
        if I <= len then
          temp := temp + Str[I]
        else begin
          loop := False;
          Break;
        end;
      end
      else
        temp := temp + Str[I];

      aline := CurrentFont.TextWidth(temp);
      if aline > MAXWIDTH then begin
        m_SayingArr[n].Text := temp;

        m_SayingArr[n].TextSurface.Width := 0;
        m_SayingArr[n].TextSurface.Height := 0;
        m_SayingArr[n].TextSurface.ImageIndexs := nil;
        // m_SayWidthsArr[n] := aline;
        Inc(m_nSayLineCount);
        Inc(n);
        if n >= MAXSAY then begin
          loop := False;
          Break;
        end;
        Str := Copy(Str, I + 1, len - I);
        temp := '';
        Break;
      end;
      Inc(I);
    end;
    if temp <> '' then begin
      if n < MAXWIDTH then begin
        m_SayingArr[n].Text := temp;
        m_SayingArr[n].TextSurface.Width := 0;
        m_SayingArr[n].TextSurface.Height := 0;
        m_SayingArr[n].TextSurface.ImageIndexs := nil;
        // m_SayWidthsArr[n] := CurrentFont.TextWidth(temp);
        Inc(m_nSayLineCount);
      end;
    end;
  end;
end;

{============================== NPCActor =============================}

procedure TNpcActor.CalcActorFrame;
var
  pm:pTMonsterAction;
  NpcConfig:PClientCustomNpcConfig;
  NpcDirAction:PNpcDirAction;
begin

  m_boUseMagic := False;
  m_nCurrentFrame := -1;

  NpcDirAction := nil;

  if m_wAppearance >= 10000 then begin
    m_nBodyOffset := 0;

    {$MESSAGE HINT '下面两行是补全pm的赋值和空值判断，对于 m_WAppearance >= 10000的情况可能不对 需要验证'} //HZQ 20230525
    pm := GetRaceByPM(m_btRace, m_wAppearance); //
    if pm = nil then Exit;

    NpcConfig := GetCustomNpcConfig(m_wAppearance);
    if NpcConfig <> nil then begin
      if NpcConfig.wDirCount > 1 then
        m_btDir := m_btDir mod NpcConfig.wDirCount
      else
        m_btDir := 0;
      NpcDirAction := @NpcConfig.Actions[m_btDir];
    end;
  end else begin
    m_nBodyOffset := GetNpcOffset(m_wAppearance);
    pm := GetRaceByPM(m_btRace, m_wAppearance);
    if pm = nil then Exit;

    if m_wAppearance = 244 then
      m_boUseEffect := True
    else if m_wAppearance = 245 then
      m_boUseEffect := True;

    //if pm = nil then Exit; HZQ move to Front Lines

    // npc4全是8方向 chongchong 2015-04-18
    if (m_wAppearance < 246) or (m_wAppearance > 272) then begin
      m_btDir := m_btDir mod 3; // 规氢篮 0, 1, 2 观俊 绝澜..
      if (m_wAppearance in [54..59, 70..75, 81..84, 90..92, 94..101, 211..225, 245]) then
        m_btDir := 0;
    end;
  end;

  case m_nCurrentAction of
    SM_TURN:begin
        if NpcDirAction <> nil then begin
          m_nStartFrame := NpcDirAction.Std_Index;
          m_nEndFrame := NpcDirAction.Std_Index + NpcDirAction.Std_Count - 1;
          m_dwFrameTime := NpcDirAction.Std_Time;
          m_dwStartTime := TimeGetTime;
          m_StartCounter := timeGetTime;
          m_nDefFrameCount := NpcDirAction.Std_Count;
          Shift(m_btDir, 0, 0, 1);

          {
          DScreen.AddChatBoardString(IntToStr(NpcDirAction.Std_Count) + ' 图片数量.', GetRGB(g_ClientConfig.btGetExpMsgFColor), GetRGB(g_ClientConfig.btGetExpMsgBColor));

          if (NpcDirAction.Std_EffFile >= 0) and (NpcDirAction.Std_EffFile < g_EffectImageList.Count) and
            (NpcDirAction.Std_EffIndex >= 0) and (NpcDirAction.Std_EffCount > 0) and
            (NpcDirAction.Std_EffTime > 0) then
          begin
            m_boUseEffect := True;
            m_nEffectStart := NpcDirAction.Std_EffIndex;
            m_nEffectFrame := NpcDirAction.Std_EffIndex;
            m_nEffectEnd := NpcDirAction.Std_EffIndex + NpcDirAction.Std_EffCount - 1;
            m_dwEffectStartTime := TimeGetTime();
            m_dwEffectFrameTime := NpcDirAction.Std_EffTime;
          end;
          }

          Exit;
        end;

        m_nStartFrame := pm.ActStand.start + m_btDir * (pm.ActStand.frame + pm.ActStand.skip);
        m_nEndFrame := m_nStartFrame + pm.ActStand.frame - 1;
        m_dwFrameTime := pm.ActStand.ftime;
        m_dwStartTime := TimeGetTime;
        m_StartCounter := timeGetTime;
        m_nDefFrameCount := pm.ActStand.frame;
        Shift(m_btDir, 0, 0, 1);
        if ((m_wAppearance = 33) or (m_wAppearance = 34)) then begin
          m_boUseEffect := True;
          m_nEffectStart := 30;
          m_nEffectFrame := 30;
          m_nEffectEnd := 39;
          m_dwEffectStartTime := TimeGetTime();
          m_dwEffectFrameTime := 300;
        end
        else begin
          case m_wAppearance of
            42..47:begin
                m_nStartFrame := 20;
                m_nEndFrame := 10;
                m_boUseEffect := True;
                m_nEffectStart := 0;
                m_nEffectFrame := 0;
                m_nEffectEnd := 19;
                m_dwEffectStartTime := TimeGetTime();
                m_dwEffectFrameTime := 100;
              end;
            51:begin
                m_boUseEffect := True;
                m_nEffectStart := 60;
                m_nEffectFrame := m_nEffectStart;
                m_nEffectEnd := m_nEffectStart + 7;
                m_dwEffectStartTime := TimeGetTime();
                m_dwEffectFrameTime := 500;
              end;
            100:begin
                m_boUseEffect := True;
                m_nEffectStart := 10;
                m_nEffectFrame := m_nEffectStart;
                m_nEffectEnd := m_nEffectStart + 11;
                m_dwEffectStartTime := TimeGetTime();
                m_dwEffectFrameTime := 100;
              end;
            217..219:begin
                m_boUseEffect := True;
                m_nEffectStart := 10;
                m_nEffectFrame := m_nEffectStart;
                m_nEffectEnd := m_nEffectStart + 16 - 1;
                m_dwEffectStartTime := TimeGetTime();
                m_dwEffectFrameTime := 150;
              end;
            221:begin
                m_boUseEffect := True;
                m_nEffectStart := 20;
                m_nEffectFrame := m_nEffectStart;
                m_nEffectEnd := m_nEffectStart + 9 - 1;
                m_dwEffectStartTime := TimeGetTime();
                m_dwEffectFrameTime := 250;
              end;
            222:begin
                m_boUseEffect := True;
                m_nEffectStart := 10;
                m_nEffectFrame := m_nEffectStart;
                m_nEffectEnd := m_nEffectStart + 9 - 1;
                m_dwEffectStartTime := TimeGetTime();
                m_dwEffectFrameTime := 250;
              end;
            224:begin
                m_boUseEffect := True;
                m_nEffectStart := 10;
                m_nEffectFrame := m_nEffectStart;
                m_nEffectEnd := m_nEffectStart + 16 - 1;
                m_dwEffectStartTime := TimeGetTime();
                m_dwEffectFrameTime := 150;
              end;
          end;

        end;
      end;
    SM_HIT:begin
        if NpcDirAction <> nil then begin
          m_nStartFrame := NpcDirAction.Act_Index;
          m_nEndFrame := NpcDirAction.Act_Index + NpcDirAction.Act_Count - 1;
          m_dwFrameTime := NpcDirAction.Act_Time;
          m_dwStartTime := TimeGetTime;
          m_StartCounter := timeGetTime;
          m_nDefFrameCount := NpcDirAction.Act_Count;
          Shift(m_btDir, 0, 0, 1);

          {
          DScreen.AddChatBoardString(IntToStr(NpcDirAction.Act_Count) + ' 图片数量.', GetRGB(g_ClientConfig.btGetExpMsgFColor), GetRGB(g_ClientConfig.btGetExpMsgBColor));

          if (NpcDirAction.Act_EffFile >= 0) and (NpcDirAction.Act_File < g_EffectImageList.Count) and
            (NpcDirAction.Act_EffIndex >= 0) and (NpcDirAction.Act_EffCount > 0) and
            (NpcDirAction.Act_EffTime > 0) then
          begin
            m_boUseEffect := True;
            m_nEffectStart := NpcDirAction.Act_EffIndex;
            m_nEffectFrame := NpcDirAction.Act_EffIndex;
            m_nEffectEnd := NpcDirAction.Act_EffIndex + NpcDirAction.Act_EffCount - 1;
            m_dwEffectStartTime := TimeGetTime();
            m_dwEffectFrameTime := NpcDirAction.Act_EffTime;
          end;
          }

          Exit;
        end;

        case m_wAppearance of
          33, 34, 52:begin
              m_nStartFrame := pm.ActStand.start + m_btDir * (pm.ActStand.frame + pm.ActStand.skip);
              m_nEndFrame := m_nStartFrame + pm.ActStand.frame - 1;
              m_dwStartTime := TimeGetTime;
              m_StartCounter := timeGetTime;
              m_nDefFrameCount := pm.ActStand.frame;
            end;
          84:begin
              if (m_nStartFrame <= 0) or (m_nStartFrame >= pm.ActCritical.start + m_btDir * (pm.ActCritical.frame + pm.ActCritical.skip)) then begin
                m_nStartFrame := pm.ActAttack.start + m_btDir * (pm.ActAttack.frame + pm.ActAttack.skip);
                m_nEndFrame := m_nStartFrame + pm.ActAttack.frame - 1;
                m_dwFrameTime := pm.ActAttack.ftime;
                m_dwStartTime := TimeGetTime;
                m_StartCounter := timeGetTime;
              end
              else begin
                m_nStartFrame := pm.ActCritical.start + m_btDir * (pm.ActCritical.frame + pm.ActCritical.skip);
                m_nEndFrame := m_nStartFrame + pm.ActCritical.frame - 1;
                m_dwFrameTime := pm.ActCritical.ftime;
                m_dwStartTime := TimeGetTime;
                m_StartCounter := timeGetTime;
              end;
            end;
          210:begin
              // if m_btDir = 4 then begin
              m_nStartFrame := pm.ActAttack.start + (pm.ActAttack.frame + pm.ActAttack.skip);
              m_nEndFrame := m_nStartFrame + pm.ActAttack.frame - 1;
              m_dwFrameTime := pm.ActAttack.ftime;
              m_dwStartTime := TimeGetTime;
              m_StartCounter := timeGetTime;
              {end else begin
                m_nStartFrame := pm.ActStand.start + m_btDir * (pm.ActStand.frame + pm.ActStand.skip);
                m_nEndFrame := m_nStartFrame + pm.ActStand.frame - 1;
                m_dwStartTime := TimeGetTime;
                m_nDefFrameCount := pm.ActStand.frame;
              end; }
            end;
          else begin
              if (pm.ActAttack.frame = 0) and (m_wAppearance >= 226) and (m_wAppearance <= 272) then begin

              end
              else begin
                m_nStartFrame := pm.ActAttack.start + m_btDir * (pm.ActAttack.frame + pm.ActAttack.skip);
                m_nEndFrame := m_nStartFrame + pm.ActAttack.frame - 1;
                m_dwFrameTime := pm.ActAttack.ftime;
                m_dwStartTime := TimeGetTime;
                m_StartCounter := timeGetTime;
                if m_wAppearance = 51 then begin
                  m_boUseEffect := True;
                  m_nEffectStart := 60;
                  m_nEffectFrame := m_nEffectStart;
                  m_nEffectEnd := m_nEffectStart + 7;
                  m_dwEffectStartTime := TimeGetTime();
                  m_dwEffectFrameTime := 500;
                end;
              end;
            end;
        end;
      end;
    SM_DIGUP:begin

        if m_wAppearance = 52 then begin
          m_bo248 := True;
          m_dwUseEffectTick := TimeGetTime + 23000;
          Randomize;
          g_PlaySound.PlaySound(Random(7) + 146);
          m_boUseEffect := True;
          m_nEffectStart := 60;
          m_nEffectFrame := m_nEffectStart;
          m_nEffectEnd := m_nEffectStart + 11;
          m_dwEffectStartTime := TimeGetTime();
          m_dwEffectFrameTime := 100;
        end;
      end;
    SM_WALK:begin
        if NpcDirAction <> nil then begin
          m_nStartFrame := NpcDirAction.Act_Index;
          m_nEndFrame := NpcDirAction.Act_Index + NpcDirAction.Act_Count - 1;
          m_dwFrameTime := NpcDirAction.Act_Time;
          m_dwStartTime := TimeGetTime;
          m_StartCounter := timeGetTime;
          m_nDefFrameCount := NpcDirAction.Act_Count;

          {
          DScreen.AddChatBoardString(IntToStr(NpcDirAction.Act_Count) + ' 图片数量.', GetRGB(g_ClientConfig.btGetExpMsgFColor), GetRGB(g_ClientConfig.btGetExpMsgBColor));

          if (NpcDirAction.Act_EffFile >= 0) and (NpcDirAction.Act_EffFile < g_EffectImageList.Count) and
            (NpcDirAction.Act_EffIndex >= 0) and (NpcDirAction.Act_EffCount > 0) and
            (NpcDirAction.Act_EffTime > 0) then
          begin
            m_boUseEffect := True;
            m_nEffectStart := NpcDirAction.Act_EffIndex;
            m_nEffectFrame := NpcDirAction.Act_EffIndex;
            m_nEffectEnd := NpcDirAction.Act_EffIndex + NpcDirAction.Act_EffCount - 1;
            m_dwEffectStartTime := TimeGetTime();
            m_dwEffectFrameTime := NpcDirAction.Act_EffTime;
          end;
          }

          m_nMaxTick := 1;
          m_nCurTick := 0;
          m_nMoveStep := 1;
          Shift(m_btDir, m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1);
          Exit;
        end;
      end;
  end;
end;

constructor TNpcActor.Create; // 0x0047C42C
var
  NpcConfig:PClientCustomNpcConfig;
begin
  inherited;
  m_EffSurface := nil;
  m_KeepSurface := nil;
  m_boHitEffect := False;
  m_bo248 := False;

  if m_wAppearance >= 10000 then begin
    NpcConfig := GetCustomNpcConfig(m_wAppearance);
    if NpcConfig <> nil then begin
      if (NpcConfig.BaseConfig.KeepPlayFile >= 0) and (NpcConfig.BaseConfig.KeepPlayFile < g_EffectImageList.Count) and
        (NpcConfig.BaseConfig.KeepPlayIndex >= 0) and (NpcConfig.BaseConfig.KeepPlayCount > 0) and
        (NpcConfig.BaseConfig.KeepPlayTime > 0) then begin
        m_nKeepFrame := NpcConfig.BaseConfig.KeepPlayIndex;
        m_LastKeepPlayTick := TimeGetTime;
      end;
    end;
  end;
end;

procedure TNpcActor.Initialize;
begin
  inherited Initialize;
end;

procedure TNpcActor.Finalize;
begin
  inherited Finalize;
  m_EffSurface := nil;
  m_KeepSurface := nil;
end;

function TNpcActor.CheckLoadUserName:Boolean;
begin
  Result := False;
  if (not GameCanvas.Active) or (not GameCanvas.Initialized) then Exit;
  m_sNameText := '';
  if PlugInEnabled then begin
    if (CompareText(m_sUserName, '不显名') <> 0) then begin
      if not ((g_ClientConfig.boHideGhost and g_ConfigDlg.ConfigCheckeds[ckHideGhost]) and m_boDeath) then begin
        //if g_ClientConfig.boShowUserName and g_ConfigDlg.ConfigCheckeds[ckShowUserName] and (not m_boDeath) then
        // 是否显示NPC名 piaoyun 2013-07-31
        if g_ClientConfig.boShowNpcName and g_ConfigDlg.ConfigCheckeds[ckShowNpcName] and (not m_boDeath) then begin
          m_sNameText := m_sDescUserName + '\' + m_sUserName;
        end
        else if (g_FocusCret = Self) then begin
          m_sNameText := m_sDescUserName + '\' + m_sUserName;
        end;
      end;
    end;
  end
  else begin
    if (CompareText(m_sUserName, '不显名') <> 0) then begin
      if (g_FocusCret = Self) then begin
        m_sNameText := m_sDescUserName + '\' + m_sUserName;
      end;
    end;
  end;

  // 尝试修复npc乱码，chongchong 2016-02-23 原来与 NameTimeTick 相关的是 30s，现改为 3s NameTimeTick > 1000 * 3
  Result := (CompareText(m_sCurNameText, m_sNameText) <> 0) { or (MyGetTickCount - m_dwShowNameTimeTick > 1000 * 2)};
end;

procedure TNpcActor.DrawChr(dx, dy:Integer; blend, boFlag:Boolean);
var
  NpcConfig:PClientCustomNpcConfig;

  procedure DrawEffect;
  begin
    if m_EffSurface <> nil then begin
      if m_nCurrentAction in [SM_HIT, SM_WALK] then begin
        if NpcConfig.BaseConfig.ActionEffectDrawMode = mdmNormal then
          GameCanvas.Draw(dx + m_nEffX + m_nShiftX, dy + m_nEffY + m_nShiftY, m_EffSurface)
        else
          GameCanvas.DrawBlend(dx + m_nEffX + m_nShiftX, dy + m_nEffY + m_nShiftY, m_EffSurface);
      end
      else begin
        if NpcConfig.BaseConfig.StandEffectDrawMode = mdmNormal then
          GameCanvas.Draw(dx + m_nEffX + m_nShiftX, dy + m_nEffY + m_nShiftY, m_EffSurface)
        else
          GameCanvas.DrawBlend(dx + m_nEffX + m_nShiftX, dy + m_nEffY + m_nShiftY, m_EffSurface);
      end;
    end;
  end;

  procedure DrawKeep;
  begin
    if m_KeepSurface <> nil then begin
      if not NpcConfig.BaseConfig.KeepPlayBlendDraw then
        GameCanvas.Draw(dx + m_nKeepX + m_nShiftX + NpcConfig.BaseConfig.KeepPlayOffsetX, dy + m_nKeepY + m_nShiftY + NpcConfig.BaseConfig.KeepPlayOffsetY, m_KeepSurface)
      else
        GameCanvas.DrawBlend(dx + m_nKeepX + m_nShiftX + NpcConfig.BaseConfig.KeepPlayOffsetX, dy + m_nKeepY + m_nShiftY + NpcConfig.BaseConfig.KeepPlayOffsetY, m_KeepSurface);
    end;
  end;

  procedure DrawBody;
  begin
    if m_BodySurface <> nil then begin
      if m_nCurrentAction in [SM_HIT, SM_WALK] then begin
        if NpcConfig.BaseConfig.ActionDrawMode = mdmNormal then
          DrawEffSurface(m_BodySurface, dx + m_nPx + m_nShiftX, dy + m_nPy + m_nShiftY, blend, m_ColorEffect)
        else
          DrawEffSurface(m_BodySurface, dx + m_nPx + m_nShiftX, dy + m_nPy + m_nShiftY, True, m_ColorEffect);

        DrawStateEffSurface(dx + m_nShiftX, dy + m_nShiftY);
      end
      else begin
        if NpcConfig.BaseConfig.StandDrawMode = mdmNormal then
          DrawEffSurface(m_BodySurface, dx + m_nPx + m_nShiftX, dy + m_nPy + m_nShiftY, blend, m_ColorEffect)
        else
          DrawEffSurface(m_BodySurface, dx + m_nPx + m_nShiftX, dy + m_nPy + m_nShiftY, True, m_ColorEffect);

        DrawStateEffSurface(dx + m_nShiftX, dy + m_nShiftY);
      end;
    end;
  end;
begin
  if m_wAppearance >= 10000 then begin
    NpcConfig := GetCustomNpcConfig(m_wAppearance);
    if NpcConfig <> nil then begin
      case NpcConfig.BaseConfig.DrawOrder of
        ndoKeep_Chr_Eff:begin
            DrawEffect;
            DrawBody;
            DrawKeep;
          end;
        ndoKeep_Eff_Chr:begin
            DrawBody;
            DrawEffect;
            DrawKeep;
          end;
        ndoChr_Keep_Eff:begin
            DrawEffect;
            DrawKeep;
            DrawBody;
          end;
        ndoChr_Eff_Keep:begin
            DrawKeep;
            DrawEffect;
            DrawBody;
          end;
        ndoEff_Keep_Chr:begin
            DrawBody;
            DrawKeep;
            DrawEffect;
          end;
        ndoEff_Chr_Keep:begin
            DrawKeep;
            DrawBody;
            DrawEffect;
          end;
      end;
    end;
  end
  else begin
    if ((m_wAppearance < 246) or (m_wAppearance > 272)) then
      m_btDir := m_btDir mod 3;
    if m_BodySurface <> nil then begin
      if m_wAppearance in [54..58, 94..98] then begin
        GameCanvas.DrawBlend(
          dx + m_nPx + m_nShiftX,
          dy + m_nPy + m_nShiftY,
          m_BodySurface);
      end
      else if m_wAppearance = 51 then begin
        DrawEffSurface(
          m_BodySurface,
          dx + m_nPx + m_nShiftX,
          dy + m_nPy + m_nShiftY,
          True,
          m_ColorEffect);
      end
      else begin
        DrawEffSurface(
          m_BodySurface,
          dx + m_nPx + m_nShiftX,
          dy + m_nPy + m_nShiftY,
          blend,
          m_ColorEffect);
      end;
    end;
    if (m_wAppearance in [64..68, 70..75, 84, 90, 91, 100, 101, 209, 217..219, 221, 222, 224]) and (m_EffSurface <> nil) then begin
      GameCanvas.DrawBlend(
        dx + m_nEffX + m_nShiftX,
        dy + m_nEffY + m_nShiftY,
        m_EffSurface);
    end;
  end;
end;

procedure TNpcActor.DrawEff(dx, dy:Integer);
begin
  // inherited;
  if m_boUseEffect and (m_EffSurface <> nil) then begin
    GameCanvas.DrawBlend(
      dx + m_nEffX + m_nShiftX,
      dy + m_nEffY + m_nShiftY,
      m_EffSurface);
  end;
end;

function TNpcActor.GetDefaultFrame(wmode:Boolean):Integer;
var
  cf:Integer;
  pm:pTMonsterAction;
  NpcConfig:PClientCustomNpcConfig;
begin
  Result := 0; // Jacky
  if m_wAppearance >= 10000 then begin
    NpcConfig := GetCustomNpcConfig(m_wAppearance);
    if NpcConfig <> nil then begin
      if NpcConfig.wDirCount > 1 then
        m_btDir := m_btDir mod NpcConfig.wDirCount
      else
        m_btDir := 0;

      if m_nCurrentDefFrame < 0 then
        cf := 0
      else if m_nCurrentDefFrame >= NpcConfig.Actions[m_btDir].Std_Count then
        cf := 0
      else
        cf := m_nCurrentDefFrame;

      Result := NpcConfig.Actions[m_btDir].Std_Index + cf;
      m_dwFrameTime := NpcConfig.Actions[m_btDir].Std_Time;
    end;
  end
  else begin
    pm := GetRaceByPM(m_btRace, m_wAppearance);
    if pm = nil then Exit;

    if (m_wAppearance < 246) or (m_wAppearance > 272) then
      m_btDir := m_btDir mod 3; // 规氢篮 0, 1, 2 观俊 绝澜..

    if m_nCurrentDefFrame < 0 then
      cf := 0
    else if m_nCurrentDefFrame >= pm.ActStand.frame then
      cf := 0
    else
      cf := m_nCurrentDefFrame;

    if (m_wAppearance in [54..59, 70..75, 81..84, 90..92, 94..101, 211..225]) then
      m_btDir := 0;

    Result := pm.ActStand.start + m_btDir * (pm.ActStand.frame + pm.ActStand.skip) + cf;
  end;
end;

procedure TNpcActor.LoadSurface(Sender:TObject);
var
  wAppearance:Integer;

  NpcConfig:PClientCustomNpcConfig;
  NpcDirAction:PNpcDirAction;
  GameImages:TGameImages;
  Index:Integer;
begin
  m_dwLoadSurfaceTime := MyGetTickCount;
  m_boLoadSurface := False;
  m_BodySurface := nil;
  // m_BodyAlphaSurface := nil;
  m_EffSurface := nil;
  m_KeepSurface := nil;

  NpcDirAction := nil;

  if m_wAppearance >= 10000 then begin
    m_nBodyOffset := 0;

    NpcConfig := GetCustomNpcConfig(m_wAppearance);
    if NpcConfig <> nil then begin
      if NpcConfig.wDirCount > 1 then
        m_btDir := m_btDir mod NpcConfig.wDirCount
      else
        m_btDir := 0;
      NpcDirAction := @NpcConfig.Actions[m_btDir];
    end;

    if NpcDirAction <> nil then begin
      if (NpcConfig.BaseConfig.KeepPlayFile >= 0) and (NpcConfig.BaseConfig.KeepPlayFile < g_EffectImageList.Count) and
        (NpcConfig.BaseConfig.KeepPlayIndex >= 0) and (NpcConfig.BaseConfig.KeepPlayCount > 0) and
        (NpcConfig.BaseConfig.KeepPlayTime > 0) then begin
        GameImages := TGameImages(g_EffectImageList.Objects[NpcConfig.BaseConfig.KeepPlayFile]);
        case m_ColorEffect of
          ceGrayScale:m_KeepSurface := GameImages.GetCachedGrayImage(m_nKeepFrame, m_nKeepX, m_nKeepY);
          ceBright:m_KeepSurface := GameImages.GetCachedBrightImage(m_nKeepFrame, m_nKeepX, m_nKeepY);
          else
            m_KeepSurface := GameImages.GetCachedImage(m_nKeepFrame, m_nKeepX, m_nKeepY);
        end;
      end;

      if m_nCurrentAction in [SM_HIT, SM_WALK] then begin
        if {(NpcDirAction.Act_File >= 0) and }(NpcDirAction.Act_File < g_EffectImageList.Count) //HZQ Act_File:WORD
        and (NpcDirAction.Act_Index >= 0) and (NpcDirAction.Act_Count > 0) and (NpcDirAction.Act_Time > 0) then begin
          GameImages := TGameImages(g_EffectImageList.Objects[NpcDirAction.Act_File]);
          case m_ColorEffect of
            ceGrayScale, ceGrayScale2:m_BodySurface := GameImages.GetCachedGrayImage(m_nCurrentFrame, m_nPx, m_nPy);
            ceBright:m_BodySurface := GameImages.GetCachedBrightImage(m_nCurrentFrame, m_nPx, m_nPy);
            else
              m_BodySurface := GameImages.GetCachedImage(m_nCurrentFrame, m_nPx, m_nPy);
          end;
        end;

        if {(NpcDirAction.Act_EffFile >= 0) and}(NpcDirAction.Act_EffFile < g_EffectImageList.Count) //HZQ Act_EffFile:WORD
        and (NpcDirAction.Act_EffIndex >= 0) and (NpcDirAction.Act_Count > 0) and (NpcDirAction.Act_Time > 0) then begin
          GameImages := TGameImages(g_EffectImageList.Objects[NpcDirAction.Act_EffFile]);
          Index := NpcDirAction.Act_EffIndex + m_nCurrentFrame - NpcDirAction.Act_Index;

          case m_ColorEffect of
            ceGrayScale:m_EffSurface := GameImages.GetCachedGrayImage(Index, m_nEffX, m_nEffY);
            ceBright:m_EffSurface := GameImages.GetCachedBrightImage(Index, m_nEffX, m_nEffY);
            else
              m_EffSurface := GameImages.GetCachedImage(Index, m_nEffX, m_nEffY);
          end;
        end;
      end else begin
        if {(NpcDirAction.Std_File >= 0) and}(NpcDirAction.Std_File < g_EffectImageList.Count) //HZQ 20230525 Std_File:WORD
        and (NpcDirAction.Std_Index >= 0) and (NpcDirAction.Std_Count > 0) and (NpcDirAction.Std_Time > 0) then begin
          GameImages := TGameImages(g_EffectImageList.Objects[NpcDirAction.Std_File]);
          case m_ColorEffect of
            ceGrayScale, ceGrayScale2:m_BodySurface := GameImages.GetCachedGrayImage(m_nCurrentFrame, m_nPx, m_nPy);
            ceBright:m_BodySurface := GameImages.GetCachedBrightImage(m_nCurrentFrame, m_nPx, m_nPy);
            else
              m_BodySurface := GameImages.GetCachedImage(m_nCurrentFrame, m_nPx, m_nPy);
          end;
        end;

        if {(NpcDirAction.Std_EffFile >= 0) and}(NpcDirAction.Std_EffFile < g_EffectImageList.Count) //HZQ Std_EffFile:WORD
        and (NpcDirAction.Std_EffIndex >= 0) and (NpcDirAction.Std_Count > 0) and (NpcDirAction.Std_Time > 0) then begin
          GameImages := TGameImages(g_EffectImageList.Objects[NpcDirAction.Std_EffFile]);
          Index := NpcDirAction.Std_EffIndex + m_nCurrentFrame - NpcDirAction.Std_Index;

          case m_ColorEffect of
            ceGrayScale:m_EffSurface := GameImages.GetCachedGrayImage(Index, m_nEffX, m_nEffY);
            ceBright:m_EffSurface := GameImages.GetCachedBrightImage(Index, m_nEffX, m_nEffY);
            else
              m_EffSurface := GameImages.GetCachedImage(Index, m_nEffX, m_nEffY);
          end;
        end;
      end;
    end;

    LoadActorIcons;
    Exit;
  end;

  if m_wAppearance >= 2000 then begin
    wAppearance := m_wAppearance - 2000;
    if m_btRace = 50 then begin
      if wAppearance < 200 then begin
        case m_ColorEffect of
          ceGrayScale, ceGrayScale2:m_BodySurface := g_WNpcImgImages.Indexs[10].GetCachedGrayImage(m_nBodyOffset + m_nCurrentFrame, m_nPx, m_nPy);
          ceBright:m_BodySurface := g_WNpcImgImages.Indexs[10].GetCachedBrightImage(m_nBodyOffset + m_nCurrentFrame, m_nPx, m_nPy);
          else
            m_BodySurface := g_WNpcImgImages.Indexs[10].GetCachedImage(m_nBodyOffset + m_nCurrentFrame, m_nPx, m_nPy);
        end;
      end
      else begin
        case m_ColorEffect of
          ceGrayScale, ceGrayScale2:m_BodySurface := g_WNpcImgImages.Indexs[11].GetCachedGrayImage(m_nBodyOffset + m_nCurrentFrame, m_nPx, m_nPy);
          ceBright:m_BodySurface := g_WNpcImgImages.Indexs[11].GetCachedBrightImage(m_nBodyOffset + m_nCurrentFrame, m_nPx, m_nPy);
          else
            m_BodySurface := g_WNpcImgImages.Indexs[11].GetCachedImage(m_nBodyOffset + m_nCurrentFrame, m_nPx, m_nPy);
        end;
      end;

      if (wAppearance in [64..67]) then begin
        case m_ColorEffect of
          ceGrayScale:m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedGrayImage(3540 + m_nCurrentFrame, m_nEffX, m_nEffY);
          ceBright:m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedBrightImage(3540 + m_nCurrentFrame, m_nEffX, m_nEffY);
          else
            m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedImage(3540 + m_nCurrentFrame, m_nEffX, m_nEffY);
        end;
      end
      else if wAppearance = 68 then begin
        case m_ColorEffect of
          ceGrayScale:m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedGrayImage(3660 + m_nCurrentFrame, m_nEffX, m_nEffY);
          ceBright:m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedBrightImage(3660 + m_nCurrentFrame, m_nEffX, m_nEffY);
          else
            m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedImage(3660 + m_nCurrentFrame, m_nEffX, m_nEffY);
        end;
      end
      else if wAppearance in [70..75] then begin
        case m_ColorEffect of
          ceGrayScale:m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedGrayImage(m_nBodyOffset + 4 + m_nCurrentFrame, m_nEffX, m_nEffY);
          ceBright:m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedBrightImage(m_nBodyOffset + 4 + m_nCurrentFrame, m_nEffX, m_nEffY);
          else
            m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedImage(m_nBodyOffset + 4 + m_nCurrentFrame, m_nEffX, m_nEffY);
        end;
      end
      else if wAppearance = 84 then begin
        if m_nStartFrame >= 22 then begin
          if m_nCurrentFrame <= 4 then begin
            case m_ColorEffect of
              ceGrayScale:m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedGrayImage(m_nBodyOffset - m_nCurrentFrame, m_nEffX, m_nEffY);
              ceBright:m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedBrightImage(m_nBodyOffset - m_nCurrentFrame, m_nEffX, m_nEffY);
              else
                m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedImage(m_nBodyOffset - m_nCurrentFrame, m_nEffX, m_nEffY);
            end;
          end
          else
            m_EffSurface := nil;
        end
        else if m_nStartFrame >= 4 then begin
          if m_nCurrentFrame >= 4 then begin
            case m_ColorEffect of
              ceGrayScale:m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedGrayImage(m_nBodyOffset + m_nCurrentFrame + 12, m_nEffX, m_nEffY);
              ceBright:m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedBrightImage(m_nBodyOffset + m_nCurrentFrame + 12, m_nEffX, m_nEffY);
              else
                m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedImage(m_nBodyOffset + m_nCurrentFrame + 12, m_nEffX, m_nEffY);
            end;
          end
          else
            m_EffSurface := nil;
        end
        else begin
          m_EffSurface := nil;
        end;
      end
      else if wAppearance in [90, 91] then begin
        case m_ColorEffect of
          ceGrayScale:m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedGrayImage(m_nBodyOffset + 4 + m_nCurrentFrame, m_nEffX, m_nEffY);
          ceBright:m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedBrightImage(m_nBodyOffset + 4 + m_nCurrentFrame, m_nEffX, m_nEffY);
          else
            m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedImage(m_nBodyOffset + 4 + m_nCurrentFrame, m_nEffX, m_nEffY);
        end;
      end
      else if wAppearance = 101 then begin
        case m_ColorEffect of
          ceGrayScale:m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedGrayImage(m_nBodyOffset + 20 + m_nCurrentFrame, m_nEffX, m_nEffY);
          ceBright:m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedBrightImage(m_nBodyOffset + 20 + m_nCurrentFrame, m_nEffX, m_nEffY);
          else
            m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedImage(m_nBodyOffset + 20 + m_nCurrentFrame, m_nEffX, m_nEffY);
        end;
      end
      else if wAppearance = 209 then begin
        case m_ColorEffect of
          ceGrayScale:m_EffSurface := g_WNpcImgImages.Indexs[11].GetCachedGrayImage(m_nBodyOffset + 4 + m_nCurrentFrame, m_nEffX, m_nEffY);
          ceBright:m_EffSurface := g_WNpcImgImages.Indexs[11].GetCachedBrightImage(m_nBodyOffset + 4 + m_nCurrentFrame, m_nEffX, m_nEffY);
          else
            m_EffSurface := g_WNpcImgImages.Indexs[11].GetCachedImage(m_nBodyOffset + 4 + m_nCurrentFrame, m_nEffX, m_nEffY);
        end;
      end;
    end;

    if wAppearance in [42..47] then
      m_BodySurface := nil;
    if m_boUseEffect then begin
      if wAppearance in [33..34] then begin
        case m_ColorEffect of
          ceGrayScale:m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedGrayImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
          ceBright:m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedBrightImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
          else
            m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
        end;
      end
      else if wAppearance = 42 then begin
        case m_ColorEffect of
          ceGrayScale:m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedGrayImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
          ceBright:m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedBrightImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
          else
            m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
        end;
        // m_nEffX := m_nEffX + 71;
        // m_nEffY := m_nEffY + 5;
      end
      else if wAppearance = 43 then begin
        case m_ColorEffect of
          ceGrayScale:m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedGrayImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
          ceBright:m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedBrightImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
          else
            m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
        end;
        // m_nEffX := m_nEffX + 71;
        // m_nEffY := m_nEffY + 37;
      end
      else if wAppearance = 44 then begin
        case m_ColorEffect of
          ceGrayScale:m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedGrayImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
          ceBright:m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedBrightImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
          else
            m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
        end;
        m_nEffX := m_nEffX + 7;
        m_nEffY := m_nEffY + 12;
      end
      else if wAppearance = 45 then begin
        case m_ColorEffect of
          ceGrayScale:m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedGrayImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
          ceBright:m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedBrightImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
          else
            m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
        end;
        m_nEffX := m_nEffX + 6;
        m_nEffY := m_nEffY + 12;
      end
      else if wAppearance = 46 then begin
        case m_ColorEffect of
          ceGrayScale:m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedGrayImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
          ceBright:m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedBrightImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
          else
            m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
        end;
        m_nEffX := m_nEffX + 7;
        m_nEffY := m_nEffY + 12;
      end
      else if wAppearance = 47 then begin
        case m_ColorEffect of
          ceGrayScale:m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedGrayImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
          ceBright:m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedBrightImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
          else
            m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
        end;
        m_nEffX := m_nEffX + 8;
        m_nEffY := m_nEffY + 12;
      end
      else if wAppearance = 51 then begin
        case m_ColorEffect of
          ceGrayScale:m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedGrayImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
          ceBright:m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedBrightImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
          else
            m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
        end;
      end
      else if wAppearance = 52 then begin
        case m_ColorEffect of
          ceGrayScale:m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedGrayImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
          ceBright:m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedBrightImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
          else
            m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
        end;
      end
      else if wAppearance = 100 then begin
        case m_ColorEffect of
          ceGrayScale:m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedGrayImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
          ceBright:m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedBrightImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
          else
            m_EffSurface := g_WNpcImgImages.Indexs[10].GetCachedImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
        end;
      end;
    end;
    // 顶戴花翎
    LoadActorIcons;
    Exit;
  end;
  // ------------------------------------------------------------------------------
  if m_btRace = 50 then begin
    if m_wAppearance < 200 then begin
      case m_ColorEffect of
        ceGrayScale, ceGrayScale2:m_BodySurface := g_WNpcImgImages.Indexs[0].GetCachedGrayImage(m_nBodyOffset + m_nCurrentFrame, m_nPx, m_nPy);
        ceBright:m_BodySurface := g_WNpcImgImages.Indexs[0].GetCachedBrightImage(m_nBodyOffset + m_nCurrentFrame, m_nPx, m_nPy);
        else
          m_BodySurface := g_WNpcImgImages.Indexs[0].GetCachedImage(m_nBodyOffset + m_nCurrentFrame, m_nPx, m_nPy);
      end;
    end
    else if (m_wAppearance >= 226) and (m_wAppearance <= 245) then begin
      case m_ColorEffect of
        ceGrayScale, ceGrayScale2:m_BodySurface := g_WNpcImgImages.Indexs[2].GetCachedGrayImage(m_nBodyOffset + m_nCurrentFrame, m_nPx, m_nPy);
        ceBright:m_BodySurface := g_WNpcImgImages.Indexs[2].GetCachedBrightImage(m_nBodyOffset + m_nCurrentFrame, m_nPx, m_nPy);
        else
          m_BodySurface := g_WNpcImgImages.Indexs[2].GetCachedImage(m_nBodyOffset + m_nCurrentFrame, m_nPx, m_nPy);

          // 修正npc3.wzl中有些资源错误 chongchong 2015-04-20
          if m_nBodyOffset + m_nCurrentFrame = 761 then
            m_nPy := -42
          else if m_nBodyOffset + m_nCurrentFrame = 891 then begin
            m_nPx := 10;
            m_nPy := -45;
          end;
      end;

      if m_wAppearance = 244 then begin
        case m_ColorEffect of
          ceGrayScale:m_EffSurface := g_WNpcImgImages.Indexs[2].GetCachedGrayImage(m_nBodyOffset + m_nCurrentFrame + 30, m_nEffX, m_nEffY);
          ceBright:m_EffSurface := g_WNpcImgImages.Indexs[2].GetCachedBrightImage(m_nBodyOffset + m_nCurrentFrame + 30, m_nEffX, m_nEffY);
          else
            m_EffSurface := g_WNpcImgImages.Indexs[2].GetCachedImage(m_nBodyOffset + m_nCurrentFrame + 30, m_nEffX, m_nEffY);
        end;
      end
      else if m_wAppearance = 245 then begin
        case m_ColorEffect of
          ceGrayScale:m_EffSurface := g_WNpcImgImages.Indexs[2].GetCachedGrayImage(m_nBodyOffset + m_nCurrentFrame + 10, m_nEffX, m_nEffY);
          ceBright:m_EffSurface := g_WNpcImgImages.Indexs[2].GetCachedBrightImage(m_nBodyOffset + m_nCurrentFrame + 10, m_nEffX, m_nEffY);
          else
            m_EffSurface := g_WNpcImgImages.Indexs[2].GetCachedImage(m_nBodyOffset + m_nCurrentFrame + 10, m_nEffX, m_nEffY);
        end;
      end
    end
    else if (m_wAppearance >= 246) and (m_wAppearance <= 272) then begin
      case m_ColorEffect of
        ceGrayScale, ceGrayScale2:m_BodySurface := g_WNpcImgImages.Indexs[3].GetCachedGrayImage(m_nBodyOffset + m_nCurrentFrame, m_nPx, m_nPy);
        ceBright:m_BodySurface := g_WNpcImgImages.Indexs[3].GetCachedBrightImage(m_nBodyOffset + m_nCurrentFrame, m_nPx, m_nPy);
        else
          m_BodySurface := g_WNpcImgImages.Indexs[3].GetCachedImage(m_nBodyOffset + m_nCurrentFrame, m_nPx, m_nPy);
      end;
    end
    else if m_wAppearance >= 1000 then begin
      case m_ColorEffect of
        ceGrayScale, ceGrayScale2:m_BodySurface := g_WNpcImgImages.Indexs[9].GetCachedGrayImage(m_nBodyOffset + m_nCurrentFrame, m_nPx, m_nPy);
        ceBright:m_BodySurface := g_WNpcImgImages.Indexs[9].GetCachedBrightImage(m_nBodyOffset + m_nCurrentFrame, m_nPx, m_nPy);
        else
          m_BodySurface := g_WNpcImgImages.Indexs[9].GetCachedImage(m_nBodyOffset + m_nCurrentFrame, m_nPx, m_nPy);
      end;
    end
    else begin
      case m_ColorEffect of
        ceGrayScale, ceGrayScale2:m_BodySurface := g_WNpcImgImages.Indexs[1].GetCachedGrayImage(m_nBodyOffset + m_nCurrentFrame, m_nPx, m_nPy);
        ceBright:m_BodySurface := g_WNpcImgImages.Indexs[1].GetCachedBrightImage(m_nBodyOffset + m_nCurrentFrame, m_nPx, m_nPy);
        else
          m_BodySurface := g_WNpcImgImages.Indexs[1].GetCachedImage(m_nBodyOffset + m_nCurrentFrame, m_nPx, m_nPy);
      end;
    end;

    if (m_wAppearance in [64..67]) then begin
      case m_ColorEffect of
        ceGrayScale:m_EffSurface := g_WNpcImgImages.Indexs[0].GetCachedGrayImage(3540 + m_nCurrentFrame, m_nEffX, m_nEffY);
        ceBright:m_EffSurface := g_WNpcImgImages.Indexs[0].GetCachedBrightImage(3540 + m_nCurrentFrame, m_nEffX, m_nEffY);
        else
          m_EffSurface := g_WNpcImgImages.Indexs[0].GetCachedImage(3540 + m_nCurrentFrame, m_nEffX, m_nEffY);
      end;
    end
    else if m_wAppearance = 68 then begin
      case m_ColorEffect of
        ceGrayScale:m_EffSurface := g_WNpcImgImages.Indexs[0].GetCachedGrayImage(3660 + m_nCurrentFrame, m_nEffX, m_nEffY);
        ceBright:m_EffSurface := g_WNpcImgImages.Indexs[0].GetCachedBrightImage(3660 + m_nCurrentFrame, m_nEffX, m_nEffY);
        else
          m_EffSurface := g_WNpcImgImages.Indexs[0].GetCachedImage(3660 + m_nCurrentFrame, m_nEffX, m_nEffY);
      end;
    end
    else if m_wAppearance in [70..75] then begin
      case m_ColorEffect of
        ceGrayScale:m_EffSurface := g_WNpcImgImages.Indexs[0].GetCachedGrayImage(m_nBodyOffset + 4 + m_nCurrentFrame, m_nEffX, m_nEffY);
        ceBright:m_EffSurface := g_WNpcImgImages.Indexs[0].GetCachedBrightImage(m_nBodyOffset + 4 + m_nCurrentFrame, m_nEffX, m_nEffY);
        else
          m_EffSurface := g_WNpcImgImages.Indexs[0].GetCachedImage(m_nBodyOffset + 4 + m_nCurrentFrame, m_nEffX, m_nEffY);
      end;
    end
    else if m_wAppearance = 84 then begin
      if m_nStartFrame >= 22 then begin
        if m_nCurrentFrame <= 4 then begin
          case m_ColorEffect of
            ceGrayScale:m_EffSurface := g_WNpcImgImages.Indexs[0].GetCachedGrayImage(m_nBodyOffset - m_nCurrentFrame, m_nEffX, m_nEffY);
            ceBright:m_EffSurface := g_WNpcImgImages.Indexs[0].GetCachedBrightImage(m_nBodyOffset - m_nCurrentFrame, m_nEffX, m_nEffY);
            else
              m_EffSurface := g_WNpcImgImages.Indexs[0].GetCachedImage(m_nBodyOffset - m_nCurrentFrame, m_nEffX, m_nEffY);
          end;
        end
        else
          m_EffSurface := nil;
      end
      else if m_nStartFrame >= 4 then begin
        if m_nCurrentFrame >= 4 then begin
          case m_ColorEffect of
            ceGrayScale:m_EffSurface := g_WNpcImgImages.Indexs[0].GetCachedGrayImage(m_nBodyOffset + m_nCurrentFrame + 12, m_nEffX, m_nEffY);
            ceBright:m_EffSurface := g_WNpcImgImages.Indexs[0].GetCachedBrightImage(m_nBodyOffset + m_nCurrentFrame + 12, m_nEffX, m_nEffY);
            else
              m_EffSurface := g_WNpcImgImages.Indexs[0].GetCachedImage(m_nBodyOffset + m_nCurrentFrame + 12, m_nEffX, m_nEffY);
          end;
        end
        else
          m_EffSurface := nil;
      end
      else begin
        m_EffSurface := nil;
      end;
    end
    else if m_wAppearance in [90, 91] then begin
      case m_ColorEffect of
        ceGrayScale:m_EffSurface := g_WNpcImgImages.Indexs[0].GetCachedGrayImage(m_nBodyOffset + 4 + m_nCurrentFrame, m_nEffX, m_nEffY);
        ceBright:m_EffSurface := g_WNpcImgImages.Indexs[0].GetCachedBrightImage(m_nBodyOffset + 4 + m_nCurrentFrame, m_nEffX, m_nEffY);
        else
          m_EffSurface := g_WNpcImgImages.Indexs[0].GetCachedImage(m_nBodyOffset + 4 + m_nCurrentFrame, m_nEffX, m_nEffY);
      end;
    end
    else if m_wAppearance = 101 then begin
      case m_ColorEffect of
        ceGrayScale:m_EffSurface := g_WNpcImgImages.Indexs[0].GetCachedGrayImage(m_nBodyOffset + 20 + m_nCurrentFrame, m_nEffX, m_nEffY);
        ceBright:m_EffSurface := g_WNpcImgImages.Indexs[0].GetCachedBrightImage(m_nBodyOffset + 20 + m_nCurrentFrame, m_nEffX, m_nEffY);
        else
          m_EffSurface := g_WNpcImgImages.Indexs[0].GetCachedImage(m_nBodyOffset + 20 + m_nCurrentFrame, m_nEffX, m_nEffY);
      end;
    end
    else if m_wAppearance = 209 then begin
      case m_ColorEffect of
        ceGrayScale:m_EffSurface := g_WNpcImgImages.Indexs[1].GetCachedGrayImage(m_nBodyOffset + 4 + m_nCurrentFrame, m_nEffX, m_nEffY);
        ceBright:m_EffSurface := g_WNpcImgImages.Indexs[1].GetCachedBrightImage(m_nBodyOffset + 4 + m_nCurrentFrame, m_nEffX, m_nEffY);
        else
          m_EffSurface := g_WNpcImgImages.Indexs[1].GetCachedImage(m_nBodyOffset + 4 + m_nCurrentFrame, m_nEffX, m_nEffY);
      end;
    end;
  end;

  if m_wAppearance in [42..47] then
    m_BodySurface := nil;
  if m_boUseEffect then begin
    if m_wAppearance in [33..34] then begin
      case m_ColorEffect of
        ceGrayScale:m_EffSurface := g_WNpcImgImages.Indexs[0].GetCachedGrayImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
        ceBright:m_EffSurface := g_WNpcImgImages.Indexs[0].GetCachedBrightImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
        else
          m_EffSurface := g_WNpcImgImages.Indexs[0].GetCachedImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
      end;
    end
    else if m_wAppearance = 42 then begin
      case m_ColorEffect of
        ceGrayScale:m_EffSurface := g_WNpcImgImages.GetCachedGrayImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
        ceBright:m_EffSurface := g_WNpcImgImages.GetCachedBrightImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
        else
          m_EffSurface := g_WNpcImgImages.GetCachedImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
      end;
      // m_nEffX := m_nEffX + 71;
      // m_nEffY := m_nEffY + 5;
    end
    else if m_wAppearance = 43 then begin
      case m_ColorEffect of
        ceGrayScale:m_EffSurface := g_WNpcImgImages.Indexs[0].GetCachedGrayImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
        ceBright:m_EffSurface := g_WNpcImgImages.Indexs[0].GetCachedBrightImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
        else
          m_EffSurface := g_WNpcImgImages.Indexs[0].GetCachedImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
      end;
      // m_nEffX := m_nEffX + 71;
      // m_nEffY := m_nEffY + 37;
    end
    else if m_wAppearance = 44 then begin
      case m_ColorEffect of
        ceGrayScale:m_EffSurface := g_WNpcImgImages.Indexs[0].GetCachedGrayImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
        ceBright:m_EffSurface := g_WNpcImgImages.Indexs[0].GetCachedBrightImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
        else
          m_EffSurface := g_WNpcImgImages.Indexs[0].GetCachedImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
      end;
      m_nEffX := m_nEffX + 7;
      m_nEffY := m_nEffY + 12;
    end
    else if m_wAppearance = 45 then begin
      case m_ColorEffect of
        ceGrayScale:m_EffSurface := g_WNpcImgImages.Indexs[0].GetCachedGrayImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
        ceBright:m_EffSurface := g_WNpcImgImages.Indexs[0].GetCachedBrightImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
        else
          m_EffSurface := g_WNpcImgImages.Indexs[0].GetCachedImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
      end;
      m_nEffX := m_nEffX + 6;
      m_nEffY := m_nEffY + 12;
    end
    else if m_wAppearance = 46 then begin
      case m_ColorEffect of
        ceGrayScale:m_EffSurface := g_WNpcImgImages.Indexs[0].GetCachedGrayImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
        ceBright:m_EffSurface := g_WNpcImgImages.Indexs[0].GetCachedBrightImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
        else
          m_EffSurface := g_WNpcImgImages.Indexs[0].GetCachedImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
      end;
      m_nEffX := m_nEffX + 7;
      m_nEffY := m_nEffY + 12;
    end
    else if m_wAppearance = 47 then begin
      case m_ColorEffect of
        ceGrayScale:m_EffSurface := g_WNpcImgImages.Indexs[0].GetCachedGrayImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
        ceBright:m_EffSurface := g_WNpcImgImages.Indexs[0].GetCachedBrightImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
        else
          m_EffSurface := g_WNpcImgImages.Indexs[0].GetCachedImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
      end;
      m_nEffX := m_nEffX + 8;
      m_nEffY := m_nEffY + 12;
    end
    else if m_wAppearance = 51 then begin
      case m_ColorEffect of
        ceGrayScale:m_EffSurface := g_WNpcImgImages.Indexs[0].GetCachedGrayImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
        ceBright:m_EffSurface := g_WNpcImgImages.Indexs[0].GetCachedBrightImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
        else
          m_EffSurface := g_WNpcImgImages.Indexs[0].GetCachedImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
      end;
    end
    else if m_wAppearance = 52 then begin
      case m_ColorEffect of
        ceGrayScale:m_EffSurface := g_WNpcImgImages.Indexs[0].GetCachedGrayImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
        ceBright:m_EffSurface := g_WNpcImgImages.Indexs[0].GetCachedBrightImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
        else
          m_EffSurface := g_WNpcImgImages.Indexs[0].GetCachedImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
      end;
    end
    else if m_wAppearance = 100 then begin
      case m_ColorEffect of
        ceGrayScale:m_EffSurface := g_WNpcImgImages.Indexs[0].GetCachedGrayImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
        ceBright:m_EffSurface := g_WNpcImgImages.Indexs[0].GetCachedBrightImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
        else
          m_EffSurface := g_WNpcImgImages.Indexs[0].GetCachedImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
      end; // if m_wAppearance in [217..219, 221, 222, 224] then begin
    end
    else if m_wAppearance in [217..219] then begin
      case m_ColorEffect of
        ceGrayScale:m_EffSurface := g_WNpcImgImages.Indexs[1].GetCachedGrayImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
        ceBright:m_EffSurface := g_WNpcImgImages.Indexs[1].GetCachedBrightImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
        else
          m_EffSurface := g_WNpcImgImages.Indexs[1].GetCachedImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
      end;
    end
    else if m_wAppearance = 221 then begin
      case m_ColorEffect of
        ceGrayScale:m_EffSurface := g_WNpcImgImages.Indexs[1].GetCachedGrayImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
        ceBright:m_EffSurface := g_WNpcImgImages.Indexs[1].GetCachedBrightImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
        else
          m_EffSurface := g_WNpcImgImages.Indexs[1].GetCachedImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
      end;
    end
    else if m_wAppearance = 222 then begin
      case m_ColorEffect of
        ceGrayScale:m_EffSurface := g_WNpcImgImages.Indexs[1].GetCachedGrayImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
        ceBright:m_EffSurface := g_WNpcImgImages.Indexs[1].GetCachedBrightImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
        else
          m_EffSurface := g_WNpcImgImages.Indexs[1].GetCachedImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
      end;
    end
    else if m_wAppearance = 224 then begin
      case m_ColorEffect of
        ceGrayScale:m_EffSurface := g_WNpcImgImages.Indexs[1].GetCachedGrayImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
        ceBright:m_EffSurface := g_WNpcImgImages.Indexs[1].GetCachedBrightImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
        else
          m_EffSurface := g_WNpcImgImages.Indexs[1].GetCachedImage(m_nBodyOffset + m_nEffectFrame, m_nEffX, m_nEffY);
      end;
    end;

  end;

  // 顶戴花翎
  LoadActorIcons;
end;

procedure TNpcActor.Run;
var
  nEffectFrame, nKeepFrame:Integer;
  dwEffectFrameTime:longword;

  NpcConfig:PClientCustomNpcConfig;
begin
  inherited Run;
  nEffectFrame := m_nEffectFrame;
  nKeepFrame := m_nKeepFrame;
  if m_boUseEffect then begin
    if m_boUseMagic then begin
      dwEffectFrameTime := Round(m_dwEffectFrameTime / 3);
    end
    else
      dwEffectFrameTime := m_dwEffectFrameTime;

    if TimeGetTime - m_dwEffectStartTime > dwEffectFrameTime then begin
      m_dwEffectStartTime := TimeGetTime();
      if m_nEffectFrame < m_nEffectEnd then begin
        Inc(m_nEffectFrame);
      end
      else begin
        if m_bo248 then begin
          if TimeGetTime > m_dwUseEffectTick then begin
            m_boUseEffect := False;
            m_bo248 := False;
            m_dwUseEffectTick := TimeGetTime();
          end;
          m_nEffectFrame := m_nEffectStart;
        end
        else
          m_nEffectFrame := m_nEffectStart;
        m_dwEffectStartTime := TimeGetTime();
      end;
    end;
  end;

  if m_wAppearance >= 10000 then begin
    NpcConfig := GetCustomNpcConfig(m_wAppearance);
    if NpcConfig <> nil then begin
      if (NpcConfig.BaseConfig.KeepPlayFile >= 0) and (NpcConfig.BaseConfig.KeepPlayFile < g_EffectImageList.Count)
        and (NpcConfig.BaseConfig.KeepPlayIndex >= 0) and (NpcConfig.BaseConfig.KeepPlayCount > 0)
        and (NpcConfig.BaseConfig.KeepPlayTime > 0) then begin
        if TimeGetTime - m_LastKeepPlayTick >= Cardinal(NpcConfig.BaseConfig.KeepPlayTime) then begin
          m_nKeepFrame := m_nKeepFrame + 1;
          if m_nKeepFrame < NpcConfig.BaseConfig.KeepPlayIndex then
            m_nKeepFrame := NpcConfig.BaseConfig.KeepPlayIndex
          else if m_nKeepFrame >= NpcConfig.BaseConfig.KeepPlayIndex + NpcConfig.BaseConfig.KeepPlayCount then
            m_nKeepFrame := NpcConfig.BaseConfig.KeepPlayIndex;

          m_LastKeepPlayTick := TimeGetTime;
        end;
      end;
    end;
  end;

  if (nEffectFrame <> m_nEffectFrame) or (nKeepFrame <> m_nKeepFrame) then begin
    m_dwLoadSurfaceTime := MyGetTickCount();
    PlayScene.LoadSurface(LoadSurface);
  end;

end;

{============================== HUMActor =============================}

// 荤恩

{-------------------------------}

constructor THumActor.Create;
begin
  inherited Create;
  m_HairSurface := nil;
  m_WeaponSurface := nil;
  m_ShieldSurface := nil; // 盾牌 chongchong 2013-09-16

  m_HorseSurface := nil; // 骑马 chongchong 2013-10-12
  m_HorseWingsEffectSurface := nil; // 骑马 马特效 chongchong 2013-10-16
  m_HorseEffectSurface := nil;

  m_HorseHairSurface := nil; // 骑马 马上人物的头发 chongchong 2013-10-17
  m_HorseHumSurface := nil; // 骑马 马上人物 chongchong 2013-10-17

  m_HumWinSurface := nil;
  m_HumWinSurface_30 := nil;
  m_boWeaponEffect := False;
  m_dwFrameTime := 150;
  m_dwFrameTick := TimeGetTime();
  m_nFrame := 0;
  m_nHumWinOffset := 0;
  m_nDressEffectIndex := -1;
  m_nWeaponEffectIndex := -1;
  m_wDBWeaponEffectOffSet := 0; // 武器发光效果外观DB 2020-11-11 00:51:03
  m_nShieldEffectIndex := -1;
  m_OnShopStall := nil;

  m_boDressEffectNoBlend := False;
  m_boDressEffectNoSex := False;
  m_boWeaponEffectNoBlend := False;
  m_boWeaponEffectNoSex := False;

  m_nMedalEffectIndex := -1; // 勋章发光效外观wil 编号
  m_boMedalEffectNoBlend := False;
  m_boMedalEffectNoSex := False;

  m_btCboDressUseDiyImage := 0;
  m_btCboWeaponUseDiyImage := 0;

  m_boShieldEffectNoBlend := False;
  m_boShieldEffectNoSex := False;

  m_boDressEffectDrawNoBlend := False;
  m_boWeaponEffectDrawNoBlend := False;
  m_boShieldEffectDrawNoBlend := False;

  m_nDressAddEffectIndex := -1; // 附加衣服特效
  m_bDressAddEffectOrder := 0; // 附加衣服特效绘制顺序
  m_wDressAddEffectOffSet := 0; // 附加衣服特效偏移
  m_wDressAddEffectCount := 0; // 附加衣服特效数量
  m_wDressAddEffectTime := 0; // 附加衣服特效时间
  m_boDressAddEffectNoBlend := False; // 附加衣服特效 - 普通绘制

  m_nDressAddEffectCurIndex := 0;
  m_nDressAddEffectLastTick := MyGetTickCount;

  m_boTrainingNG := False; // 是否学习过内功
  m_boTrainingXF := False; // 是否学习过心法

  m_nJewelryBoxStatus := jbsNoActive; // 首饰盒状态 0:未激活; 1:激活; 2:开启 chongchong 2013-10-19
  m_boShowGodBless := False; // 显示神佑袋 chongchong 2014-04-18

  FillChar(m_AbilNG, SizeOf(TAbilityNG), 0); // 内功属性
  FillChar(m_Alcohol, SizeOf(TAbilityAlcohol), 0); // 酒属性
  FillChar(m_HumMeridians, SizeOf(THumMeridians), 0); // 人物经络

  m_wHeroM2DressEffect := 0; // HeroM2 ChangeDressEffect
  m_boHeroM2DressNoBlend := False;

  m_wOldHeroM2DressEffect := 0;
  m_boOldHeroM2DressNoBlend := False;

  m_nHeroM2DressEffectX := 0;
  m_nHeroM2DressEffectY := 0;
  m_HeroM2DressEffect := nil;

  m_boBrokenShield := False;
  ;

  m_FriendHitList := TStringList.Create;
end;

destructor THumActor.Destroy;
begin
  inherited Destroy;
  m_FriendHitList.Free;
end;

procedure THumActor.Initialize;
begin
  inherited Initialize;
end;

procedure THumActor.Finalize;
var
  I:Integer;
  ActorEffect:pTClientActorEffect;
begin
  inherited Finalize;

  m_HairSurface := nil;
  m_WeaponSurface := nil;
  m_ShieldSurface := nil; // 盾牌 chongchong 2013-09-16

  m_HorseSurface := nil; // 骑马 chongchong 2013-10-12
  m_HorseWingsEffectSurface := nil; // 骑马 马特效 chongchong 2013-10-16
  m_HorseEffectSurface := nil;

  m_HorseHairSurface := nil; // 骑马 马上人物的头发 chongchong 2013-10-17
  m_HorseHumSurface := nil; // 骑马 马上人物 chongchong 2013-10-17

  m_HumWinSurface := nil;
  m_HumWinSurface_30 := nil;

  m_ShopStallSurface := nil;
  m_ShopHeadSurface := nil;
  m_HeroM2DressEffect := nil;

  // 脚本命令播放特效
  m_ActorEffects.Lock;
  try
    for I := 0 to m_ActorEffects.Count - 1 do begin
      ActorEffect := m_ActorEffects.Items[I];
      ActorEffect.Texture := nil;
    end;
  finally
    m_ActorEffects.UnLock;
  end;
end;

procedure THumActor.DrawDressEffect(ddx, ddy:Integer; blend:Boolean; ceff:TColorEffect);
var
  nX, nY:Integer;
begin
  if Assigned(m_HumWinSurface) then begin
    nX := ddx + m_nSpX + m_nShiftX;
    nY := ddy + m_nSpY + m_nShiftY;
    if not m_boEffectNormalDraw then
      GameCanvas.DrawBlend(nX, nY, m_HumWinSurface)
    else
      GameCanvas.Draw(nX, nY, m_HumWinSurface)
  end;

  if Assigned(m_HumWinSurface_30) then begin
    nX := ddx + m_nSpX_30 + m_nShiftX;
    nY := ddy + m_nSpY_30 + m_nShiftY;

    if not m_boEffect_30NormalDraw then
      GameCanvas.DrawBlend(nX, nY, m_HumWinSurface_30)
    else
      GameCanvas.Draw(nX, nY, m_HumWinSurface_30);
  end;

  if Assigned(m_HeroM2DressEffect) then begin
    nX := ddx + m_nHeroM2DressEffectX + m_nShiftX;
    nY := ddy + m_nHeroM2DressEffectY + m_nShiftY;

    if m_boHeroM2DressNoBlend then
      GameCanvas.Draw(nX, nY, m_HeroM2DressEffect)
    else
      GameCanvas.DrawBlend(nX, nY, m_HeroM2DressEffect);
  end;

  if Assigned(m_MedalEffectSurface) then begin
    nX := ddx + m_nMedalEffectX + m_nShiftX;
    nY := ddy + m_nMedalEffectY + m_nShiftY;

    if m_boMedalEffectDrawNoBlend then
      GameCanvas.Draw(nX, nY, m_MedalEffectSurface)
    else
      GameCanvas.DrawBlend(nX, nY, m_MedalEffectSurface);
  end;

  inherited;
end;

{
procedure THumActor.DrawDressEffectEx(ddx, ddy: Integer; blend: Boolean; ceff: TColorEffect);
var
  nX, nY: Integer;
begin
  if Assigned(m_HumWinSurface) then
  begin
    nX := ddx + m_nSpX + m_nShiftX;
    nY := ddy + m_nSpY + m_nShiftY;
    // GameCanvas.DrawBlend(nX, nY, m_HumWinSurface);
    GameCanvas.DrawColorAlpha(nX, nY, m_HumWinSurface, LightColor, LightAlpha, Blend_SrcAlphaColor); // Blend_SrcAlphaColor
  end;

  if Assigned(m_HumWinSurface_30) then
  begin
    nX := ddx + m_nSpX_30 + m_nShiftX;
    nY := ddy + m_nSpY_30 + m_nShiftY;
    // GameCanvas.DrawBlend(nX, nY, m_HumWinSurface_30);
    GameCanvas.DrawColorAlpha(nX, nY, m_HumWinSurface_30, LightColor, LightAlpha, Blend_SrcAlphaColor); // Blend_SrcAlphaColor
  end;

  if Assigned(m_HeroM2DressEffect) then
  begin
    nX := ddx + m_nHeroM2DressEffectX + m_nShiftX;
    nY := ddy + m_nHeroM2DressEffectY + m_nShiftY;
    //GameCanvas.DrawBlend(nX, nY, m_HeroM2DressEffect);
    GameCanvas.DrawColorAlpha(nX, nY, m_HumWinSurface, LightColor, LightAlpha, Blend_SrcAlphaColor);
  end;
  inherited;
end;
}

procedure THumActor.CalcActorFrame;
var
  // haircount: Integer;
  boShopStall:Boolean;
  nSpellSpeed:Integer;
  nMoveSpeed:Integer;
  TempFrameTime:Integer;
  NextHitTime:LongWord;
  meff:TMagicEff;
  nX, nY:Integer;
  nClientNextHitTime:Integer;
  nServerNextHitTime:Integer;

  CustomMagicConfig:PClientCustomMagicConfig;
  MagicPlusLevel:TMagicPlusLevel;
  ClientConfig:PMagicClientConfig;
  I, nDirCount:Integer;

  nDir:Integer;
  MagicEff:TMagicEff;
  //boCustomMagicNoneAction: Boolean;
  MonsterAction:pTMonsterAction;
  MonsterConfig, TempMonsterConfig:PClientCustomMonsterConfig;
  ClientAction:PMonsterClientAction;
  TempDir:Integer;
begin
  m_nChangeAppr := pTHumFeature(@m_Feature.Buffer).nChangeAppr;
  if m_nChangeAppr >= 0 then
    m_wAppearance := m_nChangeAppr
  else
    m_wAppearance := 0;

  if m_nChangeAppr >= 0 then begin
    if m_nChangeAppr >= 100000 then begin
      MonsterConfig := nil;

      for I := 0 to g_CustomMonsterConfig.Count - 1 do begin
        TempMonsterConfig := g_CustomMonsterConfig.Items[I];
        if TempMonsterConfig.wMonsterAppr = m_nChangeAppr - 100000 then begin
          MonsterConfig := TempMonsterConfig;
          Break;
        end;
      end;

      if MonsterConfig <> nil then begin
        m_nCurrentFrame := -1;

        case m_nCurrentAction of
          0, SM_TURN:begin
              if (m_nState and STATE_STONE_MODE) <> 0 then begin
                ClientAction := @MonsterConfig.Actions[matStoneRevive];

                if ClientAction.CalcDir then
                  TempDir := m_btDir
                else
                  TempDir := 0;
                m_nStartFrame := ClientAction.StartIndex + TempDir * (ClientAction.PlayCount + ClientAction.EmptyCount);
                m_nEndFrame := m_nStartFrame;
                m_dwFrameTime := ClientAction.PlayTime;
                m_dwStartTime := TimeGetTime;
                m_StartCounter := timeGetTime;
                m_nDefFrameCount := ClientAction.PlayCount;
              end
              else begin
                ClientAction := @MonsterConfig.Actions[matStand];

                if ClientAction.CalcDir then
                  TempDir := m_btDir
                else
                  TempDir := 0;
                m_nStartFrame := ClientAction.StartIndex + TempDir * (ClientAction.PlayCount + ClientAction.EmptyCount);
                m_nEndFrame := m_nStartFrame + ClientAction.PlayCount - 1;
                m_dwFrameTime := ClientAction.PlayTime;
                m_dwStartTime := TimeGetTime;
                m_StartCounter := timeGetTime;
                m_nDefFrameCount := ClientAction.PlayCount;
              end;
              Shift(m_btDir, 0, 0, 1);
            end;
          SM_WALK, SM_RUSH, SM_RUSHKUNG, SM_BACKSTEP:begin
              ClientAction := @MonsterConfig.Actions[matWalk];

              if ClientAction.CalcDir then
                TempDir := m_btDir
              else
                TempDir := 0;
              m_nStartFrame := ClientAction.StartIndex + TempDir * (ClientAction.PlayCount + ClientAction.EmptyCount);
              m_nEndFrame := m_nStartFrame + ClientAction.PlayCount - 1;
              m_dwFrameTime := ClientAction.PlayTime;
              m_dwStartTime := TimeGetTime;
              m_StartCounter := timeGetTime;
              m_nMaxTick := HA.ActWalk.usetick;
              m_nCurTick := 0;
              m_nMoveStep := 1;

              if m_nCurrentAction = SM_BACKSTEP then begin
                m_nMoveStep := m_btStep;
                Shift(GetBack(m_btDir), m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1)
              end
              else
                Shift(m_btDir, m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1);
            end;
          SM_DIGUP:begin
              ClientAction := @MonsterConfig.Actions[matStoneRevive];

              if ClientAction.CalcDir then
                TempDir := m_btDir
              else
                TempDir := 0;
              m_nStartFrame := ClientAction.StartIndex + TempDir * (ClientAction.PlayCount + ClientAction.EmptyCount);
              m_nEndFrame := m_nStartFrame + ClientAction.PlayCount - 1;
              m_dwFrameTime := ClientAction.PlayTime;
              m_dwStartTime := TimeGetTime;
              m_StartCounter := timeGetTime;
              m_nMaxTick := 0;
              m_nCurTick := 0;
              m_nDefFrameCount := ClientAction.PlayCount;

              m_nState := 0;

              Shift(m_btDir, 0, 0, 1);
            end;
          SM_LIGHTINGEX:begin

            end;
          SM_HIT:begin
              ClientAction := @MonsterConfig.Actions[matDefAttack];

              if ClientAction.CalcDir then
                TempDir := m_btDir
              else
                TempDir := 0;
              m_nStartFrame := ClientAction.StartIndex + TempDir * (ClientAction.PlayCount + ClientAction.EmptyCount);
              m_nEndFrame := m_nStartFrame + ClientAction.PlayCount - 1;
              m_dwFrameTime := ClientAction.PlayTime;
              m_dwStartTime := TimeGetTime;
              m_dwWarModeTime := TimeGetTime;
              Shift(m_btDir, 0, 0, 1);
            end;
          SM_STRUCK:begin
              ClientAction := @MonsterConfig.Actions[matStruck];

              if ClientAction.CalcDir then
                TempDir := m_btDir
              else
                TempDir := 0;
              m_nStartFrame := ClientAction.StartIndex + TempDir * (ClientAction.PlayCount + ClientAction.EmptyCount);
              m_nEndFrame := m_nStartFrame + ClientAction.PlayCount - 1;
              m_dwFrameTime := m_dwStruckFrameTime;
              m_dwStartTime := TimeGetTime;
              Shift(m_btDir, 0, 0, 1);

              m_CustomMagicStatusEffect.m_nStruck := 0;
            end;
          SM_DEATH:begin
              ClientAction := @MonsterConfig.Actions[matDie];

              if ClientAction.CalcDir then
                TempDir := m_btDir
              else
                TempDir := 0;
              m_nStartFrame := ClientAction.StartIndex + TempDir * (ClientAction.PlayCount + ClientAction.EmptyCount) + ClientAction.PlayCount - 1;
              m_nEndFrame := m_nStartFrame;
              m_dwFrameTime := ClientAction.PlayTime;
              m_dwStartTime := TimeGetTime;
            end;
          SM_NOWDEATH:begin
              ClientAction := @MonsterConfig.Actions[matDie];

              if ClientAction.CalcDir then
                TempDir := m_btDir
              else
                TempDir := 0;
              m_nStartFrame := ClientAction.StartIndex + TempDir * (ClientAction.PlayCount + ClientAction.EmptyCount);
              m_nEndFrame := m_nStartFrame + ClientAction.PlayCount - 1;
              m_dwFrameTime := ClientAction.PlayTime;
              m_dwStartTime := TimeGetTime;
            end;
          SM_SKELETON:begin

            end;
        end;
      end;
    end
    else begin
      MonsterAction := GetRaceByPM(m_btRace, m_wAppearance);
      if (m_nCurrentAction = SM_RUN) and (MonsterAction <> nil) then begin
        m_Action := MonsterAction;

        m_nStartFrame := m_Action.ActWalk.start + m_btDir * (m_Action.ActWalk.frame + m_Action.ActWalk.skip);
        m_nEndFrame := m_nStartFrame + m_Action.ActWalk.frame - 1;
        m_dwFrameTime := m_Action.ActWalk.ftime;
        m_dwStartTime := TimeGetTime;
        m_StartCounter := timeGetTime;
        m_nMaxTick := m_Action.ActWalk.usetick;
        m_nCurTick := 0;
        // WarMode := False;
        if m_nCurrentAction = SM_RUN then begin
          // 骑马一步三格 chongchong 2013-10-16
          if (m_btHorse <> 0) and g_ClientConfig.boHorseRun3Grid then
            m_nMoveStep := 3
          else
            m_nMoveStep := 2
        end
        else
          m_nMoveStep := 1;

        Shift(m_btDir, m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1);
      end
      else begin
        inherited CalcActorFrame;
      end;
    end;

    Exit;
  end;

  m_boUseEffect := False;
  m_boUseMagic := False;
  m_boHitEffect := False;
  m_boMagicEndEffect := False;
  m_nCurrentFrame := -1;

  m_btHorse := pTHumFeature(@m_Feature.Buffer).btHorseType;
  m_btDoubleHumHorse := pTHumFeature(@m_Feature.Buffer).btDoubleHumHorseType;
  m_boShowHorseWingsEffect := pTHumFeature(@m_Feature.Buffer).boShowHorseWingsEffect;
  m_btHorseHum := pTHumFeature(@m_Feature.Buffer).btHorseHum;
  m_btHorseHumExpand := pTHumFeature(@m_Feature.Buffer).btHorseHumExpand;
  m_btHorseHair := pTHumFeature(@m_Feature.Buffer).btHorseHair;
  m_btHorseEffectType := pTHumFeature(@m_Feature.Buffer).btHorseEffectType;

  m_boShowFashion := pTHumFeature(@m_Feature.Buffer).boShowFashion;
  m_boMagicShield := pTHumFeature(@m_Feature.Buffer).boMagicShield;
  m_btReLevel := pTHumFeature(@m_Feature.Buffer).btReLevel;

  m_btSex := pTHumFeature(@m_Feature.Buffer).btGender;
  m_btJob := pTHumFeature(@m_Feature.Buffer).btJob;
  m_btHair := pTHumFeature(@m_Feature.Buffer).btHair;
  m_wDress := pTHumFeature(@m_Feature.Buffer).wDress;
  m_wWeapon := pTHumFeature(@m_Feature.Buffer).wWeapon;
  m_wEffect := pTHumFeature(@m_Feature.Buffer).wDressEffType;
  m_wEffect_30 := pTHumFeature(@m_Feature.Buffer).wDressEffType_30;
  m_wShield := pTHumFeature(@m_Feature.Buffer).wShield; // 盾牌 chongchong 2013-09-16

  m_boEffectNormalDraw := pTHumFeature(@m_Feature.Buffer).boDressEffNormalDraw;
  m_boEffect_30NormalDraw := pTHumFeature(@m_Feature.Buffer).boDressEff_30NormalDraw;
  m_boDressEffNoSex := pTHumFeature(@m_Feature.Buffer).boDressEffNoSex;
  m_boDressEff_30NoSex := pTHumFeature(@m_Feature.Buffer).boDressEff_30NoSex;

  m_btOldHair := pTHumFeature(@m_Feature.Buffer).btOldHair;
  m_boPlayMoster := pTHumFeature(@m_Feature.Buffer).boPlayMoster;

  m_btCaseltGuild := pTHumFeature(@m_Feature.Buffer).btCaseltGuild; // 1=沙行会成员 //2=沙行会掌门
  m_nWeaponEffectIndex := pTHumFeature(@m_Feature.Buffer).nWeaponEffectIndex; // 武器发光效果外观wil 编号
  m_wDBWeaponEffectOffSet := pTHumFeature(@m_Feature.Buffer).wDBWeaponEffectOffSet; // 武器发光效果外观DB 2020-11-11 00:51:03
  m_wWeaponEffectOffSet := pTHumFeature(@m_Feature.Buffer).wWeaponEffectOffSet; // 武器发光效外观偏移
  m_nDressEffectIndex := pTHumFeature(@m_Feature.Buffer).nDressEffectIndex; // 衣服发光效外观wil 编号
  m_wDressEffectOffSet := pTHumFeature(@m_Feature.Buffer).wDressEffectOffSet; // 衣服发光效外观偏移
  m_nShieldEffectIndex := pTHumFeature(@m_Feature.Buffer).nShieldEffectIndex; // 武器发光效果外观wil 编号
  m_wShieldEffectOffSet := pTHumFeature(@m_Feature.Buffer).wShieldEffectOffSet; // 武器发光效外观偏移

  m_boDressEffectNoBlend := pTHumFeature(@m_Feature.Buffer).boDressEffectNoBlend;
  m_boDressEffectNoSex := pTHumFeature(@m_Feature.Buffer).boDressEffectNoSex;

  m_nMedalEffectIndex := pTHumFeature(@m_Feature.Buffer).nMedalEffectIndex; // 衣服发光效外观wil 编号
  m_wMedalEffectOffSet := pTHumFeature(@m_Feature.Buffer).wMedalEffectOffSet; // 衣服发光效外观偏移
  m_boMedalEffectNoBlend := pTHumFeature(@m_Feature.Buffer).boMedalEffectNoBlend;
  m_boMedalEffectNoSex := pTHumFeature(@m_Feature.Buffer).boMedalEffectNoSex;

  m_btCboDressUseDiyImage := pTHumFeature(@m_Feature.Buffer).btCboDressUseDiyImage;
  m_btCboWeaponUseDiyImage := pTHumFeature(@m_Feature.Buffer).btCboWeaponUseDiyImage;

  m_boWeaponEffectNoBlend := pTHumFeature(@m_Feature.Buffer).boWeaponEffectNoBlend;
  m_boWeaponEffectNoSex := pTHumFeature(@m_Feature.Buffer).boWeaponEffectNoSex;
  m_boShieldEffectNoBlend := pTHumFeature(@m_Feature.Buffer).boShieldEffectNoBlend;
  m_boShieldEffectNoSex := pTHumFeature(@m_Feature.Buffer).boShieldEffectNoSex;

  m_nDressAddEffectIndex := pTHumFeature(@m_Feature.Buffer).nDressAddEffectIndex;
  m_bDressAddEffectOrder := pTHumFeature(@m_Feature.Buffer).bDressAddEffectOrder;
  m_wDressAddEffectOffSet := pTHumFeature(@m_Feature.Buffer).wDressAddEffectOffSet;
  m_wDressAddEffectCount := pTHumFeature(@m_Feature.Buffer).wDressAddEffectCount;
  m_wDressAddEffectTime := pTHumFeature(@m_Feature.Buffer).wDressAddEffectTime;
  m_boDressAddEffectNoBlend := pTHumFeature(@m_Feature.Buffer).boDressAddEffectNoBlend;
  m_boDressAddEffectDrawCenter := pTHumFeature(@m_Feature.Buffer).boDressAddEffectDrawCenter;

  m_btBodyColor := pTHumFeature(@m_Feature.Buffer).btBodyColor; // 人体颜色

  boShopStall := m_boShopStall;
  m_boShopStall := pTHumFeature(@m_Feature.Buffer).boShopStall;

  // 发型偏移计算2 -- piaoyun 2013-6-10
  case m_btHair of
    0..5:begin
        if (m_btHair < 4) or (m_btHorse > 0) then begin
          case m_btSex of
            0:m_nHairOffset := m_btHair * 2 * HUMANFRAME;
            1:m_nHairOffset := (m_btHair + 2) * HUMANFRAME;
          end;
        end
        else begin
          case m_btHair of // 头盔
            4:m_nHairOffset := 3600;
            5:m_nHairOffset := 4800;
            else
              m_nHairOffset := -1;
          end;
        end;
      end;
    50..59:begin
        m_nHairOffset := (m_btHair - 50) * HUMANFRAME * 2 + m_btSex * HUMANFRAME;
      end;
    60..69:begin
        m_nHairOffset := (m_btHair - 60) * HUMANFRAME * 2 + m_btSex * HUMANFRAME;
      end;
    else {(m_btHair - 6)}
      m_nHairOffset := 3600 + (m_btHair - 100) * HUMANFRAME * 2 + m_btSex * HUMANFRAME;
  end;

  m_nWeaponOffset := HUMANFRAME * m_wWeapon; // (weapon*2 + m_btSex);

  if (m_wEffect = 50) then begin
    m_nHumWinOffset := 352;
  end
  else if m_wEffect <> 0 then
    m_nHumWinOffset := (m_wEffect - 1) * HUMANFRAME;

  if (m_wEffect_30 = 50) then begin
    m_nHumWinOffset_30 := 352;
  end
  else if m_wEffect_30 <> 0 then
    m_nHumWinOffset_30 := (m_wEffect_30 - 1) * HUMANFRAME;

  if (boShopStall <> m_boShopStall) and
    Assigned(m_OnShopStall) then begin
    if m_boShopStall then begin
      m_btDir := pTHumFeature(@m_Feature.Buffer).btShopStallDir;
    end;
    m_OnShopStall(Self);
  end;

  case m_nCurrentAction of
    SM_TURN:begin
        m_nStartFrame := HA.ActStand.start + m_btDir * (HA.ActStand.frame + HA.ActStand.skip);
        m_nEndFrame := m_nStartFrame + HA.ActStand.frame - 1;
        m_dwFrameTime := HA.ActStand.ftime;
        m_dwStartTime := TimeGetTime;
        m_StartCounter := timeGetTime;
        m_nDefFrameCount := HA.ActStand.frame;
        Shift(m_btDir, 0, 0, m_nEndFrame - m_nStartFrame + 1);
      end;
    SM_WALK,
      SM_BACKSTEP:begin
        m_nStartFrame := HA.ActWalk.start + m_btDir * (HA.ActWalk.frame + HA.ActWalk.skip);
        m_nEndFrame := m_nStartFrame + HA.ActWalk.frame - 1;
        m_dwFrameTime := HA.ActWalk.ftime;
        m_dwStartTime := TimeGetTime;
        m_StartCounter := timeGetTime;
        m_nMaxTick := HA.ActWalk.usetick;
        m_nCurTick := 0;
        // WarMode := False;
        m_nMoveStep := 1;

        if m_nCurrentAction = SM_BACKSTEP then begin
          m_nMoveStep := m_btStep;
          Shift(GetBack(m_btDir), m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1)
        end
        else
          Shift(m_btDir, m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1);
      end;
    SM_RUSH:begin
        if m_nRushDir = 0 then begin
          m_nRushDir := 1;
          m_nStartFrame := HA.ActRushLeft.start + m_btDir * (HA.ActRushLeft.frame + HA.ActRushLeft.skip);
          m_nEndFrame := m_nStartFrame + HA.ActRushLeft.frame - 1;
          m_dwFrameTime := HA.ActRushLeft.ftime;
          m_dwStartTime := TimeGetTime;
          m_StartCounter := timeGetTime;
          m_nMaxTick := HA.ActRushLeft.usetick;
          m_nCurTick := 0;
          m_nMoveStep := 1;
          Shift(m_btDir, 1, 0, m_nEndFrame - m_nStartFrame + 1);
        end
        else begin
          m_nRushDir := 0;
          m_nStartFrame := HA.ActRushRight.start + m_btDir * (HA.ActRushRight.frame + HA.ActRushRight.skip);
          m_nEndFrame := m_nStartFrame + HA.ActRushRight.frame - 1;
          m_dwFrameTime := HA.ActRushRight.ftime;
          m_dwStartTime := TimeGetTime;
          m_StartCounter := timeGetTime;
          m_nMaxTick := HA.ActRushRight.usetick;
          m_nCurTick := 0;
          m_nMoveStep := 1;
          Shift(m_btDir, 1, 0, m_nEndFrame - m_nStartFrame + 1);
        end;
      end;
    SM_RUSHKUNG:begin
        m_nStartFrame := HA.ActRun.start + m_btDir * (HA.ActRun.frame + HA.ActRun.skip);
        m_nEndFrame := m_nStartFrame + HA.ActRun.frame - 1;
        m_dwFrameTime := HA.ActRun.ftime;
        m_dwStartTime := TimeGetTime;
        m_StartCounter := timeGetTime;
        m_nMaxTick := HA.ActRun.usetick;
        m_nCurTick := 0;
        m_nMoveStep := 1;
        Shift(m_btDir, m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1);
      end;
    {SM_BACKSTEP:
       begin
          startframe := pm.ActWalk.start + (pm.ActWalk.frame - 1) + Dir * (pm.ActWalk.frame + pm.ActWalk.skip);
          m_nEndFrame := startframe - (pm.ActWalk.frame - 1);
          m_dwFrameTime := pm.ActWalk.ftime;
          m_dwStartTime := TimeGetTime;
          m_nMaxTick := pm.ActWalk.UseTick;
          m_nCurTick := 0;
          m_nMoveStep := 1;
          Shift (GetBack(Dir), m_nMoveStep, 0, m_nEndFrame-startframe+1);
       end;  }
    SM_SITDOWN:begin
        m_nStartFrame := HA.ActSitdown.start + m_btDir * (HA.ActSitdown.frame + HA.ActSitdown.skip);
        m_nEndFrame := m_nStartFrame + HA.ActSitdown.frame - 1;
        m_dwFrameTime := HA.ActSitdown.ftime;
        m_dwStartTime := TimeGetTime;
        m_StartCounter := timeGetTime;
      end;
    SM_RUN:begin
        {$IF DEBUG_RUN_DELAY_OTHER = 1}
        {
        if Self <> g_MySelf then
        begin
          g_OtherRunStart.Add(IntToStr(TimeGetTime));
          g_OtherRunStart.SaveToFile(g_sOtherRunStartFile);
        end;
        }
        {$IFEND}

        {$IF DEBUG_RUN_DELAY_SELF = 1}
        if Self = g_MySelf then begin

          g_SelfRunStart.Add(IntToStr(TimeGetTime));
          g_SelfRunStart.SaveToFile(g_sSelRunStartFile);
        end;
        {$IFEND}
        m_nStartFrame := HA.ActRun.start + m_btDir * (HA.ActRun.frame + HA.ActRun.skip);
        m_nEndFrame := m_nStartFrame + HA.ActRun.frame - 1;
        m_dwFrameTime := HA.ActRun.ftime;
        m_dwStartTime := TimeGetTime;
        m_StartCounter := timeGetTime;
        m_nMaxTick := HA.ActRun.usetick;
        m_nCurTick := 0;
        // WarMode := False;
        if m_nCurrentAction = SM_RUN then begin
          // 骑马一步三格 chongchong 2013-10-16
          if (m_btHorse <> 0) and g_ClientConfig.boHorseRun3Grid then
            m_nMoveStep := 3
          else
            m_nMoveStep := 2
        end
        else
          m_nMoveStep := 1;

        Shift(m_btDir, m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1);
      end;
    SM_HORSERUN:begin
        m_nStartFrame := HA.ActRun.start + m_btDir * (HA.ActRun.frame + HA.ActRun.skip);
        m_nEndFrame := m_nStartFrame + HA.ActRun.frame - 1;
        m_dwFrameTime := HA.ActRun.ftime;
        m_dwStartTime := TimeGetTime;
        m_StartCounter := timeGetTime;
        m_nMaxTick := HA.ActRun.usetick;
        m_nCurTick := 0;
        // WarMode := False;
        if m_nCurrentAction = SM_HORSERUN then
          m_nMoveStep := 3
        else
          m_nMoveStep := 1;

        // m_nMoveStep := 2;

        Shift(m_btDir, m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1);
      end;

    // 十步一杀
    SM_MAGICMOVE:begin
        meff := TNormalDrawEffect.Create(m_nCurrX, m_nCurrY, g_WMagic5Images, m_btDir * 10 + 10, 10, HA.ActHit.ftime, True);
        // modify chongchong 2013-07-18
        // PlayScene.m_EffectList.Add(meff);
        {$IF IsMultiThreadRender = 1}
        PlayScene.m_EffectList.Lock;
        try
          {$IFEND}
          PlayScene.m_EffectList.Add(meff);
          {$IF IsMultiThreadRender = 1}
        finally
          PlayScene.m_EffectList.UnLock;
        end;
        {$IFEND}
        // PlaySound(10522);
        m_nStartFrame := HA.ActHit.start + m_btDir * (HA.ActHit.frame + HA.ActHit.skip);
        m_nEndFrame := m_nStartFrame + HA.ActHit.frame - 1;
        m_dwFrameTime := HA.ActHit.ftime;
        m_dwStartTime := TimeGetTime;
        m_StartCounter := timeGetTime;
        m_nMaxTick := HA.ActHit.usetick;
        m_nCurTick := 0;
        // m_boWarMode := TRUE;
        // m_boCustomMagicNoAction := Fasle;
        // m_dwWarModeTime := TimeGetTime;
        m_nMoveStep := 0;
        Shift(m_btDir, m_nMoveStep, 0, 1);
      end;
    SM_CUSTOM_MAGICMOVE001..(SM_CUSTOM_MAGICMOVE001 + CUSTOM_MAGIC_COUNT - 1):begin
        // 自定义技能 chongchong 2015-03-20
        CustomMagicConfig := GetCustomMagicConfig(m_nCurrentAction - SM_CUSTOM_MAGICMOVE001 + CUSTOM_MAGIC_START_ID);

        // 自定义技能动作类型 chongchong 2015-03-20
        if CustomMagicConfig <> nil then begin
          case m_CurMagic.NewLevel of
            0:MagicPlusLevel := mplNone;
            1..3:MagicPlusLevel := mpl1_3;
            4..6:MagicPlusLevel := mpl4_6;
            7..9:MagicPlusLevel := mpl7_9;
            else
              MagicPlusLevel := mpl7_9;
          end;

          ClientConfig := @CustomMagicConfig.MagicConfigs[MagicPlusLevel];

          if ClientConfig.FastMove_NoHitAction then begin
            m_nStartFrame := HA.ActStand.start + m_btDir * (HA.ActStand.frame + HA.ActStand.skip);
            m_nEndFrame := m_nStartFrame + HA.ActStand.frame - 1;
            m_dwFrameTime := HA.ActStand.ftime;
            m_dwStartTime := TimeGetTime;
            m_StartCounter := timeGetTime;
            m_nMaxTick := HA.ActStand.usetick;
            m_nCurTick := 0;
          end
          else begin
            m_nStartFrame := HA.ActHit.start + m_btDir * (HA.ActHit.frame + HA.ActHit.skip);
            m_nEndFrame := m_nStartFrame + HA.ActHit.frame - 1;
            m_dwFrameTime := HA.ActHit.ftime;
            m_dwStartTime := TimeGetTime;
            m_StartCounter := timeGetTime;
            m_nMaxTick := HA.ActHit.usetick;
            m_nCurTick := 0;
          end;

          if (ClientConfig.FastMove_File >= 0) and (ClientConfig.FastMove_File < g_EffectImageList.Count)
            and {(ClientConfig.FastMove_StartIndex >= 0) and}(ClientConfig.FastMove_PlayCount > 0) then begin //HZQ 20230525 WORD类型
            nDirCount := 0;
            if ClientConfig.FastMove_CalcDir then nDirCount := m_btDir;

            nDirCount := nDirCount * (ClientConfig.FastMove_PlayCount + ClientConfig.FastMove_EmptyCount) + ClientConfig.FastMove_StartIndex;

            meff := TCustomMonTargetEffect.Create(nDirCount, -1, ClientConfig.FastMove_PlayCount, m_nCurrX, m_nCurrY);

            TCustomMonTargetEffect(meff).DrawMode := ClientConfig.FastMove_DrawMode;
            meff.ImgLib := TGameImages(g_EffectImageList.Objects[ClientConfig.FastMove_File]);
            meff.Light := ClientConfig.FastMove_LightRange;
            meff.NextFrameTime := ClientConfig.FastMove_PlayTime;
            {$IF IsMultiThreadRender = 1}
            PlayScene.m_EffectList.Lock;
            try
              {$IFEND}
              PlayScene.m_EffectList.Add(meff);
              {$IF IsMultiThreadRender = 1}
            finally
              PlayScene.m_EffectList.UnLock;
            end;
            {$IFEND}
          end;

          m_nMoveStep := 0;
          Shift(m_btDir, m_nMoveStep, 0, 1);
        end;
      end;
    SM_THROW:begin
        m_nStartFrame := HA.ActHit.start + m_btDir * (HA.ActHit.frame + HA.ActHit.skip);
        m_nEndFrame := m_nStartFrame + HA.ActHit.frame - 1;
        m_dwFrameTime := HA.ActHit.ftime;
        m_dwStartTime := TimeGetTime;
        m_StartCounter := timeGetTime;
        m_boWarMode := True;
        m_boCustomMagicNoAction := False;
        m_dwWarModeTime := TimeGetTime;
        m_boThrow := True;
        Shift(m_btDir, 0, 0, 1);
      end;
    SM_HIT, SM_POWERHIT, SM_LONGHIT, SM_WIDEHIT, SM_FIREHIT, SM_60HIT, SM_61HIT,
      SM_62HIT, SM_SWORDHIT, SM_TWNHIT, SM_CRSHIT, SM_43HIT, SM_66HIT, SM_66HIT1,
      SM_CUSTOM_HIT001..(SM_CUSTOM_HIT001 + CUSTOM_MAGIC_COUNT - 1),
      SM_CUSTOM_PUSH001..(SM_CUSTOM_PUSH001 + CUSTOM_MAGIC_COUNT - 1):begin
        m_nStartFrame := HA.ActHit.start + m_btDir * (HA.ActHit.frame + HA.ActHit.skip);
        m_nEndFrame := m_nStartFrame + HA.ActHit.frame - 1;
        m_dwFrameTime := HA.ActHit.ftime;
        m_dwStartTime := TimeGetTime;
        m_StartCounter := timeGetTime;
        m_boWarMode := True;
        m_boCustomMagicNoAction := False;
        m_dwWarModeTime := TimeGetTime;

        if (m_nCurrentAction = SM_POWERHIT) then begin
          m_boHitEffect := True;
          m_nMagLight := 2;
          m_nHitEffectNumber := 1;
        end;

        // 自定义技能
        if ((m_nCurrentAction >= SM_CUSTOM_HIT001) and (m_nCurrentAction < SM_CUSTOM_HIT001 + CUSTOM_MAGIC_COUNT)) or
          ((m_nCurrentAction >= SM_CUSTOM_PUSH001) and (m_nCurrentAction < SM_CUSTOM_PUSH001 + CUSTOM_MAGIC_COUNT)) then begin
          m_boHitEffect := True;
          m_nMagLight := 2;

          m_nHitEffectNumber := -m_nCurrentAction;

          if ((m_nCurrentAction >= SM_CUSTOM_HIT001) and (m_nCurrentAction < SM_CUSTOM_HIT001 + CUSTOM_MAGIC_COUNT)) then
            CustomMagicConfig := GetCustomMagicConfig(Abs(m_nCurrentAction) - SM_CUSTOM_HIT001 + CUSTOM_MAGIC_START_ID)
          else begin
            m_nMoveStep := m_btStep;
            CustomMagicConfig := GetCustomMagicConfig(Abs(m_nCurrentAction) - SM_CUSTOM_PUSH001 + CUSTOM_MAGIC_START_ID);
          end;

          // 自定义技能动作类型 chongchong 2015-03-20
          if CustomMagicConfig <> nil then begin
            {
            case m_nHitEffectLevel of
              0:    MagicPlusLevel := mplNone;
              1..3: MagicPlusLevel := mpl1_3;
              4..6: MagicPlusLevel := mpl4_6;
              7..9: MagicPlusLevel := mpl7_9;
            else
              MagicPlusLevel := mpl7_9;
            end;
            ClientConfig := @CustomMagicConfig.MagicConfigs[MagicPlusLevel];
            m_dwFrameTime := ClientConfig.Self_PlayTime;
            }

            case CustomMagicConfig.MagicBaseConfig.MagicActionType of
              matSpell:begin
                  m_nStartFrame := HA.ActSpell.start + m_btDir * (HA.ActSpell.frame + HA.ActSpell.skip);
                  m_nEndFrame := m_nStartFrame + HA.ActSpell.frame - 1;
                end;
              matHit:begin
                  m_nStartFrame := HA.ActHit.start + m_btDir * (HA.ActHit.frame + HA.ActHit.skip);
                  m_nEndFrame := m_nStartFrame + HA.ActHit.frame - 1;
                end;
              matJumpHit:begin
                  m_nStartFrame := HA.ActBigHit.start + m_btDir * (HA.ActBigHit.frame + HA.ActBigHit.skip);
                  m_nEndFrame := m_nStartFrame + HA.ActBigHit.frame - 1;
                end;
              matCustom:begin
                  m_nStartFrame := CustomMagicConfig.MagicBaseConfig.MagicActionStartIndex +
                    m_btDir * (CustomMagicConfig.MagicBaseConfig.MagicActionPlayCount + CustomMagicConfig.MagicBaseConfig.MagicActionEmptyCount);
                  m_nEndFrame := m_nStartFrame + CustomMagicConfig.MagicBaseConfig.MagicActionPlayCount - 1;
                end;
              else {// matNone} begin
                  m_nStartFrame := HA.ActHit.start + m_btDir * (HA.ActHit.frame + HA.ActHit.skip);
                  m_nEndFrame := m_nStartFrame + HA.ActHit.frame - 1;
                  m_boCustomMagicNoAction := True;
                end;
            end;
          end;

          m_nCurSelfEffFrame := 0;

          // 战士技能没必要支持中心多方向攻击 chongchong 2015-05-03
          {
          CustomMagicConfig := GetCustomMagicConfig(Abs(m_nCurrentAction) - SM_CUSTOM_HIT1 + CUSTOM_MAGIC_START_ID);
          // 自定义技能动作类型 chongchong 2015-03-20
          if CustomMagicConfig <> nil then
          begin
            case m_CurMagic.NewLevel of
              0:    MagicPlusLevel := mplNone;
              1..3: MagicPlusLevel := mpl1_3;
              4..6: MagicPlusLevel := mpl4_6;
              7..9: MagicPlusLevel := mpl7_9;
            else
              MagicPlusLevel := mpl7_9;
            end;

            ClientConfig := @CustomMagicConfig.MagicConfigs[MagicPlusLevel];
            if (ClientConfig.Self_File >= 0) and (ClientConfig.Self_File < g_EffectImageList.Count) and
              (ClientConfig.Self_StartIndex >= 0) and (ClientConfig.Self_PlayCount > 0) and
              (ClientConfig.Self_DirCalcType = mdctCenter) then
            begin
              PlayScene.ScreenXYfromMCXY(m_nCurrX, m_nCurrY, nX, nY);
              if ClientConfig.Self_DirCount = mdcDir8 then
                nDirCount := 8
              else
                nDirCount := 16;

              for I := 0 to nDirCount - 1 do
              begin
                meff := TMagicEff.Create(111, 0, nX, nY, nX, nY, mtExplosion, True, 0);
                meff.MagExplosionBase := ClientConfig.Self_StartIndex + I * (ClientConfig.Self_PlayCount + ClientConfig.Self_EmptyCount);
                meff.TargetActor := nil;
                meff.NextFrameTime := ClientConfig.Self_PlayTime;
                meff.ExplosionFrame := ClientConfig.Self_PlayCount;

                if I = 1 then
                  meff.Light := ClientConfig.Self_LightRange;

                meff.ImgLib := TGameImages(g_EffectImageList.Objects[ClientConfig.Self_File]);

                PlayScene.AddEffectList(meff);
              end;
            end;
          end;
          }
        end;

        if (m_nCurrentAction = SM_LONGHIT) then begin
          m_boHitEffect := True;
          m_nMagLight := 2;
          m_nHitEffectNumber := 2;
        end;
        if (m_nCurrentAction = SM_WIDEHIT) then begin
          m_boHitEffect := True;
          m_nMagLight := 2;
          m_nHitEffectNumber := 3;
        end;
        if (m_nCurrentAction = SM_FIREHIT) then begin
          m_boHitEffect := True;
          m_nMagLight := 2;
          // 4级技能强化 -- 4级烈火 (下面的代码还原了) chongchong 2013-12-04
          m_nHitEffectNumber := 4;
        end;

        if (m_nCurrentAction = SM_43HIT) then begin // 雷霆剑法    ID=43
          m_boHitEffect := True;
          m_nMagLight := 2;
          m_nHitEffectNumber := 7;

          m_CurMagic.ServerMagicCode := 0;
          m_CurMagic.MagicSerial := 43;
          m_CurMagic.EffectNumber := 38;
          m_CurMagic.targx := Self.m_nTargetX;
          m_CurMagic.targy := Self.m_nTargetY;

          m_boUseMagic := True;
          m_nCurEffFrame := 0;

          // 测试方法，先按一下雷霆剑法凝聚，在按十步一杀，打怪打出雷霆剑法就卡住了）
          // #m_nSpellFrame := 12 雷霆剑法卡 2020-03-24 12:21:01
          m_nSpellFrame := 6;

          m_nMagLight := 2;
        end;

        if (m_nCurrentAction = SM_CRSHIT) then begin
          m_boHitEffect := True;
          m_nMagLight := 2;
          m_nHitEffectNumber := 6;
        end;

        if (m_nCurrentAction = SM_TWNHIT) then begin // 龙影剑法     ID=42
          /////////////龙影剑法人物动作 -- piaoyun 2013-09-16 //////////////////
          m_nStartFrame := HA.ActBigHit.start + m_btDir * (HA.ActBigHit.frame + HA.ActBigHit.skip);
          m_nEndFrame := m_nStartFrame + HA.ActBigHit.frame - 1;
          m_dwFrameTime := HA.ActBigHit.ftime;
          m_dwStartTime := TimeGetTime;
          m_StartCounter := timeGetTime;
          m_boWarMode := TRUE;
          m_boCustomMagicNoAction := False;
          m_dwWarModeTime := TimeGetTime;
          Shift(m_btDir, 0, 0, 1);

          m_boHitEffect := True;
          m_nMagLight := 2;
          m_nHitEffectNumber := 8;
        end;

        if (m_nCurrentAction = SM_60HIT) then begin // 破魂斩
          {
          m_boHitEffect := True;
          m_nMagLight := 2;
          m_nHitEffectNumber := 9;
          m_dwFrameTime := 120;
          }
          // 修复 破魂斩动作没放完，被后继攻击动作替代 chongchong 2013-10-25
          m_boHitEffect := True;
          m_nMagLight := 2;
          m_nHitEffectNumber := -1;

          //meff := TNormalDrawEffect.Create(m_nCurrX, m_nCurrY, g_WMagic4Images, m_btDir * 20 + 10, 18, 85, True);

          // 修复战战合击破魂斩卡位（使用合击之后，立马跑步) chongchong 2018-06-20 17:21:35
          nDir := GetNextDirection(m_nCurrX, m_nCurrY, m_SM60HitX, m_SM60HitY);
          meff := TNormalDrawEffect.Create(m_nCurrX, m_nCurrY, g_WMagic4Images, nDir * 20 + 10, 18, 85, True);

          PlayScene.m_EffectList.Add(meff);
        end;

        if (m_nCurrentAction = SM_61HIT) then begin // 劈星斩
          {
          m_boHitEffect := True;
          m_nHitEffectNumber ：= 10;
          }
          // 修复劈星斩动作不对 chongchong 2013-10-26
          m_boHitEffect := True;
          m_nMagLight := 2;
          m_nHitEffectNumber := -1;
          PlayScene.m_EffectList.Add(TNormalDrawEffect.Create(m_nCurrX, m_nCurrY, g_WMagic4Images, 460, 10, 80, True));
        end;

        if (m_nCurrentAction = SM_62HIT) then begin // 雷霆一击
          m_boHitEffect := True;
          m_nMagLight := 2;
          m_nHitEffectNumber := 11;

          m_CurMagic.ServerMagicCode := 0;
          m_CurMagic.MagicSerial := 62;
          m_CurMagic.EffectNumber := 62;
          m_CurMagic.targx := Self.m_nTargetX;
          m_CurMagic.targy := Self.m_nTargetY;

          m_boUseMagic := True;
          m_nCurEffFrame := 0;
          m_nSpellFrame := 10;
          m_nMagLight := 2;
        end;

        if (m_nCurrentAction = SM_66HIT) then {// 开天斩重击特效编号  piaoyun 2013-08-24} begin
          // 修复开天斩特效有残留 chongchong 2014-09-02
          m_nEndFrame := m_nStartFrame + HA.ActHit.frame - 1;
          //m_dwFrameTime := 96;
          m_boHitEffect := True;
          m_nMagLight := 2;
          m_nHitEffectNumber := 12;
        end;
        if (m_nCurrentAction = SM_66HIT1) then {// 开天斩轻击特效编号  piaoyun 2013-08-24} begin
          // 修复开天斩特效有残留 chongchong 2014-09-02
          m_nEndFrame := m_nStartFrame + HA.ActHit.frame - 1;
          //m_dwFrameTime := 96;
          m_boHitEffect := True;
          m_nMagLight := 2;
          m_nHitEffectNumber := 25;
        end;
        if (m_nCurrentAction = SM_SWORDHIT) then {// 逐日剑法} begin
          /////////////逐日剑法人物动作 -- piaoyun 2013-08-24 //////////////////
          // 修正逐日剑法帧数一别的攻击帧数不一致，导致攻击间隔不均匀
          {
          m_nStartFrame := HA.ActBigHit.start + m_btDir * (HA.ActBigHit.frame + HA.ActBigHit.skip);
          m_nEndFrame := m_nStartFrame + HA.ActBigHit.frame - 1;
          m_dwFrameTime := HA.ActBigHit.ftime;
          }

          m_nStartFrame := HA.ActHit.start + m_btDir * (HA.ActHit.frame + HA.ActHit.skip);
          m_nEndFrame := m_nStartFrame + HA.ActHit.frame - 1;
          m_dwFrameTime := HA.ActHit.ftime;

          m_dwStartTime := TimeGetTime;
          m_StartCounter := timeGetTime;
          m_boWarMode := TRUE;
          m_dwWarModeTime := TimeGetTime;
          Shift(m_btDir, 0, 0, 1);
          //////////////////////////////////////////////////////////////////////

          m_boHitEffect := True;
          m_nMagLight := 2;
          m_nHitEffectNumber := 14;
        end;

        // 重新计算帧速 chongchong 2016-12-14
        NextHitTime := GetNextHitTime;
        if m_dwFrameTime * Cardinal(m_nEndFrame - m_nStartFrame + 1) >= NextHitTime then begin
          TempFrameTime := NextHitTime div Cardinal(m_nEndFrame - m_nStartFrame + 1) - 3;
          if g_ClientConfig.nAttackSpeed > 0 then begin
            TempFrameTime := TempFrameTime - Round(TempFrameTime * g_ClientConfig.nAttackSpeed / 1000);
          end;
          if TempFrameTime <= MIN_ATTACK_FRAME_TIME then TempFrameTime := MIN_ATTACK_FRAME_TIME;
          m_dwFrameTime := TempFrameTime;
        end else begin
          if g_ClientConfig.nAttackSpeed < 0 then begin
            TempFrameTime := Integer(m_dwFrameTime) - Round(Integer(m_dwFrameTime) * g_ClientConfig.nAttackSpeed / 1000); //HZQ 20230525
            if TempFrameTime * (m_nEndFrame - m_nStartFrame + 1) >= Integer(NextHitTime) then begin
              TempFrameTime := NextHitTime div Cardinal(m_nEndFrame - m_nStartFrame + 1) - 3;
            end;
            if TempFrameTime <= MIN_ATTACK_FRAME_TIME then TempFrameTime := MIN_ATTACK_FRAME_TIME;
            m_dwFrameTime := TempFrameTime;
          end else if g_ClientConfig.nAttackSpeed > 0 then begin
            TempFrameTime := m_dwFrameTime - Round(Integer(m_dwFrameTime) * g_ClientConfig.nAttackSpeed / 1000);
            if TempFrameTime <= MIN_ATTACK_FRAME_TIME then TempFrameTime := MIN_ATTACK_FRAME_TIME;
            m_dwFrameTime := TempFrameTime;
          end;
        end;
        if not ((m_nCurrentAction >= SM_CUSTOM_PUSH001) and (m_nCurrentAction < (SM_CUSTOM_PUSH001 + CUSTOM_MAGIC_COUNT))) then //修复自定义追心刺偏移的问题  By 一支笔 at:2021-07-07 13:11:21
          Shift(m_btDir, 0, 0, 1);
      end;
    SM_HEAVYHIT:begin
        m_nStartFrame := HA.ActHeavyHit.start + m_btDir * (HA.ActHeavyHit.frame + HA.ActHeavyHit.skip);
        m_nEndFrame := m_nStartFrame + HA.ActHeavyHit.frame - 1;
        m_dwFrameTime := HA.ActHeavyHit.ftime;
        m_dwStartTime := TimeGetTime;
        m_StartCounter := timeGetTime;
        m_boWarMode := True;
        m_boCustomMagicNoAction := False;
        m_dwWarModeTime := TimeGetTime;

        // 重新计算帧速 chongchong 2016-12-14
        NextHitTime := GetNextHitTime;
        if m_dwFrameTime * Cardinal(m_nEndFrame - m_nStartFrame + 1) >= NextHitTime then begin
          TempFrameTime := NextHitTime div Cardinal(m_nEndFrame - m_nStartFrame + 1) - 3; //HZQ 20250325
          if g_ClientConfig.nAttackSpeed > 0 then begin
            TempFrameTime := TempFrameTime - Round(TempFrameTime * g_ClientConfig.nAttackSpeed / 1000);
          end;
          if TempFrameTime <= MIN_ATTACK_FRAME_TIME then TempFrameTime := MIN_ATTACK_FRAME_TIME;
          m_dwFrameTime := TempFrameTime;
        end else begin
          if g_ClientConfig.nAttackSpeed < 0 then begin
            TempFrameTime := m_dwFrameTime - Round(Integer(m_dwFrameTime) * g_ClientConfig.nAttackSpeed / 1000);
            if Cardinal(TempFrameTime * (m_nEndFrame - m_nStartFrame + 1)) >= NextHitTime then begin
              TempFrameTime := NextHitTime div Cardinal(m_nEndFrame - m_nStartFrame + 1) - 3;
            end;
            if TempFrameTime <= MIN_ATTACK_FRAME_TIME then TempFrameTime := MIN_ATTACK_FRAME_TIME;
            m_dwFrameTime := TempFrameTime;
          end else if g_ClientConfig.nAttackSpeed > 0 then begin
            TempFrameTime := m_dwFrameTime - Round(Integer(m_dwFrameTime) * g_ClientConfig.nAttackSpeed / 1000);
            if TempFrameTime <= MIN_ATTACK_FRAME_TIME then TempFrameTime := MIN_ATTACK_FRAME_TIME;
            m_dwFrameTime := TempFrameTime;
          end;
        end;

        Shift(m_btDir, 0, 0, 1);
      end;
    SM_BIGHIT:begin
        m_nStartFrame := HA.ActBigHit.start + m_btDir * (HA.ActBigHit.frame + HA.ActBigHit.skip);
        m_nEndFrame := m_nStartFrame + HA.ActBigHit.frame - 1;
        m_dwFrameTime := HA.ActBigHit.ftime;
        m_dwStartTime := TimeGetTime;
        m_StartCounter := timeGetTime;
        m_boWarMode := True;
        m_boCustomMagicNoAction := False;
        m_dwWarModeTime := TimeGetTime;

        // 重新计算帧速 chongchong 2016-12-14
        NextHitTime := GetNextHitTime;
        if m_dwFrameTime * Cardinal(m_nEndFrame - m_nStartFrame + 1) >= NextHitTime then begin
          TempFrameTime := NextHitTime div Cardinal(m_nEndFrame - m_nStartFrame + 1) - 3;
          if g_ClientConfig.nAttackSpeed > 0 then begin
            TempFrameTime := TempFrameTime - Round(TempFrameTime * g_ClientConfig.nAttackSpeed / 1000);
          end;
          if TempFrameTime <= MIN_ATTACK_FRAME_TIME then TempFrameTime := MIN_ATTACK_FRAME_TIME;
          m_dwFrameTime := TempFrameTime;
        end else begin
          if g_ClientConfig.nAttackSpeed < 0 then begin
            TempFrameTime := m_dwFrameTime - Round(Integer(m_dwFrameTime) * g_ClientConfig.nAttackSpeed / 1000);
            if Cardinal(TempFrameTime * (m_nEndFrame - m_nStartFrame + 1)) >= NextHitTime then begin
              TempFrameTime := NextHitTime div Cardinal(m_nEndFrame - m_nStartFrame + 1) - 3;
            end;
            if TempFrameTime <= MIN_ATTACK_FRAME_TIME then TempFrameTime := MIN_ATTACK_FRAME_TIME;
            m_dwFrameTime := TempFrameTime;
          end else if g_ClientConfig.nAttackSpeed > 0 then begin
            TempFrameTime := m_dwFrameTime - Round(Integer(m_dwFrameTime) * g_ClientConfig.nAttackSpeed / 1000);
            if TempFrameTime <= MIN_ATTACK_FRAME_TIME then TempFrameTime := MIN_ATTACK_FRAME_TIME;
            m_dwFrameTime := TempFrameTime;
          end;
        end;

        Shift(m_btDir, 0, 0, 1);
      end;
    SM_100HIT:begin // 追心刺
        m_nStartFrame := HA.ActContinuousHits[1].start + m_btDir * (HA.ActContinuousHits[1].frame + HA.ActContinuousHits[1].skip);
        m_nEndFrame := m_nStartFrame + HA.ActContinuousHits[1].frame - 1;
        m_dwFrameTime := HA.ActContinuousHits[1].ftime;
        m_dwStartTime := TimeGetTime;
        m_StartCounter := timeGetTime;
        m_boWarMode := True;
        m_boCustomMagicNoAction := False;
        m_dwWarModeTime := TimeGetTime;

        m_nMoveStep := m_btStep;
        //Shift(m_btDir, m_nMoveStep, 0, m_nEndFrame - m_nStartFrame + 1);

        m_nMagLight := 2;
        m_nHitEffectNumber := 23;
        m_boHitEffect := True;

        m_nHitEffectLevel := 0;

        // 重新计算帧速 chongchong 2016-12-14
        NextHitTime := GetNextHitTime;
        if m_dwFrameTime * Cardinal(m_nEndFrame - m_nStartFrame + 1) >= NextHitTime then begin
          TempFrameTime := NextHitTime div Cardinal(m_nEndFrame - m_nStartFrame + 1) - 3;
          if g_ClientConfig.nAttackSpeed > 0 then begin
            TempFrameTime := TempFrameTime - Round(TempFrameTime * g_ClientConfig.nAttackSpeed / 1000);
          end;
          if TempFrameTime <= MIN_ATTACK_FRAME_TIME then TempFrameTime := MIN_ATTACK_FRAME_TIME;
          m_dwFrameTime := TempFrameTime;
        end else begin
          if g_ClientConfig.nAttackSpeed < 0 then begin
            TempFrameTime := m_dwFrameTime - Round(Integer(m_dwFrameTime) * g_ClientConfig.nAttackSpeed / 1000);
            if Cardinal(TempFrameTime * (m_nEndFrame - m_nStartFrame + 1)) >= NextHitTime then begin
              TempFrameTime := NextHitTime div Cardinal(m_nEndFrame - m_nStartFrame + 1) - 3;
            end;
            if TempFrameTime <= MIN_ATTACK_FRAME_TIME then TempFrameTime := MIN_ATTACK_FRAME_TIME;
            m_dwFrameTime := TempFrameTime;
          end else if g_ClientConfig.nAttackSpeed > 0 then begin
            TempFrameTime := m_dwFrameTime - Round(Integer(m_dwFrameTime) * g_ClientConfig.nAttackSpeed / 1000);
            if TempFrameTime <= MIN_ATTACK_FRAME_TIME then TempFrameTime := MIN_ATTACK_FRAME_TIME;
            m_dwFrameTime := TempFrameTime;
          end;
        end;
      end;
    SM_101HIT:begin // 三绝杀
        m_nStartFrame := HA.ActContinuousHits[2].start + m_btDir * (HA.ActContinuousHits[2].frame + HA.ActContinuousHits[2].skip);
        m_nEndFrame := m_nStartFrame + HA.ActContinuousHits[2].frame - 1;
        m_dwFrameTime := HA.ActContinuousHits[2].ftime;
        m_dwStartTime := TimeGetTime;
        m_StartCounter := timeGetTime;
        m_boWarMode := True;
        m_boCustomMagicNoAction := False;
        m_dwWarModeTime := TimeGetTime;

        m_nMagLight := 2;
        m_nHitEffectNumber := 20;
        m_boHitEffect := True;

        m_nHitEffectLevel := 0;

        // 重新计算帧速 chongchong 2016-12-14
        NextHitTime := GetNextHitTime;
        if m_dwFrameTime * Cardinal(m_nEndFrame - m_nStartFrame + 1) >= NextHitTime then begin
          TempFrameTime := NextHitTime div Cardinal(m_nEndFrame - m_nStartFrame + 1) - 3;
          if g_ClientConfig.nAttackSpeed > 0 then begin
            TempFrameTime := TempFrameTime - Round(TempFrameTime * g_ClientConfig.nAttackSpeed / 1000);
          end;
          if TempFrameTime <= MIN_ATTACK_FRAME_TIME then TempFrameTime := MIN_ATTACK_FRAME_TIME;
          m_dwFrameTime := TempFrameTime;
        end else begin
          if g_ClientConfig.nAttackSpeed < 0 then begin
            TempFrameTime := m_dwFrameTime - Round(Integer(m_dwFrameTime) * g_ClientConfig.nAttackSpeed / 1000);
            if Cardinal(TempFrameTime * (m_nEndFrame - m_nStartFrame + 1)) >= NextHitTime then begin
              TempFrameTime := NextHitTime div Cardinal(m_nEndFrame - m_nStartFrame + 1) - 3;
            end;
            if TempFrameTime <= MIN_ATTACK_FRAME_TIME then TempFrameTime := MIN_ATTACK_FRAME_TIME;
            m_dwFrameTime := TempFrameTime;
          end else if g_ClientConfig.nAttackSpeed > 0 then begin
            TempFrameTime := m_dwFrameTime - Round(Integer(m_dwFrameTime) * g_ClientConfig.nAttackSpeed / 1000);
            if TempFrameTime <= MIN_ATTACK_FRAME_TIME then TempFrameTime := MIN_ATTACK_FRAME_TIME;
            m_dwFrameTime := TempFrameTime;
          end;
        end;
      end;

    SM_102HIT:begin // 断岳斩
        m_nStartFrame := HA.ActContinuousHits[3].start + m_btDir * (HA.ActContinuousHits[3].frame + HA.ActContinuousHits[3].skip);
        m_nEndFrame := m_nStartFrame + HA.ActContinuousHits[3].frame - 1;
        m_dwFrameTime := 250; // HA.ActContinuousHits[3].ftime;
        m_dwStartTime := TimeGetTime;
        m_StartCounter := timeGetTime;
        m_boWarMode := True;
        m_boCustomMagicNoAction := False;
        m_dwWarModeTime := TimeGetTime;
        m_nMagLight := 2;
        m_nHitEffectNumber := 21;
        m_boHitEffect := True;
        m_nHitEffectLevel := 0;

        // 重新计算帧速 chongchong 2016-12-14
        NextHitTime := GetNextHitTime;
        if m_dwFrameTime * Cardinal(m_nEndFrame - m_nStartFrame + 1) >= NextHitTime then begin
          TempFrameTime := NextHitTime div Cardinal(m_nEndFrame - m_nStartFrame + 1) - 3;
          if g_ClientConfig.nAttackSpeed > 0 then begin
            TempFrameTime := TempFrameTime - Round(TempFrameTime * g_ClientConfig.nAttackSpeed / 1000);
          end;
          if TempFrameTime <= MIN_ATTACK_FRAME_TIME then TempFrameTime := MIN_ATTACK_FRAME_TIME;
          m_dwFrameTime := TempFrameTime;
        end else begin
          if g_ClientConfig.nAttackSpeed < 0 then begin
            TempFrameTime := m_dwFrameTime - Round(Integer(m_dwFrameTime) * g_ClientConfig.nAttackSpeed / 1000);
            if Cardinal(TempFrameTime * (m_nEndFrame - m_nStartFrame + 1)) >= NextHitTime then begin
              TempFrameTime := NextHitTime div Cardinal((m_nEndFrame - m_nStartFrame + 1)) - 3;
            end;
            if TempFrameTime <= MIN_ATTACK_FRAME_TIME then TempFrameTime := MIN_ATTACK_FRAME_TIME;
            m_dwFrameTime := TempFrameTime;
          end else if g_ClientConfig.nAttackSpeed > 0 then begin
            TempFrameTime := m_dwFrameTime - Round(Integer(m_dwFrameTime) * g_ClientConfig.nAttackSpeed / 1000);
            if TempFrameTime <= MIN_ATTACK_FRAME_TIME then TempFrameTime := MIN_ATTACK_FRAME_TIME;
            m_dwFrameTime := TempFrameTime;
          end;
        end;
        PlayScene.ScreenXYfromMCXY(m_nCurrX, m_nCurrY, nX, nY);
        meff := TMagicEff.Create(111, 0, nX, nY, nX, nY, mtExplosion, True, 0);
        meff.MagExplosionBase := 1920 + m_btDir * 10;
        meff.TargetActor := Self;
        meff.NextFrameTime := 60;
        meff.ExplosionFrame := 5;
        meff.ImgLib := g_cboEffect;
        PlayScene.AddEffectList(meff);
      end;

    SM_103HIT:begin // 横扫千军
        m_nStartFrame := HA.ActContinuousHits[5].start + m_btDir * (HA.ActContinuousHits[5].frame + HA.ActContinuousHits[5].skip);
        m_nEndFrame := m_nStartFrame + HA.ActContinuousHits[5].frame - 1;
        m_dwFrameTime := HA.ActContinuousHits[5].ftime;
        m_dwStartTime := TimeGetTime;
        m_StartCounter := timeGetTime;
        m_boWarMode := True;
        m_boCustomMagicNoAction := False;
        m_dwWarModeTime := TimeGetTime;
        m_nMagLight := 2;
        m_nHitEffectNumber := 22;
        m_nHitEffectLevel := 0;
        m_boHitEffect := True;

        // 重新计算帧速 chongchong 2016-12-14
        NextHitTime := GetNextHitTime;
        if m_dwFrameTime * Cardinal(m_nEndFrame - m_nStartFrame + 1) >= NextHitTime then begin
          TempFrameTime := NextHitTime div Cardinal(m_nEndFrame - m_nStartFrame + 1) - 3;
          if g_ClientConfig.nAttackSpeed > 0 then begin
            TempFrameTime := TempFrameTime - Round(TempFrameTime * g_ClientConfig.nAttackSpeed / 1000);
          end;
          if TempFrameTime <= MIN_ATTACK_FRAME_TIME then TempFrameTime := MIN_ATTACK_FRAME_TIME;
          m_dwFrameTime := TempFrameTime;
        end else begin
          if g_ClientConfig.nAttackSpeed < 0 then begin
            TempFrameTime := m_dwFrameTime - Round(Integer(m_dwFrameTime) * g_ClientConfig.nAttackSpeed / 1000);
            if TempFrameTime * (m_nEndFrame - m_nStartFrame + 1) >= Integer(NextHitTime) then begin
              TempFrameTime := NextHitTime div Cardinal(m_nEndFrame - m_nStartFrame + 1) - 3;
            end;
            if TempFrameTime <= MIN_ATTACK_FRAME_TIME then TempFrameTime := MIN_ATTACK_FRAME_TIME;
            m_dwFrameTime := TempFrameTime;
          end else if g_ClientConfig.nAttackSpeed > 0 then begin
            TempFrameTime := m_dwFrameTime - Round(Integer(m_dwFrameTime) * g_ClientConfig.nAttackSpeed / 1000);
            if TempFrameTime <= MIN_ATTACK_FRAME_TIME then TempFrameTime := MIN_ATTACK_FRAME_TIME;
            m_dwFrameTime := TempFrameTime;
          end;
        end;
      end;

    // 断空斩 chongchong 2018-01-29
    SM_113HIT:begin
        m_nStartFrame := HA.ActContinuousHits[4].start + m_btDir * (HA.ActContinuousHits[4].frame + HA.ActContinuousHits[4].skip);
        m_nEndFrame := m_nStartFrame + HA.ActContinuousHits[4].frame - 1;
        m_dwFrameTime := 250; // HA.ActContinuousHits[3].ftime;
        m_dwStartTime := TimeGetTime;
        m_StartCounter := timeGetTime;
        m_boWarMode := True;
        m_boCustomMagicNoAction := False;
        m_dwWarModeTime := TimeGetTime;
        m_nMagLight := 2;
        m_nHitEffectNumber := 26;
        m_boHitEffect := True;

        m_nHitEffectLevel := 0;

        // 重新计算帧速 chongchong 2016-12-14
        NextHitTime := GetNextHitTime;
        if m_dwFrameTime * Cardinal(m_nEndFrame - m_nStartFrame + 1) >= NextHitTime then begin
          TempFrameTime := NextHitTime div Cardinal(m_nEndFrame - m_nStartFrame + 1) - 3;
          if g_ClientConfig.nAttackSpeed > 0 then begin
            TempFrameTime := TempFrameTime - Round(TempFrameTime * g_ClientConfig.nAttackSpeed / 1000);
          end;
          if TempFrameTime <= MIN_ATTACK_FRAME_TIME then TempFrameTime := MIN_ATTACK_FRAME_TIME;
          m_dwFrameTime := TempFrameTime;
        end else begin
          if g_ClientConfig.nAttackSpeed < 0 then begin
            TempFrameTime := m_dwFrameTime - Round(Integer(m_dwFrameTime) * g_ClientConfig.nAttackSpeed / 1000);
            if TempFrameTime * (m_nEndFrame - m_nStartFrame + 1) >= Integer(NextHitTime) then begin
              TempFrameTime := NextHitTime div Cardinal(m_nEndFrame - m_nStartFrame + 1) - 3;
            end;
            if TempFrameTime <= MIN_ATTACK_FRAME_TIME then TempFrameTime := MIN_ATTACK_FRAME_TIME;
            m_dwFrameTime := TempFrameTime;
          end else if g_ClientConfig.nAttackSpeed > 0 then begin
            TempFrameTime := m_dwFrameTime - Round(Integer(m_dwFrameTime) * g_ClientConfig.nAttackSpeed / 1000);
            if TempFrameTime <= MIN_ATTACK_FRAME_TIME then TempFrameTime := MIN_ATTACK_FRAME_TIME;
            m_dwFrameTime := TempFrameTime;
          end;
        end;

        {
        PlayScene.ScreenXYfromMCXY(m_nCurrX, m_nCurrY, nX, nY);
        meff := TMagicEff.Create(111, 0, nX, nY, nX, nY, mtExplosion, True, 0);
        meff.MagExplosionBase := 400 + m_btDir * 20;
        meff.TargetActor := Self;
        meff.NextFrameTime := 60;
        meff.ExplosionFrame := 13;
        meff.ImgLib := g_cboEffect;
        PlayScene.AddEffectList(meff);

        }
      end;

    // 血魄一击 chongchong 2018-01-29
    SM_115HIT:begin
        m_nStartFrame := HA.ActContinuousHits[3].start + m_btDir * (HA.ActContinuousHits[3].frame + HA.ActContinuousHits[3].skip);
        m_nEndFrame := m_nStartFrame + HA.ActContinuousHits[3].frame - 1;
        m_dwFrameTime := 250; // HA.ActContinuousHits[3].ftime;
        m_dwStartTime := TimeGetTime;
        m_StartCounter := timeGetTime;
        m_boWarMode := True;
        m_boCustomMagicNoAction := False;
        m_dwWarModeTime := TimeGetTime;
        m_nMagLight := 2;
        m_nHitEffectNumber := 27;
        m_boHitEffect := True;

        m_nHitEffectLevel := 0;

        // 重新计算帧速 chongchong 2016-12-14
        NextHitTime := GetNextHitTime;
        if m_dwFrameTime * Cardinal(m_nEndFrame - m_nStartFrame + 1) >= NextHitTime then begin
          TempFrameTime := NextHitTime div Cardinal(m_nEndFrame - m_nStartFrame + 1) - 3;
          if g_ClientConfig.nAttackSpeed > 0 then begin
            TempFrameTime := TempFrameTime - Round(TempFrameTime * g_ClientConfig.nAttackSpeed / 1000);
          end;
          if TempFrameTime <= MIN_ATTACK_FRAME_TIME then TempFrameTime := MIN_ATTACK_FRAME_TIME;
          m_dwFrameTime := TempFrameTime;
        end else begin
          if g_ClientConfig.nAttackSpeed < 0 then begin
            TempFrameTime := m_dwFrameTime - Round(Integer(m_dwFrameTime) * g_ClientConfig.nAttackSpeed / 1000);
            if TempFrameTime * (m_nEndFrame - m_nStartFrame + 1) >= Integer(NextHitTime) then begin
              TempFrameTime := Integer(NextHitTime) div (m_nEndFrame - m_nStartFrame + 1) - 3;
            end;
            if TempFrameTime <= MIN_ATTACK_FRAME_TIME then TempFrameTime := MIN_ATTACK_FRAME_TIME;
            m_dwFrameTime := TempFrameTime;
          end else if g_ClientConfig.nAttackSpeed > 0 then begin
            TempFrameTime := m_dwFrameTime - Round(Integer(m_dwFrameTime) * g_ClientConfig.nAttackSpeed / 1000);
            if TempFrameTime <= MIN_ATTACK_FRAME_TIME then TempFrameTime := MIN_ATTACK_FRAME_TIME;
            m_dwFrameTime := TempFrameTime;
          end;
        end;

        {
        PlayScene.ScreenXYfromMCXY(m_nCurrX, m_nCurrY, nX, nY);
        meff := TMagicEff.Create(111, 0, nX, nY, nX, nY, mtExplosion, True, 0);
        meff.MagExplosionBase := 400 + m_btDir * 20;
        meff.TargetActor := Self;
        meff.NextFrameTime := 60;
        meff.ExplosionFrame := 13;
        meff.ImgLib := g_cboEffect;
        PlayScene.AddEffectList(meff);

        }
      end;
    SM_SPELL:begin
        // 自定义技能 chongchong 2015-03-20
        CustomMagicConfig := GetCustomMagicConfig(m_CurMagic.MagicSerial);

        m_boCustomMagicNoAction := False;

        // 自定义技能动作类型 chongchong 2015-03-20
        if CustomMagicConfig <> nil then begin
          case CustomMagicConfig.MagicBaseConfig.MagicActionType of
            matSpell:begin
                m_nStartFrame := HA.ActSpell.start + m_btDir * (HA.ActSpell.frame + HA.ActSpell.skip);
                m_nEndFrame := m_nStartFrame + HA.ActSpell.frame - 1;
              end;
            matHit:begin
                m_nStartFrame := HA.ActHit.start + m_btDir * (HA.ActHit.frame + HA.ActHit.skip);
                m_nEndFrame := m_nStartFrame + HA.ActHit.frame - 1;
              end;
            matJumpHit:begin
                m_nStartFrame := HA.ActBigHit.start + m_btDir * (HA.ActBigHit.frame + HA.ActBigHit.skip);
                m_nEndFrame := m_nStartFrame + HA.ActBigHit.frame - 1;
              end;
            matCustom:begin
                m_nStartFrame := CustomMagicConfig.MagicBaseConfig.MagicActionStartIndex +
                  m_btDir * (CustomMagicConfig.MagicBaseConfig.MagicActionPlayCount + CustomMagicConfig.MagicBaseConfig.MagicActionEmptyCount);
                m_nEndFrame := m_nStartFrame + CustomMagicConfig.MagicBaseConfig.MagicActionPlayCount - 1;
              end;
            else {// matNone} begin
                m_nStartFrame := HA.ActStand.start + m_btDir * (HA.ActStand.frame + HA.ActStand.skip);
                m_nEndFrame := m_nStartFrame + HA.ActStand.frame - 1;

                m_boCustomMagicNoAction := True;
              end;
          end;

          m_dwFrameTime := 50;
          m_dwStartTime := TimeGetTime;
          m_StartCounter := timeGetTime;
          m_nCurEffFrame := 0;
          m_nCurSelfEffFrame := 0;
          m_dwCurSelfEffFrameTick := TimeGetTime;

          m_UseEffectImage := nil;
          m_boUseMagic := True;

          case m_CurMagic.NewLevel of
            0:MagicPlusLevel := mplNone;
            1..3:MagicPlusLevel := mpl1_3;
            4..6:MagicPlusLevel := mpl4_6;
            7..9:MagicPlusLevel := mpl7_9;
            else
              MagicPlusLevel := mpl7_9;
          end;

          ClientConfig := @CustomMagicConfig.MagicConfigs[MagicPlusLevel];

          if (ClientConfig.Self_File >= 0) and (ClientConfig.Self_File < g_EffectImageList.Count)
            {and (ClientConfig.Self_StartIndex >= 0)}and (ClientConfig.Self_PlayCount > 0) then begin //HZQ Self_StartIndex:WORD
            // 加上动画帧数 2019-04-28 18:06:14
            m_nSpellFrame := Max(DEFSPELLFRAME, ClientConfig.Self_PlayCount);
            m_dwFrameTime := ClientConfig.Self_PlayTime;

            if (ClientConfig.Self_DirCalcType = mdctCenter) then begin
              if (not ClientConfig.Self_PlayFailNoDraw) or (m_CurMagic.ServerMagicCode > 0) then begin
                PlayScene.ScreenXYfromMCXY(m_nCurrX, m_nCurrY, nX, nY);
                if ClientConfig.Self_DirCount = mdcDir8 then
                  nDirCount := 8
                else
                  nDirCount := 16;

                for I := 0 to nDirCount - 1 do begin
                  meff := TMagicEff.Create(111, 0, nX, nY, nX, nY, mtExplosion, True, 0);
                  meff.MagExplosionBase := ClientConfig.Self_StartIndex + I * (ClientConfig.Self_PlayCount + ClientConfig.Self_EmptyCount);
                  meff.TargetActor := nil;
                  meff.NextFrameTime := ClientConfig.Self_PlayTime;
                  meff.ExplosionFrame := ClientConfig.Self_PlayCount;

                  if I = 1 then
                    meff.Light := ClientConfig.Self_LightRange;

                  meff.ImgLib := TGameImages(g_EffectImageList.Objects[ClientConfig.Self_File]);

                  PlayScene.AddEffectList(meff);
                end;
              end;
            end;
          end;
        end
        else begin
          m_nStartFrame := HA.ActSpell.start + m_btDir * (HA.ActSpell.frame + HA.ActSpell.skip);
          m_nEndFrame := m_nStartFrame + HA.ActSpell.frame - 1;

          m_dwFrameTime := HA.ActSpell.ftime;
          m_dwStartTime := TimeGetTime;
          m_StartCounter := timeGetTime;
          m_nCurEffFrame := 0;
          m_boUseMagic := True;
        end;

        case m_CurMagic.EffectNumber of // Effect
          22:begin // 火墙
              m_nMagLight := 4;
              m_nSpellFrame := 10;
            end;
          26: {// 心灵启示} begin

              m_nMagLight := 2;
              m_nSpellFrame := 20;
              m_dwFrameTime := m_dwFrameTime div 2;
            end;
          32: {// 解毒术 chongchong 2015-05-21} begin
              m_nMagLight := 2;
              m_nSpellFrame := 10;
              m_dwFrameTime := m_dwFrameTime;
            end;
          35:begin //
              m_nMagLight := 2;
              m_nSpellFrame := 15;
            end;
          43:begin // 狮子吼
              m_nMagLight := 2;

              // 修改狮子吼播放特效时不能移动 chongchong 2016-11-25
              //m_nSpellFrame := 20;

              PlayScene.ScreenXYfromMCXY(m_nCurrX, m_nCurrY, nX, nY);
              meff := TMagicEff.Create(111, 0, nX, nY, nX, nY, mtExplosion, True, 0);
              meff.MagExplosionBase := 710;
              meff.TargetActor := Self;
              meff.NextFrameTime := 80;
              meff.ExplosionFrame := 20;
              meff.ImgLib := g_WMagic2Images;
              PlayScene.AddEffectList(meff);
            end;
          48:begin // 噬血术
              if m_CurMagic.NewLevel in [4..6] then begin
                m_nSpellFrame := 13;
                m_dwFrameTime := m_dwFrameTime - m_dwFrameTime div 3;
              end
              else begin
                m_nSpellFrame := 10;
              end;
              m_nMagLight := 2;
            end;
          51:begin // 流星火雨
              m_nMagLight := 4;
              m_nSpellFrame := 10;
            end;
          61..65:begin // 合击起始动作 chongchong 2015-04-25
              m_nMagLight := 2;
              m_nSpellFrame := 10;
              m_dwFrameTime := 80; // 修复使用合击技能后，后面再使用魔法技能速度变慢 chongchong 2015-05-11
            end;
          47:begin
              if m_CurMagic.NewLevel > 0 then
                m_nSpellFrame := 20
              else
                m_nSpellFrame := 10;
            end;
          68:begin
              {if (m_CurMagic.NewLevel = 0) and (m_CurMagic.MagicLevel = 4) then
                m_nSpellFrame := 10
              else
                m_nSpellFrame := 5;
              }
              m_nSpellFrame := 9;
              m_dwFrameTime := 45;
            end;
          69:begin
              m_nSpellFrame := 9;
            end;
          107:begin // 双龙破
              m_UseEffectImage := nil;
              if m_CurMagic.EffectNumber > 0 then begin
                GetEffectBase(m_CurMagic.EffectNumber - 1, 0, m_UseEffectImage, m_nEffectFrame, m_CurMagic.NewLevel);
                m_boUseEffect := True;
                m_dwEffectFrameTime := HA.ActContinuousHits[11].ftime;
                m_nEffectFrame := m_nEffectFrame + m_btDir * 20;
                m_nEffectEnd := m_nEffectFrame + 11;
              end;
              m_nStartFrame := HA.ActContinuousHits[11].start + m_btDir * (HA.ActContinuousHits[11].frame + HA.ActContinuousHits[11].skip);
              m_nEndFrame := m_nStartFrame + HA.ActContinuousHits[11].frame - 1;
              m_dwFrameTime := HA.ActContinuousHits[11].ftime;
              m_nMagLight := 4;
              // m_nSpellFrame := 13;
              m_nSpellFrame := HA.ActContinuousHits[11].frame;
              m_nHitEffectLevel := 0;

              PlayScene.ScreenXYfromMCXY(m_nCurrX, m_nCurrY, nX, nY);
              meff := TMagicEff.Create(111, 0, nX, nY, nX, nY, mtExplosion, True, 0);
              meff.MagExplosionBase := 3990;
              meff.TargetActor := Self;
              meff.NextFrameTime := 80;
              meff.ExplosionFrame := 15;
              meff.ImgLib := g_cboEffect;
              PlayScene.AddEffectList(meff);
            end;

          104:begin // 凤舞祭
              m_UseEffectImage := nil;
              if m_CurMagic.EffectNumber > 0 then begin
                GetEffectBase(m_CurMagic.EffectNumber - 1, 0, m_UseEffectImage, m_nEffectFrame, m_CurMagic.NewLevel);
                m_boUseEffect := True;
                m_dwEffectFrameTime := HA.ActContinuousHits[6].ftime;
                m_nEffectFrame := m_nEffectFrame + m_btDir * 10;
                m_nEffectEnd := m_nEffectFrame + 6;
              end;
              m_nStartFrame := HA.ActContinuousHits[6].start + m_btDir * (HA.ActContinuousHits[6].frame + HA.ActContinuousHits[6].skip);
              m_nEndFrame := m_nStartFrame + HA.ActContinuousHits[6].frame - 1;
              m_dwFrameTime := HA.ActContinuousHits[6].ftime;
              m_nMagLight := 4;
              // m_nSpellFrame := 10;
              m_nSpellFrame := HA.ActContinuousHits[6].frame;
              m_nHitEffectLevel := 0;

              PlayScene.ScreenXYfromMCXY(m_nCurrX, m_nCurrY, nX, nY);
              meff := TMagicEff.Create(111, 0, nX, nY, nX, nY, mtExplosion, True, 0);
              meff.MagExplosionBase := 3990;
              meff.TargetActor := Self;
              meff.NextFrameTime := 80;
              meff.ExplosionFrame := 15;
              meff.ImgLib := g_cboEffect;
              PlayScene.AddEffectList(meff);
            end;
          105:begin // 惊雷爆
              m_UseEffectImage := nil;
              if m_CurMagic.EffectNumber > 0 then begin
                GetEffectBase(m_CurMagic.EffectNumber - 1, 0, m_UseEffectImage, m_nEffectFrame, m_CurMagic.NewLevel);
                m_boUseEffect := True;
                m_dwEffectFrameTime := HA.ActContinuousHits[14].ftime;
                // m_nEffectFrame := m_nEffectFrame + m_btDir * 10;
                m_nEffectEnd := m_nEffectFrame + 7;
              end;
              m_nStartFrame := HA.ActContinuousHits[14].start + m_btDir * (HA.ActContinuousHits[14].frame + HA.ActContinuousHits[14].skip);
              m_nEndFrame := m_nStartFrame + HA.ActContinuousHits[14].frame - 1;
              m_dwFrameTime := HA.ActContinuousHits[14].ftime;
              m_nMagLight := 4;
              m_nHitEffectLevel := 0;
              // m_nSpellFrame := 10;
              m_nSpellFrame := HA.ActContinuousHits[14].frame;

              PlayScene.ScreenXYfromMCXY(m_nCurrX, m_nCurrY, nX, nY);
              meff := TMagicEff.Create(111, 0, nX, nY, nX, nY, mtExplosion, True, 0);
              meff.MagExplosionBase := 3990;
              meff.TargetActor := Self;
              meff.NextFrameTime := 80;
              meff.ExplosionFrame := 15;
              meff.ImgLib := g_cboEffect;
              PlayScene.AddEffectList(meff);
            end;

          106:begin // 冰天雪地
              m_UseEffectImage := nil;
              if m_CurMagic.EffectNumber > 0 then begin
                GetEffectBase(m_CurMagic.EffectNumber - 1, 0, m_UseEffectImage, m_nEffectFrame, m_CurMagic.NewLevel);
                m_boUseEffect := True;
                m_dwEffectFrameTime := HA.ActContinuousHits[8].ftime;
                m_nEffectFrame := m_nEffectFrame + m_btDir * 10;
                m_nEffectEnd := m_nEffectFrame + 7;
              end;

              m_nStartFrame := HA.ActContinuousHits[8].start + m_btDir * (HA.ActContinuousHits[8].frame + HA.ActContinuousHits[8].skip);
              m_nEndFrame := m_nStartFrame + HA.ActContinuousHits[8].frame - 1;
              m_dwFrameTime := HA.ActContinuousHits[8].ftime;
              m_nMagLight := 4;
              m_nHitEffectLevel := 0;
              // m_nSpellFrame := 10;
              m_nSpellFrame := HA.ActContinuousHits[8].frame;

              PlayScene.ScreenXYfromMCXY(m_nCurrX, m_nCurrY, nX, nY);
              meff := TMagicEff.Create(111, 0, nX, nY, nX, nY, mtExplosion, True, 0);
              meff.MagExplosionBase := 3990;
              meff.TargetActor := Self;
              meff.NextFrameTime := 80;
              meff.ExplosionFrame := 15;
              meff.ImgLib := g_cboEffect;
              PlayScene.AddEffectList(meff);
            end;

          108:begin // 虎啸诀
              m_UseEffectImage := nil;
              if m_CurMagic.EffectNumber > 0 then begin
                GetEffectBase(m_CurMagic.EffectNumber - 1, 0, m_UseEffectImage, m_nEffectFrame, m_CurMagic.NewLevel);
                m_boUseEffect := True;
                m_dwEffectFrameTime := HA.ActContinuousHits[12].ftime;
                m_nEffectFrame := m_nEffectFrame + m_btDir * 10;
                m_nEffectEnd := m_nEffectFrame + 5;
              end;
              m_nStartFrame := HA.ActContinuousHits[12].start + m_btDir * (HA.ActContinuousHits[12].frame + HA.ActContinuousHits[12].skip);
              m_nEndFrame := m_nStartFrame + HA.ActContinuousHits[12].frame - 1;
              m_dwFrameTime := HA.ActContinuousHits[12].ftime;
              m_nMagLight := 4;
              m_nHitEffectLevel := 0;
              // m_nSpellFrame := 10;
              m_nSpellFrame := HA.ActContinuousHits[12].frame;

              PlayScene.ScreenXYfromMCXY(m_nCurrX, m_nCurrY, nX, nY);
              meff := TMagicEff.Create(111, 0, nX, nY, nX, nY, mtExplosion, True, 0);
              meff.MagExplosionBase := 3990;
              meff.TargetActor := Self;
              meff.NextFrameTime := 80;
              meff.ExplosionFrame := 15;
              meff.ImgLib := g_cboEffect;
              PlayScene.AddEffectList(meff);
            end;
          109:begin // 八卦掌
              m_UseEffectImage := nil;
              if m_CurMagic.EffectNumber > 0 then begin
                GetEffectBase(m_CurMagic.EffectNumber - 1, 0, m_UseEffectImage, m_nEffectFrame, m_CurMagic.NewLevel);
                m_boUseEffect := True;
                m_dwEffectFrameTime := HA.ActContinuousHits[15].ftime;
                m_nEffectFrame := m_nEffectFrame + m_btDir * 20;
                m_nEffectEnd := m_nEffectFrame + 9;
              end;
              m_nStartFrame := HA.ActContinuousHits[15].start + m_btDir * (HA.ActContinuousHits[15].frame + HA.ActContinuousHits[15].skip);
              m_nEndFrame := m_nStartFrame + HA.ActContinuousHits[15].frame - 1;
              m_dwFrameTime := HA.ActContinuousHits[15].ftime;
              m_nMagLight := 4;
              m_nHitEffectLevel := 0;
              // m_nSpellFrame := 12;
              m_nSpellFrame := HA.ActContinuousHits[15].frame;

              PlayScene.ScreenXYfromMCXY(m_nCurrX, m_nCurrY, nX, nY);
              meff := TMagicEff.Create(111, 0, nX, nY, nX, nY, mtExplosion, True, 0);
              meff.MagExplosionBase := 3990;
              meff.TargetActor := Self;
              meff.NextFrameTime := 80;
              meff.ExplosionFrame := 15;
              meff.ImgLib := g_cboEffect;
              PlayScene.AddEffectList(meff);
            end;
          110:begin // 三焰咒
              m_UseEffectImage := nil;
              if m_CurMagic.EffectNumber > 0 then begin
                GetEffectBase(m_CurMagic.EffectNumber - 1, 0, m_UseEffectImage, m_nEffectFrame, m_CurMagic.NewLevel);
                m_boUseEffect := True;
                m_dwEffectFrameTime := HA.ActContinuousHits[16].ftime;
                m_nEffectFrame := m_nEffectFrame + m_btDir * 20;
                m_nEffectEnd := m_nEffectFrame + 8;
              end;
              m_nStartFrame := HA.ActContinuousHits[16].start + m_btDir * (HA.ActContinuousHits[16].frame + HA.ActContinuousHits[16].skip);
              m_nEndFrame := m_nStartFrame + HA.ActContinuousHits[16].frame - 1;
              m_dwFrameTime := HA.ActContinuousHits[16].ftime;
              m_nMagLight := 4;
              m_nHitEffectLevel := 0;

              // m_nSpellFrame := 12;
              m_nSpellFrame := HA.ActContinuousHits[16].frame;

              PlayScene.ScreenXYfromMCXY(m_nCurrX, m_nCurrY, nX, nY);
              meff := TMagicEff.Create(111, 0, nX, nY, nX, nY, mtExplosion, True, 0);
              meff.MagExplosionBase := 3990;
              meff.TargetActor := Self;
              meff.NextFrameTime := 80;
              meff.ExplosionFrame := 15;
              meff.ImgLib := g_cboEffect;
              PlayScene.AddEffectList(meff);
            end;
          111:begin // 万剑归宗
              m_UseEffectImage := nil;
              if m_CurMagic.EffectNumber > 0 then begin
                GetEffectBase(m_CurMagic.EffectNumber - 1, 0, m_UseEffectImage, m_nEffectFrame, m_CurMagic.NewLevel);
                m_boUseEffect := True;
                m_dwEffectFrameTime := HA.ActContinuousHits[17].ftime;
                m_nEffectFrame := m_nEffectFrame + m_btDir * 20;
                m_nEffectEnd := m_nEffectFrame + 15;
              end;
              m_nStartFrame := HA.ActContinuousHits[17].start + m_btDir * (HA.ActContinuousHits[17].frame + HA.ActContinuousHits[17].skip);
              m_nEndFrame := m_nStartFrame + HA.ActContinuousHits[17].frame - 1;
              m_dwFrameTime := HA.ActContinuousHits[17].ftime;
              m_nMagLight := 4;
              m_nHitEffectLevel := 0;
              // m_nSpellFrame := 15;
              m_nSpellFrame := HA.ActContinuousHits[17].frame;

              PlayScene.ScreenXYfromMCXY(m_nCurrX, m_nCurrY, nX, nY);
              meff := TMagicEff.Create(111, 0, nX, nY, nX, nY, mtExplosion, True, 0);
              meff.MagExplosionBase := 3990;
              meff.TargetActor := Self;
              meff.NextFrameTime := 80;
              meff.ExplosionFrame := 15;
              meff.ImgLib := g_cboEffect;
              PlayScene.AddEffectList(meff);
            end;
          55:begin // 倚天辟地
              m_UseEffectImage := nil;
              if m_CurMagic.EffectNumber > 0 then begin
                GetEffectBase(m_CurMagic.EffectNumber - 1, 0, m_UseEffectImage, m_nEffectFrame, m_CurMagic.NewLevel);
                m_boUseEffect := True;
                m_dwEffectFrameTime := HA.ActContinuousHits[4].ftime;
                m_nEffectFrame := m_nEffectFrame + m_btDir * 20;
                m_nEffectEnd := m_nEffectFrame + 13;
              end;
              m_nStartFrame := HA.ActContinuousHits[4].start + m_btDir * (HA.ActContinuousHits[4].frame + HA.ActContinuousHits[4].skip);
              m_nEndFrame := m_nStartFrame + HA.ActContinuousHits[4].frame - 1;
              m_dwFrameTime := HA.ActContinuousHits[4].ftime;
              m_nMagLight := 4;
              m_nHitEffectLevel := 0;
              m_nSpellFrame := HA.ActContinuousHits[4].frame;
            end;
          203:begin // 死亡之眼 -- piaoyun 2013-06-24
              // m_nMagLight := 2;
              m_nSpellFrame := 17;
              m_dwFrameTime := Round(m_dwFrameTime * 0.6);
            end;

          // 旋风斩转身 piaoyun 2013-09-15
          208:begin
              m_UseEffectImage := nil;
              if m_CurMagic.EffectNumber > 0 then begin
                GetEffectBase(m_CurMagic.EffectNumber - 1, 0, m_UseEffectImage, m_nEffectFrame, m_CurMagic.NewLevel);
                m_boUseEffect := True;
                m_dwEffectFrameTime := HA.ActContinuousHits[5].ftime;
                //m_nEffectFrame := m_nEffectFrame + m_btDir * 10;
                m_nEffectEnd := m_nEffectFrame + 6;
              end;

              m_nStartFrame := HA.ActContinuousHits[5].start + m_btDir * (HA.ActContinuousHits[5].frame + HA.ActContinuousHits[5].skip);
              m_nEndFrame := m_nStartFrame + HA.ActContinuousHits[5].frame - 1;
              m_dwFrameTime := HA.ActContinuousHits[5].ftime;
              m_nMagLight := 5;
              m_nHitEffectLevel := 0;
              m_nSpellFrame := HA.ActContinuousHits[5].frame;
            end;
          else begin
              // 自定义技能，不要在这里扯蛋改照亮范围，因为自定义技能自己会配置 chongchong 2015-07-31 17:24:01
              if CustomMagicConfig = nil then
                m_nMagLight := 2;

              // 连击技能的帧用自定义的数量，不然模拟倚天辟地的技能时，爬地上半天不起来 chongchong 2015-11-03
              if (CustomMagicConfig <> nil) and (CustomMagicConfig.MagicBaseConfig.MagicActionType = matCustom) and
                (CustomMagicConfig.MagicBaseConfig.MagicActionContinue) then begin
                m_nSpellFrame := CustomMagicConfig.MagicBaseConfig.MagicActionPlayCount;
                m_dwFrameTime := 110;
              end

                // 自定义技能不要在这里扯蛋设置帧数，前面自己设置 2019-04-28 19:34:04
              else if CustomMagicConfig = nil then
                m_nSpellFrame := DEFSPELLFRAME;
            end;
        end;

        m_dwWaitMagicRequest := TimeGetTime;

        //if not boCustomMagicNoneAction then
        m_boWarMode := True;
        m_dwWarModeTime := TimeGetTime;

        // 魔法帧不加速，不然速度很难算得准 chongchong 2018-07-04 23:54:50
        nSpellSpeed := g_ClientConfig.nSpellSpeed + m_nSpellSpeed * 100;
        if nSpellSpeed <> 0 then // 魔法帧速
          m_dwFrameTime := Max(m_dwFrameTime - Round(Integer(m_dwFrameTime) * nSpellSpeed / 10 / 100), 0);

        // 重新计算帧速 chongchong 2016-12-14
        NextHitTime := g_ClientConfig.dwMagicHitFrameTime + 50;

        // 修正魔法速度以全速为准
        if m_nSpellSpeed <> 0 then // 魔法速度
          NextHitTime := _Max(Integer(NextHitTime) - m_nSpellSpeed * Integer(g_ClientConfig.dwIncSpellSpeedDecInterval), 0);

        if m_dwFrameTime * Cardinal(m_nEndFrame - m_nStartFrame + 1) >= NextHitTime then begin
          //TempFrameTime := NextHitTime div (m_nEndFrame - m_nStartFrame + 1) + 20;
          TempFrameTime := m_dwFrameTime;
          if g_ClientConfig.nSpellSpeed > 0 then begin
            TempFrameTime := TempFrameTime - Round(TempFrameTime * g_ClientConfig.nSpellSpeed / 1000);
          end;
          if TempFrameTime <= MIN_SPELL_FRAME_TIME then TempFrameTime := MIN_SPELL_FRAME_TIME;
          m_dwFrameTime := TempFrameTime;
        end else begin
          if g_ClientConfig.nSpellSpeed < 0 then begin
            TempFrameTime := m_dwFrameTime - Round(Integer(m_dwFrameTime) * g_ClientConfig.nSpellSpeed / 1000);
            if Cardinal(TempFrameTime * (m_nEndFrame - m_nStartFrame + 1)) >= NextHitTime then begin
              TempFrameTime := NextHitTime div Cardinal(m_nEndFrame - m_nStartFrame + 1) - 3;
            end;
            if TempFrameTime <= MIN_SPELL_FRAME_TIME then TempFrameTime := MIN_SPELL_FRAME_TIME;
            m_dwFrameTime := TempFrameTime;
          end else if g_ClientConfig.nSpellSpeed > 0 then begin
            TempFrameTime := m_dwFrameTime - Round(Integer(m_dwFrameTime) * g_ClientConfig.nSpellSpeed / 1000);
            if TempFrameTime <= MIN_SPELL_FRAME_TIME then TempFrameTime := MIN_SPELL_FRAME_TIME;
            m_dwFrameTime := TempFrameTime;
          end;
        end;
        Shift(m_btDir, 0, 0, 1);
      end;

    SM_STRUCK:begin
        // 骑马 修复官方马被攻击时动作 chongchong 2013-10-17
        if (m_btHorse in [1..5]) and (m_btDoubleHumHorse <> 0) then begin
          m_nStartFrame := 192 + m_btDir * (7 + 1);
          m_nEndFrame := m_nStartFrame + 3 - 1;
        end
        else begin
          m_nStartFrame := HA.ActStruck.start + m_btDir * (HA.ActStruck.frame + HA.ActStruck.skip);
          m_nEndFrame := m_nStartFrame + HA.ActStruck.frame - 1;
        end;

        //m_dwFrameTime := m_dwStruckFrameTime;                                                       // HA.ActStruck.ftime;
        m_dwFrameTime := HA.ActStruck.ftime; // 延长人物弯腰时间 原来是 - 10 chongchong 2018-07-31 19:39:52

        m_dwStartTime := TimeGetTime;
        m_StartCounter := timeGetTime;
        Shift(m_btDir, 0, 0, 1);

        m_dwGenAnicountTime := TimeGetTime;
        m_dwGenNewHitAnicountTime := TimeGetTime;
        m_dwGenNewMagAnicountTime := TimeGetTime;
        m_nCurBubbleStruck := 0;
        m_nCurNewHitBubbleStruck := 0;
        m_nCurNewMagBubbleStruck := 0;

        m_CustomMagicStatusEffect.m_nStruck := 0;
      end;

    SM_NOWDEATH:begin
        // 骑马 修复官方马死亡倒地状态 chongchong 2013-10-17
        if (m_btHorse in [1..5]) and (m_btDoubleHumHorse <> 0) then begin
          m_nStartFrame := 256 + m_btDir * (7 + 1);
          m_nEndFrame := m_nStartFrame + 8 - 1;
          m_dwFrameTime := HA.ActDie.ftime;
          m_dwStartTime := TimeGetTime;
          m_StartCounter := timeGetTime;
        end
        else begin
          // ActDie: (start: 536; frame: 4; skip: 4; ftime: 120; usetick: 0);
          m_nStartFrame := HA.ActDie.start + m_btDir * (HA.ActDie.frame + HA.ActDie.skip);
          m_nEndFrame := m_nStartFrame + HA.ActDie.frame - 1;
          m_dwFrameTime := HA.ActDie.ftime;
          m_dwStartTime := TimeGetTime;
          m_StartCounter := timeGetTime;
        end;
      end;
  end;

  {$IF Enabled_PlugEngine = 1}
  if Assigned(HookTHumActor_CalcActorFrame) then begin
    try
      HookTHumActor_CalcActorFrame(Self);
    except
      on E:Exception do begin
        DebugOutStr('[Exception] HookTHumActor_CalcActorFrame');
        DebugOutStr(E.Message);
      end;
    end;
  end;
  {$IFEND}

  // DebugOutStr('THumActor.CalcActorFrame m_dwFrameTime:' + IntToStr(m_dwFrameTime));
end;

function THumActor.UseMagicDelayTime(dwDelayTime:Integer):Integer;
//var
  //nSpellSpeed: Integer;
begin
  {nSpellSpeed := g_ClientConfig.nSpellSpeed + m_nSpellSpeed;
  if nSpellSpeed <> 0 then                                                                          // 魔法帧速
    Result := Max((300 + dwDelayTime) - Round((300 + dwDelayTime) * nSpellSpeed * 10 / 100), 0)
  else
    Result := (300 + dwDelayTime);
  }

  // 技能延时时间由数据库配置，这里不用加速了 chongchong 2018-07-04 23:54:18
  Result := (200 + dwDelayTime);
end;

function THumActor.DefaultMotion:Boolean;
var
  nFrame:Integer;
begin
  Result := inherited DefaultMotion;
  nFrame := m_nFrame;
  if (m_wEffect = 50) then begin
    if (m_nCurrentFrame <= 536) then begin
      if (TimeGetTime - m_dwFrameTick) > 100 then begin
        if nFrame < 19 then
          Inc(nFrame)
        else begin
          if not m_bo2D0 then
            m_bo2D0 := True
          else
            m_bo2D0 := False;
          nFrame := 0;
        end;
        m_dwFrameTick := TimeGetTime();
      end;
    end;
  end
  else begin
    if (m_wEffect <> 0) then begin
      if m_nCurrentFrame < 64 then begin
        if (TimeGetTime - m_dwFrameTick) > m_dwFrameTime then begin
          if nFrame < 7 then
            Inc(nFrame)
          else
            nFrame := 0;
          m_dwFrameTick := TimeGetTime();
        end;

      end
      else begin
      end;
    end;
  end;

  if (m_wEffect_30 = 50) then begin
    if (m_nCurrentFrame <= 536) then begin
      if (TimeGetTime - m_dwFrameTick) > 100 then begin
        if nFrame < 19 then
          Inc(nFrame)
        else begin
          if not m_bo2D0 then
            m_bo2D0 := True
          else
            m_bo2D0 := False;
          nFrame := 0;
        end;
        m_dwFrameTick := TimeGetTime();
      end;
    end;
  end
  else begin
    if (m_wEffect_30 <> 0) then begin
      if m_nCurrentFrame < 64 then begin
        if (TimeGetTime - m_dwFrameTick) > m_dwFrameTime then begin
          if nFrame < 7 then
            Inc(nFrame)
          else
            nFrame := 0;
          m_dwFrameTick := TimeGetTime();
        end;

      end
      else begin
      end;
    end;
  end;

  if m_nFrame <> nFrame then begin
    m_nFrame := nFrame;
    Result := True;
    // PlayScene.LoadSurface(LoadSurface);
  end;
  // if Result then
  // DScreen.AddChatBoardString(Format('DefaultMotion %d',[m_dwFrameTime]), clGreen, clWhite);
end;

function THumActor.GetDefaultFrame(wmode:Boolean):Integer;
var
  I, cf, TempDir:Integer;
  MonsterConfig, TempMonsterConfig:PClientCustomMonsterConfig;
begin
  if m_nChangeAppr >= 0 then begin
    if m_nChangeAppr >= 100000 then begin
      Result := 0;
      MonsterConfig := nil;

      for I := 0 to g_CustomMonsterConfig.Count - 1 do begin
        TempMonsterConfig := g_CustomMonsterConfig.Items[I];
        if TempMonsterConfig.wMonsterAppr = m_nChangeAppr - 100000 then begin
          MonsterConfig := TempMonsterConfig;
          Break;
        end;
      end;

      if MonsterConfig <> nil then begin
        if m_boDeath then begin
          if m_boSkeleton then
            Result := MonsterConfig.Actions[matDie].StartIndex
          else begin
            if MonsterConfig.Actions[matDie].CalcDir then
              TempDir := m_btDir
            else
              TempDir := 0;
            Result := MonsterConfig.Actions[matDie].StartIndex + TempDir * (MonsterConfig.Actions[matDie].PlayCount + MonsterConfig.Actions[matDie].EmptyCount) + (MonsterConfig.Actions[matDie].PlayCount - 1);
          end;
        end
        else begin
          if (m_nState and STATE_STONE_MODE) <> 0 then begin
            if MonsterConfig.Actions[matStoneRevive].CalcDir then
              TempDir := m_btDir
            else
              TempDir := 0;

            Result := MonsterConfig.Actions[matStoneRevive].StartIndex + TempDir * (MonsterConfig.Actions[matStoneRevive].PlayCount + MonsterConfig.Actions[matStoneRevive].EmptyCount);
          end
          else begin
            if m_nCurrentDefFrame < 0 then
              cf := 0
            else if m_nCurrentDefFrame >= MonsterConfig.Actions[matStand].PlayCount then
              cf := 0
            else
              cf := m_nCurrentDefFrame;

            if MonsterConfig.Actions[matStand].CalcDir then
              TempDir := m_btDir
            else
              TempDir := 0;
            Result := MonsterConfig.Actions[matStand].StartIndex + TempDir * (MonsterConfig.Actions[matStand].PlayCount + MonsterConfig.Actions[matStand].EmptyCount) + cf;
          end;
        end;
      end;
    end
    else
      Result := inherited GetDefaultFrame(wmode);

    Exit;
  end;

  // 修复攻击时骑马在人攻击归位前不显示 chongchong 2013-11-08
  if wmode and (m_btHorse <> 0) then
    wmode := False;

  // GlimmingMode := False;
  // dr := Dress div 2;            //HUMANFRAME * (dr)
  if m_boDeath then begin
    // 骑马 - 修复官方马骑马死亡后躺下状态 chongchong 2013-10-17
    if (m_btHorse in [1..5]) and (m_btDoubleHumHorse <> 0) then
      Result := 256 + m_btDir * (7 + 1) + 8 - 1
    else
      Result := HA.ActDie.start + m_btDir * (HA.ActDie.frame + HA.ActDie.skip) + (HA.ActDie.frame - 1)
  end
  else if wmode then begin
    if m_boCustomMagicNoAction then
      Result := HA.ActStand.start + m_btDir * (HA.ActStand.frame + HA.ActStand.skip)
    else
      Result := HA.ActWarMode.start + m_btDir * (HA.ActWarMode.frame + HA.ActWarMode.skip);
  end
  else begin
    m_nDefFrameCount := HA.ActStand.frame;
    if m_nCurrentDefFrame < 0 then
      cf := 0
    else if m_nCurrentDefFrame >= HA.ActStand.frame then
      cf := 0 // HA.ActStand.frame-1
    else
      cf := m_nCurrentDefFrame;
    Result := HA.ActStand.start + m_btDir * (HA.ActStand.frame + HA.ActStand.skip) + cf;
  end;
end;

procedure THumActor.RunFrameAction(frame:Integer);
var
  meff:TMapEffect;
  event:TClEvent;
  mfly:TFlyingAxe;
begin
  m_boHideWeapon := False;
  if m_nCurrentAction = SM_HEAVYHIT then begin
    if (frame = 5) and (m_boDigFragment) then begin
      m_boDigFragment := False;
      meff := TMapEffect.Create(8 * m_btDir, 3, m_nCurrX, m_nCurrY);
      meff.ImgLib := g_WEffectImg;
      meff.NextFrameTime := 80;
      g_PlaySound.PlaySound(s_strike_stone);
      // g_PlaySound.PlaySound (s_drop_stonepiece);

      PlayScene.AddEffectList(meff);

      event := EventMan.GetEvent(m_nCurrX, m_nCurrY, ET_PILESTONES);
      if event <> nil then
        event.m_nEventParam := event.m_nEventParam + 1;
    end;
  end;
  if m_nCurrentAction = SM_THROW then begin
    if (frame = 3) and (m_boThrow) then begin
      m_boThrow := False;
      mfly := TFlyingAxe(PlayScene.NewFlyObject(Self,
        m_nCurrX,
        m_nCurrY,
        m_nTargetX,
        m_nTargetY,
        m_nTargetRecog,
        mtFlyAxe));
      if mfly <> nil then begin
        TFlyingAxe(mfly).ReadyFrame := 40;
        mfly.ImgLib := g_WMonImages.Indexs[3];
        mfly.FlyImageBase := FLYOMAAXEBASE;
      end;

    end;
    if frame >= 3 then
      m_boHideWeapon := True;
  end;
end;

procedure THumActor.DoWeaponBreakEffect;
begin
  m_boWeaponEffect := True;
  m_nCurWeaponEffect := 0;
end;

procedure THumActor.DoBrokenShieldEffect;
begin
  m_boBrokenShield := True;
  m_nBrokenShieldEffect := 0;
end;

function THumActor.CheckLoadUserName:Boolean;
var
  StrUserName:string;
begin
  Result := False;
  if (not GameCanvas.Active) or (not GameCanvas.Initialized) then Exit;
  m_sNameText := '';

  //if Length(m_sShowUserName) > 0 then
  //  StrUserName := m_sShowUserName
  //else
  StrUserName := m_sUserName;

  if PlugInEnabled then begin
    // 修正人形怪隐藏尸体时不隐藏名字 2019-09-04 21:47:03
    //and (Actor.m_btRace = RC_PLAYOBJECT) and Actor.m_boPlayMoster
    if not ((g_ClientConfig.boHideGhost and g_ConfigDlg.ConfigCheckeds[ckHideGhost]) and m_boDeath) then begin
      if g_ClientConfig.boShowUserName and g_ConfigDlg.ConfigCheckeds[ckShowUserName] then begin
        if (not g_ConfigDlg.ConfigCheckeds[ckOnlyShowCharName]) or (g_FocusCret = Self) or ((Self = g_MySelf) and g_boSelectMySelf) then begin
          // 双人骑马时，只显示名称，不显示称号 chongchong 2013-10-16
          if not ((m_btHorse in [1, 2]) and (m_btDoubleHumHorse <> 0)) then
            m_sNameText := m_sDescUserName + '\' + StrUserName
          else
            m_sNameText := StrUserName;
        end

          // 只显示人名，就不要称号名 chongchong 2015-03-13
        else
          m_sNameText := m_sUserName;
      end
      else if (g_FocusCret = Self) or ((Self = g_MySelf) and g_boSelectMySelf) then begin
        m_sNameText := m_sDescUserName + '\' + StrUserName;
      end;
    end;
  end
  else begin
    if (g_FocusCret = Self) or ((Self = g_MySelf) and g_boSelectMySelf) then begin // and g_boSelectMySelf
      m_sNameText := m_sDescUserName + '\' + StrUserName;
    end;
  end;

  // 尝试修复npc乱码，chongchong 2016-02-23 原来与 NameTimeTick 相关的是 30s，现改为 3s NameTimeTick > 1000 * 3
  Result := (CompareText(m_sCurNameText, m_sNameText) <> 0) {or (MyGetTickCount - m_dwShowNameTimeTick >= 1000 * 2)};

  // 尝试修复npc乱码，chongchong 2016-02-23 原来与 NameTimeTick 相关的是 30s，现改为 3s NameTimeTick > 1000 * 3
  if m_boShopStall then begin
    if (CompareText(m_sCurShopNameText, m_sShopNameText) <> 0) or (MyGetTickCount - m_dwShowShopNameTimeTick >= 1000 * 2) then begin
      Result := True;
    end;
  end;
end;

function THumActor.CheckLoadDressAddEffect:Boolean;
var
  prv:Integer;
begin
  if (m_nDressAddEffectIndex >= 0) {and (m_wDressAddEffectOffSet >= 0)} and (m_wDressAddEffectCount > 0) //HZQ m_wDressAddEffectOffSet:WORD
  and (m_wDressAddEffectTime > 0) then begin
    prv := m_nDressAddEffectCurIndex;
    if (MyGetTickCount - m_nDressAddEffectLastTick > m_wDressAddEffectTime) then begin
      m_nDressAddEffectLastTick := MyGetTickCount;
      Inc(m_nDressAddEffectCurIndex);
      if m_nDressAddEffectCurIndex >= m_wDressAddEffectCount then
        m_nDressAddEffectCurIndex := 0;
    end;
    Result := prv <> m_nDressAddEffectCurIndex;
  end else begin
    m_DressAddEffectSurface := nil;
    Result := False; //HZQ 20230525
  end;
end;

procedure THumActor.LoadDressAddEffect;
var
  GameImages:TGameImages;
begin
  m_DressAddEffectSurface := nil;
  if (m_nDressAddEffectIndex >= 0) {and (m_wDressAddEffectOffSet >= 0)} and (m_wDressAddEffectCount > 0) //HZQ m_wDressAddEffectOffSet:WORD
  and (m_wDressAddEffectTime > 0) and (m_nDressAddEffectCurIndex >= 0) then begin
    g_EffectImageList.Lock;
    try
      if (m_nDressAddEffectIndex >= 0) and (m_nDressAddEffectIndex < g_EffectImageList.Count) then begin
        GameImages := TGameImages(g_EffectImageList.Objects[m_nDressAddEffectIndex]);
        if GameImages <> nil then begin
          case m_ColorEffect of
            ceGrayScale:m_DressAddEffectSurface := GameImages.GetCachedGrayImage(m_wDressAddEffectOffSet + m_nDressAddEffectCurIndex, m_nDressAddEffectX, m_nDressAddEffectY);
            ceBright:m_DressAddEffectSurface := GameImages.GetCachedBrightImage(m_wDressAddEffectOffSet + m_nDressAddEffectCurIndex, m_nDressAddEffectX, m_nDressAddEffectY);
            else
              m_DressAddEffectSurface := GameImages.GetCachedImage(m_wDressAddEffectOffSet + m_nDressAddEffectCurIndex, m_nDressAddEffectX, m_nDressAddEffectY);
          end;
        end;
      end
    finally
      g_EffectImageList.UnLock;
    end;
  end;
end;

function THumActor.CheckLoadSurface:Boolean;
var
  boLoadSurface:Boolean;
  prv, nPrv:Integer;
begin
  Result := False;
  if (not GameCanvas.Active) or (not GameCanvas.Initialized) then Exit;
  boLoadSurface := False;

  m_ColorEffect := GetDrawEffectValue;
  if (MyGetTickCount - m_dwLoadSurfaceTime >= 2 * 1000) or m_boLoadSurface or
    ((m_OldColorEffect <> m_ColorEffect) and ((m_ColorEffect in [ceGrayScale, ceBright]) or (m_OldColorEffect in [ceGrayScale, ceBright]))) then begin
    boLoadSurface := True;
  end;
  m_OldColorEffect := m_ColorEffect;

  if (m_nState and $00100000 <> 0) and (not boLoadSurface) then begin // 魔法盾效果   STATE_BUBBLEDEFENCEUP
    if not g_ClientConfig.boSkill31UseNewEffect then begin
      if (m_nCurrentAction = SM_STRUCK) and (m_nCurBubbleStruck < 3) then
        prv := MAGBUBBLESTRUCKBASE + m_nCurBubbleStruck
      else
        prv := MAGBUBBLEBASE + (m_nGenAniCount mod 3);
    end
    else {// 使用新魔法盾效果  chongchong 2015-07-25} begin
      if (m_nCurrentAction = SM_STRUCK) and (m_nCurBubbleStruck < 3) then
        prv := 920 + m_nCurBubbleStruck
      else
        prv := 910 + (m_nGenAniCount mod 3);
    end;

    if TimeGetTime - m_dwGenAnicountTime > 120 then begin
      m_dwGenAnicountTime := TimeGetTime;
      Inc(m_nGenAniCount);
      if m_nGenAniCount > 100000 then m_nGenAniCount := 0;
      Inc(m_nCurBubbleStruck);
      if m_nCurBubbleStruck > 100000 then m_nCurBubbleStruck := 0;
    end;

    if not g_ClientConfig.boSkill31UseNewEffect then begin
      if (m_nCurrentAction = SM_STRUCK) and (m_nCurBubbleStruck < 3) then
        nPrv := MAGBUBBLESTRUCKBASE + m_nCurBubbleStruck
      else
        nPrv := MAGBUBBLEBASE + (m_nGenAniCount mod 3);
    end
    else {// 使用新魔法盾效果  chongchong 2015-07-25} begin
      if (m_nCurrentAction = SM_STRUCK) and (m_nCurBubbleStruck < 3) then
        nPrv := 920 + m_nCurBubbleStruck
      else
        nPrv := 910 + (m_nGenAniCount mod 3);
    end;

    if prv <> nPrv then
      boLoadSurface := True;
  end;

  // 新武力盾效果 STATE_NEWHITBUBBLEDEFENCEUP
  if (m_nState and $00040000 <> 0) and (not boLoadSurface) then begin
    if TimeGetTime - m_dwGenNewHitAnicountTime > 120 then begin
      m_dwGenNewHitAnicountTime := TimeGetTime;
      Inc(m_nGenNewHitAniCount);
      if m_nGenNewHitAniCount > 100000 then m_nGenNewHitAniCount := 0;

      Inc(m_nCurNewHitBubbleStruck);
      if m_nCurNewHitBubbleStruck > 100000 then m_nCurNewHitBubbleStruck := 0;

      boLoadSurface := True;
    end;
  end;

  // 新道力盾效果 STATE_NEWMAGBUBBLEDEFENCEUP
  if (m_nState and $00020000 <> 0) and (not boLoadSurface) then begin
    if (m_nCurrentAction = SM_STRUCK) and (m_nCurNewMagBubbleStruck < 3) then
      prv := 723 + m_nCurNewMagBubbleStruck
    else
      prv := 720 + (m_nGenNewMagAniCount mod 3);

    if TimeGetTime - m_dwGenNewMagAnicountTime > 120 then begin
      m_dwGenNewMagAnicountTime := TimeGetTime;
      Inc(m_nGenNewMagAniCount);

      if m_nGenNewMagAniCount > 100000 then m_nGenNewMagAniCount := 0;
      Inc(m_nCurNewMagBubbleStruck);
    end;

    if (m_nCurrentAction = SM_STRUCK) and (m_nCurNewMagBubbleStruck < 3) then
      nPrv := 723 + m_nCurNewMagBubbleStruck
    else
      nPrv := 720 + (m_nGenNewMagAniCount mod 3);

    if prv <> nPrv then
      boLoadSurface := True;
  end;

  if m_boWeaponEffect then begin // 武器破碎效果
    prv := m_nCurWeaponEffect;
    if TimeGetTime - m_dwWeaponpEffectTime > 120 then begin
      m_dwWeaponpEffectTime := TimeGetTime;
      Inc(m_nCurWeaponEffect);
      if m_nCurWeaponEffect >= MAXWPEFFECTFRAME then begin
        m_boWeaponEffect := False;
        boLoadSurface := True;
      end;
    end;
    if prv <> m_nCurWeaponEffect then
      boLoadSurface := True;
  end;

  if m_boBrokenShield then begin
    prv := m_nBrokenShieldEffect;
    if TimeGetTime - m_dwBrokenShieldEffectTime > 30 then begin
      m_dwBrokenShieldEffectTime := TimeGetTime;
      Inc(m_nBrokenShieldEffect);
      if m_nBrokenShieldEffect >= 36 then begin
        m_boBrokenShield := False;
        boLoadSurface := True;
      end;
    end;
    if prv <> m_nBrokenShieldEffect then
      boLoadSurface := True;
  end;

  if m_boHitEndEffect then begin // 人物攻击动作结束后，攻击魔法效果
    prv := m_nCurrentHitFrame;
    if (m_nCurrentHitFrame < m_nStartHitFrame) or (m_nCurrentHitFrame > m_nEndHitFrame) then
      m_nCurrentHitFrame := m_nStartHitFrame;

    if TimeGetTime - m_dwStartHitTime > m_dwHitFrameTime then begin
      m_dwStartHitTime := TimeGetTime;
      if m_nCurrentHitFrame < m_nEndHitFrame then begin
        Inc(m_nCurrentHitFrame);
      end
      else begin
        m_boHitEndEffect := False;
        boLoadSurface := True;
      end;
    end;
    if prv <> m_nCurrentHitFrame then
      boLoadSurface := True;
  end;

  if m_boMagicEndEffect then begin // 人物魔法攻击动作结束后，攻击魔法效果
    prv := m_nCurrentMagicFrame;
    if (m_nCurrentMagicFrame < m_nStartMagicFrame) or (m_nCurrentMagicFrame > m_nEndMagicFrame) then
      m_nCurrentMagicFrame := m_nStartMagicFrame;

    if TimeGetTime - m_dwStartMagicTime > m_dwMagicFrameTime then begin
      m_dwStartMagicTime := TimeGetTime;
      if m_nCurrentMagicFrame < m_nEndMagicFrame then begin
        Inc(m_nCurrentMagicFrame);
      end
      else begin
        m_boMagicEndEffect := False;
        boLoadSurface := True;
      end;
    end;
    if prv <> m_nCurrentMagicFrame then
      boLoadSurface := True;
  end;

  if m_wOldHeroM2DressEffect <> m_wHeroM2DressEffect then begin
    m_wOldHeroM2DressEffect := m_wHeroM2DressEffect;
    boLoadSurface := True;
  end;

  if m_boOldHeroM2DressNoBlend <> m_boHeroM2DressNoBlend then begin
    m_boOldHeroM2DressNoBlend := m_boHeroM2DressNoBlend;
    boLoadSurface := True;
  end;

  Result := boLoadSurface;
end;

procedure THumActor.OnTargetExplosion(Sender:TObject);
var
  FlyEffect:TCustomMonFlyEffect;
begin
  if Sender is TCustomMonFlyEffect then begin
    FlyEffect := Sender as TCustomMonFlyEffect;

    if (Length(FlyEffect.ClientConfig.Sounds[custMagicExplosion]) > 0) then begin
      PlaySound(FlyEffect.ClientConfig.Sounds[custMagicExplosion]);
    end;
  end;
end;

procedure THumActor.OnTargetFinished(Sender:TObject);
var
  FlyEffect:TCustomMonFlyEffect;
  TargetEffect:TCustomMonTargetEffect;
  meff:TCustomMonTargetEffect;
  I:Integer;
  Actor:TActor;
  TargetEffImgLib:TGameImages;
begin
  if Sender is TCustomMonFlyEffect then begin
    FlyEffect := Sender as TCustomMonFlyEffect;

    if (FlyEffect.ClientConfig.Target_File >= 0) and (FlyEffect.ClientConfig.Target_File < g_EffectImageList.Count)
    //and ((FlyEffect.ClientConfig.Target_StartIndex >= 0) or (FlyEffect.ClientConfig.Target_StartIndex2 >= 0)) //hzq Target_StartIndex:WORD
    and (FlyEffect.ClientConfig.Target_PlayCount > 0)
      and ((not FlyEffect.ClientConfig.Target_KeepPlay) or (FlyEffect.ClientConfig.Target_KeepTime = 0)) then begin
      TargetEffImgLib := TGameImages(g_EffectImageList.Objects[FlyEffect.ClientConfig.Target_File]);

      Actor := PlayScene.FindActor(FlyEffect.LockTarget);
      if Actor <> nil then begin
        if FlyEffect.ClientConfig.Target_LockDraw then
          meff := TCustomMonTargetEffect.Create(FlyEffect.ClientConfig.Target_StartIndex, FlyEffect.ClientConfig.Target_StartIndex2, FlyEffect.ClientConfig.Target_PlayCount, Actor)
        else
          meff := TCustomMonTargetEffect.Create(FlyEffect.ClientConfig.Target_StartIndex, FlyEffect.ClientConfig.Target_StartIndex2, FlyEffect.ClientConfig.Target_PlayCount, Actor.m_nCurrX, Actor.m_nCurrY);

        TCustomMonTargetEffect(meff).DrawMode := FlyEffect.ClientConfig.Target_DrawMode;
        TCustomMonTargetEffect(meff).DrawMode2 := FlyEffect.ClientConfig.Target_DrawMode2;
        meff.ImgLib := TargetEffImgLib;
        meff.Light := FlyEffect.ClientConfig.Target_LightRange;
        meff.NextFrameTime := FlyEffect.ClientConfig.Target_PlayTime;
        PlayScene.AddEffectList(meff);
      end else if (FlyEffect.LockTargetX <> 0) or (FlyEffect.LockTargetY <> 0) then begin
        meff := TCustomMonTargetEffect.Create(FlyEffect.ClientConfig.Target_StartIndex, FlyEffect.ClientConfig.Target_StartIndex2, FlyEffect.ClientConfig.Target_PlayCount, FlyEffect.LockTargetX, FlyEffect.LockTargetY);

        TCustomMonTargetEffect(meff).DrawMode := FlyEffect.ClientConfig.Target_DrawMode;
        TCustomMonTargetEffect(meff).DrawMode2 := FlyEffect.ClientConfig.Target_DrawMode2;
        meff.ImgLib := TargetEffImgLib;
        meff.Light := FlyEffect.ClientConfig.Target_LightRange;
        meff.NextFrameTime := FlyEffect.ClientConfig.Target_PlayTime;
        PlayScene.AddEffectList(meff);
      end;

      for I := 0 to FlyEffect.FTargetList.Count - 1 do begin
        Actor := PlayScene.FindActor(PInt64(FlyEffect.FTargetList.Items[I])^);
        if Actor <> nil then begin
          if FlyEffect.ClientConfig.Target_LockDraw then
            meff := TCustomMonTargetEffect.Create(FlyEffect.ClientConfig.Target_StartIndex, FlyEffect.ClientConfig.Target_StartIndex2, FlyEffect.ClientConfig.Target_PlayCount, Actor)
          else
            meff := TCustomMonTargetEffect.Create(FlyEffect.ClientConfig.Target_StartIndex, FlyEffect.ClientConfig.Target_StartIndex2, FlyEffect.ClientConfig.Target_PlayCount, Actor.m_nCurrX, Actor.m_nCurrY);
          TCustomMonTargetEffect(meff).DrawMode := FlyEffect.ClientConfig.Target_DrawMode;
          TCustomMonTargetEffect(meff).DrawMode2 := FlyEffect.ClientConfig.Target_DrawMode2;
          meff.ImgLib := TargetEffImgLib;
          meff.Light := FlyEffect.ClientConfig.Target_LightRange;
          meff.NextFrameTime := FlyEffect.ClientConfig.Target_PlayTime;
          PlayScene.AddEffectList(meff);
        end;
      end;
    end;
  end else if Sender is TCustomMonTargetEffect then begin
    TargetEffect := Sender as TCustomMonTargetEffect;

    if (TargetEffect.ClientConfig.Target_File >= 0) and (TargetEffect.ClientConfig.Target_File < g_EffectImageList.Count)
    //and ((TargetEffect.ClientConfig.Target_StartIndex >= 0) or(TargetEffect.ClientConfig.Target_StartIndex2 >= 0)) //HZQ Target_StartIndex:WORD
    and (TargetEffect.ClientConfig.Target_PlayCount > 0)
      and ((not TargetEffect.ClientConfig.Target_KeepPlay) or (TargetEffect.ClientConfig.Target_KeepTime = 0)) then begin
      TargetEffImgLib := TGameImages(g_EffectImageList.Objects[TargetEffect.ClientConfig.Target_File]);

      Actor := PlayScene.FindActor(TargetEffect.LockTarget);
      //if Actor <> nil then
      begin
        if (Actor <> nil) and TargetEffect.ClientConfig.Target_LockDraw then
          meff := TCustomMonTargetEffect.Create(TargetEffect.ClientConfig.Target_StartIndex, TargetEffect.ClientConfig.Target_StartIndex2, TargetEffect.ClientConfig.Target_PlayCount, Actor)
        else begin
          if (Actor <> nil) then
            meff := TCustomMonTargetEffect.Create(TargetEffect.ClientConfig.Target_StartIndex, TargetEffect.ClientConfig.Target_StartIndex2, TargetEffect.ClientConfig.Target_PlayCount, Actor.m_nCurrX, Actor.m_nCurrY)
          else if (TargetEffect.LockX <> -1) and (TargetEffect.LockY <> -1) then
            meff := TCustomMonTargetEffect.Create(TargetEffect.ClientConfig.Target_StartIndex, TargetEffect.ClientConfig.Target_StartIndex2, TargetEffect.ClientConfig.Target_PlayCount, TargetEffect.LockX, TargetEffect.LockY)
          else
            Exit;
        end;

        TCustomMonTargetEffect(meff).DrawMode := TargetEffect.ClientConfig.Target_DrawMode;
        TCustomMonTargetEffect(meff).DrawMode2 := TargetEffect.ClientConfig.Target_DrawMode2;
        meff.ImgLib := TargetEffImgLib;
        meff.Light := TargetEffect.ClientConfig.Target_LightRange;
        meff.NextFrameTime := TargetEffect.ClientConfig.Target_PlayTime;
        PlayScene.AddEffectList(meff);
      end;

      for I := 0 to TargetEffect.FTargetList.Count - 1 do begin
        Actor := PlayScene.FindActor(PInt64(TargetEffect.FTargetList.Items[I])^);
        if Actor <> nil then begin
          if TargetEffect.ClientConfig.Target_LockDraw then
            meff := TCustomMonTargetEffect.Create(TargetEffect.ClientConfig.Target_StartIndex, TargetEffect.ClientConfig.Target_StartIndex2, TargetEffect.ClientConfig.Target_PlayCount, Actor)
          else
            meff := TCustomMonTargetEffect.Create(TargetEffect.ClientConfig.Target_StartIndex, TargetEffect.ClientConfig.Target_StartIndex2, TargetEffect.ClientConfig.Target_PlayCount, Actor.m_nCurrX, Actor.m_nCurrY);
          TCustomMonTargetEffect(meff).DrawMode := TargetEffect.ClientConfig.Target_DrawMode;
          TCustomMonTargetEffect(meff).DrawMode2 := TargetEffect.ClientConfig.Target_DrawMode2;
          meff.ImgLib := TargetEffImgLib;
          meff.Light := TargetEffect.ClientConfig.Target_LightRange;
          meff.NextFrameTime := TargetEffect.ClientConfig.Target_PlayTime;
          PlayScene.AddEffectList(meff);
        end;
      end;
    end;
  end;
end;

procedure THumActor.PlayMagicEffect(UseMagicInfo:PTUseMagicInfo; nTargetRecogID:Int64);
var
  CustomMagicConfig:PClientCustomMagicConfig;
  MagicPlusLevel:TMagicPlusLevel;
  ClientConfig:PMagicClientConfig;

  meff:TMagicEff;
  Actor:TActor;
  IntCurrentX, IntCurrentY, IntTargetX, IntTargetY:Integer;
  I, FlyDir:Integer;
  TempTargetRecog:Int64;
  I64:PInt64;
  FlyImgLib, FlyEffImgLib, ExplosionImgLib:TGameImages;
  TargetEffImgLib:TGameImages;
  Targets:TStringList;

  nX, nY:Integer;
  bofly:Boolean;
begin
  CustomMagicConfig := GetCustomMagicConfig(UseMagicInfo.MagicSerial);
  ClientConfig := nil;

  if CustomMagicConfig <> nil then begin
    case UseMagicInfo.NewLevel of
      0:MagicPlusLevel := mplNone;
      1..3:MagicPlusLevel := mpl1_3;
      4..6:MagicPlusLevel := mpl4_6;
      7..9:MagicPlusLevel := mpl7_9;
      else
        MagicPlusLevel := mpl7_9;
    end;

    ClientConfig := @CustomMagicConfig.MagicConfigs[MagicPlusLevel];
  end;

  if ClientConfig <> nil then begin
    if (ClientConfig.Self_File >= 0) and (ClientConfig.Self_File < g_EffectImageList.Count)
      {and (ClientConfig.Self_StartIndex >= 0)}and (ClientConfig.Self_PlayCount > 0) //HZQ Self_StartIndex:WORD
    and (ClientConfig.Self_DirCalcType = mdctCenter) and (ClientConfig.Self_PlayFailNoDraw)
      and (UseMagicInfo.ServerMagicCode > 0) then begin
      PlayScene.ScreenXYfromMCXY(m_nCurrX, m_nCurrY, nX, nY);
      if ClientConfig.Self_DirCount = mdcDir8 then
        FlyDir := 8
      else
        FlyDir := 16;

      for I := 0 to FlyDir - 1 do begin
        meff := TMagicEff.Create(111, 0, nX, nY, nX, nY, mtExplosion, True, 0);
        meff.MagExplosionBase := ClientConfig.Self_StartIndex + I * (ClientConfig.Self_PlayCount + ClientConfig.Self_EmptyCount);
        meff.TargetActor := nil;
        meff.NextFrameTime := ClientConfig.Self_PlayTime;
        meff.ExplosionFrame := ClientConfig.Self_PlayCount;

        if I = 1 then
          meff.Light := ClientConfig.Self_LightRange;

        meff.ImgLib := TGameImages(g_EffectImageList.Objects[ClientConfig.Self_File]);

        PlayScene.AddEffectList(meff);
      end;
    end;

    // 魔法失败时不要显示飞行或目标特效 chongchong 2015-05-09
    if (UseMagicInfo.ServerMagicCode > 0) then begin
      // 无飞行效果 chongchong 2015-03-22
      if (ClientConfig.Fly_File < 0) or (ClientConfig.Fly_File >= g_EffectImageList.Count)
        {or (ClientConfig.Fly_StartIndex < 0)}or (ClientConfig.Fly_PlayCount <= 0) then begin //HZQ Fly_StartIndex:WORD
        // 判断目标效果前奏 chongchong 2015-03-22
        if (ClientConfig.PreTarget_File >= 0) and (ClientConfig.PreTarget_File < g_EffectImageList.Count)
        //and ((ClientConfig.PreTarget_StartIndex >= 0) or(ClientConfig.PreTarget_StartIndex2 >= 0)) //HZQ PreTarget_StartIndex:WORD
        and (ClientConfig.PreTarget_PlayCount > 0) then begin
          TargetEffImgLib := TGameImages(g_EffectImageList.Objects[ClientConfig.PreTarget_File]);

          Actor := PlayScene.FindActor(nTargetRecogID);
          //if Actor <> nil then
          begin
            if (Actor <> nil) and ClientConfig.PreTarget_LockDraw then
              meff := TCustomMonTargetEffect.Create(ClientConfig.PreTarget_StartIndex, ClientConfig.PreTarget_StartIndex2, ClientConfig.PreTarget_PlayCount, Actor)
            else begin
              if Actor <> nil then
                meff := TCustomMonTargetEffect.Create(ClientConfig.PreTarget_StartIndex, ClientConfig.PreTarget_StartIndex2, ClientConfig.PreTarget_PlayCount, Actor.m_nCurrX, Actor.m_nCurrY)
              else
                meff := TCustomMonTargetEffect.Create(ClientConfig.PreTarget_StartIndex, ClientConfig.PreTarget_StartIndex2, ClientConfig.PreTarget_PlayCount, UseMagicInfo.targx, UseMagicInfo.targy)
            end;
            TCustomMonTargetEffect(meff).DrawMode := ClientConfig.PreTarget_DrawMode;
            TCustomMonTargetEffect(meff).DrawMode2 := ClientConfig.PreTarget_DrawMode2;
            meff.ImgLib := TargetEffImgLib;
            meff.Light := ClientConfig.PreTarget_LightRange;
            meff.NextFrameTime := ClientConfig.PreTarget_PlayTime;

            if Length(m_Saying) > 0 then begin
              Targets := TStringList.Create;
              try
                Targets.Delimiter := ',';
                Targets.DelimitedText := m_Saying;
                for I := 0 to Targets.Count - 1 do begin
                  TempTargetRecog := StrToInt64Def(Trim(Targets[I]), 0);
                  if TempTargetRecog <> 0 then begin
                    New(I64);
                    I64^ := TempTargetRecog;
                    TCustomMonTargetEffect(meff).FTargetList.Add(I64);
                  end;
                end;
              finally
                Targets.Free;
              end;
              { 代码移到下面了 chongchong 2015-05-03
              TCustomMonTargetEffect(meff).LockTarget := nTargetRecogID;
              TCustomMonTargetEffect(meff).ClientConfig := ClientConfig^;
              TCustomMonTargetEffect(meff).OnFinished := OnTargetFinished;
              }
            end;

            TCustomMonTargetEffect(meff).LockX := -1;
            TCustomMonTargetEffect(meff).LockY := -1;
            TCustomMonTargetEffect(meff).LockTarget := nTargetRecogID;
            if nTargetRecogID = 0 then begin
              TCustomMonTargetEffect(meff).LockX := UseMagicInfo.targx;
              TCustomMonTargetEffect(meff).LockY := UseMagicInfo.targy;
            end;
            TCustomMonTargetEffect(meff).ClientConfig := ClientConfig^;
            TCustomMonTargetEffect(meff).OnFinished := OnTargetFinished;

            PlayScene.AddEffectList(meff);

            if (Length(ClientConfig.Sounds[custMagicExplosion]) > 0) then begin
              PlaySound(ClientConfig.Sounds[custMagicExplosion]);
            end;
          end;
        end else if (ClientConfig.Target_File >= 0) and (ClientConfig.Target_File < g_EffectImageList.Count)
          //and ((ClientConfig.Target_StartIndex >= 0) or(ClientConfig.Target_StartIndex2 >= 0)) //Target_StartIndex:WORD
        and (ClientConfig.Target_PlayCount > 0) // 持续播放不要播放特效，地图特效会处理 chongchong 2015-03-21
        and ((not ClientConfig.Target_KeepPlay) or (ClientConfig.Target_KeepTime = 0)) then begin //只有目标效果，无前奏 chongchong 2015-03-22
          TargetEffImgLib := TGameImages(g_EffectImageList.Objects[ClientConfig.Target_File]);

          Actor := PlayScene.FindActor(nTargetRecogID);
          //if Actor <> nil then
          begin
            if (Actor <> nil) and ClientConfig.Target_LockDraw then
              meff := TCustomMonTargetEffect.Create(ClientConfig.Target_StartIndex, ClientConfig.Target_StartIndex2, ClientConfig.Target_PlayCount, Actor)
            else begin
              if Actor <> nil then
                meff := TCustomMonTargetEffect.Create(ClientConfig.Target_StartIndex, ClientConfig.Target_StartIndex2, ClientConfig.Target_PlayCount, Actor.m_nCurrX, Actor.m_nCurrY)
              else
                meff := TCustomMonTargetEffect.Create(ClientConfig.Target_StartIndex, ClientConfig.Target_StartIndex2, ClientConfig.Target_PlayCount, UseMagicInfo.targx, UseMagicInfo.targy)
            end;
            TCustomMonTargetEffect(meff).DrawMode := ClientConfig.Target_DrawMode;
            TCustomMonTargetEffect(meff).DrawMode2 := ClientConfig.Target_DrawMode2;
            meff.ImgLib := TargetEffImgLib;
            meff.Light := ClientConfig.Target_LightRange;
            meff.NextFrameTime := ClientConfig.Target_PlayTime;
            PlayScene.AddEffectList(meff);
          end;

          if (Length(ClientConfig.Sounds[custMagicExplosion]) > 0) then begin
            PlaySound(ClientConfig.Sounds[custMagicExplosion]);
          end;

          if Length(m_Saying) > 0 then begin
            Targets := TStringList.Create;
            try
              Targets.Delimiter := ',';
              Targets.DelimitedText := m_Saying;
              for I := 0 to Targets.Count - 1 do begin
                TempTargetRecog := StrToInt64Def(Trim(Targets[I]), 0);
                if TempTargetRecog > 0 then begin
                  Actor := PlayScene.FindActor(TempTargetRecog);
                  if Actor <> nil then begin
                    if ClientConfig.Target_LockDraw then
                      meff := TCustomMonTargetEffect.Create(ClientConfig.Target_StartIndex, ClientConfig.Target_StartIndex2, ClientConfig.Target_PlayCount, Actor)
                    else
                      meff := TCustomMonTargetEffect.Create(ClientConfig.Target_StartIndex, ClientConfig.Target_StartIndex2, ClientConfig.Target_PlayCount, Actor.m_nCurrX, Actor.m_nCurrY);
                    TCustomMonTargetEffect(meff).DrawMode := ClientConfig.Target_DrawMode;
                    TCustomMonTargetEffect(meff).DrawMode2 := ClientConfig.Target_DrawMode2;
                    meff.ImgLib := TargetEffImgLib;
                    meff.Light := ClientConfig.Target_LightRange;
                    meff.NextFrameTime := ClientConfig.Target_PlayTime;
                    PlayScene.AddEffectList(meff);
                  end;
                end;
              end;
            finally
              Targets.Free;
            end;
          end;
        end;
      end else if (ClientConfig.Fly_File >= 0) and (ClientConfig.Fly_File < g_EffectImageList.Count)
        {and (ClientConfig.Fly_StartIndex >= 0)}and (ClientConfig.Fly_PlayCount > 0) then begin //HZQ Fly_StartIndex:WORD
        Actor := PlayScene.FindActor(nTargetRecogID);
        //if Actor = nil then Exit;

        PlayScene.ScreenXYfromMCXY(m_nCurrX, m_nCurrY, IntCurrentX, IntCurrentY);
        PlayScene.ScreenXYfromMCXY(UseMagicInfo.targx, UseMagicInfo.targy, IntTargetX, IntTargetY);

        FlyDir := 0;
        if ClientConfig.Fly_CalcDir then begin
          if ClientConfig.Fly_DirCount = mdcDir8 then
            FlyDir := GetFlyDirection(IntCurrentX, IntCurrentY, IntTargetX, IntTargetY)
          else
            FlyDir := GetFlyDirection16(IntCurrentX, IntCurrentY, IntTargetX, IntTargetY);
        end;

        FlyImgLib := TGameImages(g_EffectImageList.Objects[ClientConfig.Fly_File]);

        FlyEffImgLib := nil;
        if {(ClientConfig.FlyEff_StartIndex >= 0) and}(ClientConfig.FlyEff_File >= 0) //HZQ FlyEff_StartIndex:WORD
        and (ClientConfig.Fly_File < g_EffectImageList.Count) then begin
          FlyEffImgLib := TGameImages(g_EffectImageList.Objects[ClientConfig.FlyEff_File])
        end;

        ExplosionImgLib := nil;
        if (ClientConfig.PreTarget_PlayCount > 0) and (ClientConfig.PreTarget_File >= 0)
        //and ((ClientConfig.PreTarget_StartIndex >= 0) or(ClientConfig.PreTarget_StartIndex2 >= 0)) //HZQ PreTarget_StartIndex:word
        and (ClientConfig.PreTarget_File < g_EffectImageList.Count) then begin
          ExplosionImgLib := TGameImages(g_EffectImageList.Objects[ClientConfig.PreTarget_File]);
        end;

        meff := TCustomMonFlyEffect.Create(ClientConfig.Fly_StartIndex + FlyDir * (ClientConfig.Fly_PlayCount + ClientConfig.Fly_EmptyCount),
          IntCurrentX, IntCurrentY, IntTargetX, IntTargetY, Actor, ClientConfig.Fly_PlayCount, ExplosionImgLib, ClientConfig.PreTarget_LockDraw, ClientConfig.Fly_FireGunMode);

        TCustomMonFlyEffect(meff).ClientConfig := ClientConfig^;

        //if (ClientConfig.PreTarget_File >= 0) and (ClientConfig.PreTarget_File < g_EffectImageList.Count) and
        //  ((ClientConfig.PreTarget_StartIndex >= 0) or (ClientConfig.PreTarget_StartIndex2 >= 0)) and (ClientConfig.PreTarget_PlayCount > 0) then
        begin
          if Length(m_Saying) > 0 then begin
            Targets := TStringList.Create;
            try
              Targets.Delimiter := ',';
              Targets.DelimitedText := m_Saying;
              for I := 0 to Targets.Count - 1 do begin
                TempTargetRecog := StrToInt64Def(Trim(Targets[I]), 0);
                if TempTargetRecog > 0 then begin
                  New(I64);
                  I64^ := TempTargetRecog;
                  TCustomMonFlyEffect(meff).FTargetList.Add(I64);
                end;
              end;
            finally
              Targets.Free;
            end;
          end;
          TCustomMonFlyEffect(meff).LockTarget := nTargetRecogID;

          TCustomMonFlyEffect(meff).LockTargetX := UseMagicInfo.targx;
          TCustomMonFlyEffect(meff).LockTargetY := UseMagicInfo.targy;

          TCustomMonFlyEffect(meff).OnFinished := OnTargetFinished;
        end;

        if (Length(ClientConfig.Sounds[cmstMagicFly]) > 0) then begin
          PlaySound(ClientConfig.Sounds[cmstMagicFly]);
        end;

        TCustomMonFlyEffect(meff).FlyDrawMode := ClientConfig.Fly_DrawMode;
        TCustomMonFlyEffect(meff).FlyLightRange := ClientConfig.Fly_LightRange;
        TCustomMonFlyEffect(meff).ExplosionLightRange := ClientConfig.PreTarget_LightRange;
        TCustomMonFlyEffect(meff).OnExplosion := OnTargetExplosion;

        meff.ImgLib := FlyImgLib;
        meff.Light := 1;
        meff.MagExplosionBase := ClientConfig.PreTarget_StartIndex;
        TCustomMonFlyEffect(meff).MagExplosionBase2 := ClientConfig.PreTarget_StartIndex2;
        meff.ExplosionFrame := ClientConfig.PreTarget_PlayCount;
        TCustomMonFlyEffect(meff).MagicBlend := ClientConfig.PreTarget_DrawMode = mdmBlend;
        TCustomMonFlyEffect(meff).MagicBlend2 := ClientConfig.PreTarget_DrawMode2 = mdmBlend;
        meff.NextFrameTime := ClientConfig.Fly_PlayTime;

        TCustomMonFlyEffect(meff).NextExplosionFrameTime := ClientConfig.PreTarget_PlayTime;
        TCustomMonFlyEffect(meff).FlyEffImgLib := FlyEffImgLib;
        TCustomMonFlyEffect(meff).FlyEffStartIndex := ClientConfig.FlyEff_StartIndex + FlyDir * (ClientConfig.Fly_PlayCount + ClientConfig.Fly_EmptyCount);
        TCustomMonFlyEffect(meff).FlyEffDrawMode := ClientConfig.FlyEff_DrawMode;

        if (meff <> nil) then begin
          meff.TargetRx := UseMagicInfo.targx;
          meff.TargetRy := UseMagicInfo.targy;
          if meff.TargetActor <> nil then begin
            meff.TargetRx := TActor(meff.TargetActor).m_nCurrX;
            meff.TargetRy := TActor(meff.TargetActor).m_nCurrY;
          end;
          meff.MagOwner := Self;

          {$IF IsMultiThreadRender = 1}
          PlayScene.m_EffectList.Lock;
          {$IFEND}
          PlayScene.m_EffectList.Add(meff);
          {$IF IsMultiThreadRender = 1}
          PlayScene.m_EffectList.UnLock;
          {$IFEND}

        end;
      end;
    end;
  end
  else if (UseMagicInfo.ServerMagicCode > 0) {and (UseMagicInfo.EffectNumber > 0)} then begin
    // 4级技能强化 -- 魔法效果 chongchong 2013-12-04
    PlayScene.NewMagic(Self,
      UseMagicInfo.ServerMagicCode,
      UseMagicInfo.EffectNumber,
      m_nCurrX,
      m_nCurrY,
      UseMagicInfo.targx,
      UseMagicInfo.targy,
      UseMagicInfo.target,
      UseMagicInfo.EffectType,
      UseMagicInfo.Recusion,
      UseMagicInfo.anitime,
      bofly, UseMagicInfo.NewLevel, UseMagicInfo.MagicItemType, False, UseMagicInfo.MagicLevel);

    if bofly then
      g_PlaySound.PlaySound(m_nMagicFireSound)
    else
      g_PlaySound.PlaySound(m_nMagicExplosionSound);
    // 倚天辟地屏幕震动 piaoyun 2013-09-14
    if (g_ConfigDlg.ConfigCheckeds[ckSceneShake]) then begin
      if (UseMagicInfo.MagicSerial = 114) {倚天辟地} then
        PlayScene.SceneShake();
      {
      去掉法道血魂一击屏幕震动 2019-12-21 14:35:37
      else if UseMagicInfo.MagicSerial in [116, 117] then
        PlayScene.SceneShake(1, 600);
      }
    end;
  end;
end;

procedure THumActor.Run;

  function MagicTimeOut:Boolean;
  begin
    if Self = g_MySelf then begin
       Result := TimeGetTime - m_dwWaitMagicRequest > 3000;
    end else begin
       Result := TimeGetTime - m_dwWaitMagicRequest > 2000;
    end;
    
    if Result then begin
        m_CurMagic.ServerMagicCode := 0;
    end;
  end;
var
  prv, nPrv:Integer;
  m_dwFrameTimetime:longword;
  boLoadSurface:Boolean;
  m_dwEffectFrameTimetime:LongWord;

  EndCounter:LongWord;
  boCustomHitContinue, IsMagicTimeOut:Boolean;
  CustomMagicConfig:PClientCustomMagicConfig;
begin
  boLoadSurface := False;
  if m_boUseEffect and (m_nCurrentAction = 0) then begin // 跟随人物动作的效果
    m_dwEffectFrameTimetime := m_dwEffectFrameTime;
    if TimeGetTime - m_dwEffectStartTime > m_dwEffectFrameTimetime then begin
      m_dwEffectStartTime := TimeGetTime;
      if m_nEffectFrame < m_nEffectEnd then begin
        Inc(m_nEffectFrame);
        boLoadSurface := True;
      end else begin
        m_boUseEffect := False;
      end;
    end;
  end;

  boCustomHitContinue := False;

  if (m_nCurrentAction = SM_WALK) or (m_nCurrentAction = SM_BACKSTEP) or (m_nCurrentAction = SM_RUN)
  or (m_nCurrentAction = SM_HORSERUN) or (m_nCurrentAction = SM_RUSH) or (m_nCurrentAction = SM_RUSHKUNG)
  or (m_nCurrentAction = SM_MAGICMOVE) {十步一杀} or (m_nCurrentAction = SM_100HIT)
  or ((m_nCurrentAction >= SM_CUSTOM_PUSH001) and (m_nCurrentAction < SM_CUSTOM_PUSH001 + CUSTOM_MAGIC_COUNT))
     or ((m_nCurrentAction >= SM_CUSTOM_MAGICMOVE001) and (m_nCurrentAction < SM_CUSTOM_MAGICMOVE001 + CUSTOM_MAGIC_COUNT))
  or boCustomHitContinue then begin
    if boLoadSurface then begin
      m_dwLoadSurfaceTime := MyGetTickCount;
      PlayScene.LoadSurface(LoadSurface);
    end;
    Exit;
  end;

  boLoadSurface := boLoadSurface or CheckLoadSurface;

  // 修正战士跑去攻击目标时前三刀过快 chongchong 2015-08-04
  m_boMsgMuch := (Self <> g_MySelf) and (m_MsgList.Count >= 2); {and (m_btRace <> RC_HEROOBJECT)}
  //;

  {
  // 修正消息处理不及时的判断 chongchong 2016-05-04
  if Self <> g_MySelf then
  begin
    if (m_nCurrentAction in [SM_RUN, SM_WALK]) and (m_MsgList.Count = 1) then
    begin
      m_MsgList.Lock;
      try
        PMsg := m_MsgList.Items[0];
        if PMsg.Ident in [SM_RUN, SM_WALK] then
        begin
          m_boMsgMuch := True;
        end;
      finally
        m_MsgList.UnLock;
      end;
    end
    else
      m_boMsgMuch := (m_MsgList.Count >= 2);
  end
  else
  begin
    m_boMsgMuch := False;
  end;
  }

  RunActSound(m_nCurrentFrame - m_nStartFrame);
  RunFrameAction(m_nCurrentFrame - m_nStartFrame);

  nPrv := m_nCurEffFrame;
  prv := m_nCurrentFrame;
  if m_nCurrentAction <> 0 then begin
    if (m_nCurrentFrame < m_nStartFrame) or (m_nCurrentFrame > m_nEndFrame) then
      m_nCurrentFrame := m_nStartFrame;

    if (Self <> g_MySelf) and (m_boUseMagic) then begin
      // 修正英雄举手速度比人物快 chongchong 2017-11-18
      //m_dwFrameTimetime := Round(m_dwFrameTime * 2 / 3);

      if m_boMsgMuch then
        m_dwFrameTimetime := Round(m_dwFrameTime * 2 / 3)
      else begin
        if Self = g_MyHero then
          m_dwFrameTimetime := m_dwFrameTime
        else
          m_dwFrameTimetime := Round(m_dwFrameTime * 2 / 3)
      end;
    end else begin
      if m_boMsgMuch then
        m_dwFrameTimetime := Round(m_dwFrameTime * 2 / 3)
      else
        m_dwFrameTimetime := m_dwFrameTime;
    end;

    EndCounter := timeGetTime;

    if (EndCounter - m_StartCounter) > m_dwFrameTimetime then
      {//if TimeGetTime - m_dwStartTime > m_dwFrameTimetime then} begin
      if m_nCurrentAction = SM_102HIT then // 断岳斩 恢复播放速度
        m_dwFrameTime := 100;

      if m_nCurrentFrame < m_nEndFrame then begin
        if m_boUseMagic then begin
          IsMagicTimeOut := MagicTimeOut;
          if (m_nCurEffFrame = m_nSpellFrame - 2) or IsMagicTimeOut then begin
            // 修正自定义技能举手卡 or CheckIsCustomMagic(m_CurMagic.MagicSerial)  chongchong 2016-08-31
            // 修正法师连击技能举手卡 凤舞祭 惊雷爆 冰天雪地 双龙破  or (m_CurMagic.MagicSerial in [104..107]) chongchong 2017-11-17
            // 修正英雄举手卡 or ((m_CurMagic.ServerMagicCode = -1) and (m_btRace = 1)) chongchong 2017-11-18
            // 修正自动合击举手卡 chongchong 2018-06-25 16:43:48
            if (m_CurMagic.ServerMagicCode >= 0) or (m_CurMagic.MagicSerial in [60..65, 104..107]) or (CheckIsCustomMagic(m_CurMagic.MagicSerial) and (TimeGetTime - m_dwStartTime >= 200)) or IsMagicTimeOut or ((m_CurMagic.ServerMagicCode = -1) and (m_btRace = 1)) then begin
              (*
              if CheckIsCustomMagic(m_CurMagic.MagicSerial) and IsMagicTimeOut then
              begin
                DScreen.AddChatBoardString('技能超时.', GetRGB(g_ClientConfig.btGetExpMsgFColor), GetRGB(g_ClientConfig.btGetExpMsgBColor))
              end;
              *)

              Inc(m_nCurrentFrame);
              Inc(m_nCurEffFrame);
              m_dwStartTime := TimeGetTime;
              m_StartCounter := timeGetTime;
              m_StartCounter := EndCounter;
              if m_boUseEffect then begin // 跟随人物动作的效果
                if m_nEffectFrame < m_nEffectEnd then begin
                  Inc(m_nEffectFrame);
                end
                else begin
                  m_boUseEffect := False;
                end;
              end;
            end else begin
            
            end;
          end else begin
            if m_nCurrentFrame < m_nEndFrame - 1 then begin
              Inc(m_nCurrentFrame);
              if m_boUseEffect then begin // 跟随人物动作的效果
                if m_nEffectFrame < m_nEffectEnd then begin
                  Inc(m_nEffectFrame);
                end
                else begin
                  m_boUseEffect := False;
                end;
              end;
            end;

            if m_nCurEffFrame < m_nSpellFrame - 1 then Inc(m_nCurEffFrame);
            m_dwStartTime := TimeGetTime;
            m_StartCounter := EndCounter;
          end;
        end else begin
          Inc(m_nCurrentFrame);
          {$IF DEBUG_HIT_DELAY = 1}
          DScreen.AddChatBoardString('帧间隔:' + FloatToStr((EndCounter - m_StartCounter) / g_Frequency * 1000) + '; ' + IntToStr(m_dwFrameTimetime), clWhite, clRed);
          {$IFEND}
          m_dwStartTime := TimeGetTime;
          m_StartCounter := EndCounter;
          if m_boUseEffect then begin // 跟随人物动作的效果
            if m_nEffectFrame < m_nEffectEnd then begin
              Inc(m_nEffectFrame);
            end else begin
              m_boUseEffect := False;
            end;
          end;
        end;
      end else begin // if m_nCurrentFrame < m_nEndFrame then begin
        if Self = g_MySelf then begin
          if frmMain.ServerAcceptNextAction(IsAttackAction(m_nCurrentAction)) then begin
            ActionEnded;

            // 修改自定义技能连击起始动作 cboHumDiy.wzl 回位时有帧错误  chongchong 2015-11-30
            boCustomHitContinue := False;
            if m_nCurrentAction = SM_SPELL then begin
              // 修正按下快捷键时间长点，会2次释放技能 2019-12-26
              frmMain.DoSpellEnd;

              CustomMagicConfig := GetCustomMagicConfig(m_CurMagic.MagicSerial);
              if CustomMagicConfig <> nil then begin
                boCustomHitContinue := (CustomMagicConfig.MagicBaseConfig.MagicActionType = matCustom) and CustomMagicConfig.MagicBaseConfig.MagicActionContinue;
              end;
            end else if (m_nCurrentAction >= SM_CUSTOM_HIT001) and (m_nCurrentAction < SM_CUSTOM_HIT001 + CUSTOM_MAGIC_COUNT) then begin
              CustomMagicConfig := GetCustomMagicConfig(Abs(m_nCurrentAction) - SM_CUSTOM_HIT001 + CUSTOM_MAGIC_START_ID);
              if CustomMagicConfig <> nil then begin
                boCustomHitContinue := (CustomMagicConfig.MagicBaseConfig.MagicActionType = matCustom) and CustomMagicConfig.MagicBaseConfig.MagicActionContinue;
              end;
            end else if (m_nCurrentAction >= SM_CUSTOM_PUSH001) and (m_nCurrentAction < SM_CUSTOM_PUSH001 + CUSTOM_MAGIC_COUNT) then begin
              CustomMagicConfig := GetCustomMagicConfig(Abs(m_nCurrentAction) - SM_CUSTOM_PUSH001 + CUSTOM_MAGIC_START_ID);
              if CustomMagicConfig <> nil then begin
                boCustomHitContinue := (CustomMagicConfig.MagicBaseConfig.MagicActionType = matCustom) and CustomMagicConfig.MagicBaseConfig.MagicActionContinue;
              end;
            end;

            m_nCurrentAction := 0;

            // 修改自定义技能连击起始动作 cboHumDiy.wzl 回位时有帧错误  chongchong 2015-11-30
            if boCustomHitContinue then begin
              DefaultMotion;
            end;

            {$IF TESTMODE = 1}
            //DScreen.AddChatBoardString('~~~~~~sm_spell' + IntToStr(m_CurMagic.EffectNumber), clRed, clBlack);
            {$IFEND}

            m_boUseMagic := False;
          end;
        end else begin
          ActionEnded;
          m_nCurrentAction := 0;
          m_boUseMagic := False;
        end;
        ActionChanged;

        if m_boHitEffect then begin
          // 当动作结束后，这些未执行又来了下一个动作，导致m_nHitEffectLevel = 0，这些都发生错误 chongchong 2017-11-27
          // 故些加一个 m_nHitEffectLevel_Old 来记录原来的值，免得下一个动作把这个未执行完的 m_nHitEffectLevel 赋值为 0
          // 这个地方不处理会卡技能动作 ★★★★★★ 2020-03-24 12:27:21
          m_nHitEffectLevel_Old := m_nHitEffectLevel;
          m_nHitEffectNumber_Old := m_nHitEffectNumber;

          // 修复开天斩随着角色飘 (按了开天的快捷后，迅速跑开） chongchong 2018-06-20 15:16:43
          m_nHitEndX := m_nRx;
          m_nHitEndY := m_nRy;

          // 人物攻击动作结束后，后续的攻击魔法效果
          if m_nHitEffectNumber = 4 then begin // 烈火剑法    ID=43
            if m_nHitEffectLevel >= 7 then begin
              m_btHitDir := m_btDir;
              m_nStartPosHitFrame := 6;

              // 修复开天斩随着角色飘 (按了开天的快捷后，迅速跑开） chongchong 2018-06-20 15:16:43
              //m_nHitEndX := m_nShiftX;
              //m_nHitEndY := m_nShiftY;

              m_nStartHitFrame := 0;
              m_nEndHitFrame := m_nStartHitFrame + 2;
              m_nCurrentHitFrame := m_nStartHitFrame;
              m_dwHitFrameTime := m_dwFrameTime;
              m_boHitEndEffect := True;
              m_dwStartHitTime := TimeGetTime;
            end;
          end else if m_nHitEffectNumber = 7 then begin // 雷霆剑法    ID=43
              m_btHitDir := m_btDir;
              m_nStartPosHitFrame := 8;

              // 修复开天斩随着角色飘 (按了开天的快捷后，迅速跑开） chongchong 2018-06-20 15:16:43
              //m_nHitEndX := m_nShiftX;
              //m_nHitEndY := m_nShiftY;

              m_nStartHitFrame := 0;
              m_nEndHitFrame := m_nStartHitFrame + 4;
              m_nCurrentHitFrame := m_nStartHitFrame;
              m_dwHitFrameTime := Round(m_dwFrameTime * 2 / 3);
              m_boHitEndEffect := True;
              m_dwStartHitTime := TimeGetTime;
            end else if m_nHitEffectNumber = 8 then begin // 龙影剑法
              m_btHitDir := m_btDir;
              m_nStartPosHitFrame := 8;

              // 修复开天斩随着角色飘 (按了开天的快捷后，迅速跑开） chongchong 2018-06-20 15:16:43
              //m_nHitEndX := m_nShiftX;
              //m_nHitEndY := m_nShiftY;

              m_nStartHitFrame := 0;
              m_nEndHitFrame := m_nStartHitFrame + 7;
              m_nCurrentHitFrame := m_nStartHitFrame;
              m_dwHitFrameTime := Round(m_dwFrameTime * 2 / 3);
              m_boHitEndEffect := True;
              m_dwStartHitTime := TimeGetTime;
            end else if m_nHitEffectNumber = 9 then begin // 破魂斩
              m_btHitDir := m_btDir;
              m_nStartPosHitFrame := 8;

              // 修复开天斩随着角色飘 (按了开天的快捷后，迅速跑开） chongchong 2018-06-20 15:16:43
              //m_nHitEndX := m_nShiftX;
              //m_nHitEndY := m_nShiftY;

              m_nStartHitFrame := 0;
              m_nEndHitFrame := m_nStartHitFrame + 10;
              m_nCurrentHitFrame := m_nStartHitFrame;
              // m_dwHitFrameTime := Round(m_dwFrameTime * 2 / 3);
              m_dwHitFrameTime := Round(m_dwFrameTime); // 破魂斩特效速度修改 piaoyun 2013-08-09
              m_boHitEndEffect := True;
              m_dwStartHitTime := TimeGetTime;
            end else if m_nHitEffectNumber = 12 then begin // 开天斩重击
              m_btHitDir := m_btDir;
              m_nStartPosHitFrame := 85;

              // 修复开天斩随着角色飘 (按了开天的快捷后，迅速跑开） chongchong 2018-06-20 15:16:43
              //m_nHitEndX := m_nShiftX;
              //m_nHitEndY := m_nShiftY;

              m_nStartHitFrame := 0;
              m_nEndHitFrame := m_nStartHitFrame + 4;
              m_nCurrentHitFrame := m_nStartHitFrame;
              m_dwHitFrameTime := 40; //Round(m_dwFrameTime * 2 / 3);
              m_boHitEndEffect := True;
              m_dwStartHitTime := TimeGetTime;
            end else if m_nHitEffectNumber = 14 then {// 逐日剑法} begin
              if m_nHitEffectLevel >= 7 then begin
                m_btHitDir := m_btDir;
                m_nStartPosHitFrame := 6;

                // 修复开天斩随着角色飘 (按了开天的快捷后，迅速跑开） chongchong 2018-06-20 15:16:43
                //m_nHitEndX := m_nShiftX;
                //m_nHitEndY := m_nShiftY;

                m_nStartHitFrame := 0;
                m_nEndHitFrame := m_nStartHitFrame + 2;
                m_nCurrentHitFrame := m_nStartHitFrame;
                m_dwHitFrameTime := m_dwFrameTime;
                m_boHitEndEffect := True;
                m_dwStartHitTime := TimeGetTime;
              end;
            end else if m_nHitEffectNumber = 25 then begin // 开天斩轻击
              m_btHitDir := m_btDir;
              m_nStartPosHitFrame := 85;

              // 修复开天斩随着角色飘 (按了开天的快捷后，迅速跑开） chongchong 2018-06-20 15:16:43
              //m_nHitEndX := m_nShiftX;
              //m_nHitEndY := m_nShiftY;

              m_nStartHitFrame := 0;
              m_nEndHitFrame := m_nStartHitFrame + 4;
              m_nCurrentHitFrame := m_nStartHitFrame;
              m_dwHitFrameTime := 40; // Round(m_dwFrameTime * 2 / 3);
              m_boHitEndEffect := True;
              m_dwStartHitTime := TimeGetTime;
            end else if m_nHitEffectNumber = 26 then begin // 断空斩
              m_btHitDir := m_btDir;
              m_nStartPosHitFrame := 85;

              // 修复开天斩随着角色飘 (按了开天的快捷后，迅速跑开） chongchong 2018-06-20 15:16:43
              //m_nHitEndX := m_nShiftX;
              //m_nHitEndY := m_nShiftY;

              m_nStartHitFrame := 0;
              m_nEndHitFrame := m_nStartHitFrame + 4;
              m_nCurrentHitFrame := m_nStartHitFrame;
              m_dwHitFrameTime := 40; //Round(m_dwFrameTime * 2 / 3);
              m_boHitEndEffect := True;
              m_dwStartHitTime := TimeGetTime;

              // 去掉断空斩屏幕振动 2019-12-21 14:36:40
              {
              if (g_ConfigDlg.ConfigCheckeds[ckSceneShake]) then
              begin
                PlayScene.SceneShake();
              end;
              }
            end;
        end;
        m_boHitEffect := False;
        boLoadSurface := True;
      end;

      if m_boUseMagic then begin
        if m_nCurEffFrame = m_nSpellFrame - 1 then begin
          if not m_boCreateEffect then begin
            m_boCreateEffect := True;

            m_boUseMagic := False;

            // 修正自定义技能举手后卡住 chongchong 2016-08-30
            // m_boWarMode := False;
            
            PlayMagicEffect(@m_CurMagic, m_nTargetRecog);

            if Self = g_MySelf then begin
              // DScreen.AddChatBoardString('g_dwLatestSpellTick :' + IntToStr(TimeGetTime-g_dwLatestSpellTick), clGreen, clWhite);
              g_boLatestSpell := False;
              g_dwLatestSpellTick := MyGetTickCount;
              frmMain.DoSpellEnd;

              // g_dwLatestSpellTick := MyGetTickCount +g_MySelf.UseMagicDelayTime;
            end;
            m_CurMagic.ServerMagicCode := 0;
          end;
        end else begin
          m_boCreateEffect := False;
        end;
      end;
    end;
    if m_btRace in [0, 1] then
      m_nCurrentDefFrame := 0
    else
      m_nCurrentDefFrame := -10;
    m_dwDefFrameTime := TimeGetTime;
  end else begin
    m_dwFrameTime := 200; // 无动作时恢复站立时的动作速度，防止天使之翼播放速度过快
    if TimeGetTime - m_dwSmoothMoveTime > 200 then begin
      if TimeGetTime - m_dwDefFrameTime > 500 then begin
        m_dwDefFrameTime := TimeGetTime;
        Inc(m_nCurrentDefFrame);
        if m_nCurrentDefFrame >= m_nDefFrameCount then
          m_nCurrentDefFrame := 0;
      end;
      if DefaultMotion then boLoadSurface := True;
    end;
  end;

  if (prv <> m_nCurrentFrame) or (nPrv <> m_nCurEffFrame) then
    boLoadSurface := True;

  if boLoadSurface then begin
    m_dwLoadSurfaceTime := MyGetTickCount;
    PlayScene.LoadSurface(LoadSurface);
  end;

  if CheckLoadUserName then LoadNameSurface;
  if CheckLoadNumberLable then LoadNumberLableSurface;
  if CheckLoadSay then LoadSaySurface;
  //if CheckLoadActorIcon then LoadActorIcons;
  //if CheckLoadHealthNumber then LoadHealthNumber;
  if CheckLoadFengHaoSurface then LoadFengHaoSurface; // 最原始只加了这里 修复一直跑称号显示错误 chongchong 2014-10-20
  //if CheckLoadPlayEffect then LoadPlayEffectSurface;
end;

function THumActor.light:Integer;
var
  L:Integer;
begin
  L := m_nChrLight;
  if L < m_nMagLight then begin
    if m_boUseMagic or m_boHitEffect then
      L := m_nMagLight;
  end;
  Result := L;
end;

procedure THumActor.LoadSurface(Sender:TObject);
var
  I, tdir, nWeaponIndex, nDress, nShape, nAppr, FileIdx, nEffectOffSet, nHairOffset, nEffectOffSet_30:Integer;
  GameImages:TGameImages;
  cboGameImages:TGameImages;
  nCurrentFrame:Integer;
  ActorEffect:pTClientActorEffect;
  sFileName, sTemp:string;

  nUnit, nHumEffectOffset, nMaxOffset:Integer;
  nIndex:Integer;

  nHumHorseSex:Integer;
  nHumHorseCount:Integer;
  nX, nY:Integer;
  boCustomHitContinue:Boolean;
  CustomMagicConfig:PClientCustomMagicConfig;

  SimpleShowShape:Integer;
  nFileIndex, nImgIndex:Integer;

  TempDir, TempOffset:Integer;
  MonsterConfig, TempMonsterConfig:PClientCustomMonsterConfig;
  ClientAction:PMonsterClientAction;
  giBody, giBodyEffect, giBodyEffect2:TGameImages;
  nBodyOffset:Integer;
  (*
  {$IF TESTMODE = 1}
  {$j+}
    const  Self_nCurrentFrame : integer = -1;               //声明静态变量
  {$j-}
  {$IFEND}
  *)
begin
  //  if Self = g_MySelf then begin
  //      OutputDebugString('Pause');
  //  end;

  m_dwLoadSurfaceTime := MyGetTickCount;
  m_boLoadSurface := False;
  m_BodySurface := nil;

  m_HairSurface := nil;
  m_WeaponSurface := nil;
  m_ShieldSurface := nil; // 盾牌 chongchong 2013-09-16

  m_HorseSurface := nil; // 骑马 chongchong 2013-10-12
  m_HorseWingsEffectSurface := nil; // 骑马 马特效 chongchong 2013-10-16
  m_HorseEffectSurface := nil;

  m_HorseHairSurface := nil; // 骑马 马上人物的头发 chongchong 2013-10-17
  m_HorseHumSurface := nil; // 骑马 马上人物 chongchong 2013-10-17

  m_HumWinSurface := nil;
  m_HumWinSurface_30 := nil;
  m_ShopStallSurface := nil;
  m_ShopHeadSurface := nil;

  m_WeaponEffectSurface := nil;
  m_DBWeaponEffectSurface := nil;
  m_DressEffectSurface := nil;
  m_MedalEffectSurface := nil;
  m_ShieldEffectSurface := nil;
  m_HeroM2DressEffect := nil;

  m_boDressEffectDrawNoBlend := False;
  m_boWeaponEffectDrawNoBlend := False;
  m_boShieldEffectDrawNoBlend := False;

  if m_nChangeAppr >= 0 then begin
    if m_nChangeAppr >= 100000 then begin
      MonsterConfig := nil;
      m_BodySurface := nil;

      for I := 0 to g_CustomMonsterConfig.Count - 1 do begin
        TempMonsterConfig := g_CustomMonsterConfig.Items[I];
        if TempMonsterConfig.wMonsterAppr = m_nChangeAppr - 100000 then begin
          MonsterConfig := TempMonsterConfig;
          Break;
        end;
      end;

      ClientAction := nil;
      if MonsterConfig <> nil then begin
        case m_nCurrentAction of
          0, SM_TURN:begin
              if (m_nState and STATE_STONE_MODE) <> 0 then
                ClientAction := @MonsterConfig.Actions[matStoneRevive]
              else
                ClientAction := @MonsterConfig.Actions[matStand];
            end;
          SM_WALK, SM_RUSH, SM_RUSHKUNG, SM_BACKSTEP:begin
              ClientAction := @MonsterConfig.Actions[matWalk];
            end;
          SM_DIGUP:begin
              ClientAction := @MonsterConfig.Actions[matStoneRevive];
            end;
          SM_LIGHTINGEX:begin

            end;
          SM_HIT:begin
              ClientAction := @MonsterConfig.Actions[matDefAttack];
            end;
          SM_STRUCK:begin
              ClientAction := @MonsterConfig.Actions[matStruck];
            end;
          SM_DEATH:begin
              ClientAction := @MonsterConfig.Actions[matDie];
            end;
          SM_NOWDEATH:begin
              ClientAction := @MonsterConfig.Actions[matDie];
            end;
          SM_SKELETON:begin

            end;
        end;
      end;

      if (ClientAction <> nil) and (ClientAction.StartIndex >= 0) and (ClientAction.PlayCount > 0) then begin
        if (ClientAction.ActionFile >= 0) and (ClientAction.ActionFile < g_EffectImageList.Count) then
          giBody := TGameImages(g_EffectImageList.Objects[ClientAction.ActionFile])
        else
          giBody := g_WMonImages.Images[m_nChangeAppr - 100000];
        nBodyOffset := m_nCurrentFrame;

        (*
        if (ClientAction.EffectIndex >= 0)  then
        begin
          if (ClientAction.EffectFile >= 0) and (ClientAction.EffectFile < g_EffectImageList.Count) then
            giBodyEffect := TGameImages(g_EffectImageList.Objects[ClientAction.EffectFile])
          else
            giBodyEffect := g_WMonImages.Images[m_nChangeAppr - 100000];
          //nBodyEffectOffset := ClientAction.EffectIndex + m_nCurrentFrame - ClientAction.StartIndex;

          // 修正计算动作特效不对 chongchong 2014-09-11
          if ClientAction.CalcDir then
          begin
            TempDir := m_btDir;
            TempOffset := (m_nCurrentFrame - ClientAction.StartIndex) mod (ClientAction.PlayCount + ClientAction.EmptyCount);

            if (ClientAction.ActionType = matDie) and (MonsterConfig.BaseConfig.DieNoCalcDir) then
              nBodyEffectOffset := ClientAction.EffectIndex + TempOffset
            else
              nBodyEffectOffset := ClientAction.EffectIndex + TempDir * (ClientAction.PlayCount + ClientAction.EmptyCount) + TempOffset;
          end
          else
          begin
            TempOffset := m_nCurrentFrame - ClientAction.StartIndex;
            nBodyEffectOffset := ClientAction.EffectIndex + TempOffset;
          end;
        end;

        if (ClientAction.EffectIndex2 >= 0)  then
        begin
          if (ClientAction.EffectFile2 >= 0) and (ClientAction.EffectFile2 < g_EffectImageList.Count) then
            giBodyEffect2 := TGameImages(g_EffectImageList.Objects[ClientAction.EffectFile2])
          else
            giBodyEffect2 := g_WMonImages.Images[m_nChangeAppr - 100000];
          //nBodyEffect2Offset := ClientAction.EffectIndex2 + m_nCurrentFrame - ClientAction.StartIndex;

          // 修正计算动作特效不对 chongchong 2014-09-11
          if ClientAction.CalcDir then
          begin
            TempDir := m_btDir;
            TempOffset := (m_nCurrentFrame - ClientAction.StartIndex) mod (ClientAction.PlayCount + ClientAction.EmptyCount);
            if (ClientAction.ActionType = matDie) and (MonsterConfig.BaseConfig.DieNoCalcDir) then
              nBodyEffect2Offset := ClientAction.EffectIndex2 + TempOffset
            else
              nBodyEffect2Offset := ClientAction.EffectIndex2 + TempDir * (ClientAction.PlayCount + ClientAction.EmptyCount) + TempOffset;
          end
          else
          begin
            TempOffset := m_nCurrentFrame - ClientAction.StartIndex;
            nBodyEffect2Offset := ClientAction.EffectIndex2 + TempOffset;
          end;
        end;
      *)
      end;

      if giBody <> nil then begin
        if (not m_boReverseFrame) then begin
          case m_ColorEffect of
            ceGrayScale, ceGrayScale2:m_BodySurface := giBody.GetCachedGrayImage(nBodyOffset, m_nPx, m_nPy);
            ceBright:m_BodySurface := giBody.GetCachedBrightImage(nBodyOffset, m_nPx, m_nPy);
            else
              m_BodySurface := giBody.GetCachedImage(nBodyOffset, m_nPx, m_nPy);
          end;
        end;
      end;

      (*
      if giBodyEffect <> nil then
      begin
        if (not m_boReverseFrame) then
        begin
          case m_ColorEffect of
            ceGrayScale: m_BodyEffectSurface := giBodyEffect.GetCachedGrayImage(nBodyEffectOffset, m_nEffectPx, m_nEffectPy);
            ceBright: m_BodyEffectSurface := giBodyEffect.GetCachedBrightImage(nBodyEffectOffset, m_nEffectPx, m_nEffectPy);
          else
            m_BodyEffectSurface := giBodyEffect.GetCachedImage(nBodyEffectOffset, m_nEffectPx, m_nEffectPy);
          end;
        end;
      end;

      if giBodyEffect2 <> nil then
      begin
        if (not m_boReverseFrame) then
        begin
          case m_ColorEffect of
            ceGrayScale: m_BodyEffect2Surface := giBodyEffect2.GetCachedGrayImage(nBodyEffect2Offset, m_nEffect2Px, m_nEffect2Py);
            ceBright: m_BodyEffect2Surface := giBodyEffect2.GetCachedBrightImage(nBodyEffect2Offset, m_nEffect2Px, m_nEffect2Py);
          else
            m_BodyEffect2Surface := giBodyEffect2.GetCachedImage(nBodyEffect2Offset, m_nEffect2Px, m_nEffect2Py);
          end;
        end;
      end;
      *)
    end
    else
      inherited LoadSurface(Sender);

    Exit;
  end;

  // 骑马 官方双人马 chongchong 2013-10-17 ----------------------------------------------------------
  if m_btHorse in [1, 2] then begin
    // 2 男载女, 3 男载男, 4 女载女, 5 女载男
    if m_ColorEffect = ceGrayScale then begin
      if (m_btHorseEffectType >= 1) and (m_btHorseEffectType <= 6) then
        m_HorseEffectSurface := g_WHorseImg2.GetCachedGrayImage(1920 + (m_btHorseEffectType - 1) * 320 + m_nCurrentFrame, m_nHorseEffectX, m_nHorseEffectY);

      if m_btHorse = 1 then begin
        if m_btDoubleHumHorse = 1 then begin
          m_HorseSurface := g_WHorseImg.GetCachedGrayImage(320 * m_btSex + m_nCurrentFrame, m_nPx, m_nPy)
        end
        else if m_btDoubleHumHorse in [2, 3, 4, 5] then
          m_HorseSurface := g_WHorseImg.GetCachedGrayImage(320 * m_btDoubleHumHorse + m_nCurrentFrame, m_nPx, m_nPy);
      end
      else if m_btHorse = 2 then begin
        if m_btDoubleHumHorse = 1 then begin
          m_HorseSurface := g_WHorseImg.GetCachedGrayImage(1920 + 320 * m_btSex + m_nCurrentFrame, m_nPx, m_nPy);
          m_HorseWingsEffectSurface := g_WHorseImg.GetCachedGrayImage(2560 + 320 * m_btSex + m_nCurrentFrame, m_nHorseWingsEffectX, m_nHorseWingsEffectY);
        end
        else if m_btDoubleHumHorse > 0 then begin
          case m_btDoubleHumHorse of
            2:begin
                m_HorseSurface := g_WHorseImg.GetCachedGrayImage(3520 + m_nCurrentFrame, m_nPx, m_nPy);
                m_HorseWingsEffectSurface := g_WHorseImg.GetCachedGrayImage(4800 + m_nCurrentFrame, m_nHorseWingsEffectX, m_nHorseWingsEffectY);
              end;
            3:begin
                m_HorseSurface := g_WHorseImg.GetCachedGrayImage(3200 + m_nCurrentFrame, m_nPx, m_nPy);
                m_HorseWingsEffectSurface := g_WHorseImg.GetCachedGrayImage(4480 + m_nCurrentFrame, m_nHorseWingsEffectX, m_nHorseWingsEffectY);
              end;
            4:begin
                m_HorseSurface := g_WHorseImg.GetCachedGrayImage(4160 + m_nCurrentFrame, m_nPx, m_nPy);
                m_HorseWingsEffectSurface := g_WHorseImg.GetCachedGrayImage(5440 + m_nCurrentFrame, m_nHorseWingsEffectX, m_nHorseWingsEffectY);
              end;
            5:begin
                m_HorseSurface := g_WHorseImg.GetCachedGrayImage(3840 + m_nCurrentFrame, m_nPx, m_nPy);
                m_HorseWingsEffectSurface := g_WHorseImg.GetCachedGrayImage(5120 + m_nCurrentFrame, m_nHorseWingsEffectX, m_nHorseWingsEffectY);
              end;
          end;
        end;
      end;
    end
    else if m_ColorEffect = ceBright then begin
      if (m_btHorseEffectType >= 1) and (m_btHorseEffectType <= 6) then
        m_HorseEffectSurface := g_WHorseImg2.GetCachedBrightImage(1920 + (m_btHorseEffectType - 1) * 320 + m_nCurrentFrame, m_nHorseEffectX, m_nHorseEffectY);

      if m_btHorse = 1 then begin
        if m_btDoubleHumHorse = 1 then
          m_HorseSurface := g_WHorseImg.GetCachedBrightImage(320 * m_btSex + m_nCurrentFrame, m_nPx, m_nPy)
        else if m_btDoubleHumHorse in [2, 3, 4, 5] then
          m_HorseSurface := g_WHorseImg.GetCachedBrightImage(320 * m_btDoubleHumHorse + m_nCurrentFrame, m_nPx, m_nPy);
      end
      else if m_btHorse = 2 then begin
        if m_btDoubleHumHorse = 1 then begin
          m_HorseSurface := g_WHorseImg.GetCachedBrightImage(1920 + 320 * m_btSex + m_nCurrentFrame, m_nPx, m_nPy);
          m_HorseWingsEffectSurface := g_WHorseImg.GetCachedBrightImage(2560 + 320 * m_btSex + m_nCurrentFrame, m_nHorseWingsEffectX, m_nHorseWingsEffectY);
        end
        else if m_btDoubleHumHorse > 0 then begin
          case m_btDoubleHumHorse of
            2:begin
                m_HorseSurface := g_WHorseImg.GetCachedBrightImage(3520 + m_nCurrentFrame, m_nPx, m_nPy);
                m_HorseWingsEffectSurface := g_WHorseImg.GetCachedBrightImage(4800 + m_nCurrentFrame, m_nHorseWingsEffectX, m_nHorseWingsEffectY);
              end;
            3:begin
                m_HorseSurface := g_WHorseImg.GetCachedBrightImage(3200 + m_nCurrentFrame, m_nPx, m_nPy);
                m_HorseWingsEffectSurface := g_WHorseImg.GetCachedBrightImage(4480 + m_nCurrentFrame, m_nHorseWingsEffectX, m_nHorseWingsEffectY);
              end;
            4:begin
                m_HorseSurface := g_WHorseImg.GetCachedBrightImage(4160 + m_nCurrentFrame, m_nPx, m_nPy);
                m_HorseWingsEffectSurface := g_WHorseImg.GetCachedBrightImage(5440 + m_nCurrentFrame, m_nHorseWingsEffectX, m_nHorseWingsEffectY);
              end;
            5:begin
                m_HorseSurface := g_WHorseImg.GetCachedBrightImage(3840 + m_nCurrentFrame, m_nPx, m_nPy);
                m_HorseWingsEffectSurface := g_WHorseImg.GetCachedBrightImage(5120 + m_nCurrentFrame, m_nHorseWingsEffectX, m_nHorseWingsEffectY);
              end;
          end;
        end;
      end;
    end
    else begin
      if (m_btHorseEffectType >= 1) and (m_btHorseEffectType <= 6) then
        m_HorseEffectSurface := g_WHorseImg2.GetCachedImage(1920 + (m_btHorseEffectType - 1) * 320 + m_nCurrentFrame, m_nHorseEffectX, m_nHorseEffectY);

      if m_btHorse = 1 then begin
        if m_btDoubleHumHorse = 1 then
          m_HorseSurface := g_WHorseImg.GetCachedImage(320 * m_btSex + m_nCurrentFrame, m_nPx, m_nPy)
        else if m_btDoubleHumHorse in [2, 3, 4, 5] then
          m_HorseSurface := g_WHorseImg.GetCachedImage(320 * m_btDoubleHumHorse + m_nCurrentFrame, m_nPx, m_nPy);
      end
      else if m_btHorse = 2 then begin
        if m_btDoubleHumHorse = 1 then begin
          m_HorseSurface := g_WHorseImg.GetCachedImage(1920 + 320 * m_btSex + m_nCurrentFrame, m_nPx, m_nPy);
          m_HorseWingsEffectSurface := g_WHorseImg.GetCachedImage(2560 + 320 * m_btSex + m_nCurrentFrame, m_nHorseWingsEffectX, m_nHorseWingsEffectY);
        end
        else if m_btDoubleHumHorse > 0 then begin
          case m_btDoubleHumHorse of
            2:begin
                m_HorseSurface := g_WHorseImg.GetCachedImage(3520 + m_nCurrentFrame, m_nPx, m_nPy);
                m_HorseWingsEffectSurface := g_WHorseImg.GetCachedImage(4800 + m_nCurrentFrame, m_nHorseWingsEffectX, m_nHorseWingsEffectY);
              end;
            3:begin
                m_HorseSurface := g_WHorseImg.GetCachedImage(3200 + m_nCurrentFrame, m_nPx, m_nPy);
                m_HorseWingsEffectSurface := g_WHorseImg.GetCachedImage(4480 + m_nCurrentFrame, m_nHorseWingsEffectX, m_nHorseWingsEffectY);
              end;
            4:begin
                m_HorseSurface := g_WHorseImg.GetCachedImage(4160 + m_nCurrentFrame, m_nPx, m_nPy);
                m_HorseWingsEffectSurface := g_WHorseImg.GetCachedImage(5440 + m_nCurrentFrame, m_nHorseWingsEffectX, m_nHorseWingsEffectY);
              end;
            5:begin
                m_HorseSurface := g_WHorseImg.GetCachedImage(3840 + m_nCurrentFrame, m_nPx, m_nPy);
                m_HorseWingsEffectSurface := g_WHorseImg.GetCachedImage(5120 + m_nCurrentFrame, m_nHorseWingsEffectX, m_nHorseWingsEffectY);
              end;
          end;
        end;
      end;
    end;

    if not m_boShowHorseWingsEffect then m_HorseWingsEffectSurface := nil;
    Exit;
  end;

  // 官方 horse2，因为没有资源（资源是微端更新的），先这样搞着吧 chongchong 2015-10-15
  if (m_btHorse >= 3) and (m_btHorse <= 5) then begin
    case m_ColorEffect of
      ceGrayScale:begin
          m_HorseSurface := g_WHorseImg2.GetCachedGrayImage((m_btHorse - 3) * 640 + m_btSex * 320 + m_nCurrentFrame, m_nPx, m_nPy);
          if (m_btHorseEffectType >= 1) and (m_btHorseEffectType <= 6) then
            m_HorseEffectSurface := g_WHorseImg2.GetCachedGrayImage(1920 + (m_btHorseEffectType - 1) * 320 + m_nCurrentFrame, m_nHorseEffectX, m_nHorseEffectY);
        end;
      ceBright:begin
          m_HorseSurface := g_WHorseImg2.GetCachedBrightImage((m_btHorse - 3) * 640 + m_btSex * 320 + m_nCurrentFrame, m_nPx, m_nPy);
          if (m_btHorseEffectType >= 1) and (m_btHorseEffectType <= 6) then
            m_HorseEffectSurface := g_WHorseImg2.GetCachedBrightImage(1920 + (m_btHorseEffectType - 1) * 320 + m_nCurrentFrame, m_nHorseEffectX, m_nHorseEffectY);
        end;
      else begin
          m_HorseSurface := g_WHorseImg2.GetCachedImage((m_btHorse - 3) * 640 + m_btSex * 320 + m_nCurrentFrame, m_nPx, m_nPy);
          if (m_btHorseEffectType >= 1) and (m_btHorseEffectType <= 6) then
            m_HorseEffectSurface := g_WHorseImg2.GetCachedImage(1920 + (m_btHorseEffectType - 1) * 320 + m_nCurrentFrame, m_nHorseEffectX, m_nHorseEffectY);
        end;
    end;
    Exit;
  end;

  // 骑马 三方单人马 L-Horse.wil chongchong 2013-10-17 ----------------------------------------------------------
  if m_btHorse >= 20 then begin
    // L_HumHorse.wil chongchong 2014-06-06

    if m_btHorseHumExpand = 0 then begin
      nHumHorseSex := m_btSex;
      nHumHorseCount := 2;
    end
    else begin
      nHumHorseSex := 0;
      nHumHorseCount := 1;
    end;

    case m_ColorEffect of
      ceGrayScale:begin
          if m_btHorseHum >= 300 then
            m_HorseHumSurface := g_WLHorseHumImg6.GetCachedGrayImage(600 * (nHumHorseSex + (m_btHorseHum - 300) * nHumHorseCount) + m_nCurrentFrame, m_nHorseHumX, m_nHorseHumY)
          else if m_btHorseHum >= 250 then
            m_HorseHumSurface := g_WLHorseHumImg5.GetCachedGrayImage(600 * (nHumHorseSex + (m_btHorseHum - 250) * nHumHorseCount) + m_nCurrentFrame, m_nHorseHumX, m_nHorseHumY)
          else if m_btHorseHum >= 200 then
            m_HorseHumSurface := g_WLHorseHumImg4.GetCachedGrayImage(600 * (nHumHorseSex + (m_btHorseHum - 200) * nHumHorseCount) + m_nCurrentFrame, m_nHorseHumX, m_nHorseHumY)
          else if m_btHorseHum >= 150 then
            m_HorseHumSurface := g_WLHorseHumImg3.GetCachedGrayImage(600 * (nHumHorseSex + (m_btHorseHum - 150) * nHumHorseCount) + m_nCurrentFrame, m_nHorseHumX, m_nHorseHumY)
          else if m_btHorseHum >= 100 then
            m_HorseHumSurface := g_WLHorseHumImg2.GetCachedGrayImage(600 * (nHumHorseSex + (m_btHorseHum - 100) * nHumHorseCount) + m_nCurrentFrame, m_nHorseHumX, m_nHorseHumY)
          else if m_btHorseHum >= 50 then
            m_HorseHumSurface := g_WLHorseHumImg1.GetCachedGrayImage(600 * (nHumHorseSex + (m_btHorseHum - 50) * nHumHorseCount) + m_nCurrentFrame, m_nHorseHumX, m_nHorseHumY)
          else
            m_HorseHumSurface := g_WLHorseHumImg.GetCachedGrayImage(600 * (nHumHorseSex + m_btHorseHum * nHumHorseCount) + m_nCurrentFrame, m_nHorseHumX, m_nHorseHumY);
        end;
      ceBright:begin
          if m_btHorseHum >= 300 then
            m_HorseHumSurface := g_WLHorseHumImg6.GetCachedBrightImage(600 * (nHumHorseSex + (m_btHorseHum - 300) * nHumHorseCount) + m_nCurrentFrame, m_nHorseHumX, m_nHorseHumY)
          else if m_btHorseHum >= 250 then
            m_HorseHumSurface := g_WLHorseHumImg5.GetCachedBrightImage(600 * (nHumHorseSex + (m_btHorseHum - 250) * nHumHorseCount) + m_nCurrentFrame, m_nHorseHumX, m_nHorseHumY)
          else if m_btHorseHum >= 200 then
            m_HorseHumSurface := g_WLHorseHumImg4.GetCachedBrightImage(600 * (nHumHorseSex + (m_btHorseHum - 200) * nHumHorseCount) + m_nCurrentFrame, m_nHorseHumX, m_nHorseHumY)
          else if m_btHorseHum >= 150 then
            m_HorseHumSurface := g_WLHorseHumImg3.GetCachedBrightImage(600 * (nHumHorseSex + (m_btHorseHum - 150) * nHumHorseCount) + m_nCurrentFrame, m_nHorseHumX, m_nHorseHumY)
          else if m_btHorseHum >= 100 then
            m_HorseHumSurface := g_WLHorseHumImg2.GetCachedBrightImage(600 * (nHumHorseSex + (m_btHorseHum - 100) * nHumHorseCount) + m_nCurrentFrame, m_nHorseHumX, m_nHorseHumY)
          else if m_btHorseHum >= 50 then
            m_HorseHumSurface := g_WLHorseHumImg1.GetCachedBrightImage(600 * (nHumHorseSex + (m_btHorseHum - 50) * nHumHorseCount) + m_nCurrentFrame, m_nHorseHumX, m_nHorseHumY)
          else
            m_HorseHumSurface := g_WLHorseHumImg.GetCachedBrightImage(600 * (nHumHorseSex + m_btHorseHum * nHumHorseCount) + m_nCurrentFrame, m_nHorseHumX, m_nHorseHumY);
        end;
      else begin
          if m_btHorseHum >= 300 then
            m_HorseHumSurface := g_WLHorseHumImg6.GetCachedImage(600 * (nHumHorseSex + (m_btHorseHum - 300) * nHumHorseCount) + m_nCurrentFrame, m_nHorseHumX, m_nHorseHumY)
          else if m_btHorseHum >= 250 then
            m_HorseHumSurface := g_WLHorseHumImg5.GetCachedImage(600 * (nHumHorseSex + (m_btHorseHum - 250) * nHumHorseCount) + m_nCurrentFrame, m_nHorseHumX, m_nHorseHumY)
          else if m_btHorseHum >= 200 then
            m_HorseHumSurface := g_WLHorseHumImg4.GetCachedImage(600 * (nHumHorseSex + (m_btHorseHum - 200) * nHumHorseCount) + m_nCurrentFrame, m_nHorseHumX, m_nHorseHumY)
          else if m_btHorseHum >= 150 then
            m_HorseHumSurface := g_WLHorseHumImg3.GetCachedImage(600 * (nHumHorseSex + (m_btHorseHum - 150) * nHumHorseCount) + m_nCurrentFrame, m_nHorseHumX, m_nHorseHumY)
          else if m_btHorseHum >= 100 then
            m_HorseHumSurface := g_WLHorseHumImg2.GetCachedImage(600 * (nHumHorseSex + (m_btHorseHum - 100) * nHumHorseCount) + m_nCurrentFrame, m_nHorseHumX, m_nHorseHumY)
          else if m_btHorseHum >= 50 then
            m_HorseHumSurface := g_WLHorseHumImg1.GetCachedImage(600 * (nHumHorseSex + (m_btHorseHum - 50) * nHumHorseCount) + m_nCurrentFrame, m_nHorseHumX, m_nHorseHumY)
          else
            m_HorseHumSurface := g_WLHorseHumImg.GetCachedImage(600 * (nHumHorseSex + m_btHorseHum * nHumHorseCount) + m_nCurrentFrame, m_nHorseHumX, m_nHorseHumY);
        end;
    end;
  end;

  if m_btHorse in [20..28] then begin
    if m_ColorEffect = ceGrayScale then begin
      m_HorseSurface := g_WLHorseImg.GetCachedGrayImage(600 * (m_btHorse - 20) + m_nCurrentFrame, m_nPx, m_nPy);
      m_HorseHairSurface := g_WLHorseHairImg.GetCachedGrayImage(600 * (m_btSex + m_btHorseHair * 2) + m_nCurrentFrame, m_nHorseHairX, m_nHorseHairY);

      if (m_btHorseEffectType >= 1) and (m_btHorseEffectType <= 9) then
        m_HorseEffectSurface := g_WLHorseEffectImg.GetCachedGrayImage(600 * (m_btHorseEffectType - 1) + m_nCurrentFrame, m_nHorseEffectX, m_nHorseEffectY);
    end
    else if m_ColorEffect = ceBright then begin
      m_HorseSurface := g_WLHorseImg.GetCachedBrightImage(600 * (m_btHorse - 20) + m_nCurrentFrame, m_nPx, m_nPy);
      m_HorseHairSurface := g_WLHorseHairImg.GetCachedBrightImage(600 * (m_btSex + m_btHorseHair * 2) + m_nCurrentFrame, m_nHorseHairX, m_nHorseHairY);

      if (m_btHorseEffectType >= 1) and (m_btHorseEffectType <= 9) then
        m_HorseEffectSurface := g_WLHorseEffectImg.GetCachedBrightImage(600 * (m_btHorseEffectType - 1) + m_nCurrentFrame, m_nHorseEffectX, m_nHorseEffectY);
    end
    else begin
      m_HorseSurface := g_WLHorseImg.GetCachedImage(600 * (m_btHorse - 20) + m_nCurrentFrame, m_nPx, m_nPy);
      m_HorseHairSurface := g_WLHorseHairImg.GetCachedImage(600 * (m_btSex + m_btHorseHair * 2) + m_nCurrentFrame, m_nHorseHairX, m_nHorseHairY);

      if (m_btHorseEffectType >= 1) and (m_btHorseEffectType <= 9) then
        m_HorseEffectSurface := g_WLHorseEffectImg.GetCachedImage(600 * (m_btHorseEffectType - 1) + m_nCurrentFrame, m_nHorseEffectX, m_nHorseEffectY);
    end;

    Exit;
  end
  else if m_btHorse in [29..49] then begin
    if m_ColorEffect = ceGrayScale then begin
      m_HorseSurface := g_WLHorseImg1.GetCachedGrayImage(600 * (m_btHorse - 29) + m_nCurrentFrame, m_nPx, m_nPy);
      m_HorseHairSurface := g_WLHorseHairImg.GetCachedGrayImage(600 * (m_btSex + m_btHorseHair * 2) + m_nCurrentFrame, m_nHorseHairX, m_nHorseHairY);

      if (m_btHorseEffectType >= 1) and (m_btHorseEffectType <= 21) then
        m_HorseEffectSurface := g_WLHorseEffectImg1.GetCachedGrayImage(600 * (m_btHorseEffectType - 1) + m_nCurrentFrame, m_nHorseEffectX, m_nHorseEffectY);
    end
    else if m_ColorEffect = ceBright then begin
      m_HorseSurface := g_WLHorseImg1.GetCachedBrightImage(600 * (m_btHorse - 29) + m_nCurrentFrame, m_nPx, m_nPy);
      m_HorseHairSurface := g_WLHorseHairImg.GetCachedBrightImage(600 * (m_btSex + m_btHorseHair * 2) + m_nCurrentFrame, m_nHorseHairX, m_nHorseHairY);

      if (m_btHorseEffectType >= 1) and (m_btHorseEffectType <= 21) then
        m_HorseEffectSurface := g_WLHorseEffectImg1.GetCachedBrightImage(600 * (m_btHorseEffectType - 1) + m_nCurrentFrame, m_nHorseEffectX, m_nHorseEffectY);
    end
    else begin
      m_HorseSurface := g_WLHorseImg1.GetCachedImage(600 * (m_btHorse - 29) + m_nCurrentFrame, m_nPx, m_nPy);
      m_HorseHairSurface := g_WLHorseHairImg.GetCachedImage(600 * (m_btSex + m_btHorseHair * 2) + m_nCurrentFrame, m_nHorseHairX, m_nHorseHairY);

      if (m_btHorseEffectType >= 1) and (m_btHorseEffectType <= 21) then
        m_HorseEffectSurface := g_WLHorseEffectImg1.GetCachedImage(600 * (m_btHorseEffectType - 1) + m_nCurrentFrame, m_nHorseEffectX, m_nHorseEffectY);
    end;

    Exit;
  end else if m_btHorse in [50..99] then begin
    if m_ColorEffect = ceGrayScale then begin
      m_HorseSurface := g_WLHorseImg2.GetCachedGrayImage(600 * (m_btHorse - 50) + m_nCurrentFrame, m_nPx, m_nPy);
      m_HorseHairSurface := g_WLHorseHairImg.GetCachedGrayImage(600 * (m_btSex + m_btHorseHair * 2) + m_nCurrentFrame, m_nHorseHairX, m_nHorseHairY);

      if (m_btHorseEffectType >= 1) and (m_btHorseEffectType <= 50) then
        m_HorseEffectSurface := g_WLHorseEffectImg2.GetCachedGrayImage(600 * (m_btHorseEffectType - 1) + m_nCurrentFrame, m_nHorseEffectX, m_nHorseEffectY);
    end else if m_ColorEffect = ceBright then begin
      m_HorseSurface := g_WLHorseImg2.GetCachedBrightImage(600 * (m_btHorse - 50) + m_nCurrentFrame, m_nPx, m_nPy);
      m_HorseHairSurface := g_WLHorseHairImg.GetCachedBrightImage(600 * (m_btSex + m_btHorseHair * 2) + m_nCurrentFrame, m_nHorseHairX, m_nHorseHairY);

      if (m_btHorseEffectType >= 1) and (m_btHorseEffectType <= 50) then
        m_HorseEffectSurface := g_WLHorseEffectImg2.GetCachedBrightImage(600 * (m_btHorseEffectType - 1) + m_nCurrentFrame, m_nHorseEffectX, m_nHorseEffectY);
    end else begin
      m_HorseSurface := g_WLHorseImg2.GetCachedImage(600 * (m_btHorse - 50) + m_nCurrentFrame, m_nPx, m_nPy);
      m_HorseHairSurface := g_WLHorseHairImg.GetCachedImage(600 * (m_btSex + m_btHorseHair * 2) + m_nCurrentFrame, m_nHorseHairX, m_nHorseHairY);

      if (m_btHorseEffectType >= 1) and (m_btHorseEffectType <= 50) then
        m_HorseEffectSurface := g_WLHorseEffectImg2.GetCachedBrightImage(600 * (m_btHorseEffectType - 1) + m_nCurrentFrame, m_nHorseEffectX, m_nHorseEffectY);
    end;

    Exit;
  end;

  //(m_nHitEffectNumber <= -SM_CUSTOM_HIT1) and (m_nHitEffectNumber >= -SM_CUSTOM_HIT100)

  boCustomHitContinue := False;
  if m_nCurrentAction = SM_SPELL then begin
    CustomMagicConfig := GetCustomMagicConfig(m_CurMagic.MagicSerial);
    if CustomMagicConfig <> nil then begin
      boCustomHitContinue := (CustomMagicConfig.MagicBaseConfig.MagicActionType = matCustom) and CustomMagicConfig.MagicBaseConfig.MagicActionContinue;
    end;
  end else if (m_nCurrentAction >= SM_CUSTOM_HIT001) and (m_nCurrentAction < SM_CUSTOM_HIT001 + CUSTOM_MAGIC_COUNT) then begin
    CustomMagicConfig := GetCustomMagicConfig(Abs(m_nCurrentAction) - SM_CUSTOM_HIT001 + CUSTOM_MAGIC_START_ID);
    if CustomMagicConfig <> nil then begin
      boCustomHitContinue := (CustomMagicConfig.MagicBaseConfig.MagicActionType = matCustom) and CustomMagicConfig.MagicBaseConfig.MagicActionContinue;
    end;
  end else if (m_nCurrentAction >= SM_CUSTOM_PUSH001) and (m_nCurrentAction < SM_CUSTOM_PUSH001 + CUSTOM_MAGIC_COUNT) then begin
    CustomMagicConfig := GetCustomMagicConfig(Abs(m_nCurrentAction) - SM_CUSTOM_PUSH001 + CUSTOM_MAGIC_START_ID);
    if CustomMagicConfig <> nil then begin
      boCustomHitContinue := (CustomMagicConfig.MagicBaseConfig.MagicActionType = matCustom) and CustomMagicConfig.MagicBaseConfig.MagicActionContinue;
    end;
  end;

  if ((m_nCurrentAction >= SM_100HIT) and (m_nCurrentAction <= SM_103HIT)) or (m_nCurrentAction = SM_113HIT)
    or (m_nCurrentAction = SM_115HIT) or ((m_nCurrentAction = SM_SPELL)
    and (m_CurMagic.EffectNumber in [55, 104..111, 208 {旋风斩}])) or boCustomHitContinue then begin // 连击
    {------------------------------------衣服外观-----------------------------------}

    if g_ClientConfig.boSimpleShowHumanDress and g_ConfigDlg.ConfigCheckeds[ckSimpleShowHumanDress] then begin
      if m_wDress = m_btSex then begin
        nShape := 0
      end else begin
        if not g_ConfigClient.boCustomHuamSimpleShow then begin
          case m_btJob of
            0:nShape := 3; // 重盔甲
            1:nShape := 4; // 魔法长袍
            2:nShape := 5; // 灵魂战衣
            else
              nShape := 3;
          end;
        end else begin
          case m_btJob of
            0:nShape := g_ConfigClient.nSimpleDressShapeArr[0];
            1:nShape := g_ConfigClient.nSimpleDressShapeArr[1];
            2:nShape := g_ConfigClient.nSimpleDressShapeArr[2];
            else
              nShape := g_ConfigClient.nSimpleDressShapeArr[0];
          end;
        end;
      end;

      cboGameImages := nil;
      nDress := m_btSex; //HZQ 20230525 这个值是不需要，也是错的，后面没个路径均对nDress有赋值 为了消除后面的警告

      if nShape < 1000 then begin
        if nShape <= 99 then begin // Hum.wzl  0~99
          nAppr := 0;
          nDress := nShape * 2 + m_btSex;
        end else begin
          nAppr := (nShape - 100) div 50 + 2;
          nDress := ((nShape - 100) mod 50) * 2 + m_btSex;
        end;

        //////////////////连击人物外观扩展  piaoyun 2013-07-29////////////////////
        case nAppr of
          0:cboGameImages := g_WCboHum.Indexs[0];
          2:begin
              cboGameImages := g_WCboHum.Indexs[0];
              nDress := nDress + 24;
            end;
          5:; // cboGameImages = nil 光着身子
          else
            cboGameImages := g_WCboHum.Indexs[nAppr - 1];
        end;
        //////////////////////////////////////////////////////////////////////////
      end;

      if cboGameImages = nil then begin
        cboGameImages := g_WCboHum.Indexs[0]; //g_cboHum;
        nDress := m_btSex;
      end;

      if cboGameImages <> nil then begin
        case m_ColorEffect of
          ceGrayScale, ceGrayScale2:m_BodySurface := cboGameImages.GetCachedGrayImage(nDress * 2000 + m_nCurrentFrame, m_nPx, m_nPy);
          ceBright:m_BodySurface := cboGameImages.GetCachedBrightImage(nDress * 2000 + m_nCurrentFrame, m_nPx, m_nPy);
          else
            // 这里设置断点，可以来找原因.... chongchong 2014-09-21
            m_BodySurface := cboGameImages.GetCachedImage(nDress * 2000 + m_nCurrentFrame, m_nPx, m_nPy);
        end;
      end;

      if (m_BodySurface = nil) or (m_BodySurface.Width * m_BodySurface.Height <= 16) then begin
        case m_ColorEffect of
          ceGrayScale, ceGrayScale2:m_BodySurface := cboGameImages.GetCachedGrayImage((m_btSex + 1) * 2 * 2000 + m_nCurrentFrame, m_nPx, m_nPy);
          ceBright:m_BodySurface := cboGameImages.GetCachedBrightImage((m_btSex + 1) * 2 * 2000 + m_nCurrentFrame, m_nPx, m_nPy);
          else
            m_BodySurface := cboGameImages.GetCachedImage((m_btSex + 1) * 2 * 2000 + m_nCurrentFrame, m_nPx, m_nPy);
        end;
      end;
    end else begin
      if m_btCboDressUseDiyImage = 0 then begin
        cboGameImages := nil;
        nDress := m_btSex; //HZQ 20230525 这个值是不需要，也是错的，后面没个路径均对nDress有赋值 为了消除后面的警告

        nShape := (m_wDress - m_btSex) div 2;

        if nShape < 1000 then begin
          if nShape <= 99 then begin // Hum.wzl  0~99
            nAppr := 0;
            nDress := m_wDress;
          end else begin
            nAppr := (nShape - 100) div 50 + 2;
            nDress := ((nShape - 100) mod 50) * 2 + m_btSex;
          end;

          //////////////////连击人物外观扩展  piaoyun 2013-07-29////////////////////
          case nAppr of
            0:cboGameImages := g_WCboHum.Indexs[0];
            2:begin
                cboGameImages := g_WCboHum.Indexs[0];
                nDress := nDress + 24;
              end;
            5:; // cboGameImages = nil 光着身子
            else
              cboGameImages := g_WCboHum.Indexs[nAppr - 1];
          end;
          //////////////////////////////////////////////////////////////////////////
        end;

        if cboGameImages = nil then begin
          cboGameImages := g_WCboHum.Indexs[0]; //g_cboHum;
          nDress := m_btSex;
        end;

        if cboGameImages <> nil then begin
          case m_ColorEffect of
            ceGrayScale, ceGrayScale2:m_BodySurface := cboGameImages.GetCachedGrayImage(nDress * 2000 + m_nCurrentFrame, m_nPx, m_nPy);
            ceBright:m_BodySurface := cboGameImages.GetCachedBrightImage(nDress * 2000 + m_nCurrentFrame, m_nPx, m_nPy);
            else
              // 这里设置断点，可以来找原因.... chongchong 2014-09-21
              m_BodySurface := cboGameImages.GetCachedImage(nDress * 2000 + m_nCurrentFrame, m_nPx, m_nPy);
          end;
        end;
        if (m_BodySurface = nil) or (m_BodySurface.Width * m_BodySurface.Height <= 16) then begin
          case m_ColorEffect of
            ceGrayScale, ceGrayScale2:m_BodySurface := cboGameImages.GetCachedGrayImage((m_btSex + 1) * 2 * 2000 + m_nCurrentFrame, m_nPx, m_nPy);
            ceBright:m_BodySurface := cboGameImages.GetCachedBrightImage((m_btSex + 1) * 2 * 2000 + m_nCurrentFrame, m_nPx, m_nPy);
            else
              m_BodySurface := cboGameImages.GetCachedImage((m_btSex + 1) * 2 * 2000 + m_nCurrentFrame, m_nPx, m_nPy);
          end;
        end;
      end

        // 连击使用默认衣服外观 chongchong 2015-08-03
      else begin
        nFileIndex := (m_btCboDressUseDiyImage - 1) div 30;
        nImgIndex := (m_btCboDressUseDiyImage - 1) mod 30;

        if (nFileIndex >= 0) and (nFileIndex < CBOHUMDIYFILE_COUNT) then begin
          case m_ColorEffect of
            ceGrayScale, ceGrayScale2:m_BodySurface := g_cboHumDiys[nFileIndex].GetCachedGrayImage(nImgIndex * 4000 + m_btSex * 2000 + m_nCurrentFrame, m_nPx, m_nPy);
            ceBright:m_BodySurface := g_cboHumDiys[nFileIndex].GetCachedBrightImage(nImgIndex * 4000 + m_btSex * 2000 + m_nCurrentFrame, m_nPx, m_nPy);
            else
              m_BodySurface := g_cboHumDiys[nFileIndex].GetCachedImage(nImgIndex * 4000 + m_btSex * 2000 + m_nCurrentFrame, m_nPx, m_nPy);
          end;
        end;
      end;
    end;

    {------------------------------------发型外观-----------------------------------}
    cboGameImages := nil;
    nHairOffset := -1; //HZQ 20230525
    if m_btHair < 6 then begin // 0 -- 5
      cboGameImages := g_cboHair;
      if (m_btHair < 4) or (m_btHorse > 0) then begin
        case m_btSex of
          0:nHairOffset := m_btHair * 2000; // m_btHair=0 m_btHair=2
          1:case m_btHair of
              0:nHairOffset := 0; // m_btHair=0 m_btHair=1 m_btHair=3   1800     2400
              1:nHairOffset := 2000;
              3:nHairOffset := 6000;
              //else nHairOffset := -1; //HZQ 20230525
            end;
        end;
        //end else begin
          //nHairOffset := -1; //HZQ 20230525
      end;
    end else if (m_btHair >= 100) and (m_btHair <= 109) then begin
      nHairOffset := 12000 + (m_btHair - 100) * 2000 * 2 + m_btSex * 2000;
      cboGameImages := g_cboHair;
    end else if (m_btHair >= 50) and (m_btHair <= 59) then begin // 读取cboHair10
      nHairOffset := (m_btHair - 50) * 2000 { * 2 + m_btSex * 2000};
      cboGameImages := g_cboHair10;
    end else if (m_btHair >= 60) and (m_btHair <= 69) then begin // 读取cboHair11
      nHairOffset := (m_btHair - 60) * 2000 { * 2 + m_btSex * 2000};
      cboGameImages := g_cboHair11;
    end;

    if (nHairOffset >= 0) and (cboGameImages <> nil) and m_boShowHair then begin
      case m_ColorEffect of
        ceGrayScale:m_HairSurface := cboGameImages.GetCachedGrayImage(nHairOffset + m_nCurrentFrame, m_nHpx, m_nHpy);
        ceBright:m_HairSurface := cboGameImages.GetCachedBrightImage(nHairOffset + m_nCurrentFrame, m_nHpx, m_nHpy);
        else
          m_HairSurface := cboGameImages.GetCachedImage(nHairOffset + m_nCurrentFrame, m_nHpx, m_nHpy);
      end;
    end;

    {----------------------------------武器外观-------------------------------------}
    if g_ClientConfig.boSimpleShowHumanWeapon and g_ConfigDlg.ConfigCheckeds[ckSimpleShowHumanWeapon] then begin
      if m_wWeapon > 0 then begin
        cboGameImages := nil;

        if not g_ConfigClient.boCustomHuamSimpleShow then begin
          case m_btJob of
            0:nShape := 24;
            1:nShape := 28; // 骨玉权杖
            2:nShape := 25;
            else
              nShape := 24;
          end;
        end
        else begin
          case m_btJob of
            0:nShape := g_ConfigClient.nSimpleWeaponShapeArr[0];
            1:nShape := g_ConfigClient.nSimpleWeaponShapeArr[1];
            2:nShape := g_ConfigClient.nSimpleWeaponShapeArr[2];
            else
              nShape := g_ConfigClient.nSimpleWeaponShapeArr[0];
          end;
        end;

        if nShape < 1000 then begin
          if nShape <= 99 then begin // Weapon.wzl
            nWeaponIndex := nShape * 2 + m_btSex;
            nAppr := 0;
          end
          else begin
            nWeaponIndex := ((nShape - 100) mod 50) * 2 + m_btSex; // (nShape - 100) * 2 + m_btSex;
            nAppr := (nShape - 100) div 50 + 2;
          end;
          /////////////////////// 连击武器外观 piaoyun 2013-07-29 ////////////////////
          case nAppr of
            0:cboGameImages := g_WCboWeaponList.Indexs[0];
            1:;
            2:begin
                nWeaponIndex := nWeaponIndex + 76;
                cboGameImages := g_WCboWeaponList.Indexs[0];
              end;
            4:begin
                // 美工搞错了图片帧序，特殊处理下 piaoyun 2013-07-29
                case nWeaponIndex of
                  10 * 2:nWeaponIndex := 11 * 2; // 40000--44000
                  11 * 2:nWeaponIndex := 10 * 2; // 44000--40000

                  13 * 2:nWeaponIndex := 14 * 2; // 52000--56000
                  14 * 2:nWeaponIndex := 13 * 2; // 56000--52000

                  16 * 2:nWeaponIndex := 17 * 2; // 64000--68000
                  17 * 2:nWeaponIndex := 16 * 2; // 68000--64000
                end;
                cboGameImages := g_WCboWeaponList.Indexs[nAppr - 1];
              end;
            else
              cboGameImages := g_WCboWeaponList.Indexs[nAppr - 1];
          end;
        end;
        ////////////////////////////////////////////////////////////////////////////
        if cboGameImages = nil then begin
          cboGameImages := g_WCboWeaponList.Indexs[0]; //g_cboWeapon
          nWeaponIndex := m_btSex;
        end;

        if cboGameImages <> nil then begin
          case m_ColorEffect of
            ceGrayScale:m_WeaponSurface := cboGameImages.GetCachedGrayImage(nWeaponIndex * 2000 + m_nCurrentFrame, m_nWpx, m_nWpy);
            ceBright:m_WeaponSurface := cboGameImages.GetCachedBrightImage(nWeaponIndex * 2000 + m_nCurrentFrame, m_nWpx, m_nWpy);
            else
              m_WeaponSurface := cboGameImages.GetCachedImage(nWeaponIndex * 2000 + m_nCurrentFrame, m_nWpx, m_nWpy);
          end;
        end;
      end;
    end
    else begin
      if m_btCboWeaponUseDiyImage = 0 then begin
        cboGameImages := nil;

        nShape := (m_wWeapon - m_btSex) div 2;
        if nShape < 1000 then begin
          if nShape <= 99 then begin // Weapon.wzl
            nWeaponIndex := m_wWeapon;
            nAppr := 0;
          end
          else begin
            nWeaponIndex := ((nShape - 100) mod 50) * 2 + m_btSex; // (nShape - 100) * 2 + m_btSex;
            nAppr := (nShape - 100) div 50 + 2;
          end;
          /////////////////////// 连击武器外观 piaoyun 2013-07-29 ////////////////////
          case nAppr of
            0:cboGameImages := g_WCboWeaponList.Indexs[0];
            1:;
            2:begin
                nWeaponIndex := nWeaponIndex + 76;
                cboGameImages := g_WCboWeaponList.Indexs[0];
              end;
            4:begin
                // 美工搞错了图片帧序，特殊处理下 piaoyun 2013-07-29
                case nWeaponIndex of
                  10 * 2:nWeaponIndex := 11 * 2; // 40000--44000
                  11 * 2:nWeaponIndex := 10 * 2; // 44000--40000

                  13 * 2:nWeaponIndex := 14 * 2; // 52000--56000
                  14 * 2:nWeaponIndex := 13 * 2; // 56000--52000

                  16 * 2:nWeaponIndex := 17 * 2; // 64000--68000
                  17 * 2:nWeaponIndex := 16 * 2; // 68000--64000
                end;
                cboGameImages := g_WCboWeaponList.Indexs[nAppr - 1];
              end;
            else
              cboGameImages := g_WCboWeaponList.Indexs[nAppr - 1];
          end;
        end;
        ////////////////////////////////////////////////////////////////////////////
        if cboGameImages = nil then begin
          cboGameImages := g_WCboWeaponList.Indexs[0]; //g_cboWeapon
          nWeaponIndex := m_btSex;
        end;

        if cboGameImages <> nil then begin
          case m_ColorEffect of
            ceGrayScale:m_WeaponSurface := cboGameImages.GetCachedGrayImage(nWeaponIndex * 2000 + m_nCurrentFrame, m_nWpx, m_nWpy);
            ceBright:m_WeaponSurface := cboGameImages.GetCachedBrightImage(nWeaponIndex * 2000 + m_nCurrentFrame, m_nWpx, m_nWpy);
            else
              m_WeaponSurface := cboGameImages.GetCachedImage(nWeaponIndex * 2000 + m_nCurrentFrame, m_nWpx, m_nWpy);
          end;
        end;
      end

        // 连击使用默认武器外观 chongchong 2015-08-03
      else begin
        nFileIndex := (m_btCboWeaponUseDiyImage - 1) div 30;
        nImgIndex := (m_btCboWeaponUseDiyImage - 1) mod 30;

        if (nFileIndex >= 0) and (nFileIndex < CBOHUMDIYFILE_COUNT) then begin
          case m_ColorEffect of
            ceGrayScale:m_WeaponSurface := g_cboWeaponDiys[nFileIndex].GetCachedGrayImage(nImgIndex * 4000 + m_btSex * 2000 + m_nCurrentFrame, m_nWpx, m_nWpy);
            ceBright:m_WeaponSurface := g_cboWeaponDiys[nFileIndex].GetCachedBrightImage(nImgIndex * 4000 + m_btSex * 2000 + m_nCurrentFrame, m_nWpx, m_nWpy);
            else
              m_WeaponSurface := g_cboWeaponDiys[nFileIndex].GetCachedImage(nImgIndex * 4000 + m_btSex * 2000 + m_nCurrentFrame, m_nWpx, m_nWpy);
          end;
        end;
      end;
    end;
    {---------------------------------翅膀外观-------------------------------------}
    ///////////////////////翅膀效果扩展 piaoyun 2013-07-28//////////////////////

    if (not (g_ClientConfig.boSimpleShowHumanDress and g_ConfigDlg.ConfigCheckeds[ckSimpleShowHumanDress])) then begin
      if (m_wEffect <> 0) and (m_wEffect <> 50) then begin
        if m_btCboDressUseDiyImage = 0 then begin
          nEffectOffSet := (m_wEffect - 1) * 2000;
          if m_nCurrentFrame < 64 then begin
            case m_ColorEffect of
              ceGrayScale:m_HumWinSurface := g_WCboHumEffect.Indexs[0].GetCachedGrayImage(nEffectOffSet + (m_btDir * 8) + m_nFrame, m_nSpx, m_nSpy);
              ceBright:m_HumWinSurface := g_WCboHumEffect.Indexs[0].GetCachedBrightImage(nEffectOffSet + (m_btDir * 8) + m_nFrame, m_nSpx, m_nSpy);
              else
                m_HumWinSurface := g_WCboHumEffect.Indexs[0].GetCachedImage(nEffectOffSet + (m_btDir * 8) + m_nFrame, m_nSpx, m_nSpy);
            end;
          end
          else begin
            case m_ColorEffect of
              ceGrayScale:m_HumWinSurface := g_WCboHumEffect.Indexs[0].GetCachedGrayImage(nEffectOffSet + m_nCurrentFrame, m_nSpx, m_nSpy);
              ceBright:m_HumWinSurface := g_WCboHumEffect.Indexs[0].GetCachedBrightImage(nEffectOffSet + m_nCurrentFrame, m_nSpx, m_nSpy);
              else
                m_HumWinSurface := g_WCboHumEffect.Indexs[0].GetCachedImage(nEffectOffSet + m_nCurrentFrame, m_nSpx, m_nSpy);
            end;
          end;
        end

          // 连击使用默认衣服特效外观 chongchong 2015-08-03
        else begin
          nFileIndex := (m_btCboDressUseDiyImage - 1) div 30;
          nImgIndex := (m_btCboDressUseDiyImage - 1) mod 30;

          if (nFileIndex >= 0) and (nFileIndex < CBOHUMDIYFILE_COUNT) then begin
            case m_ColorEffect of
              ceGrayScale:m_HumWinSurface := g_cboHumEffectDiys[nFileIndex].GetCachedGrayImage(nImgIndex * 4000 + m_btSex * 2000 + m_nCurrentFrame, m_nSpx, m_nSpy);
              ceBright:m_HumWinSurface := g_cboHumEffectDiys[nFileIndex].GetCachedBrightImage(nImgIndex * 4000 + m_btSex * 2000 + m_nCurrentFrame, m_nSpx, m_nSpy);
              else
                m_HumWinSurface := g_cboHumEffectDiys[nFileIndex].GetCachedImage(nImgIndex * 4000 + m_btSex * 2000 + m_nCurrentFrame, m_nSpx, m_nSpy);
            end;
          end;
        end;
      end;

      if (m_wEffect_30 <> 0) and (m_wEffect_30 <> 50) then begin
        nEffectOffSet_30 := (m_wEffect_30 - 1) * 2000;
        if m_nCurrentFrame < 64 then begin
          case m_ColorEffect of
            ceGrayScale:m_HumWinSurface_30 := g_WCboHumEffect.Indexs[0].GetCachedGrayImage(nEffectOffSet_30 + (m_btDir * 8) + m_nFrame, m_nSpx_30, m_nSpy_30);
            ceBright:m_HumWinSurface_30 := g_WCboHumEffect.Indexs[0].GetCachedBrightImage(nEffectOffSet_30 + (m_btDir * 8) + m_nFrame, m_nSpx_30, m_nSpy_30);
            else
              m_HumWinSurface_30 := g_WCboHumEffect.Indexs[0].GetCachedImage(nEffectOffSet_30 + (m_btDir * 8) + m_nFrame, m_nSpx_30, m_nSpy_30);
          end;
        end
        else begin
          case m_ColorEffect of
            ceGrayScale:m_HumWinSurface_30 := g_WCboHumEffect.Indexs[0].GetCachedGrayImage(nEffectOffSet_30 + m_nCurrentFrame, m_nSpx_30, m_nSpy_30);
            ceBright:m_HumWinSurface_30 := g_WCboHumEffect.Indexs[0].GetCachedBrightImage(nEffectOffSet_30 + m_nCurrentFrame, m_nSpx_30, m_nSpy_30);
            else
              m_HumWinSurface_30 := g_WCboHumEffect.Indexs[0].GetCachedImage(nEffectOffSet_30 + m_nCurrentFrame, m_nSpx_30, m_nSpy_30);
          end;
        end;
      end;

      {--------------------------------自定义特效外观----------------------------------}
      // 衣服、翅膀特效
      if (not g_ConfigDlg.ConfigCheckeds[ckHideHumEffect]) or (not PlugInEnabled) then begin
        if m_wHeroM2DressEffect > 0 then begin
          case m_ColorEffect of
            ceGrayScale:m_HeroM2DressEffect := g_WCboHumEffect.Indexs[0].GetCachedGrayImage((m_wHeroM2DressEffect - 1) * 2000 + m_nCurrentFrame, m_nHeroM2DressEffectX, m_nHeroM2DressEffectY);
            ceBright:m_HeroM2DressEffect := g_WCboHumEffect.Indexs[0].GetCachedBrightImage((m_wHeroM2DressEffect - 1) * 2000 + m_nCurrentFrame, m_nHeroM2DressEffectX, m_nHeroM2DressEffectY);
            else
              m_HeroM2DressEffect := g_WCboHumEffect.Indexs[0].GetCachedImage((m_wHeroM2DressEffect - 1) * 2000 + m_nCurrentFrame, m_nHeroM2DressEffectX, m_nHeroM2DressEffectY);
          end;
        end;

        if m_btCboDressUseDiyImage = 0 then begin
          ////////////////衣服、翅膀连击特效扩展 piaoyun 2013-07-30/////////////////
          if m_wEffect >= 1000 then begin
            nUnit := (m_wEffect - 1000) div 25; // 单元编号
            nUnit := nUnit + 1; // 排除第一个文件
            nHumEffectOffset := (m_wEffect - 1000) * HUMANFRAME * 2 + m_btSex * HUMANFRAME;
            nMaxOffset := 25 * HUMANFRAME * 2;
            if nHumEffectOffset >= nMaxOffset then
              nHumEffectOffset := nHumEffectOffset mod nMaxOffset;

            case nUnit of
              1:begin
                  if nHumEffectOffset < 4800 then begin
                    nEffectOffSet := 40000 + nHumEffectOffset div 600 * 2000;
                    cboGameImages := g_WCboHumEffect.Indexs[0]; //g_cboHumEffect;
                  end
                  else begin
                    nEffectOffSet := (nHumEffectOffset - 4800) div 600 * 2000;
                    cboGameImages := g_WCboHumEffect.Indexs[1]; //g_cboHumEffect2;
                  end;
                end;
              else begin
                  nEffectOffSet := nHumEffectOffset div 600 * 2000;
                  cboGameImages := g_WCboHumEffect.Indexs[nUnit]; //g_cboHumEffect3;
                end;
            end;

            if cboGameImages <> nil then begin
              case m_ColorEffect of
                ceGrayScale:m_DressEffectSurface := cboGameImages.GetCachedGrayImage(nEffectOffSet {+ 2000 * m_btSex } + m_nCurrentFrame, m_nDressEffectX, m_nDressEffectY);
                ceBright:m_DressEffectSurface := cboGameImages.GetCachedBrightImage(nEffectOffSet {+ 2000 * m_btSex} + m_nCurrentFrame, m_nDressEffectX, m_nDressEffectY);
                else
                  m_DressEffectSurface := cboGameImages.GetCachedImage(nEffectOffSet {+ 2000 * m_btSex} + m_nCurrentFrame, m_nDressEffectX, m_nDressEffectY);
              end;
            end;
          end
          else
            ////////////////衣服、翅膀连击特效扩展结束 piaoyun 2013-07-30/////////////

            {//////////////////////////引擎自定义衣服、翅膀特效////////////////////////} begin
            g_EffectImageList.Lock;
            try
              if (m_nDressEffectIndex >= 0) and (m_nDressEffectIndex < g_EffectImageList.Count) then begin
                GameImages := TGameImages(g_EffectImageList.Objects[m_nDressEffectIndex]);
                if GameImages <> nil then begin
                  cboGameImages := nil;
                  sFileName := LowerCase(ExtractFileName(GameImages.FileName));
                  if CompareLStr(sFileName, 'humeffect.w', Length('humeffect.w')) then begin
                    nEffectOffSet := m_wDressEffectOffSet div 600 * 2000;
                    cboGameImages := g_WCboHumEffect.Indexs[0]; //g_cboHumEffect;
                  end
                  else if CompareLStr(sFileName, 'humeffect2.w', Length('humeffect2.w')) then begin
                    if m_wDressEffectOffSet < 4800 then begin // humeffect2部分连击效果在 cbohumeffect中
                      nEffectOffSet := 40000 + m_wDressEffectOffSet div 600 * 2000;
                      cboGameImages := g_WCboHumEffect.Indexs[0]; //g_cboHumEffect;
                    end
                    else begin
                      nEffectOffSet := (m_wDressEffectOffSet - 4800) div 600 * 2000;
                      cboGameImages := g_WCboHumEffect.Indexs[1]; //g_cboHumEffect2;
                    end;
                  end
                  else if Pos('humeffect', sFileName) = 1 then begin
                    sTemp := Copy(sFileName, 10, MaxInt);
                    nIndex := Pos('.', sTemp);
                    if nIndex > 0 then begin
                      sTemp := Copy(sTemp, 1, nIndex - 1);

                      nIndex := StrToIntDef(sTemp, 0);
                      if nIndex > 0 then begin
                        nEffectOffSet := m_wDressEffectOffSet div 600 * 2000;
                        cboGameImages := g_WCboHumEffect.Indexs[nIndex - 1]; //g_cboHumEffect???;
                      end;
                    end;
                  end;

                  if cboGameImages <> nil then begin
                    m_boDressEffectDrawNoBlend := m_boDressEffectNoBlend;

                    if m_boDressEffectNoSex then begin
                      case m_ColorEffect of
                        ceGrayScale:m_DressEffectSurface := cboGameImages.GetCachedGrayImage(nEffectOffSet + m_nCurrentFrame, m_nDressEffectX, m_nDressEffectY);
                        ceBright:m_DressEffectSurface := cboGameImages.GetCachedBrightImage(nEffectOffSet + m_nCurrentFrame, m_nDressEffectX, m_nDressEffectY);
                        else
                          m_DressEffectSurface := cboGameImages.GetCachedImage(nEffectOffSet + m_nCurrentFrame, m_nDressEffectX, m_nDressEffectY);
                      end;
                    end
                    else begin
                      case m_ColorEffect of
                        ceGrayScale:m_DressEffectSurface := cboGameImages.GetCachedGrayImage(nEffectOffSet + 2000 * m_btSex + m_nCurrentFrame, m_nDressEffectX, m_nDressEffectY);
                        ceBright:m_DressEffectSurface := cboGameImages.GetCachedBrightImage(nEffectOffSet + 2000 * m_btSex + m_nCurrentFrame, m_nDressEffectX, m_nDressEffectY);
                        else
                          m_DressEffectSurface := cboGameImages.GetCachedImage(nEffectOffSet + 2000 * m_btSex + m_nCurrentFrame, m_nDressEffectX, m_nDressEffectY);
                      end;
                    end;
                  end;
                end;
              end;
            finally
              g_EffectImageList.UnLock;
            end;
          end;
        end
          // 连击使用默认衣服特效外观 chongchong 2015-08-03
        else begin
          nFileIndex := (m_btCboDressUseDiyImage - 1) div 30;
          nImgIndex := (m_btCboDressUseDiyImage - 1) mod 30;

          if (nFileIndex >= 0) and (nFileIndex < CBOHUMDIYFILE_COUNT) then begin
            case m_ColorEffect of
              ceGrayScale:m_DressEffectSurface := g_cboHumEffectDiys[nFileIndex].GetCachedGrayImage(nImgIndex * 4000 + m_btSex * 2000 + m_nCurrentFrame, m_nDressEffectX, m_nDressEffectY);
              ceBright:m_DressEffectSurface := g_cboHumEffectDiys[nFileIndex].GetCachedBrightImage(nImgIndex * 4000 + m_btSex * 2000 + m_nCurrentFrame, m_nDressEffectX, m_nDressEffectY);
              else
                m_DressEffectSurface := g_cboHumEffectDiys[nFileIndex].GetCachedImage(nImgIndex * 4000 + m_btSex * 2000 + m_nCurrentFrame, m_nDressEffectX, m_nDressEffectY);
            end;
          end;
        end;
        ////////////////////////引擎自定义衣服、翅膀特效结束//////////////////////
      end;

      // 武器特效
      if m_WeaponSurface <> nil then begin
        ///////////////////////连击武器效果扩展 piaoyun 2013-07-29////////////////

        if m_btCboWeaponUseDiyImage = 0 then begin
          cboGameImages := nil;
          nUnit := 0;
          // 解释：此时m_wWeaponEffectOffSet不再是偏移地址，而是服务端Anicount字段，只是利用这个结构体成员传过来而已 piaoyun 2013-07-30
          if (m_wWeaponEffectOffSet >= 1000) and (m_wWeaponEffectOffSet <= 2000) then begin
            if {(m_wWeaponEffectOffSet >= 1000) and }(m_wWeaponEffectOffSet <= 1015) then begin
              begin
                if (m_wWeaponEffectOffSet >= 1000) and (m_wWeaponEffectOffSet <= 1006) then nUnit := 1; //
                if (m_wWeaponEffectOffSet >= 1007) and (m_wWeaponEffectOffSet <= 1015) then nUnit := 2; //cboGameImages := g_cboHumEffect3;
                case m_wWeaponEffectOffSet of
                  1000:nEffectOffSet := 0;
                  1001:nEffectOffSet := 1200;
                  1002:nEffectOffSet := 6000;
                  1003:nEffectOffSet := 8400;
                  1004:nEffectOffSet := 9600;
                  1005:nEffectOffSet := 10800;
                  1006:nEffectOffSet := 13200;
                  //------------------------------------------
                  1007:nEffectOffSet := 3600;
                  1008:nEffectOffSet := 4800;
                  1009:nEffectOffSet := 6000;
                  1010:nEffectOffSet := 8400;
                  1011:nEffectOffSet := 9600;
                  1012:nEffectOffSet := 10800;
                  1013:nEffectOffSet := 14400;
                  1014:nEffectOffSet := 15600;
                  1015:nEffectOffSet := 16800;
                end;

                // 上面的nEffectOffSet偏移需要再次计算，得到cbo***.wzl里面的偏移
                case nUnit of
                  1:begin
                      if nEffectOffSet < 4800 then begin
                        nEffectOffSet := 40000 + nEffectOffSet div 600 * 2000;
                        cboGameImages := g_WCboHumEffect.Indexs[0]; //g_cboHumEffect;
                      end
                      else begin
                        nEffectOffSet := (nEffectOffSet - 4800) div 600 * 2000;
                        cboGameImages := g_WCboHumEffect.Indexs[1]; //g_cboHumEffect2;
                      end;
                    end;
                  2:begin
                      nEffectOffSet := nEffectOffSet div 600 * 2000;
                      cboGameImages := g_WCboHumEffect.Indexs[2]; //g_cboHumEffect3;
                    end;
                end;
              end;
            end
            else if (m_wWeaponEffectOffSet >= 1025) and (m_wWeaponEffectOffSet <= 2000) then begin
              if (m_wWeaponEffectOffSet >= 1100) and (m_wWeaponEffectOffSet <= 1124) then nUnit := 3; //cboWeaponEffect4.wzl
              if (m_wWeaponEffectOffSet >= 1125) and (m_wWeaponEffectOffSet <= 1149) then nUnit := 4; //cboWeaponEffect5.wzl
              case m_wWeaponEffectOffSet of
                1100:nEffectOffSet := 0; // 直接给出cboWeaponEffect4.wzl下面的偏移
                1101:nEffectOffSet := 12000;
                1102:nEffectOffSet := 24000;
                1103:nEffectOffSet := 4000;
                1104:nEffectOffSet := 16000;
                1105:nEffectOffSet := 28000;
                1106:nEffectOffSet := 8000;
                1107:nEffectOffSet := 20000;

                1108..1124:nEffectOffSet := (m_wWeaponEffectOffSet - 1100) * 4000;
                1125..1149:nEffectOffSet := (m_wWeaponEffectOffSet - 1125) * 4000;
              end;

              // 上面偏移无需再次计算
              case nUnit of
                0..2:cboGameImages := nil; // cboWeaponEffect.wzl - cboWeaponEffect3.wzl 暂无连击效果
                else
                  cboGameImages := g_CboWeaponEffectList.Indexs[nUnit];
              end;
            end;

            if cboGameImages <> nil then begin
              case m_ColorEffect of
                ceGrayScale:m_WeaponEffectSurface := cboGameImages.GetCachedGrayImage(nEffectOffSet + 2000 * m_btSex + m_nCurrentFrame, m_nWeaponEffectX, m_nWeaponEffectY);
                ceBright:m_WeaponEffectSurface := cboGameImages.GetCachedBrightImage(nEffectOffSet + 2000 * m_btSex + m_nCurrentFrame, m_nWeaponEffectX, m_nWeaponEffectY);
                else
                  m_WeaponEffectSurface := cboGameImages.GetCachedImage(nEffectOffSet + 2000 * m_btSex + m_nCurrentFrame, m_nWeaponEffectX, m_nWeaponEffectY);
              end;
            end;
          end;
        end
        else
          /////////////////////连击武器效果扩展结束 piaoyun 2013-07-29//////////////

          {//////////////////////////引擎自定义武器连击特效//////////////////////////} begin
          if (not g_ConfigDlg.ConfigCheckeds[ckHideWeaponEffect]) or (not PlugInEnabled) then begin
            if m_btCboWeaponUseDiyImage = 0 then begin
              g_EffectImageList.Lock;
              try
                if (m_nWeaponEffectIndex >= 0) and (m_nWeaponEffectIndex < g_EffectImageList.Count) then begin
                  GameImages := TGameImages(g_EffectImageList.Objects[m_nWeaponEffectIndex]);
                  if GameImages <> nil then begin
                    // 修复自定义武器特效连击时不正确 chongchong 2014-09-17
                    // 将 m_wDressEffectOffSet 改为 m_wWeaponEffectOffSet

                    cboGameImages := nil;
                    sFileName := LowerCase(ExtractFileName(GameImages.FileName));
                    if CompareLStr(sFileName, 'humeffect.w', Length('humeffect.w')) then begin
                      nEffectOffSet := m_wWeaponEffectOffSet div 600 * 2000;
                      cboGameImages := g_WCboHumEffect.Indexs[0]; //g_cboHumEffect;
                    end
                    else if CompareLStr(sFileName, 'humeffect2.w', Length('humeffect2.w')) then begin
                      if m_wWeaponEffectOffSet < 4800 then begin // humeffect2部分连击效果在 cbohumeffect中
                        nEffectOffSet := 40000 + m_wWeaponEffectOffSet div 600 * 2000;
                        cboGameImages := g_WCboHumEffect.Indexs[0]; //g_cboHumEffect;
                      end
                      else begin
                        nEffectOffSet := (m_wWeaponEffectOffSet - 4800) div 600 * 2000;
                        cboGameImages := g_WCboHumEffect.Indexs[1]; //g_cboHumEffect2;
                      end;
                    end
                    else if Pos('humeffect', sFileName) = 1 then begin
                      sTemp := Copy(sFileName, 10, MaxInt);
                      nIndex := Pos('.', sTemp);
                      if nIndex > 0 then begin
                        sTemp := Copy(sTemp, 1, nIndex - 1);

                        nIndex := StrToIntDef(sTemp, 0);
                        if nIndex > 0 then begin
                          nEffectOffSet := m_wDressEffectOffSet div 600 * 2000;
                          cboGameImages := g_WCboHumEffect.Indexs[nIndex - 1]; //g_cboHumEffect???;
                        end;
                      end;
                    end
                    else if Pos('weaponeffect', sFileName) = 1 then begin
                      sTemp := Copy(sFileName, 13, MaxInt);
                      nIndex := Pos('.', sTemp);
                      if nIndex > 0 then begin
                        sTemp := Copy(sTemp, 1, nIndex - 1);

                        nIndex := StrToIntDef(sTemp, 0);
                        if nIndex > 0 then begin
                          nEffectOffSet := m_wWeaponEffectOffSet div 600 * 2000;
                          cboGameImages := g_CboWeaponEffectList.Indexs[nIndex - 1]; //g_cboHumEffect???;
                        end;
                      end;
                    end;

                    if cboGameImages <> nil then begin
                      m_boWeaponEffectDrawNoBlend := m_boWeaponEffectNoBlend;

                      if m_boWeaponEffectNoSex then begin
                        case m_ColorEffect of
                          ceGrayScale:m_WeaponEffectSurface := cboGameImages.GetCachedGrayImage(nEffectOffSet + m_nCurrentFrame, m_nWeaponEffectX, m_nWeaponEffectY);
                          ceBright:m_WeaponEffectSurface := cboGameImages.GetCachedBrightImage(nEffectOffSet + m_nCurrentFrame, m_nWeaponEffectX, m_nWeaponEffectY);
                          else
                            m_WeaponEffectSurface := cboGameImages.GetCachedImage(nEffectOffSet + m_nCurrentFrame, m_nWeaponEffectX, m_nWeaponEffectY);
                        end;
                      end
                      else begin
                        case m_ColorEffect of
                          ceGrayScale:m_WeaponEffectSurface := cboGameImages.GetCachedGrayImage(nEffectOffSet + 2000 * m_btSex + m_nCurrentFrame, m_nWeaponEffectX, m_nWeaponEffectY);
                          ceBright:m_WeaponEffectSurface := cboGameImages.GetCachedBrightImage(nEffectOffSet + 2000 * m_btSex + m_nCurrentFrame, m_nWeaponEffectX, m_nWeaponEffectY);
                          else
                            m_WeaponEffectSurface := cboGameImages.GetCachedImage(nEffectOffSet + 2000 * m_btSex + m_nCurrentFrame, m_nWeaponEffectX, m_nWeaponEffectY);
                        end;
                      end;
                    end;
                  end;
                end;
              finally
                g_EffectImageList.UnLock;
              end;
            end

              // 连击使用默认武器外观 chongchong 2015-08-03
            else begin
              nFileIndex := (m_btCboWeaponUseDiyImage - 1) div 30;
              nImgIndex := (m_btCboWeaponUseDiyImage - 1) mod 30;

              if (nFileIndex >= 0) and (nFileIndex < CBOHUMDIYFILE_COUNT) then begin
                case m_ColorEffect of
                  ceGrayScale:m_WeaponEffectSurface := g_cboWeaponEffectDiys[nFileIndex].GetCachedGrayImage(nImgIndex * 4000 + m_btSex * 2000 + m_nCurrentFrame, m_nWeaponEffectX, m_nWeaponEffectY);
                  ceBright:m_WeaponEffectSurface := g_cboWeaponEffectDiys[nFileIndex].GetCachedBrightImage(nImgIndex * 4000 + m_btSex * 2000 + m_nCurrentFrame, m_nWeaponEffectX, m_nWeaponEffectY);
                  else
                    m_WeaponEffectSurface := g_cboWeaponEffectDiys[nFileIndex].GetCachedImage(nImgIndex * 4000 + m_btSex * 2000 + m_nCurrentFrame, m_nWeaponEffectX, m_nWeaponEffectY);
                end;
              end;
            end;
          end;
        end;
        /////////////////////////引擎自定义武器连击特效结束/////////////////////////
      end;
    end;

    // 修正追心刺最后一帧计算错误 chongchong 2014-09-21
    if (
      (m_nCurrentAction = SM_100HIT) or
      ((m_nCurrentAction >= SM_CUSTOM_PUSH001) and (m_nCurrentAction < SM_CUSTOM_PUSH001 + CUSTOM_MAGIC_COUNT))
      ) and (m_nCurrentFrame = m_nEndFrame) then
      m_nCurrentAction := 0;
  end

    {======================================非连击=================================================  }

  else begin
    // 发型偏移计算3 -- piaoyun 2013-6-10
    {if m_btHair < 6 then begin
      if (m_btHair < 4) or (m_btHorse > 0) then begin
        case m_btSex of
          0: m_nHairOffset := m_btHair * 2 * HUMANFRAME;
          1: m_nHairOffset := (m_btHair + 2) * HUMANFRAME;
        end;
      end else begin
        case m_btHair of                        // 头盔
          4: m_nHairOffset := 3600;
          5: m_nHairOffset := 4800;
        else m_nHairOffset := -1;
        end;
      end;
    end else
    begin                              // 斗笠
      m_nHairOffset := 3600 + (m_btHair - 6) * HUMANFRAME * 2 + m_btSex * HUMANFRAME;
    end;}

    case m_btHair of
      0..5:begin
          if (m_btHair < 4) or (m_btHorse > 0) then begin
            case m_btSex of
              0:m_nHairOffset := m_btHair * 2 * HUMANFRAME;
              1:m_nHairOffset := (m_btHair + 2) * HUMANFRAME;
            end;
          end
          else begin
            case m_btHair of // 头盔
              4:m_nHairOffset := 3600;
              5:m_nHairOffset := 4800;
              else
                m_nHairOffset := -1;
            end;
          end;
        end; //36，48，60，72，84，9600，10800，12000，

      { 修正发型扩展外观错误 chongchong 2013-11-21}
      50..59:begin
          m_nHairOffset := (m_btHair - 50) * HUMANFRAME {* 2 + m_btSex * HUMANFRAME};
        end;
      60..69:begin
          m_nHairOffset := (m_btHair - 60) * HUMANFRAME {* 2 + m_btSex * HUMANFRAME};
        end;

      // 斗笠 1 - 8
      100..107:begin
          m_nHairOffset := 3600 + (m_btHair - 100) * HUMANFRAME * 2 + m_btSex * HUMANFRAME;

          if g_ClientConfig.boSimpleShowHumanDress and g_ConfigDlg.ConfigCheckeds[ckSimpleShowHumanDress] then begin
            case m_btSex of
              0:m_nHairOffset := 1 * 2 * HUMANFRAME;
              1:m_nHairOffset := (1 + 2) * HUMANFRAME;
            end;
          end;
        end;

      // 斗笠 9, 10 hair3
      108..119:begin
          m_nHairOffset := 0 + (m_btHair - 108) * HUMANFRAME * 2 + m_btSex * HUMANFRAME;

          if g_ClientConfig.boSimpleShowHumanDress and g_ConfigDlg.ConfigCheckeds[ckSimpleShowHumanDress] then begin
            case m_btSex of
              0:m_nHairOffset := 1 * 2 * HUMANFRAME;
              1:m_nHairOffset := (1 + 2) * HUMANFRAME;
            end;
          end;
        end;

      // 扩展斗笠 hair4
      120..129:begin
          m_nHairOffset := 0 + (m_btHair - 120) * HUMANFRAME * 2 + m_btSex * HUMANFRAME;

          if g_ClientConfig.boSimpleShowHumanDress and g_ConfigDlg.ConfigCheckeds[ckSimpleShowHumanDress] then begin
            case m_btSex of
              0:m_nHairOffset := 1 * 2 * HUMANFRAME;
              1:m_nHairOffset := (1 + 2) * HUMANFRAME;
            end;
          end;
        end;

      // 扩展斗笠 hair5
      130..139:begin
          m_nHairOffset := 0 + (m_btHair - 130) * HUMANFRAME * 2 + m_btSex * HUMANFRAME;

          if g_ClientConfig.boSimpleShowHumanDress and g_ConfigDlg.ConfigCheckeds[ckSimpleShowHumanDress] then begin
            case m_btSex of
              0:m_nHairOffset := 1 * 2 * HUMANFRAME;
              1:m_nHairOffset := (1 + 2) * HUMANFRAME;
            end;
          end;
        end;

      // 扩展斗笠 hair6
      140..149:begin
          m_nHairOffset := 0 + (m_btHair - 140) * HUMANFRAME * 2 + m_btSex * HUMANFRAME;

          if g_ClientConfig.boSimpleShowHumanDress and g_ConfigDlg.ConfigCheckeds[ckSimpleShowHumanDress] then begin
            case m_btSex of
              0:m_nHairOffset := 1 * 2 * HUMANFRAME;
              1:m_nHairOffset := (1 + 2) * HUMANFRAME;
            end;
          end;
        end;
    end;

    if g_ClientConfig.boSimpleShowHumanDress and g_ConfigDlg.ConfigCheckeds[ckSimpleShowHumanDress] then begin
      if m_wDress = m_btSex then
        SimpleShowShape := 0
      else begin

        if not g_ConfigClient.boCustomHuamSimpleShow then begin
          case m_btJob of
            0:SimpleShowShape := 3; // 重盔甲
            1:SimpleShowShape := 4; // 魔法长袍
            2:SimpleShowShape := 5; // 灵魂战衣
            else
              SimpleShowShape := 3;
          end;
        end
        else begin
          case m_btJob of
            0:SimpleShowShape := g_ConfigClient.nSimpleDressShapeArr[0];
            1:SimpleShowShape := g_ConfigClient.nSimpleDressShapeArr[1];
            2:SimpleShowShape := g_ConfigClient.nSimpleDressShapeArr[2];
            else
              SimpleShowShape := g_ConfigClient.nSimpleDressShapeArr[0];
          end;
        end;
      end;

      case m_ColorEffect of
        ceGrayScale, ceGrayScale2:m_BodySurface := g_WHumImgImages.GetWHumGrayImg(SimpleShowShape * 2 + m_btSex, m_btSex, m_nCurrentFrame, m_nPx, m_nPy);
        ceBright:m_BodySurface := g_WHumImgImages.GetWHumBrightImg(SimpleShowShape * 2 + m_btSex, m_btSex, m_nCurrentFrame, m_nPx, m_nPy);
        else
          m_BodySurface := g_WHumImgImages.GetWHumImg(SimpleShowShape * 2 + m_btSex, m_btSex, m_nCurrentFrame, m_nPx, m_nPy);
      end;
    end
    else begin
      case m_ColorEffect of
        ceGrayScale, ceGrayScale2:m_BodySurface := g_WHumImgImages.GetWHumGrayImg(m_wDress, m_btSex, m_nCurrentFrame, m_nPx, m_nPy);
        ceBright:m_BodySurface := g_WHumImgImages.GetWHumBrightImg(m_wDress, m_btSex, m_nCurrentFrame, m_nPx, m_nPy);
        else begin
            m_BodySurface := g_WHumImgImages.GetWHumImg(m_wDress, m_btSex, m_nCurrentFrame, m_nPx, m_nPy);

            (*
            {$IF TESTMODE = 1}
              if (m_nCurrentAction > 0) and (self = g_MySelf) and (Self_nCurrentFrame <> m_nCurrentFrame) then
              begin
                DScreen.AddChatBoardString('人物图片:' + IntToStr(m_nCurrentFrame), clGreen, clBlack);
                Self_nCurrentFrame := m_nCurrentFrame;
              end;
            {$IFEND}
            *)
          end;
      end;
    end;

    if (m_BodySurface = nil) or (m_BodySurface.Width * m_BodySurface.Height <= 16) then begin
      case m_ColorEffect of
        ceGrayScale, ceGrayScale2:m_BodySurface := g_WHumImgImages.GetWHumGrayImg(0, m_btSex, m_nCurrentFrame, m_nPx, m_nPy);
        ceBright:m_BodySurface := g_WHumImgImages.GetWHumBrightImg(0, m_btSex, m_nCurrentFrame, m_nPx, m_nPy);
        else
          m_BodySurface := g_WHumImgImages.GetWHumImg(0, m_btSex, m_nCurrentFrame, m_nPx, m_nPy);
      end;
    end;

    { TODO -opiaoyun -c扩展 : 发型读取规则扩展【2013-6-10】 }
    if (m_nHairOffset >= 0) and (m_boShowHair) then begin
      // 修改为CASE语句 -- piaoyn
      case m_btHair of
        0..5:begin
            case m_ColorEffect of // 读取Hair1
              ceGrayScale:m_HairSurface := g_WHairImgImages.GetCachedGrayImage(m_nHairOffset + m_nCurrentFrame, m_nHpx, m_nHpy);
              ceBright:m_HairSurface := g_WHairImgImages.GetCachedBrightImage(m_nHairOffset + m_nCurrentFrame, m_nHpx, m_nHpy);
              else
                m_HairSurface := g_WHairImgImages.GetCachedImage(m_nHairOffset + m_nCurrentFrame, m_nHpx, m_nHpy);
            end;
          end;
        50..59: {// 读取Hair10} begin
            case m_ColorEffect of
              ceGrayScale:m_HairSurface := g_WHair10ImgImages.GetCachedGrayImage(m_nHairOffset + m_nCurrentFrame, m_nHpx, m_nHpy);
              ceBright:m_HairSurface := g_WHair10ImgImages.GetCachedBrightImage(m_nHairOffset + m_nCurrentFrame, m_nHpx, m_nHpy);
              else
                m_HairSurface := g_WHair10ImgImages.GetCachedImage(m_nHairOffset + m_nCurrentFrame, m_nHpx, m_nHpy);
            end;
          end;
        60..69: {// 读取Hair11} begin
            case m_ColorEffect of
              ceGrayScale:m_HairSurface := g_WHair11ImgImages.GetCachedGrayImage(m_nHairOffset + m_nCurrentFrame, m_nHpx, m_nHpy);
              ceBright:m_HairSurface := g_WHair11ImgImages.GetCachedBrightImage(m_nHairOffset + m_nCurrentFrame, m_nHpx, m_nHpy);
              else
                m_HairSurface := g_WHair11ImgImages.GetCachedImage(m_nHairOffset + m_nCurrentFrame, m_nHpx, m_nHpy);
            end;
          end;
        100..107:begin
            case m_ColorEffect of // 读取Hair2
              ceGrayScale:m_HairSurface := g_WHair2ImgImages.GetCachedGrayImage(m_nHairOffset + m_nCurrentFrame, m_nHpx, m_nHpy);
              ceBright:m_HairSurface := g_WHair2ImgImages.GetCachedBrightImage(m_nHairOffset + m_nCurrentFrame, m_nHpx, m_nHpy);
              else
                m_HairSurface := g_WHair2ImgImages.GetCachedImage(m_nHairOffset + m_nCurrentFrame, m_nHpx, m_nHpy);
            end;
          end;
        108..119:begin
            case m_ColorEffect of // 读取Hair3
              ceGrayScale:m_HairSurface := g_WHair3ImgImages.GetCachedGrayImage(m_nHairOffset + m_nCurrentFrame, m_nHpx, m_nHpy);
              ceBright:m_HairSurface := g_WHair3ImgImages.GetCachedBrightImage(m_nHairOffset + m_nCurrentFrame, m_nHpx, m_nHpy);
              else
                m_HairSurface := g_WHair3ImgImages.GetCachedImage(m_nHairOffset + m_nCurrentFrame, m_nHpx, m_nHpy);
            end;
          end;
        120..129:begin
            case m_ColorEffect of // 读取Hair4
              ceGrayScale:m_HairSurface := g_WHair4ImgImages.GetCachedGrayImage(m_nHairOffset + m_nCurrentFrame, m_nHpx, m_nHpy);
              ceBright:m_HairSurface := g_WHair4ImgImages.GetCachedBrightImage(m_nHairOffset + m_nCurrentFrame, m_nHpx, m_nHpy);
              else
                m_HairSurface := g_WHair4ImgImages.GetCachedImage(m_nHairOffset + m_nCurrentFrame, m_nHpx, m_nHpy);
            end;
          end;
        130..139:begin
            case m_ColorEffect of // 读取Hair5
              ceGrayScale:m_HairSurface := g_WHair5ImgImages.GetCachedGrayImage(m_nHairOffset + m_nCurrentFrame, m_nHpx, m_nHpy);
              ceBright:m_HairSurface := g_WHair5ImgImages.GetCachedBrightImage(m_nHairOffset + m_nCurrentFrame, m_nHpx, m_nHpy);
              else
                m_HairSurface := g_WHair5ImgImages.GetCachedImage(m_nHairOffset + m_nCurrentFrame, m_nHpx, m_nHpy);
            end;
          end;
        140..149:begin
            case m_ColorEffect of // 读取Hair6
              ceGrayScale:m_HairSurface := g_WHair6ImgImages.GetCachedGrayImage(m_nHairOffset + m_nCurrentFrame, m_nHpx, m_nHpy);
              ceBright:m_HairSurface := g_WHair6ImgImages.GetCachedBrightImage(m_nHairOffset + m_nCurrentFrame, m_nHpx, m_nHpy);
              else
                m_HairSurface := g_WHair6ImgImages.GetCachedImage(m_nHairOffset + m_nCurrentFrame, m_nHpx, m_nHpy);
            end;
          end;
      end;

      // 原始Mir代码 -- 注释 2013-6-10
      {if m_btHair < 6 then                      // 0 -- 5 读Hair1
      begin
        case m_ColorEffect of
          ceGrayScale: m_HairSurface := g_WHairImgImages.GetCachedGrayImage(m_nHairOffset + m_nCurrentFrame, m_nHpx, m_nHpy);
          ceBright: m_HairSurface := g_WHairImgImages.GetCachedBrightImage(m_nHairOffset + m_nCurrentFrame, m_nHpx, m_nHpy);
        else m_HairSurface := g_WHairImgImages.GetCachedImage(m_nHairOffset + m_nCurrentFrame, m_nHpx, m_nHpy);
        end;
      end else
      begin
        case m_ColorEffect of
          ceGrayScale: m_HairSurface := g_WHair2ImgImages.GetCachedGrayImage(m_nHairOffset + m_nCurrentFrame, m_nHpx, m_nHpy);
          ceBright: m_HairSurface := g_WHair2ImgImages.GetCachedBrightImage(m_nHairOffset + m_nCurrentFrame, m_nHpx, m_nHpy);
        else m_HairSurface := g_WHair2ImgImages.GetCachedImage(m_nHairOffset + m_nCurrentFrame, m_nHpx, m_nHpy);
        end;
      end; }
    end
    else
      m_HairSurface := nil;

    //////////////////////////////衣服外观//////////////////////////////////////
    ///////////////////////翅膀效果扩展 piaoyun 2013-07-29//////////////////////

    if (not (g_ClientConfig.boSimpleShowHumanDress and g_ConfigDlg.ConfigCheckeds[ckSimpleShowHumanDress])) then begin
      if ((not g_ConfigDlg.ConfigCheckeds[ckHideHumEffect]) or (not PlugInEnabled)) and (m_wEffect >= 1000) then begin
        case m_ColorEffect of
          ceGrayScale:m_HumWinSurface := g_WHumEffectImages.GetWHumEffectGrayImg(m_wEffect, m_btSex, m_nCurrentFrame, False, m_nSpx, m_nSpy, m_boDressEffNoSex); //.GetCachedGrayImage(m_nHumWinOffset + (m_btDir * 8) + m_nFrame, m_nSpx, m_nSpy);
          ceBright:m_HumWinSurface := g_WHumEffectImages.GetWHumEffectBrightImg(m_wEffect, m_btSex, m_nCurrentFrame, False, m_nSpx, m_nSpy, m_boDressEffNoSex);
          else
            m_HumWinSurface := g_WHumEffectImages.GetWHumEffectImg(m_wEffect, m_btSex, m_nCurrentFrame, False, m_nSpx, m_nSpy, m_boDressEffNoSex);
        end;
      end
        ////////////////////////////////////////////////////////////////////////////
      else if (m_wEffect = 50) then begin
        if (m_nCurrentFrame <= 536) then begin
          // 修复  Anicount = 50 字段 跑动时的衣服效果 piaoyun 2013-09-05
          if (TimeGetTime - m_dwFrameTick) > 100 then begin
            if m_nFrame < 19 then
              Inc(m_nFrame)
            else begin
              if not m_bo2D0 then
                m_bo2D0 := True
              else
                m_bo2D0 := False;
              m_nFrame := 0;
            end;
            m_dwFrameTick := TimeGetTime();
          end;

          case m_ColorEffect of
            ceGrayScale:m_HumWinSurface := g_WEffectImg.GetCachedGrayImage(m_nHumWinOffset + m_nFrame, m_nSpx, m_nSpy);
            ceBright:m_HumWinSurface := g_WEffectImg.GetCachedBrightImage(m_nHumWinOffset + m_nFrame, m_nSpx, m_nSpy);
            else
              m_HumWinSurface := g_WEffectImg.GetCachedImage(m_nHumWinOffset + m_nFrame, m_nSpx, m_nSpy);
          end;

        end;
      end
      else if (m_wEffect <> 0) then begin
        if m_nCurrentFrame < 64 then begin
          {  if (TimeGetTime - m_dwFrameTick) > m_dwFrameTime then begin
              if m_nFrame < 7 then Inc(m_nFrame)
              else m_nFrame := 0;
              m_dwFrameTick := TimeGetTime();
            end;  }

          case m_ColorEffect of
            ceGrayScale:m_HumWinSurface := g_WHumEffectImages.GetCachedGrayImage(m_nHumWinOffset + (m_btDir * 8) + m_nFrame, m_nSpx, m_nSpy);
            ceBright:m_HumWinSurface := g_WHumEffectImages.GetCachedBrightImage(m_nHumWinOffset + (m_btDir * 8) + m_nFrame, m_nSpx, m_nSpy);
            else
              m_HumWinSurface := g_WHumEffectImages.GetCachedImage(m_nHumWinOffset + (m_btDir * 8) + m_nFrame, m_nSpx, m_nSpy);
          end;
        end
        else begin
          case m_ColorEffect of
            ceGrayScale:m_HumWinSurface := g_WHumEffectImages.GetCachedGrayImage(m_nHumWinOffset + m_nCurrentFrame, m_nSpx, m_nSpy);
            ceBright:m_HumWinSurface := g_WHumEffectImages.GetCachedBrightImage(m_nHumWinOffset + m_nCurrentFrame, m_nSpx, m_nSpy);
            else
              m_HumWinSurface := g_WHumEffectImages.GetCachedImage(m_nHumWinOffset + m_nCurrentFrame, m_nSpx, m_nSpy);
          end;
        end;
      end;

      if ((not g_ConfigDlg.ConfigCheckeds[ckHideHumEffect]) or (not PlugInEnabled)) and (m_wEffect_30 >= 1000) then begin
        case m_ColorEffect of
          ceGrayScale:m_HumWinSurface_30 := g_WHumEffectImages.GetWHumEffectGrayImg(m_wEffect_30, m_btSex, m_nCurrentFrame, False, m_nSpx_30, m_nSpy_30, m_boDressEff_30NoSex); //.GetCachedGrayImage(m_nHumWinOffset + (m_btDir * 8) + m_nFrame, m_nSpx, m_nSpy);
          ceBright:m_HumWinSurface_30 := g_WHumEffectImages.GetWHumEffectBrightImg(m_wEffect_30, m_btSex, m_nCurrentFrame, False, m_nSpx_30, m_nSpy_30, m_boDressEff_30NoSex);
          else
            m_HumWinSurface_30 := g_WHumEffectImages.GetWHumEffectImg(m_wEffect_30, m_btSex, m_nCurrentFrame, False, m_nSpx_30, m_nSpy_30, m_boDressEff_30NoSex);
        end;
      end
        ////////////////////////////////////////////////////////////////////////////
      else if (m_wEffect_30 = 50) then begin
        if (m_nCurrentFrame <= 536) then begin
          // 修复  Anicount = 50 字段 跑动时的衣服效果 piaoyun 2013-09-05
          if (TimeGetTime - m_dwFrameTick) > 100 then begin
            if m_nFrame < 19 then
              Inc(m_nFrame)
            else begin
              if not m_bo2D0 then
                m_bo2D0 := True
              else
                m_bo2D0 := False;
              m_nFrame := 0;
            end;
            m_dwFrameTick := TimeGetTime();
          end;

          case m_ColorEffect of
            ceGrayScale:m_HumWinSurface_30 := g_WEffectImg.GetCachedGrayImage(m_nHumWinOffset_30 + m_nFrame, m_nSpx_30, m_nSpy_30);
            ceBright:m_HumWinSurface_30 := g_WEffectImg.GetCachedBrightImage(m_nHumWinOffset_30 + m_nFrame, m_nSpx_30, m_nSpy_30);
            else
              m_HumWinSurface_30 := g_WEffectImg.GetCachedImage(m_nHumWinOffset_30 + m_nFrame, m_nSpx_30, m_nSpy_30);
          end;

        end;
      end
      else if (m_wEffect_30 <> 0) then begin
        if m_nCurrentFrame < 64 then begin
          {  if (TimeGetTime - m_dwFrameTick) > m_dwFrameTime then begin
              if m_nFrame < 7 then Inc(m_nFrame)
              else m_nFrame := 0;
              m_dwFrameTick := TimeGetTime();
            end;  }

          case m_ColorEffect of
            ceGrayScale:m_HumWinSurface_30 := g_WHumEffectImages.GetCachedGrayImage(m_nHumWinOffset_30 + (m_btDir * 8) + m_nFrame, m_nSpx_30, m_nSpy_30);
            ceBright:m_HumWinSurface_30 := g_WHumEffectImages.GetCachedBrightImage(m_nHumWinOffset_30 + (m_btDir * 8) + m_nFrame, m_nSpx_30, m_nSpy_30);
            else
              m_HumWinSurface_30 := g_WHumEffectImages.GetCachedImage(m_nHumWinOffset_30 + (m_btDir * 8) + m_nFrame, m_nSpx_30, m_nSpy_30);
          end;
        end
        else begin
          case m_ColorEffect of
            ceGrayScale:m_HumWinSurface_30 := g_WHumEffectImages.GetCachedGrayImage(m_nHumWinOffset_30 + m_nCurrentFrame, m_nSpx_30, m_nSpy_30);
            ceBright:m_HumWinSurface_30 := g_WHumEffectImages.GetCachedBrightImage(m_nHumWinOffset_30 + m_nCurrentFrame, m_nSpx_30, m_nSpy_30);
            else
              m_HumWinSurface_30 := g_WHumEffectImages.GetCachedImage(m_nHumWinOffset_30 + m_nCurrentFrame, m_nSpx_30, m_nSpy_30);
          end;
        end;
      end;

    end;
    ////////////////////////////////衣服外观结束////////////////////////////////

    //////////////////////////////////武器外观//////////////////////////////////
    if g_ClientConfig.boSimpleShowHumanWeapon and g_ConfigDlg.ConfigCheckeds[ckSimpleShowHumanWeapon] then begin
      if m_wWeapon > 0 then begin
        if not g_ConfigClient.boCustomHuamSimpleShow then begin
          case m_btJob of
            0:SimpleShowShape := 24;
            1:SimpleShowShape := 28; // 骨玉权杖
            2:SimpleShowShape := 25;
            else
              SimpleShowShape := 24;
          end;
        end
        else begin
          case m_btJob of
            0:SimpleShowShape := g_ConfigClient.nSimpleWeaponShapeArr[0];
            1:SimpleShowShape := g_ConfigClient.nSimpleWeaponShapeArr[1];
            2:SimpleShowShape := g_ConfigClient.nSimpleWeaponShapeArr[2];
            else
              SimpleShowShape := g_ConfigClient.nSimpleWeaponShapeArr[0];
          end;
        end;

        case m_ColorEffect of
          ceGrayScale:m_WeaponSurface := g_WWeaponImages.GetWWeaponGrayImg(SimpleShowShape * 2, m_btSex, m_nCurrentFrame, m_nWpx, m_nWpy);
          ceBright:m_WeaponSurface := g_WWeaponImages.GetWWeaponBrightImg(SimpleShowShape * 2, m_btSex, m_nCurrentFrame, m_nWpx, m_nWpy);
          else
            m_WeaponSurface := g_WWeaponImages.GetWWeaponImg(SimpleShowShape * 2, m_btSex, m_nCurrentFrame, m_nWpx, m_nWpy);
        end;
      end;
    end
    else begin
      case m_ColorEffect of
        ceGrayScale:m_WeaponSurface := g_WWeaponImages.GetWWeaponGrayImg(m_wWeapon, m_btSex, m_nCurrentFrame, m_nWpx, m_nWpy);
        ceBright:m_WeaponSurface := g_WWeaponImages.GetWWeaponBrightImg(m_wWeapon, m_btSex, m_nCurrentFrame, m_nWpx, m_nWpy);
        else
          m_WeaponSurface := g_WWeaponImages.GetWWeaponImg(m_wWeapon, m_btSex, m_nCurrentFrame, m_nWpx, m_nWpy);
      end;
    end;

    if m_WeaponSurface = nil then begin
      case m_ColorEffect of
        ceGrayScale:m_WeaponSurface := g_WWeaponImages.GetWWeaponGrayImg(0, m_btSex, m_nCurrentFrame, m_nWpx, m_nWpy);
        ceBright:m_WeaponSurface := g_WWeaponImages.GetWWeaponBrightImg(0, m_btSex, m_nCurrentFrame, m_nWpx, m_nWpy);
        else
          m_WeaponSurface := g_WWeaponImages.GetWWeaponImg(0, m_btSex, m_nCurrentFrame, m_nWpx, m_nWpy);
      end;
    end;
    ////////////////////////////////武器外观结束////////////////////////////////

    // 盾牌 chongchong 2013-09-16
    if m_wShield > 0 then
      m_ShieldSurface := g_WShieldImg.GetCachedImage(HUMANFRAME * (m_wShield - 1) + m_nCurrentFrame, m_nShieldx, m_nShieldy)
    else
      m_ShieldSurface := nil;

    ///////////////////////////////摆摊效果/////////////////////////////////////
    if m_boShopStall then begin
      // m_nShopStallX  := m_nPx;
      // m_nShopStallY  := m_nPy;
      nCurrentFrame := m_nCurrentFrame div 8;
      nCurrentFrame := nCurrentFrame * 8 + 1;

      tdir := m_btDir;
      if not (m_btDir in [1, 3, 5, 7]) then begin
        tdir := m_btDir + 1;
        if tdir > 7 then tdir := 1;
      end;

      case m_ColorEffect of
        ceGrayScale:g_WHumImgImages.GetWHumGrayImg(m_wDress, m_btSex, nCurrentFrame, nX, nY);
        ceBright:g_WHumImgImages.GetWHumBrightImg(m_wDress, m_btSex, nCurrentFrame, nX, nY);
        else
          g_WHumImgImages.GetWHumImg(m_wDress, m_btSex, nCurrentFrame, nX, nY);
      end;
      case m_ColorEffect of
        ceGrayScale:begin
            case tdir of
              1:m_ShopStallSurface := g_WNewopUIImages.Grays[107];
              3:m_ShopStallSurface := g_WNewopUIImages.Grays[104];
              5:m_ShopStallSurface := g_WNewopUIImages.Grays[106];
              7:m_ShopStallSurface := g_WNewopUIImages.Grays[105];
              else
                m_ShopStallSurface := g_WNewopUIImages.Grays[107];
            end;
            m_ShopHeadSurface := g_WNewopUIImages.Grays[103];
          end;
        ceBright:begin
            case tdir of
              1:m_ShopStallSurface := g_WNewopUIImages.Brights[107];
              3:m_ShopStallSurface := g_WNewopUIImages.Brights[104];
              5:m_ShopStallSurface := g_WNewopUIImages.Brights[106];
              7:m_ShopStallSurface := g_WNewopUIImages.Brights[105];
              else
                m_ShopStallSurface := g_WNewopUIImages.Brights[107];
            end;
            m_ShopHeadSurface := g_WNewopUIImages.Brights[103];
          end;
        else begin
            case tdir of
              1:m_ShopStallSurface := g_WNewopUIImages.Images[107];
              3:m_ShopStallSurface := g_WNewopUIImages.Images[104];
              5:m_ShopStallSurface := g_WNewopUIImages.Images[106];
              7:m_ShopStallSurface := g_WNewopUIImages.Images[105];
              else
                m_ShopStallSurface := g_WNewopUIImages.Images[107];
            end;
            m_ShopHeadSurface := g_WNewopUIImages.Images[103];
          end;
      end;

      // 修正摆摊位置会在更换衣服时错位 chongchong 2015-11-09
      case tdir of
        1:begin
            m_nShopStallX := 1 - 20; // nx - 20
            m_nShopStallY := -48 + 20; // ny + 20
          end;
        3:begin
            m_nShopStallX := 4 - 30; // nx - 30
            m_nShopStallY := -47 + 40; // ny - 30
          end;
        5:begin
            m_nShopStallX := 12 - 70; // nx - 70
            m_nShopStallY := -47 + 40; // ny + 40
          end;
        7:begin
            m_nShopStallX := 9 - 70; // nx - 70
            m_nShopStallY := -47 + 10; // ny + 10
          end;
      end;
    end
    else begin
      m_ShopStallSurface := nil;
      m_ShopHeadSurface := nil;
    end;
    ///////////////////////////////摆摊效果结束/////////////////////////////////

    /////////////////////////////引擎自定义衣服特效/////////////////////////////
    if ((not g_ConfigDlg.ConfigCheckeds[ckHideHumEffect]) or (not PlugInEnabled)) and
      (not (g_ClientConfig.boSimpleShowHumanDress and g_ConfigDlg.ConfigCheckeds[ckSimpleShowHumanDress])) then begin
      g_EffectImageList.Lock;
      try
        if (m_nMedalEffectIndex >= 0) and (m_nMedalEffectIndex < g_EffectImageList.Count) then begin
          GameImages := TGameImages(g_EffectImageList.Objects[m_nMedalEffectIndex]);
          if GameImages <> nil then begin
            m_boMedalEffectDrawNoBlend := m_boMedalEffectNoBlend;

            if m_boMedalEffectNoSex then begin
              case m_ColorEffect of
                ceGrayScale:m_MedalEffectSurface := GameImages.GetCachedGrayImage(m_wMedalEffectOffSet + m_nCurrentFrame, m_nMedalEffectX, m_nMedalEffectY);
                ceBright:m_MedalEffectSurface := GameImages.GetCachedBrightImage(m_wMedalEffectOffSet + m_nCurrentFrame, m_nMedalEffectX, m_nMedalEffectY);
                else
                  m_MedalEffectSurface := GameImages.GetCachedImage(m_wMedalEffectOffSet + m_nCurrentFrame, m_nMedalEffectX, m_nMedalEffectY);
              end;
            end
            else begin
              case m_ColorEffect of
                ceGrayScale:m_MedalEffectSurface := GameImages.GetCachedGrayImage(m_wMedalEffectOffSet + HUMANFRAME * m_btSex + m_nCurrentFrame, m_nMedalEffectX, m_nMedalEffectY);
                ceBright:m_MedalEffectSurface := GameImages.GetCachedBrightImage(m_wMedalEffectOffSet + HUMANFRAME * m_btSex + m_nCurrentFrame, m_nMedalEffectX, m_nMedalEffectY);
                else
                  m_MedalEffectSurface := GameImages.GetCachedImage(m_wMedalEffectOffSet + HUMANFRAME * m_btSex + m_nCurrentFrame, m_nMedalEffectX, m_nMedalEffectY);
              end;
            end;
          end;
        end;
      finally
        g_EffectImageList.UnLock;
      end;

      if m_wHeroM2DressEffect > 0 then begin
        if (m_wHeroM2DressEffect >= 1000) then begin
          case m_ColorEffect of
            ceGrayScale:m_HeroM2DressEffect := g_WHumEffectImages.GetWHumEffectGrayImg(m_wHeroM2DressEffect, m_btSex, m_nCurrentFrame, False, m_nHeroM2DressEffectX, m_nHeroM2DressEffectY, False);
            ceBright:m_HeroM2DressEffect := g_WHumEffectImages.GetWHumEffectBrightImg(m_wHeroM2DressEffect, m_btSex, m_nCurrentFrame, False, m_nHeroM2DressEffectX, m_nHeroM2DressEffectY, False);
            else
              m_HeroM2DressEffect := g_WHumEffectImages.GetWHumEffectImg(m_wHeroM2DressEffect, m_btSex, m_nCurrentFrame, False, m_nHeroM2DressEffectX, m_nHeroM2DressEffectY, False);
          end;
        end
          ////////////////////////////////////////////////////////////////////////////
        else if (m_wHeroM2DressEffect = 50) then begin
          {
          if (m_nCurrentFrame + m_nEffigyOffset <= 536) then
          begin
            if IsGrayShow then
              m_HeroM2DressEffect := g_WEffectImg.GetCachedGrayImage(352 + m_nCurrentFrame + m_nEffigyOffset, m_nHeroM2DressEffectX, m_nHeroM2DressEffectY)
            else
              m_HeroM2DressEffect := g_WEffectImg.GetCachedImage(352 + m_nCurrentFrame + m_nEffigyOffset, m_nHeroM2DressEffectX, m_nHeroM2DressEffectY);
          end;
          }
        end
        else if (m_wHeroM2DressEffect <> 0) then begin
          case m_ColorEffect of
            ceGrayScale:m_HeroM2DressEffect := g_WHumEffectImages.GetCachedGrayImage((m_wHeroM2DressEffect - 1) * HUMANFRAME + m_nCurrentFrame, m_nHeroM2DressEffectX, m_nHeroM2DressEffectY);
            ceBright:m_HeroM2DressEffect := g_WHumEffectImages.GetCachedBrightImage((m_wHeroM2DressEffect - 1) * HUMANFRAME + m_nCurrentFrame, m_nHeroM2DressEffectX, m_nHeroM2DressEffectY);
            else
              m_HeroM2DressEffect := g_WHumEffectImages.GetCachedImage((m_wHeroM2DressEffect - 1) * HUMANFRAME + m_nCurrentFrame, m_nHeroM2DressEffectX, m_nHeroM2DressEffectY);
          end;
        end;

        (*
          case m_ColorEffect of
            ceGrayScale: m_HeroM2DressEffect := g_WHumWingImages.GetCachedGrayImage((m_wHeroM2DressEffect - 1) * HUMANFRAME + m_nCurrentFrame, m_nHeroM2DressEffectX, m_nHeroM2DressEffectY);
            ceBright: m_HeroM2DressEffect := g_WHumWingImages.GetCachedBrightImage((m_wHeroM2DressEffect - 1) * HUMANFRAME + m_nCurrentFrame, m_nHeroM2DressEffectX, m_nHeroM2DressEffectY);
          else
            m_HeroM2DressEffect := g_WHumWingImages.GetCachedImage((m_wHeroM2DressEffect - 1) * HUMANFRAME + m_nCurrentFrame, m_nHeroM2DressEffectX, m_nHeroM2DressEffectY);
          end;
        end;
        *)
      end;

      g_EffectImageList.Lock;
      try
        if (m_nDressEffectIndex >= 0) and (m_nDressEffectIndex < g_EffectImageList.Count) then begin
          GameImages := TGameImages(g_EffectImageList.Objects[m_nDressEffectIndex]);
          if GameImages <> nil then begin
            m_boDressEffectDrawNoBlend := m_boDressEffectNoBlend;

            if m_boDressEffectNoSex then begin
              case m_ColorEffect of
                ceGrayScale:m_DressEffectSurface := GameImages.GetCachedGrayImage(m_wDressEffectOffSet + m_nCurrentFrame, m_nDressEffectX, m_nDressEffectY);
                ceBright:m_DressEffectSurface := GameImages.GetCachedBrightImage(m_wDressEffectOffSet + m_nCurrentFrame, m_nDressEffectX, m_nDressEffectY);
                else
                  m_DressEffectSurface := GameImages.GetCachedImage(m_wDressEffectOffSet + m_nCurrentFrame, m_nDressEffectX, m_nDressEffectY);
              end;
            end
            else begin
              case m_ColorEffect of
                ceGrayScale:m_DressEffectSurface := GameImages.GetCachedGrayImage(m_wDressEffectOffSet + HUMANFRAME * m_btSex + m_nCurrentFrame, m_nDressEffectX, m_nDressEffectY);
                ceBright:m_DressEffectSurface := GameImages.GetCachedBrightImage(m_wDressEffectOffSet + HUMANFRAME * m_btSex + m_nCurrentFrame, m_nDressEffectX, m_nDressEffectY);
                else
                  m_DressEffectSurface := GameImages.GetCachedImage(m_wDressEffectOffSet + HUMANFRAME * m_btSex + m_nCurrentFrame, m_nDressEffectX, m_nDressEffectY);
              end;
            end;
          end;
        end;
      finally
        g_EffectImageList.UnLock;
      end;
    end;
    /////////////////////////////引擎自定义衣服特效结束/////////////////////////

    ///////////////////////////////引擎自定义武器特效///////////////////////////
    if m_WeaponSurface <> nil then begin
      if ((not g_ConfigDlg.ConfigCheckeds[ckHideWeaponEffect]) or (not PlugInEnabled)) and
        (not (g_ClientConfig.boSimpleShowHumanWeapon and g_ConfigDlg.ConfigCheckeds[ckSimpleShowHumanWeapon])) then begin
        g_EffectImageList.Lock;
        try
          if (m_nWeaponEffectIndex >= 0) and (m_nWeaponEffectIndex < g_EffectImageList.Count) then begin
            GameImages := TGameImages(g_EffectImageList.Objects[m_nWeaponEffectIndex]);
            if GameImages <> nil then begin
              m_boWeaponEffectDrawNoBlend := m_boWeaponEffectNoBlend;

              if m_boWeaponEffectNoSex then begin
                case m_ColorEffect of
                  ceGrayScale:m_WeaponEffectSurface := GameImages.GetCachedGrayImage(m_wWeaponEffectOffSet + m_nCurrentFrame, m_nWeaponEffectX, m_nWeaponEffectY);
                  ceBright:m_WeaponEffectSurface := GameImages.GetCachedBrightImage(m_wWeaponEffectOffSet + m_nCurrentFrame, m_nWeaponEffectX, m_nWeaponEffectY);
                  else
                    m_WeaponEffectSurface := GameImages.GetCachedImage(m_wWeaponEffectOffSet + m_nCurrentFrame, m_nWeaponEffectX, m_nWeaponEffectY);
                end;
              end
              else begin
                case m_ColorEffect of
                  ceGrayScale:m_WeaponEffectSurface := GameImages.GetCachedGrayImage(m_wWeaponEffectOffSet + HUMANFRAME * m_btSex + m_nCurrentFrame, m_nWeaponEffectX, m_nWeaponEffectY);
                  ceBright:m_WeaponEffectSurface := GameImages.GetCachedBrightImage(m_wWeaponEffectOffSet + HUMANFRAME * m_btSex + m_nCurrentFrame, m_nWeaponEffectX, m_nWeaponEffectY);
                  else
                    m_WeaponEffectSurface := GameImages.GetCachedImage(m_wWeaponEffectOffSet + HUMANFRAME * m_btSex + m_nCurrentFrame, m_nWeaponEffectX, m_nWeaponEffectY);
                end;
              end;

            end;
          end;

          //else  2020-11-11 00:52:50

          begin
            /////////////////////武器特效扩展 piaoyun 2013-07-28////////////////
            case m_wDBWeaponEffectOffSet of
              1000..1015: {// 绘制DB库扩展特效 piaoyun 2013-07-28} begin
                  case m_ColorEffect of
                    ceGrayScale:m_DBWeaponEffectSurface := g_WHumEffectImages.GetWHumEffectGrayImg(m_wDBWeaponEffectOffSet, m_btSex, m_nCurrentFrame, True, m_nDBWeaponEffectX, m_nDBWeaponEffectY, False);
                    ceBright:m_DBWeaponEffectSurface := g_WHumEffectImages.GetWHumEffectBrightImg(m_wDBWeaponEffectOffSet, m_btSex, m_nCurrentFrame, True, m_nDBWeaponEffectX, m_nDBWeaponEffectY, False);
                    else
                      m_DBWeaponEffectSurface := g_WHumEffectImages.GetWHumEffectImg(m_wDBWeaponEffectOffSet, m_btSex, m_nCurrentFrame, True, m_nDBWeaponEffectX, m_nDBWeaponEffectY, False);
                  end;
                end;
              1025..2000: {// 绘制扩展特效 WeaponEffect.wzl -- WeaponEffect5.wzl} begin
                  case m_ColorEffect of
                    ceGrayScale:m_DBWeaponEffectSurface := g_WeaponEffectList.GetWWeaponEffectGrayImg(m_wDBWeaponEffectOffSet, m_btSex, m_nCurrentFrame, m_nDBWeaponEffectX, m_nDBWeaponEffectY);
                    ceBright:m_DBWeaponEffectSurface := g_WeaponEffectList.GetWWeaponEffectBrightImg(m_wDBWeaponEffectOffSet, m_btSex, m_nCurrentFrame, m_nDBWeaponEffectX, m_nDBWeaponEffectY);
                    else
                      m_DBWeaponEffectSurface := g_WeaponEffectList.GetWWeaponEffectImg(m_wDBWeaponEffectOffSet, m_btSex, m_nCurrentFrame, m_nDBWeaponEffectX, m_nDBWeaponEffectY);
                  end;
                end;
            end;
            ///////////////////武器特效扩展结束 piaoyun 2013-07-28//////////////
          end;
        finally
          g_EffectImageList.UnLock;
        end;
      end;
    end;

    if (not g_ConfigDlg.ConfigCheckeds[ckHideWeaponEffect]) or (not PlugInEnabled) then begin
      g_EffectImageList.Lock;
      try
        if (m_nShieldEffectIndex >= 0) and (m_nShieldEffectIndex < g_EffectImageList.Count) then begin
          GameImages := TGameImages(g_EffectImageList.Objects[m_nShieldEffectIndex]);
          if GameImages <> nil then begin
            m_boShieldEffectDrawNoBlend := m_boShieldEffectNoBlend;

            if m_boShieldEffectNoSex then begin
              case m_ColorEffect of
                ceGrayScale:m_ShieldEffectSurface := GameImages.GetCachedGrayImage(m_wShieldEffectOffSet + m_nCurrentFrame, m_nShieldEffectX, m_nShieldEffectY);
                ceBright:m_ShieldEffectSurface := GameImages.GetCachedBrightImage(m_wShieldEffectOffSet + m_nCurrentFrame, m_nShieldEffectX, m_nShieldEffectY);
                else
                  m_ShieldEffectSurface := GameImages.GetCachedImage(m_wShieldEffectOffSet + m_nCurrentFrame, m_nShieldEffectX, m_nShieldEffectY);
              end;
            end
            else begin
              case m_ColorEffect of
                ceGrayScale:m_ShieldEffectSurface := GameImages.GetCachedGrayImage(m_wShieldEffectOffSet + HUMANFRAME * m_btSex + m_nCurrentFrame, m_nShieldEffectX, m_nShieldEffectY);
                ceBright:m_ShieldEffectSurface := GameImages.GetCachedBrightImage(m_wShieldEffectOffSet + HUMANFRAME * m_btSex + m_nCurrentFrame, m_nShieldEffectX, m_nShieldEffectY);
                else
                  m_ShieldEffectSurface := GameImages.GetCachedImage(m_wShieldEffectOffSet + HUMANFRAME * m_btSex + m_nCurrentFrame, m_nShieldEffectX, m_nShieldEffectY);
              end;
            end;

          end;
        end;
      finally
        g_EffectImageList.UnLock;
      end;
    end;
  end;

  LoadActorIcons;
  ActionChanged;
end;

procedure THumActor.DrawChr(dx, dy:Integer; blend:Boolean; boFlag:Boolean);

  procedure DrawSelfMagicEffect(AMagicID:WORD; BeforeDraw:Boolean; IsWarrDraw:Boolean);
  var
    CustomMagicConfig:PClientCustomMagicConfig;
    MagicPlusLevel:TMagicPlusLevel;
    ClientConfig:PMagicClientConfig;

    giSelfEffect:TGameImages;
    SelfEffectSurface:TTexture;
    px, py:Integer;

    IsDraw:Boolean;
    nDir:Integer;

    PlayFrameCount:Integer;
  begin
    if not IsWarrDraw then begin
      if not m_boUseMagic then Exit;
    end;

    CustomMagicConfig := GetCustomMagicConfig(AMagicID);

    ClientConfig := nil;

    if CustomMagicConfig <> nil then begin
      case m_CurMagic.NewLevel of
        0:MagicPlusLevel := mplNone;
        1..3:MagicPlusLevel := mpl1_3;
        4..6:MagicPlusLevel := mpl4_6;
        7..9:MagicPlusLevel := mpl7_9;
        else
          MagicPlusLevel := mpl7_9;
      end;

      ClientConfig := @CustomMagicConfig.MagicConfigs[MagicPlusLevel];
    end;

    if ClientConfig = nil then Exit;

    if ClientConfig.Self_SyncHumAction then begin
      m_nCurSelfEffFrame := m_nCurrentFrame - m_nStartFrame;
      PlayFrameCount := m_nEndFrame - m_nStartFrame;
    end else begin
      PlayFrameCount := ClientConfig.Self_PlayCount;

      // 自身动画支持自定义播放时间 chongchong 2014-09-04
      if not IsWarrDraw then begin
        if Integer(TimeGetTime - Cardinal(m_dwCurSelfEffFrameTick)) >= ClientConfig.Self_PlayTime - 10 then begin //HZQ 20230525
          if m_nCurSelfEffFrame < ClientConfig.Self_PlayCount - 0 then
            Inc(m_nCurSelfEffFrame);
          m_dwCurSelfEffFrameTick := TimeGetTime;

          //DScreen.AddChatBoardString('帧:' + IntToStr(m_nCurSelfEffFrame), 0, 255)
        end;
      end else begin
        if Integer(TimeGetTime - Cardinal(m_dwCurSelfEffFrameTick)) >= ClientConfig.Self_PlayTime - 10 then begin //HZQ 20230525
          if m_nCurSelfEffFrame < ClientConfig.Self_PlayCount - 0 then
            Inc(m_nCurSelfEffFrame);
          m_dwCurSelfEffFrameTick := TimeGetTime;

          //DScreen.AddChatBoardString('帧:' + IntToStr(m_nCurSelfEffFrame), 0, 255)
        end;
      end;
    end;

    if PlayFrameCount = 0 then Exit;

    // 非8方向攻击特效 chongchong 2014-09-12
    if Self = g_MySelf then
      IsDraw := (ClientConfig.Self_File >= 0) and
        (ClientConfig.Self_File < g_EffectImageList.Count) and
        (((ClientConfig.Self_DrawOrder = mdoPriorMagic) and BeforeDraw and boFlag) or
        ((ClientConfig.Self_DrawOrder = mdoPriorSelf) and (not BeforeDraw) and ((not boFlag) or (g_MySelf.m_nState and $00800000 <> 0)))) and // 如果自己是隐身状态，根本不会有 boFlag = False 的来 PlayScn.3674行
      (ClientConfig.Self_DirCalcType <> mdctCenter)
    else
      IsDraw := (ClientConfig.Self_File >= 0) and
        (ClientConfig.Self_File < g_EffectImageList.Count) and
        (((ClientConfig.Self_DrawOrder = mdoPriorMagic) and BeforeDraw) or
        ((ClientConfig.Self_DrawOrder = mdoPriorSelf) and (not BeforeDraw))) and
        (ClientConfig.Self_DirCalcType <> mdctCenter);

    m_nMagLight := ClientConfig.Self_LightRange;

    // 修改自定义技能播放自身动作 换成 m_nCurEffFrame 2019-04-28 18:06:14

    if IsDraw then begin
      if {(ClientConfig.Self_StartIndex >= 0) and}(PlayFrameCount > 0) //HZQ Self_StartIndex:WORD
      and (m_nCurSelfEffFrame {m_nCurEffFrame} in [0..PlayFrameCount - 1])
        and (not ClientConfig.Self_PlayFailNoDraw) or (m_CurMagic.ServerMagicCode > 0) or IsWarrDraw then begin
        giSelfEffect := TGameImages(g_EffectImageList.Objects[ClientConfig.Self_File]);

        // 修正战士技能 自身技能8方向计算错误 chongchong 2016-08-04
        if ((m_CurMagic.targx = -1) and (m_CurMagic.targy = -1)) or (Self <> g_MySelf) or CustomMagicConfig.NoChangeDir then
          nDir := m_btDir
        else
          nDir := GetNextDirection(m_nCurrX, m_nCurrY, m_CurMagic.targx, m_CurMagic.targy);

        SelfEffectSurface := nil;

        if giSelfEffect <> nil then begin
          if ClientConfig.Self_DirCalcType = mdctNone then begin
            SelfEffectSurface := giSelfEffect.GetCachedImage(ClientConfig.Self_StartIndex + m_nCurSelfEffFrame {m_nCurEffFrame}, px, py);
          end
          else if ClientConfig.Self_DirCalcType = mdctNormal then begin
            SelfEffectSurface := giSelfEffect.GetCachedImage(ClientConfig.Self_StartIndex +
              nDir * (ClientConfig.Self_PlayCount + ClientConfig.Self_EmptyCount) +
              m_nCurSelfEffFrame {m_nCurEffFrame}, px, py);
          end;

          //OutputDebugString(PChar('~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ DIR:' + IntToStr(nDir) + '; Index' + IntToStr(m_nCurSelfEffFrame)));

          if SelfEffectSurface <> nil then begin
            if ClientConfig.Self_DrawMode = mdmBlend then begin
              GameCanvas.DrawBlend(
                dx + px + m_nShiftX,
                dy + py + m_nShiftY,
                SelfEffectSurface);
            end
            else begin
              GameCanvas.Draw(
                dx + px + m_nShiftX,
                dy + py + m_nShiftY,
                SelfEffectSurface);
            end;
          end;
        end;
      end;
    end;
  end;

var
  idx, ax, ay:Integer;
  d:TTexture;
  wimg:TGameImages;
  nError:Integer;
  nX, nY, nX2, nY2:Integer;
begin
  //if CheckLoadActorIcon then LoadActorIcons;
  if CheckLoadDressAddEffect then LoadDressAddEffect;

  nError := 0;
  try
    // 战士自身效果绘制改成跟魔法效果绘制一样，主要是为了支持自定义时间间隔 chongchong 2016-03-16
    if m_boHitEffect and (m_nHitEffectNumber <= -SM_CUSTOM_HIT001) and (m_nHitEffectNumber > -(SM_CUSTOM_HIT001 + CUSTOM_MAGIC_COUNT)) then begin
      DrawSelfMagicEffect(Abs(m_nHitEffectNumber) - SM_CUSTOM_HIT001 + CUSTOM_MAGIC_START_ID, True, True);
    end else begin
      DrawSelfMagicEffect(m_CurMagic.MagicSerial, True, False);
    end;

    if not (m_btDir in [0..7]) then begin
      Exit;
    end;

    //DrawPlayEffect(dx, dy, True);

    if (m_btRace = 0) and m_boShopStall and (m_ShopStallSurface <> nil) and (not (m_btDir in [4, 5])) then begin // 画摆摊位置
      GameCanvas.Draw(dx + m_nShopStallX + m_nShiftX, dy + m_nShopStallY + m_nShiftY, m_ShopStallSurface.ClientRect, m_ShopStallSurface); // 画摆摊位置
    end;

    {$IF Enabled_PlugEngine = 1}
    if Assigned(HookTHumActor_DrawChr1) then begin
      try
        HookTHumActor_DrawChr1(Self, dx, dy, blend, boFlag);
      except
        on E:Exception do begin
          DebugOutStr('[Exception] HookTHumActor_DrawChr1');
          DebugOutStr(E.Message);
        end;
      end;
    end;
    {$IFEND}

    if m_btRace in [0, 1] then begin
      if (m_nCurrentFrame >= 0) and (m_nCurrentFrame <= 599) then
        m_nWpord := WORDER[m_btSex, m_nCurrentFrame];
      nError := 1;

      // 第二次绘制翅膀不要 + (not blend) 2020-03-25 15:35:38
      if (not blend) and ((m_wEffect <> 0) or (m_DressEffectSurface <> nil) or (m_MedalEffectSurface <> nil) or (m_HeroM2DressEffect <> nil) or (m_ActorEffects.Count > 0)) then begin
        if (m_btDir in [3, 4, 5]) then
          DrawDressEffect(dx, dy, blend, m_ColorEffect);
      end;

      if (m_nWpord = 0) and (not blend) and (m_wWeapon >= 2) and (m_WeaponSurface <> nil) and (not m_boHideWeapon) then begin
        nError := 11;
        if m_boShowPhantom then
          StretchDrawEffSurface(m_WeaponSurface, dx + Round(m_nWpx * 1.5) + m_nShiftX, dy + Round(m_nWpy * 1.5) + m_nShiftY, blend, ceNone);
        DrawEffSurface(m_WeaponSurface, dx + m_nWpx + m_nShiftX, dy + m_nWpy + m_nShiftY, blend, ceNone);
        nError := 12;
        DrawWeaponGlimmer(dx, dy);
        nError := 13;
      end;

      // 盾牌 不透明(人的后面) chongchong 2013-09-16
      //if (m_ShieldSurface <> nil) and (m_nWpord = 1) and (not blend) and (m_wShield > 0) then
      if (m_ShieldSurface <> nil) and (m_nWpord = 1) and (not blend) and (m_wShield > 0) and (m_btDir <> 5) then begin
        DrawEffSurface(m_ShieldSurface, dx + m_nShieldx + m_nShiftX, dy + m_nShieldy + m_nShiftY, blend, ceNone);
        DrawShieldEffect(dx, dy);
      end;

      // 骑马 chongchong 2013-10-16
      if m_HorseSurface <> nil then
        DrawEffSurface(m_HorseSurface, dx + m_nPx + m_nShiftX, dy + m_nPy + m_nShiftY, blend, m_ColorEffect);

      // 骑马 特效  chongchong 2013-09-16
      if (m_HorseWingsEffectSurface <> nil) then begin
        GameCanvas.DrawBlend(dx + m_nHorseWingsEffectX + m_nShiftX, dy + m_nHorseWingsEffectY + m_nShiftY, m_HorseWingsEffectSurface);
      end;

      // 骑马 特效  chongchong 2013-09-16
      if (m_HorseEffectSurface <> nil) then begin
        GameCanvas.DrawBlend(dx + m_nHorseEffectX + m_nShiftX, dy + m_nHorseEffectY + m_nShiftY, m_HorseEffectSurface);
      end;

      // 骑马 三方马上人物
      if m_HorseHumSurface <> nil then
        DrawEffSurface(m_HorseHumSurface, dx + m_nHorseHumX + m_nShiftX, dy + m_nHorseHumY + m_nShiftY, blend, m_ColorEffect);

      // 骑马 三方马上人物发型
      if m_HorseHairSurface <> nil then
        DrawEffSurface(m_HorseHairSurface, dx + m_nHorseHairX + m_nShiftX, dy + m_nHorseHairY + m_nShiftY, blend, m_ColorEffect);

      nError := 14;

      // 绘制衣服附加特效 - 衣服下层 chongchong 2015-06-16
      if (m_DressAddEffectSurface <> nil) and (m_bDressAddEffectOrder <> 0) then begin
        if m_boDressAddEffectDrawCenter then begin
          if m_BodySurface <> nil then begin
            if not m_boDressAddEffectNoBlend then
              GameCanvas.DrawBlend(dx + m_nPx + m_nShiftX + (m_BodySurface.Width - m_DressAddEffectSurface.Width) div 2,
                dy + m_nPy + m_nShiftY + (m_BodySurface.Height - m_DressAddEffectSurface.Height) div 2, m_DressAddEffectSurface)
            else
              GameCanvas.Draw(dx + m_nPx + m_nShiftX + (m_BodySurface.Width - m_DressAddEffectSurface.Width) div 2,
                dy + m_nPy + m_nShiftY + (m_BodySurface.Height - m_DressAddEffectSurface.Height) div 2, m_DressAddEffectSurface);
          end;
        end
        else begin
          if not m_boDressAddEffectNoBlend then
            GameCanvas.DrawBlend(dx + m_nDressAddEffectX + m_nShiftX, dy + m_nDressAddEffectY + m_nShiftY, m_DressAddEffectSurface)
          else
            GameCanvas.Draw(dx + m_nDressAddEffectX + m_nShiftX, dy + m_nDressAddEffectY + m_nShiftY, m_DressAddEffectSurface);
        end;
      end;

      if m_BodySurface <> nil then begin
        if m_boShowPhantom then
          StretchDrawEffSurface(m_BodySurface, dx + Round(m_nPx * 1.5) + m_nShiftX, dy + Round(m_nPy * 1.5) + m_nShiftY, blend, m_ColorEffect);
        DrawEffSurface(m_BodySurface, dx + m_nPx + m_nShiftX, dy + m_nPy + m_nShiftY, blend, m_ColorEffect);
        DrawStateEffSurface(dx + m_nShiftX, dy + m_nShiftY);
      end;

      // 绘制衣服附加特效 - 衣服上层 chongchong 2015-06-16
      if (m_DressAddEffectSurface <> nil) and (m_bDressAddEffectOrder = 0) then begin
        if m_boDressAddEffectDrawCenter then begin
          if m_BodySurface <> nil then begin
            if not m_boDressAddEffectNoBlend then
              GameCanvas.DrawBlend(dx + m_nPx + m_nShiftX + (m_BodySurface.Width - m_DressAddEffectSurface.Width) div 2,
                dy + m_nPy + m_nShiftY + (m_BodySurface.Height - m_DressAddEffectSurface.Height) div 2, m_DressAddEffectSurface)
            else
              GameCanvas.Draw(dx + m_nPx + m_nShiftX + (m_BodySurface.Width - m_DressAddEffectSurface.Width) div 2,
                dy + m_nPy + m_nShiftY + (m_BodySurface.Height - m_DressAddEffectSurface.Height) div 2, m_DressAddEffectSurface);
          end;
        end
        else begin
          if not m_boDressAddEffectNoBlend then
            GameCanvas.DrawBlend(dx + m_nDressAddEffectX + m_nShiftX, dy + m_nDressAddEffectY + m_nShiftY, m_DressAddEffectSurface)
          else
            GameCanvas.Draw(dx + m_nDressAddEffectX + m_nShiftX, dy + m_nDressAddEffectY + m_nShiftY, m_DressAddEffectSurface);
        end;
      end;

      nError := 15;
      if m_HairSurface <> nil then begin
        if m_boShowPhantom then
          StretchDrawEffSurface(m_HairSurface, dx + Round(m_nHpx * 1.5) + m_nShiftX, dy + Round(m_nHpy * 1.5) + m_nShiftY, blend, m_ColorEffect);
        DrawEffSurface(m_HairSurface, dx + m_nHpx + m_nShiftX, dy + m_nHpy + m_nShiftY, blend, m_ColorEffect);
      end;

      nError := 16;
      if (m_nWpord = 1) and {(not blend) and}(m_wWeapon >= 2) and (m_WeaponSurface <> nil) and (not m_boHideWeapon) then begin
        nError := 17;
        if m_boShowPhantom then
          StretchDrawEffSurface(m_WeaponSurface, dx + Round(m_nWpx * 1.5) + m_nShiftX, dy + Round(m_nWpy * 1.5) + m_nShiftY, blend, ceNone);
        DrawEffSurface(m_WeaponSurface, dx + m_nWpx + m_nShiftX, dy + m_nWpy + m_nShiftY, blend, ceNone);
        nError := 18;
        DrawWeaponGlimmer(dx, dy);
        nError := 19;
      end;

      // 盾牌 透明(挡在人前面) chongchong 2013-09-16
      //if (m_ShieldSurface <> nil) and (m_nWpord = 0) and (m_wShield > 0) then
      if (m_ShieldSurface <> nil) and (((m_nWpord = 0) and (m_wShield > 0)) or ((m_nWpord = 1) and (m_btDir = 5))) then begin
        DrawEffSurface(m_ShieldSurface, dx + m_nShieldx + m_nShiftX, dy + m_nShieldy + m_nShiftY, blend, ceNone);
        DrawShieldEffect(dx, dy);
      end;

      if (m_wEffect = 50) then begin
        nError := 20;
        DrawDressEffect(dx, dy, blend, m_ColorEffect);
        nError := 21;
      end
      else if (m_wEffect <> 0) or (m_DressEffectSurface <> nil) or (m_MedalEffectSurface <> nil) or (m_HeroM2DressEffect <> nil) or (m_ActorEffects.Count > 0) then begin
        if m_btDir in [0, 1, 2, 6, 7] then
          DrawDressEffect(dx, dy, blend, m_ColorEffect);
      end;
    end;
    nError := 30;

    {$IF Enabled_PlugEngine = 1}
    if Assigned(HookTHumActor_DrawChr2) then begin
      try
        HookTHumActor_DrawChr2(Self, dx, dy, blend, boFlag);
      except
        on E:Exception do begin
          DebugOutStr('[Exception] HookTHumActor_DrawChr2');
          DebugOutStr(E.Message);
        end;
      end;
    end;
    {$IFEND}

    nError := 301;
    //DrawPlayEffect(dx, dy, False);

    nError := 302;
    {$IF Enabled_PlugEngine = 1}
    if Assigned(HookTHumActor_DrawChr3) then begin
      try
        HookTHumActor_DrawChr3(Self, dx, dy, blend, boFlag);
      except
        on E:Exception do begin
          DebugOutStr('[Exception] HookTHumActor_DrawChr3');
          DebugOutStr(E.Message);
        end;
      end;
    end;
    {$IFEND}

    nError := 31;

    if (m_btRace = 0) and m_boShopStall and (m_ShopStallSurface <> nil) and (m_btDir in [4, 5]) then begin // 画摆摊位置
      GameCanvas.Draw(dx + m_nShopStallX + m_nShiftX, dy + m_nShopStallY + m_nShiftY, m_ShopStallSurface.ClientRect, m_ShopStallSurface);
    end;

    // 绘制摆摊顶部图片 piaoyun 2013-09-13
    if (m_btRace = 0) and (m_boShopStall) and (g_ClientConfig.boShopHeadPic) and (m_ShopHeadSurface <> nil) then begin
      nX := m_nSayX - 5 - {m_NumberLableImageInfo.Width} 54 div 2;
      nY := m_nSayY - 26;
      GameCanvas.Draw(nX, nY, m_ShopHeadSurface.ClientRect, m_ShopHeadSurface);
    end;

    if m_boBrokenShield and (m_btHorse = 0) then begin
      idx := 4100 + m_nBrokenShieldEffect;
      if (g_MySelf <> nil) and g_MySelf.m_boDeath then
        d := g_cboEffect.GetCachedGrayImage(idx, ax, ay)
      else
        d := g_cboEffect.GetCachedImage(idx, ax, ay);
      if d <> nil then
        GameCanvas.DrawBlend(
          dx + ax + m_nShiftX,
          dy + ay + m_nShiftY,
          d);
    end
    else begin
      nError := 32;
      // 骑马的时候不显示魔法盾时效果 chongchong 2014-05-15
      if (m_nState and $00100000 {STATE_BUBBLEDEFENCEUP} <> 0) and (m_btHorse = 0) {and not m_boDeath} then begin
        if not g_ClientConfig.boSkill31UseNewEffect then begin
          if (m_nCurrentAction = SM_STRUCK) and (m_nCurBubbleStruck < 3) then
            idx := MAGBUBBLESTRUCKBASE + m_nCurBubbleStruck
          else
            idx := MAGBUBBLEBASE + (m_nGenAniCount mod 3);

          if (g_MySelf <> nil) and g_MySelf.m_boDeath then
            d := g_WMagicImages.GetCachedGrayImage(idx, ax, ay)
          else
            d := g_WMagicImages.GetCachedImage(idx, ax, ay);
        end
        else {// 使用新魔法盾效果  chongchong 2015-07-25} begin
          if (m_nCurrentAction = SM_STRUCK) and (m_nCurBubbleStruck < 3) then
            idx := 920 + m_nCurBubbleStruck
          else
            idx := 910 + (m_nGenAniCount mod 3);

          if (g_MySelf <> nil) and g_MySelf.m_boDeath then
            d := g_WMagicreImages.GetCachedGrayImage(idx, ax, ay)
          else
            d := g_WMagicreImages.GetCachedImage(idx, ax, ay);

          // 强制修正新魔法盾坐标 chongchong 2015-07-26
          ax := ax + 0;
          ay := ay - 6;
        end;

        if d <> nil then begin
          GameCanvas.DrawBlend(
            dx + ax + m_nShiftX,
            dy + ay + m_nShiftY,
            d);
          // GameCanvas.StretchDraw(Bounds(dx + ax + m_nShiftX, dy + ay + m_nShiftY, Round(d.Width * 1.6), Round(d.Height * 1.6)), d.ClientRect, d, fxAnti);
        end;
      end;

      // 4级魔法盾
      if (m_nState and $00008000 {STATE_16} <> 0) and (m_btHorse = 0) {and not m_boDeath} then begin
        if (m_nCurrentAction = SM_STRUCK) and (m_nCurBubbleStruck < 3) then
          idx := 723 + m_nCurBubbleStruck
        else
          idx := 720 + (m_nGenAniCount mod 3);

        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          d := g_WMagic6Images.GetCachedGrayImage(idx, ax, ay)
        else
          d := g_WMagic6Images.GetCachedImage(idx, ax, ay);

        if d <> nil then begin
          GameCanvas.DrawBlend(
            dx + ax + m_nShiftX,
            dy + ay + m_nShiftY,
            d);
          // GameCanvas.StretchDraw(Bounds(dx + ax + m_nShiftX, dy + ay + m_nShiftY, Round(d.Width * 1.6), Round(d.Height * 1.6)), d.ClientRect, d, fxAnti);
        end;
      end;
      nError := 33;

      nError := 32;
      // 骑马的时候不显示新武力盾时效果 chongchong 2014-05-15
      if (m_nState and $00040000 {STATE_NEWHITBUBBLEDEFENCEUP} <> 0) and (m_btHorse = 0) {and not m_boDeath} then begin
        idx := 494;
        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          d := g_WMagic6Images.GetCachedGrayImage(idx, ax, ay)
        else
          d := g_WMagic6Images.GetCachedImage(idx, ax, ay);

        if d <> nil then begin

          GameCanvas.DrawBlend(
            dx + ax + m_nShiftX,
            dy + ay + m_nShiftY,
            d);
        end;

        {
        idx := 480 + m_nGenNewHitAniCount mod 7;

        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          d := g_WMagic6Images.GetCachedGrayImage(idx, ax, ay)
        else
          d := g_WMagic6Images.GetCachedImage(idx, ax, ay);
        if d <> nil then
        begin
          GameCanvas.DrawBlend(
            dx + ax + m_nShiftX,
            dy + ay + m_nShiftY,
            d);
        end;
        }
      end;
      nError := 33;

      // 骑马的时候不显示新道力盾时效果 chongchong 2014-05-15
      if (m_nState and $00020000 {STATE_NEWHITBUBBLEDEFENCEUP} <> 0) and (m_btHorse = 0) {and not m_boDeath} then begin
        {
        if (m_nCurrentAction = SM_STRUCK) and (m_nCurNewMagBubbleStruck < 3) then
          idx := 3900 + m_nCurNewMagBubbleStruck
        else
          idx := 3890 + (m_nGenNewMagAniCount mod 3);

        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          d := g_WMagicImages.GetCachedGrayImage(idx, ax, ay)
        else
          d := g_WMagicImages.GetCachedImage(idx, ax, ay);
        if d <> nil then
        begin
          GameCanvas.DrawBlend(
            dx + ax + m_nShiftX,
            dy + ay + m_nShiftY,
            d);
        end;
        }

        if (m_nCurrentAction = SM_STRUCK) and (m_nCurNewMagBubbleStruck < 3) then
          idx := 723 + m_nCurNewMagBubbleStruck
        else
          idx := 720 + (m_nGenNewMagAniCount mod 3);

        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          d := g_WMagic6Images.GetCachedGrayImage(idx, ax, ay)
        else
          d := g_WMagic6Images.GetCachedImage(idx, ax, ay);

        if d <> nil then begin
          GameCanvas.DrawBlend(
            dx + ax + m_nShiftX,
            dy + ay + m_nShiftY + 2,
            d);
        end;
      end;
      nError := 33;

      if m_boUseEffect and (m_UseEffectImage <> nil) then begin // 跟随人物动作的效果
        // GetEffectBase(m_CurMagic.EffectNumber - 1, 0, wimg, idx, m_CurMagic.NewLevel);
        if (g_MySelf <> nil) and g_MySelf.m_boDeath then
          d := m_UseEffectImage.GetCachedGrayImage(m_nEffectFrame, ax, ay)
        else
          d := m_UseEffectImage.GetCachedImage(m_nEffectFrame, ax, ay);
        if d <> nil then
          GameCanvas.DrawBlend(
            dx + ax + m_nShiftX,
            dy + ay + m_nShiftY,
            d);
      end;
    end;

    nError := 34;
    if CheckIsCustomMagic(m_CurMagic.MagicSerial) then begin
      DrawSelfMagicEffect(m_CurMagic.MagicSerial, False, False);
    end
      { TODO -opiaoyun -c注释 : *******************显示魔法效果 自身动作********************** 【2013-6-16】 }
    else if m_boUseMagic and (m_CurMagic.EffectNumber > 0) and (not (m_CurMagic.EffectNumber in [100..111])) then begin
      if m_nCurEffFrame in [0..m_nSpellFrame - 1] then begin
        // 魔法盾举手动作在使用连击的时候不要显示了 2021-01-22
        if (m_CurMagic.EffectNumber = 29) and (self = g_MySelf) and IsInContinuous {g_boContinuous} then begin //HZQ20230907
          Exit;
        end;

        // 4级技能强化 -- 4级灭天火 (人物起始动作) chongchong 2013-12-04
        if (m_CurMagic.MagicSerial = 45) and (m_CurMagic.MagicLevel = 4) and (m_CurMagic.NewLevel = 0) then begin
          idx := 80 + m_nCurEffFrame;
          wimg := g_WMagic6Images;

          d := nil;
          if wimg <> nil then begin
            if (g_MySelf <> nil) and g_MySelf.m_boDeath then
              d := wimg.GetCachedGrayImage(idx, ax, ay)
            else
              d := wimg.GetCachedImage(idx, ax, ay);
          end;

          if d <> nil then
            GameCanvas.DrawBlend(dx + ax + m_nShiftX, dy + ay + m_nShiftY, d);
        end
        else begin
          nX := 0;
          nY := 0;

          // 新魔法盾、道力盾、武力盾效果 chongchong 2015-07-25
          if g_ClientConfig.boSkill31UseNewEffect and (m_CurMagic.EffectNumber = 29) then begin
            // 4级技能、强化技能效果
            if (m_CurMagic.NewLevel > 0) or (m_CurMagic.MagicLevel = 4) then begin
              wimg := g_WMagicreImages;
              idx := 900;
            end
            else begin
              wimg := g_WMagicreImages;
              idx := 880;
            end;

            // 强制修正新魔法盾坐标 chongchong 2015-07-26
            nX := 2;
            nY := -6;
          end
            // 4级魔法盾，4级道力盾，4级武力盾用强化盾效果 chongchong 2013-12-27
          else if (m_CurMagic.EffectNumber = 29) and ((m_CurMagic.NewLevel > 0) or (m_CurMagic.MagicLevel = 4)) then begin
            GetEffectBase(m_CurMagic.EffectNumber - 1, 0, wimg, idx, 1);
          end
            // 4级新武力盾用强化盾效果 chongchong 2013-12-27
          else if (m_CurMagic.EffectNumber = 68) and ((m_CurMagic.NewLevel > 0) or (m_CurMagic.MagicLevel = 4)) then begin
            wimg := g_WMagic10Images;
            idx := 1608;
          end
            // 4级灵魂火符起手动作
          else if (m_CurMagic.EffectNumber = 10) and (m_CurMagic.MagicLevel >= 4) then begin
            wimg := g_WMagic6Images;
            idx := 120;
          end
          else
            GetEffectBase(m_CurMagic.EffectNumber - 1, 0, wimg, idx, m_CurMagic.NewLevel); // 获取事先配置好的偏移地址 -- EffectBase二位数组里面

          if idx >= 0 then begin
            case m_CurMagic.EffectNumber of
              61:if m_btJob = 2 then idx := idx - 20;
              62:idx := idx - m_btJob * 20;
              64:if m_btJob = 2 then idx := idx + 20; // 末日审判起手动作

              {
              107: idx := idx + m_btDir * (m_nSpellFrame + 7); // 双龙破
              104: idx := idx + m_btDir * (m_nSpellFrame); // 凤舞祭
              106: idx := idx + m_btDir * (m_nSpellFrame); // 冰天雪地

              108: idx := idx + m_btDir * (m_nSpellFrame); // 虎啸诀
              109: idx := idx + m_btDir * (m_nSpellFrame + 8); // 八卦掌
              110: idx := idx + m_btDir * (m_nSpellFrame + 8); // 三焰咒
              111: idx := idx + m_btDir * (m_nSpellFrame + 5); // 万剑归宗
              }
            end;

            // 新武力盾只有5帧，但由于改成5帧，举手动卡 chongchong 2015-11-03
            if (m_CurMagic.EffectNumber = 68) and (m_nCurEffFrame > 4) then
              idx := idx + 4
            else
              idx := idx + m_nCurEffFrame;

            d := nil;
            if (m_CurMagic.EffectNumber = 4) and (m_CurMagic.NewLevel > 0) and (not m_CurMagic.MagicItemType) then // 施毒术 检测 红毒使用红毒效果，绿毒使用绿毒效果
              idx := idx + 210;

            if (wimg <> nil) and (idx >= 0) then
              if (g_MySelf <> nil) and g_MySelf.m_boDeath then
                d := wimg.GetCachedGrayImage(idx, ax, ay)
              else
                d := wimg.GetCachedImage(idx, ax, ay);
            if d <> nil then
              GameCanvas.DrawBlend(
                dx + ax + m_nShiftX + nX,
                dy + ay + m_nShiftY + nY,
                d);

            {
            if (m_CurMagic.EffectNumber = 69) then
            begin
              idx := 3880 + m_nCurEffFrame;

              if (g_MySelf <> nil) and g_MySelf.m_boDeath then
                d := g_WMagicImages.GetCachedGrayImage(idx, ax, ay)
              else
                d := g_WMagicImages.GetCachedImage(idx, ax, ay);
              if d <> nil then
                GameCanvas.DrawBlend(
                  dx + ax + m_nShiftX + nX,
                  dy + ay + m_nShiftY + nY,
                  d);
            end;
            }
          end;
        end;
      end;
    end;
    nError := 35;

    // 显示魔法效果
    if m_boMagicEndEffect and (m_CurMagic.EffectNumber > 0) then begin
      if m_nCurEffFrame in [0..m_nSpellFrame - 1] then begin
        GetEffectBase(m_CurMagic.EffectNumber - 1, 0, wimg, idx, m_CurMagic.NewLevel);
        if idx >= 0 then begin
          idx := idx + (m_nCurrentMagicFrame - m_nStartMagicFrame + m_nStartPosMagicFrame);
          d := nil;
          if wimg <> nil then
            if (g_MySelf <> nil) and g_MySelf.m_boDeath then
              d := wimg.GetCachedGrayImage(idx, ax, ay)
            else
              d := wimg.GetCachedImage(idx, ax, ay);
          if d <> nil then
            GameCanvas.DrawBlend(
              dx + ax + m_nShiftX,
              dy + ay + m_nShiftY,
              d);
        end;
      end;
    end;

    // 战士正在攻击中，变脸会卡或绘制错误 2020-10-20 22:20:10
    if m_nChangeAppr >= 0 then begin
      m_boHitEffect := False;
      m_boHitEndEffect := False;
    end;

    nError := 36;
    // 显示攻击效果
    if m_boHitEffect and (m_nHitEffectNumber > 0) then begin
      GetEffectBase(m_nHitEffectNumber - 1, 1, wimg, idx, m_nHitEffectLevel);
      if idx >= 0 then begin
        if m_nHitEffectNumber = 7 then {// 雷霆剑法 chongchong 2015-04-07} begin
          idx := idx + m_btDir * 20 + (m_nCurrentFrame - m_nStartFrame);
        end
        else if m_nHitEffectNumber = 8 then begin // 龙影剑法
          idx := idx + m_btDir * 20 + (m_nCurrentFrame - m_nStartFrame);
        end
        else if m_nHitEffectNumber = 9 then begin // 破魂斩
          idx := idx + m_btDir * 20 + (m_nCurrentFrame - m_nStartFrame);
        end
        else if m_nHitEffectNumber = 10 then begin // 劈星斩
          idx := idx + m_btDir * 10 + (m_nCurrentFrame - m_nStartFrame);
        end
        else if m_nHitEffectNumber = 12 then begin // 开天斩重击
          idx := idx + m_btDir * 10 + (m_nCurrentFrame - m_nStartFrame);
        end
        else if m_nHitEffectNumber = 25 then begin // 开天斩轻击
          idx := idx + m_btDir * 10 + (m_nCurrentFrame - m_nStartFrame);
        end
        else if m_nHitEffectNumber = 20 then begin // 三绝杀
          idx := idx + m_btDir * 20 + (m_nCurrentFrame - m_nStartFrame);
          // end else
          // if m_nHitEffectNumber = 22 then begin //横扫千军
          // idx := idx + m_btDir * 20 + (m_nCurrentFrame - m_nStartFrame);
        end
        else if m_nHitEffectNumber = 14 then begin // 逐日剑法
          idx := idx + m_btDir * 10 + (m_nCurrentFrame - m_nStartFrame);
        end
          // 4级技能强化 -- 4级烈火 chongchong 2013-12-04
        else if (m_nHitEffectNumber = 4) and (m_nHitEffectLevel2 = 4) and (m_nHitEffectLevel = 0) then {// 烈火，4级，没有强化} begin
          idx := 0 + m_btDir * 10 + (m_nCurrentFrame - m_nStartFrame);
          wimg := g_WMagic6Images;
        end
          // 修复追心刺有技能物效有拖影 chongchong 2014-09-02
        else if (m_nHitEffectNumber = 23) then begin
          if m_nCurrentAction = 0 then
            idx := -1
          else
            idx := idx + m_btDir * 10 + (m_nCurrentFrame - m_nStartFrame);
        end

        else if m_nHitEffectNumber = 26 then begin // 断空斩
          idx := idx + m_btDir * 20 + (m_nCurrentFrame - m_nStartFrame);
        end
        else if m_nHitEffectNumber = 27 then begin // 血魄一击
          idx := idx + m_btDir * 10 + (m_nCurrentFrame - m_nStartFrame);
        end
        else begin
          idx := idx + m_btDir * 10 + (m_nCurrentFrame - m_nStartFrame);
        end;

        d := nil;
        if wimg <> nil then
          if (g_MySelf <> nil) and g_MySelf.m_boDeath then
            d := wimg.GetCachedGrayImage(idx, ax, ay)
          else
            d := wimg.GetCachedImage(idx, ax, ay);
        if d <> nil then
          GameCanvas.DrawBlend(
            dx + ax + m_nShiftX,
            dy + ay + m_nShiftY,
            d);

        if (m_nHitEffectNumber = 21) then begin // 断岳斩
          idx := 2000 + m_btDir * 10 + (m_nCurrentFrame - m_nStartFrame);
          d := g_cboEffect.GetCachedImage(idx, ax, ay);
          if d <> nil then
            GameCanvas.DrawBlend(
              dx + ax + m_nShiftX,
              dy + ay + m_nShiftY,
              d);
        end;
      end;
    end
      // 战士自身效果 chongchong 2015-03-24
    else if m_boHitEffect and (m_nHitEffectNumber <= -SM_CUSTOM_HIT001) and (m_nHitEffectNumber > -(SM_CUSTOM_HIT001 + CUSTOM_MAGIC_COUNT)) then begin
      // 战士自身效果绘制改成跟魔法效果绘制一样，主要是为了支持自定义时间间隔 chongchong 2016-03-16
      DrawSelfMagicEffect(Abs(m_nHitEffectNumber) - SM_CUSTOM_HIT001 + CUSTOM_MAGIC_START_ID, False, True);
      {
      CustomMagicConfig := GetCustomMagicConfig(Abs(m_nHitEffectNumber) - SM_CUSTOM_HIT1 + CUSTOM_MAGIC_START_ID);
      ClientConfig := nil;

      if CustomMagicConfig <> nil then
      begin
        case m_CurMagic.NewLevel of
          0:    MagicPlusLevel := mplNone;
          1..3: MagicPlusLevel := mpl1_3;
          4..6: MagicPlusLevel := mpl4_6;
          7..9: MagicPlusLevel := mpl7_9;
        else
          MagicPlusLevel := mpl7_9;
        end;

        ClientConfig := @CustomMagicConfig.MagicConfigs[MagicPlusLevel];
      end;

      if ClientConfig <> nil then
      begin
        if (ClientConfig.Self_File >= 0) and
          (ClientConfig.Self_File < g_EffectImageList.Count) and
          (ClientConfig.Self_DirCalcType <> mdctCenter) then
        begin
          wimg := TGameImages(g_EffectImageList.Objects[ClientConfig.Self_File]);

          if wimg <> nil then
          begin
            if ClientConfig.Self_DirCalcType = mdctNone then
            begin
              d := wimg.GetCachedImage(ClientConfig.Self_StartIndex + (m_nCurrentFrame - m_nStartFrame), ax, ay);
            end
            else if ClientConfig.Self_DirCalcType = mdctNormal then
            begin
              d := wimg.GetCachedImage(ClientConfig.Self_StartIndex +
                m_btDir * (ClientConfig.Self_PlayCount + ClientConfig.Self_EmptyCount) +
                (m_nCurrentFrame - m_nStartFrame), ax, ay);
            end;
            if d <> nil then
            begin
              if ClientConfig.Self_DrawMode = mdmBlend then
              begin
                GameCanvas.DrawBlend(
                  dx + ax + m_nShiftX,
                  dy + ay + m_nShiftY,
                  d);
              end
              else
              begin
                GameCanvas.Draw(
                  dx + ax + m_nShiftX,
                  dy + ay + m_nShiftY,
                  d);
              end;
            end;
          end;
        end;

        if m_nCurrentFrame - m_nStartFrame = HA.ActHit.frame then
        begin
          m_boHitEffect := False;
        end;
      end;
      }
      if m_nCurrentFrame - m_nStartFrame = HA.ActHit.frame then begin
        m_boHitEffect := False;
      end;
    end;

    nError := 37;
    if m_boHitEndEffect and (m_nHitEffectNumber_Old > 0) then begin
      GetEffectBase(m_nHitEffectNumber_Old - 1, 1, wimg, idx, m_nHitEffectLevel_Old);
      if idx >= 0 then begin
        // 攻击到目标后特效 chongchong 2014-09-02
        if m_nHitEffectNumber_Old = 4 then {// 烈火剑法 后面2帧} begin
          m_nStartPosHitFrame := 6;
          idx := idx + m_btHitDir * 10 + (m_nCurrentHitFrame - m_nStartHitFrame + m_nStartPosHitFrame);
        end
          // 雷霆剑法攻击到目标后目标不要特效 chongchong 2015-04-07
        else if m_nHitEffectNumber_Old = 7 then {// 雷霆剑法    ID=43} begin
          idx := -1 //idx := idx + m_btHitDir * 20 + (m_nCurrentHitFrame - m_nStartHitFrame + m_nStartPosHitFrame)
        end
        else if m_nHitEffectNumber_Old = 14 then {// 逐日剑法    ID=43} begin
          m_nStartPosHitFrame := 6;
          idx := idx + m_btHitDir * 10 + (m_nCurrentHitFrame - m_nStartHitFrame + m_nStartPosHitFrame);
        end
        else if m_nHitEffectNumber_Old = 8 then {// 龙影剑法} begin
          //m_nStartPosHitFrame := 8;
          idx := idx + m_btHitDir * 20 + (m_nCurrentHitFrame - m_nStartHitFrame + m_nStartPosHitFrame);
        end
        else if m_nHitEffectNumber_Old = 9 then // 破魂斩
          idx := idx + m_btHitDir * 20 + (m_nCurrentHitFrame - m_nStartHitFrame + m_nStartPosHitFrame)
        else if m_nHitEffectNumber_Old = 12 then {// 开天斩重击} begin
          m_nStartPosHitFrame := 85;
          idx := idx + m_btHitDir * 10 + (m_nCurrentHitFrame - m_nStartHitFrame + m_nStartPosHitFrame);
        end
        else if m_nHitEffectNumber_Old = 25 then {// 开天斩轻击} begin
          m_nStartPosHitFrame := 85;
          idx := idx + m_btHitDir * 10 + (m_nCurrentHitFrame - m_nStartHitFrame + m_nStartPosHitFrame);
        end
        else if m_nHitEffectNumber_Old = 26 then {// 开天斩重击} begin
          m_nStartPosHitFrame := 85;
          idx := 470 + m_btHitDir * 10 + (m_nCurrentHitFrame - m_nStartHitFrame + m_nStartPosHitFrame);
          wimg := g_WMagic5Images;
        end;

        d := nil;
        if wimg <> nil then begin
          if (g_MySelf <> nil) and g_MySelf.m_boDeath then
            d := wimg.GetCachedGrayImage(idx, ax, ay)
          else
            d := wimg.GetCachedImage(idx, ax, ay);
        end;

        // 修复开天斩随着角色飘 (按了开天的快捷后，迅速跑开），####### chongchong 2018-06-20 15:16:43
        {
        if d <> nil then
          GameCanvas.DrawBlend(
            dx + m_nHitEndX + ax,
            dy + m_nHitEndY + ay,
            d);

        }

        PlayScene.ScreenXYfromMCXY(m_nHitEndX, m_nHitEndY, nX, nY);
        PlayScene.ScreenXYfromMCXY(m_nRx, m_nRy, nX2, nY2);

        nX := nX + dx - nX2;
        nY := nY + dy - nY2;

        if d <> nil then
          GameCanvas.DrawBlend(
            ax + nX,
            ay + nY,
            d);
      end;
    end;

    nError := 38;
    // 显示武器破碎效果
    if m_boWeaponEffect then begin
      idx := WPEFFECTBASE + m_btDir * 10 + m_nCurWeaponEffect;
      if (g_MySelf <> nil) and g_MySelf.m_boDeath then
        d := g_WMagicImages.GetCachedGrayImage(idx, ax, ay)
      else
        d := g_WMagicImages.GetCachedImage(idx, ax, ay);
      if d <> nil then
        GameCanvas.DrawBlend(
          dx + ax + m_nShiftX,
          dy + ay + m_nShiftY,
          d);
    end;
    nError := 39;

    {$IF Enabled_PlugEngine = 1}
    if Assigned(HookTHumActor_DrawChr4) then begin
      try
        HookTHumActor_DrawChr4(Self, dx, dy, blend, boFlag);
      except
        on E:Exception do begin
          DebugOutStr('[Exception] HookTHumActor_DrawChr4');
          DebugOutStr(E.Message);
        end;
      end;
    end;
    {$IFEND}

  except
    on E:Exception do begin
      DebugOutStr('THumActor.DrawChr:' + inttostr(nError));
      DebugOutStr(E.Message);
    end;
  end;
end;

procedure THumActor.TakeHorse;
begin
  if m_btHorse = 0 then
    frmMain.SendClientMessage(CM_TAKEHORSE, 1, 0, 0, 0)
  else
    frmMain.SendClientMessage(CM_TAKEHORSE, 0, 0, 0, 0);
  LegendMap.Stop;
end;

{------------------------------------------------------------------------------}

function THeroActor.FindGroupMagic:pTClientMagic;
var
  I:Integer;
  pm:PTClientMagic;
begin
  Result := nil;
  g_HeroMagicList.Lock;
  try
    for I := 0 to g_HeroMagicList.Count - 1 do begin
      pm := PTClientMagic(g_HeroMagicList[I]);
      if pm.Def.wMagicId = GetGroupMagicId then begin
        Result := pm;
        Break;
      end;
    end;
  finally
    g_HeroMagicList.UnLock;
  end;

end;

function THeroActor.GetGroupMagicId:Integer;
begin
  Result := 0;
  case g_MySelf.m_btJob of
    0:begin
        case m_btJob of
          0:Result := 60;
          1:Result := 62;
          2:Result := 61;
        end;
      end;
    1:begin
        case m_btJob of
          0:Result := 62;
          1:Result := 65;
          2:Result := 64;
        end;
      end;
    2:begin
        case m_btJob of
          0:Result := 61;
          1:Result := 64;
          2:Result := 63;
        end;
      end;
  end;
end;

procedure THeroActor.GroupAttack;
begin
  //DScreen.AddChatBoardString('合击..........................',  clBlack, clRed);
  if (m_nMaxAngryValue > 0) and (m_nAngryValue >= m_nMaxAngryValue) then begin
    // 使用道法使用合击技能时，快速按F1使用灵魂火符，会卡技能 +  chongchong 2015-09-14
    g_dwLatestSpellTick := MyGetTickCount;

    frmMain.SendClientMessage(CM_HEROGROUPATTACK, 0, 0, 0, 0);
  end;
end;

procedure THeroActor.Rest;
begin
  frmMain.SendSay('@RestHero');
end;

procedure THeroActor.Protect;
begin
  frmMain.SendClientMessage(CM_HEROPROTECT, 0, g_nMouseCurrX, g_nMouseCurrY, 0);
end;

procedure THeroActor.Target;
var
  TargetCret:TActor;
  FocusCret:TActor;
begin
  {$IF IsMultiThreadRender = 1}
  EnterCriticalSection(g_ActorLock);
  {$IFEND}
  TargetCret := g_TargetCret;
  FocusCret := g_FocusCret;
  {$IF IsMultiThreadRender = 1}
  LeaveCriticalSection(g_ActorLock);
  {$IFEND}

  if (TargetCret = nil) or ((TargetCret <> nil) and PlayScene.IsValidActor(TargetCret) and TargetCret.m_boDeath) then begin
    if (FocusCret <> nil) and PlayScene.IsValidActor(FocusCret) and (not FocusCret.m_boDeath) then begin
      frmMain.SendClientMessage(CM_HEROTARGET, FocusCret.m_nRecogId, FocusCret.m_nCurrX, FocusCret.m_nCurrY, 0);
    end
    else begin
      frmMain.SendClientMessage(CM_HEROTARGET, 0, g_nMouseCurrX, g_nMouseCurrY, 0);
    end;
  end
  else begin
    if (FocusCret <> nil) and PlayScene.IsValidActor(FocusCret) then
      frmMain.SendClientMessage(CM_HEROTARGET, FocusCret.m_nRecogId, FocusCret.m_nCurrX, FocusCret.m_nCurrY, 0);
  end;
end;

{ TStatuaryNpcActor }

procedure TStatuaryNpcActor.CalcActorFrame;
begin
  m_boUseMagic := False;
  m_nCurrentFrame := -1;
  m_nBodyOffset := 1200;

  m_btDir := 0;

  m_nStartFrame := 0;
  m_nEndFrame := 0;
  m_dwFrameTime := 100;
  m_dwStartTime := TimeGetTime;
  m_StartCounter := timeGetTime;
  m_nDefFrameCount := 1;
end;

function TStatuaryNpcActor.CheckLoadSurface:Boolean;
begin
  Result := inherited CheckLoadSurface;
end;

constructor TStatuaryNpcActor.Create;
begin
  inherited;
  m_boShowStatuary := False;
  m_nEffigyState.Value1 := 0;
  m_nEffigyState.Value2 := 0;

  m_nEffigyOffset := 0;
  m_nOldEffigyOffset := 0;

  m_wDress := 0;
  m_wWeapon := 0;
  m_wWeaponSound := 0;
  m_wShield := 0;

  m_btHair := 0;

  m_boScaleShow := True;
  m_IsGrayShow := False;

  FIsFinalized := False;
end;

destructor TStatuaryNpcActor.Destroy;
begin
  if m_HumTexture <> nil then
    m_HumTexture.Free;

  if m_HumEffTexture <> nil then
    m_HumEffTexture.Free;

  if m_WeaponEffTexture <> nil then
    m_WeaponEffTexture.Free;

  inherited;
end;

procedure TStatuaryNpcActor.DrawChr(dx, dy:Integer; blend,
  boFlag:Boolean);
var
  SR, DR:TRect;
begin
  inherited;

  // 修复全屏模式切换到桌面，再切回游戏时天下第一有黑块 chongchong 2016-04-07
  if FIsFinalized then begin
    FIsFinalized := False;
    SetEffigyState(m_IsGrayShow, m_boScaleShow, m_nEffigyState, m_nEffigyOffset);
  end;

  if m_boShowStatuary then begin
    SR := Rect(0, 0, m_HumTexture.Width, m_HumTexture.Height);
    DR := SR;
    OffsetRect(DR, dx + m_nShiftX - 200, dy + m_nShiftY - 237);

    if m_boScaleShow then begin
      InflateRect(DR, 20, 20);

      if m_WeaponEffTexture <> nil then begin
        GameCanvas.StretchDraw(DR, SR, m_WeaponEffTexture {, Blend_SrcAlphaColor});
      end;

      if m_HumEffTexture <> nil then begin
        GameCanvas.StretchDraw(DR, SR, m_HumEffTexture {, Blend_SrcAlphaColor});
      end;

      if (m_HumTexture <> nil) then begin
        GameCanvas.StretchDraw(DR, SR, m_HumTexture);
      end;
    end
    else begin
      if m_WeaponEffTexture <> nil then begin
        GameCanvas.DrawBlend(DR.Left, DR.Top, SR, m_WeaponEffTexture {, Blend_SrcAlphaColor});
      end;

      if m_HumEffTexture <> nil then begin
        GameCanvas.DrawBlend(DR.Left, DR.Top, SR, m_HumEffTexture {, Blend_SrcAlphaColor});
      end;

      if (m_HumTexture <> nil) then begin
        GameCanvas.Draw(DR.Left, DR.Top, SR, m_HumTexture);
      end;
    end;
  end;
end;

procedure TStatuaryNpcActor.Finalize;
begin
  inherited;
  FIsFinalized := True;
end;

function TStatuaryNpcActor.GetDefaultFrame(wmode:Boolean):Integer;
begin
  Result := 0;
end;

procedure TStatuaryNpcActor.LoadSurface(Sender:TObject);
begin
  m_dwLoadSurfaceTime := MyGetTickCount;
  m_boLoadSurface := False;
  m_EffSurface := nil;
  if m_IsGrayShow then
    m_BodySurface := g_WNewopUIImages.GetCachedGrayImage(m_nBodyOffset + m_nCurrentFrame, m_nPx, m_nPy)
  else
    m_BodySurface := g_WNewopUIImages.GetCachedImage(m_nBodyOffset + m_nCurrentFrame, m_nPx, m_nPy)
      //m_BodySurface := g_WNpcImgImages.Indexs[0].GetCachedImage(2160 + m_nCurrentFrame, m_nPx, m_nPy);
end;

procedure TStatuaryNpcActor.SetEffigyState(IsGrayShow:Boolean; IsScaleShow:Boolean; nEffigyState:TFeature_New; nOffset:Integer);

function HiLong(N:Int64):Cardinal;
  begin
    Result := Large_Integer(N).HighPart;
  end;

  function LoLong(N:Int64):Cardinal;
  begin
    Result := Large_Integer(N).LowPart;
  end;

var
  LInt, HInt:Integer;
  W:Word;

  nHairOffset:Integer;

  D:TTexture;
  OffsetPt:TPoint;

  GameImages:TGameImages;
begin
  m_nEffigyState := nEffigyState;

  m_nOldEffigyOffset := m_nEffigyOffset;
  m_nEffigyOffset := nOffset;

  if m_nCurrentFrame < 0 then
    m_nCurrentFrame := 0;

  m_boScaleShow := IsScaleShow;
  m_IsGrayShow := IsGrayShow;

  if m_nEffigyState.Value1 = 0 then begin
    m_boShowStatuary := False;
    if m_HumTexture <> nil then
      FreeAndNil(m_HumTexture);

    if m_HumEffTexture <> nil then
      FreeAndNil(m_HumEffTexture);

    if m_WeaponEffTexture <> nil then
      FreeAndNil(m_WeaponEffTexture);
  end
  else begin
    m_boShowStatuary := True;

    LInt := LoLong(nEffigyState.Value1);
    HInt := HiLong(nEffigyState.Value1);

    m_wDress := LoWord(LInt);
    m_wWeapon := HiWord(LInt);

    m_btSex := m_wDress mod 2;

    m_wEffect := LoWord(HInt);
    W := HiWord(HInt);
    m_btHair := LoByte(W);
    m_wShield := HiByte(W);

    LInt := LoLong(nEffigyState.Value2);
    HInt := HiLong(nEffigyState.Value2);

    m_nDressEffectIndex := LoWord(LInt);
    m_nWeaponEffectIndex := HiWord(LInt);

    m_wDressEffectOffSet := LoWord(HInt);
    m_wWeaponEffectOffSet := HiWord(HInt);

    m_dwLoadSurfaceTime := MyGetTickCount;
    m_boLoadSurface := False;
    m_BodySurface := nil;
    m_EffSurface := nil;

    if IsGrayShow then
      m_BodySurface := g_WNewopUIImages.GetCachedGrayImage(m_nBodyOffset + m_nCurrentFrame, m_nPx, m_nPy)
    else
      m_BodySurface := g_WNewopUIImages.GetCachedImage(m_nBodyOffset + m_nCurrentFrame, m_nPx, m_nPy);

    if m_HumTexture <> nil then begin
      FreeAndNil(m_HumTexture);
    end;

    if m_HumEffTexture <> nil then begin
      FreeAndNil(m_HumEffTexture);
    end;

    if m_WeaponEffTexture <> nil then begin
      FreeAndNil(m_WeaponEffTexture);
    end;

    m_HumTexture := GameCanvas.HGE.Texture_Create(800, 800);
    m_HumEffTexture := GameCanvas.HGE.Texture_Create(800, 800);
    m_WeaponEffTexture := GameCanvas.HGE.Texture_Create(800, 800);

    nHairOffset := -1; //HZQ 20230525

    case m_btHair of
      0..5:begin
          if (m_btHair < 4) then begin
            case m_btSex of
              0:nHairOffset := m_btHair * 2 * HUMANFRAME;
              1:begin
                  if m_btHair > 0 then
                    nHairOffset := (m_btHair + 2) * HUMANFRAME
                  else
                    nHairOffset := 0
                end;
            end;
          end else begin
            case m_btHair of // 头盔
              4:nHairOffset := 3600;
              5:nHairOffset := 4800;
              else
                nHairOffset := -1;
            end;
          end;
        end;
      { 修正发型扩展外观错误 chongchong 2013-11-21}
      50..59:begin
          nHairOffset := (m_btHair - 50) * HUMANFRAME {* 2 + m_btSex * HUMANFRAME};
        end;
      60..69:begin
          nHairOffset := (m_btHair - 60) * HUMANFRAME {* 2 + m_btSex * HUMANFRAME};
        end;

      // 斗笠 1 - 8
      100..107:begin
          nHairOffset := 3600 + (m_btHair - 100) * HUMANFRAME * 2 + m_btSex * HUMANFRAME;
        end;

      // 斗笠 9, 10 hair3
      108..119:begin
          nHairOffset := 0 + (m_btHair - 108) * HUMANFRAME * 2 + m_btSex * HUMANFRAME;
        end;

      // 扩展斗笠 hair4
      120..129:begin
          nHairOffset := 0 + (m_btHair - 120) * HUMANFRAME * 2 + m_btSex * HUMANFRAME;
        end;

      // 扩展斗笠 hair5
      130..139:begin
          nHairOffset := 0 + (m_btHair - 130) * HUMANFRAME * 2 + m_btSex * HUMANFRAME;
        end;

      // 扩展斗笠 hair6
      140..149:begin
          nHairOffset := 0 + (m_btHair - 140) * HUMANFRAME * 2 + m_btSex * HUMANFRAME;
        end;
    end;

    // 武器
    if IsGrayShow then
      D := g_WWeaponImages.GetWWeaponGrayImg(m_wWeapon, m_btSex, m_nCurrentFrame + m_nEffigyOffset, OffsetPt.X, OffsetPt.Y)
    else
      D := g_WWeaponImages.GetWWeaponImg(m_wWeapon, m_btSex, m_nCurrentFrame + m_nEffigyOffset, OffsetPt.X, OffsetPt.Y);

    if D <> nil then begin
      CopyTexture(D, m_HumTexture, 200 + OffsetPt.X, 210 + OffsetPt.Y);
    end;

    // 人物身体
    if IsGrayShow then
      D := g_WHumImgImages.GetWHumGrayImg(m_wDress, m_btSex, m_nCurrentFrame + m_nEffigyOffset, OffsetPt.X, OffsetPt.Y)
    else
      D := g_WHumImgImages.GetWHumImg(m_wDress, m_btSex, m_nCurrentFrame + m_nEffigyOffset, OffsetPt.X, OffsetPt.Y);
    if (D = nil) or (D.Width * D.Height <= 16) then begin
      if IsGrayShow then
        D := g_WHumImgImages.GetWHumGrayImg(0, m_btSex, m_nCurrentFrame + m_nEffigyOffset, OffsetPt.X, OffsetPt.Y)
      else
        D := g_WHumImgImages.GetWHumImg(0, m_btSex, m_nCurrentFrame + m_nEffigyOffset, OffsetPt.X, OffsetPt.Y)
    end;
    if D <> nil then begin
      CopyTexture(D, m_HumTexture, 200 + OffsetPt.X, 210 + OffsetPt.Y);
    end;

    { TODO -opiaoyun -c扩展 : 发型读取规则扩展【2013-6-10】 }
    if nHairOffset >= 0 then begin
      // 修改为CASE语句 -- piaoyn
      case m_btHair of
        0..5:begin
            if IsGrayShow then
              D := g_WHairImgImages.GetCachedGrayImage(nHairOffset + m_nCurrentFrame + m_nEffigyOffset, OffsetPt.X, OffsetPt.Y)
            else
              D := g_WHairImgImages.GetCachedImage(nHairOffset + m_nCurrentFrame + m_nEffigyOffset, OffsetPt.X, OffsetPt.Y);
          end;
        50..59: {// 读取Hair10} begin
            if IsGrayShow then
              D := g_WHair10ImgImages.GetCachedGrayImage(nHairOffset + m_nCurrentFrame + m_nEffigyOffset, OffsetPt.X, OffsetPt.Y)
            else
              D := g_WHair10ImgImages.GetCachedImage(nHairOffset + m_nCurrentFrame + m_nEffigyOffset, OffsetPt.X, OffsetPt.Y);
          end;
        60..69: {// 读取Hair11} begin
            if IsGrayShow then
              D := g_WHair11ImgImages.GetCachedGrayImage(nHairOffset + m_nCurrentFrame + m_nEffigyOffset, OffsetPt.X, OffsetPt.Y)
            else
              D := g_WHair11ImgImages.GetCachedImage(nHairOffset + m_nCurrentFrame + m_nEffigyOffset, OffsetPt.X, OffsetPt.Y);
          end;
        100..107:begin
            if IsGrayShow then
              D := g_WHair2ImgImages.GetCachedGrayImage(nHairOffset + m_nCurrentFrame + m_nEffigyOffset, OffsetPt.X, OffsetPt.Y)
            else
              D := g_WHair2ImgImages.GetCachedImage(nHairOffset + m_nCurrentFrame + m_nEffigyOffset, OffsetPt.X, OffsetPt.Y);
          end;
        108..119:begin
            if IsGrayShow then
              D := g_WHair3ImgImages.GetCachedGrayImage(nHairOffset + m_nCurrentFrame + m_nEffigyOffset, OffsetPt.X, OffsetPt.Y)
            else
              D := g_WHair3ImgImages.GetCachedImage(nHairOffset + m_nCurrentFrame + m_nEffigyOffset, OffsetPt.X, OffsetPt.Y);
          end;
        120..129:begin
            if IsGrayShow then
              D := g_WHair4ImgImages.GetCachedGrayImage(nHairOffset + m_nCurrentFrame + m_nEffigyOffset, OffsetPt.X, OffsetPt.Y)
            else
              D := g_WHair4ImgImages.GetCachedImage(nHairOffset + m_nCurrentFrame + m_nEffigyOffset, OffsetPt.X, OffsetPt.Y);
          end;
        130..139:begin
            if IsGrayShow then
              D := g_WHair5ImgImages.GetCachedGrayImage(nHairOffset + m_nCurrentFrame + m_nEffigyOffset, OffsetPt.X, OffsetPt.Y)
            else
              D := g_WHair5ImgImages.GetCachedImage(nHairOffset + m_nCurrentFrame + m_nEffigyOffset, OffsetPt.X, OffsetPt.Y);
          end;
        140..149:begin
            if IsGrayShow then
              D := g_WHair6ImgImages.GetCachedGrayImage(nHairOffset + m_nCurrentFrame + m_nEffigyOffset, OffsetPt.X, OffsetPt.Y)
            else
              D := g_WHair6ImgImages.GetCachedImage(nHairOffset + m_nCurrentFrame + m_nEffigyOffset, OffsetPt.X, OffsetPt.Y);
          end;
      end;
    end;

    if D <> nil then begin
      CopyTexture(D, m_HumTexture, 200 + OffsetPt.X, 210 + OffsetPt.Y);
    end;

    if m_wShield > 0 then begin
      if IsGrayShow then
        D := g_WShieldImg.GetCachedGrayImage(HUMANFRAME * (m_wShield - 1) + m_nCurrentFrame + m_nEffigyOffset, OffsetPt.X, OffsetPt.Y)
      else
        D := g_WShieldImg.GetCachedImage(HUMANFRAME * (m_wShield - 1) + m_nCurrentFrame + m_nEffigyOffset, OffsetPt.X, OffsetPt.Y);
    end
    else
      D := nil;
    if D <> nil then begin
      CopyTexture(D, m_HumTexture, 200 + OffsetPt.X, 210 + OffsetPt.Y);
    end;
  end;

  ///////////////////////翅膀效果扩展 piaoyun 2013-07-29//////////////////////
  D := nil;
  if (m_wEffect >= 1000) then begin
    if IsGrayShow then
      D := g_WHumEffectImages.GetWHumEffectGrayImg(m_wEffect, m_btSex, m_nCurrentFrame + m_nEffigyOffset, False, OffsetPt.X, OffsetPt.Y, False)
    else
      D := g_WHumEffectImages.GetWHumEffectImg(m_wEffect, m_btSex, m_nCurrentFrame + m_nEffigyOffset, False, OffsetPt.X, OffsetPt.Y, False)
  end
    ////////////////////////////////////////////////////////////////////////////
  else if (m_wEffect = 50) then begin
    {
    if (m_nCurrentFrame + m_nEffigyOffset <= 536) then
    begin
      if IsGrayShow then
        D := g_WEffectImg.GetCachedGrayImage(352 + m_nCurrentFrame + m_nEffigyOffset, OffsetPt.X, OffsetPt.Y)
      else
        D := g_WEffectImg.GetCachedImage(352 + m_nCurrentFrame + m_nEffigyOffset, OffsetPt.X, OffsetPt.Y);
    end;
    }
  end
  else if (m_wEffect <> 0) then begin
    if IsGrayShow then
      D := g_WHumEffectImages.GetCachedGrayImage((m_wEffect - 1) * HUMANFRAME + m_nCurrentFrame + m_nEffigyOffset, OffsetPt.X, OffsetPt.Y)
    else
      D := g_WHumEffectImages.GetCachedImage((m_wEffect - 1) * HUMANFRAME + m_nCurrentFrame + m_nEffigyOffset, OffsetPt.X, OffsetPt.Y);
  end;

  if D <> nil then begin
    CopyTexture(D, m_HumEffTexture, 200 + OffsetPt.X, 210 + OffsetPt.Y, m_boScaleShow);
  end
  else begin
    g_EffectImageList.Lock;
    try
      if (m_nDressEffectIndex >= 0) and (m_nDressEffectIndex < g_EffectImageList.Count) then begin
        GameImages := TGameImages(g_EffectImageList.Objects[m_nDressEffectIndex]);
        if GameImages <> nil then begin
          if IsGrayShow then begin
            D := GameImages.GetCachedGrayImage(m_wDressEffectOffSet + m_nCurrentFrame + m_nEffigyOffset, OffsetPt.X, OffsetPt.Y)
          end
          else begin
            D := GameImages.GetCachedImage(m_wDressEffectOffSet + m_nCurrentFrame + m_nEffigyOffset, OffsetPt.X, OffsetPt.Y)
          end;
        end;

        if D <> nil then begin
          CopyTexture(D, m_HumEffTexture, 200 + OffsetPt.X, 210 + OffsetPt.Y, m_boScaleShow);
        end;
      end;
    finally
      g_EffectImageList.UnLock;
    end;
  end;

  D := nil;
  g_EffectImageList.Lock;
  try
    if (m_nWeaponEffectIndex >= 0) and (m_nWeaponEffectIndex < g_EffectImageList.Count) then begin
      GameImages := TGameImages(g_EffectImageList.Objects[m_nWeaponEffectIndex]);
      if GameImages <> nil then begin
        if IsGrayShow then
          D := GameImages.GetCachedGrayImage(m_wWeaponEffectOffSet + m_nCurrentFrame + m_nEffigyOffset, OffsetPt.X, OffsetPt.Y)
        else
          D := GameImages.GetCachedImage(m_wWeaponEffectOffSet + m_nCurrentFrame + m_nEffigyOffset, OffsetPt.X, OffsetPt.Y);
      end;
    end
    else begin
      /////////////////////武器特效扩展 piaoyun 2013-07-28////////////////
      case m_wDBWeaponEffectOffSet of
        1000..1015: {// 绘制DB库扩展特效 piaoyun 2013-07-28} begin
            if IsGrayShow then
              D := g_WHumEffectImages.GetWHumEffectGrayImg(m_wDBWeaponEffectOffSet, m_btSex, m_nCurrentFrame + m_nEffigyOffset, True, OffsetPt.X, OffsetPt.Y, False)
            else
              D := g_WHumEffectImages.GetWHumEffectImg(m_wDBWeaponEffectOffSet, m_btSex, m_nCurrentFrame + m_nEffigyOffset, True, OffsetPt.X, OffsetPt.Y, False);
          end;
        1025..2000: {// 绘制扩展特效 WeaponEffect.wzl -- WeaponEffect5.wzl} begin
            if IsGrayShow then
              D := g_WeaponEffectList.GetWWeaponEffectGrayImg(m_wDBWeaponEffectOffSet, m_btSex, m_nCurrentFrame + m_nEffigyOffset, OffsetPt.X, OffsetPt.Y)
            else
              D := g_WeaponEffectList.GetWWeaponEffectImg(m_wDBWeaponEffectOffSet, m_btSex, m_nCurrentFrame + m_nEffigyOffset, OffsetPt.X, OffsetPt.Y);
          end;
      end;
      ///////////////////武器特效扩展结束 piaoyun 2013-07-28//////////////
    end;
  finally
    g_EffectImageList.UnLock;
  end;

  if D <> nil then begin
    CopyTexture(D, m_WeaponEffTexture, 200 + OffsetPt.X, 210 + OffsetPt.Y, m_boScaleShow);
  end;
end;

end.
