// =====================================================================================
// 源单元：Source\RunGate\MagicIntervalUtils.pas（Delphi 7，GBK）
//   实测 LF = 198 行（[System.IO.File]::ReadAllBytes → 数 '\n'；任务书给 199，含末行）
//   本文件 1:1 覆盖：全文件 :1-198（interface :1-40 / implementation :42-198）
//
// 抽取/回读流程（非手工转录）：
//   $b=[IO.File]::ReadAllBytes('Source\RunGate\MagicIntervalUtils.pas')
//   $t=[Text.Encoding]::GetEncoding('GBK').GetString($b) -replace "`r`n","`n"
//   （抽取命令原写作数字代码页形式，此处改为等价的 GBK 名称；代码侧 GBK 实例一律经 GXX.Core.EncodingInit.GBK 获取，以消除加载顺序依赖。）
//   逐行对照，行尾裸 LF/CRLF 混用已归一化后再定行号。
//   主树 Source\RunGate\MagicIntervalUtils.pas 与本工作树同名文件 SHA256 相同：
//   593E6D539841F1D9BE80AEDBDEE9E5C051482D48582E75FA4F5D12B99B461FD9
//
// 名字对齐说明（与 uFrmGameSpeedLogic.cs 的同名接缝做了一次合并）：
//   * 本轮之前 uFrmGameSpeedLogic.cs 里有一个 **最小接缝** `TMagicInterval` / `TMagicIntervalList`，
//     其 `SaveToFile` 用 `TIniFileEx` 写 `[Interval]` 节 —— 那是**接缝臆造**，原文 (:168-186) 是
//     `TStringList` 直写 `MagicId=Interval` 明文行、**没有节头**（`uFrmMain.pas:478` 的落盘文件名是
//     `MagicCD.txt` 而非 `.ini`）。本轮已按原文改正，并把那条错误期望从窗体测试里一并修正。
//   * 接缝的 `Find` 是**线性**查找；原文 (:108-132) 是**二分**查找（隐含要求列表按 MagicId 有序）。
//     已按原文改为二分（见 §缺陷 1）。
//
// 指针类型：原文 `PMagicInterval = ^TMagicInterval`（:9）。托管侧 `TMagicInterval` 本身即引用类型，
//   `PMagicInterval` 与 `TMagicInterval` 完全同义，故不再另立类型（与 uFrmGameSpeedLogic.cs 的
//   `TProcessInfo` / `PTProcessInfo` 处理方式一致）。所有出现 `PMagicInterval` 的位置在注释里标注。
//
// 原文缺陷登记（照抄 + 差异断言，未做"顺手修正"）：
//   1. :108-132 `Search` 是 TStringList.Find 式的**二分**查找，隐含"FList 始终有序"这一前提；
//      但 `Add` (:61-72) 只按 Search 返回的插入点 `Insert`，**不排序**。调用方
//      `uFrmMagicCD.pas:486-496` 按界面行序 Add（行序来自 Magic.DB，未必升序）→ 一旦出现
//      逆序插入，`Find` 会漏命中。`UnsortedInsert_MakesLaterFindMiss` 固化该语义。
//   2. :134-166 `LoadFromFile` 的 `if not FileExists(FileName) then Exit;` 在 `Clear` **之前** ——
//      文件不存在时**保留**旧列表（不是"读失败即清空"）。`LoadFromFile_MissingFile_KeepsExistingItems` 固化。
//   3. :66 `if not Search(MagicID, I) or FDuplicates then`：短路求值让 `Search` **总是**先执行，
//      `I` 必然被赋值；且 `FDuplicates=True` 时重复项会被**插入**（而不是替换）。
//   4. :127 `if not FDuplicates then L := I;` —— 非重复模式下二分命中后把 `L` 钉在命中处再退出
//      （`H := I - 1` 已保证 `L > H`）。若照抄成 `break` 也等价，此处保留原顺序。
//   5. :152-153 `StrToIntDef(sMagicID, -1)` / `StrToIntDef(sMagicValue, -1)`：判据是 `>= 0`，
//      所以 `-1`/空串/非数字都会被跳过；`nMagicID: Integer` 隐式收窄成 `Word`（$R- 下按低 16 位截断）。
//      见 `LoadFromFile_MagicIdAboveWordRange_TruncatesToLow16Bits`。
//   6. :49 `FDuplicates := False` 是显式初始化（字段默认已是 False），照抄。
//   7. :102 `(Index >= 0) and (Index <= FList.Count - 1)` —— 用 `<= Count-1` 而不是 `< Count`，
//      空列表时 `Count-1 = -1` 与 `Index >= 0` 合起来仍然正确，照抄写法。
// =====================================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using GXX.Core.Rtl;
using GXX.Core.Util;

namespace GXX.RunGate;

/// <summary>原文 :10-13 `TMagicInterval = record MagicId: Word; Interval: LongWord; end;`。
/// （原文另声明 `PMagicInterval = ^TMagicInterval`（:9），托管侧同义，见文件头。）</summary>
public class TMagicInterval
{
    public ushort MagicId;   // Word
    public uint Interval;    // LongWord
}

