unit UpdateCommon;

interface
uses Classes;
const
  CHECKCODE1 = $AA55AA55;
  CHECKCODE2 = $FFBBA0DA;
  CHECKCODE3 = $CCD1A05F;
  CHECKCODE4 = $E05FABF3;

  CM_SOCKETCONNECT = 100;                                                                           // 连接
  CM_UPDATEBUFFER = 101;                                                                            // 请求更新
  CM_STARTUPDATE = 102;                                                                             // 开始更新
  CM_GATECHECK = 103;                                                                               // 网关检测信号 piaoyun 2013-11-22
  CM_CHECK_CODE_RECV = 104;

  SM_UPDATEBUFFER_OK = 1000;                                                                        //
  SM_UPDATEBUFFER_FAIL = 1001;                                                                      //
  SM_GATECHECK = 1003;                                                                              // 网关检测信号 piaoyun 2013-11-22
  // CM_MAINHANDLE = 200;

  WM_DATA = 202;                                                                                    // 202 - 208
  WM_COMPDATA = 203;
  WM_UPDATE_OK = 204;
  WM_UPDATE_FAIL = 205;
  WM_UPDATE_STOP = 206;
  WM_CHECK_CODE = 207;


type
  TUpdateFileType = (utFiles, utImage, utIndex);
  TUpdateDirectory = (dtData, dtMap, dtWav, dtMusic, dtSelf);
  TStreamSaveToFile = procedure(Sender: TObject; Stream: TMemoryStream; Index: Integer; const FileName: string) of object;
  TSocketBuffer = packed record
    dwCode1: LongWord;
    dwCode2: LongWord;
    dwCode3: LongWord;
    dwCode4: LongWord;

    dwCrc: LongWord;
    Ident: Word;
    //nRecogId: Integer;              // 加入包的识别码 chongchong 2016-01-18
    nLength: Integer;
  end;
  pTSocketBuffer = ^TSocketBuffer;

  TUpdateInfo = record
    Handle: THandle;
    DataType: TUpdateFileType;
    Directory: TUpdateDirectory;
    Index, Position: Integer;
    Result: Boolean;
  end;
  pTUpdateInfo = ^TUpdateInfo;

  TPakKey = packed record
    ImageCount: Integer;
    PakType: Word;
    KeyData: array[0..31] of LongWord;
    Chain: array[0..20 - 1] of Byte;
    Reserve: array[0..5] of Integer;
  end;
  pTPakKey = ^TPakKey;
implementation


end.
