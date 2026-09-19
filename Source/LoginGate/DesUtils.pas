unit DesUtils;

interface

uses
  Windows, Classes, Sysutils;

type
  PDWordArray = ^TDWordArray;
  TDWordArray = array[0..8191] of DWord;

procedure EncryptDes_New(const Indata; var Outdata; Size: Longint; const Key: string);
procedure DecryptDes_New(const Indata; var Outdata; Size: Longint; const Key: string);

implementation

uses UnitHash;

{$R-}{$Q-}
const
  BS = 20;                                                                                          { The block size in bytes for internal use }

  shifts2: array[0..15] of byte =
  (0, 0, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 0);

  des_skb: array[0..7, 0..63] of dword = (
    (
    (* for C bits (numbered as per FIPS 46) 1 2 3 4 5 6 *)
    $00000000, $00000010, $20000000, $20000010,
    $00010000, $00010010, $20010000, $20010010,
    $00000800, $00000810, $20000800, $20000810,
    $00010800, $00010810, $20010800, $20010810,
    $00000020, $00000030, $20000020, $20000030,
    $00010020, $00010030, $20010020, $20010030,
    $00000820, $00000830, $20000820, $20000830,
    $00010820, $00010830, $20010820, $20010830,
    $00080000, $00080010, $20080000, $20080010,
    $00090000, $00090010, $20090000, $20090010,
    $00080800, $00080810, $20080800, $20080810,
    $00090800, $00090810, $20090800, $20090810,
    $00080020, $00080030, $20080020, $20080030,
    $00090020, $00090030, $20090020, $20090030,
    $00080820, $00080830, $20080820, $20080830,
    $00090820, $00090830, $20090820, $20090830
    ), (
    (* for C bits (numbered as per FIPS 46) 7 8 10 11 12 13 *)
    $00000000, $02000000, $00002000, $02002000,
    $00200000, $02200000, $00202000, $02202000,
    $00000004, $02000004, $00002004, $02002004,
    $00200004, $02200004, $00202004, $02202004,
    $00000400, $02000400, $00002400, $02002400,
    $00200400, $02200400, $00202400, $02202400,
    $00000404, $02000404, $00002404, $02002404,
    $00200404, $02200404, $00202404, $02202404,
    $10000000, $12000000, $10002000, $12002000,
    $10200000, $12200000, $10202000, $12202000,
    $10000004, $12000004, $10002004, $12002004,
    $10200004, $12200004, $10202004, $12202004,
    $10000400, $12000400, $10002400, $12002400,
    $10200400, $12200400, $10202400, $12202400,
    $10000404, $12000404, $10002404, $12002404,
    $10200404, $12200404, $10202404, $12202404
    ), (
    (* for C bits (numbered as per FIPS 46) 14 15 16 17 19 20 *)
    $00000000, $00000001, $00040000, $00040001,
    $01000000, $01000001, $01040000, $01040001,
    $00000002, $00000003, $00040002, $00040003,
    $01000002, $01000003, $01040002, $01040003,
    $00000200, $00000201, $00040200, $00040201,
    $01000200, $01000201, $01040200, $01040201,
    $00000202, $00000203, $00040202, $00040203,
    $01000202, $01000203, $01040202, $01040203,
    $08000000, $08000001, $08040000, $08040001,
    $09000000, $09000001, $09040000, $09040001,
    $08000002, $08000003, $08040002, $08040003,
    $09000002, $09000003, $09040002, $09040003,
    $08000200, $08000201, $08040200, $08040201,
    $09000200, $09000201, $09040200, $09040201,
    $08000202, $08000203, $08040202, $08040203,
    $09000202, $09000203, $09040202, $09040203
    ), (
    (* for C bits (numbered as per FIPS 46) 21 23 24 26 27 28 *)
    $00000000, $00100000, $00000100, $00100100,
    $00000008, $00100008, $00000108, $00100108,
    $00001000, $00101000, $00001100, $00101100,
    $00001008, $00101008, $00001108, $00101108,
    $04000000, $04100000, $04000100, $04100100,
    $04000008, $04100008, $04000108, $04100108,
    $04001000, $04101000, $04001100, $04101100,
    $04001008, $04101008, $04001108, $04101108,
    $00020000, $00120000, $00020100, $00120100,
    $00020008, $00120008, $00020108, $00120108,
    $00021000, $00121000, $00021100, $00121100,
    $00021008, $00121008, $00021108, $00121108,
    $04020000, $04120000, $04020100, $04120100,
    $04020008, $04120008, $04020108, $04120108,
    $04021000, $04121000, $04021100, $04121100,
    $04021008, $04121008, $04021108, $04121108
    ), (
    (* for D bits (numbered as per FIPS 46) 1 2 3 4 5 6 *)
    $00000000, $10000000, $00010000, $10010000,
    $00000004, $10000004, $00010004, $10010004,
    $20000000, $30000000, $20010000, $30010000,
    $20000004, $30000004, $20010004, $30010004,
    $00100000, $10100000, $00110000, $10110000,
    $00100004, $10100004, $00110004, $10110004,
    $20100000, $30100000, $20110000, $30110000,
    $20100004, $30100004, $20110004, $30110004,
    $00001000, $10001000, $00011000, $10011000,
    $00001004, $10001004, $00011004, $10011004,
    $20001000, $30001000, $20011000, $30011000,
    $20001004, $30001004, $20011004, $30011004,
    $00101000, $10101000, $00111000, $10111000,
    $00101004, $10101004, $00111004, $10111004,
    $20101000, $30101000, $20111000, $30111000,
    $20101004, $30101004, $20111004, $30111004
    ), (
    (* for D bits (numbered as per FIPS 46) 8 9 11 12 13 14 *)
    $00000000, $08000000, $00000008, $08000008,
    $00000400, $08000400, $00000408, $08000408,
    $00020000, $08020000, $00020008, $08020008,
    $00020400, $08020400, $00020408, $08020408,
    $00000001, $08000001, $00000009, $08000009,
    $00000401, $08000401, $00000409, $08000409,
    $00020001, $08020001, $00020009, $08020009,
    $00020401, $08020401, $00020409, $08020409,
    $02000000, $0A000000, $02000008, $0A000008,
    $02000400, $0A000400, $02000408, $0A000408,
    $02020000, $0A020000, $02020008, $0A020008,
    $02020400, $0A020400, $02020408, $0A020408,
    $02000001, $0A000001, $02000009, $0A000009,
    $02000401, $0A000401, $02000409, $0A000409,
    $02020001, $0A020001, $02020009, $0A020009,
    $02020401, $0A020401, $02020409, $0A020409
    ), (
    (* for D bits (numbered as per FIPS 46) 16 17 18 19 20 21 *)
    $00000000, $00000100, $00080000, $00080100,
    $01000000, $01000100, $01080000, $01080100,
    $00000010, $00000110, $00080010, $00080110,
    $01000010, $01000110, $01080010, $01080110,
    $00200000, $00200100, $00280000, $00280100,
    $01200000, $01200100, $01280000, $01280100,
    $00200010, $00200110, $00280010, $00280110,
    $01200010, $01200110, $01280010, $01280110,
    $00000200, $00000300, $00080200, $00080300,
    $01000200, $01000300, $01080200, $01080300,
    $00000210, $00000310, $00080210, $00080310,
    $01000210, $01000310, $01080210, $01080310,
    $00200200, $00200300, $00280200, $00280300,
    $01200200, $01200300, $01280200, $01280300,
    $00200210, $00200310, $00280210, $00280310,
    $01200210, $01200310, $01280210, $01280310
    ), (
    (* for D bits (numbered as per FIPS 46) 22 23 24 25 27 28 *)
    $00000000, $04000000, $00040000, $04040000,
    $00000002, $04000002, $00040002, $04040002,
    $00002000, $04002000, $00042000, $04042000,
    $00002002, $04002002, $00042002, $04042002,
    $00000020, $04000020, $00040020, $04040020,
    $00000022, $04000022, $00040022, $04040022,
    $00002020, $04002020, $00042020, $04042020,
    $00002022, $04002022, $00042022, $04042022,
    $00000800, $04000800, $00040800, $04040800,
    $00000802, $04000802, $00040802, $04040802,
    $00002800, $04002800, $00042800, $04042800,
    $00002802, $04002802, $00042802, $04042802,
    $00000820, $04000820, $00040820, $04040820,
    $00000822, $04000822, $00040822, $04040822,
    $00002820, $04002820, $00042820, $04042820,
    $00002822, $04002822, $00042822, $04042822
    ));

  des_sptrans: array[0..7, 0..63] of dword = (
    (
    (* nibble 0 *)
    $02080800, $00080000, $02000002, $02080802,
    $02000000, $00080802, $00080002, $02000002,
    $00080802, $02080800, $02080000, $00000802,
    $02000802, $02000000, $00000000, $00080002,
    $00080000, $00000002, $02000800, $00080800,
    $02080802, $02080000, $00000802, $02000800,
    $00000002, $00000800, $00080800, $02080002,
    $00000800, $02000802, $02080002, $00000000,
    $00000000, $02080802, $02000800, $00080002,
    $02080800, $00080000, $00000802, $02000800,
    $02080002, $00000800, $00080800, $02000002,
    $00080802, $00000002, $02000002, $02080000,
    $02080802, $00080800, $02080000, $02000802,
    $02000000, $00000802, $00080002, $00000000,
    $00080000, $02000000, $02000802, $02080800,
    $00000002, $02080002, $00000800, $00080802
    ), (
    (* nibble 1 *)
    $40108010, $00000000, $00108000, $40100000,
    $40000010, $00008010, $40008000, $00108000,
    $00008000, $40100010, $00000010, $40008000,
    $00100010, $40108000, $40100000, $00000010,
    $00100000, $40008010, $40100010, $00008000,
    $00108010, $40000000, $00000000, $00100010,
    $40008010, $00108010, $40108000, $40000010,
    $40000000, $00100000, $00008010, $40108010,
    $00100010, $40108000, $40008000, $00108010,
    $40108010, $00100010, $40000010, $00000000,
    $40000000, $00008010, $00100000, $40100010,
    $00008000, $40000000, $00108010, $40008010,
    $40108000, $00008000, $00000000, $40000010,
    $00000010, $40108010, $00108000, $40100000,
    $40100010, $00100000, $00008010, $40008000,
    $40008010, $00000010, $40100000, $00108000
    ), (
    (* nibble 2 *)
    $04000001, $04040100, $00000100, $04000101,
    $00040001, $04000000, $04000101, $00040100,
    $04000100, $00040000, $04040000, $00000001,
    $04040101, $00000101, $00000001, $04040001,
    $00000000, $00040001, $04040100, $00000100,
    $00000101, $04040101, $00040000, $04000001,
    $04040001, $04000100, $00040101, $04040000,
    $00040100, $00000000, $04000000, $00040101,
    $04040100, $00000100, $00000001, $00040000,
    $00000101, $00040001, $04040000, $04000101,
    $00000000, $04040100, $00040100, $04040001,
    $00040001, $04000000, $04040101, $00000001,
    $00040101, $04000001, $04000000, $04040101,
    $00040000, $04000100, $04000101, $00040100,
    $04000100, $00000000, $04040001, $00000101,
    $04000001, $00040101, $00000100, $04040000
    ), (
    (* nibble 3 *)
    $00401008, $10001000, $00000008, $10401008,
    $00000000, $10400000, $10001008, $00400008,
    $10401000, $10000008, $10000000, $00001008,
    $10000008, $00401008, $00400000, $10000000,
    $10400008, $00401000, $00001000, $00000008,
    $00401000, $10001008, $10400000, $00001000,
    $00001008, $00000000, $00400008, $10401000,
    $10001000, $10400008, $10401008, $00400000,
    $10400008, $00001008, $00400000, $10000008,
    $00401000, $10001000, $00000008, $10400000,
    $10001008, $00000000, $00001000, $00400008,
    $00000000, $10400008, $10401000, $00001000,
    $10000000, $10401008, $00401008, $00400000,
    $10401008, $00000008, $10001000, $00401008,
    $00400008, $00401000, $10400000, $10001008,
    $00001008, $10000000, $10000008, $10401000
    ), (
    (* nibble 4 *)
    $08000000, $00010000, $00000400, $08010420,
    $08010020, $08000400, $00010420, $08010000,
    $00010000, $00000020, $08000020, $00010400,
    $08000420, $08010020, $08010400, $00000000,
    $00010400, $08000000, $00010020, $00000420,
    $08000400, $00010420, $00000000, $08000020,
    $00000020, $08000420, $08010420, $00010020,
    $08010000, $00000400, $00000420, $08010400,
    $08010400, $08000420, $00010020, $08010000,
    $00010000, $00000020, $08000020, $08000400,
    $08000000, $00010400, $08010420, $00000000,
    $00010420, $08000000, $00000400, $00010020,
    $08000420, $00000400, $00000000, $08010420,
    $08010020, $08010400, $00000420, $00010000,
    $00010400, $08010020, $08000400, $00000420,
    $00000020, $00010420, $08010000, $08000020
    ), (
    (* nibble 5 *)
    $80000040, $00200040, $00000000, $80202000,
    $00200040, $00002000, $80002040, $00200000,
    $00002040, $80202040, $00202000, $80000000,
    $80002000, $80000040, $80200000, $00202040,
    $00200000, $80002040, $80200040, $00000000,
    $00002000, $00000040, $80202000, $80200040,
    $80202040, $80200000, $80000000, $00002040,
    $00000040, $00202000, $00202040, $80002000,
    $00002040, $80000000, $80002000, $00202040,
    $80202000, $00200040, $00000000, $80002000,
    $80000000, $00002000, $80200040, $00200000,
    $00200040, $80202040, $00202000, $00000040,
    $80202040, $00202000, $00200000, $80002040,
    $80000040, $80200000, $00202040, $00000000,
    $00002000, $80000040, $80002040, $80202000,
    $80200000, $00002040, $00000040, $80200040
    ), (
    (* nibble 6 *)
    $00004000, $00000200, $01000200, $01000004,
    $01004204, $00004004, $00004200, $00000000,
    $01000000, $01000204, $00000204, $01004000,
    $00000004, $01004200, $01004000, $00000204,
    $01000204, $00004000, $00004004, $01004204,
    $00000000, $01000200, $01000004, $00004200,
    $01004004, $00004204, $01004200, $00000004,
    $00004204, $01004004, $00000200, $01000000,
    $00004204, $01004000, $01004004, $00000204,
    $00004000, $00000200, $01000000, $01004004,
    $01000204, $00004204, $00004200, $00000000,
    $00000200, $01000004, $00000004, $01000200,
    $00000000, $01000204, $01000200, $00004200,
    $00000204, $00004000, $01004204, $01000000,
    $01004200, $00000004, $00004004, $01004204,
    $01000004, $01004200, $01004000, $00004004
    ), (
    (* nibble 7 *)
    $20800080, $20820000, $00020080, $00000000,
    $20020000, $00800080, $20800000, $20820080,
    $00000080, $20000000, $00820000, $00020080,
    $00820080, $20020080, $20000080, $20800000,
    $00020000, $00820080, $00800080, $20020000,
    $20820080, $20000080, $00000000, $00820000,
    $20000000, $00800000, $20020080, $20800080,
    $00800000, $00020000, $20820000, $00000080,
    $00800000, $00020000, $20000080, $20820080,
    $00020080, $20000000, $00000000, $00820000,
    $20800080, $20020080, $20020000, $00800080,
    $20820000, $00000080, $00800080, $20020000,
    $20820080, $00800000, $20800000, $20000080,
    $00820000, $00020080, $20020080, $20800000,
    $00000080, $20820000, $00820080, $00000000,
    $20000000, $20800080, $00020000, $00820080
    ));


  crc_table: array[0..255] of Cardinal = (
    $00000000, $77073096, $EE0E612C, $990951BA, $076DC419,
    $706AF48F, $E963A535, $9E6495A3, $0EDB8832, $79DCB8A4,
    $E0D5E91E, $97D2D988, $09B64C2B, $7EB17CBD, $E7B82D07,
    $90BF1D91, $1DB71064, $6AB020F2, $F3B97148, $84BE41DE,
    $1ADAD47D, $6DDDE4EB, $F4D4B551, $83D385C7, $136C9856,
    $646BA8C0, $FD62F97A, $8A65C9EC, $14015C4F, $63066CD9,
    $FA0F3D63, $8D080DF5, $3B6E20C8, $4C69105E, $D56041E4,
    $A2677172, $3C03E4D1, $4B04D447, $D20D85FD, $A50AB56B,
    $35B5A8FA, $42B2986C, $DBBBC9D6, $ACBCF940, $32D86CE3,
    $45DF5C75, $DCD60DCF, $ABD13D59, $26D930AC, $51DE003A,
    $C8D75180, $BFD06116, $21B4F4B5, $56B3C423, $CFBA9599,
    $B8BDA50F, $2802B89E, $5F058808, $C60CD9B2, $B10BE924,
    $2F6F7C87, $58684C11, $C1611DAB, $B6662D3D, $76DC4190,
    $01DB7106, $98D220BC, $EFD5102A, $71B18589, $06B6B51F,
    $9FBFE4A5, $E8B8D433, $7807C9A2, $0F00F934, $9609A88E,
    $E10E9818, $7F6A0DBB, $086D3D2D, $91646C97, $E6635C01,
    $6B6B51F4, $1C6C6162, $856530D8, $F262004E, $6C0695ED,
    $1B01A57B, $8208F4C1, $F50FC457, $65B0D9C6, $12B7E950,
    $8BBEB8EA, $FCB9887C, $62DD1DDF, $15DA2D49, $8CD37CF3,
    $FBD44C65, $4DB26158, $3AB551CE, $A3BC0074, $D4BB30E2,
    $4ADFA541, $3DD895D7, $A4D1C46D, $D3D6F4FB, $4369E96A,
    $346ED9FC, $AD678846, $DA60B8D0, $44042D73, $33031DE5,
    $AA0A4C5F, $DD0D7CC9, $5005713C, $270241AA, $BE0B1010,
    $C90C2086, $5768B525, $206F85B3, $B966D409, $CE61E49F,
    $5EDEF90E, $29D9C998, $B0D09822, $C7D7A8B4, $59B33D17,
    $2EB40D81, $B7BD5C3B, $C0BA6CAD, $EDB88320, $9ABFB3B6,
    $03B6E20C, $74B1D29A, $EAD54739, $9DD277AF, $04DB2615,
    $73DC1683, $E3630B12, $94643B84, $0D6D6A3E, $7A6A5AA8,
    $E40ECF0B, $9309FF9D, $0A00AE27, $7D079EB1, $F00F9344,
    $8708A3D2, $1E01F268, $6906C2FE, $F762575D, $806567CB,
    $196C3671, $6E6B06E7, $FED41B76, $89D32BE0, $10DA7A5A,
    $67DD4ACC, $F9B9DF6F, $8EBEEFF9, $17B7BE43, $60B08ED5,
    $D6D6A3E8, $A1D1937E, $38D8C2C4, $4FDFF252, $D1BB67F1,
    $A6BC5767, $3FB506DD, $48B2364B, $D80D2BDA, $AF0A1B4C,
    $36034AF6, $41047A60, $DF60EFC3, $A867DF55, $316E8EEF,
    $4669BE79, $CB61B38C, $BC66831A, $256FD2A0, $5268E236,
    $CC0C7795, $BB0B4703, $220216B9, $5505262F, $C5BA3BBE,
    $B2BD0B28, $2BB45A92, $5CB36A04, $C2D7FFA7, $B5D0CF31,
    $2CD99E8B, $5BDEAE1D, $9B64C2B0, $EC63F226, $756AA39C,
    $026D930A, $9C0906A9, $EB0E363F, $72076785, $05005713,
    $95BF4A82, $E2B87A14, $7BB12BAE, $0CB61B38, $92D28E9B,
    $E5D5BE0D, $7CDCEFB7, $0BDBDF21, $86D3D2D4, $F1D4E242,
    $68DDB3F8, $1FDA836E, $81BE16CD, $F6B9265B, $6FB077E1,
    $18B74777, $88085AE6, $FF0F6A70, $66063BCA, $11010B5C,
    $8F659EFF, $F862AE69, $616BFFD3, $166CCF45, $A00AE278,
    $D70DD2EE, $4E048354, $3903B3C2, $A7672661, $D06016F7,
    $4969474D, $3E6E77DB, $AED16A4A, $D9D65ADC, $40DF0B66,
    $37D83BF0, $A9BCAE53, $DEBB9EC5, $47B2CF7F, $30B5FFE9,
    $BDBDF21C, $CABAC28A, $53B39330, $24B4A3A6, $BAD03605,
    $CDD70693, $54DE5729, $23D967BF, $B3667A2E, $C4614AB8,
    $5D681B02, $2A6F2B94, $B40BBE37, $C30C8EA1, $5A05DF1B,
    $2D02EF8D);


