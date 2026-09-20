using System;
using System.Runtime.InteropServices;

namespace GXX.Client.DxComponent;

// =============================================================================================
// DIB.pas 1:1 移植 —— 第 5 片续：DoResample 与重采样滤波器组（DIB.pas 7117-7674）。
//
// 覆盖行号：
//   7126-7135  HermiteFilter
//   7141-7147  BoxFilter
//   7151-7159  TriangleFilter
//   7162-7174  BellFilter
//   7177-7193  SplineFilter
//   7196-7213  Lanczos3Filter（含内嵌 SinC）
//   7215-7241  MitchellFilter
//   7248-7279  TContributor / TContributorList / TCList / TRGB / TColorRGB / TRGBList
//   7305-7315  Color2RGB / RGB2Color（`{$ELSE}` 分支用，见下）
//   7118-7661  Resample（含 USESCANLINE 路径的两次 GetMem/FreeMem 与两趟采样）
//   7662-7674  TDIB.DoResample（外层脚手架）
//
// 结构说明（**有意偏离，已登记**）：原文把 8 个滤波器与 4 个记录/2 个颜色转换都声明为
// `Resample` 的**局部** function/type。托管侧为了能对"滤波器权重"直接做断言
// （任务要求：滤波器权重必须可断言），把这 8 个函数提到 `DibFusionFilters` 静态类，
// 函数体逐行照抄；4 个记录提到 `DibFusionFilters` 的嵌套类型（名字加 `Dib` 前缀以避免与
// 其它分片/单元重名，映射关系写在各类型的注释里）。`Resample` 仍保留为 `DoResample` 的局部函数。
//
// ★★ 原文头号缺陷（USE_SCANLINE 路径的 R/B 颠倒）：
//   DIB.pas:6 就是 `{$DEFINE USE_SCANLINE}`，故编译进去的是 `SourceLine^[...]` 那一条路径。
//   该路径把源行**按 TColorRGB（字段序 R,G,B）**解释：`Color := SourceLine^[n]` 取到的是
//   **内存第 0 字节（在 DIB 的 24bpp 里是 B 通道）**，赋给 `Color.R`；最终 `Dst` 的字节 0 也被
//   写成"R"。于是整条重采样链在 **R/B 互换**的坐标系里运算：读进来的 Color.R 实际是 B，
//   写出去的字节 0 实际是 B —— 只有"读 Dst 的 R 通道"的人会看到 B 的值。
//   原文 `{$ELSE}` 分支用的是 `TCanvas.Pixels`（TColor 语义正确），故这不是设计意图而是路径缺陷。
//   托管侧**照抄该语义**（用 byte* 明确定位字节 0/1/2），并在测试里用差异断言锁死。
// =============================================================================================

/// <summary>
/// DIB.pas 7248-7315 / 7126-7241 —— 原文 `Resample` 的局部类型与局部函数，提到此处以便直接断言。
/// 名字映射：
///   TContributor → DibContributor；TCList → DibCList；TRGB → DibRGB；TColorRGB → DibColorRGB。
/// </summary>
public static class DibFusionFilters
{
    // -----------------------------------------------------------------------------------------
    // DIB.pas 7248-7279 —— 局部类型
    // -----------------------------------------------------------------------------------------

    /// <summary>DIB.pas 7250-7253 —— `TContributor = record pixel: Integer; weight: Single; end;`（SizeOf = 8）。</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DibContributor
    {
        /// <summary>Source pixel</summary>
        public int pixel;
        /// <summary>Pixel weight</summary>
        public float weight;
    }

    /// <summary>DIB.pas 7259-7262 —— `TCList = record n: Integer; P: PContributorList; end;`。</summary>
    public struct DibCList
    {
        /// <summary>贡献者个数。</summary>
        public int n;
        /// <summary>DIB.pas `P: PContributorList`（原为 GetMem 裸块，托管侧改用托管数组）。</summary>
        public DibContributor[] P;
    }

    /// <summary>DIB.pas 7267-7269 —— `TRGB = packed record R, G, B: Single; end;`（SizeOf = 12）。</summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DibRGB
    {
        public float R;
        public float G;
        public float B;
    }

    /// <summary>
    /// DIB.pas 7272-7274 —— `TColorRGB = packed record R, G, B: Byte; end;`（SizeOf = **3**）。
    /// 纯字节记录，`Pack = 1` 足够（§24.3 移植陷阱 1 说的是 `fixed byte[N]` 会前移，本类型没有数组字段）。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DibColorRGB
    {
        public byte R;
        public byte G;
        public byte B;
    }

