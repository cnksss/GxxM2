unit uWeatherEffectDef;
// 天气特效定义

interface

uses
  Classes;

type
  TWeateherEffect = packed record
    boIsUsed:Boolean; // 是否使用特效
    boIsDark:Boolean; // 是否黑暗
    dwIndex:LongWord; // 当前帧序
    sMusic:string[50]; // 文件名称
    dwStartOffset:LongWord; // 起始图片偏移
    dwEndOffset:LongWord; // 结束图片偏移
    dwTick:LongWord; // 当前时间
  end;

var
  g_WeateherEffect:array[0..21] of TWeateherEffect = (
    (boIsUsed:False; boIsDark:False; dwIndex:0; sMusic: ''; dwStartOffset:0; dwEndOffset:9; dwTick:0), // 黄沙效果
    (boIsUsed:False; boIsDark:False; dwIndex:0; sMusic: ''; dwStartOffset:0; dwEndOffset:9; dwTick:0), // 花瓣效果
    (boIsUsed:False; boIsDark:False; dwIndex:0; sMusic: ''; dwStartOffset:110; dwEndOffset:149; dwTick:0), // 下雪效果
    (boIsUsed:False; boIsDark:False; dwIndex:0; sMusic: ''; dwStartOffset:0; dwEndOffset:29; dwTick:0), // 1
    (boIsUsed:False; boIsDark:False; dwIndex:0; sMusic: ''; dwStartOffset:30; dwEndOffset:59; dwTick:0), // 2
    (boIsUsed:False; boIsDark:False; dwIndex:0; sMusic: ''; dwStartOffset:60; dwEndOffset:89; dwTick:0), // 3
    (boIsUsed:False; boIsDark:False; dwIndex:0; sMusic: ''; dwStartOffset:90; dwEndOffset:119; dwTick:0), // 4
    (boIsUsed:False; boIsDark:False; dwIndex:0; sMusic: ''; dwStartOffset:120; dwEndOffset:149; dwTick:0), // 5
    (boIsUsed:False; boIsDark:False; dwIndex:0; sMusic: ''; dwStartOffset:150; dwEndOffset:179; dwTick:0), // 6
    (boIsUsed:False; boIsDark:False; dwIndex:0; sMusic: ''; dwStartOffset:180; dwEndOffset:209; dwTick:0), // 7
    (boIsUsed:False; boIsDark:False; dwIndex:0; sMusic: ''; dwStartOffset:210; dwEndOffset:239; dwTick:0), // 8
    (boIsUsed:False; boIsDark:False; dwIndex:0; sMusic: ''; dwStartOffset:240; dwEndOffset:269; dwTick:0), // 9
    (boIsUsed:False; boIsDark:False; dwIndex:0; sMusic: ''; dwStartOffset:270; dwEndOffset:299; dwTick:0), // 10
    (boIsUsed:False; boIsDark:False; dwIndex:0; sMusic: ''; dwStartOffset:300; dwEndOffset:359; dwTick:0), // 11
    (boIsUsed:False; boIsDark:False; dwIndex:0; sMusic: ''; dwStartOffset:360; dwEndOffset:389; dwTick:0), // 12
    (boIsUsed:False; boIsDark:False; dwIndex:0; sMusic: ''; dwStartOffset:390; dwEndOffset:419; dwTick:0), // 13
    (boIsUsed:False; boIsDark:False; dwIndex:0; sMusic: ''; dwStartOffset:420; dwEndOffset:449; dwTick:0), // 14
    (boIsUsed:False; boIsDark:False; dwIndex:0; sMusic: ''; dwStartOffset:450; dwEndOffset:479; dwTick:0), // 15
    (boIsUsed:False; boIsDark:False; dwIndex:0; sMusic: ''; dwStartOffset:480; dwEndOffset:509; dwTick:0), // 16
    (boIsUsed:False; boIsDark:False; dwIndex:0; sMusic: ''; dwStartOffset:510; dwEndOffset:539; dwTick:0), // 17
    (boIsUsed:False; boIsDark:False; dwIndex:0; sMusic: ''; dwStartOffset:540; dwEndOffset:569; dwTick:0), // 18
    (boIsUsed:False; boIsDark:False; dwIndex:0; sMusic: ''; dwStartOffset:570; dwEndOffset:599; dwTick:0) // 19
    // (boIsUsed: False; boIsDark: False; dwIndex: 0; sMusic : '';dwStartOffset: 600; dwEndOffset: 209; dwTick: 0), // 20
    );

implementation

end.
