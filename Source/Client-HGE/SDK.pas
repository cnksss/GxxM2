unit SDK;

interface
uses
  Windows,
  SysUtils,
  Classes,
  Graphics,
  DIB,
  HUtil32,
  DxCanvas,
  HGE,
  IniFiles;

var
NEWOPUIIMAGEFILE:string = 'Resources\Data\NewopUI.pak';

//{$IFDEF BEIJING}
MOBILEIMAGEFILE:string = 'Resources\Data\Mobile.pak';
//{$ENDIF}

var
  g_sVersion:string; // = 'PPU=Q@i@NsAsLpHtWsDgHbynFnh'; //版本:2023-08-29
  g_nClientVersion:Integer; // = 20230901; //配合反挂网关使用的, 每次发新版本需要改

const
  ENCRYPOINT = 0; //加密坐标版本

  //HZQ 完整编译才能使条件生效
  {$IFDEF DEBUG}
  TESTMODE = 1; // 0:正式发布; 1:调试模式; 2:定制模式 定制模式的UI是集成在资源中的
  {$ELSE}
  TESTMODE = 0;
  {$ENDIF}

  //CustomGUITest = 0;

  TESTMODE_LOG = 0; // 插件日志输出

  PRIVATE_CLIENT = 0; // = 1 专用登录器内核

  AntiPlugInMemory = 1; // 内存加载反外挂挂件 chongchong 2017-09-01

  GAMELOGIN_MUST_PLUG = 0;

  // 内核是否支持插件 chongchong 2016-07-28
  Enabled_PlugEngine = 1;

  // 调试：  B不动，A绕着B跑时，B看A会有延时
  DEBUG_RUN_DELAY_SELF = 0;
  DEBUG_RUN_DELAY_OTHER = 0;

  DEBUG_HIT_DELAY = 0;

  DEBUG_SPELL_DELAY = 0;

  DEBUG_MOVE_STUCK = 0; // 调试移动卡

  // 绘制地图纹理名 用于调试微端地图有黑块 chongchong 2016-01-09
  DRAW_MAP_TILE_INFO = 0;

  SELECTEDFRAME = 16;
  FREEZEFRAME = 13;
  EFFECTFRAME = 14;
  RUNLOGINCODE = 0; // 进入游戏状态码,默认为0 测试为 9

  HALF_SPEED_INTERVALS_COUNT = 200;
  SPEED_INTERVALS_COUNT = HALF_SPEED_INTERVALS_COUNT * 2 + 1;

  HINT_WIN_MIN = 0;
  HINT_WIN_MAX = 2;

  Def205FontColor:TColor = 11064552;

  DirsStr:array[0..7] of string = ('上↑', '右上↗', '右→', '右下↘', '下↓', '左下↙', '左←', '左上↖');

  // 朋友、黑名单文件名 piaoyun 2013-09-11
  FriendListFile = 'Config\%s.%s.Friends.txt';
  TargetListFile = 'Config\%s.%s.Target.txt';
  BlackListFile = 'Config\%s.%s.Black.txt';

  WeaponImageDir = 'Graphics\Weapon\%d.wil';
  HumImageDir = 'Graphics\Human\%d.wil';
  //HumEffectImageDir = 'Graphics\HumEffect\%d.wil';

  ITEMFILTER = 'Config\%s.%s.ItemFilter.dat';
  MAPDESC3FILE = 'Data\MapDesc3.Dat';

  LIGHTIMAGEFILE = 'Data\Light.pak';

  UIIMAGEFILE = 'Data\UI.wil';
  UIIMAGEFILE1 = 'Data\UI1.wil';
  UIIMAGEFILE2 = 'Data\UI2.wil';
  UIIMAGEFILE3 = 'Data\UI3.wil';

  MAINIMAGEFILE = 'Data\Prguse.wil';
  MAINIMAGEFILE2 = 'Data\Prguse2.wil';
  MAINIMAGEFILE3 = 'Data\Prguse3.wil';

  UINMAGEFILE3 = 'Data\ui_n.wil';
  NSELECTMAGEFILE3 = 'Data\nselect.wil';
  UICOMMONMAGEFILE3 = 'Data\ui_common.wil';

  // 扩展两个首饰内观效果 piaoyun 2013-08-01
  HEADGEAREFFECT = 'Data\headgearEffect.wil';
  HEADGEAREFFECT2 = 'Data\headgearEffect2.wil';
  HEADGEAREFFECT3 = 'Data\headgearEffect3.wil';
  HEADGEAREFFECT4 = 'Data\headgearEffect4.wil';
  HEADGEAREFFECT5 = 'Data\headgearEffect5.wil';
  HEADGEAREFFECT6 = 'Data\headgearEffect6.wil';

  MAINIMAGEFILE_16 = 'Data\Prguse_16.wil';
  MAINIMAGEFILE2_16 = 'Data\Prguse2_16.wil';
  MAINIMAGEFILE3_16 = 'Data\Prguse3_16.wil';
  CHRSELIMAGEFILE_16 = 'Data\ChrSel_16.wil';

  EFFECTIMAGEDIR = 'Data\';

  CHRSELIMAGEFILE = 'Data\ChrSel.wil';
  MINMAPIMAGEFILE = 'Data\mmap.wil';
  MINMAPIMAGEFILE10 = 'Data\mmap10.wil';

  MINMAPIMAGEFILE11 = 'Data\mmap11.wil';
  MINMAPIMAGEFILE12 = 'Data\mmap12.wil';

  TITLESIMAGEFILE = 'Data\Tiles.wil';
  SMLTITLESIMAGEFILE = 'Data\SmTiles.wil';

  TITLESIMAGEFILES = 'Data\Tiles%d.wil';
  SMLTITLESIMAGEFILES = 'Data\SmTiles%d.wil';

  HMAP_TITLESIMAGEFILE = 'HMapData\Tiles.wzl';
  HMAP_SMLTITLESIMAGEFILE = 'HMapData\SmTiles.wzl';

  HMAP_TITLESIMAGEFILES = 'HMapData\Tiles%d.wzl';
  HMAP_SMLTITLESIMAGEFILES = 'HMapData\SmTiles%d.wzl';

  HUMWINGIMAGESFILE = 'Data\HumEffect.wil';
  // HumEffect2 - 5.wzl支持 piaoyun 2013-07-28
  HUMWINGIMAGESFILEEX = 'Data\HumEffect%d.wil';

  MAGICONIMAGESFILE = 'Data\MagIcon.wil';
  MAGICON2IMAGESFILE = 'Data\MagIcon2.wil';

  HUMIMGIMAGESFILE = 'Data\Hum.wil';
  HAIRIMGIMAGESFILE = 'Data\Hair.wil';
  HAIRIMGIMAGESFILE2 = 'Data\Hair2.wil';
  HAIRIMGIMAGESFILE3 = 'Data\Hair3.wil';
  HAIRIMGIMAGESFILE4 = 'Data\Hair4.wil';
  HAIRIMGIMAGESFILE5 = 'Data\Hair5.wil';
  HAIRIMGIMAGESFILE6 = 'Data\Hair6.wil';

  HAIRIMGIMAGESFILE10 = 'Data\Hair10.wil';
  HAIRIMGIMAGESFILE11 = 'Data\Hair11.wil';
  WEAPONIMAGESFILE = 'Data\Weapon.wil';
  NPCIMAGESFILE = 'Data\Npc.wil';
  MAGICIMAGESFILE = 'Data\Magic.wil';
  MAGIC2IMAGESFILE = 'Data\Magic2.wil';
  MAGIC3IMAGESFILE = 'Data\Magic3.wil';
  MAGIC4IMAGESFILE = 'Data\Magic4.wil';
  MAGIC5IMAGESFILE = 'Data\Magic5.wil';
  MAGIC6IMAGESFILE = 'Data\Magic6.wil';
  MAGIC7IMAGESFILE16 = 'Data\Magic7-16.wil';
  MAGIC8IMAGESFILE = 'Data\Magic8.wil';
  MAGIC8IMAGESFILE16 = 'Data\Magic8-16.wil';
  MAGIC9IMAGESFILE = 'Data\Magic9.wil';
  MAGIC10IMAGESFILE = 'Data\Magic10.wil';
  MAGICREIMAGESFILE = 'Data\Magicre.wil';

  EVENTEFFECTIMAGESFILE = EFFECTIMAGEDIR + 'Event.wil';
  BAGITEMIMAGESFILE = 'Data\Items.wil';
  STATEITEMIMAGESFILE = 'Data\StateItem.wil';
  DNITEMIMAGESFILE = 'Data\DnItems.wil';

  OBJECTIMAGEFILE = 'Data\Objects.wil';
  OBJECTIMAGEFILE1 = 'Data\Objects%d.wil';

  HMAP_OBJECTIMAGEFILE = 'HMapData\Objects.wzl';
  HMAP_OBJECTIMAGEFILE1 = 'HMapData\Objects%d.wzl';

  MONIMAGEFILE = 'Data\Mon%d.wil';
  // 支持Mon-kulou.wil文件
  MONKULOUIMAGEFILE = 'Data\Mon-kulou.wil';
  DRAGONIMAGEFILE = 'Data\Dragon.wil';
  EFFECTIMAGEFILE = 'Data\Effect.wil';

  MONEFFECTIMAGEFILE = 'Data\MonEffect.wil';

  EFFECTEXIMAGEFILE = 'Data\Effect_EX.wil';
  EFFECTSEIMAGEFILE = 'Data\Effect_SE.wil';
  EFFECTWEATHERIMAGEFILE = 'Data\Effect_Weather.wil';

  SHIELDIMAGEFILE = 'Data\Shield.wil';

  HORSEIMAGEFILE = 'Data\horse.wil';
  HORSEIMAGEFILE2 = 'Data\horse2.wil';

  LHorseImageFile = 'Data\L-Horse.wil';
  LHorseImageFile1 = 'Data\L-Horse1.wil';
  LHorseImageFile2 = 'Data\L-Horse2.wil';

  LHorseEffectImageFile = 'Data\L-HorseEffect.wil';
  LHorseEffectImageFile1 = 'Data\L-HorseEffect1.wil';
  LHorseEffectImageFile2 = 'Data\L-HorseEffect2.wil';

  LHorseHairImageFile = 'Data\L-HairHorse.wil';
  LHorseHumImageFile = 'Data\L-HumHorse.wil';
  LHorseHumImageFile1 = 'Data\L-HumHorse1.wil';
  LHorseHumImageFile2 = 'Data\L-HumHorse2.wil';
  LHorseHumImageFile3 = 'Data\L-HumHorse3.wil';
  LHorseHumImageFile4 = 'Data\L-HumHorse4.wil';
  LHorseHumImageFile5 = 'Data\L-HumHorse5.wil';
  LHorseHumImageFile6 = 'Data\L-HumHorse6.wil';

  HUMIMGIMAGESFILEX = 'Data\Hum%d.wil';
  HUMWINGIMAGESFILEX = 'Data\HumEffect%d.wil';
  STATEITEMIMAGESFILEX = 'Data\StateItem%d.wil';
  BAGITEMIMAGESFILEX = 'Data\Items%d.wil';
  DNITEMIMAGESFILEX = 'Data\DnItems%d.wil';

  WEAPONIMAGESFILEX = 'Data\Weapon%d.wil';
  WEAPONIMAGESFILWISEX = 'Data\Weapon%d.wis';
  NPCIMAGESFILEX = 'Data\Npc%d.wil';

  ANITILESFILE1 = 'Data\AniTiles1.wil';
  SAFEPOINTEFFECT = 'Data\SafePointEffect.wil';

  MONIMAGEFILEEX = 'Graphics\Monster\%d.wil';
  MONPMFILE = 'Graphics\Monster\%d.pm';

  EXPLAINFILE = 'Data\Explain.Dat';
  EXPLAIN2FILE = 'Data\Explain2.Dat';

  FILTERFILE = 'Data\Filter.dat';

  ITEMDESCFILE = 'Data\ItemDesc.Dat';
  ITEMDESCTOPFILE = 'Data\ItemDescTop.Dat';
  MAPDESCFILE = 'Data\MapDesc.Dat';
  MAPDESC1FILE = 'Data\MapDesc1.Dat';
  BINDITEMFILE = 'Data\BindItem.Dat';
  SKILLDESCFILE = 'Data\SkillDesc.Dat';
  SKILLUPGRADEDESCFILE = 'Data\SkillUpgradeDesc.Dat';

  // GodBlessItemsFile
  GODBLESSITEMSFILE = 'Data\GodBlessItems.Dat';

  FENGHAOITEMSFILE = 'Data\fenghao.dat';

  WISMAINIMAGEFILE = 'Data\Prguse.wis';
  WISMAINIMAGEFILE2 = 'Data\Prguse2.wis';
  WISMAINIMAGEFILE3 = 'Data\Prguse3.wis';

  CBOEFFECTIMAGEFILE = 'Data\cboEffect.wis';
  CBOHAIRIMGIMAGESFILE = 'Data\cboHair.wis';
  CBOHAIRIMGIMAGESFILE10 = 'Data\cboHair10.wis';
  CBOHAIRIMGIMAGESFILE11 = 'Data\cboHair11.wis';

  CBOHUMIMGIMAGESFILE = 'Data\cboHum.wis';
  CBOHUMIMGIMAGESFILEEX = 'Data\cboHum%d.wis';

  CBOHUMEFFECTIMAGESFILE = 'Data\cboHumEffect.wis';
  CBOHUMEFFECTIMAGESFILEEX = 'Data\cboHumEffect%d.wis';

  CBOWEAPONIMAGESFILE = 'Data\cboWeapon.wis';
  CBOWEAPONIMAGESFILEEX = 'Data\cboWeapon%d.wis';

  CBOHUMDIYFILE_COUNT = 10;

  CBOHUMDIYFILE = 'Data\cboHumDiy.wzl';
  CBOHUMEFFECTDIYFILE = 'Data\cboHumEffectDiy.wzl';

  CBOHUMDIYFILE_ARR = 'Data\cboHumDiy%d.wzl';
  CBOHUMEFFECTDIYFILE_ARR = 'Data\cboHumEffectDiy%d.wzl';

  CBOWEAPONDIYFILE = 'Data\cboWeaponDiy.wzl';
  CBOWEAPONEFFECTDIYFILE = 'Data\cboWeaponEffectDiy.wzl';

  CBOWEAPONDIYFILE_ARR = 'Data\cboWeaponDiy%d.wzl';
  CBOWEAPONEFFECTDIYFILE_ARR = 'Data\cboWeaponEffectDiy%d.wzl';

  WISDNITEMIMAGESFILE = 'Data\DnItems.wis';
  WISBAGITEMIMAGESFILE = 'Data\Items.wis';
  WISSTATEITEMIMAGESFILE = 'Data\StateItem.wis';
  WISEAPONIMAGESFILE = 'Data\Weapon2.wis';

  WEAPONEFFECTIMAGESFILE = 'Data\WeaponEffect.wis';
  WEAPONEFFECTIMAGESFILEEX = 'Data\WeaponEffect%d.wis';

  CBOWEAPONEFFECTIMAGESFILE = 'Data\cboWeaponEffect.wis';
  CBOWEAPONEFFECTIMAGESFILEEX = 'Data\cboWeaponEffect%d.wis';

  // 武器、衣服内观特效 piaoyun 2013-08-10
  STATEEFFECTIMAGESFILE = 'Data\StateEffect.wil';

  STATEEFFECTIMAGESFILE_Ex_Count = 6;

  STATEEFFECTIMAGESFILE_Ex:array[0..STATEEFFECTIMAGESFILE_Ex_Count - 1] of string =
  ('Data\StateEffect5.wil', 'Data\StateEffect6.wil', 'Data\StateEffect7.wil',
    'Data\StateEffect8.wil', 'Data\StateEffect9.wil', 'Data\StateEffect10.wil');
  {
  MAXX = 40;
  MAXY = 40;
  }
{ MAXX = SCREENWIDTH div 20;
 MAXY = SCREENWIDTH div 20;    }

  DEFAULTCURSOR = 0; // 系统默认光标
  IMAGECURSOR = 1; // 图形光标

  USECURSOR = DEFAULTCURSOR; // 使用什么类型的光标

  MAXFONT = 8;
  ENEMYCOLOR = 69;

  HintGroupSpaceHeight = 1;

  sGameNoticeName = '健康游戏公告';
  sGameNoticeStr1 = '抵制不良游戏，拒绝盗版游戏，注意自我保护，谨防受骗上当。适度游戏益脑，';
  sGameNoticeStr2 = '沉迷游戏伤身，合理安排时间，享受健康生活，严厉打击赌博，营造和谐环境。';

  s_GJRunStart = '开始挂机';
  s_GJRunEnd = '停止挂机';
