using System;
using System.Collections.Generic;

namespace GXX.Client.DxComponent;

/// <summary>
/// DXConsts.pas 1:1 逐字移植（1-103）。原文是纯资源串单元：43 条 resourcestring + 3 组
/// const（SDIB 4 条、SWave 6 条、SKey 4 条、SFFB 1 条），共 58 个常量，
/// 全部按原标识符原样保留（含原文的拼写错误：suported / opend / nose / posible / Bitcount in invalid
/// / PixelFormat in invalid / 'Session %s cannot be opened' 少了引号 —— 一律照抄）。
///
/// 原文全部走 Delphi 的 resourcestring（运行期格式串），此处以常量字符串表达；
/// 带 %d/%s 的条目照抄，格式化交给调用方（DelphiFormat.Format 或 string.Format）。
/// </summary>
public static class DXConsts
{
    // ===== resourcestring 段（DXConsts.pas 6-76）=====

    /// <summary>DXConsts.pas 6</summary>
    public const string SNone = "(None)";

    /// <summary>DXConsts.pas 7</summary>
    public const string SUnknownError = "Unknown Error (%d)";

    /// <summary>DXConsts.pas 9</summary>
    public const string SDirectDraw = "DirectDraw";

    /// <summary>DXConsts.pas 10</summary>
    public const string SDirect3DRM = "Direct3D RetainedMode";

    /// <summary>DXConsts.pas 11</summary>
    public const string SDirectSound = "DirectSound";

    /// <summary>DXConsts.pas 12</summary>
    public const string SDirectSoundCapture = "DirectSoundCapture";

    /// <summary>DXConsts.pas 13</summary>
    public const string SDirectDrawClipper = "Clipper";

    /// <summary>DXConsts.pas 14</summary>
    public const string SDirectDrawPalette = "Palette";

    /// <summary>DXConsts.pas 15</summary>
    public const string SDirectDrawSurface = "Surface";

    /// <summary>DXConsts.pas 16</summary>
    public const string SDirectDrawPrimarySurface = "Primary Surface";

    /// <summary>DXConsts.pas 17</summary>
    public const string SDirectSoundBuffer = "Sound Buffer";

    /// <summary>DXConsts.pas 18</summary>
    public const string SDirectSoundPrimaryBuffer = "Primary Buffer";

    /// <summary>DXConsts.pas 19</summary>
    public const string SDirectSoundCaptureBuffer = "Sound Capture Buffer";

    /// <summary>DXConsts.pas 20</summary>
    public const string STexture = "Texture";

    /// <summary>DXConsts.pas 21</summary>
    public const string SDirectPlay = "DirectPlay";

    /// <summary>DXConsts.pas 22</summary>
    public const string SSession = "Session";

    /// <summary>DXConsts.pas 24</summary>
    public const string SNotMade = "%s not made";

    /// <summary>DXConsts.pas 25 —— 原文如此：opend（opened 的笔误）</summary>
    public const string SStreamNotOpend = "Stream not opend";

    /// <summary>DXConsts.pas 26</summary>
    public const string SWaveStreamNotSet = "WaveStream not set";

    /// <summary>DXConsts.pas 27</summary>
    public const string SCannotMade = "%s cannot be made";

    /// <summary>DXConsts.pas 28</summary>
    public const string SCannotInitialized = "%s cannot be initialized";

    /// <summary>DXConsts.pas 29</summary>
    public const string SCannotChanged = "%s cannot be changed";

    /// <summary>DXConsts.pas 30</summary>
    public const string SCannotLock = "%s cannot be locked";

    /// <summary>DXConsts.pas 31</summary>
    public const string SCannotOpened = "%s cannot be opened";

    /// <summary>DXConsts.pas 32</summary>
    public const string SDLLNotLoaded = "%s not loaded";

    /// <summary>DXConsts.pas 33 —— Delphi 里 '' 是转义的单引号，故实际串为 Image '%s' not found</summary>
    public const string SImageNotFound = "Image '%s' not found";

