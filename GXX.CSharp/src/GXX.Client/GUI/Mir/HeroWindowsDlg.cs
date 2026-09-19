using GXX.Client.GUI.DxComponent;

namespace GXX.Client.GUI.Mir;

/// <summary>
/// HeroWindowsDlg.pas THeroWindows（英雄版本）1:1 移植（全文 78 行）。
///
/// 注：原文 42-59 行的 DPrevState/DNextState/DHeroPrevState/DHeroNextState 字段与
/// LoadFromStream/HeroStateWinDirectPaint/StateMemo4DirectPaint/DPrevStateClick/DNextStateClick/
/// DHeroPrevStateClick/DHeroNextStateClick 全部处于 `{ }` 注释块内，故 THeroWindows 实际只
/// 覆写构造/析构并设置 ClientVersion（见 67-76 行）。此处按原文逐字保留该注释块。
/// </summary>
public class THeroWindows : TSerialWindows
{
    // {DPrevState: TDxImageButton;
    // DNextState: TDxImageButton;
    //
    // DHeroPrevState: TDxImageButton;
    // DHeroNextState: TDxImageButton;  }

    /// <summary>HeroWindowsDlg.pas:67 constructor THeroWindows.Create。</summary>
    public THeroWindows()
    {
        // inherited;      // SerialWindowsDlg.pas:3280 TSerialWindows.Create
        ClientVersion = TClientVersion.cvHero;
    }

    /// <summary>HeroWindowsDlg.pas:73 destructor THeroWindows.Destroy。</summary>
    public void Destroy()
    {
        // inherited;      // 原文仅调用继承析构（托管侧由 Dispose/GC 承接）
    }

    // {procedure LoadFromStream(MemoryStream: TMemoryStream); override;
    // procedure HeroStateWinDirectPaint(Sender: TObject); override;
    // procedure StateMemo4DirectPaint(Sender: TObject); override;
    // procedure DPrevStateClick(Sender: TObject; X, Y: Integer); stdcall;
    // procedure DNextStateClick(Sender: TObject; X, Y: Integer); stdcall;
    //
    // procedure DHeroPrevStateClick(Sender: TObject; X, Y: Integer); stdcall;
    // procedure DHeroNextStateClick(Sender: TObject; X, Y: Integer); stdcall;  }
}
