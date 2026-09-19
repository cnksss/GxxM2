using GXX.Client.GUI.DxComponent;

namespace GXX.Client.GUI.Mir;

/// <summary>
/// MirSequelDlg.pas TSequelWindows（传奇续章装备窗口）1:1 移植（全文 273 行）。
///
/// 原文 49-61 行的一批 BottomLeft/BottomRight/BottomCenter 覆写处于 `{ }` 注释块内；
/// 81-271 行 LoadFromStream 中另有若干注释块（84-85、91-115、117-118），此处逐字保留。
/// </summary>
public class TSequelWindows : TSerialWindows
{
    /// <summary>MirSequelDlg.pas:70 constructor TSequelWindows.Create。</summary>
    public TSequelWindows()
    {
        // inherited;      // SerialWindowsDlg.pas:3280
        ClientVersion = TClientVersion.cvMirSequel;
    }

    /// <summary>MirSequelDlg.pas:76 destructor TSequelWindows.Destroy。</summary>
    public void Destroy()
    {
        // inherited;
    }

    /*
    procedure BottomLeftInRealArea(Sender: TObject; X, Y: Integer;
      var IsRealArea: Boolean); override;
    procedure BottomLeftDirectPaint(Sender: TObject); override;

    procedure BottomRightInRealArea(Sender: TObject; X, Y: Integer;
      var IsRealArea: Boolean); override;
    procedure BottomRightDirectPaint(Sender: TObject); override;

    procedure BottomCenterInRealArea(Sender: TObject; X, Y: Integer;
      var IsRealArea: Boolean); override;
    procedure BottomCenterDirectPaint(Sender: TObject); override;
    */