/// <summary>原文 :15-40 `TMagicIntervalList = class(TObject)` 的 1:1 移植。</summary>
public class TMagicIntervalList
{
    // 原文 :17 `FCS: TRTLCriticalSection`。Windows CRITICAL_SECTION 是**可重入**的，
    // 故用 Monitor（同一线程可重复 Enter）而不是非重入的 SemaphoreSlim。
    private readonly object FCS = new object();

    /// <summary>原文 :18 `FDuplicates: Boolean`。</summary>
    private bool FDuplicates;

    /// <summary>原文 :19 `FList: TList`（元素为 `PMagicInterval`）。</summary>
    private readonly List<TMagicInterval> FList = new List<TMagicInterval>();

    /// <summary>原文 :22-23 `constructor Create`：`FList := TList.Create; FDuplicates := False; InitializeCriticalSection(FCS);`。
    /// （:48-50；`FDuplicates := False` 是显式初始化，照抄。）</summary>
    public TMagicIntervalList()
    {
        FList = new List<TMagicInterval>();
        FDuplicates = false;
        // InitializeCriticalSection(FCS) —— 托管侧无对应动作
    }

    /// <summary>原文 :26 `property Duplicates: Boolean read FDuplicates write FDuplicates;`。</summary>
    public bool Duplicates
    {
        get => FDuplicates;
        set => FDuplicates = value;
    }

    /// <summary>原文 :27 `property Count: Integer read GetCount;`（:95-98）。</summary>
    public int Count => FList.Count;

    /// <summary>原文 :28 `property Items[Index: Integer]: PMagicInterval read GetItems;`（:100-106）。
    /// 越界（含负数、含 `Index = Count`）返回 **nil**（不是抛异常）——
    /// 判据是 `(Index >= 0) and (Index <= FList.Count - 1)`。</summary>
    public TMagicInterval this[int Index]
    {
        get
        {
            // 原 :102-105
            if ((Index >= 0) && (Index <= FList.Count - 1))
                return FList[Index];
            else
                return null;
        }
    }

    /// <summary>原文 :74-83 `procedure Clear`：逐个 `Dispose(PMagicInterval(FList.Items[I]))` 后 `FList.Clear`。
    /// 托管侧 `List.Clear()` 即等价（原对象由 GC 回收）。</summary>
    public void Clear()
    {
        // for I := 0 to FList.Count - 1 do Dispose(PMagicInterval(FList.Items[I]));
        FList.Clear();   // 原 :82
    }

    /// <summary>原文 :61-72 `function Add(MagicID: Word): PMagicInterval`。
    /// 参数为 `Word`；C# 侧保持 `ushort`，调用方按原文的隐式收窄语义显式转换。</summary>
    public TMagicInterval Add(ushort MagicID)
    {
        // 原 :65 `Result := nil;`
        TMagicInterval Result = null;

        // 原 :66 `if not Search(MagicID, I) or FDuplicates then`
        //   短路求值：`Search` 总是先被调用，故 I 必然被赋值（不存在未初始化读取）。
        int I;
        bool found = Search(MagicID, out I);
        if (!found || FDuplicates)
        {
            // 原 :68-70
            Result = new TMagicInterval();
            Result.MagicId = MagicID;
            FList.Insert(I, Result);
        }
        return Result;
    }

    /// <summary>原文 :85-93 `function Find(MagicID: Word): PMagicInterval`。
    /// 未命中返回 **nil**（`xUnit` 侧断言 `Assert.Null`）。</summary>
    public TMagicInterval Find(ushort MagicID)
    {
        int I;
        // 原 :89-92
        if (!Search(MagicID, out I))
            return null;
        else
            return FList[I];
    }

    /// <summary>原文 :108-132 `function Search(MagicID: Word; var Index: Integer): Boolean; virtual;`
    /// 标准 TStringList.Find 式二分：命中且 `not FDuplicates` 时把 Index 钉在命中处；
    /// `FDuplicates` 时继续向左收敛到**重复段的首个**元素。未命中时 Index = 插入点。
    /// `virtual` 保留（原文 `virtual`，可被派生类覆写）。</summary>
    public virtual bool Search(ushort MagicID, out int Index)
    {
        bool Result;
        int L, H, I, C;

        // 原 :112-114
        Result = false;
        L = 0;
        H = FList.Count - 1;

        // 原 :115-130
        while (L <= H)
        {
            I = (L + H) >> 1;                                   // 原 :117 `I := (L + H) shr 1;`
            // 原 :118 `C := PMagicInterval(FList.Items[I]).MagicId - MagicID;`
            //   MagicId 与 MagicID 都是 Word，差落在 -65535..65535，不会溢出 Integer。
            C = FList[I].MagicId - MagicID;
            if (C < 0)
                L = I + 1;                                      // 原 :120
            else
            {
                H = I - 1;                                      // 原 :123
                if (C == 0)                                     // 原 :124
                {
                    Result = true;                              // 原 :126
                    if (!FDuplicates) L = I;                    // 原 :127
                }
            }
        }
        Index = L;                                              // 原 :131
        return Result;
    }

