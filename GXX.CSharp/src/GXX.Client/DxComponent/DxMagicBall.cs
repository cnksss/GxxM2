using System;
using System.Collections.Generic;
// -------------------------------------------------------------------------------------
// DxComponents.pas 46-47 的 TMagicBallType / TMagicBallValueAlignment：
// **不在本文件重新声明**，改为引用解决方案内已有的唯一一份定义。
//
// 原因（跨车道重名事故，第 5 次）：`GXX.Client.LoadDx.GuiRecords.g.cs:92-106` 已从
// `DxComponents.pas:46-47` 生成过这两个枚举（`: byte`，成员名与顺序逐字一致），
// 而 `tests/GXX.Client.Tests/LoadDxRecordLayoutTests.cs` / `LoadDxControlLoaderTests.cs`
// 同时 `using GXX.Client.DxComponent;` 与 `using GXX.Client.LoadDx;` ——
// 若在 `GXX.Client.DxComponent` 再声明同名类型，这两个文件立刻报 6 处 CS0104
// （与台账 §12.8 记录的 TDxControlRef / TAlignEx / TDxImageButton 同型）。
// `GXX.Client/LoadDx/**` 对本车道是**只读**的调用方，无法在那边去重，
// 故此处以 using 别名引用既有定义（成员名/顺序与原文 1:1）。
//
// 待办（已上报调度方）：按 §12.8「正式归属」裁定，`DxComponents.pas` 的正式归属应是
// `GXX.Client.DxComponent`（DxMagicBall.cs 即其 1:1 移植单元之一）。授权 LoadDx 侧
// 删除 `GuiRecords.g.cs` 的重复枚举（需同步改生成器）后，本别名即可就地删除。
// -------------------------------------------------------------------------------------
using TMagicBallType = GXX.Client.LoadDx.TMagicBallType;
using TMagicBallValueAlignment = GXX.Client.LoadDx.TMagicBallValueAlignment;

namespace GXX.Client.DxComponent;

// =====================================================================================
// DxMagicBall.pas（808 行）1:1 移植。
//
//   * 22-23     TDxMagicBall 前置声明
//   * 26-93     TMagicBallOverallSetting（红蓝一体设置：Empty/Full HPMP、Empty/Full HP、
//               Splite、MiddleZoneWidth、OnlyViewHP + 特效字段）
//   * 96-144    TMagicBallAloneSetting（独立设置：Empty/Full + 特效字段）
//   * 146-178   TDxMagicBall 声明
//   * 180-808   实现段
//
// DFM: TDxMagicBall / TMagicBallOverallSetting / TMagicBallAloneSetting 无同名 .dfm
//      （Source\Client-HGE 全树 glob 核实，0 命中），设计期取值落在各 .GUI 文件的 Tgui* 记录里。
//
// -------------------------------------------------------------------------------------
// 托管侧落点与偏差（逐条登记）：
//
//   1. `TMagicBallType` / `TMagicBallValueAlignment`（DxComponents.pas 46-47）在解决方案内
//      **已有一份**定义（`LoadDx/GuiRecords.g.cs:92-106`，`: byte`，成员名与顺序逐字一致），
//      故本文件**不重复声明**，改用 using 别名引用（详见文件头 using 段与下方接缝说明）；
//      `TGetHumAbilityEvent` / `TAfterDrawMagicBallAreaEvent`（DxComponents.pas 63-64）
//      在解决方案内无既有定义，故在本文件补顶层声明（名称与原文一致）。
//
//   2. `GameCanvas.Draw(x, y, d)`（3 参整图）/ `Draw(x, y, PaintRect, d)`（4 参裁剪）/
//      `DrawBlend(x, y, PaintRect, d)` → 上游接缝 `IDxSurfacePainter` 的
//      `Draw(x, y, IDxTexture, int)`（走 `blendMode = 2`，与 DxImageButton 的
//      "非 Blend 时字面量 2" 同源）与 `Draw(x, y, TDxRect, IDxTexture)`。
//      `DrawBlend` 走 `DxPainterExt.DrawBlend`（`IDxSurfacePainterExt`）。
//      原文 3 参 `Draw` 无 BlendMode 形参，故托管侧传 2。
//
//   3. `TGameImages.Images[i]` → `IDxImageLibrary.GetImage(i)`。
//
//   4. `Round(Float)`（Delphi）= 银行家舍入（**就近偶数**），而 C# `Math.Round(double)`
//      默认 `MidpointRounding.ToEven` —— 二者一致，故直接用 `Math.Round`。
//      但 **`d.Width / dwMaxHP` 是 Integer/Integer 的浮点除法**（`/` 在 Delphi 里返回 Real），
//      故必须写成 `(double)d.Width / dwMaxHP` 才等价；写作整数除法会得到 0/1 的严重偏差。
//
//   5. `MyGetTickCount` → `DxTickCount.MyGetTickCount`（可注入，便于特效帧推进单测）。
//
//   6. `FOnStopPaint`（158/171/792-793）在上游接缝的 TDxControl 上**未暴露**（接缝已有
//      `DxControlHooks.GetOnStopPaint`，但原文这里用的是 TDxMagicBall **自己的**同名属性），
//      故本类自带 `OnStopPaint` 字段并在 Paint 末尾回调。
//
//   7. 原文 495-507 的大段注释（被注释掉的"分隔条前置绘制"）与 517-520 / 621-635 的做法
//      （分隔条移到最后）逐字保留结构。
//
//   8. 原文 **576/578 行**的怪癖：MP 特效段在 `mbaRight/mbaBottom` 系分支里用的是
//      `(Height - d.Width) div 2`（**把 Width 当 Height 用**），而同段的非特效段（604 行）
//      用的是 `(Height - d.Height) div 2`。原文如此，逐字保留并加注释。
//
//   9. 原文 **475 / 597 行**的怪癖：HPMP/MP 组合段的 `mbaRight` 用
//      `nWidth / dwMaxMP * MAX(dwMaxHP - dwMP, 0)`（**减的是 dwMaxHP 而不是 dwMaxMP**），
//      与非组合段的 730 行（减 dwMaxMP）不一致。原文如此，逐字保留并加注释。
//
//  10. 托管侧接缝：原文 `Paint`（358-794）没有"可见区为空就返回"的守卫（由
//      `GameCanvas` 负责裁剪）。托管侧为 headless 可测且与同车道 `DxImageButton.PaintImageButton`
//      （同文件 934-935）保持一致，在 `PaintMagicBall` 开头加了
//      `VisibleRect` 退化守卫（`Bottom <= Top || Right <= Left` → return）。
//      该守卫**不会**改变 `dwMaxHP <= 0` 早退之后的语义，也不会触碰绘制序列；
//      但它确实是一处原文没有的前置判断，登记在案。
//
//  11. 原文 803-806 `TDXMagicBall.DoOnInRealArea` 只 `inherited;`（空实现），
//      上游接缝 TDxControl 的对应钩子未暴露，故托管侧不落该重写（语义等价：无副作用）。
// =====================================================================================

