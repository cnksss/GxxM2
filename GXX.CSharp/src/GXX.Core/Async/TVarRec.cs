using System;

namespace GXX.Core.Async;

/// <summary>
/// Delphi RTL System.pas 的 <c>TVarRec.VType</c> 判别常量。
/// </summary>
/// <remarks>
/// <b>来源说明（重要）</b>：这些常量<b>并非</b>由 <c>AsyncCalls.pas</c> 或源码树中任何单元声明
/// —— 它们是 Delphi 编译器/<c>System</c> 单元的内置常量。已对
/// <c>Source/**/*.pas</c> 全树检索 <c>^\s*vt(Integer|Boolean|...)\s*=</c>，<b>零命中</b>，
/// 故无法“从原文抽取”；此处采用 Delphi 7 System.pas 的标准取值，并与原文
/// <c>AsyncCalls.pas:352-371</c> 的“Supported types”清单、
/// <c>AsyncCalls.pas:2337-2401</c> 的 asm <c>case</c> 分组
/// （4 字节 / 1 字节 / 2 字节 / 3 DWORD / 2 DWORD / 指针）交叉核对一致。
/// </remarks>
public static class TVarRecType
{
    /// <summary>System.pas：vtInteger = 0</summary>
    public const byte vtInteger = 0;

    /// <summary>System.pas：vtBoolean = 1</summary>
    public const byte vtBoolean = 1;

    /// <summary>System.pas：vtChar = 2</summary>
    public const byte vtChar = 2;

    /// <summary>System.pas：vtExtended = 3</summary>
    public const byte vtExtended = 3;

    /// <summary>System.pas：vtString = 4（PShortString）</summary>
    public const byte vtString = 4;

    /// <summary>System.pas：vtPointer = 5</summary>
    public const byte vtPointer = 5;

    /// <summary>System.pas：vtPChar = 6</summary>
    public const byte vtPChar = 6;

    /// <summary>System.pas：vtObject = 7</summary>
    public const byte vtObject = 7;

    /// <summary>System.pas：vtClass = 8</summary>
    public const byte vtClass = 8;

    /// <summary>System.pas：vtWideChar = 9</summary>
    public const byte vtWideChar = 9;

    /// <summary>System.pas：vtPWideChar = 10</summary>
    public const byte vtPWideChar = 10;

    /// <summary>System.pas：vtAnsiString = 11</summary>
    public const byte vtAnsiString = 11;

    /// <summary>System.pas：vtCurrency = 12</summary>
    public const byte vtCurrency = 12;

    /// <summary>System.pas：vtVariant = 13</summary>
    public const byte vtVariant = 13;

    /// <summary>System.pas：vtInterface = 14</summary>
    public const byte vtInterface = 14;

    /// <summary>System.pas：vtWideString = 15</summary>
    public const byte vtWideString = 15;

    /// <summary>System.pas：vtInt64 = 16</summary>
    public const byte vtInt64 = 16;

    /// <summary>
    /// System.pas（Delphi 2009+）：vtUnicodeString = 17。
    /// <para>
    /// 原文中该类型受 <c>{$IFDEF UNICODE}</c> 保护（2288、2300、2389 行），
    /// <b>Delphi 7 下不编译</b> —— 故本移植<b>不</b>把 17 纳入深拷贝集合，
    /// 使其自然落入 <c>CopyVarRec</c> 的 <c>else</c> 浅拷贝分支，与 D7 基线行为一致。
    /// </para>
    /// </summary>
    public const byte vtUnicodeString = 17;
}

