using System;
using System.Collections.Generic;
using GXX.Core.Rtl;
using GXX.Core.Util;

namespace GXX.Client.DxComponent;

// =====================================================================================
// DxImageButtonEx.pas（889 行，源：Source\Client-HGE\DxImageButtonEx.pas）1:1 移植。
//
// 源单元行号范围（本文件逐条对应）：
//   * 1-22      unit 头 + interface uses（Windows/Types/Classes/Controls/SysUtils/Graphics/
//               HGE/HGEFontEx/HGECanvas/GameImages/DxComponents/DxControls/DxImageButton/
//               MShare/ClFunc/Math/HUtil32）
//   * 24-140    interface 声明：TTokenBase(26) / TTokenText(45) / TTokenImage(66) /
//               TTokenPlayImage(80) / TTokenLine(100) / TLineList(121) / TDxImageButtonEx(142)
//   * 160-161   const ExpandLineHeight = 2
//   * 165-171   TTokenBase.Create
//   * 175-230   TTokenText（Create / Initialize / Paint）
//   * 234-269   TTokenImage（Create / Initialize / Paint）
//   * 273-320   TTokenPlayImage（Create / Initialize / Paint）
//   * 324-388   TTokenLine（Create / Destroy / Clear / GetCount / GetTokens / AddToken / RecalSize）
//   * 392-451   TLineList（Create / Destroy / AddLine / Clear / GetCount / GetLines / RecalSize）
//   * 455-741   ProcessButtonText（含 4 个嵌套过程：NewTokenText / NewTokenImage /
//               NewTokenPlayImage / CheckTokenImage）
//   * 743-887   TDxImageButtonEx（Create / Destroy / SetCaptionA / SetCaptionV / DoDrawCaption）
//
// DFM: 无同名 .dfm（TDxImageButtonEx 继承 TDxImageButton 的设计期外观）。
//
// -------------------------------------------------------------------------------------
// 托管侧接缝与偏差（逐条登记）：
//
//   1. `TDxImageButtonEx` 的三个覆写点（原文 146-148 `SetCaptionA` / `SetCaptionV` /
//      `DoDrawCaption` 均标 `override`）在托管基类上**没有对应的 virtual**：
//      `TDxControl.SetCaptionA/V` 在托管侧是静态助手 `DxControlHooks.SetCaptionA/V`，
//      上一波已把 `TDxImageButton.DoDrawCaption` 落成**非虚**的 `DoDrawCaptionV2()`。
//      处置（沿用本车道既有约定，见 DxImageButton.cs 文件头）：
//        * `DoDrawCaption` → `public override void DoDrawCaptionV2()` —— 需要把基类那份
//          改成 `virtual`（**一处加词**，见 DxImageButton.cs，已在报告登记）；
//        * `SetCaptionA` / `SetCaptionV` → `public void SetCaptionAV2 / SetCaptionVV2`
//          （基类无虚方法可覆写，故以公开方法承接原文的覆写位置，调用方显式调用）。
//
//   2. **原文全局被接缝化**（`DxImageButtonExEnv`，本文件内）：
//      `GameCanvas` → `Painter`（`IDxSurfacePainter`）；
//      `TextureFonts.FindFont` → `FindFont`；
//      `THGEFont.TextWidth/TextHeight/GetImageInfos` → `TextWidth/TextHeight/GetImageInfos`；
//      `g_sCurFontName` → `CurFontName`；`CurrentFont` → `CurrentFont`；
//      `g_CurrentFontHeight` → `CurrentFontHeight`；`MShare.GetRGB` → `GetRGB`；
//      `g_EffectImageList` → `EffectImageList`；`g_WBagItemImages.Looks` → `BagItemLooks`；
//      `g_WDnItemImages.Looks` → `DnItemLooks`；`g_WStateItemImages.Looks` → `StateItemLooks`；
//      `g_WNewopUIImages` → `NewopUIImages`。
//      默认值全部是**无头安全**的：Painter = TDxNullPainter，FindFont = null，
//      度量退化为 `TDxFontEnv.MeasureTextWidth/Height`，图像库 = null，GetRGB = 恒等。
//
//   3. `ClFunc.BoldTextOut(HGEFont, X, Y, Str, FColor, BColor = clBlack, Alpha = 255)`
//      （ClFunc.pas 43，**ClFunc 单元尚未移植**）→ `DxImageButtonExEnv.BoldTextOut`：
//      落到 `IDxSurfacePainter.TextRect`，4 次 ±1 像素描边 + 1 次正文的**顺序与
//      `TDxControl.DrawCaption`（DxComponents/DxControls 3752+）完全一致**。
//      `HGEFont.TextOut(X, Y, Str, FColor)` → `DxImageButtonExEnv.TextOut`（单次 TextRect）。
//
//   4. `TGameImages.Images[i]` → `IDxImageLibrary.GetImage(i)`；
//      `TGameImages.GetCachedImage(i, X, Y)`（返回纹理 + 缓存内偏移）→
//      `DxImageLibraryExt.GetCachedImage(...)`：未实现 `IDxImageLibraryCached` 时退化为
//      `GetImage(i)` 且偏移取 0（与 DxImageButton/DxImageForm 的既有处置一致）。
//
//   5. `MyGetTickCount` → `DxTickCount.MyGetTickCount`（可注入）。
//
//   6. `Copy(S, Index, Count)`（Delphi，**1 基**且参数越界返回空串）→ `DxCopy` 私有助手
//      1:1 复刻：`Index < 1` 或 `Count <= 0` 或 `Index > Length` → `''`，否则最多取 Count 个字符。
//      `Pos` 同 PosOf。`GetValidStr3_Ex` / `StrToIntDef` 复用 `GXX.Core` 既有实现。
//
//   7. `TTokenLine.Tokens[Index]` / `TLineList.Lines[Index]`（原文只读索引属性）→
//      托管侧**默认索引器**（C# 索引器无法携带名字，已在各自成员上注明原文名）。
//
//   8. 原文怪癖（逐字保留 + 差异断言）：
//      * **`TTokenImage.Paint` 忽略自己的 `FDrawBlend`**（原文 256-269 只有无混合的
//        `GameCanvas.Draw`）—— `DrawBlend` 属性写了也不生效，而 `TTokenPlayImage`
//        的同名属性是生效的（原文 307-310）。
//      * **`TTokenText.Initialize` 不先清零**（原文 186-207）：找不到字体时
//        `FWidth/FHeight` 保持**上一次**的值，而不是 0。`TTokenImage.Initialize` 同（242-254）。
//      * **`TTokenPlayImage.Initialize` 里的 `inherited`**（原文 287）指向父类
//        `TTokenBase.Initialize`，而后者是 **abstract**（原文 35）。`TTokenLine.RecalSize`
//        （原文 371-379）会对**每个** token 无条件调 `Initialize`，且 `<PlayImg:...>`
//        在客户端确有使用（ClMain.pas / SerialWindowsDlg.pas / NPCFormDeBug.pas），
//        故原文若真抛 `EAbstractError` 该功能早已不可用 → 托管侧按"抽象父类无实现 = 无操作"
//        落地（**不复刻该 inherited 调用**）。这是本文件唯一一处对原文语义的判断。
//      * **`TTokenPlayImage.Paint` 的帧推进在 `if Texture <> nil` 之外**（原文 313-319）：
//        取不到纹理也会推进帧号。
//      * 帧推进用 **严格大于** `CurTick - FDrawTick > Cardinal(FDrawTime)`（原文 314），
//        不是 `>=`。
//      * `SetCaptionA` 里的 `SL.Delimiter := '\'`（原文 773）**对 `SL.Text` 没有任何作用**
//        —— Delphi 的 `TStrings.SetTextStr` 只按 CR/LF 断行，`Delimiter` 仅影响
//        `DelimitedText`。故 `\` **不是**行分隔符。逐字保留（见 SetCaptionAV2 注释 + 差异断言）。
//      * `CheckTokenImage` 的 `IsPlayImg := False; IsBlendDraw := True;`（原文 620-621）
//        在 `if Length(Text) > 2` **之内** → 文本过短时这两个 var 形参保持调用方的值。
//      * `CheckTokenImage` 的形参是 `var`（原文 596-598）→ 托管侧用 `ref` 而非 `out`
//        （`out` 强制赋值会改变原文"未赋值即保留"的语义）。`AGameImages` 是唯一
//        无条件赋 nil 的形参（原文 605）。
// =====================================================================================

