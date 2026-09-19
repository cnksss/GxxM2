using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace GXX.Client.DxComponent;

// =====================================================================================
// GfxFonts.pas（963 行）1:1 移植。
//
//   * 9-27      TGfxFontTexture（单条文本缓存：纹理 + 文本 + 两个超时参数 + 落点 X/Y）
//   * 29        TGfxFontTextureArray = array of TGfxFontTexture
//   * 31-51     TGfxFontTextures（文本 → 纹理的缓存表：Add / Clear / FreeIdleMemory / 索引访问）
//   * 54-120    TGfxTextureFont = class(TFont)（字形度量 + TextOut/TextRect 族）
//   * 122-146   TGfxTextureFonts（字体表：Add/SetFont/RemoveFont/Lock/UnLock/…）
//   * 147-154   单元级全局变量 GfxTextureFont / GfxTextureFonts 与 4 个宽字符常量
//   * 158-963   实现段
//
// -------------------------------------------------------------------------------------
// 托管侧落点与偏差（逐条登记）：
//
//   1. **不移植 GDI 光栅化**（GetFontTexture 504-644 / NewBitmapFile 446-503 /
//      GetFontSize 398-434 / DrawText 351-396 / TextOut* 646-687 / TextRect* 688-750）：
//      这些函数创建 DIB section、调 Windows.TextOut 画字、再 NewTexture 上传。
//      属未移植的 HGECanvas/HGE 域，本波以接缝形态呈现：
//        * `TDxGfxTextureFont.GetFontSize` → 接缝委托 `FontSizeProbe`（缺省保留原文 295-301 的初始常量）
//        * `TDxGfxTextureFont.BuildGlyphTexture` → 接缝委托 `GlyphBuilder`（缺省返回 null，即"造不出纹理"）
//      原文 **纯逻辑部分全部落地**（见下），光栅化部分留给接入 HGE 的批次。
//
//   2. `TGfxFontTextures.FList : THashTable`（Source/Common/HashTable.pas 19-61）→
//      `Dictionary<string, TDxGfxFontTexture>` + 显式**保序索引表**。
//      原因：原文的 `FList.Items[I]` / `FList.Delete(nIdx)` 是**按加入顺序的索引访问**，
//      而 Dictionary 不保序；`FreeIdleMemory` 的 `FProcIdx` 游标语义依赖该顺序。
//      哈希键：原文 `Add(Text, Text, Texture)` → `HashOf(Name) = CRC16(Name)`，
//      装载因子 `mod Length(Buckets)`（构造传 1000）；`Find` 的比较是
//      `Result^.Key = Name` —— Delphi 的 AnsiString `=` 是**区分大小写**的，
//      故托管侧用 `StringComparer.Ordinal`（**不做** 不区分大小写）。
//
//   3. `TGfxTextureFont = class(TFont)` 的 `Name/Size/Charset/Style` 四个 TFont 属性 →
//      托管侧落在 `TDxGfxFontDescriptor`（Name/Size/Charset/Style/Bold）。
//      原文 `TextWidth` 里判的是 `fsBold in Style`（**不是** TDxFont.Bold），
//      托管侧以 `Descriptor.StyleBold` 表达，语义与原文一致。
//
//   4. `nsCount := Length(Text)`（AnsiString 字节数）vs `nwCount := Length(sText)`
//      （WideString 码元数）→ 托管侧 nsCount = GBK 字节数（`GXX.Core.EncodingInit.GBK`），
//      nwCount = `text.Length`。沿用上一波 `TDxFontEnv.MeasureTextWidth` 的同一处理。
//
//   5. `TextChars = [#32..#255]`（原文 319）：Delphi **AnsiChar** 集合，
//      即"ASCII 空格到 Latin-1 的补集中的可打印 ANSI 字符"。
//      `cChar := S[1]` 中的 `S := sText[I]` 是 WideString 单元素赋给 AnsiString →
//      按系统 ANSI 代码页（GBK）转换：**单字节字符**保留为 1 字节，**非 ANSI 字符**（如汉字）
//      转换后会得到 '?'（$3F = 63，**落在 [#32..#255] 内**）。
//      故原文的行为是：汉字走 `GetFontTexture('?')` 分支，**只占 1 个结果槽位**；
//      原文 342-346 那段 `if Length(S) = 2 then +2 else +1` 因为上面这层转换**恒走 +1**——
//      原文冗余，逐字保留结构并注释。
//
//   6. `GetTickCount` → `DxTickCount.MyGetTickCount`（可注入，便于超时淘汰的单测）。
// =====================================================================================

