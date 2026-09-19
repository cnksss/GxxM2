{ License, info, etc
  ------------------

  This implementation is made by Tang qishi (Satrry), to contact me
  mail to 619325079@qq.com or tangqishi@gmail.com

  This implementaion is available at
  http://www.cnblogs.com/tangqs/archive/2011/11/03/2234715.html

  copyright 2011, Tang qishi
  This header may not be removed.

  Version V2.6
  Create time : 2017-02-28
}
// ********************************************************************************/
// *   数学表达式解析计算    作者:唐齐时 Tang qishi (Satrry)  2012.01-2017.02     */
// *   以下网址有详细介绍与最新代码下载：                                         */
// *   http://www.cnblogs.com/tangqs/archive/2011/11/03/2234715.html              */
// *   有改进意见或发现任何错误请转告我,本人不胜感激。                            */
// *   E-Mail:619325079@qq.com                                                    */
// *   (  转载时请保留本说明:)  )                                                 */
// ********************************************************************************/

{
  /*   调用方式：CalcExp('1+max(0.5,sin(1))+sum(1,2^3,mod(5,3))', res, infoStr)   */
  /*   带符号参数调用方法,先调用符号定义AddSignParam，再调用 CalcExp：            */
  /*        AddSignParam(['a','s'], [1, 0.5]);  或者 AddSignParam('a=1,s=0.5')    */
  /*        CalcExp('1+a+sin(s)', res, infoStr)                                   */
  /*   其中res存储计算结果，为double型；infoStr存储计算时的提示信息,为string      */
  //*******************************************************************************/
  /*   表达式支持的功能：                                                         */
  /*   1、四则运算 + - * / 、括弧()、正负(+ -)                                    */
  /*   2、百分数 %、求幂 ^ 、整数阶乘 ! (1 至 150)                                */
  /*   3、符号参数计算，示例：a+b,使用前要先定义符号值，调用AddSignParam函数      */
  /*                    AddSignParam('a=1,b=0.5')                                 */
  /*   4、常数e、圆周率PI                                                         */
  /*   5、丰富的函数功能:                                                         */
  /*          统计函数：   max,min,sum,avg,stddev 标准偏差，均支持多参数          */
  /*          三角函数:    sin,cos,tan,arcsin,arccos,arctan                       */
  /*                       degrad(60)   角度转弧度                                */
  /*                       raddeg(3.14) 弧度转角度                                */
  /*                       costh(a,b,c) 余弦定理 cosC)                            */
  /*         指数对数函数：sqrt,power(x,y),abs,exp,log2,log10,logN(a,N),ln        */
  /*         数据处理函数：int(x),trunc(x) 取整                                   */
  /*                       frac(x) 取小数部分                                     */
  /*                       round(x) 四舍五入取整                                  */
  /*                       roundto(x,-1) 保留一位小数                             */
  /*                       mod(M,N) 求模                                          */
  /*         几何面积函数：s_tria(a,b,c) 三角形面积                               */
  /*                       s_circ(r)     圆形面积                                 */
  /*                       s_elli(a,b)   椭圆面积                                 */
  /*                       s_rect(a,b)   矩形面积                                 */
  /*                       s_poly(a,n)   正多边形面积                             */
  /*        平面几何函数： pdisplanes(x1,y1,x2,y2) 平面两点距离                   */
  /*                       pdisspace(x1,y1,z1,x2,y2,z2) 空间两点                  */
  /*                       p_line(x0,y0, A, B, C) 平面点到线距离                  */
  /*                       p_planes(x0,y0,z0 A, B, C, D)空间点到面距离            */
  /*        数列求和:      sn(a1, d, n) 等差数列前n项和                           */
  /*                       sqn(a1, q, n) 等比数列前n项和                          */
  /*      个税计算函数：   intax(x), arcintax(x) 个税反算                         */
  /*                       morrep(A,i,N,trpe,monthN)                              */
  /*   5、历史计算记录，双击计算记录可重新修改计算                                */
  /*   示例: sin(1)+(-2+(3-4))*20% , e^63+PI , 15! , log2(max(2,3)                */
  /*   注: 运算符必须为半角格式，三角函为弧度，输入可用空格间隔                   */
  //*******************************************************************************/ }

unit UnitExpCalc;

interface

uses
  SysUtils, Variants, Classes, StdCtrls, StrUtils, Dialogs, math;

type
  TSetChar = set of AnsiChar;

  RecStack = record
    sv: TStringList;
    top: integer;
  end;

function CalcExp(const aExpT: string; out calResult: double; out AInfo: string): boolean;
function CheckCalcExp(const ExpT: string; out AInfo: string): boolean;
function CheckFun(): boolean;
function AddSignParam(const ASignList: array of string; ASignValue: array of double): integer; overload;
function AddSignParam(const ASignParamStr: string): integer; overload;
function GetSignParam(Aindex: integer = 0): string; overload;
function GetSignParam(const AParamName: string): string; overload;
function DelSignParam(const AParamName: string): boolean;
procedure InitSignParam(); // 清除用户自定义的参数，恢复常数 pi, e
// 以下为本函数内部调用函数
{
  procedure Init();
  procedure FreeRes();

  procedure replaceSignParam(var AExpT: string);
  procedure SetError(var AError: boolean; var AErrorStr: string; const AStr: string);
  function Push(var aStack: RecStack; const values: string): boolean;      // 进栈
  function Pop(var aStack: RecStack; out values: string): boolean;         // 出栈
  function GetTop(var aStack: RecStack): string;
  function GetSyP(sy: char): integer;
  function GetStackStr(const aStack: RecStack): string;
  function GetParamStr(const AParamStr: string; const AParamN: integer): string;
  function IsCalcFun(const AFunName: string; const ACalcFunList: TStringList): boolean;
  function GetFunParamN(const AFunName: string; const ACalcFunList: TStringList):integer;
  function GetFunParamValuse(const AParamStr: string; var A_Valuse_arr: array of double): boolean;
  function CheckCalcFun(const ACalcFunList: TStringList): boolean;
  // 自定义函数
  function IncomeTaxCalc(const AValues: double): double;     // 最新个税计算函数
  function ArcIncomeTaxCalc(const AValues: double): double;  // 最新个税反算函数
  function My_max(const AParam: string; out ARes: double; var AError: boolean; var AErrorStr: string): boolean;
  function My_min(const AParam: string; out ARes: double; var AError: boolean; var AErrorStr: string): boolean;
  function My_sum(const AParam: string; out ARes: double; var AError: boolean; var AErrorStr: string): boolean;
  function My_avg(const AParam: string; out ARes: double; var AError: boolean; var AErrorStr: string): boolean;
  function My_stdDev(const AParam: string; out ARes: double; var AError: boolean; var AErrorStr: string): boolean;
  function My_cosTh(const AParam: string; out ARes: double; var AError: boolean; var AErrorStr: string): boolean;
  function My_AreaTriangle(const AParam: string; out ARes: double; var AError: boolean; var AErrorStr: string): boolean;
  function My_PlanesPointDis(const AParam: string; out ARes: double; var AError: boolean; var AErrorStr: string): boolean;
  function My_SpacePointDis(const AParam: string; out ARes: double; var AError: boolean; var AErrorStr: string): boolean;
  function My_PointToLineDis(const AParam: string; out ARes: double; var AError: boolean; var AErrorStr: string): boolean;
  function My_PointToPlanesDis(const AParam: string; out ARes: double; var AError: boolean; var AErrorStr: string): boolean;
  function My_SumSubSeri(const AParam: string; out ARes: double; var AError: boolean; var AErrorStr: string): boolean;
  function My_SumRatioSeri(const AParam: string; out ARes: double; var AError: boolean; var AErrorStr: string): boolean;
}

implementation

var
  symbolSets, numberSets: TSetChar;
  sMathFun: TStringList;
  SignArr: array of string;
  SignValueArr: array of double;

  // *************************************************************************
  // *************************************************************************
function AddSignParam(const ASignList: array of string; ASignValue: array of double): integer;
var // 调用格式：AddSignParam(['a','b','c'], [1, 2,3]);
  i, j, minPN: integer;
  f_sameParam: boolean;
begin
  result := 0;
  for i := 0 to High(ASignList) do // 检查符号字符是否正确
  begin
    if ASignList[i] = '' then
      Exit;

    if not(AnsiChar(ASignList[i][1]) in ['A' .. 'Z', 'a' .. 'z']) then
      Exit; // 符号首字母必须为字母

    for j := 1 to Length(ASignList[i]) do
      if not(AnsiChar(ASignList[i][j]) in ['A' .. 'Z', 'a' .. 'z', '0' .. '9', '_']) then
        Exit;
  end;

  minPN := Min(High(ASignList), High(ASignValue));
  for i := 0 to minPN do
  begin
    Inc(result);
    f_sameParam := false;
    for j := 0 to High(SignArr) do
    begin // 修改已存在的符号与对应值
      if ASignList[i] = SignArr[j] then
      begin
        SignValueArr[j] := ASignValue[i];
        f_sameParam := true;
        Break;
      end;
    end;

    if not f_sameParam then
    begin // 新增符号与对应值
      SetLength(SignValueArr, Length(SignValueArr) + 1);
      SignValueArr[high(SignValueArr)] := ASignValue[i];
      SetLength(SignArr, Length(SignArr) + 1);
      SignArr[high(SignArr)] := ASignList[i];
    end;
  end;
end;

function AddSignParam(const ASignParamStr: string): integer;
var // 调用格式：AddSignParam('a=1,b=2.1,c=3');
  i, j, commPos, equPos: integer;
  signStr, signValStr: string;
  signValue: double;
  f_signErr, f_sameParam: boolean;
begin
  result := 0;
  if Length(ASignParamStr) < 3 then
    Exit;
  commPos := 0;
  for i := 1 to Length(ASignParamStr) do
  begin
    f_signErr := false; // HZQ前置条件 20230410
    equPos := i; // HZQ
    if ASignParamStr[i] = '=' then
    begin
      // equPos := i; //HZQ
      signStr := midStr(ASignParamStr, commPos + 1, equPos - commPos - 1);
      // f_signErr := false; //移动到if块的前面，保证下一个 if始终是有值的
      if signStr = '' then
        f_signErr := true
      else if not(AnsiChar(signStr[1]) in ['A' .. 'Z', 'a' .. 'z']) then
        f_signErr := true
      else
        for j := 1 to Length(signStr) do
          if not(AnsiChar(signStr[j]) in ['A' .. 'Z', 'a' .. 'z', '0' .. '9', '_']) then
            f_signErr := true;

    end; // if ASignParamStr[i] = '=' then

    if (ASignParamStr[i] = ',') or (i = Length(ASignParamStr)) then
    begin
      commPos := i;
      if i = Length(ASignParamStr) then
        commPos := i + 1;
      signValStr := midStr(ASignParamStr, equPos + 1, commPos - equPos - 1);
      if TryStrToFloat(signValStr, signValue) and (not f_signErr) then
      begin
        // ****************************
        result := result + 1;
        f_sameParam := false;
        for j := 0 to High(SignArr) do
        begin // 修改已存在的符号与对应值
          if signStr = SignArr[j] then
          begin
            SignValueArr[j] := signValue;
            f_sameParam := true;
            Break;
          end;
        end; // for
        if not f_sameParam then
        begin // 新增符号与对应值
          SetLength(SignValueArr, Length(SignValueArr) + 1);
          SignValueArr[high(SignValueArr)] := signValue;
          SetLength(SignArr, Length(SignArr) + 1);
          SignArr[high(SignArr)] := signStr;
        end;
        // ********************************
      end; // if
    end;
  end;
end;

procedure ReplaceSignParam(var aExpT: string); // 2017-02-28 修改了该函数的bug
var
  i, len, tmpPos, signPos, signLen, replaceLen: integer;
  ExpT, tmpStr: string;
  f_SignP: boolean;
begin
  // 预处符号参数    SignArr, SignValueArr
  if (Length(SignArr) = 0) or (Length(SignValueArr) = 0) then
    Exit;
  ExpT := aExpT;
  len := Length(ExpT);
  for i := 0 to High(SignArr) do
  begin
    signPos := Pos(SignArr[i], ExpT);
    while (signPos <> 0) do
    begin
      f_SignP := true;
      signLen := Length(SignArr[i]); // 符号长度
      if (signPos - 1) > 0 then
        if (AnsiChar(ExpT[signPos - 1]) in ['A' .. 'Z', 'a' .. 'z', '0' .. '9', '_']) then
          f_SignP := false;
      if (signPos + signLen) <= len then
        if (AnsiChar(ExpT[signPos + signLen]) in ['A' .. 'Z', 'a' .. 'z', '0' .. '9', '_']) then
          f_SignP := false;
      if f_SignP then // 是计算符号时
      begin
        delete(ExpT, signPos, signLen);
        Insert(floatToStr(SignValueArr[i]), ExpT, signPos);
        len := Length(ExpT);
        replaceLen := Length(floatToStr(SignValueArr[i])); // 符号数值的字符长度
      end
      else
        replaceLen := signLen;
      tmpStr := RightStr(ExpT, len - signPos - replaceLen + 1);
      tmpPos := Pos(SignArr[i], tmpStr);
      if tmpPos <> 0 then
        signPos := signPos + tmpPos + replaceLen - 1
      else
        signPos := 0;
    end; // while
  end; // for
  aExpT := ExpT;
end;

function GetSignParam(Aindex: integer = 0): string;
var
  i: integer;
begin
  result := '';
  if Aindex < 0 then
    Exit;
  if (Aindex = 0) or (Aindex >= Length(SignArr)) then
  begin
    for i := 0 to High(SignArr) do
      result := result + SignArr[i] + '=' + floatToStr(SignValueArr[i]) + ' , ';
  end
  else
    result := 'param' + IntToStr(Aindex) + ': ' + SignArr[Aindex] + '=' + floatToStr(SignValueArr[Aindex]);
end;

function GetSignParam(const AParamName: string): string; overload;
var
  i: integer;
begin
  result := '';
  if AParamName = '' then
    Exit;
  for i := 0 to High(SignArr) do
    if SignArr[i] = AParamName then
    begin
      result := floatToStr(SignValueArr[i]);
    end;
end;

function DelSignParam(const AParamName: string): boolean;
var
  i, j: integer;
begin
  result := false;
  if AParamName = '' then
    Exit;

  for i := 0 to High(SignArr) do
  begin
    if SignArr[i] = AParamName then
    begin
      for j := (i + 1) to High(SignArr) do
      begin
        SignArr[j - 1] := SignArr[j];
        SignValueArr[j - 1] := SignValueArr[j];
      end;
      SetLength(SignArr, Length(SignArr) - 1);
      SetLength(SignValueArr, Length(SignValueArr) - 1);
      result := true;
      Break;
    end;
  end;
end;

procedure InitSignParam(); // 初始化参数
begin
  SetLength(SignArr, 0);
  SetLength(SignValueArr, 0);
  AddSignParam(['pi', 'PI', 'e', 'E'], [3.1415926535897932384, 3.1415926535897932384, 2.718281828459045, 2.718281828459045]);
end;

function CheckCalcFun(const ACalcFunList: TStringList): boolean;
var
  i, j, bkPos1, bkPos2: integer;
  fNStr: string;
  funCharError: boolean;
begin
  if ACalcFunList.Count = 0 then
  begin
    result := false;
    Exit;
  end;
  result := true;
  for i := 0 to (ACalcFunList.Count - 1) do
  begin
    fNStr := Trim(ACalcFunList[i]);
    if Length(fNStr) = 0 then
    begin
      result := false;
      Break; // 函数首字母不符
    end;
    bkPos1 := Pos('(', fNStr);
    bkPos2 := Pos(')', fNStr);
    funCharError := false;
    if (not(AnsiChar(fNStr[1]) in ['a' .. 'z', 'A' .. 'Z', '_'])) then
    begin
      result := false;
      Break; // 函数首字母不符
    end;
    for j := 2 to (bkPos1 - 1) do
      if (not(AnsiChar(fNStr[j]) in ['a' .. 'z', 'A' .. 'Z', '_', '0' .. '9'])) then
      begin
        funCharError := true;
        Break;
      end;
    if (bkPos1 + 1) > (bkPos2 - 1) then
      funCharError := true;
    for j := (bkPos1 + 1) to (bkPos2 - 1) do
      if (not(AnsiChar(fNStr[j]) in ['0' .. '9'])) then
      begin
        funCharError := true;
        Break;
      end;
    if bkPos2 <> Length(fNStr) then
      funCharError := true; // ) 后面还有字符
    if funCharError then
    begin
      result := false;
      Break; // 函数字母不符合要求
    end;
  end;
end;

function CheckFun(): boolean;
begin
  result := CheckCalcFun(sMathFun);
end;

function Pop(var aStack: RecStack; out values: string): boolean;
begin
  if aStack.top <> -1 then
  begin
    values := aStack.sv[aStack.top];
    aStack.sv.delete(aStack.top);
    aStack.top := aStack.sv.Count - 1;
    result := true;
  end
  else
    result := false; // 栈为满
end;

function Push(var aStack: RecStack; const values: string): boolean;
begin
  result := false;
  aStack.sv.Add(values);
  aStack.top := aStack.sv.Count - 1;
  result := true;
end;

function GetTop(var aStack: RecStack): string;
begin
  if (aStack.top >= 0) and (aStack.top = (aStack.sv.Count - 1)) then
    result := aStack.sv[aStack.top]
  else
    result := '';
end;

function GetSyP(sy: char): integer;
begin
  case sy of
    '#':
      result := 0;
    '(':
      result := 1;
    ',':
      result := 2;
    '+', '-':
      result := 3;
    '*', '/':
      result := 5;
    '~', '@':
      result := 7;
    '!', '%', '^':
      result := 9;
    ')':
      result := 10;
  else
    result := -1;
  end;
end;

function GetStackStr(const aStack: RecStack): string;
var
  i: integer;
  str: string;
begin
  str := '';
  for i := 0 to aStack.top do
    str := str + aStack.sv[i] + ',';
  result := str;
end;

procedure SetError(var AError: boolean; var AErrorStr: string; const AStr: string);
begin
  AError := true;
  AErrorStr := AErrorStr + AStr + ';';
end;

function GetParamStr(const AParamStr: string; const AParamN: integer): string;
var
  i, commaCount, commaPos: integer;
  remainPamraStr: string;
begin // 去多参数函数的某个参数
  result := '';
  remainPamraStr := StringReplace(AParamStr, ' ', '', [rfReplaceAll]);
  if (AParamN <= 0) or (remainPamraStr = '') then
    Exit;
  i := 1;
  commaCount := 0;
  while (i < AParamN) do
  begin
    commaPos := Pos(',', remainPamraStr);
    if (commaPos <> 0) then
      Inc(commaCount);
    remainPamraStr := RightStr(remainPamraStr, (Length(remainPamraStr) - commaPos));
    Inc(i);
  end;
  if (commaCount = (AParamN - 1)) then
  begin
    commaPos := Pos(',', remainPamraStr);
    if commaPos = 0 then
      result := remainPamraStr
    else
      result := LeftStr(remainPamraStr, commaPos - 1);
  end
  else
    result := '';
end;

function IsCalcFun(const AFunName: string; const ACalcFunList: TStringList): boolean;
var
  i, bkPos: integer;
  fNStr: string;
begin
  result := false;
  if ACalcFunList.Count = 0 then
    Exit;
  for i := 0 to (ACalcFunList.Count - 1) do
  begin
    fNStr := Trim(ACalcFunList[i]);
    bkPos := Pos('(', fNStr);
    if bkPos > 0 then
      if AFunName = LeftStr(fNStr, bkPos - 1) then
      begin
        result := true;
        Break;
      end;
  end;
end;

function GetFunParamN(const AFunName: string; const ACalcFunList: TStringList): integer;
var
  i, bkPos1, bkPos2: integer;
  fNStr: string;
begin
  result := -1;
  if ACalcFunList.Count = 0 then
    Exit;
  for i := 0 to (ACalcFunList.Count - 1) do
  begin
    fNStr := Trim(ACalcFunList[i]);
    bkPos1 := Pos('(', fNStr);
    bkPos2 := Pos(')', fNStr);
    if (bkPos1 > 0) and (bkPos2 > 0) and (bkPos2 > bkPos1) then
      if AFunName = LeftStr(fNStr, bkPos1 - 1) then
      begin
        if not TryStrToInt(midStr(fNStr, (bkPos1 + 1), (bkPos2 - bkPos1 - 1)), result) then
          result := -1;
        Break;
      end;
  end;
end;

function IncomeTaxCalc(const AValues: double): double;
var
  overPays: double;
begin // 个人所得税计算函数
  {
    按2011年9月1日的最新政策 起征点 3500元
    全月应纳税额不超过1500元 3% 0
    全月应纳税额超过1500元至4500元 10% 105
    全月应纳税额超过4500元至9000元 20% 555
    全月应纳税额超过9000元至35000元 25% 1005
    全月应纳税额超过35000元至55000元 30% 2755
    全月应纳税额超过55000元至80000元 35% 5505
    全月应纳税额超过80000元 45% 13505
  }
  overPays := AValues - 3500;
  { if overPays <= 0 then result := 0;
    if (overPays > 0   )  and (overPays <= 1500) then result := overPays * 0.03 - 0;
    if (overPays > 1500)  and (overPays <= 4500)  then result := overPays * 0.1  - 105;
    if (overPays > 4500)  and (overPays <= 9000)  then result := overPays * 0.2  - 555;
    if (overPays > 9000)  and (overPays <= 35000) then result := overPays * 0.25 - 1005;
    if (overPays > 35000) and (overPays <= 55000) then result := overPays * 0.3  - 2755;
    if (overPays > 55000) and (overPays <= 80000) then result := overPays * 0.35 - 5505;
    if (overPays > 80000) then                         result := overPays * 0.45 - 13505; }

  if overPays <= 0 then
  begin
    result := 0;
  end
  else if (overPays <= 1500) then
  begin
    result := overPays * 0.03 - 0;
  end
  else if (overPays <= 4500) then
  begin
    result := overPays * 0.1 - 105;
  end
  else if (overPays <= 9000) then
  begin
    result := overPays * 0.2 - 555;
  end
  else if (overPays <= 35000) then
  begin
    result := overPays * 0.25 - 1005;
  end
  else if (overPays <= 55000) then
  begin
    result := overPays * 0.3 - 2755;
  end
  else if (overPays <= 80000) then
  begin
    result := overPays * 0.35 - 5505;
  end
  else { if (overPays > 80000) then }
  begin
    result := overPays * 0.45 - 13505;
  end;
end;

function ArcIncomeTaxCalc(const AValues: double): double;
begin
  {
    0	    45
    45	  345
    345	  1245
    1245	7745
    7745	13745
    13745	22495
    22495 +∞
  }

  (*
    if AValues <= 0 then
    begin
    result := 0;  // 不交个税时无法反算工资
    Exit;
    end;
    if (AValues > 0)     and (AValues <= 45)    then result := 3500 + (AValues + 0)     / 0.03;
    if (AValues > 45)    and (AValues <= 345)   then result := 3500 + (AValues + 105)   / 0.1;
    if (AValues > 345)   and (AValues <= 1245)  then result := 3500 + (AValues + 555)   / 0.2;
    if (AValues > 1245)  and (AValues <= 7745)  then result := 3500 + (AValues + 1005)  / 0.25;
    if (AValues > 7745)  and (AValues <= 13745) then result := 3500 + (AValues + 2755)  / 0.3;
    if (AValues > 13745) and (AValues <= 22495) then result := 3500 + (AValues + 5505)  / 0.35;
    if (AValues > 22495)                        then result := 3500 + (AValues + 13505) / 0.45;
  *)

  if AValues <= 0 then
  begin
    result := 0; // 不交个税时无法反算工资
  end
  else if (AValues <= 45) then
  begin
    result := 3500 + (AValues + 0) / 0.03;
  end
  else if (AValues <= 345) then
  begin
    result := 3500 + (AValues + 105) / 0.1;
  end
  else if (AValues <= 1245) then
  begin
    result := 3500 + (AValues + 555) / 0.2;
  end
  else if (AValues <= 7745) then
  begin
    result := 3500 + (AValues + 1005) / 0.25;
  end
  else if (AValues <= 13745) then
  begin
    result := 3500 + (AValues + 2755) / 0.3;
  end
  else if (AValues <= 22495) then
  begin
    result := 3500 + (AValues + 5505) / 0.35;
  end
  else { if (AValues > 22495) then }
  begin
    result := 3500 + (AValues + 13505) / 0.45;
  end;
end;

function GetFunParamValuse(const AParamStr: string; var A_Valuse_arr: array of double): boolean;
var
  i: integer;
  tmpStr: string;
begin
  i := 1;
  tmpStr := '';
  try
    tmpStr := GetParamStr(AParamStr, i);
    while ((tmpStr <> '') and (i <= (High(A_Valuse_arr) + 1))) do
    begin
      A_Valuse_arr[i - 1] := StrToFloat(tmpStr);
      Inc(i);
      tmpStr := GetParamStr(AParamStr, i);
    end;
    result := true;
  except
    result := false;
  end;
end;

function My_MorRep(const AParam: string; out ARes: double; var AError: boolean; var AErrorStr: string): boolean;
var // morrep(480000,0.049,30,0,1)
  i, arrLen: integer;
  vRes: double;
  vn: array of double;
BEGIN
  result := false;
  arrLen := 1;
  for i := 1 to Length(AParam) do
    if AParam[i] = ',' then
      Inc(arrLen);
  SetLength(vn, arrLen);
  GetFunParamValuse(AParam, vn);
  if ((arrLen = 5) and (vn[0] > 0) and (vn[1] > 0) and (vn[2] > 0) and (frac(vn[2]) = 0) and (vn[4] > 0) and (vn[4] <= vn[2] * 12)
    and (frac(vn[4]) = 0)) then
  begin
    if vn[3] = 1 then // 等额本金还款方式
    begin
      vRes := vn[0] / (vn[2] * 12) + vn[0] * (1 - (vn[4] - 1) / (vn[2] * 12)) * vn[1] / 12;
    end
    else
    begin // 其余默认按等额本息还款方式
      vRes := vn[0] * (vn[1] / 12) * power(1 + (vn[1] / 12), vn[2] * 12) / (power(1 + (vn[1] / 12), vn[2] * 12) - 1);
    end;
    ARes := vRes;
    result := true;
  end
  else
  begin
    SetError(AError, AErrorStr, '按揭还款参数输入有误！');
    Exit;
  end;
end;

function My_max(const AParam: string; out ARes: double; var AError: boolean; var AErrorStr: string): boolean;
var
  i, arrLen: integer;
  vRes: double;
  vn: array of double;
begin
  result := false;
  arrLen := 1;
  for i := 1 to Length(AParam) do
    if AParam[i] = ',' then
      Inc(arrLen);
  SetLength(vn, arrLen);
  GetFunParamValuse(AParam, vn);
  vRes := vn[0];
  for i := 1 to (Length(vn) - 1) do
    vRes := max(vRes, vn[i]);
  ARes := vRes;
  result := true;
end;

function My_min(const AParam: string; out ARes: double; var AError: boolean; var AErrorStr: string): boolean;
var
  i, arrLen: integer;
  vRes: double;
  vn: array of double;
begin
  result := false;
  arrLen := 1;
  for i := 1 to Length(AParam) do
    if AParam[i] = ',' then
      Inc(arrLen);
  SetLength(vn, arrLen);
  GetFunParamValuse(AParam, vn);
  vRes := vn[0];
  for i := 1 to (Length(vn) - 1) do
    vRes := Min(vRes, vn[i]);
  ARes := vRes;
  result := true;
end;

function My_sum(const AParam: string; out ARes: double; var AError: boolean; var AErrorStr: string): boolean;
var
  i, arrLen: integer;
  vRes: double;
  vn: array of double;
begin
  result := false;
  arrLen := 1;
  for i := 1 to Length(AParam) do
    if AParam[i] = ',' then
      Inc(arrLen);
  SetLength(vn, arrLen);
  GetFunParamValuse(AParam, vn);
  vRes := vn[0];
  for i := 1 to (Length(vn) - 1) do
    vRes := vRes + vn[i];
  ARes := vRes;
  result := true;
end;

function My_avg(const AParam: string; out ARes: double; var AError: boolean; var AErrorStr: string): boolean;
var
  i, arrLen: integer;
  vRes: double;
  vn: array of double;
begin
  result := false;
  arrLen := 1;
  for i := 1 to Length(AParam) do
    if AParam[i] = ',' then
      Inc(arrLen);
  SetLength(vn, arrLen);
  GetFunParamValuse(AParam, vn);
  vRes := vn[0];
  for i := 1 to (Length(vn) - 1) do
    vRes := vRes + vn[i];
  vRes := vRes / Length(vn);
  ARes := vRes;
  result := true;
end;

function My_stdDev(const AParam: string; out ARes: double; var AError: boolean; var AErrorStr: string): boolean;
var
  i, arrLen: integer;
  vRes, avgV: double;
  vn: array of double;
begin
  result := false;
  arrLen := 1;
  for i := 1 to Length(AParam) do
    if AParam[i] = ',' then
      Inc(arrLen);
  SetLength(vn, arrLen);
  GetFunParamValuse(AParam, vn);
  avgV := 0;
  for i := 0 to (Length(vn) - 1) do
    avgV := avgV + vn[i];
  avgV := avgV / Length(vn);
  vRes := 0;
  for i := 0 to (Length(vn) - 1) do
    vRes := vRes + power((vn[i] - avgV), 2);
  vRes := sqrt(vRes / (Length(vn) - 1));
  result := true;
end;

function My_cosTh(const AParam: string; out ARes: double; var AError: boolean; var AErrorStr: string): boolean;
var
  i, arrLen: integer;
  vRes: double;
  vn: array of double;
begin
  result := false;
  arrLen := 1;
  for i := 1 to Length(AParam) do
    if AParam[i] = ',' then
      Inc(arrLen);
  SetLength(vn, arrLen);
  GetFunParamValuse(AParam, vn);
  if ((arrLen = 3) and (vn[0] + vn[1] > vn[2]) and (vn[1] + vn[2] > vn[0]) and (vn[0] + vn[2] > vn[1]) and (vn[0] > 0) and
    (vn[1] > 0) and (vn[2] > 0)) then
  begin
    vRes := (power(vn[0], 2) + power(vn[1], 2) - power(vn[2], 2)) / (2 * vn[0] * vn[1]);
    if (vRes >= 1) or (vRes <= -1) then
      SetError(AError, AErrorStr, '三边不能构成一个三角形!')
    else
    begin
      ARes := vRes;
      result := true;
    end;
  end
  else
  begin
    SetError(AError, AErrorStr, '三边不能构成一个三角形 !');
    Exit;
  end;
end;

function My_AreaTriangle(const AParam: string; out ARes: double; var AError: boolean; var AErrorStr: string): boolean;
var // 三角形面积
  i, arrLen: integer;
  vRes, tmpV: double;
  vn: array of double;
begin
  result := false;
  arrLen := 1;
  for i := 1 to Length(AParam) do
    if AParam[i] = ',' then
      Inc(arrLen);
  SetLength(vn, arrLen);
  GetFunParamValuse(AParam, vn);
  if ((arrLen = 3) and (vn[0] + vn[1] > vn[2]) and (vn[1] + vn[2] > vn[0]) and (vn[0] + vn[2] > vn[1]) and (vn[0] > 0) and
    (vn[1] > 0) and (vn[2] > 0)) then
  begin
    tmpV := 0.5 * (vn[0] + vn[1] + vn[2]);
    vRes := sqrt(tmpV * (tmpV - vn[0]) * (tmpV - vn[1]) * (tmpV - vn[2]));
    ARes := vRes;
    result := true;
  end
  else
  begin
    SetError(AError, AErrorStr, '三边不能构成一个三角形 !');
    Exit;
  end;
end;

function My_PlanesPointDis(const AParam: string; out ARes: double; var AError: boolean; var AErrorStr: string): boolean;
var // 平面两点的距离  v0,v1,v2,v3 (x1,y1) (x2,y2)
  i, arrLen: integer;
  vn: array of double;
begin
  result := false;
  arrLen := 1;
  for i := 1 to Length(AParam) do
    if AParam[i] = ',' then
      Inc(arrLen);
  SetLength(vn, arrLen);
  GetFunParamValuse(AParam, vn);
  if (arrLen = 4) then
  begin
    ARes := sqrt(power(vn[0] - vn[2], 2) + power(vn[1] - vn[3], 2));
    result := true;
  end
  else
  begin
    SetError(AError, AErrorStr, '平面两点坐标有误 !');
    Exit;
  end;
end;

function My_SpacePointDis(const AParam: string; out ARes: double; var AError: boolean; var AErrorStr: string): boolean;
var // 空间两点的距离   v0,v1,v2,v3,v4,v5, (x1,y1,z1) (x2,y2,z2)
  i, arrLen: integer;
  vn: array of double;
begin
  result := false;
  arrLen := 1;
  for i := 1 to Length(AParam) do
    if AParam[i] = ',' then
      Inc(arrLen);
  SetLength(vn, arrLen);
  GetFunParamValuse(AParam, vn);
  if (arrLen = 6) then
  begin
    ARes := sqrt(power(vn[0] - vn[3], 2) + power(vn[1] - vn[4], 2) + power(vn[2] - vn[5], 2));
    result := true;
  end
  else
  begin
    SetError(AError, AErrorStr, '空间两点坐标有误 !');
    Exit;
  end;
end;

function My_PointToLineDis(const AParam: string; out ARes: double; var AError: boolean; var AErrorStr: string): boolean;
var // 平面点到直线的距离  v0,v1, v2,v3,v4 (x1,y1) (a,b,c)   ax + by + c =0
  i, arrLen: integer;
  vn: array of double;
begin
  result := false;
  arrLen := 1;
  for i := 1 to Length(AParam) do
    if AParam[i] = ',' then
      Inc(arrLen);
  SetLength(vn, arrLen);
  GetFunParamValuse(AParam, vn);
  if (arrLen = 5) then
  begin
    ARes := abs(vn[0] * vn[2] + vn[1] * vn[3] + vn[4]) / sqrt(power(vn[2], 2) + power(vn[3], 2));
    result := true;
  end
  else
  begin
    SetError(AError, AErrorStr, '参数输入有误 !');
    Exit;
  end;
end;

function My_PointToPlanesDis(const AParam: string; out ARes: double; var AError: boolean; var AErrorStr: string): boolean;
var // 空间点到面的距离   v0,v1,v2,  v3,v4,v5,v6 (x1,y1,z1) (A,B,C,D)  Ax + By+ Cz + D =0
  i, arrLen: integer;
  vn: array of double;
begin
  result := false;
  arrLen := 1;
  for i := 1 to Length(AParam) do
    if AParam[i] = ',' then
      Inc(arrLen);
  SetLength(vn, arrLen);
  GetFunParamValuse(AParam, vn);
  if (arrLen = 7) then
  begin
    ARes := abs(vn[0] * vn[3] + vn[1] * vn[4] + vn[2] * vn[5] + vn[6]) /
      sqrt(power(vn[3], 2) + power(vn[3], 2) + power(vn[5], 2));
    result := true;
  end
  else
  begin
    SetError(AError, AErrorStr, '参数输入有误 !');
    Exit;
  end;
end;

function My_SumRatioSeri(const AParam: string; out ARes: double; var AError: boolean; var AErrorStr: string): boolean;
var // 等比数列前 n 项求和公式 v0,v1,v2 对应 a1，q, n
  i, arrLen: integer;
  vn: array of double;
begin
  result := false;
  arrLen := 1;
  for i := 1 to Length(AParam) do
    if AParam[i] = ',' then
      Inc(arrLen);
  SetLength(vn, arrLen);
  GetFunParamValuse(AParam, vn);
  if (arrLen = 3) and (vn[1] <> 1) and (vn[2] > 0) and (frac(vn[2]) = 0) then
  begin
    ARes := vn[0] * (1 - power(vn[1], vn[2])) / (1 - vn[1]);
    result := true;
  end
  else
  begin
    SetError(AError, AErrorStr, '等比数列参数输入有误，公比不能为1 !');
    Exit;
  end;
end;

function My_SumSubSeri(const AParam: string; out ARes: double; var AError: boolean; var AErrorStr: string): boolean;
var // 等差数列前 n 项求和公式  v0,v1,v2 对应 a1，d, n
  i, arrLen: integer;
  vn: array of double;
begin
  result := false;
  arrLen := 1;
  for i := 1 to Length(AParam) do
    if AParam[i] = ',' then
      Inc(arrLen);
  SetLength(vn, arrLen);
  GetFunParamValuse(AParam, vn);
  if (arrLen = 3) and (vn[2] > 0) and (frac(vn[2]) = 0) then
  begin
    ARes := vn[2] * vn[0] + vn[2] * (vn[2] - 1) * vn[1] / 2;
    result := true;
  end
  else
  begin
    SetError(AError, AErrorStr, '等比数列参数输入有误，公比不能为1 !');
    Exit;
  end;
end;

function CalcExp(const aExpT: string; out calResult: double; out AInfo: string): boolean;
var
  i, k, len, temp: integer;
  av, sy, calcV: RecStack;
  ExpT, elemStr, str1, str2, valuesStr, funNameStr, topSStr, funParamStr: string;
  v1, v2: Extended;
  vRes: double;
  caclError: boolean;
begin
  result := false;
  caclError := false;
  ExpT := StringReplace(aExpT, ' ', '', [rfReplaceAll]); // 去空格
  len := Length(ExpT);
  if len = 0 then
    Exit; // 表达式为空时退出函数
  ReplaceSignParam(ExpT); // 替换表达式中的符号参数(SignArr)
  if not CheckCalcExp(ExpT, AInfo) then
    Exit; // 检查表达式格式是否正确, 不正确时退出函数
  len := Length(ExpT); // 替换符号后，字符串长度变化，重新取长度。
  for i := len downto 1 do // 替换正负号，  正号 + 用 @，  负号 - 用 ~
  begin
    if (ExpT[i] = '-') and (i > 1) then
      if (AnsiChar(ExpT[i - 1]) in ['+', '-', '*', '/', '^', '(', ',']) then
        ExpT[i] := '~';
    if (AnsiChar(ExpT[i]) = '-') and (i = 1) then
      ExpT[i] := '~';
    if (AnsiChar(ExpT[i]) = '+') and (i > 1) then
      if (AnsiChar(ExpT[i - 1]) in ['+', '-', '*', '/', '^', '(', ',']) then
        ExpT[i] := '@';
    if (ExpT[i] = '+') and (i = 1) then
      ExpT[i] := '@';
  end;
  ExpT := StringReplace(ExpT, '@', '', [rfReplaceAll]); // 去正号
  ExpT := ExpT + '#'; // 添加结束标志 # ，优先级最低
  len := Length(ExpT);
  // 建立符号栈与逆波兰表达式栈,计算值存储栈
  av.sv := TStringList.Create; // 逆波兰栈
  sy.sv := TStringList.Create; // 符号栈
  calcV.sv := TStringList.Create; // 计算值存储栈
  try
    av.sv.Sorted := false;
    sy.sv.Sorted := false;
    calcV.sv.Sorted := false;
    calcV.top := -1;
    av.top := -1;
    sy.top := -1;
    Push(sy, '#');
    valuesStr := '';
    funNameStr := '';
    for i := 1 to len do
    begin
      elemStr := ExpT[i];
      if (valuesStr <> '') and (not(AnsiChar(ExpT[i]) in ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '.'])) then
      begin // 一个数值扫描完毕
        Push(av, valuesStr);
        while ((GetTop(sy) = '~') or (GetTop(sy) = '@')) do
          if Pop(sy, str1) then
            Push(av, str1);
        valuesStr := '';
      end;
      if (funNameStr <> '') and (not(AnsiChar(ExpT[i]) in ['a' .. 'z', 'A' .. 'Z', '0' .. '9', '_'])) then
      begin // 一个函数名扫描完毕
        Push(sy, funNameStr);
        funNameStr := '';
      end;
      case ExpT[i] of
        '#':
          begin
            while (GetTop(sy) <> '#') and (GetTop(sy) <> '') do
              if Pop(sy, str1) then // 符号栈出栈成功
                Push(av, str1); // 再存入逆波兰表达式栈
            Break; // 结束，跳出 for 循环
          end;
        '~', '@':
          Push(sy, elemStr); // 正负左单目运算符
        '!', '%':
          Push(av, elemStr); // 阶乘右单目运算
        '(':
          Push(sy, elemStr);
        ')':
          begin
            while ((GetTop(sy) <> '(') and (GetTop(sy) <> '#') and (GetTop(sy) <> '')) do
              if Pop(sy, str1) then // 符号栈出栈成功
                Push(av, str1); // 再存入逆波兰表达式栈
            Pop(sy, str1); // ')' 出栈
            topSStr := GetTop(sy);
            while ((topSStr = '~') or (topSStr = '@') or (AnsiChar(topSStr[1]) in ['a' .. 'z', 'A' .. 'Z', '_'])) do
            begin
              if Pop(sy, str1) then
                Push(av, str1);
              topSStr := GetTop(sy);
            end;
          end;
        '+', '-', '*', '/', '^':
          begin
            str1 := GetTop(sy);
            while (GetSyP(ExpT[i]) <= GetSyP(str1[1])) and (str1 <> '') do // 比较优先级
              if Pop(sy, str1) then
              begin
                Push(av, str1);
                str1 := GetTop(sy);
              end
              else
                Break;
            Push(sy, elemStr);
          end;
        '.':
          valuesStr := valuesStr + elemStr;
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9':
          begin
            if funNameStr = '' then
              valuesStr := valuesStr + elemStr
            else
              funNameStr := funNameStr + elemStr;
          end;
        'a' .. 'z', 'A' .. 'Z', '_':
          funNameStr := funNameStr + elemStr;
        ',': // 逗号处理
          begin
            while ((GetTop(sy) <> '(') and (GetTop(sy) <> ',') and (GetTop(sy) <> '#') and (GetTop(sy) <> '')) do
              if Pop(sy, str1) then
                Push(av, str1) // 符号栈出栈成功,再存入逆波兰表达式栈
              else
                Break; //
            Push(sy, elemStr);
          end;
      end; // case
    end; // for
    // showMessage('生成的逆波兰式:' + GetStackStr(av));
    // 计算逆波兰式
    try
      i := 0;
      while (not caclError) and (i <= av.top) do
      begin
        valuesStr := av.sv[i];
        case valuesStr[1] of
          '~':
            if Pop(calcV, str1) then
              Push(calcV, floatToStr(-1 * StrToFloat(str1)));
          '@':
            ;
          '!':
            begin
              if Pop(calcV, str1) then
              begin
                v1 := StrToFloat(str1);
                if (frac(v1) = 0) and (v1 <= 150) and (v1 >= 1) then
                begin // 为整数且小于150时
                  temp := Trunc(v1);
                  vRes := 1;
                  while (temp >= 1) do
                  begin // 求阶乘
                    vRes := vRes * temp;
                    Dec(temp);
                  end;
                  Push(calcV, floatToStr(vRes));
                end
                else
                begin
                  SetError(caclError, AInfo, '阶乘计算只能为整数，范围1至150!');
                end;
              end;
            end;
          '%':
            if Pop(calcV, str1) then
            begin
              vRes := StrToFloat(str1) * 0.01;
              Push(calcV, floatToStr(vRes));
            end;
          '+', '-', '*', '/', '^':
            begin
              if Pop(calcV, str1) and Pop(calcV, str2) then
              begin
                v1 := StrToFloat(str1);
                v2 := StrToFloat(str2);
                vRes := 0;
                case valuesStr[1] of
                  '+':
                    vRes := v2 + v1;
                  '-':
                    vRes := v2 - v1;
                  '*':
                    vRes := v2 * v1;
                  '/':
                    begin
                      if v1 <> 0 then
                        vRes := v2 / v1 // 除数不能为零
                      else
                        SetError(caclError, AInfo, '除数不能为零!');
                    end;
                  '^':
                    vRes := power(v2, v1);
                end;
                Push(calcV, floatToStr(vRes));
              end; // if
            end;
          '0' .. '9', '.':
            Push(calcV, valuesStr); // 数字时
          ',':
            if Pop(calcV, str1) and Pop(calcV, str2) then
              Push(calcV, str2 + ',' + str1); // 调整顺序了
          'a' .. 'z', 'A' .. 'Z', '_': // 函数时
            begin
              if IsCalcFun(valuesStr, sMathFun) and Pop(calcV, str1) then
              begin
                funParamStr := GetParamStr(str1, 1);
                if funParamStr <> '' then
                begin
                  v1 := StrToFloat(funParamStr);
                end
                else
                begin
                  v1 := 0; // 此时v1的值已经不重要了，但是还是比随机值要好 HZQ 20230410
                  SetError(caclError, AInfo, '函数参数出错!');
                end;
                // ******************************************************
                // 根据不同函数进行相应计算
                vRes := 0;
                for k := 1 to 1 do
                begin
                  valuesStr := LowerCase(valuesStr);
                  if valuesStr = 'abs' then
                  begin
                    vRes := abs(v1);
                    Break;
                  end;
                  if valuesStr = 'sin' then
                  begin
                    vRes := sin(v1);
                    Break;
                  end;
                  if valuesStr = 'cos' then
                  begin
                    vRes := cos(v1);
                    Break;
                  end;
                  if valuesStr = 'tan' then
                  begin
                    vRes := tan(v1);
                    Break;
                  end;
                  if valuesStr = 'arctan' then
                  begin
                    vRes := arctan(v1);
                    Break;
                  end;
                  if valuesStr = 'degrad' then
                  begin
                    vRes := v1 * 3.14159265358979 / 180;
                    Break;
                  end;
                  if valuesStr = 'raddeg' then
                  begin
                    vRes := v1 * 180 / 3.14159265358979;
                    Break;
                  end;
                  if valuesStr = 'int' then
                  begin
                    vRes := int(v1);
                    Break;
                  end;
                  if valuesStr = 'trunc' then
                  begin
                    vRes := Trunc(v1);
                    Break;
                  end;
                  if valuesStr = 'round' then
                  begin
                    vRes := round(v1);
                    Break;
                  end;
                  if valuesStr = 'frac' then
                  begin
                    vRes := frac(v1);
                    Break;
                  end;
                  if valuesStr = 'exp' then
                  begin
                    vRes := exp(v1);
                    Break;
                  end;
                  if valuesStr = 'intax' then
                  begin
                    vRes := IncomeTaxCalc(v1);
                    Break;
                  end; // intax 个税计算函数
                  if valuesStr = 'power' then
                  begin
                    funParamStr := '';
                    funParamStr := GetParamStr(str1, 2);
                    if funParamStr <> '' then
                    begin
                      v2 := StrToFloat(funParamStr);
                    end
                    else
                    begin
                      v2 := 0; // 此时v2的值已经不重要了，但是还是比随机值要好 HZQ 20230410
                      SetError(caclError, AInfo, '函数参数有误!');
                    end;
                    vRes := power(v1, v2);
                    Break;
                  end;
                  if valuesStr = 'sqrt' then
                  begin
                    if v1 >= 0 then
                      vRes := sqrt(v1)
                    else
                      SetError(caclError, AInfo, '根号内必须为正数!');
                    Break;
                  end;
                  if valuesStr = 'arcsin' then
                  begin
                    if (v1 >= -1) and (v1 <= 1) then
                      vRes := arcsin(v1)
                    else
                      SetError(caclError, AInfo, '反正弦函数值必须在-1到1之间!');
                    Break;
                  end;
                  if valuesStr = 'arccos' then
                  begin
                    if (v1 <= 1) and (v1 >= -1) then
                      vRes := arccos(v1)
                    else
                      SetError(caclError, AInfo, '反余弦函数值必须在-1到1之间!');
                    Break;
                  end;

                  if valuesStr = 'log2' then
                  begin
                    if v1 > 0 then
                      vRes := log2(v1)
                    else
                      SetError(caclError, AInfo, '对数函数值必须大于0!');
                    Break;
                  end;
                  if valuesStr = 'log10' then
                  begin
                    if v1 > 0 then
                      vRes := log10(v1)
                    else
                      SetError(caclError, AInfo, '对数函数值必须大于0!');
                    Break;
                  end;
                  if valuesStr = 'ln' then
                  begin
                    if v1 > 0 then
                      vRes := ln(v1)
                    else
                      SetError(caclError, AInfo, '对数函数值必须大于0!');
                    Break;
                  end;
                  if valuesStr = 'logn' then
                  begin
                    funParamStr := '';
                    funParamStr := GetParamStr(str1, 2);
                    if funParamStr <> '' then
                    begin
                      v2 := StrToFloat(funParamStr);
                    end
                    else
                    begin
                      v2 := 0; // 设置一个值总比随机好
                      SetError(caclError, AInfo, '对数函数值有误!');
                    end;
                    if (v1 > 0) and (v2 > 0) and (v2 <> 1) then
                      vRes := logN(v1, v2) // 逗号连接数值时已经调整顺序了
                    else
                      SetError(caclError, AInfo, '对数函数值与底数必须大于0，且底数不能等于1!');
                    Break;
                  end;
                  if valuesStr = 'roundto' then
                  begin
                    funParamStr := '';
                    funParamStr := GetParamStr(str1, 2);
                    if funParamStr <> '' then
                    begin
                      v2 := StrToFloat(funParamStr);
                    end
                    else
                    begin
                      v2 := 0; // HZQ
                      SetError(caclError, AInfo, '数据约位数有误!');
                    end;
                    if (frac(v2) = 0) and (v2 < 15) and (v2 > -15) then
                    begin
                      temp := round(v2);
                      vRes := roundTo(v1, temp);
                    end
                    else
                      SetError(caclError, AInfo, '数据约位数有误!');
                    Break;
                  end;
                  if valuesStr = 'mod' then
                  begin
                    funParamStr := '';
                    funParamStr := GetParamStr(str1, 2);
                    if funParamStr <> '' then
                    begin
                      v2 := StrToFloat(funParamStr);
                    end
                    else
                    begin
                      v2 := 0;
                      SetError(caclError, AInfo, '输入数据有误!')
                    end;
                    if (frac(v2) = 0) and (frac(v1) = 0) and (v2 > 0) then
                    begin
                      vRes := Trunc(v1) mod Trunc(v2);
                    end
                    else
                      SetError(caclError, AInfo, '求余必须为正整数!');
                    Break;
                  end;
                  if valuesStr = 'arcintax' then
                  begin
                    if v1 > 0 then
                      vRes := ArcIncomeTaxCalc(v1)
                    else
                      SetError(caclError, AInfo, '未交个人所得税时无法反算工资！');
                    Break;
                  end;
                  // 其他函数
                  if valuesStr = 'max' then
                  begin
                    My_max(str1, vRes, caclError, AInfo);
                    Break;
                  end;
                  if valuesStr = 'min' then
                  begin
                    My_min(str1, vRes, caclError, AInfo);
                    Break;
                  end;
                  if valuesStr = 'sum' then
                  begin
                    My_sum(str1, vRes, caclError, AInfo);
                    Break;
                  end;
                  if valuesStr = 'avg' then
                  begin
                    My_avg(str1, vRes, caclError, AInfo);
                    Break;
                  end;
                  if valuesStr = 'stddev' then
                  begin
                    My_stdDev(str1, vRes, caclError, AInfo);
                    Break;
                  end; // 统计实验标准偏差
                  if valuesStr = 'costh' then
                  begin
                    My_cosTh(str1, vRes, caclError, AInfo);
                    Break;
                  end;
                  if valuesStr = 'sn' then
                  begin
                    My_SumSubSeri(str1, vRes, caclError, AInfo);
                    Break;
                  end;
                  if valuesStr = 'sqn' then
                  begin
                    My_SumRatioSeri(str1, vRes, caclError, AInfo);
                    Break;
                  end;
                  if valuesStr = 's_tria' then
                  begin
                    My_AreaTriangle(str1, vRes, caclError, AInfo);
                    Break;
                  end;
                  if valuesStr = 'pdisplanes' then
                  begin
                    My_PlanesPointDis(str1, vRes, caclError, AInfo);
                    Break;
                  end;
                  if valuesStr = 'pdisspace' then
                  begin
                    My_SpacePointDis(str1, vRes, caclError, AInfo);
                    Break;
                  end;
                  if valuesStr = 'p_line' then
                  begin
                    My_PointToLineDis(str1, vRes, caclError, AInfo);
                    Break;
                  end;
                  if valuesStr = 'p_planes' then
                  begin
                    My_PointToPlanesDis(str1, vRes, caclError, AInfo);
                    Break;
                  end;
                  if valuesStr = 'morrep' then
                  begin
                    My_MorRep(str1, vRes, caclError, AInfo);
                    Break;
                  end;
                  if valuesStr = 's_circ' then
                  begin
                    if v1 > 0 then
                      vRes := PI * power(v1, 2)
                    else
                      SetError(caclError, AInfo, '圆半径必须为正数！');
                    Break;
                  end;
                  if valuesStr = 's_ellip' then
                  begin
                    funParamStr := '';
                    funParamStr := GetParamStr(str1, 2);
                    if funParamStr <> '' then
                    begin
                      v2 := StrToFloat(funParamStr);
                    end
                    else
                    begin
                      v2 := 0; // HZQ
                      SetError(caclError, AInfo, '椭圆参数有误！');
                    end;
                    if (v1 > 0) and (v2 > 0) then
                      vRes := PI * v1 * v2
                    else
                      SetError(caclError, AInfo, '椭圆长短半轴值必须为正数！');
                    Break;
                  end;
                  if valuesStr = 's_rect' then
                  begin
                    funParamStr := '';
                    funParamStr := GetParamStr(str1, 2);
                    if funParamStr <> '' then
                    begin
                      v2 := StrToFloat(funParamStr);
                    end
                    else
                    begin
                      v2 := 0; // HZQ
                      SetError(caclError, AInfo, '矩形长宽有误！');
                    end;
                    if (v1 > 0) and (v2 > 0) then
                      vRes := v1 * v2
                    else
                      SetError(caclError, AInfo, '矩形长宽必须为正数！');
                    Break;
                  end;
                  if valuesStr = 's_poly' then
                  begin
                    funParamStr := '';
                    funParamStr := GetParamStr(str1, 2);

                    if funParamStr <> '' then
                    begin
                      v2 := StrToFloat(funParamStr);
                    end
                    else
                    begin
                      v2 := 0;
                      SetError(caclError, AInfo, '正多边形参数有误！');
                    end;

                    if (v1 > 0) and (v2 >= 3) and (frac(v2) = 0) then
                      vRes := (v2 / 4) * power(v1, 2) * (1 / tan(PI / v2))
                    else
                      SetError(caclError, AInfo, '正多边形参数有误！');
                    Break;
                  end;
                end; // for  k := 1 to 1 do
                // *****************************************************************
                if not caclError then
                  Push(calcV, floatToStr(vRes));
              end // if
              else
                SetError(caclError, AInfo, '无法识别的函数！');
            end;
        else
          SetError(caclError, AInfo, '表达式有无效字符');
        end; // case
        Inc(i);
        if caclError then
          Break;
      end; // while
      if not caclError then
      begin
        calResult := StrToFloat(calcV.sv[calcV.top]);
        result := true;
      end;
    except
      caclError := true;
      result := false;
    end;
  finally
    av.sv.Free;
    sy.sv.Free;
    calcV.sv.Free;
  end;
