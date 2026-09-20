// ============================================================================
// uFrmCustomItemProperty.pas（Source\M2Engine\Forms\uFrmCustomItemProperty.pas，498 行，GBK）1:1 移植
// 同源 DFM：Source\M2Engine\Forms\uFrmCustomItemProperty.dfm（33,154 字节 / 1,333 行）
// 车道 p8-m2-itemprop-misc ｜ 命名空间 GXX.M2Server.Forms.ItemProperty
//
// 类：TFrmCustomItemProperty = class(TForm)（:10-157）
//   单元级过程：ShowFrmCustomItemProperty（:159、:165-176）
//   方法：FormCreate（:178-306）、btnOKClick（:308-450）、chk01Click（:452-455）、
//         edtShowName01Change（:457-461）、mmoVarChange（:463-466）、btnOK2Click（:468-484）、
//         mmoVarKeyUp（:486-490）、mmoVarMouseDown（:492-496）
//
// 【1:1 保真要点】
//   * 120 个控件的**字段名逐字照抄**（chk01..chk60 / edtShowName01..60 / pgcMain / tsBindAttr /
//     tsText / lbl3 / pnlBottom1 / lbl1 / btnOK / pnlBottom2 / lbl2 / btnOK2 / mmoVar / lblLineNum），
//     并用 `CiCheckBoxSeam chk01 => chk[1];` 形式的**只读别名**指向同一下标数组
//     （Delphi 下界为 1，故数组长度 61，下标 0 永不使用）。
//   * FormCreate / btnOKClick 的 4 组「01..60 逐条赋值」**逐条展开照抄**（顺序即原文顺序），
//     未合并为循环，以免掩盖下标错位。
//   * DFM 事件布线 1:1 复刻（见 WireDfmEvents）：DFM 实测 60 个 TCheckBox 全部
//     `OnClick = chk01Click`（DFM:44..634）、60 个 TEdit 全部 `OnChange = edtShowName01Change`
//     （DFM:644..1255/1264）——**注意原文只有这两个处理器**，其余 59 个 chk / 59 个 edt
//     在 Pascal 里并无独立方法，托管侧据此照抄（不"顺手补齐"）。
//   * 原文 8 个处理器的 Sender/Sender 参数**在体内均未被使用**；mmoVarKeyUp 的 `var Key: Word`
//     也未被改写 —— 托管侧保留形参但标注未使用（见各方法注释）。
//
// 【无头 UI】控件是接缝对象（无窗口）；ShowModal 走 CustomItemPropertyMessageBoxSeam.UiEnabled。
// ============================================================================

using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.Core.Util;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Forms.ItemProperty;

/// <summary>uFrmCustomItemProperty.pas <c>TFrmCustomItemProperty</c> 1:1（无头：控件为接缝对象）。</summary>
public sealed class TFrmCustomItemProperty
{
    // ------------------------------------------------------------------
    // 控件（DFM 层级：TForm → pgcMain{tsBindAttr, tsText} + pnlBottom1 + pnlBottom2）
    // 原文类声明的顺序即 DFM 顺序（:11-142）。
    // ------------------------------------------------------------------

    /// <summary>宿主窗体自身（DFM <c>object FrmCustomItemProperty: TFrmCustomItemProperty</c>，:17 OnCreate = FormCreate）。</summary>
    public CiFormSeam self = new();

    /// <summary>pgcMain: TPageControl（原文 :11）。</summary>
    public CiPageControlSeam pgcMain = new();
    /// <summary>tsBindAttr: TTabSheet（原文 :12）。</summary>
    public CiTabSheetSeam tsBindAttr = new();
    /// <summary>tsText: TTabSheet（原文 :13）。</summary>
    public CiTabSheetSeam tsText = new();
    /// <summary>lbl3: TLabel（原文 :14）。</summary>
    public CiLabelSeam lbl3 = new();

    /// <summary>
    /// chk01..chk60（原文 :15-74）。<b>下标 1..60 与原文一一对应，下标 0 不用</b>
    /// （Delphi 数组/DFM 命名均从 01 起）。
    /// </summary>
    public readonly CiCheckBoxSeam[] chk = new CiCheckBoxSeam[Grobal2Const.CUSTOM_PROPERTY_BIND_TYPE_COUNT + 1];

