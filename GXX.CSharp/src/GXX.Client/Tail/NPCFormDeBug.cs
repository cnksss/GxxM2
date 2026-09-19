// 源单元：Source/Client-HGE/NPCFormDeBug.pas（原文 451 行 / CRLF 计入 497 行）
// 原文 uses：Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
//           DxImageButton, Dialogs, StdCtrls, Fstate, MShare, GameImages, Spin, DxControls, DxCanvas
// 同名 .dfm 存在（Source/Client-HGE/NPCFormDeBug.dfm，**文本格式**，101 行）。
//
// 本文件覆盖该单元的**纯逻辑**部分（这也是该单元真正的价值所在）：
//   ① ColorTo256        —— TColor(0x00BBGGRR) → 调色板索引（原文 :99-134）
//   ② CalcEuclidDistance —— 三维欧氏距离（原文 :89-97）
//   ③ GetFontStr        —— 由控件字色/字号/字体名生成脚本装饰串（原文 :141-168）
//   ④ DxControlToString —— 由控件属性生成 NPC 脚本标签（原文 :170-475，约 300 行分支）
//   ⑤ Start             —— 递归遍历控件树（原文 :477-488）
//   ⑥ Open              —— 用 g_EffectImageList（WIL 图库列表）填充下拉框（原文 :51-62）
//
// 依赖收敛：DxControl 家族（TNpcLabel/TNpcScrollBox/TNpcItemButton/TNpcInputEdit/
// TNpcItemBoxButton/TNpcProgressBoxButton/TNpcUserItemButton/TCountDownLabel/
// TImgCountDownButton/TNpcButton）与 TGameImages 由另一条车道（DxComponent）负责，
// 故此处定义**只读视图接缝** <see cref="INpcControlView"/> + <see cref="INpcControlNode"/>，
// 由接入方（DxComponent 全量移植后）适配；本单元的脚本生成逻辑本身与这些接缝一一对应。
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GXX.Client.Tail;

/// <summary>原文 <c>TRGBQuad</c>（Graphics.pas），用于调色板表。</summary>
public struct TRGBQuad
{
    /// <summary>原文 TRGBQuad.rgbBlue</summary>
    public byte rgbBlue;
    /// <summary>原文 TRGBQuad.rgbGreen</summary>
    public byte rgbGreen;
    /// <summary>原文 TRGBQuad.rgbRed</summary>
    public byte rgbRed;
    /// <summary>原文 TRGBQuad.rgbReserved</summary>
    public byte rgbReserved;
}

/// <summary>
/// NPC 调试窗体的<b>控件只读视图</b>接缝。
/// <para>原文直接访问 Delphi 控件的具体类型（<c>D is TNpcLabel</c> 等）；托管侧的
/// DxComponent 控件族由另一条车道负责，故本单元以本接口承载脚本生成所需的全部属性，
/// 由接入方按<b>实际控件类型</b>适配。每个成员都标注了它在原文里的来源行。</para>
/// </summary>
public interface INpcControlView
{
    /// <summary>原文 <c>D.ControlID</c>（:204-207：为 0 时改用 99999）</summary>
    int ControlID { get; }
    /// <summary>原文 <c>D.Caption</c></summary>
    string Caption { get; }
    /// <summary>原文 <c>D.Hint</c></summary>
    string Hint { get; }
    /// <summary>原文 <c>D.Left</c></summary>
    int Left { get; }
    /// <summary>原文 <c>D.Top</c></summary>
    int Top { get; }
    /// <summary>原文 <c>D.Width</c></summary>
    int Width { get; }
    /// <summary>原文 <c>D.Height</c></summary>
    int Height { get; }
    /// <summary>原文 <c>D.ImageIndex.Image</c> 的 <c>GetWilId</c>（:136-139 返回 g_EffectImageList.IndexOfObject）</summary>
    int ImageWilId { get; }
    /// <summary>原文 <c>D.ImageIndex.Up</c></summary>
    int ImageIndexUp { get; }
    /// <summary>原文 <c>D.ImageIndex.Hot</c></summary>
    int ImageIndexHot { get; }
    /// <summary>原文 <c>D.ImageIndex.Down</c></summary>
    int ImageIndexDown { get; }
    /// <summary>原文 <c>D.CaptionColor.Up.Color</c>（TColor 0x00BBGGRR）</summary>
    int CaptionColorUpColor { get; }
    /// <summary>原文 <c>D.CaptionColor.Down.Color</c>（:155 用来判"是否非默认白"）</summary>
    int CaptionColorDownColor { get; }
    /// <summary>原文 <c>D.CaptionColor.Up.Style</c> 里是否含 <c>fsBold</c>（:159）</summary>
    bool CaptionBold { get; }
    /// <summary>原文 <c>D.CaptionColor.Up.Size</c>（:162）</summary>
    int CaptionSize { get; }
    /// <summary>原文 <c>D.CaptionColor.Up.Name</c>（:165）</summary>
    string CaptionFontName { get; }
    /// <summary>原文 <c>TNpcLabel.m_AutoColors</c> 各元素的 TColor（:149-154 生成 AUTOCOLOR 列表）</summary>
    IReadOnlyList<int> AutoColors { get; }
    /// <summary>原文 <c>m_sCmd</c>（各分支通用的 <c>/cmd</c> 后缀）</summary>
    string Cmd { get; }

    /// <summary>原文 <c>D is TNpcLabel</c></summary>
    bool IsNpcLabel { get; }
    /// <summary>原文 <c>D is TNpcScrollBox</c></summary>
    bool IsNpcScrollBox { get; }
    /// <summary>原文 <c>D is TNpcItemButton</c></summary>
    bool IsNpcItemButton { get; }
    /// <summary>原文 <c>D is TNpcInputEdit</c></summary>
    bool IsNpcInputEdit { get; }
    /// <summary>原文 <c>D is TNpcItemBoxButton</c></summary>
    bool IsNpcItemBoxButton { get; }
    /// <summary>原文 <c>D is TNpcProgressBoxButton</c></summary>
    bool IsNpcProgressBoxButton { get; }
    /// <summary>原文 <c>D is TNpcUserItemButton</c></summary>
    bool IsNpcUserItemButton { get; }
    /// <summary>原文 <c>D is TCountDownLabel</c></summary>
    bool IsCountDownLabel { get; }
    /// <summary>原文 <c>D is TImgCountDownButton</c></summary>
    bool IsImgCountDownButton { get; }
    /// <summary>原文 <c>D is TNpcButton</c></summary>
    bool IsNpcButton { get; }