/// <summary>
/// GfxFonts.pas 9-27 TGfxFontTexture：一条「文本 → 纹理」缓存记录。
/// 两个超时参数（`FOutTimeTick` 上次访问时刻 / `FOutTimeTime` 存活毫秒）
/// 由 `TDxGfxFontTextures.FreeIdleMemory` 用于淘汰。
/// </summary>
public sealed class TDxGfxFontTexture : IDisposable
{
    /// <summary>原文 FTexture（TTexture）→ 接缝 IDxTexture。</summary>
    public IDxTexture Texture;

    /// <summary>原文 FText（该纹理对应的文本）。</summary>
    public string Text = "";

    /// <summary>原文 FOutTimeTick（默认 GetTickCount，见构造 164）。</summary>
    public uint OutTimeTick;

    /// <summary>原文 FOutTimeTime（默认 1000 * 60 * 1 = 60000，见构造 165）。</summary>
    public uint OutTimeTime = 1000 * 60 * 1;

    /// <summary>原文 FX（纹理落点 X）。</summary>
    public int X;

    /// <summary>原文 FY（纹理落点 Y）。</summary>
    public int Y;

    /// <summary>原文 158-166 TGfxFontTexture.Create 逐项。</summary>
    public TDxGfxFontTexture()
    {
        Texture = null;
        Text = "";
        // 原文 163 `//FStyle := [];`（被注释掉的字段，照抄留档）
        OutTimeTick = DxTickCount.MyGetTickCount();
        OutTimeTime = 1000 * 60 * 1;
    }

    /// <summary>原文 168-173 Destroy（FTexture 非 nil 时 FreeAndNil）。</summary>
    public void Dispose() => Texture = null;
}

/// <summary>
/// GfxFonts.pas 31-51 / 176-263 TGfxFontTextures：一条字体下的「文本 → 纹理」缓存表。
///
/// 托管侧用 `Dictionary` + **保序索引表**（原文 `FList.Items[I]` 是按加入顺序索引，
/// 而 `FreeIdleMemory` 的游标推进依赖该顺序）；见文件头第 2 条。
/// </summary>
public sealed class TDxGfxFontTextures
{
    // 原文 FList:THashTable（THashTable.Create(1000)），键比较为**区分大小写**（AnsiString `=`）
    private readonly Dictionary<string, TDxGfxFontTexture> _map = new(StringComparer.Ordinal);

    // 原文 FList.Items[I] / Delete(nIdx) 的保序镜像
    private readonly List<TDxGfxFontTexture> _ordered = new();

    private uint _outTimeTick;
    private uint _outTimeTime;
    private int _procIdx;

    /// <summary>原文 176-183 TGfxFontTextures.Create 逐项（含 `FList := THashTable.Create(1000)`）。</summary>
    public TDxGfxFontTextures()
    {
        _outTimeTick = DxTickCount.MyGetTickCount();
        _outTimeTime = 1000;
        _procIdx = 0;
    }

    /// <summary>原文 46 property List（测试只读视图）。</summary>
    public IReadOnlyDictionary<string, TDxGfxFontTexture> List => _map;

    /// <summary>原文 48 property TextureCount（= FList.Count）。</summary>
    public int TextureCount => _ordered.Count;

    /// <summary>原文 47 property Textures[Index]（越界返回 nil，见 254-258）。</summary>
    public TDxGfxFontTexture GetTexture(int index)
        => (index >= 0 && index < _ordered.Count) ? _ordered[index] : null;

    /// <summary>原文 49 property OutTimeTick。</summary>
    public uint OutTimeTick { get => _outTimeTick; set => _outTimeTick = value; }

    /// <summary>原文 50 property OutTimeTime。</summary>
    public uint OutTimeTime { get => _outTimeTime; set => _outTimeTime = value; }