    /// <summary>chk01（原文 :15）。</summary>
    public CiCheckBoxSeam chk01 => chk[1];
    /// <summary>chk02（原文 :16）。</summary>
    public CiCheckBoxSeam chk02 => chk[2];
    /// <summary>chk03（原文 :17）。</summary>
    public CiCheckBoxSeam chk03 => chk[3];
    /// <summary>chk04（原文 :18）。</summary>
    public CiCheckBoxSeam chk04 => chk[4];
    /// <summary>chk05（原文 :19）。</summary>
    public CiCheckBoxSeam chk05 => chk[5];
    /// <summary>chk06（原文 :20）。</summary>
    public CiCheckBoxSeam chk06 => chk[6];
    /// <summary>chk07（原文 :21）。</summary>
    public CiCheckBoxSeam chk07 => chk[7];
    /// <summary>chk08（原文 :22）。</summary>
    public CiCheckBoxSeam chk08 => chk[8];
    /// <summary>chk09（原文 :23）。</summary>
    public CiCheckBoxSeam chk09 => chk[9];
    /// <summary>chk10（原文 :24）。</summary>
    public CiCheckBoxSeam chk10 => chk[10];
    /// <summary>chk11（原文 :25）。</summary>
    public CiCheckBoxSeam chk11 => chk[11];
    /// <summary>chk12（原文 :26）。</summary>
    public CiCheckBoxSeam chk12 => chk[12];
    /// <summary>chk13（原文 :27）。</summary>
    public CiCheckBoxSeam chk13 => chk[13];
    /// <summary>chk14（原文 :28）。</summary>
    public CiCheckBoxSeam chk14 => chk[14];
    /// <summary>chk15（原文 :29）。</summary>
    public CiCheckBoxSeam chk15 => chk[15];
    /// <summary>chk16（原文 :30）。</summary>
    public CiCheckBoxSeam chk16 => chk[16];
    /// <summary>chk17（原文 :31）。</summary>
    public CiCheckBoxSeam chk17 => chk[17];
    /// <summary>chk18（原文 :32）。</summary>
    public CiCheckBoxSeam chk18 => chk[18];
    /// <summary>chk19（原文 :33）。</summary>
    public CiCheckBoxSeam chk19 => chk[19];
    /// <summary>chk20（原文 :34）。</summary>
    public CiCheckBoxSeam chk20 => chk[20];
    /// <summary>chk21（原文 :35）。</summary>
    public CiCheckBoxSeam chk21 => chk[21];
    /// <summary>chk22（原文 :36）。</summary>
    public CiCheckBoxSeam chk22 => chk[22];
    /// <summary>chk23（原文 :37）。</summary>
    public CiCheckBoxSeam chk23 => chk[23];
    /// <summary>chk24（原文 :38）。</summary>
    public CiCheckBoxSeam chk24 => chk[24];
    /// <summary>chk25（原文 :39）。</summary>
    public CiCheckBoxSeam chk25 => chk[25];
    /// <summary>chk26（原文 :40）。</summary>
    public CiCheckBoxSeam chk26 => chk[26];
    /// <summary>chk27（原文 :41）。</summary>
    public CiCheckBoxSeam chk27 => chk[27];
    /// <summary>chk28（原文 :42）。</summary>
    public CiCheckBoxSeam chk28 => chk[28];
    /// <summary>chk29（原文 :43）。</summary>
    public CiCheckBoxSeam chk29 => chk[29];
    /// <summary>chk30（原文 :44）。</summary>
    public CiCheckBoxSeam chk30 => chk[30];
    /// <summary>chk31（原文 :45）。</summary>
    public CiCheckBoxSeam chk31 => chk[31];
    /// <summary>chk32（原文 :46）。</summary>
    public CiCheckBoxSeam chk32 => chk[32];
    /// <summary>chk33（原文 :47）。</summary>
    public CiCheckBoxSeam chk33 => chk[33];
    /// <summary>chk34（原文 :48）。</summary>
    public CiCheckBoxSeam chk34 => chk[34];
    /// <summary>chk35（原文 :49）。</summary>
    public CiCheckBoxSeam chk35 => chk[35];
    /// <summary>chk36（原文 :50）。</summary>
    public CiCheckBoxSeam chk36 => chk[36];
    /// <summary>chk37（原文 :51）。</summary>
    public CiCheckBoxSeam chk37 => chk[37];
    /// <summary>chk38（原文 :52）。</summary>
    public CiCheckBoxSeam chk38 => chk[38];
    /// <summary>chk39（原文 :53）。</summary>
    public CiCheckBoxSeam chk39 => chk[39];
    /// <summary>chk40（原文 :54）。</summary>
    public CiCheckBoxSeam chk40 => chk[40];
    /// <summary>chk41（原文 :55）。</summary>
    public CiCheckBoxSeam chk41 => chk[41];
    /// <summary>chk42（原文 :56）。</summary>
    public CiCheckBoxSeam chk42 => chk[42];
    /// <summary>chk43（原文 :57）。</summary>
    public CiCheckBoxSeam chk43 => chk[43];
    /// <summary>chk44（原文 :58）。</summary>
    public CiCheckBoxSeam chk44 => chk[44];
    /// <summary>chk45（原文 :59）。</summary>
    public CiCheckBoxSeam chk45 => chk[45];
    /// <summary>chk46（原文 :60）。</summary>
    public CiCheckBoxSeam chk46 => chk[46];
    /// <summary>chk47（原文 :61）。</summary>
    public CiCheckBoxSeam chk47 => chk[47];
    /// <summary>chk48（原文 :62）。</summary>
    public CiCheckBoxSeam chk48 => chk[48];
    /// <summary>chk49（原文 :63）。</summary>
    public CiCheckBoxSeam chk49 => chk[49];
    /// <summary>chk50（原文 :64）。</summary>
    public CiCheckBoxSeam chk50 => chk[50];
    /// <summary>chk51（原文 :65）。</summary>
    public CiCheckBoxSeam chk51 => chk[51];
    /// <summary>chk52（原文 :66）。</summary>
    public CiCheckBoxSeam chk52 => chk[52];
    /// <summary>chk53（原文 :67）。</summary>
    public CiCheckBoxSeam chk53 => chk[53];
    /// <summary>chk54（原文 :68）。</summary>
    public CiCheckBoxSeam chk54 => chk[54];
    /// <summary>chk55（原文 :69）。</summary>
    public CiCheckBoxSeam chk55 => chk[55];
    /// <summary>chk56（原文 :70）。</summary>
    public CiCheckBoxSeam chk56 => chk[56];
    /// <summary>chk57（原文 :71）。</summary>
    public CiCheckBoxSeam chk57 => chk[57];
    /// <summary>chk58（原文 :72）。</summary>
    public CiCheckBoxSeam chk58 => chk[58];
    /// <summary>chk59（原文 :73）。</summary>
    public CiCheckBoxSeam chk59 => chk[59];
    /// <summary>chk60（原文 :74）。</summary>
    public CiCheckBoxSeam chk60 => chk[60];