    // ── TNpcScrollBox（原文 :225-237）──
    /// <summary>原文 <c>TNpcScrollBox.m_List.DelimitedText</c></summary>
    string ScrollBoxDelimitedText { get; }
    /// <summary>原文 <c>TNpcScrollBox.m_List.Count</c></summary>
    int ScrollBoxListCount { get; }
    /// <summary>原文 <c>Integer(TNpcScrollBox.MouseHorizontal)</c></summary>
    bool ScrollBoxMouseHorizontal { get; }

    // ── TNpcItemButton（原文 :238-249）──
    /// <summary>原文 <c>TNpcItemButton.m_nFaceIndex</c></summary>
    int ItemFaceIndex { get; }
    /// <summary>原文 <c>TNpcItemButton.m_nCount</c></summary>
    int ItemCount { get; }
    /// <summary>原文 <c>Integer(TNpcItemButton.m_boShowBorder)</c></summary>
    bool ItemShowBorder { get; }
    /// <summary>原文 <c>TNpcItemButton.m_Light</c></summary>
    int ItemLight { get; }

    // ── TNpcInputEdit（原文 :250-278）──
    /// <summary>原文 <c>TNpcInputEdit.m_IsNumber</c></summary>
    bool EditIsNumber { get; }
    /// <summary>原文 <c>TNpcInputEdit.m_ID</c></summary>
    int EditId { get; }
    /// <summary>原文 <c>TNpcInputEdit.Transparent</c></summary>
    bool EditTransparent { get; }
    /// <summary>原文 <c>TNpcInputEdit.BackgroundColor</c></summary>
    int EditBackgroundColor { get; }
    /// <summary>原文 <c>TNpcInputEdit.DrawBorder</c></summary>
    bool EditDrawBorder { get; }
    /// <summary>原文 <c>TNpcInputEdit.BorderColor.Up.Color</c></summary>
    int EditBorderColor { get; }
    /// <summary>原文 <c>TNpcInputEdit.Font.Color</c></summary>
    int EditFontColor { get; }
    /// <summary>原文 <c>TNpcInputEdit.m_MinValue</c></summary>
    int EditMinValue { get; }
    /// <summary>原文 <c>TNpcInputEdit.m_MaxValue</c></summary>
    int EditMaxValue { get; }
    /// <summary>原文 <c>TNpcInputEdit.m_ValidityTips</c></summary>
    string EditValidityTips { get; }
    /// <summary>原文 <c>TNpcInputEdit.HintText</c></summary>
    string EditHintText { get; }
    /// <summary>原文 <c>TNpcInputEdit.HintTextFont.Color</c></summary>
    int EditHintTextFontColor { get; }

    // ── TNpcItemBoxButton（原文 :279-291）──
    /// <summary>原文 <c>TNpcItemBoxButton.m_nIndex</c></summary>
    int ItemBoxIndex { get; }
    /// <summary>原文 <c>TNpcItemBoxButton.m_StdModes</c></summary>
    string ItemBoxStdModes { get; }

    // ── TNpcProgressBoxButton（原文 :292-320）──
    /// <summary>原文 <c>m_nBgIndex</c></summary>
    int ProgressBgIndex { get; }
    /// <summary>原文 <c>m_nProgressStartIndex</c></summary>
    int ProgressStartIndex { get; }
    /// <summary>原文 <c>m_nProgressCount</c></summary>
    int ProgressCount { get; }
    /// <summary>原文 <c>m_nProgressRefresTime</c></summary>
    int ProgressRefreshTime { get; }
    /// <summary>原文 <c>m_nProgressOffsetX</c></summary>
    int ProgressOffsetX { get; }
    /// <summary>原文 <c>m_nProgressOffsetY</c></summary>
    int ProgressOffsetY { get; }
    /// <summary>原文 <c>m_nProgressMinValue</c></summary>
    int ProgressMinValue { get; }
    /// <summary>原文 <c>m_nProgressMaxValue</c></summary>
    int ProgressMaxValue { get; }
    /// <summary>原文 <c>m_nProgressValue</c></summary>
    int ProgressValue { get; }
    /// <summary>原文 <c>m_nProgressDist</c></summary>
    int ProgressDist { get; }
    /// <summary>原文 <c>m_nProgressTextColor</c></summary>
    int ProgressTextColor { get; }
    /// <summary>原文 <c>m_nProgressTextOffsetX</c></summary>
    int ProgressTextOffsetX { get; }
    /// <summary>原文 <c>m_nProgressTextOffsetY</c></summary>
    int ProgressTextOffsetY { get; }
    /// <summary>原文 <c>m_sText</c></summary>
    string ProgressText { get; }

    // ── TNpcUserItemButton（原文 :321-333）──
    /// <summary>原文 <c>TNpcUserItemButton.m_nIndex</c></summary>
    int UserItemIndex { get; }
    /// <summary>原文 <c>TNpcUserItemButton.m_Light</c></summary>
    int UserItemLight { get; }

    // ── TCountDownLabel（原文 :334-350）──
    /// <summary>原文 <c>TCountDownLabel.m_OldCountDownValue</c></summary>
    int CountDownOldValue { get; }
    /// <summary>原文 <c>TCountDownLabel.m_LoopCount</c></summary>
    int CountDownLoopCount { get; }

    // ── TImgCountDownButton（原文 :351-367）──
    /// <summary>原文 <c>TImgCountDownButton.m_ImgStartIndex</c></summary>
    int ImgCountDownStartIndex { get; }
    /// <summary>原文 <c>TImgCountDownButton.m_ImgSpace</c></summary>
    int ImgCountDownSpace { get; }

    // ── TNpcButton（原文 :368-474）──
    /// <summary>原文 <c>TNpcButton.m_ACaption</c>（决定走哪个分支：IMG/PLAYIMG/IMGEX/…）</summary>
    string ButtonACaption { get; }
    /// <summary>原文 <c>TNpcButton.m_sPostText</c></summary>
    string ButtonPostText { get; }
    /// <summary>原文 <c>TNpcButton.m_nStartImageIndex</c></summary>
    int ButtonStartImageIndex { get; }
    /// <summary>原文 <c>TNpcButton.m_nStopImageCount</c></summary>
    int ButtonStopImageCount { get; }
    /// <summary>原文 <c>TNpcButton.m_dwPlayImageTime</c></summary>
    int ButtonPlayImageTime { get; }
    /// <summary>原文 <c>TNpcButton.AddData1</c></summary>
    int ButtonAddData1 { get; }
    /// <summary>原文 <c>TNpcButton.BlendMode</c>（:387/:428 判 <c>&lt;&gt; 2</c>）</summary>
    int ButtonBlendMode { get; }
    /// <summary>原文 <c>TNpcButton.m_nShowBG</c></summary>
    bool ButtonShowBG { get; }
}

