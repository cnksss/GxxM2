// ============================================================================
// 源单元：Source/M2Engine/NoticeM.pas（119 行，GBK）
// 类型：TNoticeMsg（:9-13 record）、TNoticeList（:14 array[0..99]）、
//       TNoticeManager（:15-23 声明；:32-42 ctor、:44-54 dtor、:56-75 LoadingNotice、:77-118 GetNoticeMsg）
//
// 原文 uses：
//   接口段（:5-6） Windows, Classes, SysUtils, StringListHelper
//   实现段（:26）   M2Share
//   ⇒ 依赖面：TStringList（Classes）、FileExists（SysUtils）、
//     CompareText（SysUtils）、MainOutMessage + g_Config.sNoticeDir（M2Share）。
//
// 依赖处置（不臆造替身，见 docs\并行报告-p9-m2-forms.md §0.4）：
//   · TStringList → 既有 GXX.Core.Util.TStringList（原文 `TStrings.AddStrings` 在托管
//     TStringList 上**没有**对应成员，故在本文件以扩展方法补齐，语义 = Delphi
//     `AddStrings`（逐项 AddObject，携带 Objects））。
//   · FileExists  → 既有 GXX.M2Server.Sweep.SweepSeam.FileExists（非本车道新增）。
//   · MainOutMessage → 既有 GXX.M2Server.Sweep.SweepSeam.MainOutMessage。
//   · g_Config.sNoticeDir → 既有 GXX.M2Server.Engine.M2Config.sNoticeDir（M2Share.pas 已落地部分）。
//   · CompareText → 本车道 Sweep9FormsKit.CompareText（ASCII 大写表口径，见文件头注释）。
//
// ★ 原文缺陷（逐字保留 + 差异断言锁定，见 tests\Sweep9FormsNoticeMTests.cs）：
//   1. `:87-95` 命中循环**不 Break**：同一 `sStr` 若已登记两份（大小写不同也一样命中），
//      两份 sList 会被**全部**追加（而非只取第一份）。
//   2. `:89-93` 命中项 `sList = nil` 时：Result 保持 False、bo15 置 False ⇒
//      `:97 if not bo15 then Exit` **直接返回 False**，既不加内容也**不再走"新登记"分支**
//      （即"已登记名字但列表为 nil"会把该名字**永久卡死**在未加载状态）。
//   3. `:105-111` 的 `try/except` 只包住"建列表 + LoadFromFile + AddStrings"，
//      而 `:112 NoticeList[n14].sMsg := sStr` 与 `:113 Result := True` **在 except 之外**
//      ⇒ 文件存在但加载抛错时，名字照样登记、Result 照样为 True（内容为空）。
//   4. `:36-41` 构造把 `bo0C` 全置 True，但**全单元无任何地方读取/写入 `bo0C`**
//      ⇒ 该字段是死字段（照抄保留）。
// ============================================================================

using GXX.Core.Util;
using GXX.M2Server.Engine;
using GXX.M2Server.Sweep;

namespace GXX.M2Server.Sweep9.Forms;

/// <summary>
/// 原文 `NoticeM.pas:9-13 TNoticeMsg = record`（注释 `// 0x0C 00491BE9`）。
/// <para>用 struct 保留原文的**值语义**（`NoticeList` 是定长数组 ⇒ 逐元素原地读写）。</para>
/// </summary>
public struct TNoticeMsg
{
    /// <summary>原文 `sMsg: string`。</summary>
    public string sMsg;

    /// <summary>原文 `sList: TStringList`（可空，对应原文 `nil` 初值）。</summary>
    public TStringList? sList;

    /// <summary>原文 `bo0C: Boolean` —— **死字段**：全单元 0 处读取/写入（构造置 True 后即不再触及）。</summary>
    public bool bo0C;
}

/// <summary>
/// 原文 `NoticeM.pas:14 TNoticeList = array[0..99] of TNoticeMsg`。
/// <para>
/// Delphi 数组下界为 0（`Low=0`/`High=99`）⇒ 托管侧 `TNoticeMsg[100]` **下标不需要偏移**，
/// 与 `.pas` 里的 `n14`/`I` 逐字对应。
/// </para>
/// </summary>
public sealed class TNoticeList
{
    /// <summary>原文 `Low(NoticeList)` == 0。</summary>
    public const int Low = 0;
    /// <summary>原文 `High(NoticeList)` == 99。</summary>
    public const int High = 99;

