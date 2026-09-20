// ============================================================================
// SellPlayer.pas（Source\M2Engine\SellPlayer.pas，307 行，GBK）1:1 移植
// 车道 p8-m2-itemprop-misc ｜ 命名空间 GXX.M2Server.Misc
//
// 类：TSellPlayerList = class(TObject)（:23-47）
//   构造 :54-59、析构 :61-66、Clear :68-79、GetCount :81-84、GetItems :86-89、
//   AddSellPlayer :91-116、Search :118-145、DeleteByIndex :147-154、DeletePlayer :156-174、
//   DeletePlayerEx :176-191、LoadConfig :193-231、SaveConfig :233-264、AutoLoadSellPlayer :266-305
//   记录：TSellPlayerInfo（:11-21）+ 指针类型 PSellPlayerInfo（:9）
//
// 【1:1 保真要点】
//   * 记录 → 托管 class（引用语义 == 原文 New/Dispose 的指针语义）：FList 在原文是 TList of
//     PSellPlayerInfo（指针容器），托管侧 List<TSellPlayerInfo> 的元素即引用，语义一致。
//   * `string[N]`（ShortString）**按 GBK 字节截断**（不是按字符）：见 SellPlayerShortStr.Trunc
//     与 ShortStr.Set 的既有实现（GXX.Core.Protocol.ShortStr）。
//   * `Search` 的 `I := L + (H - L) shr 1` —— Delphi 里 `shr`（二级）**优先于** `+`（三级），
//     故等价 `L + ((H - L) shr 1)`；C# 里 `>>` 优先级**低于** `+`，必须显式加括号，否则会算成
//     `(L + (H - L)) >> 1`。**这是本单元最容易抄错的一处**（见报告 §5 易错点）。
//   * `AnsiCompareText`（大小写不敏感；工程内既有处置：MonGenLoadCore.AnsiCompareText /
//     DropLimitState.Search 用 StringComparison.OrdinalIgnoreCase 对齐 ASCII 段语义）。
//
// 【接缝】uses SysUtils/Classes/Grobal2/FastIniFile 为已移植件；M2Share/UsrEngn/M2Threads 中
//   本单元用到的全局量见 SellPlayerSeams.cs。
// ============================================================================

using GXX.Core;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.Core.Util;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Misc;

// ---------------------------------------------------------------------------
// TSellPlayerInfo（原文 :11-21）—— 原文为 record，托管侧 class（见文件头说明）
// ---------------------------------------------------------------------------

/// <summary>
/// SellPlayer.pas:11-21 <c>TSellPlayerInfo</c> 1:1。
/// <para>
/// 原文各字符串字段是 <c>string[ACCOUNT_LEN]</c>（10 字节）/ <c>string[ACTOR_NAME_LEN]</c>（14 字节）
/// 的 ShortString：**赋值时按 GBK 字节截断**。托管侧在 setter 里复刻该截断。
/// </para>
/// </summary>
public sealed class TSellPlayerInfo
{
    private string _account = "";
    private string _player = "";
    private string _delegater = "";
    private string _setUserName = "";

    /// <summary>Account: string[ACCOUNT_LEN]（ACCOUNT_LEN = 10，Grobal2.pas:30）。出售帐号。</summary>
    public string Account
    {
        get => _account;
        set => _account = SellPlayerShortStr.Trunc(value, Grobal2Const.ACCOUNT_LEN);
    }

    /// <summary>Player: string[ACTOR_NAME_LEN]（ACTOR_NAME_LEN = 14，Grobal2.pas:29）。出售角色。</summary>
    public string Player
    {
        get => _player;
        set => _player = SellPlayerShortStr.Trunc(value, Grobal2Const.ACTOR_NAME_LEN);
    }

    /// <summary>Delegater: string[ACTOR_NAME_LEN]。委托角色。</summary>
    public string Delegater
    {
        get => _delegater;
        set => _delegater = SellPlayerShortStr.Trunc(value, Grobal2Const.ACTOR_NAME_LEN);
    }

    /// <summary>SellPricesType: Integer。货币类型。</summary>
    public int SellPricesType;

    /// <summary>SellPrices: Integer。出售价格。</summary>
    public int SellPrices;

