unit Share;

interface
uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  ExtCtrls, GameImages, DxControls, DxComponents, DxMemo;
const
  PROVERSION = 20100101;
var
  g_SelectComponent: TObject; //{$IF  CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND};

  g_WPrguseImages: TGameImages;
  g_WPrguse2Images: TGameImages;
  g_WPrguse3Images: TGameImages;

  g_WisPrguseImages: TGameImages;
  g_WisPrguse2Images: TGameImages;
  g_WisPrguse3Images: TGameImages;

  g_WChrSelImages: TGameImages;

  g_WPrguseImages_16: TGameImages;
  g_WPrguse2Images_16: TGameImages;
  g_WPrguse3Images_16: TGameImages;
  g_WChrSelImages_16: TGameImages;

  g_WUIImages: TGameImages;
  g_WUI1Images: TGameImages;
  g_WUI2Images: TGameImages;
  g_WUI3Images: TGameImages;
  g_WNewopUIImages: TGameImages;
  g_sMirDataDirectory: string = 'D:\ÈÈÑª´«Ææ\';

  g_ClientVersion: TClientVersion = cvSerial;

  g_sGuiFileName: string;
  g_Background: TDxControlEngine;
  g_nBackground: Integer = 0;
  g_boClose: Boolean = False;
  g_boClearComponent: Boolean = False;
  g_boCanClose: Boolean = False;

  g_SaveComponentList: TStringList;

  g_nMouseX, g_nMouseY: Integer;
  g_nComponentX, g_nComponentY: Integer;

  g_MainBackground: TDxControlEngine;
  g_FileNameMemo: TDxScrollBox;
  g_BackgroundMemo: TDxScrollBox;
  g_FileNameList: TStringList;
  g_MirDataDirectoryList: array[TClientVersion] of string;
implementation
initialization
  begin
    g_FileNameList := TStringList.Create;
  end;
finalization
  begin
    g_FileNameList.Free;
  end;
end.