var
  SCREENWIDTH:Integer = 1024;
  SCREENHEIGHT:Integer = 768;

  WIDTHGRIDCOUNT:Integer = 12;
  HEIGHTGRIDCOUNT:Integer = 12;
  //BOTTOMGRIDCOUNT: Integer = 11;

  // SCREENWIDTH: Integer = 800;
  // SCREENHEIGHT: Integer = 600;

  MAPSURFACEWIDTH:Integer = 1024;
  MAPSURFACEHEIGHT:Integer = 768;

  // WINRIGHT: Integer = 520;
  // BOTTOMEDGE: Integer = 570;

  {
  MAXX: Integer = 40;
  MAXY: Integer = 40;
  }

  {$IF TESTMODE = 1}
  g_NoticeClickTick:LongWord;
  g_boStartRender:Boolean = False;
  g_TestModeShowOpenDoor:Boolean = True;
  g_TestModeShow1024UI:Boolean = False;
  g_TestModeUseCustomUI:Boolean = True;
  g_TestModeShowPropertyGroupCption:Boolean = True;
  g_TestModeUIPath:string = '.\Gui\'; //HZq
  g_TestModeGamePlanFile:string = 'Newopui.pak';
  g_TestModeResourceDir:string;
  g_TestDefaultAccount:string;
  g_TestDefaultAccountPassword:string;
  m_EatItemTick:LongWord;
  {$IFEND}

  //g_MySelfRunStartTick: LongWord;

