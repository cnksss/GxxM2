using System;
using System.IO;

namespace GXX.Client.DxComponent;

// =====================================================================================
// DxControlClpbrd.pas（166 行，源：Source\Client-HGE\DxComponent\DxControlClpbrd.pas）
//
// ★ 原文核实结论（**重要发现**）：本单元是兄弟单元 `StreamClipbrd.pas` 的
//   **严格前缀重复 + 未被任何单元/工程引用**：
//
//   1. 逐行比对（归一化行尾后，已实测）：
//        * 接口段：`StreamClipbrd.pas 6-11` ≡ `DxControlClpbrd.pas 6-11`
//          —— 6 个过程声明逐字相同（仅 unit 名与 StreamClipbrd 多出的 13-14 两行不同）。
//        * 实现段：`StreamClipbrd.pas 15-167` ≡ `DxControlClpbrd.pas 12-164`
//          —— **153 行、0 处差异**（含 `{ CopyStreamToClipboard }` 尾注释与 73-74 行英文消息）。
//   2. `DxControlClpbrd` **不在** `GuiEdit.dpr` 的 uses 列表里，全树也没有任何
//      `uses ... DxControlClpbrd`；而 `StreamClipbrd` 被 `Main.pas:591/606/629/637/2085/2101`
//      与 `Structure.pas:243/253/270` 实际调用。
//      → `DxControlClpbrd.pas` 是**死代码**（早期版本被 StreamClipbrd 取代后未删）。
//   3. `StreamClipbrd.pas` 在相同 6 个过程之外还多了
//      `const CF_MYFORMAT = 55555` + `StreamSaveToClipboard` + `StreamLoadFromClipboard`
//      （原文 170-217）。
//
//   托管侧处置（**不复制粘贴 130 行重复实现**）：
//   本类保留原文的 6 个**过程名与签名**（1:1 对外契约），实现**转发**到
//   `StreamClipbrd`（同名同序，语义逐字等价）。这样：
//     * 依赖 `DxControlClpbrd` 名字的下游代码仍可编译；
//     * 修改只落一处，不会出现"两份实现走偏"的经典事故；
//     * 全树唯一实现仍是 1:1 的 `StreamClipbrd`（含其接缝与偏差登记）。
//
//   源单元行号范围（转发目标见 StreamClipbrd.cs）：
//     * 1-11    unit 头 + interface uses + 6 个过程声明
//     * 14-48   CopyStreamToClipboard        → StreamClipbrd.pas 17-51
//     * 50-73   CopyStreamFromClipboard      → StreamClipbrd.pas 53-76
//     * 75-96   SaveClipboardFormat          → StreamClipbrd.pas 78-99
//     * 98-120  LoadClipboardFormat          → StreamClipbrd.pas 101-123
//     * 122-142 SaveClipboard                → StreamClipbrd.pas 125-145
//     * 144-164 LoadClipboard                → StreamClipbrd.pas 147-167
//
// DFM: 无。
//
// 唯一的签名差异：原文 `SaveClipboardFormat(fmt: Word; writer: TWriter)` 的形参名
// 在本类里保持 `fmt: ushort`（即 Word），与 `StreamClipbrd.SaveClipboardFormat` 相同。
// =====================================================================================

/// <summary>
/// DxControlClpbrd.pas（166 行）的 6 个单元级过程。**原文核实为 StreamClipbrd.pas 的
/// 未被引用的前缀重复**（见文件头），故本类全部转发到 <see cref="StreamClipbrd"/>。
/// </summary>
public static class DxControlClpbrd
{
    /// <summary>原文 14-48 <c>CopyStreamToClipboard(fmt: Cardinal; S: TStream)</c>。</summary>
    public static void CopyStreamToClipboard(uint fmt, Stream s) => StreamClipbrd.CopyStreamToClipboard(fmt, s);

    /// <summary>原文 50-73 <c>CopyStreamFromClipboard(fmt: Cardinal; S: TStream)</c>。</summary>
    public static void CopyStreamFromClipboard(uint fmt, Stream s) => StreamClipbrd.CopyStreamFromClipboard(fmt, s);

    /// <summary>原文 75-96 <c>SaveClipboardFormat(fmt: Word; writer: TWriter)</c>。</summary>
    public static void SaveClipboardFormat(ushort fmt, IDxWriter writer) => StreamClipbrd.SaveClipboardFormat(fmt, writer);

    /// <summary>原文 98-120 <c>LoadClipboardFormat(reader: TReader)</c>。</summary>
    public static void LoadClipboardFormat(IDxReader reader) => StreamClipbrd.LoadClipboardFormat(reader);

    /// <summary>原文 122-142 <c>SaveClipboard(S: TStream)</c>。</summary>
    public static void SaveClipboard(Stream s) => StreamClipbrd.SaveClipboard(s);

    /// <summary>原文 144-164 <c>LoadClipboard(S: TStream)</c>。</summary>
    public static void LoadClipboard(Stream s) => StreamClipbrd.LoadClipboard(s);
}