    /// <summary>SellTime: TDateTime。出售时间（Delphi TDateTime = Double，1899-12-30 起的天数）。</summary>
    public double SellTime;

    /// <summary>SetUser: Boolean。是否指定购买人。</summary>
    public bool SetUser;

    /// <summary>SetUserName: string[ACTOR_NAME_LEN]。指定购买人。</summary>
    public string SetUserName
    {
        get => _setUserName;
        set => _setUserName = SellPlayerShortStr.Trunc(value, Grobal2Const.ACTOR_NAME_LEN);
    }

    /// <summary>IsBuying: Boolean。正在被购买中。</summary>
    public bool IsBuying;
}

/// <summary>
/// <c>string[N]</c>（ShortString）赋值截断助手：**按 GBK 字节截断**，与
/// <see cref="GXX.Core.Protocol.ShortStr.Set"/>+<c>Get</c> 的往返一致。
/// <para>
/// 注意：工程内 p5-m2-custommagic 车道的 <c>CustomMagicShortStr.Trunc</c> 是**按字符**截断
/// （中文字符串下与 Delphi 不同）；本车道按原文（字节）落地，已在报告登记该跨车道差异。
/// </para>
/// </summary>
public static class SellPlayerShortStr
{
    /// <summary>按 GBK 字节容量截断（不抛异常；容量 &lt;= 0 时返回空串）。</summary>
    public static string Trunc(string? value, int capacity)
    {
        if (string.IsNullOrEmpty(value) || capacity <= 0) return "";
        byte[] data = EncodingInit.GBK.GetBytes(value);
        if (data.Length <= capacity) return value;
        return EncodingInit.GBK.GetString(data, 0, capacity);
    }
}

// ---------------------------------------------------------------------------
// TSellPlayerList（原文 :23-47）
// ---------------------------------------------------------------------------

/// <summary>SellPlayer.pas:23-47 <c>TSellPlayerList</c> 1:1（出售角色列表）。</summary>
public sealed class TSellPlayerList
{
    /// <summary>FList: TList（原文 :25；托管侧 List&lt;TSellPlayerInfo&gt;，指针容器等价）。</summary>
    private readonly List<TSellPlayerInfo> FList = new();

    /// <summary>FIniFileName: string（原文 :26）。</summary>
    private readonly string FIniFileName = "";

    /// <summary>原文 <c>constructor TSellPlayerList.Create</c>（:54-59）1:1。</summary>
    public TSellPlayerList()
    {
        FList.Capacity = 300;                                              // :57
        FIniFileName = M2Config.sEnvirDir + "SellPlayer.ini";              // :58 g_Config.sEnvirDir + 'SellPlayer.ini'
    }

    // 原文 destructor Destroy（:61-66）：Clear; FList.Free; inherited —— 托管侧由 GC 接管。
    // 语义等价点：Clear 会先释放全部 TSellPlayerInfo（托管侧为引用置空）。

    /// <summary>原文 <c>procedure TSellPlayerList.Clear</c>（:68-79）1:1：逐项 Dispose 后清表。</summary>
    public void Clear()
    {
        for (int I = 0; I <= FList.Count - 1; I++)                          // :73
        {
            _ = FList[I];                                                   // :75 Info := FList.Items[I]
            // :76 Dispose(Info) —— 托管侧无显式释放（引用语义由 GC 接管，等价于指针不再被持有）
        }
        FList.Clear();                                                      // :78
    }

    /// <summary>原文 <c>function GetCount: Integer</c>（:81-84）→ <c>property Count</c>。</summary>
    public int Count => FList.Count;

    /// <summary>
    /// 原文 <c>property Items[Index: Integer]: PSellPlayerInfo read GetItems</c>（:39、:86-89）。
    /// <para>
    /// ★ 托管侧**越界即抛** <see cref="ArgumentOutOfRangeException"/>；Delphi 未开范围检查时
    /// TList.Items[] 越界读的是垃圾指针（不抛）。该差异无法在托管侧复刻，已登记偏离 D-P8-2。
    /// </para>
    /// </summary>
    public TSellPlayerInfo this[int Index]
    {
        get => FList[Index];
    }