end;

function CheckCalcExp(const ExpT: string; out AInfo: string): boolean;
var // 支持 + - * / %  +- ^ ()
  i, j, funS, funE, lenExpT, pairBktN, dotPos, commaN, fPN_given: integer; // Bkt: brackets 括号
  expError: boolean;
  aExpT, str1, lStr, rStr: string;
begin
  result := false;
  expError := false; // 默认表达式正确
  aExpT := StringReplace(ExpT, ' ', '', [rfReplaceAll]); // 去空格
  aExpT := StringReplace(aExpT, '%', '/(100)', [rfReplaceAll]); // 替换百分数符号 %
  aExpT := StringReplace(aExpT, '^', '*', [rfReplaceAll]); // 替换求幂符号 ^
  aExpT := StringReplace(aExpT, '!', '*(1*1)', [rfReplaceAll]); // 替换阶乘符号 !
  lenExpT := Length(aExpT);
  if lenExpT = 0 then
    Exit; // 表达式不正确时退出函数
  // ******************************************************************
  // 查找表达式是否含有指定的系统函数名 ，找到后用 (1)* 代替后再检查表达式的正确性
  // 参数个数与逗号的检查
  // 在这一步检查之间数学常数 e 或者 PI 必须做预处理，用值代替，否则可能会与函数名冲突
  if not expError then
  begin
    funS := 0;
    i := 1;

    while i <= lenExpT do
    begin
      if (funS = 0) and (AnsiChar(aExpT[i]) in ['a' .. 'z', 'A' .. 'Z', '_']) then
        funS := i;

      if (funS <> 0) and (aExpT[i] = '(') then
      begin
        funE := i;
        for j := funS to funE - 1 do
          if not(AnsiChar(aExpT[j]) in ['a' .. 'z', 'A' .. 'Z', '_', '0' .. '9']) then
          begin
            SetError(expError, AInfo, '函数名有误');
            Break;
          end;
        str1 := midStr(aExpT, funS, funE - funS);
        if IsCalcFun(str1, sMathFun) then
        begin // 函数名匹配时
          // 再检查函数参数个数是否正确
          pairBktN := 0;
          commaN := 0;
          for j := funE to lenExpT do
          begin
            if aExpT[j] = '(' then
              Inc(pairBktN); // 带匹配括号数加1
            if aExpT[j] = ')' then
              Dec(pairBktN); // 减去已经匹配的一对括号
            if (aExpT[j] = ',') and (pairBktN = 1) then
            begin
              Inc(commaN);
              aExpT[j] := '*'; // 本函数的逗号做替换预处理
            end;
            if pairBktN = 0 then
              Break;
          end;
          fPN_given := GetFunParamN(str1, sMathFun);
          if (pairBktN = 0) and (((commaN + 1) = fPN_given) or (fPN_given = 0)) then
          begin
            if (funS - 1) > 0 then
              lStr := LeftStr(aExpT, funS - 1)
            else
              lStr := '';
            if funE <= lenExpT then
              rStr := RightStr(aExpT, lenExpT - funE + 1)
            else
              rStr := '';
            aExpT := lStr + '(1)*' + rStr;
            lenExpT := Length(aExpT);
            i := funS;
          end
          else
            SetError(expError, AInfo, '函数参数个数不符！');
        end
        else
          SetError(expError, AInfo, '无法识别的函数名！');
        funS := 0;
      end;

      if expError then
        Break;
      Inc(i);
    end; // while
  end; // if
  // ******************************************************************
  // 检查表达式字符是否正确，字符范围：0~9  + - * / . ( )
  if not expError then
    for i := 1 to lenExpT do
      if not(AnsiChar(aExpT[i]) in (symbolSets + numberSets + ['(', ')', '.'])) then
      begin
        SetError(expError, AInfo, '表达式中有无效字符');
        Break;
      end;
  // ******************************************************************
  // 检查表达式中'('、')'是否正确
  if not expError then
  begin
    pairBktN := 0;
    for i := 1 to lenExpT do
    begin
      // 检查 '('、')'是否成对出现
      if aExpT[i] = '(' then
        Inc(pairBktN); // 带匹配括号数加1
      if aExpT[i] = ')' then
        Dec(pairBktN); // 减去已经匹配的一对括号
      if pairBktN < 0 then
      begin
        SetError(expError, AInfo, '表达式中括弧有误！');
        Break;
      end;
      // 检查 '('、')' 前后字符是否正确
      // '(' 之前 + - * / ( 有效， '(' 之后 + - ( 0~9 有效
      // ')' 之前 0~9 ) 有效， ')' 之后 + - * / ) 有效
      if aExpT[i] = '(' then
      begin
        if (i - 1) >= 1 then
          if not(AnsiChar(aExpT[i - 1]) in (symbolSets + ['('])) then
            SetError(expError, AInfo, '表达式中括弧前后有误！');
        if (i + 1) <= lenExpT then
          if not(AnsiChar(aExpT[i + 1]) in (numberSets + ['(', '+', '-'])) then
            SetError(expError, AInfo, '表达式中括弧前后有误！');
      end;
      if aExpT[i] = ')' then
      begin
        if (i - 1) >= 1 then
          if not(AnsiChar(aExpT[i - 1]) in (numberSets + [')'])) then
            SetError(expError, AInfo, '表达式中括弧前后有误！');
        if (i + 1) <= lenExpT then
          if not(AnsiChar(aExpT[i + 1]) in (symbolSets + [')'])) then
            SetError(expError, AInfo, '表达式中括弧前后有误！');
      end;
      if expError then
        Break;
    end; // for
    if pairBktN <> 0 then
      SetError(expError, AInfo, '表达式中括弧配对有误！');
  end; // if
  // ******************************************************************
  // 检查表达式中 + - * / 是否正确
  if not expError then
    for i := 1 to lenExpT do
    begin
      // 四则运算符号,开头首位不能出现 * / ，结尾不能出现 + - * /
      if (AnsiChar(aExpT[i]) in ['*', '/']) and (i = 1) then
        SetError(expError, AInfo, '表达式中首字符有误！');
      if (AnsiChar(aExpT[i]) in symbolSets) and (i = lenExpT) then
        SetError(expError, AInfo, '表达式中尾字符有误！');
      // 四则运算符号前后不能的符号
      // + - 符号之前 . 无效，之后 * /  ) . 无效
      // * / 符号之前 + - * / . ( 无效，之后 * / ) . 无效
      if (AnsiChar(aExpT[i]) in ['+', '-']) and ((i - 1) >= 1) and ((i + 1) <= lenExpT) then
        if (AnsiChar(aExpT[i - 1]) in ['.']) or (AnsiChar(aExpT[i + 1]) in ['*', '/', ')', '.']) then
          SetError(expError, AInfo, '表达式中运算符前后有误！');
      if (AnsiChar(aExpT[i]) in ['*', '/']) and ((i - 1) >= 1) and ((i + 1) <= lenExpT) then
        if (AnsiChar(aExpT[i - 1]) in (symbolSets + ['.', '('])) or (AnsiChar(aExpT[i + 1]) in ['*', '/', ')', '.']) then
          SetError(expError, AInfo, '表达式中运算符前后有误！');
      if expError then
        Break;
    end; // for
  // ******************************************************************
  // 检查表达式中 . 是否正确
  if not expError then
  begin
    dotPos := 0;
    for i := 1 to lenExpT do
    begin
      // 检查小数点是否出现在开头或结尾
      if ((i = 1) or (i = lenExpT)) and (aExpT[i] = '.') then
        SetError(expError, AInfo, '表达式中小数点出现在首尾！');
      // 检查相邻小数点之间是否为纯数字字符
      if aExpT[i] = '.' then
      begin
        if dotPos > 0 then
        begin
          if (i - dotPos) = 1 then
            SetError(expError, AInfo, '表达式中小数点相邻有误！'); // . 相邻时
          for j := (dotPos + 1) to (i - 1) do
            if not(AnsiChar(aExpT[j]) in numberSets) then
              Break // 跳出小循环
            else if j = (i - 1) then
              SetError(expError, AInfo, '表达式中小数点有误！');
        end;
        dotPos := i;
      end;
      if expError then
        Break; // 跳出大循环
    end; // for
  end; // if
  // ******************************************************************
  if not expError then
    result := true;
end;

procedure Init();
begin
  symbolSets := ['+', '-', '*', '/']; // 运算符号字符
  numberSets := ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9']; // 数字符号
  sMathFun := TStringList.Create;
  sMathFun.CommaText := 'sqrt(1),abs(1),exp(1),power(2),' +
    'sin(1),cos(1),tan(1),arcsin(1),arccos(1),arctan(1),degrad(1),raddeg(1),costh(3),' +
    's_tria(3),s_circ(1),s_ellip(2),s_rect(2),s_poly(2),' + 'log2(1),log10(1),logN(2),ln(1),' +
    'int(1),trunc(1),round(1),roundto(2),frac(1),mod(2),' + 'max(0),min(0),sum(0),avg(0),stddev(0),' + 'sn(3), sqn(3),' +
    'intax(1),arcintax(1),morrep(5),' + 'pdisplanes(4), pdisspace(6), p_line(5), p_planes(7)';
  InitSignParam();
end;

procedure FreeRes();
begin
  sMathFun.Free;
end;

{
  // 2017-02-28 修改了ReplaceSignParam函数的bug
}

initialization

Init();

finalization

FreeRes();

end.