/// <summary>原文 <c>AOwner.Control[I]</c> + <c>AOwner.ComponentCount</c> 的接缝（控件树遍历）。</summary>
public interface INpcControlNode
{
    /// <summary>原文 <c>AOwner.ComponentCount</c></summary>
    int ComponentCount { get; }
    /// <summary>原文 <c>AOwner.Control[I]</c></summary>
    INpcControlView GetControl(int index);
}

/// <summary>
/// NPCFormDeBug.pas 1:1 移植（纯逻辑 + 接缝）。
/// </summary>
public static class NPCFormDeBug
{
    /// <summary>
    /// 原文 <c>g_DefColorTable</c>（DxCanvas.pas:45 <c>g_DefColorTable:TRGBQuads</c>，256 项）。
    /// <para>原文是全局调色板，由 <c>GameImages.pas</c> 在加载资源时填充
    /// （<c>Move(ColorArray, g_DefColorTable, ...)</c>）。托管侧以可注入的静态表承载，
    /// 默认值是 Delphi 的标准 256 色 VGA 调色板（原文未初始化时的形态）。</para>
    /// </summary>
    public static TRGBQuad[] g_DefColorTable = BuildDefaultColorTable();

    /// <summary>构造 256 项默认调色板（R/G/B 各 6/8/8 级的经典 256 色 VGA 表）。</summary>
    public static TRGBQuad[] BuildDefaultColorTable()
    {
        var t = new TRGBQuad[256];
        // 前 16 项：EGA/VGA 标准色（与 Delphi Graphics.palette 一致）
        int[,] ega =
        {
            {0,0,0},{128,0,0},{0,128,0},{128,128,0},{0,0,128},{128,0,128},{0,128,128},{192,192,192},
            {128,128,128},{255,0,0},{0,255,0},{255,255,0},{0,0,255},{255,0,255},{0,255,255},{255,255,255},
        };
        for (int i = 0; i < 16; i++)
        {
            t[i].rgbRed = (byte)ega[i, 0];
            t[i].rgbGreen = (byte)ega[i, 1];
            t[i].rgbBlue = (byte)ega[i, 2];
        }
        // 16..231：6×6×6 色立方（levels 0,51,102,153,204,255）
        int idx = 16;
        int[] lv = { 0, 51, 102, 153, 204, 255 };
        for (int r = 0; r < 6; r++)
            for (int g = 0; g < 6; g++)
                for (int b = 0; b < 6; b++)
                {
                    t[idx].rgbRed = (byte)lv[r];
                    t[idx].rgbGreen = (byte)lv[g];
                    t[idx].rgbBlue = (byte)lv[b];
                    idx++;
                }
        // 232..255：灰阶
        for (int i = 0; i < 24; i++)
        {
            byte v = (byte)(8 + i * 10);
            t[232 + i].rgbRed = v;
            t[232 + i].rgbGreen = v;
            t[232 + i].rgbBlue = v;
        }
        return t;
    }

    /// <summary>测试接缝：把调色板复位为默认表。</summary>
    public static void ResetColorTableForTest() => g_DefColorTable = BuildDefaultColorTable();

    /// <summary>
    /// 原文 :89-97 <c>function CalcEuclidDistance(r1, r2, g1, g2, b1, b2: Integer): Integer;</c>
    /// （函数内的局部嵌套函数，原文注释 <c>//HZQ 20230527</c>）
    /// <para><c>Result := Trunc(sqrt((valR * ValR) + (valG * ValG) + (valB * ValB) + 0.5));</c>
    /// —— 注意 <c>+ 0.5</c> 是**四舍五入**（Trunc 向零取整）。</para>
    /// </summary>
    public static int CalcEuclidDistance(int r1, int r2, int g1, int g2, int b1, int b2)
    {
        int valR = r2 - r1;
        int valG = g2 - g1;
        int valB = b2 - b1;
        return (int)Math.Truncate(Math.Sqrt((valR * valR) + (valG * valG) + (valB * valB) + 0.5));
    }

    /// <summary>
    /// 原文 :99-134 <c>function ColorTo256(c: TColor): Byte;</c>
    ///
    /// <para><c>TColor</c> 是 <c>0x00BBGGRR</c>：<c>R := c and $FF</c>、<c>G := (c and $FF00) shr 8</c>、
    /// <c>B := (c and $FF0000) shr 16</c>。</para>
    ///
    /// <para><b>原文缺陷（见报告 §5）</b>：欧氏距离的"最小距离"分支里
    /// <c>if nCurED &lt; nMinEd then begin Result := I; end;</c> —— 只更新 <c>Result</c>，
    /// **忘记更新 <c>nMinEd</c>**（原文 :127-129）。于是 <c>nMinEd</c> 恒为索引 0 的距离，
    /// 结果退化成"第一个距离小于 d(0) 的索引"，而不是真正的最小距离索引。
    /// 本移植**逐行保留**该行为，并由测试显式锁住。</para>
    /// </summary>
    public static byte ColorTo256(int c)
    {
        int R = c & 0xFF;
        int G = (c & 0xFF00) >> 8;
        int B = (c & 0xFF0000) >> 16;

        // 原文如此（NPCFormDeBug.pas:109）：nRlt := -1; //HZQ 20230527 默认返回-1...
        int nRlt = -1;
        for (int I = 0; I <= 256 - 1; I++)
        {
            if (g_DefColorTable[I].rgbBlue == B &&
                g_DefColorTable[I].rgbRed == R &&
                g_DefColorTable[I].rgbGreen == G)
            {
                nRlt = I;
                break;
            }
        }

        /* 原文如此（NPCFormDeBug.pas:117-121）：以下通过计算两个颜色的欧氏距离来确定最佳匹配索引
           d = sqrt((R2 - R1)^2 + (G2 - G1)^2 + (B2 - B1)^2) ... */

        if (nRlt < 0)
        {
            int Result = 0;
            int nMinEd = CalcEuclidDistance(R, g_DefColorTable[0].rgbRed,
                                            G, g_DefColorTable[0].rgbGreen,
                                            B, g_DefColorTable[0].rgbBlue);
            for (int i = 1; i <= 256 - 1; i++)
            {
                int nCurED = CalcEuclidDistance(R, g_DefColorTable[i].rgbRed,
                                                G, g_DefColorTable[i].rgbGreen,
                                                B, g_DefColorTable[i].rgbBlue);
                if (nCurED < nMinEd)
                {
                    // 原文如此（NPCFormDeBug.pas:127-129）：**没有** nMinEd := nCurED;
                    Result = i;
                }
            }
            return (byte)Result;
        }
        return (byte)nRlt;
    }