type
  TColorEffect = (ceNone, ceGrayScale, ceBright, ceBlack, ceWhite, ceRed, ceGreen, ceBlue, ceYellow, ceFuchsia, ceAqua, ceSilver, ceGray, ceGrayScale2);

  {===================================TGList===================================}

  TGList = class(TList)
  private
    CriticalSection:TRTLCriticalSection;
  public
    constructor Create;
    destructor Destroy; override;
    procedure Lock;
    procedure UnLock;
  end;

  {=================================TGStringList================================}
  TGStringList = class(TStringList)
  private
    CriticalSection:TRTLCriticalSection;
  public
    constructor Create;
    destructor Destroy; override;
    procedure Lock;
    procedure UnLock;
  end;

  TGHashStringList = class(THashedStringList)
  private
    CriticalSection:TRTLCriticalSection;
  public
    constructor Create;
    destructor Destroy; override;
    procedure Lock;
    procedure UnLock;
  end;

  TSortStringList = class(TStringList)
  public
    procedure NumberSort(Flag:Boolean);
    procedure DateTimeSort(Flag:Boolean);
    procedure StringSort(Flag:Boolean);
  end;

  TSpeedIntervals = array[0..SPEED_INTERVALS_COUNT - 1] of Word;

  TAntiPlugActionMode = (
    amHit {攻击}, amSpell {魔法}, amWalk {走路},
    amRun {跑步}, amTurn {转向}, amCutMeat {挖肉},
    amWalkToHit {走路到攻击}, amHitToWalk {攻击到走路},
    amRunToHit {跑步到攻击}, amHitToRun {攻击到跑步},
    amWalkToSpell {走路到魔法}, amSpellToWalk {魔法到走路},
    amRunToSpell {跑步到魔法}, amSpellToRun {魔法到跑步},
    amTurnToHit {转向到攻击}, amHitToTurn {攻击到转向},
    amTurnToSpell {转向到魔法}, amSpellToTurn {魔法到转向},
    amCutMeatToHit {挖肉到攻击}, amCutMeatToSpell {挖肉到魔法},
    amMoveToTurn {移动到转向}, amTurnToMove {转向到移动},
    amMoveToCutMeat {移动到挖肉}, amCutMeatToMove {挖肉到移动},
    amHitConcurrent {攻击并发}, amSpellConcurrent {魔法并发},
    amMoveConcurrent {移动并发} (*, amAllConcurrent {所有并发}*)
    );

  // 基本动作
  TBaseAction = (baOther, baHit, baSpell, baWalk, baRun, baTurn, baCutMeat);
  