    // -----------------------------------------------------------------------------------------
    // DIB.pas 7126-7241 —— 8 个滤波器（函数体逐行照抄）
    // -----------------------------------------------------------------------------------------

    /// <summary>DIB.pas 7126-7135 —— Hermite: `f(t) = 2|t|^3 - 3|t|^2 + 1, -1 &lt;= t &lt;= 1`。</summary>
    public static float HermiteFilter(float Value)
    {
        // f(t) = 2|t|^3 - 3|t|^2 + 1, -1 <= t <= 1
        if (Value < 0.0f)
            Value = -Value;
        if (Value < 1.0f)
            return (2.0f * Value - 3.0f) * (Value * Value) + 1.0f;
        else
            return 0.0f;
    }

    /// <summary>DIB.pas 7141-7147 —— Box（a.k.a. "Nearest Neighbour"）；原文注释照抄见文件头。</summary>
    public static float BoxFilter(float Value)
    {
        if (Value > -0.5f && Value <= 0.5f)
            return 1.0f;
        else
            return 0.0f;
    }

    /// <summary>DIB.pas 7151-7159 —— Triangle（a.k.a. "Linear"/"Bilinear"）。</summary>
    public static float TriangleFilter(float Value)
    {
        if (Value < 0.0f)
            Value = -Value;
        if (Value < 1.0f)
            return 1.0f - Value;
        else
            return 0.0f;
    }

    /// <summary>DIB.pas 7162-7174 —— Bell。</summary>
    public static float BellFilter(float Value)
    {
        if (Value < 0.0f)
            Value = -Value;
        if (Value < 0.5f)
            return 0.75f - (Value * Value);
        else if (Value < 1.5f)
        {
            Value = Value - 1.5f;
            return 0.5f * (Value * Value);
        }
        else
            return 0.0f;
    }

    /// <summary>DIB.pas 7177-7193 —— B-spline。</summary>
    public static float SplineFilter(float Value)
    {
        float tt;
        if (Value < 0.0f)
            Value = -Value;
        if (Value < 1.0f)
        {
            tt = Value * Value;
            return 0.5f * tt * Value - tt + 2.0f / 3.0f;
        }
        else if (Value < 2.0f)
        {
            Value = 2.0f - Value;
            return 1.0f / 6.0f * (Value * Value) * Value;
        }
        else
            return 0.0f;
    }

    /// <summary>DIB.pas 7197-7205 —— Lanczos3 的内嵌 `SinC`。</summary>
    public static float SinC(float Value)
    {
        if (Value != 0.0f)
        {
            Value = Value * (float)Math.PI;
            return (float)Math.Sin(Value) / Value;
        }
        else
            return 1.0f;
    }

    /// <summary>DIB.pas 7196-7213 —— Lanczos3。</summary>
    public static float Lanczos3Filter(float Value)
    {
        if (Value < 0.0f)
            Value = -Value;
        if (Value < 3.0f)
            return SinC(Value) * SinC(Value / 3.0f);
        else
            return 0.0f;
    }

    /// <summary>DIB.pas 7215-7241 —— Mitchell（B = C = 1/3）。</summary>
    public static float MitchellFilter(float Value)
    {
        const float B = 1.0f / 3.0f;
        const float C = 1.0f / 3.0f;
        float tt;
        if (Value < 0.0f)
            Value = -Value;
        tt = Value * Value;
        if (Value < 1.0f)
        {
            Value = (((12.0f - 9.0f * B - 6.0f * C) * (Value * tt))
              + ((-18.0f + 12.0f * B + 6.0f * C) * tt)
              + (6.0f - 2 * B));
            return Value / 6.0f;
        }
        else if (Value < 2.0f)
        {
            Value = (((-1.0f * B - 6.0f * C) * (Value * tt))
              + ((6.0f * B + 30.0f * C) * tt)
              + ((-12.0f * B - 48.0f * C) * Value)
              + (8.0f * B + 24 * C));
            return Value / 6.0f;
        }
        else
            return 0.0f;
    }

    // -----------------------------------------------------------------------------------------
    // DIB.pas 7305-7315 —— 颜色转换（`{$ELSE}` 分支用）
    // -----------------------------------------------------------------------------------------

