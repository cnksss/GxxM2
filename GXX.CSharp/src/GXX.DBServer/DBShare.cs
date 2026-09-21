using System;
using System.Text;
using GXX.Core;
using GXX.Core.Rtl;
using GXX.Core.Util;

namespace GXX.DBServer;

// ============================================================================================
// Source\DBServer\DBShare.pas 的**人物名校验族**（本车道按精确行号移植的部分）→ DBShare.cs
//
//   LoadChrNameList              :403-425   （23 行）
//   CheckDenyChrName             :1043-1056
//   CheckFilterNewHumanChrName   :1058-1077
//   CheckNumberName              :1103-1122
//   CheckLetterName              :1124-1143
//   CheckCanCaseChar             :1177-1202 （CheckChrName 的依赖）
//   CheckChrName                 :1204-1249
//   CheckSpecialChar             :1251-1274
//
// 【参数表示 —— 本文件最容易出错的地方，逐条写清楚】
//   原文 6 个函数的形参类型决定了它按**字节**还是按**字符**工作：
//     · `sChrName: string`（= AnsiString）        ⇒ **字节语义**：CheckDenyChrName / CheckFilterNewHumanChrName /
//                                                    CheckNumberName / CheckLetterName / CheckChrName
//     · `sChrName: WideString`                     ⇒ **UTF-16 语义**：CheckSpecialChar / CheckCanCaseChar
//   托管接缝（`SelectClientDbShareSeam`）已按此约定传参：前者收 **latin-1 字节串**
//   （见 `SelectClientAnsi`），后者收 **GBK 文本**。
//
//   ★ `CheckChrName:1247` 调 `CheckCanCaseChar(sChrName)` 时，AnsiString→WideString 是**隐式 GBK 解码**
//     ⇒ 托管侧必须先把字节串按 GBK 解成文本再传（不是直接把字节串当字符传）。
//   ★ `CheckNumberName:1111` / `CheckLetterName:1132` 也各自做了 `wChrName := sChrName` 的隐式解码，
//     再 `S := wChrName[I]; Chr := S[1]`（WideChar→AnsiString 再取**第一个字节**）——
//     这一来一回等于"按 GBK 解码成字符、再看它的**首字节**"。托管侧照此实现（见两处注释）。
//
// 【与 GXX.Core 的分工】`GetValidStr3` / `ArrestStringEx` / `CompareText` / `UpperCase` 等 RTL 已有实现；
//   但 **`UpperCase` 不能直接用 `DelphiRTL.UpperCase`**，见 `UpperCaseAnsi` 的说明。
// ============================================================================================

/// <summary>DBShare.pas 的人物名校验族（本车道按原文行号 1:1 移植的部分）。</summary>
public static class DBShare
{
    /// <summary>
    /// DBShare.pas:16 `TextChars = [#32..#255];`（被 `SelectClient.NewChr` 用作按字节过滤的判据）。
    /// </summary>
    public const char TextCharsFirst = (char)32;

    /// <summary>DBShare.pas:16 的上界 255。</summary>
    public const char TextCharsLast = (char)255;

    /// <summary>DBShare.pas:21 `MIN_CHAR_NAME_LEN = 4;`</summary>
    public const int MIN_CHAR_NAME_LEN = 4;

    /// <summary>DBShare.pas:22 `MAX_CHAR_NAME_LEN = 14;`</summary>
    public const int MAX_CHAR_NAME_LEN = 14;

    // ==========================================================================================
    // 载入禁用名单
    // ==========================================================================================

    /// <summary>
    /// DBShare.pas:403-425 `function LoadChrNameList(sFileName: string): Boolean;`
    ///
    /// 原文行为：文件不存在 → **False 且不动名单**；存在 → 清空 → `LoadFromFile` →
    /// 用一个 `while True` 循环**就地删除空白行**（`Delete(I)` 后 `Continue` **不递增 I**，
    /// 因为删除后下一行会补到同一位置）→ True。
    ///
    /// ★ 原文对"只有空白字符的行"用 `Trim(...) = ''` 判定，而 `Trim` 在 Delphi 里是 `<= ' '`
    ///   （见 `DelphiRTL.Trim`）⇒ **制表符/控制字符行也算空行**。
    /// </summary>
    public static bool LoadChrNameList(string sFileName)
    {
        bool Result = false;                                                          // :407
        if (System.IO.File.Exists(sFileName))                                         // :408
        {
            DBShareSeam.g_DenyChrNameList.Clear();                                    // :410
            DBShareSeam.g_DenyChrNameList.LoadFromFile(sFileName);                    // :411
            int I = 0;                                                                // :412
            while (true)                                                              // :413
            {
                if (DBShareSeam.g_DenyChrNameList.Count <= I) break;                  // :415
                if (DelphiRTL.Trim(DBShareSeam.g_DenyChrNameList[I]) == "")            // :416
                {
                    DBShareSeam.g_DenyChrNameList.Delete(I);                          // :418（TStrings.Delete(I)）
                    continue;                                                         // :419（注意：**不** Inc(I)）
                }
                I++;                                                                  // :421
            }
            Result = true;                                                            // :423
        }
        return Result;
    }