    /// <summary>DXConsts.pas 34</summary>
    public const string SWaveNotFound = "Wave '%s' not found";

    /// <summary>DXConsts.pas 35</summary>
    public const string SEffectNotFound = "Effect '%s' not found";

    /// <summary>DXConsts.pas 36</summary>
    public const string SListIndexError = "Index of the list exceeds the range. (%d)";

    /// <summary>DXConsts.pas 37</summary>
    public const string SScanline = "Index of the scanning line exceeded the range. (%d)";

    /// <summary>DXConsts.pas 38</summary>
    public const string SNoForm = "Form not found";

    /// <summary>DXConsts.pas 39</summary>
    public const string SSinceDirectX5 = "Necessary since DirectX 5";

    /// <summary>DXConsts.pas 40</summary>
    public const string SSinceDirectX6 = "Necessary since DirectX 6";

    /// <summary>DXConsts.pas 41</summary>
    public const string SSinceDirectX7 = "Necessary since DirectX 7";

    /// <summary>DXConsts.pas 42</summary>
    public const string S3DDeviceNotFound = "3D device not found";

    /// <summary>DXConsts.pas 43</summary>
    public const string SDisplayModeChange = "Display mode cannot be changed (%dx%d %dbit)";

    /// <summary>DXConsts.pas 44</summary>
    public const string SDisplayModeCannotAcquired = "A present display mode cannot be acquired";

    /// <summary>DXConsts.pas 45</summary>
    public const string SInvalidDIB = "DIB is invalid";

    /// <summary>DXConsts.pas 46 —— 原文如此：Bitcount in invalid</summary>
    public const string SInvalidDIBBitCount = "Bitcount in invalid (%d)";

    /// <summary>DXConsts.pas 47 —— 原文如此：PixelFormat in invalid</summary>
    public const string SInvalidDIBPixelFormat = "PixelFormat in invalid";

    /// <summary>DXConsts.pas 48</summary>
    public const string SInvalidWave = "Wave is invalid";

    /// <summary>DXConsts.pas 49</summary>
    public const string SInvalidDisplayBitCount = "It should be either of 8 or 16 or 24 or 32";

    /// <summary>DXConsts.pas 50</summary>
    public const string SInvalidWaveFormat = "Format is invalid";

    /// <summary>DXConsts.pas 51</summary>
    public const string SNotSupported = "%s not supported";

    /// <summary>DXConsts.pas 52</summary>
    public const string SStreamOpend = "Stream has already been opened";

    /// <summary>DXConsts.pas 53</summary>
    public const string SNecessaryDirectInputUseMouse = "DirectInput is necessary to use the mouse";

    // ===== {  DirectPlay  }（DXConsts.pas 55-69）=====

    /// <summary>DXConsts.pas 56</summary>
    public const string SDXPlayNotConnectedNow = "TDXPlay component is not connected now.";

    /// <summary>DXConsts.pas 57</summary>
    public const string SDXPlayProviderNotFound = "Provider '%s' not found";

    /// <summary>DXConsts.pas 58</summary>
    public const string SDXPlayProviderSpecifiedGUIDNotFound = "Provider of specified GUID is not found";

    /// <summary>DXConsts.pas 59</summary>
    public const string SDXPlayModemListCannotBeAcquired = "Modem list cannot be acquired";

    /// <summary>DXConsts.pas 60</summary>
    public const string SDXPlaySessionListCannotBeAcquired = "Session list cannot be acquired";

    /// <summary>DXConsts.pas 61</summary>
    public const string SDXPlaySessionNotFound = "Session '%s' not found";

    /// <summary>DXConsts.pas 62 —— 原文如此：%s 没有引号（与上一条 SDXPlaySessionNotFound 不一致）</summary>
    public const string SDXPlaySessionCannotOpened = "Session %s cannot be opened";

    /// <summary>DXConsts.pas 63</summary>
    public const string SDXPlayPlayerNotFound = "The player of specified ID is not found";

    /// <summary>DXConsts.pas 64</summary>
    public const string SDXPlayMessageIllegal = "The message form is illegal";