/// <summary>
/// 原文 <c>TVarRec</c> 的托管替身（Delphi RTL 变体记录）。
/// </summary>
/// <remarks>
/// <para>
/// <b>布局差异（有意）</b>：Delphi 的 <c>TVarRec</c> 是 8 字节变体记录
/// —— <c>VType: Byte</c> + 3 字节对齐填充 + 4 字节联合体；asm 中
/// <c>[eax].TVarRec.VInteger</c> 位于 +4、<c>[eax].TVarRec.VBoolean</c> 读 +4 单字节。
/// 托管侧无法把托管引用叠放成 4 字节联合体，故用
/// <see cref="VType"/> + <see cref="VReservedPadding"/> + 单个 <see cref="VPointer"/> 槽
/// 表示：所有指针型成员（VString/VExtended/VCurrency/VInt64/VVariant/VInterface/
/// VAnsiString/VWideString/VObject/VClass/VPChar/VPWideChar/VPointer）共用
/// <see cref="VPointer"/> 槽，语义与 Delphi 联合体一致。
/// </para>
/// <para>
/// <see cref="VReservedPadding"/> 对应原文 VType 之后的 3 字节填充：它的存在使得
/// <c>Result := Data</c>（整体复制）与逐字段赋值（深拷贝分支）在托管侧
/// <b>可被观测地区分</b>，从而能对 <c>CopyVarRec</c> 的两条分支写差异断言。
/// </para>
/// </remarks>
public struct TVarRec
{
    /// <summary>原文 <c>TVarRec.VType</c>（联合体判别式，偏移 0）。</summary>
    public byte VType;

    /// <summary>
    /// 原文联合体中的指针槽（偏移 4）。<c>null</c> 对应 Delphi 的 <c>nil</c>，
    /// 即 <c>CopyVarRec</c> 里 <c>Data.VPointer &lt;&gt; nil</c> 判据。
    /// </summary>
    public object VPointer;

    /// <summary>
    /// 原文 VType 之后 3 字节对齐填充的替身；仅在“整体复制”分支被搬运。
    /// </summary>
    public int VReservedPadding;

    // -- 联合体成员的类型化视图（与 VPointer 同槽）------------------------------

    /// <summary>vtInteger: Arg: Integer</summary>
    public int VInteger
    {
        get => VPointer is int v ? v : 0;
        set => VPointer = value;
    }

    /// <summary>vtBoolean: Arg: Boolean（Delphi Boolean 为 1 字节）</summary>
    public byte VBoolean
    {
        get => VPointer is byte b ? b : (byte)0;
        set => VPointer = value;
    }

    /// <summary>vtChar: Arg: AnsiChar</summary>
    public byte VChar
    {
        get => VPointer is byte b ? b : (byte)0;
        set => VPointer = value;
    }

    /// <summary>vtExtended: 原文存 PExtended，此处存解引用后的值。</summary>
    public double VExtended
    {
        get => VPointer is double d ? d : 0.0;
        set => VPointer = value;
    }

    /// <summary>vtString: 原文存 PShortString，此处存解引用后的串。</summary>
    public string VString
    {
        get => VPointer as string;
        set => VPointer = value;
    }

    /// <summary>vtPointer: Arg: Pointer</summary>
    public object VPointerValue
    {
        get => VPointer;
        set => VPointer = value;
    }

    /// <summary>vtPChar: Arg: PChar</summary>
    public string VPChar
    {
        get => VPointer as string;
        set => VPointer = value;
    }

    /// <summary>vtObject: Arg: TObject</summary>
    public object VObject
    {
        get => VPointer;
        set => VPointer = value;
    }

    /// <summary>vtClass: Arg: TClass</summary>
    public Type VClass
    {
        get => VPointer as Type;
        set => VPointer = value;
    }

    /// <summary>vtWideChar: Arg: WideChar</summary>
    public char VWideChar
    {
        get => VPointer is char c ? c : '\0';
        set => VPointer = value;
    }

    /// <summary>vtPWideChar: Arg: PWideChar</summary>
    public string VPWideChar
    {
        get => VPointer as string;
        set => VPointer = value;
    }

    /// <summary>vtAnsiString: Arg: AnsiString</summary>
    public string VAnsiString
    {
        get => VPointer as string;
        set => VPointer = value;
    }

    /// <summary>vtCurrency: 原文存 PCurrency，此处存解引用后的值。</summary>
    public decimal VCurrency
    {
        get => VPointer is decimal m ? m : 0m;
        set => VPointer = value;
    }