procedure hperm_op(var a, t: dword; n, m: dword);
begin
  t := ((a shl (16 - n)) xor a) and m;
  a := a xor t xor (t shr (16 - n));
end;

procedure perm_op(var a, b, t: dword; n, m: dword);
begin
  t := ((a shr n) xor b) and m;
  b := b xor t;
  a := a xor (t shl n);
end;

procedure DoInit(KeyB: PByteArray; KeyData: PDwordArray);
var
  c, d, t, s, t2, i: dword;
begin
  c := KeyB^[0] or (KeyB^[1] shl 8) or (KeyB^[2] shl 16) or (KeyB^[3] shl 24);
  d := KeyB^[4] or (KeyB^[5] shl 8) or (KeyB^[6] shl 16) or (KeyB^[7] shl 24);
  perm_op(d, c, t, 4, $0F0F0F0F);
  hperm_op(c, t, dword(-2), $CCCC0000);
  hperm_op(d, t, dword(-2), $CCCC0000);
  perm_op(d, c, t, 1, $55555555);
  perm_op(c, d, t, 8, $00FF00FF);
  perm_op(d, c, t, 1, $55555555);
  d := ((d and $FF) shl 16) or (d and $FF00) or ((d and $FF0000) shr 16) or
    ((c and $F0000000) shr 4);
  c := c and $FFFFFFF;
  for i := 0 to 15 do
  begin
    if shifts2[i] <> 0 then
    begin
      c := ((c shr 2) or (c shl 26));
      d := ((d shr 2) or (d shl 26));
    end
    else
    begin
      c := ((c shr 1) or (c shl 27));
      d := ((d shr 1) or (d shl 27));
    end;
    c := c and $FFFFFFF;
    d := d and $FFFFFFF;
    s := des_skb[0, c and $3F] or
      des_skb[1, ((c shr 6) and $03) or ((c shr 7) and $3C)] or
      des_skb[2, ((c shr 13) and $0F) or ((c shr 14) and $30)] or
      des_skb[3, ((c shr 20) and $01) or ((c shr 21) and $06) or ((c shr 22) and $38)];
    t := des_skb[4, d and $3F] or
      des_skb[5, ((d shr 7) and $03) or ((d shr 8) and $3C)] or
      des_skb[6, (d shr 15) and $3F] or
      des_skb[7, ((d shr 21) and $0F) or ((d shr 22) and $30)];
    t2 := ((t shl 16) or (s and $FFFF));
    KeyData^[(i shl 1)] := ((t2 shl 2) or (t2 shr 30));
    t2 := ((s shr 16) or (t and $FFFF0000));
    KeyData^[(i shl 1) + 1] := ((t2 shl 6) or (t2 shr 26));
  end;