    /// <summary>DXConsts.pas 65</summary>
    public const string SDXPlayPlayerNameIsNotSpecified = "Player name is not specified";

    /// <summary>DXConsts.pas 66</summary>
    public const string SDXPlaySessionNameIsNotSpecified = "Session name is not specified";

    /// <summary>DXConsts.pas 68</summary>
    public const string DXPlayFormNext = "Next >";

    /// <summary>DXConsts.pas 69</summary>
    public const string DXPlayFormComplete = "Complete";

    /// <summary>DXConsts.pas 73 —— 原文如此：suported（supported 的笔误）</summary>
    public const string SNotSupportGraphicFile = "This format graphic not suported";

    /// <summary>DXConsts.pas 74</summary>
    public const string SInvalidDXTFile = "This DXT file is invalid";

    /// <summary>DXConsts.pas 75</summary>
    public const string SCannotLoadGraphic = "Can't Load this Graphic";

    /// <summary>DXConsts.pas 76 —— 原文如此：posible（possible 的笔误）</summary>
    public const string SOverlay = "Not posible Overlay Surface";

    // ===== const 段（DXConsts.pas 78-99）=====

    /// <summary>DXConsts.pas 79</summary>
    public const string SDIBSize = "(%dx%d)";

    /// <summary>DXConsts.pas 80</summary>
    public const string SDIBColor = "%d color";

    /// <summary>DXConsts.pas 81</summary>
    public const string SDIBBitSize = "%d bytes";

    /// <summary>DXConsts.pas 82</summary>
    public const string SDIBBitSize_K = "%d Kbytes";

    /// <summary>DXConsts.pas 85</summary>
    public const string SWaveLength = "%.4g sec";

    /// <summary>DXConsts.pas 86</summary>
    public const string SWaveFrequency = "%dHz";

    /// <summary>DXConsts.pas 87</summary>
    public const string SWaveBitCount = "%dbit";

    /// <summary>DXConsts.pas 88</summary>
    public const string SWaveMono = "Mono";

    /// <summary>DXConsts.pas 89</summary>
    public const string SWaveStereo = "Stereo";

    /// <summary>DXConsts.pas 90</summary>
    public const string SWaveSize = "%d bytes";

    /// <summary>DXConsts.pas 93</summary>
    public const string SKeyLeft = "Left";

    /// <summary>DXConsts.pas 94</summary>
    public const string SKeyUp = "Up";

    /// <summary>DXConsts.pas 95</summary>
    public const string SKeyRight = "Right";

    /// <summary>DXConsts.pas 96</summary>
    public const string SKeyDown = "Down";

    /// <summary>DXConsts.pas 99</summary>
    public const string SFFBEffectEditor = "%s Effect Editor";

    /// <summary>
    /// 原文常量总数（DXConsts.pas 6-99 逐条清点：**63 resourcestring + 15 const = 78**）。
    /// 15 个 const 为：SDIBSize/SDIBColor/SDIBBitSize/SDIBBitSize_K（78-82）、
    /// SWaveLength/SWaveFrequency/SWaveBitCount/SWaveMono/SWaveStereo/SWaveSize（84-90）、
    /// SKeyLeft/SKeyUp/SKeyRight/SKeyDown（92-96）、SFFBEffectEditor（98-99）。
    /// 抽取脚本按 `^名 = '串';$` 逐行匹配 GBK 原文得到 78 条；C# 侧反射到的 const string
    /// 字段数同样为 78，并由 DxConstsTests 与该基线逐条双向比对。
    /// </summary>
    public const int MemberCount = 78;

