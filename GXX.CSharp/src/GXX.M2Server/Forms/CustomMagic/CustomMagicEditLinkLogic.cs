// ============================================================================
// uFrmCustomMagic.pas（Source\M2Engine\Forms\uFrmCustomMagic.pas，4800 行，GBK）1:1 移植
// 车道 p5-m2-custommagic ｜ 命名空间 GXX.M2Server.Forms.CustomMagic
//
// 本文件 = VirtualTrees.pas IVTEditLink 两个实现的**纯逻辑内核**：
//   * TDecAttribPropertyEditLink（原文 :888-907 声明、:932-1285 实现）→ DecAttribPropertyEditLinkLogic
//   * TElementPropertyEditLink（原文 :909-928 声明、:1286-1604 实现）→ ElementPropertyEditLinkLogic
//
// 处置（照搬 p2-rungate-impl 车道的 GameSpeedLogic.PrepareEditSpec / ApplyEditorResult）：
//   编辑器控件的**创建/销毁/消息泵**（TSpinEditEx.Create / TComboBox.Items.Add / WindowProc）
//   留接缝，见 CustomMagicEditLinkSeam.cs；
//   而**列 → 编辑类型 / 初值 / 范围** 与 **编辑器取值 → 字段写回 / IsChanged** 的全部判定
//   抽成 PrepareEditSpec / ApplyEditorResult 两个纯函数，可 100% 单测。
//
// 覆盖行号（Delphi）：
//   TDecAttribPropertyEditLink：932-1270、1274-1282
//   TElementPropertyEditLink ：1286-1591、1595-1603
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Forms.CustomMagic;

/// <summary>VirtualTrees 就地编辑器类型（对应原文 PrepareEdit 里 FEdit 的实际类）。</summary>
public enum TVtEditKind
{
    /// <summary>原文该列不建编辑器（FEdit 保持 nil）——见报告"原文缺陷"#1。</summary>
    None,
    /// <summary>TSpinEditEx。</summary>
    SpinEdit,
    /// <summary>TComboBox（Style = csDropDownList）。</summary>
    ComboBox,
    /// <summary>TEdit。</summary>
    Edit,
}

/// <summary>
/// PrepareEdit 的纯输出：要建哪个编辑器、范围、初值、下拉项与选中项、初始文本。
/// 生产侧据此 new 出真实控件；测试侧直接断言本对象。
/// </summary>
public sealed class TVtEditSpec
{
    /// <summary>编辑器类型。</summary>
    public TVtEditKind Kind;

    /// <summary>原文 <c>if FColumn in [1] then begin MinValue := 0; MaxValue := 100; end;</c>。</summary>
    public bool HasMinMax;
    /// <summary>MinValue。</summary>
    public int MinValue;
    /// <summary>MaxValue。</summary>
    public int MaxValue;

    /// <summary>TSpinEditEx.Value 初值。</summary>
    public int Value;

    /// <summary>TComboBox.Items（按 Low(Boolean)..High(Boolean) 顺序）。</summary>
    public readonly List<string> Items = new();

    /// <summary>TComboBox.ItemIndex 初值。</summary>
    public int ItemIndex = -1;

    /// <summary>TEdit.Text 初值。</summary>
    public string Text = "";

    /// <summary>原文 <c>Visible := False; Parent := Tree;</c>（两种编辑器都有）。</summary>
    public bool VisibleInitFalse = true;

    /// <summary>原文 <c>Style := csDropDownList;</c>（仅 TComboBox）。</summary>
    public bool DropDownList;

    /// <summary>原文 <c>OnKeyDown := EditKeyDown; OnKeyUp := EditKeyUp;</c>（仅真有编辑器时）。</summary>
    public bool HooksKeyEvents;
}

/// <summary>
/// EndEdit 的纯输入：编辑器当前值（三个通道，按 Kind 取其一）。
/// </summary>
public sealed class TVtEditorResult
{
    /// <summary>TSpinEditEx.Value（或 TSpinEdit.Value）。</summary>
    public int Value;
    /// <summary>TComboBox.ItemIndex。</summary>
    public int ItemIndex = -1;
    /// <summary>TEdit.Text。</summary>
    public string Text = "";
}