    /// <summary>edtShowName01..edtShowName60（原文 :75-134）。下标 1..60 与原文一一对应，下标 0 不用。</summary>
    public readonly CiEditSeam[] edtShowName = new CiEditSeam[Grobal2Const.CUSTOM_PROPERTY_BIND_TYPE_COUNT + 1];

    /// <summary>edtShowName01（原文 :75）。</summary>
    public CiEditSeam edtShowName01 => edtShowName[1];
    /// <summary>edtShowName02（原文 :76）。</summary>
    public CiEditSeam edtShowName02 => edtShowName[2];
    /// <summary>edtShowName03（原文 :77）。</summary>
    public CiEditSeam edtShowName03 => edtShowName[3];
    /// <summary>edtShowName04（原文 :78）。</summary>
    public CiEditSeam edtShowName04 => edtShowName[4];
    /// <summary>edtShowName05（原文 :79）。</summary>
    public CiEditSeam edtShowName05 => edtShowName[5];
    /// <summary>edtShowName06（原文 :80）。</summary>
    public CiEditSeam edtShowName06 => edtShowName[6];
    /// <summary>edtShowName07（原文 :81）。</summary>
    public CiEditSeam edtShowName07 => edtShowName[7];
    /// <summary>edtShowName08（原文 :82）。</summary>
    public CiEditSeam edtShowName08 => edtShowName[8];
    /// <summary>edtShowName09（原文 :83）。</summary>
    public CiEditSeam edtShowName09 => edtShowName[9];
    /// <summary>edtShowName10（原文 :84）。</summary>
    public CiEditSeam edtShowName10 => edtShowName[10];
    /// <summary>edtShowName11（原文 :85）。</summary>
    public CiEditSeam edtShowName11 => edtShowName[11];
    /// <summary>edtShowName12（原文 :86）。</summary>
    public CiEditSeam edtShowName12 => edtShowName[12];
    /// <summary>edtShowName13（原文 :87）。</summary>
    public CiEditSeam edtShowName13 => edtShowName[13];
    /// <summary>edtShowName14（原文 :88）。</summary>
    public CiEditSeam edtShowName14 => edtShowName[14];
    /// <summary>edtShowName15（原文 :89）。</summary>
    public CiEditSeam edtShowName15 => edtShowName[15];
    /// <summary>edtShowName16（原文 :90）。</summary>
    public CiEditSeam edtShowName16 => edtShowName[16];
    /// <summary>edtShowName17（原文 :91）。</summary>
    public CiEditSeam edtShowName17 => edtShowName[17];
    /// <summary>edtShowName18（原文 :92）。</summary>
    public CiEditSeam edtShowName18 => edtShowName[18];
    /// <summary>edtShowName19（原文 :93）。</summary>
    public CiEditSeam edtShowName19 => edtShowName[19];
    /// <summary>edtShowName20（原文 :94）。</summary>
    public CiEditSeam edtShowName20 => edtShowName[20];
    /// <summary>edtShowName21（原文 :95）。</summary>
    public CiEditSeam edtShowName21 => edtShowName[21];
    /// <summary>edtShowName22（原文 :96）。</summary>
    public CiEditSeam edtShowName22 => edtShowName[22];
    /// <summary>edtShowName23（原文 :97）。</summary>
    public CiEditSeam edtShowName23 => edtShowName[23];
    /// <summary>edtShowName24（原文 :98）。</summary>
    public CiEditSeam edtShowName24 => edtShowName[24];
    /// <summary>edtShowName25（原文 :99）。</summary>
    public CiEditSeam edtShowName25 => edtShowName[25];
    /// <summary>edtShowName26（原文 :100）。</summary>
    public CiEditSeam edtShowName26 => edtShowName[26];
    /// <summary>edtShowName27（原文 :101）。</summary>
    public CiEditSeam edtShowName27 => edtShowName[27];
    /// <summary>edtShowName28（原文 :102）。</summary>
    public CiEditSeam edtShowName28 => edtShowName[28];
    /// <summary>edtShowName29（原文 :103）。</summary>
    public CiEditSeam edtShowName29 => edtShowName[29];
    /// <summary>edtShowName30（原文 :104）。</summary>
    public CiEditSeam edtShowName30 => edtShowName[30];
    /// <summary>edtShowName31（原文 :105）。</summary>
    public CiEditSeam edtShowName31 => edtShowName[31];
    /// <summary>edtShowName32（原文 :106）。</summary>
    public CiEditSeam edtShowName32 => edtShowName[32];
    /// <summary>edtShowName33（原文 :107）。</summary>
    public CiEditSeam edtShowName33 => edtShowName[33];
    /// <summary>edtShowName34（原文 :108）。</summary>
    public CiEditSeam edtShowName34 => edtShowName[34];
    /// <summary>edtShowName35（原文 :109）。</summary>
    public CiEditSeam edtShowName35 => edtShowName[35];
    /// <summary>edtShowName36（原文 :110）。</summary>
    public CiEditSeam edtShowName36 => edtShowName[36];
    /// <summary>edtShowName37（原文 :111）。</summary>
    public CiEditSeam edtShowName37 => edtShowName[37];
    /// <summary>edtShowName38（原文 :112）。</summary>
    public CiEditSeam edtShowName38 => edtShowName[38];
    /// <summary>edtShowName39（原文 :113）。</summary>
    public CiEditSeam edtShowName39 => edtShowName[39];
    /// <summary>edtShowName40（原文 :114）。</summary>
    public CiEditSeam edtShowName40 => edtShowName[40];
    /// <summary>edtShowName41（原文 :115）。</summary>
    public CiEditSeam edtShowName41 => edtShowName[41];
    /// <summary>edtShowName42（原文 :116）。</summary>
    public CiEditSeam edtShowName42 => edtShowName[42];
    /// <summary>edtShowName43（原文 :117）。</summary>
    public CiEditSeam edtShowName43 => edtShowName[43];
    /// <summary>edtShowName44（原文 :118）。</summary>
    public CiEditSeam edtShowName44 => edtShowName[44];
    /// <summary>edtShowName45（原文 :119）。</summary>
    public CiEditSeam edtShowName45 => edtShowName[45];
    /// <summary>edtShowName46（原文 :120）。</summary>
    public CiEditSeam edtShowName46 => edtShowName[46];
    /// <summary>edtShowName47（原文 :121）。</summary>
    public CiEditSeam edtShowName47 => edtShowName[47];
    /// <summary>edtShowName48（原文 :122）。</summary>
    public CiEditSeam edtShowName48 => edtShowName[48];
    /// <summary>edtShowName49（原文 :123）。</summary>
    public CiEditSeam edtShowName49 => edtShowName[49];
    /// <summary>edtShowName50（原文 :124）。</summary>
    public CiEditSeam edtShowName50 => edtShowName[50];
    /// <summary>edtShowName51（原文 :125）。</summary>
    public CiEditSeam edtShowName51 => edtShowName[51];
    /// <summary>edtShowName52（原文 :126）。</summary>
    public CiEditSeam edtShowName52 => edtShowName[52];
    /// <summary>edtShowName53（原文 :127）。</summary>
    public CiEditSeam edtShowName53 => edtShowName[53];
    /// <summary>edtShowName54（原文 :128）。</summary>
    public CiEditSeam edtShowName54 => edtShowName[54];
    /// <summary>edtShowName55（原文 :129）。</summary>
    public CiEditSeam edtShowName55 => edtShowName[55];
    /// <summary>edtShowName56（原文 :130）。</summary>
    public CiEditSeam edtShowName56 => edtShowName[56];
    /// <summary>edtShowName57（原文 :131）。</summary>
    public CiEditSeam edtShowName57 => edtShowName[57];
    /// <summary>edtShowName58（原文 :132）。</summary>
    public CiEditSeam edtShowName58 => edtShowName[58];
    /// <summary>edtShowName59（原文 :133）。</summary>
    public CiEditSeam edtShowName59 => edtShowName[59];
    /// <summary>edtShowName60（原文 :134）。</summary>
    public CiEditSeam edtShowName60 => edtShowName[60];

