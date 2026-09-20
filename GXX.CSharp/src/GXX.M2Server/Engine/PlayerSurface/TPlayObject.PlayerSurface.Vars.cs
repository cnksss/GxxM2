// ============================================================================
// 源单元：Source\M2Engine\ObjPlayer.pas（实测 49,232 LF，GBK，混用 CRLF/裸 LF）
// 本文件：**变量容器片**（切片 1 / 4）。
// 覆盖原文行号范围：
//   · ObjPlayer.pas:119   m_nVal      （P 变量）
//   · ObjPlayer.pas:123   m_TVal      （T 变量）
//   · ObjPlayer.pas:127   m_sString   （S 变量）
//   · ObjPlayer.pas:260   m_ArrayList （S$ 数组变量容器）
//   · ObjPlayer.pas:1627  FillChar(m_sString, ...)  调用点
//   · ObjPlayer.pas:1833  FillChar(m_TVal, ...)     调用点
//   · ObjPlayer.pas:1835  FillChar(m_ZVal, ...)     调用点
//   · ObjPlayer.pas:3849  FillChar(m_ZVal, ...)     调用点
//
// 手法：`Engine/**` 既有文件对本车道**完全只读**，全部新成员以 `partial` 落在本目录。
// 命名空间与既有 partial 声明一致（GXX.M2Server.Engine）。
//
// ⚠ 本文件**不重复声明**既有成员：
//   · m_nMval / m_DyVal / m_UVal / m_JVal / m_ZVal / m_nInteger / m_StringList / m_IntegerList
//     已由 `Engine/CombatPower.cs:541-563` 声明 —— 本车道**未重复定义**。
//   · m_ZVal 的元素默认值缺陷（`null` vs 原文 ShortString 的 `''`）由
//     `TPlayObject.PlayerSurface.ZValDefault.cs` 以**不重复声明**的方式修正。
// ============================================================================

using System;
using System.Collections.Generic;

namespace GXX.M2Server.Engine;

/// <summary>
/// ObjPlayer.pas TPlayObject 的**变量容器**（P / T / S / S$ 四类）。
///
/// 原文声明（ObjPlayer.pas:119-127、260）：
/// <code>
///   m_nVal: array [0 .. 999] of Integer;        // P      （:119）
///   m_TVal: array [0 .. 499] of string[100];    // 私有变量T（:123）
///   m_sString: array [0 .. 999] of string;      // 临时私人变量S（:127）
///   m_ArrayList: TValueList;                    // S      （:260）
/// </code>
///
/// **不重复的部分**：`m_nMval`/`m_DyVal`/`m_UVal`/`m_JVal`/`m_ZVal`/`m_nInteger`/
/// `m_StringList`/`m_IntegerList` 在 `CombatPower.cs` 已存在，本文件不声明。
/// </summary>
public partial class TPlayObject
{
    /// <summary>
    /// 原文 `m_nVal: array [0 .. 999] of Integer;`（ObjPlayer.pas:119，P 变量）。
    /// 语义要点：原文用 `FillChar(Player.m_nVal[0], SizeOf(Player.m_nVal), 0)`
    /// **整块清零**（ObjNpc.pas:9394/9417「P变量重点对话框时置0」），托管侧对应 `Array.Clear`。
    /// ⚠ 原文 `m_nVal` 是**有符号** Integer 数组，`-1` 是**合法值**（不是哨兵）——
    /// 判「变量是否存在」不能用 `!= -1`，必须另设存在性标志。
    /// </summary>
    public readonly int[] m_nVal = new int[1000];

    /// <summary>
    /// 原文 `m_TVal: array [0 .. 499] of string[100];`（ObjPlayer.pas:123，私有变量 T，字符串型）。
    /// ⚠ **ShortString 语义**：原文每个元素上限 **100 字符**，超长赋值被 Delphi **静默截断**；
    /// 托管 `string` 不截断 —— 写入请走 <see cref="SetTVal"/>，
    /// 差异断言见 <c>PlayerSurfaceVarsTests.TVal_ShortStringTruncation_DiffersFromPlainString</c>。
    /// </summary>
    public readonly string[] m_TVal = new string[500];