    /// <summary>TColor 打包（Delphi <c>RGB(r,g,b)</c>）：<c>r or (g shl 8) or (b shl 16)</c>。</summary>
    public static int RGB(int r, int g, int b) => (r & 0xFF) | ((g & 0xFF) << 8) | ((b & 0xFF) << 16);

    /// <summary>原文 <c>clWhite</c>（Graphics.pas）= <c>$FFFFFF</c></summary>
    public const int clWhite = 0xFFFFFF;

    /// <summary>
    /// 原文 :141-168 <c>function GetFontStr(D: TDxImageButton): string;</c>
    /// <para>生成形如 <c>AUTOCOLOR=1,2,:FBOLD:FSIZE=12:FNAME=宋体:</c> 的装饰串。
    /// 原文的空 <c>Caption</c> 直接 <c>Exit</c>（返回空串）。</para>
    /// <para><b>差异断言要点</b>：字体名默认比较串是 <c>'宋体'</c>（:165）与字号默认 <c>9</c>（:162），
    /// 都是**硬编码**的 Delphi 默认值，不是读控件的默认字体。</para>
    /// </summary>
    public static string GetFontStr(INpcControlView D)
    {
        if (D == null) return string.Empty;
        string Result = string.Empty;

        // 原文 :147：if D.Caption = '' then Exit;
        if (D.Caption == string.Empty) return Result;

        // 原文 :149：if (D is TNpcLabel) and (TNpcLabel(D).m_AutoColors.Count > 0)
        if (D.IsNpcLabel && D.AutoColors != null && D.AutoColors.Count > 0)
        {
            Result += "AUTOCOLOR=";
            for (int I = 0; I <= D.AutoColors.Count - 1; I++)
            {
                // 原文 :152：Result := Result + Format('%d,', [ColorTo256(Integer(...Items[I]))]);
                Result += ColorTo256(D.AutoColors[I]).ToString(CultureInfo.InvariantCulture) + ",";
            }
            Result += ":";
        }
        else if (D.CaptionColorDownColor != clWhite)
        {
            // 原文 :156：Result := Result + Format('FCOLOR=%d:', [ColorTo256(D.CaptionColor.Up.Color)]);
            Result += "FCOLOR=" + ColorTo256(D.CaptionColorUpColor).ToString(CultureInfo.InvariantCulture) + ":";
        }

        if (D.CaptionBold)
            Result += "FBOLD:";

        if (D.CaptionSize != 9)
            Result += "FSIZE=" + D.CaptionSize.ToString(CultureInfo.InvariantCulture) + ":";

        if (D.CaptionFontName != "宋体")
            Result += "FNAME=" + D.CaptionFontName + ":";

        return Result;
    }

    // ── 原文 :184-194 的脚本标签常量 ─────────────────────────────────────
    /// <summary>原文 :185 — <c>C_IMG = 'IMG';</c></summary>
    public const string C_IMG = "IMG";
    /// <summary>原文 :186 — <c>C_PLAYIMG = 'PLAYIMG';</c></summary>
    public const string C_PLAYIMG = "PLAYIMG";
    /// <summary>原文 :187 — <c>C_IMGEX = 'IMGEX';</c></summary>
    public const string C_IMGEX = "IMGEX";
    /// <summary>原文 :188 — <c>C_IMGNUM = 'IMGNUM';</c></summary>
    public const string C_IMGNUM = "IMGNUM";
    /// <summary>原文 :189 — <c>C_IMGPAY = 'IMGPAY';</c></summary>
    public const string C_IMGPAY = "IMGPAY";
    /// <summary>原文 :190 — <c>C_PLAYIMGEX = 'PLAYIMGEX';</c></summary>
    public const string C_PLAYIMGEX = "PLAYIMGEX";
    /// <summary>原文 :191 — <c>C_LOOKS = 'LOOKS';</c></summary>
    public const string C_LOOKS = "LOOKS";
    /// <summary>原文 :192 — <c>C_DNITEMS = 'DNITEMS';</c></summary>
    public const string C_DNITEMS = "DNITEMS";
    /// <summary>原文 :193 — <c>C_STATEITEM = 'STATEITEM';</c></summary>
    public const string C_STATEITEM = "STATEITEM";
    /// <summary>原文 :194 — <c>C_NEWOPUI = 'NEWOPUI';</c></summary>
    public const string C_NEWOPUI = "NEWOPUI";