    /// <summary>
    /// DIB.pas 7305-7310 —— `Color2RGB(Color: TColor): TColorRGB`。
    /// `Result.R := Color and $000000FF`（低字节 → R 字段）。
    /// **本转换只被原文的 `{$ELSE}`（非 USE_SCANLINE）分支调用**；DIB.pas:6 已 `{$DEFINE USE_SCANLINE}`，
    /// 故实际编译进去的路径不走它 —— 保留以对齐原文并提供可断言面。
    /// </summary>
    public static DibColorRGB Color2RGB(int Color)
    {
        DibColorRGB Result;
        Result.R = unchecked((byte)(Color & 0x000000FF));
        Result.G = unchecked((byte)((Color & 0x0000FF00) >> 8));
        Result.B = unchecked((byte)((Color & 0x00FF0000) >> 16));
        return Result;
    }

    /// <summary>DIB.pas 7312-7315 —— `RGB2Color = Color.R or (Color.G shl 8) or (Color.B shl 16)`。</summary>
    public static int RGB2Color(DibColorRGB Color)
        => Color.R | (Color.G << 8) | (Color.B << 16);
}

/// <summary>DIB.pas 7117-7674 —— TDIB.DoResample（本文件）。</summary>
public partial class TDIB
{
    /// <summary>
    /// DIB.pas 7117-7674 1:1（`{$DEFINE USE_SCANLINE}` 分支，见文件头）。
    /// 两趟 16 进制定点式重采样：先按行权重把 `Src` 缩到中间图 `Work`（宽 = DstWidth、高 = SrcHeight），
    /// 再按列权重从 `Work` 缩到 `Dst`。
    /// 关键原文语义（全部照抄）：
    ///   * `if (xscale &lt; 1.0)` 走子采样分支（`Width := FWidth / xscale; fscale := 1.0 / xscale;`
    ///     且权重再 `/ fscale`），否则走超采样分支（**不**除 `fscale`）；
    ///   * `Left := floor(Center - Width)` / `Right := ceil(Center + Width)`（**不是**原文注释里写的
    ///     `ceil(...left)`/`floor(...right)`）；
    ///   * `weight = 0.0` 直接 `Continue`；越界索引反射：`j &lt; 0 → -j`，
    ///     `j &gt;= SrcWidth → SrcWidth - j + SrcWidth - 1`；
    ///     **该反射式在 `j &gt; 2*SrcWidth - 1` 时给出负下标**（如 SrcWidth=4、j=8 → n=-1），
    ///     于是会读到行首之前 / 缓冲区之外的内存 —— 原文如此（DIB.pas:7388/7427/7537/7576），
    ///     Delphi 无边界检查 ⇒ 结果依赖相邻堆内容。**实测**：`ftrLanczos3`/`ftrBSpline`/
    ///     `ftrMitchell`/`ftrBell` 在 `DstWidth &lt; SrcWidth` 时命中该路径，输出**不确定**；
    ///     `ftrBox`/`ftrTriangle`/`ftrHermite`（DefaultFilterRadius ≤ 1）在 4x4→2x2 下不触发，
    ///     故本片的精确断言只用这三个滤波器。托管侧用 `byte*` 原样越界读（不夹取、不抛），
    ///     以保持"同一段内存布局下行为一致"；测试**不**对越界路径做精确断言。
    ///
    ///   * 通道读的是**内存第 0/1/2 字节**（USE_SCANLINE 路径的 R/B 颠倒，见文件头）；
    ///   * `if SrcWidth &lt; 1 || SrcHeight &lt; 1` 才抛 `Exception('Source bitmap too small')` ——
    ///     `DoResample` 对**空 Self** 会先 `BB1.BitCount := 24`（1x1）再 `Assign(Self)`（回到 0x0），
    ///     故空图调用**可命中**该守卫（实测抛 'Source bitmap too small'）；
    ///   * `Work.Height := SrcHeight; Work.Width := DstWidth;` —— 两次 Set* 使 Work 先成 8bpp 再转换 24bpp；
    ///   * `DstHeight = 1` 时 `yscale = 0/… = 0` ⇒ `Width := FWidth/yscale = +Inf` ⇒
    ///     `Trunc(+Inf)` 抛 EInvalidOp（托管侧 ArithmeticException）；
    ///   * `SrcHeight = 1` 时 `Work` 只有 1 行 ⇒ `Work.ScanLine(1)` 抛 SScanline。
    /// `Src`/`Dst` 的 `BitCount := 24` 由本方法内部完成。
    /// </summary>
    public void DoResample(int AmountX, int AmountY, TFilterTypeResample TypeResample)
    {
        void Resample(TDIB Src, TDIB Dst, TFilterTypeResample filtertype, float FWidth)
        {
            float xscale, yscale;   // Zoom scale factors
            int I, j, k;            // Loop variables
            float Center;           // Filter calculation variables
            float Width, fscale, weight; // Filter calculation variables
            int Left, Right;        // Filter calculation variables
            int n;                  // Pixel number
            TDIB Work;
            DibFusionFilters.DibCList[] contrib;
            DibFusionFilters.DibRGB rgb = default;
            DibFusionFilters.DibColorRGB Color = default;
            int SrcWidth, SrcHeight, DstWidth, DstHeight;

            DstWidth = Dst.Width;
            DstHeight = Dst.Height;
            SrcWidth = Src.Width;
            SrcHeight = Src.Height;
            if (SrcWidth < 1 || SrcHeight < 1)
                throw new Exception("Source bitmap too small");

            // Create intermediate image to hold horizontal zoom
            Work = new TDIB();
            try
            {
                Work.SetHeight(SrcHeight);
                Work.SetWidth(DstWidth);
                // xscale := DstWidth / SrcWidth;
                // yscale := DstHeight / SrcHeight;
                // Improvement suggested by David Ullrich:
                if (SrcWidth == 1)
                    xscale = (float)DstWidth / SrcWidth;
                else
                    xscale = (float)(DstWidth - 1) / (SrcWidth - 1);
                if (SrcHeight == 1)
                    yscale = (float)DstHeight / SrcHeight;
                else
                    yscale = (float)(DstHeight - 1) / (SrcHeight - 1);
                // This implementation only works on 24-bit images because it uses
                // TDIB.Scanline
                // Src.PixelFormat := pf24bit;
                Src.SetBitCount(24);
                // Dst.PixelFormat := Src.PixelFormat;
                Dst.SetBitCount(24);
                // Work.PixelFormat := Src.PixelFormat;
                Work.SetBitCount(24);

                // --------------------------------------------
                // Pre-calculate filter contributions for a row
                // -----------------------------------------------
                contrib = AllocCList(DstWidth); // GetMem(contrib, DstWidth * SizeOf(TCList))
                // Horizontal sub-sampling
                // Scales from bigger to smaller width
                if (xscale < 1.0f)
                {
                    Width = FWidth / xscale;
                    fscale = 1.0f / xscale;
                    for (I = 0; I <= DstWidth - 1; I++)
                    {
                        contrib[I].n = 0;
                        contrib[I].P = AllocContributor(DibFusionSupport.Trunc(Width * 2.0 + 1));
                        Center = I / xscale;
                        // Original code:
                        // left := ceil(center - width);
                        // right := floor(center + width);
                        Left = (int)Math.Floor(Center - Width);
                        Right = (int)Math.Ceiling(Center + Width);
                        for (j = Left; j <= Right; j++)
                        {
                            switch (filtertype)
                            {
                                case TFilterTypeResample.ftrBox: weight = DibFusionFilters.BoxFilter((Center - j) / fscale) / fscale; break;
                                case TFilterTypeResample.ftrTriangle: weight = DibFusionFilters.TriangleFilter((Center - j) / fscale) / fscale; break;
                                case TFilterTypeResample.ftrHermite: weight = DibFusionFilters.HermiteFilter((Center - j) / fscale) / fscale; break;
                                case TFilterTypeResample.ftrBell: weight = DibFusionFilters.BellFilter((Center - j) / fscale) / fscale; break;
                                case TFilterTypeResample.ftrBSpline: weight = DibFusionFilters.SplineFilter((Center - j) / fscale) / fscale; break;
                                case TFilterTypeResample.ftrLanczos3: weight = DibFusionFilters.Lanczos3Filter((Center - j) / fscale) / fscale; break;
                                case TFilterTypeResample.ftrMitchell: weight = DibFusionFilters.MitchellFilter((Center - j) / fscale) / fscale; break;
                                default: weight = 0.0f; break;
                            }
                            if (weight == 0.0f)
                                continue;
                            if (j < 0)
                                n = -j;
                            else if (j >= SrcWidth)
                                n = SrcWidth - j + SrcWidth - 1;
                            else
                                n = j;
                            k = contrib[I].n;
                            contrib[I].n = contrib[I].n + 1;
                            contrib[I].P[k].pixel = n;
                            contrib[I].P[k].weight = weight;
                        }
                    }
                }
                else
                // Horizontal super-sampling
                // Scales from smaller to bigger width
                {
                    for (I = 0; I <= DstWidth - 1; I++)
                    {
                        contrib[I].n = 0;
                        contrib[I].P = AllocContributor(DibFusionSupport.Trunc(FWidth * 2.0 + 1));
                        Center = I / xscale;
                        // Original code:
                        // left := ceil(center - fwidth);
                        // right := floor(center + fwidth);
                        Left = (int)Math.Floor(Center - FWidth);
                        Right = (int)Math.Ceiling(Center + FWidth);
                        for (j = Left; j <= Right; j++)
                        {
                            switch (filtertype)
                            {
                                case TFilterTypeResample.ftrBox: weight = DibFusionFilters.BoxFilter(Center - j); break;
                                case TFilterTypeResample.ftrTriangle: weight = DibFusionFilters.TriangleFilter(Center - j); break;
                                case TFilterTypeResample.ftrHermite: weight = DibFusionFilters.HermiteFilter(Center - j); break;
                                case TFilterTypeResample.ftrBell: weight = DibFusionFilters.BellFilter(Center - j); break;
                                case TFilterTypeResample.ftrBSpline: weight = DibFusionFilters.SplineFilter(Center - j); break;
                                case TFilterTypeResample.ftrLanczos3: weight = DibFusionFilters.Lanczos3Filter(Center - j); break;
                                case TFilterTypeResample.ftrMitchell: weight = DibFusionFilters.MitchellFilter(Center - j); break;
                                default: weight = 0.0f; break;
                            }
                            if (weight == 0.0f)
                                continue;
                            if (j < 0)
                                n = -j;
                            else if (j >= SrcWidth)
                                n = SrcWidth - j + SrcWidth - 1;
                            else
                                n = j;
                            k = contrib[I].n;
                            contrib[I].n = contrib[I].n + 1;
                            contrib[I].P[k].pixel = n;
                            contrib[I].P[k].weight = weight;
                        }
                    }
                }

                // ----------------------------------------------------
                // Apply filter to sample horizontally from Src to Work
                // ----------------------------------------------------
                unsafe
                {
                    for (k = 0; k <= SrcHeight - 1; k++)
                    {
                        byte* SourceLine = (byte*)Src.ScanLine(k);
                        byte* DestPixel = (byte*)Work.ScanLine(k);
                        for (I = 0; I <= DstWidth - 1; I++)
                        {
                            rgb.R = 0.0f;
                            rgb.G = 0.0f;
                            rgb.B = 0.0f;
                            for (j = 0; j <= contrib[I].n - 1; j++)
                            {
                                // USE_SCANLINE：按 TColorRGB（字段序 R,G,B）解释源行的连续 3 字节
                                Color.R = SourceLine[contrib[I].P[j].pixel * 3];
                                Color.G = SourceLine[contrib[I].P[j].pixel * 3 + 1];
                                Color.B = SourceLine[contrib[I].P[j].pixel * 3 + 2];
                                weight = contrib[I].P[j].weight;
                                if (weight == 0.0f)
                                    continue;
                                rgb.R = rgb.R + Color.R * weight;
                                rgb.G = rgb.G + Color.G * weight;
                                rgb.B = rgb.B + Color.B * weight;
                            }
                            if (rgb.R > 255.0f)
                                Color.R = 255;
                            else if (rgb.R < 0.0f)
                                Color.R = 0;
                            else
                                Color.R = unchecked((byte)DibFusionSupport.Round(rgb.R));
                            if (rgb.G > 255.0f)
                                Color.G = 255;
                            else if (rgb.G < 0.0f)
                                Color.G = 0;
                            else
                                Color.G = unchecked((byte)DibFusionSupport.Round(rgb.G));
                            if (rgb.B > 255.0f)
                                Color.B = 255;
                            else if (rgb.B < 0.0f)
                                Color.B = 0;
                            else
                                Color.B = unchecked((byte)DibFusionSupport.Round(rgb.B));
                            // Set new pixel value
                            DestPixel[0] = Color.R;
                            DestPixel[1] = Color.G;
                            DestPixel[2] = Color.B;
                            // Move on to next column
                            DestPixel += 3;
                        }
                    }
                }

                // Free the memory allocated for horizontal filter weights
                for (I = 0; I <= DstWidth - 1; I++)
                    contrib[I].P = null; // FreeMem(contrib^[I].P)

                contrib = null;          // FreeMem(contrib)

                // -----------------------------------------------
                // Pre-calculate filter contributions for a column
                // -----------------------------------------------
                contrib = AllocCList(DstHeight); // GetMem(contrib, DstHeight * SizeOf(TCList))
                // Vertical sub-sampling
                // Scales from bigger to smaller height
                if (yscale < 1.0f)
                {
                    Width = FWidth / yscale;
                    fscale = 1.0f / yscale;
                    for (I = 0; I <= DstHeight - 1; I++)
                    {
                        contrib[I].n = 0;
                        contrib[I].P = AllocContributor(DibFusionSupport.Trunc(Width * 2.0 + 1));
                        Center = I / yscale;
                        // Original code:
                        // left := ceil(center - width);
                        // right := floor(center + width);
                        Left = (int)Math.Floor(Center - Width);
                        Right = (int)Math.Ceiling(Center + Width);
                        for (j = Left; j <= Right; j++)
                        {
                            switch (filtertype)
                            {
                                case TFilterTypeResample.ftrBox: weight = DibFusionFilters.BoxFilter((Center - j) / fscale) / fscale; break;
                                case TFilterTypeResample.ftrTriangle: weight = DibFusionFilters.TriangleFilter((Center - j) / fscale) / fscale; break;
                                case TFilterTypeResample.ftrHermite: weight = DibFusionFilters.HermiteFilter((Center - j) / fscale) / fscale; break;
                                case TFilterTypeResample.ftrBell: weight = DibFusionFilters.BellFilter((Center - j) / fscale) / fscale; break;
                                case TFilterTypeResample.ftrBSpline: weight = DibFusionFilters.SplineFilter((Center - j) / fscale) / fscale; break;
                                case TFilterTypeResample.ftrLanczos3: weight = DibFusionFilters.Lanczos3Filter((Center - j) / fscale) / fscale; break;
                                case TFilterTypeResample.ftrMitchell: weight = DibFusionFilters.MitchellFilter((Center - j) / fscale) / fscale; break;
                                default: weight = 0.0f; break;
                            }
                            if (weight == 0.0f)
                                continue;
                            if (j < 0)
                                n = -j;
                            else if (j >= SrcHeight)
                                n = SrcHeight - j + SrcHeight - 1;
                            else
                                n = j;
                            k = contrib[I].n;
                            contrib[I].n = contrib[I].n + 1;
                            contrib[I].P[k].pixel = n;
                            contrib[I].P[k].weight = weight;
                        }
                    }
                }
                else
                // Vertical super-sampling
                // Scales from smaller to bigger height
                {
                    for (I = 0; I <= DstHeight - 1; I++)
                    {
                        contrib[I].n = 0;
                        contrib[I].P = AllocContributor(DibFusionSupport.Trunc(FWidth * 2.0 + 1));
                        Center = I / yscale;
                        // Original code:
                        // left := ceil(center - fwidth);
                        // right := floor(center + fwidth);
                        Left = (int)Math.Floor(Center - FWidth);
                        Right = (int)Math.Ceiling(Center + FWidth);
                        for (j = Left; j <= Right; j++)
                        {
                            switch (filtertype)
                            {
                                case TFilterTypeResample.ftrBox: weight = DibFusionFilters.BoxFilter(Center - j); break;
                                case TFilterTypeResample.ftrTriangle: weight = DibFusionFilters.TriangleFilter(Center - j); break;
                                case TFilterTypeResample.ftrHermite: weight = DibFusionFilters.HermiteFilter(Center - j); break;
                                case TFilterTypeResample.ftrBell: weight = DibFusionFilters.BellFilter(Center - j); break;
                                case TFilterTypeResample.ftrBSpline: weight = DibFusionFilters.SplineFilter(Center - j); break;
                                case TFilterTypeResample.ftrLanczos3: weight = DibFusionFilters.Lanczos3Filter(Center - j); break;
                                case TFilterTypeResample.ftrMitchell: weight = DibFusionFilters.MitchellFilter(Center - j); break;
                                default: weight = 0.0f; break;
                            }
                            if (weight == 0.0f)
                                continue;
                            if (j < 0)
                                n = -j;
                            else if (j >= SrcHeight)
                                n = SrcHeight - j + SrcHeight - 1;
                            else
                                n = j;
                            k = contrib[I].n;
                            contrib[I].n = contrib[I].n + 1;
                            contrib[I].P[k].pixel = n;
                            contrib[I].P[k].weight = weight;
                        }
                    }
                }

                // --------------------------------------------------
                // Apply filter to sample vertically from Work to Dst
                // --------------------------------------------------
                unsafe
                {
                    byte* SourceLine = (byte*)Work.ScanLine(0);
                    int Delta = Addr(Work.ScanLine(1)) - Addr(Work.ScanLine(0));
                    byte* DestLine = (byte*)Dst.ScanLine(0);
                    int DestDelta = Addr(Dst.ScanLine(1)) - Addr(Dst.ScanLine(0));
                    for (k = 0; k <= DstWidth - 1; k++)
                    {
                        byte* DestPixel = DestLine;
                        for (I = 0; I <= DstHeight - 1; I++)
                        {
                            rgb.R = 0;
                            rgb.G = 0;
                            rgb.B = 0;
                            // weight := 0.0;
                            for (j = 0; j <= contrib[I].n - 1; j++)
                            {
                                byte* src = SourceLine + contrib[I].P[j].pixel * Delta;
                                Color.R = src[0];
                                Color.G = src[1];
                                Color.B = src[2];
                                weight = contrib[I].P[j].weight;
                                if (weight == 0.0f)
                                    continue;
                                rgb.R = rgb.R + Color.R * weight;
                                rgb.G = rgb.G + Color.G * weight;
                                rgb.B = rgb.B + Color.B * weight;
                            }
                            if (rgb.R > 255.0f)
                                Color.R = 255;
                            else if (rgb.R < 0.0f)
                                Color.R = 0;
                            else
                                Color.R = unchecked((byte)DibFusionSupport.Round(rgb.R));
                            if (rgb.G > 255.0f)
                                Color.G = 255;
                            else if (rgb.G < 0.0f)
                                Color.G = 0;
                            else
                                Color.G = unchecked((byte)DibFusionSupport.Round(rgb.G));
                            if (rgb.B > 255.0f)
                                Color.B = 255;
                            else if (rgb.B < 0.0f)
                                Color.B = 0;
                            else
                                Color.B = unchecked((byte)DibFusionSupport.Round(rgb.B));
                            DestPixel[0] = Color.R;
                            DestPixel[1] = Color.G;
                            DestPixel[2] = Color.B;
                            DestPixel += DestDelta; // Inc(Integer(DestPixel), DestDelta)
                        }
                        SourceLine += 3; // Inc(SourceLine, 1) —— 指针是 TColorRGB*，+1 = +3 字节
                        DestLine += 3;   // Inc(DestLine, 1)
                    }
                }

                // Free the memory allocated for vertical filter weights
                for (I = 0; I <= DstHeight - 1; I++)
                    contrib[I].P = null;

                contrib = null;
            }
            finally
            {
                Work.Destroy();
            }
        }

        var BB1 = new TDIB();
        BB1.SetBitCount(24);
        BB1.Assign(this);
        var BB2 = new TDIB();
        BB2.SetSize(AmountX, AmountY, 24);
        // BB2.Assign (BB1);   原文如此（DIB.pas:7669）—— 被注释掉
        Resample(BB1, BB2, TypeResample, DIBConstants.DefaultFilterRadius[(int)TypeResample]);
        Assign(BB2);
        BB1.Destroy();
        BB2.Destroy();
    }

    /// <summary>原文 `GetMem(contrib, Count * SizeOf(TCList))` 的托管等价（Count &lt;= 0 时为空数组）。</summary>
    private static DibFusionFilters.DibCList[] AllocCList(int Count)
        => Count > 0 ? new DibFusionFilters.DibCList[Count] : Array.Empty<DibFusionFilters.DibCList>();

    /// <summary>原文 `GetMem(contrib^[I].P, N * SizeOf(TContributor))` 的托管等价（N &lt;= 0 时为空数组）。</summary>
    private static DibFusionFilters.DibContributor[] AllocContributor(int N)
        => N > 0 ? new DibFusionFilters.DibContributor[N] : Array.Empty<DibFusionFilters.DibContributor>();
}
