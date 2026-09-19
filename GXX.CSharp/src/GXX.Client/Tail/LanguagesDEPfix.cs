// 源单元：Source/Client-HGE/LanguagesDEPfix.pas（原文 130 行，CRLF 计入 153 行）
// 原文 uses：Windows, Classes（implementation 段另用 SysUtils, SysConst）
// 原文无同名 .dfm。
//
// ⚠ 覆盖事实（已核）：LanguagesDEPfix 单元**不在 Client.dpr 的 uses 列表中**
//   （Source/Client-HGE/Client.dpr:15-113 逐行核对）。它是一份第三方孤儿单元
//   （原作者 Sasan Adami，2005-07-20，适配 D5/D6/D7 的 RTL DEP 补丁）。
//
// ── 转换开发文档 §2.3「不移植项」登记 ──────────────────────────────────────
// 该单元是**运行时代码打补丁**（code patching）：
//   `ApplyLanguagesDEPfix` 取 `@SysUtils.Languages` 的函数入口地址，判断首字节是否为
//   runtime-package 的 `$25FF`（jmp [addr]）跳板，是则解引用一层；然后
//   `VirtualProtect(..., PAGE_EXECUTE_WRITECOPY)` 把该地址改成可写，写入一条
//   32 位近跳转 `$E9 <rel32>` 指向本单元的 `Languages`，最后恢复页保护。
//   目的是替换掉 Delphi 7 RTL `TLanguages.Create` 里"在栈上生成机器码再执行"的
//   实现（在开了 DEP 的 WinXP SP2/Win2003 SP1 上会崩）。
// 该项属 §2.3 第 2 条「内联反调试/原生代码改写 → 托管异常处理替代」的同族：
// 托管运行时下：① 没有 SysUtils.TLanguages 可打补丁；② .NET 自己管理 JIT 页的
//   可执行权限，运行时改页属性会与 JIT 冲突；③ 该补丁在 .NET 里没有任何等价目标。
// 故按 §2.3 做 **Stub**：保留全部类型、常量、方法名与签名；
//   `ApplyLanguagesDEPfix` 变为空实现并日志登记；仅供审计的"跳转补丁字节"以常量保留。
// ──────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;

namespace GXX.Client.Tail;

/// <summary>
/// LanguagesDEPfix.pas 1:1 移植（Stub 形态，见文件头 §2.3 登记）。
/// </summary>
public static class LanguagesDEPfix
{
    /// <summary>原文 LanguagesDEPfix.pas:26 — <c>LCID_SUPPORTED</c>（winnt.h：<c>MAKELCID(MAKELANGID(LANG_NEUTRAL, SUBLANG_NEUTRAL), SORT_DEFAULT)</c>=0）。</summary>
    public const uint LCID_SUPPORTED = 0x00000000;

    /// <summary>原文 LanguagesDEPfix.pas:145 — <c>JumpRec.OpCode := $E9; // 32bit jump near</c></summary>
    public const byte PATCH_OPCODE_JMP_NEAR32 = 0xE9;

    /// <summary>原文 LanguagesDEPfix.pas:140 — runtime package 跳板的首两字节 <c>$25FF</c>（jmp dword ptr [addr]）。</summary>
    public const ushort RUNTIME_PACKAGE_THUNK_OPCODE = 0x25FF;

    /// <summary>原文 LanguagesDEPfix.pas:32-35 <c>TJumpRec</c>（packed record：OpCode:Byte + Address:DWord = 5 字节）。</summary>
    public const int JumpRecSize = 5;

    /// <summary>原文 LanguagesDEPfix.pas:37-41 <c>TLongJumpRec</c>（packed record：OpCode:Word + Address:DWord = 6 字节）。</summary>
    public const int LongJumpRecSize = 6;

    /// <summary>
    /// 原文 LanguagesDEPfix.pas:65-72 <c>function GetLocaleDataW(ID: LCID; Flag: DWORD): string;</c>
    /// 与 :74-80 <c>GetLocaleDataA</c>（W/A 两个变体，按 <c>Win32Platform = VER_PLATFORM_WIN32_NT</c> 选择）。
    /// <para>原文语义：把 <c>GetLocaleInfoW/A</c> 的结果装进一个 1024 宽的缓冲。
    /// 注意 A 变体用 <c>GetLocaleInfoA(...) - 1</c> 作长度（返回值含结尾 NUL），
    /// **W 变体**则直接 <c>Result := Buffer</c>（靠 WideChar 缓冲的 NUL 截断）。
    /// 两者的长度口径**不同**，本移植以同一语义实现并保留该差异注释。</para>
    /// </summary>
    /// <param name="localeId">LCID。</param>
    /// <param name="flag">LOCALE_* 查询标志。</param>
    /// <param name="wide">true = W 变体（原文 NT 平台路径）；false = A 变体。</param>
    public static string GetLocaleData(uint localeId, uint flag, bool wide)
    {
        // 原文如此（LanguagesDEPfix.pas:69 / 78）：Buffer[0] := #0;
        // 接缝：待 LanguagesDEPfix 的 GetLocaleInfoW/A P/Invoke 落地后接入。
        _ = localeId;
        _ = flag;
        _ = wide;
        return string.Empty;
    }