end;

procedure EncryptDes_New(const Indata; var Outdata; Size: Longint; const Key: string);
var
  KeyData: array[0..31] of DWord;
  Chain: array[0..BS - 1] of Byte;

  Key64: array[0..7] of Byte;
  KeyA: LongWord;
  KeyB: LongWord;

  Buf: PByte;
  Len: Integer;

  a, b, c: DWORD;
  k: Integer;

  l, r, t, u: dword;
  i, J: longint;

  p1, p2: pointer;
begin
{$I VM_TIGER_BLACK_START.inc}
  KeyA := 0;
  KeyB := 0;

  if Length(Key) > 0 then
  begin
    KeyA := $DBC66688;
    KeyA := KeyA xor $FFFFFFFF;
    Buf := @Key[1];

    Len := Length(Key);

    while (Len >= 8) do
    begin
        {一次处理8个再循环来加快处理速度}
      KeyA := crc_table[(KeyA xor Cardinal(Buf^)) and $FF] xor (KeyA shr 8);
      Inc(Buf);

      KeyA := crc_table[(KeyA xor Cardinal(Buf^)) and $FF] xor (KeyA shr 8);
      Inc(Buf);

      KeyA := crc_table[(KeyA xor Cardinal(Buf^)) and $FF] xor (KeyA shr 8);
      Inc(Buf);

      KeyA := crc_table[(KeyA xor Cardinal(Buf^)) and $FF] xor (KeyA shr 8);
      Inc(Buf);

      KeyA := crc_table[(KeyA xor Cardinal(Buf^)) and $FF] xor (KeyA shr 8);
      Inc(Buf);

      KeyA := crc_table[(KeyA xor Cardinal(Buf^)) and $FF] xor (KeyA shr 8);
      Inc(Buf);

      KeyA := crc_table[(KeyA xor Cardinal(Buf^)) and $FF] xor (KeyA shr 8);
      Inc(Buf);

      KeyA := crc_table[(KeyA xor Cardinal(Buf^)) and $FF] xor (KeyA shr 8);
      Inc(Buf);

      Dec(len, 8);
    end;

    {剩余的不足8个字节，则单个处理}
    if (Len <> 0) then
    begin
      repeat
        KeyA := crc_table[(KeyA xor Cardinal(Buf^)) and $FF] xor (KeyA shr 8);
        Inc(Buf);

        Dec(Len);
      until (Len = 0);
    end;

    KeyA := KeyA xor $FFFFFFFF;

    Len := Length(Key);
    k := 1;

    a := $8f36793F;     // 9E3779B9
    b := $2d175fc2;     // 9E3779B9
    c := $fe2d624a;     // E6359A60

    while len >= 12 do
    begin
      a := a + DWORD(Ord(Key[k + 0]) + (Ord(Key[k + 1]) shl 8) + (Ord(Key[k + 2])  shl 16) + (Ord(Key[k + 3])  shl 24));
      b := b + DWORD(Ord(Key[k + 5]) + (Ord(Key[k + 4]) shl 8) + (Ord(Key[k + 6])  shl 16) + (Ord(Key[k + 7])  shl 24));
      c := c + DWORD(Ord(Key[k + 8]) + (Ord(Key[k + 9]) shl 8) + (Ord(Key[k + 10]) shl 16) + (Ord(Key[k + 11]) shl 24));

      // a -= b; a -= c; a ^= c >> 13;
      a := a - b;   a := a - c;   a := a xor (c shr 13);

      // b -= c; b -= a; b ^= a << 8;
      b := b - c;   b := b - a;   b := b xor (a shl 8);

      // c -= a; c -= b; c ^= b >> 13;
      c := c - a;   c := c - b;   c := c xor (b shr 11);

      // a -= b; a -= c; a ^= c >> 12;
      a := a - b;   a := a - c;   a := a xor (c shr 12);

      // b -= c; b -= a; b ^= a << 16;
      b := b - c;   b := b - a;   b := b xor (a shl 17);

      // c -= a; c -= b; c ^= b >> 5;
      c := c - a;   c := c - b;   c := c xor (b shr 5);

      // a -= b; a -= c; a ^= c >> 3;
      a := a - b;   a := a - c;   a := a xor (c shr 3);

      // b -= c; b -= a; b ^= a << 10;
      b := b - c;   b := b - a;   b := b xor (a shl 10);

      // c -= a; c -= b; c ^= b >> 15;
      c := c - a;   c := c - b;   c := c xor (b shr 15);

      Inc(k, 12);
      Dec(len, 12);
    end;

    c := c + DWORD(Length(Key));

    if len >= 11 then c := c + DWORD(Ord(Key[k + 10]) shl 24);
    if len >= 10 then c := c + DWORD(Ord(Key[k + 9])  shl 16);
    if len >= 9  then c := c + DWORD(Ord(Key[k + 8])  shl 8);

    if len >= 8  then b := b + DWORD(Ord(Key[k + 7])  shl 24);
    if len >= 7  then b := b + DWORD(Ord(Key[k + 6])  shl 16);
    if len >= 6  then b := b + DWORD(Ord(Key[k + 4])  shl 8);
    if len >= 5  then b := b + DWORD(Ord(Key[k + 5]));

    if len >= 4  then a := a + DWORD(Ord(Key[k + 3])  shl 24);
    if len >= 3  then a := a + DWORD(Ord(Key[k + 2])  shl 16);
    if len >= 2  then a := a + DWORD(Ord(Key[k + 1])  shl 8);
    if len >= 1  then a := a + DWORD(Ord(Key[k + 0]));

    // a -= b; a -= c; a ^= c >> 13;
    a := a - b;   a := a - c;   a := a xor (c shr 13);

    // b -= c; b -= a; b ^= a << 8;
    b := b - c;   b := b - a;   b := b xor (a shl 8);

    // c -= a; c -= b; c ^= b >> 13;
    c := c - a;   c := c - b;   c := c xor (b shr 11);

    // a -= b; a -= c; a ^= c >> 12;
    a := a - b;   a := a - c;   a := a xor (c shr 12);

    // b -= c; b -= a; b ^= a << 16;
    b := b - c;   b := b - a;   b := b xor (a shl 17);

    // c -= a; c -= b; c ^= b >> 5;
    c := c - a;   c := c - b;   c := c xor (b shr 5);

    // a -= b; a -= c; a ^= c >> 3;
    a := a - b;   a := a - c;   a := a xor (c shr 3);

    // b -= c; b -= a; b ^= a << 10;
    b := b - c;   b := b - a;   b := b xor (a shl 10);

    // c -= a; c -= b; c ^= b >> 15;
    c := c - a;   c := c - b;   c := c xor (b shr 15);

    KeyB := C;

    KeyA := KeyA xor KeyB;
  end;

  // 得到64bit的key,并初始化KeyData
  Move(KeyA, Key64[0], SizeOf(KeyA));
  Move(KeyB, Key64[4], SizeOf(KeyB));
  DoInit(@KeyB, @KeyData);

  // --------------------------------------------------------------------------
  FillChar(Chain, BS, $8F);
  r := PDword(@Chain)^;
  l := PDword(dword(@Chain) + 4)^;
  t := ((l shr 4) xor r) and $0F0F0F0F;
  r := r xor t;
  l := l xor (t shl 4);
  t := ((r shr 16) xor l) and $0000FFFF;
  l := l xor t;
  r := r xor (t shl 16);
  t := ((l shr 2) xor r) and $33333333;
  r := r xor t;
  l := l xor (t shl 2);
  t := ((r shr 8) xor l) and $00FF00FF;
  l := l xor t;
  r := r xor (t shl 8);
  t := ((l shr 1) xor r) and $55555555;
  r := r xor t;
  l := l xor (t shl 1);
  r := (r shr 29) or (r shl 3);
  l := (l shr 29) or (l shl 3);
  i := 0;
  while i < 32 do
  begin
    u := r xor KeyData[i];
    t := r xor KeyData[i + 1];
    t := (t shr 4) or (t shl 28);
    l := l xor des_SPtrans[0, (u shr 2) and $3F] xor
      des_SPtrans[2, (u shr 10) and $3F] xor
      des_SPtrans[4, (u shr 18) and $3F] xor
      des_SPtrans[6, (u shr 26) and $3F] xor
      des_SPtrans[1, (t shr 2) and $3F] xor
      des_SPtrans[3, (t shr 10) and $3F] xor
      des_SPtrans[5, (t shr 18) and $3F] xor
      des_SPtrans[7, (t shr 26) and $3F];
    u := l xor KeyData[i + 2];
    t := l xor KeyData[i + 3];
    t := (t shr 4) or (t shl 28);
    r := r xor des_SPtrans[0, (u shr 2) and $3F] xor
      des_SPtrans[2, (u shr 10) and $3F] xor
      des_SPtrans[4, (u shr 18) and $3F] xor
      des_SPtrans[6, (u shr 26) and $3F] xor
      des_SPtrans[1, (t shr 2) and $3F] xor
      des_SPtrans[3, (t shr 10) and $3F] xor
      des_SPtrans[5, (t shr 18) and $3F] xor
      des_SPtrans[7, (t shr 26) and $3F];
    u := r xor KeyData[i + 4];
    t := r xor KeyData[i + 5];
    t := (t shr 4) or (t shl 28);
    l := l xor des_SPtrans[0, (u shr 2) and $3F] xor
      des_SPtrans[2, (u shr 10) and $3F] xor
      des_SPtrans[4, (u shr 18) and $3F] xor
      des_SPtrans[6, (u shr 26) and $3F] xor
      des_SPtrans[1, (t shr 2) and $3F] xor
      des_SPtrans[3, (t shr 10) and $3F] xor
      des_SPtrans[5, (t shr 18) and $3F] xor
      des_SPtrans[7, (t shr 26) and $3F];
    u := l xor KeyData[i + 6];
    t := l xor KeyData[i + 7];
    t := (t shr 4) or (t shl 28);
    r := r xor des_SPtrans[0, (u shr 2) and $3F] xor
      des_SPtrans[2, (u shr 10) and $3F] xor
      des_SPtrans[4, (u shr 18) and $3F] xor
      des_SPtrans[6, (u shr 26) and $3F] xor
      des_SPtrans[1, (t shr 2) and $3F] xor
      des_SPtrans[3, (t shr 10) and $3F] xor
      des_SPtrans[5, (t shr 18) and $3F] xor
      des_SPtrans[7, (t shr 26) and $3F];
    Inc(i, 8);
  end;
  r := (r shr 3) or (r shl 29);
  l := (l shr 3) or (l shl 29);
  t := ((r shr 1) xor l) and $55555555;
  l := l xor t;
  r := r xor (t shl 1);
  t := ((l shr 8) xor r) and $00FF00FF;
  r := r xor t;
  l := l xor (t shl 8);
  t := ((r shr 2) xor l) and $33333333;
  l := l xor t;
  r := r xor (t shl 2);
  t := ((l shr 16) xor r) and $0000FFFF;
  r := r xor t;
  l := l xor (t shl 16);
  t := ((r shr 4) xor l) and $0F0F0F0F;
  l := l xor t;
  r := r xor (t shl 4);
  PDword(@Chain)^ := l;
  PDword(dword(@Chain) + 4)^ := r;

  // ------------------------------------------------------------------------------------
  // 初始化密码等数据就可以开始加密了

  p1 := @Indata;
  p2 := @Outdata;
  for i := 1 to (Size div BS) do
  begin
    Move(p1^, p2^, BS);
    XorBlock(p2^, Chain, BS);

    //EncryptBlock(p2^, p2^, @KeyData);

    r := PDword(P2)^;
    l := PDword(dword(P2) + 4)^;
    t := ((l shr 4) xor r) and $0F0F0F0F;
    r := r xor t;
    l := l xor (t shl 4);
    t := ((r shr 16) xor l) and $0000FFFF;
    l := l xor t;
    r := r xor (t shl 16);
    t := ((l shr 2) xor r) and $33333331;
    r := r xor t;
    l := l xor (t shl 2);
    t := ((r shr 8) xor l) and $00FF00FF;
    l := l xor t;
    r := r xor (t shl 8);
    t := ((l shr 1) xor r) and $55555555;
    r := r xor t;
    l := l xor (t shl 1);
    r := (r shr 29) or (r shl 3);
    l := (l shr 29) or (l shl 3);
    j := 0;
    while j < 32 do
    begin
      u := r xor KeyData[j];
      t := r xor KeyData[j + 1];
      t := (t shr 4) or (t shl 28);
      l := l xor des_SPtrans[0, (u shr 2) and $3F] xor
        des_SPtrans[2, (u shr 10) and $3F] xor
        des_SPtrans[4, (u shr 18) and $3F] xor
        des_SPtrans[6, (u shr 26) and $3F] xor
        des_SPtrans[1, (t shr 2) and $3F] xor
        des_SPtrans[3, (t shr 10) and $3F] xor
        des_SPtrans[5, (t shr 18) and $3F] xor
        des_SPtrans[7, (t shr 26) and $3F];
      u := l xor KeyData[j + 2];
      t := l xor KeyData[j + 3];
      t := (t shr 4) or (t shl 28);
      r := r xor des_SPtrans[0, (u shr 2) and $3F] xor
        des_SPtrans[2, (u shr 10) and $3F] xor
        des_SPtrans[4, (u shr 18) and $3F] xor
        des_SPtrans[6, (u shr 26) and $3F] xor
        des_SPtrans[1, (t shr 2) and $3F] xor
        des_SPtrans[3, (t shr 10) and $3F] xor
        des_SPtrans[5, (t shr 18) and $3F] xor
        des_SPtrans[7, (t shr 26) and $3F];
      u := r xor KeyData[j + 4];
      t := r xor KeyData[j + 5];
      t := (t shr 4) or (t shl 28);
      l := l xor des_SPtrans[0, (u shr 2) and $3F] xor
        des_SPtrans[2, (u shr 10) and $3F] xor
        des_SPtrans[4, (u shr 18) and $3F] xor
        des_SPtrans[6, (u shr 26) and $3F] xor
        des_SPtrans[1, (t shr 2) and $3F] xor
        des_SPtrans[3, (t shr 10) and $3F] xor
        des_SPtrans[5, (t shr 18) and $3F] xor
        des_SPtrans[7, (t shr 26) and $3F];
      u := l xor KeyData[j + 6];
      t := l xor KeyData[j + 7];
      t := (t shr 4) or (t shl 28);
      r := r xor des_SPtrans[0, (u shr 2) and $3F] xor
        des_SPtrans[2, (u shr 10) and $3F] xor
        des_SPtrans[4, (u shr 18) and $3F] xor
        des_SPtrans[6, (u shr 26) and $3F] xor
        des_SPtrans[1, (t shr 2) and $3F] xor
        des_SPtrans[3, (t shr 10) and $3F] xor
        des_SPtrans[5, (t shr 18) and $3F] xor
        des_SPtrans[7, (t shr 26) and $3F];
      Inc(j, 8);
    end;
    r := (r shr 3) or (r shl 29);
    l := (l shr 3) or (l shl 29);
    t := ((r shr 1) xor l) and $55555555;
    l := l xor t;
    r := r xor (t shl 1);
    t := ((l shr 8) xor r) and $00FF00FF;
    r := r xor t;
    l := l xor (t shl 8);
    t := ((r shr 2) xor l) and $33333331;
    l := l xor t;
    r := r xor (t shl 2);
    t := ((l shr 16) xor r) and $0000FFFF;
    r := r xor t;
    l := l xor (t shl 16);
    t := ((r shr 4) xor l) and $0F0F0F0F;
    l := l xor t;
    r := r xor (t shl 4);
    PDword(P2)^ := l;
    PDword(dword(P2) + 4)^ := r;

    Move(p2^, Chain, BS);
    p1 := pointer(longint(p1) + BS);
    p2 := pointer(longint(p2) + BS);
  end;

  if (Size mod BS) <> 0 then
  begin
    // EncryptBlock(Chain, Chain, @KeyData);

    r := PDword(@Chain)^;
    l := PDword(dword(@Chain) + 4)^;
    t := ((l shr 4) xor r) and $0F0F0F0F;
    r := r xor t;
    l := l xor (t shl 4);
    t := ((r shr 16) xor l) and $0000FFFF;
    l := l xor t;
    r := r xor (t shl 16);
    t := ((l shr 2) xor r) and $33333331;
    r := r xor t;
    l := l xor (t shl 2);
    t := ((r shr 8) xor l) and $00FF00FF;
    l := l xor t;
    r := r xor (t shl 8);
    t := ((l shr 1) xor r) and $55555555;
    r := r xor t;
    l := l xor (t shl 1);
    r := (r shr 29) or (r shl 3);
    l := (l shr 29) or (l shl 3);
    i := 0;
    while i < 32 do
    begin
      u := r xor KeyData[i];
      t := r xor KeyData[i + 1];
      t := (t shr 4) or (t shl 28);
      l := l xor des_SPtrans[0, (u shr 2) and $3F] xor
        des_SPtrans[2, (u shr 10) and $3F] xor
        des_SPtrans[4, (u shr 18) and $3F] xor
        des_SPtrans[6, (u shr 26) and $3F] xor
        des_SPtrans[1, (t shr 2) and $3F] xor
        des_SPtrans[3, (t shr 10) and $3F] xor
        des_SPtrans[5, (t shr 18) and $3F] xor
        des_SPtrans[7, (t shr 26) and $3F];
      u := l xor KeyData[i + 2];
      t := l xor KeyData[i + 3];
      t := (t shr 4) or (t shl 28);
      r := r xor des_SPtrans[0, (u shr 2) and $3F] xor
        des_SPtrans[2, (u shr 10) and $3F] xor
        des_SPtrans[4, (u shr 18) and $3F] xor
        des_SPtrans[6, (u shr 26) and $3F] xor
        des_SPtrans[1, (t shr 2) and $3F] xor
        des_SPtrans[3, (t shr 10) and $3F] xor
        des_SPtrans[5, (t shr 18) and $3F] xor
        des_SPtrans[7, (t shr 26) and $3F];
      u := r xor KeyData[i + 4];
      t := r xor KeyData[i + 5];
      t := (t shr 4) or (t shl 28);
      l := l xor des_SPtrans[0, (u shr 2) and $3F] xor
        des_SPtrans[2, (u shr 10) and $3F] xor
        des_SPtrans[4, (u shr 18) and $3F] xor
        des_SPtrans[6, (u shr 26) and $3F] xor
        des_SPtrans[1, (t shr 2) and $3F] xor
        des_SPtrans[3, (t shr 10) and $3F] xor
        des_SPtrans[5, (t shr 18) and $3F] xor
        des_SPtrans[7, (t shr 26) and $3F];
      u := l xor KeyData[i + 6];
      t := l xor KeyData[i + 7];
      t := (t shr 4) or (t shl 28);
      r := r xor des_SPtrans[0, (u shr 2) and $3F] xor
        des_SPtrans[2, (u shr 10) and $3F] xor
        des_SPtrans[4, (u shr 18) and $3F] xor
        des_SPtrans[6, (u shr 26) and $3F] xor
        des_SPtrans[1, (t shr 2) and $3F] xor
        des_SPtrans[3, (t shr 10) and $3F] xor
        des_SPtrans[5, (t shr 18) and $3F] xor
        des_SPtrans[7, (t shr 26) and $3F];
      Inc(i, 8);
    end;
    r := (r shr 3) or (r shl 29);
    l := (l shr 3) or (l shl 29);
    t := ((r shr 1) xor l) and $55555555;
    l := l xor t;
    r := r xor (t shl 1);
    t := ((l shr 8) xor r) and $00FF00FF;
    r := r xor t;
    l := l xor (t shl 8);
    t := ((r shr 2) xor l) and $33333331;
    l := l xor t;
    r := r xor (t shl 2);
    t := ((l shr 16) xor r) and $0000FFFF;
    r := r xor t;
    l := l xor (t shl 16);
    t := ((r shr 4) xor l) and $0F0F0F0F;
    l := l xor t;
    r := r xor (t shl 4);
    PDword(@Chain)^ := l;
    PDword(dword(@Chain) + 4)^ := r;

    Move(p1^, p2^, Size mod BS);
    XorBlock(p2^, Chain, Size mod BS);
  end;