function ReadBuildTime: TDateTime;
function FastCharPos(const aSource:string; const C:Char; StartPos:Integer):Integer;
function HashPJW(const Value:string):Longint;
procedure FogCopy(PSource:Pbyte; ssx, ssy, swidth, sheight:integer;
  PDest:Pbyte; ddx, ddy, dwidth, dheight, maxfog:integer);

var
  g_wActionSpeedIntervals:array[TAntiPlugActionMode] of TSpeedIntervals;



implementation
//HZQ 去除多余的引用
//uses
//MShare,
//Grobal2;
uses
EncryptUnit_LF;

//HZQ 读取编译时间
function ReadBuildTime: TDateTime;
var
  nUsec:LongInt;
  fsApp:TFileStream;
  dosHeader: TImageDosHeader;
  ntHeader: TImageNtHeaders;
const
  UnixStartDate: TDateTime = 25569.0; // 1970/01/01    
begin
  nUsec := 0;
  try
    fsApp := TFileStream.Create(ParamStr(0), fmOpenRead or fmShareDenyNone);
  except
    fsApp := nil;
  end;

  if fsApp <> nil then begin
    if SizeOf(dosHeader) = fsApp.Read(dosHeader, Sizeof(dosHeader)) then begin
      if fsApp.Seek(dosHeader._lfanew, soBeginning) = dosHeader._lfanew then begin
        if SizeOf(ntHeader) = fsApp.Read(ntHeader, SizeOf(ntHeader)) then begin
           nUsec := ntHeader.FileHeader.TimeDateStamp;
        end;
      end;
    end;
    fsApp.Free;
  end;
  Result :=  ((nUSec + (3600 * 8)) / 86400) + UnixStartDate; //+8 Hours