    /// <summary>pnlBottom1: TPanel（原文 :135）。</summary>
    public CiPanelSeam pnlBottom1 = new();
    /// <summary>lbl1: TLabel（原文 :136）。</summary>
    public CiLabelSeam lbl1 = new();
    /// <summary>btnOK: TButton（原文 :137）。</summary>
    public CiButtonSeam btnOK = new();
    /// <summary>pnlBottom2: TPanel（原文 :138）。</summary>
    public CiPanelSeam pnlBottom2 = new();
    /// <summary>lbl2: TLabel（原文 :139）。</summary>
    public CiLabelSeam lbl2 = new();
    /// <summary>btnOK2: TButton（原文 :140）。</summary>
    public CiButtonSeam btnOK2 = new();
    /// <summary>mmoVar: TMemo（原文 :141）。</summary>
    public CiMemoSeam mmoVar = new();
    /// <summary>lblLineNum: TLabel（原文 :142）。</summary>
    public CiLabelSeam lblLineNum = new();

    // ------------------------------------------------------------------
    // 构造
    // ------------------------------------------------------------------

    /// <summary>
    /// 等价原文 :170 <c>TFrmCustomItemProperty.Create(nil)</c>：VCL 构造过程中 DFM 的
    /// <c>OnCreate = FormCreate</c>（DFM:17）会触发 <see cref="FormCreate"/>。托管侧照抄该顺序：
    /// 建控件 → 布线（DFM 事件表）→ 触发 self.OnCreate。
    /// </summary>
    public TFrmCustomItemProperty()
    {
        for (int i = 1; i <= Grobal2Const.CUSTOM_PROPERTY_BIND_TYPE_COUNT; i++)
        {
            chk[i] = new CiCheckBoxSeam { Tag = i };
            edtShowName[i] = new CiEditSeam { Tag = i };
        }
        WireDfmEvents();
        self.OnCreate?.Invoke();   // DFM:17 OnCreate = FormCreate
    }