// -------------------------------------------------------------------------------------
// DxComponents.pas 46-47 / 63-64：本单元依赖的枚举与委托（属未移植单元的顶层类型）
//
// `TMagicBallType` / `TMagicBallValueAlignment` 见文件头 using 别名（引用 LoadDx 侧的
// 唯一一份定义，避免跨车道同名重复）；`TGetHumAbilityEvent` / `TAfterDrawMagicBallAreaEvent`
// 在解决方案内**无**既有定义，故在下文补顶层声明（成员名与原文一致）。
// -------------------------------------------------------------------------------------

/// <summary>
/// DxComponents.pas 63 `TGetHumAbilityEvent = procedure(Sender; var Job:Byte; var Level,HP,MaxHP,MP,MaxMP:LongWord) of object`。
/// 托管侧以出参引用类型承接（Delphi 的 `var` 语义）。
/// </summary>
public delegate void TGetHumAbilityEvent(object sender, ByteRef job, UInt32Ref level, UInt32Ref hp,
    UInt32Ref maxHp, UInt32Ref mp, UInt32Ref maxMp);

/// <summary>
/// DxComponents.pas 64 `TAfterDrawMagicBallAreaEvent = procedure(Sender; IsHP:Boolean; Rect:TRect) of object`。
/// </summary>
public delegate void TAfterDrawMagicBallAreaEvent(object sender, bool isHp, TDxRect rect);

/// <summary>Delphi `var X:Byte` 出参。</summary>
public sealed class ByteRef
{
    public byte Value;
    public ByteRef(byte value) { Value = value; }
}

/// <summary>Delphi `var X:LongWord` 出参。</summary>
public sealed class UInt32Ref
{
    public uint Value;
    public UInt32Ref(uint value) { Value = value; }
}

// -------------------------------------------------------------------------------------
// DxMagicBall.pas 26-93：TMagicBallOverallSetting
// -------------------------------------------------------------------------------------

/// <summary>
/// DxMagicBall.pas 26-93 / 182-261 <c>TMagicBallOverallSetting</c>（红蓝一体设置）。
/// 原文 `TMagicBallOverallSetting = class(TPersistent)`；托管侧普通类 + `Assign`。
/// </summary>
public class TMagicBallOverallSetting
{
    /// <summary>原文 28 FOwner（TDxMagicBall）。</summary>
    public TDXMagicBall Owner;

    /// <summary>原文 30 FOnChange。</summary>
    public Action<TMagicBallOverallSetting> OnChange;

    /// <summary>原文 31 FOnGetImage（TOnGetImage）。</summary>
    public Action<TMagicBallOverallSetting, TImageType, object> OnGetImage;

    /// <summary>原文 34 FImage（TGameImages）→ 接缝 IDxImageLibrary。</summary>
    public IDxImageLibrary Image;

    /// <summary>原文 52 FEffectImage。</summary>
    public IDxImageLibrary EffectImage;

    /// <summary>原文 36/187 FEmptyHPMP（构造 := -1）。</summary>
    public int EmptyHPMP = -1;

    /// <summary>原文 37/188 FFullHPMP（构造 := -1）。</summary>
    public int FullHPMP = -1;

    /// <summary>原文 39/189 FEmptyHP（构造 := -1）。</summary>
    public int EmptyHP = -1;

    /// <summary>原文 40/190 FFullHP（构造 := -1）。</summary>
    public int FullHP = -1;

    /// <summary>原文 42/191 FSplite（构造 := -1；分隔条图片序号）。</summary>
    public int Splite = -1;

    /// <summary>原文 43/193 FMiddleZoneWidth（构造 := 0；中间可绘制区宽度）。</summary>
    public int MiddleZoneWidth;

    /// <summary>原文 44/194 FOnlyViewHP（构造 := False）。</summary>
    public bool OnlyViewHP;

    /// <summary>原文 47/196 FEffectCurrFrame（构造 := 0）。</summary>
    public int EffectCurrFrame;

    /// <summary>原文 48/197 FEffectLastTick（构造 := MyGetTickCount）。</summary>
    public uint EffectLastTick = DxTickCount.MyGetTickCount();

    /// <summary>原文 50/198 FEffectDrawBlend（构造 := False）。</summary>
    public bool EffectDrawBlend;

    /// <summary>原文 51/199 FEffectImageType（构造 := Prguse_wil）。</summary>
    public TImageType EffectImageType2 = TImageType.Prguse_wil;

    /// <summary>原文 53/200 FEffectHPMPStart（构造 := -1）。</summary>
    public int EffectHPMPStart = -1;

    /// <summary>原文 55/201 FEffectHPStart（构造 := -1）。</summary>
    public int EffectHPStart = -1;

    /// <summary>原文 57/202 FEffectImageCount（构造 := 0）。</summary>
    public int EffectImageCount;