// -------------------------------------------------------------------------------------
// 接缝：原文的全局对象与全局函数
// -------------------------------------------------------------------------------------

/// <summary>
/// DxImageButtonEx.pas 依赖的全局对象/全局函数的最小面（原文见文件头第 2 条）。
/// 全部可注入；默认值保证无头环境不触碰任何 UI/资源。
/// </summary>
public static class DxImageButtonExEnv
{
    /// <summary>原文全局 `GameCanvas`（TTokenImage / TTokenPlayImage / TTokenText 的绘制出口）。</summary>
    public static IDxSurfacePainter Painter = new TDxNullPainter();

    /// <summary>原文全局 `TextureFonts.FindFont(Name, Size, Style)`。</summary>
    public static Func<string, int, IReadOnlyCollection<string>, object> FindFont;

    /// <summary>原文 `THGEFont.TextWidth`。默认退化为 <see cref="TDxFontEnv.MeasureTextWidth"/>。</summary>
    public static Func<object, string, int> TextWidth = (f, t) => TDxFontEnv.MeasureTextWidth(t);

    /// <summary>原文 `THGEFont.TextHeight`。默认退化为 <see cref="TDxFontEnv.MeasureTextHeight"/>。</summary>
    public static Func<object, string, int> TextHeight = (f, t) => TDxFontEnv.MeasureTextHeight(t);

    /// <summary>原文 `THGEFont.GetImageInfos`（TextOut/BoldTextOut 需要字形序列）。</summary>
    public static Func<object, string, List<TDxTextImageInfo>> GetImageInfos =
        (f, t) => TDxFontEnv.BuildImageInfos(t);

    /// <summary>原文全局 `g_sCurFontName`（FFontName 为空时的兜底字体名）。</summary>
    public static string CurFontName = "";

    /// <summary>原文全局 `CurrentFont:THGEFont`（TTokenLine.RecalSize 的兜底度量来源）。</summary>
    public static object CurrentFont;

    /// <summary>原文全局 `g_CurrentFontHeight`（TTokenLine.RecalSize 的高度兜底）。</summary>
    public static int CurrentFontHeight;

    /// <summary>原文 `MShare.GetRGB(c256:Byte):Integer`（调色板索引 → BGR）。默认恒等。</summary>
    public static Func<int, int> GetRGB = n => n;

    /// <summary>原文全局 `g_EffectImageList`（TList：`Count` + `Objects[i]`）。</summary>
    public static IDxImageList EffectImageList;

    /// <summary>原文 `g_WBagItemImages.Looks[i]`。</summary>
    public static Func<int, IDxImageLibrary> BagItemLooks;

    /// <summary>原文 `g_WDnItemImages.Looks[i]`。</summary>
    public static Func<int, IDxImageLibrary> DnItemLooks;

    /// <summary>原文 `g_WStateItemImages.Looks[i]`。</summary>
    public static Func<int, IDxImageLibrary> StateItemLooks;

    /// <summary>原文全局 `g_WNewopUIImages`。</summary>
    public static IDxImageLibrary NewopUIImages;

    /// <summary>恢复全部默认（测试夹具用）。</summary>
    public static void Reset()
    {
        Painter = new TDxNullPainter();
        FindFont = null;
        TextWidth = (f, t) => TDxFontEnv.MeasureTextWidth(t);
        TextHeight = (f, t) => TDxFontEnv.MeasureTextHeight(t);
        GetImageInfos = (f, t) => TDxFontEnv.BuildImageInfos(t);
        CurFontName = "";
        CurrentFont = null;
        CurrentFontHeight = 0;
        GetRGB = n => n;
        EffectImageList = null;
        BagItemLooks = null;
        DnItemLooks = null;
        StateItemLooks = null;
        NewopUIImages = null;
    }

    /// <summary>
    /// 原文 `HGEFont.TextOut(X, Y, Str, FColor)`（HGEFontEx.pas）→ 一次 `TextRect`。
    /// `width`/`height` 是该次文本的绘制区尺寸（原文由字体自行决定，托管侧由调用方给出）。
    /// </summary>
    public static void TextOut(object hgeFont, int x, int y, string text, int color, int width, int height)
    {
        if (hgeFont == null || string.IsNullOrEmpty(text)) return;
        var infos = GetImageInfos(hgeFont, text);
        if (infos == null || infos.Count == 0) return;
        Painter.TextRect(x, y, TDxRect.Rect(x, y, x + width, y + height), infos, color, 2, 255, 0);
    }

    /// <summary>
    /// 原文 `ClFunc.BoldTextOut(HGEFont, X, Y, Str, FColor, BColor, Alpha)`：
    /// **先画 4 次 ±1 像素偏移的描边、再画正文**（顺序不可换，与
    /// `TDxControl.DrawCaption` 的 Bold 分支一致）。
    /// </summary>
    public static void BoldTextOut(object hgeFont, int x, int y, string text, int color, int bColor,
        int width, int height)
    {
        if (hgeFont == null || string.IsNullOrEmpty(text)) return;
        var infos = GetImageInfos(hgeFont, text);
        if (infos == null || infos.Count == 0) return;
        var rect = TDxRect.Rect(x, y, x + width, y + height);
        Painter.TextRect(x - 1, y, rect, infos, bColor, 2, 255, 0);
        Painter.TextRect(x + 1, y, rect, infos, bColor, 2, 255, 0);
        Painter.TextRect(x, y - 1, rect, infos, bColor, 2, 255, 0);
        Painter.TextRect(x, y + 1, rect, infos, bColor, 2, 255, 0);
        Painter.TextRect(x, y, rect, infos, color, 2, 255, 0);
    }
}