    /// <summary>vtVariant: const Arg: Variant</summary>
    public object VVariant
    {
        get => VPointer;
        set => VPointer = value;
    }

    /// <summary>vtInterface: const Arg: IInterface</summary>
    public object VInterface
    {
        get => VPointer;
        set => VPointer = value;
    }

    /// <summary>vtWideString: Arg: WideString</summary>
    public string VWideString
    {
        get => VPointer as string;
        set => VPointer = value;
    }

    /// <summary>vtInt64: 原文存 PInt64，此处存解引用后的值。</summary>
    public long VInt64
    {
        get => VPointer is long l ? l : 0L;
        set => VPointer = value;
    }
}

/// <summary>
/// 原文 asm 中 <c>push</c> 的宽度/来源分类（<c>TAsyncCallArrayOfConst.ExecuteAsyncCall</c>，2337-2401）。
/// </summary>
public enum TVarRecPushKind
{
    /// <summary>vtInteger：<c>push [eax].TVarRec.VInteger</c>（4 字节值）</summary>
    Integer = 0,

    /// <summary>vtBoolean / vtChar：单字节零扩展入栈</summary>
    BooleanOrChar = 1,

    /// <summary>vtWideChar：双字节零扩展入栈</summary>
    WideChar = 2,

    /// <summary>vtExtended：解引用后压 3 个 DWORD（10 字节 80 位浮点）</summary>
    Extended = 3,

    /// <summary>vtCurrency / vtInt64：解引用后压 2 个 DWORD</summary>
    CurrencyOrInt64 = 4,

    /// <summary>其余受支持类型：直接 <c>push [eax].TVarRec.VPointer</c></summary>
    Pointer = 5,
}

/// <summary>原文 2334-2402 压栈循环中的一步（cdecl 自右向左）。</summary>
public struct TVarRecPushStep
{
    /// <summary>实参下标（0-based）。</summary>
    public int ArgIndex;

    /// <summary>该实参的压栈分类。</summary>
    public TVarRecPushKind Kind;

    /// <summary>原文对该类型额外累加的字节数（Extended +8，Currency/Int64 +4，其余 0）。</summary>
    public int ExtraStackBytes;
}

/// <summary>
/// 原文 <c>CopyVarRec</c> / <c>TAsyncCallArrayOfConst.ExecuteAsyncCall</c> 中的
/// <b>可移植纯逻辑</b>部分（判别式分类、深拷贝决策、栈字节记账）。
/// <para>
/// 接缝：实际的 x86 cdecl 压栈与调用（原文 2324-2332、2339-2408 的内联 asm）不可移植，
/// 由 <see cref="IVarRecInvoker"/> 承接。
/// </para>
/// </summary>
public static class TVarRecMarshal
{
    /// <summary>
    /// <c>CopyVarRec</c> 的深拷贝集合（原文 2287-2289）：
    /// <c>[vtString, vtAnsiString, vtWideString, vtExtended, vtCurrency, vtInt64, vtVariant, vtInterface]</c>。
    /// <para>原文的 <c>{$IFDEF UNICODE} vtUnicodeString</c> 属 Delphi 2009+，D7 下不在集合内。</para>
    /// </summary>
    public static bool IsDeepCopyType(byte VType)
    {
        return VType == TVarRecType.vtString
            || VType == TVarRecType.vtAnsiString
            || VType == TVarRecType.vtWideString
            || VType == TVarRecType.vtExtended
            || VType == TVarRecType.vtCurrency
            || VType == TVarRecType.vtInt64
            || VType == TVarRecType.vtVariant
            || VType == TVarRecType.vtInterface;
    }