end;

procedure SetGlobalVersion();
var
    dtBuild:TDateTime;
    sDate:string;
begin
    dtBuild := ReadBuildTime;
    sDate := FormatDateTime('yyyy-mm-dd', dtBuild);
    g_sVersion := EncryString_LF(Format('版本:%s', [sDate]));
    sDate := FormatDateTime('yyyymmdd', dtBuild);
    g_nClientVersion := StrToIntDef(sDate, 20230831); // = 20230901; //配合反挂网关使用的, 每次发新版本需要改
end;


procedure FogCopy(PSource:Pbyte; ssx, ssy, swidth, sheight:integer; PDest:Pbyte; ddx, ddy, dwidth, dheight, maxfog:integer);
var
  row, srclen, srcheight, spitch, dpitch:integer;
begin
  if (PSource = nil) or (pDest = nil) then exit;
  spitch := swidth;
  dpitch := dwidth;
  if ddx < 0 then begin
    ssx := ssx - ddx;
    swidth := swidth + ddx;
    // dwidth := dwidth + ddx;
    ddx := 0;
  end;
  if ddy < 0 then begin
    ssy := ssy - ddy;
    sheight := sheight + ddy;
    // dheight := dheight + ddy;
    ddy := 0;
  end;
  // if ssx+swidth > dwidth then swidth := dwidth - ssx;
  // if ssy+sheight > dheight then sheight := dheight - ssy;
  srclen := _MIN(swidth, dwidth - ddx);
  srcheight := _MIN(sheight, dheight - ddy);
  if (srclen <= 0) or (srcheight <= 0) then exit;

  asm
         mov   row, 0
      @@NextRow:
         mov   eax, row
         cmp   eax, srcheight
         jae   @@Finish

         mov   esi, psource
         mov   eax, ssy
         add   eax, row
         mov   ebx, spitch
         imul  eax, ebx
         add   eax, ssx
         add   esi, eax          // sptr

         mov   edi, pdest
         mov   eax, ddy
         add   eax, row
         mov   ebx, dpitch
         imul  eax, ebx
         add   eax, ddx
         add   edi, eax          // dptr

         mov   ebx, srclen
      @@FogNext:
         cmp   ebx, 0
         jbe   @@FinOne
         cmp   ebx, 8
         jb    @@FinOne   // @@EageNext

         db $0F,$6F,$06           // / movq  mm0, [esi]
         db $0F,$6F,$0F           // / movq  mm1, [edi]
         db $0F,$FE,$C8           // / paddd mm1, mm0
         db $0F,$7F,$0F           // / movq [edi], mm1

         sub   ebx, 8
         add   esi, 8
         add   edi, 8
         jmp   @@FogNext
      {@@EageNext:
         movzx eax, [esi].byte
         movzx ecx, [edi].byte
         add   eax, ecx
         mov   [edi].byte, al

         dec   ebx
         inc   esi
         inc   edi
         jmp   @@FogNext }
      @@FinOne:
         inc   row
         jmp   @@NextRow

      @@Finish:
         db $0F,$77               // / emms
  end;