    /// <summary>原文 :134-166 `procedure LoadFromFile(FileName: string)`。
    /// ★ 文件不存在时**直接 Exit，不清空**（`Clear` 在 FileExists 判据之后）。</summary>
    public void LoadFromFile(string FileName)
    {
        // 原 :142 `if not FileExists(FileName) then Exit;`
        if (!File.Exists(FileName)) return;

        Clear();                                                // 原 :143

        // 原 :144-165 `SL := TStringList.Create; try SL.LoadFromFile(FileName); ... finally SL.Free; end;`
        var SL = new TStringList();
        SL.LoadFromFile(FileName);

        for (int I = 0; I < SL.Count; I++)                      // 原 :147 `for I := 0 to SL.Count - 1 do`
        {
            // 原 :149-150 `sMagicID := Trim(SL.Names[I]); sMagicValue := Trim(SL.ValueFromIndex[I]);`
            //   Delphi `TStrings.GetName`    = 首个 NameValueSeparator('=') 之前的部分，再 TrimRight；
            //   Delphi `TStrings.GetValueFromIndex` = 首个 '=' 之后的部分（**不做** TrimLeft），无 '=' 时为 ''。
            //   原文随后对两者都做了 `Trim`，故此处直接给出已 Trim 的等价结果。
            SplitNameValue(SL[I], out string rawName, out string rawValue);
            string sMagicID = DelphiRTL.Trim(rawName);          // 原 :149
            string sMagicValue = DelphiRTL.Trim(rawValue);      // 原 :150

            // 原 :152-153
            int nMagicID = DelphiRTL.StrToIntDef(sMagicID, -1);
            int nMagicValue = DelphiRTL.StrToIntDef(sMagicValue, -1);

            // 原 :154 `if (nMagicID >= 0) and (nMagicValue >= 0) then`
            if ((nMagicID >= 0) && (nMagicValue >= 0))
            {
                // 原 :156 `MagicInterval := Add(nMagicID);` —— nMagicID: Integer 隐式收窄为 Word。
                // 交付环境（$R- 范围检查关闭）下按低 16 位截断；此处显式写出截断。
                var MagicInterval = Add(unchecked((ushort)(nMagicID & 0xFFFF)));
                // 原 :157-160
                if (MagicInterval != null)
                {
                    MagicInterval.Interval = unchecked((uint)nMagicValue);   // 原 :159（Integer → LongWord）
                }
            }
        }
    }

    /// <summary>原文 :168-186 `procedure SaveToFile(FileName: string)`。
    /// ★ 明文行 `MagicId=Interval`，**没有 INI 节头**；由 `TStringList.SaveToFile` 以 CRLF + GBK 落盘。</summary>
    public void SaveToFile(string FileName)
    {
        // 原 :174-185
        var SL = new TStringList();
        for (int I = 0; I < FList.Count; I++)                   // 原 :176 `for I := 0 to FList.Count - 1 do`
        {
            var MagicInterval = FList[I];                       // 原 :178
            // 原 :179 `SL.Add(IntToStr(MagicInterval.MagicId) + '=' + IntToStr(MagicInterval.Interval));`
            SL.Add(DelphiRTL.IntToStr(MagicInterval.MagicId) + "=" + DelphiRTL.IntToStr(MagicInterval.Interval));
        }
        SL.SaveToFile(FileName);                                // 原 :182
    }

    /// <summary>原文 :188-191 `procedure Lock; begin EnterCriticalSection(FCS); end;`。</summary>
    public void Lock() => Monitor.Enter(FCS);

    /// <summary>原文 :193-196 `procedure UnLock; begin LeaveCriticalSection(FCS); end;`。</summary>
    public void UnLock() => Monitor.Exit(FCS);

    /// <summary>Delphi 7 `TStrings.GetName` / `TStrings.GetValueFromIndex` 的分割语义
    /// （Classes.pas：以**首个** `NameValueSeparator`='=' 切分；`GetName` 再做 TrimRight，
    /// `GetValueFromIndex` 无 '=' 时返回 ''）。原文调用方随后又 `Trim` 了两侧，故这里返回原始切片。</summary>
    private static void SplitNameValue(string line, out string name, out string value)
    {
        line ??= "";
        int P = line.IndexOf('=');       // AnsiPos(NameValueSeparator, Result)；Delphi 未找到返回 0
        if (P >= 0)                      // 原文 `if P <> 0 then`
        {
            name = line.Substring(0, P);            // System.Delete(Result, P, MaxInt) 之前的左半
            value = line.Substring(P + 1);          // System.Delete(Result, 1, P)
        }
        else
        {
            name = line;                            // 无分隔符：整行
            value = "";                             // 原样返回空串
        }
        // TStrings.GetName 的收尾 TrimRight（此处保留，便于与原文逐字对照）
        name = name.TrimEnd(' ', '\t');
    }
}