    /// <summary>
    /// 原文 :170-475 <c>function DxControlToString(D: TDxControl): string;</c>
    /// <para>按控件类型/按钮子类型生成 NPC 脚本标签。原文分支顺序**严格按原文**：
    /// Label → ScrollBox → ItemButton → InputEdit → ItemBoxButton → ProgressBoxButton →
    /// UserItemButton → CountDownLabel → ImgCountDownButton → NpcButton（其内再按
    /// <c>m_ACaption</c> 分 10 个子分支）。</para>
    /// <para><c>OffsetX / OffsetY</c> 在原文里被赋 0 且相关的两行计算**已被注释掉**（:201-202），
    /// 故本移植保留常量 0 加法（<c>D.Left + OffsetX</c>），以保字节级一致。</para>
    /// </summary>
    public static string DxControlToString(INpcControlView D)
    {
        string Result = string.Empty;
        if (D == null) return Result;

        string FontStr = string.Empty;
        // 原文 :198/200：OffsetX := 0; OffsetY := 0;（:201-202 的计算被注释掉）
        const int OffsetX = 0;
        const int OffsetY = 0;

        // 原文 :204-207：if D.ControlID = 0 then nDID := 99999 else nDID := D.ControlID;
        int nDID = D.ControlID == 0 ? 99999 : D.ControlID;

        if (D.IsNpcLabel)
        {
            // 原文 :209-224
            Result = string.Format(CultureInfo.InvariantCulture, "<{0}&TEXT", nDID);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.Caption);
            if (D.Hint != string.Empty) Result += "|" + D.Hint;
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.Left + OffsetX);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.Top + OffsetY);
            FontStr = GetFontStr(D);
            if (FontStr != string.Empty) Result += "{" + FontStr + "}";
            if (D.Cmd != string.Empty) Result += "/" + D.Cmd;
            Result += ">";
        }
        else if (D.IsNpcScrollBox)
        {
            // 原文 :225-237
            Result = string.Format(CultureInfo.InvariantCulture, "<{0}&SCROLLBOX", nDID);
            if (D.ScrollBoxListCount > 0)
                Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ScrollBoxDelimitedText);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ImageWilId);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ImageIndexUp);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.Left + OffsetX);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.Top + OffsetY);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.Width);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.Height);
            // 原文 :235：Format(':%d', [Integer(TNpcScrollBox(D).MouseHorizontal)])
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ScrollBoxMouseHorizontal ? 1 : 0);
            Result += ">";
        }
        else if (D.IsNpcItemButton)
        {
            // 原文 :238-249
            Result = string.Format(CultureInfo.InvariantCulture, "<{0}&ITEMSHOW", nDID);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ItemFaceIndex);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ItemCount);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.Left + OffsetX);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.Top + OffsetY);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ItemShowBorder ? 1 : 0);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ItemLight);
            if (D.Cmd != string.Empty) Result += "/" + D.Cmd;
            Result += ">";
        }
        else if (D.IsNpcInputEdit)
        {
            // 原文 :250-278
            if (D.EditIsNumber)
                Result = string.Format(CultureInfo.InvariantCulture, "<{0}&INPUTNUM", nDID);
            else
                Result = string.Format(CultureInfo.InvariantCulture, "<{0}&INPUTTEXT", nDID);

            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.EditId);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.Left + OffsetX);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.Top + OffsetY);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.Width);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.Height);
            if (D.EditTransparent) Result += ":-1";
            else Result += string.Format(CultureInfo.InvariantCulture, ":{0}", ColorTo256(D.EditBackgroundColor));
            if (!D.EditDrawBorder) Result += ":-1";
            else Result += string.Format(CultureInfo.InvariantCulture, ":{0}", ColorTo256(D.EditBorderColor));
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", ColorTo256(D.EditFontColor));
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.EditMinValue);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.EditMaxValue);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.EditValidityTips);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.EditHintText);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", ColorTo256(D.EditHintTextFontColor));
            Result += ">";
        }
        else if (D.IsNpcItemBoxButton)
        {
            // 原文 :279-291
            Result = string.Format(CultureInfo.InvariantCulture, "<{0}&ITEMBOX", nDID);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ItemBoxIndex);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ImageWilId);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ImageIndexUp);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.Left + OffsetX);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.Top + OffsetY);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.Width);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.Height);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ItemBoxStdModes);
            Result += ">";
        }
        else if (D.IsNpcProgressBoxButton)
        {
            // 原文 :292-320
            Result = string.Format(CultureInfo.InvariantCulture, "<{0}&PROGRESSBAR", nDID);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.Left + OffsetX);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.Top + OffsetY);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ImageWilId);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ProgressBgIndex);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ProgressStartIndex);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ProgressCount);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ProgressRefreshTime);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ProgressOffsetX);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ProgressOffsetY);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ProgressMinValue);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ProgressMaxValue);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ProgressValue);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ProgressDist);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", ColorTo256(D.ProgressTextColor));
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ProgressTextOffsetX);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ProgressTextOffsetY);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ProgressText);
            FontStr = GetFontStr(D);
            if (FontStr != string.Empty) Result += "{" + FontStr + "}";
            if (D.Cmd != string.Empty) Result += "/" + D.Cmd;
            Result += ">";
        }
        else if (D.IsNpcUserItemButton)
        {
            // 原文 :321-333（**注意标签写的是 PROGRESSBAR**，原文如此）
            Result = string.Format(CultureInfo.InvariantCulture, "<{0}&PROGRESSBAR", nDID);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.UserItemIndex);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.Left + OffsetX);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.Top + OffsetY);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ItemShowBorder ? 1 : 0);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.UserItemLight);
            if (D.Cmd != string.Empty) Result += "/" + D.Cmd;
            Result += ">";
        }
        else if (D.IsCountDownLabel)
        {
            // 原文 :334-350
            Result = string.Format(CultureInfo.InvariantCulture, "<{0}&COUNTDOWN", nDID);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.CountDownOldValue);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.CountDownLoopCount);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", ColorTo256(D.CaptionColorUpColor));
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.Left + OffsetX);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.Top + OffsetY);
            FontStr = GetFontStr(D);
            if (FontStr != string.Empty) Result += "{" + FontStr + "}";
            if (D.Cmd != string.Empty) Result += "/" + D.Cmd;
            Result += ">";
        }
        else if (D.IsImgCountDownButton)
        {
            // 原文 :351-367
            Result = string.Format(CultureInfo.InvariantCulture, "<{0}&IMGCOUNTDOWN", nDID);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.CountDownOldValue);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.CountDownLoopCount);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ImgCountDownStartIndex);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ImgCountDownSpace);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.Left + OffsetX);
            Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.Top + OffsetY);
            FontStr = GetFontStr(D);
            if (FontStr != string.Empty) Result += "{" + FontStr + "}";
            if (D.Cmd != string.Empty) Result += "/" + D.Cmd;
            Result += ">";
        }
        else if (D.IsNpcButton)
        {
            // 原文 :368-474
            Result = string.Format(CultureInfo.InvariantCulture, "<{0}&{1}", nDID, D.ButtonACaption);

            if (D.ButtonACaption == C_IMG)
            {
                // 原文 :371-378
                Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ImageIndexUp);
                Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ImageWilId);
                Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.Left + OffsetX);
                Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.Top + OffsetY);
                if (D.ButtonPostText != string.Empty)
                    Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ButtonPostText);
            }
            else if (D.ButtonACaption == C_PLAYIMG)
            {
                // 原文 :379-395
                Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ImageWilId);
                Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ButtonStartImageIndex);
                // 原文 :382：m_nStopImageCount - m_nStartImageIndex + 1
                Result += string.Format(CultureInfo.InvariantCulture, ":{0}",
                    D.ButtonStopImageCount - D.ButtonStartImageIndex + 1);
                Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ButtonPlayImageTime);
                Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.Left + OffsetX);
                Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.Top + OffsetY);
                // 原文 :387：Format(':%d', [Integer(NpcButton.BlendMode <> 2)])
                Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ButtonBlendMode != 2 ? 1 : 0);
                if (D.Hint != string.Empty)
                    Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.Hint);
                if (D.ButtonPostText != string.Empty)
                    Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ButtonPostText);
            }
            else if (D.ButtonACaption == C_IMGEX)
            {
                // 原文 :396-405
                Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ImageWilId);
                Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ImageIndexUp);
                Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ImageIndexHot);
                Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ImageIndexDown);
                Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.Left + OffsetX);
                Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.Top + OffsetY);
                if (D.ButtonPostText != string.Empty)
                    Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ButtonPostText);
            }
            else if (D.ButtonACaption == C_IMGNUM)
            {
                // 原文 :406-412
                Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ImageIndexUp);
                Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ImageIndexHot);
                Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ImageIndexDown);
                Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.Left + OffsetX);
                Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.Top + OffsetY);
            }
            else if (D.ButtonACaption == C_IMGPAY)
            {
                // 原文 :413-418（三处硬编码的 '请重新输入'）
                Result += ":请重新输入:请重新输入:请重新输入";
                Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.Left + OffsetX);
                Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.Top + OffsetY);
                Result += ":请重新输入";
            }
            else if (D.ButtonACaption == C_PLAYIMGEX)
            {
                // 原文 :419-435
                Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ImageWilId);
                Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ButtonStartImageIndex);
                Result += string.Format(CultureInfo.InvariantCulture, ":{0}",
                    D.ButtonStopImageCount - D.ButtonStartImageIndex + 1);
                Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ButtonPlayImageTime);
                // 原文 :424：AddData1 —— 注意 PLAYIMG 分支**不写**这个字段
                Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ButtonAddData1);
                Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.Left + OffsetX);
                Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.Top + OffsetY);
                Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ButtonBlendMode != 2 ? 1 : 0);
                if (D.Hint != string.Empty)
                    Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.Hint);
                if (D.ButtonPostText != string.Empty)
                    Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ButtonPostText);
            }
            else if (D.ButtonACaption == C_LOOKS)
            {
                // 原文 :436-443
                Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ImageIndexUp);
                Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.Left + OffsetX);
                Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.Top + OffsetY);
                Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ButtonShowBG ? 1 : 0);
                if (D.ButtonPostText != string.Empty)
                    Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ButtonPostText);
            }
            else if (D.ButtonACaption == C_DNITEMS)
            {
                // 原文 :444-451（与 LOOKS 分支**完全相同**的字段序列 —— 差异断言要点）
                Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ImageIndexUp);
                Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.Left + OffsetX);
                Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.Top + OffsetY);
                Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ButtonShowBG ? 1 : 0);
                if (D.ButtonPostText != string.Empty)
                    Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ButtonPostText);
            }
            else if (D.ButtonACaption == C_STATEITEM)
            {
                // 原文 :452-459（同样与 LOOKS/DNITEMS 同构）
                Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ImageIndexUp);
                Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.Left + OffsetX);
                Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.Top + OffsetY);
                Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ButtonShowBG ? 1 : 0);
                if (D.ButtonPostText != string.Empty)
                    Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ButtonPostText);
            }
            else if (D.ButtonACaption == C_NEWOPUI)
            {
                // 原文 :460-466（**没有** ButtonShowBG —— 与上面三个分支的唯一差异）
                Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ImageIndexUp);
                Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.Left + OffsetX);
                Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.Top + OffsetY);
                if (D.ButtonPostText != string.Empty)
                    Result += string.Format(CultureInfo.InvariantCulture, ":{0}", D.ButtonPostText);
            }

            FontStr = GetFontStr(D);
            if (FontStr != string.Empty) Result += "{" + FontStr + "}";
            if (D.Cmd != string.Empty) Result += "/" + D.Cmd;
            Result += ">";
        }

        return Result;
    }

    /// <summary>
    /// 原文 :477-488 <c>function Start(AOwner: TDxControl): string;</c>
    /// <para>递归遍历控件树：<c>if AOwner.ComponentCount = 0 then Exit;</c>，
    /// 然后 <c>for I := AOwner.ComponentCount - 1 downto 0</c> 逐个转脚本并递归子控件。</para>
    /// <para>托管侧把输出行收集到 <paramref name="output"/>（原文是写进 Memo2.Lines）。</para>
    /// </summary>
    public static void Start(INpcControlNode AOwner, IList<string> output)
    {
        if (AOwner == null || output == null) return;
        // 原文 :481-482：if AOwner.ComponentCount = 0 then Exit;
        if (AOwner.ComponentCount == 0) return;

        // 原文 :484：for I := AOwner.ComponentCount - 1 downto 0 do
        for (int I = AOwner.ComponentCount - 1; I >= 0; I--)
        {
            var child = AOwner.GetControl(I);
            output.Add(DxControlToString(child));
            // 原文 :486：Start(AOwner.Control[I]); —— 子控件需自己实现 INpcControlNode；
            // 接缝：DxComponent 控件族全量移植后由控件实现该接口，此处直接接入。
            if (child is INpcControlNode node) Start(node, output);
        }
    }

    /// <summary>
    /// 原文 :494-495（<c>Button2Click</c> 的主体）：
    /// <c>Memo2.Lines.Clear; Start(FrmDlg.DMerchantDlg);</c>
    /// </summary>
    public static List<string> BuildScriptLines(INpcControlNode root)
    {
        var lines = new List<string>();
        Start(root, lines);
        return lines;
    }

    /// <summary>
    /// 原文 :64-85 <c>procedure TFrmNPCDeBug.Button1Click(Sender: TObject);</c>
    /// 里对 Memo 文本的**预处理**（纯逻辑，供测试）：
    /// 去掉所有 <c>#13</c> 与 <c>#10</c>（原文两行 <c>StringReplace(..., [rfReplaceAll])</c>）。
    /// </summary>
    public static string StripCrLf(string memoText)
    {
        if (string.IsNullOrEmpty(memoText)) return memoText ?? string.Empty;
        // 原文如此（NPCFormDeBug.pas:70-71）：
        //   Str := StringReplace(Str, #13, '', [rfReplaceAll]);
        //   Str := StringReplace(Str, #10, '', [rfReplaceAll]);
        return memoText.Replace("\r", string.Empty).Replace("\n", string.Empty);
    }

    /// <summary>
    /// 原文 :51-62 <c>procedure TFrmNPCDeBug.Open();</c>
    /// <para>把 <c>g_EffectImageList</c>（WIL 图库列表）逐项加入 ComboBox，
    /// 再选中第一项。</para>
    /// </summary>
    /// <param name="comboBox">目标下拉框。</param>
    /// <param name="effectImageList">原文 <c>g_EffectImageList</c>（名称 → 图像对象）。</param>
    public static void Open(ComboBox comboBox, IEnumerable<KeyValuePair<string, object>> effectImageList)
    {
        if (comboBox == null) return;
        // 原文 :56：ComboBox1.Clear;
        comboBox.Items.Clear();
        // 原文 :57-59：for I := 0 to g_EffectImageList.Count - 1 do ComboBox1.AddItem(g_EffectImageList[I], nil);
        if (effectImageList != null)
        {
            foreach (var kv in effectImageList) comboBox.Items.Add(kv.Key);
        }
        // 原文 :60-61：if (ComboBox1.Items.Count > 0) then ComboBox1.ItemIndex := 0;
        if (comboBox.Items.Count > 0) comboBox.SelectedIndex = 0;
    }

    // ── DFM（NPCFormDeBug.dfm，**文本格式**，101 行）逐条对照 ───────────────
    /// <summary>DFM: object FrmNPCDeBug: TFrmNPCDeBug Left=-118 Top=182</summary>
    public const int DfmLeft = -118;
    /// <summary>DFM: Top=182</summary>
    public const int DfmTop = 182;
    /// <summary>DFM: Caption='NPC'#30028#38754#35843#35797 （= 'NPC界面调试'）</summary>
    public const string DfmCaption = "NPC界面调试";
    /// <summary>DFM: ClientHeight=400</summary>
    public const int DfmClientHeight = 400;
    /// <summary>DFM: ClientWidth=822</summary>
    public const int DfmClientWidth = 822;
    /// <summary>DFM: object Memo1: TMemo Left=0 Top=0 Width=561 Height=305 Font.Name=#23435#20307('宋体') Font.Height=-12 TabOrder=0</summary>
    public const string DfmMemo1FontName = "宋体";
    /// <summary>DFM: Memo1 Font.Height=-12</summary>
    public const int DfmMemo1FontHeight = -12;
    /// <summary>DFM: Memo1 Width=561 Height=305</summary>
    public const int DfmMemo1Width = 561;
    /// <summary>DFM: Memo1 Height=305</summary>
    public const int DfmMemo1Height = 305;
    /// <summary>DFM: object Memo2: TMemo Left=576 Top=0 Width=241 Height=305 TabOrder=2</summary>
    public const int DfmMemo2Left = 576;
    /// <summary>DFM: Memo2 Width=241 Height=305</summary>
    public const int DfmMemo2Width = 241;
    /// <summary>DFM: Memo2 Height=305</summary>
    public const int DfmMemo2Height = 305;
    /// <summary>DFM: object GroupBox1: TGroupBox Left=3 Top=312 Width=814 Height=73 Caption='NPC'#23545#35805#26694#32972#26223#35774#32622（= 'NPC对话框背景设置'）</summary>
    public const string DfmGroupBoxCaption = "NPC对话框背景设置";
    /// <summary>DFM: GroupBox1 Left=3 Top=312 Width=814 Height=73</summary>
    public const int DfmGroupBoxLeft = 3;
    /// <summary>DFM: GroupBox1 Top=312</summary>
    public const int DfmGroupBoxTop = 312;
    /// <summary>DFM: GroupBox1 Width=814</summary>
    public const int DfmGroupBoxWidth = 814;
    /// <summary>DFM: GroupBox1 Height=73</summary>
    public const int DfmGroupBoxHeight = 73;
    /// <summary>DFM: Label1 Caption=#22270#29255#36164#28304':'（= '图片资源:'）</summary>
    public const string DfmLabel1Caption = "图片资源:";
    /// <summary>DFM: Label2 Caption=#22270#29255#24207#21495':'（= '图片序号:'）</summary>
    public const string DfmLabel2Caption = "图片序号:";
    /// <summary>DFM: ComboBox1 Left=76 Top=23 Width=145 Height=22 Style=csOwnerDrawFixed ItemHeight=16 TabOrder=0</summary>
    public const int DfmComboBox1Left = 76;
    /// <summary>DFM: ComboBox1 Top=23 Width=145 Height=22</summary>
    public const int DfmComboBox1Top = 23;
    /// <summary>DFM: ComboBox1 Width=145 Height=22</summary>
    public const int DfmComboBox1Width = 145;
    /// <summary>DFM: ComboBox1 Height=22</summary>
    public const int DfmComboBox1Height = 22;
    /// <summary>DFM: SpinEdit1 Left=288 Top=23 Width=73 Height=22 MaxValue=0 MinValue=0 Value=0 TabOrder=1</summary>
    public const int DfmSpinEdit1Left = 288;
    /// <summary>DFM: SpinEdit1 Top=23 Width=73 Height=22</summary>
    public const int DfmSpinEdit1Top = 23;
    /// <summary>DFM: SpinEdit1 Width=73 Height=22</summary>
    public const int DfmSpinEdit1Width = 73;
    /// <summary>DFM: SpinEdit1 Height=22</summary>
    public const int DfmSpinEdit1Height = 22;
    /// <summary>DFM: Button1 Caption=#24212#29992'(&amp;A)'（= '应用(&amp;A)'）Left=480 Top=24 Width=75 Height=25 TabOrder=2</summary>
    public const string DfmButton1Caption = "应用(&A)";
    /// <summary>DFM: Button1 Left=480 Top=24 Width=75 Height=25</summary>
    public const int DfmButton1Left = 480;
    /// <summary>DFM: Button1 Top=24 Width=75 Height=25</summary>
    public const int DfmButton1Top = 24;
    /// <summary>DFM: Button1 Width=75 Height=25</summary>
    public const int DfmButton1Width = 75;
    /// <summary>DFM: Button1 Height=25</summary>
    public const int DfmButton1Height = 25;
    /// <summary>DFM: Button2 Caption=#20445#23384'(&amp;S)'（= '保存(&amp;S)'）Left=576 Top=24 Width=75 Height=25 TabOrder=3</summary>
    public const string DfmButton2Caption = "保存(&S)";
    /// <summary>DFM: Button2 Left=576 Top=24 Width=75 Height=25</summary>
    public const int DfmButton2Left = 576;
    /// <summary>DFM: Button2 Top=24 Width=75 Height=25</summary>
    public const int DfmButton2Top = 24;
    /// <summary>DFM: Button2 Width=75 Height=25</summary>
    public const int DfmButton2Width = 75;
    /// <summary>DFM: Button2 Height=25</summary>
    public const int DfmButton2Height = 25;

    /// <summary>
    /// 按 <c>NPCFormDeBug.dfm</c> 构造窗体（DFM 是文本格式，可逐条对齐）。
    /// </summary>
    public class TFrmNPCDeBug : Form
    {
        /// <summary>DFM: Memo1: TMemo</summary>
        public TextBox Memo1;          // DFM: TMemo ⇒ 多行 TextBox
        /// <summary>DFM: Memo2: TMemo</summary>
        public TextBox Memo2;
        /// <summary>DFM: GroupBox1: TGroupBox</summary>
        public GroupBox GroupBox1;
        /// <summary>DFM: Label1: TLabel（'图片资源:'）</summary>
        public Label Label1;
        /// <summary>DFM: Label2: TLabel（'图片序号:'）</summary>
        public Label Label2;
        /// <summary>DFM: ComboBox1: TComboBox（Style=csOwnerDrawFixed ItemHeight=16）</summary>
        public ComboBox ComboBox1;
        /// <summary>DFM: SpinEdit1: TSpinEdit（MaxValue=0 MinValue=0 Value=0）</summary>
        public NumericUpDown SpinEdit1;
        /// <summary>DFM: Button1: TButton（'应用(&amp;A)'）</summary>
        public Button Button1;
        /// <summary>DFM: Button2: TButton（'保存(&amp;S)'）</summary>
        public Button Button2;

        /// <summary>按 DFM 逐条构造。</summary>
        public TFrmNPCDeBug()
        {
            // DFM: FrmNPCDeBug Left=-118 Top=182 Caption='NPC界面调试'
            //      ClientHeight=400 ClientWidth=822
            Text = DfmCaption;
            ClientSize = new System.Drawing.Size(DfmClientWidth, DfmClientHeight);
            Left = DfmLeft;
            Top = DfmTop;
            StartPosition = FormStartPosition.Manual;

            // DFM: object Memo1: TMemo Left=0 Top=0 Width=561 Height=305
            //      Font.Name='宋体' Font.Height=-12 TabOrder=0
            Memo1 = new TextBox
            {
                Left = 0, Top = 0, Width = DfmMemo1Width, Height = DfmMemo1Height,
                Multiline = true, ScrollBars = ScrollBars.Vertical, TabIndex = 0,
                Font = new System.Drawing.Font(DfmMemo1FontName, 9f),
            };

            // DFM: object GroupBox1: TGroupBox Left=3 Top=312 Width=814 Height=73 Caption='NPC对话框背景设置'
            GroupBox1 = new GroupBox
            {
                Left = DfmGroupBoxLeft, Top = DfmGroupBoxTop,
                Width = DfmGroupBoxWidth, Height = DfmGroupBoxHeight,
                Text = DfmGroupBoxCaption, TabIndex = 1,
            };

            // DFM: object Label1: TLabel Left=16 Top=24 Width=52 Height=13 Caption='图片资源:'
            Label1 = new Label { Left = 16, Top = 24, Width = 52, Height = 13, Text = DfmLabel1Caption };
            // DFM: object Label2: TLabel Left=228 Top=24 Width=52 Height=13 Caption='图片序号:'
            Label2 = new Label { Left = 228, Top = 24, Width = 52, Height = 13, Text = DfmLabel2Caption };

            // DFM: object ComboBox1: TComboBox Left=76 Top=23 Width=145 Height=22
            //      Style=csOwnerDrawFixed ItemHeight=16 TabOrder=0
            ComboBox1 = new ComboBox
            {
                Left = DfmComboBox1Left, Top = DfmComboBox1Top,
                Width = DfmComboBox1Width, Height = DfmComboBox1Height,
                DropDownStyle = ComboBoxStyle.DropDownList,     // csOwnerDrawFixed ⇒ 不可编辑
                TabIndex = 0,
            };

            // DFM: object SpinEdit1: TSpinEdit Left=288 Top=23 Width=73 Height=22
            //      MaxValue=0 MinValue=0 Value=0 TabOrder=1
            SpinEdit1 = new NumericUpDown
            {
                Left = DfmSpinEdit1Left, Top = DfmSpinEdit1Top,
                Width = DfmSpinEdit1Width, Height = DfmSpinEdit1Height,
                Minimum = 0, Maximum = 0, Value = 0, TabIndex = 1,
            };

            // DFM: object Button1: TButton Left=480 Top=24 Width=75 Height=25 Caption='应用(&A)' TabOrder=2 OnClick=Button1Click
            Button1 = new Button
            {
                Left = DfmButton1Left, Top = DfmButton1Top,
                Width = DfmButton1Width, Height = DfmButton1Height,
                Text = DfmButton1Caption, TabIndex = 2,
            };
            // DFM: object Button2: TButton Left=576 Top=24 Width=75 Height=25 Caption='保存(&S)' TabOrder=3 OnClick=Button2Click
            Button2 = new Button
            {
                Left = DfmButton2Left, Top = DfmButton2Top,
                Width = DfmButton2Width, Height = DfmButton2Height,
                Text = DfmButton2Caption, TabIndex = 3,
            };

            GroupBox1.Controls.Add(Label1);
            GroupBox1.Controls.Add(Label2);
            GroupBox1.Controls.Add(ComboBox1);
            GroupBox1.Controls.Add(SpinEdit1);
            GroupBox1.Controls.Add(Button1);
            GroupBox1.Controls.Add(Button2);

            // DFM: object Memo2: TMemo Left=576 Top=0 Width=241 Height=305 TabOrder=2
            Memo2 = new TextBox
            {
                Left = DfmMemo2Left, Top = 0, Width = DfmMemo2Width, Height = DfmMemo2Height,
                Multiline = true, ScrollBars = ScrollBars.Vertical, TabIndex = 2,
                Font = new System.Drawing.Font(DfmMemo1FontName, 9f),
            };

            // DFM 声明顺序是 Memo1 / GroupBox1 / Memo2
            Controls.Add(Memo1);
            Controls.Add(GroupBox1);
            Controls.Add(Memo2);
        }
    }
}