    // ==========================================================================================
    // 校验族
    // ==========================================================================================

    /// <summary>
    /// DBShare.pas:1043-1056 `function CheckDenyChrName(sChrName: string): Boolean;`
    /// 入参：**latin-1 字节串**（AnsiString）。命中禁用名单（忽略大小写）→ False。
    /// </summary>
    public static bool CheckDenyChrName(string sChrName)
    {
        bool Result = true;                                                           // :1047
        for (int I = 0; I <= DBShareSeam.g_DenyChrNameList.Count - 1; I++)            // :1048
        {
            if (DelphiStrUtils.CompareText(sChrName, DBShareSeam.g_DenyChrNameList[I]) == 0)  // :1050
            {
                Result = false;                                                       // :1052
                break;                                                                // :1053
            }
        }
        return Result;
    }

    /// <summary>
    /// DBShare.pas:1058-1077 `function CheckFilterNewHumanChrName(sChrName: string): Boolean;`
    /// 入参：**latin-1 字节串**。
    ///
    /// 原文行为（顺序关键）：
    /// <list type="number">
    ///   <item>`sChrName := UpperCase(sChrName)`（:1063，**字节串**的大小写折叠 —— 见 <see cref="UpperCaseAnsi"/>）；</item>
    ///   <item>含 `#1`(0x01) 或 `#255`(0xFF) → True（:1065）；</item>
    ///   <item>否则命中过滤表（**两边都先 UpperCase**，子串匹配）→ True（:1070）。</item>
    /// </list>
    /// </summary>
    public static bool CheckFilterNewHumanChrName(string sChrName)
    {
        // 非法字符过滤不区分大小写 2020-05-16
        sChrName = UpperCaseAnsi(sChrName);                                           // :1063

        bool Result = (DelphiRTL.Pos("\u0001", sChrName) > 0)
                   || (DelphiRTL.Pos("\u00FF", sChrName) > 0);                        // :1065
        if (!Result)                                                                  // :1066
        {
            for (int I = 0; I <= DBShareSeam.g_FilterNewHumanNameTextList.Count - 1; I++)   // :1068
            {
                if (DelphiRTL.Pos(UpperCaseAnsi(DBShareSeam.g_FilterNewHumanNameTextList[I]), sChrName) > 0)   // :1070
                {
                    Result = true;                                                    // :1072
                    break;                                                            // :1073
                }
            }
        }
        return Result;
    }

    /// <summary>
    /// DBShare.pas:1103-1122 `function CheckNumberName(sChrName: string): Boolean;`
    /// 入参：**latin-1 字节串**。名字里含 **ASCII 数字** → True（原文是"含数字就返回真"）。
    ///
    /// 原文 :1111 `wChrName := sChrName`（隐式 GBK 解码），再 :1114-1115
    /// `S := wChrName[I]; Chr := S[1]`（WideChar→AnsiString 后取**第一个字节**）。
    /// ⇒ 只有"该字符在 GBK 下编码后的首字节落在 0x30..0x39"才算数字；
    ///   汉字的首字节 ≥0x81，永远不算（GBK 的**尾字节**范围是 0x40-0xFE，也不含数字）。
    /// 托管侧按"GBK 解码后是否含 ASCII 数字"实现 —— 与上式**等价**，且免去逐字符重编码。
    /// </summary>
    public static bool CheckNumberName(string sChrName)
    {
        bool Result = false;                                                          // :1110
        string wChrName = AnsiTextOf(sChrName);                                       // :1111（隐式解码）
        for (int I = 1; I <= wChrName.Length; I++)                                    // :1112
        {
            char Chr = FirstAnsiByteOf(wChrName[I - 1]);                              // :1114-1115
            if (Chr >= '0' && Chr <= '9')                                             // :1116
            {
                Result = true;                                                        // :1118
                return Result;                                                        // :1119 Exit
            }
        }
        return Result;
    }