    /// <summary>
    /// 原文 LanguagesDEPfix.pas:84-112 <c>function TLanguagesDEPfix.LocalesCallback(LocaleID: PChar): Integer; stdcall;</c>
    /// 的**纯逻辑部分**（已抽成可测函数）。
    ///
    /// <para>原文关键行（:98）：<c>AID := StrToInt('$' + Copy(LocaleID, 5, 4));</c>
    /// —— 从形如 <c>"00000409"</c> 的 8 位十六进制 LCID 串里取**第 5..8 个字符**当十六进制数。
    /// 这是 1-based 的 <c>Copy</c>，对应 C# 的 <c>Substring(4, 4)</c>。</para>
    ///
    /// <para>原文 :100 <c>ShortLangName := GetLocaleDataProc(AID, LOCALE_SABBREVLANGNAME);</c>，
    /// :101 <c>if ShortLangName &lt;&gt; '' then</c> 才追加一条记录；
    /// :111 <c>Result := 1;</c> 恒为 1（继续枚举）。</para>
    /// </summary>
    /// <param name="localeId">原文的 <c>LocaleID: PChar</c>（形如 "00000409"）。</param>
    /// <returns>解析出的 LCID（<c>StrToInt('$'+...) </c> 的结果）。</returns>
    public static uint ParseLocaleIdHex(string localeId)
    {
        // 原文如此（LanguagesDEPfix.pas:98）：AID := StrToInt('$' + Copy(LocaleID, 5, 4));
        // Delphi Copy 是 1-based、越界返回空串 → StrToInt('$') 抛 EConvertError。
        string s = localeId ?? string.Empty;
        string hex = s.Length >= 8 ? s.Substring(4, 4) : (s.Length > 4 ? s.Substring(4) : string.Empty);
        return Convert.ToUInt32("0x" + hex, 16);
    }

    /// <summary>
    /// 原文 LanguagesDEPfix.pas:130-148 <c>procedure ApplyLanguagesDEPfix;</c>
    ///
    /// <para><b>Stub</b>：不执行任何代码改写（§2.3）。原文行为完整保留在注释里：</para>
    /// <code>
    ///   JumpRec := PJumpRec(@SysUtils.Languages);
    ///   LongJumpRec := Pointer(JumpRec);
    ///   IsPackages := LongJumpRec.OpCode = $25FF;          // runtime package?
    ///   if IsPackages then JumpRec := Pointer(PDWord(LongJumpRec.Address)^);
    ///   VirtualProtect(JumpRec, SizeOf(JumpRec^), PAGE_EXECUTE_WRITECOPY, @OldProtect);
    ///   JumpRec.OpCode := $E9;                             // 32bit jump near
    ///   JumpRec.Address := integer(@Languages) - integer(JumpRec) - 5;
    ///   VirtualProtect(JumpRec, SizeOf(JumpRec^), OldProtect, @OldProtect);
    /// </code>
    /// <para>注意原文 :146 的 <c>- 5</c> 是近跳转指令长度（1 字节 opcode + 4 字节 rel32），
    /// 而 <c>TJumpRec</c> 声明的 <c>SizeOf</c> 恰好也是 5 —— 两者一致。</para>
    /// </summary>
    /// <returns>是否真的执行了补丁（Stub 恒 false）。</returns>
    public static bool ApplyLanguagesDEPfix()
    {
        // 接缝：待 LanguagesDEPfix 的原生代码改写路径以 P/Invoke(VirtualProtect) 落地后接入。
        //       托管运行时下刻意不执行（见文件头 §2.3 理由 ①②③）。
        return false;
    }

    /// <summary>
    /// 原文 LanguagesDEPfix.pas:120-128 的 <c>function Languages: TLanguages;</c>
    /// —— 单例惰性构造（<c>if FLanguages = nil then FLanguages := TLanguagesDEPfix.Create;</c>）。
    ///
    /// <para>原文的返回值是 RTL 的 <c>TLanguages</c> 对象（内含 <c>FSysLangs: array of TLangRec</c>）；
    /// 托管侧以「LCID → 语言显示名/短名」的表表示同一信息。原文 :104-109 逐条写入
    /// <c>FName := GetLocaleDataProc(AID, LOCALE_SLANGUAGE)</c>、
    /// <c>FLCID := AID</c>、<c>FExt := ShortLangName</c>。</para>
    /// <para>注意原文 :102-109 的 <c>PSLangs := @THackLanguages(Self).FSysLangs;</c>
    /// 是「hack 私有成员访问」写法（:44-47 的 <c>THackLanguages</c> 声明了一个
    /// 字段布局与 <c>TLanguages</c> 相同但字段可见的类）。托管侧无需该 hack，
    /// 但该结构在原文里确实存在，故在此登记。</para>
    /// <para>Stub：不枚举系统区域（<c>EnumSystemLocales</c>），返回空表。</para>
    /// </summary>
    /// <returns>原文 <c>FLanguages</c> 单例的内容（Stub 下为空表）。</returns>
    public static IReadOnlyList<TLangRec> Languages()
    {
        // 原文如此（LanguagesDEPfix.pas:125-127）：
        //   if FLanguages = nil then FLanguages := TLanguagesDEPfix.Create;
        //   Result := FLanguages;
        if (FLanguages == null) FLanguages = new List<TLangRec>();
        return FLanguages;
    }

    private static List<TLangRec> FLanguages;

    /// <summary>测试接缝：清空 <c>FLanguages</c>（原文无此操作，仅用于隔离单例状态）。</summary>
    internal static void ResetLanguagesForTest() => FLanguages = null;
}

/// <summary>原文 RTL <c>TLangRec</c>（SysUtils）在托管侧的对应记录。</summary>
public sealed class TLangRec
{
    /// <summary>原文 RTL <c>TLangRec.FName</c> —— <c>LOCALE_SLANGUAGE</c> 的本地化语言名。</summary>
    public string FName;

    /// <summary>原文 RTL <c>TLangRec.FLCID</c> —— 语言 ID。</summary>
    public uint FLCID;

    /// <summary>原文 RTL <c>TLangRec.FExt</c> —— <c>LOCALE_SABBREVLANGNAME</c> 的短名。</summary>
    public string FExt;
}