end;

function FastCharPos(const aSource:string; const C:Char; StartPos:Integer):Integer;
var
  L:Integer;
begin
  // If this assert failed, it is because you passed 0 for StartPos, lowest value is 1 !!
  Assert(StartPos > 0);

  Result := 0;
  L := Length(aSource);
  if L = 0 then exit;
  if StartPos > L then exit;
  Dec(StartPos);
  asm
      PUSH EDI                 // Preserve this register

      mov  EDI, aSource        // Point EDI at aSource
      add  EDI, StartPos
      mov  ECX, L              // Make a note of how many chars to search through
      sub  ECX, StartPos
      mov  AL,  C              // and which char we want
    @Loop:
      cmp  Al, [EDI]           // compare it against the SourceString
      jz   @Found
      inc  EDI
      dec  ECX
      jnz  @Loop
      jmp  @NotFound
    @Found:
      sub  EDI, aSource        // EDI has been incremented, so EDI-OrigAdress = Char pos !
      inc  EDI
      mov  Result,   EDI
    @NotFound:

      POP  EDI
  end;
end;

function HashPJW(const Value:string):Longint;
var
  I:Integer;
  G:Longint;
begin
  Result := 0;
  for I := 1 to Length(Value) do begin
    Result := (Result shl 4) + Ord(Value[I]);
    G := Result and $F0000000;
    if G <> 0 then
      Result := (Result xor (G shr 24)) xor G;
  end;