/// <summary>EditKeyDown / EditKeyUp 的纯决策结果。</summary>
public enum TVtEditKeyAction
{
    /// <summary>什么都不做（Key 原样透传）。</summary>
    None,
    /// <summary>只把 Key 置 0（吞掉）。</summary>
    SwallowKey,
    /// <summary>Key 置 0 → FTree.EndEditNode → Abort（原文 VK_RETURN 分支）。</summary>
    SwallowKeyThenEndEditNode,
    /// <summary>FTree.CancelEditNode → Key 置 0（原文 VK_ESCAPE 的 KeyUp 分支）。</summary>
    CancelEditNodeThenSwallowKey,
    /// <summary>PostMessage(FTree.Handle, WM_KEYDOWN, Key, 0) → Key 置 0。</summary>
    PostKeyDownThenSwallowKey,
}

/// <summary>Delphi Windows 单元的虚拟键与消息常量（本单元用到的那几个）。</summary>
public static class TVtKeys
{
    /// <summary>VK_ESCAPE。</summary>
    public const int VK_ESCAPE = 0x1B;
    /// <summary>VK_RETURN。</summary>
    public const int VK_RETURN = 0x0D;
    /// <summary>VK_UP。</summary>
    public const int VK_UP = 0x26;
    /// <summary>VK_DOWN。</summary>
    public const int VK_DOWN = 0x28;
    /// <summary>WM_KEYDOWN。</summary>
    public const int WM_KEYDOWN = 0x0100;
}

/// <summary>两个 EditLink 共享的键盘决策（原文两份 <c>EditKeyDown</c>/<c>EditKeyUp</c> 逐字相同）。</summary>
public static class TVtEditLinkKeys
{
    /// <summary>
    /// 原文 <c>EditKeyDown</c>（TDecAttribPropertyEditLink :942-975；TElementPropertyEditLink :1296-1329，
    /// 两份逐字相同）。<paramref name="shiftEmpty"/> 对应 <c>Shift = []</c>。
    /// </summary>
    public static TVtEditKeyAction EditKeyDown(TVtEditKind editKind, bool comboDroppedDown, bool shiftEmpty, int key)
    {
        bool canAdvance = true;

        switch (key)
        {
            case TVtKeys.VK_ESCAPE:
                // Key := 0; // ESC will be handled in EditKeyUp()
                return TVtEditKeyAction.SwallowKey;

            case TVtKeys.VK_RETURN:
                if (canAdvance)
                {
                    // Key := 0; FTree.EndEditNode; Abort;
                    return TVtEditKeyAction.SwallowKeyThenEndEditNode;
                }
                return TVtEditKeyAction.None;

            case TVtKeys.VK_UP:
            case TVtKeys.VK_DOWN:
                // Consider special cases before finishing edit mode.
                canAdvance = shiftEmpty;
                if (editKind == TVtEditKind.ComboBox)
                    canAdvance = canAdvance && !comboDroppedDown;
                else if (editKind == TVtEditKind.SpinEdit)
                    canAdvance = true;   // 原文如此（:966-967 / :1320-1321）：无条件覆盖前面的 Shift 判定
                if (canAdvance)
                    return TVtEditKeyAction.PostKeyDownThenSwallowKey;
                return TVtEditKeyAction.None;

            default:
                return TVtEditKeyAction.None;
        }
    }

    /// <summary>
    /// 原文 <c>EditKeyUp</c>（:977-986 / :1331-1340，两份逐字相同）：只有 VK_ESCAPE 有分支。
    /// </summary>
    public static TVtEditKeyAction EditKeyUp(int key)
        => key == TVtKeys.VK_ESCAPE ? TVtEditKeyAction.CancelEditNodeThenSwallowKey : TVtEditKeyAction.None;
}

/// <summary>
/// TDecAttribPropertyEditLink（原文 :888-907 声明 / :932-1285 实现）的纯逻辑。
/// 同时被 vstAttackDecAttr（:4414-4419）与 vstProtectedAddAttr（:4709-4714）用作 IVTEditLink。
/// </summary>
public static class DecAttribPropertyEditLinkLogic
{
    /// <summary>TSpinEditEx 分支的列集合（原文 :1176 / :1020）。</summary>
    public static readonly int[] SpinColumns = { 1, 2, 3, 5, 6, 8, 9, 10 };

    /// <summary>TComboBox 分支的列集合（原文 :1213 / :1091）。</summary>
    public static readonly int[] ComboColumns = { 4, 7, 11 };

    /// <summary>TEdit 分支的列集合（原文 :1249 / :1122）。</summary>
    public static readonly int[] EditColumns = { 13 };