/// <summary>
/// 接缝：原文 `g_EffectImageList:TList`（`Count` + `Objects[i]`，元素是 `TGameImages`）。
/// </summary>
public interface IDxImageList
{
    /// <summary>原文 `g_EffectImageList.Count`。</summary>
    int Count { get; }

    /// <summary>原文 `TGameImages(g_EffectImageList.Objects[i])`。</summary>
    IDxImageLibrary this[int index] { get; }
}

/// <summary>最小 `IDxImageList` 实现（测试与接缝默认用）。</summary>
public sealed class TDxImageListStub : IDxImageList
{
    private readonly List<IDxImageLibrary> _items = new();

    public int Count => _items.Count;

    public IDxImageLibrary this[int index]
        => index >= 0 && index < _items.Count ? _items[index] : null;

    public TDxImageListStub Add(IDxImageLibrary library) { _items.Add(library); return this; }
}

/// <summary>
/// 接缝：原文 `TGameImages.GetCachedImage(Index; var X, Y): TTexture`
/// —— 返回纹理**并输出该纹理在缓存中的绘制偏移**。
/// `IDxImageLibrary` 未含此方法（上游接缝在 DxImageButton/DxImageForm 时代只保留了
/// `GetImage`），故以独立可选接口承接（与 `IDxSurfacePainterExt` 同一模式）。
/// </summary>
public interface IDxImageLibraryCached
{
    IDxTexture GetCachedImage(int index, out int offsetX, out int offsetY);
}

/// <summary>`IDxImageLibraryCached` 的转发落点（未实现时偏移取 0）。</summary>
public static class DxImageLibraryExt
{
    /// <summary>
    /// 原文 `FImage.GetCachedImage(Index, X, Y)`；库未实现缓存接口时退化为
    /// `GetImage(Index)` + 偏移 0（与 DxImageButton.cs 文件头第 3 条同源）。
    /// </summary>
    public static IDxTexture GetCachedImage(IDxImageLibrary library, int index, out int offsetX, out int offsetY)
    {
        if (library is IDxImageLibraryCached cached)
            return cached.GetCachedImage(index, out offsetX, out offsetY);
        offsetX = 0;
        offsetY = 0;
        return library?.GetImage(index);
    }
}

// -------------------------------------------------------------------------------------
// DxImageButtonEx.pas 160-161 / 455-741：单元级常量与自由函数
// -------------------------------------------------------------------------------------

/// <summary>DxImageButtonEx.pas 的单元级常量与自由函数。</summary>
public static class DxImageButtonExUnit
{
    /// <summary>原文 161 `ExpandLineHeight = 2`（TLineList.RecalSize 的逐行追加高度）。</summary>
    public const int ExpandLineHeight = 2;

    /// <summary>Delphi `Copy(S, Index, Count)`（1 基；越界返回空串）。</summary>
    internal static string DxCopy(string s, int index, int count)
    {
        if (s == null || index < 1 || count <= 0 || index > s.Length) return "";
        int take = Math.Min(count, s.Length - (index - 1));
        return s.Substring(index - 1, take);
    }

    /// <summary>Delphi `Pos(Sub, S)`（1 基；未找到返回 0）。</summary>
    internal static int DxPos(string sub, string s)
    {
        if (string.IsNullOrEmpty(sub) || string.IsNullOrEmpty(s)) return 0;
        int i = s.IndexOf(sub, StringComparison.Ordinal);
        return i < 0 ? 0 : i + 1;
    }

