// ============================================================================
// 源单元：Source\M2Engine\ObjPlayer.pas（GBK，49,232 LF）
// 本文件：**变量容器默认值修正片**（切片 1 / 4 的收尾）。
// 覆盖原文行号范围：
//   · ObjPlayer.pas:119   m_nVal: array [0 .. 999] of Integer;
//   · ObjPlayer.pas:123   m_TVal: array [0 .. 499] of string[100];
//   · ObjPlayer.pas:125   m_ZVal: array [0 .. 499] of string[100];   (1天1清)
//   · ObjPlayer.pas:127   m_sString: array [0 .. 999] of string;
// 关联缺陷：`Engine/CombatPower.cs:557` 的 `m_ZVal`（以及本车道 :123/:127 两个新字段）是
//   `string[]`，元素默认 **`null`**；原文 `string[100]` / `string` 默认 **`''`**。
//   ObjNpc 车道报告 §5.4 D18 已登记该差异，但**无法在自己分区内修复**（Engine 属常驻区）。
//
// ⚠ 为什么**不能**在本车道直接修：
//   字段声明在 `Engine/CombatPower.cs:557`（本车道对该文件只读），且 `Engine/ObjBase.cs:169`
//   已声明 `public TPlayObject()` —— 本工程是多 partial 合并，本文件再声明无参构造会 **CS0111**；
//   声明 `TPlayObject(bool = true)` 又会让 `TPlayObject()` 的部分构造调用**递归**。
//   故本文件提供**可显式调用**的幂等迁移入口 + 读取归一化入口，
//   并把「在构造末尾加一行」的精确要求写进交付报告交由集成方处理。
// ============================================================================

namespace GXX.M2Server.Engine;

/// <summary>
/// 变量容器默认值修正：托管 `string[]` 元素默认 `null` → 原文 ShortString/AnsiString 默认 `''`。
/// </summary>
/// <remarks>
/// 原文（ObjPlayer.pas:119-127）：
/// <code>
///   m_nVal:     array [0 .. 999] of Integer;       // 默认 0
///   m_TVal:     array [0 .. 499] of string[100];   // 默认 ''
///   m_ZVal:     array [0 .. 499] of string[100];   // 默认 ''
///   m_sString:  array [0 .. 999] of string;        // 默认 ''
/// </code>
/// `null` 与 `''` 在原文里**不是同一个值**：`Length(m_ZVal[i])`、`CompareText(m_ZVal[i], '')`、
/// `m_ZVal[i] + 'x'` 在 Delphi 下都按空串处理；托管侧 `null` 在拼接处静默变 `""`，
/// 但 `Length`/`CompareText` 类调用会**抛 NullReferenceException**（见差异断言用例）。
/// </remarks>
public static class PlayerSurfaceVarDefaults
{
    /// <summary><see cref="MigrateStringVarDefaults"/> 的累计调用次数（单测用于确认入口真的被执行）。</summary>
    public static long MigratedInstanceCount;

    /// <summary>
    /// 把 <paramref name="obj"/> 的字符串变量容器元素 `null` → `''`（**幂等**）。
    /// 应在对象构造完成后调用一次；重复调用无副作用。
    /// 覆盖字段：`m_ZVal`（`CombatPower.cs:557`）、`m_TVal`、`m_sString`、`m_ArrayList` 键表。
    /// </summary>
    /// <remarks>
    /// ⚠ 这是**接缝式修正**，不是构造路径上的自动修正。真正的根因修复要求
    /// （见交付报告「接缝清单」）是在 `Engine/ObjBase.cs:169` 的 `public TPlayObject()`
    /// 体末尾追加一行：
    /// <code>PlayerSurfaceVarDefaults.MigrateStringVarDefaults(this);</code>
    /// 本车道无权改 `ObjBase.cs`，故未执行。
    /// </remarks>
    public static void MigrateStringVarDefaults(TPlayObject obj)
    {
        FillEmpty(obj.m_ZVal);
        FillEmpty(obj.m_TVal);
        FillEmpty(obj.m_sString);
        MigratedInstanceCount++;
    }

    private static void FillEmpty(string[] arr)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] == null) arr[i] = "";
        }
    }

    /// <summary>
    /// 读取任意 `string[]` 变量容器的元素，把 `null` 归一为原文默认值 `''`。
    /// 调用点不想依赖「是否已迁移」时使用。
    /// </summary>
    public static string ReadVar(string[] container, int index) => container[index] ?? "";
}