{$I VM_TIGER_BLACK_END.inc}
end;

procedure DecryptDes_New(const Indata; var Outdata; Size: Longint; const Key: string);
var
  KeyData: array[0..31] of DWord;
  Chain: array[0..BS - 1] of Byte;

  Key64: array[0..7] of Byte;
  KeyA: LongWord;
  KeyB: LongWord;

  Buf: PByte;
  Len: Integer;

  a, b, c: DWORD;
  k: Integer;

  l, r, t, u: dword;
  i, j: longint;

  p1, p2: pointer;
  Temp: array[0..BS - 1] of Byte;
begin
{$I VM_TIGER_BLACK_START.inc}
  KeyA := 0;
  KeyB := 0;

  if Length(Key) > 0 then
  begin
    KeyA := $DBC66688;
    KeyA := KeyA xor $FFFFFFFF;
    Buf := @Key[1];

    Len := Length(Key);

    while (Len >= 8) do
    begin
        {一次处理8个再循环来加快处理速度}
      KeyA := crc_table[(KeyA xor Cardinal(Buf^)) and $FF] xor (KeyA shr 8);
      Inc(Buf);

      KeyA := crc_table[(KeyA xor Cardinal(Buf^)) and $FF] xor (KeyA shr 8);
      Inc(Buf);

      KeyA := crc_table[(KeyA xor Cardinal(Buf^)) and $FF] xor (KeyA shr 8);
      Inc(Buf);

      KeyA := crc_table[(KeyA xor Cardinal(Buf^)) and $FF] xor (KeyA shr 8);
      Inc(Buf);

      KeyA := crc_table[(KeyA xor Cardinal(Buf^)) and $FF] xor (KeyA shr 8);
      Inc(Buf);

      KeyA := crc_table[(KeyA xor Cardinal(Buf^)) and $FF] xor (KeyA shr 8);
      Inc(Buf);

      KeyA := crc_table[(KeyA xor Cardinal(Buf^)) and $FF] xor (KeyA shr 8);
      Inc(Buf);

      KeyA := crc_table[(KeyA xor Cardinal(Buf^)) and $FF] xor (KeyA shr 8);
      Inc(Buf);

      Dec(len, 8);
    end;

    {剩余的不足8个字节，则单个处理}
    if (Len <> 0) then
    begin
      repeat
        KeyA := crc_table[(KeyA xor Cardinal(Buf^)) and $FF] xor (KeyA shr 8);
        Inc(Buf);

        Dec(Len);
      until (Len = 0);
    end;

    KeyA := KeyA xor $FFFFFFFF;

    Len := Length(Key);
    k := 1;

    a := $8f36793F;     // 9E3779B9
    b := $2d175fc2;     // 9E3779B9
    c := $fe2d624a;     // E6359A60

    while len >= 12 do
    begin
      a := a + DWORD(Ord(Key[k + 0]) + (Ord(Key[k + 1]) shl 8) + (Ord(Key[k + 2])  shl 16) + (Ord(Key[k + 3])  shl 24));
      b := b + DWORD(Ord(Key[k + 5]) + (Ord(Key[k + 4]) shl 8) + (Ord(Key[k + 6])  shl 16) + (Ord(Key[k + 7])  shl 24));
      c := c + DWORD(Ord(Key[k + 8]) + (Ord(Key[k + 9]) shl 8) + (Ord(Key[k + 10]) shl 16) + (Ord(Key[k + 11]) shl 24));

      // a -= b; a -= c; a ^= c >> 13;
      a := a - b;   a := a - c;   a := a xor (c shr 13);

      // b -= c; b -= a; b ^= a << 8;
      b := b - c;   b := b - a;   b := b xor (a shl 8);

      // c -= a; c -= b; c ^= b >> 13;
      c := c - a;   c := c - b;   c := c xor (b shr 11);

      // a -= b; a -= c; a ^= c >> 12;
      a := a - b;   a := a - c;   a := a xor (c shr 12);

      // b -= c; b -= a; b ^= a << 16;
      b := b - c;   b := b - a;   b := b xor (a shl 17);

      // c -= a; c -= b; c ^= b >> 5;
      c := c - a;   c := c - b;   c := c xor (b shr 5);

      // a -= b; a -= c; a ^= c >> 3;
      a := a - b;   a := a - c;   a := a xor (c shr 3);

      // b -= c; b -= a; b ^= a << 10;
      b := b - c;   b := b - a;   b := b xor (a shl 10);

      // c -= a; c -= b; c ^= b >> 15;
      c := c - a;   c := c - b;   c := c xor (b shr 15);

      Inc(k, 12);
      Dec(len, 12);
    end;

    c := c + DWORD(Length(Key));

    if len >= 11 then c := c + DWORD(Ord(Key[k + 10]) shl 24);
    if len >= 10 then c := c + DWORD(Ord(Key[k + 9])  shl 16);
    if len >= 9  then c := c + DWORD(Ord(Key[k + 8])  shl 8);

    if len >= 8  then b := b + DWORD(Ord(Key[k + 7])  shl 24);
    if len >= 7  then b := b + DWORD(Ord(Key[k + 6])  shl 16);
    if len >= 6  then b := b + DWORD(Ord(Key[k + 4])  shl 8);
    if len >= 5  then b := b + DWORD(Ord(Key[k + 5]));

    if len >= 4  then a := a + DWORD(Ord(Key[k + 3])  shl 24);
    if len >= 3  then a := a + DWORD(Ord(Key[k + 2])  shl 16);
    if len >= 2  then a := a + DWORD(Ord(Key[k + 1])  shl 8);
    if len >= 1  then a := a + DWORD(Ord(Key[k + 0]));

    // a -= b; a -= c; a ^= c >> 13;
    a := a - b;   a := a - c;   a := a xor (c shr 13);

    // b -= c; b -= a; b ^= a << 8;
    b := b - c;   b := b - a;   b := b xor (a shl 8);

    // c -= a; c -= b; c ^= b >> 13;
    c := c - a;   c := c - b;   c := c xor (b shr 11);

    // a -= b; a -= c; a ^= c >> 12;
    a := a - b;   a := a - c;   a := a xor (c shr 12);

    // b -= c; b -= a; b ^= a << 16;
    b := b - c;   b := b - a;   b := b xor (a shl 17);

    // c -= a; c -= b; c ^= b >> 5;
    c := c - a;   c := c - b;   c := c xor (b shr 5);

    // a -= b; a -= c; a ^= c >> 3;
    a := a - b;   a := a - c;   a := a xor (c shr 3);

    // b -= c; b -= a; b ^= a << 10;
    b := b - c;   b := b - a;   b := b xor (a shl 10);

    // c -= a; c -= b; c ^= b >> 15;
    c := c - a;   c := c - b;   c := c xor (b shr 15);

    KeyB := C;

    KeyA := KeyA xor KeyB;
  end;

  // 得到64bit的key,并初始化KeyData
  Move(KeyA, Key64[0], SizeOf(KeyA));
  Move(KeyB, Key64[4], SizeOf(KeyB));
  DoInit(@KeyB, @KeyData);

  // --------------------------------------------------------------------------
  FillChar(Chain, BS, $8F);
  r := PDword(@Chain)^;
  l := PDword(dword(@Chain) + 4)^;
  t := ((l shr 4) xor r) and $0F0F0F0F;
  r := r xor t;
  l := l xor (t shl 4);
  t := ((r shr 16) xor l) and $0000FFFF;
  l := l xor t;
  r := r xor (t shl 16);
  t := ((l shr 2) xor r) and $33333333;
  r := r xor t;
  l := l xor (t shl 2);
  t := ((r shr 8) xor l) and $00FF00FF;
  l := l xor t;
  r := r xor (t shl 8);
  t := ((l shr 1) xor r) and $55555555;
  r := r xor t;
  l := l xor (t shl 1);
  r := (r shr 29) or (r shl 3);
  l := (l shr 29) or (l shl 3);
  i := 0;
  while i < 32 do
  begin
    u := r xor KeyData[i];
    t := r xor KeyData[i + 1];
    t := (t shr 4) or (t shl 28);
    l := l xor des_SPtrans[0, (u shr 2) and $3F] xor
      des_SPtrans[2, (u shr 10) and $3F] xor
      des_SPtrans[4, (u shr 18) and $3F] xor
      des_SPtrans[6, (u shr 26) and $3F] xor
      des_SPtrans[1, (t shr 2) and $3F] xor
      des_SPtrans[3, (t shr 10) and $3F] xor
      des_SPtrans[5, (t shr 18) and $3F] xor
      des_SPtrans[7, (t shr 26) and $3F];
    u := l xor KeyData[i + 2];
    t := l xor KeyData[i + 3];
    t := (t shr 4) or (t shl 28);
    r := r xor des_SPtrans[0, (u shr 2) and $3F] xor
      des_SPtrans[2, (u shr 10) and $3F] xor
      des_SPtrans[4, (u shr 18) and $3F] xor
      des_SPtrans[6, (u shr 26) and $3F] xor
      des_SPtrans[1, (t shr 2) and $3F] xor
      des_SPtrans[3, (t shr 10) and $3F] xor
      des_SPtrans[5, (t shr 18) and $3F] xor
      des_SPtrans[7, (t shr 26) and $3F];
    u := r xor KeyData[i + 4];
    t := r xor KeyData[i + 5];
    t := (t shr 4) or (t shl 28);
    l := l xor des_SPtrans[0, (u shr 2) and $3F] xor
      des_SPtrans[2, (u shr 10) and $3F] xor
      des_SPtrans[4, (u shr 18) and $3F] xor
      des_SPtrans[6, (u shr 26) and $3F] xor
      des_SPtrans[1, (t shr 2) and $3F] xor
      des_SPtrans[3, (t shr 10) and $3F] xor
      des_SPtrans[5, (t shr 18) and $3F] xor
      des_SPtrans[7, (t shr 26) and $3F];
    u := l xor KeyData[i + 6];
    t := l xor KeyData[i + 7];
    t := (t shr 4) or (t shl 28);
    r := r xor des_SPtrans[0, (u shr 2) and $3F] xor
      des_SPtrans[2, (u shr 10) and $3F] xor
      des_SPtrans[4, (u shr 18) and $3F] xor
      des_SPtrans[6, (u shr 26) and $3F] xor
      des_SPtrans[1, (t shr 2) and $3F] xor
      des_SPtrans[3, (t shr 10) and $3F] xor
      des_SPtrans[5, (t shr 18) and $3F] xor
      des_SPtrans[7, (t shr 26) and $3F];
    Inc(i, 8);
  end;
  r := (r shr 3) or (r shl 29);
  l := (l shr 3) or (l shl 29);
  t := ((r shr 1) xor l) and $55555555;
  l := l xor t;
  r := r xor (t shl 1);
  t := ((l shr 8) xor r) and $00FF00FF;
  r := r xor t;
  l := l xor (t shl 8);
  t := ((r shr 2) xor l) and $33333333;
  l := l xor t;
  r := r xor (t shl 2);
  t := ((l shr 16) xor r) and $0000FFFF;
  r := r xor t;
  l := l xor (t shl 16);
  t := ((r shr 4) xor l) and $0F0F0F0F;
  l := l xor t;
  r := r xor (t shl 4);
  PDword(@Chain)^ := l;
  PDword(dword(@Chain) + 4)^ := r;

  // ------------------------------------------------------------------------------------
  // 初始化密码等数据就可以开始解密了

  p1 := @Indata;
  p2 := @Outdata;
  for i := 1 to (Size div BS) do
  begin
    Move(p1^, p2^, BS);
    Move(p1^, Temp, BS);

    //DecryptBlock(p2^, p2^, @KeyData);

    r := PDword(p2)^;
    l := PDword(dword(p2) + 4)^;
    t := ((l shr 4) xor r) and $0F0F0F0F;
    r := r xor t;
    l := l xor (t shl 4);
    t := ((r shr 16) xor l) and $0000FFFF;
    l := l xor t;
    r := r xor (t shl 16);
    t := ((l shr 2) xor r) and $33333331;
    r := r xor t;
    l := l xor (t shl 2);
    t := ((r shr 8) xor l) and $00FF00FF;
    l := l xor t;
    r := r xor (t shl 8);
    t := ((l shr 1) xor r) and $55555555;
    r := r xor t;
    l := l xor (t shl 1);
    r := (r shr 29) or (r shl 3);
    l := (l shr 29) or (l shl 3);
    j := 30;
    while j > 0 do
    begin
      u := r xor KeyData[j];
      t := r xor KeyData[j + 1];
      t := (t shr 4) or (t shl 28);
      l := l xor des_SPtrans[0, (u shr 2) and $3F] xor
        des_SPtrans[2, (u shr 10) and $3F] xor
        des_SPtrans[4, (u shr 18) and $3F] xor
        des_SPtrans[6, (u shr 26) and $3F] xor
        des_SPtrans[1, (t shr 2) and $3F] xor
        des_SPtrans[3, (t shr 10) and $3F] xor
        des_SPtrans[5, (t shr 18) and $3F] xor
        des_SPtrans[7, (t shr 26) and $3F];
      u := l xor KeyData[j - 2];
      t := l xor KeyData[j - 1];
      t := (t shr 4) or (t shl 28);
      r := r xor des_SPtrans[0, (u shr 2) and $3F] xor
        des_SPtrans[2, (u shr 10) and $3F] xor
        des_SPtrans[4, (u shr 18) and $3F] xor
        des_SPtrans[6, (u shr 26) and $3F] xor
        des_SPtrans[1, (t shr 2) and $3F] xor
        des_SPtrans[3, (t shr 10) and $3F] xor
        des_SPtrans[5, (t shr 18) and $3F] xor
        des_SPtrans[7, (t shr 26) and $3F];
      u := r xor KeyData[j - 4];
      t := r xor KeyData[j - 3];
      t := (t shr 4) or (t shl 28);
      l := l xor des_SPtrans[0, (u shr 2) and $3F] xor
        des_SPtrans[2, (u shr 10) and $3F] xor
        des_SPtrans[4, (u shr 18) and $3F] xor
        des_SPtrans[6, (u shr 26) and $3F] xor
        des_SPtrans[1, (t shr 2) and $3F] xor
        des_SPtrans[3, (t shr 10) and $3F] xor
        des_SPtrans[5, (t shr 18) and $3F] xor
        des_SPtrans[7, (t shr 26) and $3F];
      u := l xor KeyData[j - 6];
      t := l xor KeyData[j - 5];
      t := (t shr 4) or (t shl 28);
      r := r xor des_SPtrans[0, (u shr 2) and $3F] xor
        des_SPtrans[2, (u shr 10) and $3F] xor
        des_SPtrans[4, (u shr 18) and $3F] xor
        des_SPtrans[6, (u shr 26) and $3F] xor
        des_SPtrans[1, (t shr 2) and $3F] xor
        des_SPtrans[3, (t shr 10) and $3F] xor
        des_SPtrans[5, (t shr 18) and $3F] xor
        des_SPtrans[7, (t shr 26) and $3F];
      Dec(j, 8);
    end;
    r := (r shr 3) or (r shl 29);
    l := (l shr 3) or (l shl 29);
    t := ((r shr 1) xor l) and $55555555;
    l := l xor t;
    r := r xor (t shl 1);
    t := ((l shr 8) xor r) and $00FF00FF;
    r := r xor t;
    l := l xor (t shl 8);
    t := ((r shr 2) xor l) and $33333331;
    l := l xor t;
    r := r xor (t shl 2);
    t := ((l shr 16) xor r) and $0000FFFF;
    r := r xor t;
    l := l xor (t shl 16);
    t := ((r shr 4) xor l) and $0F0F0F0F;
    l := l xor t;
    r := r xor (t shl 4);
    PDword(p2)^ := l;
    PDword(dword(p2) + 4)^ := r;

    XorBlock(p2^, Chain, BS);
    Move(Temp, Chain, BS);
    p1 := pointer(longint(p1) + BS);
    p2 := pointer(longint(p2) + BS);
  end;
  if (Size mod BS) <> 0 then
  begin
    //EncryptBlock(Chain, Chain, @KeyData);

    r := PDword(@Chain)^;
    l := PDword(dword(@Chain) + 4)^;
    t := ((l shr 4) xor r) and $0F0F0F0F;
    r := r xor t;
    l := l xor (t shl 4);
    t := ((r shr 16) xor l) and $0000FFFF;
    l := l xor t;
    r := r xor (t shl 16);
    t := ((l shr 2) xor r) and $33333331;
    r := r xor t;
    l := l xor (t shl 2);
    t := ((r shr 8) xor l) and $00FF00FF;
    l := l xor t;
    r := r xor (t shl 8);
    t := ((l shr 1) xor r) and $55555555;
    r := r xor t;
    l := l xor (t shl 1);
    r := (r shr 29) or (r shl 3);
    l := (l shr 29) or (l shl 3);
    i := 0;
    while i < 32 do
    begin
      u := r xor KeyData[i];
      t := r xor KeyData[i + 1];
      t := (t shr 4) or (t shl 28);
      l := l xor des_SPtrans[0, (u shr 2) and $3F] xor
        des_SPtrans[2, (u shr 10) and $3F] xor
        des_SPtrans[4, (u shr 18) and $3F] xor
        des_SPtrans[6, (u shr 26) and $3F] xor
        des_SPtrans[1, (t shr 2) and $3F] xor
        des_SPtrans[3, (t shr 10) and $3F] xor
        des_SPtrans[5, (t shr 18) and $3F] xor
        des_SPtrans[7, (t shr 26) and $3F];
      u := l xor KeyData[i + 2];
      t := l xor KeyData[i + 3];
      t := (t shr 4) or (t shl 28);
      r := r xor des_SPtrans[0, (u shr 2) and $3F] xor
        des_SPtrans[2, (u shr 10) and $3F] xor
        des_SPtrans[4, (u shr 18) and $3F] xor
        des_SPtrans[6, (u shr 26) and $3F] xor
        des_SPtrans[1, (t shr 2) and $3F] xor
        des_SPtrans[3, (t shr 10) and $3F] xor
        des_SPtrans[5, (t shr 18) and $3F] xor
        des_SPtrans[7, (t shr 26) and $3F];
      u := r xor KeyData[i + 4];
      t := r xor KeyData[i + 5];
      t := (t shr 4) or (t shl 28);
      l := l xor des_SPtrans[0, (u shr 2) and $3F] xor
        des_SPtrans[2, (u shr 10) and $3F] xor
        des_SPtrans[4, (u shr 18) and $3F] xor
        des_SPtrans[6, (u shr 26) and $3F] xor
        des_SPtrans[1, (t shr 2) and $3F] xor
        des_SPtrans[3, (t shr 10) and $3F] xor
        des_SPtrans[5, (t shr 18) and $3F] xor
        des_SPtrans[7, (t shr 26) and $3F];
      u := l xor KeyData[i + 6];
      t := l xor KeyData[i + 7];
      t := (t shr 4) or (t shl 28);
      r := r xor des_SPtrans[0, (u shr 2) and $3F] xor
        des_SPtrans[2, (u shr 10) and $3F] xor
        des_SPtrans[4, (u shr 18) and $3F] xor
        des_SPtrans[6, (u shr 26) and $3F] xor
        des_SPtrans[1, (t shr 2) and $3F] xor
        des_SPtrans[3, (t shr 10) and $3F] xor
        des_SPtrans[5, (t shr 18) and $3F] xor
        des_SPtrans[7, (t shr 26) and $3F];
      Inc(i, 8);
    end;
    r := (r shr 3) or (r shl 29);
    l := (l shr 3) or (l shl 29);
    t := ((r shr 1) xor l) and $55555555;
    l := l xor t;
    r := r xor (t shl 1);
    t := ((l shr 8) xor r) and $00FF00FF;
    r := r xor t;
    l := l xor (t shl 8);
    t := ((r shr 2) xor l) and $33333331;
    l := l xor t;
    r := r xor (t shl 2);
    t := ((l shr 16) xor r) and $0000FFFF;
    r := r xor t;
    l := l xor (t shl 16);
    t := ((r shr 4) xor l) and $0F0F0F0F;
    l := l xor t;
    r := r xor (t shl 4);
    PDword(@Chain)^ := l;
    PDword(dword(@Chain) + 4)^ := r;


    Move(p1^, p2^, Size mod BS);
    XorBlock(p2^, Chain, Size mod BS);
  end;

{$I VM_TIGER_BLACK_END.inc}
end;

end.