    /// <summary>Delphi `SameText`（大小写不敏感，区域性无关）。</summary>
    private static bool SameText(string a, string b)
        => string.Equals(a, b, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// 原文 455-741 <c>ProcessButtonText(S: string; TokenList: TTokenLine; Font: TDxFont): string</c>
    /// —— `<Img:...>` / `<Looks:...>` / `<DnItems:...>` / `<StateItem:...>` / `<NewopUI:...>` /
    /// `<PlayImg:...>` 标记解析 + `{自定义颜色|调色板索引}` 解析，边解析边往 `TokenList` 加 token，
    /// 返回**去掉图像标记后**的纯文本（颜色标记会保留其文字）。
    ///
    /// 4 个嵌套过程在原文里是 `ProcessButtonText` 的内部函数（能直接写 `TokenList` / `Font`
    /// 的私有字段），托管侧 1:1 落成 **C# 局部函数**（同样捕获外层参数）。
    /// </summary>
    public static string ProcessButtonText(string s, TTokenLine tokenList, TDxFont font)
    {
        // ---- 原文 458-560 嵌套 function NewTokenText(Text: string): string ----
        string NewTokenText(string text)
        {
            // 原文 465-476：空串也建一个 token（尺寸取自 Font），并直接返回
            if (text.Length == 0)
            {
                var empty = new TTokenText(tokenList);
                ApplyFont(empty, font);
                empty._caption = text;
                tokenList.AddToken(empty);
                return text;
            }

            string result = "";
            int index1 = DxPos("{", text);
            int index2 = index1 > 0 ? DxPos("}", text) : 0;

            while (index1 > 0 && index2 > 0 && text != "")
            {
                string s1 = DxCopy(text, 1, index1 - 1);
                string s2 = DxCopy(text, index1 + 1, index2 - index1 - 1);

                if (s1.Length > 0)
                {
                    var t = new TTokenText(tokenList);
                    ApplyFont(t, font);
                    t._caption = s1;
                    tokenList.AddToken(t);
                    result += t._caption;
                }

                bool customColorText = false;
                if (s2.Length > 0)
                {
                    int index3 = DxPos("|", s2);
                    if (index3 > 0)
                    {
                        string s3 = DxCopy(s2, 1, index3 - 1);
                        string s4 = DxCopy(s2, index3 + 1, int.MaxValue);
                        int nColor = DelphiRTL.StrToIntDef(s4, -1);
                        if (nColor >= 0 && nColor <= 255)
                        {
                            customColorText = true;
                            if (s3.Length > 0)
                            {
                                var t = new TTokenText(tokenList);
                                ApplyFont(t, font);
                                t._fontColor = DxImageButtonExEnv.GetRGB(nColor);
                                t._caption = s3;
                                tokenList.AddToken(t);
                                result += t._caption;
                            }
                        }
                    }
                }

                if (!customColorText)
                {
                    var t = new TTokenText(tokenList);
                    ApplyFont(t, font);
                    t._caption = "{" + s2 + "}";
                    tokenList.AddToken(t);
                    result += t._caption;
                }

                text = DxCopy(text, index2 + 1, int.MaxValue);
                index1 = DxPos("{", text);
                index2 = index1 > 0 ? DxPos("}", text) : 0;
            }

            if (text.Length > 0)
            {
                var t = new TTokenText(tokenList);
                ApplyFont(t, font);
                t._caption = text;
                tokenList.AddToken(t);
                result += t._caption;
            }

            return result;
        }

        // ---- 原文 562-575 嵌套 procedure NewTokenImage ----
        void NewTokenImage(string text, IDxImageLibrary aGameImages, int aImageIndex, int offsetX, int offsetY)
        {
            if (aGameImages != null)
            {
                var t = new TTokenImage(tokenList)
                {
                    GameImages = aGameImages,
                    ImageIndex = aImageIndex,
                    OffsetX = offsetX,
                    OffsetY = offsetY,
                };
                tokenList.AddToken(t);
            }
        }

        // ---- 原文 577-594 嵌套 procedure NewTokenPlayImage ----
        void NewTokenPlayImage(string text, IDxImageLibrary aGameImages, int aImageIndex, int aPlayCount,
            int aPlayTime, int offsetX, int offsetY, bool isBlendDraw)
        {
            if (aGameImages != null)
            {
                var t = new TTokenPlayImage(tokenList)
                {
                    GameImages = aGameImages,
                    _startIndex = aImageIndex,
                    _drawIndex = aImageIndex,
                    _drawCount = aPlayCount,
                    _drawTime = aPlayTime,
                    _drawTick = DxTickCount.MyGetTickCount(),
                    OffsetX = offsetX,
                    OffsetY = offsetY,
                    _drawBlend = isBlendDraw,
                };
                tokenList.AddToken(t);
            }
        }

        // ---- 原文 596-681 嵌套 function CheckTokenImage（形参是 var → C# 用 ref）----
        bool CheckTokenImage(string text, ref IDxImageLibrary aGameImages, ref int imageIndex,
            ref int offsetX, ref int offsetY, ref bool isPlayImg, ref int playCount, ref int playTime,
            ref bool isBlendDraw)
        {
            bool result = false;                       // 原文 604（'<Img:N:F:X:Y>'）
            aGameImages = null;                        // 原文 605（唯一无条件赋值的 var 形参）

            if (text.Length > 2)
            {
                string temp = DxCopy(text, 2, text.Length - 2);
                string sName = "", s1 = "", s2 = "", s3 = "", s4 = "", s5 = "", s6 = "", s7 = "";
                temp = HUtil32.GetValidStr3_Ex(temp, ref sName, ':');
                temp = HUtil32.GetValidStr3_Ex(temp, ref s1, ':');
                temp = HUtil32.GetValidStr3_Ex(temp, ref s2, ':');
                temp = HUtil32.GetValidStr3_Ex(temp, ref s3, ':');
                temp = HUtil32.GetValidStr3_Ex(temp, ref s4, ':');
                temp = HUtil32.GetValidStr3_Ex(temp, ref s5, ':');
                temp = HUtil32.GetValidStr3_Ex(temp, ref s6, ':');
                temp = HUtil32.GetValidStr3_Ex(temp, ref s7, ':');

                int i1 = DelphiRTL.StrToIntDef(s1, -1);
                int i2 = DelphiRTL.StrToIntDef(s2, -1);
                isPlayImg = false;                     // 原文 620（在 Length(Text) > 2 之内）
                isBlendDraw = true;                    // 原文 621

                if (SameText(sName, "Img") && i2 >= 0)                       // 原文 624
                {
                    if (i1 >= 0)
                    {
                        result = true;
                        if (i2 < EffectImageCount())                          // 原文 628
                            aGameImages = EffectImageAt(i2);

                        imageIndex = i1;
                        offsetX = DelphiRTL.StrToIntDef(s3, 0);
                        offsetY = DelphiRTL.StrToIntDef(s4, 0);
                    }
                }
                else if (SameText(sName, "Looks") && i1 >= 0)                 // 原文 635
                {
                    result = true;
                    aGameImages = DxImageButtonExEnv.BagItemLooks?.Invoke(i1);
                    imageIndex = i1 % 10000;
                    offsetX = DelphiRTL.StrToIntDef(s2, 0);
                    offsetY = DelphiRTL.StrToIntDef(s3, 0);
                }
                else if (SameText(sName, "DnItems") && i1 >= 0)               // 原文 641
                {
                    result = true;
                    aGameImages = DxImageButtonExEnv.DnItemLooks?.Invoke(i1);
                    imageIndex = i1 % 10000;
                    offsetX = DelphiRTL.StrToIntDef(s2, 0);
                    offsetY = DelphiRTL.StrToIntDef(s3, 0);
                }
                else if (SameText(sName, "StateItem") && i1 >= 0)             // 原文 647
                {
                    result = true;
                    aGameImages = DxImageButtonExEnv.StateItemLooks?.Invoke(i1);
                    imageIndex = i1 % 10000;
                    offsetX = DelphiRTL.StrToIntDef(s2, 0);
                    offsetY = DelphiRTL.StrToIntDef(s3, 0);
                }
                else if (SameText(sName, "NewopUI") && i1 >= 0)              // 原文 653
                {
                    result = true;
                    aGameImages = DxImageButtonExEnv.NewopUIImages;
                    imageIndex = i1;
                    offsetX = DelphiRTL.StrToIntDef(s2, 0);
                    offsetY = DelphiRTL.StrToIntDef(s3, 0);
                }
                else if (SameText(sName, "PlayImg") && i1 >= 0 && i2 >= 0)    // 原文 659
                {
                    // 格式: <PlayImg:F:N:C:T:X:Y:M>
                    result = true;
                    isPlayImg = true;

                    playCount = DelphiRTL.StrToIntDef(s3, 0);
                    playTime = DelphiRTL.StrToIntDef(s4, 0);
                    if (playTime <= 0) playTime = 100;                        // 原文 668-669

                    if (i1 < EffectImageCount() && playCount > 0)              // 原文 671
                        aGameImages = EffectImageAt(i1);

                    imageIndex = i2;
                    offsetX = DelphiRTL.StrToIntDef(s5, 0);
                    offsetY = DelphiRTL.StrToIntDef(s6, 0);
                    isBlendDraw = DelphiRTL.StrToIntDef(s7, 0) != 0;
                }
            }

            return result;
        }

        // ---- 原文 693-741 主循环 ----
        string ProcessResult = "";
        int outerIndex1 = DxPos("<", s);
        if (outerIndex1 == 0)
        {
            ProcessResult += NewTokenText(s);
        }
        else
        {
            string strB = "";
            while (s != "")
            {
                strB += DxCopy(s, 1, outerIndex1 - 1);
                s = DxCopy(s, outerIndex1, int.MaxValue);

                int outerIndex2 = DxPos(">", s);
                if (outerIndex2 == 0)
                {
                    ProcessResult += NewTokenText(strB + s);
                    break;
                }

                bool isPlayImg = false;
                bool isBlendDraw = false;
                IDxImageLibrary gameImages = null;
                int gameImageIndex = 0;
                int ox = 0, oy = 0;
                int playCount = 0, playTime = 0;

                string strC = DxCopy(s, 1, outerIndex2);
                if (!CheckTokenImage(strC, ref gameImages, ref gameImageIndex, ref ox, ref oy,
                        ref isPlayImg, ref playCount, ref playTime, ref isBlendDraw))
                {
                    strB += strC;
                }
                else
                {
                    if (strB.Length > 0)
                        ProcessResult += NewTokenText(strB);

                    if (isPlayImg)
                        NewTokenPlayImage(strC, gameImages, gameImageIndex, playCount, playTime, ox, oy, isBlendDraw);
                    else
                        NewTokenImage(strC, gameImages, gameImageIndex, ox, oy);

                    strB = "";
                    strC = "";
                }

                s = DxCopy(s, outerIndex2 + 1, int.MaxValue);
                outerIndex1 = DxPos("<", s);
                if (outerIndex1 == 0)
                {
                    s = strB + s;
                    if (s != "")
                        ProcessResult += NewTokenText(s);
                    break;
                }
            }
        }

        return ProcessResult;
    }

    /// <summary>
    /// `<c>g_EffectImageList.Count</c>`；未注入时返回 **-1**，使 `if I &lt; Count` 恒假
    /// （原文此处对 nil 列表会 AV；托管侧退化为"不取图库"，标签文本按原文的
    /// "CheckTokenImage 返回 True 但图库为 nil" 路径处理）。
    /// </summary>
    private static int EffectImageCount()
        => DxImageButtonExEnv.EffectImageList == null ? -1 : DxImageButtonExEnv.EffectImageList.Count;

    /// <summary>`TGameImages(g_EffectImageList.Objects[i])`（越界/未注入 → null）。</summary>
    private static IDxImageLibrary EffectImageAt(int index)
    {
        var list = DxImageButtonExEnv.EffectImageList;
        return list == null || index < 0 || index >= list.Count ? null : list[index];
    }

    /// <summary>原文 466-472（及 493-498 / 515-521 / 530-536 / 550-556）的 5 项字体拷贝。</summary>
    private static void ApplyFont(TTokenText token, TDxFont font)
    {
        token._fontSize = font.Size;
        token._fontStyle = font.Style;
        token._fontColor = font.Color;
        token._fontStroke = font.Bold;
        token._fontName = font.Name;
    }
}

// -------------------------------------------------------------------------------------
// DxImageButtonEx.pas 26-43：TTokenBase
// -------------------------------------------------------------------------------------

/// <summary>
/// DxImageButtonEx.pas 26-43 / 165-171 <c>TTokenBase</c>（token 基类）。
/// 原文 `Initialize` / `Paint` 是 `virtual; abstract;` → 托管侧 `abstract`。
/// </summary>
public abstract class TTokenBase
{
    private int _width;                     // 原文 28 FWidth（Create 不赋值 → 0）
    private int _height;                    // 原文 29 FHeight
    private int _offsetX;                   // 原文 30 FOffsetX
    private int _offsetY;                   // 原文 31 FOffsetY
    private readonly TTokenLine _owner;     // 原文 32 FOwner