    /// <summary>
    /// 原文 <c>PrepareEdit</c>（:1156-1263）的"建哪种编辑器"判定。
    /// **注意**：不在此三组内的列原文不建编辑器但 <c>Result</c> 仍为 True（见报告"原文缺陷"#1）。
    /// </summary>
    public static TVtEditKind EditKindForColumn(int column)
    {
        if (Array.IndexOf(SpinColumns, column) >= 0) return TVtEditKind.SpinEdit;
        if (Array.IndexOf(ComboColumns, column) >= 0) return TVtEditKind.ComboBox;
        if (Array.IndexOf(EditColumns, column) >= 0) return TVtEditKind.Edit;
        return TVtEditKind.None;
    }

    /// <summary>
    /// 原文 <c>PrepareEdit</c>（:1156-1263）1:1：列 → 编辑类型 / 范围 / 初值 / 下拉项与选中项。
    /// <paramref name="data"/> 为 <c>FTree.GetNodeData(Node)</c> 的解引用结果。
    /// </summary>
    public static TVtEditSpec PrepareEditSpec(int column, TAttackDecAttribData data)
    {
        var spec = new TVtEditSpec { Kind = EditKindForColumn(column) };
        if (data.Data is null)
            throw new InvalidOperationException("PrepareEdit: DecAttribData.Data 为 nil（原文会在此解引用崩溃）");

        switch (spec.Kind)
        {
            case TVtEditKind.SpinEdit:
                // Visible := False; Parent := Tree;
                if (column == 1)                       // 原文 :1184 if FColumn in [1]
                {
                    spec.HasMinMax = true;
                    spec.MinValue = 0;
                    spec.MaxValue = 100;
                }
                switch (column)                        // 原文 :1190-1207
                {
                    case 1: spec.Value = data.Data.Rate; break;
                    case 2: spec.Value = data.Data.RateAdd; break;
                    case 3: spec.Value = data.Data.LowValue; break;
                    case 5: spec.Value = data.Data.LowValueAdd; break;
                    case 6: spec.Value = data.Data.HighValue; break;
                    case 8: spec.Value = data.Data.HighValueAdd; break;
                    case 9: spec.Value = data.Data.Time; break;
                    case 10: spec.Value = data.Data.TimeAdd; break;
                }
                spec.HooksKeyEvents = true;
                break;

            case TVtEditKind.ComboBox:
                spec.DropDownList = true;               // Style := csDropDownList
                if (column == 4 || column == 7)         // 原文 :1223-1234
                {
                    for (int bo = 0; bo <= 1; bo++)     // Low(Boolean)..High(Boolean)
                        spec.Items.Add(CustomMagicUtils.MagicAttackDecValueTypeNames[bo]);

                    spec.ItemIndex = column == 4
                        ? (data.Data.LowValueIsPoint ? 1 : 0)
                        : (data.Data.HighValueIsPoint ? 1 : 0);
                }
                else if (column == 11)                  // 原文 :1235-1243
                {
                    for (int bo = 0; bo <= 1; bo++)
                        spec.Items.Add(CustomMagicUtils.MagicAttackDecTimeTypeNames[bo]);

                    spec.ItemIndex = data.Data.TimeAddIsPoint ? 1 : 0;
                }
                spec.HooksKeyEvents = true;
                break;

            case TVtEditKind.Edit:
                spec.Text = data.Data.HintText;         // 原文 :1257
                spec.HooksKeyEvents = true;
                break;
        }

        return spec;
    }