    /// <summary>原文 58/203 FEffectPlayInterval（构造 := 200）。</summary>
    public int EffectPlayInterval = 200;

    private TImageType _imageType = TImageType.Prguse_wil;   // 原文 33/186 FImageType

    /// <summary>原文 75 `property ImageType` → `SetImageType`（值变化才回调 + Changed）。</summary>
    public TImageType ImageType
    {
        get => _imageType;
        set
        {
            if (_imageType == value) return;
            _imageType = value;
            if (OnGetImage != null) OnGetImage(this, _imageType, Image);
            Changed();
        }
    }

    /// <summary>原文 87 `property EffectImageType` → `SetEffectImageType`。</summary>
    public TImageType EffectImageType
    {
        get => EffectImageType2;
        set
        {
            if (EffectImageType2 == value) return;
            EffectImageType2 = value;
            if (OnGetImage != null) OnGetImage(this, EffectImageType2, EffectImage);
            Changed();
        }
    }

    /// <summary>原文 239-243 Changed。</summary>
    public void Changed() => OnChange?.Invoke(this);

    /// <summary>
    /// 原文 229-237 `SetOnGetImage`：赋值后**立即回调两次**（先主图库、再特效图库），再 Changed。
    /// </summary>
    public void SetOnGetImage(Action<TMagicBallOverallSetting, TImageType, object> value)
    {
        OnGetImage = value;
        if (OnGetImage != null)
        {
            OnGetImage(this, _imageType, Image);
            OnGetImage(this, EffectImageType2, EffectImage);
        }
        Changed();
    }

    /// <summary>
    /// 原文 245-261 `Assign`：走 `OnGetImage := Source.OnGetImage`（**注意是属性赋值**，
    /// 会触发 SetOnGetImage 的两次回调），随后逐字段直写 `FImageType`/`FEmptyHPMP`… /`FOnlyViewHP`，
    /// 最后 `Changed`。
    /// </summary>
    public void Assign(TMagicBallOverallSetting source)
    {
        if (source == null) return;

        SetOnGetImage(source.OnGetImage);
        _imageType = source._imageType;

        EmptyHPMP = source.EmptyHPMP;
        FullHPMP = source.FullHPMP;
        EmptyHP = source.EmptyHP;
        FullHP = source.FullHP;
        Splite = source.Splite;
        MiddleZoneWidth = source.MiddleZoneWidth;
        OnlyViewHP = source.OnlyViewHP;

        Changed();
    }
}

// -------------------------------------------------------------------------------------
// DxMagicBall.pas 96-144：TMagicBallAloneSetting
// -------------------------------------------------------------------------------------

/// <summary>DxMagicBall.pas 96-144 / 263-333 <c>TMagicBallAloneSetting</c>（独立设置）。</summary>
public class TMagicBallAloneSetting
{
    /// <summary>原文 98 FOwner。</summary>
    public TDXMagicBall Owner;

    /// <summary>原文 100 FOnChange。</summary>
    public Action<TMagicBallAloneSetting> OnChange;

    /// <summary>原文 101 FOnGetImage。</summary>
    public Action<TMagicBallAloneSetting, TImageType, object> OnGetImage;

    /// <summary>原文 104 FImage。</summary>
    public IDxImageLibrary Image;

    /// <summary>原文 114 FEffectImage。</summary>
    public IDxImageLibrary EffectImage;

    /// <summary>原文 106/270 FEmpty（构造 := -1）。</summary>
    public int Empty = -1;

    /// <summary>原文 107/271 FFull（构造 := -1）。</summary>
    public int Full = -1;

    /// <summary>原文 109/273 FEffectCurrFrame（构造 := 0）。</summary>
    public int EffectCurrFrame;

    /// <summary>原文 110/274 FEffectLastTick（构造 := MyGetTickCount）。</summary>
    public uint EffectLastTick = DxTickCount.MyGetTickCount();

    /// <summary>原文 112/275 FEffectDrawBlend（构造 := False）。</summary>
    public bool EffectDrawBlend;

    /// <summary>原文 113/276 FEffectImageType（构造 := Prguse_wil）。</summary>
    public TImageType EffectImageType2 = TImageType.Prguse_wil;

    /// <summary>原文 115/277 FEffectStart（构造 := -1）。</summary>
    public int EffectStart = -1;

    /// <summary>原文 116/278 FEffectImageCount（构造 := 0）。</summary>
    public int EffectImageCount;

    /// <summary>原文 117/279 FEffectPlayInterval（构造 := 200）。</summary>
    public int EffectPlayInterval = 200;

    private TImageType _imageType = TImageType.Prguse_wil;   // 原文 103/267 FImageType

    /// <summary>原文 134 `property ImageType` → `SetImageType`。</summary>
    public TImageType ImageType
    {
        get => _imageType;
        set
        {
            if (_imageType == value) return;
            _imageType = value;
            if (OnGetImage != null) OnGetImage(this, _imageType, Image);
            Changed();
        }
    }

    /// <summary>原文 139 `property EffectImageType` → `SetEffectImageType`。</summary>
    public TImageType EffectImageType
    {
        get => EffectImageType2;
        set
        {
            if (EffectImageType2 == value) return;
            EffectImageType2 = value;
            if (OnGetImage != null) OnGetImage(this, EffectImageType2, EffectImage);
            Changed();
        }
    }

    /// <summary>原文 316-320 Changed。</summary>
    public void Changed() => OnChange?.Invoke(this);

    /// <summary>原文 306-314 `SetOnGetImage`：赋值后立即回调两次（主图库 + 特效图库），再 Changed。</summary>
    public void SetOnGetImage(Action<TMagicBallAloneSetting, TImageType, object> value)
    {
        OnGetImage = value;
        if (OnGetImage != null)
        {
            OnGetImage(this, _imageType, Image);
            OnGetImage(this, EffectImageType2, EffectImage);
        }
        Changed();
    }