    /// <summary>原文 165-171 <c>TTokenBase.Create(AOwner)</c>：只置 OffsetX/OffsetY/Owner。</summary>
    protected TTokenBase(TTokenLine aOwner)
    {
        _offsetX = 0;
        _offsetY = 0;
        _owner = aOwner;
    }

    /// <summary>原文 38 `property Owner:TTokenLine read FOwner`。</summary>
    public TTokenLine Owner => _owner;

    /// <summary>原文 39 `property Width:Integer`。</summary>
    public int Width { get => _width; set => _width = value; }

    /// <summary>原文 40 `property Height:Integer`。</summary>
    public int Height { get => _height; set => _height = value; }

    /// <summary>原文 41 `property OffsetX:Integer`。</summary>
    public int OffsetX { get => _offsetX; set => _offsetX = value; }

    /// <summary>原文 42 `property OffsetY:Integer`。</summary>
    public int OffsetY { get => _offsetY; set => _offsetY = value; }

    /// <summary>原文 35 `procedure Initialize; virtual; abstract;`。</summary>
    public abstract void Initialize();

    /// <summary>原文 36 `procedure Paint(PointX, PointY:Integer); virtual; abstract;`。</summary>
    public abstract void Paint(int pointX, int pointY);
}

// -------------------------------------------------------------------------------------
// DxImageButtonEx.pas 45-64：TTokenText
// -------------------------------------------------------------------------------------

/// <summary>
/// DxImageButtonEx.pas 45-64 / 175-230 <c>TTokenText</c>（文本 token）。
///
/// 原文的 6 个字段是 `private`，但同一单元的 `ProcessButtonText` 直接写它们
/// （Delphi 的 `private` = **单元内**可见）→ 托管侧落为 `internal`（汇编内可见，
/// 语义上最接近"单元内可见"；对外仍是 6 个**只读**属性）。
/// </summary>
public class TTokenText : TTokenBase
{
    internal string _caption;                                    // 47 FCaption
    internal string _fontName;                                   // 48 FFontName
    internal int _fontColor = TDxColor.clWhite;                  // 49 FFontColor
    internal int _fontSize;                                      // 50 FFontSize
    internal IReadOnlyCollection<string> _fontStyle = Array.Empty<string>();   // 51 FFontStyle
    internal bool _fontStroke;                                   // 52 FFontStroke

    /// <summary>原文 175-184 <c>TTokenText.Create</c> 逐项。</summary>
    public TTokenText(TTokenLine aOwner) : base(aOwner)
    {
        _caption = "";
        _fontName = "";
        _fontColor = TDxColor.clWhite;
        _fontSize = 0;
        _fontStyle = Array.Empty<string>();
        _fontStroke = false;
    }

    /// <summary>原文 58 `property Caption:string read FCaption`。</summary>
    public string Caption => _caption;

    /// <summary>原文 59 `property FontName:string read FFontName`。</summary>
    public string FontName => _fontName;

    /// <summary>原文 60 `property FontColor:TColor read FFontColor`。</summary>
    public int FontColor => _fontColor;

    /// <summary>原文 61 `property FontSize:Integer read FFontSize`。</summary>
    public int FontSize => _fontSize;

    /// <summary>原文 62 `property FontStyle:TFontStyles read FFontStyle`。</summary>
    public IReadOnlyCollection<string> FontStyle => _fontStyle;

    /// <summary>原文 63 `property FontStroke:Boolean read FFontStroke`。</summary>
    public bool FontStroke => _fontStroke;

    /// <summary>
    /// 原文 186-207 <c>TTokenText.Initialize</c>：
    /// 先用 `FFontName`、再用 `g_sCurFontName` 兜底找字体；
    /// **只有找到字体才写 `FWidth/FHeight`**（找不到时保留上一次的值，不归零 —— 原文怪癖）；
    /// `FFontStroke` 时宽高各 +2。
    /// </summary>
    public override void Initialize()
    {
        object hgeFont = null;

        if (_fontName != "")
            hgeFont = DxImageButtonExEnv.FindFont?.Invoke(_fontName, _fontSize, _fontStyle);

        if (hgeFont == null)
            hgeFont = DxImageButtonExEnv.FindFont?.Invoke(DxImageButtonExEnv.CurFontName, _fontSize, _fontStyle);

        if (hgeFont != null)
        {
            Width = DxImageButtonExEnv.TextWidth(hgeFont, _caption);
            Height = DxImageButtonExEnv.TextHeight(hgeFont, "Pp");

            if (_fontStroke)
            {
                Width = Width + 2;
                Height = Height + 2;
            }
        }
    }