    /// <summary>
    /// 原文 <c>EndEdit</c>（:1007-1145）的取值/写回判定 1:1。
    /// 返回 true 表示 <c>IsChanged = True</c>（原文随后会在 Owner is TFrmCustomMagic 时调 SetConfigChanged()）。
    /// <para>
    /// 分支顺序照抄：先 <c>case FColumn of 1,2,3,5,6,8,9,10</c> 再 <c>4,7,11</c> 再 <c>13</c>；
    /// 每个字段都先比较后赋值（<c>if X &lt;&gt; TempValue then begin X := TempValue; IsChanged := True; end;</c>）。
    /// </para>
    /// </summary>
    public static bool ApplyEditorResult(int column, TAttackDecAttribData data, TVtEditorResult editor)
    {
        if (data.Data is null)
            throw new InvalidOperationException("EndEdit: DecAttribData.Data 为 nil（原文会在此解引用崩溃）");

        var d = data.Data;
        bool isChanged = false;

        switch (column)
        {
            case 1: case 2: case 3: case 5: case 6: case 8: case 9: case 10:
                {
                    int tempValue = editor.Value;
                    switch (column)
                    {
                        case 1:
                            if (d.Rate != tempValue) { d.Rate = tempValue; isChanged = true; }
                            break;
                        case 2:
                            if (d.RateAdd != tempValue) { d.RateAdd = tempValue; isChanged = true; }
                            break;
                        case 3:
                            if (d.LowValue != tempValue) { d.LowValue = tempValue; isChanged = true; }
                            break;
                        case 5:
                            if (d.LowValueAdd != tempValue) { d.LowValueAdd = tempValue; isChanged = true; }
                            break;
                        case 6:
                            if (d.HighValue != tempValue) { d.HighValue = tempValue; isChanged = true; }
                            break;
                        case 8:
                            if (d.HighValueAdd != tempValue) { d.HighValueAdd = tempValue; isChanged = true; }
                            break;
                        case 9:
                            if (d.Time != tempValue) { d.Time = tempValue; isChanged = true; }
                            break;
                        case 10:
                            if (d.TimeAdd != tempValue) { d.TimeAdd = tempValue; isChanged = true; }
                            break;
                    }
                    break;
                }

            case 4: case 7: case 11:
                {
                    int tempValue = editor.ItemIndex;
                    switch (column)
                    {
                        case 4:
                            if ((d.LowValueIsPoint ? 1 : 0) != tempValue) { d.LowValueIsPoint = tempValue == 1; isChanged = true; }
                            break;
                        case 7:
                            if ((d.HighValueIsPoint ? 1 : 0) != tempValue) { d.HighValueIsPoint = tempValue == 1; isChanged = true; }
                            break;
                        case 11:
                            if ((d.TimeAddIsPoint ? 1 : 0) != tempValue) { d.TimeAddIsPoint = tempValue == 1; isChanged = true; }
                            break;
                    }
                    break;
                }

            case 13:
                {
                    string s = editor.Text;
                    // 原文 :1125 if not SameText(...) —— SameText 大小写不敏感
                    if (!string.Equals(d.HintText, s, StringComparison.OrdinalIgnoreCase))
                    {
                        d.HintText = s;
                        isChanged = true;
                    }
                    break;
                }
        }

        return isChanged;
    }

    /// <summary>
    /// 原文 <c>SetBounds</c>（:1274-1282）：宽度取自列的 Header 右边界。
    /// 原文先 <c>FTree.Header.Columns.GetColumnBounds(FColumn, Dummy, R.Right)</c> 再 <c>FEdit.BoundsRect := R</c>。
    /// </summary>
    public static TRectSeam SetBoundsSpec(int column, TRectSeam r, IVirtualTreeHost tree)
    {
        int right = tree.GetColumnBounds(column, r.Right);
        r.Right = right;
        return r;
    }
}

/// <summary>
/// TElementPropertyEditLink（原文 :909-928 声明 / :1286-1604 实现）的纯逻辑。
/// 被 vstDecElement（:4527-4532）与 vstAddElement（同 CreateEditor 事件）用作 IVTEditLink。
/// </summary>
public static class ElementPropertyEditLinkLogic
{
    /// <summary>TSpinEditEx 分支的列集合（原文 :1506 / :1374）。</summary>
    public static readonly int[] SpinColumns = { 1, 2, 3, 5, 6, 7 };

    /// <summary>TComboBox 分支的列集合（原文 :1539 / :1429）。</summary>
    public static readonly int[] ComboColumns = { 4, 8 };

    /// <summary>TEdit 分支的列集合（原文 :1570 / :1452）。</summary>
    public static readonly int[] EditColumns = { 10 };

    /// <summary>原文 <c>PrepareEdit</c>（:1486-1587）的"建哪种编辑器"判定。</summary>
    public static TVtEditKind EditKindForColumn(int column)
    {
        if (Array.IndexOf(SpinColumns, column) >= 0) return TVtEditKind.SpinEdit;
        if (Array.IndexOf(ComboColumns, column) >= 0) return TVtEditKind.ComboBox;
        if (Array.IndexOf(EditColumns, column) >= 0) return TVtEditKind.Edit;
        return TVtEditKind.None;
    }