    /// <summary>
    /// 原文 322-333 `Assign`：与 Overall 版同形，但只搬 `FEmpty`/`FFull` 两个业务字段
    /// （**特效字段不在 Assign 里**，逐字保留）。
    /// </summary>
    public void Assign(TMagicBallAloneSetting source)
    {
        if (source == null) return;

        SetOnGetImage(source.OnGetImage);
        _imageType = source._imageType;

        Empty = source.Empty;
        Full = source.Full;

        Changed();
    }
}

// -------------------------------------------------------------------------------------
// DxMagicBall.pas 146-178 / 335-806：TDXMagicBall
// -------------------------------------------------------------------------------------

/// <summary>
/// DxMagicBall.pas 146-178 / 335-806 <c>TDXMagicBall</c>（魔法球控件）。
///
/// 纯逻辑（可单测）全部落地：
///   * `ComputeFillRect`（值 → 像素映射，四种对齐，含两处原文怪癖）
///   * `ComputeSplitZone`（HPMP 组合的中间分隔区几何）
///   * `AdvanceEffectFrame`（特效帧推进 + 回绕）
///   * 三种 `BallType` 的绘制计划（顺序、落点、回调）
/// 绘制走接缝 `IDxSurfacePainter` / `DxPainterExt`。
/// </summary>
public class TDXMagicBall : TDxControl
{
    /// <summary>原文 148 FBallType（构造 := mbtHPMP）。</summary>
    public TMagicBallType BallType = TMagicBallType.mbtHPMP;

    /// <summary>原文 149 FValueAlignment（构造 := mbaBottom）。</summary>
    public TMagicBallValueAlignment ValueAlignment = TMagicBallValueAlignment.mbaBottom;

    /// <summary>原文 151/341 FOverallSetting。</summary>
    public TMagicBallOverallSetting OverallSetting;

    /// <summary>原文 152/342 FAloneSetting。</summary>
    public TMagicBallAloneSetting AloneSetting;

    /// <summary>原文 154/168 OnGetHumAbility。</summary>
    public TGetHumAbilityEvent OnGetHumAbility;

    /// <summary>原文 155/169 OnAfterDrawMagicBallArea。</summary>
    public TAfterDrawMagicBallAreaEvent OnAfterDrawMagicBallArea;

    /// <summary>原文 156/170 OnAfterDrawMagicBallEffectArea。</summary>
    public TAfterDrawMagicBallAreaEvent OnAfterDrawMagicBallEffectArea;

    /// <summary>
    /// 原文 158/171 `property OnStopPaint:TNotifyEvent` —— 这是 TDxMagicBall **自己的**属性，
    /// 与上游接缝 `DxControlHooks.OnStopPaint` 槽位是两套（原文 792-793 用的是本属性）。
    /// </summary>
    public Action<TDXMagicBall> OnStopPaint;

    /// <summary>原文 335-349 TDXMagicBall.Create 逐项。</summary>
    public TDXMagicBall()
    {
        BallType = TMagicBallType.mbtHPMP;
        ValueAlignment = TMagicBallValueAlignment.mbaBottom;

        OverallSetting = new TMagicBallOverallSetting { Owner = this };
        AloneSetting = new TMagicBallAloneSetting { Owner = this };

        // 原文 344-345：两个设置把 OnGetImage 指向控件自身的 OnGetImage 属性。
        // 原文 `OnGetImage` 是 `function GetOnGetImage():TOnGetImage` + `procedure SetOnGetImage`，
        // 此处用控件自己的转发方法承接（见 SetOnGetImageV2）。
        OverallSetting.OnGetImage = (s, t, o) => RaiseOnGetImage(t);
        AloneSetting.OnGetImage = (s, t, o) => RaiseOnGetImage(t);

        Width = 90;
        Height = 90;
    }

    /// <summary>原文 351-356 Destroy。</summary>
    public void DisposeMagicBall()
    {
        OverallSetting = null;
        AloneSetting = null;
    }

    /// <summary>
    /// 原文 796-801 `TDXMagicBall.SetOnGetImage`：先 `inherited`，再把同一个值转挂给
    /// `FOverallSetting.OnGetImage` 与 `FAloneSetting.OnGetImage`。
    /// （上游 TDxControl 的 `SetOnGetImage(Action&lt;TDxImageIndex,TImageType&gt;)` 签名不同，故此处
    /// 以 `SetOnGetImageV2` 承接；转挂语义一致。）
    /// </summary>
    public void SetOnGetImageV2(Action<TDxImageIndex, TImageType> value)
    {
        DxControlHooks.SetOnGetImage(this, value);
        if (OverallSetting != null) OverallSetting.SetOnGetImage((s, t, o) => RaiseOnGetImage(t));
        if (AloneSetting != null) AloneSetting.SetOnGetImage((s, t, o) => RaiseOnGetImage(t));
    }

    private void RaiseOnGetImage(TImageType t) => DxControlHooks.GetOnGetImage(this)?.Invoke(ImageIndex, t);

    // ===============================================================================
    // 纯几何：值 → 像素映射（原文 406-412 / 448-454 / 651-657 / 728-734 等 6 处同形）
    // ===============================================================================

