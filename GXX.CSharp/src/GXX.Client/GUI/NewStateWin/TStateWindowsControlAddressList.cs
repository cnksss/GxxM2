namespace GXX.Client.GUI.NewStateWin;

/// <summary>
/// TStateWindows.MakeControlAddressList 的**可观察行为**移植（原文 StateWindows.pas:1282-2131）。
///
/// 原文实现：
/// <code>
/// Result := THashedStringList.Create;
/// Result.AddObject('ContinuousMagicMenu', Pointer(@ContinuousMagicMenu));
/// ...（共 845 条，顺序与第 49-892 行的字段声明完全一致）
/// </code>
/// C# 侧字段是托管引用，不能取「字段地址」；因此这里保留三个可验证的可观察量：
///   1) 构造（非 null，且为空表起步，再顺序 Add）；
///   2) 键的**顺序**（THashedStringList 是 TStringList 派生：AddObject 保序，且 IndexOf 大小写不敏感）；
///   3) 键的**条数**；
/// 并给出「名字 → 注册序号」的索引（等价于 THashedStringList.IndexOf 的返回值但大小写不敏感）。
/// 字段到控件的实际绑定留给 TStateWindows 的后续批次（见 <see cref="TStateWindowsFieldTable"/>）。
/// </summary>
public static class TStateWindowsControlAddressList
{
    /// <summary>THashedStringList.Create 的 C# 等价物：空表起步（原文 1284 行）。</summary>
    public static List<string> Create()
        => new List<string>();

    /// <summary>
    /// 依次 AddObject(name, @field) 的键序列（原文 1285-2130 行）。
    /// THashedStringList 继承自 TStringList，AddObject 追加到末尾 —— 顺序即字段声明顺序。
    /// </summary>
    public static List<string> BuildNames()
    {
        var list = Create();
        var names = TStateWindowsControlNameTable.Names;
        for (int i = 0; i < names.Length; i++)
            list.Add(names[i]);
        return list;
    }

    /// <summary>
    /// THashedStringList.IndexOf（TStringList.IndexOf 语义：**大小写不敏感**，找不到返回 -1）。
    /// </summary>
    /// <param name="names">BuildNames() 的结果</param>
    /// <param name="controlName">要查的控件名</param>
    public static int IndexOf(List<string> names, string controlName)
    {
        // 原文如此（Delphi TStringList.IndexOf → CompareStrings → AnsiCompareText）：
        // 大小写不敏感比较；C# 侧用 OrdinalIgnoreCase 对齐 ASCII 段的 AnsiCompareText 结果。
        if (names == null || controlName == null)
            return -1;

        for (int i = 0; i < names.Count; i++)
        {
            if (string.Equals(names[i], controlName, StringComparison.OrdinalIgnoreCase))
                return i;
        }

        return -1;
    }
}