    /// <summary>原文 <c>PrepareEdit</c>（:1486-1587）1:1。</summary>
    public static TVtEditSpec PrepareEditSpec(int column, TMagicElementData data)
    {
        var spec = new TVtEditSpec { Kind = EditKindForColumn(column) };
        if (data.Data is null)
            throw new InvalidOperationException("PrepareEdit: MagicElementData.Data 为 nil（原文会在此解引用崩溃）");

        switch (spec.Kind)
        {
            case TVtEditKind.SpinEdit:
                if (column == 1)                       // 原文 :1514 if FColumn in [1]
                {
                    spec.HasMinMax = true;
                    spec.MinValue = 0;
                    spec.MaxValue = 100;
                }
                switch (column)                        // 原文 :1520-1533
                {
                    case 1: spec.Value = data.Data.Rate; break;
                    case 2: spec.Value = data.Data.RateAdd; break;
                    case 3: spec.Value = data.Data.Value; break;
                    case 5: spec.Value = data.Data.ValueAdd; break;
                    case 6: spec.Value = data.Data.Time; break;
                    case 7: spec.Value = data.Data.TimeAdd; break;
                }
                spec.HooksKeyEvents = true;
                break;

            case TVtEditKind.ComboBox:
                spec.DropDownList = true;
                if (column == 4)                       // 原文 :1549-1556
                {
                    for (int bo = 0; bo <= 1; bo++)
                        spec.Items.Add(CustomMagicUtils.MagicAttackDecValueTypeNames[bo]);
                    spec.ItemIndex = data.Data.ValueIsPoint ? 1 : 0;
                }
                else if (column == 8)                  // 原文 :1557-1564
                {
                    for (int bo = 0; bo <= 1; bo++)
                        spec.Items.Add(CustomMagicUtils.MagicAttackDecTimeTypeNames[bo]);
                    spec.ItemIndex = data.Data.TimeAddIsPoint ? 1 : 0;
                }
                spec.HooksKeyEvents = true;
                break;

            case TVtEditKind.Edit:
                spec.Text = data.Data.HintText;         // 原文 :1578
                spec.HooksKeyEvents = true;
                break;
        }

        return spec;
    }

    /// <summary>
    /// 原文 <c>EndEdit</c>（:1361-1475）的取值/写回判定 1:1。
    /// <para>
    /// **与 TDecAttribPropertyEditLink.EndEdit 的差异（差异断言见测试）**：
    /// 元素记录用的字段是 <c>Value/ValueAdd/ValueIsPoint</c>，而减属性记录用的是
    /// <c>LowValue/LowValueAdd/LowValueIsPoint + HighValue/HighValueAdd/HighValueIsPoint</c>；
    /// 且元素分支的列集合是 <c>1,2,3,5,6,7</c> 与 <c>4,8</c> 与 <c>10</c>，与减属性不同。
    /// </para>
    /// </summary>
    public static bool ApplyEditorResult(int column, TMagicElementData data, TVtEditorResult editor)
    {
        if (data.Data is null)
            throw new InvalidOperationException("EndEdit: MagicElementData.Data 为 nil（原文会在此解引用崩溃）");

        var d = data.Data;
        bool isChanged = false;

        switch (column)
        {
            case 1: case 2: case 3: case 5: case 6: case 7:
                {
                    int tempValue = editor.Value;
                    switch (column)
                    {
                        case 1:
                            if (d.Rate != tempValue) { d.Rate = tempValue; isChanged = true; }
                            break;
                        case 2:
                            if (d.RateAdd != tempValue) { d.RateAdd = tempValue; isChanged = true; }
                            break;
                        case 3:
                            if (d.Value != tempValue) { d.Value = tempValue; isChanged = true; }
                            break;
                        case 5:
                            if (d.ValueAdd != tempValue) { d.ValueAdd = tempValue; isChanged = true; }
                            break;
                        case 6:
                            if (d.Time != tempValue) { d.Time = tempValue; isChanged = true; }
                            break;
                        case 7:
                            if (d.TimeAdd != tempValue) { d.TimeAdd = tempValue; isChanged = true; }
                            break;
                    }
                    break;
                }

            case 4: case 8:
                {
                    int tempValue = editor.ItemIndex;
                    switch (column)
                    {
                        case 4:
                            if ((d.ValueIsPoint ? 1 : 0) != tempValue) { d.ValueIsPoint = tempValue == 1; isChanged = true; }
                            break;
                        case 8:
                            if ((d.TimeAddIsPoint ? 1 : 0) != tempValue) { d.TimeAddIsPoint = tempValue == 1; isChanged = true; }
                            break;
                    }
                    break;
                }

            case 10:
                {
                    string s = editor.Text;
                    if (!string.Equals(d.HintText, s, StringComparison.OrdinalIgnoreCase))
                    {
                        d.HintText = s;
                        isChanged = true;
                    }
                    break;
                }
        }

        return isChanged;
    }

    /// <summary>原文 <c>SetBounds</c>（:1595-1603）——与减属性版本逐字相同。</summary>
    public static TRectSeam SetBoundsSpec(int column, TRectSeam r, IVirtualTreeHost tree)
    {
        int right = tree.GetColumnBounds(column, r.Right);
        r.Right = right;
        return r;
    }
}