    /// <summary>原文 <c>function GetItems(Index: Integer): PSellPlayerInfo</c>（:86-89）。</summary>
    public TSellPlayerInfo GetItems(int Index) => FList[Index];

    /// <summary>
    /// 原文 <c>function Search(Player: string; var Index: Integer): Boolean</c>（:118-145）1:1。
    /// <para>
    /// 二分查找；命中时 <paramref name="Index"/> 为命中项下标，未命中时为**插入位置**。
    /// </para>
    /// <para>
    /// ★ 原文 <c>I := L + (H - L) shr 1</c>（:129）：Delphi 中 <c>shr</c> 先于 <c>+</c>，
    /// 故为 <c>L + ((H - L) shr 1)</c>；C# 中 <c>&gt;&gt;</c> 后于 <c>+</c>，故此处显式加括号。
    /// </para>
    /// <para>比较用 <c>AnsiCompareText</c>（大小写不敏感）—— 托管侧 OrdinalIgnoreCase 对齐。</para>
    /// </summary>
    public bool Search(string Player, out int Index)
    {
        bool Result = false;                                                // :123

        int L = 0;                                                          // :125
        int H = FList.Count - 1;                                            // :126
        while (L <= H)                                                      // :127
        {
            int I = L + ((H - L) >> 1);                                     // :129 原文 L + (H - L) shr 1
            TSellPlayerInfo Info = FList[I];                                // :130
            int C = string.Compare(Info.Player, Player, StringComparison.OrdinalIgnoreCase);  // :131 AnsiCompareText
            if (C < 0)                                                      // :132
                L = I + 1;                                                  // :133
            else
            {
                H = I - 1;                                                  // :136
                if (C == 0)                                                 // :137
                {
                    Result = true;                                          // :139
                    L = I;                                                  // :140
                }
            }
        }
        Index = L;                                                          // :144
        return Result;
    }

    /// <summary>
    /// 原文 <c>function AddSellPlayer(Account, Player, Delegater: string; SellPricesType, SellPrices: Integer;
    /// SellTime: TDateTime; IsSetUser: Boolean; SetUserName: string): Boolean</c>（:91-116）1:1。
    /// <para>重名（Search 命中）则返回 False 且不插入；否则按 Search 给出的插入位插入并返回 True。</para>
    /// </summary>
    public bool AddSellPlayer(string Account, string Player, string Delegater, int SellPricesType,
        int SellPrices, double SellTime, bool IsSetUser, string SetUserName)
    {
        bool Result = !Search(Player, out int Index);                       // :97

        if (Result)                                                         // :99
        {
            var Info = new TSellPlayerInfo();                               // :101 New(Info)
            // :102 FillChar(Info^, SizeOf(TSellPlayerInfo), 0) —— 托管侧由字段初值（""/0/false）等价给出

            Info.Account = Account;                                         // :104
            Info.Player = Player;                                           // :105
            Info.Delegater = Delegater;                                     // :106
            Info.SellPrices = SellPrices;                                   // :107
            Info.SellPricesType = SellPricesType;                           // :108
            Info.SellTime = SellTime;                                       // :109
            Info.SetUser = IsSetUser;                                       // :110
            Info.SetUserName = SetUserName;                                 // :111
            Info.IsBuying = false;                                          // :112

            FList.Insert(Index, Info);                                      // :114
        }
        return Result;
    }

    /// <summary>原文 <c>procedure DeleteByIndex(Index: Integer)</c>（:147-154）1:1（越界语义同索引器，见 D-P8-2）。</summary>
    public void DeleteByIndex(int Index)
    {
        _ = FList[Index];                                                   // :151 Info := FList.Items[Index]
        // :152 Dispose(Info)
        FList.RemoveAt(Index);                                              // :153 FList.Delete(Index)
    }

    /// <summary>
    /// 原文 <c>function DeletePlayer(Player, Delegater: string): Boolean</c>（:156-174）1:1。
    /// <para>委托角色必须 <c>SameText</c>（大小写不敏感）相等才删除；删成功即 <c>SaveConfig</c>。</para>
    /// </summary>
    public bool DeletePlayer(string Player, string Delegater)
    {
        bool Result = false;                                                // :161
        if (Search(Player, out int Index))                                  // :162
        {
            TSellPlayerInfo Info = FList[Index];                            // :164
            if (string.Equals(Info.Delegater, Delegater, StringComparison.OrdinalIgnoreCase))  // :165 SameText
            {
                // :167 Dispose(Info)
                FList.RemoveAt(Index);                                      // :168
                Result = true;                                              // :169

                SaveConfig();                                               // :171
            }
        }
        return Result;
    }