    /// <summary>原文 FProcIdx（测试可读：FreeIdleMemory 的分片游标）。</summary>
    public int ProcIdx => _procIdx;

    /// <summary>原文 199-202 TGfxFontTextures.Add（`FList.Add(Text, Text, Texture)`）。</summary>
    public void Add(string text, TDxGfxFontTexture texture)
    {
        _map[text] = texture;
        _ordered.Add(texture);
    }

    /// <summary>原文 204-216 TGfxFontTextures.Clear（先逐项 Free 再 Clear，并把 FProcIdx 归零）。</summary>
    public void Clear()
    {
        _procIdx = 0;
        for (int i = 0; i < _ordered.Count; i++)
        {
            _ordered[i]?.Dispose();
        }
        _map.Clear();
        _ordered.Clear();
    }

    /// <summary>原文 254-258 GetTexture 的**按键查表**（`FFontTextures.List.Datas[Text]`）。
    /// 原文用 `THashTable.GetData`：命中返回 Data（即 TGfxFontTexture），未命中返回 nil。</summary>
    public TDxGfxFontTexture Find(string text)
        => _map.TryGetValue(text, out var t) ? t : null;

    /// <summary>
    /// 原文 218-252 TGfxFontTextures.FreeIdleMemory 1:1。
    /// 注意三处原文怪癖：
    ///   ① 外层门控 `GetTickCount - FOutTimeTick > FOutTimeTime` 才动作，动作时先把 FOutTimeTick 刷成当前时刻；
    ///   ② 内层 `while True` 从 `FProcIdx` 起扫，超时项 `FreeAndNil + Delete(nIdx)` 后 **`Inc(nIdx)` 再 `Continue`**
    ///      —— 删除后同一索引已是下一项，这一 `Inc` 会**跳过一个元素**（原文如此）；
    ///   ③ 每轮判 `GetTickCount - dwTimeTick > 10` → 记 `FProcIdx := nIdx` 并 Break（分片扫描）；
    ///      若整表扫完（`FList.Count <= nIdx`）则 `boCheckTimeLimit` 保持 False → `FProcIdx := 0`（下轮从头扫）。
    /// 返回本轮回收到期的纹理条数（原文无返回值，此处仅为测试可观测性）。
    /// </summary>
    public int FreeIdleMemory()
    {
        int freed = 0;
        if (DxTickCount.MyGetTickCount() - _outTimeTick > _outTimeTime)
        {
            _outTimeTick = DxTickCount.MyGetTickCount();
            uint dwTimeTick = DxTickCount.MyGetTickCount();

            int nIdx = _procIdx;
            bool boCheckTimeLimit = false;
            while (true)
            {
                if (_ordered.Count <= nIdx) break;
                var texture = _ordered[nIdx];
                if (texture != null)
                {
                    if (DxTickCount.MyGetTickCount() - texture.OutTimeTick > texture.OutTimeTime)
                    {
                        texture.Dispose();
                        RemoveOrderedAndMap(nIdx);
                        freed++;
                        nIdx++;
                        continue;                 // 原文 239：删除后仍 Inc 再 Continue（会跳过一项）
                    }
                }

                nIdx++;
                if (DxTickCount.MyGetTickCount() - dwTimeTick > 10)
                {
                    boCheckTimeLimit = true;
                    _procIdx = nIdx;
                    break;
                }
            }
            if (!boCheckTimeLimit) _procIdx = 0;
        }
        return freed;
    }

    private void RemoveOrderedAndMap(int index)
    {
        var texture = _ordered[index];
        _ordered.RemoveAt(index);
        if (texture != null && _map.TryGetValue(texture.Text, out var cur) && ReferenceEquals(cur, texture))
            _map.Remove(texture.Text);
    }

    /// <summary>原文 190-197 Destroy（逐项 FreeAndNil 后释放表）。</summary>
    public void Dispose()
    {
        for (int i = 0; i < _ordered.Count; i++)
            _ordered[i]?.Dispose();
        _map.Clear();
        _ordered.Clear();
    }
}