    /// <summary>
    /// 原文 209-230 <c>TTokenText.Paint</c>：
    /// `FCaption = ''` 直接 Exit；同样两次找字体；找到后
    /// `FFontStroke` 走 `BoldTextOut(..., clBlack)`，否则走 `HGEFont.TextOut`。
    /// 注意**每次都重新找字体**（与 Initialize 重复，原文如此）。
    /// </summary>
    public override void Paint(int pointX, int pointY)
    {
        if (_caption == "") return;

        object hgeFont = null;

        if (_fontName != "")
            hgeFont = DxImageButtonExEnv.FindFont?.Invoke(_fontName, _fontSize, _fontStyle);

        if (hgeFont == null)
            hgeFont = DxImageButtonExEnv.FindFont?.Invoke(DxImageButtonExEnv.CurFontName, _fontSize, _fontStyle);

        if (hgeFont != null)
        {
            if (_fontStroke)
                DxImageButtonExEnv.BoldTextOut(hgeFont, pointX + OffsetX, pointY + OffsetY, _caption,
                    _fontColor, TDxColor.clBlack, Width, Height);
            else
                DxImageButtonExEnv.TextOut(hgeFont, pointX + OffsetX, pointY + OffsetY, _caption,
                    _fontColor, Width, Height);
        }
    }
}

// -------------------------------------------------------------------------------------
// DxImageButtonEx.pas 66-78：TTokenImage
// -------------------------------------------------------------------------------------

/// <summary>
/// DxImageButtonEx.pas 66-78 / 234-269 <c>TTokenImage</c>（静态图片 token）。
/// 原文怪癖：**`Paint` 完全不用 `FDrawBlend`**（256-269 只有 `GameCanvas.Draw`）。
/// </summary>
public class TTokenImage : TTokenBase
{
    private IDxImageLibrary _gameImages;                  // 68 FGameImages
    private int _imageIndex;                              // 69 FImageIndex

    /// <summary>原文 234-240 <c>TTokenImage.Create</c>：GameImages=nil / ImageIndex=0 / DrawBlend=False。</summary>
    public TTokenImage(TTokenLine aOwner) : base(aOwner)
    {
        _gameImages = null;
        _imageIndex = 0;
        DrawBlend = false;
    }

    /// <summary>原文 75 `property GameImages:TGameImages`（读写）。</summary>
    public IDxImageLibrary GameImages { get => _gameImages; set => _gameImages = value; }

    /// <summary>原文 76 `property ImageIndex:Integer`（读写）。</summary>
    public int ImageIndex { get => _imageIndex; set => _imageIndex = value; }

    /// <summary>原文 77 `property DrawBlend:Boolean`（读写；**Paint 里不读它** —— 原文怪癖）。</summary>
    public bool DrawBlend { get; set; }

    /// <summary>原文 242-254 <c>Initialize</c>：取到纹理才写宽高（取不到保留旧值）。</summary>
    public override void Initialize()
    {
        if (_gameImages != null)
        {
            var texture = _gameImages.GetImage(_imageIndex);
            if (texture != null)
            {
                Width = texture.Width;
                Height = texture.Height;
            }
        }
    }

    /// <summary>原文 256-269 <c>Paint</c>：`GameCanvas.Draw(X, Y, Texture)`（无混合分支）。</summary>
    public override void Paint(int pointX, int pointY)
    {
        if (_gameImages != null)
        {
            var texture = _gameImages.GetImage(_imageIndex);
            if (texture != null)
            {
                int x = pointX + OffsetX;
                int y = pointY + OffsetY;
                DxImageButtonExEnv.Painter.Draw(x, y, texture, 2);
            }
        }
    }
}

// -------------------------------------------------------------------------------------
// DxImageButtonEx.pas 80-98：TTokenPlayImage
// -------------------------------------------------------------------------------------

/// <summary>
/// DxImageButtonEx.pas 80-98 / 273-320 <c>TTokenPlayImage</c>（逐帧播放图片 token）。
/// </summary>
public class TTokenPlayImage : TTokenBase
{
    private IDxImageLibrary _gameImages;                  // 82 FGameImages
    internal bool _drawBlend;                             // 83 FDrawBlend
    internal int _startIndex;                             // 84 FStartIndex
    internal int _drawCount;                              // 85 FDrawCount
    internal int _drawTime;                               // 86 FDrawTime
    internal uint _drawTick;                              // 87 FDrawTick
    internal int _drawIndex;                              // 88 FDrawIndex

    /// <summary>原文 273-283 <c>Create</c>：DrawTime=100 / DrawTick=MyGetTickCount / 其余 0。</summary>
    public TTokenPlayImage(TTokenLine aOwner) : base(aOwner)
    {
        _gameImages = null;
        _drawBlend = false;
        _startIndex = 0;
        _drawCount = 0;
        _drawTime = 100;
        _drawTick = DxTickCount.MyGetTickCount();
        _drawIndex = 0;
    }

    /// <summary>原文 93 `property GameImages:TGameImages`（读写）。</summary>
    public IDxImageLibrary GameImages { get => _gameImages; set => _gameImages = value; }

    /// <summary>原文 94 `property StartIndex:Integer read FStartIndex`。</summary>
    public int StartIndex => _startIndex;

    /// <summary>原文 95 `property DrawCount:Integer read FDrawCount`。</summary>
    public int DrawCount => _drawCount;

    /// <summary>原文 96 `property DrawTime:Integer read FDrawTime`。</summary>
    public int DrawTime => _drawTime;

    /// <summary>原文 97 `property DrawBlend:Boolean read FDrawBlend`（**Paint 里读它**，与 TTokenImage 不同）。</summary>
    public bool DrawBlend => _drawBlend;

    /// <summary>当前帧号（原文 88 FDrawIndex；源里无公开属性，测试与接缝需要它，故补只读属性并登记）。</summary>
    public int DrawIndex => _drawIndex;

    /// <summary>
    /// 原文 285-290 <c>Initialize</c>：`inherited;` + 宽高归零。
    /// 原文的 `inherited` 指向 **abstract** 的 `TTokenBase.Initialize`（见文件头第 8 条），
    /// 托管侧不复刻该调用（抽象父类无实现 = 无操作）。
    /// </summary>
    public override void Initialize()
    {
        // 原文 287 `inherited;` —— 见文件头第 8 条（抽象父类，语义等价于无操作）
        Width = 0;
        Height = 0;
    }