    /// <summary>原文 <c>function DeletePlayerEx(Player: string): Boolean</c>（:176-191）1:1（不看委托角色）。</summary>
    public bool DeletePlayerEx(string Player)
    {
        bool Result = false;                                                // :181
        if (Search(Player, out int Index))                                  // :182
        {
            _ = FList[Index];                                               // :184 Info := FList.Items[Index]
            // :185 Dispose(Info)
            FList.RemoveAt(Index);                                          // :186
            Result = true;                                                  // :187

            SaveConfig();                                                   // :189
        }
        return Result;
    }

    /// <summary>
    /// 原文 <c>procedure LoadConfig</c>（:193-231）1:1。
    /// <para>
    /// 逐节读 setup/count 与 1..Count 节；校验
    /// <c>(Player &lt;&gt; '') and (Delegater &lt;&gt; '') and (MoneyType &gt;= 0) and (MoneyType &lt;= 4) and (SellPrices &gt; 0)</c>；
    /// <c>SetUserName = ''</c> 时强制 <c>boSetUser := False</c>。
    /// </para>
    /// </summary>
    public void LoadConfig()
    {
        Clear();                                                            // :202
        if (File.Exists(FIniFileName))                                       // :203
        {
            var IniFile = new TFastIniFile(FIniFileName);                    // :205
            try
            {
                int Count = IniFile.ReadInteger("setup", "count", 0);        // :207
                for (int I = 0; I <= Count - 1; I++)                        // :208
                {
                    string Section = DelphiRTL.IntToStr(I + 1);             // :210
                    string Account = IniFile.ReadString(Section, "Account", "");        // :211
                    string Player = IniFile.ReadString(Section, "Player", "");          // :212
                    string Delegater = IniFile.ReadString(Section, "Delegater", "");    // :213
                    int MoneyType = IniFile.ReadInteger(Section, "MoneyType", 0);       // :214
                    int SellPrices = IniFile.ReadInteger(Section, "SellPrices", 0);     // :215
                    double SellTime = SellPlayerIni.ReadFixedDateTime(IniFile, Section, "SellTime", 0);  // :216
                    bool boSetUser = IniFile.ReadBool(Section, "SetUser", false);       // :217 ReadBoolean

                    string SetUserName = IniFile.ReadString(Section, "SetUserName", ""); // :218

                    if ((Player != "") && (Delegater != "") && (MoneyType >= 0) && (MoneyType <= 4) && (SellPrices > 0))  // :220
                    {
                        if (SetUserName == "")                              // :222
                            boSetUser = false;                              // :223
                        AddSellPlayer(Account, Player, Delegater, MoneyType, SellPrices, SellTime, boSetUser, SetUserName);  // :224
                    }
                }
            }
            finally
            {
                IniFile.Dispose();                                          // :228 IniFile.Free
            }
        }
    }