    /// <summary>
    /// DBShare.pas:1124-1143 `function CheckLetterName(sChrName: string): Boolean;`
    /// 入参：**latin-1 字节串**。名字**全部**是英文字母 → True（空串也 True，循环不执行）。
    ///
    /// 原文 :1132 `wChrName := sChrName`（隐式 GBK 解码），逐字符 :1135-1136
    /// `S := wChrName[I]; Chr := UpperCase(S)[1]`，再判 `Chr in ['A'..'Z']`：
    /// 任一个字符的"GBK 编码首字节"不是 A-Z 就 False。
    /// ⇒ 汉字首字节 ≥0x81 ⇒ False；ASCII 非字母 ⇒ False。
    /// </summary>
    public static bool CheckLetterName(string sChrName)
    {
        bool Result = true;                                                           // :1131
        string wChrName = AnsiTextOf(sChrName);                                       // :1132（隐式解码）
        for (int I = 1; I <= wChrName.Length; I++)                                    // :1133
        {
            // :1135-1136  S := wChrName[I]; Chr := UpperCase(S)[1];
            //   ⇒ 取该字符 GBK 编码后的**首字节**，再对它做 UpperCase（只折 ASCII，见 UpperCaseAnsi）。
            char ansiFirst = FirstAnsiByteOf(wChrName[I - 1]);
            char Chr = (ansiFirst >= 'a' && ansiFirst <= 'z') ? (char)(ansiFirst - 32) : ansiFirst;
            if (!(Chr >= 'A' && Chr <= 'Z'))                                          // :1137
            {
                Result = false;                                                       // :1139
                return Result;                                                        // :1140 Exit
            }
        }
        return Result;
    }

    /// <summary>
    /// DBShare.pas:1177-1202 `function CheckCanCaseChar(sChrName: WideString): Boolean;`
    /// 入参：**GBK 文本**（UTF-16）。名字里出现"有大小写形式的非英文字母"
    /// （希腊/西里尔/罗马数字/全角字母等，见 `FilterChars`）→ False。
    ///
    /// ★ `FilterChars` 是**逐字照抄原文的 WideString 常量**（原文 :1179-1181 两段字符串字面量拼接）。
    /// </summary>
    public static bool CheckCanCaseChar(string sChrName)
    {
        const string FilterChars =
            "ΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩЁАБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯⅠⅡⅢⅣⅤⅥⅦⅧⅨⅩⅪⅫＡＢＣＤＥＦＧＨＩＪＫＬＭＮＯＰＱＲＳＴＵＶＷＸＹＺ" +
            "μńňɑαβγδεζηθικλνξοπρστυφχψωабвгдежзийклмнопрстуфхцчшщъыьэюяёⅰⅱⅲⅳⅴⅵⅶⅷⅸⅹａｂｃｄｅｆｇｈｉｊｋｌｍｎｏｐｑｒｓｔｕｖｗｘｙｚ";   // :1179-1181

        bool Result = true;                                                           // :1186
        for (int I = 1; I <= sChrName.Length; I++)                                    // :1187
        {
            char WC1 = sChrName[I - 1];                                               // :1189
            for (int J = 1; J <= FilterChars.Length; J++)                             // :1191
            {
                char WC2 = FilterChars[J - 1];                                        // :1193
                if (WC1 == WC2)                                                       // :1195
                {
                    Result = false;                                                   // :1197
                    break;                                                            // :1198
                }
            }
        }
        return Result;
    }

    /// <summary>
    /// DBShare.pas:1204-1249 `function CheckChrName(sChrName: string): Boolean;`
    /// 入参：**latin-1 字节串**。GBK 字节级合法性判定：
    /// <list type="bullet">
    ///   <item>首字节 0x81..0xFE ⇒ 视为**双字节字符的前导**，置 `boIsTwoByte`、记住 `FirstChr`；</item>
    ///   <item>随后的那个字节必须满足 :1222-1223 的两条区间（`FirstChr &lt;= 0xF7` 时尾字节 0x40..0xFE；
    ///         `FirstChr &gt; 0xF7` 时尾字节 0x40..0xA0），否则 False；</item>
    ///   <item>单字节位置只接受 `0-9` / `a-z` / `A-Z`，否则 False；</item>
    ///   <item>全部通过后再过 <see cref="CheckCanCaseChar"/>（**隐式 GBK 解码**，见类注释）。</item>
    /// </list>
    /// ★ 原文有一行**已注释掉**的 `if Chr &lt; #$81 then Result:=False`（:1220），保留为注释。
    /// </summary>
    public static bool CheckChrName(string sChrName)
    {
        bool Result = true;                                                           // :1211
        bool boIsTwoByte = false;                                                     // :1212
        char FirstChr = '\0';                                                         // :1213
        for (int I = 1; I <= sChrName.Length; I++)                                    // :1214
        {
            char Chr = sChrName[I - 1];                                               // :1216
            if (boIsTwoByte)                                                          // :1217
            {
                //if Chr < #$A1 then Result:=False; //如果小于就是非法字符
                //      if Chr < #$81 then Result:=False; //如果小于就是非法字符   :1219-1220（原文注释）
                if (!((FirstChr <= '\u00F7') && (Chr >= '\u0040') && (Chr <= '\u00FE')))   // :1222
                {
                    if (!((FirstChr > '\u00F7') && (Chr >= '\u0040') && (Chr <= '\u00A0')))  // :1223
                        Result = false;
                }
                boIsTwoByte = false;                                                  // :1224
            }
            else
            {
                //if (Chr >= #$B0) and (Chr <= #$C8) then begin
                if ((Chr >= '\u0081') && (Chr <= '\u00FE'))                           // :1229
                {
                    boIsTwoByte = true;                                               // :1231
                    FirstChr = Chr;                                                   // :1232
                }
                else
                {
                    if (!((Chr >= '0') && (Chr <= '9'))                               // :1236
                     && !((Chr >= 'a') && (Chr <= 'z'))                               // :1237
                     && !((Chr >= 'A') && (Chr <= 'Z')))                              // :1238
                        Result = false;
                }
            }
            if (!Result) break;                                                       // :1242
        }

        if (Result)                                                                   // :1245
        {
            // :1247 `Result := CheckCanCaseChar(sChrName);` —— AnsiString→WideString 是**隐式 GBK 解码**
            Result = CheckCanCaseChar(AnsiTextOf(sChrName));
        }
        return Result;
    }