/// <summary>
/// GfxFonts.pas 54-120 的 `TGfxTextureFont = class(TFont)` 里
/// **TFont 自带的四个属性**的托管落点：
/// `Name`（默认 '宋体'）/ `Size`（默认 9）/ `Charset`（默认 GB2312_CHARSET = 134）/
/// `Style`（默认 []，原文只用 `fsBold in Style` 这一个判定）。
/// </summary>
public sealed class TDxGfxFontDescriptor
{
    /// <summary>原文 291 `Name := '宋体'`。</summary>
    public string Name = "宋体";

    /// <summary>原文 292 `Size := 9`。</summary>
    public int Size = 9;

    /// <summary>原文 293 `Charset := GB2312_CHARSET`（Windows.Gb2312_charset = 134）。</summary>
    public int Charset = 134;

    /// <summary>
    /// 原文 294 `Style := []`，以及 787/793 的 `fsBold in Style` 判定。
    /// 原文 GetFontSize（423）与 TextWidth 都只看 Bold 这一位，故用单个布尔表达。
    /// </summary>
    public bool StyleBold;
}

/// <summary>
/// GfxFonts.pas 54-120 / 287-798 TGfxTextureFont：一条字体（字形度量 + 文本缓存）。
///
/// **已落地的纯逻辑**（可单测）：
///   * 构造默认度量常量（295-301）
///   * `TextHeight`（766-776）
///   * `TextWidth`（778-798，含 nsCount/nwCount 双字节折算与 Bold 分支）
///   * `GetFontTextureArray`（321-349，含 TextChars 门控与结果槽位数规则）
///   * `Clear` / `Initialize` / `Finalize` / `FreeIdleMemory`（313-316 / 751-764）
///   * `GetTextTexture`（436-444）
///   * `GetFontTexture` 的**缓存查询与几何计算**部分（536-575）：命中即刷新 OutTimeTick 并返回；
///     未命中则按 `Max(TextWidth(每行))` 与 `TextHeight('pP') * 行数` 算 nWidth/nHeight。
/// **接缝化**（不做 GDI 光栅化，见文件头第 1 条）：
///   * `GetFontSize` → `FontSizeProbe`
///   * `ProvideGlyphTexture`（原文 613-623：`Texture.Texture := NewTexture(...)` 之后建档并入库）
/// </summary>
public class TDxGfxTextureFont : IDisposable
{
    /// <summary>原文 75 `nIndex:Integer`（public 字段）。</summary>
    public int nIndex;

    /// <summary>原文 56 FOwner（TGfxTextureFonts）。</summary>
    public TDxGfxTextureFonts Owner;

    /// <summary>原文 Name/Size/Charset/Style（TFont 属性）。</summary>
    public readonly TDxGfxFontDescriptor Descriptor = new();

    /// <summary>原文 57-64 的度量字段（默认值见构造 295-301）。</summary>
    public int FontWidth = 6;

    /// <summary>原文 FFontHeight（默认 12）。</summary>
    public int FontHeight = 12;

    /// <summary>原文 FFontWidthBold（默认 7）。</summary>
    public int FontWidthBold = 7;

    /// <summary>原文 FDoubleFontWidth（默认 12）。</summary>
    public int DoubleFontWidth = 12;

    /// <summary>原文 FDoubleFontHeight（默认 12）。</summary>
    public int DoubleFontHeight = 12;

    /// <summary>原文 FDoubleFontWidthBold（默认 13）。</summary>
    public int DoubleFontWidthBold = 13;

    /// <summary>原文 FTimeOutIdx（默认 0）。</summary>
    public int TimeOutIdx;

    /// <summary>原文 66 FFontTextures:TGfxFontTextures。</summary>
    public readonly TDxGfxFontTextures FontTextures;

    /// <summary>
    /// 接缝：原文 398-434 `GetFontSize` 用 GDI `GetTextExtentPoint32W` 量 '0' 与 '一' 的宽高
    /// （分别得到 FontWidth/FontHeight 与 DoubleFontWidth/DoubleFontHeight，Bold 时再量一次
    /// FontWidthBold/DoubleFontWidthBold）。缺省 null = 保留构造里的初始常量。
    /// 传入的实现应当把量到的值写回本对象的对应字段。
    /// </summary>
    public Action<TDxGfxTextureFont> FontSizeProbe;