    private readonly TNoticeMsg[] _items = new TNoticeMsg[High - Low + 1];

    /// <summary>原文 `NoticeList[Index]`（可写元素，值语义原地生效）。</summary>
    public ref TNoticeMsg this[int index] => ref _items[index - Low];
}

/// <summary>
/// 原文 `NoticeM.pas:15-23 TNoticeManager = class`（1:1）。
/// </summary>
public sealed class TNoticeManager : IDisposable
{
    /// <summary>原文 `NoticeM.pas:17 NoticeList: TNoticeList`（private）。</summary>
    public readonly TNoticeList NoticeList = new();

    /// <summary>原文 `:32-42 constructor TNoticeManager.Create`。</summary>
    public TNoticeManager()
    {
        // 原文 for I := Low(NoticeList) to High(NoticeList) do
        for (int I = TNoticeList.Low; I <= TNoticeList.High; I++)
        {
            NoticeList[I].sMsg = "";
            NoticeList[I].sList = null;      // 原文 sList := nil
            NoticeList[I].bo0C = true;
        }
    }

    /// <summary>
    /// 原文 `:44-54 destructor TNoticeManager.Destroy`。
    /// 托管侧实现 <see cref="IDisposable.Dispose"/>（原文 `Free` 的等价入口）；
    /// `inherited` 在托管侧无对应（基类是 Object）。
    /// </summary>
    public void Dispose()
    {
        // 原文 for I := Low(NoticeList) to High(NoticeList) do
        for (int I = TNoticeList.Low; I <= TNoticeList.High; I++)
        {
            // 原文 if NoticeList[I].sList <> nil then NoticeList[I].sList.Free;
            // 托管侧 TStringList 由 GC 回收 ⇒ 置空 = 释放引用（语义等价）。
            NoticeList[I].sList = null;
        }
        // 原文 inherited;
    }

    /// <summary>
    /// 原文 `:56-75 procedure TNoticeManager.LoadingNotice()`（`// 00491D54`）。
    /// </summary>
    public void LoadingNotice()
    {
        // 原文 var sFileName: string; I: Integer;
        string sFileName;

        // 原文 for I := Low(NoticeList) to High(NoticeList) do
        for (int I = TNoticeList.Low; I <= TNoticeList.High; I++)
        {
            // 原文 if NoticeList[I].sMsg = '' then Continue;
            if (NoticeList[I].sMsg == "")
                continue;

            // 原文 sFileName := g_Config.sNoticeDir + NoticeList[I].sMsg + '.txt';
            sFileName = M2Config.sNoticeDir + NoticeList[I].sMsg + ".txt";

            // 原文 if FileExists(sFileName) then
            if (SweepSeam.FileExists(sFileName))
            {
                // 原文 try ... except MainOutMessage('Error in loading notice text. file name is ' + sFileName); end;
                try
                {
                    // 原文 if NoticeList[I].sList = nil then NoticeList[I].sList := TStringList.Create;
                    NoticeList[I].sList ??= new TStringList();
                    // 原文 NoticeList[I].sList.LoadFromFile(sFileName);
                    NoticeList[I].sList!.LoadFromFile(sFileName);
                }
                catch
                {
                    // 原文的 except 是**裸 except**（吞掉一切异常），逐字保留。
                    SweepSeam.MainOutMessage("Error in loading notice text. file name is " + sFileName);
                }
            }
        }
    }

