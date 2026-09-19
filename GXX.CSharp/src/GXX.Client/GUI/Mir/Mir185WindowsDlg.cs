using GXX.Client.DxComponent;

namespace GXX.Client.GUI.Mir;

/// <summary>
/// Mir185WindowsDlg.pas T185Windows（185 版本装备窗口）1:1 移植（全文 187 行）。
///
/// 185/176/续章/205 这一族窗口没有 .dfm：控件树与几何由 SerialWindowsDlg.pas:17323
/// TSerialWindows.LoadFromStream 从 Mir.GUI 资源流载入，LoadFromStream 的 override 只在此基础上
/// 覆盖本版本的可见性/图号/位置（原文如此）。故此处每个属性赋值都带原文行号注释。
/// </summary>
public class T185Windows : TSerialWindows
{
    private TDxImageButton DPrevState;
    private TDxImageButton DNextState;

    /// <summary>Mir185WindowsDlg.pas:60 constructor T185Windows.Create。</summary>
    public T185Windows()
    {
        // inherited;      // SerialWindowsDlg.pas:3280
        ClientVersion = TClientVersion.cv185;
    }

    /// <summary>Mir185WindowsDlg.pas:66 destructor T185Windows.Destroy。</summary>
    public void Destroy()
    {
        // inherited;
    }

    /// <summary>Mir185WindowsDlg.pas:71 procedure T185Windows.LoadFromStream(MemoryStream:TMemoryStream)。</summary>
    public override void LoadFromStream(System.IO.MemoryStream MemoryStream)
    {
        base.LoadFromStream(MemoryStream);                     // 73 inherited;

        DImageButtonAccount.Visible = false;                   // 74
        DImageButtonPassWord.Visible = false;                  // 75

        DLogin.ImageIndex.ImageType = TImageType.Prguse_wil;   // 77 Prguse_wil;
        DLogin.ImageIndex.Up = 60;                             // 78

        DLoginOK.ImageIndex.ImageType = TImageType.Prguse_wil; // 80
        DLoginOK.ImageIndex.Up = -1;                           // 81
        DLoginOK.ImageIndex.Down = 62;                         // 82
        DLoginOK.OnPaint = null;                               // 83
        DLoginOK.Left = 169;                                   // 84
        DLoginOK.Top = 163;                                    // 85

        DLoginNew.ImageIndex.ImageType = TImageType.Prguse_wil; // 87
        DLoginNew.ImageIndex.Up = -1;                           // 88
        DLoginNew.ImageIndex.Down = 61;                         // 89
        DLoginNew.OnPaint = null;                               // 90
        DLoginNew.Left = 25;                                    // 91
        DLoginNew.Top = 207;                                    // 92

        DLoginChgPw.ImageIndex.ImageType = TImageType.Prguse_wil; // 94
        DLoginChgPw.ImageIndex.Up = -1;                           // 95
        DLoginChgPw.ImageIndex.Down = 53;                         // 96
        DLoginChgPw.OnPaint = null;                               // 97
        DLoginChgPw.Left = 130;                                   // 98
        DLoginChgPw.Top = 207;                                    // 99

        DLoginClose.Left = 252;                                // 101
        DLoginClose.Top = 28;                                  // 102

        DEdId_.Left = 98;                                      // 104
        DEdId_.Top = 85;                                       // 105

        DEdPasswd_.Left = 98;                                  // 107
        DEdPasswd_.Top = 117;                                  // 108

        DBottomLeftImageButton1.Visible = false;               // 110
        DMerchantDlgHelp_.Visible = false;                     // 111
        DChangeState.Visible = false;                          // 112

        DSWTitleActive.Visible = false;                        // 114
        DSWTitleButton1.Visible = false;                       // 115
        DSWTitleButton2.Visible = false;                       // 116
        DSWTitleButton3.Visible = false;                       // 117
        DSWTitleButton4.Visible = false;                       // 118
        DSWTitlePageUp.Visible = false;                        // 119
        DSWTitlePageDown.Visible = false;                      // 120

        DSUSTitleActive.Visible = false;                       // 122
        DSUSTitleButton1.Visible = false;                      // 123
        DSUSTitleButton2.Visible = false;                      // 124
        DSUSTitleButton3.Visible = false;                      // 125
        DSUSTitleButton4.Visible = false;                      // 126
        DSUSTitlePageUp.Visible = false;                       // 127
        DSUSTitlePageDown.Visible = false;                     // 128

        DUserState1.ImageIndex.ImageType = TImageType.Prguse3_wil; // 130
        DUserState1.ImageIndex.Up = 207;                           // 131

        DStateWin.ImageIndex.ImageType = TImageType.Prguse3_wil;   // 133
        DStateWin.ImageIndex.Up = 207;                             // 134

        DPrevState = new TDxImageButton(DStateWin);            // 136
        DNextState = new TDxImageButton(DStateWin);            // 137
        DPrevState.Designing = false;                          // 138
        DNextState.Designing = false;                          // 139

        DPrevState.OnGetImage = DStateWin.OnGetImage;          // 141
        DPrevState.ImageIndex.ImageType = TImageType.Prguse_wil; // 142
        DPrevState.ImageIndex.Down = 373;                      // 143
        DPrevState.Left = 7;                                   // 144
        DPrevState.Top = 128;                                  // 145
        DPrevState.OnClick = DPrevStateClick;                  // 146
        DPrevState.OnClickSound = DLoginNewClickSound;         // 147
        DPrevState.ClickCount = TClickSound.csGlass;           // 148

        DNextState.OnGetImage = DStateWin.OnGetImage;          // 150
        DNextState.ImageIndex.ImageType = TImageType.Prguse_wil; // 151
        DNextState.ImageIndex.Down = 372;                      // 152
        DNextState.Left = 7;                                   // 153
        DNextState.Top = 187;                                  // 154
        DNextState.OnClick = DNextStateClick;                  // 155
        DNextState.OnClickSound = DLoginNewClickSound;         // 156
        DNextState.ClickCount = TClickSound.csGlass;           // 157

        // while DStatePageControl.PageCount > 4 do
        // DStatePageControl.Control[DStatePageControl.ControlCount - 1].Free;

        DStateTabSheet5.TabVisible = false;                    // 162

        DMerchantDlgHelp_.Visible = false;                     // 164
        DRecallHero.Visible = false;                           // 165
        DMyHeroState.Visible = false;                          // 166
        DMyHeroBag.Visible = false;                            // 167
        DRecallDeputyHero.Visible = false;                     // 168
    }

    /// <summary>Mir185WindowsDlg.pas:171 procedure T185Windows.DPrevStateClick(Sender:TObject; X, Y:Integer)。</summary>
    public void DPrevStateClick(object Sender, int X, int Y)
    {
        if (DStatePageControl.ActivePageIndex <= 0)
            DStatePageControl.ActivePageIndex = 3;             // 174 //DStatePageControl.PageCount - 2
        else
            DStatePageControl.ActivePageIndex = DStatePageControl.ActivePageIndex - 1;
    }

    /// <summary>Mir185WindowsDlg.pas:179 procedure T185Windows.DNextStateClick(Sender:TObject; X, Y:Integer)。</summary>
    public void DNextStateClick(object Sender, int X, int Y)
    {
        if (DStatePageControl.ActivePageIndex >= 3)            // 181 {DStatePageControl.PageCount - 2}
            DStatePageControl.ActivePageIndex = 0;
        else
            DStatePageControl.ActivePageIndex = DStatePageControl.ActivePageIndex + 1;
    }
}