    /// <summary>
    /// 接缝：原文 613-617 `Texture := TGfxFontTexture.Create; Texture.Text := Text;
    /// Texture.Texture := NewTexture(PBitmapBits, nWidth, nHeight);`。
    /// 返回 null 表示造不出纹理（原文 624-628 会 FreeAndNil(Texture) 并返回 nil）。
    /// </summary>
    public Func<int, int, IDxTexture> GlyphBuilder;

    /// <summary>原文 287-305 TGfxTextureFont.Create 逐项。</summary>
    public TDxGfxTextureFont(TDxGfxTextureFonts owner)
    {
        Owner = owner;
        // 原文 291-294：Name/Size/Charset/Style 的默认值在 TDxGfxFontDescriptor 里
        FontWidth = 6;
        FontHeight = 12;
        FontWidthBold = 7;
        DoubleFontWidth = 12;
        DoubleFontHeight = 12;
        DoubleFontWidthBold = 13;

        FontTextures = new TDxGfxFontTextures();
        TimeOutIdx = 0;
    }

    /// <summary>原文 307-311 Destroy。</summary>
    public void Dispose() => FontTextures?.Dispose();

    /// <summary>原文 313-316 Clear。</summary>
    public void Clear() => FontTextures.Clear();

    /// <summary>原文 756-759 Initialize（原文只调 FFontTextures.Clear）。</summary>
    public void Initialize() => FontTextures.Clear();

    /// <summary>原文 761-764 Finalize（同 Initialize）。</summary>
    public void Finalize2() => FontTextures.Clear();

    /// <summary>原文 751-754 FreeIdleMemory（转调缓存表的同名方法）。返回回收条数（原文无返回值）。</summary>
    public int FreeIdleMemory() => FontTextures.FreeIdleMemory();

    /// <summary>原文 398-434 GetFontSize 的接缝入口（原文会改 Style 再复原，此处由注入实现自行处理）。</summary>
    public void GetFontSize() => FontSizeProbe?.Invoke(this);

    /// <summary>
    /// GfxFonts.pas 766-776 TextHeight 1:1：
    /// `if Length(Text) = Length(WideString(Text))`（即纯单字节）→ `FFontHeight`，
    /// 否则 `Max(FFontHeight, FDoubleFontHeight)`。
    /// </summary>
    public int TextHeight(string text)
    {
        int nsCount = string.IsNullOrEmpty(text) ? 0 : GXX.Core.EncodingInit.GBK.GetByteCount(text);
        int nwCount = text?.Length ?? 0;
        if (nsCount == nwCount)
        {
            return FontHeight;
        }
        return Math.Max(FontHeight, DoubleFontHeight);
    }

    /// <summary>
    /// GfxFonts.pas 778-798 TextWidth 1:1：
    /// 纯单字节 → `(fsBold in Style ? FFontWidthBold : FFontWidth) * Length(Text)`；
    /// 含双字节 → `nCount := nsCount - nwCount`（双字节**字符数**），
    /// 结果 = `双字节宽 * Max(nCount, 0) + Max(nwCount - nCount, 0) * 单字节宽`（Bold 时取各自的 Bold 宽）。
    /// </summary>
    public int TextWidth(string text)
    {
        if (string.IsNullOrEmpty(text)) return 0;

        int nsCount = GXX.Core.EncodingInit.GBK.GetByteCount(text);
        int nwCount = text.Length;

        if (nsCount == nwCount)
        {
            return Descriptor.StyleBold
                ? FontWidthBold * text.Length
                : FontWidth * text.Length;
        }

        int nCount = nsCount - nwCount;              // 双字节字符数
        if (Descriptor.StyleBold)
        {
            return DoubleFontWidthBold * Math.Max(nCount, 0)
                 + Math.Max(nwCount - nCount, 0) * FontWidthBold;
        }
        return DoubleFontWidth * Math.Max(nCount, 0)
             + Math.Max(nwCount - nCount, 0) * FontWidth;
    }