    /// <summary>
    /// uFrmCustomItemProperty.dfm 的事件布线 1:1（原文 .pas 只有 chk01Click / edtShowName01Change
    /// 两个控件事件名，DFM 里 60 个 TCheckBox 与 60 个 TEdit **全部**指向它们）。
    /// </summary>
    public void WireDfmEvents()
    {
        self.OnCreate = FormCreate;                                         // DFM:17
        for (int i = 1; i <= Grobal2Const.CUSTOM_PROPERTY_BIND_TYPE_COUNT; i++)
        {
            chk[i].OnClick = chk01Click;                                    // DFM:44,54,...,634（60 处）
            edtShowName[i].OnChange = edtShowName01Change;                   // DFM:644,654,...,1255（60 处）
        }
        btnOK.OnClick = btnOKClick;                                          // DFM:1266
        btnOK2.OnClick = btnOK2Click;                                        // DFM:1315
        mmoVar.OnChange = mmoVarChange;                                      // DFM:1326
        mmoVar.OnKeyUp = () => mmoVarKeyUp();                                // DFM:1327
        mmoVar.OnMouseDown = () => mmoVarMouseDown();                        // DFM:1328
    }

    // ------------------------------------------------------------------
    // 单元级过程（原文 :159 声明、:165-176 实现）
    // ------------------------------------------------------------------

    /// <summary>
    /// 原文 <c>procedure ShowFrmCustomItemProperty;</c>（:165-176）1:1：
    /// <c>if not boStartReady then Exit</c> → Create(nil) → try ShowModal finally Free。
    /// </summary>
    public static void ShowFrmCustomItemProperty()
    {
        if (!CustomItemPropertyGlobals.boStartReady)
            return;                                                     // :169 Exit（★ 是"返回"，不是空过程）
        var FrmCustomItemProperty = new TFrmCustomItemProperty();      // :170
        try
        {
            FrmCustomItemProperty.ShowModal();                          // :172
        }
        finally
        {
            // :174 FrmCustomItemProperty.Free —— 托管侧由 GC 接管（等价 Dispose）
        }
    }

    /// <summary>原文 :172 <c>ShowModal</c>（无头接缝）。</summary>
    public void ShowModal() => CustomItemPropertyMessageBoxSeam.ShowModal();

    // ------------------------------------------------------------------
    // FormCreate（原文 :178-306）
    // ------------------------------------------------------------------