    /// <summary>
    /// 原文 <c>procedure SaveConfig</c>（:233-264）1:1。
    /// <para>
    /// ★ 原文 <c>// IniFile.Clear;</c> 被注释掉（:243）→ **旧节不会删除**，列表收缩后文件里会留下
    /// 陈旧节（原文瑕疵，此处逐字保留，见报告 §5）。
    /// </para>
    /// <para>
    /// ★ 原文把全部写操作包在 <c>try ... finally IniFile.Free end; except end</c>：
    /// 外层裸 <c>except</c> 吞掉**一切**异常（含 Free 抛出的）。此处照抄为 <c>catch { }</c>。
    /// </para>
    /// <para>
    /// ★ Delphi <c>TFastIniFile.Destroy</c> 会 <c>FlushBuffers</c>（FastIniFile.pas:2077-2085）落盘；
    /// 托管 <c>TFastIniFile.Dispose()</c> 是空实现 → 此处显式 <c>UpdateFile()</c> 对齐（偏离 D-P8-3，
    /// 并已向 GXX.Core 提越区请求）。
    /// </para>
    /// </summary>
    public void SaveConfig()
    {
        var IniFile = new TFastIniFile(FIniFileName);                        // :240
        try
        {
            try
            {
                // IniFile.Clear;                                            // :243 原文即被注释掉
                IniFile.WriteInteger("setup", "count", FList.Count);         // :244
                for (int I = 0; I <= FList.Count - 1; I++)                  // :245
                {
                    TSellPlayerInfo Info = FList[I];                        // :247

                    string Section = DelphiRTL.IntToStr(I + 1);             // :249
                    IniFile.WriteString(Section, "Account", Info.Account);              // :250
                    IniFile.WriteString(Section, "Player", Info.Player);                // :251
                    IniFile.WriteString(Section, "Delegater", Info.Delegater);          // :252
                    IniFile.WriteInteger(Section, "MoneyType", Info.SellPricesType);    // :253
                    IniFile.WriteInteger(Section, "SellPrices", Info.SellPrices);       // :254
                    SellPlayerIni.WriteFixedDateTime(IniFile, Section, "SellTime", Info.SellTime);  // :255
                    IniFile.WriteBool(Section, "SetUser", Info.SetUser);                // :256 WriteBoolean
                    IniFile.WriteString(Section, "SetUserName", Info.SetUserName);      // :257
                }
            }
            finally
            {
                IniFile.UpdateFile();                                       // :260 IniFile.Free → FlushBuffers（见 D-P8-3）
                IniFile.Dispose();                                          // :260
            }
        }
        catch
        {
            // :262-263 原文裸 except（无语句）—— 吞掉全部异常
        }
    }

    /// <summary>
    /// 原文 <c>procedure AutoLoadSellPlayer</c>（:266-305）1:1。
    /// <para>
    /// 列表非空时：清 <c>m_boStartAutoLoadSellPlayer</c> → （多线程时加锁）把每项打包成
    /// <c>pTOffLineData</c> 追加到 <c>UserEngine.m_AutoLoadSellPlayerList</c> →
    /// 按结果列表是否非空回写标志 → 非空则 <c>MainOutMessage('正在登录出售角色...')</c>。
    /// </para>
    /// <para>
    /// 原文 <c>{$IF MULTI_THREAD = 1}</c> 的 LockW(2)/UnLockW 在托管侧照样调用（<c>g_MultiThreadRun</c>
    /// 为假时不调用），并把调用序列记录在接缝里供断言。
    /// </para>
    /// </summary>
    public void AutoLoadSellPlayer()
    {
        if (FList.Count > 0)                                                // :272
        {
            SellPlayerGlobals.m_boStartAutoLoadSellPlayer = false;           // :274

            bool locked = false;
            if (SellPlayerGlobals.g_MultiThreadRun)                          // :277
            {
                SellPlayerGlobals.LockWAutoLoadSellPlayerList(2);            // :278 LockW(2)
                locked = true;
            }
            try
            {
                for (int I = 0; I <= FList.Count - 1; I++)                  // :281
                {
                    TSellPlayerInfo Info = FList[I];                        // :283

                    var OffLineData = new TOffLineData();                   // :285 New(OffLineData)
                    OffLineData.sAccount = Info.Account;                    // :286
                    OffLineData.sCharName = Info.Player;                    // :287
                    OffLineData.boStartLogin = false;                       // :288
                    OffLineData.dwStartLoginTick = SellPlayerGlobals.MyGetTickCount();  // :289 MyGetTickCount
                    OffLineData.SessInfo = null;                            // :290

                    SellPlayerGlobals.m_AutoLoadSellPlayerList.Add(OffLineData);  // :292
                }
            }
            finally
            {
                if (locked)                                                 // :296
                    SellPlayerGlobals.UnLockWAutoLoadSellPlayerList();      // :297 UnLockW
            }

            SellPlayerGlobals.m_boStartAutoLoadSellPlayer =                 // :300
                SellPlayerGlobals.m_AutoLoadSellPlayerList.Count > 0;

            if (SellPlayerGlobals.m_boStartAutoLoadSellPlayer)              // :302
                M2ServerLog.MainOutMessage("正在登录出售角色...");           // :303
        }
    }
}
