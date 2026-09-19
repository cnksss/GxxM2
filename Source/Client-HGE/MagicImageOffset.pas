unit MagicImageOffset;

interface
uses
  GameImages;
const
  MAXEFFECT = 255;
  MAXHITEFFECT = 27;
type
  TImageOffset = record
    GameImages:TGameImages;
    ImageOffset:Integer;
  end;
  pTImageOffset = ^TImageOffset;

  TEffectImageOffsets = array[0..9] of array[0..MAXEFFECT - 1] of TImageOffset;
  pTEffectImageOffsets = ^TEffectImageOffsets;

  THitEffectImageOffsets = array[0..9] of array[0..MAXHITEFFECT - 1] of TImageOffset;
  pTHitEffectImageOffsets = ^THitEffectImageOffsets;
  
var
  EffectBase:TEffectImageOffsets;
  HitEffectBase:THitEffectImageOffsets;

procedure MagicImageOffsetInit;
procedure MagicImageOffsetInitLevel;

implementation

uses MShare;

// 魔法图标九重初始化[针对新端技能] -- piaoyun 2013-6-16
// 强化技能魔法效果 chongchong 2014-05-19

procedure MagicImageOffsetInitLevel;
var
  I:Integer;
begin
  for I := 1 to 9 do begin // 魔法盾
    EffectBase[I, 28].ImageOffset := 690; {29}
    EffectBase[I, 28].GameImages := g_WMagic6Images;
  end;

  for I := 1 to 9 do begin // 新魔法盾
    EffectBase[I, 66].ImageOffset := 900; {29}
    EffectBase[I, 66].GameImages := g_WMagicreImages;
  end;

  for I := 1 to 9 do begin // 雷电术
    EffectBase[I, 8].ImageOffset := 120 + ((I - 1) * 10); {29}
    EffectBase[I, 8].GameImages := g_WMagic7Images16;
  end;

  for I := 1 to 9 do begin // 火墙
    EffectBase[I, 19].ImageOffset := ((I - 1) * 10); {20}
    EffectBase[I, 19].GameImages := g_WMagic7Images16;
  end;

  {
  for I := 1 to 9 do begin // 召唤骷髅
    EffectBase[I, 14].ImageOffset := 860 + ((I - 1) * 20); // 15
    EffectBase[I, 14].GameImages := g_WMagic7Images16;
  end;
  }

  for I := 1 to 9 do begin // 冰咆哮
    EffectBase[I, 30].ImageOffset := ((I - 1) * 10); {31}
    EffectBase[I, 30].GameImages := g_WMagic8Images16;
  end;

  for I := 1 to 9 do begin // 灵魂火符
    EffectBase[I, 9].ImageOffset := 500 + ((I - 1) * 10); {10} //
    EffectBase[I, 9].GameImages := g_WMagic8Images16;
  end;

  for I := 1 to 9 do begin // 爆裂火焰
    EffectBase[I, 20].ImageOffset := 260 + ((I - 1) * 10); {21}
    EffectBase[I, 20].GameImages := g_WMagic7Images16;
  end;

  for I := 1 to 9 do begin // 流星火雨
    EffectBase[I, 50].ImageOffset := 430 + ((I - 1) * 10); {51}
    EffectBase[I, 50].GameImages := g_WMagic9Images;
  end;

  for I := 1 to 3 do begin // 噬血术
    EffectBase[I, 47].ImageOffset := 630 + ((I - 1) * 20); {48}
    EffectBase[I, 47].GameImages := g_WMagic9Images;
  end;

  for I := 4 to 6 do begin // 噬血术
    EffectBase[I, 47].ImageOffset := 780 + ((I - 4) * 20); {48}
    EffectBase[I, 47].GameImages := g_WMagic9Images;
  end;

  for I := 7 to 9 do begin // 噬血术
    EffectBase[I, 47].ImageOffset := 930 + ((I - 7) * 20); {48}
    EffectBase[I, 47].GameImages := g_WMagic9Images;
  end;

  for I := 1 to 3 do begin // 召唤神兽
    EffectBase[I, 27].ImageOffset := 160;
    EffectBase[I, 27].GameImages := g_WMagic8Images16;
  end;

  for I := 4 to 6 do begin // 召唤神兽
    EffectBase[I, 27].ImageOffset := 180;
    EffectBase[I, 27].GameImages := g_WMagic8Images16;
  end;

  for I := 7 to 9 do begin // 召唤神兽
    EffectBase[I, 27].ImageOffset := 200;
    EffectBase[I, 27].GameImages := g_WMagic8Images16;
  end;

  for I := 1 to 3 do begin // 召唤圣兽
    EffectBase[I, 46].ImageOffset := 160; {47} // 召唤圣兽
    EffectBase[I, 46].GameImages := g_WMagic8Images16;
  end;
  for I := 4 to 6 do begin // 召唤圣兽
    EffectBase[I, 46].ImageOffset := 180; {47} // 召唤圣兽
    EffectBase[I, 46].GameImages := g_WMagic8Images16;
  end;
  for I := 7 to 9 do begin // 召唤圣兽
    EffectBase[I, 46].ImageOffset := 200; {47} // 召唤圣兽
    EffectBase[I, 46].GameImages := g_WMagic8Images16;
  end;

  for I := 1 to 9 do begin // 召唤骷髅
    EffectBase[I, 14].ImageOffset := 1020; {47} // 召唤骷髅
    EffectBase[I, 14].GameImages := g_WMagic7Images16;
  end;

  for I := 1 to 9 do begin // 灭天火
    EffectBase[I, 33].ImageOffset := 280 + ((I - 1) * 10); {34}
    EffectBase[I, 33].GameImages := g_WMagic9Images;
  end;

  for I := 1 to 9 do begin // 施毒术
    EffectBase[I, 3].ImageOffset := 440 + ((I - 1) * 20); {4}
    EffectBase[I, 3].GameImages := g_WMagic7Images16;
  end;

  HitEffectBase[1, 0].ImageOffset := 1600; // 攻杀剑术
  HitEffectBase[1, 0].GameImages := g_WMagic7Images16;
  HitEffectBase[4, 0].ImageOffset := 1690; // 攻杀剑术
  HitEffectBase[4, 0].GameImages := g_WMagic7Images16;
  HitEffectBase[7, 0].ImageOffset := 1780; // 攻杀剑术
  HitEffectBase[7, 0].GameImages := g_WMagic7Images16;

  HitEffectBase[1, 1].ImageOffset := 2140; // 刺杀剑术
  HitEffectBase[1, 1].GameImages := g_WMagic7Images16;
  HitEffectBase[4, 1].ImageOffset := 2230; // 刺杀剑术
  HitEffectBase[4, 1].GameImages := g_WMagic7Images16;
  HitEffectBase[7, 1].ImageOffset := 2320; // 刺杀剑术
  HitEffectBase[7, 1].GameImages := g_WMagic7Images16;

  HitEffectBase[1, 3].ImageOffset := 1660; // 烈火
  HitEffectBase[1, 3].GameImages := g_WMagic8Images16;
  HitEffectBase[4, 3].ImageOffset := 1750; // 烈火
  HitEffectBase[4, 3].GameImages := g_WMagic8Images16;
  HitEffectBase[7, 3].ImageOffset := 1840; // 烈火
  HitEffectBase[7, 3].GameImages := g_WMagic8Images16;

  HitEffectBase[1, 2].ImageOffset := 1870; // 圆月弯刀
  HitEffectBase[1, 2].GameImages := g_WMagic7Images16;
  HitEffectBase[4, 2].ImageOffset := 1960; // 圆月弯刀
  HitEffectBase[4, 2].GameImages := g_WMagic7Images16;
  HitEffectBase[7, 2].ImageOffset := 2050; // 圆月弯刀
  HitEffectBase[7, 2].GameImages := g_WMagic7Images16;

  HitEffectBase[1, 13].ImageOffset := 0; // 逐日剑法
  HitEffectBase[1, 13].GameImages := g_WMagic9Images;
  HitEffectBase[4, 13].ImageOffset := 90; // 逐日剑法
  HitEffectBase[4, 13].GameImages := g_WMagic9Images;
  HitEffectBase[7, 13].ImageOffset := 180; // 逐日剑法
  HitEffectBase[7, 13].GameImages := g_WMagic9Images;

  for I := 2 to 3 do begin
    HitEffectBase[I, 0] := HitEffectBase[1, 0];
    HitEffectBase[I, 1] := HitEffectBase[1, 1];
    HitEffectBase[I, 3] := HitEffectBase[1, 3];
    HitEffectBase[I, 2] := HitEffectBase[1, 2];
    HitEffectBase[I, 13] := HitEffectBase[1, 13];
  end;

  for I := 5 to 6 do begin
    HitEffectBase[I, 0] := HitEffectBase[4, 0];
    HitEffectBase[I, 1] := HitEffectBase[4, 1];
    HitEffectBase[I, 3] := HitEffectBase[4, 3];
    HitEffectBase[I, 2] := HitEffectBase[4, 2];
    HitEffectBase[I, 13] := HitEffectBase[4, 13];
  end;

  for I := 8 to 9 do begin
    HitEffectBase[I, 0] := HitEffectBase[7, 0];
    HitEffectBase[I, 1] := HitEffectBase[7, 1];
    HitEffectBase[I, 3] := HitEffectBase[7, 3];
    HitEffectBase[I, 2] := HitEffectBase[7, 2];
    HitEffectBase[I, 13] := HitEffectBase[7, 13];
  end;