    /// <summary>原文 <c>TFrmCustomItemProperty.FormCreate(Sender: TObject)</c>（:178-306）1:1。
    /// <para>Sender 在原文体内**未被使用**，故托管侧不收该形参。</para></summary>
    public void FormCreate()
    {
        chk01.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[01];   // :180
        chk02.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[02];   // :181
        chk03.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[03];   // :182
        chk04.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[04];   // :183
        chk05.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[05];   // :184
        chk06.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[06];   // :185
        chk07.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[07];   // :186
        chk08.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[08];   // :187
        chk09.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[09];   // :188
        chk10.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[10];   // :189
        chk11.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[11];   // :190
        chk12.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[12];   // :191
        chk13.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[13];   // :192
        chk14.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[14];   // :193
        chk15.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[15];   // :194
        chk16.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[16];   // :195
        chk17.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[17];   // :196
        chk18.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[18];   // :197
        chk19.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[19];   // :198
        chk20.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[20];   // :199
        chk21.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[21];   // :200
        chk22.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[22];   // :201
        chk23.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[23];   // :202
        chk24.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[24];   // :203
        chk25.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[25];   // :204
        chk26.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[26];   // :205
        chk27.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[27];   // :206
        chk28.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[28];   // :207
        chk29.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[29];   // :208
        chk30.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[30];   // :209
        chk31.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[31];   // :210
        chk32.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[32];   // :211
        chk33.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[33];   // :212
        chk34.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[34];   // :213
        chk35.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[35];   // :214
        chk36.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[36];   // :215
        chk37.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[37];   // :216
        chk38.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[38];   // :217
        chk39.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[39];   // :218
        chk40.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[40];   // :219
        chk41.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[41];   // :220
        chk42.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[42];   // :221
        chk43.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[43];   // :222
        chk44.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[44];   // :223
        chk45.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[45];   // :224
        chk46.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[46];   // :225
        chk47.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[47];   // :226
        chk48.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[48];   // :227
        chk49.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[49];   // :228
        chk50.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[50];   // :229
        chk51.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[51];   // :230
        chk52.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[52];   // :231
        chk53.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[53];   // :232
        chk54.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[54];   // :233
        chk55.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[55];   // :234
        chk56.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[56];   // :235
        chk57.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[57];   // :236
        chk58.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[58];   // :237
        chk59.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[59];   // :238
        chk60.Checked = CustomItemPropertyGlobals.g_CustomItemPropertyChecks[60];   // :239

        edtShowName01.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[01];  // :241
        edtShowName02.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[02];  // :242
        edtShowName03.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[03];  // :243
        edtShowName04.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[04];  // :244
        edtShowName05.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[05];  // :245
        edtShowName06.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[06];  // :246
        edtShowName07.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[07];  // :247
        edtShowName08.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[08];  // :248
        edtShowName09.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[09];  // :249
        edtShowName10.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[10];  // :250
        edtShowName11.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[11];  // :251
        edtShowName12.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[12];  // :252
        edtShowName13.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[13];  // :253
        edtShowName14.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[14];  // :254
        edtShowName15.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[15];  // :255
        edtShowName16.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[16];  // :256
        edtShowName17.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[17];  // :257
        edtShowName18.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[18];  // :258
        edtShowName19.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[19];  // :259
        edtShowName20.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[20];  // :260
        edtShowName21.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[21];  // :261
        edtShowName22.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[22];  // :262
        edtShowName23.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[23];  // :263
        edtShowName24.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[24];  // :264
        edtShowName25.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[25];  // :265
        edtShowName26.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[26];  // :266
        edtShowName27.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[27];  // :267
        edtShowName28.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[28];  // :268
        edtShowName29.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[29];  // :269
        edtShowName30.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[30];  // :270
        edtShowName31.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[31];  // :271
        edtShowName32.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[32];  // :272
        edtShowName33.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[33];  // :273
        edtShowName34.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[34];  // :274
        edtShowName35.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[35];  // :275
        edtShowName36.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[36];  // :276
        edtShowName37.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[37];  // :277
        edtShowName38.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[38];  // :278
        edtShowName39.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[39];  // :279
        edtShowName40.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[40];  // :280
        edtShowName41.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[41];  // :281
        edtShowName42.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[42];  // :282
        edtShowName43.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[43];  // :283
        edtShowName44.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[44];  // :284
        edtShowName45.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[45];  // :285
        edtShowName46.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[46];  // :286
        edtShowName47.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[47];  // :287
        edtShowName48.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[48];  // :288
        edtShowName49.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[49];  // :289
        edtShowName50.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[50];  // :290
        edtShowName51.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[51];  // :291
        edtShowName52.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[52];  // :292
        edtShowName53.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[53];  // :293
        edtShowName54.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[54];  // :294
        edtShowName55.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[55];  // :295
        edtShowName56.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[56];  // :296
        edtShowName57.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[57];  // :297
        edtShowName58.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[58];  // :298
        edtShowName59.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[59];  // :299
        edtShowName60.Text = CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[60];  // :300

        btnOK.Enabled = false;                                                              // :302

        mmoVar.Text = CustomItemPropertyLogic.GetTextStr(CustomItemPropertyGlobals.g_CustomItemPropertyTextVarList);  // :304
        btnOK2.Enabled = false;                                                             // :305
    }

    // ------------------------------------------------------------------
    // btnOKClick（原文 :308-450）
    // ------------------------------------------------------------------