    /// <summary>
    /// DxMagicBall.pas 406-412（及 448-454 / 528-534 / 651-657 / 689-695 / 728-734 / 766-772）
    /// 的**值 → 像素映射**，1:1（原文共 7 处同形，此处收拢为一个可单测静态方法）。
    ///
    /// `srcRect` 是纹理的 `ClientRect`；`fullW`/`fullH` 是**该次映射的基准宽高**
    /// （原文 4 种情形各自传入 `d.Width`/`d.Height`，或 HPMP 组合下减半后的 `nWidth`/`nHeight`）。
    ///
    /// 四个分支（**原文的 `else` 即 mbaBottom**）：
    ///   * `mbaLeft`  ：`Right  = Max(Round(fullW / maxBase * value), 0)`
    ///   * `mbaRight` ：`Left   = Max(Round(fullW / maxBase * Max(maxBase - value, 0)), 0)`
    ///   * `mbaTop`   ：`Bottom = Max(Round(fullH / maxBase * value), 0)`
    ///   * `mbaBottom`：`Top    = Max(Round(fullH / maxBase * Max(maxBase - value, 0)), 0)`
    ///
    /// 注意：**除法必须用 double**（Delphi `/` 返回 Real）；`Round` 是银行家舍入
    /// （C# `Math.Round(double)` 默认 `MidpointRounding.ToEven`，一致）。
    /// </summary>
    public static TDxRect ComputeFillRect(TMagicBallValueAlignment alignment, TDxRect srcRect,
        int fullW, int fullH, uint maxBase, uint value)
    {
        var r = srcRect;
        switch (alignment)
        {
            case TMagicBallValueAlignment.mbaLeft:
                r.Right = Math.Max((int)Math.Round((double)fullW / maxBase * value), 0);
                break;
            case TMagicBallValueAlignment.mbaRight:
                r.Left = Math.Max((int)Math.Round((double)fullW / maxBase * Math.Max((long)maxBase - value, 0)), 0);
                break;
            case TMagicBallValueAlignment.mbaTop:
                r.Bottom = Math.Max((int)Math.Round((double)fullH / maxBase * value), 0);
                break;
            default:   // 原文 `else //mbaBottom:`
                r.Top = Math.Max((int)Math.Round((double)fullH / maxBase * Math.Max((long)maxBase - value, 0)), 0);
                break;
        }
        return r;
    }

    /// <summary>
    /// DxMagicBall.pas 437-446（及 556-565）的**HPMP 组合中间分隔区几何**，1:1。
    ///
    /// 原文（左/右对齐）：
    ///   `nWidth := d.Width;  nHeight := d.Height div 2;`
    ///   `PaintRect.Bottom := nHeight - MiddleZoneWidth div 2;`
    /// 原文（上/下对齐）：
    ///   `nWidth := d.Width div 2;  nHeight := d.Height;`
    ///   `PaintRect.Right := nWidth - MiddleZoneWidth div 2;`
    ///
    /// 注意 `div 2` 是**整数除法**（与上面映射里的 `/` 浮点除法不同），且 `MiddleZoneWidth div 2`
    /// 先算再减。返回值为该次映射使用的 `(nWidth, nHeight)`。
    /// </summary>
    public static TDxPoint ComputeSplitZone(TMagicBallValueAlignment alignment, TDxRect srcRect, int texW,
        int texH, int middleZoneWidth, out TDxRect halfRect)
    {
        halfRect = srcRect;
        int nWidth;
        int nHeight;

        if (alignment == TMagicBallValueAlignment.mbaLeft || alignment == TMagicBallValueAlignment.mbaRight)
        {
            nWidth = texW;
            nHeight = texH / 2;
            halfRect.Bottom = nHeight - middleZoneWidth / 2;
        }
        else
        {
            nWidth = texW / 2;
            nHeight = texH;
            halfRect.Right = nWidth - middleZoneWidth / 2;
        }
        return new TDxPoint(nWidth, nHeight);
    }

    /// <summary>
    /// DxMagicBall.pas 467-471（及 589-593）的**第二半（MP 侧）起点**，1:1：
    /// 左/右对齐时 `PaintRect.Top := nHeight + MiddleZoneWidth div 2`；
    /// 上/下对齐时 `PaintRect.Left := nWidth + MiddleZoneWidth div 2`。
    /// （原文先 `PaintRect := d.ClientRect` 再改这一个边。）
    /// </summary>
    public static TDxRect ComputeSecondHalfRect(TMagicBallValueAlignment alignment, TDxRect srcRect,
        int nWidth, int nHeight, int middleZoneWidth)
    {
        var r = srcRect;
        if (alignment == TMagicBallValueAlignment.mbaLeft || alignment == TMagicBallValueAlignment.mbaRight)
            r.Top = nHeight + middleZoneWidth / 2;
        else
            r.Left = nWidth + middleZoneWidth / 2;
        return r;
    }

    /// <summary>
    /// DxMagicBall.pas 474-479（及 596-601）的**第二半（MP 侧）右/左边界叠加**，1:1：
    ///   * `mbaLeft`  ：`Right  = PaintRect.Left + Max(Round(nWidth / dwMaxMP * dwMP), 0)`
    ///   * `mbaRight` ：`Left   = PaintRect.Left + Max(Round(nWidth / dwMaxMP * Max(dwMaxHP - dwMP, 0)), 0)`
    ///                  —— **原文 475/597 减的是 `dwMaxHP` 而不是 `dwMaxMP`**（非组合段的 730 行减 dwMaxMP），
    ///                  原文如此，逐字保留。
    ///   * `mbaTop`   ：`Bottom = Max(Round(nHeight / dwMaxMP * dwMP), 0)`
    ///   * `mbaBottom`：`Top    = Max(Round(nHeight / dwMaxMP * Max(dwMaxMP - dwMP, 0)), 0)`
    /// </summary>
    public static TDxRect ComputeSecondHalfFill(TMagicBallValueAlignment alignment, TDxRect halfRect,
        int nWidth, int nHeight, uint maxHp, uint maxMp, uint mp, bool useHpBaseForRight)
    {
        var r = halfRect;
        switch (alignment)
        {
            case TMagicBallValueAlignment.mbaLeft:
                r.Right = r.Left + Math.Max((int)Math.Round((double)nWidth / maxMp * mp), 0);
                break;
            case TMagicBallValueAlignment.mbaRight:
                {
                    // 原文 475/597：`MAX(dwMaxHP - dwMP, 0)`（useHpBaseForRight = true）
                    // 原文 730   ：`MAX(dwMaxMP - dwMP, 0)`（useHpBaseForRight = false）
                    uint baseSub = useHpBaseForRight ? maxHp : maxMp;
                    r.Left = r.Left + Math.Max(
                        (int)Math.Round((double)nWidth / maxMp * Math.Max((long)baseSub - mp, 0)), 0);
                    break;
                }
            case TMagicBallValueAlignment.mbaTop:
                r.Bottom = Math.Max((int)Math.Round((double)nHeight / maxMp * mp), 0);
                break;
            default:
                r.Top = Math.Max((int)Math.Round((double)nHeight / maxMp * Math.Max((long)maxMp - mp, 0)), 0);
                break;
        }
        return r;
    }

