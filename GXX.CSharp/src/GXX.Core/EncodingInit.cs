using System.Runtime.CompilerServices;
using System.Text;

namespace GXX.Core;

/// <summary>
/// 全局编码初始化（对应 Delphi 程序天然支持 GBK 的 AnsiString 语义）。
/// 通过 [ModuleInitializer] 在 GXX.Core 程序集加载时自动注册 GBK 代码页。
/// </summary>
public static class EncodingInit
{
    private static bool _done;

    [ModuleInitializer]
    internal static void ModuleInit() => Ensure();

    public static void Ensure()
    {
        if (_done) return;
        try
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
        catch { }
        _done = true;
    }

    private static Encoding? _gbk;

    /// <summary>GBK (CP936) 编码，对应 Delphi AnsiString 默认编码。</summary>
    public static Encoding GBK
    {
        get
        {
            Ensure();
            return _gbk ??= Encoding.GetEncoding(936);
        }
    }
}