    /// <summary>原文 <c>TFrmCustomItemProperty.btnOKClick(Sender: TObject)</c>（:308-450）1:1。
    /// <para>Sender 未使用。局部 <c>OldCrc: LongWord</c> = 托管 <c>uint</c>。</para></summary>
    public void btnOKClick()
    {
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[01] = chk01.Checked;   // :313
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[02] = chk02.Checked;   // :314
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[03] = chk03.Checked;   // :315
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[04] = chk04.Checked;   // :316
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[05] = chk05.Checked;   // :317
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[06] = chk06.Checked;   // :318
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[07] = chk07.Checked;   // :319
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[08] = chk08.Checked;   // :320
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[09] = chk09.Checked;   // :321
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[10] = chk10.Checked;   // :322
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[11] = chk11.Checked;   // :323
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[12] = chk12.Checked;   // :324
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[13] = chk13.Checked;   // :325
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[14] = chk14.Checked;   // :326
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[15] = chk15.Checked;   // :327
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[16] = chk16.Checked;   // :328
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[17] = chk17.Checked;   // :329
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[18] = chk18.Checked;   // :330
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[19] = chk19.Checked;   // :331
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[20] = chk20.Checked;   // :332
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[21] = chk21.Checked;   // :333
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[22] = chk22.Checked;   // :334
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[23] = chk23.Checked;   // :335
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[24] = chk24.Checked;   // :336
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[25] = chk25.Checked;   // :337
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[26] = chk26.Checked;   // :338
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[27] = chk27.Checked;   // :339
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[28] = chk28.Checked;   // :340
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[29] = chk29.Checked;   // :341
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[30] = chk30.Checked;   // :342
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[31] = chk31.Checked;   // :343
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[32] = chk32.Checked;   // :344
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[33] = chk33.Checked;   // :345
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[34] = chk34.Checked;   // :346
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[35] = chk35.Checked;   // :347
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[36] = chk36.Checked;   // :348
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[37] = chk37.Checked;   // :349
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[38] = chk38.Checked;   // :350
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[39] = chk39.Checked;   // :351
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[40] = chk40.Checked;   // :352
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[41] = chk41.Checked;   // :353
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[42] = chk42.Checked;   // :354
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[43] = chk43.Checked;   // :355
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[44] = chk44.Checked;   // :356
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[45] = chk45.Checked;   // :357
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[46] = chk46.Checked;   // :358
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[47] = chk47.Checked;   // :359
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[48] = chk48.Checked;   // :360
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[49] = chk49.Checked;   // :361
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[50] = chk50.Checked;   // :362
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[51] = chk51.Checked;   // :363
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[52] = chk52.Checked;   // :364
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[53] = chk53.Checked;   // :365
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[54] = chk54.Checked;   // :366
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[55] = chk55.Checked;   // :367
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[56] = chk56.Checked;   // :368
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[57] = chk57.Checked;   // :369
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[58] = chk58.Checked;   // :370
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[59] = chk59.Checked;   // :371
        CustomItemPropertyGlobals.g_CustomItemPropertyChecks[60] = chk60.Checked;   // :372

        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[01] = edtShowName01.Text;  // :374
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[02] = edtShowName02.Text;  // :375
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[03] = edtShowName03.Text;  // :376
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[04] = edtShowName04.Text;  // :377
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[05] = edtShowName05.Text;  // :378
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[06] = edtShowName06.Text;  // :379
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[07] = edtShowName07.Text;  // :380
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[08] = edtShowName08.Text;  // :381
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[09] = edtShowName09.Text;  // :382
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[10] = edtShowName10.Text;  // :383
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[11] = edtShowName11.Text;  // :384
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[12] = edtShowName12.Text;  // :385
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[13] = edtShowName13.Text;  // :386
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[14] = edtShowName14.Text;  // :387
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[15] = edtShowName15.Text;  // :388
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[16] = edtShowName16.Text;  // :389
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[17] = edtShowName17.Text;  // :390
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[18] = edtShowName18.Text;  // :391
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[19] = edtShowName19.Text;  // :392
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[20] = edtShowName20.Text;  // :393
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[21] = edtShowName21.Text;  // :394
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[22] = edtShowName22.Text;  // :395
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[23] = edtShowName23.Text;  // :396
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[24] = edtShowName24.Text;  // :397
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[25] = edtShowName25.Text;  // :398
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[26] = edtShowName26.Text;  // :399
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[27] = edtShowName27.Text;  // :400
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[28] = edtShowName28.Text;  // :401
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[29] = edtShowName29.Text;  // :402
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[30] = edtShowName30.Text;  // :403
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[31] = edtShowName31.Text;  // :404
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[32] = edtShowName32.Text;  // :405
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[33] = edtShowName33.Text;  // :406
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[34] = edtShowName34.Text;  // :407
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[35] = edtShowName35.Text;  // :408
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[36] = edtShowName36.Text;  // :409
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[37] = edtShowName37.Text;  // :410
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[38] = edtShowName38.Text;  // :411
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[39] = edtShowName39.Text;  // :412
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[40] = edtShowName40.Text;  // :413
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[41] = edtShowName41.Text;  // :414
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[42] = edtShowName42.Text;  // :415
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[43] = edtShowName43.Text;  // :416
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[44] = edtShowName44.Text;  // :417
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[45] = edtShowName45.Text;  // :418
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[46] = edtShowName46.Text;  // :419
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[47] = edtShowName47.Text;  // :420
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[48] = edtShowName48.Text;  // :421
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[49] = edtShowName49.Text;  // :422
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[50] = edtShowName50.Text;  // :423
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[51] = edtShowName51.Text;  // :424
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[52] = edtShowName52.Text;  // :425
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[53] = edtShowName53.Text;  // :426
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[54] = edtShowName54.Text;  // :427
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[55] = edtShowName55.Text;  // :428
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[56] = edtShowName56.Text;  // :429
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[57] = edtShowName57.Text;  // :430
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[58] = edtShowName58.Text;  // :431
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[59] = edtShowName59.Text;  // :432
        CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[60] = edtShowName60.Text;  // :433

        for (int I = CustomItemPropertyLogic.LowBindType; I <= CustomItemPropertyLogic.HighBindType; I++)  // :435
        {
            Config.WriteBool("Setup", "CustomItemPropertyCheck" + DelphiRTL.IntToStr(I),
                CustomItemPropertyGlobals.g_CustomItemPropertyChecks[I]);                  // :437
            Config.WriteString("Setup", "CustomItemPropertyBindName" + DelphiRTL.IntToStr(I),
                CustomItemPropertyGlobals.g_CustomItemPropertyBindNames[I]);               // :438
        }
        Config.UpdateFile();   // 偏离 D-P8-1：Delphi TIniFile.WriteXxx 立即落盘，TFastIniFile 为内存缓存 → 补一次落盘

        uint OldCrc = CustomItemPropertyGlobals.g_CustomItemPropertyCRC;                   // :441
        CustomItemPropertyGlobals.InvokeRebuildCustomItemPropertyConfig();                 // :442

        if (OldCrc != CustomItemPropertyGlobals.g_CustomItemPropertyCRC)                   // :444
        {
            CustomItemPropertyGlobals.InvokeSendCustomItemPropertyConfig();                // :446
        }

        btnOK.Enabled = false;                                                            // :449
    }