    /// <summary>MirSequelDlg.pas:81 procedure TSequelWindows.LoadFromStream(MemoryStream:TMemoryStream)。</summary>
    public override void LoadFromStream(System.IO.MemoryStream MemoryStream)
    {
        base.LoadFromStream(MemoryStream);                     // 83 inherited;

        // DLoginDlg.ImageIndex.ImageType := UI3_wil;
        // DLoginDlg.ImageIndex.Up := 2;

        DLogin.ImageIndex.ImageType = TImageType.UI3_wil;      // 87
        DLogin.ImageIndex.Up = 1;                              // 88

        // 传奇续章差按钮 chongchong 2014-04-15
        /*
        //DImageButtonAccount.Caption := '用户名';
        DImageButtonAccount.Left := 36;
        DImageButtonAccount.Top := 79;
        DImageButtonAccount.Visible := True;

        DEdId.Left := 118;
        DEdId.Top := 81;

        //DImageButtonPassWord.Caption := '密码';
        DImageButtonAccount.Left := 36;
        DImageButtonAccount.Top := 114;

        DEdPasswd.Left := 118;
        DEdPasswd.Top := 117;

        DLoginNew.Left := 30;
        DLoginNew.Top := 155;

        DLoginOK.Left := 160;
        DLoginOK.Top := 155;

        DLoginChgPw.Left := 160;
        DLoginChgPw.Top := 200;
        */

        // DSelServerDlg.ImageIndex.ImageType := UI3_wil;
        // DSelServerDlg.ImageIndex.Up := 2;

        DServerDlg.ImageIndex.ImageType = TImageType.UI3_wil;  // 120
        DServerDlg.ImageIndex.Up = 0;                          // 121

        if (ScreenSize.SCREENWIDTH != 1024)                    // 123
        {
            DSelectChr.ImageIndex.ImageType = TImageType.UI3_wil; // 124
            DSelectChr.ImageIndex.Up = 2;                         // 125
        }

        DHeroStateDlg185.ImageIndex.ImageType = TImageType.UI3_wil; // 128
        DHeroStateDlg185.ImageIndex.Up = 10;                        // 129

        DMyBag.ImageIndex.ImageType = TImageType.UI3_wil;      // 131
        DMyBag.ImageIndex.Up = 30;                             // 132
        DMyBag.ImageIndex.Hot = 31;                            // 133
        DMyBag.ImageIndex.Down = 32;                           // 134
        DMyBag.Left = DMyBag.Left - 14;                        // 135
        DMyBag.Top = DMyBag.Top - 10;                          // 136

        DMyMagic.ImageIndex.ImageType = TImageType.UI3_wil;    // 138
        DMyMagic.ImageIndex.Up = 40;                           // 139
        DMyMagic.ImageIndex.Hot = 41;                          // 140
        DMyMagic.ImageIndex.Down = 42;                         // 141
        DMyMagic.Left = DMyMagic.Left - 14;                    // 142
        DMyMagic.Top = DMyMagic.Top - 12;                      // 143

        DMyState.ImageIndex.ImageType = TImageType.UI3_wil;    // 145
        DMyState.ImageIndex.Up = 50;                           // 146
        DMyState.ImageIndex.Hot = 51;                          // 147
        DMyState.ImageIndex.Down = 52;                         // 148
        DMyState.Left = DMyState.Left - 14;                    // 149
        DMyState.Top = DMyState.Top - 12;                      // 150

        DVoice.ImageIndex.ImageType = TImageType.UI3_wil;      // 152
        DVoice.ImageIndex.Up = 60;                             // 153
        DVoice.ImageIndex.Hot = 61;                            // 154
        DVoice.ImageIndex.Down = 62;                           // 155
        DVoice.Left = DVoice.Left - 14;                        // 156
        DVoice.Top = DVoice.Top - 12;                          // 157

        DWeb.ImageIndex.ImageType = TImageType.UI3_wil;        // 159
        DWeb.ImageIndex.Up = 80;                               // 160
        DWeb.ImageIndex.Hot = 81;                              // 161
        DWeb.ImageIndex.Down = 82;                             // 162

        DBotMission.ImageIndex.ImageType = TImageType.UI3_wil; // 164
        DBotMission.ImageIndex.Up = 90;                        // 165
        DBotMission.ImageIndex.Hot = 91;                       // 166
        DBotMission.ImageIndex.Down = 92;                      // 167

        DActionLog.ImageIndex.ImageType = TImageType.UI3_wil;  // 169
        DActionLog.ImageIndex.Up = 521;                        // 170
        DActionLog.ImageIndex.Hot = 522;                       // 171
        DActionLog.ImageIndex.Down = 523;                      // 172

        DControlHelp.ImageIndex.ImageType = TImageType.UI3_wil; // 174
        DControlHelp.ImageIndex.Up = 100;                       // 175
        DControlHelp.ImageIndex.Hot = 101;                      // 176
        DControlHelp.ImageIndex.Down = 102;                     // 177

        DBotExit.ImageIndex.ImageType = TImageType.UI3_wil;    // 179
        DBotExit.ImageIndex.Up = 110;                          // 180
        DBotExit.ImageIndex.Hot = 111;                         // 181
        DBotExit.ImageIndex.Down = 112;                        // 182

        DBotFriend.ImageIndex.ImageType = TImageType.UI3_wil;  // 184
        DBotFriend.ImageIndex.Up = 120;                        // 185
        DBotFriend.ImageIndex.Hot = 121;                       // 186
        DBotFriend.ImageIndex.Down = 122;                      // 187

        DBotTrade.ImageIndex.ImageType = TImageType.UI3_wil;   // 189
        DBotTrade.ImageIndex.Up = 130;                         // 190
        DBotTrade.ImageIndex.Hot = 131;                        // 191
        DBotTrade.ImageIndex.Down = 132;                       // 192

        DBotRank.ImageIndex.ImageType = TImageType.UI3_wil;    // 194
        DBotRank.ImageIndex.Up = 140;                          // 195
        DBotRank.ImageIndex.Hot = 141;                         // 196
        DBotRank.ImageIndex.Down = 142;                        // 197

        DBotWhisper.ImageIndex.ImageType = TImageType.UI3_wil; // 199
        DBotWhisper.ImageIndex.Up = 150;                       // 200
        DBotWhisper.ImageIndex.Hot = 151;                      // 201
        DBotWhisper.ImageIndex.Down = 152;                     // 202

        DBotLogout.ImageIndex.ImageType = TImageType.UI3_wil;  // 204
        DBotLogout.ImageIndex.Up = 160;                        // 205
        DBotLogout.ImageIndex.Hot = 161;                       // 206
        DBotLogout.ImageIndex.Down = 162;                      // 207

        DBotGuild.ImageIndex.ImageType = TImageType.UI3_wil;   // 209
        DBotGuild.ImageIndex.Up = 170;                         // 210
        DBotGuild.ImageIndex.Hot = 171;                        // 211
        DBotGuild.ImageIndex.Down = 172;                       // 212

        DBotGroup.ImageIndex.ImageType = TImageType.UI3_wil;   // 214
        DBotGroup.ImageIndex.Up = 190;                         // 215
        DBotGroup.ImageIndex.Hot = 191;                        // 216
        DBotGroup.ImageIndex.Down = 192;                       // 217

        DBotMiniMap.ImageIndex.ImageType = TImageType.UI3_wil; // 219
        DBotMiniMap.ImageIndex.Up = 200;                       // 220
        DBotMiniMap.ImageIndex.Hot = 201;                      // 221
        DBotMiniMap.ImageIndex.Down = 202;                     // 222

        DBotFunc1.ImageIndex.ImageType = TImageType.UI3_wil;   // 224
        DBotFunc1.ImageIndex.Up = 220;                         // 225
        DBotFunc1.ImageIndex.Hot = 221;                        // 226
        DBotFunc1.ImageIndex.Down = 222;                       // 227

        DBotFunc2.ImageIndex.ImageType = TImageType.UI3_wil;   // 229
        DBotFunc2.ImageIndex.Up = 230;                         // 230
        DBotFunc2.ImageIndex.Hot = 231;                        // 231
        DBotFunc2.ImageIndex.Down = 232;                       // 232

        DBotFunc3.ImageIndex.ImageType = TImageType.UI3_wil;   // 234
        DBotFunc3.ImageIndex.Up = 240;                         // 235
        DBotFunc3.ImageIndex.Hot = 241;                        // 236
        DBotFunc3.ImageIndex.Down = 242;                       // 237

        DBotFunc4.ImageIndex.ImageType = TImageType.UI3_wil;   // 239
        DBotFunc4.ImageIndex.Up = 250;                         // 240
        DBotFunc4.ImageIndex.Hot = 251;                        // 241
        DBotFunc4.ImageIndex.Down = 252;                       // 242

        DBotFunc5.ImageIndex.ImageType = TImageType.UI3_wil;   // 244
        DBotFunc5.ImageIndex.Up = 260;                         // 245
        DBotFunc5.ImageIndex.Hot = 261;                        // 246
        DBotFunc5.ImageIndex.Down = 262;                       // 247

        DBotFunc6.ImageIndex.ImageType = TImageType.UI3_wil;   // 249
        DBotFunc6.ImageIndex.Up = 270;                         // 250
        DBotFunc6.ImageIndex.Hot = 271;                        // 251
        DBotFunc6.ImageIndex.Down = 272;                       // 252

        DChatMemo.PrevImageIndex.ImageType = TImageType.UI3_wil; // 254
        DChatMemo.PrevImageIndex.Up = 501;                       // 255
        DChatMemo.PrevImageIndex.Hot = 502;                      // 256
        DChatMemo.PrevImageIndex.Down = 503;                     // 257

        DChatMemo.NextImageIndex.ImageType = TImageType.UI3_wil; // 259
        DChatMemo.NextImageIndex.Up = 504;                       // 260
        DChatMemo.NextImageIndex.Hot = 505;                      // 261
        DChatMemo.NextImageIndex.Down = 506;                     // 262

        DChatMemo.BarImageIndex.ImageType = TImageType.UI3_wil;  // 264
        DChatMemo.BarImageIndex.Up = 507;                        // 265
        DChatMemo.BarImageIndex.Hot = 508;                       // 266
        DChatMemo.BarImageIndex.Down = 509;                      // 267

        DChatMemo.ScrollImageIndex.ImageType = TImageType.UI3_wil; // 269
        DChatMemo.ScrollImageIndex.Up = 500;                       // 270
    }
}