    /// <summary>
    /// 原文 292-320 <c>Paint</c>：
    /// ① 取当前帧纹理（含缓存内偏移 Pt.X/Pt.Y）；
    /// ② 有纹理就按 `FDrawBlend` 选 Draw/DrawBlend 画在 `Point + Offset + Pt`；
    /// ③ **无论有没有纹理**都做帧推进：`CurTick - FDrawTick > Cardinal(FDrawTime)` 时
    ///    刷新 tick、`Inc(FDrawIndex)`，并 `if FDrawIndex - FStartIndex + 1 > DrawCount then
    ///    FDrawIndex := FStartIndex`。
    /// </summary>
    public override void Paint(int pointX, int pointY)
    {
        if (_gameImages == null) return;

        var texture = DxImageLibraryExt.GetCachedImage(_gameImages, _drawIndex, out int ptX, out int ptY);
        if (texture != null)
        {
            // 修正物品和对应的特效对不上 chongchong 2015-04-11（原文 303 注释）
            int x = pointX + OffsetX + ptX;
            int y = pointY + OffsetY + ptY;

            if (_drawBlend)
                DxPainterExt.DrawBlend(DxImageButtonExEnv.Painter, x, y, texture.ClientRect, texture, 2);
            else
                DxImageButtonExEnv.Painter.Draw(x, y, texture, 2);
        }

        uint curTick = DxTickCount.MyGetTickCount();
        if (curTick - _drawTick > (uint)_drawTime)                  // 原文 314：严格大于 + Cardinal 回绕
        {
            _drawTick = curTick;
            _drawIndex++;
            if (_drawIndex - _startIndex + 1 > DrawCount)
                _drawIndex = _startIndex;
        }
    }
}

// -------------------------------------------------------------------------------------
// DxImageButtonEx.pas 100-119：TTokenLine
// -------------------------------------------------------------------------------------

/// <summary>
/// DxImageButtonEx.pas 100-119 / 324-388 <c>TTokenLine</c>（一行 token）。
/// 原文 `property Tokens[Index:Integer]:TTokenBase read GetTokens` → 托管侧默认索引器。
/// </summary>
public class TTokenLine
{
    private readonly List<TTokenBase> _tokens = new();     // 102 FTokens（原文 TList）

    /// <summary>原文 324-329 <c>TTokenLine.Create</c>：空表 + 宽高 0。</summary>
    public TTokenLine()
    {
        Width = 0;
        Height = 0;
    }

    /// <summary>原文 114 `property Count:Integer read GetCount`。</summary>
    public int Count => _tokens.Count;

    /// <summary>原文 115 `property Tokens[Index:Integer]:TTokenBase read GetTokens`（只读索引属性）。</summary>
    public TTokenBase this[int index] => _tokens[index];

    /// <summary>原文 117 `property Width:Integer read FWidth`（只读，由 RecalSize 写）。</summary>
    public int Width { get; private set; }

    /// <summary>原文 118 `property Height:Integer read FHeight`（只读，由 RecalSize 写）。</summary>
    public int Height { get; private set; }

    /// <summary>原文 338-346 <c>Clear</c>：逐个释放 token 后清空（托管侧 GC 负责释放）。</summary>
    public void Clear() => _tokens.Clear();

    /// <summary>原文 358-361 <c>AddToken</c>（**不查重、不判 nil**）。</summary>
    public void AddToken(TTokenBase token) => _tokens.Add(token);

    /// <summary>
    /// 原文 363-388 <c>RecalSize</c>：
    /// ① 宽高先归零；② 对**每个** token 调 `Initialize`；③ 只有 `TTokenText` 参与累加
    /// （高度取 Max、宽度累加）；④ `CurrentFont &lt;&gt; nil` 时对 ≤0 的宽/高做兜底
    /// （宽 = `CurrentFont.TextWidth('0')`、高 = `g_CurrentFontHeight`）。
    /// </summary>
    public void RecalSize()
    {
        Width = 0;
        Height = 0;

        for (int i = 0; i < _tokens.Count; i++)
        {
            var token = _tokens[i];
            token.Initialize();

            if (token is TTokenText)
            {
                Height = Math.Max(Height, token.Height);
                Width = Width + token.Width;
            }
        }

        if (DxImageButtonExEnv.CurrentFont != null)
        {
            if (Width <= 0)
                Width = DxImageButtonExEnv.TextWidth(DxImageButtonExEnv.CurrentFont, "0");

            if (Height <= 0)
                Height = DxImageButtonExEnv.CurrentFontHeight;
        }
    }
}

// -------------------------------------------------------------------------------------
// DxImageButtonEx.pas 121-140：TLineList
// -------------------------------------------------------------------------------------

/// <summary>
/// DxImageButtonEx.pas 121-140 / 392-451 <c>TLineList</c>（多行 token）。
/// 原文 `property Lines[Index:Integer]:TTokenLine read GetLines` → 托管侧默认索引器。
/// </summary>
public class TLineList
{
    private readonly List<TTokenLine> _lines = new();      // 123 FLines（原文 TList）

    /// <summary>原文 392-397 <c>TLineList.Create</c>：空表 + 宽高 0。</summary>
    public TLineList()
    {
        Width = 0;
        Height = 0;
    }

    /// <summary>原文 135 `property Count:Integer read GetCount`。</summary>
    public int Count => _lines.Count;

    /// <summary>原文 136 `property Lines[Index:Integer]:TTokenLine read GetLines`（只读索引属性）。</summary>
    public TTokenLine this[int index] => _lines[index];

    /// <summary>原文 138 `property Width:Integer read FWidth`（只读，由 RecalSize 写）。</summary>
    public int Width { get; private set; }

    /// <summary>原文 139 `property Height:Integer read FHeight`（只读，由 RecalSize 写）。</summary>
    public int Height { get; private set; }

    /// <summary>原文 406-410 <c>AddLine</c>：新建一行并入表，返回该行。</summary>
    public TTokenLine AddLine()
    {
        var line = new TTokenLine();
        _lines.Add(line);
        return line;
    }

    /// <summary>原文 412-420 <c>Clear</c>：逐行释放后清空。</summary>
    public void Clear() => _lines.Clear();

    /// <summary>
    /// 原文 432-451 <c>RecalSize</c>：逐行 `RecalSize` 后
    /// 宽取各行最大值；高**累加** `Line.Height + AddHeight`，而 `AddHeight` 从第二行起
    /// 固定为 `ExpandLineHeight`（= 2）—— 即行间距只加在**行与行之间**，末行之后不加。
    /// </summary>
    public void RecalSize()
    {
        Width = 0;
        Height = 0;
        int addHeight = 0;

        for (int i = 0; i < _lines.Count; i++)
        {
            var line = _lines[i];
            line.RecalSize();

            if (Width < line.Width)
                Width = line.Width;

            Height = Height + line.Height + addHeight;
            addHeight = DxImageButtonExUnit.ExpandLineHeight;
        }
    }
}

// -------------------------------------------------------------------------------------
// DxImageButtonEx.pas 142-156 / 743-887：TDxImageButtonEx
// -------------------------------------------------------------------------------------

/// <summary>
/// DxImageButtonEx.pas 142-156 / 743-887 <c>TDxImageButtonEx : TDxImageButton</c>
/// —— 带 token 化标题（多行 + 内嵌图片/播放图 + 自定义颜色）的按钮。
///
/// 原文的三个覆写点在托管侧的落点见文件头第 1 条：
///   * `DoDrawCaption` → <see cref="DoDrawCaptionV2"/>（**真 override**；基类
///     `TDxImageButton.DoDrawCaptionV2` 已改为 `virtual`）；
///   * `SetCaptionA` / `SetCaptionV` → <see cref="SetCaptionAV2"/> / <see cref="SetCaptionVV2"/>
///     （基类无虚方法可覆写，故以公开方法承接原文的覆写位置，调用方显式调用）。
/// </summary>
public class TDxImageButtonEx : TDxImageButton
{
    /// <summary>原文 144 FLineList（token 化的标题行表）。</summary>
    public TLineList LineList;