    // ===============================================================================
    // 纯逻辑：特效帧推进（原文 513-520 / 675-682 / 752-759，三处同形）
    // ===============================================================================

    /// <summary>
    /// DxMagicBall.pas 513-520 的**特效帧推进**，1:1（Overall 版；Alone 版见
    /// <see cref="AdvanceEffectFrameAlone"/>）：
    ///   ① 距上次 `EffectLastTick` `&gt;= EffectPlayInterval` 才推进：
    ///      刷新 `EffectLastTick := MyGetTickCount`，`Inc(EffectCurrFrame)`；
    ///   ② 随后**无条件**判 `CurrFrame &lt; 0 or &gt;= EffectImageCount` → 归 0。
    /// 注意原文用**局部 `btJob`/`dwLevel`** 决定画 HP 段还是 HPMP 段，而**不是** BallType。
    /// 返回推进后的帧号。
    /// </summary>
    public static int AdvanceEffectFrame(ref uint lastTick, ref int currFrame, int imageCount, int playInterval)
    {
        if (DxTickCount.MyGetTickCount() - lastTick >= (uint)playInterval)
        {
            lastTick = DxTickCount.MyGetTickCount();
            currFrame++;
        }

        if (currFrame < 0 || currFrame >= imageCount)
        {
            currFrame = 0;
        }
        return currFrame;
    }

    /// <summary>Overall 设置的特效帧推进（原文 513-520）。</summary>
    public int AdvanceEffectFrameOverall()
        => AdvanceEffectFrame(ref OverallSetting.EffectLastTick, ref OverallSetting.EffectCurrFrame,
            OverallSetting.EffectImageCount, OverallSetting.EffectPlayInterval);

    /// <summary>Alone 设置的特效帧推进（原文 675-682 / 752-759，两处同形）。</summary>
    public int AdvanceEffectFrameAlone()
        => AdvanceEffectFrame(ref AloneSetting.EffectLastTick, ref AloneSetting.EffectCurrFrame,
            AloneSetting.EffectImageCount, AloneSetting.EffectPlayInterval);

    /// <summary>
    /// 原文 372-375 / 386 的**门控**：
    /// `OverallsSetting.OnlyViewHP` → `dwLevel := 20`，否则 `30`；
    /// 三条默认值 `dwHP=1500 / dwMaxHP=2000 / dwMP=1700 / dwMaxMP=2000`；
    /// 有 `OnGetHumAbility` 时回调覆盖；**`dwMaxHP &lt;= 0 or dwMaxMP &lt;= 0` 则整段 Paint 直接 Exit**。
    /// 返回 false 表示原文会 Exit。
    /// </summary>
    public bool QueryHumAbility(out byte job, out uint level, out uint hp, out uint maxHp, out uint mp,
        out uint maxMp)
    {
        level = OverallSetting.OnlyViewHP ? 20u : 30u;
        hp = 1500;
        maxHp = 2000;
        mp = 1700;
        maxMp = 2000;

        var jobRef = new ByteRef(0);
        var lvRef = new UInt32Ref(level);
        var hpRef = new UInt32Ref(hp);
        var maxHpRef = new UInt32Ref(maxHp);
        var mpRef = new UInt32Ref(mp);
        var maxMpRef = new UInt32Ref(maxMp);

        OnGetHumAbility?.Invoke(this, jobRef, lvRef, hpRef, maxHpRef, mpRef, maxMpRef);

        job = jobRef.Value;
        level = lvRef.Value;
        hp = hpRef.Value;
        maxHp = maxHpRef.Value;
        mp = mpRef.Value;
        maxMp = maxMpRef.Value;

        return maxHp > 0 && maxMp > 0;
    }

    /// <summary>
    /// 原文 393 / 522 / 624 的 `(btJob = 0) and (dwLevel &lt; 28)`（"战士&lt;28 时只绘血球"）。
    /// </summary>
    public static bool UseHpOnlyPath(byte job, uint level) => job == 0 && level < 28;

    // ===============================================================================
    // Paint（原文 358-794）
    // ===============================================================================

    /// <summary>
    /// 原文 358-794 `TDXMagicBall.Paint` 的**顺序骨架**（1:1）。
    /// 三种 BallType 各自一个分支，随后统一 `OnStopPaint`。
    ///
    /// 关键顺序（不可换）：
    ///   ① 算门控（`OnlyViewHP ? 20 : 30` + 4 条默认值 + 回调）；
    ///   ② `dwMaxHP &lt;= 0 or dwMaxMP &lt;= 0` → Exit；
    ///   ③ `vtRect := VirtualRect`；
    ///   ④ `mbtHPMP`：先空底(EmptyHP/EmptyHPMP) → 再满底(FullHP/FullHPMP) → 特效 → **分隔条放到最后**；
    ///   ⑤ `mbtHP`/`mbtMP`：空底 → 满底 → 特效；
    ///   ⑥ `OnStopPaint`。
    ///
    /// 原文 495-507 的"分隔条前置绘制"是被注释掉的，托管侧照抄留档。
    /// </summary>
    public void PaintMagicBall()
    {
        var vbRect = VisibleRect;
        if (vbRect.Bottom <= vbRect.Top || vbRect.Right <= vbRect.Left) return;

        if (!QueryHumAbility(out byte job, out uint level, out uint hp, out uint maxHp, out uint mp, out uint maxMp))
            return;

        var vtRect = VirtualRect;

        if (BallType == TMagicBallType.mbtHPMP)
        {
            PaintOverall(vtRect, job, level, hp, maxHp, mp, maxMp);
        }
        else if (BallType == TMagicBallType.mbtHP)
        {
            PaintAlone(vtRect, hp, maxHp, isHp: true);
        }
        else if (BallType == TMagicBallType.mbtMP)
        {
            PaintAlone(vtRect, mp, maxMp, isHp: false);
        }

        OnStopPaint?.Invoke(this);
    }