    /// <summary>
    /// 原文 <c>TAsyncCallArrayOfConst.CopyVarRec</c>（2284-2313）1:1。
    /// <para>
    /// 原文语义：仅当 <c>Data.VPointer &lt;&gt; nil</c> <b>且</b> 类型属于深拷贝集合时，
    /// 才逐字段赋值（<c>Result.VType := ...; Result.VPointer := nil;</c> 后再复制负载）；
    /// <b>否则整体 <c>Result := Data</c></b> —— 含 VType 后的对齐填充。
    /// “指针为 nil 就不深拷贝”是原文的真实边界，不是笔误。
    /// </para>
    /// </summary>
    public static void CopyVarRec(in TVarRec Data, out TVarRec Result)
    {
        if (Data.VPointer != null && IsDeepCopyType(Data.VType))
        {
            Result = default;
            Result.VType = Data.VType;
            Result.VPointer = null;
            // 深拷贝负载。原文对指针型类型用 New(P) + 解引用赋值：
            //   vtString / vtExtended / vtCurrency / vtInt64 / vtVariant —— 新分配
            //   vtAnsiString / vtWideString / vtInterface —— 引用计数 +1（不新分配）
            // 托管侧：值类型负载重新装箱即“新分配”，引用类型负载直接共享引用。
            Result.VPointer = ClonePayload(Data.VType, Data.VPointer);
        }
        else
            Result = Data;
    }

    /// <summary>
    /// 深拷贝分支的负载复制。对值类型负载（vtExtended/vtCurrency/vtInt64/vtString/vtVariant）
    /// 返回<b>新的装箱实例</b>，对应原文的 <c>New(P)</c>；
    /// 对引用计数类型（vtAnsiString/vtWideString/vtInterface）返回同一引用，对应 AddRef。
    /// </summary>
    public static object ClonePayload(byte VType, object payload)
    {
        switch (VType)
        {
            case TVarRecType.vtAnsiString:
            case TVarRecType.vtWideString:
            case TVarRecType.vtInterface:
                return payload; // AddRef 语义：共享同一托管对象

            case TVarRecType.vtString:
                // PShortString：New + 逐字节复制 → 新的字符串实例
                return payload is string s ? new string(s.ToCharArray()) : payload;

            case TVarRecType.vtExtended:
            case TVarRecType.vtCurrency:
            case TVarRecType.vtInt64:
            case TVarRecType.vtVariant:
                // New(Result.VXxx); Result.VXxx^ := Data.VXxx^ ⇒ 新的存储
                return Rebox(payload);

            default:
                return payload;
        }
    }

    /// <summary>
    /// 为值类型负载分配新的装箱实例（对应原文 <c>New(P)</c> 的新存储）。
    /// 引用类型负载与未知类型按共享处理。
    /// </summary>
    public static object Rebox(object payload)
    {
        switch (payload)
        {
            case null: return null;
            case int v: return (object)v;
            case long v: return (object)v;
            case decimal v: return (object)v;
            case double v: return (object)v;
            case float v: return (object)v;
            case short v: return (object)v;
            case ushort v: return (object)v;
            case uint v: return (object)v;
            case ulong v: return (object)v;
            case byte v: return (object)v;
            case sbyte v: return (object)v;
            case bool v: return (object)v;
            case char v: return (object)v;
            case DateTime v: return (object)v;
            default: return payload;
        }
    }

    /// <summary>
    /// 原文析构函数 <c>TAsyncCallArrayOfConst.Destroy</c>（2258-2282）中释放各负载的分支。
    /// 托管侧只需解除引用；此处逐类型清空，保留原分支结构以便对照。
    /// </summary>
    public static void ReleaseVarRec(ref TVarRec Rec)
    {
        switch (Rec.VType)
        {
            case TVarRecType.vtAnsiString:
            case TVarRecType.vtWideString:
            // 原文如此（AsyncCalls.pas:2269-2271）：{$IFDEF UNICODE} vtUnicodeString —— D7 下不编译
            case TVarRecType.vtInterface:
            case TVarRecType.vtString:
            case TVarRecType.vtExtended:
            case TVarRecType.vtCurrency:
            case TVarRecType.vtInt64:
            case TVarRecType.vtVariant:
                Rec.VPointer = null;
                break;
        }
    }

    /// <summary>
    /// 原文 <c>ExecuteAsyncCall</c> 的参数计数式（2321）：
    /// <c>ByteCount := Length(FArgs) * SizeOf(Integer) + $40;</c>
    /// <c>$40</c> 是给“函数想要比给定参数更多参数”预留的零填充缓冲。
    /// </summary>
    public static int GetStackBufferByteCount(int argCount)
    {
        return argCount * sizeof(int) + 0x40;
    }