end;

{ TGList }

constructor TGList.Create;
begin
  inherited;
  InitializeCriticalSection(CriticalSection);
end;

destructor TGList.Destroy;
begin
  DeleteCriticalSection(CriticalSection);
  inherited;
end;

procedure TGList.Lock;
begin
  EnterCriticalSection(CriticalSection);
end;

procedure TGList.UnLock;
begin
  LeaveCriticalSection(CriticalSection);
end;

{ TGStringList }

constructor TGStringList.Create;
begin
  inherited;
  InitializeCriticalSection(CriticalSection);
end;

destructor TGStringList.Destroy;
begin
  DeleteCriticalSection(CriticalSection);
  inherited;
end;

procedure TGStringList.Lock;
begin
  EnterCriticalSection(CriticalSection);
end;

procedure TGStringList.UnLock;
begin
  LeaveCriticalSection(CriticalSection);
end;

{ TGStringList }

constructor TGHashStringList.Create;
begin
  inherited;
  InitializeCriticalSection(CriticalSection);
end;

destructor TGHashStringList.Destroy;
begin
  DeleteCriticalSection(CriticalSection);
  inherited;
end;

procedure TGHashStringList.Lock;
begin
  EnterCriticalSection(CriticalSection);
end;

procedure TGHashStringList.UnLock;
begin
  LeaveCriticalSection(CriticalSection);
end;

//---------------------------

function NumberSort_1(List:TStringList; Index1, Index2:Integer):Integer; // 从大到小排列
var
  Value1, Value2:Integer;
begin
  Result := 0;
  try
    Value1 := StrToInt(List[Index1]);
    Value2 := StrToInt(List[Index2]);
    if Value1 > Value2 then
      Result := -1
    else if Value1 < Value2 then
      Result := 1
    else
      Result := 0;
  except
  end;
end;

// -------数字排序 2

function NumberSort_2(List:TStringList; Index1, Index2:Integer):Integer; // 从小到大排列
var
  Value1, Value2:Integer;