    /// <summary>原文 390-635：`mbtHPMP` 分支。</summary>
    private void PaintOverall(TDxRect vtRect, byte job, uint level, uint hp, uint maxHp, uint mp, uint maxMp)
    {
        var gameImage = OverallSetting.Image;
        bool hpOnly = UseHpOnlyPath(job, level);

        if (gameImage != null)
        {
            if (hpOnly)
            {
                // ---- 战士 < 28：只绘血球（原文 393-425）----
                if (OverallSetting.EmptyHP >= 0)
                {
                    var d0 = gameImage.GetImage(OverallSetting.EmptyHP);
                    if (d0 != null)
                        DrawWhole(vtRect, d0);
                }

                if (OverallSetting.FullHP >= 0)
                {
                    var d = gameImage.GetImage(OverallSetting.FullHP);
                    if (d != null)
                    {
                        var paintRect = ComputeFillRect(ValueAlignment, d.ClientRect, d.Width, d.Height,
                            maxHp, hp);
                        DrawClipped(vtRect, d, paintRect);
                        RaiseAreaDrawn(true, vtRect, d, paintRect);
                    }
                }
            }
            else
            {
                // ---- 红蓝一体（原文 426-492）----
                if (OverallSetting.EmptyHPMP >= 0)
                {
                    var d0 = gameImage.GetImage(OverallSetting.EmptyHPMP);
                    if (d0 != null)
                        DrawWhole(vtRect, d0);
                }

                if (OverallSetting.FullHPMP >= 0)
                {
                    var d = gameImage.GetImage(OverallSetting.FullHPMP);
                    if (d != null)
                    {
                        // HP 半区（原文 436-454）
                        var half = ComputeSplitZone(ValueAlignment, d.ClientRect, d.Width, d.Height,
                            OverallSetting.MiddleZoneWidth, out var hpRect);
                        hpRect = ComputeFillRect(ValueAlignment, hpRect, half.X, half.Y, maxHp, hp);
                        DrawClipped(vtRect, d, hpRect);
                        RaiseAreaDrawn(true, vtRect, d, hpRect);

                        // MP 半区（原文 467-479）——注意 475 行减的是 dwMaxHP（原文怪癖）
                        var mpRect = ComputeSecondHalfRect(ValueAlignment, d.ClientRect, half.X, half.Y,
                            OverallSetting.MiddleZoneWidth);
                        mpRect = ComputeSecondHalfFill(ValueAlignment, mpRect, half.X, half.Y, maxHp, maxMp, mp,
                            useHpBaseForRight: true);
                        DrawClipped(vtRect, d, mpRect);
                        RaiseAreaDrawn(false, vtRect, d, mpRect);
                    }
                }

                // 原文 495-507（被注释掉的"分隔条前置绘制"）照抄留档：
                //   {
                //   if (FOverallSetting.FSplite >= 0) then begin
                //     d := GameImage.Images[FOverallSetting.FSplite];
                //     if d <> nil then begin
                //       PaintRect.Left := vtRect.Left + (Width - d.Width) div 2;
                //       PaintRect.Top  := vtRect.Top  + (Height - d.Height) div 2;
                //       GameCanvas.Draw(PaintRect.Left, PaintRect.Top, d);
                //     end;
                //   end;
                //   }
            }
        }

        // ---- 特效（原文 511-619）----
        var effectImage = OverallSetting.EffectImage;
        if (effectImage != null && OverallSetting.EffectImageCount > 0 && OverallSetting.EffectPlayInterval > 0)
        {
            AdvanceEffectFrameOverall();

            if (hpOnly)
            {
                if (OverallSetting.EffectHPStart >= 0)
                {
                    var d = effectImage.GetImage(OverallSetting.EffectHPStart + OverallSetting.EffectCurrFrame);
                    if (d != null)
                    {
                        var paintRect = ComputeFillRect(ValueAlignment, d.ClientRect, d.Width, d.Height,
                            maxHp, hp);
                        DrawEffect(vtRect, d, paintRect, OverallSetting.EffectDrawBlend);
                        RaiseEffectDrawn(true, vtRect, d, paintRect);
                    }
                }
            }
            else
            {
                if (OverallSetting.EffectHPMPStart >= 0)
                {
                    var d = effectImage.GetImage(OverallSetting.EffectHPMPStart + OverallSetting.EffectCurrFrame);
                    if (d != null)
                    {
                        var half = ComputeSplitZone(ValueAlignment, d.ClientRect, d.Width, d.Height,
                            OverallSetting.MiddleZoneWidth, out var hpRect);
                        hpRect = ComputeFillRect(ValueAlignment, hpRect, half.X, half.Y, maxHp, hp);
                        // 原文 576-578：落点用 `(Height - d.Width) div 2`（**把 Width 当 Height 用**）——原文怪癖
                        DrawEffectAt(vtRect, d, hpRect, OverallSetting.EffectDrawBlend, useWidthAsHeight: true);
                        RaiseEffectDrawn(true, vtRect, d, hpRect);

                        var mpRect = ComputeSecondHalfRect(ValueAlignment, d.ClientRect, half.X, half.Y,
                            OverallSetting.MiddleZoneWidth);
                        mpRect = ComputeSecondHalfFill(ValueAlignment, mpRect, half.X, half.Y, maxHp, maxMp, mp,
                            useHpBaseForRight: true);
                        // 原文 604-606：这里回到 `(Height - d.Height) div 2`
                        DrawEffect(vtRect, d, mpRect, OverallSetting.EffectDrawBlend);
                        RaiseEffectDrawn(false, vtRect, d, mpRect);
                    }
                }
            }
        }

        // ---- 分隔条放到最后（原文 621-635）----
        if (gameImage != null && !hpOnly)
        {
            if (OverallSetting.Splite >= 0)
            {
                var d = gameImage.GetImage(OverallSetting.Splite);
                if (d != null)
                {
                    Painter.Draw(vtRect.Left + (Width - d.Width) / 2, vtRect.Top + (Height - d.Height) / 2,
                        d, 2);
                }
            }
        }
    }