    /// <summary>
    /// 原文 `m_sString: array [0 .. 999] of string;`（ObjPlayer.pas:127，临时私人变量 S）。
    /// ⚠ 原文是**无长度上限的 AnsiString**（与 `m_TVal` 的 `string[100]` **不同**），
    /// 托管侧同样不截断 —— 这一条**不需要** ShortString 处理，别与 `m_TVal` 混为一谈。
    /// 元素默认值为 `''`（当前实测：内存擦写前后若未显式填充，Delphi 侧亦为 `''`）。
    /// </summary>
    public readonly string[] m_sString = new string[1000];

    /// <summary>
    /// 原文 `m_ArrayList: TValueList; // S`（ObjPlayer.pas:260）。
    /// 本车道**复用** `CombatPower.cs:567` 已定义的 `TValueListStub`（GetIndex 为
    /// UpperCompare 语义），**未造第二份**。原文在 :1680 `TValueList.Create`、
    /// :6535 `FreeAndNil` —— 托管侧即构造时创建、置 null 释放。
    /// </summary>
    public readonly TValueListStub m_ArrayList = new();

    // ------------------------------------------------------------------
    // 原文整块清零调用点（ObjPlayer.pas:1627 / 1833 / 1835 / 3849）
    // ------------------------------------------------------------------

    /// <summary>
    /// 原文 `FillChar(m_sString, SizeOf(m_sString), #0);`（ObjPlayer.pas:1627）。
    /// Delphi `FillChar` 逐字节写 `#0`；对 `string` 数组的语义即**每个元素置空串**。
    /// </summary>
    public void ClearSStrings()
    {
        // 原文 1627：FillChar(m_sString, SizeOf(m_sString), #0);
        Array.Clear(m_sString, 0, m_sString.Length);
    }

    /// <summary>
    /// 原文 `FillChar(m_TVal, SizeOf(m_TVal), 0);`（ObjPlayer.pas:1833）。
    /// ⚠ 原文用 `FillChar(..., 0)`（**不是** `#0`）—— 对 ShortString 而言两者等价（都写零字节，
    /// 长度字节为 0 即空串）。托管侧同样等价，逐字保留原文写法差异。
    /// </summary>
    public void ClearTValues()
    {
        // 原文 1833：FillChar(m_TVal, SizeOf(m_TVal), 0);
        Array.Clear(m_TVal, 0, m_TVal.Length);
    }

    /// <summary>
    /// 原文 `FillChar(m_ZVal, SizeOf(m_ZVal), 0);`
    /// （ObjPlayer.pas:1835 与 :3849 两处调用点；Z 变量「1 天 1 清」）。
    /// ⚠ `m_ZVal` 字段本身由 `CombatPower.cs:557` 声明，本方法**只做清零**、不重复声明字段；
    /// 元素默认 `null` → `''` 的修正见 `TPlayObject.PlayerSurface.ZValDefault.cs`。
    /// </summary>
    public void ClearZValues()
    {
        // 原文 1835 / 3849：FillChar(m_ZVal, SizeOf(m_ZVal), 0);
        Array.Clear(m_ZVal, 0, m_ZVal.Length);
    }

    /// <summary>
    /// 原文 `FillChar(Player.m_nVal[0], SizeOf(Player.m_nVal), 0);`
    /// （ObjNpc.pas:9394 / 9417「P变量重点对话框时置0」）—— P 变量整块清零。
    /// </summary>
    public void ClearNValues()
    {
        // 原文 ObjNpc.pas:9394 / 9417
        Array.Clear(m_nVal, 0, m_nVal.Length);
    }

    // ------------------------------------------------------------------
    // 写入口（保留原文 ShortString 截断语义）
    // ------------------------------------------------------------------

    /// <summary>原文 `string[100]` 的最大字符数（ShortString 长度字节上限）。</summary>
    public const int SHORTSTRING_100_MAX = 100;