begin
  Result := 0;
  try
    Value1 := StrToInt(List[Index1]);
    Value2 := StrToInt(List[Index2]);
    if Value1 > Value2 then
      Result := 1
    else if Value1 < Value2 then
      Result := -1
    else
      Result := 0;
  except
  end;
end;

// -------日期排序 1

function DateTimeSort_1(List:TStringList; Index1, Index2:Integer):Integer;
var
  Value1, Value2:TDateTime;
begin
  Result := 0;
  try
    Value1 := StrToDateTime(List[Index1]);
    Value2 := StrToDateTime(List[Index2]);
    if Value1 > Value2 then
      Result := -1
    else if Value1 < Value2 then
      Result := 1
    else
      Result := 0;
  except
  end;
end;

// -------日期排序 2

function DateTimeSort_2(List:TStringList; Index1, Index2:Integer):Integer;
var
  Value1, Value2:TDateTime;
begin
  Result := 0;
  try
    Value1 := StrToDateTime(List[Index1]);
    Value2 := StrToDateTime(List[Index2]);
    if Value1 > Value2 then
      Result := 1
    else if Value1 < Value2 then
      Result := -1
    else
      Result := 0;
  except
  end;
end;

// -------字符串排序 1

function StrSort_1(List:TStringList; Index1, Index2:Integer):Integer;
begin
  Result := 0;
  try
    Result := -CompareStr(List[Index1], List[Index2]);
  except
  end;
end;

// -------字符串排序 2

function StrSort_2(List:TStringList; Index1, Index2:Integer):Integer;
begin
  Result := 0;
  try
    Result := CompareStr(List[Index1], List[Index2]);
  except
  end;
end;

procedure TSortStringList.NumberSort(Flag:Boolean);
begin
  if Flag then
    CustomSort(NumberSort_1)
  else
    CustomSort(NumberSort_2);
end;

procedure TSortStringList.DateTimeSort(Flag:Boolean);
begin
  if Flag then
    CustomSort(DateTimeSort_1)
  else
    CustomSort(DateTimeSort_2);
end;

procedure TSortStringList.StringSort(Flag:Boolean);
begin
  if Flag then
    CustomSort(StrSort_1)
  else
    CustomSort(StrSort_2);
end;

procedure DrawFog(Surface:Pointer; Width, Height, Pitch:Integer; fogmask:PByte; fogwidth:Integer);
var
  row:integer;
  srclen, srcheight:integer;
  lpitch:integer;
  //src: array[0..7] of byte;
  {pSrc, } psource, pColorLevel:Pbyte;
begin
  // if Width > SCREENWIDTH + 100 then exit;
// if ssuf.Width > 900 then exit;
{  case DarkLevel of
    1: pColorLevel := @HeavyDarkColorLevel;
    2: pColorLevel := @LightDarkColorLevel;
    3: pColorLevel := @DengunColorLevel;
  else exit;
  end; }

  srclen := _MIN(Width, fogwidth);
  //pSrc := @src;
  srcheight := Height;
  lpitch := Pitch;
  psource := Surface;

  asm
            mov   row, 0
         @@NextRow:
            mov   ebx, row
            mov   eax, srcheight
            cmp   ebx, eax
            jae   @@DrawFogFin

            mov   esi, psource      // esi = ddsd.lpSurface;
            mov   eax, lpitch
            mov   ebx, row
            imul  eax, ebx
            add   esi, eax

            mov   edi, fogmask      // edi = fogmask
            mov   eax, fogwidth
            mov   ebx, row
            imul  eax, ebx
            add   edi, eax

            mov   ecx, srclen
            mov   edx, pColorLevel

         @@NextByte:
            cmp   ecx, 0
            jbe   @@Finish

            movzx eax, [edi].byte   // fogmask
            // /cmp   eax, 30
            // /ja    @@SkipByte
            imul  eax, 256
            movzx ebx, [esi].byte   // 家胶 ddsd.lpSurface;
            add   eax, ebx
            mov   al, [edx+eax].byte // pColorLevel
            mov   [esi].byte, al
         // /@@SkipByte:
            dec   ecx
            inc   esi
            inc   edi
            jmp   @@NextByte

         @@Finish:
            inc   row
            jmp   @@NextRow

         @@DrawFogFin:
            db $0F,$77               // / emms
  end;
end;

initialization
SetGlobalVersion();

end.