    /// <summary>
    /// 原文 `:77-118 function TNoticeManager.GetNoticeMsg(sStr: string; LoadList: TStringList): Boolean`
    /// （`// 00491EA0`）。
    /// </summary>
    public bool GetNoticeMsg(string sStr, TStringList LoadList)
    {
        // 原文 var bo15: Boolean; n14: Integer; sFileName: string;
        bool bo15;
        int n14;
        string sFileName;

        bool Result = false;              // 原文 Result := False;（:83）
        bo15 = true;                      // 原文 bo15 := True;（:84）

        // 原文 for n14 := Low(NoticeList) to High(NoticeList) do（:85-96）
        for (n14 = TNoticeList.Low; n14 <= TNoticeList.High; n14++)
        {
            // 原文 if CompareText(NoticeList[n14].sMsg, sStr) = 0 then
            if (Sweep9FormsKit.CompareText(NoticeList[n14].sMsg, sStr) == 0)
            {
                // 原文 if NoticeList[n14].sList <> nil then
                if (NoticeList[n14].sList != null)
                {
                    // 原文 LoadList.AddStrings(NoticeList[n14].sList); —— 见本文件扩展方法
                    LoadList.AddStrings(NoticeList[n14].sList!);
                    Result = true;
                }
                bo15 = false;             // 原文 bo15 := False;（:94，**在 if 之外**）
            }
            // ★ 原文缺陷 1（逐字保留）：此处**没有 Break** —— 命中后继续扫完 100 项。
        }                                 // 原文 :96 行尾注释 `// while`（原文如此）

        // 原文 if not bo15 then Exit;（:97）
        if (!bo15)
            return Result;                // 原文 Exit（保留当时的 Result 值）

        // 原文 for n14 := Low(NoticeList) to High(NoticeList) do（:98-117）
        for (n14 = TNoticeList.Low; n14 <= TNoticeList.High; n14++)
        {
            // 原文 if NoticeList[n14].sMsg = '' then
            if (NoticeList[n14].sMsg == "")
            {
                // 原文 sFileName := g_Config.sNoticeDir + sStr + '.txt';
                sFileName = M2Config.sNoticeDir + sStr + ".txt";

                // 原文 if FileExists(sFileName) then
                if (SweepSeam.FileExists(sFileName))
                {
                    // 原文 try ... except MainOutMessage(...); end;（:105-111）
                    try
                    {
                        NoticeList[n14].sList ??= new TStringList();
                        NoticeList[n14].sList!.LoadFromFile(sFileName);
                        LoadList.AddStrings(NoticeList[n14].sList!);
                    }
                    catch
                    {
                        SweepSeam.MainOutMessage("Error in loading notice text. file name is " + sFileName);
                    }
                    // ★ 原文缺陷 3：下面两句**在 except 之外** ⇒ 加载抛错也照样登记并返回 True。
                    NoticeList[n14].sMsg = sStr;      // 原文 :112
                    Result = true;                    // 原文 :113
                    break;                            // 原文 :114 Break;
                }
            }
        }
        return Result;                    // 原文 fall-through 到函数尾（:118 end.）
    }
}

/// <summary>
/// `TStrings.AddStrings` 的托管补齐（Delphi 语义：逐项 `AddObject(Strings[I], Strings.Objects[I])`）。
/// <para>
/// 放在本车道文件里是因为 `GXX.Core.Util.TStringList`（常驻共享区）**没有**该成员，
/// 而本车道无权修改它（分区纪律）。调用形态与原文逐字一致：`LoadList.AddStrings(other)`。
/// </para>
/// </summary>
public static class Sweep9FormsStringListExtensions
{
    /// <summary>Delphi `procedure TStrings.AddStrings(const Strings: TStrings)`。</summary>
    public static void AddStrings(this TStringList target, TStringList source)
    {
        for (int i = 0; i < source.Count; i++)
            target.AddObject(source[i], source.GetObject(i));
    }
}

/// <summary>
/// 原文 `M2Share.pas:3685 NoticeManager: TNoticeManager; // 0x004EBB9C`（全局单例）。
/// <para>
/// 创建点 `svMain.pas:1945 NoticeManager := TNoticeManager.Create;`、释放点 `:2154`；
/// 使用点 `UsrEngn.pas:4459 NoticeManager.LoadingNotice`、`ObjPlayer.pas:8215
/// NoticeManager.GetNoticeMsg('Notice', LoadList)`。
/// </para>
/// <para>
/// **接缝**：`M2Share.pas` 批次落地前，该全局在托管侧不存在；此处按其原文声明形态提供字段，
/// **不给默认实例**（原文在 `svMain` 创建前也是 nil）⇒ 未接线时保持 `null`，调用方自行判空，
/// 不臆造中性替身（《并行派发台账》§25.2）。
/// </para>
/// </summary>
public static class Sweep9FormsNoticeGlobals
{
    /// <summary>原文 `M2Share.pas:3685 NoticeManager: TNoticeManager`（未接线时为 <c>null</c> == 原文 nil）。</summary>
    public static TNoticeManager? NoticeManager;

    /// <summary>测试隔离。</summary>
    public static void Reset() => NoticeManager = null;
}