    /// <summary>
    /// 原文 asm <c>case V.VType</c> 的分类（2337-2401）。
    /// 未列出的类型走 <c>else UnknownVarRecType(V.VType)</c> → 抛 <see cref="TAsyncCallError"/>。
    /// </summary>
    public static TVarRecPushKind GetPushKind(byte VType)
    {
        switch (VType)
        {
            case TVarRecType.vtInteger:
                return TVarRecPushKind.Integer;

            case TVarRecType.vtBoolean:
            case TVarRecType.vtChar:
                return TVarRecPushKind.BooleanOrChar;

            case TVarRecType.vtWideChar:
                return TVarRecPushKind.WideChar;

            case TVarRecType.vtExtended:
                return TVarRecPushKind.Extended;

            case TVarRecType.vtCurrency:
            case TVarRecType.vtInt64:
                return TVarRecPushKind.CurrencyOrInt64;

            case TVarRecType.vtString:
            case TVarRecType.vtPointer:
            case TVarRecType.vtPChar:
            case TVarRecType.vtObject:
            case TVarRecType.vtClass:
            case TVarRecType.vtAnsiString:
            // 原文如此（AsyncCalls.pas:2388-2390）：{$IFDEF UNICODE} vtUnicodeString —— D7 下不编译。
            // 若运行期传入 17，将落入 else → UnknownVarRecType(17)，与 D7 基线一致。
            case TVarRecType.vtPWideChar:
            case TVarRecType.vtVariant:
            case TVarRecType.vtInterface:
            case TVarRecType.vtWideString:
                return TVarRecPushKind.Pointer;

            default:
                throw AsyncCallsConst.UnknownVarRecType(VType);
        }
    }

    /// <summary>
    /// 原文 asm 中对 <c>ByteCount</c> 的额外累加：<c>vtExtended</c> 为 <c>add [ByteCount], 8</c>，
    /// <c>vtCurrency/vtInt64</c> 为 <c>add [ByteCount], 4</c>，其余不累加。
    /// </summary>
    public static int GetExtraStackByteCount(byte VType)
    {
        switch (VType)
        {
            case TVarRecType.vtExtended:
                return 8;
            case TVarRecType.vtCurrency:
            case TVarRecType.vtInt64:
                return 4;
            default:
                return 0;
        }
    }

    /// <summary>
    /// 原文 2334-2402 的完整压栈序列（cdecl ⇒ 自右向左，<c>for I := High(FArgs) downto 0</c>）。
    /// 遇到未支持类型时与原文一样在该位置抛出 <c>UnknownVarRecType</c>。
    /// </summary>
    public static TVarRecPushStep[] BuildPushSequence(TVarRec[] Args)
    {
        if (Args == null)
            throw new ArgumentNullException(nameof(Args));

        int count = Args.Length;
        var steps = new TVarRecPushStep[count];
        int n = 0;
        for (int I = count - 1; I >= 0; I--) // cdecl => right to left
        {
            byte vType = Args[I].VType;
            TVarRecPushKind kind = GetPushKind(vType); // 未支持类型在此抛错，与原文等位
            steps[n].ArgIndex = I;
            steps[n].Kind = kind;
            steps[n].ExtraStackBytes = GetExtraStackByteCount(vType);
            n++;
        }
        return steps;
    }

    /// <summary>
    /// 压栈完成后 <c>add esp, [ByteCount]</c> 所用的总字节数：
    /// <c>argCount * 4 + $40 + Σ extra</c>。
    /// </summary>
    public static int ComputeStackByteCount(TVarRec[] Args)
    {
        int byteCount = GetStackBufferByteCount(Args == null ? 0 : Args.Length);
        if (Args == null)
            return byteCount;
        for (int i = 0; i < Args.Length; i++)
            byteCount += GetExtraStackByteCount(Args[i].VType);
        return byteCount;
    }
}