    /// <summary>
    /// 1:1 复刻 Delphi `string[100]` 的**赋值截断**：赋给 `m_TVal[i]` 的值超过 100 字符时静默截断。
    /// 原文调用点：ObjPlayer.pas:32244、ObjBase.pas:26290、PluginImplement.pas:4559、
    /// HumanInfo.pas:962。
    /// </summary>
    /// <remarks>
    /// ⚠ 差异断言：直接写 `m_TVal[i] = s`（C# 原生赋值）**不会**截断，
    /// 与原文行为不同；差异用例见 <c>PlayerSurfaceVarsTests</c>。
    /// </remarks>
    public static string TruncShortString100(string value)
    {
        if (value == null) return "";
        return value.Length <= SHORTSTRING_100_MAX ? value : value.Substring(0, SHORTSTRING_100_MAX);
    }

    /// <summary>
    /// 写入 `m_TVal[i]`（原文 `m_TVal[i] := sValue;` —— ObjBase.pas:26290、ObjNpc.pas:5132）。
    /// 带 Delphi `string[100]` 截断语义。
    /// </summary>
    public void SetTVal(int index, string value)
    {
        m_TVal[index] = TruncShortString100(value);
    }

    /// <summary>
    /// 写入 `m_ZVal[i]`（原文 `m_ZVal[i] := sValue;` —— ObjNpc.pas:5166、HumanInfo.pas:922）。
    /// 带 Delphi `string[100]` 截断语义。
    /// ⚠ 读 `m_ZVal` 时若该下标**从未被写过**，原文得 `''`；托管字段（`CombatPower.cs:557`）
    /// 得 `null` —— 请用 <see cref="GetZVal"/> 读取，或先调用一次清零。
    /// </summary>
    public void SetZVal(int index, string value)
    {
        m_ZVal[index] = TruncShortString100(value);
    }

    /// <summary>
    /// 读取 `m_ZVal[i]`，把托管侧元素默认值 `null` 归一为原文 ShortString 默认值 `''`。
    /// 原文读取点：ObjNpc.pas:5822 / 9156、HumanInfo.pas:594、PluginImplement.pas:4547 附近。
    /// </summary>
    public string GetZVal(int index) => m_ZVal[index] ?? "";

    /// <summary>
    /// 读取 `m_TVal[i]`，元素默认值与原文一致（新建数组元素为 `null`，此处同样归一为 `''`）。
    /// </summary>
    public string GetTVal(int index) => m_TVal[index] ?? "";

    // ------------------------------------------------------------------
    // 变量存在性（原文在 TValueList / TQuickList 上判 GetIndex >= 0）
    // ------------------------------------------------------------------

    /// <summary>
    /// 原文 `m_StringList.GetIndex(sVarName)` 的**存在性**判定（S$ 自定义字符串变量表）。
    /// ⚠ 与 `m_sString` 数组是**两套不同的 S 变量**：`m_sString[i]` 是定长下标访问，
    /// `m_StringList` 是具名键值表。本方法只做 `>= 0` 包装，避免各调用点各写一遍。
    /// </summary>
    public bool HasStringVar(string sVarName) => m_StringList.GetIndex(sVarName) >= 0;

    /// <summary>原文 `m_IntegerList.GetIndex(sVarName)` 的**存在性**判定（N$ 自定义整数变量表）。</summary>
    public bool HasIntegerVar(string sVarName) => m_IntegerList.GetIndex(sVarName) >= 0;
}

/// <summary>
/// 变量容器的**下标常量**（取自原文 `m_nVal` / `m_TVal` / `m_ZVal` 的读写把戏）。
/// 原文源码把「变量名 → 数组下标」的映射硬编码在各调用点（见 `CombatPowerUtils.GetValNameNo`），
/// 本类只登记**数组长度**，不重复实现映射。
/// </summary>
public static class PlayerVarSurfaceConst
{
    /// <summary>原文 `m_nVal`/`m_sString`/`m_nMval`/`m_DyVal`/`m_nInteger` 的元素数（ObjPlayer.pas:119/127/120/121/126）。</summary>
    public const int NATIVE_VAR_COUNT = 1000;

    /// <summary>原文 `m_UVal`/`m_JVal`/`m_TVal`/`m_ZVal` 的元素数（ObjPlayer.pas:122/123/124/125）。</summary>
    public const int PRIVATE_VAR_COUNT = 500;

    /// <summary>原文 `m_TVal`/`m_ZVal` 的 ShortString 长度（ObjPlayer.pas:123/125）。</summary>
    public const int PRIVATE_VAR_SHORTSTRING_LEN = 100;
}