    /// <summary>
    /// GfxFonts.pas 321-349 GetFontTextureArray 1:1（**结构照抄，含原文的冗余分支**）。
    /// 返回数组长度 = 逐个 WideChar 累加的槽位数（见文件头第 5 条：
    /// `S := sText[I]` 赋给 AnsiString 后，非 ANSI 字符变成 '?'，故 `Length(S)` 恒为 1，
    /// 原文 342-346 的 `if Length(S) = 2 then +2 else +1` **恒走 +1**）。
    /// 命中 `TextChars` 的字符走 `GetFontTexture`，拿到 nil 时**不加槽位**。
    /// 返回值里 nil 表示"该字符无纹理（skip 时用 TextWidth('0') 近似）"。
    /// </summary>
    public List<TDxGfxFontTexture> GetFontTextureArray(string text)
    {
        var result = new List<TDxGfxFontTexture>();
        if (text == "") return result;

        for (int i = 0; i < text.Length; i++)
        {
            // 原文 332-334：`S := sText[I]; cChar := S[1];`
            // WideChar → AnsiString（GBK）：单字节字符原样保留，非 ANSI 字符变 '?'
            string s = WideCharToAnsiString(text[i]);
            char cChar = s.Length > 0 ? s[0] : '\0';

            if (IsTextChar(cChar))
            {
                var fontTexture = GetFontTexture(s);
                if (fontTexture != null)
                {
                    result.Add(fontTexture);
                }
            }
            else
            {
                // 原文 341-347：非 TextChars 字符 → 只预留槽位（不放纹理）。
                // `Length(S)` 在上面的 GBK 转换下恒为 1，故这里恒走 +1；原文的 `= 2` 分支
                // 在当前转换语义下不可达 —— 逐字保留结构。
                if (s.Length == 2)
                {
                    result.Add(null);
                    result.Add(null);
                }
                else
                {
                    result.Add(null);
                }
            }
        }
        return result;
    }

    /// <summary>
    /// 原文 319 `TextChars = [#32..#255]` —— Delphi **AnsiChar** 集合，
    /// 即"字节值 32..255"。托管侧此处按**字符码点**判定（与原文"按字节"在纯 ASCII 下等价；
    /// 汉字经 GBK 转换已变成 '?'，见文件头第 5 条）。
    /// </summary>
    public const char TextCharsFirst = (char)32;

    /// <summary>原文 TextChars 的上界 255。</summary>
    public const char TextCharsLast = (char)255;

    /// <summary>原文 `cChar in TextChars`。</summary>
    public static bool IsTextChar(char c) => c >= TextCharsFirst && c <= TextCharsLast;

    /// <summary>
    /// 原文 `S := sText[I]`（WideChar → AnsiString，GBK）：单字节字符保留，
    /// 非 ANSI 码位（含汉字）转成 '?'（$3F）。这是理解原文 341-347 那段的关键。
    /// </summary>
    public static string WideCharToAnsiString(char c)
    {
        if (c <= 0xFF)
        {
            // 单字节范围：GBK 路径下 0x00-0x7F 原样；0x80-0xFF 在 GBK 里是双字节首字节的孤立值，
            // 无法单独成字 → 走下面的编码+回退路径（会得到 '?'）
            if (c <= 0x7F) return c.ToString();
        }

        var bytes = GXX.Core.EncodingInit.GBK.GetBytes(c.ToString());
        // Delphi 的 WideChar→AnsiString 单元素转换：不能表示时给 '?'
        if (bytes.Length == 1) return ((char)bytes[0]).ToString();
        if (bytes.Length == 2)
        {
            // 双字节 GBK 无法装进单元素 AnsiString → 转换结果是 '?'
            return "?";
        }
        return "?";
    }