end;

// 魔法图片偏移初始化[预备动作] 数据库 [Effet字段值-1] --- piaoyun 2013-6-16

procedure MagicImageOffsetInit;
var
  I, II:Integer;
begin
  FillChar(EffectBase, SizeOf(EffectBase), 0);
  FillChar(HitEffectBase, SizeOf(HitEffectBase), 0);
  for I := 0 to 3 do begin
    for II := 0 to MAXEFFECT - 1 do begin
      EffectBase[I, II].GameImages := g_WMagicImages;
    end;
  end;
  for I := 0 to 3 do begin
    for II := 0 to MAXHITEFFECT - 1 do begin
      HitEffectBase[I, II].GameImages := g_WMagicImages;
    end;
  end;

  EffectBase[0, 0].ImageOffset := 0; {1}
  EffectBase[0, 1].ImageOffset := 200; {2}
  EffectBase[0, 2].ImageOffset := 400; {3}
  EffectBase[0, 3].ImageOffset := 600; {4} // 施毒术
  EffectBase[0, 4].ImageOffset := 0; {5}
  EffectBase[0, 5].ImageOffset := 900; {6}
  EffectBase[0, 6].ImageOffset := 920; {7}
  EffectBase[0, 7].ImageOffset := 940; {8}
  EffectBase[0, 8].ImageOffset := 20; {9}
  EffectBase[0, 9].ImageOffset := 940; {10}
  EffectBase[0, 10].ImageOffset := 940; {11}
  EffectBase[0, 11].ImageOffset := 940; {12}
  EffectBase[0, 12].ImageOffset := 0; {13}
  EffectBase[0, 13].ImageOffset := 1380; {14}
  EffectBase[0, 14].ImageOffset := 1500; {15}
  EffectBase[0, 15].ImageOffset := 1520; {16}
  EffectBase[0, 16].ImageOffset := 940; {17}
  EffectBase[0, 17].ImageOffset := 1560; {18 诱惑之光}
  EffectBase[0, 18].ImageOffset := 1590; {19}
  EffectBase[0, 19].ImageOffset := 1620; {20} // 火墙
  EffectBase[0, 20].ImageOffset := 1650; {21} // 爆裂火焰
  EffectBase[0, 21].ImageOffset := 1680; {22}
  EffectBase[0, 22].ImageOffset := 0; {23}
  EffectBase[0, 23].ImageOffset := 0; {24}
  EffectBase[0, 24].ImageOffset := 0; {25}
  EffectBase[0, 25].ImageOffset := 3960; {26}
  EffectBase[0, 26].ImageOffset := 1790; {27}
  EffectBase[0, 27].ImageOffset := 0; {28}
  EffectBase[0, 28].ImageOffset := 3880; // 魔法盾
  EffectBase[0, 29].ImageOffset := 3920; {30}
  EffectBase[0, 30].ImageOffset := 3840; {31}
  EffectBase[0, 31].ImageOffset := 600; // 解毒术
  EffectBase[0, 32].ImageOffset := 600; {33}
  EffectBase[0, 33].ImageOffset := 130; {34}
  EffectBase[0, 34].ImageOffset := 160; {35}
  EffectBase[0, 35].ImageOffset := 190; {36}
  EffectBase[0, 36].ImageOffset := 0; {37}
  EffectBase[0, 37].ImageOffset := 210; {38} // 雷霆剑法    ID=43
  EffectBase[0, 38].ImageOffset := 400; {39}
  EffectBase[0, 39].ImageOffset := 600; {40}
  EffectBase[0, 40].ImageOffset := 1500; {41}
  EffectBase[0, 41].ImageOffset := 650; {42}
  EffectBase[0, 42].ImageOffset := -1; // 710                                                      {43 狮子吼}     // 修改狮子吼播放特效时不能移动 chongchong 2016-11-25
  EffectBase[0, 43].ImageOffset := 740; {44}
  EffectBase[0, 44].ImageOffset := 910; {45}
  EffectBase[0, 45].ImageOffset := 940; // 诅咒术
  EffectBase[0, 46].ImageOffset := 0; {47} // 召唤圣兽
  EffectBase[0, 47].ImageOffset := 1040; {48} // 噬血术偏移修改 piaoyun 2013-08-24  //1180
  EffectBase[0, 48].ImageOffset := 1110; {49} // 召唤火灵
  EffectBase[0, 49].ImageOffset := 0; {50} // 神龙附体
  EffectBase[0, 50].ImageOffset := 630; {51} // 流星火雨
  EffectBase[0, 51].ImageOffset := 990; {52} // 飓风破
  EffectBase[0, 52].ImageOffset := 0; {53}
  EffectBase[0, 53].ImageOffset := 0; {54}
  EffectBase[0, 54].ImageOffset := 400; {55} // 倚天辟地
  EffectBase[0, 55].ImageOffset := 0; {56}
  EffectBase[0, 56].ImageOffset := 0; {57}
  EffectBase[0, 57].ImageOffset := 0; {58}
  EffectBase[0, 58].ImageOffset := 0; {59}
  EffectBase[0, 59].ImageOffset := 0; {60}
  EffectBase[0, 60].ImageOffset := 460; {61}
  EffectBase[0, 61].ImageOffset := 290; {62}
  EffectBase[0, 62].ImageOffset := 610; {63}
  EffectBase[0, 63].ImageOffset := 190; {64}
  EffectBase[0, 64].ImageOffset := 540; {65}
  EffectBase[0, 65].ImageOffset := 0; {66}
  EffectBase[0, 66].ImageOffset := 940; // 新诅咒术 chongchong 2015-07-25
  EffectBase[0, 67].ImageOffset := 490; {68 新武力盾}
  EffectBase[0, 68].ImageOffset := 710; {69 新道力盾}
  EffectBase[0, 69].ImageOffset := 1560; {70 捕捉宠物，起手动作同诱惑之光 chongchong 2016-05-20}
  EffectBase[0, 70].ImageOffset := 1010; {71 招魂术起手动作 chongchong 2016-08-03}
  EffectBase[0, 71].ImageOffset := 0; {72}
  EffectBase[0, 72].ImageOffset := 0; {73}
  EffectBase[0, 73].ImageOffset := 0; {74}
  EffectBase[0, 74].ImageOffset := 0; {75}
  EffectBase[0, 75].ImageOffset := 0; {76}
  EffectBase[0, 76].ImageOffset := 0; {77}
  EffectBase[0, 77].ImageOffset := 0; {78}
  EffectBase[0, 78].ImageOffset := 0; {79}
  EffectBase[0, 79].ImageOffset := 0; {80}
  EffectBase[0, 80].ImageOffset := 0; {81}
  EffectBase[0, 81].ImageOffset := 0; {82}
  EffectBase[0, 82].ImageOffset := 0; {83}
  EffectBase[0, 83].ImageOffset := 0; {84}
  EffectBase[0, 84].ImageOffset := 0; {85}
  EffectBase[0, 85].ImageOffset := 0; {86}
  EffectBase[0, 86].ImageOffset := 0; {87}
  EffectBase[0, 87].ImageOffset := 0; {88}
  EffectBase[0, 88].ImageOffset := 0; {89}
  EffectBase[0, 89].ImageOffset := 0; {90}
  EffectBase[0, 90].ImageOffset := 0; {91}
  EffectBase[0, 91].ImageOffset := 0; {92}
  EffectBase[0, 92].ImageOffset := 0; {93}
  EffectBase[0, 93].ImageOffset := 0; {94}
  EffectBase[0, 94].ImageOffset := 0; {95}
  EffectBase[0, 95].ImageOffset := 0; {96}
  EffectBase[0, 96].ImageOffset := 0; {97}
  EffectBase[0, 97].ImageOffset := 0; {98}
  EffectBase[0, 98].ImageOffset := 100; {99}
  EffectBase[0, 99].ImageOffset := 0; {100}
  EffectBase[0, 100].ImageOffset := 0; {101}
  EffectBase[0, 101].ImageOffset := 0; {102}
  EffectBase[0, 102].ImageOffset := 0; {103}
  EffectBase[0, 103].ImageOffset := 640; {104} // 凤舞祭
  EffectBase[0, 104].ImageOffset := 4210; {105} // 惊雷爆
  EffectBase[0, 105].ImageOffset := 800; {106} // 冰天雪地
  EffectBase[0, 106].ImageOffset := 1040; {107} // 双龙破
  EffectBase[0, 107].ImageOffset := 1200; {108} // 虎啸诀
  EffectBase[0, 108].ImageOffset := 1440; {109} // 八卦掌
  EffectBase[0, 109].ImageOffset := 1600; {110} // 三焰咒
  EffectBase[0, 110].ImageOffset := 1760; {111} // 万剑归宗
  EffectBase[0, 111].ImageOffset := -1; {112}
  EffectBase[0, 112].ImageOffset := -1; {113}
  EffectBase[0, 113].ImageOffset := -1; {114}
  EffectBase[0, 114].ImageOffset := -1; {115}
  EffectBase[0, 115].ImageOffset := 2040; // 血魄一击(法)
  EffectBase[0, 116].ImageOffset := 2180; // 血魄一击(道)
  EffectBase[0, 117].ImageOffset := -1; {118}
  EffectBase[0, 118].ImageOffset := -1; {119}
  EffectBase[0, 119].ImageOffset := -1; {120}
  EffectBase[0, 120].ImageOffset := -1; {121}

  EffectBase[0, 121].ImageOffset := -1; {122}
  EffectBase[0, 122].ImageOffset := -1; {123}
  EffectBase[0, 123].ImageOffset := -1; {124}
  EffectBase[0, 124].ImageOffset := -1; {125}
  EffectBase[0, 125].ImageOffset := -1; {126}
  EffectBase[0, 126].ImageOffset := -1; {127}
  EffectBase[0, 127].ImageOffset := -1; {128}
  EffectBase[0, 128].ImageOffset := -1; {129}
  EffectBase[0, 129].ImageOffset := -1; {130}
  EffectBase[0, 130].ImageOffset := -1; {131}
  EffectBase[0, 131].ImageOffset := -1; {132}
  EffectBase[0, 132].ImageOffset := -1; {133}
  EffectBase[0, 133].ImageOffset := -1; {134}
  EffectBase[0, 134].ImageOffset := -1; {135}
  EffectBase[0, 135].ImageOffset := -1; {136}
  EffectBase[0, 136].ImageOffset := -1; {137}
  EffectBase[0, 137].ImageOffset := -1; {138}
  EffectBase[0, 138].ImageOffset := -1; {139}
  EffectBase[0, 139].ImageOffset := -1; {140}
  EffectBase[0, 140].ImageOffset := -1; {141}
  EffectBase[0, 141].ImageOffset := -1; {142}
  EffectBase[0, 142].ImageOffset := -1; {143}
  EffectBase[0, 143].ImageOffset := -1; {144}
  EffectBase[0, 144].ImageOffset := -1; {145}
  EffectBase[0, 145].ImageOffset := -1; {146}
  EffectBase[0, 146].ImageOffset := -1; {147}
  EffectBase[0, 147].ImageOffset := -1; {148}
  EffectBase[0, 148].ImageOffset := -1; {149}
  EffectBase[0, 149].ImageOffset := -1; {150}
  EffectBase[0, 150].ImageOffset := -1; {151}
  EffectBase[0, 151].ImageOffset := -1; {152}
  EffectBase[0, 152].ImageOffset := -1; {153}
  EffectBase[0, 153].ImageOffset := -1; {154}
  EffectBase[0, 154].ImageOffset := -1; {155}
  EffectBase[0, 155].ImageOffset := -1; {156}
  EffectBase[0, 156].ImageOffset := -1; {157}
  EffectBase[0, 157].ImageOffset := -1; {158}
  EffectBase[0, 158].ImageOffset := -1; {159}
  EffectBase[0, 159].ImageOffset := -1; {160}
  EffectBase[0, 160].ImageOffset := -1; {161}
  EffectBase[0, 161].ImageOffset := -1; {162}
  EffectBase[0, 162].ImageOffset := -1; {163}
  EffectBase[0, 163].ImageOffset := -1; {164}
  EffectBase[0, 164].ImageOffset := -1; {165}
  EffectBase[0, 165].ImageOffset := -1; {166}
  EffectBase[0, 166].ImageOffset := -1; {167}
  EffectBase[0, 167].ImageOffset := -1; {168}
  EffectBase[0, 168].ImageOffset := -1; {169}
  EffectBase[0, 169].ImageOffset := -1; {170}
  EffectBase[0, 170].ImageOffset := -1; {171}
  EffectBase[0, 171].ImageOffset := -1; {172}
  EffectBase[0, 172].ImageOffset := -1; {173}
  EffectBase[0, 173].ImageOffset := -1; {174}
  EffectBase[0, 174].ImageOffset := -1; {175}
  EffectBase[0, 175].ImageOffset := -1; {176}
  EffectBase[0, 176].ImageOffset := -1; {177}
  EffectBase[0, 177].ImageOffset := -1; {178}
  EffectBase[0, 178].ImageOffset := -1; {179}
  EffectBase[0, 179].ImageOffset := -1; {180}
  EffectBase[0, 180].ImageOffset := -1; {181}
  EffectBase[0, 181].ImageOffset := -1; {182}
  EffectBase[0, 182].ImageOffset := -1; {183}
  EffectBase[0, 183].ImageOffset := -1; {184}
  EffectBase[0, 184].ImageOffset := -1; {185}
  EffectBase[0, 185].ImageOffset := -1; {186}
  EffectBase[0, 186].ImageOffset := -1; {187}
  EffectBase[0, 187].ImageOffset := -1; {188}
  EffectBase[0, 188].ImageOffset := -1; {189}
  EffectBase[0, 189].ImageOffset := -1; {190}
  EffectBase[0, 190].ImageOffset := -1; {191}
  EffectBase[0, 191].ImageOffset := -1; {192}
  EffectBase[0, 192].ImageOffset := -1; {193}
  EffectBase[0, 193].ImageOffset := -1; {194}
  EffectBase[0, 194].ImageOffset := -1; {195}
  EffectBase[0, 195].ImageOffset := -1; {196}
  EffectBase[0, 196].ImageOffset := -1; {197}
  EffectBase[0, 197].ImageOffset := -1; {198}
  EffectBase[0, 198].ImageOffset := 100; {199} // 月灵魔法 起始动作 chongchong 2014-05-20
  EffectBase[0, 199].ImageOffset := 280; {200} // 月灵魔法 起始动作 chongchong 2014-05-20

  EffectBase[0, 200].ImageOffset := 160; {201} // 添加的新技能 -- 2013-6-17
  EffectBase[0, 201].ImageOffset := 120; {202} // 裂神符 -- 2013-06-24
  EffectBase[0, 202].ImageOffset := 0; {203} // 死亡之眼 -- 2013-06-24
  EffectBase[0, 203].ImageOffset := 200; {204} // 十步一杀 -- 2013-06-25
  EffectBase[0, 204].ImageOffset := 360; {205} // 冰霜雪雨 -- 2013-06-25
  EffectBase[0, 205].ImageOffset := 60; {206} // 冰霜群雨 -- 2013-06-25
  EffectBase[0, 206].ImageOffset := -1; {207} // **** -- 2013-09-14
  EffectBase[0, 207].ImageOffset := 1620; {208} // 旋风斩 -- 2013-09-14
  EffectBase[0, 208].ImageOffset := 1630; {209} // 五雷轰 -- 2013-09-14
  EffectBase[0, 209].ImageOffset := 1660; {210} // 幽冥火符 -- 2013-09-14

  for I := 0 to MAXEFFECT - 1 do begin
    case I of
      8, 27, 33..35, 37..39, 41..42, 43, 44, 45 {46}..48, 66:begin
          EffectBase[0, I].GameImages := g_WMagic2Images;
        end;
      31:begin
          EffectBase[0, I].GameImages := g_WMagic2Images; // 解毒术 chongchong 2015-05-21
        end;
      36:begin
          EffectBase[0, I].GameImages := g_WMonImages.Indexs[22];
        end;
      80..82:begin
          EffectBase[0, I].GameImages := g_WDragonImg;
        end;
      89:begin
          EffectBase[0, I].GameImages := g_WDragonImg;
        end;
      59..64:begin
          EffectBase[0, I].GameImages := g_WMagic4Images;
        end;
      50, 67 {新武力盾}, 68 {新道力盾}:begin
          EffectBase[0, I].GameImages := g_WMagic6Images; // 流星火雨
        end;
      51, 70 {招魂术}:begin
          EffectBase[0, I].GameImages := g_WMagic2Images; // 飓风破
        end;
      198, 199:begin
          EffectBase[0, I].GameImages := g_WMagic5Images; // 月灵魔法
        end;
      73:begin // 分身术
          EffectBase[0, I].GameImages := g_WMagic5Images;
        end;
      99..110:begin // 连击
          EffectBase[0, I].GameImages := g_cboEffect;
        end;
      54:begin // 倚天辟地
          EffectBase[0, I].GameImages := g_cboEffect;
        end;
      201:begin
          EffectBase[0, I].GameImages := g_WMagic6Images;
        end;
      202..225: {// 新技能} begin
          EffectBase[0, I].GameImages := g_WMagic10Images;
        end;
      115, 116:begin
          EffectBase[0, I].GameImages := g_WMagic8Images;
        end;
      else begin
          // EffectBase[0, I].GameImages := g_WMagicImages;
        end;
    end;
  end;

  HitEffectBase[0, 0].ImageOffset := 800; {1}
  HitEffectBase[0, 1].ImageOffset := 1410; {2}
  HitEffectBase[0, 2].ImageOffset := 1700; {3}
  HitEffectBase[0, 3].ImageOffset := 3480; {4} // 烈火
  HitEffectBase[0, 4].ImageOffset := 3390; {5}
  HitEffectBase[0, 5].ImageOffset := 40; {6}
  HitEffectBase[0, 6].ImageOffset := 220; {7} // 雷霆剑法
  HitEffectBase[0, 7].ImageOffset := 740; {8} // 龙影剑法     ID=42
  HitEffectBase[0, 8].ImageOffset := 10; {9} // 破魂斩
  HitEffectBase[0, 9].ImageOffset := 495; {10} // 劈星斩
  HitEffectBase[0, 10].ImageOffset := 310; {11} // 雷霆一击
  HitEffectBase[0, 11].ImageOffset := 470; {12} // 开天斩重击 piaoyun
  HitEffectBase[0, 12].ImageOffset := 1; {13}
  HitEffectBase[0, 13].ImageOffset := 512; {14} // 逐日剑法 原来是510(Magic6，为了使帧数一致，改为512)
  HitEffectBase[0, 14].ImageOffset := 0; {15}
  HitEffectBase[0, 15].ImageOffset := 0; {16}
  HitEffectBase[0, 16].ImageOffset := 0; {17}
  HitEffectBase[0, 17].ImageOffset := 0; {18}
  HitEffectBase[0, 18].ImageOffset := 0; {19}
  HitEffectBase[0, 19].ImageOffset := 160; {20} // 三绝杀
  HitEffectBase[0, 20].ImageOffset := 320; {21} // 断岳斩
  HitEffectBase[0, 21].ImageOffset := 560; {22} // 横扫千军
  HitEffectBase[0, 22].ImageOffset := 80; {23} // 追心刺
  HitEffectBase[0, 23].ImageOffset := 0; {24}
  HitEffectBase[0, 24].ImageOffset := 630; {25} // 开天斩轻击
  HitEffectBase[0, 25].ImageOffset := 400; {26} // 断空斩
  HitEffectBase[0, 26].ImageOffset := 2380; {26} // 血魄一击

  for I := 0 to MAXHITEFFECT - 1 do begin
    case I of
      0..4:begin
          HitEffectBase[0, I].GameImages := g_WMagicImages;
        end;
      5..7:begin
          HitEffectBase[0, I].GameImages := g_WMagic2Images;
        end;
      8..10:begin // 合击魔法
          HitEffectBase[0, I].GameImages := g_WMagic4Images;
        end;
      11:begin
          HitEffectBase[0, I].GameImages := g_WMagic5Images;
        end;
      12, 13:begin // 4级烈火  逐日剑法
          HitEffectBase[0, I].GameImages := g_WMagic6Images;
        end;
      19..23, 25:begin // 三绝杀 断岳斩 横扫千军 追心刺 断空斩
          HitEffectBase[0, I].GameImages := g_cboEffect;
        end;
      24:begin // 开天斩轻击
          HitEffectBase[0, I].GameImages := g_WMagic5Images;
        end;
      26:begin // 开天斩轻击
          HitEffectBase[0, I].GameImages := g_WMagic8Images;
        end;
    end;
  end;

  for I := 1 to 9 do begin
    Move(EffectBase[0, 0], EffectBase[I, 0], SizeOf(TImageOffset) * MAXEFFECT);
  end;

  for I := 1 to 9 do begin
    Move(HitEffectBase[0, 0], HitEffectBase[I, 0], SizeOf(TImageOffset) * MAXHITEFFECT);
  end;

  MagicImageOffsetInitLevel;
end;

end.