    /// <summary>
    /// DBShare.pas:1251-1274 `function CheckSpecialChar(sChrName: WideString): Boolean;`
    /// 入参：**GBK 文本**（UTF-16）。名字里出现 `FilterChars` 中的任一个字符 → False。
    ///
    /// ★ `FilterChars` 逐字照抄原文 :1253（含那个被两个单引号转义的 `'`）。
    /// </summary>
    public static bool CheckSpecialChar(string sChrName)
    {
        const string FilterChars = " /@?'\"\\.,:;`~!#$%^&*()-_+|[]{}";                 // :1253

        bool Result = true;                                                           // :1258
        for (int I = 1; I <= sChrName.Length; I++)                                    // :1259
        {
            char WC1 = sChrName[I - 1];                                               // :1261
            for (int J = 1; J <= FilterChars.Length; J++)                             // :1263
            {
                char WC2 = FilterChars[J - 1];                                        // :1265
                if (WC1 == WC2)                                                       // :1267
                {
                    Result = false;                                                   // :1269
                    break;                                                            // :1270
                }
            }
        }
        return Result;
    }

    // ==========================================================================================
    // 内部助手
    // ==========================================================================================

    /// <summary>
    /// 原文 `UpperCase(AnsiString)`（SysUtils.pas）的**忠实**托管实现 —— 只折 ASCII 字母。
    ///
    /// ★★ **不能**用 <see cref="DelphiRTL.UpperCase"/>（它走 `ToUpperInvariant`）。原因：
    ///   本文件的入参是**承载 GBK 字节的 latin-1 字符串**，字节 0x80..0xFF 是**双字节字的组成部分**；
    ///   `ToUpperInvariant` 会把 0xE0~0xFE 折成 0xC0~0xDE ⇒
    ///   ① 汉字字节被**改写**（0xE0→0xC0），名字直接损坏；
    ///   ② 制造**假匹配**：`0xC0` 与 `0xE0` 折到一起后，`Pos`/`CompareText` 会认为两个不同的名字相同。
    ///   原文在 **CP936(DBCS)** 下 `UpperCase` 不会对双字节字内部字节做大小写映射（只折单字节 ASCII）
    ///   ⇒ 本实现只折 `a-z`，其余字节**原样不动**。
    /// </summary>
    internal static string UpperCaseAnsi(string byteStr)
    {
        if (string.IsNullOrEmpty(byteStr)) return byteStr ?? "";
        char[] a = byteStr.ToCharArray();
        for (int i = 0; i < a.Length; i++)
        {
            if (a[i] >= 'a' && a[i] <= 'z') a[i] = (char)(a[i] - 32);
        }
        return new string(a);
    }

    /// <summary>latin-1 字节串 → GBK 文本（原文 `wChrName := sChrName` 的隐式 AnsiString→WideString 转换）。</summary>
    internal static string AnsiTextOf(string byteStr)
        => EncodingInit.GBK.GetString(SelectClientAnsi.BytesOf(byteStr));

    /// <summary>
    /// 原文 `S := wChrName[I]; Chr := S[1];`（:1114-1115 / :1135-1136）：
    /// 把一个字符按 **GBK 编码**回去，取**第一个字节**。
    /// ASCII 字符 → 自身；汉字 → 首字节（≥0x81）；无法编码的字符 → `?`(0x3F)。
    /// </summary>
    internal static char FirstAnsiByteOf(char c)
    {
        byte[] b = EncodingInit.GBK.GetBytes(c.ToString());
        return b.Length > 0 ? (char)b[0] : '\0';
    }
}