    /// <summary>
    /// GfxFonts.pas 504-644 GetFontTexture 的**缓存与几何部分**（1:1），
    /// 光栅化部分接缝化（见文件头第 1 条）。
    ///
    /// 语义：
    ///   ① `if Text = '' then Exit`（返回 nil）；
    ///   ② `FontTexture := FFontTextures.List.Datas[Text]`（**区分大小写**）；
    ///      命中 → 刷新 `OutTimeTick := GetTickCount` 并**直接返回**（不重算几何）；
    ///   ③ 未命中 → `TextList.Text := Text` 按行拆分；
    ///      `nWidth := Max(每行 TextWidth)`、`nHeight := TextHeight('pP') * 行数`；
    ///   ④ 交给 `GlyphBuilder(nWidth, nHeight)`；得到 nil → 返回 nil（原文 624-628 FreeAndNil）；
    ///      否则建 `TGfxFontTexture`（Text 落值）、入库、返回。
    /// </summary>
    public TDxGfxFontTexture GetFontTexture(string text)
    {
        if (text == "") return null;

        var cached = FontTextures.Find(text);
        if (cached != null)
        {
            cached.OutTimeTick = DxTickCount.MyGetTickCount();
            return cached;
        }

        // 原文 553-561（被注释掉的线性查找）照抄留档：
        //   {for I := 0 to FFontTextures.TextureCount - 1 do begin
        //     FontTexture := FFontTextures.Textures[I];
        //     if (FontTexture.Style = FontStyles) and (CompareStr(FontTexture.Text, Text) = 0) then begin
        //       FontTexture.OutTimeTick := GetTickCount;
        //       Result := FontTexture;
        //       Exit;
        //     end;
        //   end;}

        var lines = TDxFontEnv.SplitTextLines(text);

        int nWidth = 0;
        for (int i = 0; i < lines.Count; i++)
        {
            nWidth = Math.Max(nWidth, TextWidth(lines[i]));
        }

        // 原文 575：`nHeight := TextHeight('pP') * TextList.Count;`
        int nHeight = TextHeight("pP") * lines.Count;

        var built = GlyphBuilder?.Invoke(nWidth, nHeight);
        if (built == null) return null;              // 原文 624-628

        var record = new TDxGfxFontTexture { Text = text, Texture = built };
        FontTextures.Add(text, record);
        return record;
    }

    /// <summary>
    /// 原文 504-644 里"未命中时算出的目标位图尺寸"（测试可读的纯几何产物）。
    /// 顺序与原文 567-575 一致：先逐行取 `Max(TextWidth)`，再 `TextHeight('pP') * 行数`。
    /// </summary>
    public TDxPoint ComputeGlyphBitmapSize(string text)
    {
        var lines = TDxFontEnv.SplitTextLines(text);
        int nWidth = 0;
        for (int i = 0; i < lines.Count; i++)
        {
            nWidth = Math.Max(nWidth, TextWidth(lines[i]));
        }
        return new TDxPoint(nWidth, TextHeight("pP") * lines.Count);
    }

    /// <summary>原文 436-444 GetTextTexture。</summary>
    public IDxTexture GetTextTexture(string text)
    {
        var fontTexture = GetFontTexture(text);
        return fontTexture?.Texture;
    }
}

/// <summary>
/// GfxFonts.pas 122-146 / 801-963 TGfxTextureFonts：字体表。
/// 原文的 `Fonts: array of TGfxTextureFont`（1-based 语义的 `Font[Num]`）+ TRTLCriticalSection。
/// 本波落纯逻辑：Add / SetFont / RemoveFont / RemoveAll / Lock / UnLock / Count / Font[Num]。
/// </summary>
public class TDxGfxTextureFonts
{
    private readonly List<TDxGfxTextureFont> _fonts = new();
    private readonly object _gate = new();

    /// <summary>原文 126 property D3DFormat。</summary>
    public bool D3DFormat { get; set; }

    /// <summary>原文 128-130 GetCount。</summary>
    public int Count => _fonts.Count;

    /// <summary>原文 129-130 GetFont(Num)（**不判越界**，原文直接索引；托管侧返回 null 以保安全）。</summary>
    public TDxGfxTextureFont GetFont(int num)
        => (num >= 0 && num < _fonts.Count) ? _fonts[num] : null;

    /// <summary>原文 147-149 的 `GfxTextureFonts` 单例式全局变量在本类的静态落点。</summary>
    public static TDxGfxTextureFonts Global;

    /// <summary>原文 147 的 `GfxTextureFont`（"当前字体"）在本类的静态落点。</summary>
    public static TDxGfxTextureFont GlobalCurrent;

    /// <summary>原文 138-141 Lock / UnLock（TRTLCriticalSection → Monitor）。</summary>
    public void Lock() => System.Threading.Monitor.Enter(_gate);