    /// <summary>
    /// 原文 :437/:438 的全局 <c>Config: TIniFile</c>（M2Share.pas 全局，其 initialization 段
    /// <c>Config := TIniFile.Create(g_sSelfFilePath + sConfigFileName)</c>）。
    /// <para>
    /// **接线到既有真实现**（非接缝臆造）：<c>M2ShareState.ConfigIni</c> —— 同一份
    /// <c>!Setup.txt</c> 的托管 TFastIniFile 镜像。Delphi 的 TIniFile.WriteXxx 立即落盘，
    /// 而 TFastIniFile 只在 UpdateFile()/Save() 时落盘，故 btnOKClick 里补了一次 UpdateFile
    /// （偏离 D-P8-1，已在报告登记）。
    /// </para>
    /// </summary>
    private static TFastIniFile Config => M2ShareState.ConfigIni;

    // ------------------------------------------------------------------
    // 事件处理器（原文 :452-496）
    // ------------------------------------------------------------------

    /// <summary>原文 <c>chk01Click(Sender: TObject)</c>（:452-455）1:1。DFM 里 60 个 TCheckBox 全部指向它。</summary>
    public void chk01Click() => btnOK.Enabled = true;

    /// <summary>原文 <c>edtShowName01Change(Sender: TObject)</c>（:457-461）1:1。DFM 里 60 个 TEdit 全部指向它。</summary>
    public void edtShowName01Change() => btnOK.Enabled = true;

    /// <summary>原文 <c>mmoVarChange(Sender: TObject)</c>（:463-466）1:1。</summary>
    public void mmoVarChange() => btnOK2.Enabled = true;

    /// <summary>
    /// 原文 <c>btnOK2Click(Sender: TObject)</c>（:468-484）1:1。
    /// <para>注意顺序：先写 <c>g_CustomItemPropertyTextVarList.Text</c>，再取 OldCrc，再 Save，再比较发送。</para>
    /// </summary>
    public void btnOK2Click()
    {
        CustomItemPropertyLogic.SetTextStr(
            CustomItemPropertyGlobals.g_CustomItemPropertyTextVarList, mmoVar.Text);       // :472

        uint OldCrc = CustomItemPropertyGlobals.g_CustomItemPropertyTextVarListTextCRC;    // :474

        CustomItemPropertyGlobals.InvokeSaveCustomItemPropertyTextVarList();               // :476

        if (OldCrc != CustomItemPropertyGlobals.g_CustomItemPropertyTextVarListTextCRC)    // :478
        {
            CustomItemPropertyGlobals.InvokeSendCustomItemPropertyTextVarList();           // :480
        }

        btnOK2.Enabled = false;                                                           // :483
    }

    /// <summary>
    /// 原文 <c>mmoVarKeyUp(Sender: TObject; var Key: Word; Shift: TShiftState)</c>（:486-490）1:1。
    /// <para>Key/Shift/Sender 在体内**均未被使用**（Key 也未被改写），故托管侧不引入 TShiftState。</para>
    /// </summary>
    public void mmoVarKeyUp(object? sender = null, ushort key = 0, object? shift = null)   // :486
        => lblLineNum.Caption = CustomItemPropertyLogic.LineNumCaption(mmoVar.CaretPosY);  // :489

    /// <summary>
    /// 原文 <c>mmoVarMouseDown(Sender: TObject; Button: TMouseButton; Shift: TShiftState; X, Y: Integer)</c>
    /// （:492-496）1:1。<para>全部形参在体内**未被使用**。</para>
    /// </summary>
    public void mmoVarMouseDown(object? sender = null, int button = 0, object? shift = null, int x = 0, int y = 0)  // :492
        => lblLineNum.Caption = CustomItemPropertyLogic.LineNumCaption(mmoVar.CaretPosY);  // :495
}