    /// <summary>
    /// 原文 747-755 构造：`inherited Create(AOwner); FLineList := TLineList.Create;`
    /// （AOwner 在托管侧由 WinForms 的父子关系承接，故构造函数无参 —— 与
    /// `TDxImageButton()` 一致。）
    /// </summary>
    public TDxImageButtonEx()
    {
        LineList = new TLineList();
    }

    /// <summary>
    /// 原文 757-761 <c>Destroy</c>：`FLineList.Free; inherited Destroy;`
    /// （托管侧 GC 负责释放，此处显式丢弃引用，与 `DisposeButton` 同一约定。）
    /// </summary>
    public void DisposeImageButtonEx()
    {
        LineList = null;
    }

    /// <summary>
    /// 原文 763-790 <c>SetCaptionA</c>（覆写基类）：
    /// 清空行表 → 按 `TStringList.Text` 断行（**只按 CR/LF**，见文件头第 8 条）→
    /// 每行 `ProcessButtonText` 建 token → `FLineList.RecalSize` →
    /// 用**处理后的文本**调 `inherited SetCaptionA`。
    ///
    /// 原文 773 的 `SL.Delimiter := '\';` 对 `SL.Text` 没有任何作用
    /// （Delphi `TStrings.SetTextStr` 只按 CR/LF 断行，`Delimiter` 仅影响 `DelimitedText`），
    /// 故托管侧用既有 `TDxFontEnv.SplitTextLines`（其注释明确写着
    /// "结尾换行不产生额外空行（TStringList.SetTextStr 语义）"）承接并**忽略**该赋值。
    /// </summary>
    public void SetCaptionAV2(string value)
    {
        LineList.Clear();

        var lines = TDxFontEnv.SplitTextLines(value);
        string processed = "";
        string sNewLine = "";
        for (int i = 0; i < lines.Count; i++)
        {
            var tokenLine = LineList.AddLine();
            // 'aaa{自自定颜色|100}bbbb'（原文 780 注释）
            processed = processed + sNewLine
                + DxImageButtonExUnit.ProcessButtonText(lines[i], tokenLine, CaptionColor.Up);
            sNewLine = "\r\n";                       // Delphi sLineBreak
        }

        LineList.RecalSize();

        DxControlOps.SetCaptionA(this, processed);   // 原文 789 `inherited SetCaptionA(Value)`
    }

    /// <summary>
    /// 原文 792-822 <c>SetCaptionV</c>（覆写基类）：**整段重建逻辑被注释掉**
    /// （原文 799 注释：HZQ 20230628 —— SetCaptionV 改变了已设置好的数据图像数据，
    /// 所以只更新文本），只剩 `inherited SetCaptionV(Value)`。
    /// 托管侧照抄留档，不做任何行表重建。
    /// </summary>
    public void SetCaptionVV2(string value)
    {
        // 原文 800-820（被注释掉的整段）照抄留档：
        //   (*
        //   FLineList.Clear;
        //   SL := TStringList.Create;
        //   try
        //     SL.Delimiter := '\';
        //     SL.Text := Value;
        //     Value := '';
        //     sNewLine := '';
        //     for I := 0 to SL.Count - 1 do begin
        //       TokenLine := FLineList.AddLine;
        //       // 'aaa{自自定颜色|100}bbbb'
        //       Value := Value + sNewLine + ProcessButtonText(SL.Strings[I], TokenLine, CaptionColor.Up);
        //       sNewLine := sLineBreak;
        //     end;
        //     FLineList.RecalSize;
        //   finally
        //     SL.Free;
        //   end;
        //   *)
        DxControlOps.SetCaptionV(this, value);       // 原文 821 `inherited SetCaptionV(Value)`
    }

    /// <summary>
    /// 原文 824-887 <c>DoDrawCaption</c>（覆写基类）—— 两轮绘制：
    ///
    /// **第一轮**（855-867）：逐行逐 token，**只画非 `TTokenText` 的 token**（图片/播放图），
    /// 且一律画在 `vtRect` 的**左上原点**（`Token.Paint(vtRect.Left, vtRect.Top)`）——
    /// 即图片 token **不参与居中**，只靠自身 OffsetX/OffsetY 定位（原文怪癖）。
    ///
    /// **第二轮**（869-886）：逐行处理文本 token。行矩形 `RLine` 的两端按
    /// `R.Left + (R 宽 - 行宽) div 2` 居中，起点 `Pt := RLine.TopLeft`，
    /// 每个文本 token 画在 `(Pt.X + X, Pt.Y + Y)` 后 `Pt.X += Token.Width`；
    /// 行末 `RLine.Top += Line.Height + ExpandLineHeight`（**末行之后也会加**，但不影响输出）。
    ///
    /// `X`/`Y` 取 `CaptionOffsetX/Y`（按下时再叠加 `CaptionDownOffsetX/Y`），
    /// 而 `Enabled = False` 时保持 0（原文 839-848）。
    /// </summary>
    public override void DoDrawCaptionV2()
    {
        if (Caption == "") return;

        var vtRect = VirtualRect;
        int x = 0;
        int y = 0;
        if (Enabled)
        {
            if (MouseDowned)                              // 原文 `if MouseDowned {or Checked} then`
            {
                x = CaptionOffsetX + CaptionDownOffsetX;
                y = CaptionOffsetY + CaptionDownOffsetY;
            }
            else
            {
                x = CaptionOffsetX;
                y = CaptionOffsetY;
            }
        }

        var r = TDxRect.Empty;
        r.Left = vtRect.Left + (Width - LineList.Width) / 2;
        r.Top = vtRect.Top + (Height - LineList.Height) / 2;
        r.Right = r.Left + LineList.Width;
        r.Bottom = r.Top + LineList.Height;

        // ---- 第一轮：非文本 token（原文 855-867）----
        for (int i = 0; i < LineList.Count; i++)
        {
            var line = LineList[i];
            for (int ii = 0; ii < line.Count; ii++)
            {
                var token = line[ii];
                // 原文 859-861（被注释掉的调试输出）：
                //   //if Token is TTokenImage then begin
                //   //    OutputDebugString('Hello');
                //   //end;
                if (!(token is TTokenText))
                {
                    token.Paint(vtRect.Left, vtRect.Top);
                }
            }
        }

        // ---- 第二轮：文本 token，逐行居中（原文 869-886）----
        var rLine = r;
        for (int i = 0; i < LineList.Count; i++)
        {
            var line = LineList[i];

            rLine.Left = r.Left + (r.Right - r.Left - line.Width) / 2;
            rLine.Right = rLine.Left + line.Width;

            int ptX = rLine.Left;
            int ptY = rLine.Top;
            for (int ii = 0; ii < line.Count; ii++)
            {
                var token = line[ii];
                if (token is TTokenText)
                {
                    token.Paint(ptX + x, ptY + y);
                    ptX = ptX + token.Width;
                }
            }

            rLine.Top = rLine.Top + line.Height + DxImageButtonExUnit.ExpandLineHeight;
        }
    }
}