    /// <summary>原文 138-141 UnLock。</summary>
    public void UnLock() => System.Threading.Monitor.Exit(_gate);

    /// <summary>
    /// 原文 844-862 RemoveAll（逐项 Free 后 `SetLength(Fonts, 0)`）。
    /// </summary>
    public void RemoveAll()
    {
        lock (_gate)
        {
            for (int i = 0; i < _fonts.Count; i++)
                _fonts[i]?.Dispose();
            _fonts.Clear();
        }
    }

    /// <summary>
    /// 原文 889-908 RemoveFont(Num)：**先 `Fonts[Num].Free` 再 `Fonts[Num] := nil`**
    /// —— 原文那句 `Fonts[Num] := nil` 是**死写**（后面紧接着 `Dec(nIndex)` 与搬移循环，
    /// 会立刻被覆盖）；托管侧逐字保留"释放 + 留 null 占位"的等价结构。
    /// </summary>
    public void RemoveFont(int num)
    {
        lock (_gate)
        {
            if (num < 0 || num >= _fonts.Count) return;
            _fonts[num]?.Dispose();
            _fonts[num] = null;
        }
    }

    /// <summary>
    /// 原文 863-888 Add(FontName, FontSize, FontStyles)：
    /// 先 `SetLength(Fonts, Length(Fonts)+1)`，建 `TGfxTextureFont.Create(Self)`，
    /// 设 Name/Size/Style/Charset（**Charset := GB2312_CHARSET** 在构造里已设），
    /// 调 `GetFontSize`（量字形），`nIndex := Length(Fonts)-1`，最后 `Fonts[Length(Fonts)-1] := Font`。
    /// 返回新建的字体（原文无返回值，此处为测试可观测性）。
    /// </summary>
    public TDxGfxTextureFont Add(string fontName, int fontSize, bool bold)
    {
        lock (_gate)
        {
            var font = new TDxGfxTextureFont(this);
            font.Descriptor.Name = fontName;
            font.Descriptor.Size = fontSize;
            font.Descriptor.StyleBold = bold;
            font.Descriptor.Charset = 134;                 // 原文构造里的 GB2312_CHARSET
            font.GetFontSize();                            // 接缝（缺省保留常量）
            font.nIndex = _fonts.Count;
            _fonts.Add(font);
            return font;
        }
    }

    /// <summary>原文 863-888 Add 的缺省重载（'宋体' / 9 / 非粗体）。</summary>
    public TDxGfxTextureFont Add() => Add("宋体", 9, false);

    /// <summary>
    /// 原文 936-955 SetFont(FontName, FontSize, FontStyles)：
    /// `RemoveAll; Add(...)`，并把单元级 `GfxTextureFont` 指向新字体（原文取 `Fonts[0]`）。
    /// </summary>
    public TDxGfxTextureFont SetFont(string fontName, int fontSize, bool bold)
    {
        RemoveAll();
        var font = Add(fontName, fontSize, bold);
        GlobalCurrent = font;
        return font;
    }

    /// <summary>原文 936-955 SetFont 的缺省重载。</summary>
    public TDxGfxTextureFont SetFont() => SetFont("宋体", 9, false);

    /// <summary>原文 917-925 Initialize（原文逐个 Fonts[I].Initialize）。</summary>
    public void Initialize()
    {
        for (int i = 0; i < _fonts.Count; i++)
            _fonts[i]?.Initialize();
    }

    /// <summary>原文 926-935 Finalize。</summary>
    public void Finalize2()
    {
        for (int i = 0; i < _fonts.Count; i++)
            _fonts[i]?.Finalize2();
    }

    /// <summary>原文 909-916 FreeIdleMemory（逐个转调）。返回回收条数合计。</summary>
    public int FreeIdleMemory()
    {
        int freed = 0;
        for (int i = 0; i < _fonts.Count; i++)
        {
            if (_fonts[i] != null) freed += _fonts[i].FreeIdleMemory();
        }
        return freed;
    }

    /// <summary>原文 811-819 Destroy（RemoveAll + 释放）。</summary>
    public void Dispose()
    {
        RemoveAll();
        lock (_gate)
        {
            _fonts.Clear();
        }
    }
}