    /// <summary>
    /// 回读比对基线：DXConsts.pas 抽取结果（名 = 串，Delphi 转义已还原），
    /// 由脚本从原文生成后原样粘贴，测试逐条与上面 const 成员比对。
    /// </summary>
    public static readonly string[] ExtractionBaseline =
    {
        "SNone|(None)", "SUnknownError|Unknown Error (%d)",
        "SDirectDraw|DirectDraw", "SDirect3DRM|Direct3D RetainedMode", "SDirectSound|DirectSound",
        "SDirectSoundCapture|DirectSoundCapture", "SDirectDrawClipper|Clipper",
        "SDirectDrawPalette|Palette", "SDirectDrawSurface|Surface",
        "SDirectDrawPrimarySurface|Primary Surface", "SDirectSoundBuffer|Sound Buffer",
        "SDirectSoundPrimaryBuffer|Primary Buffer", "SDirectSoundCaptureBuffer|Sound Capture Buffer",
        "STexture|Texture", "SDirectPlay|DirectPlay", "SSession|Session",
        "SNotMade|%s not made", "SStreamNotOpend|Stream not opend", "SWaveStreamNotSet|WaveStream not set",
        "SCannotMade|%s cannot be made", "SCannotInitialized|%s cannot be initialized",
        "SCannotChanged|%s cannot be changed", "SCannotLock|%s cannot be locked",
        "SCannotOpened|%s cannot be opened", "SDLLNotLoaded|%s not loaded",
        "SImageNotFound|Image '%s' not found", "SWaveNotFound|Wave '%s' not found",
        "SEffectNotFound|Effect '%s' not found",
        "SListIndexError|Index of the list exceeds the range. (%d)",
        "SScanline|Index of the scanning line exceeded the range. (%d)",
        "SNoForm|Form not found",
        "SSinceDirectX5|Necessary since DirectX 5", "SSinceDirectX6|Necessary since DirectX 6",
        "SSinceDirectX7|Necessary since DirectX 7", "S3DDeviceNotFound|3D device not found",
        "SDisplayModeChange|Display mode cannot be changed (%dx%d %dbit)",
        "SDisplayModeCannotAcquired|A present display mode cannot be acquired",
        "SInvalidDIB|DIB is invalid", "SInvalidDIBBitCount|Bitcount in invalid (%d)",
        "SInvalidDIBPixelFormat|PixelFormat in invalid", "SInvalidWave|Wave is invalid",
        "SInvalidDisplayBitCount|It should be either of 8 or 16 or 24 or 32",
        "SInvalidWaveFormat|Format is invalid", "SNotSupported|%s not supported",
        "SStreamOpend|Stream has already been opened",
        "SNecessaryDirectInputUseMouse|DirectInput is necessary to use the mouse",
        "SDXPlayNotConnectedNow|TDXPlay component is not connected now.",
        "SDXPlayProviderNotFound|Provider '%s' not found",
        "SDXPlayProviderSpecifiedGUIDNotFound|Provider of specified GUID is not found",
        "SDXPlayModemListCannotBeAcquired|Modem list cannot be acquired",
        "SDXPlaySessionListCannotBeAcquired|Session list cannot be acquired",
        "SDXPlaySessionNotFound|Session '%s' not found",
        "SDXPlaySessionCannotOpened|Session %s cannot be opened",
        "SDXPlayPlayerNotFound|The player of specified ID is not found",
        "SDXPlayMessageIllegal|The message form is illegal",
        "SDXPlayPlayerNameIsNotSpecified|Player name is not specified",
        "SDXPlaySessionNameIsNotSpecified|Session name is not specified",
        "DXPlayFormNext|Next >", "DXPlayFormComplete|Complete",
        "SNotSupportGraphicFile|This format graphic not suported",
        "SInvalidDXTFile|This DXT file is invalid", "SCannotLoadGraphic|Can't Load this Graphic",
        "SOverlay|Not posible Overlay Surface",
        "SDIBSize|(%dx%d)", "SDIBColor|%d color", "SDIBBitSize|%d bytes", "SDIBBitSize_K|%d Kbytes",
        "SWaveLength|%.4g sec", "SWaveFrequency|%dHz", "SWaveBitCount|%dbit",
        "SWaveMono|Mono", "SWaveStereo|Stereo", "SWaveSize|%d bytes",
        "SKeyLeft|Left", "SKeyUp|Up", "SKeyRight|Right", "SKeyDown|Down",
        "SFFBEffectEditor|%s Effect Editor",
    };
}