    /// <summary>原文 637-712（mbtHP）/ 714-789（mbtMP）：独立球分支。</summary>
    private void PaintAlone(TDxRect vtRect, uint value, uint maxBase, bool isHp)
    {
        var setting = AloneSetting;
        var gameImage = setting.Image;

        if (gameImage != null)
        {
            if (setting.Empty >= 0)
            {
                var d0 = gameImage.GetImage(setting.Empty);
                if (d0 != null)
                    DrawWhole(vtRect, d0);
            }

            if (setting.Full >= 0)
            {
                var d = gameImage.GetImage(setting.Full);
                if (d != null)
                {
                    var paintRect = ComputeFillRect(ValueAlignment, d.ClientRect, d.Width, d.Height, maxBase, value);
                    DrawClipped(vtRect, d, paintRect);
                    RaiseAreaDrawn(isHp, vtRect, d, paintRect);
                }
            }
        }

        var effectImage = setting.EffectImage;
        if (effectImage != null && setting.EffectImageCount > 0 && setting.EffectPlayInterval > 0)
        {
            AdvanceEffectFrameAlone();

            if (setting.EffectStart >= 0)
            {
                var d = effectImage.GetImage(setting.EffectStart + setting.EffectCurrFrame);
                if (d != null)
                {
                    var paintRect = ComputeFillRect(ValueAlignment, d.ClientRect, d.Width, d.Height, maxBase, value);
                    DrawEffect(vtRect, d, paintRect, setting.EffectDrawBlend);
                    RaiseEffectDrawn(isHp, vtRect, d, paintRect);
                }
            }
        }
    }

    // ---- 绘制落点 ----

    /// <summary>原文 3 参 `GameCanvas.Draw(x, y, d)`：整图居中（BlendMode 传 2，见文件头第 2 条）。</summary>
    private void DrawWhole(TDxRect vtRect, IDxTexture d)
        => Painter.Draw(vtRect.Left + (Width - d.Width) / 2, vtRect.Top + (Height - d.Height) / 2, d, 2);

    /// <summary>原文 4 参 `GameCanvas.Draw(x, y, PaintRect, d)`：裁剪绘制，落点含 PaintRect 的 Left/Top。</summary>
    private void DrawClipped(TDxRect vtRect, IDxTexture d, TDxRect paintRect)
        => Painter.Draw(vtRect.Left + (Width - d.Width) / 2 + paintRect.Left,
            vtRect.Top + (Height - d.Height) / 2 + paintRect.Top, paintRect, d);

    /// <summary>原文特效段的 Draw / DrawBlend 二选一。</summary>
    private void DrawEffect(TDxRect vtRect, IDxTexture d, TDxRect paintRect, bool blend)
    {
        int x = vtRect.Left + (Width - d.Width) / 2 + paintRect.Left;
        int y = vtRect.Top + (Height - d.Height) / 2 + paintRect.Top;
        if (blend) DxPainterExt.DrawBlend(Painter, x, y, paintRect, d, 2);
        else Painter.Draw(x, y, paintRect, d);
    }

    /// <summary>
    /// 原文 576-578 的怪癖版落点：`vtRect.Top + (Height - d.Width) div 2`（**Width 当 Height 用**）。
    /// 见文件头第 8 条。
    /// </summary>
    private void DrawEffectAt(TDxRect vtRect, IDxTexture d, TDxRect paintRect, bool blend, bool useWidthAsHeight)
    {
        int x = vtRect.Left + (Width - d.Width) / 2 + paintRect.Left;
        int y = useWidthAsHeight
            ? vtRect.Top + (Height - d.Width) / 2 + paintRect.Top
            : vtRect.Top + (Height - d.Height) / 2 + paintRect.Top;
        if (blend) DxPainterExt.DrawBlend(Painter, x, y, paintRect, d, 2);
        else Painter.Draw(x, y, paintRect, d);
    }

    private void RaiseAreaDrawn(bool isHp, TDxRect vtRect, IDxTexture d, TDxRect paintRect)
    {
        if (OnAfterDrawMagicBallArea == null) return;
        OnAfterDrawMagicBallArea(this, isHp, MakeCallbackRect(vtRect, d, paintRect));
    }

    private void RaiseEffectDrawn(bool isHp, TDxRect vtRect, IDxTexture d, TDxRect paintRect)
    {
        if (OnAfterDrawMagicBallEffectArea == null) return;
        OnAfterDrawMagicBallEffectArea(this, isHp, MakeCallbackRect(vtRect, d, paintRect));
    }

    /// <summary>
    /// 原文 417-420（等 7 处同形）的回调矩形：
    /// `Left = vtRect.Left + (Width - d.Width) div 2 + PaintRect.Left`，Right/Bottom 同理用 PaintRect 的。
    /// </summary>
    private TDxRect MakeCallbackRect(TDxRect vtRect, IDxTexture d, TDxRect paintRect)
        => TDxRect.Rect(
            vtRect.Left + (Width - d.Width) / 2 + paintRect.Left,
            vtRect.Top + (Height - d.Height) / 2 + paintRect.Top,
            vtRect.Left + (Width - d.Width) / 2 + paintRect.Right,
            vtRect.Top + (Height - d.Height) / 2 + paintRect.Bottom);
}
